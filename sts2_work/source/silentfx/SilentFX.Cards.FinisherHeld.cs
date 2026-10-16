// namespace: SilentFX.Cards
// type: FinisherHeld (class)
// source lines: 54634-57085
	public sealed class FinisherHeld : HeldSession
	{
		private enum Phase
		{
			Standby,
			Gather,
			Dissolve,
			Over
		}

		private struct Mark
		{
			public CardModel Card;

			public PileType Pile;
		}

		private sealed class Blade
		{
			public CardModel Card;

			public PileType Pile;

			public int Index;

			public int Of = 1;

			public int Order;

			public int Seed;

			public bool Last;

			public float Born;

			public bool Made;

			public FinisherCard Rig;

			public Sprite2D Aura;

			public Sprite2D Streak;

			public bool Moved;

			public bool WantOut;

			public float Wait;

			public float Out;

			public float Show;

			public Vector2 HomePos;

			public float HomeRot;

			public float HomeSize = 0.24f;

			public float HomeAlpha = 1f;

			public bool SlotKnown;

			public Vector2 Slot;

			public Vector2 Pos;

			public float Rot;

			public float Size = 0.24f;

			public float Alpha;

			public float Turn;

			public float TurnSpeed;

			public float Long = 1f;

			public float Way = 1f;

			public bool Honed;

			public float Flash;

			public Vector2 Tail;

			public bool Winding;

			public float Wind;

			public bool Sent;

			public bool Going;

			public bool Final;

			public bool Landed;

			public float StabAt;

			public Vector2 Spot;

			public Vector2 Pin;

			public Vector2 Dir = new Vector2(0f, 1f);

			public Vector2 FromPos;

			public float FromRot;

			public float FromSize = 0.3f;

			public float FromAlpha;

			public float FromTurn;

			public bool Spent;

			public Blade()
			{
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			}
		}

		private const float ArmLength = 100f;

		private const float AimedCharge = 0.45f;

		private readonly List<Blade> _blades = new List<Blade>();

		private readonly List<CardModel> _playedScratch = new List<CardModel>();

		private readonly List<Mark> _marksScratch = new List<Mark>();

		private readonly Dictionary<CardModel, int> _handScratch = new Dictionary<CardModel, int>(ReferenceEqualityComparer.Instance);

		private Node2D _layer;

		private bool _hosted;

		private Node2D _under;

		private Node2D _edges;

		private Node2D _lights;

		private ColorRect _veil;

		private readonly Sprite2D[] _pileGlow = new Sprite2D[3];

		private readonly float[] _pileHome = new float[3];

		private readonly int[] _pileCount = new int[3];

		private Vector2 _drawAt;

		private Vector2 _discardAt;

		private Vector2 _exhaustAt;

		private Vector2 _view = new Vector2(1920f, 1080f);

		private Vector2 _zoom = Vector2.One;

		private int _made;

		private float _card = 0.34f;

		private float _homeCard = 0.24f;

		private float _homeAlpha = 0.95f;

		private float _homeLight = 0.86f;

		private float _handAlpha = 0.5f;

		private float _peel = 34f;

		private float _lastScale = 1.45f;

		private float _pileRise = 112f;

		private float _ringGap = 22f;

		private float _ringMin = 150f;

		private float _ringMax = 560f;

		private float _fly = 0.24f;

		private float _stagger = 0.03f;

		private float _glide = 0.18f;

		private float _rise = 0.2f;

		private float _trembleMove = 2.2f;

		private float _trembleTurn = 0.03f;

		private float _stab = 0.08f;

		private float _finalStab = 0.08f;

		private float _sink = 0.06f;

		private float _stretch = 0.25f;

		private float _sureAt = 5f;

		private float _lockTime = 0.26f;

		private int _perFrame = 2;

		private Phase _phase;

		private float _clock;

		private float _scan;

		private int _signature;

		private int _total;

		private int _expected;

		private float _idle;

		private bool _ended;

		private bool _finalDue;

		private bool _closing;

		private int _struck;

		private bool _auto;

		private float _autoClock;

		private int _autoDone;

		private Vector2 _strikeAt;

		private float _k = 1f;

		private float _half = 110f;

		private float _charge;

		private Creature _victim;

		private int _slots = 1;

		private readonly List<Mark> _spare = new List<Mark>();

		private readonly List<Mark> _spareScratch = new List<Mark>();

		private bool _out;

		private bool _ringSet;

		private Vector2 _ringAt;

		private float _side = 1f;

		private float _tight = 1f;

		private float _tremble = 1f;

		private float _tense;

		private float _grow = 1f;

		private float _growTo = 1f;

		private Node2D _mark;

		private readonly Node2D[] _arms = new Node2D[4];

		private readonly Line2D[] _armCore = new Line2D[4];

		private readonly Line2D[] _armGlow = new Line2D[4];

		private readonly Line2D[] _armStop = new Line2D[4];

		private Node2D _pip;

		private Sprite2D _pipGlow;

		private bool _aim;

		private object _aimTarget;

		private Vector2 _aimAt;

		private Vector2 _feetAt;

		private float _aimWidth = 200f;

		private float _aimHeight = 240f;

		private Vector2 _boxHalf = new Vector2(106f, 126f);

		private float _lock;

		private float _markShow;

		private float _markAlpha;

		private float _sure;

		private float _jolt;

		private float _slam = -1f;

		private float _slamTime = 0.09f;

		private bool _markOff;

		private bool _broken;

		private static Color Cold(float alpha)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			return new Color(FinisherFx.Chill.R, FinisherFx.Chill.G, FinisherFx.Chill.B, Mathf.Clamp(alpha, 0f, 1f));
		}

		private static Color Red(float alpha)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			return new Color(FinisherFx.End.R, FinisherFx.End.G, FinisherFx.End.B, Mathf.Clamp(alpha, 0f, 1f));
		}

		private static Color Gold(float alpha)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			return new Color(FinisherFx.Hone.R, FinisherFx.Hone.G, FinisherFx.Hone.B, Mathf.Clamp(alpha, 0f, 1f));
		}

		private static float Wobble(float x)
		{
			return 0.65f * Mathf.Sin(x) + 0.35f * Mathf.Sin(x * 2.31f + 1.3f);
		}

		private void ReadTuning()
		{
			FinisherCard.ReadTuning();
			_card = Mathf.Clamp(Tuning.F("finisher.card", 0.34f), 0.16f, 0.7f);
			_homeCard = Mathf.Clamp(Tuning.F("finisher.home_card", 0.24f), 0.12f, 0.6f);
			_homeAlpha = Mathf.Clamp(Tuning.F("finisher.home_alpha", 0.95f), 0.1f, 1f);
			_homeLight = Mathf.Clamp(Tuning.F("finisher.home_light", 0.86f), 0.3f, 1f);
			_handAlpha = Mathf.Clamp(Tuning.F("finisher.hand_alpha", 0.5f), 0f, 1f);
			_peel = Mathf.Clamp(Tuning.F("finisher.peel", 34f), 0f, 120f);
			_lastScale = Mathf.Clamp(Tuning.F("finisher.last_scale", 1.45f), 1f, 2.5f);
			_pileRise = Mathf.Clamp(Tuning.F("finisher.card_rise", 112f), 20f, 300f);
			_ringGap = Mathf.Clamp(Tuning.F("finisher.ring_gap", 22f), 0f, 120f);
			_ringMin = Mathf.Clamp(Tuning.F("finisher.ring_min", 150f), 60f, 500f);
			_ringMax = Mathf.Max(_ringMin, Mathf.Clamp(Tuning.F("finisher.ring_reach", 560f), 60f, 900f));
			_fly = Mathf.Clamp(Tuning.F("finisher.fly_out", 0.24f), 0.05f, 1f);
			_stagger = Mathf.Clamp(Tuning.F("finisher.stagger", 0.03f), 0f, 0.2f);
			_glide = Mathf.Clamp(Tuning.F("finisher.glide", 0.18f), 0.03f, 1f);
			_rise = Mathf.Clamp(Tuning.F("finisher.rise", 0.2f), 0.03f, 1f);
			_trembleMove = Mathf.Clamp(Tuning.F("finisher.tremble", 2.2f), 0f, 12f);
			_trembleTurn = Mathf.Clamp(Tuning.F("finisher.tremble_turn", 0.03f), 0f, 0.3f);
			_stab = Mathf.Clamp(Tuning.F("finisher.dart", 0.08f), 0.02f, 0.3f);
			_finalStab = Mathf.Clamp(Tuning.F("finisher.final_dart", 0.08f), 0.02f, 0.4f);
			_sink = Mathf.Clamp(Tuning.F("finisher.sink", 0.06f), 0.02f, 0.3f);
			_stretch = Mathf.Clamp(Tuning.F("finisher.dart_stretch", 0.25f), 0f, 1f);
			_sureAt = Mathf.Clamp(Tuning.F("finisher.sure", 5f), 1f, 20f);
			_lockTime = Mathf.Clamp(Tuning.F("finisher.lock", 0.26f), 0.05f, 1.5f);
			_perFrame = Mathf.Clamp(Tuning.I("finisher.per_frame", 2), 1, 4);
		}

		private static int Kind(PileType pile)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected I4, but got Unknown
			return (pile - 1) switch
			{
				0 => 0, 
				2 => 1, 
				3 => 2, 
				1 => 4, 
				_ => 3, 
			};
		}

		private List<Mark> Marks(out int total)
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Invalid comparison between Unknown and I4
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Invalid comparison between Unknown and I4
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Invalid comparison between Unknown and I4
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Invalid comparison between Unknown and I4
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			List<Mark> marksScratch = _marksScratch;
			marksScratch.Clear();
			_spareScratch.Clear();
			total = 0;
			int num = Mathf.Clamp(Tuning.I("finisher.max", 9), 0, 12);
			CardModel val = C?.Card;
			if (val == null)
			{
				total = Mathf.Clamp(Tuning.I("finisher.preview", 6), 0, 16);
				int num2 = Mathf.Min(total, num);
				for (int i = 0; i < total; i++)
				{
					PileType pile = (i % 4) switch
					{
						0 => (PileType)3, 
						1 => (PileType)1, 
						2 => (PileType)4, 
						_ => (PileType)0, 
					};
					((i < total - num2) ? _spareScratch : marksScratch).Add(new Mark
					{
						Card = null,
						Pile = pile
					});
				}
				return marksScratch;
			}
			TurnLog.Played(val.Owner, _playedScratch, (CardType)1);
			total = _playedScratch.Count;
			for (int j = 0; j < total; j++)
			{
				CardModel val2 = _playedScratch[j];
				if (val2 != null)
				{
					PileType val3 = (PileType)((!C.Remote && val2 != val) ? ((int)TurnLog.Where(val2)) : 0);
					if ((int)val3 != 1 && (int)val3 != 3 && (int)val3 != 4 && (int)val3 != 2)
					{
						val3 = (PileType)0;
					}
					((j < total - num) ? _spareScratch : marksScratch).Add(new Mark
					{
						Card = val2,
						Pile = val3
					});
				}
			}
			return marksScratch;
		}

		private static int Signature(List<Mark> marks, int total)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected I4, but got Unknown
			int num = marks.Count + total * 131;
			foreach (Mark mark in marks)
			{
				int num2 = ((mark.Card != null) ? RuntimeHelpers.GetHashCode(mark.Card) : 0);
				num = num * 31 + num2 * 7 + mark.Pile;
			}
			return num;
		}

		protected override void Build()
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected Obj, but got Unknown
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Expected Obj, but got Unknown
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Expected Obj, but got Unknown
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Expected Obj, but got Unknown
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			ReadTuning();
			Node val = (Node)((FxKit.Alive((GodotObject)(object)C.UiParent) && C.UiParent.IsInsideTree()) ? ((object)C.UiParent) : ((object)Front));
			_hosted = (object)val != Front;
			_layer = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Finisher")
			};
			TimeFreeze.KeepLive((Node)(object)_layer);
			val.AddChild((Node)(object)_layer, false, (InternalMode)0);
			_under = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_FinisherUnder")
			};
			((Node)_layer).AddChild((Node)(object)_under, false, (InternalMode)0);
			_view = FxKit.ViewSize();
			_drawAt = (C.Remote ? Vector2.Zero : GameRefs.PilePoint((PileType)1));
			_discardAt = (C.Remote ? Vector2.Zero : GameRefs.PilePoint((PileType)3));
			_exhaustAt = (C.Remote ? Vector2.Zero : GameRefs.PilePoint((PileType)4));
			_side = C.Facing;
			_feetAt = C.Feet;
			_zoom = Zoom();
			_veil = FxKit.Veil(new Color(0.01f, 0.012f, 0.03f, 0f));
			((Node)Back).AddChild((Node)(object)_veil, false, (InternalMode)0);
			for (int i = 0; i < _pileGlow.Length; i++)
			{
				_pileGlow[i] = FxKit.Glow(Cold(0f), new Vector2(190f, 190f));
				((Node)_under).AddChild((Node)(object)_pileGlow[i], false, (InternalMode)0);
			}
			try
			{
				if (!C.Remote)
				{
					BuildMark();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherHeld.BuildMark: " + ex);
			}
			_edges = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_FinisherCards")
			};
			((Node)_layer).AddChild((Node)(object)_edges, false, (InternalMode)0);
			_lights = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_FinisherEdges")
			};
			((Node)_layer).AddChild((Node)(object)_lights, false, (InternalMode)0);
			try
			{
				List<Mark> marks = Marks(out var total);
				Raise(marks, total);
				if (PowerFxManager.LogTriggers && C.Card != null)
				{
					DevLog.Info($"Finisher: {total} Attack(s) remembered this turn, {_blades.Count} card(s) come back");
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("FinisherHeld.Build: " + ex2);
				Clear();
			}
		}

		private void Raise(List<Mark> marks, int total)
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			_signature = Signature(marks, total);
			_total = total;
			_slots = Mathf.Max(1, marks.Count);
			_spare.Clear();
			if (Tuning.F("finisher.refill", 1f) >= 0.5f)
			{
				_spare.AddRange(_spareScratch);
			}
			int[] array = new int[5];
			_handScratch.Clear();
			for (int i = 0; i < marks.Count; i++)
			{
				Mark mark = marks[i];
				int num = Kind(mark.Pile);
				int value;
				if (num == 4 && mark.Card != null)
				{
					_handScratch.TryGetValue(mark.Card, out value);
					_handScratch[mark.Card] = value + 1;
				}
				else
				{
					value = array[num]++;
				}
				_blades.Add(new Blade
				{
					Card = mark.Card,
					Pile = mark.Pile,
					Index = value,
					Order = i,
					Seed = i * 7 + 3,
					Born = Time + (float)i * 0.04f,
					Last = (i == marks.Count - 1)
				});
			}
			foreach (Blade blade in _blades)
			{
				int num2 = Kind(blade.Pile);
				int value2 = array[num2];
				if (num2 == 4 && blade.Card != null)
				{
					_handScratch.TryGetValue(blade.Card, out value2);
				}
				blade.Of = Mathf.Max(1, value2);
			}
			_handScratch.Clear();
		}

		private bool Materialise(Blade blade)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			blade.Made = true;
			_made++;
			if (!FxKit.Alive((GodotObject)(object)_edges) || !FxKit.Alive((GodotObject)(object)_under) || !FxKit.Alive((GodotObject)(object)_lights))
			{
				return false;
			}
			blade.Rig = FinisherCard.Make(blade.Card, (Node)(object)_edges, (Node)(object)_lights);
			if (blade.Rig == null)
			{
				return false;
			}
			blade.Aura = FxKit.Glow(Red(0f), new Vector2(64f, 64f));
			((Node)_under).AddChild((Node)(object)blade.Aura, false, (InternalMode)0);
			blade.Streak = FxKit.Glow(Cold(0f), new Vector2(64f, 64f));
			((CanvasItem)blade.Streak).Visible = false;
			((Node)_under).AddChild((Node)(object)blade.Streak, false, (InternalMode)0);
			Home(blade);
			blade.Pos = blade.HomePos;
			blade.Rot = blade.HomeRot;
			blade.Size = blade.HomeSize;
			blade.Alpha = 0f;
			blade.Turn = 0f;
			blade.Long = 1f;
			blade.Tail = blade.Pos;
			Put(blade);
			return true;
		}

		private void MakeDue()
		{
			while (_made < _perFrame)
			{
				Blade blade = null;
				foreach (Blade blade2 in _blades)
				{
					if (blade2.Spent || blade2.Made || Time < blade2.Born)
					{
						continue;
					}
					if (blade == null)
					{
						blade = blade2;
					}
					else if (_phase == Phase.Gather)
					{
						if (!blade.Last && (blade2.Last || blade2.Order > blade.Order))
						{
							blade = blade2;
						}
					}
					else if (blade2.Order < blade.Order)
					{
						blade = blade2;
					}
				}
				if (blade == null)
				{
					break;
				}
				if (!Materialise(blade))
				{
					Unmake(blade);
					blade.Spent = true;
				}
			}
		}

		private void Put(Blade blade)
		{
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			FinisherCard rig = blade.Rig;
			if (rig == null)
			{
				return;
			}
			float num = Mathf.Clamp(blade.Alpha, 0f, 1f);
			float num2 = Mathf.Clamp(blade.Out, 0f, 1f);
			float hone = Mathf.SmoothStep(0.45f, 0.95f, blade.Turn) * (0.72f + 0.6f * blade.Flash + 0.45f * blade.Wind + 0.18f * _tense);
			float glint = -1f;
			if (blade.Honed && !blade.Going && _tense > 0.4f)
			{
				float num3 = ((_phase == Phase.Gather) ? 2.6f : 1.5f);
				float num4 = Mathf.PosMod(Time * num3 - (float)blade.Order * 0.13f, 1.6f);
				if (num4 < 1f)
				{
					glint = num4;
				}
			}
			rig.Put(blade.Pos, blade.Rot, blade.Size, blade.Turn, blade.Way, blade.Long, Mathf.Lerp(_homeLight, 1f, num2), num, hone, glint);
			if (FxKit.Alive((GodotObject)(object)blade.Aura))
			{
				float num5 = ((blade.Last && !blade.Going) ? Mathf.Clamp(_charge, 0f, 1f) : 0f);
				float num6 = (blade.Last ? (0.26f + 0.1f * Mathf.Sin(Time * 5f) + 0.5f * blade.Wind + 0.4f * num5) : (0.5f * blade.Wind)) * num2 * num;
				((CanvasItem)blade.Aura).Visible = num6 > 0.01f;
				if (((CanvasItem)blade.Aura).Visible)
				{
					float num7 = Mathf.Max(0.05f, blade.Size);
					float num8 = 1f + 0.55f * num5;
					((Node2D)blade.Aura).GlobalPosition = blade.Pos;
					((Node2D)blade.Aura).GlobalRotation = blade.Rot;
					((Node2D)blade.Aura).Scale = new Vector2((300f * num7 * 0.9f + 40f) * num8, 422f * num7 * 1.5f * num8) / 64f;
					Color end = FinisherFx.End;
					Color hone2 = FinisherFx.Hone;
					float num9 = 0.75f * num5 * num5;
					((CanvasItem)blade.Aura).Modulate = new Color(Mathf.Lerp(end.R, hone2.R, num9), Mathf.Lerp(end.G, hone2.G, num9), Mathf.Lerp(end.B, hone2.B, num9), Mathf.Clamp(num6, 0f, 1f));
				}
			}
		}

		protected override void Tick(float dt)
		{
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!FxKit.Alive((GodotObject)(object)_layer) || !FxKit.Alive((GodotObject)(object)_under) || !FxKit.Alive((GodotObject)(object)_edges) || !FxKit.Alive((GodotObject)(object)_lights))
				{
					if (_phase != Phase.Over)
					{
						Finish(20f);
					}
					return;
				}
				_clock += dt;
				_made = 0;
				((CanvasItem)_layer).Modulate = new Color(1f, 1f, 1f, _hosted ? Mathf.Clamp(Fade, 0f, 1f) : 1f);
				_view = FxKit.ViewSize();
				_zoom = Zoom();
				_drawAt = (C.Remote ? Vector2.Zero : GameRefs.PilePoint((PileType)1));
				_discardAt = (C.Remote ? Vector2.Zero : GameRefs.PilePoint((PileType)3));
				_exhaustAt = (C.Remote ? Vector2.Zero : GameRefs.PilePoint((PileType)4));
				bool flag = _phase == Phase.Gather;
				bool flag2 = flag || (_phase == Phase.Standby && _aim && !Hovering && (Released || C.HasTarget));
				_tense = FxKit.Damp(_tense, flag2 ? 1f : 0f, 10f, dt);
				float target = (flag ? Mathf.Clamp(Tuning.F("finisher.tighten", 0.88f), 0.5f, 1f) : Mathf.Lerp(1f, 0.95f, _tense));
				_tight = FxKit.Damp(_tight, target, 12f, dt);
				_tremble = FxKit.Damp(_tremble, flag ? 2.2f : (1f + 0.5f * _tense), 12f, dt);
				_grow = FxKit.Damp(_grow, _growTo, 8f, dt);
				for (int i = 0; i < _pileHome.Length; i++)
				{
					_pileHome[i] = 0f;
					_pileCount[i] = 0;
				}
				switch (_phase)
				{
				case Phase.Standby:
					TickStandby(dt);
					break;
				case Phase.Gather:
					TickGather(dt);
					break;
				case Phase.Dissolve:
					TickDissolve(dt);
					break;
				}
				TickPiles();
				TickMark(dt);
				if (FxKit.Alive((GodotObject)(object)_veil))
				{
					float num = Mathf.Clamp(Tuning.F("finisher.veil", 0.28f), 0f, 0.8f);
					_veil.Color = new Color(0.01f, 0.012f, 0.03f, num * _markShow * Mathf.Clamp(Arm, 0f, 1f) * (0.4f + 0.6f * _sure));
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherHeld.Tick: " + ex);
				Finish(14f);
			}
		}

		private void Finish(float rate)
		{
			Clear();
			_phase = Phase.Over;
			_markOff = true;
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
					List<Mark> marks = Marks(out var total);
					if (Signature(marks, total) != _signature)
					{
						Clear();
						Raise(marks, total);
					}
				}
			}
			_out = !Hovering;
			_charge = FxKit.Damp(_charge, 0.45f * Mathf.Clamp(_tense, 0f, 1f), 4f, dt);
			MoveAll(dt);
		}

		private float Glide(float dt)
		{
			return 1f - Mathf.Exp(-3f * dt / Mathf.Max(0.02f, _glide));
		}

		private void MoveAll(float dt)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
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
			MakeDue();
			for (int i = 0; i < _blades.Count; i++)
			{
				Blade blade = _blades[i];
				if (blade.Spent || !blade.Made)
				{
					continue;
				}
				if (blade.Rig == null || !blade.Rig.Alive)
				{
					Unmake(blade);
					blade.Spent = true;
					continue;
				}
				if (blade.Going)
				{
					Stab(blade);
				}
				else
				{
					Move(blade, dt, flag, centre, radii);
					if (blade.Sent && _clock >= blade.StabAt)
					{
						Go(blade);
					}
				}
				if (!blade.Spent)
				{
					Put(blade);
					Trail(blade, dt);
					int num = Kind(blade.Pile);
					if (num <= 2)
					{
						_pileCount[num]++;
						_pileHome[num] += (1f - Mathf.Clamp(blade.Out, 0f, 1f)) * blade.Show;
					}
				}
			}
		}

		private bool FindRing(out Vector2 centre, out Vector2 radii)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
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
				List<FxTarget> enemies = C.Enemies;
				if (enemies == null || enemies.Count == 0)
				{
					return false;
				}
				int index = 0;
				try
				{
					Vector2 val = (C.Remote ? C.AimPosition : ((CanvasItem)Front).GetGlobalMousePosition());
					float num = float.MaxValue;
					for (int i = 0; i < enemies.Count; i++)
					{
						if (enemies[i] != null)
						{
							Vector2 val2 = enemies[i].Chest - val;
							float num2 = val2.LengthSquared();
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
				float width = Mathf.Max(60f, fxTarget.Width * _zoom.X);
				float height = Mathf.Max(60f, HeightOf(fxTarget, fxTarget.Width) * _zoom.Y);
				centre = fxTarget.Chest;
				radii = Radii(width, height);
				_side = ((fxTarget.Side < 0f) ? (-1f) : 1f);
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherHeld.FindRing: " + ex.Message);
				return false;
			}
		}

		private float HeightOf(FxTarget enemy, float width)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				object key = enemy.Key;
				Creature val = (Creature)((key is Creature) ? key : null);
				if (val != null)
				{
					NCombatRoom combatRoom = GameRefs.CombatRoom;
					NCreature val2 = ((combatRoom != null) ? combatRoom.GetCreatureNode(val) : null);
					if (FxKit.Alive((GodotObject)(object)val2) && ((Node)val2).IsInsideTree() && FxKit.Alive((GodotObject)(object)val2.Hitbox))
					{
						float y = GameRefs.DrawnSize(val2.Hitbox, C.FrontParent).Y;
						if (y > 20f)
						{
							return y;
						}
					}
				}
			}
			catch
			{
			}
			return width * 1.15f;
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

		private Vector2 BoxHalf(float width, float height)
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			return new Vector2(Mathf.Clamp(width * 0.5f + 6f, 80f, Mathf.Max(80f, _view.X * 0.3f)), Mathf.Clamp(height * 0.5f + 6f, 80f, Mathf.Max(80f, _view.Y * 0.32f)));
		}

		private static float Grow(float width, float height)
		{
			return Mathf.Clamp(Mathf.Sqrt(Mathf.Max(width, height) / 260f), 0.9f, 1.3f);
		}

		private Vector2 Radii(float width, float height)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = BoxHalf(width, height);
			_growTo = Grow(width, height);
			float num = _ringGap + 211f * _card * _growTo;
			return new Vector2(Mathf.Clamp(val.X + num, _ringMin, _ringMax), Mathf.Clamp(val.Y * 0.9f + num, _ringMin, _ringMax));
		}

		private Vector2 SlotOf(Blade blade, Vector2 centre, Vector2 radii)
		{
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			float num = -(float)Math.PI / 2f;
			float num2 = 1.1f;
			float num3 = _card * _grow;
			float num4 = 0f;
			if (!blade.Last)
			{
				int num5 = (Mathf.Max(1, _slots - 1) + 1) / 2;
				float num6 = Mathf.Min(Mathf.DegToRad(150f) / (float)num5, Mathf.DegToRad(36f));
				float num7 = ((blade.Order % 2 == 0) ? (-1f) : 1f) * _side;
				num += num7 * num6 * (float)(blade.Order / 2 + 1);
				num2 = 1f;
			}
			else
			{
				num3 *= _lastScale;
				num4 = 211f * _card * _grow * (_lastScale - 1f);
			}
			float num8 = 1f + 0.03f * Mathf.Sin(Time * 2.1f + (float)blade.Seed * 0.7f);
			float num9 = radii.X * _tight * num8 * num2 + num4;
			float num10 = radii.Y * _tight * num8 * num2 + num4;
			Vector2 val = centre + new Vector2(Mathf.Cos(num) * num9, Mathf.Sin(num) * num10);
			float num11 = 211f * num3 * 0.8f + 10f;
			val.X = Mathf.Clamp(val.X, num11, Mathf.Max(num11, _view.X - num11));
			val.Y = Mathf.Clamp(val.Y, num11, Mathf.Max(num11, _view.Y - num11));
			return val;
		}

		private Vector2 PileAt(PileType pile)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			if ((int)pile != 1)
			{
				if ((int)pile != 4)
				{
					return _discardAt;
				}
				return _exhaustAt;
			}
			return _drawAt;
		}

		private void Home(Blade blade)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Clamp((Time - blade.Born) / Mathf.Max(0.02f, _rise), 0f, 1f);
			float num2 = (blade.Show = 1f - (1f - num) * (1f - num) * (1f - num));
			int num3 = Kind(blade.Pile);
			if (num3 != 4 || !HomeInHand(blade, num2))
			{
				blade.HomeSize = _homeCard * Mathf.Lerp(0.45f, 1f, num2);
				blade.HomeAlpha = _homeAlpha * num2;
				if (num3 <= 2)
				{
					Vector2 val = PileAt(blade.Pile);
					float num4 = ((val.X < _view.X * 0.5f) ? 1f : (-1f));
					float num5 = Mathf.Min(15f, 84f / (float)blade.Of);
					float num6 = Mathf.DegToRad(-90f + ((float)blade.Index - (float)(blade.Of - 1) * 0.5f) * num5 + num4 * 16f);
					float num7 = _pileRise + 10f * (float)(blade.Index % 2) + 4f * Mathf.Sin(Time * 2.6f + (float)blade.Seed * 1.3f);
					Vector2 val2 = val + new Vector2(Mathf.Cos(num6), Mathf.Sin(num6)) * num7;
					float num8 = 300f * _homeCard * 0.5f + 4f;
					val2.X = Mathf.Clamp(val2.X, num8, Mathf.Max(num8, _view.X - num8));
					blade.HomePos = val.Lerp(val2, num2);
					blade.HomeRot = (num6 + (float)Math.PI / 2f + 0.03f * Mathf.Sin(Time * 2.2f + (float)blade.Seed * 2.1f)) * num2;
				}
				else
				{
					float num9 = 0f - C.Facing;
					Vector2 val3 = new Vector2(C.Body.X * _zoom.X, C.Body.Y * _zoom.Y);
					int num10 = ((num3 == 3) ? blade.Index : (blade.Index + 1));
					Vector2 val4 = C.Chest + new Vector2(num9 * (val3.X * 0.3f + 24f), (0f - val3.Y) * 0.06f);
					float num11 = Mathf.DegToRad(-90f + num9 * Mathf.Min(112f, 22f + 17f * (float)num10));
					float num12 = val3.Y * 0.3f + 70f + 10f * (float)(num10 % 2) + 4f * Mathf.Sin(Time * 2.4f + (float)blade.Seed * 1.3f);
					Vector2 val5 = val4 + new Vector2(Mathf.Cos(num11), Mathf.Sin(num11)) * num12;
					blade.HomePos = val4.Lerp(val5, num2);
					float num13 = num9 * Mathf.DegToRad(Mathf.Min(64f, 14f + 11f * (float)num10));
					blade.HomeRot = (num13 + 0.03f * Mathf.Sin(Time * 2.2f + (float)blade.Seed * 2.1f)) * num2;
				}
			}
		}

		private bool HomeInHand(Blade blade, float e)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Invalid comparison between Unknown and I4
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			NCard val = null;
			try
			{
				if (blade.Card != null && (int)TurnLog.Where(blade.Card) == 2)
				{
					val = HandCards.NodeOf(blade.Card);
				}
			}
			catch
			{
				val = null;
			}
			if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree())
			{
				return false;
			}
			CardStandIn.Pose(val, out var centre, out var rotation, out var scale);
			Vector2 val2 = new Vector2(Mathf.Sin(rotation), 0f - Mathf.Cos(rotation));
			Vector2 val3 = new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation));
			float num = ((float)blade.Index - (float)(blade.Of - 1) * 0.5f) * 26f;
			blade.HomePos = centre + val2 * (_peel * e + 3f * Mathf.Sin(Time * 1.6f + (float)blade.Seed * 0.9f) * e) + val3 * num;
			blade.HomeRot = rotation;
			blade.HomeSize = Mathf.Clamp(scale.Y, 0.1f, 1.6f);
			blade.HomeAlpha = _handAlpha * e;
			return true;
		}

		private void Move(Blade blade, float dt, bool ring, Vector2 centre, Vector2 radii)
		{
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_0497: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0560: Unknown result type (might be due to invalid IL or missing references)
			//IL_0565: Unknown result type (might be due to invalid IL or missing references)
			bool num = C.Remote && !blade.Moved;
			Home(blade);
			bool flag = _out & ring;
			if (!blade.Moved)
			{
				blade.Moved = true;
				blade.WantOut = flag;
				blade.Wait = 0f;
			}
			else if (flag != blade.WantOut)
			{
				blade.WantOut = flag;
				blade.Wait = (float)blade.Order * _stagger;
			}
			if (C.Remote & flag)
			{
				blade.Out = 1f;
			}
			if (blade.Wait > 0f)
			{
				blade.Wait -= dt;
			}
			else
			{
				blade.Out = Mathf.MoveToward(blade.Out, blade.WantOut ? 1f : 0f, dt / Mathf.Max(0.02f, _fly));
			}
			if (ring)
			{
				Vector2 val = SlotOf(blade, centre, radii);
				if (!blade.SlotKnown || blade.Out <= 0.001f)
				{
					blade.Slot = val;
					blade.SlotKnown = true;
				}
				else
				{
					blade.Slot = blade.Slot.Lerp(val, Glide(dt));
				}
			}
			else if (!blade.SlotKnown)
			{
				blade.Slot = blade.HomePos;
			}
			float num2 = Mathf.Clamp(blade.Out, 0f, 1f);
			float num3 = 1f - (1f - num2) * (1f - num2) * (1f - num2);
			Vector2 val2 = _ringAt - blade.Slot;
			bool flag2 = val2.LengthSquared() > 4f;
			float num4 = (flag2 ? (val2.Angle() + (float)Math.PI / 2f) : blade.HomeRot);
			float num5 = (float)blade.Seed * 1.31f;
			float num6 = (blade.Last ? Mathf.Clamp(_charge, 0f, 1f) : 0f);
			float num7 = _tremble * num3 * (1f + 1.8f * num6) * (1f - Mathf.SmoothStep(0.86f, 0.97f, num6));
			Vector2 val3 = new Vector2(Wobble(Time * 43f + num5), Wobble(Time * 57f + num5 * 1.7f)) * (_trembleMove * num7);
			float num8 = Wobble(Time * 49f + num5 * 2.3f) * _trembleTurn * num7;
			blade.Wind = FxKit.Damp(blade.Wind, (blade.Winding || blade.Sent) ? 1f : 0f, 18f, dt);
			Vector2 val4 = (Vector2)(flag2 ? (-val2.Normalized()) : new Vector2(0f, -1f));
			float num9 = (blade.Last ? 46f : 22f) * blade.Wind * num3;
			if (blade.Last)
			{
				num9 += 60f * num6 * num3;
				num9 = Mathf.Min(num9, Mathf.Max(0f, blade.Slot.Y - 70f));
			}
			bool flag3 = blade.WantOut && num2 > 0.8f;
			Spring(blade, flag3 ? ((blade.Last ? 0.88f : 1f) + 0.14f * _tense + 0.06f * blade.Wind) : 0f, dt);
			if (!flag3)
			{
				if (blade.Turn < 0.3f)
				{
					blade.Honed = false;
				}
			}
			else if (!blade.Honed && blade.Turn > 0.75f)
			{
				blade.Honed = true;
				blade.Flash = 1f;
			}
			blade.Flash = FxKit.Damp(blade.Flash, 0f, 9f, dt);
			if (Mathf.Abs(blade.Turn) < 0.2f)
			{
				float num10 = blade.Slot.X - _ringAt.X;
				ref float way = ref blade.Way;
				float num11;
				if (Mathf.Abs(num10) > 8f)
				{
					num11 = ((num10 > 0f) ? 1f : (-1f));
				}
				else
				{
					num11 = _side;
				}
				way = num11;
			}
			float num12 = _card * _grow * (blade.Last ? _lastScale : 1f);
			blade.Pos = FxKit.Bezier(b: blade.HomePos.Lerp(blade.Slot, 0.5f) + new Vector2(0f, -110f), a: blade.HomePos, c: blade.Slot, t: num3) + val4 * num9 + val3;
			blade.Rot = Mathf.LerpAngle(blade.HomeRot, num4, num3) + num8;
			blade.Size = Mathf.Lerp(blade.HomeSize, num12, num3) * (1f + (blade.Last ? 0.16f : 0.1f) * blade.Wind + 0.14f * num6);
			blade.Alpha = Mathf.Lerp(blade.HomeAlpha, 1f, Mathf.Min(1f, num3 * 2f));
			if (C.Remote)
			{
				blade.Alpha *= blade.Show;
			}
			if (num)
			{
				blade.Tail = blade.Pos;
			}
			blade.Long = 1f;
		}

		private static void Spring(Blade blade, float to, float dt)
		{
			float num = Mathf.Min(dt, 0.1f);
			while (num > 0.0001f)
			{
				float num2 = Mathf.Min(num, 0.008f);
				num -= num2;
				blade.TurnSpeed += ((to - blade.Turn) * 420f - blade.TurnSpeed * 26f) * num2;
				blade.Turn += blade.TurnSpeed * num2;
			}
			blade.Turn = Mathf.Clamp(blade.Turn, -0.2f, 1.35f);
		}

		private static void Trail(Blade blade, float dt)
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
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)blade.Streak))
			{
				blade.Tail = blade.Tail.Lerp(blade.Pos, 1f - Mathf.Exp(-14f * dt));
				Vector2 val = blade.Pos - blade.Tail;
				float num = val.Length();
				float num2 = Mathf.Clamp((num - 14f) / 60f, 0f, 1f) * Mathf.Clamp(blade.Alpha, 0f, 1f);
				((CanvasItem)blade.Streak).Visible = num2 > 0.01f;
				if (((CanvasItem)blade.Streak).Visible)
				{
					((Node2D)blade.Streak).GlobalPosition = (blade.Pos + blade.Tail) * 0.5f;
					((Node2D)blade.Streak).GlobalRotation = val.Angle();
					((Node2D)blade.Streak).Scale = new Vector2(num * 1.4f, blade.Last ? 26f : 16f) / 64f;
					((CanvasItem)blade.Streak).Modulate = (blade.Going ? Gold(0.75f * num2) : (blade.Last ? Red(0.6f * num2) : Cold(0.55f * num2)));
				}
			}
		}

		private void TickPiles()
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < _pileGlow.Length; i++)
			{
				Sprite2D val = _pileGlow[i];
				if (FxKit.Alive((GodotObject)(object)val))
				{
					float num = ((_pileCount[i] > 0) ? (_pileHome[i] / (float)_pileCount[i]) : 0f);
					((CanvasItem)val).Visible = num > 0.01f;
					if (((CanvasItem)val).Visible)
					{
						((Node2D)val).GlobalPosition = i switch
						{
							1 => _discardAt, 
							0 => _drawAt, 
							_ => _exhaustAt, 
						};
						((CanvasItem)val).Modulate = Cold((0.34f + 0.1f * Mathf.Sin(Time * 7f + (float)i * 1.7f)) * num);
					}
				}
			}
		}

		private void BuildMark()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected Obj, but got Unknown
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Expected Obj, but got Unknown
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Expected Obj, but got Unknown
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Expected Obj, but got Unknown
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_030e: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			_mark = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_FinisherMark"),
				Modulate = new Color(1f, 1f, 1f, 0f),
				Visible = false
			};
			((Node)_layer).AddChild((Node)(object)_mark, false, (InternalMode)0);
			float num = Mathf.Clamp(Tuning.F("finisher.mark_width", 3f), 1f, 8f);
			Curve val = new Curve();
			val.AddPoint(new Vector2(0f, 0.08f), 0f, 0f, (TangentMode)0, (TangentMode)0);
			val.AddPoint(new Vector2(1f, 1f), 0f, 0f, (TangentMode)0, (TangentMode)0);
			Vector2 zero = Vector2.Zero;
			Vector2 val2 = new Vector2(100f, 0f);
			for (int i = 0; i < 4; i++)
			{
				Node2D val3 = new Node2D();
				((Node)_mark).AddChild((Node)(object)val3, false, (InternalMode)0);
				_arms[i] = val3;
				Line2D val4 = Props.Line(num * 4.5f, new Color(FinisherFx.End.R, FinisherFx.End.G, FinisherFx.End.B, 1f), true, zero, val2);
				val4.WidthCurve = val;
				((Node)val3).AddChild((Node)(object)val4, false, (InternalMode)0);
				_armGlow[i] = val4;
				Line2D val5 = Props.Line(num, new Color(1f, 1f, 1f, 1f), false, zero, val2);
				val5.WidthCurve = val;
				((Node)val3).AddChild((Node)(object)val5, false, (InternalMode)0);
				_armCore[i] = val5;
				Line2D val6 = Props.Line(Mathf.Max(1f, num * 0.7f), new Color(1f, 1f, 1f, 1f), false, new Vector2(100f, -9f), new Vector2(100f, 9f));
				((Node)val3).AddChild((Node)(object)val6, false, (InternalMode)0);
				_armStop[i] = val6;
			}
			_pip = new Node2D
			{
				Modulate = new Color(1f, 1f, 1f, 0f)
			};
			((Node)_mark).AddChild((Node)(object)_pip, false, (InternalMode)0);
			_pipGlow = FxKit.Glow(Red(0.5f), new Vector2(58f, 58f));
			((Node)_pip).AddChild((Node)(object)_pipGlow, false, (InternalMode)0);
			((Node)_pip).AddChild((Node)(object)FxKit.Shape(new Vector2[4]
			{
				new Vector2(0f, -6f),
				new Vector2(6f, 0f),
				new Vector2(0f, 6f),
				new Vector2(-6f, 0f)
			}, new Color(1f, 1f, 1f, 1f)), false, (InternalMode)0);
		}

		private void Aim(CardFxContext victim)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			if (victim != null && victim.HasTarget)
			{
				bool num = !_aim || _aimTarget != victim.Target || _aimAt.DistanceTo(victim.TargetChest) > 6f || _markShow < 0.15f;
				Vector2 val = Zoom();
				_aim = true;
				_aimTarget = victim.Target;
				_aimAt = victim.TargetChest;
				_feetAt = victim.TargetFeet;
				_side = ((victim.TargetSide < 0f) ? (-1f) : 1f);
				_aimWidth = Mathf.Max(60f, victim.TargetWidth * val.X);
				_aimHeight = Mathf.Max(60f, victim.TargetHeight * val.Y);
				_boxHalf = BoxHalf(_aimWidth, _aimHeight);
				if (num)
				{
					_lock = 0f;
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
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherHeld.OnTargetChanged: " + ex.Message);
			}
		}

		private static float Stepped(float t)
		{
			float num = Mathf.Clamp(t, 0f, 1f) * 3f;
			float num2 = Mathf.Floor(num);
			float num3 = Mathf.Min(1f, (num - num2) * 2.4f);
			return Mathf.Min(1f, (num2 + num3 * num3 * (3f - 2f * num3)) / 3f);
		}

		private void TickMark(float dt)
		{
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0596: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0416: Unknown result type (might be due to invalid IL or missing references)
			//IL_041a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			bool flag = FxKit.Alive((GodotObject)(object)_mark);
			if (!flag && !C.Remote)
			{
				return;
			}
			bool flag2 = _aim && !_markOff && (Released || C.HasTarget);
			if (C.Remote)
			{
				flag2 = flag2 && _phase == Phase.Gather;
			}
			_markShow = FxKit.Damp(_markShow, flag2 ? 1f : 0f, 12f, dt);
			if (flag2)
			{
				_lock = Mathf.Min(1f, _lock + dt / Mathf.Max(0.05f, _lockTime));
			}
			_sure = FxKit.Damp(_sure, Mathf.Clamp((float)_total / Mathf.Max(1f, _sureAt), 0f, 1f), 8f, dt);
			_jolt = FxKit.Damp(_jolt, 0f, 14f, dt);
			float num = 0f;
			if (_slam >= 0f && !_broken)
			{
				_slam = Mathf.Min(1f, _slam + dt / Mathf.Max(0.02f, _slamTime));
				num = _slam * _slam;
				if (_slam >= 1f)
				{
					BreakMark();
				}
			}
			if (!flag)
			{
				return;
			}
			float num2 = Stepped(_lock);
			bool flag3 = _phase == Phase.Gather;
			float num3 = ((_sure > 0.98f) ? (0.5f + 0.5f * Mathf.Sin(Time * 6.5f)) : 0f);
			float num4 = Mathf.Lerp(0.7f, 0.2f, _sure) * (1f + 0.9f * (1f - num2)) * (flag3 ? 0.85f : 1f) * (1f - 0.35f * _jolt) * (1f - num);
			float num5 = Mathf.Lerp(0.36f, 0.62f, _sure);
			float num6 = Mathf.Clamp(Arm, 0f, 1f) * _markShow * Mathf.Min(1f, _lock * 3f) * Mathf.Lerp(0.34f, 1f, _sure);
			num6 = (_markAlpha = Mathf.Clamp(num6 * (1f + 0.6f * _jolt + num), 0f, 1f));
			((CanvasItem)_mark).Modulate = new Color(1f, 1f, 1f, num6);
			((CanvasItem)_mark).Visible = num6 > 0.004f && !_broken;
			if (!((CanvasItem)_mark).Visible)
			{
				return;
			}
			_mark.GlobalPosition = _aimAt;
			float num7 = Mathf.Max(40f, _boxHalf.Length());
			Color modulate = new Color(Mathf.Lerp(0.7f, 1f, _sure), Mathf.Lerp(0.75f, 1f, _sure), Mathf.Lerp(0.82f, 1f, _sure), 1f);
			float num8 = Mathf.Clamp(0.1f + 0.5f * _sure + 0.25f * num3 + 0.5f * _jolt + num, 0f, 1f);
			for (int i = 0; i < 4; i++)
			{
				Node2D val = _arms[i];
				if (FxKit.Alive((GodotObject)(object)val))
				{
					float num9 = (((i & 1) == 0) ? (-1f) : 1f);
					float num10 = ((i < 2) ? (-1f) : 1f);
					Vector2 val2 = new Vector2(num9 * _boxHalf.X, num10 * _boxHalf.Y) / num7;
					val.Position = val2 * (num4 * num7);
					val.Rotation = val2.Angle();
					val.Scale = new Vector2(num5 * num7 / 100f, 1f);
					if (FxKit.Alive((GodotObject)(object)_armCore[i]))
					{
						((CanvasItem)_armCore[i]).Modulate = modulate;
					}
					if (FxKit.Alive((GodotObject)(object)_armStop[i]))
					{
						((CanvasItem)_armStop[i]).Modulate = modulate;
					}
					if (FxKit.Alive((GodotObject)(object)_armGlow[i]))
					{
						((CanvasItem)_armGlow[i]).Modulate = new Color(1f, 1f, 1f, num8);
					}
				}
			}
			if (FxKit.Alive((GodotObject)(object)_pip))
			{
				float num11 = Mathf.Lerp(0.7f, 1.25f, _sure) * (1f + 0.12f * num3 + 0.4f * _jolt);
				_pip.Scale = new Vector2(num11, num11);
				((CanvasItem)_pip).Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(_sure * 2.5f, 0f, 1f) * num2);
				if (FxKit.Alive((GodotObject)(object)_pipGlow))
				{
					((CanvasItem)_pipGlow).Modulate = Red(0.35f + 0.35f * _sure + 0.25f * num3);
				}
			}
		}

		private void BreakMark()
		{
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			if (_broken)
			{
				return;
			}
			_broken = true;
			_markOff = true;
			if (FxKit.Alive((GodotObject)(object)_mark))
			{
				bool num = ((CanvasItem)_mark).Visible && _markAlpha > 0.12f;
				((CanvasItem)_mark).Visible = false;
				if (num)
				{
					Shatter((Node)(FxKit.Alive((GodotObject)(object)C.FrontParent) ? ((object)C.FrontParent) : ((object)Front)), _aimAt, _boxHalf / Mathf.Max(0.2f, _zoom.X), Mathf.Lerp(0.36f, 0.62f, _sure));
				}
			}
		}

		private static void Shatter(Node parent, Vector2 at, Vector2 half, float length)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected Obj, but got Unknown
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_FinisherShards")
			};
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return;
			}
			val.GlobalPosition = at;
			float num = Mathf.Max(40f, half.Length());
			Line2D[] shards = new Line2D[8];
			Vector2[] ways = new Vector2[8];
			float[] turns = new float[8];
			float[] spins = new float[8];
			for (int i = 0; i < 8; i++)
			{
				int num2 = i / 2;
				bool flag = i % 2 == 1;
				Vector2 val2 = new Vector2((((num2 & 1) == 0) ? (-1f) : 1f) * half.X, ((num2 < 2) ? (-1f) : 1f) * half.Y) / num;
				float num3 = length * num * (flag ? 0.55f : 0.4f);
				Line2D val3 = Props.Line(flag ? 3.5f : 2.5f, new Color(1f, 1f, 1f, 0.95f), true, new Vector2((0f - num3) * 0.5f, 0f), new Vector2(num3 * 0.5f, 0f));
				((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
				shards[i] = val3;
				turns[i] = val2.Angle();
				ways[i] = val2 * num * (flag ? 1.25f : 0.6f) * (0.85f + 0.5f * FxKit.Hash(i * 7 + 2));
				spins[i] = (FxKit.Hash(i * 3 + 9) - 0.5f) * 8f;
				((Node2D)val3).Rotation = turns[i];
			}
			FxKit.Drive((Node)(object)val, 0.5f, (float u) =>
			{
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				float num4 = 1f - (1f - u) * (1f - u) * (1f - u);
				for (int j = 0; j < shards.Length; j++)
				{
					Line2D val4 = shards[j];
					if (FxKit.Alive((GodotObject)(object)val4))
					{
						((Node2D)val4).Position = ways[j] * num4 + new Vector2(0f, 150f * u * u);
						((Node2D)val4).Rotation = turns[j] + spins[j] * u;
						((CanvasItem)val4).Modulate = new Color(1f, 1f, 1f, (u < 0.5f) ? 1f : (1f - (u - 0.5f) / 0.5f));
					}
				}
			}, (TransitionType)0, (EaseType)2);
			FxKit.FreeAfter((Node)(object)val, 0.6);
		}

		protected override void OnPlay(CardFxContext play)
		{
			try
			{
				if (_phase != Phase.Standby)
				{
					return;
				}
				if (play != null && play.HasTarget)
				{
					Aim(play);
				}
				else if (C.HasTarget)
				{
					Aim(C);
				}
				_expected = ((C.Card != null) ? FinisherFx.Expected(C.Card) : (_blades.Count + _spare.Count));
				_idle = 0f;
				_victim = ((play != null && play.HasTarget) ? play.Target : C.Target);
				if (play != null && play.HasTarget)
				{
					_k = Strokes.Scale(play);
					_half = FinisherFx.Across(play) * 0.5f;
				}
				int num = 0;
				foreach (Blade blade in _blades)
				{
					if (!blade.Spent)
					{
						num++;
					}
				}
				if (!_aim || num == 0)
				{
					BeginDissolve();
					return;
				}
				_out = true;
				foreach (Blade blade2 in _blades)
				{
					if (!blade2.Spent)
					{
						blade2.Born = Mathf.Min(blade2.Born, Time);
						blade2.Wait = Mathf.Min(blade2.Wait, (float)blade2.Order * 0.012f);
					}
				}
				_phase = Phase.Gather;
				_clock = 0f;
				WindUp(0);
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherHeld.OnPlay: " + ex);
				Finish(14f);
			}
		}

		private Blade NextBlade()
		{
			Blade blade = null;
			bool flag = false;
			foreach (Blade blade2 in _blades)
			{
				if (!blade2.Last && !blade2.Spent && !blade2.Sent && blade2.Rig != null && blade2.Rig.Alive)
				{
					bool flag2 = blade2.Out >= 0.9f;
					if (blade == null || (flag2 && !flag) || (flag2 == flag && blade2.Order > blade.Order))
					{
						blade = blade2;
						flag = flag2;
					}
				}
			}
			return blade;
		}

		private void Refill(Blade gone)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			if (gone != null && !gone.Last && _spare.Count != 0)
			{
				Mark mark = _spare[_spare.Count - 1];
				_spare.RemoveAt(_spare.Count - 1);
				_blades.Add(new Blade
				{
					Card = mark.Card,
					Pile = mark.Pile,
					Index = 0,
					Of = 1,
					Order = gone.Order,
					Seed = (_blades.Count + 1) * 7 + 3,
					Born = Time,
					Last = false
				});
			}
		}

		private void Gather()
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			Blade blade = LastBlade();
			if (blade != null && !(blade.Out < 0.6f) && FxKit.Alive((GodotObject)(object)_under))
			{
				float size = 422f * Mathf.Max(0.1f, blade.Size) * 1.5f;
				FxKit.RingPulse((Node)(object)_under, blade.Pos, size, FinisherFx.Hone, 0f, 0.2f, 1f, 0.22f, 1f, 0f, 0.03f, 0.5f);
			}
		}

		private Blade LastBlade()
		{
			foreach (Blade blade in _blades)
			{
				if (blade.Last && !blade.Spent && !blade.Sent && blade.Rig != null && blade.Rig.Alive)
				{
					return blade;
				}
			}
			return null;
		}

		private bool AnyFlying()
		{
			foreach (Blade blade in _blades)
			{
				if (blade.Sent && !blade.Spent)
				{
					return true;
				}
			}
			return false;
		}

		private bool AnyWinding()
		{
			foreach (Blade blade in _blades)
			{
				if (blade.Winding && !blade.Spent)
				{
					return true;
				}
			}
			return false;
		}

		private void WindUp(int hitsDone)
		{
			foreach (Blade blade2 in _blades)
			{
				if (!blade2.Sent)
				{
					blade2.Winding = false;
				}
			}
			Blade blade = ((_expected - hitsDone <= 1) ? LastBlade() : NextBlade());
			if (blade != null)
			{
				blade.Winding = true;
			}
		}

		private void TickGather(float dt)
		{
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			_out = true;
			if (_auto && !_ended)
			{
				_autoClock += dt;
				if (_autoClock >= 0.16f)
				{
					_autoClock = 0f;
					AutoStep();
				}
			}
			if (!_ended)
			{
				float num = Mathf.Clamp(((float)(_auto ? _autoDone : _struck) + 1f) / (float)Mathf.Max(1, _expected), 0f, 1f);
				_charge = FxKit.Damp(_charge, Mathf.Lerp(0.45f, 1f, num), 10f, dt);
			}
			MoveAll(dt);
			_idle += dt;
			if (!_ended && !AnyWinding())
			{
				WindUp(_auto ? _autoDone : _struck);
			}
			if (AnyFlying())
			{
				return;
			}
			if (_ended)
			{
				BreakMark();
				FreeBlades();
				_phase = Phase.Over;
				Leave(8f);
			}
			else if ((_closing && !_auto) || !(_idle < Mathf.Clamp(Tuning.F("finisher.wait", 1.8f), 0.4f, 4f)))
			{
				if (_struck > 0 && LastBlade() != null)
				{
					Send(_strikeAt, _struck, last: true);
				}
				else
				{
					BeginDissolve();
				}
			}
		}

		public bool Strike(CardFxContext hit, Vector2 at, int index, bool last)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (Finished || _phase != Phase.Gather || _ended)
				{
					return false;
				}
				_idle = 0f;
				_strikeAt = at;
				if (hit != null && hit.HasTarget)
				{
					_feetAt = hit.TargetFeet;
					_k = Strokes.Scale(hit);
					_half = FinisherFx.Across(hit) * 0.5f;
					_victim = hit.Target;
				}
				if (C.Card == null)
				{
					if (_auto)
					{
						return true;
					}
					_auto = true;
					_autoClock = 0f;
					_autoDone = 0;
					return AutoStep();
				}
				_struck++;
				return Send(at, index, last);
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherHeld.Strike: " + ex);
				Finish(10f);
				return false;
			}
		}

		private bool AutoStep()
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			foreach (Blade blade in _blades)
			{
				if (!blade.Spent && !blade.Sent)
				{
					num++;
				}
			}
			_idle = 0f;
			bool result = Send(_strikeAt, _autoDone, num <= 1);
			_autoDone++;
			return result;
		}

		private bool Send(Vector2 at, int index, bool last)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			if (!last)
			{
				_jolt = 1f;
				Blade blade = NextBlade();
				if (blade != null)
				{
					Launch(blade, at, 0f, final: false);
					Refill(blade);
				}
				Gather();
				WindUp(index + 1);
				return blade != null;
			}
			_ended = true;
			float num = 0f;
			Blade blade2;
			while ((blade2 = NextBlade()) != null)
			{
				Launch(blade2, at, num, final: false);
				num = Mathf.Min(0.08f, num + 0.02f);
			}
			Blade blade3 = LastBlade();
			if (blade3 != null)
			{
				Launch(blade3, at, num, final: true);
				_finalDue = true;
				Sfx.PlayAt("finisher_last", num + _finalStab);
			}
			_slam = 0f;
			_slamTime = num + _finalStab;
			return blade3 != null;
		}

		private void Launch(Blade blade, Vector2 at, float delay, bool final)
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			blade.Sent = true;
			blade.Winding = true;
			blade.Final = final;
			blade.StabAt = _clock + delay;
			blade.Spot = (final ? at : (at + new Vector2(FxKit.Hash(blade.Seed + 5) - 0.5f, FxKit.Hash(blade.Seed + 9) - 0.5f) * 36f));
		}

		private void Go(Blade blade)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			blade.Going = true;
			blade.Landed = false;
			blade.FromPos = blade.Pos;
			blade.FromRot = blade.Rot;
			blade.FromSize = blade.Size;
			blade.FromAlpha = blade.Alpha;
			blade.FromTurn = blade.Turn;
			blade.Pin = (Vector2)(blade.Final ? new Vector2(blade.Spot.X, Mathf.Max(blade.Spot.Y + 30f, _feetAt.Y)) : blade.Spot);
			Vector2 val = blade.Pin - blade.Pos;
			blade.Dir = (Vector2)((val.LengthSquared() > 1f) ? val.Normalized() : new Vector2(0f, 1f));
		}

		private void Stab(Blade blade)
		{
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Max(0.01f, blade.Final ? _finalStab : _stab);
			float num2 = Mathf.Max(0f, (_clock - blade.StabAt) / num);
			float num3 = 211f * blade.FromSize;
			float num4 = (blade.Final ? (_stretch * 1.8f) : _stretch);
			blade.Size = blade.FromSize;
			blade.Rot = Mathf.LerpAngle(blade.FromRot, blade.Dir.Angle() + (float)Math.PI / 2f, Mathf.Min(1f, num2 * 4f));
			if (num2 < 1f)
			{
				float num5 = num2 * num2;
				blade.Long = 1f + num4 * num5;
				blade.Pos = blade.FromPos.Lerp(blade.Pin - blade.Dir * (num3 * blade.Long), num5);
				blade.Turn = Mathf.Lerp(blade.FromTurn, 1.2f, Mathf.Min(1f, num2 * 2f));
				blade.Alpha = Mathf.Max(blade.FromAlpha, 0.95f);
				return;
			}
			if (!blade.Landed)
			{
				blade.Landed = true;
				Land(blade);
			}
			float num6 = Mathf.Max(0.01f, blade.Final ? (_sink * 2.6f) : _sink);
			float num7 = Mathf.Clamp((num2 - 1f) * num / num6, 0f, 1f);
			blade.Long = Mathf.Max(0.02f, (1f + num4) * (blade.Final ? (1f - num7 * num7) : ((1f - num7) * (1f - num7))));
			blade.Pos = blade.Pin - blade.Dir * (num3 * blade.Long);
			blade.Turn = 1.2f;
			blade.Alpha = 1f - Mathf.SmoothStep(0.6f, 1f, num7);
			if (!(num7 < 1f))
			{
				blade.Spent = true;
				Unmake(blade);
			}
		}

		private void Land(Blade blade)
		{
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node parent = (Node)(FxKit.Alive((GodotObject)(object)C.FrontParent) ? ((object)C.FrontParent) : ((object)Front));
				if (blade.Final)
				{
					_finalDue = false;
					BreakMark();
					FinisherFx.Finale(parent, C.BackParent, blade.Spot, _feetAt, _side, _k, _half, _total, cross: false, _victim);
					FinisherFx.Shards(parent, blade.Spot, blade.Dir, 12, 1.5f * _k, heavy: true);
				}
				else
				{
					FinisherFx.CardCut(parent, blade.Pin, blade.Dir, _k, _half);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherHeld.Land: " + ex.Message);
			}
		}

		private void BeginDissolve()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			if (_phase == Phase.Dissolve || _phase == Phase.Over)
			{
				return;
			}
			_markOff = true;
			foreach (Blade blade in _blades)
			{
				blade.FromPos = blade.Pos;
				blade.FromAlpha = blade.Alpha;
			}
			_phase = Phase.Dissolve;
			_clock = 0f;
		}

		private void TickDissolve(float dt)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Clamp(_clock / 0.22f, 0f, 1f);
			foreach (Blade blade in _blades)
			{
				if (!blade.Spent && blade.Rig != null)
				{
					blade.Pos = blade.FromPos + new Vector2(0f, -14f * num);
					blade.Alpha = blade.FromAlpha * (1f - num);
					Put(blade);
					Trail(blade, dt);
				}
			}
			if (!(num < 1f))
			{
				FreeBlades();
				_phase = Phase.Over;
				Leave(10f);
			}
		}

		public override void OnCardFinished()
		{
			try
			{
				if (_phase == Phase.Gather)
				{
					_closing = true;
				}
				else if (_phase == Phase.Standby)
				{
					Leave();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherHeld.OnCardFinished: " + ex.Message);
				Finish(10f);
			}
		}

		private static void Unmake(Blade blade)
		{
			try
			{
				if (FxKit.Alive((GodotObject)(object)blade.Streak))
				{
					((Node)blade.Streak).QueueFree();
				}
				blade.Streak = null;
				if (FxKit.Alive((GodotObject)(object)blade.Aura))
				{
					((Node)blade.Aura).QueueFree();
				}
				blade.Aura = null;
				FinisherCard rig = blade.Rig;
				blade.Rig = null;
				rig?.Free();
			}
			catch (Exception ex)
			{
				DevLog.Warn("FinisherHeld (free): " + ex.Message);
			}
		}

		private void FreeBlades()
		{
			foreach (Blade blade in _blades)
			{
				blade.Spent = true;
				Unmake(blade);
			}
			_blades.Clear();
		}

		private void Clear()
		{
			FreeBlades();
			_spare.Clear();
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
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			if (_finalDue)
			{
				_finalDue = false;
				try
				{
					if (C != null && FxKit.Alive((GodotObject)(object)C.FrontParent) && C.FrontParent.IsInsideTree())
					{
						FinisherFx.Finale(C.FrontParent, C.BackParent, _strikeAt, _feetAt, _side, _k, _half, _total, cross: false);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("FinisherHeld.OnEnd (blow): " + ex.Message);
				}
			}
			try
			{
				Clear();
			}
			catch (Exception ex2)
			{
				DevLog.Warn("FinisherHeld.OnEnd: " + ex2.Message);
			}
			if (FxKit.Alive((GodotObject)(object)_layer))
			{
				((Node)_layer).QueueFree();
			}
			_layer = null;
			_under = null;
			_edges = null;
			_lights = null;
			_mark = null;
			_pip = null;
		}

		public FinisherHeld()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		}
	}
