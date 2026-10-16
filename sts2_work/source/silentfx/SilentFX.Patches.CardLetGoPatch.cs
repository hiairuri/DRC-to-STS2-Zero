// namespace: SilentFX.Patches
// type: CardLetGoPatch (class)
// source lines: 21336-21355
	public static class CardLetGoPatch
	{
		public static void Prefix(bool isFinished)
		{
			try
			{
				HeldFxManager.End(isFinished);
			}
			catch (Exception ex)
			{
				DevLog.Error("CardLetGoPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NShivThrowVfx), "Create", new Type[]
	{
		typeof(Vector2),
		typeof(Vector2),
		typeof(Color)
	})]
