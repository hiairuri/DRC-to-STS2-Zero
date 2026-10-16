// namespace: SilentFX.Cards
// type: Intros (class)
// source lines: 69353-70313
	public static class Intros
	{
		private static readonly Color Steel = new Color(0.94f, 0.97f, 1f, 1f);

		private static readonly Color SteelDark = new Color(0.6f, 0.67f, 0.78f, 1f);

		private static readonly Color Grip = new Color(0.34f, 0.27f, 0.22f, 1f);

		private static readonly Color NoGlow = new Color(0f, 0f, 0f, 0f);

		private const float ItemSize = 1.35f;

		public static void Accelerant(CardFxContext c, Node2D s)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Expected Obj, but got Unknown
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			Color venom = PoisonFx.Venom;
			Node2D vial = Props.Vial(20f, new Color(0.44f, 0.22f, 0.64f, 1f), new Color(0.64f, 0.38f, 0.88f, 1f), new Color(0.9f, 0.78f, 0.34f, 1f));
			vial.Position = new Vector2(0f, 46f);
			((Node)s).AddChild((Node)(object)vial, false, (InternalMode)0);
			Intro.Pop(vial, 0f, 0.18f);
			Node2D dropper = new Node2D
			{
				Position = new Vector2(52f, -150f),
				Rotation = 0.42f
			};
			((Node)dropper).AddChild((Node)(object)FxKit.Shape(Props.Circle(9f, 14, 1f, 1.25f), new Color(0.2f, 0.2f, 0.24f, 1f)), false, (InternalMode)0);
			((Node)dropper).AddChild((Node)(object)FxKit.Shape(Props.Box(-3.6f, 8f, 3.6f, 46f), new Color(0.82f, 0.9f, 0.96f, 0.85f)), false, (InternalMode)0);
			((Node)dropper).AddChild((Node)(object)FxKit.Shape(Props.Box(-2.4f, 18f, 2.4f, 44f), new Color(0.98f, 0.52f, 0.14f, 1f)), false, (InternalMode)0);
			((Node)dropper).AddChild((Node)(object)FxKit.Shape(new Vector2[3]
			{
				new Vector2(-3.6f, 46f),
				new Vector2(3.6f, 46f),
				new Vector2(0f, 56f)
			}, new Color(0.82f, 0.9f, 0.96f, 0.85f)), false, (InternalMode)0);
			((Node)s).AddChild((Node)(object)dropper, false, (InternalMode)0);
			Vector2 away = dropper.Position;
			Vector2 rest = new Vector2(24f, -98f);
			Intro.Run((Node)(object)dropper, 0.02f, 0.2f, (float v) =>
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				dropper.Position = away.Lerp(rest, v);
			}, (TransitionType)10, (EaseType)1);
			Vector2 val = rest;
			Vector2 val2 = new Vector2(0f, 62f);
			Vector2 tip = val + val2.Rotated(0.42f);
			Vector2 mouth = vial.Position + new Vector2(0f, -31f);
			Polygon2D drop = FxKit.Shape(Props.Teardrop(5.5f), new Color(1f, 0.82f, 0.22f, 1f));
			((Node2D)drop).Position = tip;
			((Node2D)drop).Scale = Vector2.Zero;
			((Node)s).AddChild((Node)(object)drop, false, (InternalMode)0);
			Intro.Run((Node)(object)drop, 0.22f, 0.2f, (float v) =>
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				((Node2D)drop).Scale = new Vector2(v, v);
			}, (TransitionType)0, (EaseType)2);
			Intro.Run((Node)(object)drop, 0.44f, 0.17f, (float v) =>
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				((Node2D)drop).Position = tip.Lerp(mouth, v * v);
				((Node2D)drop).Scale = new Vector2(1f - 0.25f * v, 1f + 0.45f * v);
			}, (TransitionType)0, (EaseType)2);
			Intro.At((Node)(object)s, 0.61f, () =>
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_011e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0124: Unknown result type (might be due to invalid IL or missing references)
				//IL_0138: Unknown result type (might be due to invalid IL or missing references)
				//IL_013d: Unknown result type (might be due to invalid IL or missing references)
				//IL_016e: Unknown result type (might be due to invalid IL or missing references)
				((CanvasItem)drop).Visible = false;
				Intro.Flash(s, mouth, new Color(venom.R, venom.G, venom.B, 0.85f), 46f, 2.2f, 0.26);
				Intro.Sparks(s, mouth, new Color(1f, 0.86f, 0.3f, 0.95f), 6, 60f, 170f, 0.3, new Vector2(0f, -1f), 55f);
				Intro.Run((Node)(object)vial, 0f, 0.3f, (float v) =>
				{
					//IL_0035: Unknown result type (might be due to invalid IL or missing references)
					float num3 = Mathf.Sin(v * (float)Math.PI) * (1f - v);
					vial.Scale = new Vector2(1f + 0.24f * num3, 1f - 0.22f * num3);
				}, (TransitionType)0, (EaseType)2);
				for (int num = 0; num < 5; num++)
				{
					float num2 = ((num % 2 == 0) ? (-1f) : 1f) * (6f + 5f * (float)num);
					Intro.Puff(s, mouth, mouth + new Vector2(num2, -58f - 14f * (float)num), new Color(venom.R * 0.9f, venom.G, venom.B, 0.62f), 0.04f + 0.07f * (float)num, 0.55f, 14f, 52f + 8f * (float)num);
				}
				Intro.Run((Node)(object)dropper, 0.05f, 0.25f, (float v) =>
				{
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					dropper.Position = rest.Lerp(away, v * v);
				}, (TransitionType)0, (EaseType)2);
			});
		}

		public static void InfiniteBlades(CardFxContext c, Node2D given)
		{
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			//IL_0537: Unknown result type (might be due to invalid IL or missing references)
			//IL_0590: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Max(20f, Tuning.F("blades.intro_across", 74f));
			float num2 = num * Mathf.Clamp(Tuning.F("blades.intro_tall", 0.46f), 0.1f, 1f);
			int count = Mathf.Clamp(Tuning.I("blades.intro_count", 11), 4, 18);
			float lapTime = Mathf.Max(0.2f, Tuning.F("blades.intro_lap", 0.58f));
			float rush = Mathf.Max(0f, Tuning.F("blades.intro_rush", 1.6f));
			Vector2[] array = new Vector2[385];
			float[] array2 = new float[385];
			for (int i = 0; i < array.Length; i++)
			{
				float num3 = (float)Math.PI * 2f * (float)i / (float)(array.Length - 1);
				array[i] = new Vector2(Mathf.Sin(num3) * num, (0f - Mathf.Sin(2f * num3)) * num2);
				array2[i] = ((i == 0) ? 0f : (array2[i - 1] + array[i - 1].DistanceTo(array[i])));
			}
			float num4 = Mathf.Max(1f, array2[^1]);
			Vector2[] sign = new Vector2[97];
			int j = 0;
			for (int k = 0; k <= 96; k++)
			{
				float num5;
				for (num5 = num4 * (float)k / 96f; j < array.Length - 2 && array2[j + 1] < num5; j++)
				{
				}
				float num6 = Mathf.Max(1E-05f, array2[j + 1] - array2[j]);
				sign[k] = array[j].Lerp(array[j + 1], Mathf.Clamp((num5 - array2[j]) / num6, 0f, 1f));
			}
			float num7 = Mathf.Max(0.1f, Intro.Unit(c));
			Vector2 home = new Vector2(0f - (c.Body.X * (0.3f + Tuning.F("blades.x", 0.48f)) / num7 + Tuning.F("intro.reach", 92f)), c.Body.Y * (0.1f - Tuning.F("blades.y", 0.16f)) / num7);
			Vector2 val = new Vector2(0f - num, -2f * num2);
			Vector2 heading = val.Normalized();
			float num8 = Mathf.Max(60f, home.Length());
			Vector2 turn = heading * (num8 * 0.55f) + new Vector2(0f, (0f - num8) * 0.12f);
			float num9 = (turn.Length() + turn.DistanceTo(home) + num8) * 0.5f;
			float pour = Mathf.Clamp(num9 / num4 * 0.6f, 0.15f, 0.6f);
			float rest = (float)(count - 1) / (float)count + pour;
			float steady = 1.5f * lapTime;
			float drain = rest * lapTime / (1f + 0.5f * rush);
			float total = steady + drain;
			Node2D s = Intro.Stage(c, IntroSpot.InFront, total + 0.2f);
			if (s == null)
			{
				return;
			}
			float num10 = 0.5f * rush / (drain * lapTime);
			float seconds = steady + ((num10 > 1E-05f) ? ((-1f / lapTime + Mathf.Sqrt(1f / (lapTime * lapTime) + 4f * num10 * pour)) / (2f * num10)) : (pour * lapTime));
			CardModel card = c.Card;
			object owner;
			if (card == null)
			{
				owner = null;
			}
			else
			{
				Player owner2 = card.Owner;
				owner = ((owner2 != null) ? owner2.Creature : null);
			}
			InfiniteBladesFx.StreamArrives(seconds, (Creature)owner);
			Color light = new Color(0.75f, 0.92f, 1f, 1f);
			Sprite2D halo = FxKit.Glow(new Color(light.R, light.G, light.B, 0f), new Vector2(num * 3f, num2 * 4.2f));
			((Node)s).AddChild((Node)(object)halo, false, (InternalMode)0);
			Line2D line = Props.Line(2.4f, new Color(light.R, light.G, light.B, 0f), true);
			((Node)s).AddChild((Node)(object)line, false, (InternalMode)0);
			Sprite2D source = FxKit.Glow(new Color(light.R, light.G, light.B, 0f), new Vector2(40f, 40f));
			((Node)s).AddChild((Node)(object)source, false, (InternalMode)0);
			float num11 = num4 / (float)count;
			float length = num11 / 1.42f * 0.95f;
			Color glow = new Color(0.55f, 0.85f, 1f, 0.25f);
			Node2D[] knives = new Node2D[count];
			for (int l = 0; l < count; l++)
			{
				Node2D val2 = FxKit.Blade(length, length * 0.15f, Steel, SteelDark, Grip, glow);
				((CanvasItem)val2).Visible = false;
				((Node)s).AddChild((Node)(object)val2, false, (InternalMode)0);
				knives[l] = val2;
			}
			bool closed = false;
			bool whole = false;
			bool arrived = false;
			Intro.Run((Node)(object)s, 0f, total, (float v) =>
			{
				//IL_0129: Unknown result type (might be due to invalid IL or missing references)
				//IL_017b: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
				//IL_020a: Unknown result type (might be due to invalid IL or missing references)
				//IL_024c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0265: Unknown result type (might be due to invalid IL or missing references)
				//IL_033c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0342: Unknown result type (might be due to invalid IL or missing references)
				//IL_0348: Unknown result type (might be due to invalid IL or missing references)
				//IL_034f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0354: Unknown result type (might be due to invalid IL or missing references)
				//IL_0356: Unknown result type (might be due to invalid IL or missing references)
				//IL_035c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0362: Unknown result type (might be due to invalid IL or missing references)
				//IL_0379: Unknown result type (might be due to invalid IL or missing references)
				//IL_037e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0384: Unknown result type (might be due to invalid IL or missing references)
				//IL_038a: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_0307: Unknown result type (might be due to invalid IL or missing references)
				//IL_030c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0311: Unknown result type (might be due to invalid IL or missing references)
				//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0400: Unknown result type (might be due to invalid IL or missing references)
				//IL_0411: Unknown result type (might be due to invalid IL or missing references)
				//IL_0416: Unknown result type (might be due to invalid IL or missing references)
				//IL_043a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0457: Unknown result type (might be due to invalid IL or missing references)
				//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
				float num12 = v * total;
				float num13;
				if (num12 <= steady)
				{
					num13 = num12 / lapTime;
				}
				else
				{
					float num14 = num12 - steady;
					num13 = 1.5f + num14 / lapTime * (1f + 0.5f * rush * num14 / drain);
				}
				float num15 = 1f - Mathf.Clamp((num13 - 1.5f) / rest, 0f, 1f);
				float num16 = ((num13 >= 1f) ? Mathf.Exp((0f - (num13 - 1f)) * 5f) : 0f);
				if (!whole)
				{
					whole = num13 >= 1f;
					line.Points = (whole ? sign : Intro.Partial(sign, num13));
				}
				line.Width = 2.4f + 2.6f * num16;
				line.DefaultColor = new Color(light.R, light.G, light.B, (0.38f + 0.55f * num16) * num15);
				((CanvasItem)halo).Modulate = new Color(light.R, light.G, light.B, (0.07f + 0.3f * num16) * num15 * Mathf.Min(1f, num13 * 3f));
				((CanvasItem)source).Modulate = new Color(light.R, light.G, light.B, ((num13 < 1f) ? 0.6f : (0.22f + 0.4f * num16)) * num15);
				if (!closed && num13 >= 1f)
				{
					closed = true;
					Intro.Flash(s, Vector2.Zero, new Color(0.8f, 0.94f, 1f, 0.75f), 84f, 2.2f, 0.24);
				}
				if (!arrived && num13 >= 1.5f + pour)
				{
					arrived = true;
					Intro.Flash(s, home, new Color(0.8f, 0.94f, 1f, 0.6f), 56f, 1.9f, 0.22);
				}
				for (int m = 0; m < count; m++)
				{
					Node2D val3 = knives[m];
					if (FxKit.Alive((GodotObject)(object)val3))
					{
						float num17 = num13 - (float)m / (float)count;
						if (num17 <= 0f || num17 >= 1.5f + pour)
						{
							((CanvasItem)val3).Visible = false;
						}
						else
						{
							float num18 = 1f;
							Vector2 val4;
							Vector2 val5;
							float num19;
							if (num17 < 1.5f)
							{
								val4 = OnSign(num17);
								val5 = OnSign(num17 + 0.012f) - OnSign(num17 - 0.012f);
								num19 = Mathf.SmoothStep(0f, 0.07f, num17);
							}
							else
							{
								float num20 = (num17 - 1.5f) / pour;
								val4 = FxKit.Bezier(Vector2.Zero, turn, home, num20);
								val5 = FxKit.Bezier(Vector2.Zero, turn, home, Mathf.Min(1f, num20 + 0.04f)) - FxKit.Bezier(Vector2.Zero, turn, home, Mathf.Max(0f, num20 - 0.04f));
								num19 = 1f - 0.55f * num20;
								num18 = 1f - num20 * num20 * num20;
							}
							if (val5.LengthSquared() < 1E-06f)
							{
								val5 = heading;
							}
							val5 = val5.Normalized();
							((CanvasItem)val3).Visible = num19 > 0.02f;
							val3.Position = val4 - val5 * (length * 0.29f * num19);
							val3.Rotation = val5.Angle() + (float)Math.PI / 2f;
							val3.Scale = new Vector2(num19, num19);
							((CanvasItem)val3).Modulate = new Color(1f, 1f, 1f, num18);
						}
					}
				}
			}, (TransitionType)0, (EaseType)2);
			Vector2 OnSign(float laps)
			{
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				float num12 = (laps - Mathf.Floor(laps)) * 96f;
				int num13 = Mathf.Clamp((int)num12, 0, 95);
				return sign[num13].Lerp(sign[num13 + 1], num12 - (float)num13);
			}
		}

		public static void PhantomBlades(CardFxContext c, Node2D s)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			Color spirit = new Color(0.35f, 0.75f, 1f, 1f);
			Vector2 from = new Vector2(-74f, 8f);
			Vector2 to = new Vector2(34f, -4f);
			Line2D fire = FxKit.Ribbon(24f, new Color(spirit.R, spirit.G, spirit.B, 0.85f));
			((Node)s).AddChild((Node)(object)fire, false, (InternalMode)0);
			Node2D echo = FxKit.Blade(60f, 8f, new Color(0.55f, 0.8f, 1f, 0.45f), new Color(0.3f, 0.55f, 0.95f, 0.4f), NoGlow, new Color(spirit.R, spirit.G, spirit.B, 0.35f));
			echo.Rotation = 1.4907963f;
			echo.Position = from;
			((Node)s).AddChild((Node)(object)echo, false, (InternalMode)0);
			Node2D blade = FxKit.Blade(60f, 8f, Steel, SteelDark, Grip, new Color(spirit.R, spirit.G, spirit.B, 0.3f));
			blade.Rotation = 1.4907963f;
			blade.Position = from;
			((Node)s).AddChild((Node)(object)blade, false, (InternalMode)0);
			for (int i = 0; i < 3; i++)
			{
				Polygon2D tongue = FxKit.Shape(Props.Teardrop(7f - 1.5f * (float)i, 2.8f), new Color(spirit.R, spirit.G, spirit.B, 0.8f));
				((CanvasItem)tongue).Material = (Material)(object)FxKit.Additive;
				((Node2D)tongue).Rotation = (float)Math.PI;
				((Node2D)tongue).Position = new Vector2((float)(i - 1) * 5f, 16f + 3f * (float)i);
				((Node)blade).AddChild((Node)(object)tongue, false, (InternalMode)0);
				float phase = (float)i * 2.1f;
				Intro.Run((Node)(object)tongue, 0f, 0.62f, (float v) =>
				{
					//IL_0044: Unknown result type (might be due to invalid IL or missing references)
					((Node2D)tongue).Scale = new Vector2(1f + 0.2f * Mathf.Sin(v * 50f + phase), 1f + 0.5f * Mathf.Sin(v * 37f + phase));
				}, (TransitionType)0, (EaseType)2);
			}
			Intro.Run((Node)(object)blade, 0.03f, 0.13f, (float v) =>
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				blade.Position = from.Lerp(to, v);
				fire.AddPoint(blade.Position + new Vector2(-10f, 0f), -1);
			}, (TransitionType)7, (EaseType)1);
			Intro.Run((Node)(object)echo, 0.1f, 0.22f, (float v) =>
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				echo.Position = from.Lerp(to + new Vector2(-16f, 2f), v);
				echo.Scale = new Vector2(1f + 0.5f * (1f - v), 1f + 0.25f * (1f - v));
			}, (TransitionType)7, (EaseType)1);
			Intro.At((Node)(object)s, 0.16f, () =>
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				Vector2 local = to + new Vector2(60f, -5f);
				Intro.Flash(s, local, new Color(spirit.R, spirit.G, spirit.B, 0.85f), 40f, 2.2f, 0.22);
				Intro.Sparks(s, local, new Color(0.6f, 0.9f, 1f, 0.95f), 6, 60f, 190f, 0.28, new Vector2(1f, 0f), 50f, 160f);
				Intro.Run((Node)(object)blade, 0f, 0.3f, (float v) =>
				{
					blade.Rotation = 1.4907963f + 0.1f * Mathf.Sin(v * 30f) * (1f - v);
				}, (TransitionType)0, (EaseType)2);
			});
			Intro.Run((Node)(object)fire, 0.22f, 0.4f, (float v) =>
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				((CanvasItem)fire).Modulate = new Color(1f, 1f, 1f, 1f - v);
			}, (TransitionType)0, (EaseType)2);
			float num = c.Body.X / Mathf.Max(0.1f, Mathf.Abs(s.Scale.X));
			float num2 = c.Body.Y / Mathf.Max(0.1f, s.Scale.Y);
			Vector2 parked = to + new Vector2(-16f, 2f);
			Vector2 shoulder = new Vector2(0f - (num * 0.55f + Tuning.F("intro.reach", 92f)), (0f - num2) * 0.2f);
			Intro.Run((Node)(object)echo, 0.36f, 0.3f, (float v) =>
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				echo.Position = parked.Lerp(shoulder, v * v);
				echo.Rotation = 1.4907963f - v * ((float)Math.PI / 2f);
				((CanvasItem)echo).Modulate = new Color(1f, 1f, 1f, 1f - 0.8f * v);
			}, (TransitionType)0, (EaseType)2);
		}

		public static void Envenom(CardFxContext c, Node2D s)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Expected Obj, but got Unknown
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			Color venom = PoisonFx.Venom;
			Node2D blade = FxKit.Blade(74f, 9f, Steel, SteelDark, new Color(0.62f, 0.66f, 0.2f, 1f), NoGlow);
			blade.Rotation = (float)Math.PI / 2f;
			blade.Position = new Vector2(-34f, 26f);
			((Node)s).AddChild((Node)(object)blade, false, (InternalMode)0);
			Intro.Pop(blade, 0f);
			Sprite2D sheen = FxKit.Glow(new Color(venom.R, venom.G, venom.B, 0f), new Vector2(70f, 36f));
			((Node)s).AddChild((Node)(object)sheen, false, (InternalMode)0);
			Node2D drop = new Node2D
			{
				Position = new Vector2(-4f, -66f)
			};
			((Node)drop).AddChild((Node)(object)FxKit.Glow(new Color(venom.R, venom.G, venom.B, 0.5f), new Vector2(46f, 46f)), false, (InternalMode)0);
			((Node)drop).AddChild((Node)(object)FxKit.Shape(Props.Teardrop(8f), new Color(0.66f, 0.98f, 0.24f, 1f)), false, (InternalMode)0);
			((Node)s).AddChild((Node)(object)drop, false, (InternalMode)0);
			Intro.Pop(drop, 0.1f, 0.2f);
			Vector2 hung = drop.Position;
			Vector2 hit = new Vector2(-4f, 20f);
			Intro.Run((Node)(object)drop, 0.34f, 0.16f, (float v) =>
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				drop.Position = hung.Lerp(hit, v * v);
				drop.Scale = new Vector2(1f - 0.2f * v, 1f + 0.35f * v);
			}, (TransitionType)0, (EaseType)2);
			Intro.At((Node)(object)s, 0.5f, () =>
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Unknown result type (might be due to invalid IL or missing references)
				//IL_019e: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
				((CanvasItem)drop).Visible = false;
				Intro.Flash(s, hit, new Color(venom.R, venom.G, venom.B, 0.9f), 50f, 2f, 0.22);
				Intro.Sparks(s, hit, new Color(venom.R, venom.G, venom.B, 0.95f), 7, 70f, 190f, 0.3, new Vector2(0f, -1f), 70f);
				Intro.Run((Node)(object)blade, 0f, 0.3f, (float v) =>
				{
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					Node2D val = blade;
					Color val2 = new Color(1f, 1f, 1f, 1f);
					((CanvasItem)val).Modulate = val2.Lerp(new Color(0.72f, 1f, 0.55f, 1f), v);
				}, (TransitionType)0, (EaseType)2);
				Intro.Run((Node)(object)sheen, 0f, 0.26f, (float v) =>
				{
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_005b: Unknown result type (might be due to invalid IL or missing references)
					((Node2D)sheen).Position = new Vector2(-30f + 74f * v, 24f);
					((CanvasItem)sheen).Modulate = new Color(venom.R, venom.G, venom.B, 0.85f * Mathf.Sin(v * (float)Math.PI));
				}, (TransitionType)0, (EaseType)2);
				for (int num = 0; num < 3; num++)
				{
					Polygon2D drip = FxKit.Shape(Props.Teardrop(3.2f), new Color(0.66f, 0.98f, 0.24f, 1f));
					float x = 2f + 17f * (float)num;
					((Node2D)drip).Position = new Vector2(x, 32f);
					((Node2D)drip).Scale = Vector2.Zero;
					((Node)s).AddChild((Node)(object)drip, false, (InternalMode)0);
					Intro.Run((Node)(object)drip, 0.16f + 0.08f * (float)num, 0.34f, (float v) =>
					{
						//IL_001a: Unknown result type (might be due to invalid IL or missing references)
						//IL_003f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0067: Unknown result type (might be due to invalid IL or missing references)
						float num2 = Mathf.Min(1f, v * 4f);
						((Node2D)drip).Scale = new Vector2(num2, num2);
						((Node2D)drip).Position = new Vector2(x, 32f + 46f * v * v);
						((CanvasItem)drip).Modulate = new Color(1f, 1f, 1f, 1f - v * v);
					}, (TransitionType)0, (EaseType)2);
				}
			});
		}

		public static void Afterimage(CardFxContext c, Node2D s)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			float w = c.Body.X / Mathf.Max(0.1f, Mathf.Abs(s.Scale.X));
			float num = c.Body.Y / Mathf.Max(0.1f, s.Scale.Y);
			Color[] array = new Color[3]
			{
				new Color(1f, 0.92f, 0.35f, 1f),
				new Color(0.55f, 1f, 0.45f, 1f),
				new Color(0.35f, 0.95f, 0.85f, 1f)
			};
			for (int i = 0; i < 3; i++)
			{
				Color tint = array[i];
				Sprite2D ghost = FxKit.Glow(new Color(tint.R, tint.G, tint.B, 0f), new Vector2(w * 0.9f, num * 1.05f));
				((Node)s).AddChild((Node)(object)ghost, false, (InternalMode)0);
				float far = 0f - (36f + 32f * (float)i);
				Intro.Run((Node)(object)ghost, 0.03f * (float)i, 0.42f, (float v) =>
				{
					//IL_004e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0091: Unknown result type (might be due to invalid IL or missing references)
					float num3 = ((v < 0.3f) ? (v / 0.3f) : (1f - (v - 0.3f) / 0.7f));
					num3 = 1f - (1f - num3) * (1f - num3);
					((Node2D)ghost).Position = new Vector2(far * num3, 0f);
					((CanvasItem)ghost).Modulate = new Color(tint.R, tint.G, tint.B, 0.42f * Mathf.Sin(v * (float)Math.PI));
				}, (TransitionType)0, (EaseType)2);
			}
			for (int num2 = 0; num2 < 6; num2++)
			{
				Color val = array[num2 % 3];
				float y = (0f - num) * 0.45f + num * 0.9f * (float)num2 / 5f;
				Line2D streak = Props.Line(3.5f, new Color(val.R, val.G, val.B, 0.9f), true);
				((Node)s).AddChild((Node)(object)streak, false, (InternalMode)0);
				Intro.Run((Node)(object)streak, 0.025f * (float)num2, 0.24f, (float v) =>
				{
					//IL_0045: Unknown result type (might be due to invalid IL or missing references)
					//IL_004a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0064: Unknown result type (might be due to invalid IL or missing references)
					//IL_0069: Unknown result type (might be due to invalid IL or missing references)
					//IL_0094: Unknown result type (might be due to invalid IL or missing references)
					float num3 = Mathf.Lerp(w * 0.9f, (0f - w) * 1.1f, v);
					streak.Points = new Vector2[2]
					{
						new Vector2(num3, y + 8f),
						new Vector2(num3 + 80f, y - 8f)
					};
					((CanvasItem)streak).Modulate = new Color(1f, 1f, 1f, Mathf.Sin(v * (float)Math.PI));
				}, (TransitionType)0, (EaseType)2);
			}
		}

		public static void MasterPlanner(CardFxContext c, Node2D s)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Expected Obj, but got Unknown
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Expected Obj, but got Unknown
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Expected Obj, but got Unknown
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Expected Obj, but got Unknown
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Expected Obj, but got Unknown
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Expected Obj, but got Unknown
			//IL_0447: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0602: Unknown result type (might be due to invalid IL or missing references)
			//IL_0609: Unknown result type (might be due to invalid IL or missing references)
			//IL_062b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0637: Expected Obj, but got Unknown
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0686: Unknown result type (might be due to invalid IL or missing references)
			//IL_068b: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_06db: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0725: Unknown result type (might be due to invalid IL or missing references)
			//IL_0751: Unknown result type (might be due to invalid IL or missing references)
			//IL_0760: Unknown result type (might be due to invalid IL or missing references)
			//IL_0765: Unknown result type (might be due to invalid IL or missing references)
			//IL_0789: Unknown result type (might be due to invalid IL or missing references)
			float num = c.Body.X / Mathf.Max(0.1f, Mathf.Abs(s.Scale.X));
			float num2 = c.Body.Y / Mathf.Max(0.1f, s.Scale.Y);
			Color gold = new Color(1f, 0.84f, 0.42f, 1f);
			Node2D val = new Node2D();
			((Node)val).AddChild((Node)(object)FxKit.Shape(new Vector2[8]
			{
				new Vector2(-9f, 10f),
				new Vector2(9f, 10f),
				new Vector2(11f, 0f),
				new Vector2(5f, -6f),
				new Vector2(7f, -12f),
				new Vector2(-7f, -12f),
				new Vector2(-5f, -6f),
				new Vector2(-11f, 0f)
			}, new Color(0.58f, 0.42f, 0.24f, 1f)), false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Props.Line(2f, new Color(0.3f, 0.2f, 0.1f, 1f), false, new Vector2(-6f, -6f), new Vector2(6f, -6f)), false, (InternalMode)0);
			Node2D val2 = new Node2D();
			((Node)val2).AddChild((Node)(object)FxKit.Shape(Props.Box(-11f, -7f, 11f, 7f), new Color(0.64f, 0.66f, 0.72f, 1f)), false, (InternalMode)0);
			((Node)val2).AddChild((Node)(object)Props.Line(1.6f, new Color(0.3f, 0.32f, 0.38f, 1f), false, new Vector2(-6f, 2f), new Vector2(-1f, -2f), new Vector2(5f, 2f)), false, (InternalMode)0);
			Node2D val3 = Props.Vial(8f, new Color(0.72f, 0.22f, 0.22f, 1f), new Color(0.98f, 0.4f, 0.34f, 1f), new Color(0.9f, 0.78f, 0.34f, 1f));
			val3.Position = new Vector2(0f, 4f);
			Node2D val4 = new Node2D();
			((Node)val4).AddChild((Node)(object)val3, false, (InternalMode)0);
			Node2D val5 = new Node2D();
			Line2D val6 = Props.Line(3.5f, new Color(0.82f, 0.74f, 0.56f, 1f), additive: false, Props.Circle(9f, 16));
			val6.Closed = true;
			((Node)val5).AddChild((Node)(object)val6, false, (InternalMode)0);
			Line2D val7 = Props.Line(2.5f, new Color(0.66f, 0.58f, 0.42f, 1f), additive: false, Props.Circle(4.5f, 12));
			val7.Closed = true;
			((Node)val5).AddChild((Node)(object)val7, false, (InternalMode)0);
			Node2D val8 = new Node2D();
			((Node)val8).AddChild((Node)(object)FxKit.Shape(FxKit.Star(4, 12f, 4.2f), new Color(0.86f, 0.9f, 0.96f, 1f)), false, (InternalMode)0);
			((Node)val8).AddChild((Node)(object)FxKit.Shape(Props.Circle(2.6f, 8), new Color(0.3f, 0.33f, 0.4f, 1f)), false, (InternalMode)0);
			Node2D val9 = new Node2D();
			Node2D val10 = FxKit.Blade(20f, 3.4f, new Color(0.62f, 0.95f, 0.95f, 1f), new Color(0.34f, 0.7f, 0.74f, 1f), new Color(0.4f, 0.44f, 0.52f, 1f), NoGlow);
			val10.Rotation = (float)Math.PI;
			val10.Position = new Vector2(0f, -6f);
			((Node)val9).AddChild((Node)(object)val10, false, (InternalMode)0);
			Node2D[] array = new Node2D[6] { val, val2, val4, val5, val8, val9 };
			Vector2 away = new Vector2(0f - (num * 0.3f + Tuning.F("intro.reach", 92f)), (0f - num2) * 0.42f);
			for (int i = 0; i < array.Length; i++)
			{
				Node2D item = array[i];
				Vector2 cell = new Vector2(-46f + 46f * (float)(i % 3), -38f + 50f * (float)(i / 3));
				item.Position = cell;
				((Node)s).AddChild((Node)(object)item, false, (InternalMode)0);
				Intro.Pop(item, 0.06f * (float)i, 0.15f, 1.35f);
				Intro.Run((Node)(object)item, 0.72f + 0.02f * (float)i, 0.24f, (float v) =>
				{
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0040: Unknown result type (might be due to invalid IL or missing references)
					item.Position = cell.Lerp(away, v * v);
					float num3 = 1.35f * (1f - 0.8f * v);
					item.Scale = new Vector2(num3, num3);
					item.Rotation = v * 3f;
				}, (TransitionType)0, (EaseType)2);
			}
			Node2D val11 = new Node2D
			{
				Position = new Vector2(3f, 0f),
				Scale = new Vector2((s.Scale.X < 0f) ? (-1f) : 1f, 1f)
			};
			((Node)s).AddChild((Node)(object)val11, false, (InternalMode)0);
			Line2D tick = Props.Line(4f, gold, true);
			((Node)val11).AddChild((Node)(object)tick, false, (InternalMode)0);
			Vector2[] path = new Vector2[3]
			{
				new Vector2(-27f, 54f),
				new Vector2(-12f, 69f),
				new Vector2(27f, 38f)
			};
			Node2D quill = Props.Quill(64f, new Color(0.97f, 0.93f, 0.8f, 1f), gold, new Color(0.56f, 0.4f, 0.16f, 1f), new Color(gold.R, gold.G, gold.B, 0.3f));
			quill.Rotation = 0.55f;
			quill.Position = path[0] + new Vector2(10f, -26f);
			((CanvasItem)quill).Modulate = new Color(1f, 1f, 1f, 0f);
			((Node)val11).AddChild((Node)(object)quill, false, (InternalMode)0);
			Intro.Run((Node)(object)quill, 0.36f, 0.1f, (float v) =>
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				quill.Position = path[0] + new Vector2(10f, -26f) * (1f - v);
				((CanvasItem)quill).Modulate = new Color(1f, 1f, 1f, v);
			}, (TransitionType)0, (EaseType)2);
			Intro.Run((Node)(object)tick, 0.46f, 0.16f, (float v) =>
			{
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				Vector2[] array2 = Intro.Partial(path, v);
				tick.Points = array2;
				quill.Position = array2[^1];
			}, (TransitionType)0, (EaseType)2);
			Intro.At((Node)(object)s, 0.62f, () =>
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				Intro.Flash(s, new Vector2(4f, 54f), new Color(gold.R, gold.G, gold.B, 0.6f), 60f, 1.8f, 0.2);
			});
			Intro.Run((Node)(object)quill, 0.64f, 0.1f, (float v) =>
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				quill.Position = path[2] + new Vector2(6f, -18f) * v;
				((CanvasItem)quill).Modulate = new Color(1f, 1f, 1f, 1f - v);
			}, (TransitionType)0, (EaseType)2);
			Intro.Run((Node)(object)tick, 0.72f, 0.2f, (float v) =>
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				((CanvasItem)tick).Modulate = new Color(1f, 1f, 1f, 1f - v);
			}, (TransitionType)0, (EaseType)2);
		}

		public static void Sneaky(CardFxContext c, Node2D s)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Expected Obj, but got Unknown
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			float w = c.Body.X / Mathf.Max(0.1f, Mathf.Abs(s.Scale.X));
			float num = c.Body.Y / Mathf.Max(0.1f, s.Scale.Y);
			float num2 = w * 0.58f;
			float num3 = (0f - num) * 0.56f;
			float num4 = num * 0.48f;
			float num5 = (0f - num2) * 1.15f;
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i <= 9; i++)
			{
				float num6 = (float)i / 9f;
				float num7 = Mathf.Sin(num6 * (float)Math.PI) * 0.22f;
				list.Add(new Vector2(num2 * (((i % 2 == 0) ? 0.5f : 0.88f) + num7), Mathf.Lerp(num3, num4, num6)));
			}
			Node2D wall = new Node2D();
			((Node)s).AddChild((Node)(object)wall, false, (InternalMode)0);
			Color val = new Color(0.07f, 0.05f, 0.13f, 0f);
			for (int j = 0; j < 9; j++)
			{
				float num8 = 0.86f * Mathf.Clamp(Mathf.Sin((float)j / 9f * (float)Math.PI) * 2.2f, 0.25f, 1f);
				float num9 = 0.86f * Mathf.Clamp(Mathf.Sin((float)(j + 1) / 9f * (float)Math.PI) * 2.2f, 0.25f, 1f);
				Polygon2D val2 = FxKit.Shape(new Vector2[4]
				{
					new Vector2(num5, list[j].Y),
					list[j],
					list[j + 1],
					new Vector2(num5, list[j + 1].Y)
				}, Colors.White);
				val2.Antialiased = false;
				val2.VertexColors = new Color[4]
				{
					val,
					new Color(0.07f, 0.05f, 0.13f, num8),
					new Color(0.07f, 0.05f, 0.13f, num9),
					val
				};
				((Node)wall).AddChild((Node)(object)val2, false, (InternalMode)0);
			}
			((Node)wall).AddChild((Node)(object)Props.Line(3.5f, new Color(0.6f, 0.4f, 0.92f, 0.85f), additive: true, list.ToArray()), false, (InternalMode)0);
			float start = (0f - w) * 1.9f;
			wall.Position = new Vector2(start, 0f);
			Intro.Run((Node)(object)wall, 0f, 0.2f, (float v) =>
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				wall.Position = new Vector2(Mathf.Lerp(start, 0f, v), 0f);
			}, (TransitionType)7, (EaseType)1);
			Intro.Run((Node)(object)wall, 0.36f, 0.3f, (float v) =>
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				((CanvasItem)wall).Modulate = new Color(1f, 1f, 1f, 1f - v);
				wall.Position = new Vector2(10f * v, -8f * v);
			}, (TransitionType)0, (EaseType)2);
			Intro.At((Node)(object)s, 0.34f, () =>
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				FxKit.Burst((Node)(object)s, s.ToGlobal(Vector2.Zero), new Color(0.3f, 0.2f, 0.45f, 0.55f), 14, 30f, 120f, 0.7, 0.5f, 1.3f, new Vector2(0f, -60f), 180f, null, w * 0.4f, 0f, additive: false);
			});
		}

		static Intros()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		}
	}
