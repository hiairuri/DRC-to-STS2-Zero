# 二探·GDScript 舞台与 rig 数据代理 —— 10_gd_stages.md

> 勘察代理：deep-gdstages ｜ 日期：2026-10-16
> 材料：`3817850774/suppress3/suppress_stage.gd`（1286 行）、`3817850774/wraith/wraith_form.gd`（902 行）、`3817850774/wraith/wraith_play.gd`（1525 行）、`wraith/rig.json`+`rig2.json`、`suppress/suppress_frames.json`；对照 `silentfx_decomp/split/` 下 SuppressStage/SuppressRun/SuppressFrames/SuppressShow/WraithStage/WraithHeld2/WraithPlayFx/WraithSceneWarmup 八个 C# 文件；初探报告 02_shaders.md 第②节、04_devtools_assets.md 第④⑤节。
> 证据格式：gd 用 mod 目录相对路径（如 `wraith/wraith_form.gd:162`），C# 用 `split文件名:行号`。

---

## 结论先行（TL;DR）

1. **三个 .gd 是同一模式的三个实例**：C# 只做「编译 GDScript → new Node + SetScript → Call(方法名, 参数) → 每帧 Call 喂时间/输入 → 读返回 Dictionary」，**所有动画数学都在 gd 侧**，C# 不持有任何表演状态。这是「引擎驱动、脚本表演」的干净分工，移植项目应直接照搬该握手协议（①节全表）。
2. **程序化动画的核心是三层叠加**：姿势字典插值（`x_at`，40+ 参数的关键帧栈）→ 空间变形场（呼吸/头部/袖子的 `where()` 正解，CPU 与 shader 双份同参）→ 布条质点近似（rig.json 中线折线 + 法线横推 + 分段相位偏移，无物理求解器）。撕布/风吹是「沿弧长的角度插值 + 自由端优先」，不是 Verlet。
3. **rig.json 就是 UE4→Godot 移植最该复制的「数据化骨架中间层」**：25 缕头发 = 中线折线+左右半宽+累计弧长+贴图偏移；25 个 layer = 刚体部件+关节锚点（elbow/fist/base/tip/foot/root）。gd 侧零硬编码坐标（除少量常量），全部从 rig 读（⑤⑨节）。
4. **WZ_HEAD 双份绘制协议有精确数学**：`a*(1-w)/max(1-wz_outer*a*w, 1e-4)` 是半透明 CanvasGroup 双重复合下的 alpha 去重校正，可证明两份份额之和恰等于画一次（⑦.12 节推导）。
5. suppress_frames.json 是**旧幻影（SuppressShow）的帧序列元数据**，v/pres/speed/i/u/j 多为导出器参考数据、C# 未消费；新表演（suppress_stage.gd）完全不读它（⑥节）。

---

## ① 三舞台分工与 C# 握手协议全表

### 1.1 分工

| 舞台脚本 | 卡牌 | 阶段 | C# 驱动类 | 触发条件 |
|---|---|---|---|---|
| `suppress3/suppress_stage.gd` | 压制（先古） | 持卡悬停→拿起→瞄准→释放→撕魂→散去 全程 | `SilentFX.Cards.SuppressRun.cs`（每帧 step）+ `SuppressStage.cs`（编译/build/warm） | 手中有压制卡即建（`SuppressHeld.cs:24,90`） |
| `wraith/wraith_form.gd` | 幽魂形态 | 持卡待机：25 缕纱布条屏幕边缘等待→甩入→盘绕成卡图女子→SNAP(0.76s) 展开→透明呼吸待机 | `SilentFX.Cards.WraithHeld2.cs`（HeldSession 子类，每帧 set_state） | 拿起幽魂形态卡 |
| `wraith/wraith_play.gd` | 幽魂形态 | 出招：卡光入胸→前冲爆发→火刀→风撕纱布→散尽 | `SilentFX.Cards.WraithPlayFx.cs`（每帧 set_time） | 出牌时（`WraithPlayFx.cs:239`） |

三者共性：`extends Node`、纯被驱动物、不 import 游戏状态、坐标靠 C# 传入的 back/front(/top) 节点与 lo/hi 视口；`suppress_stage.gd:2-5` 明确「三层 GlobalTransform 都是单位阵，脚本坐标=场景坐标」。两个 wraith 脚本文件头声明由 Python 原型生成（`wraith_form.gd:20` mk_game_form.py、`wraith_play.gd:21` mk_game_play.py，「change the prototype and generate again, do not edit by hand」）。

### 1.2 suppress_stage.gd ↔ SuppressRun/SuppressStage 握手全表

| 方向 | 方法/属性 | 参数 → 返回 | C# 调用点 | 语义 |
|---|---|---|---|---|
| C#→gd | `warm(dir, cache)` | (String, Dictionary) → bool | `SuppressStage.cs:178` | 每调一次读一张还没读的贴图/编译一个 shader，读完返回 true（分帧预热，`suppress_stage.gd:167-177`） |
| C#→gd | `ring_tex()` | () → ImageTexture | `SuppressStage.cs:196-198`（先 Set CACHE 再 Call） | 128×128 程序化环纹理（`suppress_stage.gd:203-213`） |
| C#→gd | `build(dir, cache, back, front, lo, hi, params)` | params={size, hit_wait, reduce_effects, remote_ui} → bool | `SuppressStage.cs:375-384` | 搭场景；size/hit_wait 来自 tuning（`SuppressStage.cs:370-373`） |
| C#→gd | `set_top(top)` | Node → bool | `SuppressRun.cs:237` | 挂「界面之上」层（全局雾/闪/波纹/48 光点），失败则弃用 |
| C#→gd | `set_soul(tex, tpos, tsize)` | (Texture2D, Vector2, Vector2) | `SuppressRun.cs:831-845` | 把敌人魂的截图纹理/位置/尺寸交给 smat；null=用程序化魂 |
| C#→gd | `lift(at)` | Vector2（鼠标处） | `SuppressRun.cs:288` | 拿起：hover→free，牌炸成雾， cues+=lift |
| C#→gd | `aim(feet, chest, w, h)` | 4 参 | `SuppressRun.cs:314` | 指向敌人：free/aim→aim，链头唰地吸住，cues+=bite |
| C#→gd | `unaim()` | () | `SuppressRun.cs:323` | aim→free |
| C#→gd | `release()` | () | `SuppressRun.cs:335` | aim→rel（沉 2 帧→化雾冲起 2 帧→凝回） |
| C#→gd | `play_auto(feet, chest, w, h)` | 4 参 | `SuppressRun.cs:379` | 无待命直接打出（快捷键）：当场出现+咬住+release |
| C#→gd | `hit(killed)` | bool | `SuppressRun.cs:388` | 游戏真伤害到账：rel→tear |
| C#→gd | `cancel(at)` | Vector2 | `SuppressRun.cs:405` | 放回：free/aim→cancel，链子缩回链头 |
| C#→gd | `dismiss()` | () | `SuppressRun.cs:420` | 撤掉（换目标/离开） |
| **C#→gd（每帧）** | `step(dt, mouse, presence, reduce_effects)` | 4 参 → **Dictionary** | `SuppressRun.cs:568-574` | 唯一每帧入口；dt 已乘 Engine.TimeScale（`SuppressRun.cs:446`） |
| gd→C#（step 返回） | `{done, yank, shake, hold, card_show, dim, cues}` | `suppress_stage.gd:538` | `SuppressRun.cs:586-657` | done→Free；yank→把敌人身体往她这拽（`SuppressRun.cs:854`）；shake→GameRefs.Shake；hold→CreatureHold 顿帧（`:665-668`）；card_show→牌重新显现；dim→敌人 modulate 压暗（`:1026` RGB×(1-0.5/0.42/0.4×dim)）；cues→音效 |
| C#→gd | `hand_pos("k2","front")` | (String,String) → Vector2 | `SuppressRun.cs:743` | 联机模式下「鼠标」用她 k2 姿势前手代替（`suppress_stage.gd:562-563`） |
| C#→gd（属性读） | `fogb` / `veil` | Get → CanvasItem | `SuppressRun.cs:771-784` | 联机待机时藏雾幕（FilterStandbyScreen） |

 cues 音效映射（`SuppressRun.cs:691-721`）：lift→`suppress_mist`（仅本地）、bite→`suppress_bite`（延 0.08s）、tear→`suppress_tear`、burst→`suppress_burst`。

### 1.3 wraith_form.gd ↔ WraithHeld2/WraithSceneWarmup 握手全表

