// namespace: SilentFX.Cards
// type: SkewerHeld (class)
// source lines: 102341-102439
	public sealed class SkewerHeld : HeldSession
	{
		private SkewerFx.Volley _volley;

		protected override void Build()
		{
			try
			{
				int num = ((C.Card == null) ? Math.Max(0, Tuning.I("skewer.preview_count", 3)) : GameRefs.EnergyOf(C.Card.Owner));
				int num2 = ((C.Card != null) ? EnergyBeads.ChemOf(C.Card, num) : 0);
				_volley = SkewerFx.Create(C, num, num2, fromCounter: true);
				if (PowerFxManager.LogTriggers && C.Card != null)
				{
					DevLog.Info($"Skewer: {num} bead(s) of energy leave the counter" + ((num2 > 0) ? $", {num2} black one(s) the relic" : ""));
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("SkewerHeld.Build: " + ex);
				_volley = null;
			}
		}

		protected override void Tick(float dt)
		{
			SkewerFx.Touch(_volley);
			if (Released && !Playing && ReleasedFor > 6f)
			{
				Leave();
			}
		}

		public override void OnTargetChanged()
		{
			if (C.HasTarget)
			{
				SkewerFx.Aim(_volley, C);
			}
			else
			{
				SkewerFx.Unaim(_volley);
			}
		}

		internal SkewerFx.Volley Take(CardFxContext play)
		{
			SkewerFx.Volley volley = _volley;
			if (MultiplayerFx.Active)
			{
				if (volley != null && !volley.Playing && !volley.Dismissed && volley.Card == play.Card)
				{
					Creature owner = volley.Owner;
					CardModel card = play.Card;
					object obj;
					if (card == null)
					{
						obj = null;
					}
					else
					{
						Player owner2 = card.Owner;
						obj = ((owner2 != null) ? owner2.Creature : null);
					}
					if (owner == obj && volley.Remote == play.Remote && (object)volley.Room == GameRefs.CombatRoom)
					{
						goto IL_0072;
					}
				}
				return null;
			}
			goto IL_0072;
			IL_0072:
			_volley = null;
			return volley;
		}

		protected override void OnPlay(CardFxContext play)
		{
			Leave(20f);
		}

		protected override void OnLeaving()
		{
			if (_volley != null)
			{
				SkewerFx.Dismiss(_volley);
			}
			_volley = null;
		}

		protected override void OnEnd()
		{
			if (_volley != null)
			{
				SkewerFx.Dismiss(_volley);
			}
			_volley = null;
		}
	}
