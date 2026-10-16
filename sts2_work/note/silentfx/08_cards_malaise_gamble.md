# 08 二探：卡牌特效矿脉——MalaiseFx（萎靡）与 GambleFlow（赌局）全文深挖

> 证据约定：`MalaiseFx.cs:520` 指 `silentfx_decomp/split/SilentFX.Cards.MalaiseFx.cs` 的第 520 行（split 文件头带原行号映射：MalaiseFx 源 71915-75832，GambleFlow 源 60411-64049，MalaiseHeld 源 75833-75915，MementoHeld 源 75916-79188）。tuning.ini 指 mod 资产 `3817850774/tuning.ini`。shader 指 `3817850774/shaders/`。

---

## 0. 结论先行（TL;DR）

1. **这两个类不是"状态机框架"，而是"每帧一个总驱动函数 + 一堆纯函数求值"的编排式动画**：MalaiseFx 的 `Step(d, dt)`（MalaiseFx.cs:2942）与 GambleFlow 的 `Update(realDt)`（GambleFlow.cs:1701）各只有一条主干，所有动画都是"给一个绝对时间 t，问一堆纯函数要一个值"（Heave/Tension/Surge/PackAt/Part/Spring）。移植时最值得抄的是这个**函数式求值架构**，而非状态机。
2. **MalaiseFx 的本质是"一个敌人的塌陷物理 + 每颗球一条独立时间轴 + 一团 shader 雾"的三层叠加**：Slump 弹簧（k=220/c=21.4）驱动敌人下沉，Orb 状态机（Waiting→Drinking→Greying→Gone）驱动每颗球的拽-咽-撕-碎，MistSheet 把 40 团 Puff 打包成 6 个数组喂给 `malaise_mist.gdshader`。
3. **GambleFlow 的本质是"预测-验证-降级"的赌局替换**：Begins 时预测抽牌堆顶 N 张 → 把旧手牌做成替身副本飞向弃牌堆、新预测牌从抽牌堆飞入手牌槽 → 真牌到达时逐个"合流"（Lerp 到真实 NCard）→ 任何不同步立刻 `Break` 退回游戏原版。它把整个游戏原生抽牌流程**接管**了几秒钟。
4. **任务书边界澄清（重要）**：`plans.*`（弹簧重物 spring=9.3/damp=0.68、蓝丝拽回、划线渗透 plans.seep）**不在 GambleFlow 里**，而在 `RetainLine.cs`（计划妥当）；GambleFlow 只读 `gamble.*` 键，且 tuning.ini 里 gamble.* 大多是 "defaults live in the code"（tuning.ini:625-627）。GambleFlow 与 MementoHeld **无任何代码交集**，二者是共享 CardStandIn/CardDeco/HandHold 设施的姊妹 held 特效。本报告③节会把三者的边界画清。
5. **四代演化史写在 tuning.ini 的中文注释里**（775-863），代码里留着三层考古：仍在读的键（v0.60/62/64）、只声明不使用的"文档常量表"（MalaiseFx.cs:774-820 十八个、GambleFlow.cs:434-500 十个，grep 证实全无常量引用）、以及死资产 `malaise_strand.gdshader`（全代码库无 "malaise_strand" 字符串，v0.64 注释"strand 已删"）。

---

## ① MalaiseFx 全景

### 1.1 类结构与嵌套类型（MalaiseFx.cs:1-484）

`MalaiseFx : CardFx`（sealed，3918 行）。八个嵌套类型各司其职：

| 类型 | 行 | 职责 |
|---|---|---|
| `Orb : EnergyBeads.Bead` | 6-87 | 能量球。~35 个字段：颜色 Own/OwnInk、状态 State、时间戳 DrinkAt/TornAt/GreyAt/FullAt、游荡 Wp/Wv/Spot、弹簧弧 Slow/Might/Arc、闪烁 Flare、所属 Stream |
| `Puff` | 89-122 | 雾团：Pos/Vel/R/Thick/Age/Seen/Anchor/Psi/Seed/Life/Orb/Goal |
| `Ember` | 124-137 | 火星：Dot 精灵 + S（路径参数）+ Lat/Seed/Size |
| `Stream` | 139-184 | v0.62/63 的一股红丝（现为 Sprite 拼装）：Glows[PuffN] + Bodies[(PuffN+1)/2] + 5 Embers + Knot + Lat/Size/Jit 随机数组 |
| `Way`(struct) | 186-215 | 丝的几何路径：C/P1/P2/E 贝塞尔控制点 + B/R/Turns/From/Sign 螺旋参数 + Normal/Sway/Split |
| `MistGroup` | 217-266 | 一团雾组：Chest/W/H/Side/U + Smoke 列表 + Sheet + Wide/Mid/Core 三层辉光 + Taken/Notches/Ember |
| `Slump` | 268-305 | 敌人的塌陷：Home/Wrote/Shift + 弹簧 Y/Speed/Rest + 颤抖 Shake/ShakeSeed/Kicks |
| `Drain` | 307-484 | **会话总装**：三层节点（MistLayer/BeadLayer/FrontLayer）、Orbs/Active/Mists/Streams、41 个 tuning 读出的参数字段、时间戳（Time/PlayAt/ReadyAt/EndAt/LastStepAt/CloseAt/DismissAt）、布尔（Playing/Closing/Dismissed/Dead/Cut/Remote/FromCounter/Pending） |

