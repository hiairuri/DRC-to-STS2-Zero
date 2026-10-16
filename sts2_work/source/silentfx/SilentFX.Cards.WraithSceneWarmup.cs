// namespace: SilentFX.Cards
// type: WraithSceneWarmup (class)
// source lines: 120557-121151
	internal static class WraithSceneWarmup
	{
		private enum Stage
		{
			Form,
			FormDraw,
			Play,
			PlayDraw,
			Done
		}

		private const int MaxWidth = 960;

		private const int MaxHeight = 540;

		private const int DrawsPerStage = 2;

		private const int MaxWaitFrames = 180;

		private static GDScript _formScript;

		private static GDScript _playScript;

		private static Dictionary _cache;

		private static bool _registered;

		private static bool _failed;

		private static bool _drawHooked;

		private static bool _treeHooked;

		private static bool _settingsHooked;

		private static int _draws;

		private static int _waitFrames;

		private static double _cpuMs;

		private static long _phaseBegan;

		private static int _renderedFrames;

		private static Stage _stage;

		private static SubViewport _view;

		private static World2D _world;

		private static Node2D _canvas;

		private static Node2D _back;

		private static Node2D _front;

		private static Node2D _top;

		internal static bool CanRun
		{
			get
			{
				if (PlayerSettings.Enabled && PlayerSettings.PrewarmEnabled && PlayerSettings.ShaderWarmupEnabled && GameRefs.CombatRoom == null && Ticker.Tree != null && !Ticker.Tree.Paused)
				{
					return FxKit.Alive((GodotObject)(object)Ticker.Tree.Root);
				}
				return false;
			}
		}

		internal static void Queue(GDScript form, GDScript play, Dictionary cache)
		{
			if (!_registered && FxKit.Alive((GodotObject)(object)form) && FxKit.Alive((GodotObject)(object)play) && cache != null)
			{
				_registered = true;
				_formScript = form;
				_playScript = play;
				_cache = cache;
				_stage = Stage.Form;
				AssetWarmup.Add("Wraith scene pipelines", Step, Cancel, () => CanRun);
			}
		}

		private static bool Step()
		{
			if (_failed)
			{
				return true;
			}
			if (!CanRun)
			{
				Suspend();
				return false;
			}
			try
			{
				if (_stage == Stage.FormDraw || _stage == Stage.PlayDraw)
				{
					if (_draws < 2)
					{
						return false;
					}
					FinishViewport();
					_stage = ((_stage == Stage.FormDraw) ? Stage.Play : Stage.Done);
					return false;
				}
				if (_stage == Stage.Form)
				{
					BuildForm();
					_stage = Stage.FormDraw;
					return false;
				}
				if (_stage == Stage.Play)
				{
					BuildPlay();
					_stage = Stage.PlayDraw;
					return false;
				}
				if (_stage == Stage.Done)
				{
					DevLog.Info($"Wraith Form: actual scene prewarm complete; CPU setup {_cpuMs:F1} ms, rendered frames {_renderedFrames} (CPU only; excludes GPU timing)");
					return true;
				}
			}
			catch (Exception error)
			{
				Fail(error);
				return true;
			}
			return false;
		}

		private static void BuildForm()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected Obj, but got Unknown
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Invalid comparison between Unknown and I8
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			_phaseBegan = Stopwatch.GetTimestamp();
			CreateViewport();
			Node val = new Node
			{
				Name = StringName.op_Implicit("SilentFX_WraithWarmForm")
			};
			((GodotObject)val).SetScript(Variant.op_Implicit((GodotObject)(object)_formScript));
			((Node)_back).AddChild(val, false, (InternalMode)0);
			Vector2 zero = Vector2.Zero;
			Vector2 val2 = LogicalSize();
			float num = 0.62f * val2.Y / 1080f;
			Vector2 val3 = new Vector2(val2.X * 0.5f - 606f * num, 12f * num / 0.62f);
			Variant val4 = ((GodotObject)val).Call(StringName.op_Implicit("build"), new Variant[8]
			{
				Variant.op_Implicit(WraithStage.Dir),
				Variant.op_Implicit(_cache),
				Variant.op_Implicit((GodotObject)(object)_back),
				Variant.op_Implicit((GodotObject)(object)_front),
				Variant.op_Implicit(zero),
				Variant.op_Implicit(val2),
				Variant.op_Implicit(val3),
				Variant.op_Implicit(num)
			});
			if ((long)val4.VariantType != 1 || !val4.AsBool())
			{
				throw new InvalidOperationException("form stage build returned false");
			}
			((GodotObject)val).Call(StringName.op_Implicit("set_param"), new Variant[2]
			{
				Variant.op_Implicit("blur"),
				Variant.op_Implicit(Math.Max(0.1f, Tuning.F("wraith.blur", 1f)))
			});
			((GodotObject)val).Call(StringName.op_Implicit("set_state"), new Variant[4]
			{
				Variant.op_Implicit(0.96f),
				Variant.op_Implicit(1f),
				Variant.op_Implicit(0f),
				Variant.op_Implicit(1f)
			});
			BeginDrawWindow();
			FinishCpuPhase();
		}

		private static void BuildPlay()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected Obj, but got Unknown
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Invalid comparison between Unknown and I8
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			_phaseBegan = Stopwatch.GetTimestamp();
			CreateViewport();
			Node val = new Node
			{
				Name = StringName.op_Implicit("SilentFX_WraithWarmPlay")
			};
			((GodotObject)val).SetScript(Variant.op_Implicit((GodotObject)(object)_playScript));
			((Node)_back).AddChild(val, false, (InternalMode)0);
			Vector2 zero = Vector2.Zero;
			Vector2 val2 = LogicalSize();
			float num = 0.62f * val2.Y / 1080f;
			Vector2 val3 = new Vector2(val2.X * 0.5f - 606f * num, 12f * num / 0.62f);
			Vector2 val4 = new Vector2(val2.X * 0.5f, val2.Y * 0.8f);
			Variant val5 = ((GodotObject)val).Call(StringName.op_Implicit("build"), new Variant[9]
			{
				Variant.op_Implicit(WraithStage.Dir),
				Variant.op_Implicit(_cache),
				Variant.op_Implicit((GodotObject)(object)_back),
				Variant.op_Implicit((GodotObject)(object)_front),
				Variant.op_Implicit(zero),
				Variant.op_Implicit(val2),
				Variant.op_Implicit(val3),
				Variant.op_Implicit(num),
				Variant.op_Implicit(val4)
			});
			if ((long)val5.VariantType != 1 || !val5.AsBool())
			{
				throw new InvalidOperationException("play stage build returned false");
			}
			((GodotObject)val).Call(StringName.op_Implicit("set_param"), new Variant[2]
			{
				Variant.op_Implicit("blur"),
				Variant.op_Implicit(Math.Max(0.1f, Tuning.F("wraith.blur", 1f)))
			});
			((GodotObject)val).Call(StringName.op_Implicit("set_param"), new Variant[2]
			{
				Variant.op_Implicit("wave"),
				Variant.op_Implicit(Math.Max(0.1f, Tuning.F("wraith.play.wave", 1f)))
			});
			((GodotObject)val).Call(StringName.op_Implicit("set_top"), new Variant[1] { Variant.op_Implicit((GodotObject)(object)_top) });
			((GodotObject)val).Call(StringName.op_Implicit("set_time"), new Variant[2]
			{
				Variant.op_Implicit(1.2f),
				Variant.op_Implicit(0f)
			});
			BeginDrawWindow();
			FinishCpuPhase();
		}

		private static void CreateViewport()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Expected Obj, but got Unknown
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Expected Obj, but got Unknown
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Expected Obj, but got Unknown
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Expected Obj, but got Unknown
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Expected Obj, but got Unknown
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Expected Obj, but got Unknown
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Expected Obj, but got Unknown
			if (FxKit.Alive((GodotObject)(object)_view))
			{
				throw new InvalidOperationException("a previous Wraith warm-up viewport is still alive");
			}
			SceneTree tree = Ticker.Tree;
			if (tree == null || !FxKit.Alive((GodotObject)(object)tree.Root))
			{
				throw new InvalidOperationException("main viewport is unavailable");
			}
			Vector2 val = LogicalSize();
			float num = Math.Min(1f, Math.Min(960f / val.X, 540f / val.Y));
			num = Mathf.Clamp(num, 0.1f, 1f);
			Vector2I val2 = new Vector2I(Math.Max(1, Mathf.RoundToInt(val.X * num)), Math.Max(1, Mathf.RoundToInt(val.Y * num)));
			_world = new World2D();
			_view = new SubViewport
			{
				Name = StringName.op_Implicit("SilentFX_WraithSceneWarmup"),
				Size = val2,
				TransparentBg = true,
				Disable3D = true,
				GuiDisableInput = true,
				World2D = _world,
				UseHdr2D = ((Viewport)tree.Root).UseHdr2D,
				Msaa2D = ((Viewport)tree.Root).Msaa2D,
				RenderTargetUpdateMode = (UpdateMode)0
			};
			((Node)tree.Root).AddChild((Node)(object)_view, false, (InternalMode)0);
			((Node)_view).AddChild((Node)new ColorRect
			{
				Name = StringName.op_Implicit("SilentFX_WraithWarmBackdrop"),
				Size = new Vector2((float)val2.X, (float)val2.Y),
				Color = new Color(0.06f, 0.08f, 0.09f, 0.12f),
				MouseFilter = (MouseFilterEnum)2
			}, false, (InternalMode)0);
			_canvas = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_WraithWarmCanvas"),
				Scale = new Vector2(num, num)
			};
			((Node)_view).AddChild((Node)(object)_canvas, false, (InternalMode)0);
			_back = new Node2D
			{
				Name = StringName.op_Implicit("Back")
			};
			_front = new Node2D
			{
				Name = StringName.op_Implicit("Front")
			};
			_top = new Node2D
			{
				Name = StringName.op_Implicit("Top")
			};
			((Node)_canvas).AddChild((Node)(object)_back, false, (InternalMode)0);
			((Node)_canvas).AddChild((Node)(object)_front, false, (InternalMode)0);
			((Node)_canvas).AddChild((Node)(object)_top, false, (InternalMode)0);
			HookLifetime(tree);
		}

		private static Vector2 LogicalSize()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = FxKit.ViewSize();
			if (!float.IsFinite(val.X) || !float.IsFinite(val.Y) || val.X < 64f || val.Y < 64f)
			{
				return new Vector2(1920f, 1080f);
			}
			return val;
		}

		private static void HookLifetime(SceneTree tree)
		{
			if (!_drawHooked)
			{
				RenderingServer.FramePostDraw += OnFramePostDraw;
				_drawHooked = true;
			}
			if (!_treeHooked)
			{
				((Node)tree.Root).TreeExiting += OnTreeExiting;
				_treeHooked = true;
			}
			if (!_settingsHooked)
			{
				PlayerSettings.Changed += OnSettingsChanged;
				_settingsHooked = true;
			}
			Ticker.Add(Guard);
		}

		private static void BeginDrawWindow()
		{
			_draws = 0;
			_waitFrames = 0;
			_view.RenderTargetUpdateMode = (UpdateMode)4;
		}

		private static void OnFramePostDraw()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_view) && (int)_view.RenderTargetUpdateMode != 0 && ++_draws >= 2)
			{
				_renderedFrames += _draws;
				_view.RenderTargetUpdateMode = (UpdateMode)0;
			}
		}

		private static void Guard(double dt)
		{
			if (!CanRun)
			{
				Suspend();
			}
			else if ((_stage == Stage.FormDraw || _stage == Stage.PlayDraw) && ++_waitFrames > 180)
			{
				Fail(new TimeoutException("the isolated Wraith viewport did not produce two FramePostDraw callbacks within 180 process frames"));
			}
		}

		private static void OnSettingsChanged()
		{
			if (!CanRun)
			{
				Suspend();
			}
		}

		private static void OnTreeExiting()
		{
			Suspend();
		}

		private static void FinishViewport()
		{
			ReleaseViewport();
		}

		private static void Suspend()
		{
			ReleaseViewport();
			_stage = Stage.Form;
		}

		private static void Fail(Exception error)
		{
			FinishCpuPhase();
			ReleaseViewport();
			_failed = true;
			DevLog.Warn($"Wraith Form: actual scene prewarm failed after CPU setup {_cpuMs:F1} ms (CPU only): {error.Message}");
		}

		private static void FinishCpuPhase()
		{
			if (_phaseBegan != 0L)
			{
				_cpuMs += Stopwatch.GetElapsedTime(_phaseBegan).TotalMilliseconds;
				_phaseBegan = 0L;
			}
		}

		private static void Cancel()
		{
			ReleaseViewport();
			_cache = null;
			_formScript = null;
			_playScript = null;
		}

		private static void ReleaseViewport()
		{
			if (FxKit.Alive((GodotObject)(object)_view))
			{
				_view.RenderTargetUpdateMode = (UpdateMode)0;
				((GodotObject)_view).Free();
			}
			_view = null;
			_back = (_front = (_top = (_canvas = null)));
			World2D world = _world;
			if (world != null)
			{
				((GodotObject)world).Dispose();
			}
			_world = null;
			if (_drawHooked)
			{
				RenderingServer.FramePostDraw -= OnFramePostDraw;
				_drawHooked = false;
			}
			if (_treeHooked)
			{
				SceneTree tree = Ticker.Tree;
				if (((tree != null) ? tree.Root : null) != null)
				{
					((Node)Ticker.Tree.Root).TreeExiting -= OnTreeExiting;
				}
				_treeHooked = false;
			}
			if (_settingsHooked)
			{
				PlayerSettings.Changed -= OnSettingsChanged;
				_settingsHooked = false;
			}
			Ticker.Remove(Guard);
			_draws = (_waitFrames = 0);
		}
	}
