// namespace: SilentFX.Cards
// type: MalaiseHeld (class)
// source lines: 75833-75915
	public sealed class MalaiseHeld : HeldSession
	{
		private MalaiseFx.Drain _drain;

		protected override void Build()
		{
			try
			{
				int num = ((C.Card == null) ? Math.Max(0, Tuning.I("malaise.preview_x", 3)) : GameRefs.EnergyOf(C.Card.Owner));
				int num2 = ((C.Card != null) ? EnergyBeads.ChemOf(C.Card, num) : 0);
				bool flag = false;
				try
				{
					flag = C.Card != null && C.Card.IsUpgraded;
				}
				catch
				{
					flag = false;
				}
				_drain = MalaiseFx.Create(C, num, num2, flag, fromCounter: true);
				if (PowerFxManager.LogTriggers && C.Card != null)
				{
					DevLog.Info($"Malaise: {num} bead(s) of energy leave the counter" + ((num2 > 0) ? $", {num2} black one(s) the relic" : "") + (flag ? ", one purple one the card" : ""));
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("MalaiseHeld.Build: " + ex);
				_drain = null;
			}
		}

		protected override void Tick(float dt)
		{
			MalaiseFx.Touch(_drain);
			if (Released && !Playing && ReleasedFor > 6f)
			{
				Leave();
			}
		}

		public override void OnTargetChanged()
		{
			if (C.HasTarget)
			{
				MalaiseFx.Aim(_drain, C);
			}
			else
			{
				MalaiseFx.Unaim(_drain);
			}
		}

		internal MalaiseFx.Drain Take()
		{
			MalaiseFx.Drain drain = _drain;
			_drain = null;
			return drain;
		}

		protected override void OnPlay(CardFxContext play)
		{
			Leave(20f);
		}

		protected override void OnLeaving()
		{
			if (_drain != null)
			{
				MalaiseFx.Dismiss(_drain);
			}
			_drain = null;
		}

		protected override void OnEnd()
		{
			if (_drain != null)
			{
				MalaiseFx.Dismiss(_drain);
			}
			_drain = null;
		}
	}
