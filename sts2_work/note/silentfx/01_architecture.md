# SilentFX v0.72.7 架构勘察报告（初探第一轮 · recon-arch）

> 勘察对象：杀戮尖塔2 mod「刃雨毒潮SilentFX」（作者：奶酪，v0.72.7， affects_gameplay=false，为静默猎手/The Silent 角色提供视觉特效与补充音效）
> 材料：反编译切分源码 `silentfx_decomp/split/`（356 个 .cs，121,783 行，索引 `_index.json`）、mod 目录 `3817850774/`、运行日志 `silentfx.log`
> 证据格式：`split文件名:行号`（行号为切分文件内行号；源单体文件行号见各文件头部 `// source lines: X-Y`）
> 日志基线（2026-10-16 00:44:08 一次启动）：`Harmony patched methods: 80, failed classes: 0 (+ 83 factories watched)`；`AttackFeed: 7 patch(es)`；`Sfx ready: 41 sound(s)`；`PowerFxManager 26 / HeldFxManager 34 / CardFxManager 55`；`Game capabilities: Shiv creation args=3, Poison trigger=False, Concoct=False, Expertise retain FX=False`；`[MP] Exact targeting unavailable; using native peer input`

---

## ① 总体架构图（文字版）

```
SilentFX（mod 程序集，约 12.2 万行 / 356 类型）
│
├─ SilentFX（根，2 类型 / 218 行）
│  ├─ DevLog      双通道日志（GD.Print + silentfx.log，500ms 定时 flush）   SilentFX.DevLog.cs
│  └─ Entry       唯一入口 [ModInitializer("Init")]，编排全初始化           SilentFX.Entry.cs:12
│
├─ SilentFX.Core（基础设施层，45 类型 / 15,148 行）——不依赖具体卡牌，全部 static 门面
│  ├─ 帧与时钟   Ticker（ProcessFrame+FramePreDraw 双 pass 帧泵）/ FxClock（受 TimeScale 缩放的特效时钟）/ FxLifetime（租约式节点自动回收）
│  ├─ 场景锚点   GameRefs（CombatRoom/UiLayer/Overlay/EnergyCounter/PilePoint/EnergyPoint/BottomOf/FacingOf/Shake，全部 try-catch 安全访问）
│  ├─ 能力探测   GameCapabilities（反射探测游戏 API 存废）/ RuntimeApiCompatibility（签名垫片，兼容 official/beta 两种重载）
│  ├─ 配置       Tuning（tuning.ini 开发者调参）/ PlayerSettings（user_settings.ini 快照模式 + 原子写）/ HandPaceSettings（RitsuLib 设置页 schema）
│  ├─ 特效原语   FxKit（1071 行节点工厂库）/ Brush（刀痕笔刷）/ Props（多边形构件）/ Shade（墨影配色/粒子）/ ShaderLib + ShaderWarmup（gdshader 热加载与预热）
│  ├─ 音频       Sfx（2275 行，41 音槽混音器）/ GameSounds（503 行，FSB 银行拆解器）/ FmodNative（1039 行，FMOD 原生挂钩）/ Mood（音乐闪避）/ WraithAudioWarmup
│  ├─ 手牌系统   HandFlow（抽洗弃事件总线+绘制栈）/ HandPace（快/慢/关三档手牌节奏）/ HandHold（保持手牌位形）/ HandCards / HandBackend
│  ├─ MP 同步    AimCompatibility（握手能力探测+消息表登记）/ AimSync（主机权威租约协议）/ AimMessage / AimLocalTarget / MultiplayerFx / RemoteFxFade / RemoteFxFocus / RemoteShivCreation
│  └─ 其他       CardRide / CardDeco / CardStandIn / CardFlight / CreatureHold / AttackFeed / TurnLog / EchoForecast / Ticker 附属（AssetWarmup / FrameDiagnostics）
│
├─ SilentFX.Powers（能力特效层，35 类型 / 19,131 行）
│  ├─ PowerFx（抽象基类：Anchor/Amount/Start/Update/OnAmountChanged/Trigger/Stop）  SilentFX.Powers.PowerFx.cs
│  ├─ FxAnchor（挂在生物节点上的锚点：BackParent/FrontParent/Feet/Chest/Targets/Facing，全 Safe* 包装）  SilentFX.Powers.FxAnchor.cs
│  ├─ PowerFxManager（26 个能力特效注册表 + Catalog + Gate）                        SilentFX.Powers.PowerFxManager.cs:26
│  └─ 26 个 *Fx（NoxiousFumesFx / WraithFormFx / InfiniteBladesFx / BurstFx 2063 行 / AccuracyFx 784 行 / CorrosiveWaveFx 876 行 …）
│
├─ SilentFX.Cards（卡牌特效层，140 类型 / 76,190 行——全 mod 最大头）
│  ├─ CardFx（OnPlay/OnHit 抽象）→ CardFxManager（55 注册 + RemoteSafe 集合）        SilentFX.Cards.CardFxManager.cs
│  ├─ HeldFx（手中待命特效抽象）→ HeldFxManager（34 注册，Live/Waiting/Playing 三态会话）  SilentFX.Cards.HeldFxManager.cs
│  ├─ HandMoves（3041 行，抽牌/打出编排）/ GambleFlow（3639）/ MalaiseFx（3918）/ ShadowStepFx（3037）/ FinisherHeld（2452）/ TimeFreeze（2068）/ ExposeWipe（1966）/ MementoHeld（3273）/ ShivFx（2105）/ SkewerFx（2054）/ PreciseCutFx（1669）/ KnifeTrapFx（1510）/ EchoingSlashFx（1540）/ EnergyBeads（1251）/ StrangleRope（1429）/ ShivFan（1346）/ WraithFigure（1222）/ WraithPlayFx（1236）…
│  └─ 支撑：HeldSession / CardFxContext / RemoteHeldFx / Suppress* 系列 / Tour* 之外的杂项
│
├─ SilentFX.Patches（Harmony 适配层，118 类型 / 4,186 行——刻意薄）
│  └─ 每个补丁类只做「try-catch + 转发给 Core/Cards/Powers 管理器」，无业务逻辑
│
└─ SilentFX.Dev（开发内务，16 类型 / 6,910 行——另一代理负责，本报告不展开）
   └─ DevPreview / Showcase / TestBench / TourController（Tour 系统）：游戏内创作/验收工具，直接复用 Core 的 FX 目录（Catalog）
```

**数据分布**（据 `_index.json`）：Cards 76,190 行（62.6%）> Powers 19,131 > Core 15,148 > Dev 6,910 > Patches 4,186 > 根 218。
资源目录（`3817850774/`，共 49 个 .gdshader + 3 个 .gd 舞台脚本，三条独立加载链）：
- `shaders/`（26 个 .gdshader）→ **唯一由 C# ShaderLib 管理的目录**（ShaderLib.cs:12 扫 `*.gdshader`，:45 按名解析；个别 Fx 类还直查文件存在性如 PiercingWailFx.cs:390、PreciseCutFx.cs:597、ShadowStepFx.cs:594）；
- `suppress3/`（11 个 .gdshader + `suppress_stage.gd`）与 `wraith/`（12 个 .gdshader + `wraith_form.gd`/`wraith_play.gd` + rig.json/rig2.json + tex/tex2）→ **不走 ShaderLib**：由 .gd 舞台脚本自建 ShaderMaterial（FileAccess 直读文件），C# 侧仅负责编译与 Call() 驱动（SuppressStage.cs:28/273-284，WraithStage.cs:36/89-90/114-122/541）；
- `sfx/`（音源 + README + choices.ini）、`suppress/`（frames/ + suppress_frames.json，SuppressFrames.cs:65）、`tuning.ini`、`user_settings.ini`（运行时生成）。

---

## ② 启动流程（Entry.Init，SilentFX.Entry.cs:12-95）

```
DevLog.Start()                        打开 silentfx.log，起 500ms flush 定时器 + ProcessExit 兜底
 ├─ Tuning.Load()                     读 tuning.ini（缺文件即返回，不报错）
 ├─ PlayerSettings.Init()             读 user_settings.ini，发布快照
 ├─ GameCapabilities.Initialize()     反射探测并打印「Game capabilities」一行（见 ④）
 ├─ AimCompatibility.Initialize()     MP 精确瞄准可用性，不可用则 Warn 降级
 ├─ HandPace.Init()                   手牌节奏偏好；探测 RitsuLib（STS2-RitsuLib 程序集），
 │                                   未加载则挂 Ticker 探针轮询 6 秒（HandPace.cs:125-140）
 └─ if (Ticker.Init())                拿 SceneTree，挂 ProcessFrame + RenderingServer.FramePreDraw
     ├─ Tree.Root.TreeExiting += DevLog.Flush
     ├─ FxKit.Warm()                  预热 SoftDot/Bubble/Additive 三件套
     ├─ AssetWarmup.Add(...)          ×N 注册预热任务（shader/音板/语音/音效计划…）
     ├─ ShaderLib.Warm()              扫描 shaders/*.gdshader 逐个入预热队列
     ├─ Sfx.Init()                    41 音槽建档 + 语音池 + 总线预热
     └─ InitializePart(...)  ×11     每项独立 try-catch（失败只记 ERROR 不阻断）：
        PowerFxManager / CardFxManager / HandBackend / HandMoves / GambleFlow /
        PreciseCutChains / SlyGold / DevPreview / Showcase / TestBench / TourController
Harmony 装配（独立 try-catch）：
 ├─ new Harmony("sts2.silentfx")
 ├─ 反射扫本程序集全部类型，HasHarmonyAttribute 的逐个 CreateClassProcessor().Patch()
 │    ← 单类失败仅计数并记 ERROR，继续（这就是「failed classes: 0」的统计来源）
 ├─ VfxFactoryPatches.Apply(Harmony)  额外监视 83 个游戏 Vfx 工厂 Create（子弹时间用）
 └─ if (TourController.Enabled) TourPatches.Install(Harmony)
    if (Harmony != null)        AttackFeedPatches.Install(Harmony)   ← 7 个补丁
DevLog.Info("Init finished")
```

设计要点：**初始化即降级**。任一步失败都只降级不崩溃；Ticker.Init() 失败（主循环不是 SceneTree）会跳过整个特效子系统注册，仅保留 Harmony 层。

---

## ③ Harmony 补丁全景表

补丁方法类型统计：**Prefix ~87 处、Postfix ~34 处、Finalizer 9 处、Transpiler 0 处**——全库不使用 IL 改写，纯前后缀 + 反射动态定位。
118 个补丁类型中约 100 个用静态 `[HarmonyPatch]`，约 15 个用 `Prepare()+TargetMethod()/TargetMethods()` 动态定位（能力缺失时自动跳过），5 个是手动安装器/辅助类（AttackFeedPatches、TourPatches、VfxFactoryPatches、ExposeHooks、StatusStepHooks、HeldHooks、StrangleSeen、AfterimageRemoteSeen、KnifeTrapSource）。

> ⚠️ **给后续代理的警告（切分伪影）**：split 文件的 `[HarmonyPatch]` 属性位于文件尾部，且属于**源码顺序上的下一个类型**（本报告已用 Prefix/Postfix 签名逐条校验归属）。直接按「文件内属性=本文件类」解读会错位一类。

| 分组 | 数量 | 典型目标方法 | 代表证据（split文件:行号） |
|---|---|---|---|
| **A. MP 瞄准同步** | 10 | `MessageTypes.Initialize`；`PeerVersionInfo.LocalDefault`；`NetClientGameService.HandshakeSucceeded`；Host/Client 构造器；`NMouseCardPlay.Start`/`NControllerCardPlay.Start`；`NCardPlay.OnCreatureHover/OnCreatureUnhover/Cleanup` | SilentFX.Patches.AimMessageTypesPatch.cs:24；AimVersionPatch.cs:22（TargetMethod 动态）；AimHostHandshakePatch.cs:22；MouseAimSessionPatch.cs:24；LocalAimHoverPatch.cs:24 |
| **B. 手牌系统（抽/洗/弃/选/持/位形）** | ~25 | `Hook.ModifyHandDraw/BeforeHandDraw/AfterCardDrawn/AfterCardDiscarded/BeforeSideTurnStart/BeforeFlush`；`CardPileCmd.Draw/Shuffle/Add×2`；`CardCmd.DiscardAndDraw/AutoPlay×2`；`NPlayerHand.Add/SelectCards`；`HandPosHelper.GetPosition/GetAngle/GetScale`；`NCard.OnReturnedFromPool/OnFreedToPool`；`NMouseCardPlay/NControllerCardPlay.Start`；`NCardPlay.OnCreatureHover/Unhover/Cleanup` | HandDrawSizedPatch.cs:24；HandDrawCommandPatch.cs:24；HandShuffleCommandPatch.cs:24；HandHoldScalePatch.cs:24（HandPosHelper.GetScale）；HandSelectionPatch.cs:24；CardLetGoPatch.cs:24 |
| **C. VFX 持有与工厂监视** | ~14 | `VfxCmd.PlayOnCreatureCenter×3/PlayVfx/PlayFullScreenInCombat`；`NCreature.StartDeathAnim`；`NHealthBar.RefreshValues`；`NDamageNumVfx.Create`；`NHitSparkVfx.Create`；`NAudioManager.PlayOneShot`；83 个 `*Vfx.Create`（反射枚举）；`NCardExhaustVfx/QuickVfx.Create` | ExposeSlashPatch.cs:24；DeathHoldPatch.cs:24；SoundHoldPatch.cs:24；VfxFactoryPatches.cs:38-75（Apply 全量扫描 MegaCrit.Sts2.Core.Nodes.Vfx，Skip 27 个类型） |

