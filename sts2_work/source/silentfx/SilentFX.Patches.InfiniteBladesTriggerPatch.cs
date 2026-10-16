// namespace: SilentFX.Patches
// type: InfiniteBladesTriggerPatch (class)
// source lines: 22781-22801
	public static class InfiniteBladesTriggerPatch
	{
		public static void Prefix(InfiniteBladesPower __instance, Player player)
		{
			try
			{
				if (player != null)
				{
					Creature owner = ((PowerModel)__instance).Owner;
					if (((owner != null) ? owner.Player : null) == player)
					{
						PowerFxManager.Trigger((PowerModel)(object)__instance);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("InfiniteBladesTriggerPatch: " + ex);
			}
		}
	}
