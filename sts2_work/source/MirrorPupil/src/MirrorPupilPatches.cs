using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace MirrorPupil;

/// <summary>
/// 两个 Harmony postfix，由游戏装 mod 时的默认 PatchAll 自动挂上（ModEntry 无需手动调用）。
/// </summary>
[HarmonyPatch]
internal static class MirrorPupilPatches
{
	// 攻击者/来源等成员没有公开写入口，按后备字段与私有字段注入。
	private static readonly FieldInfo? _attackerField =
		AccessTools.Field(typeof(AttackCommand), "<Attacker>k__BackingField");

	private static readonly FieldInfo? _modelSourceField =
		AccessTools.Field(typeof(AttackCommand), "<ModelSource>k__BackingField");

	private static readonly FieldInfo? _attackerAnimNameField =
		AccessTools.Field(typeof(AttackCommand), "_attackerAnimName");

	private static readonly FieldInfo? _attackerAnimDelayField =
		AccessTools.Field(typeof(AttackCommand), "_attackerAnimDelay");

	private static readonly FieldInfo? _sourceTypeField =
		AccessTools.Field(typeof(AttackCommand), "_sourceType");

	private static readonly object? _sourceTypeCard =
		AccessTools.Field(typeof(AttackCommand), "_sourceType") is { } sourceTypeField
			? AccessTools.Field(sourceTypeField.FieldType, "Card")?.GetValue(null)
			: null;

	/// <summary>
	/// 镜像重放时把“由卡牌发起”的攻击整体转换成镜像攻击，做法与 FromOsty 同款：
	/// 攻击者是镜像、攻击动画与时长也是镜像自己的（FromOsty 硬性要求 Monster is Osty
	/// 且延迟/音效写死奥斯提，所以照它的实现复刻）。伤害来源随之视为镜像——
	/// 玩家的力量等按来源结算的增益不再作用于这次伤害，镜像自己的状态生效。
	/// </summary>
	[HarmonyPrefix]
	[HarmonyPatch(typeof(AttackCommand), nameof(AttackCommand.FromCard))]
	private static bool FromCard_Prefix(AttackCommand __instance, CardModel card, ref AttackCommand __result)
	{
		// 镜像重放作用域之外：走原版 FromCard。
		if (!MirrorImageCmd.TryPeekAttackScope(out Creature mirror) || !mirror.IsAlive)
		{
			return true;
		}

		_attackerField?.SetValue(__instance, mirror);
		_modelSourceField?.SetValue(__instance, card);
		_attackerAnimNameField?.SetValue(__instance, MirrorImage.AttackAnimName);
		_attackerAnimDelayField?.SetValue(__instance, MirrorImage.AttackAnimDelay);
		_sourceTypeField?.SetValue(__instance, _sourceTypeCard);
		// ⚠️ prefix 返回 false 会跳过原方法体，返回值=default(引用类型)=null——
		// 必须 __result = __instance，否则链式调用 .Targeting(...) 打在 null 上
		// NRE，打牌任务中断、卡牌永远卡在处理区（2026-10-02 日志实锤）。
		__result = __instance.WithAttackerFx(null, MirrorImage.MirrorAttackSfx);
		return false;
	}

