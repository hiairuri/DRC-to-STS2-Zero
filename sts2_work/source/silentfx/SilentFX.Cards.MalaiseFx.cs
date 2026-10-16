// namespace: SilentFX.Cards
// type: MalaiseFx (class)
// source lines: 71915-75832
	public sealed class MalaiseFx : CardFx
	{
		internal sealed class Orb : EnergyBeads.Bead
		{
			public Color Own;

			public float OwnInk;

			public bool Placed;

			public float Turned;

			public int Index;

			public int Id;

			public int Rolls;

			public Vector2 Goal;

			public bool Hopping;

			public float HopAt;

			public float HopDur;

			public Vector2 HopFrom;

			public float Wait;

			public bool Surplus;

			public Vector2 Last;

			public bool Wandering;

			public Vector2 Wp;

			public Vector2 Wv;

			public Vector2 Spot;

			public float SpotAt;

			public Vector2 Hold;

			public float BobA;

			public float BobB;

			public int State;

			public float DrinkAt;

			public bool Bit;

			public int Heaved;

			public int Gulped;

			public bool Torn;

			public float TornAt;

			public float FullAt = -1f;

			public float GreyAt;

			public float Fill;

			public float Grow = 1f;

			public float Slow = 1f;

			public float Might = 1f;

			public float Arc = 1f;

			public float Spin;

			public Sprite2D Flare;

			public Stream Stream;
		}

		internal sealed class Puff
		{
			public Vector2 Pos;

			public Vector2 Vel;

			public float R;

			public float R0 = 1f;

			public float Thick = 1f;

			public float Age;

			public float Seen;

			public Vector2 Anchor;

			public Vector2 Anchor0;

			public float Psi;

			public float Born;

			public float Life = 1f;

			public float Seed;

			public float Fade = 1f;

			public Orb Orb;

			public Vector2 Goal;
		}

		internal sealed class Ember
		{
			public Sprite2D Dot;

			public float S;

			public float STear;

			public float Lat;

			public float Seed;

			public float Size = 1f;
		}

		internal sealed class Stream
		{
			public Orb Orb;

			public MistGroup Group;

			public Sprite2D[] Glows;

			public Sprite2D[] Bodies;

			public float[] Lat;

			public float[] Size;

			public float[] Jit;

			public readonly List<Ember> Embers = new List<Ember>();

			public Sprite2D Knot;

			public Vector2 From;

			public float Lift;

			public float Start;

			public bool Torn;

			public float TornAt;

			public float U = 1f;

			public float Sign = 1f;

			public float Flow;

			public float Seed;

			public int Rolls;

			public float Fade = 1f;

			public bool Cut;

			public bool Done;
		}

		private struct Way
		{
			public Vector2 C;

			public Vector2 P1;

			public Vector2 P2;

			public Vector2 E;

			public Vector2 B;

			public Vector2 Normal;

			public float R;

			public float Turns;

			public float From;

			public float Sign;

			public float Split;

			public float Sway;

			public float Time;

			public float Seed;
		}

		internal sealed class MistGroup
		{
			public Creature Target;

			public Node2D Root;

			public Vector2 Chest;

			public float W = 200f;

			public float H = 240f;

			public float Side = 1f;

			public float U = 1f;

			public float Show;

			public bool Leaving;

			public int Id;

			public int Rolls;

			public float Seed;

			public readonly List<Puff> Smoke = new List<Puff>();

			public MistSheet Sheet;

			public float NextSmoke;

			public int NextBead;

			public Sprite2D Wide;

			public Sprite2D Mid;

			public Sprite2D Core;

			public int Taken;

			public int Total;

			public int Notches;

			public float TornAt = -100f;

			public float Ember;
		}

		internal sealed class Slump
		{
			public Creature Who;

			public Node2D Picture;

			public Vector2 Home;

			public Vector2 Wrote;

			public Vector2 Shift;

			public bool Over;

			public float Y;

			public float Speed;

			public float Rest;

			public float Most;

			public bool Rising;

			public float RiseAt;

			public float RiseFrom;

			public float Shake;

			public float ShakeAt;

			public float ShakeSeed;

			public int Kicks;

			public Vector2 Off;
		}

		internal sealed class Drain
		{
			public Node2D Holder;

			public Node2D MistLayer;

			public Node2D BeadLayer;

			public Node2D FrontLayer;

			public EnergyBeads Beads;

			public readonly List<Orb> Orbs = new List<Orb>();

			public readonly List<Orb> Active = new List<Orb>();

			public readonly List<MistGroup> Mists = new List<MistGroup>();

			public readonly List<Stream> Streams = new List<Stream>();

			public MistGroup Mist;

			public Orb Extra;

			public bool FromCounter;

			public bool Remote;

			public bool Pending;

			public bool GameDone;

			public CardFxContext Context;

			public object Invocation;

			public int EnergyPart;

			public int ChemPart;

			public int ExtraPart;

			public Node Room;

			public CardModel Card;

			public Vector2 CardAt;

			public int NextId;

			public Vector2 Chest;

			public Vector2 Body = new Vector2(244f, 244f);

			public float Side = 1f;

			public bool HasAim;

			public bool Aimed;

			public Creature Target;

			public Vector2 TargetChest;

			public float TargetW = 200f;

			public float TargetH = 240f;

			public float View = 1f;

			public float Bead = 24f;

			public float Grow = 1.7f;

			public float Vary = 0.12f;

			public float Stand = 70f;

			public float ZoneW = 130f;

			public float ZoneH = 150f;

			public float Pace = 1f;

			public float Apart = 56f;

			public float SmokeAlpha = 0.45f;

			public float SmokeEvery = 0.3f;

			public int SmokeMax = 12;

			public float MistSize = 1f;

			public float GlowK = 1f;

			public float EmberLevel = 0.35f;

			public float Latch = 0.12f;

			public int Heaves = 3;

			public float HeavePx = 14f;

			public float HeaveMore = 1.3f;

			public float HeavePull = 0.17f;

			public float HeaveGrip = 0.1f;

			public float HeaveBack = 0.45f;

			public float Whip = 0.16f;

			public float Coil = 2.2f;

			public float CoilTurns = 5.5f;

			public int PuffN = 36;

			public float PuffSize = 1f;

			public float StreamAlpha = 0.85f;

			public float Time;

			public float OwnedAt;

			public bool Dismissed;

			public float DismissAt;

			public bool Closing;

			public float CloseAt;

			public bool Playing;

			public float PlayAt;

			public float ReadyAt;

			public int Stacks;

			public int Taken;

			public float LastStepAt = -1f;

			public float EndAt = -1f;

			public bool Cut;

			public StatusSteps.Run Ui;

			public bool UiDone;

			public Slump Slump;

			public float Dip;

			public float Sag;

			public float Nudge;

			public float Shudder;

			public float SlumpHold = 0.3f;

			public float Recover = 0.45f;

			public bool Dead;

			public Drain()
			{
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			}
		}

		internal sealed class MistSheet
		{
			public const int Max = 40;

			private static readonly StringName PCount = StringName.op_Implicit("count");

			private static readonly StringName PAt = StringName.op_Implicit("at");

			private static readonly StringName PAx = StringName.op_Implicit("ax");

			private static readonly StringName PEx = StringName.op_Implicit("ex");

			private static readonly StringName PSx = StringName.op_Implicit("sx");

			private static readonly StringName PAn = StringName.op_Implicit("an");

			private static readonly StringName PRo = StringName.op_Implicit("ro");

			private static readonly StringName PClock = StringName.op_Implicit("clock");

			private static readonly StringName PIntensity = StringName.op_Implicit("intensity");

			private static readonly StringName PHeat = StringName.op_Implicit("heat");

			private static readonly StringName PGrain = StringName.op_Implicit("grain");

			public readonly Node2D Holder;

			private readonly Node2D _frame;

			private readonly ColorRect _quad;

			private readonly ShaderMaterial _mat;

			private readonly Vector2[] _at = new Vector2[40];

			private readonly Vector2[] _ax = new Vector2[40];

			private readonly Vector2[] _ex = new Vector2[40];

			private readonly Vector2[] _sx = new Vector2[40];

			private readonly Vector2[] _an = new Vector2[40];

			private readonly float[] _ro = new float[40];

			private int _n;

			private Vector2 _lo;

			private Vector2 _hi;

			public bool Ok
			{
				get
				{
					if (_mat != null && FxKit.Alive((GodotObject)(object)Holder) && FxKit.Alive((GodotObject)(object)_frame))
					{
						return FxKit.Alive((GodotObject)(object)_quad);
					}
					return false;
				}
			}

			public MistSheet(Node parent, float seed)
			{
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Expected Obj, but got Unknown
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Expected Obj, but got Unknown
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0111: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				Holder = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_MalaiseMist")
				};
				_frame = new Node2D
				{
					Name = StringName.op_Implicit("Sheet"),
					Visible = false
				};
				((Node)Holder).AddChild((Node)(object)_frame, false, (InternalMode)0);
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					parent.AddChild((Node)(object)Holder, false, (InternalMode)0);
				}
				_mat = ShaderLib.NewMaterial("malaise_mist");
				if (_mat != null)
				{
					_mat.SetShaderParameter(StringName.op_Implicit("seed"), Variant.op_Implicit(seed));
					_mat.SetShaderParameter(PCount, Variant.op_Implicit(0));
					_quad = FxKit.Quad(new Vector2(8f, 8f), (Material)(object)_mat);
					_quad.Color = new Color(1f, 1f, 1f, 0f);
					((Node)_frame).AddChild((Node)(object)_quad, false, (InternalMode)0);
				}
			}

			public void Begin()
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				_n = 0;
				_lo = new Vector2(1E+09f, 1E+09f);
				_hi = new Vector2(-1E+09f, -1E+09f);
			}

			public void Add(Vector2 at, float reach, float thick, float age, float seen, Vector2 drawn, Vector2 anchor, float turn)
			{
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0126: Unknown result type (might be due to invalid IL or missing references)
				//IL_012b: Unknown result type (might be due to invalid IL or missing references)
				//IL_013c: Unknown result type (might be due to invalid IL or missing references)
				//IL_013e: Unknown result type (might be due to invalid IL or missing references)
				//IL_014f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0151: Unknown result type (might be due to invalid IL or missing references)
				//IL_018f: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
				if (_n < 40 && !(reach < 1f) && !(thick <= 0.002f) && !(seen <= 0.003f) && float.IsFinite(at.X) && float.IsFinite(at.Y) && float.IsFinite(reach))
				{
					if (!float.IsFinite(drawn.X) || !float.IsFinite(drawn.Y))
					{
						drawn = Vector2.Zero;
					}
					if (!float.IsFinite(anchor.X) || !float.IsFinite(anchor.Y))
					{
						anchor = at;
					}
					if (!float.IsFinite(turn))
					{
						turn = 0f;
					}
					float num = drawn.Length();
					if (num > 7f)
					{
						drawn *= 7f / num;
						num = 7f;
					}
					_at[_n] = at;
					_ax[_n] = new Vector2(reach, Mathf.Clamp(thick, 0f, 1f));
					_ex[_n] = new Vector2(Mathf.Clamp(age, 0f, 1f), Mathf.Clamp(seen, 0f, 1f));
					_sx[_n] = drawn;
					_an[_n] = anchor;
					_ro[_n] = turn;
					_n++;
					float num2 = reach * 2.7f * (1f + num);
					_lo = new Vector2(Mathf.Min(_lo.X, at.X - num2), Mathf.Min(_lo.Y, at.Y - num2));
					_hi = new Vector2(Mathf.Max(_hi.X, at.X + num2), Mathf.Max(_hi.Y, at.Y + num2));
				}
			}

			public void End(float clock, float intensity, float heat, float grain)
			{
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0112: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_012d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0143: Unknown result type (might be due to invalid IL or missing references)
				//IL_0148: Unknown result type (might be due to invalid IL or missing references)
				//IL_0154: Unknown result type (might be due to invalid IL or missing references)
				//IL_0159: Unknown result type (might be due to invalid IL or missing references)
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Unknown result type (might be due to invalid IL or missing references)
				//IL_019c: Unknown result type (might be due to invalid IL or missing references)
				//IL_019d: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01be: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_01da: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0210: Unknown result type (might be due to invalid IL or missing references)
				//IL_022b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0246: Unknown result type (might be due to invalid IL or missing references)
				//IL_0261: Unknown result type (might be due to invalid IL or missing references)
				//IL_027c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0292: Unknown result type (might be due to invalid IL or missing references)
				//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
				//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0302: Unknown result type (might be due to invalid IL or missing references)
				if (!Ok)
				{
					return;
				}
				if (_n == 0 || intensity <= 0.003f)
				{
					((CanvasItem)_frame).Visible = false;
					return;
				}
				Rect2 val = OnScreen((CanvasItem)(object)_frame);
				Vector2 val2 = new Vector2(Mathf.Max(_lo.X, val.Position.X - 40f), Mathf.Max(_lo.Y, val.Position.Y - 40f));
				Vector2 val3 = new Vector2(Mathf.Min(_hi.X, val.End.X + 40f), Mathf.Min(_hi.Y, val.End.Y + 40f));
				if (val3.X - val2.X < 2f || val3.Y - val2.Y < 2f)
				{
					((CanvasItem)_frame).Visible = false;
					return;
				}
				for (int i = _n; i < 40; i++)
				{
					_at[i] = Vector2.Zero;
					_ax[i] = new Vector2(1f, 0f);
					_ex[i] = new Vector2(1f, 0f);
					_sx[i] = Vector2.Zero;
					_an[i] = Vector2.Zero;
					_ro[i] = 0f;
				}
				((CanvasItem)_frame).Visible = true;
				_frame.GlobalTransform = new Transform2D(0f, val2);
				((Control)_quad).Position = Vector2.Zero;
				((Control)_quad).Size = val3 - val2;
				_mat.SetShaderParameter(PCount, Variant.op_Implicit(_n));
				_mat.SetShaderParameter(PAt, Variant.op_Implicit(_at));
				_mat.SetShaderParameter(PAx, Variant.op_Implicit(_ax));
				_mat.SetShaderParameter(PEx, Variant.op_Implicit(_ex));
				_mat.SetShaderParameter(PSx, Variant.op_Implicit(_sx));
				_mat.SetShaderParameter(PAn, Variant.op_Implicit(_an));
				_mat.SetShaderParameter(PRo, Variant.op_Implicit(_ro));
				_mat.SetShaderParameter(PClock, Variant.op_Implicit(clock));
				_mat.SetShaderParameter(PIntensity, Variant.op_Implicit(Mathf.Clamp(intensity, 0f, 1f)));
				_mat.SetShaderParameter(PHeat, Variant.op_Implicit(Mathf.Clamp(heat, 0f, 2f)));
				_mat.SetShaderParameter(PGrain, Variant.op_Implicit(Mathf.Clamp(grain, 0.3f, 3f)));
			}

			public void Free()
			{
				if (FxKit.Alive((GodotObject)(object)Holder))
				{
					((Node)Holder).QueueFree();
				}
			}
		}

		public static readonly Color Purple = new Color(0.7f, 0.38f, 1f, 1f);

		private static readonly Color Gorged = new Color(0.5f, 0.12f, 0.3f, 1f);

		private static readonly Color Ash = new Color(0.56f, 0.54f, 0.58f, 1f);

		private static readonly Color Fleck = new Color(1f, 0.46f, 0.32f, 0.95f);

		private static readonly Color GlowCore = new Color(1f, 0.68f, 0.5f, 1f);

		private static readonly Color GlowMid = new Color(0.78f, 0.22f, 0.15f, 1f);

		private static readonly Color GlowWide = new Color(0.38f, 0.07f, 0.07f, 1f);

		private static readonly Color EmberTint = new Color(1f, 0.55f, 0.35f, 1f);

		private const int Waiting = 0;

		private const int Drinking = 1;

		private const int Greying = 2;

		private const int Gone = 3;

		private const float HopTime = 0.42f;

		private const float JumpFar = 70f;

		private const float GreyTime = 0.24f;

		private const float AshTime = 1.8f;

		private const float UiLinger = 0.35f;

		private const float FadeTime = 0.32f;

		private const float MistIn = 0.35f;

		private const float MistOut = 0.6f;

		private const float SpringK = 220f;

		private const float SpringC = 21.4f;

		private const float Swell = 0.2f;

		private static readonly Color Blood = new Color(1f, 0.2f, 0.22f, 1f);

		private static readonly Color Fog = new Color(0.42f, 0.05f, 0.08f, 1f);

		private const float PuffWide = 46f;

		private const float Flat = 0.62f;

		private const float RunsPerSec = 1.4f;

		private const int EmbersPer = 5;

		private const float WaveLead = 0.02f;

		private const float WaveRun = 0.8f;

		private const float HeaveW = 20f;

		private static readonly List<Drain> Live = new List<Drain>();

		private static readonly List<Slump> Slumps = new List<Slump>();

		private static bool _ticking;

		private static bool _resetting;

		private static Color A(Color c, float alpha)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return new Color(c.R, c.G, c.B, alpha);
		}

		private static float Smooth(float x)
		{
			x = Mathf.Clamp(x, 0f, 1f);
			return x * x * (3f - 2f * x);
		}

		private static int MaxShown()
		{
			return Mathf.Clamp(Tuning.I("malaise.max", 12), 1, 16);
		}

		public override void OnPlay(CardFxContext c)
		{
			try
			{
				Drain drain = null;
				try
				{
					drain = ((c.Remote && MultiplayerFx.Active) ? null : (c.Held as MalaiseHeld)?.Take());
					if (drain != null && (drain.Dead || drain.Closing || !FxKit.Alive((GodotObject)(object)drain.Holder)))
					{
						drain = null;
					}
					bool flag = c.Card == null;
					int num = (flag ? Math.Max(0, Tuning.I("malaise.preview_x", 3)) : XOf(c.Card, drain));
					int num2 = ((!flag && Upgraded(c.Card)) ? 1 : 0);
					int num3 = num + num2;
					if (!c.HasTarget || num3 <= 0)
					{
						if (drain != null)
						{
							Close(drain);
						}
						return;
					}
					int num4 = (flag ? num : EnergyBeads.EnergyPartOf(c.Card, num));
					if (drain == null)
					{
						drain = Create(c, 0, 0, extra: false, fromCounter: false);
						if (drain == null)
						{
							return;
						}
					}
					if (drain.Remote)
					{
						drain.Target = c.Target;
						drain.Context = c;
						drain.Stacks = num3;
						drain.EnergyPart = num4;
						drain.ChemPart = num - num4;
						drain.ExtraPart = num2;
						drain.Invocation = MultiplayerFx.CurrentInvocation;
						drain.Ui = StatusSteps.Capture(c.Target, Tuning.F("malaise.max_hold", 4f));
						drain.Pending = true;
						Drain drain2 = Prior(drain);
						if (drain2 == null)
						{
							Start(drain, null);
						}
						else if (drain2.Remote && drain2.UiDone)
						{
							drain.Ui = StatusSteps.Activate(drain.Ui);
							StatusSteps.Suspend(drain.Ui);
						}
					}
					else
					{
						Begin(drain, c, num3, num4, num - num4, num2);
						if (MultiplayerFx.Active && c.Target != null)
						{
							Drain[] array = Live.ToArray();
							foreach (Drain drain3 in array)
							{
								if (drain3.Remote && !drain3.Dead && drain3.Target == c.Target)
								{
									StatusSteps.Finish(drain3.Ui);
									drain3.Ui = null;
									drain3.UiDone = true;
								}
							}
						}
					}
					if (PowerFxManager.LogTriggers && !flag)
					{
						DevLog.Info($"Malaise: {drain.Active.Count} bead(s) for {num3} stack(s)" + ((num - num4 > 0) ? $" ({num - num4} black)" : "") + ((num2 > 0) ? " (+1 upgraded)" : "") + ((drain.Ui != null) ? "" : ", icons left to the game"));
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Malaise: " + ex);
					Free(drain);
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("MalaiseFx.OnPlay: " + ex2);
			}
		}

		internal static void NoteCardFinished(CardModel card)
		{
			if (card == null)
			{
				return;
			}
			Drain[] array = Live.ToArray();
			foreach (Drain drain in array)
			{
				if (drain.Remote && !drain.Dead && !drain.GameDone && drain.Invocation != null && drain.Card == card && drain.Invocation == MultiplayerFx.CurrentInvocation)
				{
					if ((object)drain.Room == GameRefs.CombatRoom && MultiplayerFx.Active)
					{
						StatusSteps.Seal(drain.Ui);
						drain.GameDone = true;
						break;
					}
					Free(drain);
				}
			}
		}

		private static Drain Prior(Drain d)
		{
			Drain drain = Live.Find((Drain o) => o != d && !o.Dead && !o.Remote && o.Playing && o.Target == d.Target);
			if (drain != null)
			{
				return drain;
			}
			foreach (Drain item in Live)
			{
				if (item == d)
				{
					break;
				}
				if (!item.Dead && item.Target == d.Target && (item.Playing || item.Pending))
				{
					return item;
				}
			}
			return null;
		}

		private static Drain Next(Drain d)
		{
			bool flag = false;
			foreach (Drain item in Live)
			{
				if (item == d)
				{
					flag = true;
				}
				else if (flag && !item.Dead && item.Remote && item.Pending && item.Target == d.Target)
				{
					return item;
				}
			}
			return null;
		}

		private static void Start(Drain d, StatusSteps.Run previous)
		{
			if (d == null || d.Dead || !d.Pending)
			{
				return;
			}
			try
			{
				d.Pending = false;
				((CanvasItem)d.Holder).Visible = true;
				d.Ui = StatusSteps.Activate(d.Ui, previous);
				Begin(d, d.Context, d.Stacks, d.EnergyPart, d.ChemPart, d.ExtraPart);
				d.Context = null;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Malaise queued scene: " + ex);
				Free(d);
			}
		}

		public static void Reset()
		{
			_resetting = true;
			try
			{
				Drain[] array = Live.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					Free(array[i]);
				}
				Live.Clear();
				Slump[] array2 = Slumps.ToArray();
				for (int i = 0; i < array2.Length; i++)
				{
					LetGo(array2[i]);
				}
				Slumps.Clear();
			}
			finally
			{
				_resetting = false;
			}
		}

		private static int XOf(CardModel card, Drain held)
		{
			try
			{
				return Math.Max(0, card.ResolveEnergyXValue());
			}
			catch (Exception ex)
			{
				DevLog.Warn("Malaise: could not read X: " + ex.Message);
				if (held == null)
				{
					return 0;
				}
				int num = 0;
				foreach (Orb orb in held.Orbs)
				{
					if (orb.Kind != BeadKind.Extra)
					{
						num++;
					}
				}
				return num;
			}
		}

		private static bool Upgraded(CardModel card)
		{
			try
			{
				return card != null && card.IsUpgraded;
			}
			catch
			{
				return false;
			}
		}

		internal static Drain Create(CardFxContext c, int energy, int chem, bool extra, bool fromCounter)
		{
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_0509: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_052d: Expected Obj, but got Unknown
			//IL_0580: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Expected Obj, but got Unknown
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c9: Expected Obj, but got Unknown
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Expected Obj, but got Unknown
			//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
			Node val;
			if (c.Remote && MultiplayerFx.Active)
			{
				val = c.FrontParent;
			}
			else
			{
				val = ((fromCounter && FxKit.Alive((GodotObject)(object)c.UiParent) && c.UiParent.IsInsideTree()) ? c.UiParent : c.FrontParent);
			}
			if (!FxKit.Alive((GodotObject)(object)val) || !val.IsInsideTree())
			{
				return null;
			}
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				if (Live[num].Dismissed && (!c.Remote || !MultiplayerFx.Active || Live[num].Remote))
				{
					Free(Live[num]);
				}
			}
			Drain drain = new Drain
			{
				FromCounter = (fromCounter && (!c.Remote || !MultiplayerFx.Active)),
				Remote = (c.Remote && MultiplayerFx.Active),
				Room = (Node)(object)GameRefs.CombatRoom,
				Card = c.Card,
				CardAt = c.Hand,
				Chest = c.Chest,
				Body = c.Body,
				Side = ((c.Facing < 0f) ? (-1f) : 1f),
				View = FogKit.ScaleOf(c.FrontParent),
				Bead = Mathf.Clamp(Tuning.F("malaise.bead", 24f), 8f, 60f),
				Grow = Mathf.Clamp(Tuning.F("malaise.bead_grow", 1.7f), 1f, 3f),
				Vary = Mathf.Clamp(Tuning.F("malaise.vary", 0.12f), 0f, 0.4f),
				Stand = Mathf.Clamp(Tuning.F("malaise.front", 70f), 0f, 400f),
				ZoneW = Mathf.Clamp(Tuning.F("malaise.zone_w", 130f), 20f, 600f),
				ZoneH = Mathf.Clamp(Tuning.F("malaise.zone_h", 150f), 20f, 600f),
				Pace = Mathf.Clamp(Tuning.F("malaise.wander", 1f), 0.1f, 4f),
				Apart = Mathf.Clamp(Tuning.F("malaise.apart", 56f), 0f, 200f),
				SmokeAlpha = Mathf.Clamp(Tuning.F("malaise.mist", 0.45f), 0f, 1f),
				SmokeEvery = Mathf.Clamp(Tuning.F("malaise.smoke_every", 0.3f), 0.05f, 5f),
				SmokeMax = Mathf.Clamp(Tuning.I("malaise.smoke_max", 12), 0, 40),
				MistSize = Mathf.Clamp(Tuning.F("malaise.mist_size", 1f), 0.3f, 3f),
				GlowK = Mathf.Clamp(Tuning.F("malaise.glow", 1f), 0f, 3f),
				EmberLevel = Mathf.Clamp(Tuning.F("malaise.glow_ember", 0.35f), 0f, 1f),
				Latch = Mathf.Clamp(Tuning.F("malaise.latch", 0.12f), 0.02f, 1f),
				Heaves = Mathf.Clamp(Tuning.I("malaise.heaves", 3), 1, 6),
				HeavePx = Mathf.Clamp(Tuning.F("malaise.heave_px", 14f), 0f, 80f),
				HeaveMore = Mathf.Clamp(Tuning.F("malaise.heave_more", 1.3f), 0.5f, 3f),
				HeavePull = Mathf.Clamp(Tuning.F("malaise.heave_pull", 0.17f), 0.04f, 1f),
				HeaveGrip = Mathf.Clamp(Tuning.F("malaise.heave_grip", 0.1f), 0.04f, 1f),
				HeaveBack = Mathf.Clamp(Tuning.F("malaise.heave_back", 0.45f), 0f, 1f),
				Whip = Mathf.Clamp(Tuning.F("malaise.whip", 0.16f), 0.05f, 1f),
				Coil = Mathf.Clamp(Tuning.F("malaise.coil", 2.2f), 0.3f, 5f),
				CoilTurns = Mathf.Clamp(Tuning.F("malaise.coil_turns", 5.5f), 1f, 15f),
				PuffN = Mathf.Clamp(Tuning.I("malaise.puffs", 36), 6, 80),
				PuffSize = Mathf.Clamp(Tuning.F("malaise.puff_size", 1f), 0.3f, 3f),
				StreamAlpha = Mathf.Clamp(Tuning.F("malaise.stream_alpha", 0.85f), 0f, 1f)
			};
			drain.Holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Malaise"),
				Visible = !drain.Remote
			};
			TimeFreeze.KeepLive((Node)(object)drain.Holder);
			val.AddChild((Node)(object)drain.Holder, false, (InternalMode)0);
			Live.Add(drain);
			if (!_ticking)
			{
				_ticking = true;
				Ticker.Add(Update);
			}
			drain.MistLayer = new Node2D
			{
				Name = StringName.op_Implicit("Mist")
			};
			((Node)drain.Holder).AddChild((Node)(object)drain.MistLayer, false, (InternalMode)0);
			drain.BeadLayer = new Node2D
			{
				Name = StringName.op_Implicit("Beads")
			};
			((Node)drain.Holder).AddChild((Node)(object)drain.BeadLayer, false, (InternalMode)0);
			drain.FrontLayer = new Node2D
			{
				Name = StringName.op_Implicit("MistFront")
			};
			((Node)drain.Holder).AddChild((Node)(object)drain.FrontLayer, false, (InternalMode)0);
			if (drain.FromCounter)
			{
				int max = Math.Max(0, MaxShown() - (extra ? 1 : 0));
				float gap = Tuning.F("skewer.bead_gap", 0.07f);
				drain.Beads = EnergyBeads.Open<Orb>(drain.Holder, energy, chem, max, gap, Mathf.Clamp(Tuning.F("skewer.cap_size", 1f), 0.3f, 3f), Mathf.Clamp(Tuning.F("skewer.cap_dark", 0.86f), 0f, 1f));
				if (drain.Beads != null)
				{
					int count = drain.Beads.Count;
					for (int i = 0; i < count; i++)
					{
						Adopt(drain, (Orb)drain.Beads.All[i]);
					}
					if (extra)
					{
						drain.CardAt = CardPoint(drain);
						drain.Extra = drain.Beads.AddExtra<Orb>(drain.CardAt, Purple, drain.Beads.NextLeave);
						Adopt(drain, drain.Extra);
					}
				}
			}
			if (drain.FromCounter)
			{
				Step(drain, 0f);
			}
			return drain;
		}

		private static void Adopt(Drain d, Orb o)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			o.Own = o.Tint;
			o.OwnInk = ((o.Kind == BeadKind.Chem) ? 1f : 0f);
			o.Turned = ((o.Kind == BeadKind.Extra) ? 1f : 0f);
			o.Index = d.Orbs.Count;
			o.Id = d.NextId++;
			o.BobA = Mathf.Lerp(0.8f, 1.25f, FxKit.Hash(o.Id * 29 + 3));
			o.BobB = Mathf.Lerp(0.8f, 1.25f, FxKit.Hash(o.Id * 31 + 5));
			o.Last = o.Pos;
			if (d.Beads != null)
			{
				d.Beads.Dress(o, (Node)(object)d.BeadLayer);
			}
			else
			{
				EnergyBeads.DressOn(o, (Node)(object)d.BeadLayer);
			}
			d.Orbs.Add(o);
		}

		private static Orb AddOrb(Drain d, BeadKind kind)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			Orb orb = new Orb
			{
				Kind = kind,
				Tint = kind switch
				{
					BeadKind.Extra => Purple, 
					BeadKind.Chem => EnergyBeads.Rim, 
					_ => EnergyBeads.Green, 
				},
				Out = 1f,
				Left = true,
				Show = 0f,
				Wait = 0.15f
			};
			Adopt(d, orb);
			return orb;
		}

		private static Vector2 CardPoint(Drain d)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (d.Remote || d.Card == null || !FxKit.Alive((GodotObject)(object)d.Holder))
				{
					return d.CardAt;
				}
				NCard val = HandCards.NodeOf(d.Card);
				if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree() || val.Model != d.Card)
				{
					return d.CardAt;
				}
				CardStandIn.Pose(val, out var centre, out var _, out var _);
				CanvasItem val2 = (CanvasItem)(object)((val is CanvasItem) ? val : null);
				if (val2 != null)
				{
					centre = EnergyBeads.Into(val2, (CanvasItem)(object)d.Holder, centre);
				}
				Vector2 val3 = FxKit.ViewSize();
				if (centre.X > -200f && centre.X < val3.X + 200f && centre.Y > -200f && centre.Y < val3.Y + 200f)
				{
					return centre;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Malaise: where the card is could not be told: " + ex.Message);
			}
			return d.CardAt;
		}

		internal static void Aim(Drain d, CardFxContext c)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			if (d != null && !d.Dead && !d.Closing && c.HasTarget)
			{
				bool num = !d.HasAim || d.Target != c.Target || d.TargetChest.DistanceTo(c.TargetChest) > 40f;
				d.Target = c.Target;
				d.TargetChest = c.TargetChest;
				d.TargetW = c.TargetWidth;
				d.TargetH = c.TargetHeight;
				d.Side = ((c.TargetSide < 0f) ? (-1f) : 1f);
				d.Chest = c.Chest;
				d.Body = c.Body;
				d.HasAim = true;
				d.Aimed = true;
				if (num || d.Mist == null)
				{
					NewMist(d);
				}
			}
		}

		internal static void Unaim(Drain d)
		{
			if (d != null && !d.Dead && !d.Playing && d.Aimed)
			{
				d.Aimed = false;
				LeaveMist(d);
			}
		}

		internal static void Dismiss(Drain d)
		{
			if (d != null && !d.Dead && !d.Playing && !d.Dismissed)
			{
				if (!d.FromCounter || d.Beads == null)
				{
					Close(d);
					return;
				}
				d.Dismissed = true;
				d.DismissAt = d.Time;
				d.Aimed = false;
				LeaveMist(d);
				d.Beads.SendHome(d.Time);
			}
		}

		internal static void Touch(Drain d)
		{
			if (d != null && !d.Dead)
			{
				d.OwnedAt = d.Time;
			}
		}

		private static void NewMist(Drain d)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Expected Obj, but got Unknown
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			LeaveMist(d);
			if (FxKit.Alive((GodotObject)(object)d.MistLayer))
			{
				int num = d.NextId++;
				MistGroup mistGroup = new MistGroup
				{
					Target = d.Target,
					Chest = d.TargetChest,
					W = d.TargetW * d.View,
					H = d.TargetH * d.View,
					Side = d.Side,
					Id = num,
					Seed = FxKit.Hash(num * 53 + 7) * 10f,
					NextSmoke = d.Time,
					Root = new Node2D
					{
						Name = StringName.op_Implicit("Strength")
					}
				};
				mistGroup.U = Mathf.Clamp(mistGroup.H / 390f, 0.55f, 1.2f) * d.MistSize;
				((Node)d.MistLayer).AddChild((Node)(object)mistGroup.Root, false, (InternalMode)0);
				mistGroup.Wide = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
				((Node)mistGroup.Root).AddChild((Node)(object)mistGroup.Wide, false, (InternalMode)0);
				mistGroup.Mid = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
				((Node)mistGroup.Root).AddChild((Node)(object)mistGroup.Mid, false, (InternalMode)0);
				mistGroup.Core = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
				((Node)mistGroup.Root).AddChild((Node)(object)mistGroup.Core, false, (InternalMode)0);
				mistGroup.Sheet = new MistSheet((Node)(object)mistGroup.Root, mistGroup.Seed);
				d.Mists.Add(mistGroup);
				d.Mist = mistGroup;
			}
		}

		private static void LeaveMist(Drain d)
		{
			if (d.Mist != null)
			{
				d.Mist.Leaving = true;
			}
			d.Mist = null;
		}

		private static void Begin(Drain d, CardFxContext c, int stacks, int energyPart, int chemPart, int extra)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_037a: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_038d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0302: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			if (c.Card != null)
			{
				d.Card = c.Card;
			}
			foreach (Orb orb5 in d.Orbs)
			{
				if (orb5.Out < 1f)
				{
					orb5.Goal = orb5.Pos;
				}
			}
			d.Beads?.Release();
			Aim(d, c);
			d.Playing = true;
			d.Aimed = true;
			d.PlayAt = d.Time;
			d.Stacks = stacks;
			d.Taken = 0;
			d.LastStepAt = -1f;
			d.EndAt = -1f;
			int num = MaxShown();
			int num2 = Math.Min(Math.Max(0, extra), num);
			int num3 = Math.Min(Math.Max(0, chemPart), num - num2);
			int want = Math.Min(Math.Max(0, energyPart), num - num2 - num3);
			d.Active.Clear();
			Pick(d, BeadKind.Energy, want);
			Pick(d, BeadKind.Chem, num3);
			Pick(d, BeadKind.Extra, num2);
			foreach (Orb orb6 in d.Orbs)
			{
				if (!d.Active.Contains(orb6))
				{
					orb6.Surplus = true;
				}
			}
			int count = d.Active.Count;
			if (count == 0)
			{
				Close(d);
				return;
			}
			float time = d.Time;
			for (int i = 0; i < count; i++)
			{
				Orb orb = d.Active[i];
				orb.Index = i;
				if (orb.Wait > 0f)
				{
					orb.Pos = EnergyBeads.Rest(d.Chest, d.Body, d.Side, i, count, time);
					orb.Last = orb.Pos;
					orb.Goal = orb.Pos;
					orb.Placed = true;
				}
			}
			if (d.Mist != null)
			{
				d.Mist.Total = count;
				d.Mist.Taken = 0;
				d.Mist.Notches = 0;
				d.Mist.TornAt = -100f;
			}
			float num4 = Mathf.Clamp(Tuning.F("malaise.span", 0.8f) / (float)count, Mathf.Max(0.02f, Tuning.F("malaise.gap_min", 0.1f)), Mathf.Max(0.02f, Tuning.F("malaise.gap", 0.16f)));
			float num5 = Mathf.Clamp(Tuning.F("malaise.lead", 0.1f), 0f, 2f);
			Zone(d, count, out var centre, out var half);
			bool flag = false;
			for (int j = 0; j < count; j++)
			{
				if (flag)
				{
					break;
				}
				Orb orb2 = d.Active[j];
				flag = orb2.Wait > 0f || orb2.Hopping || !orb2.Placed || !orb2.Wandering || orb2.Pos.DistanceTo(centre + orb2.Wp) > 90f * d.View;
			}
			if (d.Remote)
			{
				Zone(d, count, out half, out var half2);
				Wander(d, time, 0f, half2, count);
				for (int k = 0; k < count; k++)
				{
					Orb orb3 = d.Active[k];
					orb3.Pos = centre + orb3.Wp;
					orb3.Last = orb3.Pos;
					orb3.Goal = orb3.Pos;
					orb3.Placed = true;
					orb3.Wandering = true;
					orb3.Hopping = false;
				}
			}
			d.ReadyAt = time + (d.Remote ? d.Active[0].Wait : (flag ? Mathf.Clamp(Tuning.F("malaise.approach", 0.55f), 0f, 2f) : 0f));
			float num6 = d.ReadyAt + num5;
			float num7 = time;
			for (int l = 0; l < count; l++)
			{
				Orb orb4 = d.Active[l];
				orb4.State = 0;
				orb4.Fill = 0f;
				orb4.Torn = false;
				orb4.FullAt = -1f;
				float vary = d.Vary;
				orb4.Slow = 1f + (FxKit.Hash(orb4.Id * 61 + 17) - 0.5f) * 2f * vary;
				orb4.Might = 1f + (FxKit.Hash(orb4.Id * 67 + 19) - 0.5f) * 2.4f * vary;
				orb4.Arc = ((FxKit.Hash(orb4.Id * 71 + 23) < 0.5f) ? (-1f) : 1f) * Mathf.Lerp(0.6f, 1f, FxKit.Hash(orb4.Id * 73 + 29));
				orb4.DrinkAt = num6 + (float)l * num4 + ((l > 0) ? ((FxKit.Hash(orb4.Id * 79 + 31) - 0.5f) * 2f * vary * num4) : 0f);
				num7 = Mathf.Max(num7, TearAt(d, orb4));
			}
			Sfx.PlayAt("malaise_tear", num7 - time);
			bool flag2 = c.Card != null && c.Target != null;
			if (!d.Remote)
			{
				d.Ui = (flag2 ? StatusSteps.Begin(c.Target, Tuning.F("malaise.max_hold", 4f)) : null);
			}
			d.UiDone = d.Ui == null;
			d.SlumpHold = Mathf.Clamp(Tuning.F("malaise.slump_hold", 0.3f), 0f, 2f);
			d.Recover = Mathf.Clamp(Tuning.F("malaise.recover", 0.45f), 0.05f, 2f);
			float num8 = Mathf.Clamp(d.TargetH / 240f, 0.7f, 2f);
			d.Dip = Mathf.Clamp(Tuning.F("malaise.sink_px", 12f), 0f, 60f) * num8;
			d.Sag = Mathf.Clamp(Tuning.F("malaise.slump_px", 4f), 0f, 30f) * num8;
			d.Nudge = Mathf.Clamp(Tuning.F("malaise.heave_sink", 4f), 0f, 30f) * num8;
			d.Shudder = Mathf.Clamp(Tuning.F("malaise.shudder_px", 2f), 0f, 12f) * num8;
			if (flag2 && !TimeFreeze.Active && (d.Dip > 0.01f || d.Sag > 0.01f || d.Nudge > 0.01f || d.Shudder > 0.01f))
			{
				d.Slump = Seize(c.Target);
				if (d.Slump != null)
				{
					d.Slump.Most = d.Dip * 1.5f + d.Nudge * 1.5f + d.Sag * 6f;
				}
			}
			Step(d, 0f);
		}

		private static void Pick(Drain d, BeadKind kind, int want)
		{
			int i = 0;
			foreach (Orb orb in d.Orbs)
			{
				if (i >= want)
				{
					break;
				}
				if (orb.Kind == kind && !orb.Surplus && !orb.Arrived && !d.Active.Contains(orb))
				{
					d.Active.Add(orb);
					i++;
				}
			}
			for (; i < want; i++)
			{
				d.Active.Add(AddOrb(d, kind));
			}
		}

		private static void Drink(Drain d, float time)
		{
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			if (!d.Cut && d.Target != null)
			{
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(d.Target) : null);
				if (!d.Target.IsAlive || !FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree())
				{
					CutShort(d, time);
				}
			}
			int count = d.Active.Count;
			bool flag = count > 0;
			bool flag2 = count > 0;
			float num = -1f;
			for (int i = 0; i < count; i++)
			{
				Orb orb = d.Active[i];
				if (orb.State == 0 && time >= orb.DrinkAt)
				{
					Turn(d, orb, time);
				}
				if (orb.State == 1)
				{
					if (!orb.Bit && time >= orb.DrinkAt + d.Latch)
					{
						orb.Bit = true;
						if (FxKit.Alive((GodotObject)(object)d.BeadLayer))
						{
							FxKit.Flash((Node)(object)d.BeadLayer, orb.Pos, A(Fleck, 0.45f), d.Bead * orb.Grow * 1.7f, 1.6f, 0.12);
						}
					}
					while (orb.Heaved < d.Heaves && time >= HeaveAt(d, orb, orb.Heaved))
					{
						orb.Heaved++;
						Wrench(d, orb, time);
					}
					while (!orb.Torn && orb.Gulped < orb.Heaved && time >= GulpAt(d, orb, orb.Gulped))
					{
						orb.Gulped++;
						if (FxKit.Alive((GodotObject)(object)d.BeadLayer))
						{
							FxKit.RingPulse((Node)(object)d.BeadLayer, orb.Pos, d.Bead * orb.Grow * 2.6f, Purple, 0f, 0.26f, 0.55f, 1.35f, 1f, 0f, 0.035f, 0.25f + 0.1f * (float)orb.Gulped);
						}
					}
					if (!orb.Torn && time >= TearAt(d, orb))
					{
						Tear(d, orb, time);
					}
					if (orb.Torn && time >= orb.TornAt + d.Whip)
					{
						Full(d, orb, time);
					}
					if (orb.State == 1 && !orb.Torn)
					{
						num = Mathf.Max(num, Tension(d, orb, time));
					}
				}
				else if (orb.State == 2 && time - orb.GreyAt >= 0.24f)
				{
					Crumble(d, orb);
				}
				if (orb.State != 3)
				{
					flag = false;
				}
				if (orb.State == 0 || orb.State == 1)
				{
					flag2 = false;
				}
			}
			if (num >= 0f && !d.Cut)
			{
				Sfx.Hold("malaise_drink", 0.35f + 0.65f * num);
			}
			if (flag2 && d.Mist != null)
			{
				LeaveMist(d);
			}
			if (flag && d.EndAt < 0f)
			{
				float num2 = ((d.LastStepAt >= 0f) ? (d.LastStepAt + d.SlumpHold + d.Recover + 0.1f) : time);
				d.EndAt = Mathf.Max(time + 1.8f, num2);
			}
			if (d.LastStepAt >= 0f && !d.UiDone && time - d.LastStepAt >= 0.35f)
			{
				d.UiDone = true;
				if (d.Remote && Next(d) != null)
				{
					StatusSteps.Suspend(d.Ui);
				}
				else
				{
					StatusSteps.Finish(d.Ui);
				}
			}
			if (d.Slump != null)
			{
				Sink(d, time);
			}
		}

		private static void Turn(Drain d, Orb o, float time)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			o.State = 1;
			o.Fill = 0f;
			o.Torn = false;
			o.Bit = false;
			o.Heaved = 0;
			o.Gulped = 0;
			o.Hold = o.Wp;
			if (o.Flare == null && FxKit.Alive((GodotObject)(object)d.BeadLayer))
			{
				o.Flare = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
				((Node)d.BeadLayer).AddChild((Node)(object)o.Flare, false, (InternalMode)0);
			}
			MistGroup mist = d.Mist;
			if (mist != null && !mist.Leaving)
			{
				o.Stream = MakeStream(d, o, mist, time);
				if (o.Stream != null)
				{
					d.Streams.Add(o.Stream);
				}
			}
		}

		private static void Wrench(Drain d, Orb o, float time)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			Stream stream = o.Stream;
			MistGroup mistGroup = ((stream != null) ? stream.Group : d.Mist);
			if (mistGroup != null)
			{
				mistGroup.Notches++;
			}
			Tug(d, time);
			if (stream != null && !stream.Torn && !stream.Cut && FxKit.Alive((GodotObject)(object)d.FrontLayer))
			{
				float u = stream.U;
				Vector2 val = o.Pos - stream.From;
				Vector2 val2 = (Vector2)((val.LengthSquared() > 1f) ? val.Normalized() : new Vector2(0f - d.Side, 0f));
				FxKit.Burst((Node)(object)d.FrontLayer, stream.From + val2 * (4f * u), Fleck, 2 + o.Heaved, 30f * u, 110f * u, 0.3, 0.03f * u, 0.06f * u, new Vector2(0f, 200f), 55f, val2, 4f * u, 2.5f, additive: true, 0.85f);
			}
		}

		private static void Tear(Drain d, Orb o, float time)
		{
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			o.Torn = true;
			o.TornAt = time;
			d.Taken++;
			int count = d.Active.Count;
			int steps = ((count > 0) ? Mathf.RoundToInt((float)d.Stacks * (float)d.Taken / (float)count) : d.Stacks);
			StatusSteps.StepTo(d.Ui, steps);
			Kick(d, time);
			MistGroup mistGroup = ((o.Stream != null) ? o.Stream.Group : d.Mist);
			Vector2 val;
			if (o.Stream != null)
			{
				val = o.Stream.From;
			}
			else
			{
				val = ((mistGroup != null) ? (mistGroup.Chest + Sunk(d, mistGroup)) : d.TargetChest);
			}
			float num = mistGroup?.U ?? 1f;
			if (mistGroup != null)
			{
				mistGroup.Taken++;
				mistGroup.TornAt = time;
			}
			if (o.Stream != null)
			{
				TearStream(o.Stream, time);
			}
			Vector2 val2 = o.Pos - val;
			Vector2 val3 = (Vector2)((val2.LengthSquared() > 1f) ? val2.Normalized() : new Vector2(0f - d.Side, 0f));
			if (FxKit.Alive((GodotObject)(object)d.FrontLayer))
			{
				FxKit.Flash((Node)(object)d.FrontLayer, val + val3 * (6f * num), new Color(1f, 0.86f, 0.7f, 0.85f), 46f * num, 1.8f, 0.16);
				FxKit.RingPulse((Node)(object)d.FrontLayer, val, 44f * num, Fleck, 0f, 0.22f, 0.4f, 1.4f, 1f, 0f, 0.05f, 0.5f);
				FxKit.Burst((Node)(object)d.FrontLayer, val + val3 * (10f * num), Fleck, 7, 35f * num, 140f * num, 0.4, 0.04f * num, 0.08f * num, new Vector2(0f, 260f), 140f, val3, 6f * num, 3f, additive: true, 0.9f);
			}
			if (d.Taken >= count)
			{
				d.LastStepAt = time;
			}
		}

		private static void Full(Drain d, Orb o, float time)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			o.State = 2;
			o.GreyAt = time;
			o.FullAt = time;
			o.Fill = 1f;
			if (FxKit.Alive((GodotObject)(object)d.BeadLayer))
			{
				float num = d.Bead * o.Grow;
				FxKit.Flash((Node)(object)d.BeadLayer, o.Pos, A(Gorged, 0.35f), num * 2f, 1.5f, 0.18);
				FxKit.RingPulse((Node)(object)d.BeadLayer, o.Pos, num * 3.6f, new Color(0.9f, 0.32f, 0.62f, 1f), 0f, 0.34f, 0.45f, 1.5f, 1f, 0f, 0.04f, 0.55f);
			}
		}

		private static void Crumble(Drain d, Orb o)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			o.State = 3;
			o.Show = 0f;
			Node2D holder = d.Holder;
			if (FxKit.Alive((GodotObject)(object)holder))
			{
				Vector2 pos = o.Pos;
				float num = d.Bead * o.Grow;
				float num2 = num / 24f;
				FxKit.Flash((Node)(object)holder, pos, A(Ash, 0.3f), num * 2f, 1.5f, 0.2);
				FxKit.Burst((Node)(object)holder, pos, A(Ash, 0.9f), 16, 25f * num2, 110f * num2, 1.1, 0.05f * num2, 0.12f * num2, new Vector2(0f, 120f), 180f, new Vector2(0f, 1f), num * 0.35f, 2.5f, additive: false, 0.9f);
				FxKit.Burst((Node)(object)holder, pos, new Color(0.36f, 0.35f, 0.38f, 0.85f), 6, 8f * num2, 45f * num2, 1.4, 0.11f * num2, 0.2f * num2, new Vector2(0f, 70f), 70f, new Vector2(0f, 1f), num * 0.3f, 1.5f, additive: false, 0.7f);
			}
		}

		private static void CutShort(Drain d, float time)
		{
			d.Cut = true;
			foreach (Orb item in d.Active)
			{
				if (item.State == 0 || item.State == 1)
				{
					item.State = 2;
					item.GreyAt = time;
				}
			}
			foreach (Stream stream in d.Streams)
			{
				stream.Cut = true;
			}
			if (!d.UiDone)
			{
				d.UiDone = true;
				StatusSteps.Finish(d.Ui);
			}
			LetGo(d.Slump);
			d.Slump = null;
			LeaveMist(d);
			if (d.LastStepAt < 0f)
			{
				d.LastStepAt = time;
			}
		}

		private static void Close(Drain d)
		{
			if (d == null || d.Dead || d.Closing)
			{
				return;
			}
			d.Closing = true;
			d.CloseAt = d.Time;
			d.Aimed = false;
			LeaveMist(d);
			foreach (Stream stream in d.Streams)
			{
				stream.Cut = true;
			}
			if (!d.UiDone)
			{
				d.UiDone = true;
				StatusSteps.Finish(d.Ui);
			}
			LetGo(d.Slump);
			d.Slump = null;
		}

		private static void Free(Drain d)
		{
			if (d == null || d.Dead)
			{
				return;
			}
			d.Dead = true;
			object obj;
			if (d.Remote && !_resetting && MultiplayerFx.Active && PlayerSettings.Enabled && (object)d.Room == GameRefs.CombatRoom)
			{
				Creature target = d.Target;
				if (target != null && target.IsAlive)
				{
					obj = Next(d);
					goto IL_005a;
				}
			}
			obj = null;
			goto IL_005a;
			IL_005a:
			Drain drain = (Drain)obj;
			try
			{
				LetGo(d.Slump);
			}
			catch (Exception ex)
			{
				DevLog.Warn("Malaise cleanup (victim): " + ex);
			}
			d.Slump = null;
			try
			{
				if (drain != null && drain.GameDone && Prior(drain) == null)
				{
					Start(drain, d.Ui);
				}
				StatusSteps.Finish(d.Ui);
			}
			catch (Exception ex2)
			{
				DevLog.Warn("Malaise cleanup (icons): " + ex2);
			}
			d.UiDone = true;
			d.Streams.Clear();
			try
			{
				if (FxKit.Alive((GodotObject)(object)d.Holder))
				{
					((Node)d.Holder).QueueFree();
				}
			}
			catch (Exception ex3)
			{
				DevLog.Warn("Malaise cleanup (nodes): " + ex3);
			}
			try
			{
				d.Beads?.Free();
			}
			catch (Exception ex4)
			{
				DevLog.Warn("Malaise cleanup (beads): " + ex4);
			}
		}

		private static Slump Seize(Creature who)
		{
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (who == null || !who.IsAlive)
				{
					return null;
				}
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(who) : null);
				if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree())
				{
					return null;
				}
				Node2D body = val.Body;
				if (!FxKit.Alive((GodotObject)(object)body) || !((CanvasItem)body).Visible)
				{
					return null;
				}
				Slump[] array = Slumps.ToArray();
				foreach (Slump slump in array)
				{
					if (slump.Picture == body)
					{
						LetGo(slump);
					}
				}
				Slump slump2 = new Slump
				{
					Who = who,
					Picture = body,
					Home = body.Position,
					Wrote = body.Position
				};
				Slumps.Add(slump2);
				return slump2;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Malaise (victim): " + ex.Message);
				return null;
			}
		}

		private static void Kick(Drain d, float time)
		{
			Slump slump = d.Slump;
			if (slump != null && !slump.Over && !slump.Rising)
			{
				slump.Rest = Mathf.Min(slump.Rest + d.Sag * 0.5f, d.Sag * 6f);
				slump.Speed += d.Dip * 2f * Mathf.Sqrt(220f);
				Shudder(slump, d.Shudder * 1.6f, time);
			}
		}

		private static void Tug(Drain d, float time)
		{
			Slump slump = d.Slump;
			if (slump != null && !slump.Over && !slump.Rising)
			{
				slump.Rest = Mathf.Min(slump.Rest + d.Sag * 0.5f / (float)Math.Max(1, d.Heaves), d.Sag * 6f);
				slump.Speed += d.Nudge * 2f * Mathf.Sqrt(220f);
				Shudder(slump, d.Shudder, time);
			}
		}

		private static void Shudder(Slump s, float most, float time)
		{
			s.Shake = most * Mathf.Lerp(0.8f, 1.2f, FxKit.Hash(s.Kicks * 37 + 5));
			s.ShakeSeed = FxKit.Hash(s.Kicks * 41 + 11) * 10f;
			s.ShakeAt = time;
			s.Kicks++;
		}

		private static void Sink(Drain d, float time)
		{
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			Slump slump = d.Slump;
			if (slump.Over)
			{
				d.Slump = null;
				return;
			}
			if (!slump.Rising && d.LastStepAt >= 0f && time - d.LastStepAt >= d.SlumpHold)
			{
				slump.Rising = true;
				slump.RiseAt = time;
				slump.RiseFrom = slump.Y;
				slump.Speed = 0f;
			}
			if (slump.Rising)
			{
				float num = Mathf.Clamp((time - slump.RiseAt) / Mathf.Max(0.05f, d.Recover), 0f, 1f);
				slump.Y = slump.RiseFrom * (1f - num * num * (3f - 2f * num));
				if (num >= 1f)
				{
					LetGo(slump);
					d.Slump = null;
					return;
				}
			}
			Vector2 val = Vector2.Zero;
			float num2 = time - slump.ShakeAt;
			if (slump.Shake > 0f && num2 >= 0f && num2 < 0.4f)
			{
				float num3 = slump.Shake * Mathf.Exp((0f - num2) / 0.08f) * (1f - Mathf.Exp((0f - num2) / 0.012f));
				float shakeSeed = slump.ShakeSeed;
				val = new Vector2(0.4f * num3 * (0.6f * Mathf.Sin(num2 * 97f + shakeSeed) + 0.4f * Mathf.Sin(num2 * 151f + shakeSeed * 2.3f)), num3 * (0.65f * Mathf.Sin(num2 * 83f + shakeSeed * 1.7f) + 0.35f * Mathf.Sin(num2 * 139f + shakeSeed * 3.1f)));
			}
			slump.Off = new Vector2(val.X, slump.Y + val.Y);
			Push(slump, slump.Off);
		}

		private static void Spring(Slump s, float dt)
		{
			if (s != null && !s.Over && !s.Rising && !(dt <= 0f))
			{
				int num = Mathf.Clamp(Mathf.CeilToInt(dt / 0.006f), 1, 24);
				float num2 = dt / (float)num;
				for (int i = 0; i < num; i++)
				{
					s.Speed += (-220f * (s.Y - s.Rest) - 21.4f * s.Speed) * num2;
					s.Y += s.Speed * num2;
				}
				if (!float.IsFinite(s.Y) || !float.IsFinite(s.Speed))
				{
					s.Y = s.Rest;
					s.Speed = 0f;
				}
				float num3 = Mathf.Max(1f, s.Most);
				s.Y = Mathf.Clamp(s.Y, (0f - num3) * 0.3f, num3);
			}
		}

		private static void Push(Slump s, Vector2 screen)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			if (s == null || s.Over)
			{
				return;
			}
			Node2D picture = s.Picture;
			if (picture == null)
			{
				return;
			}
			if (!FxKit.Alive((GodotObject)(object)picture))
			{
				s.Picture = null;
				return;
			}
			if (s.Who != null && !s.Who.IsAlive)
			{
				LetGo(s);
				return;
			}
			Vector2 val = Vector2.Zero;
			if (screen != Vector2.Zero && ((Node)picture).IsInsideTree())
			{
				Node parent = ((Node)picture).GetParent();
				CanvasItem val2 = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
				if (val2 != null && FxKit.Alive((GodotObject)(object)val2))
				{
					Transform2D val3 = val2.GetGlobalTransform();
					val3 = val3.AffineInverse();
					val = val3.BasisXform(screen);
					if (!float.IsFinite(val.X) || !float.IsFinite(val.Y) || val.LengthSquared() > 160000f)
					{
						val = Vector2.Zero;
					}
				}
			}
			if (!(val == s.Shift))
			{
				Vector2 position = picture.Position;
				if (position != s.Wrote)
				{
					s.Home = position - s.Shift;
				}
				Vector2 wrote = (picture.Position = ((val == Vector2.Zero) ? s.Home : (s.Home + val)));
				s.Wrote = wrote;
				s.Shift = val;
			}
		}

		private static void LetGo(Slump s)
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			if (s == null || s.Over)
			{
				return;
			}
			s.Over = true;
			Slumps.Remove(s);
			try
			{
				Node2D picture = s.Picture;
				if (FxKit.Alive((GodotObject)(object)picture) && s.Shift != Vector2.Zero)
				{
					Vector2 position = picture.Position;
					picture.Position = ((position != s.Wrote) ? (position - s.Shift) : s.Home);
				}
			}
			catch
			{
			}
			s.Picture = null;
			s.Shift = Vector2.Zero;
		}

		private static Vector2 Sunk(Drain d, MistGroup g)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			Slump slump = d.Slump;
			if (slump == null || slump.Over || g == null || slump.Who != g.Target)
			{
				return Vector2.Zero;
			}
			return slump.Off;
		}

		private static void Zone(Drain d, int n, out Vector2 centre, out Vector2 half)
		{
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			float view = d.View;
			float num = d.ZoneW * 0.5f;
			float num2 = d.ZoneH * 0.5f;
			float num3 = ApartOf(d, n);
			float num4 = 0.61f * (float)Mathf.Max(1, n) * num3 * num3;
			float num5 = Mathf.Max(1f, num4 / Mathf.Max(1f, num * num2));
			float num6 = Mathf.Min(Mathf.Sqrt(num5), 1.55f);
			num *= num5 / num6 * view;
			num2 *= num6 * view;
			float num7 = Mathf.Max(d.TargetW, 100f) * 0.5f * view;
			float num8 = d.Stand * view;
			Vector2 val = FxKit.ViewSize();
			float num9 = d.TargetChest.X - d.Side * num7;
			float num10 = ((d.Side > 0f) ? num9 : (val.X - num9)) - 40f;
			if (num8 + 2.3f * num > num10)
			{
				num8 = Mathf.Max(num8 * 0.4f, num10 - 2.3f * num);
				float num11 = Mathf.Max(45f * view, (num10 - num8) / 2.3f);
				if (num11 < num)
				{
					num2 *= Mathf.Min(1.4f, Mathf.Sqrt(num / num11));
					num = num11;
				}
			}
			float num12 = 90f + 1.15f * num2;
			float num13 = val.Y - 150f - 1.15f * num2;
			float num14 = Mathf.Clamp(d.TargetChest.Y - 0.03f * d.TargetH * view, num12, Mathf.Max(num12, num13));
			half = new Vector2(num, num2);
			centre = new Vector2(num9 - d.Side * (num8 + num), num14);
		}

		private static float GrowOf(Drain d, int n)
		{
			return Mathf.Lerp(d.Grow, 1f + (d.Grow - 1f) * 0.5f, Mathf.Clamp((float)(n - 4) / 5f, 0f, 1f));
		}

		private static float ApartOf(Drain d, int n)
		{
			return d.Apart * (0.45f + 0.55f * GrowOf(d, n));
		}

		private static bool Strolls(Orb o)
		{
			if (!o.Surplus && o.State != 3 && !o.Arrived && o.Root != null)
			{
				return FxKit.Alive((GodotObject)(object)o.Root);
			}
			return false;
		}

		private static float Roll(Orb o)
		{
			return FxKit.Hash(o.Id * 131 + 7 + o.Rolls++ * 17);
		}

		private static Vector2 FirstSpot(Orb o, int n)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			float num = (float)o.Index * 2.39996f + FxKit.Hash(o.Id * 7 + 1) * ((float)Math.PI * 2f);
			float num2 = Mathf.Sqrt(((float)o.Index + 0.5f) / (float)Math.Max(1, n)) * 0.85f;
			return new Vector2(Mathf.Cos(num) * num2, Mathf.Sin(num) * num2);
		}

		private static void PickSpot(Drain d, Orb o, Vector2 half, float time)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			Vector2 spot = o.Spot;
			float num = -1E+09f;
			for (int i = 0; i < 4; i++)
			{
				Vector2 val = new Vector2(Roll(o) * 1.7f - 0.85f, Roll(o) * 1.7f - 0.85f);
				Vector2 val2 = new Vector2(val.X * half.X, val.Y * half.Y);
				float num2 = 1000000f;
				foreach (Orb orb in d.Orbs)
				{
					if (orb != o && Strolls(orb) && orb.Wandering)
					{
						Vector2 val3 = (Vector2)((orb.State == 0) ? new Vector2(orb.Spot.X * half.X, orb.Spot.Y * half.Y) : orb.Hold);
						num2 = Mathf.Min(num2, Mathf.Min(val2.DistanceTo(orb.Wp), val2.DistanceTo(val3)));
					}
				}
				num2 -= 0.25f * val2.DistanceTo(o.Wp);
				if (num2 > num)
				{
					num = num2;
					spot = val;
				}
			}
			o.Spot = spot;
			o.SpotAt = time + Mathf.Lerp(1.6f, 3.2f, Roll(o)) / d.Pace;
		}

		private static void Wander(Drain d, float time, float dt, Vector2 half, int n)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_035a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			foreach (Orb orb in d.Orbs)
			{
				if (Strolls(orb))
				{
					if (!orb.Wandering)
					{
						orb.Wandering = true;
						orb.Spot = FirstSpot(orb, n);
						orb.Wp = new Vector2(orb.Spot.X * half.X, orb.Spot.Y * half.Y);
						orb.Wv = Vector2.Zero;
						orb.Hold = orb.Wp;
						orb.SpotAt = time + (0.4f + 1.2f * Roll(orb)) / d.Pace;
					}
					else if (orb.State == 0 && time >= orb.SpotAt)
					{
						PickSpot(d, orb, half, time);
					}
				}
			}
			if (dt <= 0f)
			{
				return;
			}
			float num = 1.35f * d.Pace;
			float num2 = 1.84f * num;
			float num3 = ApartOf(d, n) * d.View;
			Vector2 val = half * 1.15f;
			int num4 = Mathf.Clamp(Mathf.CeilToInt(dt * 120f), 1, 12);
			float num5 = dt / (float)num4;
			for (int i = 0; i < num4; i++)
			{
				foreach (Orb orb2 in d.Orbs)
				{
					if (!orb2.Wandering || !Strolls(orb2))
					{
						continue;
					}
					Vector2 val2 = (Vector2)((orb2.State == 0) ? new Vector2(orb2.Spot.X * half.X, orb2.Spot.Y * half.Y) : orb2.Hold);
					orb2.Wv += ((val2 - orb2.Wp) * (num * num) - orb2.Wv * num2) * num5;
					orb2.Wp += orb2.Wv * num5;
					if (orb2.State == 0 && num3 > 0.5f)
					{
						foreach (Orb orb3 in d.Orbs)
						{
							if (orb3 != orb2 && orb3.Wandering && Strolls(orb3))
							{
								Vector2 val3 = orb2.Wp - orb3.Wp;
								float num6 = val3.Length();
								if (!(num6 >= num3))
								{
									Vector2 val4 = (Vector2)((num6 > 0.01f) ? (val3 / num6) : new Vector2(Mathf.Cos((float)orb2.Id * 2.4f), Mathf.Sin((float)orb2.Id * 2.4f)));
									orb2.Wp += val4 * ((num3 - num6) * 5f * num5);
								}
							}
						}
					}
					float num7 = orb2.Wp.X;
					float num8 = orb2.Wp.Y;
					float num9 = orb2.Wv.X;
					float num10 = orb2.Wv.Y;
					if (num7 > val.X || num7 < 0f - val.X)
					{
						num7 = ((num7 > 0f) ? val.X : (0f - val.X));
						if (num9 * num7 > 0f)
						{
							num9 = 0f;
						}
					}
					if (num8 > val.Y || num8 < 0f - val.Y)
					{
						num8 = ((num8 > 0f) ? val.Y : (0f - val.Y));
						if (num10 * num8 > 0f)
						{
							num10 = 0f;
						}
					}
					orb2.Wp = new Vector2(num7, num8);
					orb2.Wv = new Vector2(num9, num10);
				}
			}
		}

		private static float Pull(Drain d, Orb o)
		{
			return d.HeavePull * o.Slow;
		}

		private static float Grip(Drain d, Orb o)
		{
			return d.HeaveGrip * o.Slow;
		}

		private static float HeaveAt(Drain d, Orb o, int k)
		{
			return o.DrinkAt + d.Latch + (float)k * (Pull(d, o) + Grip(d, o));
		}

		private static float WaveIn(Drain d, Orb o)
		{
			return Mathf.Min(0.02f + 0.8f * Pull(d, o), Pull(d, o) - 0.01f);
		}

		private static float GulpAt(Drain d, Orb o, int k)
		{
			return HeaveAt(d, o, k) + WaveIn(d, o);
		}

		private static float TearAt(Drain d, Orb o)
		{
			return HeaveAt(d, o, d.Heaves - 1) + Pull(d, o);
		}

		private static float PullOf(Drain d, Orb o, int k)
		{
			return d.HeavePx * Mathf.Pow(d.HeaveMore, (float)k) * o.Might * d.View;
		}

		private static float Springy(float tau, float w, float zeta)
		{
			if (tau <= 0f)
			{
				return 0f;
			}
			float num = w * Mathf.Sqrt(1f - zeta * zeta);
			return 1f - Mathf.Exp((0f - zeta) * w * tau) * (Mathf.Cos(num * tau) + zeta * w / num * Mathf.Sin(num * tau));
		}

		private static Vector2 Heave(Drain d, Orb o, float t)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			if (o.State != 1 && o.State != 2)
			{
				return Vector2.Zero;
			}
			float num = 20f / Mathf.Max(0.5f, o.Slow);
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			for (int i = 0; i < d.Heaves; i++)
			{
				float num6 = HeaveAt(d, o, i);
				if (t <= num6)
				{
					return new Vector2(num2, num3);
				}
				float num7 = PullOf(d, o, i);
				float num8 = o.Arc * 0.3f * num7;
				num2 += (num7 - num4) * Springy(t - num6, num, 0.5f);
				num3 += (num8 - num5) * Springy(t - num6, num * 0.7f, 0.7f);
				num4 = num7;
				num5 = num8;
				if (i == d.Heaves - 1)
				{
					break;
				}
				float num9 = num7 * (1f - d.HeaveBack);
				float num10 = num8 * 0.3f;
				float num11 = num6 + Pull(d, o);
				num2 += (num9 - num4) * Springy(t - num11, num * 0.8f, 0.75f);
				num3 += (num10 - num5) * Springy(t - num11, num * 0.6f, 0.75f);
				num4 = num9;
				num5 = num10;
			}
			float num12 = TearAt(d, o);
			if (t > num12)
			{
				float num13 = num4 * 0.5f;
				num2 += num13 * Springy(t - num12, num * 1.5f, 0.5f) - (num4 + num13) * Springy(t - num12 - 0.07f, 9f, 0.75f);
				num3 -= num5 * Springy(t - num12, 7f, 0.8f);
			}
			return new Vector2(num2, num3);
		}

		private static float Surge(Drain d, Orb o, float t)
		{
			float num = Mathf.Max(0.01f, WaveIn(d, o) - 0.02f);
			float num2 = 0f;
			for (int i = 0; i < d.Heaves; i++)
			{
				float num3 = (t - HeaveAt(d, o, i) - 0.02f - 0.7f * num) / Mathf.Max(0.02f, 0.45f * num);
				num2 += Mathf.Exp((0f - num3) * num3);
			}
			return Mathf.Min(1f, num2);
		}

		private static float Tension(Drain d, Orb o, float t)
		{
			float num = 0f;
			for (int i = 0; i < d.Heaves; i++)
			{
				float num2 = (t - HeaveAt(d, o, i)) / Pull(d, o);
				if (num2 < 0f)
				{
					break;
				}
				num = ((!(num2 <= 1f) && i != d.Heaves - 1) ? Mathf.Max(num, 1f - Smooth((num2 - 1f) * Pull(d, o) / (0.6f * Grip(d, o)))) : Mathf.Max(num, Smooth(num2 / 0.3f)));
			}
			return num;
		}

		private static float Pulse(Drain d, Orb o, float t)
		{
			float num = 0f;
			for (int i = 0; i < d.Heaves; i++)
			{
				float num2 = t - HeaveAt(d, o, i);
				if (num2 >= 0f)
				{
					num = Mathf.Max(num, Mathf.Exp((0f - num2) / 0.12f));
				}
			}
			return num;
		}

		private static float Gulp(Drain d, Orb o, float t)
		{
			float num = 0f;
			for (int i = 0; i < d.Heaves; i++)
			{
				float num2 = GulpAt(d, o, i);
				if (o.Torn && num2 > o.TornAt)
				{
					break;
				}
				float num3 = t - num2;
				if (num3 >= 0f)
				{
					num = Mathf.Max(num, Mathf.Exp((0f - num3) / 0.1f));
				}
			}
			return num;
		}

		private static float FillAt(Drain d, Orb o, float t)
		{
			float num = 0f;
			for (int i = 0; i < d.Heaves; i++)
			{
				float num2 = GulpAt(d, o, i);
				if (o.Torn && num2 > o.TornAt)
				{
					break;
				}
				num += Smooth((t - num2) / 0.1f);
			}
			num *= 0.55f / (float)d.Heaves;
			if (o.Torn)
			{
				num = Mathf.Lerp(num, 1f, Smooth((t - o.TornAt) / Mathf.Max(0.05f, d.Whip)));
			}
			return num;
		}

		private static Vector2 Stroll(Drain d, Orb o, Vector2 centre, float time)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			float view = d.View;
			Vector2 val = centre + o.Wp;
			float num = ((o.State == 0) ? 1f : Mathf.Lerp(1f, 0.35f, Smooth((time - o.DrinkAt) / 0.15f)));
			Vector2 val2 = val + new Vector2(1.3f * Mathf.Sin((float)Math.PI * 9f / 10f * o.BobA * time + (float)o.Id * 1.7f), 2.8f * Mathf.Sin(3.895575f * o.BobB * time + (float)o.Id * 2.9f)) * (num * view);
			Vector2 val3 = val2 - d.TargetChest;
			val3 = (Vector2)((val3.LengthSquared() > 1f) ? val3.Normalized() : new Vector2(0f - d.Side, 0f));
			float num2 = ((o.State == 1 && !o.Torn) ? Tension(d, o, time) : 0f);
			Vector2 val4 = new Vector2(Mathf.Sin(time * 61f + (float)o.Id), Mathf.Cos(time * 53f + (float)o.Id * 2f)) * (1.8f * num2 * view);
			Vector2 val5 = Heave(d, o, time);
			Vector2 val6 = new Vector2(0f - val3.Y, val3.X);
			return val2 + val3 * val5.X + val6 * val5.Y + new Vector2(0f, 0.15f * val5.X) + val4;
		}

		private static Vector2 Place(Orb o, Vector2 slot, bool snap, float time, float dt)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			if (!o.Placed)
			{
				o.Goal = slot;
				return EnergyBeads.Settle(o, slot, placed: false, dt);
			}
			Vector2 val = slot - o.Goal;
			if (val.LengthSquared() > 4900f)
			{
				o.Hopping = true;
				o.HopFrom = o.Pos;
				o.HopAt = time + (float)o.Index * 0.035f + o.Wait;
				o.HopDur = 0.42f;
				o.Wait = 0f;
			}
			o.Goal = slot;
			if (!o.Hopping)
			{
				if (!snap)
				{
					return EnergyBeads.Settle(o, slot, placed: true, dt);
				}
				float num = o.Pos.DistanceSquaredTo(slot);
				if (o.State == 0 && num > 36f)
				{
					return EnergyBeads.Settle(o, slot, placed: true, dt);
				}
				o.Show = FxKit.Damp(o.Show, 1f, 16f, dt);
				if (o.State != 0 && num > 36f && time - o.DrinkAt < 0.1f)
				{
					return o.Pos.Lerp(slot, 1f - Mathf.Exp(-30f * dt));
				}
				return slot;
			}
			o.Show = FxKit.Damp(o.Show, 1f, 16f, dt);
			float num2 = (time - o.HopAt) / Mathf.Max(0.05f, o.HopDur);
			if (num2 <= 0f)
			{
				return o.Pos;
			}
			if (num2 >= 1f)
			{
				o.Hopping = false;
				return slot;
			}
			float t = num2 * num2 * (3f - 2f * num2);
			float num3 = o.HopFrom.DistanceTo(slot);
			Vector2 b = o.HopFrom.Lerp(slot, 0.5f) + new Vector2(0f, 0f - Mathf.Min(130f, num3 * 0.28f));
			return FxKit.Bezier(o.HopFrom, b, slot, t);
		}

		private static float HopDone(Orb o, float time)
		{
			if (!o.Hopping)
			{
				return 1f;
			}
			return Mathf.Clamp((time - o.HopAt) / Mathf.Max(0.05f, o.HopDur), 0f, 1f);
		}

		private static void Track(Drain d)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (d.FromCounter && !d.Playing)
				{
					d.Beads?.Track();
					if (d.Extra != null)
					{
						d.CardAt = CardPoint(d);
						d.Extra.Start = d.CardAt;
					}
				}
				if (d.Target != null && d.HasAim && !d.Playing)
				{
					NCombatRoom combatRoom = GameRefs.CombatRoom;
					NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(d.Target) : null);
					if (FxKit.Alive((GodotObject)(object)val) && ((Node)val).IsInsideTree())
					{
						d.TargetChest = val.VfxSpawnPosition;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Malaise track: " + ex.Message);
			}
		}

		private static void Update(double realDt)
		{
			if (Live.Count == 0)
			{
				return;
			}
			float dt = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				Drain drain = Live[num];
				try
				{
					if (!drain.Dead)
					{
						Step(drain, dt);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Malaise failed and was removed: " + ex);
					Free(drain);
				}
				if (drain.Dead)
				{
					Live.RemoveAt(num);
				}
			}
		}

		private static void Step(Drain d, float dt)
		{
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0acf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0472: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_0595: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_059f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_061e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0623: Unknown result type (might be due to invalid IL or missing references)
			//IL_0651: Unknown result type (might be due to invalid IL or missing references)
			//IL_0653: Unknown result type (might be due to invalid IL or missing references)
			//IL_0986: Unknown result type (might be due to invalid IL or missing references)
			//IL_098d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0994: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_09e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a77: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a89: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0726: Unknown result type (might be due to invalid IL or missing references)
			//IL_0733: Unknown result type (might be due to invalid IL or missing references)
			//IL_0738: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_084d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0900: Unknown result type (might be due to invalid IL or missing references)
			//IL_0905: Unknown result type (might be due to invalid IL or missing references)
			//IL_0935: Unknown result type (might be due to invalid IL or missing references)
			//IL_094d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0952: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)d.Holder) || !((Node)d.Holder).IsInsideTree() || (d.Card != null && GameRefs.CombatRoom == null))
			{
				Free(d);
				return;
			}
			if (d.Remote && (!MultiplayerFx.Active || !PlayerSettings.Enabled || (object)d.Room != GameRefs.CombatRoom))
			{
				Free(d);
				return;
			}
			if (d.Remote && d.Pending)
			{
				Creature target = d.Target;
				if (target == null || !target.IsAlive)
				{
					Free(d);
					return;
				}
				d.OwnedAt += dt;
				if (!d.GameDone && d.OwnedAt >= 12f)
				{
					Free(d);
				}
				else if (Prior(d) == null && d.GameDone)
				{
					Start(d, null);
				}
				return;
			}
			if (d.Remote && (Next(d) != null || Live.Exists((Drain o) => !o.Dead && !o.Remote && o.Playing && o.Target == d.Target)))
			{
				dt *= 1.5f;
			}
			d.Time += dt;
			float time = d.Time;
			Track(d);
			if (!d.Playing && !d.Dismissed && time - d.OwnedAt > 1f)
			{
				Close(d);
			}
			Spring(d.Slump, dt);
			if (d.Playing && !d.Closing)
			{
				Drink(d, time);
			}
			float num = (d.Closing ? (1f - Mathf.Clamp((time - d.CloseAt) / 0.32f, 0f, 1f)) : 1f);
			bool flag = d.HasAim && (d.Aimed || d.Playing) && !d.Dismissed && !d.Closing;
			int num2 = (d.Playing ? d.Active.Count : d.Orbs.Count);
			Zone(d, num2, out var centre, out var half);
			float num3 = GrowOf(d, num2);
			if (flag)
			{
				Wander(d, time, dt, half, num2);
			}
			else
			{
				foreach (Orb orb2 in d.Orbs)
				{
					orb2.Wandering = false;
				}
			}
			for (int num4 = 0; num4 < d.Orbs.Count; num4++)
			{
				Orb orb = d.Orbs[num4];
				orb.Last = orb.Pos;
				if (orb.Root == null || !FxKit.Alive((GodotObject)(object)orb.Root))
				{
					continue;
				}
				int i = Math.Min(orb.Index, Math.Max(0, num2 - 1));
				Vector2 val = EnergyBeads.Rest(d.Chest, d.Body, d.Side, i, Math.Max(1, num2), time);
				bool flag2 = flag && !orb.Surplus && orb.Wandering;
				Vector2 val2 = (flag2 ? Stroll(d, orb, centre, time) : val);
				Vector2 val3;
				if (d.Dismissed && d.Beads != null)
				{
					val3 = d.Beads.Move(orb, num4, val, orb.Placed, time, dt);
				}
				else if (orb.State == 3)
				{
					val3 = orb.Pos;
					orb.Show = 0f;
				}
				else if (orb.Surplus)
				{
					val3 = orb.Pos;
					orb.Show = FxKit.Damp(orb.Show, 0f, 10f, dt);
				}
				else if (d.Beads != null && orb.Out < 1f)
				{
					val3 = d.Beads.Move(orb, num4, val2, orb.Placed, time, dt);
					orb.Goal = val2;
				}
				else
				{
					val3 = Place(orb, val2, flag2, time, dt);
				}
				bool flag3 = flag && !orb.Surplus && orb.Out >= 1f && HopDone(orb, time) > 0.45f;
				orb.Turned = ((orb.Kind == BeadKind.Extra) ? 1f : FxKit.Damp(orb.Turned, flag3 ? 1f : 0f, flag3 ? 5f : 4f, dt));
				orb.Grow = FxKit.Damp(orb.Grow, flag3 ? num3 : 1f, flag3 ? 5f : 6f, dt);
				Vector2 val4 = val3 - orb.Pos;
				float num5 = ((dt > 0.0001f) ? (val4.Length() / dt) : 0f);
				float num6 = (orb.Placed ? Mathf.Clamp(num5 * 0.03f, 0f, 40f) : 0f);
				float num7 = ((num6 > 4f && val4.LengthSquared() > 0.01f) ? val4.Angle() : 0f);
				Color val5 = orb.Own.Lerp(Purple, orb.Turned);
				float num8 = orb.OwnInk * (1f - orb.Turned);
				float num9 = d.Bead * orb.Grow;
				Vector2 pos = val3;
				float num10 = 0f;
				if (orb.State == 1 || orb.State == 2)
				{
					float num11 = (orb.Fill = ((orb.State == 2) ? 1f : FillAt(d, orb, time)));
					float num12 = ((orb.FullAt >= 0f) ? Mathf.Exp(0f - Mathf.Pow((time - orb.FullAt - 0.04f) / 0.07f, 2f)) : 0f);
					float num13 = ((orb.State == 1) ? Gulp(d, orb, time) : 0f);
					float num14 = ((orb.State == 1 && !orb.Torn) ? Tension(d, orb, time) : 0f);
					val5 = val5.Lerp(Gorged, 0.7f * num11);
					num8 = Mathf.Max(num8, 0.42f * num11);
					num9 *= 1f + 0.2f * num11 + 0.18f * num12 + 0.08f * num13;
					float num15 = ((orb.State == 2) ? Mathf.Clamp((time - orb.GreyAt) / 0.24f, 0f, 1f) : 0f);
					orb.Spin += dt * ((orb.State == 1) ? Mathf.Lerp(4f, 15f, num11) : (15f * (1f - num15)));
					float num16 = Mathf.Max(Mathf.Clamp(Mathf.Abs(Heave(d, orb, time).X - Heave(d, orb, time - 0.016f).X) / 0.016f / (250f * d.View), 0f, 1f), 0.35f * num14);
					float num17;
					if (orb.Stream != null)
					{
						num17 = orb.Stream.Sign;
					}
					else
					{
						num17 = ((val3.X <= d.TargetChest.X) ? 1f : (-1f));
					}
					num6 = d.Bead * orb.Grow * (0.55f * Smooth((time - orb.DrinkAt) / 0.3f) + 0.3f * num16) * (1f - num15);
					num7 = (0f - num17) * orb.Spin;
					pos = val3 + new Vector2(Mathf.Cos(num7), Mathf.Sin(num7)) * (num6 * 0.5f * FogKit.ScaleOf((Node)(object)d.BeadLayer));
					num10 = 0.16f + 0.18f * num14 + 0.5f * num13 + 0.5f * num12;
					if (orb.State == 2)
					{
						val5 = val5.Lerp(Ash, Smooth((num15 - 0.2f) / 0.8f));
						num8 = Mathf.Lerp(num8, 0.15f, num15);
						num9 *= 1f - 0.06f * num15;
						num10 *= 1f - num15;
					}
				}
				orb.Tint = new Color(val5.R, val5.G, val5.B, 1f);
				orb.Placed = true;
				orb.Pos = pos;
				EnergyBeads.Draw(orb, num9, num, num6, num7, num8, orb.OwnInk * (1f - orb.Turned));
				orb.Pos = val3;
				if (orb.Flare != null && FxKit.Alive((GodotObject)(object)orb.Flare))
				{
					float num18 = num9 * 3f * (1f + 0.5f * num10);
					float num19 = ((orb.State == 3) ? 0f : (num10 * num * orb.Show));
					((Node2D)orb.Flare).GlobalPosition = val3;
					((Node2D)orb.Flare).Scale = new Vector2(num18, num18) / 64f;
					((CanvasItem)orb.Flare).Modulate = A(Purple.Lerp(Gorged, 0.4f * orb.Fill), Mathf.Clamp(num19, 0f, 1f));
				}
			}
			StepMists(d, time, dt, num, centre);
			StepStreams(d, time, dt, num);
			d.Beads?.Step(time, dt, num);
			if (d.Closing && num <= 0f)
			{
				Free(d);
			}
			else if (d.EndAt >= 0f && time >= d.EndAt)
			{
				Free(d);
			}
			else if (d.Playing && time - d.PlayAt > 12f)
			{
				Free(d);
			}
			else if (d.Dismissed && (d.Beads == null || d.Beads.Settled(time) || time - d.DismissAt > 3f))
			{
				Free(d);
			}
		}

		private static float Roll(MistGroup g)
		{
			return FxKit.Hash(g.Id * 977 + 3 + g.Rolls++ * 31);
		}

		private static void StepMists(Drain d, float time, float dt, float keep, Vector2 centre)
		{
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			float px = FogKit.ScaleOf((Node)(object)d.MistLayer);
			for (int num = d.Mists.Count - 1; num >= 0; num--)
			{
				MistGroup mistGroup = d.Mists[num];
				mistGroup.Show = (mistGroup.Leaving ? (mistGroup.Show - dt / 0.6f) : Mathf.Min(1f, mistGroup.Show + dt / 0.35f));
				if ((mistGroup.Leaving && mistGroup.Show <= 0f) || !FxKit.Alive((GodotObject)(object)mistGroup.Root))
				{
					if (FxKit.Alive((GodotObject)(object)mistGroup.Root))
					{
						((Node)mistGroup.Root).QueueFree();
					}
					d.Mists.RemoveAt(num);
					if (d.Mist == mistGroup)
					{
						d.Mist = null;
					}
				}
				else
				{
					if (!mistGroup.Leaving)
					{
						mistGroup.Chest = d.TargetChest;
						mistGroup.W = d.TargetW * d.View;
						mistGroup.H = d.TargetH * d.View;
						mistGroup.Side = d.Side;
					}
					Vector2 chest = mistGroup.Chest + Sunk(d, mistGroup);
					float show = Mathf.Clamp(mistGroup.Show, 0f, 1f) * keep;
					Glow(d, mistGroup, chest, time, dt, show, px);
					Smoke(d, mistGroup, chest, time, dt, show, centre);
				}
			}
		}

		private static void Glow(Drain d, MistGroup g, Vector2 chest, float time, float dt, float show, float px)
		{
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			int num = g.Total * (d.Heaves + 1);
			float target = ((num > 0) ? Mathf.Clamp((float)(g.Notches + g.Taken) / (float)num, 0f, 1f) : 0f);
			g.Ember = FxKit.Damp(g.Ember, target, 14f, dt);
			float num2 = 0.06f * Mathf.Sin(5.340708f * time) + 0.025f * Mathf.Sin(13.194689f * time + 1f);
			bool flag = false;
			float num3 = 0f;
			foreach (Stream stream in d.Streams)
			{
				if (stream.Group == g && !stream.Torn && !stream.Cut)
				{
					flag = true;
					num3 = Mathf.Max(num3, Pulse(d, stream.Orb, time));
				}
			}
			float num4 = Mathf.Lerp(1f, d.EmberLevel, g.Ember) * (1f + num2) * (flag ? 1.06f : 1f) * (1f - 0.22f * num3) + 0.1f * num3 * Mathf.Sin(time * 53f);
			float num5 = time - g.TornAt;
			if (num5 >= 0f && num5 < 0.6f)
			{
				num4 += 0.4f * Mathf.Exp((0f - num5) / 0.06f) + 0.1f * Mathf.Exp(0f - Mathf.Pow((num5 - 0.11f) / 0.035f, 2f));
			}
			float num6 = show * d.GlowK;
			float u = g.U;
			float num7 = Mathf.Lerp(1f, 0.72f, g.Ember);
			float num8 = 0.93f + 0.07f * Mathf.Sin(time * 1.3f + g.Seed);
			Lay(g.Wide, chest, 200f * u, px, GlowWide, 0.3f * num4 * num6);
			Lay(g.Mid, chest, 86f * u * num8 * num7, px, GlowMid, 0.55f * num4 * num6);
			Lay(g.Core, chest, 30f * u * num7, px, GlowCore, 0.85f * Mathf.Lerp(1f, 0.5f, g.Ember) * num4 * num6);
		}

		private static void Lay(Sprite2D s, Vector2 at, float size, float px, Color c, float alpha)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)s))
			{
				((Node2D)s).GlobalPosition = at;
				((Node2D)s).Scale = new Vector2(size, size) / (64f * Mathf.Max(0.01f, px));
				((CanvasItem)s).Modulate = A(c, Mathf.Clamp(alpha, 0f, 1f));
			}
		}

		private static Orb SmokeFor(Drain d, MistGroup g)
		{
			int count = d.Orbs.Count;
			for (int i = 0; i < count; i++)
			{
				int num = (g.NextBead + i) % count;
				Orb orb = d.Orbs[num];
				if (!orb.Surplus && orb.State == 0 && orb.Left && !orb.Arrived && !(orb.Show < 0.5f) && !(orb.Turned < 0.5f) && orb.Wandering)
				{
					g.NextBead = num + 1;
					return orb;
				}
			}
			return null;
		}

		private static void Smoke(Drain d, MistGroup g, Vector2 chest, float time, float dt, float show, Vector2 centre)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0455: Unknown result type (might be due to invalid IL or missing references)
			//IL_045d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_046b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0470: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0426: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			if (g.Sheet == null)
			{
				return;
			}
			float u = g.U;
			float num = Mathf.Clamp(chest.DistanceTo(centre) / (136f * u), 1f, 2.4f);
			if (!g.Leaving && d.SmokeAlpha > 0.001f && d.SmokeMax > 0 && time >= g.NextSmoke)
			{
				g.NextSmoke = time + d.SmokeEvery * Mathf.Lerp(0.7f, 1.3f, Roll(g));
				Orb orb = ((g.Smoke.Count < d.SmokeMax) ? SmokeFor(d, g) : null);
				if (orb != null)
				{
					Puff puff = new Puff
					{
						Orb = orb,
						Goal = orb.Pos,
						Born = time,
						Life = Mathf.Lerp(2f, 2.5f, Roll(g)),
						Seed = Roll(g),
						R0 = Mathf.Lerp(0.85f, 1.15f, Roll(g)),
						Thick = 0.8f
					};
					puff.Pos = chest + new Vector2(Roll(g) * 14f - 7f, Roll(g) * 14f - 7f) * u;
					Vector2 val = orb.Pos - chest;
					Vector2 val2 = (Vector2)((val.LengthSquared() > 1f) ? val.Normalized() : new Vector2(0f - g.Side, 0f));
					puff.Vel = val2 * (Mathf.Lerp(16f, 26f, Roll(g)) * u * num) + new Vector2(Roll(g) * 12f - 6f, (0f - Roll(g)) * 10f) * u;
					puff.Anchor0 = puff.Pos;
					puff.Anchor = puff.Pos;
					g.Smoke.Add(puff);
				}
			}
			g.Sheet.Begin();
			for (int num2 = g.Smoke.Count - 1; num2 >= 0; num2--)
			{
				Puff puff2 = g.Smoke[num2];
				Orb orb2 = puff2.Orb;
				bool flag = orb2 != null && !orb2.Surplus && orb2.State != 3 && !orb2.Arrived;
				if (!g.Leaving & flag)
				{
					puff2.Goal = orb2.Pos;
				}
				if (!flag)
				{
					puff2.Fade = Mathf.Max(0f, puff2.Fade - dt * 2.5f);
				}
				Vector2 val3 = puff2.Goal - puff2.Pos;
				float num3 = Mathf.Max(val3.Length(), 1f);
				Vector2 val4 = val3 / num3;
				Vector2 val5 = new Vector2(0f - val4.Y, val4.X);
				float num4 = (40f + 50f * Smooth((110f * u - num3) / (90f * u))) * u * num;
				Vector2 val6 = val4 * num4 + val5 * ((15f * Mathf.Sin(time * 1.9f + puff2.Seed * 6f) + 8f * Mathf.Sin(time * 3.4f + puff2.Seed * 11f)) * u) + new Vector2(0f, -7f * u);
				float num5 = ((flag && (orb2.State == 1 || orb2.State == 2)) ? Smooth((time - orb2.DrinkAt) / 0.45f) : 0f);
				if (num5 > 0f)
				{
					float num6 = ((puff2.Goal.X <= chest.X) ? 1f : (-1f));
					val6 += val4 * (240f * u * num * num5) + val5 * (num6 * 90f * u * num5 * Smooth((90f * u - num3) / (60f * u)));
				}
				puff2.Vel = puff2.Vel.Lerp(val6, 1f - Mathf.Exp(-2.6f * dt));
				puff2.Pos += puff2.Vel * dt;
				float num7 = (time - puff2.Born) / puff2.Life;
				puff2.R = Mathf.Lerp(8f, 22f, Mathf.Pow(Mathf.Clamp(num7, 0f, 1f), 0.7f)) * puff2.R0 * u;
				puff2.Seen = d.SmokeAlpha * puff2.Fade * Smooth(num7 / 0.18f) * (1f - Smooth((num7 - 0.62f) / 0.38f)) * Smooth((num3 - 8f * u) / (30f * u));
				puff2.Age = 0.35f + 0.5f * Mathf.Clamp(num7, 0f, 1f);
				puff2.Psi = (time - puff2.Born) * 0.2f;
				puff2.Anchor = (puff2.Pos + puff2.Anchor0) * 0.5f;
				if (num7 >= 1f || puff2.Fade <= 0f)
				{
					g.Smoke.RemoveAt(num2);
				}
				else
				{
					g.Sheet.Add(puff2.Pos, puff2.R, puff2.Thick, puff2.Age, puff2.Seen, puff2.Vel * (0.006f / u), puff2.Anchor, puff2.Psi);
				}
			}
			g.Sheet.End(time * 1.4f + 20f, show, 0.2f, 1.15f * u);
		}

		private static float Roll(Stream st)
		{
			return FxKit.Hash(st.Orb.Id * 389 + 11 + st.Rolls++ * 23);
		}

		private static Stream MakeStream(Drain d, Orb o, MistGroup g, float time)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Expected Obj, but got Unknown
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			if (g == null || !FxKit.Alive((GodotObject)(object)d.MistLayer))
			{
				return null;
			}
			Vector2 val = g.Chest + Sunk(d, g);
			Stream stream = new Stream
			{
				Orb = o,
				Group = g,
				Start = time,
				U = g.U,
				Sign = ((o.Pos.X <= val.X) ? 1f : (-1f)),
				Seed = FxKit.Hash(o.Id * 41 + 9) * 10f,
				From = val
			};
			stream.Lift = (Roll(stream) - 0.5f) * 12f;
			stream.Flow = Roll(stream);
			int puffN = d.PuffN;
			stream.Glows = new Sprite2D[puffN];
			stream.Bodies = new Sprite2D[(puffN + 1) / 2];
			stream.Lat = new float[puffN];
			stream.Size = new float[puffN];
			stream.Jit = new float[puffN];
			for (int i = 0; i < puffN; i++)
			{
				stream.Lat[i] = Roll(stream) - 0.5f;
				stream.Size[i] = Mathf.Lerp(0.8f, 1.2f, Roll(stream));
				stream.Jit[i] = Roll(stream) - 0.5f;
			}
			for (int j = 0; j < stream.Bodies.Length; j++)
			{
				Sprite2D val2 = new Sprite2D
				{
					Texture = FxKit.SoftDot,
					Modulate = new Color(1f, 1f, 1f, 0f)
				};
				((Node)d.MistLayer).AddChild((Node)(object)val2, false, (InternalMode)0);
				stream.Bodies[j] = val2;
			}
			for (int k = 0; k < puffN; k++)
			{
				Sprite2D val3 = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
				((Node)d.MistLayer).AddChild((Node)(object)val3, false, (InternalMode)0);
				stream.Glows[k] = val3;
			}
			for (int l = 0; l < 5; l++)
			{
				Ember ember = new Ember
				{
					S = Roll(stream),
					Lat = Roll(stream) - 0.5f,
					Seed = Roll(stream),
					Size = Mathf.Lerp(0.7f, 1.25f, Roll(stream))
				};
				ember.Dot = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
				((Node)d.MistLayer).AddChild((Node)(object)ember.Dot, false, (InternalMode)0);
				stream.Embers.Add(ember);
			}
			stream.Knot = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
			((Node)d.MistLayer).AddChild((Node)(object)stream.Knot, false, (InternalMode)0);
			return stream;
		}

		private static void TearStream(Stream st, float time)
		{
			st.Torn = true;
			st.TornAt = time;
			foreach (Ember ember in st.Embers)
			{
				ember.STear = ember.S;
			}
		}

		private static Vector2 OnWay(in Way w, float p, out float q)
		{
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			if (p < w.Split)
			{
				q = 0f;
				float num = p / Mathf.Max(0.0001f, w.Split);
				float num2 = 1f - num;
				Vector2 val = w.C * (num2 * num2 * num2) + w.P1 * (3f * num2 * num2 * num) + w.P2 * (3f * num2 * num * num) + w.E * (num * num * num);
				float num3 = Mathf.Sin((float)Math.PI * num);
				return val + w.Normal * (w.Sway * num3 * num3 * Mathf.Sin(w.Time * 1.7f + w.Seed + num * 5.5f));
			}
			float num4 = Mathf.Clamp((p - w.Split) / Mathf.Max(0.0001f, 1f - w.Split), 0f, 1f);
			q = 1f - Mathf.Sqrt(1f - num4);
			float num5 = w.R * (1f - q);
			float num6 = w.From - w.Sign * w.Turns * q;
			return w.B + new Vector2(Mathf.Cos(num6), 0.62f * Mathf.Sin(num6)) * num5;
		}

		private static void PoseSoft(Sprite2D s, Vector2 at, float angle, float along, float across, float px, Color c, float alpha)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)s))
			{
				((Node2D)s).GlobalPosition = at;
				((Node2D)s).GlobalRotation = angle;
				((Node2D)s).Scale = new Vector2(along, across) / (64f * Mathf.Max(0.01f, px));
				((CanvasItem)s).Modulate = A(c, Mathf.Clamp(alpha, 0f, 1f));
			}
		}

		private static void StepStreams(Drain d, float time, float dt, float keep)
		{
			float px = FogKit.ScaleOf((Node)(object)d.MistLayer);
			for (int num = d.Streams.Count - 1; num >= 0; num--)
			{
				Stream stream = d.Streams[num];
				if (!stream.Done)
				{
					if (stream.Cut)
					{
						stream.Fade = Mathf.Max(0f, stream.Fade - dt / 0.3f);
					}
					Pour(d, stream, time, dt, keep * stream.Fade, px);
					if ((stream.Torn && time - stream.TornAt > d.Whip + 0.12f) || (stream.Cut && stream.Fade <= 0f))
					{
						stream.Done = true;
					}
				}
				if (stream.Done)
				{
					if (stream.Glows != null)
					{
						Sprite2D[] glows = stream.Glows;
						foreach (Sprite2D val in glows)
						{
							if (FxKit.Alive((GodotObject)(object)val))
							{
								((Node)val).QueueFree();
							}
						}
					}
					if (stream.Bodies != null)
					{
						Sprite2D[] glows = stream.Bodies;
						foreach (Sprite2D val2 in glows)
						{
							if (FxKit.Alive((GodotObject)(object)val2))
							{
								((Node)val2).QueueFree();
							}
						}
					}
					if (FxKit.Alive((GodotObject)(object)stream.Knot))
					{
						((Node)stream.Knot).QueueFree();
					}
					foreach (Ember ember in stream.Embers)
					{
						if (FxKit.Alive((GodotObject)(object)ember.Dot))
						{
							((Node)ember.Dot).QueueFree();
						}
					}
					stream.Embers.Clear();
					d.Streams.RemoveAt(num);
				}
			}
		}

		private static void Pour(Drain d, Stream st, float time, float dt, float intensity, float px)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0584: Unknown result type (might be due to invalid IL or missing references)
			//IL_0589: Unknown result type (might be due to invalid IL or missing references)
			//IL_058e: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0610: Unknown result type (might be due to invalid IL or missing references)
			//IL_0633: Unknown result type (might be due to invalid IL or missing references)
			//IL_0649: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0886: Unknown result type (might be due to invalid IL or missing references)
			//IL_0899: Unknown result type (might be due to invalid IL or missing references)
			Orb orb = st.Orb;
			MistGroup mistGroup = st.Group;
			float u = st.U;
			Vector2 val = mistGroup.Chest + Sunk(d, mistGroup);
			Vector2 pos = orb.Pos;
			Vector2 val2 = pos - val;
			float num = Mathf.Clamp(val2.Y * 0.3f, -32f * u, 32f * u) + st.Lift * u;
			Vector2 val3 = (st.From = val + new Vector2(((val2.X < 0f) ? (-9f) : 9f) * u, num));
			float num2 = (st.Torn ? 0f : Tension(d, orb, time));
			float num3 = (st.Torn ? 0f : Surge(d, orb, time));
			float num4 = (st.Torn ? Mathf.Clamp((time - st.TornAt) / Mathf.Max(0.05f, d.Whip), 0f, 1f) : 0f);
			float num5 = Mathf.Clamp((time - st.Start) / Mathf.Max(0.02f, d.Latch), 0f, 1f);
			float num6 = 1f - (1f - num5) * (1f - num5);
			float num7 = num4 * num4;
			Vector2 val4 = pos - val3;
			float num8 = Mathf.Max(val4.Length(), 1f);
			Vector2 val5 = val4 / num8;
			float num9 = Mathf.Min(d.Coil * d.Bead * orb.Grow * d.View * (1f - 0.22f * num3), 0.45f * num8);
			float num10 = d.CoilTurns * (1f + 0.12f * num3);
			float num11 = Mathf.Atan2(st.Sign * val5.X, (0f - st.Sign) * val5.Y / 0.62f);
			Vector2 val6 = new Vector2(Mathf.Cos(num11), 0.62f * Mathf.Sin(num11));
			Vector2 val7 = pos + val6 * num9;
			Vector2 val8 = new Vector2(Mathf.Sin(num11), -0.62f * Mathf.Cos(num11)) * (st.Sign * num10) - val6;
			Vector2 val9 = val8.Normalized();
			float num12 = Mathf.Max(val3.DistanceTo(val7), 1f);
			Vector2 val10 = (val7 - val3) / num12;
			Way w = new Way
			{
				C = val3,
				P1 = val3 + (val7 - val3) * 0.3f + new Vector2(0f, 0f - Mathf.Min(40f * u, 0.12f * num12)),
				P2 = val7 - val9 * (0.35f * num12),
				E = val7,
				B = pos,
				Normal = new Vector2(0f - val10.Y, val10.X),
				R = num9,
				Turns = num10,
				From = num11,
				Sign = st.Sign,
				Sway = (6f + 4f * (1f - num2)) * u,
				Time = time,
				Seed = st.Seed
			};
			float num13 = 0.5f * (num12 + val3.DistanceTo(w.P1) + w.P1.DistanceTo(w.P2) + w.P2.DistanceTo(val7));
			float num14 = num9 * 0.81f * Mathf.Sqrt(0.25f * num10 * num10 + 1f);
			w.Split = num13 / Mathf.Max(1f, num13 + 2.2f * num14);
			float num15 = (st.Torn ? 3.08f : (1.4f * (0.6f + 0.4f * num2 + 1.3f * num3)));
			st.Flow += dt * num15;
			st.Flow -= Mathf.Floor(st.Flow);
			float num16 = 46f * u * d.PuffSize * (1f + 0.35f * num3);
			float num17 = intensity * d.StreamAlpha * (0.85f + 0.3f * num3);
			int num18 = st.Glows.Length;
			for (int i = 0; i < num18; i++)
			{
				float num19 = st.Flow + ((float)i + 0.35f * st.Jit[i]) / (float)num18;
				num19 -= Mathf.Floor(num19);
				float num20 = Smooth(num19 / 0.05f) * Smooth((num6 - num19) / 0.05f);
				if (st.Torn)
				{
					num20 *= Smooth((num19 - num7) / 0.08f);
				}
				Vector2 val11 = OnWay(in w, num19, out var q);
				Vector2 val12 = OnWay(in w, Mathf.Min(1f, num19 + 0.01f), out var _) - val11;
				float num21 = ((val12.LengthSquared() > 0.0001f) ? val12.Angle() : val5.Angle());
				val11 += new Vector2(0f - Mathf.Sin(num21), Mathf.Cos(num21)) * (st.Lat[i] * num16 * 0.5f * (1f - q));
				float num22 = num16 * st.Size[i] * Mathf.Lerp(0.65f, 1f, Smooth(num19 / 0.12f)) * Mathf.Lerp(1f, 0.4f, q);
				float num23 = num17 * num20 * (1f - Smooth((q - 0.78f) / 0.22f));
				PoseSoft(st.Glows[i], val11, num21, num22 * (1.9f + 0.5f * num3), num22, px, Blood, 0.5f * num23);
				if ((i & 1) == 0)
				{
					PoseSoft(st.Bodies[i >> 1], val11, num21, num22 * 2.2f, num22 * 1.4f, px, Fog, 0.42f * num23);
				}
			}
			float num24 = (st.Torn ? 0f : Pulse(d, orb, time));
			float num25 = (st.Torn ? Mathf.Max(0f, 1f - (time - st.TornAt) / 0.12f) : Smooth(num5 / 0.4f));
			Lay(st.Knot, val3, 24f * u * (1f + 0.6f * num24 + 0.2f * num2), px, GlowCore, (0.4f + 0.45f * num24 + 0.2f * num2) * num25 * intensity);
			foreach (Ember ember in st.Embers)
			{
				if (!st.Torn)
				{
					ember.S += dt * num15 * 1.25f;
					if (ember.S >= 1f)
					{
						ember.S--;
					}
				}
				else
				{
					ember.S = ember.STear + (1f - ember.STear) * num7;
				}
				float num26 = Mathf.Clamp(ember.S, 0f, 1f);
				Vector2 at = OnWay(in w, num26, out var q3) + w.Normal * (ember.Lat * num16 * 0.4f * (1f - q3));
				float num27 = 0.6f + 0.4f * Mathf.Sin(time * 23f + ember.Seed * 40f);
				float num28 = 0.75f * num27 * Smooth(num26 / 0.06f) * (1f - Smooth((q3 - 0.5f) / 0.4f)) * Smooth((num6 - num26) / 0.08f) * intensity;
				if (st.Torn)
				{
					num28 *= ((ember.S >= 0.999f || num4 >= 1f) ? 0f : Smooth((num26 - num7) / 0.06f));
				}
				Lay(ember.Dot, at, 7f * u * ember.Size, px, EmberTint, num28);
			}
		}

		private static Rect2 OnScreen(CanvasItem node)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Viewport viewport = ((Node)node).GetViewport();
				if (viewport != null)
				{
					Transform2D canvasTransform = node.GetCanvasTransform();
					Rect2 result = canvasTransform.AffineInverse() * viewport.GetVisibleRect();
					if (result.Size.X > 100f && result.Size.Y > 100f)
					{
						return result;
					}
				}
			}
			catch
			{
			}
			return new Rect2(Vector2.Zero, FxKit.ViewSize());
		}

		static MalaiseFx()
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
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		}
	}
