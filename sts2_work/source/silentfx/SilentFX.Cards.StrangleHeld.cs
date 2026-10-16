// namespace: SilentFX.Cards
// type: StrangleHeld (class)
// source lines: 106635-106791
	public sealed class StrangleHeld : HeldSession
	{
		private bool _played;

		private Creature _waitingFor;

		private CardModel _waitingCard;

		private void CancelWaiting()
		{
			StrangleRope.ReleaseHang(_waitingFor, _waitingCard, this);
			_waitingFor = null;
			_waitingCard = null;
		}

		private void HangFor(CardFxContext c, Creature who, Vector2 at, float width, float height)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			if (MultiplayerFx.Active)
			{
				if (_waitingFor != who || _waitingCard != c.Card)
				{
					CancelWaiting();
				}
				_waitingFor = who;
				_waitingCard = c.Card;
			}
			StrangleRope.Hang(c.BackParent, c.FrontParent, at, width, height, who, c.Card, this);
		}

		protected override void OnLeaving()
		{
			CancelWaiting();
		}

		protected override void OnEnd()
		{
			CancelWaiting();
		}

		protected override void Build()
		{
		}

		protected override void Tick(float dt)
		{
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			if (_played || Leaving)
			{
				return;
			}
			CardFxContext c = C;
			if (c == null)
			{
				return;
			}
			if (c.Remote)
			{
				if (MultiplayerFx.Active && c.HasTarget && c.Target != null)
				{
					HangFor(c, c.Target, c.TargetChest, c.TargetWidth, c.TargetHeight);
				}
				else
				{
					CancelWaiting();
				}
				return;
			}
			bool flag = Released || c.HasTarget;
			if (!flag && c.Card != null && HeldFxManager.Carried == c.Card)
			{
				flag = HeldFxManager.CarriedOut;
			}
			if (!flag)
			{
				if (MultiplayerFx.Active)
				{
					CancelWaiting();
				}
				return;
			}
			if (c.HasTarget)
			{
				HangFor(c, c.Target, c.TargetChest, c.TargetWidth, c.TargetHeight);
				return;
			}
			List<FxTarget> enemies = c.Enemies;
			if (enemies == null || enemies.Count == 0)
			{
				return;
			}
			int num = -1;
			Vector2 val = Vector2.Zero;
			bool flag2 = false;
			try
			{
				val = ((CanvasItem)Front).GetGlobalMousePosition();
				flag2 = true;
			}
			catch
			{
				flag2 = false;
			}
			float num2 = float.MaxValue;
			for (int i = 0; i < enemies.Count; i++)
			{
				FxTarget fxTarget = enemies[i];
				if (fxTarget == null)
				{
					continue;
				}
				object key = fxTarget.Key;
				Creature val2 = (Creature)((key is Creature) ? key : null);
				if (val2 == null || val2.IsAlive)
				{
					float num3;
					if (!flag2)
					{
						num3 = 0f;
					}
					else
					{
						Vector2 val3 = fxTarget.Chest - val;
						num3 = val3.LengthSquared();
					}
					float num4 = num3;
					if (num < 0 || !(num4 >= num2))
					{
						num2 = num4;
						num = i;
					}
				}
			}
			if (num >= 0)
			{
				FxTarget fxTarget2 = enemies[num];
				object key2 = fxTarget2.Key;
				HangFor(c, (Creature)((key2 is Creature) ? key2 : null), fxTarget2.Chest, fxTarget2.Width, fxTarget2.Height);
			}
		}

		protected override void OnPlay(CardFxContext play)
		{
			_played = true;
			Leave(16f);
		}
	}
