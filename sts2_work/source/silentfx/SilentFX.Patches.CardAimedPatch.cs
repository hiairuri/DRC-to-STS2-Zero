// namespace: SilentFX.Patches
// type: CardAimedPatch (class)
// source lines: 21306-21320
	public static class CardAimedPatch
	{
		public static void Postfix(NCreature creature)
		{
			try
			{
				HeldFxManager.SetTarget(FxKit.Alive((GodotObject)(object)creature) ? creature.Entity : null);
			}
			catch (Exception ex)
			{
				DevLog.Error("CardAimedPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NCardPlay), "OnCreatureUnhover")]
