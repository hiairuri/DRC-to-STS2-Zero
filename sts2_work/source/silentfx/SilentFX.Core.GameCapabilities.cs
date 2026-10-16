// namespace: SilentFX.Core
// type: GameCapabilities (class)
// source lines: 36925-37162
	internal static class GameCapabilities
	{
		private const BindingFlags Declared = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private static readonly Dictionary<string, Type> Types = new Dictionary<string, Type>(StringComparer.Ordinal);

		private static readonly Lazy<bool> ExpertiseRetention = new Lazy<bool>(DetectExpertiseRetention);

		private static readonly Type BladeSymphony = Card("BladeSymphony");

		private static string _expertiseProbeFailure;

		private static bool _reported;

		internal static readonly MethodInfo ShivCreateInHand = FindShivCreateInHand();

		internal static readonly MethodInfo PoisonTrigger = DeclaredMethod(Power("PoisonPower"), "Trigger");

		internal static readonly MethodInfo ConcoctAfterDamageGiven = DeclaredMethod(Power("ConcoctPower"), "AfterDamageGiven");

		internal static bool ExpertiseRetainsDrawnCards => ExpertiseRetention.Value;

		internal static string ExpertiseProbeFailure => _expertiseProbeFailure;

		internal static Type Resolve(string fullName)
		{
			lock (Types)
			{
				if (!Types.TryGetValue(fullName, out var value))
				{
					value = (Types[fullName] = typeof(CardModel).Assembly.GetType(fullName, throwOnError: false));
				}
				return value;
			}
		}

		internal static Type Card(string name)
		{
			return Resolve("MegaCrit.Sts2.Core.Models.Cards." + name);
		}

		internal static Type Power(string name)
		{
			return Resolve("MegaCrit.Sts2.Core.Models.Powers." + name);
		}

		internal static bool Is(CardModel card, string name)
		{
			if (card != null)
			{
				return ((object)card).GetType() == Card(name);
			}
			return false;
		}

		internal static bool IsBladeSymphony(CardModel card)
		{
			if (card != null)
			{
				return ((object)card).GetType() == BladeSymphony;
			}
			return false;
		}

		internal static void Initialize()
		{
			bool expertiseRetainsDrawnCards = ExpertiseRetainsDrawnCards;
			if (!_reported)
			{
				_reported = true;
				DevLog.Info($"Game capabilities: Shiv creation args={ShivCreateInHand?.GetParameters().Length ?? 0}, Poison trigger={PoisonTrigger != null}, Concoct={ConcoctAfterDamageGiven != null}, Expertise retain FX={expertiseRetainsDrawnCards}");
				if (_expertiseProbeFailure != null)
				{
					DevLog.Warn("Expertise retain FX disabled: " + _expertiseProbeFailure);
				}
			}
		}

		private static MethodInfo FindShivCreateInHand()
		{
			Type type = Card("Shiv");
			MethodInfo? methodInfo = type?.GetMethod("CreateInHand", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[4]
			{
				typeof(Player),
				typeof(int),
				typeof(ICombatState),
				typeof(Player)
			}, null);
			if ((object)methodInfo == null)
			{
				if ((object)type == null)
				{
					return null;
				}
				methodInfo = type.GetMethod("CreateInHand", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[3]
				{
					typeof(Player),
					typeof(int),
					typeof(ICombatState)
				}, null);
			}
			return methodInfo;
		}

		private static MethodInfo DeclaredMethod(Type type, string name)
		{
			if (type == null)
			{
				return null;
			}
			MethodInfo methodInfo = null;
			MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo2 in methods)
			{
				if (!(methodInfo2.Name != name))
				{
					if (methodInfo != null)
					{
						return null;
					}
					methodInfo = methodInfo2;
				}
			}
			return methodInfo;
		}

		private static bool DetectExpertiseRetention()
		{
			try
			{
				MethodInfo? methodInfo = (DeclaredMethod(Card("Expertise"), "OnPlay")?.GetCustomAttribute<AsyncStateMachineAttribute>()?.StateMachineType)?.GetMethod("MoveNext", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (methodInfo == null)
				{
					throw new InvalidOperationException("Expertise async OnPlay body unavailable");
				}
				return CallsSingleTurnRetain(methodInfo);
			}
			catch (Exception ex)
			{
				_expertiseProbeFailure = ex.GetType().Name + ": " + ex.Message;
				return false;
			}
		}

		private static bool CallsSingleTurnRetain(MethodInfo method)
		{
			byte[] array = method.GetMethodBody()?.GetILAsByteArray();
			if (array == null)
			{
				throw new InvalidOperationException("Expertise IL unavailable");
			}
			Dictionary<ushort, OpCode> dictionary = new Dictionary<ushort, OpCode>();
			FieldInfo[] fields = typeof(OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.FieldType == typeof(OpCode))
				{
					OpCode value = (OpCode)fieldInfo.GetValue(null);
					dictionary[(ushort)value.Value] = value;
				}
			}
			bool result = false;
			int num3;
			for (int j = 0; j < array.Length; j += num3)
			{
				ushort num = array[j++];
				if (num == 254)
				{
					Require(array, j, 1);
					num = (ushort)(0xFE00 | array[j++]);
				}
				if (!dictionary.TryGetValue(num, out var value2))
				{
					throw new InvalidOperationException("Unknown Expertise IL opcode");
				}
				switch (value2.OperandType)
				{
				case OperandType.InlineNone:
					num3 = 0;
					break;
				case OperandType.ShortInlineBrTarget:
				case OperandType.ShortInlineI:
				case OperandType.ShortInlineVar:
					num3 = 1;
					break;
				case OperandType.InlineVar:
					num3 = 2;
					break;
				case OperandType.InlineBrTarget:
				case OperandType.InlineField:
				case OperandType.InlineI:
				case OperandType.InlineMethod:
				case OperandType.InlineSig:
				case OperandType.InlineString:
				case OperandType.InlineTok:
				case OperandType.InlineType:
				case OperandType.ShortInlineR:
					num3 = 4;
					break;
				case OperandType.InlineI8:
				case OperandType.InlineR:
					num3 = 8;
					break;
				case OperandType.InlineSwitch:
				{
					Require(array, j, 4);
					int num2 = BitConverter.ToInt32(array, j);
					if (num2 < 0)
					{
						throw new InvalidOperationException("Invalid Expertise IL switch");
					}
					num3 = checked(4 + num2 * 4);
					break;
				}
				default:
					throw new InvalidOperationException("Unknown Expertise IL operand");
				}
				Require(array, j, num3);
				if (value2 == OpCodes.Call || value2 == OpCodes.Callvirt)
				{
					MethodBase methodBase = method.Module.ResolveMethod(BitConverter.ToInt32(array, j), method.DeclaringType?.GetGenericArguments(), method.GetGenericArguments());
					if (methodBase?.DeclaringType?.FullName == "MegaCrit.Sts2.Core.Commands.CardCmd" && methodBase.Name == "ApplySingleTurnRetain")
					{
						result = true;
					}
				}
			}
			return result;
		}

		private static void Require(byte[] il, int offset, int size)
		{
			if (size < 0 || offset > il.Length - size)
			{
				throw new InvalidOperationException("Truncated Expertise IL");
			}
		}
	}
