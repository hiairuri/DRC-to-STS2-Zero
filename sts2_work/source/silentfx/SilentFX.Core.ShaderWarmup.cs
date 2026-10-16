// namespace: SilentFX.Core
// type: ShaderWarmup (class)
// source lines: 44937-45213
	internal static class ShaderWarmup
	{
		private static readonly Queue<Shader> Pending = new Queue<Shader>();

		private static readonly HashSet<ulong> Seen = new HashSet<ulong>();

		private static SubViewport _view;

		private static ShaderMaterial _material;

		private static World2D _world;

		private static int _draws;

		private static int _waits;

		private static bool _hooked;

		private static bool _failed;

		private static Shader _plain;

		private static readonly List<Material> ParticleMaterials = new List<Material>();

		internal static void Primitives()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected Obj, but got Unknown
			if (_plain == null)
			{
				_plain = new Shader
				{
					Code = "shader_type canvas_item;\nvoid fragment() {}"
				};
			}
			Add(_plain);
		}

		internal static void Add(Shader shader)
		{
			if (!_failed && FxKit.Alive((GodotObject)(object)shader) && Seen.Add(((GodotObject)shader).GetInstanceId()))
			{
				Pending.Enqueue(shader);
				AssetWarmup.Add("canvas pipelines", Step, null, () => PlayerSettings.ShaderWarmupEnabled);
			}
		}

		private static bool Step()
		{
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Expected Obj, but got Unknown
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Expected Obj, but got Unknown
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Expected Obj, but got Unknown
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Expected Obj, but got Unknown
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Expected Obj, but got Unknown
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Expected Obj, but got Unknown
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Expected Obj, but got Unknown
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_038f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0450: Unknown result type (might be due to invalid IL or missing references)
			if (_failed)
			{
				Clear();
				return true;
			}
			if (!PlayerSettings.ShaderWarmupEnabled)
			{
				Pause();
				return false;
			}
			try
			{
				if (FxKit.Alive((GodotObject)(object)_view))
				{
					if (_draws < 2)
					{
						if (++_waits > 600)
						{
							throw new TimeoutException("no rendered frames for the warm-up viewport");
						}
						_view.RenderTargetUpdateMode = (UpdateMode)4;
						return false;
					}
					ReleaseView();
				}
				if (Pending.Count == 0)
				{
					Clear();
					return true;
				}
				Shader val = Pending.Dequeue();
				if (!FxKit.Alive((GodotObject)(object)val))
				{
					return Pending.Count == 0;
				}
				_draws = (_waits = 0);
				_world = new World2D();
				_view = new SubViewport
				{
					Name = StringName.op_Implicit("SilentFX_ShaderWarmup"),
					Size = new Vector2I(32, 32),
					TransparentBg = true,
					Disable3D = true,
					GuiDisableInput = true,
					World2D = _world,
					UseHdr2D = ((Viewport)Ticker.Tree.Root).UseHdr2D,
					Msaa2D = ((Viewport)Ticker.Tree.Root).Msaa2D,
					RenderTargetUpdateMode = (UpdateMode)4
				};
				_material = new ShaderMaterial
				{
					Shader = val
				};
				((Node)_view).AddChild((Node)new ColorRect
				{
					Size = new Vector2(16f, 16f),
					Color = Colors.White,
					Material = (Material)(object)_material,
					MouseFilter = (MouseFilterEnum)2
				}, false, (InternalMode)0);
				((Node)_view).AddChild((Node)new Sprite2D
				{
					Texture = FxKit.SoftDot,
					Position = new Vector2(24f, 8f),
					Scale = new Vector2(0.25f, 0.25f),
					Material = (Material)(object)_material
				}, false, (InternalMode)0);
				SubViewport view = _view;
				Polygon2D val2 = new Polygon2D();
				val2.Polygon = new Vector2[3]
				{
					new Vector2(1f, 17f),
					new Vector2(15f, 17f),
					new Vector2(8f, 31f)
				};
				val2.Color = Colors.White;
				((CanvasItem)val2).Material = (Material)(object)_material;
				((Node)view).AddChild((Node)(object)val2, false, (InternalMode)0);
				SubViewport view2 = _view;
				Line2D val3 = new Line2D();
				val3.Points = new Vector2[2]
				{
					new Vector2(17f, 18f),
					new Vector2(30f, 29f)
				};
				val3.Width = 2f;
				val3.DefaultColor = Colors.White;
				val3.Antialiased = true;
				((CanvasItem)val3).Material = (Material)(object)_material;
				((Node)view2).AddChild((Node)(object)val3, false, (InternalMode)0);
				if (!_hooked)
				{
					RenderingServer.FramePostDraw += Drew;
					((Node)Ticker.Tree.Root).TreeExiting += Clear;
					_hooked = true;
				}
				((Node)Ticker.Tree.Root).AddChild((Node)(object)_view, false, (InternalMode)0);
				if (val == _plain && ParticleMaterials.Count == 0)
				{
					ParticleMaterials.Add(FxKit.Burst((Node)(object)_view, new Vector2(8f, 24f), Colors.White, 4, 1f, 2f, 0.1, 0.1f, 0.2f, Vector2.Zero).ProcessMaterial);
					ParticleMaterials.Add(FxKit.Burst((Node)(object)_view, new Vector2(16f, 24f), Colors.White, 4, 1f, 2f, 0.1, 0.1f, 0.2f, Vector2.Zero, 180f, null, 10f, 0f, additive: false).ProcessMaterial);
					ParticleMaterials.Add(FxKit.Bubbles((Node)(object)_view, new Vector2(24f, 24f), Colors.White, 4, Vector2.One, 1f, 2f, 0.1, 0.1f, 0.2f).ProcessMaterial);
				}
				return false;
			}
			catch (Exception ex)
			{
				_failed = true;
				Clear();
				DevLog.Warn("Canvas prewarm disabled after failure: " + ex.Message);
				return true;
			}
		}

		private static void Drew()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_view) && (int)_view.RenderTargetUpdateMode != 0 && ++_draws >= 2)
			{
				_view.RenderTargetUpdateMode = (UpdateMode)0;
			}
		}

		internal static void Pause()
		{
			if (FxKit.Alive((GodotObject)(object)_view))
			{
				_view.RenderTargetUpdateMode = (UpdateMode)0;
			}
		}

		private static void ReleaseView()
		{
			if (FxKit.Alive((GodotObject)(object)_view))
			{
				((GodotObject)_view).Free();
			}
			_view = null;
			ShaderMaterial material = _material;
			if (material != null)
			{
				((GodotObject)material).Dispose();
			}
			_material = null;
			World2D world = _world;
			if (world != null)
			{
				((GodotObject)world).Dispose();
			}
			_world = null;
		}

		internal static void Clear()
		{
			Pending.Clear();
			ReleaseView();
			if (_hooked)
			{
				RenderingServer.FramePostDraw -= Drew;
				SceneTree tree = Ticker.Tree;
				if (((tree != null) ? tree.Root : null) != null)
				{
					((Node)Ticker.Tree.Root).TreeExiting -= Clear;
				}
				_hooked = false;
			}
		}
	}