	/// <summary>
	/// 先古升级（选人）：欧洛巴的抚触（TouchOfOrobas）的升级表里没有本遗物时会落到头环，
	/// 这里把言灵·镜瞳指到它的先古版本。只认基础版——已是先古版再遇该选项，走原版路径换头环
	/// （与 BlackBlood 的行为一致）。状态搬运见 <see cref="Replace_Postfix"/>
	/// （AfterObtained 只取 Id 再造新实例，在这里搬状态会被丢掉）。
	/// </summary>
	[HarmonyPostfix]
	[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.GetUpgradedStarterRelic))]
	private static void GetUpgradedStarterRelic_Postfix(RelicModel starterRelic, ref RelicModel __result)
	{
		if (starterRelic is MirrorPupil)
		{
			__result = ModelDb.Relic<MirrorPupilAncient>().ToMutable();
		}
	}

	/// <summary>
	/// 联动回血的数据源（2026-10-03 用户定案）：镜像该拿的是【要回多少血】而非
	/// 实际变化量——角色满血时 CreatureCmd.Heal 照常调 HealInternal(全额请求量)
	/// （SetCurrentHpInternal 内部才钳到上限），postfix 在此捕获原始请求量。
	/// 直接解析遗物：只处理"玩家自己的生物"（宠物/镜像的回血不进来，防递归），
	/// 经 Player.GetRelic 找到镜瞳即调 HandleOwnerHeal——不依赖事件订阅时序
	/// （QuickRestart 续局不会重跑 AfterObtained，订阅式在续局会全部失联）。
	/// </summary>
	[HarmonyPostfix]
	[HarmonyPatch(typeof(Creature), nameof(Creature.HealInternal))]
	private static void HealInternal_Postfix(Creature __instance, decimal amount)
	{
		try
		{
			Player? p = __instance.Player;
			if (p == null || __instance != p.Creature) return; // 只认玩家本体（宠物回血不联动、防递归）
			p.GetRelic<MirrorPupilBase>()?.HandleOwnerHeal(amount);
		}
		catch (Exception e)
		{
			Godot.GD.PrintErr("[MirrorPupil] heal capture failed (contained): " + e.Message);
		}
	}

	/// <summary>
	/// 先古升级（搬状态）：遗物替换完成后，把旧遗物记下的镜像生命/上限搬进新遗物，
	/// 镜像不因升级重算回初始 30%。
	/// </summary>
	[HarmonyPostfix]
	[HarmonyPatch(typeof(RelicCmd), nameof(RelicCmd.Replace))]
	private static void Replace_Postfix(RelicModel original, RelicModel replace)
	{
		if (original is MirrorPupilBase from && replace is MirrorPupilBase to)
		{
			to.MirrorMaxHp = from.MirrorMaxHp;
			to.MirrorHp = from.MirrorHp;
		}
	}

	// ---------------- 宠物 UI 特权（游戏的写死 is Osty 的部分） ----------------
	// NCombatRoom.AddCreature 的宠物分支：本地奥斯提走专属提前返回（保血条、
	// 站位在主人旁），其余宠物走通用分支 = ToggleIsInteractable(false)（血条
	// _stateDisplay 直接 Visible=false）+ 站进通用宠物行（用主人 Bounds 均分，
	// 单宠时公式把镜像推到主人身体里）——这就是"镜像没血条、坐标不对"的机制。
	// 两个 postfix 把本地镜像拉回奥斯提待遇：血条恢复 + 站主人左侧 + 画序在前。

	[HarmonyPostfix]
	[HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.AddCreature))]
	private static void AddCreature_Postfix(NCombatRoom __instance, Creature creature)
	{
		try
		{
			if (creature.Monster is not MirrorImage) return;
			Player? owner = creature.PetOwner;
			if (owner == null || !LocalContext.IsMe(owner)) return;
			NCreature? node = __instance.GetCreatureNode(creature);
			NCreature? playerNode = __instance.GetCreatureNode(owner.Creature);
			if (node == null || playerNode == null) return;
			PlaceMirrorLikeOsty(node, playerNode);
		}
		catch (System.Exception e)
		{
			Godot.GD.PrintErr("[MirrorPupil] AddCreature postfix failed (contained): " + e.Message);
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.PositionPlayersAndPets))]
	private static void PositionPlayersAndPets_Postfix(List<NCreature> creatureNodes)
	{
		try
		{
			foreach (NCreature n in creatureNodes)
			{
				if (n.Entity.Monster is not MirrorImage) continue;
				Player? owner = n.Entity.PetOwner;
				if (owner == null || !LocalContext.IsMe(owner)) continue;
				NCreature? playerNode = creatureNodes.FirstOrDefault((NCreature c) => c.Entity == owner.Creature);
				if (playerNode == null) continue;
				PlaceMirrorLikeOsty(n, playerNode);
			}
		}
		catch (System.Exception e)
		{
			Godot.GD.PrintErr("[MirrorPupil] PositionPlayersAndPets postfix failed (contained): " + e.Message);
		}
	}

	/// <summary>血条恢复 + 站位（2026-10-03 用户定案：镜像底与奥斯提底以角色
	/// 底线对称）。数据：奥斯提底=奥斯提位 Y（玩家 Y−75，调校盒底边=节点原点）；
	/// 玩家底（脚线）=玩家 Y+playerFeetOffset（−20.4，ling 场景 Visuals(16,−21)
	/// ×0.29 + 骨架 bounds 换算 1.906×0.29）→ 奥斯提底高于脚线 d=54.6px → 镜像底=
	/// 脚线下方同距 → 镜像节点 Y=脚线+d−镜像脚偏移。镜像场景 Visuals 已改为与
	/// ling.tscn 完全同准（(16,−21)×0.29，2026-10-05 用户：镜像 1:1）→ 镜像脚偏移
	/// =−21+1.906×0.29=−20.45。X=奥斯提位 X（玩家半身宽+150）。画序插到主人前；
	/// 血条恢复。</summary>
	private static void PlaceMirrorLikeOsty(NCreature node, NCreature playerNode)
	{
		node.ToggleIsInteractable(node.Entity.Monster!.IsHealthBarVisible);
		const float playerFeetOffset = -20.4f;
		const float mirrorFeetOffset = -20.45f;
		float d = 75f + playerFeetOffset; // 奥斯提底高于脚线的距离（54.6）
		float mirrorBottom = playerNode.Position.Y + playerFeetOffset + d; // 脚线下方同距
		Vector2 ostySpot = playerNode.Position + new Vector2(playerNode.Hitbox.Size.X * 0.5f + 150f, -75f);
		node.Position = new Vector2(ostySpot.X, mirrorBottom - mirrorFeetOffset);
		node.GetParent()?.MoveChildSafely(node, playerNode.GetIndex());
	}
}
