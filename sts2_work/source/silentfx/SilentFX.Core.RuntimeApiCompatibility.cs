// namespace: SilentFX.Core
// type: RuntimeApiCompatibility (class)
// source lines: 42118-42296
	internal static class RuntimeApiCompatibility
	{
		private delegate decimal OfficialDamage(IRunState run, ICombatState state, Creature target, Creature dealer, decimal damage, ValueProp props, CardModel card, ModifyDamageHookType hooks, CardPreviewMode preview, out IEnumerable<AbstractModel> modifiers);

		private delegate decimal BetaDamage(IRunState run, ICombatState state, Creature target, Creature dealer, decimal damage, ValueProp props, CardModel card, CardPlay play, ModifyDamageHookType hooks, CardPreviewMode preview, out IEnumerable<AbstractModel> modifiers);

		private delegate Task OfficialLoseBlock(Creature target, decimal amount);

		private delegate Task BetaLoseBlock(PlayerChoiceContext context, Creature target, decimal amount, Creature remover);

		private delegate IDisposable OfficialSelector(ICardSelector selector);

		private delegate IDisposable BetaSelector(ICardSelector selector, bool localOnly);

		internal static readonly MethodInfo DamageMethod = FindDamageMethod();

		internal static readonly MethodInfo LoseBlockMethod = Method(typeof(CreatureCmd), "LoseBlock", typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal), typeof(Creature)) ?? Method(typeof(CreatureCmd), "LoseBlock", typeof(Creature), typeof(decimal));

		internal static readonly MethodInfo SelectorMethod = Method(typeof(CardSelectCmd), "PushSelector", typeof(ICardSelector), typeof(bool)) ?? Method(typeof(CardSelectCmd), "PushSelector", typeof(ICardSelector));

		private static readonly OfficialDamage OfficialDamageCall;

		private static readonly BetaDamage BetaDamageCall;

		private static readonly OfficialLoseBlock OfficialLoseBlockCall;

		private static readonly BetaLoseBlock BetaLoseBlockCall;

		private static readonly OfficialSelector OfficialSelectorCall;

		private static readonly BetaSelector BetaSelectorCall;

		private static readonly PropertyInfo ConsoleVisible;

		private static readonly PropertyInfo InputType;

		private static readonly PropertyInfo DirectionalNavigation;

		private static readonly PropertyInfo UsingController;

		private static readonly Func<CardPlay, Player> PlayPlayer;

		internal static bool IsConsoleVisible
		{
			get
			{
				if (!(ConsoleVisible != null))
				{
					CanvasItem instance = (CanvasItem)(object)NDevConsole.Instance;
					if (instance != null)
					{
						return instance.Visible;
					}
					return false;
				}
				return (bool)ConsoleVisible.GetValue(null);
			}
		}

		private static MethodInfo Method(Type type, string name, params Type[] parameters)
		{
			return type.GetMethod(name, BindingFlags.Static | BindingFlags.Public, null, parameters, null);
		}

		internal static Player PlayerOf(CardPlay play)
		{
			if (play != null)
			{
				if (PlayPlayer == null)
				{
					CardModel card = play.Card;
					if (card == null)
					{
						return null;
					}
					return card.Owner;
				}
				return PlayPlayer(play);
			}
			return null;
		}

		private static MethodInfo FindDamageMethod()
		{
			Type[] collection = new Type[7]
			{
				typeof(IRunState),
				typeof(ICombatState),
				typeof(Creature),
				typeof(Creature),
				typeof(decimal),
				typeof(ValueProp),
				typeof(CardModel)
			};
			Type[] collection2 = new Type[3]
			{
				typeof(ModifyDamageHookType),
				typeof(CardPreviewMode),
				typeof(IEnumerable<AbstractModel>).MakeByRefType()
			};
			List<Type> list = new List<Type>(collection) { typeof(CardPlay) };
			list.AddRange(collection2);
			List<Type> list2 = new List<Type>(collection);
			list2.AddRange(collection2);
			return Method(typeof(Hook), "ModifyDamage", list.ToArray()) ?? Method(typeof(Hook), "ModifyDamage", list2.ToArray());
		}

		internal static decimal PreviewDamage(IRunState run, ICombatState state, Creature target, Creature dealer, decimal damage, ValueProp props, CardModel card)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			IEnumerable<AbstractModel> modifiers;
			if (BetaDamageCall != null)
			{
				return BetaDamageCall(run, state, target, dealer, damage, props, card, null, (ModifyDamageHookType)14, (CardPreviewMode)1, out modifiers);
			}
			if (OfficialDamageCall != null)
			{
				return OfficialDamageCall(run, state, target, dealer, damage, props, card, (ModifyDamageHookType)14, (CardPreviewMode)1, out modifiers);
			}
			throw new MissingMethodException("No supported Hook.ModifyDamage preview signature.");
		}

		internal static Task LoseBlock(PlayerChoiceContext context, Creature target, decimal amount, Creature remover = null)
		{
			if (BetaLoseBlockCall != null)
			{
				return BetaLoseBlockCall(context, target, amount, remover);
			}
			if (OfficialLoseBlockCall != null)
			{
				return OfficialLoseBlockCall(target, amount);
			}
			throw new MissingMethodException("No supported CreatureCmd.LoseBlock signature.");
		}

		internal static IDisposable PushSelector(ICardSelector selector, bool localOnly = false)
		{
			if (BetaSelectorCall != null)
			{
				return BetaSelectorCall(selector, localOnly);
			}
			if (OfficialSelectorCall != null)
			{
				return OfficialSelectorCall(selector);
			}
			throw new MissingMethodException("No supported CardSelectCmd.PushSelector signature.");
		}

		internal static string DescribeInput(NControllerManager controller)
		{
			if (InputType != null && DirectionalNavigation != null)
			{
				return $"Input type: {InputType.GetValue(controller)}, directional navigation: {DirectionalNavigation.GetValue(controller)}";
			}
			return $"Using controller: {UsingController?.GetValue(controller) ?? "unavailable"}";
		}

		static RuntimeApiCompatibility()
		{
			MethodInfo damageMethod = DamageMethod;
			OfficialDamageCall = (((object)damageMethod != null && damageMethod.GetParameters().Length == 10) ? DamageMethod.CreateDelegate<OfficialDamage>() : null);
			MethodInfo damageMethod2 = DamageMethod;
			BetaDamageCall = (((object)damageMethod2 != null && damageMethod2.GetParameters().Length == 11) ? DamageMethod.CreateDelegate<BetaDamage>() : null);
			MethodInfo loseBlockMethod = LoseBlockMethod;
			OfficialLoseBlockCall = (((object)loseBlockMethod != null && loseBlockMethod.GetParameters().Length == 2) ? LoseBlockMethod.CreateDelegate<OfficialLoseBlock>() : null);
			MethodInfo loseBlockMethod2 = LoseBlockMethod;
			BetaLoseBlockCall = (((object)loseBlockMethod2 != null && loseBlockMethod2.GetParameters().Length == 4) ? LoseBlockMethod.CreateDelegate<BetaLoseBlock>() : null);
			MethodInfo selectorMethod = SelectorMethod;
			OfficialSelectorCall = (((object)selectorMethod != null && selectorMethod.GetParameters().Length == 1) ? SelectorMethod.CreateDelegate<OfficialSelector>() : null);
			MethodInfo selectorMethod2 = SelectorMethod;
			BetaSelectorCall = (((object)selectorMethod2 != null && selectorMethod2.GetParameters().Length == 2) ? SelectorMethod.CreateDelegate<BetaSelector>() : null);
			ConsoleVisible = typeof(NDevConsole).GetProperty("IsConsoleVisible", BindingFlags.Static | BindingFlags.Public);
			InputType = typeof(NControllerManager).GetProperty("InputType");
			DirectionalNavigation = typeof(NControllerManager).GetProperty("IsUsingDirectionalNavigation");
			UsingController = typeof(NControllerManager).GetProperty("IsUsingController");
			PlayPlayer = typeof(CardPlay).GetProperty("Player")?.GetMethod?.CreateDelegate<Func<CardPlay, Player>>();
		}
	}
