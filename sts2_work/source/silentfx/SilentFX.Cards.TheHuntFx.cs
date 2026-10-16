// namespace: SilentFX.Cards
// type: TheHuntFx (class)
// source lines: 112336-113101
	public sealed class TheHuntFx : CardFx
	{
		private sealed class HuntRecord
		{
			internal CardModel Card;

			internal object Invocation;

			internal Vector2 KillAt;

			internal ulong KillMsec;

			internal Node KillParent;

			internal float KillSize = 1f;

			internal Transform2D KillTransform = Transform2D.Identity;

			internal Creature KillTarget;

			internal bool Playing = true;

			internal bool HasKill;

			internal bool RewardPending;

			internal bool TrophyStarted;

			public HuntRecord()
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			}
		}

		private sealed class HuntOwner
		{
			internal int Plays;

			internal readonly Dictionary<object, HuntRecord> Records = new Dictionary<object, HuntRecord>(ReferenceEqualityComparer.Instance);
		}

		public static readonly Color Gold = new Color(1f, 0.82f, 0.32f, 1f);

		public static readonly Color Pale = new Color(0.82f, 0.85f, 0.88f, 1f);

		private static readonly Color Steel = new Color(0.9f, 0.96f, 1f, 1f);

		private static Vector2 _killAt;

		private static ulong _killMsec;

		private static Node _killParent;

		private static float _killSize = 1f;

		private static Transform2D _killTransform = Transform2D.Identity;

		private static CardModel _playingCard;

		private static Creature _killTarget;

		private static bool _hasKill;

		private static bool _rewardPending;

		private static bool _trophyStarted;

		private static readonly Dictionary<Creature, HuntOwner> Owned = new Dictionary<Creature, HuntOwner>(ReferenceEqualityComparer.Instance);

		private static HuntRecord CurrentRecord(CardModel card)
		{
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
			object currentInvocation = MultiplayerFx.CurrentInvocation;
			if (val == null || currentInvocation == null || card != MultiplayerFx.CurrentCard)
			{
				return null;
			}
			if (!Owned.TryGetValue(val, out var value) || !value.Records.TryGetValue(currentInvocation, out var value2))
			{
				return null;
			}
			if (value2.Card != card)
			{
				return null;
			}
			return value2;
		}

		private static void BeginOwned(CardModel card)
		{
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
			object currentInvocation = MultiplayerFx.CurrentInvocation;
			if (val == null || currentInvocation == null || card != MultiplayerFx.CurrentCard)
			{
				return;
			}
			if (!Owned.TryGetValue(val, out var value))
			{
				Owned.Add(val, value = new HuntOwner());
			}
			if (!value.Records.ContainsKey(currentInvocation))
			{
				ulong now = Time.GetTicksMsec();
				double span = 4000.0 / (double)Mathf.Clamp((float)Engine.TimeScale, 0.1f, 1f);
				object[] array = (from kv in value.Records
					where !kv.Value.Playing && (!kv.Value.HasKill || (double)(now - kv.Value.KillMsec) > span)
					select kv.Key).ToArray();
				foreach (object key in array)
				{
					value.Records.Remove(key);
				}
				value.Plays++;
				value.Records.Add(currentInvocation, new HuntRecord
				{
					Card = card,
					Invocation = currentInvocation
				});
			}
		}

		public static bool WouldKill(CardModel card, Creature target, out bool fatal)
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			fatal = false;
			try
			{
				if (card == null || target == null || !target.IsAlive)
				{
					return false;
				}
				Player owner = card.Owner;
				Creature val = ((owner != null) ? owner.Creature : null);
				ICombatState combatState = card.CombatState;
				if (val == null || combatState == null)
				{
					return false;
				}
				DamageVar damage = card.DynamicVars.Damage;
				decimal num = Math.Max(RuntimeApiCompatibility.PreviewDamage(owner.RunState, combatState, target, val, ((DynamicVar)damage).BaseValue, damage.Props, card) - (decimal)target.Block, 0m);
				IEnumerable<AbstractModel> enumerable = default;
				decimal num2 = Hook.ModifyHpLost(owner.RunState, combatState, target, num, damage.Props, val, card, (HpLossHookPhase)3, ref enumerable);
				bool flag = target.CurrentHp > 0 && num2 >= (decimal)target.CurrentHp;
				fatal = flag && target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
				return flag;
			}
			catch (Exception ex)
			{
				DevLog.Warn("The Hunt: could not work out the blow (" + ex.Message + ")");
				return false;
			}
		}

		public override void OnPlay(CardFxContext c)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
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
				if (obj != null)
				{
					BeginOwned(c.Card);
					goto IL_004b;
				}
			}
			if (MultiplayerFx.CurrentInvocation == null)
			{
				ResetLegacyTrophy();
				_playingCard = c.Card;
			}
			goto IL_004b;
			IL_004b:
			if (FxKit.Alive((GodotObject)(object)c.FrontParent))
			{
				FxKit.Flash(c.FrontParent, c.WeaponHand, new Color(Steel.R, Steel.G, Steel.B, 0.7f), 70f, 1.8f, 0.16);
			}
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Expected Obj, but got Unknown
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0446: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_053b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_0599: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0618: Unknown result type (might be due to invalid IL or missing references)
			//IL_0646: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			Node frontParent = c.FrontParent;
			float num = Mathf.Clamp(Mathf.Max(c.TargetWidth, c.TargetHeight) * Tuning.F("hunt.cut", 1.35f), 220f, 560f);
			if (killed)
			{
				try
				{
					HuntRecord huntRecord = CurrentRecord(c.Card);
					if (!MultiplayerFx.Active)
					{
						goto IL_0152;
					}
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
					if (obj == null)
					{
						goto IL_0152;
					}
					if (huntRecord != null && huntRecord.Playing && (!huntRecord.HasKill || huntRecord.KillTarget != c.Target))
					{
						huntRecord.KillAt = at;
						huntRecord.KillParent = frontParent;
						huntRecord.KillMsec = Time.GetTicksMsec();
						huntRecord.KillSize = Mathf.Clamp(num / 300f, 0.8f, 1.6f);
						CanvasItem val = (CanvasItem)(object)((frontParent is CanvasItem) ? frontParent : null);
						huntRecord.KillTransform = ((val != null && FxKit.Alive((GodotObject)(object)val) && ((Node)val).IsInsideTree()) ? val.GetGlobalTransform() : Transform2D.Identity);
						huntRecord.KillTarget = c.Target;
						huntRecord.HasKill = true;
						huntRecord.TrophyStarted = false;
						if (huntRecord.RewardPending)
						{
							StartTrophy("reward-before-hit", huntRecord);
						}
					}
					goto end_IL_0044;
					IL_0152:
					if (MultiplayerFx.CurrentInvocation == null && (!_hasKill || _killTarget != c.Target))
					{
						_killAt = at;
						_killParent = frontParent;
						_killMsec = Time.GetTicksMsec();
						_killSize = Mathf.Clamp(num / 300f, 0.8f, 1.6f);
						CanvasItem val2 = (CanvasItem)(object)((frontParent is CanvasItem) ? frontParent : null);
						_killTransform = ((val2 != null && FxKit.Alive((GodotObject)(object)val2) && ((Node)val2).IsInsideTree()) ? val2.GetGlobalTransform() : Transform2D.Identity);
						_killTarget = c.Target;
						_hasKill = true;
						_trophyStarted = false;
						DevLog.Info($"The Hunt: kill recorded; rewardPending={_rewardPending}");
						if (_rewardPending)
						{
							StartTrophy("reward-before-hit");
						}
					}
					end_IL_0044:;
				}
				catch (Exception ex)
				{
					DevLog.Warn("The Hunt kill record: " + ex);
				}
			}
			if (!FxKit.Alive((GodotObject)(object)frontParent))
			{
				return;
			}
			float targetSide = c.TargetSide;
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_HuntCut")
			};
			frontParent.AddChild((Node)(object)holder, false, (InternalMode)0);
			if (!((Node)holder).IsInsideTree())
			{
				((Node)holder).QueueFree();
				return;
			}
			holder.GlobalPosition = at;
			holder.Scale = new Vector2(targetSide, 1f);
			Vector2 val3 = new Vector2(0.5f, -0.36f) * num;
			Vector2 to = new Vector2(-0.5f, 0.34f) * num;
			Line2D glow = FxKit.Crescent(val3, to, num * 0.14f, num * 0.2f, new Color(Gold.R, Gold.G, Gold.B, 0.32f));
			Line2D cut = FxKit.Crescent(val3, to, num * 0.14f, num * 0.075f, new Color(Steel.R, Steel.G, Steel.B, 0.98f));
			Line2D edge = FxKit.Crescent(val3, to, num * 0.17f, num * 0.022f, new Color(Gold.R, Gold.G, Gold.B, 0.95f));
			((Node)holder).AddChild((Node)(object)glow, false, (InternalMode)0);
			((Node)holder).AddChild((Node)(object)cut, false, (InternalMode)0);
			((Node)holder).AddChild((Node)(object)edge, false, (InternalMode)0);
			Vector2[] full = cut.Points;
			Action<float> action = (float t) =>
			{
				if (FxKit.Alive((GodotObject)(object)cut))
				{
					Vector2[] points = Shade.Partial(full, t);
					cut.Points = points;
					if (FxKit.Alive((GodotObject)(object)glow))
					{
						glow.Points = points;
					}
					if (FxKit.Alive((GodotObject)(object)edge))
					{
						edge.Points = Shade.Partial(full, Mathf.Max(0f, t - 0.08f));
					}
				}
			};
			action(0.05f);
			Tween val4 = ((Node)holder).CreateTween();
			val4.TweenMethod(Callable.From<float>(action), Variant.op_Implicit(0.05f), Variant.op_Implicit(1f), 0.07).SetTrans((TransitionType)4).SetEase((EaseType)1);
			val4.TweenInterval(0.05);
			val4.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.2).SetTrans((TransitionType)4).SetEase((EaseType)0);
			val4.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
			Vector2 val5 = new Vector2(0f - targetSide, 0.7f);
			Vector2 value = val5.Normalized();
			FxKit.Burst(frontParent, at, new Color(Steel.R, Steel.G, Steel.B, 0.95f), 10, 220f, 620f, 0.32, 0.05f, 0.13f, new Vector2(0f, 520f), 38f, value, num * 0.12f);
			GameRefs.Shake((!killed) ? 1 : 2);
			if (killed)
			{
				FxKit.Flash(frontParent, at, new Color(Gold.R, Gold.G, Gold.B, 0.75f), num * 0.7f);
				FxKit.RingPulse(frontParent, at, num * 1.3f, Gold, 0.02f, 0.4f, 0.25f, 1.2f, 1f, 0f, 0.04f, 0.95f);
				FxKit.Burst(frontParent, at, new Color(Gold.R, Gold.G, Gold.B, 0.95f), 18, 240f, 680f, 0.5, 0.06f, 0.16f, new Vector2(0f, 380f), 180f, null, num * 0.1f);
			}
		}

		internal static void OnExtraReward(Player player)
		{
			HuntRecord huntRecord = CurrentRecord(MultiplayerFx.CurrentCard);
			if (MultiplayerFx.Active)
			{
				if (huntRecord != null && huntRecord.Playing && player != null && huntRecord.Card.Owner == player)
				{
					huntRecord.RewardPending = true;
					StartTrophy("reward", huntRecord);
				}
			}
			else if (MultiplayerFx.CurrentInvocation == null)
			{
				bool flag = _playingCard != null && player != null && _playingCard.Owner == player;
				DevLog.Info($"The Hunt: card reward seen; matchingPlay={flag}; killRecorded={_hasKill}");
				if (!flag)
				{
					SkipTrophy("reward", "no matching active Hunt play/owner");
					return;
				}
				_rewardPending = true;
				StartTrophy("reward");
			}
		}

		internal static void OnCardFinished(CardModel card)
		{
			HuntRecord huntRecord = CurrentRecord(card);
			if (huntRecord != null)
			{
				huntRecord.Playing = false;
				huntRecord.RewardPending = false;
			}
			else
			{
				if (MultiplayerFx.CurrentInvocation != null)
				{
					return;
				}
				if (MultiplayerFx.Active)
				{
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
					if (obj != null)
					{
						return;
					}
				}
				if (_playingCard != null && _playingCard == card)
				{
					if (_rewardPending && !_trophyStarted)
					{
						SkipTrophy("play-finished", "unmatched reward cleared");
					}
					_playingCard = null;
					_rewardPending = false;
				}
			}
		}

		internal static void ResetTrophy()
		{
			Owned.Clear();
			ResetLegacyTrophy();
		}

		private static void ResetLegacyTrophy()
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			if (_rewardPending && !_trophyStarted)
			{
				SkipTrophy("reset", "pending reward cleared");
			}
			_playingCard = null;
			_killTarget = null;
			_killParent = null;
			_killMsec = 0uL;
			_killTransform = Transform2D.Identity;
			_hasKill = false;
			_rewardPending = false;
			_trophyStarted = false;
		}

		private static void SkipTrophy(string trigger, string reason)
		{
			DevLog.Info("The Hunt: trophy skipped; trigger=" + trigger + "; reason=" + reason);
		}

		public static void Trophy(Creature owner = null)
		{
			if (MultiplayerFx.Active)
			{
				if (owner == null || !Owned.TryGetValue(owner, out var value))
				{
					return;
				}
				HuntRecord huntRecord = CurrentRecord(MultiplayerFx.CurrentCard);
				if (MultiplayerFx.CurrentInvocation != null)
				{
					if (huntRecord == null)
					{
						return;
					}
					Player owner2 = huntRecord.Card.Owner;
					if (((owner2 != null) ? owner2.Creature : null) != owner)
					{
						return;
					}
				}
				else
				{
					if (value.Plays != 1)
					{
						return;
					}
					huntRecord = value.Records.Values.SingleOrDefault();
					if (huntRecord == null || huntRecord.Playing)
					{
						return;
					}
				}
				try
				{
					StartTrophy("power", huntRecord);
					return;
				}
				catch (Exception ex)
				{
					DevLog.Warn("The Hunt trophy: " + ex);
					return;
				}
			}
			if (MultiplayerFx.CurrentInvocation != null)
			{
				return;
			}
			try
			{
				StartTrophy("power");
			}
			catch (Exception ex2)
			{
				DevLog.Warn("The Hunt trophy: " + ex2);
			}
		}

		private static void StartTrophy(string trigger, HuntRecord record = null)
		{
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Expected Obj, but got Unknown
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Expected Obj, but got Unknown
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_032b: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_0467: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			bool flag = record?.TrophyStarted ?? _trophyStarted;
			bool flag2 = record?.HasKill ?? _hasKill;
			bool flag3 = record?.RewardPending ?? _rewardPending;
			if (flag)
			{
				SkipTrophy(trigger, "already started for this kill");
				return;
			}
			if (!flag2)
			{
				SkipTrophy(trigger, flag3 ? "kill not recorded yet; reward retained" : "no recorded kill");
				return;
			}
			double num = 4000.0 / (double)Mathf.Clamp((float)Engine.TimeScale, 0.1f, 1f);
			ulong num2 = Time.GetTicksMsec() - (record?.KillMsec ?? _killMsec);
			if ((double)num2 > num)
			{
				SkipTrophy(trigger, $"kill expired; ageMs={num2}; windowMs={num:0}");
				return;
			}
			Node val = ((record != null) ? record.KillParent : _killParent);
			Node val2 = GameRefs.Overlay;
			if (!CanHostTrophy(val2))
			{
				val2 = null;
			}
			Node parent = val2 ?? val;
			if (!CanHostTrophy(parent))
			{
				SkipTrophy(trigger, "overlay unavailable/hidden and kill parent invalid/hidden");
				return;
			}
			float unit = (record?.KillSize ?? _killSize) * Tuning.F("hunt.trophy", 1f);
			if (unit <= 0.01f)
			{
				SkipTrophy(trigger, "hunt.trophy tuning disabled");
				return;
			}
			Vector2 at = record?.KillAt ?? _killAt;
			Node2D val3 = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_HuntTrophyLifetime")
			};
			TimeFreeze.KeepLive((Node)(object)val3);
			parent.AddChild((Node)(object)val3, false, (InternalMode)0);
			if (!((Node)val3).IsInsideTree())
			{
				((Node)val3).QueueFree();
				SkipTrophy(trigger, "lifetime host did not enter tree");
				return;
			}
			FxKit.FreeAfter((Node)(object)val3, (double)Math.Max(0f, Tuning.F("hunt.trophy_time", 0.6f)) + 2.0);
			val3.GlobalTransform = record?.KillTransform ?? _killTransform;
			parent = (Node)(object)val3;
			Node2D card = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_HuntTrophy")
			};
			TimeFreeze.KeepLive((Node)(object)card);
			parent.AddChild((Node)(object)card, false, (InternalMode)0);
			if (!((Node)card).IsInsideTree())
			{
				((Node)card).QueueFree();
				((Node)val3).QueueFree();
				SkipTrophy(trigger, "trophy card did not enter tree");
				return;
			}
			card.GlobalPosition = at;
			Vector2[] points = new Vector2[8]
			{
				new Vector2(-27f, -48f),
				new Vector2(27f, -48f),
				new Vector2(34f, -41f),
				new Vector2(34f, 41f),
				new Vector2(27f, 48f),
				new Vector2(-27f, 48f),
				new Vector2(-34f, 41f),
				new Vector2(-34f, -41f)
			};
			((Node)card).AddChild((Node)(object)FxKit.Glow(new Color(Gold.R, Gold.G, Gold.B, 0.55f), new Vector2(142.79999f, 172.79999f)), false, (InternalMode)0);
			Polygon2D val4 = FxKit.Shape(points, new Color(Gold.R, Gold.G, Gold.B, 0.3f));
			((CanvasItem)val4).Material = (Material)(object)FxKit.Additive;
			((Node)card).AddChild((Node)(object)val4, false, (InternalMode)0);
			Line2D val5 = Props.Line(4.5f, new Color(1f, 0.93f, 0.62f, 1f), additive: true, points);
			val5.Closed = true;
			((Node)card).AddChild((Node)(object)val5, false, (InternalMode)0);
			Polygon2D val6 = FxKit.Shape(FxKit.Star(2, 15f, 9f), new Color(1f, 0.96f, 0.75f, 0.95f));
			((CanvasItem)val6).Material = (Material)(object)FxKit.Additive;
			((Node2D)val6).Position = new Vector2(0f, -8f);
			((Node)card).AddChild((Node)(object)val6, false, (InternalMode)0);
			float rise = 150f * unit;
			card.Scale = Vector2.Zero;
			Action<float> action = (float t) =>
			{
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)card))
				{
					float num3 = 1f - (1f - t) * (1f - t);
					float num4 = Intro.BackOut(Mathf.Clamp(t / 0.25f, 0f, 1f)) * unit;
					float num5 = Mathf.Cos(t * ((float)Math.PI * 2f));
					card.GlobalPosition = at + new Vector2(0f, (0f - rise) * num3);
					card.Scale = new Vector2(num4 * (Mathf.Abs(num5) * 0.85f + 0.15f), num4);
					card.Rotation = Mathf.Lerp(-0.22f, 0.1f, t);
				}
			};
			Tween val7 = ((Node)card).CreateTween();
			val7.TweenMethod(Callable.From<float>(action), Variant.op_Implicit(0f), Variant.op_Implicit(1f), (double)Tuning.F("hunt.trophy_time", 0.6f));
			val7.TweenInterval(0.1);
			val7.TweenCallback(Callable.From((Action)(() =>
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)card))
				{
					Vector2 globalPosition = card.GlobalPosition;
					if (FxKit.Alive((GodotObject)(object)parent))
					{
						FxKit.Flash(parent, globalPosition, new Color(1f, 0.92f, 0.6f, 0.9f), 120f * unit, 2.4f, 0.26);
						FxKit.RingPulse(parent, globalPosition, 220f * unit, Gold, 0f, 0.34f, 0.3f, 1.3f, 1f, 0f, 0.035f, 0.9f);
						FxKit.Burst(parent, globalPosition, new Color(Gold.R, Gold.G, Gold.B, 0.95f), 26, 160f, 520f, 0.8, 0.05f, 0.15f, new Vector2(0f, -120f), 180f, null, 16f * unit, 300f);
					}
					((Node)card).QueueFree();
				}
			})));
			if (record != null)
			{
				record.TrophyStarted = true;
				record.RewardPending = false;
			}
			else
			{
				_trophyStarted = true;
				_rewardPending = false;
			}
			Sfx.PlayAt("hunt_trophy", Math.Max(0f, Tuning.F("hunt.trophy_time", 0.6f)) + 0.1f, 1f, evenFrozen: true);
			DevLog.Info($"The Hunt: trophy started; trigger={trigger}; ageMs={num2}; parent=" + ((val2 != null) ? "run overlay" : "combat fallback (overlay unavailable/hidden)"));
		}

		private static bool CanHostTrophy(Node node)
		{
			if (FxKit.Alive((GodotObject)(object)node) && node.IsInsideTree())
			{
				CanvasItem val = (CanvasItem)(object)((node is CanvasItem) ? node : null);
				if (val != null)
				{
					return val.IsVisibleInTree();
				}
				return true;
			}
			return false;
		}

		static TheHuntFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		}
	}
