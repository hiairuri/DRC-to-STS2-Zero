// namespace: SilentFX.Cards
// type: ShivPreviewFx (class)
// source lines: 99118-99160
	public sealed class ShivPreviewFx : CardFx
	{
		private readonly ShivStyle _style;

		private readonly int _count;

		private readonly bool _accuracy;

		private readonly bool _phantom;

		public ShivPreviewFx(ShivStyle style, int count, bool accuracy = false, bool phantom = false)
		{
			_style = style;
			_count = count;
			_accuracy = accuracy;
			_phantom = phantom;
		}

		public override void OnPlay(CardFxContext c)
		{
			try
			{
				if (c.Held is ShivHeld held && _style != ShivStyle.Return)
				{
					ShivFx.PreviewTake(c, held, _style, _count);
				}
				else
				{
					ShivFx.PlayCreate(c, _style, _count);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ShivFx.OnPlay: " + ex);
			}
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			ShivFx.PlayHit(c, at, _accuracy, _phantom, _style == ShivStyle.Ink);
		}
	}
