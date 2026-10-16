// namespace: SilentFX.Core
// type: AimCompatibility (class)
// source lines: 30546-31103
	internal static class AimCompatibility
	{
		internal readonly struct PeerInfo
		{
			internal readonly ulong NetId;

			internal readonly bool ReadyForBroadcasting;

			internal readonly object Data;

			internal PeerInfo(ulong netId, bool readyForBroadcasting, object data)
			{
				NetId = netId;
				ReadyForBroadcasting = readyForBroadcasting;
				Data = data;
			}
		}

		private sealed class HostInfo
		{
			internal readonly ulong NetId;

			internal readonly object Info;

			internal HostInfo(ulong netId, object info)
			{
				NetId = netId;
				Info = info;
			}
		}

		private sealed class Capabilities
		{
			internal Type VersionType;

			internal Type PeerDataType;

			internal Type RngType;

			internal FieldInfo Version;

			internal FieldInfo Branch;

			internal FieldInfo Hash;

			internal FieldInfo GameplayMods;

			internal FieldInfo OtherMods;

			internal FieldInfo PeerVersion;

			internal FieldInfo PeerId;

			internal FieldInfo PeerReady;

			internal PropertyInfo ClientLocalVersion;

			internal PropertyInfo HostLocalVersion;

			internal PropertyInfo HostConnectedPeers;

			internal PropertyInfo Seed;

			internal MethodInfo LocalDefault;

			internal MethodInfo ClientHandshake;

			internal ConstructorInfo HostConstructor;

			internal ConstructorInfo ClientConstructor;

			internal bool Available;

			internal string UnavailableReason;

			internal static Capabilities Detect()
			{
				Capabilities capabilities = new Capabilities();
				try
				{
					Assembly assembly = typeof(NetClientGameService).Assembly;
					capabilities.VersionType = assembly.GetType("MegaCrit.Sts2.Core.Multiplayer.PeerVersionInfo", throwOnError: false);
					capabilities.PeerDataType = assembly.GetType("MegaCrit.Sts2.Core.Entities.Multiplayer.NetClientData", throwOnError: false);
					capabilities.RngType = assembly.GetType("MegaCrit.Sts2.Core.Runs.RunRngSet", throwOnError: false);
					if (capabilities.VersionType == null || capabilities.PeerDataType == null || assembly.GetType("MegaCrit.Sts2.Core.Multiplayer.Connection.HandshakeManager", throwOnError: false) == null)
					{
						throw new MissingMemberException("game handshake version metadata is absent");
					}
					capabilities.Version = RequiredField(capabilities.VersionType, "version", typeof(string));
					capabilities.Branch = RequiredField(capabilities.VersionType, "branch");
					capabilities.Hash = RequiredField(capabilities.VersionType, "idDatabaseHash", typeof(uint));
					capabilities.GameplayMods = RequiredField(capabilities.VersionType, "gameplayAffectingMods", typeof(List<string>));
					capabilities.OtherMods = RequiredField(capabilities.VersionType, "otherMods", typeof(List<string>));
					capabilities.PeerVersion = RequiredField(capabilities.PeerDataType, "versionInfo", capabilities.VersionType);
					capabilities.PeerId = RequiredField(capabilities.PeerDataType, "peerId", typeof(ulong));
					capabilities.PeerReady = RequiredField(capabilities.PeerDataType, "readyForBroadcasting", typeof(bool));
					capabilities.ClientLocalVersion = RequiredProperty(typeof(NetClientGameService), "LocalVersion", capabilities.VersionType);
					capabilities.HostLocalVersion = RequiredProperty(typeof(NetHostGameService), "LocalVersion", capabilities.VersionType);
					capabilities.HostConnectedPeers = typeof(NetHostGameService).GetProperty("ConnectedPeers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (capabilities.HostConnectedPeers?.GetGetMethod(nonPublic: true) == null || !typeof(IEnumerable).IsAssignableFrom(capabilities.HostConnectedPeers.PropertyType) || capabilities.HostConnectedPeers.GetIndexParameters().Length != 0)
					{
						throw new MissingMemberException(typeof(NetHostGameService).FullName, "ConnectedPeers");
					}
					capabilities.Seed = capabilities.RngType?.GetProperty("Seed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (capabilities.Seed?.GetGetMethod(nonPublic: true) == null || (capabilities.Seed.PropertyType != typeof(ulong) && capabilities.Seed.PropertyType != typeof(uint)) || capabilities.Seed.GetIndexParameters().Length != 0)
					{
						throw new MissingMemberException("MegaCrit.Sts2.Core.Runs.RunRngSet", "Seed");
					}
					capabilities.LocalDefault = capabilities.VersionType.GetMethod("LocalDefault", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
					capabilities.ClientHandshake = typeof(NetClientGameService).GetMethod("HandshakeSucceeded", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
					{
						typeof(ulong),
						capabilities.VersionType
					}, null);
					capabilities.HostConstructor = typeof(NetHostGameService).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { capabilities.VersionType }, null);
					capabilities.ClientConstructor = typeof(NetClientGameService).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[1] { capabilities.VersionType }, null);
					if (capabilities.LocalDefault?.ReturnType != capabilities.VersionType || capabilities.ClientHandshake?.ReturnType != typeof(void) || capabilities.HostConstructor == null || capabilities.ClientConstructor == null)
					{
						throw new MissingMemberException("game handshake methods or constructors are absent");
					}
					capabilities.Available = true;
				}
				catch (Exception ex)
				{
					capabilities.UnavailableReason = ex.Message;
				}
				return capabilities;
			}

			private static FieldInfo RequiredField(Type type, string name, Type fieldType = null)
			{
				FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field == null || (fieldType != null && field.FieldType != fieldType))
				{
					throw new MissingMemberException(type.FullName, name);
				}
				return field;
			}

			private static PropertyInfo RequiredProperty(Type type, string name, Type propertyType)
			{
				PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (property?.PropertyType != propertyType || property.GetGetMethod(nonPublic: true) == null || property.GetIndexParameters().Length != 0)
				{
					throw new MissingMemberException(type.FullName, name);
				}
				return property;
			}
		}

		private const string ProtocolPrefix = "SilentFX.Targeting/";

		private const string Protocol = "SilentFX.Targeting/4:";

		private static readonly Capabilities Api = Capabilities.Detect();

		private static bool _reported;

		private static bool _normalized;

		private static object _normalizedCache;

		private static bool _hasAimType;

		private static string _fingerprintCache;

		private static Type[] _fingerprintTypes;

		private static KeyValuePair<Type, int>[] _fingerprintMap;

		private static readonly ConditionalWeakTable<NetClientGameService, HostInfo> Hosts = new ConditionalWeakTable<NetClientGameService, HostInfo>();

		internal static bool ExactAvailable => Api.Available;

		internal static MethodBase VersionTarget
		{
			get
			{
				if (!ExactAvailable)
				{
					return null;
				}
				return Api.LocalDefault;
			}
		}

		internal static MethodBase HostHandshakeTarget
		{
			get
			{
				if (!ExactAvailable)
				{
					return null;
				}
				return Api.ClientHandshake;
			}
		}

		internal static MethodBase HostServiceTarget
		{
			get
			{
				if (!ExactAvailable)
				{
					return null;
				}
				return Api.HostConstructor;
			}
		}

		internal static MethodBase ClientServiceTarget
		{
			get
			{
				if (!ExactAvailable)
				{
					return null;
				}
				return Api.ClientConstructor;
			}
		}

		internal static string Marker => "SilentFX.Targeting/4:" + Fingerprint();

		internal static void Initialize()
		{
			if (!_reported)
			{
				_reported = true;
				if (!ExactAvailable)
				{
					DevLog.Warn("[MP] Exact targeting unavailable; using native peer input: " + Api.UnavailableReason);
				}
			}
		}

		internal static bool NormalizeTypes()
		{
			Initialize();
			_normalized = false;
			_hasAimType = false;
			try
			{
				object obj = typeof(MessageTypes).GetField("_cache", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(null);
				if (obj == null)
				{
					return false;
				}
				Type type = obj.GetType();
				FieldInfo field = type.GetField("_idToType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				FieldInfo field2 = type.GetField("_typeToId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field?.GetValue(obj) is List<Type> list)
				{
					object obj2 = field2?.GetValue(obj);
					Dictionary<Type, int> map = obj2 as Dictionary<Type, int>;
					if (map != null)
					{
						Dictionary<Type, int> dictionary = new Dictionary<Type, int>(list.Count);
						for (int i = 0; i < list.Count; i++)
						{
							dictionary[list[i]] = i;
						}
						List<Type> list2 = list.Where((Type t) => t != typeof(AimMessage)).ToList();
						Dictionary<Type, int> dictionary2 = new Dictionary<Type, int>(map.Count);
						for (int num = 0; num < list2.Count; num++)
						{
							dictionary2[list2[num]] = num;
						}
						foreach (KeyValuePair<Type, int> item in map)
						{
							if (item.Key != typeof(AimMessage) && (!dictionary.TryGetValue(item.Key, out var value) || item.Value != value))
							{
								dictionary2[item.Key] = item.Value;
							}
						}
						Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
						foreach (KeyValuePair<Type, int> item2 in dictionary2)
						{
							int num2 = (map.TryGetValue(item2.Key, out var value2) ? value2 : dictionary[item2.Key]);
							if (dictionary3.TryGetValue(item2.Value, out var value3) && value3 != num2)
							{
								DevLog.Warn("[MP] Aim message normalization was skipped because it would merge distinct message IDs.");
								return false;
							}
							dictionary3[item2.Value] = num2;
						}
						bool flag = dictionary2.Values.Contains(list2.Count);
						bool flag2 = ExactAvailable && list2.Count < 255 && !flag;
						if (flag2)
						{
							dictionary2[typeof(AimMessage)] = list2.Count;
							list2.Add(typeof(AimMessage));
						}
						bool flag3 = !list.SequenceEqual(list2);
						bool flag4 = map.Count != dictionary2.Count || dictionary2.Any((KeyValuePair<Type, int> pair) => !map.TryGetValue(pair.Key, out var value4) || value4 != pair.Value);
						if (flag3)
						{
							field.SetValue(obj, list2);
						}
						if (flag4)
						{
							field2.SetValue(obj, dictionary2);
						}
						if ((obj != _normalizedCache) | flag3 | flag4)
						{
							_fingerprintCache = null;
							_fingerprintTypes = null;
							_fingerprintMap = null;
						}
						_normalized = true;
						_normalizedCache = obj;
						_hasAimType = flag2;
						if (ExactAvailable && !flag2)
						{
							DevLog.Warn(flag ? "[MP] Aim message was omitted because its ID is reserved by another message mapping." : "[MP] Aim message was omitted because the message table is full.");
						}
						return flag2;
					}
				}
				return false;
			}
			catch (Exception ex)
			{
				DevLog.Warn("[MP] Aim message normalization unavailable: " + ex.Message);
				return false;
			}
		}

		internal static void Advertise(ref object info)
		{
			try
			{
				if (!ExactAvailable || !Api.VersionType.IsInstanceOfType(info))
				{
					return;
				}
				bool flag = NormalizeTypes();
				if (!(Api.OtherMods.GetValue(info) is List<string> collection))
				{
					return;
				}
				List<string> list = new List<string>(collection);
				list.RemoveAll((string x) => x?.StartsWith("SilentFX.Targeting/", StringComparison.Ordinal) ?? false);
				if (flag && Tuning.F("mp.remote_fx", 1f) != 0f)
				{
					string marker = Marker;
					if (!marker.EndsWith(":unavailable", StringComparison.Ordinal))
					{
						list.Add(marker);
					}
				}
				Api.OtherMods.SetValue(info, list);
			}
			catch (Exception ex)
			{
				DevLog.Warn("[MP] Aim capability advertisement unavailable: " + ex.Message);
			}
		}

		internal static void RememberHost(NetClientGameService client, ulong host, object info)
		{
			if (client != null)
			{
				Hosts.Remove(client);
				if (ExactAvailable && Api.VersionType.IsInstanceOfType(info))
				{
					Hosts.Add(client, new HostInfo(host, info));
				}
			}
		}

		internal static bool HostMatches(NetClientGameService client)
		{
			if (!ExactAvailable || client == null || !Hosts.TryGetValue(client, out var value))
			{
				return false;
			}
			try
			{
				NetClient netClient = client.NetClient;
				return ((netClient != null) ? new ulong?(netClient.HostNetId) : ((ulong?)null)) == value.NetId && Matches(value.Info, Api.ClientLocalVersion.GetValue(client));
			}
			catch
			{
				return false;
			}
		}

		internal static bool PeerMatches(NetHostGameService host, object peer)
		{
			if (!ExactAvailable || host == null || !Api.PeerDataType.IsInstanceOfType(peer))
			{
				return false;
			}
			try
			{
				return Matches(Api.PeerVersion.GetValue(peer), Api.HostLocalVersion.GetValue(host));
			}
			catch
			{
				return false;
			}
		}

		internal static bool TryGetPeers(NetHostGameService host, out PeerInfo[] peers)
		{
			peers = Array.Empty<PeerInfo>();
			if (!ExactAvailable || host == null)
			{
				return false;
			}
			try
			{
				if (!(Api.HostConnectedPeers.GetValue(host) is IEnumerable enumerable))
				{
					return false;
				}
				List<PeerInfo> list = new List<PeerInfo>();
				foreach (object item in enumerable)
				{
					if (!Api.PeerDataType.IsInstanceOfType(item))
					{
						return false;
					}
					list.Add(new PeerInfo((ulong)Api.PeerId.GetValue(item), (bool)Api.PeerReady.GetValue(item), item));
				}
				peers = list.ToArray();
				return true;
			}
			catch
			{
				return false;
			}
		}

		internal static bool TryGetSeed(object rng, out ulong seed)
		{
			seed = 0uL;
			if (!ExactAvailable || rng == null || !Api.RngType.IsInstanceOfType(rng))
			{
				return false;
			}
			try
			{
				object value = Api.Seed.GetValue(rng);
				if (value is ulong num)
				{
					seed = num;
					return true;
				}
				if (value is uint num2)
				{
					seed = num2;
					return true;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

		internal static bool Matches(object remote, object local)
		{
			try
			{
				if (!ExactAvailable || !Api.VersionType.IsInstanceOfType(remote) || !Api.VersionType.IsInstanceOfType(local) || Tuning.F("mp.remote_fx", 1f) == 0f || !_normalized || !_hasAimType)
				{
					return false;
				}
				string marker = Marker;
				if (marker.EndsWith(":unavailable", StringComparison.Ordinal))
				{
					return false;
				}
				if (Api.OtherMods.GetValue(remote) is List<string> mods && Api.OtherMods.GetValue(local) is List<string> mods2 && Api.GameplayMods.GetValue(remote) is List<string> mods3 && Api.GameplayMods.GetValue(local) is List<string> mods4 && Api.Version.GetValue(remote) is string text && Api.Version.GetValue(local) is string text2)
				{
					object value = Api.Branch.GetValue(remote);
					if (value != null)
					{
						object value2 = Api.Branch.GetValue(local);
						if (value2 != null)
						{
							IEnumerable<string> enumerable = Sorted(mods);
							IEnumerable<string> enumerable2 = Sorted(mods2);
							if (!enumerable.Contains(marker) || !enumerable2.Contains(marker))
							{
								return false;
							}
							return text == text2 && value.Equals(value2) && Api.Hash.GetValue(remote).Equals(Api.Hash.GetValue(local)) && Sorted(mods3).SequenceEqual(Sorted(mods4), StringComparer.Ordinal) && enumerable.SequenceEqual(enumerable2, StringComparer.Ordinal);
						}
					}
				}
				return false;
			}
			catch
			{
				return false;
			}
		}

		private static string Fingerprint()
		{
			try
			{
				object obj = typeof(MessageTypes).GetField("_cache", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(null);
				FieldInfo fieldInfo = obj?.GetType().GetField("_idToType", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				FieldInfo fieldInfo2 = obj?.GetType().GetField("_typeToId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (!_normalized || !_hasAimType || obj != _normalizedCache || !(fieldInfo?.GetValue(obj) is List<Type> list) || !(fieldInfo2?.GetValue(obj) is Dictionary<Type, int> dictionary))
				{
					return "unavailable";
				}
				if (_fingerprintCache != null && _fingerprintTypes != null && _fingerprintTypes.Length == list.Count && _fingerprintTypes.SequenceEqual(list) && SameMappings(dictionary, _fingerprintMap))
				{
					return _fingerprintCache;
				}
				int aimId = list.Count - 1;
				if (aimId < 0 || list[aimId] != typeof(AimMessage) || !dictionary.TryGetValue(typeof(AimMessage), out var value) || value != aimId || dictionary.Any((KeyValuePair<Type, int> pair) => pair.Key != typeof(AimMessage) && pair.Value == aimId))
				{
					return "unavailable";
				}
				KeyValuePair<Type, int>[] array = dictionary.OrderBy((KeyValuePair<Type, int> pair) => pair.Key.FullName, StringComparer.Ordinal).ThenBy((KeyValuePair<Type, int> pair) => pair.Key.Assembly.FullName, StringComparer.Ordinal).ThenBy((KeyValuePair<Type, int> pair) => pair.Value)
					.ToArray();
				string text = string.Join("\n", list.Select((Type t, int i) => "L|" + i.ToString(CultureInfo.InvariantCulture) + "|" + t.FullName + "|" + t.Assembly.FullName)) + "\n" + string.Join("\n", array.Select((KeyValuePair<Type, int> pair) => "M|" + pair.Value.ToString(CultureInfo.InvariantCulture) + "|" + pair.Key.FullName + "|" + pair.Key.Assembly.FullName));
				_fingerprintCache = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("SilentFX.Targeting/4:" + text))).ToLowerInvariant();
				_fingerprintTypes = list.ToArray();
				_fingerprintMap = array;
				return _fingerprintCache;
			}
			catch
			{
				return "unavailable";
			}
		}

		private static bool SameMappings(Dictionary<Type, int> map, KeyValuePair<Type, int>[] snapshot)
		{
			if (snapshot == null || snapshot.Length != map.Count)
			{
				return false;
			}
			for (int i = 0; i < snapshot.Length; i++)
			{
				KeyValuePair<Type, int> keyValuePair = snapshot[i];
				if (!map.TryGetValue(keyValuePair.Key, out var value) || value != keyValuePair.Value)
				{
					return false;
				}
			}
			return true;
		}

		private static IEnumerable<string> Sorted(List<string> mods)
		{
			return mods.OrderBy((string x) => x, StringComparer.Ordinal);
		}
	}
