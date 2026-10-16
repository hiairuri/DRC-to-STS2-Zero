// ============================================================================
// ShadowSource_Archived.cs — 影子系统源码存档（2026-10-16 条目 61 废案定案）
// ============================================================================
// 状态：只读存档，NOT COMPILED（LingChar.csproj 无此文件；EnableDefaultCompileItems=false）。
// 来源：自 sts2_work/DRCSpineDye/dll/DyeFramework.cs 外科抽取的影子相关代码，
//       逐字保留（唯一改动：文件头说明 + 补一个最小 DyeFramework.Log shim 以保持自洽）。
// 其余特效（溶解 DeathWatcher / 闪红 HitFlash / 入场 / 攻击 AttackFxWatcher+MuzzleFollower /
// 命中 SlashImpactWatcher+HitBurstFX / FxCfg）已按条目 58 定性废案彻底删除。
//
// 【废案时的接线背景（重建时需重接的三处）】
// 1. 挂载入口：DyeFramework.OnNodeAdded → Sweep → TryAttach(sprite, skelPath)
//    读 res://drc_dye/profiles/<骨架路径>/config.json 的 shadow 段后调 AttachShadow。
//    config.json 键：enabled / mode("silhouette"|"blob") / texture / offset_y / size / alpha
//                    / oblique_deg / len_scale(旧 squash_y) / scale_x / opacity(旧 alpha)
//                    / feet_nudge_y / ground_from_bones / ground_offset_y / feet_bones
//                    / material / debug_red
// 2. 溶解联动：DeathWatcher 的死亡 tween 曾逐帧调 ShadowSilhouette.ActiveShadow?
//    .SetDissolve(d) 使影子随身体溶解同步淡出——存档后该联动已切断。
// 3. 材质：默认 res://drc_dye/profiles/ling/.../ling_shadow_material_sil.tres（已随资产树删除，
//    重建时需按当时方案重做；建档语义见 AttachShadow 头注与 WireDst 内注释）。
// ============================================================================

using Godot;

namespace DRCSpineDye;

// 存档 shim：原 DyeFramework 类中影子代码依赖的唯一成员（原 45 行逐字保留）。
internal static class DyeFramework
{
    internal static void Log(string msg) => GD.Print($"[{Time.GetTicksMsec()}ms] [DRCSpineDye] {msg}");
}

// ----------------------------------------------------------------------------
// 以下自原 DyeFramework 类（影子挂载三法，原 670-772 行）逐字抽取。
// ----------------------------------------------------------------------------
// --- shadow ------------------------------------------------------------
// mode dispatch: "silhouette" (default) = second SpineSprite sharing the
// skeleton data, full pose copied every tick, mirrored+squashed about the
// feet line - the true form of the mobile RT shadow (RTGlobalShadowActor
// renders the units into an RT, RTGlobalShadowMat projects it on the
// ground plane; the shadow IS the character's skin silhouette).
// "blob" = the old soft ellipse approximation.
internal static partial class ShadowArchiveHost
{
    public static void AttachShadow(Node sprite, Godot.Collections.Dictionary cfg, double feetSkelY)
    {
        try
        {
            if (!cfg.TryGetValue("enabled", out var en) || !en.AsBool()) return;
            var mode = cfg.TryGetValue("mode", out var m) ? m.AsString() : "silhouette";
            if (mode == "blob")
            {
                AttachShadowBlob(sprite, cfg);
                return;
            }
            // holder carries the timers and dies with the sprite; the drawn
            // node is a SIBLING of the sprite (children can be swallowed by
            // clip_children / parent redraw semantics)
            var holder = new ShadowSilhouette(sprite, cfg, (float)feetSkelY);
            sprite.AddChild(holder);
            holder.Activate();
        }
        catch (System.Exception e)
        {
            GD.PrintErr("[DRCSpineDye] shadow attach failed: " + e);
        }
    }