类级静态：`Live`（所有会话）、`Slumps`、`_ticking`（懒挂帧）、`_resetting`（MalaiseFx.cs:822-828）。

### 1.2 入口与退出编排

```
OnPlay(c)  MalaiseFx.cs:850
 ├─ 远端(MP): drain = (c.Held as MalaiseHeld)?.Take() 拿走手持会话  :857
 ├─ XOf(card) 读 X 值(失败回退数 Orbs)  :1044   Upgraded(card)  :1069
 ├─ 无目标或层数 0 → Close(drain)  :866-873
 ├─ 本地: Begin(d, c, stacks, energyPart, chemPart, extra)  :907
 │    ├─ Beads?.Release()  → Aim(d,c) → Playing=true, PlayAt
 │    ├─ Pick() 三种球各挑 want 颗(不够就 AddOrb 现造)  :1486-1488
 │    ├─ 排班: DrinkAt = ReadyAt + lead + i*gap(±vary 抖动)  :1522-1564
 │    ├─ Sfx.PlayAt("malaise_tear", 第一声时刻)  :1567
 │    ├─ StatusSteps.Begin(图标计数 UI)  :1571
 │    └─ Seize(target) 抓住敌人身体节点  :1583
 └─ 远端: Pending + Prior()/Next() 排队机  :892-921
```

手持侧 `MalaiseHeld`（83 行，全录）：`Build` 调 `MalaiseFx.Create(fromCounter:true)` 并 DevLog 汇报（MalaiseHeld.cs:12-27）；`Tick` 调 `MalaiseFx.Touch` 保活，释放超过 6 秒自动 Leave（:36-43）；`OnTargetChanged` → Aim/Unaim（:45-55）；`OnPlay` → `Leave(20f)` 让位给播放态（:64-67）；`OnLeaving/OnEnd` → `Dismiss`（:69-85）。**手持会话与播放会话是同一个 Drain 的两种宿主模式**（FromCounter 挂在 UI 层，播放挂 Front 层，MalaiseFx.cs:1107-1114）。

退出路径四条（Step 末尾，MalaiseFx.cs:3168-3183）：Closing 且淡出尽 → Free；EndAt 到（最后一股扯断 + SlumpHold + Recover + 0.1，与 1.8s 取大，:1697-1701）；Playing 超 12s 强杀；Dismissed 且球已归位或超 3s。`Free` 逐项 try-catch 释放（ victim/icons/nodes/beads 四段日志前缀，:1952-2015），并驱动远端排队机的下一场（:1984-1987）。

### 1.3 Orb 状态机（MalaiseFx.cs:774-780 + Drink:1613）

```
        Turn()              Full()              Crumble()
 [0 Waiting] ──DrinkAt──▶ [1 Drinking] ──TornAt+Whip──▶ [2 Greying] ──GreyAt+0.24──▶ [3 Gone]
     │  ▲                    │  │                      │
     │  └─ CutShort() 强制把 0/1 全部按到 2（目标死亡/换房）:1900
     └─ AddOrb 出生 Wait=0.15（:1263）
```

每个状态的角色：
- **0 待命**：在 Zone 里游荡（Wander，弹簧+排斥），被 SmokeFor 选为烟雾源（:3288）。
- **1 喝**：时间轴见 1.4；每帧由 `Drink()` 驱动（:1613-1718）：到点 Turn → +Latch 咬住闪 → 每下 Heave 触发 Wrench（火星+敌人 Tug）→ 每下 Gulp 触发 RingPulse → TearAt 到 Tear（撕断：Taken++、图标 StepTo、敌人 Kick、撕裂 Stream、胸口三件套闪光）→ +Whip 到 Full。
- **2 灰**：Fill=1，颜色 Lerp 向 Ash，Spin 减速，0.24s 后 Crumble 碎成灰（两段 Burst：16 片大火星 + 6 片慢灰，:1895-1896）。
- **3 亡**：Show=0，位置冻结。

### 1.4 每颗球的时间线（对齐 tuning.ini:825 的预算注释）

```
t=DrinkAt ──latch 0.12──▶ 咬住(Bit 闪 Fleck)                       MalaiseFx.cs:1642
   ──heave_pull 0.17──▶ 第1拽 Wrench(火星+下沉)                     :2572
   ──heave_grip 0.10──▶ 松一点 Gulp(紫环+咽)                        :2582
   ──0.17──▶ 第2拽   ──0.10──▶ 松   ──0.17──▶ 第3拽=扯断
   ──whip 0.16──▶ Full(胀大、深紫红、环) ──0.24──▶ Crumble(灰碎)
```
预算 = 0.12 + 3×0.17 + 2×0.10 + 0.16 ≈ **1.19s/球**（注释说"约 1 秒"）。错峰：`span/count` 夹在 [gap_min, gap] 之间（:1522），lead 0.1s（:1523），每球 ±vary 抖动（:1564）。

