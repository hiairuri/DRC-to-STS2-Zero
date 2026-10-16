// namespace: SilentFX.Cards
// type: BulletTimeHeld (class)
// source lines: 49076-49172
	public sealed class BulletTimeHeld : HeldSession
	{
		private static readonly StringName PAlpha = StringName.op_Implicit("alpha");

		private Node2D _clock;

		private ShaderMaterial _mat;

		private Line2D _minute;

		private Line2D _hour;

		private float _turned;

		private float _speed;

		private bool _played;

		private bool _wasFrozen;

		private float _since;

		protected override void Build()
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			_clock = BulletTimeFx.MakeClock(Tuning.F("bullet.clock_size", 330f), out _mat, out _minute, out _hour);
			((Node)Back).AddChild((Node)(object)_clock, false, (InternalMode)0);
			_clock.GlobalPosition = C.Chest;
			_turned = 0.9f;
		}

		protected override void Tick(float dt)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)_clock))
			{
				return;
			}
			_clock.GlobalPosition = C.Chest;
			bool flag = _played && !C.Remote && TimeFreeze.Active;
			if (flag)
			{
				_wasFrozen = true;
				_since = 0f;
			}
			float target;
			if (_played)
			{
				target = ((_wasFrozen && !flag) ? 5f : 0f);
			}
			else
			{
				target = Mathf.Lerp(Tuning.F("bullet.hover_speed", 0.7f), Tuning.F("bullet.held_speed", 9f), Lift) + 6f * Arm;
			}
			_speed = FxKit.Damp(_speed, target, _played ? 5f : 6f, dt);
			_turned += _speed * dt;
			float num = Mathf.Lerp(0.55f, 0.95f, Mathf.Max(Lift, _played ? 1f : 0f));
			if (FxKit.Alive((GodotObject)(object)_minute))
			{
				((Node2D)_minute).Rotation = _turned;
				((CanvasItem)_minute).Modulate = new Color(1f, 1f, 1f, num);
			}
			if (FxKit.Alive((GodotObject)(object)_hour))
			{
				((Node2D)_hour).Rotation = _turned / 6f - 1f;
				((CanvasItem)_hour).Modulate = new Color(1f, 1f, 1f, num);
			}
			if (FxKit.Alive((GodotObject)(object)_mat))
			{
				_mat.SetShaderParameter(PAlpha, Variant.op_Implicit(0.9f * num));
			}
			if (_played)
			{
				_since += dt;
				if (_since > Tuning.F("bullet.clock_time", 1.5f) - 0.4f)
				{
					Leave(3.5f);
				}
			}
		}

		protected override void OnPlay(CardFxContext play)
		{
			_played = true;
		}

		public override void OnCardFinished()
		{
			if (!_played)
			{
				Leave();
			}
		}
	}
