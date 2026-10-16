// namespace: SilentFX.Core
// type: HandFlow (class)
// source lines: 38700-39119
	public static class HandFlow
	{
		public sealed class DrawInfo
		{
			public Player Player;

			public AbstractModel Cause;

			public bool FromHandDraw;

			public int Requested;

			public int Entered;

			public bool Shuffled;

			public int ShuffledAt;
		}

		private sealed class DrawState
		{
			public DrawInfo Info;

			public readonly HashSet<CardModel> Pending = new HashSet<CardModel>(ReferenceEqualityComparer.Instance);

			public ulong Until;
		}

		private static readonly List<DrawState> DrawStack = new List<DrawState>();

		private static readonly List<DrawState> RecentDraws = new List<DrawState>();

		public static DrawInfo Drawing
		{
			get
			{
				if (DrawStack.Count != 0)
				{
					return DrawStack[DrawStack.Count - 1].Info;
				}
				return null;
			}
		}

		public static event Action<DrawInfo> DrawBegins;

		public static event Action<DrawInfo> DrawEnded;

		public static event Action<Player> ShuffleBegins;

		public static event Action<Player> ShuffleEnded;

		public static event Action<NCard, DrawInfo> DrawnCardEntered;

		public static event Action<IReadOnlyList<CardModel>, AbstractModel, int> DiscardBegins;

		public static event Action<AbstractModel> DiscardEnded;

		public static event Action<CardModel, AbstractModel> CardDiscarded;

		public static event Action<CardModel> SlyPlayBegins;

		public static event Action<Player> PlayerTurnBegins;

		public static event Action<Player> TurnEnding;

		public static event Action<Player, int> HandDrawSized;

		public static event Action<Player> HandDrawBegins;

		public static event Action<CardModel, bool> CardDrawn;

		public static event Action<NCard> CardEnteredHand;

		public static event Action<AbstractModel> SelectionBegan;

		public static event Action<AbstractModel, IReadOnlyList<CardModel>> SelectionEnded;

		internal static void RaiseHandDrawSized(Player player, int count)
		{
			if (Mine(player))
			{
				Raise(HandDrawSized, player, count, "HandDrawSized");
			}
		}

		internal static void RaiseHandDrawBegins(Player player)
		{
			if (Mine(player))
			{
				Raise(HandDrawBegins, player, "HandDrawBegins");
			}
		}

		internal static void RaiseCardDrawn(CardModel card, bool fromHandDraw)
		{
			if (Mine((card != null) ? card.Owner : null))
			{
				Raise(CardDrawn, card, fromHandDraw, "CardDrawn");
			}
		}

		internal static void RaiseCardEnteredHand(NCard node)
		{
			object player;
			if (node == null)
			{
				player = null;
			}
			else
			{
				CardModel model = node.Model;
				player = ((model != null) ? model.Owner : null);
			}
			if (Mine((Player)player))
			{
				Raise(CardEnteredHand, node, "CardEnteredHand");
			}
		}

		internal static void RaiseSelectionBegan(AbstractModel source)
		{
			Raise(SelectionBegan, source, "SelectionBegan");
		}

		internal static void RaiseSelectionEnded(AbstractModel source, IReadOnlyList<CardModel> cards)
		{
			Raise(SelectionEnded, source, cards, "SelectionEnded");
		}

		internal static bool Mine(Player player)
		{
			if (player == null)
			{
				return true;
			}
			try
			{
				return LocalContext.IsMe(player);
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandFlow.Mine: " + ex.Message);
				return true;
			}
		}

		internal static DrawInfo BeginDraw(Player player, AbstractModel cause, decimal count, bool fromHandDraw)
		{
			UpdateRecentDraws();
			DrawInfo drawInfo = new DrawInfo
			{
				Player = player,
				Cause = cause,
				FromHandDraw = fromHandDraw,
				Requested = (int)Math.Clamp(Math.Ceiling(count), 0m, 2147483647m)
			};
			DrawStack.Add(new DrawState
			{
				Info = drawInfo
			});
			if (Mine(player))
			{
				Raise(DrawBegins, drawInfo, "DrawBegins");
			}
			return drawInfo;
		}

		internal static void RaiseShuffleBegins(Player player)
		{
			try
			{
				if (player == null || !LocalContext.IsMe(player))
				{
					return;
				}
				for (int num = DrawStack.Count - 1; num >= 0; num--)
				{
					DrawInfo info = DrawStack[num].Info;
					if (info.Player == player)
					{
						info.Shuffled = true;
						info.ShuffledAt = info.Entered;
						break;
					}
				}
				Raise(ShuffleBegins, player, "ShuffleBegins");
			}
			catch (Exception ex)
			{
				DevLog.Error("HandFlow.ShuffleBegins: " + ex);
			}
		}

		internal static void RaiseShuffleEnded(Player player)
		{
			try
			{
				if (player != null && LocalContext.IsMe(player))
				{
					Raise(ShuffleEnded, player, "ShuffleEnded");
				}
			}
			catch (Exception ex)
			{
				DevLog.Error("HandFlow.ShuffleEnded: " + ex);
			}
		}

		internal static void EndDraw(DrawInfo info)
		{
			if (info == null)
			{
				return;
			}
			for (int num = DrawStack.Count - 1; num >= 0; num--)
			{
				DrawState drawState = DrawStack[num];
				if (drawState.Info == info)
				{
					DrawStack.RemoveAt(num);
					drawState.Until = FxClock.Milliseconds + 600;
					RecentDraws.Add(drawState);
					if (Mine(info.Player))
					{
						Raise(DrawEnded, info, "DrawEnded");
					}
					break;
				}
			}
		}

		internal static void NoteDrawAdd(CardModel card, PileType destination)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Invalid comparison between Unknown and I4
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Invalid comparison between Unknown and I4
			if (card == null || (int)destination != 2)
			{
				return;
			}
			CardPile pile = card.Pile;
			if (pile == null || (int)pile.Type != 1)
			{
				return;
			}
			for (int num = DrawStack.Count - 1; num >= 0; num--)
			{
				DrawState drawState = DrawStack[num];
				if (drawState.Info.Player == null || card.Owner == null || drawState.Info.Player == card.Owner)
				{
					drawState.Pending.Add(card);
					break;
				}
			}
		}

		internal static void NoteDrawEntered(NCard node)
		{
			if ((DrawStack.Count == 0 && RecentDraws.Count == 0) || !FxKit.Alive((GodotObject)(object)node) || node.Model == null)
			{
				return;
			}
			UpdateRecentDraws();
			for (int num = DrawStack.Count - 1; num >= 0; num--)
			{
				if (ClaimArrival(DrawStack[num], node))
				{
					return;
				}
			}
			int num2 = RecentDraws.Count - 1;
			while (num2 >= 0 && !ClaimArrival(RecentDraws[num2], node))
			{
				num2--;
			}
		}

		private static bool ClaimArrival(DrawState state, NCard node)
		{
			if (!state.Pending.Remove(node.Model))
			{
				return false;
			}
			state.Info.Entered++;
			if (Mine(state.Info.Player) && Mine(node.Model.Owner))
			{
				Raise(DrawnCardEntered, node, state.Info, "DrawnCardEntered");
			}
			return true;
		}

		internal static void UpdateRecentDraws()
		{
			if (RecentDraws.Count == 0)
			{
				return;
			}
			ulong milliseconds = FxClock.Milliseconds;
			for (int num = RecentDraws.Count - 1; num >= 0; num--)
			{
				if (milliseconds >= RecentDraws[num].Until)
				{
					RecentDraws.RemoveAt(num);
				}
			}
		}

		internal static void ResetDrawState()
		{
			DrawStack.Clear();
			RecentDraws.Clear();
		}

		internal static void RaiseDiscardBegins(IReadOnlyList<CardModel> cards, AbstractModel cause, int cardsToDraw)
		{
			Raise(DiscardBegins, cards, cause, cardsToDraw, "DiscardBegins");
		}

		internal static void RaiseDiscardEnded(AbstractModel cause)
		{
			Raise(DiscardEnded, cause, "DiscardEnded");
		}

		internal static void RaiseCardDiscarded(CardModel card, AbstractModel cause)
		{
			if (Mine((card != null) ? card.Owner : null))
			{
				Raise(CardDiscarded, card, cause, "CardDiscarded");
			}
		}

		internal static void RaiseSlyPlayBegins(CardModel card)
		{
			if (Mine((card != null) ? card.Owner : null))
			{
				Raise(SlyPlayBegins, card, "SlyPlayBegins");
			}
		}

		internal static void RaisePlayerTurnBegins(Player player)
		{
			if (Mine(player))
			{
				Raise(PlayerTurnBegins, player, "PlayerTurnBegins");
			}
		}

		internal static void RaiseTurnEnding(Player player)
		{
			if (player != null && LocalContext.IsMe(player))
			{
				Raise(TurnEnding, player, "TurnEnding");
			}
		}

		private static void Raise<T, U, V>(Action<T, U, V> handlers, T first, U second, V third, string name)
		{
			if (handlers == null)
			{
				return;
			}
			Delegate[] invocationList = handlers.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Action<T, U, V> action = (Action<T, U, V>)invocationList[i];
				try
				{
					action(first, second, third);
				}
				catch (Exception ex)
				{
					DevLog.Error("HandFlow." + name + ": " + ex);
				}
			}
		}

		private static void Raise<T>(Action<T> handlers, T value, string name)
		{
			if (handlers == null)
			{
				return;
			}
			Delegate[] invocationList = handlers.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Action<T> action = (Action<T>)invocationList[i];
				try
				{
					action(value);
				}
				catch (Exception ex)
				{
					DevLog.Error("HandFlow." + name + ": " + ex);
				}
			}
		}

		private static void Raise<T, U>(Action<T, U> handlers, T first, U second, string name)
		{
			if (handlers == null)
			{
				return;
			}
			Delegate[] invocationList = handlers.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Action<T, U> action = (Action<T, U>)invocationList[i];
				try
				{
					action(first, second);
				}
				catch (Exception ex)
				{
					DevLog.Error("HandFlow." + name + ": " + ex);
				}
			}
		}
	}
