// namespace: SilentFX.Cards
// type: FlickFlackFx (class)
// source lines: 46636-46680
	public sealed class FlickFlackFx : CardFx
	{
		private static readonly Color Flick = new Color(0.8f, 1f, 0.92f, 0.88f);

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)frontParent))
				{
					float num = Strokes.Scale(c);
					float num2 = Strokes.Across(c, 0.8f) * 0.5f;
					float num3 = Strokes.Vertical(c, num2);
					float targetSide = c.TargetSide;
					Brush.Cut(frontParent, at + new Vector2((0f - targetSide) * num2, (0f - num3) * 0.6f), at + new Vector2(targetSide * num2, num3 * 0.6f), 16f * num, Flick, 0f, 0.03f, 0.02f, 0.16f);
					Brush.Cut(frontParent, at + new Vector2(targetSide * num2, (0f - num3) * 0.6f), at + new Vector2((0f - targetSide) * num2, num3 * 0.6f), 16f * num, Flick, 0.06f, 0.03f, 0.02f, 0.16f);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueAttacks.OnHit: " + ex);
			}
		}

		static FlickFlackFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
