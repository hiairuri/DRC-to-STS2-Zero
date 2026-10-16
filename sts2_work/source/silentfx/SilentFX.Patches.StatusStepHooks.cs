// namespace: SilentFX.Patches
// type: StatusStepHooks (class)
// source lines: 23115-23193
	internal static class StatusStepHooks
	{
		private static bool _looked;

		private static bool _ready;

		public static bool Ready
		{
			get
			{
				if (_looked)
				{
					return _ready;
				}
				_looked = true;
				try
				{
					bool flag = Ours(FindRefresh(), typeof(StatusAmountHoldPatch));
					bool flag2 = Ours(FindChanged(), typeof(StatusChangeHoldPatch));
					bool flag3 = Ours(FindFlash(), typeof(StatusFlashHoldPatch));
					_ready = flag && (flag2 | flag3);
					if (!_ready)
					{
						DevLog.Warn($"Malaise: the status icons cannot be held (refresh {flag}, changed {flag2}, flash {flag3}) - they are left to the game");
					}
				}
				catch (Exception ex)
				{
					_ready = false;
					DevLog.Warn("Malaise: the status icons cannot be held: " + ex.Message);
				}
				return _ready;
			}
		}

		private static MethodInfo Find(string name)
		{
			try
			{
				MethodInfo methodInfo = AccessTools.Method(typeof(NPower), name, Type.EmptyTypes, (Type[])null);
				if (methodInfo != null)
				{
					return methodInfo;
				}
			}
			catch
			{
			}
			try
			{
				return AccessTools.Method(typeof(NPower), name, (Type[])null, (Type[])null);
			}
			catch
			{
				return null;
			}
		}

		internal static MethodInfo FindRefresh()
		{
			return Find("RefreshAmount");
		}

		internal static MethodInfo FindFlash()
		{
			return Find("FlashPower");
		}

		internal static MethodInfo FindChanged()
		{
			return Find("OnDisplayAmountChanged");
		}

		private static bool Ours(MethodInfo method, Type patch)
		{
			return ((method == null) ? null : Harmony.GetPatchInfo((MethodBase)method))?.Prefixes.Any((Patch p) => p.owner == "sts2.silentfx" && p.PatchMethod.DeclaringType == patch) ?? false;
		}
	}
	[HarmonyPatch]
