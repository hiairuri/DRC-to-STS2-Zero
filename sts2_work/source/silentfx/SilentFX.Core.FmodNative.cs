// namespace: SilentFX.Core
// type: FmodNative (class)
// source lines: 34539-35577
	public static class FmodNative
	{
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private delegate int UpdateFn(nint self);

		internal enum WarmResult
		{
			Pending,
			Attached,
			Unavailable
		}

		private const string Studio = "fmodstudio";

		private const string Core = "fmod";

		private const string UpdateName = "?update@System@Studio@FMOD@@QEAA?AW4FMOD_RESULT@@XZ";

		private const string MusicBus = "bus:/master/music";

		private static bool _resolver;

		private static bool _dead;

		private static string _why = "not tried yet";

		private static nint _slot;

		private static nint _original;

		private static UpdateFn _hook;

		private static UpdateFn _next;

		private static bool _hooked;

		private static bool _warmupHookOnly;

		private static double _hookedFor;

		private static long _hookStartedAt;

		private static long _systemCapturedAt;

		private static bool _hookWaitLogged;

		private static nint _system;

		private static bool _moodRequested;

		private static bool _optionalWarmAttempt;

		private static nint _core;

		private static nint _bus;

		private static nint _group;

		private static nint _low;

		private static nint _verb;

		private static bool _attached;

		private static bool _busLocked;

		private static bool _lowAdded;

		private static bool _verbAdded;

		private static bool _lowIsEq;

		private static int _lowCutoff = -1;

		private static int _lowKind = -1;

		private static int _verbWet = -1;

		private static int _verbDry = -1;

		private static int _verbDecay = -1;

		private static int _verbHighCut = -1;

		private static float _cutoffNow = -1f;

		private static float _wetNow = float.NaN;

		private static bool _lowOff = true;

		private static bool _verbOff = true;

		private static nint _meterIn;

		private static nint _meterOut;

		public static string Status
		{
			get
			{
				if (!_dead)
				{
					if (!_attached)
					{
						if (_system == IntPtr.Zero)
						{
							if (!_hooked)
							{
								return _why;
							}
							return "正在取句柄";
						}
						return "待命";
					}
					return "已接上音乐总线";
				}
				return "没接上：" + _why;
			}
		}

		public static bool Dead => _dead;

		public static bool Attached => _attached;

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool VirtualProtect(nint address, nuint size, uint protect, out uint old);

		[DllImport("fmodstudio")]
		private static extern int FMOD_Studio_System_IsValid(nint system);

		[DllImport("fmodstudio")]
		private static extern int FMOD_Studio_System_GetCoreSystem(nint system, out nint core);

		[DllImport("fmodstudio")]
		private static extern int FMOD_Studio_System_GetBus(nint system, byte[] path, out nint bus);

		[DllImport("fmodstudio")]
		private static extern int FMOD_Studio_System_FlushCommands(nint system);

		[DllImport("fmodstudio")]
		private static extern int FMOD_Studio_Bus_IsValid(nint bus);

		[DllImport("fmodstudio")]
		private static extern int FMOD_Studio_Bus_LockChannelGroup(nint bus);

		[DllImport("fmodstudio")]
		private static extern int FMOD_Studio_Bus_UnlockChannelGroup(nint bus);

		[DllImport("fmodstudio")]
		private static extern int FMOD_Studio_Bus_GetChannelGroup(nint bus, out nint group);

		[DllImport("fmod")]
		private static extern int FMOD_System_GetVersion(nint system, out uint version, out uint build);

		[DllImport("fmod")]
		private static extern int FMOD_System_CreateDSPByType(nint system, int type, out nint dsp);

		[DllImport("fmod")]
		private static extern int FMOD_ChannelGroup_AddDSP(nint group, int index, nint dsp);

		[DllImport("fmod")]
		private static extern int FMOD_ChannelGroup_RemoveDSP(nint group, nint dsp);

		[DllImport("fmod")]
		private static extern int FMOD_DSP_Release(nint dsp);

		[DllImport("fmod")]
		private static extern int FMOD_DSP_SetBypass(nint dsp, int bypass);

		[DllImport("fmod")]
		private static extern int FMOD_DSP_SetParameterFloat(nint dsp, int index, float value);

		[DllImport("fmod")]
		private static extern int FMOD_DSP_SetParameterInt(nint dsp, int index, int value);

		[DllImport("fmod")]
		private static extern int FMOD_DSP_GetNumParameters(nint dsp, out int count);

		[DllImport("fmod")]
		private static extern int FMOD_DSP_GetParameterInfo(nint dsp, int index, out nint description);

		[DllImport("fmod")]
		private static extern int FMOD_DSP_GetInfo(nint dsp, byte[] name, out uint version, out int channels, out int width, out int height);

		[DllImport("fmod")]
		private static extern int FMOD_DSP_SetMeteringEnabled(nint dsp, int input, int output);

		[DllImport("fmod")]
		private static extern int FMOD_DSP_GetMeteringInfo(nint dsp, nint input, nint output);

		internal static WarmResult WarmNeutral()
		{
			if (_dead)
			{
				return WarmResult.Unavailable;
			}
			if (_attached)
			{
				return WarmResult.Attached;
			}
			if (!OperatingSystem.IsWindows())
			{
				return WarmResult.Unavailable;
			}
			bool optionalWarmAttempt = _optionalWarmAttempt;
			_optionalWarmAttempt = !_moodRequested && (!_hooked || _warmupHookOnly);
			try
			{
				if (_system == IntPtr.Zero)
				{
					if (!HookModulesReady())
					{
						return WarmResult.Pending;
					}
					if (!_hooked)
					{
						Hook(warmupOnly: true);
					}
					if (_dead)
					{
						return WarmResult.Unavailable;
					}
					if (!_hooked && _system == IntPtr.Zero)
					{
						return WarmResult.Unavailable;
					}
					if (_system == IntPtr.Zero)
					{
						return WarmResult.Pending;
					}
				}
				LogHookWaitElapsed();
				if (_hooked)
				{
					Unhook();
				}
				long timestamp = Stopwatch.GetTimestamp();
				try
				{
					return Attach() ? WarmResult.Attached : WarmResult.Unavailable;
				}
				finally
				{
					double totalMilliseconds = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds;
					DevLog.Info($"FmodNative: neutral DSP attach elapsed {totalMilliseconds:F1} ms (synchronous Attach call; elapsed, not CPU)");
				}
			}
			catch (Exception ex)
			{
				Fail(ex.GetType().Name + ": " + ex.Message);
				return WarmResult.Unavailable;
			}
			finally
			{
				_optionalWarmAttempt = optionalWarmAttempt;
			}
		}

		internal static void CancelWarmupWait()
		{
			if (_hooked && _warmupHookOnly)
			{
				Unhook();
				DevLog.Info("FmodNative: canceled its pending neutral-prewarm hook; no Mood request owned it");
			}
		}

		private static bool Fail(string what, int result = 0)
		{
			_why = ((result != 0) ? (what + " (FMOD " + result + ")") : what);
			if (_optionalWarmAttempt)
			{
				DevLog.Warn("FmodNative: neutral prewarm unavailable - " + _why + "; leaving ordinary Mood attach available");
				try
				{
					CleanupPartialAttach();
				}
				catch
				{
				}
				try
				{
					Unhook();
				}
				catch
				{
				}
				return false;
			}
			_dead = true;
			DevLog.Warn("FmodNative: off - " + _why);
			try
			{
				Detach();
			}
			catch
			{
			}
			try
			{
				Unhook();
			}
			catch
			{
			}
			return false;
		}

		public static bool Want(double dt)
		{
			if (_dead)
			{
				return false;
			}
			_moodRequested = true;
			try
			{
				if (!OperatingSystem.IsWindows())
				{
					return Fail("only on Windows");
				}
				if (_attached)
				{
					if (FMOD_Studio_System_IsValid(_system) == 0 || FMOD_Studio_Bus_IsValid(_bus) == 0)
					{
						_attached = false;
						_low = (_verb = (_group = (_bus = (_core = IntPtr.Zero))));
						_busLocked = (_lowAdded = (_verbAdded = false));
						_system = IntPtr.Zero;
						_moodRequested = false;
						DevLog.Info("FmodNative: the sound system changed - starting over");
						return false;
					}
					return true;
				}
				if (_system == IntPtr.Zero)
				{
					if (!_hooked)
					{
						return Hook();
					}
					_warmupHookOnly = false;
					_hookedFor += dt;
					if (_hookedFor > 3.0)
					{
						return Fail("the plug-in never called update through its import table");
					}
					return false;
				}
				if (_hooked)
				{
					Unhook();
				}
				LogHookWaitElapsed();
				long timestamp = Stopwatch.GetTimestamp();
				bool result = Attach();
				_moodRequested = false;
				double totalMilliseconds = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds;
				if (totalMilliseconds > 4.0)
				{
					DevLog.Info($"FmodNative: Mood attach elapsed {totalMilliseconds:F1} ms (synchronous Attach call; elapsed, not CPU)");
				}
				return result;
			}
			catch (Exception ex)
			{
				return Fail(ex.GetType().Name + ": " + ex.Message);
			}
		}

		public static void Set(float cutoff, float wet)
		{
			if (!_attached || _dead)
			{
				return;
			}
			try
			{
				bool flag = cutoff >= 19500f;
				if (flag != _lowOff)
				{
					_lowOff = flag;
					FMOD_DSP_SetBypass(_low, flag ? 1 : 0);
				}
				if (!flag && (_cutoffNow < 0f || Math.Abs(cutoff - _cutoffNow) > _cutoffNow * 0.01f))
				{
					_cutoffNow = cutoff;
					int num = FMOD_DSP_SetParameterFloat(_low, _lowCutoff, Math.Clamp(cutoff, 20f, 22000f));
					if (num != 0)
					{
						Fail("the filter would not take a cutoff", num);
						return;
					}
				}
				bool flag2 = wet <= -59f;
				if (flag2 != _verbOff)
				{
					_verbOff = flag2;
					FMOD_DSP_SetBypass(_verb, flag2 ? 1 : 0);
				}
				if (!flag2 && (float.IsNaN(_wetNow) || Math.Abs(wet - _wetNow) > 0.2f))
				{
					_wetNow = wet;
					int num2 = FMOD_DSP_SetParameterFloat(_verb, _verbWet, Math.Clamp(wet, -80f, 6f));
					if (num2 != 0)
					{
						Fail("the reverb would not take a level", num2);
					}
				}
			}
			catch (Exception ex)
			{
				Fail(ex.GetType().Name + ": " + ex.Message);
			}
		}

		public static bool Meter(out float input, out float output)
		{
			input = (output = -80f);
			if (!_attached || _dead || _meterIn == IntPtr.Zero)
			{
				return false;
			}
			try
			{
				if (FMOD_DSP_GetMeteringInfo(_low, _meterIn, _meterOut) != 0)
				{
					return false;
				}
				input = Rms(_meterIn);
				output = Rms(_meterOut);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static float Rms(nint info)
		{
			int num = Math.Clamp((int)Marshal.ReadInt16(info, 260), 0, 32);
			if (num == 0)
			{
				return -80f;
			}
			double num2 = 0.0;
			byte[] array = new byte[4];
			for (int i = 0; i < num; i++)
			{
				Marshal.Copy(info + 4 + 128 + 4 * i, array, 0, 4);
				double num3 = BitConverter.ToSingle(array, 0);
				num2 += num3 * num3;
			}
			double num4 = Math.Sqrt(num2 / (double)num);
			if (!(num4 <= 0.0001))
			{
				return (float)(20.0 * Math.Log10(num4));
			}
			return -80f;
		}

		public static void Detach()
		{
			_moodRequested = false;
			if (!_attached)
			{
				if (_hooked)
				{
					Unhook();
				}
				return;
			}
			_attached = false;
			try
			{
				if (_system != IntPtr.Zero && FMOD_Studio_System_IsValid(_system) != 0)
				{
					if (_group != IntPtr.Zero && _low != IntPtr.Zero)
					{
						FMOD_ChannelGroup_RemoveDSP(_group, _low);
					}
					if (_group != IntPtr.Zero && _verb != IntPtr.Zero)
					{
						FMOD_ChannelGroup_RemoveDSP(_group, _verb);
					}
					if (_low != IntPtr.Zero)
					{
						FMOD_DSP_Release(_low);
					}
					if (_verb != IntPtr.Zero)
					{
						FMOD_DSP_Release(_verb);
					}
					if (_bus != IntPtr.Zero && FMOD_Studio_Bus_IsValid(_bus) != 0)
					{
						FMOD_Studio_Bus_UnlockChannelGroup(_bus);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("FmodNative: letting go - " + ex.Message);
			}
			_low = (_verb = (_group = (_bus = IntPtr.Zero)));
			_busLocked = (_lowAdded = (_verbAdded = false));
			_lowOff = (_verbOff = true);
			_cutoffNow = -1f;
			_wetNow = float.NaN;
			if (PowerFxManager.LogTriggers)
			{
				DevLog.Info("FmodNative: the filter and the reverb are off the music bus");
			}
		}

		private static nint Resolve(string name, Assembly assembly, DllImportSearchPath? path)
		{
			if (name != "fmodstudio" && name != "fmod")
			{
				return IntPtr.Zero;
			}
			foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
			{
				if (string.Equals(Path.GetFileNameWithoutExtension(module.ModuleName), name, StringComparison.OrdinalIgnoreCase))
				{
					return module.BaseAddress;
				}
			}
			return IntPtr.Zero;
		}

		private static bool Hook(bool warmupOnly = false)
		{
			if (!_resolver)
			{
				_resolver = true;
				NativeLibrary.SetDllImportResolver(typeof(FmodNative).Assembly, Resolve);
			}
			nint num = IntPtr.Zero;
			nint num2 = IntPtr.Zero;
			int num3 = 0;
			string value = null;
			foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
			{
				string text = module.ModuleName ?? "";
				if (string.Equals(text, "fmodstudio.dll", StringComparison.OrdinalIgnoreCase))
				{
					num = module.BaseAddress;
				}
				else if (text.Contains("GodotFmod", StringComparison.OrdinalIgnoreCase))
				{
					num2 = module.BaseAddress;
					num3 = module.ModuleMemorySize;
					value = text;
				}
			}
			if (num == IntPtr.Zero)
			{
				return Fail("fmodstudio.dll is not loaded");
			}
			if (num2 == IntPtr.Zero)
			{
				return Fail("the GodotFmod plug-in is not loaded");
			}
			if (!NativeLibrary.TryGetExport(num, "?update@System@Studio@FMOD@@QEAA?AW4FMOD_RESULT@@XZ", out var address) || address == IntPtr.Zero)
			{
				return Fail("fmodstudio.dll has no Studio::System::update under the expected name");
			}
			int num4 = Marshal.ReadInt32(num2, 60);
			if (num4 <= 0 || num4 > 4096 || Marshal.ReadInt32(num2, num4) != 17744)
			{
				return Fail("the plug-in is not a PE image");
			}
			if (Marshal.ReadInt16(num2, num4 + 24) != 523)
			{
				return Fail("the plug-in is not 64-bit");
			}
			int num5 = Marshal.ReadInt32(num2, num4 + 24 + 112 + 8);
			if (num5 <= 0 || num5 >= num3)
			{
				return Fail("the plug-in has no import table");
			}
			nint num6 = IntPtr.Zero;
			for (int i = num5; i + 20 <= num3; i += 20)
			{
				if (num6 != IntPtr.Zero)
				{
					break;
				}
				int num7 = Marshal.ReadInt32(num2, i + 12);
				int num8 = Marshal.ReadInt32(num2, i + 16);
				if (num7 == 0 && num8 == 0)
				{
					break;
				}
				if (num8 <= 0 || num8 >= num3)
				{
					continue;
				}
				for (int j = 0; num8 + 8 * j + 8 <= num3 && j < 4096; j++)
				{
					nint num9 = Marshal.ReadIntPtr(num2, num8 + 8 * j);
					if (num9 == IntPtr.Zero)
					{
						break;
					}
					if (num9 == address)
					{
						num6 = num2 + num8 + 8 * j;
						break;
					}
				}
			}
			if (num6 == IntPtr.Zero)
			{
				return Fail("the plug-in does not import Studio::System::update");
			}
			_hook = OnUpdate;
			_next = Marshal.GetDelegateForFunctionPointer<UpdateFn>(address);
			nint functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate(_hook);
			if (!Write(num6, functionPointerForDelegate))
			{
				return Fail("the import table could not be written");
			}
			_slot = num6;
			_original = address;
			_hooked = true;
			_warmupHookOnly = warmupOnly;
			_hookedFor = 0.0;
			_hookStartedAt = Stopwatch.GetTimestamp();
			_systemCapturedAt = 0L;
			_hookWaitLogged = false;
			if (PowerFxManager.LogTriggers)
			{
				DevLog.Info($"FmodNative: waiting for {value} to call update (entry at +0x{(long)num6 - (long)num2:X})");
			}
			return false;
		}

		private static bool Write(nint slot, nint value)
		{
			if (!VirtualProtect(slot, 8u, 4u, out var old))
			{
				return false;
			}
			Marshal.WriteIntPtr(slot, value);
			VirtualProtect(slot, 8u, old, out var _);
			return true;
		}

		private static void Unhook()
		{
			if (_hooked)
			{
				_hooked = false;
				_warmupHookOnly = false;
				if (_slot != IntPtr.Zero && _original != IntPtr.Zero && !Write(_slot, _original))
				{
					DevLog.Warn("FmodNative: the import table entry could not be put back (it keeps calling through us, which is harmless)");
				}
				_slot = IntPtr.Zero;
			}
		}

		private static int OnUpdate(nint self)
		{
			if (_system == IntPtr.Zero && self != IntPtr.Zero)
			{
				_system = self;
				_systemCapturedAt = Stopwatch.GetTimestamp();
			}
			return _next(self);
		}

		private static void LogHookWaitElapsed()
		{
			if (!_hookWaitLogged && _hookStartedAt != 0L && _systemCapturedAt != 0L)
			{
				_hookWaitLogged = true;
				DevLog.Info($"FmodNative: waiting for GodotFmod update handle elapsed {Stopwatch.GetElapsedTime(_hookStartedAt, _systemCapturedAt).TotalMilliseconds:F1} ms");
			}
		}

		private static bool HookModulesReady()
		{
			bool flag = false;
			bool flag2 = false;
			foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
			{
				string text = module.ModuleName ?? "";
				if (string.Equals(text, "fmodstudio.dll", StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
				}
				else if (text.Contains("GodotFmod", StringComparison.OrdinalIgnoreCase))
				{
					flag2 = true;
				}
				if (flag & flag2)
				{
					return true;
				}
			}
			return false;
		}

		private static string NameOf(nint dsp)
		{
			byte[] array = new byte[64];
			if (FMOD_DSP_GetInfo(dsp, array, out var _, out var _, out var _, out var _) != 0)
			{
				return "";
			}
			int num = Array.IndexOf(array, (byte)0);
			return Encoding.ASCII.GetString(array, 0, (num < 0) ? 32 : Math.Min(num, 32));
		}

		private static int Parameter(nint dsp, params string[] words)
		{
			if (FMOD_DSP_GetNumParameters(dsp, out var count) != 0)
			{
				return -1;
			}
			byte[] array = new byte[16];
			for (int i = 0; i < count && i < 64; i++)
			{
				if (FMOD_DSP_GetParameterInfo(dsp, i, out var description) != 0 || description == IntPtr.Zero)
				{
					continue;
				}
				Marshal.Copy(description + 4, array, 0, 16);
				int num = Array.IndexOf(array, (byte)0);
				string text = Encoding.ASCII.GetString(array, 0, (num < 0) ? 16 : num);
				bool flag = true;
				foreach (string value in words)
				{
					if (text.IndexOf(value, StringComparison.OrdinalIgnoreCase) < 0)
					{
						flag = false;
					}
				}
				if (flag)
				{
					return i;
				}
			}
			return -1;
		}

		private static string Parameters(nint dsp)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (FMOD_DSP_GetNumParameters(dsp, out var count) != 0)
			{
				return "";
			}
			byte[] array = new byte[16];
			for (int i = 0; i < count && i < 64; i++)
			{
				if (FMOD_DSP_GetParameterInfo(dsp, i, out var description) == 0 && description != IntPtr.Zero)
				{
					Marshal.Copy(description + 4, array, 0, 16);
					int num = Array.IndexOf(array, (byte)0);
					stringBuilder.Append(i).Append('=').Append(Encoding.ASCII.GetString(array, 0, (num < 0) ? 16 : num))
						.Append("; ");
				}
			}
			return stringBuilder.ToString();
		}

		private static bool Attach()
		{
			try
			{
				bool flag = AttachCore();
				if (!flag && !_attached)
				{
					CleanupPartialAttach();
				}
				return flag;
			}
			catch
			{
				if (!_attached)
				{
					CleanupPartialAttach();
				}
				throw;
			}
		}

		private static void CleanupPartialAttach()
		{
			try
			{
				if (_group != IntPtr.Zero && _lowAdded && _low != IntPtr.Zero)
				{
					FMOD_ChannelGroup_RemoveDSP(_group, _low);
				}
				if (_group != IntPtr.Zero && _verbAdded && _verb != IntPtr.Zero)
				{
					FMOD_ChannelGroup_RemoveDSP(_group, _verb);
				}
				if (_low != IntPtr.Zero)
				{
					FMOD_DSP_Release(_low);
				}
				if (_verb != IntPtr.Zero)
				{
					FMOD_DSP_Release(_verb);
				}
			}
			catch
			{
			}
			try
			{
				if (_busLocked && _bus != IntPtr.Zero)
				{
					FMOD_Studio_Bus_UnlockChannelGroup(_bus);
				}
			}
			catch
			{
			}
			_busLocked = (_lowAdded = (_verbAdded = false));
			_low = (_verb = (_group = (_bus = (_core = IntPtr.Zero))));
		}

		private static bool AttachCore()
		{
			if (FMOD_Studio_System_IsValid(_system) == 0)
			{
				return Fail("the handle taken from update is not a sound system");
			}
			int num = FMOD_Studio_System_GetCoreSystem(_system, out _core);
			if (num != 0 || _core == IntPtr.Zero)
			{
				return Fail("no core system", num);
			}
			num = FMOD_System_GetVersion(_core, out var version, out var build);
			if (num != 0)
			{
				return Fail("no version", num);
			}
			num = FMOD_Studio_System_GetBus(_system, Encoding.UTF8.GetBytes("bus:/master/music\0"), out _bus);
			if (num != 0 || _bus == IntPtr.Zero)
			{
				return Fail("no music bus", num);
			}
			num = FMOD_Studio_Bus_LockChannelGroup(_bus);
			if (num != 0)
			{
				return Fail("the music bus would not be held", num);
			}
			_busLocked = true;
			FMOD_Studio_System_FlushCommands(_system);
			num = FMOD_Studio_Bus_GetChannelGroup(_bus, out _group);
			if (num != 0 || _group == IntPtr.Zero)
			{
				FMOD_Studio_Bus_UnlockChannelGroup(_bus);
				_busLocked = false;
				return Fail("the music bus has no channel group", num);
			}
			nint num2 = IntPtr.Zero;
			nint num3 = IntPtr.Zero;
			nint num4 = IntPtr.Zero;
			int[] array = new int[6] { 18, 20, 17, 19, 32, 36 };
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length + 40; i++)
			{
				if (num4 != IntPtr.Zero && (num2 != IntPtr.Zero || num3 != IntPtr.Zero))
				{
					break;
				}
				int num5 = ((i < array.Length) ? array[i] : (i - array.Length + 3));
				if ((i < array.Length || Array.IndexOf(array, num5) < 0) && FMOD_System_CreateDSPByType(_core, num5, out var dsp) == 0 && dsp != IntPtr.Zero)
				{
					string text = NameOf(dsp);
					stringBuilder.Append(num5).Append('=').Append(text)
						.Append("; ");
					string text2 = text.Replace(" ", "").Replace("-", "").ToLowerInvariant();
					if (num2 == IntPtr.Zero && text2.Contains("lowpasssimple"))
					{
						num2 = dsp;
					}
					else if (num3 == IntPtr.Zero && text2.Contains("multibandeq"))
					{
						num3 = dsp;
					}
					else if (num4 == IntPtr.Zero && text2.Contains("reverb") && !text2.Contains("convolution"))
					{
						num4 = dsp;
					}
					else
					{
						FMOD_DSP_Release(dsp);
					}
				}
			}
			if (num2 != IntPtr.Zero && num3 != IntPtr.Zero)
			{
				FMOD_DSP_Release(num3);
				num3 = IntPtr.Zero;
			}
			_lowIsEq = num2 == IntPtr.Zero;
			_low = (_lowIsEq ? num3 : num2);
			_verb = num4;
			if (_low == IntPtr.Zero || _verb == IntPtr.Zero)
			{
				if (_low != IntPtr.Zero)
				{
					FMOD_DSP_Release(_low);
				}
				if (_verb != IntPtr.Zero)
				{
					FMOD_DSP_Release(_verb);
				}
				FMOD_Studio_Bus_UnlockChannelGroup(_bus);
				_busLocked = false;
				_low = (_verb = IntPtr.Zero);
				return Fail("FMOD has no low-pass or no reverb here (" + stringBuilder?.ToString() + ")");
			}
			string text3 = Parameters(_low);
			string text4 = Parameters(_verb);
			if (_lowIsEq)
			{
				_lowKind = Parameter(_low, "A", "Filter");
				_lowCutoff = Parameter(_low, "A", "Freq");
			}
			else
			{
				_lowCutoff = Parameter(_low, "Cutoff");
				if (_lowCutoff < 0)
				{
					_lowCutoff = Parameter(_low, "Freq");
				}
			}
			_verbWet = Parameter(_verb, "Wet");
			_verbDry = Parameter(_verb, "Dry");
			_verbDecay = Parameter(_verb, "Decay");
			_verbHighCut = Parameter(_verb, "High", "Cut");
			if (_lowCutoff < 0 || _verbWet < 0 || (_lowIsEq && _lowKind < 0))
			{
				FMOD_DSP_Release(_low);
				FMOD_DSP_Release(_verb);
				FMOD_Studio_Bus_UnlockChannelGroup(_bus);
				_busLocked = false;
				_low = (_verb = IntPtr.Zero);
				return Fail("the effects do not have the expected parameters (filter: " + text3 + " reverb: " + text4 + ")");
			}
			num = FMOD_DSP_SetBypass(_low, 1);
			if (num != 0)
			{
				return Fail("the filter would not start bypassed", num);
			}
			num = FMOD_DSP_SetBypass(_verb, 1);
			if (num != 0)
			{
				return Fail("the reverb would not start bypassed", num);
			}
			if (_lowIsEq)
			{
				num = FMOD_DSP_SetParameterInt(_low, _lowKind, 2);
				if (num != 0)
				{
					return Fail("the neutral filter mode could not be set", num);
				}
			}
			num = FMOD_DSP_SetParameterFloat(_low, _lowCutoff, 20000f);
			if (num != 0)
			{
				return Fail("the neutral filter cutoff could not be set", num);
			}
			num = FMOD_DSP_SetParameterFloat(_verb, _verbWet, -80f);
			if (num != 0)
			{
				return Fail("the neutral reverb level could not be set", num);
			}
			if (_verbDry >= 0)
			{
				num = FMOD_DSP_SetParameterFloat(_verb, _verbDry, 0f);
				if (num != 0)
				{
					return Fail("the neutral reverb dry level could not be set", num);
				}
			}
			if (_verbDecay >= 0)
			{
				FMOD_DSP_SetParameterFloat(_verb, _verbDecay, Math.Clamp(Tuning.F("mood.room_decay", 2.6f), 0.2f, 12f) * 1000f);
			}
			if (_verbHighCut >= 0)
			{
				FMOD_DSP_SetParameterFloat(_verb, _verbHighCut, Math.Clamp(Tuning.F("mood.room_highcut", 5000f), 500f, 20000f));
			}
			num = FMOD_ChannelGroup_AddDSP(_group, -1, _low);
			if (num == 0)
			{
				_lowAdded = true;
			}
			if (num == 0)
			{
				num = FMOD_ChannelGroup_AddDSP(_group, -1, _verb);
				if (num == 0)
				{
					_verbAdded = true;
				}
			}
			if (num != 0)
			{
				FMOD_ChannelGroup_RemoveDSP(_group, _low);
				FMOD_ChannelGroup_RemoveDSP(_group, _verb);
				FMOD_DSP_Release(_low);
				FMOD_DSP_Release(_verb);
				FMOD_Studio_Bus_UnlockChannelGroup(_bus);
				_busLocked = (_lowAdded = (_verbAdded = false));
				_low = (_verb = IntPtr.Zero);
				return Fail("the effects could not be put on the music bus", num);
			}
			if (_meterIn == IntPtr.Zero)
			{
				_meterIn = Marshal.AllocHGlobal(272);
				_meterOut = Marshal.AllocHGlobal(272);
			}
			FMOD_DSP_SetMeteringEnabled(_low, 1, 1);
			_lowOff = (_verbOff = true);
			_cutoffNow = -1f;
			_wetNow = float.NaN;
			_attached = true;
			_busLocked = true;
			_why = "ok";
			DevLog.Info($"FmodNative: FMOD {version >> 16:X}.{(version >> 8) & 0xFF:X2}.{version & 0xFF:X2} (build {build}); on {"bus:/master/music"}: {(_lowIsEq ? "multiband EQ as low-pass" : "low-pass")} + reverb");
			if (PowerFxManager.LogTriggers)
			{
				DevLog.Info("FmodNative: effects seen: " + stringBuilder?.ToString() + "| filter parameters: " + text3 + "| reverb parameters: " + text4);
			}
			return true;
		}
	}
