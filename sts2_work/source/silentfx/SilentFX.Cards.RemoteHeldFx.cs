// namespace: SilentFX.Cards
// type: RemoteHeldFx (class)
// source lines: 87677-88271
	internal static class RemoteHeldFx
	{
		private sealed class Peer
		{
			internal HeldSession Held;

			internal CardModel Claimed;
		}

		private static readonly Dictionary<ulong, Peer> Peers = new Dictionary<ulong, Peer>();

		private static readonly Dictionary<CardModel, HeldSession> Playing = new Dictionary<CardModel, HeldSession>(ReferenceEqualityComparer.Instance);

		private static readonly List<HeldSession> Owned = new List<HeldSession>();

		private static readonly HashSet<ulong> Seen = new HashSet<ulong>();

		private static readonly List<ulong> Gone = new List<ulong>();

		private static Node _room;

		private static Func<PeerInputSynchronizer, ulong, object> _readState;

		private static FieldInfo _targeting;

		private static bool _readUnavailable;

		private static bool Ready()
		{
			if (_readUnavailable)
			{
				return false;
			}
			if (_readState != null)
			{
				return true;
			}
			try
			{
				MethodInfo methodInfo = AccessTools.Method(typeof(PeerInputSynchronizer), "GetStateForPlayer", new Type[1] { typeof(ulong) }, (Type[])null);
				_targeting = AccessTools.Field(methodInfo.ReturnType, "isTargeting");
				if (_targeting?.FieldType != typeof(bool))
				{
					throw new InvalidOperationException("Peer targeting field unavailable");
				}
				_readState = AccessTools.MethodDelegate<Func<PeerInputSynchronizer, ulong, object>>(methodInfo, (object)null, true, (Type[])null);
				return true;
			}
			catch (Exception ex)
			{
				_readUnavailable = true;
				DevLog.Warn("Remote input API unavailable: " + ex.Message);
				return false;
			}
		}

		internal static bool Supports(CardModel card)
		{
			if ((!(card is Finisher) && !(card is Flechettes) && !(card is KnifeTrap) && !(card is DeadlyPoison) && !(card is PoisonedStab) && !(card is Snakebite) && !(card is Shiv) && !(card is LeadingStrike) && !(card is Pinpoint) && !(card is MementoMori) && !(card is Skewer) && !(card is Strangle) && !(card is Assassinate) && !(card is Suppress) && !(card is Outbreak) && !(card is BulletTime) && !(card is CorrosiveWave)) || 1 == 0)
			{
				return CreatesShivs(card);
			}
			return true;
		}

		private static bool CreatesShivs(CardModel card)
		{
			if ((!(card is BladeDance) && !(card is CloakAndDagger) && !(card is HiddenDaggers) && !(card is UpMySleeve) && !(card is BladeOfInk) && !(card is StormOfSteel)) || 1 == 0)
			{
				return GameCapabilities.IsBladeSymphony(card);
			}
			return true;
		}

		private static bool UsesPlayZone(CardModel card)
		{
			bool flag = CreatesShivs(card) || (card is Shiv && ShivFan.Active(card));
			if (!flag)
			{
				bool flag2 = ((card is Outbreak || card is BulletTime || card is CorrosiveWave) ? true : false);
				flag = flag2;
			}
			return flag;
		}

		internal static void Update()
		{
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Invalid comparison between Unknown and I4
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_036e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			AimSync.Update();
			if (!PlayerSettings.Enabled || !MultiplayerFx.Active || !FxKit.Alive((GodotObject)(object)GameRefs.CombatRoom))
			{
				Reset();
				return;
			}
			if ((object)_room != GameRefs.CombatRoom)
			{
				Reset();
				_room = (Node)(object)GameRefs.CombatRoom;
			}
			try
			{
				Seen.Clear();
				RunManager instance = RunManager.Instance;
				PeerInputSynchronizer val = ((instance != null) ? instance.InputSynchronizer : null);
				NGame instance2 = NGame.Instance;
				NRemoteMouseCursorContainer val2 = ((instance2 != null) ? instance2.RemoteCursorContainer : null);
				bool exact = AimSync.Exact;
				bool flag = !exact && val != null && Ready();
				foreach (Player player in CombatManager.Instance.DebugOnlyGetState().Players)
				{
					if (((player != null) ? player.Creature : null) == null || MultiplayerFx.IsLocal(player.Creature))
					{
						continue;
					}
					ulong netId = player.NetId;
					if (!player.Creature.IsAlive)
					{
						continue;
					}
					Seen.Add(netId);
					if (!Peers.TryGetValue(netId, out var value))
					{
						value = (Peers[netId] = new Peer());
					}
					CardModel card = null;
					Creature target = null;
					Vector2 val3 = Vector2.Zero;
					bool inPlayZone = false;
					if (exact)
					{
						if (!AimSync.TryReadHeld(netId, out card, out target, out inPlayZone))
						{
							if (card != null && value.Claimed == card)
							{
								value.Claimed = null;
							}
							Drop(value);
							continue;
						}
					}
					else
					{
						if (!flag)
						{
							Drop(value);
							continue;
						}
						object obj = _readState(val, netId);
						if (obj == null)
						{
							Drop(value);
							continue;
						}
						if (!(bool)_targeting.GetValue(obj))
						{
							value.Claimed = null;
							Drop(value);
							continue;
						}
						AbstractModel hoveredModel = instance.HoveredModelTracker.GetHoveredModel(netId);
						card = (CardModel)(object)((hoveredModel is CardModel) ? hoveredModel : null);
					}
					if (card != null && card.Owner == player && Supports(card))
					{
						CardPile pile = card.Pile;
						if (pile != null && (int)pile.Type == 2 && (exact || (!UsesPlayZone(card) && !(card is Shiv))))
						{
							if (value.Claimed != card)
							{
								value.Claimed = null;
							}
							if (value.Claimed == card)
							{
								Drop(value);
								continue;
							}
							if (!exact)
							{
								if (!FxKit.Alive((GodotObject)(object)val2))
								{
									Drop(value);
									continue;
								}
								try
								{
									val3 = val2.GetCursorPosition(netId);
								}
								catch (NullReferenceException)
								{
									Drop(value);
									continue;
								}
								catch (KeyNotFoundException)
								{
									Drop(value);
									continue;
								}
								target = Target(player.Creature, val3);
							}
							CardFxContext cardFxContext = Context(card, target, val3);
							if (cardFxContext == null)
							{
								Drop(value);
								continue;
							}
							if (exact)
							{
								val3 = (cardFxContext.AimPosition = (cardFxContext.HasTarget ? cardFxContext.TargetChest : cardFxContext.Chest));
							}
							if (value.Held != null && (value.Held.Finished || value.Held.C.Card != card))
							{
								Drop(value);
							}
							if (value.Held == null)
							{
								value.Held = Start(cardFxContext);
							}
							HeldSession held = value.Held;
							if (held != null)
							{
								held.C.AimPosition = val3;
								held.C.Feet = cardFxContext.Feet;
								held.C.Chest = cardFxContext.Chest;
								held.C.Body = cardFxContext.Body;
								held.C.Enemies = cardFxContext.Enemies;
								held.SetTarget(cardFxContext);
								held.Armed = (UsesPlayZone(card) ? inPlayZone : cardFxContext.HasTarget);
							}
							continue;
						}
					}
					Drop(value);
				}
				Gone.Clear();
				foreach (KeyValuePair<ulong, Peer> peer2 in Peers)
				{
					if (!Seen.Contains(peer2.Key))
					{
						Drop(peer2.Value);
						Gone.Add(peer2.Key);
					}
				}
				foreach (ulong item in Gone)
				{
					Peers.Remove(item);
				}
				for (int num = Owned.Count - 1; num >= 0; num--)
				{
					HeldSession heldSession = Owned[num];
					if (heldSession.Finished)
					{
						if (Playing.TryGetValue(heldSession.C.Card, out var value2) && value2 == heldSession)
						{
							Playing.Remove(heldSession.C.Card);
						}
						Owned.RemoveAt(num);
					}
				}
			}
			catch (Exception ex3)
			{
				Reset();
				DevLog.Warn("Remote standby stopped: " + ex3.Message);
			}
		}

		internal static bool TryObserve(Creature owner, out CardModel card, out Creature target)
		{
			bool released;
			return TryObserve(owner, out card, out target, out released);
		}

		internal static bool TryObserve(Creature owner, out CardModel card, out Creature target, out bool released)
		{
			bool inPlayZone;
			return TryObserve(owner, out card, out target, out released, out inPlayZone);
		}

		internal static bool TryObserve(Creature owner, out CardModel card, out Creature target, out bool released, out bool inPlayZone)
		{
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Invalid comparison between Unknown and I4
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Invalid comparison between Unknown and I4
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			card = null;
			target = null;
			released = false;
			inPlayZone = false;
			if (!MultiplayerFx.Active || !FxKit.Alive((GodotObject)(object)GameRefs.CombatRoom) || ((owner != null) ? owner.Player : null) == null || !owner.IsAlive || MultiplayerFx.IsLocal(owner))
			{
				return false;
			}
			try
			{
				Player player = owner.Player;
				CombatState val = CombatManager.Instance.DebugOnlyGetState();
				if ((object)owner.CombatState != val)
				{
					return false;
				}
				bool flag = false;
				foreach (Player player2 in val.Players)
				{
					if (player2 == player && player2.Creature == owner)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
				CardModel card2;
				Creature target2;
				if (AimSync.Exact)
				{
					if (!AimSync.TryReadHeld(player.NetId, out card2, out target2, out inPlayZone))
					{
						if (card2 != null && card2.Owner == player)
						{
							card = card2;
							released = true;
						}
						return false;
					}
					goto IL_01d4;
				}
				RunManager instance = RunManager.Instance;
				PeerInputSynchronizer val2 = ((instance != null) ? instance.InputSynchronizer : null);
				NGame instance2 = NGame.Instance;
				NRemoteMouseCursorContainer val3 = ((instance2 != null) ? instance2.RemoteCursorContainer : null);
				if (val2 == null || !Ready() || !FxKit.Alive((GodotObject)(object)val3))
				{
					return false;
				}
				object obj = _readState(val2, player.NetId);
				if (obj == null)
				{
					return false;
				}
				if (!(bool)_targeting.GetValue(obj))
				{
					released = true;
					return false;
				}
				HoveredModelTracker hoveredModelTracker = instance.HoveredModelTracker;
				AbstractModel obj2 = ((hoveredModelTracker != null) ? hoveredModelTracker.GetHoveredModel(player.NetId) : null);
				card2 = (CardModel)(object)((obj2 is CardModel) ? obj2 : null);
				if (card2 != null && card2.Owner == player)
				{
					CardPile pile = card2.Pile;
					if (pile != null && (int)pile.Type == 2)
					{
						target2 = Target(owner, val3.GetCursorPosition(player.NetId));
						inPlayZone = true;
						goto IL_01d4;
					}
				}
				return false;
				IL_01d4:
				if (card2 != null && card2.Owner == player)
				{
					CardPile pile2 = card2.Pile;
					if (pile2 != null && (int)pile2.Type == 2)
					{
						if (target2 != null)
						{
							bool flag2 = false;
							foreach (Creature enemy in owner.CombatState.Enemies)
							{
								if (enemy == target2 && enemy.IsAlive)
								{
									flag2 = true;
									break;
								}
							}
							if (!flag2)
							{
								return false;
							}
						}
						card = card2;
						target = target2;
						return true;
					}
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

		private static Creature Target(Creature owner, Vector2 cursor)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			foreach (Creature enemy in owner.CombatState.Enemies)
			{
				if (!enemy.IsAlive)
				{
					continue;
				}
				NCreature creatureNode = GameRefs.CombatRoom.GetCreatureNode(enemy);
				if (FxKit.Alive((GodotObject)(object)creatureNode) && FxKit.Alive((GodotObject)(object)creatureNode.Hitbox))
				{
					Rect2 globalRect = creatureNode.Hitbox.GetGlobalRect();
					if (globalRect.HasPoint(cursor))
					{
						return enemy;
					}
				}
			}
			return null;
		}

		private static CardFxContext Context(CardModel card, Creature target, Vector2 cursor)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			CardFxContext cardFxContext = CardFxManager.BuildContext(card.Owner.Creature, target);
			if (cardFxContext == null)
			{
				return null;
			}
			cardFxContext.Card = card;
			cardFxContext.Remote = true;
			cardFxContext.AimPosition = cursor;
			cardFxContext.UiParent = null;
			return cardFxContext;
		}

		private static HeldSession Start(CardFxContext c)
		{
			HeldSession heldSession = HeldFxManager.StartSession(HeldFxManager.For(((object)c.Card).GetType()), c);
			if (heldSession != null)
			{
				Owned.Add(heldSession);
			}
			return heldSession;
		}

		private static void Drop(Peer peer)
		{
			if (peer.Held != null && !peer.Held.Finished)
			{
				peer.Held.Leave();
			}
			peer.Held = null;
		}

		internal static HeldSession Claim(CardModel card, Creature target)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			if (!MultiplayerFx.Active || MultiplayerFx.IsLocal(card) || !Supports(card))
			{
				return null;
			}
			if ((object)_room != GameRefs.CombatRoom)
			{
				Reset();
				_room = (Node)(object)GameRefs.CombatRoom;
			}
			CardFxContext cardFxContext = Context(card, target, Vector2.Zero);
			if (cardFxContext == null)
			{
				return null;
			}
			if (!Peers.TryGetValue(card.Owner.NetId, out var value))
			{
				value = (Peers[card.Owner.NetId] = new Peer());
			}
			HeldSession heldSession = null;
			if (value.Held != null && !value.Held.Finished && value.Held.C.Card == card)
			{
				heldSession = value.Held;
				value.Held = null;
			}
			value.Claimed = card;
			bool flag = heldSession == null;
			if (flag)
			{
				bool flag2 = UsesPlayZone(card);
				if (!flag2)
				{
					bool flag3 = ((card is Shiv || card is LeadingStrike || card is Pinpoint || card is MementoMori || card is Skewer || card is Strangle || card is Assassinate || card is Suppress) ? true : false);
					flag2 = flag3;
				}
				flag = flag2;
			}
			if (flag)
			{
				foreach (HeldSession item in Owned)
				{
					if (!item.Finished && !item.Playing && item.C.Card == card)
					{
						item.End();
					}
				}
				return null;
			}
			flag = heldSession == null;
			if (flag)
			{
				bool flag2 = ((card is DeadlyPoison || card is PoisonedStab || card is Snakebite || card is Backstab) ? true : false);
				flag = flag2;
			}
			if (flag)
			{
				return null;
			}
			if (heldSession == null)
			{
				foreach (HeldSession item2 in Owned)
				{
					if (!item2.Finished && !item2.Playing && item2.C.Card == card)
					{
						item2.End();
					}
				}
			}
			if (heldSession == null)
			{
				heldSession = Start(cardFxContext);
			}
			if (heldSession == null)
			{
				return null;
			}
			if (Playing.TryGetValue(card, out var value2) && !value2.Finished && value2 != heldSession)
			{
				value2.OnCardFinished();
			}
			heldSession.SetTarget(cardFxContext);
			HeldFxManager.ReleaseManual(heldSession);
			Playing[card] = heldSession;
			return heldSession;
		}

		internal static HeldSession PlayingFor(CardModel card)
		{
			if (card == null || !Playing.TryGetValue(card, out var value) || value.Finished)
			{
				return null;
			}
			return value;
		}

		internal static void Finish(CardModel card)
		{
			if (card != null && Playing.Remove(card, out var value) && !value.Finished)
			{
				value.OnCardFinished();
			}
		}

		internal static void Reset()
		{
			CorrosiveWaveFx.ResetRemoteHandovers();
			foreach (HeldSession item in Owned)
			{
				item.End();
			}
			Owned.Clear();
			Peers.Clear();
			Playing.Clear();
			Seen.Clear();
			Gone.Clear();
			_room = null;
		}
	}
