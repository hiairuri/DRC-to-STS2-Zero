// namespace: SilentFX.Patches
// type: CreatureHoldBulletRestorePatch (class)
// source lines: 22409-22423
	internal static class CreatureHoldBulletRestorePatch
	{
		private static void Postfix()
		{
			try
			{
				CreatureHold.AfterBulletRestore();
			}
			catch (Exception ex)
			{
				DevLog.Warn("CreatureHoldBulletRestorePatch: " + ex);
			}
		}
	}
	[HarmonyPatch]
