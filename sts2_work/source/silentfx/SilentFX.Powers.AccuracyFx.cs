// namespace: SilentFX.Powers
// type: AccuracyFx (class)
// source lines: 853-1636
	public sealed class AccuracyFx : BasicPowerFx
	{
		private sealed class FanLine
		{
			public Line2D Line;

			public readonly Vector2[] Points = new Vector2[2];

			public object Key;

			public Vector2 End;

			public float Out;

			public bool Seen;
		}

		private static readonly StringName PAlpha = StringName.op_Implicit("alpha");

		private static readonly StringName PSpin = StringName.op_Implicit("spin");

		private readonly List<FxTarget> _targetScratch = new List<FxTarget>();

		private static readonly Color Gold = new Color(1f, 0.85f, 0.42f, 1f);

		private Node2D _scope;

		private ShaderMaterial _mat;

		private const float AimSwing = 18f;

		private Line2D _beam;

		private readonly Vector2[] _beamPoints = new Vector2[2];

		private object _aimCard;

		private Vector2 _aimEnd;

		private bool _aiming;

		private float _aimOut;

		private float _aimFlash;

		private CardModel _remoteCard;

		private CardModel _remotePlayed;

		private Creature _remoteTarget;

		private object _remoteRoom;

		private bool _remoteMode;

		private CardModel _remoteFanCard;

		private bool _remoteFanAiming;

		private readonly List<FanLine> _fanLines = new List<FanLine>();

		private object _fanCard;

		private bool _fanAiming;

		private float _fanFlash;

		private Vector2 _fanFrom;

		private const float ThrowAt = 0.22f;

		private const float FlyTime = 0.08f;

		private const float HomeAt = 0.52f;

		private const float HomeTime = 0.3f;

		private const float KnifeLength = 46f;

		private bool _opening;

		private bool _struck;

		private float _openAt;

		private Node2D _knife;

		private float _tight;

		private bool RemoteOwner
		{
			get
			{
				if (MultiplayerFx.Active)
				{
					object obj = Anchor?.Owner;
					Creature val = (Creature)((obj is Creature) ? obj : null);
					if (val != null)
					{
						return !MultiplayerFx.IsLocal(val);
					}
				}
				return false;
			}
		}

		protected override string NodeName => "Accuracy";

		protected override bool UseBack => false;

		protected override float KickDecay => 5f;

		private float Since => Time - _openAt;

		protected override void Build(bool animateIn)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			_scope = FxKit.Sigil(Tuning.F("accuracy.scope", 74f), Gold, out _mat);
			((Node)Front).AddChild((Node)(object)_scope, false, (InternalMode)0);
			_beam = MakeBeam();
			((CanvasItem)_beam).Visible = false;
			((Node)Front).AddChild((Node)(object)_beam, false, (InternalMode)0);
			HeldFxManager.LetGo += OnLetGo;
			if (_mat != null)
			{
				_mat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.74f));
				_mat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.08f));
				_mat.SetShaderParameter(StringName.op_Implicit("ticks"), Variant.op_Implicit(4f));
				_mat.SetShaderParameter(StringName.op_Implicit("tick_len"), Variant.op_Implicit(0.34f));
				_mat.SetShaderParameter(StringName.op_Implicit("tick_width"), Variant.op_Implicit(0.07f));
				_mat.SetShaderParameter(StringName.op_Implicit("tick_out"), Variant.op_Implicit(0.5f));
				_mat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.3f));
				_mat.SetShaderParameter(PAlpha, Variant.op_Implicit(0f));
			}
			if (animateIn && Tuning.F("intro.on", 1f) > 0.5f)
			{
				Open();
			}
		}

		private Vector2 Out()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = Anchor.SafeBodySize();
			return Hand() + new Vector2(val.X * 0.85f * Anchor.Side(), (0f - val.Y) * 0.16f);
		}

		private void Open()
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			_opening = true;
			_struck = false;
			_openAt = Time;
			if (!FxKit.Alive((GodotObject)(object)_knife))
			{
				_knife = FxKit.Blade(46f, 6f, new Color(0.94f, 0.97f, 1f, 1f), new Color(0.6f, 0.67f, 0.78f, 1f), new Color(0.3f, 0.36f, 0.5f, 1f), new Color(Gold.R, Gold.G, Gold.B, 0.3f));
				((CanvasItem)_knife).Visible = false;
				((Node)Front).AddChild((Node)(object)_knife, false, (InternalMode)0);
			}
		}

		protected override void OnStacked()
		{
			if (Tuning.F("intro.on", 1f) > 0.5f)
			{
				Open();
			}
		}

		private float Away()
		{
			if (!_opening)
			{
				return 0f;
			}
			if (Since < 0.52f)
			{
				return 1f;
			}
			float num = 1f - Mathf.Clamp((Since - 0.52f) / 0.3f, 0f, 1f);
			return num * num * (3f - 2f * num);
		}

		private void StepOpening()
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			if (!_opening)
			{
				return;
			}
			if (Since >= 0.82f)
			{
				_opening = false;
				if (FxKit.Alive((GodotObject)(object)_knife))
				{
					((Node)_knife).QueueFree();
				}
				_knife = null;
			}
			else
			{
				if (!FxKit.Alive((GodotObject)(object)_knife) || Since < 0.22f)
				{
					return;
				}
				Vector2 val = Hand();
				Vector2 val2 = val.Lerp(Out(), Away());
				Vector2 val3 = Out() - val;
				Vector2 val4 = (Vector2)((val3.LengthSquared() > 1f) ? val3.Normalized() : new Vector2(Anchor.Side(), 0f));
				float num = Mathf.Clamp((Since - 0.22f) / 0.08f, 0f, 1f);
				Vector2 val5 = val2 - val4 * 46f;
				((CanvasItem)_knife).Visible = true;
				_knife.GlobalPosition = val.Lerp(val5, num);
				float num2 = ((num < 1f) ? 0f : (0.1f * Mathf.Sin((Since - 0.22f - 0.08f) * 60f) * Mathf.Exp((0f - (Since - 0.22f - 0.08f)) * 9f)));
				_knife.Rotation = val4.Angle() + (float)Math.PI / 2f + num2 * Anchor.Side();
				((CanvasItem)_knife).Modulate = new Color(1f, 1f, 1f, A * Mathf.Min(1f, Away() * 1.6f));
				if (!(num < 1f) && !_struck)
				{
					_struck = true;
					Kick = 1f;
					Node frontParent = Anchor.FrontParent;
					if (FxKit.Alive((GodotObject)(object)frontParent))
					{
						FxKit.Flash(frontParent, val2, new Color(1f, 0.92f, 0.62f, 0.9f), 70f, 2.2f, 0.22);
						FxKit.Burst(frontParent, val2, new Color(Gold.R, Gold.G, Gold.B, 0.95f), 10, 120f, 340f, 0.3, 0.05f, 0.13f, new Vector2(0f, 420f), 70f, -val4, 4f);
						FxKit.RingPulse(frontParent, val2, 150f, Gold, 0f, 0.26f, 0.45f, 1.15f, 1f, 0f, 0.035f);
					}
				}
			}
		}

		private Vector2 Hand()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = Anchor.SafeBodySize();
			return Anchor.Chest() + new Vector2(val.X * Tuning.F("accuracy.hand_x", 0.24f) * Anchor.Side(), val.Y * Tuning.F("accuracy.hand_y", 0.1f));
		}

		private static Line2D MakeBeam()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected Obj, but got Unknown
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Expected Obj, but got Unknown
			Gradient val = new Gradient();
			val.Offsets = new float[2] { 0f, 1f };
			val.Colors = new Color[2]
			{
				new Color(Gold.R, Gold.G, Gold.B, 0.34f),
				new Color(Gold.R, Gold.G, Gold.B, 0f)
			};
			return new Line2D
			{
				Width = 2.5f,
				Gradient = val,
				Material = (Material)(object)FxKit.Additive,
				Antialiased = true
			};
		}

		private bool HoldingShiv()
		{
			try
			{
				CardModel val = (RemoteOwner ? _remoteCard : HeldFxManager.Carried);
				if (!(val is Shiv))
				{
					return false;
				}
				int result;
				if (Anchor.Owner != null)
				{
					Player owner = val.Owner;
					result = ((((owner != null) ? owner.Creature : null) == Anchor.Owner) ? 1 : 0);
				}
				else
				{
					result = 1;
				}
				return (byte)result != 0;
			}
			catch
			{
				return false;
			}
		}

		private bool Aimed(out Vector2 chest)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			chest = default;
			Creature val = (RemoteOwner ? _remoteTarget : HeldFxManager.PointedAt);
			if (val == null)
			{
				return false;
			}
			return FindTarget(val, out chest);
		}

		private bool FindTarget(Creature enemy, out Vector2 chest)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			chest = default;
			if (enemy == null)
			{
				return false;
			}
			Anchor.SafeTargets(_targetScratch);
			foreach (FxTarget item in _targetScratch)
			{
				if (item.Key == enemy)
				{
					chest = item.Chest;
					return true;
				}
			}
			return false;
		}

		private void OnLetGo(CardModel card, bool played)
		{
			if (Dead)
			{
				HeldFxManager.LetGo -= OnLetGo;
				return;
			}
			if (card != null && card == _fanCard)
			{
				_fanCard = null;
				if (played && !Stopping && AnyFanLineOut())
				{
					_fanFlash = 1f;
				}
			}
			if (card != null && card == _aimCard)
			{
				_aimCard = null;
				if (played && !Stopping && !(_aimOut <= 0f))
				{
					_aimFlash = 1f;
				}
			}
		}

		protected override void OnDispose()
		{
			HeldFxManager.LetGo -= OnLetGo;
			ClearRemote();
		}

		private void ClearRemote()
		{
			_remoteCard = (_remotePlayed = null);
			_remoteTarget = null;
			_remoteFanCard = null;
			_remoteFanAiming = false;
			_remoteRoom = null;
			_remoteMode = false;
			_aimCard = null;
			_aiming = false;
			_aimOut = (_aimFlash = (_tight = 0f));
			_fanCard = null;
			_fanAiming = false;
			_fanFlash = 0f;
			foreach (FanLine fanLine in _fanLines)
			{
				if (FxKit.Alive((GodotObject)(object)fanLine.Line))
				{
					((Node)fanLine.Line).QueueFree();
				}
			}
			_fanLines.Clear();
		}

		private void EnterRemote()
		{
			if (!_remoteMode || _remoteRoom != GameRefs.CombatRoom)
			{
				ClearRemote();
			}
			_remoteMode = true;
			_remoteRoom = GameRefs.CombatRoom;
		}

		private void ReadRemote()
		{
			EnterRemote();
			object owner = Anchor.Owner;
			if (!RemoteHeldFx.TryObserve((Creature)((owner is Creature) ? owner : null), out var card, out var target, out var released, out var inPlayZone))
			{
				_remoteCard = null;
				_remoteTarget = null;
				_remoteFanCard = null;
				_remoteFanAiming = false;
				if (released && (card == null || card == _remotePlayed))
				{
					_remotePlayed = null;
				}
			}
			else
			{
				if (card != _remotePlayed)
				{
					_remotePlayed = null;
				}
				_remoteCard = ((card == _remotePlayed) ? null : card);
				_remoteTarget = ((_remoteCard != null) ? target : null);
				_remoteFanCard = ((_remoteCard is Shiv && ShivFan.Active(_remoteCard)) ? _remoteCard : null);
				_remoteFanAiming = (_remoteFanCard != null) & inPlayZone;
			}
		}

		internal void OnRemoteCardPlayed(CardModel card, Creature target)
		{
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			if (!RemoteOwner || Dead || Stopping || !(card is Shiv))
			{
				return;
			}
			Player owner = card.Owner;
			if (((owner != null) ? owner.Creature : null) != Anchor.Owner)
			{
				return;
			}
			EnterRemote();
			if (_remotePlayed == card)
			{
				return;
			}
			_remotePlayed = card;
			if (_remoteCard == card)
			{
				_remoteCard = null;
				_remoteTarget = null;
			}
			if (_remoteFanCard == card)
			{
				_remoteFanCard = null;
				_remoteFanAiming = false;
				if (!Stopping && AnyFanLineOut())
				{
					_fanFlash = 1f;
				}
			}
			if (_aimCard == card)
			{
				_aimCard = null;
				_aiming = false;
				if (!(_aimOut <= 0f) && FindTarget(target, out var chest))
				{
					_aimEnd = chest;
					_aimFlash = 1f;
				}
			}
		}

		protected override void Tick(float dt)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			if (RemoteOwner)
			{
				ReadRemote();
			}
			else if (_remoteMode)
			{
				ClearRemote();
			}
			bool flag = !Stopping && HoldingShiv();
			_tight = FxKit.Damp(_tight, flag ? 1f : 0f, Tuning.F("accuracy.tight_rate", 9f), dt);
			StepOpening();
			Vector2 chest = default;
			_aiming = flag && Aimed(out chest);
			if (_aiming)
			{
				_aimCard = (RemoteOwner ? _remoteCard : HeldFxManager.Carried);
				_aimEnd = ((_aimOut > 0f) ? _aimEnd.Lerp(chest, 1f - Mathf.Exp(-18f * dt)) : chest);
			}
			float num = dt / Mathf.Max(0.01f, Tuning.F("accuracy.line_in", 0.1f));
			if (_aiming)
			{
				_aimOut = Mathf.Min(1f, _aimOut + num);
			}
			else if (_aimFlash <= 0f)
			{
				_aimOut = Mathf.Max(0f, _aimOut - num);
			}
			if (_aimFlash > 0f)
			{
				_aimFlash = Mathf.Max(0f, _aimFlash - dt / Mathf.Max(0.01f, Tuning.F("accuracy.line_flash", 0.2f)));
				if (_aimFlash <= 0f && !_aiming)
				{
					_aimOut = 0f;
				}
			}
			_fanAiming = flag && (RemoteOwner ? _remoteFanAiming : HeldFxManager.ArmedAtAll);
			if (_fanAiming)
			{
				_fanCard = (RemoteOwner ? _remoteFanCard : HeldFxManager.Carried);
			}
			TickFanLines(dt, num);
		}

		private bool AnyFanLineOut()
		{
			foreach (FanLine fanLine in _fanLines)
			{
				if (fanLine.Out > 0f)
				{
					return true;
				}
			}
			return false;
		}

		private void TickFanLines(float dt, float step)
		{
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			if (_fanFlash > 0f)
			{
				_fanFlash = Mathf.Max(0f, _fanFlash - dt / Mathf.Max(0.01f, Tuning.F("accuracy.line_flash", 0.2f)));
				if (_fanFlash <= 0f && !_fanAiming)
				{
					foreach (FanLine fanLine3 in _fanLines)
					{
						fanLine3.Out = 0f;
					}
				}
			}
			if (!_fanAiming && _fanLines.Count == 0)
			{
				return;
			}
			if (ShivFan.TryHeldHilt(RemoteOwner ? _remoteFanCard : HeldFxManager.Carried, out var at))
			{
				_fanFrom = at;
			}
			else if (_fanAiming)
			{
				Vector2 val = Hand();
				_fanFrom = val.Lerp(Out(), Away());
			}
			foreach (FanLine fanLine4 in _fanLines)
			{
				fanLine4.Seen = false;
			}
			if (_fanAiming)
			{
				Anchor.SafeTargets(_targetScratch);
				foreach (FxTarget item in _targetScratch)
				{
					FanLine fanLine = null;
					foreach (FanLine fanLine5 in _fanLines)
					{
						if (fanLine5.Key == item.Key)
						{
							fanLine = fanLine5;
							break;
						}
					}
					if (fanLine == null)
					{
						if (!FxKit.Alive((GodotObject)(object)Front))
						{
							break;
						}
						fanLine = new FanLine
						{
							Line = MakeBeam(),
							Key = item.Key
						};
						fanLine.Line.Width = Tuning.F("accuracy.fan_line_width", 1.6f);
						((CanvasItem)fanLine.Line).Visible = false;
						((Node)Front).AddChild((Node)(object)fanLine.Line, false, (InternalMode)0);
						_fanLines.Add(fanLine);
					}
					fanLine.End = item.Chest;
					fanLine.Seen = true;
				}
			}
			for (int num = _fanLines.Count - 1; num >= 0; num--)
			{
				FanLine fanLine2 = _fanLines[num];
				if (fanLine2.Seen)
				{
					fanLine2.Out = Mathf.Min(1f, fanLine2.Out + step);
				}
				else if (_fanFlash <= 0f)
				{
					fanLine2.Out = Mathf.Max(0f, fanLine2.Out - step);
				}
				if (!(fanLine2.Out > 0f) && !fanLine2.Seen)
				{
					if (FxKit.Alive((GodotObject)(object)fanLine2.Line))
					{
						((Node)fanLine2.Line).QueueFree();
					}
					_fanLines.RemoveAt(num);
				}
			}
		}

		private void LayoutFanLines()
		{
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			if (_fanLines.Count == 0)
			{
				return;
			}
			float num = ((_fanFlash > 0f && !_fanAiming) ? _fanFlash : 1f);
			float num2 = Mathf.Clamp(Tuning.F("accuracy.line_alpha", 1f) * A * (num + 2f * _fanFlash * _fanFlash), 0f, 3f);
			float num3 = Tuning.F("accuracy.reach", 0.85f);
			foreach (FanLine fanLine in _fanLines)
			{
				if (FxKit.Alive((GodotObject)(object)fanLine.Line))
				{
					if (fanLine.Out <= 0.001f)
					{
						((CanvasItem)fanLine.Line).Visible = false;
						continue;
					}
					float num4 = 1f - (1f - fanLine.Out) * (1f - fanLine.Out);
					fanLine.Points[0] = Front.ToLocal(_fanFrom);
					fanLine.Points[1] = Front.ToLocal(_fanFrom.Lerp(fanLine.End, num3 * num4));
					fanLine.Line.Points = fanLine.Points;
					((CanvasItem)fanLine.Line).Modulate = new Color(1f, 1f, 1f, num2);
					((CanvasItem)fanLine.Line).Visible = true;
				}
			}
		}

		protected override void Layout()
		{
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val3;
			if (FxKit.Alive((GodotObject)(object)_scope))
			{
				float num = Away();
				float num2 = (1f + 0.3f * Kick) * (1f + 1.1f * num) * Tuning.F("accuracy.upright", 1.3f) * Mathf.Lerp(1f, Mathf.Clamp(Tuning.F("accuracy.tight", 0.6f), 0.2f, 1f), _tight);
				float num3 = Mathf.Clamp(Tuning.F("accuracy.narrow", 0.34f), 0.05f, 1f);
				float num4 = Mathf.Tan(Mathf.DegToRad(Mathf.Clamp(Tuning.F("accuracy.slope", 10f), -30f, 30f)));
				Vector2 val = new Vector2(num3, (0f - num3) * num4 * Anchor.Side()) * num2;
				Vector2 val2 = new Vector2(0f, num2);
				Node2D scope = _scope;
				val3 = Hand();
				scope.GlobalTransform = new Transform2D(val, val2, val3.Lerp(Out(), num));
				if (FxKit.Alive((GodotObject)(object)_mat))
				{
					_mat.SetShaderParameter(PAlpha, Variant.op_Implicit(Mathf.Min(1.6f, Tuning.F("accuracy.alpha", 0.75f) * A * (1f + 0.8f * Kick) * (1f + 0.45f * _tight))));
					_mat.SetShaderParameter(PSpin, Variant.op_Implicit(Time * 0.8f));
				}
			}
			LayoutFanLines();
			if (FxKit.Alive((GodotObject)(object)_beam))
			{
				if (_aimOut <= 0.001f)
				{
					((CanvasItem)_beam).Visible = false;
					return;
				}
				float num5 = 1f - (1f - _aimOut) * (1f - _aimOut);
				val3 = Hand();
				Vector2 val4 = val3.Lerp(Out(), Away());
				_beamPoints[0] = Front.ToLocal(val4);
				_beamPoints[1] = Front.ToLocal(val4.Lerp(_aimEnd, Tuning.F("accuracy.reach", 0.85f) * num5));
				_beam.Points = _beamPoints;
				float num6 = ((_aimFlash > 0f && !_aiming) ? _aimFlash : 1f);
				float num7 = Mathf.Clamp(Tuning.F("accuracy.line_alpha", 1f) * A * (num6 + 2f * _aimFlash * _aimFlash), 0f, 3f);
				((CanvasItem)_beam).Modulate = new Color(1f, 1f, 1f, num7);
				((CanvasItem)_beam).Visible = true;
			}
		}

		static AccuracyFx()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		}
	}
