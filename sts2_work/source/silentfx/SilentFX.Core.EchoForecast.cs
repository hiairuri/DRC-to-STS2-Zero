// namespace: SilentFX.Core
// type: EchoForecast (class)
// source lines: 34434-34538
	public static class EchoForecast
	{
		private struct Enemy
		{
			public Creature Target;

			public decimal Hp;

			public decimal Block;

			public decimal Damage;
		}

		public static int Hits(CardModel card, int cap = 6)
		{
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				cap = Math.Max(1, cap);
				Player obj = ((card != null) ? card.Owner : null);
				Creature val = ((obj != null) ? obj.Creature : null);
				ICombatState val2 = ((card != null) ? card.CombatState : null);
				IRunState val3 = ((obj != null) ? obj.RunState : null);
				if (cap == 1 || val == null || val2 == null || val3 == null)
				{
					return 1;
				}
				IReadOnlyList<Creature> hittableEnemies = val2.HittableEnemies;
				DamageVar damage = card.DynamicVars.Damage;
				if (hittableEnemies == null || damage == null)
				{
					return 1;
				}
				List<Enemy> list = new List<Enemy>();
				foreach (Creature item in hittableEnemies)
				{
					if (item == null)
					{
						return 1;
					}
					decimal num = item.CurrentHp;
					decimal num2 = item.Block;
					if (!(num <= 0m))
					{
						if (num2 < 0m)
						{
							return 1;
						}
						decimal num3 = RuntimeApiCompatibility.PreviewDamage(val3, val2, item, val, ((DynamicVar)damage).BaseValue, damage.Props, card);
						if (num3 < 0m)
						{
							return 1;
						}
						list.Add(new Enemy
						{
							Target = item,
							Hp = num,
							Block = num2,
							Damage = num3
						});
					}
				}
				if (list.Count == 0)
				{
					return 1;
				}
				int num4 = 1;
				int num5 = 0;
				IEnumerable<AbstractModel> enumerable = default;
				while (num4 > 0 && num5 < cap)
				{
					num4--;
					num5++;
					for (int i = 0; i < list.Count; i++)
					{
						Enemy value = list[i];
						if (!(value.Hp <= 0m))
						{
							decimal num6 = Math.Min(value.Block, value.Damage);
							decimal num7 = value.Damage - num6;
							decimal num8 = Hook.ModifyHpLost(val3, val2, value.Target, num7, damage.Props, val, card, (HpLossHookPhase)3, ref enumerable);
							if (num8 < 0m)
							{
								return 1;
							}
							value.Block -= num6;
							if (num8 >= value.Hp)
							{
								num4++;
							}
							value.Hp = Math.Max(0m, value.Hp - num8);
							list[i] = value;
						}
					}
				}
				return Math.Clamp(num5, 1, cap);
			}
			catch (Exception ex)
			{
				DevLog.Warn("EchoForecast.Hits: " + ex);
			}
			return 1;
		}
	}
