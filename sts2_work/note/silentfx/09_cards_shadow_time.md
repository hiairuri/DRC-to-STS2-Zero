# SilentFX v0.72.7 二探报告：卡牌特效矿脉（下）——子弹时间 / 暗影步 / 悼词持牌 / 持牌三态机

> 挖掘代理：deep-shadow-time（二探）｜日期：2026-10-16
> 材料：反编译切分源码 `silentfx_decomp/split/`（只读）、mod 资产 `3817850774/`（只读）、初探报告 01/02/03
> 证据格式：`split文件名:行号`（切分文件内行号；源单体行号见各文件头 `// source lines: X-Y`）
> **切分伪影提醒**：split 文件尾部的 `[HarmonyPatch]` 属性属于源码顺序上的**下一个**类型（01_architecture.md 已校验），本报告所有补丁归属均按此规则校正。

**结论先行**：
1. 子弹时间是全库最硬的系统——「只改表现不改事实」靠 6 类补丁（死亡/血条/伤害数字/火花/声音/场景 Vfx/生物动作/敌人速度）把游戏原生反馈全部拦成「隐藏 + 排队」，回合末用 `0.18+(At-t0)*压缩比` 的追赶公式一次性爆出；联机在 `Begin()` 第一道门 `Players.Count > 1` 直接枪毙。
2. 83 个 Vfx 工厂不是硬编码名单，而是**反射全量扫描 `MegaCrit.Sts2.Core.Nodes.Vfx` 命名空间**（排除 4 个子命名空间 + 27 个 Skip 前缀），对每个 `Create` 挂 Postfix；恢复时不解冻节点，而是用 `(MethodBase, args)` **重放配方**重新调一次 `Create`。
3. 暗影步六阶段是一条「双时钟时间线」：`Anim`（动画时钟，Hold 时暂停）驱动 `Move()` 的 6 段分支，`Clock`（真实时钟）驱动雾的呼吸；待命会话通过 `Reach0/Clock0/Dark0/Spin` 四个静态字段与打出阶段无缝接力。
4. 持牌三态机（Live/Waiting/Playing）的核心创意是「悬停会话可直接提拔为持卡会话」与「打出会话转入 Waiting 等 CardFx 来 Claim」——两次零重启迁移。
5. 悼词持牌把「本回合死过的牌」从 CombatHistory 读出来环成瞄准环，打出时洗牌齐射、最朝向敌人的一张当重物压轴，取景框本身会被砸碎。

---

## ① 子弹时间系统全景（TimeFreeze 2068 行 + VfxFactoryPatches 148 行 + 9 个 Hold 补丁）

### 1.1 总纲与「只改表现不改事实」的代码级保证

tuning.ini:230-234 的设计自述：「It only changes what is shown, never what happens, and is always off in co-op.」代码层对应六道保证：

| 保证 | 证据 | 机制 |
|---|---|---|
| 联机永远关闭 | TimeFreeze.cs:288-291 | `val3.Players.Count > 1 → return false`（在拿 CombatRoom 之前的第一道业务门） |
| 不碰游戏事实 | Hold() TimeFreeze.cs:592-657 | 只做 `Item.Visible=false` + 记录 Parent/Place/Turn；游戏原生节点**不动 ProcessMode**（只有自己的节点才设 4） |
| 恢复不改状态 | Thaw() :1265-1473 | 全程无游戏 API 调用，只重放自家录制的 Events / 重调 Create |
| 玩家开关 | Begin :258-261 | `!PlayerSettings.Enabled → false`；OnSettingsChanged 实时 Thaw（:162-202） |
| 调参开关 | :268-271 | `bullet.freeze < 0.5 → false` |
| 开发模式互斥 | :272-275 | `Showcase.Active → false`（F4 展示模式不冻） |
| mod 间让位 | AnotherModStopsTime() :409-438 | 扫 AppDomain 找 `BulletTimeAnime`（"Bullet Time: As Depicted"），找到就只留时钟+闪光；`bullet.yield_to_other_mods=1` 可关 |

### 1.2 冻结清单：9 类游戏反馈的拦截点

| 冻结对象 | 补丁（split 文件） | 拦截方式 | 冻结时的动作 | 恢复时的动作 |
|---|---|---|---|---|
| 场景 Vfx | SceneVfxHoldPatch.cs:6-22（`VfxCmd.PlayVfx`） | Prefix return false | `Events.Add((_clock, 播放))`，`_heldScenes++` | Due 到点重调 `VfxCmd.PlayVfx` |
| 全屏 Vfx | FullScreenVfxHoldPatch.cs:6-22（`PlayFullScreenInCombat`） | 同上 | 同上 | 同上 |
| 音效 | SoundHoldPatch.cs:6-22（`NAudioManager.PlayOneShot`） | 同上 | 深拷 parameters 入 Events，`_heldSounds++`；`_clock<0.35` 与 `/ui/` 路径不扣（:960-967） | 到点重播 |
| 死亡动画 | DeathHoldPatch.cs:6-26（`NCreature.StartDeathAnim`） | Prefix 写 `__result=0.6f` + return false | `Deaths.Add((node, shouldRemove))`；仅 `entity.IsEnemy`、`bullet.hold_death=1`（:1144-1168） | 最后统一 `StartDeathAnim(remove)`（Die :1475-1488，时点 `num6` 再 +0.15s） |
| 血条刷新 | HealthBarHoldPatch.cs:6-21（`NHealthBar.RefreshValues`） | Prefix return false | `Bars.Add(bar)` + TimeFreezeHover.Track | `0.18+num4*scale*0.5` 时统一 RefreshValues |
| 伤害数字 | DamageNumberHoldPatch.cs:6-21（`NDamageNumVfx.Create` 后缀） | Postfix 藏节点 | `Visible=false` + Events 记 (at, damage) | `_replaying=true` 重调 Create（SpawnNumber :1199-1224） |
| 命中火花 | HitSparkHoldPatch.cs:6-21（`NHitSparkVfx.Create` 后缀） | Postfix 藏节点 | 同上 | `_replaying=true` 重放，目标还活着才重建（:1240-1262） |
| 生物动画 | SkippedMotionPatch.cs:6-50（`NCreature.SetAnimationTrigger`） | Prefix return false | `Motions[node].Add(trigger)`，仅白名单 5 个：`Attack/Cast/PowerUp/Shiv/Hit`（HeldMotions :142） | Attack/Shiv→0.09s 补播 Attack；Cast/PowerUp→0.09s 补播 Cast；Hit→`0.18+跨度*0.35`（:1358-1394） |
| 敌人速度 | CreatureHoldBulletSlowPatch.cs:6-17（`TimeFreeze.SlowEnemies` 后缀） | Postfix 联动 | `CreatureHold.AfterBulletSlow(Slowed)` 把敌人登记进 BulletSpeeds | `AfterBulletRestore` 清表（CreatureHold.cs:394-430） |
| 卡牌飞行 | FlightHold.cs:37-95（`NCardFlyVfx`） | 反射私有 `_speed/_accel` 归零 | 速度/加速度写 0、Rotation/Position 每帧钉死、Trail 隐藏 | 写回原值、Trail 恢复 |
| 其余全部 Vfx | **VfxFactoryPatches（83 工厂）** | ChildEnteredTree 钩子 + Create 后缀 | `Visible=false`（游戏）/ ProcessMode=4（自己的） | Remake 重放 or Release |

