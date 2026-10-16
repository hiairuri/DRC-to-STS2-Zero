// namespace: SilentFX.Core
// type: AimSync (class)
// source lines: 31386-31975
	internal static class AimSync
	{
		private sealed class Peer
		{
			internal ulong Round;

			internal ulong Generation;

			internal ulong Sequence;
		}

		private static readonly Dictionary<ulong, Peer> Ready = new Dictionary<ulong, Peer>();

		private static readonly ConditionalWeakTable<INetGameService, object> Attached = new ConditionalWeakTable<INetGameService, object>();

		private static readonly Dictionary<ulong, AimMessage> Targets = new Dictionary<ulong, AimMessage>();

		private static readonly Dictionary<ulong, double> SeenAt = new Dictionary<ulong, double>();

		private static INetGameService _service;

		private static object _room;

		private static ulong[] _players = Array.Empty<ulong>();

		private static ulong _seed;

		private static ulong _roster;

		private static ulong _epoch;

		private static ulong _round;

		private static ulong _generation;

		private static ulong _sequence;

		private static ulong _relaySequence;

		private static int _roomId;

		private static int _act;

		private static double _lease;

		private static double _nextOffer;

		private static double _nextSample;

		private static double _refresh;

		private static AimMessage _sent;

		private static bool _hasSent;

		private static double Now => (double)Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;

		internal static bool Exact
		{
			get
			{
				if (AimCompatibility.ExactAvailable && MultiplayerFx.Active && _service != null && _epoch != 0L)
				{
					return Now < _lease;
				}
				return false;
			}
		}

		internal static bool Attach(INetGameService service)
		{
			if (!AimCompatibility.ExactAvailable || service == null)
			{
				return false;
			}
			if (Attached.TryGetValue(service, out var _))
			{
				return true;
			}
			try
			{
				service.RegisterMessageHandler<AimMessage>((MessageHandlerDelegate<AimMessage>)((AimMessage message, ulong sender) =>
				{
					if (_service == service)
					{
						Receive(message, sender);
					}
				}));
				Attached.Add(service, new object());
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("[MP] Target receiver unavailable: " + ex.Message);
				return false;
			}
		}

		internal static void Update()
		{
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Expected Obj, but got Unknown
			try
			{
				if (!AimCompatibility.ExactAvailable)
				{
					Reset();
					return;
				}
				RunManager instance = RunManager.Instance;
				INetGameService val = ((instance != null) ? instance.NetService : null);
				bool flag = ((val is NetHostGameService || val is NetClientGameService) ? true : false);
				if (flag && !Attach(RunManager.Instance.NetService))
				{
					Reset();
					return;
				}
				if (!MultiplayerFx.Active || !FxKit.Alive((GodotObject)(object)GameRefs.CombatRoom))
				{
					Reset();
					return;
				}
				RunManager instance2 = RunManager.Instance;
				INetGameService val2 = ((instance2 != null) ? instance2.NetService : null);
				RunState val3 = ((instance2 != null) ? instance2.DebugOnlyGetState() : null);
				int? num;
				if (val3 == null)
				{
					num = null;
				}
				else
				{
					AbstractRoom currentRoom = val3.CurrentRoom;
					num = ((currentRoom != null) ? currentRoom.Id : ((int?)null));
				}
				int? num2 = num;
				if (val2 == null || !val2.IsConnected || !num2.HasValue || (!(val2 is NetHostGameService) && !(val2 is NetClientGameService)))
				{
					Reset();
					return;
				}
				if (!AimCompatibility.TryGetSeed(val3.Rng, out var seed))
				{
					Reset();
					return;
				}
				ulong[] array = (from p in CombatManager.Instance.DebugOnlyGetState().Players
					select p.NetId into id
					orderby id
					select id).ToArray();
				if (array.Length < 2 || !Enumerable.Contains(array, val2.NetId))
				{
					Reset();
					return;
				}
				if (_service != val2 || _room != GameRefs.CombatRoom || !Enumerable.SequenceEqual(_players, array) || _seed != seed || _roomId != num2.Value || _act != val3.CurrentActIndex)
				{
					Reset(clearLocal: false);
					_service = val2;
					_room = GameRefs.CombatRoom;
					_players = array;
					_seed = seed;
					_roomId = num2.Value;
					_act = val3.CurrentActIndex;
					_roster = BitConverter.ToUInt64(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join(",", array))), 0);
					_generation = Nonce();
				}
				INetGameService service = _service;
				NetHostGameService val4 = (NetHostGameService)(object)((service is NetHostGameService) ? service : null);
				if (val4 != null)
				{
					if (!RoomMatches(val4))
					{
						ClearProtocol();
						return;
					}
					if (_epoch == 0L)
					{
						_epoch = Nonce();
					}
					if (Now >= _nextOffer)
					{
						_nextOffer = Now + 1.0;
						_round++;
						SendPeers(Message(1));
					}
				}
				else if (!AimCompatibility.HostMatches((NetClientGameService)_service))
				{
					ClearProtocol();
					return;
				}
				if (!Exact)
				{
					_hasSent = false;
				}
				else
				{
					Sample();
				}
			}
			catch (Exception ex)
			{
				Reset();
				DevLog.Warn("[MP] Exact targeting fell back to peer input: " + ex.Message);
			}
		}

		private static bool RoomMatches(NetHostGameService host)
		{
			AimCompatibility.PeerInfo[] peers;
			return RoomMatches(host, out peers);
		}

		private static bool RoomMatches(NetHostGameService host, out AimCompatibility.PeerInfo[] peers)
		{
			peers = Array.Empty<AimCompatibility.PeerInfo>();
			if (!MultiplayerFx.Active || !host.IsConnected || !AimCompatibility.TryGetPeers(host, out peers) || peers.Length != _players.Length - 1)
			{
				return false;
			}
			HashSet<ulong> hashSet = new HashSet<ulong>();
			AimCompatibility.PeerInfo[] array = peers;
			for (int i = 0; i < array.Length; i++)
			{
				AimCompatibility.PeerInfo peerInfo = array[i];
				if (peerInfo.NetId == host.NetId || !Enumerable.Contains(_players, peerInfo.NetId) || !hashSet.Add(peerInfo.NetId) || !peerInfo.ReadyForBroadcasting || !AimCompatibility.PeerMatches(host, peerInfo.Data))
				{
					return false;
				}
			}
			return true;
		}

		private static AimMessage Message(byte kind)
		{
			return new AimMessage
			{
				Kind = kind,
				Seed = _seed,
				Room = _roomId,
				Act = _act,
				Roster = _roster,
				Epoch = _epoch,
				Round = _round,
				Generation = _generation,
				Source = _service.NetId
			};
		}

		private static bool Current(AimMessage message)
		{
			RunManager instance = RunManager.Instance;
			RunState val = ((instance != null) ? instance.DebugOnlyGetState() : null);
			if (AimCompatibility.ExactAvailable && MultiplayerFx.Active && _service != null && _service.IsConnected && _service == ((instance != null) ? instance.NetService : null) && _room == GameRefs.CombatRoom && FxKit.Alive((GodotObject)(object)GameRefs.CombatRoom))
			{
				int? num;
				if (val == null)
				{
					num = null;
				}
				else
				{
					AbstractRoom currentRoom = val.CurrentRoom;
					num = ((currentRoom != null) ? currentRoom.Id : ((int?)null));
				}
				if (num == _roomId && AimCompatibility.TryGetSeed(val.Rng, out var seed) && seed == _seed && val.CurrentActIndex == _act && _players.SequenceEqual(from p in CombatManager.Instance.DebugOnlyGetState().Players
					select p.NetId into id
					orderby id
					select id) && message.Seed == _seed && message.Room == _roomId && message.Act == _act)
				{
					return message.Roster == _roster;
				}
			}
			return false;
		}

		private static void Receive(AimMessage message, ulong sender)
		{
			try
			{
				if (!Current(message) || message.Epoch == 0L)
				{
					return;
				}
				INetGameService service = _service;
				NetHostGameService host = (NetHostGameService)(object)((service is NetHostGameService) ? service : null);
				if (host != null)
				{
					if (!RoomMatches(host) || sender == host.NetId || !Enumerable.Contains(_players, sender) || message.Source != sender || message.Epoch != _epoch)
					{
						return;
					}
					Peer value2;
					if (message.Kind == 2 && message.Round == _round && message.Generation != 0L)
					{
						if (!Ready.TryGetValue(sender, out var value) || value.Generation != message.Generation)
						{
							Dictionary<ulong, Peer> ready = Ready;
							Peer peer = new Peer
							{
								Generation = message.Generation
							};
							value = peer;
							ready[sender] = peer;
							Targets.Remove(sender);
							SeenAt.Remove(sender);
						}
						value.Round = _round;
						if (!_players.Where((ulong id) => id != host.NetId).All((ulong id) => Ready.TryGetValue(id, out var value4) && value4.Round == _round))
						{
							return;
						}
						_lease = Now + 3.0;
						ulong[] players = _players;
						foreach (ulong num2 in players)
						{
							if (num2 != host.NetId)
							{
								AimMessage aimMessage = Message(3);
								aimMessage.Generation = Ready[num2].Generation;
								host.SendMessage<AimMessage>(aimMessage, num2);
							}
						}
						_hasSent = false;
					}
					else if (message.Kind == 4 && Exact && Ready.TryGetValue(sender, out value2) && value2.Generation == message.Generation && message.Sequence > value2.Sequence && ValidSnapshot(message))
					{
						value2.Sequence = message.Sequence;
						message.Sequence = ++_relaySequence;
						Targets[sender] = message;
						SeenAt[sender] = Now;
						SendPeers(message, sender);
					}
					return;
				}
				INetGameService service2 = _service;
				NetClientGameService val = (NetClientGameService)(object)((service2 is NetClientGameService) ? service2 : null);
				if (val == null || sender != val.HostNetId || !AimCompatibility.HostMatches(val))
				{
					return;
				}
				AimMessage value3;
				if (message.Kind == 1)
				{
					if (message.Source == sender && message.Round != 0L)
					{
						if (_epoch != message.Epoch)
						{
							ClearProtocol();
							_epoch = message.Epoch;
						}
						if (message.Round >= _round)
						{
							_round = message.Round;
							val.SendMessage<AimMessage>(Message(2));
						}
					}
				}
				else if (message.Kind == 3 && message.Source == sender && message.Epoch == _epoch && message.Round == _round && message.Generation == _generation)
				{
					_lease = Now + 3.0;
					_hasSent = false;
				}
				else if (message.Kind == 4 && Exact && message.Epoch == _epoch && message.Source != val.NetId && Enumerable.Contains(_players, message.Source) && (!Targets.TryGetValue(message.Source, out value3) || message.Sequence > value3.Sequence) && ValidSnapshot(message))
				{
					Targets[message.Source] = message;
					SeenAt[message.Source] = Now;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("[MP] Ignored targeting packet: " + ex.Message);
			}
		}

		private static bool ValidSnapshot(AimMessage message)
		{
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Invalid comparison between Unknown and I4
			if (!message.Active && message.InPlayZone)
			{
				return false;
			}
			if (!message.Active && !message.HasCard)
			{
				return !message.Target.HasValue;
			}
			if (!message.HasCard)
			{
				return false;
			}
			CardModel val = message.Card.ToCardModelOrNull();
			if (!message.Active)
			{
				if (!message.Target.HasValue)
				{
					ulong? num;
					if (val == null)
					{
						num = null;
					}
					else
					{
						Player owner = val.Owner;
						num = ((owner != null) ? new ulong?(owner.NetId) : ((ulong?)null));
					}
					return num == message.Source;
				}
				return false;
			}
			if (((val != null) ? val.Owner : null) != null && val.Owner.NetId == message.Source)
			{
				CardPile pile = val.Pile;
				if (pile != null && (int)pile.Type == 2)
				{
					return val.Owner.Creature.IsAlive;
				}
			}
			return false;
		}

		private static void Sample()
		{
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Invalid comparison between Unknown and I4
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			double now = Now;
			if (now < _nextSample)
			{
				return;
			}
			_nextSample = now + 0.05;
			AimLocalTarget.Snapshot snapshot = AimLocalTarget.Read();
			AimMessage aimMessage = Message(4);
			if (snapshot.IsHeld)
			{
				CardModel card = snapshot.Card;
				if (card != null)
				{
					CardPile pile = card.Pile;
					if ((int)((pile != null) ? new PileType?(pile.Type) : ((PileType?)null)).GetValueOrDefault() == 2 && MultiplayerFx.IsLocal(snapshot.Card))
					{
						aimMessage.Active = true;
						aimMessage.InPlayZone = snapshot.InPlayZone;
						aimMessage.HasCard = true;
						aimMessage.Card = NetCombatCard.FromModel(snapshot.Card);
						Creature target = snapshot.Target;
						aimMessage.Target = ((target != null) ? target.CombatId : ((uint?)null));
						goto IL_00fc;
					}
				}
			}
			if (_sent.HasCard)
			{
				aimMessage.InPlayZone = false;
				aimMessage.HasCard = true;
				aimMessage.Card = _sent.Card;
			}
			goto IL_00fc;
			IL_00fc:
			if (!_hasSent || !(now < _refresh) || aimMessage.Active != _sent.Active || aimMessage.InPlayZone != _sent.InPlayZone || aimMessage.HasCard != _sent.HasCard || (aimMessage.Active && aimMessage.Card.CombatCardIndex != _sent.Card.CombatCardIndex) || aimMessage.Target != _sent.Target)
			{
				aimMessage.Sequence = ++_sequence;
				if (_service is NetHostGameService)
				{
					aimMessage.Sequence = ++_relaySequence;
					SendPeers(aimMessage);
				}
				else
				{
					_service.SendMessage<AimMessage>(aimMessage);
				}
				_sent = aimMessage;
				_hasSent = true;
				_refresh = now + 1.0;
			}
		}

		private static void SendPeers(AimMessage message, ulong? except = null)
		{
			INetGameService service = _service;
			NetHostGameService val = (NetHostGameService)(object)((service is NetHostGameService) ? service : null);
			if (val == null || !RoomMatches(val, out var peers))
			{
				return;
			}
			AimCompatibility.PeerInfo[] array = peers;
			for (int i = 0; i < array.Length; i++)
			{
				AimCompatibility.PeerInfo peerInfo = array[i];
				if (peerInfo.NetId != except)
				{
					val.SendMessage<AimMessage>(message, peerInfo.NetId);
				}
			}
		}

		internal static bool TryReadHeld(ulong player, out CardModel card, out Creature target)
		{
			bool inPlayZone;
			return TryReadHeld(player, out card, out target, out inPlayZone);
		}

		internal static bool TryReadHeld(ulong player, out CardModel card, out Creature target, out bool inPlayZone)
		{
			card = null;
			target = null;
			inPlayZone = false;
			if (!Exact || !Targets.TryGetValue(player, out var value) || !SeenAt.TryGetValue(player, out var value2) || Now - value2 >= 3.0 || value.Source != player || !Current(value) || !ValidSnapshot(value))
			{
				return false;
			}
			card = (value.HasCard ? value.Card.ToCardModelOrNull() : null);
			if (!value.Active)
			{
				return false;
			}
			inPlayZone = value.InPlayZone;
			if (value.Target.HasValue)
			{
				foreach (Creature enemy in card.Owner.Creature.CombatState.Enemies)
				{
					if (enemy.IsAlive && enemy.CombatId == value.Target)
					{
						target = enemy;
						break;
					}
				}
			}
			return true;
		}

		internal static bool Resolve(ulong player, CardModel card, out Creature target)
		{
			target = null;
			if (!TryReadHeld(player, out var card2, out var target2) || card2 != card)
			{
				return false;
			}
			target = target2;
			return true;
		}

		private static ulong Nonce()
		{
			ulong num;
			do
			{
				num = BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(8), 0);
			}
			while (num == 0L);
			return num;
		}

		private static void ClearProtocol()
		{
			if (_service == null)
			{
				_generation = 0uL;
			}
			else if (_epoch != 0L || _generation == 0L)
			{
				_generation = Nonce();
			}
			Ready.Clear();
			Targets.Clear();
			SeenAt.Clear();
			_epoch = (_round = (_sequence = (_relaySequence = 0uL)));
			_lease = (_nextOffer = (_nextSample = 0.0));
			_hasSent = false;
			_sent = default;
		}

		internal static void Reset(bool clearLocal = true)
		{
			_service = null;
			_room = null;
			_players = Array.Empty<ulong>();
			ClearProtocol();
			if (clearLocal)
			{
				AimLocalTarget.Reset();
			}
		}
	}
