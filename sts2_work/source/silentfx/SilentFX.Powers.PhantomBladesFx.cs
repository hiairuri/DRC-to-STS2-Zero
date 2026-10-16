// namespace: SilentFX.Powers
// type: PhantomBladesFx (class)
// source lines: 10116-11392
	public sealed class PhantomBladesFx : PowerFx
	{
		private sealed class Slot
		{
			public CardModel Card;

			public Node2D Node;

			public Sprite2D Aura;

			public bool Ink;

			public Vector2 Pos;

			public float Rot;

			public bool Placed;

			public float Age;

			public float Charge;

			public float Hover;
		}

		internal sealed class RemoteLoan
		{
			internal PhantomBladesFx Fx;

			internal CardModel Card;

			internal Node2D Blade;

			internal object Room;

			internal bool Closed;
		}

		private readonly List<FxTarget> _targetScratch = new List<FxTarget>();

		private const int Max = 10;

		private const string AuraName = "Charge";

		private const float HoverTime = 0.12f;

		private static readonly Color Blue = new Color(0.45f, 0.62f, 1f, 1f);

		private static readonly Color InkGlow = new Color(0.64f, 0.36f, 1f, 1f);

		private static readonly List<PhantomBladesFx> Live = new List<PhantomBladesFx>();

		private static readonly Dictionary<CardModel, Vector2> ThrownFrom = new Dictionary<CardModel, Vector2>(ReferenceEqualityComparer.Instance);

		private Node2D _back;

		private readonly List<Slot> _slots = new List<Slot>();

		private readonly List<CardModel> _hand = new List<CardModel>();

		private readonly HashSet<CardModel> _lent = new HashSet<CardModel>(ReferenceEqualityComparer.Instance);

		private readonly Dictionary<CardModel, RemoteLoan> _remoteLoans = new Dictionary<CardModel, RemoteLoan>(ReferenceEqualityComparer.Instance);

		private object _room;

		private readonly HashSet<CardModel> _wasInDraw = new HashSet<CardModel>(ReferenceEqualityComparer.Instance);

		private Creature _owner;

		private float _time;

		private float _flare;

		private float _alpha;

		private float _alphaTarget = 1f;

		private bool _stopping;

		private bool _dead;

		private bool RemoteOwner
		{
			get
			{
				if (MultiplayerFx.Active && _owner != null)
				{
					return !MultiplayerFx.IsLocal(_owner);
				}
				return false;
			}
		}

		public override bool IsAlive
		{
			get
			{
				if (!_dead)
				{
					return FxKit.Alive((GodotObject)(object)_back);
				}
				return false;
			}
		}

		private static Node2D MakeBlade(bool ink, out Sprite2D aura)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			float num = Tuning.F("phantom.length", 64f);
			float num2 = Tuning.F("phantom.halfwidth", 8.5f);
			Color hilt = new Color(0f, 0f, 0f, 0f);
			Node2D val = (ink ? FxKit.Blade(num, num2, new Color(0.72f, 0.5f, 1f, 0.9f), new Color(0.26f, 0.12f, 0.5f, 0.88f), hilt, new Color(InkGlow.R, InkGlow.G, InkGlow.B, 0.52f)) : FxKit.Blade(num, num2, new Color(0.82f, 0.9f, 1f, 0.72f), new Color(0.5f, 0.58f, 1f, 0.6f), hilt, new Color(0.42f, 0.52f, 1f, 0.34f)));
			Color val2 = (ink ? InkGlow : Blue);
			aura = FxKit.Glow(new Color(Mathf.Lerp(val2.R, 1f, 0.35f), Mathf.Lerp(val2.G, 1f, 0.35f), Mathf.Lerp(val2.B, 1f, 0.35f), 0f), new Vector2(num2 * 11f, num * 2.1f));
			((Node)aura).Name = StringName.op_Implicit("Charge");
			((Node2D)aura).Position = new Vector2(0f, (0f - num) * 0.45f);
			((Node)val).AddChild((Node)(object)aura, false, (InternalMode)0);
			((Node)val).MoveChild((Node)(object)aura, 0);
			return val;
		}

		private static bool IsInk(CardModel card)
		{
			try
			{
				return ((card != null) ? card.Enchantment : null) is Inky;
			}
			catch
			{
				return false;
			}
		}

		private void Stain(Slot slot, bool ink)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			slot.Ink = ink;
			Node2D old = slot.Node;
			if (!FxKit.Alive((GodotObject)(object)old) || !FxKit.Alive((GodotObject)(object)_back))
			{
				return;
			}
			Node2D val = MakeBlade(ink, out var aura);
			((Node)_back).AddChild((Node)(object)val, false, (InternalMode)0);
			try
			{
				((Node)_back).MoveChild((Node)(object)val, ((Node)old).GetIndex(false));
			}
			catch
			{
			}
			val.GlobalPosition = old.GlobalPosition;
			val.Rotation = old.Rotation;
			val.Scale = old.Scale;
			((CanvasItem)val).Modulate = new Color(1f, 1f, 1f, 0f);
			slot.Node = val;
			slot.Aura = aura;
			slot.Age = Mathf.Min(slot.Age, 0.35f);
			Tween val2 = ((Node)old).CreateTween();
			val2.TweenProperty((GodotObject)(object)old, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.2);
			val2.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)old))
				{
					((Node)old).QueueFree();
				}
			})));
			FxKit.FreeAfter((Node)(object)old, 0.5);
			if (ink)
			{
				float num = Tuning.F("phantom.length", 64f);
				Vector2 val3 = new Vector2(0f, -1f);
				Vector2 val4 = val3.Rotated(old.GlobalRotation);
				Vector2 globalPosition = old.GlobalPosition;
				Vector2 to = globalPosition + val4 * num;
				Brush.Cut((Node)(object)_back, globalPosition, to, 22f, new Color(InkGlow.R, InkGlow.G, InkGlow.B, 0.85f), 0f, 0.1f, 0.04f, 0.26f);
				FxKit.Flash((Node)(object)_back, globalPosition + val4 * num * 0.5f, new Color(InkGlow.R, InkGlow.G, InkGlow.B, 0.55f), 70f, 1.7f, 0.26);
			}
		}

		private Slot MakeSlot(CardModel card, bool ink)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			Slot slot = new Slot
			{
				Card = card,
				Ink = ink
			};
			slot.Node = MakeBlade(ink, out slot.Aura);
			((CanvasItem)slot.Node).Modulate = new Color(1f, 1f, 1f, 0f);
			((Node)_back).AddChild((Node)(object)slot.Node, false, (InternalMode)0);
			return slot;
		}

		public override void Start(bool animateIn)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected Obj, but got Unknown
			try
			{
				_back = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_PhantomBlades")
				};
				Anchor.BackParent.AddChild((Node)(object)_back, false, (InternalMode)0);
				object owner = Anchor.Owner;
				_owner = (Creature)((owner is Creature) ? owner : null);
				_room = GameRefs.CombatRoom;
				Live.Add(this);
				if (_owner == null)
				{
					for (int i = 0; i < 3; i++)
					{
						_slots.Add(MakeSlot(null, i == 2));
					}
				}
				_alpha = (animateIn ? 0f : 1f);
				_flare = (animateIn ? 1f : 0f);
				Sync();
				Layout(0f);
			}
			catch (Exception ex)
			{
				DevLog.Warn("PhantomBladesFx.Start: " + ex);
				Dispose();
			}
		}

		private Slot Find(CardModel card)
		{
			if (card == null)
			{
				return null;
			}
			foreach (Slot slot in _slots)
			{
				if (slot.Card == card)
				{
					return slot;
				}
			}
			return null;
		}

		private static PileType PileOf(CardModel card)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
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
			catch
			{
				return (PileType)0;
			}
		}

		private void Sync()
		{
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Invalid comparison between Unknown and I4
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Invalid comparison between Unknown and I4
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			Creature owner = _owner;
			Player val = ((owner != null) ? owner.Player : null);
			if (val == null)
			{
				return;
			}
			_hand.Clear();
			try
			{
				CardPile pile = PileTypeExtensions.GetPile((PileType)2, val);
				if (pile != null)
				{
					foreach (CardModel card in pile.Cards)
					{
						if (card is Shiv)
						{
							_hand.Add(card);
						}
					}
				}
			}
			catch
			{
				return;
			}
			for (int num = _slots.Count - 1; num >= 0; num--)
			{
				Slot slot = _slots[num];
				if (slot.Card != null)
				{
					PileType val2 = PileOf(slot.Card);
					if ((int)val2 != 2 && (int)val2 != 5)
					{
						_slots.RemoveAt(num);
						Depart(slot, val2);
					}
				}
			}
			if (_lent.Count > 0)
			{
				_lent.RemoveWhere((CardModel card) =>
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_0009: Invalid comparison between Unknown and I4
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Invalid comparison between Unknown and I4
					PileType val3 = PileOf(card);
					int num2;
					if ((int)val3 != 2)
					{
						num2 = (((int)val3 != 5) ? 1 : 0);
						if (num2 != 0 && _remoteLoans.Remove(card, out var value))
						{
							value.Closed = true;
							if (FxKit.Alive((GodotObject)(object)value.Blade))
							{
								((Node)value.Blade).QueueFree();
							}
						}
					}
					else
					{
						num2 = 0;
					}
					return (byte)num2 != 0;
				});
			}
			foreach (CardModel item in _hand)
			{
				if (_slots.Count >= 10)
				{
					break;
				}
				if (!_lent.Contains(item) && Find(item) == null)
				{
					Slot slot2 = MakeSlot(item, IsInk(item));
					if (_wasInDraw.Contains(item) && !RemoteOwner)
					{
						slot2.Pos = GameRefs.PilePoint((PileType)1);
						slot2.Rot = (float)Math.PI / 4f;
						slot2.Placed = true;
						slot2.Age = 1f;
					}
					_slots.Add(slot2);
				}
			}
			if (_slots.Count > 1)
			{
				_slots.Sort((Slot a, Slot b) => _hand.IndexOf(a.Card).CompareTo(_hand.IndexOf(b.Card)));
			}
			_wasInDraw.Clear();
			try
			{
				CardPile pile2 = PileTypeExtensions.GetPile((PileType)1, val);
				if (pile2 == null)
				{
					return;
				}
				foreach (CardModel card2 in pile2.Cards)
				{
					if (card2 is Shiv)
					{
						_wasInDraw.Add(card2);
					}
				}
			}
			catch
			{
			}
		}

		private void Depart(Slot slot, PileType where)
		{
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Invalid comparison between Unknown and I4
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Invalid comparison between Unknown and I4
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Invalid comparison between Unknown and I4
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			Node2D node = slot.Node;
			Node frontParent = Anchor.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)node))
			{
				return;
			}
			if (!FxKit.Alive((GodotObject)(object)frontParent) || !((Node)node).IsInsideTree())
			{
				((Node)node).QueueFree();
				return;
			}
			Adopt(node, frontParent);
			FxKit.FreeAfter((Node)(object)node, Mathf.Max(0.6f, Tuning.F("phantom.burn_time", 0.6f)) + 0.6f);
			if (((int)where == 3 || (int)where == 1) && !RemoteOwner)
			{
				FlyTo(node, GameRefs.PilePoint(where), 0.34f, shrink: true);
				return;
			}
			if ((int)where == 4)
			{
				Burn(node, frontParent);
				return;
			}
			Tween val = ((Node)node).CreateTween();
			val.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.22);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)node))
				{
					((Node)node).QueueFree();
				}
			})));
		}

		private static void Adopt(Node2D node, Node parent)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			Vector2 globalPosition = node.GlobalPosition;
			float globalRotation = node.GlobalRotation;
			Node parent2 = ((Node)node).GetParent();
			if (parent2 != null)
			{
				parent2.RemoveChild((Node)(object)node);
			}
			parent.AddChild((Node)(object)node, false, (InternalMode)0);
			node.GlobalPosition = globalPosition;
			node.GlobalRotation = globalRotation;
		}

		private static void FlyTo(Node2D node, Vector2 to, float time, bool shrink)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			Vector2 from = node.GlobalPosition;
			float startRot = node.GlobalRotation;
			Vector2 mid = from.Lerp(to, 0.5f) + new Vector2(0f, -80f);
			Vector2 last = from;
			FxKit.Drive((Node)(object)node, time, (float t) =>
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_007c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)node))
				{
					Vector2 val = FxKit.Bezier(from, mid, to, t);
					Vector2 val2 = val - last;
					if (val2.LengthSquared() > 0.01f)
					{
						node.Rotation = Mathf.LerpAngle(startRot, val2.Angle() + (float)Math.PI / 2f, Mathf.Min(1f, t * 4f));
					}
					node.GlobalPosition = val;
					if (shrink)
					{
						float num = 1f - 0.55f * t;
						node.Scale = new Vector2(num, num);
						((CanvasItem)node).Modulate = new Color(1f, 1f, 1f, 1f - t * t);
					}
					last = val;
				}
			}, (TransitionType)4, (EaseType)0).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)node))
				{
					((Node)node).QueueFree();
				}
			})));
		}

		private static void Burn(Node2D node, Node parent)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected Obj, but got Unknown
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Expected Obj, but got Unknown
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			ParticleProcessMaterial val = new ParticleProcessMaterial();
			val.EmissionShape = (EmissionShapeEnum)3;
			val.EmissionBoxExtents = new Vector3(7f, 26f, 0f);
			val.Direction = new Vector3(0f, -1f, 0f);
			val.Spread = 18f;
			val.InitialVelocityMin = 50f;
			val.InitialVelocityMax = 130f;
			val.Gravity = new Vector3(0f, -160f, 0f);
			val.ScaleMin = 0.14f;
			val.ScaleMax = 0.34f;
			val.Color = new Color(1f, 1f, 1f, 0.9f);
			val.ColorRamp = (Texture2D)(object)FxKit.Ramp(new float[4] { 0f, 0.3f, 0.7f, 1f }, new Color[4]
			{
				new Color(1f, 0.95f, 0.7f, 0.95f),
				new Color(1f, 0.62f, 0.22f, 0.9f),
				new Color(0.95f, 0.3f, 0.12f, 0.5f),
				new Color(0.4f, 0.1f, 0.05f, 0f)
			});
			val.ParticleFlagDisableZ = true;
			ParticleProcessMaterial processMaterial = val;
			GpuParticles2D flames = new GpuParticles2D
			{
				Amount = 18,
				Lifetime = 0.45,
				Randomness = 0.6f,
				LocalCoords = false,
				Texture = FxKit.SoftDot,
				ProcessMaterial = (Material)(object)processMaterial,
				Material = (Material)(object)FxKit.Additive,
				Position = new Vector2(0f, -28f),
				VisibilityRect = new Rect2(-300f, -500f, 600f, 800f)
			};
			((Node)node).AddChild((Node)(object)flames, false, (InternalMode)0);
			Vector2 from = node.GlobalPosition;
			float seconds = Tuning.F("phantom.burn_time", 0.6f);
			Tween val2 = FxKit.Drive((Node)(object)node, seconds, (float t) =>
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)node))
				{
					node.GlobalPosition = from + new Vector2(Mathf.Sin(t * 9f) * 3f, -46f * t);
					((CanvasItem)node).Modulate = new Color(1f + 0.6f * t, 1f - 0.45f * t, 1f - 0.85f * t, 1f - t * t);
					if (t > 0.65f && FxKit.Alive((GodotObject)(object)flames) && flames.Emitting)
					{
						flames.Emitting = false;
					}
				}
			}, (TransitionType)0, (EaseType)2);
			val2.TweenInterval(0.5);
			val2.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)node))
				{
					((Node)node).QueueFree();
				}
			})));
			FxKit.Flash(parent, from + new Vector2(0f, -28f), new Color(1f, 0.62f, 0.25f, 0.6f), 80f, 2f);
		}

		public override void Update(double dt)
		{
			try
			{
				if (_dead)
				{
					return;
				}
				if (!Anchor.Valid() || !FxKit.Alive((GodotObject)(object)_back) || !((Node)_back).IsInsideTree())
				{
					Dispose();
					return;
				}
				_time += (float)dt;
				_alpha = FxKit.Damp(_alpha, _alphaTarget, 4.5f, dt);
				_flare = FxKit.Damp(_flare, 0f, 2.2f, dt);
				if (!_stopping)
				{
					Sync();
				}
				Layout((float)dt);
				if (_stopping && _alpha < 0.03f)
				{
					Dispose();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PhantomBladesFx.Update: " + ex);
				Dispose();
			}
		}

		private void Place(int index, int count, out Vector2 pos, out float rot)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = Anchor.SafeBodySize();
			Vector2 val2 = Anchor.Chest() + new Vector2((0f - val.X) * Tuning.F("phantom.origin_x", 0.16f) * Anchor.Side(), val.Y * Tuning.F("phantom.origin_y", 0.17f));
			float num = Mathf.DegToRad(Tuning.F("phantom.fan_degrees", 120f)) * Mathf.Clamp((float)count / 5f, 0.2f, 1f) * (1f + 0.1f * _flare);
			float num2 = val.Y * Tuning.F("phantom.radius_mul", 0.16f) + Tuning.F("phantom.radius_add", 30f) + Mathf.Sin(_time * 1.3f + (float)index * 0.9f) * 6f + 14f * _flare;
			float num3 = Mathf.DegToRad(Tuning.F("phantom.lean_degrees", 30f));
			float num4 = (((float)index + 0.5f) / (float)Mathf.Max(1, count) - 0.5f) * num - num3;
			pos = val2 + new Vector2(Mathf.Sin(num4) * Anchor.Side(), 0f - Mathf.Cos(num4)) * num2;
			rot = num4 * Anchor.Side();
		}

		private static CardModel Rested()
		{
			try
			{
				return HeldHooks.Hovered();
			}
			catch
			{
				return null;
			}
		}

		private void Layout(float dt)
		{
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)_back))
			{
				return;
			}
			bool flag = ((_owner == null) ? (Mathf.Sin(_time * 0.8f) > -0.3f) : ShivFx.FirstShivPendingFor(_owner.Player));
			float num = 1f - Mathf.Exp(-9f * dt);
			int count = _slots.Count;
			CardModel val = ((_owner == null || _stopping || RemoteOwner) ? null : Rested());
			float num2 = Tuning.F("phantom.length", 64f) * Tuning.F("phantom.hover_rise", 0.2f);
			float num3 = Mathf.Clamp(Tuning.F("phantom.hover_up", 0f), 0f, 1f);
			float num4 = Tuning.F("phantom.hover_glow", 0.3f);
			for (int i = 0; i < count; i++)
			{
				Slot slot = _slots[i];
				if (!FxKit.Alive((GodotObject)(object)slot.Node))
				{
					continue;
				}
				if (slot.Card != null)
				{
					bool flag2 = IsInk(slot.Card);
					if (flag2 != slot.Ink)
					{
						Stain(slot, flag2);
					}
				}
				Place(i, count, out var pos, out var rot);
				if (!slot.Placed)
				{
					slot.Pos = pos;
					slot.Rot = rot;
					slot.Placed = true;
				}
				else
				{
					slot.Pos = slot.Pos.Lerp(pos, num);
					slot.Rot = Mathf.LerpAngle(slot.Rot, rot, num);
				}
				slot.Age = Mathf.Min(1f, slot.Age + dt / 0.18f);
				slot.Charge = FxKit.Damp(slot.Charge, flag ? 1f : 0f, flag ? 5f : 9f, dt);
				bool flag3 = val != null && slot.Card == val;
				slot.Hover = Mathf.Clamp(slot.Hover + (flag3 ? dt : (0f - dt)) / 0.12f, 0f, 1f);
				float num5 = slot.Hover * slot.Hover * (3f - 2f * slot.Hover);
				float num6 = slot.Age * (1f + 0.1f * slot.Charge);
				Vector2 val2 = new Vector2(0f, -1f);
				val2 = val2.Rotated(slot.Rot);
				val2 = val2.Lerp(new Vector2(0f, -1f), num3);
				Vector2 val3 = val2.Normalized();
				slot.Node.GlobalPosition = slot.Pos + val3 * (num2 * num6 * num5);
				slot.Node.Rotation = slot.Rot;
				slot.Node.Scale = new Vector2(num6, num6);
				float num7 = 0.8f + 0.2f * Mathf.Sin(_time * 2.3f + (float)i * 1.7f);
				float num8 = Mathf.Clamp(_alpha * slot.Age * (num7 + 0.6f * _flare + 0.25f * slot.Charge + num4 * num5), 0f, 1.3f + num4 * num5);
				((CanvasItem)slot.Node).Modulate = new Color(1f, 1f, 1f, num8);
				if (FxKit.Alive((GodotObject)(object)slot.Aura))
				{
					Color modulate = ((CanvasItem)slot.Aura).Modulate;
					float num9 = 0.55f + 0.3f * Mathf.Sin(_time * 7f + (float)i * 2.1f);
					((CanvasItem)slot.Aura).Modulate = new Color(modulate.R, modulate.G, modulate.B, Tuning.F("phantom.charge", 0.75f) * slot.Charge * num9);
				}
			}
		}

		public override void Trigger(Vector2? at = null)
		{
			if (!_dead && !_stopping)
			{
				_flare = 1f;
			}
		}

		private static PhantomBladesFx For(CardModel card)
		{
			if (card == null)
			{
				return null;
			}
			foreach (PhantomBladesFx item in Live)
			{
				if (!item._dead && !item._stopping && (item.Find(card) != null || item._lent.Contains(card)) && (!MultiplayerFx.Active || MultiplayerFx.IsLocal(card) || item.ValidRemote(card)))
				{
					return item;
				}
			}
			return null;
		}

		public static bool TryLend(CardModel card, out Node2D blade)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			blade = null;
			PhantomBladesFx phantomBladesFx = For(card);
			Slot slot = phantomBladesFx?.Find(card);
			if (slot == null || !FxKit.Alive((GodotObject)(object)slot.Node))
			{
				return false;
			}
			phantomBladesFx._slots.Remove(slot);
			phantomBladesFx._lent.Add(card);
			blade = slot.Node;
			((CanvasItem)blade).Modulate = Colors.White;
			return true;
		}

		public static void GiveBack(CardModel card, Node2D blade)
		{
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)blade))
			{
				PhantomBladesFx phantomBladesFx = For(card);
				if (phantomBladesFx == null || !FxKit.Alive((GodotObject)(object)phantomBladesFx._back) || !((Node)blade).IsInsideTree())
				{
					((Node)blade).QueueFree();
					return;
				}
				phantomBladesFx._lent.Remove(card);
				Adopt(blade, (Node)(object)phantomBladesFx._back);
				phantomBladesFx._slots.Add(new Slot
				{
					Card = card,
					Node = blade,
					Aura = ((Node)blade).GetNodeOrNull<Sprite2D>(NodePath.op_Implicit("Charge")),
					Ink = (card.Enchantment is Inky),
					Pos = blade.GlobalPosition,
					Rot = blade.GlobalRotation,
					Placed = true,
					Age = 1f
				});
			}
		}

		public static void OnShivPlayed(CardModel card, Vector2? target)
		{
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
			if (MultiplayerFx.Active && !MultiplayerFx.IsLocal(card) && !target.HasValue)
			{
				return;
			}
			PhantomBladesFx phantomBladesFx = For(card);
			Slot slot = phantomBladesFx?.Find(card);
			if (slot == null)
			{
				return;
			}
			phantomBladesFx._slots.Remove(slot);
			Node2D node = slot.Node;
			Node frontParent = phantomBladesFx.Anchor.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)node) || !FxKit.Alive((GodotObject)(object)frontParent) || !((Node)node).IsInsideTree())
			{
				if (FxKit.Alive((GodotObject)(object)node))
				{
					((Node)node).QueueFree();
				}
				return;
			}
			Vector2 from = node.GlobalPosition;
			Vector2 to;
			if (target.HasValue)
			{
				to = target.Value;
			}
			else
			{
				phantomBladesFx.Anchor.SafeTargets(phantomBladesFx._targetScratch);
				List<FxTarget> targetScratch = phantomBladesFx._targetScratch;
				to = ((targetScratch.Count > 0) ? targetScratch[0].Chest : (from + new Vector2(600f * phantomBladesFx.Anchor.Side(), 0f)));
			}
			if (ThrownFrom.Count > 32)
			{
				ThrownFrom.Clear();
			}
			ThrownFrom[card] = from;
			Adopt(node, frontParent);
			FxKit.FreeAfter((Node)(object)node, Mathf.Max(0f, Tuning.F("phantom.throw_time", 0.1f)) + 0.1f);
			((CanvasItem)node).Modulate = Colors.White;
			Vector2 val = to - from;
			if (val.LengthSquared() > 1f)
			{
				Node2D val2 = node;
				val = to - from;
				val2.Rotation = val.Angle() + (float)Math.PI / 2f;
			}
			Line2D trail = FxKit.Ribbon(10f, new Color(0.62f, 0.76f, 1f, 0.75f));
			frontParent.AddChild((Node)(object)trail, false, (InternalMode)0);
			FxKit.FreeAfter((Node)(object)trail, 0.3799999952316284);
			FxKit.Drive((Node)(object)node, Tuning.F("phantom.throw_time", 0.1f), (float t) =>
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)node))
				{
					Vector2 val4 = from.Lerp(to, t);
					node.GlobalPosition = val4;
					if (FxKit.Alive((GodotObject)(object)trail))
					{
						trail.AddPoint(((Node2D)trail).ToLocal(val4), -1);
					}
				}
			}, (TransitionType)4, (EaseType)0).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)node))
				{
					((Node)node).QueueFree();
				}
			})));
			if (!FxKit.Alive((GodotObject)(object)trail))
			{
				return;
			}
			Tween val3 = ((Node)trail).CreateTween();
			val3.TweenInterval(0.1);
			val3.TweenProperty((GodotObject)(object)trail, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.18);
			val3.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)trail))
				{
					((Node)trail).QueueFree();
				}
			})));
		}

		public static bool TakeThrowOrigin(CardModel card, out Vector2 from)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			from = default;
			if (card != null)
			{
				return ThrownFrom.Remove(card, out from);
			}
			return false;
		}

		public static void Reset()
		{
			ThrownFrom.Clear();
		}

		private bool ValidRemote(CardModel card)
		{
			if (!RemoteOwner || _dead || _stopping || !Anchor.Valid() || !_owner.IsAlive || ((card != null) ? card.Owner : null) == null || card.Owner.Creature != _owner || _room != GameRefs.CombatRoom || !FxKit.Alive((GodotObject)(object)GameRefs.CombatRoom))
			{
				return false;
			}
			CombatManager instance = CombatManager.Instance;
			CombatState val = ((instance != null) ? instance.DebugOnlyGetState() : null);
			if (val == null || (object)_owner.CombatState != val)
			{
				return false;
			}
			foreach (Player player in val.Players)
			{
				if (player == card.Owner && player.Creature == _owner)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool TryLendRemote(CardModel card, out Node2D blade, out RemoteLoan loan)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Invalid comparison between Unknown and I4
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			blade = null;
			loan = null;
			if ((int)PileOf(card) != 2)
			{
				return false;
			}
			foreach (PhantomBladesFx item in Live)
			{
				if (item.ValidRemote(card))
				{
					item.Sync();
					Slot slot = item.Find(card);
					if (slot != null && FxKit.Alive((GodotObject)(object)slot.Node) && !item._remoteLoans.ContainsKey(card))
					{
						item._slots.Remove(slot);
						item._lent.Add(card);
						blade = slot.Node;
						((CanvasItem)blade).Modulate = Colors.White;
						loan = new RemoteLoan
						{
							Fx = item,
							Card = card,
							Blade = blade,
							Room = item._room
						};
						item._remoteLoans[card] = loan;
						return true;
					}
				}
			}
			return false;
		}

		private static bool CloseRemoteLoan(RemoteLoan loan)
		{
			if (loan == null || loan.Closed)
			{
				return false;
			}
			loan.Closed = true;
			PhantomBladesFx fx = loan.Fx;
			if (!fx._remoteLoans.TryGetValue(loan.Card, out var value) || value != loan)
			{
				return false;
			}
			fx._remoteLoans.Remove(loan.Card);
			fx._lent.Remove(loan.Card);
			return true;
		}

		internal static void GiveBackRemote(RemoteLoan loan)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Invalid comparison between Unknown and I4
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Invalid comparison between Unknown and I4
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			if (!CloseRemoteLoan(loan))
			{
				return;
			}
			PhantomBladesFx fx = loan.Fx;
			Node2D blade = loan.Blade;
			if (FxKit.Alive((GodotObject)(object)blade))
			{
				PileType val = PileOf(loan.Card);
				if (!fx.ValidRemote(loan.Card) || loan.Room != fx._room || ((int)val != 2 && (int)val != 5) || !FxKit.Alive((GodotObject)(object)fx._back) || !((Node)blade).IsInsideTree() || fx.Find(loan.Card) != null)
				{
					((Node)blade).QueueFree();
					return;
				}
				Adopt(blade, (Node)(object)fx._back);
				fx._slots.Add(new Slot
				{
					Card = loan.Card,
					Node = blade,
					Aura = ((Node)blade).GetNodeOrNull<Sprite2D>(NodePath.op_Implicit("Charge")),
					Ink = (loan.Card.Enchantment is Inky),
					Pos = blade.GlobalPosition,
					Rot = blade.GlobalRotation,
					Placed = true,
					Age = 1f
				});
			}
		}

		internal static bool CommitRemoteThrow(RemoteLoan loan)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Invalid comparison between Unknown and I4
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Invalid comparison between Unknown and I4
			if (!CloseRemoteLoan(loan))
			{
				return false;
			}
			PileType val = PileOf(loan.Card);
			if (!loan.Fx.ValidRemote(loan.Card) || !FxKit.Alive((GodotObject)(object)loan.Blade) || ((int)val != 2 && (int)val != 5))
			{
				if (FxKit.Alive((GodotObject)(object)loan.Blade))
				{
					((Node)loan.Blade).QueueFree();
				}
				return false;
			}
			ThrownFrom[loan.Card] = loan.Blade.GlobalPosition;
			return true;
		}

		public static Node2D MakeSpectral(bool ink)
		{
			Sprite2D aura;
			return MakeBlade(ink, out aura);
		}

		public static bool TryTakeForFan(CardModel card, out Node2D blade)
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			blade = null;
			PhantomBladesFx phantomBladesFx = For(card);
			Slot slot = phantomBladesFx?.Find(card);
			if (slot == null)
			{
				return false;
			}
			phantomBladesFx._slots.Remove(slot);
			Node2D node = slot.Node;
			Node frontParent = phantomBladesFx.Anchor.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)node) || !FxKit.Alive((GodotObject)(object)frontParent) || !((Node)node).IsInsideTree())
			{
				if (FxKit.Alive((GodotObject)(object)node))
				{
					((Node)node).QueueFree();
				}
				return false;
			}
			Adopt(node, frontParent);
			((CanvasItem)node).Modulate = Colors.White;
			blade = node;
			return true;
		}

		public static void RememberThrowOrigin(CardModel card, Vector2 from)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (card != null)
			{
				if (ThrownFrom.Count > 32)
				{
					ThrownFrom.Clear();
				}
				ThrownFrom[card] = from;
			}
		}

		public static bool PeekThrowOrigin(CardModel card, out Vector2 from)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			from = default;
			if (card != null)
			{
				return ThrownFrom.TryGetValue(card, out from);
			}
			return false;
		}

		public static void ForgetThrowOrigin(CardModel card)
		{
			if (card != null)
			{
				ThrownFrom.Remove(card);
			}
		}

		public override void Stop(bool animateOut)
		{
			if (!_dead)
			{
				if (animateOut && FxKit.Alive((GodotObject)(object)_back) && ((Node)_back).IsInsideTree())
				{
					_stopping = true;
					_alphaTarget = 0f;
				}
				else
				{
					Dispose();
				}
			}
		}

		private void Dispose()
		{
			if (_dead)
			{
				return;
			}
			_dead = true;
			Live.Remove(this);
			foreach (RemoteLoan value in _remoteLoans.Values)
			{
				value.Closed = true;
				if (FxKit.Alive((GodotObject)(object)value.Blade))
				{
					((Node)value.Blade).QueueFree();
				}
			}
			_remoteLoans.Clear();
			if (FxKit.Alive((GodotObject)(object)_back))
			{
				((Node)_back).QueueFree();
			}
		}

		static PhantomBladesFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		}
	}
