// namespace: SilentFX.Patches
// type: HandHoldPositionPatch (class)
// source lines: 21033-21059
	public static class HandHoldPositionPatch
	{
		public static bool Prefix(ref int handSize, int cardIndex, ref Vector2 __result)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				HandHold.RaiseSize(ref handSize);
				if (HandHold.TryLinePosition(handSize, cardIndex, out var position))
				{
					__result = position;
					return false;
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("HandHoldPositionPatch: " + ex);
			}
			return true;
		}
	}
	[HarmonyPatch(typeof(HandPosHelper), "GetAngle", new Type[]
	{
		typeof(int),
		typeof(int)
	})]
