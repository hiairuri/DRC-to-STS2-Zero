// namespace: SilentFX.Cards
// type: ShivFx (class)
// source lines: 97013-99117
	public static class ShivFx
	{
		private sealed class Frame
		{
			public Type Card;

			public CardModel Model;

			public Creature Target;

			public int Made;

			public int Shown;
		}

		private static readonly Color Steel = new Color(0.75f, 0.92f, 1f, 1f);

		private static readonly Color Gold = new Color(1f, 0.85f, 0.42f, 1f);

		private static readonly Color InkGlow = new Color(0.55f, 0.34f, 0.95f, 1f);

		private static readonly Dictionary<Type, ShivStyle> Styles = new Dictionary<Type, ShivStyle>();

		private static readonly List<Frame> Stack = new List<Frame>();

		private static int _shivPlaysThisTurn;

		private static readonly Dictionary<Player, int> PlayerShivPlays = new Dictionary<Player, int>(ReferenceEqualityComparer.Instance);

		private const float CloakBulge = 90f;

		private const float CloakWidth = 70f;

		public static bool FirstShivPending => _shivPlaysThisTurn == 0;

		internal static bool FirstShivPendingFor(Player owner)
		{
			if (MultiplayerFx.Active)
			{
				if (owner != null)
				{
					if (PlayerShivPlays.TryGetValue(owner, out var value))
					{
						return value == 0;
					}
					return true;
				}
				return false;
			}
			return FirstShivPending;
		}

		public static void Init()
		{
			Styles[typeof(BladeDance)] = ShivStyle.Dance;
			Styles[typeof(CloakAndDagger)] = ShivStyle.Cloak;
			Styles[typeof(HiddenDaggers)] = ShivStyle.Sleeves;
			Styles[typeof(UpMySleeve)] = ShivStyle.Fan;
			Type type = GameCapabilities.Card("BladeSymphony");
			if (type != null)
			{
				Styles[type] = ShivStyle.Symphony;
			}
			Styles[typeof(LeadingStrike)] = ShivStyle.Return;
			Styles[typeof(BladeOfInk)] = ShivStyle.Ink;
			Styles[typeof(StormOfSteel)] = ShivStyle.None;
		}

		private static bool IsLocal(CardModel card)
		{
			try
			{
				return LocalContext.IsMine(card);
			}
			catch
			{
				return true;
			}
		}

		private static bool IsLocal(Player player)
		{
			try
			{
				return LocalContext.IsMe(player);
			}
			catch
			{
				return true;
			}
		}

		public static void NoteCardPlayed(CardPlay play)
		{
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			CardModel val = ((play != null) ? play.Card : null);
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			using (RemoteFxFade.Capture(MultiplayerFx.Active && val != null && !MultiplayerFx.IsLocal(val), (Node)((combatRoom != null) ? combatRoom.BackCombatVfxContainer : null), (Node)((combatRoom != null) ? combatRoom.CombatVfxContainer : null), GameRefs.Overlay, (Node)combatRoom))
			{
				if (MultiplayerFx.Active && val is Shiv && val.Owner != null && (MultiplayerFx.IsLocal(val) || RemoteShivOwner(val)))
				{
					PlayerShivPlays.TryGetValue(val.Owner, out var value);
					PlayerShivPlays[val.Owner] = value + 1;
				}
				if (MultiplayerFx.Active && val is Shiv)
				{
					KnifeTrapFx.OnShivPlayed();
				}
				if (MultiplayerFx.Active && IsCreationCard(val) && MultiplayerFx.CurrentCard == val)
				{
					RemoteShivCreation.Begin(MultiplayerFx.CurrentInvocation, play, GameRefs.CombatRoom);
				}
				bool flag = MultiplayerFx.Active && val is Shiv && !MultiplayerFx.IsLocal(val) && RemoteShivOwner(val) && ShivFan.Begin(play);
				if (MultiplayerFx.Active && val is Shiv && !MultiplayerFx.IsLocal(val) && !flag && RemoteShivParticipants(val, play.Target) && !KnifeTrapFx.OwnsShiv(val))
				{
					Vector2? target = null;
					if (play.Target != null)
					{
						NCombatRoom combatRoom2 = GameRefs.CombatRoom;
						NCreature val2 = ((combatRoom2 != null) ? combatRoom2.GetCreatureNode(play.Target) : null);
						if (FxKit.Alive((GodotObject)(object)val2) && ((Node)val2).IsInsideTree())
						{
							target = val2.VfxSpawnPosition;
						}
					}
					if (target.HasValue)
					{
						PhantomBladesFx.OnShivPlayed(val, target);
					}
				}
				if (val == null || !(MultiplayerFx.Active ? MultiplayerFx.IsLocal(val) : IsLocal(val)))
				{
					return;
				}
				if (Stack.Count > 16)
				{
					Stack.Clear();
				}
				Stack.Add(new Frame
				{
					Card = ((object)val).GetType(),
					Model = val,
					Target = play.Target
				});
				if (!(val is Shiv))
				{
					return;
				}
				_shivPlaysThisTurn++;
				if (!MultiplayerFx.Active)
				{
					KnifeTrapFx.OnShivPlayed();
				}
				if (ShivFan.Begin(play))
				{
					return;
				}
				Vector2? target2 = null;
				if (play.Target != null)
				{
					NCombatRoom combatRoom3 = GameRefs.CombatRoom;
					NCreature val3 = ((combatRoom3 != null) ? combatRoom3.GetCreatureNode(play.Target) : null);
					if (FxKit.Alive((GodotObject)(object)val3) && ((Node)val3).IsInsideTree())
					{
						target2 = val3.VfxSpawnPosition;
					}
				}
				PhantomBladesFx.OnShivPlayed(val, target2);
			}
		}

		public static void NoteCardFinished(CardPlay play)
		{
			RemoteShivCreation.Finish(MultiplayerFx.CurrentInvocation, play);
			if (MultiplayerFx.Active)
			{
				if (((play != null) ? play.Card : null) is KnifeTrap)
				{
					KnifeTrapFx.Close(play.Card);
				}
				else if (((play != null) ? play.Card : null) is Shiv)
				{
					KnifeTrapFx.OnShivFinished();
				}
				if (((play != null) ? play.Card : null) is Shiv)
				{
					ShivFan.Finish(play.Card);
				}
				if (!MultiplayerFx.IsLocal((play != null) ? play.Card : null))
				{
					return;
				}
			}
			Type type = ((play == null) ? null : ((object)play.Card)?.GetType());
			if (type == null)
			{
				return;
			}
			if (!MultiplayerFx.Active)
			{
				if (type == typeof(KnifeTrap))
				{
					KnifeTrapFx.Close();
				}
				else if (type == typeof(Shiv))
				{
					KnifeTrapFx.OnShivFinished();
					ShivFan.Finish(play.Card);
				}
			}
			for (int num = Stack.Count - 1; num >= 0; num--)
			{
				if (Stack[num].Model == play.Card)
				{
					Stack.RemoveRange(num, Stack.Count - num);
					break;
				}
			}
		}

		public static void OnPlayerTurnStart()
		{
			RemoteShivCreation.Reset();
			PlayerShivPlays.Clear();
			_shivPlaysThisTurn = 0;
			Stack.Clear();
			KnifeTrapFx.Close();
			ShivFan.Reset();
		}

		public static void Reset()
		{
			RemoteShivCreation.Reset();
			PlayerShivPlays.Clear();
			_shivPlaysThisTurn = 0;
			Stack.Clear();
			KnifeTrapFx.Abort();
			PhantomBladesFx.Reset();
			ShivFan.Reset();
		}

		public static CardModel CurrentShiv()
		{
			if (MultiplayerFx.Active)
			{
				if (!(MultiplayerFx.CurrentCard is Shiv))
				{
					return null;
				}
				return MultiplayerFx.CurrentCard;
			}
			if (MultiplayerFx.CurrentInvocation != null)
			{
				return null;
			}
			if (Stack.Count == 0)
			{
				return null;
			}
			Frame frame = Stack[Stack.Count - 1];
			if (!(frame.Card == typeof(Shiv)))
			{
				return null;
			}
			return frame.Model;
		}

		public static void OnShivsCreated(Player owner, int count)
		{
			if (MultiplayerFx.Active && owner != null && (!MultiplayerFx.IsLocal(owner.Creature) || (IsCreationCard(MultiplayerFx.CurrentCard) && !MultiplayerFx.IsLocal(MultiplayerFx.CurrentCard))))
			{
				if (PlayerSettings.Enabled)
				{
					OnRemoteShivsCreated(owner, count);
				}
			}
			else
			{
				if (owner == null || count <= 0 || !IsLocal(owner))
				{
					return;
				}
				Creature creature = owner.Creature;
				if (creature == null)
				{
					return;
				}
				ShivStyle value = ShivStyle.Plain;
				Creature target = null;
				int num = 0;
				IShivSource shivSource = null;
				Frame frame = null;
				if (Stack.Count > 0)
				{
					Frame frame2 = Stack[Stack.Count - 1];
					frame = frame2;
					if (!Styles.TryGetValue(frame2.Card, out value))
					{
						value = ShivStyle.Plain;
					}
					target = frame2.Target;
					num = frame2.Made;
					frame2.Made += count;
					shivSource = HeldFxManager.PlayingFor(frame2.Model) as IShivSource;
				}
				else if (creature.HasPower<InfiniteBladesPower>())
				{
					return;
				}
				if ((value == ShivStyle.None && shivSource == null) || !PlayerSettings.Enabled)
				{
					return;
				}
				CardFxContext cardFxContext = CardFxManager.BuildContext(creature, target);
				if (cardFxContext == null)
				{
					return;
				}
				if (value == ShivStyle.Cloak && frame != null)
				{
					int num2 = ((num == 0) ? Mathf.Clamp(Mathf.Max(count, CountFor(frame.Model, count)), 1, 4) : (num + count));
					int shown = frame.Shown;
					if (num2 > shown)
					{
						frame.Shown = num2;
						if (PowerFxManager.LogTriggers)
						{
							DevLog.Info($"Shiv FX: {count} into the hand (Cloak, from #{num}): {num2 - shown} dagger(s) out of the cloak{((shivSource != null) ? ", from standby" : "")}");
						}
						cardFxContext.Held = shivSource as ShivHeld;
						PlayCreate(cardFxContext, ShivStyle.Cloak, num2 - shown, shown);
					}
				}
				else
				{
					int num3 = ((shivSource != null) ? TakeHeld(cardFxContext, shivSource, value, count, num) : 0);
					if (PowerFxManager.LogTriggers)
					{
						DevLog.Info($"Shiv FX: {count} into the hand ({value}, from #{num}{((num3 > 0) ? $", {num3} from standby" : "")})");
					}
					if (num3 < count && value != ShivStyle.None)
					{
						PlayCreate(cardFxContext, value, count - num3, num + num3);
					}
				}
			}
		}

		private static bool IsCreationCard(CardModel card)
		{
			if ((!(card is BladeDance) && !(card is CloakAndDagger) && !(card is HiddenDaggers) && !(card is UpMySleeve) && !(card is BladeOfInk) && !(card is StormOfSteel) && !(card is LeadingStrike)) || 1 == 0)
			{
				return GameCapabilities.IsBladeSymphony(card);
			}
			return true;
		}

		private static void OnRemoteShivsCreated(Player owner, int count)
		{
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			if (count <= 0 || owner.Creature == null || !FxKit.Alive((GodotObject)(object)GameRefs.CombatRoom))
			{
				return;
			}
			CombatManager instance = CombatManager.Instance;
			if (instance == null || instance.IsOverOrEnding)
			{
				return;
			}
			CardModel currentCard = MultiplayerFx.CurrentCard;
			if (!IsCreationCard(currentCard) || !RemoteShivCreation.TryGet(MultiplayerFx.CurrentInvocation, currentCard, GameRefs.CombatRoom, out var frame))
			{
				return;
			}
			Player owner2 = currentCard.Owner;
			Creature val = ((owner2 != null) ? owner2.Creature : null);
			CombatManager instance2 = CombatManager.Instance;
			CombatState val2 = ((instance2 != null) ? instance2.DebugOnlyGetState() : null);
			if (val == null || !val.IsAlive || !owner.Creature.IsAlive || val2 == null || (object)val.CombatState != val2 || (object)owner.Creature.CombatState != val2)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			foreach (Player player in val2.Players)
			{
				if (player == frame.Owner && player.Creature == val)
				{
					flag = true;
				}
				if (player == owner && player.Creature == owner.Creature)
				{
					flag2 = true;
				}
			}
			if (!flag || !flag2)
			{
				return;
			}
			if (frame.Owner != owner)
			{
				if (!GameCapabilities.IsBladeSymphony(currentCard) || val.CombatState == null || val.CombatState != owner.Creature.CombatState)
				{
					return;
				}
				bool flag3 = false;
				foreach (Creature item in val.CombatState.GetTeammatesOf(val))
				{
					if (item != null && item.IsAlive && item.Player == owner)
					{
						flag3 = true;
						break;
					}
				}
				if (!flag3)
				{
					return;
				}
			}
			if (!Styles.TryGetValue(((object)currentCard).GetType(), out var value))
			{
				return;
			}
			RemoteShivCreation.Batch batch = frame.For(owner);
			int num = batch.Add(count);
			IShivSource shivSource = ((!MultiplayerFx.IsLocal(currentCard)) ? (HeldFxManager.PlayingFor(currentCard) as IShivSource) : null);
			if (value == ShivStyle.None && shivSource == null)
			{
				return;
			}
			CardFxContext cardFxContext = CardFxManager.BuildContext(val, frame.Target);
			if (cardFxContext == null)
			{
				return;
			}
			cardFxContext.Card = currentCard;
			cardFxContext.Remote = true;
			cardFxContext.UiParent = null;
			using (RemoteFxFade.Capture(!MultiplayerFx.IsLocal(currentCard), cardFxContext.BackParent, cardFxContext.FrontParent, GameRefs.Overlay, (Node)GameRefs.CombatRoom))
			{
				if (frame.Owner != owner)
				{
					CardFxContext cardFxContext2 = CardFxManager.BuildContext(owner.Creature, null);
					if (cardFxContext2 == null)
					{
						return;
					}
					cardFxContext.ShivReturnPoint = cardFxContext2.WeaponHand;
				}
				if (value == ShivStyle.Cloak)
				{
					int num2 = batch.ShowCloak(count, num, CountFor(currentCard, count), out var slot);
					if (num2 > 0)
					{
						cardFxContext.Held = shivSource as ShivHeld;
						PlayCreate(cardFxContext, value, num2, slot);
					}
				}
				else
				{
					int num3 = ((shivSource != null) ? TakeHeld(cardFxContext, shivSource, value, count, num) : 0);
					if (num3 < count && value != ShivStyle.None)
					{
						PlayCreate(cardFxContext, value, count - num3, num + num3);
					}
				}
			}
		}

		public static bool AdjustVanillaThrow(ref Vector2 thrower, ref Color tint)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			if (!PlayerSettings.Enabled)
			{
				return false;
			}
			if (MultiplayerFx.Active && ShivFan.HidesThrow(CurrentShiv()))
			{
				if (ShivFan.FromTrap(CurrentShiv()))
				{
					tint = KnifeTrapFx.Ember;
				}
				else
				{
					CardModel val = CurrentShiv();
					if (((val != null) ? val.Enchantment : null) is Inky)
					{
						tint = new Color(0.66f, 0.42f, 1f, 1f);
					}
				}
				return true;
			}
			if (!MultiplayerFx.Active && MultiplayerFx.CurrentInvocation != null)
			{
				return false;
			}
			if (MultiplayerFx.Active && KnifeTrapFx.TryThrow(out var from))
			{
				thrower = from;
				tint = KnifeTrapFx.Ember;
				return true;
			}
			if (MultiplayerFx.Active)
			{
				CardModel currentCard = MultiplayerFx.CurrentCard;
				Shiv val2 = (Shiv)(object)((currentCard is Shiv) ? currentCard : null);
				if (val2 != null && !MultiplayerFx.IsLocal((CardModel)(object)val2) && RemoteShivOwner((CardModel)(object)val2) && ((CardModel)val2).Enchantment is Inky)
				{
					tint = new Color(0.66f, 0.42f, 1f, 1f);
					return false;
				}
			}
			if (Stack.Count == 0)
			{
				return false;
			}
			Frame frame = Stack[Stack.Count - 1];
			if (frame.Card != typeof(Shiv))
			{
				return false;
			}
			if (MultiplayerFx.Active && frame.Model != MultiplayerFx.CurrentCard)
			{
				return false;
			}
			if (KnifeTrapFx.TryThrow(out var from2))
			{
				thrower = from2;
				tint = KnifeTrapFx.Ember;
				return true;
			}
			bool flag = ShivFan.HidesThrow(frame.Model);
			if (flag && ShivFan.FromTrap(frame.Model))
			{
				tint = KnifeTrapFx.Ember;
				return true;
			}
			CardModel model = frame.Model;
			if (((model != null) ? model.Enchantment : null) is Inky)
			{
				tint = new Color(0.66f, 0.42f, 1f, 1f);
			}
			return flag;
		}

		public static int CountFor(CardModel card, int fallback)
		{
			try
			{
				DynamicVarSet val = ((card != null) ? card.DynamicVars : null);
				if (val == null)
				{
					return fallback;
				}
				DynamicVar val2 = default;
				if (val.TryGetValue("Shivs", ref val2) && val2 != null)
				{
					return val2.IntValue;
				}
				DynamicVar val3 = default;
				if (val.TryGetValue("Cards", ref val3) && val3 != null)
				{
					return val3.IntValue;
				}
			}
			catch
			{
			}
			return fallback;
		}

		public static void PreviewTake(CardFxContext c, IShivSource held, ShivStyle style, int count)
		{
			int num = TakeHeld(c, held, style, count, 0);
			if (num < count && style != ShivStyle.None && style != ShivStyle.Storm)
			{
				PlayCreate(c, style, count - num, num);
			}
		}

		private static int TakeHeld(CardFxContext c, IShivSource held, ShivStyle style, int count, int start)
		{
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			Node frontParent = c.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)frontParent))
			{
				return 0;
			}
			int num = 0;
			bool ink = style == ShivStyle.Ink;
			for (int i = 0; i < count; i++)
			{
				if (!held.TryTake(out var blade))
				{
					break;
				}
				FlyToHand(c, start + i, blade, (float)i * 0.045f, ink);
				num++;
			}
			if (num == 0 || start != 0)
			{
				return num;
			}
			switch (style)
			{
			case ShivStyle.Dance:
				RingPulse(frontParent, c.Feet, c.Body.X * 1.7f, Steel, 0f, 0.45f, 0.5f, 1.3f, 0.3f, 9f);
				break;
			case ShivStyle.Sleeves:
				Glint(frontParent, c.Chest + new Vector2(c.Body.X * 0.2f, c.Body.Y * 0.14f), new Color(1f, 1f, 1f, 0.8f), 80f);
				Glint(frontParent, c.Chest + new Vector2((0f - c.Body.X) * 0.2f, c.Body.Y * 0.14f), new Color(1f, 1f, 1f, 0.8f), 80f);
				break;
			case ShivStyle.Fan:
				Glint(frontParent, WeaponHand(c) + new Vector2(c.Body.X * 0.3f * c.Facing, -6f), new Color(Steel.R, Steel.G, Steel.B, 0.7f), 90f);
				break;
			case ShivStyle.Symphony:
				RingPulse(frontParent, c.Chest + new Vector2(0f, (0f - c.Body.Y) * 0.6f), 220f, new Color(1f, 0.92f, 0.7f, 1f), 0f, 0.5f, 0.3f, 1.8f);
				break;
			case ShivStyle.Ink:
				InkSplat(frontParent, WeaponHand(c) + new Vector2(0f, -50f), 0.8f);
				break;
			}
			return num;
		}

		private static void FlyToHand(CardFxContext c, int slot, Node2D blade, float delay, bool ink)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Expected Obj, but got Unknown
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			Node frontParent = c.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)frontParent) || !FxKit.Alive((GodotObject)(object)blade))
			{
				return;
			}
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_ShivToHand")
			};
			frontParent.AddChild((Node)(object)holder, false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)holder, delay + Mathf.Max(0.01f, Tuning.F("shiv.fly_time", 0.36f)) + 0.3f);
			Line2D halo = null;
			Line2D ribbon;
			if (ink)
			{
				halo = FxKit.Ribbon(20f, new Color(InkGlow.R, InkGlow.G, InkGlow.B, 0.55f));
				((Node)holder).AddChild((Node)(object)halo, false, (InternalMode)0);
				ribbon = FxKit.Ribbon(9f, new Color(0.16f, 0.07f, 0.32f, 0.9f));
				((CanvasItem)ribbon).Material = null;
			}
			else
			{
				ribbon = FxKit.Ribbon(9f, new Color(Steel.R, Steel.G, Steel.B, 0.7f));
			}
			((Node)holder).AddChild((Node)(object)ribbon, false, (InternalMode)0);
			Vector2 launch = blade.GlobalPosition;
			float startRot = blade.GlobalRotation;
			Node parent = ((Node)blade).GetParent();
			if (parent != null)
			{
				parent.RemoveChild((Node)(object)blade);
			}
			((Node)holder).AddChild((Node)(object)blade, false, (InternalMode)0);
			blade.GlobalPosition = launch;
			blade.GlobalRotation = startRot;
			Vector2 dest = ((!c.Remote) ? (c.Hand + new Vector2(Spread(slot) * 34f, 0f)) : (c.ShivReturnPoint ?? c.WeaponHand));
			Vector2 mid = new Vector2(Mathf.Lerp(launch.X, dest.X, 0.35f), Mathf.Min(launch.Y, dest.Y) - 70f);
			float num = Tuning.F("shiv.fly_time", 0.36f);
			Vector2 last = launch;
			Action<float> action = (float t) =>
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_003d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_0098: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)holder) && FxKit.Alive((GodotObject)(object)blade))
				{
					Vector2 val2 = FxKit.Bezier(launch, mid, dest, t * t);
					Vector2 val3 = val2 - last;
					if (val3.LengthSquared() > 0.01f)
					{
						blade.Rotation = Mathf.LerpAngle(startRot, val3.Angle() + (float)Math.PI / 2f, Mathf.Min(1f, t * 5f));
					}
					blade.GlobalPosition = val2;
					Vector2 val4 = holder.ToLocal(val2);
					ribbon.AddPoint(val4, -1);
					if (FxKit.Alive((GodotObject)(object)halo))
					{
						halo.AddPoint(val4, -1);
					}
					last = val2;
				}
			};
			Tween val = ((Node)holder).CreateTween();
			val.TweenMethod(Callable.From<float>(action), Variant.op_Implicit(0f), Variant.op_Implicit(1f), (double)num).SetDelay((double)delay);
			val.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.16);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
		}

		public static void OnShivHit(CardModel shiv, Creature target)
		{
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			if (!PlayerSettings.Enabled)
			{
				return;
			}
			bool flag = (MultiplayerFx.Active ? MultiplayerFx.IsLocal(shiv) : IsLocal(shiv));
			bool flag2 = MultiplayerFx.Active && !flag && KnifeTrapFx.OwnsShiv(shiv);
			bool flag3 = MultiplayerFx.Active && !MultiplayerFx.IsLocal(shiv) && RemoteShivParticipants(shiv, target);
			if (shiv == null || target == null || (!flag && !flag2 && !flag3))
			{
				return;
			}
			Player owner = shiv.Owner;
			Creature caster = ((owner != null) ? owner.Creature : null);
			if (caster == null)
			{
				return;
			}
			CardFxContext c = CardFxManager.BuildContext(caster, target);
			if (c == null || !c.HasTarget)
			{
				return;
			}
			c.Card = shiv;
			c.Remote = flag3 | flag2;
			if (c.Remote)
			{
				c.UiParent = null;
			}
			bool accuracy = caster.HasPower<AccuracyPower>();
			int num = ((!MultiplayerFx.Active) ? _shivPlaysThisTurn : (PlayerShivPlays.TryGetValue(shiv.Owner, out var value) ? value : 0));
			bool phantom = num <= 1 && caster.HasPower<PhantomBladesPower>();
			bool ink = shiv.Enchantment is Inky;
			if (ShivFan.HitOf(shiv, target, out var fanFrom, out var fanTrap, out var wait))
			{
				bool veil = !phantom || ShivFan.TakeVeil(shiv);
				bool own = fanTrap && ShivFan.Bursts(shiv);
				Node frontParent = c.FrontParent;
				Action action = () =>
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					PlayHit(c, c.TargetChest, accuracy, phantom, ink, fanFrom, fanTrap, veil, own);
					if (accuracy)
					{
						PowerFxManager.TriggerFor<AccuracyPower>(caster, target);
					}
					if (phantom)
					{
						PowerFxManager.TriggerFor<PhantomBladesPower>(caster, target);
					}
				};
				if (wait > 0.01f && FxKit.Alive((GodotObject)(object)frontParent))
				{
					FxKit.After(frontParent, wait, action);
				}
				else
				{
					action();
				}
				return;
			}
			Vector2? origin = (KnifeTrapFx.TryGetOrigin(out var from) ? new Vector2?(from) : ((Vector2?)null));
			bool hasValue = origin.HasValue;
			if (flag3 && !flag2 && !hasValue)
			{
				PhantomBladesFx.OnShivPlayed(shiv, c.TargetChest);
			}
			if (!flag2 && !hasValue && PhantomBladesFx.TakeThrowOrigin(shiv, out var from2))
			{
				origin = from2;
			}
			PlayHit(c, c.TargetChest, accuracy, phantom, ink, origin, hasValue, veil: true, hasValue);
			if (accuracy)
			{
				PowerFxManager.TriggerFor<AccuracyPower>(caster, target);
			}
			if (phantom)
			{
				PowerFxManager.TriggerFor<PhantomBladesPower>(caster, target);
			}
		}

		private static Vector2 WeaponHand(CardFxContext c)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return c.WeaponHand;
		}

		public static Node2D MakeShiv(bool ink)
		{
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			float num = Tuning.F("shiv.length", 56f);
			float halfWidth = num * 0.15f;
			if (!ink)
			{
				return FxKit.Blade(num, halfWidth, new Color(0.94f, 0.97f, 1f, 1f), new Color(0.6f, 0.67f, 0.78f, 1f), new Color(0.34f, 0.27f, 0.22f, 1f), new Color(0.55f, 0.85f, 1f, 0.32f));
			}
			return FxKit.Blade(num, halfWidth, new Color(0.5f, 0.38f, 0.86f, 1f), new Color(0.14f, 0.1f, 0.28f, 1f), new Color(0.1f, 0.08f, 0.14f, 1f), new Color(InkGlow.R, InkGlow.G, InkGlow.B, 0.55f));
		}

		private static float Spread(int index)
		{
			if (index <= 0)
			{
				return 0f;
			}
			int num = (index + 1) / 2;
			return (index % 2 == 1) ? num : (-num);
		}

		private static float EaseOut(float t)
		{
			float num = 1f - Mathf.Clamp(t, 0f, 1f);
			return 1f - num * num;
		}

		private static void Launch(CardFxContext c, int slot, Func<float, Vector2> pre, float preTime, float delay, bool spin, bool ink, float flyTime = -1f, bool flowing = false)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Expected Obj, but got Unknown
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0332: Unknown result type (might be due to invalid IL or missing references)
			Node frontParent = c.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)frontParent))
			{
				return;
			}
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Shiv")
			};
			frontParent.AddChild((Node)(object)holder, false, (InternalMode)0);
			Line2D halo = null;
			Line2D ribbon;
			if (ink)
			{
				halo = FxKit.Ribbon(20f, new Color(InkGlow.R, InkGlow.G, InkGlow.B, 0.55f));
				((Node)holder).AddChild((Node)(object)halo, false, (InternalMode)0);
				ribbon = FxKit.Ribbon(9f, new Color(0.16f, 0.07f, 0.32f, 0.9f));
				((CanvasItem)ribbon).Material = null;
			}
			else
			{
				ribbon = FxKit.Ribbon(9f, new Color(Steel.R, Steel.G, Steel.B, 0.7f));
			}
			((Node)holder).AddChild((Node)(object)ribbon, false, (InternalMode)0);
			Node2D blade = MakeShiv(ink);
			((Node)holder).AddChild((Node)(object)blade, false, (InternalMode)0);
			((CanvasItem)blade).Visible = false;
			Vector2 launch = pre(1f);
			Vector2 dest = ((!c.Remote) ? (c.Hand + new Vector2(Spread(slot) * 34f, 0f)) : (c.ShivReturnPoint ?? c.WeaponHand));
			Vector2 mid = new Vector2(Mathf.Lerp(launch.X, dest.X, 0.35f), Mathf.Min(launch.Y, dest.Y) - 70f);
			float fly = ((flyTime > 0f) ? flyTime : Tuning.F("shiv.fly_time", 0.36f));
			float total = preTime + fly;
			FxKit.FreeAfter((Node)(object)holder, Mathf.Max(0f, delay) + Mathf.Max(0f, total) + 0.26f);
			Vector2 last = pre(0f);
			Action<float> action = (float u) =>
			{
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
				//IL_0131: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_018e: Unknown result type (might be due to invalid IL or missing references)
				//IL_018f: Unknown result type (might be due to invalid IL or missing references)
				//IL_015c: Unknown result type (might be due to invalid IL or missing references)
				//IL_015d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0162: Unknown result type (might be due to invalid IL or missing references)
				//IL_016a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0185: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)holder) && FxKit.Alive((GodotObject)(object)blade))
				{
					float num = u * total;
					bool flag = preTime > 0.001f && num < preTime;
					Vector2 val2;
					if (flag)
					{
						val2 = pre(num / preTime);
					}
					else
					{
						float num2 = Mathf.Clamp((num - preTime) / fly, 0f, 1f);
						val2 = FxKit.Bezier(launch, mid, dest, flowing ? (num2 * (0.45f + 0.55f * num2)) : (num2 * num2));
					}
					Vector2 val3 = val2 - last;
					if (spin && (flag | flowing))
					{
						blade.Rotation = num * 24f * c.Facing;
					}
					else if (val3.LengthSquared() > 0.01f)
					{
						blade.Rotation = val3.Angle() + (float)Math.PI / 2f;
					}
					float num3 = Mathf.Min(1f, num / 0.08f);
					blade.Scale = new Vector2(num3, num3);
					blade.GlobalPosition = val2;
					((CanvasItem)blade).Visible = true;
					if (!flag)
					{
						Vector2 val4 = holder.ToLocal(val2);
						ribbon.AddPoint(val4, -1);
						if (FxKit.Alive((GodotObject)(object)halo))
						{
							halo.AddPoint(val4, -1);
						}
					}
					last = val2;
				}
			};
			Tween val = ((Node)holder).CreateTween();
			val.TweenMethod(Callable.From<float>(action), Variant.op_Implicit(0f), Variant.op_Implicit(1f), (double)total).SetDelay((double)delay);
			val.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.16);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
		}

		private static void RingPulse(Node parent, Vector2 at, float size, Color tint, float delay, float time, float fromScale, float toScale, float flatten = 1f, float dashes = 0f)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Node2D ring = FxKit.Sigil(size, tint, out var mat);
			parent.AddChild((Node)(object)ring, false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)ring, Mathf.Max(0f, delay) + Mathf.Max(0f, time) + 0.1f);
			ring.GlobalPosition = at;
			if (mat != null)
			{
				mat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.82f));
				mat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.03f));
				mat.SetShaderParameter(StringName.op_Implicit("dashes"), Variant.op_Implicit(dashes));
				mat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.35f));
				mat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(0f));
			}
			Action<float> action = (float t) =>
			{
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)ring))
				{
					float num = Mathf.Lerp(fromScale, toScale, t);
					ring.Scale = new Vector2(num, num * flatten);
					if (FxKit.Alive((GodotObject)(object)mat))
					{
						mat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(0.8f * (1f - t)));
						mat.SetShaderParameter(StringName.op_Implicit("spin"), Variant.op_Implicit(t * 3f));
					}
				}
			};
			Tween val = ((Node)ring).CreateTween();
			val.TweenMethod(Callable.From<float>(action), Variant.op_Implicit(0f), Variant.op_Implicit(1f), (double)time).SetDelay((double)delay).SetTrans((TransitionType)4)
				.SetEase((EaseType)1);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)ring))
				{
					((Node)ring).QueueFree();
				}
			})));
		}

		private static void Glint(Node parent, Vector2 at, Color color, float size)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)parent))
			{
				Sprite2D val = FxKit.Glow(color, new Vector2(size, size));
				parent.AddChild((Node)(object)val, false, (InternalMode)0);
				((Node2D)val).GlobalPosition = at;
				((Node)val).CreateTween().TweenProperty((GodotObject)(object)val, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.28);
				FxKit.FreeAfter((Node)(object)val, 0.4);
			}
		}

		public static void PlayCreate(CardFxContext c, ShivStyle style, int count, int start = 0)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_055e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0563: Unknown result type (might be due to invalid IL or missing references)
			//IL_0568: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_048b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			Node frontParent = c.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)frontParent))
			{
				return;
			}
			int num = Mathf.Clamp(count, 1, 8);
			start = Mathf.Clamp(start, 0, 12);
			bool flag = start == 0;
			Vector2 hand = WeaponHand(c);
			Vector2 chest = c.Chest;
			Vector2 body = c.Body;
			if (style == ShivStyle.Return && !c.HasTarget)
			{
				style = ShivStyle.Plain;
			}
			switch (style)
			{
			case ShivStyle.Dance:
			{
				Vector2 centre = chest + new Vector2(0f, body.Y * 0.06f);
				float reach = body.X * 0.48f + 30f;
				for (int num8 = 0; num8 < num; num8++)
				{
					int num9 = start + num8;
					float phase = (float)num9 * 2.4f;
					Launch(c, num9, (float t) =>
					{
						//IL_0039: Unknown result type (might be due to invalid IL or missing references)
						//IL_0054: Unknown result type (might be due to invalid IL or missing references)
						//IL_0059: Unknown result type (might be due to invalid IL or missing references)
						float num17 = phase + t * ((float)Math.PI * 2f) * 1.2f;
						float num18 = reach * Mathf.Min(1f, t * 3.5f);
						return centre + new Vector2(Mathf.Cos(num17) * num18, Mathf.Sin(num17) * num18 * 0.32f);
					}, Tuning.F("shiv.dance_time", 0.45f), (float)num8 * 0.03f, spin: false, ink: false);
				}
				if (flag)
				{
					RingPulse(frontParent, c.Feet, body.X * 1.7f, Steel, 0f, 0.5f, 0.4f, 1.3f, 0.3f, 9f);
				}
				return;
			}
			case ShivStyle.Cloak:
				CloakOpen(c, c.Held as ShivHeld, num, start, flag);
				return;
			case ShivStyle.Sleeves:
			{
				for (int num4 = 0; num4 < num; num4++)
				{
					int num5 = start + num4;
					float num6 = ((num5 % 2 == 0) ? 1f : (-1f)) * c.Facing;
					int num7 = num5 / 2;
					Vector2 sleeve = chest + new Vector2(num6 * body.X * 0.2f, body.Y * 0.14f);
					Vector2 outTo = sleeve + new Vector2(num6 * (70f + 26f * (float)num7), 0f - (34f + 12f * (float)num7));
					Launch(c, num5, (float t) =>
					{
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						//IL_0012: Unknown result type (might be due to invalid IL or missing references)
						return sleeve.Lerp(outTo, EaseOut(t));
					}, 0.2f, (float)num4 * 0.04f, spin: false, ink: false);
					Glint(frontParent, sleeve, new Color(1f, 1f, 1f, 0.8f), 80f);
				}
				return;
			}
			case ShivStyle.Fan:
			{
				for (int num13 = 0; num13 < num; num13++)
				{
					int num14 = start + num13;
					float num15 = Mathf.DegToRad(-12f + Spread(num14) * 15f);
					Vector2 grip = hand + new Vector2(body.X * 0.3f * c.Facing, -6f);
					Vector2 outTo3 = grip + new Vector2(Mathf.Cos(num15) * c.Facing, Mathf.Sin(num15)) * 110f;
					Launch(c, num14, (float t) =>
					{
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						//IL_0012: Unknown result type (might be due to invalid IL or missing references)
						return grip.Lerp(outTo3, EaseOut(t));
					}, 0.24f, (float)num13 * 0.06f, spin: false, ink: false);
				}
				if (flag)
				{
					Glint(frontParent, hand + new Vector2(body.X * 0.3f * c.Facing, -6f), new Color(Steel.R, Steel.G, Steel.B, 0.7f), 90f);
				}
				return;
			}
			case ShivStyle.Symphony:
			{
				for (int i = 0; i < num; i++)
				{
					int num2 = start + i;
					Vector2 note = chest + new Vector2(Spread(num2) * 60f * c.Facing, (0f - body.Y) * 0.72f - 22f * Mathf.Sin((float)num2 * 1.9f));
					Launch(c, num2, (float t) =>
					{
						//IL_000c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0017: Unknown result type (might be due to invalid IL or missing references)
						return chest.Lerp(note, EaseOut(t));
					}, 0.35f, (float)i * 0.07f, spin: false, ink: false);
				}
				if (flag)
				{
					RingPulse(frontParent, chest + new Vector2(0f, (0f - body.Y) * 0.6f), 220f, new Color(1f, 0.92f, 0.7f, 1f), 0f, 0.5f, 0.3f, 1.8f);
				}
				return;
			}
			case ShivStyle.Return:
			{
				Vector2 from = c.TargetChest;
				for (int num10 = 0; num10 < num; num10++)
				{
					int num11 = start + num10;
					float num12 = Mathf.DegToRad(-100f + Spread(num11) * 38f);
					Vector2 outTo2 = from + new Vector2(Mathf.Cos(num12) * c.TargetSide, Mathf.Sin(num12)) * 90f;
					Launch(c, num11, (float t) =>
					{
						//IL_000c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0012: Unknown result type (might be due to invalid IL or missing references)
						return from.Lerp(outTo2, t);
					}, 0.12f, (float)num10 * 0.04f, spin: true, ink: false, Tuning.F("shiv.return_time", 0.27f), flowing: true);
				}
				return;
			}
			case ShivStyle.Ink:
			{
				for (int num3 = 0; num3 < num; num3++)
				{
					Launch(c, start + num3, (float _) =>
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						return hand;
					}, 0f, 0.1f + (float)num3 * 0.06f, spin: false, ink: true);
				}
				if (flag)
				{
					InkSplat(frontParent, hand, 0.8f);
				}
				return;
			}
			}
			for (int num16 = 0; num16 < num; num16++)
			{
				Launch(c, start + num16, (float _) =>
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					return hand;
				}, 0f, (float)num16 * 0.05f, spin: false, ink: false);
			}
		}

		private static float Side(float facing)
		{
			if (!(facing < 0f))
			{
				return 1f;
			}
			return -1f;
		}

		private static Vector2 CloakSpan(CardFxContext c)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			return new Vector2(c.Body.X * 0.82f, c.Body.Y * 0.64f);
		}

		internal static Vector2 CloakRoot(CardFxContext c)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			return c.Chest + new Vector2((0f - c.Body.X) * 0.42f * Side(c.Facing), (0f - c.Body.Y) * 0.3f);
		}

		private static Vector2 CloakHeart(CardFxContext c)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = CloakSpan(c);
			Vector2 val2;
			if (!(val.LengthSquared() > 0.001f))
			{
				val2 = new Vector2(0f, 1f);
			}
			else
			{
				Vector2 val3 = new Vector2(0f - val.Y, val.X);
				val2 = val3.Normalized();
			}
			Vector2 val4 = val2;
			Vector2 val5 = val * 0.5f + val4 * 45f;
			return CloakRoot(c) + new Vector2(val5.X * Side(c.Facing), val5.Y);
		}

		internal static Node2D MakeCloak(CardFxContext c, out Line2D cloth, out Line2D hem)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected Obj, but got Unknown
			Vector2 to = CloakSpan(c);
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Cloak")
			};
			cloth = FxKit.Crescent(Vector2.Zero, to, 90f, 70f, new Color(0.05f, 0.2f, 0.17f, 0.85f));
			((CanvasItem)cloth).Material = null;
			((Node)val).AddChild((Node)(object)cloth, false, (InternalMode)0);
			hem = FxKit.Crescent(Vector2.Zero, to, 112f, 8f, new Color(0.4f, 0.9f, 0.75f, 0.7f));
			((Node)val).AddChild((Node)(object)hem, false, (InternalMode)0);
			return val;
		}

		internal static void PoseCloak(Node2D holder, Line2D cloth, float facing, float open)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)holder))
			{
				float num = Side(facing);
				float num2 = Mathf.Lerp(Tuning.F("cloak.closed_scale", 0.8f), 1.04f, open);
				holder.Scale = new Vector2(num2 * num, num2);
				holder.Rotation = Mathf.Lerp(Tuning.F("cloak.closed_turn", 0.3f), 0f, open) * num;
				if (FxKit.Alive((GodotObject)(object)cloth))
				{
					cloth.Width = 70f * Mathf.Lerp(0.7f, Tuning.F("cloak.spread", 1.4f), Mathf.Max(0f, open));
				}
			}
		}

		private static void CloakOpen(CardFxContext c, ShivHeld held, int knives, int slot, bool cloak)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			Node frontParent = c.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)frontParent))
			{
				return;
			}
			float side = Side(c.Facing);
			Vector2 heart = CloakHeart(c);
			float num = Mathf.Max(0f, Tuning.F("cloak.knife_gap", 0.11f));
			float num2 = (cloak ? Mathf.Max(0f, Tuning.F("cloak.knife_lead", 0.05f)) : 0f);
			for (int i = 0; i < knives; i++)
			{
				CloakKnife(c, slot + i, heart, side, num2 + (float)i * num, i);
			}
			if (!cloak)
			{
				return;
			}
			float open0 = 0f;
			float hem0 = 0.55f;
			float alpha0 = 0f;
			try
			{
				if (held != null && held.HandOverCloak(out var open1, out var hem1, out var alpha1))
				{
					open0 = open1;
					hem0 = hem1;
					alpha0 = alpha1;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ShivFx (cloak hand-over): " + ex.Message);
			}
			Node2D holder = MakeCloak(c, out var cloth, out var hem2);
			frontParent.AddChild((Node)(object)holder, false, (InternalMode)0);
			holder.GlobalPosition = CloakRoot(c);
			PoseCloak(holder, cloth, side, open0);
			((CanvasItem)holder).Modulate = new Color(1f, 1f, 1f, alpha0);
			if (FxKit.Alive((GodotObject)(object)hem2))
			{
				((CanvasItem)hem2).Modulate = new Color(1f, 1f, 1f, hem0);
			}
			float openTime = Mathf.Max(0.05f, Tuning.F("cloak.open_time", 0.24f));
			float hold = Mathf.Max(0f, Tuning.F("cloak.hold", 0.14f)) + (float)Mathf.Max(0, knives - 1) * num;
			float fade = Mathf.Max(0.05f, Tuning.F("cloak.fade", 0.3f));
			float total = openTime + hold + fade;
			FxKit.FreeAfter((Node)(object)holder, total + 0.1f);
			FxKit.Drive((Node)(object)holder, total, (float u) =>
			{
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_015a: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					float num3 = u * total;
					float num4 = Mathf.Clamp(num3 / openTime, 0f, 1f);
					float num5 = num4 - 1f;
					float num6 = 1f + 2.2f * num5 * num5 * num5 + 1.2f * num5 * num5;
					float num7 = 0.1f * Mathf.Clamp((num3 - openTime) / (hold + fade), 0f, 1f);
					PoseCloak(holder, cloth, side, Mathf.Lerp(open0, 1f, num6) + num7);
					float num8 = Mathf.Lerp(alpha0, 1f, Mathf.Clamp(num3 / 0.07f, 0f, 1f));
					float num9 = Mathf.Clamp((num3 - openTime - hold) / fade, 0f, 1f);
					((CanvasItem)holder).Modulate = new Color(1f, 1f, 1f, num8 * (1f - num9 * num9));
					if (FxKit.Alive((GodotObject)(object)hem2))
					{
						((CanvasItem)hem2).Modulate = new Color(1f, 1f, 1f, Mathf.Lerp(hem0, 1f, num4));
					}
				}
			}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
		}

		private static void CloakKnife(CardFxContext c, int slot, Vector2 heart, float side, float delay, int rank)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected Obj, but got Unknown
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			Node parent = c.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_CloakKnife")
			};
			parent.AddChild((Node)(object)holder, false, (InternalMode)0);
			Line2D ribbon = FxKit.Ribbon(9f, new Color(Steel.R, Steel.G, Steel.B, 0.7f));
			((Node)holder).AddChild((Node)(object)ribbon, false, (InternalMode)0);
			Node2D blade = MakeShiv(ink: false);
			((Node)holder).AddChild((Node)(object)blade, false, (InternalMode)0);
			((CanvasItem)blade).Visible = false;
			Vector2 val = new Vector2(side, -0.3f - 0.16f * (float)rank);
			Vector2 dir = val.Normalized();
			Vector2 p0 = heart - dir * 14f;
			Vector2 dest = ((!c.Remote) ? (c.Hand + new Vector2(Spread(slot) * 34f, 0f)) : (c.ShivReturnPoint ?? c.WeaponHand));
			Vector2 p1 = p0 + dir * (c.Body.X * 0.95f + 150f);
			Vector2 p2 = new Vector2(Mathf.Lerp(dest.X, p0.X, 0.22f), Mathf.Lerp(dest.Y, p0.Y, 0.7f));
			float fly = Mathf.Max(0.2f, Tuning.F("cloak.knife_time", 0.6f));
			FxKit.FreeAfter((Node)(object)holder, Mathf.Max(0f, delay) + fly + 0.26f);
			float hang = Mathf.Clamp(Tuning.F("cloak.knife_hang", 0.5f), 0f, 0.9f);
			Vector2 last = p0;
			blade.Rotation = dir.Angle() + (float)Math.PI / 2f;
			Action<float> action = (float u) =>
			{
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0135: Unknown result type (might be due to invalid IL or missing references)
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Unknown result type (might be due to invalid IL or missing references)
				//IL_0129: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)holder) && FxKit.Alive((GodotObject)(object)blade))
				{
					float num = u * fly;
					float t = u + hang * Mathf.Sin((float)Math.PI * 2f * u) / ((float)Math.PI * 2f);
					Vector2 val3 = Cubic(p0, p1, p2, dest, t);
					Vector2 val4 = val3 - last;
					if (val4.LengthSquared() > 0.01f)
					{
						blade.Rotation = val4.Angle() + (float)Math.PI / 2f;
					}
					float num2 = Mathf.Clamp(0.3f + 0.7f * num / 0.1f, 0.3f, 1f);
					float num3 = 1f + 0.3f * Mathf.Clamp(1f - num / 0.16f, 0f, 1f);
					blade.Scale = new Vector2(num2, num2 * num3);
					blade.GlobalPosition = val3;
					((CanvasItem)blade).Visible = true;
					if (FxKit.Alive((GodotObject)(object)ribbon))
					{
						ribbon.AddPoint(holder.ToLocal(val3), -1);
					}
					last = val3;
				}
			};
			Tween val2 = ((Node)holder).CreateTween();
			val2.TweenMethod(Callable.From<float>(action), Variant.op_Implicit(0f), Variant.op_Implicit(1f), (double)fly).SetDelay((double)delay);
			val2.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.16);
			val2.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
			bool remoteGlint = MultiplayerFx.Active && c.Card != null && !MultiplayerFx.IsLocal(c.Card);
			FxKit.After(parent, delay + 0.05f, () =>
			{
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				using (RemoteFxFade.Capture(remoteGlint, parent))
				{
					Glint(parent, heart + dir * 56f, new Color(0.78f, 1f, 0.9f, 0.85f), 120f);
				}
			});
		}

		private static Vector2 Cubic(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			float num = 1f - t;
			return a * (num * num * num) + b * (3f * num * num * t) + c * (3f * num * t * t) + d * (t * t * t);
		}

		private static void InkSplat(Node parent, Vector2 at, float size)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)parent))
			{
				FxKit.Burst(parent, at, new Color(0.1f, 0.05f, 0.2f, 0.95f), 16, 90f, 300f, 0.6, 0.25f * size, 0.6f * size, new Vector2(0f, 520f), 180f, null, 10f, 0f, additive: false);
				Glint(parent, at, new Color(InkGlow.R, InkGlow.G, InkGlow.B, 0.7f), 150f * size);
			}
		}

		public static void PlayHit(CardFxContext c, Vector2 at, bool accuracy, bool phantom, bool ink, Vector2? origin = null, bool fromTrap = false, bool veil = true, bool own = false)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			using (RemoteFxFade.Capture(c.Remote, c.BackParent, c.FrontParent, GameRefs.Overlay, (Node)GameRefs.CombatRoom))
			{
				Node frontParent = c.FrontParent;
				if (!FxKit.Alive((GodotObject)(object)frontParent))
				{
					return;
				}
				Vector2 val = origin ?? WeaponHand(c);
				Color col;
				if (fromTrap)
				{
					col = KnifeTrapFx.Ember;
				}
				else if (ink)
				{
					col = InkGlow;
				}
				else
				{
					col = (accuracy ? Gold : Steel);
				}
				if (!own)
				{
					ThrowFlash(c, frontParent, at, val, col);
				}
				if (accuracy)
				{
					Node2D ring = FxKit.Sigil(120f, Gold, out var mat);
					frontParent.AddChild((Node)(object)ring, false, (InternalMode)0);
					ring.GlobalPosition = at;
					if (mat != null)
					{
						mat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.7f));
						mat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.05f));
						mat.SetShaderParameter(StringName.op_Implicit("ticks"), Variant.op_Implicit(4f));
						mat.SetShaderParameter(StringName.op_Implicit("tick_len"), Variant.op_Implicit(0.3f));
						mat.SetShaderParameter(StringName.op_Implicit("tick_width"), Variant.op_Implicit(0.03f));
						mat.SetShaderParameter(StringName.op_Implicit("tick_out"), Variant.op_Implicit(0.5f));
						mat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(0f));
					}
					FxKit.FreeAfter((Node)(object)ring, 0.3499999940395355);
					FxKit.Drive((Node)(object)ring, 0.25f, (float t) =>
					{
						//IL_0027: Unknown result type (might be due to invalid IL or missing references)
						//IL_0055: Unknown result type (might be due to invalid IL or missing references)
						if (FxKit.Alive((GodotObject)(object)ring))
						{
							float num = Mathf.Lerp(1.6f, 0.7f, t);
							ring.Scale = new Vector2(num, num);
							if (FxKit.Alive((GodotObject)(object)mat))
							{
								mat.SetShaderParameter(StringName.op_Implicit("alpha"), Variant.op_Implicit(1f - t));
							}
						}
					}, (TransitionType)4, (EaseType)1).TweenCallback(Callable.From((Action)(() =>
					{
						if (FxKit.Alive((GodotObject)(object)ring))
						{
							((Node)ring).QueueFree();
						}
					})));
				}
				if (phantom)
				{
					PhantomStrike(c, at, origin, veil);
				}
				if (ink)
				{
					InkSplat(frontParent, at, 1f);
				}
			}
		}

		private static void ThrowFlash(CardFxContext c, Node parent, Vector2 at, Vector2 from, Color col)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected Obj, but got Unknown
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_ShivHit")
			};
			parent.AddChild((Node)(object)holder, false, (InternalMode)0);
			holder.GlobalPosition = from;
			float targetSide = c.TargetSide;
			((Node)holder).AddChild((Node)(object)FxKit.Crescent(Vector2.Zero, at - from, -16f * targetSide, Tuning.F("shiv.streak", 10f), new Color(col.R, col.G, col.B, 0.8f)), false, (InternalMode)0);
			((Node)holder).AddChild((Node)(object)FxKit.Crescent(Vector2.Zero, at - from, -16f * targetSide, 3.5f, new Color(1f, 1f, 1f, 0.95f)), false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)holder, 0.2800000011920929);
			Tween val = ((Node)holder).CreateTween();
			val.TweenProperty((GodotObject)(object)holder, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.18);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
			Polygon2D val2 = FxKit.Shape(FxKit.Star(4, 30f, 7f), new Color(Mathf.Lerp(col.R, 1f, 0.5f), Mathf.Lerp(col.G, 1f, 0.5f), Mathf.Lerp(col.B, 1f, 0.5f), 0.95f));
			((CanvasItem)val2).Material = (Material)(object)FxKit.Additive;
			parent.AddChild((Node)(object)val2, false, (InternalMode)0);
			((Node2D)val2).GlobalPosition = at;
			((Node2D)val2).Rotation = GD.Randf() * (float)Math.PI;
			((Node2D)val2).Scale = new Vector2(0.4f, 0.4f);
			Tween val3 = ((Node)val2).CreateTween();
			val3.SetParallel(true);
			val3.TweenProperty((GodotObject)(object)val2, NodePath.op_Implicit("scale"), Variant.op_Implicit(new Vector2(1.4f, 1.4f)), 0.16).SetTrans((TransitionType)4).SetEase((EaseType)1);
			val3.TweenProperty((GodotObject)(object)val2, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.2);
			FxKit.FreeAfter((Node)(object)val2, 0.3);
			FxKit.Burst(parent, at, new Color(col.R, col.G, col.B, 0.95f), 8, 120f, 320f, 0.35, 0.08f, 0.2f, new Vector2(0f, 400f));
		}

		private static bool RemoteShivParticipants(CardModel shiv, Creature target)
		{
			CombatManager instance = CombatManager.Instance;
			CombatState val = ((instance != null) ? instance.DebugOnlyGetState() : null);
			if (!RemoteShivOwner(shiv) || target == null || (object)target.CombatState != val)
			{
				return false;
			}
			foreach (Creature enemy in val.Enemies)
			{
				if (enemy == target)
				{
					return true;
				}
			}
			return false;
		}

		private static bool RemoteShivOwner(CardModel card)
		{
			object obj;
			if (card == null)
			{
				obj = null;
			}
			else
			{
				Player owner = card.Owner;
				obj = ((owner != null) ? owner.Creature : null);
			}
			Creature val = (Creature)obj;
			CombatManager instance = CombatManager.Instance;
			CombatState val2 = ((instance != null) ? instance.DebugOnlyGetState() : null);
			if (!FxKit.Alive((GodotObject)(object)GameRefs.CombatRoom) || val == null || val2 == null || !val.IsAlive || (object)val.CombatState != val2)
			{
				return false;
			}
			foreach (Player player in val2.Players)
			{
				if (player == card.Owner && player.Creature == val)
				{
					return true;
				}
			}
			return false;
		}

		private static void PhantomStrike(CardFxContext c, Vector2 at, Vector2? origin, bool veil)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Expected Obj, but got Unknown
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
			Node parent = c.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Vector2 dir = at - (origin ?? c.Chest);
			dir = (Vector2)((dir.LengthSquared() > 1f) ? dir.Normalized() : new Vector2(c.TargetSide, 0f));
			float length = Tuning.F("shiv.phantom_length", 340f) * Mathf.Max(1f, c.TargetWidth / 340f);
			float num = Tuning.F("shiv.phantom_halfwidth", 42f);
			Color val = new Color(0f, 0f, 0f, 0f);
			Color blue = new Color(0.45f, 0.7f, 1f, 1f);
			if (veil && FxKit.Alive((GodotObject)(object)c.BackParent))
			{
				ColorRect dim = FxKit.Veil(new Color(0.03f, 0.07f, 0.2f, 0f));
				c.BackParent.AddChild((Node)(object)dim, false, (InternalMode)0);
				FxKit.FreeAfter((Node)(object)dim, 0.5899999737739563);
				Tween val2 = ((Node)dim).CreateTween();
				val2.TweenProperty((GodotObject)(object)dim, NodePath.op_Implicit("color:a"), Variant.op_Implicit(Tuning.F("shiv.phantom_veil", 0.45f)), 0.05);
				val2.TweenInterval(0.16);
				val2.TweenProperty((GodotObject)(object)dim, NodePath.op_Implicit("color:a"), Variant.op_Implicit(0f), 0.28);
				val2.TweenCallback(Callable.From((Action)(() =>
				{
					if (FxKit.Alive((GodotObject)(object)dim))
					{
						((Node)dim).QueueFree();
					}
				})));
			}
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_PhantomStrike")
			};
			parent.AddChild((Node)(object)holder, false, (InternalMode)0);
			holder.GlobalPosition = at;
			holder.Rotation = dir.Angle() + (float)Math.PI / 2f;
			Node2D[] echoes = new Node2D[2];
			for (int num2 = 0; num2 < echoes.Length; num2++)
			{
				echoes[num2] = FxKit.Blade(length, num, new Color(0.55f, 0.75f, 1f, 0.3f - 0.1f * (float)num2), new Color(0.35f, 0.55f, 1f, 0.24f - 0.08f * (float)num2), val, val);
				((Node)holder).AddChild((Node)(object)echoes[num2], false, (InternalMode)0);
			}
			Node2D shade = FxKit.Blade(length * 1.03f, num * 1.28f, new Color(0.02f, 0.05f, 0.18f, 0.8f), new Color(0.02f, 0.04f, 0.14f, 0.8f), val, val);
			((Node)holder).AddChild((Node)(object)shade, false, (InternalMode)0);
			Node2D ghost = FxKit.Blade(length, num, new Color(0.93f, 0.98f, 1f, 1f), new Color(0.5f, 0.72f, 1f, 0.96f), val, new Color(0.45f, 0.72f, 1f, 0.6f));
			((Node)holder).AddChild((Node)(object)ghost, false, (InternalMode)0);
			float startY = length + 260f;
			float endY = length * 0.4f;
			float thrust = Tuning.F("shiv.phantom_thrust", 0.11f);
			float hold = Tuning.F("shiv.phantom_hold", 0.16f);
			float fade = 0.22f;
			float total = thrust + hold + fade;
			FxKit.FreeAfter((Node)(object)holder, Mathf.Max(0f, total) + 0.1f);
			bool landed = false;
			((CanvasItem)holder).Modulate = new Color(1f, 1f, 1f, 0f);
			FxKit.Drive((Node)(object)holder, total, (float u) =>
			{
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Unknown result type (might be due to invalid IL or missing references)
				//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
				//IL_0205: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					float num3 = u * total;
					float num4 = Mathf.Clamp(num3 / thrust, 0f, 1f);
					float num5 = 1f - Mathf.Pow(1f - num4, 4f);
					float num6 = Mathf.Lerp(startY, endY, num5);
					float num7 = ((num3 > thrust && num3 < thrust + hold) ? (Mathf.Sin(num3 * 150f) * 3f) : 0f);
					if (FxKit.Alive((GodotObject)(object)ghost))
					{
						ghost.Position = new Vector2(num7, num6);
					}
					if (FxKit.Alive((GodotObject)(object)shade))
					{
						shade.Position = new Vector2(num7, num6 + length * 0.015f);
					}
					for (int i = 0; i < echoes.Length; i++)
					{
						if (FxKit.Alive((GodotObject)(object)echoes[i]))
						{
							echoes[i].Position = new Vector2(0f, num6 + (70f + 70f * (float)i) * (1f - 0.6f * num5));
						}
					}
					float num8 = ((num3 < thrust) ? Mathf.Min(1f, num4 * 3f) : ((num3 < thrust + hold) ? 1f : (1f - (num3 - thrust - hold) / fade)));
					((CanvasItem)holder).Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(num8, 0f, 1f));
					if (!landed && num4 >= 1f)
					{
						landed = true;
						using (RemoteFxFade.Capture(c.Remote, parent))
						{
							PhantomImpact(parent, at, dir, blue);
						}
					}
				}
			}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)holder))
				{
					((Node)holder).QueueFree();
				}
			})));
		}

		private static void PhantomImpact(Node parent, Vector2 at, Vector2 dir, Color blue)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected Obj, but got Unknown
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)parent))
			{
				return;
			}
			Node2D streak = new Node2D();
			parent.AddChild((Node)(object)streak, false, (InternalMode)0);
			streak.GlobalPosition = at;
			((Node)streak).AddChild((Node)(object)FxKit.Crescent(-dir * 460f, dir * 300f, 0f, 30f, new Color(blue.R, blue.G, blue.B, 0.75f)), false, (InternalMode)0);
			((Node)streak).AddChild((Node)(object)FxKit.Crescent(-dir * 460f, dir * 300f, 0f, 8f, new Color(1f, 1f, 1f, 0.95f)), false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)streak, 0.4000000059604645);
			Tween val = ((Node)streak).CreateTween();
			val.TweenProperty((GodotObject)(object)streak, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.3);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)streak))
				{
					((Node)streak).QueueFree();
				}
			})));
			Polygon2D val2 = FxKit.Shape(FxKit.Star(4, 110f, 13f), new Color(0.85f, 0.94f, 1f, 0.95f));
			((CanvasItem)val2).Material = (Material)(object)FxKit.Additive;
			parent.AddChild((Node)(object)val2, false, (InternalMode)0);
			((Node2D)val2).GlobalPosition = at;
			((Node2D)val2).Rotation = dir.Angle();
			((Node2D)val2).Scale = new Vector2(0.4f, 0.4f);
			Tween val3 = ((Node)val2).CreateTween();
			val3.SetParallel(true);
			val3.TweenProperty((GodotObject)(object)val2, NodePath.op_Implicit("scale"), Variant.op_Implicit(new Vector2(1.5f, 1.5f)), 0.2).SetTrans((TransitionType)4).SetEase((EaseType)1);
			val3.TweenProperty((GodotObject)(object)val2, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.3);
			FxKit.FreeAfter((Node)(object)val2, 0.4);
			FxKit.Flash(parent, at, new Color(0.7f, 0.86f, 1f, 0.9f), 240f, 2.3f, 0.32);
			FxKit.RingPulse(parent, at, 360f, blue, 0f, 0.42f, 0.2f, 1.3f, 1f, 0f, 0.035f, 0.95f);
			FxKit.Burst(parent, at, new Color(0.75f, 0.9f, 1f, 0.95f), 26, 220f, 620f, 0.5, 0.08f, 0.22f, new Vector2(0f, 300f), 50f, dir, 14f);
		}

		static ShivFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		}
	}
