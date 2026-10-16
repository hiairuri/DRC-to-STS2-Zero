// namespace: SilentFX.Patches
// type: WraithCardFlightPatch (class)
// source lines: 23615-23635
	public static class WraithCardFlightPatch
	{
		public static void Postfix(NCardFlyPowerVfx __instance)
		{
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			try
			{
				WraithPlayFx.CardFlies(__instance);
			}
			catch (Exception ex)
			{
				DevLog.Error("WraithCardFlightPatch: " + ex);
			}
		}
	}
}
namespace SilentFX.Dev
{
