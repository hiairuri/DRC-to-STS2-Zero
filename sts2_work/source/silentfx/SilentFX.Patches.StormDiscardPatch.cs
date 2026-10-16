// namespace: SilentFX.Patches
// type: StormDiscardPatch (class)
// source lines: 21493-21625
	public static class StormDiscardPatch
	{
		private static FieldRef<NCardFlyVfx, NCard> _card;

		private static FieldRef<NCardFlyVfx, NCardTrailVfx> _trail;

		private static FieldRef<NCardFlyVfx, Vector2> _startPos;

		private static FieldRef<NCardFlyVfx, Vector2> _endPos;

		private static FieldRef<NCardFlyVfx, float> _arcDir;

		private static FieldRef<NCardFlyVfx, float> _duration;

		private static bool _looked;

		public static void Postfix(NCardFlyVfx __instance)
		{
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			try
			{
				if (!_looked)
				{
					_looked = true;
					try
					{
						_card = AccessTools.FieldRefAccess<NCardFlyVfx, NCard>("_card");
						_trail = AccessTools.FieldRefAccess<NCardFlyVfx, NCardTrailVfx>("_vfx");
					}
					catch (Exception ex)
					{
						DevLog.Warn("StormDiscardPatch: the card-fly effect has changed (" + ex.Message + ")");
					}
					try
					{
						_startPos = AccessTools.FieldRefAccess<NCardFlyVfx, Vector2>("_startPos");
						_endPos = AccessTools.FieldRefAccess<NCardFlyVfx, Vector2>("_endPos");
						_arcDir = AccessTools.FieldRefAccess<NCardFlyVfx, float>("_arcDir");
						_duration = AccessTools.FieldRefAccess<NCardFlyVfx, float>("_duration");
					}
					catch (Exception ex2)
					{
						DevLog.Warn("StormDiscardPatch: the card-flight fields have changed (" + ex2.Message + ")");
					}
				}
				if (_card == null || !FxKit.Alive((GodotObject)(object)__instance))
				{
					return;
				}
				NCard val = _card.Invoke(__instance);
				if (!FxKit.Alive((GodotObject)(object)val))
				{
					return;
				}
				if (StormHeld.Swallowed((Control)(object)val))
				{
					((CanvasItem)val).Visible = false;
					((CanvasItem)val).Modulate = new Color(1f, 1f, 1f, 0f);
					FieldRef<NCardFlyVfx, NCardTrailVfx> trail = _trail;
					NCardTrailVfx val2 = ((trail != null) ? trail.Invoke(__instance) : null);
					if (FxKit.Alive((GodotObject)(object)val2))
					{
						((CanvasItem)val2).Visible = false;
					}
				}
				else
				{
					if (!CardFlight.TryTake(val, out var plan))
					{
						return;
					}
					if (plan.HideCard)
					{
						((CanvasItem)val).Visible = false;
						((CanvasItem)val).Modulate = new Color(1f, 1f, 1f, 0f);
					}
					if (plan.HideTrail)
					{
						FieldRef<NCardFlyVfx, NCardTrailVfx> trail2 = _trail;
						NCardTrailVfx val3 = ((trail2 != null) ? trail2.Invoke(__instance) : null);
						if (FxKit.Alive((GodotObject)(object)val3))
						{
							((CanvasItem)val3).Visible = false;
						}
					}
					if (plan.Straight && _arcDir != null)
					{
						_arcDir.Invoke(__instance) = 0f;
					}
					if (plan.Duration > 0f && _duration != null)
					{
						_duration.Invoke(__instance) = plan.Duration;
					}
					if (plan.Started != null && _startPos != null && _endPos != null)
					{
						try
						{
							plan.Started(val, _startPos.Invoke(__instance), _endPos.Invoke(__instance));
						}
						catch (Exception ex3)
						{
							DevLog.Error("StormDiscardPatch (started): " + ex3);
						}
					}
					if (plan.Hold != null || plan.Released != null)
					{
						object trail4;
						if (!plan.HideTrail)
						{
							FieldRef<NCardFlyVfx, NCardTrailVfx> trail3 = _trail;
							trail4 = ((trail3 != null) ? trail3.Invoke(__instance) : null);
						}
						else
						{
							trail4 = null;
						}
						FlightHold.Begin(__instance, val, (CanvasItem)trail4, plan);
					}
				}
			}
			catch (Exception ex4)
			{
				DevLog.Error("StormDiscardPatch: " + ex4);
			}
		}
	}