**SlowEnemies/RestoreSpeed 本体**（TimeFreeze.cs:1490-1558）：`enemy_speed=0.04`（敌人 barely breathe），遍历 `combatState.Enemies` 的 NCreature，只对 `HasSpineAnimation` 的设 `SpineAnimation.SetTimeScale(0.04)` 并记入 `Slowed`；RestoreSpeed 逐个写回 1f。补丁层再把这份名单喂给 CreatureHold 的 `BulletSpeeds` 字典——**两个冻结系统共享同一份速度账本**，避免 CreatureHold 恢复时把子弹时间设的 0.04 误写回 1。

**重入守卫（recon-shaders 交叉验证，2026-10-16）**：伤害数字与火花两条专用通道在重放时都置 `_replaying=true`（TimeFreeze.cs:80/1202/1222），监视器 OnVfxCreated 首行 `_replaying` 即退——这是「追赶压缩 replay」不会递归触发工厂监视的关键机制；没有它，解冻重放的每个伤害数字/火花都会再被冻一遍形成死循环。同理 SpawnNumber 自建 `NDamageNumVfx.Create` 也走守卫内（TimeFreeze.cs:1209）。

### 1.3 83 个 Vfx 工厂监视的完整机制（VfxFactoryPatches.cs 全文 148 行）

```
Apply(Harmony)  ← Entry.Init 单独调用（01_architecture.md:80）
 ├─ HarmonyMethod Made = 本类 "Made" 静态法
 ├─ types = typeof(NDamageNumVfx).Assembly.GetTypes()
 │    （ReflectionTypeLoadException 时取 ex.Types 非空项 :26-29）
 ├─ 过滤条件（:34，全部满足才留）：
 │    Namespace.StartsWith("MegaCrit.Sts2.Core.Nodes.Vfx")
 │    且不含 ".Forms" / ".Backgrounds" / ".Events" / ".Examples"
 │    非泛型定义、非嵌套
 │    名字不以 Skip[27] 任一前缀开头（Ordinal 比较）
 ├─ 对每个类型取 DeclaredOnly+Static+Public 方法：
 │    名字=="Create" 且非泛型方法定义
 │    且返回类型可赋值 Node
 │    且无 byref/pointer 参数（:41）
 │    → harmony.Patch(method, null, Made, null, null)  ← 纯 Postfix
 ├─ CardBurnTypes()：GameCapabilities.Resolve 找
 │    "…Nodes.Vfx.Cards.NCardExhaustVfx" + "NCardExhaustQuickVfx" 的 Create → 挂 Burned
 └─ return num  ← 日志 "83 factories watched" 的来源（01_architecture.md:16；数值口径待确认：num 含 2 个 burn 类型）
```

**Skip 27 个类型**（:6-11，Ordinal 前缀匹配）：
`NDamageNumVfx, NHitSparkVfx, NCard, NSpeechBubble, NThoughtBubble, NMonsterDeath, NDoom, NMap, NEpoch, NGainEpoch, NFailedJoin, NFullscreenText, NRelicFlash, NPotionFlash, NMirror, NRest, NRain, NLowHpBorder, NUiFlash, NSleeping, NStunned, NKinPriest, NHellraiser, NSovereignBlade, NVfxProjectileHandler, NGaseousScreenVfx, NDesaturateTransition`

读法（recon-shaders 交叉验证）：Skip 前两项 `NDamageNumVfx`/`NHitSparkVfx` **不是遗漏而是有专用通道**——火花走 HitSparkHoldPatch.cs:6-21 → `TimeFreeze.OnHitSpark`（TimeFreeze.cs:1226），伤害数字由 `TimeFreeze.SpawnNumber`（:1199-1224）自建，两路均以 `_replaying` 重入守卫防监视器拦截重放本身；`NCard` 等其余为 UI/过场/环境类（地图、epoch、休息、雨、睡眠、眩晕、低血边框……）——**声明式排除清单就是「哪些 Vfx 不该被时停」的设计文档**。

**Made 后辍的三段分工**（:103-149；出口链路经 recon-shaders 交叉验证：`Made → TimeFreeze.OnVfxCreated` 见 VfxFactoryPatches.cs:143 → TimeFreeze.cs:659，`Burned → TimeFreeze.OnCardBurn` 见 :95 → :877）：
1. `NStabVfx` 且 `FinisherFx.CoversStab() || SkewerFx.CoversStab()` → `Visible=false`（收人头/串刺要替换原生刺击，与时停无关的常态遮掩）；
2. `NShivThrowVfx` 且 `TimeFreeze.Active && ShivFan.HidesThrow(...)` → `Visible=false`（刀扇接管飞镖）；
3. `TimeFreeze.OnVfxCreated(...)`。

**OnVfxCreated（:659-705）= 冻结引擎的入口**：
- `!_active || _replaying || maker==null` 直接退（重放中不递归冻结）；
- 登记 **Recipes[node] = () => (Node)maker.Invoke(null, args)**——把「原始 MethodBase + 参数快照」录成重放配方；
- `NShivThrowVfx` → `TryFloat(args)`：args[0..1] 是 Vector2 或 Creature 时注册悬停飞刀；
- 已入树 → `Hold(node, ours:false)`；未入树 → 连 `TreeEntered` 信号延迟 Hold（等它真挂进场景）。

**Hold(node, ours)（:592-657）= 冻结原子操作**：
- `Stopped{Node, Item(=CanvasItem), Ours, At=_clock, Parent}`；`Item.Visible=false`；入 `Things`+`Known`；
- `ours=true` → `ProcessMode=4`（DISABLED，自己的特效整条停演）；
- `ours=false` → `Callable.From(...).CallDeferred()` 下一帧读 `Position/Rotation` 存进 `Place/Turn/Placed`（**新建节点当帧还没布局，必须延迟一帧采样**）；
- `ours` 由 `FromThisMod()`（:548-571）判定：`new StackTrace(2, false)` 走至多 48 帧，调用方命名空间以 "SilentFX" 开头就算自己的。

**按帧冻结/恢复**（Update :1938-2070）：
- 冻结期每帧：`Mood.Hold("bullet", …)`（音乐闪避）→ 战斗结束/无敌人则 `Thaw(fightOver:true)` → `_clock += dt`（dt 受 `Engine.TimeScale` 缩放）→ `SlowEnemies()`（幂等去重）→ 涟漪推进 → 飞刀运动 → **Burns/Things 持续压制 `Visible=false`（防游戏自己的代码又打开）** → FrozenCard 的 Card 节点保持 Visible；
- 解冻期（`_thawing`）：`_thawClock += dt`，`Due` 已按时间排序（:1467 Sort），到点执行、逐个 try-catch（:2053-2065），`Due` 清空才 `_thawing=false`。

### 1.4 追赶压缩算法（Thaw :1279-1472）——本报告最核心的公式

```
num   = max(0.02, fightOver ? bullet.replay_won(0.10) : bullet.replay(0.10))   // 追赶总预算
t0    = min(所有 Ours/Recipes 的 At、所有 Events 的 At、所有 Floaters 的 At)   // 冻结起点
t1    = max(同上)                                                              // 冻结终点
span  = t1 - t0                                                                // 冻结时长跨度
k     = span > num ? num / span : 1                                            // 时间压缩比
due(x) = 0.18 + (x - t0) * k                                                   // 通用映射（0.18s 解冻前导）
```

