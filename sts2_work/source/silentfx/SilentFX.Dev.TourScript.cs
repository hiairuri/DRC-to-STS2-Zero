// namespace: SilentFX.Dev
// type: TourScript (class)
// source lines: 28754-29326
	internal static class TourScript
	{
		private sealed class Section
		{
			internal int Number;

			internal string Name;

			internal readonly Dictionary<string, string> Values = new Dictionary<string, string>(StringComparer.Ordinal);
		}

		private sealed class Words
		{
			internal string Name;

			internal readonly List<TourLine> Lines = new List<TourLine>();
		}

		private static readonly Regex Header = new Regex("^\\[(\\d{1,2})\\s+(.+)\\]$", RegexOptions.CultureInvariant);

		private static readonly Regex Copies = new Regex("^(.+?)\\s*[×x*]\\s*(\\d+)\\s*$", RegexOptions.CultureInvariant);

		private static readonly UTF8Encoding Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

		private static readonly Regex AnySection = new Regex("^\\[(.+)\\]$", RegexOptions.CultureInvariant);

		internal static bool TryReload(TourDocument current, out TourDocument result, out string error)
		{
			result = current;
			error = null;
			try
			{
				string text = Path.Combine(DevLog.ModDir, "展示_脚本.txt");
				string text2 = Path.Combine(DevLog.ModDir, "展示_每张牌一句话.txt");
				(DateTime, long) tuple = Stamp(text);
				(DateTime, long) tuple2 = Stamp(text2);
				(DateTime, long) tuple3;
				if (current != null)
				{
					(DateTime, long) scriptStamp = current.ScriptStamp;
					tuple3 = tuple;
					if (scriptStamp.Item1 == tuple3.Item1 && scriptStamp.Item2 == tuple3.Item2)
					{
						scriptStamp = current.WordsStamp;
						tuple3 = tuple2;
						if (scriptStamp.Item1 == tuple3.Item1 && scriptStamp.Item2 == tuple3.Item2)
						{
							return true;
						}
					}
				}
				string script = File.ReadAllText(text, Utf8);
				string words = File.ReadAllText(text2, Utf8);
				tuple3 = Stamp(text);
				(DateTime, long) tuple4 = tuple;
				if (!(tuple3.Item1 != tuple4.Item1) && tuple3.Item2 == tuple4.Item2)
				{
					tuple4 = Stamp(text2);
					tuple3 = tuple2;
					if (!(tuple4.Item1 != tuple3.Item1) && tuple4.Item2 == tuple3.Item2)
					{
						TourDocument tourDocument = Parse(script, words);
						tourDocument.ScriptStamp = tuple;
						tourDocument.WordsStamp = tuple2;
						result = tourDocument;
						DevLog.Info($"Tour: scripts loaded; {tourDocument.Stops.Count} stops; pages: {tourDocument.Intro.Count} screen(s) before, {tourDocument.Outro.Count} after");
						return true;
					}
				}
				throw new FormatException("文件正在保存，请保存完成后再切站");
			}
			catch (Exception ex)
			{
				error = "展示脚本：" + ex.Message;
				DevLog.Warn("Tour: reload rejected: " + ex);
				return false;
			}
		}

		private static (DateTime, long) Stamp(string file)
		{
			FileInfo fileInfo = new FileInfo(file);
			if (!fileInfo.Exists)
			{
				throw new FileNotFoundException("找不到 " + fileInfo.Name);
			}
			return (fileInfo.LastWriteTimeUtc, fileInfo.Length);
		}

		private static TourDocument Parse(string script, string words)
		{
			TourDocument tourDocument = new TourDocument();
			Section section = new Section
			{
				Name = "默认"
			};
			Dictionary<int, Section> scenes = ReadSections(script, section, out var finale);
			if (finale != null)
			{
				throw new FormatException("第二期没有 [终幕]：后日谈直接写在一句话文件的 [结尾] 里");
			}
			Dictionary<int, Words> captions = ReadWords(words, tourDocument);
			CheckKeys(section, "敌人血量", "残血", "能量");
			int num = Number(section, "敌人血量", 200, 1, 100000);
			int num2 = Number(section, "残血", 1, 1, num);
			int fallback = Number(section, "能量", 9, 0, 999);
			int count = scenes.Count;
			if (count < 1 || Enumerable.Range(1, count).Any((int n) => !scenes.ContainsKey(n)))
			{
				throw new FormatException("脚本的编号要从 01 连着排下去，不能跳号");
			}
			if (captions.Count != count || scenes.Keys.Any((int n) => !captions.ContainsKey(n)))
			{
				throw new FormatException($"一句话文件的编号要和脚本一一对应（脚本 {count} 站，一句话 {captions.Count} 站）");
			}
			foreach (Section value3 in scenes.Values)
			{
				if (!string.Equals(value3.Name, captions[value3.Number].Name, StringComparison.Ordinal))
				{
					throw new FormatException($"编号 {value3.Number:00}：脚本里是“{value3.Name}”，一句话里是“{captions[value3.Number].Name}”");
				}
			}
			CardModel[] all = (from c in ModelDb.AllCards
				group c by ((object)c).GetType() into g
				select g.First()).ToArray();
			List<TourCard> list = new List<TourCard>();
			list.AddRange(Enumerable.Repeat(new TourCard((CardModel)(object)ModelDb.Card<StrikeSilent>()), 5));
			list.AddRange(Enumerable.Repeat(new TourCard((CardModel)(object)ModelDb.Card<DefendSilent>()), 5));
			list.Add(new TourCard((CardModel)(object)ModelDb.Card<Neutralize>()));
			list.Add(new TourCard((CardModel)(object)ModelDb.Card<Survivor>()));
			foreach (Section item in scenes.Values.OrderBy((Section s) => s.Number))
			{
				CheckKeys(item, "分组", "提示", "手牌", "抽牌堆", "消耗堆", "下回合抽到", "能量", "遗物", "敌人血量", "敌人", "敌人1", "敌人2", "敌人3", "敌人回合", "敌人1回合", "敌人2回合", "敌人3回合", "慢动作");
				if (!item.Values.ContainsKey("手牌"))
				{
					throw Bad(item, "缺少手牌");
				}
				TourDefinition tourDefinition = new TourDefinition
				{
					EnemyHp = Number(item, "敌人血量", num, 1, 100000),
					LowHp = num2,
					Energy = Number(item, "能量", fallback, 0, 999),
					EnemyTurn = Value(item, "敌人回合"),
					Slow = SlowOf(item),
					Relic = RelicOf(item),
					Text = new TourStop
					{
						Number = item.Number,
						Name = item.Name,
						Group = Value(item, "分组"),
						Cue = Value(item, "提示")
					},
					Hand = Cards(Value(item, "手牌"), item, all),
					Draw = (item.Values.ContainsKey("抽牌堆") ? Cards(Value(item, "抽牌堆"), item, all) : new List<TourCard>(list)),
					Exhaust = Cards(Value(item, "消耗堆"), item, all),
					NextDraw = Cards(Value(item, "下回合抽到"), item, all)
				};
				tourDefinition.Text.Lines.AddRange(captions[item.Number].Lines);
				if (tourDefinition.Hand.Count > CardPile.MaxCardsInHand)
				{
					throw Bad(item, "手牌超过游戏上限");
				}
				if (tourDefinition.NextDraw.Count > 5)
				{
					throw Bad(item, "下回合抽到最多指定五张牌");
				}
				if (tourDefinition.EnemyTurn != "" && !IsEnemyTurn(tourDefinition.EnemyTurn))
				{
					throw Bad(item, "敌人回合只能写 攻击、连击 或 不攻击");
				}
				for (int num3 = 0; num3 <= 3; num3++)
				{
					string text = "敌人" + num3 + "回合";
					if (num3 > 0 && item.Values.TryGetValue(text, out var value))
					{
						if (!IsEnemyTurn(value))
						{
							throw Bad(item, text + "只能写 攻击、连击 或 不攻击");
						}
						tourDefinition.EnemyTurns[num3 - 1] = value;
					}
					foreach (string item2 in Split(Value(item, (num3 == 0) ? "敌人" : ("敌人" + num3))))
					{
						if (item2 == "残血")
						{
							if (num2 > tourDefinition.EnemyHp)
							{
								throw Bad(item, "默认残血不能超过本节敌人血量");
							}
							tourDefinition.EnemyRules.Add(new TourEnemyRule
							{
								Index = num3,
								Kind = "血量",
								Amount = num2
							});
							continue;
						}
						Match match = Regex.Match(item2, "^(中毒|虚弱|易伤|格挡|血量|人工制品)\\s+(\\d+)$");
						if (!match.Success)
						{
							throw Bad(item, "无法识别敌人设置：" + item2);
						}
						string value2 = match.Groups[1].Value;
						int amount = ParseNumber(match.Groups[2].Value, 1, (value2 == "血量") ? tourDefinition.EnemyHp : 9999, item);
						tourDefinition.EnemyRules.Add(new TourEnemyRule
						{
							Index = num3,
							Kind = value2,
							Amount = amount
						});
					}
				}
				tourDocument.Stops.Add(tourDefinition);
			}
			return tourDocument;
		}

		private static Dictionary<int, Section> ReadSections(string text, Section defaults, out Section finale)
		{
			finale = null;
			Dictionary<int, Section> dictionary = new Dictionary<int, Section>();
			Section section = null;
			bool flag = false;
			int num = 0;
			string[] array = text.TrimStart('\ufeff').Split('\n');
			foreach (string obj in array)
			{
				num++;
				string text2 = obj.Trim();
				if (text2.Length == 0 || text2.StartsWith('#'))
				{
					continue;
				}
				text2 = text2.Split('#')[0].Trim();
				if (text2 == "[终幕]")
				{
					if (finale != null)
					{
						throw new FormatException("重复的 [终幕]");
					}
					section = (finale = new Section
					{
						Name = "终幕"
					});
					continue;
				}
				if (text2 == "[默认]")
				{
					if (flag)
					{
						throw new FormatException("重复的 [默认]");
					}
					flag = true;
					section = defaults;
					continue;
				}
				Match match = Header.Match(text2);
				if (match.Success)
				{
					int num2 = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
					if (num2 < 1 || num2 > 99 || dictionary.ContainsKey(num2))
					{
						throw new FormatException($"第 {num} 行：站号无效或重复：{num2}");
					}
					section = new Section
					{
						Number = num2,
						Name = match.Groups[2].Value.Trim()
					};
					dictionary.Add(num2, section);
					continue;
				}
				int num3 = text2.IndexOf('=');
				if (num3 <= 0)
				{
					throw new FormatException($"第 {num} 行：需要 [编号 牌名] 或 键 = 值");
				}
				string text3 = text2.Substring(0, num3).Trim();
				string text4 = text2;
				int num4 = num3 + 1;
				string value = text4.Substring(num4, text4.Length - num4).Trim();
				if (section == null)
				{
					throw new FormatException($"第 {num} 行：键不在任何一站内：{text3}");
				}
				if (!section.Values.TryAdd(text3, value))
				{
					throw Bad(section, "重复的键：" + text3);
				}
			}
			return dictionary;
		}

		private static Dictionary<int, Words> ReadWords(string text, TourDocument result)
		{
			Dictionary<int, Words> stops = new Dictionary<int, Words>();
			Words words = null;
			TourScreen screen = null;
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			string[] array = text.TrimStart('\ufeff').Split('\n');
			foreach (string obj in array)
			{
				num++;
				string text2 = obj.Trim();
				if (text2.Length == 0 || text2.StartsWith('#'))
				{
					continue;
				}
				Match match = Header.Match(text2);
				if (match.Success)
				{
					int num2 = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
					if (num2 < 1 || num2 > 99 || stops.ContainsKey(num2))
					{
						throw new FormatException($"一句话第 {num} 行：站号无效或重复：{num2}");
					}
					if (flag2)
					{
						throw new FormatException($"一句话第 {num} 行：站与站之间不能插页（[{num2:00}] 前面那页要放到所有站之后）");
					}
					words = new Words
					{
						Name = match.Groups[2].Value.Trim()
					};
					stops.Add(num2, words);
					flag = false;
					screen = null;
					continue;
				}
				if (AnySection.Match(text2).Success)
				{
					if (stops.Count > 0)
					{
						flag2 = true;
					}
					words = null;
					flag = true;
					screen = null;
					continue;
				}
				if (text2.StartsWith("【视频", StringComparison.Ordinal))
				{
					if (!flag)
					{
						throw new FormatException($"一句话第 {num} 行：视频插入点只能写在页里（[开头] 这样的段落），不能写在站里");
					}
					Pages().Add(new TourScreen
					{
						Cue = text2
					});
					screen = null;
					continue;
				}
				int num3 = text2.IndexOf('=');
				if (num3 <= 0)
				{
					throw new FormatException($"一句话第 {num} 行：需要 [段落] 或 标签 = 文字");
				}
				string text3 = text2.Substring(0, num3).Trim();
				string text4 = text2;
				int num4 = num3 + 1;
				string text5 = text4.Substring(num4, text4.Length - num4).Trim();
				string text6;
				if (text3 == "奶酪")
				{
					text6 = "奶酪";
				}
				else
				{
					text6 = (text3.StartsWith("opus", StringComparison.Ordinal) ? "opus" : null);
				}
				if (text6 == null)
				{
					throw new FormatException($"一句话第 {num} 行：不认识的标签“{text3}”（只能是 奶酪 或 opus 开头的）");
				}
				if (text5.Length == 0)
				{
					throw new FormatException($"一句话第 {num} 行：{text3} 后面是空的");
				}
				TourLine item = new TourLine
				{
					Who = text6,
					Text = text5
				};
				if (words != null)
				{
					words.Lines.Add(item);
					continue;
				}
				if (flag)
				{
					Screen().Lines.Add(item);
					continue;
				}
				throw new FormatException($"一句话第 {num} 行：这一行不在任何段落里");
			}
			result.Intro.RemoveAll((TourScreen s) => s.Cue == null && s.Lines.Count == 0);
			result.Outro.RemoveAll((TourScreen s) => s.Cue == null && s.Lines.Count == 0);
			return stops;
			List<TourScreen> Pages()
			{
				if (stops.Count != 0)
				{
					return result.Outro;
				}
				return result.Intro;
			}
			TourScreen Screen()
			{
				if (screen != null)
				{
					return screen;
				}
				screen = new TourScreen();
				Pages().Add(screen);
				return screen;
			}
		}

		private static string RelicOf(Section section)
		{
			string text = Value(section, "遗物");
			switch (text)
			{
			case "":
				return "";
			default:
				if (!string.Equals(text, "ChemicalX", StringComparison.OrdinalIgnoreCase))
				{
					throw Bad(section, "遗物“" + text + "”不认识：现在只能写 化合物X");
				}
				goto case "化合物X";
			case "化合物X":
			case "化合物 X":
				return "ChemicalX";
			}
		}

		private static List<TourCard> Cards(string value, Section section, CardModel[] all)
		{
			List<TourCard> list = new List<TourCard>();
			foreach (string item in Split(value))
			{
				Match match = Copies.Match(item);
				string text = (match.Success ? match.Groups[1].Value.Trim() : item);
				int count = ((!match.Success) ? 1 : ParseNumber(match.Groups[2].Value, 1, 200, section));
				CardModel[] array = FindCards(text, all);
				bool flag = false;
				if (array.Length == 0 && text.EndsWith('+'))
				{
					string text2 = text;
					array = FindCards(text2.Substring(0, text2.Length - 1), all);
					flag = true;
				}
				if (array.Length != 1)
				{
					throw Bad(section, $"牌名“{text}”{((array.Length == 0) ? "不存在" : "有歧义")}；可填写英文类名");
				}
				if (flag && !array[0].IsUpgradable)
				{
					throw Bad(section, "牌名“" + text + "”不能升级");
				}
				list.AddRange(Enumerable.Repeat(new TourCard(array[0], flag), count));
				if (list.Count > 200)
				{
					throw Bad(section, "单个牌堆最多配置 200 张");
				}
			}
			return list;
		}

		private static CardModel[] FindCards(string name, CardModel[] all)
		{
			if (name == ((CardModel)ModelDb.Card<StrikeSilent>()).Title)
			{
				return new CardModel[1] { (CardModel)ModelDb.Card<StrikeSilent>() };
			}
			if (name == ((CardModel)ModelDb.Card<DefendSilent>()).Title)
			{
				return new CardModel[1] { (CardModel)ModelDb.Card<DefendSilent>() };
			}
			CardModel[] array = all.Where((CardModel c) => string.Equals(((object)c).GetType().Name, name, StringComparison.OrdinalIgnoreCase)).ToArray();
			if (array.Length != 0)
			{
				return array;
			}
			return all.Where((CardModel c) => c.Title == name).ToArray();
		}

		private static bool IsEnemyTurn(string value)
		{
			switch (value)
			{
			case "攻击":
			case "连击":
			case "不攻击":
				return true;
			default:
				return false;
			}
		}

		private static IEnumerable<string> Split(string value)
		{
			return value.Split(new char[2] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		}

		private static string Value(Section section, string key)
		{
			if (!section.Values.TryGetValue(key, out var value))
			{
				return "";
			}
			return value;
		}

		private static FormatException Bad(Section section, string message)
		{
			return new FormatException(((section.Number == 0) ? ("[" + section.Name + "]") : $"[{section.Number:00} {section.Name}]") + " " + message);
		}

		private static int Number(Section section, string key, int fallback, int min, int max)
		{
			if (!section.Values.ContainsKey(key))
			{
				return fallback;
			}
			return ParseNumber(Value(section, key), min, max, section);
		}

		private static float SlowOf(Section section)
		{
			string text = Value(section, "慢动作");
			switch (text)
			{
			case "":
			case "关":
				return 0f;
			case "开":
				return -1f;
			default:
			{
				if (!float.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result) || !(result >= 0.05f) || !(result <= 1f))
				{
					throw Bad(section, "慢动作“" + text + "”应写 开、关，或 0.05 至 1 之间的速度（0.2 = 五分之一速）");
				}
				return result;
			}
			}
		}

		private static int ParseNumber(string text, int min, int max, Section section)
		{
			if (!int.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var result) || result < min || result > max)
			{
				throw Bad(section, $"数值“{text}”应在 {min} 至 {max} 之间");
			}
			return result;
		}

		private static void CheckKeys(Section section, params string[] allowed)
		{
			foreach (string key in section.Values.Keys)
			{
				if (!Enumerable.Contains(allowed, key))
				{
					throw Bad(section, "未知的键：" + key);
				}
			}
		}
	}
