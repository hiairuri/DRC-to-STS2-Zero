// namespace: SilentFX.Cards
// type: PiercingWailFx (class)
// source lines: 82108-82914
	public sealed class PiercingWailFx : CardFx
	{
		private sealed class Arc
		{
			public int Front;

			public float Side;

			public bool Shown;

			public Sprite2D[] Outer;

			public Sprite2D[] Core;
		}

		private sealed class Victim
		{
			public Vector2 Chest;

			public Vector2 Feet;

			public float Width;

			public float Far;

			public Node2D Body;

			public Creature Who;

			public NCreature Node;

			public Vector2 Applied = Vector2.One;

			public readonly float[] PassedAt = new float[3] { -1f, -1f, -1f };

			public Victim()
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			}
		}

		private sealed class Run
		{
			public Node2D Holder;

			public Node Parent;

			public ColorRect Veil;

			public ShaderMaterial Mat;

			public Sprite2D Glow;

			public readonly List<Arc> Arcs = new List<Arc>();

			public readonly List<Victim> Victims = new List<Victim>();

			public Vector2 Mouth;

			public Vector2 View;

			public Vector2 Body;

			public int Fronts;

			public float Gather;

			public float Gap;

			public float Travel;

			public float Tall;

			public float Half;

			public float Start;

			public float Reach;

			public float Sag;

			public float Thick;

			public float FrontsEnd;

			public float Total;

			public bool Burst;

			public bool Over;

			public bool Multiplayer;

			public Node Room;

			public readonly float[] Radius = new float[3];

			public readonly float[] Strength = new float[3];
		}

		private static readonly Color Wail = new Color(0.64f, 0.42f, 1f, 0.9f);

		private static readonly Color Dull = new Color(0.56f, 0.36f, 0.95f, 0.5f);

		private static readonly Color Drain = new Color(0.95f, 0.38f, 0.52f, 0.75f);

		private static readonly Color Breath = new Color(0.8f, 0.66f, 1f, 1f);

		private const int MaxFronts = 3;

		private const int Dots = 11;

		private static readonly float[] Force = new float[3] { 1f, 0.72f, 0.5f };

		private static readonly float[] SagShare = new float[3] { 1f, 0.45f, 0.3f };

		private static readonly List<Run> Live = new List<Run>();

		private static bool _warnedShader;

		private static bool _warnedStep;

		public static void Reset()
		{
			Run[] array = Live.ToArray();
			foreach (Run run in array)
			{
				try
				{
					Finish(run);
				}
				catch (Exception ex)
				{
					DevLog.Warn("Piercing Wail reset: " + ex.Message);
				}
			}
			Live.Clear();
		}

		public override void OnPlay(CardFxContext c)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected Obj, but got Unknown
			try
			{
				Node frontParent = c.FrontParent;
				if (!FxKit.Alive((GodotObject)(object)frontParent))
				{
					return;
				}
				Node2D val = null;
				try
				{
					val = new Node2D
					{
						Name = StringName.op_Implicit("SilentFX_Wail")
					};
					frontParent.AddChild((Node)(object)val, false, (InternalMode)0);
					if (!((Node)val).IsInsideTree())
					{
						((Node)val).QueueFree();
					}
					else
					{
						Build(c, val);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Piercing Wail could not be shown: " + ex.Message);
					if (FxKit.Alive((GodotObject)(object)val))
					{
						((Node)val).QueueFree();
					}
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("PiercingWailFx.OnPlay: " + ex2);
			}
		}

		private static float TravelOf(Run run, int front)
		{
			return run.Travel * (1f + 0.1f * (float)front);
		}

		private static void Build(CardFxContext c, Node2D holder)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0707: Unknown result type (might be due to invalid IL or missing references)
			//IL_070c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0711: Unknown result type (might be due to invalid IL or missing references)
			//IL_072e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0733: Unknown result type (might be due to invalid IL or missing references)
			//IL_073b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0740: Unknown result type (might be due to invalid IL or missing references)
			//IL_0759: Unknown result type (might be due to invalid IL or missing references)
			//IL_0760: Unknown result type (might be due to invalid IL or missing references)
			//IL_0768: Unknown result type (might be due to invalid IL or missing references)
			//IL_077b: Unknown result type (might be due to invalid IL or missing references)
			//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0531: Unknown result type (might be due to invalid IL or missing references)
			//IL_0540: Unknown result type (might be due to invalid IL or missing references)
			//IL_0574: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Expected Obj, but got Unknown
			//IL_0404: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_062b: Unknown result type (might be due to invalid IL or missing references)
			//IL_063a: Unknown result type (might be due to invalid IL or missing references)
			bool active = MultiplayerFx.Active;
			Run run = new Run
			{
				Holder = holder,
				Parent = c.FrontParent,
				Multiplayer = active,
				Room = (Node)(object)(active ? GameRefs.CombatRoom : null),
				Mouth = c.Chest + new Vector2(0f, (0f - c.Body.Y) * 0.18f),
				View = c.View,
				Body = c.Body,
				Fronts = Math.Clamp(Tuning.I("wail.fronts", 3), 1, 3),
				Gather = Mathf.Clamp(Tuning.F("wail.gather", 0.07f), 0f, 0.4f),
				Gap = Mathf.Clamp(Tuning.F("wail.gap", 0.1f), 0.02f, 0.5f),
				Travel = Mathf.Clamp(Tuning.F("wail.time", 0.52f), 0.15f, 3f),
				Tall = Mathf.Clamp(Tuning.F("wail.tall", 0.92f), 0.4f, 1.6f),
				Half = Mathf.Clamp(Tuning.F("wail.arc", 0.8f), 0.3f, 1.4f),
				Sag = Mathf.Clamp(Tuning.F("wail.sag", 0.11f), 0f, 0.3f),
				Thick = Mathf.Max(8f, c.Body.Y * Tuning.F("wail.thick", 0.26f))
			};
			if (run.View.X < 1f || run.View.Y < 1f)
			{
				run.View = new Vector2(1920f, 1080f);
			}
			Sfx.PlayAt("wail", run.Gather);
			run.Start = Mathf.Max(20f, run.Body.X * 0.25f);
			run.Reach = Mathf.Max(run.Mouth.X, run.View.X - run.Mouth.X) + run.View.X * 0.15f;
			run.Reach = Mathf.Max(run.Reach, run.Start + 200f);
			run.FrontsEnd = run.Gather + run.Gap * (float)(run.Fronts - 1) + TravelOf(run, run.Fronts - 1);
			run.Total = run.FrontsEnd + 0.7f;
			for (int i = 0; i < 3; i++)
			{
				run.Radius[i] = -1f;
				run.Strength[i] = 0f;
			}
			run.Mat = MakeWarp(c);
			if (run.Mat != null)
			{
				ColorRect val = FxKit.Veil(new Color(1f, 1f, 1f, 0f));
				((Node)val).Name = StringName.op_Implicit("SilentFX_WailWarp");
				((CanvasItem)val).Material = (Material)(object)run.Mat;
				((Node)val).AddChild((Node)new BackBufferCopy
				{
					CopyMode = (CopyModeEnum)2,
					ShowBehindParent = true
				}, false, (InternalMode)0);
				((Node)holder).AddChild((Node)(object)val, false, (InternalMode)0);
				run.Veil = val;
				run.Mat.SetShaderParameter(StringName.op_Implicit("tall"), Variant.op_Implicit(run.Tall));
				run.Mat.SetShaderParameter(StringName.op_Implicit("strength"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("wail.bend", 0.042f), 0f, 0.2f)));
				run.Mat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("wail.width", 0.055f), 0.01f, 0.3f)));
				run.Mat.SetShaderParameter(StringName.op_Implicit("centre"), Variant.op_Implicit(ScreenPoint(run, out var _)));
				run.Mat.SetShaderParameter(StringName.op_Implicit("fronts"), Variant.op_Implicit(run.Radius));
				run.Mat.SetShaderParameter(StringName.op_Implicit("powers"), Variant.op_Implicit(run.Strength));
			}
			run.Glow = FxKit.Glow(new Color(Breath.R, Breath.G, Breath.B, 0f), new Vector2(64f, 64f));
			((Node)holder).AddChild((Node)(object)run.Glow, false, (InternalMode)0);
			((Node2D)run.Glow).GlobalPosition = run.Mouth;
			for (int j = 0; j < run.Fronts; j++)
			{
				for (int k = -1; k <= 1; k += 2)
				{
					Arc arc = new Arc
					{
						Front = j,
						Side = k,
						Outer = new Sprite2D[11],
						Core = new Sprite2D[11]
					};
					for (int l = 0; l < 11; l++)
					{
						arc.Outer[l] = FxKit.Glow(new Color(Wail.R, Wail.G, Wail.B, 0f), new Vector2(64f, 64f));
						arc.Core[l] = FxKit.Glow(new Color(1f, 1f, 1f, 0f), new Vector2(64f, 64f));
						((CanvasItem)arc.Outer[l]).Visible = false;
						((CanvasItem)arc.Core[l]).Visible = false;
						((Node)holder).AddChild((Node)(object)arc.Outer[l], false, (InternalMode)0);
						((Node)holder).AddChild((Node)(object)arc.Core[l], false, (InternalMode)0);
					}
					run.Arcs.Add(arc);
				}
			}
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			foreach (FxTarget enemy in c.Enemies)
			{
				if (enemy == null)
				{
					continue;
				}
				Vector2 val2 = enemy.Chest - run.Mouth;
				float num = Mathf.Max(enemy.Width, 140f);
				Victim victim = new Victim
				{
					Chest = enemy.Chest,
					Feet = enemy.Feet,
					Width = num,
					Far = Mathf.Max(run.Start, Mathf.Sqrt(val2.X * val2.X + val2.Y * run.Tall * val2.Y * run.Tall) - num * 0.3f)
				};
				if (run.Sag > 0.001f && combatRoom != null)
				{
					object key = enemy.Key;
					Creature val3 = (Creature)((key is Creature) ? key : null);
					if (val3 != null)
					{
						try
						{
							NCreature creatureNode = combatRoom.GetCreatureNode(val3);
							if (FxKit.Alive((GodotObject)(object)creatureNode))
							{
								Node2D body = creatureNode.Body;
								if (FxKit.Alive((GodotObject)(object)body))
								{
									victim.Body = body;
									if (run.Multiplayer)
									{
										victim.Who = val3;
										victim.Node = creatureNode;
									}
								}
							}
						}
						catch
						{
							victim.Body = null;
						}
					}
				}
				run.Victims.Add(victim);
			}
			Live.Add(run);
			FxLifetime.Watch((Node)(object)holder, run.Total + 0.1f, () =>
			{
				Finish(run);
			});
			((Node)holder).TreeExiting += () =>
			{
				Restore(run);
			};
			FxKit.Drive((Node)(object)holder, run.Total, (float u) =>
			{
				Step(run, u * run.Total);
			}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				Finish(run);
			})));
		}

		private static ShaderMaterial MakeWarp(CardFxContext c)
		{
			try
			{
				if (Tuning.F("wail.warp", 1f) < 0.5f)
				{
					return null;
				}
				if (!File.Exists(Path.Combine(DevLog.ModDir, "shaders", "wail_warp.gdshader")))
				{
					if (!_warnedShader)
					{
						DevLog.Warn("Piercing Wail: shaders/wail_warp.gdshader is missing - the fronts are drawn without the distortion");
					}
					_warnedShader = true;
					return null;
				}
				CardModel card = c.Card;
				object obj;
				if (card == null)
				{
					obj = null;
				}
				else
				{
					ICombatState combatState = card.CombatState;
					obj = ((combatState != null) ? combatState.Encounter : null);
				}
				EncounterModel val = (EncounterModel)obj;
				if (val != null)
				{
					PropertyInfo property = typeof(EncounterModel).GetProperty("HasCustomBackground", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					bool flag = default;
					int num;
					if (property != null)
					{
						object value = property.GetValue(val);
						if (value is bool)
						{
							flag = (bool)value;
							num = 1;
						}
						else
						{
							num = 0;
						}
					}
					else
					{
						num = 0;
					}
					if (((uint)num & (flag ? 1u : 0u)) != 0)
					{
						return null;
					}
				}
				return ShaderLib.NewMaterial("wail_warp");
			}
			catch (Exception ex)
			{
				if (!_warnedShader)
				{
					DevLog.Warn("Piercing Wail: the distortion could not be made, the fronts are drawn without it: " + ex.Message);
				}
				_warnedShader = true;
				return null;
			}
		}

		private static Vector2 ScreenPoint(Run run, out float scale)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			scale = 1f;
			Vector2 val = run.Mouth;
			Node parent = run.Parent;
			CanvasItem val2 = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
			if (val2 != null && FxKit.Alive((GodotObject)(object)val2))
			{
				Transform2D canvasTransform = val2.GetCanvasTransform();
				val = canvasTransform * run.Mouth;
				float num = Mathf.Abs(canvasTransform.Scale.Y);
				if (num > 0.05f && num < 20f)
				{
					scale = num;
				}
			}
			return new Vector2(val.X / run.View.X, val.Y / run.View.Y);
		}

		private static void Step(Run run, float t)
		{
			if (run.Over || !FxKit.Alive((GodotObject)(object)run.Holder))
			{
				return;
			}
			try
			{
				if (run.Multiplayer)
				{
					RetireInvalidVictims(run);
				}
				Advance(run, t);
			}
			catch (Exception ex)
			{
				if (!_warnedStep)
				{
					DevLog.Warn("Piercing Wail stopped half-way: " + ex.Message);
				}
				_warnedStep = true;
				Finish(run);
			}
		}

		private static void RetireInvalidVictims(Run run)
		{
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			bool flag = MultiplayerFx.Active && (object)run.Room == combatRoom && (object)run.Parent == ((combatRoom != null) ? combatRoom.CombatVfxContainer : null) && FxKit.Alive((GodotObject)(object)run.Parent) && run.Parent.IsInsideTree();
			foreach (Victim victim in run.Victims)
			{
				if (victim.Body != null && (!flag || victim.Who == null || !victim.Who.IsAlive || !FxKit.Alive((GodotObject)(object)victim.Node) || !((Node)victim.Node).IsInsideTree() || !FxKit.Alive((GodotObject)(object)victim.Body) || !((Node)victim.Body).IsInsideTree() || victim.Node.Body != victim.Body || ((combatRoom != null) ? combatRoom.GetCreatureNode(victim.Who) : null) != victim.Node))
				{
					try
					{
						Shape(victim, Vector2.One);
					}
					finally
					{
						victim.Body = null;
					}
				}
			}
		}

		private static void Advance(Run run, float t)
		{
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			float num = ((run.Gather > 0.001f) ? Mathf.Clamp(t / run.Gather, 0f, 1f) : 1f);
			if (FxKit.Alive((GodotObject)(object)run.Glow))
			{
				float num2 = Mathf.Clamp((t - run.Gather) / 0.12f, 0f, 1f);
				if (num2 >= 1f)
				{
					if (((CanvasItem)run.Glow).Visible)
					{
						((CanvasItem)run.Glow).Visible = false;
					}
				}
				else
				{
					float num3 = Mathf.Lerp(run.Body.X * 1.5f, run.Body.X * 0.35f, num * num) * (1f + 2.2f * num2);
					((Node2D)run.Glow).Scale = new Vector2(num3, num3) / 64f;
					((CanvasItem)run.Glow).Modulate = new Color(Breath.R, Breath.G, Breath.B, (0.25f + 0.6f * num) * (1f - num2));
				}
			}
			if (!run.Burst && t >= run.Gather)
			{
				run.Burst = true;
				FxKit.Flash((Node)(object)run.Holder, run.Mouth, new Color(Breath.R, Breath.G, Breath.B, 0.7f), run.Body.X * 1.5f, 2.4f, 0.22);
				int num4 = Tuning.I("wail.shake", 2);
				if (num4 > 0)
				{
					GameRefs.Shake(num4);
				}
			}
			for (int i = 0; i < 3; i++)
			{
				run.Radius[i] = -1f;
				run.Strength[i] = 0f;
			}
			Vector2 val = ScreenPoint(run, out var scale);
			foreach (Arc arc in run.Arcs)
			{
				int front = arc.Front;
				float num5 = (t - run.Gather - run.Gap * (float)front) / TravelOf(run, front);
				float reached = ((num5 < 0f) ? (-1f) : run.Reach);
				if (num5 >= 0f && num5 < 1f)
				{
					float num6 = num5 * (1.6f - 0.6f * num5);
					float num7 = run.Start + (run.Reach - run.Start) * num6;
					float num8 = Mathf.Min(1f, num5 * 12f);
					reached = num7;
					run.Radius[front] = num7 * scale / run.View.Y;
					run.Strength[front] = Force[front] * num8 * (1f - num5 * num5 * num5);
					Lay(run, arc, num7, num6, Force[front] * num8 * Mathf.Pow(1f - num5, 0.8f));
				}
				else if (arc.Shown)
				{
					Hide(arc);
				}
				if (arc.Side > 0f)
				{
					Pass(run, front, reached, t);
				}
			}
			if (run.Mat != null)
			{
				if (t < run.FrontsEnd && FxKit.Alive((GodotObject)(object)run.Mat) && FxKit.Alive((GodotObject)(object)run.Veil))
				{
					run.Mat.SetShaderParameter(StringName.op_Implicit("centre"), Variant.op_Implicit(val));
					run.Mat.SetShaderParameter(StringName.op_Implicit("fronts"), Variant.op_Implicit(run.Radius));
					run.Mat.SetShaderParameter(StringName.op_Implicit("powers"), Variant.op_Implicit(run.Strength));
				}
				else
				{
					if (FxKit.Alive((GodotObject)(object)run.Veil))
					{
						((Node)run.Veil).QueueFree();
					}
					run.Veil = null;
					run.Mat = null;
				}
			}
			foreach (Victim victim in run.Victims)
			{
				if (victim.Body == null)
				{
					continue;
				}
				float num9 = 0f;
				for (int j = 0; j < run.Fronts; j++)
				{
					if (!(victim.PassedAt[j] < 0f))
					{
						float num10 = t - victim.PassedAt[j];
						num9 += SagShare[j] * (1f - Mathf.Exp((0f - num10) * 40f)) * Mathf.Exp((0f - num10) * 6f) * Mathf.Cos(num10 * 7f);
					}
				}
				num9 = Mathf.Clamp(num9, -0.5f, 1.4f) * run.Sag;
				Shape(victim, new Vector2(1f + 0.45f * num9, 1f - num9));
			}
		}

		private static void Lay(Run run, Arc arc, float r, float e, float alpha)
		{
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			float num = run.Thick * (1f + 1.3f * e) * (0.75f + 0.25f * Force[arc.Front]);
			float num2 = 2f * run.Half / 11f;
			for (int i = 0; i < 11; i++)
			{
				Sprite2D val = arc.Outer[i];
				Sprite2D val2 = arc.Core[i];
				if (FxKit.Alive((GodotObject)(object)val) && FxKit.Alive((GodotObject)(object)val2))
				{
					float num3 = ((float)i + 0.5f) / 11f;
					float num4 = Mathf.Lerp(0f - run.Half, run.Half, num3);
					float num5 = Mathf.Cos(num4);
					float num6 = Mathf.Sin(num4);
					Vector2 globalPosition = run.Mouth + new Vector2(arc.Side * r * num5, r / run.Tall * num6);
					Vector2 val3 = new Vector2((0f - arc.Side) * r * num6, r / run.Tall * num5);
					float globalRotation = val3.Angle();
					float num7 = Mathf.Pow(Mathf.Sin((float)Math.PI * num3), 0.8f);
					float num8 = val3.Length() * num2 * 3.2f;
					if (!arc.Shown)
					{
						((CanvasItem)val).Visible = true;
						((CanvasItem)val2).Visible = true;
					}
					((Node2D)val).GlobalPosition = globalPosition;
					((Node2D)val2).GlobalPosition = globalPosition;
					((Node2D)val).GlobalRotation = globalRotation;
					((Node2D)val2).GlobalRotation = globalRotation;
					((Node2D)val).Scale = new Vector2(num8, Mathf.Max(1f, num * num7)) / 64f;
					((Node2D)val2).Scale = new Vector2(num8 * 0.92f, Mathf.Max(0.5f, num * num7 * 0.34f)) / 64f;
					float num9 = Mathf.Clamp(alpha, 0f, 1f) * (0.2f + 0.8f * num7);
					((CanvasItem)val).Modulate = new Color(Wail.R, Wail.G, Wail.B, Wail.A * 0.42f * num9);
					((CanvasItem)val2).Modulate = new Color(Mathf.Lerp(Wail.R, 1f, 0.78f), Mathf.Lerp(Wail.G, 1f, 0.78f), Mathf.Lerp(Wail.B, 1f, 0.78f), 0.5f * num9);
				}
			}
			arc.Shown = true;
		}

		private static void Hide(Arc arc)
		{
			for (int i = 0; i < 11; i++)
			{
				if (FxKit.Alive((GodotObject)(object)arc.Outer[i]))
				{
					((CanvasItem)arc.Outer[i]).Visible = false;
				}
				if (FxKit.Alive((GodotObject)(object)arc.Core[i]))
				{
					((CanvasItem)arc.Core[i]).Visible = false;
				}
			}
			arc.Shown = false;
		}

		private static void Pass(Run run, int k, float reached, float t)
		{
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			if (reached < 0f)
			{
				return;
			}
			foreach (Victim victim in run.Victims)
			{
				if (!(victim.PassedAt[k] >= 0f) && !(reached < victim.Far))
				{
					victim.PassedAt[k] = t;
					if (k == 0)
					{
						FxKit.Flash((Node)(object)run.Holder, victim.Chest, Dull, victim.Width * 0.9f, 1.25f, 0.26);
						float num = Mathf.Max(70f, victim.Feet.Y - victim.Chest.Y);
						Brush.Dart((Node)(object)run.Holder, victim.Chest - new Vector2(0f, num * 0.6f), victim.Feet, num * 0.9f, 15f, Drain, 0.2f);
					}
					else
					{
						FxKit.Flash((Node)(object)run.Holder, victim.Chest, new Color(Dull.R, Dull.G, Dull.B, Dull.A * Force[k]), victim.Width * 0.7f, 1.2f, 0.2);
					}
				}
			}
		}

		private static void Shape(Victim v, Vector2 factor)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			if (v.Body != null)
			{
				if (!FxKit.Alive((GodotObject)(object)v.Body))
				{
					v.Body = null;
				}
				else if (!(factor == v.Applied))
				{
					Vector2 scale = v.Body.Scale;
					v.Body.Scale = new Vector2(scale.X / v.Applied.X * factor.X, scale.Y / v.Applied.Y * factor.Y);
					v.Applied = factor;
				}
			}
		}

		private static void Restore(Run run)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if (run.Over)
			{
				return;
			}
			run.Over = true;
			Live.Remove(run);
			foreach (Victim victim in run.Victims)
			{
				try
				{
					Shape(victim, Vector2.One);
				}
				catch (Exception ex)
				{
					DevLog.Warn("Piercing Wail restore: " + ex.Message);
				}
				victim.Body = null;
			}
		}

		private static void Finish(Run run)
		{
			Restore(run);
			if (FxKit.Alive((GodotObject)(object)run.Holder))
			{
				((Node)run.Holder).QueueFree();
			}
		}

		static PiercingWailFx()
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
