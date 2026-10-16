// namespace: SilentFX.Cards
// type: SuppressStage (class)
// source lines: 108947-109344
	internal static class SuppressStage
	{
		private static GDScript _script;

		private static bool _tried;

		private static bool _ok;

		private static Dictionary _cache;

		private static readonly List<HandCards.Entry> Hand = new List<HandCards.Entry>();

		private static readonly Action<double> Watcher = Watch;

		private static readonly Action<double> Warmer = WarmStep;

		private static double _watchClock;

		private static bool _warming;

		private static bool _warmed;

		private static Node _warmNode;

		public static string Dir => Path.Combine(DevLog.ModDir, "suppress3").Replace('\\', '/');

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

		public static Dictionary Cache
		{
			get
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Expected Obj, but got Unknown
				Dictionary val = _cache;
				if (val == null)
				{
					Dictionary val2 = new Dictionary();
					_cache = val2;
					val = val2;
				}
				return val;
			}
		}

		public static void Init()
		{
			try
			{
				if (AssetWarmup.Enabled)
				{
					AssetWarmup.Add("Suppress script", (Action)Warm);
				}
				else
				{
					Ticker.Add(Watcher);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("SuppressStage.Init: " + ex.Message);
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
					if (Hand[i].Card is Suppress)
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
				DevLog.Warn("SuppressStage.Watch: " + ex.Message);
				Ticker.Remove(Watcher);
			}
		}

		private static void Warm()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected Obj, but got Unknown
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			if (_warmed || _warming || !Ready)
			{
				return;
			}
			try
			{
				_warmNode = new Node
				{
					Name = StringName.op_Implicit("SilentFX_SuppressWarm")
				};
				((GodotObject)_warmNode).SetScript(Variant.op_Implicit((GodotObject)(object)_script));
				_warming = true;
				if (AssetWarmup.Enabled)
				{
					AssetWarmup.Add("Suppress resources", () =>
					{
						WarmStep(0.0);
						return !_warming;
					}, StopWarm);
				}
				else
				{
					Ticker.Add(Warmer);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("SuppressStage.Warm: " + ex.Message);
				_warming = false;
			}
		}

		private static void WarmStep(double realDt)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Invalid comparison between Unknown and I8
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Invalid comparison between Unknown and I8
			bool flag = true;
			try
			{
				if (_warmNode != null && GodotObject.IsInstanceValid((GodotObject)(object)_warmNode))
				{
					int count = Cache.Count;
					Variant val = ((GodotObject)_warmNode).Call(StringName.op_Implicit("warm"), new Variant[2]
					{
						Variant.op_Implicit(Dir),
						Variant.op_Implicit(Cache)
					});
					if ((long)val.VariantType == 1 && val.AsBool())
					{
						_warmed = true;
						AssetWarmup.Add("Suppress ring texture", () =>
						{
							//IL_0000: Unknown result type (might be due to invalid IL or missing references)
							//IL_0006: Expected Obj, but got Unknown
							//IL_000c: Unknown result type (might be due to invalid IL or missing references)
							//IL_0026: Unknown result type (might be due to invalid IL or missing references)
							//IL_0040: Unknown result type (might be due to invalid IL or missing references)
							Node val4 = new Node();
							try
							{
								((GodotObject)val4).SetScript(Variant.op_Implicit((GodotObject)(object)_script));
								((GodotObject)val4).Set(StringName.op_Implicit("CACHE"), Variant.op_Implicit(Cache));
								((GodotObject)val4).Call(StringName.op_Implicit("ring_tex"), Array.Empty<Variant>());
							}
							finally
							{
								((GodotObject)val4).Free();
							}
						});
						foreach (Variant value in Cache.Values)
						{
							Variant current = value;
							if ((long)current.VariantType == 24)
							{
								GodotObject val2 = current.AsGodotObject();
								Shader val3 = (Shader)(object)((val2 is Shader) ? val2 : null);
								if (val3 != null)
								{
									ShaderWarmup.Add(val3);
								}
							}
						}
						DevLog.Info("Suppress: pictures and shaders read in");
					}
					else
					{
						if (Cache.Count == count)
						{
							throw new IOException("resource warm-up made no progress; check missing/invalid textures");
						}
						flag = false;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("SuppressStage.WarmStep: " + ex.Message);
			}
			if (flag)
			{
				StopWarm();
			}
		}

		private static void StopWarm()
		{
			Ticker.Remove(Warmer);
			_warming = false;
			try
			{
				if (_warmNode != null && GodotObject.IsInstanceValid((GodotObject)(object)_warmNode))
				{
					((GodotObject)_warmNode).Free();
				}
			}
			catch
			{
			}
			_warmNode = null;
		}

		private static void Load()
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected Obj, but got Unknown
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			_tried = true;
			_ok = false;
			try
			{
				if (Tuning.F("suppress.v3", 1f) < 0.5f)
				{
					DevLog.Info("Suppress: the new performance is switched off (suppress.v3 0)");
					return;
				}
				string text = Dir + "/suppress_stage.gd";
				if (!File.Exists(text))
				{
					DevLog.Error("Suppress: script missing: " + text + " - the old apparition is used");
					return;
				}
				GDScript val = new GDScript();
				((Script)val).SourceCode = File.ReadAllText(text);
				Error val2 = ((Script)val).Reload(false);
				if ((int)val2 != 0 || !((Script)val).CanInstantiate())
				{
					DevLog.Error("Suppress: suppress_stage.gd did not compile (" + ((object)val2/*cast due to constrained. prefix*/).ToString() + ") - the old apparition is used");
					return;
				}
				_script = val;
				_ok = true;
				DevLog.Info("Suppress: stage script compiled");
			}
			catch (Exception ex)
			{
				_ok = false;
				DevLog.Error("SuppressStage.Load: " + ex);
			}
		}

		public static void Fail(string why)
		{
			if (_ok)
			{
				DevLog.Error("Suppress: " + why + " - the old apparition takes over");
			}
			_ok = false;
		}

		public static Node Make(Node2D back, Node2D front, Vector2 lo, Vector2 hi, bool remoteUi = false)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected Obj, but got Unknown
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Expected Obj, but got Unknown
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Invalid comparison between Unknown and I8
			if (!PlayerSettings.Enabled || !Ready || !FxKit.Alive((GodotObject)(object)back) || !FxKit.Alive((GodotObject)(object)front))
			{
				return null;
			}
			Node val = null;
			try
			{
				val = new Node
				{
					Name = StringName.op_Implicit("SilentFX_SuppressStage")
				};
				((GodotObject)val).SetScript(Variant.op_Implicit((GodotObject)(object)_script));
				((Node)back).AddChild(val, false, (InternalMode)0);
				Dictionary val2 = new Dictionary
				{
					[Variant.op_Implicit("size")] = Variant.op_Implicit(Mathf.Clamp(Tuning.F("suppress.v3.size", 1f), 0.5f, 1.6f)),
					[Variant.op_Implicit("hit_wait")] = Variant.op_Implicit(Mathf.Clamp(Tuning.F("suppress.hit_wait", 0.8f), 0.1f, 2f)),
					[Variant.op_Implicit("reduce_effects")] = Variant.op_Implicit(PlayerSettings.ReduceScreenEffects),
					[Variant.op_Implicit("remote_ui")] = Variant.op_Implicit(remoteUi)
				};
				Variant val7 = ((GodotObject)val).Call(StringName.op_Implicit("build"), new Variant[7]
				{
					Variant.op_Implicit(Dir),
					Variant.op_Implicit(Cache),
					Variant.op_Implicit((GodotObject)(object)back),
					Variant.op_Implicit((GodotObject)(object)front),
					Variant.op_Implicit(lo),
					Variant.op_Implicit(hi),
					Variant.op_Implicit(val2)
				});
				if ((long)val7.VariantType == 1 && val7.AsBool())
				{
					return val;
				}
				Fail("the stage could not be built");
			}
			catch (Exception ex)
			{
				Fail("the stage could not be built (" + ex.Message + ")");
			}
			if (FxKit.Alive((GodotObject)(object)val))
			{
				val.QueueFree();
			}
			return null;
		}
	}
