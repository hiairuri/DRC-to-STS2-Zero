// namespace: SilentFX
// type: DevLog (class)
// source lines: 101-210
	public static class DevLog
	{
		private static readonly object Lock = new object();

		private static readonly object FileLock = new object();

		private static readonly StringBuilder Pending = new StringBuilder();

		private static Timer _flushTimer;

		private static string _path;

		public static string ModDir { get; private set; } = "";

		public static void Start()
		{
			try
			{
				ModDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
				if (string.IsNullOrEmpty(ModDir))
				{
					ModDir = Path.Combine(Path.GetDirectoryName(OS.GetExecutablePath()) ?? ".", "mods", "SilentFX");
				}
				_path = Path.Combine(ModDir, "silentfx.log");
				File.WriteAllText(_path, $"=== SilentFX log {DateTime.Now:yyyy-MM-dd HH:mm:ss} ==={Environment.NewLine}");
				if (_flushTimer == null)
				{
					AppDomain.CurrentDomain.ProcessExit += OnExit;
					_flushTimer = new Timer((object? _) =>
					{
						Flush();
					}, null, 500, 500);
				}
			}
			catch (Exception ex)
			{
				_path = null;
				GD.PrintErr("[SilentFX] could not open log file: " + ex.Message);
			}
		}

		public static void Info(string msg)
		{
			Write("INFO", msg);
		}

		public static void Warn(string msg)
		{
			Write("WARN", msg);
		}

		public static void Error(string msg)
		{
			Write("ERROR", msg);
		}

		private static void Write(string level, string msg)
		{
			string text = $"{DateTime.Now:HH:mm:ss.fff} [{level}] {msg}";
			if (level != "INFO" || Tuning.I("perf.console_log", 0) != 0)
			{
				GD.Print("[SilentFX] " + text);
			}
			if (_path == null)
			{
				return;
			}
			try
			{
				lock (Lock)
				{
					Pending.Append(text).Append(Environment.NewLine);
				}
			}
			catch
			{
			}
		}

		internal static void Flush()
		{
			try
			{
				lock (FileLock)
				{
					string contents;
					lock (Lock)
					{
						if (_path == null || Pending.Length == 0)
						{
							return;
						}
						contents = Pending.ToString();
						Pending.Clear();
					}
					File.AppendAllText(_path, contents);
				}
			}
			catch
			{
			}
		}

		private static void OnExit(object sender, EventArgs args)
		{
			_flushTimer?.Dispose();
			Flush();
		}
	}
	[ModInitializer("Init")]
