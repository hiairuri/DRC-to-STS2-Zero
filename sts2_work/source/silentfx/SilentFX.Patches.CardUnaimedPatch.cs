// namespace: SilentFX.Patches
// type: CardUnaimedPatch (class)
// source lines: 21321-21335
	public static class CardUnaimedPatch
	{
		public static void Postfix()
		{
			try
			{
				HeldFxManager.SetTarget(null);
			}
			catch (Exception ex)
			{
				DevLog.Error("CardUnaimedPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NCardPlay), "Cleanup")]
