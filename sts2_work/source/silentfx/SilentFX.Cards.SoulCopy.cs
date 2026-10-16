// namespace: SilentFX.Cards
// type: SoulCopy (class)
// source lines: 110610-110918
	internal sealed class SoulCopy
	{
		private const uint Bit = 524288u;

		private static readonly Dictionary<CanvasItem, (int Count, uint Was)> Marked = new Dictionary<CanvasItem, (int, uint)>(ReferenceEqualityComparer.Instance);

		private const float Scale = 0.5f;

		private SubViewport _vp;

		private Node2D _body;

		private readonly List<CanvasItem> _changed = new List<CanvasItem>();

		public bool Ok
		{
			get
			{
				if (FxKit.Alive((GodotObject)(object)_vp))
				{
					return FxKit.Alive((GodotObject)(object)_body);
				}
				return false;
			}
		}

		public Texture2D Texture
		{
			get
			{
				if (!FxKit.Alive((GodotObject)(object)_vp))
				{
					return null;
				}
				return (Texture2D)(object)((Viewport)_vp).GetTexture();
			}
		}

		public Vector2 TPos
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return field;
			}
			[CompilerGenerated]
			private set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				field = value;
			}
		}

		public Vector2 TSize
		{
			[CompilerGenerated]
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return field;
			}
			[CompilerGenerated]
			private set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				field = value;
			}
		}

		public static SoulCopy Make(Creature target, Node holder)
		{
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Expected Obj, but got Unknown
			if (target == null || !FxKit.Alive((GodotObject)(object)holder) || Tuning.F("suppress.v3.soul_copy", 1f) < 0.5f)
			{
				return null;
			}
			SoulCopy soulCopy = null;
			try
			{
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(target) : null);
				if (!FxKit.Alive((GodotObject)(object)val) || !FxKit.Alive((GodotObject)(object)val.Body) || !((Node)val.Body).IsInsideTree())
				{
					return null;
				}
				Node2D body = val.Body;
				if (((CanvasItem)body).GetCanvasLayerNode() != null)
				{
					DevLog.Info("Suppress: the enemy is drawn on a canvas layer - his soul is a plain ghost");
					return null;
				}
				Viewport viewport = ((Node)body).GetViewport();
				if (viewport == null)
				{
					return null;
				}
				Rect2 visibleRect = viewport.GetVisibleRect();
				Vector2 val2 = visibleRect.Size * 0.5f;
				if (val2.X < 32f || val2.Y < 32f)
				{
					return null;
				}
				soulCopy = new SoulCopy
				{
					_body = body
				};
				SubViewport val3 = new SubViewport
				{
					Name = StringName.op_Implicit("SilentFX_SoulCopy"),
					TransparentBg = true,
					Disable3D = true,
					Size = new Vector2I((int)val2.X, (int)val2.Y),
					CanvasCullMask = 524288u,
					RenderTargetUpdateMode = (UpdateMode)4,
					GuiDisableInput = true
				};
				((Viewport)val3).World2D = viewport.World2D;
				holder.AddChild((Node)(object)val3, false, (InternalMode)0);
				soulCopy._vp = val3;
				soulCopy.Mark((Node)(object)body);
				for (Node parent = ((Node)body).GetParent(); parent != null; parent = parent.GetParent())
				{
					CanvasItem val4 = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
					if (val4 != null)
					{
						soulCopy.Add(val4);
					}
				}
				soulCopy.Follow();
				DevLog.Info("Suppress: his soul is drawn from a copy of his own picture (" + soulCopy._changed.Count + " node(s) marked)");
				return soulCopy;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Suppress (soul copy): " + ex.Message);
				soulCopy?.Dispose();
				return null;
			}
		}

		private void Add(CanvasItem item)
		{
			if (!_changed.Contains(item))
			{
				if (Marked.TryGetValue(item, out (int, uint) value))
				{
					Marked[item] = (value.Item1 + 1, value.Item2);
				}
				else
				{
					uint visibilityLayer = item.VisibilityLayer;
					Marked[item] = (1, visibilityLayer);
					item.VisibilityLayer = visibilityLayer | 0x80000;
				}
				_changed.Add(item);
			}
		}

		private void Mark(Node n)
		{
			CanvasItem val = (CanvasItem)(object)((n is CanvasItem) ? n : null);
			if (val != null)
			{
				Add(val);
			}
			foreach (Node child in n.GetChildren(true))
			{
				Mark(child);
			}
		}

		public void Follow()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			if (Ok && (int)_vp.RenderTargetUpdateMode != 0)
			{
				Transform2D canvasTransform = ((CanvasItem)_body).GetCanvasTransform();
				Transform2D identity = Transform2D.Identity;
				Transform2D canvasTransform2 = identity.Scaled(new Vector2(0.5f, 0.5f)) * canvasTransform;
				((Viewport)_vp).CanvasTransform = canvasTransform2;
				Transform2D val = canvasTransform2.AffineInverse();
				Vector2 val2 = val * Vector2.Zero;
				Vector2 val3 = val * new Vector2((float)_vp.Size.X, (float)_vp.Size.Y);
				TPos = new Vector2(Mathf.Min(val2.X, val3.X), Mathf.Min(val2.Y, val3.Y));
				Vector2 val4 = val3 - val2;
				TSize = val4.Abs();
			}
		}

		public static void Reset()
		{
			foreach (KeyValuePair<CanvasItem, (int, uint)> item in Marked)
			{
				try
				{
					if (FxKit.Alive((GodotObject)(object)item.Key) && (item.Value.Item2 & 0x80000) == 0)
					{
						CanvasItem key = item.Key;
						key.VisibilityLayer &= 0xFFF7FFFFu;
					}
				}
				catch
				{
				}
			}
			Marked.Clear();
		}

		public void Freeze()
		{
			if (FxKit.Alive((GodotObject)(object)_vp))
			{
				_vp.RenderTargetUpdateMode = (UpdateMode)0;
			}
		}

		public void Dispose()
		{
			foreach (CanvasItem item in _changed)
			{
				try
				{
					if (!Marked.TryGetValue(item, out (int, uint) value))
					{
						continue;
					}
					if (value.Item1 > 1)
					{
						Marked[item] = (value.Item1 - 1, value.Item2);
						continue;
					}
					Marked.Remove(item);
					if (FxKit.Alive((GodotObject)(object)item) && (value.Item2 & 0x80000) == 0)
					{
						item.VisibilityLayer &= 0xFFF7FFFFu;
					}
				}
				catch
				{
				}
			}
			_changed.Clear();
			if (FxKit.Alive((GodotObject)(object)_vp))
			{
				((Node)_vp).QueueFree();
			}
			_vp = null;
			_body = null;
		}

		public SoulCopy()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			TSize = Vector2.One;
			base..ctor();
		}
	}
