// namespace: SilentFX.Cards
// type: SuppressFrames (class)
// source lines: 108221-108557
	internal static class SuppressFrames
	{
		internal sealed class Frame
		{
			public string File;

			public Texture2D Tex;

			public Vector2 Front;

			public Vector2 Chest;

			public Vector2 Head;

			public Vector2 Shoulder;

			public float V;

			public float Fade;

			public bool Snapped;
		}

		public static readonly List<Frame> Intro = new List<Frame>();

		public static readonly List<Frame> Idle = new List<Frame>();

		public static readonly List<Frame> Aim = new List<Frame>();

		public static readonly List<Frame> Attack = new List<Frame>();

		public static float Fps = 24f;

		public static Vector2 Size = new Vector2(430f, 460f);

		public static Vector2 Ground = new Vector2(215f, 450f);

		public static float Faces = -1f;

		public static int AttackFromAim = 3;

		public static int Cut = 9;

		public static int SlowEnd = 36;

		private static bool _tried;

		private static bool _ok;

		private static readonly List<Frame> Queue = new List<Frame>();

		private static readonly Action<double> Loader = LoadStep;

		private static readonly Action<double> Watcher = Watch;

		private static bool _loading;

		private static bool _watching;

		private static double _idle;

		public static string Dir => Path.Combine(DevLog.ModDir, "suppress").Replace('\\', '/');

		public static bool Ok
		{
			get
			{
				if (!_tried)
				{
					Read();
				}
				return _ok;
			}
		}

		public static bool IntroReady
		{
			get
			{
				if (Ok && Loaded(Intro) && Idle.Count > 0)
				{
					return Idle[0].Tex != null;
				}
				return false;
			}
		}

		public static bool AttackReady
		{
			get
			{
				if (Ok)
				{
					return Loaded(Attack);
				}
				return false;
			}
		}

		private static bool Loaded(List<Frame> list)
		{
			if (list.Count == 0)
			{
				return false;
			}
			foreach (Frame item in list)
			{
				if (item.Tex == null)
				{
					return false;
				}
			}
			return true;
		}

		public static void Warm()
		{
			if (!Ok)
			{
				return;
			}
			_idle = 0.0;
			if (!_watching)
			{
				_watching = true;
				Ticker.Add(Watcher);
			}
			if (_loading)
			{
				return;
			}
			Queue.Clear();
			List<Frame>[] array = new List<Frame>[4] { Intro, Idle, Aim, Attack };
			for (int i = 0; i < array.Length; i++)
			{
				foreach (Frame item in array[i])
				{
					if (item.Tex == null)
					{
						Queue.Add(item);
					}
				}
			}
			if (Queue.Count != 0)
			{
				_loading = true;
				Ticker.Add(Loader);
			}
		}

		private static void Read()
		{
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			_tried = true;
			_ok = false;
			try
			{
				if (Tuning.F("suppress.on", 1f) < 0.5f)
				{
					return;
				}
				string text = Dir + "/suppress_frames.json";
				if (!File.Exists(text))
				{
					DevLog.Error("Suppress: " + text + " missing (run docs/ref/suppress_cut/attack_v1/export_game_202.py) - no hunter");
					return;
				}
				using JsonDocument jsonDocument = JsonDocument.Parse(File.ReadAllText(text));
				JsonElement rootElement = jsonDocument.RootElement;
				Fps = Num(rootElement, "fps", 24f);
				Size = Vec(rootElement, "size", Size);
				Ground = Vec(rootElement, "ground", Ground);
				Faces = ((Num(rootElement, "faces", -1f) < 0f) ? (-1f) : 1f);
				AttackFromAim = (int)Num(rootElement, "attack_from_aim", 3f);
				Cut = (int)Num(rootElement, "cut", 9f);
				SlowEnd = (int)Num(rootElement, "slow_end", 36f);
				JsonElement property = rootElement.GetProperty("groups");
				Fill(property, "intro", Intro);
				Fill(property, "idle", Idle);
				Fill(property, "aim", Aim);
				Fill(property, "attack", Attack);
				_ok = Intro.Count > 0 && Idle.Count > 0 && Aim.Count == Idle.Count && Attack.Count > Cut + 2;
				DevLog.Info(_ok ? $"Suppress: hunter frames listed ({Intro.Count} intro, {Idle.Count} idle, {Aim.Count} aim, {Attack.Count} attack, {Size.X}x{Size.Y})" : "Suppress: suppress_frames.json is incomplete - no hunter");
			}
			catch (Exception ex)
			{
				_ok = false;
				DevLog.Error("Suppress: frames could not be read: " + ex.Message);
			}
		}

		private static float Num(JsonElement e, string name, float fallback)
		{
			if (!e.TryGetProperty(name, out var value) || value.ValueKind != JsonValueKind.Number)
			{
				return fallback;
			}
			return (float)value.GetDouble();
		}

		private static Vector2 Vec(JsonElement e, string name, Vector2 fallback)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			if (!e.TryGetProperty(name, out var value) || value.ValueKind != JsonValueKind.Array || value.GetArrayLength() < 2)
			{
				return fallback;
			}
			return new Vector2((float)value[0].GetDouble(), (float)value[1].GetDouble());
		}

		private static void Fill(JsonElement groups, string name, List<Frame> list)
		{
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			list.Clear();
			if (!groups.TryGetProperty(name, out var value) || value.ValueKind != JsonValueKind.Array)
			{
				return;
			}
			foreach (JsonElement item in value.EnumerateArray())
			{
				list.Add(new Frame
				{
					File = item.GetProperty("file").GetString(),
					Front = Vec(item, "f", Vector2.Zero),
					Chest = Vec(item, "c", Vector2.Zero),
					Head = Vec(item, "h", Vector2.Zero),
					Shoulder = Vec(item, "sh", Vector2.Zero),
					V = Num(item, "v", 0f),
					Fade = Num(item, "f5", 0f),
					Snapped = (item.TryGetProperty("snapped", out var value2) && value2.ValueKind == JsonValueKind.True)
				});
			}
		}

		private static void LoadStep(double realDt)
		{
			int num = Mathf.Clamp(Tuning.I("suppress.load_per_frame", 4), 1, 32);
			while (num-- > 0 && Queue.Count > 0)
			{
				Frame frame = Queue[0];
				Queue.RemoveAt(0);
				if (frame.Tex != null)
				{
					continue;
				}
				try
				{
					Image val = Image.LoadFromFile(Dir + "/" + frame.File);
					try
					{
						if (val != null && !val.IsEmpty())
						{
							frame.Tex = (Texture2D)(object)ImageTexture.CreateFromImage(val);
						}
						else
						{
							DevLog.Warn("Suppress: picture empty: " + frame.File);
						}
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Suppress: picture failed: " + frame.File + " (" + ex.Message + ")");
				}
			}
			if (Queue.Count == 0)
			{
				_loading = false;
				Ticker.Remove(Loader);
			}
		}

		private static void Watch(double realDt)
		{
			if (GameRefs.CombatRoom != null || _loading)
			{
				_idle = 0.0;
				return;
			}
			_idle += realDt;
			if (_idle < 20.0)
			{
				return;
			}
			List<Frame>[] array = new List<Frame>[4] { Intro, Idle, Aim, Attack };
			for (int i = 0; i < array.Length; i++)
			{
				foreach (Frame item in array[i])
				{
					item.Tex = null;
				}
			}
			_watching = false;
			Ticker.Remove(Watcher);
			DevLog.Info("Suppress: hunter pictures released");
		}

		public static void Sample(List<Frame> list, float t, float fpsOfList, out Frame a, out Frame b, out float mix)
		{
			int count = list.Count;
			float num = t * fpsOfList;
			num -= Mathf.Floor(num / (float)count) * (float)count;
			int num2 = (int)Mathf.Floor(num) % count;
			a = list[num2];
			b = list[(num2 + 1) % count];
			mix = num - Mathf.Floor(num);
		}

		static SuppressFrames()
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		}
	}
