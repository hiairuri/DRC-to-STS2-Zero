// namespace: SilentFX.Dev
// type: Showcase (class)
// source lines: 24351-25724
	public static class Showcase
	{
		private sealed class Item
		{
			public string Group;

			public Type Card;

			public string Title;

			public string Note;

			public bool Persistent;

			public bool Targeted;

			public float Hold = 0.8f;

			public float Slow;

			public Func<int, Task> Setup;

			public Func<int, Task> Apply;

			public Func<int, Task> Trigger;

			public Func<int, Task> Rearm;

			public string TriggerNote;

			public Func<int, Task> After;
		}

		private sealed class AutoSelector : ICardSelector
		{
			public Task<IEnumerable<CardModel>> GetSelectedCards(IEnumerable<CardModel> options, int minSelect, int maxSelect)
			{
				return Task.FromResult((IEnumerable<CardModel>)options.Take(Math.Max(0, minSelect)).ToList());
			}

			public CardRewardSelection GetSelectedCardReward(IReadOnlyList<CardCreationResult> options, IReadOnlyList<CardRewardAlternative> alternatives)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				return default;
			}
		}

		private static readonly List<Item> Items = new List<Item>();

		private static readonly Dictionary<Key, bool> Held = new Dictionary<Key, bool>();

		private static readonly List<TaskCompletionSource<bool>> Waiters = new List<TaskCompletionSource<bool>>();

		private static readonly HashSet<PowerModel> Baseline = new HashSet<PowerModel>(ReferenceEqualityComparer.Instance);

		private static readonly List<CardModel> Spawned = new List<CardModel>();

		private static readonly List<(Creature Enemy, int Max, int Current)> EnemyHp = new List<(Creature, int, int)>();

		private static ICombatState _state;

		private static Player _player;

		private static Creature _me;

		private static int _gen;

		private static int _index;

		private static bool _prepared;

		private static bool _paused;

		private static double _wantScale = 1.0;

		private static float _frameDt;

		private static readonly Dictionary<CardModel, PileType> Home = new Dictionary<CardModel, PileType>(ReferenceEqualityComparer.Instance);

		private static readonly HashSet<CardModel> HadCostChange = new HashSet<CardModel>(ReferenceEqualityComparer.Instance);

		private static readonly List<CardModel> DrawOrder = new List<CardModel>();

		private static readonly List<(Creature Owner, Type Power, int Amount)> BasePowers = new List<(Creature, Type, int)>();

		private static readonly MethodInfo ApplyByType = typeof(PowerCmd).GetMethods().FirstOrDefault((MethodInfo m) => m.Name == "Apply" && m.IsGenericMethodDefinition && m.GetParameters().Length == 6 && m.GetParameters()[1].ParameterType == typeof(Creature));

		private static int _baseBlock;

		private static decimal _baseEnergy;

		private static readonly HashSet<CardModel> Started = new HashSet<CardModel>(ReferenceEqualityComparer.Instance);

		private static readonly HashSet<CardModel> Finished = new HashSet<CardModel>(ReferenceEqualityComparer.Instance);

		private static IDisposable _selectorScope;

		private static CanvasLayer _layer;

		private static Label _title;

		private static Label _phase;

		private static Label _info;

		private static Label _note;

		private static ColorRect _bar;

		private static float _barWidth;

		private static string _phaseText = "";

		private static Color _phaseColor = Colors.White;

		private static bool _holdArmed;

		private static readonly Color Orange = new Color(1f, 0.72f, 0.3f, 1f);

		private static readonly Color Dim = new Color(0.7f, 0.74f, 0.74f, 1f);

		private static Font _font;

		public static bool Active { get; private set; }

		internal static bool Restoring { get; private set; }

		private static float SlowScale => Mathf.Clamp(Tuning.F("demo.slow", 0.3f), 0.05f, 1f);

		public static void NoteCardStarted(CardPlay play)
		{
			if (Active && ((play != null) ? play.Card : null) != null)
			{
				Started.Add(play.Card);
			}
		}

		public static void NoteCardFinished(CardPlay play)
		{
			if (Active && ((play != null) ? play.Card : null) != null)
			{
				Finished.Add(play.Card);
			}
		}

		private static bool Busy()
		{
			try
			{
				CombatManager instance = CombatManager.Instance;
				return instance != null && _player != null && instance.IsExecutingCardOrPotionEffect(_player);
			}
			catch
			{
				return false;
			}
		}

		public static void Init()
		{
			if (File.Exists(Path.Combine(DevLog.ModDir, "dev_preview.txt")))
			{
				BuildItems();
				Ticker.Add(Update);
				DevLog.Info($"Showcase ready: F4 in combat plays all {Items.Count} demos");
			}
		}

		private static void One(string group, Type card, bool targeted, string note, Func<int, Task> setup = null, Func<int, Task> after = null, float hold = 0.8f, float slow = 0f)
		{
			Items.Add(new Item
			{
				Group = group,
				Card = card,
				Targeted = targeted,
				Note = note,
				Setup = setup,
				After = after,
				Hold = hold,
				Slow = slow
			});
		}

		private static void Per(string group, Type card, string note, string triggerNote, Func<int, Task> trigger, Func<int, Task> setup = null, Func<int, Task> apply = null, string title = null)
		{
			Items.Add(new Item
			{
				Group = group,
				Card = card,
				Title = title,
				Note = note,
				Persistent = true,
				TriggerNote = triggerNote,
				Trigger = trigger,
				Setup = setup,
				Apply = apply
			});
		}

		private static void BuildItems()
		{
			Items.Clear();
			string text = "第一批 · 能力牌";
			Per(text, typeof(NoxiousFumes), "开场：她朝敌人吹出一整片绿雾，贴着地面滚过去。之后毒雾笼在每个敌人脚下", "回合开始：毒雾在每个敌人脚底爆开", TriggerNoxious);
			Per(text, typeof(WraithForm), "待命：鼠标放在牌上，屏幕四周起雾、冒出蠢蠢欲动的纱布条；牌拿到出牌区，纱布条卷进来缠成卡面上那个长角的女鬼（半透明）。打出：全场暗下来，她变实、长出双臂、拳上燃起鬼火，尖啸，纱布被抽走，两团鬼火落进她体内散开。之后本体半透明（无实体），脚边常驻一层薄绿雾", "回合开始：绿雾升起缠住她再散去（模拟触发）", (int _) => Sim(typeof(WraithFormPower)));
			Per(text, typeof(InfiniteBlades), "开场：小刀从一点光里涌出，首尾相接沿 ∞ 跑一圈半，再从交叉处流回她肩后。之后肩后一枚小小的 ∞ 光环，小刀沿着它追逐", "回合开始：∞ 一亮、向外炸开一圈，刀拖着光高高划进手牌，∞ 再重新画出来（模拟触发）", (int _) => Sim(typeof(InfiniteBladesPower)));
			Per(text, typeof(PhantomBlades), "开场：一把刀带着蓝色魂火刺出，残影慢半拍跟上、飘回肩后。之后手里有几张小刀，肩后就有几把虚刃（本回合还没出过小刀时蓄能发亮）", "每回合第一张小刀：蓄能的虚刃飞出，幻影巨刃贯穿目标", (int gen) => ThrowShiv(gen, asFirstOfTurn: true));
			Per(text, typeof(Envenom), "开场：一滴毒落在匕首上，顺着刀身流到刀尖滴下。之后猎人身上没有特效", "攻击打掉血：毒液甩到敌人身上、溅开、往下滴", (int gen) => PlayNew(gen, typeof(StrikeSilent), targeted: true));
			Per(text, typeof(Afterimage), "开场：三道彩色残影向后甩出又收回，斜向速度线扫过。之后身后三个残影", "每打出一张牌：残影拉开", (int gen) => PlayNew(gen, typeof(DefendSilent), targeted: false));
			text = "第二批 · 稀有能力";
			Per(text, typeof(Abrasive), "开场：一圈橘色尖刺绕着她挨个闪出来、马上收回。之后她身上什么都不画", "被攻击：朝攻击者炸出三根短刺、零点几秒收回；攻击者身上一下橘光、一把火星，顿一下往后缩；不留痕迹（模拟触发）", (int _) => Sim(typeof(ThornsPower)));
			Per(text, typeof(MasterPlanner), "开场：身前一套工具排成整齐的格子，羽毛笔打个勾，工具收走，笔出现在头顶。之后头顶一支金色羽毛笔；悬停无奇巧的技能牌时提笔发亮（不抖），拿起时笔淡出、在那张牌上淡入", "先打出：羽毛笔在牌上一划；再丢弃这张牌：笔一颤，牌上那一划亮起散开、光射回笔上", TriggerPlanner);
			Per(text, typeof(Sneaky), "开场：一道锯齿边的暗影滑过把她遮住，再散成烟。之后身周淡淡的暗紫色烟雾", "队友打出攻击牌：烟雾自下而上扫过一次（只有联机会触发）（模拟触发）", (int _) => Sim(typeof(SneakyPower)));
			Per(text, typeof(ToolsOfTheTrade), "开场：她抛出两件工具——小叉子落到抽牌堆上方，绳镖甩到弃牌堆上方吊着。真实回合里：叉子扎住多抽的那张牌、塞进手牌；选弃牌时绳镖在手牌上方抡圈甩火花，射中被弃的牌、绳子绕一圈把它拽进弃牌堆", "这里只演示叉子被推一下、绳镖荡起来（模拟触发）", (int _) => Sim(typeof(ToolsOfTheTradePower)));
			Per(text, typeof(Tracking), "开场：青色脚印从她脚下一步步蔓延到每个敌人脚下。虚弱的敌人被红色准星砸中标记（锁定线收拢、准星砸落、闪光、震屏）", "攻击打到被标记的敌人：准星收缩", (int gen) => PlayNew(gen, typeof(StrikeSilent), targeted: true), (int _) => ApplyTo<WeakPower>(Enemy(), 3));
			Per(text, typeof(WellLaidPlans), "开场：青绿色羽毛笔在手牌上划一道线后消失。鼠标移到结束回合上时，每张会留下的牌各自浮出一圈淡青线", "回合结束：每张牌各自被青线捆紧打结，牌往弃牌堆窜一下、线绷直、被拽回（虚无 / 回合末自动生效 / 自带保留的牌不捆）（模拟触发）", (int _) => Sim(typeof(WellLaidPlansPower)));
			Per(text, typeof(Burst), "未升级：一颗金色光点绕身飞", "下一张技能牌打出两次：金光炸开", (int gen) => PlayNew(gen, typeof(DefendSilent), targeted: false));
			Items[Items.Count - 1].Rearm = (int gen) => RearmBurst(gen, 1);
			Per(text, typeof(Burst), "升级后：两颗光点（接下来两张技能牌都打出两次）", "打出一张技能牌：炸开一次，剩一颗光点", (int gen) => PlayNew(gen, typeof(DefendSilent), targeted: false), null, (int gen) => RearmBurst(gen, 2), "爆发+");
			Items[Items.Count - 1].Rearm = (int gen) => RearmBurst(gen, 2);
			text = "第二批 · 金卡";
			One(text, typeof(Murder), targeted: true, "鼠标移到牌上：背景变暗；指向敌人：头顶红色倒三角；打出：三角一闪，两道血红斩击");
			One(text, typeof(Assassinate), targeted: true, "指向敌人：场景变暗，敌人背后聚起一团黑雾、裹住它的后背，雾里一点红光像心跳一样跳；打出：黑雾穿过敌人，一道柔和的红光从背后贯穿而出，雾被冲散；猎人不做攻击动作");
			One(text, typeof(BulletTime), targeted: false, "鼠标移到牌上：身后出现时钟；拿起：指针飞转；打出：全场变冷蓝，指针停下", null, null, 1.4f);
			One(text, typeof(Malaise), targeted: true, "指向敌人：绿色光圈；打出：病绿色虚线环向内收拢，力量流失");
			One(text, typeof(StormOfSteel), targeted: false, "悬停：一层层正反交替旋转的风环，手牌发抖、被风丝牵着；拿起：手牌被吸进漏斗底、卷上去，环头长出刀；打出：环飞进弃牌堆，刀进手牌", null, null, 1.2f);
			text = "第三批 · 小刀系";
			One(text, typeof(KnifeTrap), targeted: true, "拿起：小刀从右下角消耗牌堆升起；指向敌人：背景变暗、刀围住它；打出：逐把刺入", SetupKnifeTrap, null, 0.6f, 0.45f);
			One(text, typeof(BladeOfInk), targeted: false, "拿起：墨刃悬在手边滴墨；打出：飞进手牌（随后打一张墨影小刀）", null, (int gen) => ThrowShiv(gen, asFirstOfTurn: false));
			Per(text, typeof(Accuracy), "开场：准星先在身前放大亮出，一把小刀射中正中心，准星带着刀收回到手上。之后手上金色准星，向每个敌人拉瞄准线", "小刀命中：目标身上准星收拢", (int gen) => ThrowShiv(gen, asFirstOfTurn: false));
			One(text, typeof(HiddenDaggers), targeted: false, "拿起：刀藏在两边袖口，露出一半；打出：弹出飞进手牌（自动丢两张牌）");
			One(text, typeof(UpMySleeve), targeted: false, "拿起：刀在身前排成扇面，指向敌人；打出：飞进手牌");
			Type type = GameCapabilities.Card("BladeSymphony");
			if (type != null)
			{
				One(text, type, targeted: false, "拿起：头顶两把刀轮流跳动，每跳一次一个光圈；打出：飞进手牌");
			}
			One(text, typeof(BladeDance), targeted: false, "拿起：刀绕身慢转，要打出时越转越快；打出：飞进手牌（随后打一张小刀）", null, (int gen) => ThrowShiv(gen, asFirstOfTurn: false));
			One(text, typeof(CloakAndDagger), targeted: false, "拿起：斗篷遮身，刀握在下摆轻轻抖动；打出：斗篷一甩，刀飞进手牌");
			One(text, typeof(LeadingStrike), targeted: true, "拿起：反手握刀；指向敌人：对准蓄力；打出：刺出后旋转着弹回手里");
			text = "第四批 · 毒系";
			One(text, typeof(Outbreak), targeted: false, "拿起：瘟疫沿地面爬向每个敌人；打出：敌人脚下喷出毒液，中毒立即发作", null, null, 1.4f);
			Per(text, typeof(CorrosiveWave), "拿起：脚边一片柔和的绿光；打出：酸液从脚下漫开，铺满整个地面", "抽一张牌：一圈更亮的酸液从她脚下荡开，扫到的敌人脚下泛起涟漪", TriggerDraw);
			Per(text, typeof(Accelerant), "开场：滴管往小瓶里滴一滴药，瓶子一缩、开始冒烟。之后中毒敌人的后上方有加速箭头（一层一个，最多五个）", "中毒发作两次：第二次是黄绿色", TriggerAccelerant);
			Type type2 = GameCapabilities.Card("Concoct");
			Type concoctPower = GameCapabilities.Power("ConcoctPower");
			if (type2 != null && concoctPower != null)
			{
				Per(text, type2, "肩旁冒泡的小毒瓶（这张牌要选队友，这里直接给自己上效果）", "攻击打掉血：敌人身上溅毒", (int gen) => PlayNew(gen, typeof(StrikeSilent), targeted: true), null, (int _) => ApplyTo(concoctPower, _me, 3));
			}
			One(text, typeof(BubbleBubble), targeted: true, "拿起没有特效；指向有毒的敌人：绿色圆圈；打出：毒池沸腾，大泡升起炸开", (int _) => ApplyTo<PoisonPower>(Enemy(), 4));
			One(text, typeof(Mirage), targeted: false, "拿起：中毒敌人身上的雾气飞速涌向猎人；打出：雾气汇成护罩", SetupPoisonAll, null, 1.2f);
			One(text, typeof(DeadlyPoison), targeted: true, "拿起：手里凝出一团毒液；指向敌人：目标身上出现标记；打出：掷出");
			One(text, typeof(PoisonedStab), targeted: true, "拿起：握刀的手上一个细长的绿色三角，尖端指向敌人；打出：绿色刺痕贯穿，毒液喷出");
			One(text, typeof(Snakebite), targeted: true, "打出：扭动的绿光飞去，张嘴沿敌人轮廓裹住它，继续往前飞把这一圈拉断，爆炸，蛇飞走");
			Per(text, null, "中毒的敌人身上飘着毒泡，层数越多越密", "中毒发作：绿环 + 气泡爆开", TriggerPoison, null, (int _) => ApplyTo<PoisonPower>(Enemy(), 8), "中毒（敌人身上的效果）");
		}

		private static Creature Enemy()
		{
			try
			{
				ICombatState state = _state;
				return (state != null) ? state.HittableEnemies.FirstOrDefault((Creature e) => e != null && e.IsAlive) : null;
			}
			catch
			{
				return null;
			}
		}

		private static bool IsShiv(CardModel card)
		{
			if (card != null && card.Tags != null)
			{
				return card.Tags.Contains((CardTag)5);
			}
			return false;
		}

		private static List<CardModel> HandCards()
		{
			return PileTypeExtensions.GetPile((PileType)2, _player).Cards.ToList();
		}

		private static Task Sim(Type power)
		{
			PowerFxManager.TriggerType(_me, power, Enemy());
			return Task.CompletedTask;
		}

		private static async Task ApplyTo<T>(Creature target, int amount) where T : PowerModel
		{
			if (target != null)
			{
				await PowerCmd.Apply<T>((PlayerChoiceContext)new ThrowingPlayerChoiceContext(), target, (decimal)amount, _me, (CardModel)null, false);
			}
		}

		private static async Task ApplyTo(Type power, Creature target, int amount)
		{
			if (!(power == null) && target != null && !(ApplyByType == null))
			{
				await (Task)ApplyByType.MakeGenericMethod(power).Invoke(null, new object[6]
				{
					(object)new ThrowingPlayerChoiceContext(),
					target,
					(decimal)amount,
					_me,
					null,
					false
				});
			}
		}

		private static async Task SetupPoisonAll(int gen)
		{
			foreach (Creature item in _state.HittableEnemies.ToList())
			{
				await ApplyTo<PoisonPower>(item, 4);
			}
		}

		private static async Task SetupKnifeTrap(int gen)
		{
			List<CardModel> list = PileTypeExtensions.GetPile((PileType)4, _player).Cards.Where(IsShiv).ToList();
			if (list.Count > 0)
			{
				await CardPileCmd.RemoveFromCombat((IEnumerable<CardModel>)list, true);
			}
			for (int i = 0; i < 4; i++)
			{
				await Spawn(typeof(Shiv), (PileType)4, silent: true);
			}
		}

		private static async Task TriggerNoxious(int gen)
		{
			NoxiousFumesPower power = _me.GetPower<NoxiousFumesPower>();
			if (power != null)
			{
				await ((AbstractModel)power).AfterSideTurnStart((CombatSide)1, _state.GetCreaturesOnSide((CombatSide)1), _state);
			}
		}

		private static async Task RearmBurst(int gen, int charges)
		{
			int num = charges - _me.GetPowerAmount<BurstPower>();
			if (num > 0)
			{
				await ApplyTo<BurstPower>(_me, num);
			}
		}

		private static async Task TriggerPlanner(int gen)
		{
			CardModel card = await Spawn(typeof(DefendSilent), (PileType)2, silent: false);
			if (card != null)
			{
				await Wait(gen, 0.25f);
				await Play(gen, card, null);
				await Wait(gen, 1f);
				CardPile pile = card.Pile;
				if (pile == null || (int)pile.Type != 2)
				{
					await CardPileCmd.Add(card, (PileType)2, (CardPilePosition)1, (AbstractModel)null, false);
				}
				await Wait(gen, 0.6f);
				await CardCmd.Discard((PlayerChoiceContext)new ThrowingPlayerChoiceContext(), card);
				await Wait(gen, 0.8f);
			}
		}

		private static async Task TriggerDraw(int gen)
		{
			await CardPileCmd.Draw((PlayerChoiceContext)new ThrowingPlayerChoiceContext(), 1m, _player, false);
		}

		private static async Task TriggerAccelerant(int gen)
		{
			Creature val = Enemy();
			if (val != null)
			{
				await ApplyTo<PoisonPower>(val, 6);
				await Wait(gen, 0.5f);
				await TriggerPoison(gen);
			}
		}

		private static async Task TriggerPoison(int gen)
		{
			Creature val = Enemy();
			PoisonPower val2 = ((val != null) ? val.GetPower<PoisonPower>() : null);
			if (val2 != null)
			{
				if (GameCapabilities.PoisonTrigger != null)
				{
					await (Task)GameCapabilities.PoisonTrigger.Invoke(val2, null);
				}
				else
				{
					await ((AbstractModel)val2).AfterSideTurnStart((CombatSide)2, _state.GetCreaturesOnSide((CombatSide)2), _state);
				}
			}
		}

		private static async Task ThrowShiv(int gen, bool asFirstOfTurn)
		{
			CardModel val = ((IEnumerable<CardModel>)HandCards()).FirstOrDefault((Func<CardModel, bool>)IsShiv);
			if (val == null)
			{
				val = await Spawn(typeof(Shiv), (PileType)2, silent: false);
			}
			CardModel shiv = val;
			await Wait(gen, 0.25f);
			if (asFirstOfTurn)
			{
				ShivFx.OnPlayerTurnStart();
			}
			await Play(gen, shiv, Enemy());
		}

		private static async Task PlayNew(int gen, Type type, bool targeted)
		{
			CardModel card = await Spawn(type, (PileType)2, silent: false);
			await Wait(gen, 0.25f);
			await Play(gen, card, targeted ? Enemy() : null);
		}

		private static CardModel Canonical(Type type)
		{
			if (!(type == null))
			{
				return ModelDb.AllCards.FirstOrDefault((CardModel c) => ((object)c).GetType() == type);
			}
			return null;
		}

		private static async Task<CardModel> Spawn(Type type, PileType pile, bool silent)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			CardModel val = Canonical(type);
			if (val == null)
			{
				DevLog.Warn("Showcase: no such card " + type?.Name);
				return null;
			}
			CardModel card = _state.CreateCard(val, _player);
			await CardPileCmd.Add(card, pile, (CardPilePosition)1, (AbstractModel)null, silent);
			Spawned.Add(card);
			return card;
		}

		private static async Task HoldFirst(int gen, int index, float within, CardModel card, Creature target, string title, string info, string note, string speedText, Color color, bool brief)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			if (card != null && HeldFxManager.For(((object)card).GetType()) != null)
			{
				await PlayerCmd.SetEnergy((decimal)Tuning.I("demo.energy", 6), _player);
				Check(gen);
				_holdArmed = false;
				HeldFxManager.Begin(card, () => _holdArmed);
				SetCaption(index, within, title, info, note, speedText + " · 拿起", color);
				await Wait(gen, brief ? 0.55f : 1.3f);
				_holdArmed = true;
				if (target != null)
				{
					HeldFxManager.SetTarget(target);
				}
				SetCaption(index, within, title, info, note, speedText + ((target != null) ? " · 指向敌人" : " · 出牌区"), color);
				await Wait(gen, brief ? 0.5f : 1.1f);
				SetCaption(index, within, title, info, note, speedText + " · 打出", color);
				HeldFxManager.End(played: true);
			}
		}

		private static async Task<bool> Play(int gen, CardModel card, Creature target)
		{
			if (card == null)
			{
				return false;
			}
			await PlayerCmd.SetEnergy((decimal)Tuning.I("demo.energy", 6), _player);
			if (!card.CanPlayTargeting(target))
			{
				DevLog.Warn("Showcase: the game will not let " + ((object)card).GetType().Name + " be played right now");
				return false;
			}
			Started.Remove(card);
			Finished.Remove(card);
			TaskHelper.RunSafely(card.OnEnqueuePlayVfx(target));
			RunManager.Instance.ActionQueueSynchronizer.RequestEnqueue((GameAction)new PlayCardAction(card, target));
			float t = 0f;
			while (!Started.Contains(card) && t < 2.5f)
			{
				await NextFrame();
				Check(gen);
				t += _frameDt * (float)Engine.TimeScale;
			}
			if (!Started.Contains(card))
			{
				DevLog.Warn("Showcase: " + ((object)card).GetType().Name + " was queued but never played");
				return false;
			}
			t = 0f;
			while (!Finished.Contains(card) && t < 30f)
			{
				await NextFrame();
				Check(gen);
				t += _frameDt * (float)Engine.TimeScale;
			}
			if (!Finished.Contains(card))
			{
				DevLog.Warn("Showcase: " + ((object)card).GetType().Name + " did not finish in time");
			}
			t = 0f;
			while (Busy() && t < 10f)
			{
				await NextFrame();
				Check(gen);
				t += _frameDt * (float)Engine.TimeScale;
			}
			await Wait(gen, 0.15f);
			return true;
		}

		private static async Task RunItem(int gen, int index)
		{
			Item item = Items[index];
			CardModel val = Canonical(item.Card);
			string title = item.Title ?? ((val != null) ? val.Title : null) ?? item.Card?.Name ?? "?";
			string info = item.Group + ((val != null) ? ("\u3000·\u3000" + RarityName(val.Rarity) + TypeName(val.Type)) : "");
			float slow = ((item.Slow > 0f) ? item.Slow : SlowScale);
			string slowText = $"慢速 ×{slow:0.##}";
			DevLog.Info($"Showcase: {index + 1}/{Items.Count} {item.Card?.Name ?? item.Title}");
			if (item.Persistent)
			{
				SetCaption(index, 0f, title, info, item.Note, "准备中…", Dim);
				await Reset(gen);
				SetCaption(index, 0f, title, info, item.Note, "原速 · 常驻", Colors.White);
				if (item.Setup != null)
				{
					await item.Setup(gen);
				}
				if (item.Apply != null)
				{
					await item.Apply(gen);
				}
				else
				{
					CardModel card = await Spawn(item.Card, (PileType)2, silent: false);
					await Wait(gen, 0.3f);
					await HoldFirst(gen, index, 0f, card, null, title, info, item.Note, "原速", Colors.White, brief: false);
					await Play(gen, card, null);
					SetCaption(index, 0.1f, title, info, item.Note, "原速 · 常驻", Colors.White);
				}
				await Wait(gen, 2f);
				SetCaption(index, 0.3f, title, info, item.TriggerNote, "原速 · 触发", Colors.White);
				await item.Trigger(gen);
				await Wait(gen, 1.4f);
				if (item.Rearm != null)
				{
					await item.Rearm(gen);
				}
				SetScale(slow);
				SetCaption(index, 0.55f, title, info, item.Note, slowText + " · 常驻", Orange);
				await Wait(gen, 0.8f);
				SetCaption(index, 0.75f, title, info, item.TriggerNote, slowText + " · 触发", Orange);
				await item.Trigger(gen);
				await Wait(gen, 1f);
				SetScale(1.0);
				return;
			}
			for (int pass = 0; pass < 2; pass++)
			{
				bool slowPass = pass == 1;
				SetCaption(index, (float)pass * 0.5f, title, info, item.Note, "准备中…", Dim);
				await Reset(gen);
				SetCaption(index, (float)pass * 0.5f, title, info, item.Note, slowPass ? slowText : "原速", slowPass ? Orange : Colors.White);
				if (item.Setup != null)
				{
					await item.Setup(gen);
				}
				CardModel card = await Spawn(item.Card, (PileType)2, silent: false);
				await Wait(gen, 0.35f);
				if (slowPass)
				{
					SetScale(slow);
				}
				Creature target = (item.Targeted ? Enemy() : null);
				await HoldFirst(gen, index, (float)pass * 0.5f, card, target, title, info, item.Note, slowPass ? slowText : "原速", slowPass ? Orange : Colors.White, slowPass);
				await Play(gen, card, target);
				if (item.After != null)
				{
					await Wait(gen, 0.45f);
					await item.After(gen);
				}
				await Wait(gen, item.Hold);
				SetScale(1.0);
			}
		}

		private static async void RunFrom(int index)
		{
			int gen = ++_gen;
			_index = Mathf.Clamp(index, 0, Items.Count);
			try
			{
				_paused = false;
				SetScale(1.0);
				await WaitForBusy();
				Check(gen);
				if (!_prepared)
				{
					await Prepare();
					_prepared = true;
					Check(gen);
				}
				for (int i = _index; i < Items.Count; i++)
				{
					_index = i;
					await RunItem(gen, i);
				}
				await Reset(gen);
				SetCaption(Items.Count - 1, 1f, "演示结束", "", "按 F4 可以再看一遍；这场战斗保存并退出后会从头重新开始", "完", Colors.White);
				if (FxKit.Alive((GodotObject)(object)_bar))
				{
					((Control)_bar).Size = new Vector2(_barWidth, ((Control)_bar).Size.Y);
				}
				await Wait(gen, 3.5f);
				Stop("finished");
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex2)
			{
				DevLog.Error("Showcase failed: " + ex2);
				if (gen == _gen)
				{
					Stop("error");
				}
			}
		}

		private static async Task Prepare()
		{
			int hp = Tuning.I("demo.enemy_hp", 400);
			foreach (var item in EnemyHp)
			{
				if (item.Enemy.IsAlive)
				{
					await CreatureCmd.SetMaxAndCurrentHp(item.Enemy, (decimal)hp);
				}
			}
		}

		private static async Task Reset(int gen)
		{
			Check(gen);
			SetScale(1.0);
			HeldFxManager.End(played: false);
			await ClearAddedPowers();
			foreach (Creature item in _state.Enemies.ToList())
			{
				if (item.IsAlive && item.CurrentHp < item.MaxHp)
				{
					await CreatureCmd.SetCurrentHp(item, (decimal)item.MaxHp);
				}
			}
			await DropExtraBlock();
			List<CardModel> list = (from c in HandCards()
				where IsShiv(c) || Spawned.Contains(c)
				select c).ToList();
			if (list.Count > 0)
			{
				await CardPileCmd.RemoveFromCombat((IEnumerable<CardModel>)list, false);
			}
			List<CardModel> hand = HandCards();
			for (int i = hand.Count - 1; i >= 6; i--)
			{
				await CardPileCmd.Add(hand[i], (PileType)3, (CardPilePosition)2, (AbstractModel)null, false);
			}
			if (hand.Count < 4)
			{
				await CardPileCmd.Draw((PlayerChoiceContext)new ThrowingPlayerChoiceContext(), (decimal)(4 - hand.Count), _player, false);
			}
			await Wait(gen, 0.35f);
		}

		private static async Task DropExtraBlock()
		{
			int num = _me.Block - _baseBlock;
			if (num > 0)
			{
				await RuntimeApiCompatibility.LoseBlock((PlayerChoiceContext)new ThrowingPlayerChoiceContext(), _me, num);
			}
		}

		private static async Task RestoreCards()
		{
			List<CardModel> list = _player.PlayerCombatState.AllCards.Where((CardModel c) => !Home.ContainsKey(c)).ToList();
			if (list.Count > 0)
			{
				await CardPileCmd.RemoveFromCombat((IEnumerable<CardModel>)list, false);
			}
			foreach (KeyValuePair<CardModel, PileType> item in Home.OrderBy((KeyValuePair<CardModel, PileType> p) =>
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Invalid comparison between Unknown and I4
				return ((int)p.Value == 2) ? 1 : 0;
			}).ToList())
			{
				CardModel card = item.Key;
				CardPile pile = card.Pile;
				PileType val = (PileType)((pile != null) ? ((int)pile.Type) : 0);
				if ((int)val != 0 && (int)item.Value != 0 && (int)item.Value != 5)
				{
					if (val != item.Value)
					{
						await CardPileCmd.Add(card, item.Value, (CardPilePosition)1, (AbstractModel)null, false);
					}
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

		private static async Task RestoreBasePowers()
		{
			foreach (var entry in BasePowers)
			{
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

		private static async Task ClearAddedPowers()
		{
			foreach (Creature item in _state.Creatures.ToList())
			{
				foreach (PowerModel item2 in item.Powers.ToList())
				{
					if (!Baseline.Contains(item2))
					{
						await PowerCmd.Remove(item2);
					}
				}
			}
			await RestoreBasePowers();
		}

		private static void Start()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			if (TestBench.Active || Active)
			{
				return;
			}
			string text = Bind();
			if (text != null)
			{
				DevLog.Info("Showcase: cannot start: " + text);
				BuildUi();
				SetCaption(0, 0f, "现在还不能开始演示", "", text, "", Colors.White);
				CanvasLayer layer = _layer;
				_layer = null;
				FxKit.FreeAfter((Node)(object)layer, 3.0);
				return;
			}
			Baseline.Clear();
			EnemyHp.Clear();
			Home.Clear();
			HadCostChange.Clear();
			BasePowers.Clear();
			DrawOrder.Clear();
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
			Spawned.Clear();
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
			Active = true;
			_prepared = false;
			_paused = false;
			_selectorScope = RuntimeApiCompatibility.PushSelector((ICardSelector)(object)new AutoSelector());
			BuildUi();
			DevLog.Info($"Showcase: started ({Items.Count} demos, {EnemyHp.Count} enemies)");
			RunFrom(0);
		}

		private static string Bind()
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Invalid comparison between Unknown and I4
			try
			{
				CombatManager instance = CombatManager.Instance;
				if (GameRefs.CombatRoom == null || instance == null || !instance.IsInProgress || instance.IsOverOrEnding)
				{
					return "请先进入一场战斗（主菜单点“继续游戏”），再按 F4";
				}
				_state = (ICombatState)(object)instance.DebugOnlyGetState();
				if (_state == null)
				{
					return "请先进入一场战斗，再按 F4";
				}
				if (_state.Players.Count != 1)
				{
					return "演示只能在单人游戏里运行";
				}
				if ((int)_state.CurrentSide != 1 || instance.PlayerActionsDisabled)
				{
					return "请等到自己的回合（能出牌的时候）再按 F4";
				}
				_player = LocalContext.GetMe(_state);
				Player player = _player;
				_me = ((player != null) ? player.Creature : null);
				if (_me == null || !_me.IsAlive)
				{
					return "没有找到你的角色";
				}
				if (Enemy() == null)
				{
					return "场上没有可以攻击的敌人";
				}
				return null;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Showcase: Bind failed: " + ex);
				return "读取战斗状态失败（详见 silentfx.log）";
			}
		}

		private static void Stop(string why)
		{
			if (Active)
			{
				Active = false;
				_gen++;
				_paused = false;
				_wantScale = 1.0;
				Engine.TimeScale = 1.0;
				try
				{
					_selectorScope?.Dispose();
				}
				catch (Exception ex)
				{
					DevLog.Warn("Showcase: selector scope: " + ex.Message);
				}
				_selectorScope = null;
				HeldFxManager.End(played: false);
				if (FxKit.Alive((GodotObject)(object)_layer))
				{
					((Node)_layer).QueueFree();
				}
				_layer = null;
				DevLog.Info("Showcase: stopped (" + why + ")");
				Restore();
			}
		}

		private static async void Restore()
		{
			Restoring = true;
			try
			{
				await WaitForBusy();
				CombatManager instance = CombatManager.Instance;
				if (GameRefs.CombatRoom == null || instance == null || !instance.IsInProgress || instance.IsOverOrEnding)
				{
					return;
				}
				await ClearAddedPowers();
				await DropExtraBlock();
				await RestoreCards();
				foreach (var entry in EnemyHp)
				{
					if (entry.Enemy.IsAlive)
					{
						await CreatureCmd.SetMaxAndCurrentHp(entry.Enemy, (decimal)entry.Max);
						await CreatureCmd.SetCurrentHp(entry.Enemy, (decimal)entry.Current);
					}
				}
				await PlayerCmd.SetEnergy(_baseEnergy, _player);
				DevLog.Info("Showcase: fight put back as it was");
			}
			catch (Exception ex)
			{
				DevLog.Warn("Showcase: restore failed: " + ex.Message);
			}
			finally
			{
				Restoring = false;
				Started.Clear();
				Finished.Clear();
				Spawned.Clear();
				EnemyHp.Clear();
				Baseline.Clear();
				Home.Clear();
				HadCostChange.Clear();
				BasePowers.Clear();
				DrawOrder.Clear();
			}
		}

		private static void TogglePause()
		{
			_paused = !_paused;
			Engine.TimeScale = (_paused ? 0.0 : _wantScale);
			RefreshPhase();
		}

		private static void SetScale(double scale)
		{
			_wantScale = scale;
			if (!_paused)
			{
				Engine.TimeScale = scale;
			}
		}

		private static void Check(int gen)
		{
			if (gen != _gen || !Active)
			{
				throw new OperationCanceledException();
			}
		}

		private static Task NextFrame()
		{
			TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
			Waiters.Add(taskCompletionSource);
			return taskCompletionSource.Task;
		}

		private static async Task Wait(int gen, float gameSeconds)
		{
			for (float t = 0f; t < gameSeconds; t += _frameDt * (float)Engine.TimeScale)
			{
				await NextFrame();
				Check(gen);
			}
		}

		private static async Task WaitForBusy()
		{
			float real = 0f;
			float quiet = 0f;
			while (real < 12f && quiet < 0.25f)
			{
				await NextFrame();
				real += _frameDt;
				quiet = (Busy() ? 0f : (quiet + _frameDt));
			}
		}

		private static bool Pressed(Key key)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			bool flag = Input.IsKeyPressed(key);
			Held.TryGetValue(key, out var value);
			Held[key] = flag;
			if (flag)
			{
				return !value;
			}
			return false;
		}

		private static void Update(double dt)
		{
			_frameDt = (float)dt;
			if (Waiters.Count > 0)
			{
				TaskCompletionSource<bool>[] array = Waiters.ToArray();
				Waiters.Clear();
				TaskCompletionSource<bool>[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].TrySetResult(result: true);
				}
			}
			bool flag = Pressed((Key)4194335);
			bool flag2 = Pressed((Key)4194334);
			bool flag3 = Pressed((Key)4194338);
			bool flag4 = Pressed((Key)4194340);
			bool flag5 = Pressed((Key)4194341);
			bool flag6 = Pressed((Key)4194337);
			if (TourController.Exclusive || Restoring)
			{
				return;
			}
			if (flag)
			{
				if (Active)
				{
					Stop("F4");
				}
				else
				{
					Start();
				}
			}
			else
			{
				if (!Active)
				{
					return;
				}
				if (GameRefs.CombatRoom == null)
				{
					Stop("left the fight");
				}
				else if (flag2)
				{
					TogglePause();
				}
				else if (flag3)
				{
					RunFrom(_index);
				}
				else if (flag4)
				{
					Jump(_index + 1);
				}
				else if (flag5)
				{
					Jump(_index - 1);
				}
				else if (flag6)
				{
					int j = Mathf.Clamp(_index, 0, Items.Count - 1);
					for (string text = Items[j].Group; j < Items.Count && Items[j].Group == text; j++)
					{
					}
					Jump(j);
				}
			}
		}

		private static void Jump(int index)
		{
			if (index < 0)
			{
				index = 0;
			}
			if (index >= Items.Count)
			{
				index = Items.Count - 1;
			}
			RunFrom(index);
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

		private static Label MakeLabel(float x, float y, float w, float h, int size, Color color, HorizontalAlignment align)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Expected Obj, but got Unknown
			//IL_0083: Expected Obj, but got Unknown
			Label val = new Label
			{
				Position = new Vector2(x, y),
				Size = new Vector2(w, h),
				HorizontalAlignment = align,
				VerticalAlignment = (VerticalAlignment)1,
				ClipText = true,
				LabelSettings = new LabelSettings
				{
					Font = CaptionFont(),
					FontSize = size,
					FontColor = color,
					OutlineSize = 6,
					OutlineColor = new Color(0f, 0f, 0f, 0.85f)
				}
			};
			((Node)_layer).AddChild((Node)(object)val, false, (InternalMode)0);
			return val;
		}

		private static void BuildUi()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected Obj, but got Unknown
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Expected Obj, but got Unknown
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Expected Obj, but got Unknown
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Expected Obj, but got Unknown
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_0310: Expected Obj, but got Unknown
			if (FxKit.Alive((GodotObject)(object)_layer))
			{
				((Node)_layer).QueueFree();
			}
			SceneTree tree = Ticker.Tree;
			if (tree != null)
			{
				_layer = new CanvasLayer
				{
					Name = StringName.op_Implicit("SilentFX_Showcase"),
					Layer = 95
				};
				((Node)tree.Root).AddChild((Node)(object)_layer, false, (InternalMode)0);
				Vector2 val = FxKit.ViewSize();
				float num = 1040f;
				float num2 = 176f;
				float num3 = (val.X - num) * 0.5f;
				float num4 = 82f;
				_barWidth = num;
				((Node)_layer).AddChild((Node)new ColorRect
				{
					Color = new Color(0.02f, 0.05f, 0.05f, 0.8f),
					Position = new Vector2(num3, num4),
					Size = new Vector2(num, num2),
					MouseFilter = (MouseFilterEnum)2
				}, false, (InternalMode)0);
				((Node)_layer).AddChild((Node)new ColorRect
				{
					Color = new Color(0.5f, 0.98f, 0.32f, 0.9f),
					Position = new Vector2(num3, num4),
					Size = new Vector2(6f, num2),
					MouseFilter = (MouseFilterEnum)2
				}, false, (InternalMode)0);
				_title = MakeLabel(num3 + 28f, num4 + 8f, num - 330f, 54f, 38, Colors.White, (HorizontalAlignment)0);
				_phase = MakeLabel(num3 + num - 320f, num4 + 10f, 296f, 50f, 30, Colors.White, (HorizontalAlignment)2);
				_info = MakeLabel(num3 + 28f, num4 + 60f, num - 56f, 32f, 22, new Color(0.62f, 0.92f, 0.72f, 1f), (HorizontalAlignment)0);
				_note = MakeLabel(num3 + 28f, num4 + 94f, num - 56f, 36f, 25, new Color(0.95f, 0.95f, 0.88f, 1f), (HorizontalAlignment)0);
				MakeLabel(num3 + 28f, num4 + 134f, num - 56f, 28f, 18, new Color(0.7f, 0.74f, 0.74f, 1f), (HorizontalAlignment)0).Text = "F4 结束演示\u3000\u3000F3 暂停 / 继续\u3000\u3000F7 重播这一个\u3000\u3000F9 下一个\u3000\u3000F10 上一个\u3000\u3000F6 下一组";
				((Node)_layer).AddChild((Node)new ColorRect
				{
					Color = new Color(1f, 1f, 1f, 0.14f),
					Position = new Vector2(num3, num4 + num2 - 5f),
					Size = new Vector2(num, 5f),
					MouseFilter = (MouseFilterEnum)2
				}, false, (InternalMode)0);
				_bar = new ColorRect
				{
					Color = new Color(0.5f, 0.98f, 0.32f, 0.95f),
					Position = new Vector2(num3, num4 + num2 - 5f),
					Size = new Vector2(0f, 5f),
					MouseFilter = (MouseFilterEnum)2
				};
				((Node)_layer).AddChild((Node)(object)_bar, false, (InternalMode)0);
			}
		}

		private static void SetCaption(int index, float within, string title, string info, string note, string phase, Color phaseColor)
		{
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_layer))
			{
				if (FxKit.Alive((GodotObject)(object)_title))
				{
					_title.Text = (Active ? $"{index + 1} / {Items.Count}\u3000{title}" : title);
				}
				if (FxKit.Alive((GodotObject)(object)_info))
				{
					_info.Text = info ?? "";
				}
				if (FxKit.Alive((GodotObject)(object)_note))
				{
					_note.Text = note ?? "";
				}
				_phaseText = phase ?? "";
				_phaseColor = phaseColor;
				RefreshPhase();
				if (FxKit.Alive((GodotObject)(object)_bar) && Items.Count > 0)
				{
					((Control)_bar).Size = new Vector2(_barWidth * Mathf.Clamp(((float)index + within) / (float)Items.Count, 0f, 1f), ((Control)_bar).Size.Y);
				}
			}
		}

		private static void RefreshPhase()
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_phase))
			{
				_phase.Text = (_paused ? "已暂停" : _phaseText);
				if (_phase.LabelSettings != null)
				{
					_phase.LabelSettings.FontColor = (Color)(_paused ? new Color(1f, 0.45f, 0.4f, 1f) : _phaseColor);
				}
			}
		}

		private static string RarityName(CardRarity rarity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected I4, but got Unknown
			return (rarity - 1) switch
			{
				0 => "基础", 
				1 => "普通", 
				2 => "罕见", 
				3 => "稀有", 
				4 => "先古", 
				6 => "衍生", 
				_ => "", 
			};
		}

		private static string TypeName(CardType type)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected I4, but got Unknown
			return (type - 1) switch
			{
				0 => "攻击牌", 
				1 => "技能牌", 
				2 => "能力牌", 
				_ => "", 
			};
		}

		static Showcase()
		{
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		}
	}
