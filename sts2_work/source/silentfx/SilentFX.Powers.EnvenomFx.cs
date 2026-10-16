// namespace: SilentFX.Powers
// type: EnvenomFx (class)
// source lines: 5521-5977
	public sealed class EnvenomFx : BasicPowerFx
	{
		private readonly List<FxTarget> _targetScratch = new List<FxTarget>();

		private static readonly Color Venom = new Color(0.5f, 0.98f, 0.32f, 1f);

		private static readonly Color Pale = new Color(0.84f, 1f, 0.62f, 1f);

		private float _lastDrops = -9f;

		private float _lastFling = -9f;

		private int _flings;

		protected override string NodeName => "Envenom";

		protected override bool UseBack => false;

		protected override void Build(bool animateIn)
		{
		}

		protected override void Layout()
		{
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				float num = 200f;
				Anchor.SafeTargets(_targetScratch);
				List<FxTarget> targetScratch = _targetScratch;
				Vector2 val;
				if (at.HasValue)
				{
					val = at.Value;
					float num2 = float.MaxValue;
					foreach (FxTarget item in targetScratch)
					{
						Vector2 val2 = item.Chest - val;
						float num3 = val2.LengthSquared();
						if (!(num3 >= num2))
						{
							num2 = num3;
							num = item.Width;
						}
					}
				}
				else
				{
					if (targetScratch.Count == 0)
					{
						return;
					}
					val = targetScratch[0].Chest;
					num = targetScratch[0].Width;
				}
				bool flag = Time - _lastDrops >= Tuning.F("envenom.drop_rest", 0.7f);
				if (flag)
				{
					_lastDrops = Time;
				}
				float light = ((Time - _lastFling < Tuning.F("envenom.fling_rest", 0.45f)) ? Mathf.Clamp(Tuning.F("envenom.fling_repeat", 0.6f), 0.2f, 1f) : 1f);
				_lastFling = Time;
				_flings++;
				Node frontParent = Anchor.FrontParent;
				Vector2 way = Way(val, _flings);
				float width = Mathf.Clamp(num, 130f, 360f);
				Fling(frontParent, val, way, num, light);
				Pop(frontParent, val, way, width, light);
				int num4;
				if (MultiplayerFx.Active)
				{
					object owner = Anchor.Owner;
					Creature val3 = (Creature)((owner is Creature) ? owner : null);
					if (val3 != null && val3.IsPlayer)
					{
						num4 = ((!MultiplayerFx.IsLocal(val3)) ? 1 : 0);
						goto IL_01bb;
					}
				}
				num4 = 0;
				goto IL_01bb;
				IL_01bb:
				bool remoteFade = (byte)num4 != 0;
				if (flag)
				{
					Drips(frontParent, val, width, remoteFade);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Envenom trigger failed: " + ex.Message);
			}
		}

		private Vector2 Way(Vector2 hit, int nth)
		{
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			float num = Anchor.Side();
			float num2 = 0f;
			try
			{
				if (Anchor.Chest != null)
				{
					Vector2 val = hit - Anchor.Chest();
					if (Mathf.Abs(val.X) > 30f)
					{
						num = ((val.X < 0f) ? (-1f) : 1f);
						num2 = Mathf.Clamp(val.Y / Mathf.Abs(val.X), -0.3f, 0.2f);
					}
				}
			}
			catch
			{
			}
			float num3 = Tuning.F("envenom.fling_lift", 10f) + (FxKit.Hash(nth * 3 + 1) - 0.5f) * Tuning.F("envenom.fling_vary", 14f);
			Vector2 val2 = new Vector2(num, num2);
			val2 = val2.Normalized();
			return val2.Rotated((0f - num) * Mathf.DegToRad(num3));
		}

		private static void Fling(Node parent, Vector2 hit, Vector2 way, float width, float light)
		{
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Expected Obj, but got Unknown
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0408: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Expected Obj, but got Unknown
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Expected Obj, but got Unknown
			//IL_0520: Unknown result type (might be due to invalid IL or missing references)
			//IL_052f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0564: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent) || !parent.IsInsideTree())
			{
				return;
			}
			float num = Tuning.F("envenom.splash_scale", 1f);
			float num2 = Mathf.Clamp(width * 0.5f + 190f, 260f, 520f) * Tuning.F("envenom.fling_reach", 1f) * num;
			float num3 = Mathf.Min(width * 0.16f, 70f);
			float num4 = Mathf.Clamp(width / 260f, 1f, 1.45f) * num;
			float num5 = Mathf.Clamp(Tuning.F("envenom.fling_fan", 11f), 0f, 60f);
			Vector2 val = new Vector2(0f - way.Y, way.X);
			Brush.Cut(parent, hit - way * num3, hit + way * (num2 * 0.5f), 9f * num4, new Color(Venom.R, Venom.G, Venom.B, 0.62f * light), 0f, 0.05f, 0.02f, 0.18f);
			int num6 = (int)((float)Tuning.I("envenom.fling_mist", 16) * light + 0.5f);
			if (num6 > 0)
			{
				FxKit.Burst(parent, hit, new Color(Pale.R, Pale.G, Pale.B, 0.85f), num6, num2 * 0.9f, num2 * 2.4f, 0.42, 0.07f * num4, 0.16f * num4, new Vector2(0f, 520f), num5 * 1.2f, way, 9f, num2 * 1.5f);
			}
			int count = Mathf.Clamp((int)((float)Tuning.I("envenom.fling_drops", 10) * light + 0.5f), 0, 24);
			if (count <= 0)
			{
				return;
			}
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_EnvenomFling")
			};
			parent.AddChild((Node)(object)holder, false, (InternalMode)0);
			if (!((Node)holder).IsInsideTree())
			{
				((Node)holder).QueueFree();
				return;
			}
			holder.GlobalPosition = hit;
			float drag = Mathf.Max(0.5f, Tuning.F("envenom.fling_drag", 3.2f));
			float gravity = Tuning.F("envenom.fling_gravity", 620f);
			float num7 = Mathf.DegToRad(num5);
			float num8 = Mathf.Max(0.3f, Tuning.F("envenom.fling_life", 1f));
			float num9 = 0.5f * num8;
			float num10 = num2 * drag / (1f - Mathf.Exp((0f - drag) * num9));
			Node2D[] drops = new Node2D[count];
			Vector2[] start = new Vector2[count];
			Vector2[] speed = new Vector2[count];
			float[] born = new float[count];
			float[] life = new float[count];
			float[] fat = new float[count];
			float num11 = 0.1f;
			for (int i = 0; i < count; i++)
			{
				float num12 = ((float)i + 0.5f) / (float)count;
				float num13 = GD.Randf();
				float num14 = (GD.Randf() - 0.5f) * 2f * num7 * (1.25f - 0.75f * num12);
				speed[i] = way.Rotated(num14) * (num10 * Mathf.Lerp(0.3f, 1f, num12) * Mathf.Lerp(0.9f, 1.08f, GD.Randf()));
				start[i] = hit - way * (num3 * (1f - num12) * GD.Randf()) + val * ((GD.Randf() - 0.5f) * 20f * num4);
				born[i] = 0.05f * (1f - num12) * GD.Randf();
				life[i] = Mathf.Lerp(0.34f, 0.5f, 0.35f * num13 + 0.65f * num12) * num8;
				fat[i] = Mathf.Lerp(12f, 21f, num13) * num4;
				num11 = Mathf.Max(num11, born[i] + life[i]);
				Node2D val2 = new Node2D
				{
					Visible = false
				};
				((Node)holder).AddChild((Node)(object)val2, false, (InternalMode)0);
				((Node)val2).AddChild((Node)new Sprite2D
				{
					Texture = FxKit.SoftDot,
					Modulate = new Color(0.07f, 0.36f, 0.08f, 0.8f),
					Scale = new Vector2(1.15f, 1.35f)
				}, false, (InternalMode)0);
				((Node)val2).AddChild((Node)(object)FxKit.Glow(new Color(Venom.R, Venom.G, Venom.B, 0.95f), new Vector2(64f, 64f)), false, (InternalMode)0);
				Sprite2D val3 = FxKit.Glow(new Color(Pale.R, Pale.G, Pale.B, 0.95f), new Vector2(30f, 46f));
				((Node2D)val3).Position = new Vector2(13f, 0f);
				((Node)val2).AddChild((Node)(object)val3, false, (InternalMode)0);
				drops[i] = val2;
			}
			float total = num11;
			FxKit.Drive((Node)(object)holder, total, (float u) =>
			{
				//IL_007f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0156: Unknown result type (might be due to invalid IL or missing references)
				//IL_0160: Unknown result type (might be due to invalid IL or missing references)
				//IL_019a: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					float num15 = u * total;
					for (int j = 0; j < count; j++)
					{
						Node2D val4 = drops[j];
						if (FxKit.Alive((GodotObject)(object)val4))
						{
							float num16 = num15 - born[j];
							if (num16 < 0f || num16 >= life[j])
							{
								((CanvasItem)val4).Visible = false;
							}
							else
							{
								float num17 = num16 / life[j];
								float num18 = Mathf.Exp((0f - drag) * num16);
								Vector2 val5 = speed[j] * num18 + new Vector2(0f, gravity * num16);
								((CanvasItem)val4).Visible = true;
								val4.GlobalPosition = start[j] + speed[j] * ((1f - num18) / drag) + new Vector2(0f, 0.5f * gravity * num16 * num16);
								val4.GlobalRotation = val5.Angle();
								float num19 = fat[j] * (1f - 0.55f * num17 * num17);
								float num20 = num19 * (1f + Mathf.Clamp(val5.Length() / 300f, 0f, 4.5f));
								val4.Scale = new Vector2(num20, num19) / 64f;
								((CanvasItem)val4).Modulate = new Color(1f, 1f, 1f, Mathf.Min(1f, num16 / 0.03f) * (1f - num17 * num17 * num17));
							}
						}
					}
				}
			}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
		}

		private static void Pop(Node parent, Vector2 hit, Vector2 way, float width, float light)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			float num = Tuning.F("envenom.pop", 1f) * Tuning.F("envenom.splash_scale", 1f);
			if (!(num <= 0.01f))
			{
				FxKit.Flash(parent, hit, new Color(0.86f, 1f, 0.74f, 0.55f * light), width * 0.22f * num, 2f, 0.18);
				if (light >= 0.99f)
				{
					FxKit.RingPulse(parent, hit, width * 0.8f * num, Venom, 0f, 0.28f, 0.25f, 1f, 1f, 0f, 0.034f, 0.5f);
				}
				FxKit.Burst(parent, hit, new Color(Venom.R, Venom.G, Venom.B, 0.9f), Math.Max(1, (int)((float)Tuning.I("envenom.pop_amount", 9) * light)), 60f, 200f, 0.45, 0.1f * num, 0.24f * num, new Vector2(0f, 480f), 70f, way, width * 0.05f);
			}
		}

		private static void Drips(Node parent, Vector2 where, float width, bool remoteFade)
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			float num = Tuning.F("envenom.splash_scale", 1f);
			int num2 = Mathf.Clamp(Tuning.I("envenom.drops", 3), 0, 8);
			float num3 = Mathf.Clamp(width / 220f, 1f, 1.8f) * num;
			float num4 = Mathf.Max(0f, Tuning.F("envenom.drop_first", 0.3f));
			float num5 = Mathf.Max(0.05f, Tuning.F("envenom.drop_gap", 0.36f));
			for (int i = 0; i < num2; i++)
			{
				Vector2 from = where + new Vector2((GD.Randf() - 0.5f) * width * 0.5f, (GD.Randf() - 0.35f) * width * 0.36f);
				float size = Mathf.Lerp(0.8f, 1.15f, GD.Randf()) * num3;
				FxKit.After(parent, num4, () =>
				{
					//IL_0031: Unknown result type (might be due to invalid IL or missing references)
					using (RemoteFxFade.Capture(remoteFade, parent))
					{
						Drop(parent, from, size);
					}
				});
				num4 += num5 * Mathf.Lerp(0.7f, 1.4f, GD.Randf());
			}
		}

		private static void Drop(Node parent, Vector2 from, float size)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected Obj, but got Unknown
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Expected Obj, but got Unknown
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent) || !parent.IsInsideTree())
			{
				return;
			}
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_EnvenomDrop")
			};
			parent.AddChild((Node)(object)holder, false, (InternalMode)0);
			holder.GlobalPosition = from;
			holder.Scale = Vector2.Zero;
			((Node)holder).AddChild((Node)new Sprite2D
			{
				Texture = FxKit.SoftDot,
				Modulate = new Color(0.07f, 0.36f, 0.08f, 0.85f),
				Scale = new Vector2(22f, 22f) / 64f
			}, false, (InternalMode)0);
			((Node)holder).AddChild((Node)(object)FxKit.Glow(new Color(Venom.R, Venom.G, Venom.B, 0.95f), new Vector2(15f, 15f)), false, (InternalMode)0);
			float seconds = Mathf.Max(0.12f, Tuning.F("envenom.drop_life", 0.34f));
			float fall = Tuning.F("envenom.drop_fall", 70f) * size;
			FxKit.Drive((Node)(object)holder, seconds, (float t) =>
			{
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					float num = Mathf.Clamp(t / 0.25f, 0f, 1f);
					float num2 = Mathf.Clamp((t - 0.25f) / 0.75f, 0f, 1f);
					holder.GlobalPosition = from + new Vector2(0f, fall * num2 * num2);
					holder.Scale = new Vector2(size * num * (1f - 0.3f * num2), size * num * (1f + 0.9f * num2));
					((CanvasItem)holder).Modulate = new Color(1f, 1f, 1f, 1f - num2 * num2 * num2);
				}
			}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
		}

		static EnvenomFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		}
	}
