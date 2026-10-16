// namespace: SilentFX.Cards
// type: WraithStage (class)
// source lines: 121152-121883
	internal static class WraithStage
	{
		private static GDScript _form;

		private static GDScript _play;

		private static bool _tried;

		private static bool _ok;

		private static Dictionary _cache;

		private static readonly List<string> Pending = new List<string>();

		private static readonly List<HandCards.Entry> Hand = new List<HandCards.Entry>();

		private static readonly Action<double> Warmer = WarmStep;

		private static readonly Action<double> Watcher = Watch;

		private static bool _warming;

		private static bool _warmed;

		private static double _watchClock;

		private static bool _loggedFirstActualPlay;

		private static double _pictureWarmCpuMs;

		private static readonly StringName MSetParam = StringName.op_Implicit("set_param");

		public static string Dir => Path.Combine(DevLog.ModDir, "wraith").Replace('\\', '/');

		public static bool Ready
		{
			get
			{
				if (!_tried)
				{
					Load();
				}
				return _ok;
			}
		}

		public static void Init()
		{
			try
			{
				WraithAudioWarmup.Init();
				if (AssetWarmup.Enabled)
				{
					AssetWarmup.Add("Wraith scripts", (Action)Warm);
				}
				else
				{
					Ticker.Add(Watcher);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("WraithStage.Init: " + ex.Message);
			}
		}

		private static void Load()
		{
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Expected Obj, but got Unknown
			_tried = true;
			_ok = false;
			try
			{
				if (Tuning.F("wraith.v2", 1f) < 0.5f)
				{
					DevLog.Info("Wraith Form: the new apparition is switched off (wraith.v2 0)");
					return;
				}
				string dir = Dir;
				if (!Directory.Exists(dir))
				{
					DevLog.Error("Wraith Form: folder missing: " + dir + " - the old apparition is used");
					return;
				}
				_form = Compile(dir + "/wraith_form.gd");
				_play = Compile(dir + "/wraith_play.gd");
				_cache = new Dictionary();
				_ok = _form != null && _play != null;
				DevLog.Info(_ok ? "Wraith Form: scripts compiled" : "Wraith Form: scripts did not compile - the old apparition is used");
			}
			catch (Exception ex)
			{
				_ok = false;
				DevLog.Error("WraithStage.Load: " + ex);
			}
		}

		private static GDScript Compile(string path)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected Obj, but got Unknown
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (!File.Exists(path))
			{
				DevLog.Error("Wraith Form: script missing: " + path);
				return null;
			}
			GDScript val = new GDScript();
			((Script)val).SourceCode = File.ReadAllText(path);
			Error val2 = ((Script)val).Reload(false);
			if ((int)val2 != 0 || !((Script)val).CanInstantiate())
			{
				DevLog.Error("Wraith Form: " + Path.GetFileName(path) + " did not compile (" + ((object)val2/*cast due to constrained. prefix*/).ToString() + ")");
				return null;
			}
			return val;
		}

		public static void Fail(string why)
		{
			if (_ok)
			{
				DevLog.Error("Wraith Form: " + why + " - the old apparition takes over");
			}
			_ok = false;
		}

		public static void Place(CardFxContext c, Vector2 lo, Vector2 hi, out Vector2 org, out float k, out Vector2 who)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = hi - lo;
			float num = Mathf.Clamp(Tuning.F("wraith.figure.size", 1f), 0.4f, 1.6f);
			k = 0.62f * val.Y / 1080f * num;
			float num2 = k / 0.62f;
			float num3 = Mathf.Clamp(c.Body.Y, 160f, 620f);
			who = new Vector2(c.Chest.X, c.Feet.Y - num3 * 0.5f);
			float num4 = lo.X + val.X * 0.5f;
			float num5 = lo.Y + 12f * num2;
			if (Tuning.F("wraith.figure.follow", 1f) >= 0.5f)
			{
				float num6 = Mathf.Min(380f * num2, val.X * 0.5f);
				num4 = Mathf.Clamp(c.Chest.X, lo.X + num6, hi.X - num6);
				num5 = Mathf.Clamp(c.Feet.Y - 1440f * k, lo.Y + 6f, lo.Y + val.Y * 0.12f);
			}
			org = new Vector2(num4 - 606f * k, num5);
		}

		public static Node MakeForm(Node back, Node front, Vector2 lo, Vector2 hi, Vector2 org, float k)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected Obj, but got Unknown
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Invalid comparison between Unknown and I8
			if (!Ready || !FxKit.Alive((GodotObject)(object)back) || !FxKit.Alive((GodotObject)(object)front))
			{
				return null;
			}
			Node val = null;
			try
			{
				val = new Node
				{
					Name = StringName.op_Implicit("SilentFX_WraithForm")
				};
				((GodotObject)val).SetScript(Variant.op_Implicit((GodotObject)(object)_form));
				back.AddChild(val, false, (InternalMode)0);
				Variant val2 = ((GodotObject)val).Call(StringName.op_Implicit("build"), new Variant[8]
				{
					Variant.op_Implicit(Dir),
					Variant.op_Implicit(_cache),
					Variant.op_Implicit((GodotObject)(object)back),
					Variant.op_Implicit((GodotObject)(object)front),
					Variant.op_Implicit(lo),
					Variant.op_Implicit(hi),
					Variant.op_Implicit(org),
					Variant.op_Implicit(k)
				});
				if ((long)val2.VariantType == 1 && val2.AsBool())
				{
					return val;
				}
				Fail("the standby could not be built");
			}
			catch (Exception ex)
			{
				Fail("the standby could not be built (" + ex.Message + ")");
			}
			if (FxKit.Alive((GodotObject)(object)val))
			{
				val.QueueFree();
			}
			return null;
		}

		public static Node MakePlay(Node back, Node front, Vector2 lo, Vector2 hi, Vector2 org, float k, Vector2 who)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected Obj, but got Unknown
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Invalid comparison between Unknown and I8
			if (!Ready || !FxKit.Alive((GodotObject)(object)back) || !FxKit.Alive((GodotObject)(object)front))
			{
				return null;
			}
			Node val = null;
			long timestamp = Stopwatch.GetTimestamp();
			try
			{
				val = new Node
				{
					Name = StringName.op_Implicit("SilentFX_WraithPlay")
				};
				((GodotObject)val).SetScript(Variant.op_Implicit((GodotObject)(object)_play));
				back.AddChild(val, false, (InternalMode)0);
				Variant val2 = ((GodotObject)val).Call(StringName.op_Implicit("build"), new Variant[9]
				{
					Variant.op_Implicit(Dir),
					Variant.op_Implicit(_cache),
					Variant.op_Implicit((GodotObject)(object)back),
					Variant.op_Implicit((GodotObject)(object)front),
					Variant.op_Implicit(lo),
					Variant.op_Implicit(hi),
					Variant.op_Implicit(org),
					Variant.op_Implicit(k),
					Variant.op_Implicit(who)
				});
				if ((long)val2.VariantType == 1 && val2.AsBool())
				{
					return val;
				}
				Fail("the playing of the card could not be built");
			}
			catch (Exception ex)
			{
				Fail("the playing of the card could not be built (" + ex.Message + ")");
			}
			finally
			{
				if (!_loggedFirstActualPlay)
				{
					_loggedFirstActualPlay = true;
					DevLog.Info($"Wraith Form: first actual MakePlay/build CPU {Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds:F1} ms (CPU only; excludes GPU/render)");
				}
			}
			if (FxKit.Alive((GodotObject)(object)val))
			{
				val.QueueFree();
			}
			return null;
		}

		public static void Tune(Node script, CardFxContext c = null, Vector2 lo = default(Vector2), Vector2 hi = default(Vector2))
		{
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_028f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0294: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0304: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0359: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0363: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!FxKit.Alive((GodotObject)(object)script) || !((GodotObject)script).HasMethod(MSetParam))
				{
					return;
				}
				float num = Mathf.Clamp(Tuning.F("wraith.ui.soft", 0.03f), 0.004f, 0.2f);
				float num2 = Mathf.Clamp(Tuning.F("wraith.ui.top", 0.15f), 0f, 0.45f);
				float num3 = Mathf.Clamp(Tuning.F("wraith.ui.bottom", 0.8f), 0.55f, 1f);
				float num4 = Mathf.Clamp(Tuning.F("wraith.ui.corner", 0.7f), 0.45f, 1f);
				if (c != null && hi.Y - lo.Y > 100f)
				{
					float num5 = hi.Y - lo.Y;
					float num6 = c.Feet.Y;
					float num7 = c.Feet.Y - Mathf.Max(c.Body.Y, 0f);
					if (c.Enemies != null)
					{
						foreach (FxTarget enemy in c.Enemies)
						{
							if (enemy != null)
							{
								num6 = Mathf.Max(num6, enemy.Feet.Y);
								num7 = Mathf.Min(num7, enemy.Feet.Y - Mathf.Max(enemy.Height, 0f));
							}
						}
					}
					if (float.IsFinite(num6) && float.IsFinite(num7))
					{
						float num8 = (num6 - lo.Y) / num5 + num * 0.5f + 0.008f;
						float num9 = (num7 - lo.Y) / num5 - num * 0.5f - 0.008f;
						if (num8 < 0.97f)
						{
							num3 = Mathf.Max(num3, num8);
							num4 = Mathf.Max(num4, num8);
						}
						if (num9 > 0.03f)
						{
							num2 = Mathf.Min(num2, num9);
						}
					}
				}
				((GodotObject)script).Call(MSetParam, new Variant[2]
				{
					Variant.op_Implicit("ui_top"),
					Variant.op_Implicit(Mathf.Clamp(num2, 0f, 0.45f))
				});
				((GodotObject)script).Call(MSetParam, new Variant[2]
				{
					Variant.op_Implicit("ui_bot"),
					Variant.op_Implicit(Mathf.Clamp(num3, 0.55f, 1f))
				});
				((GodotObject)script).Call(MSetParam, new Variant[2]
				{
					Variant.op_Implicit("ui_corner"),
					Variant.op_Implicit(Mathf.Clamp(num4, 0.45f, 1f))
				});
				((GodotObject)script).Call(MSetParam, new Variant[2]
				{
					Variant.op_Implicit("ui_left"),
					Variant.op_Implicit(Mathf.Clamp(Tuning.F("wraith.ui.left", 0.17f), 0f, 0.45f))
				});
				((GodotObject)script).Call(MSetParam, new Variant[2]
				{
					Variant.op_Implicit("ui_right"),
					Variant.op_Implicit(Mathf.Clamp(Tuning.F("wraith.ui.right", 0.8f), 0.55f, 1f))
				});
				((GodotObject)script).Call(MSetParam, new Variant[2]
				{
					Variant.op_Implicit("ui_soft"),
					Variant.op_Implicit(num)
				});
				((GodotObject)script).Call(MSetParam, new Variant[2]
				{
					Variant.op_Implicit("blur"),
					Variant.op_Implicit(Mathf.Clamp(Tuning.F("wraith.blur", 1f), 0f, 3f))
				});
			}
			catch (Exception ex)
			{
				DevLog.Warn("Wraith Form (told where the interface is): " + ex.Message);
			}
		}

		public static bool FightOver()
		{
			try
			{
				CombatManager instance = CombatManager.Instance;
				return instance != null && instance.IsOverOrEnding;
			}
			catch
			{
				return false;
			}
		}

		private static void Watch(double realDt)
		{
			try
			{
				if (_warmed || _warming)
				{
					Ticker.Remove(Watcher);
					return;
				}
				_watchClock += realDt;
				if (_watchClock < 0.75)
				{
					return;
				}
				_watchClock = 0.0;
				if (GameRefs.CombatRoom == null)
				{
					return;
				}
				HandCards.Fill(Hand);
				bool flag = false;
				for (int i = 0; i < Hand.Count; i++)
				{
					if (Hand[i].Card is WraithForm)
					{
						flag = true;
					}
				}
				Hand.Clear();
				if (flag)
				{
					Warm();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("WraithStage.Watch: " + ex.Message);
				Ticker.Remove(Watcher);
			}
		}

		public static void Warm()
		{
			if (_warmed || _warming || !Ready)
			{
				return;
			}
			try
			{
				Pending.Clear();
				string dir = Dir;
				string[] array = new string[3] { "tex", "tex2", "tex3" };
				foreach (string text in array)
				{
					string path = dir + "/" + text;
					if (Directory.Exists(path))
					{
						string[] files = Directory.GetFiles(path, "*.png");
						foreach (string path2 in files)
						{
							Pending.Add(text + "/" + Path.GetFileName(path2));
						}
					}
				}
				Pending.Add("rig");
				Pending.Add("rig2");
				_warming = true;
				if (AssetWarmup.Enabled)
				{
					AssetWarmup.Add("Wraith pictures", () =>
					{
						WarmStep(0.0);
						return !_warming;
					}, () =>
					{
						Pending.Clear();
						_warming = false;
					});
					array = Directory.GetFiles(dir, "*.gdshader");
					foreach (string path3 in array)
					{
						string name = Path.GetFileName(path3);
						AssetWarmup.Add("Wraith shader/" + name, () =>
						{
							WarmShader(name);
						});
					}
					AssetWarmup.Add("Wraith plain shader variants", () =>
					{
						WarmShader(null);
					});
				}
				else
				{
					Ticker.Add(Warmer);
				}
			}
			catch (Exception ex)
			{
				_warming = false;
				_warmed = true;
				DevLog.Warn("WraithStage.Warm: " + ex.Message);
			}
		}

		private static void WarmStep(double realDt)
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Invalid comparison between Unknown and I8
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			long timestamp = Stopwatch.GetTimestamp();
			try
			{
				string dir = Dir;
				Stopwatch stopwatch = Stopwatch.StartNew();
				int num = (AssetWarmup.Enabled ? 1 : 4);
				while (Pending.Count > 0 && num-- > 0 && stopwatch.ElapsedMilliseconds < 4)
				{
					string text = Pending[Pending.Count - 1];
					Pending.RemoveAt(Pending.Count - 1);
					if (_cache == null || _cache.ContainsKey(Variant.op_Implicit(text)))
					{
						continue;
					}
					if (text == "rig" || text == "rig2")
					{
						string path = dir + "/" + text + ".json";
						if (File.Exists(path))
						{
							Variant val = Json.ParseString(File.ReadAllText(path));
							if ((long)val.VariantType == 27)
							{
								_cache[Variant.op_Implicit(text)] = val;
							}
						}
						continue;
					}
					Image val2 = Image.LoadFromFile(dir + "/" + text);
					try
					{
						if (val2 == null || val2.IsEmpty())
						{
							DevLog.Warn("Wraith prewarm: picture missing or empty: " + text);
						}
						else
						{
							_cache[Variant.op_Implicit(text)] = Variant.op_Implicit((GodotObject)(object)ImageTexture.CreateFromImage(val2));
						}
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
				if (Pending.Count <= 0)
				{
					_warming = false;
					_warmed = true;
					Ticker.Remove(Warmer);
					DevLog.Info($"Wraith Form: pictures read in; CPU {_pictureWarmCpuMs + Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds:F1} ms");
					WraithSceneWarmup.Queue(_form, _play, _cache);
				}
			}
			catch (Exception ex)
			{
				Pending.Clear();
				_warming = false;
				_warmed = true;
				Ticker.Remove(Warmer);
				DevLog.Warn($"WraithStage.WarmStep failed after CPU {_pictureWarmCpuMs + Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds:F1} ms: {ex.Message}");
			}
			finally
			{
				_pictureWarmCpuMs += Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds;
			}
		}

		private static void WarmShader(string file)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Expected Obj, but got Unknown
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			if (!Ready)
			{
				return;
			}
			Node val = new Node();
			try
			{
				((GodotObject)val).SetScript(Variant.op_Implicit((GodotObject)(object)_form));
				((GodotObject)val).Set(StringName.op_Implicit("DIR"), Variant.op_Implicit(Dir));
				((GodotObject)val).Set(StringName.op_Implicit("CACHE"), Variant.op_Implicit(_cache));
				string text = "shader_type canvas_item;\nvoid fragment() {\n}\n";
				Variant val2;
				if (file != null)
				{
					val2 = ((GodotObject)val).Call(StringName.op_Implicit("shader_code"), new Variant[2]
					{
						Variant.op_Implicit(file),
						Variant.op_Implicit("")
					});
					GodotObject val3 = val2.AsGodotObject();
					GodotObject obj = ((val3 is Shader) ? val3 : null) ?? throw new IOException("could not prepare " + file);
					ShaderWarmup.Add((Shader)(object)obj);
					text = ((Shader)obj).Code;
					if (file == "body.gdshader")
					{
						val2 = ((GodotObject)val).Call(StringName.op_Implicit("shader_code"), new Variant[2]
						{
							Variant.op_Implicit(file),
							Variant.op_Implicit("render_mode blend_add;")
						});
						GodotObject val4 = val2.AsGodotObject();
						ShaderWarmup.Add((Shader)(object)((val4 is Shader) ? val4 : null));
					}
				}
				val2 = ((GodotObject)val).Call(StringName.op_Implicit("zoned"), new Variant[2]
				{
					Variant.op_Implicit(text),
					Variant.op_Implicit(false)
				});
				GodotObject val5 = val2.AsGodotObject();
				ShaderWarmup.Add((Shader)(object)((val5 is Shader) ? val5 : null));
				if (file == null)
				{
					val2 = ((GodotObject)val).Call(StringName.op_Implicit("zoned"), new Variant[2]
					{
						Variant.op_Implicit(text),
						Variant.op_Implicit(true)
					});
					GodotObject val6 = val2.AsGodotObject();
					ShaderWarmup.Add((Shader)(object)((val6 is Shader) ? val6 : null));
				}
			}
			finally
			{
				((GodotObject)val).Free();
			}
		}
	}
}
