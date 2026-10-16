// namespace: SilentFX.Dev
// type: TourController (class)
// source lines: 27336-28314
	public static class TourController
	{
		private static readonly Key[] Keys;

		private static readonly Dictionary<Key, bool> Held;

		private static readonly List<TaskCompletionSource<bool>> Waiters;

		private static TourDocument _document;

		private static RunState _run;

		private static CancellationTokenSource _cancel;

		private static bool _initialized;

		private static bool _starting;

		private static bool _working;

		private static bool _exiting;

		private static bool _inputWasAllowed;

		private static int _index;

		private static int _retry;

		private static double _toastTime;

		private static double _healTime;

		private static ICombatState _finaleState;

		private static bool _finaleWon;

		private static bool _finaleShown;

		private static double _finaleWait;

		private static CanvasLayer _shieldLayer;

		private static Control _shield;

		internal static bool Enabled => _initialized;

		public static bool Active { get; private set; }

		public static bool Exclusive
		{
			get
			{
				if (!Active)
				{
					return _starting;
				}
				return true;
			}
		}

		public static bool Busy
		{
			get
			{
				if (Exclusive)
				{
					return _working;
				}
				return false;
			}
		}

		internal static bool BlockingInput
		{
			get
			{
				if (Active && !FinaleFinished && _shieldLayer != null)
				{
					return !ChoiceOpen();
				}
				return false;
			}
		}

		private static bool FinaleFinished
		{
			get
			{
				if (!_finaleWon)
				{
					if (_finaleState != null)
					{
						if (_finaleState.Enemies.Any((Creature e) => e.IsAlive))
						{
							ICombatState finaleState = _finaleState;
							CombatManager instance = CombatManager.Instance;
							if ((object)finaleState == ((instance != null) ? instance.DebugOnlyGetState() : null))
							{
								return CombatManager.Instance.IsOverOrEnding;
							}
							return false;
						}
						return true;
					}
					return false;
				}
				return true;
			}
		}

		public static void Init()
		{
			try
			{
				if (!_initialized && (Tuning.I("tour.on", 0) != 0 || File.Exists(Path.Combine(DevLog.ModDir, "dev_preview.txt"))))
				{
					_initialized = true;
					Ticker.Add(Update);
					DevLog.Info("Tour: ready; Ctrl+Home start, Ctrl+End exit; Right/PageDown next, Left/PageUp previous, Home replay, End captions");
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: init failed: " + ex);
			}
		}

		public static async void Start()
		{
			if (!_initialized)
			{
				return;
			}
			bool acquired = false;
			try
			{
				if (Exclusive || _working)
				{
					return;
				}
				Tuning.Load();
				string error = Rejection();
				if (error != null)
				{
					Report(error);
					return;
				}
				if (Showcase.Active || Showcase.Restoring)
				{
					Report("请先用 F4 退出自动导览，并等还原完成");
					return;
				}
				if (TourScene.Restoring)
				{
					Report("上一次展示还在把化合物X还原，稍等一下再开");
					return;
				}
				if (!TourPatches.VerifyInstalled(out error))
				{
					Report("展示补丁没有装齐：" + error);
					return;
				}
				if (!TourScript.TryReload(null, out var document, out error))
				{
					Report(error);
					return;
				}
				_run = RunManager.Instance.DebugOnlyGetState();
				_starting = (_working = true);
				acquired = true;
				_exiting = false;
				_cancel?.Dispose();
				_cancel = new CancellationTokenSource();
				DevPreview.SuspendForTour();
				TourUi.Open();
				TourUi.SetBusy(busy: true);
				if (TestBench.Active || TestBench.Working)
				{
					DevLog.Info("Tour: waiting for TestBench's own exit/restore");
					TestBench.RequestTourExit();
					await WaitUntil(() =>
					{
						if (TestBench.Active && !TestBench.TourExitPending && !TestBench.Working)
						{
							throw new InvalidOperationException("测试面板还原失败，请先手动退出测试面板");
						}
						return !TestBench.Active && !TestBench.Working;
					}, _cancel.Token);
				}
				_cancel.Token.ThrowIfCancellationRequested();
				error = Rejection();
				if (error != null || _run != RunManager.Instance.DebugOnlyGetState())
				{
					throw new InvalidOperationException(error ?? "开始展示前已离开原来的这一局");
				}
				_document = document;
				TourScene.NoteStart(LocalContext.GetMe((IPlayerCollection)(object)_run));
				_index = (_retry = -1);
				_finaleState = null;
				_finaleWon = (_finaleShown = false);
				Active = true;
				TourUi.SetCaptionsVisible(visible: true);
				TourUi.ShowIntro(document.Intro);
				DevLog.Info($"Tour: started in profile {SaveManager.Instance.CurrentProfileId}; intro shown; no room entered yet");
			}
			catch (OperationCanceledException)
			{
				DevLog.Info("Tour: start cancelled");
			}
			catch (Exception ex2)
			{
				DevLog.Warn("Tour: start failed: " + ex2);
				if (!_exiting)
				{
					Report("展示未开始：" + ex2.Message);
				}
			}
			finally
			{
				if (acquired)
				{
					_starting = (_working = false);
					TourUi.SetBusy(busy: false);
					if (_exiting)
					{
						FinishExit();
					}
					else if (!Active)
					{
						_run = null;
						_cancel?.Cancel();
						_toastTime = 6.0;
					}
				}
			}
		}

		public static void Next()
		{
			try
			{
				if (Active && !_working && !_exiting && !FinaleFinished && (!TourUi.PageUp || !TourUi.Advance()) && _index <= _document.Stops.Count && (_index != _document.Stops.Count || _document.Finale != null))
				{
					Navigate(_index + 1);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: next failed: " + ex);
				Report(ex.Message);
			}
		}

		public static void Previous()
		{
			try
			{
				if (Active && !_working && !_exiting && _index >= 0 && !FinaleFinished)
				{
					Navigate(_index - 1);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: previous failed: " + ex);
				Report(ex.Message);
			}
		}

		public static void Replay()
		{
			try
			{
				if (Active && !_working && !_exiting && !FinaleFinished)
				{
					Navigate((_retry >= 0) ? _retry : _index);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: replay failed: " + ex);
				Report(ex.Message);
			}
		}

		internal static void NoteCardPlayed(CardModel card)
		{
			try
			{
				if (Active && !_working && !_exiting && _document != null && _index >= 0 && _index < _document.Stops.Count && card != null && LocalContext.IsMine(card))
				{
					float slow = _document.Stops[_index].Slow;
					if (slow != 0f)
					{
						TourSlow.Begin(slow);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: slow motion trigger failed: " + ex);
			}
		}

		internal static void NoteCardFinished()
		{
			try
			{
				TourSlow.Release();
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: slow motion release failed: " + ex);
			}
		}

		public static void Exit()
		{
			try
			{
				if (Exclusive && !_exiting)
				{
					_exiting = true;
					TourSlow.End();
					_cancel?.Cancel();
					TourTurnPlan.Cancel();
					TourUi.Close();
					DevLog.Info("Tour: exit requested; remaining commands cancelled, current room will be kept");
					if (!_working)
					{
						FinishExit();
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: exit failed: " + ex);
			}
		}

		private static string Rejection()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Invalid comparison between Unknown and I4
			RunManager instance = RunManager.Instance;
			if (instance == null || !instance.IsInProgress)
			{
				return "需要先进入一局猎人的测试存档";
			}
			INetGameService netService = instance.NetService;
			if (netService == null || (int)netService.Type != 1)
			{
				return "展示只允许单人模式";
			}
			RunState val = instance.DebugOnlyGetState();
			if (val == null || val.Players.Count != 1)
			{
				return "展示要求这一局只有一位玩家";
			}
			Player me = LocalContext.GetMe((IPlayerCollection)(object)val);
			if (!(((me != null) ? me.Character : null) is Silent))
			{
				return "本地玩家必须是猎人";
			}
			if (!me.Creature.IsAlive)
			{
				return "玩家已死亡，不能开始展示";
			}
			CombatManager instance2 = CombatManager.Instance;
			if (instance2 != null && instance2.IsInProgress)
			{
				CombatState val2 = instance2.DebugOnlyGetState();
				if (val2 == null || val2.Players.Count != 1)
				{
					return "当前战斗必须只有一位玩家";
				}
			}
			int num = Tuning.I("tour.profile", 3);
			switch (num)
			{
			default:
				return "tour.profile 应为 0、1、2 或 3";
			case 1:
			case 2:
			case 3:
				if (SaveManager.Instance.CurrentProfileId != num)
				{
					return $"展示只允许第 {num} 号存档位；当前是第 {SaveManager.Instance.CurrentProfileId} 号";
				}
				break;
			case 0:
				break;
			}
			return null;
		}

		private static async void Navigate(int target)
		{
			bool acquired = false;
			bool enteredScene = false;
			try
			{
				if (!Active || _working || _exiting || FinaleFinished)
				{
					return;
				}
				bool flag = target > _document.Stops.Count;
				bool flag2 = target == _document.Stops.Count;
				if (!TourScript.TryReload(_document, out var next, out var error))
				{
					Report(error);
					return;
				}
				error = Rejection();
				if (error != null)
				{
					Report(error);
					Exit();
					return;
				}
				bool working;
				acquired = (working = true);
				_working = working;
				TourSlow.End();
				_cancel?.Cancel();
				_cancel?.Dispose();
				_cancel = new CancellationTokenSource();
				TourTurnPlan.Cancel();
				CancellationToken cancel = _cancel.Token;
				if (flag)
				{
					target = next.Stops.Count + 1;
				}
				else if (flag2)
				{
					target = next.Stops.Count;
				}
				TourUi.SetBusy(busy: true);
				Check(cancel);
				if (_finaleState != null)
				{
					await WaitUntil(() => FinaleFinished || (TourScene.Ready() && InputAllowed()), cancel);
					Check(cancel);
					if (FinaleFinished)
					{
						return;
					}
				}
				_document = next;
				if (target < 0)
				{
					_finaleState = null;
					_index = (_retry = -1);
					TourUi.ShowIntro(next.Intro);
					DevLog.Info("Tour: intro shown");
					return;
				}
				bool finale = target > next.Stops.Count && next.Finale != null;
				if (target >= next.Stops.Count && !finale)
				{
					_finaleState = null;
					_index = next.Stops.Count;
					_retry = -1;
					TourUi.ShowOutro(next.Outro);
					DevLog.Info("Tour: outro shown");
					return;
				}
				await WaitUntil(() =>
				{
					CombatManager instance = CombatManager.Instance;
					return (instance == null || !instance.IsInProgress || TourScene.Ready()) && InputAllowed();
				}, cancel);
				Check(cancel);
				if (!FinaleFinished)
				{
					Shield(shown: true);
					_retry = target;
					enteredScene = true;
					_finaleState = null;
					if (finale)
					{
						TourUi.ShowNothing();
					}
					await TourScene.Prepare(finale ? next.Finale : next.Stops[target], cancel);
					Check(cancel);
					_index = target;
					_retry = -1;
					if (finale)
					{
						_finaleState = (ICombatState)(object)CombatManager.Instance.DebugOnlyGetState();
						DevLog.Info("Tour: finale ready; enemies unprotected, player protected");
					}
					else
					{
						TourUi.ShowStop(next.Stops[target].Text, target, next.Stops.Count);
					}
				}
			}
			catch (OperationCanceledException)
			{
				DevLog.Info("Tour: navigation cancelled");
			}
			catch (Exception ex2)
			{
				TourTurnPlan.Cancel();
				DevLog.Warn("Tour: navigation failed: " + ex2);
				if (!_exiting)
				{
					if (enteredScene)
					{
						TourUi.SetCaptionsVisible(visible: false);
					}
					Report((enteredScene ? "摆站失败，场面可能已改变；Home 重试，End 显示字幕。" : "切站失败，仍在当前场面。") + ex2.Message);
				}
			}
			finally
			{
				if (acquired)
				{
					Shield(shown: false);
					_working = false;
					TourUi.SetBusy(busy: false);
					if (_exiting)
					{
						FinishExit();
					}
				}
			}
		}

		private static void FinishExit()
		{
			TourScene.RestoreRelics(_run);
			TourSlow.End();
			TourTurnPlan.Cancel();
			Shield(shown: false);
			TourUi.Close();
			Active = (_starting = (_working = (_exiting = false)));
			_finaleState = null;
			_finaleWon = (_finaleShown = false);
			_run = null;
			_index = (_retry = -1);
			_cancel?.Dispose();
			_cancel = null;
			DevLog.Info("Tour: exited; tool exclusivity and protections released; current fight retained");
		}

		private static void Update(double dt)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Invalid comparison between Unknown and I4
			try
			{
				TaskCompletionSource<bool>[] array = Waiters.ToArray();
				Waiters.Clear();
				TaskCompletionSource<bool>[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].TrySetResult(result: true);
				}
				HashSet<Key> hashSet = new HashSet<Key>();
				Key[] keys = Keys;
				foreach (Key val in keys)
				{
					bool flag = Input.IsKeyPressed(val);
					if (flag && !Held.GetValueOrDefault(val))
					{
						hashSet.Add(val);
					}
					Held[val] = flag;
				}
				if (Exclusive)
				{
					RunManager instance = RunManager.Instance;
					if (instance != null && instance.IsInProgress && _run == RunManager.Instance.DebugOnlyGetState())
					{
						INetGameService netService = RunManager.Instance.NetService;
						if (netService != null && (int)netService.Type == 1)
						{
							goto IL_00d2;
						}
					}
					Exit();
				}
				goto IL_00d2;
				IL_0165:
				int num = (InputAllowed() ? 1 : 0);
				goto IL_016d;
				IL_00d2:
				UpdateFinale();
				if (FxKit.Alive((GodotObject)(object)_shield))
				{
					_shield.Size = FxKit.ViewSize();
					_shield.MouseFilter = (MouseFilterEnum)(ChoiceOpen() ? 2 : 0);
				}
				if (!Exclusive && _toastTime > 0.0)
				{
					_toastTime -= dt;
					if (_toastTime <= 0.0)
					{
						TourUi.Close();
					}
				}
				int num2;
				if (!Exclusive)
				{
					if (hashSet.Count > 0)
					{
						num2 = (Input.IsKeyPressed((Key)4194326) ? 1 : 0);
						if (num2 != 0)
						{
							goto IL_0165;
						}
					}
					else
					{
						num2 = 0;
					}
					num = 0;
					goto IL_016d;
				}
				num2 = 1;
				goto IL_0165;
				IL_016d:
				bool flag2 = (byte)num != 0;
				if (num2 == 0)
				{
					_inputWasAllowed = true;
				}
				else
				{
					if (!flag2 || !_inputWasAllowed)
					{
						hashSet.Clear();
					}
					_inputWasAllowed = flag2;
				}
				if (flag2)
				{
					bool flag3 = Input.IsKeyPressed((Key)4194326);
					if (flag3 && hashSet.Contains((Key)4194318))
					{
						Exit();
						return;
					}
					if (flag3 && hashSet.Contains((Key)4194317))
					{
						Start();
						return;
					}
					if (Active && !flag3 && !_exiting && !FinaleFinished)
					{
						if (hashSet.Contains((Key)4194318))
						{
							TourUi.SetCaptionsVisible(!TourUi.CaptionsVisible);
						}
						else if (hashSet.Contains((Key)4194317))
						{
							Replay();
						}
						else if (hashSet.Contains((Key)4194324) || hashSet.Contains((Key)4194321))
						{
							Next();
						}
						else if (hashSet.Contains((Key)4194323) || hashSet.Contains((Key)4194319))
						{
							Previous();
						}
					}
				}
				_healTime -= dt;
				if (Active && !_working && !_exiting && !FinaleFinished && _healTime <= 0.0 && TourScene.Ready())
				{
					_healTime = 0.5;
					Creature creature = LocalContext.GetMe((ICombatState)(object)CombatManager.Instance.DebugOnlyGetState()).Creature;
					if (creature.CurrentHp < creature.MaxHp)
					{
						Heal(creature);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: update failed: " + ex);
				Exit();
			}
		}

		private static void UpdateFinale()
		{
			if (!Active || _exiting || !FinaleFinished || _finaleShown)
			{
				return;
			}
			if (!_finaleWon)
			{
				_finaleWon = true;
				_cancel?.Cancel();
				TourTurnPlan.Cancel();
				Shield(shown: false);
				TourUi.SetBusy(busy: false);
				float num = Tuning.F("tour.finale_wait", 1.6f);
				_finaleWait = (float.IsFinite(num) ? ((double)Math.Max(0f, num)) : 1.6);
				DevLog.Info($"Tour: finale won; input released; final line in {_finaleWait:0.###} game seconds");
			}
			else if (Ticker.Tree != null && !Ticker.Tree.Paused)
			{
				double processDeltaTime = ((Node)Ticker.Tree.Root).GetProcessDeltaTime();
				if (double.IsFinite(processDeltaTime) && processDeltaTime > 0.0)
				{
					_finaleWait -= processDeltaTime;
				}
				if (!(_finaleWait > 0.0))
				{
					_finaleShown = true;
					TourUi.ShowFinale(_document.FinaleText);
					DevLog.Info("Tour: finale line shown: " + _document.FinaleText);
				}
			}
		}

		private static async void Heal(Creature me)
		{
			_working = true;
			try
			{
				Check(_cancel.Token, me.CombatState);
				await CreatureCmd.SetCurrentHp(me, (decimal)me.MaxHp);
				DevLog.Info("Tour: player healed at idle player turn");
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex2)
			{
				DevLog.Warn("Tour: player heal failed: " + ex2);
			}
			finally
			{
				_working = false;
				if (_exiting)
				{
					FinishExit();
				}
			}
		}

		internal static bool InputAllowed()
		{
			try
			{
				if (!NGame.IsGameFocusedWindow() || RuntimeApiCompatibility.IsConsoleVisible)
				{
					return false;
				}
				SceneTree tree = Ticker.Tree;
				Control val = ((tree != null) ? ((Viewport)tree.Root).GuiGetFocusOwner() : null);
				if (val is LineEdit || val is TextEdit)
				{
					return false;
				}
				if (!FinaleFinished && ChoiceOpen())
				{
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: input gate failed: " + ex.Message);
				return false;
			}
		}

		private static bool ChoiceOpen()
		{
			NOverlayStack instance = NOverlayStack.Instance;
			if (instance == null || instance.ScreenCount <= 0)
			{
				NPlayerHand instance2 = NPlayerHand.Instance;
				if (instance2 == null)
				{
					return false;
				}
				return instance2.IsInCardSelection;
			}
			return true;
		}

		internal static bool CaptureArrow(InputEvent inputEvent)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Invalid comparison between Unknown and I8
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Invalid comparison between Unknown and I8
			try
			{
				if (Active && !FinaleFinished)
				{
					InputEventKey val = (InputEventKey)(object)((inputEvent is InputEventKey) ? inputEvent : null);
					if (val != null && ((long)val.Keycode == 4194319 || (long)val.Keycode == 4194321))
					{
						if (!NGame.IsGameFocusedWindow() || RuntimeApiCompatibility.IsConsoleVisible)
						{
							return false;
						}
						SceneTree tree = Ticker.Tree;
						Control val2 = ((tree != null) ? ((Viewport)tree.Root).GuiGetFocusOwner() : null);
						return !(val2 is LineEdit) && !(val2 is TextEdit);
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: arrow gate failed: " + ex);
				return false;
			}
		}

		internal static bool Protect(Creature creature)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Invalid comparison between Unknown and I4
			try
			{
				if (Active && !_exiting && creature != null && _run != null)
				{
					INetGameService netService = RunManager.Instance.NetService;
					if (netService != null && (int)netService.Type == 1 && _run == RunManager.Instance.DebugOnlyGetState())
					{
						CombatManager instance = CombatManager.Instance;
						CombatState val = instance.DebugOnlyGetState();
						if (!instance.IsInProgress || val == null || val.Players.Count != 1 || (object)creature.CombatState != val)
						{
							return false;
						}
						Creature val2 = creature;
						Player me = LocalContext.GetMe((ICombatState)(object)val);
						if (val2 == ((me != null) ? me.Creature : null))
						{
							return true;
						}
						if ((object)val == _finaleState)
						{
							return false;
						}
						return val.Enemies.Contains(creature) && !val.Enemies.Any((Creature e) => e != creature && e.IsAlive);
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tour: protection check failed: " + ex);
				return false;
			}
		}

		internal static void Check(CancellationToken cancel, ICombatState state = null)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Invalid comparison between Unknown and I4
			cancel.ThrowIfCancellationRequested();
			if (Active && !_exiting && _run == RunManager.Instance.DebugOnlyGetState())
			{
				INetGameService netService = RunManager.Instance.NetService;
				if (netService != null && (int)netService.Type == 1 && (state == null || (!CombatManager.Instance.IsOverOrEnding && (object)state == CombatManager.Instance.DebugOnlyGetState())))
				{
					return;
				}
			}
			throw new OperationCanceledException("Tour no longer owns this run/combat", cancel);
		}

		internal static async Task WaitUntil(Func<bool> ready, CancellationToken cancel)
		{
			try
			{
				Stopwatch watch = Stopwatch.StartNew();
				double timeout = Math.Max(5f, Tuning.F("tour.wait_seconds", 60f));
				while (true)
				{
					cancel.ThrowIfCancellationRequested();
					if (ready())
					{
						return;
					}
					if (watch.Elapsed.TotalSeconds > timeout)
					{
						break;
					}
					TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
					Waiters.Add(taskCompletionSource);
					await taskCompletionSource.Task;
				}
				throw new TimeoutException("等待游戏空闲超时，请完成当前操作后重试");
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				DevLog.Warn("Tour: wait failed: " + ex2.Message);
				throw;
			}
		}

		private static void Shield(bool shown)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Expected Obj, but got Unknown
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Expected Obj, but got Unknown
			if (!shown)
			{
				if (FxKit.Alive((GodotObject)(object)_shieldLayer))
				{
					((Node)_shieldLayer).QueueFree();
				}
				_shield = null;
				_shieldLayer = null;
			}
			else if (!FxKit.Alive((GodotObject)(object)_shieldLayer))
			{
				_shieldLayer = new CanvasLayer
				{
					Name = StringName.op_Implicit("SilentFX_TourInputLock"),
					Layer = 110
				};
				_shield = new Control
				{
					Name = StringName.op_Implicit("InputLock"),
					Size = FxKit.ViewSize(),
					MouseFilter = (MouseFilterEnum)0,
					FocusMode = (FocusModeEnum)2
				};
				((Node)Ticker.Tree.Root).AddChild((Node)(object)_shieldLayer, false, (InternalMode)0);
				((Node)_shieldLayer).AddChild((Node)(object)_shield, false, (InternalMode)0);
				_shield.GrabFocus();
			}
		}

		private static void Report(string reason)
		{
			DevLog.Warn("Tour: " + reason);
			TourUi.Open();
			TourUi.Toast(reason);
			if (!Exclusive)
			{
				_toastTime = 6.0;
			}
		}

		static TourController()
		{
			Key[] array = new Key[6];
			RuntimeHelpers.InitializeArray(array, __ldtoken(<PrivateImplementationDetails>.2F09B987B8E38235284EF1A869A65873370D55A57991E5731F47C1877DF03666));
			Keys = array;
			Held = new Dictionary<Key, bool>();
			Waiters = new List<TaskCompletionSource<bool>>();
			_index = -1;
			_retry = -1;
		}
	}
