// namespace: SilentFX.Powers
// type: AccelerantFx (class)
// source lines: 319-852
	public sealed class AccelerantFx : BasicPowerFx
	{
		private sealed class Mark
		{
			public Node2D Root;

			public Sprite2D Glow;

			public Sprite2D Orb;

			public Sprite2D Core;

			public readonly Polygon2D[] Chevrons = new Polygon2D[3];

			public readonly Polygon2D[] Ghosts = new Polygon2D[3];

			public Vector2 At;

			public float Unit = 1f;

			public float Show;

			public float Flare;

			public float Beat;

			public readonly Polygon2D[] Echoes = new Polygon2D[6];

			public readonly bool[] EchoOn = new bool[6];

			public readonly float[] EchoAge = new float[6];

			public readonly float[] EchoGain = new float[6];

			public readonly float[] EchoY = new float[6];

			public readonly float[] EchoLit = new float[6];

			public int NextEcho;

			public bool Seen;
		}

		private readonly List<FxTarget> _targetScratch = new List<FxTarget>();

		private const int MaxChevrons = 3;

		private const float Step = 19f;

		private const int MaxEchoes = 6;

		private static readonly Color PulseColor = new Color(0.9f, 1f, 0.62f, 1f);

		private static readonly List<AccelerantFx> Live = new List<AccelerantFx>();

		private readonly Dictionary<object, Mark> _marks = new Dictionary<object, Mark>();

		private readonly List<object> _gone = new List<object>();

		private int _total = 1;

		private float _recount;

		private float _spit;

		protected override string NodeName => "Accelerant";

		protected override bool UseBack => false;

		protected override float KickDecay => 2.6f;

		private static float PulseTime => Mathf.Max(0.12f, Tuning.F("accelerant.pulse_time", 0.44f));

		private static AccelerantFx Primary()
		{
			foreach (AccelerantFx item in Live)
			{
				if (!item.Dead && !item.Stopping)
				{
					return item;
				}
			}
			return null;
		}

		protected override void Build(bool animateIn)
		{
			Live.Add(this);
			_total = Total();
		}

		protected override void OnDispose()
		{
			Live.Remove(this);
		}

		private int Total()
		{
			try
			{
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				if (combatRoom == null)
				{
					return Math.Max(1, Amount);
				}
				int num = 0;
				foreach (NCreature creatureNode in combatRoom.CreatureNodes)
				{
					Creature val = (FxKit.Alive((GodotObject)(object)creatureNode) ? creatureNode.Entity : null);
					if (val != null && val.IsPlayer && val.IsAlive)
					{
						num += val.GetPowerAmount<AccelerantPower>();
					}
				}
				return Math.Max(1, num);
			}
			catch
			{
				return Math.Max(1, Amount);
			}
		}

		public override void OnAmountChanged(int amount)
		{
			_recount = 0f;
		}

		private Mark MakeMark()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected Obj, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Expected Obj, but got Unknown
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Expected Obj, but got Unknown
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Expected Obj, but got Unknown
			Mark mark = new Mark
			{
				Root = new Node2D()
			};
			((Node)Front).AddChild((Node)(object)mark.Root, false, (InternalMode)0);
			mark.Glow = FxKit.Glow(new Color(PoisonFx.Acid.R, PoisonFx.Acid.G, PoisonFx.Acid.B, 0.25f), new Vector2(70f, 120f));
			((Node)mark.Root).AddChild((Node)(object)mark.Glow, false, (InternalMode)0);
			mark.Orb = FxKit.Glow(new Color(PoisonFx.Acid.R, PoisonFx.Acid.G, PoisonFx.Acid.B, 0.6f), new Vector2(64f, 64f));
			((Node2D)mark.Orb).Position = new Vector2(0f, 24f);
			((Node)mark.Root).AddChild((Node)(object)mark.Orb, false, (InternalMode)0);
			mark.Core = FxKit.Glow(new Color(0.92f, 1f, 0.72f, 0.95f), new Vector2(24f, 24f));
			((Node2D)mark.Core).Position = new Vector2(0f, 24f);
			((Node)mark.Root).AddChild((Node)(object)mark.Core, false, (InternalMode)0);
			Vector2[] polygon = new Vector2[6]
			{
				new Vector2(0f, -9f),
				new Vector2(15f, 5f),
				new Vector2(15f, 12f),
				new Vector2(0f, -2f),
				new Vector2(-15f, 12f),
				new Vector2(-15f, 5f)
			};
			for (int i = 0; i < 3; i++)
			{
				mark.Chevrons[i] = new Polygon2D
				{
					Polygon = polygon,
					Color = PoisonFx.Acid,
					Material = (Material)(object)FxKit.Additive,
					Antialiased = true,
					Position = new Vector2(0f, (float)(-i) * 19f)
				};
				((Node)mark.Root).AddChild((Node)(object)mark.Chevrons[i], false, (InternalMode)0);
				mark.Ghosts[i] = new Polygon2D
				{
					Polygon = polygon,
					Color = PoisonFx.Acid,
					Material = (Material)(object)FxKit.Additive,
					Antialiased = true,
					Visible = false
				};
				((Node)mark.Root).AddChild((Node)(object)mark.Ghosts[i], false, (InternalMode)0);
			}
			for (int j = 0; j < 6; j++)
			{
				mark.Echoes[j] = new Polygon2D
				{
					Polygon = polygon,
					Color = PulseColor,
					Material = (Material)(object)FxKit.Additive,
					Antialiased = true,
					Visible = false
				};
				((Node)mark.Root).AddChild((Node)(object)mark.Echoes[j], false, (InternalMode)0);
			}
			return mark;
		}

		protected override void Tick(float dt)
		{
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_0415: Unknown result type (might be due to invalid IL or missing references)
			//IL_043d: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			_recount -= dt;
			if (_recount <= 0f)
			{
				_recount = 0.25f;
				_total = Total();
			}
			bool flag = Primary() == this;
			foreach (Mark value3 in _marks.Values)
			{
				value3.Seen = false;
			}
			if (flag)
			{
				Anchor.SafeTargets(_targetScratch);
				float num = Tuning.F("accelerant.x", 0.46f);
				float num2 = Tuning.F("accelerant.y", 0.8f);
				float num3 = Tuning.F("accelerant.size", 1f);
				foreach (FxTarget item in _targetScratch)
				{
					if (item.Poison > 0 && item.Key != null)
					{
						if (!_marks.TryGetValue(item.Key, out var value))
						{
							value = MakeMark();
							_marks[item.Key] = value;
						}
						value.Seen = true;
						float num4 = Mathf.Max(60f, item.Feet.Y - item.Chest.Y);
						value.At = item.Chest + new Vector2((item.Width * num + 16f) * item.Side, (0f - num4) * num2);
						value.Unit = Mathf.Clamp(item.Width / 230f, 1f, 2.1f) * num3;
					}
				}
			}
			_gone.Clear();
			foreach (KeyValuePair<object, Mark> mark in _marks)
			{
				Mark value2 = mark.Value;
				value2.Show = FxKit.Damp(value2.Show, value2.Seen ? 1f : 0f, 7f, dt);
				value2.Flare = FxKit.Damp(value2.Flare, 0f, 2.6f, dt);
				value2.Beat = FxKit.Damp(value2.Beat, 0f, 9f, dt);
				for (int i = 0; i < 6; i++)
				{
					if (value2.EchoOn[i])
					{
						value2.EchoAge[i] += dt;
						if (value2.EchoAge[i] >= PulseTime)
						{
							value2.EchoOn[i] = false;
						}
					}
				}
				if (!value2.Seen && value2.Show < 0.02f)
				{
					_gone.Add(mark.Key);
				}
			}
			foreach (object item2 in _gone)
			{
				if (FxKit.Alive((GodotObject)(object)_marks[item2].Root))
				{
					((Node)_marks[item2].Root).QueueFree();
				}
				_marks.Remove(item2);
			}
			_spit -= dt;
			if (!flag || _total <= 3 || !(_spit <= 0f) || !FxKit.Alive((GodotObject)(object)Anchor.FrontParent))
			{
				return;
			}
			_spit = 0.2f;
			foreach (Mark value4 in _marks.Values)
			{
				if (value4.Seen && !(value4.Show < 0.5f))
				{
					FxKit.Burst(Anchor.FrontParent, value4.At + new Vector2(0f, 20f * value4.Unit), new Color(PoisonFx.Acid.R, PoisonFx.Acid.G, PoisonFx.Acid.B, 0.95f), 2, 90f * value4.Unit, 220f * value4.Unit, 0.45, 0.05f * value4.Unit, 0.12f * value4.Unit, new Vector2(0f, -220f), 24f, new Vector2(0f, -1f), 6f * value4.Unit);
				}
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			Primary()?.Flare(at);
		}

		private void Flare(Vector2? at)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			Mark mark = null;
			float num = float.MaxValue;
			foreach (Mark value in _marks.Values)
			{
				if (!at.HasValue)
				{
					Ignite(value);
					continue;
				}
				Vector2 val = value.At - at.Value;
				float num2 = val.LengthSquared();
				if (!(num2 >= num))
				{
					num = num2;
					mark = value;
				}
			}
			if (mark != null)
			{
				Ignite(mark);
			}
		}

		public static void OnPoisonBites(object enemy, int pulses)
		{
			AccelerantFx fx = Primary();
			if (fx != null && enemy != null)
			{
				Action action = () =>
				{
					fx.Answer(enemy, pulses);
				};
				if (!TimeFreeze.Defer(action))
				{
					action();
				}
			}
		}

		private void Answer(object enemy, int pulses)
		{
			if (Dead || Stopping || !FxKit.Alive((GodotObject)(object)Front))
			{
				return;
			}
			float num = Tuning.F("poison.pulse_gap", 0.2f);
			int num2 = Mathf.Clamp(pulses, 1, 6);
			bool shared = MultiplayerFx.Active && Anchor.Owner != null;
			Node parent = (Node)(shared ? ((object)Anchor.FrontParent) : ((object)Front));
			for (int i = 0; i < num2; i++)
			{
				FxKit.After(parent, (float)i * num, () =>
				{
					AccelerantFx accelerantFx = (shared ? Primary() : this);
					if ((!shared || (MultiplayerFx.Active && accelerantFx?.Anchor.FrontParent == parent)) && accelerantFx != null && !accelerantFx.Dead && accelerantFx._marks.TryGetValue(enemy, out var value))
					{
						accelerantFx.Ignite(value);
					}
				});
			}
		}

		private void Ignite(Mark mark)
		{
			mark.Flare = 1f;
			mark.Beat = 1f;
			int num = Mathf.Clamp(Tuning.I("accelerant.pulse_arrows", 2), 1, 2);
			for (int i = 0; i < num; i++)
			{
				int nextEcho = mark.NextEcho;
				mark.NextEcho = (mark.NextEcho + 1) % 6;
				mark.EchoOn[nextEcho] = true;
				mark.EchoAge[nextEcho] = -0.07f * (float)i;
				mark.EchoGain[nextEcho] = ((i == 0) ? 1f : 0.45f);
			}
		}

		protected override void Layout()
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_030b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0315: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0464: Unknown result type (might be due to invalid IL or missing references)
			//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_060c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
			int num = Mathf.Clamp(_total, 1, 3);
			bool flag = _total > 3;
			float num2 = Tuning.F("accelerant.chase", 7f) * (flag ? 2.1f : 1f);
			float num3 = (flag ? 1.32f : 1f);
			float pulseTime = PulseTime;
			foreach (Mark value in _marks.Values)
			{
				if (!FxKit.Alive((GodotObject)(object)value.Root))
				{
					continue;
				}
				value.Root.GlobalPosition = value.At + new Vector2(0f, Mathf.Sin(Time * 2f) * 3f * value.Unit);
				value.Root.Scale = new Vector2(value.Unit, value.Unit);
				((CanvasItem)value.Root).Modulate = new Color(1f, 1f, 1f, A * value.Show);
				float num4 = 19f * (float)(num - 1) * num3 + Tuning.F("accelerant.pulse_rise", 70f) * num3;
				for (int i = 0; i < 6; i++)
				{
					value.EchoLit[i] = 0f;
					Polygon2D val = value.Echoes[i];
					if (FxKit.Alive((GodotObject)(object)val) && (((CanvasItem)val).Visible = value.EchoOn[i] && value.EchoAge[i] >= 0f))
					{
						float num5 = Mathf.Clamp(value.EchoAge[i] / pulseTime, 0f, 1f);
						float num6 = 1f - (1f - num5) * (1f - num5);
						float num7 = value.EchoGain[i] * Mathf.Pow(1f - num5, 1.3f);
						value.EchoY[i] = (0f - num4) * num6;
						value.EchoLit[i] = num7;
						((Node2D)val).Position = new Vector2(0f, value.EchoY[i]);
						float num8 = (0.95f + 0.5f * num6) * num3 * (0.8f + 0.2f * value.EchoGain[i]);
						((Node2D)val).Scale = new Vector2(num8, num8);
						((CanvasItem)val).Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(1.5f * num7 * Mathf.Min(1f, num5 * 14f + 0.35f), 0f, 1.5f));
					}
				}
				if (FxKit.Alive((GodotObject)(object)value.Glow))
				{
					float num9 = 19f * (float)(num - 1) * num3;
					((Node2D)value.Glow).Position = new Vector2(0f, (0f - num9) * 0.5f);
					((Node2D)value.Glow).Scale = new Vector2(76f * num3, 64f + num9 * 1.5f) / 64f;
					float num10 = (flag ? 0.36f : 0.2f) + 0.06f * Mathf.Sin(Time * num2) + 0.5f * value.Flare;
					((CanvasItem)value.Glow).Modulate = new Color(PoisonFx.Acid.R, PoisonFx.Acid.G, PoisonFx.Acid.B, num10);
				}
				float num11 = 0.5f + 0.5f * Mathf.Sin(Time * num2);
				if (FxKit.Alive((GodotObject)(object)value.Orb))
				{
					float num12 = (1f + 0.1f * Mathf.Sin(Time * 3.1f) + 0.14f * num11 + 0.55f * value.Beat) * (flag ? 1.4f : 1f);
					((Node2D)value.Orb).Scale = new Vector2(num12, num12);
					((CanvasItem)value.Orb).Modulate = new Color(PoisonFx.Acid.R, PoisonFx.Acid.G, PoisonFx.Acid.B, Mathf.Clamp((flag ? 0.8f : 0.58f) + 0.12f * num11 + 0.4f * value.Beat, 0f, 1f));
				}
				if (FxKit.Alive((GodotObject)(object)value.Core))
				{
					float num13 = (24f + 5f * num11 + 14f * value.Beat) * (flag ? 1.35f : 1f) / 64f;
					((Node2D)value.Core).Scale = new Vector2(num13, num13);
				}
				for (int j = 0; j < 3; j++)
				{
					Polygon2D val2 = value.Chevrons[j];
					if (!FxKit.Alive((GodotObject)(object)val2))
					{
						continue;
					}
					((CanvasItem)val2).Visible = j < num;
					float num14 = Mathf.Max(0f, Mathf.Sin(Time * num2 - (float)j * 1.1f));
					float num15 = 0f;
					for (int k = 0; k < 6; k++)
					{
						if (!(value.EchoLit[k] <= 0f))
						{
							float num16 = (value.EchoY[k] + (float)j * 19f * num3) / (19f * num3 * 0.9f);
							num15 = Mathf.Max(num15, value.EchoLit[k] * Mathf.Exp((0f - num16) * num16));
						}
					}
					float num17 = Mathf.Clamp((flag ? 0.74f : 0.56f) + 0.7f * num14 * num14 + 1.1f * num15 + 0.25f * value.Flare, 0f, 1.6f);
					((CanvasItem)val2).Modulate = new Color(1f, 1f, 1f, num17);
					float num18 = (0.86f + 0.26f * num15 + (flag ? (0.1f * num14) : 0f)) * num3;
					((Node2D)val2).Scale = new Vector2(num18, num18);
					((Node2D)val2).Position = new Vector2(0f, (float)(-j) * 19f * num3);
					Polygon2D val3 = value.Ghosts[j];
					if (FxKit.Alive((GodotObject)(object)val3))
					{
						((CanvasItem)val3).Visible = flag && j < num;
						if (((CanvasItem)val3).Visible)
						{
							float num19 = (Time * 1.7f + (float)j * 0.31f) % 1f;
							((Node2D)val3).Position = new Vector2(0f, (float)(-j) * 19f * num3 - num19 * 54f);
							float num20 = num18 * (1f + 0.55f * num19);
							((Node2D)val3).Scale = new Vector2(num20, num20);
							((CanvasItem)val3).Modulate = new Color(1f, 1f, 1f, 0.55f * (1f - num19) * (1f - num19));
						}
					}
				}
			}
		}

		static AccelerantFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
