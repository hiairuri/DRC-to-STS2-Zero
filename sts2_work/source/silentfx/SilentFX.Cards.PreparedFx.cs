// namespace: SilentFX.Cards
// type: PreparedFx (class)
// source lines: 48595-48635
	public sealed class PreparedFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node parent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					Vector2 hand = c.WeaponHand;
					FxKit.Flash(parent, hand, new Color(0.86f, 0.94f, 1f, 0.8f), 30f, 1.8f, 0.12);
					FxKit.After(parent, 0.10000000149011612, () =>
					{
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						//IL_0020: Unknown result type (might be due to invalid IL or missing references)
						FxKit.Flash(parent, hand, new Color(0.86f, 0.94f, 1f, 0.6f), 22f, 1.8f, 0.12);
					});
					Node val = Over.Ui(c);
					if (FxKit.Alive((GodotObject)(object)val) && val != parent)
					{
						Over.HandSpan(c, out var left, out var right);
						Brush.Dart(val, GameRefs.PilePoint((PileType)1), left.Lerp(right, 0.5f) + new Vector2(0f, -40f), 220f, 10f, Brush.Steel, 0.1f, 0.03f);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}
	}
