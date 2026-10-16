// namespace: SilentFX.Cards
// type: WraithHeldChoice (class)
// source lines: 120544-120556
	public sealed class WraithHeldChoice : HeldFx
	{
		public override bool Hover => true;

		public override HeldSession Create()
		{
			if (WraithStage.Ready)
			{
				return new WraithHeld2();
			}
			return new WraithHeld();
		}
	}
