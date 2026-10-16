// namespace: SilentFX.Patches
// type: CreatureHoldBulletSlowPatch (class)
// source lines: 22394-22408
	internal static class CreatureHoldBulletSlowPatch
	{
		private static void Postfix(List<NCreature> ___Slowed)
		{
			try
			{
				CreatureHold.AfterBulletSlow(___Slowed);
			}
			catch (Exception ex)
			{
				DevLog.Warn("CreatureHoldBulletSlowPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(TimeFreeze), "RestoreSpeed")]
