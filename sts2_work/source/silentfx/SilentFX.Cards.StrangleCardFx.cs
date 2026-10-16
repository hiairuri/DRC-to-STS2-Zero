// namespace: SilentFX.Cards
// type: StrangleCardFx (class)
// source lines: 106595-106634
	public sealed class StrangleCardFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if (!c.HasTarget || (!MultiplayerFx.Active && MultiplayerFx.CurrentInvocation != null))
			{
				return;
			}
			try
			{
				if (!StrangleRope.Tighten(c.Target, c.TargetChest, c.Card, wait: true, c.FrontParent))
				{
					StrangleRope.Begin(c.BackParent, c.FrontParent, c.TargetChest, c.TargetWidth, c.TargetHeight, c.Card, wait: true, c.Target);
				}
				StrangleGripFx.Answer(c.Target, c.Card);
			}
			catch (Exception ex)
			{
				DevLog.Warn("Strangle (played): " + ex.Message);
			}
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if ((MultiplayerFx.Active || MultiplayerFx.CurrentInvocation == null) && !StrangleRope.SnapFor(c.Card, c.Target))
				{
					StrangleRope.Begin(c.BackParent, c.FrontParent, at, c.TargetWidth, c.TargetHeight, null, wait: false, c.Target);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Strangle (hit): " + ex.Message);
			}
		}
	}
