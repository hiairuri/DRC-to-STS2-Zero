// namespace: SilentFX.Cards
// type: MementoHeld (class)
// source lines: 75916-79188
	public sealed class MementoHeld : HeldSession
	{
		private enum Phase
		{
			Standby,
			Barrage,
			Dissolve,
			Over
		}

		private struct Mark
		{
			public CardModel Card;

			public PileType Pile;
		}

		private sealed class Copy
		{
			public CardModel Card;

			public NCard Node;

			public PileType Pile;

			public int Index;

			public int Of = 1;

			public int Order;

			public int Seed;

			public float Born;

			public bool Puffed;

			public bool Moved;

			public bool WantOut;

			public float Wait;

			public float Out;

			public bool HomeKnown;

			public Vector2 HomePos;

			public float HomeRot;

			public Vector2 HomeScale = new Vector2(0.2f, 0.2f);

			public float HomeAlpha;

			public bool SlotKnown;

			public Vector2 Slot;

			public Vector2 Pos;

			public float Rot;

			public Vector2 Scale = new Vector2(0.2f, 0.2f);

			public float Alpha;

			public Vector2 FromPos;

			public float FromRot;

			public Vector2 FromScale = new Vector2(0.2f, 0.2f);

			public float FromAlpha;

			public float Coil;

			public float Reach = 1f;

			public float LaunchAt;

			public float Weight;

			public bool Heavy;

			public bool Launched;

			public float Flight;

			public Vector2 Spot;

			public bool Hit;

			public Vector2 Tail;

			public Sprite2D Halo;

			public Node2D Streak;

			public Sprite2D StreakDark;

			public Sprite2D StreakLight;

			public Copy()
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			}
		}

		private sealed class Wither
		{
			public MementoHeld Owner;

			public int Deaths = 1;

			public int Seed;

			public float Level;

			public Node2D Inner;

			public ShadeMist Veil;

			public Sprite2D Shadow;

			public Sprite2D[] Blobs = new Sprite2D[0];

			public Sprite2D[] Flakes = new Sprite2D[0];
		}

		private const string DecoKey = "memento";

		private const int MaxBlobs = 7;

		private const int MaxFlakes = 6;

		private const float CardWidth = 300f;

		private const float CardHeight = 422f;

		private const float RuleShare = 0.23f;

		private const float RuleUneven = 1.75f;

		private const float RuleMost = 92f;

		private const float RuleGap = 11f;

		private const float GemHalf = 6f;

		internal static readonly Color MistColor = new Color(0.07f, 0.04f, 0.12f, 1f);

		internal static readonly Color Violet = new Color(0.5f, 0.3f, 0.85f, 1f);

		private static readonly Color RimColor = new Color(0.62f, 0.42f, 1f, 1f);

		private static readonly Color RuleColor = new Color(0.74f, 0.62f, 1f, 1f);

		private static readonly Color GhostTint = new Color(0.48f, 0.4f, 0.7f, 1f);

		private static readonly Color GhostLit = new Color(0.8f, 1f, 0.86f, 1f);

		private static readonly Color GhostColor = new Color(0.6f, 1f, 0.76f, 1f);

		private const float HeavyReach = 2.3f;

		private const float HeavyTravel = 1.2f;

		private readonly List<Copy> _copies = new List<Copy>();

		private readonly List<CardDeco.Handle> _decos = new List<CardDeco.Handle>();

		private readonly List<CardModel> _handMarks = new List<CardModel>();

		private readonly List<CardModel> _deathsScratch = new List<CardModel>();

		private readonly List<Mark> _marksScratch = new List<Mark>();

		private Node2D _layer;

		private bool _hosted;

		private Node2D _under;

		private Node2D _cards;

		private ShadeMist _drawMist;

		private ShadeMist _discardMist;

		private int _drawCount;

		private int _discardCount;

		private Vector2 _drawAt;

		private Vector2 _discardAt;

		private Vector2 _view = new Vector2(1920f, 1080f);

		private float _stackShow;

		private float _pileScale = 0.42f;

		private float _pileAlpha = 0.95f;

		private float _pileRise = 235f;

		private float _ringScale = 0.34f;

		private float _ringAlpha = 0.95f;

		private float _ringMin = 150f;

		private float _ringMax = 300f;

		private float _fly = 0.22f;

		private float _stagger = 0.03f;

		private float _glide = 0.18f;

		private float _rise = 0.22f;

		private float _trembleMove = 3f;

		private float _trembleTurn = 0.04f;

		private float _peel = 30f;

		private float _handAlpha = 0.45f;

		private float _halo = 0.55f;

		private Phase _phase;

		private float _clock;

		private float _scan;

		private int _signature;

		private bool _struck;

		private bool _decoOut;

		private float _struckFor;

		private Vector2 _strikeAt;

		private int _slamCount;

		private int _landed;

		private Copy _heavy;

		private bool _finale;

		private float _lead = 0.07f;

		private float _travel = 0.07f;

		private float _beat;

		private float _drawBack = 46f;

		private float _hitUnit = 1f;

		private Node2D _victim;

		private Vector2 _victimHome;

		private Vector2 _victimWrote;

		private Vector2 _victimMoved;

		private Vector2 _joltAt;

		private Vector2 _joltSpeed;

		private bool _out;

		private bool _ringSet;

		private Vector2 _ringAt;

		private int _ringRoom = 8;

		private Node2D _frame;

		private readonly Node2D[] _corners = new Node2D[4];

		private readonly Line2D[] _rules = new Line2D[16];

		private readonly Sprite2D[] _grounds = new Sprite2D[8];

		private Vector2 _arms = new Vector2(49f, 58f);

		private Vector2 _armsDrawn;

		private bool _aim;

		private object _aimTarget;

		private Vector2 _aimAt;

		private float _aimWidth = 200f;

		private float _aimHeight = 240f;

		private Vector2 _boxHalf = new Vector2(106f, 126f);

		private float _cornerSize = 1f;

		private float _close;

		private float _frameShow;

		private float _snap = -1f;

		private float _snapTime = 0.1f;

		private float _flinch;

		private bool _frameOff;

		private bool _puffDue;

		private float _puffWait;

		private const float JoltSpring = 1000f;

		private const float JoltDamp = 32f;

		private const float JoltKick = 950f;

		private const float JoltMax = 40f;

		private float DecoLevel
		{
			get
			{
				if (Finished || _decoOut)
				{
					return 0f;
				}
				float num = Mathf.Clamp(Tuning.F("memento.hand_mist", 1f), 0f, 1.5f);
				return Mathf.Clamp(Fade, 0f, 1f) * Mathf.Lerp(0.85f, 1f, Mathf.Clamp(Lift, 0f, 1f)) * num;
			}
		}

		public bool Striking
		{
			get
			{
				if (_phase == Phase.Barrage)
				{
					return _slamCount > 0;
				}
				return false;
			}
		}

		private static Color Mist(float alpha)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			return new Color(MistColor.R, MistColor.G, MistColor.B, Mathf.Clamp(alpha, 0f, 1f));
		}

		private static Color Lit(float alpha)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			return new Color(Violet.R, Violet.G, Violet.B, Mathf.Clamp(alpha, 0f, 1f));
		}

		private static Color Rim(float alpha)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			return new Color(RimColor.R, RimColor.G, RimColor.B, Mathf.Clamp(alpha, 0f, 1f));
		}

		private static Color Rule(float alpha)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			return new Color(RuleColor.R, RuleColor.G, RuleColor.B, Mathf.Clamp(alpha, 0f, 1f));
		}

		private static Color Ghost(float alpha)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			return new Color(GhostColor.R, GhostColor.G, GhostColor.B, Mathf.Clamp(alpha, 0f, 1f));
		}

		private static Color Pale(float alpha)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			return new Color(0.93f, 1f, 0.95f, Mathf.Clamp(alpha, 0f, 1f));
		}

		private static float Wobble(float x)
		{
			return 0.65f * Mathf.Sin(x) + 0.35f * Mathf.Sin(x * 2.31f + 1.3f);
		}

		private void ReadTuning()
		{
			_pileScale = Mathf.Clamp(Tuning.F("memento.pile_scale", 0.42f), 0.15f, 0.8f);
			_pileAlpha = Mathf.Clamp(Tuning.F("memento.pile_alpha", 0.95f), 0.1f, 1f);
			_pileRise = Mathf.Clamp(Tuning.F("memento.pile_rise", 235f), 80f, 420f);
			_ringScale = Mathf.Clamp(Tuning.F("memento.ring_scale", 0.34f), 0.12f, 0.8f);
			_ringAlpha = Mathf.Clamp(Tuning.F("memento.ring_alpha", 0.95f), 0.1f, 1f);
			_ringMin = Mathf.Clamp(Tuning.F("memento.ring_min", 150f), 60f, 500f);
			_ringMax = Mathf.Max(_ringMin, Mathf.Clamp(Tuning.F("memento.ring_max", 300f), 60f, 600f));
			_fly = Mathf.Clamp(Tuning.F("memento.fly_out", 0.22f), 0.05f, 1f);
			_stagger = Mathf.Clamp(Tuning.F("memento.stagger", 0.03f), 0f, 0.2f);
			_glide = Mathf.Clamp(Tuning.F("memento.glide", 0.18f), 0.03f, 1f);
			_rise = Mathf.Clamp(Tuning.F("memento.rise", 0.22f), 0.03f, 1f);
			_trembleMove = Mathf.Clamp(Tuning.F("memento.tremble", 3f), 0f, 12f);
			_trembleTurn = Mathf.Clamp(Tuning.F("memento.tremble_turn", 0.04f), 0f, 0.3f);
			_peel = Mathf.Clamp(Tuning.F("memento.peel", 30f), 0f, 120f);
			_handAlpha = Mathf.Clamp(Tuning.F("memento.hand_alpha", 0.45f), 0f, 1f);
			_halo = Mathf.Clamp(Tuning.F("memento.halo", 0.55f), 0f, 1f);
		}

		public static List<CardModel> Deaths(CardModel card)
		{
			List<CardModel> list = new List<CardModel>();
			FillDeaths(card, list);
			return list;
		}

		private static void FillDeaths(CardModel card, List<CardModel> list)
		{
			list.Clear();
			try
			{
				object obj;
				if (card == null)
				{
					obj = null;
				}
				else
				{
					Player owner = card.Owner;
					obj = ((owner != null) ? owner.Creature : null);
				}
				Creature val = (Creature)obj;
				ICombatState val2 = ((card != null) ? card.CombatState : null);
				CombatManager instance = CombatManager.Instance;
				CombatHistory val3 = ((instance != null) ? instance.History : null);
				if (val == null || val2 == null || val3 == null)
				{
					return;
				}
				foreach (CombatHistoryEntry entry in val3.Entries)
				{
					CardDiscardedEntry val4 = (CardDiscardedEntry)(object)((entry is CardDiscardedEntry) ? entry : null);
					if (val4 != null && val4.Card != null && ((CombatHistoryEntry)val4).Actor == val && ((CombatHistoryEntry)val4).HappenedThisTurn(val2))
					{
						list.Add(val4.Card);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoHeld.Deaths: " + ex.Message);
				list.Clear();
			}
		}

		private List<Mark> Marks()
		{
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Invalid comparison between Unknown and I4
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Invalid comparison between Unknown and I4
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Invalid comparison between Unknown and I4
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			List<Mark> marksScratch = _marksScratch;
			marksScratch.Clear();
			CardModel val = C?.Card;
			if (val == null)
			{
				return marksScratch;
			}
			int num = Mathf.Clamp(Mathf.RoundToInt(Tuning.F("memento.max", 12f)), 0, 24);
			FillDeaths(val, _deathsScratch);
			foreach (CardModel item in _deathsScratch)
			{
				if (marksScratch.Count >= num)
				{
					break;
				}
				if (item != null && item != val)
				{
					PileType val2;
					try
					{
						CardPile pile = item.Pile;
						val2 = (PileType)((pile != null) ? ((int)pile.Type) : 0);
					}
					catch
					{
						val2 = (PileType)0;
					}
					if ((int)val2 == 2 || (int)val2 == 1 || (int)val2 == 3)
					{
						marksScratch.Add(new Mark
						{
							Card = item,
							Pile = val2
						});
					}
				}
			}
			return marksScratch;
		}

		private static int Signature(List<Mark> marks)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected I4, but got Unknown
			int num = marks.Count;
			foreach (Mark mark in marks)
			{
				int hashCode = RuntimeHelpers.GetHashCode(mark.Card);
				num = num * 31 + hashCode * 7 + mark.Pile;
			}
			return num;
		}

		protected override void Build()
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Expected Obj, but got Unknown
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Expected Obj, but got Unknown
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Expected Obj, but got Unknown
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			ReadTuning();
			Node val = (Node)((!C.Remote && FxKit.Alive((GodotObject)(object)C.UiParent) && C.UiParent.IsInsideTree()) ? ((object)C.UiParent) : ((object)Front));
			_hosted = (object)val != Front;
			_layer = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Memento")
			};
			TimeFreeze.KeepLive((Node)(object)_layer);
			val.AddChild((Node)(object)_layer, false, (InternalMode)0);
			_under = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_MementoUnder")
			};
			((Node)_layer).AddChild((Node)(object)_under, false, (InternalMode)0);
			_cards = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_MementoCards")
			};
			((Node)_layer).AddChild((Node)(object)_cards, false, (InternalMode)0);
			_view = FxKit.ViewSize();
			if (!C.Remote)
			{
				_drawAt = GameRefs.PilePoint((PileType)1);
				_discardAt = GameRefs.PilePoint((PileType)3);
			}
			try
			{
				if (!C.Remote)
				{
					BuildFrame();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoHeld.BuildFrame: " + ex);
			}
			try
			{
				Raise(Marks());
			}
			catch (Exception ex2)
			{
				DevLog.Warn("MementoHeld.Build: " + ex2);
				Clear();
			}
		}

		private void Raise(List<Mark> marks)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Invalid comparison between Unknown and I4
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Invalid comparison between Unknown and I4
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Invalid comparison between Unknown and I4
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Invalid comparison between Unknown and I4
			_signature = Signature(marks);
			_drawCount = 0;
			_discardCount = 0;
			Dictionary<CardModel, int> dictionary = new Dictionary<CardModel, int>(ReferenceEqualityComparer.Instance);
			List<Copy> list = new List<Copy>();
			List<Copy> list2 = new List<Copy>();
			List<Copy> list3 = new List<Copy>();
			foreach (Mark mark in marks)
			{
				if ((int)mark.Pile == 2)
				{
					if (dictionary.TryGetValue(mark.Card, out var value))
					{
						dictionary[mark.Card] = value + 1;
						continue;
					}
					dictionary[mark.Card] = 1;
					_handMarks.Add(mark.Card);
					list2.Add(new Copy
					{
						Card = mark.Card,
						Pile = (PileType)2
					});
					continue;
				}
				Copy copy = new Copy
				{
					Card = mark.Card,
					Pile = mark.Pile
				};
				if ((int)mark.Pile == 1)
				{
					copy.Index = _drawCount++;
					list.Add(copy);
				}
				else
				{
					copy.Index = _discardCount++;
					list3.Add(copy);
				}
			}
			_copies.AddRange(list);
			_copies.AddRange(list2);
			_copies.AddRange(list3);
			for (int i = 0; i < _copies.Count; i++)
			{
				Copy copy2 = _copies[i];
				copy2.Order = i;
				copy2.Seed = i * 7 + 3;
				copy2.Born = Time + (float)i * 0.03f;
				copy2.Of = (((int)copy2.Pile == 1) ? Mathf.Max(1, _drawCount) : (((int)copy2.Pile != 3) ? 1 : Mathf.Max(1, _discardCount)));
			}
			if (C.Remote)
			{
				return;
			}
			if (_drawCount > 0 && _drawMist == null)
			{
				_drawMist = StackMist(1.7f);
			}
			if (_discardCount > 0 && _discardMist == null)
			{
				_discardMist = StackMist(6.3f);
			}
			foreach (CardModel handMark in _handMarks)
			{
				Wilt(handMark, dictionary[handMark]);
			}
		}

		private ShadeMist StackMist(float seed)
		{
			ShadeMist shadeMist = new ShadeMist(1, seed);
			shadeMist.Set("wisp", 1.6f);
			shadeMist.Set("glow", 0.45f);
			if (FxKit.Alive((GodotObject)(object)_under))
			{
				((Node)_under).AddChild((Node)(object)shadeMist.Root, false, (InternalMode)0);
			}
			return shadeMist;
		}

		private bool Materialise(Copy copy)
		{
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			if (copy.Card == null || !FxKit.Alive((GodotObject)(object)_cards))
			{
				return false;
			}
			copy.Node = CardStandIn.Make(copy.Card, (Node)(object)_cards);
			if (!FxKit.Alive((GodotObject)(object)copy.Node))
			{
				copy.Node = null;
				return false;
			}
			if (C.Remote)
			{
				Vector2 val = Radii(_aimWidth, _aimHeight);
				_ringRoom = Mathf.Max(3, Mathf.FloorToInt((float)Math.PI * 2f * Mathf.Min(val.X, val.Y) / (300f * _ringScale * 1.12f)));
				if (!_ringSet)
				{
					_ringAt = _aimAt;
					_ringSet = true;
				}
				copy.Slot = SlotOf(copy, _aimAt, val);
				copy.SlotKnown = true;
				copy.Pos = copy.Slot;
				Vector2 val2 = _aimAt - copy.Slot;
				copy.Rot = val2.Angle() + (float)Math.PI / 2f;
				copy.Scale = new Vector2(_ringScale, _ringScale);
				copy.Alpha = 0f;
			}
			else
			{
				Home(copy);
				copy.Pos = copy.HomePos;
				copy.Rot = copy.HomeRot;
				copy.Scale = copy.HomeScale;
				copy.Alpha = copy.HomeAlpha;
			}
			copy.Tail = copy.Pos;
			MakeUnder(copy);
			Put(copy);
			return true;
		}

		private void MakeUnder(Copy copy)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected Obj, but got Unknown
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Expected Obj, but got Unknown
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_under))
			{
				if (!FxKit.Alive((GodotObject)(object)copy.Halo))
				{
					copy.Halo = FxKit.Glow(Rim(0f), new Vector2(64f, 64f));
					((Node)_under).AddChild((Node)(object)copy.Halo, false, (InternalMode)0);
				}
				if (!FxKit.Alive((GodotObject)(object)copy.Streak))
				{
					Node2D val = new Node2D
					{
						Name = StringName.op_Implicit("SilentFX_MementoStreak"),
						Visible = false
					};
					copy.StreakDark = new Sprite2D
					{
						Texture = FxKit.SoftDot,
						Modulate = Mist(0f)
					};
					copy.StreakLight = FxKit.Glow(Lit(0f), new Vector2(64f, 64f));
					((Node)val).AddChild((Node)(object)copy.StreakDark, false, (InternalMode)0);
					((Node)val).AddChild((Node)(object)copy.StreakLight, false, (InternalMode)0);
					((Node)_under).AddChild((Node)(object)val, false, (InternalMode)0);
					copy.Streak = val;
				}
			}
		}

		private void Put(Copy copy)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)copy.Node))
			{
				Vector2 val = new Vector2(Mathf.Max(0.02f, copy.Scale.X), Mathf.Max(0.02f, copy.Scale.Y));
				float num = Mathf.Clamp(copy.Alpha, 0f, 1f);
				CardStandIn.Place(copy.Node, copy.Pos, copy.Rot, val);
				float num2 = Mathf.Clamp(copy.Coil, 0f, 1f);
				((CanvasItem)copy.Node).Modulate = new Color(Mathf.Lerp(GhostTint.R, GhostLit.R, num2), Mathf.Lerp(GhostTint.G, GhostLit.G, num2), Mathf.Lerp(GhostTint.B, GhostLit.B, num2), num);
				if (FxKit.Alive((GodotObject)(object)copy.Halo))
				{
					((Node2D)copy.Halo).GlobalPosition = copy.Pos;
					((Node2D)copy.Halo).GlobalRotation = copy.Rot;
					((Node2D)copy.Halo).Scale = new Vector2(300f * val.X * Mathf.Lerp(1.62f, 1.3f, num2), 422f * val.Y * Mathf.Lerp(1.42f, 1.2f, num2)) / 64f;
					float num3 = _halo * num * (0.82f + 0.18f * Mathf.Sin(Time * 2.6f + (float)copy.Seed)) * (1f + 0.8f * num2);
					((CanvasItem)copy.Halo).Modulate = new Color(Mathf.Lerp(RimColor.R, GhostColor.R, num2), Mathf.Lerp(RimColor.G, GhostColor.G, num2), Mathf.Lerp(RimColor.B, GhostColor.B, num2), Mathf.Clamp(num3, 0f, 1f));
				}
			}
		}

		private void Wilt(CardModel card, int deaths)
		{
			try
			{
				CardDeco.Handle handle = CardDeco.Attach(card, "memento", WitherBuild, WitherTick);
				if (handle != null && handle.Alive && handle.State is Wither wither)
				{
					wither.Owner = this;
					wither.Deaths = Mathf.Max(1, deaths);
					if (!_decos.Contains(handle))
					{
						_decos.Add(handle);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoHeld.Wilt: " + ex.Message);
			}
		}

		private static void WitherBuild(CardDeco.Handle handle)
		{
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Expected Obj, but got Unknown
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Expected Obj, but got Unknown
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Expected Obj, but got Unknown
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Expected Obj, but got Unknown
			Node2D val = handle?.Root;
			if (FxKit.Alive((GodotObject)(object)val))
			{
				Wither wither = new Wither
				{
					Seed = (RuntimeHelpers.GetHashCode(handle.Card) & 0xFF)
				};
				wither.Veil = new ShadeMist(0, Mathf.PosMod((float)wither.Seed * 0.37f, 10f));
				wither.Veil.Set("foot", 0.95f);
				wither.Veil.Set("bulge", 0.22f);
				wither.Veil.Set("taper", 1.3f);
				wither.Veil.Set("wisp", 1.5f);
				wither.Veil.Set("glow", 0.2f);
				wither.Veil.Set("hollow", 0.15f);
				wither.Veil.Root.Position = new Vector2(0f, 214f);
				wither.Veil.Size(350f, 250f);
				((Node)val).AddChild((Node)(object)wither.Veil.Root, false, (InternalMode)0);
				wither.Inner = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_MementoWither"),
					Modulate = new Color(1f, 1f, 1f, 0f)
				};
				((Node)val).AddChild((Node)(object)wither.Inner, false, (InternalMode)0);
				wither.Shadow = new Sprite2D
				{
					Texture = FxKit.SoftDot,
					Modulate = Mist(0f),
					Position = new Vector2(0f, 112f)
				};
				((Node)wither.Inner).AddChild((Node)(object)wither.Shadow, false, (InternalMode)0);
				wither.Blobs = new Sprite2D[7];
				for (int i = 0; i < 7; i++)
				{
					wither.Blobs[i] = new Sprite2D
					{
						Texture = FxKit.SoftDot,
						Modulate = Mist(0f)
					};
					((Node)wither.Inner).AddChild((Node)(object)wither.Blobs[i], false, (InternalMode)0);
				}
				wither.Flakes = new Sprite2D[6];
				for (int j = 0; j < 6; j++)
				{
					wither.Flakes[j] = new Sprite2D
					{
						Texture = FxKit.SoftDot,
						Modulate = Mist(0f)
					};
					((Node)wither.Inner).AddChild((Node)(object)wither.Flakes[j], false, (InternalMode)0);
				}
				handle.State = wither;
			}
		}

		private static void WitherTick(CardDeco.Handle handle, float dt)
		{
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			if (!(handle?.State is Wither wither) || !FxKit.Alive((GodotObject)(object)wither.Inner))
			{
				return;
			}
			float target = ((wither.Owner != null && !wither.Owner.Finished) ? wither.Owner.DecoLevel : 0f);
			wither.Level = FxKit.Damp(wither.Level, target, 7f, dt);
			float num = Mathf.Clamp(wither.Level, 0f, 1.5f);
			float time = handle.Time;
			int num2 = Mathf.Clamp(wither.Deaths - 1, 0, 2);
			float num3 = 1f + 0.22f * (float)num2;
			float num4 = 0.5f + 0.5f * Mathf.Sin(time * 1.5f + (float)wither.Seed);
			((CanvasItem)wither.Inner).Modulate = new Color(1f, 1f, 1f, Mathf.Min(1f, num));
			if (wither.Veil != null && FxKit.Alive((GodotObject)(object)wither.Veil.Root))
			{
				wither.Veil.Size(350f, 240f + 46f * num4 + 30f * (float)num2);
				wither.Veil.Intensity = (0.5f + 0.2f * num4) * num3 * num;
			}
			if (FxKit.Alive((GodotObject)(object)wither.Shadow))
			{
				((Node2D)wither.Shadow).Scale = new Vector2(390f, 300f + 36f * num4) / 64f;
				((CanvasItem)wither.Shadow).Modulate = Mist((0.36f + 0.14f * num4) * num3);
			}
			int num5 = Mathf.Min(wither.Blobs.Length, 5 + num2);
			for (int i = 0; i < wither.Blobs.Length; i++)
			{
				Sprite2D val = wither.Blobs[i];
				if (FxKit.Alive((GodotObject)(object)val))
				{
					((CanvasItem)val).Visible = i < num5;
					if (i < num5)
					{
						int num6 = wither.Seed + i * 13;
						float num7 = 4.6f + 2.6f * FxKit.Hash(num6 + 1);
						float num8 = time / num7 + FxKit.Hash(num6 + 2);
						num8 -= Mathf.Floor(num8);
						float num9 = ((FxKit.Hash(num6 + 3) < 0.5f) ? (-1f) : 1f);
						float num10 = Mathf.Lerp(-90f, 90f, FxKit.Hash(num6 + 4)) + num9 * (num8 - 0.5f) * 70f + Mathf.Sin(time * 0.8f + (float)i * 2.1f) * 16f;
						float num11 = Mathf.Lerp(196f, -150f, num8);
						float num12 = Mathf.Lerp(120f, 190f, FxKit.Hash(num6 + 5)) * (0.8f + 0.5f * num8);
						float num13 = Mathf.Pow(Mathf.Max(0f, Mathf.Sin((float)Math.PI * num8)), 0.7f);
						((Node2D)val).Position = new Vector2(num10, num11);
						((Node2D)val).Scale = new Vector2(num12 * 1.15f, num12) / 64f;
						((CanvasItem)val).Modulate = Mist(num13 * 0.46f * num3);
					}
				}
			}
			int num14 = Mathf.Min(wither.Flakes.Length, 4 + num2);
			for (int j = 0; j < wither.Flakes.Length; j++)
			{
				Sprite2D val2 = wither.Flakes[j];
				if (FxKit.Alive((GodotObject)(object)val2))
				{
					((CanvasItem)val2).Visible = j < num14;
					if (j < num14)
					{
						int num15 = wither.Seed + 200 + j * 17;
						float num16 = 1.5f + 0.9f * FxKit.Hash(num15 + 1);
						float num17 = time / num16 + FxKit.Hash(num15 + 2);
						int num18 = Mathf.FloorToInt(num17);
						float num19 = num17 - (float)num18;
						int num20 = num15 + num18 % 64 * 29;
						float num21 = Mathf.Lerp(-128f, 128f, FxKit.Hash(num20 + 3)) + Mathf.Sin(num19 * 5f + (float)j) * 7f;
						float num22 = Mathf.Lerp(110f, 205f, FxKit.Hash(num20 + 4)) + 26f * num19 + 95f * num19 * num19;
						float num23 = Mathf.Lerp(8f, 15f, FxKit.Hash(num20 + 5));
						((Node2D)val2).Position = new Vector2(num21, num22);
						((Node2D)val2).Rotation = (FxKit.Hash(num20 + 6) - 0.5f) * 1.2f + num19 * 0.8f;
						((Node2D)val2).Scale = new Vector2(num23 * 0.75f, num23 * 1.35f) / 64f;
						((CanvasItem)val2).Modulate = Mist(((num19 < 0.15f) ? (num19 / 0.15f) : (1f - (num19 - 0.15f) / 0.85f)) * 0.9f);
					}
				}
			}
		}

		private void RemoveDecos(float fade)
		{
			foreach (CardDeco.Handle deco in _decos)
			{
				try
				{
					if (deco != null && deco.Alive && (!(deco.State is Wither wither) || wither.Owner == this))
					{
						deco.Remove(fade);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("MementoHeld (deco): " + ex.Message);
				}
			}
			_decos.Clear();
		}

		protected override void Tick(float dt)
		{
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!FxKit.Alive((GodotObject)(object)_layer) || !FxKit.Alive((GodotObject)(object)_under) || !FxKit.Alive((GodotObject)(object)_cards))
				{
					if (_phase != Phase.Over)
					{
						Finish(20f);
					}
					else
					{
						ReleaseBody();
					}
					return;
				}
				_clock += dt;
				((CanvasItem)_layer).Modulate = new Color(1f, 1f, 1f, _hosted ? Mathf.Clamp(Fade, 0f, 1f) : 1f);
				_view = FxKit.ViewSize();
				if (!C.Remote)
				{
					_drawAt = GameRefs.PilePoint((PileType)1);
					_discardAt = GameRefs.PilePoint((PileType)3);
				}
				if (_struck)
				{
					_struckFor += dt;
				}
				switch (_phase)
				{
				case Phase.Standby:
					TickStandby(dt);
					break;
				case Phase.Barrage:
					TickBarrage(dt);
					break;
				case Phase.Dissolve:
					TickDissolve(dt);
					break;
				}
				if (_phase != Phase.Standby && _phase != Phase.Over && _clock > 3f)
				{
					Finish(12f);
				}
				TickJolt(dt);
				TickStacks(dt);
				TickFrame(dt);
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoHeld.Tick: " + ex);
				Finish(14f);
			}
		}

		private void Finish(float rate)
		{
			ReleaseBody();
			Clear();
			_phase = Phase.Over;
			_frameOff = true;
			Leave(rate);
		}

		private void TickStandby(float dt)
		{
			if (!Released && !Leaving)
			{
				_scan += dt;
				if (_scan >= 0.3f)
				{
					_scan = 0f;
					ReadTuning();
					List<Mark> marks = Marks();
					if (Signature(marks) != _signature)
					{
						Clear();
						Raise(marks);
					}
				}
			}
			if (C.Remote && !Hovering && C.HasTarget)
			{
				Aim(C);
			}
			_out = !Hovering && (!C.Remote || C.HasTarget);
			if (!C.Remote && _out && !Released && !Leaving && _copies.Count > 0)
			{
				Sfx.Hold("memento_hang", Mathf.Clamp(Lift, 0f, 1f) * Mathf.Clamp(0.5f + 0.1f * (float)_copies.Count, 0.5f, 1f));
			}
			MoveAll(dt);
		}

		private float Glide(float dt)
		{
			return 1f - Mathf.Exp(-3f * dt / Mathf.Max(0.02f, _glide));
		}

		private void MoveAll(float dt)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			bool flag = FindRing(out var centre, out var radii);
			if (flag)
			{
				if (!_ringSet)
				{
					_ringAt = centre;
					_ringSet = true;
				}
				else
				{
					_ringAt = _ringAt.Lerp(centre, Glide(dt));
				}
			}
			_ringRoom = Mathf.Max(3, Mathf.FloorToInt((float)Math.PI * 2f * Mathf.Min(radii.X, radii.Y) / (300f * _ringScale * 1.12f)));
			bool flag2 = false;
			for (int num = _copies.Count - 1; num >= 0; num--)
			{
				Copy copy = _copies[num];
				if (copy.Hit || copy.Launched)
				{
					continue;
				}
				if (copy.Node == null)
				{
					if ((C.Remote && (!_out || !flag)) || Time < copy.Born)
					{
						continue;
					}
					if (!Materialise(copy))
					{
						_copies.RemoveAt(num);
						flag2 = true;
						continue;
					}
				}
				else if (!FxKit.Alive((GodotObject)(object)copy.Node))
				{
					Drop(num);
					flag2 = true;
					continue;
				}
				Move(copy, dt, flag, centre, radii);
				Put(copy);
				Trail(copy, dt);
			}
			if (flag2)
			{
				Renumber();
			}
		}

		private void Renumber()
		{
			for (int i = 0; i < _copies.Count; i++)
			{
				_copies[i].Order = i;
			}
		}

		private bool FindRing(out Vector2 centre, out Vector2 radii)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			centre = _ringAt;
			radii = new Vector2(_ringMin + 40f, _ringMin + 40f);
			try
			{
				if (_aim && (Released || _phase != Phase.Standby || C.HasTarget))
				{
					centre = _aimAt;
					radii = Radii(_aimWidth, _aimHeight);
					return true;
				}
				if (C.Remote)
				{
					return false;
				}
				List<FxTarget> enemies = C.Enemies;
				if (enemies == null || enemies.Count == 0)
				{
					return false;
				}
				int index = 0;
				try
				{
					Vector2 globalMousePosition = ((CanvasItem)Front).GetGlobalMousePosition();
					float num = float.MaxValue;
					for (int i = 0; i < enemies.Count; i++)
					{
						if (enemies[i] != null)
						{
							Vector2 val = enemies[i].Chest - globalMousePosition;
							float num2 = val.LengthSquared();
							if (!(num2 >= num))
							{
								num = num2;
								index = i;
							}
						}
					}
				}
				catch
				{
					index = 0;
				}
				FxTarget fxTarget = enemies[index];
				if (fxTarget == null)
				{
					return false;
				}
				float num3 = Mathf.Max(60f, fxTarget.Width * Zoom().X);
				centre = fxTarget.Chest;
				radii = Radii(num3, num3 * 1.15f);
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoHeld.FindRing: " + ex.Message);
				return false;
			}
		}

		private Vector2 Zoom()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = C.FrontParent;
				CanvasItem val = (CanvasItem)(object)((frontParent is CanvasItem) ? frontParent : null);
				Vector2 val2;
				Transform2D globalTransform;
				Vector2 result;
				if (val == null || !FxKit.Alive((GodotObject)(object)val))
				{
					val2 = Vector2.One;
				}
				else
				{
					globalTransform = val.GetGlobalTransform();
					result = globalTransform.Scale;
					val2 = result.Abs();
				}
				Vector2 val3;
				if (!FxKit.Alive((GodotObject)(object)_layer) || !((Node)_layer).IsInsideTree())
				{
					val3 = Vector2.One;
				}
				else
				{
					globalTransform = ((CanvasItem)_layer).GetGlobalTransform();
					result = globalTransform.Scale;
					val3 = result.Abs();
				}
				Vector2 val4 = val3;
				float num = val2.X / Mathf.Max(0.01f, val4.X);
				float num2 = val2.Y / Mathf.Max(0.01f, val4.Y);
				if (num > 0.2f && num < 5f && num2 > 0.2f && num2 < 5f)
				{
					result = new Vector2(num, num2);
					return result;
				}
			}
			catch
			{
			}
			return Vector2.One;
		}

		private static Vector2 BoxHalf(float width, float height)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			return new Vector2(Mathf.Clamp(width * 0.5f + 6f, 90f, 240f), Mathf.Clamp(height * 0.5f + 6f, 100f, 250f));
		}

		private Vector2 Radii(float width, float height)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = BoxHalf(width, height);
			float num = 422f * _ringScale * 0.5f + 18f;
			return new Vector2(Mathf.Clamp(val.X + num, _ringMin, _ringMax), Mathf.Clamp(val.Y + num * 0.85f, _ringMin, _ringMax));
		}

		private Vector2 SlotOf(Copy copy, Vector2 centre, Vector2 radii)
		{
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			int num = Mathf.Max(1, _copies.Count);
			int num2 = num;
			int num3 = Mathf.Clamp(copy.Order, 0, num - 1);
			int num4 = 0;
			if (num > _ringRoom)
			{
				int num5 = num / 2;
				num4 = ((copy.Order % 2 == 0) ? 1 : 0);
				num2 = Mathf.Max(1, (num4 == 1) ? (num - num5) : num5);
				num3 = copy.Order / 2;
			}
			float num6 = -(float)Math.PI / 2f - (float)Math.PI * 2f * (float)num3 / (float)num2 - ((num4 == 1) ? ((float)Math.PI / (float)num2) : 0f);
			float num7 = 1f + 0.035f * Mathf.Sin(Time * 2.1f + (float)copy.Seed * 0.7f);
			float num8 = (float)num4 * 422f * _ringScale * 0.82f;
			float num9 = radii.X * num7 + num8;
			float num10 = radii.Y * num7 + num8;
			Vector2 val = centre + new Vector2(Mathf.Cos(num6) * num9, Mathf.Sin(num6) * num10);
			float num11 = 422f * _ringScale * 0.5f;
			val.X = Mathf.Clamp(val.X, num11, Mathf.Max(num11, _view.X - num11));
			val.Y = Mathf.Clamp(val.Y, num11, Mathf.Max(num11, _view.Y - num11));
			return val;
		}

		private void Home(Copy copy)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Invalid comparison between Unknown and I4
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Invalid comparison between Unknown and I4
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Clamp((Time - copy.Born) / Mathf.Max(0.02f, _rise), 0f, 1f);
			float num2 = 1f - (1f - num) * (1f - num) * (1f - num);
			if ((int)copy.Pile == 2)
			{
				HomeInHand(copy, num2);
				return;
			}
			bool flag = (int)copy.Pile == 1;
			Vector2 val = (flag ? _drawAt : _discardAt);
			float num3 = (flag ? 1f : (-1f));
			if (!copy.Puffed)
			{
				copy.Puffed = true;
				if (FxKit.Alive((GodotObject)(object)_under))
				{
					Shade.Puff((Node)(object)_under, val + new Vector2(num3 * 14f, -50f), 90f, 1.8f, 0.4f, 0.75f, 0.45f);
				}
			}
			float num4 = ((copy.Of > 1) ? Mathf.Min(0.14f, 0.6f / (float)(copy.Of - 1)) : 0f);
			float num5 = num3 * 0.08f + ((float)copy.Index - (float)(copy.Of - 1) * 0.5f) * num4;
			float num6 = _pileRise + 16f * FxKit.Hash(copy.Seed);
			Vector2 val2 = val + new Vector2(Mathf.Sin(num5), 0f - Mathf.Cos(num5)) * num6 + new Vector2(0f, Mathf.Sin(Time * 1.6f + (float)copy.Seed * 0.9f) * 6f);
			float num7 = 300f * _pileScale * 0.5f + 6f;
			val2.X = Mathf.Clamp(val2.X, num7, Mathf.Max(num7, _view.X - num7));
			float num8 = _pileScale * Mathf.Lerp(0.45f, 1f, num2);
			copy.HomePos = val.Lerp(val2, num2);
			copy.HomeRot = (num5 * 0.8f + Mathf.Sin(Time * 1.2f + (float)copy.Seed * 1.7f) * 0.03f) * num2;
			copy.HomeScale = new Vector2(num8, num8);
			copy.HomeAlpha = _pileAlpha * num2;
			copy.HomeKnown = true;
		}

		private void HomeInHand(Copy copy, float e)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Invalid comparison between Unknown and I4
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			NCard val = null;
			try
			{
				if (copy.Card != null)
				{
					CardPile pile = copy.Card.Pile;
					if (pile != null && (int)pile.Type == 2)
					{
						val = HandCards.NodeOf(copy.Card);
					}
				}
			}
			catch
			{
				val = null;
			}
			if (FxKit.Alive((GodotObject)(object)val) && ((Node)val).IsInsideTree())
			{
				CardStandIn.Pose(val, out var centre, out var rotation, out var scale);
				Vector2 val2 = new Vector2(Mathf.Sin(rotation), 0f - Mathf.Cos(rotation));
				copy.HomePos = centre + val2 * (_peel * e) + new Vector2(0f, Mathf.Sin(Time * 1.6f + (float)copy.Seed * 0.9f) * 3f * e);
				copy.HomeRot = rotation;
				copy.HomeScale = new Vector2(Mathf.Clamp(scale.X, 0.1f, 1.6f), Mathf.Clamp(scale.Y, 0.1f, 1.6f));
				copy.HomeAlpha = _handAlpha * e;
				copy.HomeKnown = true;
			}
			else
			{
				if (!copy.HomeKnown)
				{
					copy.HomePos = C.Hand;
					copy.HomeRot = 0f;
					copy.HomeScale = new Vector2(0.6f, 0.6f);
					copy.HomeKnown = true;
				}
				copy.HomeAlpha = 0f;
			}
		}

		private void Move(Copy copy, float dt, bool ring, Vector2 centre, Vector2 radii)
		{
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_0423: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			if (!C.Remote)
			{
				Home(copy);
			}
			bool flag = _out & ring;
			if (!copy.Moved)
			{
				copy.Moved = true;
				copy.WantOut = flag;
				copy.Wait = 0f;
			}
			else if (flag != copy.WantOut)
			{
				copy.WantOut = flag;
				copy.Wait = (float)copy.Order * _stagger;
			}
			if (copy.Wait > 0f)
			{
				copy.Wait -= dt;
			}
			else
			{
				copy.Out = Mathf.MoveToward(copy.Out, copy.WantOut ? 1f : 0f, dt / Mathf.Max(0.02f, _fly));
			}
			if (ring && (!C.Remote | flag))
			{
				Vector2 val = SlotOf(copy, centre, radii);
				if (!copy.SlotKnown || copy.Out <= 0.001f)
				{
					copy.Slot = val;
					copy.SlotKnown = true;
				}
				else
				{
					copy.Slot = copy.Slot.Lerp(val, Glide(dt));
				}
			}
			else if (!copy.SlotKnown)
			{
				copy.Slot = copy.HomePos;
			}
			float num = Mathf.Clamp(copy.Out, 0f, 1f);
			float num2 = 1f - (1f - num) * (1f - num) * (1f - num);
			Vector2 val2 = _ringAt - copy.Slot;
			float num3 = ((val2.LengthSquared() > 4f) ? (val2.Angle() + (float)Math.PI / 2f) : copy.HomeRot);
			float num4 = (float)copy.Seed * 1.31f;
			float num5 = Mathf.Clamp(copy.Coil, 0f, 1f);
			float num6 = num2 * (1f + 0.8f * Mathf.Sin((float)Math.PI * num5)) * (1f - 0.8f * num5 * num5);
			Vector2 val3 = new Vector2(Wobble(Time * 43f + num4), Wobble(Time * 57f + num4 * 1.7f)) * (_trembleMove * num6);
			float num7 = Wobble(Time * 49f + num4 * 2.3f) * _trembleTurn * num6;
			if (C.Remote)
			{
				if (flag)
				{
					copy.Pos = copy.Slot + val3;
					copy.Rot = num3 + num7;
				}
				copy.Scale = new Vector2(_ringScale, _ringScale);
				copy.Alpha = _ringAlpha * Mathf.Min(1f, num2 * 2f);
			}
			else
			{
				copy.Pos = copy.HomePos.Lerp(copy.Slot, num2) + new Vector2(0f, (0f - Mathf.Sin((float)Math.PI * num2)) * 34f) + val3;
				copy.Rot = Mathf.LerpAngle(copy.HomeRot, num3, num2) + num7;
				copy.Scale = copy.HomeScale.Lerp(new Vector2(_ringScale, _ringScale), num2);
				copy.Alpha = Mathf.Lerp(copy.HomeAlpha, _ringAlpha, Mathf.Min(1f, num2 * 2f));
			}
			if (!(num5 <= 0f))
			{
				float num8 = 1f - (1f - num5) * (1f - num5);
				if (val2.LengthSquared() > 4f)
				{
					copy.Pos -= val2.Normalized() * (_drawBack * copy.Reach * num8 * num2);
				}
				float num9 = 1f + 0.1f * Mathf.Max(0f, copy.Reach - 1f);
				copy.Scale = new Vector2(copy.Scale.X * (1f + 0.07f * num8) * num9, copy.Scale.Y * (1f - 0.1f * num8) * num9);
			}
		}

		private static void Trail(Copy copy, float dt)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)copy.Streak))
			{
				return;
			}
			copy.Tail = copy.Tail.Lerp(copy.Pos, 1f - Mathf.Exp(-13f * dt));
			Vector2 val = copy.Pos - copy.Tail;
			float num = val.Length();
			float num2 = Mathf.Clamp((num - 16f) / 70f, 0f, 1f) * Mathf.Clamp(copy.Alpha, 0f, 1f);
			((CanvasItem)copy.Streak).Visible = num2 > 0.01f;
			if (((CanvasItem)copy.Streak).Visible)
			{
				copy.Streak.GlobalPosition = (copy.Pos + copy.Tail) * 0.5f;
				copy.Streak.GlobalRotation = val.Angle();
				if (FxKit.Alive((GodotObject)(object)copy.StreakDark))
				{
					((Node2D)copy.StreakDark).Scale = new Vector2(num * 1.35f, 36f) / 64f;
					((CanvasItem)copy.StreakDark).Modulate = Mist(0.85f * num2);
				}
				if (FxKit.Alive((GodotObject)(object)copy.StreakLight))
				{
					((Node2D)copy.StreakLight).Scale = new Vector2(num * 1.2f, 10f) / 64f;
					((CanvasItem)copy.StreakLight).Modulate = Lit(0.5f * num2);
				}
			}
		}

		private void TickStacks(float dt)
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			if (!C.Remote)
			{
				_stackShow = FxKit.Damp(_stackShow, (_phase == Phase.Standby && !_out) ? 1f : 0f, 8f, dt);
				StackTick(_drawMist, _drawAt, 1f, _drawCount);
				StackTick(_discardMist, _discardAt, -1f, _discardCount);
			}
		}

		private void StackTick(ShadeMist mist, Vector2 pile, float side, int count)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			if (mist != null && FxKit.Alive((GodotObject)(object)mist.Root))
			{
				mist.Root.GlobalPosition = pile + new Vector2(side * 20f, 0f - (_pileRise - 70f));
				mist.Size(230f + 12f * (float)count, 124f + 16f * Mathf.Sin(Time * 1.4f + side));
				mist.Intensity = ((count > 0) ? (0.8f * _stackShow * Mathf.Min(1f, Time * 4f)) : 0f);
			}
		}

		private static Vector2[] Diamond(float halfWidth, float halfHeight)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			return new Vector2[4]
			{
				new Vector2(0f, 0f - halfHeight),
				new Vector2(halfWidth, 0f),
				new Vector2(0f, halfHeight),
				new Vector2(0f - halfWidth, 0f)
			};
		}

		private static Vector2 CornerArms(float halfWidth, float halfHeight)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			float num = halfWidth * 2f * 0.23f;
			float num2 = halfHeight * 2f * 0.23f;
			float num3 = Mathf.Min(Mathf.Min(num, num2) * 1.75f, 92f);
			return new Vector2(Mathf.Min(num, num3), Mathf.Min(num2, num3));
		}

		private void BuildFrame()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected Obj, but got Unknown
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected Obj, but got Unknown
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Expected Obj, but got Unknown
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			_frame = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_MementoFrame"),
				Modulate = new Color(1f, 1f, 1f, 0f),
				Visible = false
			};
			((Node)_layer).AddChild((Node)(object)_frame, false, (InternalMode)0);
			float num = Mathf.Clamp(Tuning.F("memento.rule_width", 2f), 1f, 6f);
			for (int i = 0; i < 4; i++)
			{
				Node2D val = new Node2D();
				((Node)_frame).AddChild((Node)(object)val, false, (InternalMode)0);
				_corners[i] = val;
				Sprite2D val2 = new Sprite2D
				{
					Texture = FxKit.SoftDot,
					Modulate = Mist(0.42f)
				};
				((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
				_grounds[i * 2] = val2;
				Sprite2D val3 = FxKit.Glow(Lit(0.22f), new Vector2(64f, 64f));
				((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
				_grounds[i * 2 + 1] = val3;
				for (int j = 0; j < 4; j++)
				{
					Line2D val4 = ((j < 2) ? Props.Line(num * 3.4f, Lit(0.3f), true) : Props.Line(num, Rule(0.95f), false));
					((CanvasItem)val4).Visible = false;
					((Node)val).AddChild((Node)(object)val4, false, (InternalMode)0);
					_rules[i * 4 + j] = val4;
				}
				((Node)val).AddChild((Node)(object)FxKit.Shape(Diamond(6f, 6f), Rule(1f)), false, (InternalMode)0);
			}
			_armsDrawn = Vector2.Zero;
		}

		private void FitCorners(Vector2 arms)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			if (Mathf.Abs(arms.X - _armsDrawn.X) < 0.2f && Mathf.Abs(arms.Y - _armsDrawn.Y) < 0.2f)
			{
				return;
			}
			_armsDrawn = arms;
			Vector2[] array = new Vector2[2]
			{
				new Vector2(11f, 0f),
				new Vector2(Mathf.Max(11f, arms.X), 0f)
			};
			Vector2[] array2 = new Vector2[2]
			{
				new Vector2(0f, 11f),
				new Vector2(0f, Mathf.Max(11f, arms.Y))
			};
			bool flag = arms.X > 12f;
			bool flag2 = arms.Y > 12f;
			float num = Mathf.Max(11f, Mathf.Max(arms.X, arms.Y));
			Vector2 position = arms * 0.22f;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					Line2D val = _rules[i * 4 + j];
					if (FxKit.Alive((GodotObject)(object)val))
					{
						bool flag3 = (j & 1) == 0;
						val.Points = (flag3 ? array : array2);
						((CanvasItem)val).Visible = (flag3 ? flag : flag2);
					}
				}
				Sprite2D val2 = _grounds[i * 2];
				if (FxKit.Alive((GodotObject)(object)val2))
				{
					((Node2D)val2).Position = position;
					((Node2D)val2).Scale = new Vector2(num * 2.6f, num * 2.6f) / 64f;
				}
				Sprite2D val3 = _grounds[i * 2 + 1];
				if (FxKit.Alive((GodotObject)(object)val3))
				{
					((Node2D)val3).Position = position;
					((Node2D)val3).Scale = new Vector2(num * 2f, num * 2f) / 64f;
				}
			}
		}

		private void Aim(CardFxContext victim)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (victim != null && victim.HasTarget)
			{
				bool num = !_aim || _aimTarget != victim.Target || _aimAt.DistanceTo(victim.TargetChest) > 6f || _frameShow < 0.15f;
				Vector2 val = Zoom();
				_aim = true;
				_aimTarget = victim.Target;
				_aimAt = victim.TargetChest;
				_aimWidth = Mathf.Max(60f, victim.TargetWidth * val.X);
				_aimHeight = Mathf.Max(60f, victim.TargetHeight * val.Y);
				_boxHalf = BoxHalf(_aimWidth, _aimHeight);
				_arms = CornerArms(_boxHalf.X, _boxHalf.Y);
				_cornerSize = Mathf.Clamp(Mathf.Min(_boxHalf.X, _boxHalf.Y) / 120f, 0.8f, 1.25f);
				if (num)
				{
					_close = 0f;
					_puffDue = true;
				}
			}
		}

		public override void OnTargetChanged()
		{
			try
			{
				if (_phase == Phase.Standby && C != null && C.HasTarget)
				{
					Aim(C);
				}
				else if (_phase == Phase.Standby)
				{
					CardFxContext c = C;
					if (c != null && c.Remote)
					{
						_aim = false;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoHeld.OnTargetChanged: " + ex.Message);
			}
		}

		private void TickFrame(float dt)
		{
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)_frame))
			{
				return;
			}
			_puffWait = Mathf.Max(0f, _puffWait - dt);
			bool flag = _aim && !_frameOff && (Released || C.HasTarget);
			_frameShow = FxKit.Damp(_frameShow, flag ? 1f : 0f, 10f, dt);
			if (flag)
			{
				_close = Mathf.Min(1f, _close + dt / Mathf.Max(0.05f, Tuning.F("memento.frame_close", 0.5f)));
			}
			float num = 1f - (1f - _close) * (1f - _close) * (1f - _close);
			float num2 = Mathf.Clamp(Tuning.F("memento.frame_open", 1.35f), 1f, 2.5f);
			_flinch = FxKit.Damp(_flinch, 0f, 16f, dt);
			float num3 = Mathf.Lerp(num2, 1f, num) * (1f + 0.02f * num * Mathf.Sin(Time * 2.2f)) * (1f + _flinch);
			float num4 = Mathf.Clamp(Arm, 0f, 1f) * _frameShow * Mathf.Min(1f, _close * 3f) * (0.9f + 0.1f * Mathf.Sin(Time * 2.2f + 1f));
			float num5 = Mathf.Lerp(0.4f, 1f, num);
			if (_snap >= 0f)
			{
				_snap = Mathf.Min(1f, _snap + dt / Mathf.Max(0.03f, _snapTime));
				float num6 = Mathf.Lerp(1f, 0.12f, _snap * _snap);
				num3 *= num6;
				num5 *= num6;
				num4 *= 1f - Mathf.Clamp((_snap - 0.7f) / 0.3f, 0f, 1f);
				if (_snap >= 1f)
				{
					_frameOff = true;
				}
			}
			((CanvasItem)_frame).Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(num4, 0f, 1f));
			((CanvasItem)_frame).Visible = num4 > 0.004f;
			if (!((CanvasItem)_frame).Visible)
			{
				return;
			}
			_frame.GlobalPosition = _aimAt;
			float num7 = _boxHalf.X * num3;
			float num8 = _boxHalf.Y * num3;
			float cornerSize = _cornerSize;
			FitCorners(_arms * (num5 / Mathf.Max(0.1f, cornerSize)));
			bool flag2 = _puffDue && _puffWait <= 0f && _snap < 0f && num4 > 0.3f && FxKit.Alive((GodotObject)(object)_under);
			for (int i = 0; i < 4; i++)
			{
				Node2D val = _corners[i];
				if (FxKit.Alive((GodotObject)(object)val))
				{
					float num9 = (((i & 1) == 0) ? (-1f) : 1f);
					float num10 = ((i < 2) ? (-1f) : 1f);
					val.Position = new Vector2(num9 * num7, num10 * num8);
					val.Scale = new Vector2((0f - num9) * cornerSize, (0f - num10) * cornerSize);
					if (flag2)
					{
						Shade.Puff((Node)(object)_under, _aimAt + new Vector2(num9 * num7, num10 * num8), 64f, 1.6f, 0.4f, 0.55f, 0.35f);
					}
				}
			}
			if (flag2)
			{
				_puffDue = false;
				_puffWait = 0.35f;
			}
		}

		protected override void OnPlay(CardFxContext play)
		{
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (_phase != Phase.Standby)
				{
					return;
				}
				_decoOut = true;
				CardFxContext cardFxContext = ((play != null && play.HasTarget) ? play : C);
				if (C.Remote && (cardFxContext == null || !cardFxContext.HasTarget))
				{
					_aim = false;
				}
				if (cardFxContext != null && cardFxContext.HasTarget)
				{
					Aim(cardFxContext);
				}
				if (!_aim)
				{
					BeginDissolve();
					return;
				}
				_out = true;
				for (int num = _copies.Count - 1; num >= 0; num--)
				{
					Copy copy = _copies[num];
					if (copy.Node == null)
					{
						copy.Born = Mathf.Min(copy.Born, Time);
						copy.Puffed = true;
						if (!Materialise(copy))
						{
							_copies.RemoveAt(num);
							continue;
						}
					}
					copy.Wait = Mathf.Min(copy.Wait, (float)copy.Order * 0.012f);
				}
				Renumber();
				_strikeAt = _aimAt;
				_hitUnit = ((cardFxContext != null && cardFxContext.HasTarget) ? Mathf.Clamp(Mathf.Max(cardFxContext.TargetWidth, cardFxContext.TargetHeight) / 260f, 1f, 2.2f) : 1f);
				TakeBody();
				Schedule(cardFxContext?.Chest ?? (_aimAt + new Vector2(-1f, 0f)));
				_phase = Phase.Barrage;
				_clock = 0f;
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoHeld.OnPlay: " + ex);
				Finish(14f);
			}
		}

		private void Schedule(Vector2 her)
		{
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			int num = (_slamCount = _copies.Count);
			_landed = 0;
			_heavy = null;
			_finale = false;
			_lead = Mathf.Clamp(Tuning.F("memento.windup", 0.05f), 0.02f, 0.5f);
			_travel = Mathf.Clamp(Tuning.F("memento.drive", 0.06f), 0.03f, 0.3f);
			_beat = Mathf.Clamp(Tuning.F("memento.beat", 0f), 0f, 0.6f);
			_drawBack = Mathf.Clamp(Tuning.F("memento.draw_back", 46f), 0f, 160f);
			if (num == 0)
			{
				return;
			}
			Vector2 val = her - _aimAt;
			val = (Vector2)((val.LengthSquared() > 4f) ? val.Normalized() : new Vector2(-1f, 0f));
			Vector2 radii = Radii(_aimWidth, _aimHeight);
			float num2 = float.MinValue;
			foreach (Copy copy in _copies)
			{
				Vector2 val2 = SlotOf(copy, _aimAt, radii) - _aimAt;
				float num3;
				if (!(val2.LengthSquared() > 4f))
				{
					num3 = -2f;
				}
				else
				{
					Vector2 val3 = val2.Normalized();
					num3 = val3.Dot(val);
				}
				float num4 = num3;
				if (!(num4 <= num2))
				{
					num2 = num4;
					_heavy = copy;
				}
			}
			List<Copy> list = new List<Copy>(num);
			foreach (Copy copy2 in _copies)
			{
				if (copy2 != _heavy)
				{
					list.Add(copy2);
				}
			}
			int num5 = (_signature % 7919 + 7919) % 7919 + (int)(Mathf.Max(0f, Time) * 61f) % 997;
			for (int num6 = list.Count - 1; num6 > 0; num6--)
			{
				int index = Mathf.Clamp((int)(FxKit.Hash(num5 + num6 * 31) * (float)(num6 + 1)), 0, num6);
				Copy value = list[num6];
				list[num6] = list[index];
				list[index] = value;
			}
			int count = list.Count;
			float num7 = Mathf.Clamp(Tuning.F("memento.barrage", 0.08f), 0.02f, 1.5f);
			float num8 = ((count > 1) ? Mathf.Clamp(num7 / (float)(count - 1), 0.006f, 0.02f) : 0f);
			float num9 = _lead;
			for (int i = 0; i < count; i++)
			{
				float num10 = ((count > 1) ? ((float)i / (float)(count - 1)) : 0f);
				Ready(list[i], num9, 0.6f * num10, 1f + 0.35f * num10, num5 + i * 53);
				num9 += num8 * (1.3f - 0.6f * num10) * (0.7f + 0.6f * FxKit.Hash(num5 + i * 53 + 11));
			}
			if (_heavy != null)
			{
				float num11 = ((count > 0) ? (list[count - 1].LaunchAt + 0.012f + _beat) : 0f);
				Ready(_heavy, Mathf.Max(_lead * 1.6f, num11), 1f, 1.35f, num5 + 977);
				_heavy.Heavy = true;
				_heavy.Spot = Vector2.Zero;
			}
		}

		private void Ready(Copy copy, float launchAt, float weight, float reach, int salt)
		{
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			copy.LaunchAt = launchAt;
			copy.Weight = weight;
			copy.Reach = reach;
			copy.Heavy = false;
			copy.Launched = false;
			copy.Hit = false;
			copy.Flight = 0f;
			copy.Coil = 0f;
			Vector2 val = new Vector2(Mathf.Clamp((FxKit.Hash(salt + 3) - 0.5f) * _aimWidth * 0.44f, -170f, 170f), Mathf.Clamp((FxKit.Hash(salt + 7) - 0.5f) * _aimHeight * 0.36f, -120f, 120f));
			Vector2 val2 = SlotOf(copy, _aimAt, Radii(_aimWidth, _aimHeight)) - _aimAt;
			copy.Spot = ((val2.Dot(val) > 0f) ? (-val) : val);
		}

		private void TickBarrage(float dt)
		{
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			_out = true;
			float num = Mathf.Clamp(Tuning.F("memento.wait", 1.6f), 0.3f, 2.8f);
			Node parent = (Node)(FxKit.Alive((GodotObject)(object)C.FrontParent) ? ((object)C.FrontParent) : ((object)Front));
			if (_slamCount == 0)
			{
				if (!_struck)
				{
					if (_clock >= num)
					{
						BeginDissolve();
					}
					return;
				}
				if (_snap < 0f)
				{
					_snap = 0f;
				}
				if (!(_struckFor < 0.1f))
				{
					Finale(parent, _strikeAt, new Vector2(1f, 0f), _strikeAt, card: false);
					_phase = Phase.Over;
					Leave(8f);
				}
				return;
			}
			bool flag = false;
			foreach (Copy copy2 in _copies)
			{
				if (!copy2.Hit && !copy2.Launched && !copy2.Heavy)
				{
					flag = true;
				}
			}
			float num2 = dt / Mathf.Max(0.02f, _lead);
			foreach (Copy copy3 in _copies)
			{
				if (!copy3.Hit && !copy3.Launched)
				{
					copy3.Coil = Mathf.MoveToward(copy3.Coil, 1f, num2);
					if (copy3.Heavy && !flag)
					{
						copy3.Reach = Mathf.MoveToward(copy3.Reach, 2.3f, dt * 1.4f / Mathf.Max(0.08f, _beat + _travel));
					}
				}
			}
			MoveAll(dt);
			if (_heavy != null && !_copies.Contains(_heavy))
			{
				_heavy = null;
			}
			bool flag2 = false;
			for (int i = 0; i < _copies.Count; i++)
			{
				Copy copy = _copies[i];
				if (copy.Hit)
				{
					continue;
				}
				if (!FxKit.Alive((GodotObject)(object)copy.Node))
				{
					Unmake(copy);
					copy.Hit = true;
					continue;
				}
				if (!copy.Launched)
				{
					if ((copy.Heavy && (flag || !_struck)) || _clock < copy.LaunchAt)
					{
						flag2 = true;
						continue;
					}
					Launch(copy);
				}
				if (!Drive(copy, dt, parent))
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				if (!_struck && _clock >= num)
				{
					BeginDissolve();
				}
				return;
			}
			if (!_finale)
			{
				if (_snap < 0f)
				{
					_snap = 0f;
				}
				Finale(parent, _strikeAt, new Vector2(1f, 0f), _strikeAt, card: false);
			}
			FreeCopies();
			_phase = Phase.Over;
			Leave(8f);
		}

		private static void Hold(Copy copy)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			copy.FromPos = copy.Pos;
			copy.FromRot = copy.Rot;
			copy.FromScale = copy.Scale;
			copy.FromAlpha = copy.Alpha;
		}

		private void Launch(Copy copy)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			Hold(copy);
			copy.Launched = true;
			copy.Flight = 0f;
			copy.Coil = 1f;
			copy.Tail = copy.Pos;
			if (copy.Heavy && _snap < 0f)
			{
				_snapTime = Mathf.Max(0.05f, _travel * 1.2f);
				_snap = 0f;
			}
			if (FxKit.Alive((GodotObject)(object)_under))
			{
				FxKit.Flash((Node)(object)_under, copy.Pos, Ghost(copy.Heavy ? 0.5f : 0.3f), copy.Heavy ? 130f : 76f, 1.7f, 0.14);
			}
		}

		private bool Drive(Copy copy, float dt, Node parent)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Max(0.02f, copy.Heavy ? (_travel * 1.2f) : _travel);
			copy.Flight += dt;
			float num2 = Mathf.Clamp(copy.Flight / num, 0f, 1f);
			float num3 = num2 * num2 * num2;
			Vector2 val = _strikeAt + copy.Spot;
			Vector2 val2 = val - copy.FromPos;
			float num4 = val2.Length();
			Vector2 dir = (Vector2)((num4 > 1f) ? (val2 / num4) : new Vector2(0f, 1f));
			float num5 = ((num4 > 1f) ? (val2.Angle() + (float)Math.PI / 2f) : copy.FromRot);
			float num6 = Mathf.Min(1f, num2 * num2 * 1.5f);
			copy.Pos = copy.FromPos.Lerp(val, num3);
			copy.Rot = Mathf.LerpAngle(copy.FromRot, num5, Mathf.Min(1f, num2 * 4f));
			copy.Scale = new Vector2(copy.FromScale.X * Mathf.Lerp(1f, 0.72f, num6), copy.FromScale.Y * Mathf.Lerp(1f, 1.5f, num6));
			copy.Alpha = Mathf.Lerp(copy.FromAlpha, 1f, Mathf.Min(1f, 0.5f + num2));
			copy.Coil = 1f;
			copy.Tail = copy.Pos;
			Put(copy);
			DriveTrail(copy, num3);
			if (num2 < 1f)
			{
				return false;
			}
			Impact(copy, val, dir, parent);
			return true;
		}

		private static void DriveTrail(Copy copy, float e)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)copy.Streak))
			{
				return;
			}
			Vector2 val = copy.FromPos.Lerp(copy.Pos, Mathf.Clamp(e - 0.5f, 0f, 0.35f));
			Vector2 val2 = copy.Pos - val;
			float num = val2.Length();
			((CanvasItem)copy.Streak).Visible = num > 6f;
			if (((CanvasItem)copy.Streak).Visible)
			{
				copy.Streak.GlobalPosition = (copy.Pos + val) * 0.5f;
				copy.Streak.GlobalRotation = val2.Angle();
				float num2 = 300f * Mathf.Max(0.05f, copy.Scale.X);
				float num3 = Mathf.Clamp(copy.Alpha, 0f, 1f) * Mathf.Min(1f, num / 60f);
				if (FxKit.Alive((GodotObject)(object)copy.StreakDark))
				{
					((Node2D)copy.StreakDark).Scale = new Vector2(num * 1.35f, num2 * 0.9f) / 64f;
					((CanvasItem)copy.StreakDark).Modulate = Mist(0.55f * num3);
				}
				if (FxKit.Alive((GodotObject)(object)copy.StreakLight))
				{
					((Node2D)copy.StreakLight).Scale = new Vector2(num * 1.3f, num2 * (copy.Heavy ? 0.7f : 0.5f)) / 64f;
					((CanvasItem)copy.StreakLight).Modulate = Ghost((copy.Heavy ? 1f : 0.8f) * num3);
				}
			}
		}

		private void Impact(Copy copy, Vector2 at, Vector2 dir, Node parent)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			Vector2 fromPos = copy.FromPos;
			bool heavy = copy.Heavy;
			float weight = copy.Weight;
			int seed = copy.Seed;
			copy.Hit = true;
			Unmake(copy);
			_landed++;
			try
			{
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					if (heavy)
					{
						Finale(parent, at, dir, fromPos, card: true);
					}
					else
					{
						Blow(parent, at, dir, fromPos, weight, seed);
					}
				}
				Jolt(dir, heavy ? 1f : (0.26f + 0.3f * weight));
				if (!heavy)
				{
					_flinch = Mathf.Max(_flinch, 0.04f + 0.05f * weight);
					if ((_landed - 1) % 3 == 0 && Tuning.F("memento.shake", 1f) >= 0.5f)
					{
						GameRefs.Shake(1);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoHeld.Impact: " + ex.Message);
			}
		}

		private float Through(Vector2 dir)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Max(30f, _aimWidth * 0.5f);
			float num2 = Mathf.Max(30f, _aimHeight * 0.5f);
			float num3 = dir.X * dir.X / (num * num) + dir.Y * dir.Y / (num2 * num2);
			if (!(num3 > 1E-07f))
			{
				return num;
			}
			return Mathf.Clamp(1f / Mathf.Sqrt(num3), 30f, 420f);
		}

		private void Blow(Node parent, Vector2 at, Vector2 dir, Vector2 from, float weight, int seed)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			float hitUnit = _hitUnit;
			float num = Mathf.Clamp(Tuning.F("memento.blow", 1f), 0.3f, 2f);
			float num2 = (62f + 34f * weight) * hitUnit * num;
			FxKit.Flash(parent, at, Ghost(0.85f), num2, 2.1f, 0.15);
			FxKit.Flash(parent, at, Pale(0.9f), num2 * 0.42f, 1.7f, 0.09);
			FxKit.RingPulse(parent, at, (150f + 70f * weight) * hitUnit * num, Ghost(1f), 0f, 0.24f, 0.18f, 1f, 1f, 0f, 0.035f, 0.75f);
			Brush.Cut(parent, from, at, 18f * hitUnit, Ghost(0.6f), 0f, 0.01f, 0.02f, 0.15f);
			float num3 = Mathf.Min(480f, Through(dir) + 70f + 70f * FxKit.Hash(seed + 21));
			Shards(parent, at, dir, (weight > 0.3f) ? 6 : 5, num3, 0.4f, 11f * Mathf.Min(hitUnit, 1.6f), 0.3f, seed);
			for (int i = 0; i < 2; i++)
			{
				float num4 = ((i == 0) ? (-1f) : 1f) * (0.1f + 0.22f * FxKit.Hash(seed * 3 + i * 19 + 1));
				Vector2 val = dir.Rotated(num4);
				float num5 = num3 * (0.75f + 0.45f * FxKit.Hash(seed * 5 + i * 23 + 2));
				Brush.Dart(parent, at, at + val * num5, num5 * 0.55f, 12f * hitUnit, Ghost(0.6f), 0.15f + 0.05f * (float)i);
			}
		}

		private void Finale(Node parent, Vector2 at, Vector2 dir, Vector2 from, bool card)
		{
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0366: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0434: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			if (_finale)
			{
				return;
			}
			_finale = true;
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			int slamCount = _slamCount;
			if (slamCount > 0)
			{
				Sfx.Play("memento_slam", Mathf.Lerp(0.7f, 1.1f, Mathf.Clamp((float)slamCount / 6f, 0f, 1f)));
			}
			float num = Mathf.Clamp(Tuning.F("memento.payoff", 1f), 0.3f, 2f);
			if (!C.Remote && _aim)
			{
				Shatter(parent, _aimAt, _boxHalf.X, _boxHalf.Y, Mathf.Clamp(slamCount, 0, 8));
			}
			if (!card)
			{
				FxKit.Flash(parent, at, Lit(0.6f), 80f * num, 1.9f, 0.2);
				Shade.Puff(parent, at, 110f * num, 1.7f, 0.36f, 0.7f, 0.5f);
				return;
			}
			int num2 = Mathf.Clamp(slamCount, 1, 12);
			float hitUnit = _hitUnit;
			float body = (140f + 20f * (float)num2) * num;
			Shade.Puff(parent, at, body, 2f, 0.44f, 0.95f, 0.7f);
			FxKit.RingPulse(parent, at, (270f + 34f * (float)num2) * num, Ghost(1f), 0f, 0.4f, 0.2f, 1f, 1f, 0f, 0.05f, 0.95f);
			FxKit.RingPulse(parent, at, (200f + 26f * (float)num2) * num, new Color(RimColor.R, RimColor.G, RimColor.B, 1f), 0.05f, 0.36f, 0.2f, 1f, 1f, 0f, 0.025f, 0.6f);
			FxKit.Flash(parent, at, Ghost(0.95f), (96f + 14f * (float)num2) * num, 2.3f, 0.26);
			FxKit.Flash(parent, at, Pale(0.9f), (44f + 5f * (float)num2) * num, 1.8f, 0.12);
			Brush.Cut(parent, from, at, 34f * hitUnit, Ghost(0.85f), 0f, 0.01f, 0.03f);
			float num3 = Mathf.Min(560f, Through(dir) + 150f);
			Shards(parent, at, dir, Mathf.Clamp(9 + num2, 10, 20), num3 * 1.25f, 0.55f, 15f * Mathf.Min(hitUnit, 1.6f), 0.46f, 977);
			for (int i = 0; i < 4; i++)
			{
				float num4 = ((float)i - 1.5f) * 0.17f + (FxKit.Hash(i * 17 + 5) - 0.5f) * 0.1f;
				Vector2 val = dir.Rotated(num4);
				float num5 = num3 * (0.9f + 0.5f * FxKit.Hash(i * 7 + 2));
				Brush.Dart(parent, at, at + val * num5, num5 * 0.6f, (20f - 2f * (float)i) * hitUnit, Ghost(0.75f), 0.2f + 0.03f * (float)i, 0.01f * (float)i);
			}
			FxKit.Burst(parent, at, Ghost(0.85f), 12 + num2, 260f, 880f + 30f * (float)num2, 0.5, 0.08f, 0.24f, new Vector2(0f, -60f), 26f, dir, 14f, 1100f, additive: true, 1f);
			Vector2 beyond = at + dir * Mathf.Min(num3 * 0.35f, 140f);
			FxKit.After(parent, 0.1, () =>
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				Shade.Puff(parent, beyond, body * 1.15f, 1.4f, 0.8f, 0.55f, 0.45f);
			});
			if (Tuning.F("memento.shake", 1f) >= 0.5f)
			{
				GameRefs.Shake((num2 >= 8) ? 3 : ((num2 < 4) ? 1 : 2));
			}
		}

		private static void Shards(Node parent, Vector2 at, Vector2 dir, int count, float reach, float cone, float size, float time, int seed)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected Obj, but got Unknown
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent) || count <= 0)
			{
				return;
			}
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_MementoSlivers")
			};
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return;
			}
			val.GlobalPosition = at;
			Transform2D globalTransform = ((CanvasItem)val).GetGlobalTransform();
			Vector2 scale = globalTransform.Scale;
			Vector2 val2 = scale.Abs();
			float num = ((val2.X > 0.2f && val2.X < 5f) ? (1f / val2.X) : 1f);
			float num2 = dir.Angle();
			Polygon2D[] slivers = new Polygon2D[count];
			Vector2[] ways = new Vector2[count];
			float[] turns = new float[count];
			float[] spins = new float[count];
			for (int i = 0; i < count; i++)
			{
				int num3 = seed * 13 + i * 29;
				float num4 = num2 + (FxKit.Hash(num3 + 1) - 0.5f) * 2f * cone;
				float num5 = reach * num * (0.45f + 0.85f * FxKit.Hash(num3 + 2));
				ways[i] = new Vector2(Mathf.Cos(num4), Mathf.Sin(num4)) * num5;
				turns[i] = num4 + (float)Math.PI / 2f;
				spins[i] = (FxKit.Hash(num3 + 3) - 0.5f) * 3.2f;
				float num6 = size * (0.6f + 0.8f * FxKit.Hash(num3 + 4));
				Polygon2D val3 = FxKit.Shape(Diamond(Mathf.Max(1.2f, num6 * 0.22f), num6), (i % 3 == 0) ? Pale(1f) : Ghost(0.95f));
				((CanvasItem)val3).Material = (Material)(object)FxKit.Additive;
				((Node2D)val3).Rotation = turns[i];
				((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
				slivers[i] = val3;
			}
			FxKit.Drive((Node)(object)val, time, (float u) =>
			{
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				float num7 = 1f - (1f - u) * (1f - u) * (1f - u);
				for (int j = 0; j < slivers.Length; j++)
				{
					Polygon2D val4 = slivers[j];
					if (FxKit.Alive((GodotObject)(object)val4))
					{
						((Node2D)val4).Position = ways[j] * num7;
						((Node2D)val4).Rotation = turns[j] + spins[j] * u;
						((Node2D)val4).Scale = new Vector2(1f - 0.5f * u, 1f + 1.6f * (1f - u) * (1f - u));
						((CanvasItem)val4).Modulate = new Color(1f, 1f, 1f, (u < 0.45f) ? 1f : (1f - (u - 0.45f) / 0.55f));
					}
				}
			}, (TransitionType)0, (EaseType)2);
			FxKit.FreeAfter((Node)(object)val, (double)time + 0.1);
		}

		public void Strike(Vector2 at)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (_struck || Finished || _phase == Phase.Over)
				{
					return;
				}
				_struck = true;
				_struckFor = 0f;
				_decoOut = true;
				if (_phase == Phase.Dissolve)
				{
					return;
				}
				if (_phase == Phase.Standby)
				{
					OnPlay(null);
					if (_phase != Phase.Barrage)
					{
						return;
					}
				}
				Vector2 val = at - _strikeAt;
				if (val.LengthSquared() < 14400f)
				{
					_strikeAt = at;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoHeld.Strike: " + ex);
				Finish(10f);
			}
		}

		private void TakeBody()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			_victim = null;
			_joltAt = Vector2.Zero;
			_joltSpeed = Vector2.Zero;
			try
			{
				if (Tuning.F("memento.jolt", 1f) <= 0.01f)
				{
					return;
				}
				object aimTarget = _aimTarget;
				Creature val = (Creature)((aimTarget is Creature) ? aimTarget : null);
				if (val == null)
				{
					return;
				}
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				NCreature val2 = ((combatRoom != null) ? combatRoom.GetCreatureNode(val) : null);
				if (FxKit.Alive((GodotObject)(object)val2) && ((Node)val2).IsInsideTree())
				{
					Node2D body = val2.Body;
					if (FxKit.Alive((GodotObject)(object)body))
					{
						_victim = body;
						_victimHome = body.Position;
						_victimWrote = _victimHome;
						_victimMoved = Vector2.Zero;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoHeld.TakeBody: " + ex.Message);
				_victim = null;
			}
		}

		private void Jolt(Vector2 dir, float strength)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			if (_victim != null)
			{
				float num = Mathf.Clamp(Tuning.F("memento.jolt", 1f), 0f, 3f);
				_joltSpeed += dir * (950f * strength * num * Mathf.Clamp(_hitUnit, 1f, 1.8f));
			}
		}

		private void TickJolt(float dt)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			if (_victim == null)
			{
				return;
			}
			if (_joltAt == Vector2.Zero && _joltSpeed == Vector2.Zero)
			{
				if (_victimMoved != Vector2.Zero)
				{
					Nudge(Vector2.Zero);
				}
				return;
			}
			int num = Mathf.Clamp(Mathf.CeilToInt(dt / 0.006f), 1, 24);
			float num2 = dt / (float)num;
			for (int i = 0; i < num; i++)
			{
				_joltSpeed += (-1000f * _joltAt - 32f * _joltSpeed) * num2;
				_joltAt += _joltSpeed * num2;
			}
			float num3 = _joltAt.Length();
			if (!float.IsFinite(num3) || !float.IsFinite(_joltSpeed.X) || !float.IsFinite(_joltSpeed.Y) || (num3 < 0.3f && _joltSpeed.LengthSquared() < 100f))
			{
				_joltAt = Vector2.Zero;
				_joltSpeed = Vector2.Zero;
			}
			else if (num3 > 40f)
			{
				_joltAt *= 40f / num3;
			}
			Nudge(_joltAt);
		}

		private void Nudge(Vector2 offset)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			if (_victim == null)
			{
				return;
			}
			if (!FxKit.Alive((GodotObject)(object)_victim))
			{
				_victim = null;
				return;
			}
			Vector2 val = Vector2.Zero;
			if (offset != Vector2.Zero)
			{
				val = offset;
				Node parent = ((Node)_victim).GetParent();
				CanvasItem val2 = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
				if (val2 != null && FxKit.Alive((GodotObject)(object)val2))
				{
					Transform2D val3 = val2.GetGlobalTransform();
					val3 = val3.AffineInverse();
					val = val3.BasisXform(offset);
				}
				if (!float.IsFinite(val.X) || !float.IsFinite(val.Y))
				{
					val = Vector2.Zero;
				}
			}
			Vector2 position = _victim.Position;
			if (position != _victimWrote)
			{
				_victimHome = position - _victimMoved;
			}
			else if (val == _victimMoved)
			{
				return;
			}
			_victim.Position = _victimHome + val;
			_victimWrote = _victim.Position;
			_victimMoved = val;
		}

		private void ReleaseBody()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Nudge(Vector2.Zero);
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoHeld (victim): " + ex.Message);
			}
			_victim = null;
			_joltAt = Vector2.Zero;
			_joltSpeed = Vector2.Zero;
		}

		private static void Shatter(Node parent, Vector2 at, float halfWidth, float halfHeight, int extra)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected Obj, but got Unknown
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_MementoShards")
			};
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return;
			}
			val.GlobalPosition = at;
			Vector2 val2 = CornerArms(halfWidth, halfHeight);
			int num = 12 + Mathf.Max(0, extra);
			Polygon2D[] shards = new Polygon2D[num];
			Vector2[] ways = new Vector2[num];
			float[] spins = new float[num];
			bool[] slim = new bool[num];
			for (int i = 0; i < num; i++)
			{
				float num2 = 9f;
				int num3 = i;
				Vector2 val3;
				Vector2[] points;
				if (i < 12)
				{
					int num4 = i % 3;
					num3 = i / 3;
					float num5 = (((num3 & 1) == 0) ? (-1f) : 1f);
					float num6 = ((num3 < 2) ? (-1f) : 1f);
					val3 = new Vector2(num5 * halfWidth, num6 * halfHeight);
					points = Diamond(6f, 9f);
					switch (num4)
					{
					case 1:
						val3.X -= num5 * val2.X * 0.5f;
						points = Diamond(val2.X * 0.5f, 1.6f);
						break;
					case 2:
						val3.Y -= num6 * val2.Y * 0.5f;
						points = Diamond(1.6f, val2.Y * 0.5f);
						break;
					}
					if (num4 != 0)
					{
						slim[i] = true;
						num2 = 4f;
					}
				}
				else
				{
					float num7 = (float)Math.PI * 2f * FxKit.Hash(i * 11 + 3);
					val3 = new Vector2(Mathf.Cos(num7) * halfWidth, Mathf.Sin(num7) * halfHeight) * (0.6f + 0.6f * FxKit.Hash(i * 5 + 1));
					points = Diamond(3.5f, 5.25f);
				}
				ways[i] = val3 * (1.15f + 0.5f * FxKit.Hash(num3 * 7 + 2));
				spins[i] = (FxKit.Hash(i * 3 + 9) - 0.5f) * num2;
				Polygon2D val4 = FxKit.Shape(points, Rule(0.95f));
				((CanvasItem)val4).Material = (Material)(object)FxKit.Additive;
				((Node)val).AddChild((Node)(object)val4, false, (InternalMode)0);
				shards[i] = val4;
			}
			FxKit.Drive((Node)(object)val, 0.5f, (float u) =>
			{
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				float num8 = 1f - (1f - u) * (1f - u);
				for (int j = 0; j < shards.Length; j++)
				{
					Polygon2D val5 = shards[j];
					if (FxKit.Alive((GodotObject)(object)val5))
					{
						float num9 = (slim[j] ? Mathf.Min(1f, 0.2f + 4f * u) : (1f + 0.5f * (1f - u)));
						((Node2D)val5).Position = ways[j] * (0.1f + 0.9f * num8) + new Vector2(0f, 140f * u * u);
						((Node2D)val5).Rotation = spins[j] * u;
						((Node2D)val5).Scale = new Vector2(num9, num9);
						((CanvasItem)val5).Modulate = new Color(1f, 1f, 1f, (u < 0.6f) ? 1f : (1f - (u - 0.6f) / 0.4f));
					}
				}
			}, (TransitionType)0, (EaseType)2);
			FxKit.FreeAfter((Node)(object)val, 0.6);
		}

		private void BeginDissolve()
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			if (_phase == Phase.Dissolve || _phase == Phase.Over)
			{
				return;
			}
			_decoOut = true;
			_frameOff = true;
			int num = 0;
			foreach (Copy copy in _copies)
			{
				if (!copy.Hit)
				{
					Hold(copy);
					if (copy.Alpha > 0.2f && num < 12 && FxKit.Alive((GodotObject)(object)_under))
					{
						num++;
						Shade.Puff((Node)(object)_under, copy.Pos, 80f, 1.6f, 0.34f, 0.6f, 0.3f);
					}
				}
			}
			_phase = Phase.Dissolve;
			_clock = 0f;
		}

		private void TickDissolve(float dt)
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Clamp(_clock / 0.22f, 0f, 1f);
			for (int num2 = _copies.Count - 1; num2 >= 0; num2--)
			{
				Copy copy = _copies[num2];
				if (copy.Node == null)
				{
					_copies.RemoveAt(num2);
				}
				else if (!FxKit.Alive((GodotObject)(object)copy.Node))
				{
					Drop(num2);
				}
				else
				{
					copy.Pos = copy.FromPos + new Vector2(0f, -14f * num);
					copy.Alpha = copy.FromAlpha * (1f - num);
					Put(copy);
					Trail(copy, dt);
				}
			}
			if (!(num < 1f))
			{
				FreeCopies();
				_phase = Phase.Over;
				Leave(10f);
			}
		}

		public override void OnCardFinished()
		{
			try
			{
				if (_phase == Phase.Barrage)
				{
					if (!_struck)
					{
						BeginDissolve();
					}
				}
				else if (_phase == Phase.Standby)
				{
					Leave();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoHeld.OnCardFinished: " + ex.Message);
				Finish(10f);
			}
		}

		private static void Unmake(Copy copy)
		{
			try
			{
				if (FxKit.Alive((GodotObject)(object)copy.Streak))
				{
					((Node)copy.Streak).QueueFree();
				}
				copy.Streak = null;
				if (FxKit.Alive((GodotObject)(object)copy.Halo))
				{
					((Node)copy.Halo).QueueFree();
				}
				copy.Halo = null;
				if (copy.Node != null)
				{
					CardStandIn.Free(copy.Node);
				}
				copy.Node = null;
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoHeld (free): " + ex.Message);
			}
		}

		private void Drop(int index)
		{
			Copy copy = _copies[index];
			_copies.RemoveAt(index);
			Unmake(copy);
		}

		private void FreeCopies()
		{
			foreach (Copy copy in _copies)
			{
				Unmake(copy);
			}
			_copies.Clear();
		}

		private void Clear()
		{
			FreeCopies();
			RemoveDecos(0.15f);
			_handMarks.Clear();
		}

		protected override void OnLeaving()
		{
			if (C.Remote && _phase == Phase.Standby)
			{
				BeginDissolve();
			}
		}

		protected override void OnEnd()
		{
			ReleaseBody();
			try
			{
				Clear();
			}
			catch (Exception ex)
			{
				DevLog.Warn("MementoHeld.OnEnd: " + ex.Message);
			}
			if (FxKit.Alive((GodotObject)(object)_layer))
			{
				((Node)_layer).QueueFree();
			}
			_layer = null;
			_under = null;
			_cards = null;
			_frame = null;
		}

		public MementoHeld()
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		}

		static MementoHeld()
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
		}
	}
