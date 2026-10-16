// namespace: SilentFX.Cards
// type: CardFxManager (class)
// source lines: 49259-49725
	public static class CardFxManager
	{
		private sealed class HeldOnlyFx : CardFx
		{
		}

		private static readonly Dictionary<Type, CardFx> Map = new Dictionary<Type, CardFx>();

		private static readonly HashSet<Type> HitTypes = new HashSet<Type>();

		private static readonly List<(string Name, CardFx Fx, HeldFx Held)> Catalog = new List<(string, CardFx, HeldFx)>();

		private static readonly HashSet<Type> RemoteSafe = new HashSet<Type>
		{
			typeof(Finisher),
			typeof(Flechettes),
			typeof(KnifeTrap),
			typeof(BladeDance),
			typeof(CloakAndDagger),
			typeof(HiddenDaggers),
			typeof(UpMySleeve),
			typeof(BladeOfInk),
			typeof(StormOfSteel),
			typeof(Shiv),
			typeof(LeadingStrike),
			typeof(Malaise),
			typeof(Expose),
			typeof(Suppress),
			typeof(BulletTime),
			typeof(BubbleBubble),
			typeof(DeadlyPoison),
			typeof(PoisonedStab),
			typeof(Snakebite),
			typeof(TheHunt),
			typeof(Strangle),
			typeof(Accelerant),
			typeof(NoxiousFumes),
			typeof(InfiniteBlades),
			typeof(PhantomBlades),
			typeof(Envenom),
			typeof(Afterimage),
			typeof(MasterPlanner),
			typeof(Sneaky),
			typeof(Backstab),
			typeof(Dash),
			typeof(EchoingSlash),
			typeof(MementoMori),
			typeof(Pounce),
			typeof(PreciseCut),
			typeof(Skewer),
			typeof(Pinpoint),
			typeof(Predator),
			typeof(Ricochet),
			typeof(SuckerPunch),
			typeof(FlickFlack),
			typeof(Acrobatics),
			typeof(Backflip),
			typeof(Blur),
			typeof(EscapePlan),
			typeof(Footwork),
			typeof(LegSweep),
			typeof(Deflect),
			typeof(DodgeAndRoll),
			typeof(Anticipate),
			typeof(Untouchable),
			typeof(PiercingWail),
			typeof(Murder),
			typeof(Assassinate),
			typeof(Outbreak),
			typeof(CorrosiveWave)
		};

		public static IReadOnlyList<(string Name, CardFx Fx, HeldFx Held)> All => Catalog;

		private static bool CanShow(CardModel card)
		{
			if (MultiplayerFx.Active)
			{
				Player owner = card.Owner;
				if (((owner != null) ? owner.Creature : null) != null)
				{
					if (!MultiplayerFx.IsLocal(card))
					{
						return RemoteSafe.Contains(((object)card).GetType());
					}
					return true;
				}
				return false;
			}
			return IsLocal(card);
		}

		public static void Init()
		{
			HeldFxManager.Init();
			string[] array = new string[3] { "BladeSymphony", "Fade", "Sidestep" };
			for (int i = 0; i < array.Length; i++)
			{
				Type type = GameCapabilities.Card(array[i]);
				if (type != null)
				{
					RemoteSafe.Add(type);
				}
			}
			CardFxManager.Register<Murder>("Murder", (CardFx)new MurderFx());
			CardFxManager.Register<Assassinate>("Assassinate", (CardFx)new AssassinateFx());
			CardFxManager.Register<BulletTime>("Bullet Time", (CardFx)new BulletTimeFx());
			CardFxManager.Register<Malaise>("Malaise", (CardFx)new MalaiseFx());
			CardFxManager.Register<StormOfSteel>("Storm of Steel", (CardFx)new StormOfSteelFx());
			CardFxManager.Register<KnifeTrap>("Knife Trap", (CardFx)new KnifeTrapFx());
			CardFxManager.Register<ShadowStep>("Shadow Step", (CardFx)new ShadowStepFx());
			CardFxManager.Register<TheHunt>("The Hunt", (CardFx)new TheHuntFx());
			CardFxManager.Register<Outbreak>("Outbreak", (CardFx)new OutbreakFx());
			CardFxManager.Register<BubbleBubble>("Bubble Bubble", (CardFx)new BubbleBubbleFx());
			CardFxManager.Register<Mirage>("Mirage", (CardFx)new MirageFx());
			CardFxManager.Register<DeadlyPoison>("Deadly Poison", (CardFx)new DeadlyPoisonFx());
			CardFxManager.Register<PoisonedStab>("Poisoned Stab", (CardFx)new PoisonedStabFx());
			CardFxManager.Register<Snakebite>("Snakebite", (CardFx)new SnakebiteFx());
			CardFxManager.Register<Accelerant>("Accelerant (opening)", (CardFx)new IntroFx(1.25f, IntroSpot.InFront, Intros.Accelerant));
			CardFxManager.Register<NoxiousFumes>("Noxious Fumes (opening)", (CardFx)new NoxiousCloudFx());
			CardFxManager.Register<WraithForm>("Wraith Form (opening)", (CardFx)new WraithPlayFx());
			CardFxManager.Register<InfiniteBlades>("Infinite Blades (opening)", (CardFx)new IntroFx(1f, IntroSpot.InFront, Intros.InfiniteBlades));
			CardFxManager.Register<PhantomBlades>("Phantom Blades (opening)", (CardFx)new IntroFx(0.8f, IntroSpot.InFront, Intros.PhantomBlades));
			CardFxManager.Register<Envenom>("Envenom (opening)", (CardFx)new IntroFx(1.1f, IntroSpot.InFront, Intros.Envenom));
			CardFxManager.Register<Afterimage>("Afterimage (opening)", (CardFx)new IntroFx(0.62f, IntroSpot.OnBody, Intros.Afterimage));
			CardFxManager.Register<MasterPlanner>("Master Planner (opening)", (CardFx)new IntroFx(1.05f, IntroSpot.InFront, Intros.MasterPlanner));
			CardFxManager.Register<Sneaky>("Sneaky (opening)", (CardFx)new IntroFx(0.78f, IntroSpot.OnBody, Intros.Sneaky));
			CardFxManager.Register<Backstab>("Backstab", (CardFx)new BackstabFx());
			CardFxManager.Register<Dash>("Dash", (CardFx)new DashFx());
			CardFxManager.Register<EchoingSlash>("Echoing Slash", (CardFx)new EchoingSlashFx());
			CardFxManager.Register<Finisher>("Finisher", (CardFx)new FinisherFx());
			CardFxManager.Register<Flechettes>("Flechettes", (CardFx)new FlechettesThrowFx());
			CardFxManager.Register<MementoMori>("Memento Mori", (CardFx)new MementoStrikeFx());
			CardFxManager.Register<Pinpoint>("Pinpoint", (CardFx)new PinpointFx());
			CardFxManager.Register<Pounce>("Pounce", (CardFx)new PounceFx());
			CardFxManager.Register<PreciseCut>("Precise Cut", (CardFx)new PreciseCutFx());
			CardFxManager.Register<Skewer>("Skewer", (CardFx)new SkewerFx());
			CardFxManager.Register<Strangle>("Strangle", (CardFx)new StrangleCardFx());
			CardFxManager.Register<Suppress>("Suppress", (CardFx)new SuppressFx());
			CardFxManager.Register<Predator>("Predator", (CardFx)new PredatorFx());
			CardFxManager.Register<Ricochet>("Ricochet", (CardFx)new RicochetFx());
			CardFxManager.Register<SuckerPunch>("Sucker Punch", (CardFx)new SuckerPunchFx());
			CardFxManager.Register<FlickFlack>("Flick-Flack", (CardFx)new FlickFlackFx());
			CardFxManager.Register<Acrobatics>("Acrobatics", (CardFx)new AcrobaticsFx());
			CardFxManager.Register<Backflip>("Backflip", (CardFx)new BackflipFx());
			CardFxManager.Register<Blur>("Blur", (CardFx)new BlurFx());
			Catalog.Add(("Calculated Gamble", new HeldOnlyFx(), HeldFxManager.For(typeof(CalculatedGamble))));
			CardFxManager.Register<EscapePlan>("Escape Plan", (CardFx)new EscapePlanFx());
			if (GameCapabilities.ExpertiseRetainsDrawnCards)
			{
				CardFxManager.Register<Expertise>("Expertise", (CardFx)new ExpertiseFx());
			}
			CardFxManager.Register<Expose>("Expose", (CardFx)new ExposeFx());
			RegisterOptional("Fade", "Fade", () => new FadeFx());
			CardFxManager.Register<Footwork>("Footwork", (CardFx)new FootworkFx());
			CardFxManager.Register<HandTrick>("Hand Trick", (CardFx)new HandTrickFx());
			CardFxManager.Register<LegSweep>("Leg Sweep", (CardFx)new LegSweepFx());
			CardFxManager.Register<Reflex>("Reflex", (CardFx)new ReflexFx());
			RegisterOptional("Sidestep", "Sidestep", () => new SidestepFx());
			CardFxManager.Register<Tactician>("Tactician", (CardFx)new TacticianFx());
			CardFxManager.Register<Deflect>("Deflect", (CardFx)new DeflectFx());
			CardFxManager.Register<DodgeAndRoll>("Dodge and Roll", (CardFx)new DodgeAndRollFx());
			CardFxManager.Register<PiercingWail>("Piercing Wail", (CardFx)new PiercingWailFx());
			CardFxManager.Register<Anticipate>("Anticipate", (CardFx)new AnticipateFx());
			CardFxManager.Register<Prepared>("Prepared", (CardFx)new PreparedFx());
			CardFxManager.Register<Untouchable>("Untouchable", (CardFx)new UntouchableFx());
			Catalog.Add(("Corrosive Wave (standby only)", new HeldOnlyFx(), HeldFxManager.For(typeof(CorrosiveWave))));
			ShivFx.Init();
			Catalog.Add(("Shiv (plain) + hit", new ShivPreviewFx(ShivStyle.Plain, 1), null));
			Catalog.Add(("Shiv hit with Accuracy", new ShivPreviewFx(ShivStyle.Plain, 1, accuracy: true), null));
			Catalog.Add(("Shiv hit with Phantom Blades", new ShivPreviewFx(ShivStyle.Plain, 1, accuracy: false, phantom: true), null));
			Catalog.Add(("Blade of Ink", new ShivPreviewFx(ShivStyle.Ink, 2), new ShivHeldFx(ShivStyle.Ink, 2)));
			Catalog.Add(("Blade Dance", new ShivPreviewFx(ShivStyle.Dance, 3), new ShivHeldFx(ShivStyle.Dance)));
			Catalog.Add(("Cloak and Dagger", new ShivPreviewFx(ShivStyle.Cloak, 1), new ShivHeldFx(ShivStyle.Cloak, 1)));
			Catalog.Add(("Hidden Daggers", new ShivPreviewFx(ShivStyle.Sleeves, 2), new ShivHeldFx(ShivStyle.Sleeves, 2)));
			Catalog.Add(("Up My Sleeve", new ShivPreviewFx(ShivStyle.Fan, 3), new ShivHeldFx(ShivStyle.Fan)));
			if (GameCapabilities.Card("BladeSymphony") != null)
			{
				Catalog.Add(("Blade Symphony", new ShivPreviewFx(ShivStyle.Symphony, 3), new ShivHeldFx(ShivStyle.Symphony)));
			}
			Catalog.Add(("Leading Strike", new ShivPreviewFx(ShivStyle.Return, 2), new ShivHeldFx(ShivStyle.Return, 1)));
			DevLog.Info($"CardFxManager ready, {Map.Count} card effect(s) registered");
		}

		public static void Register<T>(string name, CardFx fx) where T : CardModel
		{
			Register(typeof(T), name, fx);
		}

		private static void Register(Type type, string name, CardFx fx)
		{
			Map[type] = fx;
			if (DrawsHits(fx))
			{
				HitTypes.Add(type);
			}
			else
			{
				HitTypes.Remove(type);
			}
			Catalog.Add((name, fx, HeldFxManager.For(type)));
		}

		private static void RegisterOptional(string cardName, string name, Func<CardFx> make)
		{
			Type type = GameCapabilities.Card(cardName);
			if (type == null)
			{
				DevLog.Info("Card FX unavailable in this game build: " + cardName);
				return;
			}
			try
			{
				Register(type, name, make());
			}
			catch (Exception ex)
			{
				DevLog.Error("Optional card FX setup failed (" + cardName + "): " + ex);
			}
		}

		private static bool IsLocal(CardModel card)
		{
			try
			{
				return LocalContext.IsMine(card);
			}
			catch
			{
				return true;
			}
		}

		private static bool DrawsHits(CardFx fx)
		{
			if (fx == null)
			{
				return false;
			}
			try
			{
				MethodInfo method = fx.GetType().GetMethod("OnHit", new Type[3]
				{
					typeof(CardFxContext),
					typeof(Vector2),
					typeof(bool)
				});
				return method == null || method.DeclaringType != typeof(CardFx);
			}
			catch (Exception ex)
			{
				DevLog.Warn("CardFxManager.DrawsHits: " + ex.Message);
				return true;
			}
		}

		public static void OnCardPlayed(CardPlay play)
		{
			CardModel val = ((play != null) ? play.Card : null);
			if (val == null || !CanShow(val))
			{
				return;
			}
			HeldSession heldSession = ((MultiplayerFx.Active && !MultiplayerFx.IsLocal(val)) ? RemoteHeldFx.Claim(val, play.Target) : HeldFxManager.Claim(val));
			if (!PlayerSettings.Enabled)
			{
				if (heldSession != null && !heldSession.Finished)
				{
					heldSession.Leave();
				}
				return;
			}
			Map.TryGetValue(((object)val).GetType(), out var value);
			if (value == null && heldSession == null)
			{
				return;
			}
			Player owner = val.Owner;
			CardFxContext cardFxContext = BuildContext((owner != null) ? owner.Creature : null, play.Target);
			if (cardFxContext == null)
			{
				if (heldSession != null && !heldSession.Finished)
				{
					heldSession.Leave();
				}
				return;
			}
			cardFxContext.Card = val;
			cardFxContext.Remote = MultiplayerFx.Active && !MultiplayerFx.IsLocal(val);
			cardFxContext.Held = heldSession;
			bool flag = false;
			try
			{
				if (value != null)
				{
					DevLog.Info("Card FX: " + ((object)val).GetType().Name + ((heldSession != null) ? " (from standby)" : ""));
					using (RemoteFxFade.Capture(cardFxContext.Remote, cardFxContext.BackParent, cardFxContext.FrontParent, GameRefs.Overlay, (Node)GameRefs.CombatRoom))
					{
						value.OnPlay(cardFxContext);
					}
				}
				HeldFxManager.PlayManual(heldSession, cardFxContext);
				flag = true;
			}
			finally
			{
				if (!flag && heldSession != null && !heldSession.Finished)
				{
					heldSession.Leave();
				}
			}
		}

		public static void OnDamage(CardModel source, Creature target, DamageResult result)
		{
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			if (!PlayerSettings.Enabled || source == null || target == null || !HitTypes.Contains(((object)source).GetType()) || !Map.TryGetValue(((object)source).GetType(), out var value) || !CanShow(source))
			{
				return;
			}
			Player owner = source.Owner;
			CardFxContext cardFxContext = BuildContext((owner != null) ? owner.Creature : null, target);
			if (cardFxContext == null || !cardFxContext.HasTarget)
			{
				return;
			}
			cardFxContext.Card = source;
			cardFxContext.Remote = MultiplayerFx.Active && !MultiplayerFx.IsLocal(source);
			using (RemoteFxFade.Capture(cardFxContext.Remote, cardFxContext.BackParent, cardFxContext.FrontParent, GameRefs.Overlay, (Node)GameRefs.CombatRoom))
			{
				value.OnHit(cardFxContext, cardFxContext.TargetChest, result != null && result.WasTargetKilled);
			}
		}

		public static CardFxContext BuildContext(Creature caster, Creature target)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0323: Unknown result type (might be due to invalid IL or missing references)
			//IL_0355: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			if (caster == null)
			{
				return null;
			}
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			if (combatRoom == null)
			{
				return null;
			}
			NCreature creatureNode = combatRoom.GetCreatureNode(caster);
			if (!FxKit.Alive((GodotObject)(object)creatureNode) || !((Node)creatureNode).IsInsideTree())
			{
				return null;
			}
			CardFxContext cardFxContext = new CardFxContext
			{
				BackParent = (Node)(object)combatRoom.BackCombatVfxContainer,
				FrontParent = (Node)(object)combatRoom.CombatVfxContainer,
				Feet = GameRefs.BottomOf(creatureNode),
				Chest = creatureNode.VfxSpawnPosition,
				Body = (Vector2)(FxKit.Alive((GodotObject)(object)creatureNode.Hitbox) ? GameRefs.DrawnSize(creatureNode.Hitbox, (Node)(object)combatRoom.CombatVfxContainer) : new Vector2(244f, 244f)),
				View = FxKit.ViewSize(),
				Facing = GameRefs.FacingOf(creatureNode),
				Target = target
			};
			if (!FxKit.Alive((GodotObject)(object)cardFxContext.BackParent) || !FxKit.Alive((GodotObject)(object)cardFxContext.FrontParent))
			{
				return null;
			}
			cardFxContext.ExhaustPile = new Vector2(cardFxContext.View.X * 0.953f, cardFxContext.View.Y * 0.73f);
			try
			{
				NCombatUi ui = combatRoom.Ui;
				if (FxKit.Alive((GodotObject)(object)ui))
				{
					cardFxContext.UiParent = (Node)(object)ui;
					NExhaustPileButton exhaustPile = ui.ExhaustPile;
					if (FxKit.Alive((GodotObject)(object)exhaustPile) && ((Node)exhaustPile).IsInsideTree())
					{
						Vector2 val = ((Control)exhaustPile).GlobalPosition + ((Control)exhaustPile).Size * 0.5f;
						if (val.X > 0f && val.X < cardFxContext.View.X && val.Y > 0f && val.Y < cardFxContext.View.Y)
						{
							cardFxContext.ExhaustPile = val;
						}
					}
				}
			}
			catch
			{
			}
			if (target != null)
			{
				NCreature creatureNode2 = combatRoom.GetCreatureNode(target);
				if (FxKit.Alive((GodotObject)(object)creatureNode2) && ((Node)creatureNode2).IsInsideTree())
				{
					cardFxContext.HasTarget = true;
					cardFxContext.TargetFeet = GameRefs.BottomOf(creatureNode2);
					cardFxContext.TargetChest = creatureNode2.VfxSpawnPosition;
					Vector2 val2 = GameRefs.DrawnSize(creatureNode2.Hitbox, (Node)(object)combatRoom.CombatVfxContainer);
					cardFxContext.TargetWidth = (FxKit.Alive((GodotObject)(object)creatureNode2.Hitbox) ? val2.X : 200f);
					cardFxContext.TargetHeight = (FxKit.Alive((GodotObject)(object)creatureNode2.Hitbox) ? val2.Y : 240f);
					cardFxContext.TargetPoison = target.GetPowerAmount<PoisonPower>();
					cardFxContext.TargetSide = ((cardFxContext.TargetChest.X < cardFxContext.Chest.X) ? (-1f) : 1f);
					if (target != caster)
					{
						cardFxContext.Facing = cardFxContext.TargetSide;
					}
				}
			}
			ICombatState combatState = caster.CombatState;
			if (combatState != null)
			{
				foreach (Creature hittableEnemy in combatState.HittableEnemies)
				{
					NCreature creatureNode3 = combatRoom.GetCreatureNode(hittableEnemy);
					if (FxKit.Alive((GodotObject)(object)creatureNode3) && ((Node)creatureNode3).IsInsideTree())
					{
						cardFxContext.Enemies.Add(new FxTarget
						{
							Key = hittableEnemy,
							Feet = GameRefs.BottomOf(creatureNode3),
							Chest = creatureNode3.VfxSpawnPosition,
							Width = (FxKit.Alive((GodotObject)(object)creatureNode3.Hitbox) ? GameRefs.DrawnSize(creatureNode3.Hitbox, (Node)(object)combatRoom.CombatVfxContainer).X : 200f),
							Height = (FxKit.Alive((GodotObject)(object)creatureNode3.Hitbox) ? GameRefs.DrawnSize(creatureNode3.Hitbox, (Node)(object)combatRoom.CombatVfxContainer).Y : 240f),
							Weak = hittableEnemy.HasPower<WeakPower>(),
							Poison = hittableEnemy.GetPowerAmount<PoisonPower>(),
							Side = ((creatureNode3.VfxSpawnPosition.X < cardFxContext.Chest.X) ? (-1f) : 1f)
						});
					}
				}
			}
			return cardFxContext;
		}
	}
