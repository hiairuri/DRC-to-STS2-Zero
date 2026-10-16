# SilentFX 勘察报告 ④：开发工具与资产生成链（初探第一轮）

> 勘察代理：recon-devtools ｜ 日期：2026-10-16 ｜ 材料：`silentfx_decomp/split/`（C# 反编译）、`3817850774/`（mod 资产目录）
> 结论先行：**SilentFX.Dev 是一套「游戏内特效调试台 + 自动演示 + 脚本化巡演」四级工具体系**，由文本文件（tuning.ini / 展示_脚本.txt / 展示_每张牌一句话.txt）数据驱动、支持热重载；资产侧采用「代码生成（Python 原型→GDScript）+ rig.json 数据化骨架 + 贴图三代拷贝-重绘 + 帧序列带骨骼元数据 JSON」的工业化组织。作者奶酪（Claude Opus 协作）的方法论可概括为：**外置调参、夹具化测试、快照还原、原型生成、双轨对照**。

---

## ① Dev 命名空间全景（16 个类型）

初始化顺序见 `SilentFX.Entry.cs:38-41`：DevPreview → Showcase → TestBench → TourController，四者互斥协作（任一 Active 时其余让位，如 `DevPreview.cs:108`、`Showcase.cs:1068`、`TestBench.cs:240`）。全部日志写入 mod 目录 `silentfx.log`（`SilentFX.DevLog.cs`，500ms 定时 flush）。

### 进入方式总表（热键）

| 工具 | 进入条件 | 热键 |
|---|---|---|
| DevPreview | mod 目录存在 `dev_preview.txt` 文件开关（`SilentFX.Dev.DevPreview.cs:55`） | F6 显示/隐藏、F7 触发、F8 重载 shader+tuning、F9/F10 上/下一个、F5 慢动作（`DevPreview.cs:117-156`） |
| Showcase | 同上文件开关（`Showcase.cs:165`） | F4 播放/停止全部演示、F3 暂停、F7 重播本个、F9/F10 上/下一个、F6 下一组（`Showcase.cs:1062-1116`） |
| TestBench | **默认开启**（`Tuning.I("bench.on",1)!=0`，`TestBench.cs:216-224`），战斗中自己回合点左上角「测试」按钮（`TestBench.cs:1152-1165`） | 纯按钮 UI + 慢动作开关（`TestBench.cs:1216`） |
| TourController | `Tuning.I("tour.on")` 或 `dev_preview.txt`（`TourController.cs:118`） | Ctrl+Home 启动、Ctrl+End 退出、Right/PageDown 下一站、Left/PageUp 上一站、Home 重播、End 字幕开关（`TourController.cs:122, 649-678`） |

### 各类型职责