> **C 组深意（recon-shaders 交叉核对坐实，2026-10-16）**：VfxFactoryPatches 的两条出口全通向 TimeFreeze——`Made`（VfxFactoryPatches.cs:103-148）→ `TimeFreeze.OnVfxCreated`（TimeFreeze.cs:659），`Burned`（:91）→ `TimeFreeze.OnCardBurn`（:877），另有 ShivFan.HidesThrow 判断（VfxFactoryPatches.cs:128，冻结期藏飞刀 Vfx）。即**子弹时间期间游戏创建的所有 Vfx 都被 TimeFreeze 拦截持有/排队**。Skip 列表前两项（NDamageNumVfx/NHitSparkVfx）不是遗漏而是有专用通道：HitSparkHoldPatch → TimeFreeze.OnHitSpark（TimeFreeze.cs:1226，冻结期隐藏、事后重放）与 TimeFreeze.SpawnNumber 自行重建伤害数字（:1199-1224），两条路都用 `_replaying` 重入守卫（:80/:1202/:1222）防监视器把重放本身再拦一遍。
| **D. 卡牌钩子（通用战斗事件）** | ~10 | `Hook.BeforeCardPlayed/AfterDamageGiven/BeforeFlush/AfterCardPlayed×2/BeforeSideTurnStart/AfterCardDiscarded`；`NCard.AnimMultiCardPlay`；`NCardFlyVfx._Ready`；`NCreature.SetAnimationTrigger`；`NCardFlyPowerVfx._Ready` | CardPlayedHookPatch.cs:24；DamageGivenHookPatch.cs:24；StormDiscardPatch.cs:24；SkippedMotionPatch.cs:24 |
| **E. 能力触发（26 个 Power 方法）** | ~24 | `ThornsPower.BeforeDamageReceived`；`MasterPlannerPower/SneakyPower.AfterCardPlayed`；`ToolsOfTheTradePower.AfterPlayerTurnStart`；`BurstPower.AfterModifyingCardPlayCount`；`NoxiousFumesPower/WraithFormPower.AfterSideTurnStart`；`InfiniteBladesPower.BeforeHandDraw`；`AfterimagePower.BeforeCardPlayed/AfterCardPlayed`；`EnvenomPower.AfterDamageGiven`；`BlurPower.AfterPreventingBlockClear`；`StranglePower.BeforeCardPlayed/AfterCardPlayed`；`SpeedsterPower.AfterCardDrawn`；`BlockNextTurnPower.AfterBlockCleared`；`IntangiblePower.AfterModifyingHpLostAfterOsty`；`CorrosiveWavePower.AfterCardDrawn`；`PoisonPower.Trigger`(动态)；`ConcoctPower.AfterDamageGiven`(动态) | ThornsTriggerPatch.cs:24；AfterimageTriggerPatch.cs:24；EnvenomTriggerPatch.cs:22（尾部属性即自身）；PoisonTriggerPatch.cs:22（Prepare 门控） |
| **F. 飞镖/Shiv** | 5 | `NShivThrowVfx.Create×2（不同重载）`；`NCreature.SetAnimationTrigger`；`Shiv.CreateInHand`（动态，探测到 3 参签名）；`NCombatRoom._ExitTree` | ShivThrowVfxPatch.cs:24；ShivCreatedPatch.cs:22（Prepare: GameCapabilities.ShivCreateInHand != null） |
| **G. 状态条（Power UI）** | 7 | `NPower` 的 refresh/changed/flash 三法（StatusStepHooks 反射查找，动态）；`Hook.AfterCombatEnd`；`NCombatRoom._ExitTree` | StatusAmountHoldPatch.cs:22；StatusStepHooks.cs:82 |
| **H. 通用战斗/清理** | ~10 | `Creature.ApplyPowerInternal/RemovePowerInternal`；`PowerModel.SetAmount`；`NCombatRoom._ExitTree×3`；`CombatRoom.AddExtraReward`；`CombatHistory.BlockGained`；`TimeFreeze.SlowEnemies/RestoreSpeed`；`Hook.AfterCombatEnd`；`CardModel.OnPlayWrapper`；`CardCmd.AutoPlay`（KnifeTrap 作用域） | PowerAppliedPatch.cs:24；MultiplayerCardScopePatch.cs:24；KnifeTrapAutoPlayScopePatch.cs:24 |
| **I. Dev Tour（手动安装）** | 8 | TourPatches.Install 动态扫描 + TourDamage/SetHp/Kill/Arrow/HandLock/EndTurnLock/BeforeDraw | TourPatches.cs:35-85（Install + VerifyInstalled 双保险） |
| **J. AttackFeed（手动安装）** | 7 | `AttackCommand.Execute/Count/Damage/Landed/End/Reset` 系列 | AttackFeedPatches.cs:147-198（Install，Prefix+Postfix+Finalizer 三件套） |

**补丁编写范式**（以 HandShuffleCommandPatch 为样本，HandShuffleCommandPatch.cs:24）：
- `Prefix(out State __state)` 开作用域 → `Postfix(ref Task __result, State __state)` → `Finalizer(Exception __exception, State __state)` 保证异常也收尾；
- 每个方法体第一行 try、catch 里 `DevLog.Error/Warn("类名: " + ex)`——**补丁永不通向游戏崩溃**；
- Prefix 可通过 `return false` + 写 `__result` 覆盖原方法（如 HandHoldScalePatch.cs:6-22 在手牌保持状态下把 `GetScale` 结果压成 0）。

---

## ④ 各管理器职责与关键设计

### 4.1 帧泵与时间
- **Ticker**（SilentFX.Core.Ticker.cs:24-42）：`Engine.GetMainLoop()` 取 SceneTree，挂 `ProcessFrame`（主 pass）与 `RenderingServer.FramePreDraw`（Late pass，累计 dt 封顶 0.25s）。订阅用 `HashSet` 去重 + `List` 保序，倒序 dispatch，ArrayPool 租数组快照，**单回调异常=记日志+自动移除**（Ticker.cs:133-137）。dt 计算考虑 `Engine.TimeScale`（子弹时间时用真实墙上时间）。
- **FxClock**（SilentFX.Core.FxClock.cs:15-29）：静态构造即 `Ticker.Add(Update)`，按 TimeScale 累积毫秒——全库特效动画的统一时钟。
- **FxLifetime**（SilentFX.Core.FxLifetime.cs:21-47）：`Watch(node, seconds, cleanup)` 租约；到期/离树/换房间即 `QueueFree`；`TimeFreeze.HoldsVisual(node)` 的节点暂停计时（与子弹时间联动）。

### 4.2 游戏能力探测 + 优雅降级（日志「Game capabilities」行的由来）
**GameCapabilities**（SilentFX.Core.GameCapabilities.cs）：
- `Resolve/Card/Power`：按全名反射取类型（缓存 + lock）；
- `FindShivCreateInHand()`（:82-106）：先按 4 参 `(Player,int,ICombatState,Player)` 找 `Shiv.CreateInHand`，找不到再按 3 参找——**用签名探测游戏版本差异**，日志 `Shiv creation args=3` 即此结果；
- `DeclaredMethod(Power("PoisonPower"),"Trigger")` / `ConcoctPower.AfterDamageGiven`（:20-22）：方法存在性探测（存在重名冲突时返回 null 放弃）；
- `DetectExpertiseRetention()`（:130-146）：**IL 级探测**——取 `Expertise.OnPlay` 异步状态机 MoveNext 的 IL 字节码，逐 opcode 扫描是否调用 `CardCmd.ApplySingleTurnRetain`（:148-232，手写 IL 反汇编器）；失败记 `_expertiseProbeFailure` 并在日志 Warn 说明原因。本机结果 `Expertise retain FX=False`。
- 打印时机：Initialize() 一次性汇报（:68-80）。

**RuntimeApiCompatibility**（SilentFX.Core.RuntimeApiCompatibility.cs）：对 `Hook.ModifyDamage`、`CreatureCmd.LoseBlock`、`CardSelectCmd.PushSelector` 三个「游戏版本间签名变过」的 API，用委托声明 official/beta 两套签名，启动时按参数个数择一创建委托（:162-181），调用方统一走 `PreviewDamage/LoseBlock/PushSelector` 包装，两套签名都找不到就抛 `MissingMethodException`。

**补丁级降级**：所有 `Prepare()` 返回 false 的补丁类整体跳过（如 PoisonTriggerPatch、ShivCreatedPatch、Expose*Hold、Aim* 四件套、Status*Hold）。

**Init 隔离**：Entry.InitializePart（SilentFX.Entry.cs:97-107）每项独立 try-catch。

### 4.3 配置双层
- **Tuning**（SilentFX.Core.Tuning.cs）：`tuning.ini` 键值（`intro.scale`、`wraith.fog_rest`、`hand.fast_scale`、`mp.remote_fx`、`perf.*` 等），大小写不敏感， float/int 双解析，热修靠 F8（DevPreview 重载）。
- **PlayerSettings**（SilentFX.Core.PlayerSettings.cs）：`user_settings.ini`，不可变 Snapshot + `Volatile.Read` 无锁读、`Changed` 事件广播（逐个调用并捕获监听器异常 :471-490）；保存走 `.tmp` + `File.Move` 原子替换并保留未知键（:407-469）；`ResetPageDefaults()` 提供「恢复默认」。默认值表见 :209-234。
- **HandPaceSettings**（SilentFX.Core.HandPaceSettings.cs:12+）：以字典形式声明 RitsuLib 设置页 schema（4 个 section：general/sound/visual/performance，中英双语标签）——无 RitsuLib 时退回 ini 文件（HandPace.cs:142-159 探测程序集）。

### 4.4 手牌系统
- **HandFlow**（SilentFX.Core.HandFlow.cs）：手牌事件总线（DrawBegins/DrawEnded/ShuffleBegins/CardDrawn/CardEnteredHand/DiscardBegins/CardDiscarded/SlyPlayBegins/PlayerTurnBegins/TurnEnding/SelectionBegan…），维护 `DrawStack`（嵌套绘制会话）+ `RecentDraws`（600ms 残留窗，:225）解决「抽牌命令已结束但卡牌节点才进手」的时序差；所有 Raise 逐个调用并 try-catch（:361-421）；`Mine(player)` 用 `LocalContext.IsMe` 过滤非本地玩家（MP 下只给自己的手牌放动画）。
- **HandPace**（SilentFX.Core.HandPace.cs）：Off/Fast/Slow 三档；Fast 时 `Scale=0.6`（tuning `hand.fast_scale`）压缩抽牌动画时长。
- **HandHold**（SilentFX.Core.HandHold.cs）：`Hold(size,seconds)` 临时撑住手牌数量（配合 RaiseSize 改 HandPosHelper）、`Line(spacing,seconds,lift)` 一字排开、`Squeeze(amount,hold)` 向聚拢、`Slot/LineSlot` 供特效查询卡位；`[ThreadStatic] _insideLinePosition` 防重入（:22-23, :149-161）。
- **HandCards/HandBackend**：HandCards 提供 Fill/FillSelected/NodeOf/Focused/EndTurnHovered 等手牌节点查询（含 `AccessTools.FieldRefAccess` 拿私有字段 :97）；HandBackend 负责 Init/Reset 编排（CardRide/CardDeco/HandHold/CardStandIn/HandFlow，SilentFX.Core.HandBackend.cs:8-25）。

