// namespace: SilentFX.Patches
// type: DamageNumberHoldPatch (class)
// source lines: 20260-20283
	public static class DamageNumberHoldPatch
	{
		public static void Postfix(NDamageNumVfx __result, Vector2 globalPosition, int damage)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			try
			{
				TimeFreeze.OnDamageNumber(__result, globalPosition, damage);
			}
			catch (Exception ex)
			{
				DevLog.Error("DamageNumberHoldPatch: " + ex);
			}
		}
	}
	[HarmonyPatch(typeof(NHitSparkVfx), "Create", new Type[]
	{
		typeof(Creature),
		typeof(bool)
	})]