| 类型 | 行数 | 职责 |
|---|---|---|
| **DevPreview** | 715 | 单特效预览台：遍历 `PowerFxManager.All`（26 个 power FX）+ `CardFxManager.All`（55 个 card FX）生成条目（`DevPreview.cs:69-84`），在 CanvasLayer(Layer 90) 上造两个 ColorRect 假敌人（`DevPreview.cs:474-494`）即时播放；开启 `PowerFxManager.LogTriggers`（`:58`）；F8 热重载 `ShaderLib.Reload()+Tuning.Load()`（`:138-143`）；`LogUnderMouse` 是 UI 遮挡诊断器（`:589-645`） |
| **Showcase** | 1374 | 全量自动演示：约 60 个条目按五批组织（能力牌/稀有能力/金卡/小刀系/毒系，`Showcase.cs:204-261`），每条目带 Setup/Apply/Trigger/Rearm/After 钩子自动摆场面；**原速+慢速两遍播放**（`:580-606`）；AutoSelector 自动代替玩家做选牌（`:37-50`）；字幕 UI 显示标题/批次/说明/阶段+进度条（`:1270-1289`） |
| **TestBench** | 1611 | 真实战斗手动测牌台：8 组 82 张卡点击入手牌照常打出（`:21-129, 1513-1559`）；一键给敌人 +3虚弱/+5中毒/+1人工制品+12格挡/残血/9999 血（`:1196-1211`）；升级版开关；**「选音效」页：5 个 tab、每音效槽试听/换候选/调音量/调间隔**（`:1242-1509`）；**「处理音乐」页：谋杀/子弹时间/幽魂/刺杀的音量·变远·变空·定格旋钮**（`:1293-1404`） |
| **TourController** | 979 | 脚本化巡演状态机：启动前置检查（必须单人、角色是猎人 Silent、指定存档位 `tour.profile`，`:344-398`）；等 TestBench 退出后接管（`:180-192`）；Shield 输入锁（Layer 110，`:919-959`）；**Protect：巡演中玩家免伤**（`:830-867`）；空闲回合每 0.5s 治疗（`:681-689`）；终幕 finale（`:698-729`） |
| **TourScript** | 573 | 巡演剧本解析器：读 `展示_脚本.txt`（INI 式 `[01 站名]`+键=值：手牌/抽牌堆/消耗堆/下回合抽到/能量/遗物/敌人血量/敌人/敌人回合/慢动作/分组/提示，`:36, 106-136`）+ `展示_每张牌一句话.txt`（`[01 站名]`+`奶酪 =`/`opus =` 对白，`[开头]/[结尾]` 页，`【视频…】` 插入点，`:297-423`）；**文件时间戳热重载**（`:38-54, 83-91`）；严格校验连号/站名一一对应/未知键（`:106-125, 566-573`） |
| **TourScene** | 347 | 摆站执行器：`EnterRoomDebug` 进固定战斗 InkletsNormal（3 只墨灵，`:151-153`）→ 清空所有能力/格挡 → 设 HP/能量/牌堆/敌人规则（中毒/虚弱/易伤/格挡/血量）→ 指定敌人下回合动作（攻击/连击/不攻击）→ **VerifyPile 逐堆后置校验，不一致抛错**（`:314-321`） |
| **TourUi** | 995 | AVG 字幕机：奶酪(金)/opus(橙) 双人对白渲染（`:6-8, 445-476`）；优先加载游戏自带思源宋体（`:291-322`）；逐行渐入动画（`:853-980`）；intro/outro/finale 翻页；FitPage 超屏自动缩字（`:789-833`）；雾效 shader 背景（`:401-407`） |
| TourDocument/TourDefinition/TourStop/TourLine/TourScreen/TourCard/TourEnemyRule | 6-28 | 纯数据结构：站点=手牌/抽牌堆/消耗堆/下回合规划+敌人规则+慢动作；TourCard 记录卡牌与升级态 |
| **TourSlow** | 89 | 慢动作管理：出牌时降速（`tour.slow` 默认 0.2），牌结算后 `slow_tail`(0.5s) 恢复（`TourSlow.cs:36-60`） |
| **TourTurnPlan** | 123 | 「下回合抽到」计划：摆站时挂起，到指定回合注入手牌（Arm/Cancel） |

---

## ② 「游戏内调试台」工作流推演

以「调 Wraith Form（幽魂形态）女鬼特效」为例，四级工具接力：

1. **DevPreview（孤立预览）**：不进战斗，F6 唤起预览层，F9 翻到 `[card]`/power 条目，假人+假敌人上直接看特效；F5 慢动作 0.15x（`Tuning.F("dev.slow_motion",0.15)`，`DevPreview.cs:259`）逐帧看；不满意就改 shader/tuning.ini，**F8 不重启游戏热重载**（`:138-143`）。
2. **TestBench（真实战斗）**：进自己回合点「测试」→ 自动快照当前牌堆/能量/HP/能力（`Snapshot()`，`TestBench.cs:554-588`）→ 敌人血量拉 9999、能量保持 9（`bench.energy`，`:593-596`）→ 点「幽魂形态」牌入手牌，照常打出；「选音效」页试听 41 个音效槽的候选并选定；「处理音乐」页实时压低调音乐音量/变远/变空试 3 秒效果。退出测试即完整还原（`Restore()`，`:801-832`）。
3. **Showcase（批量回归）**：按 F4，从第一批能力牌开始逐个自动播放（每项自动摆场面、拿起、指向、打出、等结算、还原），**每项原速一遍+慢速一遍**；F9/F10/F6 在条目/批次间跳转；结束后「这场战斗保存并退出后会从头重新开始」（`Showcase.cs:631`）。
4. **Tour（叙事验收）**：Ctrl+Home 启动巡演，按 `展示_脚本.txt` 的站号逐站摆出战斗（手牌/敌人状态/能量精确到个位数），右侧字幕滚动奶酪与 opus 的开发对白；Right 下一站；终幕打完 finale 显示结语。**观众视角 = 作者向同事/用户展示「这 mod 做了什么」的宣传片，同时是最终回归测试**。

