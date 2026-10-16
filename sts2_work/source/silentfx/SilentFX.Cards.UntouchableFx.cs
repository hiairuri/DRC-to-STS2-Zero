// namespace: SilentFX.Cards
// type: UntouchableFx (class)
// source lines: 48636-48691
	public sealed class UntouchableFx : CardFx
	{
		private static readonly Color Shell = new Color(0.8f, 0.92f, 1f, 0.85f);

		public override void OnPlay(CardFxContext c)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)frontParent))
				{
					Vector2 val = c.Chest + new Vector2(0f, (0f - c.Body.Y) * 0.95f);
					Vector2 val2 = c.Chest + new Vector2(0f, c.Body.Y * 0.55f);
					for (int i = -1; i <= 1; i += 2)
					{
						Vector2 a = val2 + new Vector2((float)i * c.Body.X * 0.55f, 0f);
						Vector2 ctrl = c.Chest + new Vector2((float)i * c.Body.X * 1.25f, (0f - c.Body.Y) * 0.35f);
						Brush.Arc(frontParent, a, ctrl, val, 22f, Shell, 0f, 0.07f, 0.05f);
					}
					FxKit.Flash(frontParent, val, new Color(0.9f, 0.97f, 1f, 0.6f), 40f, 1.6f, 0.16);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}

		static UntouchableFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
