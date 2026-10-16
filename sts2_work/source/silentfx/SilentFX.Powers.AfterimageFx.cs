// namespace: SilentFX.Powers
// type: AfterimageFx (class)
// source lines: 1637-1953
	public sealed class AfterimageFx : PowerFx
	{
		private static readonly StringName PAlpha = StringName.op_Implicit("alpha");

		private const int Count = 3;

		private Node2D _back;

		private Node2D _flip;

		private FigureCopy _copy;

		private readonly List<CanvasItem> _echoes = new List<CanvasItem>();

		private readonly List<ShaderMaterial> _materials = new List<ShaderMaterial>();

		private Vector2 _quadSize;

		private bool _drawn;

		private float _time;

		private float _kick;

		private float _alpha;

		private float _alphaTarget = 1f;

		private bool _stopping;

		private bool _dead;

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

		public override void Start(bool animateIn)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected Obj, but got Unknown
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				_back = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_Afterimage")
				};
				Anchor.BackParent.AddChild((Node)(object)_back, false, (InternalMode)0);
				Vector2 val = Anchor.SafeBodySize();
				_quadSize = new Vector2(val.X * Tuning.F("afterimage.width_mul", 1.25f), val.Y * Tuning.F("afterimage.height_mul", 1.1f));
				CanvasItem val2 = Anchor.Body?.Invoke();
				if (val2 != null)
				{
					Vector2 box = new Vector2(val.X * Tuning.F("afterimage.copy_w", 2f), val.Y * Tuning.F("afterimage.copy_h", 1.45f));
					_copy = FigureCopy.Make(val2, box, (Node)(object)_back, "Afterimage");
				}
				if (_copy != null && _copy.Ok)
				{
					BuildOutlines();
				}
				else
				{
					BuildDrawn();
				}
				_alpha = (animateIn ? 0f : 1f);
				_kick = (animateIn ? 1f : 0f);
				Layout();
			}
			catch (Exception ex)
			{
				DevLog.Warn("AfterimageFx.Start: " + ex);
				Dispose();
			}
		}

		private void BuildOutlines()
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Expected Obj, but got Unknown
			_drawn = false;
			Texture2D texture = _copy.Texture;
			for (int i = 0; i < 3; i++)
			{
				ShaderMaterial val = ShaderLib.NewMaterial("echo_outline");
				if (val == null || texture == null)
				{
					DropCopy();
					BuildDrawn();
					break;
				}
				val.SetShaderParameter(StringName.op_Implicit("shift"), Variant.op_Implicit((float)i * 2.1f));
				val.SetShaderParameter(StringName.op_Implicit("edge_px"), Variant.op_Implicit(Tuning.F("afterimage.edge_px", 3f)));
				Sprite2D val2 = new Sprite2D
				{
					Name = StringName.op_Implicit("SilentFX_Echo" + i),
					Texture = texture,
					Centered = false,
					Scale = new Vector2(2f, 2f),
					Material = (Material)(object)val,
					TextureFilter = (TextureFilterEnum)2
				};
				((Node)_back).AddChild((Node)(object)val2, false, (InternalMode)0);
				_echoes.Add((CanvasItem)(object)val2);
				_materials.Add(val);
			}
		}

		private void BuildDrawn()
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected Obj, but got Unknown
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			_drawn = true;
			foreach (CanvasItem echo in _echoes)
			{
				if (FxKit.Alive((GodotObject)(object)echo))
				{
					((Node)echo).QueueFree();
				}
			}
			_echoes.Clear();
			_materials.Clear();
			_flip = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_AfterimageDrawn")
			};
			((Node)_back).AddChild((Node)(object)_flip, false, (InternalMode)0);
			for (int i = 0; i < 3; i++)
			{
				ShaderMaterial val = ShaderLib.NewMaterial("echo_figure");
				if (val != null)
				{
					val.SetShaderParameter(StringName.op_Implicit("shift"), Variant.op_Implicit((float)i * 2.1f));
					ColorRect val2 = FxKit.Quad(_quadSize, (Material)(object)val);
					((Node)_flip).AddChild((Node)(object)val2, false, (InternalMode)0);
					_echoes.Add((CanvasItem)(object)val2);
					_materials.Add(val);
				}
			}
		}

		private void DropCopy()
		{
			_copy?.Dispose();
			_copy = null;
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
				if (!_drawn && (_copy == null || !_copy.Ok))
				{
					DropCopy();
					BuildDrawn();
				}
				_time += (float)dt;
				_kick = FxKit.Damp(_kick, 0f, 3.2f, dt);
				_alpha = FxKit.Damp(_alpha, _alphaTarget, 4.5f, dt);
				Layout((float)dt);
				if (_stopping && _alpha < 0.03f)
				{
					Dispose();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("AfterimageFx.Update: " + ex);
				Dispose();
			}
		}

		private void Layout(float dt = 0f)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)_back))
			{
				return;
			}
			Vector2 val = Anchor.Feet();
			float num = Anchor.Side();
			_back.GlobalPosition = val;
			if (FxKit.Alive((GodotObject)(object)_flip))
			{
				_flip.Scale = new Vector2(num, 1f);
			}
			if (!_drawn)
			{
				_copy?.Follow(val, dt);
			}
			float num2 = Tuning.F("afterimage.gap", 30f);
			float num3 = Tuning.F("afterimage.alpha", 0.42f) * (_drawn ? 1f : Tuning.F("afterimage.copy_alpha", 1.15f));
			for (int i = 0; i < _echoes.Count; i++)
			{
				CanvasItem val2 = _echoes[i];
				if (!FxKit.Alive((GodotObject)(object)val2))
				{
					continue;
				}
				float num4 = (float)(i + 1) * num2 * (1f + 1.3f * _kick);
				float num5 = Mathf.Sin(_time * 2.2f - (float)i * 0.7f) * 6f;
				if (_drawn)
				{
					ColorRect val3 = (ColorRect)(object)((val2 is ColorRect) ? val2 : null);
					if (val3 != null)
					{
						((Control)val3).Position = new Vector2((0f - _quadSize.X) * 0.5f - num4 + num5, 0f - _quadSize.Y);
						goto IL_01a1;
					}
				}
				Sprite2D val4 = (Sprite2D)(object)((val2 is Sprite2D) ? val2 : null);
				if (val4 != null && _copy != null)
				{
					((Node2D)val4).Position = _copy.Origin - val + new Vector2(num * (0f - num4 + num5), 0f);
				}
				goto IL_01a1;
				IL_01a1:
				float num6 = Mathf.Pow(0.58f, (float)i);
				float num7 = num3 * num6 * (1f + 1.4f * _kick) * _alpha;
				if (i < _materials.Count && FxKit.Alive((GodotObject)(object)_materials[i]))
				{
					_materials[i].SetShaderParameter(PAlpha, Variant.op_Implicit(num7));
				}
			}
		}

		public override void Trigger(Vector2? at = null)
		{
			if (!_dead && !_stopping)
			{
				_kick = 1f;
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
			if (!_dead)
			{
				_dead = true;
				DropCopy();
				if (FxKit.Alive((GodotObject)(object)_back))
				{
					((Node)_back).QueueFree();
				}
			}
		}

		public static void Reset()
		{
			FigureCopy.Reset();
		}
	}
