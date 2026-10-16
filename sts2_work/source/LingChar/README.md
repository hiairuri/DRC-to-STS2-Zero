# LingChar — 零（龙族）可玩角色 mod（spike）

《杀戮尖塔2》可玩角色：龙族的零。卡池**临时用无色牌卡池**，初始牌组**六张打击 + 四张防御**（借用静默猎手版本）。

`python build_lingchar.py` = 组装资产 → dotnet 编译 → 打 pck → 安装到游戏 Mods 目录。

## 角色注册机制（反编译考证，零猜测）

- **模型自动发现**：`ModelDb.Init()` 遍历 `AllAbstractModelSubtypes`（游戏子类型 + `ReflectionHelper.GetSubtypesInMods<AbstractModel>()`）——mod 程序集里的 `CharacterModel` 子类自动入库，id=`Slugify(类名)`="LING"。
- **选人列表**：`ModelDb.AllCharacters` 是硬编码 5 角色数组 → `src/ModEntry.cs` 用 Harmony postfix `get_AllCharacters` 追加 Ling（`ModelDb.Contains` 守卫 Init 前的早调用）。
- **解锁**：`UnlockState.Characters` = AllCharacters 减 epoch 门控角色；Ling 无 epoch 门控 = **默认解锁**，零存档迁移。
- **本地化**：mod pck 内 `LingChar/localization/<lang>/characters.json`（游戏按 `res://<modid>/localization/...` 合并 mod 表，键=`LING.title` 等）。

## 资产面（id="ling" 派生）

不可覆写的路径成员要求的资产全部由 pcksrc 提供（Silent 场景的文本 .tscn 复制 + 外科手术：文件名/头 uid/骨架引用改写，内部脚本与贴图引用保持指向原版资产）：

| pcksrc 内路径 | 来源 |
|---|---|
| `scenes/creature_visuals/ling.tscn` | silent 版复制；骨架引用→`res://animations/characters/ling/ling_skel_data.tres`（**uid 已剔除**——uid 命中会压过 path， skins 缺席时会加载回静默骨架） |
| `scenes/vfx/card_trail_ling.tscn` 等五个兄弟场景 | verbatim 复制（出牌拖尾/能量计数器/休息点/商人/选人背景） |
| `materials/transitions/ling_transition_mat.tres` | verbatim（贴图引用静默的过场图，存在） |
| `animations/characters/ling/ling_skel_data.tres` | 自建；引用下方共享 spine 文件 |
| `ling/animations/characters/silent/*` + `.godot/imported/*` | **与 LingSilentSkin 完全同路径复用**（md5 导入产物名不变即有效）；skin 缺席时本 pck 独立供给 |
| `images/ui/top_panel/character_icon_ling*.png.import`、`images/ui/hands/multiplayer_hand_ling_*.png.import` | remap 改写到游戏 pck 的静默 ctex 产物（uid 剔除，占位 png 不参与运行时） |

可覆写虚成员直接借用 Silent 资产（选人大图/锁定图/地图标记/图标场景/选人与转场音效）；攻击/施法/死亡 FMOD 事件路径派生自 id（`event:/sfx/characters/ling/...` 不存在——FMOD 播放缺失事件只记日志，专属音效后补）。

## 视觉装饰分工

战斗内渲染由 **DyeFramework（2026-10-08 已归并进本 mod，原 DRCSpineDye 独立 mod 退役）**接管：框架源码以 Compile Include 直接编进 LingChar.dll——本 spike 已于 2026-10-16 随 MirrorPupil 一同迁入 `D:\项目\dragonraja\spikes\LingChar\`，该引用由跨仓路径改为**树内相对引用**（单一事实源仍留 dragonraja 仓 sts2_work/DRCSpineDye/dll/，禁止另起第二份），`res://drc_dye/` 资产随 LingChar.pck 分发，ModEntry 显式调 `DyeFramework.Initialize()`（幂等，与其自带 ModInitializer 特性互为双保险）。本角色骨架路径 = LingSilentSkin profile 的匹配路径，框架自动挂 holo 直出材质/斜二测骨骼皮影子/死亡溶解/胜利 win+winloop 链/入场 tibu watcher。`Ling.GenerateAnimator` 初始态直接 `idle_loop`（入场由框架 watcher 播 tibu，避免双播）；动画分支含 Relaxed/Shiv→既有动画的映射（ling 无 relaxed_loop/shiv）。

## 临时借用项（后续替换）

