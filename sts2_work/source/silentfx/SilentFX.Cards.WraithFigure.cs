// namespace: SilentFX.Cards
// type: WraithFigure (class)
// source lines: 115991-117212
	internal sealed class WraithFigure
	{
		private sealed class Strip
		{
			public Line2D Line;

			public int Kind;

			public float Sway;

			public float Peak;

			public Curve Laid;

			public bool Loose;

			public Vector2[] Track;

			public float[] Cum;

			public float Len;

			public float Place;

			public float Delay;

			public float Travel;

			public float Hover;

			public float Ph1;

			public float Ph2;

			public float W1;

			public float W2;

			public float AwayDelay;

			public readonly Vector2[] Pts = new Vector2[16];
		}

		private sealed class Dark
		{
			public Line2D Line;

			public Line2D Rim;

			public int Kind;

			public int Index;

			public bool Face;

			public Vector2[] Fig;

			public Vector2[] Buf;

			public float Shown = -1f;
		}

		private sealed class Flame
		{
			public Node2D Holder;

			public ShaderMaterial Mat;

			public Sprite2D Glow;

			public float Seed;

			public float Lean;
		}

		public const float FormTime = 0.85f;

		public const float AwayTime = 0.5f;

		private const int Joints = 16;

		private const int Guide = 40;

		private static readonly StringName PSolid = StringName.op_Implicit("solid");

		private static readonly StringName PGlow = StringName.op_Implicit("glow");

		private static readonly StringName PPower = StringName.op_Implicit("power");

		private static readonly StringName PClock = StringName.op_Implicit("clock");

		private static readonly StringName PLean = StringName.op_Implicit("lean");

		private static readonly StringName PSeed = StringName.op_Implicit("seed");

		private readonly List<Strip> _strips = new List<Strip>();

		private readonly List<Dark> _darks = new List<Dark>();

		private readonly List<Flame> _flames = new List<Flame>();

		private ShaderMaterial _gauze;

		private Sprite2D _under;

		private Sprite2D _low;

		private Sprite2D _core;

		private Sprite2D _eye;

		private Vector2 _eyeSize;

		private static Texture2D _white;

		private static Curve _loose;

		private static Texture2D _soft;

		public float Wake;

		public float Reach = 1f;

		public float Form;

		public float Solid;

		public float Hands;

		public float Blast;

		public float Away;

		public float Fire = 1f;

		public float Clock;

		public Node2D Root { get; private set; }

		public Vector2 At
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return field;
			}
			[CompilerGenerated]
			private set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				field = value;
			}
		}

		public float R { get; private set; }

		public Vector2 Head
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return field;
			}
			[CompilerGenerated]
			private set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				field = value;
			}
		}

		public Vector2 FistLeft
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return field;
			}
			[CompilerGenerated]
			private set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				field = value;
			}
		}

		public Vector2 FistRight
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return field;
			}
			[CompilerGenerated]
			private set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				field = value;
			}
		}

		public bool Alive => FxKit.Alive((GodotObject)(object)Root);

		private static Curve LooseCurve
		{
			get
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Expected Obj, but got Unknown
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				if (!FxKit.Alive((GodotObject)(object)_loose))
				{
					_loose = new Curve();
					_loose.AddPoint(new Vector2(0f, 0.55f), 0f, 0f, (TangentMode)1, (TangentMode)1);
					_loose.AddPoint(new Vector2(0.1f, 1f), 0f, 0f, (TangentMode)1, (TangentMode)1);
					_loose.AddPoint(new Vector2(0.82f, 0.9f), 0f, 0f, (TangentMode)1, (TangentMode)1);
					_loose.AddPoint(new Vector2(1f, 0.28f), 0f, 0f, (TangentMode)1, (TangentMode)1);
				}
				return _loose;
			}
		}

		private static Texture2D White
		{
			get
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				if (!FxKit.Alive((GodotObject)(object)_white))
				{
					_white = (Texture2D)(object)FxKit.Ramp(new float[2] { 0f, 1f }, new Color[2]
					{
						Colors.White,
						Colors.White
					});
				}
				return _white;
			}
		}

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

		public static void Place(CardFxContext c, Vector2 lo, Vector2 hi, out Vector2 at, out float r)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = hi - lo;
			r = Mathf.Min(val.Y * 0.3f, val.X * 0.19f) * Mathf.Clamp(Tuning.F("wraith.figure.size", 1f), 0.4f, 1.6f);
			float num = Mathf.Clamp(c.Body.Y, 160f, 620f);
			float num2 = lo.X + val.X * 0.5f;
			float num3 = lo.Y + val.Y * 0.36f;
			if (Tuning.F("wraith.figure.follow", 1f) >= 0.5f)
			{
				num2 = c.Chest.X;
				num3 = Mathf.Min(c.Chest.Y, c.Feet.Y - num) + num * 0.1f - r * 0.47f;
			}
			float num4 = Mathf.Min(r * 1f, val.X * 0.5f);
			num2 = Mathf.Clamp(num2, lo.X + num4, hi.X - num4);
			num3 = Mathf.Clamp(num3, lo.Y + r * 1.09f, lo.Y + val.Y * 0.5f);
			at = new Vector2(num2, num3);
		}

		public static WraithFigure Make(Node parent, Vector2 at, float r, Vector2 lo, Vector2 hi)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected Obj, but got Unknown
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return null;
			}
			WraithFigure wraithFigure = new WraithFigure
			{
				At = at,
				R = Mathf.Max(40f, r)
			};
			wraithFigure.Root = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_WraithFigure")
			};
			TimeFreeze.KeepLive((Node)(object)wraithFigure.Root);
			parent.AddChild((Node)(object)wraithFigure.Root, false, (InternalMode)0);
			try
			{
				wraithFigure.Build(lo, hi);
				return wraithFigure;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Wraith Form figure: " + ex.Message);
				wraithFigure.Free();
				return null;
			}
		}

		public void MoveTo(Node parent)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)Root) || !FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Node parent2 = ((Node)Root).GetParent();
			if (parent2 != parent)
			{
				if (parent2 != null)
				{
					parent2.RemoveChild((Node)(object)Root);
				}
				parent.AddChild((Node)(object)Root, false, (InternalMode)0);
				((CanvasItem)Root).Modulate = Colors.White;
			}
		}

		public void Free()
		{
			if (FxKit.Alive((GodotObject)(object)Root))
			{
				((Node)Root).QueueFree();
			}
			Root = null;
			_strips.Clear();
			_darks.Clear();
			_flames.Clear();
		}

		private Vector2 P(float x, float y)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			return At + new Vector2(x, y) * R;
		}

		private void Build(Vector2 lo, Vector2 hi)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Expected Obj, but got Unknown
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			float r = R;
			Head = P(WraithFigureData.HeadAt[0], WraithFigureData.HeadAt[1]);
			FistLeft = P(WraithFigureData.FistLeft[0], WraithFigureData.FistLeft[1]);
			FistRight = P(WraithFigureData.FistRight[0], WraithFigureData.FistRight[1]);
			_gauze = ShaderLib.NewMaterial("wraith_gauze");
			_under = FxKit.Glow(new Color(0.1f, 0.5f, 0.22f, 0f), new Vector2(r * 3.6f, r * 3.4f));
			((Node2D)_under).Position = At + new Vector2(0f, r * 0.1f);
			((Node)Root).AddChild((Node)(object)_under, false, (InternalMode)0);
			_low = FxKit.Glow(new Color(0.22f, 0.62f, 0.14f, 0f), new Vector2(r * 3.4f, r * 2.3f));
			((Node2D)_low).Position = At + new Vector2(0f, r * 1.05f);
			((Node)Root).AddChild((Node)(object)_low, false, (InternalMode)0);
			_core = new Sprite2D
			{
				Texture = FxKit.SoftDot,
				Modulate = new Color(0.02f, 0.07f, 0.05f, 0f),
				Scale = new Vector2(r * 0.95f, r * 1.9f) / 64f,
				Position = At + new Vector2(0f, (0f - r) * 0.05f)
			};
			((Node)Root).AddChild((Node)(object)_core, false, (InternalMode)0);
			float[] data = WraithFigureData.Data;
			List<int> list = new List<int>();
			for (int i = 0; i < data.Length; i += 3 + (int)data[i + 2] * 3)
			{
				list.Add(i);
			}
			int[] array = new int[10] { 6, 5, 2, 4, 8, 1, 3, 7, 0, 77 };
			foreach (int num in array)
			{
				for (int k = 0; k < list.Count; k++)
				{
					int num2 = list[k];
					int num3 = (int)data[num2];
					bool flag = num3 == 7 && data[num2 + 4] > -0.45f;
					if (num == 77)
					{
						if (!flag)
						{
							continue;
						}
					}
					else if ((num3 != num) | flag)
					{
						continue;
					}
					if (num3 <= 2 || num3 == 8)
					{
						AddStrip(k, num2, lo, hi);
					}
					else
					{
						AddDark(k, num2, flag);
					}
				}
			}
			_eyeSize = new Vector2(r * 0.2f, r * 0.16f);
			_eye = FxKit.Glow(new Color(0.3f, 1f, 0.22f, 0f), _eyeSize);
			((Node2D)_eye).Position = P(WraithFigureData.EyeAt[0], WraithFigureData.EyeAt[1]);
			((Node)Root).AddChild((Node)(object)_eye, false, (InternalMode)0);
			AddFlame(FistLeft, 1.3f, -0.25f);
			AddFlame(FistRight, 6.1f, 0.25f);
		}

		private void AddStrip(int index, int o, Vector2 lo, Vector2 hi)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Expected Obj, but got Unknown
			float[] data = WraithFigureData.Data;
			int num = (int)data[o + 2];
			Vector2[] array = new Vector2[num];
			float[] array2 = new float[num];
			float num2 = 0.001f;
			for (int i = 0; i < num; i++)
			{
				int num3 = o + 3 + i * 3;
				array[i] = P(data[num3], data[num3 + 1]);
				array2[i] = data[num3 + 2] * R;
				num2 = Mathf.Max(num2, array2[i]);
			}
			int num4 = index * 7;
			Strip strip = new Strip
			{
				Kind = (int)data[o],
				Sway = data[o + 1],
				Peak = num2
			};
			Vector2 val = array[0] - At;
			float num5 = Mathf.Max(val.Length(), 1f);
			float num6 = val.Angle();
			float num7 = Mathf.PosMod((float)Math.PI * 2f * Mathf.PosMod((float)index * 0.618034f + 0.11f, 1f) - num6 - 1.8849558f, (float)Math.PI * 2f) + 1.8849558f;
			Vector2 dir = Vector2.FromAngle(num6 + num7);
			float num8 = EdgeReach(At, dir, lo, hi, 70f);
			Vector2[] array3 = new Vector2[40 + num];
			for (int j = 0; j < 40; j++)
			{
				float num9 = (float)j / 40f;
				float num10 = num6 + num7 * (1f - num9 * num9);
				float num11 = num5 + (num8 - num5) * Mathf.Pow(1f - num9, 1.25f);
				array3[j] = At + Vector2.FromAngle(num10) * num11;
			}
			for (int k = 0; k < num; k++)
			{
				array3[40 + k] = array[k];
			}
			for (int l = 0; l < 4; l++)
			{
				for (int m = 30; m < 42 && m < array3.Length - 1; m++)
				{
					array3[m] = array3[m] * 0.5f + (array3[m - 1] + array3[m + 1]) * 0.25f;
				}
			}
			float[] array4 = new float[array3.Length];
			for (int n = 1; n < array3.Length; n++)
			{
				array4[n] = array4[n - 1] + array3[n].DistanceTo(array3[n - 1]);
			}
			strip.Track = array3;
			strip.Cum = array4;
			strip.Len = array4[^1];
			strip.Place = strip.Len - array4[40];
			strip.Delay = 0.3f * FxKit.Hash(num4 + 1) + ((strip.Kind == 2) ? 0.1f : 0f);
			strip.Travel = 0.42f + 0.12f * FxKit.Hash(num4 + 2);
			strip.Hover = (230f + 260f * FxKit.Hash(num4 + 3)) * Mathf.Clamp(R / 324f, 0.5f, 1.6f);
			strip.Ph1 = (float)Math.PI * 2f * FxKit.Hash(num4 + 4);
			strip.Ph2 = (float)Math.PI * 2f * FxKit.Hash(num4 + 5);
			strip.W1 = 2.6f + 2.2f * FxKit.Hash(num4 + 6);
			strip.W2 = 1.1f + 0.9f * FxKit.Hash(num4 + 8);
			strip.AwayDelay = 0.22f * FxKit.Hash(num4 + 9) + ((strip.Kind == 2 || strip.Kind == 8) ? 0f : 0.06f);
			strip.Laid = WidthCurve(array2, num2);
			float num12 = FxKit.Hash(num4 + 10);
			float num13 = ((strip.Kind >= 2) ? 0.86f : (0.66f + 0.34f * FxKit.Hash(num4 + 12)));
			strip.Line = new Line2D
			{
				Width = num2,
				WidthCurve = strip.Laid,
				Texture = White,
				TextureMode = (LineTextureMode)2,
				JointMode = (LineJointMode)2,
				DefaultColor = new Color((0.86f + 0.14f * num12) * num13, (0.94f + 0.06f * num12) * num13, (0.88f + 0.12f * FxKit.Hash(num4 + 11)) * num13, 1f),
				Visible = false
			};
			if (_gauze != null)
			{
				((CanvasItem)strip.Line).Material = (Material)(object)_gauze;
			}
			((Node)Root).AddChild((Node)(object)strip.Line, false, (InternalMode)0);
			_strips.Add(strip);
		}

		private void AddDark(int index, int o, bool face)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Expected Obj, but got Unknown
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Expected Obj, but got Unknown
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			float[] data = WraithFigureData.Data;
			int num = (int)data[o];
			int num2 = (int)data[o + 2];
			Vector2[] array = new Vector2[num2];
			float[] array2 = new float[num2];
			float num3 = 0.001f;
			for (int i = 0; i < num2; i++)
			{
				int num4 = o + 3 + i * 3;
				array[i] = P(data[num4], data[num4 + 1]);
				array2[i] = data[num4 + 2] * R;
				num3 = Mathf.Max(num3, array2[i]);
			}
			Dark dark = new Dark
			{
				Kind = num,
				Index = index,
				Face = face,
				Fig = array,
				Buf = new Vector2[num2]
			};
			Line2D val = new Line2D
			{
				Width = num3,
				WidthCurve = WidthCurve(array2, num3),
				JointMode = (LineJointMode)2,
				Antialiased = true,
				Visible = false
			};
			switch (num)
			{
			case 3:
			case 4:
			{
				float num5 = R * 0.03f;
				float[] array3 = new float[num2];
				for (int j = 0; j < num2; j++)
				{
					array3[j] = array2[j] + num5;
				}
				dark.Rim = new Line2D
				{
					Width = num3 + num5,
					WidthCurve = WidthCurve(array3, num3 + num5),
					JointMode = (LineJointMode)2,
					Antialiased = true,
					BeginCapMode = (LineCapMode)2,
					EndCapMode = (LineCapMode)((num == 4) ? 2 : 0),
					DefaultColor = new Color(0.74f, 1f, 0.26f, 1f),
					Visible = false
				};
				((Node)Root).AddChild((Node)(object)dark.Rim, false, (InternalMode)0);
				val.DefaultColor = ((num == 3) ? new Color(0.09f, 0.105f, 0.17f, 1f) : new Color(0.02f, 0.05f, 0.045f, 1f));
				val.BeginCapMode = (LineCapMode)2;
				val.EndCapMode = (LineCapMode)((num == 4) ? 2 : 0);
				break;
			}
			case 5:
				val.Texture = Soft;
				val.TextureMode = (LineTextureMode)2;
				val.DefaultColor = new Color(0.016f, 0.06f, 0.036f, 1f);
				val.Antialiased = false;
				break;
			case 6:
				val.DefaultColor = new Color(0.035f, 0.105f, 0.06f, 1f);
				break;
			default:
				val.DefaultColor = new Color(0.3f, 1f, 0.16f, 1f);
				((CanvasItem)val).Material = (Material)(object)FxKit.Additive;
				val.BeginCapMode = (LineCapMode)2;
				val.EndCapMode = (LineCapMode)2;
				break;
			}
			dark.Line = val;
			((Node)Root).AddChild((Node)(object)val, false, (InternalMode)0);
			_darks.Add(dark);
		}

		private void AddFlame(Vector2 fist, float seed, float lean)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected Obj, but got Unknown
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			float r = R;
			Flame flame = new Flame
			{
				Seed = seed,
				Lean = lean
			};
			flame.Holder = new Node2D
			{
				Position = fist
			};
			((Node)Root).AddChild((Node)(object)flame.Holder, false, (InternalMode)0);
			flame.Glow = FxKit.Glow(new Color(0.35f, 1f, 0.3f, 0f), new Vector2(r * 0.8f, r * 0.8f));
			((Node2D)flame.Glow).Position = new Vector2(0f, (0f - r) * 0.12f);
			((Node)flame.Holder).AddChild((Node)(object)flame.Glow, false, (InternalMode)0);
			flame.Mat = ShaderLib.NewMaterial("wraith_flame");
			if (flame.Mat != null)
			{
				flame.Mat.SetShaderParameter(PPower, Variant.op_Implicit(0f));
				flame.Mat.SetShaderParameter(PClock, Variant.op_Implicit(0f));
				flame.Mat.SetShaderParameter(PSeed, Variant.op_Implicit(seed));
				flame.Mat.SetShaderParameter(PLean, Variant.op_Implicit(lean));
				Vector2 val = new Vector2(r * 0.6f, r * 1f);
				ColorRect val2 = FxKit.Quad(val, (Material)(object)flame.Mat);
				((Control)val2).Position = new Vector2((0f - val.X) * 0.5f, (0f - val.Y) * 0.88f);
				((Node)flame.Holder).AddChild((Node)(object)val2, false, (InternalMode)0);
			}
			_flames.Add(flame);
		}

		private static Curve WidthCurve(float[] widths, float peak)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected Obj, but got Unknown
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			Curve val = new Curve();
			int num = widths.Length;
			for (int i = 0; i < num; i++)
			{
				val.AddPoint(new Vector2((float)i / (float)(num - 1), Mathf.Clamp(widths[i] / peak, 0f, 1f)), 0f, 0f, (TangentMode)1, (TangentMode)1);
			}
			return val;
		}

		private static float EdgeReach(Vector2 from, Vector2 dir, Vector2 lo, Vector2 hi, float margin)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			float num;
			if (dir.X > 0.0001f)
			{
				num = (hi.X + margin - from.X) / dir.X;
			}
			else
			{
				num = ((dir.X < -0.0001f) ? ((lo.X - margin - from.X) / dir.X) : 100000f);
			}
			float num2;
			if (dir.Y > 0.0001f)
			{
				num2 = (hi.Y + margin - from.Y) / dir.Y;
			}
			else
			{
				num2 = ((dir.Y < -0.0001f) ? ((lo.Y - margin - from.Y) / dir.Y) : 100000f);
			}
			return Mathf.Clamp(Mathf.Min(num, num2), 200f, 5000f);
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

		private static float Smooth(float v)
		{
			v = Clamp01(v);
			return v * v * (3f - 2f * v);
		}

		public void Step()
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_0396: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)Root))
			{
				return;
			}
			float num = 1f - Smooth(Away * 1.6f - 0.3f);
			float num2 = Smooth(Form);
			float num3 = Mathf.Max(0f, Blast);
			if (FxKit.Alive((GodotObject)(object)_gauze))
			{
				_gauze.SetShaderParameter(PSolid, Variant.op_Implicit(Clamp01(Solid)));
				_gauze.SetShaderParameter(PGlow, Variant.op_Implicit(Clamp01(num3)));
			}
			if (FxKit.Alive((GodotObject)(object)_under))
			{
				((CanvasItem)_under).Modulate = new Color(0.1f, 0.5f, 0.22f, 0.26f * num2 * num);
			}
			if (FxKit.Alive((GodotObject)(object)_low))
			{
				((CanvasItem)_low).Modulate = new Color(0.22f, 0.62f, 0.14f, 0.6f * Clamp01(Solid) * num);
			}
			if (FxKit.Alive((GodotObject)(object)_core))
			{
				((CanvasItem)_core).Modulate = new Color(0.02f, 0.07f, 0.05f, 0.78f * Clamp01(Solid) * num);
			}
			for (int i = 0; i < _strips.Count; i++)
			{
				PoseStrip(_strips[i]);
			}
			for (int j = 0; j < _darks.Count; j++)
			{
				PoseDark(_darks[j], num);
			}
			if (FxKit.Alive((GodotObject)(object)_eye))
			{
				float num4 = Clamp01((0.18f * Clamp01(Form) + 0.42f * Clamp01(Solid) + 0.8f * num3) * num);
				((CanvasItem)_eye).Modulate = new Color(0.3f, 1f, 0.22f, num4);
				((Node2D)_eye).Scale = _eyeSize * (1f + 2.5f * num3) / 64f;
			}
			for (int k = 0; k < _flames.Count; k++)
			{
				Flame flame = _flames[k];
				if (!FxKit.Alive((GodotObject)(object)flame.Holder))
				{
					continue;
				}
				float num5 = Smooth(Hands * 1.6f - 0.6f) * (1f + 0.5f * num3) * Mathf.Max(0f, Fire) * (1f + 0.08f * Mathf.Sin(Clock * 33f + flame.Seed * 5f) + 0.05f * Mathf.Sin(Clock * 57f + flame.Seed * 9f));
				num5 = Mathf.Clamp(num5, 0f, 1.6f);
				bool flag = num5 > 0.01f;
				if (((CanvasItem)flame.Holder).Visible != flag)
				{
					((CanvasItem)flame.Holder).Visible = flag;
				}
				if (flag)
				{
					if (FxKit.Alive((GodotObject)(object)flame.Mat))
					{
						flame.Mat.SetShaderParameter(PPower, Variant.op_Implicit(num5));
						flame.Mat.SetShaderParameter(PClock, Variant.op_Implicit(Clock));
						flame.Mat.SetShaderParameter(PLean, Variant.op_Implicit(flame.Lean + 0.12f * Mathf.Sin(Clock * 4.3f + flame.Seed)));
					}
					if (FxKit.Alive((GodotObject)(object)flame.Glow))
					{
						((CanvasItem)flame.Glow).Modulate = new Color(0.35f, 1f, 0.3f, 0.4f * Mathf.Min(num5, 1.3f));
					}
				}
			}
		}

		private static Vector2 Sample(Strip s, float along)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			Vector2[] track = s.Track;
			float[] cum = s.Cum;
			if (along <= 0f)
			{
				return track[0];
			}
			if (along >= s.Len)
			{
				return track[^1];
			}
			int num = 0;
			int num2 = cum.Length - 1;
			while (num2 - num > 1)
			{
				int num3 = num + num2 >> 1;
				if (cum[num3] <= along)
				{
					num = num3;
				}
				else
				{
					num2 = num3;
				}
			}
			float num4 = cum[num2] - cum[num];
			if (!(num4 < 1E-05f))
			{
				return track[num].Lerp(track[num2], (along - cum[num]) / num4);
			}
			return track[num];
		}

		private void PoseStrip(Strip s)
		{
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0368: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_0538: Unknown result type (might be due to invalid IL or missing references)
			Line2D line = s.Line;
			if (!FxKit.Alive((GodotObject)(object)line))
			{
				return;
			}
			float clock = Clock;
			float num = ((s.Kind == 8) ? Hands : Form);
			float num2 = Smooth(Wake);
			if (s.Kind == 8)
			{
				num2 = Mathf.Min(num2, Smooth(num * 6f));
			}
			float num3 = ((num >= 1f) ? 1f : Smooth((num * 0.85f - s.Delay) / s.Travel));
			float num4 = Clamp01((Away * 0.5f - s.AwayDelay) / 0.26f);
			float num5 = 0.7f + 0.2f * Mathf.Sin(clock * s.W1 * 0.6f + s.Ph1) + 0.3f * Mathf.Pow(Mathf.Max(0f, Mathf.Sin(clock * 1.3f + s.Ph2 * 3f)), 6f);
			float num6 = Mathf.Lerp(s.Hover * num5 * num2 * Mathf.Clamp(Reach, 0f, 1.5f), s.Len, num3);
			float num7 = (s.Len - s.Place) * Mathf.Pow(num3, 1.7f);
			if (num4 > 0f)
			{
				float num8 = num4 * num4;
				num7 = (s.Len - s.Place) * (1f - Mathf.Min(1f, num8 * 1.5f));
				num6 = s.Len * (1f - num8) - 60f * num8;
			}
			if (num6 - num7 < 2f || num2 <= 0.001f || num4 >= 1f)
			{
				if (((CanvasItem)line).Visible)
				{
					((CanvasItem)line).Visible = false;
				}
				return;
			}
			float num9 = ((s.Kind <= 1 && num3 >= 1f) ? Blast : 0f);
			for (int i = 0; i < 16; i++)
			{
				float num10 = (float)i / 15f;
				float num11 = num7 + (num6 - num7) * num10;
				Vector2 val = Sample(s, num11);
				Vector2 val2 = Sample(s, num11 + 3f) - Sample(s, num11 - 3f);
				Vector2 val3;
				Vector2 val4;
				if (!(val2.LengthSquared() > 1E-08f))
				{
					val3 = Vector2.Zero;
				}
				else
				{
					val4 = new Vector2(0f - val2.Y, val2.X);
					val3 = val4.Normalized();
				}
				Vector2 val5 = val3;
				float num12 = 22f * Mathf.Pow(num10, 1.1f) * (1f - num3);
				float num13 = 24f * Mathf.Sin((float)Math.PI * num3) * (0.3f + 0.7f * num10);
				float num14 = s.Sway * R * 0.034f * Mathf.Pow(num10, 1.5f) * num3 * (1f - num4);
				float num15 = num12 * Mathf.Sin(clock * s.W1 - num10 * 5f + s.Ph1) + num13 * Mathf.Sin(clock * 9f - num10 * 7f + s.Ph2) + num14 * (Mathf.Sin(clock * s.W2 - num10 * 3.2f + s.Ph2) + 0.5f * Mathf.Sin(clock * s.W2 * 1.9f - num10 * 6f + s.Ph1));
				val += val5 * num15;
				if (num9 != 0f)
				{
					Vector2 val6 = val - Head;
					Vector2 val7 = ((val6.LengthSquared() > 1f) ? val6.Normalized() : Vector2.Up);
					val4 = val7 + new Vector2(0f, -0.35f);
					val7 = val4.Normalized();
					val += val7 * (num9 * R * 0.46f * Mathf.Pow(num10, 1.3f));
					val += val5 * (num9 * 7f * Mathf.Sin(clock * 86f + s.Ph1) * Mathf.Sin((float)Math.PI * num10));
				}
				s.Pts[i] = val;
			}
			line.Points = s.Pts;
			bool flag = num3 < 0.78f || num4 > 0.12f;
			if (flag != s.Loose)
			{
				s.Loose = flag;
				line.WidthCurve = (flag ? LooseCurve : s.Laid);
			}
			float num16 = (flag ? Mathf.Max(s.Peak, R * 0.05f) : s.Peak);
			line.Width = num16 * (1f + 0.3f * Mathf.Sin((float)Math.PI * num3));
			float v = num2 * (0.82f - 0.18f * num3 + 0.34f * Clamp01(Solid) * num3) * (1f - num4 * num4 * num4);
			((CanvasItem)line).Modulate = new Color(1f, 1f, 1f, Clamp01(v));
			if (!((CanvasItem)line).Visible)
			{
				((CanvasItem)line).Visible = true;
			}
		}

		private void PoseDark(Dark d, float gone)
		{
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			Line2D line = d.Line;
			if (!FxKit.Alive((GodotObject)(object)line))
			{
				return;
			}
			float num = Clamp01(Solid);
			float num2 = Smooth(Form * 1.25f - 0.25f);
			float num3;
			float num4;
			switch (d.Kind)
			{
			case 3:
				num3 = num2;
				num4 = (0.42f + 0.58f * num) * gone;
				break;
			case 4:
				num3 = Smooth(Hands);
				num4 = gone;
				break;
			case 5:
				num3 = Smooth(num * 1.2f);
				num4 = 0.92f * num * gone;
				break;
			case 6:
				num3 = Smooth(num * 1.6f - 0.5f);
				num4 = 0.85f * gone;
				break;
			default:
				num3 = 1f;
				num4 = Clamp01((0.3f * num2 + 0.7f * num) * (1f + 1.2f * Mathf.Max(0f, Blast))) * gone;
				break;
			}
			bool flag = FxKit.Alive((GodotObject)(object)d.Rim);
			if (num3 <= 0.01f || num4 <= 0.01f)
			{
				if (((CanvasItem)line).Visible)
				{
					((CanvasItem)line).Visible = false;
				}
				if (flag && ((CanvasItem)d.Rim).Visible)
				{
					((CanvasItem)d.Rim).Visible = false;
				}
				return;
			}
			bool flag2 = d.Kind == 6;
			if (flag2 || num3 != d.Shown)
			{
				Vector2[] array = d.Fig;
				int num5 = array.Length;
				if (flag2)
				{
					for (int i = 0; i < num5; i++)
					{
						float num6 = (float)i / (float)(num5 - 1);
						d.Buf[i] = array[i] + new Vector2(R * 0.03f * num6 * Mathf.Sin(Clock * 1.3f + (float)d.Index * 1.7f - num6 * 3f), 0f);
					}
					array = d.Buf;
				}
				Vector2[] points = (line.Points = ((num3 >= 0.999f) ? array : Partial(array, num3)));
				if (flag)
				{
					d.Rim.Points = points;
				}
				d.Shown = num3;
			}
			((CanvasItem)line).Modulate = new Color(1f, 1f, 1f, num4);
			if (!((CanvasItem)line).Visible)
			{
				((CanvasItem)line).Visible = true;
			}
			if (flag)
			{
				bool flag3 = num > 0.02f;
				if (((CanvasItem)d.Rim).Visible != flag3)
				{
					((CanvasItem)d.Rim).Visible = flag3;
				}
				if (flag3)
				{
					((CanvasItem)d.Rim).Modulate = new Color(1f, 1f, 1f, num4 * num);
				}
			}
		}

		private static Vector2[] Partial(Vector2[] pts, float shown)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			int num = pts.Length;
			float num2 = Clamp01(shown) * (float)(num - 1);
			int num3 = (int)Mathf.Floor(num2);
			bool flag = num3 < num - 1;
			Vector2[] array = new Vector2[num3 + 1 + (flag ? 1 : 0)];
			for (int i = 0; i <= num3; i++)
			{
				array[i] = pts[i];
			}
			if (flag)
			{
				array[num3 + 1] = pts[num3].Lerp(pts[num3 + 1], num2 - (float)num3);
			}
			return array;
		}
	}
