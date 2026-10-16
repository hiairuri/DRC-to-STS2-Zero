// namespace: SilentFX.Patches
// type: ShivThrowVfxPatch (class)
// source lines: 21356-21424
	public static class ShivThrowVfxPatch
	{
		private static FieldRef<NShivThrowVfx, Array<GpuParticles2D>> _throw;

		private static bool _looked;

		public static void Prefix(ref Vector2 throwerCenterPosition, ref Color tint, out bool __state)
		{
			__state = false;
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			try
			{
				__state = ShivFx.AdjustVanillaThrow(ref throwerCenterPosition, ref tint);
			}
			catch (Exception ex)
			{
				DevLog.Error("ShivThrowVfxPatch: " + ex);
			}
		}

		public static void Postfix(NShivThrowVfx __result, bool __state)
		{
			if (!__state || __result == null)
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
						_throw = AccessTools.FieldRefAccess<NShivThrowVfx, Array<GpuParticles2D>>("_throwParticles");
					}
					catch (Exception ex)
					{
						DevLog.Warn("ShivThrowVfxPatch: no _throwParticles (" + ex.Message + ")");
					}
				}
				FieldRef<NShivThrowVfx, Array<GpuParticles2D>> val = _throw;
				Array<GpuParticles2D> val2 = ((val != null) ? val.Invoke(__result) : null);
				if (val2 == null)
				{
					return;
				}
				foreach (GpuParticles2D item in val2)
				{
					if (FxKit.Alive((GodotObject)(object)item))
					{
						((CanvasItem)item).Visible = false;
					}
				}
			}
			catch (Exception ex2)
			{
				DevLog.Error("ShivThrowVfxPatch (hide): " + ex2);
			}
		}
	}
	[HarmonyPatch(typeof(NShivThrowVfx), "Create", new Type[]
	{
		typeof(Creature),
		typeof(Creature),
		typeof(Color)
	})]
