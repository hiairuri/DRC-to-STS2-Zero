// namespace: SilentFX.Core
// type: Brush (class)
// source lines: 32530-33120
	public static class Brush
	{
		private sealed class Spindle
		{
			public Node2D Root;

			public Sprite2D Outer;

			public Sprite2D Core;

			public Color Color;
		}

		public static readonly Color Wind = new Color(0.8f, 0.97f, 0.95f, 0.55f);

		public static readonly Color Steel = new Color(0.78f, 0.88f, 1f, 0.9f);

		public static readonly Color Gold = new Color(1f, 0.82f, 0.42f, 0.9f);

		public static readonly Color Amber = new Color(1f, 0.62f, 0.22f, 0.9f);

		public static readonly Color Blood = new Color(1f, 0.24f, 0.22f, 0.9f);

		public static readonly Color Violet = new Color(0.72f, 0.5f, 1f, 0.85f);

		public static readonly Color Earth = new Color(0.8f, 0.76f, 0.68f, 0.5f);

		private static Color Pale(Color c, float alpha)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			return new Color(Mathf.Lerp(c.R, 1f, 0.78f), Mathf.Lerp(c.G, 1f, 0.78f), Mathf.Lerp(c.B, 1f, 0.78f), alpha);
		}

		private static Spindle Make(Node parent, Color color)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected Obj, but got Unknown
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return null;
			}
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Brush")
			};
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return null;
			}
			Sprite2D val2 = FxKit.Glow(new Color(color.R, color.G, color.B, 0f), new Vector2(64f, 64f));
			Sprite2D val3 = FxKit.Glow(Pale(color, 0f), new Vector2(64f, 64f));
			((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
			return new Spindle
			{
				Root = val,
				Outer = val2,
				Core = val3,
				Color = color
			};
		}

		private static void Shape(Spindle s, Vector2 a, Vector2 b, float thick, float alpha)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			if (s != null && FxKit.Alive((GodotObject)(object)s.Root) && FxKit.Alive((GodotObject)(object)s.Outer) && FxKit.Alive((GodotObject)(object)s.Core))
			{
				Vector2 val = b - a;
				float num = Mathf.Max(6f, val.Length());
				s.Root.GlobalPosition = (a + b) * 0.5f;
				s.Root.GlobalRotation = ((val.LengthSquared() > 0.01f) ? val.Angle() : 0f);
				((Node2D)s.Outer).Scale = new Vector2(num * 1.3f, Mathf.Max(1f, thick)) / 64f;
				((Node2D)s.Core).Scale = new Vector2(num * 1.12f, Mathf.Max(0.5f, thick * 0.36f)) / 64f;
				float num2 = Mathf.Clamp(alpha, 0f, 1f);
				((CanvasItem)s.Outer).Modulate = new Color(s.Color.R, s.Color.G, s.Color.B, s.Color.A * num2);
				((CanvasItem)s.Core).Modulate = Pale(s.Color, Mathf.Min(1f, s.Color.A * 1.15f) * num2);
			}
		}

		private static void End(Tween tween, Spindle s, Action then = null)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			tween.TweenCallback(Callable.From((Action)(() =>
			{
				try
				{
					then?.Invoke();
				}
				catch (Exception ex)
				{
					DevLog.Warn("Brush callback failed: " + ex.Message);
				}
				if (FxKit.Alive((GodotObject)(object)s.Root))
				{
					((Node)s.Root).QueueFree();
				}
			})));
		}

		public static void Cut(Node parent, Vector2 from, Vector2 to, float thick, Color color, float delay = 0f, float inTime = 0.05f, float hold = 0.04f, float fade = 0.22f)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			if (delay > 0.001f)
			{
				FxKit.After(parent, delay, () =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0019: Unknown result type (might be due to invalid IL or missing references)
					Cut(parent, from, to, thick, color, 0f, inTime, hold, fade);
				});
				return;
			}
			Spindle s = Make(parent, color);
			if (s != null)
			{
				float total = inTime + hold + fade;
				FxKit.FreeAfter((Node)(object)s.Root, Mathf.Max(0f, total) + 0.1f);
				Shape(s, from, from.Lerp(to, 0.15f), thick, 0f);
				End(FxKit.Drive((Node)(object)s.Root, total, (float u) =>
				{
					//IL_0060: Unknown result type (might be due to invalid IL or missing references)
					//IL_0086: Unknown result type (might be due to invalid IL or missing references)
					//IL_008b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0093: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
					float num = u * total;
					float num2 = Mathf.Clamp(num / Mathf.Max(0.001f, inTime), 0f, 1f);
					float num3 = Mathf.Clamp((num - inTime - hold) / Mathf.Max(0.001f, fade), 0f, 1f);
					Vector2 b = from.Lerp(to, 0.15f + 0.85f * (1f - (1f - num2) * (1f - num2)));
					Vector2 a = from.Lerp(to, 0.55f * num3 * num3);
					Shape(s, a, b, thick * (1f - 0.7f * num3), 1f - num3);
				}, (TransitionType)0, (EaseType)2), s);
			}
		}

		public static void Dart(Node parent, Vector2 from, Vector2 to, float length, float thick, Color color, float time, float delay = 0f, Action onArrive = null)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			if (delay > 0.001f)
			{
				FxKit.After(parent, delay, () =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					Dart(parent, from, to, length, thick, color, time, 0f, onArrive);
				});
				return;
			}
			Spindle s = Make(parent, color);
			if (s == null)
			{
				onArrive?.Invoke();
				return;
			}
			float distance = from.DistanceTo(to);
			Vector2 dir = (Vector2)((distance > 0.5f) ? ((to - from) / distance) : new Vector2(1f, 0f));
			Shape(s, from, from + dir * 8f, thick, 0f);
			FxKit.FreeAfter((Node)(object)s.Root, Mathf.Max(0.02f, time) + 0.1f);
			End(FxKit.Drive((Node)(object)s.Root, Mathf.Max(0.02f, time), (float u) =>
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				float num = u * (0.55f + 0.45f * u);
				Vector2 val = from + dir * (distance * num);
				float num2 = Mathf.Clamp(distance * num, 8f, Mathf.Max(8f, length));
				Shape(s, val - dir * num2, val, thick, Mathf.Min(1f, u * 7f));
			}, (TransitionType)0, (EaseType)2), s, onArrive);
		}

		public static void Follow(Node parent, Func<float, Vector2> path, float span, float thick, Color color, float time, float delay = 0f, Action onArrive = null)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent) || path == null)
			{
				return;
			}
			if (delay > 0.001f)
			{
				FxKit.After(parent, delay, () =>
				{
					//IL_0019: Unknown result type (might be due to invalid IL or missing references)
					Follow(parent, path, span, thick, color, time, 0f, onArrive);
				});
				return;
			}
			Spindle s = Make(parent, color);
			if (s == null)
			{
				onArrive?.Invoke();
				return;
			}
			Shape(s, path(0f), path(0.02f), thick, 0f);
			FxKit.FreeAfter((Node)(object)s.Root, Mathf.Max(0.02f, time) + 0.1f);
			End(FxKit.Drive((Node)(object)s.Root, Mathf.Max(0.02f, time), (float u) =>
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				float num = u * (0.55f + 0.45f * u);
				float arg = Mathf.Max(0f, num - span);
				Shape(s, path(arg), path(Mathf.Max(num, 0.02f)), thick, Mathf.Min(1f, u * 7f) * Mathf.Min(1f, (1f - u) * 5f + 0.35f));
			}, (TransitionType)0, (EaseType)2), s, onArrive);
		}

		public static void Gust(Node parent, Vector2 from, Vector2 to, int count, float spread, float thick, Color color, float time, float delay = 0f)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Vector2 val = to - from;
			float num = val.Length();
			if (!(num < 1f))
			{
				Vector2 val2 = new Vector2(0f - val.Y, val.X) / num;
				for (int i = 0; i < count; i++)
				{
					float num2 = FxKit.Hash(i * 7 + 3);
					float num3 = FxKit.Hash(i * 13 + 5);
					float num4 = ((count <= 1) ? 0f : (Mathf.Lerp(0f - spread, spread, (float)i / (float)(count - 1)) + (num2 - 0.5f) * spread * 0.3f));
					Vector2 val3 = from + val2 * num4 + val * (0.1f * num3);
					Vector2 to2 = to + val2 * num4 * 0.7f - val * (0.06f * num2);
					Dart(parent, val3, to2, num * (0.3f + 0.25f * num2), thick * (0.7f + 0.6f * num3), color, time * (0.85f + 0.3f * num2), delay + 0.03f * (float)i * num3);
				}
			}
		}

		public static void Arc(Node parent, Vector2 a, Vector2 ctrl, Vector2 b, float thick, Color color, float delay = 0f, float inTime = 0.06f, float hold = 0.03f, float fade = 0.24f)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected Obj, but got Unknown
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			if (delay > 0.001f)
			{
				FxKit.After(parent, delay, () =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0013: Unknown result type (might be due to invalid IL or missing references)
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					Arc(parent, a, ctrl, b, thick, color, 0f, inTime, hold, fade);
				});
				return;
			}
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_BrushArc")
			};
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			if (!((Node)val).IsInsideTree())
			{
				((Node)val).QueueFree();
				return;
			}
			float num = a.DistanceTo(ctrl) + ctrl.DistanceTo(b);
			float step = num / 14f;
			Sprite2D[] outer = new Sprite2D[14];
			Sprite2D[] core = new Sprite2D[14];
			float[] fat = new float[14];
			for (int num2 = 0; num2 < 14; num2++)
			{
				float num3 = ((float)num2 + 0.5f) / 14f;
				Vector2 globalPosition = FxKit.Bezier(a, ctrl, b, num3);
				Vector2 val2 = FxKit.Bezier(a, ctrl, b, Mathf.Min(1f, num3 + 0.03f)) - FxKit.Bezier(a, ctrl, b, Mathf.Max(0f, num3 - 0.03f));
				float globalRotation = ((val2.LengthSquared() > 0.001f) ? val2.Angle() : 0f);
				fat[num2] = Mathf.Pow(Mathf.Sin((float)Math.PI * num3), 0.8f);
				outer[num2] = FxKit.Glow(new Color(color.R, color.G, color.B, 0f), new Vector2(64f, 64f));
				core[num2] = FxKit.Glow(Pale(color, 0f), new Vector2(64f, 64f));
				((Node)val).AddChild((Node)(object)outer[num2], false, (InternalMode)0);
				((Node)val).AddChild((Node)(object)core[num2], false, (InternalMode)0);
				((Node2D)outer[num2]).GlobalPosition = globalPosition;
				((Node2D)core[num2]).GlobalPosition = globalPosition;
				((Node2D)outer[num2]).GlobalRotation = globalRotation;
				((Node2D)core[num2]).GlobalRotation = globalRotation;
			}
			float total = inTime + hold + fade;
			FxKit.Drive((Node)(object)val, total, (float u) =>
			{
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0127: Unknown result type (might be due to invalid IL or missing references)
				//IL_0157: Unknown result type (might be due to invalid IL or missing references)
				//IL_0161: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
				float num4 = u * total;
				float num5 = Mathf.Clamp(num4 / Mathf.Max(0.001f, inTime), 0f, 1f);
				float num6 = Mathf.Clamp((num4 - inTime - hold) / Mathf.Max(0.001f, fade), 0f, 1f);
				for (int i = 0; i < 14; i++)
				{
					if (FxKit.Alive((GodotObject)(object)outer[i]) && FxKit.Alive((GodotObject)(object)core[i]))
					{
						float num7 = ((float)i + 0.5f) / 14f;
						float num8 = Mathf.Clamp((num5 - num7) * 14f * 0.6f + 0.5f, 0f, 1f);
						float num9 = Mathf.Clamp(1f - (num6 * 1.35f - num7 * 0.35f), 0f, 1f);
						float num10 = thick * fat[i] * (0.35f + 0.65f * num9);
						((Node2D)outer[i]).Scale = new Vector2(step * 2.6f, Mathf.Max(1f, num10)) / 64f;
						((Node2D)core[i]).Scale = new Vector2(step * 2.4f, Mathf.Max(0.5f, num10 * 0.34f)) / 64f;
						float num11 = num8 * num9 * (0.25f + 0.75f * fat[i]);
						((CanvasItem)outer[i]).Modulate = new Color(color.R, color.G, color.B, color.A * 0.55f * num11);
						((CanvasItem)core[i]).Modulate = Pale(color, Mathf.Min(1f, color.A) * 0.75f * num11);
					}
				}
			}, (TransitionType)0, (EaseType)2);
			FxKit.FreeAfter((Node)(object)val, (double)total + 0.1);
		}

		public static void Orb(Node parent, Vector2 from, Vector2 ctrl, Vector2 to, float size, Color color, float time, float delay = 0f, Action onArrive = null)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			if (delay > 0.001f)
			{
				FxKit.After(parent, delay, () =>
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0013: Unknown result type (might be due to invalid IL or missing references)
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					Orb(parent, from, ctrl, to, size, color, time, 0f, onArrive);
				});
				return;
			}
			Spindle s = Make(parent, color);
			if (s == null)
			{
				onArrive?.Invoke();
				return;
			}
			Vector2 last = from;
			FxKit.FreeAfter((Node)(object)s.Root, Mathf.Max(0.02f, time) + 0.1f);
			End(FxKit.Drive((Node)(object)s.Root, Mathf.Max(0.02f, time), (float u) =>
			{
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_008e: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)s.Root))
				{
					float t = u * u * (3f - 2f * u);
					Vector2 val = FxKit.Bezier(from, ctrl, to, t);
					Vector2 val2 = val - last;
					last = val;
					float num = Mathf.Clamp(val2.Length() * 1.6f, 0f, size * 2.2f);
					Vector2 val3 = (Vector2)((val2.LengthSquared() > 0.01f) ? val2.Normalized() : new Vector2(1f, 0f));
					float num2 = 0.6f + 0.4f * Mathf.Min(1f, u * 5f);
					Shape(s, val - val3 * (size * 0.5f + num), val + val3 * size * 0.5f, size * num2, Mathf.Min(1f, u * 6f));
				}
			}, (TransitionType)0, (EaseType)2), s, () =>
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				FxKit.Flash(parent, to, new Color(color.R, color.G, color.B, 0.75f), size * 1.6f, 2f, 0.22);
				onArrive?.Invoke();
			});
		}

		public static void Wave(Node parent, Vector2 at, float size, Color color, float time = 0.45f, float delay = 0f, float flatten = 1f, float from = 0.15f)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			FxKit.RingPulse(parent, at, size, color, delay, time, from, 1f, flatten, 0f, 0.085f, color.A * 0.7f);
		}

		public static void Dust(Node parent, Vector2 at, float dirX, int amount = 10, float scale = 1f)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			FxKit.Burst(parent, at, Earth, amount, 40f * scale, 190f * scale, 0.55, 0.45f * scale, 1f * scale, new Vector2(0f, -30f), (dirX == 0f) ? 80f : 38f, new Vector2((dirX == 0f) ? 0f : dirX, -0.35f), 16f * scale, 120f, additive: false, 0.9f);
		}

		public static void Sparks(Node parent, Vector2 at, Color color, int amount, float speed, Vector2? dir = null, float spread = 180f)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			FxKit.Burst(parent, at, new Color(color.R, color.G, color.B, Mathf.Min(1f, color.A + 0.1f)), amount, speed * 0.35f, speed, 0.34, 0.05f, 0.13f, new Vector2(0f, 320f), spread, dir, 6f);
		}

		public static void Motes(Node parent, Vector2 at, Color color, int amount, float radius, float rise = 130f, double life = 0.7)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			FxKit.Burst(parent, at, color, amount, rise * 0.4f, rise, life, 0.06f, 0.16f, new Vector2(0f, -40f), 26f, new Vector2(0f, -1f), radius, 30f, additive: true, 0.55f);
		}

		static Brush()
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
		}
	}
