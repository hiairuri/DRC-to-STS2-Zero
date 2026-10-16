// namespace: SilentFX.Cards
// type: SidestepFx (class)
// source lines: 48018-48069
	public sealed class SidestepFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node parent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					float f = c.Facing;
					Vector2 val = c.Feet + new Vector2(0f, (0f - c.Body.Y) * 0.1f);
					Vector2 val2 = val + new Vector2(f * c.Body.X * 0.85f, 0f);
					Vector2 to = val + new Vector2((0f - f) * c.Body.X * 0.95f, 0f);
					Brush.Dart(parent, val2, to, c.Body.X * 1.1f, 18f, Brush.Wind, 0.12f, 0f, () =>
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_000c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_001b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0044: Unknown result type (might be due to invalid IL or missing references)
						//IL_0049: Unknown result type (might be due to invalid IL or missing references)
						//IL_004e: Unknown result type (might be due to invalid IL or missing references)
						//IL_0056: Unknown result type (might be due to invalid IL or missing references)
						//IL_005b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0062: Unknown result type (might be due to invalid IL or missing references)
						//IL_006c: Unknown result type (might be due to invalid IL or missing references)
						Vector2 ctrl = (to + c.Chest) * 0.5f + new Vector2((0f - f) * 40f, (0f - c.Body.Y) * 0.5f);
						Brush.Orb(parent, to, ctrl, c.Chest, 16f, Brush.Amber, 0.26f);
					});
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}
	}
