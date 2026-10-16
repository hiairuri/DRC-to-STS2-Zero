// namespace: SilentFX.Core
// type: ShaderLib (class)
// source lines: 44829-44936
	public static class ShaderLib
	{
		private static readonly Dictionary<string, Shader> Cache = new Dictionary<string, Shader>();

		private static readonly HashSet<string> Missing = new HashSet<string>();

		internal static void Warm()
		{
			string text = Path.Combine(DevLog.ModDir, "shaders");
			if (!Directory.Exists(text))
			{
				DevLog.Warn("Shader directory missing: " + text);
				return;
			}
			string[] files = Directory.GetFiles(text, "*.gdshader");
			foreach (string path in files)
			{
				string name = Path.GetFileNameWithoutExtension(path);
				try
				{
					if (AssetWarmup.Enabled)
					{
						AssetWarmup.Add("shader/" + name, () =>
						{
							ShaderWarmup.Add(Get(name));
						});
					}
					else
					{
						Get(name);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Shader warm-up failed for " + name + ": " + ex.Message);
				}
			}
		}

		private static string PathOf(string name)
		{
			return Path.Combine(DevLog.ModDir, "shaders", name + ".gdshader");
		}

		public static Shader Get(string name)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected Obj, but got Unknown
			if (Cache.TryGetValue(name, out var value) && FxKit.Alive((GodotObject)(object)value))
			{
				return value;
			}
			if (Missing.Contains(name))
			{
				return null;
			}
			string text = PathOf(name);
			if (!File.Exists(text))
			{
				Missing.Add(name);
				DevLog.Error("Shader file missing: " + text);
				return null;
			}
			Shader val = new Shader
			{
				Code = File.ReadAllText(text)
			};
			Cache[name] = val;
			return val;
		}

		public static ShaderMaterial NewMaterial(string name)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected Obj, but got Unknown
			Shader val = Get(name);
			if (val != null)
			{
				return new ShaderMaterial
				{
					Shader = val
				};
			}
			return null;
		}

		public static void Reload()
		{
			Missing.Clear();
			foreach (KeyValuePair<string, Shader> item in Cache)
			{
				try
				{
					string path = PathOf(item.Key);
					if (FxKit.Alive((GodotObject)(object)item.Value) && File.Exists(path))
					{
						item.Value.Code = File.ReadAllText(path);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Shader reload failed for " + item.Key + ": " + ex.Message);
				}
			}
		}
	}
