// namespace: SilentFX.Cards
// type: GambleHeld (class)
// source lines: 60175-60410
	public sealed class GambleHeld : HeldSession
	{
		private const float LineSeconds = 20f;

		private static GambleHeld _liner;

		private bool _done;

		private bool _started;

		private float _since;

		private float _spacing = -1f;

		private float _waited;

		private float _playedFor;

		private ulong _rearmAt;

		private bool _readied;

		private Sprite2D _ground;

		internal static bool HandOver()
		{
			GambleHeld liner = _liner;
			_liner = null;
			if (liner == null)
			{
				return false;
			}
			liner._done = true;
			liner._readied = false;
			return true;
		}

		internal static void Forget()
		{
			_liner = null;
		}

		protected override void Build()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Expected Obj, but got Unknown
			try
			{
				if (!(Tuning.F("gamble.ground", 0.3f) <= 0.01f) && FxKit.Alive((GodotObject)(object)Front))
				{
					_ground = new Sprite2D
					{
						Name = StringName.op_Implicit("Ground"),
						Texture = FxKit.SoftDot,
						Modulate = new Color(0f, 0f, 0f, 0f)
					};
					((Node)Front).AddChild((Node)(object)_ground, false, (InternalMode)0);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleHeld.Build: " + ex.Message);
				_ground = null;
			}
		}

		protected override void Tick(float dt)
		{
			if (_done)
			{
				if (!Leaving)
				{
					Leave(16f);
				}
				return;
			}
			try
			{
				if (Leaving)
				{
					LetGo();
					return;
				}
				if (Playing)
				{
					_playedFor += dt;
					if (_liner != this || _playedFor > Mathf.Clamp(Tuning.F("gamble.play_wait", 1.2f), 0.2f, 5f))
					{
						LetGo();
						Leave(16f);
					}
					return;
				}
				if (Hovering)
				{
					if (_started)
					{
						_started = false;
						Drop();
					}
					return;
				}
				if (Released)
				{
					_waited += dt;
					if (_waited > Mathf.Clamp(Tuning.F("gamble.queue_wait", 3f), 0.5f, 6f))
					{
						LetGo();
						Leave();
						return;
					}
				}
				if (!_started)
				{
					if (Time <= 0f || Lift < Tuning.F("gamble.pick_lift", 0f))
					{
						return;
					}
					if (Tuning.F("gamble.held_row", 1f) < 0.5f || HandCards.Selecting)
					{
						LetGo();
						return;
					}
					_started = true;
					_since = 0f;
					_spacing = -1f;
				}
				else
				{
					_since += dt;
				}
				Lay();
				GambleFlow.HeldGround(_ground);
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleHeld: " + ex);
				LetGo();
			}
		}

		private void Lay()
		{
			float num = Mathf.Clamp(Tuning.F("gamble.breathe", 0.1f), 0f, 0.3f);
			if (_since < num && _liner != this)
			{
				if (!_readied)
				{
					_readied = true;
					GambleFlow.Prepare(num, Mathf.Clamp(Tuning.F("gamble.breathe_px", 7f), 0f, 30f));
				}
				return;
			}
			int count = GambleFlow.HandCount();
			bool flag = _since >= num + Mathf.Clamp(Tuning.F("gamble.gather", 0.3f), 0.05f, 1.5f);
			float num2 = (flag ? GambleFlow.HeldOpen(count) : GambleFlow.HeldTight(count));
			ulong milliseconds = FxClock.Milliseconds;
			bool flag2 = _liner == this && Mathf.Abs(num2 - _spacing) < 0.01f;
			if (flag2 && milliseconds < _rearmAt && HandHold.Lined)
			{
				GambleFlow.Sideways(on: true);
				return;
			}
			_spacing = num2;
			_rearmAt = milliseconds + 8000;
			_liner = this;
			_readied = false;
			HandHold.Line(num2, 20f, GambleFlow.RowLift());
			if (HandHold.Lined)
			{
				GambleFlow.Sideways(on: true);
				if (!flag2)
				{
					GambleFlow.RowMoves(flag, num2);
				}
			}
			else
			{
				_liner = null;
				_done = true;
				GambleFlow.Sideways(on: false);
			}
		}

		protected override void OnPlay(CardFxContext play)
		{
			_playedFor = 0f;
		}

		protected override void OnLeaving()
		{
			LetGo();
		}

		protected override void OnEnd()
		{
			LetGo();
		}

		private void LetGo()
		{
			_done = true;
			Drop();
		}

		private void Drop()
		{
			bool flag = _liner == this;
			if (_readied)
			{
				_readied = false;
				if (!flag)
				{
					GambleFlow.Unprepare();
				}
			}
			if (!flag)
			{
				return;
			}
			_liner = null;
			GambleFlow.Sideways(on: false);
			try
			{
				HandHold.Release();
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleHeld (release): " + ex);
			}
		}
	}
