// namespace: SilentFX.Cards
// type: FlechettesOrbit (class)
// source lines: 58918-60067
	public sealed class FlechettesOrbit : HeldSession
	{
		private sealed class Shaker
		{
			public Sprite2D Glow;

			public NCard Host;

			public float Phase;

			public float Amp;

			public float Level;

			public int Keep;
		}

		private enum Leg
		{
			Out,
			Orbit,
			Strike,
			Home,
			Done
		}

		private sealed class Slot
		{
			public CardModel Card;

			public NCard Copy;

			public NCard Dimmed;

			public Leg Now;

			public int Index;

			public float T;

			public float Span;

			public Vector2 Pos;

			public float Rot;

			public Vector2 Scale = Vector2.One;

			public Vector2 FromPos;

			public float FromRot;

			public Vector2 FromScale = Vector2.One;

			public Vector2 Spot;

			public float Shade = 1f;

			public float Alpha = 1f;

			public bool Behind;

			public Slot()
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			}
		}

		private const string DecoKey = "flech";

		private static readonly Color White = new Color(1f, 1f, 1f, 0.9f);

		private static readonly Color PaleSteel = new Color(0.84f, 0.92f, 1f, 1f);

		private static readonly Dictionary<NCard, (float Base, CardModel Model)> Tilted = new Dictionary<NCard, (float, CardModel)>(ReferenceEqualityComparer.Instance);

		private static readonly List<NCard> Scratch = new List<NCard>();

		private static int _shakers;

		private readonly List<Slot> _slots = new List<Slot>();

		private readonly List<HandCards.Entry> _hand = new List<HandCards.Entry>();

		private bool _orbiting;

		private bool _shaking;

		private int _count;

		private int _shots;

		private float _waitingForHit;

		private float _spin;

		private int _beat = int.MinValue;

		private float _rx;

		private float _ry;

		private float _spinRate;

		private float _spinPlay;

		private float _cardScale;

		private float _dim;

		private float _outTime;

		private float _stagger;

		private float _homeTime;

		private float _quickHome;

		private float _shotTime;

		private float _backTime;

		private bool _layered;

		private Node2D _mark;

		private readonly Line2D[] _corners = new Line2D[4];

		private readonly Vector2[] _cornerPoints = new Vector2[3];

		private bool _hasMark;

		private bool _markInward;

		private Vector2 _markAt;

		private float _markPop = 1f;

		public static void Reset()
		{
			try
			{
				StraightenAll();
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlechettesOrbit.Reset: " + ex.Message);
			}
			try
			{
				CardDeco.RemoveAll("flech", 0f);
			}
			catch (Exception ex2)
			{
				DevLog.Warn("FlechettesOrbit.Reset decos: " + ex2.Message);
			}
		}

		private static void Tilt(NCard host, CardModel card, float offset)
		{
			if (FxKit.Alive((GodotObject)(object)host))
			{
				if (!Tilted.TryGetValue(host, out (float, CardModel) value) || value.Item2 != card)
				{
					value = (((Control)host).Rotation, card);
					Tilted[host] = value;
				}
				((Control)host).Rotation = value.Item1 + offset;
			}
		}

		private static void Straighten(NCard host)
		{
			if (host == null || !Tilted.Remove(host, out (float, CardModel) value))
			{
				return;
			}
			try
			{
				if (FxKit.Alive((GodotObject)(object)host) && host.Model == value.Item2)
				{
					((Control)host).Rotation = value.Item1;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlechettesOrbit (straighten): " + ex.Message);
			}
		}

		private static void StraightenAll()
		{
			Scratch.Clear();
			foreach (KeyValuePair<NCard, (float, CardModel)> item in Tilted)
			{
				Scratch.Add(item.Key);
			}
			foreach (NCard item2 in Scratch)
			{
				Straighten(item2);
			}
			Scratch.Clear();
			Tilted.Clear();
		}

		private static void StopShakes()
		{
			try
			{
				StraightenAll();
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlechettesOrbit (stop shake): " + ex.Message);
			}
			try
			{
				CardDeco.RemoveAll("flech", 0.1f);
			}
			catch (Exception ex2)
			{
				DevLog.Warn("FlechettesOrbit (remove deco): " + ex2.Message);
			}
		}

		private static void BuildShake(CardDeco.Handle h)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			Sprite2D val = FxKit.Glow(new Color(PaleSteel.R, PaleSteel.G, PaleSteel.B, 0f), new Vector2(430f, 560f));
			((Node)val).Name = StringName.op_Implicit("SilentFX_FlechGlow");
			((Node)h.Root).AddChild((Node)(object)val, false, (InternalMode)0);
			_shakers++;
			h.State = new Shaker
			{
				Glow = val,
				Phase = FxKit.Hash(_shakers) * ((float)Math.PI * 2f),
				Amp = Mathf.Clamp(Tuning.F("flech.shake", 0.035f), 0f, 0.12f)
			};
		}

		private static void ShakeTick(CardDeco.Handle h, float dt)
		{
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			if (h == null || !(h.State is Shaker shaker))
			{
				return;
			}
			NCard host = h.Host;
			try
			{
				bool flag = shaker.Keep > 0;
				if (shaker.Keep > 0)
				{
					shaker.Keep--;
				}
				shaker.Level = FxKit.Damp(shaker.Level, flag ? 1f : 0f, flag ? 16f : 22f, dt);
				if (shaker.Host != host)
				{
					Straighten(shaker.Host);
					shaker.Host = host;
				}
				if (!flag && shaker.Level < 0.04f)
				{
					Straighten(host);
					shaker.Host = null;
					h.Remove(0.1f);
					return;
				}
				if (FxKit.Alive((GodotObject)(object)shaker.Glow))
				{
					((CanvasItem)shaker.Glow).Modulate = new Color(PaleSteel.R, PaleSteel.G, PaleSteel.B, shaker.Level * (0.2f + 0.05f * Mathf.Sin(h.Time * 9f + shaker.Phase)));
				}
				if (FxKit.Alive((GodotObject)(object)host))
				{
					Tilt(host, h.Card, shaker.Amp * shaker.Level * Mathf.Sin(h.Time * 38f + shaker.Phase));
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlechettesOrbit (shake): " + ex.Message);
				Straighten(shaker.Host);
				Straighten(host);
				shaker.Host = null;
				h.Remove(0f);
			}
		}

		private static void ShakeLeft(CardDeco.Handle h)
		{
			if (h != null)
			{
				if (h.State is Shaker shaker)
				{
					Straighten(shaker.Host);
					shaker.Host = null;
				}
				Straighten(h.Host);
			}
		}

		protected override void Build()
		{
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Expected Obj, but got Unknown
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			_rx = Mathf.Max(70f, C.Body.X * Tuning.F("flech.orbit_x", 0.95f));
			_ry = Mathf.Max(26f, C.Body.Y * Tuning.F("flech.orbit_y", 0.32f));
			_spinRate = Tuning.F("flech.spin", 3.2f);
			_spinPlay = Tuning.F("flech.spin_play", 1.35f);
			_cardScale = Mathf.Clamp(Tuning.F("flech.card_scale", 0.33f), 0.1f, 1f);
			_dim = Mathf.Clamp(Tuning.F("flech.dim", 0.42f), 0f, 1f);
			_outTime = Mathf.Max(0.05f, Tuning.F("flech.out", 0.22f));
			_stagger = Mathf.Max(0f, Tuning.F("flech.stagger", 0.03f));
			_homeTime = Mathf.Max(0.05f, Tuning.F("flech.home", 0.18f));
			_quickHome = Mathf.Max(0.05f, Tuning.F("flech.home_quick", 0.12f));
			_shotTime = Mathf.Max(0.03f, Tuning.F("flech.shot", 0.09f));
			_backTime = Mathf.Max(0.05f, Tuning.F("flech.back", 0.16f));
			_layered = Tuning.F("flech.layers", 1f) > 0.5f;
			_markInward = Tuning.F("flech.mark_inward", 1f) > 0.5f;
			if (!C.Remote)
			{
				_mark = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_FlechMark"),
					Visible = false
				};
				((Node)Front).AddChild((Node)(object)_mark, false, (InternalMode)0);
				for (int i = 0; i < _corners.Length; i++)
				{
					_corners[i] = Props.Line(3f, White, true);
					((Node)_mark).AddChild((Node)(object)_corners[i], false, (InternalMode)0);
				}
			}
		}

		private bool IsDart(CardModel card)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Invalid comparison between Unknown and I4
			if (card != null && card != C.Card)
			{
				return (int)card.Type == 2;
			}
			return false;
		}

		private NCard RealOf(CardModel card)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Invalid comparison between Unknown and I4
			if (C.Remote)
			{
				return null;
			}
			try
			{
				if (card != null)
				{
					CardPile pile = card.Pile;
					if (pile != null && (int)pile.Type == 2)
					{
						NCard val = HandCards.NodeOf(card);
						return (FxKit.Alive((GodotObject)(object)val) && ((Node)val).IsInsideTree() && val.Model == card) ? val : null;
					}
				}
				return null;
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlechettesOrbit.RealOf: " + ex.Message);
				return null;
			}
		}

		protected override void Tick(float dt)
		{
			try
			{
				if (Playing && !Leaving)
				{
					_waitingForHit += dt;
					if (_waitingForHit > 3f)
					{
						SendHome(_quickHome);
						Leave();
					}
				}
				if (!Hovering && !Leaving && !_orbiting)
				{
					LiftOff();
				}
				else if (Hovering && _orbiting)
				{
					SendHome(_homeTime);
				}
				if (!C.Remote && _orbiting && _slots.Count > 0 && !Playing && !Leaving)
				{
					int num = Mathf.Max(1, _count);
					float num2 = ((num > 10) ? ((float)num * 0.5f) : ((float)num));
					float num3 = Sfx.Lead("flechette_orbit");
					int num4 = Mathf.FloorToInt((_spin + _spinRate * num3 - (float)Math.PI / 2f) * num2 / ((float)Math.PI * 2f));
					if (_beat != int.MinValue && num4 != _beat)
					{
						Sfx.One("flechette_orbit", num3, Mathf.Clamp(Lift, 0f, 1f));
					}
					_beat = num4;
				}
				else
				{
					_beat = int.MinValue;
				}
				if (_orbiting || _slots.Count > 0)
				{
					_spin += _spinRate * (Playing ? _spinPlay : 1f) * dt;
					bool flag = false;
					for (int i = 0; i < _slots.Count; i++)
					{
						Slot slot = _slots[i];
						Advance(slot, dt);
						if (slot.Now != Leg.Done)
						{
							flag = true;
							Put(slot);
						}
					}
					if (!flag && !_orbiting)
					{
						_slots.Clear();
					}
				}
				else if (Hovering && !Leaving)
				{
					ShakeHand();
				}
				TickMark(dt);
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlechettesOrbit: " + ex);
				End();
			}
		}

		private void ShakeHand()
		{
			if (C.Remote)
			{
				return;
			}
			_shaking = true;
			HandCards.Fill(_hand);
			foreach (HandCards.Entry item in _hand)
			{
				if (IsDart(item.Card))
				{
					CardDeco.Handle handle = CardDeco.Attach(item.Card, "flech", BuildShake, ShakeTick, ShakeLeft);
					if (handle != null && handle.Alive && handle.State is Shaker shaker)
					{
						shaker.Keep = 3;
					}
				}
			}
			_hand.Clear();
		}

		private void LiftOff()
		{
			ClearSlots();
			StopShaking();
			_orbiting = true;
			if (C.Remote)
			{
				int num = 0;
				foreach (CardModel card in PileTypeExtensions.GetPile((PileType)2, C.Card.Owner).Cards)
				{
					if (IsDart(card))
					{
						_slots.Add(new Slot
						{
							Card = card,
							Index = num,
							Now = Leg.Out,
							T = (0f - _stagger) * (float)num,
							Span = _outTime
						});
						num++;
					}
				}
				_count = num;
				return;
			}
			HandCards.Fill(_hand);
			int num2 = 0;
			foreach (HandCards.Entry item in _hand)
			{
				if (IsDart(item.Card))
				{
					_slots.Add(new Slot
					{
						Card = item.Card,
						Index = num2,
						Now = Leg.Out,
						T = (0f - _stagger) * (float)num2,
						Span = _outTime
					});
					num2++;
				}
			}
			_hand.Clear();
			_count = num2;
		}

		private bool Spawn(Slot s, NCard real)
		{
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)s.Copy))
			{
				return true;
			}
			if ((!C.Remote && real == null) || !FxKit.Alive((GodotObject)(object)Front))
			{
				s.Now = Leg.Done;
				return false;
			}
			NCard val = CardStandIn.Make(s.Card, (Node)(object)Front);
			if (!FxKit.Alive((GodotObject)(object)val))
			{
				s.Now = Leg.Done;
				return false;
			}
			s.Copy = val;
			s.Behind = false;
			if (C.Remote)
			{
				OrbitPose(s, out s.Pos, out s.Rot, out s.Scale, out var _);
			}
			else
			{
				CardStandIn.Pose(real, out s.Pos, out s.Rot, out s.Scale);
			}
			s.FromPos = s.Pos;
			s.FromRot = s.Rot;
			s.FromScale = s.Scale;
			s.Shade = 1f;
			s.Alpha = 1f;
			Put(s);
			if (!C.Remote)
			{
				CardStandIn.Dim(real, _dim);
				s.Dimmed = real;
			}
			return true;
		}

		private void Retire(Slot s)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			s.Now = Leg.Done;
			NCard copy = s.Copy;
			NCard dimmed = s.Dimmed;
			s.Copy = null;
			s.Dimmed = null;
			try
			{
				if (copy != null)
				{
					if (FxKit.Alive((GodotObject)(object)copy))
					{
						((CanvasItem)copy).Modulate = Colors.White;
					}
					CardStandIn.Free(copy);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlechettesOrbit (free copy): " + ex.Message);
			}
			if (dimmed != null)
			{
				CardStandIn.Show(dimmed);
			}
		}

		private void ClearSlots()
		{
			for (int i = 0; i < _slots.Count; i++)
			{
				try
				{
					Retire(_slots[i]);
				}
				catch (Exception ex)
				{
					DevLog.Warn("FlechettesOrbit (retire): " + ex.Message);
				}
			}
			_slots.Clear();
			_orbiting = false;
		}

		private void StopShaking()
		{
			if (_shaking)
			{
				_shaking = false;
				StopShakes();
			}
		}

		private void RestoreAll()
		{
			try
			{
				ClearSlots();
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlechettesOrbit (restore): " + ex.Message);
			}
			StopShaking();
		}

		private void SendHome(float span)
		{
			_orbiting = false;
			foreach (Slot slot in _slots)
			{
				if (slot.Now == Leg.Out && slot.Copy == null)
				{
					slot.Now = Leg.Done;
				}
				else if (slot.Now == Leg.Out || slot.Now == Leg.Orbit)
				{
					BeginHome(slot, span);
				}
			}
		}

		private void BeginHome(Slot s, float span)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			Layer(s, behind: false);
			s.Now = Leg.Home;
			s.T = 0f;
			s.Span = span;
			s.FromPos = s.Pos;
			s.FromRot = s.Rot;
			s.FromScale = s.Scale;
		}

		private void Layer(Slot s, bool behind)
		{
			if (s.Behind == behind || !FxKit.Alive((GodotObject)(object)s.Copy))
			{
				return;
			}
			Node2D val = (behind ? Back : Front);
			if (!FxKit.Alive((GodotObject)(object)val))
			{
				return;
			}
			Node parent = ((Node)s.Copy).GetParent();
			if ((object)parent != val)
			{
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					parent.RemoveChild((Node)(object)s.Copy);
				}
				((Node)val).AddChild((Node)(object)s.Copy, false, (InternalMode)0);
			}
			s.Behind = behind;
		}

		private void OrbitPose(Slot s, out Vector2 pos, out float rot, out Vector2 scale, out float depth)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			float num = _spin + (float)Math.PI * 2f * (float)s.Index / (float)Mathf.Max(1, _count);
			float num2 = Mathf.Sin(num);
			float num3 = 1f - 0.14f * Mathf.Clamp(Arm, 0f, 1f);
			pos = C.Chest + new Vector2(Mathf.Cos(num) * _rx * num3, num2 * _ry * num3);
			depth = num2;
			float num4 = _cardScale * (0.84f + 0.16f * (num2 * 0.5f + 0.5f));
			scale = new Vector2(num4, num4);
			rot = -0.36f * num2;
			if (_hasMark && Arm > 0.01f)
			{
				Vector2 val = _markAt - pos;
				if (val.LengthSquared() > 1f)
				{
					rot = Mathf.LerpAngle(rot, val.Angle() + (float)Math.PI / 2f, 0.6f * Mathf.Clamp(Arm, 0f, 1f));
				}
			}
		}

		private void Advance(Slot s, float dt)
		{
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_029c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0307: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031e: Unknown result type (might be due to invalid IL or missing references)
			//IL_037b: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0384: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			if (s.Now == Leg.Done)
			{
				return;
			}
			NCard val = RealOf(s.Card);
			if (s.Copy == null)
			{
				if (s.Now != Leg.Out)
				{
					Retire(s);
					return;
				}
				s.T += dt;
				if (s.T < 0f || !Spawn(s, val))
				{
					return;
				}
				s.T = 0f;
			}
			else
			{
				if (!FxKit.Alive((GodotObject)(object)s.Copy))
				{
					Retire(s);
					return;
				}
				if (s.Dimmed != val)
				{
					if (s.Dimmed != null)
					{
						CardStandIn.Show(s.Dimmed);
					}
					s.Dimmed = null;
					if (val != null)
					{
						CardStandIn.Dim(val, _dim);
						s.Dimmed = val;
					}
				}
				else if (val != null && ((CanvasItem)val).Modulate.A > _dim + 0.04f)
				{
					CardStandIn.Dim(val, _dim);
				}
				s.T += dt;
			}
			float num = Mathf.Clamp(s.T / Mathf.Max(0.02f, s.Span), 0f, 1f);
			switch (s.Now)
			{
			case Leg.Out:
			{
				float num4 = 1f - (1f - num) * (1f - num) * (1f - num);
				OrbitPose(s, out var pos2, out var rot2, out var scale3, out var _);
				s.Pos = (C.Remote ? pos2 : s.FromPos.Lerp(pos2, num4));
				s.Rot = (C.Remote ? rot2 : Mathf.LerpAngle(s.FromRot, rot2, num4));
				s.Scale = (C.Remote ? scale3 : s.FromScale.Lerp(scale3, num4));
				s.Shade = 1f;
				s.Alpha = (C.Remote ? num4 : 1f);
				if (num >= 1f)
				{
					s.Now = Leg.Orbit;
				}
				break;
			}
			case Leg.Orbit:
			{
				OrbitPose(s, out var pos, out var rot, out var scale2, out var depth);
				s.Pos = pos;
				s.Rot = rot;
				s.Scale = scale2;
				s.Shade = 0.72f + 0.28f * (depth * 0.5f + 0.5f);
				s.Alpha = 1f;
				if (_layered)
				{
					if (depth < -0.08f)
					{
						Layer(s, behind: true);
					}
					else if (depth > 0.08f)
					{
						Layer(s, behind: false);
					}
				}
				break;
			}
			case Leg.Strike:
			{
				float num3 = num * (0.55f + 0.45f * num);
				Vector2 val2 = s.Spot - s.FromPos;
				s.Pos = s.FromPos + val2 * num3;
				if (val2.LengthSquared() > 1f)
				{
					s.Rot = Mathf.LerpAngle(s.FromRot, val2.Angle() + (float)Math.PI / 2f, Mathf.Min(1f, num * 4f));
				}
				s.Scale = s.FromScale * (1f - 0.2f * num);
				s.Shade = 1f;
				s.Alpha = 1f;
				if (num >= 1f)
				{
					BeginHome(s, _backTime);
				}
				break;
			}
			case Leg.Home:
			{
				float num2 = num * num * (3f - 2f * num);
				if (val != null)
				{
					CardStandIn.Pose(val, out var centre, out var rotation, out var scale);
					s.Pos = s.FromPos.Lerp(centre, num2);
					s.Rot = Mathf.LerpAngle(s.FromRot, rotation, num2);
					s.Scale = s.FromScale.Lerp(scale, num2);
					s.Alpha = 1f;
				}
				else
				{
					s.Alpha = 1f - num;
				}
				s.Shade = Mathf.Lerp(s.Shade, 1f, num2);
				if (num >= 1f)
				{
					Retire(s);
				}
				break;
			}
			}
		}

		private void Put(Slot s)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)s.Copy))
			{
				CardStandIn.Place(s.Copy, s.Pos, s.Rot, s.Scale);
				float num = ((C.Remote || Leaving) ? 1f : (1f / Mathf.Clamp(Fade, 0.4f, 1f)));
				((CanvasItem)s.Copy).Modulate = new Color(s.Shade, s.Shade, s.Shade, Mathf.Clamp(s.Alpha, 0f, 1f) * num);
			}
		}

		private void SetMark(Vector2 at, float width, float height)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			if (C.Remote)
			{
				_hasMark = true;
				_markAt = at;
			}
			else
			{
				if (!FxKit.Alive((GodotObject)(object)_mark))
				{
					return;
				}
				float num2;
				float num = (num2 = Mathf.Clamp(Mathf.Min(width, height) * 0.2f, 30f, Mathf.Max(30f, Tuning.F("flech.mark_size", 54f))));
				float num3 = Tuning.F("flech.mark_len", 22f);
				float num4 = (_markInward ? 1f : (-1f));
				int num5 = 0;
				for (int i = -1; i <= 1; i += 2)
				{
					for (int j = -1; j <= 1; j += 2)
					{
						Vector2 val = new Vector2((float)j * num2, (float)i * num);
						if (FxKit.Alive((GodotObject)(object)_corners[num5]))
						{
							_cornerPoints[0] = val + new Vector2((float)j * num3 * num4, 0f);
							_cornerPoints[1] = val;
							_cornerPoints[2] = val + new Vector2(0f, (float)i * num3 * num4);
							_corners[num5].Points = _cornerPoints;
						}
						num5++;
					}
				}
				if (!_hasMark || at.DistanceSquaredTo(_markAt) > 16f)
				{
					_markPop = 0f;
				}
				_hasMark = true;
				_markAt = at;
			}
		}

		private void TickMark(float dt)
		{
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)_mark))
			{
				float num = (_hasMark ? Mathf.Clamp(Arm, 0f, 1f) : 0f);
				if (num < 0.01f)
				{
					((CanvasItem)_mark).Visible = false;
					return;
				}
				_markPop = Mathf.Min(1f, _markPop + dt / 0.12f);
				float num2 = 1f - (1f - _markPop) * (1f - _markPop);
				float num3 = Mathf.Lerp(1.25f, 1f, num2) * (1f + 0.025f * Mathf.Sin(Time * 5.5f));
				((CanvasItem)_mark).Visible = true;
				_mark.GlobalPosition = _markAt;
				_mark.Scale = new Vector2(num3, num3);
				((CanvasItem)_mark).Modulate = new Color(1f, 1f, 1f, num);
			}
		}

		public override void OnTargetChanged()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (C.HasTarget)
				{
					SetMark(C.TargetChest, C.TargetWidth, C.TargetHeight);
					_markPop = 0f;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlechettesOrbit.OnTargetChanged: " + ex.Message);
			}
		}

		protected override void OnPlay(CardFxContext play)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			_waitingForHit = 0f;
			try
			{
				if (!_orbiting && !Leaving)
				{
					LiftOff();
				}
				if (play != null && play.HasTarget)
				{
					SetMark(play.TargetChest, play.TargetWidth, play.TargetHeight);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlechettesOrbit.OnPlay: " + ex);
				End();
			}
		}

		public bool Shoot(Vector2 at)
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				_waitingForHit = 0f;
				if (Finished || Leaving || !_orbiting)
				{
					return false;
				}
				Slot slot = null;
				float num = float.MaxValue;
				foreach (Slot slot2 in _slots)
				{
					if ((slot2.Now == Leg.Out || slot2.Now == Leg.Orbit) && FxKit.Alive((GodotObject)(object)slot2.Copy))
					{
						float num2 = slot2.Pos.DistanceSquaredTo(at);
						if (num2 < num)
						{
							num = num2;
							slot = slot2;
						}
					}
				}
				if (slot == null)
				{
					foreach (Slot slot3 in _slots)
					{
						if (slot3.Now == Leg.Out && slot3.Copy == null && Spawn(slot3, RealOf(slot3.Card)))
						{
							slot = slot3;
							break;
						}
					}
				}
				if (slot == null)
				{
					return false;
				}
				int num3 = _shots++;
				Vector2 spot = at + new Vector2((FxKit.Hash(num3 * 5 + 1) - 0.5f) * 56f, (FxKit.Hash(num3 * 9 + 2) - 0.5f) * 72f);
				Vector2 pos = slot.Pos;
				Layer(slot, behind: false);
				slot.Now = Leg.Strike;
				slot.T = 0f;
				slot.Span = _shotTime;
				slot.FromPos = slot.Pos;
				slot.FromRot = slot.Rot;
				slot.FromScale = slot.Scale;
				slot.Spot = spot;
				slot.Shade = 1f;
				slot.Alpha = 1f;
				Put(slot);
				Node fx = C.FrontParent;
				if (FxKit.Alive((GodotObject)(object)fx))
				{
					Vector2 val = spot - pos;
					Vector2 dir = (Vector2)((val.LengthSquared() > 1f) ? val.Normalized() : new Vector2(1f, 0f));
					Brush.Dart(fx, pos, spot, 170f, 8f, White, _shotTime, 0f, () =>
					{
						//IL_0007: Unknown result type (might be due to invalid IL or missing references)
						//IL_0020: Unknown result type (might be due to invalid IL or missing references)
						//IL_0044: Unknown result type (might be due to invalid IL or missing references)
						//IL_0049: Unknown result type (might be due to invalid IL or missing references)
						//IL_0055: Unknown result type (might be due to invalid IL or missing references)
						//IL_005a: Unknown result type (might be due to invalid IL or missing references)
						FxKit.Flash(fx, spot, new Color(1f, 1f, 1f, 0.85f), 36f, 1.7f, 0.12);
						Brush.Sparks(fx, spot, White, 4, 260f, -dir, 55f);
					});
				}
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlechettesOrbit.Shoot: " + ex);
				return false;
			}
		}

		public override void OnCardFinished()
		{
			try
			{
				SendHome(_quickHome);
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlechettesOrbit.OnCardFinished: " + ex.Message);
				RestoreAll();
			}
			Leave();
		}

		protected override void OnLeaving()
		{
			try
			{
				SendHome(_homeTime);
				StopShaking();
			}
			catch (Exception ex)
			{
				DevLog.Warn("FlechettesOrbit.OnLeaving: " + ex.Message);
				RestoreAll();
			}
		}

		protected override void OnEnd()
		{
			RestoreAll();
		}

		static FlechettesOrbit()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		}
	}
