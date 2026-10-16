// namespace: SilentFX.Cards
// type: MementoStrikeFx (class)
// source lines: 79189-79256
	public sealed class MementoStrikeFx : CardFx
	{
		private static readonly Color Ghost = new Color(0.74f, 1f, 0.8f, 0.85f);

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			try
			{
				MementoHeld mementoHeld = (HeldFxManager.PlayingFor(c.Card) as MementoHeld) ?? (c.Held as MementoHeld);
				if (mementoHeld != null)
				{
					mementoHeld.Strike(at);
					flag = mementoHeld.Striking;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoStrikeFx: " + ex.Message);
			}
			Node frontParent = c.FrontParent;
			if (FxKit.Alive((GodotObject)(object)frontParent))
			{
				float num = Strokes.Scale(c);
				float num2 = Strokes.Across(c, 0.95f) * 0.5f;
				float targetSide = c.TargetSide;
				Vector2 a = at + new Vector2((0f - targetSide) * num2 * 0.8f, (0f - num2) * 0.75f);
				Vector2 b = at + new Vector2(targetSide * num2 * 0.5f, num2 * 0.8f);
				Vector2 ctrl = at + new Vector2(targetSide * num2 * 0.75f, (0f - num2) * 0.35f);
				Brush.Arc(frontParent, a, ctrl, b, 34f * num, Ghost, 0f, 0.06f, 0.04f, 0.28f);
				Brush.Motes(frontParent, at, new Color(0.86f, 1f, 0.9f, 0.75f), 9, num2 * 0.6f, 150f, 0.8);
				if (!flag)
				{
					FxKit.Flash(frontParent, at, new Color(Ghost.R, Ghost.G, Ghost.B, 0.7f), 70f * num, 2f, 0.16);
					FxKit.RingPulse(frontParent, at, 170f * num, new Color(Ghost.R, Ghost.G, Ghost.B, 1f), 0f, 0.26f, 0.2f, 1f, 1f, 0f, 0.035f, 0.6f);
				}
			}
		}

		static MementoStrikeFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
