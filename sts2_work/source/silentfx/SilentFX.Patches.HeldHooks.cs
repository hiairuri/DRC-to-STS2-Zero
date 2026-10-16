// namespace: SilentFX.Patches
// type: HeldHooks (class)
// source lines: 21184-21275
	internal static class HeldHooks
	{
		private static Func<NMouseCardPlay, bool> _inPlayZone;

		private static bool _looked;

		public static bool OverPlayArea(NMouseCardPlay play)
		{
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)play) || !((Node)play).IsInsideTree())
			{
				return false;
			}
			if (!_looked)
			{
				_looked = true;
				try
				{
					MethodInfo methodInfo = AccessTools.Method(typeof(NMouseCardPlay), "IsCardInPlayZone", (Type[])null, (Type[])null);
					if (methodInfo != null)
					{
						_inPlayZone = AccessTools.MethodDelegate<Func<NMouseCardPlay, bool>>(methodInfo, (object)null, true, (Type[])null);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("HeldHooks: using the fallback play-area test: " + ex.Message);
				}
			}
			if (_inPlayZone != null)
			{
				try
				{
					return _inPlayZone(play);
				}
				catch
				{
					_inPlayZone = null;
				}
			}
			Viewport viewport = ((Node)play).GetViewport();
			if (viewport != null)
			{
				float y = viewport.GetMousePosition().Y;
				Rect2 visibleRect = viewport.GetVisibleRect();
				return y < visibleRect.Size.Y * 0.75f;
			}
			return false;
		}

		public static CardModel Hovered()
		{
			NPlayerHand instance = NPlayerHand.Instance;
			if (!FxKit.Alive((GodotObject)(object)instance) || !((Node)instance).IsInsideTree() || !((CanvasItem)instance).IsVisibleInTree())
			{
				return null;
			}
			if (instance.InCardPlay || instance.IsInCardSelection)
			{
				return null;
			}
			NHandCardHolder focusedHolder = instance.FocusedHolder;
			if (!FxKit.Alive((GodotObject)(object)focusedHolder) || !((Node)focusedHolder).IsInsideTree())
			{
				return null;
			}
			return ((NCardHolder)focusedHolder).CardModel;
		}

		public static void Begin(NCardPlay play, Func<bool> overPlayArea)
		{
			object obj;
			if (play == null)
			{
				obj = null;
			}
			else
			{
				NHandCardHolder holder = play.Holder;
				obj = ((holder != null) ? ((NCardHolder)holder).CardModel : null);
			}
			CardModel val = (CardModel)obj;
			if (val != null)
			{
				HeldFxManager.Begin(val, overPlayArea);
			}
		}
	}
	[HarmonyPatch(typeof(NMouseCardPlay), "Start")]
