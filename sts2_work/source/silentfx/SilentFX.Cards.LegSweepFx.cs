// namespace: SilentFX.Cards
// type: LegSweepFx (class)
// source lines: 47914-47959
	public sealed class LegSweepFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node parent = c.FrontParent;
				if (c.HasTarget && FxKit.Alive((GodotObject)(object)parent))
				{
					float k = Strokes.Scale(c);
					float side = c.TargetSide;
					float num = Mathf.Max(c.TargetWidth * 0.75f, 130f);
					Vector2 feet = c.TargetFeet + new Vector2(0f, -14f * k);
					Brush.Arc(parent, feet + new Vector2((0f - side) * num, -18f * k), feet + new Vector2(0f, 30f * k), feet + new Vector2(side * num, -18f * k), 30f * k, Brush.Wind, 0f, 0.07f, 0.03f, 0.22f);
					FxKit.After(parent, 0.05000000074505806, () =>
					{
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						//IL_0031: Unknown result type (might be due to invalid IL or missing references)
						//IL_004a: Unknown result type (might be due to invalid IL or missing references)
						//IL_0085: Unknown result type (might be due to invalid IL or missing references)
						//IL_0099: Unknown result type (might be due to invalid IL or missing references)
						Brush.Dust(parent, feet, side, 12, 1.1f * k);
						FxKit.Burst(parent, c.TargetChest, new Color(0.62f, 0.42f, 0.92f, 0.7f), 8, 20f, 70f, 0.7, 0.2f * k, 0.42f * k, new Vector2(0f, 160f), 60f, new Vector2(0f, 1f), c.TargetWidth * 0.22f, 0f, additive: true, 0.7f);
					});
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}
	}
