// namespace: SilentFX.Cards
// type: PredatorFx (class)
// source lines: 46565-46604
	public sealed class PredatorFx : CardFx
	{
		private static readonly Color Maw = new Color(1f, 0.38f, 0.16f, 0.95f);

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)frontParent))
				{
					float num = Strokes.Scale(c);
					float num2 = Strokes.Across(c, 0.85f) * 0.5f;
					float num3 = Strokes.Vertical(c, num2);
					float targetSide = c.TargetSide;
					Brush.Cut(frontParent, at + new Vector2((0f - targetSide) * num2 * 0.8f, (0f - num3) * 0.8f), at + new Vector2(targetSide * num2 * 0.8f, num3 * 0.7f), 54f * num, Maw, 0f, 0.04f, 0.03f, 0.18f);
					FxKit.Flash(frontParent, at, new Color(1f, 0.62f, 0.35f, 0.6f), 110f * num, 1.5f, 0.16);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueAttacks.OnHit: " + ex);
			}
		}

		static PredatorFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
