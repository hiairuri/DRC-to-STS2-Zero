// namespace: SilentFX.Patches
// type: ExposeHooks (class)
// source lines: 19791-19964
	internal static class ExposeHooks
	{
		private static readonly string[] VfxNames = new string[4] { "NPowerAppliedVfx", "NPowerAppliedDebuffVfx", "NPowerAppliedBuffVfx", "NPowerRemovedVfx" };

		private static Dictionary<string, Type> _vfx;

		private static MethodInfo _removal;

		private static bool _looked;

		public static MethodInfo Removal
		{
			get
			{
				if (_looked)
				{
					return _removal;
				}
				_looked = true;
				try
				{
					MethodInfo methodInfo = FindRemoval();
					Patches val = ((methodInfo == null) ? null : Harmony.GetPatchInfo((MethodBase)methodInfo));
					if (val != null && val.Prefixes.Any((Patch p) => p.owner == "sts2.silentfx" && p.PatchMethod.DeclaringType == typeof(ExposeIconHoldPatch)))
					{
						_removal = methodInfo;
					}
					else
					{
						DevLog.Warn("Expose: the power row cannot be held (NPowerContainer.OnPowerRemoved) - the Artifact icon is left to the game");
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Expose: the power row cannot be held: " + ex.Message);
				}
				return _removal;
			}
		}

		internal static MethodInfo FindRemoval()
		{
			try
			{
				return AccessTools.Method(typeof(NPowerContainer), "OnPowerRemoved", new Type[1] { typeof(PowerModel) }, (Type[])null);
			}
			catch
			{
				return null;
			}
		}

		internal static MethodInfo FindIncreased()
		{
			try
			{
				MethodInfo methodInfo = AccessTools.Method(typeof(NCreature), "OnPowerIncreased", new Type[3]
				{
					typeof(PowerModel),
					typeof(int),
					typeof(bool)
				}, (Type[])null);
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
				return AccessTools.Method(typeof(NCreature), "OnPowerIncreased", (Type[])null, (Type[])null);
			}
			catch
			{
				return null;
			}
		}

		internal static List<MethodBase> Creates(string name)
		{
			List<MethodBase> list = new List<MethodBase>();
			try
			{
				Type type = VfxType(name);
				if (type == null)
				{
					return list;
				}
				MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
				foreach (MethodInfo methodInfo in methods)
				{
					if (!(methodInfo.Name != "Create") && !methodInfo.IsGenericMethodDefinition && typeof(Node).IsAssignableFrom(methodInfo.ReturnType) && !methodInfo.GetParameters().Any((ParameterInfo p) => p.ParameterType.IsByRef || p.ParameterType.IsPointer))
					{
						list.Add(methodInfo);
					}
				}
			}
			catch (Exception ex)
			{
				list.Clear();
				DevLog.Warn("Expose: " + name + ".Create could not be looked up: " + ex.Message);
			}
			return list;
		}

		private static Type VfxType(string name)
		{
			if (_vfx == null)
			{
				Dictionary<string, Type> dictionary = new Dictionary<string, Type>(StringComparer.Ordinal);
				Type[] array;
				try
				{
					array = typeof(NCreature).Assembly.GetTypes();
				}
				catch (ReflectionTypeLoadException ex)
				{
					array = ex.Types.Where((Type t) => t != null).ToArray();
				}
				Type[] array2 = array;
				foreach (Type type in array2)
				{
					string text = type.Namespace;
					if (text != null && text.StartsWith("MegaCrit.Sts2.Core.Nodes.Vfx", StringComparison.Ordinal) && !type.IsNested && Array.IndexOf(VfxNames, type.Name) >= 0)
					{
						dictionary[type.Name] = type;
					}
				}
				_vfx = dictionary;
				string[] vfxNames = VfxNames;
				foreach (string text2 in vfxNames)
				{
					if (!dictionary.ContainsKey(text2))
					{
						DevLog.Warn("Expose: the game's " + text2 + " was not found - it will not be covered");
					}
				}
			}
			if (!_vfx.TryGetValue(name, out var value))
			{
				return null;
			}
			return value;
		}

		internal static PowerModel PowerIn(object[] args)
		{
			if (args == null)
			{
				return null;
			}
			foreach (object obj in args)
			{
				PowerModel val = (PowerModel)((obj is PowerModel) ? obj : null);
				if (val != null)
				{
					return val;
				}
			}
			return null;
		}

		internal static void Hide(object made)
		{
			CanvasItem val = (CanvasItem)((made is CanvasItem) ? made : null);
			if (val != null && GodotObject.IsInstanceValid((GodotObject)(object)val))
			{
				val.Visible = false;
			}
		}
	}
	[HarmonyPatch(typeof(VfxCmd), "PlayOnCreatureCenter")]
