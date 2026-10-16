// namespace: SilentFX.Patches
// type: SceneVfxHoldPatch (class)
// source lines: 20303-20323
	public static class SceneVfxHoldPatch
	{
		public static bool Prefix(Vector2 position, string path, Control vfxContainer)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			if (!PlayerSettings.Enabled)
			{
				return true;
			}
			try
			{
				return !TimeFreeze.SkipSceneVfx(position, path, vfxContainer);
			}
			catch (Exception ex)
			{
				DevLog.Error("SceneVfxHoldPatch: " + ex);
				return true;
			}
		}
	}
	[HarmonyPatch(typeof(VfxCmd), "PlayFullScreenInCombat")]
