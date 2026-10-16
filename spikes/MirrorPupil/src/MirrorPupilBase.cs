using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace MirrorPupil;

/// <summary>
/// 言灵·镜瞳的共享逻辑。<see cref="MirrorPupil"/>（基础版）与 <see cref="MirrorPupilAncient"/>（先古版）
/// 只差两个数字：瞳印每次减少多少、减少后的下限。
///
/// 镜像的生命持久化：镜像的生命/上限每次变动都会由 <see cref="MirrorImageCmd"/> 同步进
/// <see cref="MirrorHp"/>/<see cref="MirrorMaxHp"/>（[SavedProperty]，随存档自动持久化），
/// 下一场战斗召唤时从这两个值取。
/// </summary>
public abstract class MirrorPupilBase : RelicModel
{
    /// <summary>每场战斗开始时获得的瞳印层数。</summary>
    public virtual int StartingMarks => 100;

    /// <summary>判定成功后瞳印减少的层数。</summary>
    public virtual int ProcDecrement => 25;

    /// <summary>瞳印减少后的下限。</summary>
    public virtual int ProcFloor => 25;

    /// <summary>镜像初始生命上限 = 角色初始最大生命 × 该比例（向上取整）。</summary>
    public virtual decimal MirrorHpFraction => 0.3m;

    private int _mirrorMaxHp;

    private int _mirrorHp;

    /// <summary>镜像的生命上限。镜像一变动就同步进来。</summary>
    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int MirrorMaxHp
    {
        get
        {
            return _mirrorMaxHp;
        }
        set
        {
            AssertMutable();
            _mirrorMaxHp = value;
        }
    }

    /// <summary>镜像的生命。镜像一变动就同步进来；≤0 时下场战斗不召唤。</summary>
    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int MirrorHp
    {
        get
        {
            return _mirrorHp;
        }
        set
        {
            AssertMutable();
            _mirrorHp = value;
        }
    }

    public override bool SpawnsPets => true;

