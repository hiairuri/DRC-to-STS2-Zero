// namespace: SilentFX.Cards
// type: BubbleBubbleHeld (class)
// source lines: 84282-84285
	public sealed class BubbleBubbleHeld : AimMarkHeld
	{
		protected override bool Marks => C.TargetPoison > 0;
	}
