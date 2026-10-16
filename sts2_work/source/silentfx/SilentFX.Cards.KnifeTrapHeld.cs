// namespace: SilentFX.Cards
// type: KnifeTrapHeld (class)
// source lines: 71824-71914
	public sealed class KnifeTrapHeld : HeldSession
	{
		private KnifeTrapFx.Trap _trap;

		private int _shivs;

		private bool _rang;

		protected override void Build()
		{
			int num = (_shivs = ((C.Card == null) ? Tuning.I("trap.blades", 8) : KnifeTrapFx.CountSpentShivs(C.Card)));
			_trap = KnifeTrapFx.Create(C, num, fromPile: true);
			if (PowerFxManager.LogTriggers && C.Card != null)
			{
				DevLog.Info($"Knife Trap: {num} spent Shiv(s) rise from the exhaust pile");
			}
		}

		protected override void Tick(float dt)
		{
			if (Hovering)
			{
				_rang = false;
			}
			else if (!C.Remote && !_rang && _shivs > 0 && !Leaving && !Released && !Playing)
			{
				_rang = true;
				Sfx.Play("trap_ring");
			}
		}

		protected override void OnPlay(CardFxContext play)
		{
			if (C.Remote && !_rang && _shivs > 0)
			{
				_rang = true;
				Sfx.Play("trap_ring");
			}
			base.OnPlay(play);
		}

		public override void OnTargetChanged()
		{
			if (C.Card != null)
			{
				Player owner = C.Card.Owner;
				if (ShivFan.Active((owner != null) ? owner.Creature : null))
				{
					KnifeTrapFx.Unaim(_trap);
					return;
				}
			}
			if (C.HasTarget)
			{
				KnifeTrapFx.Aim(_trap, C);
			}
			else
			{
				KnifeTrapFx.Unaim(_trap);
			}
		}

		internal KnifeTrapFx.Trap Take()
		{
			KnifeTrapFx.Trap trap = _trap;
			_trap = null;
			return trap;
		}

		protected override void OnLeaving()
		{
			if (_trap != null)
			{
				KnifeTrapFx.Dismiss(_trap);
			}
			_trap = null;
		}

		protected override void OnEnd()
		{
			if (C.Remote && _trap != null)
			{
				KnifeTrapFx.Free(_trap);
			}
			else if (_trap != null)
			{
				KnifeTrapFx.Dismiss(_trap);
			}
			_trap = null;
		}
	}
