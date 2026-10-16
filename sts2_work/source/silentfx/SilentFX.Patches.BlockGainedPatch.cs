// namespace: SilentFX.Patches
// type: BlockGainedPatch (class)
// source lines: 23098-23114
	public static class BlockGainedPatch
	{
		public static void Prefix(Creature receiver, int amount, CardPlay cardPlay)
		{
			try
			{
				if (receiver != null && cardPlay != null && amount > 0 && receiver.HasPower<ShadowmeldPower>())
				{
					PowerFxManager.TriggerFor<ShadowmeldPower>(receiver, (Creature)null);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("BlockGainedPatch: " + ex);
			}
		}
	}
