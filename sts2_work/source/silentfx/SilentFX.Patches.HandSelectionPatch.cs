// namespace: SilentFX.Patches
// type: HandSelectionPatch (class)
// source lines: 21126-21183
	public static class HandSelectionPatch
	{
		public static void Prefix(AbstractModel source)
		{
			try
			{
				HandFlow.RaiseSelectionBegan(source);
			}
			catch (Exception ex)
			{
				DevLog.Error("HandSelectionPatch (begin): " + ex);
			}
		}

		public static void Postfix(ref Task<IEnumerable<CardModel>> __result, AbstractModel source)
		{
			try
			{
				__result = Complete(__result, source);
			}
			catch (Exception ex)
			{
				DevLog.Error("HandSelectionPatch (wrap): " + ex);
			}
		}

		private static async Task<IEnumerable<CardModel>> Complete(Task<IEnumerable<CardModel>> original, AbstractModel source)
		{
			IEnumerable<CardModel> enumerable;
			try
			{
				enumerable = await original;
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandSelectionPatch (selection): " + ex);
				try
				{
					HandFlow.RaiseSelectionEnded(source, new List<CardModel>());
				}
				catch (Exception ex2)
				{
					DevLog.Error("HandSelectionPatch (end): " + ex2);
				}
				throw;
			}
			List<CardModel> list = ((enumerable == null) ? new List<CardModel>() : new List<CardModel>(enumerable));
			try
			{
				HandFlow.RaiseSelectionEnded(source, list);
			}
			catch (Exception ex3)
			{
				DevLog.Error("HandSelectionPatch (end): " + ex3);
			}
			return list;
		}
	}