### 1.5 Slump：敌人的塌陷物理（MalaiseFx.cs:2141-2160）

```csharp
// Spring(Slump s, float dt)：半隐式欧拉 + 定长子步
n = clamp(ceil(dt/0.006), 1, 24);  h = dt/n;
for i in n:  s.Speed += (-220*(s.Y - s.Rest) - 21.4*s.Speed) * h;
            s.Y     += s.Speed * h;
// NaN 熔断 → 回到 Rest；最后 clamp(Y, -0.3*Most, Most)
```
- 踢注入：`Kick`（撕断）`Speed += Dip*2*sqrt(220)`，`Tug`（每拽）`Speed += Nudge*2*sqrt(220)`（:2065-2085）——**速度按临界阻尼冲量缩放**（sqrt(k)），保证任何参数下回弹节奏一致。
- `Rest` 累积： Sag*0.5（每拽 1/Heaves 份）/ Sag*0.5（撕断），上限 Sag*6（:2070, 2081）。
- 颤抖：双频双时间常数指数衰减 `Shake*exp(-t/0.08)*(1-exp(-t/0.012)) * (0.6sin97t+0.4sin151t, 0.65sin83t+0.35sin139t)`（:2133-2135）——"一下就停"的质感。
- 恢复：最后一股撕断后 SlumpHold(0.3)s 保持，再 Recover(0.45)s smoothstep 回 0（:2111-2128）。
- `Push` 把屏幕位移写进敌人 Body 节点，并处理"别人也在动这个节点"的冲突（比较 Wrote，:2236-2246）；`LetGo` 精确还原（:2249-2283）。

### 1.6 Wander：球在敌人面前的闲逛（MalaiseFx.cs:2418-2560）

- 目标点：`FirstSpot` 黄金角太阳花分布（`2.39996 rad` 步进 + 半径 sqrt 归一，:2362-2368）；每 1.6~3.2s/Pace 用 `PickSpot` 在 4 个随机候选里挑"离大家最远"的（:2394-2413）。
- 运动：`Wv += ((goal-Wp)*ω² - Wv*2ζω)*h`，ω=1.35*Pace，ζ=1.84/(2×1.35)=0.68（:2503-2518）——**与 plans.damp 同一个阻尼比的闲逛版**。
- 排斥：距离 < ApartOf 时按 `(apart-d)*5` 互推（:2520-2535）。
- 边界：半盒 ±1.15*half 夹紧，速度反向分量清零（:2540-2557）。
- Zone 计算（:2297-2336）按球数动态变宽变高（0.61·n·apart² 面积 → 宽高分配），并按屏幕边缘自动靠近（num10 判定）。

---

## ② puff / strand 数据流

### 2.1 Puff → MistSheet → shader（MalaiseFx.cs:3304-3452 → 486-756）

**生成**（Smoke）：每 SmokeEvery×(0.7~1.3) 秒，`SmokeFor` 从 Orbs 里轮选一颗"待命、已到场、在游荡"的球（:3288-3302），以胸口为中心生成 Puff：初速 = 朝向球 (16~26)·u·num + 随机抖动 + 上飘（:3399-3402），Life 2~2.5s，R0 0.85~1.15。

**逐帧**（:3409-3450）：
- 目标速度 = 指向球的主速度 `40+50·Smooth((110u-dist)/(90u))` + 双正弦侧摆 `15sin(1.9t)+8sin(3.4t)` + 上飘 `-7u`（:3426-3427）；
- 球进入 Drinking 后追加吸入加速 `240u·num·num5` + 切向 `90u·num5`（:3428-3433）；
- `Vel = Vel.Lerp(target, 1-exp(-2.6dt))`（指数趋近，:3434）；
- 半径 `8→22·pow(age,0.7)·R0·u`（:3437）；透明度 Seen 三段曲线（淡入 /0.18、淡出 /0.62+0.38、近距消失，:3382, 3438）；
- 打包进 Sheet：`Add(Pos, R, Thick, Age, Seen, Vel*(0.006/u), Anchor, Psi)`（:3448）——**注意速度×0.006/u 转成"拖尾拉伸向量"**。

**MistSheet 批量上传**（:588-747）：
- `Begin()`：_n=0，包围盒 _lo/_hi 置极值（:594-596）；
- `Add()`：六数组打包 + **防御性清洗**（NaN 全查、reach<1/thick≤0.002/seen≤0.003 剔除、drawn 长度截断 7、包围盒按 reach·2.7·(1+len) 扩张，:632-661）；
- `End(clock, intensity, heat, grain)`：空帧/intensity 过低 → quad 隐藏（:710-722）；屏幕矩形 ∩ 包围盒 + 40px 裕量（:715-717）；**未用槽位清零**（防上一帧残留，:723-731）；quad 尺寸=裁剪矩形、frame 平移到角点（:732-735）；一次 SetShaderParameter × 11（:736-746）。

### 2.2 Strand（v0.62/63 的红丝，v0.64 已降级为 Sprite 拼装）

