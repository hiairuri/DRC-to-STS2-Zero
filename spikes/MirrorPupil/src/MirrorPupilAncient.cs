using MegaCrit.Sts2.Core.Entities.Relics;

namespace MirrorPupil;

/// <summary>
/// 言灵·镜瞳的先古版本：瞳印每次减少 15（不低于 40），其余同基础版。
/// 与原版先古初始遗物（BlackBlood 等）一致，保持 Starter 稀有度；
/// 由 <see cref="MirrorPupilPatches"/> 把映射挂进“欧洛巴的抚触”，在先古之民处替换而来。
/// </summary>
public sealed class MirrorPupilAncient : MirrorPupilBase
{
	public override RelicRarity Rarity => RelicRarity.Starter;

	public override int ProcDecrement => 15;

	public override int ProcFloor => 40;
}
