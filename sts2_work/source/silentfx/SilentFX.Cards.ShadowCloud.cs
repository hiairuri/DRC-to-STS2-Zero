// namespace: SilentFX.Cards
// type: ShadowCloud (class)
// source lines: 91097-91512
	internal sealed class ShadowCloud
	{
		public const int Max = 48;

		private static readonly StringName PCount = StringName.op_Implicit("count");

		private static readonly StringName PAt = StringName.op_Implicit("at");

		private static readonly StringName PAx = StringName.op_Implicit("ax");

		private static readonly StringName PEx = StringName.op_Implicit("ex");

		private static readonly StringName PSx = StringName.op_Implicit("sx");

		private static readonly StringName PRun = StringName.op_Implicit("run");

		private static readonly StringName PCb = StringName.op_Implicit("cb");

		private static readonly StringName PHead = StringName.op_Implicit("head");

		private static readonly StringName PHeadR = StringName.op_Implicit("head_r");

		private static readonly StringName PSpin = StringName.op_Implicit("spin");

		private static readonly StringName PClock = StringName.op_Implicit("clock");

		private static readonly StringName PIntensity = StringName.op_Implicit("intensity");

		private static readonly StringName PPulse = StringName.op_Implicit("pulse");

		private static readonly StringName PVeil = StringName.op_Implicit("veil");

		public readonly Node2D Node;

		private readonly Node2D _frame;

		private readonly ColorRect _quad;

		private readonly ShaderMaterial _mat;

		private readonly Sprite2D _aura;

		private readonly Vector2[] _at = new Vector2[48];

		private readonly Vector2[] _ax = new Vector2[48];

		private readonly Vector2[] _ex = new Vector2[48];

		private readonly Vector2[] _sx = new Vector2[48];

		private readonly Vector2[] _run = new Vector2[48];

		private readonly float[] _cb = new float[48];

		private int _n;

		private Vector2 _lo;

		private Vector2 _hi;

		public bool Ok
		{
			get
			{
				if (_mat != null && FxKit.Alive((GodotObject)(object)Node) && FxKit.Alive((GodotObject)(object)_frame))
				{
					return FxKit.Alive((GodotObject)(object)_quad);
				}
				return false;
			}
		}

		public ShadowCloud(float seed, bool veil, bool aura)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Expected Obj, but got Unknown
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Expected Obj, but got Unknown
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			Node = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_ShadowCloud")
			};
			if (aura)
			{
				_aura = Shade.Aura(new Vector2(64f, 64f), 0f);
				((Node)Node).AddChild((Node)(object)_aura, false, (InternalMode)0);
			}
			_frame = new Node2D
			{
				Name = StringName.op_Implicit("Sheet"),
				Visible = false
			};
			((Node)Node).AddChild((Node)(object)_frame, false, (InternalMode)0);
			_mat = ShaderLib.NewMaterial("shadow_cloud");
			if (_mat != null)
			{
				_mat.SetShaderParameter(PVeil, Variant.op_Implicit(veil ? 1f : 0f));
				_mat.SetShaderParameter(StringName.op_Implicit("seed"), Variant.op_Implicit(seed));
				_mat.SetShaderParameter(PCount, Variant.op_Implicit(0));
				_mat.SetShaderParameter(StringName.op_Implicit("wisp"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("shadowstep.wisp", 1.25f), 0.3f, 3f)));
				_mat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("shadowstep.glow", 0.7f), 0f, 2f)));
				_mat.SetShaderParameter(StringName.op_Implicit("twist"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("shadowstep.twist", 1.5f), 0f, 4f)));
				_mat.SetShaderParameter(StringName.op_Implicit("core"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("shadowstep.core", 1.6f), 1f, 3f)));
				_mat.SetShaderParameter(StringName.op_Implicit("comb"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("shadowstep.comb", 3f), 1f, 8f)));
				_mat.SetShaderParameter(StringName.op_Implicit("sweep"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("shadowstep.sweep", 0.9f), 0f, 2f)));
				_quad = FxKit.Quad(new Vector2(8f, 8f), (Material)(object)_mat);
				_quad.Color = new Color(1f, 1f, 1f, 0f);
				((Node)_frame).AddChild((Node)(object)_quad, false, (InternalMode)0);
			}
		}

		public void Begin()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			_n = 0;
			_lo = new Vector2(1E+09f, 1E+09f);
			_hi = new Vector2(-1E+09f, -1E+09f);
		}

		public void Add(Vector2 at, float reach, float thick, float age, float seen)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			Add(at, reach, thick, age, seen, Vector2.Zero, Vector2.Zero, 0f);
		}

		public void Add(Vector2 at, float reach, float thick, float age, float seen, Vector2 drawn, Vector2 way, float combed)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			if (_n < 48 && !(reach < 1f) && !(thick <= 0.002f) && !(seen <= 0.003f) && float.IsFinite(at.X) && float.IsFinite(at.Y) && float.IsFinite(reach))
			{
				if (!float.IsFinite(drawn.X) || !float.IsFinite(drawn.Y))
				{
					drawn = Vector2.Zero;
				}
				if (!float.IsFinite(way.X) || !float.IsFinite(way.Y) || !float.IsFinite(combed))
				{
					way = Vector2.Zero;
					combed = 0f;
				}
				float num = drawn.Length();
				if (num > 7f)
				{
					drawn *= 7f / num;
					num = 7f;
				}
				_at[_n] = at;
				_ax[_n] = new Vector2(reach, Mathf.Clamp(thick, 0f, 1f));
				_ex[_n] = new Vector2(Mathf.Clamp(age, 0f, 1f), Mathf.Clamp(seen, 0f, 1f));
				_sx[_n] = drawn;
				_run[_n] = way;
				_cb[_n] = Mathf.Clamp(combed, 0f, 1f);
				_n++;
				float num2 = reach * 2.7f;
				float num3 = num2;
				float num4 = num2;
				if (num > 0.0001f)
				{
					Vector2 val = drawn / num;
					float num5 = (1f + num) * (1f + num) - 1f;
					num3 = num2 * Mathf.Sqrt(1f + num5 * val.X * val.X);
					num4 = num2 * Mathf.Sqrt(1f + num5 * val.Y * val.Y);
				}
				_lo = new Vector2(Mathf.Min(_lo.X, at.X - num3), Mathf.Min(_lo.Y, at.Y - num4));
				_hi = new Vector2(Mathf.Max(_hi.X, at.X + num3), Mathf.Max(_hi.Y, at.Y + num4));
			}
		}

		public void End(Vector2 head, float reach, float spin, float clock, float intensity = 1f, float pulse = 0f)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			if (!Ok)
			{
				return;
			}
			if (_n == 0 || intensity <= 0.003f)
			{
				((CanvasItem)_frame).Visible = false;
				return;
			}
			Rect2 val = Seen();
			Vector2 val2 = new Vector2(Mathf.Max(_lo.X, val.Position.X - 40f), Mathf.Max(_lo.Y, val.Position.Y - 40f));
			Vector2 val3 = new Vector2(Mathf.Min(_hi.X, val.End.X + 40f), Mathf.Min(_hi.Y, val.End.Y + 40f));
			if (val3.X - val2.X < 2f || val3.Y - val2.Y < 2f)
			{
				((CanvasItem)_frame).Visible = false;
				return;
			}
			for (int i = _n; i < 48; i++)
			{
				_at[i] = Vector2.Zero;
				_ax[i] = new Vector2(1f, 0f);
				_ex[i] = new Vector2(1f, 0f);
				_sx[i] = Vector2.Zero;
				_run[i] = Vector2.Zero;
				_cb[i] = 0f;
			}
			((CanvasItem)_frame).Visible = true;
			_frame.GlobalTransform = new Transform2D(0f, val2);
			((Control)_quad).Position = Vector2.Zero;
			((Control)_quad).Size = val3 - val2;
			_mat.SetShaderParameter(PCount, Variant.op_Implicit(_n));
			_mat.SetShaderParameter(PAt, Variant.op_Implicit(_at));
			_mat.SetShaderParameter(PAx, Variant.op_Implicit(_ax));
			_mat.SetShaderParameter(PEx, Variant.op_Implicit(_ex));
			_mat.SetShaderParameter(PSx, Variant.op_Implicit(_sx));
			_mat.SetShaderParameter(PRun, Variant.op_Implicit(_run));
			_mat.SetShaderParameter(PCb, Variant.op_Implicit(_cb));
			_mat.SetShaderParameter(PHead, Variant.op_Implicit(head));
			_mat.SetShaderParameter(PHeadR, Variant.op_Implicit(Mathf.Max(1f, reach)));
			_mat.SetShaderParameter(PSpin, Variant.op_Implicit(spin));
			_mat.SetShaderParameter(PClock, Variant.op_Implicit(clock));
			_mat.SetShaderParameter(PIntensity, Variant.op_Implicit(Mathf.Clamp(intensity, 0f, 1f)));
			_mat.SetShaderParameter(PPulse, Variant.op_Implicit(Mathf.Clamp(pulse, 0f, 1f)));
		}

		public void Thin(float veil)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			if (_mat != null && FxKit.Alive((GodotObject)(object)_mat))
			{
				_mat.SetShaderParameter(PVeil, Variant.op_Implicit(Mathf.Clamp(veil, 0f, 1f)));
			}
		}

		public void Aura(Vector2 at, float size, float alpha)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_aura))
			{
				((Node2D)_aura).GlobalPosition = at;
				((Node2D)_aura).GlobalRotation = 0f;
				Node parent = ((Node)_aura).GetParent();
				CanvasItem val = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
				Vector2 val2;
				if (val == null || !FxKit.Alive((GodotObject)(object)val))
				{
					val2 = Vector2.One;
				}
				else
				{
					Transform2D globalTransform = val.GetGlobalTransform();
					Vector2 scale = globalTransform.Scale;
					val2 = scale.Abs();
				}
				Vector2 val3 = val2;
				((Node2D)_aura).Scale = new Vector2(size / Mathf.Max(0.01f, val3.X), size / Mathf.Max(0.01f, val3.Y)) / 64f;
				((CanvasItem)_aura).Modulate = Shade.Glowing(Mathf.Clamp(alpha, 0f, 1f));
			}
		}

		public void Hide()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_frame))
			{
				((CanvasItem)_frame).Visible = false;
			}
			if (FxKit.Alive((GodotObject)(object)_aura))
			{
				((CanvasItem)_aura).Modulate = Shade.Glowing(0f);
			}
		}

		public void Free()
		{
			if (FxKit.Alive((GodotObject)(object)Node))
			{
				((Node)Node).QueueFree();
			}
		}

		private Rect2 Seen()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Viewport viewport = ((Node)_frame).GetViewport();
				if (viewport != null)
				{
					Transform2D canvasTransform = ((CanvasItem)_frame).GetCanvasTransform();
					Rect2 result = canvasTransform.AffineInverse() * viewport.GetVisibleRect();
					if (result.Size.X > 100f && result.Size.Y > 100f)
					{
						return result;
					}
				}
			}
			catch
			{
			}
			return new Rect2(Vector2.Zero, FxKit.ViewSize());
		}
	}
