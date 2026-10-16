using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Timeline;

namespace LingChar;

/// <summary>
/// Ling 的纪元注册。原版 ProgressSaveManager.ObtainCharUnlockEpoch 对每个可玩
/// 角色硬性要求 &lt;ID&gt;2/3/4_EPOCH 存在（每次 Act Boss 战胜后
/// EpochModel.Get(text+"N_EPOCH")，查不到就抛 ArgumentException——2026-10-03
/// 日志实锤 LING2/LING3_EPOCH 两连炸，炸断战斗收尾=处理区卡死）。
/// EpochModelSubtypes.All 是硬编码原版列表，mod 纪元不会自动注册——这里用
/// 反射把 LING1..5_EPOCH 注入 EpochModel 的私有静态注册表（先强跑静态构造，
/// 再加字典、重置惰性缓存）。纪元放在无人使用的 FarFuture1 列（0-4 行），
/// QueueUnlocks 用基类空实现（零默认解锁，无解锁演出）。
/// </summary>
internal static class LingEpochs
{
	private class Ling1Epoch : EpochModel
	{
		public override string Id => "LING1_EPOCH";
		public override EpochEra Era => EpochEra.FarFuture1;
		public override int EraPosition => 0;
	}

	private class Ling2Epoch : EpochModel
	{
		public override string Id => "LING2_EPOCH";
		public override EpochEra Era => EpochEra.FarFuture1;
		public override int EraPosition => 1;
	}

	private class Ling3Epoch : EpochModel
	{
		public override string Id => "LING3_EPOCH";
		public override EpochEra Era => EpochEra.FarFuture1;
		public override int EraPosition => 2;
	}

	private class Ling4Epoch : EpochModel
	{
		public override string Id => "LING4_EPOCH";
		public override EpochEra Era => EpochEra.FarFuture1;
		public override int EraPosition => 3;
	}

	private class Ling5Epoch : EpochModel
	{
		public override string Id => "LING5_EPOCH";
		public override EpochEra Era => EpochEra.FarFuture1;
		public override int EraPosition => 4;
	}

	private static readonly System.Type[] EpochTypes =
	{
		typeof(Ling1Epoch), typeof(Ling2Epoch), typeof(Ling3Epoch),
		typeof(Ling4Epoch), typeof(Ling5Epoch),
	};

	public static void Register()
	{
		try
		{
			// force the static ctor BEFORE touching the private dictionaries
			RuntimeHelpers.RunClassConstructor(typeof(EpochModel).TypeHandle);
			var byId = (Dictionary<string, System.Type>?)AccessTools.Field(typeof(EpochModel), "_epochTypeDictionary")?.GetValue(null);
			var byType = (Dictionary<System.Type, string>?)AccessTools.Field(typeof(EpochModel), "_typeToIdDictionary")?.GetValue(null);
			if (byId == null || byType == null)
			{
				GD.PrintErr("[LingChar] epoch dictionaries not found, epochs not registered");
				return;
			}
			foreach (var t in EpochTypes)
			{
				var instance = (EpochModel)System.Activator.CreateInstance(t)!;
				byId[instance.Id] = t;
				byType[t] = instance.Id;
			}
			// reset the lazy AllEpochIds cache so it re-derives with ours
			AccessTools.Field(typeof(EpochModel), "_allEpochIds")?.SetValue(null, null);
			// 序列化网络 ID 缓存（ModelIdSerializationCache）在启动时从
			// AllEpochIds 构建并冻结——早于 mod 初始化，我们刚注入的纪元不在表里
			// → 战斗结束写回放时 GetNetIdForEpochId 抛异常炸断收尾（2026-10-03
			// 日志实锤 LING2_EPOCH）。补注入两侧映射并重算位宽。
			var nameToId = AccessTools.Field(typeof(MegaCrit.Sts2.Core.Multiplayer.Serialization.ModelIdSerializationCache), "_epochNameToNetIdMap")?.GetValue(null) as Dictionary<string, int>;
			var idToName = AccessTools.Field(typeof(MegaCrit.Sts2.Core.Multiplayer.Serialization.ModelIdSerializationCache), "_netIdToEpochNameMap")?.GetValue(null) as List<string>;
			if (nameToId != null && idToName != null)
			{
				foreach (var t in EpochTypes)
				{
					var id = ((EpochModel)System.Activator.CreateInstance(t)!).Id;
					if (!nameToId.ContainsKey(id))
					{
						nameToId[id] = idToName.Count;
						idToName.Add(id);
					}
				}
				AccessTools.Property(typeof(MegaCrit.Sts2.Core.Multiplayer.Serialization.ModelIdSerializationCache), "EpochIdBitSize")
					?.SetValue(null, Godot.Mathf.CeilToInt(Godot.Mathf.Log(System.Math.Max(1, idToName.Count)) / Godot.Mathf.Log(2f)));
				GD.Print($"[LingChar] epoch net-id cache patched (epochs={idToName.Count})");
			}
			else
			{
				GD.Print("[LingChar] net-id cache not initialized yet (will build from AllEpochIds)");
			}
			GD.Print($"[LingChar] epochs registered: LING1..5_EPOCH (total ids={byId.Count})");
		}
		catch (System.Exception e)
		{
			GD.PrintErr("[LingChar] epoch registration failed (contained): " + e);
		}
	}
}
