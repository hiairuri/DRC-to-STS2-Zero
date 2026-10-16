// namespace: SilentFX.Patches
// type: HandHoldScalePatch (class)
// source lines: 21081-21095
	public static class HandHoldScalePatch
	{
		public static void Prefix(ref int handSize)
		{
			try
			{
				HandHold.RaiseSize(ref handSize);
			}
			catch (Exception ex)
			{
				DevLog.Error("HandHoldScalePatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NCard), "OnReturnedFromPool", new Type[] { })]
