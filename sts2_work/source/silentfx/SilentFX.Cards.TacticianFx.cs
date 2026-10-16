// namespace: SilentFX.Cards
// type: TacticianFx (class)
// source lines: 48070-48111
	public sealed class TacticianFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node val = Over.Ui(c);
				if (FxKit.Alive((GodotObject)(object)val))
				{
					Vector2 val2 = GameRefs.EnergyPoint();
					for (int i = 0; i < 2; i++)
					{
						Vector2 val3 = c.Chest + new Vector2(((float)i - 0.5f) * 30f, (0f - c.Body.Y) * 0.1f);
						Vector2 ctrl = new Vector2((val3.X + val2.X) * 0.5f, Mathf.Min(val3.Y, val2.Y) - 150f - 60f * (float)i);
						Brush.Orb(val, val3, ctrl, val2, 26f, Brush.Amber, 0.34f, 0.07f * (float)i);
					}
					FxKit.Flash(c.FrontParent, c.Chest, new Color(1f, 0.75f, 0.35f, 0.45f), c.Body.X * 0.7f, 1.5f, 0.16);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}
	}
