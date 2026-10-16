using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

namespace LingChar;

/// <summary>
/// SelectWarm v8（2026-10-09，用户：首次卡顿明显，切换也卡顿明显，程度大致相同）。
/// v7 分段归因定案：~350ms 全部在 proc→postdraw（绘制提交段），且**每个新 bg
/// 实例的首次绘制都要重付**（spine 网格首建/纹理上传发生在 _draw，属实例私有，
/// 预热无法跨实例继承）——预热只能救"资源加载"，救不了"新实例首绘"。
/// v8 = 实例复用（根治）：bg 常驻入池，切角色只换可见性，杜绝 Instantiate+首绘+销毁：
///   1) AddChildSafely prefix：child 名带 "_bg" 且池中有同名热实例 → 换入热实例
///      （脱离旧父/复位 Modulate+ZIndex/可见），fresh 实例（从未绘制，释放零成本）
///      直接 QueueFree，跳过原方法；
///   2) QueueFreeSafely prefix："_bg" 节点且名字曾经过选人 bg 通道 → 不销毁，
///      回收入池（spine 网格/GPU 资源全保留），跳过原方法；
///   3) 预热实例不再销毁——画 2 帧后以 "SILENT_bg"/"LING_bg" 为键入池，
///      首次真实切换即复用（预热期已付首绘成本）。
///   名字按游戏约定 characterModel.Id.Entry+"_bg"；大小写不符时自然走原路径、
///   下次 QueueFreeSafely 按真实名回收（自愈）。IsInstanceValid 失效自动淘汰。
/// </summary>
internal static class BgInstancePool
{
    private const int Cap = 16;
    internal static readonly Dictionary<string, Node> Stash = new();

    internal static bool IsBgName(string name) => !string.IsNullOrEmpty(name) && name.EndsWith("_bg");

    internal static readonly HashSet<string> _seenBgNames = new();

    internal static void Put(string key, Node n)
    {
        if (Stash.ContainsKey(key))
        {
            var old = Stash[key];
            if (GodotObject.IsInstanceValid(old))
            {
                old.QueueFree(); // 同名换新，弃旧（理论上少见）
            }
        }
        Stash[key] = n;
        if (Stash.Count > Cap)
        {
            string evict = null;
            foreach (var kv in Stash)
            {
                if (kv.Key == key) continue;
                if (!GodotObject.IsInstanceValid(kv.Value)) { evict = kv.Key; break; }
                if (evict == null) evict = kv.Key;
            }
            if (evict != null)
            {
                var dead = Stash[evict];
                Stash.Remove(evict);
                if (GodotObject.IsInstanceValid(dead)) dead.QueueFree();
            }
        }
    }
}

[HarmonyPatch(typeof(GodotTreeExtensions), nameof(GodotTreeExtensions.AddChildSafely))]
internal static class AddChildSafelyReuse
{
    private static int _hits;

    private static bool Prefix(Node parent, Node child)
    {
        try
        {
            if (child == null || parent == null) return true;
            string key = child.Name.ToString();
            if (!BgInstancePool.IsBgName(key)) return true;
            BgInstancePool._seenBgNames.Add(key);
            if (!BgInstancePool.Stash.TryGetValue(key, out var warm))
                return true; // 池中无货：走原路径（新实例首绘，一次性成本）
            if (!GodotObject.IsInstanceValid(warm))
            {
                BgInstancePool.Stash.Remove(key);
                return true;
            }
            // 复用：换入热实例
            var wp = warm.GetParent();
            wp?.RemoveChild(warm);
            if (warm is CanvasItem ci)
            {
                ci.Modulate = Colors.White; // 复位预热期的 α=0.01
                ci.ZIndex = 0;              // 复位预热期的 -4096
                ci.Visible = true;
            }
            // v8.2：重置进场脚本——否则 _fired=true 永不重播 entry（用户回归：入场动画只播一次）。
            // _t_enter 会随重进树由 _enter_tree 自动复位；_ready 不重跑，故按原时序手动隐藏
            // SpineSprite，由 EntryPlayer._process 重新施加 entry 时点亮
            var entry = warm.FindChild("EntryPlayer", true, false);
            if (entry != null)
            {
                entry.Set("_fired", false);
                entry.Set("_t_visible", 0);
                entry.Set("_frames_n", 0);
                if (entry.GetParent() is CanvasItem spr)
                    spr.Visible = false;
            }
            warm.Name = key;
            parent.AddChild(warm, forceReadableName: false, Node.InternalMode.Disabled);
            BgInstancePool.Stash.Remove(key); // 已回树；下次 QueueFreeSafely 会再回收
            child.QueueFree();                // fresh 实例从未绘制，释放零成本
            if (_hits < 10)
            {
                _hits++;
                GD.Print($"[LingWarm] bg reuse hit: {key}（热实例换入，fresh 未绘制即弃）");
            }
            return false;
        }
        catch (System.Exception ex)
        {
            GD.Print("[LingWarm] AddChildSafely prefix EXC (fallthrough): ", ex.Message);
            return true;
        }
    }
}

