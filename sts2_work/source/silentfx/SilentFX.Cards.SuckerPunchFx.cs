// namespace: SilentFX.Cards
// type: SuckerPunchFx (class)
// source lines: 46605-46635
	public sealed class SuckerPunchFx : CardFx
	{
		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)frontParent))
				{
					float num = Strokes.Scale(c);
					float targetSide = c.TargetSide;
					FxKit.Flash(frontParent, at, new Color(1f, 0.94f, 0.82f, 0.9f), 90f * num, 1.45f, 0.13);
					Brush.Dust(frontParent, at, targetSide, 12, 1.2f * num);
					FxKit.Burst(frontParent, at + new Vector2(0f, (0f - c.TargetHeight) * 0.15f), new Color(0.62f, 0.42f, 0.92f, 0.7f), 8, 20f, 70f, 0.7, 0.2f * num, 0.42f * num, new Vector2(0f, 160f), 60f, new Vector2(0f, 1f), c.TargetWidth * 0.22f, 0f, additive: true, 0.7f);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueAttacks.OnHit: " + ex);
			}
		}
	}
