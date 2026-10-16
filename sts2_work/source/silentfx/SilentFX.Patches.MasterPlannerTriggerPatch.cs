// namespace: SilentFX.Patches
// type: MasterPlannerTriggerPatch (class)
// source lines: 22073-22098
	public static class MasterPlannerTriggerPatch
	{
		public static void Prefix(MasterPlannerPower __instance, CardPlay cardPlay)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Invalid comparison between Unknown and I4
			try
			{
				CardModel val = ((cardPlay != null) ? cardPlay.Card : null);
				if (val != null && (int)val.Type == 2)
				{
					Player owner = val.Owner;
					if (((owner != null) ? owner.Creature : null) == ((PowerModel)__instance).Owner && !MasterPlannerFx.IsTagged(val) && (val.Keywords == null || !val.Keywords.Contains((CardKeyword)6)))
					{
						MasterPlannerFx.Tag(val);
						MasterPlannerFx.Sealed(((PowerModel)__instance).Owner, val);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("MasterPlannerTriggerPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "AfterCardDiscarded")]