| 方向 | 方法 | 参数 → 返回 | C# 调用点 | 语义 |
|---|---|---|---|---|
| C#→gd | `build(dir, cache, back, front, lo, hi, org, k)` | 8 参 → bool | `WraithHeld2.cs:130`；warmup `WraithSceneWarmup.cs:199-209` | org=她图角落点、k=图像素大小（`wraith_form.gd:598-673`） |
| C#→gd | `set_param(key, value)` | ("haze"/"shake"/"dusk"/"alpha"/"arms"/"ui_*"/"haze_ui"/"blur") | `WraithHeld2.cs:137-157`、`WraithStage.Tune:408-442` | 见 `wraith_form.gd:17-19` 文件头清单 |
| C#→gd | `times()` | () → PackedFloat32Array | `WraithHeld2.cs:147-152` | `[SNAP, SNAP+0.30, SNAP+0.95]`（`wraith_form.gd:897-898`）；C# 取 [1] 作 `_readyAt`（可以接招的时刻） |
| C#→gd | `phase()` | () → [PH, breath] | `WraithHeld2.cs:384-386` | 摇摆相位+呼吸量；C# 存为 SwayPhase，出招时经 `set_param("ph", …)` 传给 play 续接（`WraithPlayFx.cs:1010-1014`） |
| **C#→gd（每帧）** | `set_state(t, wake, clock, show)` | t<0=边缘等待（wake 0..1 出来多少）；t≥0=放入后秒数；clock=自由钟；show=她+纱条透明度 | `WraithHeld2.cs:353-359` | 唯一每帧入口（`wraith_form.gd:675-683`） |
| C#→gd | `set_top(top)` | Node → bool | `WraithHeld2.cs:216` | 建「界面之上」舞台+双份绘制（`wraith_form.gd:790-830`） |
| C#→gd（warmup 专用） | `shader_code(file, add)` / `zoned(code, add)` | → Shader | `WraithStage.WarmShader:691-727` | 借 gd 的 shader 源码改写能力预热编译变体 |
| C#→gd（属性写） | `DIR` / `CACHE` | Set | `WraithStage.cs:685-686` | 只挂脚本不 build 也能编译 shader |

warmup 时序（`WraithSceneWarmup.cs`）：`build` → `set_param("blur",…)` → `set_state(0.96,1,0,1)` → 开 SubViewport（≤960×540、透明背景、独立 World2D）等 2 个 FramePostDraw → 销毁；play 侧同理多 `set_param("wave")`+`set_top`+`set_time(1.2,0)`（`:319-334`）。

### 1.4 wraith_play.gd ↔ WraithPlayFx 握手全表

| 方向 | 方法 | 参数 → 返回 | C# 调用点 | 语义 |
|---|---|---|---|---|
| C#→gd | `build(dir, cache, back, front, lo, hi, org, k, who)` | 9 参 → bool | `WraithPlayFx.cs:287-298`；warmup `WraithSceneWarmup.cs:303-314` | who=出牌者站位（她图坐标系） |
| C#→gd | `set_param(key, value)` | "shake"/"dark"/"fire_pad"/"puff"/"wave"/"wave_push"/"ui_*"/"blur" | `WraithPlayFx.cs:408-438`、`WraithStage.Tune` | 见 `wraith_play.gd:16-20` 文件头清单 |
| C#→gd | `times()` | () → [T_BURST, T_GO, T_HIT, T_GO+3.05] | `WraithPlayFx.cs:439-447` | C# 缓存四个时刻驱动音效/震屏/伤害判定（`:1094-1124`） |
| C#→gd | `chest()` | () → Vector2 | `WraithPlayFx.cs:674-677` | 她胸口的场景坐标；C# 把游戏自己的飞牌 Path2D 终点改到这点（`wraith.card.to_her`） |
| C#→gd | `set_fade(a)` | 0..1 | `WraithPlayFx.cs:449, 1132` | 整体淡入淡出（待机→出招交接） |
| **C#→gd（每帧）** | `set_time(t, clock)` | t=出牌后秒数、clock=自由钟 | `WraithPlayFx.cs:987-1082` | 唯一每帧入口；内部 `x_at(t)→pose()`（`wraith_play.gd:1227-1237`） |
| C#→gd | `set_top(top)` | Node → bool | `WraithPlayFx.cs:549` | 双份绘制舞台 |

C# 侧每帧计时细节：`run.T += dt/Pace`（pace 可调 0.4~3，`:1075`）；飞牌期间把 T 平滑推到 T_BURST-0.002（`:1054-1058`）；`set_time` 的耗时被 Stopwatch 统计并日志（`:1077-1089，worst 帧耗时`）。

---

## ② suppress_stage.gd（1286 行）逐段解析

### 2.1 常量与姿势表（:11-27）

- `F1=1/30`：**表演以 30fps 帧为最小时间单位**（「沉 2 帧」「冲起 2 帧」= 2/30 s），即使游戏跑在其他帧率。
- `PW=1456, PH=817`：原型画面尺寸——她的 5 张姿势图坐标都在这个 1456×817 空间里。
- `POSE`（:14-20）：k2/k25/k3/k4/k5 五姿势，每姿势 `pos`（图左上角）+ `front`（前手）+ `back`（后手）三个原型坐标。
- `SKEW=(-14,22)`：沉到底的位移；`TEX_K=0.44/0.7`：原型贴图缩 0.44、本模贴图是原图 0.7，故缩放补偿 0.44/0.7。
- 撕魂时间线常量：`FREEZE=0.12`（魂到手定格）、`FADE=0.70`（她散成光点）、`HS2=3/30`（撕下顿住）、`SLOW=0.55`（收尾慢放倍率）、`SETTLE=0.12`（定格后正常速度回余势）。
- 坐标映射 `H(p)=Vector2(hi.x+(p.x-PW)*S, lo.y+p.y*S)`（:152）：原型画面**右对齐**到视口右边、底部对齐，S=场景像素/原型像素（:241，含 size_k）。

### 2.2 build 场景搭建（:233-293）

顺序即图层顺序（注释 :248「身后：压暗、地雾、她的雾、她、拖影线、手光、链子」）：
1. **back 层（角色身后）**：veil 暗幕（Color(0,0.02,0.03,0)）→ fogb 地雾（fog.gdshader，y0=lo.y+60S 到 y1=lo.y+560S 渐隐）→ aura_b（aura shader）→ **her[POSE] 5 张姿势图共用同一个 hmat**（her.gdshader；pxk=0.7/0.44、dark=0.72、ghost=1.0、feet_y=lo.y+590S、fade=190S，:255-257）→ ghosts×3 拖影 → aura_f → slines×2（smear）→ hg_f/hg_b/column 手光柱 → rope_tail/rope_bk（rope shader，rmat 共用）。
2. **front 层（角色身前）**：lurk 敌人脚下雾（lurk shader）→ gnd 地光 → tether 魂与身体间一缕（tether shader，width=64S）→ soul（soul shader，has_tex 默认 0）→ sglow → rope_fr → fingers×3 雾指头（knots=0）→ head_glow → **pool 220 个光点**（radial 点纹理 + ADD 混合，对象池）→ burst_q → flash/flash2 → ring1/2/3（ring_tex 程序化环）。
3. **top 层（set_top :296-312，整个界面之上）**：grade（MUL 混合偏绿压暗）→ atmos 全局雾（atmos shader）→ top_pool 48 个界面光点 → BackBufferCopy → ripple 扭曲波纹（ripple shader，span=hi-lo）→ white 全屏闪 → dark_top 冲击帧压暗。
4. 返回 false 的唯一路径是贴图缺失（:244）。

### 2.3 step 每帧链路（:408-538）

主链路一行行：`clock+=dt`（dt 钳 0~0.1）→ rel/tear 状态推进（:420-443）→ 她（pose_now→hs 可见性/off/sc 位移缩放→fr/bk 两手世界坐标→hmat 13 参数 :498-501）→ her_light 手上光（:565-578）→ ghost_trails 拖影（:580-597）→ aura → hand_smears 甩手拖影线（:599-621，二次贝塞尔 `a0.lerp(cc,u).lerp(cc.lerp(a2,u),u)`，cc=a1*2-(a0+a2)*0.5 反推控制点）→ soul_grip→update_soul→update_chain→update_lurk→update_lights→update_atmos→update_particles → 输出字典。

- **phase 状态机**（:99）：`hover→free→aim→(cancel|rel)→tear→gone`，外加 `dismiss` 时间戳可叠任何阶段。done 条件（:528-532）：tear 且 tm>flight+SETTLE+FADE+0.4 且光点尽；cancel 后 0.45s；dismiss 后 0.4s。
- **撕魂时间线 tm**（:426-434）：`tt=clock-t_tear`（真实时间）；`te=max(0,tt-HS2)`（去掉顿住）；`tm`（表演时间）分段：`te∈[flight,flight+FREEZE)→tm=flight`（魂到手定格）；`[flight+FREEZE,+SETTLE)→tm=te-FREEZE`（正常速度回余势）；之后 `tm=flight+SETTLE+(te-…)*SLOW`（慢放）。同时 `prate`（光点钟速率）在同样区间 lerp 到 SLOW——**慢放是「时间重映射」不是降帧**，光点/雾/光环全部跟随。
- **flight 动态**（:1141）：`flight=clamp(0.13+dist/5000S, 0.13, 0.24)`，魂飞多久由她手到敌人的距离决定。
- **撕魂拽敌**（:441-443）：`mag=62S*s01(0,0.035,tt)*exp(-max(tt-HS2,0)*7)`，方向=(k4前手-G).normalized()，作为 yank 输出给 C# 搬敌人身体。
- 输出字典（:538）：`{done, yank, shake, hold, card_show, dim, cues}`，shake 是int 等级（1/2/3 三档）、hold=0.26 顿帧秒数、dim 敌人压暗量。

### 2.4 魂与链子（:623-871）

