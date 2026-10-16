// namespace: SilentFX.Core
// type: Mood (class)
// source lines: 40068-40486
	public static class Mood
	{
		private sealed class Lease
		{
			public float Volume;

			public float Pitch;

			public float Muffle;

			public float Room;

			public float Attack;

			public float Release;

			public double Until;
		}

		private static readonly Dictionary<string, Lease> Leases;

		private static readonly List<string> Gone;

		private static float _volume;

		private static float _pitch;

		private static float _muffle;

		private static float _room;

		private static float _volumeTo;

		private static float _pitchTo;

		private static float _muffleTo;

		private static float _roomTo;

		private static float _volumeRate;

		private static float _pitchRate;

		private static float _muffleRate;

		private static float _roomRate;

		private static float _release;

		private static float _sentMusic;

		private static float _sentPitch;

		private static float _sentAmbience;

		private static ulong _musicId;

		private static ulong _ambienceId;

		private static double _now;

		private static double _said;

		private static bool _ticking;

		private static Node _proxy;

		private static readonly StringName SetVolume;

		private static readonly StringName SetPitch;

		private static readonly StringName IsValid;

		private static readonly StringName MusicEvent;

		private static readonly StringName AmbienceEvent;

		public static float Volume => _volume;

		public static float Pitch => _pitch;

		public static float Muffle => _muffle;

		public static float Room => _room;

		public static bool Busy => Leases.Count > 0;

		public static bool HasMusic => Event(MusicEvent) != null;

		static Mood()
		{
			Leases = new Dictionary<string, Lease>();
			Gone = new List<string>();
			_volume = 1f;
			_pitch = 1f;
			_volumeTo = 1f;
			_pitchTo = 1f;
			_release = 0.6f;
			_sentMusic = 1f;
			_sentPitch = 1f;
			_sentAmbience = 1f;
			SetVolume = StringName.op_Implicit("set_volume");
			SetPitch = StringName.op_Implicit("set_pitch");
			IsValid = StringName.op_Implicit("is_valid");
			MusicEvent = StringName.op_Implicit("_musicEv");
			AmbienceEvent = StringName.op_Implicit("_ambienceEv");
			PlayerSettings.Changed += SettingsChanged;
		}

		private static void SettingsChanged()
		{
			if (!PlayerSettings.MusicEnabled)
			{
				Reset();
			}
		}

		public static void Hold(string key, float volume = 1f, float pitch = 1f, float muffle = 0f, float room = 0f, float attack = 0.4f, float release = 0.6f, float life = 0.25f)
		{
			try
			{
				if (!string.IsNullOrEmpty(key) && PlayerSettings.MusicEnabled)
				{
					bool flag = !Leases.TryGetValue(key, out var value);
					if (flag)
					{
						value = new Lease();
						Leases[key] = value;
					}
					value.Volume = Mathf.Clamp(volume, 0f, 1f);
					value.Pitch = Mathf.Clamp(pitch, 0.25f, 1f);
					value.Muffle = Mathf.Clamp(muffle, 0f, 1f);
					value.Room = Mathf.Clamp(room, 0f, 1f);
					value.Attack = Mathf.Max(0.01f, attack);
					value.Release = Mathf.Max(0.01f, release);
					value.Until = _now + (double)Mathf.Max(0.05f, life);
					if (flag && PowerFxManager.LogTriggers)
					{
						DevLog.Info($"Mood: {key} - volume {value.Volume:F2}, pitch {value.Pitch:F2}, muffle {value.Muffle:F2}, room {value.Room:F2}");
					}
					if (!_ticking)
					{
						_ticking = true;
						Ticker.Add(Update);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Mood.Hold (" + key + "): " + ex.Message);
			}
		}

		public static void Clear(string key, float release = -1f)
		{
			if (key != null && Leases.TryGetValue(key, out var value))
			{
				_release = ((release > 0f) ? release : value.Release);
				Leases.Remove(key);
				if (PowerFxManager.LogTriggers)
				{
					DevLog.Info($"Mood: {key} let go (back in {_release:F2} s)");
				}
			}
		}

		public static void Reset()
		{
			try
			{
				Leases.Clear();
				_volume = (_pitch = (_volumeTo = (_pitchTo = 1f)));
				_muffle = (_room = (_muffleTo = (_roomTo = 0f)));
				Apply(0.0, neutral: true);
				FmodNative.Detach();
				Ticker.Remove(Update);
				_ticking = false;
				WraithAudioWarmup.ScheduleAfterMoodReset();
			}
			catch (Exception ex)
			{
				DevLog.Warn("Mood.Reset: " + ex.Message);
			}
		}

		private static void Update(double dt)
		{
			try
			{
				if (!PlayerSettings.MusicEnabled)
				{
					Reset();
					return;
				}
				_now += dt;
				Gone.Clear();
				foreach (KeyValuePair<string, Lease> lease in Leases)
				{
					if (lease.Value.Until < _now)
					{
						Gone.Add(lease.Key);
					}
				}
				foreach (string item in Gone)
				{
					Clear(item);
				}
				float num = 1f;
				float num2 = 1f;
				float num3 = 0f;
				float num4 = 0f;
				float num5 = 10f;
				foreach (Lease value in Leases.Values)
				{
					num = Mathf.Min(num, value.Volume);
					num2 = Mathf.Min(num2, value.Pitch);
					num3 = Mathf.Max(num3, value.Muffle);
					num4 = Mathf.Max(num4, value.Room);
					num5 = Mathf.Min(num5, value.Attack);
				}
				float dt2 = (float)dt;
				_volume = Toward(_volume, num, ref _volumeTo, ref _volumeRate, dt2, num5, _release, deeperIsLower: true);
				_pitch = Toward(_pitch, num2, ref _pitchTo, ref _pitchRate, dt2, num5, _release, deeperIsLower: true);
				_muffle = Toward(_muffle, num3, ref _muffleTo, ref _muffleRate, dt2, num5, _release, deeperIsLower: false);
				_room = Toward(_room, num4, ref _roomTo, ref _roomRate, dt2, num5, _release, deeperIsLower: false);
				bool flag = Leases.Count == 0 && _volume > 0.999f && _pitch > 0.999f && _muffle < 0.001f && _room < 0.001f;
				if (flag)
				{
					_volume = (_pitch = 1f);
					_muffle = (_room = 0f);
				}
				Apply(dt, flag);
				if (flag)
				{
					Ticker.Remove(Update);
					_ticking = false;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Mood.Update: " + ex.Message);
				Leases.Clear();
				Ticker.Remove(Update);
				_ticking = false;
			}
		}

		private static float Toward(float now, float target, ref float goal, ref float rate, float dt, float attack, float release, bool deeperIsLower)
		{
			if (Mathf.IsEqualApprox(now, target))
			{
				goal = target;
				return target;
			}
			if (!Mathf.IsEqualApprox(goal, target) || rate <= 0f)
			{
				goal = target;
				bool flag = (deeperIsLower ? (target < now) : (target > now));
				rate = Mathf.Abs(target - now) / Mathf.Max(0.01f, flag ? attack : release);
			}
			return Mathf.MoveToward(now, target, dt * rate);
		}

		private static GodotObject Event(StringName field)
		{
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Invalid comparison between Unknown and I8
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!FxKit.Alive((GodotObject)(object)_proxy) || !_proxy.IsInsideTree())
				{
					_proxy = null;
					NRun instance = NRun.Instance;
					NRunMusicController val = ((instance != null) ? instance.RunMusicController : null);
					if (!FxKit.Alive((GodotObject)(object)val))
					{
						return null;
					}
					_proxy = ((Node)val).GetNodeOrNull<Node>(NodePath.op_Implicit("Proxy"));
					if (!FxKit.Alive((GodotObject)(object)_proxy))
					{
						return null;
					}
				}
				Variant val2 = ((GodotObject)_proxy).Get(field);
				if ((long)val2.VariantType != 24)
				{
					return null;
				}
				GodotObject val3 = val2.AsGodotObject();
				if (!FxKit.Alive(val3))
				{
					return null;
				}
				if (val3.HasMethod(IsValid))
				{
					Variant val4 = val3.Call(IsValid, Array.Empty<Variant>());
					if (!val4.AsBool())
					{
						return null;
					}
				}
				return val3;
			}
			catch
			{
				_proxy = null;
				return null;
			}
		}

		private static void Apply(double dt, bool neutral)
		{
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			float num = Mathf.Clamp(Sfx.Knob("mood.ambience", 1f), 0f, 1f);
			float num2 = Mathf.Lerp(1f, _volume, num);
			GodotObject val = Event(MusicEvent);
			if (val != null)
			{
				ulong instanceId = val.GetInstanceId();
				if (instanceId != _musicId)
				{
					_musicId = instanceId;
					_sentMusic = 1f;
					_sentPitch = 1f;
				}
				if (Mathf.Abs(_volume - _sentMusic) > 0.003f || (neutral && _sentMusic != 1f))
				{
					val.Call(SetVolume, new Variant[1] { Variant.op_Implicit(_volume) });
					_sentMusic = _volume;
				}
				if (Mathf.Abs(_pitch - _sentPitch) > 0.002f || (neutral && _sentPitch != 1f))
				{
					val.Call(SetPitch, new Variant[1] { Variant.op_Implicit(_pitch) });
					_sentPitch = _pitch;
				}
			}
			GodotObject val2 = Event(AmbienceEvent);
			if (val2 != null)
			{
				ulong instanceId2 = val2.GetInstanceId();
				if (instanceId2 != _ambienceId)
				{
					_ambienceId = instanceId2;
					_sentAmbience = 1f;
				}
				if (Mathf.Abs(num2 - _sentAmbience) > 0.003f || (neutral && _sentAmbience != 1f))
				{
					val2.Call(SetVolume, new Variant[1] { Variant.op_Implicit(num2) });
					_sentAmbience = num2;
				}
			}
			bool num3 = !neutral && (_muffle > 0.004f || _room > 0.004f) && Sfx.Knob("mood.dsp", 1f) > 0.5f;
			float num4 = 20000f;
			float num5 = -80f;
			if (num3)
			{
				float num6 = Mathf.Clamp(Sfx.Knob("mood.cutoff_min", 420f), 80f, 8000f);
				num4 = 20000f * Mathf.Pow(num6 / 20000f, _muffle);
				num5 = ((_room <= 0.004f) ? (-80f) : Mathf.Lerp(-34f, Mathf.Clamp(Sfx.Knob("mood.room_max", -5f), -30f, 6f), _room));
				if (FmodNative.Want(dt))
				{
					FmodNative.Set(num4, num5);
				}
			}
			else if (FmodNative.Attached)
			{
				FmodNative.Set(20000f, -80f);
			}
			if (!(!PowerFxManager.LogTriggers | neutral))
			{
				_said += dt;
				if (!(_said < 0.5))
				{
					_said = 0.0;
					string value = (FmodNative.Meter(out var input, out var output) ? $", through the filter {input:F1} -> {output:F1} dB" : "");
					DevLog.Info($"Mood: volume {_volume:F2}, pitch {_pitch:F2}, cutoff {num4:F0} Hz, reverb {num5:F0} dB; music {((val != null) ? "ok" : "none")}, filter {(FmodNative.Attached ? "on" : (FmodNative.Dead ? "unavailable" : "not yet"))}{value}");
				}
			}
		}

		public static string Report()
		{
			if (!PlayerSettings.MusicEnabled)
			{
				return "音乐处理：总开关关着";
			}
			if (!HasMusic)
			{
				return "游戏现在没在放音乐，没东西可处理（换一场有音乐的战斗再试）";
			}
			string text;
			if (FmodNative.Dead)
			{
				text = "没接上（原因在日志里）";
			}
			else
			{
				text = (FmodNative.Attached ? "已接上" : "待命");
			}
			if (FmodNative.Meter(out var input, out var output))
			{
				text += $" {input:F0}→{output:F0} dB";
			}
			return $"音乐 音量{_volume * 100f:F0}% 速度{_pitch * 100f:F0}% 远{_muffle * 100f:F0}% 空{_room * 100f:F0}%｜滤波：{text}";
		}
	}