    private static void AttachShadowBlob(Node sprite, Godot.Collections.Dictionary cfg)
    {
        try
        {
            var texPath = cfg.TryGetValue("texture", out var tp) ? tp.AsString() : "";
            if (string.IsNullOrEmpty(texPath) || !ResourceLoader.Exists(texPath)) return;
            var tex = GD.Load<Texture2D>(texPath);
            var blob = new ShadowBlob { Texture = tex, Target = sprite };
            blob.OffsetY = cfg.TryGetValue("offset_y", out var oy) ? (float)oy.AsDouble() : 0f;
            if (cfg.TryGetValue("size", out var sz) && sz.VariantType == Variant.Type.Array)
            {
                var arr = sz.AsGodotArray();
                if (arr.Count == 2)
                {
                    var wpx = (float)arr[0].AsDouble();
                    var hpx = (float)arr[1].AsDouble();
                    blob.Scale = new Vector2(wpx / tex.GetWidth(), hpx / tex.GetHeight());
                }
            }
            var alpha = cfg.TryGetValue("alpha", out var al) ? (float)al.AsDouble() : 0.55f;
            blob.Modulate = new Color(0f, 0f, 0f, alpha);
            // same z as the SpineSprite (tree order already puts the blob just
            // before it): above the arena floor, below the character. The z999
            // diagnostic proved the earlier invisibility was a layer issue.
            blob.ZIndex = sprite is CanvasItem ci ? ci.ZIndex : 0;

            var parent = sprite.GetParent();
            if (parent != null)
            {
                parent.AddChild(blob);
                parent.MoveChild(blob, sprite.GetIndex());
            }
            else
            {
                sprite.AddChild(blob);
                blob.ShowBehindParent = true;
            }
            GD.Print($"[DRCSpineDye] shadow attached ({texPath}, alpha={alpha:0.00}, parent={parent?.Name ?? "?"}, idx={blob.GetIndex()})");

            // follow tick (no _Process: no source generators in this assembly)
            var tick = new Timer { WaitTime = 1.0 / 30.0, Autostart = true };
            blob.AddChild(tick);
            tick.Timeout += blob.Follow;
            blob.Follow();
            ReportBlobLater(blob, sprite);
        }
        catch (System.Exception e)
        {
            GD.PrintErr("[DRCSpineDye] shadow attach failed: " + e);
        }
    }

    private static void ReportBlobLater(Node blob, Node sprite)
    {
        var timer = new Timer { WaitTime = 3.0, OneShot = true, Autostart = true };
        blob.AddChild(timer);
        timer.Timeout += () =>
        {
            try
            {
                if (!GodotObject.IsInstanceValid(blob)) return;
                var n2 = blob as Node2D;
                var s2 = sprite as Node2D;
                GD.Print($"[DRCSpineDye] shadow probe: blob_gpos={n2?.GlobalPosition}, blob_gscale={n2?.GlobalScale}, " +
                         $"visible={n2?.Visible}, in_tree={blob.IsInsideTree()}, sprite_gpos={s2?.GlobalPosition}, sprite_gscale={s2?.GlobalScale}");
            }
            catch (System.Exception e)
            {
                GD.PrintErr("[DRCSpineDye] shadow probe failed: " + e);
            }
        };
    }
}

/// <summary>Soft ground blob that follows the character (timer-driven;
/// C# _Process overrides are not registered in this assembly).</summary>
/// （原 DyeFramework.cs 1277-1289 行逐字保留）
public class ShadowBlob : Sprite2D
{
    public Node? Target;
    public float OffsetY;

    public void Follow()
    {
        if (Target is Node2D n2 && GodotObject.IsInstanceValid(n2) && IsInsideTree())
            GlobalPosition = new Vector2(n2.GlobalPosition.X, n2.GlobalPosition.Y + OffsetY);
    }
}

// ----------------------------------------------------------------------------
// ShadowSilhouette — 原 DyeFramework.cs 2955-3439 行逐字保留。
// 剪影影子：第二 SpineSprite 共享骨架数据，逐 tick 全量复制骨骼位姿，
// 以斜二测画法（cabinet projection）向地面投影。生命周期三态机
// CreateDst → WireDst → Sync，含 Rebuild（死亡时容器被游戏释放后的重建）。
// ----------------------------------------------------------------------------
public class ShadowSilhouette : Node2D
{
    // the live instance, for cross-component pose probes (DeathWatcher)
    public static ShadowSilhouette? ActiveShadow;

