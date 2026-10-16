// namespace: SilentFX.Core
// type: MultiplayerFx (class)
// source lines: 40487-40597
	internal static class MultiplayerFx
	{
		internal sealed class OwnerTimes
		{
			internal ulong RollLandsAt;

			internal ulong BladesArriveAt;
		}

		private static readonly AsyncLocal<CardModel> Source = new AsyncLocal<CardModel>();

		private static readonly AsyncLocal<object> Invocation = new AsyncLocal<object>();

		private static readonly ConditionalWeakTable<Creature, OwnerTimes> Times = new ConditionalWeakTable<Creature, OwnerTimes>();

		internal static bool Active
		{
			get
			{
				if (Tuning.F("mp.remote_fx", 1f) == 0f)
				{
					return false;
				}
				try
				{
					CombatManager instance = CombatManager.Instance;
					int result;
					if (instance == null)
					{
						result = 0;
					}
					else
					{
						CombatState val = instance.DebugOnlyGetState();
						result = ((((val != null) ? new int?(val.Players.Count) : ((int?)null)) > 1) ? 1 : 0);
					}
					return (byte)result != 0;
				}
				catch
				{
					return false;
				}
			}
		}

		internal static CardModel CurrentCard => Source.Value;

		internal static object CurrentInvocation => Invocation.Value;

		internal static bool IsLocal(CardModel card)
		{
			try
			{
				return ((card != null) ? card.Owner : null) != null && LocalContext.IsMine(card);
			}
			catch
			{
				return false;
			}
		}

		internal static bool IsLocal(Creature owner)
		{
			try
			{
				return ((owner != null) ? owner.Player : null) != null && LocalContext.IsMe(owner.Player);
			}
			catch
			{
				return false;
			}
		}

		internal static CardModel Enter(CardModel card)
		{
			CardModel value = Source.Value;
			Source.Value = card;
			return value;
		}

		internal static void Leave(CardModel previous)
		{
			Source.Value = previous;
		}

		internal static object EnterInvocation()
		{
			object value = Invocation.Value;
			Invocation.Value = new object();
			return value;
		}

		internal static void LeaveInvocation(object previous)
		{
			Invocation.Value = previous;
		}

		internal static OwnerTimes For(Creature owner)
		{
			if (owner != null)
			{
				return Times.GetValue(owner, (Creature _) => new OwnerTimes());
			}
			return null;
		}

		internal static void Reset()
		{
			Times.Clear();
		}
	}
