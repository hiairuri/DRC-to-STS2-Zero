// namespace: SilentFX.Patches
// type: AttackFeedPatches (class)
// source lines: 19569-19790
	internal static class AttackFeedPatches
	{
		private static class ExecutePatch
		{
			private static void Prefix(AttackCommand __instance, out AttackFeed.ExecuteScope __state)
			{
				__state = null;
				try
				{
					__state = AttackFeed.Enter(__instance);
				}
				catch (Exception e)
				{
					AttackFeed.Warn("Execute prefix", e);
				}
			}

			private static void Postfix(Task<AttackCommand> __result, AttackFeed.ExecuteScope __state)
			{
				try
				{
					AttackFeed.Returned(__state, __result);
				}
				catch (Exception e)
				{
					AttackFeed.Warn("Execute postfix", e);
				}
			}

			private static void Finalizer(AttackFeed.ExecuteScope __state, Exception __exception)
			{
				try
				{
					AttackFeed.Leave(__state, __exception);
				}
				catch (Exception e)
				{
					AttackFeed.Warn("Execute finalizer", e);
				}
			}
		}

		private static class CountPatch
		{
			private static void Postfix(AttackCommand attackCommand, decimal __result)
			{
				try
				{
					AttackFeed.Begin(attackCommand, __result);
				}
				catch (Exception e)
				{
					AttackFeed.Warn("hit count", e);
				}
			}
		}

		private static class DamagePatch
		{
			private static void Prefix(Creature dealer, CardModel cardSource, out AttackFeed.DamageScope __state)
			{
				__state = null;
				try
				{
					__state = AttackFeed.EnterDamage(dealer, cardSource);
				}
				catch (Exception e)
				{
					AttackFeed.Warn("damage prefix", e);
				}
				try
				{
					if (cardSource is EchoingSlash)
					{
						EchoingSlashFx.BlowBegins(cardSource);
					}
				}
				catch (Exception e2)
				{
					AttackFeed.Warn("damage prefix (Echoing Slash)", e2);
				}
			}

			private static void Finalizer(AttackFeed.DamageScope __state)
			{
				try
				{
					AttackFeed.LeaveDamage(__state);
				}
				catch (Exception e)
				{
					AttackFeed.Warn("damage finalizer", e);
				}
			}
		}

		private static class LandedPatch
		{
			private static void Prefix(Creature dealer, DamageResult results, CardModel cardSource)
			{
				try
				{
					AttackFeed.DamageGiven(dealer, cardSource, results);
				}
				catch (Exception e)
				{
					AttackFeed.Warn("damage hook", e);
				}
			}
		}

		private static class EndPatch
		{
			private static void Prefix(AttackCommand command)
			{
				try
				{
					AttackFeed.AfterAttack(command);
				}
				catch (Exception e)
				{
					AttackFeed.Warn("attack end", e);
				}
			}
		}

		private static class ResetPatch
		{
			private static void Prefix()
			{
				try
				{
					AttackFeed.Reset();
				}
				catch (Exception e)
				{
					AttackFeed.Warn("reset", e);
				}
			}
		}

		private static bool _installed;

		internal static void Install(Harmony harmony)
		{
			if (_installed)
			{
				return;
			}
			_installed = true;
			int done = 0;
			int failed = 0;
			One("AttackCommand.Execute", () => AccessTools.Method(typeof(AttackCommand), "Execute", new Type[1] { typeof(PlayerChoiceContext) }, (Type[])null), typeof(ExecutePatch));
			One("Hook.ModifyAttackHitCount", () => AccessTools.Method(typeof(Hook), "ModifyAttackHitCount", (Type[])null, (Type[])null), typeof(CountPatch));
			One("CreatureCmd.Damage", FindDamageMethod, typeof(DamagePatch));
			One("Hook.AfterDamageGiven", () => AccessTools.Method(typeof(Hook), "AfterDamageGiven", (Type[])null, (Type[])null), typeof(LandedPatch));
			One("Hook.AfterAttack", () => AccessTools.Method(typeof(Hook), "AfterAttack", (Type[])null, (Type[])null), typeof(EndPatch));
			One("Hook.AfterCombatEnd", () => AccessTools.Method(typeof(Hook), "AfterCombatEnd", (Type[])null, (Type[])null), typeof(ResetPatch));
			One("NCombatRoom._ExitTree", () => AccessTools.Method(typeof(NCombatRoom), "_ExitTree", (Type[])null, (Type[])null), typeof(ResetPatch));
			bool flag = failed == 0 && Ticker.Tree != null;
			AttackFeed.Configure(flag);
			try
			{
				DevLog.Info($"AttackFeed: {done} patch(es) installed" + (flag ? "" : $", feed disabled ({failed} missing; ticker={Ticker.Tree != null})"));
			}
			catch (Exception e)
			{
				AttackFeed.Warn("installation log", e);
			}
			void One(string name, Func<MethodBase> target, Type patch)
			{
				try
				{
					MethodBase methodBase = target() ?? throw new MissingMethodException(name);
					harmony.Patch(methodBase, Part("Prefix"), Part("Postfix"), (HarmonyMethod)null, Part("Finalizer"));
					done++;
				}
				catch (Exception e2)
				{
					failed++;
					AttackFeed.Warn("patch not installed (" + name + ")", e2);
				}
				HarmonyMethod Part(string part)
				{
					//IL_0019: Unknown result type (might be due to invalid IL or missing references)
					//IL_001e: Unknown result type (might be due to invalid IL or missing references)
					//IL_003a: Expected Obj, but got Unknown
					MethodInfo methodInfo = AccessTools.DeclaredMethod(patch, part, (Type[])null, (Type[])null);
					if (!(methodInfo == null))
					{
						return new HarmonyMethod(methodInfo)
						{
							priority = ((part == "Prefix") ? 800 : 0)
						};
					}
					return null;
				}
			}
		}

		internal static MethodInfo FindDamageMethod()
		{
			return AccessTools.Method(typeof(CreatureCmd), "Damage", new Type[7]
			{
				typeof(PlayerChoiceContext),
				typeof(IEnumerable<Creature>),
				typeof(decimal),
				typeof(ValueProp),
				typeof(Creature),
				typeof(CardModel),
				typeof(CardPlay)
			}, (Type[])null) ?? AccessTools.Method(typeof(CreatureCmd), "Damage", new Type[6]
			{
				typeof(PlayerChoiceContext),
				typeof(IEnumerable<Creature>),
				typeof(decimal),
				typeof(ValueProp),
				typeof(Creature),
				typeof(CardModel)
			}, (Type[])null);
		}
	}
