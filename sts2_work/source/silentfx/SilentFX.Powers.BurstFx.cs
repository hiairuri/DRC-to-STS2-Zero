// namespace: SilentFX.Powers
// type: BurstFx (class)
// source lines: 2406-4468
	public sealed class BurstFx : BasicPowerFx
	{
		private sealed class Spark
		{
			public readonly List<Vector2> Trail = new List<Vector2>();

			public readonly List<float> At = new List<float>();

			public readonly List<bool> Near = new List<bool>();

			public Vector2[] BackPoints = Array.Empty<Vector2>();

			public Vector2[] FrontPoints = Array.Empty<Vector2>();

			public Line2D RibbonBack;

			public Line2D RibbonFront;

			public Gradient GradBack;

			public Gradient GradFront;

			public Sprite2D HeadBack;

			public Sprite2D HeadFront;

			public Color Tint;

			public Vector2 Head;

			public bool Gone;

			public float Way;

			public float Tilt;

			public float Phase;
		}

		private sealed class Orb
		{
			public CardModel Card;

			public FxAnchor Anchor;

			public Node2D Holder;

			public Line2D Ribbon;

			public Sprite2D Head;

			public Color Tint;

			public readonly List<Vector2> Trail = new List<Vector2>();

			public readonly List<float> At = new List<float>();

			public Vector2[] Points = Array.Empty<Vector2>();

			public Vector2 From;

			public Vector2 Spot;

			public Vector2 Pos;

			public float Off0;

			public float Glide;

			public float Clock;

			public float Age;

			public float Alpha;

			public float Fade = 1f;

			public bool Fading;

			public bool Remote;

			public object Room;

			public object Invocation;
		}

		private sealed class Ghost
		{
			public CardModel Card;

			public Node2D Holder;

			public NCard Copy;

			public bool Born;

			public float Age;

			public bool Called;

			public Vector2 Centre;

			public float Rotation;

			public Vector2 Scale;

			public float Wide;

			public float Tall;

			public float Alpha;

			public float Grow;

			public float Rise;

			public float Hold;

			public bool Punch;

			public float PunchAt = -1f;

			public float EndAt = -1f;

			public bool Punched;

			public bool Dropped;
		}

		private static readonly Color Gold = new Color(1f, 0.8f, 0.32f, 1f);

		private static readonly Color Pale = new Color(1f, 0.96f, 0.82f, 1f);

		private static readonly Color GhostGold = new Color(1f, 0.86f, 0.5f, 1f);

		private const float HeadSize = 46f;

		private static readonly Vector2 HeadScale = new Vector2(46f, 46f) / 64f;

		private const float LetMax = 4f;

		private const float OrbMax = 15f;

		private const float EmergeTime = 0.18f;

		private const float PunchTime = 0.06f;

		private const float DissolveTime = 0.3f;

		private static readonly List<BurstFx> Live = new List<BurstFx>();

		private static readonly List<Orb> Orbs = new List<Orb>();

		private static readonly List<Ghost> Ghosts = new List<Ghost>();

		private static bool _hooked;

		private static bool _ticking;

		private static CardModel _noted;

		private readonly List<Spark> _sparks = new List<Spark>();

		private readonly float[] _maskMarks = new float[7];

		private readonly float[] _maskOffsetScratch = new float[7];

		private readonly Color[] _maskColorScratch = new Color[7];

		private readonly float[][] _maskOffsets = new float[8][];

		private readonly Color[][] _maskColors = new Color[8][];

		private Sprite2D _core;

		private CardModel _for;

		private bool _let;

		private float _letFor;

		private CardModel _seen;

		private bool _seenFits;

		private int _waiter = -1;

		private float _wait;

		private float _goneFor;

		private readonly HashSet<CardModel> _remotePlayed = new HashSet<CardModel>(ReferenceEqualityComparer.Instance);

		private object _remoteRoom;

		private bool _remoteMode;

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

		protected override string NodeName => "Burst";

		protected override float KickDecay => 4f;

		private float Off
		{
			get
			{
				if (_waiter >= 0)
				{
					return _wait * _wait * (3f - 2f * _wait);
				}
				return 0f;
			}
		}

		private static int CountFor(int amount)
		{
			return Mathf.Clamp(amount, 1, 4);
		}

		internal static void Note(CardModel card)
		{
			_noted = card;
		}

		protected override void Build(bool animateIn)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			Live.Add(this);
			if (!_hooked)
			{
				_hooked = true;
				HeldFxManager.LetGo += OnLetGo;
			}
			_core = FxKit.Glow(new Color(Gold.R, Gold.G, Gold.B, 0.2f), new Vector2(170f, 170f));
			((Node)Back).AddChild((Node)(object)_core, false, (InternalMode)0);
			Rebuild();
			if (animateIn)
			{
				Flourish();
			}
		}

		private void Rebuild()
		{
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Expected Obj, but got Unknown
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Expected Obj, but got Unknown
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			foreach (Spark spark2 in _sparks)
			{
				Node[] array = new Node[4]
				{
					(Node)spark2.RibbonBack,
					(Node)spark2.RibbonFront,
					(Node)spark2.HeadBack,
					(Node)spark2.HeadFront
				};
				foreach (Node val in array)
				{
					if (FxKit.Alive((GodotObject)(object)val))
					{
						val.QueueFree();
					}
				}
			}
			_sparks.Clear();
			_waiter = -1;
			_wait = 0f;
			_goneFor = 0f;
			Shape(out var centre, out var rx, out var ry, out var speed);
			int num = CountFor(Amount);
			for (int j = 0; j < num; j++)
			{
				Color val2 = ((j % 2 == 0) ? Gold : Pale);
				float num2 = ((j % 2 == 0) ? 1f : (-1f));
				Spark spark = new Spark
				{
					Tint = val2,
					GradBack = new Gradient(),
					GradFront = new Gradient(),
					Way = num2,
					Tilt = num2 * ((j < 2) ? 0.45f : 0.12f),
					Phase = (float)j * ((float)Math.PI * 2f) / (float)num
				};
				spark.RibbonBack = FxKit.Ribbon(11f, new Color(val2.R, val2.G, val2.B, 0.85f));
				spark.RibbonBack.Gradient = spark.GradBack;
				((Node)Back).AddChild((Node)(object)spark.RibbonBack, false, (InternalMode)0);
				spark.HeadBack = FxKit.Glow(new Color(val2.R, val2.G, val2.B, 0.95f), new Vector2(46f, 46f));
				((Node)Back).AddChild((Node)(object)spark.HeadBack, false, (InternalMode)0);
				spark.RibbonFront = FxKit.Ribbon(11f, new Color(val2.R, val2.G, val2.B, 0.85f));
				spark.RibbonFront.Gradient = spark.GradFront;
				((Node)Front).AddChild((Node)(object)spark.RibbonFront, false, (InternalMode)0);
				spark.HeadFront = FxKit.Glow(new Color(val2.R, val2.G, val2.B, 0.95f), new Vector2(46f, 46f));
				((Node)Front).AddChild((Node)(object)spark.HeadFront, false, (InternalMode)0);
				Vector2 item = (spark.Head = Orbit(spark, centre, rx, ry, speed, out var near));
				spark.Trail.Add(item);
				spark.At.Add(Time);
				spark.Near.Add(near);
				_sparks.Add(spark);
			}
		}

		private int Last()
		{
			for (int num = _sparks.Count - 1; num >= 0; num--)
			{
				if (!_sparks[num].Gone)
				{
					return num;
				}
			}
			return -1;
		}

		private bool Missing()
		{
			foreach (Spark spark in _sparks)
			{
				if (spark.Gone)
				{
					return true;
				}
			}
			return false;
		}

		public override void OnAmountChanged(int amount)
		{
			try
			{
				if (Dead || !FxKit.Alive((GodotObject)(object)Front) || !FxKit.Alive((GodotObject)(object)Back) || amount <= 0)
				{
					return;
				}
				int num = 0;
				foreach (Spark spark2 in _sparks)
				{
					if (spark2.Gone)
					{
						num++;
					}
				}
				if (num > 0 && CountFor(amount) == _sparks.Count - num)
				{
					Spark spark = ((_waiter >= 0 && _waiter < _sparks.Count) ? _sparks[_waiter] : null);
					for (int num2 = _sparks.Count - 1; num2 >= 0; num2--)
					{
						if (_sparks[num2].Gone)
						{
							_sparks.RemoveAt(num2);
						}
					}
					_waiter = ((spark != null) ? _sparks.IndexOf(spark) : (-1));
					if (_waiter < 0)
					{
						_wait = 0f;
					}
					_goneFor = 0f;
				}
				else if (CountFor(amount) != _sparks.Count || num > 0)
				{
					Rebuild();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BurstFx.OnAmountChanged: " + ex);
				Dispose();
			}
		}

		private Vector2 Centre()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return CentreOf(Anchor);
		}

		private static Vector2 CentreOf(FxAnchor anchor)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = anchor.SafeBodySize();
			return anchor.Chest() + new Vector2(0f, val.Y * 0.04f);
		}

		private static Vector2 Spot(FxAnchor anchor, Vector2 centre, Vector2 body)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			return centre + new Vector2((0f - anchor.Side()) * body.X * Tuning.F("burst.wait_back", 0.8f), (0f - body.Y) * Tuning.F("burst.wait_up", 0.26f));
		}

		private static Vector2 SpotOf(FxAnchor anchor)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return Spot(anchor, CentreOf(anchor), anchor.SafeBodySize());
		}

		private static Vector2 Tremble(float clock)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			return new Vector2(0.6f * Mathf.Sin(clock * 61f) + 0.4f * Mathf.Sin(clock * 97f + 1.3f), 0.6f * Mathf.Sin(clock * 73f + 0.7f) + 0.4f * Mathf.Sin(clock * 113f + 2.1f));
		}

		private static void OnLetGo(CardModel card, bool played)
		{
			try
			{
				if (!played || card == null)
				{
					return;
				}
				for (int num = Live.Count - 1; num >= 0; num--)
				{
					BurstFx burstFx = Live[num];
					if (!burstFx.RemoteOwner && !burstFx.Dead && burstFx._for == card)
					{
						burstFx._let = true;
						burstFx._letFor = 0f;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BurstFx.OnLetGo: " + ex);
			}
		}

		private static bool Doubled(CardModel card, Creature who)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			try
			{
				if ((int)card.Type != 2)
				{
					goto IL_001e;
				}
				Player owner = card.Owner;
				if (((owner != null) ? owner.Creature : null) != who)
				{
					goto IL_001e;
				}
				goto end_IL_0000;
				IL_001e:
				return false;
				end_IL_0000:;
			}
			catch
			{
				return false;
			}
			try
			{
				return card.CanPlay();
			}
			catch
			{
				return true;
			}
		}

		private bool Wanted(float dt)
		{
			if (RemoteOwner)
			{
				return RemoteWanted();
			}
			if (_remoteMode)
			{
				ClearRemote();
			}
			object owner = Anchor.Owner;
			Creature val = (Creature)((owner is Creature) ? owner : null);
			if (val == null || Stopping || Tuning.F("burst.wait", 1f) < 0.5f)
			{
				_for = null;
				_let = false;
				return false;
			}
			if (_let)
			{
				_letFor += dt;
				if (_letFor > 4f)
				{
					_let = false;
				}
			}
			if (!_let)
			{
				CardModel carried = HeldFxManager.Carried;
				if (carried != _seen)
				{
					_seen = carried;
					_seenFits = carried != null && Doubled(carried, val);
				}
				_for = ((carried != null && _seenFits && HeldFxManager.CarriedOut) ? carried : null);
			}
			return _for != null;
		}

		private void ClearRemote()
		{
			_remotePlayed.Clear();
			_remoteRoom = null;
			_remoteMode = false;
			_for = (_seen = null);
			_seenFits = (_let = false);
			_letFor = 0f;
		}

		private void EnterRemote()
		{
			if (!_remoteMode || _remoteRoom != GameRefs.CombatRoom)
			{
				ClearRemote();
			}
			_remoteMode = true;
			_remoteRoom = GameRefs.CombatRoom;
		}

		private bool RemoteWanted()
		{
			EnterRemote();
			if (Stopping || Tuning.F("burst.wait", 1f) < 0.5f)
			{
				_for = null;
				_let = false;
				return false;
			}
			object owner = Anchor.Owner;
			if (!RemoteHeldFx.TryObserve((Creature)((owner is Creature) ? owner : null), out var card, out var _, out var released))
			{
				_for = (_seen = null);
				_seenFits = (_let = false);
				if (released)
				{
					if (card == null)
					{
						_remotePlayed.Clear();
					}
					else
					{
						_remotePlayed.Remove(card);
					}
				}
				return false;
			}
			if (card != _seen)
			{
				_seen = card;
				int seenFits;
				if (card != null)
				{
					CardModel card2 = card;
					object owner2 = Anchor.Owner;
					seenFits = (Doubled(card2, (Creature)((owner2 is Creature) ? owner2 : null)) ? 1 : 0);
				}
				else
				{
					seenFits = 0;
				}
				_seenFits = (byte)seenFits != 0;
			}
			_for = ((_seenFits && !_remotePlayed.Contains(card)) ? card : null);
			return _for != null;
		}

		private void RemotePlayed(CardModel card)
		{
			EnterRemote();
			_remotePlayed.Add(card);
			if (_for == card)
			{
				_for = null;
				_let = false;
			}
		}

		private void Shape(out Vector2 centre, out float rx, out float ry, out float speed)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = Anchor.SafeBodySize();
			centre = Centre();
			rx = val.X * Tuning.F("burst.rx_mul", 0.52f);
			ry = val.Y * Tuning.F("burst.ry_mul", 0.2f);
			speed = Tuning.F("burst.speed", 5.2f);
		}

		private Vector2 Orbit(Spark spark, Vector2 centre, float rx, float ry, float speed, out bool near)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			float num = Time * speed * spark.Way + spark.Phase;
			Vector2 val = new Vector2(Mathf.Cos(num) * rx, Mathf.Sin(num) * ry);
			near = Mathf.Sin(num) > 0f;
			return centre + val.Rotated(spark.Tilt);
		}

		protected override void Tick(float dt)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			if (!Stopping && Amount > 0 && Missing())
			{
				_goneFor += dt;
				if (_goneFor > 3f)
				{
					Rebuild();
				}
			}
			Vector2 body = Anchor.SafeBodySize();
			Shape(out var centre, out var rx, out var ry, out var speed);
			float num = Tuning.F("burst.trail", 0.3f);
			int count = _sparks.Count;
			bool flag = Wanted(dt);
			Vector2 val = centre;
			if (flag || _waiter >= 0)
			{
				val = Spot(Anchor, centre, body);
			}
			if (flag && _waiter < 0)
			{
				_waiter = Last();
				_wait = 0f;
			}
			if (_waiter >= 0)
			{
				if (_waiter >= count || _sparks[_waiter].Gone)
				{
					_waiter = -1;
					_wait = 0f;
				}
				else
				{
					_wait = FxKit.Damp(_wait, flag ? 1f : 0f, flag ? 9f : 6f, dt);
					if (!flag && _wait < 0.003f)
					{
						_waiter = -1;
						_wait = 0f;
					}
				}
			}
			float off = Off;
			for (int i = 0; i < count; i++)
			{
				Spark spark = _sparks[i];
				if (spark.Gone)
				{
					continue;
				}
				Vector2 val2 = Orbit(spark, centre, rx, ry, speed, out var near);
				Vector2 head = val2;
				if (i == _waiter && off > 0f)
				{
					val2 = val2.Lerp(val, off);
					float num2 = Mathf.Clamp(Tuning.F("burst.wait_shake", 2.5f), 0f, 20f) * off;
					head = val2 + Tremble(Time) * num2;
					if (off > 0.5f)
					{
						near = true;
					}
				}
				spark.Head = head;
				spark.Trail.Add(val2);
				spark.At.Add(Time);
				spark.Near.Add(near);
				while (spark.Trail.Count > 2 && (Time - spark.At[0] > num || spark.Trail.Count > 120))
				{
					spark.Trail.RemoveAt(0);
					spark.At.RemoveAt(0);
					spark.Near.RemoveAt(0);
				}
			}
		}

		private void Mask(Gradient gradient, Color c, float lo, float hi)
		{
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			float[] maskMarks = _maskMarks;
			maskMarks[0] = 0f;
			maskMarks[1] = lo - 0.012f;
			maskMarks[2] = lo + 0.012f;
			maskMarks[3] = 0.45f;
			maskMarks[4] = hi - 0.012f;
			maskMarks[5] = hi + 0.012f;
			maskMarks[6] = 1f;
			Array.Sort(maskMarks);
			int num = 0;
			float num2 = -1f;
			float[] array = maskMarks;
			int i;
			for (i = 0; i < array.Length; i++)
			{
				float num3 = Mathf.Clamp(array[i], 0f, 1f);
				if (!(num3 <= num2 + 0.0005f))
				{
					num2 = num3;
					float num4 = ((num3 < 0.45f) ? Mathf.Lerp(0f, 0.7f, num3 / 0.45f) : Mathf.Lerp(0.7f, 1f, (num3 - 0.45f) / 0.55f));
					if (num3 < lo || num3 > hi)
					{
						num4 = 0f;
					}
					_maskOffsetScratch[num] = num3;
					_maskColorScratch[num] = new Color(c.R, c.G, c.B, c.A * num4);
					num++;
				}
			}
			float[][] maskOffsets = _maskOffsets;
			i = num;
			float[] array2 = maskOffsets[i] ?? (maskOffsets[i] = new float[num]);
			Color[][] maskColors = _maskColors;
			i = num;
			Color[] array3 = maskColors[i] ?? (maskColors[i] = new Color[num]);
			Array.Copy(_maskOffsetScratch, array2, num);
			Array.Copy(_maskColorScratch, array3, num);
			gradient.Offsets = array2;
			gradient.Colors = array3;
		}

		protected override void Layout()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0407: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_core))
			{
				((Node2D)_core).GlobalPosition = Centre();
				((CanvasItem)_core).Modulate = new Color(Gold.R, Gold.G, Gold.B, (0.16f + 0.05f * Mathf.Sin(Time * 6f) + 0.5f * Kick) * A);
			}
			for (int i = 0; i < _sparks.Count; i++)
			{
				Spark spark = _sparks[i];
				if (spark.Gone)
				{
					continue;
				}
				List<Vector2> trail = spark.Trail;
				int count = trail.Count;
				if (count == 0 || !FxKit.Alive((GodotObject)(object)spark.RibbonBack) || !FxKit.Alive((GodotObject)(object)spark.RibbonFront))
				{
					continue;
				}
				bool flag = spark.Near[count - 1];
				float num = 0f;
				for (int num2 = count - 2; num2 >= 0; num2--)
				{
					if (spark.Near[num2] != flag)
					{
						num = ((count > 1) ? (((float)num2 + 0.5f) / (float)(count - 1)) : 0f);
						break;
					}
				}
				if (spark.BackPoints.Length != count)
				{
					spark.BackPoints = new Vector2[count];
					spark.FrontPoints = new Vector2[count];
				}
				Vector2[] backPoints = spark.BackPoints;
				Vector2[] frontPoints = spark.FrontPoints;
				for (int j = 0; j < count; j++)
				{
					backPoints[j] = Back.ToLocal(trail[j]);
					frontPoints[j] = Front.ToLocal(trail[j]);
				}
				spark.RibbonBack.Points = backPoints;
				spark.RibbonFront.Points = frontPoints;
				float num3 = ((i == _waiter) ? Off : 0f);
				float num4 = 1f - num3 * num3 * num3;
				Color c = new Color(spark.Tint.R, spark.Tint.G, spark.Tint.B, 0.85f);
				if (flag)
				{
					Mask(spark.GradFront, c, num, 1f);
					Mask(spark.GradBack, c, 0f, num);
				}
				else
				{
					Mask(spark.GradBack, c, num, 1f);
					Mask(spark.GradFront, c, 0f, num);
				}
				((CanvasItem)spark.RibbonBack).Visible = num4 > 0.02f && (!flag || num > 0.001f);
				((CanvasItem)spark.RibbonFront).Visible = num4 > 0.02f && (flag || num > 0.001f);
				((CanvasItem)spark.RibbonBack).Modulate = new Color(1f, 1f, 1f, A * num4);
				((CanvasItem)spark.RibbonFront).Modulate = new Color(1f, 1f, 1f, A * num4);
				Vector2 head = spark.Head;
				float num5 = 0.35f * num3;
				Color modulate = new Color(Mathf.Lerp(spark.Tint.R, 1f, num5), Mathf.Lerp(spark.Tint.G, 1f, num5), Mathf.Lerp(spark.Tint.B, 1f, num5), (0.95f + 0.05f * num3) * A);
				Vector2 scale = HeadScale * (1f + 0.22f * num3);
				if (FxKit.Alive((GodotObject)(object)spark.HeadBack))
				{
					((CanvasItem)spark.HeadBack).Visible = !flag;
					((Node2D)spark.HeadBack).GlobalPosition = head;
					((CanvasItem)spark.HeadBack).Modulate = modulate;
					((Node2D)spark.HeadBack).Scale = scale;
				}
				if (FxKit.Alive((GodotObject)(object)spark.HeadFront))
				{
					((CanvasItem)spark.HeadFront).Visible = flag;
					((Node2D)spark.HeadFront).GlobalPosition = head;
					((CanvasItem)spark.HeadFront).Modulate = modulate;
					((Node2D)spark.HeadFront).Scale = scale;
				}
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			CardModel noted = _noted;
			_noted = null;
			bool flag = noted != null && RemoteOwner;
			if (flag)
			{
				RemotePlayed(noted);
			}
			if (noted != null && !flag)
			{
				_let = false;
				_for = null;
				if (noted == _seen)
				{
					_seenFits = false;
				}
			}
			bool flag2 = false;
			try
			{
				if (noted != null)
				{
					flag2 = ((flag || Tuning.F("burst.ghost", 1f) >= 0.5f) ? Detach(noted) : Launch(noted));
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BurstFx.OnTrigger: " + ex);
			}
			if (flag2)
			{
				Kick = 0f;
			}
			else
			{
				Flourish();
			}
		}

		private static void Drop(CanvasItem node)
		{
			if (FxKit.Alive((GodotObject)(object)node))
			{
				node.Visible = false;
				((Node)node).QueueFree();
			}
		}

		private bool Launch(CardModel card)
		{
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			if (MultiplayerFx.Active && !MultiplayerFx.IsLocal(card))
			{
				return false;
			}
			NCard node = HandCards.NodeOf(card);
			if (!Shows(node, card))
			{
				return false;
			}
			int num = Chosen();
			if (num < 0)
			{
				return false;
			}
			Spark spark = _sparks[num];
			if (spark.Trail.Count == 0)
			{
				return false;
			}
			Node val = Top(Anchor.FrontParent);
			if (val == null)
			{
				return false;
			}
			float seen = 46f * FogKit.ScaleOf((Node)(object)Front);
			if (!Shoot(val, spark.Head, card, node, spark.Tint, seen, withGhost: false))
			{
				return false;
			}
			Leave(num);
			return true;
		}

		private int Chosen()
		{
			if (_waiter < 0 || _waiter >= _sparks.Count || _sparks[_waiter].Gone)
			{
				return Last();
			}
			return _waiter;
		}

		private void Leave(int pick)
		{
			Spark spark = _sparks[pick];
			spark.Gone = true;
			Drop((CanvasItem)(object)spark.RibbonBack);
			Drop((CanvasItem)(object)spark.RibbonFront);
			Drop((CanvasItem)(object)spark.HeadBack);
			Drop((CanvasItem)(object)spark.HeadFront);
			if (pick == _waiter)
			{
				_waiter = -1;
				_wait = 0f;
			}
			_goneFor = 0f;
		}

		private static Node Top(Node fallback)
		{
			Node val = GameRefs.Overlay;
			if (FxKit.Alive((GodotObject)(object)val) && val.IsInsideTree())
			{
				CanvasItem val2 = (CanvasItem)(object)((val is CanvasItem) ? val : null);
				if (val2 == null || val2.IsVisibleInTree())
				{
					goto IL_002a;
				}
			}
			val = fallback;
			goto IL_002a;
			IL_002a:
			if (!FxKit.Alive((GodotObject)(object)val) || !val.IsInsideTree())
			{
				return null;
			}
			return val;
		}

		private static bool Shows(NCard node, CardModel card)
		{
			if (FxKit.Alive((GodotObject)(object)node) && ((Node)node).IsInsideTree())
			{
				return node.Model == card;
			}
			return false;
		}

		private static bool Shoot(Node layer, Vector2 from, CardModel card, NCard node, Color tint, float seen, bool withGhost)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected Obj, but got Unknown
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_BurstShot")
			};
			layer.AddChild((Node)(object)holder, false, (InternalMode)0);
			if (!((Node)holder).IsInsideTree())
			{
				((Node)holder).QueueFree();
				return false;
			}
			float unit = 1f / FogKit.ScaleOf(layer);
			float fly = Mathf.Clamp(Tuning.F("burst.shot_time", 0.14f), 0.04f, 2f);
			int ghosts = Mathf.Clamp(Mathf.RoundToInt(Tuning.F("burst.shot_ghosts", 5f)), 0, 12);
			float pop = Mathf.Clamp(Tuning.F("burst.pop", 1f), 0f, 4f);
			float total = fly + 0.26f;
			FxKit.FreeAfter((Node)(object)holder, total + 0.1f);
			Color val = new Color(Mathf.Lerp(tint.R, 1f, 0.45f), Mathf.Lerp(tint.G, 1f, 0.45f), Mathf.Lerp(tint.B, 1f, 0.45f), 1f);
			Line2D streak = FxKit.Ribbon(Mathf.Max(3f, seen * 0.16f) * unit, new Color(val.R, val.G, val.B, 0.95f));
			((CanvasItem)streak).Visible = false;
			((Node)holder).AddChild((Node)(object)streak, false, (InternalMode)0);
			Sprite2D head = FxKit.Glow(val, new Vector2(seen, seen) * (1.2f * unit));
			((Node)holder).AddChild((Node)(object)head, false, (InternalMode)0);
			((Node2D)head).GlobalPosition = from;
			Vector2[] points = new Vector2[13];
			CardStandIn.Pose(node, out var aim, out var _, out var drawn);
			int dropped = 0;
			bool landed = false;
			Ghost twin = null;
			if (withGhost)
			{
				twin = new Ghost
				{
					Card = card
				};
				Ghosts.Add(twin);
				Wake();
			}
			Action<float> action = (float u) =>
			{
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0120: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_0132: Unknown result type (might be due to invalid IL or missing references)
				//IL_015d: Unknown result type (might be due to invalid IL or missing references)
				//IL_024c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0251: Unknown result type (might be due to invalid IL or missing references)
				//IL_0254: Unknown result type (might be due to invalid IL or missing references)
				//IL_025b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_0330: Unknown result type (might be due to invalid IL or missing references)
				float num = u * total;
				float num2 = Mathf.Clamp(num / fly, 0f, 1f);
				if (!landed)
				{
					if (!Shows(node, card))
					{
						node = HandCards.NodeOf(card);
					}
					if (Shows(node, card))
					{
						CardStandIn.Pose(node, out aim, out var _, out drawn);
					}
				}
				Vector2 val2 = aim - from;
				Vector2 val3 = new Vector2(val2.Y, 0f - val2.X);
				if (val3.Y > 0f)
				{
					val3 = -val3;
				}
				Vector2 b = (from + aim) * 0.5f + val3 * 0.16f;
				float num3 = num2 * num2;
				while (dropped < ghosts && num2 >= ((float)dropped + 1f) / ((float)ghosts + 1f))
				{
					float num4 = ((float)dropped + 1f) / ((float)ghosts + 1f);
					dropped++;
					FxKit.Flash((Node)(object)holder, FxKit.Bezier(from, b, aim, num4 * num4), new Color(tint.R, tint.G, tint.B, 0.5f), seen * unit, 0.55f, 0.2);
				}
				if (!landed && num2 >= 1f)
				{
					landed = true;
					if (FxKit.Alive((GodotObject)(object)head))
					{
						((CanvasItem)head).Visible = false;
					}
					Pop(layer, aim, drawn, pop, (twin != null) ? 6 : 12);
					if (twin != null)
					{
						Bear(twin, layer, card, node);
					}
				}
				if (!landed && FxKit.Alive((GodotObject)(object)head))
				{
					((Node2D)head).GlobalPosition = FxKit.Bezier(from, b, aim, num3);
				}
				if (FxKit.Alive((GodotObject)(object)streak))
				{
					float num5 = Mathf.Clamp((num - fly) / 0.1f, 0f, 1f);
					float num6 = Mathf.Lerp(Mathf.Max(0f, num3 - 0.55f), 1f, num5);
					for (int i = 0; i <= 12; i++)
					{
						points[i] = ((Node2D)streak).ToLocal(FxKit.Bezier(from, b, aim, Mathf.Lerp(num6, num3, (float)i / 12f)));
					}
					streak.Points = points;
					((CanvasItem)streak).Modulate = new Color(1f, 1f, 1f, 1f - num5);
					((CanvasItem)streak).Visible = num5 < 0.999f && num3 - num6 > 0.0005f;
				}
			};
			action(0f);
			FxKit.Drive((Node)(object)holder, total, action, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
			return true;
		}

		private static void Pop(Node layer, Vector2 at, Vector2 drawn, float pop, int sparks)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			if (!(pop <= 0.01f) && FxKit.Alive((GodotObject)(object)layer) && layer.IsInsideTree())
			{
				float num = 1f / FogKit.ScaleOf(layer);
				float num2 = Mathf.Clamp(drawn.Y, 0.2f, 1.5f) * pop;
				float num3 = 422f * num2 * num;
				FxKit.Flash(layer, at, new Color(1f, 0.94f, 0.72f, 0.85f), num3 * 0.42f, 2f, 0.2);
				FxKit.RingPulse(layer, at, num3 * 1.25f, Gold, 0f, 0.32f, 0.22f, 1f, 1f, 0f, 0.035f, 0.9f);
				FxKit.Burst(layer, at, new Color(1f, 0.86f, 0.45f, 0.95f), sparks, 170f * num2 * num, 520f * num2 * num, 0.4, 0.08f * num2 * num, 0.22f * num2 * num, Vector2.Zero);
			}
		}

		private bool Detach(CardModel card)
		{
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Expected Obj, but got Unknown
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			bool remoteOwner = RemoteOwner;
			if (remoteOwner)
			{
				if (card != null)
				{
					Player owner = card.Owner;
					if (((owner != null) ? owner.Creature : null) == Anchor.Owner && MultiplayerFx.CurrentInvocation != null && FxKit.Alive((GodotObject)(object)GameRefs.CombatRoom))
					{
						goto IL_0041;
					}
				}
				return false;
			}
			goto IL_0041;
			IL_0041:
			int num = Chosen();
			if (num < 0)
			{
				return false;
			}
			Spark spark = _sparks[num];
			int count = spark.Trail.Count;
			if (count == 0)
			{
				return false;
			}
			Node frontParent = Anchor.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)frontParent) || !frontParent.IsInsideTree())
			{
				return false;
			}
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_BurstOrb")
			};
			frontParent.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return false;
			}
			FxKit.FreeAfter((Node)(object)val, 16.0);
			Color tint = spark.Tint;
			Orb orb = new Orb
			{
				Card = card,
				Anchor = Anchor,
				Holder = val,
				Tint = tint,
				From = spark.Trail[count - 1],
				Off0 = ((num == _waiter) ? Off : 0f),
				Clock = Time,
				Alpha = A,
				Remote = remoteOwner,
				Room = (remoteOwner ? GameRefs.CombatRoom : null),
				Invocation = (remoteOwner ? MultiplayerFx.CurrentInvocation : null)
			};
			orb.Spot = SpotOf(Anchor);
			orb.Ribbon = FxKit.Ribbon(11f, new Color(tint.R, tint.G, tint.B, 0.85f));
			((Node)val).AddChild((Node)(object)orb.Ribbon, false, (InternalMode)0);
			orb.Head = FxKit.Glow(new Color(tint.R, tint.G, tint.B, 0.95f), new Vector2(46f, 46f));
			((Node)val).AddChild((Node)(object)orb.Head, false, (InternalMode)0);
			int num2 = count - 1;
			if (spark.Near[num2])
			{
				while (num2 > 0 && spark.Near[num2 - 1])
				{
					num2--;
				}
			}
			for (int i = num2; i < count; i++)
			{
				orb.Trail.Add(spark.Trail[i]);
				orb.At.Add(spark.At[i]);
			}
			Lay(orb);
			Orbs.Add(orb);
			Wake();
			Leave(num);
			return true;
		}

		private static void Wake()
		{
			if (!_ticking)
			{
				_ticking = true;
				Ticker.Add(Step);
			}
		}

		private static void Step(double realDt)
		{
			if (Orbs.Count == 0 && Ghosts.Count == 0)
			{
				return;
			}
			float dt = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			for (int num = Orbs.Count - 1; num >= 0; num--)
			{
				Orb orb = Orbs[num];
				bool flag = false;
				try
				{
					flag = Advance(orb, dt);
				}
				catch (Exception ex)
				{
					DevLog.Warn("BurstFx (waiting spark): " + ex);
				}
				if (!flag)
				{
					Orbs.Remove(orb);
					Drop((CanvasItem)(object)orb.Holder);
				}
			}
			for (int num2 = Ghosts.Count - 1; num2 >= 0; num2--)
			{
				Ghost ghost = Ghosts[num2];
				bool flag2 = false;
				try
				{
					flag2 = Advance(ghost, dt);
				}
				catch (Exception ex2)
				{
					DevLog.Warn("BurstFx (ghost): " + ex2);
				}
				if (!flag2)
				{
					Ghosts.Remove(ghost);
					End(ghost);
				}
			}
		}

		private static bool Advance(Orb orb, float dt)
		{
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)orb.Holder) || !((Node)orb.Holder).IsInsideTree())
			{
				return false;
			}
			if (orb.Remote && !RemoteCurrent(orb))
			{
				orb.Fading = true;
			}
			orb.Age += dt;
			orb.Clock += dt;
			if (orb.Age > 15f)
			{
				orb.Fading = true;
			}
			if (orb.Fading)
			{
				orb.Fade -= dt / 0.25f;
				if (orb.Fade <= 0f)
				{
					return false;
				}
			}
			try
			{
				if (orb.Anchor != null && orb.Anchor.Valid())
				{
					orb.Spot = SpotOf(orb.Anchor);
				}
			}
			catch
			{
			}
			orb.Glide = FxKit.Damp(orb.Glide, 1f, 9f, dt);
			orb.Alpha = FxKit.Damp(orb.Alpha, 1f, 4.5f, dt);
			Lay(orb);
			return true;
		}

		private static void Lay(Orb orb)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			float num = orb.Glide * orb.Glide * (3f - 2f * orb.Glide);
			float num2 = Mathf.Lerp(orb.Off0, 1f, num);
			Vector2 val = orb.From.Lerp(orb.Spot, num);
			float num3 = Mathf.Clamp(Tuning.F("burst.wait_shake", 2.5f), 0f, 20f) * num2;
			orb.Pos = val + Tremble(orb.Clock) * num3;
			float num4 = Mathf.Clamp(orb.Alpha, 0f, 1f) * Mathf.Clamp(orb.Fade, 0f, 1f);
			orb.Trail.Add(val);
			orb.At.Add(orb.Clock);
			float num5 = Tuning.F("burst.trail", 0.3f);
			while (orb.Trail.Count > 2 && (orb.Clock - orb.At[0] > num5 || orb.Trail.Count > 120))
			{
				orb.Trail.RemoveAt(0);
				orb.At.RemoveAt(0);
			}
			if (FxKit.Alive((GodotObject)(object)orb.Ribbon))
			{
				float num6 = 1f - num2 * num2 * num2;
				int count = orb.Trail.Count;
				bool flag = num6 > 0.02f && count > 1;
				if (flag)
				{
					if (orb.Points.Length != count)
					{
						orb.Points = new Vector2[count];
					}
					for (int i = 0; i < count; i++)
					{
						orb.Points[i] = orb.Holder.ToLocal(orb.Trail[i]);
					}
					orb.Ribbon.Points = orb.Points;
				}
				((CanvasItem)orb.Ribbon).Visible = flag;
				((CanvasItem)orb.Ribbon).Modulate = new Color(1f, 1f, 1f, num4 * num6);
			}
			if (FxKit.Alive((GodotObject)(object)orb.Head))
			{
				float num7 = 0.35f * num2;
				((Node2D)orb.Head).GlobalPosition = orb.Pos;
				((CanvasItem)orb.Head).Modulate = new Color(Mathf.Lerp(orb.Tint.R, 1f, num7), Mathf.Lerp(orb.Tint.G, 1f, num7), Mathf.Lerp(orb.Tint.B, 1f, num7), (0.95f + 0.05f * num2) * num4);
				((Node2D)orb.Head).Scale = HeadScale * (1f + 0.22f * num2);
			}
		}

		private static int Pending(CardModel card)
		{
			if (card == null)
			{
				return -1;
			}
			bool flag = MultiplayerFx.Active && !MultiplayerFx.IsLocal(card);
			for (int i = 0; i < Orbs.Count; i++)
			{
				Orb orb = Orbs[i];
				if ((!flag || orb.Remote) && (!orb.Remote || (MultiplayerFx.CurrentInvocation != null && orb.Invocation == MultiplayerFx.CurrentInvocation)) && !orb.Fading && orb.Card == card)
				{
					return i;
				}
			}
			return -1;
		}

		private static bool RemoteCurrent(Orb orb)
		{
			if (MultiplayerFx.Active && FxKit.Alive((GodotObject)(object)GameRefs.CombatRoom) && orb.Room == GameRefs.CombatRoom)
			{
				object obj = orb.Anchor?.Owner;
				Creature val = (Creature)((obj is Creature) ? obj : null);
				if (val != null && val.IsAlive && !MultiplayerFx.IsLocal(val))
				{
					CardModel card = orb.Card;
					object obj2;
					if (card == null)
					{
						obj2 = null;
					}
					else
					{
						Player owner = card.Owner;
						obj2 = ((owner != null) ? owner.Creature : null);
					}
					return obj2 == val;
				}
			}
			return false;
		}

		private static void PopRemote(Orb orb, NCard node)
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			if (!RemoteCurrent(orb) || !Shows(node, orb.Card) || !((CanvasItem)node).IsVisibleInTree())
			{
				orb.Fading = true;
				return;
			}
			Node val = Top(orb.Anchor.FrontParent);
			if (val == null)
			{
				orb.Fading = true;
				return;
			}
			CardStandIn.Pose(node, out var centre, out var _, out var scale);
			Orbs.Remove(orb);
			Drop((CanvasItem)(object)orb.Holder);
			Pop(val, centre, scale, Mathf.Clamp(Tuning.F("burst.pop", 1f), 0f, 4f), 12);
		}

		internal static void OnReplay(NCard node)
		{
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			try
			{
				if (Orbs.Count == 0 || !FxKit.Alive((GodotObject)(object)node))
				{
					return;
				}
				int num = Pending(node.Model);
				if (num < 0)
				{
					return;
				}
				if (Orbs[num].Remote)
				{
					PopRemote(Orbs[num], node);
				}
				else if ((!MultiplayerFx.Active || MultiplayerFx.IsLocal(node.Model)) && !(Tuning.F("burst.ghost", 1f) < 0.5f))
				{
					Color modulate = ((CanvasItem)node).Modulate;
					Vector2 scale = ((Control)node).Scale;
					Vector2 position = ((Control)node).Position;
					Tween playPileTween = node.PlayPileTween;
					if (playPileTween != null && playPileTween.IsValid())
					{
						playPileTween.Kill();
					}
					Orb orb = Orbs[num];
					Orbs.RemoveAt(num);
					try
					{
						((CanvasItem)node).Modulate = modulate;
						((Control)node).Scale = scale;
						((Control)node).Position = position;
						node.UpdateVisuals((PileType)5, (CardPreviewMode)1);
					}
					catch (Exception ex)
					{
						DevLog.Warn("BurstFx.OnReplay (card): " + ex);
					}
					Fire(orb, node);
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("BurstFx.OnReplay: " + ex2);
			}
		}

		private static void Fire(Orb orb, NCard node)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			try
			{
				Node val = (FxKit.Alive((GodotObject)(object)orb.Holder) ? ((Node)orb.Holder).GetParent() : null);
				Node val2 = Top(val);
				float seen = 46f * FogKit.ScaleOf(val);
				flag = val2 != null && Shoot(val2, orb.Pos, orb.Card, node, orb.Tint, seen, withGhost: true);
			}
			catch (Exception ex)
			{
				DevLog.Warn("BurstFx.Fire: " + ex);
			}
			if (flag)
			{
				Drop((CanvasItem)(object)orb.Holder);
				return;
			}
			orb.Fading = true;
			Orbs.Add(orb);
		}

		private static void Bear(Ghost ghost, Node layer, CardModel card, NCard node)
		{
			bool flag = false;
			try
			{
				flag = Make(ghost, layer, card, node);
			}
			catch (Exception ex)
			{
				DevLog.Warn("BurstFx.Bear: " + ex);
			}
			if (!flag)
			{
				Ghosts.Remove(ghost);
				End(ghost);
			}
		}

		private static bool Make(Ghost ghost, Node layer, CardModel card, NCard node)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected Obj, but got Unknown
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			if (ghost.Dropped)
			{
				return false;
			}
			if (!Shows(node, card))
			{
				node = HandCards.NodeOf(card);
			}
			if (!Shows(node, card) || !FxKit.Alive((GodotObject)(object)layer) || !layer.IsInsideTree())
			{
				return false;
			}
			CardStandIn.Pose(node, out var centre, out var rotation, out var scale);
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_BurstGhost")
			};
			layer.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return false;
			}
			ghost.Holder = val;
			ghost.Copy = CardStandIn.Make(card, (Node)(object)val);
			if (!FxKit.Alive((GodotObject)(object)ghost.Copy))
			{
				ghost.Copy = null;
				return false;
			}
			ghost.Centre = centre;
			ghost.Rotation = rotation;
			ghost.Scale = scale;
			ghost.Wide = 300f * Mathf.Clamp(scale.X, 0.2f, 1.5f);
			ghost.Tall = 422f * Mathf.Clamp(scale.Y, 0.2f, 1.5f);
			ghost.Alpha = Mathf.Clamp(Tuning.F("burst.ghost_alpha", 0.72f), 0.05f, 1f);
			ghost.Grow = Mathf.Clamp(Tuning.F("burst.ghost_grow", 1.14f), 1f, 2f);
			ghost.Rise = Mathf.Clamp(Tuning.F("burst.ghost_rise", 0.1f), 0f, 0.6f);
			ghost.Hold = Mathf.Clamp(Tuning.F("burst.ghost_hold", 0.9f), 0.23f, 6f);
			ghost.Age = 0f;
			ghost.Born = true;
			FxLifetime.Watch((Node)(object)val, ghost.Hold + 0.06f + 0.3f + 2f, () =>
			{
				Release(ghost);
			});
			Draw(ghost);
			Wake();
			return true;
		}

		private static bool Advance(Ghost ghost, float dt)
		{
			ghost.Age += dt;
			if (!ghost.Born)
			{
				if (!ghost.Dropped)
				{
					return ghost.Age < 3f;
				}
				return false;
			}
			if (!FxKit.Alive((GodotObject)(object)ghost.Holder) || !((Node)ghost.Holder).IsInsideTree() || !FxKit.Alive((GodotObject)(object)ghost.Copy))
			{
				return false;
			}
			if (ghost.EndAt < 0f)
			{
				if (ghost.Called)
				{
					ghost.Punch = true;
					ghost.PunchAt = Mathf.Max(ghost.Age, 0.18f);
					ghost.EndAt = ghost.PunchAt + 0.06f;
				}
				else if (ghost.Age >= ghost.Hold)
				{
					ghost.EndAt = ghost.Age;
				}
			}
			if (ghost.EndAt >= 0f && ghost.Age >= ghost.EndAt + 0.3f)
			{
				return false;
			}
			if (ghost.Punch && !ghost.Punched && ghost.Age >= ghost.PunchAt)
			{
				ghost.Punched = true;
				Strike(ghost);
			}
			Draw(ghost);
			return true;
		}

		private static float Over(float u)
		{
			float num = Mathf.Clamp(u, 0f, 1f) - 1f;
			return 1f + num * num * (2.4f * num + 1.4f);
		}

		private static void Draw(Ghost ghost)
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)ghost.Copy))
			{
				float age = ghost.Age;
				float num = Mathf.Clamp(age / 0.18f, 0f, 1f);
				float num2 = 1f - (1f - num) * (1f - num);
				float num3 = 1f + (ghost.Grow - 1f) * Over(num);
				float num4 = ghost.Alpha * num2;
				Vector2 val = new Vector2((0f - ghost.Wide) * 0.04f, (0f - ghost.Tall) * ghost.Rise) * num2;
				float num5 = -0.052f * num2;
				float num6 = ((ghost.EndAt < 0f) ? age : Mathf.Min(age, ghost.Punch ? ghost.PunchAt : ghost.EndAt));
				val.Y += 3f * Mathf.Sin(13.823009f * Mathf.Max(0f, num6 - 0.18f));
				float num7 = 0f;
				if (ghost.Punch)
				{
					float num8 = Mathf.Clamp((age - ghost.PunchAt) / 0.06f, 0f, 1f);
					num7 = 0.1f * (1f - (1f - num8) * (1f - num8));
				}
				if (ghost.EndAt >= 0f && age > ghost.EndAt)
				{
					float num9 = Mathf.Clamp((age - ghost.EndAt) / 0.3f, 0f, 1f);
					float num10 = 1f - (1f - num9) * (1f - num9);
					num7 = Mathf.Lerp(num7, 0.2f, num10);
					num4 *= 1f - num9;
					val.Y -= ghost.Tall * 0.12f * num10;
				}
				CardStandIn.Place(ghost.Copy, ghost.Centre + val, ghost.Rotation + num5, ghost.Scale * (num3 + num7));
				((CanvasItem)ghost.Copy).Modulate = new Color(GhostGold.R, GhostGold.G, GhostGold.B, Mathf.Clamp(num4, 0f, 1f));
			}
		}

		private static void Strike(Ghost ghost)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			Node parent = ((Node)ghost.Holder).GetParent();
			float num = 1f / FogKit.ScaleOf(parent);
			float num2 = ghost.Tall / 422f;
			Vector2 val = ghost.Centre + new Vector2((0f - ghost.Wide) * 0.04f, (0f - ghost.Tall) * ghost.Rise);
			FxKit.Flash((Node)(object)ghost.Holder, val, new Color(1f, 1f, 1f, 0.7f), ghost.Tall * 0.85f * num, 1.35f, 0.14);
			FxKit.Burst(parent, val, new Color(1f, 0.86f, 0.45f, 0.95f), 10, 200f * num2 * num, 560f * num2 * num, 0.36, 0.08f * num2 * num, 0.2f * num2 * num, Vector2.Zero);
		}

		private static void Release(Ghost ghost)
		{
			NCard copy = ghost.Copy;
			if (copy != null)
			{
				ghost.Copy = null;
				CardStandIn.Free(copy);
			}
		}

		private static void End(Ghost ghost)
		{
			try
			{
				Release(ghost);
				if (FxKit.Alive((GodotObject)(object)ghost.Holder))
				{
					((Node)ghost.Holder).QueueFree();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BurstFx (ghost, free): " + ex);
			}
		}

		internal static void NotePlay(CardModel card, int index)
		{
			try
			{
				if (card != null && MultiplayerFx.Active && !MultiplayerFx.IsLocal(card))
				{
					foreach (BurstFx item in Live)
					{
						if (!item.Dead && item.RemoteOwner)
						{
							object owner = item.Anchor.Owner;
							Player owner2 = card.Owner;
							if (owner == ((owner2 != null) ? owner2.Creature : null))
							{
								item.RemotePlayed(card);
							}
						}
					}
					if (index >= 1)
					{
						foreach (Orb orb in Orbs)
						{
							if (orb.Remote && orb.Card == card && orb.Invocation == MultiplayerFx.CurrentInvocation)
							{
								orb.Fading = true;
							}
						}
					}
				}
				if (card == null || index < 1)
				{
					return;
				}
				for (int i = 0; i < Ghosts.Count; i++)
				{
					Ghost ghost = Ghosts[i];
					if (!ghost.Called && ghost.Card == card)
					{
						ghost.Called = true;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BurstFx.NotePlay: " + ex);
			}
		}

		internal static void NoteFinished(CardModel card, int index, int count)
		{
			try
			{
				if (card == null || index < count - 1)
				{
					return;
				}
				for (int i = 0; i < Orbs.Count; i++)
				{
					if (Orbs[i].Card == card && (!Orbs[i].Remote || Orbs[i].Invocation == MultiplayerFx.CurrentInvocation))
					{
						Orbs[i].Fading = true;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BurstFx.NoteFinished: " + ex);
			}
		}

		internal static void Abandon()
		{
			try
			{
				for (int i = 0; i < Orbs.Count; i++)
				{
					Orbs[i].Fading = true;
				}
				for (int j = 0; j < Ghosts.Count; j++)
				{
					Ghost ghost = Ghosts[j];
					if (!ghost.Born)
					{
						ghost.Dropped = true;
					}
					else if (ghost.EndAt < 0f)
					{
						ghost.EndAt = ghost.Age;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BurstFx.Abandon: " + ex);
			}
		}

		internal static void Reset()
		{
			try
			{
				_noted = null;
				for (int num = Orbs.Count - 1; num >= 0; num--)
				{
					Drop((CanvasItem)(object)Orbs[num].Holder);
				}
				Orbs.Clear();
				for (int num2 = Ghosts.Count - 1; num2 >= 0; num2--)
				{
					End(Ghosts[num2]);
				}
				Ghosts.Clear();
			}
			catch (Exception ex)
			{
				DevLog.Warn("BurstFx.Reset: " + ex);
			}
		}

		private void Flourish()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			Node frontParent = Anchor.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)frontParent))
			{
				return;
			}
			Vector2 val = Centre();
			Node2D ring = FxKit.Sigil(280f, Gold, out var mat);
			frontParent.AddChild((Node)(object)ring, false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)ring, 0.550000011920929);
			ring.GlobalPosition = val;
			if (mat != null)
			{
				mat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.8f));
				mat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.05f));
				mat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.5f));
			}
			FxKit.Drive((Node)(object)ring, 0.45f, (float t) =>
			{
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)ring))
				{
					float num = 0.3f + 2.2f * t;
					ring.Scale = new Vector2(num, num);
					if (FxKit.Alive((GodotObject)(object)mat))
					{
						mat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(1f - t));
					}
				}
			}, (TransitionType)4, (EaseType)1).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)ring))
				{
					((Node)ring).QueueFree();
				}
			})));
			FxKit.Burst(frontParent, val, new Color(1f, 0.86f, 0.45f, 0.95f), 26, 160f, 420f, 0.5, 0.1f, 0.28f, Vector2.Zero);
		}

		protected override void OnDispose()
		{
			if (_remoteMode)
			{
				ClearRemote();
			}
			Live.Remove(this);
		}

		static BurstFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		}
	}