    private readonly Node _target;
    private readonly float _scaleX, _opacity, _nudgeY, _feetSkelY, _obliqueDeg, _lenScale;
    private readonly string _matPath;
    private readonly bool _debugRed;
    // DRC's RT shadow lands on a GROUND PLANE, not on the skeleton's static
    // bounds - the feet are not necessarily touching the ground (shop/event
    // scenes pose the character differently). The ground line is therefore
    // MEASURED from the foot bones every tick, with the skeleton bounds as
    // fallback when the bones can't be found.
    private readonly string[] _feetBones;
    private readonly bool _groundFromBones;
    private readonly float _groundOffsetY;
    private GodotObject[]? _srcFootBones;
    private Node? _dst;
    private GodotObject? _srcSkel, _dstSkel;
    private GodotObject[]? _srcBones, _dstBones, _srcSlots, _dstSlots;
    private ShaderMaterial? _mat;
    private float _dissolve;
    private Timer? _poll, _tick, _probe, _check;
    private int _phase, _tries, _syncTries, _checks, _groundLogs;
    private ulong _groundLogMs;
    private bool _warned;

    internal static float BoneRot(GodotObject? skel, string bone)
    {
        var b = skel?.Call("find_bone", bone).AsGodotObject();
        return b != null ? b.Call("get_rotation").AsSingle() : float.NaN;
    }

    internal void PoseCheck(string tag)
    {
        try
        {
            if (_dst == null || !GodotObject.IsInstanceValid(_dst)) return;
            var sk = _target.Call("get_skeleton").AsGodotObject();
            var st = _target.Get("animation_state").AsGodotObject()
                     ?? _target.Call("get_animation_state").AsGodotObject();
            var ao = st?.Call("get_current", 0).AsGodotObject()?.Call("get_animation").AsGodotObject();
            var anim = ao != null ? ao.Call("get_name").AsString() : "";
            DyeFramework.Log($"pose[{tag}] anim={anim} " +
                     $"src(b10={BoneRot(sk, "bone10"):0.0}/b11={BoneRot(sk, "bone11"):0.0}) " +
                     $"dst(b10={BoneRot(_dstSkel, "bone10"):0.0}/b11={BoneRot(_dstSkel, "bone11"):0.0})");
        }
        catch (System.Exception e)
        {
            GD.PrintErr("[DRCSpineDye] pose check failed: " + e.Message);
        }
    }

    public ShadowSilhouette(Node target, Godot.Collections.Dictionary cfg, float feetSkelY)
    {
        _target = target;
        _feetSkelY = feetSkelY;
        // ground projection = 斜二测画法 (cabinet/oblique drawing): vertical
        // extent falls onto the ground at 45 deg with HALF length; horizontal
        // extent keeps true length. oblique_deg=90 + the old squash_y value
        // reproduces the previous straight-down mirror exactly.
        _obliqueDeg = cfg.TryGetValue("oblique_deg", out var od) ? (float)od.AsDouble() : 45f;
        float ls = cfg.TryGetValue("len_scale", out var lv) ? (float)lv.AsDouble() : -1f;
        if (ls < 0) ls = cfg.TryGetValue("squash_y", out var sq) ? (float)sq.AsDouble() : 0.5f;
        _lenScale = ls;
        _scaleX = cfg.TryGetValue("scale_x", out var sx) ? (float)sx.AsDouble() : 1.0f;
        float op = cfg.TryGetValue("opacity", out var opv) ? (float)opv.AsDouble() : -1f;
        if (op < 0) op = cfg.TryGetValue("alpha", out var al) ? (float)al.AsDouble() : 0.55f;
        _opacity = op;
        _nudgeY = cfg.TryGetValue("feet_nudge_y", out var ny) ? (float)ny.AsDouble() : 0f;
        // DRC ground-plane philosophy: the ground is SCENE geometry - the game
        // places the character node ON the floor, so the ground line defaults
        // to the node's own Y (+ small offset for the sole). Bone-based
        // measurement is opt-in only (bone origins sit at the ankle, not the
        // sole, and the trailing dress measures below the feet).
        _groundFromBones = cfg.TryGetValue("ground_from_bones", out var gfb) && gfb.AsBool();
        _groundOffsetY = cfg.TryGetValue("ground_offset_y", out var gy) ? (float)gy.AsDouble() : 4f;
        var fb = new System.Collections.Generic.List<string>();
        if (cfg.TryGetValue("feet_bones", out var fbv) && fbv.VariantType == Variant.Type.Array)
            foreach (var b in fbv.AsGodotArray()) fb.Add(b.AsString());
        if (fb.Count == 0)
        {
            fb.Add("bone43");  // zeroR_youjiao_z (right foot)
            fb.Add("bone46");  // zeroL_zuojiao_z (left foot)
        }
        _feetBones = fb.ToArray();
        _matPath = cfg.TryGetValue("material", out var mp) ? mp.AsString()
            : "res://drc_dye/profiles/ling/animations/characters/silent/ling_shadow_material_sil.tres";
        _debugRed = cfg.TryGetValue("debug_red", out var dr) && dr.AsBool();
        Visible = false;
        ZIndex = target is CanvasItem ci ? ci.ZIndex : 0;
    }

