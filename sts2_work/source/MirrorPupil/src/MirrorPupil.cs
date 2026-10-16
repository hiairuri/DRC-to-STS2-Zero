using MegaCrit.Sts2.Core.Entities.Relics;

namespace MirrorPupil;

/// <summary>
/// 言灵·镜瞳（初始遗物）。
/// 注册：ModEntry 里 ModHelper.AddModelToPool&lt;对应角色的遗物池, MirrorPupil&gt;()。
/// </summary>
public sealed class MirrorPupil : MirrorPupilBase
{
	public override RelicRarity Rarity => RelicRarity.Starter;
}
