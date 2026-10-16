// namespace: SilentFX.Cards
// type: ShivHeldFx (class)
// source lines: 99165-99210
	public sealed class ShivHeldFx : HeldFx
	{
		private readonly ShivStyle _style;

		private readonly int _previewCount;

		public override bool Aimed => _style == ShivStyle.Return;

		public ShivHeldFx(ShivStyle style, int previewCount = 3)
		{
			_style = style;
			_previewCount = previewCount;
		}

		public override bool AimedFor(CardModel card)
		{
			if (Aimed)
			{
				return !Fan(card);
			}
			return false;
		}

		public override bool HoverFor(CardModel card)
		{
			if (!Hover)
			{
				return Fan(card);
			}
			return true;
		}

		private bool Fan(CardModel card)
		{
			if (_style == ShivStyle.Return)
			{
				return ShivFan.Active(card);
			}
			return false;
		}

		public override HeldSession Create()
		{
			return new ShivHeld(_style, _previewCount);
		}
	}
