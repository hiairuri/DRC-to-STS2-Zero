// namespace: SilentFX.Cards
// type: ExposeFx (class)
// source lines: 46991-47670
	public sealed class ExposeFx : CardFx
	{
		private enum Step
		{
			Hold,
			Run,
			Recoil,
			Grind,
			Push,
			Give,
			Body
		}

		private sealed class Leg
		{
			public Step Kind;

			public float T0;

			public float T1;

			public float D0;

			public float D1;

			public float Speed;

			public float Boost;

			public float Ease;
		}

		private sealed class Cue
		{
			public float At;

			public int Kind;

			public int On;

			public bool Done;
		}

		private sealed class Plan
		{
			public readonly List<Leg> Legs = new List<Leg>();

			public readonly List<Cue> Cues = new List<Cue>();

			public float[] Contact = new float[0];

			public float[] Leave = new float[0];

			public float BodyFrom;

			public float BodyTime;

			public float Feet;

			public float End;
		}

		private static readonly Color Scan = new Color(0.94f, 0.98f, 1f, 1f);

		private static readonly Color Bare = new Color(0.88f, 0.94f, 1f, 1f);

		private static readonly Color Sore = new Color(1f, 0.26f, 0.23f, 1f);

		private static readonly Color Shard = new Color(0.72f, 0.88f, 1f, 0.9f);

		private static readonly Color Ember = new Color(1f, 0.78f, 0.36f, 1f);

		private const float Strike = 0.05f;

		private const float After = 0.32f;

		private const float Ebb = 0.035f;

		private const float Bite = 0.022f;

		private const float Slip = 0.42f;

		public override void OnPlay(CardFxContext c)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node parent = c.FrontParent;
				if (c.HasTarget && FxKit.Alive((GodotObject)(object)parent))
				{
					float k = Strokes.Scale(c);
					Vector2 feet = c.TargetFeet;
					float width = Mathf.Max(c.TargetWidth * 1.25f, 180f);
					float height = Mathf.Max(c.TargetHeight, 1f);
					Creature target = c.Target;
					CardModel card = c.Card;
					object caster;
					if (card == null)
					{
						caster = null;
					}
					else
					{
						Player owner = card.Owner;
						caster = ((owner != null) ? owner.Creature : null);
					}
					ExposeWipe.Arm(target, (Creature)caster, parent, (ExposeWipe.Run run) =>
					{
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						Sweep(parent, feet, width, height, k, run);
					}, c.Remote, c.Card);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("BlueSkills.OnPlay: " + ex);
			}
		}

		private static Plan Schedule(float start, float bottom, float top, float sweep, ExposeWipe.Zone zone)
		{
			Plan plan = new Plan();
			float num = Mathf.Max(1f, start - top);
			float num2 = Mathf.Max(1f, bottom - top);
			float num3 = 0.4f * num2 / sweep;
			float num4 = Mathf.Clamp(num3 * 1.6f, 650f, 1300f);
			float num5 = Mathf.Clamp(Tuning.F("expose.gear_time", 0.17f), 0.03f, 0.8f);
			float num6 = Mathf.Clamp(Tuning.F("expose.shield_time", 0.15f), 0.03f, 0.8f);
			plan.Legs.Add(new Leg
			{
				Kind = Step.Hold,
				T0 = 0f,
				T1 = 0.05f
			});
			float t = 0.05f;
			float d = 0f;
			float num7 = -1f;
			int num8 = zone?.Obstacles.Count ?? 0;
			plan.Contact = new float[num8];
			plan.Leave = new float[num8];
			for (int i = 0; i < num8; i++)
			{
				ExposeWipe.Obstacle obstacle = zone.Obstacles[i];
				float num9 = Mathf.Max(start - obstacle.Bottom, d);
				float num10 = Mathf.Max(start - obstacle.Top, num9 + 8f);
				plan.Contact[i] = -1f;
				plan.Leave[i] = -1f;
				if (num10 > num - 4f)
				{
					continue;
				}
				if (num9 > d + 0.5f)
				{
					num7 = Run(plan, ref t, ref d, num9, num4, num7);
				}
				plan.Contact[i] = t;
				if (obstacle.Gear)
				{
					float num11 = Mathf.Min(2.5f, num9);
					plan.Cues.Add(new Cue
					{
						At = t,
						Kind = 0,
						On = i
					});
					plan.Legs.Add(new Leg
					{
						Kind = Step.Recoil,
						T0 = t,
						T1 = t + 0.022f,
						D0 = num9,
						D1 = num9 - num11
					});
					t += 0.022f;
					plan.Legs.Add(new Leg
					{
						Kind = Step.Grind,
						T0 = t,
						T1 = t + num5,
						D0 = num9 - num11,
						D1 = num10
					});
					for (int j = 0; j < 3; j++)
					{
						plan.Cues.Add(new Cue
						{
							At = t + num5 * ((float)j + 0.42f) / 3f,
							Kind = 1,
							On = i
						});
					}
					t += num5;
					plan.Cues.Add(new Cue
					{
						At = t,
						Kind = 2,
						On = i
					});
					num7 = num4 * 1.6f;
				}
				else
				{
					float num12 = num9 + (num10 - num9) * 0.45f;
					float num13 = num6 * 0.7f;
					float num14 = num6 - num13;
					plan.Legs.Add(new Leg
					{
						Kind = Step.Push,
						T0 = t,
						T1 = t + num13,
						D0 = num9,
						D1 = num12
					});
					plan.Cues.Add(new Cue
					{
						At = t + num13 * 0.3f,
						Kind = 3,
						On = i
					});
					plan.Cues.Add(new Cue
					{
						At = t + num13 * 0.68f,
						Kind = 3,
						On = i
					});
					t += num13;
					plan.Cues.Add(new Cue
					{
						At = t,
						Kind = 4,
						On = i
					});
					plan.Legs.Add(new Leg
					{
						Kind = Step.Give,
						T0 = t,
						T1 = t + num14,
						D0 = num12,
						D1 = num10
					});
					t += num14;
					num7 = Mathf.Min(2f * (num10 - num12) / num14, num4 * 2f);
				}
				d = num10;
				plan.Leave[i] = t;
			}
			float num15 = Mathf.Clamp(start - bottom, 0f, num);
			float num16 = num;
			float num17 = sweep * Mathf.Clamp(1f + 0.5f * (num - num2) / num2, 1f, 1.5f);
			float ease = 0.4f;
			if (num7 >= 0f)
			{
				if (num15 > d + 0.5f)
				{
					num7 = Run(plan, ref t, ref d, num15, num3, num7);
				}
				num16 = Mathf.Max(1f, num - d);
				num17 = sweep * Mathf.Clamp(num16 / num2, 0.5f, 1f);
				ease = Mathf.Clamp(num7 * num17 / num16, 0.4f, 0.85f);
			}
			plan.Legs.Add(new Leg
			{
				Kind = Step.Body,
				T0 = t,
				T1 = t + num17,
				D0 = d,
				D1 = num,
				Ease = ease
			});
			plan.BodyFrom = t;
			plan.BodyTime = num17;
			plan.End = t + num17;
			plan.Feet = plan.End;
			for (float num18 = 0.05f; num18 < plan.End; num18 += 0.004166667f)
			{
				if (!(Travel(plan, num18) < num15 - 0.5f))
				{
					plan.Feet = num18;
					break;
				}
			}
			return plan;
		}

		private static float Run(Plan plan, ref float t, ref float d, float to, float nominal, float entry)
		{
			float num = to - d;
			float num2 = ((entry < 0f) ? 0f : (Mathf.Max(entry, nominal * 0.2f) - nominal));
			float num3 = 0f;
			float num4 = num / Mathf.Max(1f, Mathf.Min(nominal, nominal + num2)) + 0.001f;
			for (int i = 0; i < 24; i++)
			{
				float num5 = (num3 + num4) * 0.5f;
				if (Gone(num5, nominal, num2) < num)
				{
					num3 = num5;
				}
				else
				{
					num4 = num5;
				}
			}
			float num6 = Mathf.Max(0.001f, num4);
			plan.Legs.Add(new Leg
			{
				Kind = Step.Run,
				T0 = t,
				T1 = t + num6,
				D0 = d,
				D1 = to,
				Speed = nominal,
				Boost = num2
			});
			t += num6;
			d = to;
			return nominal + num2 * Mathf.Exp((0f - num6) / 0.035f);
		}

		private static float Gone(float time, float nominal, float boost)
		{
			return nominal * time + boost * 0.035f * (1f - Mathf.Exp((0f - time) / 0.035f));
		}

		private static float Teeth(float u)
		{
			float num = Mathf.Clamp(u, 0f, 1f) * 3f;
			float num2 = Mathf.Min(2f, Mathf.Floor(num));
			float num3 = num - num2;
			float num4 = Mathf.Clamp(num3 / 0.42f, 0f, 1f);
			float num5 = Mathf.Clamp((num3 - 0.42f) / 0.58000004f, 0f, 1f);
			return (num2 + 0.86f * num4 * num4 * (3f - 2f * num4) + 0.14f * num5 * num5) / 3f;
		}

		private static float Travel(Plan plan, float t)
		{
			List<Leg> legs = plan.Legs;
			for (int i = 0; i < legs.Count; i++)
			{
				Leg leg = legs[i];
				if (!(t > leg.T1) || i >= legs.Count - 1)
				{
					float num = Mathf.Max(0.0001f, leg.T1 - leg.T0);
					float num2 = Mathf.Clamp((t - leg.T0) / num, 0f, 1f);
					return leg.Kind switch
					{
						Step.Run => Mathf.Min(leg.D1, leg.D0 + Gone(num2 * num, leg.Speed, leg.Boost)), 
						Step.Recoil => Mathf.Lerp(leg.D0, leg.D1, 1f - (1f - num2) * (1f - num2)), 
						Step.Grind => Mathf.Lerp(leg.D0, leg.D1, Teeth(num2)), 
						Step.Push => Mathf.Lerp(leg.D0, leg.D1, 1f - (1f - num2) * (1f - num2)), 
						Step.Give => Mathf.Lerp(leg.D0, leg.D1, num2 * num2), 
						Step.Body => Mathf.Lerp(leg.D0, leg.D1, num2 * (leg.Ease + (1f - leg.Ease) * num2)), 
						_ => leg.D0, 
					};
				}
			}
			return 0f;
		}

		private static void Sweep(Node parent, Vector2 feet, float width, float height, float k, ExposeWipe.Run run)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Expected Obj, but got Unknown
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			if ((run.Remote && !ExposeWipe.CanShow(run)) || !FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			using (RemoteFxFade.Capture(run.Remote, parent))
			{
				Node2D root = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_ExposeScan")
				};
				parent.AddChild((Node)(object)root, false, (InternalMode)0);
				if (!((Node)root).IsInsideTree())
				{
					((Node)root).QueueFree();
					return;
				}
				ExposeWipe.Zone zone = ExposeWipe.Open(run, parent, feet);
				Sprite2D wash = Dot((Node)(object)root);
				Sprite2D[] band = new Sprite2D[3]
				{
					Dot((Node)(object)root),
					Dot((Node)(object)root),
					Dot((Node)(object)root)
				};
				Sprite2D halo = Dot((Node)(object)root);
				Sprite2D line = Dot((Node)(object)root);
				Sprite2D hot = Dot((Node)(object)root);
				Sprite2D spot = Dot((Node)(object)root);
				Sprite2D core = Dot((Node)(object)root);
				float sweep = Mathf.Max(0.08f, Tuning.F("expose.scan_time", 0.24f));
				float bare = Tuning.F("expose.bare", 0.3f);
				float bottom = feet.Y + 6f;
				float head = feet.Y - height;
				float top = head - height * 0.14f - 18f;
				float num = feet.X - width * 0.5f;
				float num2 = feet.X + width * 0.5f;
				float start = bottom;
				if (zone != null)
				{
					num = Mathf.Min(num, zone.Left - 12f);
					num2 = Mathf.Max(num2, zone.Right + 12f);
					start = Mathf.Max(bottom, zone.Bottom) + 12f;
				}
				float wide = num2 - num;
				float mid = (num + num2) * 0.5f;
				Plan plan = Schedule(start, bottom, top, sweep, zone);
				float total = plan.End + 0.32f;
				float shedAt = ((plan.Feet <= 0.051f) ? 0f : plan.Feet);
				int fleckCount = Mathf.Max(1, Tuning.I("expose.flecks", 18));
				GpuParticles2D flecks = null;
				bool shed = false;
				bool winked = false;
				int frame = 0;
				FxKit.Drive((Node)(object)root, total, (float u) =>
				{
					//IL_022d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0238: Unknown result type (might be due to invalid IL or missing references)
					//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
					//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
					//IL_041f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0438: Unknown result type (might be due to invalid IL or missing references)
					//IL_044a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0460: Unknown result type (might be due to invalid IL or missing references)
					//IL_0500: Unknown result type (might be due to invalid IL or missing references)
					//IL_050c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0521: Unknown result type (might be due to invalid IL or missing references)
					//IL_052d: Unknown result type (might be due to invalid IL or missing references)
					//IL_047c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0483: Unknown result type (might be due to invalid IL or missing references)
					//IL_048e: Unknown result type (might be due to invalid IL or missing references)
					//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
					//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
					//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
					//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
					//IL_0550: Unknown result type (might be due to invalid IL or missing references)
					//IL_056d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0572: Unknown result type (might be due to invalid IL or missing references)
					//IL_059f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0670: Unknown result type (might be due to invalid IL or missing references)
					//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
					//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
					//IL_085f: Unknown result type (might be due to invalid IL or missing references)
					//IL_073b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0754: Unknown result type (might be due to invalid IL or missing references)
					//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
					//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
					//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
					//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
					//IL_0824: Unknown result type (might be due to invalid IL or missing references)
					if (FxKit.Alive((GodotObject)(object)root))
					{
						if (run.Remote && !ExposeWipe.CanShow(run))
						{
							((Node)root).QueueFree();
						}
						else
						{
							frame++;
							float num3 = u * total;
							float num4 = Mathf.Clamp(num3 / 0.05f, 0f, 1f);
							float num5 = Mathf.Clamp((num3 - 0.05f) / 0.05f, 0f, 1f);
							float num6 = Mathf.Clamp((num3 - plan.BodyFrom) / plan.BodyTime, 0f, 1f);
							float num7 = Mathf.Clamp((num3 - plan.End) / 0.32f, 0f, 1f);
							float num8 = start - Travel(plan, Mathf.Min(num3, plan.End));
							int num9 = -1;
							float num10 = 0f;
							bool flag = false;
							for (int i = 0; i < plan.Contact.Length; i++)
							{
								if (!(plan.Contact[i] < 0f) && !(num3 < plan.Contact[i]))
								{
									bool flag2 = num3 <= plan.Leave[i];
									float num11 = (flag2 ? 1f : Mathf.Clamp(1f - (num3 - plan.Leave[i]) / 0.07f, 0f, 1f));
									if (!(num11 <= num10))
									{
										num10 = num11;
										num9 = i;
										flag = flag2;
									}
								}
							}
							ExposeWipe.Obstacle obstacle = ((num9 >= 0) ? zone.Obstacles[num9] : null);
							float num12 = (flag ? ((FxKit.Hash(frame * 7 + 1) - 0.5f) * 2f) : 0f);
							float shake = ((flag && obstacle.Gear) ? ((FxKit.Hash(frame * 13 + 5) - 0.5f) * 3f) : 0f);
							Vector2 val = new Vector2(mid, num8 + num12);
							ExposeWipe.Line(run, val, shake);
							for (int j = 0; j < plan.Cues.Count; j++)
							{
								Cue cue = plan.Cues[j];
								if (!cue.Done && !(num3 < cue.At))
								{
									cue.Done = true;
									Throw(parent, zone.Obstacles[cue.On], cue.Kind, j, num8, k);
									if (cue.Kind == 0)
									{
										Sfx.Play("expose_gear");
									}
									else if (cue.Kind == 3 && (j == 0 || plan.Cues[j - 1].Kind != 3 || plan.Cues[j - 1].On != cue.On))
									{
										Sfx.Play("expose_shield");
									}
								}
							}
							float num13 = Mathf.Clamp((num6 - 0.8f) / 0.2f, 0f, 1f);
							float num14 = wide * ((num4 < 1f) ? Mathf.Lerp(0.35f, 1.12f, num4) : Mathf.Lerp(1.12f, 1f, num5)) * (1f - 0.6f * num13 * num13);
							float num15 = num4 * (1f - Mathf.Min(1f, num7 * 5f));
							Place(halo, val, num14 * 1.12f, 40f * k, Scan, (0.5f + 0.16f * num10) * num15);
							Place(line, val, num14, (9f + 2f * num10) * k, Scan, num15);
							Place(hot, val, num14 * 0.8f, 4f * k, Colors.White, num15);
							if (obstacle != null)
							{
								Vector2 at = new Vector2(obstacle.X, val.Y);
								Place(spot, at, obstacle.Width * 1.7f, 22f * k, obstacle.Gear ? Ember : Shard, 0.85f * num10 * num15);
								Place(core, at, obstacle.Width, 7f * k, Colors.White, num10 * num15);
							}
							else
							{
								Place(spot, val, 1f, 1f, Scan, 0f);
								Place(core, val, 1f, 1f, Scan, 0f);
							}
							float num16 = num5 * num15;
							for (int l = 0; l < band.Length; l++)
							{
								Place(band[l], val + new Vector2(0f, (14f + 24f * (float)l) * k), num14 * (1f - 0.1f * (float)l), (26f + 22f * (float)l) * k, Scan, (0.3f - 0.09f * (float)l) * num16);
							}
							float num17 = Mathf.Clamp(bottom - num8, 0f, bottom - head + height * 0.05f);
							float num18 = Mathf.Clamp(num7 * 2.5f, 0f, 1f);
							Color color = new Color(Mathf.Lerp(Bare.R, Sore.R, num18), Mathf.Lerp(Bare.G, Sore.G, num18), Mathf.Lerp(Bare.B, Sore.B, num18), 1f);
							float alpha = bare * Mathf.Min(1f, num17 / 50f) * (1f + 0.35f * Mathf.Sin((float)Math.PI * num18)) * (1f - num7 * num7);
							Place(wash, new Vector2(feet.X, bottom - num17 * 0.5f), width * 0.92f, num17 * 1.3f + 12f, color, alpha);
							if (!shed && num3 >= shedAt)
							{
								shed = true;
								flecks = FxKit.Burst(parent, new Vector2(feet.X, Mathf.Min(num8, bottom)), new Color(0.8f, 0.92f, 1f, 0.95f), fleckCount, 70f, 260f, Mathf.Max(0.05f, plan.End - shedAt), 0.07f * k, 0.17f * k, new Vector2(0f, -240f), 24f, new Vector2(0f, -1f), 6f, 0f, additive: true, 0f);
								if (FxKit.Alive((GodotObject)(object)flecks))
								{
									Material processMaterial = flecks.ProcessMaterial;
									ParticleProcessMaterial val2 = (ParticleProcessMaterial)(object)((processMaterial is ParticleProcessMaterial) ? processMaterial : null);
									if (val2 != null)
									{
										val2.EmissionShape = (EmissionShapeEnum)3;
										val2.EmissionBoxExtents = new Vector3(width * 0.36f, 3f, 0f);
									}
								}
							}
							if (FxKit.Alive((GodotObject)(object)flecks))
							{
								((Node2D)flecks).GlobalPosition = new Vector2(feet.X, Mathf.Min(num8, bottom) + 4f);
							}
							if (!winked && num3 >= plan.End)
							{
								winked = true;
								ExposeWipe.Passed(run);
								FxKit.Flash(parent, new Vector2(mid, top), new Color(1f, 1f, 1f, 0.75f), 30f * k, 2.4f, 0.14);
							}
						}
					}
				}, (TransitionType)0, (EaseType)2);
				FxKit.FreeAfter((Node)(object)root, (double)total + 0.1);
			}
		}

		private static void Throw(Node parent, ExposeWipe.Obstacle on, int kind, int seed, float y, float k)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)parent) && on != null)
			{
				Vector2 val = new Vector2(on.X, y);
				float num = ((seed % 2 == 0) ? 1f : (-1f));
				switch (kind)
				{
				case 0:
					Brush.Sparks(parent, val, Brush.Gold, 5, 520f, new Vector2(1f, -0.3f), 30f);
					Brush.Sparks(parent, val, Brush.Gold, 5, 520f, new Vector2(-1f, -0.3f), 30f);
					FxKit.Flash(parent, val, new Color(1f, 0.92f, 0.7f, 0.7f), 24f * k, 1.8f, 0.1);
					break;
				case 1:
					Brush.Sparks(parent, val, Brush.Amber, 3, 400f, new Vector2(num, -0.2f), 34f);
					break;
				case 2:
					Brush.Sparks(parent, val, Brush.Gold, 4, 600f, new Vector2(1f, -0.45f), 36f);
					Brush.Sparks(parent, val, Brush.Gold, 4, 600f, new Vector2(-1f, -0.45f), 36f);
					FxKit.Flash(parent, val, new Color(1f, 1f, 1f, 0.6f), 20f * k, 2f, 0.1);
					break;
				case 3:
					FxKit.Burst(parent, val, Shard, 2, 120f, 260f, 0.34, 0.08f * k, 0.16f * k, new Vector2(0f, 420f), 30f, new Vector2(num, -0.15f), 6f);
					break;
				default:
				{
					Vector2 val2 = new Vector2(on.X, (on.Top + on.Bottom) * 0.5f);
					FxKit.Burst(parent, val2, Shard, 6, 200f, 480f, 0.42, 0.1f * k, 0.22f * k, new Vector2(0f, 420f), 34f, new Vector2(1f, -0.15f), 8f);
					FxKit.Burst(parent, val2, Shard, 6, 200f, 480f, 0.42, 0.1f * k, 0.22f * k, new Vector2(0f, 420f), 34f, new Vector2(-1f, -0.15f), 8f);
					FxKit.Flash(parent, val2, new Color(Shard.R, Shard.G, Shard.B, 0.55f), Mathf.Max(on.Width, on.Bottom - on.Top) * 1.2f, 1.9f, 0.14);
					break;
				}
				}
			}
		}

		private static Sprite2D Dot(Node root)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			Sprite2D val = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
			root.AddChild((Node)(object)val, false, (InternalMode)0);
			return val;
		}

		private static void Place(Sprite2D dot, Vector2 at, float wide, float tall, Color color, float alpha)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)dot))
			{
				((Node2D)dot).GlobalPosition = at;
				((Node2D)dot).Scale = new Vector2(Mathf.Max(1f, wide), Mathf.Max(1f, tall)) / 64f;
				((CanvasItem)dot).Modulate = new Color(color.R, color.G, color.B, Mathf.Clamp(alpha, 0f, 1f));
			}
		}

		static ExposeFx()
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
		}
	}
