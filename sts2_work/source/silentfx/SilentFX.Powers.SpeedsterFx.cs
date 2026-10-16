// namespace: SilentFX.Powers
// type: SpeedsterFx (class)
// source lines: 13730-14107
	public sealed class SpeedsterFx : BasicPowerFx
	{
		private readonly List<FxTarget> _targetScratch = new List<FxTarget>();

		private static readonly Color Quick = new Color(1f, 0.96f, 0.72f, 0.92f);

		private const int Streaks = 3;

		private const int Dots = 9;

		private Sprite2D[] _far;

		private Sprite2D[] _near;

		private Sprite2D _pool;

		private int _phase;

		private float _t;

		private float _ramp = 0.85f;

		private float _spinLen = 0.85f;

		private float _inLen = 0.2f;

		private float _from;

		private float _to;

		private float _angle;

		private float _size = 1f;

		private bool _shown;

		protected override string NodeName => "Speedster";

		protected override void Build(bool animateIn)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			int num = 27;
			_far = new Sprite2D[num];
			_near = new Sprite2D[num];
			_pool = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
			((CanvasItem)_pool).Visible = false;
			((Node)Front).AddChild((Node)(object)_pool, false, (InternalMode)0);
			for (int i = 0; i < num; i++)
			{
				_far[i] = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
				_near[i] = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
				((CanvasItem)_far[i]).Visible = false;
				((CanvasItem)_near[i]).Visible = false;
				((Node)Back).AddChild((Node)(object)_far[i], false, (InternalMode)0);
				((Node)Front).AddChild((Node)(object)_near[i], false, (InternalMode)0);
			}
			if (animateIn)
			{
				Spin(full: true);
			}
		}

		protected override void OnStacked()
		{
			Spin(full: true);
		}

		private void Spin(bool full)
		{
			_phase = 1;
			_t = 0f;
			_size = (full ? 1f : Mathf.Clamp(Tuning.F("speedster.kick_size", 0.75f), 0.3f, 1.5f));
			_ramp = Mathf.Clamp(full ? Tuning.F("speedster.spin_time", 0.85f) : Tuning.F("speedster.kick_time", 0.32f), 0.1f, 4f);
			_spinLen = _ramp;
			_inLen = Mathf.Clamp(Tuning.F("speedster.sink_time", 0.2f), 0.06f, 1f);
			_from = (float)Math.PI * 2f * (full ? Tuning.F("speedster.spin_from", 2f) : Tuning.F("speedster.kick_from", 4f));
			_to = (float)Math.PI * 2f * Tuning.F("speedster.spin_to", 6.5f);
		}

		protected override void Tick(float dt)
		{
			if (_phase == 0)
			{
				return;
			}
			_t += dt;
			if (_phase == 1)
			{
				float num = Mathf.Clamp(_t / _ramp, 0f, 1f);
				_angle += Mathf.Lerp(_from, _to, num * num) * dt;
				if (_t >= _spinLen)
				{
					_phase = 2;
					_t = 0f;
				}
			}
			else
			{
				float num2 = Mathf.Clamp(_t / _inLen, 0f, 1f);
				_angle += _to * (1f + 1.6f * num2) * dt;
				if (num2 >= 1f)
				{
					_phase = 0;
					Flick();
				}
			}
			if (_angle > (float)Math.PI * 128f)
			{
				_angle -= (float)Math.PI * 128f;
			}
		}

		protected override void Layout()
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0535: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_056e: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_058c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_043f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			if (_near == null || _far == null)
			{
				return;
			}
			if (_phase == 0)
			{
				if (_shown)
				{
					HideAll();
				}
				return;
			}
			Vector2 val = Anchor.SafeBodySize();
			Vector2 val2 = Anchor.Feet() + new Vector2(0f, -10f);
			float num = ((_phase == 1) ? Mathf.Clamp(_t / _ramp, 0f, 1f) : 1f);
			float num2 = ((_phase == 2) ? Mathf.Clamp(_t / _inLen, 0f, 1f) : 0f);
			float num3 = val.X * 0.8f * _size * Mathf.Lerp(1f, 0.7f, Mathf.Pow(num, 1.5f)) * (1f - num2 * num2);
			float num4 = Mathf.Lerp(0.3f, 0.24f, num) * (1f - 0.5f * num2);
			float num5 = num * num * (1f - 0.5f * num2);
			float num6 = Tuning.F("speedster.judder", 8f) * num5 * _size;
			Vector2 val3 = new Vector2(num6 * (Mathf.Sin(Time * 113f) + 0.6f * Mathf.Sin(Time * 71f + 1.3f)) / 1.6f, num6 * 0.45f * Mathf.Sin(Time * 127f + 0.7f));
			float num7 = Mathf.Lerp(0.75f, (float)Math.PI * 16f / 25f, num);
			float num8 = Mathf.Lerp(20f, 30f, num) * (1f - 0.55f * num2) * Mathf.Sqrt(_size);
			float num9 = num * 0.8f;
			Color val4 = new Color(Mathf.Lerp(Brush.Wind.R, Quick.R, num9), Mathf.Lerp(Brush.Wind.G, Quick.G, num9), Mathf.Lerp(Brush.Wind.B, Quick.B, num9), Mathf.Lerp(0.8f, Quick.A, num9));
			float num10 = (Stopping ? A : 1f) * Mathf.Min(1f, ((_phase == 1) ? _t : 1f) * 8f) * (1f - Mathf.SmoothStep(0.75f, 1f, num2));
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					int num11 = i * 9 + j;
					Sprite2D val5 = _near[num11];
					Sprite2D val6 = _far[num11];
					if (FxKit.Alive((GodotObject)(object)val5) && FxKit.Alive((GodotObject)(object)val6))
					{
						float num12 = (float)j / 8f;
						float num13 = _angle + (float)i * ((float)Math.PI * 2f) / 3f - num7 * num12;
						float num14 = 1f + 0.08f * num5 * Mathf.Sin(Time * 90f + (float)i * 2.1f + (float)j * 0.9f);
						float num15 = Mathf.Sin(num13);
						float num16 = Mathf.Cos(num13);
						Vector2 globalPosition = val2 + val3 + new Vector2(num16 * num3 * num14, num15 * num3 * num4 * num14);
						Vector2 val7 = new Vector2((0f - num15) * num3, num16 * num3 * num4);
						float num17 = Mathf.Max(num8, num7 / 8f * val7.Length() * 2.4f);
						bool flag = num15 >= 0f;
						Sprite2D val8 = (flag ? val5 : val6);
						Sprite2D val9 = (flag ? val6 : val5);
						if (((CanvasItem)val9).Visible)
						{
							((CanvasItem)val9).Visible = false;
						}
						if (!((CanvasItem)val8).Visible)
						{
							((CanvasItem)val8).Visible = true;
						}
						((Node2D)val8).GlobalPosition = globalPosition;
						((Node2D)val8).GlobalRotation = ((val7.LengthSquared() > 0.0001f) ? val7.Angle() : 0f);
						((Node2D)val8).Scale = new Vector2(num17, Mathf.Max(1f, num8 * (1f - 0.65f * num12))) / 64f;
						float num18 = (1f - num12) * (1f - num12) * 0.6f;
						float num19 = num10 * (1f - num12) * (1f - 0.5f * num12) * (flag ? 1f : 0.6f);
						((CanvasItem)val8).Modulate = new Color(Mathf.Lerp(val4.R, 1f, num18), Mathf.Lerp(val4.G, 1f, num18), Mathf.Lerp(val4.B, 1f, num18), val4.A * num19);
					}
				}
			}
			if (FxKit.Alive((GodotObject)(object)_pool))
			{
				if (!((CanvasItem)_pool).Visible)
				{
					((CanvasItem)_pool).Visible = true;
				}
				((Node2D)_pool).GlobalPosition = val2 + val3;
				((Node2D)_pool).Scale = new Vector2(num3 * 1.7f + 36f, num3 * num4 * 1.7f + 16f) / 64f;
				((CanvasItem)_pool).Modulate = new Color(val4.R, val4.G, val4.B, num10 * (0.1f + 0.2f * num + 0.35f * num2));
			}
			_shown = true;
		}

		private void HideAll()
		{
			for (int i = 0; i < _near.Length; i++)
			{
				if (FxKit.Alive((GodotObject)(object)_near[i]))
				{
					((CanvasItem)_near[i]).Visible = false;
				}
				if (FxKit.Alive((GodotObject)(object)_far[i]))
				{
					((CanvasItem)_far[i]).Visible = false;
				}
			}
			if (FxKit.Alive((GodotObject)(object)_pool))
			{
				((CanvasItem)_pool).Visible = false;
			}
			_shown = false;
		}

		private void Flick()
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (Stopping || !Anchor.Valid())
				{
					return;
				}
				Node frontParent = Anchor.FrontParent;
				if (FxKit.Alive((GodotObject)(object)frontParent))
				{
					Vector2 val = Anchor.Feet();
					Vector2 val2 = Anchor.SafeBodySize();
					FxKit.Flash(frontParent, val + new Vector2(0f, -8f), new Color(1f, 0.97f, 0.8f, 0.85f), val2.X * 0.55f * _size, 1.5f, 0.12);
					float num = val2.Y * Mathf.Clamp(Tuning.F("speedster.flick", 0.95f), 0f, 2f) * _size;
					if (num > 10f)
					{
						Brush.Dart(frontParent, val + new Vector2(0f, -4f), val + new Vector2(0f, 0f - num), num * 0.55f, 11f, Quick, 0.13f);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Speedster: the flick could not be shown: " + ex.Message);
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			if (!Anchor.Valid())
			{
				return;
			}
			Node parent = Anchor.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			int num;
			if (MultiplayerFx.Active)
			{
				object owner = Anchor.Owner;
				Creature val = (Creature)((owner is Creature) ? owner : null);
				if (val != null)
				{
					num = ((!MultiplayerFx.IsLocal(val)) ? 1 : 0);
					goto IL_005b;
				}
			}
			num = 0;
			goto IL_005b;
			IL_005b:
			bool remote = (byte)num != 0;
			Vector2 val2 = default;
			if (!remote)
			{
				val2 = GameRefs.PilePoint((PileType)1);
				FxKit.Flash(parent, val2, new Color(1f, 0.97f, 0.8f, 0.7f), 54f, 1.6f, 0.12);
			}
			Anchor.SafeTargets(_targetScratch);
			foreach (FxTarget item in _targetScratch)
			{
				Vector2 to = item.Chest;
				float size = Mathf.Max(item.Width / 260f, 1f);
				if (remote)
				{
					FxKit.After(parent, 0.10999999940395355, () =>
					{
						//IL_0031: Unknown result type (might be due to invalid IL or missing references)
						//IL_004a: Unknown result type (might be due to invalid IL or missing references)
						using (RemoteFxFade.Capture(remote, parent))
						{
							FxKit.Flash(parent, to, new Color(1f, 0.97f, 0.8f, 0.8f), 46f * size, 1.7f, 0.12);
						}
					});
				}
				else
				{
					Brush.Dart(parent, val2, to, 260f, 13f, Quick, 0.11f, 0f, () =>
					{
						//IL_000c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0025: Unknown result type (might be due to invalid IL or missing references)
						FxKit.Flash(parent, to, new Color(1f, 0.97f, 0.8f, 0.8f), 46f * size, 1.7f, 0.12);
					});
				}
			}
			if (!(Tuning.F("speedster.trigger_spin", 1f) < 0.5f))
			{
				if (_phase == 1)
				{
					_spinLen = Mathf.Min(Mathf.Max(_spinLen, _t + 0.26f), _ramp + 1.2f);
				}
				else if (_phase == 0)
				{
					Spin(full: false);
				}
			}
		}

		static SpeedsterFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
