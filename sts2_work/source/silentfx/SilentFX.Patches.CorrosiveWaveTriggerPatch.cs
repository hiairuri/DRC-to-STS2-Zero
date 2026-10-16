// namespace: SilentFX.Patches
// type: CorrosiveWaveTriggerPatch (class)
// source lines: 22449-22476
	public static class CorrosiveWaveTriggerPatch
	{
		public static void Prefix(CorrosiveWavePower __instance, CardModel card)
		{
			try
			{
				object obj;
				if (card == null)
				{
					obj = null;
				}
				else
				{
					Player owner = card.Owner;
					obj = ((owner != null) ? owner.Creature : null);
				}
				if (obj == ((PowerModel)__instance).Owner)
				{
					PowerFxManager.Trigger((PowerModel)(object)__instance);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("CorrosiveWaveTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch]
