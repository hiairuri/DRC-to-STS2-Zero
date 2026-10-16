// namespace: SilentFX.Patches
// type: HandShuffleCommandPatch (class)
// source lines: 20672-20774
	public static class HandShuffleCommandPatch
	{
		public sealed class State
		{
			public Player Player;

			public bool Ended;
		}

		public static void Prefix(Player player, out State __state)
		{
			__state = null;
			try
			{
				if (player != null && LocalContext.IsMe(player))
				{
					__state = new State
					{
						Player = player
					};
					HandFlow.RaiseShuffleBegins(player);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("HandShuffleCommandPatch (begin): " + ex);
			}
		}

		public static void Postfix(ref Task __result, State __state)
		{
			try
			{
				if (__state != null)
				{
					if (__result == null)
					{
						End(__state);
					}
					else
					{
						__result = Complete(__result, __state);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("HandShuffleCommandPatch (wrap): " + ex);
				End(__state);
			}
		}

		private static async Task Complete(Task original, State state)
		{
			try
			{
				await original;
			}
			finally
			{
				End(state);
			}
		}

		private static void End(State state)
		{
			try
			{
				if (state != null && !state.Ended)
				{
					state.Ended = true;
					HandFlow.RaiseShuffleEnded(state.Player);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("HandShuffleCommandPatch (end): " + ex);
			}
		}

		public static void Finalizer(Exception __exception, State __state)
		{
			try
			{
				if (__exception != null)
				{
					End(__state);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("HandShuffleCommandPatch (fault): " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(CardPileCmd), "Add", new Type[]
	{
		typeof(CardModel),
		typeof(PileType),
		typeof(CardPilePosition),
		typeof(AbstractModel),
		typeof(bool)
	})]
