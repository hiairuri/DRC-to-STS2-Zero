// namespace: SilentFX.Patches
// type: StrangleSeen (class)
// source lines: 22945-22966
	public static class StrangleSeen
	{
		private static readonly ConditionalWeakTable<StranglePower, HashSet<CardModel>> Notes = new ConditionalWeakTable<StranglePower, HashSet<CardModel>>();

		public static void Note(StranglePower power, CardModel card)
		{
			if (power != null && card != null)
			{
				Notes.GetValue(power, (StranglePower _) => new HashSet<CardModel>(ReferenceEqualityComparer.Instance)).Add(card);
			}
		}

		public static bool Take(StranglePower power, CardModel card)
		{
			if (power != null && card != null && Notes.TryGetValue(power, out var value))
			{
				return value.Remove(card);
			}
			return false;
		}
	}
	[HarmonyPatch(typeof(StranglePower), "BeforeCardPlayed")]