- `MakeStream`（:3459-3540）：建 PuffN(36) 个 Glow + 19 个 SoftDot Body + 5 Ember + 1 Knot，全部挂在 MistLayer；预生成 Lat/Size/Jit 三组随机数组。
- `OnWay`（:3552-3586）：**双段路径**——前段三次贝塞尔（C→P1→P2→E）+ 沿法线的 `Sway·sin²(πt)·sin(1.7·Time+Seed+5.5t)` 摇摆；后段螺旋（半径 R(1-q) 收缩、角度 From-Sign·Turns·q、y×0.62 扁椭圆——"稍微从上往下看"）。`Split` 按两段弧长比分配（:3804-3806）。
- `Pour`（:3663-3861）：每帧重建 Way，Flow 以 `1.4·(0.6+0.4·tension+1.3·surge)` 速度循环（:3807-3809）；每个 Glow 沿路径取点、按切线角摆放、按 q 淡出；偶数位补一个 Body；Ember 沿路径以 1.25 倍速跑；Knot 在根部脉动。
- 撕裂：`TearStream` 冻结 Ember.S 于 STear，之后 Ember 被 num7 推到终点消失（:3849-3858）；Done 后全部 QueueFree（:3622-3659）。
- **考古层**：`malaise_strand.gdshader`（seg/fl 双数组 + layer 0/1 前后两次绘制）仍在资产里，但全代码库无 "malaise_strand" 引用——v0.64 起雾改走 malaise_mist + Sprite 拼装，strand shader 成死资产（tuning.ini:853 "strand 已删"）。

---

## ③ GambleFlow 全景 + 边界澄清

### 3.1 边界澄清（先回答任务书的三处指认）

| 任务书说法 | 实际归属 | 证据 |
|---|---|---|
| 弹簧重物 spring=9.3/damp=0.68 | **RetainLine.cs**（计划妥当） | RetainLine.cs:314-315 读 `plans.spring/plans.damp`；GambleFlow 只读 `gamble.*` |
| 蓝丝拽回 / 划线渗透 plans.seep | **RetainLine.cs** | RetainLine.cs:682-853 读 `plans.seep/seep_time/seep_light` |
| 悬停预览暗场 | **GambleHeld**（手持行侧转，无暗场；暗场是 murder.held_dark 另一张卡） | GambleHeld.cs:149-190（Lay/呼吸/侧转）；tuning.ini:209 |
| 与 MementoHeld 的关系 | **无代码交集**；共享 CardStandIn/CardDeco/HandHold/HandFlow 设施的姊妹实现 | MementoHeld.cs:735/849 用 CardStandIn.Make/CardDeco.Attach("memento")；GambleFlow.cs:1131/3249 用 CardStandIn.Make/CardDeco.Attach("gamble.edge") |

GambleFlow 的 held 搭子是 `GambleHeld`（236 行）：拖出手牌 → `HandHold.Line` 一字排开 + `GambleFlow.Sideways(true)` 侧转 → 呼吸 `Prepare` → 打/放回时 `Sideways(false)` + `HandHold.Release()`（GambleHeld.cs:139-190, 207-238）。

### 3.2 结构与触发链（GambleFlow.cs）

嵌套类型：`DeckLook`（3D 牌堆观瞻：Width/Shrink/Rise/Dark/Pitch/Thick/Shadow，Read() 从 tuning 读，:6-38）、`Turned`（单张侧转五元组，:40-51）、`Formation`（编队 + `Pose(index, out at, x, y, light)` 逐张求位姿，:53-119）、`EdgeRig`（侧脸多边形 rig，:121-126）、`Flow`（会话，~120 个字段，:128-349）、`Turnee`（侧转中的真实手卡，:351-432）。

```
Init()  挂 9 个 HandFlow 事件 + Ticker.Add(Update)                    :506-522
OnDiscardBegins(cards, cause, n)  cause is CalculatedGamble            :1226
 └─ Begin()  建 Flow（预分配 20+ 数组，读 52 个 gamble.* 键）              :1262-1435
     ├─ MapSlots：弃牌与手槽做一一映射（gamble.pin=1 时钉住原位）        :2249
     ├─ BuildOld：旧手牌逐张做成 CardStandIn 副本 + CardFlight.Claim 接管 :1912
     └─ HandHold.Hold(n, timeout) 撑住手牌位形                          :1424
Update(realDt)                                                          :1701
 ├─ 六个破坏条件：timeout / 洗翘超时 / 捡起别张卡 / 选择中 / 层级亡 / 无槽  :1713-1743
 ├─ KeepUnseen（真牌没到时保持透明）→ Pin（钉回原位）                    :1746-1747
 ├─ StepOld：旧手牌飞弃牌堆      → Raise：造新副本（每帧≤3 张）           :2069 / 2354
 ├─ Cover（预隐藏真牌）→ StepSwaps（合流检测）→ StepNew：新牌飞入手牌      :2392-2704
 └─ Close：收尾判定                                                     :1765
```

### 3.3 两条编舞时间线

