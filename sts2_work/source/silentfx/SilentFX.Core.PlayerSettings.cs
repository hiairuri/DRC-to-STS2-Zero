// namespace: SilentFX.Core
// type: PlayerSettings (class)
// source lines: 40598-41085
	public static class PlayerSettings
	{
		private sealed class Snapshot
		{
			internal readonly bool Enabled;

			internal readonly bool AudioPreference;

			internal readonly bool MusicPreference;

			internal readonly bool ShakePreference;

			internal readonly bool Reduce;

			internal readonly bool Prewarm;

			internal readonly bool Shader;

			internal readonly float Volume;

			internal readonly int Pace;

			internal Snapshot(bool enabled, bool audio, float volume, bool music, bool shake, bool reduce, bool prewarm, bool shader, int pace)
			{
				Enabled = enabled;
				AudioPreference = audio;
				Volume = volume;
				MusicPreference = music;
				ShakePreference = shake;
				Reduce = reduce;
				Prewarm = prewarm;
				Shader = shader;
				Pace = pace;
			}
		}

		private const string UserFile = "user_settings.ini";

		private const string MasterKey = "fx.enabled";

		private const string PaceKey = "hand.pace";

		private const string AudioKey = "sfx.on";

		private const string VolumeKey = "sfx.volume";

		private const string MusicKey = "mood.on";

		private const string ShakeKey = "fx.screen_shake";

		private const string ReduceKey = "fx.reduce_screen_effects";

		private const string PrewarmKey = "perf.prewarm";

		private const string ShaderKey = "perf.shader_warmup";

		private static readonly object Sync = new object();

		private static readonly Dictionary<string, string> Values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		private static bool _loaded;

		private static Snapshot _snapshot;

		public static bool Enabled => Current.Enabled;

		public static bool AdditionalAudioEnabled
		{
			get
			{
				Snapshot current = Current;
				if (current.Enabled)
				{
					return current.AudioPreference;
				}
				return false;
			}
		}

		internal static bool AdditionalAudioPreference => Current.AudioPreference;

		public static float AudioVolume => Current.Volume;

		public static bool MusicEnabled
		{
			get
			{
				Snapshot current = Current;
				if (current.Enabled)
				{
					return current.MusicPreference;
				}
				return false;
			}
		}

		internal static bool MusicPreference => Current.MusicPreference;

		public static bool ScreenShakeEnabled
		{
			get
			{
				Snapshot current = Current;
				if (current.Enabled)
				{
					return current.ShakePreference;
				}
				return false;
			}
		}

		internal static bool ScreenShakePreference => Current.ShakePreference;

		public static bool ReduceScreenEffects => Current.Reduce;

		public static bool PrewarmEnabled => Current.Prewarm;

		public static bool ShaderWarmupEnabled => Current.Shader;

		private static Snapshot Current
		{
			get
			{
				Snapshot snapshot = Volatile.Read(in _snapshot);
				if (snapshot != null)
				{
					return snapshot;
				}
				lock (Sync)
				{
					EnsureLoaded();
					return _snapshot;
				}
			}
		}

		public static event Action Changed;

		public static void Init()
		{
			lock (Sync)
			{
				EnsureLoaded();
				PublishSnapshot();
			}
			DevLog.Info("PlayerSettings: loaded user_settings.ini");
		}

		public static void SetEnabled(bool value)
		{
			SetBoolean("fx.enabled", value);
		}

		public static void SetAdditionalAudioEnabled(bool value)
		{
			SetBoolean("sfx.on", value);
		}

		public static void SetAudioVolume(float value)
		{
			if (!float.IsFinite(value))
			{
				DevLog.Warn("PlayerSettings: ignored a non-finite audio volume");
			}
			else
			{
				SetText("sfx.volume", Clamp(value).ToString("0.###", CultureInfo.InvariantCulture));
			}
		}

		public static void SetMusicEnabled(bool value)
		{
			SetBoolean("mood.on", value);
		}

		public static void SetScreenShakeEnabled(bool value)
		{
			SetBoolean("fx.screen_shake", value);
		}

		public static void SetReduceScreenEffects(bool value)
		{
			SetBoolean("fx.reduce_screen_effects", value);
		}

		public static void SetPrewarmEnabled(bool value)
		{
			SetBoolean("perf.prewarm", value);
		}

		public static void SetShaderWarmupEnabled(bool value)
		{
			SetBoolean("perf.shader_warmup", value);
		}

		internal static int GetHandPacePreference()
		{
			return Current.Pace;
		}

		internal static void SetHandPacePreference(int value)
		{
			SetText("hand.pace", Math.Clamp(value, 0, 2).ToString(CultureInfo.InvariantCulture));
		}

		public static void ResetPageDefaults()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				["fx.enabled"] = "1",
				["hand.pace"] = "2",
				["sfx.on"] = "1",
				["sfx.volume"] = "1",
				["mood.on"] = "1",
				["fx.screen_shake"] = "1",
				["fx.reduce_screen_effects"] = "0",
				["perf.prewarm"] = "1",
				["perf.shader_warmup"] = "1"
			};
			lock (Sync)
			{
				EnsureLoaded();
				foreach (KeyValuePair<string, string> item in dictionary)
				{
					Values[item.Key] = item.Value;
				}
				SaveUser();
				PublishSnapshot();
			}
			RaiseChanged();
		}

		private static bool ReadBoolean(string key, bool defaultValue, string legacyTuningKey = null, int legacyDefault = 0)
		{
			if (Values.TryGetValue(key, out var value))
			{
				if (bool.TryParse(value, out var result))
				{
					return result;
				}
				if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result2))
				{
					return result2 != 0;
				}
				if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result3) && float.IsFinite(result3))
				{
					return result3 != 0f;
				}
				return defaultValue;
			}
			if (legacyTuningKey != null)
			{
				return Tuning.I(legacyTuningKey, legacyDefault) != 0;
			}
			return defaultValue;
		}

		private static float ReadNumber(string key, float defaultValue, string legacyTuningKey = null, float legacyDefault = 0f)
		{
			float result;
			if (Values.TryGetValue(key, out var value))
			{
				if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result) || !float.IsFinite(result))
				{
					result = defaultValue;
				}
			}
			else
			{
				result = ((legacyTuningKey == null) ? defaultValue : Tuning.F(legacyTuningKey, legacyDefault));
				if (!float.IsFinite(result))
				{
					result = defaultValue;
				}
			}
			return Clamp(result);
		}

		private static float Clamp(float value)
		{
			if (!float.IsFinite(value))
			{
				return 1f;
			}
			return Math.Clamp(value, 0f, 1f);
		}

		private static void SetBoolean(string key, bool value)
		{
			SetText(key, value ? "1" : "0");
		}

		private static void SetText(string key, string value)
		{
			lock (Sync)
			{
				EnsureLoaded();
				if (Values.TryGetValue(key, out var value2) && string.Equals(value2, value, StringComparison.OrdinalIgnoreCase))
				{
					return;
				}
				Values[key] = value;
				SaveUser();
				PublishSnapshot();
			}
			RaiseChanged();
		}

		private static void EnsureLoaded()
		{
			if (_loaded)
			{
				if (_snapshot == null)
				{
					PublishSnapshot();
				}
				return;
			}
			string modDir = DevLog.ModDir;
			if (string.IsNullOrEmpty(modDir))
			{
				if (_snapshot == null)
				{
					PublishSnapshot();
				}
				return;
			}
			_loaded = true;
			try
			{
				string path = Path.Combine(modDir, "user_settings.ini");
				if (!File.Exists(path))
				{
					return;
				}
				string[] array = File.ReadAllLines(path);
				for (int i = 0; i < array.Length; i++)
				{
					if (TrySplit(array[i], out var key, out var value))
					{
						Values[key] = value;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("PlayerSettings: could not read user_settings.ini: " + ex.Message);
			}
			PublishSnapshot();
		}

		private static void PublishSnapshot()
		{
			string text = (Values.TryGetValue("hand.pace", out var value) ? value : null);
			int pace = Math.Clamp(Tuning.I("hand.pace", 2), 0, 2);
			if (text != null)
			{
				if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
				{
					pace = Math.Clamp(result, 0, 2);
				}
				else
				{
					switch (text.Trim().ToLowerInvariant())
					{
					case "off":
						pace = 0;
						break;
					case "fast":
						pace = 1;
						break;
					case "slow":
						pace = 2;
						break;
					}
				}
			}
			Volatile.Write(ref _snapshot, new Snapshot(ReadBoolean("fx.enabled", defaultValue: true), ReadBoolean("sfx.on", defaultValue: true, "sfx.on", 1), ReadNumber("sfx.volume", 1f, "sfx.volume", 1f), ReadBoolean("mood.on", defaultValue: true, "mood.on", 1), ReadBoolean("fx.screen_shake", defaultValue: true, "fx.screen_shake", 1), ReadBoolean("fx.reduce_screen_effects", defaultValue: false), ReadBoolean("perf.prewarm", defaultValue: true, "perf.prewarm", 1), ReadBoolean("perf.shader_warmup", defaultValue: true, "perf.shader_warmup", 1), pace));
		}

		private static bool TrySplit(string raw, out string key, out string value)
		{
			key = null;
			value = null;
			if (raw == null)
			{
				return false;
			}
			string text = raw.Trim();
			if (text.Length == 0 || text[0] == '#' || text[0] == ';')
			{
				return false;
			}
			int num = text.IndexOf('=');
			if (num <= 0)
			{
				return false;
			}
			key = text.Substring(0, num).Trim();
			value = text.Substring(num + 1).Trim();
			return key.Length > 0;
		}

		private static void SaveUser()
		{
			string modDir = DevLog.ModDir;
			if (string.IsNullOrEmpty(modDir))
			{
				DevLog.Warn("PlayerSettings: mod path is not ready; this setting is only in memory");
				return;
			}
			string text = Path.Combine(modDir, "user_settings.ini");
			string text2 = text + ".tmp";
			try
			{
				List<string> list = (File.Exists(text) ? new List<string>(File.ReadAllLines(text)) : new List<string> { "# SilentFX: player choices from Settings > Mod设置（RitsuLib）> SilentFX.", "# Unknown settings are kept when this page saves or resets." });
				HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				List<string> list2 = new List<string>(list.Count + Values.Count);
				foreach (string item in list)
				{
					string value2;
					if (!TrySplit(item, out var key, out var _))
					{
						list2.Add(item);
					}
					else if (!Values.TryGetValue(key, out value2))
					{
						list2.Add(item);
					}
					else if (hashSet.Add(key))
					{
						list2.Add(key + " = " + value2);
					}
				}
				foreach (KeyValuePair<string, string> value3 in Values)
				{
					if (hashSet.Add(value3.Key))
					{
						list2.Add(value3.Key + " = " + value3.Value);
					}
				}
				File.WriteAllLines(text2, list2, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
				if (File.Exists(text))
				{
					File.Move(text2, text, overwrite: true);
				}
				else
				{
					File.Move(text2, text);
				}
			}
			catch (Exception ex)
			{
				try
				{
					if (File.Exists(text2))
					{
						File.Delete(text2);
					}
				}
				catch
				{
				}
				DevLog.Warn("PlayerSettings: could not write user_settings.ini: " + ex.Message);
			}
		}

		private static void RaiseChanged()
		{
			Action changed = Changed;
			if (changed == null)
			{
				return;
			}
			Delegate[] invocationList = changed.GetInvocationList();
			foreach (Delegate obj in invocationList)
			{
				try
				{
					((Action)obj)();
				}
				catch (Exception ex)
				{
					DevLog.Warn("PlayerSettings change listener failed: " + ex.Message);
				}
			}
		}
	}
