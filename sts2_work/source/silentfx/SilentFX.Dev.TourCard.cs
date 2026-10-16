// namespace: SilentFX.Dev
// type: TourCard (class)
// source lines: 28718-28745
	internal sealed class TourCard
	{
		internal readonly CardModel Canonical;

		internal readonly bool Upgraded;

		internal string Title => Canonical.Title + (Upgraded ? "+" : "");

		internal (Type, int) Key => (((object)Canonical).GetType(), Upgraded ? 1 : 0);

		internal TourCard(CardModel canonical, bool upgraded = false)
		{
			Canonical = canonical;
			Upgraded = upgraded;
		}

		internal bool Matches(CardModel card)
		{
			Type type = ((object)card).GetType();
			int currentUpgradeLevel = card.CurrentUpgradeLevel;
			(Type, int) key = Key;
			if (type == key.Item1)
			{
				return currentUpgradeLevel == key.Item2;
			}
			return false;
		}
	}
