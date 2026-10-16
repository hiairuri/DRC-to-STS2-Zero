// namespace: SilentFX.Core
// type: TurnLog (class)
// source lines: 45420-45556
	public static class TurnLog
	{
		public static void Played(Player player, List<CardModel> into, CardType? only = null)
		{
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (into == null)
				{
					return;
				}
				into.Clear();
				object obj;
				if (player == null)
				{
					obj = null;
				}
				else
				{
					Creature creature = player.Creature;
					obj = ((creature != null) ? creature.CombatState : null);
				}
				ICombatState val = (ICombatState)obj;
				CombatManager instance = CombatManager.Instance;
				object obj2;
				if (instance == null)
				{
					obj2 = null;
				}
				else
				{
					CombatHistory history = instance.History;
					obj2 = ((history != null) ? history.CardPlaysFinished : null);
				}
				IEnumerable<CardPlayFinishedEntry> enumerable = (IEnumerable<CardPlayFinishedEntry>)obj2;
				if (val == null || enumerable == null)
				{
					return;
				}
				foreach (CardPlayFinishedEntry item in enumerable)
				{
					CardPlay val2 = ((item != null) ? item.CardPlay : null);
					CardModel val3 = ((val2 != null) ? val2.Card : null);
					if (val3 != null && RuntimeApiCompatibility.PlayerOf(val2) == player && ((CombatHistoryEntry)item).HappenedThisTurn(val) && (!only.HasValue || val3.Type == only.Value))
					{
						into.Add(val3);
					}
				}
			}
			catch (Exception ex)
			{
				into?.Clear();
				DevLog.Warn("TurnLog.Played: " + ex);
			}
		}

		public static int Count(Player player, CardType? only = null)
		{
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				object obj;
				if (player == null)
				{
					obj = null;
				}
				else
				{
					Creature creature = player.Creature;
					obj = ((creature != null) ? creature.CombatState : null);
				}
				ICombatState val = (ICombatState)obj;
				CombatManager instance = CombatManager.Instance;
				object obj2;
				if (instance == null)
				{
					obj2 = null;
				}
				else
				{
					CombatHistory history = instance.History;
					obj2 = ((history != null) ? history.CardPlaysFinished : null);
				}
				IEnumerable<CardPlayFinishedEntry> enumerable = (IEnumerable<CardPlayFinishedEntry>)obj2;
				if (val == null || enumerable == null)
				{
					return 0;
				}
				int num = 0;
				foreach (CardPlayFinishedEntry item in enumerable)
				{
					CardPlay val2 = ((item != null) ? item.CardPlay : null);
					CardModel val3 = ((val2 != null) ? val2.Card : null);
					if (val3 != null && RuntimeApiCompatibility.PlayerOf(val2) == player && ((CombatHistoryEntry)item).HappenedThisTurn(val) && (!only.HasValue || val3.Type == only.Value))
					{
						num++;
					}
				}
				return num;
			}
			catch (Exception ex)
			{
				DevLog.Warn("TurnLog.Count: " + ex);
			}
			return 0;
		}

		public static PileType Where(CardModel card)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				PileType? val;
				if (card == null)
				{
					val = null;
				}
				else
				{
					CardPile pile = card.Pile;
					val = ((pile != null) ? new PileType?(pile.Type) : ((PileType?)null));
				}
				PileType? val2 = val;
				return val2.GetValueOrDefault();
			}
			catch (Exception ex)
			{
				DevLog.Warn("TurnLog.Where: " + ex);
			}
			return (PileType)0;
		}
	}
