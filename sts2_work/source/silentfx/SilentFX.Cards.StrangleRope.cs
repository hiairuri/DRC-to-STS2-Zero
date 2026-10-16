// namespace: SilentFX.Cards
// type: StrangleRope (class)
// source lines: 106792-108220
	public static class StrangleRope
	{
		private sealed class Strand
		{
			private readonly Line2D _glow;

			private readonly Line2D _body;

			private readonly Line2D _core;

			private readonly float _width;

			private readonly Gradient[] _fades;

			public Strand(Node parent, float width, bool end, bool cover)
			{
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				_width = width;
				if (cover)
				{
					_fades = new Gradient[3];
				}
				_glow = Layer(parent, new Color(Hot.R, Hot.G, Hot.B, 0.26f), additive: true, end, cover, 0);
				_body = Layer(parent, Red, additive: false, end, cover, 1);
				_core = Layer(parent, new Color(1f, 0.6f, 0.54f, 0.8f), additive: true, end, cover, 2);
			}

			private Line2D Layer(Node parent, Color color, bool additive, bool end, bool cover, int index)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_002c: Expected Obj, but got Unknown
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Expected Obj, but got Unknown
				//IL_004f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Expected Obj, but got Unknown
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0103: Unknown result type (might be due to invalid IL or missing references)
				//IL_010a: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0112: Unknown result type (might be due to invalid IL or missing references)
				//IL_0113: Unknown result type (might be due to invalid IL or missing references)
				//IL_011a: Unknown result type (might be due to invalid IL or missing references)
				//IL_011b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0122: Unknown result type (might be due to invalid IL or missing references)
				//IL_0123: Unknown result type (might be due to invalid IL or missing references)
				//IL_012a: Unknown result type (might be due to invalid IL or missing references)
				//IL_012b: Unknown result type (might be due to invalid IL or missing references)
				Line2D val = new Line2D
				{
					DefaultColor = color,
					JointMode = (LineJointMode)2,
					BeginCapMode = (LineCapMode)2,
					EndCapMode = (LineCapMode)2,
					Antialiased = true
				};
				if (additive)
				{
					((CanvasItem)val).Material = (Material)(object)FxKit.Additive;
				}
				if (end)
				{
					Curve val2 = new Curve();
					val2.AddPoint(new Vector2(0f, 1f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					val2.AddPoint(new Vector2(0.7f, 0.84f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					val2.AddPoint(new Vector2(1f, 0.1f), 0f, 0f, (TangentMode)0, (TangentMode)0);
					val.WidthCurve = val2;
				}
				if (cover)
				{
					Color val3 = new Color(color.R, color.G, color.B, 0f);
					Gradient val4 = new Gradient();
					val4.Offsets = new float[6] { 0f, 0.001f, 0.002f, 0.003f, 0.004f, 1f };
					val4.Colors = new Color[6] { color, color, val3, val3, color, color };
					val.Gradient = val4;
					_fades[index] = val4;
				}
				parent.AddChild((Node)(object)val, false, (InternalMode)0);
				return val;
			}

			public void Cover(float lo, float hi)
			{
				if (_fades == null)
				{
					return;
				}
				float num = 0.05f;
				if (lo > hi)
				{
					lo = 0.002f;
					hi = 0.003f;
					num = 0f;
				}
				float num2 = Mathf.Clamp(lo - num, 0.001f, 0.99f);
				float num3 = Mathf.Clamp(lo, num2 + 0.001f, 0.992f);
				float num4 = Mathf.Clamp(hi, num3 + 0.001f, 0.994f);
				float num5 = Mathf.Clamp(hi + num, num4 + 0.001f, 0.996f);
				float[] offsets = new float[6] { 0f, num2, num3, num4, num5, 1f };
				Gradient[] fades = _fades;
				foreach (Gradient val in fades)
				{
					if (FxKit.Alive((GodotObject)(object)val))
					{
						val.Offsets = offsets;
					}
				}
			}

			public void Set(Vector2[] points, float alpha, float fat, float hot)
			{
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_011c: Unknown result type (might be due to invalid IL or missing references)
				//IL_015e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
				bool show = alpha > 0.01f && points != null && points.Length >= 2;
				float num = Mathf.Clamp(hot, 0f, 1f);
				Put(_glow, points, show, _width * 2.7f * fat, new Color(1f, 1f, 1f, Mathf.Min(1f, alpha) * (1f + 1.2f * num)));
				if (_fades == null && FxKit.Alive((GodotObject)(object)_body))
				{
					float num2 = 0.7f * num;
					_body.DefaultColor = new Color(Mathf.Lerp(Red.R, White.R, num2), Mathf.Lerp(Red.G, White.G, num2), Mathf.Lerp(Red.B, White.B, num2), 1f);
				}
				Put(_body, points, show, _width * fat, new Color(1f, 1f, 1f, Mathf.Min(1f, alpha)));
				Put(_core, points, show, _width * 0.3f * fat, new Color(1f, 1f, 1f, Mathf.Min(1f, alpha) * (1f + num)));
			}

			private static void Put(Line2D line, Vector2[] points, bool show, float width, Color tint)
			{
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)line))
				{
					((CanvasItem)line).Visible = show;
					if (show)
					{
						line.Points = points;
						line.Width = width;
						((CanvasItem)line).Modulate = tint;
					}
				}
			}
		}

		private sealed class Rope
		{
			public Node2D Front;

			public Node2D Behind;

			public CardModel Card;

			public Vector2 At;

			public float Time;

			public float Tight;

			public float Grip;

			public float SnapAt;

			public bool Snapped;

			public bool Done;

			public Creature Who;

			public Node2D Body;

			public Vector2 Home;

			public Vector2 Wrote;

			public Vector2 Factor = Vector2.One;

			public bool Waits;

			public bool Asked;

			public bool Going;

			public float Shown;

			public Action<float> Step;

			public Action<CardModel, bool> Pull;

			public bool Owned;

			public bool Closed;

			public bool Pending;

			public Node Scene;

			public object Invocation;

			public CardModel Source;

			public readonly HashSet<object> Requests = new HashSet<object>(ReferenceEqualityComparer.Instance);

			public Rope()
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			}
		}

		private static readonly Color Red = new Color(0.88f, 0.05f, 0.09f, 1f);

		private static readonly Color Hot = new Color(1f, 0.2f, 0.16f, 1f);

		private static readonly Color White = new Color(1f, 0.9f, 0.86f, 1f);

		private const int QuarterPoints = 12;

		private const int BackPoints = 10;

		private const int RunPoints = 24;

		private static Rope _pending;

		private static readonly List<Rope> Live = new List<Rope>();

		private static readonly List<Rope> Waiting = new List<Rope>();

		private static bool _ticking;

		private static bool _hangFailed;

		public static void Reset()
		{
			_pending = null;
			_hangFailed = false;
			Rope[] array = Live.ToArray();
			foreach (Rope rope in array)
			{
				try
				{
					Close(rope);
				}
				catch (Exception ex)
				{
					DevLog.Warn("Strangle reset: " + ex.Message);
				}
			}
			Live.Clear();
			array = Waiting.ToArray();
			foreach (Rope rope2 in array)
			{
				try
				{
					Free(rope2);
				}
				catch (Exception ex2)
				{
					DevLog.Warn("Strangle reset: " + ex2.Message);
				}
			}
			Waiting.Clear();
			_ticking = false;
		}

		public static void Play(Node back, Node front, Vector2 at, float width, float height)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Begin(back, front, at, width, height, null, wait: false, null);
		}

		public static void Play(Node back, Node front, Vector2 at, float width, float height, Creature who, CardModel card)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Begin(back, front, at, width, height, card, wait: false, who);
		}

		internal static bool SnapFor(CardModel card, Creature who = null)
		{
			Rope rope;
			if (MultiplayerFx.Active || MultiplayerFx.CurrentInvocation != null)
			{
				if (!MultiplayerFx.Active || MultiplayerFx.CurrentInvocation == null || MultiplayerFx.CurrentCard != card)
				{
					return false;
				}
				rope = Live.FindLast((Rope r) => r.Owned && r.Pending && !r.Closed && r.Card == card && r.Invocation == MultiplayerFx.CurrentInvocation && r.Who == who);
			}
			else
			{
				rope = _pending;
			}
			if (rope == null || rope.Card != card)
			{
				return false;
			}
			if (rope.Done || !FxKit.Alive((GodotObject)(object)rope.Front))
			{
				if (_pending == rope)
				{
					_pending = null;
				}
				return false;
			}
			if (!rope.Snapped)
			{
				rope.SnapAt = Mathf.Min(rope.SnapAt, Mathf.Max(rope.Time, rope.Tight + rope.Grip));
			}
			return true;
		}

		internal static void Hang(Node back, Node front, Vector2 at, float width, float height, Creature who, CardModel source = null, object subscriber = null)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			if (_hangFailed || !FxKit.Alive((GodotObject)(object)front) || !front.IsInsideTree() || (who != null && !who.IsAlive))
			{
				return;
			}
			try
			{
				if (MultiplayerFx.Active && (who == null || ((source != null) ? source.Owner : null) == null))
				{
					return;
				}
				Rope rope = Waiter(who, at, source, front);
				if (rope != null)
				{
					rope.Asked = true;
					rope.Going = false;
					if (rope.Owned)
					{
						rope.Requests.Add(subscriber ?? source);
					}
					return;
				}
				for (int num = Live.Count - 1; num >= 0; num--)
				{
					if (!On(Live[num], who, at))
					{
						continue;
					}
					bool num2;
					if (!MultiplayerFx.Active)
					{
						num2 = !Live[num].Owned;
					}
					else
					{
						if (!Live[num].Owned || Live[num].Source != source)
						{
							continue;
						}
						num2 = Live[num].Scene == front;
					}
					if (num2)
					{
						return;
					}
				}
				rope = Put(back, front, at, width, height, who, hang: true);
				if (rope != null && rope.Owned)
				{
					rope.Source = source;
					rope.Requests.Add(subscriber ?? source);
				}
			}
			catch (Exception ex)
			{
				_hangFailed = true;
				DevLog.Warn("Strangle (slack): " + ex.Message);
			}
		}

		internal static void ReleaseHang(Creature who, CardModel source, object subscriber)
		{
			if (subscriber == null)
			{
				return;
			}
			foreach (Rope item in Waiting)
			{
				if (item.Owned && item.Who == who && item.Source == source)
				{
					item.Requests.Remove(subscriber);
					item.Asked = item.Requests.Count != 0;
				}
			}
		}

		internal static bool Tighten(Creature who, Vector2 at, CardModel card, bool wait, Node scene = null)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			Rope rope = Waiter(who, at, card, scene);
			if (rope == null || rope.Pull == null || !FxKit.Alive((GodotObject)(object)rope.Front) || !((Node)rope.Front).IsInsideTree())
			{
				return false;
			}
			if (TimeFreeze.Active)
			{
				return false;
			}
			try
			{
				rope.Pull(card, wait);
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Strangle (hauled): " + ex.Message);
				Free(rope);
				return false;
			}
		}

		private static Rope Waiter(Creature who, Vector2 at, CardModel source = null, Node scene = null)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			for (int num = Waiting.Count - 1; num >= 0; num--)
			{
				Rope rope = Waiting[num];
				if (!rope.Waits || !On(rope, who, at))
				{
					continue;
				}
				bool num2;
				if (!MultiplayerFx.Active)
				{
					num2 = !rope.Owned;
				}
				else
				{
					if (!rope.Owned || rope.Source != source)
					{
						continue;
					}
					if (scene == null)
					{
						goto IL_0064;
					}
					num2 = rope.Scene == scene;
				}
				if (!num2)
				{
					continue;
				}
				goto IL_0064;
				IL_0064:
				return rope;
			}
			return null;
		}

		private static bool On(Rope rope, Creature who, Vector2 at)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (rope == null || rope.Done)
			{
				return false;
			}
			if (who != null && rope.Who != null)
			{
				return rope.Who == who;
			}
			return rope.At.DistanceSquaredTo(at) < 3600f;
		}

		private static void Idle(double realDt)
		{
			if (Waiting.Count == 0)
			{
				return;
			}
			float obj = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			for (int num = Waiting.Count - 1; num >= 0; num--)
			{
				if (num < Waiting.Count)
				{
					Rope rope = Waiting[num];
					try
					{
						if (!rope.Waits)
						{
							Waiting.RemoveAt(num);
						}
						else if (rope.Done || !FxKit.Alive((GodotObject)(object)rope.Front) || !((Node)rope.Front).IsInsideTree())
						{
							Free(rope);
						}
						else if (rope.Owned && !MultiplayerFx.Active)
						{
							Free(rope);
						}
						else
						{
							if (rope.Owned)
							{
								rope.Asked = rope.Requests.Count != 0;
							}
							if (!rope.Asked)
							{
								rope.Going = true;
							}
							rope.Asked = false;
							if (rope.Owned)
							{
								rope.Requests.Clear();
							}
							rope.Step?.Invoke(obj);
							if (rope.Going && rope.Shown <= 0f)
							{
								Free(rope);
							}
						}
					}
					catch (Exception ex)
					{
						DevLog.Warn("Strangle (slack): " + ex.Message);
						try
						{
							Free(rope);
						}
						catch
						{
						}
						Waiting.Remove(rope);
					}
				}
			}
		}

		internal static void Begin(Node back, Node front, Vector2 at, float width, float height, CardModel card, bool wait, Creature who)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Rope rope = Put(back, front, at, width, height, who, hang: false);
			if (rope == null)
			{
				return;
			}
			try
			{
				rope.Pull(card, wait);
			}
			catch
			{
				Free(rope);
				throw;
			}
		}

		private static Rope Put(Node back, Node front, Vector2 at, float width, float height, Creature who, bool hang)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0398: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0433: Unknown result type (might be due to invalid IL or missing references)
			//IL_0438: Unknown result type (might be due to invalid IL or missing references)
			//IL_044d: Expected Obj, but got Unknown
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Expected Obj, but got Unknown
			//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0642: Unknown result type (might be due to invalid IL or missing references)
			//IL_0647: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)front))
			{
				return null;
			}
			float slack = 0f;
			float haul = 0.15f;
			float grip = 0.1f;
			float outTime = 0.22f;
			float tightAt = 0.15f;
			float total = 1f;
			float yank = 0.05f;
			float over = 0f;
			float still = 0f;
			bool stilled = false;
			bool heavy = false;
			Creature creature = who;
			Node2D body = null;
			bool flag = false;
			try
			{
				NCreature val = Find(who, at);
				if (val != null)
				{
					if (creature == null)
					{
						creature = val.Entity;
					}
					Node2D body2 = val.Body;
					if (FxKit.Alive((GodotObject)(object)body2))
					{
						if (((CanvasItem)body2).Visible)
						{
							body = body2;
						}
						else
						{
							flag = true;
						}
					}
				}
			}
			catch
			{
				body = null;
			}
			float w = Mathf.Clamp(width, 110f, 900f);
			float num = Mathf.Clamp(height, 110f, 800f);
			float root = Mathf.Sqrt(Mathf.Clamp(w / 240f, 1f, 2.2f));
			float thick = Tuning.F("strangle.rope", 10f) * root;
			float num2 = Mathf.Clamp(Tuning.F("strangle.girth", 1f) * (flag ? Tuning.F("strangle.girth_bare", 1.1f) : 1f), 0.3f, 3f);
			float a0 = (w * Tuning.F("strangle.loose", 0.72f) + 30f) * num2;
			float a1 = Mathf.Min(w * Tuning.F("strangle.bite", 0.4f) * num2, a0 * 0.75f);
			float b0 = Mathf.Min(a0 * 0.36f, num * 0.42f);
			float b1 = Mathf.Min(a1 * 0.3f, num * 0.22f);
			float tail0 = Mathf.Min(a0 + w * 0.25f + 40f, w * 0.75f + 130f);
			float num3 = (Girth(a0, b0) - Girth(a1, b1)) * 0.5f * Mathf.Clamp(Tuning.F("strangle.give", 0.35f), 0f, 1f);
			float tail1 = Mathf.Max(tail0, Mathf.Min(tail0 + Mathf.Max(0f, num3), w * 0.8f + 170f));
			float recoil = Tuning.F("strangle.recoil", 240f) * root;
			float droop = num * 0.04f + 8f;
			float cover = w * Mathf.Clamp(Tuning.F("strangle.cover", 0.46f), 0.1f, 1f);
			float squeeze = ((body != null) ? Mathf.Clamp(Tuning.F("strangle.pinch", 0.07f), 0f, 0.2f) : 0f);
			if (squeeze <= 0.001f)
			{
				body = null;
			}
			Vector2 centre = at + new Vector2(0f, (0f - num) * Tuning.F("strangle.height", 0.05f));
			float slip = Mathf.Clamp(Tuning.F("strangle.slip", 0.12f), 0.03f, 1f);
			float sway = Mathf.Clamp(Tuning.F("strangle.sway", 1f), 0f, 3f);
			double phase = (double)at.X * 0.013;
			float fall = -1.5f;
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_StrangleRope")
			};
			if (hang)
			{
				TimeFreeze.KeepLive((Node)(object)holder);
			}
			front.AddChild((Node)(object)holder, false, (InternalMode)0);
			if (!((Node)holder).IsInsideTree())
			{
				((Node)holder).QueueFree();
				return null;
			}
			Node2D behind = null;
			if (FxKit.Alive((GodotObject)(object)back) && back != front)
			{
				behind = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_StrangleRopeFar")
				};
				if (hang)
				{
					TimeFreeze.KeepLive((Node)(object)behind);
				}
				back.AddChild((Node)(object)behind, false, (InternalMode)0);
				if (!((Node)behind).IsInsideTree())
				{
					((Node)behind).QueueFree();
					behind = null;
				}
			}
			Node2D farHome = behind ?? holder;
			bool seen = flag || behind == null;
			Strand runLeft = new Strand((Node)(object)farHome, thick * 0.9f, end: true, seen);
			Strand runRight = new Strand((Node)(object)farHome, thick * 0.9f, end: true, seen);
			Strand nearLeft = new Strand((Node)(object)holder, thick, end: false, cover: false);
			Strand nearRight = new Strand((Node)(object)holder, thick, end: false, cover: false);
			Sprite2D knot = FxKit.Glow(new Color(1f, 0.34f, 0.26f, 0f), new Vector2(64f, 64f));
			((Node)holder).AddChild((Node)(object)knot, false, (InternalMode)0);
			Vector2[] leftPts = new Vector2[12];
			Vector2[] rightPts = new Vector2[12];
			Vector2[] runLeftPts = new Vector2[24];
			Vector2[] runRightPts = new Vector2[24];
			Rope rope = new Rope
			{
				Front = holder,
				Behind = behind,
				At = at,
				Who = creature,
				SnapAt = float.MaxValue,
				Owned = MultiplayerFx.Active,
				Scene = front
			};
			((Node)holder).TreeExiting += () =>
			{
				if (rope.Waits)
				{
					Free(rope);
				}
				else
				{
					Close(rope);
				}
			};
			float brokeAt = 0f;
			float brokeA = a1;
			float brokeB = b1;
			float brokeTail = tail1;
			float brokeSlope = -0.2f;
			float brokeFat = 1f;
			float brokeGrip = 0f;
			Vector2 brokeMid = centre;
			rope.Pull = (CardModel forCard, bool wait) =>
			{
				//IL_0391: Unknown result type (might be due to invalid IL or missing references)
				//IL_0396: Unknown result type (might be due to invalid IL or missing references)
				//IL_0302: Unknown result type (might be due to invalid IL or missing references)
				//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_032d: Unknown result type (might be due to invalid IL or missing references)
				//IL_047b: Unknown result type (might be due to invalid IL or missing references)
				if (!rope.Done && (!rope.Owned || !rope.Closed) && FxKit.Alive((GodotObject)(object)holder))
				{
					rope.Waits = false;
					rope.Going = false;
					rope.Shown = 1f;
					Waiting.Remove(rope);
					rope.Card = forCard;
					if (rope.Owned)
					{
						rope.Source = forCard;
						rope.Invocation = MultiplayerFx.CurrentInvocation;
						rope.Pending = wait;
						rope.Requests.Clear();
					}
					grip = Mathf.Clamp(wait ? Tuning.F("strangle.grip", 0.1f) : Tuning.F("strangle.quick_grip", 0.1f), 0.02f, 1f);
					outTime = Mathf.Clamp(wait ? Tuning.F("strangle.out", 0.26f) : Tuning.F("strangle.quick_out", 0.22f), 0.08f, 1f);
					if (hang)
					{
						yank = Mathf.Clamp(Tuning.F("strangle.yank", 0.05f), 0.02f, 0.3f);
						over = Mathf.Clamp(Tuning.F("strangle.over", 0.12f), 0f, 0.2f);
						still = Mathf.Clamp(Tuning.F("strangle.stop", 0.05f), 0f, 0.2f);
						tightAt = yank + Mathf.Clamp(Tuning.F("strangle.settle", 0.1f), 0.02f, 0.5f);
					}
					else
					{
						slack = Mathf.Clamp(wait ? Tuning.F("strangle.slack", 0.08f) : Tuning.F("strangle.quick_slack", 0.05f), 0f, 0.5f);
						haul = Mathf.Clamp(wait ? Tuning.F("strangle.haul", 0.22f) : Tuning.F("strangle.quick_haul", 0.15f), 0.05f, 1f);
						tightAt = slack + haul;
					}
					float num4 = tightAt + grip;
					float num5 = Mathf.Max(num4, Tuning.F("strangle.wait", 0.75f));
					total = (wait ? num5 : num4) + outTime + 0.08f;
					heavy = wait;
					Node2D val2 = body;
					if (!FxKit.Alive((GodotObject)(object)val2) || !((CanvasItem)val2).Visible)
					{
						val2 = null;
					}
					if (creature != null && !creature.IsAlive)
					{
						val2 = null;
					}
					for (int num6 = Live.Count - 1; num6 >= 0; num6--)
					{
						Rope rope2 = Live[num6];
						bool flag2 = ((!rope.Owned) ? (rope2.At.DistanceSquaredTo(at) < 3600f) : (rope2.Scene == front && On(rope2, creature, at)));
						if (!rope2.Snapped & flag2)
						{
							rope2.SnapAt = Mathf.Min(rope2.SnapAt, rope2.Time);
						}
						if (val2 != null && rope2.Body == val2)
						{
							val2 = null;
						}
					}
					rope.Body = val2;
					rope.Factor = Vector2.One;
					if (val2 != null)
					{
						rope.Home = val2.Scale;
						rope.Wrote = rope.Home;
					}
					rope.Tight = tightAt;
					rope.Grip = grip;
					rope.SnapAt = (wait ? num5 : num4);
					Live.Add(rope);
					if (wait && !rope.Owned)
					{
						_pending = rope;
					}
					FxKit.Drive((Node)(object)holder, total, (float u) =>
					{
						Frame(u * total);
					}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
					{
						Free(rope);
					})));
					if (behind != null)
					{
						FxKit.FreeAfter((Node)(object)behind, total + 0.5f);
					}
				}
			};
			if (hang)
			{
				rope.Waits = true;
				rope.Asked = true;
				rope.Step = (float dt) =>
				{
					if (rope.Going)
					{
						fall = 2.5f;
					}
					else if (rope.Shown <= 0f || rope.Shown >= 1f)
					{
						fall = -1.5f;
					}
					rope.Shown = Mathf.Clamp(rope.Shown + (rope.Going ? (0f - dt) : dt) / slip, 0f, 1f);
					Frame(0f);
				};
				Waiting.Add(rope);
				if (!_ticking)
				{
					Ticker.Add(Idle);
					_ticking = true;
				}
				Frame(0f);
			}
			return rope;
			void Frame(float t)
			{
				//IL_0834: Unknown result type (might be due to invalid IL or missing references)
				//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
				//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_08d1: Unknown result type (might be due to invalid IL or missing references)
				//IL_08db: Unknown result type (might be due to invalid IL or missing references)
				//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
				//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_08f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0906: Unknown result type (might be due to invalid IL or missing references)
				//IL_090b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0926: Unknown result type (might be due to invalid IL or missing references)
				//IL_0937: Unknown result type (might be due to invalid IL or missing references)
				//IL_093c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0941: Unknown result type (might be due to invalid IL or missing references)
				//IL_0951: Unknown result type (might be due to invalid IL or missing references)
				//IL_0956: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0ad3: Unknown result type (might be due to invalid IL or missing references)
				//IL_0acc: Unknown result type (might be due to invalid IL or missing references)
				//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0436: Unknown result type (might be due to invalid IL or missing references)
				//IL_043b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0440: Unknown result type (might be due to invalid IL or missing references)
				//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0507: Unknown result type (might be due to invalid IL or missing references)
				//IL_050c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0511: Unknown result type (might be due to invalid IL or missing references)
				//IL_051b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0537: Unknown result type (might be due to invalid IL or missing references)
				//IL_055c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0613: Unknown result type (might be due to invalid IL or missing references)
				//IL_0654: Unknown result type (might be due to invalid IL or missing references)
				//IL_065e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0697: Unknown result type (might be due to invalid IL or missing references)
				//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_073c: Unknown result type (might be due to invalid IL or missing references)
				//IL_073e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0778: Unknown result type (might be due to invalid IL or missing references)
				if (!rope.Done && FxKit.Alive((GodotObject)(object)holder))
				{
					bool waits = rope.Waits;
					if (!waits)
					{
						rope.Time = t;
					}
					if (rope.Body != null && rope.Who != null && !rope.Who.IsAlive)
					{
						Pinch(rope, Vector2.One);
						rope.Body = null;
					}
					if (!rope.Snapped)
					{
						float num4;
						float num5;
						float num6;
						float num7;
						if (hang)
						{
							num4 = (waits ? 0f : Mathf.Clamp(t / tightAt, 0f, 1f));
							num5 = (waits ? 0f : Yank(t, yank, tightAt - yank, over));
							num6 = 0f;
							num7 = 1f;
							if (!waits && !stilled && t >= yank)
							{
								stilled = true;
								if (still > 0.005f && rope.Who != null && rope.Who.IsAlive)
								{
									if (rope.Owned)
									{
										if (!rope.Closed && MultiplayerFx.Active)
										{
											CreatureHold.Hold(rope.Who, still, rope);
										}
									}
									else
									{
										CreatureHold.Hold(rope.Who, still);
									}
								}
							}
						}
						else
						{
							num4 = Mathf.Clamp((t - slack) / haul, 0f, 1f);
							num5 = Haul(num4);
							num6 = Mathf.Clamp(1f - t / 0.07f, 0f, 1f);
							num7 = 1f - Mathf.Exp((0f - t) * 34f);
						}
						float num8 = Mathf.Clamp(num5, 0f, 1f);
						float num9 = ((waits || t <= tightAt) ? 0f : (1f - Mathf.Exp((0f - (t - tightAt)) * 10f)));
						float num10 = num9 * Mathf.Sin(t * 96f);
						float num11 = 1.07f - 0.07f * num7;
						float num12 = (hang ? (1f - num8) : 0f);
						float num13 = 0f;
						float num14 = 0f;
						float num15 = 0f;
						if (num12 > 0f)
						{
							double num16 = (double)FxClock.Milliseconds * 0.001 * 2.6 + phase;
							float num17 = (float)Math.Sin(num16);
							float num18 = (float)Math.Sin(num16 - 1.1);
							num13 = b0 * (0.45f + 0.05f * sway * num18) * num12;
							num14 = (w * 0.012f + 2f) * sway * num17 * num12;
							num15 = 0.05f * sway * num18 * num12;
						}
						float num19 = (waits ? Mathf.Clamp(rope.Shown, 0f, 1f) : 1f);
						float num20 = 1f - num19 * num19 * (3f - 2f * num19);
						float num21 = droop * num20 * fall;
						float num22 = Mathf.Lerp(a0 * num11, a1, num5) * (1f - 0.035f * num9 + 0.01f * num10) * (1f + 0.1f * num20);
						float num23 = Mathf.Lerp(b0 * num11, b1, num5) * (1f - 0.035f * num9 - 0.014f * num10);
						Vector2 val2 = centre + new Vector2(num14, droop * (num7 * (1f - num8) - 0.8f * (1f - num7)) + droop * 0.6f * num12 + num21);
						float num24 = Mathf.Lerp(tail0, tail1, num5) + 14f * root * num9;
						float num25 = Mathf.Lerp(0.3f + 0.28f * num12, -0.2f, num8);
						float sag = num24 * (0.24f + 0.08f * num12) * (1f - num8);
						float num26 = (1f + 0.55f * num6) * (1f - 0.22f * num9);
						float num27 = Mathf.Max(num6, Mathf.Max(0.35f * Mathf.Sin((float)Math.PI * num4), 0.9f * num9 * num9));
						float quiver = thick * 0.34f * num9;
						Vector2 val3 = val2 + new Vector2(0f, num23 + num13);
						Near(num22, num23 + num13, val2, 0f);
						Run(runLeftPts, 1f, num22, num23, val2, num24, num25 + num15, num8, sag, quiver, t);
						Run(runRightPts, -1f, num22, num23, val2, num24, num25 - num15, num8, sag, quiver, t);
						Hide(runLeft, runLeftPts);
						Hide(runRight, runRightPts);
						runLeft.Set(runLeftPts, num19, num26, num27 * 0.7f);
						runRight.Set(runRightPts, num19, num26, num27 * 0.7f);
						nearLeft.Set(leftPts, num19, num26, num27);
						nearRight.Set(rightPts, num19, num26, num27);
						if (FxKit.Alive((GodotObject)(object)knot))
						{
							((Node2D)knot).GlobalPosition = val3;
							float num28 = thick * (2.2f + 3.6f * num9) * (0.9f + 0.1f * Mathf.Sin(t * 95f));
							((Node2D)knot).Scale = new Vector2(num28, num28) / 64f;
							((CanvasItem)knot).Modulate = new Color(1f, 0.34f + 0.4f * num9, 0.26f + 0.4f * num9, 0.7f * num9);
						}
						if (rope.Body != null)
						{
							float num29 = squeeze * Mathf.Clamp(num5, 0f, 1.2f);
							Pinch(rope, new Vector2(1f - num29, 1f + 0.4f * num29));
						}
						if (!waits && t >= rope.SnapAt)
						{
							rope.Snapped = true;
							if (_pending == rope)
							{
								_pending = null;
							}
							brokeAt = t;
							brokeA = num22;
							brokeB = num23;
							brokeMid = val2;
							brokeTail = num24;
							brokeSlope = num25;
							brokeFat = num26;
							brokeGrip = Mathf.Clamp(num5, 0f, 1.2f);
							try
							{
								Break(front, val3, w, num22, num23, root, heavy);
							}
							catch (Exception ex)
							{
								DevLog.Warn("Strangle (snap): " + ex.Message);
							}
						}
					}
					else
					{
						float num30 = Mathf.Clamp((t - brokeAt) / outTime, 0f, 1f);
						if (num30 >= 1f)
						{
							Free(rope);
						}
						else
						{
							float num31 = Mathf.Clamp(num30 / 0.3f, 0f, 1f);
							float gap = 1.5507964f * (1f - (1f - num31) * (1f - num31));
							Near(brokeA * (1f + 0.25f * num31), brokeB, brokeMid, gap);
							float num32 = Mathf.Clamp((num30 - 0.2f) / 0.8f, 0f, 1f);
							float num33 = 1f - (1f - num32) * (1f - num32) * (1f - num32);
							float num34 = brokeTail + recoil * num33;
							float num35 = Mathf.Sqrt(Mathf.Max(0.05f, 1f - brokeSlope * brokeSlope));
							Vector2 val4 = brokeMid + new Vector2(0f, 0f - brokeB);
							Whip(runLeftPts, brokeMid + new Vector2(brokeA, 0f), val4 + new Vector2((0f - num34) * num35, num34 * brokeSlope), 1f, num33, num32);
							Whip(runRightPts, brokeMid + new Vector2(0f - brokeA, 0f), val4 + new Vector2(num34 * num35, num34 * brokeSlope), -1f, num33, num32);
							Hide(runLeft, runLeftPts);
							Hide(runRight, runRightPts);
							float alpha = 1f - num31 * num31;
							float alpha2 = 1f - num32 * num32;
							float fat = brokeFat * (1f - 0.55f * num30);
							runLeft.Set(runLeftPts, alpha2, fat, 0.6f * (1f - num32));
							runRight.Set(runRightPts, alpha2, fat, 0.6f * (1f - num32));
							nearLeft.Set(leftPts, alpha, fat, 0.8f * (1f - num31));
							nearRight.Set(rightPts, alpha, fat, 0.8f * (1f - num31));
							if (FxKit.Alive((GodotObject)(object)knot))
							{
								((CanvasItem)knot).Visible = false;
							}
							if (rope.Body != null)
							{
								float num36 = Mathf.Clamp((t - brokeAt) / 0.14f, 0f, 1f);
								float num37 = squeeze * brokeGrip * (1f - BackOut(num36));
								Pinch(rope, (Vector2)((num36 >= 1f) ? Vector2.One : new Vector2(1f - num37, 1f + 0.4f * num37)));
							}
						}
					}
				}
			}
			void Hide(Strand strand, Vector2[] pts)
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				if (seen)
				{
					float x = farHome.ToLocal(centre).X;
					float num4 = Mathf.Abs(farHome.ToLocal(centre + new Vector2(cover, 0f)).X - x);
					float num5 = 0f;
					for (int i = 1; i < pts.Length; i++)
					{
						num5 += pts[i].DistanceTo(pts[i - 1]);
					}
					float num6 = 2f;
					float num7 = -1f;
					if (num5 > 1f)
					{
						float num8 = 0f;
						for (int j = 0; j < pts.Length; j++)
						{
							if (j > 0)
							{
								num8 += pts[j].DistanceTo(pts[j - 1]);
							}
							if (!(Mathf.Abs(pts[j].X - x) >= num4))
							{
								float num9 = num8 / num5;
								if (num9 < num6)
								{
									num6 = num9;
								}
								if (num9 > num7)
								{
									num7 = num9;
								}
							}
						}
					}
					strand.Cover(num6, num7);
				}
			}
			void Near(float num6, float num7, Vector2 mid, float gap)
			{
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				for (int i = 0; i < 12; i++)
				{
					float num4 = (float)i / 11f;
					float num5 = Mathf.Lerp((float)Math.PI / 2f + gap, (float)Math.PI, num4);
					leftPts[i] = holder.ToLocal(mid + new Vector2(num6 * Mathf.Cos(num5), num7 * Mathf.Sin(num5)));
					float num8 = Mathf.Lerp((float)Math.PI / 2f - gap, 0f, num4);
					rightPts[i] = holder.ToLocal(mid + new Vector2(num6 * Mathf.Cos(num8), num7 * Mathf.Sin(num8)));
				}
			}
			void Run(Vector2[] pts, float s, float num4, float num5, Vector2 mid, float length, float slope, float taut, float sag, float quiver, float t)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_005d: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_0101: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Unknown result type (might be due to invalid IL or missing references)
				//IL_0104: Unknown result type (might be due to invalid IL or missing references)
				//IL_0107: Unknown result type (might be due to invalid IL or missing references)
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0122: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_012c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0131: Unknown result type (might be due to invalid IL or missing references)
				//IL_0171: Unknown result type (might be due to invalid IL or missing references)
				//IL_0173: Unknown result type (might be due to invalid IL or missing references)
				//IL_0178: Unknown result type (might be due to invalid IL or missing references)
				Vector2 val2 = mid + new Vector2(s * num4, 0f);
				Vector2 val3 = mid + new Vector2(0f, 0f - num5);
				float num6 = Mathf.Sqrt(Mathf.Max(0.05f, 1f - slope * slope));
				Vector2 val4 = val3 + new Vector2((0f - s) * length * num6, length * slope);
				Vector2 b2 = val3 + new Vector2((0f - s) * length * 0.5f, sag);
				float num7 = Girth(num4, num5) * 0.25f;
				float num8 = num7 / Mathf.Max(1f, num7 + length);
				for (int i = 0; i < 24; i++)
				{
					Vector2 val5;
					float num11;
					if (i < 10)
					{
						float num9 = (float)i / 9f;
						float num10 = (0f - num9) * (float)Math.PI * 0.5f;
						val5 = mid + new Vector2(s * num4 * Mathf.Cos(num10), num5 * Mathf.Sin(num10));
						num11 = num8 * num9;
					}
					else
					{
						float num12 = (float)(i - 10 + 1) / 14f;
						val5 = FxKit.Bezier(val3, b2, val4, num12);
						num11 = num8 + (1f - num8) * num12;
					}
					val5 = val5.Lerp(val2.Lerp(val4, num11), taut);
					val5.Y += Mathf.Sin(t * 130f + (float)i * 1.3f + s) * quiver * Mathf.Sin((float)Math.PI * num11);
					pts[i] = farHome.ToLocal(val5);
				}
			}
			void Whip(Vector2[] pts, Vector2 k, Vector2 end, float s, float e, float r)
			{
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				float num4 = 0.9f * e;
				for (int i = 0; i < 24; i++)
				{
					float num5 = (float)i / 23f;
					Vector2 val2 = k.Lerp(end, Mathf.Lerp(num4, 1f, num5));
					val2.Y += Mathf.Sin(num5 * 7f - r * 15f + s) * 24f * root * (1f - r) * (1f - num5);
					pts[i] = farHome.ToLocal(val2);
				}
			}
		}

		private static NCreature Find(Creature who, Vector2 at)
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			if (combatRoom == null)
			{
				return null;
			}
			if (who != null)
			{
				NCreature creatureNode = combatRoom.GetCreatureNode(who);
				if (!FxKit.Alive((GodotObject)(object)creatureNode) || !((Node)creatureNode).IsInsideTree())
				{
					return null;
				}
				return creatureNode;
			}
			if (MultiplayerFx.Active)
			{
				return null;
			}
			NCreature result = null;
			float num = 1600f;
			foreach (NCreature creatureNode2 in combatRoom.CreatureNodes)
			{
				if (FxKit.Alive((GodotObject)(object)creatureNode2) && ((Node)creatureNode2).IsInsideTree())
				{
					Vector2 vfxSpawnPosition = creatureNode2.VfxSpawnPosition;
					float num2 = vfxSpawnPosition.DistanceSquaredTo(at);
					if (!(num2 >= num))
					{
						result = creatureNode2;
						num = num2;
					}
				}
			}
			return result;
		}

		private static void Pinch(Rope rope, Vector2 factor)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			Node2D body = rope.Body;
			if (body == null)
			{
				return;
			}
			if (!FxKit.Alive((GodotObject)(object)body))
			{
				rope.Body = null;
			}
			else if (!(factor == rope.Factor))
			{
				Vector2 scale = body.Scale;
				if (scale != rope.Wrote)
				{
					rope.Home = new Vector2(scale.X / rope.Factor.X, scale.Y / rope.Factor.Y);
				}
				Vector2 wrote = (body.Scale = (Vector2)((factor == Vector2.One) ? rope.Home : new Vector2(rope.Home.X * factor.X, rope.Home.Y * factor.Y)));
				rope.Wrote = wrote;
				rope.Factor = factor;
			}
		}

		private static void Close(Rope rope)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			if (rope.Owned)
			{
				rope.Closed = true;
				rope.Requests.Clear();
				CreatureHold.Release(rope.Who, rope);
			}
			Live.Remove(rope);
			Waiting.Remove(rope);
			if (_pending == rope)
			{
				_pending = null;
			}
			try
			{
				Pinch(rope, Vector2.One);
			}
			catch
			{
			}
			rope.Body = null;
		}

		private static void Free(Rope rope)
		{
			Close(rope);
			if (!rope.Done)
			{
				rope.Done = true;
				if (FxKit.Alive((GodotObject)(object)rope.Front))
				{
					((Node)rope.Front).QueueFree();
				}
				if (FxKit.Alive((GodotObject)(object)rope.Behind))
				{
					((Node)rope.Behind).QueueFree();
				}
			}
		}

		private static void Break(Node parent, Vector2 at, float w, float a, float b, float root, bool heavy)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)parent))
			{
				FxKit.Flash(parent, at, new Color(1f, 0.93f, 0.88f, 0.95f), w * 0.42f, 2.4f, 0.15);
				FxKit.Flash(parent, at, new Color(1f, 0.14f, 0.14f, 0.8f), w * 0.8f, 2f);
				Color color = new Color(1f, 0.3f, 0.26f, 0.95f);
				Brush.Cut(parent, at, at + new Vector2(a * 1.25f, (0f - b) * 1.1f), 20f * root, color, 0f, 0.04f, 0.02f, 0.18f);
				Brush.Cut(parent, at, at + new Vector2((0f - a) * 1.25f, (0f - b) * 1.1f), 20f * root, color, 0f, 0.04f, 0.02f, 0.18f);
				FxKit.Burst(parent, at, new Color(1f, 0.2f, 0.18f, 0.95f), Tuning.I("strangle.shreds", 12), 220f, 640f, 0.42, 0.06f, 0.15f, new Vector2(0f, 760f));
				if (heavy && Tuning.F("strangle.shake", 1f) > 0.5f)
				{
					GameRefs.Shake(1);
				}
			}
		}

		private static float Girth(float a, float b)
		{
			return (float)Math.PI * (3f * (a + b) - Mathf.Sqrt(Mathf.Max(0f, (3f * a + b) * (a + 3f * b))));
		}

		private static float Haul(float x)
		{
			if (x <= 0f)
			{
				return 0f;
			}
			if (x >= 1f)
			{
				return 1f;
			}
			if (x < 0.68f)
			{
				float num = x / 0.68f;
				return 1.07f * num * num * (3f - 2f * num);
			}
			float num2 = (x - 0.68f) / 0.32f;
			return 1f + 0.07f * (1f - num2 * num2 * (3f - 2f * num2));
		}

		private static float Yank(float t, float pull, float ease, float over)
		{
			if (t <= 0f)
			{
				return 0f;
			}
			if (t < pull)
			{
				float num = t / pull;
				return (1f + over) * num * (2f - num);
			}
			if (ease <= 0.001f || t >= pull + ease)
			{
				return 1f;
			}
			float num2 = (t - pull) / ease;
			return 1f + over * (1f - num2 * num2 * (3f - 2f * num2));
		}

		private static float BackOut(float x)
		{
			float num = x - 1f;
			return 1f + 2.70158f * num * num * num + 1.70158f * num * num;
		}

		static StrangleRope()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		}
	}