分项时刻表：

| 对象 | 公式 | 出处 |
|---|---|---|
| 游戏 Vfx 重放（Remake） | `0.18 + (At-t0)*k` | :1317 |
| 自己的节点（Release） | `0.18 + (At-t0)*k` | :1334 |
| 普通游戏节点（无配方） | `0`（立刻恢复可见） | :1324 |
| Events（场景/声音/数字） | `0.18 + (At-t0)*k` | :1345 |
| 悬停飞刀 Zip | `max(0.04, 0.18+(At-t0)*k - 0.06)`（比产生早 60ms 射出） | :1352 |
| 玩家 Attack/Cast 补播 | `0.09` | :1365/1375 |
| 敌人 Hit | `0.18 + span*k*0.35` | :1387 |
| 血条刷新 | `0.18 + span*k*0.5` | :1398 |
| 冰冻卡 SendOff | `0.072 + i*min(0.05, num/max(1,牌数))`（依次飞回牌堆） | :1418 |
| 死亡动画 | `num6`（全放完）+0.15 | :1429-1436 |
| 总时长 | `max(num6, 卡片结束时刻) + 0.4` | :1423/1472 |

返回值交给 FlushHookPatch.cs:20-33 的 Postfix：`__result = TimeFreeze.Linger(__result, __state)`——**把 flush 的 Task 挂住 `Cmd.Wait(seconds, false)`**。这就是 tuning.ini:238-239 注释「the whole catching-up is squeezed into this many seconds (it used to be 1.6, one thing after another)」的落地：旧版 1.6s 逐个放，新版 0.10s 一起爆。注意 Linger 只等画面，游戏逻辑该结算的早已结算完——与时停总纲一致。

### 1.5 rings[8] 涟漪与读屏滤镜

- **数据结构**：`Ripples:List<Ripple{Reach,Speed}>` + `RippleUniform:float[8]`（静态构造 :144-145）；`AddRipple(speed)` 满 8 个挤掉最旧（:536-539），只在下 `_veilMat!=null` 时入队；
- **触发点**：Begin 建立滤镜时 `AddRipple(bullet.ring_first=1.3)`（:348）；之后每打一张牌 `AddRipple(bullet.ring_speed=0.2)`（:1035）；ReduceScreenEffects 下不发涟漪（:346-349）；
- **推进**：`Reach += Speed*dt`，`>2.4` 移除；`RippleUniform[i] = i<Ripples.Count ? Reach : -1` 写入 uniform（:1992-2004）；
- **shader**（freeze.gdshader:22-46）：`rings[8]` 为各环当前半径（屏高倍数）；crest = `Σ exp(-x²)·smoothstep(0.07,0.15,at)/(1+1.6at)`（高斯环冠 + 起点隐身 + 扩散衰减）；`push = outwards*crest*ring_bend*amount` 弯折环后画面；红蓝分离 `pull=split*amount*(1+3crest)`；drain 去色 + corners 暗角；
- **centre**：`ScreenPoint(c.Chest)`（:499-530）经 canvas transform 归一化到 0..1，视口尺寸异常时兜底 (0.28, 0.58)；
- **滤镜开关**：`bullet.filter=1` 且反射 `EncounterModel.HasCustomBackground==false` 才用 shader（:449-477），自定义背景的战斗退回平涂 `bullet.tint=0.18`——**读屏特效对自定义背景的防御**。

### 1.6 悬停飞刀（Floaters）与冰冻卡（FrozenCard）

**Floaters**（:777-875）：游戏扔出 Shiv（或自家 ShivFan `FloatKnife`）时，在 `_front` 下建一个 `SilentFX_FloatingShiv`（ShivFx.MakeShiv），运动公式（Update :2006-2020）：

```
pos = From + (To-From)*(Halt*(1-exp(-Age*float_brake)) + min(Age*0.006,0.05))
      + 垂直方向*sin(Age*1.4+Phase)*3
alpha = clamp(Age/0.12, 0, 1)
```

`Halt = clamp((dist-float_gap)/dist, 0.25, 0.95)`（距敌 150px 以内开始刹停）；指数刹车 + 正弦飘摆；解冻时 `Zip`（:890-914）0.1s TRANS_BACK/EASE_OUT 补飞到目标。`Claimed` 列表做「牌打的刀 vs 游戏给的刀」去重（:828-837）。

**FrozenCard**（:981-1114）：打出的牌在 UI 层克隆 NCard，飞往「陈列架」：`x = 0.25W + (i%9)*0.5W/8`、`y = 0.19H + 0.1H*(i/9) + 16*hash`、缩放 0.42、旋转 ±0.13rad、Modulate 染冰蓝；同时 PlayArea 的 Modulate.A 打 0（出牌区隐去）。Thaw 时 `SendOff`（:1677-1748）按牌堆类型缩到 0.08 淡出飞回。卡片数极多时 `num8=min(0.05, 0.10/牌数)` 保证总时长不爆炸。

### 1.7 TimeFreezeHover：冻结世界里的「实时血条窥探」

（TimeFreezeHover.cs 全文 275 行）血条被冻结时，鼠标悬停上去反而显示**实时数值**：反射 `NHealthBar._hpLabel/_blockLabel/_blockContainer`（:113-117），悬停时把文字改写为 `Creature.CurrentHp/MaxHp/Block` 实算值，离开 Restore 写回冻结快照（FrozenHp/FrozenBlock）。MegaLabel 走 `SetTextAutoSize`（:255）。拿不到私有字段时一次性 Warn 降级为普通冻结显示（:145）。

### 1.8 联动台账（其他系统如何接入时停）

- `KeepLive(node)`（:204-211）：给自建节点打 `silentfx_live` meta，OnChild 见到就不冻——**全库 24 处调用**（HeldSession Back/Front、MementoHeld _layer、ShadowStepHeld _under/_streamRoot/_mass、EnergyBeads _top、FinisherHeld、MalaiseFx、SkewerFx、StrangleRope×2、SuppressRun×3、SuppressShow×2、TheHuntFx×2、WraithFigure、WraithFormIntroFx、WraithHeld2、WraithPlayFx）；
- `FxLifetime.HoldsVisual`（:213-238）向上走父链查 Known 里 ProcessMode==4 的自己的节点 → 暂停租约倒计时；
- `Defer(action)`（:1116-1124）：PowerFxManager:203/257、MasterPlannerFx×3、AccelerantFx 的能力图标/插画在时停中改期播放；
- `HoldMotion` 被 SkippedMotionPatch 与 ExposeSlash 等共用；
- `Thaw` 的两个定时器：FlushHookPatch（回合末）、SideTurnStartHookPatch（敌方回合开始 :12）+ Update 内的 IsEnding/EnemyLeft 兜底。

---

## ② 暗影步六阶段编排（ShadowStepFx 3037 行）

### 2.1 时间线数据结构

`Run` 类（:221-378）是单次暗影步的全部状态。阶段时长与累计时刻（Begin :758-766，参数出处 tuning.ini:483-489）：

