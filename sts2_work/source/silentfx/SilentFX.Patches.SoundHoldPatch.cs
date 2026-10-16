// namespace: SilentFX.Patches
// type: SoundHoldPatch (class)
// source lines: 20349-20367
	public static class SoundHoldPatch
	{
		public static bool Prefix(string path, Dictionary<string, float> parameters, float volume)
		{
			if (!PlayerSettings.Enabled)
			{
				return true;
			}
			try
			{
				return !TimeFreeze.HoldSound(path, parameters, volume);
			}
			catch (Exception ex)
			{
				DevLog.Error("SoundHoldPatch: " + ex);
				return true;
			}
		}
	}
