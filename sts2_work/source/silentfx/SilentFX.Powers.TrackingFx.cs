// namespace: SilentFX.Powers
// type: TrackingFx (class)
// source lines: 18598-19185
	public sealed class TrackingFx : BasicPowerFx
	{
		private sealed class Mark
		{
			public Node2D Node;

			public ShaderMaterial Mat;

			public float Size;

			public float Alpha;

			public float Target = 1f;

			public float Snap = 1f;

			public float Age;

			public bool Slams;

			public bool Landed;

			public Line2D[] Locks;
		}

		private static readonly StringName PAlpha = StringName.op_Implicit("alpha");

		private static readonly StringName PSpin = StringName.op_Implicit("spin");

		private static readonly StringName PInnerSpin = StringName.op_Implicit("inner_spin");

		private readonly List<FxTarget> _targetScratch = new List<FxTarget>();

		private const float SlamAt = 0.3f;

		private const float BounceTime = 0.3f;

		private static readonly Color Red = new Color(1f, 0.3f, 0.16f, 1f);

		private static readonly Color Trail = new Color(0.3f, 0.92f, 0.98f, 1f);

		private readonly Dictionary<object, Mark> _marks = new Dictionary<object, Mark>();

		private readonly Dictionary<object, float> _holdUntil = new Dictionary<object, float>();

		private float _lastShake = -10f;

		private bool _slammed;

		private float _trailEnds;

		protected override string NodeName => "Tracking";

		protected override bool UseBack => false;

		protected override void Build(bool animateIn)
		{
			if (animateIn && Tuning.F("intro.on", 1f) > 0.5f)
			{
				LayTrail();
			}
		}

		protected override void OnStacked()
		{
			if (Tuning.F("intro.on", 1f) > 0.5f)
			{
				LayTrail();
			}
		}

		private void LayTrail()
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			Node ground = Anchor.BackParent;
			List<FxTarget> targets;
			Vector2 body;
			float unit;
			bool laidAny;
			if (FxKit.Alive((GodotObject)(object)ground))
			{
				Anchor.SafeTargets(_targetScratch);
				targets = _targetScratch;
				body = Anchor.SafeBodySize();
				unit = Mathf.Clamp(body.Y / 300f, 0.7f, 1.25f);
				laidAny = false;
				Lay(1f);
				Lay(-1f);
			}
			void Lay(float direction)
			{
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00df: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0103: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
				//IL_0237: Unknown result type (might be due to invalid IL or missing references)
				targets.Sort((FxTarget a, FxTarget b) => (a.Feet.X * direction).CompareTo(b.Feet.X * direction));
				List<Vector2> list = new List<Vector2> { Anchor.Feet() + new Vector2(body.X * 0.55f * direction, -4f) };
				List<object> list2 = new List<object> { null };
				foreach (FxTarget item in targets)
				{
					if (item.Side == direction && item.Key != null && !(item.Feet.X * direction < list[list.Count - 1].X * direction + 30f))
					{
						list.Add(item.Feet + new Vector2((0f - item.Width) * 0.1f * direction, -4f));
						list2.Add(item.Key);
					}
				}
				if (list.Count >= 2)
				{
					if (!laidAny)
					{
						_holdUntil.Clear();
						_trailEnds = Time;
						laidAny = true;
					}
					float num = Mathf.Max(30f, Tuning.F("tracking.stride", 104f) * unit);
					float num2 = Mathf.Max(0.02f, Tuning.F("tracking.step_time", 0.06f));
					int num3 = 0;
					float num4 = num * 0.5f;
					for (int num5 = 1; num5 < list.Count; num5++)
					{
						Vector2 val = list[num5 - 1];
						float num6;
						for (num6 = val.DistanceTo(list[num5]); num4 < num6; num4 += num)
						{
							Node ground2 = ground;
							val = list[num5 - 1];
							Print(ground2, val.Lerp(list[num5], num4 / num6), num3, (float)num3 * num2, unit, last: false, direction);
							num3++;
						}
						num4 -= num6;
						Print(ground, list[num5], num3, (float)num3 * num2, unit * 1.35f, last: true, direction);
						_holdUntil[list2[num5]] = Time + (float)num3 * num2 + 0.1f;
						num3++;
					}
					_trailEnds = Mathf.Max(_trailEnds, Time + (float)num3 * num2 + 1f);
				}
			}
		}

		private static void Print(Node ground, Vector2 at, int index, float delay, float unit, bool last, float direction)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected Obj, but got Unknown
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			Node2D holder = new Node2D
			{
				Name = StringName.op_Implicit("SilentFX_Track")
			};
			ground.AddChild((Node)(object)holder, false, (InternalMode)0);
			if (!((Node)holder).IsInsideTree())
			{
				((Node)holder).QueueFree();
				return;
			}
			float num;
			if (last)
			{
				num = 0f;
			}
			else
			{
				num = ((index % 2 == 0) ? (-1f) : 1f);
			}
			holder.GlobalPosition = at + new Vector2(0f, num * 10f * unit);
			Node2D val = Props.Paw(Trail, 24f);
			val.Rotation = (float)Math.PI / 2f + num * 0.18f;
			((Node)holder).AddChild((Node)(object)val, false, (InternalMode)0);
			holder.Scale = Vector2.Zero;
			float num2 = (last ? 0.95f : 0.7f);
			Intro.Run((Node)(object)holder, delay, num2, (float v) =>
			{
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				float num3 = unit * Intro.BackOut(v / 0.22f);
				holder.Scale = new Vector2(num3 * direction, num3 * 0.62f);
				((CanvasItem)holder).Modulate = new Color(1f, 1f, 1f, (v < 0.5f) ? 1f : (1f - (v - 0.5f) / 0.5f));
			}, (TransitionType)0, (EaseType)2);
			if (last)
			{
				FxKit.After((Node)(object)holder, delay, () =>
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0034: Unknown result type (might be due to invalid IL or missing references)
					FxKit.Flash((Node)(object)holder, holder.GlobalPosition, new Color(Trail.R, Trail.G, Trail.B, 0.55f), 90f, 2f, 0.25);
				});
			}
			FxKit.FreeAfter((Node)(object)holder, (double)(delay + num2) + 0.2);
		}

		private Mark Create(FxTarget target)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			Mark mark = new Mark
			{
				Size = Mathf.Max(target.Width * Tuning.F("tracking.size_mul", 1f), 170f),
				Slams = (Tuning.F("tracking.slam", 1f) > 0.5f)
			};
			mark.Node = FxKit.Sigil(mark.Size, Red, out mark.Mat);
			((Node)Front).AddChild((Node)(object)mark.Node, false, (InternalMode)0);
			if (mark.Mat != null)
			{
				mark.Mat.SetShaderParameter(StringName.op_Implicit("radius"), Variant.op_Implicit(0.72f));
				mark.Mat.SetShaderParameter(StringName.op_Implicit("width"), Variant.op_Implicit(0.02f));
				mark.Mat.SetShaderParameter(StringName.op_Implicit("ticks"), Variant.op_Implicit(4f));
				mark.Mat.SetShaderParameter(StringName.op_Implicit("tick_len"), Variant.op_Implicit(0.3f));
				mark.Mat.SetShaderParameter(StringName.op_Implicit("tick_width"), Variant.op_Implicit(0.02f));
				mark.Mat.SetShaderParameter(StringName.op_Implicit("tick_out"), Variant.op_Implicit(0.5f));
				mark.Mat.SetShaderParameter(StringName.op_Implicit("inner"), Variant.op_Implicit(0.36f));
				mark.Mat.SetShaderParameter(StringName.op_Implicit("inner_width"), Variant.op_Implicit(0.016f));
				mark.Mat.SetShaderParameter(StringName.op_Implicit("inner_dashes"), Variant.op_Implicit(6f));
				mark.Mat.SetShaderParameter(StringName.op_Implicit("glow"), Variant.op_Implicit(0.22f));
				mark.Mat.SetShaderParameter(PAlpha, Variant.op_Implicit(0f));
			}
			if (!mark.Slams)
			{
				return mark;
			}
			mark.Snap = 0f;
			mark.Alpha = 1f;
			mark.Locks = new Line2D[4];
			for (int i = 0; i < mark.Locks.Length; i++)
			{
				Line2D val = Props.Line(6.5f, new Color(1f, 0.38f, 0.22f, 0.95f), true);
				((Node)Front).AddChild((Node)(object)val, false, (InternalMode)0);
				mark.Locks[i] = val;
			}
			return mark;
		}

		private void Land(Mark mark)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0326: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0372: Unknown result type (might be due to invalid IL or missing references)
			mark.Landed = true;
			if (mark.Locks != null)
			{
				Line2D[] locks = mark.Locks;
				foreach (Line2D val in locks)
				{
					if (FxKit.Alive((GodotObject)(object)val))
					{
						((Node)val).QueueFree();
					}
				}
				mark.Locks = null;
			}
			Node frontParent = Anchor.FrontParent;
			if (!FxKit.Alive((GodotObject)(object)frontParent) || !FxKit.Alive((GodotObject)(object)mark.Node))
			{
				return;
			}
			Vector2 globalPosition = mark.Node.GlobalPosition;
			float size = mark.Size;
			FxKit.Flash(frontParent, globalPosition, new Color(1f, 0.86f, 0.8f, 0.95f), size * 0.9f, 2f, 0.22);
			FxKit.Flash(frontParent, globalPosition, new Color(1f, 0.25f, 0.12f, 0.75f), size * 1.5f, 1.7f, 0.4);
			FxKit.Beam(frontParent, globalPosition - new Vector2(size * 1.25f, 0f), globalPosition + new Vector2(size * 1.25f, 0f), 7f, Red, 0.05f, 0.05f, 0.24f);
			FxKit.Beam(frontParent, globalPosition - new Vector2(0f, size * 1.05f), globalPosition + new Vector2(0f, size * 1.05f), 7f, Red, 0.05f, 0.05f, 0.24f);
			FxKit.RingPulse(frontParent, globalPosition, size * 1.9f, new Color(1f, 0.45f, 0.25f, 1f), 0f, 0.38f, 0.45f, 1.35f, 1f, 0f, 0.04f, 0.95f);
			FxKit.RingPulse(frontParent, globalPosition, size * 2.6f, new Color(1f, 0.3f, 0.16f, 1f), 0.07f, 0.48f, 0.4f, 1.3f, 1f, 12f, 0.022f, 0.7f);
			FxKit.Burst(frontParent, globalPosition, new Color(1f, 0.45f, 0.25f, 0.95f), 24, 220f, 620f, 0.45, 0.1f, 0.26f, new Vector2(0f, 260f));
			float num = Tuning.F("tracking.veil", 0.26f);
			if (num > 0.01f)
			{
				ColorRect veil = FxKit.Veil(new Color(0.2f, 0.01f, 0f, 0f));
				frontParent.AddChild((Node)(object)veil, false, (InternalMode)0);
				FxKit.FreeAfter((Node)(object)veil, 0.4399999976158142);
				if (((Node)veil).IsInsideTree())
				{
					Tween val2 = ((Node)veil).CreateTween();
					val2.TweenProperty((GodotObject)(object)veil, NodePath.op_Implicit("color:a"), Variant.op_Implicit(num), 0.04);
					val2.TweenProperty((GodotObject)(object)veil, NodePath.op_Implicit("color:a"), Variant.op_Implicit(0f), 0.3);
					val2.TweenCallback(Callable.From((Action)(() =>
					{
						if (FxKit.Alive((GodotObject)(object)veil))
						{
							((Node)veil).QueueFree();
						}
					})));
				}
				else
				{
					((Node)veil).QueueFree();
				}
			}
			int num2 = Tuning.I("tracking.shake", 2);
			if (num2 > 0 && Time - _lastShake > 0.25f)
			{
				_lastShake = Time;
				GameRefs.Shake(num2);
				Sfx.Play("track_slam", _slammed ? 0.6f : 1f);
				_slammed = true;
			}
		}

		protected override void Tick(float dt)
		{
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			foreach (Mark value5 in _marks.Values)
			{
				value5.Target = 0f;
			}
			Anchor.SafeTargets(_targetScratch);
			foreach (FxTarget item in _targetScratch)
			{
				if (!item.Weak || item.Key == null)
				{
					continue;
				}
				if (!_marks.TryGetValue(item.Key, out var value))
				{
					if (_holdUntil.TryGetValue(item.Key, out var value2) && Time < value2)
					{
						continue;
					}
					value = Create(item);
					_marks[item.Key] = value;
				}
				value.Target = 1f;
				if (FxKit.Alive((GodotObject)(object)value.Node))
				{
					value.Node.GlobalPosition = item.Chest;
				}
			}
			if (_holdUntil.Count > 0 && Time > _trailEnds)
			{
				_holdUntil.Clear();
			}
			List<object> list = null;
			foreach (KeyValuePair<object, Mark> mark in _marks)
			{
				Mark value3 = mark.Value;
				value3.Age += dt;
				if (value3.Target > 0f && value3.Slams && !value3.Landed && value3.Age >= 0.3f)
				{
					Land(value3);
				}
				value3.Alpha = FxKit.Damp(value3.Alpha, value3.Target, 6f, dt);
				value3.Snap = FxKit.Damp(value3.Snap, 0f, 5f, dt);
				if (value3.Target <= 0f && value3.Alpha < 0.02f)
				{
					(list ?? (list = new List<object>())).Add(mark.Key);
				}
			}
			if (list == null)
			{
				return;
			}
			foreach (object item2 in list)
			{
				if (!_marks.Remove(item2, out var value4))
				{
					continue;
				}
				if (FxKit.Alive((GodotObject)(object)value4.Node))
				{
					((Node)value4.Node).QueueFree();
				}
				if (value4.Locks == null)
				{
					continue;
				}
				Line2D[] locks = value4.Locks;
				foreach (Line2D val in locks)
				{
					if (FxKit.Alive((GodotObject)(object)val))
					{
						((Node)val).QueueFree();
					}
				}
			}
		}

		protected override void Layout()
		{
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			float num = Tuning.F("tracking.alpha", 0.8f);
			foreach (Mark value in _marks.Values)
			{
				if (!FxKit.Alive((GodotObject)(object)value.Node))
				{
					continue;
				}
				float num2 = 1f + 0.7f * value.Snap;
				float num3 = 1f + 0.9f * value.Snap;
				float num4 = value.Snap * 1.6f;
				if (value.Slams && value.Age < 0.6f)
				{
					if (value.Age < 0.3f)
					{
						float num5 = value.Age / 0.3f;
						float num6 = ((num5 < 0.5f) ? 0f : ((num5 - 0.5f) / 0.5f));
						num2 *= Mathf.Lerp(3.2f, 0.82f, num6 * num6);
						num3 *= 0.35f + 0.65f * num5;
						num4 += (1f - num5) * 5f;
						LayLocks(value, num5);
					}
					else
					{
						float num7 = (value.Age - 0.3f) / 0.3f;
						num2 *= 1f - 0.18f * (1f - num7) * Mathf.Cos(num7 * (float)Math.PI * 2.2f);
						num3 *= 1f + 1.4f * (1f - num7);
					}
				}
				value.Node.Scale = new Vector2(num2, num2);
				if (FxKit.Alive((GodotObject)(object)value.Mat))
				{
					value.Mat.SetShaderParameter(PAlpha, Variant.op_Implicit(num * A * value.Alpha * num3));
					value.Mat.SetShaderParameter(PSpin, Variant.op_Implicit(Time * 0.5f + num4));
					value.Mat.SetShaderParameter(PInnerSpin, Variant.op_Implicit((0f - Time) * 1.3f));
				}
			}
		}

		private void LayLocks(Mark m, float a)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			if (m.Locks == null)
			{
				return;
			}
			Vector2 globalPosition = m.Node.GlobalPosition;
			float num = a * a;
			float num2 = Mathf.Lerp(620f, m.Size * 0.34f, num);
			float num3 = Mathf.Lerp(980f, m.Size * 0.6f, num);
			for (int i = 0; i < m.Locks.Length; i++)
			{
				Line2D val = m.Locks[i];
				if (FxKit.Alive((GodotObject)(object)val))
				{
					float num4 = (float)Math.PI / 4f + (float)i * (float)Math.PI * 0.5f;
					Vector2 val2 = new Vector2(Mathf.Cos(num4), Mathf.Sin(num4));
					val.Points = new Vector2[2]
					{
						((Node2D)val).ToLocal(globalPosition + val2 * num2),
						((Node2D)val).ToLocal(globalPosition + val2 * num3)
					};
					((CanvasItem)val).Modulate = new Color(1f, 1f, 1f, A * Mathf.Min(1f, a * 3f));
				}
			}
		}

		protected override void OnTrigger(Vector2? at)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			Mark mark = null;
			float num = float.MaxValue;
			foreach (Mark value in _marks.Values)
			{
				if (FxKit.Alive((GodotObject)(object)value.Node))
				{
					float num2;
					if (!at.HasValue)
					{
						num2 = 0f;
					}
					else
					{
						Vector2 globalPosition = value.Node.GlobalPosition;
						num2 = globalPosition.DistanceSquaredTo(at.Value);
					}
					float num3 = num2;
					if (num3 < num)
					{
						num = num3;
						mark = value;
					}
					if (!at.HasValue)
					{
						value.Snap = 1f;
					}
				}
			}
			if (mark != null)
			{
				mark.Snap = 1f;
				FxKit.Burst(Anchor.FrontParent, mark.Node.GlobalPosition, new Color(1f, 0.4f, 0.22f, 0.95f), 14, 140f, 380f, 0.4, 0.1f, 0.24f, new Vector2(0f, 300f));
			}
		}

		protected override void OnDispose()
		{
			foreach (Mark value in _marks.Values)
			{
				if (value.Locks == null)
				{
					continue;
				}
				Line2D[] locks = value.Locks;
				foreach (Line2D val in locks)
				{
					if (FxKit.Alive((GodotObject)(object)val))
					{
						((Node)val).QueueFree();
					}
				}
			}
		}

		static TrackingFx()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		}
	}