**旧手牌离场**（StepOld，:2069-2231）：
```
square(0.2s)  Ripple 从中央向两端错峰收紧成薄叠 → windup(0.06s) 整体后仰 WindPx
 → fly_out(0.3s) Rush 贝兹弧线飞向弃牌堆（航向 Heading×Lean 倾斜、缩到 PileScale）
 → 落地 Puff（PaleFlash×2 + 尘土 + gamble_shuffle 音效）→ 释放副本            :880-904
```
拖尾形变：`formation.Drag = OldVel·TrailTime`（限长 TrailPx），`Lead = clamp(Vel.X/300)` 让后排牌比前排多位移（:2173-2175）——**用速度直接当形变量，不建粒子历史**。

**新牌入场**（StepNew，:2458-2652）：
```
fly_in(0.32s) OutBack(LandOver=0.9 过冲) 从抽牌堆弧线飞入，落地 LandSquash 压扁 + Pulse 冲击
 → PackAt 循环（:2429-2456）：hold(0.1) 薄叠 → open(0.2) Spring(SpreadOver) 展开
   → regather(0.14) Spring(GatherOver) 收紧 → reopen(0.2) 再展开
 → fan(0.26) + stagger(0.02) 逐张"翻正"（Turn 1→0，侧牌转正）                  :2622-2626
 → 与真实 NCard 合流：Lerp 到 Centre[i]/Angle[i]/Scale[i]，Settle 接管后 Drop 副本 :2627-2647
```

### 3.4 侧转：假 3D 牌堆（StepTurn/Stand/Place，:2827-3189）

- `TurnOf`（:605-619）由深度索引推出五元组：`Width=Lerp(1,side,turn)`、`Size=1-shrink·depth·turn`、`Rise=rise·depth·turn`、`Light=1-(1-dark)·(depth/8)·turn`、`Lean=pitch·sqrt(1-Width²)`——**纯 2D 仿 3D，无真 3D**。
- Turnee 生命周期：Calm 2 帧确认节点稳定 → Taken → Going（侧转）→ `Sprung` 弹簧（rate=22/ζ=0.62 转过去，_backRate=14/ζ=0.66 转回来，:2965-2970）→ Stand（:3032，宽度/亮度/升起/落点 Hit 冲击）→ Place（:3072，呼吸位移或 BeginMove 行内移动：开 0.3s Spring、合 0.2s Rush、回 0.28s）。
- 侧脸：`CardDeco.Attach(card, "gamble.edge", ...)` 挂三个 Polygon2D（阴影条 + 纸边 + 描线，MakeEdge :944-1064），`SetEdge` 按当前 Width 求厚度（:1066-1096）；EdgeLeft 移除（:3292-3295）。

### 3.5 预测-验证-降级（GambleFlow 的魂）

- `StillNext`（:2332-2352）每帧核对抽牌堆顶与预测一致；`OnDrawnCardEntered`（:1481-1542）核对到达顺序与身份，不符即 `Reveal + Break`。
- `Break(why)`（:3498-3535）：丢弃全部副本、显示回真牌、日志 `"GambleFlow: back to the game's own draw (" + why + ")"`。**每个 why 都是一句人话**（"card 3 is not the predicted one"、"the draw pile changed"）。
- `Finish(why)`（:3537-3629）：七段清理（副本/隐藏/旧手/Hold/Layer…），每段独立 try-catch，最后日志 `"GambleFlow: ended (" + why + ")"`。

---

## ④ 动画技法编目（公式 + 参数 + 出处）

### 4.1 缓动函数族

| 函数 | 公式 | 出处 |
|---|---|---|
| Smooth | `t²(3-2t)` | MalaiseFx.cs:839；GambleFlow.cs:640 |
| Rush | `t²(1.6-0.6t)`（起步慢收尾快） | GambleFlow.cs:645 |
| OutBack | `1+(s+1)(t-1)³+s(t-1)²`（过冲 s） | GambleFlow.cs:650 |
| Spring(u, over, peak) | 由 `ζ=-ln(over)/√(π²+ln²over)`、`ω_d=π/peak` 反解欠阻尼阶跃，尾部 u∈[0.8,1] blend 回 1 | GambleFlow.cs:656-677 |
| Springy(τ,w,ζ) | `1-e^{-ζωτ}(cos ω_dτ + ζω/ω_d·sin ω_dτ)` | MalaiseFx.cs:2597-2605 |
| Pulse(v) | `v²·e^{2(1-min(v,40))}`（快起慢落脉冲） | GambleFlow.cs:679-686 |
| Bump(v) | `sin²(πv)` | GambleFlow.cs:688 |
| Kicked(t,stiff,damp) | `e^{-damp·stiff·t}·sin(stiff√(1-damp²)t)/farthest`（farthest=首峰归一） | RetainLine.cs:295-303 |

**技法要点**：`Spring(u, over, peak)` 是全书最值得移植的弹簧 API——调参者不读 ω/ζ，直接给"过头几成、几秒到峰"。

### 4.2 数值弹簧（半隐式欧拉 + 定长子步）

