using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MirrorPupil;

/// <summary>
/// 镜为你碎：镜像版的「为你而死」。
/// 原版 DieForYouPower 是 sealed，且描述文本写死宠物名（奥斯提），不能复用，
/// 所以按同样的四条裁定自定一个状态：替主人挡未格挡的攻击伤害、死亡后留场可复活。
/// </summary>
public sealed class MirrorShatterPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Single;

	public override bool ShouldPlayVfx => false;

	public override Creature ModifyUnblockedDamageTarget(Creature target, decimal _, ValueProp props, Creature? __)
	{
		if (target != base.Owner.PetOwner?.Creature)
		{
			return target;
		}
		if (base.Owner.IsDead)
		{
			return target;
		}
		if (!props.IsPoweredAttack())
		{
			return target;
		}
		// 奥斯提承伤优先（2026-10-03 用户裁定）：主人若有存活的奥斯提，
		// 让位给原版 DieForYouPower 改写目标，镜像不抢。
		if (base.Owner.PetOwner?.Osty is { IsAlive: true })
		{
			return target;
		}
		return base.Owner;
	}

	/// <summary>死亡的镜像不再被选中、也不再替主人承伤（与原版一致）。</summary>
	public override bool ShouldAllowHitting(Creature creature)
	{
		return creature.IsAlive;
	}

	public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
	{
		if (creature != base.Owner)
		{
			return true;
		}
		return false;
	}

	public override bool ShouldPowerBeRemovedAfterOwnerDeath()
	{
		return false;
	}
}
