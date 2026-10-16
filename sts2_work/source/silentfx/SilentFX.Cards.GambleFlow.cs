// namespace: SilentFX.Cards
// type: GambleFlow (class)
// source lines: 60411-64049
	public static class GambleFlow
	{
		private struct DeckLook
		{
			public float Width;

			public float Shrink;

			public float Rise;

			public float Dark;

			public float Pitch;

			public float Thick;

			public float Shadow;

			public static DeckLook Read()
			{
				DeckLook result = new DeckLook
				{
					Width = SideWidth(),
					Shrink = Mathf.Clamp(Tuning.F("gamble.depth_scale", 0.018f), 0f, 0.05f),
					Rise = Mathf.Clamp(Tuning.F("gamble.depth_rise", 3f), -12f, 12f),
					Dark = Mathf.Clamp(Tuning.F("gamble.depth_dark", 0.74f), 0.3f, 1f),
					Thick = ((Tuning.F("gamble.edge", 1f) > 0.5f) ? Mathf.Clamp(Tuning.F("gamble.thick", 9f), 0f, 30f) : 0f),
					Shadow = Mathf.Clamp(Tuning.F("gamble.edge_shadow", 0.42f), 0f, 1f)
				};
				float num = Mathf.Tan(Mathf.DegToRad(Mathf.Clamp(Tuning.F("gamble.skew", 10f), -35f, 35f)));
				float num2 = Mathf.Sqrt(Mathf.Max(0f, 1f - result.Width * result.Width));
				result.Pitch = ((num2 > 0.05f) ? (num * result.Width / num2) : 0f);
				return result;
			}
		}

		private struct Turned
		{
			public float Width;

			public float Size;

			public float Rise;

			public float Light;

			public float Lean;
		}

		private struct Formation
		{
			public Vector2 Centre;

			public float Rotation;

			public float Scale;

			public float Spacing;

			public int Count;

			public Vector2 CardScale;

			public float Turn;

			public float Unit;

			public DeckLook Look;

			public Vector2 Squash;

			public Vector2 Drag;

			public float Lead;

			public Vector2 Size
			{
				get
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					return CardScale * Scale;
				}
			}

			public void Pose(int index, out Vector2 at, out Vector2 x, out Vector2 y, out float light)
			{
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_015e: Unknown result type (might be due to invalid IL or missing references)
				//IL_011c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0122: Unknown result type (might be due to invalid IL or missing references)
				//IL_0129: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0133: Unknown result type (might be due to invalid IL or missing references)
				Turned turned = TurnOf(in Look, index, Count, Turn);
				float num = ((float)index - (float)(Count - 1) * 0.5f) * Spacing * (1f + Squash.X) * Scale;
				float num2 = Mathf.Clamp(1f - Squash.Y, 0.3f, 1.5f);
				float num3 = 211f * CardScale.Y * Scale * turned.Size * (1f - num2);
				Vector2 centre = Centre;
				Vector2 val = new Vector2(num, num3 - turned.Rise * Unit * Scale);
				at = centre + val.Rotated(Rotation);
				if (Count > 1)
				{
					float num4 = (float)index / (float)(Count - 1);
					float num5 = Mathf.Lerp(num4, 1f - num4, (Mathf.Clamp(Lead, -1f, 1f) + 1f) * 0.5f);
					at -= Drag * num5;
				}
				Axes(new Vector2(CardScale.X * Scale, CardScale.Y * Scale * num2), Rotation, in turned, out x, out y);
				light = turned.Light;
			}
		}

		private sealed class EdgeRig
		{
			public Node2D Root;

			public Polygon2D Shade;
		}

		private sealed class Flow
		{
			public int N;

			public int Hand;

			public int Known;

			public bool Late;

			public bool Waiting;

			public bool ShuffleSeen;

			public float ShuffleAge;

			public float ShuffleWait;

			public bool Held;

			public AbstractModel Cause;

			public Player Owner;

			public HandFlow.DrawInfo Draw;

			public Node2D Layer;

			public Node2D Under;

			public Node2D OldLayer;

			public Node2D NewLayer;

			public DeckLook Look;

			public CardModel[] Discards;

			public int[] SlotOf;

			public bool[] Claimed;

			public bool[] Flown;

			public bool[] Departed;

			public NCard[] Gone;

			public int OldCount;

			public NCard[] OldCopy;

			public Node2D[] OldRig;

			public EdgeRig[] OldEdge;

			public CardModel[] OldCard;

			public Vector2[] OldAt;

			public Vector2[] OldX;

			public Vector2[] OldY;

			public float[] OldLight;

			public Vector2[] OldOff;

			public Vector2 OldCentre;

			public Vector2 OldBase;

			public float OldOpen;

			public bool OldTrailed;

			public bool OldGone;

			public Vector2 OldPrev;

			public Vector2 OldVel;

			public bool OldMoving;

			public Sprite2D OldShade;

			public CardModel[] Predicted;

			public NCard[] Copy;

			public Node2D[] Rig;

			public EdgeRig[] Edge;

			public int Made;

			public Vector2[] Centre;

			public float[] Angle;

			public Vector2[] Scale;

			public NCard[] Real;

			public float[] RealFor;

			public float[] Settle;

			public bool[] Swapped;

			public Vector2 RowCentre;

			public Vector2 RowScale;

			public float Unit = 1f;

			public bool NewMade;

			public bool Launched;

			public bool Landed;

			public bool Shown;

			public float AfterShow;

			public Vector2 NewPrev;

			public Vector2 NewVel;

			public bool NewMoving;

			public Sprite2D NewShade;

			public readonly List<NCard> Hidden = new List<NCard>();

			public bool Pinned;

			public bool Broken;

			public bool Closing;

			public float ClosingFor;

			public float Age;

			public float Clock;

			public float Timeout;

			public float SquareTime;

			public float WindTime;

			public float OutTime;

			public float InAt;

			public float InTime;

			public float HoldTime;

			public float OpenTime;

			public float GatherTime;

			public float AgainTime;

			public float FanTime;

			public float Stagger;

			public float Ripple;

			public float Tight;

			public float Open;

			public float Lift;

			public float PileScale;

			public float Arc;

			public float Lean;

			public float Breathe;

			public float SpreadOver;

			public float GatherOver;

			public float FanOver;

			public float LandOver;

			public float LandSquash;

			public float WindPx;

			public float TrailPx;

			public float TrailTime;

			public float Ground;

			public bool Dust;

			public float SwapPx;

			public float SwapTurn;

			public float SwapWait;

			public float SettleTime;

			public float Flash;

			public bool PreHide;

			public bool Streak;
		}

		private sealed class Turnee
		{
			public NCard Node;

			public CardModel Card;

			public CanvasItem Home;

			public NHandCardHolder Holder;

			public bool Wanted;

			public bool Going;

			public bool Taken;

			public float TakenAt;

			public int Calm;

			public int Index;

			public int Count;

			public float Turn;

			public float Speed;

			public Vector2 Base = Vector2.One;

			public Vector2 Origin;

			public Vector2 Sized = Vector2.One;

			public Vector2 Moved;

			public float Light = 1f;

			public Vector2 Wrote = Vector2.One;

			public float Up;

			public float Hit;

			public float HitAt = -100f;

			public bool Placed;

			public Vector2 Seen;

			public bool Moving;

			public Vector2 From;

			public Vector2 Aim;

			public float MoveAt;

			public float MoveTime;

			public float MoveOver;

			public bool MoveRush;

			public float Fade;

			public CardDeco.Handle Edge;

			public int EdgeTries;

			public bool NoEdge;

			public Turnee()
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			}
		}

		private const int MaxHand = 10;

		private const float CardWide = 300f;

		private const float CardTall = 422f;

		private const float DepthSteps = 8f;

		private const float FaceTight = 30f;

		private const float FaceOpen = 150f;

		private const string EdgeKey = "gamble.edge";

		private const float ShadeReach = 1.9f;

		private static readonly Color Pale = new Color(0.9f, 0.95f, 1f, 1f);

		private static readonly Color StreakColor = new Color(0.86f, 0.93f, 1f, 0.5f);

		private static readonly Color Stock = new Color(0.9f, 0.87f, 0.78f, 1f);

		private static readonly Color StockLine = new Color(0.25f, 0.22f, 0.2f, 1f);

		private static readonly List<HandCards.Entry> Scratch = new List<HandCards.Entry>();

		private static readonly List<HandCards.Entry> Counted = new List<HandCards.Entry>();

		private static readonly List<HandCards.Entry> InHand = new List<HandCards.Entry>();

		private static Flow _flow;

		private static bool _ready;

		private const float Never = -100f;

		private const float RowHitTime = 0.06f;

		private static readonly List<Turnee> Turnees = new List<Turnee>();

		private static bool _sideways;

		private static bool _prepared;

		private static float _preparedAt = -100f;

		private static float _breathTime;

		private static float _breathPx;

		private static bool _quick;

		private static float _backRate = 14f;

		private static float _backStagger;

		private static bool _turnFailed;

		private static bool _placing;

		private static float _rowClock;

		private static float _closedAt = -100f;

		private static float _openedAt = -100f;

		private static float _facedAt = -100f;

		private static float _rowTight;

		private static bool HeldTurn => Tuning.F("gamble.held_turn", 1f) > 0.5f;

		public static void Init()
		{
			if (!_ready)
			{
				_ready = true;
				HandFlow.DiscardBegins += OnDiscardBegins;
				HandFlow.CardDiscarded += OnCardDiscarded;
				HandFlow.DrawBegins += OnDrawBegins;
				HandFlow.DrawnCardEntered += OnDrawnCardEntered;
				HandFlow.DrawEnded += OnDrawEnded;
				HandFlow.DiscardEnded += OnDiscardEnded;
				HandFlow.SelectionBegan += OnSelection;
				HandFlow.ShuffleBegins += OnShuffleBegins;
				HandFlow.ShuffleEnded += OnShuffleEnded;
				Ticker.Add(Update);
			}
		}

		public static void Reset()
		{
			Finish("reset");
			GambleHeld.Forget();
			FaceAll();
			_turnFailed = false;
			_placing = false;
			_rowClock = 0f;
			_closedAt = -100f;
			_openedAt = -100f;
			_facedAt = -100f;
			_preparedAt = -100f;
		}

		private static float SideWidth()
		{
			return Mathf.Clamp(Tuning.F("gamble.side", 0.11f), 0.04f, 1f);
		}

		private static float HandScale(int count)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				return Mathf.Clamp(HandPosHelper.GetScale(Mathf.Clamp(count, 1, 10)).X, 0.3f, 1.5f);
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow (hand scale): " + ex.Message);
			}
			return 0.8f;
		}

		internal static float DeckTight(int count)
		{
			return Mathf.Clamp(Tuning.F("gamble.pack_tight", 10f), 3f, 80f) * HandScale(count);
		}

		internal static float DeckOpen(int count)
		{
			float num = Mathf.Clamp(Tuning.F("gamble.pack_open", 78f), 10f, 300f) * HandScale(count);
			if (count > 1)
			{
				num = Mathf.Min(num, Mathf.Max(200f, Tuning.F("gamble.deck_width", 760f)) / (float)(count - 1));
			}
			return Mathf.Max(DeckTight(count) * 1.5f, num);
		}

		internal static float RowLift()
		{
			return Mathf.Clamp(Tuning.F("gamble.row_lift", 0f), -200f, 300f);
		}

		internal static float HeldTight(int count)
		{
			if (!HeldTurn)
			{
				return 30f;
			}
			return DeckTight(count);
		}

		internal static float HeldOpen(int count)
		{
			if (HeldTurn)
			{
				return DeckOpen(count);
			}
			if (count <= 1)
			{
				return 150f;
			}
			return Mathf.Min(150f, 1000f / (float)(count - 1));
		}

		internal static int HandCount()
		{
			HandCards.Fill(Counted);
			return Counted.Count;
		}

		private static Turned TurnOf(in DeckLook look, int index, int count, float turn)
		{
			float num = Mathf.Clamp(turn, -0.25f, 1.25f);
			turn = Mathf.Clamp(turn, 0f, 1f);
			float num2 = Mathf.Clamp((float)(count - 1 - index), 0f, 8f);
			float num3 = Mathf.Max(look.Width * 0.45f, Mathf.Lerp(1f, look.Width, num));
			return new Turned
			{
				Width = num3,
				Size = 1f - look.Shrink * num2 * turn,
				Rise = look.Rise * num2 * turn,
				Light = 1f - (1f - look.Dark) * (num2 / 8f) * turn,
				Lean = look.Pitch * Mathf.Sqrt(Mathf.Max(0f, 1f - num3 * num3))
			};
		}

		private static void Axes(Vector2 size, float rotation, in Turned turned, out Vector2 x, out Vector2 y)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = new Vector2(size.X * turned.Size * turned.Width, (0f - size.X) * turned.Size * turned.Lean);
			x = val.Rotated(rotation);
			val = new Vector2(0f, size.Y * turned.Size);
			y = val.Rotated(rotation);
		}

		private static float Smooth(float t)
		{
			return t * t * (3f - 2f * t);
		}

		private static float Rush(float t)
		{
			return t * t * (1.6f - 0.6f * t);
		}

		private static float OutBack(float t, float s)
		{
			float num = Mathf.Clamp(t, 0f, 1f) - 1f;
			return 1f + (s + 1f) * num * num * num + s * num * num;
		}

		private static float Spring(float u, float over, float peak = 0.45f)
		{
			if (u <= 0f)
			{
				return 0f;
			}
			if (u >= 1f)
			{
				return 1f;
			}
			if (over < 0.005f)
			{
				return Smooth(u);
			}
			float num = Mathf.Log(Mathf.Clamp(over, 0.005f, 0.6f));
			float num2 = (0f - num) / Mathf.Sqrt(9.869605f + num * num);
			float num3 = (float)Math.PI / Mathf.Clamp(peak, 0.15f, 0.9f);
			float num4 = num2 / Mathf.Sqrt(1f - num2 * num2) * num3;
			float num5 = 1f - Mathf.Exp((0f - num4) * u) * (Mathf.Cos(num3 * u) + num4 / num3 * Mathf.Sin(num3 * u));
			float num6 = Mathf.Clamp((u - 0.8f) / 0.2f, 0f, 1f);
			return Mathf.Lerp(num5, 1f, num6 * num6 * (3f - 2f * num6));
		}

		private static float Pulse(float v)
		{
			if (!(v <= 0f))
			{
				return v * v * Mathf.Exp(2f * (1f - Mathf.Min(v, 40f)));
			}
			return 0f;
		}

		private static float Bump(float v)
		{
			float num = Mathf.Sin((float)Math.PI * Mathf.Clamp(v, 0f, 1f));
			return num * num;
		}

		private static void Sprung(ref float x, ref float speed, float target, float rate, float damping, float dt)
		{
			dt = Mathf.Min(dt, 0.1f);
			if (!(dt <= 0f))
			{
				int num = Mathf.Clamp(Mathf.CeilToInt(dt * rate / 0.25f), 1, 24);
				float num2 = dt / (float)num;
				for (int i = 0; i < num; i++)
				{
					speed += (rate * rate * (target - x) - 2f * damping * rate * speed) * num2;
					x += speed * num2;
				}
			}
		}

		private static float Part(float u, float start, float length)
		{
			if (!(length <= 0.0001f))
			{
				return Mathf.Clamp((u - start) / length, 0f, 1f);
			}
			if (!(u >= start))
			{
				return 0f;
			}
			return 1f;
		}

		private static float Seconds(string key, float def, float pace)
		{
			return Mathf.Clamp(Tuning.F(key, def), 0.02f, 1f) * pace;
		}

		private static bool Sly(CardModel card)
		{
			try
			{
				return card != null && card.IsSlyThisTurn;
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow (sly): " + ex.Message);
			}
			return false;
		}

		private static Node2D Sub(Node parent, string name)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected Obj, but got Unknown
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit(name)
			};
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			return val;
		}

		private static Vector2 ArcPoint(Flow flow, Vector2 from, Vector2 to)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Clamp(from.DistanceTo(to) * flow.Arc, 0f, 220f);
			return (from + to) * 0.5f + new Vector2(0f, 0f - num);
		}

		private static float Heading(Vector2 from, Vector2 ctrl, Vector2 to, float e)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			e = Mathf.Clamp(e, 0f, 1f);
			Vector2 val = (ctrl - from) * (1f - e) + (to - ctrl) * e;
			if (val.LengthSquared() < 0.01f)
			{
				return 0f;
			}
			if (val.X < 0f)
			{
				val = -val;
			}
			return Mathf.Clamp(val.Angle(), -1f, 1f);
		}

		private static void Trail(Flow flow, Vector2 from, Vector2 to, float time, bool arriving)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (flow.Streak && FxKit.Alive((GodotObject)(object)flow.Under))
				{
					Vector2 ctrl = ArcPoint(flow, from, to);
					float over = flow.LandOver;
					Brush.Follow((Node)(object)flow.Under, (float head) =>
					{
						//IL_0056: Unknown result type (might be due to invalid IL or missing references)
						//IL_005c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0062: Unknown result type (might be due to invalid IL or missing references)
						//IL_0068: Unknown result type (might be due to invalid IL or missing references)
						float t = (Mathf.Sqrt(0.3025f + 1.8f * Mathf.Clamp(head, 0f, 1f)) - 0.55f) / 0.9f;
						float t2 = (arriving ? Mathf.Min(1f, OutBack(t, over)) : Rush(t));
						return FxKit.Bezier(from, ctrl, to, t2);
					}, 0.3f, 16f, StreakColor, time);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow.Trail: " + ex);
			}
		}

		private static void PaleFlash(Vector2 at, float width, float alpha, float flat = 0.55f)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node uiLayer = GameRefs.UiLayer;
				if (FxKit.Alive((GodotObject)(object)uiLayer) && uiLayer.IsInsideTree() && !(alpha <= 0.01f))
				{
					Sprite2D val = FxKit.Glow(new Color(Pale.R, Pale.G, Pale.B, Mathf.Clamp(alpha, 0f, 1f)), new Vector2(width, width * flat));
					uiLayer.AddChild((Node)(object)val, false, (InternalMode)0);
					((Node2D)val).GlobalPosition = at;
					Tween val2 = ((Node)val).CreateTween();
					val2.SetParallel(true);
					val2.TweenProperty((GodotObject)(object)val, NodePath.op_Implicit("scale"), Variant.op_Implicit(((Node2D)val).Scale * 1.6f), 0.24).SetTrans((TransitionType)4).SetEase((EaseType)1);
					val2.TweenProperty((GodotObject)(object)val, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.24);
					FxKit.FreeAfter((Node)(object)val, 0.33999999999999997);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow.PaleFlash: " + ex);
			}
		}

		private static void Puff(Flow flow, Vector2 at)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				PaleFlash(at, 170f * flow.Unit, flow.Flash * 0.9f, 1f);
				Node uiLayer = GameRefs.UiLayer;
				if (flow.Dust && FxKit.Alive((GodotObject)(object)uiLayer) && uiLayer.IsInsideTree())
				{
					Brush.Motes(uiLayer, at, new Color(Pale.R, Pale.G, Pale.B, 0.55f), 6, 16f, 90f, 0.45);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow.Puff: " + ex);
			}
		}

		private static void Land(Flow flow, float tight)
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Sfx.Play("gamble_shuffle");
				float num = GroundLine(flow.RowCentre.Y + 211f * flow.RowScale.Y);
				float num2 = Mathf.Clamp(tight * (float)(flow.Hand - 1) + 300f * flow.Unit, 240f, 640f);
				PaleFlash(new Vector2(flow.RowCentre.X, (flow.RowCentre.Y + num) * 0.5f), num2, flow.Flash * 0.7f);
				PaleFlash(new Vector2(flow.RowCentre.X, num), num2 * 1.5f, flow.Flash, 0.1f);
				Node uiLayer = GameRefs.UiLayer;
				if (flow.Dust && FxKit.Alive((GodotObject)(object)uiLayer) && uiLayer.IsInsideTree())
				{
					Brush.Dust(uiLayer, new Vector2(flow.RowCentre.X - num2 * 0.18f, num), -1f, 5, 0.8f);
					Brush.Dust(uiLayer, new Vector2(flow.RowCentre.X + num2 * 0.18f, num), 1f, 5, 0.8f);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow.Land: " + ex);
			}
		}

		private static float GroundLine(float bottom)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return Mathf.Min(bottom, FxKit.ViewSize().Y - 5f);
		}

		private static void Ground(Flow flow, ref Sprite2D sprite, float left, float right, float bottom, float alpha)
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected Obj, but got Unknown
			alpha = Mathf.Clamp(alpha * flow.Ground, 0f, 1f);
			if (sprite == null)
			{
				if (alpha <= 0.01f || !FxKit.Alive((GodotObject)(object)flow.Under))
				{
					return;
				}
				sprite = new Sprite2D
				{
					Name = StringName.op_Implicit("Ground"),
					Texture = FxKit.SoftDot
				};
				((Node)flow.Under).AddChild((Node)(object)sprite, false, (InternalMode)0);
			}
			if (FxKit.Alive((GodotObject)(object)sprite))
			{
				((Node2D)sprite).GlobalPosition = new Vector2((left + right) * 0.5f, GroundLine(bottom));
				((Node2D)sprite).Scale = new Vector2(Mathf.Max(60f, right - left + 110f * flow.Unit), 36f * flow.Unit) / 64f;
				((CanvasItem)sprite).Modulate = new Color(0f, 0f, 0f, alpha);
			}
		}

		private static EdgeRig MakeEdge()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected Obj, but got Unknown
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected Obj, but got Unknown
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02de: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Expected Obj, but got Unknown
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_039f: Expected Obj, but got Unknown
			float num = 211f * Mathf.Clamp(Tuning.F("gamble.edge_height", 0.965f), 0.5f, 1.05f);
			float num2 = Mathf.Clamp(Tuning.F("gamble.edge_round", 14f), 2f, 60f);
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("Edge")
			};
			Polygon2D val2 = new Polygon2D();
			val2.Polygon = new Vector2[4]
			{
				new Vector2(-2.9f, 0f - num + num2),
				new Vector2(-0.9f, 0f - num + num2 * 0.5f),
				new Vector2(-0.9f, num - num2 * 0.5f),
				new Vector2(-2.9f, num - num2)
			};
			val2.VertexColors = new Color[4]
			{
				new Color(0f, 0f, 0f, 0f),
				new Color(0f, 0f, 0f, 1f),
				new Color(0f, 0f, 0f, 1f),
				new Color(0f, 0f, 0f, 0f)
			};
			Polygon2D val3 = val2;
			((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
			Vector2[] array = new Vector2[10];
			for (int i = 0; i <= 4; i++)
			{
				float num3 = (float)Math.PI / 2f * (float)i / 4f;
				float num4 = -0.14999998f - 0.85f * Mathf.Sin(num3);
				float num5 = num2 * Mathf.Cos(num3);
				array[i] = new Vector2(num4, 0f - num + num2 - num5);
				array[array.Length - 1 - i] = new Vector2(num4, num - num2 + num5);
			}
			Vector2[] array2 = new Vector2[array.Length + 2];
			Color[] array3 = new Color[array2.Length];
			array2[0] = new Vector2(0.3f, 0f - num);
			for (int j = 0; j < array.Length; j++)
			{
				array2[j + 1] = array[j];
			}
			array2[^1] = new Vector2(0.3f, num);
			for (int k = 0; k < array2.Length; k++)
			{
				float num6 = Mathf.Lerp(0.8f, 1f, Mathf.Clamp(array2[k].X + 1f, 0f, 1f));
				array3[k] = new Color(Stock.R * num6, Stock.G * num6, Stock.B * num6, 1f);
			}
			((Node)val).AddChild((Node)new Polygon2D
			{
				Polygon = array2,
				VertexColors = array3,
				Antialiased = true
			}, false, (InternalMode)0);
			Vector2[] array4 = new Vector2[array.Length * 2];
			for (int l = 0; l < array.Length; l++)
			{
				array4[l] = array[l];
				array4[array4.Length - 1 - l] = array[l] + new Vector2(0.14f, 0f);
			}
			((Node)val).AddChild((Node)new Polygon2D
			{
				Polygon = array4,
				Color = new Color(StockLine.R, StockLine.G, StockLine.B, 0.9f),
				Antialiased = true
			}, false, (InternalMode)0);
			return new EdgeRig
			{
				Root = val,
				Shade = val3
			};
		}

		private static void SetEdge(EdgeRig edge, Vector2 at, float wide, float tall, float light, float alpha, float shade)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			Node2D root = edge.Root;
			if (!FxKit.Alive((GodotObject)(object)root))
			{
				return;
			}
			if (wide < 0.002f || alpha <= 0.003f)
			{
				if (((CanvasItem)root).Visible)
				{
					((CanvasItem)root).Visible = false;
				}
				return;
			}
			root.Position = at;
			root.Scale = new Vector2(wide, tall);
			((CanvasItem)root).Modulate = new Color(light, light, light, alpha);
			if (!((CanvasItem)root).Visible)
			{
				((CanvasItem)root).Visible = true;
			}
			if (FxKit.Alive((GodotObject)(object)edge.Shade))
			{
				((CanvasItem)edge.Shade).Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(shade, 0f, 1f));
			}
		}

		private static NCard MakeCopy(CardModel card, Node2D layer, out Node2D rig, out EdgeRig edge)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected Obj, but got Unknown
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			rig = null;
			edge = null;
			if (card == null || !FxKit.Alive((GodotObject)(object)layer))
			{
				return null;
			}
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("Card")
			};
			((Node)layer).AddChild((Node)(object)val, false, (InternalMode)0);
			EdgeRig edgeRig = null;
			if (Tuning.F("gamble.edge", 1f) > 0.5f)
			{
				try
				{
					edgeRig = MakeEdge();
					((CanvasItem)edgeRig.Root).Visible = false;
					((Node)val).AddChild((Node)(object)edgeRig.Root, false, (InternalMode)0);
				}
				catch (Exception ex)
				{
					DevLog.Warn("GambleFlow (edge): " + ex.Message);
					edgeRig = null;
				}
			}
			NCard val2 = CardStandIn.Make(card, (Node)(object)val);
			if (!FxKit.Alive((GodotObject)(object)val2))
			{
				((Node)val).QueueFree();
				return null;
			}
			((Control)val2).Position = Vector2.Zero;
			((Control)val2).Rotation = 0f;
			((Control)val2).Scale = Vector2.One;
			rig = val;
			edge = edgeRig;
			return val2;
		}

		private static void Put(Node2D rig, NCard copy, EdgeRig edge, Vector2 at, Vector2 x, Vector2 y, float light, float alpha, in DeckLook look)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)rig) && FxKit.Alive((GodotObject)(object)copy))
			{
				Vector2 val;
				if (!FxKit.Alive((GodotObject)(object)copy.Body))
				{
					val = Vector2.One;
				}
				else
				{
					Vector2 scale = copy.Body.Scale;
					val = scale.Abs();
				}
				Vector2 val2 = val;
				float num = Mathf.Max(0.001f, val2.X);
				float num2 = Mathf.Max(0.001f, val2.Y);
				rig.GlobalTransform = new Transform2D(x / num, y / num2, at);
				((CanvasItem)copy).Modulate = new Color(light, light, light, alpha);
				if (edge != null)
				{
					float num3 = x.Length();
					float num4 = y.Length();
					float num5 = ((num4 > 0.0001f) ? Mathf.Clamp(num3 / num4, 0f, 1f) : 1f);
					float num6 = Mathf.Sqrt(Mathf.Max(0f, 1f - num5 * num5));
					float wide = look.Thick * num4 * num6 * num / Mathf.Max(0.0001f, num3);
					SetEdge(edge, new Vector2(-150f * num, 0f), wide, num2, light, alpha, look.Shadow * num6);
				}
			}
		}

		private static void FreeCopy(NCard copy, Node2D rig, CardModel shown)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (copy != null)
			{
				try
				{
					if (FxKit.Alive((GodotObject)(object)copy) && copy.Model == shown)
					{
						((CanvasItem)copy).Modulate = Colors.White;
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("GambleFlow (tint): " + ex.Message);
				}
				CardStandIn.Free(copy);
			}
			try
			{
				if (FxKit.Alive((GodotObject)(object)rig))
				{
					if (copy != null && GodotObject.IsInstanceValid((GodotObject)(object)copy) && (object)((Node)copy).GetParent() == rig)
					{
						((Node)rig).RemoveChild((Node)(object)copy);
					}
					((Node)rig).QueueFree();
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("GambleFlow (rig): " + ex2.Message);
			}
		}

		private static void OnDiscardBegins(IReadOnlyList<CardModel> cards, AbstractModel cause, int cardsToDraw)
		{
			try
			{
				if (_flow != null)
				{
					Finish("another discard began");
				}
				if (cause is CalculatedGamble)
				{
					bool flag = GambleHeld.HandOver();
					bool flag2 = false;
					try
					{
						flag2 = Begin(cards, cause, cardsToDraw);
					}
					catch (Exception ex)
					{
						DevLog.Warn("GambleFlow.Begin: " + ex);
						Finish("failed to begin");
					}
					Sideways(on: false, quick: true);
					if ((!flag2 & flag) && HandHold.Lined)
					{
						HandHold.Release();
					}
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("GambleFlow.OnDiscardBegins: " + ex2);
				Finish("failed to begin");
				Sideways(on: false, quick: true);
			}
		}

		private static bool Begin(IReadOnlyList<CardModel> cards, AbstractModel cause, int cardsToDraw)
		{
			//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cb: Expected Obj, but got Unknown
			if (cards == null || cardsToDraw <= 0)
			{
				return false;
			}
			if (!HandPace.On || Tuning.F("gamble.flow", 1f) < 0.5f)
			{
				return false;
			}
			if (cardsToDraw > 10 || cards.Count != cardsToDraw || HandCards.Selecting)
			{
				return false;
			}
			Node uiLayer = GameRefs.UiLayer;
			if (!FxKit.Alive((GodotObject)(object)uiLayer) || !uiLayer.IsInsideTree())
			{
				return false;
			}
			for (int i = 0; i < cardsToDraw; i++)
			{
				if (cards[i] == null)
				{
					return false;
				}
			}
			Player owner = cards[0].Owner;
			if (owner == null)
			{
				return false;
			}
			CardPile pile = PileTypeExtensions.GetPile((PileType)1, owner);
			IReadOnlyList<CardModel> readOnlyList = ((pile != null) ? pile.Cards : null);
			if (readOnlyList == null)
			{
				return false;
			}
			int num = Mathf.Min(cardsToDraw, readOnlyList.Count);
			for (int j = 0; j < num; j++)
			{
				if (readOnlyList[j] == null)
				{
					return false;
				}
			}
			bool flag = num < cardsToDraw;
			if (flag && Tuning.F("gamble.reshuffle", 1f) < 0.5f)
			{
				return false;
			}
			float num2 = Mathf.Clamp(HandPace.Scale, 0.2f, 2f);
			bool flag2 = cardsToDraw < 2;
			Flow flow = new Flow
			{
				N = cardsToDraw,
				Hand = cardsToDraw,
				Known = num,
				Late = flag,
				Waiting = flag,
				ShuffleWait = Mathf.Clamp(Tuning.F("gamble.shuffle_wait", 2.5f), 0.5f, 6f),
				Cause = cause,
				Owner = owner,
				Look = DeckLook.Read(),
				Discards = new CardModel[cardsToDraw],
				SlotOf = new int[cardsToDraw],
				Claimed = new bool[cardsToDraw],
				Flown = new bool[cardsToDraw],
				Departed = new bool[cardsToDraw],
				Gone = new NCard[cardsToDraw],
				OldCopy = new NCard[cardsToDraw],
				OldRig = new Node2D[cardsToDraw],
				OldEdge = new EdgeRig[cardsToDraw],
				OldCard = new CardModel[cardsToDraw],
				OldAt = new Vector2[cardsToDraw],
				OldX = new Vector2[cardsToDraw],
				OldY = new Vector2[cardsToDraw],
				OldLight = new float[cardsToDraw],
				OldOff = new Vector2[cardsToDraw],
				Predicted = new CardModel[cardsToDraw],
				Copy = new NCard[cardsToDraw],
				Rig = new Node2D[cardsToDraw],
				Edge = new EdgeRig[cardsToDraw],
				Centre = new Vector2[cardsToDraw],
				Angle = new float[cardsToDraw],
				Scale = new Vector2[cardsToDraw],
				Real = new NCard[cardsToDraw],
				RealFor = new float[cardsToDraw],
				Settle = new float[cardsToDraw],
				Swapped = new bool[cardsToDraw],
				Timeout = (float)cardsToDraw * 0.6f + (flag ? 7f : 3f),
				SquareTime = Seconds("gamble.square", 0.2f, num2),
				WindTime = Seconds("gamble.windup", 0.06f, num2),
				OutTime = Seconds("gamble.fly_out", 0.3f, num2),
				InTime = Seconds("gamble.fly_in", 0.32f, num2),
				HoldTime = (flag2 ? 0f : Seconds("gamble.hold", 0.1f, num2)),
				OpenTime = (flag2 ? 0f : Seconds("gamble.open", 0.2f, num2)),
				GatherTime = (flag2 ? 0f : Seconds("gamble.regather", 0.14f, num2)),
				AgainTime = ((flag2 || Tuning.F("gamble.reopen", 0f) <= 0f) ? 0f : Seconds("gamble.reopen", 0.2f, num2)),
				FanTime = Seconds("gamble.fan", 0.26f, num2) * (flag2 ? 0.3f : 1f),
				Stagger = (flag2 ? 0f : (Mathf.Clamp(Tuning.F("gamble.fan_stagger", 0.02f), 0f, 0.05f) * num2)),
				Ripple = Mathf.Clamp(Tuning.F("gamble.ripple", 0.012f), 0f, 0.04f) * num2,
				Tight = DeckTight(cardsToDraw),
				Open = DeckOpen(cardsToDraw),
				Lift = RowLift(),
				PileScale = Mathf.Clamp(Tuning.F("gamble.pile_scale", 0.25f), 0.05f, 1f),
				Arc = Mathf.Clamp(Tuning.F("gamble.arc", 0.22f), 0f, 0.6f),
				Lean = Mathf.Clamp(Tuning.F("gamble.lean", 0.45f), 0f, 1f),
				Breathe = Mathf.Clamp(Tuning.F("gamble.breathe_out", 0.07f), 0f, 0.3f),
				SpreadOver = Mathf.Clamp(Tuning.F("gamble.spread_over", 0.12f), 0f, 0.4f),
				GatherOver = Mathf.Clamp(Tuning.F("gamble.gather_over", 0.09f), 0f, 0.4f),
				FanOver = Mathf.Clamp(Tuning.F("gamble.fan_over", 0.06f), 0f, 0.2f),
				LandOver = Mathf.Clamp(Tuning.F("gamble.land_over", 0.9f), 0f, 2.5f),
				LandSquash = Mathf.Clamp(Tuning.F("gamble.land_squash", 0.08f), 0f, 0.25f),
				WindPx = Mathf.Clamp(Tuning.F("gamble.windup_px", 14f), 0f, 60f),
				TrailPx = Mathf.Clamp(Tuning.F("gamble.trail_px", 24f), 0f, 80f),
				TrailTime = Mathf.Clamp(Tuning.F("gamble.trail", 0.014f), 0f, 0.05f),
				Ground = Mathf.Clamp(Tuning.F("gamble.ground", 0.3f), 0f, 1f),
				Dust = (Tuning.F("gamble.dust", 1f) > 0.5f),
				SwapPx = Mathf.Clamp(Tuning.F("gamble.swap_px", 4f), 0.5f, 40f),
				SwapTurn = Mathf.DegToRad(Mathf.Clamp(Tuning.F("gamble.swap_deg", 1f), 0.1f, 20f)),
				SwapWait = Mathf.Clamp(Tuning.F("gamble.swap_wait", 0.5f), 0.1f, 1.5f),
				SettleTime = Mathf.Clamp(Tuning.F("gamble.settle", 0.1f), 0.02f, 0.4f),
				Flash = Mathf.Clamp(Tuning.F("gamble.flash", 0.22f), 0f, 1f),
				PreHide = (Tuning.F("gamble.prehide", 1f) > 0.5f),
				Streak = (Tuning.F("gamble.streak", 1f) > 0.5f)
			};
			flow.InAt = flow.SquareTime + Mathf.Clamp(Tuning.F("gamble.cross", 0.06f), 0f, 1f) * num2;
			for (int k = 0; k < cardsToDraw; k++)
			{
				flow.Discards[k] = cards[k];
				if (k < num)
				{
					flow.Predicted[k] = readOnlyList[k];
				}
			}
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Gamble")
			};
			uiLayer.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return false;
			}
			flow.Layer = val;
			flow.Under = Sub((Node)(object)val, "Streaks");
			flow.OldLayer = Sub((Node)(object)val, "OldHand");
			flow.NewLayer = Sub((Node)(object)val, "NewHand");
			_flow = flow;
			MapSlots(flow);
			flow.Unit = ReadUnit(cardsToDraw);
			BuildOld(flow);
			if (_flow != flow)
			{
				return false;
			}
			HandHold.Release();
			flow.Held = true;
			HandHold.Hold(cardsToDraw, flow.Timeout);
			if (!ReadSlots(flow))
			{
				Finish("no hand slots");
				return false;
			}
			if (PowerFxManager.LogTriggers)
			{
				DevLog.Info($"GambleFlow: run of {cardsToDraw} (old block: {flow.OldCount}, known: {num}, waits for a shuffle: {flag}, pinned: {flow.Pinned})");
			}
			return true;
		}

		private static void OnDiscardFlight(Flow flow, int index)
		{
			if (_flow == flow && index >= 0 && index < flow.N)
			{
				flow.Flown[index] = true;
				flow.Departed[index] = true;
			}
		}

		private static void OnCardDiscarded(CardModel card, AbstractModel cause)
		{
			Flow flow = _flow;
			if (flow != null && card != null)
			{
				int num = DiscardIndex(flow, card);
				if (num >= 0)
				{
					flow.Departed[num] = true;
				}
			}
		}

		private static bool Mine(Flow flow, HandFlow.DrawInfo info)
		{
			if (info == null)
			{
				return false;
			}
			if (flow.Draw == null && info.Cause == flow.Cause && info.Requested == flow.N)
			{
				flow.Draw = info;
			}
			return info == flow.Draw;
		}

		private static void OnDrawBegins(HandFlow.DrawInfo info)
		{
			Flow flow = _flow;
			if (flow != null && info != null && flow.Draw == null)
			{
				Mine(flow, info);
			}
		}

		private static void OnDrawnCardEntered(NCard node, HandFlow.DrawInfo info)
		{
			Flow flow = _flow;
			if (flow == null)
			{
				return;
			}
			try
			{
				if (!FxKit.Alive((GodotObject)(object)node))
				{
					return;
				}
				if (!Mine(flow, info))
				{
					Reveal(flow, node);
					if (!flow.Closing)
					{
						Break(flow, "another draw put a card in the hand");
					}
					return;
				}
				int num = info.Entered - 1;
				if (info.Entered >= flow.Hand)
				{
					flow.Closing = true;
				}
				if (num < 0 || num >= flow.Hand)
				{
					Reveal(flow, node);
					return;
				}
				if (num >= flow.Known)
				{
					Reveal(flow, node);
					Break(flow, "card " + num + " came before the shuffle was over");
					return;
				}
				Raise(flow, all: true);
				int num2 = HandIndex(node);
				if (flow.Broken || node.Model != flow.Predicted[num] || (num2 >= 0 && num2 != num))
				{
					Reveal(flow, node);
					Break(flow, "card " + num + " is not the predicted one");
				}
				else if (flow.Copy[num] == null)
				{
					Reveal(flow, node);
				}
				else
				{
					flow.Real[num] = node;
					flow.RealFor[num] = 0f;
					Conceal(flow, node);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow.OnDrawnCardEntered: " + ex);
				Finish("failed on an arrival");
			}
		}

		private static void OnDrawEnded(HandFlow.DrawInfo info)
		{
			Flow flow = _flow;
			if (flow != null && info != null && ((flow.Draw != null) ? (info == flow.Draw) : (info.Cause == flow.Cause)))
			{
				flow.Closing = true;
			}
		}

		private static void OnDiscardEnded(AbstractModel cause)
		{
			Flow flow = _flow;
			if (flow != null && cause == flow.Cause)
			{
				flow.Closing = true;
			}
		}

		private static void OnSelection(AbstractModel source)
		{
			if (_flow != null)
			{
				Finish("a hand selection began");
			}
		}

		private static void OnShuffleBegins(Player player)
		{
			Flow flow = _flow;
			if (flow == null || flow.Broken || !flow.Waiting)
			{
				return;
			}
			try
			{
				if (player != null && flow.Owner != null && player != flow.Owner)
				{
					return;
				}
				if (!Mine(flow, HandFlow.Drawing))
				{
					Break(flow, "a shuffle outside the card's own draw");
					return;
				}
				flow.ShuffleSeen = true;
				flow.ShuffleAge = flow.Age;
				if (PowerFxManager.LogTriggers)
				{
					DevLog.Info($"GambleFlow: the shuffle begins ({flow.Known} of {flow.N} card(s) known)");
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow.OnShuffleBegins: " + ex);
				Finish("failed on a shuffle");
			}
		}

		private static void OnShuffleEnded(Player player)
		{
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Invalid comparison between Unknown and I4
			Flow flow = _flow;
			if (flow == null || flow.Broken || !flow.Waiting)
			{
				return;
			}
			try
			{
				if (player != null && flow.Owner != null && player != flow.Owner)
				{
					return;
				}
				if (!Mine(flow, HandFlow.Drawing))
				{
					Break(flow, "a shuffle outside the card's own draw");
					return;
				}
				int known = flow.Known;
				if (flow.Draw.Shuffled && flow.Draw.ShuffledAt > known)
				{
					Break(flow, "more cards were drawn before the shuffle than the pile held");
					return;
				}
				for (int i = 0; i < known; i++)
				{
					CardModel obj = flow.Predicted[i];
					CardPile val = ((obj != null) ? obj.Pile : null);
					if (val == null || (int)val.Type != 2)
					{
						Break(flow, "card " + i + " was not drawn before the shuffle");
						return;
					}
				}
				CardPile pile = PileTypeExtensions.GetPile((PileType)1, flow.Owner);
				IReadOnlyList<CardModel> readOnlyList = ((pile != null) ? pile.Cards : null);
				if (readOnlyList == null)
				{
					Break(flow, "no draw pile after the shuffle");
					return;
				}
				int num = Mathf.Min(flow.N - known, readOnlyList.Count);
				for (int j = 0; j < num; j++)
				{
					if (readOnlyList[j] == null)
					{
						Break(flow, "the new pile cannot be read");
						return;
					}
					flow.Predicted[known + j] = readOnlyList[j];
				}
				int num2 = known + num;
				if (num2 <= 0)
				{
					Finish("nothing to draw");
					return;
				}
				flow.Hand = num2;
				flow.Known = num2;
				flow.Waiting = false;
				if (num2 < 2)
				{
					flow.HoldTime = 0f;
					flow.OpenTime = 0f;
					flow.GatherTime = 0f;
					flow.AgainTime = 0f;
					flow.Stagger = 0f;
				}
				flow.Tight = DeckTight(num2);
				flow.Open = DeckOpen(num2);
				flow.Timeout = flow.Age + (float)num2 * 0.6f + 4f;
				if (flow.Held)
				{
					HandHold.Hold(num2, flow.Timeout - flow.Age);
				}
				if (!ReadSlots(flow))
				{
					Finish("no hand slots");
					return;
				}
				if (flow.Draw.Entered >= num2)
				{
					flow.Closing = true;
				}
				flow.InAt = flow.Clock;
				if (PowerFxManager.LogTriggers)
				{
					DevLog.Info($"GambleFlow: the shuffle is over - a new hand of {num2} ({known} drawn before it, {flow.N} asked for)");
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow.OnShuffleEnded: " + ex);
				Finish("failed on a shuffle");
			}
		}

		private static void Update(double realDt)
		{
			float num = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			StepTurn(num);
			Flow flow = _flow;
			if (flow == null)
			{
				return;
			}
			try
			{
				flow.Age += num;
				if (flow.Age > flow.Timeout)
				{
					Finish("timeout");
					return;
				}
				if (flow.Waiting && flow.ShuffleSeen && flow.Age - flow.ShuffleAge > flow.ShuffleWait)
				{
					Finish("the shuffle took too long");
					return;
				}
				CardModel carried = HeldFxManager.Carried;
				if (carried != null && (object)carried != flow.Cause)
				{
					Finish("a card was picked up");
					return;
				}
				if (HandCards.Selecting)
				{
					Finish("a hand selection is open");
					return;
				}
				if (!FxKit.Alive((GodotObject)(object)flow.Layer) || !((Node)flow.Layer).IsInsideTree())
				{
					Finish("the layer is gone");
					return;
				}
				if (!ReadSlots(flow))
				{
					Finish("the hand is gone");
					return;
				}
				flow.Clock += num;
				flow.Look = DeckLook.Read();
				KeepUnseen(flow);
				Pin(flow);
				StepOld(flow, num);
				Raise(flow, !flow.Late && (flow.Draw != null || flow.Clock >= flow.InAt));
				Cover(flow);
				StepSwaps(flow, num);
				StepNew(flow, num);
				if (_flow == flow)
				{
					Close(flow, num);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow.Update: " + ex);
				Finish("failed");
			}
		}

		private static void Close(Flow flow, float dt)
		{
			if (flow.Broken)
			{
				if (!flow.OldGone)
				{
					return;
				}
				bool flag = true;
				if (!flow.Closing)
				{
					for (int i = 0; i < flow.N; i++)
					{
						NCard val = flow.Gone[i];
						if (val != null && !flow.Flown[i] && FxKit.Alive((GodotObject)(object)val) && val.Model == flow.Discards[i])
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					Finish("done");
				}
			}
			else
			{
				if (!flow.Closing)
				{
					return;
				}
				flow.ClosingFor += dt;
				if (flow.Waiting)
				{
					if (flow.ClosingFor > 0.3f)
					{
						Finish("the draw ended without its shuffle");
					}
					return;
				}
				bool flag2 = !flow.OldGone;
				for (int j = 0; j < flow.Hand; j++)
				{
					if (flow.Swapped[j])
					{
						continue;
					}
					if (flow.Real[j] != null)
					{
						flag2 = true;
					}
					else if (flow.Copy[j] != null)
					{
						if (flow.ClosingFor > 0.4f)
						{
							Drop(flow, j);
						}
						else
						{
							flag2 = true;
						}
					}
				}
				if (!flag2)
				{
					Finish("done");
				}
				else if (flow.Shown && Mathf.Min(flow.AfterShow, flow.ClosingFor) > flow.SwapWait + flow.SettleTime + 0.6f)
				{
					Finish("late");
				}
			}
		}

		private static bool ReadSlots(Flow flow)
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			int hand = flow.Hand;
			for (int i = 0; i < hand; i++)
			{
				if (!HandHold.Slot(hand, i, out flow.Centre[i], out flow.Angle[i], out flow.Scale[i]))
				{
					return false;
				}
			}
			if (!HandHold.LineSlot(hand, 0, 100f, flow.Lift, out var centre, out var scale))
			{
				return false;
			}
			if (!HandHold.LineSlot(hand, hand - 1, 100f, flow.Lift, out var centre2, out var _))
			{
				return false;
			}
			flow.RowCentre = (centre + centre2) * 0.5f;
			flow.RowScale = scale;
			if (hand > 1)
			{
				flow.Unit = Mathf.Clamp(centre.DistanceTo(centre2) / (100f * (float)(hand - 1)), 0.2f, 5f);
			}
			return true;
		}

		private static float ReadUnit(int n)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			if (n < 2)
			{
				return 1f;
			}
			if (!HandHold.LineSlot(n, 0, 100f, 0f, out var centre, out var scale))
			{
				return 1f;
			}
			if (!HandHold.LineSlot(n, n - 1, 100f, 0f, out var centre2, out scale))
			{
				return 1f;
			}
			return Mathf.Clamp(centre.DistanceTo(centre2) / (100f * (float)(n - 1)), 0.2f, 5f);
		}

		private static Formation OldRow(Flow flow, Vector2 centre, float rotation, float scale, float spacing)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			return new Formation
			{
				Count = flow.OldCount,
				CardScale = flow.OldBase,
				Centre = centre,
				Rotation = rotation,
				Scale = scale,
				Spacing = spacing,
				Turn = 1f,
				Unit = flow.Unit,
				Look = flow.Look
			};
		}

		private static void BuildOld(Flow flow)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0391: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_0458: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			float duration = Mathf.Clamp(Tuning.F("gamble.discard_time", 0.12f), 0.05f, 0.6f);
			HandCards.Fill(Scratch);
			int num = 0;
			Vector2 val = Vector2.Zero;
			Vector2 x;
			for (int i = 0; i < Scratch.Count; i++)
			{
				if (num >= flow.N)
				{
					break;
				}
				HandCards.Entry entry = Scratch[i];
				int num2 = DiscardIndex(flow, entry.Card);
				if (num2 < 0 || flow.Gone[num2] != null || Sly(entry.Card) || !FxKit.Alive((GodotObject)(object)entry.Node) || !((Node)entry.Node).IsInsideTree())
				{
					continue;
				}
				CardStandIn.Pose(entry.Node, out var centre, out var rotation, out var scale);
				Vector2 val2 = scale;
				float num3 = 1f;
				Turnee turnee = TurneeOf(entry.Node);
				if (turnee != null && turnee.Taken)
				{
					val2 = new Vector2(scale.X / turnee.Sized.X, scale.Y / turnee.Sized.Y);
					num3 = turnee.Light;
				}
				NCard val3 = MakeCopy(entry.Card, flow.OldLayer, out var rig, out var edge);
				if (FxKit.Alive((GodotObject)(object)val3))
				{
					x = new Vector2(scale.X, 0f);
					Vector2 val4 = x.Rotated(rotation);
					x = new Vector2(0f, scale.Y);
					Vector2 val5 = x.Rotated(rotation);
					Put(rig, val3, edge, centre, val4, val5, num3, 1f, in flow.Look);
					flow.OldCopy[num] = val3;
					flow.OldRig[num] = rig;
					flow.OldEdge[num] = edge;
					flow.OldCard[num] = entry.Card;
					flow.OldAt[num] = centre;
					flow.OldX[num] = val4;
					flow.OldY[num] = val5;
					flow.OldLight[num] = num3;
					val += val2;
					num++;
					if (turnee != null)
					{
						Face(turnee);
						Turnees.Remove(turnee);
					}
					flow.Gone[num2] = entry.Node;
					CardStandIn.Hide(entry.Node);
					int index = num2;
					CardFlight.Claim(entry.Card, new CardFlight.Plan
					{
						HideCard = true,
						HideTrail = true,
						Duration = duration,
						Started = (NCard flying, Vector2 start, Vector2 end) =>
						{
							OnDiscardFlight(flow, index);
						}
					}, flow.Timeout);
					flow.Claimed[num2] = true;
				}
			}
			flow.OldCount = num;
			if (num == 0)
			{
				flow.OldGone = true;
				return;
			}
			flow.OldBase = val / (float)num;
			flow.OldOpen = ((num > 1) ? (Mathf.Abs(flow.OldAt[num - 1].X - flow.OldAt[0].X) / (float)(num - 1)) : 0f);
			Formation formation = OldRow(flow, Vector2.Zero, 0f, 1f, flow.OldOpen);
			Vector2 val6 = Vector2.Zero;
			for (int num4 = 0; num4 < num; num4++)
			{
				formation.Pose(num4, out var at, out x, out var _, out var _);
				flow.OldOff[num4] = at;
				val6 += flow.OldAt[num4] - at;
			}
			flow.OldCentre = val6 / (float)num;
			for (int num5 = 0; num5 < num; num5++)
			{
				flow.OldOff[num5] = flow.OldAt[num5] - (flow.OldCentre + flow.OldOff[num5]);
			}
		}

		private static void KeepUnseen(Flow flow)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < flow.N; i++)
			{
				NCard val = flow.Gone[i];
				if (val != null && !flow.Flown[i] && !flow.Departed[i] && FxKit.Alive((GodotObject)(object)val) && val.Model == flow.Discards[i] && ((CanvasItem)val).Modulate.A > 0.01f)
				{
					((CanvasItem)val).Modulate = new Color(1f, 1f, 1f, 0f);
				}
			}
		}

		private static void StepOld(Flow flow, float dt)
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_04db: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
			if (flow.OldGone)
			{
				return;
			}
			int oldCount = flow.OldCount;
			float num = flow.Tight * flow.Unit;
			float num2 = ((oldCount > 2) ? flow.Ripple : 0f);
			float length = Mathf.Max(0.02f, flow.SquareTime - num2 * (float)((oldCount - 1) / 2));
			float num3 = Part(flow.Clock, flow.SquareTime + flow.WindTime, flow.OutTime);
			float num4 = Rush(num3);
			Vector2 val = GameRefs.PilePoint((PileType)3);
			Vector2 val2 = ArcPoint(flow, flow.OldCentre, val);
			if (num3 > 0f && !flow.OldTrailed)
			{
				flow.OldTrailed = true;
				Trail(flow, flow.OldCentre, val, flow.OutTime, arriving: false);
			}
			float num5 = Smooth(Part(flow.Clock, flow.SquareTime - flow.WindTime * 0.5f, flow.WindTime * 1.5f)) * (1f - Smooth(Mathf.Clamp(num3 * 3f, 0f, 1f)));
			Vector2 val3 = val2 - flow.OldCentre;
			Vector2 val4 = ((val3.LengthSquared() > 0.01f) ? (-val3.Normalized() * (flow.WindPx * flow.Unit)) : Vector2.Zero);
			Vector2 val5 = FxKit.Bezier(flow.OldCentre, val2, val, num4) + val4 * num5;
			float num6 = Mathf.Lerp(1f, flow.PileScale, num4);
			float rotation = Heading(flow.OldCentre, val2, val, num4) * flow.Lean * Smooth(Mathf.Clamp(num3 * 4f, 0f, 1f)) - Heading(flow.OldCentre, val2, val, 0f) * 0.2f * num5;
			if (dt > 0.0001f)
			{
				Vector2 val6 = (flow.OldMoving ? ((val5 - flow.OldPrev) / dt) : Vector2.Zero);
				flow.OldVel = flow.OldVel.Lerp(val6, 1f - Mathf.Exp(-28f * dt));
				flow.OldPrev = val5;
				flow.OldMoving = true;
			}
			Formation formation = OldRow(flow, val5, rotation, num6, flow.OldOpen);
			Vector2 val7 = flow.OldVel * flow.TrailTime;
			formation.Drag = val7.LimitLength(flow.TrailPx * flow.Unit) * num6;
			formation.Lead = Mathf.Clamp(flow.OldVel.X / 300f, -1f, 1f);
			float alpha = Mathf.Clamp((1f - num3) * 5f, 0f, 1f);
			float num7 = val5.X;
			float num8 = val5.X;
			for (int i = 0; i < oldCount; i++)
			{
				NCard val8 = flow.OldCopy[i];
				if (val8 == null)
				{
					continue;
				}
				if (!FxKit.Alive((GodotObject)(object)val8) || !((Node)val8).IsInsideTree() || val8.Model != flow.OldCard[i])
				{
					flow.OldCopy[i] = null;
					FreeCopy(val8, flow.OldRig[i], flow.OldCard[i]);
					flow.OldRig[i] = null;
					flow.OldEdge[i] = null;
					continue;
				}
				float num9 = Part(flow.Clock - (float)Mathf.Min(i, oldCount - 1 - i) * num2, 0f, length);
				float num10 = Part(num9, 0.16f, 0.84f);
				float num11 = Spring(num10, flow.GatherOver, 0.5f);
				float num12 = flow.Breathe * Bump(num9 / 0.34f);
				float num13 = 1f - Smooth(num10);
				float num14 = 1f - Smooth(Mathf.Clamp(num10 * 1.6f, 0f, 1f));
				formation.Spacing = Mathf.Max(num * 0.3f, flow.OldOpen + (num - flow.OldOpen) * (num11 - num12));
				formation.Squash = new Vector2(0f, 0.05f * num5 - 0.5f * Mathf.Max(0f, num11 - 1f));
				formation.Pose(i, out var at, out var x, out var y, out var light);
				if (num13 > 0f)
				{
					at += flow.OldOff[i] * num13;
				}
				if (num14 > 0f)
				{
					x = x.Lerp(flow.OldX[i], num14);
					y = y.Lerp(flow.OldY[i], num14);
					light = Mathf.Lerp(light, flow.OldLight[i], num14);
				}
				Put(flow.OldRig[i], val8, flow.OldEdge[i], at, x, y, light, alpha, in flow.Look);
				num7 = Mathf.Min(num7, at.X);
				num8 = Mathf.Max(num8, at.X);
			}
			Ground(flow, ref flow.OldShade, num7, num8, val5.Y + 211f * flow.OldBase.Y * num6, Smooth(Part(flow.Clock, 0f, flow.SquareTime * 0.7f)) * (1f - Smooth(Mathf.Clamp(num3 * 3f, 0f, 1f))));
			if (!(num3 < 1f))
			{
				for (int j = 0; j < oldCount; j++)
				{
					NCard copy = flow.OldCopy[j];
					flow.OldCopy[j] = null;
					FreeCopy(copy, flow.OldRig[j], flow.OldCard[j]);
					flow.OldRig[j] = null;
					flow.OldEdge[j] = null;
				}
				flow.OldGone = true;
				Puff(flow, val);
			}
		}

		private static int DiscardIndex(Flow flow, CardModel card)
		{
			if (card == null)
			{
				return -1;
			}
			for (int i = 0; i < flow.N; i++)
			{
				if (flow.Discards[i] == card)
				{
					return i;
				}
			}
			return -1;
		}

		private static void MapSlots(Flow flow)
		{
			int n = flow.N;
			for (int i = 0; i < n; i++)
			{
				flow.SlotOf[i] = n - 1 - i;
			}
			flow.Pinned = false;
			if (Tuning.F("gamble.pin", 1f) < 0.5f)
			{
				return;
			}
			HandCards.Fill(Scratch);
			if (Scratch.Count != n)
			{
				return;
			}
			int[] array = new int[n];
			bool[] array2 = new bool[n];
			for (int j = 0; j < n; j++)
			{
				int num = -1;
				for (int k = 0; k < n; k++)
				{
					if (Scratch[k].Card == flow.Discards[j])
					{
						num = k;
						break;
					}
				}
				if (num < 0 || array2[num])
				{
					return;
				}
				array2[num] = true;
				array[j] = num;
			}
			for (int l = 0; l < n; l++)
			{
				flow.SlotOf[l] = array[l];
			}
			flow.Pinned = true;
		}

		private static void Pin(Flow flow)
		{
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			if (!flow.Held || !flow.Pinned || flow.Draw != null || flow.Closing)
			{
				return;
			}
			NPlayerHand instance = NPlayerHand.Instance;
			if (!FxKit.Alive((GodotObject)(object)instance) || !((Node)instance).IsInsideTree() || FxKit.Alive((GodotObject)(object)instance.FocusedHolder))
			{
				return;
			}
			HandCards.Fill(Scratch);
			foreach (HandCards.Entry item in Scratch)
			{
				if (!FxKit.Alive((GodotObject)(object)item.Holder))
				{
					continue;
				}
				int num = DiscardIndex(flow, item.Card);
				if (num >= 0)
				{
					int num2 = flow.SlotOf[num];
					Vector2 position = HandPosHelper.GetPosition(flow.N, num2);
					Vector2 targetPosition = item.Holder.TargetPosition;
					if (!(targetPosition.DistanceSquaredTo(position) <= 1f))
					{
						item.Holder.SetTargetPosition(position);
						item.Holder.SetTargetAngle(HandPosHelper.GetAngle(flow.N, num2));
					}
				}
			}
		}

		private static bool StillNext(Flow flow)
		{
			CardPile pile = PileTypeExtensions.GetPile((PileType)1, flow.Owner);
			IReadOnlyList<CardModel> readOnlyList = ((pile != null) ? pile.Cards : null);
			if (readOnlyList == null || readOnlyList.Count < flow.Known)
			{
				return false;
			}
			if (flow.Waiting && readOnlyList.Count != flow.Known)
			{
				return false;
			}
			for (int i = 0; i < flow.Known; i++)
			{
				if (readOnlyList[i] != flow.Predicted[i])
				{
					return false;
				}
			}
			return true;
		}

		private static void Raise(Flow flow, bool all)
		{
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			if (flow.NewMade || flow.Broken)
			{
				return;
			}
			if (flow.Made == 0 && flow.Draw == null && !StillNext(flow))
			{
				Break(flow, "the draw pile changed");
				return;
			}
			int num = (all ? flow.N : Mathf.Clamp((int)Tuning.F("gamble.copies_per_frame", 3f), 1, 10));
			for (int i = 0; i < num; i++)
			{
				if (flow.Made >= flow.Known)
				{
					break;
				}
				int made = flow.Made;
				NCard val = MakeCopy(flow.Predicted[made], flow.NewLayer, out var rig, out var edge);
				if (!FxKit.Alive((GodotObject)(object)val))
				{
					Break(flow, "no copy for card " + made);
					return;
				}
				((CanvasItem)val).Modulate = new Color(1f, 1f, 1f, 0f);
				flow.Copy[made] = val;
				flow.Rig[made] = rig;
				flow.Edge[made] = edge;
				flow.Made++;
			}
			if (!flow.Waiting && flow.Made >= flow.Hand)
			{
				flow.NewMade = true;
			}
		}

		private static void Cover(Flow flow)
		{
			if (!flow.PreHide || flow.Broken || flow.Draw == null)
			{
				return;
			}
			Node uiLayer = GameRefs.UiLayer;
			if (!FxKit.Alive((GodotObject)(object)uiLayer))
			{
				return;
			}
			foreach (Node child in uiLayer.GetChildren(false))
			{
				NCard val = (NCard)(object)((child is NCard) ? child : null);
				if (val == null || !FxKit.Alive((GodotObject)(object)val))
				{
					continue;
				}
				CardModel model = val.Model;
				if (model == null)
				{
					continue;
				}
				for (int i = 0; i < flow.Known; i++)
				{
					if (model == flow.Predicted[i])
					{
						if (flow.Real[i] == null && flow.Copy[i] != null)
						{
							Conceal(flow, val);
						}
						break;
					}
				}
			}
		}

		private static float PackAt(Flow flow, float u, float tight, float open)
		{
			float holdTime = flow.HoldTime;
			if (u <= holdTime)
			{
				return tight;
			}
			float num = holdTime + flow.OpenTime;
			if (u < num)
			{
				return Mathf.Lerp(tight, open, Spring(Part(u, holdTime, flow.OpenTime), flow.SpreadOver));
			}
			float num2 = num + flow.GatherTime;
			if (u < num2)
			{
				return Mathf.Max(tight * 0.35f, Mathf.Lerp(open, tight, Spring(Part(u, num, flow.GatherTime), flow.GatherOver, 0.5f)));
			}
			if (flow.AgainTime <= 0f)
			{
				return tight;
			}
			float num3 = num2 + flow.AgainTime;
			if (u < num3)
			{
				return Mathf.Lerp(tight, open, Spring(Part(u, num2, flow.AgainTime), flow.SpreadOver));
			}
			return open;
		}

		private static void StepNew(Flow flow, float dt)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0656: Unknown result type (might be due to invalid IL or missing references)
			//IL_0658: Unknown result type (might be due to invalid IL or missing references)
			//IL_065a: Unknown result type (might be due to invalid IL or missing references)
			//IL_066d: Unknown result type (might be due to invalid IL or missing references)
			//IL_067d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Unknown result type (might be due to invalid IL or missing references)
			//IL_059c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_062a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_060b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			if (!flow.NewMade || flow.Broken || flow.Waiting)
			{
				return;
			}
			int hand = flow.Hand;
			if (flow.Late && !flow.Launched)
			{
				flow.InAt = flow.Clock;
			}
			float num = flow.Clock - flow.InAt;
			Vector2 val = GameRefs.PilePoint((PileType)1);
			if (num >= 0f && !flow.Launched)
			{
				flow.Launched = true;
				if (flow.Draw == null && !StillNext(flow))
				{
					Break(flow, "the draw pile changed");
					return;
				}
				Trail(flow, val, flow.RowCentre, flow.InTime, arriving: true);
			}
			float num2 = flow.Tight * flow.Unit;
			float num3 = flow.Open * flow.Unit;
			Formation formation = new Formation
			{
				Count = hand,
				CardScale = flow.RowScale,
				Centre = flow.RowCentre,
				Rotation = 0f,
				Scale = 1f,
				Spacing = num2,
				Turn = 1f,
				Unit = flow.Unit,
				Look = flow.Look
			};
			float alpha = 1f;
			if (num < flow.InTime)
			{
				float num4 = Part(num, 0f, flow.InTime);
				float num5 = OutBack(num4, flow.LandOver);
				Vector2 val2 = ArcPoint(flow, val, flow.RowCentre);
				formation.Centre = FxKit.Bezier(val, val2, flow.RowCentre, num5);
				formation.Scale = Mathf.Lerp(flow.PileScale, 1f, OutBack(num4, flow.LandOver * 0.6f));
				formation.Rotation = Heading(val, val2, flow.RowCentre, num5) * flow.Lean * Smooth(Mathf.Clamp((1f - num5) * 3f, 0f, 1f));
				alpha = Mathf.Clamp(num4 * 6f, 0f, 1f);
			}
			float num6 = flow.InTime / (flow.LandOver + 1f);
			float num7 = Pulse((num - num6) / Mathf.Max(0.03f, flow.InTime * 0.16f));
			if (!flow.Landed && num >= num6)
			{
				flow.Landed = true;
				Land(flow, num2);
			}
			if (dt > 0.0001f)
			{
				Vector2 val3 = (flow.NewMoving ? ((formation.Centre - flow.NewPrev) / dt) : Vector2.Zero);
				flow.NewVel = flow.NewVel.Lerp(val3, 1f - Mathf.Exp(-28f * dt));
				flow.NewPrev = formation.Centre;
				flow.NewMoving = true;
			}
			Vector2 val4 = flow.NewVel * flow.TrailTime;
			formation.Drag = val4.LimitLength(flow.TrailPx * flow.Unit) * formation.Scale;
			formation.Lead = Mathf.Clamp(flow.NewVel.X / 300f, -1f, 1f);
			float num8 = num - flow.InTime;
			float num9 = flow.HoldTime + flow.OpenTime + flow.GatherTime + flow.AgainTime;
			float num10 = num8 - num9;
			if (num10 >= flow.FanTime + flow.Stagger * (float)(hand - 1))
			{
				flow.Shown = true;
			}
			if (flow.Shown)
			{
				flow.AfterShow += dt;
			}
			float over = flow.FanOver / Mathf.Max(0.2f, 1f - flow.Look.Width);
			float num11 = formation.Centre.X;
			float num12 = formation.Centre.X;
			for (int i = 0; i < hand; i++)
			{
				NCard val5 = flow.Copy[i];
				if (val5 == null)
				{
					continue;
				}
				if (!FxKit.Alive((GodotObject)(object)val5) || !((Node)val5).IsInsideTree() || val5.Model != flow.Predicted[i])
				{
					Drop(flow, i);
					continue;
				}
				float num13 = ((num8 <= 0f) ? num2 : PackAt(flow, num8 - (float)Mathf.Min(i, hand - 1 - i) * flow.Ripple, num2, num3));
				float num14 = flow.LandSquash * num7;
				if (num13 < num2)
				{
					num14 -= 0.07f * Mathf.Clamp((num2 - num13) / Mathf.Max(0.01f, num2), 0f, 1f);
				}
				else if (num13 > num3)
				{
					num14 += 0.035f * Mathf.Clamp((num13 - num3) / Mathf.Max(0.01f, 0.12f * (num3 - num2)), 0f, 1f);
				}
				formation.Spacing = num13;
				formation.Squash = new Vector2(0.5f * num7, num14);
				float num15 = ((num10 < 0f) ? 0f : Part(num10, (float)i * flow.Stagger, flow.FanTime));
				float num16 = Smooth(num15);
				float num17 = Spring(num15, over, 0.62f);
				formation.Turn = 1f - num17;
				formation.Pose(i, out var at, out var x, out var y, out var light);
				NCard val6 = flow.Real[i];
				bool flag = flow.Settle[i] > 0f && FxKit.Alive((GodotObject)(object)val6);
				if ((num15 > 0f) | flag)
				{
					at = at.Lerp(flow.Centre[i], num16);
					float num18 = Mathf.LerpAngle(formation.Rotation, flow.Angle[i], num16);
					val4 = formation.Size;
					Vector2 size = val4.Lerp(flow.Scale[i], num16);
					if (flag)
					{
						CardStandIn.Pose(val6, out var centre, out var rotation, out var scale);
						float num19 = Smooth(Mathf.Clamp(flow.Settle[i], 0f, 1f));
						at = at.Lerp(centre, num19);
						num18 = Mathf.LerpAngle(num18, rotation, num19);
						size = size.Lerp(scale, num19);
					}
					Turned turned = TurnOf(in flow.Look, i, hand, 1f - num17);
					Axes(size, num18, in turned, out x, out y);
					light = turned.Light;
				}
				Put(flow.Rig[i], val5, flow.Edge[i], at, x, y, light, alpha, in flow.Look);
				num11 = Mathf.Min(num11, at.X);
				num12 = Mathf.Max(num12, at.X);
			}
			Ground(flow, ref flow.NewShade, num11, num12, flow.RowCentre.Y + 211f * flow.RowScale.Y, Smooth(Part(num, num6, flow.InTime * 0.3f)) * (1f - Smooth(Part(num10, 0f, flow.FanTime))));
		}

		private static void StepSwaps(Flow flow, float dt)
		{
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < flow.Hand; i++)
			{
				NCard val = flow.Real[i];
				if (val == null || flow.Swapped[i])
				{
					continue;
				}
				flow.RealFor[i] += dt;
				if (flow.Copy[i] == null || !FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree() || val.Model != flow.Predicted[i])
				{
					Drop(flow, i);
				}
				else
				{
					if (!flow.Shown)
					{
						continue;
					}
					if (flow.Settle[i] > 0f)
					{
						if (flow.Settle[i] >= 1f)
						{
							Drop(flow, i);
						}
						else
						{
							flow.Settle[i] = Mathf.Min(1f, flow.Settle[i] + dt / flow.SettleTime);
						}
						continue;
					}
					CardStandIn.Pose(val, out var centre, out var rotation, out var scale);
					float num = Mathf.Abs(rotation - flow.Angle[i]) % ((float)Math.PI * 2f);
					if (num > (float)Math.PI)
					{
						num = (float)Math.PI * 2f - num;
					}
					if (centre.DistanceTo(flow.Centre[i]) <= flow.SwapPx && num <= flow.SwapTurn && Mathf.Abs(scale.X - flow.Scale[i].X) <= 0.03f * Mathf.Max(0.01f, flow.Scale[i].X))
					{
						Drop(flow, i);
					}
					else if (Mathf.Min(flow.RealFor[i], flow.AfterShow) >= flow.SwapWait)
					{
						flow.Settle[i] = 0.001f;
					}
				}
			}
		}

		internal static void Sideways(bool on, bool quick = false)
		{
			bool flag = on && HeldTurn && !_turnFailed;
			if (flag != _sideways && PowerFxManager.LogTriggers)
			{
				DevLog.Info(flag ? "GambleFlow: the hand turns sideways" : $"GambleFlow: the hand faces us again ({Turnees.Count} card(s) turned)");
			}
			if (!flag && (_sideways || _prepared))
			{
				_facedAt = _rowClock;
			}
			_sideways = flag;
			_prepared = false;
			_quick = quick;
			_backRate = Mathf.Clamp(Tuning.F("gamble.turn_ease", 14f), 2f, 60f) * (quick ? 2.5f : 1f);
			_backStagger = (quick ? 0f : Mathf.Clamp(Tuning.F("gamble.back_stagger", 0.015f), 0f, 0.05f));
		}

		internal static void RowMoves(bool open, float spacing)
		{
			if (open)
			{
				_openedAt = _rowClock;
				return;
			}
			_closedAt = _rowClock;
			_openedAt = -100f;
			_rowTight = spacing;
			Sfx.Play("gamble_shuffle", 0.85f);
		}

		internal static void Prepare(float seconds, float px)
		{
			if (HeldTurn && !_turnFailed && !_sideways)
			{
				_prepared = true;
				_preparedAt = _rowClock;
				_breathTime = Mathf.Max(0.02f, seconds - 0.035f);
				_breathPx = px;
			}
		}

		internal static void Unprepare()
		{
			if (_prepared)
			{
				_prepared = false;
				_quick = false;
				_backStagger = 0f;
				_facedAt = _rowClock;
			}
		}

		internal static void HeldGround(Sprite2D sprite)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)sprite))
			{
				return;
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			int num5 = 0;
			for (int i = 0; i < Turnees.Count; i++)
			{
				Turnee turnee = Turnees[i];
				if (turnee.Taken && FxKit.Alive((GodotObject)(object)turnee.Node))
				{
					CardStandIn.Pose(turnee.Node, out var centre, out var _, out var scale);
					float num6 = centre.Y + 211f * scale.Y;
					if (num5 == 0)
					{
						num = centre.X;
						num2 = centre.X;
						num3 = num6;
					}
					else
					{
						num = Mathf.Min(num, centre.X);
						num2 = Mathf.Max(num2, centre.X);
						num3 = Mathf.Max(num3, num6);
					}
					num4 += Mathf.Clamp(turnee.Turn, 0f, 1f);
					num5++;
				}
			}
			float num7 = ((num5 == 0) ? 0f : (Mathf.Clamp(Tuning.F("gamble.ground", 0.3f), 0f, 1f) * num4 / (float)num5));
			if (num7 <= 0.005f)
			{
				((CanvasItem)sprite).Modulate = new Color(0f, 0f, 0f, 0f);
				return;
			}
			((Node2D)sprite).GlobalPosition = new Vector2((num + num2) * 0.5f, GroundLine(num3));
			((Node2D)sprite).Scale = new Vector2(Mathf.Max(60f, num2 - num + 110f), 36f) / 64f;
			((CanvasItem)sprite).Modulate = new Color(0f, 0f, 0f, num7);
		}

		private static Turnee TurneeOf(NCard node)
		{
			for (int i = 0; i < Turnees.Count; i++)
			{
				if (Turnees[i].Node == node)
				{
					return Turnees[i];
				}
			}
			return null;
		}

		private static void StepTurn(float dt)
		{
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_064b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0352: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			if (!_sideways && !_prepared && Turnees.Count == 0)
			{
				_rowClock = 0f;
				_closedAt = -100f;
				_openedAt = -100f;
				_facedAt = -100f;
				_preparedAt = -100f;
				return;
			}
			try
			{
				_rowClock += dt;
				if (_sideways && (!HandHold.Lined || HandCards.Selecting))
				{
					_sideways = false;
					_facedAt = _rowClock;
				}
				if (_prepared && (_rowClock - _preparedAt > 0.6f || HandCards.Selecting))
				{
					_prepared = false;
					_facedAt = _rowClock;
				}
				for (int i = 0; i < Turnees.Count; i++)
				{
					Turnees[i].Wanted = false;
				}
				if (_sideways || _prepared)
				{
					HandCards.Fill(InHand);
					int count = InHand.Count;
					for (int j = 0; j < count; j++)
					{
						HandCards.Entry entry = InHand[j];
						if (FxKit.Alive((GodotObject)(object)entry.Holder) && (object)((Node)entry.Node).GetParent() == entry.Holder)
						{
							Turnee turnee = TurneeOf(entry.Node);
							if (turnee != null && turnee.Card != entry.Card)
							{
								Face(turnee);
								Turnees.Remove(turnee);
								turnee = null;
							}
							if (turnee == null)
							{
								turnee = new Turnee
								{
									Node = entry.Node,
									Card = entry.Card,
									Home = (CanvasItem)(object)entry.Holder,
									Holder = entry.Holder
								};
								Turnees.Add(turnee);
							}
							turnee.Wanted = true;
							turnee.Index = j;
							turnee.Count = count;
						}
					}
				}
				DeckLook look = DeckLook.Read();
				float unit = ReadUnit(2);
				float rate = Mathf.Clamp(Tuning.F("gamble.turn_snap", 22f), 2f, 60f);
				float num = Mathf.Clamp(Tuning.F("gamble.ripple", 0.012f), 0f, 0.04f) * 1.5f;
				CardModel carried = HeldFxManager.Carried;
				for (int num2 = Turnees.Count - 1; num2 >= 0; num2--)
				{
					Turnee turnee2 = Turnees[num2];
					NCard node = turnee2.Node;
					if (!FxKit.Alive((GodotObject)(object)node) || node.Model != turnee2.Card || (object)((Node)node).GetParent() != turnee2.Home || turnee2.Card == carried)
					{
						Face(turnee2);
						Turnees.RemoveAt(num2);
						continue;
					}
					if (!turnee2.Taken)
					{
						if (!turnee2.Wanted)
						{
							Turnees.RemoveAt(num2);
							continue;
						}
						Vector2 val = ((Control)node).Scale - Vector2.One;
						turnee2.Calm = ((val.LengthSquared() < 1E-06f) ? (turnee2.Calm + 1) : 0);
						if (turnee2.Calm < 2)
						{
							continue;
						}
						turnee2.Taken = true;
						turnee2.TakenAt = _rowClock;
						turnee2.Base = ((Control)node).Scale;
						turnee2.Origin = ((Control)node).Position;
						turnee2.Wrote = ((Control)node).Scale;
						turnee2.Sized = Vector2.One;
						turnee2.Moved = Vector2.Zero;
						turnee2.Light = 1f;
						turnee2.Turn = 0f;
						turnee2.Speed = 0f;
						turnee2.Going = false;
						turnee2.Placed = false;
						turnee2.Moving = false;
						turnee2.Fade = 0f;
						turnee2.Hit = 0f;
						turnee2.HitAt = -100f;
						turnee2.Up = 0f;
					}
					if (turnee2.Wanted)
					{
						turnee2.Fade = 0f;
						int num3 = Mathf.Min(turnee2.Index, turnee2.Count - 1 - turnee2.Index);
						if (_sideways && !turnee2.Going && _rowClock >= _closedAt + (float)num3 * num)
						{
							turnee2.Going = true;
						}
					}
					else if (turnee2.Going && _rowClock >= _facedAt + (float)turnee2.Index * _backStagger)
					{
						turnee2.Going = false;
					}
					if (turnee2.Going)
					{
						Sprung(ref turnee2.Turn, ref turnee2.Speed, 1f, rate, 0.62f, dt);
					}
					else
					{
						Sprung(ref turnee2.Turn, ref turnee2.Speed, 0f, _backRate, 0.66f, dt);
					}
					turnee2.Turn = Mathf.Clamp(turnee2.Turn, -0.2f, 1.2f);
					if (turnee2.Going)
					{
						if (Mathf.Abs(turnee2.Turn - 1f) < 0.003f && Mathf.Abs(turnee2.Speed) < 0.05f)
						{
							turnee2.Turn = 1f;
							turnee2.Speed = 0f;
						}
					}
					else if (!turnee2.Wanted && Mathf.Abs(turnee2.Turn) < 0.004f && Mathf.Abs(turnee2.Speed) < 0.08f && (!turnee2.Moving || _rowClock >= turnee2.MoveAt + turnee2.MoveTime))
					{
						turnee2.Turn = 0f;
						turnee2.Speed = 0f;
						turnee2.Fade += dt / (_quick ? 0.06f : 0.14f);
						if (turnee2.Fade >= 1f || !turnee2.Placed)
						{
							Face(turnee2);
							Turnees.RemoveAt(num2);
							continue;
						}
					}
					Stand(turnee2, in look, unit);
				}
				if (!_placing && Turnees.Count > 0)
				{
					_placing = true;
					Callable val2 = Callable.From((Action)(() =>
					{
						PlaceAll();
					}));
					val2.CallDeferred(Array.Empty<Variant>());
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow.StepTurn: " + ex);
				_turnFailed = true;
				FaceAll();
			}
		}

		private static void Stand(Turnee turnee, in DeckLook look, float unit)
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			NCard node = turnee.Node;
			Turned turned = TurnOf(in look, turnee.Index, turnee.Count, turnee.Turn);
			float num = Pulse((_rowClock - turnee.HitAt) / 0.06f) * Mathf.Clamp(turnee.Turn, 0f, 1f);
			float num2 = 1f + 0.035f * num;
			turnee.Hit = num;
			Vector2 val = new Vector2(turned.Width * turned.Size, turned.Size * num2);
			Vector2 scale = ((Control)node).Scale;
			Vector2 wrote = (((Control)node).Scale = new Vector2(scale.X / turnee.Sized.X * val.X, scale.Y / turnee.Sized.Y * val.Y));
			turnee.Sized = val;
			turnee.Wrote = wrote;
			float num3 = (FxKit.Alive((GodotObject)(object)node.Body) ? Mathf.Abs(node.Body.Scale.Y) : 1f);
			Transform2D globalTransform = turnee.Home.GetGlobalTransform();
			float num4 = 211f * globalTransform.Y.Length() * num3 * turned.Size * (num2 - 1f);
			turnee.Up = turned.Rise * unit + num4;
			Color modulate = ((CanvasItem)node).Modulate;
			((CanvasItem)node).Modulate = new Color(turned.Light, turned.Light, turned.Light, modulate.A);
			turnee.Light = turned.Light;
			StandEdge(turnee, in look, in turned);
		}

		private static void PlaceAll()
		{
			_placing = false;
			try
			{
				for (int i = 0; i < Turnees.Count; i++)
				{
					Place(Turnees[i]);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow.PlaceAll: " + ex);
				_turnFailed = true;
				FaceAll();
			}
		}

		private static void Place(Turnee turnee)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			if (!turnee.Taken)
			{
				return;
			}
			NCard node = turnee.Node;
			NHandCardHolder holder = turnee.Holder;
			if (!FxKit.Alive((GodotObject)(object)node) || !FxKit.Alive((GodotObject)(object)holder) || !((Node)holder).IsInsideTree() || (object)((Node)node).GetParent() != holder || node.Model != turnee.Card)
			{
				return;
			}
			Vector2 position = ((Control)holder).Position;
			Vector2 targetPosition = holder.TargetPosition;
			float num = (float)turnee.Index - (float)(turnee.Count - 1) * 0.5f;
			bool flag = turnee.Placed && turnee.Seen.DistanceSquaredTo(position) > 0.25f;
			Vector2 val;
			if (!turnee.Moving && !_sideways && (_prepared || !flag))
			{
				float num2 = (_prepared ? Smooth(Part(_rowClock, Mathf.Max(turnee.TakenAt, _preparedAt), _breathTime)) : 0f);
				val = position + new Vector2(num * _breathPx * num2, (0f - Mathf.Abs(num)) * _breathPx * 0.15f * num2);
			}
			else
			{
				if (!turnee.Moving || targetPosition.DistanceSquaredTo(turnee.Aim) > 0.25f)
				{
					BeginMove(turnee, position, targetPosition);
				}
				float num3 = Part(_rowClock, turnee.MoveAt, turnee.MoveTime);
				float num4;
				if (turnee.MoveOver > 0.005f)
				{
					num4 = Spring(num3, turnee.MoveOver);
				}
				else
				{
					num4 = (turnee.MoveRush ? Rush(num3) : Smooth(num3));
				}
				val = turnee.From + (turnee.Aim - turnee.From) * num4;
			}
			if (val.DistanceSquaredTo(position) > 4000000f)
			{
				val = position;
				turnee.Moving = false;
			}
			turnee.Seen = val;
			turnee.Placed = true;
			val.X -= num * _rowTight * 0.45f * turnee.Hit;
			if (turnee.Fade > 0f)
			{
				val = val.Lerp(position, Smooth(Mathf.Clamp(turnee.Fade, 0f, 1f)));
			}
			Vector2 val2 = Vector2.Zero;
			Node parent = ((Node)holder).GetParent();
			CanvasItem val3 = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
			Transform2D val4;
			if (val3 != null)
			{
				val4 = val3.GetGlobalTransform();
				val2 = val4.BasisXform(val - position);
			}
			val2.Y -= turnee.Up;
			val4 = ((CanvasItem)holder).GetGlobalTransform();
			val4 = val4.AffineInverse();
			Vector2 val5 = val4.BasisXform(val2);
			((Control)node).Position = ((Control)node).Position + (val5 - turnee.Moved);
			turnee.Moved = val5;
		}

		private static void BeginMove(Turnee turnee, Vector2 held, Vector2 target)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			int num = Mathf.Min(turnee.Index, turnee.Count - 1 - turnee.Index);
			float num2 = Mathf.Clamp(Tuning.F("gamble.ripple", 0.012f), 0f, 0.04f);
			turnee.From = (turnee.Placed ? turnee.Seen : held);
			turnee.Aim = target;
			turnee.Moving = true;
			turnee.MoveOver = 0f;
			turnee.MoveRush = false;
			turnee.HitAt = -100f;
			if (_sideways && _openedAt > _closedAt)
			{
				turnee.MoveAt = _rowClock + (float)num * num2;
				turnee.MoveTime = Mathf.Clamp(Tuning.F("gamble.held_open", 0.3f), 0.05f, 1f);
				turnee.MoveOver = Mathf.Clamp(Tuning.F("gamble.spread_over", 0.12f), 0f, 0.4f);
			}
			else if (_sideways)
			{
				turnee.MoveAt = _rowClock + (float)num * num2 * 1.5f;
				turnee.MoveTime = Mathf.Clamp(Tuning.F("gamble.held_close", 0.2f), 0.05f, 1f);
				turnee.MoveRush = true;
				turnee.HitAt = turnee.MoveAt + turnee.MoveTime - 0.03f;
			}
			else
			{
				turnee.MoveAt = _rowClock + (float)turnee.Index * _backStagger;
				turnee.MoveTime = (_quick ? 0.12f : Mathf.Clamp(Tuning.F("gamble.held_back", 0.28f), 0.05f, 1f));
			}
		}

		private static void StandEdge(Turnee turnee, in DeckLook look, in Turned turned)
		{
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			if (turnee.NoEdge)
			{
				return;
			}
			if (look.Thick <= 0.01f)
			{
				if (turnee.Edge != null)
				{
					DropEdge(turnee);
				}
				return;
			}
			try
			{
				CardDeco.Handle handle = turnee.Edge;
				if (handle == null || !handle.Alive)
				{
					turnee.EdgeTries++;
					handle = (turnee.Edge = ((turnee.EdgeTries > 3) ? null : CardDeco.Attach(turnee.Card, "gamble.edge", BuildEdge, null, EdgeLeft)));
					if (handle == null || !handle.Alive || !(handle.State is EdgeRig))
					{
						turnee.NoEdge = true;
						DropEdge(turnee);
						return;
					}
				}
				EdgeRig edgeRig = handle.State as EdgeRig;
				Node2D root = handle.Root;
				if (edgeRig != null && FxKit.Alive((GodotObject)(object)edgeRig.Root))
				{
					if (!FxKit.Alive((GodotObject)(object)root) || !((Node)root).IsInsideTree() || (object)((Node)root).GetParent() != turnee.Node)
					{
						((CanvasItem)edgeRig.Root).Visible = false;
						return;
					}
					Transform2D globalTransform = ((CanvasItem)root).GetGlobalTransform();
					float num = globalTransform.X.Length();
					float num2 = globalTransform.Y.Length();
					float num3 = Mathf.Clamp(turned.Width, 0f, 1f);
					float num4 = Mathf.Sqrt(Mathf.Max(0f, 1f - num3 * num3));
					float wide = look.Thick * num2 * num4 / Mathf.Max(0.0001f, num);
					SetEdge(edgeRig, new Vector2(-150f, 0f), wide, 1f, 1f, 1f, look.Shadow * num4);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow (side face): " + ex.Message);
				turnee.NoEdge = true;
				DropEdge(turnee);
			}
		}

		private static void BuildEdge(CardDeco.Handle handle)
		{
			EdgeRig edgeRig = MakeEdge();
			((CanvasItem)edgeRig.Root).Visible = false;
			((Node)handle.Root).AddChild((Node)(object)edgeRig.Root, false, (InternalMode)0);
			((CanvasItem)handle.Root).ShowBehindParent = true;
			handle.State = edgeRig;
		}

		private static void EdgeLeft(CardDeco.Handle handle)
		{
			handle.Remove(0f);
		}

		private static void DropEdge(Turnee turnee)
		{
			CardDeco.Handle edge = turnee.Edge;
			turnee.Edge = null;
			try
			{
				if (edge != null)
				{
					edge.Remove(0f);
				}
				else if (turnee.Card != null)
				{
					CardDeco.Remove(turnee.Card, "gamble.edge", 0f);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow (side face off): " + ex.Message);
			}
		}

		private static void Face(Turnee turnee)
		{
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			DropEdge(turnee);
			try
			{
				NCard node = turnee.Node;
				if (!turnee.Taken || !FxKit.Alive((GodotObject)(object)node) || node.Model != turnee.Card)
				{
					return;
				}
				bool flag = (object)((Node)node).GetParent() == turnee.Home;
				Vector2 scale = ((Control)node).Scale;
				Vector2 val;
				if (!flag)
				{
					val = scale - turnee.Wrote;
					if (!(val.LengthSquared() < 1E-06f))
					{
						goto IL_00ba;
					}
				}
				Vector2 val2 = new Vector2(scale.X / turnee.Sized.X, scale.Y / turnee.Sized.Y);
				val = val2 - turnee.Base;
				((Control)node).Scale = ((val.LengthSquared() < 0.0001f) ? turnee.Base : val2);
				goto IL_00ba;
				IL_00ba:
				if (flag)
				{
					Vector2 val3 = ((Control)node).Position - turnee.Moved;
					val = val3 - turnee.Origin;
					((Control)node).Position = ((val.LengthSquared() < 0.01f) ? turnee.Origin : val3);
				}
				Color modulate = ((CanvasItem)node).Modulate;
				if (Mathf.Abs(modulate.R - turnee.Light) + Mathf.Abs(modulate.G - turnee.Light) + Mathf.Abs(modulate.B - turnee.Light) < 0.02f)
				{
					((CanvasItem)node).Modulate = new Color(1f, 1f, 1f, modulate.A);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow (face): " + ex.Message);
			}
			finally
			{
				turnee.Taken = false;
				turnee.Going = false;
				turnee.Sized = Vector2.One;
				turnee.Moved = Vector2.Zero;
				turnee.Light = 1f;
				turnee.Turn = 0f;
				turnee.Speed = 0f;
				turnee.Placed = false;
				turnee.Moving = false;
				turnee.Fade = 0f;
				turnee.Hit = 0f;
				turnee.Up = 0f;
			}
		}

		private static void FaceAll()
		{
			_sideways = false;
			_prepared = false;
			for (int num = Turnees.Count - 1; num >= 0; num--)
			{
				Face(Turnees[num]);
			}
			Turnees.Clear();
			try
			{
				CardDeco.RemoveAll("gamble.edge", 0f);
			}
			catch (Exception ex)
			{
				DevLog.Warn("GambleFlow (side faces off): " + ex.Message);
			}
		}

		private static int HandIndex(NCard node)
		{
			HandCards.Fill(Scratch);
			for (int i = 0; i < Scratch.Count; i++)
			{
				if (Scratch[i].Node == node)
				{
					return i;
				}
			}
			return -1;
		}

		private static bool Holds(List<NCard> list, NCard node)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == node)
				{
					return true;
				}
			}
			return false;
		}

		private static void Conceal(Flow flow, NCard real)
		{
			if (FxKit.Alive((GodotObject)(object)real) && !Holds(flow.Hidden, real))
			{
				flow.Hidden.Add(real);
				CardStandIn.Hide(real);
			}
		}

		private static void Reveal(Flow flow, NCard real)
		{
			if (real == null)
			{
				return;
			}
			for (int num = flow.Hidden.Count - 1; num >= 0; num--)
			{
				if (flow.Hidden[num] == real)
				{
					flow.Hidden.RemoveAt(num);
					CardStandIn.Show(real);
				}
			}
		}

		private static void Drop(Flow flow, int s)
		{
			NCard copy = flow.Copy[s];
			Node2D rig = flow.Rig[s];
			flow.Copy[s] = null;
			flow.Rig[s] = null;
			flow.Edge[s] = null;
			FreeCopy(copy, rig, flow.Predicted[s]);
			NCard val = flow.Real[s];
			if (val != null && !flow.Swapped[s])
			{
				flow.Swapped[s] = true;
				Reveal(flow, val);
			}
		}

		private static void Break(Flow flow, string why)
		{
			if (flow.Broken)
			{
				return;
			}
			flow.Broken = true;
			flow.Waiting = false;
			flow.Shown = true;
			DevLog.Info("GambleFlow: back to the game's own draw (" + why + ")");
			for (int i = 0; i < flow.N; i++)
			{
				Drop(flow, i);
			}
			for (int num = flow.Hidden.Count - 1; num >= 0; num--)
			{
				try
				{
					CardStandIn.Show(flow.Hidden[num]);
				}
				catch (Exception ex)
				{
					DevLog.Warn("GambleFlow.Break (show): " + ex);
				}
			}
			flow.Hidden.Clear();
			try
			{
				if (FxKit.Alive((GodotObject)(object)flow.NewShade))
				{
					((CanvasItem)flow.NewShade).Visible = false;
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("GambleFlow.Break (shade): " + ex2.Message);
			}
		}

		private static void Finish(string why)
		{
			Flow flow = _flow;
			_flow = null;
			if (flow == null)
			{
				return;
			}
			for (int i = 0; i < flow.N; i++)
			{
				try
				{
					NCard copy = flow.Copy[i];
					Node2D rig = flow.Rig[i];
					flow.Copy[i] = null;
					flow.Rig[i] = null;
					flow.Edge[i] = null;
					FreeCopy(copy, rig, flow.Predicted[i]);
					NCard copy2 = flow.OldCopy[i];
					Node2D rig2 = flow.OldRig[i];
					flow.OldCopy[i] = null;
					flow.OldRig[i] = null;
					flow.OldEdge[i] = null;
					FreeCopy(copy2, rig2, flow.OldCard[i]);
				}
				catch (Exception ex)
				{
					DevLog.Warn("GambleFlow.Finish (copy): " + ex);
				}
			}
			for (int num = flow.Hidden.Count - 1; num >= 0; num--)
			{
				try
				{
					CardStandIn.Show(flow.Hidden[num]);
				}
				catch (Exception ex2)
				{
					DevLog.Warn("GambleFlow.Finish (show): " + ex2);
				}
			}
			flow.Hidden.Clear();
			for (int j = 0; j < flow.N; j++)
			{
				try
				{
					NCard val = flow.Gone[j];
					flow.Gone[j] = null;
					if (val != null && (!FxKit.Alive((GodotObject)(object)val) || val.Model == flow.Discards[j]))
					{
						CardStandIn.Show(val);
					}
					if (flow.Claimed[j] && !flow.Flown[j])
					{
						CardFlight.Release(flow.Discards[j]);
					}
				}
				catch (Exception ex3)
				{
					DevLog.Warn("GambleFlow.Finish (old hand): " + ex3);
				}
			}
			try
			{
				if (flow.Held)
				{
					if (PowerFxManager.LogTriggers)
					{
						DevLog.Info($"GambleFlow: hold released with {HandCount()} card(s) in the hand, held for {flow.Hand}");
					}
					HandHold.Release();
				}
			}
			catch (Exception ex4)
			{
				DevLog.Warn("GambleFlow.Finish (hold): " + ex4);
			}
			try
			{
				if (FxKit.Alive((GodotObject)(object)flow.Layer))
				{
					((Node)flow.Layer).QueueFree();
				}
			}
			catch (Exception ex5)
			{
				DevLog.Warn("GambleFlow.Finish (layer): " + ex5);
			}
			if (PowerFxManager.LogTriggers)
			{
				DevLog.Info("GambleFlow: ended (" + why + ")");
			}
		}

		static GambleFlow()
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
