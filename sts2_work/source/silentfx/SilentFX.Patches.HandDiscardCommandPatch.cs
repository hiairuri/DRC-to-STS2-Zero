// namespace: SilentFX.Patches
// type: HandDiscardCommandPatch (class)
// source lines: 20819-20932
	public static class HandDiscardCommandPatch
	{
		public sealed class State
		{
			public AbstractModel Cause;

			public bool Ended;
		}

		public static void Prefix(PlayerChoiceContext choiceContext, ref IEnumerable<CardModel> cardsToDiscard, int cardsToDraw, out State __state)
		{
			__state = null;
			try
			{
				if (cardsToDiscard == null)
				{
					return;
				}
				List<CardModel> list = (List<CardModel>)(cardsToDiscard = (cardsToDiscard as List<CardModel>) ?? new List<CardModel>(cardsToDiscard));
				if (list.Count == 0)
				{
					return;
				}
				foreach (CardModel item in list)
				{
					if (!HandFlow.Mine((item != null) ? item.Owner : null))
					{
						return;
					}
				}
				__state = new State
				{
					Cause = ((choiceContext != null) ? choiceContext.LastInvolvedModel : null)
				};
				HandFlow.RaiseDiscardBegins(list, __state.Cause, cardsToDraw);
			}
			catch (Exception ex)
			{
				DevLog.Error("HandDiscardCommandPatch (begin): " + ex);
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
				DevLog.Error("HandDiscardCommandPatch (wrap): " + ex);
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
					HandFlow.RaiseDiscardEnded(state.Cause);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("HandDiscardCommandPatch (end): " + ex);
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
				DevLog.Error("HandDiscardCommandPatch (fault): " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "AfterCardDiscarded", new Type[]
	{
		typeof(ICombatState),
		typeof(PlayerChoiceContext),
		typeof(CardModel)
	})]
