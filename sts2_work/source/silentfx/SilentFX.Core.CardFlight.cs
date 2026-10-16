// namespace: SilentFX.Core
// type: CardFlight (class)
// source lines: 38590-38699
	public static class CardFlight
	{
		public sealed class Plan
		{
			public bool HideCard;

			public bool HideTrail;

			public bool Straight;

			public float Duration;

			public Action<NCard, Vector2, Vector2> Started;

			public Func<bool> Hold;

			public float HoldMax = 0.5f;

			public Action<NCard> Released;
		}

		private static readonly Dictionary<CardModel, (Plan Plan, double Until)> Claims = new Dictionary<CardModel, (Plan, double)>(ReferenceEqualityComparer.Instance);

		public static void Claim(CardModel card, Plan plan, float seconds = 3f)
		{
			if (card == null)
			{
				return;
			}
			double num = FxClock.Milliseconds;
			List<CardModel> list = new List<CardModel>();
			foreach (KeyValuePair<CardModel, (Plan, double)> claim in Claims)
			{
				if (num >= claim.Value.Item2)
				{
					list.Add(claim.Key);
				}
			}
			foreach (CardModel item in list)
			{
				Claims.Remove(item);
			}
			if (plan == null || !(seconds > 0f))
			{
				Claims.Remove(card);
			}
			else
			{
				Claims[card] = (plan, num + (double)seconds * 1000.0);
			}
		}

		public static bool Claimed(CardModel card)
		{
			if (card != null && Claims.TryGetValue(card, out (Plan, double) value))
			{
				return (double)FxClock.Milliseconds < value.Item2;
			}
			return false;
		}

		public static void Release(CardModel card)
		{
			if (card != null)
			{
				Claims.Remove(card);
			}
		}

		public static bool Holds(CardModel card, Plan plan)
		{
			if (card != null && plan != null && Claims.TryGetValue(card, out (Plan, double) value) && value.Item1 == plan)
			{
				return (double)FxClock.Milliseconds < value.Item2;
			}
			return false;
		}

		public static void Release(CardModel card, Plan plan)
		{
			if (card != null && plan != null && Claims.TryGetValue(card, out (Plan, double) value) && value.Item1 == plan)
			{
				Claims.Remove(card);
			}
		}

		public static void Reset()
		{
			Claims.Clear();
		}

		internal static bool TryTake(NCard node, out Plan plan)
		{
			plan = null;
			if (Claims.Count == 0)
			{
				return false;
			}
			if (!FxKit.Alive((GodotObject)(object)node) || node.Model == null)
			{
				return false;
			}
			if (!Claims.Remove(node.Model, out (Plan, double) value) || (double)FxClock.Milliseconds >= value.Item2)
			{
				return false;
			}
			(plan, _) = value;
			return true;
		}
	}
