// namespace: SilentFX.Patches
// type: PowerRemovedPatch (class)
// source lines: 22688-22707
	public static class PowerRemovedPatch
	{
		public static void Postfix(PowerModel power)
		{
			try
			{
				AfterimagePower val = (AfterimagePower)(object)((power is AfterimagePower) ? power : null);
				if (val != null)
				{
					AfterimageRemoteSeen.Forget(val);
				}
				PowerFxManager.OnRemoved(power);
			}
			catch (Exception ex)
			{
				DevLog.Error("PowerRemovedPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(PowerModel), "SetAmount")]
