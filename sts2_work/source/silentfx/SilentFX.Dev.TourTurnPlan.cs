// namespace: SilentFX.Dev
// type: TourTurnPlan (class)
// source lines: 29416-29538
	internal static class TourTurnPlan
	{
		private sealed class Plan
		{
			internal ICombatState State;

			internal Player Player;

			internal int Turn;

			internal TourCard[] Cards;

			internal CancellationToken Cancel;
		}

		private static Plan _pending;

		internal static void Arm(ICombatState state, Player player, IReadOnlyList<TourCard> cards, CancellationToken cancel)
		{
			try
			{
				_pending = ((cards.Count == 0) ? null : new Plan
				{
					State = state,
					Player = player,
					Turn = player.PlayerCombatState.TurnNumber + 1,
					Cards = cards.ToArray(),
					Cancel = cancel
				});
				if (_pending != null)
				{
					DevLog.Info($"Tour: next-draw plan armed for turn {_pending.Turn}: [{string.Join(", ", cards.Select((TourCard c) => c.Title))}]");
				}
			}
			catch (Exception ex)
			{
				_pending = null;
				DevLog.Warn("Tour: arm turn plan failed: " + ex);
				throw;
			}
		}

		internal static void Cancel()
		{
			try
			{
				if (_pending != null)
				{
					DevLog.Info("Tour: pending turn plan cancelled");
				}
				_pending = null;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: cancel turn plan failed: " + ex);
			}
		}

		internal static Task BeforeDrawCompleted(Task original, ICombatState state, Player player)
		{
			try
			{
				Plan pending = _pending;
				if (!TourController.Active || pending == null || pending.State != state || pending.Player != player || player.PlayerCombatState.TurnNumber < pending.Turn)
				{
					return original;
				}
				_pending = null;
				return Apply(original, pending);
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: draw hook failed: " + ex);
				return original;
			}
		}

		private static async Task Apply(Task original, Plan plan)
		{
			await original;
			try
			{
				TourController.Check(plan.Cancel, plan.State);
				if (plan.Player.PlayerCombatState.TurnNumber != plan.Turn)
				{
					throw new InvalidOperationException("下回合抽牌计划已过期");
				}
				CardPile draw = PileTypeExtensions.GetPile((PileType)1, plan.Player);
				List<CardModel> chosen = new List<CardModel>();
				TourCard[] cards = plan.Cards;
				foreach (TourCard requested in cards)
				{
					TourController.Check(plan.Cancel, plan.State);
					CardModel card = draw.Cards.FirstOrDefault((CardModel c) => requested.Matches(c) && !chosen.Contains(c));
					if (card == null)
					{
						card = TourScene.CreateCard(requested, plan.State, plan.Player);
						await CardPileCmd.Add(card, (PileType)1, (CardPilePosition)1, (AbstractModel)null, true);
					}
					chosen.Add(card);
				}
				TourController.Check(plan.Cancel, plan.State);
				if (PileTypeExtensions.GetPile((PileType)2, plan.Player).Cards.Count + chosen.Count > CardPile.MaxCardsInHand)
				{
					throw new InvalidOperationException("保留的手牌过多，无法保证下回合抽到指定牌");
				}
				for (int num = chosen.Count - 1; num >= 0; num--)
				{
					draw.MoveToTopInternal(chosen[num]);
				}
				DevLog.Info($"Tour: next-draw plan applied at turn {plan.Turn}: [{string.Join(", ", chosen.Select((CardModel c) => c.Title))}]");
			}
			catch (OperationCanceledException)
			{
				DevLog.Info("Tour: stale turn plan ignored");
			}
			catch (Exception ex2)
			{
				DevLog.Warn("Tour: next-draw plan failed: " + ex2);
				TourUi.Toast("下回合抽牌安排失败：" + ex2.Message);
			}
		}
	}