| 阶段 | 时长键(默认) | 累计时刻 | 代码 |
|---|---|---|---|
| 抵达 arrive | `(1-Reach0)*0.2` | 0→arrive | :1512-1513 |
| 蓄 charge | `shadowstep.charge`=0.28 × Lerp(0.7,1,Reach0) | →num2 | :1513 |
| 冲/甩 round | `shadowstep.round`=1.5 | →num3 | :1514 |
| 升+悬 hang | `shadowstep.hang`=0.30 | →num4 | :1515 |
| 砸前仰 rear | `shadowstep.rear`=0.16 | →num5 | :1516 |
| 砸 drop | `shadowstep.drop`=0.15 | →num6 | :1517 |
| 顿 stop | `shadowstep.stop`=0.05 | Hold 暂停 | :1527 |
| 散 after | `shadowstep.after`=0.55 | 落地后 | :1911 |

**双时钟**：`Anim`（动画时钟，砸中那帧被 `Hold` 刹住 :1523-1529）驱动 Move 分支；`Clock`（真实时钟）驱动雾呼吸与 Dim 渐暗。`HomeAfter()`（:507-510）= 0.2+全部阶段和，供外部对齐。

### 2.2 Move() 六段分支逐段拆解（:1561-1669）

1. **抵达**（anim<arrive）：`Head=road.In(Lerp(Reach0,1,t²))`，雾团半径从待命半径涨到 `Reach(0)*0.82`；`lobe=t`（后叶张开）；
2. **蓄**（anim<num2）：`Head = Start + (-dir*58*ease + 2.5*t²*sin(anim*83), 12*ease + 2*t²*cos(anim*71))`——**83/71 rad/s 的双高频正弦颤抖**，幅度随 t² 增长；`lean=-0.22*Smooth((t-0.35)/0.65)` 前倾；`charging=Smooth(t/0.85)`（Fill 里 5 团蓄力雾 :2320-2328）；
3. **冲/甩**（anim<num3）：沿 `road.Loop(num8)` 疾走（ShadowRoad 环道）；`gather` 收束（卷到的牌越多雾越满 :1604-1606）；`Launch()` 第一次进入时触发出发冲击波 `ShockGo(kind 0)` + 起步 Puff/Motes（:2848-2864）；
4. **升**（anim<num4）：`Head = Hang + (-dir*60*(1-(1-t)²), 5*sin(πt))`，`R=hangReach*(1+0.04t)`；`reaching=0.6*Smooth(t)`（Fill 里 2 团下探雾 :2330-2338）；
5. **悬→仰**（anim<num5）：`Head=val+(dir*14,-view.Y*0.045)*ease`，`lean=-0.28*ease`；
6. **砸**（anim<num6）：`t=pow(u,2.2)`（加速下坠），`Head=Lerp(val2,val3,pow(u,2.2))`，`R=Lerp(hangReach*1.12, landReach, pow(u,1.4))`；到点 `Hit()`：砸地冲击波 `ShockGo(kind 1)` + `GameRefs.Shake(2)` + `Hold=stop` 顿帧；`Sfx.Lead("shadow_strike")` 提前量预播（:1518-1522）；
7. **落地后**：`R=landReach*Lerp(0.85,0.7)`、`lean=Lerp(0.7,1.6)`、`ragged=0.18`；`Land()` 触发脚下影池喷发（ShadowPoolFx.Landed erupt=1.5）与满地 Puff/Motes/Sparks；`Burst()` 贴地炸开持续 After 秒；After 且无挂雾 → Cleared。

**音画对齐细节**：`Patience`（:767）= `min(8.5, max(0.5,wait) + 总时长 + trail + 0.6 + 0.5*牌数)`——等游戏收牌的超时保险。

### 2.3 三层雾 + Veil 的分层交叉淡入

`Layers(road, u, fall)`（:1232-1258）按沿环进度 u 与下落进度 fall 在 back/hand/front 三个 ShadowCloud 间 Smooth 交叉：

- `u > Lerp(StopTime(3),StopTime(4),0.55)` → back 涨、hand 退（影子沉到角色背后）；
- `u > Lerp(StopTime(6),StopTime(7),0.5)` → front 涨、hand 归零（影子盖到角色前面）；
- `fall > 0.22` → back 涨、front 退（下坠时回到背后）；

Back/Front 缺失时自动把权重并回 Hand（:1680-1690）。`Veil` 是卷牌时蒙在牌上的薄纱：有牌在天上（CarriedAt 起 0.3s）时 `Thin(Lerp(1,0.45,快))` + 半强度 Fill（:1849-1858）。

### 2.4 影子雾渲染协议（ShadowCloud 416 行）

`Fill()`（:2180-2409）每帧 `Begin()→Add(...)×N→End(head,r,spin,mist,intensity,pulse)`：最多 48 个 puff 打包进**一个 quad**，6 个 48 长数组 uniform（at/ax/ex/sx/run/cb）喂 shadow_cloud.gdshader。一帧的组成：

- 主体 `Add(head, BodyR*0.86, …)` + 后叶 lobe（`-heading*(r*0.42*(1+0.5*max(0,LongR-1)))`）+ 3 个公转团（角度 `spin*0.8+i*2.094+0.4*sin(mist*2.3+i*1.7)`，LongR>1.02 时按轴拉伸 + 垂直收缩 pow(LongR,-0.25)）；
- 蓄力 5 团（半径 Lerp(2.5,0.5,charging²) 内缩 + sin(π*charging) 明暗）；
- 下探 2 团、彗尾 12 段、挂雾 Wisps（≤9）、天幕 RoofNow、Burst 落地炸；
- `End` 时算所有 puff 的包围盒（`Seen()` 取视口Rect 与 _lo/_hi 交集 +40px 边距），quad 只覆盖有内容的区域（:281-315）——**性能关键：48 puff 合一 quad + 视口裁剪**。

### 2.5 拖影、速度拉伸与体积守恒

- **彗尾**（:1762-1795）：12 段采样历史 `Was(run, anim - tail*pow((j+1)/12, 1.1))`——幂律让尾端密、根端疏；半径 `past.R*Lerp(0.78,0.2,pow(t,0.9))*tail_wide`；`Drawn` 把相邻采样点连线方向拉出（clamp(Δ/(0.55R)-1, 0.02, 6)）；
- **速度拉伸**（:1721-1740）：`fast=Smooth((Speed-1200)/3600)`；`LongR=1+stretch*0.75*fast+lean`；`LongR>1.02` → `drawn=heading*(LongR-1)`、`bodyR=R*pow(LongR,-0.25)`（**拉长同时变细，保体积**）；`LongR<0.98` → 垂直拉宽、`bodyR=R*pow((1/LongR),-0.6)`；
- **历史轨迹库** `Way:List<Past>`（:1700-1720）只保留 anim-1s 以后的采样、超 64 条才压缩；`Was()` 二分查找 + Lerp 插值（:1260-1318）。

### 2.6 飞牌动态模糊（Blur :2674-2749）

