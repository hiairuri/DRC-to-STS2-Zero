using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace MirrorPupil;

/// <summary>
/// 瞳印：言灵·镜瞳的计数状态（Counter 层数）。每场战斗开始时施加 100 层，
/// 判定成功后减少。自然随战斗结束清空。
/// </summary>
public sealed class PupilMarkPower : PowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;
}
