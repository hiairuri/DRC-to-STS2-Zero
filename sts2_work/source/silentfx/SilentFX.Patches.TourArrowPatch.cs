// namespace: SilentFX.Patches
// type: TourArrowPatch (class)
// source lines: 23535-23557
	public static class TourArrowPatch
	{
		public static bool Prefix(InputEvent inputEvent, MethodBase __originalMethod)
		{
			try
			{
				if (!TourController.Active)
				{
					return true;
				}
				if (TourController.BlockingInput && __originalMethod.DeclaringType == typeof(NInputManager))
				{
					return false;
				}
				return !TourController.CaptureArrow(inputEvent);
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourArrowPatch: " + ex);
				return true;
			}
		}
	}
