// namespace: SilentFX.Cards
// type: PinpointFocus (class)
// source lines: 82915-83231
	internal static class PinpointFocus
	{
		private sealed class Click
		{
			public Node2D Mark;

			public ShaderMaterial Mat;

			public float Focus = 1f;
		}

		public const int Notches = 3;

		private const string Key = "pinpoint_click";

		private const float ArtY = -70f;

		private const float ClickTime = 0.46f;

		private static readonly StringName PFocus = StringName.op_Implicit("focus");

		private static readonly StringName PAlpha = StringName.op_Implicit("alpha");

		private static readonly StringName PSplit = StringName.op_Implicit("split");

		private static readonly StringName PLock = StringName.op_Implicit("lock");

		private static readonly StringName PShut = StringName.op_Implicit("shut");

		public static int Struck;

		private static readonly List<HandCards.Entry> Hand = new List<HandCards.Entry>();

		private static Player _owner;

		private static double _wait;

		private static bool _ticking;

		public static Node2D Reticle(out ShaderMaterial mat)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected Obj, but got Unknown
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			mat = null;
			Node2D val = null;
			try
			{
				ShaderMaterial val2 = ShaderLib.NewMaterial("pinpoint_reticle");
				if (val2 == null)
				{
					return null;
				}
				val = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_PinpointReticle")
				};
				ColorRect val3 = FxKit.Quad(new Vector2(100f, 100f), (Material)(object)val2);
				val3.Color = new Color(1f, 1f, 1f, 0f);
				((Control)val3).Position = new Vector2(-50f, -50f);
				((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
				val2.SetShaderParameter(PAlpha, Variant.op_Implicit(0f));
				mat = val2;
				return val;
			}
			catch (Exception ex)
			{
				DevLog.Warn("PinpointFocus.Reticle: " + ex.Message);
				if (val != null && FxKit.Alive((GodotObject)(object)val))
				{
					((Node)val).QueueFree();
				}
				mat = null;
				return null;
			}
		}

		public static void Show(ShaderMaterial mat, float focus, float alpha, float flash, float shut = 0f)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			if (mat != null && FxKit.Alive((GodotObject)(object)mat))
			{
				mat.SetShaderParameter(PShut, Variant.op_Implicit(Mathf.Clamp(shut, -0.2f, 1f)));
				float num = Mathf.Clamp(focus, 0f, 1f);
				mat.SetShaderParameter(PFocus, Variant.op_Implicit(num));
				mat.SetShaderParameter(PAlpha, Variant.op_Implicit(Mathf.Clamp(alpha, 0f, 1f)));
				mat.SetShaderParameter(PSplit, Variant.op_Implicit(0.075f * Mathf.Pow(1f - num, 1.5f)));
				mat.SetShaderParameter(PLock, Variant.op_Implicit(Mathf.Clamp(flash, 0f, 1f)));
			}
		}

		public static void Read(CardModel card, out int cut, out int whole)
		{
			whole = 3;
			cut = Steps(card);
			try
			{
				if (((card != null) ? card.Owner : null) == null)
				{
					return;
				}
				CardEnergyCost energyCost = card.EnergyCost;
				if (energyCost != null)
				{
					int canonical = energyCost.Canonical;
					if (canonical <= 0)
					{
						whole = 1;
						cut = 1;
					}
					else
					{
						whole = canonical;
						cut = canonical - Mathf.Clamp(energyCost.GetResolved(), 0, canonical);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PinpointFocus.Read: " + ex.Message);
			}
		}

		public static int Steps(CardModel card)
		{
			try
			{
				if (((card != null) ? card.Owner : null) == null)
				{
					return Mathf.Clamp(Tuning.I("pinpoint.preview_steps", 2), 0, 3);
				}
				return Mathf.Clamp(TurnLog.Count(card.Owner, (CardType)2), 0, 3);
			}
			catch (Exception ex)
			{
				DevLog.Warn("PinpointFocus.Steps: " + ex.Message);
				return 0;
			}
		}

		public static float Share(float from, float to, float focus)
		{
			return from * Mathf.Pow(Mathf.Max(0.0001f, to / Mathf.Max(0.0001f, from)), Mathf.Clamp(focus, 0f, 1f));
		}

		public static void NoteCardFinished(CardModel card)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Invalid comparison between Unknown and I4
			try
			{
				if (card != null && (int)card.Type == 2 && card.Owner != null && (!MultiplayerFx.Active || MultiplayerFx.IsLocal(card)) && !(Tuning.F("pinpoint.click", 1f) < 0.5f))
				{
					_owner = card.Owner;
					_wait = 0.06;
					if (!_ticking)
					{
						_ticking = true;
						Ticker.Add(Step);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PinpointFocus.NoteCardFinished: " + ex.Message);
			}
		}

		private static void Step(double dt)
		{
			_wait -= dt;
			if (_wait > 0.0)
			{
				return;
			}
			Ticker.Remove(Step);
			_ticking = false;
			Player owner = _owner;
			_owner = null;
			try
			{
				ClickAll(owner);
			}
			catch (Exception ex)
			{
				DevLog.Warn("PinpointFocus.Click: " + ex.Message);
			}
		}

		private static void ClickAll(Player owner)
		{
			if (owner == null)
			{
				return;
			}
			int num = TurnLog.Count(owner, (CardType)2);
			if (num < 1)
			{
				return;
			}
			HandCards.Fill(Hand);
			int num2 = 0;
			foreach (HandCards.Entry item in Hand)
			{
				CardModel card = item.Card;
				if (!(card is Pinpoint) || card.Owner != owner)
				{
					continue;
				}
				Read(card, out var cut, out var whole);
				if (num <= whole)
				{
					cut = Mathf.Clamp(Mathf.Max(cut, num), 1, whole);
					float focus = (float)cut / (float)whole;
					CardDeco.Remove(card, "pinpoint_click", 0f);
					CardDeco.Attach(card, "pinpoint_click", (CardDeco.Handle h) =>
					{
						Build(h, focus);
					}, Tick);
					num2++;
				}
			}
			Hand.Clear();
			if (num2 > 0 && PowerFxManager.LogTriggers)
			{
				DevLog.Info($"Pinpoint: a Skill was played ({num} this turn) - the reticle shows on {num2} card(s) in the hand");
			}
		}

		private static void Build(CardDeco.Handle h, float focus)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (h != null && FxKit.Alive((GodotObject)(object)h.Root))
				{
					Node2D val = Reticle(out var mat);
					if (val != null)
					{
						val.Position = new Vector2(0f, -70f);
						((Node)h.Root).AddChild((Node)(object)val, false, (InternalMode)0);
						h.State = new Click
						{
							Mark = val,
							Mat = mat,
							Focus = Mathf.Clamp(focus, 0f, 1f)
						};
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PinpointFocus.Build: " + ex.Message);
			}
		}

		private static void Tick(CardDeco.Handle h, float dt)
		{
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!(h?.State is Click click) || !FxKit.Alive((GodotObject)(object)click.Mark))
				{
					h?.Remove(0f);
					return;
				}
				float time = h.Time;
				if (time >= 0.46f)
				{
					h.Remove(0f);
					return;
				}
				float num = Mathf.Clamp(time / 0.1f, 0f, 1f);
				float num2 = 1f - (1f - num) * (1f - num) * (1f - num);
				float num3 = Mathf.Clamp((time - 0.22f) / 0.24000001f, 0f, 1f);
				float num4 = (1f - num3) * (1f - num3);
				float num5 = Share(210f, 120f, click.Focus) * Mathf.Lerp(1.22f, 1f, num2);
				click.Mark.Scale = new Vector2(num5, num5) / 100f;
				float flash = ((click.Focus > 0.999f) ? (Mathf.Exp((0f - Mathf.Max(0f, time - 0.08f)) * 16f) * Mathf.Min(1f, time / 0.08f)) : 0f);
				Show(click.Mat, click.Focus, Mathf.Min(1f, time / 0.04f) * num4, flash);
			}
			catch (Exception ex)
			{
				DevLog.Warn("PinpointFocus.Tick: " + ex.Message);
				h?.Remove(0f);
			}
		}

		public static void Reset()
		{
			_owner = null;
			_wait = 0.0;
			if (_ticking)
			{
				Ticker.Remove(Step);
			}
			_ticking = false;
			Hand.Clear();
			try
			{
				CardDeco.RemoveAll("pinpoint_click", 0f);
			}
			catch (Exception ex)
			{
				DevLog.Warn("PinpointFocus.Reset: " + ex.Message);
			}
		}
	}
