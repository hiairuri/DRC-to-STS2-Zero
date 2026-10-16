// namespace: SilentFX.Patches
// type: ShivCreatedPatch (class)
// source lines: 22291-22315
	public static class ShivCreatedPatch
	{
		public static bool Prepare()
		{
			return GameCapabilities.ShivCreateInHand != null;
		}

		public static MethodBase TargetMethod()
		{
			return GameCapabilities.ShivCreateInHand;
		}

		public static void Prefix(Player __0, int __1)
		{
			try
			{
				ShivFx.OnShivsCreated(__0, __1);
			}
			catch (Exception ex)
			{
				DevLog.Error("ShivCreatedPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NCombatRoom), "_ExitTree")]
