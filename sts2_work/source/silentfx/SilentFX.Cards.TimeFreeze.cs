// namespace: SilentFX.Cards
// type: TimeFreeze (class)
// source lines: 113384-115451
	public static class TimeFreeze
	{
		private sealed class Stopped
		{
			public Node Node;

			public CanvasItem Item;

			public bool Ours;

			public float At;

			public Node Parent;

			public Vector2 Place;

			public float Turn;

			public bool Placed;
		}

		private sealed class FrozenCard
		{
			public Control Frame;

			public Node Card;

			public CardModel Model;
		}

		private sealed class Floater
		{
			public Node2D Node;

			public Vector2 From;

			public Vector2 To;

			public Vector2 Aim;

			public float Halt;

			public float At;

			public float Age;

			public float Phase;
		}

		private sealed class Ripple
		{
			public float Reach;

			public float Speed;
		}

		[CompilerGenerated]
		private static class <>O
		{
			public static ChildEnteredTreeEventHandler <0>__OnChild;

			public static Action<double> <1>__Update;
		}

		private static readonly string[] HeldMotions;

		private static readonly StringName LiveMark;

		private static int _heldSounds;

		private static int _heldScenes;

		private static bool _active;

		private static bool _thawing;

		private static bool _replaying;

		private static bool _ticking;

		private static bool _warnedSlow;

		private static bool _otherChecked;

		private static bool _otherFound;

		private static float _clock;

		private static float _thawClock;

		private static Creature _owner;

		private static Node _front;

		private static Node _back;

		private static ColorRect _veil;

		private static CanvasItem _playArea;

		private static Color _playAreaTint;

		private static ShaderMaterial _veilMat;

		private static readonly List<Ripple> Ripples;

		private static readonly float[] RippleUniform;

		private static readonly List<Floater> Floaters;

		private static readonly List<CanvasItem> Burns;

		private static readonly List<Vector2> Claimed;

		private static readonly List<NCreature> Slowed;

		private static readonly List<(NCreature Node, bool Remove)> Deaths;

		private static readonly List<Stopped> Things;

		private static readonly Dictionary<Node, Stopped> Known;

		private static readonly Dictionary<Node, Func<Node>> Recipes;

		private static readonly List<(float At, Action Go)> Events;

		private static readonly List<(float Due, Action Go)> Due;

		private static readonly HashSet<NHealthBar> Bars;

		private static readonly Dictionary<NCreature, HashSet<string>> Motions;

		private static readonly List<FrozenCard> Cards;

		public static bool Active => _active;

		static TimeFreeze()
		{
			HeldMotions = new string[5] { "Attack", "Cast", "PowerUp", "Shiv", "Hit" };
			LiveMark = StringName.op_Implicit("silentfx_live");
			Ripples = new List<Ripple>();
			RippleUniform = new float[8];
			Floaters = new List<Floater>();
			Burns = new List<CanvasItem>();
			Claimed = new List<Vector2>();
			Slowed = new List<NCreature>();
			Deaths = new List<(NCreature, bool)>();
			Things = new List<Stopped>();
			Known = new Dictionary<Node, Stopped>(ReferenceEqualityComparer.Instance);
			Recipes = new Dictionary<Node, Func<Node>>(ReferenceEqualityComparer.Instance);
			Events = new List<(float, Action)>();
			Due = new List<(float, Action)>();
			Bars = new HashSet<NHealthBar>();
			Motions = new Dictionary<NCreature, HashSet<string>>();
			Cards = new List<FrozenCard>();
			PlayerSettings.Changed += OnSettingsChanged;
		}

		private static void OnSettingsChanged()
		{
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			if (!PlayerSettings.Enabled)
			{
				if (_active)
				{
					Thaw();
				}
			}
			else
			{
				if (!_active)
				{
					return;
				}
				if (PlayerSettings.ReduceScreenEffects)
				{
					Ripples.Clear();
					if (FxKit.Alive((GodotObject)(object)_veilMat))
					{
						_veilMat.SetShaderParameter(StringName.op_Implicit("amount"), Variant.op_Implicit(Math.Min(Tuning.F("bullet.filter_amount", 0.85f), 0.36f)));
					}
					else if (FxKit.Alive((GodotObject)(object)_veil))
					{
						_veil.Color = new Color(0.3f, 0.52f, 0.95f, Math.Min(Tuning.F("bullet.tint", 0.18f), 0.08f));
					}
				}
				else if (FxKit.Alive((GodotObject)(object)_veilMat))
				{
					_veilMat.SetShaderParameter(StringName.op_Implicit("amount"), Variant.op_Implicit(Tuning.F("bullet.filter_amount", 0.85f)));
				}
				else if (FxKit.Alive((GodotObject)(object)_veil))
				{
					_veil.Color = new Color(0.3f, 0.52f, 0.95f, Tuning.F("bullet.tint", 0.18f));
				}
			}
		}

		public static void KeepLive(Node node)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			if (node != null)
			{
				((GodotObject)node).SetMeta(LiveMark, Variant.op_Implicit(true));
			}
		}

		internal static bool HoldsVisual(Node node)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Invalid comparison between Unknown and I8
			try
			{
				if (!_active && !_thawing)
				{
					return false;
				}
				Node val = node;
				while (FxKit.Alive((GodotObject)(object)val))
				{
					if (Known.TryGetValue(val, out var value) && value.Ours && (long)val.ProcessMode == 4)
					{
						return true;
					}
					val = val.GetParent();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Bullet Time visual ownership: " + ex);
			}
			return false;
		}

		public static bool Begin(CardFxContext c)
		{
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_034b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Expected Obj, but got Unknown
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0370: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Expected Obj, but got Unknown
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Expected Obj, but got Unknown
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			if (!PlayerSettings.Enabled)
			{
				return false;
			}
			if (_active)
			{
				return true;
			}
			try
			{
				if (Tuning.F("bullet.freeze", 1f) < 0.5f)
				{
					return false;
				}
				if (Showcase.Active)
				{
					return false;
				}
				if (AnotherModStopsTime())
				{
					return false;
				}
				CardModel val = c?.Card;
				if (val == null)
				{
					return false;
				}
				Player owner = val.Owner;
				Creature val2 = ((owner != null) ? owner.Creature : null);
				ICombatState val3 = ((val2 != null) ? val2.CombatState : null);
				if (val3 == null || val3.Players.Count > 1)
				{
					return false;
				}
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				if (combatRoom == null)
				{
					return false;
				}
				if (_thawing)
				{
					Finish();
				}
				_front = (Node)(object)combatRoom.CombatVfxContainer;
				_back = (Node)(object)combatRoom.BackCombatVfxContainer;
				if (!FxKit.Alive((GodotObject)(object)_front) || !FxKit.Alive((GodotObject)(object)_back))
				{
					return false;
				}
				_owner = val2;
				_clock = 0f;
				_heldScenes = 0;
				_heldSounds = 0;
				Things.Clear();
				Known.Clear();
				Recipes.Clear();
				Events.Clear();
				TimeFreezeHover.Clear();
				Bars.Clear();
				Motions.Clear();
				Ripples.Clear();
				Floaters.Clear();
				Claimed.Clear();
				Slowed.Clear();
				Deaths.Clear();
				_veil = FxKit.Veil(new Color(0.3f, 0.52f, 0.95f, 0f));
				((Node)_veil).Name = StringName.op_Implicit("SilentFX_FreezeVeil");
				KeepLive((Node)(object)_veil);
				_veilMat = MakeFilter(val3, c);
				if (_veilMat != null)
				{
					_veil.Color = Colors.White;
					((CanvasItem)_veil).Material = (Material)(object)_veilMat;
					((Node)_veil).AddChild((Node)new BackBufferCopy
					{
						CopyMode = (CopyModeEnum)2,
						ShowBehindParent = true
					}, false, (InternalMode)0);
				}
				_front.AddChild((Node)(object)_veil, false, (InternalMode)0);
				if (_veilMat != null)
				{
					float num = Tuning.F("bullet.filter_amount", 0.85f);
					if (PlayerSettings.ReduceScreenEffects)
					{
						num = Math.Min(num, 0.36f);
					}
					((Node)_veil).CreateTween().TweenProperty((GodotObject)(object)_veilMat, NodePath.op_Implicit("shader_parameter/amount"), Variant.op_Implicit(num), 0.25);
					if (!PlayerSettings.ReduceScreenEffects)
					{
						AddRipple(Tuning.F("bullet.ring_first", 1.3f));
					}
				}
				else
				{
					float num2 = Tuning.F("bullet.tint", 0.18f);
					if (PlayerSettings.ReduceScreenEffects)
					{
						num2 = Math.Min(num2, 0.08f);
					}
					((Node)_veil).CreateTween().TweenProperty((GodotObject)(object)_veil, NodePath.op_Implicit("color:a"), Variant.op_Implicit(num2), 0.12);
				}
				try
				{
					NCombatUi ui = combatRoom.Ui;
					Control val4 = ((ui != null) ? ui.PlayContainer : null);
					if (FxKit.Alive((GodotObject)(object)val4))
					{
						_playArea = (CanvasItem)(object)val4;
						_playAreaTint = ((CanvasItem)val4).Modulate;
					}
				}
				catch
				{
					_playArea = null;
				}
				Node front = _front;
				ChildEnteredTreeEventHandler val5 = <>O.<0>__OnChild;
				if (val5 == null)
				{
					ChildEnteredTreeEventHandler val6 = OnChild;
					<>O.<0>__OnChild = val6;
					val5 = val6;
				}
				front.ChildEnteredTree += val5;
				Node back = _back;
				ChildEnteredTreeEventHandler val7 = <>O.<0>__OnChild;
				if (val7 == null)
				{
					ChildEnteredTreeEventHandler val8 = OnChild;
					<>O.<0>__OnChild = val8;
					val7 = val8;
				}
				back.ChildEnteredTree += val7;
				_active = true;
				if (!_ticking)
				{
					_ticking = true;
					Ticker.Add(Update);
				}
				DevLog.Info("Bullet Time: time stopped until the end of the turn");
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Error("Bullet Time freeze could not start: " + ex);
				Reset();
				return false;
			}
		}

		private static bool AnotherModStopsTime()
		{
			if (_otherChecked)
			{
				return _otherFound;
			}
			_otherChecked = true;
			try
			{
				if (Tuning.F("bullet.yield_to_other_mods", 1f) < 0.5f)
				{
					return false;
				}
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				for (int i = 0; i < assemblies.Length; i++)
				{
					if (!(assemblies[i].GetName().Name != "BulletTimeAnime"))
					{
						_otherFound = true;
						DevLog.Warn("Bullet Time: the mod \"Bullet Time: As Depicted\" is also installed and stops time itself. SilentFX leaves the freeze to it (only the clock and the flash are shown). Turn that mod off to see SilentFX's own freeze.");
						break;
					}
				}
			}
			catch
			{
				_otherFound = false;
			}
			return _otherFound;
		}

		private static ShaderMaterial MakeFilter(ICombatState state, CardFxContext c)
		{
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (Tuning.F("bullet.filter", 1f) < 0.5f)
				{
					return null;
				}
				EncounterModel val = ((state != null) ? state.Encounter : null);
				PropertyInfo property = typeof(EncounterModel).GetProperty("HasCustomBackground", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				bool flag = default;
				int num;
				if (val != null && property != null)
				{
					object value = property.GetValue(val);
					if (value is bool)
					{
						flag = (bool)value;
						num = 1;
					}
					else
					{
						num = 0;
					}
				}
				else
				{
					num = 0;
				}
				if (((uint)num & (flag ? 1u : 0u)) != 0)
				{
					return null;
				}
				ShaderMaterial val2 = ShaderLib.NewMaterial("freeze");
				if (val2 == null)
				{
					return null;
				}
				val2.SetShaderParameter(StringName.op_Implicit("amount"), Variant.op_Implicit(0f));
				for (int i = 0; i < RippleUniform.Length; i++)
				{
					RippleUniform[i] = -1f;
				}
				val2.SetShaderParameter(StringName.op_Implicit("rings"), Variant.op_Implicit(RippleUniform));
				val2.SetShaderParameter(StringName.op_Implicit("centre"), Variant.op_Implicit(ScreenPoint(c.Chest)));
				return val2;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Bullet Time: the screen filter could not be made, using the flat tint: " + ex.Message);
				return null;
			}
		}

		private static Vector2 ScreenPoint(Vector2 global)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = FxKit.ViewSize();
			Vector2 val2 = global;
			Node front = _front;
			CanvasItem val3 = (CanvasItem)(object)((front is CanvasItem) ? front : null);
			if (val3 != null && FxKit.Alive((GodotObject)(object)val3))
			{
				val2 = val3.GetCanvasTransform() * global;
			}
			if (val.X < 1f || val.Y < 1f)
			{
				return new Vector2(0.28f, 0.58f);
			}
			return new Vector2(Mathf.Clamp(val2.X / val.X, 0f, 1f), Mathf.Clamp(val2.Y / val.Y, 0f, 1f));
		}

		private static void AddRipple(float speed)
		{
			if (_veilMat != null)
			{
				if (Ripples.Count >= RippleUniform.Length)
				{
					Ripples.RemoveAt(0);
				}
				Ripples.Add(new Ripple
				{
					Reach = Tuning.F("bullet.ring_start", 0.1f),
					Speed = speed
				});
			}
		}

		private static bool FromThisMod()
		{
			try
			{
				StackTrace stackTrace = new StackTrace(2, fNeedFileInfo: false);
				int num = Math.Min(stackTrace.FrameCount, 48);
				for (int i = 0; i < num; i++)
				{
					Type type = stackTrace.GetFrame(i)?.GetMethod()?.DeclaringType;
					if (!(type == null) && !(type == typeof(TimeFreeze)))
					{
						string text = type.Namespace;
						if (text != null && text.StartsWith("SilentFX", StringComparison.Ordinal))
						{
							return true;
						}
					}
				}
			}
			catch
			{
			}
			return false;
		}

		private static void OnChild(Node node)
		{
			if (!_active || !FxKit.Alive((GodotObject)(object)node))
			{
				return;
			}
			try
			{
				if (!Known.ContainsKey(node) && !((GodotObject)node).HasMeta(LiveMark))
				{
					Hold(node, FromThisMod());
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Bullet Time: could not hold an effect back: " + ex.Message);
			}
		}

		private static void Hold(Node node, bool ours)
		{
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			if (Known.ContainsKey(node))
			{
				return;
			}
			Stopped stopped = new Stopped
			{
				Node = node
			};
			Node val = node;
			stopped.Item = (CanvasItem)(object)((val is CanvasItem) ? val : null);
			stopped.Ours = ours;
			stopped.At = _clock;
			stopped.Parent = node.GetParent();
			Stopped thing = stopped;
			if (thing.Item != null)
			{
				thing.Item.Visible = false;
			}
			Things.Add(thing);
			Known[node] = thing;
			if (ours)
			{
				node.ProcessMode = (ProcessModeEnum)4;
				return;
			}
			Callable val2 = Callable.From((Action)(() =>
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				try
				{
					if (FxKit.Alive((GodotObject)(object)node))
					{
						Node val3 = node;
						Node2D val4 = (Node2D)(object)((val3 is Node2D) ? val3 : null);
						if (val4 != null)
						{
							thing.Place = val4.Position;
							thing.Turn = val4.Rotation;
							thing.Placed = true;
						}
						else
						{
							Node val5 = node;
							Control val6 = (Control)(object)((val5 is Control) ? val5 : null);
							if (val6 != null)
							{
								thing.Place = val6.Position;
								thing.Turn = val6.Rotation;
								thing.Placed = true;
							}
						}
					}
				}
				catch
				{
				}
			}));
			val2.CallDeferred(Array.Empty<Variant>());
		}

		public static void OnVfxCreated(MethodBase maker, object[] args, object made)
		{
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			if (!_active || _replaying || maker == null)
			{
				return;
			}
			if (PowerFxManager.LogTriggers)
			{
				DevLog.Info($"Bullet Time: the game made {made?.GetType().Name ?? "nothing"} ({maker.DeclaringType?.Name}.Create, {args?.Length ?? 0} arguments)");
			}
			Node node = (Node)((made is Node) ? made : null);
			if (node == null || !FxKit.Alive((GodotObject)(object)node) || Recipes.ContainsKey(node))
			{
				return;
			}
			object[] with = args;
			Recipes[node] = () =>
			{
				object? obj = maker.Invoke(null, with);
				return (Node)((obj is Node) ? obj : null);
			};
			if (node is NShivThrowVfx)
			{
				TryFloat(with);
			}
			if (node.IsInsideTree())
			{
				Hold(node, ours: false);
				return;
			}
			((GodotObject)node).Connect(SignalName.TreeEntered, Callable.From((Action)(() =>
			{
				try
				{
					if (_active && FxKit.Alive((GodotObject)(object)node) && !((GodotObject)node).HasMeta(LiveMark))
					{
						Hold(node, ours: false);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Bullet Time: could not hold one of the game's effects back: " + ex.Message);
				}
			})), 4u);
		}

		private static void TryFloat(object[] args)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (args == null || args.Length < 2)
				{
					return;
				}
				if (args[0] is Vector2 val && args[1] is Vector2 to)
				{
					AddFloater(val, to, fromCard: false);
					return;
				}
				object obj = args[0];
				Creature val2 = (Creature)((obj is Creature) ? obj : null);
				if (val2 != null)
				{
					object obj2 = args[1];
					Creature val3 = (Creature)((obj2 is Creature) ? obj2 : null);
					if (val3 != null)
					{
						FloatBetween(val2, val3, fromCard: false);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Bullet Time: a thrown blade could not be hung in the air: " + ex.Message);
			}
		}

		private static void FloatBetween(Creature thrower, Creature target, bool fromCard)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				if (combatRoom != null && thrower != null && target != null)
				{
					NCreature creatureNode = combatRoom.GetCreatureNode(thrower);
					NCreature creatureNode2 = combatRoom.GetCreatureNode(target);
					if (FxKit.Alive((GodotObject)(object)creatureNode) && FxKit.Alive((GodotObject)(object)creatureNode2))
					{
						AddFloater(creatureNode.VfxSpawnPosition, creatureNode2.VfxSpawnPosition, fromCard);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Bullet Time: a thrown blade could not be hung in the air: " + ex.Message);
			}
		}

		public static void FloatKnife(Vector2 from, Vector2 to, bool ink)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			if (_active)
			{
				AddFloater(from, to, fromCard: true, claim: false, ink);
			}
		}

		private static void AddFloater(Vector2 from, Vector2 to, bool fromCard, bool claim = true, bool ink = false)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			if (Tuning.F("bullet.floaters", 1f) < 0.5f)
			{
				return;
			}
			Vector2 val;
			if (fromCard)
			{
				if (claim)
				{
					Claimed.Add(to);
				}
			}
			else
			{
				for (int i = 0; i < Claimed.Count; i++)
				{
					val = Claimed[i];
					if (!(val.DistanceTo(to) > 4f))
					{
						Claimed.RemoveAt(i);
						return;
					}
				}
			}
			if (Floaters.Count >= 40 || !FxKit.Alive((GodotObject)(object)_front))
			{
				return;
			}
			float num = from.DistanceTo(to);
			if (!(num < 60f))
			{
				int count = Floaters.Count;
				Vector2 val2 = (to - from) / num;
				Vector2 val3 = new Vector2(0f - val2.Y, val2.X);
				Vector2 val4 = from + val3 * ((FxKit.Hash(count * 5 + 1) - 0.5f) * 40f);
				Vector2 val5 = to + val3 * ((FxKit.Hash(count * 5 + 3) - 0.5f) * 44f);
				float num2 = Tuning.F("bullet.float_gap", 150f) + 30f * (float)(count % 4);
				float halt = Mathf.Clamp((num - num2) / num, 0.25f, 0.95f);
				Node2D val6 = ShivFx.MakeShiv(ink);
				((Node)val6).Name = StringName.op_Implicit("SilentFX_FloatingShiv");
				KeepLive((Node)(object)val6);
				((CanvasItem)val6).Modulate = new Color(0.84f, 0.94f, 1f, 0f);
				_front.AddChild((Node)(object)val6, false, (InternalMode)0);
				val6.GlobalPosition = val4;
				val = val5 - val4;
				val6.Rotation = val.Angle() + (float)Math.PI / 2f;
				Floaters.Add(new Floater
				{
					Node = val6,
					From = val4,
					To = val5,
					Aim = to,
					Halt = halt,
					At = _clock,
					Phase = FxKit.Hash(count * 7 + 2) * ((float)Math.PI * 2f)
				});
				if (PowerFxManager.LogTriggers)
				{
					DevLog.Info($"Bullet Time: a blade hangs in the air (#{count}, {num:0} px to go)");
				}
			}
		}

		public static void OnCardBurn(object made)
		{
			if (_active)
			{
				CanvasItem val = (CanvasItem)((made is CanvasItem) ? made : null);
				if (val != null && FxKit.Alive((GodotObject)(object)val))
				{
					val.Visible = false;
					Burns.Add(val);
				}
			}
		}

		private static void Zip(Floater floater)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			Node2D node = floater.Node;
			if (!FxKit.Alive((GodotObject)(object)node))
			{
				return;
			}
			if (!((Node)node).IsInsideTree())
			{
				((Node)node).QueueFree();
				return;
			}
			Tween val = ((Node)node).CreateTween();
			val.TweenProperty((GodotObject)(object)node, NodePath.op_Implicit("global_position"), Variant.op_Implicit(floater.To), 0.1).SetTrans((TransitionType)4).SetEase((EaseType)0);
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)node))
				{
					((Node)node).QueueFree();
				}
			})));
		}

		public static bool SkipSceneVfx(Vector2 position, string path, Control container)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			if (!_active || _replaying || string.IsNullOrEmpty(path))
			{
				return false;
			}
			Events.Add((_clock, () =>
			{
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)container) && ((Node)container).IsInsideTree())
				{
					VfxCmd.PlayVfx(position, path, container);
				}
			}));
			_heldScenes++;
			return true;
		}

		public static bool SkipFullScreenVfx(string path, Creature spawner)
		{
			if (!_active || _replaying || string.IsNullOrEmpty(path))
			{
				return false;
			}
			Events.Add((_clock, () =>
			{
				VfxCmd.PlayFullScreenInCombat(path, spawner);
			}));
			_heldScenes++;
			return true;
		}

		public static bool HoldSound(string path, Dictionary<string, float> parameters, float volume)
		{
			if (!_active || _replaying || string.IsNullOrEmpty(path))
			{
				return false;
			}
			if (Tuning.F("bullet.hold_sound", 1f) < 0.5f)
			{
				return false;
			}
			if (_clock < 0.35f)
			{
				return false;
			}
			if (path.Contains("/ui/", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			Dictionary<string, float> copy = ((parameters == null) ? new Dictionary<string, float>() : new Dictionary<string, float>(parameters));
			Events.Add((_clock, () =>
			{
				NAudioManager instance = NAudioManager.Instance;
				if (instance != null)
				{
					instance.PlayOneShot(path, copy, volume);
				}
			}));
			_heldSounds++;
			return true;
		}

		public static void OnCardPlayed(CardPlay play)
		{
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Expected Obj, but got Unknown
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			if (!_active)
			{
				return;
			}
			CardModel val = ((play != null) ? play.Card : null);
			if (val == null || val is BulletTime)
			{
				return;
			}
			try
			{
				if (!LocalContext.IsMine(val))
				{
					return;
				}
			}
			catch
			{
			}
			if (!PlayerSettings.ReduceScreenEffects)
			{
				AddRipple(Tuning.F("bullet.ring_speed", 0.2f));
			}
			if (val is Shiv && play.Target != null && !ShivFan.Active(val))
			{
				Player owner = val.Owner;
				FloatBetween((owner != null) ? owner.Creature : null, play.Target, fromCard: true);
			}
			try
			{
				Node uiLayer = GameRefs.UiLayer;
				if (!FxKit.Alive((GodotObject)(object)uiLayer))
				{
					return;
				}
				Vector2 val2 = FxKit.ViewSize();
				Vector2 globalPosition = new Vector2(val2.X * 0.5f, val2.Y * 0.8f);
				float num = 0.8f;
				NCard val3 = NCard.FindOnTable(val, (PileType?)null);
				if (FxKit.Alive((GodotObject)(object)val3) && ((Node)val3).IsInsideTree())
				{
					Transform2D globalTransform = ((CanvasItem)val3).GetGlobalTransform();
					globalPosition = globalTransform.Origin;
					num = globalTransform.Scale.X;
				}
				Control val4 = new Control
				{
					Name = StringName.op_Implicit("SilentFX_FrozenCard"),
					MouseFilter = (MouseFilterEnum)2,
					MouseBehaviorRecursive = (MouseBehaviorRecursiveEnum)1
				};
				uiLayer.AddChild((Node)(object)val4, false, (InternalMode)0);
				NCard val5 = NCard.Create(val, (ModelVisibility)1);
				if (!FxKit.Alive((GodotObject)(object)val5))
				{
					((Node)val4).QueueFree();
					return;
				}
				((Node)val4).AddChild((Node)(object)val5, false, (InternalMode)0);
				try
				{
					val5.UpdateVisuals((PileType)2, (CardPreviewMode)1);
				}
				catch
				{
				}
				int count = Cards.Count;
				Cards.Add(new FrozenCard
				{
					Frame = val4,
					Card = (Node)(object)val5,
					Model = val
				});
				int num2 = Mathf.Max(3, Tuning.I("bullet.cards_per_row", 9));
				float num3 = val2.X * 0.5f / (float)(num2 - 1);
				Vector2 val6 = new Vector2(val2.X * 0.25f + (float)(count % num2) * num3, val2.Y * (0.19f + 0.1f * (float)(count / num2)) + 16f * FxKit.Hash(count * 3 + 1));
				float num4 = (FxKit.Hash(count * 7 + 2) - 0.5f) * 0.26f;
				float num5 = Tuning.F("bullet.card_scale", 0.42f);
				val4.GlobalPosition = globalPosition;
				val4.Scale = new Vector2(num, num);
				((CanvasItem)val4).Modulate = new Color(1f, 1f, 1f, 0.95f);
				Tween val7 = ((Node)val4).CreateTween();
				val7.SetParallel(true);
				val7.TweenProperty((GodotObject)(object)val4, NodePath.op_Implicit("global_position"), Variant.op_Implicit(val6), 0.3).SetTrans((TransitionType)4).SetEase((EaseType)1);
				val7.TweenProperty((GodotObject)(object)val4, NodePath.op_Implicit("scale"), Variant.op_Implicit(new Vector2(num5, num5)), 0.3).SetTrans((TransitionType)4).SetEase((EaseType)1);
				val7.TweenProperty((GodotObject)(object)val4, NodePath.op_Implicit("rotation"), Variant.op_Implicit(num4), 0.3);
				val7.TweenProperty((GodotObject)(object)val4, NodePath.op_Implicit("modulate"), Variant.op_Implicit(new Color(0.72f, 0.86f, 1f, 0.92f)), 0.3);
				if (FxKit.Alive((GodotObject)(object)_playArea))
				{
					_playArea.Modulate = new Color(_playAreaTint.R, _playAreaTint.G, _playAreaTint.B, 0f);
				}
				if (PowerFxManager.LogTriggers)
				{
					DevLog.Info($"Bullet Time: froze {((object)val).GetType().Name} (slot {count})");
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Bullet Time: could not freeze the played card: " + ex.Message);
			}
		}

		public static bool Defer(Action later)
		{
			if (!_active || later == null)
			{
				return false;
			}
			Events.Add((_clock, later));
			return true;
		}

		public static bool HoldMotion(NCreature node, string trigger)
		{
			if (!_active || !FxKit.Alive((GodotObject)(object)node) || trigger == null)
			{
				return false;
			}
			if (Array.IndexOf(HeldMotions, trigger) < 0)
			{
				return false;
			}
			if (!Motions.TryGetValue(node, out var value))
			{
				value = (Motions[node] = new HashSet<string>());
			}
			value.Add(trigger);
			return true;
		}

		public static bool HoldDeath(NCreature node, bool shouldRemove)
		{
			if (!_active || !FxKit.Alive((GodotObject)(object)node))
			{
				return false;
			}
			if (Tuning.F("bullet.hold_death", 1f) < 0.5f)
			{
				return false;
			}
			Creature entity = node.Entity;
			if (entity == null || !entity.IsEnemy)
			{
				return false;
			}
			foreach (var death in Deaths)
			{
				if (death.Node == node)
				{
					return true;
				}
			}
			Deaths.Add((node, shouldRemove));
			return true;
		}

		public static bool HoldBar(NHealthBar bar)
		{
			if (!_active || !FxKit.Alive((GodotObject)(object)bar))
			{
				return false;
			}
			Bars.Add(bar);
			TimeFreezeHover.Track(bar);
			return true;
		}

		public static void OnDamageNumber(NDamageNumVfx node, Vector2 at, int damage)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (_active && !_replaying)
			{
				if (FxKit.Alive((GodotObject)(object)node))
				{
					((CanvasItem)node).Visible = false;
				}
				Events.Add((_clock, () =>
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					SpawnNumber(at, damage);
				}));
			}
		}

		private static void SpawnNumber(Vector2 at, int damage)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			_replaying = true;
			try
			{
				NRun instance = NRun.Instance;
				NGlobalUi val = ((instance != null) ? instance.GlobalUi : null);
				if (FxKit.Alive((GodotObject)(object)val))
				{
					NDamageNumVfx val2 = NDamageNumVfx.Create(at, damage);
					if (FxKit.Alive((GodotObject)(object)val2))
					{
						((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Bullet Time: a damage number could not be shown: " + ex.Message);
			}
			finally
			{
				_replaying = false;
			}
		}

		public static void OnHitSpark(NHitSparkVfx node, Creature target)
		{
			if (!_active || _replaying)
			{
				return;
			}
			if (FxKit.Alive((GodotObject)(object)node))
			{
				((CanvasItem)node).Visible = false;
			}
			if (target == null)
			{
				return;
			}
			Events.Add((_clock, () =>
			{
				_replaying = true;
				try
				{
					if (target.IsAlive)
					{
						Control vfxContainer = target.GetVfxContainer();
						NHitSparkVfx val = NHitSparkVfx.Create(target, false);
						if (FxKit.Alive((GodotObject)(object)vfxContainer) && FxKit.Alive((GodotObject)(object)val))
						{
							((Node)vfxContainer).AddChild((Node)(object)val, false, (InternalMode)0);
						}
					}
				}
				catch
				{
				}
				finally
				{
					_replaying = false;
				}
			}));
		}

		public static float Thaw(bool fightOver = false)
		{
			//IL_0672: Unknown result type (might be due to invalid IL or missing references)
			//IL_0702: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0725: Unknown result type (might be due to invalid IL or missing references)
			if (!_active)
			{
				return 0f;
			}
			TimeFreezeHover.Clear();
			_active = false;
			Unhook();
			RestoreSpeed();
			float num = Mathf.Max(0.02f, fightOver ? Tuning.F("bullet.replay_won", 0.1f) : Tuning.F("bullet.replay", 0.1f));
			Due.Clear();
			_thawClock = 0f;
			float num2 = float.MaxValue;
			float num3 = 0f;
			foreach (Stopped thing in Things)
			{
				if (thing.Ours || Recipes.ContainsKey(thing.Node))
				{
					num2 = Mathf.Min(num2, thing.At);
					num3 = Mathf.Max(num3, thing.At);
				}
			}
			foreach (var @event in Events)
			{
				num2 = Mathf.Min(num2, @event.At);
				num3 = Mathf.Max(num3, @event.At);
			}
			foreach (Floater floater in Floaters)
			{
				num2 = Mathf.Min(num2, floater.At);
				num3 = Mathf.Max(num3, floater.At);
			}
			if (num2 > num3)
			{
				num2 = num3;
			}
			float num4 = num3 - num2;
			float num5 = ((num4 > num) ? (num / num4) : 1f);
			float num6 = 0.18f + num4 * num5;
			int num7 = 0;
			foreach (Stopped thing2 in Things)
			{
				Stopped held = thing2;
				if (!held.Ours)
				{
					if (Recipes.TryGetValue(held.Node, out var recipe))
					{
						Due.Add((0.18f + (held.At - num2) * num5, () =>
						{
							Remake(held, recipe);
						}));
						num7++;
						continue;
					}
					Due.Add((0f, () =>
					{
						if (FxKit.Alive((GodotObject)(object)held.Item))
						{
							held.Item.Visible = true;
						}
					}));
				}
				else
				{
					Due.Add((0.18f + (held.At - num2) * num5, () =>
					{
						Release(held);
					}));
				}
			}
			Known.Clear();
			Recipes.Clear();
			foreach (var event2 in Events)
			{
				Action item = event2.Go;
				Due.Add((0.18f + (event2.At - num2) * num5, item));
			}
			Things.Clear();
			Events.Clear();
			foreach (Floater floater2 in Floaters)
			{
				Floater blade = floater2;
				Due.Add((Mathf.Max(0.04f, 0.18f + (blade.At - num2) * num5 - 0.06f), () =>
				{
					Zip(blade);
				}));
			}
			Floaters.Clear();
			foreach (KeyValuePair<NCreature, HashSet<string>> motion in Motions)
			{
				NCreature node = motion.Key;
				HashSet<string> value = motion.Value;
				bool flag = FxKit.Alive((GodotObject)(object)node) && node.Entity == _owner;
				if (flag && (value.Contains("Attack") || value.Contains("Shiv")))
				{
					Due.Add((0.09f, () =>
					{
						if (FxKit.Alive((GodotObject)(object)node))
						{
							node.SetAnimationTrigger("Attack");
						}
					}));
				}
				else if (flag && (value.Contains("Cast") || value.Contains("PowerUp")))
				{
					Due.Add((0.09f, () =>
					{
						if (FxKit.Alive((GodotObject)(object)node))
						{
							node.SetAnimationTrigger("Cast");
						}
					}));
				}
				if (!value.Contains("Hit"))
				{
					continue;
				}
				Due.Add((0.18f + num4 * num5 * 0.35f, () =>
				{
					if (FxKit.Alive((GodotObject)(object)node))
					{
						node.SetAnimationTrigger("Hit");
					}
				}));
			}
			Motions.Clear();
			List<NHealthBar> bars = new List<NHealthBar>(Bars);
			Bars.Clear();
			Due.Add((0.18f + num4 * num5 * 0.5f, () =>
			{
				foreach (NHealthBar item2 in bars)
				{
					try
					{
						if (FxKit.Alive((GodotObject)(object)item2) && ((Node)item2).IsInsideTree())
						{
							item2.RefreshValues();
						}
					}
					catch
					{
					}
				}
			}));
			float num8 = Mathf.Min(0.05f, num / (float)Mathf.Max(1, Cards.Count));
			for (int num9 = 0; num9 < Cards.Count; num9++)
			{
				FrozenCard card = Cards[num9];
				Due.Add((0.072000004f + (float)num9 * num8, () =>
				{
					SendOff(card);
				}));
			}
			num6 = Mathf.Max(num6, 0.072000004f + (float)Cards.Count * num8 + 0.35f);
			Cards.Clear();
			if (Deaths.Count > 0)
			{
				List<(NCreature Node, bool Remove)> deaths = new List<(NCreature, bool)>(Deaths);
				Deaths.Clear();
				Due.Add((num6, () =>
				{
					foreach (var item3 in deaths)
					{
						Die(item3.Node, item3.Remove);
					}
				}));
				num6 += 0.15f;
			}
			if (FxKit.Alive((GodotObject)(object)_playArea))
			{
				_playArea.Modulate = _playAreaTint;
			}
			_playArea = null;
			if (FxKit.Alive((GodotObject)(object)_veil))
			{
				ColorRect veil = _veil;
				Tween val = ((Node)veil).CreateTween();
				if (FxKit.Alive((GodotObject)(object)_veilMat))
				{
					val.TweenProperty((GodotObject)(object)_veilMat, NodePath.op_Implicit("shader_parameter/amount"), Variant.op_Implicit(0f), 0.45);
				}
				else
				{
					val.TweenProperty((GodotObject)(object)veil, NodePath.op_Implicit("color:a"), Variant.op_Implicit(0f), 0.55);
				}
				val.TweenCallback(Callable.From((Action)(() =>
				{
					if (FxKit.Alive((GodotObject)(object)veil))
					{
						((Node)veil).QueueFree();
					}
				})));
			}
			_veil = null;
			_veilMat = null;
			Ripples.Clear();
			Burns.Clear();
			Due.Sort(((float Due, Action Go) a, (float Due, Action Go) b) => a.Due.CompareTo(b.Due));
			_thawing = true;
			DevLog.Info($"Bullet Time: time starts again ({Due.Count} held back - of the game's: {num7} effects to make again, {_heldScenes} scene effects, {_heldSounds} sounds; {num6 + 0.4f:0.0}s)");
			_heldScenes = 0;
			_heldSounds = 0;
			return num6 + 0.4f;
		}

		private static void Die(NCreature node, bool remove)
		{
			try
			{
				if (FxKit.Alive((GodotObject)(object)node) && ((Node)node).IsInsideTree())
				{
					node.StartDeathAnim(remove);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Bullet Time: a held-back death could not be played: " + ex.Message);
			}
		}

		private static void SlowEnemies()
		{
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				float num = Mathf.Clamp(Tuning.F("bullet.enemy_speed", 0.04f), 0f, 1f);
				if (num > 0.99f)
				{
					return;
				}
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				Creature owner = _owner;
				object obj;
				if (owner == null)
				{
					obj = null;
				}
				else
				{
					ICombatState combatState = owner.CombatState;
					obj = ((combatState != null) ? combatState.Enemies : null);
				}
				IReadOnlyList<Creature> readOnlyList = (IReadOnlyList<Creature>)obj;
				if (combatRoom == null || readOnlyList == null)
				{
					return;
				}
				for (int i = 0; i < readOnlyList.Count; i++)
				{
					NCreature creatureNode = combatRoom.GetCreatureNode(readOnlyList[i]);
					if (FxKit.Alive((GodotObject)(object)creatureNode) && !Slowed.Contains(creatureNode) && creatureNode.HasSpineAnimation)
					{
						SpineAnimationAccess spineAnimation = creatureNode.SpineAnimation;
						spineAnimation.SetTimeScale(num);
						Slowed.Add(creatureNode);
					}
				}
			}
			catch (Exception ex)
			{
				if (!_warnedSlow)
				{
					DevLog.Warn("Bullet Time: the enemies could not be slowed: " + ex.Message);
				}
				_warnedSlow = true;
			}
		}

		private static void RestoreSpeed()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			foreach (NCreature item in Slowed)
			{
				try
				{
					if (FxKit.Alive((GodotObject)(object)item) && item.HasSpineAnimation)
					{
						SpineAnimationAccess spineAnimation = item.SpineAnimation;
						spineAnimation.SetTimeScale(1f);
					}
				}
				catch
				{
				}
			}
			Slowed.Clear();
		}

		private static bool EnemyLeft()
		{
			try
			{
				Creature owner = _owner;
				object obj;
				if (owner == null)
				{
					obj = null;
				}
				else
				{
					ICombatState combatState = owner.CombatState;
					obj = ((combatState != null) ? combatState.Enemies : null);
				}
				IReadOnlyList<Creature> readOnlyList = (IReadOnlyList<Creature>)obj;
				if (readOnlyList == null)
				{
					return true;
				}
				for (int i = 0; i < readOnlyList.Count; i++)
				{
					if (readOnlyList[i] != null && readOnlyList[i].IsAlive && readOnlyList[i].IsPrimaryEnemy)
					{
						return true;
					}
				}
				return false;
			}
			catch
			{
				return true;
			}
		}

		private static void Release(Stopped thing)
		{
			if (FxKit.Alive((GodotObject)(object)thing.Node))
			{
				thing.Node.ProcessMode = (ProcessModeEnum)0;
				if (thing.Item != null)
				{
					thing.Item.Visible = true;
				}
				Restart(thing.Node, 0);
			}
		}

		private static void Remake(Stopped thing, Func<Node> recipe)
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)thing.Node) && thing.Node.IsInsideTree())
			{
				if (thing.Item != null)
				{
					thing.Item.Visible = true;
				}
				return;
			}
			Node parent = thing.Parent;
			if (!FxKit.Alive((GodotObject)(object)parent) || !parent.IsInsideTree())
			{
				return;
			}
			Node val = recipe();
			if (!FxKit.Alive((GodotObject)(object)val))
			{
				return;
			}
			if (val.GetParent() == null)
			{
				parent.AddChild(val, false, (InternalMode)0);
			}
			if (!thing.Placed)
			{
				return;
			}
			Node2D val2 = (Node2D)(object)((val is Node2D) ? val : null);
			if (val2 != null && val2.Position == Vector2.Zero && thing.Place != Vector2.Zero)
			{
				val2.Position = thing.Place;
				val2.Rotation = thing.Turn;
				return;
			}
			Control val3 = (Control)(object)((val is Control) ? val : null);
			if (val3 != null && val3.Position == Vector2.Zero && thing.Place != Vector2.Zero)
			{
				val3.Position = thing.Place;
			}
		}

		private static void Restart(Node node, int depth)
		{
			GpuParticles2D val = (GpuParticles2D)(object)((node is GpuParticles2D) ? node : null);
			if (val != null && (val.OneShot || val.Emitting))
			{
				val.Restart();
			}
			if (depth > 6)
			{
				return;
			}
			foreach (Node child in node.GetChildren(false))
			{
				Restart(child, depth + 1);
			}
		}

		private static void SendOff(FrozenCard card)
		{
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Invalid comparison between Unknown and I4
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Invalid comparison between Unknown and I4
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			Control frame = card.Frame;
			if (!FxKit.Alive((GodotObject)(object)frame))
			{
				Free(card);
				return;
			}
			Node card2 = card.Card;
			CanvasItem val = (CanvasItem)(object)((card2 is CanvasItem) ? card2 : null);
			if (val != null && FxKit.Alive((GodotObject)(object)val))
			{
				val.Visible = true;
			}
			PileType val2 = (PileType)3;
			try
			{
				CardModel model = card.Model;
				PileType? val3;
				if (model == null)
				{
					val3 = null;
				}
				else
				{
					CardPile pile = model.Pile;
					val3 = ((pile != null) ? new PileType?(pile.Type) : ((PileType?)null));
				}
				val2 = (PileType)(((??)val3) ?? 3);
			}
			catch
			{
				val2 = (PileType)3;
			}
			Vector2 val4 = GameRefs.PilePoint((PileType)(((int)val2 != 4 && (int)val2 != 1) ? 3 : ((int)val2)));
			Tween val5 = ((Node)frame).CreateTween();
			val5.SetParallel(true);
			val5.TweenProperty((GodotObject)(object)frame, NodePath.op_Implicit("global_position"), Variant.op_Implicit(val4), 0.3).SetTrans((TransitionType)4).SetEase((EaseType)0);
			val5.TweenProperty((GodotObject)(object)frame, NodePath.op_Implicit("scale"), Variant.op_Implicit(new Vector2(0.08f, 0.08f)), 0.3).SetTrans((TransitionType)4).SetEase((EaseType)0);
			val5.TweenProperty((GodotObject)(object)frame, NodePath.op_Implicit("modulate:a"), Variant.op_Implicit(0f), 0.3).SetTrans((TransitionType)4).SetEase((EaseType)0);
			val5.SetParallel(false);
			val5.TweenCallback(Callable.From((Action)(() =>
			{
				Free(card);
			})));
		}

		private static void Free(FrozenCard card)
		{
			try
			{
				if (FxKit.Alive((GodotObject)(object)card.Card))
				{
					GodotTreeExtensions.QueueFreeSafely(card.Card);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Bullet Time: a frozen card could not be handed back: " + ex.Message);
			}
			card.Card = null;
			Control frame = card.Frame;
			card.Frame = null;
			if (FxKit.Alive((GodotObject)(object)frame))
			{
				((CanvasItem)frame).Visible = false;
				if (((Node)frame).IsInsideTree())
				{
					FxKit.FreeAfter((Node)(object)frame, 0.25);
				}
				else
				{
					((Node)frame).QueueFree();
				}
			}
		}

		public static async Task Linger(Task wait, float seconds)
		{
			if (wait != null)
			{
				await wait;
			}
			try
			{
				await Cmd.Wait(seconds, false);
			}
			catch (Exception ex)
			{
				DevLog.Warn("Bullet Time: could not hold the turn: " + ex.Message);
			}
		}

		private static void Unhook()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected Obj, but got Unknown
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected Obj, but got Unknown
			try
			{
				if (FxKit.Alive((GodotObject)(object)_front))
				{
					Node front = _front;
					ChildEnteredTreeEventHandler val = <>O.<0>__OnChild;
					if (val == null)
					{
						ChildEnteredTreeEventHandler val2 = OnChild;
						<>O.<0>__OnChild = val2;
						val = val2;
					}
					front.ChildEnteredTree -= val;
				}
				if (FxKit.Alive((GodotObject)(object)_back))
				{
					Node back = _back;
					ChildEnteredTreeEventHandler val3 = <>O.<0>__OnChild;
					if (val3 == null)
					{
						ChildEnteredTreeEventHandler val4 = OnChild;
						<>O.<0>__OnChild = val4;
						val3 = val4;
					}
					back.ChildEnteredTree -= val3;
				}
			}
			catch
			{
			}
			_front = null;
			_back = null;
		}

		private static void Finish()
		{
			foreach (var item in Due)
			{
				try
				{
					item.Go();
				}
				catch (Exception ex)
				{
					DevLog.Warn("Bullet Time: a held-back effect failed: " + ex.Message);
				}
			}
			Due.Clear();
			_thawing = false;
		}

		public static void Reset()
		{
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			bool flag = _active || _thawing;
			TimeFreezeHover.Clear();
			if (_thawing)
			{
				Finish();
			}
			_active = false;
			_thawing = false;
			Unhook();
			foreach (Stopped thing in Things)
			{
				try
				{
					if (thing.Ours && FxKit.Alive((GodotObject)(object)thing.Node))
					{
						thing.Node.QueueFree();
					}
				}
				catch
				{
				}
			}
			Things.Clear();
			Known.Clear();
			Recipes.Clear();
			Events.Clear();
			Due.Clear();
			Bars.Clear();
			Motions.Clear();
			foreach (FrozenCard card in Cards)
			{
				Free(card);
			}
			Cards.Clear();
			RestoreSpeed();
			Burns.Clear();
			foreach (Floater floater in Floaters)
			{
				try
				{
					if (FxKit.Alive((GodotObject)(object)floater.Node))
					{
						((Node)floater.Node).QueueFree();
					}
				}
				catch
				{
				}
			}
			Floaters.Clear();
			foreach (var death in Deaths)
			{
				Die(death.Node, death.Remove);
			}
			Deaths.Clear();
			Ripples.Clear();
			try
			{
				if (FxKit.Alive((GodotObject)(object)_playArea))
				{
					_playArea.Modulate = _playAreaTint;
				}
				if (FxKit.Alive((GodotObject)(object)_veil))
				{
					((Node)_veil).QueueFree();
				}
			}
			catch
			{
			}
			_playArea = null;
			_veil = null;
			_veilMat = null;
			_owner = null;
			if (flag)
			{
				DevLog.Info("Bullet Time: freeze dropped");
			}
		}

		private static void Update(double realDt)
		{
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0282: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			if (!_active && !_thawing)
			{
				return;
			}
			float num = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			if (_active)
			{
				if (GameRefs.CombatRoom == null)
				{
					Reset();
					return;
				}
				Mood.Hold("bullet", Sfx.Knob("mood.bullet.volume", 0.6f), Sfx.Knob("mood.bullet.pitch", 0.85f), Sfx.Knob("mood.bullet.far", 0.5f), Sfx.Knob("mood.bullet.hollow", 0.2f), Mathf.Max(0.05f, Sfx.Knob("mood.bullet.in", 0.55f)), Mathf.Max(0.05f, Sfx.Knob("mood.bullet.back", 0.8f)), 0.3f);
				bool flag = false;
				try
				{
					CombatManager instance = CombatManager.Instance;
					flag = instance == null || !instance.IsInProgress || instance.IsEnding;
				}
				catch
				{
					flag = false;
				}
				if (flag || !EnemyLeft())
				{
					Thaw(fightOver: true);
					return;
				}
				_clock += num;
				SlowEnemies();
				TimeFreezeHover.Update();
				if (FxKit.Alive((GodotObject)(object)_veilMat))
				{
					for (int num2 = Ripples.Count - 1; num2 >= 0; num2--)
					{
						Ripples[num2].Reach += Ripples[num2].Speed * num;
						if (Ripples[num2].Reach > 2.4f)
						{
							Ripples.RemoveAt(num2);
						}
					}
					for (int i = 0; i < RippleUniform.Length; i++)
					{
						RippleUniform[i] = ((i < Ripples.Count) ? Ripples[i].Reach : (-1f));
					}
					_veilMat.SetShaderParameter(StringName.op_Implicit("rings"), Variant.op_Implicit(RippleUniform));
				}
				float num3 = Tuning.F("bullet.float_brake", 3f);
				foreach (Floater floater in Floaters)
				{
					if (FxKit.Alive((GodotObject)(object)floater.Node))
					{
						floater.Age += num;
						float num4 = 1f - Mathf.Exp((0f - floater.Age) * num3);
						float num5 = Mathf.Min(floater.Age * 0.006f, 0.05f);
						Vector2 val = floater.To - floater.From;
						Vector2 val2 = new Vector2(0f - val.Y, val.X);
						Vector2 val3 = val2.Normalized();
						floater.Node.GlobalPosition = floater.From + val * (floater.Halt * num4 + num5) + val3 * (Mathf.Sin(floater.Age * 1.4f + floater.Phase) * 3f);
						((CanvasItem)floater.Node).Modulate = new Color(0.84f, 0.94f, 1f, Mathf.Clamp(floater.Age / 0.12f, 0f, 1f));
					}
				}
				for (int num6 = Burns.Count - 1; num6 >= 0; num6--)
				{
					if (!FxKit.Alive((GodotObject)(object)Burns[num6]))
					{
						Burns.RemoveAt(num6);
					}
					else if (Burns[num6].Visible)
					{
						Burns[num6].Visible = false;
					}
				}
				foreach (Stopped thing in Things)
				{
					if (thing.Item != null && FxKit.Alive((GodotObject)(object)thing.Item) && thing.Item.Visible)
					{
						thing.Item.Visible = false;
					}
				}
				{
					foreach (FrozenCard card2 in Cards)
					{
						Node card = card2.Card;
						CanvasItem val4 = (CanvasItem)(object)((card is CanvasItem) ? card : null);
						if (val4 != null && FxKit.Alive((GodotObject)(object)val4) && !val4.Visible)
						{
							val4.Visible = true;
						}
					}
					return;
				}
			}
			_thawClock += num;
			while (Due.Count > 0 && Due[0].Due <= _thawClock)
			{
				Action item = Due[0].Go;
				Due.RemoveAt(0);
				try
				{
					item();
				}
				catch (Exception ex)
				{
					DevLog.Warn("Bullet Time: a held-back effect failed: " + ex.Message);
				}
			}
			if (Due.Count == 0)
			{
				_thawing = false;
			}
		}
	}