- `soul_grip(tm)`（:627-643）：撕魂飞行是**二次贝塞尔**——`p=start.lerp(c,u2).lerp(c.lerp(hold,u2),u2)`，u2=eout(tm/flight)（三次缓出），控制点 c=start-hold 中点 +(-70,-60)S（向左上偏的弧）。落地后从 k4 手过渡到 k5 手（×0.8）。
- `update_soul`（:645-695）：15 个 shader 参数（:673-678）；soul quad 只覆盖「敌人~魂」AABB（:680-684，省填充）；速度拉伸 `st=clamp(|v|/2600S,0,0.35)*detach`，到达瞬间反向 -0.18（:669-670 压缩顿感）。
- `update_chain`（:699-850）：链身 21 点二次贝塞尔（fr→ctrl→end，ctrl=中点+sag+swing，:770-780），`bend` 侧向正弦波、`wave_u` 高斯包络上行波（:779）；show_from/show_to 只画一段（拿起时从链头长出、放回时缩回，:783-789）；**三根雾指头**（:823-837）每根 8 点二次贝塞尔、张开角 spread 0.62（咬住）/0.95（爪子）；aim 时每 0.7s 一个脉冲光点沿链流（:799-803 + riders :844-850）。
- `update_tail`（:853-868）：手后松垂链尾 11 点，双正弦轻晃。

### 2.5 光/雾/粒子（:873-1264）

- `update_lurk`（:875-900）：敌人脚下雾，amt/beat/burst 三量；tear 后留一缕残雾沉下去。
- `update_lights`（:902-991）：手光 hg_f/hg_b 随 charge 呼吸、column 冲起竖光柱（k25 阶段 (1.8,6.4)S、k3 阶段 (1.5,7.2)S）、gnd 地光、flash/flash2/ring1-3 各时刻闪光环（凝回手上/咬住处/魂到手大闪/魂炸开）。
- `update_atmos`（:993-1083）：按 phase 查表 want_atm/veil/fog/grade（:998-1010），`damp` 指数趋近（k=3.5~6）；冲击帧序列：第 1 帧强闪 wa=0.55、第 2 帧压暗 dk=0.42、之后衰减闪（:1047-1049）；shock 冲击波半径 40→1700px（eout）；ripple 屏幕扭曲 30→1500px。
- 粒子系统（:1210-1264）：`addp(t0,life,p0,v,k,g,s0,s1,c,st,fin)`——**带阻尼 k 的指数趋近运动** `pos=p0+v(1-e^{-ka})/k`（k=0 则匀速）+ 重力 g + 尺寸 s0→s1 插值 + 速度拉伸 st（沿速度方向拉长，:1239-1241）+ alpha 淡入淡出；`t0` 按普通钟给、内部转 pclock（慢放钟，:1211）。riders 是沿链子折线插值的流动光点（:1247-1262）。
- `arrive_fx`（:1186-1206）：**从 her_k5.png 轮廓采样**——随机取点直到 alpha≥0.6，最多 90 点/6000 次尝试（:1197-1204），缓存 Image 不读回显卡（:163）。

### 2.6 strip() 网格生成（:1268-1285）与 warm()（:167-177）

`strip(mi, pts, half)`：每点算法线（中心差分 `pts[i+1]-pts[i-1]` 正交化）→ 左右两点 `p±n*half` → UV=(累计弧长, 0/1) → TRIANGLE_STRIP，返回总长（供 shader 的 tail_fade/head_fade/pulse_pos 用）。链子/链尾/拖影线/雾指头四类 mesh 全部复用。
`warm()`：每次调用只读一张图或编一个 shader 就返回 false，C# 每帧调一次直到 true——**分帧预热防卡顿**（`SuppressStage.cs:178`）。

---

## ③ wraith_form.gd（902 行）逐段解析

### 3.1 常量（:25-66）

`ORG=(584,12)` 她图角落点、`K=0.62` 图像素、`KK=k/0.62` 相对原型的缩放；`CHEST=(604,690)`、`HEADC=(604,430)`、`SH_L/R=(444/764,684)`、`FIST_L/R=(286/972,904)`（与 rig2 的 elbow/fist 精确一致）；`SWELL=0.06`、`LIFT=7.0` 呼吸膨胀/上提量；`SNAP=0.76` 她盘紧啪地展开的时刻；`ORDER`（:66）25 缕头发落位顺序（侧发从上到下交替、前发从外到内，F 系列最后——「什么挂在她脸上的最后」）。

### 3.2 25 缕头发：make_strand / track / idle_points / pose_strand

- **make_strand**（:162-215）：从 rig 取点，**THIN=2 抽稀**（中线点间距 8px，只留偶数点和末点；但 arc>8 的弯曲处全留，:167——曲率大处加密）；算静法线 nrm；Polygon2D 的 uv = 中线点±法线×半宽-贴图偏移（:179-180，贴图随网格变形）；**i0=根点**（arc 首次 ≥0 处，:184-186）；**入场弧线**（:187-207）：从屏幕边（phi0=黄金角螺旋定位+hash 抖动，:193-195）绕她头螺旋收拢到根点，`phi=phi0+delta*pow(u,2.1)`、`rr=r1+(R0-r1)*pow(1-u,1.35)`（96 段，先快后慢地绕进来），弧长表 cum；`land/delay/out/poke/w1/ph` 全部由 ORDER 排名+hash1 确定性生成（:209-214）。
- **track(st,s)**（:217-247）：s≥0 沿静止弧二分（含超出末端的切线外推）；s<0 沿入场弧二分——**同一函数支持「就位前」和「就位后」两段轨迹**。
- **idle_points**（:266-291）：待机摇摆=行波 `w=sin(ph-k1*u+ph1)+0.45*sin(2ph-k2*u+ph2)`（u=归一化弧长，沿头发传播的波）+ 呼吸膨胀 breathe() + 滞后 lag（对 rise_at 做中心差分，:288——头发跟随身体起伏但慢半拍）。
- **pose_strand 三阶段**（:293-376）：
  1. **等待**（t<delay）：头在屏幕边探头探脑——`probe=0.72+0.20*sin(CL*w1*1.7+ph)+0.34*pow(max(0,sin(CL*2.3+ph*3)),6)`（:308，脉冲式 dart），head=-la+(260+poke)*probe*WAKE-260；
  2. **甩入**（delay≤t<land）：`e=pow(x,2.6)` 全程加速的鞭击（:312），head 从 -la+poke*0.9 lerp 到 0，`stretch=1+1.5*sin(PI*min(x*1.08,1))^1.2`（中途拉伸，:314）；沿途 writhe 扭动 `+nrm*16*(0.3+0.7f)*sin(wt*15-f*7+ph)`（:331，越靠梢越强）；
  3. **展开**（t≥land）：`head=last*(1-pow(1-y,2.2))` 沿自身位置冲出并急停（:317），settled=smoothstep(0.75,1,y) 后混合到 idle_points；SNAP 前 0.11s 绷紧（taut=26*smoothstep）、SNAP 后 2.4s 内炸开（e3=120*exp(-ts/0.14) 沿径向外抛 + e4 法向摆动，:342-356）；着位后 1.5s 内一次回抽lash（e1/e2，:340-341）。
  - 最后 ribbon 化：运动中 fat=1.35（飞入时更胖，:364）。

### 3.3 16 圈身带 + 6 臂带 + 26 碎条（:378-512）

- **make_ribbon**（:380-393）：**双层 Polygon2D**（前/后），uv=(i/(n-1), 0/1) 条带参数化，共用 strip_shader；vertex_colors 表达前后分层——`cf=Color(tintc,1,1,alpha*f)`、`cb=…*(1-f)*0.75`（:404-405，后层暗 25%）。
- **pose_wrap**（:422-446）：16 圈纱布绕躯干——椭圆螺旋 `R=torso+3400*far+dl*330*(1-e)`（far=(1-e)^2.3 从大半径收拢）、`th=th_h-spin*dl`（头到尾差 span 弧度的螺距）、y 方向 `0.24*sin` 压扁+绕轴翻转 dir；`torso=128+26*sm(700,590,y)`（肩宽腰窄，:433）；front=sm(-0.18,0.18,sin(th))（朝向相机侧更亮）。
- **pose_armwrap**（:456-480）：6 条臂带从肩到拳的螺旋，`s=head-(1-f)*0.62`（头从 -0.25 走到 1.22），R=40+55(1-x)+16sin(3s)。
- **pose_shred**（:492-512）：26 条 SNAP 时抛飞的碎条，极坐标 `r=max(40,headr-(1-f)*len*(0.5+1.2y))`+curl 曲率+flutter 抖动，0.42s 生命。

### 3.4 整体 pose（:516-596）与 build（:598-673）

- **fist_at**（:516-519）：**双杆 IK 闭式解**——肘取肩拳中点，前臂绕肘转 ang.y，上臂绕肩转 ang.x，两次旋转即得拳位（无迭代）。
- **pose(t)**（:521-596）：torso/arms/horn 三个 sm 进度喂 body_mats（:546-552）；taut/pop/bob 驱动 figure 的 scale/position（:540-541，以 CHEST 为锚点的挤压回弹）；arms 摆动 r=1°×P.arms×idle，upper/fore 用 breath_at 相位差（-0.15/-0.55）产生异步（:543-545）；dusk 场暗（:589-590）；shk 高频抖动 `16*exp(-since/0.10)*sin(since*93)`（:591-592）。
- **build**（:598-673）：rig.json 经 CACHE 读入（:600-603，C# 预热时已解析）；LO/HI 把视口换算成她图坐标（:607）；fan 缕挂 back、front 缕挂 frontn（:647——侧发在脑后、前发在脸前）；glow 用 body.gdshader+blend_add 变体（:658）；flash 在她胸后 150px（:665）、ring/ring2 2600×2600 以 HEADC 为中心（:666-668）。

