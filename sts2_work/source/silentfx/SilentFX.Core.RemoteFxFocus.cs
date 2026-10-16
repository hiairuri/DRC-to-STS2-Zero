// namespace: SilentFX.Core
// type: RemoteFxFocus (class)
// source lines: 41701-42010
	internal static class RemoteFxFocus
	{
		private sealed class Lease
		{
			internal CardModel Card;

			internal bool Held;

			internal bool Out;

			internal bool Queued;

			internal bool Playing;

			internal object Invocation;

			internal object CurrentPlay;

			internal int Index = -1;

			internal int Count = 1;

			internal double Idle;
		}

		private static readonly List<Lease> Live = new List<Lease>();

		private static Lease _carried;

		private static object _room;

		private const double AbandonedSeconds = 300.0;

		internal static float Factor
		{
			get
			{
				try
				{
					if (!CheckEnvironment())
					{
						return 1f;
					}
					SampleCarried();
					foreach (Lease item in Live)
					{
						if (item.Out || item.Queued || item.Playing)
						{
							return 0.5f;
						}
					}
				}
				catch
				{
					Reset();
				}
				return 1f;
			}
		}

		internal static void Begin(CardModel card)
		{
			try
			{
				if (!CheckEnvironment() || !Trackable(card))
				{
					return;
				}
				if (_carried != null)
				{
					_carried.Held = false;
					_carried.Out = false;
					if (!_carried.Queued && !_carried.Playing)
					{
						Live.Remove(_carried);
					}
				}
				_carried = new Lease
				{
					Card = card,
					Held = true
				};
				Live.Add(_carried);
				SampleCarried();
			}
			catch
			{
				Reset();
			}
		}

		internal static void LetGo(CardModel card, bool played)
		{
			try
			{
				if (!CheckEnvironment() || !Trackable(card))
				{
					return;
				}
				Lease carried = _carried;
				if (carried != null && carried.Card == card)
				{
					_carried = null;
					carried.Held = false;
					carried.Out = false;
					carried.Idle = 0.0;
					if (played && !carried.Playing)
					{
						carried.Queued = true;
					}
					else if (!carried.Playing && !carried.Queued)
					{
						Live.Remove(carried);
					}
				}
			}
			catch
			{
				Reset();
			}
		}

		internal static void BeforeCardPlayed(CardPlay play)
		{
			try
			{
				if (!CheckEnvironment() || !Trackable((play != null) ? play.Card : null))
				{
					return;
				}
				object currentInvocation = MultiplayerFx.CurrentInvocation;
				int num = Convert.ToInt32(play.PlayIndex);
				int num2 = Math.Max(1, Convert.ToInt32(play.PlayCount));
				Lease lease = null;
				foreach (Lease item in Live)
				{
					if (item.Card == play.Card && item.Playing)
					{
						if (currentInvocation != null && item.Invocation == currentInvocation)
						{
							lease = item;
							break;
						}
						if (currentInvocation == null && item.Invocation == null && num == item.Index + 1 && item.Count == num2)
						{
							lease = item;
							break;
						}
					}
				}
				if (lease == null)
				{
					foreach (Lease item2 in Live)
					{
						if (item2.Card == play.Card && !item2.Playing && (item2.Queued || item2.Held))
						{
							lease = item2;
							break;
						}
					}
				}
				if (lease != null && (!lease.Playing || num >= lease.Index))
				{
					lease.Queued = false;
					lease.Held = false;
					lease.Out = false;
					lease.Playing = true;
					lease.Invocation = currentInvocation;
					lease.CurrentPlay = play;
					lease.Index = num;
					lease.Count = num2;
					lease.Idle = 0.0;
				}
			}
			catch
			{
				Reset();
			}
		}

		internal static void AfterCardPlayed(CardPlay play)
		{
			try
			{
				if (!CheckEnvironment() || !Trackable((play != null) ? play.Card : null))
				{
					return;
				}
				object currentInvocation = MultiplayerFx.CurrentInvocation;
				foreach (Lease item in Live)
				{
					if (!item.Playing || item.Card != play.Card || item.Invocation != currentInvocation || item.CurrentPlay != play)
					{
						continue;
					}
					item.Idle = 0.0;
					if (Convert.ToInt32(play.PlayIndex) >= Math.Max(1, Convert.ToInt32(play.PlayCount)) - 1)
					{
						Live.Remove(item);
						if (_carried == item)
						{
							_carried = null;
						}
					}
					break;
				}
			}
			catch
			{
				Reset();
			}
		}

		internal static void Update(double realDt)
		{
			try
			{
				if (!CheckEnvironment())
				{
					return;
				}
				SampleCarried();
				SceneTree tree = Ticker.Tree;
				if ((tree != null && tree.Paused) || Engine.TimeScale <= 0.001 || !double.IsFinite(realDt))
				{
					return;
				}
				double num = Math.Clamp(realDt, 0.0, 0.25);
				for (int num2 = Live.Count - 1; num2 >= 0; num2--)
				{
					Lease lease = Live[num2];
					if (!lease.Held || _carried != lease)
					{
						lease.Idle += num;
						if (!(lease.Idle < 300.0))
						{
							Live.RemoveAt(num2);
							if (_carried == lease)
							{
								_carried = null;
							}
						}
					}
				}
			}
			catch
			{
				Reset();
			}
		}

		private static void SampleCarried()
		{
			if (_carried == null || !_carried.Held)
			{
				return;
			}
			if (HeldFxManager.Carried != _carried.Card)
			{
				_carried.Held = false;
				_carried.Out = false;
				if (!_carried.Queued && !_carried.Playing)
				{
					Live.Remove(_carried);
				}
				_carried = null;
			}
			else
			{
				_carried.Out = HeldFxManager.CarriedOut;
			}
		}

		private static bool CheckEnvironment()
		{
			if (!MultiplayerFx.Active || !PlayerSettings.Enabled)
			{
				Reset();
				return false;
			}
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			if (!FxKit.Alive((GodotObject)(object)combatRoom))
			{
				Reset();
				return false;
			}
			if (_room != combatRoom)
			{
				Reset();
				_room = combatRoom;
			}
			return true;
		}

		private static bool Trackable(CardModel card)
		{
			if (card != null)
			{
				return MultiplayerFx.IsLocal(card);
			}
			return false;
		}

		internal static void Reset()
		{
			Live.Clear();
			_carried = null;
			_room = null;
		}
	}
