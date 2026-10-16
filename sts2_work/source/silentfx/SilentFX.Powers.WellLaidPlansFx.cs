// namespace: SilentFX.Powers
// type: WellLaidPlansFx (class)
// source lines: 19186-19275
	public sealed class WellLaidPlansFx : BasicPowerFx
	{
		private static readonly Color Blue = new Color(0.4f, 0.74f, 1f, 1f);

		private RetainLine _line;

		protected override string NodeName => "WellLaidPlans";

		protected override bool UseFront => false;

		protected override void Build(bool animateIn)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			_line = new RetainLine(Blue, Holds);
			HandFlow.HandDrawBegins += OnHandDraw;
			if (animateIn && Tuning.F("intro.on", 1f) > 0.5f)
			{
				_line.Open();
			}
		}

		protected override void OnStacked()
		{
			if (Tuning.F("intro.on", 1f) > 0.5f)
			{
				_line?.Open();
			}
		}

		private bool Holds(CardModel card)
		{
			if (!RetainLine.Holdable(card))
			{
				return false;
			}
			if (Anchor.Owner == null)
			{
				return true;
			}
			try
			{
				Player owner = card.Owner;
				return ((owner != null) ? owner.Creature : null) == Anchor.Owner;
			}
			catch
			{
				return false;
			}
		}

		private void OnHandDraw(Player player)
		{
			if (!Dead && _line != null && (Anchor.Owner == null || ((player != null) ? player.Creature : null) == Anchor.Owner))
			{
				_line.Release();
			}
		}

		protected override void Tick(float dt)
		{
			_line?.Update(dt);
		}

		protected override void Layout()
		{
		}

		protected override void OnTrigger(Vector2? at)
		{
			_line?.Bind();
		}

		protected override void OnStopping()
		{
			_line?.Release();
		}

		protected override void OnDispose()
		{
			HandFlow.HandDrawBegins -= OnHandDraw;
			_line?.Free();
			_line = null;
		}

		static WellLaidPlansFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
