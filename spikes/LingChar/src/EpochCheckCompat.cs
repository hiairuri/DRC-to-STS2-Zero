using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace LingChar;

/// <summary>
/// 第三方兼容垫片（2026-10-03 日志实锤）：某个已加载 mod 给原版
/// SaveManager.UpdateProgressAfterCombatWon 内的
/// ProgressSaveManager.CheckFifteenElitesDefeatedEpoch 打了 Harmony 补丁
/// （栈帧名 _Patch1），其内部按自己已知的角色表逐个查 epoch——遇到
/// CHARACTER.LING 抛 ArgumentOutOfRangeException，异常沿
/// EndCombatInternal → CheckWinCondition 一路炸断战斗收尾流程＝
/// 处理区的牌永远不进结果堆（每次胜利必现）。
/// Harmony finalizer 挂在原版 SaveManager.UpdateProgressAfterCombatWon 上：
/// 捕获 ArgumentOutOfRangeException 即吞掉（返回 null），其余异常原样重抛。
/// 无论补丁来自哪个 mod、内部怎么写，胜利收尾不再被打断；代价只是该 mod 的
/// "十五精英"纪元检查对 Ling 不生效（本就查不到的角色）。
/// </summary>
[HarmonyPatch]
internal static class EpochCheckCompat
{
    [HarmonyFinalizer]
    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.UpdateProgressAfterCombatWon))]
    private static System.Exception? SwallowProgressCrash(System.Exception? __exception)
    {
        if (__exception is ArgumentOutOfRangeException)
        {
            Godot.GD.Print("[LingChar] swallowed third-party epoch-check crash in UpdateProgressAfterCombatWon (CHARACTER.LING not in their table)");
            return null;
        }
        return __exception;
    }
}
