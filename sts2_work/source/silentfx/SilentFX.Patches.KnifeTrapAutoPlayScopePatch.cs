// namespace: SilentFX.Patches
// type: KnifeTrapAutoPlayScopePatch (class)
// source lines: 22613-22640
	internal static class KnifeTrapAutoPlayScopePatch
	{
		private sealed class Scope
		{
			internal KnifeTrapSource.Link Previous;
		}

		private static void Prefix([HarmonyArgument(1)] CardModel card, out Scope __state)
		{
			__state = null;
			if (MultiplayerFx.Active)
			{
				__state = new Scope
				{
					Previous = KnifeTrapSource.Enter(card)
				};
			}
		}

		private static void Finalizer(Scope __state)
		{
			if (__state != null)
			{
				KnifeTrapSource.Leave(__state.Previous);
			}
		}
	}
	[HarmonyPatch(typeof(CardModel), "OnPlayWrapper")]
