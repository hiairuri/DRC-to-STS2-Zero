// namespace: SilentFX.Core
// type: GameSounds (class)
// source lines: 37572-38074
	public static class GameSounds
	{
		private sealed class Sample
		{
			public string Name;

			public int Rate;

			public int Channels;

			public long Offset;

			public long Length;

			public long Frames;

			public uint Crc;
		}

		private sealed class Bank
		{
			public string Path;

			public long Data;

			public readonly Dictionary<string, Sample> Samples = new Dictionary<string, Sample>(StringComparer.OrdinalIgnoreCase);

			public bool Bad;
		}

		private static readonly Dictionary<string, Bank> Banks = new Dictionary<string, Bank>();

		private static readonly Dictionary<string, AudioStream> Streams = new Dictionary<string, AudioStream>(StringComparer.OrdinalIgnoreCase);

		private static readonly Dictionary<uint, byte[]> Setups = new Dictionary<uint, byte[]>();

		private static readonly HashSet<string> Told = new HashSet<string>();

		private static uint[] _oggTable;

		private static uint[] _zipTable;

		private static byte[] _fmodImage;

		private static bool _fmodRead;

		public static AudioStream Get(string bank, string sample)
		{
			string text = bank + "/" + sample;
			if (Streams.TryGetValue(text, out var value))
			{
				return value;
			}
			AudioStream val = null;
			long timestamp = Stopwatch.GetTimestamp();
			try
			{
				val = Build(bank, sample);
			}
			catch (Exception ex)
			{
				Tell(text, "GameSounds: " + text + " - " + ex.Message);
			}
			Streams[text] = val;
			double totalMilliseconds = Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds;
			if (GameRefs.CombatRoom != null)
			{
				DevLog.Info($"Cold sound during combat: {text}, {totalMilliseconds:F1} ms; prewarm did not cover this stream");
			}
			if (totalMilliseconds > 8.0)
			{
				DevLog.Info($"GameSounds: {text} took {totalMilliseconds:F0} ms to cut");
			}
			return val;
		}

		public static float Length(string bank, string sample)
		{
			Bank bank2 = Open(bank);
			if (bank2 == null || !bank2.Samples.TryGetValue(sample, out var value) || value.Rate <= 0)
			{
				return 0f;
			}
			return (float)value.Frames / (float)value.Rate;
		}

		private static void Tell(string key, string text)
		{
			if (Told.Add(key))
			{
				DevLog.Warn(text);
			}
		}

		private static AudioStream Build(string bankName, string sampleName)
		{
			Bank bank = Open(bankName);
			if (bank == null)
			{
				return null;
			}
			if (!bank.Samples.TryGetValue(sampleName, out var value))
			{
				Tell(bankName + "/" + sampleName, $"GameSounds: the game's {bankName} bank has no sample \"{sampleName}\" (a game update may have renamed it)");
				return null;
			}
			byte[] array = Setup(value.Crc);
			if (array == null)
			{
				return null;
			}
			FileAccess val = FileAccess.Open(bank.Path, (ModeFlags)1);
			byte[] buffer;
			try
			{
				if (val == null)
				{
					return null;
				}
				val.Seek((ulong)(bank.Data + value.Offset));
				buffer = val.GetBuffer(value.Length);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			if (buffer == null || buffer.Length < 4)
			{
				return null;
			}
			AudioStreamOggVorbis val2 = AudioStreamOggVorbis.LoadFromBuffer(ToOgg(value, array, buffer));
			if (val2 == null)
			{
				Tell(bankName + "/" + sampleName, "GameSounds: Godot would not take the rebuilt stream of " + sampleName);
			}
			return (AudioStream)(object)val2;
		}

		private static Bank Open(string name)
		{
			if (Banks.TryGetValue(name, out var value))
			{
				if (!value.Bad)
				{
					return value;
				}
				return null;
			}
			Bank bank = new Bank
			{
				Path = "res://banks/desktop/" + name + ".bank"
			};
			Banks[name] = bank;
			try
			{
				FileAccess val = FileAccess.Open(bank.Path, (ModeFlags)1);
				try
				{
					if (val == null)
					{
						throw new IOException("cannot open " + bank.Path);
					}
					long length = (long)val.GetLength();
					long num = -1L;
					for (long num2 = 0L; num2 < length && num2 < 67108864; num2 += 1048573)
					{
						if (num >= 0)
						{
							break;
						}
						val.Seek((ulong)num2);
						byte[] buffer = val.GetBuffer(Math.Min(1048576L, length - num2));
						for (int i = 0; i + 4 <= buffer.Length; i++)
						{
							if (buffer[i] == 70 && buffer[i + 1] == 83 && buffer[i + 2] == 66 && buffer[i + 3] == 53)
							{
								num = num2 + i;
								break;
							}
						}
					}
					if (num < 0)
					{
						throw new IOException("no FSB5 block in " + bank.Path);
					}
					val.Seek((ulong)num);
					byte[] buffer2 = val.GetBuffer(64L);
					uint num3 = BitConverter.ToUInt32(buffer2, 4);
					int num4 = (int)BitConverter.ToUInt32(buffer2, 8);
					int num5 = (int)BitConverter.ToUInt32(buffer2, 12);
					int num6 = (int)BitConverter.ToUInt32(buffer2, 16);
					long num7 = BitConverter.ToUInt32(buffer2, 20);
					uint num8 = BitConverter.ToUInt32(buffer2, 24);
					if (num8 != 15)
					{
						throw new IOException("the samples are not Vorbis (codec " + num8 + ")");
					}
					int num9 = ((num3 == 1) ? 60 : 64);
					if (num4 <= 0 || num4 > 100000 || num5 <= 0 || num6 <= 0)
					{
						throw new IOException("unexpected FSB5 header");
					}
					val.Seek((ulong)(num + num9));
					byte[] buffer3 = val.GetBuffer((long)num5);
					byte[] buffer4 = val.GetBuffer((long)num6);
					bank.Data = num + num9 + num5 + num6;
					int[] array = new int[10] { 0, 8000, 11000, 11025, 16000, 22050, 24000, 32000, 44100, 48000 };
					List<Sample> list = new List<Sample>(num4);
					int num10 = 0;
					for (int j = 0; j < num4; j++)
					{
						ulong num11 = BitConverter.ToUInt64(buffer3, num10);
						num10 += 8;
						bool flag = (num11 & 1) != 0;
						int num12 = (int)((num11 >> 1) & 0xF);
						Sample sample = new Sample
						{
							Rate = ((num12 < array.Length && array[num12] > 0) ? array[num12] : 44100),
							Channels = (int)((num11 >> 5) & 1) + 1,
							Offset = (long)(((num11 >> 6) & 0xFFFFFFF) * 16),
							Frames = (long)((num11 >> 34) & 0x3FFFFFFF)
						};
						while (flag)
						{
							uint num13 = BitConverter.ToUInt32(buffer3, num10);
							num10 += 4;
							flag = (num13 & 1) != 0;
							int num14 = (int)((num13 >> 1) & 0xFFFFFF);
							int num15 = (int)((num13 >> 25) & 0x7F);
							if (num15 == 1 && num14 >= 1)
							{
								sample.Channels = buffer3[num10];
							}
							else if (num15 == 2 && num14 >= 4)
							{
								sample.Rate = (int)BitConverter.ToUInt32(buffer3, num10);
							}
							else if (num15 == 11 && num14 >= 4)
							{
								sample.Crc = BitConverter.ToUInt32(buffer3, num10);
							}
							num10 += num14;
						}
						list.Add(sample);
					}
					for (int k = 0; k < num4; k++)
					{
						int num16 = (int)BitConverter.ToUInt32(buffer4, 4 * k);
						int l;
						for (l = num16; l < buffer4.Length && buffer4[l] != 0; l++)
						{
						}
						list[k].Name = Encoding.UTF8.GetString(buffer4, num16, l - num16);
						list[k].Length = ((k + 1 < num4) ? list[k + 1].Offset : num7) - list[k].Offset;
						bank.Samples[list[k].Name] = list[k];
					}
					DevLog.Info($"GameSounds: {name}.bank - {num4} samples of the game's own");
					return bank;
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				bank.Bad = true;
				DevLog.Warn("GameSounds: " + name + ".bank could not be read - " + ex.Message);
				return null;
			}
		}

		private static byte[] Setup(uint crc)
		{
			if (Setups.TryGetValue(crc, out var value))
			{
				return value;
			}
			byte[] array = null;
			try
			{
				if (!_fmodRead)
				{
					_fmodRead = true;
					string text = Path.Combine(Path.GetDirectoryName(OS.GetExecutablePath()) ?? "", "fmod.dll");
					if (!File.Exists(text))
					{
						throw new IOException("no fmod.dll next to the game (" + text + ")");
					}
					_fmodImage = File.ReadAllBytes(text);
				}
				if (_fmodImage == null)
				{
					return null;
				}
				array = FromFmod(_fmodImage, crc);
				if (array == null)
				{
					throw new IOException($"fmod.dll has no Vorbis setup {crc:X8}");
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("GameSounds: the game's samples cannot be decoded here - " + ex.Message);
			}
			Setups[crc] = array;
			return array;
		}

		private static byte[] FromFmod(byte[] d, uint crc)
		{
			int num = BitConverter.ToInt32(d, 60);
			int num2 = BitConverter.ToUInt16(d, num + 6);
			int num3 = BitConverter.ToUInt16(d, num + 20);
			ulong imageBase = BitConverter.ToUInt64(d, num + 48);
			List<(uint Va, uint Size, uint Raw)> map = new List<(uint, uint, uint)>();
			for (int i = 0; i < num2; i++)
			{
				int num4 = num + 24 + num3 + 40 * i;
				uint val = BitConverter.ToUInt32(d, num4 + 8);
				uint item = BitConverter.ToUInt32(d, num4 + 12);
				uint val2 = BitConverter.ToUInt32(d, num4 + 16);
				uint item2 = BitConverter.ToUInt32(d, num4 + 20);
				map.Add((item, Math.Max(val, val2), item2));
			}
			byte[] bytes = BitConverter.GetBytes(crc);
			for (int j = 12; j + 20 <= d.Length; j++)
			{
				if (d[j] != bytes[0] || d[j + 1] != bytes[1] || d[j + 2] != bytes[2] || d[j + 3] != bytes[3])
				{
					continue;
				}
				long num5 = Offset(BitConverter.ToUInt64(d, j - 12));
				int num6 = (int)BitConverter.ToUInt32(d, j - 4);
				long num7 = Offset(BitConverter.ToUInt64(d, j + 4));
				int num8 = (int)BitConverter.ToUInt32(d, j + 12);
				int num9 = (int)BitConverter.ToUInt32(d, j + 16);
				if (num5 >= 0 && num7 >= 0 && num6 >= 64 && num6 <= 65536 && num8 >= 0 && num9 >= 0 && num8 + num9 <= num6 && num7 + num6 <= d.Length && num5 + num9 <= d.Length)
				{
					byte[] array = new byte[num6];
					Buffer.BlockCopy(d, (int)num7, array, 0, num6);
					Buffer.BlockCopy(d, (int)num5, array, num8, num9);
					if (Crc32(array) == crc)
					{
						return array;
					}
				}
			}
			return null;
			long Offset(ulong address)
			{
				if (address < imageBase)
				{
					return -1L;
				}
				ulong num10 = address - imageBase;
				foreach (var (num11, num12, num13) in map)
				{
					if (num10 >= num11 && num10 < (ulong)((long)num11 + (long)num12))
					{
						return (long)(num10 - num11 + num13);
					}
				}
				return -1L;
			}
		}

		private static uint Crc32(byte[] data)
		{
			if (_zipTable == null)
			{
				uint[] array = new uint[256];
				for (uint num = 0u; num < 256; num++)
				{
					uint num2 = num;
					for (int i = 0; i < 8; i++)
					{
						num2 = (((num2 & 1) != 0) ? ((num2 >> 1) ^ 0xEDB88320u) : (num2 >> 1));
					}
					array[num] = num2;
				}
				_zipTable = array;
			}
			uint num3 = uint.MaxValue;
			foreach (byte b in data)
			{
				num3 = (num3 >> 8) ^ _zipTable[(num3 ^ b) & 0xFF];
			}
			return num3 ^ 0xFFFFFFFFu;
		}

		private static uint OggCrc(byte[] data, int from, int length, uint crc)
		{
			if (_oggTable == null)
			{
				uint[] array = new uint[256];
				for (uint num = 0u; num < 256; num++)
				{
					uint num2 = num << 24;
					for (int i = 0; i < 8; i++)
					{
						num2 = (((num2 & 0x80000000u) != 0) ? ((num2 << 1) ^ 0x4C11DB7) : (num2 << 1));
					}
					array[num] = num2;
				}
				_oggTable = array;
			}
			for (int j = from; j < from + length; j++)
			{
				crc = (crc << 8) ^ _oggTable[((crc >> 24) ^ data[j]) & 0xFF];
			}
			return crc;
		}

		private static void Page(MemoryStream output, int sequence, long granule, byte flags, params ArraySegment<byte>[] packets)
		{
			List<byte> list = new List<byte>();
			int num = 0;
			ArraySegment<byte>[] array = packets;
			foreach (ArraySegment<byte> arraySegment in array)
			{
				int count = arraySegment.Count;
				for (int j = 0; j < count / 255; j++)
				{
					list.Add(byte.MaxValue);
				}
				list.Add((byte)(count % 255));
				num += count;
			}
			byte[] array2 = new byte[27 + list.Count + num];
			array2[0] = 79;
			array2[1] = 103;
			array2[2] = 103;
			array2[3] = 83;
			array2[5] = flags;
			BitConverter.GetBytes(granule).CopyTo(array2, 6);
			BitConverter.GetBytes(1).CopyTo(array2, 14);
			BitConverter.GetBytes(sequence).CopyTo(array2, 18);
			array2[26] = (byte)list.Count;
			list.CopyTo(array2, 27);
			int num2 = 27 + list.Count;
			array = packets;
			for (int i = 0; i < array.Length; i++)
			{
				ArraySegment<byte> arraySegment2 = array[i];
				Buffer.BlockCopy(arraySegment2.Array, arraySegment2.Offset, array2, num2, arraySegment2.Count);
				num2 += arraySegment2.Count;
			}
			BitConverter.GetBytes(OggCrc(array2, 0, array2.Length, 0u)).CopyTo(array2, 22);
			output.Write(array2, 0, array2.Length);
		}

		private static byte[] ToOgg(Sample s, byte[] setup, byte[] data)
		{
			byte[] array = new byte[30];
			array[0] = 1;
			Encoding.ASCII.GetBytes("vorbis").CopyTo(array, 1);
			array[11] = (byte)s.Channels;
			BitConverter.GetBytes(s.Rate).CopyTo(array, 12);
			array[28] = 184;
			array[29] = 1;
			byte[] array2 = new byte[20]
			{
				3, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0
			};
			Encoding.ASCII.GetBytes("vorbis").CopyTo(array2, 1);
			array2[7] = 4;
			Encoding.ASCII.GetBytes("fsb5").CopyTo(array2, 11);
			array2[^1] = 1;
			using MemoryStream memoryStream = new MemoryStream(data.Length + data.Length / 40 + 8192);
			Page(memoryStream, 0, 0L, 2, new ArraySegment<byte>(array));
			Page(memoryStream, 1, 0L, 0, new ArraySegment<byte>(array2), new ArraySegment<byte>(setup));
			List<ArraySegment<byte>> list = new List<ArraySegment<byte>>();
			int num = 0;
			while (num + 2 <= data.Length)
			{
				int num2 = BitConverter.ToUInt16(data, num);
				num += 2;
				if (num2 == 0 || num + num2 > data.Length)
				{
					break;
				}
				list.Add(new ArraySegment<byte>(data, num, num2));
				num += num2;
			}
			long num3 = 0L;
			int num4 = 0;
			for (int i = 0; i < list.Count; i++)
			{
				ArraySegment<byte> arraySegment = list[i];
				int num5 = ((((arraySegment.Array[arraySegment.Offset] >> 1) & 1) != 0) ? 2048 : 256);
				if (num4 != 0)
				{
					num3 += (num4 + num5) / 4;
				}
				num4 = num5;
				bool flag = i == list.Count - 1;
				Page(memoryStream, i + 2, flag ? s.Frames : Math.Min(num3, s.Frames), (byte)(flag ? 4u : 0u), arraySegment);
			}
			return memoryStream.ToArray();
		}
	}