互斥规则：Tour 启动时若 TestBench 在跑会等它退出并还原（`TourController.cs:180-192`）；Showcase 启动时若 TestBench Active 直接拒绝（`Showcase.cs:806`）。

---

## ③ 开发方法论提炼（Claude 式）

**Top 5：**

1. **四级预览体系，按「孤立→批量→真实→叙事」递进**。DevPreview 不需要进战斗（CanvasLayer 假人），TestBench 在真实战斗但快照还原，Showcase 全量自动回归，Tour 脚本化+文案。任何修改都可按成本最低的层级先验证。
2. **一切可调参数外置为文本 + 热重载**。`tuning.ini`（英文注释、按特效分节、`key = value`，`SilentFX.Core.Tuning.cs:7-33` 是极简 INI 解析器）管视觉/音频数值；`展示_脚本.txt`+`展示_每张牌一句话.txt` 管巡演夹具与文案。**改数值/改文案零编译**：F8 重载（`DevPreview.cs:138`），TourScript 按文件时间戳在切站时热重载（`TourScript.cs:38-54`）。
3. **快照-还原纪律（可弃实验场）**。TestBench/Showcase 进入即快照：牌堆归属 Home、抽牌顺序 DrawOrder、能量、HP、全部能力 Baseline、敌人 HP、遗物（`TestBench.cs:554-588, 801-832`）；退出时逐项还原。Tour 中玩家免伤+空闲治疗（`TourController.cs:830-867, 681-689`）。**试特效永远不毁档**。
4. **原型驱动 + 代码生成，「不要手改生成物」**。wraith 的 GDScript 文件头明示：「Generated by docs/ref/wraith_cut/anim/mk_game_play.py - change the prototype and generate again, do not edit by hand」（`wraith/wraith_play.gd:22`；wraith_form.gd 同，mk_game_form.py）。原型是「云端批准的动画原型」（anim2.gd/form.gd），Python 生成器烘焙为游戏可驱动文件。
5. **双轨对照 + 声明式夹具 + 后置校验**。Showcase 每个特效原速/慢速各播一遍（`Showcase.cs:580-606`）；Tour 站点写成声明式 INI，TourScene 摆站后 **VerifyPile 逐堆比对**（`TourScene.cs:314-321`），状态与脚本不一致立即抛错——把「测试断言」直接做进了工具链。

辅助方法：日志全留痕（silentfx.log 记录每次 Tour 摆站的完整状态 dump，`TourScene.cs:240, 273`）；参数旋钮化（TestBench 的 Dial 分段按钮点一下即写 tuning，`TestBench.cs:1567-1585`）。

---

## ④ 资产生成链与组织模式

### 4.1 suppress：154 帧序列 + 骨骼元数据 JSON

**统计**（Python/PIL 实测）：154 个 PNG 全部 **357×372 RGBA 带 alpha**；分组：aim 40、attack 59、idle 40、intro 15。

`suppress/suppress_frames.json` 索引结构：

| 字段 | 值 | 含义 |
|---|---|---|
| frames_version / fps / scale | "202" / 24 / 0.38 | 版本、帧率、绘制缩放 |
| size / ground | [357,372] / [171.9,336] | 画布与落脚点 |
| faces | -1 | 面向翻转（note：猎人面朝屏幕左） |
| aim_v / attack_from_aim / cut / slow_end | 112 / 3 / 9 / 36 | 瞄准线速度、attack 从 aim 第 3 帧接入、保留前 9 帧、慢动作为止于 36 帧 |
| groups.intro/idle/aim/attack | 每帧一个对象 | `file` + `f`(前拳)/`c`(胸拳)/`h`(头)/`sh`(前肩) 关键点坐标 + `v`(瞄准线)/`j`(抖动相位)/`pres`/`snapped`/`speed` |