### 4.5 特效注册表（Powers/Cards 同构）
- **PowerFxManager**（SilentFX.Powers.PowerFxManager.cs:26-58）：`Register<T>(name, factory, onEnemies, gate)` 四要素；`RegisterOptional` 用 GameCapabilities 探测可选能力（本机 `ConcoctPower` 缺失→日志 `Power FX unavailable in this game build: ConcoctPower`，:45, :79-95）；Catalog 同时供 Dev Showcase 列举；`RemoteOpacity` ConditionalWeakTable 记录远端淡出。
- **CardFxManager**（SilentFX.Cards.CardFxManager.cs:16-74）：`RemoteSafe` 白名单 53 个卡型——MP 下游玩者的卡只有命中白名单才播特效（:78-91 `CanShow`）；Catalog 供开发工具。
- **HeldFxManager**（SilentFX.Cards.HeldFxManager.cs）：手中待命特效，Live/Waiting/Playing 三态会话队列，`Carried/CarriedOut/PointedAt/ArmedAtAll` 状态查询 + `LetGo` 事件；悬停 dwell 计时（_hoverDwell）。
- 基类合同：PowerFx（Start/Update/OnAmountChanged/Trigger/Stop，SilentFX.Powers.PowerFx.cs）、CardFx（OnPlay/OnHit，SilentFX.Cards.CardFx.cs）、HeldFx、FxAnchor（Safe* 系列全部吞异常返回默认值，SilentFX.Powers.FxAnchor.cs）。

### 4.6 其他
- **AttackFeed**（SilentFX.Core.AttackFeed.cs）：攻击飞行追踪（Flight/ExecuteScope/DamageScope），`AsyncLocal` 承载跨 await 上下文（:49-51），主线程校验 `OnMain`（:57），Began/Aimed/Ended 事件；`AttackInfo`（16 行）携带 Card/Attacker/Random/Invocation。
- **CreatureHold**（SilentFX.Core.CreatureHold.cs）：按 token 引用计数冻结生物动画/ProcessMode（子弹时间配合），`SyncMode()` 下忽略 token（单人模式随便冻）。
- **AssetWarmup**（SilentFX.Core.AssetWarmup.cs）：FIFO 预热队列，`CanRun` 前置检查、单步 >8ms 记日志、只在非战斗且未暂停且空闲 0.5s 后跑一格（:66-154），drain 时输出汇总（`Prewarm queue drained: …`）。
- **ShaderLib/ShaderWarmup**（SilentFX.Core.ShaderLib.cs / ShaderWarmup.cs）：运行时从 `shaders/*.gdshader` 读码建 `Shader`（Reload 支持热修）；预热逐帧编译限一片。**注意边界：ShaderLib 只管理 shaders/ 一个目录；suppress3/ 与 wraith/ 的 shader 由 .gd 舞台脚本自建 ShaderMaterial，C# 只经 GDScript.SourceCode+Reload(false) 编译（失败回退 "the old apparition"，SuppressStage.cs:279-287、WraithStage.cs:114-122）与 Call() 驱动**——uniform 实际设置点在 .gd 内而非 C#（与 recon-shaders 的 02_shaders.md 结论互证）。
- **FrameDiagnostics**（SilentFX.Core.FxKit 同级，SilentFX.Core.FrameDiagnostics.cs:39-84）：`perf.trace=1` 时每 5s 输出帧率/长帧/GC/分配/Ticker 回调 Top5 CPU。
- **TurnLog / EchoForecast / CardFlight / CardRide / CardDeco / CardStandIn**：回合内出牌记录、回响刀数预测、卡牌飞行认领、坐骑（替身卡骑行）、装饰挂件（Attach/Handle/tick/left 合同）、替身卡制造。

### 4.8 类间依赖维度：TimeFreeze 是横向枢纽（recon-shaders 触发，已复核；deep-fx-core 认领 #13 前扩为全 API 面）

全库 43 个 .cs 引用 `TimeFreeze.*`。其 19 个公开成员的**外部调用点共 67 处**，按语义分五族：

**① 长动画保活族 KeepLive（23 处）**——冻结期向 TimeFreeze 报到续命：
EnergyBeads:1055 / FinisherHeld:520 / HeldSession:62,63 / MalaiseFx:1170 / MementoHeld:570 / ShadowStepHeld:218,246,256 / SkewerFx:570 / StrangleRope:664,681 / SuppressRun:166,167,227 / SuppressShow:308,309 / TheHuntFx:639,654 / WraithFigure:360 / WraithFormIntroFx:440 / WraithHeld2:205 / WraithPlayFx:584

**② 状态查询族 Active（17 处）**——按冻结状态分支：
BulletTimeHeld:46 / ExposeWipe:318 / HandTrickWipe:218 / MalaiseFx:1581 / PreciseCutFx:797 / ShivFan:674,860 / StatusSteps:190,339 / StrangleRope:415 / TimeFreezeHover:125,189 / Sfx:1257,2242 / BiteCoverPatch:37 / VfxFactoryPatches:128 / MasterPlannerFx:1139

**③ 推迟/解冻族 Defer（7）+Thaw（2）+Linger（1）**——排队到解冻后执行：
Defer：Sfx:1238（延迟播音）/ AccelerantFx:372 / MasterPlannerFx:416,436,460 / PowerFxManager:203,257（能力触发）
Thaw：FlushHookPatch:11 / SideTurnStartHookPatch:12；Linger：FlushHookPatch:26

**④ 计流暂停查询 HoldsVisual（2 处，internal）**——租约/节点计时暂停判据：
FxLifetime:104 / EchoingSlashFx:1147

**⑤ 持有钩子族（每个 Hold*/Skip*/On* 恰好 1 个调用点，且全部落在 Patches 层——即第③节 C/D/F 组补丁就是 TimeFreeze 公开 API 的门面）**：
Begin←BulletTimeFx:101（子弹时间卡自身发起）；OnVfxCreated←VfxFactoryPatches:143；OnCardBurn←VfxFactoryPatches:95；OnCardPlayed←CardPlayedHookPatch:11；SkipSceneVfx←SceneVfxHoldPatch:15；SkipFullScreenVfx←FullScreenVfxHoldPatch:14；HoldSound←SoundHoldPatch:14；HoldMotion←SkippedMotionPatch:37；HoldDeath←DeathHoldPatch:14；HoldBar←HealthBarHoldPatch:14；OnDamageNumber←DamageNumberHoldPatch:15；OnHitSpark←HitSparkHoldPatch:14；FloatKnife←ShivFan:697,889；Reset←ShivResetPatch:10。

**架构结论**：「子弹时间」不是一张卡的特效而是全 mod 的时间维度基础设施——补丁层（Harmony）只负责把游戏事件翻译成 TimeFreeze 的 Hold*/On* 调用，特效层通过 KeepLive 保活、HoldsVisual 让租约暂停、Defer 让音效/触发排队。二探任何卡牌 FX 线都应先查该文件是否在上述清单内。

---

## ⑤ MP（多人）特效同步方案

整体是「**能力协商 → 主机权威租约 → 20Hz 快照 → 本地渲染代理**」四层，全部在 `AimCompatibility/AimSync/AimMessage/AimLocalTarget/MultiplayerFx/RemoteFxFade/RemoteFxFocus/RemoteShivCreation` 八文件中。

**第 1 层 · 能力协商（AimCompatibility.cs）**
- `Capabilities.Detect()`（:79-131）反射校验游戏握手元数据（PeerVersionInfo 的 version/branch/idDatabaseHash/gameplayAffectingMods/otherMods 字段、HandshakeManager 类型、LocalDefault 静态法、Host/Client 构造器等 15 项），任缺即 `ExactAvailable=false`——本机日志 `game handshake version metadata is absent` 即死于此，整体降级为游戏原生 peer input。
- **消息表登记（NormalizeTypes，:240-330）**：游戏 `MessageTypes` 的 `_cache._idToType/_typeToId` 是私有的，mod 直接反射把自定义 `AimMessage` 追加进类型表尾（要求表未满 <255、ID 不冲突，还会防「合并 distinct message IDs」），失败仅 Warn 不改表。
- **广告与验签（:332-504）**：通过 `PeerVersionInfo.LocalDefault` 的 Postfix（AimVersionPatch）在 `otherMods` 里插标记 `SilentFX.Targeting/4:<fingerprint>`；fingerprint 是整个消息类型表（L|行 + M|行）的 SHA256（:506-538）。双方标记一致 + version/branch/idDatabaseHash/gameplayMods 全部相同才认为「对方也装了同协议版本的 SilentFX」（`Matches`，:467-504）。 Host 侧还校验每个 peer 的 `readyForBroadcasting` 与版本（PeerMatches/TryGetPeers）。

**第 2 层 · 主机权威租约协议（AimSync.cs）**
- 上下文键：service + room + players 排序名单 + RNG seed + roomId + act（:159-170），任一变化即 Reset；`Roster` 为名单 SHA256 防冒名。
- 握手四消息（AimMessage.cs:6-12）：`Offer(1)` 主机每 1s 发起 round → `Ack(2)` 客户端应答 → 全员到齐主机发 `Go(3)` 并授予 `_lease = Now+3s` → `Snapshot(4)` 数据流。Epoch/Round/Generation/Sequence 全随机 nonce（:552-561）。
- `Exact` 属性（:61-71）= 能力可用 ∧ MP 激活 ∧ 已连 ∧ 租约未过期——全库 MP 特效的总开关。

**第 3 层 · 快照流（AimSync.Sample，:426-485）**
- 20Hz（`_nextSample+0.05`）读本地 `AimLocalTarget.Read()`（当前持有的卡、是否在选怪、目标 Creature、是否在出牌区），打包 `NetCombatCard + CombatId` 发送；只在字段变化或 1s 刷新时发（:469）。
- 主机中转：收到客户端 Snapshot 后校验 `ValidSnapshot`（:380-424：卡在手牌堆∧持有者存活∧发送者匹配等），改写入 `_relaySequence` 再转发其他 peer——**主机是中继+序列号权威**。
- `TryReadHeld(player, out card, out target, out inPlayZone)`（:512-539）供渲染侧查询任意玩家的持卡，带 3s 新鲜度。

**第 4 层 · 本地渲染代理**
- `AimLocalTarget`（SilentFX.Core.AimLocalTarget.cs）：本地会话跟踪（Begin/SetTarget/ClearTarget/End 由 B 组补丁驱动），`Read()` 返回不可变 Snapshot。
- `RemoteFxFade`（SilentFX.Core.RemoteFxFade.cs）：`Capture(remote, parents)` 作用域内把新建节点登记为 root，`BeforeDraw`（挂在 FramePreDraw）从 root 向下走子树（上限 16384 节点）把 CanvasItem 的 `Modulate.A *= factor`，`FramePostDraw` 统一 Restore——**远端玩家的 VFX 整树按系数淡出**，不碰节点本身。
- `RemoteFxFocus`（SilentFX.Core.RemoteFxFocus.cs）：租约跟踪每张远端卡的 Held/Out/Queued/Playing 状态，有任何远端卡正在出牌则 factor=0.5（强化淡出），空闲 300s 回收。
- `RemoteShivCreation`（SilentFX.Core.RemoteShivCreation.cs）：按 (invocation, player) 分 Batch，计算远端玩家视野里「应看到几张飞镖」的差值（ShowCloak），补偿渲染。
- `MultiplayerFx`（SilentFX.Core.MultiplayerFx.cs）：`Active` = tuning `mp.remote_fx` ≠0 ∧ 战斗中玩家数 >1；`AsyncLocal<CardModel> Source` 做异步上下文里的「当前卡」传递；`IsLocal` 走游戏 `LocalContext`。

---

## ⑥ FxKit 原语清单（SilentFX.Core.FxKit.cs，1071 行）

**共享资源**：`SoftDot`（径向渐变软点贴图，:12）、`Bubble`（气泡贴图，:76）、`Additive`（叠加混合 CanvasItemMaterial，:58）。
**节点工厂**：

| 原语 | 行号 | 用途 |
|---|---|
| `Quad(size, material)` | :142 | 带材质 ColorRect（shader 载体） |
| `Glow(color, size)` | :161 | 加色软光 Sprite |
| `Ramp(offsets,colors)` / `FadeInOut(peakAt)` | :181/:199 | 1D 渐变纹理（粒子ColorRamp/拖尾） |
| `Ribbon(width,color)` | :243 | 带宽曲线+渐变+圆角接缝的 Line2D 拖尾 |
| `Blade(length,halfWidth,light,dark,hilt,glow)` | :319 | 程序化刀刃（光晕+双面多边形+三段剑柄） |
| `Sigil(size,tint,out mat)` | :545 | shader 法阵 quad（sigil.gdshader） |
| `Crescent(from,to,bulge,width,color,segments)` | :566 | 贝塞尔弯月刀痕 |
| `Veil(color)` | :639 | 全屏罩布 |
| `Shape(points,color)` / `Star(points,outer,inner)` | :691/:707 | 多边形/星形 |
| `Burst(parent,pos,color,amount,vel,life,scale,gravity,…)` | :457 | GPU 粒子爆发（17 参数全可控，ReduceScreenEffects 自动减量 35%） |
| `Bubbles(parent,pos,color,amount,box,…)` | :721 | 盒状范围气泡粒子 |
| `RingPulse(...)` | :846 | 延迟扩散圆环（可虚线） |
| `Flash(parent,at,color,size,grow,time)` | :913 | 闪光 |
| `Beam(parent,from,to,width,color,extend,hold,fade)` | :939 | 光束（伸出-保持-消退三段） |
**行为原语**：`Drive(owner,seconds,step,trans,ease)` Tween 封装（:663）、`After(parent,delay,action)` 延时（:798）、`Damp(current,target,rate,dt)` 指数阻尼（:422）、`Bezier(a,b,c,t)`（:229）、`FreeAfter(node,seconds)`→FxLifetime（:417）、`Alive(o)` 实例有效性三查（:215）、`ViewSize()` 视口尺寸带 1920×1080 兜底（:427）、`Hash(i)` 稳定哈希（:1053）、`AccessibleColor(color,flash)` 色弱友好调整（:1059，私有）。

