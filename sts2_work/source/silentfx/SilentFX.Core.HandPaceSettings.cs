// namespace: SilentFX.Core
// type: HandPaceSettings (class)
// source lines: 39665-40067
	public static class HandPaceSettings
	{
		private const string ModId = "SilentFX";

		private const string PaceKey = "hand_pace";

		private const string ActionReset = "reset_defaults";

		public static object CreateRitsuLibSettingsSchema()
		{
			try
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["pageId"] = "main";
				dictionary["title"] = "静默猎手设置 / SilentFX settings";
				dictionary["sections"] = new object[4]
				{
					new Dictionary<string, object>
					{
						["id"] = "general",
						["title"] = "总开关与手牌 / General and hand",
						["entries"] = new object[2]
						{
							Toggle("master", "SilentFX总开关 / SilentFX master switch", "关闭后恢复游戏原版表现；已开始的短动画会自然结束，持续音效和屏幕效果由模组清理。各分项选择会保留。"),
							Choice("hand_pace", "手牌动画速度 / Hand animation speed", "慢速 = 完整的抽牌 / 弃牌动画；快速 = 同样的动画压缩播放；关闭 = 游戏原本的动画。")
						}
					},
					new Dictionary<string, object>
					{
						["id"] = "sound",
						["title"] = "音频 / Audio",
						["entries"] = new object[3]
						{
							Toggle("additional_audio", "模组附加音效 / Additional mod sounds", "仅控制 SilentFX 添加的音效。"),
							new Dictionary<string, object>
							{
								["id"] = "audio_volume",
								["type"] = "slider",
								["key"] = "audio_volume",
								["label"] = "模组音量 / Mod volume",
								["description"] = "0 至 100%；默认 100%。",
								["min"] = 0,
								["max"] = 100,
								["step"] = 1
							},
							Toggle("music_processing", "特殊音乐处理 / Special music processing", "允许 SilentFX 对游戏当前音乐做效果处理；关闭后保持原版音乐。")
						}
					},
					new Dictionary<string, object>
					{
						["id"] = "visual",
						["title"] = "视觉与可访问性 / Visuals and accessibility",
						["entries"] = new object[2]
						{
							Toggle("screen_shake", "模组震屏 / Mod screen shake", "控制 SilentFX 添加的屏幕震动。"),
							Toggle("reduce_effects", "减少闪光和强屏幕效果 / Reduce flashes and strong screen effects", "默认关闭；开启后使用较弱的闪光与屏幕效果。")
						}
					},
					new Dictionary<string, object>
					{
						["id"] = "performance",
						["title"] = "性能 / Performance",
						["entries"] = new object[3]
						{
							Toggle("resource_prewarm", "资源预缓存 / Resource pre-warming", "关闭后暂停准备任务；重新开启后继续剩余任务。"),
							Toggle("shader_warmup", "着色器预热 / Shader warm-up", "关闭后暂停着色器预热；重新开启后继续剩余任务。"),
							new Dictionary<string, object>
							{
								["id"] = "reset_defaults",
								["type"] = "button",
								["key"] = "reset_defaults",
								["label"] = "恢复本页默认 / Reset this page",
								["buttonText"] = "恢复默认 / Reset defaults",
								["description"] = "只恢复此页九项设置，保留其他设置和音效方案。"
							}
						}
					}
				};
				Dictionary<string, object> dictionary2 = dictionary;
				dictionary = new Dictionary<string, object>();
				dictionary["modId"] = "SilentFX";
				dictionary["modDisplayName"] = "SilentFX 猎人卡牌特效";
				dictionary["pages"] = new object[1] { dictionary2 };
				return dictionary;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Player settings schema could not be built: " + ex.Message);
				return null;
			}
		}

		public static object GetRitsuLibSettingValue(string key)
		{
			if (IsPace(key))
			{
				return GetRitsuLibSettingString(key);
			}
			if (IsVolume(key))
			{
				return GetRitsuLibSettingDouble(key);
			}
			return GetRitsuLibSettingBool(key);
		}

		public static void SetRitsuLibSettingValue(string key, object value)
		{
			if (IsPace(key))
			{
				SetRitsuLibSettingString(key, Convert.ToString(value, CultureInfo.InvariantCulture));
				return;
			}
			if (IsVolume(key))
			{
				try
				{
					SetRitsuLibSettingDouble(key, Convert.ToDouble(value, CultureInfo.InvariantCulture));
					return;
				}
				catch (Exception ex)
				{
					DevLog.Warn("Settings volume write failed: " + ex.Message);
					return;
				}
			}
			if (TryBoolean(value, out var result))
			{
				SetRitsuLibSettingBool(key, result);
			}
		}

		public static string GetRitsuLibSettingString(string key)
		{
			if (!IsPace(key))
			{
				return "";
			}
			return ToValue(HandPace.Preference);
		}

		public static void SetRitsuLibSettingString(string key, string value)
		{
			if (IsPace(key))
			{
				switch ((value ?? "").Trim().ToLowerInvariant())
				{
				case "slow":
				case "2":
					HandPace.Set(HandPace.Mode.Slow);
					break;
				case "fast":
				case "1":
					HandPace.Set(HandPace.Mode.Fast);
					break;
				case "off":
				case "0":
					HandPace.Set(HandPace.Mode.Off);
					break;
				default:
					DevLog.Warn("Settings sent an unknown hand pace value '" + value + "'");
					break;
				}
			}
		}

		public static int GetRitsuLibSettingInt(string key)
		{
			if (!IsVolume(key))
			{
				if (!IsPace(key))
				{
					return 0;
				}
				return (int)HandPace.Preference;
			}
			return (int)Math.Round(PlayerSettings.AudioVolume * 100f);
		}

		public static void SetRitsuLibSettingInt(string key, int value)
		{
			bool value2;
			if (IsVolume(key))
			{
				PlayerSettings.SetAudioVolume((float)Math.Clamp(value, 0, 100) / 100f);
			}
			else if (IsPace(key))
			{
				HandPace.Set(HandPace.FromNumber(value));
			}
			else if (TryReadBooleanKey(key, out value2))
			{
				SetRitsuLibSettingBool(key, value != 0);
			}
		}

		public static double GetRitsuLibSettingDouble(string key)
		{
			if (!IsVolume(key))
			{
				return GetRitsuLibSettingInt(key);
			}
			return Math.Round(PlayerSettings.AudioVolume * 100f);
		}

		public static void SetRitsuLibSettingDouble(string key, double value)
		{
			if (IsVolume(key) && !double.IsNaN(value) && !double.IsInfinity(value))
			{
				PlayerSettings.SetAudioVolume((float)(Math.Clamp(value, 0.0, 100.0) / 100.0));
			}
		}

		public static bool GetRitsuLibSettingBool(string key)
		{
			bool value;
			return TryReadBooleanKey(key, out value) & value;
		}

		public static void SetRitsuLibSettingBool(string key, bool value)
		{
			string text = (key ?? "").Trim().ToLowerInvariant();
			if (text == null)
			{
				return;
			}
			switch (text.Length)
			{
			case 16:
				switch (text[0])
				{
				case 'a':
					if (text == "additional_audio")
					{
						PlayerSettings.SetAdditionalAudioEnabled(value);
					}
					break;
				case 'm':
					if (text == "music_processing")
					{
						PlayerSettings.SetMusicEnabled(value);
					}
					break;
				case 'r':
					if (text == "resource_prewarm")
					{
						PlayerSettings.SetPrewarmEnabled(value);
					}
					break;
				}
				break;
			case 6:
				if (text == "master")
				{
					PlayerSettings.SetEnabled(value);
				}
				break;
			case 12:
				if (text == "screen_shake")
				{
					PlayerSettings.SetScreenShakeEnabled(value);
				}
				break;
			case 14:
				if (text == "reduce_effects")
				{
					PlayerSettings.SetReduceScreenEffects(value);
				}
				break;
			case 13:
				if (text == "shader_warmup")
				{
					PlayerSettings.SetShaderWarmupEnabled(value);
				}
				break;
			}
		}

		public static void SaveRitsuLibSettings()
		{
		}

		public static void InvokeRitsuLibSettingAction(string actionId)
		{
			if (string.Equals(actionId, "reset_defaults", StringComparison.OrdinalIgnoreCase))
			{
				PlayerSettings.ResetPageDefaults();
			}
		}

		private static bool TryReadBooleanKey(string key, out bool value)
		{
			value = false;
			switch ((key ?? "").Trim().ToLowerInvariant())
			{
			case "master":
				value = PlayerSettings.Enabled;
				return true;
			case "additional_audio":
				value = PlayerSettings.AdditionalAudioPreference;
				return true;
			case "music_processing":
				value = PlayerSettings.MusicPreference;
				return true;
			case "screen_shake":
				value = PlayerSettings.ScreenShakePreference;
				return true;
			case "reduce_effects":
				value = PlayerSettings.ReduceScreenEffects;
				return true;
			case "resource_prewarm":
				value = PlayerSettings.PrewarmEnabled;
				return true;
			case "shader_warmup":
				value = PlayerSettings.ShaderWarmupEnabled;
				return true;
			default:
				return false;
			}
		}

		private static bool TryBoolean(object value, out bool result)
		{
			if (value is bool flag)
			{
				result = flag;
				return true;
			}
			string text = Convert.ToString(value, CultureInfo.InvariantCulture)?.Trim();
			if (bool.TryParse(text, out result))
			{
				return true;
			}
			if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result2))
			{
				result = result2 != 0;
				return true;
			}
			result = false;
			return false;
		}

		private static bool IsPace(string key)
		{
			return string.Equals(key, "hand_pace", StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsVolume(string key)
		{
			return string.Equals(key, "audio_volume", StringComparison.OrdinalIgnoreCase);
		}

		private static string ToValue(HandPace.Mode mode)
		{
			return mode switch
			{
				HandPace.Mode.Fast => "fast", 
				HandPace.Mode.Off => "off", 
				_ => "slow", 
			};
		}

		private static Dictionary<string, object> Toggle(string key, string label, string description)
		{
			return new Dictionary<string, object>
			{
				["id"] = key,
				["type"] = "toggle",
				["key"] = key,
				["label"] = label,
				["description"] = description
			};
		}

		private static Dictionary<string, object> Choice(string key, string label, string description)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["id"] = key;
			dictionary["type"] = "choice";
			dictionary["key"] = key;
			dictionary["label"] = label;
			dictionary["description"] = description;
			dictionary["presentation"] = ((Tuning.I("hand.settings_dropdown", 0) != 0) ? "dropdown" : "stepper");
			dictionary["options"] = new object[3]
			{
				new Dictionary<string, object>
				{
					["value"] = "slow",
					["label"] = "慢速 / Slow"
				},
				new Dictionary<string, object>
				{
					["value"] = "fast",
					["label"] = "快速 / Fast"
				},
				new Dictionary<string, object>
				{
					["value"] = "off",
					["label"] = "关闭 / Off"
				}
			};
			return dictionary;
		}
	}
