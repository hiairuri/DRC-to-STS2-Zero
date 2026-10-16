// namespace: SilentFX.Patches
// type: FullScreenVfxHoldPatch (class)
// source lines: 20324-20348
	public static class FullScreenVfxHoldPatch
	{
		public static bool Prefix(string path, Creature spawner)
		{
			if (!PlayerSettings.Enabled)
			{
				return true;
			}
			try
			{
				return !TimeFreeze.SkipFullScreenVfx(path, spawner);
			}
			catch (Exception ex)
			{
				DevLog.Error("FullScreenVfxHoldPatch: " + ex);
				return true;
			}
		}
	}
	[HarmonyPatch(typeof(NAudioManager), "PlayOneShot", new Type[]
	{
		typeof(string),
		typeof(Dictionary<string, float>),
		typeof(float)
	})]
