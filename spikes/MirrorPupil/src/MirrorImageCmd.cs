using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace MirrorPupil;

/// <summary>
/// 镜像的召唤与“伤害来源视为镜像”的作用域。
///
/// 按 OstyCmd（奥斯提召唤系统）逐行对照重做（2026-10-02 用户指令），要点全部对齐：
///   - fresh 召唤的初始生命 = Heal(…, playAnim: false) 静默落血——奥斯提的召唤没有
///     回复生命特效的机制就在这一参（我们此前用默认 playAnim=true → 绿色十字回血
///     特效 + 数字 + 音效全放，就是用户指出的“多了回血特效”）。
///   - 复活路径 = AddPetInternal → SetMaxHp → Heal(…, playAnim: true)：Heal 的
///     isDead 分支自动播复活动画（与奥斯提复活完全同款，复活该有的特效有）。
///   - 卡源新召唤的亮相 = Modulate 透明→白 0.35s 延迟 0.1 + StartReviveAnim
///     （OstyCmd 对 CardModel 来源的召唤同款）。
///   - TrackBlockStatus(主人)：替主承伤（镜为你碎）的格挡显示跟随主人（奥斯提同款）。
/// 镜像不占奥斯提的槽（Player.Osty 只认 Monster is Osty），两边互不可见。
/// 镜像的生命/上限一有变动就同步进遗物（MirrorPupilBase.SyncMirrorState），
/// 由存档自动持久化——下一场战斗从遗物里取值召唤。
/// </summary>
public static class MirrorImageCmd
{
	private const string SummonSfx = "event:/sfx/characters/necrobinder/necrobinder_summon";

	/// <summary>按“本遗物的主人”找当前战斗里的镜像；不在场返回 null。</summary>
	public static Creature? FindMirror(Player player)
	{
		return player.Creature.CombatState?.Allies
			.FirstOrDefault((Creature c) => c.Monster is MirrorImage && c.PetOwner == player);
	}

	/// <summary>是否有“伤害来源视为镜像”的作用域正在生效（=此牌由镜像触发）。</summary>
	public static bool InAttackScope => _attackScope.Count > 0;

	/// <summary>
	/// 战斗开始时的召唤：以遗物存下的上限/当前生命创建镜像（本场战斗还没有镜像时）。
	/// </summary>
	public static async Task SummonFull(PlayerChoiceContext choiceContext, Player player, int maxHp, int currentHp)
	{
		ICombatState combatState = player.Creature.CombatState!;
		Creature? existing = FindMirror(player);
		if (existing != null)
		{
			// 防御：本场已有镜像就只把数值对齐，不再创建第二只（静默落血）。
			await CreatureCmd.SetMaxHp(existing, maxHp);
			await CreatureCmd.Heal(existing, currentHp, playAnim: false);
			return;
		}
		if (CombatManager.Instance.IsInProgress)
		{
			SfxCmd.Play(SummonSfx);
		}
		Creature mirror = await CreateMirror(choiceContext, player, showSummonAnim: false);
		await CreatureCmd.SetMaxHp(mirror, maxHp);
		// 初始生命静默落血——奥斯提召唤没有回血特效的机制就在 playAnim: false。
		await CreatureCmd.Heal(mirror, currentHp, playAnim: false);
		SyncMirror(player, mirror);
		FinishSummon(combatState, choiceContext, player, maxHp);
	}

	/// <summary>
	/// “镜像召唤 1”：活着则生命/上限各 +1（GainMaxHp 自带等量回复，与奥斯提活着路径
	/// 同款）；已死亡（尸体在场）则以 1/1 复活（回血特效 + 复活动画）；不在场则以
	/// 1/1 新建（带亮相动画）。
	/// </summary>
	public static async Task SummonOne(PlayerChoiceContext choiceContext, Player player)
	{
		ICombatState combatState = player.Creature.CombatState!;
		Creature? mirror = FindMirror(player);
		if (mirror is { IsAlive: true })
		{
			// 奥斯提 alive 路径用的 GainMaxHp 内部是默认 playAnim=true 的 Heal
			// （绿色十字+数字）——拆开成静默等价：上限+1、生命+1 不放特效
			await CreatureCmd.SetMaxHp(mirror, mirror.MaxHp + 1);
			await CreatureCmd.Heal(mirror, 1, playAnim: false);
			FinishSummon(combatState, choiceContext, player, 1);
			return;
		}
		if (mirror != null)
		{
			// 复活路径：与 OstyCmd 完全同款——重新挂上宠物，再给 1 点上限与生命；
			// Heal 的 isDead 分支自动播放复活动画（playAnim 保持默认 true）。
			player.PlayerCombatState!.AddPetInternal(mirror);
			await CreatureCmd.SetMaxHp(mirror, 1);
			await CreatureCmd.Heal(mirror, 1);
			FinishSummon(combatState, choiceContext, player, 1);
			return;
		}
		Creature created = await CreateMirror(choiceContext, player, showSummonAnim: true);
		await CreatureCmd.SetMaxHp(created, 1);
		await CreatureCmd.Heal(created, 1, playAnim: false);
		SyncMirror(player, created);
		FinishSummon(combatState, choiceContext, player, 1);
	}

