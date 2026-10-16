// namespace: SilentFX.Core
// type: Tuning (class)
// source lines: 45365-45419
	public static class Tuning
	{
		private static readonly Dictionary<string, (float? Float, int? Integer)> Values = new Dictionary<string, (float?, int?)>(StringComparer.OrdinalIgnoreCase);

		public static void Load()
		{
			Values.Clear();
			try
			{
				string path = Path.Combine(DevLog.ModDir, "tuning.ini");
				if (!File.Exists(path))
				{
					return;
				}
				string[] array = File.ReadAllLines(path);
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i].Trim();
					if (text.Length != 0 && text[0] != '#' && text[0] != ';')
					{
						int num = text.IndexOf('=');
						if (num > 0)
						{
							string s = text.Substring(num + 1).Trim();
							float? item = (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? new float?(result) : ((float?)null));
							int? item2 = (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result2) ? new int?(result2) : ((int?)null));
							Values[text.Substring(0, num).Trim()] = (item, item2);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Tuning load failed: " + ex.Message);
			}
		}

		public static float F(string key, float def)
		{
			if (!Values.TryGetValue(key, out (float?, int?) value))
			{
				return def;
			}
			return value.Item1 ?? def;
		}

		public static int I(string key, int def)
		{
			if (!Values.TryGetValue(key, out (float?, int?) value))
			{
				return def;
			}
			return value.Item2 ?? def;
		}
	}
