// namespace: SilentFX.Cards
// type: OutbreakHeld (class)
// source lines: 81892-82107
	public sealed class OutbreakHeld : HeldSession
	{
		private sealed class Reach
		{
			public Vector2 To;

			public float Width;

			public float Seed;

			public Line2D Soft;

			public Line2D Core;

			public Sprite2D Head;

			public Sprite2D Dome;

			public Sprite2D Bump;
		}

		private readonly List<Reach> _reaches = new List<Reach>();

		private readonly Vector2[] _points = new Vector2[21];

		private Sprite2D _well;

		private ColorRect _veil;

		private float _grow;

		private float _strain;

		public float Grow => _grow;

		protected override void Build()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			Color venom = PoisonFx.Venom;
			if (!C.Remote)
			{
				_veil = FxKit.Veil(new Color(0.1f, 0.34f, 0.06f, 0f));
				((Node)Back).AddChild((Node)(object)_veil, false, (InternalMode)0);
			}
			_well = FxKit.Glow(new Color(venom.R, venom.G, venom.B, 0f), new Vector2(64f, 64f));
			((Node)Back).AddChild((Node)(object)_well, false, (InternalMode)0);
			((Node2D)_well).GlobalPosition = C.Feet;
			int num = 0;
			foreach (FxTarget enemy in C.Enemies)
			{
				if (_reaches.Count >= 6)
				{
					break;
				}
				Reach reach = new Reach
				{
					To = enemy.Feet,
					Width = Mathf.Clamp(enemy.Width, 130f, 380f),
					Seed = 1.7f * (float)num++
				};
				reach.Dome = FxKit.Glow(new Color(venom.R, venom.G, venom.B, 0f), new Vector2(64f, 64f));
				((Node)Back).AddChild((Node)(object)reach.Dome, false, (InternalMode)0);
				reach.Bump = FxKit.Glow(new Color(0.9f, 1f, 0.78f, 0f), new Vector2(64f, 64f));
				((Node)Back).AddChild((Node)(object)reach.Bump, false, (InternalMode)0);
				reach.Soft = OutbreakLook.Line((Node)(object)Back, 26f, 0.22f);
				reach.Core = OutbreakLook.Line((Node)(object)Back, 7f, 1f);
				reach.Head = FxKit.Glow(new Color(0.92f, 1f, 0.8f, 0f), new Vector2(64f, 64f));
				((Node)Back).AddChild((Node)(object)reach.Head, false, (InternalMode)0);
				_reaches.Add(reach);
			}
		}

		protected override void Tick(float dt)
		{
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_051e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0559: Unknown result type (might be due to invalid IL or missing references)
			//IL_0566: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			float num = (Armed ? 1f : 0.34f);
			float grow = _grow;
			float num2;
			if (num > _grow)
			{
				num2 = (Armed ? Tuning.F("outbreak.creep_fast", 1.5f) : Tuning.F("outbreak.creep", 1f));
			}
			else
			{
				num2 = 2.5f;
			}
			_grow = Mathf.MoveToward(_grow, num, Mathf.Max(0.1f, num2) * dt);
			bool flag = _grow > grow + 5E-05f;
			if ((!C.Remote & flag) && !Hovering && !Leaving && !Playing)
			{
				Sfx.Hold("outbreak_creep", 1f, 0.92f + 0.3f * Mathf.Clamp(_grow, 0f, 1f), 0.12f);
			}
			bool flag2 = _grow >= 0.999f;
			_strain = Mathf.MoveToward(_strain, flag2 ? 1f : 0f, dt / (flag2 ? 1.4f : 0.25f));
			Color venom = PoisonFx.Venom;
			Vector2 feet = C.Feet;
			if (!C.Remote && FxKit.Alive((GodotObject)(object)_veil))
			{
				_veil.Color = new Color(0.1f, 0.34f, 0.06f, Tuning.F("outbreak.held_veil", 0.16f) * Arm);
			}
			if (FxKit.Alive((GodotObject)(object)_well))
			{
				float num3 = 0.85f + 0.15f * Mathf.Sin(Time * (3f + 5f * _strain));
				((Node2D)_well).Scale = new Vector2(C.Body.X * (1.5f + 0.4f * Arm), C.Body.X * 0.36f) / 64f;
				((CanvasItem)_well).Modulate = new Color(venom.R, venom.G, venom.B, (0.3f + 0.25f * Arm) * num3);
			}
			foreach (Reach reach in _reaches)
			{
				OutbreakLook.Lay(reach.Soft, Back, _points, feet, reach.To, _grow, reach.Seed, Time);
				OutbreakLook.Lay(reach.Core, Back, _points, feet, reach.To, _grow, reach.Seed, Time);
				if (FxKit.Alive((GodotObject)(object)reach.Head))
				{
					Vector2 val = OutbreakLook.Point(feet, reach.To, _grow, reach.Seed, Time);
					Vector2 val2 = OutbreakLook.Point(feet, reach.To, Mathf.Max(0f, _grow - 0.02f), reach.Seed, Time);
					((Node2D)reach.Head).GlobalPosition = val;
					Sprite2D head = reach.Head;
					Vector2 val3 = val - val2;
					float globalRotation;
					if (!(val3.LengthSquared() > 0.01f))
					{
						globalRotation = 0f;
					}
					else
					{
						val3 = val - val2;
						globalRotation = val3.Angle();
					}
					((Node2D)head).GlobalRotation = globalRotation;
					((Node2D)reach.Head).Scale = (flag ? new Vector2(96f, 30f) : new Vector2(46f, 34f)) / 64f;
					((CanvasItem)reach.Head).Modulate = new Color(0.92f, 1f, 0.8f, 0.9f * Mathf.Min(1f, _grow * 8f) * (1f - 0.8f * _strain));
				}
				float num4 = 1f + (0.02f + 0.05f * _strain) * Mathf.Sin(Time * (9f + 16f * _strain) + reach.Seed);
				float num5 = (flag2 ? Mathf.Min(1f, 0.35f + _strain) : _strain);
				if (FxKit.Alive((GodotObject)(object)reach.Dome))
				{
					((Node2D)reach.Dome).GlobalPosition = reach.To;
					((Node2D)reach.Dome).Scale = new Vector2(reach.Width * (0.8f + 0.6f * _strain) * num4, reach.Width * (0.2f + 0.1f * _strain)) / 64f;
					((CanvasItem)reach.Dome).Modulate = new Color(venom.R, venom.G, venom.B, (0.34f + 0.3f * _strain) * num5);
				}
				if (FxKit.Alive((GodotObject)(object)reach.Bump))
				{
					float num6 = (6f + 16f * _strain) * num4;
					((Node2D)reach.Bump).GlobalPosition = reach.To + new Vector2(0f, 0f - num6);
					((Node2D)reach.Bump).Scale = new Vector2(reach.Width * (0.34f + 0.22f * _strain), 14f + 22f * _strain) / 64f;
					((CanvasItem)reach.Bump).Modulate = new Color(0.9f, 1f, 0.78f, (0.2f + 0.45f * _strain) * num5);
				}
			}
		}
	}
