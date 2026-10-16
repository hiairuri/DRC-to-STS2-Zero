// namespace: SilentFX.Cards
// type: SuppressFx (class)
// source lines: 108558-108632
	public sealed class SuppressFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			try
			{
				SuppressHeld suppressHeld = c.Held as SuppressHeld;
				SuppressRun suppressRun = suppressHeld?.HandOver();
				if (suppressRun != null && !suppressRun.Dead)
				{
					suppressRun.Played(c, fromStandby: true);
					Log("attack from the standby");
					return;
				}
				if (suppressHeld == null || !suppressHeld.Old)
				{
					suppressRun = SuppressRun.Create(c);
					if (suppressRun != null)
					{
						suppressRun.Played(c, fromStandby: false);
						Log("attack without a standby");
						return;
					}
				}
				if (!c.Remote)
				{
					SuppressShow suppressShow = suppressHeld?.Take();
					if (suppressShow != null && suppressShow.Dead)
					{
						suppressShow = null;
					}
					if (suppressShow == null)
					{
						suppressShow = SuppressShow.Create(c);
					}
					if (suppressShow != null)
					{
						suppressShow.Play(c, c.Card == null);
						Log("attack (old apparition) " + ((c.Held != null) ? "from the standby" : "without a standby"));
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("SuppressFx.OnPlay: " + ex);
			}
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			try
			{
				SuppressRun.For(c.Card)?.Hit(killed);
				SuppressShow.PlayingFor(c.Card)?.Hit(killed);
			}
			catch (Exception ex)
			{
				DevLog.Warn("SuppressFx.OnHit: " + ex);
			}
		}

		private static void Log(string what)
		{
			if (PowerFxManager.LogTriggers)
			{
				DevLog.Info("Suppress: " + what);
			}
		}

		public static void Reset()
		{
			SuppressShow.Reset();
			SuppressRun.Reset();
		}
	}
