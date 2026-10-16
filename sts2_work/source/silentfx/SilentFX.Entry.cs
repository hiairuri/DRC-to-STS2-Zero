// namespace: SilentFX
// type: Entry (class)
// source lines: 211-318
	public static class Entry
	{
		public const string ModId = "SilentFX";

		public const string HarmonyId = "sts2.silentfx";

		public static Harmony Harmony { get; private set; }

		public static void Init()
		{
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Expected Obj, but got Unknown
			DevLog.Start();
			DevLog.Info($"{"SilentFX"} v{typeof(Entry).Assembly.GetName().Version} initializing");
			try
			{
				Tuning.Load();
				PlayerSettings.Init();
				GameCapabilities.Initialize();
				AimCompatibility.Initialize();
				HandPace.Init();
				if (Ticker.Init())
				{
					((Node)Ticker.Tree.Root).TreeExiting += DevLog.Flush;
					FxKit.Warm();
					AssetWarmup.Add("common particle pipelines", (Action)ShaderWarmup.Primitives);
					ShaderLib.Warm();
					AssetWarmup.Add("energy bead resources", (Action)EnergyBeads.Warm);
					AssetWarmup.Add("Echoing Slash variants", (Action)EchoingSlashFx.Warm);
					Sfx.Init();
					InitializePart("PowerFxManager", PowerFxManager.Init);
					InitializePart("CardFxManager", CardFxManager.Init);
					InitializePart("HandBackend", HandBackend.Init);
					InitializePart("HandMoves", HandMoves.Init);
					InitializePart("GambleFlow", GambleFlow.Init);
					InitializePart("PreciseCutChains", PreciseCutChains.Init);
					InitializePart("SlyGold", SlyGold.Init);
					InitializePart("DevPreview", DevPreview.Init);
					InitializePart("Showcase", Showcase.Init);
					InitializePart("TestBench", TestBench.Init);
					InitializePart("TourController", TourController.Init);
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("Setup failed: " + ex);
			}
			try
			{
				Harmony = new Harmony("sts2.silentfx");
				int num = 0;
				Type[] typesFromAssembly = AccessTools.GetTypesFromAssembly(Assembly.GetExecutingAssembly());
				foreach (Type type in typesFromAssembly)
				{
					if (GeneralExtensions.HasHarmonyAttribute(type))
					{
						try
						{
							Harmony.CreateClassProcessor(type).Patch();
						}
						catch (Exception ex2)
						{
							num++;
							DevLog.Error("Harmony patch class failed (" + type.FullName + "): " + ex2);
						}
					}
				}
				int value = Harmony.GetPatchedMethods().Count();
				int value2 = VfxFactoryPatches.Apply(Harmony);
				DevLog.Info($"Harmony patched methods: {value}, failed classes: {num} (+ {value2} of the game's effect factories watched for Bullet Time)");
			}
			catch (Exception ex3)
			{
				DevLog.Error("Harmony assembly setup failed: " + ex3);
			}
			try
			{
				if (Harmony != null && TourController.Enabled)
				{
					TourPatches.Install(Harmony);
				}
				if (Harmony != null)
				{
					AttackFeedPatches.Install(Harmony);
				}
			}
			catch (Exception ex4)
			{
				DevLog.Warn("Tour patches failed: " + ex4.Message);
			}
			DevLog.Info("Init finished");
		}

		private static void InitializePart(string name, Action initialize)
		{
			try
			{
				initialize();
			}
			catch (Exception ex)
			{
				DevLog.Error("Setup failed (" + name + "): " + ex);
			}
		}
	}
}
namespace SilentFX.Powers
{
