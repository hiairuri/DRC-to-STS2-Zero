// namespace: SilentFX.Core
// type: Props (class)
// source lines: 41086-41373
	public static class Props
	{
		public static Vector2[] Circle(float r, int points = 20, float sx = 1f, float sy = 1f)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			Vector2[] array = new Vector2[points];
			for (int i = 0; i < points; i++)
			{
				float num = (float)Math.PI * 2f * (float)i / (float)points;
				array[i] = new Vector2(Mathf.Cos(num) * r * sx, Mathf.Sin(num) * r * sy);
			}
			return array;
		}

		public static Vector2[] Teardrop(float r, float tip = 2.3f)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			Vector2[] array = new Vector2[15];
			for (int i = 0; i < 14; i++)
			{
				float num = -0.45f + 4.0415926f * (float)i / 13f;
				array[i] = new Vector2(Mathf.Cos(num) * r, Mathf.Sin(num) * r);
			}
			array[14] = new Vector2(0f, (0f - r) * tip);
			return array;
		}

		public static Vector2[] Box(float x0, float y0, float x1, float y1)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			return new Vector2[4]
			{
				new Vector2(x0, y0),
				new Vector2(x1, y0),
				new Vector2(x1, y1),
				new Vector2(x0, y1)
			};
		}

		public static Line2D Line(float width, Color color, bool additive, params Vector2[] points)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected Obj, but got Unknown
			return new Line2D
			{
				Width = width,
				DefaultColor = color,
				JointMode = (LineJointMode)2,
				BeginCapMode = (LineCapMode)2,
				EndCapMode = (LineCapMode)2,
				Material = (Material)(object)(additive ? FxKit.Additive : null),
				Antialiased = true,
				Points = (points ?? Array.Empty<Vector2>())
			};
		}

		public static Node2D Quill(float l, Color feather, Color edge, Color nib, Color glow)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected Obj, but got Unknown
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0342: Unknown result type (might be due to invalid IL or missing references)
			//IL_0357: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_041b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043a: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0452: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_046c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0471: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			Node2D val = new Node2D();
			if (glow.A > 0.01f)
			{
				Sprite2D val2 = FxKit.Glow(glow, new Vector2(l * 0.9f, l * 1.4f));
				((Node2D)val2).Position = new Vector2(0f, (0f - l) * 0.58f);
				((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
			}
			Vector2[] points = new Vector2[15]
			{
				new Vector2(0f, -0.34f * l),
				new Vector2(-0.11f * l, -0.42f * l),
				new Vector2(-0.17f * l, -0.55f * l),
				new Vector2(-0.07f * l, -0.585f * l),
				new Vector2(-0.175f * l, -0.64f * l),
				new Vector2(-0.16f * l, -0.77f * l),
				new Vector2(-0.06f * l, -0.795f * l),
				new Vector2(-0.12f * l, -0.9f * l),
				new Vector2(-0.02f * l, -1.05f * l),
				new Vector2(0.07f * l, -0.93f * l),
				new Vector2(0.105f * l, -0.8f * l),
				new Vector2(0.04f * l, -0.74f * l),
				new Vector2(0.115f * l, -0.68f * l),
				new Vector2(0.11f * l, -0.54f * l),
				new Vector2(0.065f * l, -0.43f * l)
			};
			((Node)val).AddChild((Node)(object)FxKit.Shape(points, feather), false, (InternalMode)0);
			Line2D val3 = Line(1.6f, edge, additive: false, points);
			val3.Closed = true;
			((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
			Color color = new Color(edge.R, edge.G, edge.B, edge.A * 0.6f);
			((Node)val).AddChild((Node)(object)Line(1.2f, color, false, new Vector2(0f, -0.46f * l), new Vector2(-0.13f * l, -0.53f * l)), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Line(1.2f, color, false, new Vector2(0f, -0.62f * l), new Vector2(-0.14f * l, -0.72f * l)), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Line(1.2f, color, false, new Vector2(0f, -0.8f * l), new Vector2(-0.09f * l, -0.88f * l)), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Line(1.2f, color, false, new Vector2(0f, -0.56f * l), new Vector2(0.09f * l, -0.63f * l)), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Line(1.2f, color, false, new Vector2(0f, -0.82f * l), new Vector2(0.07f * l, -0.89f * l)), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Line(2.4f, edge, false, new Vector2(0f, -0.1f * l), new Vector2(-0.015f * l, -1.02f * l)), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)FxKit.Shape(new Vector2[4]
			{
				new Vector2(0f, 0f),
				new Vector2(-0.034f * l, -0.09f * l),
				new Vector2(0f, -0.13f * l),
				new Vector2(0.034f * l, -0.09f * l)
			}, nib), false, (InternalMode)0);
			return val;
		}

		public static Node2D Vial(float r, Color glass, Color liquid, Color rim)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Expected Obj, but got Unknown
			Node2D val = new Node2D();
			((Node)val).AddChild((Node)(object)FxKit.Shape(Box((0f - r) * 0.36f, (0f - r) * 1.45f, r * 0.36f, (0f - r) * 0.6f), glass.Darkened(0.15f)), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)FxKit.Shape(Circle(r, 22, 1f, 0.86f), glass), false, (InternalMode)0);
			Polygon2D val2 = FxKit.Shape(Circle(r * 0.72f, 18, 1f, 0.7f), liquid);
			((Node2D)val2).Position = new Vector2(0f, r * 0.14f);
			((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
			Polygon2D val3 = FxKit.Shape(Circle(r * 0.16f, 10, 1f, 1.7f), new Color(1f, 1f, 1f, 0.55f));
			((Node2D)val3).Position = new Vector2((0f - r) * 0.46f, (0f - r) * 0.2f);
			((Node2D)val3).Rotation = 0.5f;
			((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
			Polygon2D val4 = FxKit.Shape(Circle(r * 0.5f, 14, 1f, 0.34f), rim);
			((Node2D)val4).Position = new Vector2(0f, (0f - r) * 1.5f);
			((Node)val).AddChild((Node)(object)val4, false, (InternalMode)0);
			return val;
		}

		public static Node2D Paw(Color color, float r)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected Obj, but got Unknown
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			Node2D val = new Node2D();
			((Node)val).AddChild((Node)(object)FxKit.Glow(new Color(color.R, color.G, color.B, 0.35f), new Vector2(r * 4.2f, r * 4.2f)), false, (InternalMode)0);
			Polygon2D val2 = FxKit.Shape(Circle(r * 0.62f, 14, 1.15f, 0.95f), color);
			((CanvasItem)val2).Material = (Material)(object)FxKit.Additive;
			((Node2D)val2).Position = new Vector2(0f, r * 0.28f);
			((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
			Vector2[] array = new Vector2[4]
			{
				new Vector2(-0.64f, -0.3f),
				new Vector2(-0.23f, -0.7f),
				new Vector2(0.23f, -0.7f),
				new Vector2(0.64f, -0.3f)
			};
			foreach (Vector2 val3 in array)
			{
				Polygon2D val4 = FxKit.Shape(Circle(r * 0.25f, 10, 0.8f, 1.2f), color);
				((CanvasItem)val4).Material = (Material)(object)FxKit.Additive;
				((Node2D)val4).Position = val3 * r;
				((Node2D)val4).Rotation = val3.X * 0.5f;
				((Node)val).AddChild((Node)(object)val4, false, (InternalMode)0);
			}
			return val;
		}
	}
