// namespace: SilentFX.Patches
// type: StatusStepsRoomExitPatch (class)
// source lines: 23328-23349
	internal static class StatusStepsRoomExitPatch
	{
		private static void Prefix()
		{
			try
			{
				StatusSteps.Reset();
			}
			catch (Exception ex)
			{
				DevLog.Warn("StatusStepsRoomExitPatch: " + ex);
			}
			try
			{
				MalaiseFx.Reset();
			}
			catch (Exception ex2)
			{
				DevLog.Warn("StatusStepsRoomExitPatch (Malaise): " + ex2);
			}
		}
	}