    /// <summary>遗物悬停信息：显示镜像当前状态（2026-10-03 用户要求）。</summary>
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            var ls = new LocString("relics", "MIRROR_PUPIL.mirrorStatus");
            ls.Add("Hp", (decimal)MirrorHp);
            ls.Add("MaxHp", (decimal)MirrorMaxHp);
            yield return new HoverTip(ls);
        }
    }

    public override async Task AfterObtained()
    {
        // 一局游戏开始（获得本遗物）时：镜像初始生命/上限 = 角色初始最大生命的 30%（向上取整）。
        // 之后不再重算——数值只在镜像变动时被镜像自己同步进来。
        if (MirrorMaxHp <= 0)
        {
            int initial = (int)Math.Ceiling(Owner.Character.StartingHp * MirrorHpFraction);
            MirrorMaxHp = initial;
            MirrorHp = initial;
        }
        // 局外+局内回血联动统一数据源（2026-10-03 用户定案）：镜像该拿的是
        // 【要回多少血】而非实际变化量——角色满血时治疗不改变生命值，按变化量
        // 计算会漏掉。捕获在 MirrorPupilPatches.HealInternal_Postfix（直接解析
        // 遗物，无事件订阅时序问题）；此处仅初始化数值。
    }

    public override async Task BeforeCombatStart()
    {
        PlayerChoiceContext choiceContext = new ThrowingPlayerChoiceContext();

        // 每场战斗开始：获得瞳印，并召唤镜像（若其生命大于 0）。
        await PowerCmd.Apply<PupilMarkPower>(choiceContext, Owner.Creature, StartingMarks, null, null);
        if (MirrorHp > 0)
        {
            await MirrorImageCmd.SummonFull(choiceContext, Owner, MirrorMaxHp, MirrorHp);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 防护铁律：钩子里绝不让异常逃逸——出牌任务中断=卡牌永远卡在处理区
        // （2026-10-02 日志实锤）。任何失败记日志后照常返回。
        try
        {
            await AfterCardPlayedInner(choiceContext, cardPlay);
        }
        catch (Exception e)
        {
            Godot.GD.PrintErr("[MirrorPupil] AfterCardPlayed failed (contained): " + e);
        }
    }

    private async Task AfterCardPlayedInner(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 逐点诊断（每张牌最多三行，量可控）
        int marksProbe = Owner.Creature.GetPowerAmount<PupilMarkPower>();
        Godot.GD.Print($"[MirrorPupil] hook enter: card={cardPlay.Card.Id} owner={cardPlay.Card.Owner == Owner} marks={marksProbe}");
        // 防递归（用户方案）：由镜像触发的重放牌不再进入判定
        if (MirrorImageCmd.InAttackScope)
        {
            return;
        }
        if (cardPlay.Card.Owner != Owner)
        {
            return;
        }

        int marks = Owner.Creature.GetPowerAmount<PupilMarkPower>();
        if (marks <= 0)
        {
            return;
        }

        // 用户定案的完整顺序（2026-10-03）：判定 → 召唤 → 判是否攻击牌 →
        // 再打出攻击牌 → 最后减少层数。（防递归由上面的作用域判定承担，
        // 扣层放最后不再有无限重放风险。）
        int roll = Owner.RunState.Rng.Niche.NextInt(1, 101);
        Godot.GD.Print($"[MirrorPupil] roll={roll} marks={marks} proc={roll <= marks}");
        if (roll > marks)
        {
            return;
        }

        // 1. 召唤：镜像召唤 1（活着则生命/上限各 +1，已死亡则以 1/1 复活）。
        await MirrorImageCmd.SummonOne(choiceContext, Owner);

        // 2. 攻击牌 → 结算完后用【普通的打出】把这张牌再打出（用户指定，
        //    不走 AutoPlay——它的默认流程硬插 0.25-0.35s 牌堆动画等待+结尾
        //    最小等待，skipCardPileVisuals 又会把牌的补间移动跳掉=视觉卡死
        //    在处理区）。直接调 OnPlayWrapper 手动管线：AddDuringManualCardPlay
        //    自会把处理区的牌摆位、正常播放打出与进堆动画，节奏=普通出牌。
        //    资源沿用原牌的结算信息。伤害来源视为镜像。
        //    ⚠️ 目标永不为 null：君王之剑带 SeekingEdge 时 TargetType 变
        //    AllEnemies，游戏允许空目标打出（IsValidTarget(null)=true for
        //    AllEnemies）——原牌 cardPlay.Target 可能本来就是 null（2026-10-03
        //    实锤：原目标没死也炸 ThrowIfNull）。手动管线又没有 AutoPlay 的
        //    重随机逻辑。规则=原目标活着用原目标，否则/为空从存活敌人重随机；
        //    无存活敌人→不重放也不扣层。
        bool replayed = false;
        if (cardPlay.Card.Type == CardType.Attack)
        {
            Creature? mirror2 = MirrorImageCmd.FindMirror(Owner);
            if (mirror2 is { IsAlive: true })
            {
                IReadOnlyList<Creature>? hittable = Owner.Creature.CombatState?.HittableEnemies;
                Godot.GD.Print($"[MirrorPupil] replay gate: attack card, hittable={hittable?.Count ?? -1}");
                if (hittable != null && hittable.Count > 0)
                {
                    // 目标三段式（用户定案）：原目标合法（存活且在可打列表）用原
                    // 目标；不合法则随机一个合法目标；一个都没有→不重放不扣层。
                    Creature? target = null;
                    if (cardPlay.Target is { IsAlive: true } alive && hittable.Contains(alive))
                    {
                        target = alive;
                    }
                    else
                    {
                        target = Owner.RunState.Rng.CombatTargets.NextItem(hittable);
                    }
                    if (target != null)
                    {
                        using (MirrorImageCmd.AttackAs(mirror2))
                        {
                            await cardPlay.Card.OnPlayWrapper(choiceContext, target,
                                isAutoPlay: false, cardPlay.Resources);
                        }
                        replayed = true;
                        Godot.GD.Print("[MirrorPupil] replay done");
                    }
                }
            }
        }

        // 3. 只有重放真正发生才减少层数（不低于下限）——用户规则（2026-10-03）：
        //    非攻击牌 proc 只召唤；攻击牌无存活敌人/无镜像时也不扣。
        if (replayed)
        {
            PupilMarkPower? power = Owner.Creature.GetPower<PupilMarkPower>();
            if (power != null)
            {
                decimal reduced = Math.Max(ProcFloor, power.Amount - ProcDecrement);
                if (reduced != power.Amount)
                {
                    await PowerCmd.ModifyAmount(choiceContext, power, reduced - power.Amount, null, null);
                }
            }
        }
    }

    // 联动回血统一走 HealInternal postfix（MirrorPupilPatches.HealInternal_Postfix
    // → HandleOwnerHeal）：按【请求量】联动（角色满血时治疗不改变生命值，按实际
    // 变化量计算会漏）；只处理玩家本体的治疗（宠物/镜像的回血不进来=防递归）。

    /// <summary>主人受到治疗（含满血溢出）时由补丁调用：镜像回一半（向上取整）。
    /// 在场走 CreatureCmd.Heal（带特效）；不在场直接记账进遗物状态。</summary>
    internal void HandleOwnerHeal(decimal amount)
    {
        try
        {
            if (amount <= 0m || MirrorMaxHp <= 0)
            {
                return;
            }
            decimal share = Math.Ceiling(amount / 2m);
            Creature? mirror = MirrorImageCmd.FindMirror(Owner);
            if (mirror is { IsAlive: true })
            {
                MegaCrit.Sts2.Core.Helpers.TaskHelper.RunSafely(CreatureCmd.Heal(mirror, share));
                return;
            }
            MirrorHp = Math.Min(MirrorMaxHp, MirrorHp + (int)share);
        }
        catch (Exception e)
        {
            Godot.GD.PrintErr("[MirrorPupil] heal link failed (contained): " + e.Message);
        }
    }

    /// <summary>把镜像的当前生命/上限写回遗物（由 <see cref="MirrorImageCmd"/> 在镜像创建与变动时调用）。</summary>
    internal void SyncMirrorState(Creature mirror)
    {
        MirrorMaxHp = mirror.MaxHp;
        MirrorHp = mirror.CurrentHp;
    }
}
