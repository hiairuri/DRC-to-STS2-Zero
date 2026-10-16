// namespace: SilentFX.Cards
// type: PreciseCutChains (class)
// source lines: 84952-85567
	public static class PreciseCutChains
	{
		private sealed class State
		{
			public ColorRect Rect;

			public ShaderMaterial Mat;

			public float Out;

			public float Speed;

			public float Want;

			public float GlintAt = -99f;

			public float Ready;

			public float ReadyFor;

			public float Flash = 99f;

			public float Step = 99f;

			public float Aim = 99f;

			public bool WasReady;

			public bool Started;
		}

		private struct Mark
		{
			public ulong Until;

			public float Drawn;
		}

		private const string Key = "precise";

		private const float HalfW = 150f;

		private const float HalfH = 211f;

		private const float Pad = 60f;

		private const float Omega = 24f;

		private const float Zeta = 0.5f;

		private const float GlintWait = 0.05f;

		private const float GlintRun = 0.26f;

		private const float GleamRun = 0.62f;

		private const float StarTime = 0.36f;

		private static readonly StringName PSize = StringName.op_Implicit("size");

		private static readonly StringName PCard = StringName.op_Implicit("card");

		private static readonly StringName PPad = StringName.op_Implicit("pad");

		private static readonly StringName PFrom = StringName.op_Implicit("from_x");

		private static readonly StringName PSide = StringName.op_Implicit("side_len");

		private static readonly StringName PDepth = StringName.op_Implicit("depth");

		private static readonly StringName PSeed = StringName.op_Implicit("seed");

		private static readonly StringName PBare = StringName.op_Implicit("bare");

		private static readonly StringName PHot = StringName.op_Implicit("hot");

		private static readonly StringName PSpark = StringName.op_Implicit("spark_age");

		private static readonly StringName PGlint = StringName.op_Implicit("glint");

		private static readonly StringName PStar = StringName.op_Implicit("star");

		private static readonly StringName PReady = StringName.op_Implicit("ready");

		private static readonly StringName PLit = StringName.op_Implicit("lit");

		private static bool _warnedShader;

		private static readonly List<HandCards.Entry> Hand = new List<HandCards.Entry>();

		private static readonly Dictionary<CardModel, Mark> Marks = new Dictionary<CardModel, Mark>(ReferenceEqualityComparer.Instance);

		private static readonly List<CardModel> Expired = new List<CardModel>();

		private static bool _ready;

		private static bool _off;

		private static int _failures;

		private static bool _pileFailed;

		private static bool _damageFailed;

		private static double _scanIn;

		public static void Init()
		{
			if (!_ready)
			{
				_ready = true;
				Ticker.Add(Update);
			}
		}

		public static void Reset()
		{
			try
			{
				CardDeco.RemoveAll("precise", 0f);
			}
			catch (Exception ex)
			{
				DevLog.Warn("PreciseCutChains.Reset: " + ex);
			}
			try
			{
				PreciseCutFx.Reset();
			}
			catch (Exception ex2)
			{
				DevLog.Warn("PreciseCutFx.Reset: " + ex2);
			}
			Marks.Clear();
			Hand.Clear();
			_scanIn = 0.0;
			_failures = 0;
			_off = false;
			_pileFailed = false;
			_damageFailed = false;
		}

		public static float Drawn(CardModel card)
		{
			try
			{
				if (card == null)
				{
					return 1f;
				}
				if (TryShare(card, out var share))
				{
					return share;
				}
				if (MultiplayerFx.Active && !MultiplayerFx.IsLocal(card))
				{
					return 1f;
				}
				if (Marks.TryGetValue(card, out var value) && Time.GetTicksMsec() < value.Until)
				{
					return value.Drawn;
				}
				if (CardDeco.Find(card, "precise")?.State is State { Started: not false } state)
				{
					return Mathf.Clamp(state.Want, 0f, 1f);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PreciseCutChains.Drawn: " + ex.Message);
			}
			return 1f;
		}

		private static void Fail(string where, Exception e)
		{
			DevLog.Warn("PreciseCutChains." + where + ": " + e);
			if (!_off && ++_failures >= 4)
			{
				_off = true;
				try
				{
					CardDeco.RemoveAll("precise", 0f);
				}
				catch (Exception ex)
				{
					DevLog.Warn("PreciseCutChains (switching off): " + ex.Message);
				}
				DevLog.Warn("PreciseCutChains: switched off for this fight after repeated failures");
			}
		}

		private static void Update(double realDt)
		{
			if (_off)
			{
				return;
			}
			_scanIn -= realDt;
			if (_scanIn > 0.0)
			{
				return;
			}
			_scanIn = 0.1;
			try
			{
				HandCards.Fill(Hand);
				foreach (HandCards.Entry item in Hand)
				{
					if (item.Card is PreciseCut)
					{
						CardDeco.Attach(item.Card, "precise", Build, Tick, Left);
					}
				}
				if (Marks.Count == 0)
				{
					return;
				}
				ulong ticksMsec = Time.GetTicksMsec();
				Expired.Clear();
				foreach (KeyValuePair<CardModel, Mark> mark in Marks)
				{
					if (ticksMsec >= mark.Value.Until)
					{
						Expired.Add(mark.Key);
					}
				}
				foreach (CardModel item2 in Expired)
				{
					Marks.Remove(item2);
				}
				Expired.Clear();
			}
			catch (Exception e)
			{
				Fail("Update", e);
			}
		}

		private static int Others(CardModel card)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Invalid comparison between Unknown and I4
			if (!_pileFailed)
			{
				try
				{
					Player owner = card.Owner;
					CardPile val = ((owner != null) ? PileTypeExtensions.GetPile((PileType)2, owner) : null);
					if (((val != null) ? val.Cards : null) != null)
					{
						int count = val.Cards.Count;
						CardPile pile = card.Pile;
						return count - ((pile != null && (int)pile.Type == 2) ? 1 : 0);
					}
				}
				catch (Exception ex)
				{
					_pileFailed = true;
					DevLog.Warn("PreciseCutChains: cannot read the hand pile (" + ex.Message + ")");
				}
			}
			return Hand.Count - 1;
		}

		private static bool TryShare(CardModel card, out float share)
		{
			share = 1f;
			bool flag = MultiplayerFx.Active && !MultiplayerFx.IsLocal(card);
			if (_damageFailed && !flag)
			{
				return false;
			}
			try
			{
				DynamicVarSet dynamicVars = card.DynamicVars;
				decimal baseValue = ((DynamicVar)dynamicVars.CalculationBase).BaseValue;
				if (baseValue <= 0m)
				{
					return false;
				}
				decimal num = ((CalculatedVar)dynamicVars.CalculatedDamage).Calculate((Creature)null);
				share = Mathf.Clamp((float)(num / baseValue), 0f, 1f);
				return true;
			}
			catch (Exception ex)
			{
				if (!flag)
				{
					_damageFailed = true;
				}
				DevLog.Warn("PreciseCutChains: cannot read the damage (" + ex.Message + ")");
			}
			return false;
		}

		private static float Share(CardModel card)
		{
			if (TryShare(card, out var share))
			{
				return share;
			}
			return Mathf.Clamp(1f - (float)Mathf.Max(0, Others(card)) / 9f, 0f, 1f);
		}

		private static void Build(CardDeco.Handle h)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (h == null || !FxKit.Alive((GodotObject)(object)h.Root))
				{
					return;
				}
				ShaderMaterial val = ShaderLib.NewMaterial("card_edge");
				if (val == null)
				{
					if (!_warnedShader)
					{
						DevLog.Warn("Precise Cut: shaders/card_edge.gdshader is missing - the card's edge is not drawn");
					}
					_warnedShader = true;
					return;
				}
				State state = new State
				{
					Mat = val
				};
				Vector2 size = new Vector2(420f, 542f);
				state.Rect = FxKit.Quad(size, (Material)(object)val);
				((Node)state.Rect).Name = StringName.op_Implicit("SilentFX_CardEdge");
				((Control)state.Rect).Position = new Vector2(-210f, -271f);
				((Node)h.Root).AddChild((Node)(object)state.Rect, false, (InternalMode)0);
				Lay(val, size);
				if (h.Card != null)
				{
					state.Want = Share(h.Card);
					state.Out = state.Want;
					state.Started = true;
					state.WasReady = state.Want >= 0.999f;
					state.Ready = (state.WasReady ? 1f : 0f);
					state.ReadyFor = Every() - 0.9f;
				}
				Pose(state, 0f);
				h.State = state;
			}
			catch (Exception e)
			{
				Fail("Build", e);
			}
		}

		private static void Lay(ShaderMaterial mat, Vector2 size)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			mat.SetShaderParameter(PSize, Variant.op_Implicit(size));
			mat.SetShaderParameter(PCard, Variant.op_Implicit(new Vector2(300f, 422f)));
			mat.SetShaderParameter(PPad, Variant.op_Implicit(60f));
			mat.SetShaderParameter(PFrom, Variant.op_Implicit(Mathf.Clamp(Tuning.F("precise.edge_from", 44f), 0f, 200f)));
			mat.SetShaderParameter(PSide, Variant.op_Implicit(Mathf.Clamp(Tuning.F("precise.edge_side", 230f), 0f, 400f)));
			mat.SetShaderParameter(PDepth, Variant.op_Implicit(Mathf.Clamp(Tuning.F("precise.edge_depth", 13f), 3f, 30f)));
			mat.SetShaderParameter(PSeed, Variant.op_Implicit((float)GD.RandRange(0.0, 50.0)));
		}

		internal static ColorRect EdgeCopy(CardModel card)
		{
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				bool flag = MultiplayerFx.Active && !MultiplayerFx.IsLocal(card);
				if ((!flag && _off) || card == null)
				{
					return null;
				}
				float num;
				if (flag)
				{
					num = Drawn(card);
				}
				else
				{
					num = ((CardDeco.Find(card, "precise")?.State is State { Started: not false } state) ? Mathf.Clamp(state.Out, 0f, 1f) : Share(card));
				}
				if (num <= 0.004f)
				{
					return null;
				}
				ShaderMaterial val = ShaderLib.NewMaterial("card_edge");
				if (val == null)
				{
					return null;
				}
				Vector2 size = new Vector2(420f, 542f);
				ColorRect val2 = FxKit.Quad(size, (Material)(object)val);
				((Node)val2).Name = StringName.op_Implicit("SilentFX_CardEdgeCopy");
				((Control)val2).Position = new Vector2(-210f, -271f);
				Lay(val, size);
				val.SetShaderParameter(PBare, Variant.op_Implicit(num));
				val.SetShaderParameter(PReady, Variant.op_Implicit((num >= 0.999f) ? 1f : 0f));
				val.SetShaderParameter(PLit, Variant.op_Implicit(Mathf.Clamp(Tuning.F("precise.gleam", 1f), 0f, 1f)));
				val.SetShaderParameter(PHot, Variant.op_Implicit(0f));
				val.SetShaderParameter(PSpark, Variant.op_Implicit(9f));
				val.SetShaderParameter(PGlint, Variant.op_Implicit(-1f));
				val.SetShaderParameter(PStar, Variant.op_Implicit(0f));
				return val2;
			}
			catch (Exception ex)
			{
				DevLog.Warn("PreciseCutChains.EdgeCopy: " + ex.Message);
				return null;
			}
		}

		internal static void Twinkle(CardModel card)
		{
			try
			{
				if (!_off && card != null && CardDeco.Find(card, "precise")?.State is State state)
				{
					state.Aim = 0f;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PreciseCutChains.Twinkle: " + ex.Message);
			}
		}

		private static float Every()
		{
			return Mathf.Max(1.28f, Tuning.F("precise.gleam_every", 2.2f));
		}

		private static void Tick(CardDeco.Handle h, float dt)
		{
			try
			{
				if (_off || !(h?.State is State state) || !FxKit.Alive((GodotObject)(object)h.Root) || !FxKit.Alive((GodotObject)(object)state.Rect) || !FxKit.Alive((GodotObject)(object)state.Mat))
				{
					return;
				}
				CardModel card = h.Card;
				if (card == null)
				{
					return;
				}
				float num = Share(card);
				if (!state.Started)
				{
					state.Started = true;
					state.Want = num;
					state.Out = num;
				}
				if (Mathf.Abs(num - state.Want) > 0.004f)
				{
					if (num > state.Want)
					{
						state.GlintAt = -0.05f;
					}
					if (num > state.Want)
					{
						state.Step = 0f;
					}
					state.Want = num;
				}
				int num2 = Mathf.Clamp(Mathf.CeilToInt(dt / 0.008f), 1, 8);
				float num3 = dt / (float)num2;
				for (int i = 0; i < num2; i++)
				{
					float num4 = 576f * (state.Want - state.Out) - 24f * state.Speed;
					state.Speed += num4 * num3;
					state.Out += state.Speed * num3;
				}
				if (!float.IsFinite(state.Out) || !float.IsFinite(state.Speed))
				{
					state.Out = state.Want;
					state.Speed = 0f;
				}
				if (state.Out < 0f)
				{
					state.Out = 0f;
					if (state.Speed < 0f)
					{
						state.Speed = 0f;
					}
				}
				else if (state.Out > 1.08f)
				{
					state.Out = 1.08f;
					if (state.Speed > 0f)
					{
						state.Speed = 0f;
					}
				}
				bool flag = state.Want >= 0.999f && state.Out >= 0.96f;
				if (flag && !state.WasReady)
				{
					state.ReadyFor = 0f;
					state.Flash = 0f;
				}
				state.WasReady = flag;
				state.ReadyFor += dt;
				state.Flash += dt;
				state.Step += dt;
				state.Aim += dt;
				state.Ready = FxKit.Damp(state.Ready, flag ? 1f : 0f, flag ? 9f : 16f, dt);
				if (state.GlintAt > -90f)
				{
					state.GlintAt += dt;
				}
				Pose(state, h.Time);
			}
			catch (Exception e)
			{
				Fail("Tick", e);
			}
		}

		private static void Pose(State s, float time)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			ShaderMaterial mat = s.Mat;
			float num = Mathf.Clamp(Tuning.F("precise.gleam", 1f), 0f, 2f);
			mat.SetShaderParameter(PBare, Variant.op_Implicit(Mathf.Max(0f, s.Out)));
			mat.SetShaderParameter(PReady, Variant.op_Implicit(Mathf.Clamp(s.Ready, 0f, 1f)));
			mat.SetShaderParameter(PLit, Variant.op_Implicit(Mathf.Min(1f, num)));
			mat.SetShaderParameter(PHot, Variant.op_Implicit((s.Step < 0.7f) ? Mathf.Exp((0f - s.Step) / 0.13f) : 0f));
			mat.SetShaderParameter(PSpark, Variant.op_Implicit(s.Step));
			float num2 = -1f;
			if (s.GlintAt > -90f)
			{
				float num3 = s.GlintAt / 0.26f;
				if (num3 >= 1f)
				{
					s.GlintAt = -99f;
				}
				else if (num3 >= 0f)
				{
					num2 = num3 * num3 * (3f - 2f * num3);
				}
			}
			float num4 = Every();
			float num5 = s.ReadyFor - Mathf.Floor(s.ReadyFor / num4) * num4;
			bool flag = s.WasReady && num > 0.01f;
			float num6 = num5 / 0.62f;
			if (((num2 < 0f) & flag) && num6 < 1f)
			{
				num2 = num6 * num6 * (3f - 2f * num6);
			}
			mat.SetShaderParameter(PGlint, Variant.op_Implicit(num2));
			float num7 = 0f;
			float num8 = (num5 - 0.55799997f) / 0.36f;
			if (flag && num8 >= 0f && num8 < 1f)
			{
				num7 = Mathf.Sin((float)Math.PI * num8);
			}
			float num9 = s.Flash / 0.36f;
			if (flag && num9 < 1f)
			{
				num7 = Mathf.Max(num7, Mathf.Sin((float)Math.PI * Mathf.Clamp(num9, 0f, 1f)));
			}
			float num10 = s.Aim / 0.36f;
			if (num10 < 1f)
			{
				num7 = Mathf.Max(num7, 0.8f * Mathf.Sin((float)Math.PI * Mathf.Clamp(num10, 0f, 1f)));
			}
			mat.SetShaderParameter(PStar, Variant.op_Implicit(num7));
		}

		private static void Left(CardDeco.Handle h)
		{
			try
			{
				if (!_off && h?.State is State state && h.Card != null)
				{
					float drawn = (TryShare(h.Card, out var share) ? share : Mathf.Clamp(state.Want, 0f, 1f));
					Marks[h.Card] = new Mark
					{
						Until = Time.GetTicksMsec() + 3000,
						Drawn = drawn
					};
				}
			}
			catch (Exception e)
			{
				Fail("Left", e);
			}
		}
	}
