// namespace: SilentFX.Powers
// type: InfiniteBladesFx (class)
// source lines: 6396-7112
	public sealed class InfiniteBladesFx : PowerFx
	{
		private sealed class Blade
		{
			public Node2D Node;

			public Line2D Trail;

			public float Offset;

			public float LastAt = -1f;

			public bool Unborn = true;

			public Vector2 Pos;

			public float Shown;
		}

		private const int Samples = 256;

		private const int LoopPoints = 72;

		private const int TrailPoints = 7;

		private static readonly Color Steel = new Color(0.75f, 0.92f, 1f, 1f);

		private static float[] _byDistance;

		private Node2D _root;

		private Line2D _loop;

		private Sprite2D _glow;

		private readonly List<Blade> _blades = new List<Blade>();

		private readonly Vector2[] _trail = new Vector2[7];

		private float _lead;

		private float _boost;

		private float _time;

		private float _alpha;

		private float _alphaTarget = 1f;

		private float _drawn = 1f;

		private float _wait;

		private float _flare;

		private int _shownPoints = -1;

		private bool _stopping;

		private bool _dead;

		private bool _opening;

		private static ulong _streamAt;

		private bool RemoteOwner
		{
			get
			{
				if (MultiplayerFx.Active)
				{
					object obj = Anchor?.Owner;
					Creature val = (Creature)((obj is Creature) ? obj : null);
					if (val != null)
					{
						return !MultiplayerFx.IsLocal(val);
					}
				}
				return false;
			}
		}

		public override bool IsAlive
		{
			get
			{
				if (!_dead)
				{
					return FxKit.Alive((GodotObject)(object)_root);
				}
				return false;
			}
		}

		public static void StreamArrives(float seconds, Creature owner = null)
		{
			ulong num = FxClock.Milliseconds + (ulong)(Mathf.Clamp(seconds, 0f, 4f) * 1000f);
			if (!MultiplayerFx.Active || owner == null || MultiplayerFx.IsLocal(owner))
			{
				_streamAt = num;
			}
			if (MultiplayerFx.Active && owner != null)
			{
				MultiplayerFx.For(owner).BladesArriveAt = num;
			}
		}

		private static Node2D MakeBlade(float length)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			return FxKit.Blade(length, length * 0.15f, new Color(0.94f, 0.97f, 1f, 1f), new Color(0.6f, 0.67f, 0.78f, 1f), new Color(0.34f, 0.27f, 0.22f, 1f), new Color(0.55f, 0.85f, 1f, 0.3f));
		}

		private static int CountFor(int amount)
		{
			return Mathf.Clamp(amount, 1, Mathf.Max(1, Tuning.I("blades.max", 6)));
		}

		private static void EnsureTable()
		{
			if (_byDistance != null)
			{
				return;
			}
			float[] array = new float[257];
			float num = 0f;
			for (int i = 1; i <= 256; i++)
			{
				float num2 = (float)Math.PI * 2f * ((float)i - 0.5f) / 256f;
				float num3 = Mathf.Cos(num2);
				float num4 = Mathf.Cos(2f * num2);
				num = (array[i] = num + Mathf.Sqrt(num3 * num3 + num4 * num4) * ((float)Math.PI / 128f));
			}
			_byDistance = new float[257];
			int j = 0;
			for (int k = 0; k <= 256; k++)
			{
				float num5;
				for (num5 = num * (float)k / 256f; j < 255 && array[j + 1] < num5; j++)
				{
				}
				float num6 = Mathf.Max(1E-06f, array[j + 1] - array[j]);
				_byDistance[k] = (float)Math.PI * 2f * ((float)j + Mathf.Clamp((num5 - array[j]) / num6, 0f, 1f)) / 256f;
			}
		}

		private static float ParamAt(float distance)
		{
			float num = (distance - Mathf.Floor(distance)) * 256f;
			int num2 = Mathf.Min((int)num, 255);
			return (float)Math.PI + Mathf.Lerp(_byDistance[num2], _byDistance[num2 + 1], num - (float)num2);
		}

		private static Vector2 OnSign(float distance, float across, float tall)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			float num = ParamAt(distance);
			return new Vector2(Mathf.Sin(num) * across, Mathf.Sin(2f * num) * tall);
		}

		private void Measure(out Vector2 centre, out float across, out float tall)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = Anchor.SafeBodySize();
			centre = Anchor.Chest() + new Vector2((0f - val.X) * Tuning.F("blades.x", 0.48f) * Anchor.Side(), (0f - val.Y) * Tuning.F("blades.y", 0.16f) + Mathf.Sin(_time * 1.3f) * 3f);
			across = val.X * Tuning.F("blades.reach", 0.22f) + 22f;
			tall = across * Tuning.F("blades.tall", 0.42f);
		}

		public override void Start(bool animateIn)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected Obj, but got Unknown
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Expected Obj, but got Unknown
			try
			{
				EnsureTable();
				_root = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_Blades")
				};
				Anchor.BackParent.AddChild((Node)(object)_root, false, (InternalMode)0);
				_glow = FxKit.Glow(new Color(Steel.R, Steel.G, Steel.B, 0f), new Vector2(64f, 64f));
				((Node)_root).AddChild((Node)(object)_glow, false, (InternalMode)0);
				_loop = new Line2D
				{
					Width = 2.2f,
					DefaultColor = Colors.White,
					JointMode = (LineJointMode)2,
					BeginCapMode = (LineCapMode)2,
					EndCapMode = (LineCapMode)2,
					Material = (Material)(object)FxKit.Additive,
					Antialiased = true
				};
				((Node)_root).AddChild((Node)(object)_loop, false, (InternalMode)0);
				Resize(CountFor(Amount));
				_alpha = (animateIn ? 0f : 1f);
				_boost = (animateIn ? 2f : 0f);
				_drawn = (animateIn ? 0f : 1f);
				_lead = 0.995f;
				if (!animateIn)
				{
					foreach (Blade blade in _blades)
					{
						blade.Unborn = false;
					}
					goto IL_01db;
				}
				ulong milliseconds = FxClock.Milliseconds;
				ulong num;
				if (MultiplayerFx.Active)
				{
					object owner = Anchor.Owner;
					Creature val = (Creature)((owner is Creature) ? owner : null);
					if (val != null)
					{
						num = MultiplayerFx.For(val).BladesArriveAt;
						goto IL_01b3;
					}
				}
				num = _streamAt;
				goto IL_01b3;
				IL_01db:
				Layout(0f);
				return;
				IL_01b3:
				ulong num2 = num;
				if (num2 > milliseconds && num2 - milliseconds < 4000)
				{
					_opening = true;
					_wait = (float)(num2 - milliseconds) / 1000f;
				}
				goto IL_01db;
			}
			catch (Exception ex)
			{
				DevLog.Warn("InfiniteBladesFx.Start: " + ex);
				Dispose();
			}
		}

		private void Resize(int count)
		{
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			while (_blades.Count > count)
			{
				Blade blade = _blades[_blades.Count - 1];
				if (FxKit.Alive((GodotObject)(object)blade.Node))
				{
					((Node)blade.Node).QueueFree();
				}
				if (FxKit.Alive((GodotObject)(object)blade.Trail))
				{
					((Node)blade.Trail).QueueFree();
				}
				_blades.RemoveAt(_blades.Count - 1);
			}
			while (_blades.Count < count)
			{
				Blade blade2 = new Blade
				{
					Node = MakeBlade(Tuning.F("blades.length", 28f)),
					Trail = FxKit.Ribbon(5f, new Color(Steel.R, Steel.G, Steel.B, 0.75f)),
					Offset = (float)_blades.Count / (float)count
				};
				((CanvasItem)blade2.Node).Visible = false;
				((CanvasItem)blade2.Trail).Visible = false;
				((Node)_root).AddChild((Node)(object)blade2.Trail, false, (InternalMode)0);
				((Node)_root).AddChild((Node)(object)blade2.Node, false, (InternalMode)0);
				_blades.Add(blade2);
			}
		}

		public override void OnAmountChanged(int amount)
		{
			try
			{
				if (!_dead && FxKit.Alive((GodotObject)(object)_root))
				{
					int num = CountFor(amount);
					if (num != _blades.Count)
					{
						Resize(num);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("InfiniteBladesFx.OnAmountChanged: " + ex);
				Dispose();
			}
		}

		public override void Update(double dt)
		{
			try
			{
				if (_dead)
				{
					return;
				}
				if (!Anchor.Valid() || !FxKit.Alive((GodotObject)(object)_root) || !((Node)_root).IsInsideTree())
				{
					Dispose();
					return;
				}
				float num = (float)dt;
				_time += num;
				if (_opening)
				{
					_wait -= num;
					if (_wait > 0f && !_stopping)
					{
						Layout(num);
						return;
					}
					_opening = false;
					_wait = 0f;
					if (!_stopping)
					{
						_flare = 1f;
						_alpha = Mathf.Max(_alpha, 0.7f);
					}
				}
				_boost = FxKit.Damp(_boost, 0f, 1.6f, dt);
				_flare = FxKit.Damp(_flare, 0f, 3.2f, dt);
				_lead += num * Tuning.F("blades.loops", 0.34f) * (1f + _boost);
				_lead -= Mathf.Floor(_lead);
				_alpha = FxKit.Damp(_alpha, _alphaTarget, 5f, dt);
				if (_wait > 0f)
				{
					_wait -= num;
				}
				else if (_drawn < 1f)
				{
					_drawn = Mathf.Min(1f, _drawn + num / Mathf.Max(0.1f, Tuning.F("blades.redraw", 0.55f)));
				}
				Layout(num);
				if (_stopping && _alpha < 0.03f)
				{
					Dispose();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("InfiniteBladesFx.Update: " + ex);
				Dispose();
			}
		}

		private void Layout(float dt)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_045f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)_root))
			{
				return;
			}
			Measure(out var centre, out var across, out var tall);
			float num = Mathf.Clamp(_alpha, 0f, 1f);
			float num2 = Tuning.F("blades.alpha", 0.8f);
			_root.GlobalPosition = centre;
			_root.Scale = new Vector2(Anchor.Side(), 1f);
			((CanvasItem)_root).Modulate = new Color(1f, 1f, 1f, num * Mathf.Lerp(num2, 1f, Mathf.Clamp(_flare, 0f, 1f)));
			if (FxKit.Alive((GodotObject)(object)_loop))
			{
				int num3 = Mathf.Clamp(Mathf.RoundToInt(_drawn * 72f) + 1, 2, 73);
				if (num3 != _shownPoints || dt <= 0f)
				{
					_shownPoints = num3;
					Vector2[] array = new Vector2[num3];
					for (int i = 0; i < num3; i++)
					{
						array[i] = OnSign((float)i / 72f, across, tall);
					}
					_loop.Points = array;
				}
				_loop.Width = 2.2f + 2.4f * _flare;
				_loop.DefaultColor = new Color(Steel.R, Steel.G, Steel.B, Tuning.F("blades.line", 0.42f) + 0.5f * _flare);
			}
			if (FxKit.Alive((GodotObject)(object)_glow))
			{
				((Node2D)_glow).Scale = new Vector2(across * 3f, tall * 4.2f) / 64f;
				((CanvasItem)_glow).Modulate = new Color(Steel.R, Steel.G, Steel.B, 0.07f + 0.3f * _flare);
			}
			int count = _blades.Count;
			float num4 = Tuning.F("blades.trail", 0.1f);
			for (int j = 0; j < count; j++)
			{
				Blade blade = _blades[j];
				float num5 = (float)j / (float)count - blade.Offset;
				num5 -= Mathf.Round(num5);
				blade.Offset += num5 * (1f - Mathf.Exp(-3f * dt));
				float num6 = _lead + blade.Offset;
				num6 -= Mathf.Floor(num6);
				if (blade.Unborn && _drawn >= 1f && blade.LastAt >= 0f && blade.LastAt - num6 > 0.5f)
				{
					blade.Unborn = false;
				}
				blade.LastAt = num6;
				Vector2 val = OnSign(num6, across, tall);
				Vector2 val2 = OnSign(num6 + 0.004f, across, tall);
				blade.Pos = centre + new Vector2(val.X * Anchor.Side(), val.Y);
				float num7 = Mathf.Min(num6, 1f - num6);
				blade.Shown = (blade.Unborn ? 0f : Mathf.Lerp(blade.Shown, 1f, 1f - Mathf.Exp(-6f * dt)));
				float num8 = (blade.Unborn ? 0f : Mathf.Max(blade.Shown, Mathf.SmoothStep(0f, 0.08f, num7)));
				if (FxKit.Alive((GodotObject)(object)blade.Node))
				{
					((CanvasItem)blade.Node).Visible = num8 > 0.02f;
					blade.Node.Position = val;
					Node2D node = blade.Node;
					Vector2 val3 = val2 - val;
					node.Rotation = val3.Angle() + (float)Math.PI / 2f;
					blade.Node.Scale = new Vector2(num8, num8);
				}
				if (FxKit.Alive((GodotObject)(object)blade.Trail))
				{
					((CanvasItem)blade.Trail).Visible = num8 > 0.02f;
					for (int k = 0; k < 7; k++)
					{
						_trail[k] = OnSign(num6 - num4 * (1f - (float)k / 6f), across, tall);
					}
					blade.Trail.Points = _trail;
					((CanvasItem)blade.Trail).Modulate = new Color(1f, 1f, 1f, num8 * 0.8f);
				}
			}
		}

		public override void Trigger(Vector2? at = null)
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Expected Obj, but got Unknown
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			if (_dead || _stopping || _blades.Count == 0)
			{
				return;
			}
			Node frontParent = Anchor.FrontParent;
			Measure(out var centre, out var across, out var tall);
			for (int i = 0; i < _blades.Count; i++)
			{
				Blade blade = _blades[i];
				LaunchToHand((blade.Shown > 0.3f) ? blade.Pos : centre, i, _blades.Count);
				blade.Unborn = true;
				blade.Shown = 0f;
				blade.LastAt = -1f;
			}
			_flare = 1f;
			_boost = 2.6f;
			_drawn = 0f;
			_wait = 0.18f;
			if (!FxKit.Alive((GodotObject)(object)frontParent))
			{
				return;
			}
			Line2D echo = new Line2D
			{
				Width = 5f,
				DefaultColor = new Color(0.88f, 0.97f, 1f, 1f),
				Closed = true,
				JointMode = (LineJointMode)2,
				Material = (Material)(object)FxKit.Additive,
				Antialiased = true
			};
			Vector2[] array = new Vector2[72];
			for (int j = 0; j < 72; j++)
			{
				array[j] = OnSign((float)j / 72f, across, tall);
			}
			echo.Points = array;
			frontParent.AddChild((Node)(object)echo, false, (InternalMode)0);
			((Node2D)echo).GlobalPosition = centre;
			FxKit.Drive((Node)(object)echo, 0.48f, (float t) =>
			{
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)echo))
				{
					float num = Mathf.Lerp(1f, Tuning.F("blades.echo", 3.2f), t);
					((Node2D)echo).Scale = new Vector2(num, num);
					echo.Width = Mathf.Lerp(5f, 1.5f, t) / num * 1.6f;
					((CanvasItem)echo).Modulate = new Color(1f, 1f, 1f, 0.95f * (1f - t) * (1f - t));
				}
			}, (TransitionType)4, (EaseType)1).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)echo))
				{
					((Node)echo).QueueFree();
				}
			})));
			FxKit.Flash(frontParent, centre, new Color(0.8f, 0.94f, 1f, 0.7f), across * 2.4f, 2f, 0.26);
			FxKit.Burst(frontParent, centre, new Color(0.85f, 0.95f, 1f, 0.95f), 16, 180f, 480f, 0.42, 0.06f, 0.16f, new Vector2(0f, 220f));
		}

		private void LaunchToHand(Vector2 from, int index, int count)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Expected Obj, but got Unknown
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			Node parent = Anchor.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Vector2 to;
			if (RemoteOwner)
			{
				Vector2 val = Anchor.SafeBodySize();
				to = Anchor.Chest() + new Vector2(val.X * 0.15f * Anchor.Side(), val.Y * 0.22f);
			}
			else
			{
				Vector2 val2 = FxKit.ViewSize();
				to = new Vector2(val2.X * 0.5f + ((float)index - (float)(count - 1) * 0.5f) * 40f, val2.Y * 0.93f);
			}
			Vector2 mid = new Vector2(Mathf.Lerp(from.X, to.X, 0.3f), from.Y - Tuning.F("blades.arc", 260f) - 26f * (float)(index % 3));
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_BladeToHand")
			};
			parent.AddChild((Node)(object)holder, false, (InternalMode)0);
			Line2D ribbon = FxKit.Ribbon(13f, new Color(0.72f, 0.92f, 1f, 0.85f));
			((Node)holder).AddChild((Node)(object)ribbon, false, (InternalMode)0);
			Node2D flyer = MakeBlade(Tuning.F("blades.fly_length", 52f));
			((Node)holder).AddChild((Node)(object)flyer, false, (InternalMode)0);
			flyer.GlobalPosition = from;
			((CanvasItem)flyer).Visible = false;
			Vector2 last = from;
			float num = Tuning.F("blades.fly_time", 0.55f);
			float num2 = (float)index * 0.07f;
			FxKit.FreeAfter((Node)(object)holder, Mathf.Max(0f, num2) + Mathf.Max(0f, num) + 0.32f);
			Tween val3 = ((Node)holder).CreateTween();
			val3.TweenMethod(Callable.From<float>((Action<float>)Advance), Variant.op_Implicit(0f), Variant.op_Implicit(1f), (double)num).SetDelay((double)num2).SetTrans((TransitionType)1)
				.SetEase((EaseType)2);
			val3.TweenCallback(Callable.From((Action)(() =>
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					FxKit.Flash(parent, to, new Color(0.8f, 0.94f, 1f, 0.55f), 70f, 1.8f, 0.18);
				}
			})));
			val3.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.22);
			val3.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
			void Advance(float t)
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0079: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)holder) && FxKit.Alive((GodotObject)(object)flyer))
				{
					Vector2 val4 = FxKit.Bezier(from, mid, to, t);
					Vector2 val5 = val4 - last;
					if (val5.LengthSquared() > 0.01f)
					{
						flyer.Rotation = val5.Angle() + (float)Math.PI / 2f;
					}
					((CanvasItem)flyer).Visible = true;
					flyer.GlobalPosition = val4;
					float num3 = Mathf.Lerp(0.55f, 1f, Mathf.Min(1f, t * 3f));
					flyer.Scale = new Vector2(num3, num3);
					if (FxKit.Alive((GodotObject)(object)ribbon))
					{
						ribbon.AddPoint(holder.ToLocal(val4), -1);
					}
					last = val4;
				}
			}
		}

		public override void Stop(bool animateOut)
		{
			if (!_dead)
			{
				if (animateOut && FxKit.Alive((GodotObject)(object)_root) && ((Node)_root).IsInsideTree())
				{
					_stopping = true;
					_alphaTarget = 0f;
				}
				else
				{
					Dispose();
				}
			}
		}

		private void Dispose()
		{
			if (!_dead)
			{
				_dead = true;
				if (FxKit.Alive((GodotObject)(object)_root))
				{
					((Node)_root).QueueFree();
				}
			}
		}

		static InfiniteBladesFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