同层笔刷库：**Brush**（SilentFX.Core.Brush.cs）：Cut/Dart/Follow/Gust/Arc/Orb/Wave/Dust/Sparks/Motes 十个「刀痕与粒子」一句式 API。**Props**（SilentFX.Core.Props.cs）：Circle/Teardrop/Box/Line/Quill/Vial/Paw 几何构件。**Shade**（SilentFX.Core.Shade.cs）：Inked/Rimmed/Glowing 三色系 + Aura/Blob/Motes/Sparks/Puff/Partial。

---

## ⑦ 音频架构（Sfx 2275 + GameSounds 503 + FmodNative 1039 + Mood 419）

```
Sfx（混音与编排层）
 ├─ Slots[41]（SilentFX.Core.Sfx.cs:192-788）：每个音效位 = 名称 + 中文标签 + 2~6 个 Option
 │   Option（:6-65）：Sample（游戏银行内样本名）/ From-To（样本内截取区间，秒）/ Lead（提前量）/ Gain/Speed / Via（hall/low 总线）/ Soft（淡入）/ Tail（截尾）/ With（叠加层）
 │   例：murder_cut 三选一，每个 Plus 一个低沉层（:224-232）；gamble_shuffle 用 Riffle() 程序化生成洗牌节奏（:855-879）
 ├─ 玩家自选：choices.ini 记录每槽手动选择（ReadPicks/WritePicks :1105-1188），日志「0 picked by hand」
 ├- Knobs：面板数值（SetKnob 写盘），sfx.on/mood.on 直接代理 PlayerSettings（:946-985）
 ├─ 播放：Play（延迟队列+变调）/ One（随机变体防重复）/ PlayAt（扣 Lead 提前量）/ Hold（持续音）/ Hush/Stop（淡出）
 ├─ 语音池：8 个 AudioStreamPlayer（MakeVoice :1859），Bus 优先 SFX 否则 Master（:1684）
 ├─ Bed：A/B 双播放器交叉淡入淡出的环境底噪（Weave :2088-2133， murder_drone/wraith_held/outbreak_creep 等 Bed=true 槽）
 ├─ Scatter：随机 sprinkles（:1941）
 └─ 总线：hall（混响）/ low（低通）两个 Godot 总线按需创建并挂效果（Room :1734-1757）
GameSounds（游戏资产拆解层，SilentFX.Core.GameSounds.cs）
 └─ 直接打开游戏 FMOD .fsb 银行文件：Bank/Sample 索引（率/声道/偏移/长度/CRC），按 CRC 取解码 Setup，
    切出 PCM 构建 AudioStream（:98-200）；冷切计时并日志告警（>8ms / 战斗中冷切 :50-77）；
    样本被游戏改名会一次性 Warn 不再刷屏（Tell :90-96）
FmodNative（原生层，SilentFX.Core.FmodNative.cs，仅 Windows）
 ├─ DllImport 20+ 个 FMOD C API（:132-192）
 ├─ Hook()（:535-643）：VirtualProtect 改内存保护，往 fmodstudio 模块导入表写钩子，截获
 │   System::update（?update@System@Studio@FMOD，:20），从而拿到 FMOD Studio system 句柄；3 秒等不到就放弃
 ├─ Attach()（:774+）：GetCoreSystem→GetBus("bus:/master/music")→GetChannelGroup→LockChannelGroup，
 │   插入低通 EQ + 混响 DSP（按名字/参数名反射定位参数索引 :721-753），带 metering
 └─ Set(cutoff,wet)（:374-418）/ Meter()（:420-465）：供 Mood 驱动；任何一步失败即 _dead 全功能关闭
Mood（音乐闪避编排，SilentFX.Core.Mood.cs）
 └─ Lease 字典（名称→音量/音高/闷罩/房间感/释放时间），Update 里阻尼趋近目标，
    通过 FmodNative.Set 作用到音乐总线（:376-383），同时驱动游戏音乐事件的音量/音高（set_volume/set_pitch）
WraithAudioWarmup：幽魂相关音频的延迟预热调度
```

三者关系：**GameSounds 供料 → Sfx 编排 → （Mood 需要时）FmodNative 直连游戏音乐总线**。Sfx 全部走 Godot 音频（不碰 FMOD），FmodNative 只服务 Mood 的音乐处理，且可整体失败降级（日志 `FmodNative: off - …`）。

**Option 参数精确语义**（回应 recon-notes 待确认点，据 Sfx.cs:1615-1660 Start() 逐行实证）：
- `Sample`+`Bank`：取自哪个游戏 FMOD 银行的哪个样本（GameSounds.Get，:1601）；
- `From`：样本内起播点（秒，`player.Play(From)`，:1653）；`To`：样本内截止点——StopAt = 现实时钟 + (To−From)/实际播放速率（:1655），即「到点淡出」由 Ticker 驱动；
- `Gain`：音量乘子（Clamp 0~8，:1634，再乘 PlayerSettings.AudioVolume）；`Speed`：速率乘子（与每次播放的随机 pitch 合成后 Clamp 0.25~4，:1632，同时决定 To 段实际时长）；
- `Lead`：预滚提前量——PlayAt/One 用 `wait − Lead/Speed` 补偿「听觉先于画面」的对齐需求（:1208/:1272）；
- `Soft`：淡入秒数（Tween volume_linear，:1637-1643）；`Tail`：停止时的淡出尾长（voice.Out，:1657）；`Via`：总线路由（hall/low，:1639）；
- `With`：同时叠加的附属层（Plus()，每层可带自己的 After 延迟）；`After`：该层相对延迟。
- 另有两个全局节流：同名 40ms 内不重播（LastPlayed，:1447-1449）、8 语音池占尽即弃（Free()，:1825）。

### 4.7 配置层边界实证（回应 recon-notes 交叉线索，2026-10-16 补充）

对 `Tuning.F/I("字面量")` 全库提取并与 tuning.ini 键做差集（Python 校验）：

- **代码侧共 1029 个不同调参键，ini 仅文档化 652 个 → 462 个「只在代码中」的隐藏键**（默认值活在本行 `Tuning.F(key, def)` 的 def 里）。集中度：gamble 59、finisher 36、wraith 30、memento 27、precise 21、strangle 21、fumes 20、envenom 19、skewer 17、flech 16、sly 15…；ini 里 5 处 `(defaults live in the code: ...)` 注释（tuning.ini:627/638/651/663 等）只点名了其中约 30 个。
- **反向差集（约 100 个 ini 键无字面量读取点）经抽查全部有动态读取路径，非死键**：① 每音槽家族 `Sfx.Knob("sfx."+name)`（Sfx.cs:1079/1100/1195/1268/1440，含 `.every/.lead` 后缀）；② `Sfx.Knob("mood.*")`（Mood.cs:330/368/373/375）；③ `Seconds("moves.*")` 包装器（HandMoves.cs:369，字面 regex 漏统计）；④ PlayerSettings 旧版回退键 `perf.prewarm/perf.shader_warmup/sfx.on/mood.on/sfx.volume`（PlayerSettings.cs:381 的 legacyTuningKey 机制）。
- **结论**：配置面「代码即文档、ini 即精选」——ini 是给人看的调参入口，真实可调面约 1.6 倍于 ini 所列；`Tuning.F/I` 的 def 兜底（键缺失/类型不符/文件缺失均静默回退，Tuning.cs:41-57）使任何键删掉都不影响运行。二挖「配置层 vs 代码层」时以此差集表为准（全表可由本报告方法复现）。

---

## ⑧ 工程组织特点观察（Claude 开发方法论侧写）

1. **六层命名空间 = 依赖单向**：根(入口) → Core(基础设施) → Powers/Cards(特效实现) → Patches(薄适配) → Dev(内务)。Patches 4,186 行只做转发，业务全在可单测的管理器里——「Harmony 是防腐层」。
2. **反射优先、永不硬绑**：GameCapabilities（签名探测）、RuntimeApiCompatibility（双签名委托）、AimCompatibility（15 项握手元数据校验）、ExposeHooks/StatusStepHooks（按名字找方法）、VfxFactoryPatches（枚举整个 Vfx 命名空间）。游戏更新改 API 时 mod 降级而非崩溃。
3. **一切皆租约/作用域**：FxLifetime.Lease、RemoteFxFocus.Lease、AimSync 3 秒租约、CreatureHold token 引用计数、RemoteFxFade Scope（IDisposable 栈）、HandFlow DrawStack。资源生命周期显式化。
4. **AsyncLocal 承载跨 await 上下文**：MultiplayerFx.Source/Invocation、AttackFeed.Executing/Damaging——异步战斗命令流里的「当前卡/当前攻击」传递，不依赖全局可变状态。
5. **ConditionalWeakTable 挂载状态**：AimSync.Attached、MultiplayerFx.Times、PowerFxManager.RemoteOpacity、RemoteShivCreation.Frames——给游戏对象附加数据且不阻止 GC。
6. **数值防御强迫症**：几乎每个 public 入口都 `float.IsFinite` + `Math.Clamp`；颜色有 AccessibleColor；音量 Clamp(0,1)。
7. **事件广播必逐个 try-catch**：HandFlow.Raise 三重载、PlayerSettings.RaiseChanged、AttackFeed 事件——一个监听器炸不掉其他。
8. **双层配置 + 双语**：开发者 tuning.ini / 玩家 user_settings.ini 分离；RitsuLib schema 中英双语标签；代码注释中英混排（面向中文玩家社区）。
9. **可观测性内建**：DevLog 双通道、初始化每阶段一行日志、Prewarm 8ms 预算告警、FrameDiagnostics perf.trace、GameSounds 冷切告警——日志本身就是验证接口（本次架构推断大部分由 silentfx.log 交叉证实）。
10. **Dev 工具是一等公民**：Showcase/TestBench/Tour 直接消费 FX 注册表 Catalog，在游戏内验收特效；TourPatches.Install 带 VerifyInstalled 自检。
11. **零 Transpiler**：全靠 Prefix/Postfix/Finalizer + 反射；唯一的 IL 读取是 Expertise 保留效果的行为探测（且失败即关闭该特效）。
12. **切分文件尾属性伪影**（对后续勘察重要）：反编译切分导致 `[HarmonyPatch]` 落在类体之后且归属下一个类型——说明原作者代码是属性在类前的标准写法，分析时须按源码行号归属。

---

## ⑨ 【线索清单】给二探的高价值深挖点