    public void Activate()
    {
        // the holder dies with the character sprite; drag the drawn node
        // (its SIBLING) along so a scene switch cannot orphan it
        TreeExiting += () =>
        {
            try
            {
                if (_dst != null && GodotObject.IsInstanceValid(_dst)) _dst.QueueFree();
            }
            catch { /* contained */ }
        };
        _poll = new Timer { WaitTime = 0.2, Autostart = true };
        AddChild(_poll);
        _poll.Timeout += Tick;
    }

    /// <summary>
    /// Dissolve-synced fade: the death tween drives this with the SAME value
    /// it feeds the character's dissolve shader, so the shadow's opacity
    /// (configured 0.8) erases linearly in step with the body's erosion.
    /// _dissolve survives a mid-death Rebuild (WireDst re-applies it).
    /// </summary>
    public void SetDissolve(float d)
    {
        _dissolve = Mathf.Clamp(d, 0f, 1f);
        if (_mat != null)
            _mat.SetShaderParameter("opacity", _opacity * (1f - _dissolve));
    }

    internal void LogPair(string tag, string anim, float sr10, float sr11)
    {
        try
        {
            DyeFramework.Log($"pose[{tag}] anim={anim} " +
                     $"src(b10={sr10:0.0}/b11={sr11:0.0}) " +
                     $"dst(b10={BoneRot(_dstSkel, "bone10"):0.0}/b11={BoneRot(_dstSkel, "bone11"):0.0})");
        }
        catch { /* contained */ }
    }

    private void Fail(string why)
    {
        if (!_warned)
        {
            _warned = true;
            GD.PrintErr("[DRCSpineDye] silhouette shadow unavailable: " + why);
        }
        Shutdown("target invalid (tick)");
    }

    private void Shutdown(string why = "unspecified")
    {
        try
        {
            if (ActiveShadow == this)
            {
                ActiveShadow = null;
                DyeFramework.Log("shadow shutdown: " + why);
            }
            _poll?.QueueFree(); _poll = null;
            _tick?.QueueFree(); _tick = null;
            _check?.QueueFree(); _check = null;
            if (_dst != null && GodotObject.IsInstanceValid(_dst)) _dst.QueueFree();
            _dst = null;
            // drop cached native wrappers immediately: they must not outlive
            // the freed skeleton into the GC finalizer (a finalizer disposing
            // a wrapper whose native the scene already freed is a process-level
            // ExecutionEngineException - crash dump 23:56, finalizer thread)
            _srcSkel = null; _dstSkel = null;
            _srcBones = null; _dstBones = null;
            _srcSlots = null; _dstSlots = null;
            QueueFree();
        }
        catch { /* contained */ }
    }

    private void Tick()
    {
        try
        {
            if (!GodotObject.IsInstanceValid(_target)) { Shutdown("target invalid (tick)"); return; }
            if (_phase == 0) CreateDst();
            else if (_phase == 1) WireDst();
            else Sync();
        }
        catch (System.Exception e)
        {
            Fail("tick: " + e.Message);
        }
    }

    private void CreateDst()
    {
        if (++_tries > 100) { Fail("source skeleton never appeared"); return; }
        var skel = _target.Call("get_skeleton").AsGodotObject();
        if (skel == null) return; // skeleton still loading
        var res = skel.Call("get_data").AsGodotObject();
        if (res == null) return;
        if (!ClassDB.ClassExists("SpineSprite")) { Fail("SpineSprite not in ClassDB"); return; }
        var parent = _target.GetParent();
        if (parent == null) { Fail("sprite has no parent"); return; }
        var dstObj = ClassDB.Instantiate("SpineSprite").AsGodotObject();
        if (dstObj is not Node dstNode) { Fail("instantiate SpineSprite failed"); return; }
        // our own sweep must never dye-attach this sprite
        dstNode.SetMeta("drc_dye_attached", true);
        parent.AddChild(dstNode);
        // immediately before the character in tree order: drawn above the
        // arena, below the character
        parent.MoveChild(dstNode, _target.GetIndex());
        dstNode.Set("visible", false);
        dstNode.Call("set_skeleton_data_res", res);
        _dst = dstNode;
        _phase = 1;
        _tries = 0;
        if (_poll != null) _poll.WaitTime = 0.05;
        GD.Print("[DRCSpineDye] silhouette shadow sprite created, waiting for skeleton");
    }

