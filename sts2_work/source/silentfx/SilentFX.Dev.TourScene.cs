// namespace: SilentFX.Dev
// type: TourScene (class)
// source lines: 28315-28661
	internal static class TourScene
	{
		private static bool _hadChem;

		private static bool _changedChem;

		internal static bool Restoring { get; private set; }

		internal static void NoteStart(Player me)
		{
			_changedChem = false;
			try
			{
				_hadChem = ((me != null) ? me.GetRelic<ChemicalX>() : null) != null;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: relic note failed: " + ex.Message);
				_hadChem = false;
			}
		}

		private static async Task SetChem(bool wanted, Player player)
		{
			ChemicalX relic = player.GetRelic<ChemicalX>();
			if (wanted && relic == null)
			{
				await RelicCmd.Obtain<ChemicalX>(player);
			}
			else
			{
				if (wanted || relic == null)
				{
					return;
				}
				await RelicCmd.Remove((RelicModel)(object)relic);
			}
			_changedChem = true;
			DevLog.Info("Tour: Chemical X " + (wanted ? "given" : "taken back"));
		}

		internal static async void RestoreRelics(RunState run)
		{
			if (!_changedChem || Restoring)
			{
				return;
			}
			Restoring = true;
			try
			{
				RunManager instance = RunManager.Instance;
				if (run == null || instance == null || !instance.IsInProgress || run != instance.DebugOnlyGetState())
				{
					return;
				}
				await TourController.WaitUntil(() =>
				{
					CombatManager instance3 = CombatManager.Instance;
					return instance3 == null || !instance3.IsInProgress || Ready();
				}, CancellationToken.None);
				RunManager instance2 = RunManager.Instance;
				if (run == ((instance2 != null) ? instance2.DebugOnlyGetState() : null))
				{
					Player me = LocalContext.GetMe((IPlayerCollection)(object)run);
					if (me != null)
					{
						await SetChem(_hadChem, me);
						_changedChem = false;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: relic restore failed: " + ex.Message);
			}
			finally
			{
				Restoring = false;
			}
		}

		internal static bool Ready()
		{
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Invalid comparison between Unknown and I4
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Invalid comparison between Unknown and I4
			try
			{
				CombatManager instance = CombatManager.Instance;
				if (instance == null || !instance.IsInProgress || instance.IsOverOrEnding)
				{
					return false;
				}
				RunManager instance2 = RunManager.Instance;
				CombatState val = instance.DebugOnlyGetState();
				Player val2 = ((val == null) ? null : LocalContext.GetMe((ICombatState)(object)val));
				int result;
				if (val != null && val.Players.Count == 1 && val2 != null && val2.Creature.IsAlive)
				{
					PlayerCombatState playerCombatState = val2.PlayerCombatState;
					if (playerCombatState != null && (int)playerCombatState.Phase == 3 && (int)val.CurrentSide == 1 && !instance.PlayerActionsDisabled && !instance.IsExecutingCardOrPotionEffect(val2) && instance2 != null)
					{
						ActionQueueSet actionQueueSet = instance2.ActionQueueSet;
						if (((actionQueueSet != null) ? new bool?(actionQueueSet.IsEmpty) : ((bool?)null)) == true)
						{
							ActionExecutor actionExecutor = instance2.ActionExecutor;
							if (actionExecutor != null && !actionExecutor.IsRunning && !instance2.ActionExecutor.IsPaused && NCombatRoom.Instance != null && NPlayerHand.Instance != null && !NPlayerHand.Instance.IsInCardSelection && !NPlayerHand.Instance.InCardPlay)
							{
								NGame instance3 = NGame.Instance;
								if (instance3 == null)
								{
									result = 1;
								}
								else
								{
									NTransition transition = instance3.Transition;
									result = ((((transition != null) ? new bool?(transition.InTransition) : ((bool?)null)) != true) ? 1 : 0);
								}
								goto IL_0163;
							}
						}
					}
				}
				result = 0;
				goto IL_0163;
				IL_0163:
				return (byte)result != 0;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: readiness check failed: " + ex.Message);
				return false;
			}
		}

		internal static async Task Prepare(TourDefinition stop, CancellationToken cancel)
		{
			_ = 13;
			try
			{
				TourController.Check(cancel);
				CombatState old = CombatManager.Instance.DebugOnlyGetState();
				TourTurnPlan.Cancel();
				Stopwatch watch = Stopwatch.StartNew();
				string label = ((stop.Text.Number == 0) ? "finale" : $"stop {stop.Text.Number:00} {stop.Text.Name}");
				DevLog.Info("Tour: " + label + " - entering inklets_normal");
				EncounterModel val = ((EncounterModel)ModelDb.Encounter<InkletsNormal>()).ToMutable();
				val.DebugRandomizeRng();
				await RunManager.Instance.EnterRoomDebug((RoomType)1, (MapPointType)0, (AbstractModel)(object)val, true);
				TourController.Check(cancel);
				await TourController.WaitUntil(() => old != CombatManager.Instance.DebugOnlyGetState() && Ready(), cancel);
				CombatState state = CombatManager.Instance.DebugOnlyGetState();
				Player player = LocalContext.GetMe((ICombatState)(object)state);
				TourController.Check(cancel, (ICombatState)(object)state);
				double entered = watch.Elapsed.TotalSeconds;
				DevLog.Info($"Tour: fight entered in {entered:0.00} s; opening draw history preserved; round {state.RoundNumber}");
				NCombatRoom room = NCombatRoom.Instance;
				if (state.Enemies.Count != 3 || state.Enemies.Any((Creature e) => !(e.Monster is Inklet) || !e.IsAlive))
				{
					throw new InvalidOperationException("新战斗没有三只活着的墨灵");
				}
				foreach (Creature enemy2 in state.Enemies)
				{
					if (room.GetCreatureNode(enemy2) == null)
					{
						throw new InvalidOperationException("墨灵节点尚未创建");
					}
				}
				Creature[] enemies = state.Enemies.OrderBy((Creature e) =>
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					return ((Control)room.GetCreatureNode(e)).GlobalPosition.X;
				}).ToArray();
				DevLog.Info("Tour: clearing all powers and block");
				Creature[] array = state.Creatures.ToArray();
				foreach (Creature creature in array)
				{
					int removals = 0;
					while (creature.Powers.Count > 0)
					{
						TourController.Check(cancel, (ICombatState)(object)state);
						int num2 = removals + 1;
						removals = num2;
						if (num2 > 100)
						{
							throw new InvalidOperationException("能力在移除后不断重生，停止摆站");
						}
						await PowerCmd.Remove(creature.Powers[0]);
					}
					TourController.Check(cancel, (ICombatState)(object)state);
					if (creature.Block > 0)
					{
						await RuntimeApiCompatibility.LoseBlock((PlayerChoiceContext)new ThrowingPlayerChoiceContext(), creature, creature.Block);
					}
				}
				TourController.Check(cancel, (ICombatState)(object)state);
				await CreatureCmd.SetCurrentHp(player.Creature, (decimal)player.Creature.MaxHp);
				array = enemies;
				foreach (Creature obj in array)
				{
					TourController.Check(cancel, (ICombatState)(object)state);
					await CreatureCmd.SetMaxAndCurrentHp(obj, (decimal)stop.EnemyHp);
				}
				await SetChem(stop.Relic == "ChemicalX" || _hadChem, player);
				TourController.Check(cancel, (ICombatState)(object)state);
				foreach (TourEnemyRule rule in stop.EnemyRules)
				{
					array = ((rule.Index == 0) ? enemies : new Creature[1] { enemies[rule.Index - 1] });
					foreach (Creature enemy in array)
					{
						TourController.Check(cancel, (ICombatState)(object)state);
						await ApplyRule(rule, enemy, player.Creature);
					}
					DevLog.Info($"Tour: enemy {((rule.Index == 0) ? "all" : rule.Index.ToString())}: {rule.Kind} {rule.Amount}");
				}
				for (int num = 0; num < enemies.Length; num++)
				{
					TourController.Check(cancel, (ICombatState)(object)state);
					Creature creature = enemies[num];
					string turn = stop.EnemyTurns[num] ?? stop.EnemyTurn;
					switch (turn)
					{
					case "攻击":
					case "连击":
						creature.Monster.SetMoveImmediate((MoveState)creature.Monster.MoveStateMachine.States[(turn == "连击") ? "WHIRLWIND_MOVE" : "JAB_MOVE"], true);
						break;
					case "不攻击":
						await CreatureCmd.Stun(creature, "JAB_MOVE");
						break;
					}
					DevLog.Info($"Tour: enemy {num + 1} turn = {((turn.Length == 0) ? "natural" : turn)}; move = {((MonsterState)creature.Monster.NextMove).Id}");
				}
				for (int num3 = 0; num3 < enemies.Length; num3++)
				{
					Creature val2 = enemies[num3];
					DevLog.Info($"Tour: enemy {num3 + 1}: HP {val2.CurrentHp}/{val2.MaxHp}, block {val2.Block}, powers [{string.Join(", ", val2.Powers.Select((PowerModel p) => ((object)p).GetType().Name + " " + p.Amount))}]");
				}
				DevLog.Info("Tour: replacing combat piles (run deck unchanged)");
				PileType[] array2 = new PileType[5];
				RuntimeHelpers.InitializeArray(array2, __ldtoken(<PrivateImplementationDetails>.6486F2D69AE81EA4A81D06A9F45C2F14B7D35CC9454CB8F98045F99998E8DF9A));
				PileType[] array3 = array2;
				foreach (PileType val3 in array3)
				{
					TourController.Check(cancel, (ICombatState)(object)state);
					CardModel[] array4 = PileTypeExtensions.GetPile(val3, player).Cards.ToArray();
					if (array4.Length != 0)
					{
						await CardPileCmd.RemoveFromCombat((IEnumerable<CardModel>)array4, (int)val3 != 2 && (int)val3 != 5);
					}
				}
				await AddPile(stop.Draw, (PileType)1, (ICombatState)(object)state, player, cancel);
				await AddPile(stop.Exhaust, (PileType)4, (ICombatState)(object)state, player, cancel);
				await AddPile(stop.Hand, (PileType)2, (ICombatState)(object)state, player, cancel);
				TourController.Check(cancel, (ICombatState)(object)state);
				await PlayerCmd.SetEnergy((decimal)stop.Energy, player);
				TourController.Check(cancel, (ICombatState)(object)state);
				TourTurnPlan.Arm((ICombatState)(object)state, player, stop.NextDraw, cancel);
				VerifyPile((PileType)2, stop.Hand, player);
				VerifyPile((PileType)1, stop.Draw, player);
				VerifyPile((PileType)4, stop.Exhaust, player);
				if (PileTypeExtensions.GetPile((PileType)3, player).Cards.Count != 0 || PileTypeExtensions.GetPile((PileType)5, player).Cards.Count != 0 || player.PlayerCombatState.Energy != stop.Energy || player.Creature.Powers.Count != 0)
				{
					throw new InvalidOperationException("摆站后的状态与脚本不一致，请检查遗物或其他模组的入场效果");
				}
				if (player.GetRelic<ChemicalX>() != null != (stop.Relic == "ChemicalX" || _hadChem))
				{
					throw new InvalidOperationException("化合物X 与脚本不一致");
				}
				DevLog.Info($"Tour: {label} - fight entered in {entered:0.00} s; hand {stop.Hand.Count} [{string.Join(", ", stop.Hand.Select((TourCard c) => c.Title))}]; enemies {string.Join("/", enemies.Select((Creature e) => e.CurrentHp))}; energy {player.PlayerCombatState.Energy}; total {watch.Elapsed.TotalSeconds:0.00} s");
			}
			catch (OperationCanceledException)
			{
				DevLog.Info("Tour: scene preparation cancelled at a command boundary");
				throw;
			}
			catch (Exception ex2)
			{
				DevLog.Warn("Tour: scene preparation failed: " + ex2);
				throw;
			}
		}

		internal static CardModel CreateCard(TourCard requested, ICombatState state, Player player)
		{
			CardModel val = state.CreateCard(requested.Canonical, player);
			if (requested.Upgraded)
			{
				if (!val.IsUpgradable)
				{
					throw new InvalidOperationException("不能升级：" + requested.Title);
				}
				val.UpgradeInternal();
				val.FinalizeUpgradeInternal();
			}
			return val;
		}

		private static async Task AddPile(IEnumerable<TourCard> models, PileType pile, ICombatState state, Player player, CancellationToken cancel)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			foreach (TourCard model in models)
			{
				TourController.Check(cancel, state);
				await CardPileCmd.Add(CreateCard(model, state, player), pile, (CardPilePosition)1, (AbstractModel)null, (int)pile != 2);
			}
			DevLog.Info($"Tour: {pile} {PileTypeExtensions.GetPile(pile, player).Cards.Count} [{string.Join(", ", PileTypeExtensions.GetPile(pile, player).Cards.Select((CardModel c) => c.Title))}]");
		}

		private static void VerifyPile(PileType pile, List<TourCard> expected, Player player)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			if (!PileTypeExtensions.GetPile(pile, player).Cards.Select((CardModel c) => (((object)c).GetType(), CurrentUpgradeLevel: c.CurrentUpgradeLevel)).SequenceEqual(expected.Select((TourCard c) => c.Key)))
			{
				throw new InvalidOperationException(((object)pile/*cast due to constrained. prefix*/).ToString() + " 与脚本不一致，请检查遗物或其他模组");
			}
		}

		private static async Task ApplyRule(TourEnemyRule rule, Creature enemy, Creature me)
		{
			ThrowingPlayerChoiceContext val = new ThrowingPlayerChoiceContext();
			switch (rule.Kind)
			{
			case "血量":
				await CreatureCmd.SetCurrentHp(enemy, (decimal)rule.Amount);
				break;
			case "格挡":
				await CreatureCmd.GainBlock(enemy, (decimal)rule.Amount, (ValueProp)4, (CardPlay)null, true);
				break;
			case "中毒":
				await PowerCmd.Apply<PoisonPower>((PlayerChoiceContext)(object)val, enemy, (decimal)rule.Amount, me, (CardModel)null, true);
				break;
			case "虚弱":
				await PowerCmd.Apply<WeakPower>((PlayerChoiceContext)(object)val, enemy, (decimal)rule.Amount, me, (CardModel)null, true);
				break;
			case "易伤":
				await PowerCmd.Apply<VulnerablePower>((PlayerChoiceContext)(object)val, enemy, (decimal)rule.Amount, me, (CardModel)null, true);
				break;
			case "人工制品":
				await PowerCmd.Apply<ArtifactPower>((PlayerChoiceContext)(object)val, enemy, (decimal)rule.Amount, me, (CardModel)null, true);
				break;
			default:
				throw new InvalidOperationException("Unknown enemy rule: " + rule.Kind);
			}
		}
	}