[HarmonyPatch(typeof(GodotTreeExtensions), nameof(GodotTreeExtensions.QueueFreeSafely))]
internal static class QueueFreeSafelyStash
{
    private static int _stashed;

    private static bool Prefix(Node node)
    {
        try
        {
            if (node == null) return true;
            string key = node.Name.ToString();
            if (!BgInstancePool.IsBgName(key) || !BgInstancePool._seenBgNames.Contains(key))
                return true;
            BgInstancePool.Put(key, node); // 回收而非销毁（调用方已 RemoveChild）
            if (_stashed < 10)
            {
                _stashed++;
                GD.Print($"[LingWarm] bg stashed for reuse: {key}");
            }
            return false;
        }
        catch (System.Exception ex)
        {
            GD.Print("[LingWarm] QueueFreeSafely prefix EXC (fallthrough): ", ex.Message);
            return true;
        }
    }
}

[HarmonyPatch(typeof(AssetCache), nameof(AssetCache.GetScene))]
internal static class GetSceneWarmRedirect
{
    private static readonly HashSet<string> _redirectLogged = new();
    private static int _probes;

    private static bool Prefix(string path, ref PackedScene __result, out long __state)
    {
        __state = Stopwatch.GetTimestamp();
        try
        {
            var packed = SelectWarm.GetKeepAlive(path);
            if (packed != null && GodotObject.IsInstanceValid(packed))
            {
                __result = packed;
                if (_redirectLogged.Add(path))
                    GD.Print($"[LingWarm] GetScene redirect hit: {path}");
                return false; // 跳过原方法
            }
            return true; // 放行原方法
        }
        catch (System.Exception ex)
        {
            GD.Print("[LingWarm] GetScene prefix EXC (fallthrough): ", ex.Message);
            return true;
        }
    }

    private static void Postfix(string path, long __state, PackedScene __result)
    {
        try
        {
            if (!SelectWarm.IsProtectedPath(path)) return;
            if (_probes >= 14) return;
            _probes++;
            double ms = Stopwatch.GetElapsedTime(__state).TotalMilliseconds;
            var packed = SelectWarm.GetKeepAlive(path);
            string state = packed == null ? "keepAlive=null"
                : GodotObject.IsInstanceValid(packed) ? "keepAlive=valid"
                : "keepAlive=DISPOSED";
            GD.Print($"[LingWarm] GetScene '{path.GetFile()}' took {ms:F2}ms result={( __result == null ? "NULL" : "ok" )} {state}");
        }
        catch (System.Exception ex)
        {
            GD.Print("[LingWarm] GetScene postfix EXC: ", ex.Message);
        }
    }
}

[HarmonyPatch(typeof(AssetCache), nameof(AssetCache.UnloadAssets))]
internal static class UnloadAssetsProtector
{
    private static bool _logged;

    private static void Prefix(ref IEnumerable<string> assetsToUnloadSet)
    {
        try
        {
            if (assetsToUnloadSet == null) return;
            var snapshot = assetsToUnloadSet.ToList();
            int before = snapshot.Count;
            var filtered = snapshot.Where(p => !SelectWarm.IsProtectedPath(p)).ToList();
            if (before != filtered.Count && !_logged)
            {
                _logged = true;
                GD.Print($"[LingWarm] UnloadAssets filtered {before - filtered.Count} protected select-bg path(s)");
            }
            assetsToUnloadSet = filtered;
        }
        catch (System.Exception ex)
        {
            GD.Print("[LingWarm] UnloadAssets prefix EXC (fallthrough): ", ex.Message);
        }
    }
}

[HarmonyPatch(typeof(NCharacterSelectScreen), nameof(NCharacterSelectScreen.SelectCharacter))]
internal static class SelectCharacterTimer
{
    private static long _t0;
    private static int _count;

    private static void Prefix() => _t0 = Stopwatch.GetTimestamp();

    private static void Postfix(NCharacterSelectButton charSelectButton, CharacterModel characterModel)
    {
        try
        {
            if (_count >= 14) return;
            _count++;
            double ms = Stopwatch.GetElapsedTime(_t0).TotalMilliseconds;
            string who = charSelectButton != null && charSelectButton.IsRandom ? "random" : characterModel.Id.Entry;
            GD.Print($"[LingWarm] SelectCharacter({who}) total {ms:F2}ms  ← 用户感知口径");
        }
        catch (System.Exception ex)
        {
            GD.Print("[LingWarm] SelectCharacter postfix EXC: ", ex.Message);
        }
    }
}
