// namespace: SilentFX.Cards
// type: AssassinateFx (class)
// source lines: 45694-46097
	public sealed class AssassinateFx : CardFx
	{
		private sealed class ActiveRun
		{
			internal CardModel Card;

			internal Creature Caster;

			internal Node CasterNode;

			internal Node Room;

			internal ulong Since;
		}

		internal static readonly Color Crimson = new Color(1f, 0.2f, 0.24f, 1f);

		private static CardModel _active;

		private static ulong _activeSince;

		private static Node _activeRoom;

		private static readonly Dictionary<object, ActiveRun> ActiveRuns = new Dictionary<object, ActiveRun>(ReferenceEqualityComparer.Instance);

		public static void Reset()
		{
			_active = null;
			_activeRoom = null;
			_activeSince = 0uL;
			ActiveRuns.Clear();
		}

		internal static Vector2 Behind(Vector2 chest, float width, float height)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return Behind(chest, width, height, 1f);
		}

		internal static Vector2 Behind(Vector2 chest, float width, float height, float side)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			return chest + new Vector2((width * 0.5f + Tuning.F("assassinate.behind", 46f)) * side, (0f - height) * 0.06f);
		}

		internal static Vector2 CloudSize(float width, float height)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			float num = Tuning.F("assassinate.cloud", 1.25f);
			return new Vector2(Mathf.Max(width * 0.95f, 250f), Mathf.Max(height * 1.15f, 270f)) * num;
		}

		public static bool Replaces(Creature creature)
		{
			if (creature == null)
			{
				return false;
			}
			object currentInvocation = MultiplayerFx.CurrentInvocation;
			if (currentInvocation != null)
			{
				PruneActiveRuns();
				if (ActiveRuns.TryGetValue(currentInvocation, out var value) && value.Card == MultiplayerFx.CurrentCard && (object)value.Room == GameRefs.CombatRoom && value.Caster == creature)
				{
					CardModel card = value.Card;
					object obj;
					if (card == null)
					{
						obj = null;
					}
					else
					{
						Player owner = card.Owner;
						obj = ((owner != null) ? owner.Creature : null);
					}
					if (obj == value.Caster && FxKit.Alive((GodotObject)(object)value.CasterNode) && value.CasterNode.IsInsideTree() && FxKit.Alive((GodotObject)(object)value.Room) && value.Room.IsInsideTree())
					{
						NCombatRoom combatRoom = GameRefs.CombatRoom;
						if ((object)((combatRoom != null) ? combatRoom.GetCreatureNode(creature) : null) == value.CasterNode && creature.IsAlive)
						{
							return Tuning.F("assassinate.keep_swing", 0f) < 0.5f;
						}
					}
				}
				return false;
			}
			if (MultiplayerFx.Active)
			{
				return false;
			}
			if (_active == null)
			{
				return false;
			}
			if ((object)_activeRoom != GameRefs.CombatRoom || FxClock.Milliseconds - _activeSince > 4000)
			{
				_active = null;
				_activeRoom = null;
				return false;
			}
			if (Tuning.F("assassinate.keep_swing", 0f) < 0.5f)
			{
				Player owner2 = _active.Owner;
				return ((owner2 != null) ? owner2.Creature : null) == creature;
			}
			return false;
		}

		public static void NoteCardFinished(CardModel card)
		{
			if (card == null)
			{
				return;
			}
			if (MultiplayerFx.CurrentInvocation != null)
			{
				if (ActiveRuns.TryGetValue(MultiplayerFx.CurrentInvocation, out var value) && value.Card == card)
				{
					ActiveRuns.Remove(MultiplayerFx.CurrentInvocation);
				}
			}
			else if (!MultiplayerFx.Active && card == _active)
			{
				_active = null;
			}
		}

		public override void OnPlay(CardFxContext c)
		{
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node parent = c.FrontParent;
				if (!c.HasTarget || !FxKit.Alive((GodotObject)(object)parent))
				{
					return;
				}
				object currentInvocation = MultiplayerFx.CurrentInvocation;
				PruneActiveRuns();
				if (MultiplayerFx.Active)
				{
					CardModel card = c.Card;
					object obj;
					if (card == null)
					{
						obj = null;
					}
					else
					{
						Player owner = card.Owner;
						obj = ((owner != null) ? owner.Creature : null);
					}
					Creature val = (Creature)obj;
					NCombatRoom combatRoom = GameRefs.CombatRoom;
					if (currentInvocation != null && MultiplayerFx.CurrentCard == c.Card && val != null && FxKit.Alive((GodotObject)(object)combatRoom) && !ActiveRuns.ContainsKey(currentInvocation))
					{
						ActiveRuns.Add(currentInvocation, new ActiveRun
						{
							Card = c.Card,
							Caster = val,
							CasterNode = (Node)(object)combatRoom.GetCreatureNode(val),
							Room = (Node)(object)combatRoom,
							Since = FxClock.Milliseconds
						});
					}
				}
				else if (!c.Remote && currentInvocation == null)
				{
					_active = c.Card;
					_activeSince = FxClock.Milliseconds;
					_activeRoom = (Node)(object)GameRefs.CombatRoom;
				}
				float targetSide = c.TargetSide;
				bool flag = c.Held is AssassinateHeld;
				Vector2 at = ((c.Held is IHeldOrigin heldOrigin) ? heldOrigin.Origin : Behind(c.TargetChest, c.TargetWidth, c.TargetHeight, targetSide));
				Node val2 = (FxKit.Alive((GodotObject)(object)c.BackParent) ? c.BackParent : parent);
				Vector2 val3 = CloudSize(c.TargetWidth, c.TargetHeight);
				if (!flag)
				{
					Shade.Puff(val2, at, val3.Y * 0.75f, 1.5f, 0.5f, 0.95f, 0.4f);
				}
				Vector2 val4 = c.TargetChest + new Vector2((0f - (c.TargetWidth * 0.5f + 90f)) * targetSide, 0f);
				for (int i = 0; i < 6; i++)
				{
					float num = (float)i / 5f;
					Vector2 at2 = at.Lerp(val4, num);
					float size = val3.Y * Mathf.Lerp(0.55f, 0.3f, num);
					FxKit.After(parent, 0.014f * (float)i, () =>
					{
						//IL_000c: Unknown result type (might be due to invalid IL or missing references)
						Shade.Puff(parent, at2, size, 1.7f, 0.36f, 0.85f, 0.35f);
					});
				}
				ColorRect veil = FxKit.Veil(new Color(0.01f, 0f, 0.02f, 0f));
				val2.AddChild((Node)(object)veil, false, (InternalMode)0);
				FxKit.FreeAfter((Node)(object)veil, 0.6800000071525574);
				if (!((Node)veil).IsInsideTree())
				{
					((Node)veil).QueueFree();
					return;
				}
				Tween val5 = ((Node)veil).CreateTween();
				val5.TweenProperty((GodotObject)(object)veil, NodePath.op_Implicit("color:a"), Variant.op_Implicit(Tuning.F("assassinate.hush", 0.42f)), 0.06);
				val5.TweenInterval(0.22);
				val5.TweenProperty((GodotObject)(object)veil, NodePath.op_Implicit("color:a"), Variant.op_Implicit(0f), 0.3);
				val5.TweenCallback(Callable.From((Action)(() =>
				{
					if (FxKit.Alive((GodotObject)(object)veil))
					{
						((Node)veil).QueueFree();
					}
				})));
			}
			catch (Exception ex)
			{
				DevLog.Warn("AssassinateFx.OnPlay: " + ex);
			}
		}

		private static void PruneActiveRuns()
		{
			if (ActiveRuns.Count == 0)
			{
				return;
			}
			if (!MultiplayerFx.Active)
			{
				ActiveRuns.Clear();
				return;
			}
			List<object> list = new List<object>();
			foreach (KeyValuePair<object, ActiveRun> activeRun in ActiveRuns)
			{
				if ((object)activeRun.Value.Room == GameRefs.CombatRoom && FxClock.Milliseconds - activeRun.Value.Since <= 4000 && FxKit.Alive((GodotObject)(object)activeRun.Value.Room) && activeRun.Value.Room.IsInsideTree() && activeRun.Value.Caster != null && activeRun.Value.Caster.IsAlive)
				{
					CardModel card = activeRun.Value.Card;
					object obj;
					if (card == null)
					{
						obj = null;
					}
					else
					{
						Player owner = card.Owner;
						obj = ((owner != null) ? owner.Creature : null);
					}
					if (obj == activeRun.Value.Caster && FxKit.Alive((GodotObject)(object)activeRun.Value.CasterNode) && activeRun.Value.CasterNode.IsInsideTree())
					{
						NCombatRoom combatRoom = GameRefs.CombatRoom;
						if ((object)((combatRoom != null) ? combatRoom.GetCreatureNode(activeRun.Value.Caster) : null) == activeRun.Value.CasterNode)
						{
							continue;
						}
					}
				}
				list.Add(activeRun.Key);
			}
			foreach (object item in list)
			{
				ActiveRuns.Remove(item);
			}
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Expected Obj, but got Unknown
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0365: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Node frontParent = c.FrontParent;
				if (!FxKit.Alive((GodotObject)(object)frontParent))
				{
					return;
				}
				Sfx.Play("assassin_pierce");
				if (!c.Remote)
				{
					Mood.Clear("assassinate");
				}
				float num = Tuning.F("assassinate.size", 1f) * Strokes.Scale(c);
				float targetSide = c.TargetSide;
				Node2D val = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_AssassinateHit")
				};
				frontParent.AddChild((Node)(object)val, false, (InternalMode)0);
				if (!((Node)val).IsInsideTree())
				{
					((Node)val).QueueFree();
					return;
				}
				val.GlobalPosition = at;
				Vector2 val2 = new Vector2(0f - targetSide, 0.1f);
				val.Rotation = val2.Angle();
				float x0 = 0f - (c.TargetWidth * 0.5f + 60f);
				float x1 = c.TargetWidth * 0.5f + 230f * num;
				Sprite2D outer = FxKit.Glow(new Color(Crimson.R, Crimson.G, Crimson.B, 0f), new Vector2(64f, 64f));
				Sprite2D core = FxKit.Glow(new Color(1f, 0.9f, 0.88f, 0f), new Vector2(64f, 64f));
				((Node)val).AddChild((Node)(object)outer, false, (InternalMode)0);
				((Node)val).AddChild((Node)(object)core, false, (InternalMode)0);
				float hold = Tuning.F("assassinate.hold", 0.1f);
				float total = 0.05f + hold + 0.28f;
				float thick = Tuning.F("assassinate.ray", 46f) * num;
				FxKit.Drive((Node)(object)val, total, (float u) =>
				{
					//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
					//IL_00de: Unknown result type (might be due to invalid IL or missing references)
					//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
					//IL_0109: Unknown result type (might be due to invalid IL or missing references)
					//IL_0113: Unknown result type (might be due to invalid IL or missing references)
					//IL_013a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0144: Unknown result type (might be due to invalid IL or missing references)
					//IL_017f: Unknown result type (might be due to invalid IL or missing references)
					//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
					if (FxKit.Alive((GodotObject)(object)outer) && FxKit.Alive((GodotObject)(object)core))
					{
						float num2 = u * total;
						float num3 = Mathf.Clamp(num2 / 0.05f, 0f, 1f);
						float num4 = Mathf.Clamp((num2 - 0.05f - hold) / 0.28f, 0f, 1f);
						float num5 = Mathf.Lerp(x0 + 40f, x1, 1f - (1f - num3) * (1f - num3));
						float num6 = Mathf.Lerp(x0, x1 - 60f, num4 * num4);
						float num7 = Mathf.Max(20f, num5 - num6);
						Vector2 position = new Vector2((num5 + num6) * 0.5f, 0f);
						float num8 = 1f - 0.75f * num4;
						((Node2D)outer).Position = position;
						((Node2D)core).Position = position;
						((Node2D)outer).Scale = new Vector2(num7 * 1.3f, thick * num8) / 64f;
						((Node2D)core).Scale = new Vector2(num7 * 1.15f, thick * 0.36f * num8) / 64f;
						((CanvasItem)outer).Modulate = new Color(Crimson.R, Crimson.G, Crimson.B, 0.95f * (1f - num4));
						((CanvasItem)core).Modulate = new Color(1f, 0.9f, 0.88f, 1f - num4);
					}
				}, (TransitionType)0, (EaseType)2);
				FxKit.FreeAfter((Node)(object)val, (double)total + 0.2);
				FxKit.Flash(frontParent, at, new Color(1f, 0.92f, 0.9f, 0.95f), 60f * num, 1.6f, 0.12);
				FxKit.Flash(frontParent, at, new Color(Crimson.R, Crimson.G, Crimson.B, 0.6f), 150f * num, 1.9f);
				Node parent = (FxKit.Alive((GodotObject)(object)c.BackParent) ? c.BackParent : frontParent);
				Vector2 val3 = CloudSize(c.TargetWidth, c.TargetHeight);
				Shade.Puff(parent, Behind(c.TargetChest, c.TargetWidth, c.TargetHeight, targetSide), val3.Y * 0.8f, 2f, 0.55f, 0.85f, 0.5f);
				Shade.Puff(frontParent, at + new Vector2((0f - targetSide) * c.TargetWidth * 0.3f, 0f), val3.Y * 0.45f, 2.2f, 0.45f, 0.7f, 0.4f);
				FxKit.Burst(frontParent, at + new Vector2((0f - targetSide) * c.TargetWidth * 0.25f, 0f), new Color(0.86f, 0.08f, 0.12f, 0.95f), 16, 160f, 480f, 0.55, 0.08f * num, 0.2f * num, new Vector2(0f, 700f), 30f, new Vector2(0f - targetSide, -0.3f), 6f * num, 0f, additive: false);
				GameRefs.Shake((!killed) ? 1 : 2);
			}
			catch (Exception ex)
			{
				DevLog.Warn("AssassinateFx.OnHit: " + ex);
			}
		}

		static AssassinateFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
