// namespace: SilentFX.Cards
// type: DashFx (class)
// source lines: 46494-46564
	public sealed class DashFx : CardFx
	{
		private static readonly Color Teal = new Color(0.55f, 0.98f, 0.92f, 0.85f);

		public override void OnPlay(CardFxContext c)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (c.HasTarget && FxKit.Alive((GodotObject)(object)frontParent))
				{
					float targetSide = c.TargetSide;
					Vector2 val = c.Chest + new Vector2(targetSide * c.Body.X * 0.3f, 0f);
					Vector2 to = c.TargetChest - new Vector2(targetSide * c.TargetWidth * 0.35f, 0f);
					Brush.Dart(frontParent, val, to, Mathf.Max(260f, c.TargetWidth * 0.65f), 22f, Teal, 0.11f);
					Brush.Dust(frontParent, c.Feet, 0f - targetSide, 8);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueAttacks.OnPlay: " + ex);
			}
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)frontParent))
				{
					float num = Strokes.Scale(c);
					float num2 = Strokes.Across(c, 1.15f) * 0.5f;
					float targetSide = c.TargetSide;
					Brush.Cut(frontParent, at + new Vector2((0f - targetSide) * num2, 6f * num), at + new Vector2(targetSide * num2, -10f * num), 38f * num, Teal, 0f, 0.04f, 0.03f, 0.2f);
					FxKit.Flash(frontParent, at, new Color(0.85f, 1f, 0.97f, 0.7f), 70f * num, 1.7f, 0.14);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueAttacks.OnHit: " + ex);
			}
		}

		static DashFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
