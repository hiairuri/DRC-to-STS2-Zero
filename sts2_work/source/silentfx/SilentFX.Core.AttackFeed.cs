// namespace: SilentFX.Core
// type: AttackFeed (class)
// source lines: 32163-32529
	public static class AttackFeed
	{
		internal sealed class Flight
		{
			internal AttackCommand Command;

			internal AttackInfo Info;

			internal Task Completion;

			internal bool Started;

			internal bool Closed;

			internal ulong LastProgress;

			internal ulong WallStart;

			internal ulong LastDamage;

			internal readonly List<ulong> Gaps = new List<ulong>();
		}

		internal sealed class ExecuteScope
		{
			internal Flight Previous;

			internal Flight Flight;
		}

		internal sealed class DamageScope
		{
			internal DamageScope Previous;

			internal Flight Execution;

			internal Flight Owner;

			internal bool Landed;
		}

		private const ulong StaleMs = 8000uL;

		private static readonly List<Flight> Active = new List<Flight>();

		private static readonly AsyncLocal<Flight> Executing = new AsyncLocal<Flight>();

		private static readonly AsyncLocal<DamageScope> Damaging = new AsyncLocal<DamageScope>();

		private static int _mainThread;

		private static bool _ready;

		private static bool OnMain => Environment.CurrentManagedThreadId == _mainThread;

		public static event Action<AttackInfo> Began;

		public static event Action<AttackInfo, Creature, float> Aimed;

		public static event Action<AttackInfo> Ended;

		internal static void Configure(bool ready)
		{
			_mainThread = Environment.CurrentManagedThreadId;
			_ready = ready;
		}

		public static AttackInfo Current(CardModel card)
		{
			if (!OnMain || card == null)
			{
				return null;
			}
			for (int num = Active.Count - 1; num >= 0; num--)
			{
				if (Active[num].Info.Card == card)
				{
					return Active[num].Info;
				}
			}
			return null;
		}

		internal static AttackInfo CurrentDamage(CardModel card)
		{
			if (!OnMain || card == null)
			{
				return null;
			}
			Flight flight = Damaging.Value?.Owner;
			if (flight == null || !flight.Started || flight.Closed || flight.Info.Card != card)
			{
				return null;
			}
			return flight.Info;
		}

		internal static ExecuteScope Enter(AttackCommand command)
		{
			if (!_ready || !OnMain)
			{
				return null;
			}
			ExecuteScope executeScope = new ExecuteScope
			{
				Previous = Executing.Value
			};
			AbstractModel modelSource = command.ModelSource;
			CardModel val = (CardModel)(object)((modelSource is CardModel) ? modelSource : null);
			if (val != null && (LocalContext.IsMine(val) || (MultiplayerFx.Active && val is Ricochet)))
			{
				executeScope.Flight = new Flight
				{
					Command = command,
					Info = new AttackInfo
					{
						Card = val,
						Attacker = command.Attacker,
						Random = command.IsRandomlyTargeted,
						Invocation = (MultiplayerFx.Active ? MultiplayerFx.CurrentInvocation : null)
					}
				};
			}
			Executing.Value = executeScope.Flight;
			return executeScope;
		}

		internal static void Begin(AttackCommand command, decimal count)
		{
			if (OnMain)
			{
				Flight value = Executing.Value;
				if (value != null && !value.Started && !value.Closed && value.Command == command)
				{
					value.Info.PlannedHits = (int)Math.Clamp(decimal.Ceiling(count), 0m, 2147483647m);
					value.Info.BeganAtMs = (value.LastProgress = FxClock.Milliseconds);
					value.WallStart = Time.GetTicksMsec();
					value.Started = true;
					Active.Add(value);
					Ticker.Add(Tick);
					Raise(Began, value.Info, "Began");
				}
			}
		}

		internal static void Returned(ExecuteScope scope, Task task)
		{
			if (OnMain && scope?.Flight != null)
			{
				Flight flight = scope.Flight;
				flight.Completion = task;
				if (task == null || task.IsCompleted)
				{
					Finish(flight, CompletionReason(task));
				}
			}
		}

		internal static void Leave(ExecuteScope scope, Exception exception)
		{
			if (scope == null)
			{
				return;
			}
			try
			{
				if (OnMain && exception != null)
				{
					Finish(scope.Flight, "Execute threw");
				}
			}
			finally
			{
				Executing.Value = scope.Previous;
			}
		}

		internal static DamageScope EnterDamage(Creature dealer, CardModel card)
		{
			if (!_ready || !OnMain)
			{
				return null;
			}
			Flight value = Executing.Value;
			DamageScope value2 = Damaging.Value;
			if (value == null && value2 == null)
			{
				return null;
			}
			DamageScope damageScope = new DamageScope
			{
				Previous = value2,
				Execution = value
			};
			if (value != null && value.Started && !value.Closed && value2?.Execution != value && value.Info.Card == card && value.Info.Attacker == dealer)
			{
				damageScope.Owner = value;
			}
			Damaging.Value = damageScope;
			return damageScope;
		}

		internal static void LeaveDamage(DamageScope scope)
		{
			if (scope != null)
			{
				Damaging.Value = scope.Previous;
			}
		}

		internal static void DamageGiven(Creature dealer, CardModel card, DamageResult result)
		{
			if (!OnMain || result == null)
			{
				return;
			}
			DamageScope value = Damaging.Value;
			Flight flight = value?.Owner;
			if (flight != null && !flight.Closed && !value.Landed && flight.Info.Card == card && flight.Info.Attacker == dealer)
			{
				value.Landed = true;
				ulong ticksMsec = Time.GetTicksMsec();
				if (flight.Info.Landed != 0)
				{
					flight.Gaps.Add(ticksMsec - flight.LastDamage);
				}
				flight.LastDamage = ticksMsec;
				flight.LastProgress = FxClock.Milliseconds;
				flight.Info.Landed++;
			}
		}

		internal static void AfterAttack(AttackCommand command)
		{
			if (!OnMain)
			{
				return;
			}
			for (int num = Active.Count - 1; num >= 0; num--)
			{
				if (Active[num].Command == command)
				{
					Finish(Active[num], "AfterAttack");
					break;
				}
			}
		}

		internal static void Reset()
		{
			if (OnMain && Active.Count != 0)
			{
				Flight[] array = Active.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					Finish(array[i], "combat/room ended");
				}
			}
		}

		private static void Tick(double _)
		{
			int count = Active.Count;
			if (count == 0)
			{
				return;
			}
			Flight[] array = ArrayPool<Flight>.Shared.Rent(count);
			try
			{
				ulong milliseconds = FxClock.Milliseconds;
				Active.CopyTo(array);
				for (int i = 0; i < count; i++)
				{
					Flight flight = array[i];
					Task completion = flight.Completion;
					if (completion != null && completion.IsCompleted)
					{
						Finish(flight, CompletionReason(flight.Completion));
					}
					else if (milliseconds - flight.LastProgress >= 8000)
					{
						Finish(flight, "stale timeout");
					}
				}
			}
			catch (Exception e)
			{
				Warn("cleanup", e);
			}
			finally
			{
				ArrayPool<Flight>.Shared.Return(array, clearArray: true);
			}
		}

		private static string CompletionReason(Task task)
		{
			if (task == null || !task.IsFaulted)
			{
				if (task == null || !task.IsCanceled)
				{
					return "Execute completed";
				}
				return "Execute canceled";
			}
			return "Execute faulted";
		}

		private static void Finish(Flight f, string reason)
		{
			if (f == null || f.Closed)
			{
				return;
			}
			f.Closed = true;
			if (f.Started)
			{
				Active.Remove(f);
				if (Active.Count == 0)
				{
					Ticker.Remove(Tick);
				}
				try
				{
					DevLog.Info($"AttackFeed: {((object)f.Info.Card).GetType().Name} planned={f.Info.PlannedHits} landed={f.Info.Landed} durationMs={Time.GetTicksMsec() - f.WallStart} gapsMs=[{string.Join(",", f.Gaps)}] end={reason}");
				}
				catch (Exception e)
				{
					Warn("log", e);
				}
				Raise(Ended, f.Info, "Ended");
			}
		}

		private static void Raise(Action<AttackInfo> handlers, AttackInfo info, string name)
		{
			if (handlers == null)
			{
				return;
			}
			Delegate[] invocationList = handlers.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				Action<AttackInfo> action = (Action<AttackInfo>)invocationList[i];
				try
				{
					action(info);
				}
				catch (Exception e)
				{
					Warn(name + " subscriber", e);
				}
			}
		}

		internal static void Warn(string where, Exception e)
		{
			try
			{
				DevLog.Warn("AttackFeed: " + where + ": " + e);
			}
			catch
			{
			}
		}
	}