- 结构：`SoftLayer`（BackBufferCopy CopyMode=2）+ 每张牌一个 `Soft` Node2D + `shadow_blur.gdshader`；
- 速度估计：`Vel = Lerp(Vel, (now-was)/da, 1-exp(-da*40))`（指数阻尼，:2695）；
- 参数：`soft = amount*3.2*Smooth((v-250)/2600)`（高斯强度）、`smear = min(30, amount*along*v*0.0065)*velDir`（方向性涂抹）；两者都低于阈值（soft<0.15 且 smear<0.6）或 alpha 太低 → 隐藏（省 GPU）；
- 门槛 `HasShader("shadow_blur")` + `ScreenReads(c)`（自定义背景不读屏，:542-588）+ `shadowstep.card_blur>0.01`。

### 2.7 冲击波环（ShockStep :2760-2846）

`shadow_shock.gdshader` 全屏 quad（BackBufferCopy）：

| 参数 | kind 0（出发） | kind 1（砸地） |
|---|---|---|
| 时长 | 0.30s | 0.46s |
| 半径 | `view.X*0.5*(1-pow(1-n,2.2))` | `view.Y*0.42*(1-pow(1-n,2.2))` |
| lie（压扁） | 0.42 | 1 |
| width | 46*(1+0.8n) | 64*(1+0.8n) |
| push | 15*num2*pow(1-n,1.3) | 30*num2*pow(1-n,1.3) |

centre 用 `GetCanvasTransform()*目标点` 换算，并用 `|canvasTransform.Scale.Y|` 校正 radius/width/push（clamp 0.05..20 防爆，:2817-2827）。

### 2.8 天幕（RoofStep :1918-2157）——v0.48 的「遮天蔽日」

tuning.ini:535-537 的意象注释：「从它绕过弃牌堆往上爬开始，走过的地方雾不散……砸下来的时候天幕就是它的尾巴，顺着来路被拽过去、从她头顶灌进地里。天幕底下的战场是它的影子。」

- **留迹**：冲环阶段（`t∈[tl,th)`）沿环道每隔 `max(max(60,total*0.6/24), roof_gap*R*(0.8+0.4hash))` 留一个 `RoofPiece`（≤24 个，:1985-2000）；
- **拽回**：砸中后 `num8 = num7 + roof_pull(5200px/s)*(t-HitAt)`，RoofPiece 沿 `Along+num8` 前移，越过 `total+top→ground` 距离即消亡（:2010-2030）；
- **下坠两段**：`num17<=0`（还在环道上）用 road.Loop 定位 + Layers 分层；`num17>0`（越过弃牌堆顶上）用 `Hang.Lerp(top, Smooth(num17/110)) + (ground-top)*比例` 走竖直段，权重按 (num26-0.22)/0.5 从 hand 切到 front（:2074-2084）；
- **下垂雾缕**：`n%3==1` 的片加第二个 RoofPuff（At 下移、Drawn 朝下，:2116-2133）；
- **影子**：`ShadeLo/ShadeHi` 取所有天幕片 X 范围 ∩ 头部 ±R*0.6，`Roofed=Smooth((hi-lo)/500)*roof`；`shadow_shade.gdshader` 双层 quad（背景 `roof_dim=0.78` / 人物 `roof_dim_who=0.45`），uniform from_x/to_x/lean/amount；lean 随风速倾斜（`roof_lean=160*Smooth(Speed/1500)`，:2154）；LayShade 有 `IsFinite` 防御（:2167）。

### 2.9 卷牌与甩牌（Cards :2461-2672）

- **Take**（:924-1002）：CardStandIn.Make 克隆真牌进 CardLayer、CardFlight.Claim 接管真卡飞行（HideCard+HideTrail）、KeepUnseen 每帧把真牌 Modulate.A 压 0；
- **卷住**：影子头部追上牌（`num7<=0`）时记 CaughtAt+Order，随后 0.085s Grab 平滑贴合到 `Was(t-Lag*e)` 的历史影迹 + Side 侧偏 + `sin(t*7+Rate)` 公转（:2657-2661）；
- **甩出**：`s>=StopAlong(3..4)*fling` 时统一记 FlingAt/Caught；之后按 `Order*pour` 差拍沿 `Bezier(From, From+Tan*dist*0.45, Pile, pow(t,1.15))` 飞进弃牌堆，缩放→0.26、旋转对齐速度方向、末段淡出；每落一张 `Slam()`（shadow_slam + pile_shake + Puff，:2866-2892）；牌少时 Gulp 补一口吞咽雾（:1219-1230）。

### 2.10 待命阶段与接棒（ShadowStepHeld 604 行）

- 拖牌距离 `pull=0.28`（屏高比例）→ `Reach` 0..1（Pulled :473-501：卡中心离牌线越远黑雾伸得越远）；
- `Dark=0.42` 战场渐暗 + `hand_dark=0.55` 其余手牌变暗（HandShade）；
- 黑雾沿 ShadowRoad 入道预演（LayBody+LayStream），`bank` 大雾团（820×700）跟手牌中心漂移（Aim 12/s 阻尼）；
- **接棒**：OnPlay 时把 `Reach0/Clock0/Dark0/Spin` 四个静态值交给 ShadowStepFx.Begin（:747-750），打出动画从待命姿态无缝起手；`HandOver()`（:153-178）立即藏起待命层。

### 2.11 ShadowRoad（500 行）——「路程↔时间」双向映射

- 10 个 stop 点 Catmull-Rom（每段 18 子分）→ 163 点折线 + 弧长表 `_length`；
- 速度键 `Speeds[10]={2,2.4,2.3,1.9,1.5,1.9,1.6,1.25,0.85,0.22}`（**冲过半程后骤降=悬停感**，物理含义待确认）→ `_uOfS[241]` 累积归一化，`Along(u)/TimeAt(s)` 二分互查；
- `reach[10]` 各站雾团半径、`HangReach=_reach[9]*View.Y`、`LandReach=land(0.15)*View.Y`；
- 布局全部由手牌实测（:108-135：手牌 X 范围/Y 均值/高度）驱动，无手牌时兜底 0.36W~0.64W。

---

## ③ MementoHeld 悼词持牌特效（3273 行）

### 3.1 四相会话（Phase 枚举 :6-12）

`Standby → Barrage → Dissolve → Over`（Over 是终态）。基类 HeldSession 提供 Time/Arm/Lift/Fade 三速阻尼（Arm 8/s、Lift 9/s、Fade 7/s，HeldSession.cs:114-116）。

### 3.2 Standby：把「本回合的死牌」环在瞄准环上

