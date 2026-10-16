using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace MirrorPupil;

/// <summary>
/// 镜像：言灵·镜瞳召唤的友方生物。走宠物通道（PlayerCmd.AddPet），但完全不占奥斯提的槽——
/// Player.Osty 只认 Monster is Osty，两边互不可见。
///
/// 按 Osty（奥斯提）完整代码逐成员对照拷贝再改，防遗漏（2026-10-02 用户指令）：
///   - Min/MaxInitialHp=1/1、IsHealthBarVisible=存活（与 Osty 同）
///   - DeathSfx/HasDeathSfx：MonsterModel 默认值即 HasDeathSfx=true + id 派生路径
///     （event:/sfx/enemy/enemy_attacks/mirror_image/... 不存在→死亡静默，与 Osty
///     覆写 DeathSfx 的动机相同，只是我们暂无专属音效，不指向奥斯提的声音）
///   - GenerateMoveStateMachine：NOTHING_MOVE 自环（与 Osty 同）
///   - GenerateAnimator：Osty 的完整动画图结构（cast/attack/hurt 下一态+Hit 分支、
///     die 下一态、任意态触发），映射到 ling 骨架的动画集
///     （idle_loop/attack/cast/hurt/die/win/winloop/tibu/stand；ling 无 dead_loop/
///     revive/attack_poke——die 无下一态=停末帧，而 ling die 第三阶段本就是全槽
///     渐隐=自然化空，dead_loop 缺失只 warn 无碍）
/// 实际生命由召唤方（MirrorImageCmd）写入，初始 HP 只是占位。
/// </summary>
public sealed class MirrorImage : MonsterModel
{
	/// <summary>镜像的攻击动画触发名（对应 GenerateAnimator 的任意态分支）。</summary>
	public const string AttackAnimName = "Attack";

	/// <summary>镜像的攻击动画时长（与奥斯提的 attackerAnimDelay 一致）。</summary>
	public const float AttackAnimDelay = 0.3f;

	/// <summary>
	/// 镜像的攻击音效（FMOD 路径）：mod 提供音频后填这里，null 则攻击不出声。
	/// 攻击命令不会回退去读 MonsterModel.AttackSfx（那是怪物自身行动的约定路径），
	/// 所以重放转换时显式传这一份。
	/// </summary>
	public const string? MirrorAttackSfx = null;

	public override int MinInitialHp => 1;

	public override int MaxInitialHp => 1;

	public override bool IsHealthBarVisible => base.Creature.IsAlive;

	protected override MonsterMoveStateMachine GenerateMoveStateMachine()
	{
		// 不做任何行动（与奥斯提同一形状）。
		MoveState nothing = new MoveState("NOTHING_MOVE", (IReadOnlyList<Creature> _) => Task.CompletedTask);
		nothing.FollowUpState = nothing;
		return new MonsterMoveStateMachine(new List<MonsterState> { nothing }, nothing);
	}

	public override CreatureAnimator GenerateAnimator(MegaSprite controller)
	{
		// Osty 的完整动画图：cast/attack/hurt 各自回 idle 且带 Hit 分支；
		// die 无下一态（ling 无 dead_loop——见类注释）。
		AnimState idle = new AnimState("idle_loop", isLooping: true);
		AnimState cast = new AnimState("cast");
		AnimState attack = new AnimState("attack");
		AnimState hurt = new AnimState("hurt");
		AnimState die = new AnimState("die");
		idle.AddBranch("Hit", hurt);
		cast.NextState = idle;
		cast.AddBranch("Hit", hurt);
		attack.NextState = idle;
		attack.AddBranch("Hit", hurt);
		hurt.NextState = idle;
		hurt.AddBranch("Hit", hurt);
		CreatureAnimator creatureAnimator = new CreatureAnimator(idle, controller);
		creatureAnimator.AddAnyState("Idle", idle);
		creatureAnimator.AddAnyState("Dead", die);
		creatureAnimator.AddAnyState("Hit", hurt);
		creatureAnimator.AddAnyState("Attack", attack);
		creatureAnimator.AddAnyState("Cast", cast);
		creatureAnimator.AddAnyState("PowerUp", cast);
		creatureAnimator.AddAnyState("Shiv", attack);
		creatureAnimator.AddAnyState("Relaxed", idle);
		creatureAnimator.AddAnyState("Win", new AnimState("win"));
		// 龙族镜像战士材质（严格移植：运行时 darkenParam=0 → 贴图反相负片 1−albedo，
		// 见 ling_mirror_*.gdshader 头注）：
		// 骨架此时已就绪，材质一经设置即持久
		MirrorMaterial.Apply(controller);
		return creatureAnimator;
	}
}
