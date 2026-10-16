// namespace: SilentFX.Cards
// type: AimMarkHeld (class)
// source lines: 84218-84281
	public abstract class AimMarkHeld : HeldSession
	{
		private static readonly StringName PAlpha = StringName.op_Implicit("alpha");

		private static readonly StringName PSpin = StringName.op_Implicit("spin");

		private Node2D _mark;

		private ShaderMaterial _markMat;

		private Vector2 _markAt;

		private float _size = 1f;

		private float _show;

		protected virtual Color Tint
		{
			get
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				return PoisonFx.Venom;
			}
		}

		protected virtual bool Marks => true;

		protected override void Build()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			_mark = HeldParts.AimRing((Node)(object)Front, 150f, Tint, out _markMat);
			_markAt = C.Chest;
		}

		protected override void Tick(float dt)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			bool flag = C.HasTarget && Marks;
			if (flag)
			{
				_markAt = C.TargetChest;
				_size = Mathf.Clamp(C.TargetWidth / 190f, 0.85f, 1.9f);
			}
			_show = FxKit.Damp(_show, flag ? 1f : 0f, 11f, dt);
			if (FxKit.Alive((GodotObject)(object)_mark))
			{
				_mark.GlobalPosition = _markAt;
				float num = Mathf.Lerp(1.7f, 1f, _show) * _size;
				_mark.Scale = new Vector2(num, num);
				if (FxKit.Alive((GodotObject)(object)_markMat))
				{
					_markMat.SetShaderParameter(PAlpha, Variant.op_Implicit(0.8f * _show));
					_markMat.SetShaderParameter(PSpin, Variant.op_Implicit(Time * 1.6f));
				}
			}
		}
	}