	private static async Task<Creature> CreateMirror(PlayerChoiceContext choiceContext, Player player, bool showSummonAnim)
	{
		Creature mirror = await PlayerCmd.AddPet<MirrorImage>(player);
		// 亮相动画（OstyCmd 卡源召唤同款：透明→白 0.35s 延迟 0.1 + StartReviveAnim）
		NCreature? mirrorNode = NCombatRoom.Instance?.GetCreatureNode(mirror);
		if (mirrorNode != null && showSummonAnim)
		{
			mirrorNode.Modulate = Colors.Transparent;
			Tween tween = mirrorNode.CreateTween();
			tween.TweenProperty(mirrorNode, "modulate", Colors.White, 0.3499999940395355f).SetDelay(0.10000000149011612f);
			mirrorNode.StartReviveAnim();
		}
		await PowerCmd.Apply<MirrorShatterPower>(choiceContext, mirror, 1m, null, null);
		// 镜为你碎：格挡显示跟随主人（OstyCmd 的 TrackBlockStatus 同款）
		mirrorNode?.TrackBlockStatus(player.Creature);
		SubscribeMirrorState(player, mirror);
		return mirror;
	}

	/// <summary>把镜像当前的生命/上限立刻写回遗物（后续变动由事件接管）。</summary>
	private static void SyncMirror(Player player, Creature mirror)
	{
		player.GetRelic<MirrorPupilBase>()?.SyncMirrorState(mirror);
	}

	/// <summary>镜像状态一变就同步进遗物（随存档持久化）。只在创建时挂一次。</summary>
	private static void SubscribeMirrorState(Player player, Creature mirror)
	{
		MirrorPupilBase? relic = player.GetRelic<MirrorPupilBase>();
		if (relic == null)
		{
			return;
		}
		mirror.CurrentHpChanged += (_, _) => relic.SyncMirrorState(mirror);
		mirror.MaxHpChanged += (_, _) => relic.SyncMirrorState(mirror);
		mirror.Died += _ =>
		{
			// 镜像死亡：遗物状态归零；之后若有“召唤 1”复活，再由生命变动事件重新同步。
			relic.MirrorMaxHp = 0;
			relic.MirrorHp = 0;
		};
	}

	private static void FinishSummon(ICombatState combatState, PlayerChoiceContext choiceContext, Player player, int amount)
	{
		CombatManager.Instance.History.Summoned(combatState, amount, player);
		Hook.AfterSummon(combatState, choiceContext, player, amount);
	}

	/// <summary>
	/// “伤害来源视为镜像”的作用域：作用域内所有“由卡牌发起”的攻击
	/// （AttackCommand.FromCard）都会被 <see cref="MirrorPupilPatches"/> 换成镜像作为攻击者。
	/// 可嵌套（重放的牌再触发重放时各回各的镜像）。
	/// </summary>
	public static IDisposable AttackAs(Creature mirror)
	{
		_attackScope.Add(mirror);
		return new AttackScopeToken();
	}

	public static bool TryPeekAttackScope(out Creature mirror)
	{
		if (_attackScope.Count > 0)
		{
			mirror = _attackScope[_attackScope.Count - 1];
			return true;
		}
		mirror = null!;
		return false;
	}

	private static readonly List<Creature> _attackScope = new List<Creature>();

	private sealed class AttackScopeToken : IDisposable
	{
		public void Dispose()
		{
			_attackScope.RemoveAt(_attackScope.Count - 1);
		}
	}
}
