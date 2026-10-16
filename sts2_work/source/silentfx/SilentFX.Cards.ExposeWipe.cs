// namespace: SilentFX.Cards
// type: ExposeWipe (class)
// source lines: 52668-54633
	internal static class ExposeWipe
	{
		internal sealed class Obstacle
		{
			public bool Gear;

			public float X;

			public float Width;

			public float Top;

			public float Bottom;
		}

		internal sealed class Zone
		{
			public float Left;

			public float Right;

			public float Bottom;

			public readonly List<Obstacle> Obstacles = new List<Obstacle>();
		}

		internal sealed class Touched
		{
			public CanvasItem Item;

			public ShaderMaterial Mat;

			public Material Was;

			public bool Hid;
		}

		internal sealed class Run
		{
			public bool Remote;

			public bool Multiplayer;

			public Node Room;

			public object Invocation;

			public bool Cancelled;

			public Creature Target;

			public Creature Caster;

			public Action<Run> Begin;

			public bool Started;

			public bool Opened;

			public bool Ended;

			public bool Ui;

			public float Age;

			public float PassAge;

			public float Wait;

			public float Land;

			public float MaxHold;

			public bool Slashed;

			public float SlashAt;

			public bool Landed;

			public int SoreAmount;

			public NCreature Victim;

			public NHealthBar Bar;

			public Control BarBox;

			public NPowerContainer Row;

			public Control Shield;

			public CanvasItem Outline;

			public bool HoldBar;

			public bool ShieldPassed;

			public float ShieldTop = -1000000f;

			public ShaderMaterial ShieldMat;

			public readonly List<Touched> ShieldTouched = new List<Touched>();

			public NPower Gear;

			public Control GearIcon;

			public PowerModel GearModel;

			public bool HoldGear;

			public bool Asked;

			public PowerModel AskedModel;

			public float GearTop = -1000000f;

			public float GearLeft = -1f;

			public ShaderMaterial GearMat;

			public readonly List<Touched> GearTouched = new List<Touched>();

			public bool HadSore;

			public NPower Sore;

			public Control SoreIcon;

			public int SoreStage;

			public bool SoreSet;

			public bool SoreOwn;

			public bool SoreLoose;

			public float SoreCut;

			public float SoreClock;

			public float SoreAge;

			public ShaderMaterial SoreMat;

			public readonly List<Touched> SoreTouched = new List<Touched>();

			public readonly HashSet<ulong> Seen = new HashSet<ulong>();

			public bool Pinned;

			public Vector2 PinHome;

			public Vector2 PinAt;

			public bool HasLine;

			public float LineY = 1000000f;

			public float Shake;

			public bool Moved;

			public Transform2D ToUi = Transform2D.Identity;

			public Run()
			{
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			}
		}

		private const float Far = 1000000f;

		private const float Band = 7f;

		private const float GearRest = 0.05f;

		private const float OwnWipe = 0.12f;

		private const float PulseTime = 0.3f;

		private const string Slash = "vfx/vfx_flying_slash";

		private static readonly List<Run> _runs = new List<Run>();

		private static bool _ticking;

		private static readonly Stack<Run> _applied = new Stack<Run>();

		private static FieldRef<NPower, Tween> _animIn;

		private static bool _lookedAnim;

		private static FieldRef<NPowerContainer, List<NPower>> _rowList;

		private static bool _lookedList;

		public static bool Covering
		{
			get
			{
				if (_applied.Count > 0 && IsRegistered(_applied.Peek()))
				{
					return _applied.Peek().Ui;
				}
				return false;
			}
		}

		private static Run ForTarget(Creature target)
		{
			for (int i = 0; i < _runs.Count; i++)
			{
				if (_runs[i].Target == target)
				{
					return _runs[i];
				}
			}
			return null;
		}

		private static bool IsRegistered(Run run)
		{
			if (run != null && _runs.Contains(run))
			{
				return Current(run);
			}
			return false;
		}

		private static bool Current(Run run)
		{
			if (!run.Cancelled)
			{
				if (run.Multiplayer)
				{
					if ((object)run.Room == GameRefs.CombatRoom)
					{
						if (run.Remote)
						{
							return MultiplayerFx.Active;
						}
						return true;
					}
					return false;
				}
				return true;
			}
			return false;
		}

		internal static bool CanShow(Run run)
		{
			if (run != null)
			{
				return Current(run);
			}
			return false;
		}

		private static void Prune()
		{
			Run[] array = _runs.ToArray();
			foreach (Run run in array)
			{
				if (!Current(run))
				{
					run.Cancelled = true;
					run.Begin = null;
					run.Started = true;
					Release(run, quiet: true);
				}
			}
		}

		public static void Arm(Creature target, Creature caster, Node parent, Action<Run> begin, bool remote = false, CardModel source = null)
		{
			if (begin == null)
			{
				return;
			}
			Run run = new Run
			{
				Target = target,
				Caster = caster,
				Begin = begin
			};
			try
			{
				run.Multiplayer = MultiplayerFx.Active;
				run.Remote = remote && run.Multiplayer;
				run.Room = (Node)(object)(run.Multiplayer ? GameRefs.CombatRoom : null);
				run.Invocation = ((run.Multiplayer && source == MultiplayerFx.CurrentCard) ? MultiplayerFx.CurrentInvocation : null);
				Prune();
				Run[] array = _runs.ToArray();
				foreach (Run run2 in array)
				{
					if (run2.Target == target || (!run.Remote && !run2.Remote))
					{
						Release(run2, quiet: true);
						Start(run2);
					}
				}
				run.Wait = Mathf.Max(0f, Tuning.F("expose.wait", 1.4f));
				run.Land = Mathf.Max(0f, Tuning.F("expose.land", 0.9f));
				run.MaxHold = Mathf.Max(0.2f, Tuning.F("expose.max_hold", 1.6f));
				if (target == null || !FxKit.Alive((GodotObject)(object)parent))
				{
					Start(run);
					return;
				}
				_runs.Add(run);
				run.SoreAmount = SoreOf(target);
				run.Ui = Tuning.F("expose.wipe", 1f) >= 0.5f && !TimeFreeze.Active && Look(run);
				if (!_ticking)
				{
					Ticker.AddLate(Update);
					_ticking = true;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ExposeWipe.Arm: " + ex);
				try
				{
					Release(run, quiet: true);
				}
				catch
				{
				}
				Start(run);
			}
		}

		private static bool Look(Run run)
		{
			try
			{
				if (ShaderLib.Get("expose_wipe") == null)
				{
					return false;
				}
				NCombatRoom combatRoom = GameRefs.CombatRoom;
				NCreature val = ((combatRoom != null) ? combatRoom.GetCreatureNode(run.Target) : null);
				if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree())
				{
					return false;
				}
				NHealthBar val2 = Find<NHealthBar>((Node)(object)val, 6);
				NPowerContainer val3 = Find<NPowerContainer>((Node)(object)val, 6);
				if (!FxKit.Alive((GodotObject)(object)val2) || !FxKit.Alive((GodotObject)(object)val3) || !((Node)val2).IsInsideTree() || !((Node)val3).IsInsideTree())
				{
					return false;
				}
				Control nodeOrNull = ((Node)val2).GetNodeOrNull<Control>(NodePath.op_Implicit("%BlockContainer"));
				if (!FxKit.Alive((GodotObject)(object)nodeOrNull))
				{
					return false;
				}
				run.Victim = val;
				run.Bar = val2;
				run.Row = val3;
				run.Shield = nodeOrNull;
				run.Outline = ((Node)val2).GetNodeOrNull<CanvasItem>(NodePath.op_Implicit("%BlockOutline"));
				run.BarBox = ((Node)val2).GetNodeOrNull<Control>(NodePath.op_Implicit("%HpBarContainer"));
				NPower val4 = null;
				NPower val5 = null;
				foreach (Node child in ((Node)val3).GetChildren(false))
				{
					NPower val6 = (NPower)(object)((child is NPower) ? child : null);
					if (val6 != null && FxKit.Alive((GodotObject)(object)val6))
					{
						run.Seen.Add(((GodotObject)val6).GetInstanceId());
						PowerModel val7 = ModelOf(val6);
						if (val4 == null && val7 is ArtifactPower)
						{
							val4 = val6;
						}
						if (val5 == null && val7 is VulnerablePower)
						{
							val5 = val6;
						}
					}
				}
				run.HadSore = val5 != null;
				run.Sore = val5;
				run.SoreIcon = ((val5 != null) ? ((Node)val5).GetNodeOrNull<Control>(NodePath.op_Implicit("%Icon")) : null);
				if (val4 != null && ExposeHooks.Removal != null)
				{
					Control nodeOrNull2 = ((Node)val4).GetNodeOrNull<Control>(NodePath.op_Implicit("%Icon"));
					if (FxKit.Alive((GodotObject)(object)nodeOrNull2) && ((CanvasItem)nodeOrNull2).IsVisibleInTree())
					{
						run.Gear = val4;
						run.GearIcon = nodeOrNull2;
						run.GearModel = ModelOf(val4);
						run.HoldGear = run.GearModel != null;
					}
				}
				run.HoldBar = BlockOf(run.Target) > 0 && ((CanvasItem)nodeOrNull).Visible && ((CanvasItem)nodeOrNull).IsVisibleInTree();
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("ExposeWipe.Look: " + ex);
				run.HoldBar = false;
				run.HoldGear = false;
				return false;
			}
		}

		public static void SlashSeen(Creature on, string path)
		{
			Prune();
			if (on == null || path != "vfx/vfx_flying_slash")
			{
				return;
			}
			Run run = null;
			for (int i = 0; i < _runs.Count; i++)
			{
				Run run2 = _runs[i];
				if ((!run2.Multiplayer || MultiplayerFx.CurrentInvocation == null || run2.Invocation == MultiplayerFx.CurrentInvocation) && !run2.Started && !run2.Slashed && ((!run2.Multiplayer && run2.Caster == null) || on == run2.Caster || on == run2.Target))
				{
					if (run != null)
					{
						return;
					}
					run = run2;
				}
			}
			if (run != null)
			{
				run.Slashed = true;
				run.SlashAt = run.Age;
			}
		}

		private static void Start(Run run)
		{
			if (run == null || run.Started)
			{
				return;
			}
			run.Started = true;
			Action<Run> begin = run.Begin;
			run.Begin = null;
			try
			{
				begin?.Invoke(run);
			}
			catch (Exception ex)
			{
				DevLog.Warn("ExposeWipe: the line could not be struck: " + ex);
			}
			try
			{
				if (IsRegistered(run) && (!run.Ui || !run.Opened))
				{
					Release(run, quiet: false);
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("ExposeWipe: " + ex2);
			}
		}

		public static Zone Open(Run run, Node parent, Vector2 feet)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (!IsRegistered(run) || !run.Ui || run.Opened)
			{
				return null;
			}
			try
			{
				Zone zone = Measure(run, parent, feet);
				ShaderMaterial val = ((zone != null && run.HoldGear) ? NewMat(0f) : null);
				ShaderMaterial val2 = ((zone != null && run.HoldBar) ? NewMat(0f) : null);
				if (zone == null || (run.HoldGear && val == null) || (run.HoldBar && val2 == null))
				{
					Release(run, quiet: false);
					return null;
				}
				if (run.HoldGear)
				{
					run.GearMat = val;
					Dress((Node)(object)run.Gear, val, run.GearTouched, worn: false, 5);
				}
				if (run.HoldBar)
				{
					run.ShieldMat = val2;
					Dress((Node)(object)run.Shield, val2, run.ShieldTouched, worn: false, 5);
					Dress((Node)(object)run.Outline, val2, run.ShieldTouched, worn: false, 3);
				}
				run.Opened = true;
				return zone;
			}
			catch (Exception ex)
			{
				DevLog.Warn("ExposeWipe.Open: " + ex);
				try
				{
					Release(run, quiet: false);
				}
				catch
				{
					_runs.Remove(run);
				}
				return null;
			}
		}

		public static void Line(Run run, Vector2 at, float shake)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (IsRegistered(run) && run.Opened)
			{
				run.LineY = (run.Moved ? (run.ToUi * at).Y : at.Y);
				run.HasLine = true;
				run.Shake = shake;
			}
		}

		public static void Passed(Run run)
		{
			if (IsRegistered(run))
			{
				run.Ended = true;
				run.Shake = 0f;
			}
		}

		public static bool HoldsBar(NHealthBar bar)
		{
			Prune();
			Run run = FindByBar(bar);
			if (run != null && run.HoldBar && bar != null)
			{
				return bar == run.Bar;
			}
			return false;
		}

		public static bool HoldRemoval(NPowerContainer row, PowerModel power)
		{
			Prune();
			Run run = FindByRow(row);
			if (run == null || !run.HoldGear || row == null || power == null)
			{
				return false;
			}
			if (row != run.Row || !IsGear(run, power))
			{
				return false;
			}
			run.Asked = true;
			run.AskedModel = power;
			return true;
		}

		public static bool CoversRemoved(PowerModel power)
		{
			Prune();
			Run run = FindByPower(power);
			if (run != null && run.HoldGear && power != null)
			{
				return IsGear(run, power);
			}
			return false;
		}

		public static bool CoversApplied(PowerModel power)
		{
			Prune();
			Run run = FindByPower(power);
			if (run != null && run.Ui && power is VulnerablePower)
			{
				return power.Owner == run.Target;
			}
			return false;
		}

		public static bool EnterApplied(NCreature node, PowerModel power)
		{
			Prune();
			Run run = ForTarget((node != null) ? node.Entity : null);
			int num;
			int num2;
			if (PlayerSettings.Enabled && run != null && node != null && power is VulnerablePower && power.Owner == node.Entity)
			{
				if (run.Ui)
				{
					num = ((node == run.Victim) ? 1 : 0);
					if (num == 0)
					{
						goto IL_005c;
					}
				}
				else
				{
					num = 1;
				}
				num2 = (CoversApplied(power) ? 1 : 0);
				goto IL_005d;
			}
			num = 0;
			goto IL_005c;
			IL_005c:
			num2 = 0;
			goto IL_005d;
			IL_005d:
			bool flag = (byte)num2 != 0;
			_applied.Push(flag ? run : null);
			if (num == 0)
			{
				return false;
			}
			run.Landed = true;
			return flag;
		}

		public static void LeaveApplied()
		{
			if (_applied.Count > 0)
			{
				_applied.Pop();
			}
		}

		private static Run FindByBar(NHealthBar bar)
		{
			if (bar == null)
			{
				return null;
			}
			for (int i = 0; i < _runs.Count; i++)
			{
				if (_runs[i].HoldBar && _runs[i].Bar == bar)
				{
					return _runs[i];
				}
			}
			return null;
		}

		private static Run FindByRow(NPowerContainer row)
		{
			if (row == null)
			{
				return null;
			}
			for (int i = 0; i < _runs.Count; i++)
			{
				if (_runs[i].HoldGear && _runs[i].Row == row)
				{
					return _runs[i];
				}
			}
			return null;
		}

		private static Run FindByPower(PowerModel power)
		{
			if (power == null)
			{
				return null;
			}
			for (int i = 0; i < _runs.Count; i++)
			{
				Run run = _runs[i];
				if (run.HoldGear && IsGear(run, power))
				{
					return run;
				}
				if (run.Ui && power is VulnerablePower && power.Owner == run.Target)
				{
					return run;
				}
			}
			return null;
		}

		private static bool IsGear(Run run, PowerModel power)
		{
			if (power != run.GearModel)
			{
				if (power is ArtifactPower)
				{
					return power.Owner == run.Target;
				}
				return false;
			}
			return true;
		}

		private static void Update(double realDt)
		{
			Prune();
			if (_runs.Count == 0)
			{
				Ticker.Remove(Update);
				_ticking = false;
				return;
			}
			try
			{
				if (Ticker.Tree != null && Ticker.Tree.Paused)
				{
					return;
				}
				float num = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
				Run[] array = _runs.ToArray();
				foreach (Run run in array)
				{
					try
					{
						if (!IsRegistered(run))
						{
							continue;
						}
						run.Age += num;
						if (run.Started)
						{
							goto IL_0156;
						}
						if (run.Ui && (!FxKit.Alive((GodotObject)(object)run.Victim) || !((Node)run.Victim).IsInsideTree()))
						{
							Release(run, quiet: true);
							Start(run);
							continue;
						}
						if (!run.Landed && SoreOf(run.Target) > run.SoreAmount)
						{
							run.Landed = true;
						}
						bool flag = (run.Slashed ? (run.Age - run.SlashAt >= run.Land) : (run.Age >= run.Wait));
						if (run.Landed || flag)
						{
							Start(run);
							if (IsRegistered(run))
							{
								goto IL_0156;
							}
						}
						goto end_IL_0081;
						IL_0156:
						run.PassAge += num;
						if (run.PassAge >= run.MaxHold || !FxKit.Alive((GodotObject)(object)run.Victim) || !((Node)run.Victim).IsInsideTree())
						{
							Release(run, quiet: false);
							continue;
						}
						Step(run, num);
						if (run.Ended && !run.HoldGear && !run.HoldBar && run.SoreStage == 4)
						{
							Release(run, quiet: false);
						}
						end_IL_0081:;
					}
					catch (Exception ex)
					{
						DevLog.Warn("ExposeWipe: " + ex);
						try
						{
							Release(run, quiet: true);
						}
						catch
						{
							_runs.Remove(run);
						}
						Start(run);
					}
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("ExposeWipe: " + ex2);
				Run[] array = _runs.ToArray();
				foreach (Run run2 in array)
				{
					try
					{
						Release(run2, quiet: true);
					}
					catch
					{
						_runs.Remove(run2);
					}
					if (!run2.Started)
					{
						Start(run2);
					}
				}
			}
		}

		private static void Step(Run run, float dt)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			float num = (run.HasLine ? run.LineY : 1000000f);
			KeepPinned(run);
			if (run.HoldGear)
			{
				if (!FxKit.Alive((GodotObject)(object)run.Gear))
				{
					LetGearGo(run, keep: false);
				}
				else
				{
					Cut(run.GearMat, num, run.Shake);
					HidePassed(run.GearTouched, num);
					if (RectOf(run.GearIcon, out var rect))
					{
						run.GearTop = rect.Position.Y;
					}
					if (run.HasLine && num <= run.GearTop - 2f)
					{
						run.GearLeft = ((run.GearLeft < 0f) ? 0f : (run.GearLeft + dt));
					}
					if ((run.Asked && run.GearLeft >= 0.05f) || run.Ended)
					{
						LetGearGo(run, keep: true);
					}
				}
			}
			if (run.HoldBar)
			{
				if (!FxKit.Alive((GodotObject)(object)run.Bar) || !FxKit.Alive((GodotObject)(object)run.Shield))
				{
					LetBarGo(run, quiet: false);
				}
				else
				{
					Cut(run.ShieldMat, num, 0f);
					HidePassed(run.ShieldTouched, num);
					if (RectOf(run.Shield, out var rect2))
					{
						run.ShieldTop = rect2.Position.Y;
					}
					if (run.HasLine && num <= run.ShieldTop - 2f)
					{
						run.ShieldPassed = true;
					}
					if ((run.ShieldPassed && BlockOf(run.Target) <= 0) || run.Ended)
					{
						LetBarGo(run, quiet: false);
					}
				}
			}
			StepSore(run, dt, num);
		}

		private static void StepSore(Run run, float dt, float line)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0410: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			if (run.SoreStage == 4)
			{
				return;
			}
			if (run.SoreStage == 0)
			{
				if (run.HadSore)
				{
					if (!FxKit.Alive((GodotObject)(object)run.Sore) || !RectOf(run.SoreIcon, out var rect))
					{
						run.SoreStage = 4;
					}
					else if (run.HasLine && !(line > rect.GetCenter().Y))
					{
						if (!DressSore(run, written: false))
						{
							run.SoreStage = 4;
							return;
						}
						run.SoreMat.SetShaderParameter(StringName.op_Implicit("cut_y"), Variant.op_Implicit(-1000000f));
						run.SoreMat.SetShaderParameter(StringName.op_Implicit("heat"), Variant.op_Implicit(0.8f));
						run.SoreStage = 3;
						run.SoreClock = 0f;
					}
					return;
				}
				NPower val = NewSore(run);
				if (val == null)
				{
					return;
				}
				run.Sore = val;
				run.SoreIcon = ((Node)val).GetNodeOrNull<Control>(NodePath.op_Implicit("%Icon"));
				if (!FxKit.Alive((GodotObject)(object)run.SoreIcon))
				{
					run.SoreStage = 4;
					return;
				}
				run.SoreLoose = !StopEntrance(val);
				Settle(run);
				if (!DressSore(run, written: true))
				{
					EndSore(run);
					return;
				}
				Pin(run);
				if (!RectOf(run.SoreIcon, out var rect2))
				{
					EndSore(run);
					return;
				}
				run.SoreSet = false;
				run.SoreCut = rect2.End.Y + 7f;
				run.SoreStage = 1;
				run.SoreClock = 0f;
				run.SoreAge = 0f;
			}
			if (!FxKit.Alive((GodotObject)(object)run.Sore) || !FxKit.Alive((GodotObject)(object)run.SoreIcon) || run.SoreMat == null)
			{
				EndSore(run);
				return;
			}
			run.SoreAge += dt;
			if (run.SoreLoose && run.SoreStage != 3 && run.SoreAge < 0.6f)
			{
				Settle(run);
			}
			if (run.SoreStage == 1)
			{
				if (!RectOf(run.SoreIcon, out var rect3))
				{
					EndSore(run);
					return;
				}
				if (!run.SoreSet && (run.HasLine || run.Ended))
				{
					run.SoreSet = true;
					run.SoreOwn = run.Ended || line < rect3.End.Y - 1f;
				}
				float num = (run.SoreSet ? line : 1000000f);
				if (run.SoreOwn)
				{
					float num2 = (rect3.Size.Y + 14f) / 0.12f;
					run.SoreCut = Mathf.Min(run.SoreCut, rect3.End.Y + 7f) - num2 * dt;
					num = Mathf.Max(run.SoreCut, line);
				}
				run.SoreMat.SetShaderParameter(StringName.op_Implicit("cut_y"), Variant.op_Implicit(num));
				run.SoreMat.SetShaderParameter(StringName.op_Implicit("heat"), Variant.op_Implicit(1f));
				if (num <= rect3.Position.Y - 7f)
				{
					run.SoreMat.SetShaderParameter(StringName.op_Implicit("cut_y"), Variant.op_Implicit(-1000000f));
					ShowAgain(run.SoreTouched);
					run.SoreStage = 2;
					run.SoreClock = 0f;
				}
			}
			else if (run.SoreStage == 2)
			{
				run.SoreClock += dt;
				float num3 = Mathf.Clamp(run.SoreClock / Mathf.Max(0.05f, Tuning.F("expose.cool", 0.45f)), 0f, 1f);
				run.SoreMat.SetShaderParameter(StringName.op_Implicit("heat"), Variant.op_Implicit(1f - num3 * num3 * (3f - 2f * num3)));
				if (num3 >= 1f)
				{
					EndSore(run);
				}
			}
			else
			{
				run.SoreClock += dt;
				float num4 = Mathf.Clamp(run.SoreClock / 0.3f, 0f, 1f);
				run.SoreMat.SetShaderParameter(StringName.op_Implicit("heat"), Variant.op_Implicit(0.8f * (1f - num4) * (1f - num4)));
				if (num4 >= 1f)
				{
					EndSore(run);
				}
			}
		}

		private static NPower NewSore(Run run)
		{
			if (!FxKit.Alive((GodotObject)(object)run.Row))
			{
				return null;
			}
			foreach (Node child in ((Node)run.Row).GetChildren(false))
			{
				NPower val = (NPower)(object)((child is NPower) ? child : null);
				if (val != null && FxKit.Alive((GodotObject)(object)val) && run.Seen.Add(((GodotObject)val).GetInstanceId()) && ModelOf(val) is VulnerablePower)
				{
					return val;
				}
			}
			return null;
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
					DevLog.Warn("ExposeWipe: no _animInTween (" + ex.Message + ")");
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
				DevLog.Warn("ExposeWipe: the icon's entrance could not be stopped (" + ex2.Message + ")");
				return false;
			}
		}

		private static void Settle(Run run)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			Control soreIcon = run.SoreIcon;
			if (FxKit.Alive((GodotObject)(object)soreIcon))
			{
				if (soreIcon.Position.Y != 0f)
				{
					soreIcon.Position = new Vector2(soreIcon.Position.X, 0f);
				}
				Color modulate = ((CanvasItem)soreIcon).Modulate;
				if (modulate.A < 1f)
				{
					((CanvasItem)soreIcon).Modulate = new Color(modulate.R, modulate.G, modulate.B, 1f);
				}
			}
			Node sore = (Node)(object)run.Sore;
			if (!FxKit.Alive((GodotObject)(object)sore))
			{
				return;
			}
			CanvasItem val = (CanvasItem)(object)((sore is CanvasItem) ? sore : null);
			if (val != null)
			{
				Color modulate2 = val.Modulate;
				if (modulate2.A < 1f)
				{
					val.Modulate = new Color(modulate2.R, modulate2.G, modulate2.B, 1f);
				}
			}
		}

		private static bool DressSore(Run run, bool written)
		{
			Control soreIcon = run.SoreIcon;
			if (!FxKit.Alive((GodotObject)(object)soreIcon) || !FxKit.Alive((GodotObject)(object)run.Sore) || ((CanvasItem)soreIcon).UseParentMaterial)
			{
				return false;
			}
			if (((CanvasItem)soreIcon).Material != null && !(soreIcon is TextureRect))
			{
				return false;
			}
			ShaderMaterial val = NewMat(1f);
			if (val == null)
			{
				return false;
			}
			run.SoreMat = val;
			Material material = ((CanvasItem)soreIcon).Material;
			((CanvasItem)soreIcon).Material = (Material)(object)val;
			run.SoreTouched.Add(new Touched
			{
				Item = (CanvasItem)(object)soreIcon,
				Mat = val,
				Was = material
			});
			Control nodeOrNull = ((Node)run.Sore).GetNodeOrNull<Control>(NodePath.op_Implicit("%AmountLabel"));
			if (FxKit.Alive((GodotObject)(object)nodeOrNull) && !((CanvasItem)nodeOrNull).UseParentMaterial && ((CanvasItem)nodeOrNull).Material == null)
			{
				((CanvasItem)nodeOrNull).Material = (Material)(object)val;
				run.SoreTouched.Add(new Touched
				{
					Item = (CanvasItem)(object)nodeOrNull,
					Mat = val
				});
			}
			else if (written && FxKit.Alive((GodotObject)(object)nodeOrNull) && ((CanvasItem)nodeOrNull).Visible)
			{
				((CanvasItem)nodeOrNull).Visible = false;
				run.SoreTouched.Add(new Touched
				{
					Item = (CanvasItem)(object)nodeOrNull,
					Hid = true
				});
			}
			return true;
		}

		private static void EndSore(Run run)
		{
			Restore(run.SoreTouched);
			run.SoreMat = null;
			run.SoreStage = 4;
		}

		private static List<NPower> ListOf(NPowerContainer row)
		{
			if (!_lookedList)
			{
				_lookedList = true;
				try
				{
					_rowList = AccessTools.FieldRefAccess<NPowerContainer, List<NPower>>("_powerNodes");
				}
				catch (Exception ex)
				{
					DevLog.Warn("ExposeWipe: no _powerNodes (" + ex.Message + ")");
				}
			}
			if (_rowList == null || !FxKit.Alive((GodotObject)(object)row))
			{
				return null;
			}
			try
			{
				return _rowList.Invoke(row);
			}
			catch (Exception ex2)
			{
				_rowList = null;
				DevLog.Warn("ExposeWipe: the power row's list could not be read (" + ex2.Message + ")");
				return null;
			}
		}

		private static void Pin(Run run)
		{
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (run.Pinned || !run.HoldGear || Tuning.F("expose.same_slot", 1f) < 0.5f || !FxKit.Alive((GodotObject)(object)run.Gear) || !FxKit.Alive((GodotObject)(object)run.Sore) || !FxKit.Alive((GodotObject)(object)run.Row) || (object)((Node)run.Gear).GetParent() != run.Row || (object)((Node)run.Sore).GetParent() != run.Row)
				{
					return;
				}
				List<NPower> list = ListOf(run.Row);
				if (list != null && list.IndexOf(run.Gear) >= 0 && list.IndexOf(run.Sore) >= 0)
				{
					run.PinHome = ((Control)run.Sore).Position;
					run.PinAt = ((Control)run.Gear).Position;
					((Control)run.Sore).Position = run.PinAt;
					run.Pinned = true;
					if (run.SoreMat != null)
					{
						run.SoreMat.SetShaderParameter(StringName.op_Implicit("band"), Variant.op_Implicit(0.5f));
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ExposeWipe.Pin: " + ex);
				Unpin(run);
			}
		}

		private static void KeepPinned(Run run)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			if (!run.Pinned)
			{
				return;
			}
			try
			{
				if (!run.HoldGear || !FxKit.Alive((GodotObject)(object)run.Gear) || !FxKit.Alive((GodotObject)(object)run.Sore))
				{
					Unpin(run);
					return;
				}
				Vector2 position = ((Control)run.Sore).Position;
				Vector2 val = position - run.PinAt;
				if (!(val.LengthSquared() > 0.25f))
				{
					val = ((Control)run.Gear).Position - run.PinAt;
					if (!(val.LengthSquared() > 0.25f))
					{
						goto IL_008a;
					}
				}
				run.PinHome = position;
				goto IL_008a;
				IL_008a:
				run.PinAt = ((Control)run.Gear).Position;
				((Control)run.Sore).Position = run.PinAt;
			}
			catch (Exception ex)
			{
				DevLog.Warn("ExposeWipe.KeepPinned: " + ex);
				Unpin(run);
			}
		}

		private static void Unpin(Run run)
		{
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			if (!run.Pinned)
			{
				return;
			}
			run.Pinned = false;
			try
			{
				if (run.Multiplayer && FxKit.Alive((GodotObject)(object)run.Sore) && FxKit.Alive((GodotObject)(object)run.Gear))
				{
					Vector2 val = ((Control)run.Sore).Position - run.PinAt;
					if (!(val.LengthSquared() > 0.25f))
					{
						val = ((Control)run.Gear).Position - run.PinAt;
						if (!(val.LengthSquared() > 0.25f))
						{
							goto IL_008d;
						}
					}
					run.PinHome = ((Control)run.Sore).Position;
				}
				goto IL_008d;
				IL_008d:
				if (FxKit.Alive((GodotObject)(object)run.Sore))
				{
					((Control)run.Sore).Position = run.PinHome;
				}
				if (run.SoreMat != null)
				{
					run.SoreMat.SetShaderParameter(StringName.op_Implicit("band"), Variant.op_Implicit(7f));
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ExposeWipe.Unpin: " + ex.Message);
			}
		}

		private static bool TakeSlot(Run run)
		{
			try
			{
				if (!FxKit.Alive((GodotObject)(object)run.Gear) || !FxKit.Alive((GodotObject)(object)run.Sore))
				{
					return false;
				}
				List<NPower> list = ListOf(run.Row);
				if (list == null)
				{
					return false;
				}
				int num = list.IndexOf(run.Gear);
				int num2 = list.IndexOf(run.Sore);
				if (num < 0 || num2 < 0 || num == num2)
				{
					return false;
				}
				if (num2 == num - 1)
				{
					return true;
				}
				list.RemoveAt(num2);
				list.Insert((num2 < num) ? (num - 1) : num, run.Sore);
				return true;
			}
			catch (Exception ex)
			{
				DevLog.Warn("ExposeWipe.TakeSlot: " + ex);
				return false;
			}
		}

		private static void LetGearGo(Run run, bool keep)
		{
			bool holdGear = run.HoldGear;
			run.HoldGear = false;
			Restore(run.GearTouched);
			run.GearMat = null;
			if (run.Pinned)
			{
				if (((holdGear && run.Asked) & keep) && TakeSlot(run))
				{
					run.Pinned = false;
				}
				else
				{
					Unpin(run);
				}
			}
			if (!holdGear || !run.Asked)
			{
				return;
			}
			run.Asked = false;
			try
			{
				Node gear = (Node)(object)run.Gear;
				if (FxKit.Alive((GodotObject)(object)gear))
				{
					CanvasItem val = (CanvasItem)(object)((gear is CanvasItem) ? gear : null);
					if (val != null)
					{
						val.Visible = false;
					}
				}
				MethodInfo removal = ExposeHooks.Removal;
				if (removal != null && run.AskedModel != null && FxKit.Alive((GodotObject)(object)run.Row) && ((Node)run.Row).IsInsideTree())
				{
					removal.Invoke(run.Row, new object[1] { run.AskedModel });
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ExposeWipe: the power row could not be told that the Artifact is gone: " + ex);
			}
		}

		private static void LetBarGo(Run run, bool quiet)
		{
			bool holdBar = run.HoldBar;
			run.HoldBar = false;
			Restore(run.ShieldTouched);
			run.ShieldMat = null;
			if (!holdBar)
			{
				return;
			}
			try
			{
				if (FxKit.Alive((GodotObject)(object)run.Bar) && ((Node)run.Bar).IsInsideTree())
				{
					if ((quiet || run.ShieldPassed) && BlockOf(run.Target) <= 0 && FxKit.Alive((GodotObject)(object)run.Shield))
					{
						((CanvasItem)run.Shield).Visible = false;
					}
					run.Bar.RefreshValues();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ExposeWipe: the health bar could not be refreshed: " + ex);
			}
		}

		private static void Release(Run run, bool quiet)
		{
			if (run == null)
			{
				return;
			}
			_runs.Remove(run);
			run.Ui = false;
			try
			{
				LetGearGo(run, keep: false);
			}
			catch (Exception ex)
			{
				DevLog.Warn("ExposeWipe (gear): " + ex);
			}
			Unpin(run);
			try
			{
				LetBarGo(run, quiet);
			}
			catch (Exception ex2)
			{
				DevLog.Warn("ExposeWipe (bar): " + ex2);
			}
			try
			{
				EndSore(run);
			}
			catch (Exception ex3)
			{
				DevLog.Warn("ExposeWipe (Vulnerable): " + ex3);
			}
		}

		public static void Reset()
		{
			try
			{
				Run[] array = _runs.ToArray();
				_runs.Clear();
				foreach (Run obj in array)
				{
					obj.Cancelled = true;
					obj.Begin = null;
					obj.Started = true;
					Release(obj, quiet: true);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("ExposeWipe.Reset: " + ex);
			}
		}

		private static ShaderMaterial NewMat(float mode)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			ShaderMaterial val = ShaderLib.NewMaterial("expose_wipe");
			if (val == null)
			{
				return null;
			}
			val.SetShaderParameter(StringName.op_Implicit("mode"), Variant.op_Implicit(mode));
			val.SetShaderParameter(StringName.op_Implicit("band"), Variant.op_Implicit(7f));
			val.SetShaderParameter(StringName.op_Implicit("cut_y"), Variant.op_Implicit(1000000f));
			val.SetShaderParameter(StringName.op_Implicit("heat"), Variant.op_Implicit(0f));
			val.SetShaderParameter(StringName.op_Implicit("shake"), Variant.op_Implicit(0f));
			return val;
		}

		private static void Cut(ShaderMaterial mat, float line, float shake)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if (mat != null)
			{
				mat.SetShaderParameter(StringName.op_Implicit("cut_y"), Variant.op_Implicit(line));
				mat.SetShaderParameter(StringName.op_Implicit("shake"), Variant.op_Implicit(shake));
			}
		}

		private static void Dress(Node root, ShaderMaterial mat, List<Touched> into, bool worn, int depth)
		{
			if (!FxKit.Alive((GodotObject)(object)root) || mat == null)
			{
				return;
			}
			bool worn2 = false;
			CanvasItem val = (CanvasItem)(object)((root is CanvasItem) ? root : null);
			if (val != null && !(root is GpuParticles2D) && !(root is CpuParticles2D))
			{
				if (val.UseParentMaterial)
				{
					if (worn)
					{
						worn2 = true;
					}
					else
					{
						into.Add(new Touched
						{
							Item = val
						});
					}
				}
				else if (val.Material == null)
				{
					val.Material = (Material)(object)mat;
					into.Add(new Touched
					{
						Item = val,
						Mat = mat
					});
					worn2 = true;
				}
				else if (val is TextureRect)
				{
					Material material = val.Material;
					val.Material = (Material)(object)mat;
					into.Add(new Touched
					{
						Item = val,
						Mat = mat,
						Was = material
					});
					worn2 = true;
				}
				else
				{
					into.Add(new Touched
					{
						Item = val
					});
				}
			}
			if (depth <= 0)
			{
				return;
			}
			foreach (Node child in root.GetChildren(false))
			{
				Dress(child, mat, into, worn2, depth - 1);
			}
		}

		private static void HidePassed(List<Touched> list, float line)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < list.Count; i++)
			{
				Touched touched = list[i];
				if (touched.Mat != null || touched.Hid || !FxKit.Alive((GodotObject)(object)touched.Item) || !touched.Item.Visible)
				{
					continue;
				}
				float num = line;
				CanvasItem item = touched.Item;
				Control val = (Control)(object)((item is Control) ? item : null);
				if (val != null)
				{
					Rect2 val2 = Drawn(val);
					num = val2.GetCenter().Y;
				}
				else
				{
					CanvasItem item2 = touched.Item;
					Node2D val3 = (Node2D)(object)((item2 is Node2D) ? item2 : null);
					if (val3 != null)
					{
						num = val3.GlobalPosition.Y;
					}
				}
				if (!(line > num))
				{
					touched.Item.Visible = false;
					touched.Hid = true;
				}
			}
		}

		private static void ShowAgain(List<Touched> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				Touched touched = list[i];
				if (touched.Mat == null && touched.Hid)
				{
					touched.Hid = false;
					if (FxKit.Alive((GodotObject)(object)touched.Item))
					{
						touched.Item.Visible = true;
					}
				}
			}
		}

		private static void Restore(List<Touched> list)
		{
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			for (int num = list.Count - 1; num >= 0; num--)
			{
				Touched touched = list[num];
				try
				{
					if (FxKit.Alive((GodotObject)(object)touched.Item))
					{
						if (touched.Mat != null)
						{
							Material material = touched.Item.Material;
							if (material != null && FxKit.Alive((GodotObject)(object)touched.Mat) && ((GodotObject)material).GetInstanceId() == ((GodotObject)touched.Mat).GetInstanceId())
							{
								Material was = touched.Was;
								ShaderMaterial val = (ShaderMaterial)(object)((was is ShaderMaterial) ? was : null);
								if (val != null && FxKit.Alive((GodotObject)(object)val))
								{
									Variant shaderParameter = touched.Mat.GetShaderParameter(StringName.op_Implicit("pulse"));
									if ((int)shaderParameter.VariantType != 0)
									{
										val.SetShaderParameter(StringName.op_Implicit("pulse"), shaderParameter);
									}
								}
								touched.Item.Material = ((touched.Was != null && FxKit.Alive((GodotObject)(object)touched.Was)) ? touched.Was : null);
							}
						}
						else if (touched.Hid)
						{
							touched.Item.Visible = true;
						}
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("ExposeWipe.Restore: " + ex.Message);
				}
			}
			list.Clear();
		}

		private static Zone Measure(Run run, Node parent, Vector2 feet)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)run.Victim) || !((Node)run.Victim).IsInsideTree() || !FxKit.Alive((GodotObject)(object)run.Bar) || !FxKit.Alive((GodotObject)(object)run.Row) || !FxKit.Alive((GodotObject)(object)run.Shield) || !((Node)run.Shield).IsInsideTree())
			{
				return null;
			}
			Transform2D val = Transform2D.Identity;
			CanvasItem val2 = (CanvasItem)(object)((parent is CanvasItem) ? parent : null);
			if (val2 != null && FxKit.Alive((GodotObject)(object)val2) && ((Node)val2).IsInsideTree())
			{
				Transform2D canvasTransform = ((CanvasItem)run.Shield).GetCanvasTransform();
				Transform2D canvasTransform2 = val2.GetCanvasTransform();
				if (!canvasTransform.X.IsEqualApprox(canvasTransform2.X) || !canvasTransform.Y.IsEqualApprox(canvasTransform2.Y) || !canvasTransform.Origin.IsEqualApprox(canvasTransform2.Origin))
				{
					val = canvasTransform2.AffineInverse() * canvasTransform;
					run.ToUi = val.AffineInverse();
					run.Moved = true;
				}
			}
			bool flag = false;
			Rect2 val3 = default;
			if (RectOf(run.BarBox, out var rect))
			{
				val3 = rect;
				flag = true;
			}
			bool flag2 = RectOf(run.Shield, out var rect2);
			if (flag2)
			{
				val3 = (flag ? val3.Merge(rect2) : rect2);
				flag = true;
			}
			foreach (Node child in ((Node)run.Row).GetChildren(false))
			{
				NPower val4 = (NPower)(object)((child is NPower) ? child : null);
				if (val4 != null && FxKit.Alive((GodotObject)(object)val4) && RectOf(((Node)val4).GetNodeOrNull<Control>(NodePath.op_Implicit("%Icon")), out var rect3))
				{
					val3 = (flag ? val3.Merge(rect3) : rect3);
					flag = true;
				}
			}
			if (!flag)
			{
				return null;
			}
			Rect2 val5;
			if (run.Moved)
			{
				val5 = val * val3;
				val3 = val5.Abs();
			}
			float num = val3.Position.X + val3.Size.X * 0.5f;
			if (!Finite(val3) || Mathf.Abs(num - feet.X) > 700f || Mathf.Abs(val3.End.Y - feet.Y) > 500f)
			{
				return null;
			}
			Zone zone = new Zone
			{
				Left = val3.Position.X,
				Right = val3.End.X,
				Bottom = val3.End.Y
			};
			if (run.HoldGear)
			{
				if (FxKit.Alive((GodotObject)(object)run.Gear) && RectOf(run.GearIcon, out var rect4))
				{
					run.GearTop = rect4.Position.Y;
					List<Obstacle> obstacles = zone.Obstacles;
					Rect2 rect5;
					if (!run.Moved)
					{
						rect5 = rect4;
					}
					else
					{
						val5 = val * rect4;
						rect5 = val5.Abs();
					}
					obstacles.Add(Stand(gear: true, rect5));
				}
				else
				{
					LetGearGo(run, keep: false);
				}
			}
			if (run.HoldBar)
			{
				if (flag2)
				{
					run.ShieldTop = rect2.Position.Y;
					List<Obstacle> obstacles2 = zone.Obstacles;
					Rect2 rect6;
					if (!run.Moved)
					{
						rect6 = rect2;
					}
					else
					{
						val5 = val * rect2;
						rect6 = val5.Abs();
					}
					obstacles2.Add(Stand(gear: false, rect6));
				}
				else
				{
					LetBarGo(run, quiet: false);
				}
			}
			zone.Obstacles.Sort((Obstacle a, Obstacle b) => b.Bottom.CompareTo(a.Bottom));
			return zone;
		}

		private static Obstacle Stand(bool gear, Rect2 rect)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			return new Obstacle
			{
				Gear = gear,
				X = rect.Position.X + rect.Size.X * 0.5f,
				Width = rect.Size.X,
				Top = rect.Position.Y,
				Bottom = rect.End.Y
			};
		}

		private static bool RectOf(Control box, out Rect2 rect)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			rect = default;
			if (!FxKit.Alive((GodotObject)(object)box) || !((Node)box).IsInsideTree() || !((CanvasItem)box).IsVisibleInTree())
			{
				return false;
			}
			rect = Drawn(box);
			if (Finite(rect) && rect.Size.X >= 4f && rect.Size.Y >= 4f && rect.Size.X <= 1600f)
			{
				return rect.Size.Y <= 500f;
			}
			return false;
		}

		private static Rect2 Drawn(Control box)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			Rect2 val = ((CanvasItem)box).GetGlobalTransform() * new Rect2(Vector2.Zero, box.Size);
			return val.Abs();
		}

		private static bool Finite(Rect2 rect)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			if (float.IsFinite(rect.Position.X) && float.IsFinite(rect.Position.Y) && float.IsFinite(rect.Size.X))
			{
				return float.IsFinite(rect.Size.Y);
			}
			return false;
		}

		private static int BlockOf(Creature creature)
		{
			try
			{
				return (creature != null) ? creature.Block : 0;
			}
			catch
			{
				return 0;
			}
		}

		private static int SoreOf(Creature creature)
		{
			try
			{
				return (creature != null) ? creature.GetPowerAmount<VulnerablePower>() : 0;
			}
			catch
			{
				return 0;
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
