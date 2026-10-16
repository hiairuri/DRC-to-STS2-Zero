// namespace: SilentFX.Cards
// type: SuppressHeld (class)
// source lines: 108633-108856
	public sealed class SuppressHeld : HeldSession
	{
		private SuppressShow _show;

		private SuppressRun _run;

		private bool _v3;

		private bool _lifted;

		private bool _handed;

		internal bool Old => !_v3;

		protected override void Build()
		{
			try
			{
				if (SuppressStage.Ready)
				{
					_run = SuppressRun.Create(C);
					_v3 = _run != null;
				}
				if (!_v3 && !C.Remote)
				{
					_show = SuppressShow.Create(C);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("SuppressHeld.Build: " + ex);
				_show = null;
				_run = null;
			}
		}

		protected override void Tick(float dt)
		{
			if (_v3)
			{
				TickNew();
				return;
			}
			SuppressShow show = _show;
			if (show != null && !show.Dead)
			{
				show.OwnedAt = show.Time;
				show.Presence = Mathf.Clamp(Fade, 0f, 1f);
				show.Lifted = !Hovering && (C.Card == null || Released || C.HasTarget || CarriedOut());
				show.Aimed = C.HasTarget;
				if (Released && !Playing && ReleasedFor > 6f)
				{
					Leave();
				}
			}
		}

		private bool CarriedOut()
		{
			try
			{
				return C.Card != null && HeldFxManager.Carried == C.Card && HeldFxManager.CarriedOut;
			}
			catch
			{
				return false;
			}
		}

		private void TickNew()
		{
			if (_handed)
			{
				return;
			}
			if (Released && !Playing && ReleasedFor > 6f)
			{
				Leave();
				return;
			}
			if (_run == null || _run.Dead)
			{
				if (Leaving)
				{
					return;
				}
				_run = SuppressRun.Create(C);
				_lifted = false;
				if (_run == null)
				{
					return;
				}
			}
			_run.SetPresence(Mathf.Clamp(Fade, 0f, 1f));
			bool flag = !Hovering && (C.Remote || C.Card == null || Released || C.HasTarget || CarriedOut());
			if (flag && !_lifted)
			{
				_lifted = true;
				_run.Lift();
				if (C.HasTarget)
				{
					_run.Aim(C);
				}
			}
			else if (!flag && _lifted && !Released)
			{
				_run.Cancel();
				_run = SuppressRun.Create(C);
				_lifted = false;
			}
		}

		public override void OnTargetChanged()
		{
			if (!_v3)
			{
				_show?.SetTarget(C);
			}
			else if (_run != null && !_run.Dead && !_handed && _lifted)
			{
				if (C.HasTarget)
				{
					_run.Aim(C);
				}
				else
				{
					_run.Unaim();
				}
			}
		}

		protected override void OnRelease()
		{
			if (_v3 && _run != null && !_run.Dead && !_handed)
			{
				if (!_lifted)
				{
					_lifted = true;
					_run.Lift();
				}
				if (C.HasTarget)
				{
					_run.Aim(C);
				}
				_run.Release();
			}
		}

		internal SuppressRun HandOver()
		{
			if (!_v3 || _run == null || _run.Dead)
			{
				return null;
			}
			_handed = true;
			return _run;
		}

		internal SuppressShow Take()
		{
			SuppressShow show = _show;
			_show = null;
			return show;
		}

		protected override void OnPlay(CardFxContext play)
		{
			if (!_v3 || !_handed)
			{
				Leave(20f);
			}
		}

		public override void OnCardFinished()
		{
			if (_v3)
			{
				_run?.CardFinished();
			}
			Leave();
		}

		protected override void OnLeaving()
		{
			if (_v3)
			{
				if (!_handed)
				{
					_run?.Dismiss();
				}
				else
				{
					_run?.CardFinished();
				}
				_run = null;
			}
			else
			{
				_show?.Dismiss();
				_show = null;
			}
		}

		protected override void OnEnd()
		{
			if (_v3)
			{
				if (!_handed)
				{
					_run?.Dismiss();
				}
				else
				{
					_run?.CardFinished();
				}
				_run = null;
			}
			else
			{
				_show?.Dismiss();
				_show = null;
			}
		}
	}
