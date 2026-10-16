// namespace: SilentFX.Cards
// type: StatusSteps (class)
// source lines: 104110-105274
	internal static class StatusSteps
	{
		internal sealed class Kind
		{
			public string Name;

			public Func<Creature, int> Read;

			public Func<PowerModel, bool> Is;
		}

		internal sealed class Counter
		{
			public Kind Kind;

			public int? After;

			public int Before;

			public int Shown;

			public NPower Node;

			public Label Label;

			public Control Icon;

			public bool Fresh;

			public bool Hidden;

			public bool Loose;

			public float Age;

			public float PopAt = -1f;

			public bool Asked;

			public PowerModel AskedModel;
		}

		internal sealed class Run
		{
			public Creature Target;

			public bool Multi;

			public Node Room;

			public bool Ui;

			public bool Released;

			public bool Suspended;

			public bool Captured;

			public float Age;

			public float MaxHold;

			public int Steps;

			public NCreature Victim;

			public NPowerContainer Row;

			public readonly List<Counter> Counters = new List<Counter>();

			public readonly HashSet<ulong> Seen = new HashSet<ulong>();
		}

		private static readonly Kind[] Kinds = new Kind[2]
		{
			new Kind
			{
				Name = "Strength",
				Read = (Creature c) => c.GetPowerAmount<StrengthPower>(),
				Is = (PowerModel p) => p is StrengthPower
			},
			new Kind
			{
				Name = "Weak",
				Read = (Creature c) => c.GetPowerAmount<WeakPower>(),
				Is = (PowerModel p) => p is WeakPower
			}
		};

		private const float PopTime = 0.16f;

		private const float PopDrop = 8f;

		private static Run _run;

		private static readonly List<Run> Runs = new List<Run>();

		private static readonly Stack<Run> Windows = new Stack<Run>();

		private static bool _ticking;

		private static bool _covering;

		private static bool _ours;

		private static FieldRef<NPower, Tween> _animIn;

		private static bool _lookedAnim;

		private static MethodInfo _refresh;

		private static MethodInfo _flash;

		private static bool _lookedMethods;

		public static bool Covering
		{
			get
			{
				if (_covering && Windows.Count > 0 && Current(Windows.Peek()))
				{
					return Windows.Peek().Ui;
				}
				return false;
			}
		}

		private static bool Current(Run run)
		{
			if (run != null && !run.Released)
			{
				if (!run.Multi)
				{
					return run == _run;
				}
				if (MultiplayerFx.Active && PlayerSettings.Enabled && (object)run.Room == GameRefs.CombatRoom)
				{
					return Runs.Contains(run);
				}
				return false;
			}
			return false;
		}

		private static Run For(Creature target)
		{
			Run[] array = Runs.ToArray();
			foreach (Run run in array)
			{
				if (!MultiplayerFx.Active || !PlayerSettings.Enabled || (object)run.Room != GameRefs.CombatRoom)
				{
					Release(run);
				}
				else if (run.Target == target && run.Ui)
				{
					return run;
				}
			}
			if (_run == null || _run.Target != target || !_run.Ui)
			{
				return null;
			}
			return _run;
		}

		public static Run Begin(Creature target, float maxHold)
		{
			try
			{
				Run run = _run;
				if (run != null)
				{
					Release(run);
				}
				if (MultiplayerFx.Active)
				{
					Run run2 = For(target);
					if (run2 != null)
					{
						Release(run2);
					}
				}
				if (target == null || Tuning.F("malaise.steps", 1f) < 0.5f)
				{
					return null;
				}
				if (TimeFreeze.Active || !StatusStepHooks.Ready)
				{
					return null;
				}
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(target) : null);
				if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree())
				{
					return null;
				}
				NPowerContainer val2 = Find<NPowerContainer>((Node)(object)val, 6);
				if (!FxKit.Alive((GodotObject)(object)val2) || !((Node)val2).IsInsideTree())
				{
					return null;
				}
				LookMethods();
				if (_refresh == null)
				{
					return null;
				}
				Run run3 = new Run
				{
					Target = target,
					Multi = false,
					Room = (Node)(object)GameRefs.CombatRoom,
					Ui = true,
					MaxHold = Mathf.Clamp(maxHold, 0.5f, 12f),
					Victim = val,
					Row = val2
				};
				Kind[] kinds = Kinds;
				foreach (Kind kind in kinds)
				{
					int amount = 0;
					if (!TryRead(kind, target, out amount))
					{
						amount = 0;
					}
					run3.Counters.Add(new Counter
					{
						Kind = kind,
						Before = amount,
						Shown = amount
					});
				}
				foreach (Node child in ((Node)val2).GetChildren(false))
				{
					NPower val3 = (NPower)(object)((child is NPower) ? child : null);
					if (val3 == null || !FxKit.Alive((GodotObject)(object)val3))
					{
						continue;
					}
					PowerModel val4 = ModelOf(val3);
					if (val4 == null)
					{
						continue;
					}
					run3.Seen.Add(((GodotObject)val3).GetInstanceId());
					foreach (Counter counter in run3.Counters)
					{
						if (counter.Node == null && counter.Kind.Is(val4))
						{
							Take(counter, val3, fresh: false);
						}
					}
				}
				_run = run3;
				if (!_ticking)
				{
					Ticker.AddLate(Update);
					_ticking = true;
				}
				return run3;
			}
			catch (Exception ex)
			{
				DevLog.Warn("StatusSteps.Begin: " + ex);
				try
				{
					if (_run != null)
					{
						Release(_run);
					}
				}
				catch
				{
					_run = null;
				}
				return null;
			}
		}

		internal static Run Capture(Creature target, float maxHold)
		{
			if (!MultiplayerFx.Active || target == null || Tuning.F("malaise.steps", 1f) < 0.5f)
			{
				return null;
			}
			Run run = new Run
			{
				Target = target,
				Multi = true,
				Captured = true,
				Room = (Node)(object)GameRefs.CombatRoom,
				MaxHold = Mathf.Clamp(maxHold, 0.5f, 12f)
			};
			Kind[] kinds = Kinds;
			foreach (Kind kind in kinds)
			{
				if (!TryRead(kind, target, out var amount))
				{
					return null;
				}
				run.Counters.Add(new Counter
				{
					Kind = kind,
					Before = amount,
					Shown = amount
				});
			}
			return run;
		}

		internal static void Seal(Run run)
		{
			if (run == null || run.Released || (object)run.Room != GameRefs.CombatRoom)
			{
				return;
			}
			foreach (Counter counter in run.Counters)
			{
				if (TryRead(counter.Kind, run.Target, out var amount))
				{
					counter.After = amount;
				}
			}
		}

		internal static Run Activate(Run run, Run previous = null)
		{
			if (run == null || run.Released || !MultiplayerFx.Active || (object)run.Room != GameRefs.CombatRoom)
			{
				return null;
			}
			if (run.Ui)
			{
				run.Suspended = false;
				return run;
			}
			if (TimeFreeze.Active || !StatusStepHooks.Ready)
			{
				return null;
			}
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(run.Target) : null);
			NPowerContainer val2 = Find<NPowerContainer>((Node)(object)val, 6);
			if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree() || !FxKit.Alive((GodotObject)(object)val2) || !((Node)val2).IsInsideTree())
			{
				return null;
			}
			LookMethods();
			if (_refresh == null)
			{
				return null;
			}
			Run run2 = For(run.Target);
			if (run2 != null && run2 != previous)
			{
				Release(run2);
			}
			run.Victim = val;
			run.Row = val2;
			run.Ui = true;
			if (previous != null && Current(previous) && previous.Row == val2)
			{
				foreach (Counter next in run.Counters)
				{
					Counter counter = previous.Counters.Find((Counter c) => c.Kind == next.Kind);
					if (counter != null)
					{
						next.Node = counter.Node;
						next.Label = counter.Label;
						next.Icon = counter.Icon;
						next.Fresh = counter.Fresh;
						next.Hidden = counter.Hidden;
						next.Loose = counter.Loose;
						next.Asked = counter.Asked;
						next.AskedModel = counter.AskedModel;
						Drop(counter);
						counter.Asked = false;
						counter.AskedModel = null;
					}
				}
				previous.Released = true;
				previous.Ui = false;
				Runs.Remove(previous);
			}
			Runs.Add(run);
			foreach (Node child in ((Node)val2).GetChildren(false))
			{
				NPower val3 = (NPower)(object)((child is NPower) ? child : null);
				if (val3 == null || !FxKit.Alive((GodotObject)(object)val3))
				{
					continue;
				}
				PowerModel val4 = ModelOf(val3);
				if (val4 == null)
				{
					continue;
				}
				run.Seen.Add(((GodotObject)val3).GetInstanceId());
				foreach (Counter counter2 in run.Counters)
				{
					if (counter2.Node == null && counter2.Kind.Is(val4))
					{
						Take(counter2, val3, counter2.Before == 0);
					}
				}
			}
			foreach (Counter counter3 in run.Counters)
			{
				if (counter3.Node != null && !counter3.Hidden)
				{
					Write(counter3, counter3.Before);
				}
			}
			if (!_ticking)
			{
				Ticker.AddLate(Update);
				_ticking = true;
			}
			return run;
		}

		internal static void Suspend(Run run)
		{
			if (!Current(run))
			{
				return;
			}
			foreach (Counter counter in run.Counters)
			{
				Count(run, counter, 0f);
			}
			run.Suspended = true;
		}

		public static void StepTo(Run run, int steps)
		{
			if (run != null && Current(run) && run.Ui)
			{
				run.Steps = Math.Max(run.Steps, steps);
			}
		}

		public static void Finish(Run run)
		{
			if (run == null)
			{
				return;
			}
			try
			{
				if (Current(run) && run.Ui)
				{
					foreach (Counter counter in run.Counters)
					{
						Count(run, counter, 0f);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("StatusSteps.Finish: " + ex);
			}
			try
			{
				Release(run);
			}
			catch (Exception ex2)
			{
				DevLog.Warn("StatusSteps.Finish (release): " + ex2);
				if (run == _run)
				{
					_run = null;
				}
			}
		}

		public static void Reset()
		{
			try
			{
				Run run = _run;
				_run = null;
				_covering = false;
				Run[] array = Runs.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					Release(array[i]);
				}
				Runs.Clear();
				if (run != null)
				{
					Release(run);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("StatusSteps.Reset: " + ex);
			}
		}

		public static bool HoldsAmount(NPower power)
		{
			if (_ours || power == null)
			{
				return false;
			}
			PowerModel val = ModelOf(power);
			Run run = ((val == null) ? null : For(val.Owner));
			if (run == null || !run.Ui)
			{
				return false;
			}
			for (int i = 0; i < run.Counters.Count; i++)
			{
				if (run.Counters[i].Node == power)
				{
					return true;
				}
			}
			if (val != null)
			{
				return Counted(val);
			}
			return false;
		}

		public static bool HoldRemoval(NPowerContainer row, PowerModel power)
		{
			Run run = ((power == null) ? null : For(power.Owner));
			if (run == null || !run.Ui || _ours || row == null || row != run.Row)
			{
				return false;
			}
			if (ExposeHooks.Removal == null)
			{
				return false;
			}
			foreach (Counter counter in run.Counters)
			{
				if (!counter.Asked && FxKit.Alive((GodotObject)(object)counter.Node) && counter.Kind.Is(power) && (ModelOf(counter.Node) == power || power.Owner == run.Target))
				{
					counter.Asked = true;
					counter.AskedModel = power;
					return true;
				}
			}
			return false;
		}

		public static bool CoversApplied(PowerModel power)
		{
			Run run = ((power == null) ? null : For(power.Owner));
			if (run != null && run.Ui)
			{
				return Counted(power);
			}
			return false;
		}

		public static bool CoversRemoved(PowerModel power)
		{
			return CoversApplied(power);
		}

		public static bool EnterApplied(NCreature node, PowerModel power)
		{
			Run run = ((power == null || node == null || !PlayerSettings.Enabled || !Counted(power)) ? null : For(node.Entity));
			if (run != null && node != run.Victim)
			{
				run = null;
			}
			if (run != null && power.Owner != run.Target)
			{
				run = null;
			}
			Windows.Push(run);
			_covering = run != null;
			return run != null;
		}

		public static void LeaveApplied()
		{
			if (Windows.Count > 0)
			{
				Windows.Pop();
			}
			_covering = Windows.Count > 0 && Windows.Peek() != null;
		}

		private static bool Counted(PowerModel power)
		{
			for (int i = 0; i < Kinds.Length; i++)
			{
				if (Kinds[i].Is(power))
				{
					return true;
				}
			}
			return false;
		}

		private static void Update(double realDt)
		{
			List<Run> list = new List<Run>(Runs);
			if (_run != null)
			{
				list.Add(_run);
			}
			if (list.Count == 0)
			{
				Ticker.Remove(Update);
				_ticking = false;
			}
			else
			{
				if (Ticker.Tree != null && Ticker.Tree.Paused)
				{
					return;
				}
				float num = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
				foreach (Run item in list)
				{
					try
					{
						if (item.Multi && (!MultiplayerFx.Active || !PlayerSettings.Enabled || (object)item.Room != GameRefs.CombatRoom))
						{
							Release(item);
							continue;
						}
						if (!item.Suspended)
						{
							item.Age += num;
						}
						if (item.Age >= item.MaxHold || !FxKit.Alive((GodotObject)(object)item.Victim) || !((Node)item.Victim).IsInsideTree() || !FxKit.Alive((GodotObject)(object)item.Row))
						{
							Release(item);
							continue;
						}
						Scan(item);
						if (item.Suspended)
						{
							continue;
						}
						foreach (Counter counter in item.Counters)
						{
							Count(item, counter, num);
						}
					}
					catch (Exception ex)
					{
						DevLog.Warn("StatusSteps: " + ex);
						try
						{
							Release(item);
						}
						catch
						{
							if (item == _run)
							{
								_run = null;
							}
							Runs.Remove(item);
						}
					}
				}
			}
		}

		private static void Scan(Run run)
		{
			foreach (Node child in ((Node)run.Row).GetChildren(false))
			{
				NPower val = (NPower)(object)((child is NPower) ? child : null);
				if (val == null || !FxKit.Alive((GodotObject)(object)val))
				{
					continue;
				}
				ulong instanceId = ((GodotObject)val).GetInstanceId();
				if (run.Seen.Contains(instanceId))
				{
					continue;
				}
				PowerModel val2 = ModelOf(val);
				if (val2 == null)
				{
					continue;
				}
				run.Seen.Add(instanceId);
				foreach (Counter counter in run.Counters)
				{
					if (counter.Node == null && counter.Kind.Is(val2))
					{
						Take(counter, val, fresh: true);
						break;
					}
				}
			}
		}

		private static void Count(Run run, Counter counter, float dt)
		{
			if (counter.Node != null && !FxKit.Alive((GodotObject)(object)counter.Node))
			{
				Drop(counter);
			}
			if (counter.Node == null)
			{
				return;
			}
			counter.Age += dt;
			if (counter.Loose && counter.Age < 0.6f)
			{
				Settle(counter);
			}
			int num = counter.Shown;
			int amount = 0;
			bool flag = counter.After.HasValue;
			if (flag)
			{
				amount = counter.After.Value;
			}
			else
			{
				flag = !run.Captured && (counter.Asked || TryRead(counter.Kind, run.Target, out amount));
			}
			if (flag)
			{
				int value = amount - counter.Before;
				num = counter.Before + Math.Sign(value) * Math.Min(run.Steps, Math.Abs(value));
			}
			if (counter.Hidden)
			{
				if (num == counter.Before)
				{
					Conceal(counter);
					return;
				}
				counter.Hidden = false;
				counter.PopAt = run.Age;
				Write(counter, num);
				Flash(counter);
			}
			else if (num != counter.Shown)
			{
				Write(counter, num);
				Flash(counter);
			}
			if (counter.PopAt >= 0f)
			{
				Pop(run, counter);
			}
			if (counter.Asked && num == 0)
			{
				LetGo(run, counter);
			}
		}

		private static void Take(Counter counter, NPower power, bool fresh)
		{
			counter.Node = power;
			counter.Label = ((Node)power).GetNodeOrNull<Label>(NodePath.op_Implicit("%AmountLabel"));
			counter.Icon = ((Node)power).GetNodeOrNull<Control>(NodePath.op_Implicit("%Icon"));
			counter.Age = 0f;
			counter.PopAt = -1f;
			if (fresh)
			{
				counter.Fresh = true;
				counter.Hidden = true;
				counter.Loose = !StopEntrance(power);
				Settle(counter);
				Conceal(counter);
			}
		}

		private static void Drop(Counter counter)
		{
			counter.Node = null;
			counter.Label = null;
			counter.Icon = null;
			counter.Hidden = false;
			counter.Fresh = false;
			counter.PopAt = -1f;
		}

		private static void Write(Counter counter, int value)
		{
			counter.Shown = value;
			if (!counter.Asked)
			{
				Refresh(counter);
			}
			if (FxKit.Alive((GodotObject)(object)counter.Label))
			{
				string text = value.ToString(CultureInfo.InvariantCulture);
				Label label = counter.Label;
				MegaLabel val = (MegaLabel)(object)((label is MegaLabel) ? label : null);
				if (val != null)
				{
					val.SetTextAutoSize(text);
				}
				else
				{
					counter.Label.Text = text;
				}
			}
		}

		private static void Flash(Counter counter)
		{
			if (_flash == null || !FxKit.Alive((GodotObject)(object)counter.Node))
			{
				return;
			}
			_ours = true;
			try
			{
				_flash.Invoke(counter.Node, null);
			}
			catch (Exception ex)
			{
				_flash = null;
				DevLog.Warn("StatusSteps: the icon could not be flashed (" + ex.Message + ")");
			}
			finally
			{
				_ours = false;
			}
		}

		private static void Refresh(Counter counter)
		{
			if (_refresh == null || !FxKit.Alive((GodotObject)(object)counter.Node))
			{
				return;
			}
			_ours = true;
			try
			{
				_refresh.Invoke(counter.Node, null);
			}
			catch (Exception ex)
			{
				DevLog.Warn("StatusSteps: the icon could not be refreshed (" + ex.Message + ")");
			}
			finally
			{
				_ours = false;
			}
		}

		private static void Conceal(Counter counter)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			Node node = (Node)(object)counter.Node;
			if (!FxKit.Alive((GodotObject)(object)node))
			{
				return;
			}
			CanvasItem val = (CanvasItem)(object)((node is CanvasItem) ? node : null);
			if (val != null)
			{
				Color modulate = val.Modulate;
				if (modulate.A > 0f)
				{
					val.Modulate = new Color(modulate.R, modulate.G, modulate.B, 0f);
				}
			}
		}

		private static void Pop(Run run, Counter counter)
		{
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Clamp((run.Age - counter.PopAt) / 0.16f, 0f, 1f);
			float num2 = 1f - (1f - num) * (1f - num);
			Node node = (Node)(object)counter.Node;
			if (FxKit.Alive((GodotObject)(object)node))
			{
				CanvasItem val = (CanvasItem)(object)((node is CanvasItem) ? node : null);
				if (val != null)
				{
					Color modulate = val.Modulate;
					val.Modulate = new Color(modulate.R, modulate.G, modulate.B, num2);
				}
			}
			Control icon = counter.Icon;
			if (FxKit.Alive((GodotObject)(object)icon))
			{
				icon.Position = new Vector2(icon.Position.X, -8f * (1f - num2));
			}
			if (num >= 1f)
			{
				counter.PopAt = -1f;
				Show(counter);
			}
		}

		private static void Settle(Counter counter)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			Control icon = counter.Icon;
			if (FxKit.Alive((GodotObject)(object)icon))
			{
				if (icon.Position.Y != 0f)
				{
					icon.Position = new Vector2(icon.Position.X, 0f);
				}
				Color modulate = ((CanvasItem)icon).Modulate;
				if (modulate.A < 1f)
				{
					((CanvasItem)icon).Modulate = new Color(modulate.R, modulate.G, modulate.B, 1f);
				}
			}
		}

		private static void Show(Counter counter)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			Settle(counter);
			Node node = (Node)(object)counter.Node;
			if (!FxKit.Alive((GodotObject)(object)node))
			{
				return;
			}
			CanvasItem val = (CanvasItem)(object)((node is CanvasItem) ? node : null);
			if (val != null)
			{
				Color modulate = val.Modulate;
				if (modulate.A < 1f)
				{
					val.Modulate = new Color(modulate.R, modulate.G, modulate.B, 1f);
				}
			}
		}

		private static bool StopEntrance(NPower power)
		{
			if (!_lookedAnim)
			{
				_lookedAnim = true;
				try
				{
					_animIn = AccessTools.FieldRefAccess<NPower, Tween>("_animInTween");
				}
				catch (Exception ex)
				{
					DevLog.Warn("StatusSteps: no _animInTween (" + ex.Message + ")");
				}
			}
			if (_animIn == null)
			{
				return false;
			}
			try
			{
				Tween val = _animIn.Invoke(power);
				if (val != null && val.IsValid())
				{
					val.Kill();
				}
				return true;
			}
			catch (Exception ex2)
			{
				_animIn = null;
				DevLog.Warn("StatusSteps: the icon's entrance could not be stopped (" + ex2.Message + ")");
				return false;
			}
		}

		private static void LetGo(Run run, Counter counter)
		{
			PowerModel askedModel = counter.AskedModel;
			counter.Asked = false;
			counter.AskedModel = null;
			Node node = (Node)(object)counter.Node;
			Drop(counter);
			MethodInfo removal = ExposeHooks.Removal;
			if (removal == null || askedModel == null || !FxKit.Alive((GodotObject)(object)run.Row) || !((Node)run.Row).IsInsideTree())
			{
				return;
			}
			CanvasItem val = null;
			if (FxKit.Alive((GodotObject)(object)node))
			{
				CanvasItem val2 = (CanvasItem)(object)((node is CanvasItem) ? node : null);
				if (val2 != null && val2.Visible)
				{
					val2.Visible = false;
					val = val2;
				}
			}
			_ours = true;
			try
			{
				removal.Invoke(run.Row, new object[1] { askedModel });
			}
			catch (Exception ex)
			{
				DevLog.Warn("StatusSteps: the power row could not be told that the power is gone: " + ex);
				if (FxKit.Alive((GodotObject)(object)val))
				{
					val.Visible = true;
				}
			}
			finally
			{
				_ours = false;
			}
		}

		private static void Release(Run run)
		{
			if (run == null || run.Released)
			{
				return;
			}
			run.Released = true;
			if (run == _run)
			{
				_run = null;
			}
			Runs.Remove(run);
			run.Ui = false;
			_covering = Windows.Count > 0 && Current(Windows.Peek()) && Windows.Peek().Ui;
			foreach (Counter counter in run.Counters)
			{
				try
				{
					if (counter.Node == null || !FxKit.Alive((GodotObject)(object)counter.Node))
					{
						Drop(counter);
						continue;
					}
					if (counter.Fresh)
					{
						Show(counter);
					}
					counter.PopAt = -1f;
					counter.Hidden = false;
					if (counter.Asked)
					{
						LetGo(run, counter);
					}
					else
					{
						Refresh(counter);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("StatusSteps.Release (" + counter.Kind.Name + "): " + ex.Message);
				}
			}
		}

		private static void LookMethods()
		{
			if (!_lookedMethods)
			{
				_lookedMethods = true;
				try
				{
					_refresh = StatusStepHooks.FindRefresh();
					_flash = StatusStepHooks.FindFlash();
				}
				catch (Exception ex)
				{
					DevLog.Warn("StatusSteps: " + ex.Message);
				}
				if (_refresh == null)
				{
					DevLog.Warn("StatusSteps: NPower.RefreshAmount was not found - Malaise's icons are left to the game");
				}
				if (_flash == null)
				{
					DevLog.Warn("StatusSteps: NPower.FlashPower was not found - Malaise's steps do not flash");
				}
			}
		}

		private static bool TryRead(Kind kind, Creature creature, out int amount)
		{
			amount = 0;
			try
			{
				if (creature == null)
				{
					return false;
				}
				amount = kind.Read(creature);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static PowerModel ModelOf(NPower power)
		{
			try
			{
				return power.Model;
			}
			catch
			{
				return null;
			}
		}

		private static T Find<T>(Node root, int depth) where T : class
		{
			if (!FxKit.Alive((GodotObject)(object)root))
			{
				return null;
			}
			foreach (Node child in root.GetChildren(false))
			{
				if (!FxKit.Alive((GodotObject)(object)child))
				{
					continue;
				}
				if (child is T result)
				{
					return result;
				}
				if (depth > 0)
				{
					T val = Find<T>(child, depth - 1);
					if (val != null)
					{
						return val;
					}
				}
			}
			return null;
		}
	}
