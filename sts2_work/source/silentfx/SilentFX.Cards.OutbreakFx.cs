// namespace: SilentFX.Cards
// type: OutbreakFx (class)
// source lines: 81350-81891
	public sealed class OutbreakFx : CardFx
	{
		private sealed class Geyser
		{
			public Vector2 Feet;

			public float Width;

			public float High;

			public float Seed;

			public Line2D Soft;

			public Line2D Core;

			public Sprite2D Pulse;

			public Sprite2D Outer;

			public Sprite2D Inner;

			public Sprite2D Pool;

			public GpuParticles2D Jet;

			public GpuParticles2D Haze;
		}

		private const float Rise = 0.1f;

		private const float Stand = 0.6f;

		private const float Sink = 0.75f;

		private const float Hang = 0.55f;

		private const float Pull = 2400f;

		public override void OnPlay(CardFxContext c)
		{
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Expected Obj, but got Unknown
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Expected Obj, but got Unknown
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node back = c.BackParent;
				Node front = c.FrontParent;
				if (!FxKit.Alive((GodotObject)(object)back) || !FxKit.Alive((GodotObject)(object)front))
				{
					return;
				}
				float start = ((c.Held is OutbreakHeld outbreakHeld) ? Mathf.Clamp(outbreakHeld.Grow, 0f, 1f) : 0f);
				float lead = Mathf.Clamp(Sfx.Lead("outbreak_erupt"), 0.14f, 0.3f);
				if (!c.Remote)
				{
					Sfx.Hush("outbreak_creep");
				}
				ColorRect veil = FxKit.Veil(new Color(0.1f, 0.34f, 0.06f, 0f));
				back.AddChild((Node)(object)veil, false, (InternalMode)0);
				FxKit.FreeAfter((Node)(object)veil, 1.600000023841858);
				Tween val = ((Node)veil).CreateTween();
				val.TweenProperty((GodotObject)(object)veil, NodePath.op_Implicit("color:a"), Variant.op_Implicit(Tuning.F("outbreak.veil", 0.44f)), (double)lead);
				val.TweenInterval(0.6000000238418579);
				val.TweenProperty((GodotObject)(object)veil, NodePath.op_Implicit("color:a"), Variant.op_Implicit(0f), 0.6);
				val.TweenCallback(Callable.From((Action)(() =>
				{
					if (FxKit.Alive((GodotObject)(object)veil))
					{
						((Node)veil).QueueFree();
					}
				})));
				FxKit.Flash(back, c.Feet, new Color(0.86f, 1f, 0.7f, 0.85f), c.Body.X * 1.5f, 1.8f, 0.22);
				if (c.Enemies.Count == 0)
				{
					return;
				}
				Sfx.PlayAt("outbreak_erupt", lead);
				Node2D floor = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_OutbreakFloor")
				};
				back.AddChild((Node)(object)floor, false, (InternalMode)0);
				Node2D val2 = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_Outbreak")
				};
				front.AddChild((Node)(object)val2, false, (InternalMode)0);
				if (!((Node)floor).IsInsideTree() || !((Node)val2).IsInsideTree())
				{
					((Node)floor).QueueFree();
					((Node)val2).QueueFree();
					return;
				}
				float total = lead + 0.6f + 0.75f + 0.55f;
				FxKit.FreeAfter((Node)(object)floor, total + 0.3f);
				FxKit.FreeAfter((Node)(object)val2, total + 0.3f);
				List<Geyser> geysers = new List<Geyser>();
				int num = 0;
				foreach (FxTarget enemy in c.Enemies)
				{
					if (geysers.Count < 6)
					{
						geysers.Add(Make(floor, val2, enemy.Feet, enemy.Width, 1.7f * (float)num++));
						continue;
					}
					break;
				}
				Vector2 from = c.Feet;
				Vector2[] points = new Vector2[21];
				bool up = false;
				bool spent = false;
				FxKit.Drive((Node)(object)val2, total, (float u) =>
				{
					//IL_0189: Unknown result type (might be due to invalid IL or missing references)
					float num2 = u * total;
					float num3 = Mathf.Clamp(num2 / lead, 0f, 1f);
					float grow = Mathf.Lerp(start, 1f, 1f - (1f - num3) * (1f - num3));
					float veins = ((num2 < lead) ? 1f : (1f - Mathf.Clamp((num2 - lead) / 0.3f, 0f, 1f)));
					float num4 = num2 - lead;
					if (!up && num4 >= 0f)
					{
						up = true;
						foreach (Geyser item in geysers)
						{
							Burst(back, front, item);
						}
						GameRefs.Shake(3);
					}
					if (!spent && num4 >= 0.6f)
					{
						spent = true;
						foreach (Geyser item2 in geysers)
						{
							if (FxKit.Alive((GodotObject)(object)item2.Jet))
							{
								item2.Jet.Emitting = false;
							}
							if (FxKit.Alive((GodotObject)(object)item2.Haze))
							{
								item2.Haze.Emitting = false;
							}
						}
					}
					foreach (Geyser item3 in geysers)
					{
						Pose(item3, floor, points, from, num2, lead, grow, veins, num3, num4);
					}
				}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
				{
					foreach (Geyser item4 in geysers)
					{
						if (FxKit.Alive((GodotObject)(object)item4.Outer))
						{
							((CanvasItem)item4.Outer).Visible = false;
						}
						if (FxKit.Alive((GodotObject)(object)item4.Inner))
						{
							((CanvasItem)item4.Inner).Visible = false;
						}
						if (FxKit.Alive((GodotObject)(object)item4.Pool))
						{
							((CanvasItem)item4.Pool).Visible = false;
						}
					}
				})));
			}
			catch (Exception ex)
			{
				DevLog.Warn("OutbreakFx.OnPlay: " + ex);
			}
		}

		private static Geyser Make(Node2D floor, Node2D holder, Vector2 feet, float enemyWidth, float seed)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Expected Obj, but got Unknown
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Expected Obj, but got Unknown
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_041e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0428: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_045b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Expected Obj, but got Unknown
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0507: Unknown result type (might be due to invalid IL or missing references)
			//IL_0512: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_0550: Expected Obj, but got Unknown
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_0572: Unknown result type (might be due to invalid IL or missing references)
			//IL_0577: Unknown result type (might be due to invalid IL or missing references)
			Color venom = PoisonFx.Venom;
			Color acid = PoisonFx.Acid;
			float num = Mathf.Clamp(enemyWidth, 130f, 380f);
			float num2 = Mathf.Clamp(num * Tuning.F("outbreak.column", 3f), 320f, 900f);
			Geyser geyser = new Geyser
			{
				Feet = feet,
				Width = num,
				High = num2,
				Seed = seed
			};
			geyser.Pool = FxKit.Glow(new Color(venom.R, venom.G, venom.B, 0f), new Vector2(64f, 64f));
			((Node)floor).AddChild((Node)(object)geyser.Pool, false, (InternalMode)0);
			((Node2D)geyser.Pool).GlobalPosition = feet;
			geyser.Soft = OutbreakLook.Line((Node)(object)floor, 26f, 0.22f);
			geyser.Core = OutbreakLook.Line((Node)(object)floor, 7f, 1f);
			geyser.Pulse = FxKit.Glow(new Color(0.92f, 1f, 0.8f, 0f), new Vector2(64f, 64f));
			((Node)floor).AddChild((Node)(object)geyser.Pulse, false, (InternalMode)0);
			geyser.Outer = FxKit.Glow(new Color(venom.R, venom.G, venom.B, 0f), new Vector2(64f, 64f));
			((Node)holder).AddChild((Node)(object)geyser.Outer, false, (InternalMode)0);
			geyser.Inner = FxKit.Glow(new Color(0.9f, 1f, 0.78f, 0f), new Vector2(64f, 64f));
			((Node)holder).AddChild((Node)(object)geyser.Inner, false, (InternalMode)0);
			float num3 = Mathf.Sqrt(4800f * num2);
			ParticleProcessMaterial val = new ParticleProcessMaterial();
			val.EmissionShape = (EmissionShapeEnum)3;
			val.EmissionBoxExtents = new Vector3(Mathf.Max(8f, num * 0.14f), 4f, 0f);
			val.Direction = new Vector3(0f, -1f, 0f);
			val.Spread = 6f;
			val.InitialVelocityMin = num3 * 0.72f;
			val.InitialVelocityMax = num3 * 1.04f;
			val.Gravity = new Vector3(0f, 2400f, 0f);
			val.ScaleMin = 0.1f;
			val.ScaleMax = 0.3f;
			val.Color = new Color(acid.R, acid.G, acid.B, 0.95f);
			val.ColorRamp = (Texture2D)(object)FxKit.Ramp(new float[3] { 0f, 0.75f, 1f }, new Color[3]
			{
				new Color(1f, 1f, 1f, 1f),
				new Color(1f, 1f, 1f, 0.9f),
				new Color(1f, 1f, 1f, 0f)
			});
			val.ParticleFlagDisableZ = true;
			ParticleProcessMaterial processMaterial = val;
			geyser.Jet = new GpuParticles2D
			{
				Amount = Mathf.Clamp(Tuning.I("outbreak.drops", 110), 10, 300),
				Lifetime = Mathf.Clamp(2f * num3 / 2400f, 0.6f, 2.2f),
				Randomness = 0.6f,
				Texture = FxKit.SoftDot,
				ProcessMaterial = (Material)(object)processMaterial,
				Material = (Material)(object)FxKit.Additive,
				VisibilityRect = new Rect2(-1600f, -2000f, 3200f, 2800f),
				Emitting = false
			};
			((Node)holder).AddChild((Node)(object)geyser.Jet, false, (InternalMode)0);
			((Node2D)geyser.Jet).GlobalPosition = feet + new Vector2(0f, -8f);
			ParticleProcessMaterial processMaterial2 = new ParticleProcessMaterial
			{
				EmissionShape = (EmissionShapeEnum)3,
				EmissionBoxExtents = new Vector3(Mathf.Max(8f, num * 0.2f), 6f, 0f),
				Direction = new Vector3(0f, -1f, 0f),
				Spread = 15f,
				InitialVelocityMin = num3 * 0.3f,
				InitialVelocityMax = num3 * 0.62f,
				Gravity = new Vector3(0f, 1080f, 0f),
				ScaleMin = 1.2f,
				ScaleMax = 2.8f,
				Color = new Color(venom.R, venom.G, venom.B, Tuning.F("outbreak.mist", 0.11f)),
				ColorRamp = (Texture2D)(object)FxKit.FadeInOut(0.2f),
				ParticleFlagDisableZ = true
			};
			geyser.Haze = new GpuParticles2D
			{
				Amount = 26,
				Lifetime = 1.1,
				Randomness = 0.6f,
				Texture = FxKit.SoftDot,
				ProcessMaterial = (Material)(object)processMaterial2,
				Material = (Material)(object)FxKit.Additive,
				VisibilityRect = new Rect2(-1600f, -2000f, 3200f, 2800f),
				Emitting = false
			};
			((Node)holder).AddChild((Node)(object)geyser.Haze, false, (InternalMode)0);
			((Node2D)geyser.Haze).GlobalPosition = feet + new Vector2(0f, -10f);
			return geyser;
		}

		private static void Burst(Node back, Node front, Geyser g)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			Color venom = PoisonFx.Venom;
			Color acid = PoisonFx.Acid;
			if (FxKit.Alive((GodotObject)(object)g.Jet))
			{
				g.Jet.Emitting = true;
			}
			if (FxKit.Alive((GodotObject)(object)g.Haze))
			{
				g.Haze.Emitting = true;
			}
			if (FxKit.Alive((GodotObject)(object)front))
			{
				FxKit.Flash(front, g.Feet + new Vector2(0f, (0f - g.Width) * 0.18f), new Color(0.9f, 1f, 0.78f, 0.95f), g.Width * 1.4f, 2f, 0.22);
				FxKit.Burst(front, g.Feet + new Vector2(0f, -10f), new Color(acid.R, acid.G, acid.B, 0.95f), 20, 260f, 620f, 0.5, 0.08f, 0.2f, new Vector2(0f, 500f), 180f, null, g.Width * 0.2f, 500f);
			}
			if (FxKit.Alive((GodotObject)(object)back))
			{
				Brush.Wave(back, g.Feet, g.Width * 3f, new Color(venom.R, venom.G, venom.B, 0.9f), 0.5f, 0f, 0.3f);
			}
		}

		private static void Pose(Geyser g, Node2D floor, Vector2[] points, Vector2 from, float t, float lead, float grow, float veins, float run, float since)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_0469: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_049a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0530: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_0553: Unknown result type (might be due to invalid IL or missing references)
			Color venom = PoisonFx.Venom;
			if (FxKit.Alive((GodotObject)(object)g.Soft))
			{
				((CanvasItem)g.Soft).Visible = veins > 0.01f;
				((CanvasItem)g.Soft).Modulate = new Color(1f, 1f, 1f, veins);
				if (((CanvasItem)g.Soft).Visible)
				{
					OutbreakLook.Lay(g.Soft, floor, points, from, g.Feet, grow, g.Seed, t);
				}
			}
			if (FxKit.Alive((GodotObject)(object)g.Core))
			{
				((CanvasItem)g.Core).Visible = veins > 0.01f;
				((CanvasItem)g.Core).Modulate = new Color(1f, 1f, 1f, veins);
				if (((CanvasItem)g.Core).Visible)
				{
					OutbreakLook.Lay(g.Core, floor, points, from, g.Feet, grow, g.Seed, t);
				}
			}
			if (FxKit.Alive((GodotObject)(object)g.Pulse))
			{
				bool flag = t < lead;
				((CanvasItem)g.Pulse).Visible = flag;
				if (flag)
				{
					Vector2 val = OutbreakLook.Point(from, g.Feet, run * run, g.Seed, t);
					Vector2 val2 = OutbreakLook.Point(from, g.Feet, Mathf.Min(1f, run * run + 0.02f), g.Seed, t);
					((Node2D)g.Pulse).GlobalPosition = val;
					Sprite2D pulse = g.Pulse;
					Vector2 val3 = val2 - val;
					float globalRotation;
					if (!(val3.LengthSquared() > 0.01f))
					{
						globalRotation = 0f;
					}
					else
					{
						val3 = val2 - val;
						globalRotation = val3.Angle();
					}
					((Node2D)pulse).GlobalRotation = globalRotation;
					((Node2D)g.Pulse).Scale = new Vector2(150f, 40f) / 64f;
					((CanvasItem)g.Pulse).Modulate = new Color(0.92f, 1f, 0.8f, 0.95f * Mathf.Sin((float)Math.PI * Mathf.Clamp(run, 0.05f, 1f)) + 0.3f * run);
				}
			}
			if (since < 0f)
			{
				if (FxKit.Alive((GodotObject)(object)g.Pool))
				{
					float num = run * run;
					((Node2D)g.Pool).Scale = new Vector2(g.Width * (0.9f + 0.7f * num), g.Width * 0.26f) / 64f;
					((CanvasItem)g.Pool).Modulate = new Color(venom.R, venom.G, venom.B, 0.55f * num);
				}
				return;
			}
			float num2 = Mathf.Clamp(since / 0.1f, 0f, 1f);
			num2 = 1f - (1f - num2) * (1f - num2);
			float num3 = Mathf.Clamp((since - 0.6f) / 0.75f, 0f, 1f);
			float num4 = 1f + 0.05f * Mathf.Sin(since * 31f + g.Seed) + 0.03f * Mathf.Sin(since * 53f + g.Seed * 2f);
			float num5 = g.High * num2 * num4 * (1f - num3 * num3);
			float num6 = 1f - Mathf.SmoothStep(0f, 1f, num3);
			float num7 = 1f + 0.08f * Mathf.Sin(since * 23f + g.Seed * 3f);
			if (FxKit.Alive((GodotObject)(object)g.Outer))
			{
				((Node2D)g.Outer).GlobalPosition = g.Feet + new Vector2(0f, (0f - num5) * 0.42f);
				((Node2D)g.Outer).Scale = new Vector2(g.Width * 0.95f * num7, Mathf.Max(4f, num5)) / 64f;
				((CanvasItem)g.Outer).Modulate = new Color(venom.R, venom.G, venom.B, 0.58f * num6);
			}
			if (FxKit.Alive((GodotObject)(object)g.Inner))
			{
				((Node2D)g.Inner).GlobalPosition = g.Feet + new Vector2(0f, (0f - num5) * 0.4f);
				((Node2D)g.Inner).Scale = new Vector2(g.Width * 0.36f * num7, Mathf.Max(4f, num5 * 0.92f)) / 64f;
				((CanvasItem)g.Inner).Modulate = new Color(0.9f, 1f, 0.78f, 0.9f * num6);
			}
			if (FxKit.Alive((GodotObject)(object)g.Pool))
			{
				float num8 = Mathf.Clamp((since - 0.6f - 0.375f) / 0.925f, 0f, 1f);
				((Node2D)g.Pool).Scale = new Vector2(g.Width * (1.6f + 0.9f * Mathf.Min(1f, since / 0.25f)), g.Width * 0.34f) / 64f;
				((CanvasItem)g.Pool).Modulate = new Color(venom.R, venom.G, venom.B, 0.55f * (1f - num8) * (1f - num8));
			}
		}
	}
