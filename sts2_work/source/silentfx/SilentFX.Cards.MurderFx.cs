// namespace: SilentFX.Cards
// type: MurderFx (class)
// source lines: 79613-80859
	public sealed class MurderFx : CardFx
	{
		private sealed class Blow
		{
			public Creature Who;

			public float Stop;

			public float Beat;

			public Vector2 PushA;

			public Vector2 PushB;

			public Node2D Picture;

			public Vector2 Home;

			public Vector2 Wrote;

			public Vector2 Shift;

			public CanvasItem Lit;

			public Color LitHome;

			public Color LitWrote;

			public float Glare;

			public bool Over;

			public bool Owned;

			public LightGroup LightGroup;

			public Action ReleaseCrabClaim;

			public Node Room;

			public Node VictimNode;

			public Node2D VictimBody;
		}

		private sealed class LightGroup
		{
			public CanvasItem Body;

			public Color Home;

			public Color Wrote;

			public bool Retired;

			public readonly Dictionary<Blow, float> Gains = new Dictionary<Blow, float>(ReferenceEqualityComparer.Instance);
		}

		private sealed class CrabGroup
		{
			public Node2D Sprite;

			public bool Retired;

			public readonly HashSet<object> Claims = new HashSet<object>(ReferenceEqualityComparer.Instance);
		}

		internal static readonly Color Blood = new Color(1f, 0.12f, 0.18f, 1f);

		private static readonly Dictionary<CanvasItem, LightGroup> Lights = new Dictionary<CanvasItem, LightGroup>(ReferenceEqualityComparer.Instance);

		private static readonly Dictionary<Node2D, CrabGroup> Crabs = new Dictionary<Node2D, CrabGroup>(ReferenceEqualityComparer.Instance);

		private static readonly List<Blow> Live = new List<Blow>();

		private static Node2D _crab;

		private static int _crabHolds;

		public static void Reset()
		{
			Blow[] array = Live.ToArray();
			foreach (Blow blow in array)
			{
				try
				{
					LetGo(blow);
				}
				catch (Exception ex)
				{
					DevLog.Warn("Murder reset: " + ex.Message);
				}
			}
			Live.Clear();
			RetireOwned();
			ReleaseCrab();
		}

		internal static void Dim(float attack, float life = 0.25f)
		{
			Mood.Hold("murder", Sfx.Knob("mood.murder.volume", 0.55f), Sfx.Knob("mood.murder.pitch", 1f), Sfx.Knob("mood.murder.far", 0.55f), Sfx.Knob("mood.murder.hollow", 0.35f), attack, Mathf.Max(0.05f, Sfx.Knob("mood.murder.back", 0.45f)), life);
		}

		public override void OnPlay(CardFxContext c)
		{
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				int num;
				if (c != null)
				{
					num = (c.Remote ? 1 : 0);
					if (num != 0)
					{
						goto IL_0057;
					}
				}
				else
				{
					num = 0;
				}
				Dim(0.15f, Tuning.F("murder.hold", 0.75f) + 0.3f);
				Sfx.Hold("murder_drone", 1f, 1f, Tuning.F("murder.hold", 0.75f) + 0.3f);
				goto IL_0057;
				IL_0057:
				if (num == 0 && FxKit.Alive((GodotObject)(object)c.BackParent))
				{
					float num2 = ((c.Held is MurderHeld) ? Tuning.F("murder.held_dark", 0.4f) : 0f);
					ColorRect veil = FxKit.Veil(new Color(0.1f, 0f, 0.02f, num2));
					c.BackParent.AddChild((Node)(object)veil, false, (InternalMode)0);
					Tween val = ((Node)veil).CreateTween();
					val.TweenProperty((GodotObject)(object)veil, NodePath.op_Implicit("color:a"), Variant.op_Implicit(Tuning.F("murder.dark", 0.55f)), 0.15);
					val.TweenInterval((double)Tuning.F("murder.hold", 0.75f));
					val.TweenProperty((GodotObject)(object)veil, NodePath.op_Implicit("color:a"), Variant.op_Implicit(0f), 0.4);
					val.TweenCallback(Callable.From((Action)(() =>
					{
						if (FxKit.Alive((GodotObject)(object)veil))
						{
							((Node)veil).QueueFree();
						}
					})));
				}
				if (FxKit.Alive((GodotObject)(object)c.FrontParent))
				{
					Sprite2D val2 = FxKit.Glow(new Color(Blood.R, Blood.G, Blood.B, 0.9f), new Vector2(50f, 50f));
					c.FrontParent.AddChild((Node)(object)val2, false, (InternalMode)0);
					((Node2D)val2).GlobalPosition = c.Chest + new Vector2(c.Body.X * 0.15f * c.Facing, c.Body.Y * 0.1f);
					Tween val3 = ((Node)val2).CreateTween();
					val3.SetParallel(true);
					val3.TweenProperty((GodotObject)(object)val2, NodePath.op_Implicit("scale"), Variant.op_Implicit(new Vector2(3.4f, 3.4f)), 0.45).SetTrans((TransitionType)4).SetEase((EaseType)1);
					val3.TweenProperty((GodotObject)(object)val2, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.45);
					FxKit.FreeAfter((Node)(object)val2, 0.6);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Murder (played): " + ex.Message);
			}
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_055b: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Expected Obj, but got Unknown
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			object obj;
			if (c == null)
			{
				obj = null;
			}
			else
			{
				CardModel card = c.Card;
				if (card == null)
				{
					obj = null;
				}
				else
				{
					Player owner = card.Owner;
					obj = ((owner != null) ? owner.Creature : null);
				}
			}
			Creature val = (Creature)obj;
			if (val != null && c.Target == val)
			{
				return;
			}
			bool remote = c?.Remote ?? false;
			if (!MultiplayerFx.Active)
			{
				RetireOwned();
			}
			else
			{
				RetireLegacyLights();
			}
			Node parent = c.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Node2D holder = null;
			try
			{
				float size = Mathf.Clamp(c.TargetWidth * 1.1f, 230f, 420f) * Tuning.F("murder.size", 1f);
				float num = ((c.TargetSide < 0f) ? (-1f) : 1f);
				float num2 = Weight(c);
				float beat = Mathf.Clamp(Tuning.F("murder.beat", 0.08f), 0.03f, 0.3f);
				float stop = Mathf.Clamp(Mathf.Lerp(Tuning.F("murder.stop_min", 0.08f), Tuning.F("murder.stop_max", 0.22f), num2), 0.05f, 0.4f);
				if (killed)
				{
					stop = Mathf.Min(stop, 0.08f);
				}
				holder = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_Murder")
				};
				parent.AddChild((Node)(object)holder, false, (InternalMode)0);
				if (!((Node)holder).IsInsideTree())
				{
					((Node)holder).QueueFree();
					return;
				}
				holder.GlobalPosition = at;
				holder.Scale = new Vector2(num, 1f);
				Vector2 val2 = new Vector2(num, 0.62f);
				Vector2 dirA = val2.Normalized();
				val2 = new Vector2((0f - num) * 1.6f, 1.05f);
				Vector2 dirB = val2.Normalized();
				float num3 = Tuning.F("murder.knock", 20f) * Mathf.Lerp(1f, 1.7f, num2);
				Blow blow = new Blow
				{
					Who = c.Target,
					Stop = stop,
					Beat = beat,
					Owned = MultiplayerFx.Active
				};
				val2 = new Vector2(dirA.X, dirA.Y * 0.5f);
				blow.PushA = val2.Normalized() * num3;
				val2 = new Vector2(dirB.X, dirB.Y * 0.5f);
				blow.PushB = val2.Normalized() * (num3 * 0.35f);
				Blow blow2 = blow;
				float w = Mathf.Clamp(c.TargetWidth, 150f, 640f);
				float h = Mathf.Clamp(c.TargetHeight, 150f, 520f);
				Wash(parent, at, w, h, stop);
				Slash(holder, new Vector2(0f - size, (0f - size) * 0.62f), new Vector2(size, size * 0.62f), size * 0.3f, size * 0.2f, stop);
				float back = Mathf.Max(0.05f, Sfx.Knob("mood.murder.back", 0.45f));
				if (!remote)
				{
					Dim(0.05f, stop + 0.12f);
					Mood.Hold("murder_stop", Sfx.Knob("mood.murder.stop_volume", 0.06f), Sfx.Knob("mood.murder.pitch", 1f), Sfx.Knob("mood.murder.far", 0.55f), Sfx.Knob("mood.murder.hollow", 0.35f), 0.03f, back, stop + 0.12f);
				}
				if (!remote)
				{
					Sfx.Hush("murder_drone", 0.04f);
				}
				Sfx.Play("murder_edge", Mathf.Lerp(0.8f, 1f, num2));
				Sfx.PlayAt("murder_cut", stop, Mathf.Lerp(0.85f, 1.1f, num2));
				Sfx.PlayAt("murder_cross", stop + beat, Mathf.Lerp(0.8f, 1f, num2));
				if (!killed)
				{
					try
					{
						Seize(blow2, holder);
					}
					catch (Exception ex)
					{
						DevLog.Warn("Murder (hold): " + ex.Message);
						LetGo(blow2);
						blow2.Over = false;
					}
				}
				float total = stop + beat + 0.6f;
				Live.Add(blow2);
				((Node)holder).TreeExiting += () =>
				{
					LetGo(blow2);
				};
				bool opened = false;
				bool crossed = false;
				FxKit.Drive((Node)(object)holder, total, (float u) =>
				{
					//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
					//IL_005a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0066: Unknown result type (might be due to invalid IL or missing references)
					float num4 = u * total;
					if (!opened && num4 >= stop)
					{
						opened = true;
						if (!remote)
						{
							Mood.Clear("murder_stop", back);
							Mood.Clear("murder", back);
						}
						try
						{
							Open(parent, holder, at, size, dirA, w, remote);
						}
						catch (Exception ex3)
						{
							DevLog.Warn("Murder (cut): " + ex3.Message);
						}
					}
					if (!crossed && num4 >= stop + beat)
					{
						crossed = true;
						try
						{
							Cross(parent, holder, at, size, dirB, remote);
						}
						catch (Exception ex4)
						{
							DevLog.Warn("Murder (second cut): " + ex4.Message);
						}
					}
					Step(blow2, num4);
				}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
				{
					LetGo(blow2);
					if (FxKit.Alive((GodotObject)(object)holder))
					{
						((Node)holder).QueueFree();
					}
				})));
				if (blow2.Owned)
				{
					FxLifetime.Watch((Node)(object)holder, total, () =>
					{
						LetGo(blow2);
					});
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("Murder (hit): " + ex2.Message);
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			}
		}

		private static float Weight(CardFxContext c)
		{
			if (c.Card == null)
			{
				return 0.5f;
			}
			float num = 0f;
			try
			{
				num = (float)((CalculatedVar)c.Card.DynamicVars.CalculatedDamage).Calculate(c.Target);
			}
			catch
			{
				num = 0f;
			}
			float num2 = Tuning.F("murder.stop_low", 8f);
			float num3 = Mathf.Max(num2 + 1f, Tuning.F("murder.stop_high", 45f));
			return Mathf.Clamp((num - num2) / (num3 - num2), 0f, 1f);
		}

		private static void Seize(Blow blow, Node2D holder)
		{
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			Creature who = blow.Who;
			if (who == null || !who.IsAlive)
			{
				return;
			}
			if (blow.Owned)
			{
				CreatureHold.Hold(who, blow.Stop, blow);
			}
			else
			{
				CreatureHold.Hold(who, blow.Stop);
			}
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			if (combatRoom == null)
			{
				return;
			}
			NCreature creatureNode = combatRoom.GetCreatureNode(who);
			if (!FxKit.Alive((GodotObject)(object)creatureNode) || !((Node)creatureNode).IsInsideTree())
			{
				return;
			}
			if (blow.Owned)
			{
				blow.Room = (Node)(object)combatRoom;
				blow.VictimNode = (Node)(object)creatureNode;
				blow.VictimBody = creatureNode.Body;
			}
			Node2D body = creatureNode.Body;
			if (FxKit.Alive((GodotObject)(object)body) && ((CanvasItem)body).Visible)
			{
				if (Tuning.F("murder.knock", 20f) > 0.01f)
				{
					blow.Picture = body;
				}
				float num = Mathf.Min(Tuning.F("murder.whiten", 2.6f), 6f);
				if (num > 1.01f)
				{
					blow.Lit = (CanvasItem)(object)body;
					blow.LitHome = ((CanvasItem)body).Modulate;
					blow.LitWrote = blow.LitHome;
					blow.Glare = num;
					if (blow.Owned)
					{
						JoinLight(blow, (CanvasItem)(object)body);
					}
				}
			}
			else
			{
				MonsterModel monster = who.Monster;
				string text = null;
				if (monster is Crusher)
				{
					text = "%ArmBoneL";
				}
				else if (monster is Rocket)
				{
					text = "%ArmBoneR";
				}
				NCombatBackground background = combatRoom.Background;
				if (text == null || !FxKit.Alive((GodotObject)(object)background))
				{
					return;
				}
				Node2D nodeOrNull = ((Node)background).GetNodeOrNull<Node2D>(NodePath.op_Implicit("%KaiserCrab"));
				if (!FxKit.Alive((GodotObject)(object)nodeOrNull))
				{
					return;
				}
				if (Tuning.F("murder.claw_stop", 1f) > 0.5f)
				{
					HoldCrab((Node)(object)holder, ((Node)nodeOrNull).GetNodeOrNull<Node2D>(NodePath.op_Implicit("%Visuals")), blow.Stop, blow);
				}
				float num2 = Tuning.F("murder.claw_knock", 1.4f);
				Node2D val = ((num2 > 0.01f) ? ((Node)nodeOrNull).GetNodeOrNull<Node2D>(NodePath.op_Implicit(text)) : null);
				if (FxKit.Alive((GodotObject)(object)val))
				{
					blow.Picture = val;
					blow.PushA *= num2;
					blow.PushB *= num2;
				}
			}
			if (blow.Picture != null)
			{
				blow.Home = blow.Picture.Position;
				blow.Wrote = blow.Home;
			}
		}

		private static void Step(Blow blow, float t)
		{
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			if (blow.Over)
			{
				return;
			}
			if (!MultiplayerFx.Active)
			{
				RetireOwned();
			}
			try
			{
				if (blow.Owned && blow.VictimNode != null)
				{
					NCombatRoom combatRoom = GameRefs.CombatRoom;
					NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(blow.Who) : null);
					if ((object)combatRoom != blow.Room || (object)val != blow.VictimNode || !FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree() || val.Body != blow.VictimBody)
					{
						LetGo(blow);
						return;
					}
				}
				if (blow.Who != null && !blow.Who.IsAlive)
				{
					LetGo(blow);
					return;
				}
				float num = ((t < blow.Stop) ? (1f - 0.2f * (t / blow.Stop)) : (0.8f * Mathf.Clamp(1f - (t - blow.Stop) / 0.09f, 0f, 1f)));
				Light(blow, 1f + (blow.Glare - 1f) * num);
				Push(blow, blow.PushA * Spring(t - blow.Stop) + blow.PushB * Spring(t - blow.Stop - blow.Beat));
			}
			catch (Exception ex)
			{
				DevLog.Warn("Murder (victim): " + ex.Message);
				LetGo(blow);
			}
		}

		private static float Spring(float tau)
		{
			if (tau <= 0f)
			{
				return 0f;
			}
			float num = (float)Math.PI * 2f / Mathf.Clamp(Tuning.F("murder.spring", 0.3f), 0.12f, 1f);
			float num2 = 0.42f * num / Mathf.Sqrt(0.8236f);
			float num3 = Mathf.Atan(num / num2) / num;
			float num4 = Mathf.Exp((0f - num2) * num3) * Mathf.Sin(num * num3);
			return Mathf.Exp((0f - num2) * tau) * Mathf.Sin(num * tau) / num4;
		}

		private static void Push(Blow blow, Vector2 screen)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			Node2D picture = blow.Picture;
			if (picture == null)
			{
				return;
			}
			if (!FxKit.Alive((GodotObject)(object)picture))
			{
				blow.Picture = null;
				return;
			}
			Vector2 val = Vector2.Zero;
			if (screen != Vector2.Zero && ((Node)picture).IsInsideTree())
			{
				Node parent = ((Node)picture).GetParent();
				CanvasItem val2 = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
				if (val2 != null && FxKit.Alive((GodotObject)(object)val2))
				{
					Transform2D val3 = val2.GetGlobalTransform();
					val3 = val3.AffineInverse();
					val = val3.BasisXform(screen);
					if (!float.IsFinite(val.X) || !float.IsFinite(val.Y) || val.LengthSquared() > 160000f)
					{
						val = Vector2.Zero;
					}
				}
			}
			if (!(val == blow.Shift))
			{
				Vector2 position = picture.Position;
				if (position != blow.Wrote)
				{
					blow.Home = position - blow.Shift;
				}
				Vector2 wrote = (picture.Position = ((val == Vector2.Zero) ? blow.Home : (blow.Home + val)));
				blow.Wrote = wrote;
				blow.Shift = val;
			}
		}

		private static void Light(Blow blow, float gain)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			if (blow.Owned)
			{
				LightGroup lightGroup = blow.LightGroup;
				if (lightGroup != null && !lightGroup.Retired)
				{
					if (!MultiplayerFx.Active)
					{
						RetireOwned();
						return;
					}
					lightGroup.Gains[blow] = gain;
					WriteLight(lightGroup);
				}
				return;
			}
			CanvasItem lit = blow.Lit;
			if (lit == null)
			{
				return;
			}
			if (!FxKit.Alive((GodotObject)(object)lit) || lit.Modulate != blow.LitWrote)
			{
				blow.Lit = null;
				return;
			}
			Color litHome = blow.LitHome;
			Color val = (Color)((gain <= 1.001f) ? litHome : new Color(litHome.R * gain, litHome.G * gain, litHome.B * gain, litHome.A));
			if (!(val == blow.LitWrote))
			{
				lit.Modulate = val;
				blow.LitWrote = val;
			}
		}

		private static void LetGo(Blow blow)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (blow.Over)
			{
				return;
			}
			blow.Over = true;
			Live.Remove(blow);
			try
			{
				Push(blow, Vector2.Zero);
			}
			catch
			{
			}
			if (blow.Owned)
			{
				CreatureHold.Release(blow.Who, blow);
				try
				{
					blow.ReleaseCrabClaim?.Invoke();
				}
				catch (Exception ex)
				{
					DevLog.Warn("Murder (shared crab let go): " + ex.Message);
				}
				try
				{
					LeaveLight(blow);
				}
				catch (Exception ex2)
				{
					DevLog.Warn("Murder (shared light let go): " + ex2.Message);
				}
			}
			try
			{
				if (!blow.Owned)
				{
					Light(blow, 1f);
				}
			}
			catch
			{
			}
			blow.Picture = null;
			blow.Lit = null;
		}

		private static void JoinLight(Blow blow, CanvasItem body)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if (Lights.TryGetValue(body, out var value) && (!FxKit.Alive((GodotObject)(object)body) || body.Modulate != value.Wrote))
			{
				RetireLight(value, restore: false);
				value = null;
			}
			if (value == null)
			{
				value = new LightGroup
				{
					Body = body,
					Home = body.Modulate,
					Wrote = body.Modulate
				};
				Lights[body] = value;
			}
			blow.LightGroup = value;
			value.Gains[blow] = 1f;
		}

		private static void RetireLegacyLights()
		{
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				Blow blow = Live[num];
				if (!blow.Owned && blow.Lit != null)
				{
					try
					{
						Light(blow, 1f);
					}
					catch
					{
					}
					blow.Lit = null;
				}
			}
		}

		private static void WriteLight(LightGroup group)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			if (group.Retired)
			{
				return;
			}
			CanvasItem body = group.Body;
			if (!FxKit.Alive((GodotObject)(object)body) || body.Modulate != group.Wrote)
			{
				RetireLight(group, restore: false);
				return;
			}
			float num = 1f;
			foreach (float value in group.Gains.Values)
			{
				num = Mathf.Max(num, value);
			}
			Color home = group.Home;
			Color val = (Color)((num <= 1.001f) ? home : new Color(home.R * num, home.G * num, home.B * num, home.A));
			if (!(val == group.Wrote))
			{
				body.Modulate = val;
				group.Wrote = val;
			}
		}

		private static void RetireLight(LightGroup group, bool restore)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			if (!group.Retired)
			{
				group.Retired = true;
				if (restore && FxKit.Alive((GodotObject)(object)group.Body) && group.Body.Modulate == group.Wrote)
				{
					group.Body.Modulate = group.Home;
				}
				if (Lights.TryGetValue(group.Body, out var value) && value == group)
				{
					Lights.Remove(group.Body);
				}
				group.Gains.Clear();
			}
		}

		private static void LeaveLight(Blow blow)
		{
			LightGroup lightGroup = blow.LightGroup;
			blow.LightGroup = null;
			if (lightGroup != null && !lightGroup.Retired)
			{
				lightGroup.Gains.Remove(blow);
				if (lightGroup.Gains.Count == 0)
				{
					RetireLight(lightGroup, restore: true);
				}
				else
				{
					WriteLight(lightGroup);
				}
			}
		}

		private static void RetireOwned()
		{
			if (Lights.Count == 0 && Crabs.Count == 0)
			{
				return;
			}
			foreach (LightGroup item in new List<LightGroup>(Lights.Values))
			{
				RetireLight(item, restore: true);
			}
			foreach (CrabGroup item2 in new List<CrabGroup>(Crabs.Values))
			{
				RetireCrab(item2);
			}
		}

		private static void RetireCrab(CrabGroup group)
		{
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected Obj, but got Unknown
			if (group.Retired)
			{
				return;
			}
			group.Retired = true;
			group.Claims.Clear();
			if (Crabs.TryGetValue(group.Sprite, out var value) && value == group)
			{
				Crabs.Remove(group.Sprite);
			}
			if ((_crab != group.Sprite || _crabHolds <= 0) && FxKit.Alive((GodotObject)(object)group.Sprite))
			{
				MegaSprite val = new MegaSprite(Variant.op_Implicit((GodotObject)(object)group.Sprite));
				if (val.IsAnimationStateReady())
				{
					val.GetAnimationState().SetTimeScale(1f);
				}
			}
		}

		private static void HoldCrab(Node owner, Node2D sprite, float seconds, Blow blow = null)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected Obj, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected Obj, but got Unknown
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)sprite) || !FxKit.Alive((GodotObject)(object)owner) || !owner.IsInsideTree() || seconds <= 0.001f)
			{
				return;
			}
			MegaSprite val = new MegaSprite(Variant.op_Implicit((GodotObject)(object)sprite));
			if (!val.IsAnimationStateReady())
			{
				return;
			}
			Node timer = new Node
			{
				Name = StringName.op_Implicit("SilentFX_MurderHold")
			};
			owner.AddChild(timer, false, (InternalMode)0);
			if (!timer.IsInsideTree())
			{
				timer.QueueFree();
				return;
			}
			CrabGroup group;
			object claim;
			bool releasedClaim;
			if (MultiplayerFx.Active && blow != null && blow.Owned)
			{
				if (!Crabs.TryGetValue(sprite, out group))
				{
					group = new CrabGroup
					{
						Sprite = sprite
					};
					Crabs[sprite] = group;
				}
				claim = new object();
				group.Claims.Add(claim);
				releasedClaim = false;
				blow.ReleaseCrabClaim = LetClaim;
				timer.TreeExiting += LetClaim;
				Tween val2 = timer.CreateTween();
				val2.TweenInterval((double)seconds);
				val2.TweenCallback(Callable.From((Action)(() =>
				{
					LetClaim();
					if (FxKit.Alive((GodotObject)(object)timer))
					{
						timer.QueueFree();
					}
				})));
				FxLifetime.Watch(timer, seconds, LetClaim);
				val.GetAnimationState().SetTimeScale(0f);
				return;
			}
			_crab = sprite;
			_crabHolds++;
			bool released = false;
			timer.TreeExiting += Let;
			Tween val3 = timer.CreateTween();
			val3.TweenInterval((double)seconds);
			val3.TweenCallback(Callable.From((Action)(() =>
			{
				Let();
				if (FxKit.Alive((GodotObject)(object)timer))
				{
					timer.QueueFree();
				}
			})));
			val.GetAnimationState().SetTimeScale(0f);
			void Let()
			{
				if (!released)
				{
					released = true;
					_crabHolds = Math.Max(0, _crabHolds - 1);
					if (_crabHolds == 0)
					{
						ReleaseCrab();
					}
				}
			}
			void LetClaim()
			{
				if (!releasedClaim)
				{
					releasedClaim = true;
					if (!group.Retired)
					{
						group.Claims.Remove(claim);
						if (group.Claims.Count == 0)
						{
							RetireCrab(group);
						}
					}
				}
			}
		}

		private static void ReleaseCrab()
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected Obj, but got Unknown
			Node2D crab = _crab;
			_crab = null;
			_crabHolds = 0;
			try
			{
				if (FxKit.Alive((GodotObject)(object)crab) && (!MultiplayerFx.Active || !Crabs.TryGetValue(crab, out var value) || value.Retired || value.Claims.Count <= 0))
				{
					MegaSprite val = new MegaSprite(Variant.op_Implicit((GodotObject)(object)crab));
					if (val.IsAnimationStateReady())
					{
						val.GetAnimationState().SetTimeScale(1f);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Murder (crab let go): " + ex.Message);
			}
		}

		private static void Wash(Node parent, Vector2 at, float w, float h, float stop)
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Clamp(Tuning.F("murder.wash", 0.85f), 0f, 1f);
			if (num <= 0.01f)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				float num2 = ((i == 0) ? 1.3f : 0.8f);
				Sprite2D wash = FxKit.Glow(new Color(1f, 1f, 1f, num * ((i == 0) ? 0.75f : 1f)), new Vector2(w * num2, h * num2));
				parent.AddChild((Node)(object)wash, false, (InternalMode)0);
				((Node2D)wash).GlobalPosition = at;
				Tween val = ((Node)wash).CreateTween();
				val.TweenInterval((double)(stop * 0.75f));
				val.TweenProperty((GodotObject)(object)wash, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), (double)(stop * 0.25f + 0.06f)).SetTrans((TransitionType)4).SetEase((EaseType)0);
				val.TweenCallback(Callable.From((Action)(() =>
				{
					if (FxKit.Alive((GodotObject)(object)wash))
					{
						((Node)wash).QueueFree();
					}
				})));
			}
		}

		private static void Slash(Node2D holder, Vector2 from, Vector2 to, float bulge, float width, float open)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected Obj, but got Unknown
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			Node2D cut = new Node2D();
			((Node)holder).AddChild((Node)(object)cut, false, (InternalMode)0);
			float hair = Mathf.Clamp(width * 0.06f, 3f, 6f);
			Line2D red = FxKit.Crescent(from, to, bulge, hair, new Color(Blood.R, Blood.G, Blood.B, 0.95f));
			Line2D core = FxKit.Crescent(from, to, bulge, hair, new Color(1f, 0.92f, 0.9f, 1f));
			((CanvasItem)red).Visible = false;
			((Node)cut).AddChild((Node)(object)red, false, (InternalMode)0);
			((Node)cut).AddChild((Node)(object)core, false, (InternalMode)0);
			cut.Scale = new Vector2(0.35f, 0.35f);
			float opens = Mathf.Max(open, 0.035f);
			float total = opens + 0.07f + 0.05f + 0.32f;
			FxKit.Drive((Node)(object)cut, total, (float u) =>
			{
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)cut))
				{
					float num = u * total;
					float num2 = Mathf.Clamp(num / 0.035f, 0f, 1f);
					float num3 = Mathf.Clamp((num - opens) / 0.07f, 0f, 1f);
					float num4 = Mathf.Clamp((num - opens - 0.07f - 0.05f) / 0.32f, 0f, 1f);
					float num5 = BackOut(num3);
					float num6 = (0.35f + 0.65f * (1f - (1f - num2) * (1f - num2))) * (1f + 0.06f * Mathf.Min(1f, num3 * 1.5f));
					cut.Scale = new Vector2(num6, num6);
					if (FxKit.Alive((GodotObject)(object)red))
					{
						((CanvasItem)red).Visible = num3 > 0f;
						red.Width = Mathf.Max(hair, width * num5);
					}
					if (FxKit.Alive((GodotObject)(object)core))
					{
						core.Width = Mathf.Lerp(hair, width * 0.3f, num5) * (1f - 0.6f * num4);
					}
					((CanvasItem)cut).Modulate = new Color(1f, 1f, 1f, 1f - num4 * num4);
				}
			}, (TransitionType)0, (EaseType)2);
		}

		private static void Open(Node parent, Node2D holder, Vector2 at, float size, Vector2 dir, float w, bool remote = false)
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)parent))
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					Sprite2D val = FxKit.Glow(new Color(1f, 0.85f, 0.85f, 0.9f), new Vector2(size * 1.3f, size * 1.3f));
					((Node)holder).AddChild((Node)(object)val, false, (InternalMode)0);
					((Node)val).CreateTween().TweenProperty((GodotObject)(object)val, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.2);
				}
				Vector2 val2 = new Vector2(dir.Y, 0f - dir.X);
				if (val2.Y > 0f)
				{
					val2 = -val2;
				}
				Color color = new Color(1f, 0.3f, 0.3f, 0.95f);
				int amount = Tuning.I("murder.spray", 22);
				Vector2 gravity = new Vector2(0f, 560f);
				Vector2 val3 = dir + val2 * 0.45f;
				FxKit.Burst(parent, at, color, amount, 380f, 980f, 0.36, 0.07f, 0.17f, gravity, 20f, val3.Normalized(), size * 0.14f);
				FxKit.Burst(parent, at, new Color(1f, 0.16f, 0.2f, 0.95f), Tuning.I("murder.shards", 34), 200f, 560f, 0.7, 0.14f, 0.42f, new Vector2(0f, 620f));
				Vector2 val4 = new Vector2(0f - dir.Y, dir.X);
				float num = w * 0.55f + 70f;
				Color color2 = new Color(1f, 0.62f, 0.58f, 0.7f);
				for (int i = -1; i <= 1; i++)
				{
					float num2 = Mathf.Abs(i);
					Vector2 val5 = at + val4 * ((float)i * w * 0.16f) - dir * (w * 0.1f);
					Brush.Dart(parent, val5, val5 + dir * num, num * 0.7f, 13f - 3f * num2, color2, 0.11f + 0.02f * num2, 0.012f * (float)(i + 1));
				}
				Punch(dir, Tuning.I("murder.shake", 2), remote);
			}
		}

		private static void Cross(Node parent, Node2D holder, Vector2 at, float size, Vector2 dir, bool remote = false)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)parent) && FxKit.Alive((GodotObject)(object)holder))
			{
				Slash(holder, new Vector2(size * 0.8f, (0f - size) * 0.55f), new Vector2((0f - size) * 0.8f, size * 0.5f), (0f - size) * 0.22f, size * 0.14f, 0f);
				FxKit.Burst(parent, at, new Color(1f, 0.3f, 0.3f, 0.95f), 12, 300f, 760f, 0.3, 0.06f, 0.14f, new Vector2(0f, 560f), 22f, dir, size * 0.1f);
				Punch(dir, Mathf.Min(1, Tuning.I("murder.shake", 2)), remote);
			}
		}

		private static void Punch(Vector2 dir, int strength, bool remote = false)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!remote && strength > 0 && !(Tuning.F("fx.screen_shake", 1f) < 0.5f))
				{
					NGame instance = NGame.Instance;
					if (FxKit.Alive((GodotObject)(object)instance) && GameRefs.CombatRoom != null)
					{
						float num = Mathf.PosMod(dir.Angle() * 180f / (float)Math.PI, 360f);
						ShakeStrength val = (ShakeStrength)((strength >= 3) ? 4 : ((strength == 2) ? 3 : 2));
						instance.ScreenShake(val, (ShakeDuration)1, num);
					}
				}
			}
			catch
			{
			}
		}

		private static float BackOut(float x)
		{
			float num = x - 1f;
			return 1f + 2.70158f * num * num * num + 1.70158f * num * num;
		}

		static MurderFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
