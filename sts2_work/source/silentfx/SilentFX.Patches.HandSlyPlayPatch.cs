// namespace: SilentFX.Patches
// type: HandSlyPlayPatch (class)
// source lines: 20956-20980
	public static class HandSlyPlayPatch
	{
		public static void Prefix(CardModel card, AutoPlayType type)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			try
			{
				if ((int)type == 2)
				{
					HandFlow.RaiseSlyPlayBegins(card);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("HandSlyPlayPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "BeforeSideTurnStart", new Type[]
	{
		typeof(ICombatState),
		typeof(CombatSide),
		typeof(IReadOnlyList<Creature>)
	})]