- **数据源**：`FillDeaths`（:440-477）扫 `CombatHistory.Entries` 里 `CardDiscardedEntry && Actor==玩家 && HappenedThisTurn`——悼词 Mori「本回合弃过的牌」的游戏事实直接读史册，零猜测；
- **限量**：`memento.max=12`（:499）；抽牌堆/弃牌堆/手牌分三个 Copy 桶（draw/discard/hand），`Born=Time+i*0.03` 差拍登场；
- **手牌死牌**：`Wilt`（:845-864）给真牌挂 CardDeco("memento") 萎凋装饰（Veil ShadeMist + 桌面影 + 7 Blobs 上浮 + 6 Flakes 飘落，Deaths 越多 bloom 越浓：`num2=clamp(Deaths-1,0,2)`、`num3=1+0.22*num2`，WitherTick :945-1030）；
- **差量更新**：每 0.3s 重扫，`Signature(marks)`（牌哈希+牌堆类型滚动组合，:532-544）不变就不重建（:1126-1137）；
- **环绕布局**：`FindRing`（:1226-1304）取鼠标最近的敌人 Chest 为环心；`SlotOf`（:1389-1421）椭圆角 `-π/2-2π*i/n`，超 `_ringRoom`（周长/牌宽算容量）分内外双圈；`Radii` = BoxHalf + 牌高一半 +18，clamp ring_min/max；
- **两种 Home**：牌堆位扇形展开（`pile_rise=235` 抛物线 + 每张 ±0.14rad，:1466-1476）/ 手牌真位外侧 `peel=30` 贴着（:1480-1537）；
- **出入场**：`_out`（悬停中/MP 有目标）时 `Out=MoveToward(1, dt/fly_out=0.22)`；`Glide(dt)=1-exp(-3*dt/0.18)` 指数滑向环位；tremble 抖动 `Wobble(Time*43)/Wobble(Time*57)*tremble*num6`；
- **取景框**（TickFrame :1947-2019）：4 角角标（Diamond + 4 条 Line2D：粗光边 + 细规则线）+ grounds 软影；`frame_close=0.5` 收缩、`flinch` 受击回弹（Damp 16/s）、`snap=0.1s` 闪收（`num6=Lerp(1,0.12,snap²)`）；`CornerArms` 的 RuleShare=0.23/RuleUneven=1.75/RuleMost=92 公式（:1750-1757）；
- **牌堆雾**（TickStacks :1706-1729）：抽/弃牌堆上各一团 ShadeMist，大小随张数呼吸。

### 3.3 Barrage：洗牌齐射 + 重物压轴

**Schedule**（:2081-2172）：
- 每张牌的着点 = `_strikeAt + Spot`（Spot 在瞄准框内随机偏移，若槽位在偏点击方向同侧则翻负号保证着点分散在框内，:2196-2198）；
- **heavy 选择**：与「敌人方向」点积最大的槽位（:2119-2138）——视觉上最像「正对敌人扎进去的那张」；
- **洗牌**：Fisher-Yates，种子 `= (_signature%7919+7919)%7919 + (Time*61)%997`（:2147-2154）——同一手死牌的齐射顺序每局稳定又可变；
- **差拍**：总时长 `barrage=0.08` 分摊到 count-1 个间隔（clamp 0.006..0.02），乘 `(1.3-0.6t)`（后段加速）与 hash 抖动；heavy 压轴且 `lead*1.6`、weight 1、reach 1.35。

**TickBarrage**（:2201-2306）：Coil 蓄力 → `Launch`（记 FromPos、heavy 触发取景框 snap、Flash）→ `Drive`（:2342-2389）：`t³` 缓动冲刺、旋转对齐速度向量、scale (1,1.5) 拉伸、`DriveTrail` 拖尾（`e>0.5` 才拉出）→ `Impact`：

- **普通张 Blow**（:2500-2538）：双 Flash（Ghost + Pale）+ RingPulse + `Brush.Cut` 刀痕 + `Shards` 5-6 菱形碎片（cone=0.4 锥形散射、FxKit.Drive 0.4s）+ 2 枚 Dart（±0.1~0.32rad 偏角）；
- **heavy Finale**（:2540-2632）：`Shatter` 取景框整体碎裂（12+extra 片：4 角×3（角块/横边/竖边）+ 随机片，0.5s Drive 带重力下落 `140u²`，:2978-3086）+ 双 RingPulse（270+34n / 200+26n）+ 双 Flash + 34px 大刀痕 + 10-20 碎片 + 4 Dart + 12+n 粒子 Burst + 0.1s 后追一口 Puff + Shake 3；
- **Jolt 弹簧打脸**（:2805-2880）：对目标 NCreature.Body 施加弹簧位移——`JoltSpring=1000 / JoltDamp=32 / JoltKick=950 / JoltMax=40`，每帧 `ceil(dt/0.006)` 个子步积分；`Nudge` 处理与游戏动画的写冲突（发现 Body 被游戏移动过就重算 Home，:2944-2948）；
- **每落 3 张轻震 1** + `_flinch` 取景框回弹；`Sfx.Play("memento_slam", Lerp(0.7,1.1,张数/6))` 音调随齐射规模涨。

**Strike(at)**（:2715-2755）：MementoStrikeFx 每次真实命中时调（CardFxManager → `HeldFxManager.PlayingFor(c.Card) as MementoHeld`）——把弹幕目标点挪到真实命中点（120px 内），**演出跟着真实结算走**；没开过 Brambo 就 OnPlay(null) 补开。

### 3.4 Dissolve 与收尾

- `BeginDissolve`（:3088-3112）：未命中/超时（`memento.wait=1.6`）→ 每张上浮 14px、0.22s 淡出，最多 12 口 Puff；
- `OnCardFinished`（:3148-3169）：Barrage 未 struck → Dissolve；Standby → Leave；
- Remote：`_ringAt=AimPosition`、环位排列不变；OnLeaving 时若还在 Standby 先溶解（:3220-3226）。

---

## ④ HeldFxManager 三态机（623 行）+ RemoteHeldFx（595 行）

### 4.1 三个容器

- `Live:List<HeldSession>`：全部活会话，Update 里逐个 `Step(dt)`（:607-624）；
- `Waiting:List<HeldSession>`：打出后 `Release()` 的会话，`ReleasedFor>6s` 回收（:599-606）；
- `Playing:List<HeldSession>`：`Claim` 后进入打出阶段的会话；
- 另有 `_held`（当前持卡会话）与 `_hover`（悬停会话）两个单例槽。

### 4.2 完整状态迁移图

```
[Hover 轮询] PollHover(realDt)                     HeldHooks.Hovered() 读悬停卡
   dwell≥0.12s 且 HoverFor(card) 且 CanPlay        （_hoverRefused 每卡只试一次）
        │
        ▼
   StartSession(hovering:true) ──→ _hover/_hoverCard（不在 Live? 在 Live，Step 驱动）
        │
[拿起] Begin(card)                                  Carried=card
   ├─ _hover 命中同卡 ──→ 直接提拔为 _held（"picked up X (from hover)"，:172-186）零重启
   └─ 否则 StartSession → _held（:212-223）
        │
[松手未打出] End(played:false)
   ├─ heldFx.HoverFor(card) ──→ Armed=false, Hovering=true, SetTarget(null) → _hover（回到悬停态）
   └─ 否则 Leave() 淡出
        │
[打出] End(played:true) ──→ held.Release() ──→ Waiting.Add（:279-283）
        │
[卡牌特效认领] Claim(card)（CardFxManager.cs:267 调）
   ├─ Waiting 里按卡找到 ──→ Playing.Add（:413-426）
   ├─ _held 命中 ──→ Release() ──→ Playing.Add（:427-436）
   └─ _hover 命中且 ShivFan.Active ──→ Release() ──→ Playing.Add（:437-446）
        │
[打出阶段结束] OnCardFinished(card)（CardFinishedHookPatch 调）
   └─ Playing 匹配项 Remove + session.OnCardFinished()（:470-489）
```

**Armed 判定**（Update :586-598）：`_held.Armed = _heldFx.AimedFor(card) ? C.HasTarget : overPlayArea()`——瞄准型卡要有目标才武装，其他卡拖到出牌区上方才武装（MementoHeld 的 `_out`、ShadowStepHeld 的 Pulled 都吃这个）。

