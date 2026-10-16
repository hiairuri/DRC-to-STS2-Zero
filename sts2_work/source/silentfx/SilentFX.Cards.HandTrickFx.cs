// namespace: SilentFX.Cards
// type: HandTrickFx (class)
// source lines: 67091-67147
	public sealed class HandTrickFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (c == null)
				{
					return;
				}
				bool flag = false;
				try
				{
					flag = HandTrickWipe.Begin(c.Card);
				}
				catch (Exception ex)
				{
					DevLog.Warn("HandTrickFx.OnPlay: " + ex);
				}
				if (flag)
				{
					return;
				}
				Node parent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					Vector2 hand = c.WeaponHand;
					float r = Mathf.Clamp(c.Body.X * 0.16f, 20f, 40f);
					Brush.Follow(parent, (float t) =>
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_0036: Unknown result type (might be due to invalid IL or missing references)
						//IL_0041: Unknown result type (might be due to invalid IL or missing references)
						//IL_0046: Unknown result type (might be due to invalid IL or missing references)
						return hand + new Vector2(Mathf.Cos(t * ((float)Math.PI * 2f) * 1.25f - 1.2f), Mathf.Sin(t * ((float)Math.PI * 2f) * 1.25f - 1.2f)) * r;
					}, 0.3f, 10f, Brush.Gold, 0.26f, 0f, () =>
					{
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						//IL_000c: Unknown result type (might be due to invalid IL or missing references)
						Brush.Sparks(parent, hand, Brush.Gold, 5, 200f);
					});
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("HandTrickFx.OnPlay: " + ex2);
			}
		}

		public static void Reset()
		{
			HandTrickWipe.Reset();
		}
	}