- 消费方 `SilentFX.Cards.SuppressFrames.cs`：Frame 类持有 Tex+Front/Chest/Head/Shoulder+V+Fade/Snapped（`:6-25`）；加载时把 JSON 字段映射为静态参数（`:180-184`）；**自检条件 `Aim.Count == Idle.Count && Attack.Count > Cut + 2`**（`:190`）。
- **帧间像素差异极小**（实测 aim 相邻帧平均差 ≈0.13）：idle 组 `j` 从 15 递增到 39+，是「同一姿势+抖动相位循环」的待机序列。
- **生成链推断**：note 明写 "export-pixel coords inside size" 且关键点名 `f = front fist (rig arms.left)`、`h = head`——**直接对应 rig（骨架）骨骼名**；工作目录中 `LingSilentSkin_mod/_pcksrc/ling/animations/characters/silent/` 存在 Spine 三件套（`ling.json` + `ling.atlas` + `Chr_ling001.png`），游戏原生 Silent 本身是 Spine 骨骼动画（`pck_silent_scenes/scenes/creature_visuals/silent.tscn` 用 SpineSkeletonDataResource）。**结论：154 帧极可能是从 Spine 骨骼动画按关键姿势渲染导出的位图序列，JSON 同步导出骨骼关键点**（具体导出脚本待二探确认）。

### 4.2 wraith 三代贴图演化（tex → tex2 → tex3）

| 代 | 数量 | 命名体系 | 内容 |
|---|---|---|---|
| **tex（一代）** | 32 | `s_F*/s_L*/s_R*`（布条分条）+ body/dress/fireL/fireR/glow/glowL/glowR | 发条（fan/front 两类 25 条）+ 身体/裙/鬼火/辉光。对应 `rig.json` 的 7 layers |
| **tex2（二代）** | 15 | **语义化部件命名**：hornL/hornR（角）、eye/slits（眼缝）、torso_c/under_jaw/under_neck（躯干/颚/颈）、lowL/lowR（下摆）、eyefire + glow_* 8 个辉光 | 角色部件化拆解，部件带关节数据（horn 有 base/tip、low 有 elbow/fist）。`rig2.json` 主引用此代 |
| **tex3（三代）** | 28 | 沿用 s_* + skull/lowL/lowR | **= 一代 s_* 拷贝后局部重绘**：实测 24 个 s_* 中 12 个与一代 bit-identical（F6/F10/L1b/L3 及全部 R 系），12 个被重绘（F1-F9 大部 + L1/L2/L4/L5） |

`rig.json` vs `rig2.json`（Python 实测对比）：strands 25 条**完全相同**；layers 从 7 → 25（新增 horn/low/torso/skull/eyefire/slits + glow_* 系列 + torso_c/under_jaw/under_neck/lowL_c/lowR_c）；chest [604,680]→[604,690]；fireL/fireR 换成 tex2 的大尺寸亮版（176×276→318×648，平均色 126,167,65→157,230,72）并新增 `k`(波数)/`shapes`(细分数)/`tip` 参数。**rig2 跨代引用**：`../tex/dress.png`、`../tex3/skull.png`——三代贴图按需混用。

> 线索：rig2 引用了 `tex2/torso.png`、`tex2/glow_torso.png`，但 tex2 目录中**不存在这两个文件**（疑似未发布或移走）——待二探确认。

### 4.3 贴图技术特征（实测）

- 全部 RGBA；无调色板模式、无 16bit。
- **glow 系列 = 辉光贴图**：极亮单色（实测 glowL 平均 RGB (38,255,18)、glow.png (49,255,54)）+ 低覆盖率（glow.png 仅 1.44% 不透明像素）→ 配合加色混合 shader。命名约定 `glow_<部位>`（glow_eye/glow_hornL/glow_lowR/glow_slits）。
- **s_* = 长条布片**：尺寸竖长（s_F1 226×834、s_L5 371×269 横长条等），运行时由 rig 折线生成带状网格（见⑤）。
- suppress3/tex：10 个，`aura_k2/k25/k3/k4/k5`（654×777）+ `her_k2/k25/k3/k4/k5`（785×981），k 后缀疑为等级/时间档位（待二探）。
- **与游戏原生对比**：游戏原生 Vfx（`decomp/MegaCrit.Sts2.Core.Nodes.Vfx/` 140+ 类）几乎不引用贴图——全目录仅 6 个 res:// 路径（如 `res://images/vfx/brush_particle_2.png`、地图圆圈 tres），**原生以程序几何+shader 为主**；SilentFX 是「PNG 资产 + 自定义 shader + rig 数据」混合路线，命名完全自建体系。