1. 卡池=ColorlessCardPool（用户指定，专属卡池后补）。
2. 打击/防御=静默猎手版本；遗物/药水池=Silent 池。
3. 选人大图/地图标记/图标=静默的图；专属美术后补（见上表 `.import` remap 方案）。
4. `LING.*` 文案是占位初稿（loc/eng+zhs）。

## 言灵·镜瞳（MirrorPupil 遗物，已集成）

初始遗物=MirrorPupil spike 的言灵·镜瞳（源码级集成：csproj 直接编译 `..\MirrorPupil\src\**\*.cs`，单一事实源留在 spike）：

- `ModEntry.Initialize`：`ModHelper.AddModelToPool<SilentRelicPool, MirrorPupil>()`（Ling 借用 Silent 遗物池；副作用=静默猎手的遗物奖励池也会出现它，专属池后补时一并解决）。
- `Ling.StartingRelics` = 言灵·镜瞳（不再是蛇戒）。先古版由"欧洛巴的抚触"替换获得（spike 的 Harmony 补丁经本工程 PatchAll 自动挂上）。
- 镜像召唤物视觉：`scenes/creature_visuals/mirror_image.tscn`——**用奥斯提（真宠物）的场景模板整体拷贝再改**（剥掉火焰 VFX 节点与材质、骨架换镜像 ling_skel_data）：宠物的血条/状态栏/点击区/站位布局都由场景里奥斯提调校过的 Bounds/CenterPos/IntentPos 驱动，角色场景模板的 Bounds 配置与宠物不同（这是"没血条、坐标不对"的根源）。
- **召唤系统按 OstyCmd 逐行重做**（2026-10-02 用户指令）：①fresh 召唤初始生命=`Heal(…, playAnim: false)` **静默落血**——奥斯提召唤没有回血特效的机制就在这一参（此前默认 true=绿色十字+数字+音效全放）；②复活路径=AddPetInternal→SetMaxHp→Heal(playAnim 默认 true)=回血特效+复活动画（Heal 的 isDead 分支）；③卡源新召唤亮相=Modulate 透明→白 0.35s 延迟 0.1+StartReviveAnim（奥斯提同款）；④TrackBlockStatus(主人)=镜为你碎的格挡显示跟随。
- 镜像材质=**龙卡真身考证版**（2026-10-03）：父材质 uexp 内嵌 shader 哈希对 shaders_index.csv 命中 37 处（双拷贝）→ FS=u066941 家族（与战斗 p215 同族）。真公式：`out = base + w×(A − base)`，w=屏幕浓度梯度（跨屏增白渐变），w→1=纯白克隆；alpha=A×顶点色.a。按用户指令 w 梯度不移植 → STS2 版=均匀增白 `mix(base.rgb, vec3(A), whiten)`（whiten uniform 默认 1=纯白，<1 留贴图明暗）；MixColor/ScanLine 确认死参数。
- 镜像死亡：ling 的 die 第三阶段=全槽渐隐→镜像死在原地自然"化空"，`dead_loop` 缺失只 warn 无碍。
- `MirrorImage` 已按**奥斯提（Osty）完整代码逐成员对照重写**（用户指令防遗漏）：NOTHING_MOVE 状态机、Osty 形状的完整动画图（cast/attack/hurt 各回 idle+Hit 分支）、IsHealthBarVisible=存活。血条/状态显示由 NCreature 通用层（%HealthBar）负责，宠物流与奥斯提一致。
- **卡牌卡处理区 bug（2026-10-02 日志实锤，已修）**：`FromCard_Prefix` 返回 false 跳过原方法体但没设 `__result`→`FromCard` 返回 null→重放时 `.Targeting(...)` NRE→出牌任务中断。修复=prefix 签名加 `ref AttackCommand __result` 并赋 `__instance`；另加 `AfterCardPlayed` 整体 try/catch 防护（钩子异常绝不逃逸中断出牌管线）。
- **重放目标三段式+扣层规则（2026-10-03 终版）**：①原目标合法（存活且在 HittableEnemies）用原目标；②不合法→随机一个合法目标；③一个都没有→不重放也不扣层。君王之剑"原目标没死也炸"的真因=SeekingEdge 使其 TargetType 变 AllEnemies，游戏允许**空目标**打出（IsValidTarget(null) 对 AllEnemies 为 true），重放传 null 炸 ThrowIfNull；扣层规则=**只有重放真正发生才扣**（非攻击 proc 只召唤不扣层）。
- **重放无目标卡死（2026-10-02 实机实锤，已修）**：原牌把目标打死且无存活敌人时，AutoPlay 走 `MoveToResultPileWithoutPlaying` 把**正在打出的原牌**二次塞回 Play 堆→出牌管线状态损坏卡死。修复=重放前检查 `HittableEnemies.Count>0`，无目标就不重放（瞳印照常结算）。
- **镜像血条/坐标真因（2026-10-02 源码实锤，已修）**：游戏的宠物 UI 特权写死 `is Osty`——`NCombatRoom.AddCreature` 宠物分支里本地奥斯提走专属提前返回（保血条+站主人旁），**其余宠物一律 `ToggleIsInteractable(false)`（`_stateDisplay`=%HealthBar 直接 Visible=false）+ 通用站位公式（单宠时把镜像推进主人身体）**。修复=两个 Harmony postfix（`AddCreature`+`PositionPlayersAndPets`）把本地镜像拉回奥斯提待遇：`ToggleIsInteractable(IsHealthBarVisible)` 恢复血条、站主人左侧（X-60-BoundsW/2, Y+10）、画序插到主人前。
- **loc 键勘误**：PowerModel 的 loc 键=模型 id（Slugify 类名）`PUPIL_MARK_POWER.*`，原表写成 `PUPIL_MARK.*` 导致状态名/描述查不到（日志 `Key not found`）。
- **游戏崩溃真因（2026-10-03 转储定案）**：`CrashDumps/SlayTheSpire2.exe.24004.dmp` 经 dotnet-dump 解析＝崩溃线程是 **GC 终结器线程**（`GodotObject.Finalize→Dispose→原生 AV→ExecutionEngineException`）——主线程拆战斗场景与 GC 终结器释放"原生已随场景释放的包装"并发＝进程级崩溃。缓解（DyeFramework.dll 52,224B 已装）：①`CombatEnded`+1.5s 后主线程 `GC.Collect+WaitForPendingFinalizers`（场景图静止期排空终结器，竞态窗口不再开）；②`ShadowSilhouette.Shutdown` 主动置空缓存包装数组（陈旧包装不过夜）。诊断日志保留中：`[MirrorPupil] hook enter/roll/replay gate`、`attack fx: bursts scheduled`。
- **无目标卡死（2026-10-03 破案）**：上一局诊断日志全链干净（钩子 enter/roll/replay gate/replay done 全部正常，重放门控实战生效）——卡死发生在**胜利后**：**BaseLib**（workshop 角色框架，其 dll 内含 `CheckFifteenElitesDefeatedEpoch`/`SkipEliteEpochCheck` 字符串实锤）给原版该检查打的 Harmony 补丁按自己的角色表查 epoch，遇到 CHARACTER.LING 抛 ArgumentOutOfRangeException，异常沿 `EndCombatInternal → CheckWinCondition` 炸断战斗收尾＝处理区的牌永远不进结果堆。**修复=`src/EpochCheckCompat.cs`**：HarmonyFinalizer 挂在原版 `SaveManager.UpdateProgressAfterCombatWon` 上，捕获 ArgumentOutOfRangeException 即吞掉（其余异常原样重抛）——无论补丁来自哪个 mod 都兜得住；代价只是该 mod 的"十五精英"纪元检查对 Ling 不生效。
- **生效 mod 集（2026-10-08 起）**：LingChar（角色+镜瞳+镜像+光影框架一体）+ 零的皮肤（LingSilentSkin，纯 pck 无 dll）。DRCSpineDye 已退役（dll+pck+json 移入 sts2_work/disabled_mods/）。其余多载 BaseLib / QuickRestart / 奥卡雾锻婚纱 / LingUrbanSkin 按需禁用；兼容垫片保留兜底。
- loc：relics/powers/monsters 三表（zhs+eng）并入 mod loc 合并。
- 遗物/状态图标暂缺（spike 留白）：逻辑不受影响，图标后补。

## 已验证

- `dotnet build` 0 警告 0 错误（ModEntry/Ling 全部游戏 API 过真编译）。
- pck 25 文件 1.71MB，安装于 `Mods/LingChar/{LingChar.json, LingChar.dll, LingChar.pck}`。
- 待游戏内验收：选人界面出现"零"且默认解锁、进战模型/入场/攻击/死亡、无色卡池奖励、初始牌组 6+4。
