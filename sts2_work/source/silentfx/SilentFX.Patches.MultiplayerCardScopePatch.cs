// namespace: SilentFX.Patches
// type: MultiplayerCardScopePatch (class)
// source lines: 22641-22672
	internal static class MultiplayerCardScopePatch
	{
		internal sealed class Scope
		{
			internal CardModel Previous;

			internal object PreviousInvocation;
		}

		private static void Prefix(CardModel __instance, out Scope __state)
		{
			__state = null;
			if (MultiplayerFx.Active)
			{
				__state = new Scope
				{
					Previous = MultiplayerFx.Enter(__instance),
					PreviousInvocation = MultiplayerFx.EnterInvocation()
				};
			}
		}

		private static void Finalizer(Scope __state)
		{
			if (__state != null)
			{
				MultiplayerFx.Leave(__state.Previous);
				MultiplayerFx.LeaveInvocation(__state.PreviousInvocation);
			}
		}
	}
	[HarmonyPatch(typeof(Creature), "ApplyPowerInternal")]
