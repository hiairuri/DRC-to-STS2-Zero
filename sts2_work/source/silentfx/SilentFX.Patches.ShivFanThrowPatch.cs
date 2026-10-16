// namespace: SilentFX.Patches
// type: ShivFanThrowPatch (class)
// source lines: 21425-21443
	public static class ShivFanThrowPatch
	{
		public static void Prefix(Creature owner, ref Creature target)
		{
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			try
			{
				ShivFan.RedirectThrow(owner, ref target);
			}
			catch (Exception ex)
			{
				DevLog.Error("ShivFanThrowPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NCreature), "SetAnimationTrigger")]
