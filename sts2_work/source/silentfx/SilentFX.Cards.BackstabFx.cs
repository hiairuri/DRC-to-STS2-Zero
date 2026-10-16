// namespace: SilentFX.Cards
// type: BackstabFx (class)
// source lines: 46375-46430
	public sealed class BackstabFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (c.HasTarget && !(c.Held is BackstabHeld))
				{
					Shade.Puff(FxKit.Alive((GodotObject)(object)c.BackParent) ? c.BackParent : c.FrontParent, BackstabHeld.Behind(c), 150f * Strokes.Scale(c), 1.4f, 0.4f, 0.9f, 0.35f);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueAttacks.OnPlay: " + ex);
			}
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)frontParent))
				{
					float num = Strokes.Scale(c);
					float targetSide = c.TargetSide;
					Vector2 val = at + new Vector2(targetSide * (c.TargetWidth * 0.5f + 60f), -14f * num);
					Vector2 to = at + new Vector2((0f - targetSide) * (c.TargetWidth * 0.5f + 40f), 10f * num);
					Brush.Cut(frontParent, val, to, 26f * num, Brush.Steel, 0f, 0.04f, 0.05f, 0.2f);
					FxKit.Flash(frontParent, at, new Color(0.92f, 0.96f, 1f, 0.8f), 54f * num, 1.6f, 0.14);
					Shade.Puff(frontParent, at + new Vector2(targetSide * c.TargetWidth * 0.3f, 0f), 130f * num, 1.8f, 0.36f, 0.6f, 0.3f);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueAttacks.OnHit: " + ex);
			}
		}
	}
