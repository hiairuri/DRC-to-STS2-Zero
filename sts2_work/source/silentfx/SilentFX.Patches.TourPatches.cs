// namespace: SilentFX.Patches
// type: TourPatches (class)
// source lines: 23350-23469
	internal static class TourPatches
	{
		internal static IEnumerable<MethodBase> ArrowMethods()
		{
			try
			{
				MethodBase[] array = new MethodBase[2]
				{
					AccessTools.Method(typeof(NInputManager), "ProcessHotkeyInput", new Type[1] { typeof(InputEvent) }, (Type[])null),
					AccessTools.Method(typeof(NInputManager), "ProcessFkbInput", new Type[1] { typeof(InputEvent) }, (Type[])null)
				};
				MethodInfo methodInfo = AccessTools.Method(typeof(NControllerManager), "CheckForArrowKeyInput", new Type[1] { typeof(InputEvent) }, (Type[])null);
				IEnumerable<MethodBase> result;
				if (!(methodInfo == null))
				{
					result = array.Concat(new MethodInfo[1] { methodInfo });
				}
				else
				{
					IEnumerable<MethodBase> enumerable = array;
					result = enumerable;
				}
				return result;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: input targets unavailable: " + ex);
				throw;
			}
		}

		internal static void Install(Harmony harmony)
		{
			int done = 0;
			int failed = 0;
			One("Creature.LoseHpInternal", () => AccessTools.Method(typeof(Creature), "LoseHpInternal", (Type[])null, (Type[])null), typeof(TourDamagePatch));
			One("Creature.SetCurrentHpInternal", () => AccessTools.Method(typeof(Creature), "SetCurrentHpInternal", (Type[])null, (Type[])null), typeof(TourSetHpPatch));
			One("CreatureCmd.KillWithoutCheckingWinCondition", () => AccessTools.Method(typeof(CreatureCmd), "KillWithoutCheckingWinCondition", new Type[3]
			{
				typeof(Creature),
				typeof(bool),
				typeof(int)
			}, (Type[])null), typeof(TourKillPatch));
			One("Hook.BeforeHandDraw", () => AccessTools.Method(typeof(Hook), "BeforeHandDraw", (Type[])null, (Type[])null), typeof(TourBeforeDrawPatch), postfix: true);
			One("NPlayerHand.AreCardActionsAllowed", () => AccessTools.Method(typeof(NPlayerHand), "AreCardActionsAllowed", (Type[])null, (Type[])null), typeof(TourHandLockPatch));
			One("NEndTurnButton.CallReleaseLogic", () => AccessTools.Method(typeof(NEndTurnButton), "CallReleaseLogic", (Type[])null, (Type[])null), typeof(TourEndTurnLockPatch));
			foreach (MethodBase method in ArrowMethods())
			{
				One(method?.DeclaringType?.Name + "." + (method?.Name ?? "missing shared input intercept"), () => method, typeof(TourArrowPatch));
			}
			DevLog.Info($"Tour: {done} patch(es) installed" + ((failed > 0) ? $", {failed} NOT installed - the tour will refuse to start" : ""));
			void One(string what, Func<MethodBase> target, Type patch, bool postfix = false)
			{
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Expected Obj, but got Unknown
				try
				{
					MethodBase methodBase = target();
					if (methodBase == null)
					{
						throw new MissingMethodException(what);
					}
					HarmonyMethod val = new HarmonyMethod(AccessTools.Method(patch, postfix ? "Postfix" : "Prefix", (Type[])null, (Type[])null));
					if (postfix)
					{
						harmony.Patch(methodBase, (HarmonyMethod)null, val, (HarmonyMethod)null, (HarmonyMethod)null);
					}
					else
					{
						harmony.Patch(methodBase, val, (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
					}
					done++;
				}
				catch (Exception ex)
				{
					failed++;
					DevLog.Warn("Tour: patch not installed (" + what + "): " + ex.Message);
				}
			}
		}

		internal static bool VerifyInstalled(out string error)
		{
			error = null;
			try
			{
				List<(MethodBase, Type)> list = new List<(MethodBase, Type)>();
				list.Add((AccessTools.Method(typeof(Creature), "LoseHpInternal", (Type[])null, (Type[])null), typeof(TourDamagePatch)));
				list.Add((AccessTools.Method(typeof(Creature), "SetCurrentHpInternal", (Type[])null, (Type[])null), typeof(TourSetHpPatch)));
				list.Add((AccessTools.Method(typeof(CreatureCmd), "KillWithoutCheckingWinCondition", new Type[3]
				{
					typeof(Creature),
					typeof(bool),
					typeof(int)
				}, (Type[])null), typeof(TourKillPatch)));
				list.Add((AccessTools.Method(typeof(Hook), "BeforeHandDraw", (Type[])null, (Type[])null), typeof(TourBeforeDrawPatch)));
				list.Add((AccessTools.Method(typeof(NPlayerHand), "AreCardActionsAllowed", (Type[])null, (Type[])null), typeof(TourHandLockPatch)));
				list.Add((AccessTools.Method(typeof(NEndTurnButton), "CallReleaseLogic", (Type[])null, (Type[])null), typeof(TourEndTurnLockPatch)));
				list.AddRange(from m in ArrowMethods()
					select (m: m, typeof(TourArrowPatch)));
				foreach (var item2 in list)
				{
					MethodBase item = item2.Item1;
					Type patch = item2.Item2;
					Patches val = ((item == null) ? null : Harmony.GetPatchInfo(item));
					if (val == null || !val.Prefixes.Concat(val.Postfixes).Any((Patch p) => p.owner == "sts2.silentfx" && p.PatchMethod.DeclaringType == patch))
					{
						throw new InvalidOperationException(patch.Name + " / " + (item?.Name ?? "missing method"));
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				error = ex.Message;
				DevLog.Warn("Tour: patch verification failed: " + ex);
				return false;
			}
		}
	}