| 用途 | 公式 | 参数 | 出处 |
|---|---|---|---|
| 敌人塌陷 | `v+=(-k(y-rest)-c·v)h; y+=v·h` | k=220, c=21.4, h≤0.006, n≤24 | MalaiseFx.cs:2141-2159 |
| 侧转 | `v+=(rate²(target-x)-2ζ·rate·v)h; x+=v·h` | rate=22/ζ=0.62（转）；14/0.66（回） | GambleFlow.cs:694-707, 2965-2970 |
| 球游荡 | 同上（ω=1.35·Pace, ζ=0.68） | Pace=1 | MalaiseFx.cs:2503-2518 |
| 计划妥当重物 | Kicked 解析式 | spring=9.3, damp=0.68 | RetainLine.cs:313-319 |

### 4.3 反复使用的"质感"手法

1. **多指数叠加瞬态**：胸口撕裂闪光 = `0.4·e^{-t/0.06} + 0.1·e^{-((t-0.11)/0.035)²}`（快指数+慢高斯，MalaiseFx.cs:3262）；颤抖 = `e^{-t/0.08}·(1-e^{-t/0.012})`×双频正弦（:2133）；一口咽下 = `e^{-t/0.12}`（:2691）、灰碎环 = `e^{-(t-0.04)/0.07}`（:3121）。**"一下"的质感 = 多个不同时间常数的叠加**。
2. **阻尼趋近**：`FxKit.Damp(x, tgt, rate, dt)` 指数逼近，全库最高频调用（MalaiseFx 中 Show/Turned/Grow/Ember/Vel 全用它，:2845, 3107-3108, 3246, 3434…）。
3. **错峰三件套**：Ripple（编队中央优先，GambleFlow.cs:2194, 2610）、Stagger（逐张，:2622）、vary/Hash 随机（MalaiseFx.cs:1560-1563；`FxKit.Hash(id*prime+salt)` 是全书唯一随机源，:2357-2360）。
4. **拖尾=速度形变**：见 3.3；另有 Brush.Follow 拉丝（沿弧线重采样 `t=(√(0.3025+1.8h)-0.55)/0.9`，GambleFlow.cs:812-821）。
5. **挤压回弹**：Formation.Squash（X 来自拖尾、Y 来自落地/展开，:2201, 2621）；LandSquash=0.08。
6. **双频正弦 bob**：球待机漂浮 `1.3sin(9π/10·BobA·t+id·1.7), 2.8sin(3.895575·BobB·t+id·2.9)`（MalaiseFx.cs:2775）——频率取无理数避免可察循环。
7. **临界阻尼冲量**：踢/拽速度 = 位移×2√k（MalaiseFx.cs:2071, 2082）——任何参数下回弹节奏一致。
8. **过冲调参链**：LandOver 0.9（落地）/ FanOver 0.06（翻正）/ SpreadOver 0.12 / GatherOver 0.09 各自独立（GambleFlow.cs:1373-1376）。
9. **路径参数化流动**：Flow/Ember.S 沿 Way 循环，头部/尾部 Smooth 遮罩（:3817, 3826-3827）。
10. **状态即颜色**：Orb 颜色 = Own→Purple(Turned)→Gorged(Fill)→Ash(Grey) 四段 Lerp（:3113-3151），无数值以外的"状态指示器"。

---

## ⑤ shader 驱动模式（以 malaise_mist 为样本）

### 5.1 驱动链

```
Puff(每团) ──Add()──▶ MistSheet._at/_ax/_ex/_sx/_an/_ro[40] ──End()──▶ 11×SetShaderParameter ──▶ ColorRect quad
```
- **StringName 静态预缓存**（MalaiseFx.cs:490-510）：count/at/ax/ex/sx/an/ro/clock/intensity/heat/grain 九个 static readonly + 构造时 "seed" 直写（:580）。
- **每帧一次批量上传**（:736-746），从不逐 uniform 设。
- **quad 定位术**：`frame.GlobalTransform = 平移(裁剪角点)`、`quad.Position = 0`、`quad.Size = 裁剪尺寸`（:733-735）；空帧 `Visible=false` 早退（:710-722）；裁剪矩形 = 屏幕可见矩形 ∩ puff 包围盒 + 40px 裕量（:715-717，OnScreen 视口反算 :3863-3896）。

### 5.2 六数组语义（shader 注释即文档，malaise_mist.gdshader:17-22）

| uniform | 打包内容 | 代码出处 |
|---|---|---|
| `at[N]` | 位置（canvas px） | _at ← puff.Pos（:652） |
| `ax[N]` | x=半径, y=厚度 0..1 | ← (R, clamp(Thick))（:653） |
| `ex[N]` | x=年龄(破边), y=可见度 | ← (Age, Seen)（:654） |
| `sx[N]` | 拖尾方向，长度=长宽比-1（截断 7） | ← Vel·0.006/u（:646-651, 655） |
| `an[N]` | 该团自带噪声的材质锚点 | ← (Pos+Anchor0)/2（:656, 3441） |
| `ro[N]` | 锚点旋转（rad） | ← Psi（:657） |

shader 端：40 团指数场叠加 `c=thick^2.5·e^{-2.5d2}·smoothstep(0,0.3,seen)`，沿 sx 做椭圆化距离（:93-98），**加权平均出共享噪声坐标 s_q**（防同心环，注释 :116）；`blend_premul_alpha` 自照明（col_rim→col_body→col_core 三段 mix + heat 变量）；alpha=field·intensity·seen（:135-136）。

