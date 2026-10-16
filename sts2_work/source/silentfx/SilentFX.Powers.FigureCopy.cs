// namespace: SilentFX.Powers
// type: FigureCopy (class)
// source lines: 5978-6267
	internal sealed class FigureCopy
	{
		private static readonly uint[] Bits = new uint[4] { 262144u, 131072u, 65536u, 32768u };

		private static readonly bool[] Taken = new bool[Bits.Length];

		private static readonly Dictionary<CanvasItem, (int Count, bool Had)>[] Marked = new Dictionary<CanvasItem, (int, bool)>[4]
		{
			new Dictionary<CanvasItem, (int, bool)>(ReferenceEqualityComparer.Instance),
			new Dictionary<CanvasItem, (int, bool)>(ReferenceEqualityComparer.Instance),
			new Dictionary<CanvasItem, (int, bool)>(ReferenceEqualityComparer.Instance),
			new Dictionary<CanvasItem, (int, bool)>(ReferenceEqualityComparer.Instance)
		};

		public const float Scale = 0.5f;

		private int _slot = -1;

		private SubViewport _vp;

		private CanvasItem _body;

		private readonly HashSet<CanvasItem> _changed = new HashSet<CanvasItem>(ReferenceEqualityComparer.Instance);

		private Vector2 _box;

		private float _remark;

		private uint Bit => Bits[_slot];

		public bool Ok
		{
			get
			{
				if (FxKit.Alive((GodotObject)(object)_vp) && FxKit.Alive((GodotObject)(object)_body))
				{
					return ((Node)_body).IsInsideTree();
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

		public Vector2 Origin
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

		public static FigureCopy Make(CanvasItem body, Vector2 box, Node holder, string who)
		{
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Expected Obj, but got Unknown
			if (!FxKit.Alive((GodotObject)(object)body) || !((Node)body).IsInsideTree() || !FxKit.Alive((GodotObject)(object)holder))
			{
				return null;
			}
			if (Tuning.F("afterimage.copy", 1f) < 0.5f)
			{
				return null;
			}
			FigureCopy figureCopy = null;
			try
			{
				if (body.GetCanvasLayerNode() != null)
				{
					DevLog.Info(who + ": she is drawn on a canvas layer - the old drawn echo is used");
					return null;
				}
				Viewport viewport = ((Node)body).GetViewport();
				if (viewport == null)
				{
					return null;
				}
				int num = Array.IndexOf(Taken, value: false);
				if (num < 0)
				{
					DevLog.Info(who + ": no free copy layer - the old drawn echo is used");
					return null;
				}
				Taken[num] = true;
				Vector2I size = new Vector2I(Mathf.Max(16, (int)(box.X * 0.5f)), Mathf.Max(16, (int)(box.Y * 0.5f)));
				figureCopy = new FigureCopy
				{
					_body = body,
					_box = box,
					_slot = num
				};
				SubViewport val = new SubViewport
				{
					Name = StringName.op_Implicit("SilentFX_FigureCopy"),
					TransparentBg = true,
					Disable3D = true,
					Size = size,
					CanvasCullMask = figureCopy.Bit,
					RenderTargetUpdateMode = (UpdateMode)4,
					GuiDisableInput = true
				};
				((Viewport)val).World2D = viewport.World2D;
				holder.AddChild((Node)(object)val, false, (InternalMode)0);
				figureCopy._vp = val;
				figureCopy.MarkAll();
				DevLog.Info(who + ": the echoes are drawn from a copy of her own picture (layer " + (18 - num) + ", " + figureCopy._changed.Count + " node(s) marked)");
				return figureCopy;
			}
			catch (Exception ex)
			{
				DevLog.Warn(who + " (figure copy): " + ex.Message);
				figureCopy?.Dispose();
				return null;
			}
		}

		private void MarkAll()
		{
			Mark((Node)(object)_body);
			for (Node parent = ((Node)_body).GetParent(); parent != null; parent = parent.GetParent())
			{
				CanvasItem val = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
				if (val != null)
				{
					Add(val);
				}
			}
		}

		private void Add(CanvasItem item)
		{
			if (_changed.Contains(item))
			{
				return;
			}
			Dictionary<CanvasItem, (int, bool)> dictionary = Marked[_slot];
			if (dictionary.TryGetValue(item, out var value))
			{
				dictionary[item] = (value.Item1 + 1, value.Item2);
			}
			else
			{
				bool flag = (item.VisibilityLayer & Bit) != 0;
				dictionary[item] = (1, flag);
				if (!flag)
				{
					item.VisibilityLayer |= Bit;
				}
			}
			_changed.Add(item);
		}

		private void Mark(Node n)
		{
			if ((object)n != _body && ((object)n.Name).ToString().StartsWith("SilentFX"))
			{
				return;
			}
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

		public void Follow(Vector2 feet, float dt)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			if (!Ok)
			{
				return;
			}
			Origin = feet - new Vector2(_box.X * 0.5f, _box.Y * 0.88f);
			((Viewport)_vp).CanvasTransform = new Transform2D(new Vector2(0.5f, 0f), new Vector2(0f, 0.5f), -Origin * 0.5f);
			_remark += dt;
			if (!(_remark >= 1f))
			{
				return;
			}
			_remark = 0f;
			try
			{
				Mark((Node)(object)_body);
			}
			catch
			{
			}
		}

		private static void Unmark(CanvasItem item, uint bit, bool had)
		{
			try
			{
				if (!had && FxKit.Alive((GodotObject)(object)item))
				{
					item.VisibilityLayer &= ~bit;
				}
			}
			catch
			{
			}
		}

		public static void Reset()
		{
			for (int i = 0; i < Bits.Length; i++)
			{
				foreach (KeyValuePair<CanvasItem, (int, bool)> item in Marked[i])
				{
					Unmark(item.Key, Bits[i], item.Value.Item2);
				}
				Marked[i].Clear();
				Taken[i] = false;
			}
		}

		public void Dispose()
		{
			if (_slot >= 0)
			{
				Dictionary<CanvasItem, (int, bool)> dictionary = Marked[_slot];
				foreach (CanvasItem item in _changed)
				{
					if (dictionary.TryGetValue(item, out var value))
					{
						if (value.Item1 > 1)
						{
							dictionary[item] = (value.Item1 - 1, value.Item2);
							continue;
						}
						dictionary.Remove(item);
						Unmark(item, Bit, value.Item2);
					}
				}
				Taken[_slot] = false;
				_slot = -1;
			}
			_changed.Clear();
			if (FxKit.Alive((GodotObject)(object)_vp))
			{
				((Node)_vp).QueueFree();
			}
			_vp = null;
			_body = null;
		}
	}
