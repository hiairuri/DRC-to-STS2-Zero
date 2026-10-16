// namespace: SilentFX.Core
// type: FxKit (class)
// source lines: 35715-36785
	public static class FxKit
	{
		private static Texture2D _softDot;

		private static CanvasItemMaterial _additive;

		private static Texture2D _bubble;

		public static Texture2D SoftDot
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Expected Obj, but got Unknown
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00eb: Expected Obj, but got Unknown
				if (_softDot == null)
				{
					Gradient val = new Gradient();
					val.Offsets = new float[3] { 0f, 0.35f, 1f };
					val.Colors = new Color[3]
					{
						new Color(1f, 1f, 1f, 1f),
						new Color(1f, 1f, 1f, 0.5f),
						new Color(1f, 1f, 1f, 0f)
					};
					_softDot = (Texture2D)new GradientTexture2D
					{
						Gradient = val,
						Width = 64,
						Height = 64,
						Fill = (FillEnum)1,
						FillFrom = new Vector2(0.5f, 0.5f),
						FillTo = new Vector2(1f, 0.5f)
					};
				}
				return _softDot;
			}
		}

		public static CanvasItemMaterial Additive
		{
			get
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Expected Obj, but got Unknown
				if (_additive == null)
				{
					_additive = new CanvasItemMaterial
					{
						BlendMode = (BlendModeEnum)1
					};
				}
				return _additive;
			}
		}

		public static Texture2D Bubble
		{
			get
			{
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Expected Obj, but got Unknown
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0117: Unknown result type (might be due to invalid IL or missing references)
				//IL_0126: Expected Obj, but got Unknown
				if (_bubble == null)
				{
					Gradient val = new Gradient();
					val.Offsets = new float[5] { 0f, 0.55f, 0.8f, 0.9f, 1f };
					val.Colors = new Color[5]
					{
						new Color(1f, 1f, 1f, 0.1f),
						new Color(1f, 1f, 1f, 0.16f),
						new Color(1f, 1f, 1f, 1f),
						new Color(1f, 1f, 1f, 0.55f),
						new Color(1f, 1f, 1f, 0f)
					};
					_bubble = (Texture2D)new GradientTexture2D
					{
						Gradient = val,
						Width = 64,
						Height = 64,
						Fill = (FillEnum)1,
						FillFrom = new Vector2(0.5f, 0.5f),
						FillTo = new Vector2(1f, 0.5f)
					};
				}
				return _bubble;
			}
		}

		internal static void Warm()
		{
			try
			{
				_ = SoftDot;
				_ = Bubble;
				_ = Additive;
			}
			catch (Exception ex)
			{
				DevLog.Warn("FxKit warm-up: " + ex.Message);
			}
		}

		public static ColorRect Quad(Vector2 size, Material material)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Expected Obj, but got Unknown
			return new ColorRect
			{
				Color = Colors.White,
				Size = size,
				MouseFilter = (MouseFilterEnum)2,
				Material = material
			};
		}

		public static Sprite2D Glow(Color color, Vector2 size)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected Obj, but got Unknown
			return new Sprite2D
			{
				Texture = SoftDot,
				Material = (Material)(object)Additive,
				Modulate = color,
				Scale = size / 64f
			};
		}

		public static GradientTexture1D Ramp(float[] offsets, Color[] colors)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected Obj, but got Unknown
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected Obj, but got Unknown
			Gradient val = new Gradient();
			val.Offsets = offsets;
			val.Colors = colors;
			return new GradientTexture1D
			{
				Gradient = val,
				Width = 64
			};
		}

		public static GradientTexture1D FadeInOut(float peakAt = 0.15f)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			return Ramp(new float[3] { 0f, peakAt, 1f }, new Color[3]
			{
				new Color(1f, 1f, 1f, 0f),
				new Color(1f, 1f, 1f, 1f),
				new Color(1f, 1f, 1f, 0f)
			});
		}

		public static bool Alive(GodotObject o)
		{
			if (o == null || !GodotObject.IsInstanceValid(o))
			{
				return false;
			}
			Node val = (Node)(object)((o is Node) ? o : null);
			if (val != null)
			{
				return !((GodotObject)val).IsQueuedForDeletion();
			}
			return true;
		}

		public static Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			float num = 1f - t;
			return a * (num * num) + b * (2f * num * t) + c * (t * t);
		}

		public static Line2D Ribbon(float width, Color color)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected Obj, but got Unknown
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Expected Obj, but got Unknown
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Expected Obj, but got Unknown
			Curve val = new Curve();
			val.AddPoint(new Vector2(0f, 0.05f), 0f, 0f, (TangentMode)0, (TangentMode)0);
			val.AddPoint(new Vector2(0.7f, 1f), 0f, 0f, (TangentMode)0, (TangentMode)0);
			val.AddPoint(new Vector2(1f, 0.6f), 0f, 0f, (TangentMode)0, (TangentMode)0);
			Gradient val2 = new Gradient();
			val2.Offsets = new float[3] { 0f, 0.45f, 1f };
			val2.Colors = new Color[3]
			{
				new Color(color.R, color.G, color.B, 0f),
				new Color(color.R, color.G, color.B, color.A * 0.7f),
				color
			};
			return new Line2D
			{
				Width = width,
				WidthCurve = val,
				Gradient = val2,
				JointMode = (LineJointMode)2,
				BeginCapMode = (LineCapMode)2,
				EndCapMode = (LineCapMode)2,
				Material = (Material)(object)Additive,
				Antialiased = true
			};
		}

		private static Vector2[] Rect(float x0, float y0, float x1, float y1)
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

		public static Node2D Blade(float length, float halfWidth, Color light, Color dark, Color hilt, Color glow)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected Obj, but got Unknown
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Expected Obj, but got Unknown
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Expected Obj, but got Unknown
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Expected Obj, but got Unknown
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Expected Obj, but got Unknown
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Expected Obj, but got Unknown
			Node2D val = new Node2D();
			Sprite2D val2 = Glow(glow, new Vector2(halfWidth * 7f, length * 1.7f));
			((Node2D)val2).Position = new Vector2(0f, (0f - length) * 0.45f);
			((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
			Polygon2D val3 = new Polygon2D();
			val3.Polygon = new Vector2[4]
			{
				new Vector2(0f, 0f - length),
				new Vector2(0f, 0f),
				new Vector2((0f - halfWidth) * 0.75f, 0f),
				new Vector2(0f - halfWidth, (0f - length) * 0.2f)
			};
			val3.Color = light;
			val3.Antialiased = true;
			((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
			val3 = new Polygon2D();
			val3.Polygon = new Vector2[4]
			{
				new Vector2(0f, 0f - length),
				new Vector2(halfWidth, (0f - length) * 0.2f),
				new Vector2(halfWidth * 0.75f, 0f),
				new Vector2(0f, 0f)
			};
			val3.Color = dark;
			val3.Antialiased = true;
			((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
			if (hilt.A > 0.01f)
			{
				((Node)val).AddChild((Node)new Polygon2D
				{
					Polygon = Rect((0f - halfWidth) * 1.8f, 0f, halfWidth * 1.8f, length * 0.07f),
					Color = hilt,
					Antialiased = true
				}, false, (InternalMode)0);
				((Node)val).AddChild((Node)new Polygon2D
				{
					Polygon = Rect((0f - halfWidth) * 0.42f, length * 0.07f, halfWidth * 0.42f, length * 0.36f),
					Color = hilt.Darkened(0.25f),
					Antialiased = true
				}, false, (InternalMode)0);
				((Node)val).AddChild((Node)new Polygon2D
				{
					Polygon = Rect((0f - halfWidth) * 0.7f, length * 0.36f, halfWidth * 0.7f, length * 0.42f),
					Color = hilt,
					Antialiased = true
				}, false, (InternalMode)0);
			}
			return val;
		}

		public static void FreeAfter(Node node, double seconds)
		{
			FxLifetime.Watch(node, seconds);
		}

		public static float Damp(float current, float target, float rate, double dt)
		{
			return Mathf.Lerp(current, target, 1f - Mathf.Exp((0f - rate) * (float)dt));
		}

		public static Vector2 ViewSize()
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Vector2 result;
				if (Ticker.Tree == null)
				{
					result = new Vector2(1920f, 1080f);
				}
				else
				{
					Rect2 visibleRect = ((Viewport)Ticker.Tree.Root).GetVisibleRect();
					result = visibleRect.Size;
				}
				return result;
			}
			catch
			{
				return new Vector2(1920f, 1080f);
			}
		}

		public static GpuParticles2D Burst(Node parent, Vector2 globalPos, Color color, int amount, float velMin, float velMax, double life, float scaleMin, float scaleMax, Vector2 gravity, float spread = 180f, Vector2? dir = null, float radius = 10f, float damping = 0f, bool additive = true, float explosiveness = 0.95f)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected Obj, but got Unknown
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Expected Obj, but got Unknown
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			if (!Alive((GodotObject)(object)parent))
			{
				return null;
			}
			color = AccessibleColor(color);
			if (PlayerSettings.ReduceScreenEffects)
			{
				amount = Math.Max(1, (int)Math.Ceiling((float)amount * 0.65f));
			}
			Vector2 val = dir ?? new Vector2(0f, -1f);
			ParticleProcessMaterial val2 = new ParticleProcessMaterial();
			val2.EmissionShape = (EmissionShapeEnum)1;
			val2.EmissionSphereRadius = radius;
			val2.Direction = new Vector3(val.X, val.Y, 0f);
			val2.Spread = spread;
			val2.InitialVelocityMin = velMin;
			val2.InitialVelocityMax = velMax;
			val2.Gravity = new Vector3(gravity.X, gravity.Y, 0f);
			val2.DampingMin = damping;
			val2.DampingMax = damping;
			val2.ScaleMin = scaleMin;
			val2.ScaleMax = scaleMax;
			val2.Color = color;
			val2.ColorRamp = (Texture2D)(object)Ramp(new float[3] { 0f, 0.6f, 1f }, new Color[3]
			{
				new Color(1f, 1f, 1f, 1f),
				new Color(1f, 1f, 1f, 0.8f),
				new Color(1f, 1f, 1f, 0f)
			});
			val2.ParticleFlagDisableZ = true;
			ParticleProcessMaterial processMaterial = val2;
			GpuParticles2D val3 = new GpuParticles2D
			{
				Amount = Math.Max(1, amount),
				Lifetime = life,
				OneShot = true,
				Explosiveness = explosiveness,
				Randomness = 0.5f,
				Texture = SoftDot,
				ProcessMaterial = (Material)(object)processMaterial,
				VisibilityRect = new Rect2(-1600f, -1600f, 3200f, 3200f)
			};
			if (additive)
			{
				((CanvasItem)val3).Material = (Material)(object)Additive;
			}
			parent.AddChild((Node)(object)val3, false, (InternalMode)0);
			((Node2D)val3).GlobalPosition = globalPos;
			val3.Emitting = true;
			FreeAfter((Node)(object)val3, life + 0.7);
			return val3;
		}

		public static Node2D Sigil(float size, Color tint, out ShaderMaterial mat)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected Obj, but got Unknown
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			Node2D val = new Node2D();
			mat = ShaderLib.NewMaterial("sigil");
			if (mat == null)
			{
				return val;
			}
			mat.SetShaderParameter(StringName.op_Implicit("tint"), Variant.op_Implicit(tint));
			ColorRect val2 = Quad(new Vector2(size, size), (Material)(object)mat);
			((Control)val2).Position = new Vector2((0f - size) * 0.5f, (0f - size) * 0.5f);
			((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
			return val;
		}

		public static Line2D Crescent(Vector2 from, Vector2 to, float bulge, float width, Color color, int segments = 22)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected Obj, but got Unknown
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Expected Obj, but got Unknown
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = (from + to) * 0.5f;
			Vector2 val2 = to - from;
			Vector2 val3;
			if (!(val2.LengthSquared() > 0.001f))
			{
				val3 = new Vector2(0f, -1f);
			}
			else
			{
				Vector2 val4 = new Vector2(0f - val2.Y, val2.X);
				val3 = val4.Normalized();
			}
			Vector2 val5 = val3;
			Vector2 b = val + val5 * bulge;
			Curve val6 = new Curve();
			val6.AddPoint(new Vector2(0f, 0f), 0f, 0f, (TangentMode)0, (TangentMode)0);
			val6.AddPoint(new Vector2(0.55f, 1f), 0f, 0f, (TangentMode)0, (TangentMode)0);
			val6.AddPoint(new Vector2(1f, 0f), 0f, 0f, (TangentMode)0, (TangentMode)0);
			Line2D val7 = new Line2D
			{
				Width = width,
				WidthCurve = val6,
				DefaultColor = color,
				JointMode = (LineJointMode)2,
				Material = (Material)(object)Additive,
				Antialiased = true
			};
			for (int i = 0; i <= segments; i++)
			{
				val7.AddPoint(Bezier(from, b, to, (float)i / (float)segments), -1);
			}
			return val7;
		}

		public static ColorRect Veil(Color color)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected Obj, but got Unknown
			color = AccessibleColor(color);
			return new ColorRect
			{
				Color = color,
				Size = new Vector2(9000f, 7000f),
				Position = new Vector2(-3500f, -3000f),
				MouseFilter = (MouseFilterEnum)2
			};
		}

		public static Tween Drive(Node owner, float seconds, Action<float> step, TransitionType trans = (TransitionType)0L, EaseType ease = (EaseType)2L)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			Tween val = owner.CreateTween();
			bool failed = false;
			val.TweenMethod(Callable.From<float>((Action<float>)((float t) =>
			{
				if (failed || !Alive((GodotObject)(object)owner))
				{
					return;
				}
				try
				{
					step(t);
				}
				catch (Exception ex)
				{
					failed = true;
					DevLog.Warn("Effect tween failed: " + ex);
				}
			})), Variant.op_Implicit(0f), Variant.op_Implicit(1f), (double)seconds).SetTrans(trans).SetEase(ease);
			return val;
		}

		public static Polygon2D Shape(Vector2[] points, Color color)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected Obj, but got Unknown
			return new Polygon2D
			{
				Polygon = points,
				Color = color,
				Antialiased = true
			};
		}

		public static Vector2[] Star(int points, float outer, float inner)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			Vector2[] array = new Vector2[points * 2];
			for (int i = 0; i < points * 2; i++)
			{
				float num = ((i % 2 == 0) ? outer : inner);
				float num2 = (float)Math.PI * (float)i / (float)points - (float)Math.PI / 2f;
				array[i] = new Vector2(Mathf.Cos(num2) * num, Mathf.Sin(num2) * num);
			}
			return array;
		}

		public static GpuParticles2D Bubbles(Node parent, Vector2 globalPos, Color color, int amount, Vector2 box, float velMin, float velMax, double life, float scaleMin, float scaleMax, bool oneShot = true, float explosiveness = 0.85f)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected Obj, but got Unknown
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Expected Obj, but got Unknown
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			if (!Alive((GodotObject)(object)parent))
			{
				return null;
			}
			ParticleProcessMaterial val = new ParticleProcessMaterial();
			val.EmissionShape = (EmissionShapeEnum)3;
			val.EmissionBoxExtents = new Vector3(Math.Max(1f, box.X), Math.Max(1f, box.Y), 0f);
			val.Direction = new Vector3(0f, -1f, 0f);
			val.Spread = 22f;
			val.InitialVelocityMin = velMin;
			val.InitialVelocityMax = velMax;
			val.Gravity = new Vector3(0f, -70f, 0f);
			val.ScaleMin = scaleMin;
			val.ScaleMax = scaleMax;
			val.Color = color;
			val.ColorRamp = (Texture2D)(object)Ramp(new float[4] { 0f, 0.12f, 0.85f, 1f }, new Color[4]
			{
				new Color(1f, 1f, 1f, 0f),
				new Color(1f, 1f, 1f, 1f),
				new Color(1f, 1f, 1f, 1f),
				new Color(1f, 1f, 1f, 0f)
			});
			val.ParticleFlagDisableZ = true;
			ParticleProcessMaterial processMaterial = val;
			GpuParticles2D val2 = new GpuParticles2D
			{
				Amount = Math.Max(1, amount),
				Lifetime = life,
				OneShot = oneShot,
				Explosiveness = (oneShot ? explosiveness : 0f),
				Randomness = 0.7f,
				Texture = Bubble,
				ProcessMaterial = (Material)(object)processMaterial,
				Material = (Material)(object)Additive,
				VisibilityRect = new Rect2(-1200f, -1600f, 2400f, 2400f)
			};
			parent.AddChild((Node)(object)val2, false, (InternalMode)0);
			((Node2D)val2).GlobalPosition = globalPos;
			val2.Emitting = true;
			if (oneShot)
			{
				FreeAfter((Node)(object)val2, life + 0.7);
			}
			return val2;
		}

		public static void After(Node parent, double delay, Action action)
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Expected Obj, but got Unknown
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			if (!Alive((GodotObject)(object)parent) || action == null)
			{
				return;
			}
			if (delay <= 0.001 || !parent.IsInsideTree())
			{
				try
				{
					action();
					return;
				}
				catch (Exception ex)
				{
					DevLog.Warn("Immediate effect failed: " + ex);
					return;
				}
			}
			Node timer = new Node
			{
				Name = StringName.op_Implicit("SilentFX_Delay")
			};
			parent.AddChild(timer, false, (InternalMode)0);
			FreeAfter(timer, Math.Max(0.0, delay) + 0.1);
			Tween val = timer.CreateTween();
			val.TweenInterval(delay);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				try
				{
					action();
				}
				catch (Exception ex2)
				{
					DevLog.Warn("Delayed effect failed: " + ex2.Message);
				}
				if (Alive((GodotObject)(object)timer))
				{
					timer.QueueFree();
				}
			})));
		}

		public static void RingPulse(Node parent, Vector2 at, float size, Color tint, float delay, float time, float fromScale, float toScale, float flatten = 1f, float dashes = 0f, float width = 0.03f, float peak = 0.8f)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			if (!Alive((GodotObject)(object)parent))
			{
				return;
			}
			tint = AccessibleColor(tint);
			if (PlayerSettings.ReduceScreenEffects)
			{
				peak *= 0.55f;
			}
			Node2D ring = Sigil(size, tint, out var mat);
			parent.AddChild((Node)(object)ring, false, (InternalMode)0);
			FreeAfter((Node)(object)ring, Mathf.Max(0f, delay) + Mathf.Max(0f, time) + 0.1f);
			ring.GlobalPosition = at;
			ring.Scale = new Vector2(fromScale, fromScale * flatten);
			if (mat != null)
			{
				mat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.82f));
				mat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(width));
				mat.SetShaderParameter(StringName.op_Implicit("dashes"), Variant.op_Implicit(dashes));
				mat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.35f));
				mat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(0f));
			}
			Action<float> action = (float t) =>
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0087: Unknown result type (might be due to invalid IL or missing references)
				if (Alive((GodotObject)(object)ring))
				{
					float num = Mathf.Lerp(fromScale, toScale, t);
					ring.Scale = new Vector2(num, num * flatten);
					if (Alive((GodotObject)(object)mat))
					{
						mat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(peak * (1f - t)));
						mat.SetShaderParameter(StringName.op_Implicit("spin"), Variant.op_Implicit(t * 3f));
					}
				}
			};
			Tween val = ((Node)ring).CreateTween();
			val.TweenMethod(Callable.From<float>(action), Variant.op_Implicit(0f), Variant.op_Implicit(1f), (double)time).SetDelay((double)delay).SetTrans((TransitionType)4)
				.SetEase((EaseType)1);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (Alive((GodotObject)(object)ring))
				{
					((Node)ring).QueueFree();
				}
			})));
		}

		public static void Flash(Node parent, Vector2 at, Color color, float size, float grow = 2.2f, double time = 0.3)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			if (Alive((GodotObject)(object)parent))
			{
				color = AccessibleColor(color, flash: true);
				Sprite2D val = Glow(color, new Vector2(size, size));
				parent.AddChild((Node)(object)val, false, (InternalMode)0);
				((Node2D)val).GlobalPosition = at;
				Tween val2 = ((Node)val).CreateTween();
				val2.SetParallel(true);
				val2.TweenProperty((GodotObject)(object)val, NodePath.op_Implicit("scale"), Variant.op_Implicit(((Node2D)val).Scale * grow), time).SetTrans((TransitionType)4).SetEase((EaseType)1);
				val2.TweenProperty((GodotObject)(object)val, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), time);
				FreeAfter((Node)(object)val, time + 0.1);
			}
		}

		public static void Beam(Node parent, Vector2 from, Vector2 to, float width, Color color, float extend = 0.07f, float hold = 0.1f, float fade = 0.22f)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Expected Obj, but got Unknown
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			if (!Alive((GodotObject)(object)parent))
			{
				return;
			}
			color = AccessibleColor(color);
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Beam")
			};
			parent.AddChild((Node)(object)holder, false, (InternalMode)0);
			FreeAfter((Node)(object)holder, Mathf.Max(0f, extend) + Mathf.Max(0f, hold) + Mathf.Max(0f, fade) + 0.1f);
			Line2D[] layers = new Line2D[3]
			{
				Layer(width * 2.6f, new Color(color.R, color.G, color.B, 0.25f)),
				Layer(width, new Color(color.R, color.G, color.B, 0.85f)),
				Layer(width * 0.36f, new Color(Mathf.Lerp(color.R, 1f, 0.75f), Mathf.Lerp(color.G, 1f, 0.75f), Mathf.Lerp(color.B, 1f, 0.75f), 0.95f))
			};
			Line2D[] array = layers;
			foreach (Line2D val in array)
			{
				((Node)holder).AddChild((Node)(object)val, false, (InternalMode)0);
			}
			Vector2 a = holder.ToLocal(from);
			Vector2 b = holder.ToLocal(to);
			Action<float> action = (float t) =>
			{
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				if (Alive((GodotObject)(object)holder))
				{
					Vector2[] points = new Vector2[2]
					{
						a,
						a.Lerp(b, t)
					};
					Line2D[] array2 = layers;
					foreach (Line2D val3 in array2)
					{
						if (Alive((GodotObject)(object)val3))
						{
							val3.Points = points;
						}
					}
				}
			};
			action(0.02f);
			Tween val2 = ((Node)holder).CreateTween();
			val2.TweenMethod(Callable.From<float>(action), Variant.op_Implicit(0.02f), Variant.op_Implicit(1f), (double)extend).SetTrans((TransitionType)4).SetEase((EaseType)1);
			val2.TweenInterval((double)hold);
			val2.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), (double)fade).SetTrans((TransitionType)4).SetEase((EaseType)0);
			val2.TweenCallback(Callable.From((Action)(() =>
			{
				if (Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
			static Line2D Layer(float w, Color c)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Expected Obj, but got Unknown
				return new Line2D
				{
					Width = w,
					DefaultColor = c,
					BeginCapMode = (LineCapMode)2,
					EndCapMode = (LineCapMode)2,
					Material = (Material)(object)Additive,
					Antialiased = true
				};
			}
		}

		public static float Hash(int i)
		{
			float num = Mathf.Sin((float)i * 127.1f + 311.7f) * 43758.547f;
			return num - Mathf.Floor(num);
		}

		private static Color AccessibleColor(Color color, bool flash = false)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if (!PlayerSettings.ReduceScreenEffects)
			{
				return color;
			}
			float num = (flash ? 0.35f : 0.6f);
			return new Color(color.R, color.G, color.B, color.A * num);
		}
	}
