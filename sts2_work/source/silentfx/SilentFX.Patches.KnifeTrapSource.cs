// namespace: SilentFX.Patches
// type: KnifeTrapSource (class)
// source lines: 22560-22612
	internal static class KnifeTrapSource
	{
		internal sealed class Link
		{
			internal CardModel Trap;

			internal CardModel Shiv;

			internal object Invocation;

			internal object Room;

			internal object Owner;
		}

		private static readonly AsyncLocal<Link> Current = new AsyncLocal<Link>();

		internal static Link Enter(CardModel child)
		{
			Link value = Current.Value;
			CardModel currentCard = MultiplayerFx.CurrentCard;
			Current.Value = ((MultiplayerFx.Active && MultiplayerFx.CurrentInvocation != null && currentCard is KnifeTrap && child is Shiv && child.Owner != null && child.Owner == currentCard.Owner) ? new Link
			{
				Trap = currentCard,
				Shiv = child,
				Invocation = MultiplayerFx.CurrentInvocation,
				Room = GameRefs.CombatRoom,
				Owner = currentCard.Owner
			} : null);
			return value;
		}

		internal static void Leave(Link previous)
		{
			Current.Value = previous;
		}

		internal static CardModel For(CardModel shiv)
		{
			return LinkFor(shiv)?.Trap;
		}

		internal static Link LinkFor(CardModel shiv)
		{
			Link value = Current.Value;
			if (!MultiplayerFx.Active || shiv == null || shiv != MultiplayerFx.CurrentCard || shiv != value?.Shiv || shiv.Owner != value.Trap.Owner || shiv.Owner != value.Owner || value.Invocation == null || value.Room != GameRefs.CombatRoom)
			{
				return null;
			}
			return value;
		}
	}
	[HarmonyPatch(typeof(CardCmd), "AutoPlay")]
