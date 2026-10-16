// namespace: SilentFX.Patches
// type: AfterimageRemoteSeen (class)
// source lines: 22802-22836
	internal static class AfterimageRemoteSeen
	{
		private static readonly ConditionalWeakTable<AfterimagePower, Dictionary<CardModel, int>> Notes = new ConditionalWeakTable<AfterimagePower, Dictionary<CardModel, int>>();

		internal static void Note(AfterimagePower power, CardModel card)
		{
			if (power != null && card != null)
			{
				Notes.GetValue(power, (AfterimagePower _) => new Dictionary<CardModel, int>(ReferenceEqualityComparer.Instance)).TryAdd(card, ((PowerModel)power).Amount);
			}
		}

		internal static bool Take(AfterimagePower power, CardModel card)
		{
			if (power != null && card != null && Notes.TryGetValue(power, out var value) && value.Remove(card, out var value2))
			{
				return value2 > 0;
			}
			return false;
		}

		internal static void Forget(AfterimagePower power)
		{
			if (power != null)
			{
				Notes.Remove(power);
			}
		}

		internal static void Reset()
		{
			Notes.Clear();
		}
	}
	[HarmonyPatch(typeof(AfterimagePower), "BeforeCardPlayed")]
