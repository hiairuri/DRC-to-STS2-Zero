// namespace: SilentFX.Cards
// type: SuppressShow (class)
// source lines: 110919-112335
	internal sealed class SuppressShow
	{
		private static readonly List<SuppressShow> Live = new List<SuppressShow>();

		private static bool _ticking;

		public CardModel Card;

		public bool Dead;

		public bool Playing;

		public bool Auto;

		public bool Lifted;

		public bool Aimed;

		public float Presence = 1f;

		public float OwnedAt;

		private bool _dismissed;

		private float _dismissAt;

		private Node2D _back;

		private Node2D _front;

		private ColorRect _fieldBack;

		private ColorRect _fieldFront;

		private ColorRect _soulQuad;

		private ShaderMaterial _fb;

		private ShaderMaterial _ff;

		private ShaderMaterial _sm;

		private ShaderMaterial _fig;

		private Sprite2D _hunter;

		private Vector2 _view;

		private Creature _caster;

		private Vector2 _feet;

		private Vector2 _chest;

		private Vector2 _body = new Vector2(244f, 244f);

		private float _side = 1f;

		private float _flip = 1f;

		public Creature Target;

		private bool _hasTarget;

		private Vector2 _tChest;

		private Vector2 _tFeet;

		private float _tW = 200f;

		private float _tH = 240f;

		public float Time;

		private float _mist;

		private float _veil;

		private float _pull;

		private float _figShow;

		private float _aim;

		private float _plume;

		private float _oldPlume;

		private float _introT = -1f;

		private float _loopT;

		private Vector2 _oldA;

		private Vector2 _oldB;

		private Vector2 _oldC;

		private float _x = -1f;

		private float _playAt;

		private float _handover;

		private bool _hit;

		private bool _killed;

		private bool _cut;

		private bool _reacted;

		private float _cutAt = -1f;

		private float _doneAt = -1f;

		private bool _held;

		private Vector2 _front_;

		private Vector2 _chestFist;

		private Vector2 _head;

		private float _k = 0.4f;

		private static readonly StringName PT = StringName.op_Implicit("t");

		private static readonly StringName PDensity = StringName.op_Implicit("density");

		private static readonly StringName PVeil = StringName.op_Implicit("veil");

		private static readonly StringName PFloor = StringName.op_Implicit("floor_y");

		private static readonly StringName PSink = StringName.op_Implicit("sink");

		private static readonly StringName PPull = StringName.op_Implicit("pull");

		private static readonly StringName PWaveAt = StringName.op_Implicit("wave_at");

		private static readonly StringName PWaveR = StringName.op_Implicit("wave_r");

		private static readonly StringName PWaveK = StringName.op_Implicit("wave_k");

		private static readonly StringName PW = StringName.op_Implicit("w");

		private static readonly StringName PGlow = StringName.op_Implicit("glow");

		private static readonly StringName PTb = StringName.op_Implicit("tb");

		private static readonly StringName PTc = StringName.op_Implicit("tc");

		private static readonly StringName PTd = StringName.op_Implicit("td");

		private static readonly StringName[] PA = new StringName[4]
		{
			StringName.op_Implicit("a0"),
			StringName.op_Implicit("a1"),
			StringName.op_Implicit("a2"),
			StringName.op_Implicit("a3")
		};

		private static readonly StringName[] PB = new StringName[4]
		{
			StringName.op_Implicit("b0"),
			StringName.op_Implicit("b1"),
			StringName.op_Implicit("b2"),
			StringName.op_Implicit("b3")
		};

		private static readonly StringName[] PC = new StringName[4]
		{
			StringName.op_Implicit("c0"),
			StringName.op_Implicit("c1"),
			StringName.op_Implicit("c2"),
			StringName.op_Implicit("c3")
		};

		private static readonly StringName[] PP = new StringName[4]
		{
			StringName.op_Implicit("p0"),
			StringName.op_Implicit("p1"),
			StringName.op_Implicit("p2"),
			StringName.op_Implicit("p3")
		};

		private static readonly Vector2[] Jolt = new Vector2[5]
		{
			new Vector2(0f, 0f),
			new Vector2(11f, -7f),
			new Vector2(-8f, 5f),
			new Vector2(5f, -3f),
			new Vector2(-2f, 1f)
		};

		private Vector2 _sa0;

		private Vector2 _sb0;

		private Vector2 _sc0;

		private float _dtLast;

		private readonly SuppressFrames.Frame[] _slot = new SuppressFrames.Frame[4];

		private readonly float[] _wt = new float[4];

		private float Sc => Mathf.Clamp(_k / 0.38f, 0.5f, 2.2f);

		public static SuppressShow Create(CardFxContext c)
		{
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			if (c == null || !FxKit.Alive((GodotObject)(object)c.BackParent) || !FxKit.Alive((GodotObject)(object)c.FrontParent))
			{
				return null;
			}
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				if (!Live[num].Playing && Live[num].Card == c.Card)
				{
					Live[num].Free();
				}
			}
			SuppressShow suppressShow = new SuppressShow
			{
				Card = c.Card
			};
			suppressShow._view = ((c.View.X > 100f && c.View.Y > 100f) ? c.View : FxKit.ViewSize());
			CardModel card = c.Card;
			object caster;
			if (card == null)
			{
				caster = null;
			}
			else
			{
				Player owner = card.Owner;
				caster = ((owner != null) ? owner.Creature : null);
			}
			suppressShow._caster = (Creature)caster;
			suppressShow._feet = c.Feet;
			suppressShow._chest = c.Chest;
			suppressShow._body = c.Body;
			suppressShow._side = ((c.Facing < 0f) ? (-1f) : 1f);
			suppressShow._flip = suppressShow._side;
			Live.Add(suppressShow);
			if (!_ticking)
			{
				_ticking = true;
				Ticker.Add(Update);
			}
			try
			{
				suppressShow.Build(c);
				suppressShow.Step(0f);
				return suppressShow;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Suppress: could not be built: " + ex);
				suppressShow.Free();
				return null;
			}
		}

		private void Build(CardFxContext c)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected Obj, but got Unknown
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected Obj, but got Unknown
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Expected Obj, but got Unknown
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			SuppressFrames.Warm();
			_back = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_SuppressBack")
			};
			_front = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_SuppressFront")
			};
			TimeFreeze.KeepLive((Node)(object)_back);
			TimeFreeze.KeepLive((Node)(object)_front);
			c.BackParent.AddChild((Node)(object)_back, false, (InternalMode)0);
			c.FrontParent.AddChild((Node)(object)_front, false, (InternalMode)0);
			_back.GlobalTransform = Transform2D.Identity;
			_front.GlobalTransform = Transform2D.Identity;
			FxKit.FreeAfter((Node)(object)_back, 900.0);
			FxKit.FreeAfter((Node)(object)_front, 900.0);
			_fb = SuppressLook.Field();
			if (_fb != null)
			{
				_fieldBack = SuppressLook.Screen((Node)(object)_back, _view, (Material)(object)_fb);
			}
			_fig = SuppressLook.Figure();
			_hunter = new Sprite2D
			{
				Centered = false,
				Material = (Material)(object)_fig,
				Visible = false,
				Name = StringName.op_Implicit("SilentFX_Hunter")
			};
			((Node)_back).AddChild((Node)(object)_hunter, false, (InternalMode)0);
			_ff = SuppressLook.Field();
			if (_ff != null)
			{
				_fieldFront = SuppressLook.Screen((Node)(object)_front, _view, (Material)(object)_ff);
				_ff.SetShaderParameter(StringName.op_Implicit("front"), Variant.op_Implicit(1f));
			}
			_sm = SuppressLook.SoulMist();
			if (_sm != null)
			{
				_soulQuad = SuppressLook.Screen((Node)(object)_front, _view, (Material)(object)_sm);
			}
			ShaderMaterial[] array = new ShaderMaterial[3] { _fb, _ff, _sm };
			foreach (ShaderMaterial obj in array)
			{
				if (obj != null)
				{
					obj.SetShaderParameter(StringName.op_Implicit("size"), Variant.op_Implicit(_view));
				}
			}
		}

		public void SetTarget(CardFxContext c)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			if (Dead)
			{
				return;
			}
			bool flag = c != null && c.HasTarget && c.Target != null;
			if (!flag || !_hasTarget || Target != c.Target)
			{
				if (_hasTarget && _plume > 0.02f && !Playing)
				{
					_oldA = _sa0;
					_oldB = _sb0;
					_oldC = _sc0;
					_oldPlume = _plume;
				}
				_plume = 0f;
				_hasTarget = flag;
				Target = (flag ? c.Target : null);
				if (flag)
				{
					_tChest = c.TargetChest;
					_tFeet = c.TargetFeet;
					_tW = Mathf.Max(60f, c.TargetWidth);
					_tH = Mathf.Max(80f, c.TargetHeight);
					_side = ((c.TargetSide < 0f) ? (-1f) : 1f);
				}
			}
		}

		public void Dismiss()
		{
			if (!Dead && !Playing && !_dismissed)
			{
				_dismissed = true;
				_dismissAt = Time;
			}
		}

		public void Play(CardFxContext c, bool auto)
		{
			if (!Dead)
			{
				if (c != null && c.HasTarget)
				{
					SetTarget(c);
				}
				Playing = true;
				Auto = auto;
				_dismissed = false;
				Lifted = true;
				Aimed = true;
				Presence = 1f;
				_playAt = Time;
				_x = (SuppressFrames.Ok ? ((float)Mathf.Clamp(SuppressFrames.AttackFromAim, 0, SuppressFrames.Attack.Count - 1)) : 0f);
				_handover = ((_figShow > 0.2f && _introT >= 0f) ? 1f : 0f);
				if (_figShow < 0.2f)
				{
					_figShow = 0.15f;
				}
				if (_plume < 0.35f)
				{
					_plume = 0.35f;
				}
			}
		}

		public void Hit(bool killed)
		{
			if (Dead || !Playing)
			{
				return;
			}
			if (_hit)
			{
				if (_cut)
				{
					Echo();
				}
				return;
			}
			_hit = true;
			_killed = killed;
			if (_cut && !_reacted)
			{
				React();
			}
		}

		public static SuppressShow PlayingFor(CardModel card)
		{
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				SuppressShow suppressShow = Live[num];
				if (!suppressShow.Dead && suppressShow.Playing && suppressShow.Card == card && !suppressShow._hit)
				{
					return suppressShow;
				}
			}
			for (int num2 = Live.Count - 1; num2 >= 0; num2--)
			{
				if (!Live[num2].Dead && Live[num2].Playing && Live[num2].Card == card)
				{
					return Live[num2];
				}
			}
			return null;
		}

		public static void Reset()
		{
			SuppressShow[] array = Live.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Free();
			}
			Live.Clear();
		}

		public void Free()
		{
			if (Dead)
			{
				return;
			}
			Dead = true;
			try
			{
				if (_held && Target != null)
				{
					CreatureHold.Release(Target);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Suppress release: " + ex.Message);
			}
			_held = false;
			try
			{
				if (FxKit.Alive((GodotObject)(object)_back))
				{
					((Node)_back).QueueFree();
				}
				if (FxKit.Alive((GodotObject)(object)_front))
				{
					((Node)_front).QueueFree();
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("Suppress cleanup: " + ex2.Message);
			}
		}

		private static void Update(double realDt)
		{
			if (Live.Count == 0)
			{
				return;
			}
			float dt = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				SuppressShow suppressShow = Live[num];
				try
				{
					if (!suppressShow.Dead)
					{
						suppressShow.Step(dt);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Suppress failed and was removed: " + ex);
					suppressShow.Free();
				}
				if (suppressShow.Dead)
				{
					Live.RemoveAt(num);
				}
			}
		}

		private void Step(float dt)
		{
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)_back) || !FxKit.Alive((GodotObject)(object)_front) || !((Node)_back).IsInsideTree() || (Card != null && GameRefs.CombatRoom == null))
			{
				Free();
				return;
			}
			Time += dt;
			Track();
			if (!Playing && !_dismissed && Time - OwnedAt > 1f)
			{
				Dismiss();
			}
			float num = 0f;
			if (_dismissed)
			{
				num = Mathf.Clamp((Time - _dismissAt) / 0.45f, 0f, 1f);
			}
			if (Playing && _doneAt >= 0f)
			{
				num = Mathf.Clamp((Time - _doneAt) / 0.55f, 0f, 1f);
			}
			float num2 = (1f - num) * Mathf.Clamp(Presence, 0f, 1f);
			((CanvasItem)_back).Modulate = new Color(1f, 1f, 1f, num2);
			((CanvasItem)_front).Modulate = new Color(1f, 1f, 1f, num2);
			_dtLast = dt;
			if (Playing)
			{
				Attack(dt);
			}
			else
			{
				Standby(dt);
			}
			DrawFigure();
			DrawFog(dt);
			DrawSoul(dt);
			Shake();
			if (num >= 1f)
			{
				Free();
			}
		}

		private void Track()
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				if (combatRoom == null)
				{
					return;
				}
				if (_caster != null)
				{
					NCreature creatureNode = combatRoom.GetCreatureNode(_caster);
					if (FxKit.Alive((GodotObject)(object)creatureNode) && ((Node)creatureNode).IsInsideTree())
					{
						_feet = GameRefs.BottomOf(creatureNode);
						_chest = creatureNode.VfxSpawnPosition;
						if (!_hasTarget)
						{
							_side = GameRefs.FacingOf(creatureNode);
						}
					}
				}
				if (_hasTarget && Target != null)
				{
					NCreature creatureNode2 = combatRoom.GetCreatureNode(Target);
					if (FxKit.Alive((GodotObject)(object)creatureNode2) && ((Node)creatureNode2).IsInsideTree())
					{
						_tChest = creatureNode2.VfxSpawnPosition;
						_tFeet = GameRefs.BottomOf(creatureNode2);
					}
					else if (Playing && _doneAt < 0f && _cut)
					{
						_doneAt = Time;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Suppress track: " + ex.Message);
			}
		}

		private void Standby(float dt)
		{
			bool flag = Lifted && !_dismissed && SuppressFrames.IntroReady;
			_figShow = Mathf.MoveToward(_figShow, flag ? 1f : 0f, dt / (flag ? 0.12f : 0.25f));
			if (flag)
			{
				if (_introT < 0f)
				{
					_introT = 0f;
				}
				_introT += dt;
				if (_introT * SuppressFrames.Fps >= (float)(SuppressFrames.Intro.Count - 1))
				{
					_loopT += dt;
				}
			}
			else if (_figShow <= 0f)
			{
				_introT = -1f;
				_loopT = 0f;
			}
			bool flag2 = Aimed && _hasTarget && Lifted && !_dismissed;
			_aim = FxKit.Damp(_aim, (flag2 && _figShow > 0.5f) ? 1f : 0f, flag2 ? 6f : 9f, dt);
			_plume = Mathf.MoveToward(_plume, flag2 ? 1f : 0f, dt / (flag2 ? 0.9f : 0.3f));
			_oldPlume = Mathf.MoveToward(_oldPlume, 0f, dt / 0.6f);
			float target = (_dismissed ? 0f : (0.55f + 0.25f * (Lifted ? 1f : 0f) + 0.12f * _aim));
			_mist = FxKit.Damp(_mist, target, 2.2f, dt);
			_veil = FxKit.Damp(_veil, _dismissed ? 0f : (0.12f + 0.1f * _figShow + 0.14f * _aim), 3f, dt);
			_pull = FxKit.Damp(_pull, 0.4f * _aim, 3f, dt);
		}

		private static float Ss(float x)
		{
			x = Mathf.Clamp(x, 0f, 1f);
			return x * x * (3f - 2f * x);
		}

		private void Attack(float dt)
		{
			_dtLast = dt;
			int num = (SuppressFrames.Ok ? SuppressFrames.Attack.Count : 59);
			int num2 = (SuppressFrames.Ok ? SuppressFrames.Cut : 9);
			int num3 = (SuppressFrames.Ok ? SuppressFrames.SlowEnd : 36);
			float fps = SuppressFrames.Fps;
			_figShow = Mathf.MoveToward(_figShow, 1f, dt / 0.1f);
			_handover = Mathf.MoveToward(_handover, 0f, dt / 0.14f);
			_loopT += dt;
			_oldPlume = Mathf.MoveToward(_oldPlume, 0f, dt / 0.4f);
			_plume = 1f;
			if (!_cut)
			{
				float num4 = (Auto ? 0.2f : Mathf.Clamp(Tuning.F("suppress.hit_wait", 0.8f), 0.1f, 2f));
				bool num5 = !_hit && Time - _playAt < num4;
				float num6 = fps * (_hit ? 1.9f : 1.15f);
				float num7 = (num5 ? ((float)num2 - 0.4f) : ((float)num2));
				_x = Mathf.Max(_x, Mathf.Min(_x + num6 * dt, num7));
				if (_x >= (float)num2 - 0.001f)
				{
					_cut = true;
					_cutAt = Time;
					Global();
					if (_hit || Auto)
					{
						React();
					}
				}
				_mist = FxKit.Damp(_mist, 1f, 5f, dt);
				_veil = FxKit.Damp(_veil, 0.46f, 6f, dt);
				_pull = FxKit.Damp(_pull, 0.95f, 7f, dt);
			}
			else
			{
				_x += fps * ((_x < (float)num3) ? 1.45f : 1.2f) * dt;
				if (_x >= (float)(num - 1))
				{
					_x = num - 1;
					if (_doneAt < 0f)
					{
						_doneAt = Time;
					}
				}
				float num8 = CurrentFade();
				_mist = FxKit.Damp(_mist, 0.95f * (1f - Ss(num8 * 1.1f)), 4f, dt);
				_veil = FxKit.Damp(_veil, 0.3f * (1f - Ss(num8)), 4f, dt);
				_pull = FxKit.Damp(_pull, 0f, 3f, dt);
			}
			_aim = 1f;
		}

		private float CurrentFade()
		{
			List<SuppressFrames.Frame> attack = SuppressFrames.Attack;
			if (!SuppressFrames.Ok || attack.Count == 0)
			{
				return Mathf.Clamp((_x - 36f) / 22f, 0f, 1f);
			}
			int num = Mathf.Clamp((int)Mathf.Floor(_x), 0, attack.Count - 1);
			int index = Mathf.Min(num + 1, attack.Count - 1);
			return Mathf.Lerp(attack[num].Fade, attack[index].Fade, _x - Mathf.Floor(_x));
		}

		private void Global()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_front))
			{
				float sc = Sc;
				Vector2 at = (_hasTarget ? _tChest : _front_);
				FxKit.Flash((Node)(object)_front, _front_, new Color(SuppressLook.Pale, 0.75f), 150f * sc, 2.4f);
				FxKit.Flash((Node)(object)_front, Mid(), new Color(SuppressLook.Soul, 0.45f), 260f * sc, 2f, 0.36);
				FxKit.RingPulse((Node)(object)_front, at, 300f * sc, SuppressLook.Pale, 0f, 0.55f, 0.35f, 3.6f, 0.55f, 0f, 0.03f, 0.7f);
				FxKit.RingPulse((Node)(object)_front, at, 300f * sc, SuppressLook.Soul, 0.06f, 0.75f, 0.3f, 5.5f, 0.45f, 9f, 0.02f, 0.45f);
			}
		}

		private Vector2 Mid()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			return (_front_ + (_hasTarget ? Head() : _front_)) * 0.5f;
		}

		private void React()
		{
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if (_reacted)
			{
				return;
			}
			_reacted = true;
			if (!_hasTarget || !FxKit.Alive((GodotObject)(object)_front))
			{
				return;
			}
			float sc = Sc;
			Vector2 val = Head();
			try
			{
				if (!_killed && Target != null && Target.IsAlive)
				{
					CreatureHold.Hold(Target, Mathf.Clamp(Tuning.F("suppress.hold", 0.32f), 0.05f, 1.5f));
					_held = true;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Suppress hold: " + ex.Message);
			}
			FxKit.Flash((Node)(object)_front, val, new Color(SuppressLook.Pale, 0.9f), 120f * sc, 2f, 0.24);
			FxKit.Flash((Node)(object)_front, _tChest, new Color(SuppressLook.Soul, 0.55f), Mathf.Max(_tW, _tH) * 1.3f, 0.3f, 0.42);
			Press(sc);
			FxKit.Burst((Node)(object)_front, _tChest, new Color(SuppressLook.Ash, 0.85f), 28, 40f, 170f, 1.15, 0.22f, 0.7f, new Vector2(0f, 280f), 70f, new Vector2(0f, 1f), _tW * 0.35f, 0.5f, additive: false, 0.8f);
			FxKit.Burst((Node)(object)_front, val, new Color(SuppressLook.Pale, 0.6f), 10, 60f, 200f, 0.6, 0.1f, 0.3f, new Vector2(0f, 120f), 160f, new Vector2(0f, -1f), 20f, 1f, additive: true, 0.9f);
			FxKit.RingPulse((Node)(object)_front, _tFeet, 260f * sc, SuppressLook.Ash, 0.08f, 0.7f, 0.5f, 2.4f, 0.22f, 0f, 0.05f, 0.6f);
		}

		private void Press(float sc)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Expected Obj, but got Unknown
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = Head();
			for (int i = 0; i < 6; i++)
			{
				float num = (FxKit.Hash(i * 13 + 5) - 0.5f) * _tW * 0.9f;
				Vector2 from = val + new Vector2(num, (0f - _tH) * (0.65f + 0.12f * (float)i));
				Vector2 to = val + new Vector2(num * 0.7f, _tH * (0.1f + 0.05f * FxKit.Hash(i + 2)));
				float w = (_tW * 0.55f + 60f * sc) * (0.8f + 0.4f * FxKit.Hash(i * 3 + 1));
				Sprite2D p = new Sprite2D
				{
					Texture = FxKit.SoftDot,
					Modulate = new Color(0.04f, 0.06f, 0.07f, 0f)
				};
				((Node)_front).AddChild((Node)(object)p, false, (InternalMode)0);
				((Node2D)p).GlobalPosition = from;
				float delay = 0.025f * (float)i;
				FxKit.FreeAfter((Node)(object)p, 0.9 + (double)delay);
				FxKit.Drive((Node)(object)p, 0.55f + delay, (float t) =>
				{
					//IL_004a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0052: Unknown result type (might be due to invalid IL or missing references)
					//IL_008a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0094: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
					float num2 = Mathf.Clamp((t * (0.55f + delay) - delay) / 0.55f, 0f, 1f);
					float num3 = num2 * num2 * (3f - 2f * num2);
					((Node2D)p).GlobalPosition = from.Lerp(to, num3 * num3);
					((Node2D)p).Scale = new Vector2(w * (1f + 0.9f * num3), w * (0.75f - 0.45f * num3)) / 64f;
					((CanvasItem)p).Modulate = new Color(0.04f, 0.06f, 0.07f, 0.62f * Mathf.Sin((float)Math.PI * num2));
				}, (TransitionType)0, (EaseType)2);
			}
		}

		private void Echo()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (!_hasTarget || !FxKit.Alive((GodotObject)(object)_front))
			{
				return;
			}
			FxKit.Flash((Node)(object)_front, _tChest, new Color(SuppressLook.Soul, 0.4f), Mathf.Max(_tW, _tH) * 0.8f, 0.4f);
			try
			{
				if (Target != null && Target.IsAlive)
				{
					CreatureHold.Hold(Target, 0.15f);
					_held = true;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Suppress hold: " + ex.Message);
			}
		}

		private Vector2 Head()
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			return new Vector2(_tChest.X, Mathf.Min(_tChest.Y - _tH * 0.2f, _tFeet.Y - _tH * 0.86f));
		}

		private void DrawFigure()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_053c: Unknown result type (might be due to invalid IL or missing references)
			//IL_056b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0570: Unknown result type (might be due to invalid IL or missing references)
			//IL_0575: Unknown result type (might be due to invalid IL or missing references)
			//IL_057c: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_042d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0503: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0513: Unknown result type (might be due to invalid IL or missing references)
			//IL_0518: Unknown result type (might be due to invalid IL or missing references)
			//IL_051f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_0528: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)_hunter))
			{
				return;
			}
			float dtLast = _dtLast;
			Vector2 size = SuppressFrames.Size;
			Vector2 ground = SuppressFrames.Ground;
			float num = Mathf.Clamp(_body.Y * Mathf.Clamp(Tuning.F("suppress.size", 1.75f), 0.6f, 3f), 300f, _view.Y * 0.68f);
			float num2 = num / Mathf.Max(size.Y, 1f);
			float num3 = _feet.Y + 0.03f * num;
			num2 = Mathf.Min(num2, (num3 - _view.Y * 0.02f) / Mathf.Max(ground.Y, 1f));
			_k = Mathf.Max(0.05f, num2);
			float num4 = ((_side == SuppressFrames.Faces) ? 1f : (-1f));
			_flip = ((_figShow < 0.05f) ? num4 : Mathf.MoveToward(_flip, num4, dtLast / 0.18f));
			float flip = ((Mathf.Abs(_flip) < 0.05f) ? (0.05f * (float)Mathf.Sign(num4)) : _flip);
			float num5 = _chest.X - _side * (_body.X * 0.42f + 0.16f * size.X * _k);
			num5 = Mathf.Clamp(num5, _view.X * 0.1f, _view.X * 0.9f);
			Vector2 pos = new Vector2(num5, num3) - new Vector2(ground.X * _k * flip, ground.Y * _k);
			Gather();
			float num6 = 0f;
			Texture2D val = null;
			for (int i = 0; i < 4; i++)
			{
				if (_slot[i]?.Tex == null)
				{
					_wt[i] = 0f;
				}
				num6 += _wt[i];
				if (val == null && _slot[i]?.Tex != null)
				{
					val = _slot[i].Tex;
				}
			}
			Vector2 val2 = Vector2.Zero;
			Vector2 val3 = Vector2.Zero;
			if (num6 > 0.0001f && val != null && _fig != null)
			{
				for (int j = 0; j < 4; j++)
				{
					if (!(_wt[j] <= 0f))
					{
						val2 += _slot[j].Front * (_wt[j] / num6);
						val3 += _slot[j].Chest * (_wt[j] / num6);
					}
				}
				((CanvasItem)_hunter).Visible = _figShow > 0.001f;
				_hunter.Texture = _slot[0]?.Tex ?? val;
				float num7 = ((_slot[0]?.Tex != null) ? (_wt[0] / num6) : 0f);
				_fig.SetShaderParameter(PTb, Variant.op_Implicit((GodotObject)(object)(_slot[1]?.Tex ?? val)));
				_fig.SetShaderParameter(PTc, Variant.op_Implicit((GodotObject)(object)(_slot[2]?.Tex ?? val)));
				_fig.SetShaderParameter(PTd, Variant.op_Implicit((GodotObject)(object)(_slot[3]?.Tex ?? val)));
				_fig.SetShaderParameter(PW, Variant.op_Implicit(new Vector4(num7, _wt[1] / num6, _wt[2] / num6, _wt[3] / num6)));
				float num8 = ((_cutAt >= 0f) ? Mathf.Exp((0f - (Time - _cutAt)) * 6f) : 0f);
				_fig.SetShaderParameter(PGlow, Variant.op_Implicit(num8 + 0.15f * ((Playing && !_cut) ? 1f : 0f)));
				((Node2D)_hunter).GlobalPosition = pos;
				((Node2D)_hunter).Scale = new Vector2(_k * flip, _k);
				((CanvasItem)_hunter).Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(_figShow, 0f, 1f));
				_front_ = Map(val2);
				_chestFist = Map(val3);
			}
			else
			{
				((CanvasItem)_hunter).Visible = false;
				_front_ = _chest + new Vector2(_side * _body.X * 0.25f, (0f - _body.Y) * 0.75f);
				_chestFist = _chest + new Vector2((0f - _side) * _body.X * 0.1f, (0f - _body.Y) * 0.35f);
			}
			Vector2 Map(Vector2 p)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				return pos + new Vector2(p.X * _k * flip, p.Y * _k);
			}
		}

		private int Gather()
		{
			for (int i = 0; i < 4; i++)
			{
				_slot[i] = null;
				_wt[i] = 0f;
			}
			if (!SuppressFrames.Ok)
			{
				return 0;
			}
			float fps = SuppressFrames.Fps;
			List<SuppressFrames.Frame> idle = SuppressFrames.Idle;
			List<SuppressFrames.Frame> aim = SuppressFrames.Aim;
			int count = idle.Count;
			float num = _loopT * fps / 3f;
			float num2 = num - Mathf.Floor(num / (float)count) * (float)count;
			int num3 = Mathf.Clamp((int)Mathf.Floor(num2), 0, count - 1);
			int index = (num3 + 1) % count;
			float num4 = num2 - Mathf.Floor(num2);
			if (Playing && _x >= 0f)
			{
				List<SuppressFrames.Frame> attack = SuppressFrames.Attack;
				int num5 = Mathf.Clamp((int)Mathf.Floor(_x), 0, attack.Count - 1);
				int index2 = Mathf.Min(num5 + 1, attack.Count - 1);
				float num6 = _x - Mathf.Floor(_x);
				float handover = _handover;
				_slot[0] = attack[num5];
				_wt[0] = (1f - num6) * (1f - handover);
				_slot[1] = attack[index2];
				_wt[1] = num6 * (1f - handover);
				_slot[2] = ((_aim > 0.5f) ? aim[num3] : idle[num3]);
				_wt[2] = handover;
				return 3;
			}
			List<SuppressFrames.Frame> intro = SuppressFrames.Intro;
			if (_introT >= 0f && _introT * fps < (float)(intro.Count - 1))
			{
				float num7 = _introT * fps;
				int num8 = Mathf.Clamp((int)Mathf.Floor(num7), 0, intro.Count - 1);
				float num9 = num7 - Mathf.Floor(num7);
				_slot[0] = intro[num8];
				_wt[0] = 1f - num9;
				_slot[1] = intro[Mathf.Min(num8 + 1, intro.Count - 1)];
				_wt[1] = num9;
				return 2;
			}
			float num10 = Mathf.Clamp(_aim, 0f, 1f);
			_slot[0] = idle[num3];
			_wt[0] = (1f - num10) * (1f - num4);
			_slot[1] = idle[index];
			_wt[1] = (1f - num10) * num4;
			_slot[2] = aim[num3];
			_wt[2] = num10 * (1f - num4);
			_slot[3] = aim[index];
			_wt[3] = num10 * num4;
			return 4;
		}

		private void DrawFog(float dt)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			float num = -1f;
			float num2 = 0f;
			Vector2 val = (_hasTarget ? _tChest : _front_);
			if (_cutAt >= 0f)
			{
				float num3 = Time - _cutAt;
				num = 40f + num3 * _view.X * 1.5f;
				num2 = Mathf.Exp((0f - num3) * 2.2f);
			}
			float num4 = _veil + ((_cutAt >= 0f) ? (0.32f * Mathf.Exp((0f - (Time - _cutAt)) * 3.2f)) : 0f);
			if (PlayerSettings.ReduceScreenEffects)
			{
				num4 *= 0.55f;
			}
			(ShaderMaterial, float)[] array = new (ShaderMaterial, float)[2]
			{
				(_fb, 0.95f),
				(_ff, 0.62f)
			};
			for (int i = 0; i < array.Length; i++)
			{
				var (val2, num5) = array[i];
				if (val2 != null)
				{
					val2.SetShaderParameter(PT, Variant.op_Implicit(Time));
					val2.SetShaderParameter(PDensity, Variant.op_Implicit(_mist * num5));
					val2.SetShaderParameter(PVeil, Variant.op_Implicit(num4));
					val2.SetShaderParameter(PFloor, Variant.op_Implicit(_feet.Y));
					val2.SetShaderParameter(PSink, Variant.op_Implicit(_front_));
					val2.SetShaderParameter(PPull, Variant.op_Implicit(_pull));
					val2.SetShaderParameter(PWaveAt, Variant.op_Implicit(val));
					val2.SetShaderParameter(PWaveR, Variant.op_Implicit(num));
					val2.SetShaderParameter(PWaveK, Variant.op_Implicit(num2));
				}
			}
		}

		private void Curve(int i, Vector2 a, Vector2 b, Vector2 c, float width, float density, float u0 = 0f, float u1 = 1f)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			if (_sm != null)
			{
				_sm.SetShaderParameter(PA[i], Variant.op_Implicit(a));
				_sm.SetShaderParameter(PB[i], Variant.op_Implicit(b));
				_sm.SetShaderParameter(PC[i], Variant.op_Implicit(c));
				_sm.SetShaderParameter(PP[i], Variant.op_Implicit(new Vector4(width, Mathf.Max(0f, density), u0, u1)));
			}
		}

		private void DrawSoul(float dt)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0285: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0327: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037d: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_0403: Unknown result type (might be due to invalid IL or missing references)
			//IL_0405: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0451: Unknown result type (might be due to invalid IL or missing references)
			//IL_0456: Unknown result type (might be due to invalid IL or missing references)
			//IL_045c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0461: Unknown result type (might be due to invalid IL or missing references)
			//IL_0466: Unknown result type (might be due to invalid IL or missing references)
			//IL_0478: Unknown result type (might be due to invalid IL or missing references)
			//IL_047d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_0488: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			if (_sm == null)
			{
				return;
			}
			_sm.SetShaderParameter(PT, Variant.op_Implicit(Time));
			float sc = Sc;
			Vector2 front_ = _front_;
			Vector2 chestFist = _chestFist;
			for (int i = 0; i < 4; i++)
			{
				Curve(i, Vector2.Zero, Vector2.Zero, Vector2.Zero, 1f, 0f);
			}
			Curve(1, _oldA, _oldB, _oldC, 24f * sc, 0.7f * _oldPlume);
			if (!_hasTarget)
			{
				Swirl(front_, sc);
				return;
			}
			Vector2 val = Head();
			float num = 22f * sc + _tW * 0.05f;
			if (!Playing || !_cut)
			{
				float num2 = Mathf.Clamp(_tH * 0.9f, 120f, 380f);
				Vector2 val2 = front_ - val;
				float num3 = 0f;
				if (Playing)
				{
					float num4 = (SuppressFrames.Ok ? ((float)SuppressFrames.AttackFromAim) : 3f);
					float num5 = (SuppressFrames.Ok ? ((float)SuppressFrames.Cut) : 9f);
					num3 = Ss((_x - num4) / Mathf.Max(num5 - num4, 0.5f));
				}
				Vector2 val3 = val + new Vector2(0f, _tH * 0.08f);
				Vector2 val4 = val + new Vector2(Mathf.Sin(Time * 1.3f) * 18f * sc, (0f - num2) * 0.55f);
				Vector2 val5 = val + new Vector2(val2.X * 0.18f * _aim + Mathf.Sin(Time * 0.9f + 1f) * 26f * sc, 0f - num2);
				Vector2 val6 = (val + front_) * 0.5f + new Vector2(Mathf.Sin(Time * 7f) * 10f * sc, -0.3f * val2.Length());
				Vector2 val7 = val4.Lerp(val6, num3);
				Vector2 val8 = val5.Lerp(front_, num3);
				float num6 = 0.25f + 0.75f * Ss(_plume);
				_sa0 = val3;
				_sb0 = val7;
				_sc0 = val8;
				Curve(0, val3, val7, val8, num * (1f + 0.4f * num3), (0.75f + 0.35f * num3) * _plume, 0f, Mathf.Lerp(num6, 1f, num3));
				Swirl(front_, sc);
			}
			else
			{
				float num7 = Time - _cutAt;
				float num8 = 1f - Ss(CurrentFade() * 1.15f);
				float num9 = Ss(num7 / 0.9f);
				Vector2 b = val + new Vector2((chestFist.X - val.X) * 0.12f, -120f * sc);
				Vector2 val9 = val + new Vector2(0f, -230f * sc);
				Vector2 c = val9.Lerp(chestFist + new Vector2(0f, -70f * sc), 0.15f + 0.4f * num9);
				Curve(0, val, b, c, num * 1.1f, 0.85f * num8, 0f, 0.92f);
				Vector2 b2 = front_ + new Vector2(0f, 140f * sc);
				val9 = front_ + new Vector2(0f, 170f * sc);
				Vector2 c2 = val9.Lerp(chestFist + new Vector2(0f, 45f * sc), 0.3f + 0.45f * num9);
				Curve(1, front_, b2, c2, num, 0.8f * num8, 0.06f, 0.94f);
				float num10 = 30f * sc;
				float num11 = Mathf.Sin(Time * 2.6f) * 0.4f;
				val9 = new Vector2(0f - num10, (0f - num10) * 0.4f);
				Vector2 a = chestFist + val9.Rotated(num11);
				val9 = new Vector2(num10 * 0.2f, (0f - num10) * 1.9f);
				Vector2 b3 = chestFist + val9.Rotated(num11);
				val9 = new Vector2(num10 * 1.2f, (0f - num10) * 0.2f);
				Curve(2, a, b3, chestFist + val9.Rotated(num11), 17f * sc, 0.95f * num8);
			}
		}

		private void Swirl(Vector2 f, float sc)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			float num = 44f * sc;
			float num2 = Time * 2.4f;
			float density = 0.45f * _aim * ((Playing && _cut) ? 0f : 1f);
			Curve(3, f + D(num2) * num, f + D(num2 + 1.6f) * num * 1.5f, f + D(num2 + 3.1f) * num * 0.55f, 12f * sc, density);
			static Vector2 D(float x)
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				return new Vector2(Mathf.Cos(x), Mathf.Sin(x));
			}
		}

		private void Shake()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = Vector2.Zero;
			if (_cutAt >= 0f)
			{
				int num = (int)((Time - _cutAt) * 24f);
				if (num >= 0 && num < Jolt.Length)
				{
					val = Jolt[num] * (0.6f * Sc);
				}
			}
			if (FxKit.Alive((GodotObject)(object)_back))
			{
				_back.GlobalTransform = new Transform2D(0f, val);
			}
			if (FxKit.Alive((GodotObject)(object)_front))
			{
				_front.GlobalTransform = new Transform2D(0f, val);
			}
		}

		public SuppressShow()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		}

		static SuppressShow()
		{
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		}
	}
