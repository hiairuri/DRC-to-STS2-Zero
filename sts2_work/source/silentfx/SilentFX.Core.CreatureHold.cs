// namespace: SilentFX.Core
// type: CreatureHold (class)
// source lines: 34006-34433
	public static class CreatureHold
	{
		private sealed class HoldState
		{
			public Creature Creature;

			public NCreature Node;

			public Node2D Body;

			public bool Spine;

			public ProcessModeEnum Mode;

			public float Remaining;

			public Dictionary<object, float> Tokens;

			public List<object> TokenKeys;
		}

		private static readonly List<HoldState> Holds = new List<HoldState>();

		private static readonly Dictionary<NCreature, float> BulletSpeeds = new Dictionary<NCreature, float>(ReferenceEqualityComparer.Instance);

		private static readonly Action<double> Tick = Update;

		private static bool _ticking;

		private static bool _multiplayer;

		public static void Hold(Creature creature, float maxSeconds = 3f)
		{
			HoldClaim(creature, maxSeconds, null);
		}

		public static void Hold(Creature creature, float maxSeconds, object token)
		{
			HoldClaim(creature, maxSeconds, token);
		}

		private static void HoldClaim(Creature creature, float maxSeconds, object token)
		{
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if ((!SyncMode() && token != null) || creature == null)
				{
					return;
				}
				if (!creature.IsAlive || !float.IsFinite(maxSeconds) || maxSeconds <= 0f)
				{
					if (!creature.IsAlive)
					{
						DropCreature(creature);
					}
					else
					{
						Release(creature, token);
					}
					return;
				}
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(creature) : null);
				if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree())
				{
					return;
				}
				for (int num = Holds.Count - 1; num >= 0; num--)
				{
					HoldState holdState = Holds[num];
					if (holdState.Creature == creature)
					{
						if (holdState.Node == val && holdState.Body == val.Body)
						{
							Claim(holdState, maxSeconds, token);
							Freeze(holdState);
							return;
						}
						Drop(num);
					}
				}
				Node2D body = val.Body;
				if (!FxKit.Alive((GodotObject)(object)body))
				{
					return;
				}
				bool hasSpineAnimation = val.HasSpineAnimation;
				if (!hasSpineAnimation || SpineReady(val))
				{
					HoldState holdState2 = new HoldState
					{
						Creature = creature,
						Node = val,
						Body = body,
						Spine = hasSpineAnimation,
						Mode = ((Node)body).ProcessMode
					};
					Claim(holdState2, maxSeconds, token);
					if (!_ticking)
					{
						Ticker.Add(Tick);
						_ticking = true;
					}
					Holds.Add(holdState2);
					Freeze(holdState2);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("CreatureHold.Hold: " + ex);
				Release(creature, token);
			}
		}

		public static void Release(Creature creature)
		{
			Release(creature, null);
		}

		public static void Release(Creature creature, object token)
		{
			try
			{
				bool flag = SyncMode();
				if (creature == null || (!flag && token != null))
				{
					return;
				}
				for (int num = Holds.Count - 1; num >= 0; num--)
				{
					HoldState holdState = Holds[num];
					if (holdState.Creature == creature)
					{
						if (!flag)
						{
							Drop(num);
						}
						else
						{
							if (token == null)
							{
								holdState.Remaining = 0f;
							}
							else
							{
								holdState.Tokens?.Remove(token);
							}
							if (!HasClaims(holdState) || !Valid(holdState))
							{
								Drop(num);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("CreatureHold.Release: " + ex);
			}
		}

		public static bool Held(Creature creature)
		{
			try
			{
				SyncMode();
				if (creature == null)
				{
					return false;
				}
				for (int num = Holds.Count - 1; num >= 0; num--)
				{
					HoldState holdState = Holds[num];
					if (holdState.Creature == creature)
					{
						if (Valid(holdState) && HasClaims(holdState))
						{
							return true;
						}
						Drop(num);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("CreatureHold.Held: " + ex);
				Release(creature);
			}
			return false;
		}

		private static void Claim(HoldState held, float seconds, object token)
		{
			if (token == null)
			{
				held.Remaining = Math.Max(held.Remaining, seconds);
				return;
			}
			HoldState holdState = held;
			if (holdState.Tokens == null)
			{
				holdState.Tokens = new Dictionary<object, float>(ReferenceEqualityComparer.Instance);
			}
			holdState = held;
			if (holdState.TokenKeys == null)
			{
				holdState.TokenKeys = new List<object>();
			}
			held.Tokens[token] = Math.Max(held.Tokens.GetValueOrDefault(token), seconds);
		}

		private static bool HasClaims(HoldState held)
		{
			if (!(held.Remaining > 0f))
			{
				Dictionary<object, float> tokens = held.Tokens;
				if (tokens == null)
				{
					return false;
				}
				return tokens.Count > 0;
			}
			return true;
		}

		private static void DropCreature(Creature creature)
		{
			for (int num = Holds.Count - 1; num >= 0; num--)
			{
				if (Holds[num].Creature == creature)
				{
					Drop(num);
				}
			}
		}

		private static bool SyncMode()
		{
			bool active = MultiplayerFx.Active;
			if (_multiplayer != active)
			{
				Reset();
				_multiplayer = active;
			}
			return active;
		}

		public static void Reset()
		{
			try
			{
				for (int num = Holds.Count - 1; num >= 0; num--)
				{
					Drop(num);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("CreatureHold.Reset: " + ex);
			}
		}

		private static bool SpineReady(NCreature node)
		{
			NCreatureVisuals visuals = node.Visuals;
			MegaSprite val = ((visuals != null) ? visuals.SpineBody : null);
			if (val != null)
			{
				return val.IsAnimationStateReady();
			}
			return false;
		}

		private static bool Valid(HoldState held)
		{
			if (!held.Creature.IsAlive || !FxKit.Alive((GodotObject)(object)held.Node) || !((Node)held.Node).IsInsideTree() || !FxKit.Alive((GodotObject)(object)held.Body) || !((Node)held.Body).IsInsideTree())
			{
				return false;
			}
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			if (combatRoom != null && combatRoom.GetCreatureNode(held.Creature) == held.Node)
			{
				return held.Node.Body == held.Body;
			}
			return false;
		}

		private static void Freeze(HoldState held)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			if (held.Spine)
			{
				if (SpineReady(held.Node))
				{
					SpineAnimationAccess spineAnimation = held.Node.SpineAnimation;
					spineAnimation.SetTimeScale(0f);
				}
			}
			else
			{
				((Node)held.Body).ProcessMode = (ProcessModeEnum)4;
			}
		}

		private static void Drop(int index)
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			HoldState holdState = Holds[index];
			Holds.RemoveAt(index);
			try
			{
				if (holdState.Spine)
				{
					if (FxKit.Alive((GodotObject)(object)holdState.Node) && holdState.Node.HasSpineAnimation && SpineReady(holdState.Node))
					{
						SpineAnimationAccess spineAnimation = holdState.Node.SpineAnimation;
						spineAnimation.SetTimeScale(BulletSpeeds.TryGetValue(holdState.Node, out var value) ? value : 1f);
					}
				}
				else if (FxKit.Alive((GodotObject)(object)holdState.Body))
				{
					((Node)holdState.Body).ProcessMode = holdState.Mode;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("CreatureHold restore: " + ex);
			}
		}

		private static void Update(double realDt)
		{
			try
			{
				SyncMode();
				if (Holds.Count == 0)
				{
					return;
				}
				float num = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
				if (!float.IsFinite(num) || num < 0f)
				{
					Reset();
					return;
				}
				for (int num2 = Holds.Count - 1; num2 >= 0; num2--)
				{
					HoldState holdState = Holds[num2];
					holdState.Remaining -= num;
					Dictionary<object, float> tokens = holdState.Tokens;
					if (tokens != null && tokens.Count > 0)
					{
						holdState.TokenKeys.Clear();
						holdState.TokenKeys.AddRange(holdState.Tokens.Keys);
						foreach (object tokenKey in holdState.TokenKeys)
						{
							float num3 = holdState.Tokens[tokenKey] - num;
							if (num3 <= 0f)
							{
								holdState.Tokens.Remove(tokenKey);
							}
							else
							{
								holdState.Tokens[tokenKey] = num3;
							}
						}
						holdState.TokenKeys.Clear();
					}
					if (!HasClaims(holdState) || !Valid(holdState))
					{
						Drop(num2);
					}
					else
					{
						Freeze(holdState);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("CreatureHold.Update: " + ex);
				Reset();
			}
		}

		internal static void AfterBulletSlow(List<NCreature> slowed)
		{
			try
			{
				if (slowed == null)
				{
					return;
				}
				float value = Mathf.Clamp(Tuning.F("bullet.enemy_speed", 0.04f), 0f, 1f);
				for (int i = 0; i < slowed.Count; i++)
				{
					NCreature val = slowed[i];
					if (FxKit.Alive((GodotObject)(object)val) && !BulletSpeeds.ContainsKey(val))
					{
						BulletSpeeds.Add(val, value);
					}
				}
				Update(0.0);
			}
			catch (Exception ex)
			{
				DevLog.Warn("CreatureHold.AfterBulletSlow: " + ex);
			}
		}

		internal static void AfterBulletRestore()
		{
			try
			{
				BulletSpeeds.Clear();
				Update(0.0);
			}
			catch (Exception ex)
			{
				DevLog.Warn("CreatureHold.AfterBulletRestore: " + ex);
			}
		}
	}
