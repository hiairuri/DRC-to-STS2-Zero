// namespace: SilentFX.Powers
// type: StrangleGripFx (class)
// source lines: 14108-14600
	public sealed class StrangleGripFx : BasicPowerFx
	{
		private static readonly List<StrangleGripFx> Live = new List<StrangleGripFx>();

		private static bool _hooked;

		private CardModel _for;

		private bool _let;

		private float _letFor;

		private CardModel _seen;

		private bool _seenPlayable;

		private bool _answered;

		private StranglePower _power;

		private Creature _applier;

		private Node _room;

		private Node _applierNode;

		private Node _victimNode;

		private bool _multiplayer;

		private CardModel _hung;

		private CardModel _claimed;

		private CardModel _triggerCard;

		private object _triggerInvocation;

		private readonly Dictionary<object, CardModel> _answers = new Dictionary<object, CardModel>(ReferenceEqualityComparer.Instance);

		private readonly Dictionary<object, CardModel> _finished = new Dictionary<object, CardModel>(ReferenceEqualityComparer.Instance);

		protected override string NodeName => "Strangle";

		protected override bool UseBack => false;

		protected override bool UseFront => false;

		internal void Bind(StranglePower power)
		{
			_power = power;
			_applier = ((power != null) ? ((PowerModel)power).Applier : null);
			_multiplayer = MultiplayerFx.Active;
			if (_multiplayer)
			{
				_room = (Node)(object)GameRefs.CombatRoom;
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				_applierNode = (Node)(object)((combatRoom != null) ? combatRoom.GetCreatureNode(_applier) : null);
				NCombatRoom combatRoom2 = GameRefs.CombatRoom;
				_victimNode = (Node)(object)((combatRoom2 != null) ? combatRoom2.GetCreatureNode((power != null) ? ((PowerModel)power).Owner : null) : null);
			}
		}

		protected override void Build(bool animateIn)
		{
			Live.Add(this);
			if (!_hooked)
			{
				_hooked = true;
				HeldFxManager.LetGo += OnLetGo;
			}
		}

		protected override void Layout()
		{
		}

		private static void OnLetGo(CardModel card, bool played)
		{
			if (MultiplayerFx.Active)
			{
				if (card == null)
				{
					return;
				}
				{
					foreach (StrangleGripFx item in Live)
					{
						if (!item.ValidPlay(card) || !MultiplayerFx.IsLocal(item._applier))
						{
							continue;
						}
						if (played)
						{
							item.NotePlayed(card);
							continue;
						}
						if (item._claimed == card)
						{
							item._claimed = null;
						}
						if (item._hung == card)
						{
							item.ReleaseWaiting();
							item._let = false;
						}
					}
					return;
				}
			}
			if (!played || card == null)
			{
				return;
			}
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				StrangleGripFx strangleGripFx = Live[num];
				if (!strangleGripFx.Dead && strangleGripFx._for == card)
				{
					strangleGripFx._let = true;
					strangleGripFx._letFor = 0f;
				}
			}
		}

		internal static void Answer(Creature who, CardModel card = null)
		{
			if (who == null)
			{
				return;
			}
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				StrangleGripFx strangleGripFx = Live[num];
				if (!strangleGripFx.Dead && strangleGripFx.Anchor != null && strangleGripFx.Anchor.Owner == who)
				{
					if (MultiplayerFx.Active)
					{
						object currentInvocation = MultiplayerFx.CurrentInvocation;
						if (currentInvocation != null && card == MultiplayerFx.CurrentCard && strangleGripFx.ValidPlay(card))
						{
							strangleGripFx._answers[currentInvocation] = card;
							strangleGripFx._claimed = card;
							if (strangleGripFx._hung == card)
							{
								strangleGripFx._let = false;
								strangleGripFx.ReleaseWaiting();
							}
						}
					}
					else
					{
						strangleGripFx._answered = true;
						strangleGripFx._let = false;
						strangleGripFx._for = null;
					}
				}
			}
		}

		protected override void Tick(float dt)
		{
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			if (_multiplayer)
			{
				if (!ValidBinding())
				{
					Dispose();
				}
				else
				{
					TickMultiplayer(dt);
				}
				return;
			}
			if (MultiplayerFx.Active && Anchor.Owner is Creature)
			{
				Dispose();
				return;
			}
			object owner = Anchor.Owner;
			Creature val = (Creature)((owner is Creature) ? owner : null);
			if (val == null || Stopping)
			{
				_for = null;
				_let = false;
				return;
			}
			if (_let)
			{
				_letFor += dt;
				if (_letFor > Mathf.Clamp(Tuning.F("strangle.await", 4f), 0.5f, 30f))
				{
					_let = false;
				}
			}
			if (!_let)
			{
				CardModel carried = HeldFxManager.Carried;
				if (carried != _seen)
				{
					_seen = carried;
					_seenPlayable = false;
					if (carried != null)
					{
						_answered = false;
						try
						{
							_seenPlayable = carried.CanPlay();
						}
						catch
						{
							_seenPlayable = true;
						}
					}
				}
				_for = ((carried != null && _seenPlayable && HeldFxManager.CarriedOut) ? carried : null);
			}
			if (_for != null && val.IsAlive)
			{
				Vector2 val2 = Anchor.SafeBodySize();
				StrangleRope.Hang(Anchor.BackParent, Anchor.FrontParent, Anchor.Chest(), val2.X, val2.Y, val);
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			if (_multiplayer)
			{
				if (!ValidBinding())
				{
					Dispose();
					return;
				}
				CardModel triggerCard = _triggerCard;
				object triggerInvocation = _triggerInvocation;
				if (triggerInvocation != null && ValidPlay(triggerCard) && !_finished.ContainsKey(triggerInvocation))
				{
					_finished[triggerInvocation] = triggerCard;
					_claimed = triggerCard;
					bool num = _answers.Remove(triggerInvocation, out var value) && value == triggerCard;
					object owner = Anchor.Owner;
					Creature who = (Creature)((owner is Creature) ? owner : null);
					Vector2 val = Anchor.SafeBodySize();
					Vector2 at2 = Anchor.Chest();
					bool flag = !num && StrangleRope.Tighten(who, at2, triggerCard, wait: false, Anchor.FrontParent);
					if (_hung == triggerCard)
					{
						_let = false;
						ReleaseWaiting();
					}
					if (!num && !flag)
					{
						StrangleRope.Play(Anchor.BackParent, Anchor.FrontParent, at2, val.X, val.Y, who, triggerCard);
					}
				}
			}
			else
			{
				if ((MultiplayerFx.Active && Anchor.Owner is Creature) || !Anchor.Valid())
				{
					return;
				}
				try
				{
					_let = false;
					_for = null;
					if (_answered)
					{
						_answered = false;
						return;
					}
					Vector2 val2 = Anchor.SafeBodySize();
					Vector2 at3 = Anchor.Chest();
					object owner2 = Anchor.Owner;
					if (!StrangleRope.Tighten((Creature)((owner2 is Creature) ? owner2 : null), at3, null, wait: false))
					{
						StrangleRope.Play(Anchor.BackParent, Anchor.FrontParent, at3, val2.X, val2.Y);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Strangle (power): " + ex.Message);
				}
			}
		}

		protected override void OnDispose()
		{
			ReleaseWaiting();
			_answers.Clear();
			_finished.Clear();
			Live.Remove(this);
		}

		private bool ValidBinding()
		{
			if (_multiplayer && MultiplayerFx.Active && !Dead && !Stopping && _power != null && _applier != null && ((PowerModel)_power).Applier == _applier && ((PowerModel)_power).Owner == Anchor.Owner && ((PowerModel)_power).Owner != null && ((PowerModel)_power).Owner.IsAlive && FxKit.Alive((GodotObject)(object)_room) && _room.IsInsideTree() && (object)_room == GameRefs.CombatRoom && FxKit.Alive((GodotObject)(object)_applierNode) && _applierNode.IsInsideTree() && FxKit.Alive((GodotObject)(object)_victimNode) && _victimNode.IsInsideTree())
			{
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				if ((object)((combatRoom != null) ? combatRoom.GetCreatureNode(_applier) : null) == _applierNode)
				{
					NCombatRoom combatRoom2 = GameRefs.CombatRoom;
					if ((object)((combatRoom2 != null) ? combatRoom2.GetCreatureNode(((PowerModel)_power).Owner) : null) == _victimNode)
					{
						CombatManager instance = CombatManager.Instance;
						CombatState val = ((instance != null) ? instance.DebugOnlyGetState() : null);
						if (val == null || (object)_applier.CombatState != val || _applier.Player == null || _applier.Player.Creature != _applier)
						{
							return false;
						}
						bool flag = false;
						bool flag2 = false;
						foreach (Player player in val.Players)
						{
							if (player == _applier.Player)
							{
								flag = true;
								break;
							}
						}
						foreach (PowerModel power in ((PowerModel)_power).Owner.Powers)
						{
							if ((object)power == _power)
							{
								flag2 = true;
								break;
							}
						}
						return flag & flag2;
					}
				}
			}
			return false;
		}

		private bool ValidPlay(CardModel card)
		{
			if (ValidBinding() && card != null)
			{
				return card.Owner == _applier.Player;
			}
			return false;
		}

		internal void NotePlayed(CardModel card)
		{
			if (ValidPlay(card))
			{
				_claimed = card;
				if (_hung == card)
				{
					_let = true;
					_letFor = 0f;
				}
			}
		}

		internal void TriggerCard(CardModel card, object invocation)
		{
			CardModel triggerCard = _triggerCard;
			object triggerInvocation = _triggerInvocation;
			_triggerCard = card;
			_triggerInvocation = invocation;
			try
			{
				Trigger();
			}
			finally
			{
				_triggerCard = triggerCard;
				_triggerInvocation = triggerInvocation;
			}
		}

		private void ReleaseWaiting()
		{
			if (_hung != null)
			{
				object obj = Anchor?.Owner;
				StrangleRope.ReleaseHang((Creature)((obj is Creature) ? obj : null), _hung, this);
			}
			_hung = null;
			_for = null;
		}

		private void TickMultiplayer(float dt)
		{
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			if (!_applier.IsAlive)
			{
				_let = false;
				ReleaseWaiting();
				return;
			}
			if (_let)
			{
				_letFor += dt;
				if (_letFor > Mathf.Clamp(Tuning.F("strangle.await", 4f), 0.5f, 30f))
				{
					_let = false;
					ReleaseWaiting();
				}
			}
			if (!_let)
			{
				CardModel card;
				bool flag;
				if (MultiplayerFx.IsLocal(_applier))
				{
					card = HeldFxManager.Carried;
					if (card == null)
					{
						_claimed = null;
					}
					if (card != null && card.Owner != _applier.Player)
					{
						card = null;
					}
					if (card != _seen)
					{
						_seen = card;
						_seenPlayable = false;
						if (card != null)
						{
							try
							{
								_seenPlayable = card.CanPlay();
							}
							catch
							{
								_seenPlayable = true;
							}
						}
					}
					flag = card != null && _seenPlayable && HeldFxManager.CarriedOut;
				}
				else
				{
					bool flag2 = RemoteHeldFx.TryObserve(_applier, out card, out var target, out var released, out var inPlayZone);
					if (released && (card == null || card == _claimed))
					{
						_claimed = null;
					}
					flag = flag2 && (inPlayZone || target != null);
				}
				if (!flag || !ValidPlay(card) || card == _claimed)
				{
					card = null;
				}
				if (_hung != card)
				{
					ReleaseWaiting();
				}
				_hung = (_for = card);
			}
			if (_hung != null)
			{
				Vector2 val = Anchor.SafeBodySize();
				Node backParent = Anchor.BackParent;
				Node frontParent = Anchor.FrontParent;
				Vector2 at = Anchor.Chest();
				float x = val.X;
				float y = val.Y;
				object owner = Anchor.Owner;
				StrangleRope.Hang(backParent, frontParent, at, x, y, (Creature)((owner is Creature) ? owner : null), _hung, this);
			}
		}
	}