1. **AimCompatibility.cs:240-330（NormalizeTypes）**：不依赖游戏支持就往 MessageTypes 私有缓存注册自定义网络消息的完整算法（含 ID 冲突回退策略）——MP mod 的稀缺技术，值得逐行。
2. **AimSync.cs:281-378（Receive）与 :426-485（Sample）**：主机中继 + 序列号改写 + ValidSnapshot 反作弊式校验的完整状态机。
3. **GameCapabilities.cs:148-232（CallsSingleTurnRetain）**：手写 IL opcode walker 探测游戏方法行为，全库唯一读 IL 的地方。
4. **FmodNative.cs:535-668（Hook/Write/OnUpdate）**：导入表挂钩捕获 FMOD system 的 Win32 细节（VirtualProtect、钩子链保留、3 秒超时）。
5. **GameSounds.cs:98-200（Build）**：FSB 银行解析 + CRC→解码参数 + PCM 重建 AudioStream 的完整链路（决定 mod 如何零资产复用游戏音效）。
6. **Sfx.cs:2088-2142（Weave/Pass）**：Bed 环境音 A/B 交叉淡入淡出的相位推进算法；以及 :1658-1757 总线/效果房创建。
7. **HeldFxManager.cs 全文（623 行）+ SilentFX.Cards.RemoteHeldFx.cs（595 行）**：手中待命特效三态机与远端持卡渲染的对接。
8. **SilentFX.Cards.HandMoves.cs（3041 行）**：抽牌/打出动画编排（Streak/Act/Whirl/Mix/Ghost 五种子结构），手牌系统的执行层。
9. **SilentFX.Cards.TimeFreeze.cs（2068 行）**：子弹时间枢纽——VfxFactoryPatches Made/Burned 双出口（OnVfxCreated :659 / OnCardBurn :877）、Skip 列表前两项的专用通道（OnHitSpark :1226 / SpawnNumber :1199-1224，_replaying 守卫）；**且是全 mod 时间维度基础设施：43 个文件引用 TimeFreeze，21 个直接调 KeepLive/HoldsVisual/Defer（详见 ④.8），任何卡牌 FX 勘察先查是否在册**。
10. **SilentFX.Core.RemoteFxFade.cs:185-276（BeforeDraw）**：Modulate 淡出walk 与 TopLevel/继承的处理细节、与 Ticker Late pass 的时序配合。
11. **SilentFX.Cards.GambleFlow.cs（3639 行）/ MalaiseFx.cs（3918）/ ShadowStepFx.cs（3037）/ MementoHeld.cs（3273）**：单体最大的四个特效实现，后续「特效技法」勘察的主矿脉。
12. **SilentFX.Patches.ExposeHooks.cs（174 行）与 StatusStepHooks.cs（79 行）**：按约定名反射查找游戏 UI 方法的辅助器（FindRemoval/FindIncreased/FindRefresh/FindChanged/FindFlash + Creates 多载枚举）。
13. **tuning.ini 全量键**（`3817850774/tuning.ini`）：intro.* / noxious.* / wraith.* / hand.* / mp.remote_fx / perf.*——调参面即特效行为面。**（已补充实证：代码侧实有 1029 键 vs ini 652 键，462 个隐藏键差集见 ④.7，逐键默认值+调用点全表见附录 A，共 509 调用点、41 个多site键默认值已校验一致）**
14. **SilentFX.Core.Sfx.cs:845-897（Riffle/Pop）**：用固定种子随机程序化生成「洗牌/气泡」复合音层的公式化作曲。
15. **SilentFX.Patches.TourPatches.cs:35-85**：手动安装 + VerifyInstalled 自检的补丁安装范式（与 AttackFeedPatches.Install 对照）。

**待二探确认事项**：
- 「80 patched methods」的精确构成（静态属性类 ≈100 个但部分同类多方法/部分被 Prepare 跳过，需逐类核对计数口径）；
- Dev 命名空间（DevPreview/Showcase/TestBench/Tour*）与 Core 的交互全貌（本轮仅确认入口与 Catalog 复用）；
- Powers/Cards 层各特效类的内部动画技法（本轮只读了基类与注册表）。

---

*报告完 · recon-arch · 2026-10-16*

---

## 附录 A：462 个隐藏调参键全表（recon-notes 线索触发，2026-10-16 建表）

提取方法：正则扫描全部 .cs 中 `Tuning.F/I("key", def)` 字面量，与 tuning.ini 的 `key =` 行做差集；
每行格式：`键 = 默认值 [F=float/I=int] 首个调用点`，多调用点标注 `+N more`。
共 462 键 / 509 调用点；其中 41 个键有 2~3 个调用点，已校验所有多调用点键的默认值完全一致（0 冲突）。
二探建「配置全字典」时直接复用；ini 已文档化的 652 键见 tuning.ini 本体。

