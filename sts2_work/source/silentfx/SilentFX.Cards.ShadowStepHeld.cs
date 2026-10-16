// namespace: SilentFX.Cards
// type: ShadowStepHeld (class)
// source lines: 92013-92616
	public sealed class ShadowStepHeld : HeldSession
	{
		private const float BankWidth = 820f;

		private const float BankHeight = 700f;

		private const float BankX = -170f;

		private const float BankY = 10f;

		private static readonly List<ShadowStepHeld> Sessions = new List<ShadowStepHeld>();

		private static readonly Action<double> Driver = Drive;

		private static bool _ticking;

		private static bool _idle = true;

		private static bool _played;

		private static Node2D _under;

		private static ColorRect _veil;

		private static ShadeMist _pool;

		private static Sprite2D _poolGlow;

		private static Node2D _streamRoot;

		private static ShadowCloud _stream;

		private static Node2D _mass;

		private static ShadeMist _bank;

		private static HandShade _shade;

		private static CardModel _card;

		private static ShadowRoad _road;

		private static float _level;

		private static float _reach;

		private static float _bankLevel;

		private static float _arm;

		private static float _phase;

		private static float _clock;

		private static float _idleFor;

		private static Vector2 _at;

		private static float _turn;

		private static float _size = 0.8f;

		private static bool _placed;

		private static float _body = 240f;

		private ulong _tickedAt;

		internal static float Level
		{
			get
			{
				if (!_idle)
				{
					return Mathf.Clamp(_level, 0f, 1f);
				}
				return 0f;
			}
		}

		internal static float Reach
		{
			get
			{
				if (!_played && !_idle)
				{
					return Mathf.Clamp(_reach, 0f, 1f);
				}
				return 0f;
			}
		}

		internal static float Clock => _clock;

		internal static float Dark
		{
			get
			{
				if (!_idle)
				{
					return Mathf.Clamp(Tuning.F("shadowstep.dark", 0.42f), 0f, 1f) * Mathf.Clamp(_level, 0f, 1f);
				}
				return 0f;
			}
		}

		protected override void Build()
		{
			_tickedAt = FxClock.Milliseconds;
			if (!Sessions.Contains(this))
			{
				Sessions.Add(this);
			}
			if (_played)
			{
				_level = 0f;
				_reach = 0f;
				_bankLevel = 0f;
			}
			_played = false;
			_idle = false;
			Ensure(C);
			if (!_ticking)
			{
				_ticking = true;
				Ticker.Add(Driver);
			}
		}

		protected override void Tick(float dt)
		{
			_tickedAt = FxClock.Milliseconds;
			if (!Finished && !Sessions.Contains(this))
			{
				Sessions.Add(this);
				_idle = false;
			}
		}

		protected override void OnPlay(CardFxContext play)
		{
			Leave(18f);
		}

		protected override void OnEnd()
		{
			Sessions.Remove(this);
		}

		internal static void HandOver()
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			_played = true;
			try
			{
				_shade?.Free();
			}
			catch (Exception ex)
			{
				DevLog.Warn("Shadow Step standby (hand over): " + ex.Message);
			}
			_shade = null;
			try
			{
				_stream?.Hide();
				if (FxKit.Alive((GodotObject)(object)_veil))
				{
					_veil.Color = new Color(0.02f, 0f, 0.05f, 0f);
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("Shadow Step standby (hand over, hide): " + ex2.Message);
			}
		}

		public static void Reset()
		{
			Sessions.Clear();
			_played = false;
			TearDown();
			HandShade.ResetAll();
			ShadowStepFx.EndAll();
		}

		private static void Ensure(CardFxContext c)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected Obj, but got Unknown
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Expected Obj, but got Unknown
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Expected Obj, but got Unknown
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (c != null && FxKit.Alive((GodotObject)(object)c.BackParent) && (!FxKit.Alive((GodotObject)(object)_under) || ((Node)_under).GetParent() != c.BackParent))
				{
					if (FxKit.Alive((GodotObject)(object)_under))
					{
						((Node)_under).QueueFree();
					}
					_under = new Node2D
					{
						Name = StringName.op_Implicit("SilentFX_ShadowStandbyUnder")
					};
					TimeFreeze.KeepLive((Node)(object)_under);
					c.BackParent.AddChild((Node)(object)_under, false, (InternalMode)0);
					_veil = FxKit.Veil(new Color(0.02f, 0f, 0.05f, 0f));
					((Node)_under).AddChild((Node)(object)_veil, false, (InternalMode)0);
					_poolGlow = Shade.Aura(new Vector2(64f, 64f), 0f);
					((Node)_under).AddChild((Node)(object)_poolGlow, false, (InternalMode)0);
					_pool = new ShadeMist(1, 2.7f);
					_pool.Set("wisp", 1.2f);
					_pool.Set("glow", 0.7f);
					_pool.Set("speed", 1.5f);
					((Node)_under).AddChild((Node)(object)_pool.Root, false, (InternalMode)0);
				}
				Node uiLayer = GameRefs.UiLayer;
				if (FxKit.Alive((GodotObject)(object)uiLayer) && (!FxKit.Alive((GodotObject)(object)_mass) || ((Node)_mass).GetParent() != uiLayer))
				{
					if (FxKit.Alive((GodotObject)(object)_streamRoot))
					{
						((Node)_streamRoot).QueueFree();
					}
					if (FxKit.Alive((GodotObject)(object)_mass))
					{
						((Node)_mass).QueueFree();
					}
					_streamRoot = new Node2D
					{
						Name = StringName.op_Implicit("SilentFX_ShadowStream"),
						ZIndex = 5
					};
					TimeFreeze.KeepLive((Node)(object)_streamRoot);
					uiLayer.AddChild((Node)(object)_streamRoot, false, (InternalMode)0);
					_stream = new ShadowCloud(3.7f, veil: false, aura: false);
					((Node)_streamRoot).AddChild((Node)(object)_stream.Node, false, (InternalMode)0);
					_mass = new Node2D
					{
						Name = StringName.op_Implicit("SilentFX_ShadowMass"),
						ZIndex = 6,
						Visible = false
					};
					TimeFreeze.KeepLive((Node)(object)_mass);
					uiLayer.AddChild((Node)(object)_mass, false, (InternalMode)0);
					_bank = new ShadeMist(2, 4.2f);
					_bank.Set("wisp", 1.25f);
					_bank.Set("glow", 0.7f);
					_bank.Set("speed", 1.25f);
					_bank.Root.Position = new Vector2(-170f, 10f);
					_bank.Size(820f, 700f);
					((Node)_mass).AddChild((Node)(object)_bank.Root, false, (InternalMode)0);
					_placed = false;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Shadow Step standby (build): " + ex.Message);
			}
		}

		private static float Ease(float current, float target, float rate, float dt)
		{
			float num = FxKit.Damp(current, target, rate, dt);
			if (!(Mathf.Abs(num - target) < 0.003f))
			{
				return num;
			}
			return target;
		}

		private static void Drive(double realDt)
		{
			if (_idle)
			{
				return;
			}
			try
			{
				Advance((float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f)));
			}
			catch (Exception ex)
			{
				DevLog.Warn("Shadow Step standby: " + ex.Message);
				TearDown();
			}
		}

		private static void Advance(float dt)
		{
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0562: Unknown result type (might be due to invalid IL or missing references)
			//IL_0571: Unknown result type (might be due to invalid IL or missing references)
			//IL_0576: Unknown result type (might be due to invalid IL or missing references)
			//IL_059b: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_076d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0777: Unknown result type (might be due to invalid IL or missing references)
			//IL_077c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0616: Unknown result type (might be due to invalid IL or missing references)
			//IL_064d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0654: Unknown result type (might be due to invalid IL or missing references)
			//IL_0659: Unknown result type (might be due to invalid IL or missing references)
			//IL_065e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0670: Unknown result type (might be due to invalid IL or missing references)
			//IL_068a: Unknown result type (might be due to invalid IL or missing references)
			//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
			ShadowStepHeld shadowStepHeld = null;
			for (int num = Sessions.Count - 1; num >= 0; num--)
			{
				ShadowStepHeld shadowStepHeld2 = Sessions[num];
				ulong milliseconds = FxClock.Milliseconds;
				if (shadowStepHeld2.Finished || (milliseconds > shadowStepHeld2._tickedAt && milliseconds - shadowStepHeld2._tickedAt > 1000))
				{
					Sessions.RemoveAt(num);
				}
				else if (shadowStepHeld == null && !shadowStepHeld2.Leaving && !shadowStepHeld2.Playing)
				{
					shadowStepHeld = shadowStepHeld2;
				}
			}
			int num2;
			if (shadowStepHeld != null)
			{
				num2 = ((!_played) ? 1 : 0);
				if (num2 != 0)
				{
					_idleFor = 0f;
					_card = shadowStepHeld.C?.Card;
					Ensure(shadowStepHeld.C);
					goto IL_00c9;
				}
			}
			else
			{
				num2 = 0;
			}
			_idleFor += dt;
			goto IL_00c9;
			IL_00c9:
			bool flag = num2 != 0 || (!_played && _idleFor < Mathf.Max(0f, Tuning.F("shadowstep.hold", 0.2f)));
			_level = Ease(rate: Mathf.Max(1f, flag ? Tuning.F("shadowstep.fade_in", 8f) : (_played ? 14f : Tuning.F("shadowstep.fade_out", 6f))), current: _level, target: flag ? 1f : 0f, dt: dt);
			_clock += dt;
			if (_clock > 3600f)
			{
				_clock -= 3600f;
			}
			bool flag2 = false;
			if (!_played && (_level > 0f || !HandShade.Clear))
			{
				if (_shade == null)
				{
					_shade = new HandShade();
				}
				_shade.Update(_card, Tuning.F("shadowstep.hand_dark", 0.55f) * _level, dt, 16f);
				flag2 = true;
			}
			if ((num2 != 0 && shadowStepHeld.C != null) & flag2)
			{
				_road = ShadowRoad.Make(shadowStepHeld.C, _card, _shade.Hand);
				_body = Mathf.Max(shadowStepHeld.C.Body.X, 120f);
			}
			ShadowRoad road = _road;
			bool flag3 = num2 != 0 && !shadowStepHeld.Hovering;
			float num3 = 0f;
			if (flag3 && road != null)
			{
				float num4 = Pulled(road);
				float num5 = Mathf.Clamp(shadowStepHeld.Arm, 0f, 1f);
				num3 = (shadowStepHeld.Released ? 1f : Mathf.Lerp(num4, 1f, num5 * num5 * (3f - 2f * num5)));
			}
			if (!_played)
			{
				_reach = Ease(_reach, num3, (num3 > _reach) ? 13f : 10f, dt);
			}
			float num6 = Mathf.Clamp(_reach, 0f, 1f);
			float num7 = Mathf.Clamp((num6 - 0.6f) / 0.4f, 0f, 1f);
			num7 = num7 * num7 * (3f - 2f * num7);
			_bankLevel = (_played ? Ease(_bankLevel, 0f, 14f, dt) : Ease(_bankLevel, flag3 ? num7 : 0f, 12f, dt));
			_arm = Ease(_arm, (flag3 && shadowStepHeld.Armed) ? 1f : 0f, 6f, dt);
			_phase += dt * Mathf.Lerp(1.3f, 2.5f, _arm);
			if (_phase > 628.3185f)
			{
				_phase -= 628.3185f;
			}
			float num8 = Mathf.Clamp(_level, 0f, 1f);
			if (FxKit.Alive((GodotObject)(object)_veil) && !_played)
			{
				_veil.Color = new Color(0.02f, 0f, 0.05f, Mathf.Clamp(Tuning.F("shadowstep.dark", 0.42f), 0f, 1f) * num8);
			}
			if (FxKit.Alive((GodotObject)(object)_under) && _pool != null && FxKit.Alive((GodotObject)(object)_pool.Root) && road != null)
			{
				float num9 = num8 * (1f - 0.6f * num6);
				float num10 = 1f + 0.07f * Mathf.Sin(_clock * 5.3f) + 0.05f * Mathf.Sin(_clock * 8.9f + 1.3f);
				_pool.Root.GlobalPosition = road.Feet + new Vector2(0f, 4f);
				_pool.Size(_body * 1.55f * num10, _body * 0.42f * (2f - num10));
				_pool.Intensity = 0.85f * num9;
				_pool.Pulse = 0.25f + 0.25f * Mathf.Sin(_clock * 6.1f);
				if (FxKit.Alive((GodotObject)(object)_poolGlow))
				{
					((Node2D)_poolGlow).GlobalPosition = road.Feet + new Vector2(0f, 6f);
					((Node2D)_poolGlow).Scale = new Vector2(_body * 2f, _body * 0.55f) / 64f;
					((CanvasItem)_poolGlow).Modulate = Shade.Glowing(0.3f * num9);
				}
			}
			if (_stream != null && _stream.Ok && road != null && !_played)
			{
				_stream.Begin();
				if (num8 > 0.01f && num6 > 0.02f)
				{
					Vector2 val = road.In(num6) + new Vector2(4f * Mathf.Sin(_clock * 4.3f), 3f * Mathf.Sin(_clock * 5.9f + 0.7f)) * num6;
					float reach = road.ClumpReach(num6);
					ShadowRoad.LayBody(_stream, val, reach, Turn(_clock), _clock, 1f, num8, Vector2.Right, 0f);
					road.LayStream(_stream, _clock, num6, 0f, num8);
					_stream.End(val, reach, Turn(_clock), _clock);
				}
				else
				{
					_stream.End(Vector2.Zero, 1f, 0f, _clock, 0f);
				}
			}
			if (FxKit.Alive((GodotObject)(object)_mass) && _bank != null)
			{
				float num11 = num8 * _bankLevel;
				bool flag4 = num11 >= 0.01f;
				((CanvasItem)_mass).Visible = flag4;
				if (!flag4)
				{
					_placed = false;
				}
				else
				{
					if (flag2)
					{
						Aim(dt);
					}
					_mass.GlobalTransform = new Transform2D(_turn, new Vector2(_size, _size), 0f, _at);
					float num12 = Mathf.Lerp(-34f, 34f, _arm) + 12f * Mathf.Sin(_phase);
					_bank.Set("front", (num12 - -170f) / 410f);
					_bank.Set("reach", Mathf.Lerp(0.2f, 0.52f, _arm));
					_bank.Intensity = num11;
					_bank.Pulse = 0.35f * _arm;
				}
			}
			if (!flag && Sessions.Count == 0 && _level <= 0f && _bankLevel <= 0f && (_played || HandShade.Clear))
			{
				TearDown();
			}
		}

		internal static float Turn(float clock)
		{
			return clock * 1.6f;
		}

		private static float Pulled(ShadowRoad road)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				NCard val = HandCards.NodeOf(_card);
				float num;
				if (FxKit.Alive((GodotObject)(object)val) && ((Node)val).IsInsideTree())
				{
					CardStandIn.Pose(val, out var centre, out var _, out var _);
					num = centre.Y;
				}
				else
				{
					if (!FxKit.Alive((GodotObject)(object)_mass) || !((Node)_mass).IsInsideTree())
					{
						return 0f;
					}
					num = ((CanvasItem)_mass).GetGlobalMousePosition().Y - road.Tall * 0.25f;
				}
				float num2 = Mathf.Max(60f, road.View.Y * Mathf.Clamp(Tuning.F("shadowstep.pull", 0.28f), 0.05f, 0.8f));
				return Mathf.Clamp((road.Level - road.Tall * 0.06f - num) / num2, 0f, 1f);
			}
			catch
			{
				return 0f;
			}
		}

		private static void Aim(float dt)
		{
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			NCard val = null;
			List<HandCards.Entry> hand = _shade.Hand;
			for (int i = 0; i < hand.Count; i++)
			{
				if (hand[i].Card != _card && FxKit.Alive((GodotObject)(object)hand[i].Node))
				{
					val = hand[i].Node;
					break;
				}
			}
			Vector2 val2;
			float num2;
			float num3;
			if (val != null)
			{
				Transform2D globalTransform = ((CanvasItem)val).GetGlobalTransform();
				float num = (FxKit.Alive((GodotObject)(object)val.Body) ? val.Body.Scale.X : 1f);
				val2 = globalTransform.Origin;
				num2 = globalTransform.Rotation;
				num3 = Mathf.Clamp(globalTransform.Scale.X * num, 0.3f, 2f);
			}
			else
			{
				Vector2 val3 = FxKit.ViewSize();
				val2 = new Vector2(val3.X * 0.34f, val3.Y * 0.9f);
				num2 = 0f;
				num3 = 0.8f;
			}
			if (!_placed)
			{
				_at = val2;
				_turn = num2;
				_size = num3;
				_placed = true;
			}
			else
			{
				float num4 = 1f - Mathf.Exp(-12f * dt);
				_at = _at.Lerp(val2, num4);
				_turn = Mathf.LerpAngle(_turn, num2, num4);
				_size = Mathf.Lerp(_size, num3, num4);
			}
		}

		private static void TearDown()
		{
			_idle = true;
			_level = 0f;
			_reach = 0f;
			_bankLevel = 0f;
			_arm = 0f;
			_idleFor = 0f;
			_placed = false;
			try
			{
				_shade?.Free();
			}
			catch (Exception ex)
			{
				DevLog.Warn("Shadow Step standby (clean up): " + ex.Message);
			}
			_shade = null;
			_card = null;
			_road = null;
			if (FxKit.Alive((GodotObject)(object)_under))
			{
				((Node)_under).QueueFree();
			}
			if (FxKit.Alive((GodotObject)(object)_streamRoot))
			{
				((Node)_streamRoot).QueueFree();
			}
			if (FxKit.Alive((GodotObject)(object)_mass))
			{
				((Node)_mass).QueueFree();
			}
			_under = null;
			_veil = null;
			_pool = null;
			_poolGlow = null;
			_streamRoot = null;
			_stream = null;
			_mass = null;
			_bank = null;
		}
	}
