// namespace: SilentFX.Powers
// type: MasterPlannerFx (class)
// source lines: 7275-9327
	public sealed class MasterPlannerFx : BasicPowerFx
	{
		private sealed class Signature
		{
			public Node2D Holder;

			public Line2D Line;

			public Line2D Halo;

			public Line2D Glint;

			public NCard Follow;

			public CardModel Card;

			public Vector2 At;

			public float Scale = 1f;

			public float Written;

			public float Shown = -1f;

			public bool Wait;

			public float Age = -1f;

			public float Gone = -1f;

			public bool Mark;
		}

		private enum Stage
		{
			Waiting,
			Writing,
			Signed
		}

		private sealed class Pen
		{
			public CardModel Card;

			public NCard Node;

			public bool Locked;

			public bool Began;

			public Node2D Root;

			public Node2D Quill;

			public Vector2 At;

			public float Rot;

			public float Scale = 1f;

			public bool Placed;

			public Stage Stage;

			public float T;

			public float Age;

			public float In;

			public bool Wanted = true;

			public Signature Ink;

			public CardFlight.Plan Plan;

			public bool InPlay;

			public float Out = -1f;

			public bool Off;

			public float Lost;

			public bool Heard;

			public float Phase;

			public bool Demo;
		}

		private sealed class Payoff
		{
			public CardModel Card;

			public Signature Ink;

			public float Wait;

			public float T;

			public bool Sparked;

			public bool Demo;

			public Vector2 At;

			public float Scale = 1f;
		}

		private static readonly Color Gold = new Color(0.98f, 0.84f, 0.45f, 1f);

		private static readonly Color Bright = new Color(1f, 0.95f, 0.76f, 1f);

		private static readonly Color Feather = new Color(1f, 0.97f, 0.86f, 0.94f);

		private static readonly Color Nib = new Color(0.56f, 0.4f, 0.16f, 1f);

		private const int MaxMarks = 8;

		private const int Steps = 72;

		private static readonly HashSet<CardModel> Tagged = new HashSet<CardModel>(ReferenceEqualityComparer.Instance);

		private static readonly List<MasterPlannerFx> Live = new List<MasterPlannerFx>();

		private static bool _hooked;

		private static Vector2[] _path;

		private static float[] _run;

		private static float _pathKey = float.NaN;

		private Node2D _root;

		private Node2D _quill;

		private Sprite2D _glow;

		private Node2D _marks;

		private readonly List<Line2D> _tally = new List<Line2D>();

		private readonly List<Signature> _signatures = new List<Signature>();

		private readonly List<Pen> _pens = new List<Pen>();

		private readonly List<Payoff> _payoffs = new List<Payoff>();

		private float _length;

		private int _count;

		private int _previewCount;

		private float _recount;

		private float _jump = 9f;

		private float _openDelay;

		private bool _opened;

		private float _pop = 9f;

		private float _ping = 9f;

		private float _shine = 9f;

		private float _alert;

		private float _seen = 1f;

		private bool _demoSecond;

		private CardModel _carried;

		private bool _carriedCounts;

		private CardModel _remoteCard;

		private readonly HashSet<CardModel> _remotePlayed = new HashSet<CardModel>(ReferenceEqualityComparer.Instance);

		private object _remoteRoom;

		private bool _remoteMode;

		private bool _remoteCounts;

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

		protected override string NodeName => "MasterPlanner";

		protected override bool UseFront => false;

		protected override float KickDecay => 3f;

		private static float InkY => Tuning.F("planner.ink_y", 108f);

		private static float WriteTime => Mathf.Max(0.06f, Tuning.F("planner.write", 0.9f));

		private void ClearRemote()
		{
			_remoteCard = null;
			_remotePlayed.Clear();
			_remoteRoom = null;
			_remoteMode = (_remoteCounts = false);
			_carried = null;
			_carriedCounts = false;
			_alert = 0f;
			_seen = 1f;
		}

		private void EnterRemote()
		{
			if (!_remoteMode || _remoteRoom != GameRefs.CombatRoom)
			{
				ClearRemote();
				for (int num = _pens.Count - 1; num >= 0; num--)
				{
					Drop(_pens[num]);
				}
				_payoffs.Clear();
				for (int num2 = _signatures.Count - 1; num2 >= 0; num2--)
				{
					FreeSignature(_signatures[num2]);
				}
			}
			_remoteMode = true;
			_remoteRoom = GameRefs.CombatRoom;
		}

		private bool ReadRemote()
		{
			EnterRemote();
			object owner = Anchor.Owner;
			if (!RemoteHeldFx.TryObserve((Creature)((owner is Creature) ? owner : null), out var card, out var _, out var released))
			{
				_remoteCard = null;
				_remoteCounts = false;
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
			if (_remotePlayed.Contains(card))
			{
				_remoteCard = null;
				_remoteCounts = false;
				return false;
			}
			if (card != _remoteCard)
			{
				_remoteCard = card;
				try
				{
					_remoteCounts = WouldSeal(card, Anchor.Owner) && card.CanPlay();
				}
				catch
				{
					_remoteCounts = false;
				}
			}
			if (!Stopping)
			{
				return _remoteCounts;
			}
			return false;
		}

		private void ConsumeRemote(CardModel card)
		{
			EnterRemote();
			Player owner = card.Owner;
			if (((owner != null) ? owner.Creature : null) == Anchor.Owner)
			{
				_remotePlayed.Add(card);
				if (_remoteCard == card)
				{
					_remoteCard = null;
					_remoteCounts = false;
				}
			}
		}

		public static void Tag(CardModel card)
		{
			if (card != null)
			{
				Tagged.Add(card);
			}
		}

		public static bool IsTagged(CardModel card)
		{
			if (card != null)
			{
				return Tagged.Contains(card);
			}
			return false;
		}

		public static void Reset()
		{
			Tagged.Clear();
		}

		private static MasterPlannerFx For(Creature owner)
		{
			if (owner == null)
			{
				return null;
			}
			foreach (MasterPlannerFx item in Live)
			{
				if (!item.Dead && !item.Stopping && item.Anchor?.Owner == owner)
				{
					return item;
				}
			}
			return null;
		}

		private static bool WouldSeal(CardModel card, object owner)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			if (card == null || owner == null)
			{
				return false;
			}
			try
			{
				if ((int)card.Type != 2 || Tagged.Contains(card))
				{
					return false;
				}
				Player owner2 = card.Owner;
				if (((owner2 != null) ? owner2.Creature : null) != owner)
				{
					return false;
				}
				return card.Keywords == null || !card.Keywords.Contains((CardKeyword)6);
			}
			catch
			{
				return false;
			}
		}

		public static void OnCardPlayed(CardPlay play)
		{
			CardModel val = ((play != null) ? play.Card : null);
			object obj;
			if (val == null)
			{
				obj = null;
			}
			else
			{
				Player owner = val.Owner;
				obj = ((owner != null) ? owner.Creature : null);
			}
			Creature val2 = (Creature)obj;
			if (val2 != null && WouldSeal(val, val2) && val2.HasPower<MasterPlannerPower>())
			{
				Tag(val);
				Sealed(val2, val);
			}
		}

		public static void Sealed(Creature owner, CardModel card)
		{
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			MasterPlannerFx fx = For(owner);
			if (fx != null)
			{
				if (PowerFxManager.LogTriggers)
				{
					DevLog.Info("Master Planner: sealed " + ((object)card)?.GetType().Name);
				}
				Action action = () =>
				{
					fx.Seal(card);
				};
				if (!TimeFreeze.Defer(action))
				{
					action();
				}
			}
		}

		public static void Discarded(Creature owner, CardModel card)
		{
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			MasterPlannerFx fx = For(owner);
			if (fx != null)
			{
				Action action = () =>
				{
					fx.Flinch();
				};
				if (!TimeFreeze.Defer(action))
				{
					action();
				}
			}
		}

		public static void PaidOff(Creature owner, CardModel card)
		{
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			MasterPlannerFx fx = For(owner);
			if (fx != null)
			{
				if (PowerFxManager.LogTriggers)
				{
					DevLog.Info("Master Planner: paid off with " + ((object)card)?.GetType().Name);
				}
				Action action = () =>
				{
					fx.BeginPayoff(card);
				};
				if (!TimeFreeze.Defer(action))
				{
					action();
				}
			}
		}

		private static void OnLetGo(CardModel card, bool played)
		{
			try
			{
				object owner;
				if (card == null)
				{
					owner = null;
				}
				else
				{
					Player owner2 = card.Owner;
					owner = ((owner2 != null) ? owner2.Creature : null);
				}
				For((Creature)owner)?.LetGo(card, played);
			}
			catch (Exception ex)
			{
				DevLog.Warn("Master Planner (let go): " + ex.Message);
			}
		}

		private static void LateAll(double realDt)
		{
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				MasterPlannerFx masterPlannerFx = Live[num];
				if (!masterPlannerFx.Dead)
				{
					try
					{
						masterPlannerFx.Follow();
					}
					catch (Exception ex)
					{
						DevLog.Warn("Master Planner (follow): " + ex.Message);
					}
				}
			}
		}

		private static NCard Find(CardModel card)
		{
			if (card == null)
			{
				return null;
			}
			try
			{
				NCard val = NCard.FindOnTable(card, (PileType?)null);
				return (FxKit.Alive((GodotObject)(object)val) && ((Node)val).IsInsideTree()) ? val : null;
			}
			catch
			{
				return null;
			}
		}

		private static bool Where(NCard node, CardModel card, out Vector2 at, out float turn, out float scale)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			at = default;
			turn = 0f;
			scale = 1f;
			try
			{
				if (!FxKit.Alive((GodotObject)(object)node) || !((Node)node).IsInsideTree() || !((CanvasItem)node).IsVisibleInTree())
				{
					return false;
				}
				if (card != null && node.Model != null && node.Model != card)
				{
					return false;
				}
				Transform2D globalTransform = ((CanvasItem)node).GetGlobalTransform();
				at = globalTransform.Origin;
				turn = globalTransform.Rotation;
				float num = globalTransform.Scale.X;
				if (FxKit.Alive((GodotObject)(object)node.Body))
				{
					num *= node.Body.Scale.X;
				}
				scale = Mathf.Clamp(Mathf.Abs(num), 0.02f, 1.3f);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static Vector2[] Path()
		{
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			float inkY = InkY;
			float num = Tuning.F("planner.ink_w", 216f);
			float num2 = Tuning.F("planner.ink_amp", 44f);
			float num3 = Tuning.F("planner.ink_waves", 3.25f);
			float num4 = Tuning.F("planner.ink_decay", 3.4f);
			float num5 = Tuning.F("planner.ink_rise", 10f);
			float num6 = inkY + 3.1f * num + 7.3f * num2 + 13.7f * num3 + 29.9f * num4 + 0.71f * num5;
			if (_path != null && num6 == _pathKey)
			{
				return _path;
			}
			Vector2[] array = new Vector2[73];
			for (int i = 0; i <= 72; i++)
			{
				float num7 = (float)i / 72f;
				array[i] = new Vector2(-0.5f * num + num * num7, inkY + num5 * (0.5f - num7) - num2 * Mathf.Exp((0f - num4) * num7) * Mathf.Sin((float)Math.PI * 2f * num3 * num7));
			}
			float[] array2 = new float[73];
			for (int j = 1; j <= 72; j++)
			{
				array2[j] = array2[j - 1] + array[j].DistanceTo(array[j - 1]);
			}
			_path = array;
			_run = array2;
			_pathKey = num6;
			return array;
		}

		private static float Along(float time)
		{
			Vector2[] array = Path();
			float[] run = _run;
			float num = Mathf.Clamp(time, 0f, 1f);
			if (run == null || run.Length != array.Length || run[^1] < 0.01f)
			{
				return num;
			}
			float num2 = (0.5f * num + 0.5f * num * num * (3f - 2f * num)) * run[^1];
			int num3 = run.Length - 1;
			for (int i = 1; i <= num3; i++)
			{
				if (!(run[i] < num2))
				{
					float num4 = run[i] - run[i - 1];
					return ((float)(i - 1) + ((num4 > 0.0001f) ? ((num2 - run[i - 1]) / num4) : 0f)) / (float)num3;
				}
			}
			return 1f;
		}

		private static Vector2 PathAt(float written)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			Vector2[] array = Path();
			float num = Mathf.Clamp(written, 0f, 1f) * (float)(array.Length - 1);
			int num2 = Mathf.Min((int)num, array.Length - 2);
			return array[num2].Lerp(array[num2 + 1], num - (float)num2);
		}

		private static float SlopeAt(float written)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = PathAt(written + 0.012f) - PathAt(written - 0.012f);
			float num = val.Length();
			if (!(num < 0.001f))
			{
				return Mathf.Clamp(val.Y / num, -1f, 1f);
			}
			return 0f;
		}

		private static Vector2[] Slice(Vector2[] path, float from, float to)
		{
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			int num = path.Length - 1;
			float num2 = Mathf.Clamp(from, 0f, 1f) * (float)num;
			float num3 = Mathf.Clamp(to, 0f, 1f) * (float)num;
			if (num3 - num2 < 0.01f)
			{
				num3 = Mathf.Min((float)num, num2 + 0.01f);
			}
			int num4 = Mathf.Min((int)num2 + 1, num);
			int num5 = Mathf.Min((int)num3, num);
			List<Vector2> list = new List<Vector2>(Mathf.Max(2, num5 - num4 + 3));
			int num6 = Mathf.Min((int)num2, num - 1);
			list.Add(path[num6].Lerp(path[num6 + 1], num2 - (float)num6));
			for (int i = num4; i <= num5; i++)
			{
				if ((float)i > num2 && (float)i < num3)
				{
					list.Add(path[i]);
				}
			}
			int num7 = Mathf.Min((int)num3, num - 1);
			list.Add(path[num7].Lerp(path[num7 + 1], num3 - (float)num7));
			return list.ToArray();
		}

		private Node Layer()
		{
			Node uiLayer = GameRefs.UiLayer;
			if (!FxKit.Alive((GodotObject)(object)uiLayer))
			{
				return Anchor.FrontParent;
			}
			return uiLayer;
		}

		protected override void Build(bool animateIn)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Expected Obj, but got Unknown
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Expected Obj, but got Unknown
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Expected Obj, but got Unknown
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			Live.Add(this);
			if (!_hooked)
			{
				_hooked = true;
				HeldFxManager.LetGo += OnLetGo;
				Ticker.AddLate(LateAll);
			}
			FlightHold.Init();
			Vector2 val = Anchor.SafeBodySize();
			_length = Mathf.Clamp(val.X * Tuning.F("planner.size_mul", 0.46f), 70f, 140f);
			float length = _length;
			_marks = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_PlannerTally")
			};
			((Node)Back).AddChild((Node)(object)_marks, false, (InternalMode)0);
			for (int i = 0; i < 8; i++)
			{
				Line2D val2 = new Line2D();
				val2.Width = 2.6f;
				val2.DefaultColor = new Color(Bright.R, Bright.G, Bright.B, 0.95f);
				val2.BeginCapMode = (LineCapMode)2;
				val2.EndCapMode = (LineCapMode)2;
				((CanvasItem)val2).Material = (Material)(object)FxKit.Additive;
				val2.Antialiased = true;
				val2.Points = new Vector2[2]
				{
					new Vector2(-2f, 6f),
					new Vector2(2f, -6f)
				};
				((CanvasItem)val2).Visible = false;
				Line2D val3 = val2;
				((Node)_marks).AddChild((Node)(object)val3, false, (InternalMode)0);
				_tally.Add(val3);
			}
			_root = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_PlannerQuill")
			};
			((Node)Back).AddChild((Node)(object)_root, false, (InternalMode)0);
			_quill = Props.Quill(length, Feather, Gold, Nib, new Color(0f, 0f, 0f, 0f));
			((Node)_root).AddChild((Node)(object)_quill, false, (InternalMode)0);
			_glow = FxKit.Glow(new Color(Gold.R, Gold.G, Gold.B, 0.16f), new Vector2(length * 0.9f, length * 1.4f));
			((Node2D)_glow).Position = new Vector2(0f, (0f - length) * 0.58f);
			((Node)_quill).AddChild((Node)(object)_glow, false, (InternalMode)0);
			((Node)_quill).MoveChild((Node)(object)_glow, 0);
			_count = Count();
			_openDelay = ((animateIn && Tuning.F("intro.on", 1f) > 0.5f) ? Tuning.F("planner.open_delay", 0.74f) : 0f);
		}

		private int Count()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Invalid comparison between Unknown and I4
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Invalid comparison between Unknown and I4
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Invalid comparison between Unknown and I4
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Invalid comparison between Unknown and I4
			if (Anchor.Owner == null)
			{
				return _previewCount;
			}
			int num = 0;
			try
			{
				foreach (CardModel item in Tagged)
				{
					Player owner = item.Owner;
					if (((owner != null) ? owner.Creature : null) == Anchor.Owner)
					{
						CardPile pile = item.Pile;
						PileType? val = ((pile != null) ? new PileType?(pile.Type) : ((PileType?)null));
						if ((int)val.GetValueOrDefault() == 1 || (int)val.GetValueOrDefault() == 2 || (int)val.GetValueOrDefault() == 3 || (int)val.GetValueOrDefault() == 5)
						{
							num++;
						}
					}
				}
				return num;
			}
			catch
			{
				return _count;
			}
		}

		private Vector2 SealPoint()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = Anchor.SafeBodySize();
			return Anchor.Chest() + new Vector2(val.X * Tuning.F("planner.x", 0.04f) * Anchor.Side(), (0f - val.Y) * Tuning.F("planner.y", 0.56f));
		}

		private Vector2 TablePoint()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = FxKit.ViewSize();
			Vector2 val2 = new Vector2(val.X * 0.5f, val.Y * 0.407f);
			if (Anchor.Owner == null)
			{
				return val2;
			}
			try
			{
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				object obj;
				if (combatRoom == null)
				{
					obj = null;
				}
				else
				{
					NCombatUi ui = combatRoom.Ui;
					obj = ((ui != null) ? ui.PlayContainer : null);
				}
				Control val3 = (Control)obj;
				if (!FxKit.Alive((GodotObject)(object)val3) || !((Node)val3).IsInsideTree())
				{
					return val2;
				}
				Vector2 val4 = val3.GlobalPosition + val3.Size * 0.5f + new Vector2(0f, -100f);
				return (val4.X > 0f && val4.X < val.X && val4.Y > 0f && val4.Y < val.Y) ? val4 : val2;
			}
			catch
			{
				return val2;
			}
		}

		protected override void Tick(float dt)
		{
			if (_openDelay > 0f && !_opened && Time >= _openDelay)
			{
				_opened = true;
				_jump = 0f;
			}
			_jump += dt;
			_pop += dt;
			_ping += dt;
			_shine += dt;
			_recount -= dt;
			if (_recount <= 0f)
			{
				_recount = 0.3f;
				int num = Count();
				if (num > _count)
				{
					_pop = 0f;
				}
				_count = num;
			}
			if (RemoteOwner)
			{
				bool flag = ReadRemote();
				_alert = 0f;
				float num2 = Mathf.Max(0.02f, Tuning.F("planner.blink", 0.09f));
				_seen = (flag ? Mathf.Max(0f, _seen - dt / num2) : Mathf.Min(1f, _seen + dt / Mathf.Max(num2, Tuning.F("planner.return", 0.26f))));
				return;
			}
			if (_remoteMode)
			{
				ClearRemote();
			}
			bool flag2 = false;
			CardModel val = null;
			if (Anchor.Owner != null && !Stopping)
			{
				try
				{
					CardModel carried = HeldFxManager.Carried;
					if (carried != _carried)
					{
						_carried = carried;
						_carriedCounts = WouldSeal(carried, Anchor.Owner) && carried.CanPlay();
					}
					if (carried != null)
					{
						if (_carriedCounts)
						{
							if (HeldFxManager.CarriedOut)
							{
								val = carried;
							}
							else
							{
								flag2 = true;
							}
						}
					}
					else
					{
						flag2 = WouldSeal(HeldHooks.Hovered(), Anchor.Owner);
					}
				}
				catch
				{
					flag2 = false;
					val = null;
				}
			}
			if (val != null && PenOf(val) == null)
			{
				NewPen(val);
			}
			_alert = FxKit.Damp(_alert, flag2 ? 1f : 0f, 7f, dt);
			StepPens(dt, val);
			StepPayoffs(dt);
			StepSignatures(dt);
			bool flag3 = false;
			foreach (Pen pen in _pens)
			{
				if (pen.Stage != Stage.Signed && (pen.Stage != Stage.Waiting || pen.Wanted || !(pen.In <= 0.05f)) && (pen.Stage != Stage.Writing || pen.Placed))
				{
					flag3 = true;
					break;
				}
			}
			float num3 = Mathf.Max(0.02f, Tuning.F("planner.blink", 0.09f));
			if (flag3)
			{
				_seen = Mathf.Max(0f, _seen - dt / num3);
			}
			else
			{
				_seen = Mathf.Min(1f, _seen + dt / Mathf.Max(num3, Tuning.F("planner.return", 0.26f)));
			}
		}

		protected override void Layout()
		{
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)_root) || !FxKit.Alive((GodotObject)(object)_quill))
			{
				return;
			}
			float num = ((_jump < 0.4f) ? Mathf.Sin((float)Math.PI * _jump / 0.4f) : 0f);
			float num2 = ((_ping < 0.35f) ? (1f - _ping / 0.35f) : 0f);
			float num3 = ((_shine < 0.36f) ? (1f - _shine / 0.36f) : 0f);
			num3 *= num3;
			Vector2 val = SealPoint() + new Vector2(0f, Mathf.Sin(Time * 1.2f) * 3f - 20f * num - 9f * _alert);
			val += new Vector2(Mathf.Sin(Time * 47f) * 2.2f, Mathf.Cos(Time * 39f) * 1.8f) * (1.6f * num2);
			_root.GlobalPosition = val;
			float num4 = 1f + 0.35f * num + 0.08f * Kick + 0.1f * _alert + 0.18f * num2 + 0.16f * num3;
			float num5 = Anchor.Side();
			_root.Rotation = 0f;
			_root.Scale = Vector2.One;
			_root.GlobalScale = new Vector2(num4, num4);
			float num6 = Mathf.Lerp(Tuning.F("planner.alpha", 0.85f), 1f, Mathf.Max(_alert, Mathf.Max(num, num3)));
			float num7 = ((_openDelay <= 0f) ? 1f : Mathf.Clamp((Time - _openDelay) / 0.1f, 0f, 1f));
			((CanvasItem)_root).Modulate = new Color(1f, 1f, 1f, A * num6 * Mathf.Clamp(_seen, 0f, 1f) * num7);
			float num8 = Mathf.Lerp(0.42f, Tuning.F("planner.lean", 0.62f), _alert) + 0.05f * Mathf.Sin(Time * 1.7f);
			num8 -= 0.5f * num2 * Mathf.Sin(_ping * 40f);
			_quill.Scale = new Vector2(num5, 1f);
			_quill.Rotation = num8 * num5;
			if (FxKit.Alive((GodotObject)(object)_glow))
			{
				float num9 = _alert * (0.75f + 0.25f * Mathf.Sin(Time * 3.2f));
				((CanvasItem)_glow).Modulate = new Color(Gold.R, Gold.G, Gold.B, Mathf.Min(0.9f, 0.14f + 0.3f * num + 0.1f * Kick + 0.34f * num9 + 0.3f * num2 + 0.6f * num3));
			}
			if (!FxKit.Alive((GodotObject)(object)_marks))
			{
				return;
			}
			int num10 = Mathf.Min(_count, 8);
			_marks.GlobalPosition = SealPoint() + new Vector2(((0f - _length) * 0.22f - (float)num10 * 7f) * Anchor.Side(), 4f);
			_marks.Scale = new Vector2(Anchor.Side(), 1f);
			((CanvasItem)_marks).Modulate = new Color(1f, 1f, 1f, A * 0.9f * num7);
			for (int i = 0; i < _tally.Count; i++)
			{
				Line2D val2 = _tally[i];
				if (FxKit.Alive((GodotObject)(object)val2))
				{
					((CanvasItem)val2).Visible = i < num10;
					if (((CanvasItem)val2).Visible)
					{
						((Node2D)val2).Position = new Vector2((float)i * 7f, 0f);
						float num11 = ((i == num10 - 1 && _pop < 0.45f) ? (1.2f * (1f - _pop / 0.45f)) : 0f);
						float num12 = 1f + num11 + 0.5f * num + 0.4f * num2 + 0.4f * num3;
						((Node2D)val2).Scale = new Vector2(num12 * Anchor.Side(), num12);
					}
				}
			}
		}

		private Pen PenOf(CardModel card)
		{
			if (card == null)
			{
				return null;
			}
			foreach (Pen pen in _pens)
			{
				if (!pen.Demo && pen.Card == card)
				{
					return pen;
				}
			}
			return null;
		}

		private Pen NewPen(CardModel card)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected Obj, but got Unknown
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			if (Dead)
			{
				return null;
			}
			Node val = Layer();
			if (!FxKit.Alive((GodotObject)(object)val))
			{
				return null;
			}
			Node2D val2 = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_PlannerPen"),
				Modulate = new Color(1f, 1f, 1f, 0f),
				Visible = false
			};
			val.AddChild((Node)(object)val2, false, (InternalMode)0);
			if (!((Node)val2).IsInsideTree())
			{
				((Node)val2).QueueFree();
				return null;
			}
			Node2D val3 = Props.Quill(_length, Feather, Gold, Nib, new Color(Gold.R, Gold.G, Gold.B, 0.3f));
			((Node)val2).AddChild((Node)(object)val3, false, (InternalMode)0);
			Pen pen = new Pen
			{
				Card = card,
				Root = val2,
				Quill = val3,
				Phase = (float)_pens.Count * 2.1f + Time
			};
			_pens.Add(pen);
			return pen;
		}

		private void Seal(CardModel card)
		{
			if (Dead || Stopping || card == null)
			{
				return;
			}
			if (RemoteOwner)
			{
				ConsumeRemote(card);
				return;
			}
			Pen pen = PenOf(card) ?? NewPen(card);
			if (pen != null && pen.Stage == Stage.Waiting)
			{
				Write(pen);
			}
		}

		private void LetGo(CardModel card, bool played)
		{
			if (Dead || Stopping || card == null || RemoteOwner)
			{
				return;
			}
			Pen pen = PenOf(card);
			if (!played || TimeFreeze.Active)
			{
				return;
			}
			if (pen == null)
			{
				if (!WouldSeal(card, Anchor.Owner))
				{
					return;
				}
				pen = NewPen(card);
				if (pen == null)
				{
					return;
				}
			}
			if (pen.Stage == Stage.Waiting)
			{
				Write(pen);
			}
		}

		private static void Refresh(Pen pen)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			if (pen.Demo)
			{
				return;
			}
			if (!pen.Locked)
			{
				NCard val = Find(pen.Card);
				if (val != null)
				{
					pen.Node = val;
				}
			}
			if (Where(pen.Node, pen.Card, out var at, out var turn, out var scale))
			{
				pen.At = at;
				pen.Rot = turn;
				pen.Scale = scale;
				pen.Placed = true;
				if (pen.Ink != null && !pen.Locked)
				{
					pen.Ink.Follow = pen.Node;
				}
			}
		}

		private void Write(Pen pen)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			pen.Stage = Stage.Writing;
			pen.T = 0f;
			pen.Wanted = true;
			Refresh(pen);
			Node val = Layer();
			if (FxKit.Alive((GodotObject)(object)val))
			{
				Signature signature = MakeSignature(val, Gold);
				if (signature != null)
				{
					signature.Card = pen.Card;
					signature.Follow = pen.Node;
					signature.At = pen.At;
					signature.Scale = pen.Scale;
					signature.Written = 0f;
					pen.Ink = signature;
					_signatures.Add(signature);
					if (FxKit.Alive((GodotObject)(object)pen.Root) && ((Node)pen.Root).GetParent() == val)
					{
						val.MoveChild((Node)(object)pen.Root, -1);
					}
				}
			}
			if (pen.Demo || pen.Card == null || Tuning.F("planner.hold", 1f) < 0.5f || CardFlight.Claimed(pen.Card))
			{
				return;
			}
			CardFlight.Plan plan = new CardFlight.Plan
			{
				HoldMax = Mathf.Clamp(Tuning.F("planner.hold_max", 1.2f), 0f, 3f),
				Hold = () => !Dead && pen.Stage == Stage.Writing && _pens.Contains(pen),
				Started = (NCard node, Vector2 _, Vector2 _) =>
				{
					pen.Began = true;
					if (FxKit.Alive((GodotObject)(object)node))
					{
						pen.Node = node;
						pen.Locked = true;
						if (pen.Ink != null)
						{
							pen.Ink.Follow = node;
						}
					}
				},
				Released = (NCard _) =>
				{
					TakeOff(pen, flash: true);
				}
			};
			pen.Plan = plan;
			CardFlight.Claim(pen.Card, plan, 45f);
		}

		private void TakeOff(Pen pen, bool flash)
		{
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			if (pen.Off)
			{
				return;
			}
			pen.Off = true;
			if (Dead || !_pens.Contains(pen))
			{
				return;
			}
			Signature ink = pen.Ink;
			if (ink != null)
			{
				ink.Written = 1f;
				if (ink.Age < 0f)
				{
					ink.Age = 0f;
				}
				ink.Wait = false;
				ink.Gone = 0f;
			}
			if (pen.Stage != Stage.Signed)
			{
				pen.Stage = Stage.Signed;
				pen.T = 0f;
				FreeQuill(pen);
			}
			if (!flash)
			{
				return;
			}
			Kick = 1f;
			float num = Tuning.F("planner.takeoff_flash", 1f);
			Node val = Layer();
			if (num < 0.01f || !FxKit.Alive((GodotObject)(object)val))
			{
				return;
			}
			if (!Where(pen.Node, pen.Card, out var at, out var turn, out var scale))
			{
				if (!pen.Placed)
				{
					return;
				}
				at = pen.At;
				turn = pen.Rot;
				scale = pen.Scale;
			}
			Vector2 val2 = at;
			Vector2 val3 = new Vector2(0f, InkY) * scale;
			Vector2 at2 = val2 + val3.Rotated(turn);
			FxKit.Flash(val, at, new Color(1f, 0.9f, 0.56f, Mathf.Min(1f, 0.42f * num)), 380f * scale, 1.35f, 0.2);
			FxKit.Flash(val, at2, new Color(Bright.R, Bright.G, Bright.B, Mathf.Min(1f, 0.7f * num)), 170f * scale, 1.7f, 0.16);
		}

		private static void FreeQuill(Pen pen)
		{
			if (FxKit.Alive((GodotObject)(object)pen.Root))
			{
				((Node)pen.Root).QueueFree();
			}
			pen.Root = null;
			pen.Quill = null;
		}

		private void Drop(Pen pen)
		{
			_pens.Remove(pen);
			FreeQuill(pen);
			if (pen.Plan != null && pen.Card != null && !pen.Began)
			{
				CardFlight.Release(pen.Card, pen.Plan);
			}
			Signature ink = pen.Ink;
			if (ink != null && !(ink.Gone >= 0f))
			{
				ink.Wait = false;
				if (ink.Age < 0f)
				{
					ink.Age = 0f;
				}
				ink.Gone = 0f;
			}
		}

		private void StepPens(float dt, CardModel outside)
		{
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Invalid comparison between Unknown and I4
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Invalid comparison between Unknown and I4
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Invalid comparison between Unknown and I4
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Max(0.02f, Tuning.F("planner.blink", 0.09f));
			float writeTime = WriteTime;
			for (int num2 = _pens.Count - 1; num2 >= 0; num2--)
			{
				Pen pen = _pens[num2];
				pen.Age += dt;
				pen.T += dt;
				Refresh(pen);
				switch (pen.Stage)
				{
				case Stage.Waiting:
					if (pen.Demo)
					{
						if (pen.T > 0.45f)
						{
							Write(pen);
							break;
						}
					}
					else
					{
						pen.Wanted = !Stopping && pen.Card == outside;
					}
					pen.In = Mathf.Clamp(pen.In + (pen.Wanted ? dt : (0f - dt)) / num, 0f, 1f);
					if (!pen.Wanted && pen.In <= 0f)
					{
						Drop(pen);
					}
					break;
				case Stage.Writing:
					pen.In = Mathf.Min(1f, pen.In + dt / (num * 0.6f));
					if (!pen.Placed)
					{
						pen.T = 0f;
						pen.Lost += dt;
						if (pen.Lost > 0.35f)
						{
							Drop(pen);
						}
						break;
					}
					if (pen.Ink != null)
					{
						pen.Ink.Written = Along(pen.T / writeTime);
					}
					if (!pen.Heard)
					{
						pen.Heard = true;
						Sfx.Play("planner_write", 1f, 0f, Mathf.Clamp(0.9f / writeTime, 0.5f, 2f));
					}
					if (!(pen.T < writeTime))
					{
						pen.Stage = Stage.Signed;
						pen.T = 0f;
						FreeQuill(pen);
						if (pen.Ink != null)
						{
							pen.Ink.Written = 1f;
							pen.Ink.Age = 0f;
							pen.Ink.Wait = true;
						}
						if (Anchor.Owner == null)
						{
							_previewCount = Mathf.Min(8, _previewCount + 1);
						}
					}
					break;
				case Stage.Signed:
					if (pen.Demo && pen.T > 0.22f)
					{
						TakeOff(pen, flash: true);
					}
					break;
				}
				if (_pens.Contains(pen))
				{
					if (pen.Stage != Stage.Waiting && !pen.Demo && !pen.Off)
					{
						PileType? val = null;
						try
						{
							CardModel card = pen.Card;
							PileType? val2;
							if (card == null)
							{
								val2 = null;
							}
							else
							{
								CardPile pile = card.Pile;
								val2 = ((pile != null) ? new PileType?(pile.Type) : ((PileType?)null));
							}
							val = val2;
						}
						catch
						{
							val = null;
						}
						if ((int)val.GetValueOrDefault() == 5)
						{
							pen.InPlay = true;
						}
						else if (pen.InPlay)
						{
							pen.Out = ((pen.Out < 0f) ? 0f : (pen.Out + dt));
						}
						bool flag = pen.Began || CardFlight.Holds(pen.Card, pen.Plan);
						if (pen.Stage == Stage.Signed)
						{
							if (pen.Out >= 0f && !flag)
							{
								TakeOff(pen, (int)val.GetValueOrDefault() == 3 || (int)val.GetValueOrDefault() == 1);
							}
							else if (pen.Out > 0.8f && !pen.Began)
							{
								TakeOff(pen, flash: false);
							}
							else if (!pen.InPlay && pen.T > 4f)
							{
								TakeOff(pen, flash: false);
							}
						}
					}
					if (pen.Stage == Stage.Signed && (pen.Off || pen.T > 40f))
					{
						Drop(pen);
					}
				}
			}
		}

		private void Place(Pen pen)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			Node2D root = pen.Root;
			if (!FxKit.Alive((GodotObject)(object)root))
			{
				return;
			}
			if (!pen.Placed)
			{
				((CanvasItem)root).Visible = false;
				return;
			}
			((CanvasItem)root).Visible = true;
			float num;
			if (pen.Stage == Stage.Writing)
			{
				num = Mathf.Clamp(pen.T / WriteTime, 0f, 1f);
			}
			else
			{
				num = ((pen.Stage == Stage.Signed) ? 1f : 0f);
			}
			float written = Along(num);
			Vector2 val = PathAt(written);
			float num2 = Tuning.F("planner.lean", 0.62f);
			float num3 = pen.In;
			if (pen.Stage == Stage.Waiting)
			{
				val += new Vector2(-3f, 0f - (9f + 2.5f * Mathf.Sin(Time * 3.1f + pen.Phase)));
				num2 += 0.04f * Mathf.Sin(Time * 1.7f + pen.Phase);
			}
			else
			{
				num2 += Tuning.F("planner.sway", 0.3f) * SlopeAt(written);
				num3 *= 1f - Mathf.SmoothStep(Mathf.Clamp(Tuning.F("planner.fade_from", 0.5f), 0f, 0.95f), 1f, num);
			}
			Vector2 at = pen.At;
			Vector2 val2 = val * pen.Scale;
			root.GlobalPosition = at + val2.Rotated(pen.Rot);
			float num4 = Tuning.F("planner.out_scale", 1.6f) * Mathf.Clamp(pen.Scale, 0.4f, 1.3f);
			root.Rotation = 0f;
			root.Scale = Vector2.One;
			root.GlobalScale = new Vector2(num4, num4);
			((CanvasItem)root).Modulate = new Color(1f, 1f, 1f, A * Mathf.Clamp(num3, 0f, 1f));
			if (FxKit.Alive((GodotObject)(object)pen.Quill))
			{
				pen.Quill.Rotation = num2 + pen.Rot;
			}
		}

		private void Follow()
		{
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			if (RemoteOwner)
			{
				EnterRemote();
				return;
			}
			for (int i = 0; i < _pens.Count; i++)
			{
				Pen pen = _pens[i];
				Refresh(pen);
				Place(pen);
			}
			for (int j = 0; j < _signatures.Count; j++)
			{
				Signature signature = _signatures[j];
				if (FxKit.Alive((GodotObject)(object)signature.Holder))
				{
					Vector2 at;
					float turn;
					float scale;
					if (signature.Follow == null && signature.Card == null)
					{
						((CanvasItem)signature.Holder).Visible = true;
						signature.Holder.GlobalPosition = signature.At;
						signature.Holder.Rotation = 0f;
						signature.Holder.Scale = new Vector2(signature.Scale, signature.Scale);
					}
					else if (Where(signature.Follow, signature.Card, out at, out turn, out scale))
					{
						((CanvasItem)signature.Holder).Visible = true;
						signature.Holder.GlobalPosition = at;
						signature.Holder.Rotation = turn;
						signature.Holder.Scale = new Vector2(scale, scale);
					}
					else
					{
						((CanvasItem)signature.Holder).Visible = false;
					}
				}
			}
		}

		private static Signature MakeSignature(Node layer, Color color)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected Obj, but got Unknown
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Expected Obj, but got Unknown
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Expected Obj, but got Unknown
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_PlannerSignature"),
				Visible = false
			};
			layer.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return null;
			}
			Line2D val2 = new Line2D
			{
				Width = 11f,
				DefaultColor = new Color(color.R, color.G, color.B, 0.22f),
				BeginCapMode = (LineCapMode)2,
				EndCapMode = (LineCapMode)2,
				JointMode = (LineJointMode)2,
				Material = (Material)(object)FxKit.Additive,
				Antialiased = true
			};
			Line2D val3 = new Line2D
			{
				Width = 5f,
				DefaultColor = color,
				BeginCapMode = (LineCapMode)2,
				EndCapMode = (LineCapMode)2,
				JointMode = (LineJointMode)2,
				Material = (Material)(object)FxKit.Additive,
				Antialiased = true
			};
			((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
			return new Signature
			{
				Holder = val,
				Line = val3,
				Halo = val2
			};
		}

		private void FreeSignature(Signature sig)
		{
			if (FxKit.Alive((GodotObject)(object)sig.Holder))
			{
				((Node)sig.Holder).QueueFree();
			}
			_signatures.Remove(sig);
		}

		private void StepSignatures(float dt)
		{
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			Vector2[] path = Path();
			float num = Mathf.Max(0f, Tuning.F("planner.ink_stay", 0.3f));
			float num2 = Mathf.Max(0.05f, Tuning.F("planner.ink_fade", 0.4f));
			for (int num3 = _signatures.Count - 1; num3 >= 0; num3--)
			{
				Signature signature = _signatures[num3];
				if (!FxKit.Alive((GodotObject)(object)signature.Holder) || !FxKit.Alive((GodotObject)(object)signature.Line) || !FxKit.Alive((GodotObject)(object)signature.Halo))
				{
					FreeSignature(signature);
				}
				else if (!signature.Mark)
				{
					if (signature.Written != signature.Shown)
					{
						signature.Shown = signature.Written;
						Vector2[] points = Slice(path, 0f, Mathf.Max(0.004f, signature.Written));
						signature.Line.Points = points;
						signature.Halo.Points = points;
					}
					if (signature.Age >= 0f)
					{
						signature.Age += dt;
					}
					if (signature.Gone < 0f)
					{
						float num4 = ((signature.Age >= 0f && signature.Age < 0.14f) ? (1f - signature.Age / 0.14f) : 0f);
						signature.Line.Width = 5f + 1.5f * num4;
						((CanvasItem)signature.Holder).Modulate = new Color(1f, 1f, 1f, A);
						if (signature.Age > 45f)
						{
							signature.Gone = num;
						}
					}
					else
					{
						signature.Gone += dt;
						float num5 = ((signature.Gone < 0.2f) ? (1f - signature.Gone / 0.2f) : 0f);
						signature.Line.Width = 5f + 3.5f * num5;
						signature.Line.DefaultColor = Gold.Lerp(Bright, num5);
						signature.Halo.DefaultColor = new Color(Gold.R, Gold.G, Gold.B, 0.22f + 0.4f * num5);
						float num6 = ((signature.Gone < num) ? 1f : (1f - (signature.Gone - num) / num2));
						((CanvasItem)signature.Holder).Modulate = new Color(1f, 1f, 1f, A * Mathf.Clamp(num6, 0f, 1f));
						if (signature.Gone > num + num2)
						{
							FreeSignature(signature);
						}
					}
				}
			}
		}

		private static NCard FindPlaying(CardModel card)
		{
			if (card == null)
			{
				return null;
			}
			try
			{
				NCard val = NCard.FindOnTable(card, (PileType?)(PileType)5);
				if (!FxKit.Alive((GodotObject)(object)val))
				{
					val = NCard.FindOnTable(card, (PileType?)null);
				}
				return (FxKit.Alive((GodotObject)(object)val) && ((Node)val).IsInsideTree()) ? val : null;
			}
			catch
			{
				return null;
			}
		}

		private void BeginPayoff(CardModel card)
		{
			if (!Dead && !Stopping)
			{
				if (RemoteOwner)
				{
					EnterRemote();
					Shine();
				}
				else
				{
					_payoffs.Add(new Payoff
					{
						Card = card
					});
				}
			}
		}

		private void Shine()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			_shine = 0f;
			Kick = 1f;
			if (!(_seen < 0.5f) && FxKit.Alive((GodotObject)(object)Back))
			{
				FxKit.Flash((Node)(object)Back, SealPoint() + new Vector2(0f, (0f - _length) * 0.55f), new Color(Bright.R, Bright.G, Bright.B, 0.5f * A), _length * 1.5f, 1.6f, 0.26);
			}
		}

		private void StepPayoffs(float dt)
		{
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			for (int num = _payoffs.Count - 1; num >= 0; num--)
			{
				Payoff payoff = _payoffs[num];
				if (payoff.Ink == null)
				{
					payoff.Wait += dt;
					NCard val = null;
					bool flag = payoff.Demo;
					if (!payoff.Demo)
					{
						val = FindPlaying(payoff.Card);
						try
						{
							flag = val != null && ((CanvasItem)val).IsVisibleInTree() && ((CanvasItem)val).Modulate.A > 0.5f;
						}
						catch
						{
							flag = false;
						}
					}
					payoff.T = (flag ? (payoff.T + dt) : 0f);
					if (!payoff.Demo && (payoff.T < 0.05f || payoff.Wait < Tuning.F("planner.mark_delay", 0.1f)))
					{
						if (payoff.Wait > 1.2f)
						{
							Shine();
							_payoffs.RemoveAt(num);
						}
						continue;
					}
					if (!StartMark(payoff, val))
					{
						_payoffs.RemoveAt(num);
						continue;
					}
				}
				payoff.T += dt;
				Signature ink = payoff.Ink;
				if (!FxKit.Alive((GodotObject)(object)ink.Holder) || !FxKit.Alive((GodotObject)(object)ink.Line) || !FxKit.Alive((GodotObject)(object)ink.Halo))
				{
					_payoffs.RemoveAt(num);
					continue;
				}
				float t = payoff.T;
				float num2 = Mathf.Max(0.06f, Tuning.F("planner.mark_run", 0.22f));
				float num3 = Mathf.Max(num2, Tuning.F("planner.mark_stay", 0.55f));
				float num4 = Mathf.Max(0.05f, Tuning.F("planner.mark_fade", 0.4f));
				float num5 = ((t < 0.05f) ? (t / 0.05f) : Mathf.Max(0f, 1f - (t - 0.05f) / 0.26f));
				ink.Line.Width = 5f + 4f * num5;
				ink.Halo.Width = 12f + 10f * num5;
				ink.Halo.DefaultColor = new Color(Gold.R, Gold.G, Gold.B, 0.24f + 0.4f * num5);
				float num6;
				if (t < 0.04f)
				{
					num6 = t / 0.04f;
				}
				else
				{
					num6 = ((t < num3) ? 1f : (1f - (t - num3) / num4));
				}
				((CanvasItem)ink.Holder).Modulate = new Color(1f, 1f, 1f, A * Mathf.Clamp(num6, 0f, 1f));
				float num7 = t / num2 * 1.22f;
				if (FxKit.Alive((GodotObject)(object)ink.Glint))
				{
					bool flag2 = num7 < 1.22f;
					((CanvasItem)ink.Glint).Visible = flag2;
					if (flag2)
					{
						ink.Glint.Points = Slice(Path(), num7 - 0.22f, num7);
					}
				}
				if (!payoff.Sparked && num7 >= 1f)
				{
					payoff.Sparked = true;
					Node val2 = Layer();
					if (FxKit.Alive((GodotObject)(object)val2) && ((CanvasItem)ink.Holder).Visible)
					{
						Vector2[] array = Path();
						Brush.Sparks(val2, ink.Holder.ToGlobal(array[^1]), Gold, 6, 240f, new Vector2(1f, -0.35f), 55f);
					}
				}
				if (!(t <= num3 + num4))
				{
					FreeSignature(ink);
					_payoffs.RemoveAt(num);
				}
			}
		}

		private bool StartMark(Payoff pay, NCard node)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Expected Obj, but got Unknown
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			Node val = Layer();
			if (!FxKit.Alive((GodotObject)(object)val))
			{
				return false;
			}
			Signature signature = MakeSignature(val, Bright);
			if (signature == null)
			{
				return false;
			}
			signature.Mark = true;
			signature.Card = pay.Card;
			signature.Follow = node;
			signature.At = pay.At;
			signature.Scale = pay.Scale;
			signature.Written = 1f;
			Vector2[] points = Path();
			signature.Line.Points = points;
			signature.Halo.Points = points;
			((CanvasItem)signature.Holder).Modulate = new Color(1f, 1f, 1f, 0f);
			signature.Glint = Props.Line(8f, new Color(1f, 1f, 1f, 0.95f), true);
			Curve val2 = new Curve();
			val2.AddPoint(new Vector2(0f, 0.15f), 0f, 0f, (TangentMode)0, (TangentMode)0);
			val2.AddPoint(new Vector2(1f, 1f), 0f, 0f, (TangentMode)0, (TangentMode)0);
			signature.Glint.WidthCurve = val2;
			((CanvasItem)signature.Glint).Visible = false;
			((Node)signature.Holder).AddChild((Node)(object)signature.Glint, false, (InternalMode)0);
			_signatures.Add(signature);
			pay.Ink = signature;
			pay.T = 0f;
			Shine();
			Vector2 at = pay.At;
			float turn = 0f;
			float scale = pay.Scale;
			if (!pay.Demo && !Where(node, pay.Card, out at, out turn, out scale))
			{
				return true;
			}
			Vector2 val3 = at;
			Vector2 val4 = new Vector2(0f, InkY) * scale;
			Vector2 at2 = val3 + val4.Rotated(turn);
			FxKit.Flash(val, at2, new Color(1f, 0.9f, 0.55f, 0.5f), Tuning.F("planner.card_size", 330f) * 0.9f * scale, 1.5f, 0.26);
			return true;
		}

		private void Flinch()
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			if (!Dead && !Stopping)
			{
				_ping = 0f;
				Kick = 1f;
				if (RemoteOwner)
				{
					EnterRemote();
				}
				Node val = (Node)(RemoteOwner ? ((object)Back) : ((object)Layer()));
				if (FxKit.Alive((GodotObject)(object)val) && _seen > 0.5f)
				{
					FxKit.RingPulse(val, SealPoint() + new Vector2(0f, (0f - _length) * 0.5f), _length * 1.5f, Gold, 0f, 0.26f, 0.6f, 1.35f);
				}
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			if (RemoteOwner)
			{
				EnterRemote();
				return;
			}
			Vector2 at2 = TablePoint();
			if (_demoSecond)
			{
				_payoffs.Add(new Payoff
				{
					Demo = true,
					At = at2,
					Scale = 0.8f
				});
			}
			else
			{
				Pen pen = NewPen(null);
				if (pen != null)
				{
					pen.Demo = true;
					pen.At = at2;
					pen.Scale = 0.8f;
					pen.Placed = true;
				}
			}
			_demoSecond = !_demoSecond;
		}

		protected override void OnDispose()
		{
			ClearRemote();
			Live.Remove(this);
			if (FxKit.Alive((GodotObject)(object)_root))
			{
				((Node)_root).QueueFree();
			}
			foreach (Pen pen in _pens)
			{
				if (FxKit.Alive((GodotObject)(object)pen.Root))
				{
					((Node)pen.Root).QueueFree();
				}
				if (pen.Plan != null && pen.Card != null && !pen.Began)
				{
					CardFlight.Release(pen.Card, pen.Plan);
				}
			}
			_pens.Clear();
			_payoffs.Clear();
			foreach (Signature signature in _signatures)
			{
				if (FxKit.Alive((GodotObject)(object)signature.Holder))
				{
					((Node)signature.Holder).QueueFree();
				}
			}
			_signatures.Clear();
		}

		static MasterPlannerFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		}
	}
