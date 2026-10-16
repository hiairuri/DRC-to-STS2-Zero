// namespace: SilentFX.Dev
// type: TourUi (class)
// source lines: 29551-30545
	public static class TourUi
	{
		private static readonly Color OpusTint = new Color(0.91f, 0.57f, 0.4f, 1f);

		private static readonly Color CheeseTint = new Color(0.96f, 0.82f, 0.38f, 1f);

		private static readonly Color TextTint = new Color(0.965f, 0.945f, 0.91f, 1f);

		private static readonly Color MetaTint = new Color(0.69f, 0.72f, 0.705f, 1f);

		private static readonly Color CueTint = new Color(0.59f, 0.67f, 0.65f, 1f);

		private const float Gutter = 96f;

		private const float GutterGap = 22f;

		private static CanvasLayer _layer;

		private static bool _ticking;

		private static Control _stop;

		private static ColorRect _fog;

		private static ShaderMaterial _fogMaterial;

		private static VBoxContainer _box;

		private static ColorRect _rule;

		private static readonly List<(CanvasItem Item, float At)> Rows = new List<(CanvasItem, float)>();

		private static float _stopClock;

		private static bool _stopShown;

		private static bool _busy;

		private static bool _captions = true;

		private static float _stopAlpha;

		private static float _sinceHeld = 99f;

		private static Control _page;

		private static ColorRect _pageDim;

		private static readonly List<(CanvasItem Item, float Shown)> Paragraphs = new List<(CanvasItem, float)>();

		private static bool _pageShown;

		private static float _pageAlpha;

		private static float _pageClock;

		private static VBoxContainer _pageColumn;

		private static float _pageFade = 0.7f;

		private static int _pageFits;

		private static int _pageShrunk;

		private static Label _pageHint;

		private static IReadOnlyList<TourScreen> _screens;

		private static int _screen;

		private static Label _toast;

		private static float _toastLeft;

		private static Font _medium;

		private static Font _bold;

		public static bool IsOpen => FxKit.Alive((GodotObject)(object)_layer);

		public static bool CaptionsVisible => _captions;

		public static bool PageUp => _pageShown;

		private static Font Medium => _medium ?? (_medium = GameFont("medium", 500));

		private static Font Bold => _bold ?? (_bold = GameFont("bold", 700));

		public static void Open()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected Obj, but got Unknown
			try
			{
				if (FxKit.Alive((GodotObject)(object)_layer))
				{
					return;
				}
				SceneTree tree = Ticker.Tree;
				if (tree != null)
				{
					_layer = new CanvasLayer
					{
						Name = StringName.op_Implicit("SilentFX_Tour"),
						Layer = 96
					};
					((Node)tree.Root).AddChild((Node)(object)_layer, false, (InternalMode)0);
					BuildStop();
					BuildPage();
					BuildToast();
					_stopShown = false;
					_pageShown = false;
					_busy = false;
					_stopAlpha = 0f;
					_pageAlpha = 0f;
					_sinceHeld = 99f;
					if (!_ticking)
					{
						Ticker.AddLate(Tick);
						_ticking = true;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourUi.Open: " + ex.Message);
			}
		}

		public static void Close()
		{
			try
			{
				if (_ticking)
				{
					Ticker.Remove(Tick);
					_ticking = false;
				}
				if (FxKit.Alive((GodotObject)(object)_layer))
				{
					((Node)_layer).QueueFree();
				}
				_layer = null;
				_stop = null;
				_fog = null;
				_fogMaterial = null;
				_box = null;
				_rule = null;
				_page = null;
				_pageDim = null;
				_pageHint = null;
				_toast = null;
				Rows.Clear();
				Paragraphs.Clear();
				_screens = null;
				_stopShown = false;
				_pageShown = false;
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourUi.Close: " + ex.Message);
			}
		}

		public static void ShowIntro(IReadOnlyList<TourScreen> screens)
		{
			ShowScreens(screens);
		}

		public static void ShowOutro(IReadOnlyList<TourScreen> screens)
		{
			ShowScreens(screens);
		}

		private static void ShowScreens(IReadOnlyList<TourScreen> screens)
		{
			_screens = screens ?? Array.Empty<TourScreen>();
			_screen = 0;
			ShowScreen((_screens.Count > 0) ? _screens[0] : new TourScreen());
		}

		public static void ShowNothing()
		{
			try
			{
				if (!FxKit.Alive((GodotObject)(object)_layer))
				{
					Open();
				}
				_pageShown = false;
				_stopShown = false;
				_busy = false;
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourUi.ShowNothing: " + ex.Message);
			}
		}

		public static void ShowFinale(string text)
		{
			_screens = null;
			ShowPage(null, new string[1] { text ?? "" }, all: true, finale: true);
		}

		public static bool Advance()
		{
			try
			{
				if (!_pageShown)
				{
					return false;
				}
				for (int i = 0; i < Paragraphs.Count; i++)
				{
					if (!(Paragraphs[i].Shown >= 0f))
					{
						Paragraphs[i] = (Paragraphs[i].Item, _pageClock);
						return true;
					}
				}
				if (_screens != null && _screen + 1 < _screens.Count)
				{
					_screen++;
					ShowScreen(_screens[_screen]);
					return true;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourUi.Advance: " + ex.Message);
			}
			return false;
		}

		public static void ShowStop(TourStop stop, int index, int count)
		{
			try
			{
				if (!FxKit.Alive((GodotObject)(object)_layer))
				{
					Open();
				}
				if (FxKit.Alive((GodotObject)(object)_box) && stop != null)
				{
					_pageShown = false;
					_busy = false;
					FillStop(stop, index, count);
					_stopShown = true;
					_stopClock = 0f;
					_sinceHeld = 99f;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourUi.ShowStop: " + ex.Message);
			}
		}

		public static void SetBusy(bool busy)
		{
			_busy = busy;
		}

		public static void SetCaptionsVisible(bool visible)
		{
			_captions = visible;
		}

		public static void Toast(string text, float seconds = 5f)
		{
			try
			{
				if (FxKit.Alive((GodotObject)(object)_toast))
				{
					_toast.Text = text ?? "";
					_toastLeft = seconds;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourUi.Toast: " + ex.Message);
			}
		}

		private static Font GameFont(string weight, int systemWeight)
		{
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Expected Obj, but got Unknown
			string[] array = new string[2]
			{
				"res://themes/fonts/zhs/source_han_serif_sc_" + weight + "_shared.tres",
				"res://fonts/zhs/SourceHanSerifSC-" + ((weight == "bold") ? "Bold" : "Medium") + ".otf"
			};
			foreach (string text in array)
			{
				try
				{
					if (ResourceLoader.Exists(text, ""))
					{
						Font val = ResourceLoader.Load<Font>(text, (string)null, (CacheMode)1);
						if (val != null)
						{
							return val;
						}
					}
				}
				catch
				{
				}
			}
			DevLog.Info("TourUi: the game's serif (" + weight + ") is not there - using the system's");
			SystemFont val2 = new SystemFont();
			val2.FontNames = new string[6] { "Source Han Serif SC", "Noto Serif CJK SC", "SimSun", "Microsoft YaHei UI", "Microsoft YaHei", "serif" };
			val2.FontWeight = systemWeight;
			return (Font)(object)val2;
		}

		private static Label Text(string text, Font font, int size, Color color, float width = 0f, float spacing = 0f)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Expected Obj, but got Unknown
			//IL_0089: Expected Obj, but got Unknown
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			Label val = new Label
			{
				Text = (text ?? ""),
				MouseFilter = (MouseFilterEnum)2,
				LabelSettings = new LabelSettings
				{
					Font = font,
					FontSize = size,
					FontColor = color,
					LineSpacing = spacing,
					ShadowSize = Mathf.Max(3, size / 6),
					ShadowColor = new Color(0f, 0f, 0f, 0.62f),
					ShadowOffset = new Vector2(0f, 2f)
				}
			};
			if (width > 0f)
			{
				val.AutowrapMode = (AutowrapMode)3;
				((Control)val).CustomMinimumSize = new Vector2(width, 0f);
				((Control)val).SizeFlagsHorizontal = (SizeFlags)3;
			}
			return val;
		}

		private static void BuildStop()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected Obj, but got Unknown
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Expected Obj, but got Unknown
			//IL_0081: Expected Obj, but got Unknown
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Expected Obj, but got Unknown
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Expected Obj, but got Unknown
			_stop = new Control
			{
				Name = StringName.op_Implicit("Stop"),
				MouseFilter = (MouseFilterEnum)2,
				Modulate = new Color(1f, 1f, 1f, 0f)
			};
			_stop.SetAnchorsPreset((LayoutPreset)15, false);
			((Node)_layer).AddChild((Node)(object)_stop, false, (InternalMode)0);
			_fogMaterial = new ShaderMaterial
			{
				Shader = new Shader
				{
					Code = "shader_type canvas_item;\nuniform float strength = 0.46;\nuniform vec2 feather = vec2(0.16, 0.30);\nvoid fragment() {\n    vec2 d = min(UV, 1.0 - UV);\n    float a = smoothstep(0.0, feather.x, d.x) * smoothstep(0.0, feather.y, d.y);\n    COLOR = vec4(0.024, 0.035, 0.047, a * strength * COLOR.a);\n}"
				}
			};
			_fog = new ColorRect
			{
				Name = StringName.op_Implicit("Mist"),
				Color = Colors.White,
				Material = (Material)(object)_fogMaterial,
				MouseFilter = (MouseFilterEnum)2
			};
			((Node)_stop).AddChild((Node)(object)_fog, false, (InternalMode)0);
			_box = new VBoxContainer
			{
				Name = StringName.op_Implicit("Words"),
				MouseFilter = (MouseFilterEnum)2
			};
			((Control)_box).AddThemeConstantOverride(StringName.op_Implicit("separation"), 0);
			((Node)_stop).AddChild((Node)(object)_box, false, (InternalMode)0);
		}

		private static Control Indented(Control inner, float top = 0f, float bottom = 0f)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Expected Obj, but got Unknown
			MarginContainer val = new MarginContainer
			{
				MouseFilter = (MouseFilterEnum)2
			};
			((Control)val).AddThemeConstantOverride(StringName.op_Implicit("margin_left"), 118);
			((Control)val).AddThemeConstantOverride(StringName.op_Implicit("margin_top"), (int)top);
			((Control)val).AddThemeConstantOverride(StringName.op_Implicit("margin_bottom"), (int)bottom);
			((Node)val).AddChild((Node)(object)inner, false, (InternalMode)0);
			return (Control)val;
		}

		private static Control Said(string who, Color tint, string text, int size, float width, float below)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Expected Obj, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Expected Obj, but got Unknown
			HBoxContainer val = new HBoxContainer
			{
				MouseFilter = (MouseFilterEnum)2
			};
			((Control)val).AddThemeConstantOverride(StringName.op_Implicit("separation"), 22);
			Label val2 = Text(who, Bold, Mathf.Max(12, size - 3), tint);
			((Control)val2).CustomMinimumSize = new Vector2(96f, 0f);
			val2.HorizontalAlignment = (HorizontalAlignment)2;
			((Control)val2).SizeFlagsVertical = (SizeFlags)0;
			((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)Text(text, Medium, size, TextTint, width, Mathf.Round((float)size * 0.42f)), false, (InternalMode)0);
			MarginContainer val3 = new MarginContainer
			{
				MouseFilter = (MouseFilterEnum)2
			};
			((Control)val3).AddThemeConstantOverride(StringName.op_Implicit("margin_bottom"), (int)below);
			((Node)val3).AddChild((Node)(object)val, false, (InternalMode)0);
			return (Control)val3;
		}

		private static void FillStop(TourStop stop, int index, int count)
		{
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Expected Obj, but got Unknown
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
			foreach (Node child in ((Node)_box).GetChildren(false))
			{
				((Node)_box).RemoveChild(child);
				child.QueueFree();
			}
			Rows.Clear();
			int num = Mathf.Clamp(Tuning.I("tour.font", 28), 16, 48);
			float width = Mathf.Clamp(Tuning.F("tour.width", 1060f), 480f, 1600f);
			float at = 0f;
			int value = ((stop.Number > 0) ? stop.Number : (index + 1));
			Row(Indented((Control)(object)Text(((count > 0) ? $"{value:00} / {count}" : $"{value:00}") + (string.IsNullOrWhiteSpace(stop.Group) ? "" : ("\u3000" + stop.Group.Trim())), Medium, Mathf.Max(12, num - 5), MetaTint)), 0.06f);
			Row(Indented((Control)(object)Text(stop.Name, Bold, num * 2, Colors.White), 0f, 2f), 0.16f);
			_rule = new ColorRect
			{
				Color = new Color(OpusTint, 0.85f),
				CustomMinimumSize = new Vector2(0f, 2f),
				SizeFlagsHorizontal = (SizeFlags)0,
				MouseFilter = (MouseFilterEnum)2
			};
			Control val = Indented((Control)(object)_rule, 0f, 16f);
			((Node)_box).AddChild((Node)(object)val, false, (InternalMode)0);
			at = 0.42f;
			foreach (TourLine line in stop.Lines)
			{
				if (!string.IsNullOrWhiteSpace(line.Text))
				{
					Row(Said(line.Who, TintOf(line.Who), line.Text.Trim(), num, width, 14f), 0.5f);
				}
			}
			if (!string.IsNullOrWhiteSpace(stop.Cue) && Tuning.F("tour.cue", 1f) >= 0.5f)
			{
				Row(Indented((Control)(object)Text(stop.Cue.Trim(), Medium, Mathf.Max(12, num - 7), CueTint, width), 4f), 0.3f);
			}
			void Row(Control row, float gap)
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				((CanvasItem)row).Modulate = new Color(1f, 1f, 1f, 0f);
				((Node)_box).AddChild((Node)(object)row, false, (InternalMode)0);
				Rows.Add(((CanvasItem)(object)row, at));
				at += gap;
			}
		}

		private static Color TintOf(string who)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (!(who == "奶酪"))
			{
				return OpusTint;
			}
			return CheeseTint;
		}

		private static void BuildPage()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected Obj, but got Unknown
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected Obj, but got Unknown
			_page = new Control
			{
				Name = StringName.op_Implicit("Page"),
				MouseFilter = (MouseFilterEnum)2,
				Modulate = new Color(1f, 1f, 1f, 0f)
			};
			_page.SetAnchorsPreset((LayoutPreset)15, false);
			((Node)_layer).AddChild((Node)(object)_page, false, (InternalMode)0);
			_pageDim = new ColorRect
			{
				Color = new Color(0.016f, 0.024f, 0.031f, 0.8f),
				MouseFilter = (MouseFilterEnum)2
			};
			((Control)_pageDim).SetAnchorsPreset((LayoutPreset)15, false);
			((Node)_page).AddChild((Node)(object)_pageDim, false, (InternalMode)0);
		}

		private static void ShowPage(string heading, IReadOnlyList<string> paragraphs, bool all, bool finale = false)
		{
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Expected Obj, but got Unknown
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Expected Obj, but got Unknown
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_036a: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_027a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!FxKit.Alive((GodotObject)(object)_layer))
				{
					Open();
				}
				if (!FxKit.Alive((GodotObject)(object)_page))
				{
					return;
				}
				foreach (Node child in ((Node)_page).GetChildren(false))
				{
					if ((object)child != _pageDim)
					{
						((Node)_page).RemoveChild(child);
						child.QueueFree();
					}
				}
				Paragraphs.Clear();
				float num = Mathf.Clamp(Tuning.F("tour.page_width", 1120f), 480f, 1700f);
				int num2 = Mathf.Clamp(Tuning.I("tour.page_font", 32), 16, 56);
				if (finale)
				{
					num2 = Mathf.Clamp(num2 + Tuning.I("tour.finale_grow", 14), 16, 84);
				}
				if (FxKit.Alive((GodotObject)(object)_pageDim))
				{
					_pageDim.Color = new Color(0.016f, 0.024f, 0.031f, finale ? Mathf.Clamp(Tuning.F("tour.finale_dim", 0.9f), 0f, 1f) : 0.8f);
				}
				CenterContainer val = new CenterContainer
				{
					MouseFilter = (MouseFilterEnum)2
				};
				((Control)val).SetAnchorsPreset((LayoutPreset)15, false);
				((Node)_page).AddChild((Node)(object)val, false, (InternalMode)0);
				VBoxContainer val2 = new VBoxContainer
				{
					MouseFilter = (MouseFilterEnum)2
				};
				((Control)val2).AddThemeConstantOverride(StringName.op_Implicit("separation"), 30);
				((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
				_pageColumn = val2;
				_pageFade = (finale ? Mathf.Max(0.2f, Tuning.F("tour.finale_fade", 1.6f)) : 0.7f);
				_pageFits = 14;
				_pageShrunk = 0;
				if (!string.IsNullOrEmpty(heading))
				{
					Label val3 = Text(heading, Bold, num2 - 2, OpusTint);
					((Node)val2).AddChild((Node)(object)val3, false, (InternalMode)0);
					Paragraphs.Add(((CanvasItem)(object)val3, 0f));
				}
				int num3 = 0;
				foreach (string paragraph in paragraphs)
				{
					string text = (paragraph ?? "").Trim();
					if (text.Length != 0)
					{
						bool flag = text.StartsWith("附", StringComparison.Ordinal);
						float width = ((finale && (float)(text.Length * num2) < num) ? 0f : num);
						Label val4 = (flag ? Text(text, Medium, num2 - 8, MetaTint, num, Mathf.Round((float)num2 * 0.3f)) : Text(text, Medium, num2, TextTint, width, Mathf.Round((float)num2 * 0.62f)));
						if (finale)
						{
							val4.HorizontalAlignment = (HorizontalAlignment)1;
						}
						((CanvasItem)val4).Modulate = new Color(1f, 1f, 1f, 0f);
						((Node)val2).AddChild((Node)(object)val4, false, (InternalMode)0);
						List<(CanvasItem Item, float Shown)> paragraphs2 = Paragraphs;
						Label item = val4;
						float item2;
						if (!all && num3 != 0)
						{
							item2 = -1f;
						}
						else
						{
							item2 = (finale ? 0.9f : 0.35f);
						}
						paragraphs2.Add(((CanvasItem)(object)item, item2));
						num3++;
					}
				}
				_pageHint = Text("→", Medium, 22, new Color(MetaTint, 0.55f));
				((Control)_pageHint).SetAnchorsPreset((LayoutPreset)7, false);
				Label pageHint = _pageHint;
				((Control)pageHint).Position = ((Control)pageHint).Position + new Vector2(-8f, -64f);
				((CanvasItem)_pageHint).Visible = !finale && Tuning.F("tour.hints", 1f) >= 0.5f;
				((Node)_page).AddChild((Node)(object)_pageHint, false, (InternalMode)0);
				_pageShown = true;
				_pageClock = 0f;
				_stopShown = false;
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourUi.ShowPage: " + ex.Message);
			}
		}

		private static void ShowScreen(TourScreen screen)
		{
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Expected Obj, but got Unknown
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Expected Obj, but got Unknown
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!FxKit.Alive((GodotObject)(object)_layer))
				{
					Open();
				}
				if (!FxKit.Alive((GodotObject)(object)_page))
				{
					return;
				}
				foreach (Node child in ((Node)_page).GetChildren(false))
				{
					if ((object)child != _pageDim)
					{
						((Node)_page).RemoveChild(child);
						child.QueueFree();
					}
				}
				Paragraphs.Clear();
				float width = Mathf.Clamp(Tuning.F("tour.page_width", 1120f), 480f, 1700f);
				int num = Mathf.Clamp(Tuning.I("tour.page_font", 32), 16, 56);
				if (FxKit.Alive((GodotObject)(object)_pageDim))
				{
					_pageDim.Color = new Color(0.016f, 0.024f, 0.031f, 0.8f);
				}
				CenterContainer val = new CenterContainer
				{
					MouseFilter = (MouseFilterEnum)2
				};
				((Control)val).SetAnchorsPreset((LayoutPreset)15, false);
				((Node)_page).AddChild((Node)(object)val, false, (InternalMode)0);
				VBoxContainer val2 = new VBoxContainer
				{
					MouseFilter = (MouseFilterEnum)2
				};
				((Control)val2).AddThemeConstantOverride(StringName.op_Implicit("separation"), 30);
				((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
				_pageColumn = val2;
				_pageFade = 0.7f;
				_pageFits = 14;
				_pageShrunk = 0;
				if (!string.IsNullOrEmpty(screen?.Cue))
				{
					Label val3 = Text(screen.Cue.Trim(), Medium, Mathf.Max(16, num - 10), new Color(MetaTint, 0.7f));
					val3.HorizontalAlignment = (HorizontalAlignment)1;
					((CanvasItem)val3).Modulate = new Color(1f, 1f, 1f, 0f);
					((Node)val2).AddChild((Node)(object)val3, false, (InternalMode)0);
					Paragraphs.Add(((CanvasItem)(object)val3, 0.2f));
				}
				else if (screen != null)
				{
					foreach (TourLine line in screen.Lines)
					{
						if (!string.IsNullOrWhiteSpace(line.Text))
						{
							Control val4 = Said(line.Who, TintOf(line.Who), line.Text.Trim(), num, width, 0f);
							((CanvasItem)val4).Modulate = new Color(1f, 1f, 1f, 0f);
							((Node)val2).AddChild((Node)(object)val4, false, (InternalMode)0);
							Paragraphs.Add(((CanvasItem)(object)val4, (Paragraphs.Count == 0) ? 0.35f : (-1f)));
						}
					}
				}
				_pageHint = Text("→", Medium, 22, new Color(MetaTint, 0.55f));
				((Control)_pageHint).SetAnchorsPreset((LayoutPreset)7, false);
				Label pageHint = _pageHint;
				((Control)pageHint).Position = ((Control)pageHint).Position + new Vector2(-8f, -64f);
				((CanvasItem)_pageHint).Visible = Tuning.F("tour.hints", 1f) >= 0.5f;
				((Node)_page).AddChild((Node)(object)_pageHint, false, (InternalMode)0);
				_pageShown = true;
				_pageClock = 0f;
				_stopShown = false;
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourUi.ShowScreen: " + ex.Message);
			}
		}

		private static void FitPage()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			if (_pageFits <= 0 || !FxKit.Alive((GodotObject)(object)_pageColumn))
			{
				return;
			}
			float num = FxKit.ViewSize().Y - Mathf.Max(60f, Tuning.F("tour.page_margin", 140f));
			if (((Control)_pageColumn).Size.Y <= num)
			{
				if (_pageClock > 0.5f)
				{
					_pageFits = 0;
				}
				return;
			}
			_pageFits--;
			_pageShrunk++;
			Smaller((Node)(object)_pageColumn);
			int num2 = Mathf.Max(14, 30 - _pageShrunk * 2);
			((Control)_pageColumn).AddThemeConstantOverride(StringName.op_Implicit("separation"), num2);
			((Control)_pageColumn).ResetSize();
			if (_pageFits == 0 || _pageShrunk == 1)
			{
				DevLog.Info($"TourUi: the page is taller than the screen ({((Control)_pageColumn).Size.Y:0} > {num:0}); set smaller, step {_pageShrunk}");
			}
			static void Smaller(Node node)
			{
				foreach (Node child in node.GetChildren(false))
				{
					Label val = (Label)(object)((child is Label) ? child : null);
					if (val != null && val.LabelSettings != null)
					{
						val.LabelSettings.FontSize = Mathf.Max(18, val.LabelSettings.FontSize - 1);
						val.LabelSettings.LineSpacing = Mathf.Max(4f, val.LabelSettings.LineSpacing - 1f);
					}
					else
					{
						Smaller(child);
					}
				}
			}
		}

		private static void BuildToast()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			_toast = Text("", Medium, 20, new Color(1f, 0.86f, 0.62f, 1f));
			((CanvasItem)_toast).Modulate = new Color(1f, 1f, 1f, 0f);
			((Control)_toast).Position = new Vector2(36f, FxKit.ViewSize().Y - 300f);
			((Node)_layer).AddChild((Node)(object)_toast, false, (InternalMode)0);
		}

		private static float Ease(float t)
		{
			t = Mathf.Clamp(t, 0f, 1f);
			return t * t * (3f - 2f * t);
		}

		private static void Tick(double delta)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_025a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (!FxKit.Alive((GodotObject)(object)_layer))
				{
					Close();
					return;
				}
				float num = Mathf.Min((float)delta, 0.1f);
				Vector2 val = FxKit.ViewSize();
				bool flag = HeldFxManager.Carried != null;
				_sinceHeld = (flag ? 0f : (_sinceHeld + num));
				float num2 = Mathf.Max(0f, Tuning.F("tour.back", 1.2f));
				float num3;
				if (!_stopShown || _busy || !_captions || _pageShown)
				{
					num3 = 0f;
				}
				else
				{
					num3 = ((flag || _sinceHeld < num2) ? Mathf.Clamp(Tuning.F("tour.held_alpha", 0.1f), 0f, 1f) : 1f);
				}
				_stopAlpha = FxKit.Damp(_stopAlpha, num3, (num3 < _stopAlpha) ? 16f : 4.5f, num);
				if (FxKit.Alive((GodotObject)(object)_stop))
				{
					((CanvasItem)_stop).Modulate = new Color(1f, 1f, 1f, _stopAlpha);
				}
				if (_stopShown && FxKit.Alive((GodotObject)(object)_box))
				{
					_stopClock += num;
					foreach (var (val2, num4) in Rows)
					{
						if (FxKit.Alive((GodotObject)(object)val2))
						{
							val2.Modulate = new Color(1f, 1f, 1f, Ease((_stopClock - num4) / 0.38f));
						}
					}
					if (FxKit.Alive((GodotObject)(object)_rule))
					{
						((Control)_rule).CustomMinimumSize = new Vector2(Mathf.Round(210f * Ease((_stopClock - 0.14f) / 0.46f)), 2f);
					}
					((Control)_box).ResetSize();
					Vector2 size = ((Control)_box).Size;
					float num5 = Tuning.F("tour.top", 112f);
					((Control)_box).Position = new Vector2(Mathf.Round((val.X - size.X) * 0.5f - 59f), num5);
					if (FxKit.Alive((GodotObject)(object)_fog))
					{
						Vector2 val3 = new Vector2(190f, 84f);
						((Control)_fog).Position = ((Control)_box).Position - val3;
						((Control)_fog).Size = size + val3 * 2f;
						ShaderMaterial fogMaterial = _fogMaterial;
						if (fogMaterial != null)
						{
							fogMaterial.SetShaderParameter(StringName.op_Implicit("strength"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("tour.fog", 0.46f), 0f, 1f)));
						}
					}
				}
				_pageAlpha = FxKit.Damp(_pageAlpha, _pageShown ? 1f : 0f, _pageShown ? 3.5f : 9f, num);
				if (FxKit.Alive((GodotObject)(object)_page))
				{
					((CanvasItem)_page).Modulate = new Color(1f, 1f, 1f, _pageAlpha);
				}
				if (_pageShown)
				{
					_pageClock += num;
					FitPage();
					bool flag2 = false;
					for (int i = 0; i < Paragraphs.Count; i++)
					{
						var (val4, num6) = Paragraphs[i];
						if (FxKit.Alive((GodotObject)(object)val4))
						{
							if (num6 < 0f)
							{
								flag2 = true;
								val4.Modulate = new Color(1f, 1f, 1f, 0f);
							}
							else
							{
								val4.Modulate = new Color(1f, 1f, 1f, Ease((_pageClock - num6) / _pageFade));
							}
						}
					}
					if (!flag2 && _screens != null && _screen + 1 < _screens.Count)
					{
						flag2 = true;
					}
					if (FxKit.Alive((GodotObject)(object)_pageHint))
					{
						((CanvasItem)_pageHint).Modulate = new Color(1f, 1f, 1f, (flag2 ? 1f : 0.6f) * (0.55f + 0.45f * Mathf.Sin(_pageClock * 2.2f)));
					}
				}
				if (FxKit.Alive((GodotObject)(object)_toast))
				{
					_toastLeft = Mathf.Max(0f, _toastLeft - num);
					((CanvasItem)_toast).Modulate = new Color(1f, 1f, 1f, Mathf.Clamp(_toastLeft / 0.6f, 0f, 1f));
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("TourUi.Tick: " + ex.Message);
			}
		}

		static TourUi()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		}
	}
}
namespace SilentFX.Core
{
