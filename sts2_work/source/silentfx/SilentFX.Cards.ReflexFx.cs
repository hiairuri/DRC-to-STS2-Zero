// namespace: SilentFX.Cards
// type: ReflexFx (class)
// source lines: 47960-48017
	public sealed class ReflexFx : CardFx
	{
		private static readonly Color Snap = new Color(0.45f, 0.95f, 1f, 0.95f);

		public override void OnPlay(CardFxContext c)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)frontParent))
				{
					Brush.Cut(frontParent, c.Chest + new Vector2(0f, c.Body.Y * 0.55f), c.Chest + new Vector2(0f, (0f - c.Body.Y) * 0.75f), 16f, Snap, 0f, 0.03f, 0.02f, 0.16f);
					FxKit.Flash(frontParent, c.Chest, new Color(Snap.R, Snap.G, Snap.B, 0.5f), c.Body.X * 0.8f, 1.5f, 0.14);
					Node val = Over.Ui(c);
					if (FxKit.Alive((GodotObject)(object)val) && val != frontParent)
					{
						Over.HandSpan(c, out var left, out var right);
						Vector2 val2 = GameRefs.PilePoint((PileType)1);
						Brush.Dart(val, val2, left.Lerp(right, 0.4f) + new Vector2(0f, -40f), 240f, 12f, Snap, 0.1f, 0.04f);
						Brush.Dart(val, val2, left.Lerp(right, 0.62f) + new Vector2(0f, -40f), 240f, 12f, Snap, 0.1f, 0.09f);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}

		static ReflexFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
