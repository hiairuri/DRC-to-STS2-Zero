// namespace: SilentFX.Patches
// type: BlurTriggerPatch (class)
// source lines: 22928-22944
	public static class BlurTriggerPatch
	{
		public static void Prefix(BlurPower __instance, AbstractModel preventer)
		{
			try
			{
				if ((object)preventer == __instance)
				{
					PowerFxManager.Trigger((PowerModel)(object)__instance);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("BlurTriggerPatch: " + ex);
			}
		}
	}
