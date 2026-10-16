using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Events;

namespace LingChar;

/// <summary>
/// 建筑师对话兼容（2026-10-03 两轮定案）：通关后建筑师对话的角色对话表只有
/// 原版五角色——mod 角色 LoadDialogue 取不到 → Dialogue=null。
/// 第一轮：只在 Dialogue=null 时垫 `new AncientDialogue("")` 不够——空构造的行
/// LineText=null 且 Speaker=None，OnRoomEnter 会因"对话非空且有行"清空选项，
/// 而 PlayCurrentLine 在 `LineText == null` 处提前返回、永远走不到尾部
/// SetEventState（重设按钮）→ 界面上没有任何可点的东西=无报错纯逻辑卡死。
/// 终版：垫行带文本（characters 表 LING.architectLine，中英齐备）、说话人=
/// Character（GetSpeaker 返回玩家生物非空）——PlayCurrentLine 完整走通：
/// 气泡显示文本 → SetEventState(继续按钮) → PROCEED → WinRun → SetLocalPlayerReady
/// → 正常结算。StartAttackers/EndAttackers 默认 None=无攻击演出。
/// </summary>
[HarmonyPatch]
internal static class ArchitectDialogueCompat
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TheArchitect), "LoadDialogue")]
    private static void LoadDialogue_Postfix(TheArchitect __instance)
    {
        try
        {
            var field = AccessTools.Field(typeof(TheArchitect), "_dialogue");
            if (field?.GetValue(__instance) != null) return;
            var dialogue = new AncientDialogue("");
            var line = dialogue.Lines[0];
            line.LineText = new LocString("characters", "LING.architectLine");
            line.Speaker = AncientDialogueSpeaker.Character;
            field.SetValue(__instance, dialogue);
            Godot.GD.Print("[LingChar] architect dialogue null (modded character), substituted minimal spoken line");
        }
        catch (Exception e)
        {
            Godot.GD.PrintErr("[LingChar] architect dialogue compat failed (contained): " + e.Message);
        }
    }
}
