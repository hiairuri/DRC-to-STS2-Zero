// namespace: SilentFX.Dev
// type: DevPreview (class)
// source lines: 23636-24350
	public static class DevPreview
	{
		private sealed class Item
		{
			public string Name;

			public Func<PowerFx> MakePower;

			public bool OnEnemy;

			public CardFx Card;

			public HeldFx Held;
		}

		private static readonly Key[] PreviewKeys;

		private static readonly Dictionary<Key, bool> Held;

		private static readonly List<Item> Entries;

		private static CanvasLayer _layer;

		private static PowerFx _fx;

		private static CardFx _card;

		private static CardFxContext _cardContext;

		private static double _hitTimer;

		private static HeldFx _heldFx;

		private static HeldSession _heldSession;

		private static double _holdTimer;

		private static int _holdStage;

		private static double _finishTimer;

		private static bool _shown;

		private static int _index;

		private static bool _slow;

		private static double _normalTimeScale;

		public static void Init()
		{
			if (File.Exists(Path.Combine(DevLog.ModDir, "dev_preview.txt")))
			{
				Ticker.Add(Update);
				PowerFxManager.LogTriggers = true;
				DevLog.Info("DevPreview enabled: F6 show/hide, F7 trigger, F8 reload, F9 next, F10 previous, F5 slow motion");
			}
		}

		private static void EnsureEntries()
		{
			if (Entries.Count > 0)
			{
				return;
			}
			foreach (var item2 in PowerFxManager.All)
			{
				List<Item> entries = Entries;
				Item item = new Item();
				(item.Name, item.MakePower, item.OnEnemy) = item2;
				entries.Add(item);
			}
			foreach (var item3 in CardFxManager.All)
			{
				Entries.Add(new Item
				{
					Name = "[card] " + item3.Name,
					Card = item3.Fx,
					Held = item3.Held
				});
			}
		}

		private static bool Pressed(Key key)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			bool flag = Input.IsKeyPressed(key);
			Held.TryGetValue(key, out var value);
			Held[key] = flag;
			if (flag)
			{
				return !value;
			}
			return false;
		}

		private static void Update(double dt)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0316: Unknown result type (might be due to invalid IL or missing references)
			if (Showcase.Active || TourController.Exclusive)
			{
				Key[] previewKeys = PreviewKeys;
				foreach (Key val in previewKeys)
				{
					Held[val] = Input.IsKeyPressed(val);
				}
				return;
			}
			if (Pressed((Key)4194337))
			{
				if (!_shown)
				{
					Show(animateIn: true);
				}
				else
				{
					Hide();
				}
			}
			if (Pressed((Key)4194338))
			{
				DevLog.Info("DevPreview: trigger");
				_fx?.Trigger();
				PowerFxManager.TriggerAll();
				if (_shown && _card != null)
				{
					PlayCard();
				}
			}
			if (Pressed((Key)4194339))
			{
				ShaderLib.Reload();
				Tuning.Load();
				PowerFxManager.RestartAll();
				DevLog.Info("DevPreview: shaders and tuning reloaded");
				LogUnderMouse();
				if (_shown)
				{
					Hide();
					Show(animateIn: false);
				}
			}
			if (Pressed((Key)4194336))
			{
				ToggleSlowMotion();
			}
			bool flag = Pressed((Key)4194340);
			bool flag2 = Pressed((Key)4194341);
			if (flag | flag2)
			{
				EnsureEntries();
				if (Entries.Count > 0)
				{
					_index = (_index + (flag ? 1 : (Entries.Count - 1))) % Entries.Count;
					Hide();
					Show(animateIn: true);
				}
			}
			if (_fx != null)
			{
				_fx.Update(dt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
				if (!_fx.IsAlive)
				{
					Hide();
				}
			}
			if (_holdTimer >= 0.0)
			{
				_holdTimer -= dt * Engine.TimeScale;
				if (_holdTimer < 0.0 && _shown && _card != null && _cardContext != null)
				{
					if (_holdStage == -1)
					{
						_holdStage = 0;
						_holdTimer = 1.3;
						if (_heldSession != null && !_heldSession.Finished)
						{
							_heldSession.Hovering = false;
						}
					}
					else if (_holdStage == 0)
					{
						_holdStage = 1;
						_holdTimer = 1.2;
						if (_heldSession != null && !_heldSession.Finished)
						{
							_heldSession.Armed = true;
							_heldSession.SetTarget(_cardContext);
						}
					}
					else
					{
						PlayNow();
					}
				}
			}
			if (_finishTimer >= 0.0)
			{
				_finishTimer -= dt * Engine.TimeScale;
				if (_finishTimer < 0.0 && _heldSession != null && !_heldSession.Finished)
				{
					_heldSession.OnCardFinished();
				}
			}
			if (!(_hitTimer >= 0.0))
			{
				return;
			}
			_hitTimer -= dt * Engine.TimeScale;
			if (!(_hitTimer < 0.0) || !_shown || _card == null || _cardContext == null)
			{
				return;
			}
			try
			{
				_card.OnHit(_cardContext, _cardContext.TargetChest, killed: false);
			}
			catch (Exception ex)
			{
				DevLog.Warn("DevPreview card hit failed: " + ex);
			}
		}

		internal static void SuspendForTour()
		{
			try
			{
				Hide();
				if (_slow)
				{
					Engine.TimeScale = _normalTimeScale;
				}
				_slow = false;
			}
			catch (Exception ex)
			{
				DevLog.Warn("DevPreview: tour handoff failed: " + ex);
			}
		}

		private static void ToggleSlowMotion()
		{
			if (_slow)
			{
				Engine.TimeScale = _normalTimeScale;
				_slow = false;
			}
			else
			{
				_normalTimeScale = Engine.TimeScale;
				Engine.TimeScale = Tuning.F("dev.slow_motion", 0.15f);
				_slow = true;
			}
			DevLog.Info("DevPreview: slow motion " + (_slow ? "on" : "off"));
		}

		private static void PlayCard()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			EndHold();
			if (_heldFx == null)
			{
				PlayNow();
				return;
			}
			try
			{
				CardFxContext cardContext = _cardContext;
				CardFxContext c = new CardFxContext
				{
					BackParent = cardContext.BackParent,
					FrontParent = cardContext.FrontParent,
					Feet = cardContext.Feet,
					Chest = cardContext.Chest,
					Body = cardContext.Body,
					Enemies = cardContext.Enemies,
					View = cardContext.View,
					ExhaustPile = cardContext.ExhaustPile,
					HasTarget = false
				};
				_heldSession = HeldFxManager.StartSession(_heldFx, c, _heldFx.Hover);
				_holdStage = (_heldFx.Hover ? (-1) : 0);
				_holdTimer = 1.3;
			}
			catch (Exception ex)
			{
				DevLog.Warn("DevPreview standby failed: " + ex);
				PlayNow();
			}
		}

		private static void PlayNow()
		{
			try
			{
				_holdTimer = -1.0;
				_cardContext.Held = ((_heldSession != null && !_heldSession.Finished) ? _heldSession : null);
				HeldFxManager.ReleaseManual(_heldSession);
				_card.OnPlay(_cardContext);
				HeldFxManager.PlayManual(_heldSession, _cardContext);
				_hitTimer = 0.45;
				_finishTimer = 1.8;
			}
			catch (Exception ex)
			{
				DevLog.Warn("DevPreview card play failed: " + ex);
			}
		}

		private static void EndHold()
		{
			if (_heldSession != null && !_heldSession.Finished)
			{
				_heldSession.End();
			}
			_heldSession = null;
			_holdTimer = -1.0;
			_finishTimer = -1.0;
		}

		private static void Show(bool animateIn)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected Obj, but got Unknown
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Expected Obj, but got Unknown
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Expected Obj, but got Unknown
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Expected Obj, but got Unknown
			//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Expected Obj, but got Unknown
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0514: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_057e: Unknown result type (might be due to invalid IL or missing references)
			//IL_057f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0585: Unknown result type (might be due to invalid IL or missing references)
			//IL_0591: Unknown result type (might be due to invalid IL or missing references)
			//IL_059d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03de: Unknown result type (might be due to invalid IL or missing references)
			EnsureEntries();
			SceneTree tree = Ticker.Tree;
			if (tree == null || Entries.Count == 0)
			{
				return;
			}
			Rect2 visibleRect = ((Viewport)tree.Root).GetVisibleRect();
			Vector2 size = visibleRect.Size;
			_layer = new CanvasLayer
			{
				Name = StringName.op_Implicit("SilentFX_Preview"),
				Layer = 90
			};
			((Node)tree.Root).AddChild((Node)(object)_layer, false, (InternalMode)0);
			Node2D val = new Node2D
			{
				Name = StringName.op_Implicit("Back")
			};
			((Node)_layer).AddChild((Node)(object)val, false, (InternalMode)0);
			Vector2 val2 = new Vector2(150f, 300f);
			Vector2 val3 = new Vector2(size.X * 0.3f, size.Y * 0.74f);
			Vector2 val4 = val3 - new Vector2(0f, val2.Y * 0.55f);
			ColorRect val5 = new ColorRect
			{
				Color = new Color(0.16f, 0.22f, 0.2f, 1f),
				Size = val2,
				Position = val3 - new Vector2(val2.X * 0.5f, val2.Y),
				MouseFilter = (MouseFilterEnum)2
			};
			((Node)_layer).AddChild((Node)(object)val5, false, (InternalMode)0);
			List<FxTarget> targets = new List<FxTarget>();
			Vector2[] array = new Vector2[2]
			{
				new Vector2(size.X * 0.66f, size.Y * 0.7f),
				new Vector2(size.X * 0.84f, size.Y * 0.76f)
			};
			Vector2[] array2 = new Vector2[2]
			{
				new Vector2(170f, 220f),
				new Vector2(260f, 300f)
			};
			ColorRect[] array3 = new ColorRect[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array3[i] = new ColorRect
				{
					Color = new Color(0.3f, 0.12f, 0.12f, 1f),
					Size = array2[i],
					Position = array[i] - new Vector2(array2[i].X * 0.5f, array2[i].Y),
					MouseFilter = (MouseFilterEnum)2
				};
				((Node)_layer).AddChild((Node)(object)array3[i], false, (InternalMode)0);
				targets.Add(new FxTarget
				{
					Key = "dummy" + i,
					Feet = array[i],
					Chest = array[i] - new Vector2(0f, array2[i].Y * 0.55f),
					Width = array2[i].X,
					Height = array2[i].Y,
					Weak = true,
					Poison = 6
				});
			}
			Node2D val6 = new Node2D
			{
				Name = StringName.op_Implicit("Front")
			};
			((Node)_layer).AddChild((Node)(object)val6, false, (InternalMode)0);
			CanvasLayer layer = _layer;
			Item item = Entries[_index];
			_shown = true;
			if (item.MakePower != null)
			{
				bool onEnemy = item.OnEnemy;
				Vector2 anchorFeet = (onEnemy ? targets[0].Feet : val3);
				Vector2 anchorChest = (onEnemy ? targets[0].Chest : val4);
				Vector2 anchorSize = (onEnemy ? array2[0] : val2);
				CanvasItem anchorBody = (CanvasItem)(object)(onEnemy ? array3[0] : val5);
				_fx = item.MakePower();
				_fx.Amount = (onEnemy ? 6 : 2);
				_fx.Anchor = new FxAnchor
				{
					BackParent = (Node)(object)val,
					FrontParent = (Node)(object)val6,
					Feet = () =>
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						return anchorFeet;
					},
					Chest = () =>
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						return anchorChest;
					},
					BodySize = () =>
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						return anchorSize;
					},
					Body = () => anchorBody,
					Targets = () => targets,
					IsValid = () => FxKit.Alive((GodotObject)(object)layer)
				};
				_fx.Start(animateIn);
			}
			else
			{
				_card = item.Card;
				_heldFx = item.Held;
				_cardContext = new CardFxContext
				{
					BackParent = (Node)(object)val,
					FrontParent = (Node)(object)val6,
					Feet = val3,
					Chest = val4,
					Body = val2,
					HasTarget = true,
					TargetFeet = targets[0].Feet,
					TargetChest = targets[0].Chest,
					TargetWidth = targets[0].Width,
					TargetPoison = targets[0].Poison,
					TargetHeight = array2[0].Y,
					Enemies = targets,
					View = size,
					ExhaustPile = new Vector2(size.X * 0.953f, size.Y * 0.73f)
				};
				PlayCard();
			}
			DevLog.Info($"DevPreview: showing '{item.Name}' (viewport {size.X}x{size.Y})");
		}

		private static void Hide()
		{
			if (_fx != null)
			{
				try
				{
					_fx.Stop(animateOut: false);
				}
				catch
				{
				}
				_fx = null;
			}
			EndHold();
			_heldFx = null;
			_card = null;
			_cardContext = null;
			_hitTimer = -1.0;
			_shown = false;
			if (FxKit.Alive((GodotObject)(object)_layer))
			{
				((Node)_layer).QueueFree();
			}
			_layer = null;
		}

		private static void LogUnderMouse()
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				SceneTree tree = Ticker.Tree;
				Window val = ((tree != null) ? tree.Root : null);
				if (val == null)
				{
					return;
				}
				Control val2 = ((Viewport)val).GuiGetHoveredControl();
				DevLog.Info($"Under the mouse at {((Viewport)val).GetMousePosition()}: " + ((val2 == null) ? "nothing" : $"{((Node)val2).GetPath()} ({((object)val2).GetType().Name}, filter {val2.MouseFilter}, visible {((CanvasItem)val2).IsVisibleInTree()})"));
				Control val3 = ((Viewport)val).GuiGetFocusOwner();
				DevLog.Info("Keyboard focus: " + ((val3 == null) ? "nothing" : ((object)((Node)val3).GetPath()).ToString()));
				DevLog.Info($"Mouse buttons held: {Input.GetMouseButtonMask()}, time scale {Engine.TimeScale}, tree paused {Ticker.Tree.Paused}");
				try
				{
					NControllerManager instance = NControllerManager.Instance;
					if (instance != null)
					{
						DevLog.Info(RuntimeApiCompatibility.DescribeInput(instance));
					}
				}
				catch (Exception ex)
				{
					DevLog.Info("Input type: not readable (" + ex.Message + ")");
				}
				int listed = 0;
				WalkUnderMouse((Node)(object)val, ref listed);
				DevLog.Info($"Controls under the mouse that take it: {listed}");
				NRun instance2 = NRun.Instance;
				object obj;
				if (instance2 == null)
				{
					obj = null;
				}
				else
				{
					NGlobalUi globalUi = instance2.GlobalUi;
					obj = ((globalUi != null) ? globalUi.TopBar : null);
				}
				NTopBar val4 = (NTopBar)obj;
				if (FxKit.Alive((GodotObject)(object)val4))
				{
					DescribeButton("Map", (NClickableControl)(object)val4.Map);
					DescribeButton("Deck", (NClickableControl)(object)val4.Deck);
					DescribeButton("Pause", (NClickableControl)(object)val4.Pause);
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("LogUnderMouse: " + ex2.Message);
			}
		}

		private static void WalkUnderMouse(Node node, ref int listed)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Invalid comparison between Unknown and I8
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			Control val = (Control)(object)((node is Control) ? node : null);
			if (val != null && ((CanvasItem)val).IsVisibleInTree() && (long)val.MouseFilter != 2)
			{
				bool flag;
				try
				{
					Rect2 globalRect = val.GetGlobalRect();
					flag = globalRect.HasPoint(((CanvasItem)val).GetGlobalMousePosition());
				}
				catch
				{
					flag = false;
				}
				if (flag && listed < 40)
				{
					listed++;
					DevLog.Info($"  takes the mouse: {((Node)val).GetPath()} ({((object)val).GetType().Name}, {val.MouseFilter}, at {val.GetGlobalRect()})");
				}
			}
			foreach (Node child in node.GetChildren(false))
			{
				WalkUnderMouse(child, ref listed);
			}
		}

		private static void DescribeButton(string name, NClickableControl button)
		{
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)button))
			{
				DevLog.Info("  " + name + " button: gone");
				return;
			}
			string value = "?";
			try
			{
				FieldInfo field = typeof(NClickableControl).GetField("_isHovered", BindingFlags.Instance | BindingFlags.NonPublic);
				if (field != null)
				{
					value = field.GetValue(button)?.ToString();
				}
			}
			catch
			{
			}
			DevLog.Info($"  {name} button: enabled {button.IsEnabled}, hovered {value}, visible {((CanvasItem)button).IsVisibleInTree()}, filter {((Control)button).MouseFilter}, tint {((CanvasItem)button).Modulate}, at {((Control)button).GetGlobalRect()}");
		}

		static DevPreview()
		{
			Key[] array = new Key[6];
			RuntimeHelpers.InitializeArray(array, __ldtoken(<PrivateImplementationDetails>.E0493D0AE3F0B27EAB6B5D8C8E2CC5C3218CDCC12603CD0AE36A9F5F6D83CDF7));
			PreviewKeys = array;
			Held = new Dictionary<Key, bool>();
			Entries = new List<Item>();
			_hitTimer = -1.0;
			_holdTimer = -1.0;
			_finishTimer = -1.0;
			_normalTimeScale = 1.0;
		}
	}
