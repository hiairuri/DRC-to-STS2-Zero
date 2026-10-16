// namespace: SilentFX.Cards
// type: KnifeTrapFx (class)
// source lines: 70314-71823
	public sealed class KnifeTrapFx : CardFx
	{
		internal sealed class Trap
		{
			public CardModel Card;

			public bool Remote;

			public bool Scoped;

			public object Invocation;

			public object Room;

			public object Owner;

			public Node2D Holder;

			public Node Front;

			public Node2D Sigil;

			public ShaderMaterial Mat;

			public ColorRect Veil;

			public Sprite2D PileGlow;

			public Node2D[] Blades = Array.Empty<Node2D>();

			public float[] Seat = Array.Empty<float>();

			public float[] StabAt = Array.Empty<float>();

			public float[] WindAt = Array.Empty<float>();

			public float[] FillDue = Array.Empty<float>();

			public float[] FillAt = Array.Empty<float>();

			public bool[] Fresh = Array.Empty<bool>();

			public Vector2[] ThroughFrom = Array.Empty<Vector2>();

			public Vector2[] ThroughTo = Array.Empty<Vector2>();

			public bool[] Burst = Array.Empty<bool>();

			public bool[] Final = Array.Empty<bool>();

			public float PierceTime = 0.2f;

			public float KickAt = -1f;

			public int Reserve;

			public int Pending = -1;

			public int[] Order = Array.Empty<int>();

			public int Next;

			public int Expected;

			public bool Auto;

			public int AutoNext;

			public float AutoTotal;

			public bool FromPile;

			public Vector2 Pile;

			public Vector2 Centre;

			public Vector2 CentreGoal;

			public Vector2 Feet;

			public float Radius = 150f;

			public float Spin;

			public bool Placed;

			public bool Aimed;

			public float AimedAt;

			public bool Stabbing;

			public float StabClock;

			public float Time;

			public float CloseAt = -1f;

			public float Linger = -1f;

			public bool Dismissed;

			public float Sink;

			public bool ShivInPlay;

			public Vector2 LastFrom;

			public bool HasLast;

			public bool Dead;

			public bool Volley;

			public int Fired;
		}

		private sealed class Volley
		{
			public CardModel Card;

			public object Owner;

			public object Invocation;

			public object Room;

			public bool Remote;

			public Trap Magazine;

			public Vector2 Pile;

			public int Expected;

			public int Next;

			public bool ShivInPlay;

			public bool Launched;

			public bool Closed;

			public ulong ClosedAt;
		}

		private static readonly StringName PAlpha = StringName.op_Implicit("alpha");

		private static readonly StringName PSpin = StringName.op_Implicit("spin");

		private static readonly StringName PInnerSpin = StringName.op_Implicit("inner_spin");

		public static readonly Color Ember = new Color(1f, 0.46f, 0.2f, 1f);

		private const float FadeTime = 0.3f;

		private const float MaxStabLife = 14f;

		private static readonly List<Trap> Live = new List<Trap>();

		private static Trap _s;

		private static Volley _volley;

		private static readonly List<Volley> MultiplayerVolleys = new List<Volley>();

		private static bool _ticking;

		private static readonly List<Trap> Plays = new List<Trap>();

		private static Trap CurrentTrap
		{
			get
			{
				if (!MultiplayerFx.Active)
				{
					if (MultiplayerFx.CurrentInvocation != null)
					{
						return null;
					}
					return _s;
				}
				KnifeTrapSource.Link source = KnifeTrapSource.LinkFor(MultiplayerFx.CurrentCard);
				if (source == null)
				{
					return null;
				}
				return Plays.LastOrDefault((Trap t) => t.Card == source.Trap && t.Invocation == source.Invocation && t.Room == source.Room && t.Owner == source.Trap.Owner);
			}
		}

		private static Volley CurrentVolley
		{
			get
			{
				if (!MultiplayerFx.Active)
				{
					if (MultiplayerFx.CurrentInvocation != null)
					{
						return null;
					}
					return _volley;
				}
				KnifeTrapSource.Link link = KnifeTrapSource.LinkFor(MultiplayerFx.CurrentCard);
				CardModel val = link?.Trap ?? MultiplayerFx.CurrentCard;
				object obj = link?.Invocation ?? MultiplayerFx.CurrentInvocation;
				if (obj == null)
				{
					return null;
				}
				for (int num = MultiplayerVolleys.Count - 1; num >= 0; num--)
				{
					Volley volley = MultiplayerVolleys[num];
					if (volley.Card == val && volley.Owner == ((val != null) ? val.Owner : null) && volley.Room == GameRefs.CombatRoom && volley.Invocation == obj)
					{
						return volley;
					}
				}
				return null;
			}
		}

		public static bool Throwing
		{
			get
			{
				if (!IsActive(CurrentTrap) || !CurrentTrap.ShivInPlay)
				{
					if (CurrentVolley != null)
					{
						return CurrentVolley.ShivInPlay;
					}
					return false;
				}
				return true;
			}
		}

		public static bool Active => IsActive(CurrentTrap);

		private static bool IsActive(Trap t)
		{
			if (t != null && !t.Dead && t.Stabbing && !t.Auto && t.CloseAt < 0f)
			{
				return FxKit.Alive((GodotObject)(object)t.Holder);
			}
			return false;
		}

		internal static bool OwnsShiv(CardModel card)
		{
			if (MultiplayerFx.Active && card == MultiplayerFx.CurrentCard)
			{
				if (!IsActive(CurrentTrap))
				{
					return CurrentVolley != null;
				}
				return true;
			}
			return false;
		}

		public override void OnPlay(CardFxContext c)
		{
			if ((!MultiplayerFx.Active && MultiplayerFx.CurrentInvocation != null) || (MultiplayerFx.Active && c.Card != null && MultiplayerFx.CurrentInvocation == null))
			{
				return;
			}
			if (c.Card != null)
			{
				Player owner = c.Card.Owner;
				if (ShivFan.Active((owner != null) ? owner.Creature : null))
				{
					BeginVolley(c);
					return;
				}
			}
			if (!c.HasTarget || !FxKit.Alive((GodotObject)(object)c.FrontParent))
			{
				return;
			}
			bool flag = c.Card == null;
			int num = (flag ? Tuning.I("trap.blades", 8) : CountSpentShivs(c.Card));
			Trap trap = (c.Held as KnifeTrapHeld)?.Take();
			if (trap == null || trap.Dead)
			{
				trap = Create(c, num, fromPile: false);
				if (trap == null)
				{
					return;
				}
			}
			Aim(trap, c);
			if (MultiplayerFx.Active && c.Card != null)
			{
				trap.Invocation = MultiplayerFx.CurrentInvocation;
				trap.Room = GameRefs.CombatRoom;
				trap.Owner = c.Card.Owner;
				if (!Plays.Contains(trap))
				{
					Plays.Add(trap);
				}
			}
			else
			{
				if (_s != null && _s != trap)
				{
					Free(_s);
				}
				_s = trap;
			}
			trap.Stabbing = true;
			trap.StabClock = 0f;
			trap.Auto = flag;
			trap.AutoTotal = Tuning.F("trap.time", 1.15f);
			trap.Expected = ((!flag) ? num : 0);
			trap.Reserve = ((!flag && !(Tuning.F("trap.refill", 1f) < 0.5f)) ? Math.Max(0, num - trap.Blades.Length) : 0);
			if (PowerFxManager.LogTriggers && !flag)
			{
				DevLog.Info($"Knife Trap: {trap.Blades.Length} blade(s) for {num} spent Shiv(s)");
			}
		}

		internal static Trap Create(CardFxContext c, int shivs, bool fromPile)
		{
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Expected Obj, but got Unknown
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0413: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			if (c.Remote)
			{
				fromPile = false;
			}
			Node val = ((fromPile && FxKit.Alive((GodotObject)(object)c.UiParent)) ? c.UiParent : c.FrontParent);
			if (!FxKit.Alive((GodotObject)(object)val))
			{
				return null;
			}
			int num = Mathf.Clamp(shivs, 0, Mathf.Clamp(Tuning.I("trap.ring", 16), 2, 16));
			Trap trap = new Trap
			{
				Card = c.Card,
				Remote = c.Remote,
				Scoped = (MultiplayerFx.Active && c.Card != null),
				FromPile = fromPile,
				Front = c.FrontParent,
				Pile = (c.Remote ? Vector2.Zero : c.ExhaustPile),
				Blades = new Node2D[num],
				Seat = new float[num],
				StabAt = new float[num],
				WindAt = new float[num],
				FillDue = new float[num],
				FillAt = new float[num],
				Fresh = new bool[num],
				ThroughFrom = new Vector2[num],
				ThroughTo = new Vector2[num],
				Burst = new bool[num],
				Final = new bool[num],
				PierceTime = Mathf.Clamp(Tuning.F("trap.pierce_time", 0.2f), 0.08f, 0.6f),
				Order = MakeOrder(num),
				Aimed = (!c.Remote && !fromPile)
			};
			if (FxKit.Alive((GodotObject)(object)c.BackParent))
			{
				trap.Veil = FxKit.Veil(new Color(0.02f, 0.01f, 0.04f, 0f));
				c.BackParent.AddChild((Node)(object)trap.Veil, false, (InternalMode)0);
				Node2D val2 = FxKit.Sigil(100f, Ember, out var mat);
				c.BackParent.AddChild((Node)(object)val2, false, (InternalMode)0);
				if (mat != null)
				{
					mat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.84f));
					mat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.022f));
					mat.SetShaderParameter(StringName.op_Implicit("dashes"), Variant.op_Implicit(12f));
					mat.SetShaderParameter(StringName.op_Implicit("inner"), Variant.op_Implicit(0.58f));
					mat.SetShaderParameter(StringName.op_Implicit("inner_width"), Variant.op_Implicit(0.014f));
					mat.SetShaderParameter(StringName.op_Implicit("ticks"), Variant.op_Implicit((float)Math.Max(num, 6)));
					mat.SetShaderParameter(StringName.op_Implicit("tick_len"), Variant.op_Implicit(0.16f));
					mat.SetShaderParameter(StringName.op_Implicit("tick_width"), Variant.op_Implicit(0.016f));
					mat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.3f));
					mat.SetShaderParameter(StringName.op_Implicit("fill"), Variant.op_Implicit(0.05f));
					mat.SetShaderParameter(PAlpha, Variant.op_Implicit(0f));
				}
				trap.Sigil = val2;
				trap.Mat = mat;
			}
			Node2D val3 = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_KnifeTrap")
			};
			val.AddChild((Node)(object)val3, false, (InternalMode)0);
			trap.Holder = val3;
			if (fromPile)
			{
				trap.PileGlow = FxKit.Glow(new Color(Ember.R, Ember.G, Ember.B, 0f), new Vector2(210f, 210f));
				((Node)val3).AddChild((Node)(object)trap.PileGlow, false, (InternalMode)0);
				((Node2D)trap.PileGlow).GlobalPosition = trap.Pile;
				FxKit.Burst((Node)(object)val3, trap.Pile, new Color(1f, 0.62f, 0.28f, 0.95f), 12, 80f, 260f, 0.5, 0.08f, 0.18f, new Vector2(0f, 300f), 50f, new Vector2(0f, -1f), 16f);
			}
			float num2 = Tuning.F("trap.blade_length", 54f);
			for (int i = 0; i < num; i++)
			{
				trap.StabAt[i] = -1f;
				trap.WindAt[i] = -1f;
				trap.FillDue[i] = -1f;
				trap.FillAt[i] = -1f;
				trap.Seat[i] = ((c.Remote | fromPile) ? 0f : 1f);
				trap.Blades[i] = FxKit.Blade(num2, num2 * 0.14f, new Color(0.96f, 0.95f, 0.92f, 1f), new Color(0.62f, 0.58f, 0.55f, 1f), new Color(0.3f, 0.2f, 0.16f, 1f), new Color(Ember.R, Ember.G, Ember.B, 0.3f));
				((CanvasItem)trap.Blades[i]).Modulate = new Color(1f, 1f, 1f, 0f);
				((Node)val3).AddChild((Node)(object)trap.Blades[i], false, (InternalMode)0);
			}
			Live.Add(trap);
			if (!_ticking)
			{
				_ticking = true;
				Ticker.Add(Update);
			}
			if (c.Remote && c.HasTarget)
			{
				Aim(trap, c);
			}
			Step(trap, 0f);
			return trap;
		}

		internal static void Aim(Trap t, CardFxContext c)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			if (t != null && !t.Dead && c.HasTarget)
			{
				t.CentreGoal = c.TargetChest;
				t.Feet = c.TargetFeet;
				t.Radius = Mathf.Clamp(c.TargetWidth * 0.75f, 110f, 220f) * Mathf.Clamp(0.86f + 0.035f * (float)t.Blades.Length, 0.9f, 1.45f);
				if (!t.Placed || !t.Aimed)
				{
					t.Centre = t.CentreGoal;
					t.Placed = true;
				}
				if (!t.Aimed)
				{
					t.Aimed = true;
					t.AimedAt = t.Time;
				}
				if (FxKit.Alive((GodotObject)(object)t.Sigil))
				{
					float num = t.Radius * 2.7f;
					t.Sigil.Scale = new Vector2(num / 100f, num / 100f * 0.3f);
				}
			}
		}

		internal static void Unaim(Trap t)
		{
			if (t != null && !t.Dead && t.Aimed && !t.Stabbing && (t.FromPile || t.Remote))
			{
				t.Aimed = false;
				t.AimedAt = t.Time;
			}
		}

		internal static void Dismiss(Trap t)
		{
			if (t != null && !t.Dead && !t.Stabbing)
			{
				if (!t.FromPile)
				{
					t.CloseAt = ((t.CloseAt < 0f) ? t.Time : t.CloseAt);
					return;
				}
				t.Aimed = false;
				t.AimedAt = t.Time;
				t.Dismissed = true;
			}
		}

		private static void BeginVolley(CardFxContext c)
		{
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			if (!MultiplayerFx.Active && _volley != null)
			{
				EndVolley();
			}
			else if (MultiplayerFx.Active && CurrentVolley != null)
			{
				EndVolley(CurrentVolley);
			}
			Trap trap = (c.Held as KnifeTrapHeld)?.Take();
			if (trap != null && (trap.Dead || !trap.FromPile))
			{
				if (!trap.Dead)
				{
					Free(trap);
				}
				trap = null;
			}
			if (trap != null)
			{
				trap.Volley = true;
				trap.Fired = 0;
				if (trap.Aimed)
				{
					trap.Aimed = false;
					trap.AimedAt = trap.Time;
				}
			}
			Volley volley = new Volley
			{
				Card = c.Card
			};
			CardModel card = c.Card;
			volley.Owner = ((card != null) ? card.Owner : null);
			volley.Invocation = (MultiplayerFx.Active ? MultiplayerFx.CurrentInvocation : null);
			volley.Room = GameRefs.CombatRoom;
			volley.Remote = c.Remote;
			volley.Magazine = trap;
			volley.Pile = (c.Remote ? Vector2.Zero : c.ExhaustPile);
			volley.Expected = CountSpentShivs(c.Card);
			Volley volley2 = volley;
			if (MultiplayerFx.Active)
			{
				MultiplayerVolleys.Add(volley2);
			}
			else
			{
				_volley = volley2;
			}
			if (PowerFxManager.LogTriggers)
			{
				DevLog.Info($"Knife Trap (Fan of Knives): {volley2.Expected} spent Shiv(s) out of the pile{((trap != null) ? $", {trap.Blades.Length} hanging over it" : "")}");
			}
		}

		public static bool TakeVolleyKnife(out Vector2 from, out float rot, out int index, out bool last, out bool hanging, out Vector2 pile)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			from = default;
			rot = 0f;
			index = 0;
			last = false;
			hanging = false;
			pile = default;
			Volley currentVolley = CurrentVolley;
			if (currentVolley == null || !currentVolley.ShivInPlay || currentVolley.Launched)
			{
				return false;
			}
			currentVolley.Launched = true;
			index = currentVolley.Next;
			last = currentVolley.Next >= currentVolley.Expected;
			pile = currentVolley.Pile;
			Trap magazine = currentVolley.Magazine;
			if (magazine != null && !magazine.Dead && magazine.Fired < magazine.Blades.Length)
			{
				Node2D val = magazine.Blades[magazine.Fired];
				magazine.Fired++;
				if (FxKit.Alive((GodotObject)(object)val) && ((CanvasItem)val).Visible)
				{
					from = val.GlobalPosition;
					rot = val.GlobalRotation;
					((CanvasItem)val).Visible = false;
					hanging = true;
					return true;
				}
			}
			from = currentVolley.Pile;
			return true;
		}

		private static void CloseVolley(Volley requested = null)
		{
			Volley volley = requested ?? CurrentVolley;
			if (volley != null)
			{
				if (volley.Next >= volley.Expected && !volley.ShivInPlay)
				{
					EndVolley(volley);
				}
				else if (!volley.Closed)
				{
					volley.Closed = true;
					volley.ClosedAt = FxClock.Milliseconds;
				}
			}
		}

		private static void EndVolley(Volley requested = null)
		{
			Volley volley = requested ?? CurrentVolley;
			if (volley != null)
			{
				if (MultiplayerFx.Active)
				{
					MultiplayerVolleys.Remove(volley);
				}
				else
				{
					_volley = null;
				}
				if (volley?.Magazine != null && !volley.Magazine.Dead)
				{
					Dismiss(volley.Magazine);
				}
			}
		}

		public static void OnShivPlayed()
		{
			Volley currentVolley = CurrentVolley;
			if (currentVolley != null)
			{
				currentVolley.ShivInPlay = true;
				currentVolley.Launched = false;
				currentVolley.Next++;
			}
			Trap currentTrap = CurrentTrap;
			if (IsActive(currentTrap))
			{
				currentTrap.ShivInPlay = true;
				Choose(currentTrap, "card play");
			}
		}

		public static void OnShivFinished()
		{
			Volley currentVolley = CurrentVolley;
			if (currentVolley != null)
			{
				currentVolley.ShivInPlay = false;
				if (currentVolley.Closed && currentVolley.Next >= currentVolley.Expected)
				{
					EndVolley();
				}
			}
			Trap currentTrap = CurrentTrap;
			if (currentTrap != null)
			{
				currentTrap.ShivInPlay = false;
				currentTrap.Pending = -1;
			}
		}

		public static bool TryThrow(out Vector2 from)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			from = default;
			Trap currentTrap = CurrentTrap;
			if (!IsActive(currentTrap) || currentTrap.Blades.Length == 0)
			{
				return false;
			}
			if (currentTrap.Pending < 0)
			{
				Choose(currentTrap, "throw");
			}
			return Strike(currentTrap, out from);
		}

		public static bool TryGetOrigin(out Vector2 from)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			from = default;
			Trap currentTrap = CurrentTrap;
			if (!IsActive(currentTrap) || currentTrap.Blades.Length == 0)
			{
				return false;
			}
			if (currentTrap.Pending < 0 && !currentTrap.ShivInPlay)
			{
				Choose(currentTrap, "hit");
			}
			if (currentTrap.Pending >= 0 && currentTrap.StabAt[currentTrap.Pending] < 0f)
			{
				Strike(currentTrap, out var _);
			}
			if (!currentTrap.HasLast)
			{
				return false;
			}
			from = currentTrap.LastFrom;
			return true;
		}

		public static void Close(CardModel card = null)
		{
			if (MultiplayerFx.Active)
			{
				Trap[] array = Plays.ToArray();
				foreach (Trap trap in array)
				{
					if (card == null || (trap.Card == card && MultiplayerFx.CurrentInvocation != null && trap.Invocation == MultiplayerFx.CurrentInvocation && trap.Room == GameRefs.CombatRoom))
					{
						CloseTrap(trap);
					}
				}
				Volley[] array2 = MultiplayerVolleys.ToArray();
				foreach (Volley volley in array2)
				{
					if (card == null || (volley.Card == card && MultiplayerFx.CurrentInvocation != null && volley.Invocation == MultiplayerFx.CurrentInvocation && volley.Room == GameRefs.CombatRoom))
					{
						CloseVolley(volley);
					}
				}
			}
			else if (MultiplayerFx.CurrentInvocation == null)
			{
				CloseVolley();
				CloseTrap(_s);
			}
		}

		private static void CloseTrap(Trap s)
		{
			if (s != null && !s.Dead && s.Stabbing && !(s.CloseAt >= 0f))
			{
				if (!s.Auto && s.Next < s.Expected)
				{
					s.Linger = s.Time + 2.5f;
				}
				else
				{
					s.CloseAt = s.Time;
				}
			}
		}

		public static void Abort()
		{
			Trap[] array = Live.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				Free(array[i]);
			}
			Live.Clear();
			Plays.Clear();
			_s = null;
			_volley = null;
			MultiplayerVolleys.Clear();
		}

		private static float Angle(Trap t, int i)
		{
			return (float)Math.PI * 2f * (float)i / (float)Math.Max(1, t.Blades.Length) - (float)Math.PI / 2f + t.Spin;
		}

		private static Vector2 RingPoint(Trap t, int i, float stab, float settle, float wind = 0f)
		{
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			float num = Angle(t, i);
			float num2 = t.Radius * (1f + 0.16f * (1f - settle) + 0.16f * wind * (1f - stab)) * (1f - 0.74f * stab * stab);
			return t.Centre + new Vector2(Mathf.Cos(num) * num2, Mathf.Sin(num) * num2 * 0.82f);
		}

		private static Vector2 PilePoint(Trap t, int i, out float rot)
		{
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			int num = t.Blades.Length;
			float num2 = Mathf.Min(13f, 110f / (float)Math.Max(1, num));
			float num3 = Mathf.DegToRad(-90f + ((float)i - (float)(num - 1) * 0.5f) * num2 - 12f);
			float num4 = 62f + 16f * (float)(i % 2) + 5f * Mathf.Sin(t.Time * 5.5f + (float)i * 1.3f);
			rot = num3 + (float)Math.PI / 2f + 0.05f * Mathf.Sin(t.Time * 11f + (float)i * 2.1f);
			return t.Pile + new Vector2(Mathf.Cos(num3), Mathf.Sin(num3)) * num4;
		}

		private static void Choose(Trap t, string why)
		{
			int num = t.Blades.Length;
			if (num != 0)
			{
				int num2 = t.Order[t.Next % num];
				t.Next++;
				if (why != null && PowerFxManager.LogTriggers)
				{
					DevLog.Info($"Knife Trap: blade {t.Next}/{t.Expected} ({why})");
				}
				t.Pending = num2;
				t.WindAt[num2] = t.Time;
				t.StabAt[num2] = -1f;
				t.Burst[num2] = false;
				t.Final[num2] = false;
				t.Seat[num2] = 1f;
				t.Fresh[num2] = false;
				t.FillDue[num2] = -1f;
				t.FillAt[num2] = -1f;
				if (t.Linger >= 0f)
				{
					t.Linger = t.Time + ((t.Next >= t.Expected) ? 0.8f : 2.5f);
				}
			}
		}

		private static bool Strike(Trap t, out Vector2 from)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			from = default;
			int pending = t.Pending;
			if (pending < 0 || pending >= t.Blades.Length)
			{
				return false;
			}
			if (t.StabAt[pending] >= 0f)
			{
				from = t.LastFrom;
				return true;
			}
			from = RingPoint(t, pending, 0f, 1f, Wind(t, pending));
			t.StabAt[pending] = t.Time;
			Vector2 val = t.Centre - from;
			Vector2 val2 = (Vector2)((val.LengthSquared() > 1f) ? val.Normalized() : new Vector2(1f, 0f));
			Vector2 val3 = t.Centre + val2 * (t.Radius * 1.25f + 90f);
			t.ThroughFrom[pending] = from;
			t.ThroughTo[pending] = val3;
			t.Burst[pending] = false;
			t.Final[pending] = (t.Auto ? (t.AutoNext >= t.Blades.Length) : (t.Next >= t.Expected && t.Reserve <= 0));
			Node val4 = Fx(t);
			if (val4 != null)
			{
				float inTime = t.PierceTime * 0.55f;
				Brush.Cut(val4, from, val3, 11f, new Color(Ember.R, Ember.G, Ember.B, 0.55f), 0f, inTime, 0.03f, 0.24f);
				Brush.Cut(val4, from, val3, 3.5f, new Color(1f, 0.95f, 0.86f, 0.9f), 0f, inTime, 0.02f, 0.16f);
			}
			if (t.Reserve > 0)
			{
				t.Reserve--;
				t.FillDue[pending] = t.Time + t.PierceTime;
			}
			t.LastFrom = from;
			t.HasLast = true;
			return true;
		}

		private static Node Fx(Trap t)
		{
			if (FxKit.Alive((GodotObject)(object)t.Front))
			{
				return t.Front;
			}
			Node val = (FxKit.Alive((GodotObject)(object)t.Holder) ? ((Node)t.Holder).GetParent() : null);
			if (!FxKit.Alive((GodotObject)(object)val))
			{
				return null;
			}
			return val;
		}

		private static void Impact(Trap t, int i, Vector2 at, Vector2 dir, bool final)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			Node val = Fx(t);
			if (val != null)
			{
				t.KickAt = t.Time;
				Blast(val, at, dir, final, Tuning.I(final ? "trap.final_shake" : "trap.shake", (!final) ? 1 : 2), t.Radius);
				if (final)
				{
					Sfx.Play("trap_final");
				}
			}
		}

		public static void Blast(Node fx, Vector2 at, Vector2 dir, bool final, int shake, float radius = 150f)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)fx))
			{
				Color color = new Color(1f, 0.8f, 0.52f, 0.95f);
				FxKit.Flash(fx, at, new Color(Ember.R, Ember.G, Ember.B, 0.85f), final ? 320f : 180f, final ? 2.2f : 1.9f, final ? 0.3 : 0.18);
				FxKit.Flash(fx, at, new Color(1f, 0.97f, 0.9f, 0.95f), final ? 130f : 72f, 2.2f, 0.12);
				FxKit.Burst(fx, at, color, final ? 34 : 18, 260f, final ? 1000f : 800f, 0.4, 0.05f, 0.17f, new Vector2(0f, 700f), final ? 180f : 55f, dir);
				Brush.Sparks(fx, at, Ember, final ? 14 : 8, 460f);
				int count = Math.Max(0, Tuning.I(final ? "trap.final_shards" : "trap.shards", final ? 12 : 5));
				Splinters(fx, at, dir, count, final ? 180f : 70f, final ? 1.25f : 1f);
				if (final)
				{
					FxKit.RingPulse(fx, at, radius * 2.2f, Ember, 0f, 0.45f, 0.35f, 1.3f, 0.82f, 0f, 0.05f, 0.95f);
					FxKit.RingPulse(fx, at, radius * 2.2f, new Color(1f, 0.95f, 0.86f, 1f), 0.04f, 0.4f, 0.3f, 1.15f, 0.82f, 0f, 0.022f);
				}
				Punch(dir, shake);
			}
		}

		internal static void Splinters(Node parent, Vector2 at, Vector2 dir, int count, float cone, float scale)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < count; i++)
			{
				bool flag = i % 3 == 2;
				float num = (flag ? ((float)Math.PI) : Mathf.DegToRad(cone));
				float num2 = dir.Angle() + (float)GD.RandRange((double)(0f - num), (double)num);
				float num3 = (float)GD.RandRange(700.0, 1350.0) * (flag ? 0.7f : 1f) * scale;
				Vector2 v = new Vector2(Mathf.Cos(num2), Mathf.Sin(num2)) * num3;
				float spin = (float)GD.RandRange(16.0, 34.0) * ((GD.Randf() < 0.5f) ? (-1f) : 1f);
				float num4 = (float)GD.RandRange(22.0, 40.0) * scale;
				float life = (float)GD.RandRange(0.45, 0.8);
				float r0 = num2 + (float)Math.PI / 2f;
				Node2D shard = FxKit.Blade(num4, num4 * 0.15f, new Color(0.98f, 0.96f, 0.92f, 1f), new Color(0.62f, 0.58f, 0.55f, 1f), new Color(0.3f, 0.2f, 0.16f, 1f), new Color(Ember.R, Ember.G, Ember.B, 0.55f));
				parent.AddChild((Node)(object)shard, false, (InternalMode)0);
				shard.GlobalPosition = at;
				shard.GlobalRotation = r0;
				shard.Scale = Vector2.Zero;
				FxKit.FreeAfter((Node)(object)shard, life + 0.15f);
				Vector2 fall = new Vector2(0f, 1500f);
				FxKit.Drive((Node)(object)shard, life, (float u) =>
				{
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0042: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					//IL_004d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0058: Unknown result type (might be due to invalid IL or missing references)
					//IL_005d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0063: Unknown result type (might be due to invalid IL or missing references)
					//IL_0069: Unknown result type (might be due to invalid IL or missing references)
					//IL_006e: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
					if (FxKit.Alive((GodotObject)(object)shard))
					{
						float num5 = u * life;
						float num6 = (1f - Mathf.Exp(-2.4f * num5)) / 2.4f;
						shard.GlobalPosition = at + v * num6 + 0.5f * fall * num5 * num5;
						shard.GlobalRotation = r0 + spin * num6;
						float num7 = Mathf.Min(1f, num5 / 0.04f);
						shard.Scale = new Vector2(num7, num7);
						((CanvasItem)shard).Modulate = new Color(1f, 1f, 1f, 1f - Mathf.SmoothStep(0.55f, 1f, u));
					}
				}, (TransitionType)0, (EaseType)2);
			}
		}

		private static void Punch(Vector2 dir, int strength)
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (strength > 0 && !(Tuning.F("fx.screen_shake", 1f) < 0.5f))
				{
					NGame instance = NGame.Instance;
					if (FxKit.Alive((GodotObject)(object)instance) && GameRefs.CombatRoom != null)
					{
						float num = Mathf.PosMod(dir.Angle() * 180f / (float)Math.PI, 360f);
						ShakeStrength val = (ShakeStrength)((strength >= 3) ? 4 : ((strength == 2) ? 3 : 2));
						instance.ScreenShake(val, (ShakeDuration)1, num);
					}
				}
			}
			catch
			{
			}
		}

		private static float Wind(Trap t, int i)
		{
			if (!(t.WindAt[i] < 0f))
			{
				return Mathf.Clamp((t.Time - t.WindAt[i]) / 0.1f, 0f, 1f);
			}
			return 0f;
		}

		private static void Update(double realDt)
		{
			if (Live.Count == 0)
			{
				return;
			}
			if (!MultiplayerFx.Active)
			{
				if (_volley != null && _volley.Closed && FxClock.Milliseconds - _volley.ClosedAt > 2500)
				{
					EndVolley();
				}
			}
			else
			{
				Volley[] array = MultiplayerVolleys.ToArray();
				foreach (Volley volley in array)
				{
					if (volley.Closed && FxClock.Milliseconds - volley.ClosedAt > 2500)
					{
						EndVolley(volley);
					}
				}
			}
			float dt = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				Trap trap = Live[num];
				try
				{
					if (trap.Scoped && !MultiplayerFx.Active)
					{
						Free(trap);
					}
					else if (!trap.Dead)
					{
						Step(trap, dt);
					}
				}
				catch (Exception ex)
				{
					DevLog.Error("Knife Trap failed and was removed: " + ex);
					Free(trap);
				}
				if (trap.Dead)
				{
					Live.RemoveAt(num);
				}
			}
		}

		private static void Step(Trap t, float dt)
		{
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Unknown result type (might be due to invalid IL or missing references)
			//IL_038b: Unknown result type (might be due to invalid IL or missing references)
			//IL_040d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aa6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_055c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0561: Unknown result type (might be due to invalid IL or missing references)
			//IL_0573: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_0594: Unknown result type (might be due to invalid IL or missing references)
			//IL_0599: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07df: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0603: Unknown result type (might be due to invalid IL or missing references)
			//IL_0605: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05de: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0812: Unknown result type (might be due to invalid IL or missing references)
			//IL_0809: Unknown result type (might be due to invalid IL or missing references)
			//IL_093c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0817: Unknown result type (might be due to invalid IL or missing references)
			//IL_081b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0835: Unknown result type (might be due to invalid IL or missing references)
			//IL_083a: Unknown result type (might be due to invalid IL or missing references)
			//IL_077f: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0667: Unknown result type (might be due to invalid IL or missing references)
			//IL_066e: Unknown result type (might be due to invalid IL or missing references)
			//IL_067d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0682: Unknown result type (might be due to invalid IL or missing references)
			//IL_0687: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_068f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0691: Unknown result type (might be due to invalid IL or missing references)
			//IL_0695: Unknown result type (might be due to invalid IL or missing references)
			//IL_069a: Unknown result type (might be due to invalid IL or missing references)
			//IL_069d: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0860: Unknown result type (might be due to invalid IL or missing references)
			//IL_0863: Unknown result type (might be due to invalid IL or missing references)
			//IL_0868: Unknown result type (might be due to invalid IL or missing references)
			//IL_086d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0871: Unknown result type (might be due to invalid IL or missing references)
			//IL_0896: Unknown result type (might be due to invalid IL or missing references)
			//IL_089b: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)t.Holder))
			{
				Free(t);
				return;
			}
			t.Time += dt;
			float time = t.Time;
			int num = t.Blades.Length;
			t.Spin += dt * Mathf.Clamp(Tuning.F("trap.spin", 0.4f) + Tuning.F("trap.spin_per_blade", 0.075f) * (float)num, 0.45f, 1.9f);
			Vector2 from;
			if (t.Stabbing)
			{
				t.StabClock += dt;
				if (t.Auto)
				{
					while (t.AutoNext < num && t.StabClock >= 0.45f + (float)t.AutoNext * (0.45f / (float)Math.Max(1, num)))
					{
						t.AutoNext++;
						Choose(t, null);
						t.WindAt[t.Pending] = -1f;
						Strike(t, out from);
						t.Pending = -1;
					}
					if (t.CloseAt < 0f && t.StabClock >= t.AutoTotal)
					{
						t.CloseAt = time;
					}
				}
				if (t.CloseAt < 0f && t.Linger >= 0f && time >= t.Linger)
				{
					t.CloseAt = time;
				}
				if (t.CloseAt < 0f && t.StabClock >= 14f)
				{
					t.CloseAt = time;
				}
			}
			float num2 = ((t.CloseAt < 0f) ? 0f : Mathf.Clamp((time - t.CloseAt) / 0.3f, 0f, 1f));
			t.Centre = t.Centre.Lerp(t.CentreGoal, 1f - Mathf.Exp(-12f * dt));
			float num3 = 0f;
			float num4 = 0f;
			for (int i = 0; i < num; i++)
			{
				if (time - t.AimedAt >= (float)i * 0.025f || t.Stabbing)
				{
					t.Seat[i] = FxKit.Damp(t.Seat[i], t.Aimed ? 1f : 0f, 10f, dt);
				}
				num3 += t.Seat[i];
				num4 = Mathf.Max(num4, t.Seat[i]);
			}
			float num5;
			if (num > 0)
			{
				num5 = num3 / (float)num;
			}
			else
			{
				num5 = (t.Aimed ? 1f : 0f);
			}
			if (num == 0)
			{
				num4 = num5;
			}
			if (t.Dismissed && num4 < 0.06f)
			{
				t.Sink = Mathf.Min(1f, t.Sink + dt / 0.22f);
			}
			float num6 = (1f - num2) * (1f - t.Sink);
			float num7 = Mathf.Clamp(Tuning.F("trap.refill_time", 0.24f), 0.08f, 0.8f);
			for (int j = 0; j < num; j++)
			{
				Node2D val = t.Blades[j];
				if (!FxKit.Alive((GodotObject)(object)val))
				{
					continue;
				}
				if (t.Volley && j < t.Fired)
				{
					if (((CanvasItem)val).Visible)
					{
						((CanvasItem)val).Visible = false;
					}
					continue;
				}
				if (t.FillDue[j] >= 0f && time >= t.FillDue[j])
				{
					if (!t.Burst[j] && t.StabAt[j] >= 0f)
					{
						t.Burst[j] = true;
						Vector2 val2 = t.ThroughTo[j] - t.ThroughFrom[j];
						if (num6 > 0.05f)
						{
							Impact(t, j, t.Centre, (Vector2)((val2.LengthSquared() > 1f) ? val2.Normalized() : new Vector2(1f, 0f)), t.Final[j]);
						}
					}
					t.FillDue[j] = -1f;
					t.Fresh[j] = true;
					t.FillAt[j] = time;
					if (!t.Remote && num6 > 0.3f)
					{
						FxKit.Flash((Node)(object)t.Holder, t.Pile, new Color(Ember.R, Ember.G, Ember.B, 0.45f), 60f, 1.6f, 0.14);
					}
				}
				bool b = t.Fresh[j];
				float num8 = 1f;
				if (b && t.FillAt[j] >= 0f)
				{
					num8 = Mathf.Clamp((time - t.FillAt[j]) / num7, 0f, 1f);
				}
				float num9 = Mathf.Clamp((time - (float)j * 0.03f) / 0.14f, 0f, 1f);
				bool flag = !b && t.StabAt[j] >= 0f;
				float num10 = (flag ? Mathf.Clamp((time - t.StabAt[j]) / t.PierceTime, 0f, 1f) : 0f);
				if (t.StabAt[j] >= 0f)
				{
					num9 = 1f;
				}
				float num11 = Mathf.SmoothStep(0f, 1f, t.Seat[j]);
				float num12 = (b ? 0f : Wind(t, j));
				Vector2 val3 = RingPoint(t, j, 0f, t.FromPile ? num11 : num9, num12);
				float num13 = Angle(t, j) - (float)Math.PI / 2f;
				Vector2 val4 = val3;
				float globalRotation = num13;
				if (t.FromPile && num11 < 0.999f)
				{
					Vector2 val5 = PilePoint(t, j, out var rot);
					val5 = t.Pile.Lerp(val5, num9 * (1f - t.Sink));
					if (t.Placed)
					{
						val4 = FxKit.Bezier(b: val5.Lerp(val3, 0.5f) + new Vector2(0f, -110f), a: val5, c: val3, t: num11);
						globalRotation = Mathf.LerpAngle(rot, num13, num11);
					}
					else
					{
						val4 = val5;
						globalRotation = rot;
					}
				}
				float num14 = 1f;
				float num15 = 1f;
				if (b && t.FillAt[j] >= 0f)
				{
					if (num8 < 1f)
					{
						float num16 = 1f - (1f - num8) * (1f - num8);
						if (!t.Remote)
						{
							Vector2 b2 = t.Pile.Lerp(val3, 0.5f) + new Vector2(0f, -90f);
							val4 = FxKit.Bezier(t.Pile, b2, val3, num16);
							Vector2 val6 = FxKit.Bezier(t.Pile, b2, val3, Mathf.Min(1f, num16 + 0.05f)) - val4;
							globalRotation = Mathf.LerpAngle((val6.LengthSquared() > 1f) ? (val6.Angle() + (float)Math.PI / 2f) : num13, num13, Mathf.SmoothStep(0.55f, 1f, num8));
						}
						num14 = Mathf.Lerp(0.6f, 1f, num16) * (1f + 0.3f * Mathf.Sin((float)Math.PI * Mathf.Clamp((num8 - 0.7f) / 0.3f, 0f, 1f)));
						num15 = Mathf.Clamp(num8 / 0.2f, 0f, 1f);
					}
					else
					{
						t.FillAt[j] = -1f;
						if (num6 > 0.3f)
						{
							FxKit.Flash((Node)(object)t.Holder, val3, new Color(Ember.R, Ember.G, Ember.B, 0.55f), 54f, 1.9f, 0.16);
						}
					}
				}
				if (flag)
				{
					Vector2 val7 = t.ThroughFrom[j];
					Vector2 val8 = t.ThroughTo[j];
					Vector2 val9 = val8 - val7;
					Vector2 val10 = (Vector2)((val9.LengthSquared() > 1f) ? val9.Normalized() : new Vector2(1f, 0f));
					val4 = val7.Lerp(val8, 1f - Mathf.Pow(1f - num10, 1.7f));
					globalRotation = val10.Angle() + (float)Math.PI / 2f;
					num14 *= 1.15f;
					if (!t.Burst[j])
					{
						from = val4 - t.Centre;
						if (from.Dot(val10) >= 0f)
						{
							t.Burst[j] = true;
							if (num6 > 0.05f)
							{
								Impact(t, j, t.Centre, val10, t.Final[j]);
							}
						}
					}
				}
				val.GlobalPosition = val4;
				val.GlobalRotation = globalRotation;
				float num17 = num9 * (1f - t.Sink) * (1f + 0.18f * num12 * (1f - num10)) * num14;
				val.Scale = new Vector2(num17, num17);
				((CanvasItem)val).Modulate = new Color(1f, 1f, 1f, num9 * (1f - Mathf.SmoothStep(0.7f, 1f, num10)) * num6 * num15 * (t.Remote ? num11 : 1f));
			}
			float num18 = ((t.Remote && !t.Stabbing) ? 0f : 1f);
			if (FxKit.Alive((GodotObject)(object)t.Veil))
			{
				t.Veil.Color = new Color(0.02f, 0.01f, 0.04f, Tuning.F("trap.veil", 0.5f) * num5 * num6 * num18);
			}
			if (FxKit.Alive((GodotObject)(object)t.Sigil) && FxKit.Alive((GodotObject)(object)t.Mat))
			{
				float num19 = ((t.KickAt < 0f) ? 0f : Mathf.Exp(-9f * (time - t.KickAt)));
				float num20 = t.Radius * 2.7f * (1f + 0.1f * num19);
				t.Sigil.Scale = new Vector2(num20 / 100f, num20 / 100f * 0.3f);
				t.Sigil.GlobalPosition = t.Feet;
				t.Mat.SetShaderParameter(PAlpha, Variant.op_Implicit(Mathf.Min(1f, 0.85f * num5 * num6 * num18 + 0.6f * num19 * num6 * num18)));
				t.Mat.SetShaderParameter(PSpin, Variant.op_Implicit(t.Spin * 1.8f));
				t.Mat.SetShaderParameter(PInnerSpin, Variant.op_Implicit((0f - t.Spin) * 2.8f));
			}
			if (FxKit.Alive((GodotObject)(object)t.PileGlow))
			{
				float num21 = (0.4f + 0.12f * Mathf.Sin(time * 7f)) * (1f - 0.75f * num5) * Mathf.Min(1f, time / 0.15f) * num6;
				((CanvasItem)t.PileGlow).Modulate = new Color(Ember.R, Ember.G, Ember.B, num21);
			}
			if (num2 >= 1f || t.Sink >= 1f)
			{
				Free(t);
			}
		}

		internal static void Free(Trap t)
		{
			if (t != null && !t.Dead)
			{
				t.Dead = true;
				if (FxKit.Alive((GodotObject)(object)t.Holder))
				{
					((Node)t.Holder).QueueFree();
				}
				if (FxKit.Alive((GodotObject)(object)t.Sigil))
				{
					((Node)t.Sigil).QueueFree();
				}
				if (FxKit.Alive((GodotObject)(object)t.Veil))
				{
					((Node)t.Veil).QueueFree();
				}
				Plays.Remove(t);
				if (_s == t)
				{
					_s = null;
				}
			}
		}

		private static int[] MakeOrder(int n)
		{
			if (n <= 0)
			{
				return Array.Empty<int>();
			}
			int num = Math.Max(1, n / 2 + 1);
			while (num > 1 && Gcd(num, n) != 1)
			{
				num--;
			}
			int[] array = new int[n];
			for (int i = 0; i < n; i++)
			{
				array[i] = i * num % n;
			}
			return array;
		}

		private static int Gcd(int a, int b)
		{
			while (b != 0)
			{
				int num = b;
				b = a % b;
				a = num;
			}
			return a;
		}

		internal static int CountSpentShivs(CardModel card)
		{
			try
			{
				Player val = ((card != null) ? card.Owner : null);
				if (val == null)
				{
					return 0;
				}
				CardPile pile = PileTypeExtensions.GetPile((PileType)4, val);
				if (pile == null)
				{
					return 0;
				}
				int num = 0;
				foreach (CardModel card2 in pile.Cards)
				{
					if (card2 != null && card2.Tags != null && card2.Tags.Contains((CardTag)5))
					{
						num++;
					}
				}
				return num;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Knife Trap: could not count spent Shivs: " + ex.Message);
				return 0;
			}
		}

		static KnifeTrapFx()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		}
	}