### 3.5 WZ_HEAD 双份绘制协议（:689-849）——本文件最大创新

问题（注释 :690-697）：游戏界面占窗口顶部带（bar/遗物）与底部带（手牌）及两个底角；她的纱布条会伸进这些带里——画在角色层会被 UI 挡住，画在 UI 上又会在场地中央盖住角色。
方案：**每条可入带的纱条画两份**——原地一份（角色层）+「界面之上」舞台一份；每份只出自己份额的 alpha，两份之和=画一次。
实现：
- `WZ_HEAD`（:698-716）：注入的 GLSL 份额函数（数值推导见 ⑦.12）。
- `zoned(code, add)`（:718-746）：**运行时改写任意 shader 源码**——fragment 结尾插 `COLOR.rgb=mix(COLOR.rgb,wz_veil.rgb,wz_veil.a); COLOR.a=wz_a(COLOR.a);`，vertex 里插 `wz_p=(MODEL_MATRIX*vec4(VERTEX,0,1)).xy`（世界坐标），头部插 WZ_HEAD；ADD 混合的材质定义 WZ_ADD 走无校正分支。改写失败会 push_error 并退回画两份（:743）。
- `zone_numbers`（:760-782）：把 ui_top/ui_bot/ui_corner/ui_left/ui_right/ui_soft（窗口比例）换算成场景坐标带 wz_y/wz_c（Vector4：上带 [y0-s,y0+s]、下带、底角 [x 左, x 右, 角线 y, 半软宽]）。
- `twin_poly/set_top/sync_top`（:784-849）：twin 复制 Polygon2D；set_top 建 root_t 舞台并按绘制序重建整棵树（shredn_t→figure_t(CanvasGroup，self_modulate 同步，半透明组）；sync_top 每帧同步 transform/polygon/vertex_colors/可见性，`wz_outer=figure.self_modulate.a*SHOW`（:839-840），顶层雾副本混入 dusk 颜色（:838）。
- 运动模糊 `veil_up/pose_veil`（:851-895）：把节点裹进 CanvasGroup 挂 wraith_veil.gdshader；blur 量=距她距离（on=lerp(0.45*WAKE,1,…)×远离衰减，:879）+ P.blur×BLUR_PX(5)×KK；gl_compatibility 渲染器下半透明组不清真则关模糊（:852-857）。

### 3.6 节奏输出（:897-901）

`times()=[SNAP,SNAP+0.30,SNAP+0.95]`（绷紧→可接招→安定）；`phase()=[PH,breath]`（出招动画续接她的摇摆相位）。

---

## ④ wraith_play.gd（1525 行）逐段解析

### 4.1 常量与 KEYS 六姿势（:22-116）

- 时间轴：`T_BURST=0.60`（卡光入胸）、`T_GO=0.67`（爆发）、`LUNGE=0.52`（前冲时长）、`T_REL=0.97`（火脱刃）、`FLY=1.15`（火飞行）、`T_HIT=2.12`（火入敌身）、`T_PULL=1.01`（第一缕纱被风撕）、`PULL_SPAN=1.15`、`PULL_DUR=0.52`（一缕纱 0.52s 被吹走）。
- 空间锚：`CHEST=(604,690)`、`SH=[(444,684),(764,684)]`、`FISTS=[(286,904),(972,904)]`、`HC=(604,470)`（头心）、`HR=160/HZ=110`（头半径/穹顶高）、`NECK=(604,575)`、`MOUTH=(604,588)`、`OFF=(941.9,19.4)`（图角在视图中的位置）、`ANCHOR=(604,1450)`（她被拉伸/按压的根）、`WHO=(606,1240)`（出牌者站位）。
- **KEYS 六姿势 × 39 个数值参数**（:98-105）：standby→drawing it in→it bursts→the X behind her→down both sides→stripped。参数语义全部由文件头注释 :84-97 逐条定义（见 ⑧ 节全文收录）——sink/sy/breath/tilt/rise/nod/sleeve/fore/knife/blade_l/blade_w/fan/flare/updraft/part/eye_len/eye_rot/glow/howl/shed/who/zoom/head_s/blast/ring/card/streak/flashc/fly/tail/strip/body/ringa 等。

### 4.2 x_at(t) 全参数插值栈（:120-189）

完整数学见 ⑦.1。结构：`A→B`（蓄力，1.43×coil 超调）+ `→C`（爆发，out 缓动）+ 五个叠加层（trem 颤抖 / bump 过冲回落 / late 头发迟滞 / wob 摆动 / blade/blast/flashc/ring 等过程量）。

### 4.3 风与撕布（:377-601）

注释 :377-382 是物理设定的「散文规范」：尖叫从嘴发出风；第一道波掠过（近的先感到「 knock 」）；之后风渐强，每条纱的**自由端先抬、转向背离她嘴、 flutter 越来越剧、绷紧——然后最外层最先在根部撕裂**，被朝着指向的方向直着吹走，自由端在前，越来越快；没有任何一缕会先往一个方向再往另一个方向。
- `lift_of`（:393-401）：风进度 = `0.20u+0.80u³+0.22x·e^(1-x)·(1-u)`（u=到撕裂的归一化进度，x=波掠过后的时间）——knock 脉冲+三次方渐强。
- `wind_of`（:403-424）：每条纱的风向=纱体质心相对嘴的角度（避开正下/正上，:404-405 注释）+ hash 抖动；`tear=T_PULL+order*PULL_SPAN`（order 由 build 时排序给定：侧发从上到下占前 50%、前发从外到内占后 58%，:1045-1058；「只是一片的碎条最先被风带走」:1064-1072）。
- **wind_turn**（:426-446）：自由端先转根的插值——混合系数 `f=pow(lam,1+1.8(1-u))·min(1,0.25+u/0.10)`（u=从根起的归一化弧长）：梢部 f≈lam 全转、根部 f≈lam^2.8 几乎不动；角度走「短弧」（dl*wsg<0 且 |dl|>1.9 时 ±TAU 绕近路，:443）；叠加 flutter `fa·u·sin(FLUT_K·s-FLUT_W·TT+sph)`，fa=FLUT·lam·(1-0.6lam³)（:436）。
- `torn_of`（:448-459）：**撕开瞬间的姿态一次性算好并缓存**（把 TT/X 临时改成 tear 时刻重算 pose_strand，与当前帧无关——「whatever time it is now: it does not depend on what was drawn before」）。
- `fly`（:470-499）：撕后飞行——位移 `d=FLY_FAR·x²·(0.35+0.65x)`（x=带 FLY_LAG 根部滞后的进度，:493）加速直线；侧漂=flutter+根部鞭打 `whip=30·exp(-tau/0.10)·sin(TAU·7·tau)`（:484，刚脱手抽一鞭）；根上段 lerp 入列（:497）。
- `puff`（:501-529）：撕开处的尘：3 个 sprite，e=(1-(1-y)²) 扩散、alpha∝(1-y)^1.3。
- `pose_strips/pose_bands`（:531-601）： tau≤0 画在身（wind_turn 中）、tau>0 画飞行；GHOSTS=2 重影（GWIDE=[1.7,2.6]、GALPHA=[0.30,0.15]，:603-605）在不能模糊时模拟运动残影。

### 4.4 火：fire_ways / fmap / tails / blades（:738-760, 874-955, 1127-1156, 1164-1225）

- **fire_ways**（:1164-1225）：抛物线火迹——8 个控制点（刃尖→延刃向→越过她上方→背后交叉→远侧→俯冲进他，:1195-1196）→ **Catmull-Rom 601 段细分**（:1200-1209，标准公式 + 顶部 soft 指数折返 `ceil_y+soft·exp((y-ceil_y-soft)/soft)` + x 钳位）→ 累积弧长表 acl → **101 点等弧长重采样**（:1210-1222，线性扫描指针 qq）→ APEX=离 way[3]（最高点）最近的采样 index/100（:1220-1222）。「stepped evenly along its length, so that how fast it goes is fmap's to say」。
- **fmap**（:191-199）：速度曲线——`f<0.45: 0.47(1-(1-f/0.45)^1.8)`（抛出快、渐缓）；之后 `0.47+0.53((f-0.45)/0.55)^1.9`（坠落渐快）；APEX 处「all but hangs」。
- **fnear**（:738-742）：火的视觉大小沿路程 1→0.46（远）→1.12（近）→收 0.5。
- **tails 22 舌**（:914-935）：前 14 个密集（ft=fl-0.085fj，一个火焰的连续身段）、后 8 个稀疏（ft=fl-0.085-0.24fj，空气中渐熄的余火）；位置从 fpath[fmap(ft)] 取、法向抖动、过 APEX 时 reparent 到 trail_front（:933-935，从她背后换到身前）。
- blades（:773-789）：blade 节点挂在 grip（肘位）上，rotation=-knife、scale=(blade_w, max(blade_l,0.01))；fire.gdshader 的 phase/amount 驱动。
- sheds（:874-893）：刃火脱手 3 发/侧，二次贝塞尔 src→ctl→dst（入他身），旋转 lerp、scale 中部膨胀。

### 4.5 空间变形场（:294-347）与 pose（:751-960）

- `d_breath`（:301-305）：高斯膨胀 `SWELL·HB·exp(-dx²/28900-dy²/40000)`（胸部 170×200px 高斯）+ 上提 `LIFT·HB·smoothstep(CHEST.y+90, CHEST.y-130, p.y)`（反向 smoothstep=上半身）。
- `head_rigid`（:314-322）：绕 NECK 的**刚体旋转**（tilt）+ 穹顶 z 随转（`y2=yr·cos+z·sin`，脸面上抬、冠部下后）+ 伪透视 x 扩缩 `(1+z2/1600)` + nod 面内旋转 + head_s 缩放 + rise 拉伸——**闭式解，无迭代**；head_w（:307-308）是影响衰减（HR×0.8~1.3 羽化）。
- `sleeve_k/d_sleeve`（:327-344）：绕肩的衰减旋转（沿肩-拳轴 t>0.5 才开始、两侧 110~170px 外衰减）。
- `where(p)`（:346-347）= p+d_breath+d_head+d_sleeve×2——**CPU 正解**；其**反解在 torso.gdshader 里以 10 次不动点迭代完成**（`wraith/torso.gdshader:76-78`：`p -= 0.85*(where(p)-q)`，与 gd 的 where 逐字同参同常数）。gd 侧对 torso 不做迭代，只喂 uniform（:762-767：breath/tilt/nod/rise/hscale 归零、sleeve_l/r 给 torso_mat、neck 不给袖子）+ 节点变换。
- pose（:751-960）其余：figure 以 ANCHOR 为根的 sy 拉伸+zoom 缩放（:754-760）；horns 双变换保持视长（:797-802，sx=lerp(1,ax2.len/ax.len,0.45)）；jaw/skull/slits 各 Transform2D 从两点刚体变换（:804-814）；eye 火 root/tip 缩放旋转（:816-820）；howl 光环 rings/rings2/cone（:832-861）；backlight/veil/dims（:862-873）。

### 4.6 build（:962-1162）与 set_time（:1227-1246）

build 从 rig2 取 23 个 layer（dress 贴图换 ../tex/dress.png :991；lowL/lowR 的 elbow 存 meta 驱动前臂；torso_c/under_neck/skull/under_jaw/hornL/hornR/fireL/fireR/eyefire/slits + 10 个 glow_*）；12 条身带（bstrands）复用长纱的几何按 12 个锚点摆放（:1073-1111）； strands 建 GHOSTS 重影；风参数在 build 时一次算好（wind_of）。
set_time：`x_at(TT)→pose()` + 自震（`shk=5e^{-s3/0.05}+13e^{-s2/0.24}+1.5(1-sm)`，off=sin/cos 双频，:1240-1246）。

### 4.7 WZ_HEAD（:1259-1437）与 blur/wave（:1439-1517）

与 form 同协议（WZ_HEAD 常量逐字相同 :1267-1285）；twin_tree/twin_walk 整树复制+zone_pair 逐材质换份额材质（uniform 全量拷贝 :1356-1359，fire_mats 特例双份 :1363-1365）；rings/cone 整体移到顶层（:1413-1416）。
blur：wind 越烈 blur 越深（core=near=far 以嘴为中心 :1475-1476）；火的 blur 按速度 `(0.8+1.9·fast)`（:1487-1488）。
wave（:1493-1517）：第一波尖叫把画面推开——BackBufferCopy 读屏 + wraith_wave shader，front=1.5s、power∝1/(1+0.8·front) 衰减。

---

## ⑤ rig.json / rig2.json 完整数据格式

### 5.1 顶层

```
size: [1212, 1704]   # 画布尺寸（她自己的像素）
scale: 2             # 导出倍率（gd 未读取——rig["size"]/rig["scale"]/rig["chest"] 在 gd 中零引用，已核实）
chest: [604, 680]    # 导出器记录的胸口；gd 用自己的常量 CHEST=(604,690)（差 10px，gd 常量为权威）
strands: [25 条]     # 布条（头发/前发）
layers: {7 或 25 个} # 刚体贴图层
```

### 5.2 strand 全字段（25 条完全同构）

| 字段 | 类型 | 语义 | 实例（L1） |
|---|---|---|---|
| name | str | 唯一名；L*=左侧发 R*=右侧发 F*=前发，b/t 后缀=同族副条 | L1 |
| kind | "fan"/"front" | fan=侧发（脑后层 back）；front=前发（脸前层 frontn） | fan |
| family | str | 动画参数族（同族共享摇摆相位 hash，`wraith_form.gd:278`） | L1 |
| tex | str | 贴图文件名（wraith/tex/s_*.png；play 侧读 ../tex3/，`wraith_play.gd:620`） | s_L1.png |
| ox/oy | int | 贴图在 1212×1704 画布中的偏移（uv 减去它，:179-180） | 137/406 |
| pts | [[x,y]…] | 中线折线，发根→发梢，点间距≈8px | 62 点 |
| left/right | [float…] | 每中线点左/右半边宽（轮廓；最大 13~79px） | 62 个 |
| arc | [float…] | 每点**累计折线弧长**（arc[0]=-16 表示首点在根前 16px；步长≈8px） | -16→468.6 |
| len | float | 导出器记录的条带长度（与 arc 末值差 16~48px，疑为平滑曲线长/有效长，**待确认**） | 428.6 |

25 条全表（n=抽稀后点数/原始点数；Lmax/Rmax=最大半宽）：

| name | kind | family | tex | ox,oy | 原始pts | len | arc 范围 | Lmax | Rmax |
|---|---|---|---|---|---|---|---|---|---|
| L1 | fan | L1 | s_L1.png | 137,406 | 62 | 428.6 | -16→468.6 | 44.8 | 31.8 |
| L1b | fan | L1b | s_L1b.png | 224,434 | 45 | 322.3 | -16→338.3 | 30.3 | 29.2 |
| L2 | fan | L2 | s_L2.png | 180,468 | 54 | 358.0 | -16→406.0 | 28.1 | 29.5 |
| L3 | fan | L2 | s_L3.png | 133,532 | 61 | 392.1 | -16→464.1 | 40.3 | 29.5 |
| L5 | fan | L5 | s_L5.png | 167,470 | 62 | 403.5 | -16→475.5 | 29.7 | 48.6 |
| L4 | fan | L5 | s_L4.png | 342,476 | 34 | 218.6 | -16→250.6 | 28.5 | 22.5 |
| R1 | fan | R1 | s_R1.png | 700,396 | 86 | 629.3 | -16→661.3 | 32.8 | 46.9 |
| R1t | fan | R1 | s_R1t.png | 894,403 | 64 | 444.1 | -16→484.1 | 25.8 | 15.6 |
| R2 | fan | R1 | s_R2.png | 684,454 | 31 | 199.8 | -16→223.8 | 31.7 | 13.2 |
| R3 | fan | R3 | s_R3.png | 684,472 | 60 | 364.7 | -16→452.7 | 36.6 | 27.5 |
| R3t | fan | R3 | s_R3t.png | 887,510 | 53 | 363.0 | -16→403.1 | 18.5 | 30.7 |
| R4 | fan | R3 | s_R4.png | 816,556 | 44 | 296.8 | -16→328.8 | 36.0 | 19.3 |
| R5 | fan | R5 | s_R5.png | 696,488 | 63 | 433.1 | -16→481.1 | 51.6 | 79.4 |
| R5b | fan | R5 | s_R5b.png | 718,528 | 62 | 429.6 | -16→469.6 | 18.9 | 35.2 |
| F8 | front | F8 | s_F8.png | 426,392 | 41 | 276.4 | -24→292.4 | 26.3 | 28.0 |
| F2 | front | F2 | s_F2.png | 430,378 | 94 | 691.2 | -24→723.2 | 25.9 | 26.5 |
| F1 | front | F1 | s_F1.png | 358,356 | 113 | 843.7 | -32→867.7 | 26.9 | 29.7 |
| F3 | front | F3 | s_F3.png | 524,351 | 100 | 742.7 | -24→766.7 | 23.2 | 25.9 |
| F3b | front | F3 | s_F3b.png | 538,325 | 78 | 554.1 | -48→570.1 | 34.2 | 26.4 |
| F7 | front | F7 | s_F7.png | 552,339 | 104 | 780.1 | -24→796.1 | 36.9 | 68.3 |
| F4 | front | F7 | s_F4.png | 568,332 | 85 | 607.4 | -40→631.4 | 20.5 | 60.6 |
| F9 | front | F9 | s_F9.png | 618,338 | 38 | 246.0 | -32→262.0 | 63.3 | 30.4 |
| F5 | front | F5 | s_F5.png | 630,358 | 112 | 831.3 | -24→863.4 | 65.9 | 31.1 |
| F6 | front | F5 | s_F6.png | 638,380 | 104 | 773.1 | -24→797.1 | 17.5 | 23.0 |
| F10 | front | F10 | s_F10.png | 648,404 | 46 | 329.4 | -16→345.4 | 30.3 | 20.0 |

合计 1696 个中线点；14 fan + 11 front；15 个 family（L2/L3、L5/L4、R1/R1t/R2、R3/R3t/R4、R5/R5b、F3/F3b、F7/F4、F5/F6 八组共享）。

### 5.3 layers：rig.json 7 个 vs rig2.json 25 个的完整差异

**共有 7 个（form 用 5 个：dress/body/fireL/fireR/glow；glowL/glowR 仅 play 的刃光用）**：

| layer | tex | ox,oy | w,h | 附加字段 |
|---|---|---|---|---|
| dress | dress.png / ../tex/dress.png | 29,529 | 1183,1175 | — |
| body | body.png | 130,177 | 926,1015 | — |
| fireL | fireL.png | 189,598 / 222,652 | 176,276 / 106,216 | k=3.0, shapes=39, foot=(143.9,860) / (260.9,860), tip=(305,660)（rig2 独有 tip） |
| fireR | fireR.png | 862,598 / 896,652 | 183,276 / 116,216 | k=3.0, shapes=35, foot=(1075.1,860)/(971.1,860), tip=(927,660)（rig2） |
| glow | glow.png | 0,0 | 1212,1704 | — |
| glowL | ../tex/glowL.png | 229,658 | 108,227 | — |
| glowR | ../tex/glowR.png | 899,658 | 106,227 | — |

**rig2 新增 18 个**（play 的部件化身体）：torso、torso_c、under_neck、under_jaw、skull、lowL、lowR、lowL_c、lowR_c、hornL、hornR、eyefire、slits、glow_hornL、glow_hornR、glow_lowL、glow_lowR、glow_eye、glow_slits、glow_torso（实际 20 个新键，去重后 rig2 共 25 键）。

关键锚点字段（全部被 `wraith_play.gd:996-1118` 读取）：

| layer | 字段 | 值 | gd 用途 |
|---|---|---|---|
| lowL | elbow / fist | (425,792) / (286,904) | 肘关节锚（low 节点 meta，:997-998） |
| lowR | elbow / fist | (805,785) / (972,904) | 同上；**与 gd 常量 FISTS 精确一致** |
| hornL | base / tip | (566.4,446.4) / (420,183) | 角的根/尖（:1009，视长保持变换的输入） |
| hornR | base / tip | (656.6,450.3) / (708,302) | 同上 |
| eyefire | root / tip | (575.7,536.5) / (506,485.7) | 眼火的根/尖（:1115-1116） |
| fireL/R | foot | (260.9,860) / (971.1,860) | 刃火 shader 的 foot uniform（UV 归一化，:1025） |
| fireL/R/eyefire/slits | k=3.0 | — | **贴图 3 倍分辨率**，sprite scale=1/k（`wraith_play.gd:276`） |
| fireL/R/eyefire/slits | shapes=39/35/6/12 | — | **gd 与 C# 均不读取**（fire.gdshader 的 uniform 只有 phase/foot/amount/seed）——导出器元数据，语义待确认（疑为源画火焰细分数） |

### 5.4 「UE4 骨骼导出成什么格式能复用它」

rig.json 的本质是**两部分**：①布条模拟骨架（中线折线+半宽+弧长）②刚体部件+关节锚点。UE4 侧导出脚本应产出：
- 每条骨骼链 → 一条 strand（沿骨骼采样中线点，每 8px 一点；左右半宽可由骨骼影响的蒙皮宽度/自定义属性导出）；
- 每个刚体部件（头/角/胸/肘/拳/眼）→ 一个 layer（tex+ox/oy+w/h+关节锚点 base/tip/elbow/fist/foot/root）；
- 全局锚（chest/neck/mouth/head 圆心半径）→ 顶层 size/chest 或直接进 gd 常量。
详见 ⑨ 节。

---

## ⑥ suppress_frames.json 全字段语义

### 6.1 顶层（C# 读取处 `SuppressFrames.cs:178-189`）

| 字段 | 值 | C# 用途 |
|---|---|---|
| frames_version | "202" | 未读；对应导出脚本 `docs/ref/suppress_cut/attack_v1/export_game_202.py`（缺失时报错信息里点名，`SuppressFrames.cs:173`） |
| fps | 24 | 帧序列播放速率（Sample/Gather/Standby） |
| scale | 0.38 | **C# 未读**（导出分辨率参考） |
| size | [357,372] | 猎人精灵尺寸（覆盖 C# 默认 430×460）；定位 `pos = (chestX, feetY+0.03num) - (ground.X·k·flip, ground.Y·k)`（`SuppressShow.cs:968-980`） |
| ground | [171.9,336] | 脚底在精灵内的锚点 |
| faces | -1 | 源图猎人朝左；`_flip = (_side==Faces)?1:-1`（`SuppressShow.cs:975`）后 Scale.x=k·flip 翻转 |
| aim_v | 112.0 | **C# 未读**（aim 组的 v 值恰好=112，疑为瞄准位移量） |
| attack_from_aim | 3 | 从瞄准态直接打时 attack 从第 3 帧起播（`SuppressShow.cs:415, 1303`） |
| cut | 9 | 「切」帧：播到第 9 帧触发 Global()+React（`SuppressShow.cs:695-705`）；snapped 恰从第 9 帧起 true |
| slow_end | 36 | 第 36 帧后播放减速 1.45→1.2（`SuppressShow.cs:713`） |
| note | str | 导出说明（锚点定义+朝向+坐标系） |
| groups | intro 15 / idle 40 / aim 40 / attack 59 | 四组帧序列 |

### 6.2 每帧字段（Frame 类只解析 f/c/h/sh/v/f5/snapped，`SuppressFrames.cs:241-251`）

| 字段 | 含义 | C# 消费 |
|---|---|---|
| file | 帧图路径（frames/intro_000.png） | ✔ 加载纹理 |
| f | 前拳位置（导出像素，源 rig arms.left=使用者右手） | ✔ 加权混合后 Map 到场景（`SuppressShow.cs:1004, 1036`） |
| c | 近胸拳（rig arms.right） | ✔ 同上 |
| h | 头位置 | ✔ 解析但未见于驱动（Frame.Head） |
| sh | 前肩位置 | ✔ 解析但未见于驱动 |
| v | 位移量（见 6.3） | 解析（Frame.V）但**当前 C# 未消费** |
| f5 | 淡出量 0→1（attack 第 37 帧起） | ✔ CurrentFade 驱散雾/幕/收手（`SuppressShow.cs:739`） |
| pres | 精灵存在度 1→0（与 f5 反相） | ✘ 未读 |
| snapped | 是否已咬住/切 | 解析未消费 |
| speed | 归一化速率 0~1（起手 0.34→悬停 0→尾段 0.07） | ✘ 未读 |
| i | 源动画帧号（attack 33→91） | ✘ 未读 |
| u | 线性钟 =41.7×帧序（=1000px/s×t） | ✘ 未读 |
| j | idle/aim 源帧号（15→132 步长 3，对应 C# 的 1/3 速循环 `SuppressShow.cs:1055`） | ✘ 未读 |

### 6.3 v 到底是什么（待确认，附数据）

- intro：-1375→-791.7，**严格线性** +41.7/帧（=1000px/s）；
- attack：0→333.3 线性（1000px/s，8 帧）→ 悬停段 ~218px/s（帧 10-36）→ 尾段加速到 ~700px/s（帧 44-58，与 f5 淡出同步）；
- idle：恒定 -1000；aim：恒定 112（=aim_v）。
- 前 9 帧 v 与 u 逐帧相等，之后 v 滞后于 u——**v 是 ease 后的路程、u 是线性钟**（同单位导出 px）的假设最顺；但 intro 为何是负值、idle 为何恒定 -1000 无法从现有材料解释（C# 不用 v，无旁证）。标记**待确认**（疑为导出器给动画作者用的「跳跃弧线参考位移」，或某个已废弃驱动的输入）。

---

## ⑦ 程序化动画数学编目

### 7.1 x_at 插值栈（wraith_play.gd:120-189）——三层叠加

```
第一层（姿势基座）：A(standby) →B(drawing it in) →C(it bursts)
  coil = sm(0,1,clamp(t/(T_BURST-0.06),0,1))          # 蓄力进度
  s = t-T_GO;  u = clamp(s/LUNGE,0,1)
  out = 1-(1-u^1.35)^2.6                               # 前冲缓动：起步快、到达久、过冲
  coiled[k] = lerp(A[k], B[k], 1.43*coil)              # 70% 处即达 B（超调系数 1.43）
  d[k]      = lerp(coiled[k], C[k], out)
第二层（叠加项，全部加减到 d[k] 上）：
  trem  : t∈(0.25,T_BURST) 且 s≤0：tilt +=0.5·trem·sin(TAU·13·CL)；sleeve +=0.7·trem·sin(TAU·13·CL+1)   # 13Hz 待发颤抖
  bump  : s>0：zoom +=0.045·sin(π·clamp((s-0.10)/0.85))-0.04·sm(0.9,2.6,s)；sink +=10·同sin           # 过冲回落+下沉
  late  : 头发迟滞：fan/lerp(B·1.43, C+8·e^{-s/0.40}, sm(0.08,0.66,s))                                  # 头发比身体晚 0.08~0.66s
  wob   : s>0：tilt +=0.8·sin(TAU·6.7s)·sm(0.15,0.5,s)；breath +=0.08·同；eye_rot +=3·sin(TAU·6.7s+1)·sm  # 6.7Hz 尖叫摆动
第三层（过程量，公式覆盖）：
  blade_l=2.5(1-(1-clamp(s/0.26))²) →lerp→1.75 →×(1-sm(1.55,2.35,s))   # 刃火：长出→余焰→熄灭
  blade_w=lerp(1.5,1.2, sm(0,0.30,t-T_REL))
  blast =(44+26·e^{-s/0.30})·sm(0,0.16,s)            # 冲出脸上的纱：指数衰减余量
  flashc=e^{-(t-T_BURST)/0.11}                        # 胸口光闪
  back  =(0.25+0.45·e^{-s/0.40})·sm(0,0.12,s)·(1-sm(1.9,2.7,s))  # 背后光
  ringa =sm(0,0.15,s)·(1-sm(1.9,2.6,s))；ring=(s>0)
  dark  =s≤0 ? lerp(A.dark,0.70,coil) : lerp(0.70, lerp(0.60,0.50,sm(2,2.8,s)), sm(0,0.10,s))
  card/streak：0≤t<T_BURST 期间 1/clamp(t/T_BURST)，否则 -1
  fly   =(t-T_REL)/FLY（0≤f<1.45 有效）
  who   =sm(T_HIT-0.02,T_HIT+0.12,t)·(1-sm(T_HIT+0.7,T_HIT+1.5,t))  # 火入他身点亮
  body  =1-sm(1.00,1.90,s)；glow×=(1-sm(1.5,2.3,s))；eye_len×=(1-sm(2.45,2.80,s))；eyea=1-sm(2.55,2.85,s)；veil=dark·(1-sm(2.3,3.0,s))
```
要点：**同一栈内线性基座+三角函数叠加+指数衰减过程量三者正交**，任何参数可被 Tuning 整体缩放而不破坏其他层。

### 7.2 pose_strand 三阶段（wraith_form.gd:293-376）

见 ③.2。数学要点：head 沿 track 的弧参数运动；`pts[i]=track(head-(last-arc[i])·stretch)`（整条纱按 stretch 沿轨迹重采样，头进梢尾随）；writhe/lash/snap 全部是「法向 × 幅值(弧长 u 的幂) × 正弦/指数」的一阶扰动，无求解器。

### 7.3 wind_turn 自由端先转根（wraith_play.gd:426-446）

```
f(u) = lam^(1+1.8(1-u)) · min(1, 0.25+u/0.10)      # u=归一化弧长（0 根→1 梢）
a   = a0 + Δangle(a0→wd+bow(u-0.5))·f + fa·u·sin(FLUT_K·s-FLUT_W·TT+sph)
res[i+1] = res[i] + dir(a)·|seg|                    # 保长旋转（No turn is made longer or shorter）
```
u=1（梢）：指数=1，f=lam 全转；u=0（根）：指数=2.8，f=lam^2.8 且再乘 0.25——**自由端先动、根最后动**的硬性保证；短弧选择（±TAU 绕近路，:443）保证「不发生反向」。

### 7.4 撕布飞行 fly（:470-499）

`x=clamp((τ-FLY_DUR·FLY_LAG·(1-v))·k,0,1)`，`d=FLY_FAR·x²·(0.35+0.65x)`——**二次加速**（root 滞后 FLY_LAG×0.14 使纱被抽长）；whip=30·e^{-τ/0.10}·sin(TAU·7τ) 根部脱手鞭打；侧漂 flutter+curl·x²。

### 7.5 魂飞行二次贝塞尔（suppress_stage.gd:637-639）

`p=lerp(lerp(start,c,u2), lerp(c,hold,u2), u2)`，u2=eout(tm/flight)——de Casteljau 形式，控制点 c 决定弧线凸向左上。

### 7.6 撕魂时间重映射（:426-434）与 damp/smoothstep 工具箱

tm 分段见 ②.3；`damp(a,b,k,dt)=lerp(b,a,e^{-k·dt})`（:151，帧率无关指数趋近）；`eout(u)=1-(1-u)³`（:148-150）；`s01=smoothstep`。**全项目不用任何物理积分**，全部解析式——这对移植是关键信号（可逐帧确定性重算，支持任意时间跳转/慢放/倒放）。

### 7.7 fire_ways Catmull-Rom + 弧长重参数化（:1195-1222）

8 控制点 → 601 段（每段内标准 Catmull-Rom 4 点式）→ 601 累积弧长 → 101 等距点（指针扫描）。「等距」使 fmap 完全掌控速度；顶部 soft 折返与 x 钳位保证不出屏。

### 7.8 fmap/fnear 速度-大小曲线（:191-199, 738-742）

fmap：两段幂（1.8/1.9）拼出「快抛-顶点悬停-加速坠落」；fnear：1→0.46→1.12→×0.5 的远近呼吸。

### 7.9 呼吸/头部/袖子变形场（form :249-264；play :301-347）

breath_at：42% 相位 smoothstep 吸气 + 余弦余部 ^1.3 呼气；rise_at=2·breath_at(ph-0.30)-1（升降量）；breathe 高斯膨胀（170×200px）+ 反向 smoothstep 上提；head_rigid 见 ④.5；idle lag=对 rise_at 的中心差分（滞后半拍）。

### 7.10 fist_at 双杆 IK 闭式解（form :516-519）

elbow=mid(sh,fist)；f=elbow+rot(fist-elbow, ang.y)；return sh+rot(f-sh, ang.x)——两环旋转解析逆解（等效 LBS：给定肩/拳/两段等长约束的角度参数化）。

### 7.11 torso.gdshader 的 10 次不动点迭代反解（wraith/torso.gdshader:76-78）

```
for (int i = 0; i < 10; i++) p -= 0.85 * (where(p) - q);
```
逐像素求 where 的反函数（q=目标像素，p=源图坐标）。**gd 侧不做迭代**：CPU 只用正解 where() 摆节点（`wraith_play.gd:346-347`，与 shader 的 where 逐字同参同常数——breath/swell/lift/170²/200²/headr/headz/neck/1600 全一致）。这是「CPU 正解 + GPU 反解」的双轨同参设计。

### 7.12 WZ_HEAD 数值正确性推导（form :698-716 / play :1267-1285）

设：纱条真 alpha=a；CanvasGroup 组透明度 outer（wz_outer）；屏幕位置 p 处 UI 占比 u(p)（其软边与 shader 的 w(p) 同剖面：u=1-smoothstep(y0-s,y0+s)=1-w_top 等）；背后景色 B（角色层）/U（UI）。
- 非 ADD 分支：back 份 α_b=outer·a·(1-w)/max(1-outer·a·w,1e-4)；top 份 α_t=outer·a·w。
- 复合（back 组覆于 M=B(1-u)+Uu，再 top 组覆上）：总 alpha = α_b+α_t-α_b·α_t = **outer·a**（代数恒等，代入即证，交叉项 α_bα_t 恰被分母 1-outer·a·w 抵消）。
- 即：**任意 w∈[0,1] 处两份合计=画一次**；w=0 全在角色层、w=1 全在 UI 上、软边各出一份、无重复无缺失。
- ADD 分支（#ifdef WZ_ADD）：加法混合无 (1-α) 项，α_b+α_t=outer·a 天然成立，故免校正——这就是两支分支的数学原因。
- max(…,1e-4)：防 outer·a·w→1 时分母爆炸（ADD 组 outer=1 时不走该支）。
- wz_veil：顶层副本把 dusk 幕色混入 RGB（`form:838`），弥补「UI 上没有场暗」的观感差。

### 7.13 strip 网格与UV（suppress_stage.gd:1268-1285 / wraith_play.gd:359-375）

双边条带 + 累计弧长 UV.x（供 shader 做头尾淡出/脉冲定位/流动）；wraith_play 的 strip() 用 rig 的 left/right 半宽（st[k] 缩放）。

---

## ⑧ 「注释等效物」编目（gd 文件头的 API 文档式注释，逐条中译）

> 两个 wraith 脚本是 Python 生成的「原型移植」，文件头即完整 API 契约；KEYS 字段注释是整套程序化动画的「参数词典」。以下逐条收录（原文英文，附中译）。

### 8.1 wraith_form.gd:1-20（API 契约）

- `build(dir, cache, back, front, lo, hi, org, k)`：back/front=角色身后/身前节点（坐标即场景坐标）；lo..hi=其中在屏部分；org,k=她图角落点与图像素大小。
- `set_state(t, wake, clock, show)`：t<0=纱条在屏幕边等待（wake 0..1=已出来多少）；t≥0=放入后秒数（SNAP 时她绷紧啪地展开，之后呼吸）；clock=自由运行的钟；show 0..1=她与纱条（不含雾）的显隐。
- `times()=[SNAP, 可被出牌接管的时刻, 她安定的时刻]`。
- `phase()=[她头发摇摆的相位, 她的呼吸]`（出牌动画从它们续接）。
- `set_top(top)`：build 之后可选——整个界面之上的节点；她位于界面处（"ui_..."=她占据的窗口带）的部分画在那里、盖在界面之上；场地中央她仍在角色身后。
- `set_param(key,v)`："haze","shake","dusk","alpha","arms"；"ui_top","ui_bot","ui_corner","ui_left","ui_right","ui_soft"=界面在窗口中的比例位置；"haze_ui"=雾画到界面上的比例（0..1）；"blur"=运动物体的模糊量（0=不模糊：mod 要求之前如此）。
- 文件尾：由 `docs/ref/wraith_cut/anim/mk_game_form.py` 生成——改原型重新生成，勿手改。

### 8.2 wraith_play.gd:1-21（API 契约）

- `build(dir,cache,back,front,lo,hi,org,k,who)`：who=出牌者的中部。
- `set_time(t,clock)`：t=出牌后秒数（她的爆发在 T_BURST）；clock=自由钟。
- `set_fade(a)`：全部的她 0..1（待机阶段的交接）。
- `times()=[T_BURST,T_GO,T_HIT,结尾]`。
- `chest()`：她胸口在场景中的位置（卡飞向的点）。
- `set_top/set_param`：同 form；另有 "fire_pad"=她抛的火距窗顶保持的距离、"wave"=第一波尖叫把画面推开的强度（0=不）、"wave_push"=推多远、"puff"=纱撕开处的尘（0=无）。
- 尾注：由 `mk_game_play.py` 从云端原型 `anim2.gd` 生成。

### 8.3 wraith_play.gd:84-97（KEYS 39 参数词典，逐条中译）

- sink：整体下沉（+）/上浮（-）；sy：拉伸（>1）/压扁（<1）。
- breath：0 呼 1 吸，超出=胸廓冲出；tilt：头后仰（+）/低头（-）；rise：颈部的拉伸。
- sleeve：袖摆上扬（+）；fore：双拳上举外张（+）/下收内合（-）；knife：刃尖外张（+）。
- blade_l/blade_w：她的火刃×卡图尺寸；fan：侧发上扬（+）；flare：前发绽开（+）；updraft：发梢浮起。
- part：前发拨开垂她喉胸；eye_len/eye_rot：她眼睛的火；glow：她印记的光；howl：0..1 啸叫升腾。
- shed：-1 无，0..1 刃火脱手飞向出牌者途中；who：他被它照亮。
- zoom：全部的她靠近我们（以胸口为心变大）；head_s：她的头更近。
- blast：她口中吹开脸上蒙纱的气；ring：0..1 尖叫的环朝我们来。
- blade_l 0：她的刀只剩柄；card/streak：打出的卡与她抽出的光（走了多远）。
- flashc：落入她胸的光；fly/tail：刃火脱手的火——头在途中位置（背后、越顶、远侧、落入出牌者）与尾迹展开量。
- strip：吹离她的纱，最外层最后；body：她还剩多少；ringa：环的强度。

### 8.4 其他高价值注释等效物（摘译）

- `suppress_stage.gd:2-9`：整套表演流程的散文描述（悬停→拿起→瞄准→松手→撕魂→放回，逐阶段一句）。
- `wraith_form.gd:690-697`：WZ_HEAD 协议的问题陈述与方案（"each of the two shows of every point only its share… shares that together are the strip drawn once, exactly"）。
- `wraith_play.gd:377-382`：风撕纱的物理设定（第一道波的 knock→渐强→自由端先抬→绷紧→最外层先在根部撕裂→直着吹走，"Nothing of it ever goes one way and then another"）。
- `wraith_play.gd:426-429`：wind_turn 的三不变量（自由端先/根最后/不改变任何一段长度）。
- `wraith_play.gd:448-450`：torn_of 与历史无关（一次性按撕开时刻的姿态计算）。
- `wraith_form.gd:795-796`：角「从正面看后甩会几乎消失；它保长并外摆 instead——这样读得懂」。
- `wraith_play.gd:1198`：「沿长度等距步进，快慢由 fmap 说了算」。

---

## ⑨ 对移植项目的价值（UE4→Godot 骨架中间层设计）

1. **rig.json 格式直接复用为移植中间层**（本报告核心结论）。UE4 导出脚本应产出与 rig.json 同构的 JSON：
   - `strands[]`：每条骨骼链→中线折线（每 8px 采样）+ 左右半宽（可由蒙皮权重宽度或自定义属性导出）+ 累计弧长 + 贴图偏移。Godot 侧 make_strand 类代码零修改接入。
   - `layers{}`：每个刚体部件→{tex, ox, oy, w, h} + 关节锚点（elbow/fist/base/tip/foot/root，即 UE4 的 socket/骨骼位置）。gd 侧全部从 rig 读，无硬编码。
   - 全局锚（chest/neck/mouth/head 圆）→ 顶层字段或 gd 常量（当前 rig 的 chest 未被 gd 读，属于可清理项）。
2. **「参数化姿势字典 + x_at 插值栈」替代 AnimBlueprint 混合空间**：6 姿势×39 参数的 KEYS + 三层叠加（基座/三角叠加/指数过程量）比 UE4 的 BlendSpace+AnimCurve 更轻量、可热调、可确定性重算（无积分）。UE4 侧把动画曲线的关键帧导出为 KEYS JSON 即可。
3. **CPU 正解 + GPU 反解双轨同参**：torso 的 10 次不动点迭代在 shader、gd 只写正解——移植时对「贴图部件的形变」照此办理：CPU 摆节点用正解，像素级蒙皮形变用 shader 迭代反解，两侧常数必须逐字一致（本项目即如此，可当检查单）。
4. **C#↔GDScript 握手协议照搬**：`编译→SetScript→Call("build",…)→每帧 Call("set_time",t,clock)→读返回 Dictionary`；输入只有时间和少量事件，输出只有 {done, shake, hold, cues…}。游戏逻辑与表演彻底解耦，表演可孤立预热（WraithSceneWarmup 的 SubViewport 两帧绘法是最佳实践）。
5. **WZ_HEAD 双份绘制协议**解决「特效要盖 UI 又必须留在角色层」——UE4 移植到 Godot 时 UI 与角色分 CanvasLayer 的场景都会遇到；协议含数值证明（⑦.12），可直接移植（zoned() 的运行时 shader 源码注入是通用手法）。
6. **时间重映射式慢放**（suppress 的 tm/prate 双钟）：表演按「表演钟」走、光点按「慢放钟」走，比全局 timeScale 更可控；UE4 的 slow-mo 移植时建议保留双钟设计。
7. **分帧预热链**（warm() 每次一张图 / WraithStage.Warm 每帧 4 张且 4ms 让步 / 离屏 SubViewport 实绘 2 帧）：移植验收时防卡顿的现成方案。
8. **可清理/待确认清单**：rig 的 chest(680) 与 gd 常量(690) 不一致；len 与 arc 末值差 16~48；shapes 字段无人消费；suppress_frames 的 v/pres/speed/i/u/j 未被 C# 消费（导出链冗余或预留）。

---

## 附：本报告证据索引（关键行号）

| 主题 | 位置 |
|---|---|
| suppress phase 状态机 | suppress3/suppress_stage.gd:99 |
| suppress build | suppress3/suppress_stage.gd:233-312 |
| suppress step 输出字典 | suppress3/suppress_stage.gd:408-538 |
| 撕魂时间线 tm | suppress3/suppress_stage.gd:426-434 |
| smat 魂参数 | suppress3/suppress_stage.gd:673-678 |
| rmat 链参数 | suppress3/suppress_stage.gd:811-814 |
| amat 雾参数 | suppress3/suppress_stage.gd:1057-1060 |
| strip() 网格 | suppress3/suppress_stage.gd:1268-1285 |
| warm() 分帧 | suppress3/suppress_stage.gd:167-177 |
| form make_strand/track/pose_strand | wraith/wraith_form.gd:162-215 / 217-247 / 293-376 |
| form ribbon/wrap/armwrap/shred | wraith/wraith_form.gd:380-512 |
| form WZ_HEAD/zoned/zone_numbers | wraith/wraith_form.gd:698-782 |
| form veil_up/pose_veil | wraith/wraith_form.gd:859-895 |
| form times/phase | wraith/wraith_form.gd:897-901 |
| play KEYS/x_at | wraith/wraith_play.gd:98-105 / 120-189 |
| play fmap | wraith/wraith_play.gd:191-199 |
| play wind_of/wind_turn/torn_of/fly/puff | wraith/wraith_play.gd:403-529 |
| play pose_strips/pose_bands | wraith/wraith_play.gd:531-605 |
| play fire_ways | wraith/wraith_play.gd:1164-1225 |
| play tails 22 | wraith/wraith_play.gd:914-935 |
| play torso/neck_mat 驱动 | wraith/wraith_play.gd:762-767 |
| play twin_tree/twin_poly | wraith/wraith_play.gd:1287-1437 |
| torso 10 次迭代反解 | wraith/torso.gdshader:76-78 |
| rig.json/rig2.json | wraith/rig.json、wraith/rig2.json（本报告 ⑤ 节全表） |
| suppress_frames.json | suppress/suppress_frames.json（本报告 ⑥ 节） |
| C# 驱动：SuppressRun.step | SilentFX.Cards.SuppressRun.cs:497-689 |
| C# 驱动：WraithHeld2.Tick | SilentFX.Cards.WraithHeld2.cs:300-372 |
| C# 驱动：WraithPlayFx.Step | SilentFX.Cards.WraithPlayFx.cs:954-1133 |
| C# 预热：WraithSceneWarmup | SilentFX.Cards.WraithSceneWarmup.cs:88-337 |
| C# 帧元数据：SuppressFrames | SilentFX.Cards.SuppressFrames.cs:154-331 |
