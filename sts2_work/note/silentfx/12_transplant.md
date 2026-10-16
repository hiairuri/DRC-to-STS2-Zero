# 12 · SilentFX 挖掘成果 → LingChar/LingSilentSkin 移植映射报告（三探·移植映射代理）

> 日期：2026-10-16 ｜ 输入：silentfx_decomp/reports/01~11 共 11 份 + 项目状态交接.md + 光影层移植计划.md（关键段）
> 输出对象：龙族零（UE4 资产）→ 杀戮尖塔2（Godot）移植项目，落点=spikes/LingChar、sts2_work/DRCSpineDye/dll/DyeFramework.cs、hit_fx_ling.json / attack_fx_ling.json、build_lingchar.py
> **铁律符合性总声明**：本报告引用的全部 silentFX 数值均标注来源，且**只引用「可从 silentFX 反编译源码（split/*.cs）或 gdshader/gd/json 资产直读」的数**——这属于铁律 1 三链路中的「源码/资产」链路，合法；任何 silentFX 运行画面/截图均未使用、也不得作为取数依据。我们自己的项目数值继续只走 UE4 资产/源码/运行时 hook 三链路。
> **去污染总声明**：下文所有新增文件（shader/config/纹理/脚本）一律落 LingChar 自有路径（pcksrc 新子目录或 drc_dye/profiles/ling/ 下），**禁止写入 LingSilentSkin 任何路径**；drc_dye 的 silent profile 虽在 LingChar.pck 内，但它服务皮肤角色观感——除非条目明确说明，默认只动 ling profile。

---

## 0. 结论先行

1. SilentFX 的价值分层非常清晰：**数学与工程结构可直接搬（P0/P1），角色专属叙事内容不搬（P3）**。我们项目当前最大的三个缺口——调参循环慢（十轮打回全靠实机跑局）、溶解/死亡观感是"通用近似版"、攻击特效锚点靠手工取数——分别对应 silentFX 的 DevPreview+tuning 体系、her.gdshader 溶解族+灰碎粒子配方、rig.json/锚点导出模式，三者都是"直接搬公式/架构"级。
2. SilentFX 全库 49 个 gdshader 中约 20 个共享同一套 hash21+vnoise+fbm 基础段（02_shaders §3.1），这部分是我们影子/溶解/死亡粒子三条线**共用的噪声地基**，一次移植三线受益。
3. 明确不搬的：FSB 拆解/FM OD 导入表钩子（我们音频是自有 UE4 资产，不缺料）、MP 四层同步（无需求且 silentFX 自己的 Exact 链在目标游戏版本上从未实跑，06_mp_compat §结论 3）、HandMoves 手牌编排全家桶（角色 mod 不动手牌层）。

**如果只做三件事（Top3，详细论证见 §6）**：
1. **FxTuning 统一调参层 + DevPreviewLing 预览台**（把"改 config→看效果"从"重建+实机一局"压到"F8 秒级"）；
2. **溶解双频公式 + 灰碎两段式死亡粒子**（补完光影层阶段 3 尾巴，数值可直读）；
3. **构建期锚点/rig 侧车导出**（把 muzzle_hull 式手工取数泛化为 build_lingchar.py 自动化）。

---

## 1. P0 清单（立即能做：直接搬公式/架构，数值可直读）

### P0-1 溶解 shader 升级：单阈值 → 双频噪声 + 亮边带
- **来源**：02_shaders T5（her.gdshader:45-47 双频溶解；shade_mist.gdshader:94 噪声蚀边）。
- **落点**：LingChar pcksrc 内 ling_holo/dissolve 自有路径下的 `ling_dissolve_mix.gdshader`（现有实现=单噪声阈值 discard+边缘带，见光影层计划条目表 STS2 实现行）+ DyeFramework.cs DeathWatcher（无需改，uniform 接口不变）。
- **公式（可从 silentFX 资产直读）**：
  - 双频噪声 `dn = nn(q1)*0.75 + nn(q2)*0.25`，阈值带 `a *= 1 - smoothstep(dn-0.08, dn+0.08, dissolve)`；
  - 亮边 `c.rgb += vec3(0.30,0.55,0.52) * (smoothstep(dn-0.25,dn) - smoothstep(dn-0.08,dn+0.08)) * 0.8`；
  - 蚀边变体（若想让溶解边缘"破碎"而非光滑）：`1 - smoothstep(0.12, 1.08, edge + (n-0.5)*wisp)`，edge 可接现有噪声场。
- **与 UE4 真值的关系**：UE4 侧公式已解码定案（ps1=世界坐标投影噪声、`ceil((noise+off−T)·(noise+off−1))` 双门+discard、边缘=liangbian_color，见光影层计划条目表"溶解公式源码解码 ✅"行）——**先保证逐行复刻 UE 公式**；双频/亮边带作为"观感增强层"叠加或作为逐行公式失真时的等效补偿，两者不冲突。铁律 1 口径：UE 数值来自 uexp/GLSL 资产链，silentFX 数值来自其 gdshader 资产链，均合法。
- **收益**：溶解边缘从"硬阈值"升级为"有层次的侵蚀+发光缘"，直接提升死亡观感（当前验收痛点之一）。
- **要点**：亮边颜色可做成 config 键（对照我们 `_note_*` 惯例）；保持 `pipeline_encode=0` sRGB 直通结论不被破坏（条目 45 教训）。

### P0-2 死亡粒子：灰碎两段式 + 阻尼解析粒子公式
- **来源**：08_cards_malaise_gamble §1.3（MalaiseFx.cs:1895-1896：Crumble 两段 Burst=16 片大火星+6 片慢灰）；10_gd_stages §2.5（suppress_stage.gd:1210-1264 `addp()`：`pos=p0+v(1-e^{-ka})/k`，k=0 匀速，+重力+尺寸插值+速度拉伸+淡入淡出）；02_shaders T4（puff 幂混合，若要灰烬成雾）。
- **落点**：DyeFramework.cs 死亡序列（DeathWatcher → 溶解 tween → 粒子 → 还原）中现有通用灰烬版；粒子参数段落 LingChar 自有 config（如 attack_fx_ling.json 同级新增 death_fx 段，build_lingchar.py 注入）。
- **真值链先行**：光影层计划阶段 3.5 的 UE4 真值路径（FIGHTER_DEATH_DISPLAY_EFFECT_CFGID → bny → res_effects/particles 属性转储 → GPUParticles2D）仍是第一优先；silentFX 配方是**真值查不到/形态对不上时的结构模板**（发射量分两档、快慢双速、速度拉伸），不是数值替代。
- **收益**：死亡"溶解完就没了"升级为"侵蚀+两段灰碎+余烬"，与 0.5s/1.2 线性 tween（已定案常量）时序对齐；两段式的参数（16/6、双速）可从 silentFX 源码直读，属观感结构标定。
- **要点**：沿用条目 55 定案的 DragToDamping 语义（`return k`，与 UE AccelerationDrag 1:1）——不要重蹈 55① 的换算覆辙。

### P0-3 FxTuning：统一调参解析器（tuning.ini 模式移植）
- **来源**：01_architecture §4.3（Tuning.cs：缺文件静默、float/int 双解析、大小写不敏感、def 兜底）；03_notes §①②（652 键 ini+462 隐藏键、"ini 是精选入口、代码是全量默认"结论）；08 §6.1（0=回到旧版惯例、参数即版本史）。
- **落点**：spikes/LingChar/src/ 新增 `FxTuning.cs`（~80 行微型解析器）+ 现有 hit_fx_ling.json / attack_fx_ling.json / death_fx 段统一挂载；DyeFramework.cs 的 FxCfg 解析逐步切换（保持旧键兼容）。
- **移植的四个特性**（全部是 silentFX 源码直读的机制，非数值）：
  1. **代码默认兜底**：C# 侧 `T(key, def)`，JSON 缺键/类型不符/文件缺失一律静默回退 def——删任何键不炸（对照我们 attack_fx "缺失键走代码默认值"惯例，推广为全键惯例）；
  2. **`_note_*` 注释即文档**：我们已有 `_note_dir_rot_fix` 先例（条目 52），推广为强制惯例——每个键一段"这个数管什么、为什么取这个值、哪轮定的"；
  3. **0=回到旧版**：开关键的 0 值绑定历史行为（我们 dir_rot_fix 默认 0=关已是同款），验收打回时回退成本为零；
  4. **保险参数文化**：每个异步动画必有超时收尾（silentFX 的 expose.wait/land/max_hold、shadowstep.wait 族）——我们 MuzzleFollower 已有 ExpiresAt，推广到 hit_fx 双脉冲与死亡序列。
- **收益**：hit_fx/attack_fx 十轮迭代的每轮 config 改动全部可追溯、可回退；新特效接入成本从"改 C#+改 JSON+懂解析逻辑"降到"加一个键"。
- **去污染**：FxTuning.cs 属 LingChar.dll 源码（LingChar.csproj Compile Include），不触碰皮肤链。

### P0-4 通用动效数学库：解析弹簧 + 指数阻尼 + 稳定哈希 + 多指数瞬态
- **来源**：08 §4.1/4.2、09 §⑤、05 §1.2——以下公式全部可从 silentFX 反编译源码直读：
  - `Spring(u, over, peak)`：由"过头几成+几秒到峰"反解 `ζ=-ln(over)/√(π²+ln²over)`、`ω_d=π/peak`（GambleFlow.cs:656-677）——**调参者不碰 ω/ζ**；
  - 半隐式欧拉+定长子步：`n=clamp(ceil(dt/0.006),1,24)`，`v+=(-k(y-rest)-c·v)h`（MalaiseFx.cs:2141-2159，k=220/c=21.4 为 silentFX 直读参考值）；
  - 临界阻尼冲量：踢入速度=位移×2√k（MalaiseFx.cs:2071/2082）——任意参数下回弹节奏一致；
  - `Damp(x,tgt,rate,dt)=Lerp(x,tgt,1-exp(-rate·dt))`（FxKit.cs:422）；
  - 稳定哈希 `Hash(i)=frac(sin(i·127.1+311.7)·43758.547)`（FxKit.cs:1053）——确定性伪随机，特效可复现可调试；
  - 多指数叠加瞬态："一下"的质感=2~3 个不同时间常数叠加（0.4·e^{-t/0.06}+0.1·e^{-((t-0.11)/0.035)²}，MalaiseFx.cs:3262）。
- **落点**：DyeFramework.cs 内新增 static FxMath 区段（该文件已是框架单一事实源，由 LingChar.csproj 源码包含）；先服务 hit_fx 双脉冲、MuzzleFollower flash 三段包络（条目 54 的交叉衰减可换成解析弹簧）、死亡粒子。
- **收益**：条目 41-56 反复出现的"拍脑袋系数→打回"循环（Damping×40/×60 被打回、halo 倍数五轮调整）根源是缺一套"直觉参数→物理正确"的换算层；Spring(u,over,peak) 这类 API 让"过头 9 成、0.3 秒到峰"直接可调。
- **要点**：受击弹簧我们已有同构（MurderFx Spring ζ=0.42ω 与条目 41 Damping 修复后的指数衰减语义一致），合并进 FxMath 避免第二份实现。

### P0-5 噪声公共段：hash21+vnoise+fbm（4~5 层+旋转矩阵）
- **来源**：02_shaders §3.1（约 20/49 shader 共享；旋转矩阵 `vec2(1.6x+1.2y+3.1,-1.2x+1.6y+1.7)` 防轴对齐）+ T3 mist() 三层域扭曲（malaise_mist.gdshader:72-78）。
- **落点**：LingChar pcksrc 新增自有 gdshader include 段（Godot 4.7 可用 `#include` 或生成期拼接，build_lingchar.py 装配期注入三个自有 shader：dissolve/影子影斑/死亡雾）。
- **收益**：影子软边（P1-7）、溶解噪声（P0-1）、死亡灰雾（P0-2）共用一份经过实战的噪声实现，省掉三份各写各的。
- **去污染**：include 文件放 drc_dye/profiles/ling/shaders/ 或 pcksrc 新子目录，silent profile 不引用。

---

## 2. P1 清单（本迭代内值得做）

### P1-1 DevPreviewLing 预览台 + F8 热重载
- **来源**：04_devtools_assets §①②（DevPreview：CanvasLayer Layer 90 假人 ColorRect、F6 显隐/F7 触发/F8 重载/F9/F10 翻页、F5 慢动作 0.15x；发布卫生=tour.on/dev_preview.txt 标记，03_notes §3.4 条 30）；02_shaders §4.1（ShaderLib.Reload 热重载循环 "Edit and press F8"）。
- **落点**：spikes/LingChar/src/ 新增 `DevPreviewLing.cs`（Harmony/ProcessFrame 驱动）；开关=游戏 Mods/LingChar 目录存在 `dev_preview.txt`（构建脚本不打包该文件）；config 热重载=FxTuning.Reload()（P0-3）+ 重建受影响 ShaderMaterial。
- **设计**：假战斗位（两个 ColorRect 当"她"与"假敌人"），热键直触发：hit_fx 八发射器、attack fx 枪口三件套（可注入假 muzzle 坐标）、死亡溶解全序列、影子/闪红；F5 慢动作用 Engine.TimeScale（注意我们的 MuzzleFollower 跟随走 ProcessFrame，TimeScale 下照常工作）。
- **收益**：条目 41→56 十轮打回的每一轮都是"重建→进游戏→打一局→截图"分钟级循环；预览台把它压到秒级，且 HITCH 监视器（条目 36 已有）可同场量化。
- **要点**：热键层必须 try-catch 全包（silentFX "补丁永不通向游戏崩溃"范式，01 §③）；发布包不含 dev_preview.txt 即自动休眠。

### P1-2 战斗资产分帧预热（AssetWarmup 模式 → 扩展 SelectWarm）
- **来源**：01 §4.6（AssetWarmup：FIFO 每帧一任务、CanRun 前置、8ms 预算披露、0.5s 安静期、排空统计）；02 T15（ShaderWarmup 32×32 离屏视口逐 shader 渲 2 帧；warm() 每调读一张图，suppress_stage.gd:167-177；WraithSceneWarmup 整场景预演）。
- **落点**：spikes/LingChar/src/SelectWarm.cs 家族（现有 v8 实例池+HITCH 监视器）；新增战斗资产段：hitfx_* 五纹理、dissolve/holo shader、attack fx 纹理的**启动期/主菜单期**预上传与 PSO 预编译（离屏 SubViewport 2 帧法）。
- **与 SelectWarm 教训对齐**：条目 37 定案"每个新实例首次 _draw 的 spine 网格首建 ~350ms 实例私有，预热救不了"——所以本条只做**跨实例可继承的资源**（纹理上传/shader 编译），不做实例级预热；首绘卡顿继续靠 v8 池（选人）+ 战斗侧待 HITCH 数据说话。
- **收益**：首次战斗出现特效时不再付纹理/shader 冷加载；预热漏项由 8ms 预算日志显式披露（对照 GameSounds "prewarm did not cover this stream" 的自警模式，07 §4.5）。
- **要点**：只在非战斗且未暂停窗口跑（照抄 AssetWarmup 的 CanRun 语义）；每帧一步防卡。

### P1-3 构建期锚点/rig 侧车导出（suppress_frames + rig.json 模式）
- **来源**：03_notes §4.1（suppress_frames.json：帧+锚点 f/c/h/sh+ground+faces 同期导出，C# 只读 7 个字段）；10_gd_stages §⑤⑨（rig.json：strand=中线折线+左右半宽+累计弧长；layer=贴图+关节锚点 elbow/fist/base/tip）；我们已有先例=条目 48/52 的 muzzle_hull/muzzle_bone 由 build_lingchar.py 解析 ling.json 注入。
- **落点**：build_lingchar.py 装配期新段：解析 ling.json → 导出 `anchors_ling.json`（每 slot：绑骨、attachment hull 四顶点、deform、槽位骨名；骨架级：胸口/脚底/头心等常量锚）→ 注入 ling profile config 或独立侧车文件（LingChar.pck 自有路径）。
- **泛化两步**：第一步=把 muzzle_hull 的取数自动化（消灭"两点差分+精确众数带"的手工脚本 diag_dir8/52④ 流程，改为构建期直算+构建日志对账）；第二步=若未来做披风/发丝类程序特效，按 rig strand 格式（每 8px 采样中线+半宽+弧长）从骨骼链导出（10 §⑨.1 的"UE4 骨骼导出成什么格式"答案，适用于 spine 同理）。
- **收益**：攻击特效方向链（δ=+7.81° 十轮定案史）的每一环都源于手工几何取证；自动化后重建即校验（构建日志打印 hull 轴角/枪口距，偏离即 FAIL）。
- **铁律**：所有数值仍来自 ling.json 资产直读，符合三链路。

### P1-4 命中特效 hit_fx 升级：解析火花 + 三层光束 + 环脉冲
- **来源**：02 T13（card_edge.gdshader:131-155 解析火花：`at=start+dir*speed*age+vec2(0,260)*age²`、速度方向尾迹 seg SDF——无粒子系统）；05 §1.3（Beam 三层法：外 2.6×宽 α0.25/中 1× α0.85/芯 0.36× 近白 α0.95，FxKit.cs:939-1051；Ribbon WidthCurve (0,0.05)(0.7,1)(1,0.6)）；05 §1.3 RingPulse（sigil shader+缩放/alpha/spin）。
- **落点**：DyeFramework.cs HitBurstFX 八发射器模块（条目 57）——E4 射线×7 可换三层 Beam 结构增质感；E5 火花爆可增"解析火花"变体（shader 化，省 CPUParticles 配额，且规避条目 42/55 的 SubUV 方形网格血案）。
- **收益**：hit_fx 观感从"贴图+粒子"升级出程序化层；解析火花随速度自动拉尾，天然规避"方块光效"类打回。
- **要点**：先做 Beam 三层（纯 Sprite2D 组合，半天量级）；解析火花作为 shader 试水（配 BackBufferCopy 不需要，普通 quad 即可）。

### P1-5 受击闪红升级：16-tap 轮廓提取 + 提亮共享
- **来源**：02 T6（echo_outline.gdshader:17-30：8 方向×2 半径=16 tap，`shape=max(a,Σ/16*1.25)`、`glow=shape-core*inner`——比 Sobel 便宜、无方向伪影）；11 §C8（MurderFx 受害体提亮 whiten=2.6、LightGroup 多 blow 取 max 共享，MurderFx.cs:748-872）。
- **落点**：DyeFramework.cs HitFlash（现=整卡染色闪红）；升级为"轮廓发光+本体提亮"两层：轮廓层用 16-tap shader（读我们的 atlas 纹理 alpha），本体层维持现有 modulate 路径。多段攻击提亮取 max 防叠加爆白。
- **收益**：受击反馈从"糊一层红"变"边缘亮起+微提亮"，与 STS2 原生 hitspark 语义更贴。
- **要点**：silent profile 不动（ HitFlash `/ling/` 过滤已有先例，条目 45③ v_col.rgb 不乘的隐患口径保留）。

### P1-6 光照滤镜阶段：shadow_shade 区间压暗方案
- **来源**：02 §①#15（shadow_shade.gdshader：from_x/to_x/soft/deep/low/lean/amount——纯几何区间遮光+高度渐变+地面投影后移；09 §2.8 天幕双层 roof_dim=0.78/0.45 双 quad 用法）；11 §② D4（ExposeWipe 与游戏 UI 逐帧协商协议）。
- **落点**：光影层计划阶段 2 未完成项"光照滤镜的事件驱动近似"（交接 §7②）——现有 ling_holo 四变体已备 point/ambient/fog/darken/flash uniform 全关待用（光影层计划条目表 STS2 实现行）；shadow_shade 提供"事件驱动区域压暗"的现成几何方案（Boss 大招时对玩家区间压暗+渐变），比全屏 Darken 更有戏。
- **收益**：光照滤镜从"uniform 备好没接线"推进到"至少一条事件链点亮"；触发时机按铁律口径只对齐效果语义不对齐手游时序（光影层计划 §关键路径既有结论）。
- **要点**：先接一条最便宜的事件（如敌人 Intent 出现→区域压暗 0.3s），验收后再扩。

### P1-7 影子体系：噪声蚀边软影（雾家族 v1 段）
- **来源**：02 §3.2 雾家族演化（shade_mist v1：解析形状场+噪声蚀边+中空+边缘紫光——**全部参数静态**，是雾家族最便宜的一档）；P0-5 公共段复用。
- **落点**：DyeFramework.cs 影子层（现=SpineSprite 镜像拉扁压黑，光影层计划阶段 1 已定案贴图斑方案）——可选叠加：影斑边缘用噪声蚀边替代硬 alpha 截断（quad 叠在影子下，edge=椭圆解析场，wisp 控破碎度）。
- **收益**：影子从"纯黑椭圆"到"有墨水质感的软影"，成本=一个静态参数 quad。
- **要点**：保持可开关（config 键 shadow.ink=0 回退旧版，0=回到旧版惯例）；silent profile 的影子观感属皮肤链，默认不动。

---

## 3. P2 清单（记录备查，按需取用）

- **P2-1 入场姿势插值栈（KEYS 模式）**：10 §4.1（wraith_play.gd KEYS 6 姿势×39 参数+`x_at(t)` 三层叠加：线性基座+三角函数叠加+指数过程量，三层正交可整体缩放）；我们 Ling 战斗入场若要从"纯染色"升级为"姿势程序动画"（idle→attack 姿势插值），此架构直接套用；姿势数值从我们自己的 ling.json 动画轨道采样（资产链），silentFX 只提供架构。备查。
- **P2-2 WZ_HEAD 双份绘制协议**：02 T11 / 10 §3.5（同物体画两份、`a*(1-w)/max(1-wz_outer*a*w,1e-4)` 份额和恒等于画一次，数学证明在 10 §⑦.12）；当未来出现"特效要盖 UI 又要留在角色层"需求时是标准答案；备查。
- **P2-3 ReduceScreenEffects 单点收口**：05 §1.5（AccessibleColor + Burst×0.65/RingPulse×0.55/Flash×0.35 三处局部减量，收口 4 行）；我们做"减少闪光"可访问性开关时照抄此结构。备查。
- **P2-4 追赶压缩公式**：09 §1.4（`due=0.18+(At-t0)*k, k=min(1,budget/span)`，TimeFreeze.cs:1279-1345）；若将来做时停/慢动作类演出，这是"只改表现不改事实"的完整配方（含重放配方 Recipes、_replaying 重入守卫）。备查。
- **P2-5 帧序列+锚点导出模式**：03 §4.1 / 10 §⑥（suppress_frames：fps/scale/size/ground/faces + 每帧 f/c/h/sh/f5）；我们目前全 Spine 实时渲染无帧序列需求；若未来预渲染任何位图序列（如选人大特效），按此格式同期导出锚点。备查。
- **P2-6 音频切片经济学**：07 §②③（Sfx Option 参数模型：Sample/From-To/Gain/Speed/Lead/Via/Soft/Tail/With；Bed sin² 包络 Weave:2088-2131；Pop 四音素查表:186-190；Riffle 幂律排布:855-879；总线限幅 -1.5dB）。**触发条件=我们引入 UE4 音频资产**（当前 LingChar 无音频）：一个 OGG 切多段当多音效用（From/To/Gain 参数表），动画对齐用 Lead 提前量（与 attack fx bursts=[0.166667] 的节拍思想同构）；程序化排布（Riffle 式固定种子+幂律间隔）可零素材生成复合音。备查。
- **P2-7 FrameDiagnostics 式自计**：05 §3.5（每 5s 帧率/长帧/分配/GC/回调 Top5 CPU，Tuning 开关默认零开销）；我们的 HITCH 监视器（条目 36/37）已有帧级证据，补一个"回调 Top5"即可归因到函数。备查。
- **P2-8 Tour 式脚本化验收**：04 §①（TourScript：INI 站点+文案双文件+VerifyPile 后置校验）；我们十轮验收靠用户人工打回，若特效数量继续增长，"一站一个特效+声明式夹具"的巡演可做回归。备查。
- **P2-9 双签名委托择一**：06 §5.3（RuntimeApiCompatibility：official/beta 双委托+参数个数绑定+调用点无分支）；STS2 游戏更新改 API 时（decomp 基于 v0.107.1）我们的 Harmony patch 层可借用此模式做版本垫片。备查。
- **P2-10 让位协议**：11 §⑦.10（Replaces/CoversStab/CoversBite 等 10+ 查询口构成"mod 与游戏动画仲裁"）；我们与第三方 mod（BaseLib shim 已有）冲突时的协商范式参考。备查。

---

## 4. P3 清单（明确不照搬 + 理由）

| # | 项 | 理由 |
|---|---|---|
| P3-1 | FSB 银行拆解（GameSounds）+ FMOD 导入表钩子（FmodNative） | 我们的音频来源是自有 UE4 资产（可解出 PCM/OGG 直接进 Godot），不存在"只能从游戏银行偷样本"的约束；Win32 导入表钩子只适用于"拿不到官方 API 又要控宿主音频"的 mod 场景，07 §⑨ 反面提醒明确指出不应照搬。 |
| P3-2 | MP 四层同步（AimSync 租约协议/NormalizeTypes 消息表注入/RemoteFxFade 等） | LingChar 无多人特效同步需求；且 06 报告实证：目标游戏构建缺 PeerVersionInfo，silentFX 的 Exact 链**从未实跑**，协议正确性无生产验证——照抄等于抄一份未验证代码。仅 NormalizeTypes 的"list 为真相源+防合并宁可放弃"两条原则值记录（P2 级思想）。 |
| P3-3 | HandMoves/HandFlow/HandHold 手牌编排全家桶（3041+423+391 行） | LingChar 是角色+特效 mod，不改手牌系统；事件总线模式在将来做"牌池特效"时可取 HandFlow 单件（嵌套会话栈+600ms 残留窗+逐订阅者容错），其余不搬。 |
| P3-4 | suppress_stage.gd / wraith_play.gd 舞台内容整体移植 | 撕魂/女鬼是 Silent 角色专属叙事；Ling 有自己的观感语言（枪口/镜瞳/溶解）。只搬其中的数学（P0-2 addp、P2-1 KEYS 架构），不搬内容与美术基调。 |
| P3-5 | GameCapabilities IL opcode walker | 我们锁定的游戏版本固定（Steam v0.107.1，decomp 齐备），行为级探测无必要；版本更新时先用 P2-9 双签名委托，仍不够再考虑。 |
| P3-6 | 49 shader 全量搬运 | silentFX 观感语言（红雾/毒潮/幽魂绿）与 Ling 观感语言（蓝紫/镜面/冰蓝）不同族；只搬共用数学（P0-5 公共段）与个别公式（T5/T6/T13），其余保持"配方库"身份按需取。 |
| P3-7 | malaise_strand 式死资产共存 | silentFX 反面教材（v0.64 已删 strand 却仍随包发布，08 §2.2）：我们构建期已有 verify_pck 系列逐项校验惯例，新增资产必须同步登记，不留死文件。 |

---

## 5. 九大专题速查（任务书指定对照面 → 对应条目）

| 专题 | 我们现状 | silentFX 做法 | 结论/条目 |
|---|---|---|---|
| tuning.ini 配置模式 | hit_fx_ling.json/attack_fx_ling.json 平铺键+散落 C# 默认+`_note_*` 零星 | 代码 def 兜底+ini 精选+注释即文档+0=回旧版+F8 热重载+保险参数 | **P0-3 直接移植机制**（非数值）；P1-1 接热重载 |
| 溶解公式 | UE4 逐行公式已解码定案，ling_dissolve_mix=单噪声阈值 discard+边缘带 | her.gdshader 双频 0.75/0.25+阈值带+三色亮边带（数值 gdshader 直读） | **P0-1**：UE 公式为主，双频/亮边做增强层或等效补偿 |
| 影子体系 | SpineSprite 镜像拉扁压黑（贴图斑方案定案） | 雾家族五级演化，v1 shade_mist=解析场+噪声蚀边，全静态参数 | **P1-7**：只取 v1 档软边；P0-5 供噪声段 |
| 入场特效 | entry_player.gd 染色时序（[LingSelect] 埋点） | wraith KEYS 6 姿势×39 参数三层插值栈+phase() 接棒 | **P2-1**：架构备查，数值走自家 spine 轨道采样 |
| 预热 | SelectWarm v1→v8.2 全史（池化定案）+HITCH 监视器 | AssetWarmup 分帧队列+8ms 预算+ShaderWarmup 离屏 2 帧+warm() 分帧 | **P1-2**：补战斗资产段，实例级卡顿已有定案不重做 |
| DevPreview | 无（十轮验收全靠实机） | 四级工具（预览/测试台/巡演/展示）+dev_preview.txt 发布卫生 | **P1-1**：先做一级（预览台），F8 热重载闭环 |
| rig.json 中间层 | muzzle_hull/muzzle_bone 手工解析注入（条目 48/52） | rig.json：strand 中线折线+半宽+弧长；layer 关节锚点 | **P1-3**：构建期自动化泛化，strand 格式留档 |
| suppress_frames 锚点 | 无帧序列需求 | 帧+锚点+ground/faces/f5 同期导出，C# 只读 7 字段 | **P2-5**：备查格式规范 |
| 音频切片 | 无音频资产 | Option 参数模型+Lead 对齐+总线限幅+程序化排布 | **P2-6**：条件触发（引入 UE4 音频时启用） |

### 5.1 专题深析一：配置体系（tuning.ini ↔ hit_fx_ling.json）

**silentFX 的四层结构**（03_notes §②，全部机制可从源码直读）：
```
第 4 层（最低）：C# 代码内置默认 —— Tuning.F(key, def)，键缺失/类型错/文件缺全静默回退
第 3 层：       tuning.ini —— 作者发布层，652 键带中英注释，"ini 是精选入口、代码是全量默认"
第 2 层：       user_settings.ini —— 玩家层（仅 9 键，保留未知键+原子写）
第 1 层（最高）：sfx/choices.ini —— 面板手选，胜过 tuning 同名键
```
代码侧实际可调面 1029 键（=ini 652 + 462 隐藏键，01 §④.7），任何键删掉都不影响运行——**配置即版本史**（0=回到旧版惯例贯穿全库）。

**我们现状的差距**：hit_fx_ling.json 与 attack_fx_ling.json 已具备"JSON 键+C# 兜底"雏形（attack fx "缺失键走代码默认值"是条目 54⑤ 的明文口径），但有三处未成体系：① 默认值散落在 DyeFramework.cs 各构造点，无一键一处的登记表；② `_note_*` 只有 dir_rot_fix/flash_life 两处，其余键的"为什么是这个值"不可考；③ 无热重载——每改一个 α 值都要重建+实机。

**移植方案**（P0-3 + P1-1 合并实施）：
1. `FxTuning.cs` 单件：Load（无文件静默）→ F/I/B 三读取 API（def 兜底+Clamp 区间内联）→ Reload（F8 或热键触发，重发 Changed 事件，监听方各自淡出重建——照抄 PlayerSettings.Changed 的逐个 try-catch 广播，01 §4.3）；
2. JSON 结构升级：每特效一个 section（对照 tuning.ini 的 per-card section），`_note_<key>` 紧跟 `<key>`；构建期 build_lingchar.py 校验"每个非 _note 键必须有 _note"（可做成 WARN 起步）；
3. 开关键强制 `0=旧行为` 语义并在 _note 写明旧版是哪一轮（对照 03_notes §3.5 条 32）；
4. 每个异步序列（hit_fx 双脉冲、死亡溶解+粒子、attack 两拍）必须有 `*_wait`/`*_timeout` 收尾键——silentFX 保险参数文化的直接移植。

### 5.2 专题深析二：溶解与死亡观感（her.gdshader ↔ ling_dissolve_mix）

**UE4 侧已定案**（光影层计划条目表"溶解公式源码解码 ✅"行）：ps1=世界坐标投影噪声（pc5_h[1] 缩放）、阈值=pc5_h[2].z、`ceil((noise+off−T)·(noise+off−1))` 双门+discard（pc5_h[3].y 门控）、边缘=mix(→pc5_h[0].xyz=liangbian_color)。这是**必须逐行复刻的主干**。

**silentFX 的增强层**（全部数值 her.gdshader:45-47 / shade_mist.gdshader:94 直读）：
- 双频混合让侵蚀有"两个世界"的层次：高频细节 + 低频大势，单频噪声的溶解边缘总显得"均匀地烂"；
- 亮边带宽用两个 smoothstep 之差构造（内沿 dn-0.08、外沿 dn-0.25），比单 smoothstep 的窄带更有"能量沿缘流动"感；
- suppress_stage.gd:498-501 展示了每帧驱动面：presence（存在度）/ghost（拖影）/dissolve（溶解量）/smear/mist/t/light_at 七参数联动——我们的 DeathWatcher 只驱动 dissolve 一个 uniform，可低成本加 presence（溶解前 0.1s 微微透明化）。

**死亡粒子配方**（08 §1.3 + 10 §2.5 直读）：
- 灰碎=Crumble 瞬间两段 Burst：16 片大火星（快、寿命短）+ 6 片慢灰（慢、灰化色 Lerp 向 Ash）——"爆发+余烬"两拍结构；
- `addp()` 阻尼解析粒子：`pos=p0+v(1-e^{-ka})/k`，配合速度拉伸 `st=clamp(|v|/2600S,0,0.35)`（到达瞬间反向 -0.18 制造顿感，suppress_stage.gd:669-670）；
- 时序锚点：撕断 Kick→顿 0.3s→Recover 0.45s smoothstep 回位（08 §1.5），对应我们死亡序列=溶解 tween 0.5s（已定案常量 0.5/1.2/0.05）内前 0.15s 放快灰、后 0.35s 放慢灰。
- **合规注**：16/6、双速、0.15/0.35 拆分均为 silentFX 结构参考值；最终数值以 UE4 真值链（FIGHTER_DEATH_DISPLAY_EFFECT_CFGID → particles 属性转储）优先，查不到的再按"结构对齐、数值设计标定"登记（条目 54⑤ 口径）。

### 5.3 专题深析三：影子与雾（雾家族演化 ↔ SpineSprite 拉扁压黑）

02_shaders §3.2 的五级演化路径本身就是一份**升级路线图**，我们不必走完，取两档：

| 档 | silentFX 实现 | 映射到我们 | 成本 |
|---|---|---|---|
| v1 蚀边软影 | shade_mist：解析形状场+`1-smoothstep(0.12,1.08,edge+(n-0.5)*wisp)`+中空+边缘紫光，全静态参数 | 影子 quad 叠加层：edge=椭圆场（照 SpineSprite 影斑轮廓），wisp 控破碎度 | 一个静态参数 shader，半天 |
| v4 场景化 | toxic_fog：噪声钉死 canvas 坐标（quad 任意移动不拉伸）+bank 贴地长堤 | 若做"地面技能范围圈/毒地带"类卡牌特效，这是防拉伸的标准解 | 备查 |

不取 v2/v3/v5（48/40 puff 动态雾团+六数组上传）——那是为"角色化身雾"设计的动态体，我们的影子是静态贴图斑，上了也是空转；等将来做" Ling 化雾形态变身"类特效再回头取 T4 幂混合公式（`c=pow(thick,2.5)*exp(-2.5*d2); field=pow(sum,1/2.5)`，重叠不肿胀）。

### 5.4 专题深析四：预热体系（SelectWarm 之痛 ↔ silentFX 三件套）

我们 SelectWarm v1→v8.2 七轮迭代的教训链（条目 31-39）：
- v1 CallDeferred 不 flush → v2 同步 AddChild 不激活 → v3 ProcessFrame 事件订阅（可用通道）→ v4-v5 发现卡在 C# 之外 → v6 α=0 可能被剔除 → **v7 定案 ~350ms 全在绘制提交段且每实例重付** → v8 实例池根治。

silentFX 的对应解法覆盖了我们没有的两块：
1. **AssetWarmup 分帧队列**（01 §4.6）：每帧只跑一个 work、CanRun 前置检查（非战斗+未暂停+0.5s 安静期）、分步任务（Step 返回 false 下帧续）、**8ms 预算披露**（超时打日志，慢单元显形）、排空统计——这是"预热有没有盖住"的可观测闭环，我们的 SelectWarm 没有；
2. **离屏视口 2 帧法**（ShaderWarmup.cs:150-234：32×32 SubViewport 挂四种节点类型共享材质渲染 2 帧；WraithSceneWarmup 用 ≤960×540 真场景预演）——**针对 v7 定案的"绘制提交段"卡顿**，战斗资产（hitfx 五纹理+dissolve/holo shader+attack fx 纹理）可在主菜单期用小视口真实绘 2 帧，把 PSO/纹理上传挪出战斗首帧。

**边界诚实**：v7 已证明 spine 网格首建是实例私有的，离屏预热救不了战斗内 SpineSprite 首绘——那部分要等战斗侧 HITCH 数据决定是否做"战斗骨架离屏预演"（WraithSceneWarmup 同法，P2 级）。本条目只做资源级（P1-2）。

### 5.5 专题深析五：预览台（十轮打回之痛 ↔ DevPreview 四级体系）

silentFX 四级=DevPreview（孤立假人）→TestBench（真实战斗快照还原）→Showcase（全量自动回归+原速/慢速双放）→Tour（叙事验收）。我们的对应缺口与最小路径：

| silentFX 层 | 我们的最小实现 | 优先级 |
|---|---|---|
| DevPreview | DevPreviewLing：CanvasLayer 假人（两个 ColorRect 站位）+热键触发 hit_fx/attack_fx/死亡序列/影子/闪红 + F8 重载 + F5 慢动作（Engine.TimeScale）+ dev_preview.txt 开关 | **P1-1** |
| TestBench 式快照还原 | 暂不需要（我们不改牌堆/HP） | — |
| Showcase 式自动回归 | 预览台加"循环全部特效"键即够 | P2 |
| Tour 叙事验收 | 备查（P2-8） | P2 |

**设计细节照抄三点**（04 §①，机制直读）：① 热键层每步 try-catch+单项日志，坏一步不拖垮其他（silentFX "初始化即降级"范式）；② F9/F10 条目翻页遍历特效注册表（我们=FxCfg 各 section 枚举）；③ 发布卫生：`dev_preview.txt` 不进 pck（build_lingchar.py 装配清单天然不含游戏 Mods 目录文件，只要代码按"文件不存在即全休眠"实现）。

**收益量化**：条目 41→56 十六轮中纯观感调整约 10 轮，每轮节省"重建+进局+截图"≈5 分钟且反馈更即时（可慢动作逐帧看）——一个迭代日内回本。

### 5.6 专题深析六：锚点/rig 中间层（手工 muzzle_hull ↔ rig.json 生态）

silentFX 有两个互相印证的"数据化骨架"样板：
- **suppress_frames.json**：154 帧 PNG+每帧 f(前拳)/c(胸拳)/h(头)/sh(前肩) 四锚点+ground/faces 顶层锚——「位图管视觉、JSON 管锚点」，C# 只读 7 个字段（10 §⑥.2 的消费清单）；
- **rig.json/rig2.json**：25 条 strand（中线折线 pts+左右半宽 left/right+累计弧长 arc+贴图偏移）+ 25 个 layer（贴图+关节锚点 elbow/fist/base/tip/foot）——GDScript 侧零硬编码坐标全从 rig 读（10 §⑤）。

我们的对照现状：muzzle_hull（四顶点+deform）、muzzle_bone（slots 段解析）、muzzle_dir（轴向量）已由条目 48/52 手工从 ling.json 取证后写死进 config——**数据是对的，但取证过程不可复现**（diag_dir6/7/8 系列一次性脚本）。

**泛化方案**（P1-3）：build_lingchar.py 新增 `export_anchors()` 段：
1. 读 ling.json `skins[0].attachments`：每个 mesh 附件算 hull（四角法，条目 51④(c) 已验证的数学：四顶点/2=顶点数、|U|/atlas 尺寸校验平行四边形）；
2. 读 `slots` 段：slot→bone 映射（qiang02→bone104 先例）；
3. 输出 `anchors_ling.json`（LingChar.pck 自有路径）+ 构建日志打印每个 hull 的轴角/长度（与 config 注入值对账，偏离即 FAIL——条目 52④ 构建期打印 predicted_screen 的推广）；
4. muzzle_hull/muzzle_bone/muzzle_dir 三键改由该段自动注入（config 保留人工覆盖位，`_note` 注明"构建期自动计算，人工值优先生效"）。

第二步（strand 导出，视需求启动）：若做披风/长发类程序摆动特效，按 spine 骨骼链每 8px 采样中线+从蒙皮宽度估半宽，格式照 rig.json strand 五字段——spine41_parse 已有骨骼解析基础，增量小。

### 5.7 专题深析七：音频（UE4 音频资产 ↔ Sfx 三层）

当前 LingChar/LingSilentSkin 均无音频资产，本专题**条件触发**：一旦决定给 Ling 加受击/入场/死亡音效（UE4 侧资源库里应有可解出素材），silentFX 的资产经济学直接可用：

1. **切片复用**：一个 OGG/WAV 用 Option 参数表（From/To 截取秒、Gain、Speed 变速）切成多个音效位——silentFX 41 槽 145 候选只用约 132 个游戏样本、一个 blade 样本切三段当三个刀音（07 §②.3 设计模式读法）；UE4 导出时按"整段+参数表"交付，不预切文件；
2. **Lead 提前量**：`PlayAt 用 wait−Lead/Speed 补偿`让响点落在动画时刻（07 §③.2）——与 attack fx 的 bursts=[0.166667] 开火时刻对齐是同一问题，音效侧用 Lead、视觉侧用 bursts，两表共用 config；
3. **总线限幅**：Master 挂 limiter（sfx.ceiling=-1.5dB 直读参考值）防多音效叠加爆音——STS2 游戏自身音频架构不明，mod 自建总线是安全做法（07 §③.2 Out 限幅链）；
4. **程序化排布**：Riffle（固定种子+幂律间隔+钟形增益）零素材生成洗牌类复合音——若 Ling 牌组需要洗牌/翻牌音，这是零资产方案。

不搬：FSB 拆解/FM OD 钩子（P3-1，理由见 §4）。

---

## 6. 如果只做三件事（Top3 论证）

### Top1：FxTuning 调参层 + DevPreviewLing 预览台（P0-3 + P1-1）
攻击特效 41→56 条目共十六轮迭代、每轮"重建→实机→截图打回"，是全项目最大的时间黑洞；hit_fx 移植（条目 57）后特效面还会继续扩张。silentFX 的答案被其作者用 462 个隐藏键+四级工具验证过：**调参数据与代码分离、注释即文档、热重载秒级看效果**。两件合起来约 1.5 个工作日，此后每一次观感迭代都省半小时起步——这是复利最高的投入。实施顺序：先 FxTuning（无 UI 依赖），再预览台（依赖 Reload）。

### Top2：溶解双频 + 灰碎死亡粒子（P0-1 + P0-2）
这是光影层阶段 3 唯一没闭环的观感尾巴（公式已解码、管线已通、观感停留在"通用近似"）。her.gdshader 双频+亮边是 silentFX 全库被复用最多的溶解配方（suppress_stage 她的魂身同款），数值可从资产直读、合规零风险；灰碎两段式给死亡收尾一个"重音"。合计约 1 个工作日，直接可验收（死亡观感三段对照法在光影层计划阶段 3.6 已备）。

### Top3：构建期锚点/rig 侧车导出（P1-3）
方向链十轮定案史（条目 48-53）证明：几何真值手工取证既慢又易引入伪影（θ_top 顶缘污染、单缘/双缘法反复）。muzzle_hull 已趟通"构建期从 ling.json 注入"的路，把它泛化为通用锚点导出（slot 绑骨/attachment hull/deform/常量锚一次算齐+构建日志对账+偏离即 FAIL），此后任何新特效（枪口外的手部/脚部/头部件）都免手工取证。约 1 个工作日，兼作 P1-3 第二步 strand 格式的地基。

---

## 7. 附 0：执行注意事项（铁律落点）

1. **Mimosa 钩子**（交接铁律 4）：所有新 .cs/.gdshader/.json 落盘用 Write/Edit 工具，不走 Bash 重定向；指纹一律 SHA-256；build_lingchar.py 改动后先跑 `tools/toolchain_check.py`（FAIL=0）再构建；重跑构建前 `mv icon_import_scratch scratch_old_<ts>` 让位。
2. **去污染验证**（铁律 2）：每轮新增资产后跑交集校验（LingChar.pck ∩ LingSilentSkin.pck = ∅），新 shader/config 路径全部落在 drc_dye/profiles/ling/ 或 pcksrc 自有新子目录；构建期受控同步链（relocate_asset/md5 产品名）不受影响；drc_dye 的 silent profile 默认不动。
3. **数值登记**（铁律 1）：凡从 silentFX 引入的常数（附 A 全表），在对应 config `_note_*` 与主日志条目中登记来源="silentFX 资产/源码直读（文件:行号）"，与「UE 真值」「设计标定」三类分开——延续条目 52③ `_note_dir_rot_fix` 口径；严禁用 silentFX 运行画面反推任何数。
4. **基线管理**：三件套改动后按惯例重基线（tools/knownGood + toolchain_manifest.json + verify_pck_N.py 断言），verify 脚本编号顺延。

## 8. 附 A：silentFX 引用数值来源登记表（铁律 1 合规清单）

下表汇总本报告引用的全部 silentFX 侧常数及其可直读位置（均为反编译源码 split/*.cs 或 mod 资产 gdshader/gd/json，属铁律 1「源码/资产」链路）：

| 常数/公式 | 值 | 来源（可直读位置） | 用于条目 |
|---|---|---|---|
| 双频溶解混合比 | nn(q1)*0.75 + nn(q2)*0.25 | wraith/her.gdshader:45-47 | P0-1 |
| 溶解阈值带 | smoothstep(dn-0.08, dn+0.08, dissolve) | wraith/her.gdshader:46 | P0-1 |
| 亮边色/带宽 | vec3(0.30,0.55,0.52)，带 dn-0.25→dn-0.08，×0.8 | wraith/her.gdshader:47 | P0-1 |
| 噪声蚀边 | 1-smoothstep(0.12,1.08, edge+(n-0.5)*wisp) | shaders/shade_mist.gdshader:94 | P0-1/P1-7 |
| mist() 三层域扭曲 | warp 系 1.6/0.8、频率 2.3/4.6 | shaders/malaise_mist.gdshader:72-78 | P0-5 |
| fbm 旋转矩阵 | vec2(1.6x+1.2y+3.1, -1.2x+1.6y+1.7) | 上述约 20 个 shader 头部（02 §3.1） | P0-5 |
| puff 幂混合 | pow(thick,2.5)*exp(-2.5*d2)，field=pow(sum,1/2.5) | shaders/shadow_cloud.gdshader:116 | §5.3 |
| 解析弹簧 | ζ=-ln(over)/√(π²+ln²over)，ω_d=π/peak | split/SilentFX.Cards.GambleFlow.cs:656-677 | P0-4 |
| 半隐式欧拉参考参数 | k=220/c=21.4，子步 h≤0.006、n≤24 | split/SilentFX.Cards.MalaiseFx.cs:2141-2159 | P0-4 |
| 临界阻尼冲量 | 速度注入=位移×2√k | MalaiseFx.cs:2071/2082 | P0-4 |
| 稳定哈希 | frac(sin(i·127.1+311.7)·43758.547) | split/SilentFX.Core.FxKit.cs:1053-1057 | P0-4 |
| 多指数瞬态 | 0.4·e^{-t/0.06}+0.1·e^{-((t-0.11)/0.035)²} | MalaiseFx.cs:3262 | P0-4 |
| 灰碎两段 | 16 片大火星+6 片慢灰 | MalaiseFx.cs:1895-1896 | P0-2 |
| 阻尼解析粒子 | pos=p0+v(1-e^{-ka})/k；速度拉伸 clamp(|v|/2600S,0,0.35) | suppress3/suppress_stage.gd:1210-1264, :669-670 | P0-2 |
| 解析火花 | at=start+dir·speed·age+vec2(0,260)·age² | shaders/card_edge.gdshader:131-155 | P1-4 |
| Beam 三层 | 外 2.6×α0.25 / 中 1×α0.85 / 芯 0.36×α0.95 | FxKit.cs:939-1051 | P1-4 |
| Ribbon 宽度曲线 | (0,0.05)(0.7,1)(1,0.6) | FxKit.cs:243-298 | P1-4 |
| 16-tap 轮廓 | shape=max(a,Σ/16*1.25)，glow=shape-core*inner | shaders/echo_outline.gdshader:17-30 | P1-5 |
| 受击提亮参考 | whiten=2.6，多 blow 取 max 共享 | split/SilentFX.Cards.MurderFx.cs:748-872 | P1-5 |
| 区间压暗 | from_x/to_x/soft/lean/amount；双层 0.78/0.45 用法 | shaders/shadow_shade.gdshader；split/ShadowStepFx.cs:2154 附近 | P1-6 |
| 预热预算 | 每帧一任务、8ms 披露、0.5s 安静期 | split/SilentFX.Core.AssetWarmup.cs:73-160 | P1-2 |
| 离屏预热 | 32×32 SubViewport 渲 2 帧 | split/SilentFX.Core.ShaderWarmup.cs:150-234 | P1-2 |
| 慢动作 | F5=0.15x（dev.slow_motion） | split/SilentFX.Dev.DevPreview.cs:259 | P1-1 |
| 追赶压缩 | due=0.18+(At-t0)*k | split/SilentFX.Cards.TimeFreeze.cs:1279-1345 | P2-4 |
| 音频限幅参考 | ceiling=-1.5dB | 3817850774/tuning.ini:988-990 | P2-6 |
| reduce 减量 | Burst×0.65 / RingPulse×0.55 / Flash 分支 ×0.35 | FxKit.cs:497-500/868-871/1059-1073 | P2-3 |
| WZ_HEAD 份额 | a*(1-w)/max(1-wz_outer*a*w,1e-4) | wraith/wraith_form.gd:698-716 | P2-2 |

## 9. 附 B：P0/P1 实施顺序与工作量估计

```
第 1 步  P0-3 FxTuning        0.5 天   （无依赖；顺带给现有键补 _note）
第 2 步  P0-4 FxMath 数学库   0.5 天   （纯 static 函数，无依赖）
第 3 步  P1-1 DevPreviewLing  1.0 天   （依赖 FxTuning.Reload）
第 4 步  P0-1 溶解双频        0.5 天   （shader 单文件，预览台里验收）
第 5 步  P0-2 死亡粒子        0.5 天   （同上）
第 6 步  P0-5 噪声公共段      0.25 天  （合并进第 4/5 步顺手做）
第 7 步  P1-3 锚点导出        1.0 天   （build_lingchar.py；verify 脚本同步）
第 8 步  P1-2 战斗资产预热    0.5 天   （SelectWarm 扩展）
第 9 步  P1-4/P1-5/P1-6/P1-7  各 0.25-0.5 天，按验收反馈排
```
全程遵守交接铁律 3（改脚本先跑 toolchain_check）与条目惯例（verify_pck_N 编号顺延、knownGood 重基线）。

## 10. 附 C：本报告与 11 份挖掘报告的引用索引

- 01_architecture：§③补丁范式、§4.3 配置双层、§4.6 AssetWarmup、§④.7 隐藏键差集 → P0-3/P1-2
- 02_shaders：T3/T4/T5/T6/T13/T15、§3.1 公共段、§3.2 雾家族、§4.1-4.3 管理机制 → P0-1/P0-5/P1-2/P1-4/P1-5/P1-7
- 03_notes：§①② 配置四层、§3.5 调参哲学、§4.1 suppress_frames → P0-3/P1-3/P2-5
- 04_devtools_assets：§①② 四级工具、§③ 方法论 → P1-1
- 05_fx_core：§1.2-1.5 FxKit 原语、§2.1 Spec/Shaper、§3.4 预热、§⑥ 20 模式 → P0-4/P1-4
- 06_mp_compat：§结论 3（Exact 链未实跑）、§5.3 双签名 → P3-2/P2-9
- 07_audio：§② Option 模型、§③ 播放链、§⑨ UE4 价值评估 → P2-6/P3-1
- 08_cards_malaise_gamble：§1.3 Orb 灰碎、§1.5 Slump、§4.1-4.2 缓动/弹簧族、§6.1 演化史 → P0-2/P0-4
- 09_cards_shadow_time：§1.4 追赶压缩、§⑤ 技法表 → P2-4/P1-6
- 10_gd_stages：§① 握手协议、§⑤ rig 格式、§⑥ suppress_frames、§⑨ 移植价值 → P0-2/P1-3/P2-1/P2-5
- 11_cards_sweep：§⑥ 共性 16 条、§⑧ Top20、D4 ExposeWipe、C8 MurderFx → P1-5/P1-6/P2-10

*报告完 · 12_transplant · 2026-10-16*
