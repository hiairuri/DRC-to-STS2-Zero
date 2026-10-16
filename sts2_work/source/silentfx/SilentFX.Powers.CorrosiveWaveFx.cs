// namespace: SilentFX.Powers
// type: CorrosiveWaveFx (class)
// source lines: 4645-5520
	public sealed class CorrosiveWaveFx : BasicPowerFx
	{
		public struct Floor
		{
			public Vector2 Her;

			public Vector2 View;

			public float Far;

			public float Vanish;

			public float Focal;

			public float Left => -200f;

			public float Right => View.X + 200f;

			public float Near => View.Y + 200f;

			public float Seen
			{
				get
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0028: Unknown result type (might be due to invalid IL or missing references)
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					//IL_0069: Unknown result type (might be due to invalid IL or missing references)
					float num = Mathf.Max(Distance(new Vector2(0f, Far)), Distance(new Vector2(View.X, Far)));
					float num2 = Mathf.Max(Distance(new Vector2(0f, View.Y)), Distance(new Vector2(View.X, View.Y)));
					return Mathf.Max(num, num2);
				}
			}

			public float All
			{
				get
				{
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_0040: Unknown result type (might be due to invalid IL or missing references)
					//IL_0057: Unknown result type (might be due to invalid IL or missing references)
					float num = Mathf.Max(Distance(new Vector2(Left, Far)), Distance(new Vector2(Right, Far)));
					float num2 = Mathf.Max(Distance(new Vector2(Left, Near)), Distance(new Vector2(Right, Near)));
					return Mathf.Max(num, num2) + 320f;
				}
			}

			public float Scale(float y)
			{
				float num = Far - Vanish;
				return Mathf.Max(0.04f, (Mathf.Max(y, Far + 1f) - num) / Mathf.Max(1f, Her.Y - num));
			}

			public float Distance(Vector2 point)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				float num = Scale(point.Y);
				float num2 = (point.X - Her.X) / num;
				float num3 = Focal * (1f / num - 1f);
				return Mathf.Sqrt(num2 * num2 + num3 * num3);
			}
		}

		public sealed class Flood
		{
			private static readonly StringName PSize = StringName.op_Implicit("size");

			private static readonly StringName PHorizon = StringName.op_Implicit("horizon");

			private static readonly StringName PVanish = StringName.op_Implicit("vanish");

			private static readonly StringName PFocal = StringName.op_Implicit("focal");

			private static readonly StringName PHer = StringName.op_Implicit("her");

			private static readonly StringName PFarSoft = StringName.op_Implicit("far_soft");

			private static readonly StringName PReach = StringName.op_Implicit("reach");

			private static readonly StringName PCrest = StringName.op_Implicit("crest");

			private static readonly StringName PFlow = StringName.op_Implicit("flow");

			private static readonly StringName PPulseR = StringName.op_Implicit("pulse_r");

			private static readonly StringName PPulseA = StringName.op_Implicit("pulse_a");

			private static readonly StringName PFeet = StringName.op_Implicit("feet");

			private static readonly StringName PFeetN = StringName.op_Implicit("feet_n");

			private static readonly StringName PKick = StringName.op_Implicit("kick");

			private static readonly StringName PStain = StringName.op_Implicit("stain");

			private static readonly StringName PGlow = StringName.op_Implicit("glow");

			private static readonly StringName PHaze = StringName.op_Implicit("haze");

			private static readonly StringName PBubbles = StringName.op_Implicit("bubbles");

			private static readonly StringName PWade = StringName.op_Implicit("wade");

			private static readonly StringName PAlpha = StringName.op_Implicit("alpha");

			private Node2D _backHolder;

			private Node2D _frontHolder;

			private ColorRect _backQuad;

			private ColorRect _frontQuad;

			private ShaderMaterial _backMat;

			private ShaderMaterial _frontMat;

			private readonly Vector4[] _feet = new Vector4[8];

			private readonly Vector4[] _local = new Vector4[8];

			private int _feetCount;

			public float Alpha = 1f;

			public float Reach = 100000f;

			public float Crest;

			public float Flow;

			public float Kick;

			public float Bubbles = 1f;

			public float Haze = 1f;

			public readonly float[] PulseAt = new float[4] { -1f, -1f, -1f, -1f };

			public readonly float[] PulseGain = new float[4];

			public static Flood Make(Node back, Node front)
			{
				if (!FxKit.Alive((GodotObject)(object)back))
				{
					return null;
				}
				ShaderMaterial val = ShaderLib.NewMaterial("acid_tide");
				if (val == null)
				{
					return null;
				}
				float seed = (float)GD.RandRange(0.0, 40.0);
				Flood flood = new Flood();
				flood._backMat = val;
				flood._backHolder = Sheet(back, "Flood", val, 0f, seed, out flood._backQuad);
				if (FxKit.Alive((GodotObject)(object)front) && Tuning.F("wave.flood_wade", 9f) >= 0.5f)
				{
					ShaderMaterial val2 = ShaderLib.NewMaterial("acid_tide");
					if (val2 != null)
					{
						flood._frontMat = val2;
						flood._frontHolder = Sheet(front, "FloodFilm", val2, 1f, seed, out flood._frontQuad);
					}
				}
				return flood;
			}

			private static Node2D Sheet(Node parent, string name, ShaderMaterial mat, float layer, float seed, out ColorRect quad)
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Expected Obj, but got Unknown
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				mat.SetShaderParameter(PAlpha, Variant.op_Implicit(0f));
				mat.SetShaderParameter(StringName.op_Implicit("layer"), Variant.op_Implicit(layer));
				mat.SetShaderParameter(StringName.op_Implicit("seed"), Variant.op_Implicit(seed));
				Node2D val = new Node2D
				{
					Name = StringName.op_Implicit(name)
				};
				parent.AddChild((Node)(object)val, false, (InternalMode)0);
				quad = FxKit.Quad(new Vector2(64f, 64f), (Material)(object)mat);
				((Node)val).AddChild((Node)(object)quad, false, (InternalMode)0);
				return val;
			}

			public void ClearFeet()
			{
				_feetCount = 0;
			}

			public void Stand(Vector2 feet, float halfWidth, float ripple)
			{
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				if (_feetCount < 8)
				{
					_feet[_feetCount++] = new Vector4(feet.X, feet.Y, Mathf.Max(10f, halfWidth), ripple);
				}
			}

			public void Show(Floor floor)
			{
				Place(_backHolder, _backQuad, _backMat, floor);
				Place(_frontHolder, _frontQuad, _frontMat, floor);
			}

			private void Place(Node2D holder, ColorRect quad, ShaderMaterial mat, Floor floor)
			{
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_012b: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_013d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0154: Unknown result type (might be due to invalid IL or missing references)
				//IL_016b: Unknown result type (might be due to invalid IL or missing references)
				//IL_017d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0182: Unknown result type (might be due to invalid IL or missing references)
				//IL_0183: Unknown result type (might be due to invalid IL or missing references)
				//IL_0188: Unknown result type (might be due to invalid IL or missing references)
				//IL_019f: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
				//IL_020d: Unknown result type (might be due to invalid IL or missing references)
				//IL_023d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0242: Unknown result type (might be due to invalid IL or missing references)
				//IL_0272: Unknown result type (might be due to invalid IL or missing references)
				//IL_0277: Unknown result type (might be due to invalid IL or missing references)
				//IL_028d: Unknown result type (might be due to invalid IL or missing references)
				//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
				//IL_0306: Unknown result type (might be due to invalid IL or missing references)
				//IL_033b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0360: Unknown result type (might be due to invalid IL or missing references)
				//IL_037f: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0112: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)holder) && FxKit.Alive((GodotObject)(object)quad) && FxKit.Alive((GodotObject)(object)mat))
				{
					float num = floor.Vanish * 1.3f;
					Vector2 val = new Vector2(floor.Left, floor.Far - num);
					Vector2 val2 = new Vector2(floor.Right - floor.Left, floor.Near - val.Y);
					float num2 = FogKit.ScaleOf(((Node)holder).GetParent());
					holder.GlobalPosition = val;
					((Control)quad).Size = val2 / num2;
					for (int i = 0; i < 8; i++)
					{
						_local[i] = ((i < _feetCount) ? new Vector4(_feet[i].X - val.X, _feet[i].Y - val.Y, _feet[i].Z, _feet[i].W) : new Vector4(0f, 0f, 0f, -1f));
					}
					mat.SetShaderParameter(PSize, Variant.op_Implicit(val2));
					mat.SetShaderParameter(PHorizon, Variant.op_Implicit(num));
					mat.SetShaderParameter(PVanish, Variant.op_Implicit(floor.Vanish));
					mat.SetShaderParameter(PFocal, Variant.op_Implicit(floor.Focal));
					mat.SetShaderParameter(PHer, Variant.op_Implicit(floor.Her - val));
					mat.SetShaderParameter(PFarSoft, Variant.op_Implicit(Mathf.Max(18f, (floor.Her.Y - floor.Far) * 0.9f)));
					mat.SetShaderParameter(PReach, Variant.op_Implicit(Reach));
					mat.SetShaderParameter(PCrest, Variant.op_Implicit(Mathf.Clamp(Crest, 0f, 1f)));
					mat.SetShaderParameter(PFlow, Variant.op_Implicit(Flow));
					mat.SetShaderParameter(PPulseR, Variant.op_Implicit(new Vector4(PulseAt[0], PulseAt[1], PulseAt[2], PulseAt[3])));
					mat.SetShaderParameter(PPulseA, Variant.op_Implicit(new Vector4(PulseGain[0], PulseGain[1], PulseGain[2], PulseGain[3])));
					mat.SetShaderParameter(PFeet, Variant.op_Implicit(_local));
					mat.SetShaderParameter(PFeetN, Variant.op_Implicit(_feetCount));
					mat.SetShaderParameter(PKick, Variant.op_Implicit(Mathf.Clamp(Kick, 0f, 1f)));
					mat.SetShaderParameter(PStain, Variant.op_Implicit(Tuning.F("wave.flood_stain", 0.46f)));
					mat.SetShaderParameter(PGlow, Variant.op_Implicit(Tuning.F("wave.flood_glow", 0.3f)));
					mat.SetShaderParameter(PHaze, Variant.op_Implicit(Tuning.F("wave.flood_haze", 0.06f) * Mathf.Clamp(Haze, 0f, 1f)));
					mat.SetShaderParameter(PBubbles, Variant.op_Implicit(Mathf.Clamp(Bubbles, 0f, 1f)));
					mat.SetShaderParameter(PWade, Variant.op_Implicit(Tuning.F("wave.flood_wade", 9f)));
					mat.SetShaderParameter(PAlpha, Variant.op_Implicit(Mathf.Clamp(Alpha, 0f, 1f)));
				}
			}
		}

		private sealed class RemotePool
		{
			internal float Reach;

			internal ulong Stamp;

			internal CardModel Card;

			internal Creature Owner;

			internal object Room;

			internal bool Consumed;
		}

		private const float Margin = 200f;

		private const float Everywhere = 100000f;

		private const int MaxFeet = 8;

		private const int MaxPulses = 4;

		private static float _heldReach;

		private static ulong _heldStamp;

		private static readonly ConditionalWeakTable<object, RemotePool> RemoteHandovers = new ConditionalWeakTable<object, RemotePool>();

		private Flood _flood;

		private Sprite2D _stain;

		private GpuParticles2D _bubbles;

		private List<FxTarget> _targets;

		private Floor _floor;

		private bool _surveyed;

		private float _far;

		private float _flow;

		private float _reach;

		private bool _arrived;

		private float _shown;

		private float _drainFrom;

		private readonly float[] _pulseAt = new float[4] { -1f, -1f, -1f, -1f };

		private readonly float[] _pulseGain = new float[4];

		private readonly List<object> _rippleKeys = new List<object>();

		private readonly List<float> _rippleAges = new List<float>();

		protected override string NodeName => "CorrosiveWave";

		protected override float KickDecay => 2.6f;

		protected override float FadeRate
		{
			get
			{
				if (!Stopping)
				{
					return 4.5f;
				}
				return 3.5f / (Mathf.Max(0.2f, Tuning.F("wave.flood_drain", 0.9f)) + 0.08f);
			}
		}

		public static Floor Survey(Vector2 her, Vector2 body, List<FxTarget> enemies, Vector2 view)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			if (body.X < 40f || body.Y < 40f)
			{
				body = new Vector2(200f, 300f);
			}
			float num = her.Y;
			if (enemies != null)
			{
				for (int i = 0; i < enemies.Count; i++)
				{
					FxTarget fxTarget = enemies[i];
					if (fxTarget != null)
					{
						num = Mathf.Min(num, Mathf.Clamp(fxTarget.Feet.Y, her.Y - body.Y * 0.25f, her.Y + body.Y));
					}
				}
			}
			return FloorAt(her, num - body.Y * Tuning.F("wave.flood_back", 0.16f), view);
		}

		public static Floor FloorAt(Vector2 her, float far, Vector2 view)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			Floor result = new Floor
			{
				Her = her,
				View = view,
				Far = Mathf.Min(far, her.Y - 8f)
			};
			result.Vanish = Mathf.Max(40f, (view.Y - result.Far) * Mathf.Clamp(Tuning.F("wave.flood_lift", 0.3f), 0.05f, 2f));
			result.Focal = (her.Y - (result.Far - result.Vanish)) / Mathf.Clamp(Tuning.F("wave.flood_tilt", 0.2f), 0.05f, 1f);
			return result;
		}

		public static void HandOver(float reach)
		{
			_heldReach = reach;
			_heldStamp = Time.GetTicksMsec();
		}

		internal static bool RecordRemoteHandover(float reach, CardModel card, Creature owner, object invocation, object room)
		{
			if (MultiplayerFx.Active && card != null && owner != null && invocation != null && room != null)
			{
				Player owner2 = card.Owner;
				if (((owner2 != null) ? owner2.Creature : null) == owner && !MultiplayerFx.IsLocal(owner))
				{
					RemoteHandovers.Remove(invocation);
					RemoteHandovers.Add(invocation, new RemotePool
					{
						Reach = reach,
						Stamp = Time.GetTicksMsec(),
						Card = card,
						Owner = owner,
						Room = room
					});
					return true;
				}
			}
			return false;
		}

		internal static void ResetRemoteHandovers()
		{
			RemoteHandovers.Clear();
		}

		private static bool TryTakeRemoteHandover(CardModel card, Creature owner, object invocation, object room, out float reach)
		{
			reach = 0f;
			if (invocation == null || !RemoteHandovers.TryGetValue(invocation, out var value))
			{
				return false;
			}
			ulong ticksMsec = Time.GetTicksMsec();
			if (value.Consumed || ticksMsec < value.Stamp || ticksMsec - value.Stamp >= 1500 || value.Card != card || value.Owner != owner || value.Room != room)
			{
				if (ticksMsec >= value.Stamp && ticksMsec - value.Stamp >= 1500)
				{
					RemoteHandovers.Remove(invocation);
				}
				return false;
			}
			value.Consumed = true;
			RemoteHandovers.Remove(invocation);
			reach = value.Reach;
			return true;
		}

		private float StartReach()
		{
			if (MultiplayerFx.Active)
			{
				object owner = Anchor.Owner;
				Creature val = (Creature)((owner is Creature) ? owner : null);
				if (val != null && !MultiplayerFx.IsLocal(val))
				{
					CardModel currentCard = MultiplayerFx.CurrentCard;
					object currentInvocation = MultiplayerFx.CurrentInvocation;
					NCombatRoom combatRoom = GameRefs.CombatRoom;
					if (currentCard != null && currentInvocation != null)
					{
						Player owner2 = currentCard.Owner;
						if (((owner2 != null) ? owner2.Creature : null) == val && TryTakeRemoteHandover(currentCard, val, currentInvocation, combatRoom, out var reach))
						{
							if (!(reach > 60f))
							{
								return 60f;
							}
							return Mathf.Min(reach, 700f);
						}
					}
					return 60f;
				}
			}
			ulong ticksMsec = Time.GetTicksMsec();
			if (ticksMsec < _heldStamp || ticksMsec - _heldStamp >= 1500 || !(_heldReach > 60f))
			{
				return 60f;
			}
			return Mathf.Min(_heldReach, 700f);
		}

		protected override void Build(bool animateIn)
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected Obj, but got Unknown
			Look(0f);
			_flood = Flood.Make((Node)(object)Back, (Node)(object)Front);
			if (_flood == null)
			{
				_stain = new Sprite2D
				{
					Texture = FxKit.SoftDot,
					Modulate = new Color(0.05f, 0.26f, 0.08f, 0f)
				};
				((Node)Back).AddChild((Node)(object)_stain, false, (InternalMode)0);
			}
			Color venom = PoisonFx.Venom;
			_bubbles = FxKit.Bubbles((Node)(object)Front, Vector2.Zero, new Color(venom.R, venom.G, venom.B, 0.75f), Tuning.I("wave.flood_bubbles", 26), new Vector2(300f, 60f), 16f, 60f, 1.1, 0.05f, 0.15f, oneShot: false);
			_arrived = !animateIn;
			_reach = (animateIn ? StartReach() : 100000f);
			if (animateIn)
			{
				Arrive();
			}
		}

		private void Look(float dt)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			_targets = Anchor.SafeTargets();
			Vector2 her = Anchor.Feet();
			Vector2 view = FxKit.ViewSize();
			Floor floor = Survey(her, Anchor.SafeBodySize(), _targets, view);
			_far = (_surveyed ? FxKit.Damp(_far, floor.Far, 4f, dt) : floor.Far);
			_surveyed = true;
			_floor = FloorAt(her, _far, view);
		}

		protected override void Tick(float dt)
		{
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			Look(dt);
			_flow += dt * (1f + 1.5f * Kick);
			if (!_arrived && !Stopping)
			{
				float num = Mathf.Max(200f, Mathf.Max(_floor.Her.X, _floor.View.X - _floor.Her.X));
				_reach += num / Mathf.Max(0.2f, Tuning.F("wave.flood_time", 0.85f)) * dt;
				if (_reach >= _floor.All)
				{
					_arrived = true;
				}
			}
			float num2 = Tuning.F("wave.flood_pulse_speed", 2300f);
			float num3 = _floor.Seen + 120f;
			for (int i = 0; i < 4; i++)
			{
				float num4 = _pulseAt[i];
				if (num4 < 0f)
				{
					continue;
				}
				float num5 = num4 + num2 * dt;
				if (!_arrived)
				{
					num5 = Mathf.Min(num5, Mathf.Max(num4, _reach));
				}
				_pulseAt[i] = num5;
				if (!Stopping && _targets != null)
				{
					for (int j = 0; j < _targets.Count; j++)
					{
						FxTarget fxTarget = _targets[j];
						if (fxTarget != null)
						{
							float num6 = _floor.Distance(fxTarget.Feet);
							if (num4 < num6 && num6 <= num5)
							{
								Splash(fxTarget);
							}
						}
					}
				}
				if (num5 >= num3)
				{
					_pulseAt[i] = -1f;
				}
			}
			for (int num7 = _rippleAges.Count - 1; num7 >= 0; num7--)
			{
				_rippleAges[num7] += dt;
				if (!(_rippleAges[num7] <= 0.9f))
				{
					_rippleAges.RemoveAt(num7);
					_rippleKeys.RemoveAt(num7);
				}
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			Pulse(1f);
		}

		protected override void OnStacked()
		{
			Pulse(1.25f);
		}

		private void Pulse(float gain)
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			float num2 = -1f;
			for (int i = 0; i < 4; i++)
			{
				if (_pulseAt[i] < 0f)
				{
					num = i;
					break;
				}
				if (_pulseAt[i] > num2)
				{
					num2 = _pulseAt[i];
					num = i;
				}
			}
			_pulseAt[num] = 0f;
			_pulseGain[num] = gain;
			Node frontParent = Anchor.FrontParent;
			if (FxKit.Alive((GodotObject)(object)frontParent))
			{
				Color venom = PoisonFx.Venom;
				FxKit.Flash(frontParent, Anchor.Feet() + new Vector2(0f, -8f), new Color(venom.R, venom.G, venom.B, 0.32f), Anchor.SafeBodySize().X * 1.3f, 1.7f, 0.26);
			}
		}

		private void Splash(FxTarget target)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			int num = _rippleKeys.IndexOf(target.Key);
			if (num >= 0)
			{
				_rippleAges[num] = 0f;
			}
			else
			{
				_rippleKeys.Add(target.Key);
				_rippleAges.Add(0f);
			}
			Node frontParent = Anchor.FrontParent;
			if (FxKit.Alive((GodotObject)(object)frontParent))
			{
				Color acid = PoisonFx.Acid;
				float num2 = Mathf.Clamp(target.Width, 120f, 520f);
				FxKit.Flash(frontParent, target.Feet + new Vector2(0f, -10f), new Color(acid.R, acid.G, acid.B, 0.4f), num2 * 0.7f, 1.7f);
				FxKit.Burst(frontParent, target.Feet + new Vector2(0f, -6f), new Color(acid.R, acid.G, acid.B, 0.95f), Tuning.I("wave.splash", 10), 180f, 420f, 0.5, 0.07f, 0.18f, new Vector2(0f, 900f), 32f, new Vector2(0f, -1f), num2 * 0.24f);
			}
		}

		private void Arrive()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			Node frontParent = Anchor.FrontParent;
			if (FxKit.Alive((GodotObject)(object)frontParent))
			{
				Color acid = PoisonFx.Acid;
				Vector2 val = Anchor.SafeBodySize();
				Vector2 val2 = Anchor.Feet();
				FxKit.Flash(frontParent, val2 + new Vector2(0f, -12f), new Color(acid.R, acid.G, acid.B, 0.5f), val.X * 2.2f, 2.4f, 0.4);
				FxKit.Burst(frontParent, val2 + new Vector2(0f, -6f), new Color(acid.R, acid.G, acid.B, 0.95f), 14, 220f, 520f, 0.6, 0.08f, 0.2f, new Vector2(0f, 900f), 40f, new Vector2(0f, -1f), val.X * 0.35f);
			}
			if (Tuning.F("wave.flood_shake", 1f) >= 0.5f)
			{
				GameRefs.Shake(1);
			}
		}

		protected override void OnStopping()
		{
			float num = _floor.Seen + 160f;
			_drainFrom = (_arrived ? num : Mathf.Min(_reach, num));
			if (FxKit.Alive((GodotObject)(object)_bubbles))
			{
				_bubbles.Emitting = false;
			}
		}

		protected override void Layout()
		{
			//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_057b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)Back))
			{
				return;
			}
			if (!_surveyed)
			{
				Look(0f);
			}
			float reach = (_arrived ? 100000f : _reach);
			float num = Mathf.Min(1f, A * 3f);
			float num2 = (_arrived ? 1f : Mathf.Clamp(_reach / Mathf.Max(1f, _floor.Seen), 0f, 1f));
			if (Stopping)
			{
				float num3 = Mathf.Clamp(MathF.Log(1f / Mathf.Max(Alpha, 0.0001f)) / 3.2f, 0f, 1f);
				reach = _drainFrom * Mathf.Pow(1f - num3, 1.5f);
				num = _shown * (1f - Mathf.Pow(num3, 2.5f));
				num2 *= 1f - num3;
			}
			else
			{
				_shown = num;
			}
			if (MultiplayerFx.Active)
			{
				object owner = Anchor.Owner;
				Creature val = (Creature)((owner is Creature) ? owner : null);
				if (val == null || !MultiplayerFx.IsLocal(val))
				{
					goto IL_0163;
				}
			}
			Sfx.Hold("pool", Mathf.Clamp(num, 0f, 1f) * Mathf.Clamp(num2, 0f, 1f));
			goto IL_0163;
			IL_0163:
			if (_flood != null)
			{
				_flood.Alpha = num;
				_flood.Reach = reach;
				_flood.Crest = ((_arrived || Stopping) ? 0f : 1f);
				_flood.Flow = _flow;
				_flood.Kick = Kick;
				_flood.Bubbles = num2;
				_flood.Haze = num2;
				float num4 = Mathf.Max(1f, _floor.Seen + 120f);
				for (int i = 0; i < 4; i++)
				{
					float num5 = _pulseAt[i];
					_flood.PulseAt[i] = num5;
					float num6 = Mathf.Clamp(num5 / num4, 0f, 1f);
					_flood.PulseGain[i] = ((num5 < 0f) ? 0f : (_pulseGain[i] * Mathf.Min(1f, num5 / 160f) * (1f - num6 * num6)));
				}
				_flood.ClearFeet();
				_flood.Stand(_floor.Her, Anchor.SafeBodySize().X * 0.42f, -1f);
				if (_targets != null)
				{
					for (int j = 0; j < _targets.Count; j++)
					{
						FxTarget fxTarget = _targets[j];
						if (fxTarget != null)
						{
							int num7 = _rippleKeys.IndexOf(fxTarget.Key);
							_flood.Stand(fxTarget.Feet, fxTarget.Width * 0.5f, (num7 >= 0) ? _rippleAges[num7] : (-1f));
						}
					}
				}
				_flood.Show(_floor);
			}
			else if (FxKit.Alive((GodotObject)(object)_stain))
			{
				float num8 = _floor.Right - _floor.Left;
				float num9 = _floor.Near - _floor.Far;
				float num10;
				if (Stopping)
				{
					num10 = num2;
				}
				else
				{
					num10 = (_arrived ? 1f : num2);
				}
				float num11 = Mathf.Max(0.05f, num10);
				((Node2D)_stain).GlobalPosition = new Vector2(Mathf.Lerp(_floor.Her.X, (_floor.Left + _floor.Right) * 0.5f, num11), _floor.Far + num9 * 0.55f);
				((Node2D)_stain).Scale = new Vector2(num8 * 1.5f * num11, num9 * 1.5f) / 64f;
				((CanvasItem)_stain).Modulate = new Color(0.05f, 0.26f, 0.08f, Tuning.F("wave.flood_stain", 0.46f) * num);
			}
			if (FxKit.Alive((GodotObject)(object)_bubbles))
			{
				float num12 = FogKit.ScaleOf((Node)(object)Front);
				float num13 = _floor.Far + 24f;
				float num14 = Mathf.Max(num13 + 16f, _floor.View.Y);
				((Node2D)_bubbles).GlobalPosition = new Vector2(_floor.View.X * 0.5f, (num13 + num14) * 0.5f);
				((CanvasItem)_bubbles).Modulate = new Color(1f, 1f, 1f, num * num2 * num2);
				Material processMaterial = _bubbles.ProcessMaterial;
				ParticleProcessMaterial val2 = (ParticleProcessMaterial)(object)((processMaterial is ParticleProcessMaterial) ? processMaterial : null);
				if (val2 != null)
				{
					val2.EmissionBoxExtents = new Vector3(Mathf.Max(40f, _floor.View.X * 0.5f / num12), Mathf.Max(8f, (num14 - num13) * 0.5f / num12), 0f);
				}
			}
		}
	}
