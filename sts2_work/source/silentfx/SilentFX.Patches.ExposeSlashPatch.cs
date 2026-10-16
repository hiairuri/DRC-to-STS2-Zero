// namespace: SilentFX.Patches
// type: ExposeSlashPatch (class)
// source lines: 19965-19983
	public static class ExposeSlashPatch
	{
		public static void Prefix(Creature target, string path)
		{
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			try
			{
				ExposeWipe.SlashSeen(target, path);
			}
			catch (Exception ex)
			{
				DevLog.Error("ExposeSlashPatch: " + ex);
			}
		}
	}
	[HarmonyPatch]
