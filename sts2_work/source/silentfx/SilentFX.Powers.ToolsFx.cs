// namespace: SilentFX.Powers
// type: ToolsFx (class)
// source lines: 15292-18597
	public sealed class ToolsFx : BasicPowerFx
	{
		private enum Mode
		{
			Thrown,
			Perch,
			Fetch,
			Return
		}

		private enum Step
		{
			Lift,
			Plunge,
			Stuck,
			Lever,
			Flick,
			Follow
		}

		private enum Dart
		{
			Thrown,
			Perch,
			Draw,
			Gather,
			Shoot,
			Stuck,
			Haul,
			Loop,
			Return
		}

		private enum Got
		{
			Waiting,
			OnKnife,
			Thrown
		}

		private sealed class Fetch
		{
			public NCard Real;

			public CardModel Card;

			public NCard Copy;

			public Got State;

			public float T;

			public Vector2 From;

			public float FromAngle;

			public float FromSize;

			public float Side = 1f;

			public Vector2 Pos;

			public Vector2 Scale;

			public float Angle;

			public bool Landed;

			public float Since;

			public float Time;

			public float Hop;

			public Line2D Trail;
		}

		private struct Pick
		{
			public CardModel Card;

			public NCard Node;

			public Vector2 Centre;

			public float Rot;

			public Vector2 Scale;
		}

		private sealed class Skewered
		{
			public CardModel Card;

			public NCard Real;

			public NCard Copy;

			public bool Flown;

			public CardFlight.Plan Plan;

			public Vector2 Centre;

			public float Rot;

			public Vector2 Scale;

			public Vector2 StackAt;

			public float StackRot;

			public Vector2 Pos;

			public Vector2 NowScale;

			public float NowRot;

			public float Alpha = 1f;
		}

		private sealed class Orphan
		{
			public Skewered Of;

			public float For;
		}

		private const int MaxHand = 10;

		private const int RopePoints = 11;

		private const int TrailPoints = 14;

		private const float KunaiLength = 66f;

		private const float WristReach = 96f;

		private static readonly Color Rope = new Color(0.84f, 0.76f, 0.58f, 1f);

		private static readonly Color RopeDark = new Color(0.26f, 0.2f, 0.13f, 0.85f);

		private const float ThrowTime = 0.45f;

		private static readonly Color Silver = new Color(0.86f, 0.9f, 0.96f, 1f);

		private static readonly Color Steel = new Color(0.56f, 0.67f, 0.86f, 1f);

		private static readonly Color Edge = new Color(0.2f, 0.23f, 0.31f, 1f);

		private static readonly Color Grip = new Color(0.25f, 0.27f, 0.33f, 1f);

		private static readonly Color GripDark = new Color(0.13f, 0.14f, 0.19f, 1f);

		private static readonly Color Spark = new Color(1f, 0.84f, 0.46f, 0.95f);

		private readonly List<HandCards.Entry> _cards = new List<HandCards.Entry>();

		private Node2D _rig;

		private bool _placed;

		private bool _subscribed;

		private Node2D _fork;

		private Sprite2D _glow;

		private Mode _mode = Mode.Perch;

		private Step _step;

		private float _stepTime;

		private Vector2 _stepFrom;

		private float _stepFromAngle;

		private Vector2 _wrist;

		private Vector2 _pos;

		private Vector2 _from;

		private float _angle;

		private float _fromAngle;

		private float _modeTime;

		private float _swing;

		private float _swingVel;

		private readonly List<Fetch> _fetches = new List<Fetch>();

		private readonly List<Fetch> _pack = new List<Fetch>();

		private float _packWait;

		private float _jab = 9f;

		private int _stabsWanted;

		private int _stabsDone;

		private int _jobsMade;

		private bool _handDrawing;

		private int _drawTotal;

		private int _drawSeen;

		private int _handAtDraw;

		private float _sinceDrawn;

		private float _drawTime;

		private bool _drawNotified;

		private Node2D _kunai;

		private Sprite2D _kunaiGlow;

		private readonly Vector2[] _rope = new Vector2[11];

		private readonly Vector2[] _ropeLocal = new Vector2[11];

		private Line2D _ropeEdge;

		private Line2D _ropeLine;

		private Line2D _ring;

		private Line2D _trail;

		private Dart _dart = Dart.Perch;

		private float _dartTime;

		private Vector2 _dartPos;

		private Vector2 _dartFrom;

		private float _dartAngle = (float)Math.PI;

		private Vector2 _anchor;

		private float _hangSwing;

		private float _hangSwingVel;

		private float _settle;

		private float _pull;

		private bool _wantDraw;

		private Vector2 _aim = new Vector2(-0.83f, -0.55f);

		private readonly List<Pick> _picks = new List<Pick>();

		private readonly List<Pick> _shotPicks = new List<Pick>();

		private float _noPicks;

		private readonly List<Skewered> _skewer = new List<Skewered>();

		private readonly List<Orphan> _orphans = new List<Orphan>();

		private Vector2 _shotFrom;

		private Vector2 _shotDir;

		private float _ringAlong;

		private float _ringStop;

		private float _shotTime;

		private bool _hauling;

		private Vector2 _anchorFrom;

		private Vector2 _entry;

		private float _entryAlong;

		private float _gatherTime;

		private float _sink;

		private float _sinkDepth;

		private float _sunk;

		private float _packScale = 1f;

		private float _haulEnd;

		private float _haulSlope;

		private Vector2 _anchorTo;

		private float _loopFrom;

		private float _loopTurn;

		private float _loopReach;

		private float _loopAngle;

		private Vector2 _dropFrom;

		private float _dropScale = 1f;

		private float _dropTime = 0.08f;

		private Polygon2D _bladeFace;

		private Polygon2D _bladeShade;

		private Line2D _bladeRim;

		private Line2D _bladeRidge;

		private Node2D _slit;

		private Line2D _shadow;

		private const float RidgeTop = -63f;

		private const float RidgeEnd = -29f;

		private static readonly Vector2[] BladeOutline = new Vector2[9]
		{
			new Vector2(0f, -66f),
			new Vector2(5.2f, -52f),
			new Vector2(9.5f, -41f),
			new Vector2(4.2f, -30f),
			new Vector2(2.6f, -27f),
			new Vector2(-2.6f, -27f),
			new Vector2(-4.2f, -30f),
			new Vector2(-9.5f, -41f),
			new Vector2(-5.2f, -52f)
		};

		private static readonly Vector2[] BladeShadow = new Vector2[6]
		{
			new Vector2(0f, -66f),
			new Vector2(5.2f, -52f),
			new Vector2(9.5f, -41f),
			new Vector2(4.2f, -30f),
			new Vector2(2.6f, -27f),
			new Vector2(0f, -27f)
		};

		private const float LoopBend = 2.4f;

		protected override string NodeName => "Tools";

		protected override bool UseFront => false;

		protected override float KickDecay => 1.5f;

		private bool Working
		{
			get
			{
				if (!Dead)
				{
					return !Stopping;
				}
				return false;
			}
		}

		private bool Busy
		{
			get
			{
				if (_dart != Dart.Gather && _dart != Dart.Shoot && _dart != Dart.Stuck && _dart != Dart.Haul)
				{
					if (_dart == Dart.Loop)
					{
						return _skewer.Count > 0;
					}
					return false;
				}
				return true;
			}
		}

		private static bool OnScreen => FxKit.Alive((GodotObject)(object)GameRefs.UiLayer);

		private static float RopeLength => Tuning.F("tools.rope", 62f);

		private static float KunaiSize => Mathf.Clamp(Tuning.F("tools.kunai_size", 1.5f), 0.5f, 2.5f);

		private static float PullTime => Mathf.Max(0.1f, Tuning.F("tools.pull_time", 0.28f));

		private static float LoopTime => Mathf.Max(0.2f, Tuning.F("tools.loop_time", 0.6f));

		private static float LoopKeep => Mathf.Clamp(Tuning.F("tools.loop_keep", 0.15f), 0f, 0.6f);

		protected override void Build(bool animateIn)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected Obj, but got Unknown
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Expected Obj, but got Unknown
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				_rig = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_ToolsRig"),
					Visible = false
				};
				((Node)Back).AddChild((Node)(object)_rig, false, (InternalMode)0);
				float num = Tuning.F("tools.blade_shape", 0f);
				_fork = ((num > 1.5f) ? MakeFork(out _glow) : ((num > 0.5f) ? MakeBlade(out _glow) : MakeKnife(out _glow)));
				((Node)_rig).AddChild((Node)(object)_fork, false, (InternalMode)0);
				_ropeEdge = Props.Line(5.6f, RopeDark, false);
				_ropeLine = Props.Line(3.2f, Rope, false);
				((CanvasItem)_ropeEdge).ZIndex = 1;
				((CanvasItem)_ropeLine).ZIndex = 1;
				((Node)_rig).AddChild((Node)(object)_ropeEdge, false, (InternalMode)0);
				((Node)_rig).AddChild((Node)(object)_ropeLine, false, (InternalMode)0);
				_ring = Props.Line(2.4f, Rope, additive: false, Props.Circle(5f, 12));
				_ring.Closed = true;
				((CanvasItem)_ring).ZIndex = 1;
				((Node)_rig).AddChild((Node)(object)_ring, false, (InternalMode)0);
				_shadow = Props.Line(7f, new Color(0f, 0f, 0f, 0.3f), false);
				_shadow.BeginCapMode = (LineCapMode)0;
				((CanvasItem)_shadow).ZIndex = 1;
				((CanvasItem)_shadow).Visible = false;
				((Node)_rig).AddChild((Node)(object)_shadow, false, (InternalMode)0);
				_slit = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_ToolsSlit"),
					ZIndex = 1,
					Visible = false
				};
				((Node)_slit).AddChild((Node)(object)Props.Line(3.4f, new Color(0.04f, 0.03f, 0.05f, 0.9f), false, new Vector2(-11f, 0f), new Vector2(11f, 0f)), false, (InternalMode)0);
				((Node)_slit).AddChild((Node)(object)Props.Line(1.2f, new Color(1f, 1f, 1f, 0.35f), false, new Vector2(-10f, -2.2f), new Vector2(10f, -2.2f)), false, (InternalMode)0);
				((Node)_rig).AddChild((Node)(object)_slit, false, (InternalMode)0);
				_kunai = MakeKunai(out _kunaiGlow);
				((Node)_rig).AddChild((Node)(object)_kunai, false, (InternalMode)0);
				if (animateIn && Tuning.F("intro.on", 1f) > 0.5f)
				{
					Vector2 val = Anchor.SafeBodySize();
					_from = Anchor.Chest() + new Vector2(val.X * 0.15f * Anchor.Side(), val.Y * 0.2f);
					_mode = Mode.Thrown;
					_dartFrom = _from;
					_dart = Dart.Thrown;
				}
				Subscribe();
			}
			catch (Exception ex)
			{
				DevLog.Warn("ToolsFx.Build: " + ex);
				OnDispose();
			}
		}

		private static Node2D MakeKnife(out Sprite2D glow)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected Obj, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_029d: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0424: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_ToolsKnife"),
				ZIndex = 1
			};
			glow = FxKit.Glow(new Color(0.8f, 0.9f, 1f, 0.22f), new Vector2(50f, 116f));
			((Node2D)glow).Position = new Vector2(0f, -46f);
			((Node)val).AddChild((Node)(object)glow, false, (InternalMode)0);
			Color color = new Color(0.97f, 0.97f, 1f, 1f);
			Color color2 = new Color(0.66f, 0.7f, 0.8f, 1f);
			Color color3 = new Color(0.09f, 0.09f, 0.13f, 1f);
			Color color4 = new Color(0.55f, 0.57f, 0.63f, 1f);
			Color color5 = new Color(0.22f, 0.23f, 0.28f, 1f);
			Vector2[] array = new Vector2[19];
			Vector2[] array2 = new Vector2[19];
			Vector2[] array3 = new Vector2[19];
			Vector2[] array4 = new Vector2[19];
			for (int i = 0; i <= 18; i++)
			{
				float num = (float)i / 18f;
				float num2 = 6.2f * Mathf.Sin(num * ((float)Math.PI * 2f) * 0.98f) * Mathf.Min(1f, num * 3.2f);
				float num3 = 9.6f * Mathf.Pow(num, 0.46f);
				float num4 = -60f * num;
				array[i] = new Vector2(num2 - num3, num4);
				array2[i] = new Vector2(num2 + num3, num4);
				array3[i] = new Vector2(num2 + num3 * 0.1f, num4);
				array4[i] = new Vector2(num2 + num3 * 0.5f, num4);
			}
			((Node)val).AddChild((Node)(object)FxKit.Shape(Band(array, array2), color), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)FxKit.Shape(Band(array3, array2), color2), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)FxKit.Shape(Band(array4, array2), color3), false, (InternalMode)0);
			Line2D val2 = Props.Line(1.2f, color3, additive: false, Band(array, array2));
			val2.Closed = true;
			((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
			Vector2 val3 = new Vector2((array[18].X + array2[18].X) * 0.5f, -60f);
			Vector2 val4 = val3 + new Vector2(-14f, -1f);
			Vector2 val5 = val3 + new Vector2(14f, -5f);
			float[] array5 = new float[4] { 0.06f, 0.3f, 0.7f, 0.94f };
			foreach (float num5 in array5)
			{
				Vector2 val6 = val4.Lerp(val5, num5);
				((Node)val).AddChild((Node)(object)FxKit.Shape(new Vector2[3]
				{
					val6 + new Vector2(-2.2f, 3f),
					val6 + new Vector2(2.2f, 3f),
					val6 + new Vector2(0.4f, 10.5f)
				}, color5), false, (InternalMode)0);
			}
			((Node)val).AddChild((Node)(object)Props.Line(10.5f, color5, false, val4, val5), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Props.Line(7.6f, color4, false, val4, val5), false, (InternalMode)0);
			Vector2 val7 = val3 + new Vector2(0f, -7f);
			Vector2 val8 = val3 + new Vector2(-2.5f, -33f);
			((Node)val).AddChild((Node)(object)Props.Line(9.6f, color5, false, val7, val8), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Props.Line(6.8f, color4, false, val7, val8), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Props.Line(1.6f, new Color(0.78f, 0.8f, 0.86f, 0.8f), false, val7 + new Vector2(-1.6f, -2f), val8 + new Vector2(-1.6f, 3f)), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Props.Line(9.8f, color5, false, val8 + new Vector2(0.4f, 5f), val8), false, (InternalMode)0);
			return val;
		}

		private static Vector2[] Band(Vector2[] a, Vector2[] b)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			int num = a.Length;
			bool flag = num > 0 && a[0].DistanceSquaredTo(b[0]) < 0.01f;
			Vector2[] array = new Vector2[flag ? (2 * num - 1) : (2 * num)];
			for (int i = 0; i < num; i++)
			{
				array[i] = a[i];
			}
			int num2 = (flag ? (num - 1) : num);
			for (int j = 0; j < num2; j++)
			{
				array[num + j] = b[num - 1 - j];
			}
			return array;
		}

		private static Node2D MakeFork(out Sprite2D glow)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected Obj, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_ToolsFork"),
				ZIndex = 1
			};
			glow = FxKit.Glow(new Color(0.8f, 0.9f, 1f, 0.2f), new Vector2(36f, 92f));
			((Node2D)glow).Position = new Vector2(0f, -38f);
			((Node)val).AddChild((Node)(object)glow, false, (InternalMode)0);
			Vector2[] points = new Vector2[30]
			{
				new Vector2(-6f, 0f),
				new Vector2(-7.8f, -4f),
				new Vector2(-7.8f, -17f),
				new Vector2(-7.2f, -23f),
				new Vector2(-4.6f, -28.5f),
				new Vector2(-2.1f, -33f),
				new Vector2(-1.7f, -40f),
				new Vector2(-2.6f, -70f),
				new Vector2(-1.9f, -73.5f),
				new Vector2(0f, -75f),
				new Vector2(1.9f, -73.5f),
				new Vector2(2.6f, -70f),
				new Vector2(1.7f, -40f),
				new Vector2(2.1f, -33f),
				new Vector2(4.6f, -28.5f),
				new Vector2(7.2f, -23f),
				new Vector2(7.8f, -17f),
				new Vector2(7.8f, -4f),
				new Vector2(6f, 0f),
				new Vector2(4.3f, -4f),
				new Vector2(4.3f, -15f),
				new Vector2(2.95f, -16.6f),
				new Vector2(1.6f, -15f),
				new Vector2(1.6f, -4f),
				new Vector2(0f, 0f),
				new Vector2(-1.6f, -4f),
				new Vector2(-1.6f, -15f),
				new Vector2(-2.95f, -16.6f),
				new Vector2(-4.3f, -15f),
				new Vector2(-4.3f, -4f)
			};
			((Node)val).AddChild((Node)(object)FxKit.Shape(points, Silver), false, (InternalMode)0);
			Line2D val2 = Props.Line(0.9f, new Color(Edge.R, Edge.G, Edge.B, 0.75f), additive: false, points);
			val2.Closed = true;
			((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Props.Line(1.3f, new Color(Steel.R, Steel.G, Steel.B, 0.9f), false, new Vector2(1f, -36f), new Vector2(1.3f, -70f)), false, (InternalMode)0);
			return val;
		}

		private static Node2D MakeBlade(out Sprite2D glow)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected Obj, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_ToolsBlade"),
				ZIndex = 1
			};
			glow = FxKit.Glow(new Color(0.8f, 0.9f, 1f, 0.22f), new Vector2(54f, 124f));
			((Node2D)glow).Position = new Vector2(0f, -50f);
			((Node)val).AddChild((Node)(object)glow, false, (InternalMode)0);
			Vector2[] array = new Vector2[15];
			Vector2[] array2 = new Vector2[15];
			Vector2[] array3 = new Vector2[15];
			for (int i = 0; i <= 14; i++)
			{
				float num = (float)i / 14f;
				float num2 = 4.8f * Mathf.Sin(num * ((float)Math.PI * 2f)) * Mathf.Min(1f, num * 4f);
				float num3 = 8.5f * Mathf.Pow(num, 0.55f);
				float num4 = -62f * num;
				array[i] = new Vector2(num2 - num3, num4);
				array2[i] = new Vector2(num2 + num3, num4);
				array3[i] = new Vector2(num2, num4);
			}
			Vector2[] array4 = new Vector2[29];
			Vector2[] array5 = new Vector2[29];
			for (int j = 0; j <= 14; j++)
			{
				array4[j] = array[j];
				array5[j] = array3[j];
			}
			for (int k = 0; k < 14; k++)
			{
				array4[15 + k] = array2[14 - k];
				array5[15 + k] = array2[14 - k];
			}
			((Node)val).AddChild((Node)(object)FxKit.Shape(array4, Silver), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)FxKit.Shape(array5, Steel), false, (InternalMode)0);
			Line2D val2 = Props.Line(1.3f, Edge, additive: false, array4);
			val2.Closed = true;
			((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Props.Line(1.1f, new Color(1f, 1f, 1f, 0.85f), additive: false, array3), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Props.Line(10f, GripDark, false, new Vector2(0f, -66f), new Vector2(0f, -97f)), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Props.Line(7.2f, Grip, false, new Vector2(0f, -66f), new Vector2(0f, -96f)), false, (InternalMode)0);
			for (int l = 0; l < 3; l++)
			{
				float num5 = -74f - 7f * (float)l;
				((Node)val).AddChild((Node)(object)Props.Line(1.4f, new Color(0.44f, 0.47f, 0.56f, 1f), false, new Vector2(-3.6f, num5 + 1.5f), new Vector2(3.6f, num5 - 1.5f)), false, (InternalMode)0);
			}
			((Node)val).AddChild((Node)(object)Props.Line(10.5f, GripDark, false, new Vector2(-18f, -62f), new Vector2(18f, -67f)), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Props.Line(7.6f, Grip, false, new Vector2(-17f, -62f), new Vector2(17f, -67f)), false, (InternalMode)0);
			Polygon2D val3 = FxKit.Shape(Props.Circle(6.2f, 14), GripDark);
			((Node2D)val3).Position = new Vector2(0f, -99f);
			((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
			Polygon2D val4 = FxKit.Shape(Props.Circle(4.4f, 12), Grip);
			((Node2D)val4).Position = ((Node2D)val3).Position;
			((Node)val).AddChild((Node)(object)val4, false, (InternalMode)0);
			return val;
		}

		private Node2D MakeKunai(out Sprite2D glow)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected Obj, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_ToolsKunai"),
				ZIndex = 2
			};
			glow = FxKit.Glow(new Color(0.8f, 0.9f, 1f, 0.26f), new Vector2(56f, 108f));
			((Node2D)glow).Position = new Vector2(0f, -40f);
			((Node)val).AddChild((Node)(object)glow, false, (InternalMode)0);
			_bladeFace = FxKit.Shape(BladeOutline, Silver);
			((Node)val).AddChild((Node)(object)_bladeFace, false, (InternalMode)0);
			_bladeShade = FxKit.Shape(BladeShadow, Steel);
			((Node)val).AddChild((Node)(object)_bladeShade, false, (InternalMode)0);
			_bladeRim = Props.Line(1.3f, Edge, additive: false, BladeOutline);
			_bladeRim.Closed = true;
			((Node)val).AddChild((Node)(object)_bladeRim, false, (InternalMode)0);
			_bladeRidge = Props.Line(1f, new Color(1f, 1f, 1f, 0.85f), false, new Vector2(0f, -63f), new Vector2(0f, -29f));
			((Node)val).AddChild((Node)(object)_bladeRidge, false, (InternalMode)0);
			_sunk = 0f;
			((Node)val).AddChild((Node)(object)Props.Line(6.4f, GripDark, false, new Vector2(0f, -26f), new Vector2(0f, -9f)), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Props.Line(4.4f, Grip, false, new Vector2(0f, -26f), new Vector2(0f, -9f)), false, (InternalMode)0);
			for (int i = 0; i < 4; i++)
			{
				float num = -24f + 4.4f * (float)i;
				((Node)val).AddChild((Node)(object)Props.Line(1.1f, new Color(0.44f, 0.47f, 0.56f, 1f), false, new Vector2(-2.6f, num + 1.2f), new Vector2(2.6f, num - 1.2f)), false, (InternalMode)0);
			}
			Line2D val2 = Props.Line(3.8f, Edge, additive: false, Props.Circle(6.2f, 16));
			val2.Closed = true;
			((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
			Line2D val3 = Props.Line(2.2f, Silver, additive: false, Props.Circle(6.2f, 16));
			val3.Closed = true;
			((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
			return val;
		}

		private static Vector2[] CutOff(Vector2[] outline, float cut)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			List<Vector2> list = new List<Vector2>(outline.Length + 2);
			for (int i = 0; i < outline.Length; i++)
			{
				Vector2 val = outline[i];
				Vector2 val2 = outline[(i + 1) % outline.Length];
				bool flag = val.Y >= cut;
				bool flag2 = val2.Y >= cut;
				if (flag)
				{
					list.Add(val);
				}
				if (flag != flag2)
				{
					list.Add(val + (val2 - val) * ((cut - val.Y) / (val2.Y - val.Y)));
				}
			}
			return list.ToArray();
		}

		private void SinkKunai(float depth)
		{
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			depth = Mathf.Clamp(depth, 0f, 36f);
			if ((!(Mathf.Abs(depth - _sunk) < 0.25f) || (depth <= 0f && _sunk > 0f)) && FxKit.Alive((GodotObject)(object)_bladeFace) && FxKit.Alive((GodotObject)(object)_bladeShade) && FxKit.Alive((GodotObject)(object)_bladeRim) && FxKit.Alive((GodotObject)(object)_bladeRidge))
			{
				_sunk = depth;
				float num = -66f + depth;
				Vector2[] array = ((depth <= 0f) ? BladeOutline : CutOff(BladeOutline, num));
				Vector2[] polygon = ((depth <= 0f) ? BladeShadow : CutOff(BladeShadow, num));
				_bladeFace.Polygon = array;
				_bladeShade.Polygon = polygon;
				_bladeRim.Points = array;
				float num2 = Mathf.Max(-63f, num + 1.5f);
				((CanvasItem)_bladeRidge).Visible = num2 < -30f;
				if (((CanvasItem)_bladeRidge).Visible)
				{
					_bladeRidge.Points = new Vector2[2]
					{
						new Vector2(0f, num2),
						new Vector2(0f, -29f)
					};
				}
			}
		}

		private void Subscribe()
		{
			if (!_subscribed)
			{
				HandFlow.HandDrawSized += OnSized;
				HandFlow.HandDrawBegins += OnHandDraw;
				HandFlow.CardDrawn += OnDrawn;
				HandFlow.DrawnCardEntered += OnEntered;
				HandFlow.SelectionBegan += OnSelect;
				HandFlow.SelectionEnded += OnSelected;
				_subscribed = true;
			}
		}

		private void Unsubscribe()
		{
			HandFlow.HandDrawSized -= OnSized;
			HandFlow.HandDrawBegins -= OnHandDraw;
			HandFlow.CardDrawn -= OnDrawn;
			HandFlow.DrawnCardEntered -= OnEntered;
			HandFlow.SelectionBegan -= OnSelect;
			HandFlow.SelectionEnded -= OnSelected;
			_subscribed = false;
		}

		private bool Mine(Player player)
		{
			if (Anchor.Owner != null)
			{
				return ((player != null) ? player.Creature : null) == Anchor.Owner;
			}
			return false;
		}

		private bool Mine(AbstractModel source)
		{
			if (Anchor.Owner != null)
			{
				ToolsOfTheTradePower val = (ToolsOfTheTradePower)(object)((source is ToolsOfTheTradePower) ? source : null);
				if (val != null)
				{
					return ((PowerModel)val).Owner == Anchor.Owner;
				}
			}
			return false;
		}

		private void OnSized(Player player, int count)
		{
			try
			{
				if (Working && Mine(player))
				{
					_drawTotal = count;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ToolsFx.OnSized: " + ex);
			}
		}

		private void OnHandDraw(Player player)
		{
			try
			{
				if (!Working || !Mine(player))
				{
					return;
				}
				ClearFetch();
				_drawSeen = 0;
				_sinceDrawn = 0f;
				_drawTime = 0f;
				_drawNotified = false;
				_handDrawing = true;
				_stabsDone = 0;
				_jobsMade = 0;
				_stabsWanted = 0;
				HandCards.Fill(_cards);
				_handAtDraw = _cards.Count;
				if (!(Tuning.F("tools.fetch", 1f) < 0.5f))
				{
					int num = 10 - _handAtDraw;
					int num2 = ((_drawTotal > 0) ? Mathf.Min(_drawTotal, num) : num);
					_stabsWanted = Mathf.Clamp(Mathf.Max(1, Amount), 0, Mathf.Max(0, num2));
					if (_stabsWanted > 0 && Tuning.F("tools.early_stab", 1f) > 0.5f)
					{
						BeginStab();
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ToolsFx.OnHandDraw: " + ex);
			}
		}

		private void OnDrawn(CardModel card, bool fromHandDraw)
		{
			try
			{
				if (Working && fromHandDraw && card != null)
				{
					Player owner = card.Owner;
					if (((owner != null) ? owner.Creature : null) == Anchor.Owner)
					{
						_sinceDrawn = 0f;
						_drawNotified = true;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ToolsFx.OnDrawn: " + ex);
			}
		}

		private void OnEntered(NCard node, HandFlow.DrawInfo draw)
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!Working || !_handDrawing || !FxKit.Alive((GodotObject)(object)node) || draw == null || !draw.FromHandDraw)
				{
					return;
				}
				CardModel model = node.Model;
				if (model == null)
				{
					return;
				}
				Player owner = model.Owner;
				if (((owner != null) ? owner.Creature : null) != Anchor.Owner)
				{
					return;
				}
				_drawSeen++;
				if (_jobsMade >= _stabsWanted)
				{
					return;
				}
				if (!_placed)
				{
					_pos = PerchPoint();
					HomeDart();
					_placed = true;
				}
				Place();
				NCard val = CardStandIn.Make(model, (Node)(object)_rig);
				if (FxKit.Alive((GodotObject)(object)val))
				{
					((CanvasItem)val).Visible = false;
					CardStandIn.Hide(node);
					_fetches.Add(new Fetch
					{
						Real = node,
						Card = model,
						Copy = val,
						State = Got.Waiting
					});
					_jobsMade++;
					if (_mode != Mode.Fetch)
					{
						BeginStab();
					}
					else if (_step == Step.Stuck)
					{
						_jab = 0f;
						Sparks(GameRefs.PilePoint((PileType)1) + new Vector2(0f, 0f - Tuning.F("tools.stab_y", 10f)), 2);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ToolsFx.OnEntered: " + ex);
				ClearFetch();
				Go(Mode.Return);
			}
		}

		private void OnSelect(AbstractModel source)
		{
			try
			{
				if (Working && Mine(source))
				{
					_handDrawing = false;
					if (Busy)
					{
						_wantDraw = true;
						return;
					}
					_picks.Clear();
					_noPicks = 0f;
					GoDart(Dart.Draw);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ToolsFx.OnSelect: " + ex);
			}
		}

		private void OnSelected(AbstractModel source, IReadOnlyList<CardModel> chosen)
		{
			try
			{
				if (Working && Mine(source))
				{
					_handDrawing = false;
					_wantDraw = false;
					DropFetchOf(chosen);
					if (!Busy && (chosen == null || chosen.Count == 0 || !Loose(chosen)))
					{
						GoDart(Dart.Return);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ToolsFx.OnSelected: " + ex);
				FreeSkewer(showNow: true);
				GoDart(Dart.Return);
			}
		}

		private static bool Seen(NCard node)
		{
			try
			{
				return FxKit.Alive((GodotObject)(object)node) && ((Node)node).IsInsideTree() && ((CanvasItem)node).IsVisibleInTree();
			}
			catch
			{
				return false;
			}
		}

		private static bool Shows(NCard node, CardModel card)
		{
			try
			{
				return FxKit.Alive((GodotObject)(object)node) && ((Node)node).IsInsideTree() && node.Model == card;
			}
			catch
			{
				return false;
			}
		}

		private static bool IsSly(CardModel card)
		{
			try
			{
				return card != null && card.IsSlyThisTurn;
			}
			catch
			{
				return false;
			}
		}

		private static Vector2 DrawnScale(NCard node)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = (FxKit.Alive((GodotObject)(object)node.Body) ? node.Body.Scale : Vector2.One);
			Transform2D globalTransform = ((CanvasItem)node).GetGlobalTransform();
			Vector2 val2 = globalTransform.Scale * val;
			return val2.Abs();
		}

		private static float EaseOut(float u)
		{
			return 1f - (1f - u) * (1f - u);
		}

		private static float EaseBoth(float u)
		{
			return u * u * (3f - 2f * u);
		}

		private static Vector2 Toward(Vector2 from, Vector2 to, float rate, float dt)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			return new Vector2(FxKit.Damp(from.X, to.X, rate, dt), FxKit.Damp(from.Y, to.Y, rate, dt));
		}

		private static float AngleOf(Vector2 dir)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return Mathf.Atan2(dir.X, 0f - dir.Y);
		}

		private void Place()
		{
			if (!FxKit.Alive((GodotObject)(object)_rig))
			{
				return;
			}
			Node val = (Node)(OnScreen ? ((object)GameRefs.UiLayer) : ((object)Back));
			if (!FxKit.Alive((GodotObject)(object)val))
			{
				return;
			}
			Node parent = ((Node)_rig).GetParent();
			if (parent != val)
			{
				if (parent != null)
				{
					parent.RemoveChild((Node)(object)_rig);
				}
				val.AddChild((Node)(object)_rig, false, (InternalMode)0);
			}
			bool flag = _mode != Mode.Perch || _dart != Dart.Perch || _fetches.Count > 0 || _skewer.Count > 0;
			((CanvasItem)_rig).ZIndex = ((flag && (object)val != Back) ? 10 : 0);
		}

		private void Flash(Vector2 at, float size)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_rig))
			{
				FxKit.Flash((Node)(object)_rig, at, new Color(Silver.R, Silver.G, Silver.B, 0.7f), size, 1.8f, 0.2);
			}
		}

		private void Sparks(Vector2 at, int count)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_rig))
			{
				FxKit.Burst((Node)(object)_rig, at, Spark, count, 120f, 320f, 0.3, 0.06f, 0.14f, new Vector2(0f, 380f));
			}
		}

		private Vector2 PerchPoint()
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			if (OnScreen)
			{
				return GameRefs.PilePoint((PileType)1) + new Vector2(Tuning.F("tools.pile_x", 4f), 0f - Tuning.F("tools.pile_y", 150f));
			}
			Vector2 val = Anchor.SafeBodySize();
			return Anchor.Chest() + new Vector2((0f - val.X) * Tuning.F("tools.x", 0.4f) * Anchor.Side(), (0f - val.Y) * Tuning.F("tools.y", 0.8f));
		}

		private void Go(Mode mode)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (_mode == Mode.Fetch && mode != Mode.Fetch)
			{
				DropUnthrown();
			}
			_from = _pos;
			_fromAngle = _angle;
			_mode = mode;
			_modeTime = 0f;
		}

		private void SetStep(Step step)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			_step = step;
			_stepTime = 0f;
			_stepFrom = _pos;
			_stepFromAngle = _angle;
		}

		private void BeginStab()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			if (!_placed)
			{
				_pos = PerchPoint();
				HomeDart();
				_placed = true;
			}
			Go(Mode.Fetch);
			SetStep(Step.Lift);
			Place();
		}

		private void Catch(float kick)
		{
			_swing = 0f;
			_swingVel = kick;
			_angle = 0f;
			Kick = Mathf.Max(Kick, 0.7f);
			Go(Mode.Perch);
		}

		private int Waiting()
		{
			int num = 0;
			for (int i = 0; i < _fetches.Count; i++)
			{
				Fetch fetch = _fetches[i];
				if (fetch.State == Got.Waiting)
				{
					if (Shows(fetch.Real, fetch.Card) && Shows(fetch.Copy, fetch.Card))
					{
						num++;
						continue;
					}
					EndJob(fetch);
					i--;
				}
			}
			return num;
		}

		private bool Holding()
		{
			for (int num = _pack.Count - 1; num >= 0; num--)
			{
				Fetch fetch = _pack[num];
				if (!Shows(fetch.Real, fetch.Card) || !Shows(fetch.Copy, fetch.Card))
				{
					EndJob(fetch);
				}
			}
			return _pack.Count > 0;
		}

		private void CarryPack(float size)
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			int count = _pack.Count;
			float num = ((count <= 1) ? 0f : Mathf.Min(Tuning.F("tools.pack_spread", 0.22f), 0.75f / (float)(count - 1)));
			for (int i = 0; i < count; i++)
			{
				Fetch fetch = _pack[i];
				float num2 = ((float)i - (float)(count - 1) * 0.5f) * num;
				fetch.Scale = DrawnScale(fetch.Real) * size;
				fetch.Angle = _angle * 0.6f + num2;
				Vector2 pos = _pos;
				Vector2 val = new Vector2(0f, 116.48f * fetch.Scale.Y);
				fetch.Pos = pos + val.Rotated(_angle + num2);
			}
		}

		private void Toss()
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			if (_pack.Count == 0)
			{
				return;
			}
			float num = Mathf.Max(0.12f, Tuning.F("tools.toss_time", 0.46f));
			float num2 = Tuning.F("tools.toss_hop", 230f);
			float num3 = Mathf.Max(0f, Tuning.F("tools.toss_gap", 0.07f));
			for (int i = 0; i < _pack.Count; i++)
			{
				Fetch fetch = _pack[i];
				fetch.State = Got.Thrown;
				fetch.T = 0f;
				fetch.From = fetch.Pos;
				fetch.FromAngle = fetch.Angle;
				fetch.FromSize = Tuning.F("tools.lever_size", 0.56f);
				fetch.Time = num + num3 * (float)i;
				fetch.Hop = num2 + 26f * (float)i;
				Vector2 origin = ((CanvasItem)fetch.Real).GetGlobalTransform().Origin;
				fetch.Side = ((origin.X >= fetch.Pos.X) ? 1f : (-1f));
				if (FxKit.Alive((GodotObject)(object)_rig))
				{
					fetch.Trail = FxKit.Ribbon(Tuning.F("tools.toss_trail", 9f), new Color(Silver.R, Silver.G, Silver.B, 0.42f));
					((Node)fetch.Trail).Name = StringName.op_Implicit("SilentFX_ToolsToss");
					((Node)_rig).AddChild((Node)(object)fetch.Trail, false, (InternalMode)0);
					((Node)_rig).MoveChild((Node)(object)fetch.Trail, 0);
				}
			}
			Sparks(_pos, 2 + _pack.Count);
			_pack.Clear();
		}

		private void EndJob(Fetch job)
		{
			if (job != null)
			{
				CardStandIn.Free(job.Copy);
				job.Copy = null;
				CardStandIn.Show(job.Real);
				if (FxKit.Alive((GodotObject)(object)job.Trail))
				{
					((Node)job.Trail).QueueFree();
				}
				job.Trail = null;
				_fetches.Remove(job);
				_pack.Remove(job);
			}
		}

		private void DropUnthrown()
		{
			for (int num = _fetches.Count - 1; num >= 0; num--)
			{
				if (_fetches[num].State != Got.Thrown)
				{
					EndJob(_fetches[num]);
				}
			}
			_pack.Clear();
		}

		private void DropFetchOf(IReadOnlyList<CardModel> cards)
		{
			if (cards == null)
			{
				return;
			}
			for (int num = _fetches.Count - 1; num >= 0; num--)
			{
				Fetch fetch = _fetches[num];
				foreach (CardModel card in cards)
				{
					if (card == fetch.Card)
					{
						EndJob(fetch);
						break;
					}
				}
			}
		}

		private void ClearFetch()
		{
			for (int num = _fetches.Count - 1; num >= 0; num--)
			{
				EndJob(_fetches[num]);
			}
			_fetches.Clear();
			_pack.Clear();
		}

		private void StepTosses(float dt)
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			for (int num = _fetches.Count - 1; num >= 0; num--)
			{
				Fetch fetch = _fetches[num];
				if (fetch.State == Got.Thrown)
				{
					if (!Shows(fetch.Real, fetch.Card) || !Shows(fetch.Copy, fetch.Card))
					{
						EndJob(fetch);
					}
					else
					{
						fetch.T += dt;
						Transform2D globalTransform = ((CanvasItem)fetch.Real).GetGlobalTransform();
						Vector2 origin = globalTransform.Origin;
						Vector2 val = DrawnScale(fetch.Real);
						float num2 = Mathf.Clamp(fetch.T / Mathf.Max(0.12f, fetch.Time), 0f, 1f);
						if (num2 < 1f)
						{
							Vector2 b = (fetch.From + origin) * 0.5f + new Vector2(0f, 0f - fetch.Hop);
							fetch.Pos = FxKit.Bezier(fetch.From, b, origin, num2);
							fetch.Angle = Mathf.Lerp(fetch.FromAngle, globalTransform.Rotation + fetch.Side * ((float)Math.PI * 2f), EaseOut(num2));
							fetch.Scale = val * Mathf.Lerp(fetch.FromSize, 1f, Mathf.Pow(num2, 0.8f));
							if (FxKit.Alive((GodotObject)(object)fetch.Trail))
							{
								fetch.Trail.AddPoint(((Node2D)fetch.Trail).ToLocal(fetch.Pos), -1);
								while (fetch.Trail.GetPointCount() > 10)
								{
									fetch.Trail.RemovePoint(0);
								}
							}
						}
						else
						{
							if (!fetch.Landed)
							{
								fetch.Landed = true;
								Sparks(origin + new Vector2(0f, 208f * val.Y), 5);
							}
							fetch.Since += dt;
							float num3 = EaseOut(Mathf.Clamp(fetch.Since / 0.1f, 0f, 1f));
							fetch.Pos = origin;
							fetch.Angle = globalTransform.Rotation;
							fetch.Scale = new Vector2(val.X * Mathf.Lerp(1.08f, 1f, num3), val.Y * Mathf.Lerp(0.86f, 1f, num3));
							if (FxKit.Alive((GodotObject)(object)fetch.Trail))
							{
								for (int i = 0; i < 2; i++)
								{
									if (fetch.Trail.GetPointCount() <= 0)
									{
										break;
									}
									fetch.Trail.RemovePoint(0);
								}
							}
							if (fetch.Since >= 0.1f)
							{
								EndJob(fetch);
							}
						}
					}
				}
			}
		}

		private void TickKnife(float dt)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			switch (_mode)
			{
			case Mode.Thrown:
			{
				float num2 = Mathf.Clamp(_modeTime / 0.45f, 0f, 1f);
				Vector2 val2 = PerchPoint();
				_pos = FxKit.Bezier(b: new Vector2((_from.X + val2.X) * 0.5f, Mathf.Min(_from.Y, val2.Y) - 240f), a: _from, c: val2, t: num2);
				_angle = (float)Math.PI * 4f * num2;
				if (num2 >= 1f)
				{
					Catch(-3f);
				}
				break;
			}
			case Mode.Perch:
				_swingVel += ((0f - Tuning.F("tools.swing", 30f)) * _swing - 6f * _swingVel) * dt;
				_swing = Mathf.Clamp(_swing + _swingVel * dt, -0.45f, 0.45f);
				_pos = PerchPoint() + new Vector2(0f, 3f * Mathf.Sin(Time * 2f));
				_angle = _swing + 0.025f * Mathf.Sin(Time * 1.4f);
				break;
			case Mode.Fetch:
				TickFetch(dt);
				break;
			case Mode.Return:
			{
				float num = Mathf.Clamp(_modeTime / 0.35f, 0f, 1f);
				Vector2 val = PerchPoint();
				Vector2 b = (_from + val) * 0.5f + new Vector2(0f, -90f);
				_pos = FxKit.Bezier(_from, b, val, EaseOut(num));
				_angle = Mathf.LerpAngle(_fromAngle, 0f, EaseOut(num));
				if (num >= 1f)
				{
					Catch(2f);
				}
				break;
			}
			default:
				Go(Mode.Return);
				break;
			}
		}

		private void TickFetch(float dt)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_051c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0521: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0490: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			_stepTime += dt;
			Vector2 val = GameRefs.PilePoint((PileType)1) + new Vector2(0f, 0f - Tuning.F("tools.stab_y", 10f));
			bool flag = _stabsDone == 0;
			float num = Tuning.F("tools.lever_size", 0.56f);
			Vector2 val2;
			switch (_step)
			{
			case Step.Lift:
			{
				float num8 = (flag ? 0.14f : 0.1f);
				float num9 = Mathf.Clamp(_stepTime / num8, 0f, 1f);
				Vector2 val3 = (flag ? (PerchPoint() + new Vector2(0f, -30f)) : (val + new Vector2(4f, -108f)));
				_pos = _stepFrom.Lerp(val3, EaseOut(num9));
				_angle = Mathf.LerpAngle(_stepFromAngle, flag ? (-0.2f) : (-0.14f), EaseOut(num9));
				if (num9 >= 1f)
				{
					SetStep(Step.Plunge);
				}
				break;
			}
			case Step.Plunge:
			{
				float num7 = Mathf.Clamp(_stepTime / 0.085f, 0f, 1f);
				_pos = _stepFrom.Lerp(val, num7 * num7);
				_angle = Mathf.Lerp(_stepFromAngle, 0f, num7);
				if (num7 >= 1f)
				{
					Flash(val, 70f);
					Sparks(val, 6);
					Kick = Mathf.Max(Kick, 0.6f);
					SetStep(Step.Stuck);
					_packWait = 0f;
					_jab = 9f;
				}
				break;
			}
			case Step.Stuck:
			{
				_jab += dt;
				float num4 = ((_jab < 0.14f) ? (1f - _jab / 0.14f) : 0f);
				_pos = val + new Vector2(0f, 4f + 7f * num4 * num4);
				_angle = 0.07f * Mathf.Sin(_stepTime * 46f) * Mathf.Exp((0f - _stepTime) * 9f) + 0.05f * num4 * Mathf.Sin(_jab * 60f);
				int num5 = Waiting();
				bool flag2 = _jobsMade >= _stabsWanted;
				if (num5 > 0)
				{
					_packWait += dt;
					if (_stepTime < 0.07f || _jab < 0.06f)
					{
						break;
					}
					float num6 = Mathf.Min(1.4f, 0.15f + Mathf.Max(0f, Tuning.F("tools.pack_wait", 0.32f)) * (float)(_stabsWanted - 1));
					if (!flag2 && _handDrawing && _packWait < num6)
					{
						break;
					}
					foreach (Fetch fetch in _fetches)
					{
						if (fetch.State == Got.Waiting)
						{
							fetch.State = Got.OnKnife;
							if (FxKit.Alive((GodotObject)(object)fetch.Copy))
							{
								((CanvasItem)fetch.Copy).Visible = true;
							}
							_pack.Add(fetch);
						}
					}
					SetStep(Step.Lever);
					CarryPack(0.22f);
				}
				else if (flag2 || (!_handDrawing && _stepTime > 0.2f) || _stepTime > 1.6f)
				{
					Go(Mode.Return);
				}
				break;
			}
			case Step.Lever:
			{
				if (!Holding())
				{
					Go(Mode.Return);
					break;
				}
				float num10 = 0.22f + 0.02f * (float)Mathf.Min(4, _pack.Count - 1);
				float num11 = Mathf.Clamp(_stepTime / num10, 0f, 1f);
				float num12 = EaseOut(num11);
				_pos = val.Lerp(val + new Vector2(26f, -92f), num12);
				_angle = Mathf.Lerp(0f, -0.52f, EaseBoth(num11));
				CarryPack(Mathf.Lerp(0.22f, num, num12));
				if (num11 >= 1f)
				{
					SetStep(Step.Flick);
					Vector2 pos = _pos;
					val2 = new Vector2(0f, 96f);
					_wrist = pos - val2.Rotated(_angle);
				}
				break;
			}
			case Step.Flick:
			{
				float num3 = Mathf.Clamp(_stepTime / 0.11f, 0f, 1f);
				_angle = Mathf.Lerp(_stepFromAngle, -1.3f, EaseOut(num3));
				Vector2 wrist = _wrist;
				val2 = new Vector2(0f, 96f);
				_pos = wrist + val2.Rotated(_angle);
				if (_pack.Count > 0 && Holding())
				{
					CarryPack(num);
					if (num3 >= 0.45f)
					{
						Toss();
					}
				}
				if (num3 >= 1f)
				{
					SetStep(Step.Follow);
				}
				break;
			}
			case Step.Follow:
			{
				float num2 = Mathf.Clamp(_stepTime / 0.16f, 0f, 1f);
				_pos = _stepFrom + new Vector2(-16f, 26f) * EaseOut(num2);
				_angle = Mathf.Lerp(_stepFromAngle, -0.4f, EaseOut(num2));
				if (!(num2 < 1f))
				{
					_stabsDone++;
					if (Waiting() > 0 || (_handDrawing && _jobsMade < _stabsWanted))
					{
						SetStep(Step.Lift);
					}
					else
					{
						Go(Mode.Return);
					}
				}
				break;
			}
			}
			if (_mode == Mode.Fetch && _modeTime > 6f)
			{
				Go(Mode.Return);
			}
		}

		private Vector2 DartHook()
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			if (OnScreen)
			{
				return GameRefs.PilePoint((PileType)3) + new Vector2(Tuning.F("tools.star_x", 6f), 0f - Tuning.F("tools.star_y", 150f));
			}
			Vector2 val = Anchor.SafeBodySize();
			return Anchor.Chest() + new Vector2(val.X * Tuning.F("tools.x", 0.4f) * Anchor.Side(), (0f - val.Y) * Tuning.F("tools.y", 0.8f));
		}

		private void HomeDart()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			_anchor = DartHook();
			_dartPos = _anchor + RopeLength * new Vector2(Mathf.Sin(_hangSwing), Mathf.Cos(_hangSwing));
			_dartAngle = (float)Math.PI - _hangSwing;
			for (int i = 0; i < _rope.Length; i++)
			{
				_rope[i] = _dartPos;
			}
		}

		private void GoDart(Dart state)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			if (state == Dart.Perch && _dart != Dart.Perch)
			{
				_settle = 0.35f;
			}
			_dartFrom = _dartPos;
			_dart = state;
			_dartTime = 0f;
		}

		private void CatchDart(float swingVel)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			_hangSwing = 0f;
			_hangSwingVel = swingVel;
			Kick = Mathf.Max(Kick, 0.7f);
			GoDart(Dart.Perch);
			_settle = 0.12f;
			Flash(DartHook(), 46f);
		}

		private void ApproachDart(Vector2 target, float rate, float dt)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			_dartPos = Toward(_dartPos, target, rate, dt);
		}

		private void ReadPicks(float dt)
		{
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			HandCards.FillSelected(_cards);
			if (_cards.Count == 0)
			{
				_noPicks += dt;
				if (_noPicks > 0.25f && HandCards.Selecting)
				{
					_picks.Clear();
				}
				return;
			}
			_noPicks = 0f;
			_picks.Clear();
			foreach (HandCards.Entry card in _cards)
			{
				if (card.Card != null && Seen(card.Node))
				{
					CardStandIn.Pose(card.Node, out var centre, out var rotation, out var scale);
					_picks.Add(new Pick
					{
						Card = card.Card,
						Node = card.Node,
						Centre = centre,
						Rot = rotation,
						Scale = scale
					});
				}
			}
		}

		private Vector2 AimPoint(out bool picked)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			picked = _picks.Count > 0;
			if (picked)
			{
				Vector2 val = Vector2.Zero;
				foreach (Pick pick in _picks)
				{
					val += pick.Centre;
				}
				return val / (float)_picks.Count;
			}
			Vector2 val2 = FxKit.ViewSize();
			if (HandCards.SelectedPlace(out var at) && at.X > val2.X * 0.15f && at.X < val2.X * 0.85f && at.Y > val2.Y * 0.12f && at.Y < val2.Y * 0.8f)
			{
				return at;
			}
			return new Vector2(val2.X * 0.5f, val2.Y * 0.42f);
		}

		private bool Loose(IReadOnlyList<CardModel> chosen)
		{
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			_shotPicks.Clear();
			foreach (CardModel item in chosen)
			{
				if (item == null || IsSly(item))
				{
					continue;
				}
				bool flag = false;
				foreach (Pick pick in _picks)
				{
					if (pick.Card == item)
					{
						_shotPicks.Add(pick);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					NCard node = HandCards.NodeOf(item);
					if (Shows(node, item) && Seen(node))
					{
						CardStandIn.Pose(node, out var centre, out var rotation, out var scale);
						_shotPicks.Add(new Pick
						{
							Card = item,
							Node = node,
							Centre = centre,
							Rot = rotation,
							Scale = scale
						});
					}
				}
			}
			_picks.Clear();
			if (_shotPicks.Count == 0 || !FxKit.Alive((GodotObject)(object)_rig))
			{
				return false;
			}
			FreeSkewer(showNow: true);
			Place();
			float num = PullTime + 0.08f;
			float duration = 1.17f * num + 1.125f * num * num;
			_hauling = false;
			_shotPicks.Sort((Pick a, Pick b) =>
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return a.Centre.X.CompareTo(b.Centre.X);
			});
			Vector2 val = Vector2.Zero;
			foreach (Pick shotPick in _shotPicks)
			{
				NCard val2 = CardStandIn.Make(shotPick.Card, (Node)(object)_rig);
				if (FxKit.Alive((GodotObject)(object)val2))
				{
					Skewered skewered = new Skewered
					{
						Card = shotPick.Card,
						Real = shotPick.Node,
						Copy = val2,
						Centre = shotPick.Centre,
						Rot = shotPick.Rot,
						Scale = shotPick.Scale,
						Pos = shotPick.Centre,
						NowRot = shotPick.Rot,
						NowScale = shotPick.Scale
					};
					NCard val3 = HandCards.NodeOf(skewered.Card);
					if (Shows(val3, skewered.Card))
					{
						skewered.Real = val3;
					}
					if (Shows(skewered.Real, skewered.Card))
					{
						CardStandIn.Hide(skewered.Real);
					}
					Skewered held = skewered;
					skewered.Plan = new CardFlight.Plan
					{
						HideCard = true,
						HideTrail = true,
						Straight = true,
						Duration = duration,
						HoldMax = 2f,
						Hold = () => Working && !_hauling && _skewer.Contains(held),
						Started = (NCard _, Vector2 _, Vector2 _) =>
						{
							held.Flown = true;
						}
					};
					CardFlight.Claim(skewered.Card, skewered.Plan);
					_skewer.Add(skewered);
					val += skewered.Centre;
				}
			}
			_shotPicks.Clear();
			int count = _skewer.Count;
			if (count == 0)
			{
				return false;
			}
			_entry = val / (float)count;
			for (int num2 = 0; num2 < count; num2++)
			{
				Skewered skewered2 = _skewer[num2];
				float num3 = (float)num2 - (float)(count - 1) * 0.5f;
				float num4 = Mathf.Min(1f, 3f / (float)count);
				skewered2.StackAt = _entry + new Vector2(8f * num3 * num4, -6f * num3 * num4) * skewered2.Scale.Y;
				skewered2.StackRot = 0.04f * num3 * num4;
			}
			if (count == 1)
			{
				_skewer[0].StackAt = _skewer[0].Centre;
				_skewer[0].StackRot = _skewer[0].Rot;
				return BeginShot();
			}
			float num5 = Sfx.Lead("gamble_shuffle");
			_gatherTime = Tuning.F("tools.gather_time", 0f);
			if (_gatherTime <= 0.01f)
			{
				_gatherTime = Mathf.Clamp(num5, 0.16f, 0.45f);
			}
			Sfx.Play("gamble_shuffle", Tuning.F("tools.gather_volume", 0.85f), Mathf.Max(0f, _gatherTime - num5));
			GoDart(Dart.Gather);
			return true;
		}

		private bool BeginShot()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			float kunaiSize = KunaiSize;
			Vector2 val = _entry - _dartPos;
			if (_skewer.Count == 0 || val.Length() < 60f)
			{
				FreeSkewer(showNow: true);
				return false;
			}
			_shotFrom = _dartPos;
			_shotDir = val.Normalized();
			_aim = _shotDir;
			_dartAngle = AngleOf(_shotDir);
			_ringAlong = 0f;
			_entryAlong = val.Length();
			_sinkDepth = Mathf.Clamp(Tuning.F("tools.embed", 22f), 4f, 36f) * kunaiSize;
			_ringStop = _entryAlong + _sinkDepth - 66f * kunaiSize;
			if (_ringStop < 20f)
			{
				FreeSkewer(showNow: true);
				return false;
			}
			_shotTime = Mathf.Clamp(_ringStop / Mathf.Max(1000f, Tuning.F("tools.shot_speed", 9000f)), 0.06f, 0.18f);
			_sink = 0f;
			_packScale = 1f;
			GoDart(Dart.Shoot);
			return true;
		}

		private static float Hermite(float u, float m0, float m1)
		{
			float num = u * u;
			float num2 = num * u;
			return (num2 - 2f * num + u) * m0 + (-2f * num2 + 3f * num) + (num2 - num) * m1;
		}

		private void BeginHaul(Vector2 hook)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			_hauling = true;
			_anchorFrom = _anchor;
			Vector2 val = _anchorFrom - hook;
			float num = val.Length();
			_anchorTo = ((num > 1f) ? (hook + val / num * Mathf.Min(num, Mathf.Max(0f, Tuning.F("tools.loop_lag", 90f)))) : hook);
			_haulEnd = Mathf.Clamp(_shotDir.Dot(_anchorTo - _shotFrom), -8f, _ringStop * 0.6f);
			Vector2 val2 = _shotFrom + _shotDir * _haulEnd - _anchorTo;
			_loopReach = Mathf.Max(8f, val2.Length());
			_loopFrom = Mathf.Atan2(val2.X, val2.Y);
			Vector2 val3 = -_shotDir;
			float num2 = val3.Dot(new Vector2(Mathf.Cos(_loopFrom), 0f - Mathf.Sin(_loopFrom)));
			_loopTurn = ((num2 >= 0f) ? ((float)Math.PI * 2f - _loopFrom) : (0f - ((float)Math.PI * 2f + _loopFrom)));
			if (Mathf.Abs(_loopTurn) < 3.7699115f)
			{
				_loopTurn += (float)Mathf.Sign(_loopTurn) * ((float)Math.PI * 2f);
			}
			float loopKeep = LoopKeep;
			float num3 = Mathf.Abs(_loopTurn) * ((1f - loopKeep) * 2.4f + loopKeep) / LoopTime * _loopReach;
			_haulSlope = Mathf.Clamp(num3 * PullTime / Mathf.Max(1f, _ringStop - _haulEnd), 0.25f, 1.6f);
			GoDart(Dart.Haul);
		}

		private float HoldDrawn(Vector2 target, bool hard, float dt)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = GameRefs.PilePoint((PileType)3) + new Vector2(0f, 0f - Tuning.F("tools.draw_lift", 30f));
			Vector2 val2 = target - val;
			if (val2.LengthSquared() > 400f)
			{
				_aim = Toward(_aim, val2.Normalized(), 10f, dt);
				_aim = (Vector2)((_aim.LengthSquared() > 0.0001f) ? _aim.Normalized() : new Vector2(-0.83f, -0.55f));
			}
			_pull = FxKit.Damp(_pull, hard ? 1f : 0.4f, 6f, dt);
			float num = Mathf.Lerp(Tuning.F("tools.draw_min", 6f), Tuning.F("tools.draw_back", 38f), _pull);
			Vector2 val3 = new Vector2(0f - _aim.Y, _aim.X);
			Vector2 val4 = val - _aim * num + val3 * (Mathf.Sin(Time * 43f) * (0.5f + 1.5f * _pull));
			ApproachDart(val4, 13f, dt);
			_dartAngle = Mathf.LerpAngle(_dartAngle, AngleOf(_aim), 1f - Mathf.Exp(-13f * dt));
			_anchor = Toward(_anchor, (target + val4) * 0.5f, Tuning.F("tools.slide", 8f), dt);
			return 0.8f + 2.2f * _pull;
		}

		private void FreeSkewer(bool showNow)
		{
			foreach (Skewered item in _skewer)
			{
				CardStandIn.Free(item.Copy);
				item.Copy = null;
				if (item.Flown)
				{
					continue;
				}
				if (!showNow)
				{
					if (Shows(item.Real, item.Card))
					{
						_orphans.Add(new Orphan
						{
							Of = item
						});
					}
				}
				else
				{
					CardFlight.Release(item.Card, item.Plan);
					if (Shows(item.Real, item.Card))
					{
						CardStandIn.Show(item.Real);
					}
				}
			}
			_skewer.Clear();
		}

		private void StepOrphans(float dt)
		{
			for (int num = _orphans.Count - 1; num >= 0; num--)
			{
				Orphan orphan = _orphans[num];
				orphan.For += dt;
				Skewered of = orphan.Of;
				bool flag = of.Flown || !Shows(of.Real, of.Card);
				if (flag || !(orphan.For < 1.5f))
				{
					if (!flag)
					{
						CardFlight.Release(of.Card, of.Plan);
						CardStandIn.Show(of.Real);
					}
					_orphans.RemoveAt(num);
				}
			}
		}

		private void StepRope(Vector2? pin, bool taut, float shake, float dt)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			int num = _rope.Length;
			_rope[0] = _dartPos;
			if (pin.HasValue)
			{
				Vector2 value = pin.Value;
				float num2 = _dartPos.DistanceTo(value);
				float num3 = (taut ? 0f : Mathf.Max(0f, RopeLength * 1.2f - num2));
				Vector2 b = (_dartPos + value) * 0.5f + new Vector2(0f, num3 * 0.6f);
				Vector2 val = ((num2 > 1f) ? (new Vector2(0f - (value.Y - _dartPos.Y), value.X - _dartPos.X) / num2) : Vector2.Zero);
				float num4 = ((shake > 0.01f) ? (shake * Mathf.Sin(Time * 57f)) : 0f);
				for (int i = 1; i < num; i++)
				{
					float num5 = (float)i / (float)(num - 1);
					_rope[i] = FxKit.Bezier(_dartPos, b, value, num5) + val * (num4 * Mathf.Sin((float)Math.PI * num5));
				}
			}
			else
			{
				float num6 = RopeLength / (float)(num - 1) * 0.9f;
				for (int j = 1; j < num; j++)
				{
					Vector2 val2 = _rope[j] + new Vector2(0f, 260f * dt) - _rope[j - 1];
					float num7 = val2.Length();
					_rope[j] = ((num7 > 0.001f) ? (_rope[j - 1] + val2 / num7 * num6) : (_rope[j - 1] + new Vector2(0f, num6)));
				}
			}
		}

		private void StepTrail(bool trailing)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)_rig))
			{
				return;
			}
			if (trailing && !FxKit.Alive((GodotObject)(object)_trail))
			{
				_trail = FxKit.Ribbon(Tuning.F("tools.trail", 11f), new Color(Silver.R, Silver.G, Silver.B, 0.85f));
				((Node)_trail).Name = StringName.op_Implicit("SilentFX_ToolsTrail");
				((CanvasItem)_trail).ZIndex = 1;
				((Node)_rig).AddChild((Node)(object)_trail, false, (InternalMode)0);
			}
			if (!FxKit.Alive((GodotObject)(object)_trail))
			{
				return;
			}
			if (trailing)
			{
				_trail.AddPoint(((Node2D)_trail).ToLocal(_dartPos), -1);
				while (_trail.GetPointCount() > 14)
				{
					_trail.RemovePoint(0);
				}
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				if (_trail.GetPointCount() <= 0)
				{
					break;
				}
				_trail.RemovePoint(0);
			}
		}

		private void TickDart(float dt)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0556: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0751: Unknown result type (might be due to invalid IL or missing references)
			//IL_0757: Unknown result type (might be due to invalid IL or missing references)
			//IL_076b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0770: Unknown result type (might be due to invalid IL or missing references)
			//IL_0775: Unknown result type (might be due to invalid IL or missing references)
			//IL_0942: Unknown result type (might be due to invalid IL or missing references)
			//IL_0947: Unknown result type (might be due to invalid IL or missing references)
			//IL_094f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0954: Unknown result type (might be due to invalid IL or missing references)
			//IL_095b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0970: Unknown result type (might be due to invalid IL or missing references)
			//IL_0975: Unknown result type (might be due to invalid IL or missing references)
			//IL_097a: Unknown result type (might be due to invalid IL or missing references)
			//IL_097f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b71: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b82: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b87: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b8c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf6: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_060a: Unknown result type (might be due to invalid IL or missing references)
			//IL_060f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_0795: Unknown result type (might be due to invalid IL or missing references)
			//IL_079b: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_0806: Unknown result type (might be due to invalid IL or missing references)
			//IL_0812: Unknown result type (might be due to invalid IL or missing references)
			//IL_0817: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a76: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a7b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a80: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a90: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_084e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0850: Unknown result type (might be due to invalid IL or missing references)
			//IL_0861: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0afc: Unknown result type (might be due to invalid IL or missing references)
			bool trailing = false;
			bool flag = true;
			bool taut = true;
			float shake = 0f;
			Vector2 val = DartHook();
			float kunaiSize = KunaiSize;
			switch (_dart)
			{
			case Dart.Thrown:
			{
				float num9 = Mathf.Clamp((_dartTime - 0.1f) / 0.45f, 0f, 1f);
				Vector2 val5 = val + new Vector2(0f, RopeLength);
				Vector2 b = new Vector2((_dartFrom.X + val5.X) * 0.5f, Mathf.Min(_dartFrom.Y, val5.Y) - 170f);
				Vector2 dartPos = _dartPos;
				_dartPos = FxKit.Bezier(_dartFrom, b, val5, num9);
				Vector2 val6 = _dartPos - dartPos;
				if (val6.LengthSquared() > 0.5f)
				{
					_dartAngle = AngleOf(val6.Normalized());
				}
				_anchor = val;
				flag = false;
				taut = false;
				trailing = true;
				if (num9 >= 1f)
				{
					CatchDart(6.5f);
				}
				break;
			}
			case Dart.Perch:
			{
				_anchor = Toward(_anchor, val, 18f, dt);
				float num4 = Tuning.F("tools.swing", 30f);
				_hangSwingVel += ((0f - num4) * Mathf.Sin(_hangSwing) - 1.1f * _hangSwingVel + 4f * Mathf.Sin(Time * 1.3f)) * dt;
				_hangSwing = Mathf.Clamp(_hangSwing + _hangSwingVel * dt, -1.3f, 1.3f);
				Vector2 val3 = _anchor + RopeLength * new Vector2(Mathf.Sin(_hangSwing), Mathf.Cos(_hangSwing));
				if (_settle > 0f)
				{
					_settle -= dt;
					ApproachDart(val3, 20f, dt);
				}
				else
				{
					_dartPos = val3;
				}
				_dartAngle = Mathf.LerpAngle(_dartAngle, (float)Math.PI - _hangSwing, 1f - Mathf.Exp(-16f * dt));
				_pull = FxKit.Damp(_pull, 0f, 8f, dt);
				if (_wantDraw)
				{
					_wantDraw = false;
					if (HandCards.Selecting)
					{
						GoDart(Dart.Draw);
					}
				}
				break;
			}
			case Dart.Draw:
			{
				ReadPicks(dt);
				Vector2 target = AimPoint(out var picked);
				shake = HoldDrawn(target, picked, dt);
				if (_dartTime > 0.9f && !HandCards.Selecting)
				{
					GoDart(Dart.Return);
				}
				break;
			}
			case Dart.Gather:
			{
				float num = Mathf.Clamp(_dartTime / Mathf.Max(0.05f, _gatherTime), 0f, 1f);
				float num2 = 1f - (1f - num) * (1f - num) * (1f - num);
				foreach (Skewered item in _skewer)
				{
					item.Pos = item.Centre.Lerp(item.StackAt, num2);
					item.NowRot = Mathf.LerpAngle(item.Rot, item.StackRot, num2);
					float num3 = ((num >= 1f) ? (0.03f * Mathf.Max(0f, 1f - (_dartTime - _gatherTime) / 0.08f)) : 0f);
					item.NowScale = item.Scale * (1f - num3);
				}
				shake = HoldDrawn(_entry, hard: true, dt);
				if (_dartTime >= _gatherTime + Mathf.Max(0f, Tuning.F("tools.gather_rest", 0.07f)) && !BeginShot())
				{
					GoDart(Dart.Return);
				}
				break;
			}
			case Dart.Shoot:
			{
				float num10 = Mathf.Clamp(_dartTime / Mathf.Max(0.02f, _shotTime), 0f, 1f);
				_ringAlong = _ringStop * num10;
				_dartPos = _shotFrom + _shotDir * _ringAlong;
				_dartAngle = AngleOf(_shotDir);
				_sink = Mathf.Clamp(_ringAlong + 66f * kunaiSize - _entryAlong, 0f, _sinkDepth);
				trailing = true;
				if (num10 >= 1f)
				{
					Flash(_entry, 120f);
					Sparks(_entry, 9);
					Kick = 1f;
					GoDart(Dart.Stuck);
				}
				break;
			}
			case Dart.Stuck:
			{
				float dartTime = _dartTime;
				_dartPos = _shotFrom + _shotDir * _ringStop;
				_dartAngle = AngleOf(_shotDir) + 0.05f * Mathf.Sin(dartTime * 62f) * Mathf.Exp((0f - dartTime) * 15f);
				_sink = _sinkDepth;
				shake = 6f * Mathf.Exp((0f - dartTime) * 11f);
				float num18 = Tuning.F("tools.knock", 12f) * Mathf.Exp((0f - dartTime) * 20f);
				foreach (Skewered item2 in _skewer)
				{
					item2.Pos = item2.StackAt + _shotDir * num18;
					item2.NowRot = item2.StackRot;
					item2.NowScale = item2.Scale;
				}
				if (dartTime >= Mathf.Max(0.02f, Tuning.F("tools.stick", 0.12f)))
				{
					BeginHaul(val);
				}
				break;
			}
			case Dart.Haul:
			{
				float pullTime = PullTime;
				float num5 = Mathf.Clamp(_dartTime / pullTime, 0f, 1f);
				_ringAlong = Mathf.Lerp(_ringStop, _haulEnd, Hermite(num5, 0.15f, _haulSlope));
				_dartPos = _shotFrom + _shotDir * _ringAlong;
				_dartAngle = AngleOf(_shotDir);
				float num6 = Mathf.Clamp(Tuning.F("tools.pull_shrink", 0.16f), 0.02f, 1f);
				float num7 = (_packScale = Mathf.Lerp(1f, Mathf.Lerp(num6, 1f, 0.3f), Mathf.Pow(num5, 1.4f)));
				_sink = _sinkDepth * Mathf.Lerp(1f, 0.5f, Mathf.Pow(num5, 1.4f));
				Vector2 val4 = _dartPos + _shotDir * (66f * kunaiSize - _sink);
				foreach (Skewered item3 in _skewer)
				{
					item3.Pos = val4 + (item3.StackAt - _entry) * num7;
					item3.NowScale = item3.Scale * num7;
					item3.NowRot = item3.StackRot;
					item3.Alpha = 1f;
				}
				_anchor = _anchorFrom.Lerp(_anchorTo, EaseBoth(num5));
				trailing = true;
				if (num5 >= 1f)
				{
					float num8 = _haulSlope * Mathf.Max(1f, _ringStop - _haulEnd) / pullTime;
					_dropFrom = val4;
					_dropScale = num7;
					_dropTime = Mathf.Clamp(val4.DistanceTo(GameRefs.PilePoint((PileType)3)) / Mathf.Max(600f, num8), 0.05f, 0.16f);
					_loopAngle = _dartAngle;
					_sink = 0f;
					GoDart(Dart.Loop);
				}
				break;
			}
			case Dart.Loop:
			{
				float loopTime = LoopTime;
				float num11 = Mathf.Clamp(_dartTime / loopTime, 0f, 1f);
				float loopKeep = LoopKeep;
				float num12 = (1f - loopKeep) * (1f - Mathf.Pow(1f - num11, 2.4f)) + loopKeep * num11;
				float num13 = _loopFrom + _loopTurn * num12;
				float num14 = Mathf.Lerp(_loopReach, RopeLength, EaseBoth(Mathf.Clamp(num11 / 0.6f, 0f, 1f)));
				_anchor = Toward(_anchor, val, 6f, dt);
				_dartPos = _anchor + num14 * new Vector2(Mathf.Sin(num13), Mathf.Cos(num13));
				_dartAngle = Mathf.LerpAngle(_loopAngle, (float)Math.PI - num13, EaseBoth(Mathf.Clamp(num11 / 0.4f, 0f, 1f)));
				trailing = num11 < 0.5f;
				if (_skewer.Count > 0)
				{
					float num15 = Mathf.Clamp(_dartTime / Mathf.Max(0.02f, _dropTime), 0f, 1f);
					Vector2 val7 = GameRefs.PilePoint((PileType)3);
					Vector2 val8 = _dropFrom.Lerp(val7, num15);
					float num16 = Mathf.Clamp(Tuning.F("tools.pull_shrink", 0.16f), 0.02f, 1f);
					float num17 = Mathf.Lerp(_dropScale, num16, num15);
					foreach (Skewered item4 in _skewer)
					{
						item4.Pos = val8 + (item4.StackAt - _entry) * num17;
						item4.NowScale = item4.Scale * num17;
						item4.NowRot = item4.StackRot;
						item4.Alpha = Mathf.Clamp((1f - num15) / 0.3f, 0f, 1f);
					}
					if (num15 >= 1f)
					{
						Flash(val7, 90f);
						Sparks(val7, 5);
						Kick = 1f;
						FreeSkewer(showNow: false);
					}
				}
				if (num11 >= 1f)
				{
					_hangSwing = 0f;
					_hangSwingVel = _loopTurn * loopKeep / loopTime;
					FreeSkewer(showNow: false);
					GoDart(Dart.Perch);
					_settle = 0f;
				}
				break;
			}
			case Dart.Return:
			{
				_anchor = Toward(_anchor, val, 14f, dt);
				Vector2 val2 = val + new Vector2(0f, RopeLength);
				float x = _dartPos.X;
				ApproachDart(val2, 11f, dt);
				_dartAngle = Mathf.LerpAngle(_dartAngle, (float)Math.PI, 1f - Mathf.Exp(-12f * dt));
				_pull = FxKit.Damp(_pull, 0f, 8f, dt);
				taut = false;
				if (_dartPos.DistanceTo(val2) < 8f || _dartTime > 0.9f)
				{
					CatchDart((_dartPos.X < x) ? (-4.5f) : 4.5f);
				}
				break;
			}
			default:
				GoDart(Dart.Return);
				break;
			}
			StepRope(flag ? new Vector2?(_anchor) : ((Vector2?)null), taut, shake, dt);
			StepTrail(trailing);
			StepOrphans(dt);
		}

		protected override void Tick(float dt)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				_modeTime += dt;
				_dartTime += dt;
				if (!_placed)
				{
					_placed = true;
					_pos = ((_mode == Mode.Thrown) ? _from : PerchPoint());
					HomeDart();
					if (_dart == Dart.Thrown)
					{
						_dartPos = _dartFrom;
						for (int i = 0; i < _rope.Length; i++)
						{
							_rope[i] = _dartPos;
						}
					}
				}
				if (_handDrawing)
				{
					_drawTime += dt;
					_sinceDrawn += dt;
					if ((_drawNotified && _sinceDrawn > 0.7f) || (!_drawNotified && _drawTime > 3f))
					{
						_handDrawing = false;
					}
				}
				TickKnife(dt);
				StepTosses(dt);
				TickDart(dt);
				Place();
			}
			catch (Exception ex)
			{
				DevLog.Warn("ToolsFx.Tick: " + ex);
				ClearFetch();
				FreeSkewer(showNow: true);
				Go(Mode.Return);
				GoDart(Dart.Return);
			}
		}

		protected override void Layout()
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!FxKit.Alive((GodotObject)(object)_rig) || !FxKit.Alive((GodotObject)(object)_fork))
				{
					return;
				}
				((CanvasItem)_rig).Visible = _placed;
				if (!_placed)
				{
					return;
				}
				((CanvasItem)_rig).Modulate = new Color(1f, 1f, 1f, A);
				float num = Tuning.F("tools.blade_size", 1.1f);
				_fork.GlobalTransform = new Transform2D(_angle, new Vector2(num, num), 0f, _pos);
				if (FxKit.Alive((GodotObject)(object)_glow))
				{
					((CanvasItem)_glow).Modulate = new Color(1f, 1f, 1f, (_mode == Mode.Perch) ? 0.55f : 1f);
				}
				if (FxKit.Alive((GodotObject)(object)_kunai))
				{
					float kunaiSize = KunaiSize;
					_kunai.GlobalTransform = new Transform2D(_dartAngle, new Vector2(kunaiSize, kunaiSize), 0f, _dartPos);
					bool flag = _sink > 0.5f && Busy && _skewer.Count > 0;
					SinkKunai(flag ? (_sink / kunaiSize) : 0f);
					Vector2 val = new Vector2(Mathf.Sin(_dartAngle), 0f - Mathf.Cos(_dartAngle));
					Vector2 val2 = _dartPos + val * (66f * kunaiSize - _sink);
					float num2 = Mathf.Clamp(_packScale, 0.1f, 1f);
					if (FxKit.Alive((GodotObject)(object)_slit))
					{
						((CanvasItem)_slit).Visible = flag;
						if (flag)
						{
							_slit.GlobalTransform = new Transform2D(_dartAngle, new Vector2(kunaiSize * num2, kunaiSize * num2), 0f, val2);
						}
					}
					if (FxKit.Alive((GodotObject)(object)_shadow))
					{
						((CanvasItem)_shadow).Visible = flag;
						if (flag)
						{
							Vector2 val3 = _dartPos - val2;
							Vector2 val4 = val2 + val3.Rotated(0.17f);
							_shadow.Points = new Vector2[2]
							{
								_rig.ToLocal(val2),
								_rig.ToLocal(val4)
							};
							_shadow.Width = 6.5f * kunaiSize * Mathf.Lerp(0.6f, 1f, num2);
						}
					}
					if (FxKit.Alive((GodotObject)(object)_kunaiGlow))
					{
						((CanvasItem)_kunaiGlow).Modulate = new Color(1f, 1f, 1f, (_dart == Dart.Perch) ? 0.5f : Mathf.Lerp(0.7f, 1f, _pull));
					}
				}
				if (FxKit.Alive((GodotObject)(object)_ropeLine) && FxKit.Alive((GodotObject)(object)_ropeEdge))
				{
					Vector2[] ropeLocal = _ropeLocal;
					float num3 = 0f;
					for (int i = 0; i < ropeLocal.Length; i++)
					{
						ropeLocal[i] = _rig.ToLocal(_rope[i]);
						if (i > 0)
						{
							num3 += _rope[i].DistanceTo(_rope[i - 1]);
						}
					}
					_ropeLine.Points = ropeLocal;
					_ropeEdge.Points = ropeLocal;
					float num4 = Mathf.Clamp(1f / Mathf.Sqrt(Mathf.Max(1f, num3 / Mathf.Max(1f, RopeLength))), 0.5f, 1f);
					_ropeLine.Width = 3.2f * num4;
					_ropeEdge.Width = 5.6f * num4;
				}
				if (FxKit.Alive((GodotObject)(object)_ring))
				{
					((Node2D)_ring).GlobalPosition = _anchor;
				}
				foreach (Fetch fetch in _fetches)
				{
					if (fetch.State != Got.Waiting && Shows(fetch.Copy, fetch.Card))
					{
						CardStandIn.Place(fetch.Copy, fetch.Pos, fetch.Angle, fetch.Scale);
					}
				}
				foreach (Skewered item in _skewer)
				{
					if (Shows(item.Copy, item.Card))
					{
						CardStandIn.Place(item.Copy, item.Pos, item.NowRot, item.NowScale);
						((CanvasItem)item.Copy).Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(item.Alpha, 0f, 1f));
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ToolsFx.Layout: " + ex);
				ClearFetch();
				FreeSkewer(showNow: true);
				Go(Mode.Return);
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			if (_mode == Mode.Perch)
			{
				_swingVel += 4.5f;
			}
			if (_dart == Dart.Perch)
			{
				_hangSwingVel += 4.5f;
			}
		}

		protected override void OnStacked()
		{
			if (_mode == Mode.Perch)
			{
				_swingVel += 7f;
				_hangSwingVel += 7f;
				Kick = 1f;
			}
		}

		protected override void OnStopping()
		{
			Unsubscribe();
			ClearFetch();
			FreeSkewer(showNow: true);
			foreach (Orphan orphan in _orphans)
			{
				if (!orphan.Of.Flown)
				{
					CardFlight.Release(orphan.Of.Card, orphan.Of.Plan);
					if (Shows(orphan.Of.Real, orphan.Of.Card))
					{
						CardStandIn.Show(orphan.Of.Real);
					}
				}
			}
			_orphans.Clear();
			_handDrawing = false;
			_wantDraw = false;
			Go(Mode.Return);
			GoDart(Dart.Return);
		}

		protected override void OnDispose()
		{
			try
			{
				OnStopping();
			}
			catch (Exception ex)
			{
				DevLog.Warn("ToolsFx stopping cleanup: " + ex);
			}
			try
			{
				if (FxKit.Alive((GodotObject)(object)_rig))
				{
					((Node)_rig).QueueFree();
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("ToolsFx.OnDispose: " + ex2);
			}
			_rig = null;
			_fork = null;
			_glow = null;
			_kunai = null;
			_kunaiGlow = null;
			_bladeFace = null;
			_bladeShade = null;
			_bladeRim = null;
			_bladeRidge = null;
			_slit = null;
			_shadow = null;
			_ropeEdge = null;
			_ropeLine = null;
			_ring = null;
			_trail = null;
		}

		public ToolsFx()
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		}

		static ToolsFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		}
	}