    private void WireDst()
    {
        if (_dst == null || ++_tries > 100) { Fail("dst skeleton never appeared"); return; }
        var dskel = _dst.Call("get_skeleton").AsGodotObject();
        if (dskel == null) return;
        var sskel = _target.Call("get_skeleton").AsGodotObject();
        if (sskel == null) { Fail("src skeleton vanished"); return; }
        var sb = sskel.Call("get_bones").AsGodotArray();
        var db = dskel.Call("get_bones").AsGodotArray();
        var ss = sskel.Call("get_slots").AsGodotArray();
        var ds = dskel.Call("get_slots").AsGodotArray();
        if (sb.Count != db.Count || ss.Count != ds.Count || sb.Count == 0)
        {
            Fail($"bone/slot count mismatch src=({sb.Count},{ss.Count}) dst=({db.Count},{ds.Count})");
            return;
        }
        _srcSkel = sskel; _dstSkel = dskel;
        _srcBones = ObjArray(sb); _dstBones = ObjArray(db);
        _srcSlots = ObjArray(ss); _dstSlots = ObjArray(ds);
        // ground-line measurement bones (src side only - the mirror line is
        // computed from where the character's feet actually ARE)
        var footList = new System.Collections.Generic.List<GodotObject>();
        foreach (var fbName in _feetBones)
        {
            var fbObj = sskel.Call("find_bone", fbName).AsGodotObject();
            if (fbObj != null) footList.Add(fbObj);
        }
        _srcFootBones = footList.ToArray();
        // slots: attachment/deform are copied ONCE here. Per-tick getters
        // returning non-trivial Variants (Object refs, PackedFloat32Array)
        // leak native buffers - nothing disposes them, 50 slots x 60 Hz grew
        // unbounded until the game crashed on a long session (log: repeated
        // '_internal_spine_objects_invalidated already connected' from the
        // same calls). This skin never swaps attachments mid-battle, so the
        // one-shot copy is exact; the per-tick sync only pushes inline
        // scalars (floats/colors), which live inside the Variant value.
        for (int i = 0; i < _srcSlots.Length; i++)
        {
            try
            {
                _dstSlots[i].Call("set_attachment", _srcSlots[i].Call("get_attachment"));
                _dstSlots[i].Call("set_deform", _srcSlots[i].Call("get_deform"));
            }
            catch (System.Exception se)
            {
                GD.PrintErr($"[DRCSpineDye] slot {i} initial attachment copy failed: {se.Message}");
            }
        }
        // silhouette material: the fresh sprite's own materials may be null
        // (the game always sets them through the MegaSpine pipeline) - null
        // materials could mean nothing drawn at all. All four slots families
        // get the same flat-tint material, like the mobile RT shadow which
        // renders the lit units as-is.
        try
        {
            var mat = GD.Load<ShaderMaterial>(_matPath);
            if (mat != null)
            {
                _mat = mat;
                mat.SetShaderParameter("tint", _debugRed ? new Color(1f, 0f, 0f, 1f) : new Color(0f, 0f, 0f, 1f));
                mat.SetShaderParameter("opacity", (_debugRed ? 1.0f : _opacity) * (1f - _dissolve));
                _dst.Set("normal_material", mat);
                _dst.Set("additive_material", mat);
                _dst.Set("multiply_material", mat);
                _dst.Set("screen_material", mat);
            }
            else
            {
                Fail("silhouette material not found: " + _matPath);
                return;
            }
        }
        catch (System.Exception me)
        {
            Fail("silhouette material load failed: " + me.Message);
            return;
        }
        // the character's z - the arena floor is drawn ABOVE z=0 canvas items,
        // a shadow left at the default 0 gets swallowed (same lesson as the
        // old blob's ZIndex=-1 incident)
        if (_dst is CanvasItem dc && _target is CanvasItem tc) dc.ZIndex = tc.ZIndex;
        if (_poll != null) _poll.Stop();
        _tick = new Timer { WaitTime = 1.0 / 60.0, Autostart = true };
        AddChild(_tick);
        _tick.Timeout += Sync;
        ActiveShadow = this;
        // baseline follow check: do the copied bone values track the source
        // during plain idle breathing? (first ~16 s, then quiet)
        _check = new Timer { WaitTime = 2.0, Autostart = true };
        AddChild(_check);
        _check.Timeout += () =>
        {
            if (_checks >= 8) { _check?.Stop(); return; }
            _checks++;
            PoseCheck($"idle-check-{_checks}");
        };
        // one-shot probe: transforms as the scene sees them, for occlusion triage
        _probe = new Timer { WaitTime = 3.0, OneShot = true, Autostart = true };
        AddChild(_probe);
        _probe.Timeout += () =>
        {
            try
            {
                if (_dst == null || !GodotObject.IsInstanceValid(_dst)) return;
                var d2 = _dst as Node2D; var t2 = _target as Node2D;
                GD.Print($"[DRCSpineDye] silhouette probe: dst_gpos={d2?.GlobalPosition} dst_gscale={d2?.GlobalScale} " +
                         $"dst_z={( _dst as CanvasItem)?.ZIndex} dst_visible={d2?.Visible} dst_in_tree={_dst.IsInsideTree()} " +
                         $"src_z={( _target as CanvasItem)?.ZIndex} src_gpos={t2?.GlobalPosition} src_visible={t2?.Visible}");
            }
            catch (System.Exception pe) { GD.PrintErr("[DRCSpineDye] silhouette probe failed: " + pe.Message); }
        };
        DyeFramework.Log($"silhouette shadow ready: bones={_srcBones.Length} slots={_srcSlots.Length} " +
                 $"oblique_deg={_obliqueDeg:0} len_scale={_lenScale:0.00} scale_x={_scaleX:0.00} opacity={(_debugRed ? 1f : _opacity):0.00} " +
                 $"feet_skel_y={_feetSkelY:0.0} mat={_matPath} debug_red={_debugRed}");
    }

