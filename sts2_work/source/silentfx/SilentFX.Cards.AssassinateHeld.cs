// namespace: SilentFX.Cards
// type: AssassinateHeld (class)
// source lines: 46098-46273
	public sealed class AssassinateHeld : HeldSession, IHeldOrigin
	{
		private ColorRect _veil;

		private ShadeMist _cloud;

		private ShadeMist _wrap;

		private Sprite2D _ember;

		private Sprite2D _emberCore;

		private Vector2 _at;

		private Vector2 _size = new Vector2(300f, 320f);

		private float _show;

		private float _beatWas;

		public Vector2 Origin
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _at;
			}
		}

		protected override void Build()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			_veil = FxKit.Veil(new Color(0.01f, 0f, 0.02f, 0f));
			((Node)Back).AddChild((Node)(object)_veil, false, (InternalMode)0);
			_cloud = new ShadeMist(1, 5.3f);
			_cloud.Set("wisp", 1.5f);
			_cloud.Set("glow", 0.35f);
			_cloud.Set("speed", 1.5f);
			((Node)Back).AddChild((Node)(object)_cloud.Root, false, (InternalMode)0);
			_wrap = new ShadeMist(1, 8.1f);
			_wrap.Set("wisp", 1.6f);
			_wrap.Set("glow", 0.3f);
			_wrap.Set("hollow", 0.55f);
			_wrap.Set("speed", 1.3f);
			((Node)Front).AddChild((Node)(object)_wrap.Root, false, (InternalMode)0);
			_ember = FxKit.Glow(new Color(AssassinateFx.Crimson.R, AssassinateFx.Crimson.G, AssassinateFx.Crimson.B, 0f), new Vector2(64f, 64f));
			_emberCore = FxKit.Glow(new Color(1f, 0.86f, 0.84f, 0f), new Vector2(64f, 64f));
			((Node)Front).AddChild((Node)(object)_ember, false, (InternalMode)0);
			((Node)Front).AddChild((Node)(object)_emberCore, false, (InternalMode)0);
			_at = C.Chest;
		}

		protected override void Tick(float dt)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03af: Unknown result type (might be due to invalid IL or missing references)
			//IL_03da: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_044c: Unknown result type (might be due to invalid IL or missing references)
			//IL_048f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			bool flag = C.HasTarget && !Playing;
			float targetSide = C.TargetSide;
			if (C.HasTarget)
			{
				_at = AssassinateFx.Behind(C.TargetChest, C.TargetWidth, C.TargetHeight, targetSide);
				_size = AssassinateFx.CloudSize(C.TargetWidth, C.TargetHeight);
			}
			_show = FxKit.Damp(_show, flag ? 1f : 0f, 8f, dt);
			if (FxKit.Alive((GodotObject)(object)_veil))
			{
				_veil.Color = new Color(0.01f, 0f, 0.02f, C.Remote ? 0f : (Tuning.F("assassinate.dim", 0.26f) * _show));
			}
			float num = 0.55f + 0.45f * _show;
			float num2 = 1f + 0.04f * Mathf.Sin(Time * 2.1f);
			if (_cloud != null && FxKit.Alive((GodotObject)(object)_cloud.Root))
			{
				_cloud.Root.GlobalPosition = _at;
				_cloud.Size(_size.X * num * num2, _size.Y * num);
				_cloud.Intensity = 0.96f * _show;
			}
			if (_wrap != null && FxKit.Alive((GodotObject)(object)_wrap.Root))
			{
				_wrap.Root.GlobalPosition = _at + new Vector2((0f - targetSide) * _size.X * 0.22f, _size.Y * 0.08f);
				_wrap.Size(_size.X * 0.85f * num, _size.Y * 0.8f * num * num2);
				_wrap.Intensity = 0.6f * _show;
			}
			float num3 = Time * 1.25f % 1f;
			float num4 = Mathf.Max(Bump(num3, 0.08f), 0.7f * Bump(num3, 0.26f));
			if ((!C.Remote & flag) && !Hovering && _show > 0.5f)
			{
				float num5 = Sfx.Lead("assassin_beat") * 1.25f;
				if (Crossed(_beatWas, num3, Mathf.PosMod(0.08f - num5, 1f)))
				{
					Sfx.Play("assassin_beat");
				}
				if (Crossed(_beatWas, num3, Mathf.PosMod(0.26f - num5, 1f)))
				{
					Sfx.Play("assassin_beat", 0.7f);
				}
				Mood.Hold("assassinate", Sfx.Knob("mood.assassinate.volume", 0.75f), 1f, Sfx.Knob("mood.assassinate.far", 0.3f), Sfx.Knob("mood.assassinate.hollow", 0.2f), 0.5f, Mathf.Max(0.05f, Sfx.Knob("mood.assassinate.back", 0.5f)));
			}
			_beatWas = num3;
			Vector2 globalPosition = _at + new Vector2((0f - targetSide) * _size.X * 0.1f, (0f - _size.Y) * 0.06f);
			float num6 = Mathf.Clamp(_size.Y / 300f, 0.9f, 1.8f);
			if (FxKit.Alive((GodotObject)(object)_ember))
			{
				float num7 = (90f + 50f * num4) * num6;
				((Node2D)_ember).GlobalPosition = globalPosition;
				((Node2D)_ember).Scale = new Vector2(num7, num7) / 64f;
				((CanvasItem)_ember).Modulate = new Color(AssassinateFx.Crimson.R, AssassinateFx.Crimson.G, AssassinateFx.Crimson.B, (0.3f + 0.4f * num4) * _show);
			}
			if (FxKit.Alive((GodotObject)(object)_emberCore))
			{
				float num8 = (20f + 14f * num4) * num6;
				((Node2D)_emberCore).GlobalPosition = globalPosition;
				((Node2D)_emberCore).Scale = new Vector2(num8, num8) / 64f;
				((CanvasItem)_emberCore).Modulate = new Color(1f, 0.86f, 0.84f, (0.55f + 0.45f * num4) * _show);
			}
		}

		private static bool Crossed(float was, float now, float point)
		{
			if (!(was <= now))
			{
				if (!(point > was))
				{
					return point <= now;
				}
				return true;
			}
			if (was < point)
			{
				return point <= now;
			}
			return false;
		}

		private static float Bump(float phase, float at)
		{
			float num = (phase - at) / 0.07f;
			return Mathf.Exp((0f - num) * num);
		}

		protected override void OnPlay(CardFxContext play)
		{
			Leave();
		}

		public AssassinateHeld()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		}
	}
