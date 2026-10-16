// namespace: SilentFX.Cards
// type: PinpointHeld (class)
// source lines: 83308-83587
	public sealed class PinpointHeld : HeldSession
	{
		private Sprite2D _beam;

		private Sprite2D _core;

		private Node2D _mark;

		private ShaderMaterial _markMat;

		private Sprite2D _lens;

		private int _steps;

		private int _whole = 3;

		private float _lock;

		private int _racked;

		private float _rackClock;

		private float _reread;

		private float _focus;

		private float _kick;

		private float _show;

		private bool _shutting;

		private float _shut;

		private float _shutHeld;

		private int _struck;

		private Vector2 _shutAt;

		protected override void Build()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			Color color = new Color(PinpointFx.Needle.R, PinpointFx.Needle.G, PinpointFx.Needle.B, 0f);
			if (!C.Remote)
			{
				_beam = FxKit.Glow(color, new Vector2(64f, 64f));
				_core = FxKit.Glow(new Color(1f, 1f, 0.95f, 0f), new Vector2(64f, 64f));
			}
			_lens = FxKit.Glow(color, new Vector2(64f, 64f));
			if (!C.Remote)
			{
				((Node)Front).AddChild((Node)(object)_beam, false, (InternalMode)0);
				((Node)Front).AddChild((Node)(object)_core, false, (InternalMode)0);
			}
			if (!C.Remote && Tuning.F("pinpoint.reticle", 1f) > 0.01f)
			{
				_mark = PinpointFocus.Reticle(out _markMat);
				if (_mark != null)
				{
					((Node)Front).AddChild((Node)(object)_mark, false, (InternalMode)0);
				}
			}
			((Node)Front).AddChild((Node)(object)_lens, false, (InternalMode)0);
			PinpointFocus.Read(C.Card, out _steps, out _whole);
			if (PowerFxManager.LogTriggers && C.Card != null)
			{
				DevLog.Info($"Pinpoint: held with {_steps} of {_whole} notch(es) drawn in");
			}
		}

		protected override void Tick(float dt)
		{
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0337: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_033c: Unknown result type (might be due to invalid IL or missing references)
			//IL_033e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0401: Unknown result type (might be due to invalid IL or missing references)
			//IL_046d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0479: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0484: Unknown result type (might be due to invalid IL or missing references)
			//IL_0489: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_052a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0534: Unknown result type (might be due to invalid IL or missing references)
			//IL_0544: Unknown result type (might be due to invalid IL or missing references)
			//IL_054b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_05af: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_064e: Unknown result type (might be due to invalid IL or missing references)
			//IL_069a: Unknown result type (might be due to invalid IL or missing references)
			//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_077d: Unknown result type (might be due to invalid IL or missing references)
			//IL_078e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0798: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07af: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
			_reread += dt;
			if (!_shutting && _reread > 0.3f)
			{
				_reread = 0f;
				PinpointFocus.Read(C.Card, out _steps, out _whole);
			}
			if (_racked > _steps)
			{
				_racked = _steps;
			}
			if (_racked < _steps && Time > 0.1f)
			{
				_rackClock += dt;
				if (_rackClock >= 0.085f)
				{
					_rackClock = 0f;
					_racked++;
					_kick = 1f;
					if (_racked >= _whole)
					{
						_lock = 1f;
					}
				}
			}
			float target = (float)_racked / (float)Mathf.Max(1, _whole);
			_lock = FxKit.Damp(_lock, 0f, 9f, dt);
			_focus = FxKit.Damp(_focus, target, 24f, dt);
			_kick = FxKit.Damp(_kick, 0f, 11f, dt);
			float shut = 0f;
			float num = 0f;
			if (_shutting && !C.Remote)
			{
				float num2 = Mathf.Max(0.03f, Tuning.F("pinpoint.shut", 0.14f));
				bool flag = _shut >= 1f;
				bool flag2 = PinpointFocus.Struck != _struck;
				_shut = (flag2 ? 1f : Mathf.Min(1f, _shut + dt / num2));
				if (!flag && _shut >= 1f)
				{
					_lock = Mathf.Lerp(0.15f, 1f, Mathf.Clamp(_focus, 0f, 1f));
				}
				if (_shut >= 1f)
				{
					_shutHeld += dt;
				}
				if (flag2)
				{
					Leave(11f);
				}
				else if (_shutHeld > 1.5f)
				{
					Leave(14f);
				}
				shut = ShutCurve(_shut);
				num = Mathf.Clamp(_shut * 3f, 0f, 1f);
			}
			bool flag3 = !C.Remote && (_shutting || (C.HasTarget && !Playing));
			_show = FxKit.Damp(_show, flag3 ? 1f : 0f, 10f, dt);
			float num3 = Mathf.Clamp(_focus, 0f, 1f);
			float num4 = 1f - num3;
			float num5 = Strokes.Scale(C);
			Color needle = PinpointFx.Needle;
			float num6 = 0.6f + 0.4f * Arm;
			Vector2 weaponHand = C.WeaponHand;
			Vector2 val = (_shutting ? _shutAt : C.TargetChest) - weaponHand;
			float num7 = val.Length();
			float num8 = (num4 * 0.022f * Mathf.Sin(Time * 2.3f) + num4 * 0.01f * Mathf.Sin(Time * 5.1f + 1.3f)) * (1f - num);
			float num9 = ((num7 > 1f) ? val.Angle() : 0f) + num8;
			Vector2 val2 = new Vector2(Mathf.Cos(num9), Mathf.Sin(num9));
			Vector2 val3 = weaponHand + val2 * num7;
			float num10 = Mathf.Clamp(Tuning.F("pinpoint.still", 1.8f), 0f, 12f) * num5 * (1f - num);
			val3 += new Vector2(Mathf.Sin(Time * 1.31f) + 0.45f * Mathf.Sin(Time * 2.87f + 0.7f), 0.8f * Mathf.Sin(Time * 1.73f + 2.1f) + 0.4f * Mathf.Sin(Time * 3.41f + 4f)) * num10;
			val = val3 - weaponHand;
			num7 = val.Length();
			if (num7 > 1f)
			{
				num9 = val.Angle();
			}
			float num11 = 1f + 0.1f * num4 * Mathf.Sin(Time * 4.6f) * (1f - num);
			float num12 = 8f * Mathf.Sqrt(num5);
			if (FxKit.Alive((GodotObject)(object)_beam))
			{
				((Node2D)_beam).GlobalPosition = (weaponHand + val3) * 0.5f;
				((Node2D)_beam).GlobalRotation = num9;
				((Node2D)_beam).Scale = new Vector2(num7 * 1.08f, num12) / 64f;
				((CanvasItem)_beam).Modulate = new Color(needle.R, needle.G, needle.B, Mathf.Min(1f, Mathf.Lerp(0.22f, 0.36f, num3) + 0.15f * _kick) * num6 * _show);
			}
			if (FxKit.Alive((GodotObject)(object)_core))
			{
				((Node2D)_core).GlobalPosition = (weaponHand + val3) * 0.5f;
				((Node2D)_core).GlobalRotation = num9;
				((Node2D)_core).Scale = new Vector2(num7 * 1.02f, Mathf.Max(2.2f, num12 * 0.2f)) / 64f;
				((CanvasItem)_core).Modulate = new Color(1f, 1f, 0.95f, Mathf.Min(1f, Mathf.Lerp(0.4f, 0.85f, num3 * num3) + 0.2f * _kick) * num6 * _show);
			}
			if (FxKit.Alive((GodotObject)(object)_mark))
			{
				float num13 = PinpointFocus.Share(250f, 78f, num3) * num5 * num11 * (1f - 0.12f * _kick);
				_mark.GlobalPosition = val3;
				_mark.GlobalRotation = 0f;
				_mark.Scale = new Vector2(num13, num13) / 100f * Mathf.Clamp(Tuning.F("pinpoint.reticle", 1f), 0.3f, 3f);
				float num14 = Mathf.Clamp(_focus * (float)Mathf.Max(1, _whole), 0f, 1f);
				PinpointFocus.Show(_markMat, num3, num14 * num6 * _show * Mathf.Clamp(Fade, 0f, 1f), _lock, shut);
			}
			if (FxKit.Alive((GodotObject)(object)_lens))
			{
				float num15 = 26f * (1f + 0.25f * _kick);
				((Node2D)_lens).GlobalPosition = weaponHand;
				((Node2D)_lens).Scale = new Vector2(num15, num15) / 64f;
				((CanvasItem)_lens).Modulate = new Color(needle.R, needle.G, needle.B, Mathf.Lerp(0.22f, 0.55f, num3) * Mathf.Clamp(Lift, 0f, 1f) * (Playing ? 0f : 1f));
			}
		}

		private static float ShutCurve(float t)
		{
			if (t <= 0f)
			{
				return 0f;
			}
			if (t < 0.22f)
			{
				return -0.05f * Mathf.Sin(t / 0.22f * (float)Math.PI * 0.5f);
			}
			float num = Mathf.Min(1f, (t - 0.22f) / 0.78f);
			return -0.05f + 1.05f * num * num;
		}

		protected override void OnPlay(CardFxContext play)
		{
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			if (C.Remote)
			{
				Leave(14f);
				return;
			}
			_racked = _steps;
			if (!FxKit.Alive((GodotObject)(object)_mark) || _steps <= 0 || (!C.HasTarget && (play == null || !play.HasTarget)) || !(Tuning.F("pinpoint.shut", 0.14f) > 0.001f))
			{
				Leave(14f);
				return;
			}
			_shutting = true;
			Sfx.PlayAt("pinpoint_shut", Mathf.Max(0.03f, Tuning.F("pinpoint.shut", 0.14f)));
			_shut = 0f;
			_shutHeld = 0f;
			_struck = PinpointFocus.Struck;
			_shutAt = ((play != null && play.HasTarget) ? play.TargetChest : C.TargetChest);
		}
	}
