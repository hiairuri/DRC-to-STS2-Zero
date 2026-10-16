// namespace: SilentFX.Cards
// type: RicochetFx (class)
// source lines: 89129-89154
	public sealed class RicochetFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			try
			{
				RicochetRun.Begin(c);
			}
			catch (Exception ex)
			{
				DevLog.Warn("RicochetFx.OnPlay: " + ex);
			}
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			try
			{
				RicochetRun.Hit(c);
			}
			catch (Exception ex)
			{
				DevLog.Warn("RicochetFx.OnHit: " + ex);
			}
		}
	}
