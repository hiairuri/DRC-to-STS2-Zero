// namespace: SilentFX.Cards
// type: ShadowStepFx (class)
// source lines: 92617-95653
	public sealed class ShadowStepFx : CardFx
	{
		private sealed class Rolled
		{
			public CardModel Card;

			public NCard Real;

			public NCard Copy;

			public Node2D Rig;

			public Node2D Soft;

			public ShaderMaterial SoftMat;

			public Vector2 At;

			public float Turn;

			public Vector2 Size;

			public float Dark0;

			public float Lag;

			public float Side;

			public float Rate;

			public float CaughtAt = -1f;

			public int Order = -1;

			public float LeftAt = -1f;

			public Vector2 From;

			public float FromTurn;

			public float FromSize = 1f;

			public Vector2 Tan;

			public Vector2 Now;

			public float NowTurn;

			public float NowSize = 1f;

			public float NowAlpha = 1f;

			public Vector2 Vel;

			public bool Poured;

			public bool Claimed;

			public bool Flown;

			public bool Gone;
		}

		private struct Wisp
		{
			public Vector2 Pos;

			public Vector2 Vel;

			public float R;

			public float Born;

			public float Life;

			public float Grow;

			public int Layer;
		}

		private struct Past
		{
			public float T;

			public Vector2 At;

			public float R;

			public float Back;

			public float Hand;

			public float Front;

			public float Far;
		}

		private struct TailPuff
		{
			public Vector2 At;

			public float R;

			public float Thick;

			public float Age;

			public float Seen;

			public Vector2 Drawn;

			public float Back;

			public float Hand;

			public float Front;

			public float Far;
		}

		private struct RoofPiece
		{
			public float Along;

			public float R;

			public float Born;

			public float Back;

			public float Hand;

			public float Front;

			public int N;
		}

		private struct RoofPuff
		{
			public Vector2 At;

			public float R;

			public float Thick;

			public float Age;

			public float Seen;

			public Vector2 Drawn;

			public Vector2 Way;

			public float Combed;

			public float Back;

			public float Hand;

			public float Front;
		}

		private sealed class Look
		{
			public float T;

			public float Back;

			public float Hand;

			public float Front;

			public float Fade = 1f;

			public float Lobe;

			public float Ragged;

			public float LumpAt = 0.5f;

			public float LongR = 1f;

			public float BodyR = 100f;

			public Vector2 Drawn;

			public Vector2 Axis = Vector2.Right;

			public float Combed;

			public float Mark;

			public float Charging;

			public float Reaching;

			public Vector2 Down = Vector2.Down;

			public float Pulse;

			public float Fast;

			public float Reach = 1f;

			public float DrawnIn;

			public float Mist;

			public Look()
			{
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			}
		}

		private sealed class Run
		{
			public CardFxContext C;

			public Creature Owner;

			public Node2D Root;

			public Node2D DimRoot;

			public Node2D TopRoot;

			public ColorRect Dim;

			public ShadowCloud Back;

			public ShadowCloud Body;

			public Node2D CardLayer;

			public Node2D SoftLayer;

			public ShadowCloud Veil;

			public ShadowCloud Front;

			public Node2D ShockFrame;

			public ColorRect Shock;

			public ShaderMaterial ShockMat;

			public float ShockAt = -1f;

			public int ShockKind;

			public readonly List<Rolled> Cards = new List<Rolled>();

			public readonly List<Wisp> Wisps = new List<Wisp>();

			public readonly List<Past> Way = new List<Past>();

			public readonly TailPuff[] Tail = new TailPuff[12];

			public readonly List<RoofPiece> Roof = new List<RoofPiece>();

			public readonly List<RoofPuff> RoofNow = new List<RoofPuff>();

			public float RoofAt = -1f;

			public int RoofCount;

			public float Roofed;

			public float ShadeLo;

			public float ShadeHi;

			public Node2D ShadeRoot;

			public ColorRect ShadeBackQuad;

			public ColorRect ShadeFrontQuad;

			public ShaderMaterial ShadeBack;

			public ShaderMaterial ShadeFront;

			public readonly Look Now = new Look();

			public ShadowRoad Road;

			public float Reach0;

			public float Clock0;

			public float Dark0;

			public float Arrive;

			public float Charge;

			public float Round;

			public float HangFor;

			public float Rear;

			public float Drop;

			public float Stop;

			public float After;

			public float Clock;

			public float Anim;

			public float Hold;

			public float Last;

			public float Spin;

			public Vector2 Head;

			public Vector2 Heading = Vector2.Right;

			public float R = 100f;

			public float Speed;

			public float Far;

			public Vector2 WispAt;

			public int WispCount;

			public int Caught;

			public float FlingAt = -1f;

			public float HitAt = -1f;

			public float CarriedAt = -1f;

			public float Patience;

			public int Flicks;

			public int Neared;

			public bool[] Kicked;

			public int Taken;

			public int TakenSeen;

			public float DoneFor;

			public bool Launched;

			public bool Slammed;

			public bool StrikeHeard;

			public bool Landed;

			public bool Cleared;

			public bool Finished;

			public Run()
			{
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			}
		}

		private const int Under = 0;

		private const int Hand = 1;

		private const int Top = 2;

		private const float Grab = 0.085f;

		private const int TailPuffs = 12;

		private const int RoofMax = 24;

		private static readonly StringName PFromX = StringName.op_Implicit("from_x");

		private static readonly StringName PToX = StringName.op_Implicit("to_x");

		private static readonly StringName PLean = StringName.op_Implicit("lean");

		private static readonly StringName PAmount = StringName.op_Implicit("amount");

		private static readonly StringName PSoft = StringName.op_Implicit("soft");

		private static readonly StringName PSmear = StringName.op_Implicit("smear");

		private static readonly List<Run> Live = new List<Run>();

		public override void OnPlay(CardFxContext c)
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
					obj = ((owner != null) ? owner.Creature : null);
				}
				Creature val = (Creature)obj;
				ShadowEcho.Mark(val);
				for (int num = Live.Count - 1; num >= 0; num--)
				{
					if (!FxKit.Alive((GodotObject)(object)Live[num].Root) || Live[num].Owner == val)
					{
						Finish(Live[num]);
					}
				}
				Run run = new Run
				{
					C = c,
					Owner = val
				};
				bool flag = false;
				try
				{
					flag = Begin(run);
				}
				catch (Exception ex)
				{
					DevLog.Warn("Shadow Step failed to start: " + ex.Message);
				}
				if (!flag)
				{
					Finish(run);
				}
				ShadowStepHeld.HandOver();
			}
			catch (Exception ex2)
			{
				DevLog.Warn("ShadowStepFx.OnPlay: " + ex2);
			}
		}

		internal static void EndAll()
		{
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				try
				{
					Finish(Live[num]);
				}
				catch (Exception ex)
				{
					DevLog.Warn("Shadow Step (end all): " + ex.Message);
				}
			}
			Live.Clear();
		}

		private static float ChargeTime()
		{
			return Mathf.Clamp(Tuning.F("shadowstep.charge", 0.28f), 0f, 0.6f);
		}

		private static float RoundTime()
		{
			return Mathf.Clamp(Tuning.F("shadowstep.round", 1.5f), 0.8f, 3f);
		}

		private static float HangTime()
		{
			return Mathf.Clamp(Tuning.F("shadowstep.hang", 0.3f), 0f, 1f);
		}

		private static float RearTime()
		{
			return Mathf.Clamp(Tuning.F("shadowstep.rear", 0.16f), 0.02f, 0.4f);
		}

		private static float DropTime()
		{
			return Mathf.Clamp(Tuning.F("shadowstep.drop", 0.15f), 0.05f, 0.4f);
		}

		private static float HoldTime()
		{
			return Mathf.Clamp(Tuning.F("shadowstep.stop", 0.05f), 0f, 0.2f);
		}

		private static float AfterTime()
		{
			return Mathf.Clamp(Tuning.F("shadowstep.after", 0.55f), 0.2f, 1f);
		}

		internal static float HomeAfter()
		{
			return 0.2f + ChargeTime() + RoundTime() + HangTime() + RearTime() + DropTime() + HoldTime();
		}

		private static float RollDim()
		{
			return Mathf.Clamp(Tuning.F("shadowstep.roll_dim", 0.38f), 0f, 0.9f);
		}

		private static float TrailLife()
		{
			return Mathf.Clamp(Tuning.F("shadowstep.trail", 0.72f), 0.15f, 1.5f);
		}

		private static float Smooth(float x)
		{
			x = Mathf.Clamp(x, 0f, 1f);
			return x * x * (3f - 2f * x);
		}

		private static bool CanHost(Node node)
		{
			if (FxKit.Alive((GodotObject)(object)node) && node.IsInsideTree())
			{
				CanvasItem val = (CanvasItem)(object)((node is CanvasItem) ? node : null);
				if (val != null)
				{
					return val.IsVisibleInTree();
				}
				return true;
			}
			return false;
		}

		private static bool ScreenReads(CardFxContext c)
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
					ICombatState combatState = card.CombatState;
					obj = ((combatState != null) ? combatState.Encounter : null);
				}
				EncounterModel val = (EncounterModel)obj;
				if (val == null)
				{
					return true;
				}
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
				return ((uint)num & (flag ? 1u : 0u)) == 0;
			}
			catch
			{
				return false;
			}
		}

		private static bool HasShader(string name)
		{
			try
			{
				return File.Exists(Path.Combine(DevLog.ModDir, "shaders", name + ".gdshader"));
			}
			catch
			{
				return false;
			}
		}

		private static bool Begin(Run run)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected Obj, but got Unknown
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Expected Obj, but got Unknown
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Expected Obj, but got Unknown
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Expected Obj, but got Unknown
			//IL_097b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0980: Unknown result type (might be due to invalid IL or missing references)
			//IL_0990: Unknown result type (might be due to invalid IL or missing references)
			//IL_099c: Expected Obj, but got Unknown
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Expected Obj, but got Unknown
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Expected Obj, but got Unknown
			//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a81: Unknown result type (might be due to invalid IL or missing references)
			//IL_0698: Unknown result type (might be due to invalid IL or missing references)
			//IL_069d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_070d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0712: Unknown result type (might be due to invalid IL or missing references)
			//IL_0727: Expected Obj, but got Unknown
			//IL_075a: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_085e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0863: Unknown result type (might be due to invalid IL or missing references)
			//IL_0878: Expected Obj, but got Unknown
			//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
			CardFxContext c = run.C;
			Node val = GameRefs.UiLayer;
			if (!FxKit.Alive((GodotObject)(object)val))
			{
				val = c.FrontParent;
			}
			if (!FxKit.Alive((GodotObject)(object)val) || !val.IsInsideTree())
			{
				return false;
			}
			Node2D val2 = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_ShadowRun"),
				ZIndex = 7
			};
			val.AddChild((Node)(object)val2, false, (InternalMode)0);
			if (!((Node)val2).IsInsideTree())
			{
				((Node)val2).QueueFree();
				return false;
			}
			run.Root = val2;
			Live.Add(run);
			FxLifetime.Watch((Node)(object)val2, 9.100000381469727, () =>
			{
				Finish(run);
			});
			ShadowPoolFx.Expect(run.Owner);
			run.Body = new ShadowCloud(3.7f, veil: false, aura: true);
			((Node)val2).AddChild((Node)(object)run.Body.Node, false, (InternalMode)0);
			run.CardLayer = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_ShadowRollCards")
			};
			((Node)val2).AddChild((Node)(object)run.CardLayer, false, (InternalMode)0);
			bool flag = ScreenReads(c);
			if (flag && Tuning.F("shadowstep.card_blur", 1f) > 0.01f && HasShader("shadow_blur"))
			{
				try
				{
					Node2D val3 = new Node2D
					{
						Name = StringName.op_Implicit("SilentFX_ShadowRollSoft"),
						Visible = false
					};
					((Node)val2).AddChild((Node)(object)val3, false, (InternalMode)0);
					((Node)val3).AddChild((Node)new BackBufferCopy
					{
						CopyMode = (CopyModeEnum)2
					}, false, (InternalMode)0);
					run.SoftLayer = val3;
				}
				catch (Exception ex)
				{
					DevLog.Warn("Shadow Step (soft cards): " + ex.Message);
					run.SoftLayer = null;
				}
			}
			run.Veil = new ShadowCloud(3.7f, veil: true, aura: false);
			((Node)val2).AddChild((Node)(object)run.Veil.Node, false, (InternalMode)0);
			if (flag && Tuning.F("shadowstep.shock", 1f) > 0.01f && HasShader("shadow_shock"))
			{
				try
				{
					ShaderMaterial val4 = ShaderLib.NewMaterial("shadow_shock");
					if (val4 != null)
					{
						Node2D val5 = new Node2D
						{
							Name = StringName.op_Implicit("SilentFX_ShadowShock"),
							Visible = false
						};
						((Node)val2).AddChild((Node)(object)val5, false, (InternalMode)0);
						ColorRect val6 = FxKit.Quad(new Vector2(8f, 8f), (Material)(object)val4);
						val6.Color = new Color(1f, 1f, 1f, 0f);
						((Node)val6).AddChild((Node)new BackBufferCopy
						{
							CopyMode = (CopyModeEnum)2,
							ShowBehindParent = true
						}, false, (InternalMode)0);
						((Node)val5).AddChild((Node)(object)val6, false, (InternalMode)0);
						run.ShockFrame = val5;
						run.Shock = val6;
						run.ShockMat = val4;
					}
				}
				catch (Exception ex2)
				{
					DevLog.Warn("Shadow Step (shock): " + ex2.Message);
					run.ShockFrame = null;
					run.Shock = null;
					run.ShockMat = null;
				}
			}
			run.Reach0 = Mathf.Clamp(ShadowStepHeld.Reach, 0f, 1f);
			run.Clock0 = ShadowStepHeld.Clock;
			run.Dark0 = ShadowStepHeld.Dark;
			run.Spin = ((run.Reach0 > 0.02f) ? ShadowStepHeld.Turn(run.Clock0) : 0f);
			List<HandCards.Entry> list = new List<HandCards.Entry>();
			if (c.Card != null)
			{
				HandCards.Fill(list);
			}
			run.Road = ShadowRoad.Make(c, c.Card, list);
			ShadowRoad road = run.Road;
			run.Arrive = (1f - run.Reach0) * 0.2f;
			run.Charge = ChargeTime() * Mathf.Lerp(0.7f, 1f, run.Reach0);
			run.Round = RoundTime();
			run.HangFor = HangTime();
			run.Rear = RearTime();
			run.Drop = DropTime();
			run.Stop = HoldTime();
			run.After = AfterTime();
			float num = run.Arrive + run.Charge + run.Round + run.HangFor + run.Rear + run.Drop + run.Stop + run.After;
			run.Patience = Mathf.Min(8.5f, Mathf.Max(0.5f, Tuning.F("shadowstep.wait", 1.2f)) + num + TrailLife() + 0.6f + 0.5f * (float)list.Count);
			foreach (HandCards.Entry item in list)
			{
				if (item.Card != null && item.Card != c.Card && FxKit.Alive((GodotObject)(object)item.Node) && ((Node)item.Node).IsInsideTree())
				{
					CardStandIn.Pose(item.Node, out var centre, out var rotation, out var scale);
					if (!(scale.X < 0.05f) && !(scale.Y < 0.05f))
					{
						Take(run, item, centre, rotation, scale, 30f);
					}
				}
			}
			for (int num2 = 0; num2 < run.Cards.Count; num2++)
			{
				Rolled rolled = run.Cards[num2];
				rolled.Lag = 0.028f + 0.085f * FxKit.Hash(num2 * 7 + 3);
				rolled.Side = ((num2 % 2 == 0) ? 1f : (-1f)) * (0.35f + 0.65f * FxKit.Hash(num2 * 5 + 1));
				rolled.Rate = 9f + 8f * FxKit.Hash(num2 * 3 + 2);
			}
			run.Head = road.In(run.Reach0);
			run.WispAt = run.Head;
			run.R = road.ClumpReach(run.Reach0);
			run.Kicked = new bool[c.Enemies?.Count ?? 0];
			if (FxKit.Alive((GodotObject)(object)c.BackParent))
			{
				run.DimRoot = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_ShadowRollDim")
				};
				c.BackParent.AddChild((Node)(object)run.DimRoot, false, (InternalMode)0);
				run.Dim = FxKit.Veil(new Color(0.02f, 0f, 0.05f, 0f));
				((Node)run.DimRoot).AddChild((Node)(object)run.Dim, false, (InternalMode)0);
				if (Roofing() > 0.01f && Tuning.F("shadowstep.roof_dim", 0.78f) > 0.01f)
				{
					run.ShadeBack = MakeShade((Node)(object)run.DimRoot, road.View, out run.ShadeBackQuad);
				}
				run.Back = new ShadowCloud(3.7f, veil: false, aura: true);
				((Node)run.DimRoot).AddChild((Node)(object)run.Back.Node, false, (InternalMode)0);
			}
			if (Roofing() > 0.01f && Tuning.F("shadowstep.roof_dim_who", 0.45f) > 0.01f && FxKit.Alive((GodotObject)(object)c.FrontParent) && c.FrontParent.IsInsideTree())
			{
				try
				{
					run.ShadeRoot = new Node2D
					{
						Name = StringName.op_Implicit("SilentFX_ShadowRoofShade")
					};
					c.FrontParent.AddChild((Node)(object)run.ShadeRoot, false, (InternalMode)0);
					run.ShadeFront = MakeShade((Node)(object)run.ShadeRoot, road.View, out run.ShadeFrontQuad);
					if (run.ShadeFront == null)
					{
						((Node)run.ShadeRoot).QueueFree();
						run.ShadeRoot = null;
					}
				}
				catch (Exception ex3)
				{
					DevLog.Warn("Shadow Step (shade): " + ex3.Message);
					if (FxKit.Alive((GodotObject)(object)run.ShadeRoot))
					{
						((Node)run.ShadeRoot).QueueFree();
					}
					run.ShadeRoot = null;
					run.ShadeFront = null;
					run.ShadeFrontQuad = null;
				}
			}
			Node val7 = ((Tuning.F("shadowstep.over_ui", 1f) >= 0.5f) ? GameRefs.Overlay : null);
			if (!CanHost(val7))
			{
				val7 = val;
			}
			run.TopRoot = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_ShadowRunTop"),
				ZIndex = 8
			};
			val7.AddChild((Node)(object)run.TopRoot, false, (InternalMode)0);
			if (((Node)run.TopRoot).IsInsideTree())
			{
				run.Front = new ShadowCloud(3.7f, veil: false, aura: true);
				((Node)run.TopRoot).AddChild((Node)(object)run.Front.Node, false, (InternalMode)0);
			}
			else
			{
				((Node)run.TopRoot).QueueFree();
				run.TopRoot = null;
			}
			Step(run, 0f);
			if (run.Finished)
			{
				return true;
			}
			Tween val8 = ((Node)val2).CreateTween();
			val8.TweenMethod(Callable.From<float>((Action<float>)((float v) =>
			{
				float dt = v - run.Last;
				run.Last = v;
				try
				{
					Step(run, dt);
				}
				catch (Exception ex4)
				{
					DevLog.Warn("Shadow Step run failed: " + ex4.Message);
					Finish(run);
				}
			})), Variant.op_Implicit(0f), Variant.op_Implicit(9f), 9.0);
			val8.TweenCallback(Callable.From((Action)(() =>
			{
				Finish(run);
			})));
			return true;
		}

		private static float Roofing()
		{
			return Mathf.Clamp(Tuning.F("shadowstep.roof", 1f), 0f, 1f);
		}

		private static ShaderMaterial MakeShade(Node parent, Vector2 view, out ColorRect quad)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			quad = null;
			try
			{
				if (!FxKit.Alive((GodotObject)(object)parent) || !HasShader("shadow_shade"))
				{
					return null;
				}
				ShaderMaterial val = ShaderLib.NewMaterial("shadow_shade");
				if (val == null)
				{
					return null;
				}
				ColorRect val2 = FxKit.Veil(new Color(1f, 1f, 1f, 0f));
				((CanvasItem)val2).Material = (Material)(object)val;
				((CanvasItem)val2).Visible = false;
				val.SetShaderParameter(StringName.op_Implicit("deep"), Variant.op_Implicit(Mathf.Max(200f, view.Y)));
				val.SetShaderParameter(StringName.op_Implicit("soft"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("shadowstep.roof_edge", 260f), 20f, 900f)));
				val.SetShaderParameter(PAmount, Variant.op_Implicit(0f));
				parent.AddChild((Node)(object)val2, false, (InternalMode)0);
				quad = val2;
				return val;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Shadow Step (shade): " + ex.Message);
				quad = null;
				return null;
			}
		}

		private static void Take(Run run, HandCards.Entry entry, Vector2 at, float turn, Vector2 size, float claim)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected Obj, but got Unknown
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			Rolled card = null;
			try
			{
				Node2D val = new Node2D
				{
					Name = StringName.op_Implicit("Card")
				};
				((Node)run.CardLayer).AddChild((Node)(object)val, false, (InternalMode)0);
				card = new Rolled
				{
					Card = entry.Card,
					Real = entry.Node,
					Rig = val,
					At = at,
					Turn = turn,
					Size = size,
					Now = at,
					NowTurn = turn,
					Dark0 = HandShade.ShownOf(entry.Node)
				};
				NCard val2 = CardStandIn.Make(entry.Card, (Node)(object)val);
				if (!FxKit.Alive((GodotObject)(object)val2))
				{
					Drop(card);
					return;
				}
				card.Copy = val2;
				((Control)val2).Position = Vector2.Zero;
				((Control)val2).Rotation = 0f;
				((Control)val2).Scale = Vector2.One;
				Put(card, at, turn, 1f, card.Dark0, 1f);
				CardStandIn.Hide(entry.Node);
				CardFlight.Claim(entry.Card, new CardFlight.Plan
				{
					HideCard = true,
					HideTrail = true,
					Started = (NCard node, Vector2 from, Vector2 to) =>
					{
						Taken(run, card);
					}
				}, claim);
				card.Claimed = true;
				run.Cards.Add(card);
			}
			catch (Exception ex)
			{
				DevLog.Warn("Shadow Step (take a card): " + ex.Message);
				try
				{
					if (card != null)
					{
						Drop(card);
					}
					CardStandIn.Show(entry.Node);
					CardFlight.Release(entry.Card);
					return;
				}
				catch (Exception ex2)
				{
					DevLog.Warn("Shadow Step (take a card, undo): " + ex2.Message);
					return;
				}
			}
			Soften(run, card);
		}

		private static void Soften(Run run, Rolled card)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected Obj, but got Unknown
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (FxKit.Alive((GodotObject)(object)run.SoftLayer))
				{
					ShaderMaterial val = ShaderLib.NewMaterial("shadow_blur");
					if (val != null)
					{
						Node2D val2 = new Node2D
						{
							Name = StringName.op_Implicit("Soft"),
							Visible = false
						};
						((Node)run.SoftLayer).AddChild((Node)(object)val2, false, (InternalMode)0);
						Vector2 val3 = new Vector2(300f, 422f) * 1.36f;
						ColorRect val4 = FxKit.Quad(val3, (Material)(object)val);
						val4.Color = new Color(1f, 1f, 1f, 0f);
						((Control)val4).Position = -val3 * 0.5f;
						((Node)val2).AddChild((Node)(object)val4, false, (InternalMode)0);
						val.SetShaderParameter(StringName.op_Implicit("view"), Variant.op_Implicit(run.Road.View));
						val.SetShaderParameter(PSoft, Variant.op_Implicit(0f));
						val.SetShaderParameter(PSmear, Variant.op_Implicit(Vector2.Zero));
						card.Soft = val2;
						card.SoftMat = val;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Shadow Step (soft card): " + ex.Message);
				card.Soft = null;
				card.SoftMat = null;
			}
		}

		private static void Taken(Run run, Rolled card)
		{
			card.Flown = true;
			run.Taken++;
		}

		private static void Put(Rolled card, Vector2 at, float turn, float size, float dark, float alpha)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			card.Now = at;
			card.NowTurn = turn;
			card.NowSize = size;
			card.NowAlpha = alpha;
			Node2D rig = card.Rig;
			NCard copy = card.Copy;
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
				Vector2 val3 = new Vector2(card.Size.X * size / Mathf.Max(0.001f, val2.X), card.Size.Y * size / Mathf.Max(0.001f, val2.Y));
				rig.GlobalTransform = new Transform2D(turn, val3, 0f, at);
				float num = Mathf.Clamp(dark, 0f, 1f);
				((CanvasItem)rig).Modulate = new Color(Mathf.Lerp(1f, Shade.Ink.R, num), Mathf.Lerp(1f, Shade.Ink.G, num), Mathf.Lerp(1f, Shade.Ink.B, num), Mathf.Clamp(alpha, 0f, 1f));
			}
		}

		private static void Drop(Rolled card)
		{
			NCard copy = card.Copy;
			Node2D rig = card.Rig;
			Node2D soft = card.Soft;
			card.Copy = null;
			card.Rig = null;
			card.Soft = null;
			card.SoftMat = null;
			if (copy != null)
			{
				CardStandIn.Free(copy);
			}
			try
			{
				if (FxKit.Alive((GodotObject)(object)soft))
				{
					((Node)soft).QueueFree();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Shadow Step (soft card, free): " + ex.Message);
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
				DevLog.Warn("Shadow Step (copy): " + ex2.Message);
			}
		}

		private static void Step(Run run, float dt)
		{
			if (run.Finished || !FxKit.Alive((GodotObject)(object)run.Root))
			{
				return;
			}
			run.Clock += dt;
			KeepUnseen(run);
			if (!run.Cleared)
			{
				Move(run, dt);
			}
			if (run.Finished)
			{
				return;
			}
			Gulp(run);
			if (run.Cleared)
			{
				int num = TakenState(run);
				run.DoneFor = ((num == 0) ? 0f : (run.DoneFor + dt));
				if (num == 2 || (num == 1 && run.DoneFor > 0.35f))
				{
					Finish(run);
					return;
				}
			}
			if (run.Clock > run.Patience)
			{
				Finish(run);
			}
		}

		private static void KeepUnseen(Run run)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < run.Cards.Count; i++)
			{
				Rolled rolled = run.Cards[i];
				if (!rolled.Flown && !rolled.Gone)
				{
					NCard real = rolled.Real;
					if (!FxKit.Alive((GodotObject)(object)real) || !((Node)real).IsInsideTree() || real.Model != rolled.Card)
					{
						rolled.Gone = true;
					}
					else if (((CanvasItem)real).Modulate.A > 0.01f)
					{
						((CanvasItem)real).Modulate = new Color(1f, 1f, 1f, 0f);
					}
				}
			}
		}

		private static int TakenState(Run run)
		{
			int result = 2;
			for (int i = 0; i < run.Cards.Count; i++)
			{
				Rolled rolled = run.Cards[i];
				if (!rolled.Flown)
				{
					if (!rolled.Gone)
					{
						return 0;
					}
					result = 1;
				}
			}
			return result;
		}

		private static void Gulp(Run run)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			if (run.Taken != run.TakenSeen)
			{
				run.TakenSeen = run.Taken;
				if (run.Slammed && !(Tuning.F("shadowstep.gulp", 1f) < 0.5f) && FxKit.Alive((GodotObject)(object)run.Root))
				{
					Shade.Puff((Node)(object)run.Root, run.Road.Pile, run.Road.Tall * 0.3f, 1.6f, 0.26f, 0.5f);
				}
			}
		}

		private static void Layers(ShadowRoad road, float u, float fall, out float back, out float hand, out float front)
		{
			back = 0f;
			hand = 1f;
			front = 0f;
			float num = (u - Mathf.Lerp(road.StopTime(3), road.StopTime(4), 0.55f)) / 0.05f + 0.5f;
			if (num > 0f)
			{
				back = Smooth(num * 2f);
				hand = 1f - Smooth(num * 2f - 1f);
				front = 0f;
			}
			num = (u - Mathf.Lerp(road.StopTime(6), road.StopTime(7), 0.5f)) / 0.05f + 0.5f;
			if (num > 0f)
			{
				back = 1f - Smooth(num * 2f - 1f);
				hand = 0f;
				front = Smooth(num * 2f);
			}
			num = (fall - 0.22f) / 0.5f;
			if (num > 0f)
			{
				back = Smooth(num * 2f);
				hand = 0f;
				front = 1f - Smooth(num * 2f - 1f);
			}
		}

		private static Past Was(Run run, float t)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			List<Past> way = run.Way;
			int count = way.Count;
			if (count == 0)
			{
				Look now = run.Now;
				return new Past
				{
					T = t,
					At = run.Head,
					R = run.R,
					Back = now.Back,
					Hand = now.Hand,
					Front = now.Front,
					Far = run.Far
				};
			}
			if (t >= way[count - 1].T)
			{
				return way[count - 1];
			}
			if (t <= way[0].T)
			{
				return way[0];
			}
			int num = 0;
			int num2 = count - 1;
			while (num2 - num > 1)
			{
				int num3 = (num + num2) / 2;
				if (way[num3].T <= t)
				{
					num = num3;
				}
				else
				{
					num2 = num3;
				}
			}
			Past past = way[num];
			Past past2 = way[num2];
			float num4 = Mathf.Clamp((t - past.T) / Mathf.Max(1E-05f, past2.T - past.T), 0f, 1f);
			return new Past
			{
				T = t,
				At = past.At.Lerp(past2.At, num4),
				R = Mathf.Lerp(past.R, past2.R, num4),
				Back = Mathf.Lerp(past.Back, past2.Back, num4),
				Hand = Mathf.Lerp(past.Hand, past2.Hand, num4),
				Front = Mathf.Lerp(past.Front, past2.Front, num4),
				Far = Mathf.Lerp(past.Far, past2.Far, num4)
			};
		}

		private static void Move(Run run, float dt)
		{
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_040c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0411: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0875: Unknown result type (might be due to invalid IL or missing references)
			//IL_087a: Unknown result type (might be due to invalid IL or missing references)
			//IL_087d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0882: Unknown result type (might be due to invalid IL or missing references)
			//IL_049c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04be: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0896: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_061a: Unknown result type (might be due to invalid IL or missing references)
			//IL_061f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0624: Unknown result type (might be due to invalid IL or missing references)
			//IL_0641: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_057a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0805: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Unknown result type (might be due to invalid IL or missing references)
			//IL_0818: Unknown result type (might be due to invalid IL or missing references)
			//IL_069a: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06be: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_076e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0772: Unknown result type (might be due to invalid IL or missing references)
			//IL_0777: Unknown result type (might be due to invalid IL or missing references)
			//IL_079e: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ab9: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ada: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bbe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bf0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cc9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b33: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b3b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b42: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b54: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d56: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d5f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d64: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d68: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dcd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dcf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dd8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dda: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f59: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e75: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e65: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ea9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f96: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f9e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fa5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fcf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fd4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fe5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fef: Unknown result type (might be due to invalid IL or missing references)
			//IL_1030: Unknown result type (might be due to invalid IL or missing references)
			//IL_103e: Unknown result type (might be due to invalid IL or missing references)
			//IL_1043: Unknown result type (might be due to invalid IL or missing references)
			//IL_104d: Unknown result type (might be due to invalid IL or missing references)
			//IL_1052: Unknown result type (might be due to invalid IL or missing references)
			//IL_1061: Unknown result type (might be due to invalid IL or missing references)
			//IL_1066: Unknown result type (might be due to invalid IL or missing references)
			//IL_106b: Unknown result type (might be due to invalid IL or missing references)
			//IL_1086: Unknown result type (might be due to invalid IL or missing references)
			//IL_1088: Unknown result type (might be due to invalid IL or missing references)
			//IL_1122: Unknown result type (might be due to invalid IL or missing references)
			//IL_1124: Unknown result type (might be due to invalid IL or missing references)
			//IL_1126: Unknown result type (might be due to invalid IL or missing references)
			//IL_138a: Unknown result type (might be due to invalid IL or missing references)
			//IL_140e: Unknown result type (might be due to invalid IL or missing references)
			//IL_143f: Unknown result type (might be due to invalid IL or missing references)
			//IL_1444: Unknown result type (might be due to invalid IL or missing references)
			//IL_1449: Unknown result type (might be due to invalid IL or missing references)
			//IL_1451: Unknown result type (might be due to invalid IL or missing references)
			//IL_14be: Unknown result type (might be due to invalid IL or missing references)
			//IL_14fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_1500: Unknown result type (might be due to invalid IL or missing references)
			//IL_1502: Unknown result type (might be due to invalid IL or missing references)
			//IL_1509: Unknown result type (might be due to invalid IL or missing references)
			//IL_1518: Unknown result type (might be due to invalid IL or missing references)
			//IL_151f: Unknown result type (might be due to invalid IL or missing references)
			//IL_156a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1576: Unknown result type (might be due to invalid IL or missing references)
			//IL_157b: Unknown result type (might be due to invalid IL or missing references)
			//IL_15b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_15c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_15ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_15d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_15dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_15f6: Unknown result type (might be due to invalid IL or missing references)
			ShadowRoad road = run.Road;
			CardFxContext c = run.C;
			Look now = run.Now;
			float clock = run.Clock;
			float num = dt;
			if (run.Hold > 0f)
			{
				run.Hold -= dt;
				num = 0f;
			}
			run.Anim += num;
			float arrive = run.Arrive;
			float num2 = arrive + run.Charge;
			float num3 = num2 + run.Round;
			float num4 = num3 + run.HangFor;
			float num5 = num4 + run.Rear;
			float num6 = num5 + run.Drop;
			if (!run.StrikeHeard && run.HitAt < 0f && run.Anim >= num6 - Sfx.Lead("shadow_strike"))
			{
				run.StrikeHeard = true;
				Sfx.Play("shadow_strike");
			}
			if (run.HitAt < 0f && run.Anim >= num6)
			{
				run.Anim = num6;
				run.HitAt = num6;
				run.Hold = run.Stop;
				Hit(run);
			}
			if (run.HitAt >= 0f && !run.Landed && run.Hold <= 0f)
			{
				Land(run);
			}
			float anim = run.Anim;
			float num7 = Mathf.Clamp((anim - num2) / run.Round, 0f, 1f);
			float num8 = road.Along(num7);
			Vector2 head = run.Head;
			int count = run.Cards.Count;
			float num9 = ((count == 0) ? 1f : ((float)run.Caught / (float)count));
			float dir = road.Dir;
			Vector2 view = road.View;
			float hangReach = road.HangReach;
			float landReach = road.LandReach;
			Vector2 val = road.Hang + new Vector2((0f - dir) * 60f, 0f);
			Vector2 val2 = val + new Vector2(dir * 14f, (0f - view.Y) * 0.045f);
			Vector2 val3 = road.Feet + new Vector2(0f, -6f);
			Vector2 val4 = val3 - val2;
			val4 = ((val4.LengthSquared() > 1f) ? val4.Normalized() : Vector2.Down);
			float num10 = 1f;
			float lobe = 1f;
			float num11 = 1f;
			float num12 = 0f;
			float num13 = 0f;
			Vector2 heading = run.Heading;
			float num14 = 0f;
			float num15 = 0f;
			float reaching = 0f;
			float ragged = 0f;
			bool flag = false;
			Vector2 val6;
			if (anim < arrive)
			{
				float num16 = anim / Mathf.Max(0.001f, arrive);
				num10 = Mathf.Lerp(run.Reach0, 1f, num16 * num16);
				run.Head = road.In(num10);
				run.R = Mathf.Lerp(road.ClumpReach(run.Reach0), road.Reach(0f) * 0.82f, num16);
				Vector2 val5 = road.In(Mathf.Min(1f, num10 + 0.02f));
				if (val5.DistanceSquaredTo(run.Head) > 0.01f)
				{
					val6 = val5 - run.Head;
					run.Heading = val6.Normalized();
				}
				heading = run.Heading;
				lobe = num16;
			}
			else if (anim < num2)
			{
				float num17 = (anim - arrive) / Mathf.Max(0.001f, num2 - arrive);
				float num18 = Smooth(num17 / 0.65f);
				float num19 = num17 * num17;
				run.Head = road.Start + new Vector2((0f - dir) * 58f * num18 + 2.5f * num19 * Mathf.Sin(anim * 83f), 12f * num18 + 2f * num19 * Mathf.Cos(anim * 71f));
				run.R = road.Reach(0f) * Mathf.Lerp(0.82f, 0.74f, num18);
				run.Heading = new Vector2(dir, 0f);
				heading = new Vector2(0f, 1f);
				num13 = -0.22f * Smooth((num17 - 0.35f) / 0.65f);
				num12 = 0.12f * num17 * num17;
				lobe = ((run.Reach0 < 0.97f) ? (1f - num18) : 0f);
				num15 = Smooth(num17 / 0.85f);
			}
			else if (anim < num3)
			{
				if (!run.Launched)
				{
					Launch(run);
				}
				run.Head = road.Loop(num8);
				Vector2 val7 = road.Loop(Mathf.Min(1f, num8 + 0.004f));
				if (val7.DistanceSquaredTo(run.Head) > 0.01f)
				{
					val6 = val7 - run.Head;
					run.Heading = val6.Normalized();
				}
				float num20 = road.Reach(num8);
				float num21 = 1f - Smooth((num8 - road.StopAlong(3)) / Mathf.Max(0.001f, road.StopAlong(4) - road.StopAlong(3)) * 2f);
				num20 *= Mathf.Lerp(1f, Mathf.Lerp(Mathf.Clamp(Tuning.F("shadowstep.gather", 0.85f), 0.3f, 1f), 1f, num9), num21);
				run.R = num20;
				heading = run.Heading;
				lobe = Smooth((anim - num2) / 0.08f);
				num13 = 0.3f * (1f - Smooth((anim - num2) / 0.14f));
			}
			else if (anim < num4)
			{
				if (!run.Launched)
				{
					Launch(run);
				}
				float num22 = (anim - num3) / Mathf.Max(0.001f, num4 - num3);
				run.Head = road.Hang + new Vector2((0f - dir) * 60f * (1f - (1f - num22) * (1f - num22)), 5f * Mathf.Sin((float)Math.PI * num22));
				run.R = hangReach * (1f + 0.04f * num22);
				heading = run.Heading;
				reaching = 0.6f * Smooth(num22);
			}
			else if (anim < num5)
			{
				if (!run.Launched)
				{
					Launch(run);
				}
				float num23 = (anim - num4) / Mathf.Max(0.001f, num5 - num4);
				float num24 = Smooth(num23 / 0.8f);
				run.Head = val + new Vector2(dir * 14f, (0f - view.Y) * 0.045f) * num24;
				run.R = hangReach * 1.04f * (1f + 0.08f * num24);
				run.Heading = val4;
				heading = new Vector2(0f - val4.Y, val4.X);
				num13 = -0.28f * num24;
				num12 = 0.1f * num23 * num23;
				reaching = 0.6f + 0.4f * num23;
			}
			else if (anim < num6)
			{
				if (!run.Launched)
				{
					Launch(run);
				}
				float num25 = (anim - num5) / Mathf.Max(0.001f, num6 - num5);
				num14 = Mathf.Pow(num25, 2.2f);
				run.Head = val2.Lerp(val3, num14);
				run.R = Mathf.Lerp(hangReach * 1.12f, landReach, Mathf.Pow(num25, 1.4f));
				run.Heading = val4;
				heading = val4;
				num13 = 1.3f * Smooth(num25 / 0.5f);
				reaching = 1f - Smooth(num25 / 0.35f);
				num12 = 0.04f;
			}
			else
			{
				float num26 = anim - num6;
				float num27 = Smooth(num26 / 0.14f);
				num14 = 1f;
				run.Head = val3;
				run.Heading = val4;
				heading = new Vector2(1f, 0f);
				num13 = Mathf.Lerp(0.7f, 1.6f, num27);
				run.R = landReach * Mathf.Lerp(0.85f, 0.7f, num27);
				num11 = 1f - num27;
				num12 = 0.1f * Mathf.Exp((0f - num26) * 9f);
				ragged = 0.18f;
				flag = true;
			}
			Vector2 head2 = run.Head;
			Vector2 heading2 = run.Heading;
			float num28 = ((num > 0f) ? head2.DistanceTo(head) : 0f);
			if (num > 0f)
			{
				run.Speed = Mathf.Lerp(run.Speed, num28 / num, 1f - Mathf.Exp((0f - num) * 32f));
			}
			run.Spin += dir * num28 / Mathf.Max(40f, run.R * 1.25f) * Mathf.Clamp(Tuning.F("shadowstep.spin", 1f), 0f, 3f);
			run.Far += num28;
			Layers(road, num7, num14, out var back, out var hand, out var front);
			bool flag2 = run.Back != null && run.Back.Ok;
			if (!flag2)
			{
				hand = Mathf.Max(hand, back);
				back = 0f;
			}
			if (run.Front == null || !run.Front.Ok)
			{
				hand = Mathf.Max(hand, front);
				front = 0f;
			}
			int layer = 1;
			if (back >= hand && back >= front)
			{
				layer = 0;
			}
			else if (front >= hand)
			{
				layer = 2;
			}
			if (num > 0f)
			{
				run.Way.Add(new Past
				{
					T = anim,
					At = head2,
					R = run.R,
					Back = back,
					Hand = hand,
					Front = front,
					Far = run.Far
				});
				int i;
				for (i = 0; i < run.Way.Count - 2 && run.Way[i + 1].T < anim - 1f; i++)
				{
				}
				if (i > 64)
				{
					run.Way.RemoveRange(0, i);
				}
			}
			float num29 = Smooth((run.Speed - 1200f) / 3600f);
			float num30 = 1f + Mathf.Clamp(Tuning.F("shadowstep.stretch", 1f), 0f, 2f) * 0.75f * num29 + num13;
			float bodyR = run.R;
			Vector2 drawn = Vector2.Zero;
			float combed = 0f;
			if (num30 > 1.02f)
			{
				drawn = heading * (num30 - 1f);
				bodyR = run.R * Mathf.Pow(num30, -0.25f);
				if (!flag)
				{
					combed = Mathf.Clamp(Mathf.Max(num29, Smooth(num13)), 0f, 1f);
				}
			}
			else if (num30 < 0.98f)
			{
				float num31 = 1f / Mathf.Max(0.4f, num30);
				drawn = new Vector2(0f - heading.Y, heading.X) * (num31 - 1f);
				bodyR = run.R * Mathf.Pow(num31, -0.6f);
			}
			now.T = anim;
			now.Back = back;
			now.Hand = hand;
			now.Front = front;
			now.Fade = num11;
			now.Lobe = lobe;
			now.Ragged = ragged;
			now.LongR = num30;
			now.BodyR = bodyR;
			now.Drawn = drawn;
			now.Axis = heading;
			now.Combed = combed;
			now.Mark = run.Far * 0.5f;
			now.Charging = num15;
			now.Reaching = reaching;
			now.Down = val4;
			now.Pulse = Mathf.Clamp(Mathf.Clamp(Tuning.F("shadowstep.speed_glow", 0.08f), 0f, 1f) * Smooth((run.Speed - 1500f) / 4500f) + num12, 0f, 1f);
			now.Fast = num29;
			now.Reach = num10;
			now.DrawnIn = Smooth((anim - arrive) / Mathf.Max(0.05f, (num2 - arrive) * 0.8f));
			now.Mist = run.Clock0 + clock;
			float num32 = Mathf.Clamp(Tuning.F("shadowstep.tail", 0.26f), 0f, 0.6f);
			float num33 = Mathf.Clamp(Tuning.F("shadowstep.tail_wide", 1f), 0.3f, 2f);
			Vector2 val8 = head2;
			for (int j = 0; j < 12; j++)
			{
				ref TailPuff reference = ref run.Tail[j];
				reference.Seen = 0f;
				if (!(num32 <= 0.01f) && !(anim < arrive))
				{
					float num34 = ((float)j + 1f) / 12f;
					Past past = Was(run, anim - num32 * Mathf.Pow(num34, 1.1f));
					Vector2 at = Was(run, anim - num32 * Mathf.Pow(Mathf.Min(1f, num34 + 1f / 12f), 1.1f)).At;
					Vector2 at2 = past.At;
					float num35 = at2.DistanceTo(val8);
					float num36 = at2.DistanceTo(head2);
					float num37 = past.R * Mathf.Lerp(0.78f, 0.2f, Mathf.Pow(num34, 0.9f)) * num33;
					float num38 = Smooth((num36 - 0.25f * run.R) / Mathf.Max(1f, 0.5f * run.R));
					Vector2 val9 = val8 - at;
					val8 = at2;
					if (!(num38 <= 0.01f))
					{
						reference.At = at2;
						reference.R = num37;
						reference.Thick = Mathf.Lerp(0.92f, 0.6f, num34);
						reference.Age = Mathf.Lerp(0.12f, 0.55f, num34);
						reference.Seen = num38 * Mathf.Pow(1f - num34, 0.55f) * (0.4f + 0.6f * num11);
						reference.Drawn = ((val9.LengthSquared() > 1f) ? (val9.Normalized() * Mathf.Clamp(num35 / Mathf.Max(1f, 0.55f * num37) - 1f, 0.02f, 6f)) : (heading2 * 0.02f));
						reference.Back = past.Back;
						reference.Hand = past.Hand;
						reference.Front = past.Front;
						reference.Far = past.Far;
					}
				}
			}
			float life = TrailLife();
			float num39 = Mathf.Max(90f, Mathf.Clamp(Tuning.F("shadowstep.linger_gap", 0.55f), 0.2f, 2f) * run.R);
			if (anim >= arrive && run.HitAt < 0f && run.Speed > 300f && head2.DistanceTo(run.WispAt) >= num39 && run.Roof.Count == 0)
			{
				float num40 = Mathf.Sin((float)run.WispCount * 2.399f + 0.7f);
				Vector2 val10 = new Vector2(0f - heading2.Y, heading2.X);
				run.Wisps.Add(new Wisp
				{
					Pos = head2 - heading2 * (run.R * 0.3f) + val10 * (num40 * run.R * 0.2f),
					R = run.R * 0.55f,
					Born = anim,
					Life = life,
					Grow = 0.6f,
					Layer = layer,
					Vel = heading2 * (run.Speed * 0.05f) + val10 * (num40 * 22f) + new Vector2(0f, -18f)
				});
				run.WispCount++;
				run.WispAt = head2;
			}
			for (int num41 = run.Wisps.Count - 1; num41 >= 0; num41--)
			{
				if (anim - run.Wisps[num41].Born >= run.Wisps[num41].Life)
				{
					run.Wisps.RemoveAt(num41);
				}
			}
			while (run.Wisps.Count > 9)
			{
				run.Wisps.RemoveAt(0);
			}
			Cards(run, anim, num, num7, num8, num15);
			if (run.Finished)
			{
				return;
			}
			RoofStep(run, anim, num2, num3, num5, num8, num14, val2, val3, val4, back, hand, front);
			bool flag3 = false;
			for (int k = 0; k < run.Cards.Count; k++)
			{
				if (flag3)
				{
					break;
				}
				flag3 = run.Cards[k].Copy != null;
			}
			float aura = Mathf.Clamp(Tuning.F("shadowstep.aura", 0.16f), 0f, 1f);
			int floor = ((!flag2) ? 1 : 0);
			Fill(run, run.Back, 0, floor, aura);
			Fill(run, run.Body, 1, floor, aura);
			if (flag3)
			{
				run.CarriedAt = clock;
			}
			float num42 = ((run.CarriedAt < 0f) ? 0f : (1f - Smooth((clock - run.CarriedAt) / 0.3f)));
			if (num42 > 0.004f && run.Veil != null)
			{
				run.Veil.Thin(Mathf.Lerp(1f, 0.45f, num29));
				Fill(run, run.Veil, 1, -1, 0f, num42);
			}
			else
			{
				run.Veil?.Hide();
			}
			Fill(run, run.Front, 2, floor, aura);
			if (FxKit.Alive((GodotObject)(object)run.Dim))
			{
				float num43 = Smooth((num7 - road.StopTime(6)) / Mathf.Max(0.001f, 1f - road.StopTime(6)));
				float num44 = Mathf.Lerp(run.Dark0, RollDim() * Mathf.Lerp(1f, Mathf.Clamp(Tuning.F("shadowstep.hang_dim", 1.6f), 1f, 2.5f), num43), Smooth(clock / 0.35f));
				if (run.ShadeBack != null)
				{
					num44 *= Mathf.Lerp(1f, Mathf.Clamp(Tuning.F("shadowstep.roof_lit", 0.6f), 0.2f, 1f), run.Roofed);
				}
				num44 += 0.07f * Mathf.Sin((float)Math.PI * num15);
				if (run.HitAt >= 0f)
				{
					num44 *= 1f - Smooth((anim - run.HitAt) / 0.3f);
				}
				run.Dim.Color = new Color(0.02f, 0f, 0.05f, Mathf.Clamp(num44, 0f, 0.95f));
			}
			if (run.Neared < 3 && Tuning.F("shadowstep.near", 1f) >= 0.5f && FxKit.Alive((GodotObject)(object)run.TopRoot))
			{
				float num45 = Mathf.Lerp(road.StopTime(7), 1f, 0.15f + 0.3f * (float)run.Neared);
				if (num7 >= num45)
				{
					int num46 = run.Neared++;
					Vector2 at3 = head2 + new Vector2(dir * (0.55f - 0.5f * (float)num46) * run.R, (0.5f + 0.12f * (float)num46) * run.R);
					Shade.Puff((Node)(object)run.TopRoot, at3, run.R * 0.55f, 2.7f, 0.5f, 0.55f, 0.7f);
				}
			}
			if (anim >= num2 && anim < num3 && num7 > road.StopTime(3) && run.Kicked != null && c.Enemies != null && FxKit.Alive((GodotObject)(object)c.BackParent))
			{
				float num47 = view.X * 0.3f;
				for (int l = 0; l < run.Kicked.Length && l < c.Enemies.Count; l++)
				{
					if (run.Kicked[l])
					{
						continue;
					}
					FxTarget fxTarget = c.Enemies[l];
					if (fxTarget != null)
					{
						Vector2 feet = fxTarget.Feet;
						float num48 = Mathf.Abs(head2.X - feet.X);
						if (!(head2.Y >= feet.Y - 40f) && !(num48 >= num47))
						{
							run.Kicked[l] = true;
							float num49 = 1f - num48 / num47;
							float num50 = Mathf.Max(fxTarget.Width, 120f);
							Shade.Puff(c.BackParent, feet + new Vector2(0f, -20f), num50 * (0.7f + 0.5f * num49), 1.9f, 0.42f, 0.45f + 0.4f * num49, 0.5f);
							Shade.Motes(c.BackParent, feet + new Vector2(0f, -10f), 4, 260f, new Vector2((head2.X >= feet.X) ? 0.4f : (-0.4f), -1f), 40f, num50 * 0.25f, 0.4);
						}
					}
				}
			}
			ShockStep(run);
			if (run.HitAt >= 0f && run.Landed && anim - run.HitAt > run.After && run.Wisps.Count == 0 && run.Roof.Count == 0 && run.ShockAt < 0f)
			{
				run.Cleared = true;
				Clear(run);
			}
		}

		private static void RoofStep(Run run, float t, float tl, float th, float td, float s, float fall, Vector2 top, Vector2 ground, Vector2 down, float wBack, float wHand, float wFront)
		{
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0449: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0604: Unknown result type (might be due to invalid IL or missing references)
			//IL_0607: Unknown result type (might be due to invalid IL or missing references)
			//IL_0616: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_0665: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Unknown result type (might be due to invalid IL or missing references)
			//IL_079d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Unknown result type (might be due to invalid IL or missing references)
			//IL_080c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0813: Unknown result type (might be due to invalid IL or missing references)
			//IL_0818: Unknown result type (might be due to invalid IL or missing references)
			ShadowRoad road = run.Road;
			Look now = run.Now;
			run.RoofNow.Clear();
			float num = Roofing();
			float num2 = float.MaxValue;
			float num3 = float.MinValue;
			if (num <= 0.01f)
			{
				run.Roof.Clear();
			}
			else
			{
				float total = road.Total;
				if (t >= tl && t < th && run.HitAt < 0f)
				{
					float num4 = s * total;
					if (run.RoofAt < 0f)
					{
						run.RoofAt = road.StopAlong(4) * total;
					}
					float num5 = Mathf.Clamp(Tuning.F("shadowstep.roof_gap", 0.4f), 0.2f, 1.5f);
					while (num4 >= run.RoofAt && run.Roof.Count < 24)
					{
						float num6 = road.Reach(run.RoofAt / total);
						run.Roof.Add(new RoofPiece
						{
							Along = run.RoofAt,
							R = num6,
							Born = t,
							Back = wBack,
							Hand = wHand,
							Front = wFront,
							N = run.RoofCount
						});
						run.RoofCount++;
						run.RoofAt += Mathf.Max(Mathf.Max(60f, total * 0.6f / 24f), num5 * num6 * (0.8f + 0.4f * FxKit.Hash(run.RoofCount * 5 + 1)));
					}
				}
				if (run.HitAt >= 0f && t - run.HitAt > 1.5f)
				{
					run.Roof.Clear();
				}
				if (run.Roof.Count > 0)
				{
					float num7 = Mathf.Max(1f, top.DistanceTo(ground));
					float num8 = 0f;
					if (t >= td)
					{
						num8 = ((run.HitAt < 0f) ? (fall * num7) : (num7 + Mathf.Clamp(Tuning.F("shadowstep.roof_pull", 5200f), 1500f, 20000f) * (t - run.HitAt)));
					}
					float num9 = Smooth(num8 / 160f);
					float num10 = 1f - num9;
					float num11 = Mathf.Clamp(Tuning.F("shadowstep.roof_wide", 0.5f), 0.2f, 1.2f);
					float num12 = Mathf.Clamp(Tuning.F("shadowstep.roof_swell", 0.3f), 0f, 1f);
					float num13 = Mathf.Clamp(Tuning.F("shadowstep.roof_sink", 0.1f), 0f, 0.5f);
					float num14 = Mathf.Clamp(Tuning.F("shadowstep.roof_hang", 1f), 0f, 2f);
					bool flag = run.Back != null && run.Back.Ok;
					bool flag2 = run.Front != null && run.Front.Ok;
					for (int num15 = run.Roof.Count - 1; num15 >= 0; num15--)
					{
						RoofPiece roofPiece = run.Roof[num15];
						float num16 = roofPiece.Along + num8;
						float num17 = num16 - total;
						if (num17 >= num7)
						{
							run.Roof.RemoveAt(num15);
						}
						else
						{
							float num18 = Mathf.Max(0f, t - roofPiece.Born);
							float num19 = 1f - Mathf.Exp(-1.8f * num18);
							int n = roofPiece.N;
							float num20 = n switch
							{
								2 => 0.85f, 
								1 => 0.65f, 
								0 => 0.45f, 
								_ => 1f, 
							};
							float num21 = 0.8f + 0.45f * FxKit.Hash(n * 3 + 5);
							float num22 = roofPiece.R * num11 * num20 * num21 * (1f + num12 * num19) * Mathf.Lerp(1f, 0.85f, num9);
							float back = roofPiece.Back;
							float hand = roofPiece.Hand;
							float front = roofPiece.Front;
							float num23 = Mathf.Min(1f, num18 * 7f + 0.15f) * num;
							float num24 = 1f;
							bool flag3 = num17 <= 0f;
							Vector2 val5;
							Vector2 val;
							if (flag3)
							{
								float num25 = num16 / total;
								val = road.Loop(num25);
								Vector2 val2 = road.Loop(Mathf.Min(1f, num25 + 0.004f));
								Vector2 val3;
								if (!(val2.DistanceSquaredTo(val) > 0.01f))
								{
									val3 = new Vector2(0f - road.Dir, 0f);
								}
								else
								{
									Vector2 val4 = val2 - val;
									val3 = val4.Normalized();
								}
								val5 = val3;
								if (num8 > 0f)
								{
									Layers(road, road.TimeAt(num25), 0f, out back, out hand, out front);
								}
							}
							else
							{
								float num26 = num17 / num7;
								val = road.Hang.Lerp(top, Smooth(num17 / 110f)) + (ground - top) * num26;
								val5 = down;
								float num27 = (num26 - 0.22f) / 0.5f;
								back = Smooth(num27 * 2f);
								hand = 0f;
								front = 1f - Smooth(num27 * 2f - 1f);
								num23 *= 1f - Smooth((num26 - 0.8f) / 0.2f);
								num24 = 1f - Smooth(num17 / 260f);
							}
							if (!flag)
							{
								hand = Mathf.Max(hand, back);
								back = 0f;
							}
							if (!flag2)
							{
								hand = Mathf.Max(hand, front);
								front = 0f;
							}
							float num28 = 9f * Mathf.Sin(now.Mist * 1.6f + (float)n * 1.7f);
							val += new Vector2(0f, num13 * roofPiece.R * num19 * (0.5f + 1.1f * FxKit.Hash(n * 7 + 2)) * num24 + num28 * num10);
							num2 = Mathf.Min(num2, val.X);
							num3 = Mathf.Max(num3, val.X);
							float num29 = Mathf.Lerp(1.25f, 2f, num9);
							float num30 = total - roofPiece.Along - Mathf.Max(0f, num8 - num7);
							run.RoofNow.Add(new RoofPuff
							{
								At = val,
								R = num22,
								Thick = Mathf.Lerp(0.95f, 0.84f, num19),
								Age = Mathf.Lerp(Mathf.Lerp(0.06f, 0.32f, num19), 0.16f, num9),
								Seen = num23,
								Drawn = val5 * (num29 - 1f),
								Way = new Vector2(run.Far - num30 - now.Mark, 0f),
								Combed = 0.3f * num9,
								Back = back,
								Hand = hand,
								Front = front
							});
							if (flag3 && n % 3 == 1 && num14 > 0.01f && num10 > 0.02f)
							{
								float num31 = (1f - Mathf.Exp(-1.3f * num18)) * num14;
								run.RoofNow.Add(new RoofPuff
								{
									At = val + new Vector2(14f * Mathf.Sin(now.Mist * 1.1f + (float)n), num22 * (0.42f + 0.3f * num31)),
									R = num22 * 0.4f,
									Thick = 0.8f,
									Age = 0.42f,
									Seen = num23 * num10 * Mathf.Min(1f, num31 * 3f),
									Drawn = new Vector2(0f, 0.4f + 0.7f * num31),
									Way = Vector2.Zero,
									Combed = 0f,
									Back = back,
									Hand = hand,
									Front = front
								});
							}
						}
					}
				}
			}
			float num32 = 0f;
			if (num3 >= num2)
			{
				run.ShadeLo = Mathf.Min(num2, run.Head.X - run.R * 0.6f);
				run.ShadeHi = Mathf.Max(num3, run.Head.X + run.R * 0.6f);
				num32 = Smooth((run.ShadeHi - run.ShadeLo) / 500f) * num;
			}
			else if (run.HitAt >= 0f && run.RoofCount > 0 && num > 0.01f)
			{
				num32 = Smooth((run.ShadeHi - run.ShadeLo) / 500f) * num;
			}
			if (run.HitAt >= 0f)
			{
				num32 *= 1f - Smooth((t - run.HitAt) / 0.3f);
			}
			run.Roofed = num32;
			float lean = road.Dir * Mathf.Clamp(Tuning.F("shadowstep.roof_lean", 160f), 0f, 600f) * Smooth(run.Speed / 1500f);
			LayShade(run.ShadeBackQuad, run.ShadeBack, run.ShadeLo, run.ShadeHi, num32 * Mathf.Clamp(Tuning.F("shadowstep.roof_dim", 0.78f), 0f, 0.95f), lean);
			LayShade(run.ShadeFrontQuad, run.ShadeFront, run.ShadeLo, run.ShadeHi, num32 * Mathf.Clamp(Tuning.F("shadowstep.roof_dim_who", 0.45f), 0f, 0.9f), lean);
		}

		private static void LayShade(ColorRect quad, ShaderMaterial mat, float lo, float hi, float amount, float lean)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			if (mat != null && FxKit.Alive((GodotObject)(object)mat) && FxKit.Alive((GodotObject)(object)quad))
			{
				if (amount <= 0.004f || !float.IsFinite(lo) || !float.IsFinite(hi))
				{
					((CanvasItem)quad).Visible = false;
					return;
				}
				((CanvasItem)quad).Visible = true;
				mat.SetShaderParameter(PFromX, Variant.op_Implicit(lo));
				mat.SetShaderParameter(PToX, Variant.op_Implicit(hi));
				mat.SetShaderParameter(PLean, Variant.op_Implicit(lean));
				mat.SetShaderParameter(PAmount, Variant.op_Implicit(Mathf.Clamp(amount, 0f, 1f)));
			}
		}

		private static void Fill(Run run, ShadowCloud cloud, int layer, int floor, float aura, float intensity = 1f)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_051d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0522: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_039d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_0592: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0781: Unknown result type (might be due to invalid IL or missing references)
			//IL_0788: Unknown result type (might be due to invalid IL or missing references)
			//IL_0795: Unknown result type (might be due to invalid IL or missing references)
			//IL_079a: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_0679: Unknown result type (might be due to invalid IL or missing references)
			//IL_067e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0683: Unknown result type (might be due to invalid IL or missing references)
			//IL_0688: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0694: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0877: Unknown result type (might be due to invalid IL or missing references)
			//IL_089d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
			if (cloud == null || !cloud.Ok)
			{
				return;
			}
			ShadowRoad road = run.Road;
			Look now = run.Now;
			float t = now.T;
			Vector2 head = run.Head;
			Vector2 heading = run.Heading;
			float r = run.R;
			float num = layer switch
			{
				1 => now.Hand, 
				0 => now.Back, 
				_ => now.Front, 
			} * now.Fade;
			cloud.Begin();
			if (num > 0.004f)
			{
				float num2 = 0.35f + 0.65f * now.Fade;
				Vector2 val = ((now.Drawn.LengthSquared() > 1E-08f) ? now.Drawn.Normalized() : heading);
				Vector2 val2 = new Vector2(0f - val.Y, val.X);
				float num3 = run.Far - now.Mark;
				cloud.Add(head, now.BodyR * 0.86f, num2, now.Ragged, num, now.Drawn, new Vector2(num3, 0f), now.Combed);
				if (now.Lobe > 0.01f)
				{
					Vector2 val3 = -heading * (r * 0.42f * (1f + 0.5f * Mathf.Max(0f, now.LongR - 1f)));
					cloud.Add(head + val3, now.BodyR * 0.74f, 0.95f * num2 * Mathf.Clamp(now.Lobe, 0f, 1f), now.Ragged, num, now.Drawn, new Vector2(num3 + val3.Dot(val), val3.Dot(val2)), now.Combed);
				}
				for (int i = 0; i < 3; i++)
				{
					float num4 = run.Spin * 0.8f + (float)i * 2.094f + 0.4f * Mathf.Sin(now.Mist * 2.3f + (float)i * 1.7f);
					Vector2 val4 = new Vector2(Mathf.Cos(num4), Mathf.Sin(num4)) * (r * now.LumpAt);
					if (now.LongR > 1.02f)
					{
						float num5 = val4.Dot(now.Axis);
						val4 += now.Axis * (num5 * (now.LongR - 1f) * 0.6f) - (val4 - now.Axis * num5) * (1f - Mathf.Pow(now.LongR, -0.25f));
					}
					cloud.Add(head + val4, now.BodyR * 0.52f, 0.9f * num2, now.Ragged, num, now.Drawn, new Vector2(num3 + val4.Dot(val), val4.Dot(val2)), now.Combed);
				}
				cloud.Aura(head, r * 3.4f, aura * num);
			}
			else
			{
				cloud.Aura(head, 8f, 0f);
			}
			if (layer == 1 && now.Charging > 0.01f && now.Charging < 0.999f)
			{
				for (int j = 0; j < 5; j++)
				{
					float num6 = (float)j * 1.2566f + 0.9f + 1.6f * now.Charging;
					Vector2 val5 = new Vector2(Mathf.Cos(num6), Mathf.Sin(num6) * 0.7f) * (r * Mathf.Lerp(2.5f, 0.5f, now.Charging * now.Charging));
					if (!(val5.LengthSquared() < 1f))
					{
						cloud.Add(head + val5, r * Mathf.Lerp(0.42f, 0.3f, now.Charging), 0.8f, 0.45f, Mathf.Sin((float)Math.PI * now.Charging) * 0.9f, -val5.Normalized() * 1.2f, Vector2.Zero, 0f);
					}
				}
			}
			if (now.Reaching > 0.01f && num > 0.004f)
			{
				for (int k = 0; k < 2; k++)
				{
					float num7 = 10f * Mathf.Sin(now.Mist * 2.6f + (float)k * 2.1f);
					Vector2 at = head + now.Down * (r * (0.45f + 0.1f * (float)k + (0.12f + 0.16f * (float)k) * now.Reaching)) + new Vector2(num7, 0f);
					cloud.Add(at, r * Mathf.Lerp(0.34f, 0.18f, (float)k), 0.92f, 0f, num * Mathf.Min(1f, now.Reaching * 2.5f), now.Down * (0.15f + 0.05f * (float)k + (0.3f + 0.3f * (float)k) * now.Reaching), Vector2.Zero, 0f);
				}
			}
			for (int l = 0; l < 12; l++)
			{
				ref TailPuff reference = ref run.Tail[l];
				if (!(reference.Seen <= 0.003f))
				{
					float num8 = layer switch
					{
						1 => reference.Hand, 
						0 => reference.Back, 
						_ => reference.Front, 
					};
					if (!(num8 <= 0.004f))
					{
						cloud.Add(reference.At, reference.R, reference.Thick, reference.Age, reference.Seen * num8, reference.Drawn, new Vector2(reference.Far - now.Mark, 0f), 1f);
					}
				}
			}
			if (layer == 1)
			{
				road.LayStream(cloud, now.Mist, now.Reach, now.DrawnIn, 1f);
				int num9 = 0;
				for (int m = 0; m < run.Cards.Count; m++)
				{
					if (num9 >= 4)
					{
						break;
					}
					Rolled rolled = run.Cards[m];
					if (rolled.Copy != null && !rolled.Poured && !(rolled.LeftAt < 0f))
					{
						float num10 = rolled.Vel.Length();
						if (!(num10 < 600f))
						{
							num9++;
							Vector2 val6 = rolled.Vel / num10;
							cloud.Add(rolled.Now - val6 * 40f, 34f, 0.8f, 0.3f, 0.8f * rolled.NowAlpha, val6 * 3f, Vector2.Zero, 0f);
						}
					}
				}
			}
			float num11 = Mathf.Clamp(Tuning.F("shadowstep.trail_thick", 0.62f), 0.2f, 1f);
			for (int num12 = run.Wisps.Count - 1; num12 >= 0; num12--)
			{
				Wisp wisp = run.Wisps[num12];
				if (wisp.Layer == layer)
				{
					float num13 = Mathf.Clamp((t - wisp.Born) / Mathf.Max(0.01f, wisp.Life), 0f, 1f);
					float num14 = 1f - Mathf.Exp(-3.2f * num13);
					cloud.Add(wisp.Pos + wisp.Vel * (num14 * 0.9f), wisp.R * (0.85f + wisp.Grow * num13), num11 * (1f - 0.35f * num13), Mathf.Max(num13, 0.02f), Mathf.Pow(1f - num13, 1.4f) * Mathf.Min(1f, num13 * 9f + 0.2f));
				}
			}
			if (layer == floor && run.HitAt >= 0f)
			{
				Burst(run, cloud, t - run.HitAt);
			}
			for (int n = 0; n < run.RoofNow.Count; n++)
			{
				RoofPuff roofPuff = run.RoofNow[n];
				float num15 = layer switch
				{
					1 => roofPuff.Hand, 
					0 => roofPuff.Back, 
					_ => roofPuff.Front, 
				};
				if (!(num15 <= 0.004f))
				{
					cloud.Add(roofPuff.At, roofPuff.R, roofPuff.Thick, roofPuff.Age, roofPuff.Seen * num15, roofPuff.Drawn, roofPuff.Way, roofPuff.Combed);
				}
			}
			cloud.End(head, r, run.Spin, now.Mist, intensity, now.Pulse);
		}

		private static void Burst(Run run, ShadowCloud cloud, float since)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			float num = since / Mathf.Max(0.05f, run.After);
			if (since <= 0f || num >= 1f)
			{
				return;
			}
			ShadowRoad road = run.Road;
			Vector2 feet = road.Feet;
			float num2 = Mathf.Clamp(Tuning.F("shadowstep.burst", 1f), 0.3f, 2f);
			float num3 = 1f - Mathf.Pow(1f - num, 2.6f);
			float num4 = road.View.X * 0.36f * num3 * num2;
			float num5 = Mathf.Min(1f, since / 0.025f);
			float num6 = Mathf.Pow(1f - num, 1.1f) * num5;
			for (int i = -1; i <= 1; i += 2)
			{
				for (int j = 0; j < 5; j++)
				{
					float num7 = ((float)j + 1f) / 5f;
					float num8 = 0.5f + 0.5f * Mathf.Sin(since * 16f + (float)j * 1.9f + (float)i);
					Vector2 val = new Vector2((float)i * (run.R * 0.35f + num4 * (0.22f + 0.78f * num7)), -14f - (26f + 40f * num7) * num8 * (1f - 0.6f * num));
					float reach = Mathf.Lerp(120f, 78f, num7) * (0.7f + 0.55f * num3) * num2;
					cloud.Add(feet + val, reach, Mathf.Lerp(1f, 0.8f, num7) * (1f - 0.45f * num), 0.06f + 0.6f * num, num6 * Mathf.Lerp(1f, 0.85f, num7), new Vector2((float)i * (0.9f + 1.5f * (1f - num)), 0f), new Vector2(Mathf.Abs(val.X) - num4 * 0.7f, 0f - val.Y), 1f);
				}
			}
			float landReach = road.LandReach;
			for (int k = 0; k < 3; k++)
			{
				float num9 = ((float)k - 1f) * landReach * 0.8f;
				float num10 = 1f - Mathf.Pow(1f - Mathf.Min(1f, num * 1.7f), 2.2f);
				float num11 = 70f + (190f + 80f * FxKit.Hash(k + 11)) * num10;
				cloud.Add(feet + new Vector2(num9 * (1f + 0.6f * num10), 0f - num11), 84f * (1f + 0.5f * num10), 0.9f * (1f - 0.7f * num), 0.12f + 0.7f * num, Mathf.Pow(1f - Mathf.Min(1f, num * 1.4f), 1.2f) * num5, new Vector2(num9 * 0.004f, 0f - Mathf.Max(0.05f, 1.1f * (1f - num))), Vector2.Zero, 0f);
			}
		}

		private static void Cards(Run run, float t, float da, float u, float s, float charging)
		{
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_077a: Unknown result type (might be due to invalid IL or missing references)
			//IL_077f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0791: Unknown result type (might be due to invalid IL or missing references)
			//IL_0796: Unknown result type (might be due to invalid IL or missing references)
			//IL_079b: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_080c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0811: Unknown result type (might be due to invalid IL or missing references)
			//IL_0816: Unknown result type (might be due to invalid IL or missing references)
			//IL_081a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0862: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_044e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0453: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_0532: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_0611: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_0625: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Unknown result type (might be due to invalid IL or missing references)
			//IL_063a: Unknown result type (might be due to invalid IL or missing references)
			//IL_063f: Unknown result type (might be due to invalid IL or missing references)
			//IL_065b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0660: Unknown result type (might be due to invalid IL or missing references)
			//IL_0663: Unknown result type (might be due to invalid IL or missing references)
			//IL_066a: Unknown result type (might be due to invalid IL or missing references)
			//IL_066f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0673: Unknown result type (might be due to invalid IL or missing references)
			//IL_0678: Unknown result type (might be due to invalid IL or missing references)
			//IL_067b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0692: Unknown result type (might be due to invalid IL or missing references)
			//IL_0697: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_056d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0735: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_057d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
			ShadowRoad road = run.Road;
			float clock = run.Clock;
			int count = run.Cards.Count;
			float num = Mathf.Clamp(Tuning.F("shadowstep.hand_dark", 0.55f), 0f, 1f);
			float num2 = Mathf.Clamp(Tuning.F("shadowstep.roll_dark", 0.8f), 0f, 1f);
			float num3 = Mathf.Min(0.03f, Mathf.Clamp(Tuning.F("shadowstep.pour", 0.1f), 0f, 0.4f) / (float)Mathf.Max(1, count - 1));
			float num4 = Mathf.Clamp(Tuning.F("shadowstep.flight", 0.15f), 0.06f, 0.8f);
			float amount = Mathf.Clamp(Tuning.F("shadowstep.card_blur", 1f), 0f, 3f);
			float along = Mathf.Clamp(Tuning.F("shadowstep.card_smear", 1f), 0f, 3f);
			Vector2 head = run.Head;
			Vector2 heading = run.Heading;
			Vector2 val = new Vector2(0f - heading.Y, heading.X);
			float r = run.R;
			float dir = road.Dir;
			bool flag = t >= run.Arrive + run.Charge;
			float num5 = Mathf.Lerp(road.StopAlong(3), road.StopAlong(4), Mathf.Clamp(Tuning.F("shadowstep.fling", 0.25f), 0f, 1f));
			if (((run.FlingAt < 0f) & flag) && s >= num5)
			{
				run.FlingAt = t;
				for (int i = 0; i < count; i++)
				{
					Rolled rolled = run.Cards[i];
					if (!(rolled.CaughtAt >= 0f))
					{
						rolled.CaughtAt = t;
						rolled.Order = run.Caught++;
					}
				}
				if (count == 0)
				{
					Slam(run);
				}
			}
			bool flag2 = u >= road.StopTime(6);
			bool flag3 = u >= 1f;
			bool visible = false;
			for (int j = 0; j < count; j++)
			{
				Rolled rolled2 = run.Cards[j];
				if (rolled2.Poured || rolled2.Copy == null)
				{
					continue;
				}
				Vector2 now = rolled2.Now;
				float num6 = Mathf.Lerp(rolled2.Dark0, num, Mathf.Clamp(clock / 0.12f, 0f, 1f));
				if (rolled2.CaughtAt < 0f)
				{
					float num7 = (rolled2.At.X - head.X) * dir - (r * 0.45f + road.CardWidth * 0.3f);
					if (!flag || !(num7 <= 0f))
					{
						if (!flag)
						{
							float num8 = Mathf.Max(0f, (rolled2.At.X - road.Start.X) * dir);
							float num9 = charging * Mathf.Exp((0f - num8) / (road.CardWidth * 3f));
							Put(rolled2, rolled2.At + new Vector2((0f - dir) * 28f * num9, 4f * num9), rolled2.Turn - dir * 0.24f * num9, 1f, Mathf.Lerp(num6, num2, 0.75f * num9), 1f);
						}
						else
						{
							float num10 = Mathf.Clamp(1f - num7 / (road.CardWidth * 2.6f), 0f, 1f);
							num10 *= num10;
							Put(rolled2, rolled2.At + new Vector2(dir * 22f * num10, -34f * num10), rolled2.Turn + dir * 0.2f * num10, 1f + 0.03f * num10, Mathf.Lerp(num6, num2, 0.8f * num10), 1f);
						}
						if (Blur(rolled2, now, da, amount, along))
						{
							visible = true;
						}
						continue;
					}
					rolled2.CaughtAt = t;
					rolled2.Order = run.Caught++;
					if (run.Flicks < 6 && FxKit.Alive((GodotObject)(object)run.Root))
					{
						run.Flicks++;
						Shade.Motes((Node)(object)run.Root, rolled2.At + new Vector2(0f, (0f - road.Tall) * 0.25f), 3, 380f, new Vector2(0f - dir, -0.9f), 45f, 14f, 0.3);
					}
				}
				if (flag3)
				{
					rolled2.Poured = true;
					Drop(rolled2);
					if (!run.Slammed)
					{
						Slam(run);
					}
				}
				else if (run.FlingAt >= 0f && (flag2 || rolled2.LeftAt >= 0f || t >= run.FlingAt + (float)Mathf.Max(0, rolled2.Order) * num3))
				{
					if (rolled2.LeftAt < 0f)
					{
						rolled2.LeftAt = t;
						rolled2.From = rolled2.Now;
						rolled2.FromTurn = rolled2.NowTurn;
						rolled2.FromSize = rolled2.NowSize;
						rolled2.Tan = ((rolled2.Vel.Length() > 200f) ? rolled2.Vel.Normalized() : heading);
					}
					float num11 = (t - rolled2.LeftAt) / num4;
					if (num11 >= 1f)
					{
						rolled2.Poured = true;
						Drop(rolled2);
						if (!run.Slammed)
						{
							Slam(run);
						}
						else if (rolled2.Order % 2 == 0 && FxKit.Alive((GodotObject)(object)run.Root))
						{
							Shade.Puff((Node)(object)run.Root, road.Pile, road.Tall * 0.26f, 1.5f, 0.22f, 0.45f, 0.5f);
						}
						continue;
					}
					Vector2 b = rolled2.From + rolled2.Tan * (rolled2.From.DistanceTo(road.Pile) * 0.45f);
					float num12 = Mathf.Pow(Mathf.Max(0f, num11), 1.15f);
					Vector2 val2 = FxKit.Bezier(rolled2.From, b, road.Pile, num12);
					Vector2 val3 = FxKit.Bezier(rolled2.From, b, road.Pile, Mathf.Min(1f, num12 + 0.02f));
					float turn = rolled2.FromTurn;
					if (val3.DistanceSquaredTo(val2) > 0.01f)
					{
						float fromTurn = rolled2.FromTurn;
						Vector2 val4 = val3 - val2;
						turn = Mathf.LerpAngle(fromTurn, val4.Angle() + (float)Math.PI / 2f, Smooth(num11 / 0.3f));
					}
					Put(rolled2, val2, turn, Mathf.Lerp(rolled2.FromSize, 0.26f, Smooth(num11 / 0.8f)), num2, 1f - Mathf.Clamp((num11 - 0.85f) / 0.15f, 0f, 1f));
					if (Blur(rolled2, now, da, amount, along))
					{
						visible = true;
					}
				}
				else
				{
					float num13 = t - rolled2.CaughtAt;
					float num14 = Smooth(num13 / 0.085f);
					Vector2 val5 = Was(run, t - rolled2.Lag * num14).At + val * (rolled2.Side * r * 0.34f) + heading * (r * 0.1f * Mathf.Sin(num13 * 7f + rolled2.Rate));
					Vector2 at = rolled2.At.Lerp(val5, num14) + new Vector2(0f, (0f - road.Tall) * 0.2f * Mathf.Sin((float)Math.PI * Mathf.Clamp(num13 / 0.085f, 0f, 1f)));
					Put(rolled2, at, rolled2.Turn + dir * (2.2f * num14 + num13 * rolled2.Rate), Mathf.Lerp(1f, 0.62f, num14), Mathf.Lerp(num6, num2, num14), 1f);
					if (Blur(rolled2, now, da, amount, along))
					{
						visible = true;
					}
				}
			}
			if (FxKit.Alive((GodotObject)(object)run.SoftLayer))
			{
				((CanvasItem)run.SoftLayer).Visible = visible;
			}
		}

		private static bool Blur(Rolled card, Vector2 was, float da, float amount, float along)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			if (da > 0f)
			{
				card.Vel = card.Vel.Lerp((card.Now - was) / da, 1f - Mathf.Exp((0f - da) * 40f));
			}
			if (!float.IsFinite(card.Vel.X) || !float.IsFinite(card.Vel.Y))
			{
				card.Vel = Vector2.Zero;
			}
			Node2D soft = card.Soft;
			ShaderMaterial softMat = card.SoftMat;
			if (soft == null || softMat == null)
			{
				return false;
			}
			try
			{
				if (!FxKit.Alive((GodotObject)(object)soft) || !FxKit.Alive((GodotObject)(object)softMat))
				{
					return false;
				}
				if (!FxKit.Alive((GodotObject)(object)card.Copy))
				{
					((CanvasItem)soft).Visible = false;
					return false;
				}
				float num = card.Vel.Length();
				float num2 = amount * 3.2f * Smooth((num - 250f) / 2600f);
				float num3 = Mathf.Min(30f, amount * along * num * 0.0065f);
				if ((num2 < 0.15f && num3 < 0.6f) || card.NowAlpha <= 0.02f)
				{
					((CanvasItem)soft).Visible = false;
					return false;
				}
				((CanvasItem)soft).Visible = true;
				soft.GlobalTransform = new Transform2D(card.NowTurn, card.Size * card.NowSize, 0f, card.Now);
				softMat.SetShaderParameter(PSoft, Variant.op_Implicit(num2));
				softMat.SetShaderParameter(PSmear, Variant.op_Implicit(card.Vel / Mathf.Max(1f, num) * num3));
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Shadow Step (soft card, lay): " + ex.Message);
				card.Soft = null;
				card.SoftMat = null;
				try
				{
					if (FxKit.Alive((GodotObject)(object)soft))
					{
						((Node)soft).QueueFree();
					}
				}
				catch
				{
				}
				return false;
			}
		}

		private static void ShockGo(Run run, int kind)
		{
			if (run.ShockMat != null)
			{
				run.ShockAt = run.Clock;
				run.ShockKind = kind;
			}
		}

		private static void ShockStep(Run run)
		{
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			if (run.ShockAt < 0f)
			{
				return;
			}
			try
			{
				if (run.ShockMat == null || !FxKit.Alive((GodotObject)(object)run.ShockFrame) || !FxKit.Alive((GodotObject)(object)run.Shock) || !FxKit.Alive((GodotObject)(object)run.ShockMat))
				{
					run.ShockAt = -1f;
					return;
				}
				ShadowRoad road = run.Road;
				bool flag = run.ShockKind == 1;
				float num = (run.Clock - run.ShockAt) / (flag ? 0.46f : 0.3f);
				if (num >= 1f)
				{
					run.ShockAt = -1f;
					((CanvasItem)run.ShockFrame).Visible = false;
					return;
				}
				float num2 = Mathf.Clamp(Tuning.F("shadowstep.shock", 1f), 0f, 3f);
				float num3 = (flag ? (road.View.X * 0.5f) : (road.View.Y * 0.42f)) * (1f - Mathf.Pow(1f - num, 2.2f));
				float num4 = (flag ? 0.42f : 1f);
				Vector2 val = (flag ? road.Feet : road.Start);
				float num5 = (flag ? 64f : 46f) * (1f + 0.8f * num);
				float num6 = (flag ? 30f : 15f) * num2 * Mathf.Pow(1f - num, 1.3f);
				Vector2 val2 = new Vector2(num3 + num5 * 3f, (num3 + num5 * 3f) * num4);
				((CanvasItem)run.ShockFrame).Visible = true;
				run.ShockFrame.GlobalTransform = new Transform2D(0f, val - val2);
				((Control)run.Shock).Position = Vector2.Zero;
				((Control)run.Shock).Size = val2 * 2f;
				Transform2D canvasTransform = ((CanvasItem)run.ShockFrame).GetCanvasTransform();
				float num7 = Mathf.Abs(canvasTransform.Scale.Y);
				if (!(num7 > 0.05f) || !(num7 < 20f))
				{
					num7 = 1f;
				}
				run.ShockMat.SetShaderParameter(StringName.op_Implicit("view"), Variant.op_Implicit(road.View));
				run.ShockMat.SetShaderParameter(StringName.op_Implicit("centre"), Variant.op_Implicit(canvasTransform * val));
				run.ShockMat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(num3 * num7));
				run.ShockMat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(num5 * num7));
				run.ShockMat.SetShaderParameter(StringName.op_Implicit("push"), Variant.op_Implicit(num6 * num7));
				run.ShockMat.SetShaderParameter(StringName.op_Implicit("lie"), Variant.op_Implicit(num4));
			}
			catch (Exception ex)
			{
				DevLog.Warn("Shadow Step (shock): " + ex.Message);
				run.ShockAt = -1f;
				run.ShockMat = null;
				try
				{
					if (FxKit.Alive((GodotObject)(object)run.ShockFrame))
					{
						((CanvasItem)run.ShockFrame).Visible = false;
					}
				}
				catch
				{
				}
			}
		}

		private static void Launch(Run run)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			run.Launched = true;
			ShockGo(run, 0);
			Node2D root = run.Root;
			if (FxKit.Alive((GodotObject)(object)root))
			{
				ShadowRoad road = run.Road;
				Shade.Puff((Node)(object)root, road.Start + new Vector2((0f - road.Dir) * 50f, -10f), road.Tall * 0.55f, 2.2f, 0.36f, 0.7f, 0.8f);
				Shade.Motes((Node)(object)root, road.Start, 8, 520f, new Vector2(0f - road.Dir, -0.35f), 50f, road.Tall * 0.2f, 0.35);
			}
		}

		private static void Slam(Run run)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			run.Slammed = true;
			Sfx.Play("shadow_slam");
			if (Tuning.F("shadowstep.pile_shake", 1f) >= 0.5f)
			{
				GameRefs.Shake(1);
			}
			Node2D root = run.Root;
			if (FxKit.Alive((GodotObject)(object)root))
			{
				Vector2 pile = run.Road.Pile;
				float num = run.Road.Tall * 0.34f;
				Vector2 value = new Vector2((0f - run.Road.Dir) * 0.35f, -1f);
				Shade.Puff((Node)(object)root, pile, num * 2.2f, 1.9f, 0.4f, 0.9f, 1f);
				Shade.Motes((Node)(object)root, pile, 10, 480f, value, 60f, num * 0.3f);
				Shade.Sparks((Node)(object)root, pile, 6, 400f, value, 65f, num * 0.25f);
			}
		}

		private static void Hit(Run run)
		{
			ShockGo(run, 1);
			int num = Mathf.Clamp(Mathf.RoundToInt(Tuning.F("shadowstep.shake", 2f)), 0, 3);
			if (num >= 1)
			{
				GameRefs.Shake(num);
			}
		}

		private static void Land(Run run)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			run.Landed = true;
			CardFxContext c = run.C;
			ShadowPoolFx.Landed(run.Owner, Mathf.Clamp(Tuning.F("shadowstep.erupt", 1.5f), 1f, 2.5f));
			if (FxKit.Alive((GodotObject)(object)c.BackParent))
			{
				Shade.Puff(c.BackParent, run.Road.Feet, c.Body.X * 1.3f, 2.4f, 0.45f, 0.85f, 0.9f);
			}
			if (FxKit.Alive((GodotObject)(object)c.FrontParent))
			{
				Shade.Motes(c.FrontParent, run.Road.Feet, 12, 420f, new Vector2(0f, -1f), 80f, c.Body.X * 0.35f);
				Shade.Sparks(c.FrontParent, run.Road.Feet, 5, 380f, new Vector2(0f, -1f), 80f, c.Body.X * 0.3f);
			}
		}

		private static void Clear(Run run)
		{
			try
			{
				run.Back?.Free();
				run.Body?.Free();
				run.Veil?.Free();
				run.Front?.Free();
				if (FxKit.Alive((GodotObject)(object)run.SoftLayer))
				{
					((Node)run.SoftLayer).QueueFree();
				}
				if (FxKit.Alive((GodotObject)(object)run.ShockFrame))
				{
					((Node)run.ShockFrame).QueueFree();
				}
				if (FxKit.Alive((GodotObject)(object)run.DimRoot))
				{
					((Node)run.DimRoot).QueueFree();
				}
				if (FxKit.Alive((GodotObject)(object)run.ShadeRoot))
				{
					((Node)run.ShadeRoot).QueueFree();
				}
				if (FxKit.Alive((GodotObject)(object)run.TopRoot))
				{
					if (((Node)run.TopRoot).IsInsideTree())
					{
						FxKit.FreeAfter((Node)(object)run.TopRoot, 0.6);
					}
					else
					{
						((Node)run.TopRoot).QueueFree();
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Shadow Step (clear): " + ex.Message);
			}
			run.Back = null;
			run.Body = null;
			run.Veil = null;
			run.Front = null;
			run.SoftLayer = null;
			run.ShockFrame = null;
			run.Shock = null;
			run.ShockMat = null;
			run.ShockAt = -1f;
			run.DimRoot = null;
			run.TopRoot = null;
			run.Dim = null;
			run.ShadeRoot = null;
			run.ShadeBackQuad = null;
			run.ShadeFrontQuad = null;
			run.ShadeBack = null;
			run.ShadeFront = null;
			run.Roofed = 0f;
			run.Wisps.Clear();
			run.Roof.Clear();
			run.RoofNow.Clear();
			run.Way.Clear();
		}

		private static void Finish(Run run)
		{
			if (run == null || run.Finished)
			{
				return;
			}
			run.Finished = true;
			run.Landed = true;
			Live.Remove(run);
			ShadowPoolFx.Landed(run.Owner);
			for (int i = 0; i < run.Cards.Count; i++)
			{
				Rolled rolled = run.Cards[i];
				try
				{
					Drop(rolled);
					if (!rolled.Flown)
					{
						NCard real = rolled.Real;
						if (real != null && (!FxKit.Alive((GodotObject)(object)real) || real.Model == rolled.Card))
						{
							CardStandIn.Show(real);
						}
						if (rolled.Claimed)
						{
							CardFlight.Release(rolled.Card);
						}
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Shadow Step (clean up): " + ex.Message);
				}
			}
			run.Cards.Clear();
			run.Cleared = true;
			Clear(run);
			Node2D root = run.Root;
			run.Root = null;
			if (FxKit.Alive((GodotObject)(object)root))
			{
				if (((Node)root).IsInsideTree())
				{
					FxKit.FreeAfter((Node)(object)root, 0.6);
				}
				else
				{
					((Node)root).QueueFree();
				}
			}
		}
	}
