// namespace: SilentFX.Cards
// type: AnticipateFx (class)
// source lines: 48560-48594
	public sealed class AnticipateFx : CardFx
	{
		private static readonly Color Read = new Color(0.72f, 1f, 0.82f, 1f);

		public override void OnPlay(CardFxContext c)
		{
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node parent = c.FrontParent;
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					float size = Mathf.Max(c.Body.X, c.Body.Y) * 2.1f;
					FxKit.RingPulse(parent, c.Chest, size, Read, 0f, 0.3f, 1f, 0.38f, 0.9f, 0f, 0.07f, 0.55f);
					FxKit.After(parent, 0.2199999988079071, () =>
					{
						//IL_000c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0034: Unknown result type (might be due to invalid IL or missing references)
						FxKit.Flash(parent, c.Chest, new Color(Read.R, Read.G, Read.B, 0.4f), c.Body.X * 0.8f, 1.3f, 0.16);
					});
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}

		static AnticipateFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
