// namespace: SilentFX.Patches
// type: HandDrawCommandPatch (class)
// source lines: 20590-20671
	public static class HandDrawCommandPatch
	{
		public static void Prefix(PlayerChoiceContext choiceContext, decimal count, Player player, bool fromHandDraw, out HandFlow.DrawInfo __state)
		{
			__state = null;
			try
			{
				__state = HandFlow.BeginDraw(player, (choiceContext != null) ? choiceContext.LastInvolvedModel : null, count, fromHandDraw);
			}
			catch (Exception ex)
			{
				DevLog.Error("HandDrawCommandPatch (begin): " + ex);
			}
		}

		public static void Postfix(ref Task<IEnumerable<CardModel>> __result, HandFlow.DrawInfo __state)
		{
			try
			{
				if (__state != null)
				{
					if (__result == null)
					{
						HandFlow.EndDraw(__state);
					}
					else
					{
						__result = Complete(__result, __state);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("HandDrawCommandPatch (wrap): " + ex);
				End(__state);
			}
		}

		private static async Task<IEnumerable<CardModel>> Complete(Task<IEnumerable<CardModel>> original, HandFlow.DrawInfo info)
		{
			try
			{
				return await original;
			}
			finally
			{
				End(info);
			}
		}

		private static void End(HandFlow.DrawInfo info)
		{
			try
			{
				HandFlow.EndDraw(info);
			}
			catch (Exception ex)
			{
				DevLog.Error("HandDrawCommandPatch (end): " + ex);
			}
		}

		public static void Finalizer(Exception __exception, HandFlow.DrawInfo __state)
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
				DevLog.Error("HandDrawCommandPatch (fault): " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(CardPileCmd), "Shuffle", new Type[]
	{
		typeof(PlayerChoiceContext),
		typeof(Player)
	})]
