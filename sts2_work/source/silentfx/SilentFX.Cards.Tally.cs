// namespace: SilentFX.Cards
// type: Tally (class)
// source lines: 46274-46357
	internal static class Tally
	{
		private sealed class State
		{
			public int Hits;

			public Vector2 Last;

			public ulong At;
		}

		private static readonly Dictionary<CardModel, State> Map = new Dictionary<CardModel, State>(ReferenceEqualityComparer.Instance);

		private static readonly List<CardModel> Stale = new List<CardModel>();

		public static void Begin(CardModel card, Vector2 origin)
		{
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			if (card == null)
			{
				return;
			}
			ulong milliseconds = FxClock.Milliseconds;
			Stale.Clear();
			foreach (KeyValuePair<CardModel, State> item in Map)
			{
				if (milliseconds - item.Value.At > 8000)
				{
					Stale.Add(item.Key);
				}
			}
			foreach (CardModel item2 in Stale)
			{
				Map.Remove(item2);
			}
			Map[card] = new State
			{
				Hits = 0,
				Last = origin,
				At = milliseconds
			};
		}

		public static int Hit(CardModel card, Vector2 at, Vector2 origin, out Vector2 previous)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			previous = origin;
			if (card == null)
			{
				return 0;
			}
			ulong milliseconds = FxClock.Milliseconds;
			if (!Map.TryGetValue(card, out var value) || milliseconds - value.At > 8000)
			{
				value = new State
				{
					Hits = 0,
					Last = origin,
					At = milliseconds
				};
				Map[card] = value;
			}
			previous = value.Last;
			int hits = value.Hits;
			value.Hits++;
			value.Last = at;
			value.At = milliseconds;
			return hits;
		}

		public static void Reset()
		{
			Map.Clear();
			Stale.Clear();
		}
	}