### 4.4 代码生成链（资产生成链的「链」在哪）

wraith 目录 = 12 个 `.gdshader` + 2 个生成的 `.gd` + rig.json/rig2.json + 三代贴图：

- `wraith_play.gd`（1524 行）：打出幽魂形态时的「女鬼」。文件头声明由 `mk_game_play.py` 从云端原型 `anim2.gd` 生成（`:22`）。**KEYS 数组定义 6 个关键姿势**（standby→drawing it in→it bursts→the X behind her→down both sides→stripped），每姿势 40+ 参数（sink/sy/breath/tilt/sleeve/fore/blade_l/fan/flare/eye_len/glow/howl/strip/body/ring…），`x_at(t)` 姿势间插值（`:105+`）——**参数化程序动画**。
- `wraith_form.gd`：待命（悬停）阶段，同样由 `mk_game_form.py` 生成。
- rig strands 在 gd 中由 `make_strand()` 读入、`pose_strand()` 逐帧摆姿势生成带状网格（`wraith_play.gd:609-618, 668+`）；THIN=2（中线每 2 点保留，间距 8px）。
- tuning.ini 中 wraith 段（body_alpha/width_mul/fog_in/fog_hold/fog_out/fog_rest/motes_amount）即这些生成文件的运行时旋钮。

---

## ⑤ rig 数据结构

`wraith/rig.json` 与 `rig2.json` 同构（size 1212×1704，scale 2）：

```
{
  "size": [1212, 1704], "scale": 2, "chest": [604, 680],
  "strands": [ 25 条布条 ],
  "layers": { 7~25 个贴图层 }
}
```

**strand（布条）字段**（`rig.json` L1 实例）：

| 字段 | 示例 | 含义 |
|---|---|---|
| name / kind / family | L1 / fan / L1 | 名称；类型 fan(扇形展开)/front(前发)；家族（多条共享一套动画参数，如 L2/L3 同族 L2） |
| tex / ox / oy | s_L1.png / 137 / 406 | 贴图及在 1212×1704 画布中的偏移 |
| pts | 62 个 [x,y] | 中线折线（从发根到发梢） |
| left / right | 62 个浮点 | 每个中线点的左/右半边宽（布条轮廓） |
| arc | 62 个角度 | 每点切线弧度（用于法线/朝向） |
| len | 428.6 | 总长 |

**layer（贴图层）字段**：`tex/ox/oy/w/h` 基础放置；fire 层带 `k`(波数 3.0)/`shapes`(细分数)/`foot`/`tip`；horn 层带 `base`/`tip`（角的根与尖）；low 层带 `elbow`/`fist`（肘/拳关节）。GDScript 侧对应常量：CHEST=[604,690]、SH=[444/764,684]、FISTS=[286/972,904]、HC=[604,470]（`wraith_play.gd:88-96`）——**与 rig2 的 chest/fist 数值精确一致，rig.json 就是 gd 动画的骨架数据源**。

---

## ⑥ 可借鉴点（龙族零 → 杀戮尖塔2 移植项目，UE4 资产 → Godot）

