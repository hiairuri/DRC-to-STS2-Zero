// namespace: SilentFX.Cards
// type: MirageFx (class)
// source lines: 79257-79612
	public sealed class MirageFx : CardFx
	{
		private static readonly StringName PFill = StringName.op_Implicit("fill");

		private static readonly StringName PFlash = StringName.op_Implicit("flash");

		private static readonly StringName PScatter = StringName.op_Implicit("scatter");

		private static readonly StringName PChurn = StringName.op_Implicit("churn");

		private static readonly StringName PIntensity = StringName.op_Implicit("intensity");

		public static readonly Color Haze = new Color(0.55f, 1f, 0.82f, 1f);

		private const float Inner = 0.66f;

		public static int PoisonOn(CardFxContext c)
		{
			int num = 0;
			if (c?.Enemies == null)
			{
				return 0;
			}
			foreach (FxTarget enemy in c.Enemies)
			{
				if (enemy != null && enemy.Poison > 0)
				{
					num += enemy.Poison;
				}
			}
			return num;
		}

		public static float PowerOf(CardFxContext c)
		{
			return Mathf.Clamp((float)PoisonOn(c) / Mathf.Max(1f, Tuning.F("mirage.full_poison", 24f)), 0f, 1f);
		}

		public static void Ellipse(CardFxContext c, float power, out Vector2 centre, out Vector2 size)
		{
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			float num = Tuning.F("mirage.size", 1f) * (1f + 0.18f * Mathf.Clamp(power, 0f, 1f));
			float num2 = Mathf.Clamp(c.Body.X * 1.05f, 170f, 330f) * num;
			float num3 = Mathf.Clamp(c.Body.Y * 1.45f, 260f, 520f) * num;
			float num4 = ((c.Facing < 0f) ? (-1f) : 1f);
			size = new Vector2(num2, num3);
			centre = c.Chest + new Vector2(num4 * (c.Body.X * Tuning.F("mirage.ahead", 0.4f) + num2 * 0.5f), (0f - c.Body.Y) * 0.04f);
		}

		public static Node2D MakeMist(Node parent, Vector2 centre, Vector2 size, out ColorRect quad, out ShaderMaterial mat)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			Node2D val = FogKit.Quad(parent, "SilentFX_MirageMist", out quad, out mat);
			if (val == null)
			{
				return null;
			}
			mat.SetShaderParameter(StringName.op_Implicit("shape"), Variant.op_Implicit(1f));
			mat.SetShaderParameter(StringName.op_Implicit("seed"), Variant.op_Implicit(2f));
			mat.SetShaderParameter(StringName.op_Implicit("thick"), Variant.op_Implicit(1f));
			mat.SetShaderParameter(PFill, Variant.op_Implicit(0f));
			mat.SetShaderParameter(StringName.op_Implicit("col_deep"), Variant.op_Implicit(new Color(0.22f, 0.46f, 0.4f, 1f)));
			mat.SetShaderParameter(StringName.op_Implicit("col_mid"), Variant.op_Implicit(new Color(0.52f, 0.88f, 0.76f, 1f)));
			mat.SetShaderParameter(StringName.op_Implicit("col_glow"), Variant.op_Implicit(new Color(0.93f, 1f, 0.96f, 1f)));
			PlaceMist(val, quad, mat, centre, size);
			return val;
		}

		public static void PlaceMist(Node2D holder, ColorRect quad, ShaderMaterial mat, Vector2 centre, Vector2 size)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = size / 0.66f;
			FogKit.Place(holder, quad, mat, centre - val * 0.5f, val);
		}

		public override void OnPlay(CardFxContext c)
		{
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (c == null)
				{
					return;
				}
				Node frontParent = c.FrontParent;
				if (!FxKit.Alive((GodotObject)(object)frontParent))
				{
					return;
				}
				try
				{
					int num = PoisonOn(c);
					float num2 = PowerOf(c);
					float strength = ((num > 0) ? (0.72f + 0.28f * num2) : 0.4f);
					Ellipse(c, num2, out var centre, out var size);
					float begun = ((c.Held is MirageHeld mirageHeld) ? Mathf.Clamp(mirageHeld.Filled, 0f, 0.6f) : 0f);
					if (Tuning.F("mirage.feed", 1f) >= 0.5f)
					{
						int num3 = 0;
						float time = Tuning.F("mirage.feed_time", 0.2f);
						foreach (FxTarget enemy in c.Enemies)
						{
							if (enemy != null && enemy.Poison > 0 && num3 < 6)
							{
								Wisp(frontParent, enemy.Chest, centre, num3, time);
								num3++;
							}
						}
					}
					Mist(frontParent, centre, size, strength, begun);
				}
				catch (Exception ex)
				{
					DevLog.Warn("Mirage effect failed: " + ex.Message);
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("MirageFx.OnPlay: " + ex2);
			}
		}

		private static void Mist(Node parent, Vector2 centre, Vector2 size, float strength, float begun)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected Obj, but got Unknown
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			Node2D root = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Mirage")
			};
			parent.AddChild((Node)(object)root, false, (InternalMode)0);
			if (!((Node)root).IsInsideTree())
			{
				((Node)root).QueueFree();
				return;
			}
			float fillTime = Mathf.Max(0.05f, Tuning.F("mirage.fill_time", 0.2f));
			float flashAt = fillTime + 0.03f;
			float flashDown = Mathf.Max(0.05f, Tuning.F("mirage.flash_time", 0.26f));
			float goAt = flashAt + 0.05f + 0.05f;
			float goTime = Mathf.Max(0.1f, Tuning.F("mirage.scatter_time", 0.42f));
			float total = goAt + goTime;
			FxKit.FreeAfter((Node)(object)root, total + 0.1f);
			MakeMist((Node)(object)root, centre, size, out var _, out var mat);
			Sprite2D bloom = FxKit.Glow(new Color(Haze.R, Haze.G, Haze.B, 0f), new Vector2(size.X * 1.5f, size.Y * 1.3f));
			((Node)root).AddChild((Node)(object)bloom, false, (InternalMode)0);
			((Node2D)bloom).GlobalPosition = centre;
			Vector2 bloomScale = ((Node2D)bloom).Scale;
			float peak = Tuning.F("mirage.mist_peak", 0.74f) * strength;
			float light = Tuning.F("mirage.flash", 0.55f) * strength;
			FxKit.Drive((Node)(object)root, total, (float u) =>
			{
				//IL_0138: Unknown result type (might be due to invalid IL or missing references)
				//IL_0170: Unknown result type (might be due to invalid IL or missing references)
				//IL_0187: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_0201: Unknown result type (might be due to invalid IL or missing references)
				//IL_0237: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)root))
				{
					float num = u * total;
					float num2 = Mathf.Clamp(num / fillTime, 0f, 1f);
					float num3 = Mathf.Lerp(begun, 1f, 1f - (1f - num2) * (1f - num2));
					float num4 = ((num < flashAt) ? 0f : ((num < flashAt + 0.05f) ? ((num - flashAt) / 0.05f) : Mathf.Max(0f, 1f - (num - flashAt - 0.05f) / flashDown)));
					num4 *= Mathf.Lerp(0.6f, 1f, num4);
					float num5 = Mathf.Clamp((num - goAt) / goTime, 0f, 1f);
					float num6 = 1f - (1f - num5) * (1f - num5);
					float num7 = Mathf.Min(1f, begun + num / 0.08f) * (1f - num5 * num5);
					if (FxKit.Alive((GodotObject)(object)mat))
					{
						mat.SetShaderParameter(PFill, Variant.op_Implicit(num3));
						mat.SetShaderParameter(PFlash, Variant.op_Implicit(Mathf.Clamp(num4 * (0.55f + 0.45f * strength), 0f, 1f)));
						mat.SetShaderParameter(PScatter, Variant.op_Implicit(num6));
						mat.SetShaderParameter(PChurn, Variant.op_Implicit(num * 1.6f));
						mat.SetShaderParameter(PIntensity, Variant.op_Implicit(Mathf.Clamp(peak * num7, 0f, 1f)));
					}
					if (FxKit.Alive((GodotObject)(object)bloom))
					{
						((Node2D)bloom).Scale = bloomScale * (1f + 0.22f * num6);
						((CanvasItem)bloom).Modulate = new Color(Haze.R, Haze.G, Haze.B, light * num4);
					}
				}
			}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)root))
				{
					((Node)root).QueueFree();
				}
			})));
			FxKit.After(parent, goAt, () =>
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					Brush.Motes(parent, centre, new Color(Haze.R, Haze.G, Haze.B, 0.7f * strength), 7, size.X * 0.34f, 110f, 0.6);
				}
			});
		}

		private static void Wisp(Node parent, Vector2 from, Vector2 to, int index, float time)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected Obj, but got Unknown
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_MirageWisp")
			};
			parent.AddChild((Node)(object)holder, false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)holder, Mathf.Max(0f, time) + (float)index * 0.02f + 0.1f);
			if (!((Node)holder).IsInsideTree())
			{
				((Node)holder).QueueFree();
				return;
			}
			Sprite2D[] nodes = new Sprite2D[7];
			Vector2[] offsets = new Vector2[7];
			for (int i = 0; i < 7; i++)
			{
				float num = FxKit.Hash(index * 13 + i * 3 + 2);
				nodes[i] = FxKit.Glow(new Color(Haze.R, Haze.G, Haze.B, 0f), new Vector2(150f + 60f * num, 50f + 22f * num));
				((CanvasItem)nodes[i]).Visible = false;
				((Node)holder).AddChild((Node)(object)nodes[i], false, (InternalMode)0);
				offsets[i] = new Vector2(0f, (FxKit.Hash(index * 7 + i * 5 + 1) - 0.5f) * 44f);
			}
			Vector2 mid = new Vector2((from.X + to.X) * 0.5f, Mathf.Min(from.Y, to.Y) - 70f - 30f * (float)index);
			float peak = Tuning.F("mirage.streak_alpha", 0.3f);
			Action<float> action = (float t) =>
			{
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					for (int j = 0; j < 7; j++)
					{
						Sprite2D val2 = nodes[j];
						if (FxKit.Alive((GodotObject)(object)val2))
						{
							float num2 = t * 1.45f - (float)j * 0.075f;
							if (num2 <= 0f || num2 >= 1f)
							{
								((CanvasItem)val2).Visible = false;
							}
							else
							{
								Vector2 val3 = FxKit.Bezier(from, mid, to, num2);
								Vector2 val4 = FxKit.Bezier(from, mid, to, Mathf.Min(1f, num2 + 0.03f));
								((CanvasItem)val2).Visible = true;
								((Node2D)val2).GlobalPosition = val3 + offsets[j] * (1f - num2);
								Vector2 val5 = val4 - val3;
								if (val5.LengthSquared() > 0.01f)
								{
									val5 = val4 - val3;
									((Node2D)val2).Rotation = val5.Angle();
								}
								((CanvasItem)val2).Modulate = new Color(Haze.R, Haze.G, Haze.B, peak * Mathf.Sin((float)Math.PI * num2));
							}
						}
					}
				}
			};
			Tween val = ((Node)holder).CreateTween();
			val.TweenMethod(Callable.From<float>(action), Variant.op_Implicit(0f), Variant.op_Implicit(1f), (double)time).SetDelay((double)((float)index * 0.02f)).SetTrans((TransitionType)4)
				.SetEase((EaseType)0);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
		}

		static MirageFx()
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		}
	}