**时停协同**：`dt = realDt*clamp(Engine.TimeScale,0,4)`（:585）——持牌待命特效在子弹时间里同样减速，不会从凝固的画面里凸出来。

### 4.3 HeldSession 基类合同（241 行）

Start（建 Back/Front 双根 + KeepLive + Build + Step(0)）→ Step（Time+=dt；Arm/Lift/Fade 三阻尼；Tick(dt)；Fade<0.03 自动 End）→ Release（OnRelease）→ Play（OnPlay）→ Leave(rate)（OnLeaving，rate 取 max 可叠加）→ End（OnEnd + QueueFree 双根）。SetTarget 整包拷贝目标字段后回调 OnTargetChanged（:212-240）。

### 4.4 RemoteHeldFx：联机下端游持卡渲染

- **双数据源**：`AimSync.Exact`（主机权威快照，3s 新鲜度）优先；否则反射 `PeerInputSynchronizer.GetStateForPlayer(netId)` + `isTargeting` 字段 + `HoveredModelTracker.GetHoveredModel(netId)` + `NRemoteMouseCursorContainer.GetCursorPosition(netId)`，再用 Hitbox.HasPoint 反算目标（Target() :420-442）；任一环节失败 `_readUnavailable` 一次性降级（:31-58）；
- **Supports 白名单**（:60-76）：Finisher/Flechettes/KnifeTrap/DeadlyPoison/PoisonedStab/Snakebite/Shiv/LeadingStrike/Pinpoint/MementoMori/Skewer/Strangle/Assassinate/Suppress/Outbreak/BulletTime/CorrosiveWave 17 张 + BladeDance/CloakAndDagger/HiddenDaggers/UpMySleeve/BladeOfInk/StormOfSteel 飞镖系；
- **UsesPlayZone**（:78-87）：飞镖系/Outbreak/BulletTime/CorrosiveWave 用「是否拖到出牌区」判定 armed，其余用 HasTarget；
- **Claim**（:479-565）：远端卡进入打出阶段时接管会话——对 DeadlyPoison/PoisonedStab/Snakebite/Backstab 直接不接（那些要本地命中帧驱动）， ShivFan 类也不接；接了的登记 `Playing[card]`，`PlayingFor` 供卡牌特效查询；
- Update 每帧跑：清 `Seen` → 逐玩家重建 Peer.Held → 回收下线 peer → Owned 里 Finished 的退账。

---

## ⑤ 动画技法编目（公式 + 参数 + 出处）

| # | 技法 | 公式/参数 | 出处 |
|---|---|---|---|
| 1 | 手写 smoothstep | `Smooth(x)=clamp(x,0,1)²*(3-2*clamp)` | ShadowStepFx.cs:522-526 |
| 2 | 指数阻尼 glide | `1-exp(-k*dt/max(0.02,t))`，k=3 | MementoHeld.cs:1151-1154 |
| 3 | 三速状态阻尼 | Arm 8/s、Lift 9/s、Fade 7/s（离开时 leaveRate） | HeldSession.cs:114-116 |
| 4 | 速度→拉伸体积守恒 | `LongR=1+stretch*0.75*Smooth((v-1200)/3600)+lean`；`bodyR=R*LongR^-0.25`（压扁时 `R*(1/LongR)^-0.6`） | ShadowStepFx.cs:1721-1740 |
| 5 | 幂律彗尾采样 | `Was(anim-tail*((j+1)/12)^1.1)`；半径 `Lerp(0.78,0.2,pow(t,0.9))*tail_wide` | ShadowStepFx.cs:1762-1795 |
| 6 | 历史轨迹库 | 只留 anim-1s，>64 条才 RemoveRange；二分+Lerp 查值 | ShadowStepFx.cs:1260-1318, 1700-1720 |
| 7 | 高速颤抖叠加 | `head += 2.5*t²*sin(anim*83)`、`2*t²*cos(anim*71)` | ShadowStepFx.cs:1581 |
| 8 | Catmull-Rom 环道 | 10 站 ×18 子分=163 点；`0.5*(2p1+(-p0+p2)t+(2p0-5p1+4p2-p3)t²+(-p0+3p1-3p2+p3)t³)` | ShadowRoad.cs:283-296 |
| 9 | 路程↔时间双向映射 | `Speeds[10]` 键 → `_uOfS[241]` 累积归一化 → Along/TimeAt 二分 | ShadowRoad.cs:304-324, 398-424 |
| 10 | 追赶压缩映射 | `due=0.18+(At-t0)*k`，`k=min(1, 0.10/span)` | TimeFreeze.cs:1279-1345 |
| 11 | 涟漪 crest | `Σ exp(-x²)*smoothstep(0.07,0.15,at)/(1+1.6*at)`，x=(far-at)/0.05 | freeze.gdshader:34-41 |
| 12 | 指数刹车悬停 | `Halt*(1-exp(-Age*float_brake))+min(Age*0.006,0.05)` + `sin(Age*1.4+Phase)*3` 飘摆 | TimeFreeze.cs:2012-2017 |
| 13 | 弹簧积分 | `v+=(-k*x-d*v)*h; x+=v*h`，k=1000/d=32/kick=950/max=40，h=6ms 子步 | MementoHeld.cs:2862-2878 |
| 14 | 双频伪噪声 | `Wobble(x)=0.65sin(x)+0.35sin(2.31x+1.3)` | MementoHeld.cs:408-411 |
| 15 | 稳定哈希散布 | `FxKit.Hash(i*7+3)` 等（同 seed 同结果，跨帧稳定） | 全库 |
| 16 | t³ 冲刺 + 速度对齐旋转 | `pos=Lerp(From,To,t³)`；`rot=LerpAngle(From, vel.Angle()+π/2, min(1,t*4))` | MementoHeld.cs:2375-2376 |
| 17 | 椭圆环绕布局 | `角=-π/2-2π*i/n`；超容量分内外圈（奇数错开 π/n） | MementoHeld.cs:1404-1419 |
| 18 | 取景框角标 | `arms=(min(w*0.23,h*0.23)*1.75, 同) clamp 92` | MementoHeld.cs:1750-1757 |
| 19 | 读屏滤镜红蓝分离 | `pull=split*amount*(1+3*crest)` 分别采 R/B | freeze.gdshader:48-52 |
| 20 | canvas transform 反算 | 冲击波/aura 用 `GetCanvasTransform()` 把世界参数换算并 clamp 0.05..20 防爆 | ShadowStepFx.cs:2817-2827; ShadowCloud.cs:345-359 |
| 21 | 包围盒裁剪 | 48 puff 合一 quad，quad = (puff 包围盒 ∩ 视口) +40px | ShadowCloud.cs:281-315 |
| 22 | 差拍齐射 | 总时长/(n-1) clamp[0.006,0.02] × (1.3-0.6t) × hash | MementoHeld.cs:2156-2163 |
| 23 | 签名差量更新 | `Signature=Σ(牌哈希*7+牌堆)` 滚动哈希，不变不重建 | MementoHeld.cs:532-544, 1126-1137 |
| 24 | 提前量预播 | `Sfx.Lead("shadow_strike")` 在砸落前 Lead 秒发音 | ShadowStepFx.cs:1518-1522 |
| 25 | 重物后坐 | 着点选「与来袭方向点积最大」的槽 | MementoHeld.cs:2119-2138 |

