// namespace: SilentFX.Patches
// type: VfxFactoryPatches (class)
// source lines: 20368-20515
	public static class VfxFactoryPatches
	{
		private static readonly string[] Skip = new string[27]
		{
			"NDamageNumVfx", "NHitSparkVfx", "NCard", "NSpeechBubble", "NThoughtBubble", "NMonsterDeath", "NDoom", "NMap", "NEpoch", "NGainEpoch",
			"NFailedJoin", "NFullscreenText", "NRelicFlash", "NPotionFlash", "NMirror", "NRest", "NRain", "NLowHpBorder", "NUiFlash", "NSleeping",
			"NStunned", "NKinPriest", "NHellraiser", "NSovereignBlade", "NVfxProjectileHandler", "NGaseousScreenVfx", "NDesaturateTransition"
		};

		public static int Apply(Harmony harmony)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected Obj, but got Unknown
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Expected Obj, but got Unknown
			int num = 0;
			HarmonyMethod val = new HarmonyMethod(typeof(VfxFactoryPatches).GetMethod("Made", BindingFlags.Static | BindingFlags.Public));
			Type[] array;
			try
			{
				array = typeof(NDamageNumVfx).Assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				array = ex.Types.Where((Type t) => t != null).ToArray();
			}
			Type[] array2 = array;
			foreach (Type type in array2)
			{
				string text = type.Namespace;
				if (text == null || !text.StartsWith("MegaCrit.Sts2.Core.Nodes.Vfx", StringComparison.Ordinal) || text.Contains(".Forms") || text.Contains(".Backgrounds") || text.Contains(".Events") || text.Contains(".Examples") || type.IsGenericTypeDefinition || type.IsNested || Skip.Any((string s) => type.Name.StartsWith(s, StringComparison.Ordinal)))
				{
					continue;
				}
				MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
				foreach (MethodInfo methodInfo in methods)
				{
					if (!(methodInfo.Name != "Create") && !methodInfo.IsGenericMethodDefinition && typeof(Node).IsAssignableFrom(methodInfo.ReturnType) && !methodInfo.GetParameters().Any((ParameterInfo p) => p.ParameterType.IsByRef || p.ParameterType.IsPointer))
					{
						try
						{
							harmony.Patch((MethodBase)methodInfo, (HarmonyMethod)null, val, (HarmonyMethod)null, (HarmonyMethod)null);
							num++;
						}
						catch (Exception ex2)
						{
							DevLog.Warn($"VfxFactoryPatches: could not watch {type.Name}.Create ({ex2.Message})");
						}
					}
				}
			}
			HarmonyMethod val2 = new HarmonyMethod(typeof(VfxFactoryPatches).GetMethod("Burned", BindingFlags.Static | BindingFlags.Public));
			foreach (Type item in CardBurnTypes())
			{
				try
				{
					MethodInfo[] methods = item.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
					foreach (MethodInfo methodInfo2 in methods)
					{
						if (!(methodInfo2.Name != "Create"))
						{
							harmony.Patch((MethodBase)methodInfo2, (HarmonyMethod)null, val2, (HarmonyMethod)null, (HarmonyMethod)null);
							num++;
						}
					}
				}
				catch (Exception ex3)
				{
					DevLog.Warn($"VfxFactoryPatches: could not watch {item.Name}.Create ({ex3.Message})");
				}
			}
			return num;
		}

		internal static IEnumerable<Type> CardBurnTypes()
		{
			string[] array = new string[2] { "NCardExhaustVfx", "NCardExhaustQuickVfx" };
			foreach (string text in array)
			{
				Type type = GameCapabilities.Resolve("MegaCrit.Sts2.Core.Nodes.Vfx.Cards." + text);
				if (type != null)
				{
					yield return type;
				}
			}
		}

		public static void Burned(object __result)
		{
			try
			{
				TimeFreeze.OnCardBurn(__result);
			}
			catch (Exception ex)
			{
				DevLog.Error("VfxFactoryPatches (burn): " + ex);
			}
		}

		public static void Made(MethodBase __originalMethod, object[] __args, object __result)
		{
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			try
			{
				NStabVfx val = (NStabVfx)((__result is NStabVfx) ? __result : null);
				if (val != null && (FinisherFx.CoversStab() || SkewerFx.CoversStab()))
				{
					if (GodotObject.IsInstanceValid((GodotObject)(object)val))
					{
						((CanvasItem)val).Visible = false;
					}
					return;
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("VfxFactoryPatches (Finisher): " + ex);
			}
			try
			{
				NShivThrowVfx val2 = (NShivThrowVfx)((__result is NShivThrowVfx) ? __result : null);
				if (val2 != null && TimeFreeze.Active && ShivFan.HidesThrow(ShivFx.CurrentShiv()))
				{
					if (GodotObject.IsInstanceValid((GodotObject)(object)val2))
					{
						((CanvasItem)val2).Visible = false;
					}
					return;
				}
			}
			catch (Exception ex2)
			{
				DevLog.Error("VfxFactoryPatches (Fan of Knives): " + ex2);
			}
			try
			{
				TimeFreeze.OnVfxCreated(__originalMethod, __args, __result);
			}
			catch (Exception ex3)
			{
				DevLog.Error("VfxFactoryPatches: " + ex3);
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "ModifyHandDraw")]
