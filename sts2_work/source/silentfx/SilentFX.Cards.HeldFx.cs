// namespace: SilentFX.Cards
// type: HeldFx (class)
// source lines: 68450-68470
	public sealed class HeldFx<T> : HeldFx where T : HeldSession, new()
	{
		private readonly bool _aimed;

		private readonly bool _hover;

		public override bool Aimed => _aimed;

		public override bool Hover => _hover;

		public HeldFx(bool aimed = false, bool hover = false)
		{
			_aimed = aimed;
			_hover = hover;
		}

		public override HeldSession Create()
		{
			return new T();
		}
	}
