using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Modding;

namespace LingChar;

/// <summary>
/// 零（Ling）可玩角色 mod。
/// 注册机制（反编译考证，无需手动注册模型）：
///   ModelDb.Init() 遍历 AllAbstractModelSubtypes = 游戏子类型 +
///   ReflectionHelper.GetSubtypesInMods&lt;AbstractModel&gt;() —— mod 程序集里的
///   CharacterModel 子类会被自动实例化进 _contentById（id=Slugify(类名)="LING"）。
/// 唯一要补的是 ModelDb.AllCharacters 硬编码数组（选人界面/卡池聚合/解锁集合都走它），
/// 用 Harmony postfix 追加 Ling。解锁门控（UnlockState.Characters）= AllCharacters
/// 减去 epoch 门控角色，Ling 无 epoch 门控 = 默认解锁，零存档迁移。
/// 言灵·镜瞳遗物（MirrorPupil spike，源码级集成）注册进 Ling 借用的 Silent 遗物池。
/// </summary>
[ModInitializer(nameof(Initialize))]
public static class ModEntry
{
    public static void Initialize()
    {
        // 纪元注册先于一切 epoch 查询（Act Boss 胜利即查 LING2/3/4_EPOCH）
        LingEpochs.Register();
        // 遗物进池（Ling 的遗物池=借用 SilentRelicPool）：ModelDb 预载校验 + 奖励池可见
        ModHelper.AddModelToPool<SilentRelicPool, global::MirrorPupil.MirrorPupil>();
        // 先古版镜瞳必须进 EventRelicPool（与原版 BlackBlood/RingOfTheDrake 同池，见
        // EventRelicPool.GenerateAllRelics）：先古之民-欧洛巴斯事件 TouchOfOrobas.SetupForPlayer
        // → set_UpgradedRelic 读 HoverTips → RelicModel.get_Pool 对 ModelDb.AllRelicPools
        // 做 First(p => p.AllRelicIds.Contains(Id))，不在任何池=InvalidOperationException
        // "Sequence contains no matching element" → GenerateInitialOptions 中断 → 事件 0 选项
        // （2026-10-10 godot.log:7079 实锤）。EventRelicPool 是共享事件池，不进任何奖励池。
        ModHelper.AddModelToPool<EventRelicPool, global::MirrorPupil.MirrorPupilAncient>();
        // dye 框架（影子/溶解/闪红/入场）已废案（条目 61，2026-10-16）：
        // DyeFramework.Initialize 调用随之移除，影子源码存档见根目录 ShadowSource_Archived.cs。
        // 选人立绘预热（重 spine 资产线程化加载+保活+管线焐热，消除切换卡顿）
        SelectWarm.Warm();
        new Harmony("LingChar").PatchAll(typeof(ModEntry).Assembly);
    }
}

[HarmonyPatch(typeof(ModelDb), "get_AllCharacters")]
internal static class AllCharactersPatch
{
    static void Postfix(ref IEnumerable<CharacterModel> __result)
    {
        if (!ModelDb.Contains(typeof(Ling))) return; // Init 之前的极早调用，跳过
        __result = __result.Append(ModelDb.Character<Ling>());
    }
}