1. **tuning.ini 模式直接照搬**：把 UE4 里散落在曲线/材质参数/ Niagara 模块里的数值统一导出为「按特效分节、英文注释、key=value」的 INI，运行时 `Tuning.F(key, default)` 读取 + 一个热键重载。移植调优效率提升最大的单点。
2. **DevPreview 式「无战斗预览台」**：CanvasLayer + ColorRect 假人 + 假目标即可承载 FX（`DevPreview.cs:440-494`）。Godot 侧可做成 Engine 插件或游戏内 overlay：UE4 特效移植后先在此层孤立验证，再进真实战斗。
3. **快照-还原实验场**：进入测试时存牌堆/HP/能量/状态，退出逐项还原（`TestBench.cs:554-588`）。移植项目验收特效时保证试玩存档不污染。
4. **声明式巡演夹具**：INI 式站点脚本（手牌/敌人/能量/慢动作）+ 独立的一句话文案文件 + AVG 字幕 UI，是「特效移植验收」的现成方案——每个移植完成的特效配一站，Ctrl+Home 从头巡演一遍即交付验收。
5. **rig.json 数据化中间层**：UE4 的骨骼/布片先导出为「贴图偏移 + 折线 + 半边宽 + 弧度」的 JSON，再由 Godot 运行时生成带状网格——比直接搬 UE4 动画资产更轻，且参数（rig 数值）与表现（gdshader）分离，可独立调优。
6. **贴图三代管理法**：tex→tex2→tex3 = 拷贝-局部重绘-跨代引用（rig2 同时引用 ../tex/ 与 ../tex3/）。UE4 资产迭代时同样「整包拷贝再改」，rig/配置文件用相对路径跨代引用，避免重复资产。
7. **命名规范**：`glow_<部位>`（辉光）、`s_<方位><序号>`（布条）、`<部位>_c`（变体）可直接作为移植资产命名约定；「位图管视觉、JSON 管锚点（拳/头/肩）」的帧序列组织方式，适用于 UE4 导出动画序列时同步导出 socket 元数据。

---

## ⑦ 【线索清单】给二探的深挖点

1. **suppress 154 帧导出管线**：`LingSilentSkin_mod/_pcksrc/ling/animations/characters/silent/ling.json`（Spine 骨骼）是否就是帧序列之源？导出器脚本在哪（作者工作区 `docs/ref/`？）？`export-pixel coords` 的具体含义。
2. **云端原型与生成器**：`docs/ref/wraith_cut/anim/anim2.gd`、`form.gd`、`mk_game_play.py`、`mk_game_form.py` 在发布包中不存在——「改原型重新生成」工作流的原型文件结构、生成器输入输出（待二探确认是否能找到作者工作区材料）。
3. **wraith_play.gd 核心算法**：KEYS 姿势插值 `x_at(t)`、`pose_strand()` 布条变形数学、head_rigid 头部刚体变换——布条动画的完整实现（本报告仅定位）。
4. **rig2 引用但不存在的文件**：`tex2/torso.png`、`tex2/glow_torso.png` 缺失原因（未发布/被删/路径错）。
5. **巡演脚本实例**：`展示_脚本.txt` / `展示_每张牌一句话.txt` 运行时才放入 mod 目录，发布包无实例——站数规模、文案写法、终幕（[结尾]）内容待二探找运行环境或作者示例。
6. **TourPatches**：巡演依赖的 Harmony 补丁（`TourController.cs:161` VerifyInstalled）具体守护了哪些游戏方法——「工具如何安全地操控战斗」的关键。
7. **音频资产组织**：`sfx/choices.ini`（音效候选与本机微调，`sfx/README.txt`）、41 个音效槽的候选清单、Mood 音乐 DSP（FMOD）链路细节。
8. **suppress3 的 aura_k*/her_k***：k2/k25/k3/k4/k5 是等级还是时间档位，与 SuppressFx 动画阶段的对应关系。
9. **shaders/ 目录（81 个文件）** 与 wraith/*.gdshader 的分工、`ShaderLib.Reload()` 热重载机制（F8 的完整链路）。
10. **PlayerSettings/RitsuLib 设置页**与 user_settings.ini：mod 面向普通用户的设置层与 Dev 层如何分层（本报告未展开）。
11. **HandPace / GameCapabilities** 等非 Dev 基础设施对 Dev 工具的支撑（如 `GameCapabilities.PoisonTrigger` 反射调用兜底，`Showcase.cs:400-407`）——版本兼容策略。

---

*附：本报告全部统计数字来自 PIL 实测（154/32/15/28/10 张贴图的尺寸、色彩模式、alpha、MD5 对比、帧间差异、glow 亮度），代码证据均注明 split 文件名:行号；未实地运行游戏，工作流推演基于代码路径。*
