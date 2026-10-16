// namespace: SilentFX.Core
// type: HandCards (class)
// source lines: 38075-38332
	public static class HandCards
	{
		public struct Entry
		{
			public CardModel Card;

			public NCard Node;

			public NHandCardHolder Holder;
		}

		private static FieldRef<NPlayerHand, NSelectedHandCardContainer> _selected;

		private static FieldRef<NClickableControl, bool> _hovered;

		private static bool _selectedLooked;

		private static bool _hoveredLooked;

		public static bool Selecting
		{
			get
			{
				try
				{
					NPlayerHand instance = NPlayerHand.Instance;
					return FxKit.Alive((GodotObject)(object)instance) && ((Node)instance).IsInsideTree() && instance.IsInCardSelection;
				}
				catch (Exception ex)
				{
					DevLog.Warn("HandCards.Selecting: " + ex);
				}
				return false;
			}
		}

		public static void Fill(List<Entry> into)
		{
			try
			{
				if (into == null)
				{
					return;
				}
				into.Clear();
				NPlayerHand instance = NPlayerHand.Instance;
				if (!FxKit.Alive((GodotObject)(object)instance) || !((Node)instance).IsInsideTree())
				{
					return;
				}
				foreach (NHandCardHolder activeHolder in instance.ActiveHolders)
				{
					if (FxKit.Alive((GodotObject)(object)activeHolder))
					{
						NCard cardNode = ((NCardHolder)activeHolder).CardNode;
						if (((NCardHolder)activeHolder).CardModel != null && FxKit.Alive((GodotObject)(object)cardNode))
						{
							into.Add(new Entry
							{
								Card = ((NCardHolder)activeHolder).CardModel,
								Node = cardNode,
								Holder = activeHolder
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				into?.Clear();
				DevLog.Warn("HandCards.Fill: " + ex);
			}
		}

		public static void FillSelected(List<Entry> into)
		{
			try
			{
				if (into == null)
				{
					return;
				}
				into.Clear();
				NPlayerHand instance = NPlayerHand.Instance;
				if (!FxKit.Alive((GodotObject)(object)instance) || !((Node)instance).IsInsideTree())
				{
					return;
				}
				if (!_selectedLooked)
				{
					_selectedLooked = true;
					try
					{
						_selected = AccessTools.FieldRefAccess<NPlayerHand, NSelectedHandCardContainer>("_selectedHandCardContainer");
					}
					catch (Exception ex)
					{
						DevLog.Warn("HandCards: no selected hand container (" + ex.Message + ")");
					}
				}
				FieldRef<NPlayerHand, NSelectedHandCardContainer> selected = _selected;
				NSelectedHandCardContainer val = ((selected != null) ? selected.Invoke(instance) : null);
				if (!FxKit.Alive((GodotObject)(object)val))
				{
					return;
				}
				foreach (NSelectedHandCardHolder holder in val.Holders)
				{
					if (FxKit.Alive((GodotObject)(object)holder))
					{
						NCard cardNode = ((NCardHolder)holder).CardNode;
						if (((NCardHolder)holder).CardModel != null && FxKit.Alive((GodotObject)(object)cardNode))
						{
							into.Add(new Entry
							{
								Card = ((NCardHolder)holder).CardModel,
								Node = cardNode,
								Holder = null
							});
						}
					}
				}
			}
			catch (Exception ex2)
			{
				into?.Clear();
				DevLog.Warn("HandCards.FillSelected: " + ex2);
			}
		}

		public static bool SelectedPlace(out Vector2 at)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			at = default;
			try
			{
				NPlayerHand instance = NPlayerHand.Instance;
				if (!FxKit.Alive((GodotObject)(object)instance) || !((Node)instance).IsInsideTree())
				{
					return false;
				}
				if (!_selectedLooked)
				{
					_selectedLooked = true;
					try
					{
						_selected = AccessTools.FieldRefAccess<NPlayerHand, NSelectedHandCardContainer>("_selectedHandCardContainer");
					}
					catch (Exception ex)
					{
						DevLog.Warn("HandCards: no selected hand container (" + ex.Message + ")");
					}
				}
				FieldRef<NPlayerHand, NSelectedHandCardContainer> selected = _selected;
				NSelectedHandCardContainer val = ((selected != null) ? selected.Invoke(instance) : null);
				if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree())
				{
					return false;
				}
				at = ((Control)val).GlobalPosition;
				return true;
			}
			catch (Exception ex2)
			{
				DevLog.Warn("HandCards.SelectedPlace: " + ex2);
			}
			return false;
		}

		public static NCard NodeOf(CardModel card)
		{
			try
			{
				if (card == null)
				{
					return null;
				}
				NPlayerHand instance = NPlayerHand.Instance;
				if (FxKit.Alive((GodotObject)(object)instance) && ((Node)instance).IsInsideTree())
				{
					NCard card2 = instance.GetCard(card);
					if (FxKit.Alive((GodotObject)(object)card2))
					{
						return card2;
					}
				}
				NCard val = NCard.FindOnTable(card, (PileType?)null);
				return FxKit.Alive((GodotObject)(object)val) ? val : null;
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandCards.NodeOf: " + ex);
			}
			return null;
		}

		public static CardModel Focused()
		{
			try
			{
				NPlayerHand instance = NPlayerHand.Instance;
				if (!FxKit.Alive((GodotObject)(object)instance) || !((Node)instance).IsInsideTree() || !((CanvasItem)instance).IsVisibleInTree())
				{
					return null;
				}
				NHandCardHolder focusedHolder = instance.FocusedHolder;
				return (FxKit.Alive((GodotObject)(object)focusedHolder) && ((Node)focusedHolder).IsInsideTree()) ? ((NCardHolder)focusedHolder).CardModel : null;
			}
			catch (Exception ex)
			{
				DevLog.Warn("HandCards.Focused: " + ex);
			}
			return null;
		}

		public static bool EndTurnHovered()
		{
			try
			{
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				object obj;
				if (combatRoom == null)
				{
					obj = null;
				}
				else
				{
					NCombatUi ui = combatRoom.Ui;
					obj = ((ui != null) ? ui.EndTurnButton : null);
				}
				NEndTurnButton val = (NEndTurnButton)obj;
				if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree() || !((CanvasItem)val).IsVisibleInTree() || !((NClickableControl)val).IsEnabled)
				{
					return false;
				}
				if (!_hoveredLooked)
				{
					_hoveredLooked = true;
					try
					{
						_hovered = AccessTools.FieldRefAccess<NClickableControl, bool>("_isHovered");
					}
					catch (Exception ex)
					{
						DevLog.Warn("HandCards: no button hover field (" + ex.Message + ")");
					}
				}
				return _hovered != null && _hovered.Invoke((NClickableControl)(object)val);
			}
			catch (Exception ex2)
			{
				DevLog.Warn("HandCards.EndTurnHovered: " + ex2);
			}
			return false;
		}
	}
