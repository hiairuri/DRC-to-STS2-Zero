// namespace: SilentFX.Patches
// type: BurstTriggerPatch (class)
// source lines: 22193-22226
	public static class BurstTriggerPatch
	{
		public static void Prefix(BurstPower __instance, CardModel __0)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Invalid comparison between Unknown and I4
			try
			{
				if (__0 != null)
				{
					if ((int)__0.Type != 2)
					{
						return;
					}
					Player owner = __0.Owner;
					if (((owner != null) ? owner.Creature : null) != ((PowerModel)__instance).Owner)
					{
						return;
					}
				}
				BurstFx.Note(__0);
				PowerFxManager.Trigger((PowerModel)(object)__instance);
			}
			catch (Exception ex)
			{
				DevLog.Error("BurstTriggerPatch: " + ex);
			}
			finally
			{
				BurstFx.Note(null);
			}
		}
	}
	[HarmonyPatch(typeof(NCard), "AnimMultiCardPlay", new Type[] { })]