    private static GodotObject[] ObjArray(Godot.Collections.Array a)
    {
        var r = new GodotObject[a.Count];
        for (int i = 0; i < a.Count; i++) r[i] = a[i].AsGodotObject();
        return r;
    }

    // dst was freed out from under us (container teardown at death): drop the
    // dead references and re-run the create/wire state machine against the
    // character's CURRENT parent
    private void Rebuild()
    {
        try
        {
            DyeFramework.Log("shadow dst lost - rebuilding");
            if (_tick != null && GodotObject.IsInstanceValid(_tick)) _tick.Stop();
            _dst = null; _dstSkel = null; _dstBones = null; _dstSlots = null;
            _phase = 0; _tries = 0; _syncTries = 0;
            if (_poll == null || !GodotObject.IsInstanceValid(_poll))
            {
                _poll = new Timer { WaitTime = 0.05, Autostart = true };
                AddChild(_poll);
                _poll.Timeout += Tick;
            }
            else
            {
                _poll.Start();
            }
        }
        catch (System.Exception e)
        {
            Fail("rebuild: " + e.Message);
        }
    }

    private void Sync()
    {
        if (_dst == null || !GodotObject.IsInstanceValid(_dst) || !GodotObject.IsInstanceValid(_target))
        {
            // at death the game frees the character's original container - our
            // sibling dst dies with it while the character itself survives
            // (dsample log: sprite_valid=True at the same moment). Rebuild.
            if (GodotObject.IsInstanceValid(_target))
            {
                Rebuild();
                return;
            }
            Shutdown("dst/target invalid (sync)");
            return;
        }
        try
        {
            if (++_syncTries >= 120)
            {
                _syncTries = 0;
                if (_dst.GetParent() == null || !_dst.IsInsideTree()) { Shutdown("dst not in tree (watchdog)"); return; }
            }
            bool vis = _target.Get("visible").AsBool();
            // the DRAWN node is _dst (a sibling), not this holder - the first
            // build lost exactly here: the holder's visibility was synced and
            // the shadow sprite stayed visible=false forever (probe: dst_visible=False)
            if (_dst is Node2D d2v && d2v.Visible != vis) d2v.Visible = vis;
            if (!vis) return;
            if (_srcBones == null || _dstBones == null || _srcSlots == null || _dstSlots == null) return;

            // full pose: local values (what timelines write) AND applied values
            // (what constraints write) - whichever family the runtime's
            // update_world_transform consumes, the copied pose takes effect
            for (int i = 0; i < _srcBones.Length; i++)
            {
                var s = _srcBones[i]; var d = _dstBones[i];
                d.Call("set_x", s.Call("get_x"));
                d.Call("set_y", s.Call("get_y"));
                d.Call("set_rotation", s.Call("get_rotation"));
                d.Call("set_scale_x", s.Call("get_scale_x"));
                d.Call("set_scale_y", s.Call("get_scale_y"));
                d.Call("set_shear_x", s.Call("get_shear_x"));
                d.Call("set_shear_y", s.Call("get_shear_y"));
                d.Call("set_a_x", s.Call("get_a_x"));
                d.Call("set_a_y", s.Call("get_a_y"));
                d.Call("set_applied_rotation", s.Call("get_applied_rotation"));
                d.Call("set_a_scale_x", s.Call("get_a_scale_x"));
                d.Call("set_a_scale_y", s.Call("get_a_scale_y"));
                d.Call("set_a_shear_x", s.Call("get_a_shear_x"));
                d.Call("set_a_shear_y", s.Call("get_a_shear_y"));
            }
            // slots: color only (inline Color Variant - no native heap).
            // debug_red: fully opaque (nothing may dim or fade the
            // diagnostic). final look: black rgb is ignored by the material
            // (it flattens tint anyway), slot alpha carries the die fade so
            // the shadow dissolves with the character. attachments/deforms
            // were copied once at wire time (see WireDst).
            for (int i = 0; i < _srcSlots.Length; i++)
            {
                var s = _srcSlots[i]; var d = _dstSlots[i];
                if (_debugRed)
                {
                    d.Call("set_color", new Color(1f, 1f, 1f, 1f));
                }
                else
                {
                    float a = s.Call("get_color").AsColor().A;
                    d.Call("set_color", new Color(0f, 0f, 0f, a));
                }
            }
            // skeleton-level alpha: carries the game's hit-flash color alpha
            // (rgb=1 no-ops on the flattened tint)
            float sa = _srcSkel!.Call("get_color").AsColor().A;
            _dstSkel!.Call("set_color", new Color(1f, 1f, 1f, _debugRed ? 1f : sa));
            // bake world transforms + redraw in case this build's SpineSprite
            // does not self-update an idle animation state
            _dstSkel.Call("update_world_transform", 0);
            _dst.Call("queue_redraw");
            // keep the character's z (the game may raise it mid-battle)
            if (_dst is CanvasItem dc2 && _target is CanvasItem tc2 && dc2.ZIndex != tc2.ZIndex)
                dc2.ZIndex = tc2.ZIndex;

            // ground projection, 斜二测画法: a point at offset (ex, ey) from
            // the feet line maps to (ex, 0) + len_scale*(-ey)*(cosθ, sinθ) -
            // vertical extent falls at θ=45° with half length, horizontal
            // stays true. The feet line = the LOWEST VISUAL POINT of the
            // DRC ground-plane: the ground is SCENE geometry, not a property
            // of the pose. The game places the character node ON the floor,
            // so the ground line is simply the node's own Y (+ sole offset).
            // Guessing from the pose (foot bones / bounds / dress hem) always
            // breaks somewhere - the dress hem alone anchors the mirror line
            // half a body behind the feet.
            if (_target is Node2D t2 && _dst is Node2D d2)
            {
                var nsc = t2.Scale;
                float th = _obliqueDeg * Mathf.Pi / 180f;
                float kx = _lenScale * Mathf.Cos(th);
                float ky = _lenScale * Mathf.Sin(th);
                float feetLocal = _groundOffsetY;
                if (_groundFromBones && _srcFootBones != null && _srcFootBones.Length > 0)
                {
                    float best = float.NaN;
                    foreach (var fb in _srcFootBones)
                    {
                        float fy = fb.Call("get_world_y").AsSingle() * nsc.Y;
                        if (float.IsNaN(best) || fy > best) best = fy; // lowest on screen
                    }
                    if (!float.IsNaN(best)) feetLocal = best;
                }
                d2.Transform = new Transform2D(
                    new Vector2(nsc.X * _scaleX, 0f),
                    new Vector2(-kx * nsc.Y, -ky * nsc.Y),
                    new Vector2(t2.Position.X + kx * feetLocal, t2.Position.Y + feetLocal + ky * feetLocal));
            }
        }
        catch (System.Exception e)
        {
            Fail("sync: " + e.Message);
        }
    }
}
