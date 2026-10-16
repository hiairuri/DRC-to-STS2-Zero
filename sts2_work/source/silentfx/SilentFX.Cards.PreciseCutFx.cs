// namespace: SilentFX.Cards
// type: PreciseCutFx (class)
// source lines: 85568-87236
	public sealed class PreciseCutFx : CardFx
	{
		private sealed class Flight
		{
			public bool Remote;

			public Node Room;

			public object Owner;

			public CardModel Card;

			public FinisherCard Rig;

			public ColorRect Edge;

			public NCard Real;

			public Node Layer;

			public Line2D Wake;

			public readonly List<Vector2> Path = new List<Vector2>();

			public int Phase;

			public float T;

			public float Age;

			public Vector2 Tip;

			public Vector2 Aim;

			public float Turn;

			public float Size;

			public float Stretch = 1f;

			public Vector2 Tip0;

			public Vector2 Aim0;

			public float Turn0;

			public float Size0;

			public float Stretch0 = 1f;

			public float Given;

			public CardFxContext C;

			public Vector2 At;

			public Vector2 Cut;

			public Vector2 From;

			public Vector2 To;

			public float Length;

			public float Drawn;

			public bool Choked;

			public float CutSize;

			public float Approach;

			public float Leave = 1.32f;

			public Vector2 Ctrl;

			public Vector2 Ctrl2;

			public Vector2 Pile;

			public float Home;

			public bool GameFlying;
		}

		private sealed class Run
		{
			public Node2D Holder;

			public Node Parent;

			public Vector2 A;

			public Vector2 B;

			public Vector2 Middle;

			public Vector2 Dir;

			public Vector2 From;

			public Vector2 To;

			public float Drawn;

			public bool Choked;

			public float Scale;

			public Line2D Glow;

			public Line2D Core;

			public Line2D SeamGlow;

			public Line2D SeamCore;

			public Line2D SeamDark;

			public Line2D Bloom;

			public Line2D BloomGlow;

			public float BloomPeak;

			public Line2D[] Needles;

			public Vector2[] NeedleAt;

			public Vector2[] NeedleWay;

			public FinisherCard Card;

			public float CardSize;

			public ColorRect Rect;

			public ShaderMaterial Mat;

			public Vector2 View;

			public float HalfLen;

			public float Reach;

			public float Shift;

			public float ShiftAt;

			public float Total;

			public bool Led;

			public bool LineGone;

			public bool Struck;

			public bool Over;
		}

		private static readonly Color Cold = new Color(0.84f, 0.97f, 1f, 1f);

		private static readonly Color Dull = new Color(0.6f, 0.66f, 0.74f, 1f);

		private const int Points = 12;

		private const float InTime = 0.075f;

		private const float HoldTime = 0.03f;

		private const float FadeTime = 0.05f;

		private const float CardAfter = 0.05f;

		private const float Rise = 0.04f;

		private const float Stay = 0.08f;

		private const float Close = 0.2f;

		private const float After = 0.14f;

		private static Curve _taper;

		private static bool _warnedShader;

		private static bool _warnedStep;

		private static bool _warnedCard;

		private static readonly List<Run> Live = new List<Run>();

		private const float HalfW = 150f;

		private const float HalfH = 211f;

		private const float EdgeOn = 1.15f;

		private const float Long = 1.4f;

		private const float ReadyWait = 0.9f;

		private const int Trail = 14;

		private const float TrailStep = 26f;

		private static readonly List<Flight> Flights = new List<Flight>();

		private static bool _watching;

		private static bool _warnedFlight;

		private static Curve _wake;

		public override void OnPlay(CardFxContext c)
		{
			try
			{
				Take(c);
			}
			catch (Exception ex)
			{
				if (!_warnedFlight)
				{
					DevLog.Warn("Precise Cut: the card is not flown, the cut is drawn alone (" + ex.Message + ")");
				}
				_warnedFlight = true;
			}
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			Flight flight = ((c.Card != null) ? FlightOf(c.Card, 0) : null);
			if (flight != null)
			{
				try
				{
					Launch(flight, c, at);
					return;
				}
				catch (Exception ex)
				{
					DevLog.Warn("Precise Cut: the card could not be thrown (" + ex.Message + ")");
					GoHome(flight);
				}
			}
			float num = Mathf.Clamp(PreciseCutChains.Drawn(c.Card), 0f, 1f);
			bool choked = num <= 0.001f;
			float num2 = Mathf.Max(120f, c.TargetWidth);
			float num3 = Mathf.Max(120f, c.TargetHeight);
			float num4 = ((c.TargetSide < 0f) ? (-1f) : 1f);
			float num5 = Mathf.Clamp(0.5f * num3 / num2, 0.2f, 0.75f);
			Vector2 val = new Vector2(num4, num5);
			Vector2 dir = val.Normalized();
			float num6 = Mathf.Clamp(Mathf.Sqrt(num2 * num2 + 0.25f * num3 * num3) * 1.12f, 220f, 1000f);
			Show(c, at, dir, num6 * Share(num, choked), num, choked, led: false);
		}

		private static float Share(float drawn, bool choked)
		{
			return (choked ? 0.3f : Mathf.Lerp(0.55f, 1f, drawn)) * Mathf.Clamp(Tuning.F("precise.cut_length", 1f), 0.3f, 2f);
		}

		private static void Show(CardFxContext c, Vector2 at, Vector2 dir, float length, float drawn, bool choked, bool led)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected Obj, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			Node frontParent = c.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)frontParent))
			{
				return;
			}
			Node2D val = null;
			try
			{
				val = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_PreciseCut")
				};
				frontParent.AddChild((Node)(object)val, false, (InternalMode)0);
				if (!((Node)val).IsInsideTree())
				{
					((Node)val).QueueFree();
				}
				else
				{
					Build(c, val, at, dir, length, drawn, choked, led);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Precise Cut could not be shown: " + ex.Message);
				if (FxKit.Alive((GodotObject)(object)val))
				{
					((Node)val).QueueFree();
				}
			}
		}

		private static void Build(CardFxContext c, Node2D holder, Vector2 at, Vector2 dir, float length, float drawn, bool choked, bool led)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Expected Obj, but got Unknown
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b30: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_061b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0650: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_082b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0836: Unknown result type (might be due to invalid IL or missing references)
			//IL_085b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0866: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b0f: Unknown result type (might be due to invalid IL or missing references)
			//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_09c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09db: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a37: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a6a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa3: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Max(120f, c.TargetHeight);
			Run run = new Run
			{
				Holder = holder,
				Parent = c.FrontParent,
				Dir = dir,
				From = at - dir * (length * 0.5f),
				To = at + dir * (length * 0.5f),
				Drawn = drawn,
				Choked = choked,
				Scale = Strokes.Scale(c),
				View = c.View,
				HalfLen = length * 0.5f,
				Reach = Mathf.Clamp(0.5f * num, 70f, 280f),
				Led = led
			};
			if (run.View.X < 1f || run.View.Y < 1f)
			{
				run.View = new Vector2(1920f, 1080f);
			}
			run.A = holder.ToLocal(run.From);
			run.B = holder.ToLocal(run.To);
			run.Middle = (run.A + run.B) * 0.5f;
			float num2 = 0.155f;
			run.ShiftAt = num2 + Mathf.Clamp(Tuning.F("precise.beat", 0.1f), 0.03f, 0.4f);
			run.Total = (choked ? num2 : (run.ShiftAt + 0.04f + 0.08f + 0.2f + 0.14f));
			run.Shift = (choked ? 0f : (Mathf.Lerp(4f, 11f, drawn) * run.Scale * Mathf.Clamp(Tuning.F("precise.shift", 1f), 0f, 4f)));
			if (!choked && run.Shift > 0.2f)
			{
				run.Mat = MakeShift(c);
			}
			if (run.Mat != null)
			{
				Vector2 val = new Vector2(Mathf.Abs(dir.X) * run.HalfLen + Mathf.Abs(dir.Y) * run.Reach + 8f, Mathf.Abs(dir.Y) * run.HalfLen + Mathf.Abs(dir.X) * run.Reach + 8f);
				ColorRect val2 = FxKit.Quad(val * 2f, (Material)(object)run.Mat);
				((Node)val2).Name = StringName.op_Implicit("SilentFX_CutShift");
				val2.Color = new Color(1f, 1f, 1f, 0f);
				((Control)val2).Position = run.Middle - val;
				((CanvasItem)val2).Visible = false;
				((Node)val2).AddChild((Node)new BackBufferCopy
				{
					CopyMode = (CopyModeEnum)2,
					ShowBehindParent = true
				}, false, (InternalMode)0);
				((Node)holder).AddChild((Node)(object)val2, false, (InternalMode)0);
				run.Rect = val2;
				Aim(run, 0f);
			}
			if (!c.Remote && !led && !choked && c.Card != null && Tuning.F("precise.card", 1f) >= 0.5f)
			{
				try
				{
					FinisherCard.ReadTuning();
					run.Card = FinisherCard.Make(c.Card, (Node)(object)holder, (Node)(object)holder);
					run.CardSize = Mathf.Clamp(0.42f * run.Scale * Mathf.Clamp(Tuning.F("precise.card_size", 1f), 0.4f, 2.5f), 0.16f, 0.9f);
				}
				catch (Exception ex)
				{
					if (!_warnedCard)
					{
						DevLog.Warn("Precise Cut: no card to cut with, the line is drawn alone (" + ex.Message + ")");
					}
					_warnedCard = true;
					run.Card = null;
				}
			}
			Live.Add(run);
			Color val3 = (choked ? Dull : Cold);
			float num3 = (choked ? 0.45f : Mathf.Lerp(0.55f, 1f, drawn));
			float num4 = Mathf.Sqrt(run.Scale);
			run.Glow = Hair((choked ? 6f : Mathf.Lerp(7f, 10f, drawn)) * num4, new Color(val3.R, val3.G, val3.B, 0.34f * num3));
			run.Core = Hair((choked ? 1.8f : Mathf.Lerp(2.2f, 3f, drawn)) * num4, new Color(1f, 1f, 1f, num3));
			((Node)holder).AddChild((Node)(object)run.Glow, false, (InternalMode)0);
			((Node)holder).AddChild((Node)(object)run.Core, false, (InternalMode)0);
			if (!choked)
			{
				if (run.Mat == null)
				{
					run.SeamDark = Hair(2.2f * num4, new Color(0.02f, 0.03f, 0.05f, 0.55f), additive: false);
					((CanvasItem)run.SeamDark).Visible = false;
					((Node)holder).AddChild((Node)(object)run.SeamDark, false, (InternalMode)0);
				}
				run.SeamGlow = Hair(7f * num4, new Color(Cold.R, Cold.G, Cold.B, 0.26f * num3));
				run.SeamCore = Hair(1.8f * num4, new Color(1f, 1f, 1f, 0.95f * num3));
				((CanvasItem)run.SeamGlow).Visible = false;
				((CanvasItem)run.SeamCore).Visible = false;
				((Node)holder).AddChild((Node)(object)run.SeamGlow, false, (InternalMode)0);
				((Node)holder).AddChild((Node)(object)run.SeamCore, false, (InternalMode)0);
				Span(run.SeamGlow, run.A, run.B, 0f, 1f);
				Span(run.SeamCore, run.A, run.B, 0f, 1f);
				run.BloomPeak = Mathf.Lerp(10f, 20f, drawn) * num4 * Mathf.Clamp(Tuning.F("precise.bloom", 1f), 0f, 3f);
				if (run.BloomPeak > 0.5f)
				{
					run.BloomGlow = Hair(1f, new Color(Cold.R, Cold.G, Cold.B, 0.1f * num3));
					run.Bloom = Hair(1f, new Color(1f, 1f, 1f, 0.96f * num3));
					((CanvasItem)run.BloomGlow).Visible = false;
					((CanvasItem)run.Bloom).Visible = false;
					((Node)holder).AddChild((Node)(object)run.BloomGlow, false, (InternalMode)0);
					((Node)holder).AddChild((Node)(object)run.Bloom, false, (InternalMode)0);
					Span(run.BloomGlow, run.A, run.B, 0f, 1f);
					Span(run.Bloom, run.A, run.B, 0f, 1f);
					int num5 = ((!(Tuning.F("precise.needles", 0f) < 0.5f)) ? (3 + Mathf.RoundToInt(4f * drawn)) : 0);
					Vector2 val4 = run.A.DirectionTo(run.B);
					Vector2 val5 = new Vector2(0f - val4.Y, val4.X);
					float num6 = run.A.DistanceTo(run.B) / Mathf.Max(1f, length) * run.Scale * Mathf.Lerp(0.6f, 1f, drawn);
					run.Needles = new Line2D[num5 * 2];
					run.NeedleAt = new Vector2[num5 * 2];
					run.NeedleWay = new Vector2[num5 * 2];
					for (int i = 0; i < num5 * 2; i++)
					{
						float num7 = ((float)(i / 2) + 0.5f + 0.32f * (float)GD.RandRange(-1.0, 1.0)) / (float)num5;
						float num8 = ((i % 2 == 0) ? 1f : (-1f));
						run.NeedleAt[i] = run.A.Lerp(run.B, Mathf.Clamp(num7, 0.08f, 0.92f));
						Vector2[] needleWay = run.NeedleWay;
						int num9 = i;
						Vector2 val6 = val5 * num8;
						needleWay[num9] = val6.Rotated((float)GD.RandRange(-0.1, 0.1)) * ((float)GD.RandRange(30.0, 78.0) * num6);
						Line2D val7 = Props.Line(Mathf.Max(1.2f, 1.8f * num4), new Color(1f, 1f, 1f, 0.95f * num3), true, run.NeedleAt[i], run.NeedleAt[i]);
						((CanvasItem)val7).Visible = false;
						((Node)holder).AddChild((Node)(object)val7, false, (InternalMode)0);
						run.Needles[i] = val7;
					}
				}
				if (run.SeamDark != null)
				{
					Span(run.SeamDark, run.A, run.B, 0f, 1f);
				}
			}
			else
			{
				Brush.Sparks(run.Parent, at, new Color(Dull.R, Dull.G, Dull.B, 0.8f), 3, 170f);
			}
			Step(run, 0f);
			FxKit.Drive((Node)(object)holder, run.Total, (float u) =>
			{
				Step(run, u * run.Total);
			}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				Finish(run);
			})));
		}

		private static Line2D Hair(float width, Color color, bool additive = true)
		{
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Expected Obj, but got Unknown
			if (_taper == null)
			{
				Curve val = new Curve();
				val.AddPoint(new Vector2(0f, 0f), 0f, 0f, (TangentMode)0, (TangentMode)0);
				val.AddPoint(new Vector2(0.14f, 1f), 0f, 0f, (TangentMode)0, (TangentMode)0);
				val.AddPoint(new Vector2(0.86f, 1f), 0f, 0f, (TangentMode)0, (TangentMode)0);
				val.AddPoint(new Vector2(1f, 0f), 0f, 0f, (TangentMode)0, (TangentMode)0);
				_taper = val;
			}
			Line2D val2 = Props.Line(Mathf.Max(1f, width), color, additive);
			val2.WidthCurve = _taper;
			return val2;
		}

		private static void Span(Line2D line, Vector2 a, Vector2 b, float from, float to)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)line))
			{
				Vector2[] array = new Vector2[13];
				for (int i = 0; i <= 12; i++)
				{
					array[i] = a.Lerp(b, Mathf.Lerp(from, to, (float)i / 12f));
				}
				line.Points = array;
			}
		}

		private static ShaderMaterial MakeShift(CardFxContext c)
		{
			try
			{
				if (Tuning.F("precise.shift_on", 1f) < 0.5f)
				{
					return null;
				}
				if (!File.Exists(Path.Combine(DevLog.ModDir, "shaders", "cut_shift.gdshader")))
				{
					if (!_warnedShader)
					{
						DevLog.Warn("Precise Cut: shaders/cut_shift.gdshader is missing - the cut is drawn without the slide");
					}
					_warnedShader = true;
					return null;
				}
				CardModel card = c.Card;
				object obj;
				if (card == null)
				{
					obj = null;
				}
				else
				{
					ICombatState combatState = card.CombatState;
					obj = ((combatState != null) ? combatState.Encounter : null);
				}
				EncounterModel val = (EncounterModel)obj;
				if (val != null && Tuning.F("precise.shift_boss", 0f) < 0.5f)
				{
					PropertyInfo property = typeof(EncounterModel).GetProperty("HasCustomBackground", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					bool flag = default;
					int num;
					if (property != null)
					{
						object value = property.GetValue(val);
						if (value is bool)
						{
							flag = (bool)value;
							num = 1;
						}
						else
						{
							num = 0;
						}
					}
					else
					{
						num = 0;
					}
					if (((uint)num & (flag ? 1u : 0u)) != 0)
					{
						return null;
					}
				}
				return ShaderLib.NewMaterial("cut_shift");
			}
			catch (Exception ex)
			{
				if (!_warnedShader)
				{
					DevLog.Warn("Precise Cut: the slide could not be made, the cut is drawn without it: " + ex.Message);
				}
				_warnedShader = true;
				return null;
			}
		}

		private static void Aim(Run run, float pixels)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)run.Mat) && FxKit.Alive((GodotObject)(object)run.Holder) && ((Node)run.Holder).IsInsideTree())
			{
				Transform2D globalTransformWithCanvas = ((CanvasItem)run.Holder).GetGlobalTransformWithCanvas();
				Vector2 val = globalTransformWithCanvas.BasisXform(run.A.DirectionTo(run.B));
				float num = val.Length();
				if (!(num > 0.01f) || !float.IsFinite(num))
				{
					val = run.Dir;
					num = 1f;
				}
				run.Mat.SetShaderParameter(StringName.op_Implicit("view"), Variant.op_Implicit(run.View));
				run.Mat.SetShaderParameter(StringName.op_Implicit("point"), Variant.op_Implicit(globalTransformWithCanvas * run.Middle));
				run.Mat.SetShaderParameter(StringName.op_Implicit("dir"), Variant.op_Implicit(val / num));
				run.Mat.SetShaderParameter(StringName.op_Implicit("half_len"), Variant.op_Implicit(run.HalfLen * num));
				run.Mat.SetShaderParameter(StringName.op_Implicit("reach"), Variant.op_Implicit(run.Reach * num));
				run.Mat.SetShaderParameter(StringName.op_Implicit("shift"), Variant.op_Implicit(pixels * num));
			}
		}

		private static void Step(Run run, float t)
		{
			if (run.Over || !FxKit.Alive((GodotObject)(object)run.Holder))
			{
				return;
			}
			try
			{
				Advance(run, t);
			}
			catch (Exception ex)
			{
				if (!_warnedStep)
				{
					DevLog.Warn("Precise Cut stopped half-way: " + ex.Message);
				}
				_warnedStep = true;
				Finish(run);
			}
		}

		private static float CardAlong(float t)
		{
			return Mathf.Lerp(-0.24f, 1.32f, Mathf.Clamp(t / 0.125f, 0f, 1f));
		}

		private static void FreeCard(Run run)
		{
			FinisherCard card = run.Card;
			run.Card = null;
			if (card == null)
			{
				return;
			}
			try
			{
				card.Free();
			}
			catch (Exception ex)
			{
				DevLog.Warn("Precise Cut: the card could not be given back (" + ex.Message + ")");
			}
		}

		public static void Reset()
		{
			foreach (Run item in Live)
			{
				FreeCard(item);
			}
			Live.Clear();
			foreach (Flight flight in Flights)
			{
				End(flight);
			}
			Flights.Clear();
		}

		private static Flight FlightOf(CardModel card, int phase = -1)
		{
			foreach (Flight flight in Flights)
			{
				if (flight.Card == card && flight.Phase < 4 && (phase < 0 || flight.Phase == phase))
				{
					return flight;
				}
			}
			return null;
		}

		private static void Take(CardFxContext c)
		{
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Expected Obj, but got Unknown
			CardModel card = c?.Card;
			if (card == null || TimeFreeze.Active || Tuning.F("precise.fly", 1f) < 0.5f || FlightOf(card) != null)
			{
				return;
			}
			bool remote = c.Remote;
			if (remote && PreciseCutChains.Drawn(card) <= 0.001f)
			{
				return;
			}
			Node val = (remote ? c.FrontParent : GameRefs.UiLayer);
			if (!FxKit.Alive((GodotObject)(object)val) || !val.IsInsideTree())
			{
				return;
			}
			NCard val2 = null;
			Vector2 centre;
			float rotation;
			float num;
			if (remote)
			{
				centre = c.WeaponHand;
				rotation = 0f;
				num = Mathf.Clamp(0.42f * Strokes.Scale(c) * Mathf.Clamp(Tuning.F("precise.card_size", 1f), 0.4f, 2.5f), 0.16f, 0.9f);
			}
			else
			{
				val2 = HandCards.NodeOf(card);
				if (!FxKit.Alive((GodotObject)(object)val2) || !((Node)val2).IsInsideTree())
				{
					return;
				}
				CardStandIn.Pose(val2, out centre, out rotation, out var scale);
				num = Mathf.Clamp(scale.Y, 0.2f, 2f);
			}
			FinisherCard.ReadTuning();
			FinisherCard finisherCard = FinisherCard.Make(card, val, null);
			if (finisherCard == null)
			{
				return;
			}
			Vector2 val3 = new Vector2(150f, -211f);
			Vector2 val4 = val3.Rotated(rotation);
			Flight flight = new Flight
			{
				Remote = remote,
				Room = (Node)(object)(remote ? GameRefs.CombatRoom : null),
				Owner = (remote ? card.Owner : null),
				Card = card,
				C = c,
				Rig = finisherCard,
				Real = val2,
				Layer = val,
				Given = num,
				Size = num,
				Size0 = num,
				Aim = val4.Normalized(),
				Tip = centre + val4 * num
			};
			flight.Aim0 = flight.Aim;
			flight.Tip0 = flight.Tip;
			try
			{
				if (FxKit.Alive((GodotObject)(object)finisherCard.Rig))
				{
					((CanvasItem)finisherCard.Rig).ZIndex = 8;
					flight.Edge = PreciseCutChains.EdgeCopy(card);
					if (flight.Edge != null)
					{
						((Node)finisherCard.Rig).AddChild((Node)(object)flight.Edge, false, (InternalMode)0);
					}
				}
				if (_wake == null)
				{
					Curve val5 = new Curve();
					val5.AddPoint(new Vector2(0f, 0f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					val5.AddPoint(new Vector2(1f, 1f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					_wake = val5;
				}
				flight.Wake = Props.Line(6f, new Color(Cold.R, Cold.G, Cold.B, 0.75f), true);
				flight.Wake.WidthCurve = _wake;
				((CanvasItem)flight.Wake).ZIndex = 7;
				((CanvasItem)flight.Wake).Visible = false;
				val.AddChild((Node)(object)flight.Wake, false, (InternalMode)0);
			}
			catch (Exception ex)
			{
				DevLog.Warn("Precise Cut: the card flies plain (" + ex.Message + ")");
			}
			if (!remote)
			{
				CardStandIn.Hide(val2);
				CardFlight.Claim(card, new CardFlight.Plan
				{
					HideCard = true,
					HideTrail = true,
					Started = (NCard _, Vector2 _, Vector2 _) =>
					{
						Flight flight2 = FlightOf(card);
						if (flight2 == null)
						{
							foreach (Flight flight3 in Flights)
							{
								if (flight3.Card == card)
								{
									flight2 = flight3;
								}
							}
						}
						if (flight2 != null)
						{
							flight2.GameFlying = true;
						}
					}
				}, 8f);
			}
			Flights.Add(flight);
			if (!_watching)
			{
				_watching = true;
				Ticker.Add(Update);
			}
			Place(flight, remote ? 0f : 1f);
		}

		private static void Update(double realDt)
		{
			if (Flights.Count == 0)
			{
				return;
			}
			float dt = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			for (int num = Flights.Count - 1; num >= 0; num--)
			{
				Flight flight = Flights[num];
				bool flag = false;
				try
				{
					flag = Fly(flight, dt);
				}
				catch (Exception ex)
				{
					if (!_warnedFlight)
					{
						DevLog.Warn("Precise Cut: the card's flight stopped (" + ex.Message + ")");
					}
					_warnedFlight = true;
				}
				if (!flag)
				{
					End(flight);
					Flights.Remove(flight);
				}
			}
		}

		private static bool Fly(Flight f, float dt)
		{
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_047f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0486: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0586: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
			if (f.Remote)
			{
				if (MultiplayerFx.Active && (object)f.Room == GameRefs.CombatRoom)
				{
					object owner = f.Owner;
					CardModel card = f.Card;
					if (owner == ((card != null) ? card.Owner : null) && !MultiplayerFx.IsLocal(f.Card) && FxKit.Alive((GodotObject)(object)f.Layer) && f.Layer.IsInsideTree())
					{
						goto IL_005f;
					}
				}
				return false;
			}
			goto IL_005f;
			IL_005f:
			f.Age += dt;
			f.T += dt;
			bool flag = FxKit.Alive((GodotObject)(object)f.Real) && f.Real.Model == f.Card;
			if (flag && !f.GameFlying)
			{
				Color modulate = ((CanvasItem)f.Real).Modulate;
				if (modulate.A > 0f)
				{
					((CanvasItem)f.Real).Modulate = new Color(modulate.R, modulate.G, modulate.B, 0f);
				}
			}
			if (f.Phase == 4)
			{
				if (flag && !f.GameFlying)
				{
					return f.Age < 8f;
				}
				return false;
			}
			if (f.Rig == null || !f.Rig.Alive)
			{
				Drop(f);
				return true;
			}
			if (f.Remote && f.Phase == 3)
			{
				Place(f, Mathf.Clamp(1f - f.T / 0.05f, 0f, 1f));
				return f.T < 0.05f;
			}
			switch (f.Phase)
			{
			case 0:
			{
				float num4 = 1f - Mathf.Exp((0f - f.T) / 0.06f);
				f.Tip = f.Tip0 - f.Aim * (26f * f.Given * num4);
				f.Turn = 0.3f * num4;
				f.Size = f.Given * Mathf.Lerp(1f, 0.94f, num4);
				Place(f, 1f);
				if (f.GameFlying || f.T > 0.9f)
				{
					GoHome(f);
				}
				break;
			}
			case 1:
			{
				float num3 = ((f.Approach > 0.001f) ? Mathf.Clamp(f.T / f.Approach, 0f, 1f) : 1f);
				f.Tip = f.Tip0.Lerp(f.From.Lerp(f.To, CardAlong(0f)), num3);
				f.Aim = Vector2.FromAngle(Mathf.LerpAngle(f.Aim0.Angle(), f.Cut.Angle(), Mathf.Min(1f, num3 * 2f)));
				f.Turn = Mathf.Lerp(f.Turn0, 1.15f, num3);
				f.Size = Mathf.Lerp(f.Size0, f.CutSize, num3);
				f.Stretch = Mathf.Lerp(f.Stretch0, 1.4f, num3);
				Place(f, 1f);
				if (num3 >= 1f)
				{
					f.Phase = 2;
					f.T = Mathf.Max(0f, f.T - f.Approach);
					Show(f.C, f.At, f.Cut, f.Length, f.Drawn, f.Choked, led: true);
				}
				break;
			}
			case 2:
			{
				float num5 = CardAlong(f.T);
				f.Tip = f.From.Lerp(f.To, Mathf.Min(num5, f.Leave));
				f.Aim = f.Cut;
				f.Turn = 1.15f;
				f.Size = f.CutSize;
				f.Stretch = 1.4f;
				Place(f, 1f);
				if (num5 >= f.Leave || f.T >= 0.125f)
				{
					GoHome(f);
				}
				break;
			}
			default:
			{
				float num = Mathf.Clamp(f.T / Mathf.Max(0.05f, f.Home), 0f, 1f);
				float num2 = Mathf.Lerp(num, num * num * (3f - 2f * num), 0.5f);
				Vector2 val = f.Tip0.Lerp(f.Ctrl, num2);
				Vector2 val2 = f.Ctrl.Lerp(f.Ctrl2, num2);
				Vector2 val3 = f.Ctrl2.Lerp(f.Pile, num2);
				Vector2 val4 = val.Lerp(val2, num2);
				Vector2 val5 = val2.Lerp(val3, num2);
				f.Tip = val4.Lerp(val5, num2);
				Vector2 val6 = val5 - val4;
				if (val6.LengthSquared() > 1f)
				{
					f.Aim = val6.Normalized();
				}
				f.Turn = Mathf.Lerp(f.Turn0, 1.15f, Mathf.Min(1f, num * 4f));
				f.Size = Mathf.Lerp(f.Size0, Mathf.Min(f.Size0, 0.3f) * 0.6f, Mathf.Sqrt(num));
				f.Stretch = Mathf.Lerp(f.Stretch0, 1.1f, num);
				Place(f, (num < 0.86f) ? 1f : (1f - (num - 0.86f) / 0.14f));
				if (num >= 1f)
				{
					FxKit.Flash(f.Layer, f.Pile, new Color(Cold.R, Cold.G, Cold.B, 0.55f), 54f, 1.5f, 0.16);
					Drop(f);
				}
				break;
			}
			}
			return true;
		}

		private static void Place(Flight f, float alpha)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			if (f.Rig == null || !f.Rig.Alive)
			{
				return;
			}
			if (f.Remote)
			{
				alpha *= Mathf.Clamp(f.Age / 0.05f, 0f, 1f);
			}
			Vector2 val = FinisherCard.PointOffset(f.Turn, 1f, f.Stretch);
			float num = f.Aim.Angle() - val.Angle();
			f.Rig.Put(f.Tip - val.Rotated(num) * f.Size, num, f.Size, f.Turn, 1f, f.Stretch, 1f, alpha, 0f, -1f);
			if (FxKit.Alive((GodotObject)(object)f.Edge))
			{
				((CanvasItem)f.Edge).Modulate = new Color(1f, 1f, 1f, alpha);
			}
			if (!FxKit.Alive((GodotObject)(object)f.Wake) || f.Phase == 0)
			{
				return;
			}
			if (f.Path.Count != 0)
			{
				Vector2 val2 = f.Path[f.Path.Count - 1];
				if (!(val2.DistanceTo(f.Tip) >= 26f))
				{
					goto IL_0155;
				}
			}
			f.Path.Add(f.Tip);
			goto IL_0155;
			IL_0155:
			while (f.Path.Count > 14)
			{
				f.Path.RemoveAt(0);
			}
			Vector2[] array = new Vector2[f.Path.Count + 1];
			for (int i = 0; i < f.Path.Count; i++)
			{
				array[i] = ((Node2D)f.Wake).ToLocal(f.Path[i]);
			}
			array[f.Path.Count] = ((Node2D)f.Wake).ToLocal(f.Tip);
			f.Wake.Points = array;
			((CanvasItem)f.Wake).Visible = true;
			((CanvasItem)f.Wake).Modulate = new Color(1f, 1f, 1f, alpha);
		}

		private static void Launch(Flight f, CardFxContext c, Vector2 at)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Clamp(PreciseCutChains.Drawn(c.Card), 0f, 1f);
			bool flag = num <= 0.001f;
			if (f.Remote & flag)
			{
				Drop(f);
				new PreciseCutFx().OnHit(c, at, killed: false);
				return;
			}
			Vector2 val = at - f.Tip;
			Vector2 val2 = ((val.LengthSquared() > 100f) ? val.Normalized() : f.Aim);
			float num2 = Mathf.Max(120f, c.TargetWidth);
			float num3 = Mathf.Max(120f, c.TargetHeight);
			float num4 = Mathf.Clamp(Mathf.Sqrt(num2 * num2 * val2.X * val2.X + num3 * num3 * val2.Y * val2.Y) * 1.12f + 60f, 220f, 1000f);
			f.C = c;
			f.At = at;
			f.Drawn = num;
			f.Choked = flag;
			f.Cut = val2;
			f.Length = num4 * Share(num, flag);
			f.From = at - val2 * (f.Length * 0.5f);
			f.To = at + val2 * (f.Length * 0.5f);
			f.CutSize = Mathf.Clamp(0.42f * Strokes.Scale(c) * Mathf.Clamp(Tuning.F("precise.card_size", 1f), 0.4f, 2.5f), 0.16f, 0.9f);
			float num5 = (CardAlong(99f) - CardAlong(0f)) * f.Length / 0.125f;
			Vector2 val3 = f.From.Lerp(f.To, CardAlong(0f)) - f.Tip;
			float num6 = val3.Dot(val2);
			f.Approach = ((num6 > 1f) ? Mathf.Clamp(num6 / Mathf.Max(1f, num5), 0.02f, 0.09f) : 0f);
			f.Leave = Mathf.Clamp(Within(f.From, val2) / Mathf.Max(1f, f.Length), 0.8f, CardAlong(99f));
			Begin(f, 1);
		}

		private static void Begin(Flight f, int phase)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			f.Phase = phase;
			f.T = 0f;
			f.Tip0 = f.Tip;
			f.Aim0 = f.Aim;
			f.Turn0 = f.Turn;
			f.Size0 = f.Size;
			f.Stretch0 = f.Stretch;
		}

		private static void GoHome(Flight f)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			Begin(f, 3);
			if (!f.Remote)
			{
				f.Pile = GameRefs.PilePoint((PileType)3);
				float num = f.Tip0.DistanceTo(f.Pile);
				f.Ctrl = Inside(f.Tip0 + f.Aim * Mathf.Clamp(num * 0.35f, 100f, 420f), 0.11f, 0.07f);
				f.Ctrl2 = Inside(f.Pile + new Vector2(-0.74f, -0.67f) * Mathf.Clamp(num * 0.45f, 240f, 520f), 0.02f, 0.03f);
				f.Home = Mathf.Clamp(num / 2800f, 0.32f, 0.52f) * Mathf.Clamp(Tuning.F("precise.home", 1f), 0.3f, 3f);
			}
		}

		private static Vector2 Inside(Vector2 point, float side, float top)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = FxKit.ViewSize();
			float num = val.X * side;
			float num2 = val.Y * top;
			return new Vector2(Mathf.Clamp(point.X, num, Mathf.Max(num, val.X - num)), Mathf.Clamp(point.Y, num2, Mathf.Max(num2, val.Y - num2)));
		}

		private static float Within(Vector2 from, Vector2 dir)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = FxKit.ViewSize();
			float num;
			if (dir.X > 0.001f)
			{
				num = (val.X - 30f - from.X) / dir.X;
			}
			else
			{
				num = ((dir.X < -0.001f) ? ((30f - from.X) / dir.X) : float.MaxValue);
			}
			float num2;
			if (dir.Y > 0.001f)
			{
				num2 = (val.Y - 30f - from.Y) / dir.Y;
			}
			else
			{
				num2 = ((dir.Y < -0.001f) ? ((30f - from.Y) / dir.Y) : float.MaxValue);
			}
			return Mathf.Max(0f, Mathf.Min(num, num2));
		}

		private static void Drop(Flight f)
		{
			FinisherCard rig = f.Rig;
			f.Rig = null;
			f.Edge = null;
			try
			{
				rig?.Free();
			}
			catch (Exception ex)
			{
				DevLog.Warn("Precise Cut: the card could not be given back (" + ex.Message + ")");
			}
			if (FxKit.Alive((GodotObject)(object)f.Wake))
			{
				((Node)f.Wake).QueueFree();
			}
			f.Wake = null;
			f.Phase = 4;
			f.T = 0f;
		}

		private static void End(Flight f)
		{
			Drop(f);
			if (f.Remote || f.GameFlying)
			{
				return;
			}
			try
			{
				CardFlight.Release(f.Card);
				if (FxKit.Alive((GodotObject)(object)f.Real) && f.Real.Model == f.Card)
				{
					CardStandIn.Show(f.Real);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Precise Cut: the card could not be shown again (" + ex.Message + ")");
			}
		}

		private static void Advance(Run run, float t)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05db: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_066c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_0680: Unknown result type (might be due to invalid IL or missing references)
			//IL_0685: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0699: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_086e: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0803: Unknown result type (might be due to invalid IL or missing references)
			//IL_080c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0811: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0886: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
			if (run.Card != null)
			{
				float num = t / 0.125f;
				if (num >= 1f || !run.Card.Alive)
				{
					FreeCard(run);
				}
				else
				{
					Vector2 at = run.From.Lerp(run.To, CardAlong(t));
					float alpha;
					if (num < 0.1f)
					{
						alpha = num / 0.1f;
					}
					else
					{
						alpha = ((num > 0.7f) ? ((1f - num) / 0.3f) : 1f);
					}
					run.Card.Put(at, run.Dir.Angle() + (float)Math.PI / 2f, run.CardSize, 1.15f, 1f, 1.4f, 1f, alpha, 1.3f, -1f);
				}
			}
			float num2 = 0.155f;
			if (t < num2)
			{
				float num3 = Mathf.Clamp(t / 0.075f, 0f, 1f);
				float num4 = Mathf.Clamp((t - 0.075f - 0.03f) / 0.05f, 0f, 1f);
				float to = ((run.Card != null || run.Led) ? Mathf.Clamp(CardAlong(t) + 0.04f, 0f, 1f) : (0.12f + 0.88f * (1f - (1f - num3) * (1f - num3))));
				float num5 = 0.45f * num4 * num4;
				Span(run.Glow, run.A, run.B, num5, to);
				Span(run.Core, run.A, run.B, num5, to);
				Color modulate = new Color(1f, 1f, 1f, 1f - num4);
				if (FxKit.Alive((GodotObject)(object)run.Glow))
				{
					((CanvasItem)run.Glow).Modulate = modulate;
				}
				if (FxKit.Alive((GodotObject)(object)run.Core))
				{
					((CanvasItem)run.Core).Modulate = modulate;
				}
			}
			else if (!run.LineGone)
			{
				run.LineGone = true;
				if (FxKit.Alive((GodotObject)(object)run.Glow))
				{
					((CanvasItem)run.Glow).Visible = false;
				}
				if (FxKit.Alive((GodotObject)(object)run.Core))
				{
					((CanvasItem)run.Core).Visible = false;
				}
			}
			float num6 = t - run.ShiftAt;
			if (run.Choked || num6 < 0f)
			{
				return;
			}
			if (!run.Struck)
			{
				run.Struck = true;
				if (FxKit.Alive((GodotObject)(object)run.SeamGlow))
				{
					((CanvasItem)run.SeamGlow).Visible = true;
				}
				if (FxKit.Alive((GodotObject)(object)run.SeamCore))
				{
					((CanvasItem)run.SeamCore).Visible = true;
				}
				if (FxKit.Alive((GodotObject)(object)run.SeamDark))
				{
					((CanvasItem)run.SeamDark).Visible = true;
				}
				if (FxKit.Alive((GodotObject)(object)run.Rect))
				{
					((CanvasItem)run.Rect).Visible = true;
				}
				if (FxKit.Alive((GodotObject)(object)run.BloomGlow))
				{
					((CanvasItem)run.BloomGlow).Visible = true;
				}
				if (FxKit.Alive((GodotObject)(object)run.Bloom))
				{
					((CanvasItem)run.Bloom).Visible = true;
				}
				float scale = run.Scale;
				if (run.Needles != null)
				{
					Line2D[] needles = run.Needles;
					foreach (Line2D val in needles)
					{
						if (FxKit.Alive((GodotObject)(object)val))
						{
							((CanvasItem)val).Visible = true;
						}
					}
				}
				FxKit.Flash(run.Parent, (run.From + run.To) * 0.5f, new Color(Cold.R, Cold.G, Cold.B, 0.5f), 64f * scale * Mathf.Lerp(0.6f, 1f, run.Drawn), 1.5f, 0.16);
				Brush.Sparks(run.Parent, run.To, Cold, 2 + Mathf.RoundToInt(4f * run.Drawn), 260f * scale, run.Dir, 24f);
				Brush.Sparks(run.Parent, run.From, Cold, 1 + Mathf.RoundToInt(2f * run.Drawn), 180f * scale, -run.Dir, 24f);
				if (run.Drawn >= 0.999f)
				{
					FxKit.Flash(run.Parent, run.To, new Color(1f, 0.97f, 0.9f, 0.7f), 46f * scale, 1.6f, 0.14);
				}
			}
			float num7;
			if (!(num6 < 0.04f))
			{
				num7 = ((!(num6 < 0.12f)) ? (1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp((num6 - 0.04f - 0.08f) / 0.2f, 0f, 1f))) : 1f);
			}
			else
			{
				float num8 = 1f - num6 / 0.04f;
				num7 = 1f - num8 * num8 * num8;
			}
			float num9 = run.Shift * num7;
			if (FxKit.Alive((GodotObject)(object)run.Bloom) && FxKit.Alive((GodotObject)(object)run.BloomGlow))
			{
				float num10 = Mathf.Clamp((num6 - 0.04f - 0.08f) / 0.2f, 0f, 1f);
				float num11 = ((num6 < 0.04f) ? num7 : ((1f - num10) * (1f - num10)));
				run.Bloom.Width = Mathf.Max(0.6f, run.BloomPeak * num11);
				run.BloomGlow.Width = Mathf.Max(1f, run.BloomPeak * 1.9f * num11);
				Color modulate2 = new Color(1f, 1f, 1f, Mathf.Clamp(num11 * 1.5f, 0f, 1f));
				((CanvasItem)run.Bloom).Modulate = modulate2;
				((CanvasItem)run.BloomGlow).Modulate = modulate2;
			}
			if (run.Needles != null)
			{
				float num12 = Mathf.Clamp(num6 / 0.22f, 0f, 1f);
				float num13 = 1f - (1f - num12) * (1f - num12) * (1f - num12);
				float num14 = num13 * num12;
				for (int j = 0; j < run.Needles.Length; j++)
				{
					Line2D val2 = run.Needles[j];
					if (FxKit.Alive((GodotObject)(object)val2))
					{
						val2.Points = new Vector2[2]
						{
							run.NeedleAt[j] + run.NeedleWay[j] * num14,
							run.NeedleAt[j] + run.NeedleWay[j] * num13
						};
						((CanvasItem)val2).Modulate = new Color(1f, 1f, 1f, 1f - num12 * num12);
						if (num12 >= 1f)
						{
							((CanvasItem)val2).Visible = false;
						}
					}
				}
			}
			if (run.Rect != null)
			{
				if (!FxKit.Alive((GodotObject)(object)run.Rect) || (num7 <= 0.001f && num6 > 0.04f))
				{
					if (FxKit.Alive((GodotObject)(object)run.Rect))
					{
						((Node)run.Rect).QueueFree();
					}
					run.Rect = null;
					run.Mat = null;
				}
				else
				{
					Aim(run, num9);
				}
			}
			else if (FxKit.Alive((GodotObject)(object)run.SeamDark) && FxKit.Alive((GodotObject)(object)run.SeamCore) && FxKit.Alive((GodotObject)(object)run.SeamGlow))
			{
				Vector2 val3 = run.A.DirectionTo(run.B);
				Vector2 val4 = new Vector2(0f - val3.Y, val3.X);
				((Node2D)run.SeamCore).Position = val3 * (num9 * 0.5f);
				((Node2D)run.SeamGlow).Position = ((Node2D)run.SeamCore).Position;
				((Node2D)run.SeamDark).Position = val4 * 2.2f - val3 * (num9 * 0.5f);
			}
			float num15 = ((num6 < 0.12f) ? 1f : Mathf.Clamp(1f - (num6 - 0.04f - 0.08f) / 0.34f, 0f, 1f));
			num15 *= num15;
			Color modulate3 = new Color(1f, 1f, 1f, num15);
			if (FxKit.Alive((GodotObject)(object)run.SeamGlow))
			{
				((CanvasItem)run.SeamGlow).Modulate = modulate3;
			}
			if (FxKit.Alive((GodotObject)(object)run.SeamCore))
			{
				((CanvasItem)run.SeamCore).Modulate = modulate3;
			}
			if (FxKit.Alive((GodotObject)(object)run.SeamDark))
			{
				((CanvasItem)run.SeamDark).Modulate = new Color(1f, 1f, 1f, num15 * num7);
			}
		}

		private static void Finish(Run run)
		{
			if (!run.Over)
			{
				run.Over = true;
				FreeCard(run);
				Live.Remove(run);
				run.Rect = null;
				run.Mat = null;
				if (FxKit.Alive((GodotObject)(object)run.Holder))
				{
					((Node)run.Holder).QueueFree();
				}
			}
		}

		static PreciseCutFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		}
	}
