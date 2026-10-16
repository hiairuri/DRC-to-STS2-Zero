// namespace: SilentFX.Patches
// type: HuntRoomExitPatch (class)
// source lines: 22546-22559
	internal static class HuntRoomExitPatch
	{
		private static void Prefix()
		{
			try
			{
				TheHuntFx.ResetTrophy();
			}
			catch (Exception ex)
			{
				DevLog.Warn("HuntRoomExitPatch: " + ex);
			}
		}
	}
