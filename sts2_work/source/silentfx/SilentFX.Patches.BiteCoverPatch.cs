// namespace: SilentFX.Patches
// type: BiteCoverPatch (class)
// source lines: 21809-21901
	public static class BiteCoverPatch
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class <>c
		{
			public static readonly <>c <>9 = new <>c();

			public static ChildEnteredTreeEventHandler <>9__1_0;

			internal void <Cover>b__1_0(Node node)
			{
				CanvasItem val = (CanvasItem)(object)((node is CanvasItem) ? node : null);
				if (val != null)
				{
					val.Visible = false;
				}
			}
		}

		public static bool Prefix(Creature target, string path, out Action __state)
		{
			__state = null;
			try
			{
				if (!PlayerSettings.Enabled)
				{
					return true;
				}
				if (path != "vfx/vfx_bite" || !SnakebiteFx.CoversBite)
				{
					return true;
				}
				if (TimeFreeze.Active)
				{
					return false;
				}
				Control val = ((target != null) ? target.GetVfxContainer() : null);
				if (!FxKit.Alive((GodotObject)(object)val))
				{
					return true;
				}
				__state = Cover((Node)(object)val);
			}
			catch (Exception ex)
			{
				DevLog.Error("BiteCoverPatch: " + ex);
			}
			return true;
		}

		private static Action Cover(Node container)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected Obj, but got Unknown
			ChildEnteredTreeEventHandler val = <>c.<>9__1_0;
			if (val == null)
			{
				ChildEnteredTreeEventHandler val2 = (Node node) =>
				{
					CanvasItem val3 = (CanvasItem)(object)((node is CanvasItem) ? node : null);
					if (val3 != null)
					{
						val3.Visible = false;
					}
				};
				<>c.<>9__1_0 = val2;
				val = val2;
			}
			ChildEnteredTreeEventHandler hide = val;
			container.ChildEnteredTree += hide;
			return () =>
			{
				if (FxKit.Alive((GodotObject)(object)container))
				{
					container.ChildEnteredTree -= hide;
				}
			};
		}

		public static void Finalizer(Action __state)
		{
			try
			{
				__state?.Invoke();
			}
			catch
			{
			}
		}
	}
	[HarmonyPatch(typeof(Hook), "BeforeCardPlayed")]
