// namespace: SilentFX.Cards
// type: ShadowRoad (class)
// source lines: 91513-92012
	internal sealed class ShadowRoad
	{
		public const int Stops = 10;

		private const int Sub = 18;

		private const int Table = 240;

		private static readonly float[] Speeds = new float[10] { 2f, 2.4f, 2.3f, 1.9f, 1.5f, 1.9f, 1.6f, 1.25f, 0.85f, 0.22f };

		public Vector2 View;

		public Vector2 Feet;

		public Vector2 Start;

		public Vector2 End;

		public Vector2 Pile;

		public Vector2 Hang;

		public float Dir = 1f;

		public float Level;

		public float Tall = 320f;

		public float CardWidth = 230f;

		public int Count;

		private Vector2 _bend;

		private Vector2[] _loop;

		private float[] _length;

		private float _total = 1f;

		private float _land = 0.2f;

		private readonly float[] _stopS = new float[10];

		private readonly float[] _stopU = new float[10];

		private readonly float[] _reach = new float[10];

		private readonly float[] _uOfS = new float[241];

		public float Total => _total;

		public float HangReach => _reach[9] * View.Y;

		public float LandReach => _land * View.Y;

		public static ShadowRoad Make(CardFxContext c, CardModel except, List<HandCards.Entry> hand)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			ShadowRoad shadowRoad = new ShadowRoad();
			Vector2 val = c.View;
			if (val.X < 200f || val.Y < 200f)
			{
				val = new Vector2(1920f, 1080f);
			}
			shadowRoad.View = val;
			float num = float.MaxValue;
			float num2 = float.MinValue;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			int num6 = 0;
			if (hand != null)
			{
				foreach (HandCards.Entry item in hand)
				{
					if (item.Card != null && item.Card != except && FxKit.Alive((GodotObject)(object)item.Node) && ((Node)item.Node).IsInsideTree())
					{
						CardStandIn.Pose(item.Node, out var centre, out var _, out var scale);
						if (!(scale.X < 0.05f) && !(scale.Y < 0.05f))
						{
							float num7 = 150f * scale.X;
							num = Mathf.Min(num, centre.X - num7);
							num2 = Mathf.Max(num2, centre.X + num7);
							num3 += centre.Y;
							num4 = Mathf.Max(num4, 422f * scale.Y);
							num5 = Mathf.Max(num5, num7 * 2f);
							num6++;
						}
					}
				}
			}
			if (num6 == 0)
			{
				num = val.X * 0.36f;
				num2 = val.X * 0.64f;
				num3 = val.Y * 0.92f;
				num4 = 320f;
				num5 = 230f;
			}
			else
			{
				num3 /= (float)num6;
			}
			shadowRoad.Count = num6;
			shadowRoad.Level = Mathf.Clamp(num3, val.Y * 0.7f, val.Y * 1.05f);
			shadowRoad.Tall = Mathf.Clamp(num4, 180f, 520f);
			shadowRoad.CardWidth = Mathf.Clamp(num5, 120f, 360f);
			shadowRoad.Pile = GameRefs.PilePoint((PileType)3);
			float num8 = (shadowRoad.Dir = ((shadowRoad.Pile.X >= (num + num2) * 0.5f) ? 1f : (-1f)));
			float num9 = ((num8 > 0f) ? num2 : num);
			float num10 = ((num8 > 0f) ? num : num2);
			shadowRoad.Start = new Vector2(num10 - num8 * shadowRoad.CardWidth * 0.3f, shadowRoad.Level);
			shadowRoad.End = new Vector2(num9 + num8 * shadowRoad.CardWidth * 0.2f, shadowRoad.Level);
			if ((shadowRoad.End.X - shadowRoad.Start.X) * num8 < 160f)
			{
				shadowRoad.End = new Vector2(shadowRoad.Start.X + num8 * 160f, shadowRoad.Level);
			}
			shadowRoad.Feet = c.Feet + new Vector2(0f, 4f - c.Body.Y * Tuning.F("shadow.ground", 0.07f));
			shadowRoad._bend = new Vector2(Mathf.Lerp(shadowRoad.Feet.X, shadowRoad.Start.X, 0.7f), shadowRoad.Feet.Y + 40f);
			shadowRoad.Build();
			return shadowRoad;
		}

		private void Build()
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0291: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_030f: Unknown result type (might be due to invalid IL or missing references)
			//IL_031f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0343: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_0358: Unknown result type (might be due to invalid IL or missing references)
			//IL_0377: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0500: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_0425: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0435: Unknown result type (might be due to invalid IL or missing references)
			//IL_043c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0457: Unknown result type (might be due to invalid IL or missing references)
			//IL_0459: Unknown result type (might be due to invalid IL or missing references)
			//IL_045e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0468: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0474: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0480: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_048a: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_0498: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_054c: Unknown result type (might be due to invalid IL or missing references)
			//IL_071d: Unknown result type (might be due to invalid IL or missing references)
			Vector2 v = View;
			float dir = Dir;
			float num = v.Y * Mathf.Clamp(Tuning.F("shadowstep.dip", 0.032f), -0.05f, 0.15f);
			float num2 = (Start.X + End.X) * 0.5f;
			float num3 = ((dir > 0f) ? Mathf.Min(Pile.X - v.X * 0.135f, End.X + v.X * 0.06f) : Mathf.Max(Pile.X + v.X * 0.135f, End.X - v.X * 0.06f));
			if ((num3 - Mathf.Lerp(num2, End.X, 0.55f)) * dir < 60f)
			{
				num3 = Mathf.Lerp(num2, End.X, 0.55f) + dir * 60f;
			}
			float num4 = X(0.935f);
			float num5 = X(0.915f);
			if ((num5 - num3) * dir < v.X * 0.06f)
			{
				num5 = num3 + dir * v.X * 0.06f;
			}
			if ((num4 - num5) * dir < v.X * 0.015f)
			{
				num4 = num5 + dir * v.X * 0.015f;
			}
			Hang = new Vector2(Feet.X + dir * v.X * 0.05f, v.Y * 0.07f);
			Vector2[] array = new Vector2[10]
			{
				Start,
				new Vector2(Mathf.Lerp(Start.X, num2, 0.55f), Level + num * 0.75f),
				new Vector2(Mathf.Lerp(num2, End.X, 0.55f), Level + num),
				new Vector2(num3, Level + num * 0.6f),
				new Vector2(num5, Mathf.Min(v.Y * 0.8f, Level - v.Y * 0.1f)),
				new Vector2(num4, v.Y * 0.52f),
				new Vector2(num4 - dir * v.X * 0.01f, v.Y * 0.24f),
				new Vector2(Mathf.Lerp(num4, Hang.X, 0.2f), v.Y * 0.035f),
				new Vector2(Mathf.Lerp(num4, Hang.X, 0.6f), v.Y * 0.02f),
				Hang
			};
			_loop = new Vector2[163];
			int num6 = 0;
			for (int i = 0; i < 9; i++)
			{
				Vector2 val = array[Mathf.Max(i - 1, 0)];
				Vector2 val2 = array[i];
				Vector2 val3 = array[i + 1];
				Vector2 val4 = array[Mathf.Min(i + 2, 9)];
				for (int j = 0; j < 18; j++)
				{
					float num7 = (float)j / 18f;
					_loop[num6++] = 0.5f * (2f * val2 + (-val + val3) * num7 + (2f * val - 5f * val2 + 4f * val3 - val4) * (num7 * num7) + (-val + 3f * val2 - 3f * val3 + val4) * (num7 * num7 * num7));
				}
			}
			_loop[num6] = array[9];
			_length = new float[_loop.Length];
			for (int k = 1; k < _loop.Length; k++)
			{
				_length[k] = _length[k - 1] + _loop[k - 1].DistanceTo(_loop[k]);
			}
			_total = Mathf.Max(1f, _length[_loop.Length - 1]);
			for (int l = 0; l < 10; l++)
			{
				_stopS[l] = _length[l * 18] / _total;
			}
			_uOfS[0] = 0f;
			for (int m = 1; m <= 240; m++)
			{
				float x = ((float)m - 0.5f) / 240f;
				_uOfS[m] = _uOfS[m - 1] + 1f / (240f * Mathf.Max(0.05f, Key(_stopS, Speeds, x)));
			}
			float num8 = Mathf.Max(0.0001f, _uOfS[240]);
			for (int n = 0; n <= 240; n++)
			{
				_uOfS[n] /= num8;
			}
			for (int num9 = 0; num9 < 10; num9++)
			{
				float num10 = _stopS[num9] * 240f;
				int num11 = Mathf.Clamp((int)num10, 0, 239);
				_stopU[num9] = Mathf.Lerp(_uOfS[num11], _uOfS[num11 + 1], num10 - (float)num11);
			}
			float num12 = 1.4f * Mathf.Clamp(Tuning.F("shadowstep.size", 1f), 0.4f, 2f);
			float num13 = num12 * Mathf.Clamp(Tuning.F("shadowstep.top", 1f), 0.5f, 1.6f);
			_reach[0] = 0.44f * Tall / v.Y;
			_reach[1] = 0.185f * num12;
			_reach[2] = 0.235f * num12;
			_reach[3] = 0.25f * num12;
			_reach[4] = 0.25f * num12;
			_reach[5] = 0.255f * num13;
			_reach[6] = 0.27f * num13;
			_reach[7] = 0.295f * num13;
			_reach[8] = 0.315f * num13;
			_reach[9] = 0.325f * num13;
			_land = Mathf.Clamp(Tuning.F("shadowstep.land", 0.15f), 0.04f, 0.4f) * num12;
			float X(float share)
			{
				if (!(dir > 0f))
				{
					return v.X * (1f - share);
				}
				return v.X * share;
			}
		}

		private static float Key(float[] xs, float[] ys, float x)
		{
			if (x <= xs[0])
			{
				return ys[0];
			}
			for (int i = 1; i < xs.Length; i++)
			{
				if (!(x > xs[i]))
				{
					float num = (x - xs[i - 1]) / Mathf.Max(1E-05f, xs[i] - xs[i - 1]);
					num = num * num * (3f - 2f * num);
					return Mathf.Lerp(ys[i - 1], ys[i], num);
				}
			}
			return ys[^1];
		}

		public Vector2 In(float along)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			return FxKit.Bezier(Feet, _bend, Start, Mathf.Clamp(along, 0f, 1f));
		}

		public Vector2 Loop(float s)
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Clamp(s, 0f, 1f) * _total;
			int num2 = 0;
			int num3 = _loop.Length - 1;
			while (num3 - num2 > 1)
			{
				int num4 = (num2 + num3) / 2;
				if (_length[num4] <= num)
				{
					num2 = num4;
				}
				else
				{
					num3 = num4;
				}
			}
			float num5 = (num - _length[num2]) / Mathf.Max(0.0001f, _length[num3] - _length[num2]);
			return _loop[num2].Lerp(_loop[num3], Mathf.Clamp(num5, 0f, 1f));
		}

		public float Along(float u)
		{
			u = Mathf.Clamp(u, 0f, 1f);
			int num = 0;
			int num2 = 240;
			while (num2 - num > 1)
			{
				int num3 = (num + num2) / 2;
				if (_uOfS[num3] <= u)
				{
					num = num3;
				}
				else
				{
					num2 = num3;
				}
			}
			float num4 = (u - _uOfS[num]) / Mathf.Max(1E-06f, _uOfS[num2] - _uOfS[num]);
			return ((float)num + Mathf.Clamp(num4, 0f, 1f)) / 240f;
		}

		public float TimeAt(float s)
		{
			float num = Mathf.Clamp(s, 0f, 1f) * 240f;
			int num2 = Mathf.Clamp((int)num, 0, 239);
			return Mathf.Lerp(_uOfS[num2], _uOfS[num2 + 1], Mathf.Clamp(num - (float)num2, 0f, 1f));
		}

		public float StopAlong(int stop)
		{
			return _stopS[Mathf.Clamp(stop, 0, 9)];
		}

		public float StopTime(int stop)
		{
			return _stopU[Mathf.Clamp(stop, 0, 9)];
		}

		public float Reach(float s)
		{
			return Key(_stopS, _reach, Mathf.Clamp(s, 0f, 1f)) * View.Y;
		}

		public float ClumpReach(float along)
		{
			float num = Mathf.Clamp(along, 0f, 1f);
			return Mathf.Lerp(40f, _reach[0] * View.Y * 0.82f, num * num * (3f - 2f * num));
		}

		private static float Smooth(float x)
		{
			x = Mathf.Clamp(x, 0f, 1f);
			return x * x * (3f - 2f * x);
		}

		public void LayStream(ShadowCloud cloud, float clock, float reach, float from, float seen)
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			if (cloud == null || reach <= 0.02f || seen <= 0.01f || from >= 0.999f)
			{
				return;
			}
			float num = Mathf.PosMod(clock * 1.25f, 1f);
			for (int i = 0; i < 7; i++)
			{
				float num2 = ((float)i + num) / 7f;
				if (!(num2 < from))
				{
					float num3 = num2 * reach;
					float thick = 0.95f * Smooth(num2 / 0.18f) * Smooth((num2 - from) / 0.12f + 0.35f);
					Vector2 at = In(num3) + new Vector2(0f, 26f * (1f - num3) + 5f * Mathf.Sin(clock * 3.1f + (float)i * 1.9f));
					cloud.Add(at, Mathf.Lerp(40f, 76f, num2) * (0.65f + 0.35f * reach), thick, 0.22f, seen);
				}
			}
		}

		public static void LayBody(ShadowCloud cloud, Vector2 at, float reach, float turn, float clock, float thick, float seen, Vector2 heading, float lobe)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			if (cloud != null && !(seen <= 0.003f))
			{
				cloud.Add(at, reach * 0.86f, thick, 0f, seen);
				if (lobe > 0.01f)
				{
					cloud.Add(at - heading * (reach * 0.42f), reach * 0.74f, 0.95f * thick * Mathf.Clamp(lobe, 0f, 1f), 0f, seen);
				}
				for (int i = 0; i < 3; i++)
				{
					float num = turn * 0.8f + (float)i * 2.094f + 0.4f * Mathf.Sin(clock * 2.3f + (float)i * 1.7f);
					cloud.Add(at + new Vector2(Mathf.Cos(num), Mathf.Sin(num)) * (reach * 0.5f), reach * 0.52f, 0.9f * thick, 0f, seen);
				}
			}
		}
	}
