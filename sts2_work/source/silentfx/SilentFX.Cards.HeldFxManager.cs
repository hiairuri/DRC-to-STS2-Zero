// namespace: SilentFX.Cards
// type: HeldFxManager (class)
// source lines: 68471-69093
	public static class HeldFxManager
	{
		private static readonly Dictionary<Type, HeldFx> Map = new Dictionary<Type, HeldFx>();

		private static readonly List<HeldSession> Live = new List<HeldSession>();

		private static readonly List<HeldSession> Waiting = new List<HeldSession>();

		private static readonly List<HeldSession> Playing = new List<HeldSession>();

		private static HeldSession _held;

		private static HeldFx _heldFx;

		private static Func<bool> _overPlayArea;

		private static Creature _target;

		private static HeldSession _hover;

		private static CardModel _hoverCard;

		private static CardModel _hoverSeen;

		private static float _hoverDwell;

		private static bool _hoverRefused;

		private static Func<bool> _carriedOut;

		public static CardModel Carried { get; private set; }

		public static bool CarriedOut
		{
			get
			{
				if (Carried == null)
				{
					return false;
				}
				if (_carriedOut == null)
				{
					return true;
				}
				try
				{
					return _carriedOut();
				}
				catch
				{
					return false;
				}
			}
		}

		public static Creature PointedAt
		{
			get
			{
				if (Carried == null || _held == null || _held.Finished)
				{
					return null;
				}
				return _target;
			}
		}

		public static bool ArmedAtAll
		{
			get
			{
				CardModel carried = Carried;
				if (carried == null || _held == null || _held.Finished || _held.C.Card != carried)
				{
					return false;
				}
				if (_held.Armed)
				{
					return ShivFan.Active(carried);
				}
				return false;
			}
		}

		public static event Action<CardModel, bool> LetGo;

		public static void Init()
		{
			RemoteFxFocus.Reset();
			Ticker.Add(RemoteFxFocus.Update);
			HeldFxManager.Register<BladeDance>((HeldFx)new ShivHeldFx(ShivStyle.Dance));
			HeldFxManager.Register<CloakAndDagger>((HeldFx)new ShivHeldFx(ShivStyle.Cloak));
			HeldFxManager.Register<HiddenDaggers>((HeldFx)new ShivHeldFx(ShivStyle.Sleeves));
			HeldFxManager.Register<UpMySleeve>((HeldFx)new ShivHeldFx(ShivStyle.Fan));
			Type type = GameCapabilities.Card("BladeSymphony");
			if (type != null)
			{
				try
				{
					Map[type] = new ShivHeldFx(ShivStyle.Symphony);
				}
				catch (Exception ex)
				{
					DevLog.Error("Optional standby setup failed (BladeSymphony): " + ex);
				}
			}
			HeldFxManager.Register<LeadingStrike>((HeldFx)new ShivHeldFx(ShivStyle.Return));
			HeldFxManager.Register<BladeOfInk>((HeldFx)new ShivHeldFx(ShivStyle.Ink));
			HeldFxManager.Register<StormOfSteel>((HeldFx)new HeldFx<StormHeld>(aimed: false, hover: true));
			HeldFxManager.Register<KnifeTrap>((HeldFx)new HeldFx<KnifeTrapHeld>(aimed: true));
			HeldFxManager.Register<Shiv>((HeldFx)new ShivHeldFx(ShivStyle.Return));
			HeldFxManager.Register<DeadlyPoison>((HeldFx)new HeldFx<DeadlyPoisonHeld>(aimed: true));
			HeldFxManager.Register<Snakebite>((HeldFx)new HeldFx<SnakebiteHeld>(aimed: true));
			HeldFxManager.Register<PoisonedStab>((HeldFx)new HeldFx<PoisonedStabHeld>(aimed: true));
			HeldFxManager.Register<BubbleBubble>((HeldFx)new HeldFx<BubbleBubbleHeld>(aimed: true));
			HeldFxManager.Register<Outbreak>((HeldFx)new HeldFx<OutbreakHeld>());
			HeldFxManager.Register<Mirage>((HeldFx)new HeldFx<MirageHeld>());
			HeldFxManager.Register<CorrosiveWave>((HeldFx)new HeldFx<CorrosiveWaveHeld>());
			HeldFxManager.Register<Malaise>((HeldFx)new HeldFx<MalaiseHeld>(aimed: true));
			HeldFxManager.Register<Murder>((HeldFx)new HeldFx<MurderHeld>(aimed: true, hover: true));
			HeldFxManager.Register<Assassinate>((HeldFx)new HeldFx<AssassinateHeld>(aimed: true));
			HeldFxManager.Register<BulletTime>((HeldFx)new HeldFx<BulletTimeHeld>(aimed: false, hover: true));
			HeldFxManager.Register<ShadowStep>((HeldFx)new HeldFx<ShadowStepHeld>(aimed: false, hover: true));
			HeldFxManager.Register<Shadowmeld>((HeldFx)new HeldFx<ShadowmeldHeld>());
			HeldFxManager.Register<TheHunt>((HeldFx)new HeldFx<TheHuntHeld>(aimed: true));
			HeldFxManager.Register<Backstab>((HeldFx)new HeldFx<BackstabHeld>(aimed: true));
			HeldFxManager.Register<Pinpoint>((HeldFx)new HeldFx<PinpointHeld>(aimed: true));
			HeldFxManager.Register<PreciseCut>((HeldFx)new HeldFx<PreciseCutHeld>(aimed: true));
			HeldFxManager.Register<Flechettes>((HeldFx)new HeldFx<FlechettesOrbit>(aimed: true, hover: true));
			HeldFxManager.Register<MementoMori>((HeldFx)new HeldFx<MementoHeld>(aimed: true, hover: true));
			HeldFxManager.Register<CalculatedGamble>((HeldFx)new HeldFx<GambleHeld>());
			HeldFxManager.Register<Finisher>((HeldFx)new HeldFx<FinisherHeld>(aimed: true, hover: true));
			HeldFxManager.Register<Skewer>((HeldFx)new HeldFx<SkewerHeld>(aimed: true));
			HeldFxManager.Register<Strangle>((HeldFx)new HeldFx<StrangleHeld>(aimed: true));
			HeldFxManager.Register<Suppress>((HeldFx)new HeldFx<SuppressHeld>(aimed: true, hover: true));
			HeldFxManager.Register<WraithForm>((HeldFx)new WraithHeldChoice());
			WraithStage.Init();
			SuppressStage.Init();
			PlayerSettings.Changed += OnSettingsChanged;
			Ticker.Add(Update);
			DevLog.Info($"HeldFxManager ready, {Map.Count} standby effect(s) registered");
		}

		public static void Register<T>(HeldFx fx) where T : CardModel
		{
			Map[typeof(T)] = fx;
		}

		public static HeldFx For(Type cardType)
		{
			if (!(cardType != null) || !Map.TryGetValue(cardType, out var value))
			{
				return null;
			}
			return value;
		}

		public static void Begin(CardModel card, Func<bool> overPlayArea)
		{
			Carried = card;
			_carriedOut = overPlayArea;
			RemoteFxFocus.Begin(card);
			DropHeld();
			if (!PlayerSettings.Enabled)
			{
				DropHover();
				return;
			}
			if (card != null && _hover != null && !_hover.Finished && _hoverCard == card && Map.TryGetValue(((object)card).GetType(), out var value))
			{
				_held = _hover;
				_heldFx = value;
				_overPlayArea = overPlayArea;
				_target = null;
				_hover = null;
				_hoverCard = null;
				_held.Hovering = false;
				if (PowerFxManager.LogTriggers)
				{
					DevLog.Info("Held FX: picked up " + ((object)card).GetType().Name + " (from hover)");
				}
				return;
			}
			DropHover();
			if (card == null || !Map.TryGetValue(((object)card).GetType(), out var value2))
			{
				return;
			}
			bool flag;
			try
			{
				flag = card.CanPlay();
			}
			catch
			{
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			Player owner = card.Owner;
			CardFxContext cardFxContext = CardFxManager.BuildContext((owner != null) ? owner.Creature : null, null);
			if (cardFxContext == null)
			{
				return;
			}
			cardFxContext.Card = card;
			HeldSession heldSession = StartSession(value2, cardFxContext);
			if (heldSession != null)
			{
				_held = heldSession;
				_heldFx = value2;
				_overPlayArea = overPlayArea;
				_target = null;
				if (PowerFxManager.LogTriggers)
				{
					DevLog.Info("Held FX: picked up " + ((object)card).GetType().Name);
				}
			}
		}

		public static void SetTarget(Creature target)
		{
			if (_held == null || _held.Finished || target == _target)
			{
				return;
			}
			_target = target;
			CardFxContext target2 = null;
			if (target != null)
			{
				CardModel card = _held.C.Card;
				object caster;
				if (card == null)
				{
					caster = null;
				}
				else
				{
					Player owner = card.Owner;
					caster = ((owner != null) ? owner.Creature : null);
				}
				target2 = CardFxManager.BuildContext((Creature)caster, target);
			}
			_held.SetTarget(target2);
		}

		public static void End(bool played)
		{
			CardModel carried = Carried;
			Carried = null;
			_carriedOut = null;
			RemoteFxFocus.LetGo(carried, played);
			if (carried != null)
			{
				try
				{
					LetGo?.Invoke(carried, played);
				}
				catch (Exception ex)
				{
					DevLog.Warn("HeldFxManager (let go): " + ex);
				}
			}
			HeldSession held = _held;
			HeldFx heldFx = _heldFx;
			_held = null;
			_heldFx = null;
			_overPlayArea = null;
			_target = null;
			if (held == null || held.Finished)
			{
				return;
			}
			if (played)
			{
				held.Release();
				Waiting.Add(held);
			}
			else if (heldFx != null && held.C.Card != null && heldFx.HoverFor(held.C.Card))
			{
				held.Armed = false;
				held.Hovering = true;
				if (held.C.HasTarget)
				{
					held.SetTarget(null);
				}
				_hover = held;
				_hoverCard = held.C.Card;
			}
			else
			{
				held.Leave();
			}
		}

		private static void DropHover()
		{
			if (_hover != null && !_hover.Finished)
			{
				_hover.Leave();
			}
			_hover = null;
			_hoverCard = null;
		}

		private static void PollHover(float realDt)
		{
			if (!PlayerSettings.Enabled)
			{
				DropHover();
				_hoverSeen = null;
				_hoverDwell = 0f;
				_hoverRefused = false;
				return;
			}
			if (_hover != null && _hover.Finished)
			{
				_hover = null;
				_hoverCard = null;
			}
			if (_held != null && !_held.Finished)
			{
				return;
			}
			CardModel val = null;
			try
			{
				val = HeldHooks.Hovered();
			}
			catch
			{
				val = null;
			}
			if (val != _hoverSeen)
			{
				_hoverSeen = val;
				_hoverDwell = 0f;
				_hoverRefused = false;
			}
			_hoverDwell += realDt;
			if (_hover != null)
			{
				if (val == _hoverCard || _hoverDwell < 0.12f)
				{
					return;
				}
				DropHover();
			}
			if (val == null || _hoverRefused || _hoverDwell < 0.12f)
			{
				return;
			}
			_hoverRefused = true;
			if (!Map.TryGetValue(((object)val).GetType(), out var value) || !value.HoverFor(val))
			{
				return;
			}
			bool flag;
			try
			{
				flag = val.CanPlay();
			}
			catch
			{
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			Player owner = val.Owner;
			CardFxContext cardFxContext = CardFxManager.BuildContext((owner != null) ? owner.Creature : null, null);
			if (cardFxContext == null)
			{
				return;
			}
			cardFxContext.Card = val;
			HeldSession heldSession = StartSession(value, cardFxContext, hovering: true);
			if (heldSession != null)
			{
				_hover = heldSession;
				_hoverCard = val;
				if (PowerFxManager.LogTriggers)
				{
					DevLog.Info("Held FX: hovering over " + ((object)val).GetType().Name);
				}
			}
		}

		private static void DropHeld()
		{
			if (_held != null && !_held.Finished)
			{
				_held.Leave();
			}
			_held = null;
			_heldFx = null;
			_overPlayArea = null;
			_target = null;
		}

		public static HeldSession Claim(CardModel card)
		{
			if (card == null)
			{
				return null;
			}
			for (int i = 0; i < Waiting.Count; i++)
			{
				HeldSession heldSession = Waiting[i];
				if (heldSession.C.Card == card)
				{
					Waiting.RemoveAt(i);
					if (heldSession.Finished)
					{
						return null;
					}
					Playing.Add(heldSession);
					return heldSession;
				}
			}
			if (_held != null && !_held.Finished && _held.C.Card == card)
			{
				HeldSession held = _held;
				_held = null;
				_heldFx = null;
				_overPlayArea = null;
				held.Release();
				Playing.Add(held);
				return held;
			}
			if (_hover != null && !_hover.Finished && _hover.C.Card == card && ShivFan.Active(card))
			{
				HeldSession hover = _hover;
				_hover = null;
				_hoverCard = null;
				hover.Hovering = false;
				hover.Release();
				Playing.Add(hover);
				return hover;
			}
			return null;
		}

		public static HeldSession PlayingFor(CardModel card)
		{
			if (card == null)
			{
				return null;
			}
			if (MultiplayerFx.Active && !MultiplayerFx.IsLocal(card))
			{
				return RemoteHeldFx.PlayingFor(card);
			}
			foreach (HeldSession item in Playing)
			{
				if (!item.Finished && item.C.Card == card)
				{
					return item;
				}
			}
			return null;
		}

		public static void OnCardFinished(CardModel card)
		{
			if (card == null)
			{
				return;
			}
			RemoteHeldFx.Finish(card);
			for (int num = Playing.Count - 1; num >= 0; num--)
			{
				HeldSession heldSession = Playing[num];
				if (heldSession.C.Card == card)
				{
					Playing.RemoveAt(num);
					if (!heldSession.Finished)
					{
						heldSession.OnCardFinished();
					}
				}
			}
		}

		public static HeldSession StartSession(HeldFx fx, CardFxContext c, bool hovering = false)
		{
			if (!PlayerSettings.Enabled || fx == null || c == null || !FxKit.Alive((GodotObject)(object)c.BackParent) || !FxKit.Alive((GodotObject)(object)c.FrontParent))
			{
				return null;
			}
			HeldSession heldSession = null;
			try
			{
				heldSession = fx.Create();
				heldSession.Hovering = hovering;
				heldSession.Start(c);
				Live.Add(heldSession);
				return heldSession;
			}
			catch (Exception ex)
			{
				if (c.Remote)
				{
					heldSession?.End();
				}
				DevLog.Warn("Held effect failed to start: " + ex);
				return null;
			}
		}

		public static void ReleaseManual(HeldSession session)
		{
			if (session != null && !session.Finished)
			{
				session.Release();
			}
		}

		public static void PlayManual(HeldSession session, CardFxContext play)
		{
			if (session != null && !session.Finished)
			{
				session.Play(play);
			}
		}

		public static void Reset()
		{
			RemoteFxFocus.Reset();
			RemoteHeldFx.Reset();
			AimSync.Reset();
			foreach (HeldSession item in Live)
			{
				item.End();
			}
			Live.Clear();
			Waiting.Clear();
			Playing.Clear();
			_held = null;
			_heldFx = null;
			_overPlayArea = null;
			_target = null;
			_hover = null;
			_hoverCard = null;
			_hoverSeen = null;
			Carried = null;
			_carriedOut = null;
		}

		private static void OnSettingsChanged()
		{
			if (PlayerSettings.Enabled)
			{
				return;
			}
			DropHover();
			DropHeld();
			foreach (HeldSession item in Waiting)
			{
				if (!item.Finished)
				{
					item.Leave();
				}
			}
			Waiting.Clear();
			_hoverSeen = null;
			_hoverDwell = 0f;
			_hoverRefused = false;
		}

		private static void Update(double realDt)
		{
			RemoteHeldFx.Update();
			PollHover((float)realDt);
			if (Live.Count == 0)
			{
				return;
			}
			float dt = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			if (_held != null && !_held.Finished)
			{
				bool flag = true;
				try
				{
					flag = _overPlayArea == null || _overPlayArea();
				}
				catch
				{
					flag = true;
				}
				_held.Armed = ((_heldFx != null && _heldFx.AimedFor(_held.C.Card)) ? _held.C.HasTarget : flag);
			}
			for (int num = Waiting.Count - 1; num >= 0; num--)
			{
				if (Waiting[num].Finished || Waiting[num].ReleasedFor > 6f)
				{
					Waiting[num].Leave();
					Waiting.RemoveAt(num);
				}
			}
			for (int num2 = Live.Count - 1; num2 >= 0; num2--)
			{
				HeldSession heldSession = Live[num2];
				try
				{
					heldSession.Step(dt);
				}
				catch (Exception ex)
				{
					DevLog.Error("Held effect failed and was removed: " + ex);
					heldSession.End();
				}
				if (heldSession.Finished)
				{
					Live.RemoveAt(num2);
					Playing.Remove(heldSession);
				}
			}
		}
	}
