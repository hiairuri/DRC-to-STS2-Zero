// namespace: SilentFX.Powers
// type: ShadowTwinFx (class)
// source lines: 13061-13486
	public sealed class ShadowTwinFx : BasicPowerFx
	{
		private sealed class Spot
		{
			public object Key;

			public Vector2 Feet;

			public float Width = 200f;

			public bool Here;

			public float Seen;

			public float Hit;

			public Sprite2D Ground;

			public Sprite2D Rim;

			public Sprite2D Core;
		}

		private static readonly Color Cold = new Color(0.36f, 0.32f, 1f, 1f);

		private const int MaxSpots = 8;

		private readonly ShadowPresence _presence = new ShadowPresence(full: true);

		private readonly List<FxTarget> _targetScratch = new List<FxTarget>();

		private readonly List<Spot> _spots = new List<Spot>();

		private Murk _reachLeft;

		private Murk _reachRight;

		private Vector2 _tipLeft;

		private Vector2 _tipRight;

		private float _farLeft;

		private float _farRight;

		private float _crept;

		private float _scan;

		private float _rise;

		private float _surge;

		private float _sinceHit = 10f;

		private float _aim = 1f;

		protected override string NodeName => "ShadowTwin";

		protected override float KickDecay => 5f;

		protected override void Build(bool animateIn)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			if (Tuning.F("shadow.reach", 1f) > 0.01f && FxKit.Alive((GodotObject)(object)Back) && ((Node)Back).IsInsideTree())
			{
				_reachLeft = Murk.Make((Node)(object)Back, 0.3f);
				_reachRight = Murk.Make((Node)(object)Back, 0.3f);
			}
			_crept = (animateIn ? 0f : 1f);
			_presence.Build(Back, Front, Anchor.SafeBodySize());
			_rise = (animateIn ? 0f : 1f);
			if (!animateIn)
			{
				return;
			}
			_surge = 1f;
			try
			{
				_presence.Erupt(Anchor, 0.9f);
			}
			catch (Exception ex)
			{
				DevLog.Warn("Shadow aura eruption failed: " + ex.Message);
			}
		}

		protected override void OnStacked()
		{
			_rise = Mathf.Min(_rise, 0.4f);
			_surge = 1f;
		}

		protected override void Tick(float dt)
		{
			_rise = Mathf.Min(1f, _rise + dt / Mathf.Max(0.05f, Tuning.F("shadow.twin_rise", 0.42f)));
			_sinceHit += dt;
			_surge = FxKit.Damp(_surge, 0f, 3.2f, dt);
			ReachStep(dt);
		}

		private Vector2 GroundPoint()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = Anchor.SafeBodySize();
			return Anchor.Feet() + new Vector2(0f, 2f - val.Y * Tuning.F("shadow.ground", 0.07f));
		}

		private void ReachStep(float dt)
		{
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			if (_reachLeft == null && _reachRight == null)
			{
				return;
			}
			if (_rise > 0.35f && !Stopping)
			{
				_crept = Mathf.Min(1f, _crept + dt / Mathf.Clamp(Tuning.F("shadow.reach_time", 0.6f), 0.05f, 3f));
			}
			_scan -= dt;
			if (_scan <= 0f)
			{
				_scan = 0.2f;
				Scan();
			}
			Vector2 val = GroundPoint();
			_farLeft = 0f;
			_farRight = 0f;
			_tipLeft = val;
			_tipRight = val;
			Vector2 val2 = val;
			Vector2 val3 = val;
			for (int i = 0; i < _spots.Count; i++)
			{
				Spot spot = _spots[i];
				if (spot.Here)
				{
					float num = spot.Feet.X - val.X;
					if (num >= 0f && num > _farRight)
					{
						_farRight = num;
						val3 = spot.Feet;
					}
					else if (num < 0f && 0f - num > _farLeft)
					{
						_farLeft = 0f - num;
						val2 = spot.Feet;
					}
				}
			}
			float num2 = 1f - Mathf.Pow(1f - Mathf.Clamp(_crept, 0f, 1f), 2.4f);
			_tipLeft = val.Lerp(val2, num2);
			_tipRight = val.Lerp(val3, num2);
			for (int num3 = _spots.Count - 1; num3 >= 0; num3--)
			{
				Spot spot2 = _spots[num3];
				float num4 = spot2.Feet.X - val.X;
				float num5 = ((num4 >= 0f) ? _farRight : _farLeft) * num2;
				float target = (spot2.Here ? Mathf.Clamp((num5 - Mathf.Abs(num4) + 120f) / 120f, 0f, 1f) : 0f);
				spot2.Seen = FxKit.Damp(spot2.Seen, target, 7f, dt);
				spot2.Hit = FxKit.Damp(spot2.Hit, 0f, 6f, dt);
				if (!spot2.Here && spot2.Seen < 0.02f)
				{
					FreeSpot(spot2);
					_spots.RemoveAt(num3);
				}
			}
		}

		private void Scan()
		{
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			Anchor.SafeTargets(_targetScratch);
			for (int i = 0; i < _spots.Count; i++)
			{
				_spots[i].Here = false;
			}
			for (int j = 0; j < _targetScratch.Count; j++)
			{
				FxTarget fxTarget = _targetScratch[j];
				if (fxTarget == null)
				{
					continue;
				}
				Spot spot = null;
				for (int k = 0; k < _spots.Count && spot == null; k++)
				{
					Spot spot2 = _spots[k];
					if (spot2.Here)
					{
						continue;
					}
					bool num;
					if (fxTarget.Key == null)
					{
						if (spot2.Key != null)
						{
							continue;
						}
						num = spot2.Feet.DistanceSquaredTo(fxTarget.Feet) < 3600f;
					}
					else
					{
						num = spot2.Key == fxTarget.Key;
					}
					if (num)
					{
						spot = spot2;
					}
				}
				if (spot == null)
				{
					if (_spots.Count >= 8 || !FxKit.Alive((GodotObject)(object)Back))
					{
						continue;
					}
					spot = new Spot
					{
						Key = fxTarget.Key
					};
					spot.Ground = Shade.Blob(new Vector2(64f, 64f), 0f);
					spot.Rim = FxKit.Glow(new Color(Cold.R, Cold.G, Cold.B, 0f), new Vector2(64f, 64f));
					spot.Core = Shade.Blob(new Vector2(64f, 64f), 0f);
					((Node)Back).AddChild((Node)(object)spot.Ground, false, (InternalMode)0);
					((Node)Back).AddChild((Node)(object)spot.Rim, false, (InternalMode)0);
					((Node)Back).AddChild((Node)(object)spot.Core, false, (InternalMode)0);
					_spots.Add(spot);
				}
				spot.Here = true;
				spot.Feet = fxTarget.Feet;
				spot.Width = Mathf.Max(fxTarget.Width, 120f);
			}
		}

		private static void FreeSpot(Spot spot)
		{
			if (FxKit.Alive((GodotObject)(object)spot.Ground))
			{
				((Node)spot.Ground).QueueFree();
			}
			if (FxKit.Alive((GodotObject)(object)spot.Rim))
			{
				((Node)spot.Rim).QueueFree();
			}
			if (FxKit.Alive((GodotObject)(object)spot.Core))
			{
				((Node)spot.Core).QueueFree();
			}
		}

		private void LayReach()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0260: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			if (_reachLeft == null && _reachRight == null)
			{
				return;
			}
			Vector2 val = Anchor.SafeBodySize();
			Vector2 a = GroundPoint();
			float num = Mathf.Clamp(Tuning.F("shadow.reach", 1f), 0f, 1f);
			float num2 = Mathf.Sin(Time * 0.9f) + 0.25f * Mathf.Sin(Time * 1.9f + 0.7f);
			float thick = Mathf.Clamp(val.X * 0.22f, 26f, 60f) * Mathf.Clamp(Tuning.F("shadow.reach_thick", 1f), 0.3f, 3f) * (1f + 0.04f * num2 + 0.3f * Kick);
			float num3 = A * num * 0.8f * Mathf.Clamp(_rise * 2f, 0f, 1f);
			_reachLeft?.Lay(a, _tipLeft, thick, (_farLeft > 1f) ? num3 : 0f);
			_reachRight?.Lay(a, _tipRight, thick, (_farRight > 1f) ? num3 : 0f);
			for (int i = 0; i < _spots.Count; i++)
			{
				Spot spot = _spots[i];
				float num4 = Mathf.Clamp(spot.Seen, 0f, 1f) * A * num;
				float num5 = spot.Width * 1.15f * (0.55f + 0.45f * spot.Seen) * (1f + 0.03f * num2 + 0.14f * spot.Hit);
				float num6 = Mathf.Clamp(num5 * 0.24f, 12f, 70f);
				if (FxKit.Alive((GodotObject)(object)spot.Ground))
				{
					((Node2D)spot.Ground).GlobalPosition = spot.Feet;
					((Node2D)spot.Ground).Scale = new Vector2(num5 * 1.7f, num6 * 2.1f) / 64f;
					((CanvasItem)spot.Ground).Modulate = Shade.Inked(0.82f * num4);
				}
				if (FxKit.Alive((GodotObject)(object)spot.Rim))
				{
					((Node2D)spot.Rim).GlobalPosition = spot.Feet;
					((Node2D)spot.Rim).Scale = new Vector2(num5 * 1.3f, num6 * 2f) / 64f;
					((CanvasItem)spot.Rim).Modulate = new Color(Cold.R, Cold.G, Cold.B, Mathf.Clamp(0.18f + 0.04f * num2 + 0.35f * spot.Hit, 0f, 0.9f) * num4);
				}
				if (FxKit.Alive((GodotObject)(object)spot.Core))
				{
					((Node2D)spot.Core).GlobalPosition = spot.Feet;
					((Node2D)spot.Core).Scale = new Vector2(num5 * 1.2f, num6 * 1.2f) / 64f;
					((CanvasItem)spot.Core).Modulate = Shade.Inked(0.9f * num4);
				}
			}
		}

		private float Lunge()
		{
			if (_sinceHit < 0.05f)
			{
				return _sinceHit / 0.05f;
			}
			float num = 1f - (_sinceHit - 0.05f) / 0.3f;
			if (!(num <= 0f))
			{
				return num * num;
			}
			return 0f;
		}

		protected override void Layout()
		{
			if (FxKit.Alive((GodotObject)(object)Back))
			{
				float flood = Intro.BackOut(Mathf.Clamp(_rise * 1.6f, 0f, 1f));
				_presence.Layout(Anchor, Time, A, flood, Intro.BackOut(_rise), _surge, Lunge(), _aim, Kick);
				LayReach();
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			_sinceHit = 0f;
			Vector2 val = Anchor.Feet();
			ref float aim = ref _aim;
			float num;
			if (at.HasValue)
			{
				num = ((at.Value.X < val.X) ? (-1f) : 1f);
			}
			else
			{
				num = Anchor.Side();
			}
			aim = num;
			if (at.HasValue)
			{
				Spot spot = null;
				float num2 = float.MaxValue;
				for (int i = 0; i < _spots.Count; i++)
				{
					float num3 = Mathf.Abs(_spots[i].Feet.X - at.Value.X);
					if (_spots[i].Here && !(num3 >= num2))
					{
						num2 = num3;
						spot = _spots[i];
					}
				}
				if (spot != null && num2 < spot.Width)
				{
					spot.Hit = 1f;
				}
			}
			Node backParent = Anchor.BackParent;
			if (FxKit.Alive((GodotObject)(object)backParent))
			{
				Vector2 val2 = Anchor.SafeBodySize();
				Shade.Motes(backParent, val + new Vector2(0f, (0f - val2.Y) * 0.12f), 5, 280f, new Vector2(_aim, -0.35f), 40f, val2.X * 0.3f, 0.32);
			}
		}

		static ShadowTwinFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
