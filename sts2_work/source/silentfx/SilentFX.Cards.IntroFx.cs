// namespace: SilentFX.Cards
// type: IntroFx (class)
// source lines: 69099-69141
	public sealed class IntroFx : CardFx
	{
		private readonly float _life;

		private readonly IntroSpot _spot;

		private readonly Action<CardFxContext, Node2D> _play;

		public IntroFx(float life, IntroSpot spot, Action<CardFxContext, Node2D> play)
		{
			_life = life;
			_spot = spot;
			_play = play;
		}

		public override void OnPlay(CardFxContext c)
		{
			try
			{
				if (c == null || _play == null || Tuning.F("intro.on", 1f) < 0.5f)
				{
					return;
				}
				Node2D val = Intro.Stage(c, _spot, _life);
				if (val == null)
				{
					return;
				}
				try
				{
					_play(c, val);
				}
				catch (Exception ex)
				{
					DevLog.Warn("Opening failed: " + ex.Message);
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("IntroFx.OnPlay: " + ex2);
			}
		}
	}
