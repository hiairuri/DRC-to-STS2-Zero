// namespace: SilentFX.Powers
// type: PowerFxManager (class)
// source lines: 11731-12374
	public static class PowerFxManager
	{
		private static readonly Dictionary<Type, Func<PowerFx>> Factories = new Dictionary<Type, Func<PowerFx>>();

		private static readonly List<(string Name, Func<PowerFx> Make, bool OnEnemy)> Catalog = new List<(string, Func<PowerFx>, bool)>();

		private static readonly HashSet<Type> OnEnemies = new HashSet<Type>();

		private static readonly Dictionary<Type, Func<Creature, bool>> Gates = new Dictionary<Type, Func<Creature, bool>>();

		private static readonly Dictionary<PowerModel, PowerFx> Active = new Dictionary<PowerModel, PowerFx>(ReferenceEqualityComparer.Instance);

		private static readonly List<PowerFx> Retiring = new List<PowerFx>();

		private static readonly ConditionalWeakTable<PowerFx, object> RemoteOpacity = new ConditionalWeakTable<PowerFx, object>();

		private static double _syncTimer;

		public static bool LogTriggers;

		public static IReadOnlyList<(string Name, Func<PowerFx> Make, bool OnEnemy)> All => Catalog;

		public static void Init()
		{
			PowerFxManager.Register<NoxiousFumesPower>("Noxious Fumes", (Func<PowerFx>)(() => new NoxiousFumesFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<IntangiblePower>("Intangible (Wraith Form's ghost body)", (Func<PowerFx>)(() => new IntangibleFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<WraithFormPower>("Wraith Form (the Dexterity drain)", (Func<PowerFx>)(() => new WraithFormFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<InfiniteBladesPower>("Infinite Blades", (Func<PowerFx>)(() => new InfiniteBladesFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<PhantomBladesPower>("Phantom Blades", (Func<PowerFx>)(() => new PhantomBladesFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<EnvenomPower>("Envenom", (Func<PowerFx>)(() => new EnvenomFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<AfterimagePower>("Afterimage", (Func<PowerFx>)(() => new AfterimageFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<ThornsPower>("Abrasive (Thorns)", (Func<PowerFx>)(() => new ThornsFx()), false, (Func<Creature, bool>)ThornsFx.FromAbrasive);
			PowerFxManager.Register<MasterPlannerPower>("Master Planner", (Func<PowerFx>)(() => new MasterPlannerFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<SneakyPower>("Sneaky", (Func<PowerFx>)(() => new SneakyFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<ToolsOfTheTradePower>("Tools of the Trade", (Func<PowerFx>)(() => new ToolsFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<TrackingPower>("Tracking", (Func<PowerFx>)(() => new TrackingFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<WellLaidPlansPower>("Well-Laid Plans", (Func<PowerFx>)(() => new WellLaidPlansFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<BurstPower>("Burst", (Func<PowerFx>)(() => new BurstFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<AccuracyPower>("Accuracy", (Func<PowerFx>)(() => new AccuracyFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<CorrosiveWavePower>("Corrosive Wave", (Func<PowerFx>)(() => new CorrosiveWaveFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<AccelerantPower>("Accelerant", (Func<PowerFx>)(() => new AccelerantFx()), false, (Func<Creature, bool>)null);
			RegisterOptional("ConcoctPower", "Concoct", () => new ConcoctFx());
			PowerFxManager.Register<PoisonPower>("Poison (on an enemy)", (Func<PowerFx>)(() => new PoisonFx()), true, (Func<Creature, bool>)null);
			PowerFxManager.Register<ShadowStepPower>("Shadow Step (the shadow under her feet)", (Func<PowerFx>)(() => new ShadowPoolFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<DoubleDamagePower>("Shadow Step (the doubled turn)", (Func<PowerFx>)(() => new ShadowTwinFx()), false, (Func<Creature, bool>)ShadowEcho.HasStepped);
			PowerFxManager.Register<ShadowmeldPower>("Shadowmeld", (Func<PowerFx>)(() => new ShadowmeldFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<TheHuntPower>("The Hunt (the trophy)", (Func<PowerFx>)(() => new TheHuntTrophyFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<BlurPower>("Blur (the haze while Block is kept)", (Func<PowerFx>)(() => new BlurHazeFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<StranglePower>("Strangle (the grip on an enemy)", (Func<PowerFx>)(() => new StrangleGripFx()), true, (Func<Creature, bool>)null);
			PowerFxManager.Register<SpeedsterPower>("Speedster", (Func<PowerFx>)(() => new SpeedsterFx()), false, (Func<Creature, bool>)null);
			PowerFxManager.Register<BlockNextTurnPower>("Block next turn (Dodge and Roll)", (Func<PowerFx>)(() => new RollEchoFx()), false, (Func<Creature, bool>)null);
			PlayerSettings.Changed += OnSettingsChanged;
			Ticker.Add(Update);
			DevLog.Info($"PowerFxManager ready, {Factories.Count} power effect(s) registered");
		}

		public static void Register<T>(string name, Func<PowerFx> factory, bool onEnemies = false, Func<Creature, bool> gate = null) where T : PowerModel
		{
			Register(typeof(T), name, factory, onEnemies, gate);
		}

		private static void Register(Type type, string name, Func<PowerFx> factory, bool onEnemies, Func<Creature, bool> gate)
		{
			Factories[type] = factory;
			if (onEnemies)
			{
				OnEnemies.Add(type);
			}
			if (gate != null)
			{
				Gates[type] = gate;
			}
			Catalog.Add((name, factory, onEnemies));
		}

		private static void RegisterOptional(string powerName, string name, Func<PowerFx> factory)
		{
			Type type = GameCapabilities.Power(powerName);
			if (type == null)
			{
				DevLog.Info("Power FX unavailable in this game build: " + powerName);
				return;
			}
			try
			{
				Register(type, name, factory, onEnemies: false, null);
			}
			catch (Exception ex)
			{
				DevLog.Error("Optional power FX setup failed (" + powerName + "): " + ex);
			}
		}

		private static bool OwnerFits(Creature owner, PowerModel power)
		{
			if (owner == null || power == null)
			{
				return false;
			}
			if (OnEnemies.Contains(((object)power).GetType()))
			{
				if (!owner.IsPlayer)
				{
					return owner.IsAlive;
				}
				return false;
			}
			if (!owner.IsPlayer)
			{
				return false;
			}
			if (MultiplayerFx.Active && (power is ToolsOfTheTradePower || power is WellLaidPlansPower) && !MultiplayerFx.IsLocal(owner))
			{
				return false;
			}
			if (Gates.TryGetValue(((object)power).GetType(), out var value))
			{
				return value(owner);
			}
			return true;
		}

		public static bool Handles(PowerModel power)
		{
			if (power != null)
			{
				return Factories.ContainsKey(((object)power).GetType());
			}
			return false;
		}

		internal static void OnRemoteCardPlayed(CardModel card, Creature target)
		{
			if (!MultiplayerFx.Active || !(card is Shiv) || MultiplayerFx.IsLocal(card))
			{
				return;
			}
			foreach (PowerFx value in Active.Values)
			{
				if (value is AccuracyFx accuracyFx)
				{
					accuracyFx.OnRemoteCardPlayed(card, target);
				}
			}
		}

		public static void OnApplied(Creature owner, PowerModel power)
		{
			if (PlayerSettings.Enabled && Handles(power))
			{
				TryAttach(owner, power, animate: true);
			}
		}

		public static void OnRemoved(PowerModel power)
		{
			if (power != null && Active.Remove(power, out var value))
			{
				if (!OnEnemies.Contains(((object)power).GetType()) || LogTriggers)
				{
					DevLog.Info("Power FX off: " + ((object)power).GetType().Name);
				}
				Retire(value);
			}
		}

		public static void OnAmountChanged(PowerModel power)
		{
			if (power != null && Active.TryGetValue(power, out var value))
			{
				value.Amount = power.Amount;
				using (CaptureOpacity(value))
				{
					value.OnAmountChanged(power.Amount);
				}
			}
		}

		public static void Trigger(PowerModel power, Creature target = null)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			if (!PlayerSettings.Enabled || power == null || !Active.TryGetValue(power, out var fx))
			{
				return;
			}
			Vector2? at = null;
			if (target != null)
			{
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(target) : null);
				if (FxKit.Alive((GodotObject)(object)val))
				{
					at = val.VfxSpawnPosition;
				}
			}
			if (LogTriggers)
			{
				DevLog.Info("Power FX trigger: " + ((object)power).GetType().Name);
			}
			if (!TimeFreeze.Defer(Show))
			{
				Show();
			}
			void Show()
			{
				using (CaptureOpacity(fx))
				{
					fx.Trigger(at);
				}
			}
		}

		internal static void NoteStrangleCardPlayed(StranglePower power, CardModel card)
		{
			if (MultiplayerFx.Active && power != null && Active.TryGetValue((PowerModel)(object)power, out var value) && value is StrangleGripFx strangleGripFx)
			{
				strangleGripFx.NotePlayed(card);
			}
		}

		internal static void TriggerStrangle(StranglePower power, CardModel card)
		{
			if (!MultiplayerFx.Active)
			{
				Trigger((PowerModel)(object)power);
			}
			else
			{
				if (power == null || !Active.TryGetValue((PowerModel)(object)power, out var fx))
				{
					return;
				}
				StrangleGripFx grip = fx as StrangleGripFx;
				if (grip == null)
				{
					return;
				}
				object invocation = MultiplayerFx.CurrentInvocation;
				if (invocation == null || MultiplayerFx.CurrentCard != card)
				{
					return;
				}
				Action action = () =>
				{
					using (CaptureOpacity(fx))
					{
						grip.TriggerCard(card, invocation);
					}
				};
				if (LogTriggers)
				{
					DevLog.Info("Power FX trigger: " + ((object)power).GetType().Name);
				}
				if (!TimeFreeze.Defer(action))
				{
					action();
				}
			}
		}

		public static void TriggerFor<T>(Creature owner, Creature target = null) where T : PowerModel
		{
			if (owner == null)
			{
				return;
			}
			foreach (PowerModel power in owner.Powers)
			{
				if (power is T)
				{
					Trigger(power, target);
					break;
				}
			}
		}

		public static bool TriggerType(Creature owner, Type powerType, Creature target = null)
		{
			if (owner == null || powerType == null)
			{
				return false;
			}
			foreach (PowerModel power in owner.Powers)
			{
				if (powerType.IsInstanceOfType(power))
				{
					Trigger(power, target);
					return true;
				}
			}
			return false;
		}

		public static void TriggerAll()
		{
			foreach (PowerFx item in Active.Values.ToList())
			{
				item.Trigger();
			}
		}

		public static void ClearAll(bool animate)
		{
			foreach (PowerFx item in Active.Values.ToList())
			{
				if (animate)
				{
					Retire(item);
					continue;
				}
				try
				{
					item.Stop(animateOut: false);
				}
				catch (Exception ex)
				{
					DevLog.Warn("Stop failed: " + ex.Message);
				}
			}
			Active.Clear();
			if (animate)
			{
				return;
			}
			foreach (PowerFx item2 in Retiring.ToList())
			{
				try
				{
					item2.Stop(animateOut: false);
				}
				catch
				{
				}
			}
			Retiring.Clear();
		}

		private static void Retire(PowerFx fx)
		{
			try
			{
				using (CaptureOpacity(fx))
				{
					fx.Stop(animateOut: true);
					if (fx.IsAlive)
					{
						Retiring.Add(fx);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Stop failed: " + ex.Message);
			}
		}

		public static void RestartAll()
		{
			ClearAll(animate: false);
			_syncTimer = 1.0;
		}

		private static void TryAttach(Creature owner, PowerModel power, bool animate)
		{
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			if (!PlayerSettings.Enabled || Active.ContainsKey(power) || !OwnerFits(owner, power))
			{
				return;
			}
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			if (combatRoom == null)
			{
				return;
			}
			NCreature creatureNode = combatRoom.GetCreatureNode(owner);
			if (!FxKit.Alive((GodotObject)(object)creatureNode) || !((Node)creatureNode).IsInsideTree())
			{
				return;
			}
			PowerFx powerFx = Factories[((object)power).GetType()]();
			powerFx.Anchor = BuildAnchor(combatRoom, creatureNode, owner);
			if (powerFx is StrangleGripFx strangleGripFx)
			{
				StranglePower val = (StranglePower)(object)((power is StranglePower) ? power : null);
				if (val != null)
				{
					strangleGripFx.Bind(val);
				}
			}
			powerFx.Amount = power.Amount;
			StranglePower val2 = (StranglePower)(object)((power is StranglePower) ? power : null);
			Creature val3 = ((val2 != null) ? ((PowerModel)val2).Applier : owner);
			bool flag = MultiplayerFx.Active && val3 != null && val3.IsPlayer && !MultiplayerFx.IsLocal(val3);
			if (flag)
			{
				bool flag2 = ((power is IntangiblePower || power is WraithFormPower || power is ShadowStepPower || power is DoubleDamagePower || power is ShadowmeldPower || power is AccelerantPower) ? true : false);
				flag = !flag2;
			}
			if (flag)
			{
				RemoteOpacity.Add(powerFx, new object());
			}
			using (CaptureOpacity(powerFx))
			{
				powerFx.Start(animate);
				Active[power] = powerFx;
				if (!OnEnemies.Contains(((object)power).GetType()) || LogTriggers)
				{
					DevLog.Info($"Power FX on: {((object)power).GetType().Name} x{power.Amount} (body {powerFx.Anchor.SafeBodySize()})");
				}
			}
		}

		private static IDisposable CaptureOpacity(PowerFx fx)
		{
			object value;
			return RemoteFxFade.Capture(fx != null && RemoteOpacity.TryGetValue(fx, out value), fx?.Anchor?.BackParent, fx?.Anchor?.FrontParent, GameRefs.Overlay, (Node)GameRefs.CombatRoom);
		}

		private static FxAnchor BuildAnchor(NCombatRoom room, NCreature node, Creature owner)
		{
			return new FxAnchor
			{
				Owner = owner,
				BackParent = (Node)(object)room.BackCombatVfxContainer,
				FrontParent = (Node)(object)room.CombatVfxContainer,
				Feet = () =>
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					return GameRefs.BottomOf(node);
				},
				Chest = () =>
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					return node.VfxSpawnPosition;
				},
				BodySize = () =>
				{
					//IL_0016: Unknown result type (might be due to invalid IL or missing references)
					return GameRefs.DrawnSize(node.Hitbox, (Node)(object)room.CombatVfxContainer);
				},
				Body = () => (CanvasItem)(object)node.Body,
				Facing = () => GameRefs.FacingOf(node),
				Targets = () =>
				{
					List<FxTarget> list = new List<FxTarget>();
					FillTargetSnapshot(room, node, owner, list);
					return list;
				},
				FillTargets = (List<FxTarget> list) =>
				{
					FillTargetSnapshot(room, node, owner, list);
				},
				IsValid = () => FxKit.Alive((GodotObject)(object)node) && ((Node)node).IsInsideTree() && FxKit.Alive((GodotObject)(object)room)
			};
		}

		private static void FillTargetSnapshot(NCombatRoom room, NCreature node, Creature owner, List<FxTarget> list)
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			ICombatState combatState = owner.CombatState;
			if (combatState == null || !FxKit.Alive((GodotObject)(object)room))
			{
				list.Clear();
				return;
			}
			int num = 0;
			foreach (Creature hittableEnemy in combatState.HittableEnemies)
			{
				NCreature creatureNode = room.GetCreatureNode(hittableEnemy);
				if (FxKit.Alive((GodotObject)(object)creatureNode) && ((Node)creatureNode).IsInsideTree())
				{
					FxTarget fxTarget;
					if (num < list.Count)
					{
						fxTarget = list[num];
					}
					else
					{
						fxTarget = new FxTarget();
						list.Add(fxTarget);
					}
					fxTarget.Key = hittableEnemy;
					fxTarget.Feet = GameRefs.BottomOf(creatureNode);
					fxTarget.Chest = creatureNode.VfxSpawnPosition;
					fxTarget.Width = (FxKit.Alive((GodotObject)(object)creatureNode.Hitbox) ? GameRefs.DrawnSize(creatureNode.Hitbox, (Node)(object)room.CombatVfxContainer).X : 200f);
					fxTarget.Weak = hittableEnemy.HasPower<WeakPower>();
					fxTarget.Poison = hittableEnemy.GetPowerAmount<PoisonPower>();
					fxTarget.Side = ((creatureNode.VfxSpawnPosition.X < node.VfxSpawnPosition.X) ? (-1f) : 1f);
					num++;
				}
			}
			if (num < list.Count)
			{
				list.RemoveRange(num, list.Count - num);
			}
		}

		private static void Update(double realDt)
		{
			double dt = realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f);
			if (Active.Count > 0)
			{
				List<PowerModel> list = null;
				foreach (KeyValuePair<PowerModel, PowerFx> item in Active)
				{
					try
					{
						using (CaptureOpacity(item.Value))
						{
							item.Value.Update(dt);
						}
					}
					catch (Exception value)
					{
						DevLog.Warn($"Power FX update failed ({((object)item.Key).GetType().Name}): {value}");
						(list ?? (list = new List<PowerModel>())).Add(item.Key);
						continue;
					}
					if (!item.Value.IsAlive)
					{
						(list ?? (list = new List<PowerModel>())).Add(item.Key);
					}
				}
				if (list != null)
				{
					foreach (PowerModel item2 in list)
					{
						if (Active.Remove(item2, out var value2))
						{
							try
							{
								value2.Stop(animateOut: false);
							}
							catch
							{
							}
						}
					}
				}
			}
			for (int num = Retiring.Count - 1; num >= 0; num--)
			{
				PowerFx powerFx = Retiring[num];
				bool flag;
				try
				{
					using (CaptureOpacity(powerFx))
					{
						powerFx.Update(dt);
						flag = !powerFx.IsAlive;
					}
				}
				catch
				{
					flag = true;
				}
				if (flag)
				{
					Retiring.RemoveAt(num);
				}
			}
			_syncTimer += realDt;
			if (_syncTimer >= 0.5)
			{
				_syncTimer = 0.0;
				Sync();
			}
		}

		private static void Sync()
		{
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			if (combatRoom == null)
			{
				ClearAll(animate: false);
			}
			else
			{
				if (!PlayerSettings.Enabled)
				{
					return;
				}
				foreach (NCreature item in combatRoom.CreatureNodes.ToList())
				{
					if (!FxKit.Alive((GodotObject)(object)item))
					{
						continue;
					}
					Creature entity = item.Entity;
					if (entity == null)
					{
						continue;
					}
					foreach (PowerModel item2 in entity.Powers.ToList())
					{
						if (Handles(item2) && !Active.ContainsKey(item2) && OwnerFits(entity, item2))
						{
							try
							{
								TryAttach(entity, item2, animate: true);
							}
							catch (Exception value)
							{
								DevLog.Warn($"Attach failed ({((object)item2).GetType().Name}): {value}");
							}
						}
					}
				}
				foreach (KeyValuePair<PowerModel, PowerFx> kv in Active.ToList())
				{
					bool flag;
					try
					{
						Creature owner = kv.Key.Owner;
						flag = owner != null && owner.Powers.Any((PowerModel p) => p == kv.Key);
					}
					catch
					{
						flag = false;
					}
					if (!flag)
					{
						Active.Remove(kv.Key);
						Retire(kv.Value);
					}
				}
			}
		}

		private static void OnSettingsChanged()
		{
			if (!PlayerSettings.Enabled)
			{
				ClearAll(animate: true);
			}
		}
	}
