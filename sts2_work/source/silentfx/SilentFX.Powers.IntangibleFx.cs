// namespace: SilentFX.Powers
// type: IntangibleFx (class)
// source lines: 7113-7274
	public sealed class IntangibleFx : PowerFx
	{
		private Node2D _marker;

		private CanvasItem _body;

		private Color _bodyOriginal = Colors.White;

		private bool _bodyTinted;

		private float _intensity;

		private float _target = 1f;

		private float _pulse;

		private bool _stopping;

		private bool _dead;

		private bool _held;

		public override bool IsAlive
		{
			get
			{
				if (!_dead)
				{
					return FxKit.Alive((GodotObject)(object)_marker);
				}
				return false;
			}
		}

		public override void Start(bool animateIn)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected Obj, but got Unknown
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			_marker = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Intangible")
			};
			Anchor.FrontParent.AddChild((Node)(object)_marker, false, (InternalMode)0);
			try
			{
				_body = Anchor.Body?.Invoke();
				if (FxKit.Alive((GodotObject)(object)_body))
				{
					_bodyOriginal = _body.Modulate;
					_bodyTinted = true;
				}
			}
			catch
			{
				_body = null;
			}
			_intensity = (animateIn ? 0f : 1f);
			_pulse = (animateIn ? 1f : 0f);
			Apply();
		}

		public override void Update(double dt)
		{
			if (_dead)
			{
				return;
			}
			if (!Anchor.Valid())
			{
				Dispose();
				return;
			}
			float num = _target;
			if (!_stopping)
			{
				int num2 = WraithFormIntroFx.PhaseFor(Anchor.Owner);
				if (num2 == 1 || num2 == 2)
				{
					_held = true;
					num = Mathf.Min(num, Mathf.Clamp(Tuning.F("wraith.intro.body_before", 0.25f), 0f, 1f));
					_pulse = 0f;
				}
				else if (_held)
				{
					_held = false;
					_intensity = 1f;
					_pulse = 1f;
				}
			}
			_intensity = FxKit.Damp(_intensity, num, _stopping ? 4.5f : 2.6f, dt);
			_pulse = FxKit.Damp(_pulse, 0f, 2.4f, dt);
			Apply();
			if (_stopping && _intensity < 0.03f)
			{
				Dispose();
			}
		}

		public override void Trigger(Vector2? at = null)
		{
			if (!_dead && !_stopping)
			{
				_pulse = 1f;
			}
		}

		public override void Stop(bool animateOut)
		{
			if (!_dead)
			{
				if (animateOut && FxKit.Alive((GodotObject)(object)_marker) && ((Node)_marker).IsInsideTree())
				{
					_stopping = true;
					_target = 0f;
				}
				else
				{
					Dispose();
				}
			}
		}

		private void Apply()
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			if (_bodyTinted && FxKit.Alive((GodotObject)(object)_body))
			{
				float num = Tuning.F("wraith.body_alpha", 0.6f) - 0.28f * _pulse;
				Color val = new Color(_bodyOriginal.R * 0.74f, _bodyOriginal.G * 0.8f, _bodyOriginal.B, _bodyOriginal.A * num);
				_body.Modulate = _bodyOriginal.Lerp(val, Mathf.Clamp(_intensity, 0f, 1f));
			}
		}

		private void Dispose()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (!_dead)
			{
				_dead = true;
				if (_bodyTinted && FxKit.Alive((GodotObject)(object)_body))
				{
					_body.Modulate = _bodyOriginal;
				}
				_bodyTinted = false;
				if (FxKit.Alive((GodotObject)(object)_marker))
				{
					((Node)_marker).QueueFree();
				}
			}
		}

		public IntangibleFx()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		}
	}
