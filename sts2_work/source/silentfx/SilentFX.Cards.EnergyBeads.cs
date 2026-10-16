// namespace: SilentFX.Cards
// type: EnergyBeads (class)
// source lines: 51417-52667
	public sealed class EnergyBeads
	{
		public class Bead
		{
			public BeadKind Kind;

			public Color Tint;

			public Vector2 Pos;

			public float Show;

			public Node2D Root;

			public Vector2 Start;

			public float LeaveAt;

			public bool Left;

			public float Out;

			public float HomeAt;

			public float Home;

			public Vector2 HomeFrom;

			public bool Arrived;

			internal int From = -1;

			internal BeadLook Look;
		}

		internal sealed class BeadLook
		{
			public Sprite2D Halo;

			public Sprite2D Body;

			public Sprite2D Ring;

			public Sprite2D Glow;

			public Sprite2D Core;

			public Sprite2D Blot;
		}

		private sealed class Source
		{
			public bool Relic;

			public Control Icon;

			public Node2D Layer;

			public Vector2 Point;

			public Vector2 Spot;

			public float Size = 128f;

			public Color CapTint;

			public Sprite2D Cap;

			public Node2D Ring;

			public ShaderMaterial RingMat;

			public float CapShow;

			public int Made;

			public int Away;
		}

		private static readonly StringName PAlpha = StringName.op_Implicit("alpha");

		private static readonly StringName PSpin = StringName.op_Implicit("spin");

		public const float OutTime = 0.34f;

		public const float HomeTime = 0.28f;

		public static readonly Color Green = new Color(0.3f, 1f, 0.46f, 1f);

		public static readonly Color GreenPale = new Color(0.86f, 1f, 0.9f, 1f);

		public static readonly Color Ink = new Color(0.025f, 0.025f, 0.032f, 1f);

		public static readonly Color Rim = new Color(0.84f, 0.88f, 0.95f, 1f);

		private static readonly Color Dark = new Color(0.02f, 0.05f, 0.03f, 1f);

		private readonly List<Bead> _beads = new List<Bead>();

		private readonly List<Source> _sources = new List<Source>();

		private Node2D _top;

		private bool _warned;

		public float CapSize = 1f;

		public float CapDark = 0.86f;

		private static Texture2D _capTexture;

		private static Texture2D _inkDisc;

		private static Shader _inkSigil;

		public Node2D Holder { get; private set; }

		public IReadOnlyList<Bead> All => _beads;

		public float NextLeave { get; private set; }

		public int Count => _beads.Count;

		public int EnergyCount { get; private set; }

		public int ChemCount { get; private set; }

		public bool Homeward { get; private set; }

		public bool Released { get; private set; }

		public float LastHome { get; private set; }

		public Vector2 Counter
		{
			get
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				if (_sources.Count <= 0)
				{
					return GameRefs.EnergyPoint();
				}
				return _sources[0].Point;
			}
		}

		public static bool ChemOn => Tuning.F("beads.chem", 1f) >= 0.5f;

		public static float RimLight => Mathf.Clamp(Tuning.F("beads.chem_rim", 1f), 0f, 3f);

		public static float HaloLight => 0.8f * Mathf.Clamp(Tuning.F("beads.chem_halo", 1f), 0f, 3f);

		private static float InkHalo => 0.6f * Mathf.Clamp(Tuning.F("beads.chem_halo", 1f), 0f, 3f);

		internal static Texture2D CapTexture => _capTexture ?? (_capTexture = Disc(128));

		public static Texture2D InkDisc => _inkDisc ?? (_inkDisc = Disc(64));

		private static Color A(Color c, float alpha)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return new Color(c.R, c.G, c.B, alpha);
		}

		internal static void Warm()
		{
			_ = CapTexture;
			_ = InkDisc;
			EnsureInkShader();
			ShaderWarmup.Add(_inkSigil);
		}

		private static void EnsureInkShader()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected Obj, but got Unknown
			if (!FxKit.Alive((GodotObject)(object)_inkSigil))
			{
				Shader val = ShaderLib.Get("sigil");
				if (val != null)
				{
					_inkSigil = new Shader
					{
						Code = val.Code.Replace("blend_add", "blend_mix")
					};
				}
			}
		}

		private static Texture2D Disc(int size)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected Obj, but got Unknown
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Expected Obj, but got Unknown
			Gradient val = new Gradient();
			val.Offsets = new float[3] { 0f, 0.6f, 1f };
			val.Colors = new Color[3]
			{
				new Color(1f, 1f, 1f, 1f),
				new Color(1f, 1f, 1f, 0.96f),
				new Color(1f, 1f, 1f, 0f)
			};
			return (Texture2D)new GradientTexture2D
			{
				Gradient = val,
				Width = size,
				Height = size,
				Fill = (FillEnum)1,
				FillFrom = new Vector2(0.5f, 0.5f),
				FillTo = new Vector2(1f, 0.5f)
			};
		}

		public static void MakeInk(Sprite2D sprite)
		{
			if (FxKit.Alive((GodotObject)(object)sprite))
			{
				((CanvasItem)sprite).Material = null;
				sprite.Texture = InkDisc;
			}
		}

		public static Node2D InkSigil(float size, out ShaderMaterial mat)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected Obj, but got Unknown
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Expected Obj, but got Unknown
			mat = null;
			try
			{
				EnsureInkShader();
				if (_inkSigil == null)
				{
					return null;
				}
				mat = new ShaderMaterial
				{
					Shader = _inkSigil
				};
				mat.SetShaderParameter(StringName.op_Implicit("tint"), Variant.op_Implicit(Ink));
				Node2D val = new Node2D();
				ColorRect val2 = FxKit.Quad(new Vector2(size, size), (Material)(object)mat);
				((Control)val2).Position = new Vector2((0f - size) * 0.5f, (0f - size) * 0.5f);
				((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
				return val;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Beads: no ink ring: " + ex.Message);
				mat = null;
				return null;
			}
		}

		public static int ChemOf(CardModel card, int energy)
		{
			if (card == null || !ChemOn)
			{
				return 0;
			}
			try
			{
				if (!card.EnergyCost.CostsX)
				{
					return 0;
				}
				int num = Math.Max(0, energy);
				int num2 = Hook.ModifyXValue(card.CombatState, card, num);
				return Math.Max(0, num2 - num);
			}
			catch (Exception ex)
			{
				DevLog.Warn("Beads: could not tell what modifies X: " + ex.Message);
				return 0;
			}
		}

		public static int EnergyPartOf(CardModel card, int x)
		{
			if (card == null || !ChemOn)
			{
				return x;
			}
			try
			{
				return Mathf.Clamp(card.EnergyCost.CapturedXValue, 0, Math.Max(0, x));
			}
			catch (Exception ex)
			{
				DevLog.Warn("Beads: could not tell what X was paid with: " + ex.Message);
				return x;
			}
		}

		public static EnergyBeads Open<T>(Node2D holder, int energy, int chem, int max, float gap, float capSize, float capDark) where T : Bead, new()
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Expected Obj, but got Unknown
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)holder))
			{
				return null;
			}
			EnergyBeads energyBeads = new EnergyBeads
			{
				Holder = holder,
				CapSize = capSize,
				CapDark = capDark
			};
			int num = Math.Max(0, max);
			int num2 = Mathf.Clamp(chem, 0, num);
			int num3 = Mathf.Clamp(energy, 0, num - num2);
			int num4 = num3 + num2;
			Source source = new Source
			{
				Layer = holder,
				CapTint = Dark
			};
			energyBeads._sources.Add(source);
			energyBeads.FindCounter(source);
			source.Cap = new Sprite2D
			{
				Texture = CapTexture,
				Modulate = A(Dark, 0f)
			};
			((Node)holder).AddChild((Node)(object)source.Cap, false, (InternalMode)0);
			source.Ring = FxKit.Sigil(100f, Green, out source.RingMat);
			((Node)holder).AddChild((Node)(object)source.Ring, false, (InternalMode)0);
			if (source.RingMat != null)
			{
				source.RingMat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.76f));
				source.RingMat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.03f));
				source.RingMat.SetShaderParameter(StringName.op_Implicit("dashes"), Variant.op_Implicit(16f));
				source.RingMat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.2f));
				source.RingMat.SetShaderParameter(PAlpha, Variant.op_Implicit(0f));
			}
			int num5 = 0;
			if (num2 > 0)
			{
				int num6 = energyBeads.FindRelic();
				if (num6 >= 0)
				{
					num5 = num6;
				}
			}
			bool flag = num5 > 0;
			int num7 = (flag ? Math.Max(num3, num2) : num4);
			float num8 = ((num7 > 0) ? Mathf.Min(Mathf.Max(0.02f, gap), 0.5f / (float)num7) : 0f);
			for (int i = 0; i < num4; i++)
			{
				bool flag2 = i >= num3;
				int num9 = ((flag2 & flag) ? (i - num3) : i);
				int num10 = (flag2 ? num5 : 0);
				Source source2 = energyBeads._sources[num10];
				T val = new T
				{
					Kind = (flag2 ? BeadKind.Chem : BeadKind.Energy),
					Tint = (flag2 ? Rim : Green),
					Pos = source2.Point,
					Start = source2.Point,
					LeaveAt = 0.04f + (float)num9 * num8
				};
				val.From = num10;
				source2.Made++;
				energyBeads._beads.Add(val);
			}
			energyBeads.EnergyCount = num3;
			energyBeads.ChemCount = num2;
			energyBeads.NextLeave = 0.04f + (float)num7 * num8;
			if (num3 == 0)
			{
				FxKit.RingPulse((Node)(object)holder, source.Point, source.Size * 1.1f, A(Green, 0.5f), 0f, 0.35f, 0.72f, 1f, 1f, 0f, 0.03f, 0.26f);
			}
			return energyBeads;
		}

		public T AddExtra<T>(Vector2 from, Color tint, float leaveAt) where T : Bead, new()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			T val = new T
			{
				Kind = BeadKind.Extra,
				Tint = tint,
				Pos = from,
				Start = from,
				LeaveAt = leaveAt
			};
			_beads.Add(val);
			return val;
		}

		public void Dress(Bead b, Node parent = null)
		{
			DressOn(b, (Node)(((object)parent) ?? ((object)Holder)));
		}

		public static void DressOn(Bead b, Node parent)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected Obj, but got Unknown
			if (b == null || b.Look != null)
			{
				return;
			}
			if (FxKit.Alive((GodotObject)(object)parent))
			{
				b.Root = new Node2D();
				parent.AddChild((Node)(object)b.Root, false, (InternalMode)0);
				b.Look = new BeadLook
				{
					Halo = Dot((Node)(object)b.Root),
					Body = Dot((Node)(object)b.Root),
					Ring = Dot((Node)(object)b.Root),
					Glow = Dot((Node)(object)b.Root),
					Core = Dot((Node)(object)b.Root),
					Blot = Dot((Node)(object)b.Root)
				};
				if (FxKit.Alive((GodotObject)(object)b.Look.Ring))
				{
					b.Look.Ring.Texture = FxKit.Bubble;
				}
				MakeInk(b.Look.Blot);
			}
		}

		public static void Draw(Bead b, float size, float alpha, float stretch = 0f, float rot = 0f, float ink = -1f, float chem = -1f)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			BeadLook beadLook = b?.Look;
			if (beadLook != null && FxKit.Alive((GodotObject)(object)b.Root))
			{
				b.Root.GlobalPosition = b.Pos;
				b.Root.GlobalRotation = rot;
				float num = Mathf.Clamp(alpha * b.Show, 0f, 1f);
				float num2;
				if (ink < 0f)
				{
					num2 = ((b.Kind == BeadKind.Chem) ? 1f : 0f);
				}
				else
				{
					num2 = ink;
				}
				float num3 = Mathf.Clamp(num2, 0f, 1f);
				float num4;
				if (chem < 0f)
				{
					num4 = ((b.Kind == BeadKind.Chem) ? num3 : 0f);
				}
				else
				{
					num4 = chem;
				}
				float num5 = Mathf.Clamp(num4, 0f, num3);
				Color tint = b.Tint;
				Vector2 val = new Vector2((0f - stretch) * 0.5f, 0f);
				Lay(beadLook.Halo, val, new Vector2(size * 2.6f + stretch, size * 2.6f), A(tint, 0.28f * Mathf.Lerp(1f, InkHalo, num3 - num5) * Mathf.Lerp(1f, HaloLight, num5) * num));
				Lay(beadLook.Body, val, new Vector2(size * 1.2f + stretch, size * 1.2f), A(tint, 0.92f * (1f - num3) * num));
				Lay(beadLook.Ring, val, new Vector2(size * 1.2f + stretch, size * 1.2f), A(tint, Mathf.Min(1f, 0.9f * RimLight) * (num3 - num5) * num));
				Lay(beadLook.Glow, val, new Vector2(size * 1.6f + stretch, size * 1.6f), A(tint, Mathf.Min(1f, 0.95f * RimLight) * num5 * num));
				Lay(beadLook.Core, val * 0.6f, new Vector2(size * 0.6f + stretch * 0.6f, size * 0.6f), A(Whiter(tint), 0.9f * (1f - num3) * num));
				Lay(beadLook.Blot, val * 0.6f, new Vector2(size * 0.84f + stretch * 0.8f, size * 0.84f), A(Ink, 0.96f * num3 * num));
			}
		}

		private static void Lay(Sprite2D sprite, Vector2 at, Vector2 size, Color tint)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)sprite))
			{
				((Node2D)sprite).Position = at;
				((Node2D)sprite).Scale = size / 64f;
				((CanvasItem)sprite).Modulate = tint;
			}
		}

		private static Color Whiter(Color c)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (c.Equals(Green))
			{
				return GreenPale;
			}
			return new Color(Mathf.Lerp(c.R, 1f, 0.8f), Mathf.Lerp(c.G, 1f, 0.8f), Mathf.Lerp(c.B, 1f, 0.8f), 1f);
		}

		private static Sprite2D Dot(Node parent)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			Sprite2D val = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			return val;
		}

		public static Vector2 Rest(Vector2 chest, Vector2 body, float side, int i, int n, float time, float gap = 34f, float step = 38f)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Max(body.Y * 1.05f, 180f);
			float num2 = ((n > 1) ? Mathf.Min(step, num / (float)(n - 1)) : 0f);
			float num3 = body.X * 0.5f + gap;
			return chest + new Vector2((0f - side) * num3, (0f - body.Y) * 0.06f + ((float)i - (float)(n - 1) * 0.5f) * num2) + new Vector2(Mathf.Sin(time * 1.7f + (float)i * 2.4f) * 14f, Mathf.Cos(time * 2.3f + (float)i * 1.3f) * 6f);
		}

		public void Track()
		{
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < _sources.Count; i++)
			{
				Source source = _sources[i];
				if (source.Relic)
				{
					Measure(source);
				}
				else
				{
					FindCounter(source);
				}
			}
			for (int j = 0; j < _beads.Count; j++)
			{
				Bead bead = _beads[j];
				if (bead.From >= 0 && bead.From < _sources.Count)
				{
					bead.Start = _sources[bead.From].Point;
				}
			}
		}

		public Vector2 Move(Bead b, int i, Vector2 rest, bool placed, float time, float dt)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if (b == null)
			{
				return rest;
			}
			Source src = SourceOf(b);
			Vector2 start = b.Start;
			Vector2 result = b.Pos;
			if (Homeward)
			{
				if (b.Arrived)
				{
					b.Show = 0f;
				}
				else if (time >= b.HomeAt)
				{
					if (b.Home <= 0f)
					{
						b.HomeFrom = b.Pos;
					}
					b.Home = Mathf.Min(1f, b.Home + dt / 0.28f);
					Vector2 b2 = b.HomeFrom.Lerp(start, 0.5f) + new Vector2(0f, -110f);
					result = FxKit.Bezier(b.HomeFrom, b2, start, b.Home * b.Home);
					if (b.Home >= 1f)
					{
						b.Arrived = true;
						b.Show = 0f;
						LastHome = time;
						Glint(b, src, 0.45f, 0.45f, 1.4f, 0.14);
					}
				}
				else
				{
					result = b.Pos.Lerp(rest, 1f - Mathf.Exp(-12f * dt));
				}
			}
			else if (b.Out < 1f)
			{
				if (time >= b.LeaveAt)
				{
					if (!b.Left)
					{
						b.Left = true;
						Glint(b, src, 0.6f, 0.55f, 1.5f, 0.16);
					}
					b.Out = Mathf.Min(1f, b.Out + dt / 0.34f);
					float t = 1f - Mathf.Pow(1f - b.Out, 3f);
					Vector2 b3 = start.Lerp(rest, 0.45f) + new Vector2((FxKit.Hash(i * 3 + 1) - 0.5f) * 120f, -150f - 22f * (float)(i % 3));
					result = FxKit.Bezier(start, b3, rest, t);
					b.Show = Mathf.Min(1f, b.Out * 8f);
				}
				else
				{
					result = start;
					b.Show = 0f;
				}
			}
			else
			{
				result = Settle(b, rest, placed, dt);
			}
			return result;
		}

		public static Vector2 Settle(Bead b, Vector2 rest, bool placed, float dt)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			Vector2 result = (placed ? b.Pos.Lerp(rest, 1f - Mathf.Exp(-12f * dt)) : rest);
			b.Show = FxKit.Damp(b.Show, 1f, 16f, dt);
			return result;
		}

		private void Glint(Bead b, Source src, float alpha, float size, float grow, double time)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			float alpha2 = ((b.Kind == BeadKind.Chem) ? (alpha * 0.55f) : alpha);
			if (src != null)
			{
				FxKit.Flash((Node)(object)src.Layer, src.Spot, A(b.Tint, alpha2), src.Size * size, grow, time);
			}
			else
			{
				FxKit.Flash((Node)(object)Holder, b.Start, A(b.Tint, alpha2), 64f * size, grow, time);
			}
		}

		private Source SourceOf(Bead b)
		{
			if (b == null || b.From < 0 || b.From >= _sources.Count)
			{
				return null;
			}
			return _sources[b.From];
		}

		private static bool Counts(Bead b)
		{
			if (b.Root != null)
			{
				return FxKit.Alive((GodotObject)(object)b.Root);
			}
			return true;
		}

		public void SendHome(float time)
		{
			if (Homeward || Released)
			{
				return;
			}
			Homeward = true;
			LastHome = time;
			int count = _beads.Count;
			for (int i = 0; i < count; i++)
			{
				Bead bead = _beads[i];
				if (!bead.Left)
				{
					bead.Arrived = true;
					continue;
				}
				bead.Out = 1f;
				bead.Home = 0f;
				bead.HomeAt = time + (float)(count - 1 - i) * 0.04f;
			}
		}

		public void Release()
		{
			Released = true;
			for (int i = 0; i < _beads.Count; i++)
			{
				_beads[i].Out = 1f;
				_beads[i].Left = true;
			}
		}

		public void Step(float time, float dt, float keep)
		{
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < _sources.Count; i++)
			{
				_sources[i].Away = 0;
			}
			for (int j = 0; j < _beads.Count; j++)
			{
				Bead bead = _beads[j];
				if (bead.Left && !bead.Arrived && Counts(bead))
				{
					Source source = SourceOf(bead);
					if (source != null)
					{
						source.Away++;
					}
				}
			}
			for (int k = 0; k < _sources.Count; k++)
			{
				Source source2 = _sources[k];
				if (!FxKit.Alive((GodotObject)(object)source2.Cap))
				{
					continue;
				}
				float target = ((Released || source2.Made <= 0 || source2.Away <= 0) ? 0f : Mathf.Lerp(0.72f, 1f, Mathf.Clamp((float)source2.Away / (float)source2.Made, 0f, 1f)));
				source2.CapShow = FxKit.Damp(source2.CapShow, target, 10f, dt);
				float num = source2.Size * CapSize;
				float num2 = num * (0.55f + 0.45f * Mathf.Min(1f, source2.CapShow / 0.72f));
				((Node2D)source2.Cap).GlobalPosition = source2.Spot;
				((Node2D)source2.Cap).Scale = new Vector2(num2, num2) / 128f;
				((CanvasItem)source2.Cap).Modulate = A(source2.CapTint, CapDark * source2.CapShow * keep);
				if (FxKit.Alive((GodotObject)(object)source2.Ring))
				{
					float num3 = num * 1.04f / 100f;
					source2.Ring.GlobalPosition = source2.Spot;
					source2.Ring.Scale = new Vector2(num3, num3);
					if (FxKit.Alive((GodotObject)(object)source2.RingMat))
					{
						source2.RingMat.SetShaderParameter(PAlpha, Variant.op_Implicit(0.5f * source2.CapShow * keep * (0.8f + 0.2f * Mathf.Sin(time * 3f))));
						source2.RingMat.SetShaderParameter(PSpin, Variant.op_Implicit(time * 0.6f));
					}
				}
			}
		}

		public bool Settled(float time)
		{
			for (int i = 0; i < _beads.Count; i++)
			{
				if (!_beads[i].Arrived && Counts(_beads[i]))
				{
					return false;
				}
			}
			for (int j = 0; j < _sources.Count; j++)
			{
				if (_sources[j].CapShow >= 0.03f)
				{
					return false;
				}
			}
			return time - LastHome > 0.2f;
		}

		public void Free()
		{
			try
			{
				if (FxKit.Alive((GodotObject)(object)_top))
				{
					((Node)_top).QueueFree();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Beads cleanup: " + ex.Message);
			}
			_top = null;
		}

		private void FindCounter(Source src)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = GameRefs.EnergyPoint();
			float size = 128f;
			try
			{
				Control energyCounter = GameRefs.EnergyCounter;
				if (FxKit.Alive((GodotObject)(object)energyCounter) && ((Node)energyCounter).IsInsideTree())
				{
					Vector2 size2 = energyCounter.Size;
					if (size2.X > 30f && size2.Y > 30f)
					{
						Vector2 val2 = ((CanvasItem)energyCounter).GetGlobalTransform() * (size2 * 0.5f);
						Vector2 val3 = FxKit.ViewSize();
						if (val2.X > 0f && val2.X < val3.X && val2.Y > 0f && val2.Y < val3.Y)
						{
							val = val2;
							Vector2 val4 = GameRefs.DrawnSize(energyCounter, (Node)(object)Holder);
							size = Mathf.Clamp(Mathf.Min(val4.X, val4.Y), 70f, 240f);
						}
					}
				}
			}
			catch
			{
			}
			src.Point = val;
			src.Spot = val;
			src.Size = size;
		}

		private static Control ChemIcon()
		{
			try
			{
				NRun instance = NRun.Instance;
				if (!FxKit.Alive((GodotObject)(object)instance))
				{
					return null;
				}
				NGlobalUi globalUi = instance.GlobalUi;
				if (!FxKit.Alive((GodotObject)(object)globalUi))
				{
					return null;
				}
				NRelicInventory relicInventory = globalUi.RelicInventory;
				if (!FxKit.Alive((GodotObject)(object)relicInventory) || !((Node)relicInventory).IsInsideTree())
				{
					return null;
				}
				IReadOnlyList<NRelicInventoryHolder> relicNodes = relicInventory.RelicNodes;
				if (relicNodes == null)
				{
					return null;
				}
				for (int i = 0; i < relicNodes.Count; i++)
				{
					NRelicInventoryHolder val = relicNodes[i];
					if (!FxKit.Alive((GodotObject)(object)val))
					{
						continue;
					}
					NRelic relic = val.Relic;
					if (FxKit.Alive((GodotObject)(object)relic) && relic.Model is ChemicalX)
					{
						TextureRect icon = relic.Icon;
						if (FxKit.Alive((GodotObject)(object)icon) && ((Node)icon).IsInsideTree() && ((CanvasItem)icon).IsVisibleInTree())
						{
							return (Control)(object)icon;
						}
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Beads: the relic bar could not be read: " + ex.Message);
			}
			return null;
		}

		private int FindRelic()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Expected Obj, but got Unknown
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected Obj, but got Unknown
			Control val = ChemIcon();
			if (val == null)
			{
				return -1;
			}
			Source source = new Source
			{
				Relic = true,
				Icon = val,
				CapTint = Ink,
				Layer = Holder
			};
			try
			{
				Node overlay = GameRefs.Overlay;
				if (FxKit.Alive((GodotObject)(object)overlay) && overlay.IsInsideTree())
				{
					CanvasItem val2 = (CanvasItem)(object)((overlay is CanvasItem) ? overlay : null);
					if (val2 == null || val2.IsVisibleInTree())
					{
						_top = new Node2D
						{
							Name = StringName.op_Implicit("SilentFX_BeadsTop")
						};
						TimeFreeze.KeepLive((Node)(object)_top);
						overlay.AddChild((Node)(object)_top, false, (InternalMode)0);
						if (((Node)_top).IsInsideTree())
						{
							source.Layer = _top;
						}
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Beads: no layer over the top bar: " + ex.Message);
			}
			if (!Measure(source))
			{
				Free();
				return -1;
			}
			if (Tuning.F("beads.relic_cap", 1f) >= 0.5f)
			{
				source.Cap = new Sprite2D
				{
					Texture = CapTexture,
					Modulate = A(Ink, 0f)
				};
				((Node)source.Layer).AddChild((Node)(object)source.Cap, false, (InternalMode)0);
				source.Ring = FxKit.Sigil(100f, Rim, out source.RingMat);
				((Node)source.Layer).AddChild((Node)(object)source.Ring, false, (InternalMode)0);
				if (source.RingMat != null)
				{
					source.RingMat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.76f));
					source.RingMat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.03f));
					source.RingMat.SetShaderParameter(StringName.op_Implicit("dashes"), Variant.op_Implicit(16f));
					source.RingMat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.2f));
					source.RingMat.SetShaderParameter(PAlpha, Variant.op_Implicit(0f));
				}
			}
			_sources.Add(source);
			return _sources.Count - 1;
		}

		private bool Measure(Source src)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Control icon = src.Icon;
				if (!FxKit.Alive((GodotObject)(object)icon) || !((Node)icon).IsInsideTree() || !((CanvasItem)icon).IsVisibleInTree())
				{
					return false;
				}
				Vector2 size = icon.Size;
				if (size.X < 4f || size.Y < 4f)
				{
					return false;
				}
				Transform2D globalTransform = ((CanvasItem)icon).GetGlobalTransform();
				Vector2 val = globalTransform * (size * 0.5f);
				Vector2 val2 = ((CanvasItem)icon).GetCanvasTransform() * val;
				Vector2 val3 = FxKit.ViewSize();
				if (!(val2.X > 0f) || !(val2.X < val3.X) || !(val2.Y > 0f) || !(val2.Y < val3.Y))
				{
					return false;
				}
				Node2D val4 = (FxKit.Alive((GodotObject)(object)src.Layer) ? src.Layer : Holder);
				Vector2 val5 = Into((CanvasItem)(object)icon, (CanvasItem)(object)val4, globalTransform * Vector2.Zero);
				Vector2 val6 = Into((CanvasItem)(object)icon, (CanvasItem)(object)val4, globalTransform * new Vector2(size.X, 0f));
				Vector2 val7 = Into((CanvasItem)(object)icon, (CanvasItem)(object)val4, globalTransform * new Vector2(0f, size.Y));
				Vector2 val8;
				if (!FxKit.Alive((GodotObject)(object)val4))
				{
					val8 = Vector2.One;
				}
				else
				{
					Transform2D globalTransform2 = ((CanvasItem)val4).GetGlobalTransform();
					Vector2 scale = globalTransform2.Scale;
					val8 = scale.Abs();
				}
				Vector2 val9 = val8;
				float num = val5.DistanceTo(val6) / Mathf.Max(0.01f, val9.X);
				float num2 = val5.DistanceTo(val7) / Mathf.Max(0.01f, val9.Y);
				src.Point = Into((CanvasItem)(object)icon, (CanvasItem)(object)Holder, val);
				src.Spot = Into((CanvasItem)(object)icon, (CanvasItem)(object)val4, val);
				src.Size = Mathf.Clamp(Mathf.Min(num, num2), 24f, 200f);
				return true;
			}
			catch (Exception ex)
			{
				if (!_warned)
				{
					DevLog.Warn("Beads: the relic's icon could not be measured: " + ex.Message);
				}
				_warned = true;
				return false;
			}
		}

		internal static Vector2 Into(CanvasItem from, CanvasItem to, Vector2 global)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)from) || !FxKit.Alive((GodotObject)(object)to) || !((Node)from).IsInsideTree() || !((Node)to).IsInsideTree())
			{
				return global;
			}
			Transform2D canvasTransform = from.GetCanvasTransform();
			Transform2D canvasTransform2 = to.GetCanvasTransform();
			if (canvasTransform.X.IsEqualApprox(canvasTransform2.X) && canvasTransform.Y.IsEqualApprox(canvasTransform2.Y) && canvasTransform.Origin.IsEqualApprox(canvasTransform2.Origin))
			{
				return global;
			}
			return canvasTransform2.AffineInverse() * (canvasTransform * global);
		}

		static EnergyBeads()
		{
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
		}
	}