### 5.3 只驱动不改的 uniform

`wisp/core/cover/warp/col_*` 用 shader 默认值（:23-36），代码只驱动 10 个；`grain` 由 U（敌人尺寸系数）缩放（`1.15·u`，:3451）。**"代码驱动物理量、shader 持有美术量"** 的干净分工。strand shader 的 `layer 0/1` 前后两次绘制思路（malaise_strand.gdshader:27）在 v0.64 后由 Sprite 层级（MistLayer 前/后）实现。

---

## ⑥ 「注释等效物」编目（作者设计意图的提取）

代码几乎无注释，作者把设计意图写进了四个地方：

### 6.1 tuning.ini 中文注释 = 四代演化史（tuning.ini:775-863）

- **v0.58**：能量球吸力量（黑球共享 skewer，:766-773）。
- **v0.60**：球在敌人面前闲逛 + 胸口红光；删除 `spread/spread_time/drink/mist_draw`（:777, 798）。
- **v0.62/63**：拔河式"拽三下、一下比一下狠"；精确时间预算 :825；删除 `back_px/tug/travel/lump`（:798）。
- **v0.64**："找回 v0.58 的感觉，更浓"——改雾漩涡；删除 `strand`（:853）。
- **v0.58 黑球**（:766-773）与 skewer 共用 beads.* 键——代码印证：MalaiseFx.cs:1196-1197 直接读 `skewer.bead_gap/cap_size/cap_dark`。

### 6.2 文档常量表（声明即不使用，grep 全文证实）

| 位置 | 常量 | 实值去向 |
|---|---|---|
| MalaiseFx.cs:782-820 | HopTime 0.42/JumpFar 70/GreyTime 0.24/AshTime 1.8/UiLinger 0.35/FadeTime 0.32/MistIn 0.35/MistOut 0.6/SpringK 220/SpringC 21.4/Swell 0.2/PuffWide 46/Flat 0.62/RunsPerSec 1.4/EmbersPer 5/WaveLead 0.02/WaveRun 0.8/HeaveW 20 | 全部只出现在声明行；实际计算用字面量（如 :1676 的 0.24、:3049 的 220/21.4、:3451 的 1.4） |
| GambleFlow.cs:434-500 | MaxHand 10/CardWide 300/CardTall 422/DepthSteps 8/FaceTight 30/FaceOpen 150/ShadeReach 1.9/Never -100/RowHitTime 0.06 | 同样只声明；实值 10/300/422/8/30/150/-100/0.06 以字面量散落（如 :1275 的 10、:609 的 8） |

**解读**：这是"Claude 式"生成的典型痕迹——先立一张人话参数表（常量名自带语义），演化中改成了字面量却没删表。对读者而言常量表就是**写给下一个人看的参数注释**；AshTime/MistIn/WaveLead/RunsPerSec 这些名字本身记录了灰烬世代与 strand 世代的存在。另：`malaise.steps`（tuning.ini 有注释）在代码中未见读取（StatusSteps.StepTo 无条件调用）——**待确认**（可能由 StatusSteps 内部键控）。

### 6.3 日志文案 = 设计自述

- `"Malaise: N bead(s) of energy leave the counter, M black one(s) the relic, one purple one the card"`（MalaiseHeld.cs:26）——三色球的叙事命名（counter/relic/card）。
- `"GambleFlow: back to the game's own draw (" + why + ")"`（:3507）——降级即交还主权。
- `"the hand turns sideways" / "the hand faces us again (N card(s) turned)"`（:2711）、`"the shuffle is over - a new hand of N (K drawn before it, N asked for)"`（:1691）——把状态机翻译成画面语言。
- 每个 Break/Finish 的 why 都是完整从句（"card 3 came before the shuffle was over"）——**异常路径即文档**。

### 6.4 命名即意图

- 萎靡动词序列：Turn（咬）→ Heave/Wrench（拽）→ Gulp（咽）→ Tear（撕）→ Full（胀）→ Crumble（碎）；敌人侧 Seize/Kick/Tug/Shudder/Sink/LetGo。**一整套物理动词把"力量被吸干"写成了殴打小剧场**。
- GambleFlow 的 Old*/New*（旧手/新手）、Gone/Predicted/Real/Copy（真牌/预测/副本）、Turnee/Turned/DeckLook/Formation。
- shader 头注（malaise_mist.gdshader:1-11）用第一人称英文讲完整设计（"lit from inside… blended premultiplied… each puff carries its own piece of the mist"）。

---

## ⑦ 创意点 Top10（对移植最有启发）

