// namespace: SilentFX.Cards
// type: ExpertiseFx (class)
// source lines: 46937-46990
	public sealed class ExpertiseFx : CardFx
	{
		private static readonly Color Blue = new Color(0.4f, 0.74f, 1f, 0.9f);

		public override void OnPlay(CardFxContext c)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			if (!GameCapabilities.ExpertiseRetainsDrawnCards)
			{
				return;
			}
			try
			{
				Node val = Over.Ui(c);
				if (FxKit.Alive((GodotObject)(object)val))
				{
					Vector2 val2 = FxKit.ViewSize();
					Vector2 val3 = GameRefs.PilePoint((PileType)1);
					Vector2 val4 = c.Chest + new Vector2(0f, c.Body.Y * 0.12f);
					Vector2 val5 = new Vector2(val3.X + 40f, val2.Y - 4f);
					Vector2 ctrl = new Vector2(Mathf.Lerp(val4.X, val5.X, 0.25f), val5.Y - 24f);
					Brush.Orb(val, val4, ctrl, val5, 18f, Blue, 0.24f);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}

		static ExpertiseFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
