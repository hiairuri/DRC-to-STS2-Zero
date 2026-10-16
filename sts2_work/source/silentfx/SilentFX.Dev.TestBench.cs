// namespace: SilentFX.Dev
// type: TestBench (class)
// source lines: 25725-27335
	public static class TestBench
	{
		private enum Operation
		{
			Enter,
			Leave,
			AddCard,
			ClearHand,
			ClearPowers,
			Weak,
			Poison,
			Guard,
			LowHp,
			Chem,
			KeepAlive
		}

		private static readonly (string Name, Type[] Cards)[] Groups = new (string, Type[])[8]
		{
			("能力牌", new Type[15]
			{
				typeof(NoxiousFumes),
				typeof(WraithForm),
				typeof(InfiniteBlades),
				typeof(PhantomBlades),
				typeof(Envenom),
				typeof(Afterimage),
				typeof(Abrasive),
				typeof(MasterPlanner),
				typeof(Sneaky),
				typeof(ToolsOfTheTrade),
				typeof(Tracking),
				typeof(WellLaidPlans),
				typeof(Accuracy),
				typeof(Accelerant),
				typeof(FanOfKnives)
			}),
			("金卡 / 技能", new Type[10]
			{
				typeof(Burst),
				typeof(Murder),
				typeof(Assassinate),
				typeof(BulletTime),
				typeof(Malaise),
				typeof(StormOfSteel),
				typeof(KnifeTrap),
				typeof(ShadowStep),
				typeof(Shadowmeld),
				typeof(TheHunt)
			}),
			("小刀系", new Type[8]
			{
				typeof(Shiv),
				typeof(BladeOfInk),
				typeof(HiddenDaggers),
				typeof(UpMySleeve),
				GameCapabilities.Card("BladeSymphony"),
				typeof(BladeDance),
				typeof(CloakAndDagger),
				typeof(LeadingStrike)
			}),
			("毒系", new Type[8]
			{
				typeof(Outbreak),
				typeof(CorrosiveWave),
				GameCapabilities.Card("Concoct"),
				typeof(BubbleBubble),
				typeof(Mirage),
				typeof(DeadlyPoison),
				typeof(PoisonedStab),
				typeof(Snakebite)
			}),
			("蓝卡攻击", new Type[11]
			{
				typeof(Backstab),
				typeof(Dash),
				typeof(EchoingSlash),
				typeof(Finisher),
				typeof(Flechettes),
				typeof(MementoMori),
				typeof(Pinpoint),
				typeof(Pounce),
				typeof(PreciseCut),
				typeof(Skewer),
				typeof(Strangle)
			}),
			("蓝卡技能", new Type[14]
			{
				typeof(Acrobatics),
				typeof(Blur),
				typeof(CalculatedGamble),
				typeof(EscapePlan),
				typeof(Expertise),
				typeof(Expose),
				GameCapabilities.Card("Fade"),
				typeof(Footwork),
				typeof(HandTrick),
				typeof(LegSweep),
				typeof(Reflex),
				GameCapabilities.Card("Sidestep"),
				typeof(Tactician),
				typeof(Speedster)
			}),
			("白卡", new Type[11]
			{
				typeof(Predator),
				typeof(Ricochet),
				typeof(SuckerPunch),
				typeof(FlickFlack),
				typeof(Backflip),
				typeof(Deflect),
				typeof(DodgeAndRoll),
				typeof(PiercingWail),
				typeof(Anticipate),
				typeof(Prepared),
				typeof(Untouchable)
			}),
			("辅助", new Type[5]
			{
				typeof(Neutralize),
				typeof(StrikeSilent),
				typeof(DefendSilent),
				typeof(Survivor),
				typeof(Suppress)
			})
		};

		private static readonly HashSet<PowerModel> Baseline = new HashSet<PowerModel>(ReferenceEqualityComparer.Instance);

		private static readonly List<CardModel> Spawned = new List<CardModel>();

		private static readonly List<(Creature Enemy, int Max, int Current)> EnemyHp = new List<(Creature, int, int)>();

		private static readonly Dictionary<CardModel, PileType> Home = new Dictionary<CardModel, PileType>(ReferenceEqualityComparer.Instance);

		private static readonly HashSet<CardModel> HadCostChange = new HashSet<CardModel>(ReferenceEqualityComparer.Instance);

		private static readonly List<CardModel> DrawOrder = new List<CardModel>();

		private static readonly List<(Creature Owner, Type Power, int Amount)> BasePowers = new List<(Creature, Type, int)>();

		private static readonly MethodInfo ApplyByType = typeof(PowerCmd).GetMethods().FirstOrDefault((MethodInfo m) => m.Name == "Apply" && m.IsGenericMethodDefinition && m.GetParameters().Length == 6 && m.GetParameters()[1].ParameterType == typeof(Creature));

		private static readonly HashSet<Type> MissingCards = new HashSet<Type>();

		private static ICombatState _state;

		private static Player _player;

		private static Creature _me;

		private static object _room;

		private static int _baseBlock;

		private static decimal _baseEnergy;

		private static int _baseHp;

		private static bool _hadChem;

		private static int _gen;

		private static bool _working;

		private static bool _exitRequested;

		private static bool _expanded;

		private static bool _upgraded;

		private static bool _sounds;

		private static int _soundTab;

		private static bool _slow;

		private static double _normalTimeScale = 1.0;

		private static double _keepAliveTimer;

		private static double _statusTimer;

		private static string _statusText = "";

		private static CanvasLayer _layer;

		private static Label _status;

		private static Font _font;

		private static Vector2 _view;

		public static bool Active { get; private set; }

		internal static bool Working => _working;

		internal static bool TourExitPending => _exitRequested;

		internal static void RequestTourExit()
		{
			try
			{
				RequestExit();
				FreeUi();
			}
			catch (Exception ex)
			{
				DevLog.Warn("TestBench: tour handoff failed: " + ex);
			}
		}

		public static void Init()
		{
			try
			{
				if (Tuning.I("bench.on", 1) != 0)
				{
					Ticker.Add(Update);
					DevLog.Info("TestBench ready: manual card testing in single-player combat");
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("TestBench: init failed: " + ex.Message);
			}
		}

		private static void Update(double dt)
		{
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (TourController.Exclusive)
				{
					FreeUi();
					if (Active && _exitRequested && !_working && Ready())
					{
						RunOperation(Operation.Leave);
					}
					return;
				}
				if (!FightRunning(out var state))
				{
					FreeUi();
					if (_state != null || Active || _working)
					{
						DropState();
					}
					return;
				}
				if (_state != null && (state != _state || GameRefs.CombatRoom != _room))
				{
					FreeUi();
					DropState();
				}
				if (Showcase.Active)
				{
					FreeUi();
					return;
				}
				_statusTimer = Math.Max(0.0, _statusTimer - dt);
				if (_statusTimer == 0.0 && _statusText.Length > 0)
				{
					_statusText = "";
					if (FxKit.Alive((GodotObject)(object)_status))
					{
						_status.Text = "";
					}
				}
				Vector2 val = FxKit.ViewSize();
				if (!FxKit.Alive((GodotObject)(object)_layer) || val != _view)
				{
					BuildUi();
				}
				if (!Active)
				{
					return;
				}
				_keepAliveTimer = Math.Max(0.0, _keepAliveTimer - dt);
				if (!_working && Ready())
				{
					if (_exitRequested)
					{
						RunOperation(Operation.Leave);
					}
					else if (!(_keepAliveTimer > 0.0))
					{
						_keepAliveTimer = 0.25;
						RunOperation(Operation.KeepAlive);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("TestBench: update failed: " + ex.Message);
			}
		}

		private static bool FightRunning(out ICombatState state)
		{
			state = null;
			CombatManager instance = CombatManager.Instance;
			if (GameRefs.CombatRoom == null || instance == null || !instance.IsInProgress || instance.IsOverOrEnding)
			{
				return false;
			}
			state = (ICombatState)(object)instance.DebugOnlyGetState();
			if (state != null)
			{
				return state.Players.Count == 1;
			}
			return false;
		}

		private static bool Bind()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Invalid comparison between Unknown and I4
			CombatManager instance = CombatManager.Instance;
			if (!FightRunning(out _state))
			{
				return false;
			}
			if ((int)_state.CurrentSide != 1 || instance.PlayerActionsDisabled)
			{
				return false;
			}
			_player = LocalContext.GetMe(_state);
			Player player = _player;
			_me = ((player != null) ? player.Creature : null);
			if (_me == null || !_me.IsAlive)
			{
				return false;
			}
			if (_state.HittableEnemies.FirstOrDefault((Creature e) => e != null && e.IsAlive) == null)
			{
				return false;
			}
			if (instance.IsExecutingCardOrPotionEffect(_player))
			{
				return false;
			}
			_room = GameRefs.CombatRoom;
			return true;
		}

		private static bool Ready()
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Invalid comparison between Unknown and I4
			CombatManager instance = CombatManager.Instance;
			if (_player != null && _me != null && _me.IsAlive && _state != null && instance != null && (int)_state.CurrentSide == 1 && !instance.PlayerActionsDisabled)
			{
				return !instance.IsExecutingCardOrPotionEffect(_player);
			}
			return false;
		}

		private static void Check(int gen)
		{
			if (gen != _gen || !Active || !FightRunning(out var state) || state != _state || GameRefs.CombatRoom != _room)
			{
				throw new OperationCanceledException();
			}
		}

		private static async void RunOperation(Operation operation, Type type = null)
		{
			int gen = _gen;
			bool acquired = false;
			try
			{
				if (Showcase.Active || (TourController.Exclusive && operation != Operation.Leave))
				{
					return;
				}
				if (_working || (_exitRequested && operation != Operation.Leave))
				{
					SetStatus((operation == Operation.AddCard) ? "现在不能加牌，稍等" : "正在处理，请稍等");
					return;
				}
				if (operation == Operation.Enter)
				{
					if (Active)
					{
						return;
					}
					if (!Bind())
					{
						_state = null;
						_player = null;
						_me = null;
						_room = null;
						SetStatus("等到自己的回合再点");
						return;
					}
					Snapshot();
					Active = true;
					_expanded = true;
				}
				else
				{
					Check(gen);
					if (!Ready())
					{
						SetStatus((operation == Operation.AddCard) ? "现在不能加牌，稍等" : "等到自己的回合再点");
						return;
					}
				}
				_working = true;
				acquired = true;
				switch (operation)
				{
				case Operation.Enter:
					BuildUi();
					foreach (var item in EnemyHp.ToList())
					{
						Check(gen);
						if (item.Enemy.IsAlive)
						{
							await CreatureCmd.SetMaxAndCurrentHp(item.Enemy, (decimal)Tuning.I("bench.enemy_hp", 9999));
						}
					}
					Check(gen);
					SetStatus("测试已开启：选牌加入手牌，照常出牌和结束回合");
					break;
				case Operation.Leave:
					await Restore(gen);
					Check(gen);
					DropState();
					BuildUi();
					SetStatus("已退出测试，战斗已还原");
					break;
				case Operation.AddCard:
					await Spawn(gen, type);
					break;
				case Operation.ClearHand:
				{
					List<CardModel> list = (from c in HandCards()
						where IsShiv(c) || Spawned.Contains(c)
						select c).ToList();
					if (list.Count > 0)
					{
						await CardPileCmd.RemoveFromCombat((IEnumerable<CardModel>)list, false);
					}
					Check(gen);
					SetStatus("手中的测试牌和小刀已清空");
					break;
				}
				case Operation.ClearPowers:
					await ClearAddedPowers(gen);
					await DropExtraBlock(gen);
					Check(gen);
					SetStatus("能力与额外格挡已还原，可以重看开场");
					break;
				case Operation.Weak:
					await ApplyToEnemies<WeakPower>(gen, 3);
					Check(gen);
					SetStatus("敌人已增加3层虚弱");
					break;
				case Operation.Poison:
					await ApplyToEnemies<PoisonPower>(gen, 5);
					Check(gen);
					SetStatus("敌人已增加5层中毒");
					break;
				case Operation.Guard:
				{
					int block = Math.Max(1, Tuning.I("bench.block", 12));
					await ApplyToEnemies<ArtifactPower>(gen, 1);
					Check(gen);
					foreach (Creature item2 in _state.HittableEnemies.ToList())
					{
						Check(gen);
						if (item2 != null && item2.IsAlive)
						{
							await CreatureCmd.GainBlock(item2, (decimal)block, (ValueProp)4, (CardPlay)null, true);
						}
					}
					Check(gen);
					SetStatus("敌人已增加1层人工制品和" + block + "点格挡");
					break;
				}
				case Operation.LowHp:
					foreach (Creature item3 in _state.Enemies.ToList())
					{
						Check(gen);
						if (item3.IsAlive)
						{
							await CreatureCmd.SetMaxAndCurrentHp(item3, (decimal)Tuning.I("bench.low_hp", 8));
						}
					}
					Check(gen);
					SetStatus("敌人只剩一口气：狩猎瞄准它会出金色标记（退出测试即还原）");
					break;
				case Operation.Chem:
				{
					ChemicalX chem = _player.GetRelic<ChemicalX>();
					if (chem == null)
					{
						await RelicCmd.Obtain<ChemicalX>(_player);
					}
					else
					{
						await RelicCmd.Remove((RelicModel)(object)chem);
					}
					Check(gen);
					BuildUi();
					SetStatus((chem != null) ? "化合物X已移除" : "已获得化合物X：X 牌 +2（退出测试即还原）");
					break;
				}
				case Operation.KeepAlive:
					await KeepAlive(gen);
					break;
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex2)
			{
				DevLog.Warn("TestBench: " + operation.ToString() + " failed: " + ex2.Message);
				try
				{
					if (gen == _gen)
					{
						if (operation == Operation.Leave)
						{
							_exitRequested = false;
						}
						SetStatus("操作失败，请稍后重试（详见日志）");
					}
				}
				catch (Exception ex3)
				{
					DevLog.Warn("TestBench: status failed: " + ex3.Message);
				}
			}
			finally
			{
				if (acquired && gen == _gen)
				{
					_working = false;
				}
			}
		}

		private static void Snapshot()
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			ClearSnapshot();
			DrawOrder.AddRange(PileTypeExtensions.GetPile((PileType)1, _player).Cards);
			foreach (CardModel allCard in _player.PlayerCombatState.AllCards)
			{
				Dictionary<CardModel, PileType> home = Home;
				CardPile pile = allCard.Pile;
				home[allCard] = (PileType)((pile != null) ? ((int)pile.Type) : 0);
				if (allCard.EnergyCost.HasLocalModifiers)
				{
					HadCostChange.Add(allCard);
				}
			}
			_baseBlock = _me.Block;
			_baseEnergy = _player.PlayerCombatState.Energy;
			_baseHp = _me.CurrentHp;
			_hadChem = HasChem();
			foreach (Creature creature in _state.Creatures)
			{
				foreach (PowerModel power in creature.Powers)
				{
					Baseline.Add(power);
					BasePowers.Add((creature, ((object)power).GetType(), power.Amount));
				}
			}
			foreach (Creature enemy in _state.Enemies)
			{
				if (enemy.IsAlive)
				{
					EnemyHp.Add((enemy, enemy.MaxHp, enemy.CurrentHp));
				}
			}
		}

		private static async Task KeepAlive(int gen)
		{
			Check(gen);
			int num = Tuning.I("bench.energy", 9);
			if (_player.PlayerCombatState.Energy < num)
			{
				await PlayerCmd.SetEnergy((decimal)num, _player);
			}
			Check(gen);
			foreach (Creature item in _state.Enemies.ToList())
			{
				Check(gen);
				if (!Ready())
				{
					return;
				}
				if (item.IsAlive && (decimal)item.CurrentHp < (decimal)item.MaxHp / 2m)
				{
					await CreatureCmd.SetCurrentHp(item, (decimal)item.MaxHp);
				}
			}
			Check(gen);
			if (Ready() && _me.CurrentHp < _me.MaxHp)
			{
				await CreatureCmd.SetCurrentHp(_me, (decimal)_me.MaxHp);
			}
		}

		private static async Task Spawn(int gen, Type type)
		{
			Check(gen);
			if (HandCards().Count >= CardPile.MaxCardsInHand)
			{
				SetStatus("手牌满了：先打出几张，或点“清空测试牌”");
				return;
			}
			CardModel canonical = Canonical(type);
			if (canonical != null)
			{
				CardModel card = _state.CreateCard(canonical, _player);
				Spawned.Add(card);
				if (_upgraded && card.IsUpgradable)
				{
					card.UpgradeInternal();
					card.FinalizeUpgradeInternal();
				}
				await CardPileCmd.Add(card, (PileType)2, (CardPilePosition)1, (AbstractModel)null, false);
				Check(gen);
				SetStatus("已加入：" + canonical.Title + ((_upgraded && card.IsUpgraded) ? "（升级版）" : ""));
			}
		}

		private static CardModel Canonical(Type type)
		{
			CardModel? obj = ((type == null) ? null : ModelDb.AllCards.FirstOrDefault((CardModel c) => ((object)c).GetType() == type));
			if (obj == null && type != null && MissingCards.Add(type))
			{
				DevLog.Warn("TestBench: no such card " + type.Name);
			}
			return obj;
		}

		private static bool IsShiv(CardModel card)
		{
			if (card != null && card.Tags != null)
			{
				return card.Tags.Contains((CardTag)5);
			}
			return false;
		}

		private static bool HasChem()
		{
			Player player = _player;
			return ((player != null) ? player.GetRelic<ChemicalX>() : null) != null;
		}

		private static List<CardModel> HandCards()
		{
			return PileTypeExtensions.GetPile((PileType)2, _player).Cards.ToList();
		}

		private static async Task ApplyToEnemies<T>(int gen, int amount) where T : PowerModel
		{
			Check(gen);
			foreach (Creature item in _state.HittableEnemies.ToList())
			{
				Check(gen);
				if (item != null && item.IsAlive)
				{
					await PowerCmd.Apply<T>((PlayerChoiceContext)new ThrowingPlayerChoiceContext(), item, (decimal)amount, _me, (CardModel)null, false);
				}
			}
		}

		private static async Task DropExtraBlock(int gen)
		{
			Check(gen);
			int num = _me.Block - _baseBlock;
			if (num > 0)
			{
				await RuntimeApiCompatibility.LoseBlock((PlayerChoiceContext)new ThrowingPlayerChoiceContext(), _me, num);
			}
		}

		private static async Task RestoreCards(int gen)
		{
			Check(gen);
			List<CardModel> list = _player.PlayerCombatState.AllCards.Where((CardModel c) => !Home.ContainsKey(c)).ToList();
			if (list.Count > 0)
			{
				await CardPileCmd.RemoveFromCombat((IEnumerable<CardModel>)list, false);
			}
			Check(gen);
			foreach (KeyValuePair<CardModel, PileType> item in Home.OrderBy((KeyValuePair<CardModel, PileType> p) =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Invalid comparison between Unknown and I4
				return ((int)p.Value == 2) ? 1 : 0;
			}).ToList())
			{
				Check(gen);
				CardModel card = item.Key;
				CardPile pile = card.Pile;
				PileType val = (PileType)((pile != null) ? ((int)pile.Type) : 0);
				if ((int)val != 0 && (int)item.Value != 0 && (int)item.Value != 5)
				{
					if (val != item.Value)
					{
						await CardPileCmd.Add(card, item.Value, (CardPilePosition)1, (AbstractModel)null, false);
					}
					Check(gen);
					if (!HadCostChange.Contains(card) && card.EnergyCost.HasLocalModifiers)
					{
						card.EnergyCost.EndOfTurnCleanup();
						card.InvokeEnergyCostChanged();
					}
				}
			}
			CardPile pile2 = PileTypeExtensions.GetPile((PileType)1, _player);
			foreach (CardModel item2 in DrawOrder)
			{
				if (pile2.Cards.Contains(item2))
				{
					pile2.MoveToBottomInternal(item2);
				}
			}
		}

		private static async Task RestoreBasePowers(int gen)
		{
			Check(gen);
			foreach (var entry in BasePowers.ToList())
			{
				Check(gen);
				if (!entry.Owner.IsAlive)
				{
					continue;
				}
				PowerModel val = entry.Owner.Powers.FirstOrDefault((PowerModel p) => ((object)p).GetType() == entry.Power);
				if (val == null)
				{
					if (ApplyByType == null)
					{
						continue;
					}
					if (ApplyByType.MakeGenericMethod(entry.Power).Invoke(null, new object[6]
					{
						(object)new ThrowingPlayerChoiceContext(),
						entry.Owner,
						(decimal)entry.Amount,
						null,
						null,
						true
					}) is Task task)
					{
						await task;
					}
					Check(gen);
					val = entry.Owner.Powers.FirstOrDefault((PowerModel p) => ((object)p).GetType() == entry.Power);
					if (val == null)
					{
						continue;
					}
					Baseline.Add(val);
				}
				if (val.Amount != entry.Amount)
				{
					val.SetAmount(entry.Amount, true);
				}
			}
		}

		private static async Task ClearAddedPowers(int gen)
		{
			Check(gen);
			foreach (Creature item in _state.Creatures.ToList())
			{
				Check(gen);
				foreach (PowerModel item2 in item.Powers.ToList())
				{
					Check(gen);
					if (!Baseline.Contains(item2))
					{
						await PowerCmd.Remove(item2);
					}
				}
			}
			await RestoreBasePowers(gen);
		}

		private static async Task Restore(int gen)
		{
			Check(gen);
			await ClearAddedPowers(gen);
			await DropExtraBlock(gen);
			await RestoreCards(gen);
			Check(gen);
			ChemicalX relic = _player.GetRelic<ChemicalX>();
			if (relic != null && !_hadChem)
			{
				await RelicCmd.Remove((RelicModel)(object)relic);
			}
			else if (relic == null && _hadChem)
			{
				await RelicCmd.Obtain<ChemicalX>(_player);
			}
			Check(gen);
			foreach (var entry in EnemyHp.ToList())
			{
				Check(gen);
				if (entry.Enemy.IsAlive)
				{
					await CreatureCmd.SetMaxAndCurrentHp(entry.Enemy, (decimal)entry.Max);
					Check(gen);
					await CreatureCmd.SetCurrentHp(entry.Enemy, (decimal)entry.Current);
				}
			}
			Check(gen);
			await PlayerCmd.SetEnergy(_baseEnergy, _player);
			Check(gen);
			await CreatureCmd.SetCurrentHp(_me, (decimal)_baseHp);
		}

		private static void RequestExit()
		{
			if (Active)
			{
				_exitRequested = true;
				SetStatus("正在等待空闲的己方回合，然后还原战斗");
			}
		}

		private static void ClearSnapshot()
		{
			Baseline.Clear();
			Spawned.Clear();
			EnemyHp.Clear();
			Home.Clear();
			HadCostChange.Clear();
			DrawOrder.Clear();
			BasePowers.Clear();
		}

		private static void DropState()
		{
			_gen++;
			Active = false;
			_working = false;
			_exitRequested = false;
			_expanded = false;
			_upgraded = false;
			RestoreScale();
			ClearSnapshot();
			_state = null;
			_player = null;
			_me = null;
			_room = null;
			_baseBlock = 0;
			_baseEnergy = 0m;
			_baseHp = 0;
			_hadChem = false;
			_keepAliveTimer = 0.0;
			_statusTimer = 0.0;
			_statusText = "";
		}

		private static void RestoreScale()
		{
			if (_slow)
			{
				Engine.TimeScale = _normalTimeScale;
			}
			_slow = false;
		}

		private static void ToggleSlow()
		{
			if (Active)
			{
				if (_slow)
				{
					RestoreScale();
				}
				else
				{
					_normalTimeScale = Engine.TimeScale;
					Engine.TimeScale = Mathf.Clamp(Tuning.F("bench.slow", 0.3f), 0.05f, 1f);
					_slow = true;
				}
				BuildUi();
			}
		}

		private static Font CaptionFont()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected Obj, but got Unknown
			if (_font == null)
			{
				SystemFont val = new SystemFont();
				val.FontNames = new string[6] { "Microsoft YaHei UI", "Microsoft YaHei", "SimHei", "PingFang SC", "Noto Sans CJK SC", "sans-serif" };
				val.FontWeight = 600;
				_font = (Font)(object)val;
			}
			return _font;
		}

		private static void FreeUi()
		{
			if (FxKit.Alive((GodotObject)(object)_layer))
			{
				((Node)_layer).QueueFree();
			}
			_layer = null;
			_status = null;
		}

		private static void SetStatus(string text)
		{
			_statusText = text;
			_statusTimer = 3.0;
			if (FxKit.Alive((GodotObject)(object)_status))
			{
				_status.Text = text;
			}
		}

		private static Label MakeLabel(float x, float y, float w, float h, Color color)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Expected Obj, but got Unknown
			//IL_008b: Expected Obj, but got Unknown
			Label val = new Label
			{
				Position = new Vector2(x, y),
				Size = new Vector2(w, h),
				HorizontalAlignment = (HorizontalAlignment)0,
				VerticalAlignment = (VerticalAlignment)1,
				ClipText = true,
				MouseFilter = (MouseFilterEnum)2,
				LabelSettings = new LabelSettings
				{
					Font = CaptionFont(),
					FontSize = 17,
					FontColor = color,
					OutlineSize = 6,
					OutlineColor = new Color(0f, 0f, 0f, 0.85f)
				}
			};
			((Node)_layer).AddChild((Node)(object)val, false, (InternalMode)0);
			return val;
		}

		private static StyleBoxFlat ButtonStyle(Color color)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected Obj, but got Unknown
			return new StyleBoxFlat
			{
				BgColor = color,
				CornerRadiusTopLeft = 4,
				CornerRadiusTopRight = 4,
				CornerRadiusBottomLeft = 4,
				CornerRadiusBottomRight = 4,
				ContentMarginLeft = 8f,
				ContentMarginRight = 8f,
				ContentMarginTop = 3f,
				ContentMarginBottom = 3f
			};
		}

		private static Button MakeButton(string text, float x, float y, float w, float h, Color color, Action pressed)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected Obj, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			Button val = new Button
			{
				Text = text,
				Position = new Vector2(x, y),
				Size = new Vector2(w, h),
				FocusMode = (FocusModeEnum)0,
				MouseFilter = (MouseFilterEnum)0
			};
			((Control)val).AddThemeFontOverride(StringName.op_Implicit("font"), CaptionFont());
			((Control)val).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 17);
			((Control)val).AddThemeColorOverride(StringName.op_Implicit("font_color"), color);
			((Control)val).AddThemeColorOverride(StringName.op_Implicit("font_hover_color"), color);
			((Control)val).AddThemeColorOverride(StringName.op_Implicit("font_pressed_color"), color);
			((Control)val).AddThemeStyleboxOverride(StringName.op_Implicit("normal"), (StyleBox)(object)ButtonStyle(new Color(0.07f, 0.13f, 0.13f, 0.95f)));
			((Control)val).AddThemeStyleboxOverride(StringName.op_Implicit("hover"), (StyleBox)(object)ButtonStyle(new Color(0.13f, 0.24f, 0.22f, 1f)));
			((Control)val).AddThemeStyleboxOverride(StringName.op_Implicit("pressed"), (StyleBox)(object)ButtonStyle(new Color(0.03f, 0.09f, 0.08f, 1f)));
			CanvasLayer layer = _layer;
			((BaseButton)val).Pressed += () =>
			{
				try
				{
					if (layer == _layer && !Showcase.Active && FightRunning(out var _))
					{
						pressed();
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("TestBench: button failed: " + ex.Message);
				}
			};
			((Node)_layer).AddChild((Node)(object)val, false, (InternalMode)0);
			return val;
		}

		private static float ButtonWidth(string text)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return Mathf.Ceil(CaptionFont().GetStringSize(text, (HorizontalAlignment)0, -1f, 17, (JustificationFlag)3, (Direction)0, (Orientation)0).X) + 16f;
		}

		private static void BuildUi()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected Obj, but got Unknown
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Expected Obj, but got Unknown
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0492: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_050a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0525: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_1381: Unknown result type (might be due to invalid IL or missing references)
			//IL_1543: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0768: Unknown result type (might be due to invalid IL or missing references)
			//IL_0762: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_1464: Unknown result type (might be due to invalid IL or missing references)
			//IL_1469: Unknown result type (might be due to invalid IL or missing references)
			//IL_146b: Unknown result type (might be due to invalid IL or missing references)
			//IL_146e: Invalid comparison between Unknown and I4
			//IL_14a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_14ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_1470: Unknown result type (might be due to invalid IL or missing references)
			//IL_1473: Invalid comparison between Unknown and I4
			//IL_14b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_14b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_14d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_14b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_14b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_1489: Unknown result type (might be due to invalid IL or missing references)
			//IL_148e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0832: Unknown result type (might be due to invalid IL or missing references)
			//IL_082c: Unknown result type (might be due to invalid IL or missing references)
			//IL_132c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e6e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0871: Unknown result type (might be due to invalid IL or missing references)
			//IL_086b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0892: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ed5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ecc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f13: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f0d: Unknown result type (might be due to invalid IL or missing references)
			//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0f69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0fbc: Unknown result type (might be due to invalid IL or missing references)
			//IL_10d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_10f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_111a: Unknown result type (might be due to invalid IL or missing references)
			//IL_113b: Unknown result type (might be due to invalid IL or missing references)
			//IL_118e: Unknown result type (might be due to invalid IL or missing references)
			FreeUi();
			SceneTree tree = Ticker.Tree;
			if (tree == null || Showcase.Active || TourController.Exclusive)
			{
				return;
			}
			_layer = new CanvasLayer
			{
				Name = StringName.op_Implicit("SilentFX_TestBench"),
				Layer = 96
			};
			((Node)tree.Root).AddChild((Node)(object)_layer, false, (InternalMode)0);
			_view = FxKit.ViewSize();
			float num = Tuning.F("bench.x", 16f);
			float num2 = Tuning.F("bench.y", 158f);
			Color white = new Color(0.94f, 0.96f, 0.94f, 1f);
			if (!Active || !_expanded)
			{
				string text = (Active ? "展开测试面板" : "测试");
				float num3 = (Active ? ButtonWidth(text) : 84f);
				MakeButton(text, num, num2, num3, 34f, white, () =>
				{
					if (!Active)
					{
						RunOperation(Operation.Enter);
					}
					else
					{
						_expanded = true;
						BuildUi();
					}
				});
				_status = MakeLabel(num + num3 + 8f, num2, Math.Max(0f, _view.X - num - num3 - 24f), 34f, white);
				_status.Text = _statusText;
				return;
			}
			float left = 16f;
			float num4 = _view.X - 16f;
			ColorRect val = new ColorRect
			{
				Color = new Color(0.02f, 0.05f, 0.05f, 0.78f),
				Position = new Vector2(left, num2),
				MouseFilter = (MouseFilterEnum)2
			};
			((Node)_layer).AddChild((Node)(object)val, false, (InternalMode)0);
			float rowY = num2 + 4f;
			float cursor = left + 4f;
			float end = num4 - 4f;
			Utility("收起", () =>
			{
				_expanded = false;
				BuildUi();
			});
			Utility("退出测试", RequestExit);
			Utility("清空测试牌", () =>
			{
				RunOperation(Operation.ClearHand);
			});
			Utility("清除能力（重看开场）", () =>
			{
				RunOperation(Operation.ClearPowers);
			});
			Utility("敌人 +3 虚弱", () =>
			{
				RunOperation(Operation.Weak);
			});
			Utility("敌人 +5 中毒", () =>
			{
				RunOperation(Operation.Poison);
			});
			Utility("敌人 +1 人工制品 +12 格挡", () =>
			{
				RunOperation(Operation.Guard);
			});
			Utility("敌人残血", () =>
			{
				RunOperation(Operation.LowHp);
			});
			Utility(HasChem() ? "化合物X：开" : "化合物X：关", () =>
			{
				RunOperation(Operation.Chem);
			});
			Utility(_slow ? "慢动作：开" : "慢动作：关", ToggleSlow);
			Utility(_upgraded ? "升级版：开" : "升级版：关", () =>
			{
				_upgraded = !_upgraded;
				BuildUi();
			});
			Utility(_sounds ? "回到选牌" : "选音效", () =>
			{
				_sounds = !_sounds;
				if (_sounds)
				{
					SetStatus("点一下就试听并且就用它（金色 = 现在用的）。调完点“回到选牌”打牌听实际效果");
				}
				BuildUi();
			});
			if (end - cursor < 260f)
			{
				cursor = left + 4f;
				rowY += 34f;
			}
			_status = MakeLabel(cursor, rowY, Math.Max(0f, end - cursor), 30f, white);
			_status.Text = _statusText;
			rowY += 34f;
			Color gold;
			Color green;
			float at;
			if (_sounds)
			{
				gold = new Color(1f, 0.84f, 0.42f, 1f);
				Color val2 = new Color(0.62f, 0.66f, 0.64f, 1f);
				green = new Color(0.62f, 0.92f, 0.72f, 1f);
				at = left + 4f;
				(string, float)[] steps = new (string, float)[5]
				{
					("100", 1f),
					("75", 0.75f),
					("55", 0.55f),
					("40", 0.4f),
					("25", 0.25f)
				};
				(string, float)[] steps2 = new (string, float)[4]
				{
					("关", 0f),
					("轻", 0.3f),
					("中", 0.55f),
					("重", 0.8f)
				};
				(string, float)[] steps3 = new (string, float)[4]
				{
					("关", 0f),
					("轻", 0.2f),
					("中", 0.4f),
					("重", 0.65f)
				};
				at = left + 4f;
				string[] array = new string[5] { "处理音乐", "谋杀 / 幽魂 / 已定的", "尖啸 / 刺杀 / 终结技 / 暗影步 / 洗牌", "毒 / 狩猎 / 铭记 / 蛇咬 / 陷阱 / 切击 / 跟踪 / 飞镖", "新配音" };
				for (int num5 = 0; num5 < array.Length; num5++)
				{
					int tab = num5;
					Press(array[num5], (_soundTab == num5) ? gold : white, () =>
					{
						_soundTab = tab;
						BuildUi();
					});
				}
				at += 20f;
				bool sfxOn = Sfx.Knob("sfx.on", 1f) > 0.5f;
				Press(sfxOn ? "模组加的声音：开" : "模组加的声音：关", sfxOn ? gold : val2, () =>
				{
					Sfx.SetKnob("sfx.on", sfxOn ? 0f : 1f);
					if (sfxOn)
					{
						Sfx.Reset();
					}
					BuildUi();
				});
				rowY += 38f;
				if (_soundTab == 0)
				{
					at = left + 4f;
					Caption("处理游戏的音乐", 150f, green);
					bool moodOn = Sfx.Knob("mood.on", 1f) > 0.5f;
					bool dspOn = Sfx.Knob("mood.dsp", 1f) > 0.5f;
					Press(moodOn ? "总开关：开" : "总开关：关", moodOn ? gold : val2, () =>
					{
						Sfx.SetKnob("mood.on", moodOn ? 0f : 1f);
						if (moodOn)
						{
							Mood.Reset();
						}
						BuildUi();
					});
					Press(dspOn ? "变远/变空：开" : "变远/变空：关", dspOn ? gold : val2, () =>
					{
						Sfx.SetKnob("mood.dsp", dspOn ? 0f : 1f);
						BuildUi();
					});
					Press("看现在的状态", white, () =>
					{
						SetStatus(Mood.Report());
					});
					Caption("试听 = 现在就把音乐处理 3 秒；数字越小压得越低", 520f, val2);
					rowY += 34f;
					Row("谋杀·瞄准时", () =>
					{
						Mood.Hold("try", Sfx.Knob("mood.murder.volume", 0.55f), 1f, Sfx.Knob("mood.murder.far", 0.55f), Sfx.Knob("mood.murder.hollow", 0.35f), Sfx.Knob("mood.murder.in", 0.6f), Sfx.Knob("mood.murder.back", 0.45f), 3f);
						SetStatus(Mood.HasMusic ? "谋杀·瞄准：音乐处理 3 秒" : Mood.Report());
					});
					Dial("音量", "mood.murder.volume", 0.55f, steps);
					Dial("变远", "mood.murder.far", 0.55f, steps2);
					Dial("变空", "mood.murder.hollow", 0.35f, steps3);
					rowY += 34f;
					Row("谋杀·定格瞬间", () =>
					{
						Mood.Hold("try", Sfx.Knob("mood.murder.volume", 0.55f), 1f, Sfx.Knob("mood.murder.far", 0.55f), Sfx.Knob("mood.murder.hollow", 0.35f), 0.4f, Sfx.Knob("mood.murder.back", 0.45f), 1.5f);
						FxKit.After((Node)(object)_layer, 1.2, () =>
						{
							Mood.Hold("try_stop", Sfx.Knob("mood.murder.stop_volume", 0.06f), 1f, Sfx.Knob("mood.murder.far", 0.55f), Sfx.Knob("mood.murder.hollow", 0.35f), 0.03f, Sfx.Knob("mood.murder.back", 0.45f), Sfx.Knob("mood.murder.stop_try", 0.22f));
						});
						SetStatus(Mood.HasMusic ? "谋杀·定格：先压 1.2 秒，然后静 0.2 秒，再放回来" : Mood.Report());
					});
					Dial("定格时音量", "mood.murder.stop_volume", 0.06f, new (string, float)[4]
					{
						("不变", 1f),
						("30", 0.3f),
						("6", 0.06f),
						("0", 0f)
					});
					Dial("放回来用", "mood.murder.back", 0.45f, new (string, float)[3]
					{
						("0.2秒", 0.2f),
						("0.45秒", 0.45f),
						("0.8秒", 0.8f)
					});
					rowY += 34f;
					Row("子弹时间", () =>
					{
						Mood.Hold("try", Sfx.Knob("mood.bullet.volume", 0.6f), Sfx.Knob("mood.bullet.pitch", 0.85f), Sfx.Knob("mood.bullet.far", 0.5f), Sfx.Knob("mood.bullet.hollow", 0.2f), Sfx.Knob("mood.bullet.in", 0.55f), Sfx.Knob("mood.bullet.back", 0.8f), 3f);
						SetStatus(Mood.HasMusic ? "子弹时间：音乐处理 3 秒" : Mood.Report());
					});
					Dial("音量", "mood.bullet.volume", 0.6f, new (string, float)[4]
					{
						("100", 1f),
						("80", 0.8f),
						("60", 0.6f),
						("45", 0.45f)
					});
					Dial("速度", "mood.bullet.pitch", 0.85f, new (string, float)[6]
					{
						("100", 1f),
						("95", 0.95f),
						("90", 0.9f),
						("85", 0.85f),
						("75", 0.75f),
						("60", 0.6f)
					});
					Dial("变远", "mood.bullet.far", 0.5f, steps2);
					Dial("变空", "mood.bullet.hollow", 0.2f, steps3);
					rowY += 34f;
					Row("幽魂形态", () =>
					{
						Mood.Hold("try", Sfx.Knob("mood.wraith.volume", 0.45f), 1f, Sfx.Knob("mood.wraith.far", 0.35f), Sfx.Knob("mood.wraith.hollow", 0.45f), 0.35f, Sfx.Knob("mood.wraith.back", 0.9f), 3f);
						SetStatus(Mood.HasMusic ? "幽魂形态：音乐处理 3 秒" : Mood.Report());
					});
					Dial("音量", "mood.wraith.volume", 0.45f, new (string, float)[4]
					{
						("100", 1f),
						("70", 0.7f),
						("45", 0.45f),
						("30", 0.3f)
					});
					Dial("变远", "mood.wraith.far", 0.35f, steps2);
					Dial("变空", "mood.wraith.hollow", 0.45f, steps3);
					rowY += 34f;
					Row("刺杀·瞄准时", () =>
					{
						Mood.Hold("try", Sfx.Knob("mood.assassinate.volume", 0.75f), 1f, Sfx.Knob("mood.assassinate.far", 0.3f), Sfx.Knob("mood.assassinate.hollow", 0.2f), 0.5f, Sfx.Knob("mood.assassinate.back", 0.5f), 3f);
						SetStatus(Mood.HasMusic ? "刺杀·瞄准：音乐处理 3 秒" : Mood.Report());
					});
					Dial("音量", "mood.assassinate.volume", 0.75f, new (string, float)[4]
					{
						("100", 1f),
						("85", 0.85f),
						("75", 0.75f),
						("55", 0.55f)
					});
					Dial("变远", "mood.assassinate.far", 0.3f, steps2);
					Dial("变空", "mood.assassinate.hollow", 0.2f, steps3);
					rowY += 38f;
				}
				else
				{
					Sfx.Slot[] slots = Sfx.Slots;
					foreach (Sfx.Slot slot in slots)
					{
						if (slot.Fixed || slot.Page != _soundTab)
						{
							continue;
						}
						at = left + 4f;
						bool flag = Sfx.HasFile(slot.Name);
						Caption(slot.Label + (flag ? "（文件）" : ""), 190f, green);
						int num7 = Sfx.Chosen(slot.Name);
						string name = slot.Name;
						for (int num8 = 0; num8 < slot.Options.Length; num8++)
						{
							int pick = num8;
							Press(slot.Options[num8].Label, (num7 == num8) ? gold : white, () =>
							{
								SetStatus(Sfx.Choose(name, pick));
								BuildUi();
							});
						}
						Press("不配", (num7 < 0) ? gold : val2, () =>
						{
							SetStatus(Sfx.Choose(name, -1));
							BuildUi();
						});
						at += 10f;
						Press("小声点", white, () =>
						{
							SetStatus(name + " 音量 ×" + Sfx.Nudge(name, 0.7f).ToString("0.00"));
							BuildUi();
						});
						Press("大声点", white, () =>
						{
							SetStatus(name + " 音量 ×" + Sfx.Nudge(name, 1.4f).ToString("0.00"));
							BuildUi();
						});
						Caption("×" + Sfx.Knob("sfx." + name, 1f).ToString("0.00"), 70f, val2);
						rowY += 34f;
					}
					rowY += 4f;
					int num9 = 0;
					slots = Sfx.Slots;
					foreach (Sfx.Slot slot2 in slots)
					{
						if (!slot2.Fixed || slot2.Page != _soundTab)
						{
							continue;
						}
						at = ((num9 % 2 == 0) ? (left + 4f) : (left + 4f + (end - left) * 0.5f));
						string name2 = slot2.Name;
						string text2 = (slot2.Settled ? "已定：" : "") + slot2.Label + (Sfx.HasFile(name2) ? "（文件）" : "");
						Caption(text2, ButtonWidth(text2) - 8f, green);
						Press("试听", new Color(0.7f, 0.9f, 1f, 1f), () =>
						{
							SetStatus(Sfx.Audition(name2));
						});
						Press("小声点", white, () =>
						{
							SetStatus(name2 + " 音量 ×" + Sfx.Nudge(name2, 0.7f).ToString("0.00"));
							BuildUi();
						});
						Press("大声点", white, () =>
						{
							SetStatus(name2 + " 音量 ×" + Sfx.Nudge(name2, 1.4f).ToString("0.00"));
							BuildUi();
						});
						Caption("×" + Sfx.Knob("sfx." + name2, 1f).ToString("0.00"), 62f, val2);
						if (slot2.Every > 0f)
						{
							if (slot2.Every < 2f)
							{
								Dial("隔", "sfx." + name2 + ".every", slot2.Every, new (string, float)[4]
								{
									("0.35秒", 0.35f),
									("0.55秒", 0.55f),
									("0.8秒", 0.8f),
									("1.2秒", 1.2f)
								});
							}
							else
							{
								Dial("隔", "sfx." + name2 + ".every", slot2.Every, new (string, float)[4]
								{
									("3秒", 3f),
									("6秒", 6f),
									("10秒", 10f),
									("16秒", 16f)
								});
							}
						}
						num9++;
						if (num9 % 2 == 0)
						{
							rowY += 34f;
						}
					}
					if (num9 % 2 != 0)
					{
						rowY += 34f;
					}
				}
				((Control)val).Size = new Vector2(num4 - left, rowY - num2);
				return;
			}
			(string, Type[])[] groups = Groups;
			for (int num6 = 0; num6 < groups.Length; num6++)
			{
				(string, Type[]) tuple = groups[num6];
				MakeLabel(left + 4f, rowY, 96f, 30f, new Color(0.62f, 0.92f, 0.72f, 1f)).Text = tuple.Item1;
				cursor = left + 104f;
				Type[] item = tuple.Item2;
				foreach (Type type in item)
				{
					if (type == null)
					{
						continue;
					}
					CardModel val3 = Canonical(type);
					if (val3 != null)
					{
						string text3 = val3.Title;
						if (string.IsNullOrEmpty(text3))
						{
							text3 = "未命名卡牌";
						}
						float num11 = ButtonWidth(text3);
						if (cursor + num11 > end && cursor > left + 104f)
						{
							rowY += 34f;
							cursor = left + 104f;
						}
						CardRarity rarity = val3.Rarity;
						Color val4;
						if ((int)rarity == 3)
						{
							val4 = new Color(0.62f, 0.86f, 1f, 1f);
						}
						else
						{
							val4 = (((int)rarity != 4) ? white : new Color(1f, 0.84f, 0.42f, 1f));
						}
						Color color = val4;
						MakeButton(text3, cursor, rowY, num11, 30f, color, () =>
						{
							RunOperation(Operation.AddCard, type);
						});
						cursor += num11 + 4f;
					}
				}
				rowY += 34f;
			}
			((Control)val).Size = new Vector2(num4 - left, rowY - num2);
			void Caption(string text4, float width, Color color2)
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				MakeLabel(at, rowY, width, 30f, color2).Text = text4;
				at += width + 4f;
			}
			void Dial(string caption, string key, float def, (string Text, float Value)[] array2)
			{
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				Caption(caption, ButtonWidth(caption) - 8f, white);
				float num12 = Sfx.Knob(key, def);
				for (int i = 0; i < array2.Length; i++)
				{
					(string, float) tuple2 = array2[i];
					float value = tuple2.Item2;
					Press(tuple2.Item1, (Mathf.Abs(num12 - value) < 0.011f) ? gold : white, () =>
					{
						Sfx.SetKnob(key, value);
						BuildUi();
					});
				}
				at += 10f;
			}
			void Press(string text4, Color color2, Action action)
			{
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				float num12 = ButtonWidth(text4);
				MakeButton(text4, at, rowY, num12, 30f, color2, action);
				at += num12 + 4f;
			}
			void Row(string label, Action listen)
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				at = left + 4f;
				Caption(label, 150f, green);
				Press("试听", new Color(0.7f, 0.9f, 1f, 1f), listen);
			}
			void Utility(string text4, Action action)
			{
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				float num12 = ButtonWidth(text4);
				if (cursor + num12 > end && cursor > left + 4f)
				{
					cursor = left + 4f;
					rowY += 34f;
				}
				MakeButton(text4, cursor, rowY, num12, 30f, white, action);
				cursor += num12 + 4f;
			}
		}
	}
