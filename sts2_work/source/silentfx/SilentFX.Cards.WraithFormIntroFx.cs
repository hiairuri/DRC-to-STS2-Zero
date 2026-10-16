// namespace: SilentFX.Cards
// type: WraithFormIntroFx (class)
// source lines: 118095-119307
	public sealed class WraithFormIntroFx : CardFx
	{
		private sealed class Wisp
		{
			public Line2D Line;

			public Sprite2D Head;

			public Vector2[] Pts;

			public Vector2 From;

			public Vector2 Bend;

			public float Start;

			public float Alpha;
		}

		private sealed class Run
		{
			public object Owner;

			public Node2D Back;

			public Node2D Front;

			public ColorRect VeilBack;

			public ColorRect VeilFront;

			public ColorRect FlashRect;

			public ColorRect Warp;

			public ShaderMaterial WarpMat;

			public Sprite2D Core;

			public Node2D MistHold;

			public ShaderMaterial Mist;

			public Node2D RingIn;

			public ShaderMaterial RingInMat;

			public WraithFigure Figure;

			public float Form0;

			public float Wake0;

			public float Clock0;

			public readonly List<Wisp> Wisps = new List<Wisp>();

			public Vector2 At;

			public float R;

			public Vector2 Head;

			public Vector2 Mass;

			public Vector2 Into;

			public Vector2 Feet;

			public Vector2 Body;

			public Vector2 Lo;

			public Vector2 Hi;

			public Vector2 ScreenAt;

			public float Clock;

			public float Pace = 1f;

			public float ScreamAt = 1.05f;

			public float LandAt = 0.86f;

			public float Dark;

			public float Dim;

			public float FlashPeak;

			public float MistAmount;

			public float Fire;

			public bool Pour;

			public bool NoWarp;

			public bool Screamed;

			public bool Smoked;

			public bool Landed;

			public int Phase = 1;

			public ulong Born;

			public bool Done;
		}

		private const float SinkTime = 0.42f;

		private const float WindTime = 0.6f;

		private const float SolidFrom = 0.04f;

		private const float SolidTime = 0.46f;

		private const float HandsFrom = 0.24f;

		private const float HandsTime = 0.54f;

		private const float InhaleTime = 0.22f;

		private const float AwayAfter = 0.26f;

		private const float SmokeAfter = 0.38f;

		private const float LiftAfter = 0.46f;

		private const float LiftTime = 0.62f;

		private const float PourAfter = 0.52f;

		private const float PourTime = 0.34f;

		private const float PourTail = 0.22f;

		private const float PourMist = 0.4f;

		private const float LandAfter = 0.86f;

		private const float Tail = 1.45f;

		private const float WarpTime = 0.62f;

		private const int WispJoints = 10;

		private static readonly List<Run> Runs = new List<Run>();

		private static readonly Action<double> Driver = Drive;

		private static Texture2D _soft;

		private static Texture2D Soft
		{
			get
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Expected Obj, but got Unknown
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Expected Obj, but got Unknown
				if (!FxKit.Alive((GodotObject)(object)_soft))
				{
					Gradient val = new Gradient();
					val.Offsets = new float[3] { 0f, 0.5f, 1f };
					val.Colors = new Color[3]
					{
						new Color(1f, 1f, 1f, 0f),
						new Color(1f, 1f, 1f, 1f),
						new Color(1f, 1f, 1f, 0f)
					};
					_soft = (Texture2D)new GradientTexture2D
					{
						Gradient = val,
						Width = 8,
						Height = 32,
						Fill = (FillEnum)0,
						FillFrom = new Vector2(0.5f, 0f),
						FillTo = new Vector2(0.5f, 1f)
					};
				}
				return _soft;
			}
		}

		public static int PhaseFor(object owner)
		{
			try
			{
				int num = WraithPlayFx.PhaseFor(owner);
				if (num != 0)
				{
					return num;
				}
				ulong ticksMsec = Time.GetTicksMsec();
				for (int num2 = Runs.Count - 1; num2 >= 0; num2--)
				{
					Run run = Runs[num2];
					if (!run.Done && run.Owner == owner && (ticksMsec <= run.Born || ticksMsec - run.Born <= 20000))
					{
						return run.Phase;
					}
				}
			}
			catch
			{
			}
			return 0;
		}

		public static void Reset()
		{
			for (int num = Runs.Count - 1; num >= 0; num--)
			{
				Free(Runs[num]);
			}
			Runs.Clear();
		}

		public override void OnPlay(CardFxContext c)
		{
			if (c == null || Tuning.F("intro.on", 1f) < 0.5f || Tuning.F("wraith.intro.on", 1f) < 0.5f)
			{
				return;
			}
			Run run = new Run();
			try
			{
				CardModel card = c.Card;
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
				run.Owner = owner;
				for (int num = Runs.Count - 1; num >= 0; num--)
				{
					if (Runs[num].Owner == run.Owner)
					{
						Free(Runs[num]);
						Runs.RemoveAt(num);
					}
				}
				if (!Begin(run, c))
				{
					Free(run);
					return;
				}
				Runs.Add(run);
				Ticker.Add(Driver);
			}
			catch (Exception ex)
			{
				DevLog.Warn("Wraith Form opening failed to start: " + ex.Message);
				Free(run);
				Runs.Remove(run);
			}
		}

		private static bool Begin(Run run, CardFxContext c)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_043e: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)c.BackParent) || !FxKit.Alive((GodotObject)(object)c.FrontParent))
			{
				return false;
			}
			Vector2 view = ((c.View.X > 100f && c.View.Y > 100f) ? c.View : FxKit.ViewSize());
			Seen(c.FrontParent, view, out run.Lo, out run.Hi);
			run.Body = new Vector2(Mathf.Clamp(c.Body.X, 120f, 520f), Mathf.Clamp(c.Body.Y, 160f, 620f));
			run.Into = c.Chest + new Vector2(0f, (0f - run.Body.Y) * 0.08f);
			run.Feet = c.Feet;
			run.Pace = Mathf.Clamp(Tuning.F("wraith.intro.pace", 1f), 0.4f, 3f);
			run.ScreamAt = Mathf.Clamp(Tuning.F("wraith.intro.scream_at", 1.05f), 0.5f, 2.5f);
			run.Dark = Mathf.Clamp(Tuning.F("wraith.intro.dark", 0.86f), 0f, 0.97f);
			run.Dim = Mathf.Clamp(Tuning.F("wraith.intro.dim", 0.3f), 0f, 0.8f);
			run.FlashPeak = Mathf.Clamp(Tuning.F("wraith.intro.flash", 0.55f), 0f, 1f);
			run.MistAmount = Mathf.Clamp(Tuning.F("wraith.intro.mist", 0.7f), 0f, 1f);
			run.Fire = Mathf.Clamp(Tuning.F("wraith.intro.fire", 1f), 0f, 1.5f);
			run.Pour = Tuning.F("wraith.intro.pour", 1f) >= 0.5f;
			run.NoWarp = Tuning.F("wraith.intro.warp", 1f) < 0.5f || OwnBackdrop(c);
			run.Born = Time.GetTicksMsec();
			WraithFigure wraithFigure = null;
			float form = 0f;
			if (c.Held is WraithHeld wraithHeld)
			{
				wraithFigure = wraithHeld.Take(out form);
			}
			Vector2 at;
			float r;
			if (wraithFigure != null)
			{
				at = wraithFigure.At;
				r = wraithFigure.R;
			}
			else
			{
				form = 0f;
				WraithFigure.Place(c, run.Lo, run.Hi, out at, out r);
			}
			run.At = at;
			run.R = r;
			run.Form0 = Mathf.Clamp(form, 0f, 1f);
			run.ScreamAt += 0.3f * (1f - run.Form0);
			run.LandAt = (run.Pour ? 0.86f : 0.46f);
			run.Mass = at + new Vector2(0f, r * 0.3f);
			double longest = (double)((run.ScreamAt + 1.45f) * run.Pace) + 3.0;
			run.Back = MakeRoot(c.BackParent, "SilentFX_WraithIntroBack", longest);
			run.Front = MakeRoot(c.FrontParent, "SilentFX_WraithIntroFront", longest);
			if (run.Back == null || run.Front == null)
			{
				wraithFigure?.Free();
				return false;
			}
			run.VeilBack = FxKit.Veil(new Color(0.01f, 0.045f, 0.028f, 0f));
			((Node)run.Back).AddChild((Node)(object)run.VeilBack, false, (InternalMode)0);
			run.Mist = MakeMist(run.Back, run.Mass, new Vector2(r * 3f, r * 2.6f), 0f, 3.7f, out run.MistHold);
			if (wraithFigure != null)
			{
				wraithFigure.MoveTo((Node)(object)run.Back);
			}
			else
			{
				wraithFigure = WraithFigure.Make((Node)(object)run.Back, at, r, run.Lo, run.Hi);
				if (wraithFigure == null)
				{
					return false;
				}
			}
			run.Figure = wraithFigure;
			run.Wake0 = Mathf.Clamp(wraithFigure.Wake, 0f, 1f);
			run.Clock0 = wraithFigure.Clock;
			run.Head = wraithFigure.Head;
			run.ScreenAt = ScreenPoint(c.FrontParent, run.Head, view);
			run.VeilFront = FxKit.Veil(new Color(0f, 0.03f, 0.016f, 0f));
			((Node)run.Front).AddChild((Node)(object)run.VeilFront, false, (InternalMode)0);
			run.Core = FxKit.Glow(new Color(0.8f, 1f, 0.84f, 0f), new Vector2(r * 1.3f, r * 1f));
			((Node2D)run.Core).Position = run.Head;
			((Node)run.Front).AddChild((Node)(object)run.Core, false, (InternalMode)0);
			run.RingIn = FxKit.Sigil(r * 2.6f, new Color(0.62f, 1f, 0.58f, 1f), out run.RingInMat);
			run.RingIn.Position = run.Head;
			((CanvasItem)run.RingIn).Visible = false;
			((Node)run.Front).AddChild((Node)(object)run.RingIn, false, (InternalMode)0);
			if (run.RingInMat != null)
			{
				run.RingInMat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.82f));
				run.RingInMat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.022f));
				run.RingInMat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.55f));
				run.RingInMat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(0f));
			}
			if (run.Pour)
			{
				AddWisps(run);
			}
			run.FlashRect = FxKit.Veil(new Color(0.72f, 1f, 0.8f, 0f));
			((CanvasItem)run.FlashRect).Material = (Material)(object)FxKit.Additive;
			((Node)run.Front).AddChild((Node)(object)run.FlashRect, false, (InternalMode)0);
			Step(run);
			return true;
		}

		private static Node2D MakeRoot(Node parent, string name, double longest)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected Obj, but got Unknown
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit(name)
			};
			TimeFreeze.KeepLive((Node)(object)val);
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return null;
			}
			val.GlobalTransform = Transform2D.Identity;
			FxKit.FreeAfter((Node)(object)val, longest);
			return val;
		}

		private static ShaderMaterial MakeMist(Node2D parent, Vector2 heart, Vector2 size, float layer, float seed, out Node2D holder)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Expected Obj, but got Unknown
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			holder = null;
			ShaderMaterial val = ShaderLib.NewMaterial("wraith_shroud");
			if (val == null)
			{
				return null;
			}
			val.SetShaderParameter(StringName.op_Implicit("layer"), Variant.op_Implicit(layer));
			val.SetShaderParameter(StringName.op_Implicit("seed"), Variant.op_Implicit(seed));
			val.SetShaderParameter(StringName.op_Implicit("clock"), Variant.op_Implicit(0f));
			val.SetShaderParameter(StringName.op_Implicit("intensity"), Variant.op_Implicit(0f));
			val.SetShaderParameter(StringName.op_Implicit("rise"), Variant.op_Implicit(0f));
			val.SetShaderParameter(StringName.op_Implicit("burst"), Variant.op_Implicit(0f));
			val.SetShaderParameter(StringName.op_Implicit("lean"), Variant.op_Implicit(0f));
			holder = new Node2D
			{
				Position = heart
			};
			((Node)parent).AddChild((Node)(object)holder, false, (InternalMode)0);
			ColorRect val2 = FxKit.Quad(size, (Material)(object)val);
			((Control)val2).Position = new Vector2((0f - size.X) * 0.5f, (0f - size.Y) * 0.5f);
			((Node)holder).AddChild((Node)(object)val2, false, (InternalMode)0);
			return val;
		}

		private static void AddWisps(Run run)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			float r = run.R;
			WraithFigure figure = run.Figure;
			float num = r * 0.5f;
			AddWisp(run, figure.FistLeft + new Vector2(0f, (0f - num) * 0.3f), 0.52f, new Color(0.5f, 1f, 0.4f, 0.95f), 46f, 1f, 401, num);
			AddWisp(run, figure.FistRight + new Vector2(0f, (0f - num) * 0.3f), 0.57f, new Color(0.5f, 1f, 0.4f, 0.95f), 46f, 1f, 408, num);
			int num2 = Mathf.Clamp(Tuning.I("wraith.intro.pour_wisps", 9), 0, 24);
			for (int i = 0; i < num2; i++)
			{
				int num3 = i * 53 + 611;
				Vector2 origin = run.At + new Vector2((FxKit.Hash(num3) - 0.5f) * r * 1.5f, (FxKit.Hash(num3 + 1) - 0.6f) * r * 1.4f);
				AddWisp(run, origin, 0.48f + 0.17f * FxKit.Hash(num3 + 2), new Color(0.84f, 1f, 0.88f, 0.8f), 16f + 16f * FxKit.Hash(num3 + 3), 0.75f, num3 + 4, 0f);
			}
		}

		private static void AddWisp(Run run, Vector2 origin, float start, Color color, float width, float alpha, int h, float ball)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = run.Into;
			Wisp wisp = new Wisp
			{
				Pts = new Vector2[10],
				From = origin,
				Bend = new Vector2(Mathf.Lerp(origin.X, val.X, 0.55f) + (origin.X - val.X) * 0.35f + (FxKit.Hash(h) - 0.5f) * run.R * 0.35f, Mathf.Min(origin.Y, val.Y) - run.R * (0.3f + 0.25f * FxKit.Hash(h + 1))),
				Start = start,
				Alpha = alpha
			};
			for (int i = 0; i < 10; i++)
			{
				wisp.Pts[i] = origin;
			}
			wisp.Line = FxKit.Ribbon(width, color);
			wisp.Line.Texture = Soft;
			wisp.Line.TextureMode = (LineTextureMode)2;
			((CanvasItem)wisp.Line).Visible = false;
			((Node)run.Front).AddChild((Node)(object)wisp.Line, false, (InternalMode)0);
			if (ball > 1f)
			{
				wisp.Head = FxKit.Glow(new Color(color.R, color.G, color.B, 0f), new Vector2(ball, ball));
				((Node2D)wisp.Head).Position = origin;
				((CanvasItem)wisp.Head).Visible = false;
				((Node)run.Front).AddChild((Node)(object)wisp.Head, false, (InternalMode)0);
			}
			run.Wisps.Add(wisp);
		}

		internal static void Seen(Node parent, Vector2 view, out Vector2 lo, out Vector2 hi)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			lo = Vector2.Zero;
			hi = view;
			try
			{
				CanvasItem val = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
				if (val != null && FxKit.Alive((GodotObject)(object)val))
				{
					Transform2D canvasTransform = val.GetCanvasTransform();
					Transform2D val2 = canvasTransform.AffineInverse();
					Vector2 val3 = val2 * Vector2.Zero;
					Vector2 val4 = val2 * view;
					Vector2 val5 = new Vector2(Mathf.Min(val3.X, val4.X), Mathf.Min(val3.Y, val4.Y));
					Vector2 val6 = new Vector2(Mathf.Max(val3.X, val4.X), Mathf.Max(val3.Y, val4.Y));
					float num = val6.X - val5.X;
					float num2 = val6.Y - val5.Y;
					if (num > 200f && num < 20000f && num2 > 200f && num2 < 20000f)
					{
						lo = val5;
						hi = val6;
					}
				}
			}
			catch
			{
			}
		}

		private static Vector2 ScreenPoint(Node parent, Vector2 global, Vector2 view)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Vector2 val = global;
				CanvasItem val2 = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
				if (val2 != null && FxKit.Alive((GodotObject)(object)val2))
				{
					val = val2.GetCanvasTransform() * global;
				}
				if (view.X < 1f || view.Y < 1f)
				{
					return new Vector2(0.5f, 0.37f);
				}
				return new Vector2(Mathf.Clamp(val.X / view.X, 0f, 1f), Mathf.Clamp(val.Y / view.Y, 0f, 1f));
			}
			catch
			{
				return new Vector2(0.5f, 0.37f);
			}
		}

		private static bool OwnBackdrop(CardFxContext c)
		{
			try
			{
				CardModel card = c.Card;
				object obj;
				if (card == null)
				{
					obj = null;
				}
				else
				{
					Player owner = card.Owner;
					if (owner == null)
					{
						obj = null;
					}
					else
					{
						Creature creature = owner.Creature;
						obj = ((creature != null) ? creature.CombatState : null);
					}
				}
				object obj2 = ((obj != null) ? ((ICombatState)obj).Encounter : null);
				if (obj2 == null)
				{
					return false;
				}
				PropertyInfo property = obj2.GetType().GetProperty("HasCustomBackground", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				bool flag = default;
				int num;
				if (property != null)
				{
					object value = property.GetValue(obj2);
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
				return (byte)((uint)num & (flag ? 1u : 0u)) != 0;
			}
			catch
			{
				return false;
			}
		}

		private static void Drive(double realDt)
		{
			if (Runs.Count == 0)
			{
				Ticker.Remove(Driver);
				return;
			}
			float num = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			for (int num2 = Runs.Count - 1; num2 >= 0; num2--)
			{
				Run run = Runs[num2];
				try
				{
					if (run.Done || !FxKit.Alive((GodotObject)(object)run.Back) || !FxKit.Alive((GodotObject)(object)run.Front) || !((Node)run.Front).IsInsideTree())
					{
						Free(run);
						Runs.RemoveAt(num2);
					}
					else
					{
						run.Clock += num / run.Pace;
						Step(run);
						if (run.Clock >= run.ScreamAt + 1.45f)
						{
							Free(run);
							Runs.RemoveAt(num2);
						}
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Wraith Form opening: " + ex.Message);
					Free(run);
					Runs.Remove(run);
				}
			}
		}

		private static void Free(Run run)
		{
			if (run == null)
			{
				return;
			}
			run.Done = true;
			run.Phase = 0;
			try
			{
				if (FxKit.Alive((GodotObject)(object)run.Back))
				{
					((Node)run.Back).QueueFree();
				}
				else
				{
					run.Figure?.Free();
				}
				if (FxKit.Alive((GodotObject)(object)run.Front))
				{
					((Node)run.Front).QueueFree();
				}
			}
			catch
			{
			}
			run.Back = null;
			run.Front = null;
			run.Figure = null;
			run.Wisps.Clear();
		}

		private static float Clamp01(float v)
		{
			if (!(v < 0f))
			{
				if (!(v > 1f))
				{
					return v;
				}
				return 1f;
			}
			return 0f;
		}

		private static float EaseOut(float v)
		{
			return 1f - (1f - v) * (1f - v);
		}

		private static float Smooth(float v)
		{
			return v * v * (3f - 2f * v);
		}

		private static float BackOut(float v)
		{
			v = Clamp01(v) - 1f;
			return 1f + 2.70158f * v * v * v + 1.70158f * v * v;
		}

		private static void Step(Run run)
		{
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0621: Unknown result type (might be due to invalid IL or missing references)
			//IL_075c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0783: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_06de: Unknown result type (might be due to invalid IL or missing references)
			float clock = run.Clock;
			float num = clock - run.ScreamAt;
			float num2 = ((num < 0f) ? Smooth(Clamp01((clock - (run.ScreamAt - 0.22f)) / 0.22f)) : 0f);
			float num3 = ((num < 0f) ? 0f : Smooth(Clamp01((num - 0.46f) / 0.62f)));
			float num4 = ((num < 0f) ? 0f : Mathf.Exp((0f - num) / 0.1f));
			if (!run.Screamed && num >= 0f)
			{
				Scream(run);
			}
			if (run.Screamed && !run.Smoked && num >= 0.38f)
			{
				Smoke(run);
			}
			if (run.Screamed && !run.Landed && num >= run.LandAt)
			{
				Land(run);
			}
			run.Phase = ((num < 0f) ? 1 : (run.Landed ? 3 : 2));
			float num5 = EaseOut(Clamp01(clock / 0.42f)) * (1f + 0.1f * num2) * (1f - num3);
			float num6 = (PlayerSettings.ReduceScreenEffects ? 0.55f : 1f);
			if (FxKit.Alive((GodotObject)(object)run.VeilBack))
			{
				run.VeilBack.Color = new Color(0.01f, 0.045f, 0.028f, Clamp01(run.Dark * num5 * num6));
			}
			if (FxKit.Alive((GodotObject)(object)run.VeilFront))
			{
				run.VeilFront.Color = new Color(0f, 0.03f, 0.016f, Clamp01(run.Dim * num5 * num6));
			}
			if (FxKit.Alive((GodotObject)(object)run.FlashRect))
			{
				float num7 = ((num < 0f) ? 0f : (run.FlashPeak * Mathf.Exp((0f - num) / 0.085f) * num6));
				run.FlashRect.Color = new Color(0.72f, 1f, 0.8f, (num7 < 0.004f) ? 0f : num7);
			}
			float num8 = ((num < 0f) ? 0f : EaseOut(Clamp01(num / 0.55f)));
			float num9 = 0f;
			float num10 = 1f;
			if (num >= 0f)
			{
				if (run.Pour)
				{
					num9 = Clamp01((num - 0.52f) / 0.4f);
					num9 *= num9;
					num10 = (1f - 0.55f * Smooth(Clamp01((num - 0.06f) / 0.28f))) * (1f - num9 * num9);
				}
				else
				{
					num10 = 1f - Smooth(Clamp01((num - 0.06f) / 0.56f));
				}
			}
			float num11 = run.MistAmount * Smooth(Clamp01(clock / 0.36f));
			float rise = EaseOut(Clamp01((clock - 0.04f) / 0.72f));
			float num12 = 0.1f * Mathf.Sin(clock * 1.9f);
			Vector2 val = new Vector2(1f - 0.07f * num2 + 0.4f * num8, 1f - 0.04f * num2 + 0.3f * num8);
			if (FxKit.Alive((GodotObject)(object)run.MistHold))
			{
				run.MistHold.Position = ((num9 > 0f) ? run.Mass.Lerp(run.Into, num9) : run.Mass);
			}
			SetMist(run.Mist, run.MistHold, clock, num11 * num10, rise, num8, num12 * 0.6f, new Vector2(val.X * Mathf.Lerp(1f, 0.14f, num9), val.Y * Mathf.Lerp(1f, 0.4f, num9)));
			WraithFigure figure = run.Figure;
			if (figure != null && figure.Alive)
			{
				float num13 = 0.25f * (1f - run.Form0);
				figure.Wake = Mathf.Max(run.Wake0, Clamp01(clock / 0.15f));
				figure.Reach = 1f;
				figure.Form = Clamp01(run.Form0 + clock / 0.6f);
				figure.Solid = Smooth(Clamp01((clock - 0.04f - num13) / 0.46f));
				figure.Hands = Clamp01((clock - 0.24f - num13) / 0.54f);
				figure.Blast = ((num < 0f) ? (-0.22f * num2) : (BackOut(num / 0.1f) * (1f - 0.25f * Smooth(Clamp01((num - 0.1f) / 0.3f)))));
				figure.Away = ((num < 0.26f) ? 0f : Clamp01((num - 0.26f) / 0.5f));
				float num14 = run.Fire * (1f - 0.42f * num2);
				if (num >= 0f)
				{
					num14 *= 1f + 0.85f * Mathf.Exp((0f - num) / 0.12f);
					num14 = ((!run.Pour) ? (num14 * (1f - Smooth(Clamp01((num - 0.3f) / 0.5f)))) : (num14 * (1f - Smooth(Clamp01((num - 0.42f) / 0.22f)))));
				}
				figure.Fire = num14;
				figure.Clock = run.Clock0 + clock;
				figure.Step();
			}
			if (FxKit.Alive((GodotObject)(object)run.Core))
			{
				float v = ((num < 0f) ? (0.1f * Clamp01((clock - 0.3f) / 0.5f) * (1f - 0.3f * num2)) : ((0.1f + 0.8f * num4) * (1f - Smooth(Clamp01((num - 0.12f) / 0.4f)))));
				((CanvasItem)run.Core).Modulate = new Color(0.8f, 1f, 0.84f, Clamp01(v));
			}
			if (FxKit.Alive((GodotObject)(object)run.RingIn))
			{
				float num15 = Clamp01((clock - (run.ScreamAt - 0.22f)) / 0.22f);
				bool flag = num < 0f && num15 > 0f;
				if (((CanvasItem)run.RingIn).Visible != flag)
				{
					((CanvasItem)run.RingIn).Visible = flag;
				}
				if (flag)
				{
					float num16 = Mathf.Lerp(2.6f, 0.3f, num15 * num15);
					run.RingIn.Scale = new Vector2(num16, num16);
					if (FxKit.Alive((GodotObject)(object)run.RingInMat))
					{
						run.RingInMat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(0.55f * num15 * num6));
					}
				}
			}
			if (FxKit.Alive((GodotObject)(object)run.Warp))
			{
				if (num > 0.62f)
				{
					((Node)run.Warp).QueueFree();
					run.Warp = null;
				}
				else if (FxKit.Alive((GodotObject)(object)run.WarpMat))
				{
					float num17 = 1f - Clamp01(num / 0.62f);
					run.WarpMat.SetShaderParameter(StringName.op_Implicit("reach"), Variant.op_Implicit(0.05f + Mathf.Max(0f, num) * 2.6f));
					run.WarpMat.SetShaderParameter(StringName.op_Implicit("amount"), Variant.op_Implicit(num17 * Mathf.Sqrt(num17) * num6));
				}
			}
			if (num > 0f)
			{
				for (int i = 0; i < run.Wisps.Count; i++)
				{
					StepWisp(run.Wisps[i], run.Into, num);
				}
			}
		}

		private static void SetMist(ShaderMaterial mat, Node2D holder, float clock, float intensity, float rise, float burst, float lean, Vector2 swell)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)holder))
			{
				holder.Scale = swell;
			}
			if (FxKit.Alive((GodotObject)(object)mat))
			{
				mat.SetShaderParameter(StringName.op_Implicit("clock"), Variant.op_Implicit(clock));
				mat.SetShaderParameter(StringName.op_Implicit("intensity"), Variant.op_Implicit(Clamp01(intensity * (PlayerSettings.ReduceScreenEffects ? 0.65f : 1f))));
				mat.SetShaderParameter(StringName.op_Implicit("rise"), Variant.op_Implicit(rise));
				mat.SetShaderParameter(StringName.op_Implicit("burst"), Variant.op_Implicit(burst));
				mat.SetShaderParameter(StringName.op_Implicit("lean"), Variant.op_Implicit(lean));
			}
		}

		private static void StepWisp(Wisp w, Vector2 into, float since)
		{
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			Line2D line = w.Line;
			if (!FxKit.Alive((GodotObject)(object)line))
			{
				return;
			}
			float num = (since - w.Start) / 0.34f;
			bool flag = FxKit.Alive((GodotObject)(object)w.Head);
			if (num <= 0f || num >= 1.22f)
			{
				if (((CanvasItem)line).Visible)
				{
					((CanvasItem)line).Visible = false;
				}
				if (flag && ((CanvasItem)w.Head).Visible)
				{
					((CanvasItem)w.Head).Visible = false;
				}
				return;
			}
			float num2 = Clamp01(num);
			num2 *= num2;
			float num3 = Clamp01(num - 0.22f);
			num3 *= num3;
			int num4 = w.Pts.Length;
			for (int i = 0; i < num4; i++)
			{
				w.Pts[i] = FxKit.Bezier(w.From, w.Bend, into, Mathf.Lerp(num3, num2, (float)i / (float)(num4 - 1)));
			}
			float num5 = Clamp01(w.Alpha * Mathf.Min(1f, num * 5f) * (1f - Clamp01((num - 1f) / 0.22f)));
			line.Points = w.Pts;
			((CanvasItem)line).Modulate = new Color(1f, 1f, 1f, num5);
			if (!((CanvasItem)line).Visible)
			{
				((CanvasItem)line).Visible = true;
			}
			if (flag)
			{
				Color modulate = ((CanvasItem)w.Head).Modulate;
				((Node2D)w.Head).Position = w.Pts[num4 - 1];
				((CanvasItem)w.Head).Modulate = new Color(modulate.R, modulate.G, modulate.B, num5 * 0.85f);
				if (!((CanvasItem)w.Head).Visible)
				{
					((CanvasItem)w.Head).Visible = true;
				}
			}
		}

		private static void Scream(Run run)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Expected Obj, but got Unknown
			run.Screamed = true;
			if (!FxKit.Alive((GodotObject)(object)run.Front))
			{
				return;
			}
			Vector2 head = run.Head;
			float r = run.R;
			float pace = run.Pace;
			int num = Tuning.I("wraith.intro.shake", 3);
			if (num > 0)
			{
				GameRefs.Shake(num);
			}
			float num2 = Mathf.Clamp(Tuning.F("wraith.intro.sound", 1f), 0f, 2f);
			if (num2 > 0.01f)
			{
				Sfx.Play("wraith_scream", num2);
			}
			if (!run.NoWarp)
			{
				try
				{
					ShaderMaterial val = ShaderLib.NewMaterial("wraith_scream");
					if (val != null)
					{
						val.SetShaderParameter(StringName.op_Implicit("amount"), Variant.op_Implicit(0f));
						val.SetShaderParameter(StringName.op_Implicit("reach"), Variant.op_Implicit(0.05f));
						val.SetShaderParameter(StringName.op_Implicit("centre"), Variant.op_Implicit(run.ScreenAt));
						ColorRect val2 = FxKit.Veil(Colors.White);
						((CanvasItem)val2).Material = (Material)(object)val;
						((Node)val2).AddChild((Node)new BackBufferCopy
						{
							CopyMode = (CopyModeEnum)2,
							ShowBehindParent = true
						}, false, (InternalMode)0);
						((Node)run.Front).AddChild((Node)(object)val2, false, (InternalMode)0);
						run.Warp = val2;
						run.WarpMat = val;
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Wraith Form opening (the bent air): " + ex.Message);
					run.Warp = null;
					run.WarpMat = null;
				}
			}
			FxKit.Flash((Node)(object)run.Front, head, new Color(0.82f, 1f, 0.84f, 0.95f), r * 1.8f, 3.2f, 0.32 * (double)pace);
			Vector2 val3 = run.Hi - run.Lo;
			float size = Mathf.Max(val3.X, val3.Y) * 0.5f;
			FxKit.RingPulse((Node)(object)run.Front, head, size, new Color(0.88f, 1f, 0.9f, 1f), 0f, 0.42f * pace, 0.1f, 5.2f, 1f, 0f, 0.04f, 0.95f);
			FxKit.RingPulse((Node)(object)run.Front, head, size, new Color(0.45f, 1f, 0.38f, 1f), 0.06f * pace, 0.56f * pace, 0.1f, 4.4f, 1f, 0f, 0.065f, 0.7f);
			FxKit.RingPulse((Node)(object)run.Front, head, size, new Color(0.16f, 0.72f, 0.28f, 1f), 0.14f * pace, 0.72f * pace, 0.1f, 3.6f, 1f, 0f, 0.095f, 0.5f);
			FxKit.Burst((Node)(object)run.Front, head, new Color(0.58f, 1f, 0.46f, 0.95f), 48, 420f, 1500f, 0.55 * (double)pace, 0.05f, 0.15f, new Vector2(0f, -90f), 180f, null, r * 0.3f, 520f);
		}

		private static void Smoke(Run run)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			run.Smoked = true;
			Vector2 val = run.At + new Vector2(0f, run.R * 0.1f);
			float pace = run.Pace;
			if (FxKit.Alive((GodotObject)(object)run.Front))
			{
				FxKit.Burst((Node)(object)run.Front, val, new Color(0.76f, 0.92f, 0.8f, 0.18f), 50, 260f, 1100f, 0.8 * (double)pace, 0.9f, 2.8f, new Vector2(0f, -60f), 180f, null, run.R * 0.7f, 1200f, additive: true, 0.8f);
			}
			if (FxKit.Alive((GodotObject)(object)run.Back))
			{
				FxKit.Burst((Node)(object)run.Back, val + new Vector2(0f, run.R * 0.4f), new Color(0.03f, 0.1f, 0.06f, 0.6f), 40, 200f, 900f, 0.9 * (double)pace, 1.2f, 3.2f, new Vector2(0f, -40f), 180f, null, run.R * 0.8f, 1000f, additive: false, 0.8f);
			}
			int num = Tuning.I("wraith.intro.shake2", 2);
			if (num > 0)
			{
				GameRefs.Shake(num);
			}
		}

		private static void Land(Run run)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			run.Landed = true;
			if (run.Pour && FxKit.Alive((GodotObject)(object)run.Front))
			{
				Vector2 val = run.Into;
				float pace = run.Pace;
				FxKit.Flash((Node)(object)run.Front, val, new Color(0.6f, 1f, 0.62f, 0.85f), run.Body.Y * 0.95f, 1.9f, 0.26 * (double)pace);
				FxKit.RingPulse((Node)(object)run.Front, val, run.Body.Y * 1.5f, new Color(0.62f, 1f, 0.6f, 1f), 0f, 0.36f * pace, 0.25f, 1.25f, 1f, 0f, 0.05f, 0.65f);
				FxKit.Burst((Node)(object)run.Front, val, new Color(0.62f, 1f, 0.66f, 0.45f), 18, 30f, 150f, 0.42 * (double)pace, 0.25f, 0.6f, new Vector2(0f, -160f), 180f, null, run.Body.X * 0.28f, 80f);
				Vector2 globalPos = new Vector2(val.X, Mathf.Lerp(val.Y, run.Feet.Y, 0.8f));
				FxKit.Burst((Node)(object)run.Front, globalPos, new Color(0.3f, 0.86f, 0.46f, 0.3f), 22, 260f, 760f, 0.85 * (double)pace, 1f, 2.6f, new Vector2(0f, -30f), 14f, new Vector2(1f, 0f), run.Body.X * 0.2f, 900f, additive: true, 0.9f);
				FxKit.Burst((Node)(object)run.Front, globalPos, new Color(0.3f, 0.86f, 0.46f, 0.3f), 22, 260f, 760f, 0.85 * (double)pace, 1f, 2.6f, new Vector2(0f, -30f), 14f, new Vector2(-1f, 0f), run.Body.X * 0.2f, 900f, additive: true, 0.9f);
				int num = Tuning.I("wraith.intro.shake3", 0);
				if (num > 0)
				{
					GameRefs.Shake(num);
				}
			}
		}
	}
