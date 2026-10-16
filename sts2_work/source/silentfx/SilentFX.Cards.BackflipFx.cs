// namespace: SilentFX.Cards
// type: BackflipFx (class)
// source lines: 46791-46848
	public sealed class BackflipFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node parent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					float f = c.Facing;
					Vector2 a = c.Chest + new Vector2(f * c.Body.X * 0.75f, c.Body.Y * 0.1f);
					Vector2 ctrl = c.Chest + new Vector2(0f, (0f - c.Body.Y) * 1.75f);
					Vector2 b = c.Chest + new Vector2((0f - f) * c.Body.X * 0.85f, c.Body.Y * 0.2f);
					Brush.Follow(parent, (float t) =>
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						//IL_000d: Unknown result type (might be due to invalid IL or missing references)
						//IL_0013: Unknown result type (might be due to invalid IL or missing references)
						return FxKit.Bezier(a, ctrl, b, t);
					}, 0.42f, 20f, Brush.Wind, 0.26f);
					Brush.Follow(parent, (float t) =>
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						//IL_0016: Unknown result type (might be due to invalid IL or missing references)
						//IL_001b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0021: Unknown result type (might be due to invalid IL or missing references)
						//IL_0027: Unknown result type (might be due to invalid IL or missing references)
						return FxKit.Bezier(a, ctrl + new Vector2(0f, 26f), b, t);
					}, 0.3f, 11f, Brush.Wind, 0.26f, 0.04f);
					FxKit.After(parent, 0.2199999988079071, () =>
					{
						//IL_000c: Unknown result type (might be due to invalid IL or missing references)
						Brush.Dust(parent, c.Feet, 0f - f, 8);
					});
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}
	}