```
TOTAL hidden keys: 462 | total call sites: 509


### accelerant (3)
accelerant.pulse_arrows = 2   [I]  SilentFX.Powers.AccelerantFx.cs:406
accelerant.pulse_rise = 70f   [F]  SilentFX.Powers.AccelerantFx.cs:454
accelerant.pulse_time = 0.44f   [F]  SilentFX.Powers.AccelerantFx.cs:75

### accuracy (5)
accuracy.narrow = 0.34f   [F]  SilentFX.Powers.AccuracyFx.cs:748
accuracy.slope = 10f   [F]  SilentFX.Powers.AccuracyFx.cs:749
accuracy.tight = 0.6f   [F]  SilentFX.Powers.AccuracyFx.cs:747
accuracy.tight_rate = 9f   [F]  SilentFX.Powers.AccuracyFx.cs:538
accuracy.upright = 1.3f   [F]  SilentFX.Powers.AccuracyFx.cs:747

### afterimage (5)
afterimage.copy = 1f   [F]  SilentFX.Powers.FigureCopy.cs:96
afterimage.copy_alpha = 1.15f   [F]  SilentFX.Powers.AfterimageFx.cs:244
afterimage.copy_h = 1.45f   [F]  SilentFX.Powers.AfterimageFx.cs:75
afterimage.copy_w = 2f   [F]  SilentFX.Powers.AfterimageFx.cs:75
afterimage.edge_px = 3f   [F]  SilentFX.Powers.AfterimageFx.cs:122

### blades (5)
blades.intro_across = 74f   [F]  SilentFX.Cards.Intros.cs:172
blades.intro_count = 11   [I]  SilentFX.Cards.Intros.cs:174
blades.intro_lap = 0.58f   [F]  SilentFX.Cards.Intros.cs:175
blades.intro_rush = 1.6f   [F]  SilentFX.Cards.Intros.cs:176
blades.intro_tall = 0.46f   [F]  SilentFX.Cards.Intros.cs:173

### cloak (10)
cloak.closed_scale = 0.8f   [F]  SilentFX.Cards.ShivFx.cs:1418
cloak.closed_turn = 0.3f   [F]  SilentFX.Cards.ShivFx.cs:1420
cloak.fade = 0.3f   [F]  SilentFX.Cards.ShivFx.cs:1481
cloak.hold = 0.14f   [F]  SilentFX.Cards.ShivFx.cs:1480
cloak.knife_gap = 0.11f   [F]  SilentFX.Cards.ShivFx.cs:1444
cloak.knife_hang = 0.5f   [F]  SilentFX.Cards.ShivFx.cs:1573
cloak.knife_lead = 0.05f   [F]  SilentFX.Cards.ShivFx.cs:1445
cloak.knife_time = 0.6f   [F]  SilentFX.Cards.ShivFx.cs:1571
cloak.open_time = 0.24f   [F]  SilentFX.Cards.ShivFx.cs:1479
cloak.spread = 1.4f   [F]  SilentFX.Cards.ShivFx.cs:1423

### deflect (2)
deflect.gap = 0.08f   [F]  SilentFX.Cards.DeflectFx.cs:63
deflect.reach = 2.6f   [F]  SilentFX.Cards.DeflectFx.cs:65

### envenom (19)
envenom.drop_fall = 70f   [F]  SilentFX.Powers.EnvenomFx.cs:428
envenom.drop_first = 0.3f   [F]  SilentFX.Powers.EnvenomFx.cs:371
envenom.drop_gap = 0.36f   [F]  SilentFX.Powers.EnvenomFx.cs:372
envenom.drop_life = 0.34f   [F]  SilentFX.Powers.EnvenomFx.cs:427
envenom.drop_rest = 0.7f   [F]  SilentFX.Powers.EnvenomFx.cs:78
envenom.drops = 3   [I]  SilentFX.Powers.EnvenomFx.cs:369
envenom.fling_drag = 3.2f   [F]  SilentFX.Powers.EnvenomFx.cs:236
envenom.fling_drops = 10   [I]  SilentFX.Powers.EnvenomFx.cs:220
envenom.fling_fan = 11f   [F]  SilentFX.Powers.EnvenomFx.cs:212
envenom.fling_gravity = 620f   [F]  SilentFX.Powers.EnvenomFx.cs:237
envenom.fling_life = 1f   [F]  SilentFX.Powers.EnvenomFx.cs:239
envenom.fling_lift = 10f   [F]  SilentFX.Powers.EnvenomFx.cs:149
envenom.fling_mist = 16   [I]  SilentFX.Powers.EnvenomFx.cs:215
envenom.fling_reach = 1f   [F]  SilentFX.Powers.EnvenomFx.cs:209
envenom.fling_repeat = 0.6f   [F]  SilentFX.Powers.EnvenomFx.cs:83
envenom.fling_rest = 0.45f   [F]  SilentFX.Powers.EnvenomFx.cs:83
envenom.fling_vary = 14f   [F]  SilentFX.Powers.EnvenomFx.cs:149
envenom.pop = 1f   [F]  SilentFX.Powers.EnvenomFx.cs:346
envenom.pop_amount = 9   [I]  SilentFX.Powers.EnvenomFx.cs:354

### expose (3)
expose.bare = 0.3f   [F]  SilentFX.Cards.ExposeFx.cs:401
expose.flecks = 18   [I]  SilentFX.Cards.ExposeFx.cs:419
expose.scan_time = 0.24f   [F]  SilentFX.Cards.ExposeFx.cs:400

### finisher (36)
finisher.blink = 0.42f   [F]  SilentFX.Cards.FinisherFx.cs:891
finisher.card = 0.34f   [F]  SilentFX.Cards.FinisherFx.cs:448 +1 more
finisher.card_rise = 112f   [F]  SilentFX.Cards.FinisherHeld.cs:361
finisher.dart = 0.08f   [F]  SilentFX.Cards.FinisherHeld.cs:371
finisher.dart_stretch = 0.25f   [F]  SilentFX.Cards.FinisherFx.cs:449 +1 more
finisher.edge_on = 0.3f   [F]  SilentFX.Cards.FinisherCard.cs:59
finisher.final_dart = 0.08f   [F]  SilentFX.Cards.FinisherHeld.cs:372
finisher.fly_out = 0.24f   [F]  SilentFX.Cards.FinisherHeld.cs:365
finisher.glide = 0.18f   [F]  SilentFX.Cards.FinisherHeld.cs:367
finisher.hand_alpha = 0.5f   [F]  SilentFX.Cards.FinisherHeld.cs:358
finisher.home_alpha = 0.95f   [F]  SilentFX.Cards.FinisherHeld.cs:356
finisher.home_card = 0.24f   [F]  SilentFX.Cards.FinisherHeld.cs:355
finisher.home_light = 0.86f   [F]  SilentFX.Cards.FinisherHeld.cs:357
finisher.last_scale = 1.45f   [F]  SilentFX.Cards.FinisherFx.cs:448 +1 more
finisher.lean = 0.3f   [F]  SilentFX.Cards.FinisherCard.cs:60
finisher.lock = 0.26f   [F]  SilentFX.Cards.FinisherHeld.cs:376
finisher.loose = 1f   [F]  SilentFX.Cards.FinisherFx.cs:405
finisher.mark_width = 3f   [F]  SilentFX.Cards.FinisherHeld.cs:1576
finisher.max = 9   [I]  SilentFX.Cards.FinisherFx.cs:410 +1 more
finisher.payoff = 1f   [F]  SilentFX.Cards.FinisherFx.cs:889
finisher.peel = 34f   [F]  SilentFX.Cards.FinisherHeld.cs:359
finisher.per_frame = 2   [I]  SilentFX.Cards.FinisherHeld.cs:377
finisher.preview = 6   [I]  SilentFX.Cards.FinisherHeld.cs:426
finisher.ring_gap = 22f   [F]  SilentFX.Cards.FinisherHeld.cs:362
finisher.ring_min = 150f   [F]  SilentFX.Cards.FinisherHeld.cs:363
finisher.ring_reach = 560f   [F]  SilentFX.Cards.FinisherHeld.cs:364
finisher.rise = 0.2f   [F]  SilentFX.Cards.FinisherHeld.cs:368
finisher.shake = 1f   [F]  SilentFX.Cards.FinisherFx.cs:1150
finisher.sink = 0.06f   [F]  SilentFX.Cards.FinisherHeld.cs:373
finisher.stagger = 0.03f   [F]  SilentFX.Cards.FinisherHeld.cs:366
finisher.sure = 5f   [F]  SilentFX.Cards.FinisherHeld.cs:375
finisher.tighten = 0.88f   [F]  SilentFX.Cards.FinisherHeld.cs:803
finisher.tremble = 2.2f   [F]  SilentFX.Cards.FinisherHeld.cs:369
finisher.tremble_turn = 0.03f   [F]  SilentFX.Cards.FinisherHeld.cs:370
finisher.veil = 0.28f   [F]  SilentFX.Cards.FinisherHeld.cs:828
finisher.wait = 1.8f   [F]  SilentFX.Cards.FinisherHeld.cs:2068

### flech (16)
flech.back = 0.16f   [F]  SilentFX.Cards.FlechettesOrbit.cs:326
flech.card_scale = 0.33f   [F]  SilentFX.Cards.FlechettesOrbit.cs:319
flech.dim = 0.42f   [F]  SilentFX.Cards.FlechettesOrbit.cs:320
flech.home = 0.18f   [F]  SilentFX.Cards.FlechettesOrbit.cs:323
flech.home_quick = 0.12f   [F]  SilentFX.Cards.FlechettesOrbit.cs:324
flech.layers = 1f   [F]  SilentFX.Cards.FlechettesOrbit.cs:327
flech.mark_inward = 1f   [F]  SilentFX.Cards.FlechettesOrbit.cs:328
flech.mark_len = 22f   [F]  SilentFX.Cards.FlechettesOrbit.cs:923
flech.orbit_x = 0.95f   [F]  SilentFX.Cards.FlechettesOrbit.cs:315
flech.orbit_y = 0.32f   [F]  SilentFX.Cards.FlechettesOrbit.cs:316
flech.out = 0.22f   [F]  SilentFX.Cards.FlechettesOrbit.cs:321
flech.shake = 0.035f   [F]  SilentFX.Cards.FlechettesOrbit.cs:244
flech.shot = 0.09f   [F]  SilentFX.Cards.FlechettesOrbit.cs:325
flech.spin = 3.2f   [F]  SilentFX.Cards.FlechettesOrbit.cs:317
flech.spin_play = 1.35f   [F]  SilentFX.Cards.FlechettesOrbit.cs:318
flech.stagger = 0.03f   [F]  SilentFX.Cards.FlechettesOrbit.cs:322

### footwork (5)
footwork.fade = 0.26f   [F]  SilentFX.Cards.FootworkFx.cs:123
footwork.ghost = 0.3f   [F]  SilentFX.Cards.FootworkFx.cs:180
footwork.print = 0.42f   [F]  SilentFX.Cards.FootworkFx.cs:34
footwork.step = 0.085f   [F]  SilentFX.Cards.FootworkFx.cs:33
footwork.sway = 0.7f   [F]  SilentFX.Cards.FootworkFx.cs:179

### fumes (20)
fumes.fog_clear = 0.8f   [F]  SilentFX.Powers.NoxiousFumesFx.cs:155
fumes.fog_drift = 46f   [F]  SilentFX.Cards.NoxiousCloudFx.cs:121 +1 more
fumes.fog_lead = 190f   [F]  SilentFX.Powers.NoxiousFumesFx.cs:155
fumes.fog_levels = 6f   [F]  SilentFX.Powers.NoxiousFumesFx.cs:104
fumes.fog_low = 0.6f   [F]  SilentFX.Powers.NoxiousFumesFx.cs:460
fumes.fog_max = 0.7f   [F]  SilentFX.Powers.NoxiousFumesFx.cs:461
fumes.fog_motes = 26   [I]  SilentFX.Powers.NoxiousFumesFx.cs:597
fumes.fog_past = 240f   [F]  SilentFX.Powers.NoxiousFumesFx.cs:156
fumes.fog_rise = 250f   [F]  SilentFX.Powers.NoxiousFumesFx.cs:90
fumes.fog_soft = 260f   [F]  SilentFX.Powers.NoxiousFumesFx.cs:537
fumes.fog_to_edge = 1f   [F]  SilentFX.Powers.NoxiousFumesFx.cs:157
fumes.sheet_alpha = 0.64f   [F]  SilentFX.Cards.NoxiousCloudFx.cs:119
fumes.sheet_light = 0.2f   [F]  SilentFX.Cards.NoxiousCloudFx.cs:122
fumes.sheet_mouth = 0.22f   [F]  SilentFX.Cards.NoxiousCloudFx.cs:97
fumes.sheet_rise = 0.9f   [F]  SilentFX.Cards.NoxiousCloudFx.cs:96
fumes.sheet_settle = 0.75f   [F]  SilentFX.Powers.NoxiousFumesFx.cs:83
fumes.sheet_travel = 0.8f   [F]  SilentFX.Powers.NoxiousFumesFx.cs:81
fumes.sheet_veil = 0.3f   [F]  SilentFX.Cards.NoxiousCloudFx.cs:120
fumes.surge_time = 0.8f   [F]  SilentFX.Powers.NoxiousFumesFx.cs:85
fumes.veil_max = 0.26f   [F]  SilentFX.Powers.NoxiousFumesFx.cs:462

### gamble (59)
gamble.arc = 0.22f   [F]  SilentFX.Cards.GambleFlow.cs:1370
gamble.back_stagger = 0.015f   [F]  SilentFX.Cards.GambleFlow.cs:2721
gamble.breathe = 0.1f   [F]  SilentFX.Cards.GambleHeld.cs:151
gamble.breathe_out = 0.07f   [F]  SilentFX.Cards.GambleFlow.cs:1372
gamble.breathe_px = 7f   [F]  SilentFX.Cards.GambleHeld.cs:157
gamble.copies_per_frame = 3f   [F]  SilentFX.Cards.GambleFlow.cs:2366
gamble.cross = 0.06f   [F]  SilentFX.Cards.GambleFlow.cs:1391
gamble.deck_width = 760f   [F]  SilentFX.Cards.GambleFlow.cs:567
gamble.depth_dark = 0.74f   [F]  SilentFX.Cards.GambleFlow.cs:29
gamble.depth_rise = 3f   [F]  SilentFX.Cards.GambleFlow.cs:28
gamble.depth_scale = 0.018f   [F]  SilentFX.Cards.GambleFlow.cs:27
gamble.discard_time = 0.12f   [F]  SilentFX.Cards.GambleFlow.cs:1967
gamble.dust = 1f   [F]  SilentFX.Cards.GambleFlow.cs:1382
gamble.edge = 1f   [F]  SilentFX.Cards.GambleFlow.cs:30 +1 more
gamble.edge_height = 0.965f   [F]  SilentFX.Cards.GambleFlow.cs:996
gamble.edge_round = 14f   [F]  SilentFX.Cards.GambleFlow.cs:997
gamble.edge_shadow = 0.42f   [F]  SilentFX.Cards.GambleFlow.cs:31
gamble.fan_over = 0.06f   [F]  SilentFX.Cards.GambleFlow.cs:1375
gamble.fan_stagger = 0.02f   [F]  SilentFX.Cards.GambleFlow.cs:1364
gamble.flash = 0.22f   [F]  SilentFX.Cards.GambleFlow.cs:1387
gamble.flow = 1f   [F]  SilentFX.Cards.GambleFlow.cs:1271
gamble.gather = 0.3f   [F]  SilentFX.Cards.GambleHeld.cs:162
gamble.gather_over = 0.09f   [F]  SilentFX.Cards.GambleFlow.cs:1374
gamble.ground = 0.3f   [F]  SilentFX.Cards.GambleFlow.cs:1381 +2 more
gamble.held_back = 0.28f   [F]  SilentFX.Cards.GambleFlow.cs:3222
gamble.held_close = 0.2f   [F]  SilentFX.Cards.GambleFlow.cs:3215
gamble.held_open = 0.3f   [F]  SilentFX.Cards.GambleFlow.cs:3209
gamble.held_row = 1f   [F]  SilentFX.Cards.GambleHeld.cs:126
gamble.held_turn = 1f   [F]  SilentFX.Cards.GambleFlow.cs:504
gamble.land_over = 0.9f   [F]  SilentFX.Cards.GambleFlow.cs:1376
gamble.land_squash = 0.08f   [F]  SilentFX.Cards.GambleFlow.cs:1377
gamble.lean = 0.45f   [F]  SilentFX.Cards.GambleFlow.cs:1371
gamble.pack_open = 78f   [F]  SilentFX.Cards.GambleFlow.cs:564
gamble.pack_tight = 10f   [F]  SilentFX.Cards.GambleFlow.cs:559
gamble.pick_lift = 0f   [F]  SilentFX.Cards.GambleHeld.cs:122
gamble.pile_scale = 0.25f   [F]  SilentFX.Cards.GambleFlow.cs:1369
gamble.pin = 1f   [F]  SilentFX.Cards.GambleFlow.cs:2257
gamble.play_wait = 1.2f   [F]  SilentFX.Cards.GambleHeld.cs:94
gamble.prehide = 1f   [F]  SilentFX.Cards.GambleFlow.cs:1388
gamble.queue_wait = 3f   [F]  SilentFX.Cards.GambleHeld.cs:113
gamble.reopen = 0f   [F]  SilentFX.Cards.GambleFlow.cs:1362
gamble.reshuffle = 1f   [F]  SilentFX.Cards.GambleFlow.cs:1311
gamble.ripple = 0.012f   [F]  SilentFX.Cards.GambleFlow.cs:1365 +2 more
gamble.row_lift = 0f   [F]  SilentFX.Cards.GambleFlow.cs:574
gamble.settle = 0.1f   [F]  SilentFX.Cards.GambleFlow.cs:1386
gamble.shuffle_wait = 2.5f   [F]  SilentFX.Cards.GambleFlow.cs:1324
gamble.side = 0.11f   [F]  SilentFX.Cards.GambleFlow.cs:540
gamble.skew = 10f   [F]  SilentFX.Cards.GambleFlow.cs:33
gamble.spread_over = 0.12f   [F]  SilentFX.Cards.GambleFlow.cs:1373 +1 more
gamble.streak = 1f   [F]  SilentFX.Cards.GambleFlow.cs:1389
gamble.swap_deg = 1f   [F]  SilentFX.Cards.GambleFlow.cs:1384
gamble.swap_px = 4f   [F]  SilentFX.Cards.GambleFlow.cs:1383
gamble.swap_wait = 0.5f   [F]  SilentFX.Cards.GambleFlow.cs:1385
gamble.thick = 9f   [F]  SilentFX.Cards.GambleFlow.cs:30
gamble.trail = 0.014f   [F]  SilentFX.Cards.GambleFlow.cs:1380
gamble.trail_px = 24f   [F]  SilentFX.Cards.GambleFlow.cs:1379
gamble.turn_ease = 14f   [F]  SilentFX.Cards.GambleFlow.cs:2720
gamble.turn_snap = 22f   [F]  SilentFX.Cards.GambleFlow.cs:2906
gamble.windup_px = 14f   [F]  SilentFX.Cards.GambleFlow.cs:1378

### handtrick (10)
handtrick.aside_scale = 0.62f   [F]  SilentFX.Cards.HandTrickWipe.cs:273
handtrick.aside_tilt = 0.14f   [F]  SilentFX.Cards.HandTrickWipe.cs:293
handtrick.aside_time = 0.24f   [F]  SilentFX.Cards.HandTrickWipe.cs:287
handtrick.aside_x = 0.16f   [F]  SilentFX.Cards.HandTrickWipe.cs:292
handtrick.aside_y = 0.45f   [F]  SilentFX.Cards.HandTrickWipe.cs:292
handtrick.halo = 0.34f   [F]  SilentFX.Cards.HandTrickWipe.cs:658
handtrick.leave_time = 0.36f   [F]  SilentFX.Cards.HandTrickWipe.cs:758
handtrick.streak = 0.5f   [F]  SilentFX.Cards.HandTrickWipe.cs:675
handtrick.wipe = 1f   [F]  SilentFX.Cards.HandTrickWipe.cs:214
handtrick.wipe_time = 0.62f   [F]  SilentFX.Cards.HandTrickWipe.cs:762

### malaise (1)
malaise.preview_x = 3   [I]  SilentFX.Cards.MalaiseFx.cs:863 +1 more

### memento (27)
memento.blow = 1f   [F]  SilentFX.Cards.MementoHeld.cs:2523
memento.draw_back = 46f   [F]  SilentFX.Cards.MementoHeld.cs:2110
memento.fly_out = 0.22f   [F]  SilentFX.Cards.MementoHeld.cs:422
memento.frame_close = 0.5f   [F]  SilentFX.Cards.MementoHeld.cs:1967
memento.frame_open = 1.35f   [F]  SilentFX.Cards.MementoHeld.cs:1970
memento.glide = 0.18f   [F]  SilentFX.Cards.MementoHeld.cs:424
memento.halo = 0.55f   [F]  SilentFX.Cards.MementoHeld.cs:430
memento.hand_alpha = 0.45f   [F]  SilentFX.Cards.MementoHeld.cs:429
memento.hand_mist = 1f   [F]  SilentFX.Cards.MementoHeld.cs:355
memento.jolt = 1f   [F]  SilentFX.Cards.MementoHeld.cs:2774 +1 more
memento.max = 12f   [F]  SilentFX.Cards.MementoHeld.cs:499
memento.payoff = 1f   [F]  SilentFX.Cards.MementoHeld.cs:2592
memento.peel = 30f   [F]  SilentFX.Cards.MementoHeld.cs:428
memento.pile_alpha = 0.95f   [F]  SilentFX.Cards.MementoHeld.cs:416
memento.pile_rise = 235f   [F]  SilentFX.Cards.MementoHeld.cs:417
memento.pile_scale = 0.42f   [F]  SilentFX.Cards.MementoHeld.cs:415
memento.ring_alpha = 0.95f   [F]  SilentFX.Cards.MementoHeld.cs:419
memento.ring_max = 300f   [F]  SilentFX.Cards.MementoHeld.cs:421
memento.ring_min = 150f   [F]  SilentFX.Cards.MementoHeld.cs:420
memento.ring_scale = 0.34f   [F]  SilentFX.Cards.MementoHeld.cs:418
memento.rise = 0.22f   [F]  SilentFX.Cards.MementoHeld.cs:425
memento.rule_width = 2f   [F]  SilentFX.Cards.MementoHeld.cs:1788
memento.shake = 1f   [F]  SilentFX.Cards.MementoHeld.cs:2472 +1 more
memento.stagger = 0.03f   [F]  SilentFX.Cards.MementoHeld.cs:423
memento.tremble = 3f   [F]  SilentFX.Cards.MementoHeld.cs:426
memento.tremble_turn = 0.04f   [F]  SilentFX.Cards.MementoHeld.cs:427
memento.wait = 1.6f   [F]  SilentFX.Cards.MementoHeld.cs:2210

### mirage (10)
mirage.ahead = 0.4f   [F]  SilentFX.Cards.MirageFx.cs:55
mirage.feed = 1f   [F]  SilentFX.Cards.MirageFx.cs:126
mirage.feed_time = 0.2f   [F]  SilentFX.Cards.MirageFx.cs:129
mirage.fill_time = 0.2f   [F]  SilentFX.Cards.MirageFx.cs:179
mirage.flash = 0.55f   [F]  SilentFX.Cards.MirageFx.cs:192
mirage.flash_time = 0.26f   [F]  SilentFX.Cards.MirageFx.cs:181
mirage.full_poison = 24f   [F]  SilentFX.Cards.MirageFx.cs:39
mirage.mist_peak = 0.74f   [F]  SilentFX.Cards.MirageFx.cs:191
mirage.scatter_time = 0.42f   [F]  SilentFX.Cards.MirageFx.cs:183
mirage.size = 1f   [F]  SilentFX.Cards.MirageFx.cs:50

### moves (20)
moves.acro_apart = 0.12f   [F]  SilentFX.Cards.HandMoves.cs:1045
moves.acro_apex = 260f   [F]  SilentFX.Cards.HandMoves.cs:2334
moves.acro_gap = 0.25f   [F]  SilentFX.Cards.HandMoves.cs:1043
moves.acro_hang = 0.38f   [F]  SilentFX.Cards.HandMoves.cs:1209
moves.acro_hold = 0.5f   [F]  SilentFX.Cards.HandMoves.cs:962 +1 more
moves.acro_mix = 1f   [F]  SilentFX.Cards.HandMoves.cs:999
moves.acro_rise = 520f   [F]  SilentFX.Cards.HandMoves.cs:1211
moves.acro_turns = 1f   [F]  SilentFX.Cards.HandMoves.cs:1217 +1 more
moves.backflip_hang = 0.45f   [F]  SilentFX.Cards.HandMoves.cs:911
moves.backflip_hop = 190f   [F]  SilentFX.Cards.HandMoves.cs:912
moves.backflip_snap = 1.5f   [F]  SilentFX.Cards.HandMoves.cs:913
moves.escape_ghost = 0.14f   [F]  SilentFX.Cards.HandMoves.cs:1804
moves.escape_ghosts = 4f   [F]  SilentFX.Cards.HandMoves.cs:1757
moves.escape_small = 0.6f   [F]  SilentFX.Cards.HandMoves.cs:1582
moves.escape_tail = 0.11f   [F]  SilentFX.Cards.HandMoves.cs:1738
moves.escape_tail_width = 34f   [F]  SilentFX.Cards.HandMoves.cs:1716
moves.on = 1f   [F]  SilentFX.Cards.HandMoves.cs:803 +2 more
moves.prepared_alpha = 0.55f   [F]  SilentFX.Cards.HandMoves.cs:1417
moves.prepared_prehide = 1f   [F]  SilentFX.Cards.HandMoves.cs:803
moves.prepared_rise = 14f   [F]  SilentFX.Cards.HandMoves.cs:1419

### murder (13)
murder.beat = 0.08f   [F]  SilentFX.Cards.MurderFx.cs:249
murder.claw_knock = 1.4f   [F]  SilentFX.Cards.MurderFx.cs:496
murder.claw_stop = 1f   [F]  SilentFX.Cards.MurderFx.cs:492
murder.knock = 20f   [F]  SilentFX.Cards.MurderFx.cs:271 +1 more
murder.shake = 2   [I]  SilentFX.Cards.MurderFx.cs:1196 +1 more
murder.spray = 22   [I]  SilentFX.Cards.MurderFx.cs:1182
murder.spring = 0.3f   [F]  SilentFX.Cards.MurderFx.cs:561
murder.stop_high = 45f   [F]  SilentFX.Cards.MurderFx.cs:401
murder.stop_low = 8f   [F]  SilentFX.Cards.MurderFx.cs:400
murder.stop_max = 0.22f   [F]  SilentFX.Cards.MurderFx.cs:250
murder.stop_min = 0.08f   [F]  SilentFX.Cards.MurderFx.cs:250
murder.wash = 0.85f   [F]  SilentFX.Cards.MurderFx.cs:1052
murder.whiten = 2.6f   [F]  SilentFX.Cards.MurderFx.cs:457

### pinpoint (1)
pinpoint.preview_steps = 2   [I]  SilentFX.Cards.PinpointFocus.cs:141

### pounce (3)
pounce.mark_time = 0.6f   [F]  SilentFX.Cards.PounceFx.cs:353
pounce.point = 1f   [F]  SilentFX.Cards.PounceFx.cs:139
pounce.point_time = 0.24f   [F]  SilentFX.Cards.PounceFx.cs:180

### precise (21)
precise.aim_gap = 7f   [F]  SilentFX.Cards.PreciseCutHeld.cs:344
precise.aim_line = 0.26f   [F]  SilentFX.Cards.PreciseCutHeld.cs:369
precise.aim_open = 34f   [F]  SilentFX.Cards.PreciseCutHeld.cs:343
precise.aim_thick = 1f   [F]  SilentFX.Cards.PreciseCutHeld.cs:350
precise.aim_turn = 1f   [F]  SilentFX.Cards.PreciseCutHeld.cs:145
precise.beat = 0.1f   [F]  SilentFX.Cards.PreciseCutFx.cs:428
precise.bloom = 1f   [F]  SilentFX.Cards.PreciseCutFx.cs:494
precise.card = 1f   [F]  SilentFX.Cards.PreciseCutFx.cs:452
precise.card_size = 1f   [F]  SilentFX.Cards.PreciseCutFx.cs:458 +2 more
precise.cut_length = 1f   [F]  SilentFX.Cards.PreciseCutFx.cs:271
precise.edge_depth = 13f   [F]  SilentFX.Cards.PreciseCutChains.cs:375
precise.edge_from = 44f   [F]  SilentFX.Cards.PreciseCutChains.cs:373
precise.edge_side = 230f   [F]  SilentFX.Cards.PreciseCutChains.cs:374
precise.fly = 1f   [F]  SilentFX.Cards.PreciseCutFx.cs:797
precise.gleam = 1f   [F]  SilentFX.Cards.PreciseCutChains.cs:424 +1 more
precise.gleam_every = 2.2f   [F]  SilentFX.Cards.PreciseCutChains.cs:455
precise.home = 1f   [F]  SilentFX.Cards.PreciseCutFx.cs:1296
precise.needles = 0f   [F]  SilentFX.Cards.PreciseCutFx.cs:505
precise.shift = 1f   [F]  SilentFX.Cards.PreciseCutFx.cs:430
precise.shift_boss = 0f   [F]  SilentFX.Cards.PreciseCutFx.cs:618
precise.shift_on = 1f   [F]  SilentFX.Cards.PreciseCutFx.cs:593

### roll (6)
roll.echo_time = 0.24f   [F]  SilentFX.Cards.DodgeAndRollFx.cs:227
roll.half_turns = 3   [I]  SilentFX.Cards.DodgeAndRollFx.cs:149
roll.hop = 0.3f   [F]  SilentFX.Cards.DodgeAndRollFx.cs:67
roll.print_alpha = 0.2f   [F]  SilentFX.Powers.RollEchoFx.cs:101
roll.reach = 1f   [F]  SilentFX.Cards.DodgeAndRollFx.cs:89
roll.time = 0.3f   [F]  SilentFX.Cards.DodgeAndRollFx.cs:41

### shadow (14)
shadow.erupt_shake = 0f   [F]  SilentFX.Powers.ShadowPoolFx.cs:87
shadow.flood = 0.36f   [F]  SilentFX.Powers.ShadowPoolFx.cs:92
shadow.foot = 1.15f   [F]  SilentFX.Powers.ShadowPresence.cs:269 +1 more
shadow.mist_high = 0.5f   [F]  SilentFX.Powers.ShadowPresence.cs:301
shadow.mist_low = 0.26f   [F]  SilentFX.Powers.ShadowPresence.cs:301
shadow.mist_wide = 1.25f   [F]  SilentFX.Powers.ShadowPresence.cs:303
shadow.spike = 1f   [F]  SilentFX.Cards.ShadowEcho.cs:316
shadow.spike_fall = 0.15f   [F]  SilentFX.Cards.ShadowEcho.cs:458
shadow.spike_height = 2.1f   [F]  SilentFX.Cards.ShadowEcho.cs:324
shadow.spike_hold = 0.1f   [F]  SilentFX.Cards.ShadowEcho.cs:457
shadow.spike_lunge = 0.045f   [F]  SilentFX.Cards.ShadowEcho.cs:136
shadow.spike_rise = 0.06f   [F]  SilentFX.Cards.ShadowEcho.cs:456
shadow.strike_shake = 1f   [F]  SilentFX.Cards.ShadowEcho.cs:230
shadow.strike_shake_at = 18f   [F]  SilentFX.Cards.ShadowEcho.cs:234

### shadowmeld (2)
shadowmeld.dusk = 0.14f   [F]  SilentFX.Cards.ShadowmeldHeld.cs:57 +1 more
shadowmeld.dusk_front = 0.07f   [F]  SilentFX.Powers.ShadowmeldFx.cs:137

### shadowstep (5)
shadowstep.fade_in = 8f   [F]  SilentFX.Cards.ShadowStepHeld.cs:359
shadowstep.fade_out = 6f   [F]  SilentFX.Cards.ShadowStepHeld.cs:359
shadowstep.gulp = 1f   [F]  SilentFX.Cards.ShadowStepFx.cs:1225
shadowstep.hold = 0.2f   [F]  SilentFX.Cards.ShadowStepHeld.cs:358
shadowstep.trail_thick = 0.62f   [F]  SilentFX.Cards.ShadowStepFx.cs:2379

### skewer (17)
skewer.arrow_length = 120f   [F]  SilentFX.Cards.SkewerFx.cs:559
skewer.arrow_thick = 13f   [F]  SilentFX.Cards.SkewerFx.cs:560
skewer.bead = 24f   [F]  SilentFX.Cards.SkewerFx.cs:561
skewer.bead_gap = 0.07f   [F]  SilentFX.Cards.MalaiseFx.cs:1196 +1 more
skewer.beat = 0.3f   [F]  SilentFX.Cards.SkewerFx.cs:846
skewer.cap_dark = 0.86f   [F]  SilentFX.Cards.MalaiseFx.cs:1197 +1 more
skewer.cap_size = 1f   [F]  SilentFX.Cards.MalaiseFx.cs:1197 +1 more
skewer.column_gap = 34f   [F]  SilentFX.Cards.SkewerFx.cs:562
skewer.column_step = 38f   [F]  SilentFX.Cards.SkewerFx.cs:563
skewer.first_hit = 0.16f   [F]  SilentFX.Cards.SkewerFx.cs:849
skewer.flight = 0.09f   [F]  SilentFX.Cards.SkewerFx.cs:848
skewer.hold_pad = 0.9f   [F]  SilentFX.Cards.SkewerFx.cs:1151
skewer.linger = 0.3f   [F]  SilentFX.Cards.SkewerFx.cs:932
skewer.max = 12   [I]  SilentFX.Cards.SkewerFx.cs:227
skewer.preview_count = 3   [I]  SilentFX.Cards.SkewerFx.cs:274 +1 more
skewer.quiver = 0.2f   [F]  SilentFX.Cards.SkewerFx.cs:564
skewer.timeout = 1.6f   [F]  SilentFX.Cards.SkewerFx.cs:850

### sly (15)
sly.dart_min = 0.45f   [F]  SilentFX.Cards.HandMoves.cs:2934
sly.glint_period = 2.4f   [F]  SilentFX.Cards.SlyGold.cs:679
sly.hop = 0f   [F]  SilentFX.Cards.HandMoves.cs:2422
sly.hop_gamble = 0f   [F]  SilentFX.Cards.HandMoves.cs:2190 +1 more
sly.lean_dodge = 1f   [F]  SilentFX.Cards.HandMoves.cs:2558
sly.lean_rise = 0.5f   [F]  SilentFX.Cards.HandMoves.cs:2445
sly.lean_speed = 1f   [F]  SilentFX.Cards.HandMoves.cs:2493
sly.lean_sway = 3.2f   [F]  SilentFX.Cards.HandMoves.cs:2492
sly.on = 1f   [F]  SilentFX.Cards.HandMoves.cs:2197 +2 more
sly.toss = 0f   [F]  SilentFX.Cards.SlyGold.cs:442
sly.toss_hop = 70f   [F]  SilentFX.Cards.SlyGold.cs:545
sly.toss_time = 0.32f   [F]  SilentFX.Cards.SlyGold.cs:511
sly.wipe_alpha = 0.62f   [F]  SilentFX.Cards.SlyGold.cs:424
sly.wipe_fade = 0.42f   [F]  SilentFX.Cards.SlyGold.cs:417
sly.wipe_flash = 1f   [F]  SilentFX.Cards.SlyGold.cs:277

### speedster (10)
speedster.flick = 0.95f   [F]  SilentFX.Powers.SpeedsterFx.cs:281
speedster.judder = 8f   [F]  SilentFX.Powers.SpeedsterFx.cs:173
speedster.kick_from = 4f   [F]  SilentFX.Powers.SpeedsterFx.cs:84
speedster.kick_size = 0.75f   [F]  SilentFX.Powers.SpeedsterFx.cs:80
speedster.kick_time = 0.32f   [F]  SilentFX.Powers.SpeedsterFx.cs:81
speedster.sink_time = 0.2f   [F]  SilentFX.Powers.SpeedsterFx.cs:83
speedster.spin_from = 2f   [F]  SilentFX.Powers.SpeedsterFx.cs:84
speedster.spin_time = 0.85f   [F]  SilentFX.Powers.SpeedsterFx.cs:81
speedster.spin_to = 6.5f   [F]  SilentFX.Powers.SpeedsterFx.cs:85
speedster.trigger_spin = 1f   [F]  SilentFX.Powers.SpeedsterFx.cs:363

### strangle (21)
strangle.bite = 0.4f   [F]  SilentFX.Cards.StrangleRope.cs:639
strangle.cover = 0.46f   [F]  SilentFX.Cards.StrangleRope.cs:647
strangle.girth = 1f   [F]  SilentFX.Cards.StrangleRope.cs:637
strangle.girth_bare = 1.1f   [F]  SilentFX.Cards.StrangleRope.cs:637
strangle.give = 0.35f   [F]  SilentFX.Cards.StrangleRope.cs:643
strangle.grip = 0.1f   [F]  SilentFX.Cards.StrangleRope.cs:756
strangle.haul = 0.22f   [F]  SilentFX.Cards.StrangleRope.cs:768
strangle.height = 0.05f   [F]  SilentFX.Cards.StrangleRope.cs:653
strangle.loose = 0.72f   [F]  SilentFX.Cards.StrangleRope.cs:638
strangle.out = 0.26f   [F]  SilentFX.Cards.StrangleRope.cs:757
strangle.pinch = 0.07f   [F]  SilentFX.Cards.StrangleRope.cs:648
strangle.quick_grip = 0.1f   [F]  SilentFX.Cards.StrangleRope.cs:756
strangle.quick_haul = 0.15f   [F]  SilentFX.Cards.StrangleRope.cs:768
strangle.quick_out = 0.22f   [F]  SilentFX.Cards.StrangleRope.cs:757
strangle.quick_slack = 0.05f   [F]  SilentFX.Cards.StrangleRope.cs:767
strangle.recoil = 240f   [F]  SilentFX.Cards.StrangleRope.cs:645
strangle.rope = 10f   [F]  SilentFX.Cards.StrangleRope.cs:636
strangle.shake = 1f   [F]  SilentFX.Cards.StrangleRope.cs:1367
strangle.shreds = 12   [I]  SilentFX.Cards.StrangleRope.cs:1366
strangle.slack = 0.08f   [F]  SilentFX.Cards.StrangleRope.cs:767
strangle.wait = 0.75f   [F]  SilentFX.Cards.StrangleRope.cs:772

### suppress (13)
suppress.hit_wait = 0.8f   [F]  SilentFX.Cards.SuppressShow.cs:692 +1 more
suppress.hold = 0.32f   [F]  SilentFX.Cards.SuppressShow.cs:815
suppress.load_per_frame = 4   [I]  SilentFX.Cards.SuppressFrames.cs:257
suppress.on = 1f   [F]  SilentFX.Cards.SuppressFrames.cs:166
suppress.size = 1.75f   [F]  SilentFX.Cards.SuppressShow.cs:970
suppress.v3 = 1f   [F]  SilentFX.Cards.SuppressStage.cs:268
suppress.v3.dim = 1f   [F]  SilentFX.Cards.SuppressRun.cs:978
suppress.v3.hide_arrow = 1f   [F]  SilentFX.Cards.SuppressRun.cs:1094
suppress.v3.hold = 1f   [F]  SilentFX.Cards.SuppressRun.cs:665
suppress.v3.over_ui = 1f   [F]  SilentFX.Cards.SuppressRun.cs:212
suppress.v3.size = 1f   [F]  SilentFX.Cards.SuppressStage.cs:370
suppress.v3.soul_copy = 1f   [F]  SilentFX.Cards.SoulCopy.cs:97
suppress.v3.yank = 1f   [F]  SilentFX.Cards.SuppressRun.cs:895

### symphony (6)
symphony.armed_tempo = 1.18f   [F]  SilentFX.Cards.ShivHeld.cs:466
symphony.hop_height = 46f   [F]  SilentFX.Cards.ShivHeld.cs:531
symphony.hop_time = 0.56f   [F]  SilentFX.Cards.ShivHeld.cs:836
symphony.pause = 0.05f   [F]  SilentFX.Cards.ShivHeld.cs:838
symphony.settle = 0.26f   [F]  SilentFX.Cards.ShivHeld.cs:837
symphony.windup = 0.12f   [F]  SilentFX.Cards.ShivHeld.cs:835

### trap (3)
trap.refill = 1f   [F]  SilentFX.Cards.KnifeTrapFx.cs:320
trap.refill_time = 0.24f   [F]  SilentFX.Cards.KnifeTrapFx.cs:1273
trap.ring = 16   [I]  SilentFX.Cards.KnifeTrapFx.cs:369

### wail (12)
wail.arc = 0.8f   [F]  SilentFX.Cards.PiercingWailFx.cs:252
wail.bend = 0.042f   [F]  SilentFX.Cards.PiercingWailFx.cs:285
wail.fronts = 3   [I]  SilentFX.Cards.PiercingWailFx.cs:247
wail.gap = 0.1f   [F]  SilentFX.Cards.PiercingWailFx.cs:249
wail.gather = 0.07f   [F]  SilentFX.Cards.PiercingWailFx.cs:248
wail.sag = 0.11f   [F]  SilentFX.Cards.PiercingWailFx.cs:253
wail.shake = 2   [I]  SilentFX.Cards.PiercingWailFx.cs:564
wail.tall = 0.92f   [F]  SilentFX.Cards.PiercingWailFx.cs:251
wail.thick = 0.26f   [F]  SilentFX.Cards.PiercingWailFx.cs:254
wail.time = 0.52f   [F]  SilentFX.Cards.PiercingWailFx.cs:250
wail.warp = 1f   [F]  SilentFX.Cards.PiercingWailFx.cs:386
wail.width = 0.055f   [F]  SilentFX.Cards.PiercingWailFx.cs:286

### wave (14)
wave.flood_back = 0.16f   [F]  SilentFX.Powers.CorrosiveWaveFx.cs:401
wave.flood_bubbles = 26   [I]  SilentFX.Powers.CorrosiveWaveFx.cs:540
wave.flood_drain = 0.9f   [F]  SilentFX.Powers.CorrosiveWaveFx.cs:368
wave.flood_glow = 0.3f   [F]  SilentFX.Powers.CorrosiveWaveFx.cs:286
wave.flood_haze = 0.06f   [F]  SilentFX.Powers.CorrosiveWaveFx.cs:287
wave.flood_lift = 0.3f   [F]  SilentFX.Powers.CorrosiveWaveFx.cs:419
wave.flood_pulse_speed = 2300f   [F]  SilentFX.Powers.CorrosiveWaveFx.cs:583
wave.flood_shake = 1f   [F]  SilentFX.Powers.CorrosiveWaveFx.cs:751
wave.flood_stain = 0.46f   [F]  SilentFX.Powers.CorrosiveWaveFx.cs:285 +1 more
wave.flood_tilt = 0.2f   [F]  SilentFX.Powers.CorrosiveWaveFx.cs:420
wave.flood_time = 0.85f   [F]  SilentFX.Powers.CorrosiveWaveFx.cs:577
wave.flood_wade = 9f   [F]  SilentFX.Powers.CorrosiveWaveFx.cs:162 +1 more
wave.held_creep = 0.1f   [F]  SilentFX.Cards.CorrosiveWaveHeld.cs:56
wave.held_creep_time = 2.5f   [F]  SilentFX.Cards.CorrosiveWaveHeld.cs:57

### wraith (30)
wraith.card.fire = 1f   [F]  SilentFX.Cards.WraithCardFire.cs:75 +1 more
wraith.card.fire_size = 1f   [F]  SilentFX.Cards.WraithPlayFx.cs:826
wraith.card.to_her = 1f   [F]  SilentFX.Cards.WraithPlayFx.cs:661
wraith.figure.follow = 1f   [F]  SilentFX.Cards.WraithFigure.cs:328 +1 more
wraith.figure.size = 1f   [F]  SilentFX.Cards.WraithFigure.cs:324 +1 more
wraith.held.haze = 1f   [F]  SilentFX.Cards.WraithHeld.cs:89 +1 more
wraith.held.on = 1f   [F]  SilentFX.Cards.WraithHeld.cs:49 +1 more
wraith.held.over_ui = 1f   [F]  SilentFX.Cards.WraithHeld2.cs:180
wraith.held.shake = 0.6f   [F]  SilentFX.Cards.WraithHeld2.cs:145
wraith.intro.body_before = 0.25f   [F]  SilentFX.Powers.IntangibleFx.cs:86
wraith.intro.dark = 0.86f   [F]  SilentFX.Cards.WraithFormIntroFx.cs:343
wraith.intro.dim = 0.3f   [F]  SilentFX.Cards.WraithFormIntroFx.cs:344
wraith.intro.fire = 1f   [F]  SilentFX.Cards.WraithFormIntroFx.cs:347
wraith.intro.flash = 0.55f   [F]  SilentFX.Cards.WraithFormIntroFx.cs:345
wraith.intro.mist = 0.7f   [F]  SilentFX.Cards.WraithFormIntroFx.cs:346
wraith.intro.on = 1f   [F]  SilentFX.Cards.WraithFormIntroFx.cs:242 +2 more
wraith.intro.pace = 1f   [F]  SilentFX.Cards.WraithFormIntroFx.cs:341 +1 more
wraith.intro.pour = 1f   [F]  SilentFX.Cards.WraithFormIntroFx.cs:348 +1 more
wraith.intro.pour_wisps = 9   [I]  SilentFX.Cards.WraithFormIntroFx.cs:514
wraith.intro.scream_at = 1.05f   [F]  SilentFX.Cards.WraithFormIntroFx.cs:342
wraith.intro.shake = 3   [I]  SilentFX.Cards.WraithFormIntroFx.cs:1096 +1 more
wraith.intro.shake2 = 2   [I]  SilentFX.Cards.WraithFormIntroFx.cs:1169
wraith.intro.shake3 = 0   [I]  SilentFX.Cards.WraithFormIntroFx.cs:1209 +1 more
wraith.intro.sound = 1f   [F]  SilentFX.Cards.WraithFormIntroFx.cs:1101 +1 more
wraith.intro.warp = 1f   [F]  SilentFX.Cards.WraithFormIntroFx.cs:349
wraith.play.dark = 1f   [F]  SilentFX.Cards.WraithPlayFx.cs:416
wraith.play.knock = 1   [I]  SilentFX.Cards.WraithPlayFx.cs:1182
wraith.play.over_ui = 1f   [F]  SilentFX.Cards.WraithPlayFx.cs:523
wraith.play.shake = 0.6f   [F]  SilentFX.Cards.WraithPlayFx.cs:411
wraith.v2 = 1f   [F]  SilentFX.Cards.WraithStage.cs:78
```
