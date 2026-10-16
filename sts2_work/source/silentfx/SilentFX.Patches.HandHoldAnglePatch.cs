// namespace: SilentFX.Patches
// type: HandHoldAnglePatch (class)
// source lines: 21060-21080
	public static class HandHoldAnglePatch
	{
		public static bool Prefix(ref int handSize, ref float __result)
		{
			try
			{
				HandHold.RaiseSize(ref handSize);
				if (HandHold.Lined)
				{
					__result = 0f;
					return false;
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("HandHoldAnglePatch: " + ex);
			}
			return true;
		}
	}
	[HarmonyPatch(typeof(HandPosHelper), "GetScale", new Type[] { typeof(int) })]