1. **函数式求值动画架构**：一个 `Step(d,dt)` 主干 + 纯函数 `Heave/Tension/Surge/PackAt/Part/Spring(t)`——所有动画都是"给绝对时间问值"，可暂停、可倒放、可错峰，无隐式状态。移植首选（MalaiseFx.cs:2942；GambleFlow.cs:1701）。
2. **`Spring(u, over, peak)` 解析弹簧**：用"过头几成 + 峰值时刻"反解 ζ/ω_d，尾部平滑回 1——调参直觉与代码零翻译（GambleFlow.cs:656）。
3. **六数组雾打包 + 单 quad**：40 团软雾一次 draw call；C# 端六个定长数组 + NaN/极值防御 + 未用槽位清零；shader 端指数场融合 + 每团自带旋转噪声锚点。任何 2D 引擎都能复刻（打包成 uniform 数组/实例化属性）（MalaiseFx.cs:486-747 + malaise_mist.gdshader）。
4. **拖尾=速度→同向形变**：编队整体沿速度方向拉伸（限长）+ Lead 前后错位，零粒子开销的"运动模糊"（GambleFlow.cs:2173-2175, 2581-2583）。
5. **假 3D 牌堆五元组**：Width/Size/Rise/Light/Lean 由深度索引线性推出，一张牌侧转的"厚度感"全在 `Lean=pitch·√(1-W²)` 这类小公式里（GambleFlow.cs:605-619, 621-638）。
6. **多指数叠加做"一下"**：闪光/颤抖/咽下全部是 2~3 个不同时间常数（0.06 指数 + 0.035 高斯；0.08/0.012 双指数 × 双频正弦）的叠加——廉价的"手感"来源（MalaiseFx.cs:2133, 3262）。
7. **临界阻尼冲量**：注入速度 = 目标位移 × 2√k，任意参数下回弹节奏一致（MalaiseFx.cs:2071, 2082）。
8. **错峰三件套 + 黄金角太阳花**：Ripple（中央优先）+ Stagger（逐张）+ vary（每颗 Hash 随机）；首批驻点用黄金角铺开再最大-最小距离挑选（MalaiseFx.cs:2362, 2394-2413）。
9. **预测-验证-降级的流程接管**：赌局把整个原生抽牌接管数秒，但每个不一致都立刻 Break 交还主权并说原因；真牌到达后用 Lerp+Settle "合流"而不是硬切换（GambleFlow.cs:1481-1542, 2627-2647, 3498）。
10. **参数即版本史 + 文档常量表 + 动词命名**：tuning 注释写四代演化与"已删"清单、0=回到旧版惯例、F8 热重载；代码里常量表/日志 why/动词序列三件套保证单人+AI 协作的可维护性（tuning.ini:775-863；MalaiseFx.cs:774-820）。

---

## 补记：TimeFreeze 隐式依赖与键数核对（recon-arch 指路后的增量复核）

**TimeFreeze 是萎靡长动画的隐式前提**（grep MalaiseFx.cs 全文仅两处，均为结构性依赖）：
1. `Create()` 建 Holder 后立即 `TimeFreeze.KeepLive(drain.Holder)`（MalaiseFx.cs:1170）——整个 Drain 节点树（雾层/球层/前层）注册为子弹时间"续命"对象，萎靡的 1.19s/球长动画因此不会被时停回收。
2. `Begin()` 抓敌人身体的前置条件含 `!TimeFreeze.Active`（:1581）——子弹时间激活时不 Seize 敌人（避免两个系统同时写敌人 Body 节点打架；Slump/Push 的冲突处理亦见 :2236-2246）。
3. 相关：`CutShort()` 的触发条件之一就是 `TimeFreeze.Active`（:3127）——时停开始时正在喝的球直接进入灰化收尾，不等拽完。

**gamble.* 键数核对**：GambleFlow.cs 本体读取 **52** 个去重 gamble.* 键（grep `Tuning.[FI]("gamble.` 实测，含 edge/edge_height/edge_round/edge_shadow 四个侧脸键与 turn_ease/turn_snap/back_stagger 等侧转键）；加上 GambleHeld.cs 的 ground/breathe/breathe_px/gather/pick_lift/queue_wait/play_wait/held_row 后总量约 60，与 recon-arch 统计的"gamble 59 居首"吻合。全部 462 个隐藏键的逐键默认值+调用点全表见 01_architecture.md 附录 A，建配置字典直接取用，无需重读本报告参数表。

**malaise 键数核对**：MalaiseFx.cs 读取 41 个 malaise.* 键（本报告①.1/⑥.1 已逐条列行号），另跨卡复用 skewer.bead_gap/cap_size/cap_dark（:1196-1197）——与 03_notes.md"malaise 41"的前缀统计一致。

---

## 附：待确认清单

1. `malaise.steps`（tuning.ini 有注释）在 MalaiseFx 中无读取点，图标计数是否受开关控制待确认（疑在 StatusSteps 内部）。
2. MalaiseFx 常量表（AshTime/GreyTime/MistIn…）与字面量并存的原因，推测为 v0.62→v0.64 重构未接线，无直接证据。
3. `malaise_strand.gdshader` 确认无代码引用（死资产），但为何仍随包发布未知（可能 ShaderWarmup 预热清单残留）。
4. GambleFlow 的 `gamble.*` 键大多未列在 tuning.ini（"defaults live in the code"），完整默认值以代码 `Begin()`（:1317-1390）为准，本报告已按代码记录。