---

## ⑥ 「注释等效物」编目（字符串字面量与日志文案）

作者几乎不写行注释，**命名与日志即注释**：

**节点名（结构自文档）**：`SilentFX_ShadowRun / ShadowRollCards / ShadowRollSoft / ShadowRollDim / ShadowRunTop / ShadowRoofShade / ShadowShock / ShadowStandbyUnder / ShadowStream / ShadowMass`（ShadowStepFx/ShadowStepHeld）、`SilentFX_Memento / MementoUnder / MementoCards / MementoFrame / MementoStreak / MementoWither / MementoSlivers / MementoShards`（MementoHeld）、`SilentFX_FreezeVeil / FrozenCard / FloatingShiv`（TimeFreeze）、`SilentFX_ShadowPuddle`（ShadowPresence）、`SilentFX_HeldBack / HeldFront`（HeldSession）。

**日志文案（行为自述）**：
- `"Bullet Time: time stopped until the end of the turn"`（TimeFreeze.cs:398）——总纲一句话；
- `"Bullet Time: time starts again ({Due.Count} held back - of the game's: {n} effects to make again, {s} scene effects, {s} sounds; {t:0.0}s)"`（:1469）——**自带统计的自述日志**，把追赶爆了什么全打出来；
- `"Bullet Time: the game made {type} ({DeclaringType}.Create, {n} arguments)"`（:669）——83 工厂的运行时可见性；
- `"Bullet Time: a blade hangs in the air (#{i}, {px:0} px to go)"`（:872）；
- `"Bullet Time: the mod \"Bullet Time: As Depicted\" is also installed and stops time itself. SilentFX leaves the freeze to it (only the clock and the flash are shown). Turn that mod off to see SilentFX's own freeze."`（:428）——**把让位决策和用户操作指引写进 Warn**；
- `"Bullet Time: froz {card} (slot {i})"`（:1107）；
- `"Bullet Time hover: native health-bar labels are unavailable; keeping the normal frozen display"`（TimeFreezeHover.cs:145）——降级说明即文档；
- `"Held FX: picked up {card} (from hover)"`（HeldFxManager.cs:183）/ `"hovering over {card}"`（:390）；
- `"Shadow Step failed to start: …"`（ShadowStepFx.cs:442）/ `"Shadow Step (take a card, undo): …"`（:997）——**回滚路径双段式日志**（undo 单独一类）；
- `"Held effect failed and was removed: …"`（HeldFxManager.cs:616）——异常=移除的容错合同；
- `"VfxFactoryPatches: could not watch {type}.Create"`（VfxFactoryPatches.cs:50）。

**变量名即语义**：`Rolled`（被卷的牌）、`Past`（历史采样）、`TailPuff/RoofPiece/RoofPuff`（雾的结构件）、`Look`（雾的每帧长相）、`Stopped`（被冻的节点）、`FrozenCard/Floater/Ripple`（时停三件套）、`Wither`（萎凋装饰）、`Peek`（血条窥探）、`Patience/StrikeHeard/DoneFor`（保险字段）。

**tuning.ini 中文注释本身**（:472-578 shadowstep 70+ 键、:221-265 bullet 24 键）是作者留给我们最完整的「设计文档」，关键句已并入上文各节。

---

## ⑦ 创意点 Top10

1. **追赶压缩（0.10s）**：时停不跳过任何动画，而是把整回合的反馈按发生顺序攒着，回合末 `0.18+(At-t0)*k` 一次性爆出——「时停不是静音，是憋大招」；旧版 1.6s 逐个放到新版 0.1s 齐放的演化写在注释里（tuning.ini:238-239）。
2. **重放配方（Recipes）**：对游戏原生 Vfx 不保存节点状态，只保存 `(MethodBase, args)`，解冻时重新调一次 `Create`——「录像重放」式冻结，零状态推断、零兼容风险（TimeFreeze.cs:677-681）。
3. **83 工厂全命名空间反射 + Skip 声明式排除**：游戏更新加了新 Vfx 自动纳入监视；27 个 Skip 前缀就是「不冻清单」的设计文档（VfxFactoryPatches.cs:6-34）。
4. **FromThisMod 栈检测**：`new StackTrace(2,false)` 走 48 帧找 SilentFX 命名空间，决定「自己的特效继续演、游戏的才冻」——时停中 SilentFX 的世界是活的（:548-571）。
5. **持牌三态机两次零重启迁移**：悬停会话直接提拔为持卡会话（from hover）；持卡会话 Release 后进 Waiting 等 CardFx Claim；回退还能降回悬 hover——三次迁移都不重建节点（HeldFxManager.cs:172-186, 279-299, 407-448）。
6. **暗影步六阶段双时钟时间线 + 待命接力**：Anim（顿帧）+ Clock（呼吸）分离；待命态 Reach0/Clock0/Dark0/Spin 四参接棒，打出从「已经被拖出半步」的姿态起手（ShadowStepFx.cs:747-750）。
7. **体积守恒拉伸**：速度把雾团拉长时半径按 `LongR^-0.25` 收缩，视觉拉伸不涨体积；配 `comb` 梳丝 + `sweep` 后掠（:1721-1740）。
8. **天幕（屋顶影子）**：影子绕过弃牌堆往上爬时「走过的地方雾不散」（RoofPiece 沿环道留迹 ≤24 片），砸落时天幕变尾巴被 `roof_pull=5200px/s` 拽回灌进地里，天幕底下战场是自己的影子（双层 shadow_shade 0.78/0.45）。
9. **取景框即道具**：悼词的瞄准取景框在 heavy 压轴时整体碎裂（Shatter 12+extra 片带重力），瞄准 UI 成为被击打的对象（MementoHeld.cs:2540-2632, 2978-3086）。
10. **弹幕洗牌 + 正点积重物 + 弹簧打脸**：死牌齐射顺序用 `signature%7919+Time*61%997` 种子洗牌；着点选与来袭方向点积最大的槽当压轴重物；命中时对敌人 Body 施加 k=1000/d=32 的弹簧位移（6ms 子步），打脸有物理回弹（:2147-2168, 2805-2880）。

**待确认清单**：
- 日志「83 factories watched」与 `Apply()` 返回值 num 的精确对应（num 含 2 个 burn 类型；Entry.cs 日志行未读）——**待确认**；
- `Speeds[10]` 速度键（冲段 2.4 突降至悬停 0.22）的取值理由——**待确认**（疑似悬停感的手调值）；
- `HoldSound` 对 `_clock<0.35` 与 `/ui/` 的豁免原因（前者疑防 freeze 建立音被扣）——**待确认**；
- Skip 里 `NVfxProjectileHandler/NGaseousScreenVfx/NDesaturateTransition` 的排除动机——**待确认**；
- MementoHeld Remote 分支（_ringRoom/双圈/aim）与 ShadowEcho 全文仅粗读——**待确认**；
- `Sfx.Lead("shadow_strike")` 的具体提前量在 Sfx.cs Slot 定义中，本轮未读——**待确认**。

---

*报告完 · deep-shadow-time · 2026-10-16*
