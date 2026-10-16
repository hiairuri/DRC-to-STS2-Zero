# SilentFX 特效框架核心深挖报告（二探 · deep-fx-core）

> 对象：杀戮尖塔2 mod「刃雨毒潮 SilentFX」v0.72.7（作者奶酪，Claude 编写）
> 范围：FxKit 特效标准库、HandMoves 手牌编排层、HandFlow 事件总线、时钟/生命周期/预热三件套、AttackFeed 攻击追踪、Brush/Props/Shade/Card* 挂件体系
> 证据：`split文件名:行号`（切分文件内行号）。读不透处标「待确认」。
> 结论先行：**SilentFX 的可复用内核不是某段动画，而是四件基础设施——「双 pass 帧泵 + 租约回收 + 声明式时间线 + 事件总线」，加上一个全静态、零外部依赖的程序化特效标准库 FxKit。3000 行编排不靠状态机，靠「Spec 声明 + Shaper 函数 + 每帧轮询」三件套平铺直叙。**

---

## ① FxKit API 全表（SilentFX.Core.FxKit.cs，1071 行，源 35715-36785）

### 1.1 定位与形态

`public static class FxKit`——纯静态门面，**无一个实例成员**；全部特效通过静态工厂造节点、由调用方挂载。自带 3 个懒加载程序化资源（SoftDot/Bubble 贴图 + Additive 材质），**零外部图片依赖**，`Warm()` 统一预热（FxKit.cs:128-140，失败只 Warn 不崩）。

### 1.2 程序化资源与基础件

| API | 签名要点 | 实现要点 | 可搬性 |
|---|---|---|---|
| `SoftDot` | 属性，64×64 | `GradientTexture2D` 径向：offsets 0/0.35/1 → alpha 1/0.5/0；Fill=Radial，FillFrom(0.5,0.5)→FillTo(1,0.5)（FxKit.cs:34-55） | ★★★ 一片渐变贴图打天下 |
| `Bubble` | 属性，64×64 | 5 段 offsets 0/0.55/0.8/0.9/1 → alpha 0.1/0.16/1/0.55/0，壳+高光一笔画出（FxKit.cs:102-124） | ★★★ |
| `Additive` | 属性 | `CanvasItemMaterial{BlendMode=1}` 单例（FxKit.cs:65-73） | ★★★ Godot 即 `CanvasItemMaterial.blend_mode = ADD` |
| `Quad(size, material)` | → ColorRect | 白底、MouseFilter=Ignore（FxKit.cs:142-159） | ★★★ |
| `Glow(color, size)` | → Sprite2D | SoftDot + Additive + `Scale=size/64`（FxKit.cs:161-179） | ★★★ 最小发光单元 |
| `Ramp(offsets, colors)` | → GradientTexture1D，宽 64 | 粒子 ColorRamp 工厂（FxKit.cs:181-197） | ★★★ |
| `FadeInOut(peakAt=0.15)` | → GradientTexture1D | 0/peak/1 三点 alpha 0/1/0（FxKit.cs:199-213） | ★★ |
| `Shape(points, color)` | → Polygon2D | Antialiased=true（FxKit.cs:691-705） | ★★★ |
| `Star(points, outer, inner)` | → Vector2[] | 从 -π/2 起内外交替（FxKit.cs:707-719） | ★★ |
| `Hash(i)` | → float∈[0,1) | GLSL 经典 `sin(i·127.1+311.7)·43758.547` 取小数（FxKit.cs:1053-1057） | ★★★ 确定性伪随机 |
| `Alive(o)` | → bool | `IsInstanceValid` + `IsQueuedForDeletion` **双检**（FxKit.cs:215-227） | ★★★ 全库 400+ 处调用 |
| `Bezier(a,b,c,t)` | → Vector2 | 二次贝塞尔解析式（FxKit.cs:229-241） | ★★★ |
| `Damp(current,target,rate,dt)` | → float | `Lerp(cur,tgt, 1-exp(-rate·dt))` 帧率无关阻尼（FxKit.cs:422-425） | ★★★ 指数阻尼，Godot 无内置 |
| `ViewSize()` | → Vector2 | try-catch + 1920×1080 兜底（FxKit.cs:427-455） | ★★ |
| `FreeAfter(node, seconds)` | void | 转调 `FxLifetime.Watch`（FxKit.cs:417-420） | ★★★ |

### 1.3 形状/线条原语

| API | 签名要点 | 实现要点 | 可搬性 |
|---|---|---|---|
| `Ribbon(width,color)` | → Line2D | WidthCurve (0,0.05)(0.7,1)(1,0.6) 头细尾粗；Gradient 头透明/中 0.7/尾实色；Joint=Round、两端圆帽、Additive、抗锯齿（FxKit.cs:243-298） | ★★★ 拖尾/刀痕标准件 |
| `Blade(length,halfWidth,light,dark,hilt,glow)` | → Node2D | 外发光 Glow（7×半宽、1.7×长）+ 左右两片不对称 Polygon2D 刀身（light/dark 双面明暗）+ hilt.A>0.01 才建三段剑柄（护手 1.8×/握把 0.42×/刃座 0.7×，中段 `hilt.Darkened(0.25)`）（FxKit.cs:319-415） | ★★  alpha 阈值决定子节点增减的模式可搬 |
| `Crescent(from,to,bulge,width,color,segments=22)` | → Line2D | 弦法向偏移 bulge 得控制点，22 段 Bezier 采样；WidthCurve 0/0.55/1→0/1/0 月牙（FxKit.cs:566-637） | ★★★ 弧刀/斩击线 |
| `Beam(...)` | 三层 Line2D | 外 2.6×宽 alpha 0.25 / 中 1× alpha 0.85 / 芯 0.36× 近白 alpha 0.95；时间线=extend 画线(0.02→1)+hold+fade，`static local function Layer(w,c)` 生成（FxKit.cs:939-1051） | ★★★ 激光标准三层法 |
| `RingPulse(...)` | 12 参 | Sigil 着色器 quad + shader 参数 radius0.82/width/dashes/glow0.35/alpha0；缩放 from→to、alpha=peak·(1-t)、spin=t·3；delay 启动、Trans=Quad Ease=In（FxKit.cs:846-911） | ★★ 依赖 sigil shader |
| `Flash(parent,at,color,size,grow=2.2,time=0.3)` | void | Glow + 并行 Tween（scale×grow、alpha→0）（FxKit.cs:913-937） | ★★★ |
| `Sigil(size,tint,out mat)` | → Node2D | `ShaderLib.NewMaterial("sigil")` + Quad 居中（FxKit.cs:545-564） | ★★ |
| `Veil(color)` | → ColorRect | 9000×7000 大帘幕（FxKit.cs:639-661） | ★★ |

### 1.4 粒子/行为原语

| API | 签名要点 | 实现要点 | 可搬性 |
|---|---|---|---|
| `Burst(...)` | **16 参**，→ GpuParticles2D | EmissionShape=Sphere(radius)；Direction 默认 (0,-1)；Spread=180；ColorRamp 三段 1/0.8/0；OneShot+Explosiveness0.95+Randomness0.5；DisableZ；**VisibilityRect 3200²防被剔除**；`FreeAfter(life+0.7)`；ReduceScreenEffects → `amount=ceil(amount·0.65)`（FxKit.cs:457-543） | ★★★ UE→Godot 即 CPUParticles/GPUParticles 参数映射 |
| `Bubbles(...)` | 12 参 | EmissionShape=Box、Direction(0,-1)、Spread22、Gravity(0,-70) 上浮、Randomness0.7、Bubble 贴图；`oneShot=false` 时 Explosiveness=0 且**不 FreeAfter**（FxKit.cs:721-796） | ★★★ 常驻/一次性双模式 |
| `Drive(owner,seconds,step,trans,ease)` | → Tween | TweenMethod 0→1 逐帧回调；`failed` 标志**一次性熔断**（炸一次不再调）+ Alive 检查 + "Effect tween failed"（FxKit.cs:663-689） | ★★★ 万能逐帧驱动器 |
| `After(parent,delay,action)` | void | delay≤0.001 或不在树中→同步执行（catch "Immediate effect failed"）；否则建 `SilentFX_Delay` 空节点 + TweenInterval + TweenCallback（FxKit.cs:798-844） | ★★★ 延时节拍的唯一入口 |

### 1.5 ReduceScreenEffects 减量策略（单点收口）

所有强视觉效果汇入一个私有闸门 `AccessibleColor(color, flash=false)`（FxKit.cs:1059-1073）：开启后 alpha×0.6（Flash 类×0.35）。加上三处局部减量：Burst 粒子数×0.65（FxKit.cs:497-500）、RingPulse peak×0.55（FxKit.cs:868-871）、Flash 走 flash 分支。开关由 `PlayerSettings.ReduceScreenEffects` 提供（默认关，设置页文案「减少闪光和强屏幕效果 / Reduce flashes and strong screen effects」，HandPaceSettings.cs:52）。**这是整个 mod 唯一横切关注点，收口在 4 行代码里——移植时照抄这个结构。**

---

## ② 手牌编排架构（SilentFX.Cards.HandMoves.cs，3041 行，源 64050-67090）

### 2.1 核心抽象：不是状态机，是「声明式 Spec + 纯函数 Shaper + 每帧轮询」

Claude 组织 3000 行编排的方式可以一句话概括：**每种抽牌/打出动画 = 一个 `CardRide.Spec` 声明 + 一组 `Shaper` 纯函数 + 若干每帧挂件回调，全部塞进统一的 Acts 列表由 `Update` 平推**。没有显式状态机、没有 async/await 编排、没有行为树。

```
HandMoves.Init() 订阅 12 个 HandFlow 事件 + Ticker.Add(Update)   HandMoves.cs:263-290
Update(realDt) 每帧五路并行，各自 try-catch + 恢复：              HandMoves.cs:2950-3029
  ├─ StepActs(num)      进行中的替身演出（数组快照迭代）
  ├─ UpdateCover(real)  Prepared 预隐藏遮盖管理
  ├─ UpdateHop(real,num) 诡诈卡倾斜弹簧（sly lean）
  ├─ UpdateSly(real)    诡诈打出后飞回手中
  └─ UpdateTies(real,num) → StepStays(dt)  Expertise 留牌驻光
```

**声明式时间线 `CardRide.Spec`**（CardRide.cs:19-36）：

```csharp
public sealed class Spec {
    public float Duration = 0.3f;          // NewRide 钳 0.05~1.6
    public float Delay;
    public Pose? From;                     // 缺省=读真身实时 pose
    public Shaper Shape;                   // ref Pose, float t, in Pose from, in Pose to
    public Node Parent;                    // 缺省=GameRefs.UiLayer
    public Action<NCard> Started;
    public Action<NCard, Pose, float> Frame;   // 每帧挂件回调
    public Action<Pose> Arrived;
}
public delegate void Shaper(ref Pose pose, float t, in Pose from, in Pose to);
```

`Pose = {Position, Rotation, Scale, Alpha}`（CardRide.cs:6-15）。**Shaper 是纯函数整形器**——位置/旋转/缩放/透明度四个通道的每帧表达式，测试与复用零成本。

**替身演出模式（全库通用）**：真身 NCard `Hide(alpha=0)` → `CardStandIn.Make` 造同模替身挂 UiLayer（输入旁路：MouseFilter=Ignore+MouseBehaviorRecursive）→ 替身按 Spec 演出 → 结束 `Free(替身)+Show(真身)`。`Perform()`（HandMoves.cs:540-617）与 `CardRide.ToCard()`（CardRide.cs:74-110）是两条入口，前者自管 Acts 列表、后者挂 Ticker late pass。**实时目标**：每帧 `CardStandIn.Pose(real)` 读真身位姿作为 to，因此手牌被鼠标掀起/重排时动画自动追随（CardRide.cs:322、HandMoves.cs:759）。

### 2.2 五种子结构（以实际代码为准）

| 子结构 | 触发 | 运动核心 | 代码 |
|---|---|---|---|
| **Act**（替身演出容器） | 全部抽牌动画的底座 | 列表管理 + 快照迭代 + 异常 EndAct | HandMoves.cs:17-34, 540-797 |
| **Streak**（拖尾辉光） | DartBack 飞行 | 单 Sprite2D 按帧间速度拉伸（len=clamp(Δd/Δt·0.075, 24, longest)），MoveChild 到替身下层 | HandMoves.cs:6-15, 471-538 |
| **Whirl**（绕体旋飞） | EscapePlan 抽牌 | 三段贝塞尔/椭圆轨道：0-0.275 俯冲至胸上方、0.275-0.725 椭圆半圈（CircleIn/CircleOut 常量）、0.725-1 弹回；尾迹 Ribbon 双线 0.11s 时间窗重采样 + 4 枚 Afterimage 残影 | HandMoves.cs:36-63, 1600-1834 |
| **Mix**（混抽错峰） | Acrobatics 多抽 | `MixTimes`：28 次随机排列（前 24 次全洗、后 4 次相邻交换防同序），找一个「最大错峰≤上限」的洗牌结果，给每张牌一个离场时刻 | HandMoves.cs:65-74, 997-1139 |
| **Ghost**（幽灵虚影） | Prepared 抽牌 | 阻尼跟踪槽位（`1-exp(-14Δt)`）+ Blink 双脉冲（t=0.36/0.57 各闪一次）+ Solid(0.72 后)凝实 + 440×570 大辉光 | HandMoves.cs:76-97, 1322-1470 |
| **Hopper**（倾斜弹簧） | 诡诈卡待弃时持续 | 弹簧积分子步（dt 钳 0.1、每子步≤0.016s；k=13²、ζ=0.68/0.9）+ 双正弦摇摆（3.3/5.7Hz）+ 鼠标避让 + 抬升 lean | HandMoves.cs:99-124, 2433-2691 |
| **Tie/Stay**（留牌驻光） | Expertise 保留抽牌 | 弹簧积分下压 duck（676·x−40.56·v）+ Sheen 扫光；turn ending 时检测留牌 | HandMoves.cs:142-174, 1895-2150 |
| **Under**（屏底穿入） | Expertise 抽牌 | 0.58 前下潜至 `y+211·scaleY+14`（屏外）、0.58 后 BackOut 浮回；Glint 蓝反光条扫过屏底 | HandMoves.cs:176-183, 1227-1320 |

### 2.3 五种抽牌演出的运动公式（可复用的「手感」资产）

1. **后空翻（Backflip）**：Hop 抛物线 `4h·u(1−u)` + 0.85s 后落地压扁（scaleX×1.06、scaleY×0.92 正弦挤压）+ 整圈 360° 翻滚（snap 指数控制）（HandMoves.cs:872-946）
2. **弹簧入场（SpringIn）**：hang 相位偏移缓入 + 抛升 `4·rise·u(1−u)`（rise 最高 520，且不超过屏顶−250）+ N 圈整旋 + 0.85 落地挤压；Mix 等待期原地压扁（HandMoves.cs:1165-1225）
3. **绕胸旋飞（Whirl）**：见上表
4. **幽灵浮现（Ghost）**：见上表
5. **回飞（DartBack，诡诈打出后飞回）**：三次缓出 + 沿运动方向拉伸（0.2·sin(πt) 各向异性）+ 最小尺寸保底 + 金色 Trail + 抵达 Flash（HandMoves.cs:2844-2948）

**关键时间常量全部命名并走调参表**：`Land=0.85`（落地时刻）、`CircleIn=0.275/CircleOut=0.725`（旋飞出入口）、`Depth=0.14`；每个时长都经 `Seconds(key, def)=Clamp(Tuning.F(key,def),0.08,1.5)·Pace()`（HandMoves.cs:369-372），Pace 来自 HandPace 快/慢/关三档。

### 2.4 与 HandFlow 事件总线的对接

HandMoves 只认 `HandFlow` 的 12 个事件（HandMoves.cs:271-281）：DrawBegins（Prepared 预隐藏）、DrawnCardEntered（按 cause 分派到 5 种 Ride）、DrawEnded、DiscardBegins/Ended（认领弃牌飞行）、SlyPlayBegins、PlayerTurnBegins/TurnEnding、HandDrawBegins、SelectionBegan/Ended。**HandMoves 从不直接碰游戏 API——所有游戏状态变化经 HandFlow 归一化**。

### 2.5 HandFlow 事件总线本体（SilentFX.Core.HandFlow.cs，423 行，源 38700-39119）

- 16 个 event（HandFlow.cs:48-80）：DrawBegins/DrawEnded/ShuffleBegins/ShuffleEnded/DrawnCardEntered/DiscardBegins/DiscardEnded/CardDiscarded/SlyPlayBegins/PlayerTurnBegins/TurnEnding/HandDrawSized/HandDrawBegins/CardDrawn/CardEnteredHand/SelectionBegan/SelectionEnded
- **嵌套会话栈** `DrawStack`（HandFlow.cs:32、151-170）：BeginDraw 压栈、EndDraw 按 info 引用移除——支持「抽牌中触发抽牌」
- **600ms 残留窗** `RecentDraws`（HandFlow.cs:34、225-226、262-281、297-311）：EndDraw 时不扔，设 `Until = FxClock.Milliseconds + 600`；`NoteDrawEntered` 先查栈、再倒序查残留——**解决卡牌异步入场（晚于 DrawEnded）的归属问题**；`UpdateRecentDraws` 惰性清扫（HandBackend 每帧调，HandFlow.cs:297-311、HandBackend.cs:27-37）
- **泛型 Raise 逐个调用**（HandFlow.cs:361-422）：`GetInvocationList()` 逐订阅者 try-catch，日志 `"HandFlow.{name}: "`——一个坏订阅者炸不掉总线
- `DrawInfo{Player/Cause/FromHandDraw/Requested/Entered/Shuffled/ShuffledAt}`（HandFlow.cs:6-21）：Shuffled 标记让 Mix 混抽在洗牌时放弃错峰（HandMoves.cs:1003-1007）
- MP 过滤：所有 Raise 过 `Mine(player)`（LocalContext.IsMe，异常默认 true）（HandFlow.cs:134-149）

### 2.6 三档节奏 / 保持位形 / 手牌查询

- **HandPace**（160 行）：`Mode{Off,Fast,Slow}`；Fast → `Scale=Tuning.F("hand.fast_scale",0.6)` 钳 0.1-1；探测 RitsuLib 设置页（每 0.5s 一次、6s 放弃，找不到退回 user_settings.ini）（HandPace.cs:125-159）
- **HandHold**（391 行）：`Hold(size 1-10, seconds)` 临时抬高手牌下限（`RaiseSize(ref handSize)`）；`Line(spacing,seconds,lift)` 一排展开；`Squeeze(amount=0.8, hold=0.12)` 向质心收拢+角度归零；`Slot/LineSlot` 输出经全局变换的槽位；`[ThreadStatic] _insideLinePosition` 重入保护（HandHold.cs:22-23、122-162）
- **HandCards**（261 列）：`Entry{Card/Node/Holder}`；Fill/FillSelected（`AccessTools.FieldRefAccess` 懒反射，一次性失败标记 `_selectedLooked`/`_hoveredLooked`）；NodeOf/Focused/Selecting/EndTurnHovered（HandCards.cs:78-260）
- **HandBackend**（75 行）：Init 编排 CardRide.Init+CardDeco.Init+HandHold.Init+Ticker；Reset 四段各自 try-catch（HandBackend.cs:8-74）

---

## ③ 时钟与生命周期三件套

### 3.1 Ticker 双 pass 帧泵（SilentFX.Core.Ticker.cs，154 行，源 45214-45364）

```
Tree.ProcessFrame      += OnFrame      → Dispatch(Subs, ...)     主 pass
RenderingServer.FramePreDraw += OnLateFrame → Dispatch(LateSubs, ...) 渲染前 pass
```

- **dt 三保险**（Ticker.cs:83-94）：① 墙钟 `Time.GetTicksUsec` 差；② `Engine.TimeScale>0.001` 时用 `GetProcessDeltaTime()/timeScale` **还原未缩放宽 dt**（子弹时间动画不减速的关键）；③ `Clamp(dt, 0, 0.25)` 封顶（防挂起后大跳）；`_lateDt` 累加同样封 0.25
- **订阅管理**（Ticker.cs:44-78）：`Members` HashSet 去重 + `Subs` List 保序；Add 时会从 Late 摘除（互斥，防双泵双调）
- **快照迭代 + 自愈**（Ticker.cs:113-153）：`ArrayPool.Rent` 拷贝回调数组、**倒序遍历**、`members.Contains` 二次确认（容忍迭代中增删）；回调炸 → `DevLog.Error("Ticker callback failed and was removed: ")` + **自动 Remove**——坏订阅者被熔断摘除，帧泵永不死亡
- 谁用 late pass：CardRide.Update、CardDeco.Update——**渲染前一刻读位姿最准**（CardRide.cs:65-72、CardDeco.cs:113-120）

### 3.2 FxClock 特效时钟（SilentFX.Core.FxClock.cs，30 行，源 35688-35714）

静态构造即 `Ticker.Add(Update)`（FxClock.cs:10-13）；毫秒累加 `= realDt × Clamp(TimeScale, 0, 4)`（FxClock.cs:15-29）——**故意跟随 TimeScale**：子弹时间（TimeScale 放大）下特效时钟同步变慢，600ms 残留窗、CardFlight 租约、AttackFeed stale 判定全部自动进入慢动作，与动画一致。`Milliseconds => ulong`。

### 3.3 FxLifetime 租约回收（SilentFX.Core.FxLifetime.cs，142 行，源 36786-36924）

```
Lease{ Node, Room(=GameRefs.CombatRoom 快照), Remaining, Cleanup }
Update(dt):  dt = realDt × Clamp(TimeScale,0,4)
  逐条倒序：
    ① Room==当前CombatRoom && Alive && IsInsideTree && TimeFreeze.HoldsVisual(node) → continue（豁免，不扣时）
    ② Remaining -= dt；>0 且 Alive && IsInsideTree → continue
    ③ 否则移除：Cleanup?.Invoke() + QueueFree()（各自 try-catch）
  StopIfEmpty：列表空 → Ticker.Remove(Update)，零空转
```

设计要点：**Room 快照**——跨战斗/回主菜单时旧房间租约立即失效回收，防节点串场景；**TimeFreeze 豁免**——子弹时间冻结的视觉节点（Known.Ours && ProcessMode==4）暂停倒计时（TimeFreeze.cs:213-238、FxLifetime.cs:104）；**空转自动摘除**——没有租约就不占帧泵。

### 3.4 AssetWarmup 分帧预热（SilentFX.Core.AssetWarmup.cs，174 行，源 31976-32146）

- `record Work(Name, Step:Func<bool>, Cancel, CanRun)`（AssetWarmup.cs:6）；Queue + `HashSet<string> Names` 去重
- **每帧只跑一个 work**（AssetWarmup.cs:89-153）：队列轮转找第一个 `CanRun()` 为真的，Stopwatch 计时执行
- **只在安全窗口运行**（AssetWarmup.cs:73-83）：`CombatRoom==null && !Tree.Paused`，且连续 0.5s 无战斗（`_quiet`）
- **分步任务**：`Step` 返回 false → 重新入队下帧继续；Cancel 用于失败回滚
- **8ms 预算披露**（AssetWarmup.cs:137-140）：`≥8ms → DevLog.Info("Prewarm slow unit: {name} {ms} ms (outside combat)")`
- **排空统计**（AssetWarmup.cs:160）：`"Prewarm queue drained: {completed} jobs, {failed} failed, CPU {cpuMs} ms, slowest unit {worstMs} ms; GPU timing requires in-game profiling"`
- 设置变更 → `ShaderWarmup.Pause()`（AssetWarmup.cs:58-64）

### 3.5 FrameDiagnostics 自计开销（SilentFX.Core.FrameDiagnostics.cs，113 行，源 35578-35687）

- 启用条件（FrameDiagnostics.cs:42-56）：`Tuning.I("perf.trace",0)!=0 && CombatRoom!=null && !Paused`
- **每 5s 一报**（FrameDiagnostics.cs:72-83）：`"Perf (CPU only): {frames} frames/{elapsed}s, wall peak {ms} ms, >33ms={n}, >100ms={n}, main-thread allocation={MiB} MiB, GC={gc0/gc1/gc2}"` + 按 CPU 总时排序的 Top5 `"Perf callback: {Type.Method}, calls=, CPU total=, peak="`
- 与 Ticker 的衔接：Ticker.Dispatch 在 `FrameDiagnostics.Enabled` 时对每个回调记 Stopwatch（Ticker.cs:129-145）——**零开销默认关闭**
- 标题明示 **CPU only，GPU 需游戏内 profiler**——自计边界诚实

---

## ④ 攻击追踪（AttackFeed 370 行 + AttackInfo 19 行，源 32163-32529）

- `AttackInfo{Card, Attacker, PlannedHits, Random, Landed, BeganAtMs, Invocation}`（AttackInfo.cs:4-18）
- **AsyncLocal 上下文链**（AttackFeed.cs:49-51）：`AsyncLocal<Flight> Executing` + `AsyncLocal<DamageScope> Damaging`——攻击 Execute 是 async 方法，伤害在 await 之后才发生，靠 AsyncLocal 把「谁在打」传到伤害回调
- **栈式 Scope**：Enter/Leave 配对保存 Previous（AttackFeed.cs:101-129、162-179）；EnterDamage 判 `Execution/Owner` 双重归属（AttackFeed.cs:181-204）
- **主线程守卫**：`OnMain = CurrentManagedThreadId==_mainThread`（AttackFeed.cs:57），所有入口先过
- **生命周期**：Begin 时 `Active.Add` + `Ticker.Add(Tick)`；Tick 双终结条件——`Task.IsCompleted`（正常/取消/故障三种 reason）或 **8s stale timeout**（`StaleMs=8000`，AttackFeed.cs:45、284-287）；Finish 时 `Active` 空则摘 Ticker
- **遥测即文档**（AttackFeed.cs:329）：`"AttackFeed: {CardType} planned={n} landed={m} durationMs={} gapsMs=[{d1,d2,...}] end={reason}"`——计划命中/实际命中/总时长/每击间隔全部落日志，调优多段攻击节奏靠读它
- 事件 Began/Aimed/Ended（AttackFeed.cs:59-63）；Reset 在战斗/房间结束全 Finish（AttackFeed.cs:252-262）

---

## ⑤ 笔刷与构件库

### 5.1 Brush（SilentFX.Core.Brush.cs，594 行，源 32530-33120）——刀痕笔刷

- 8 色常量调色板 Wind/Steel/Gold/Amber/Blood/Violet/Earth（Brush.cs:17-29）+ `Pale(c,α)` 向白插值 0.78
- **Spindle 双Sprite 结构**（外发光+核，Brush.cs:40-80）+ `Shape(a,b,thick,α)` 拉成线段：`Scale=(len·1.3, max(1,thick))/64`——**一切刀痕的基础**
- `Cut`：三段式划切（inTime 0.05 划出 + hold 0.04 + fade 0.22），刀头 0.15→0.85 缓动、刀尾 0.55·k² 跟进，粗细随 fade 衰减（Brush.cs:133-184）
- `Dart`：飞行刀光，头速 `0.55u+0.45u²`，尾长 clamp(8, length)（Brush.cs:186-249）
- `Follow(parent, Func<float,Vector2> path, ...)`：**路径函数驱动拖尾**（Brush.cs:251-287）
- `Gust`：count 条 Dart 横向错峰，Hash 随机化（Brush.cs:289-339）
- `Arc`：14 段贝塞尔，每段双 Sprite2D，`fat=sin(πu)^0.8` 中段粗两头细；出场逐段点亮、退场错峰（Brush.cs:341-455）
- `Orb`：贝塞尔弹道 + 速度拉伸 + 抵达 Flash（Brush.cs:457-537）
- `Wave/Dust/Sparks/Motes` → 封装 RingPulse/Burst（Brush.cs:539-575）

### 5.2 Props（SilentFX.Core.Props.cs，291 行，源 41086-41373）——多边形构件

`Circle(r,points=20,sx,sy)`、`Teardrop(r,tip=2.3)`、`Box`、`Line(width,color,additive,params points)`、`Quill(l,...)`（羽毛笔：15 点轮廓 + 5 道羽纹 + 笔尖，全按 l 归一化）、`Vial(r,glass,liquid,rim)`（烧瓶：瓶身/液体/高光/瓶圈）、`Paw(color,r)`（爪印：主掌 + 4 趾 additive）。**全部代码画多边形，零贴图。**

### 5.3 Shade（SilentFX.Core.Shade.cs，166 行，源 44572-44734）——墨影体系

Ink(0.035,0.012,0.075)/Rim(0.64,0.38,1)/Glow(0.42,0.22,0.8) 三色 + Inked/Rimmed/Glowing(α) 工厂；Aura/Blob 两个 Sprite 工厂；Motes/Sparks → Burst 封装；`Puff` → ShadeMist 着色器雾团（强度三段：u<0.15 起、之后线性衰）；`Partial(path, fraction)` 路径按比例重采样截取（Shade.cs:124-155，拖尾缩尾用）。

### 5.4 卡牌挂件三件套

| 类 | 职责 | 要点 |
|---|---|---|
| **CardStandIn**（260 行，源 38333-38589） | 替身 NCard 管理 | Make：`NCard.Create(card, ModelVisibility)` + UpdateVisuals + **输入旁路**（MouseFilter=2/BehaviorRecursive=1，Free 时还原）；Hide/Show/Dim=Modulate alpha；Pose/Place 全局变换↔父空间 `AffineInverse` 换算并**除以 Body.Scale 抵消内部缩放**（CardStandIn.cs:132-227） |
| **CardRide**（367 行，源 33570-33933） | 卡片飞行动画 | 见 2.1；`ToCard`（真身↔替身）/ `Fly`（Func<Pose> 动态目标）双入口；Allowed 校验 pile∈{2 手牌,5 场上}；`Duration+1s` 硬超时；Stop 必 `Show(real)` 防真身永久隐身（CardRide.cs:303-366） |
| **CardDeco**（452 行，源 33121-33569） | 卡面挂件 | `Attach(card,key,build,tick,left)` 同 (card,key) 幂等；Follow：Root 挂卡节点随动（Position=0、Scale=Body.Scale）；**卡离手 → Detach 到 UiLayer 保留全局变换 + Left 回调 + 0.15s 淡出**（RemoveHandle/Update，CardDeco.cs:293-451） |
| **CardFlight**（113 行，源 38590-38699） | 弃牌飞行认领 | `Claims: Dictionary<CardModel,(Plan,Until)>`；Claim/Claimed/Holds/TryTake；FxClock 毫秒判过期；Patch 侧 TryTake 消费——**认领制防两个系统抢同一张卡的飞行动画** |

---

## ⑥ 设计模式编目（本代码库重复出现的 20 个模式）

1. **静态门面**：Core 45 类型全 static，无继承无实例——门面即框架（FxKit/HandFlow/HandHold/Ticker…）
2. **声明式 Spec + 回调槽**：`CardRide.Spec{Duration/From/Shape/Frame/Arrived}`、`CardFlight.Plan`、`AssetWarmup.Work{Step/Cancel/CanRun}`、`CardDeco.Attach(build/tick/left)`——行为=数据+3 个函数槽
3. **纯函数整形器**：`Shaper(ref Pose,t,from,to)`——位姿表达式零副作用
4. **事件总线 + 逐个订阅者容错**：HandFlow.Raise 三泛型重载
5. **双 pass 帧泵**：ProcessFrame 主 + FramePreDraw 渲染前
6. **租约制自动回收**：FxLifetime、CardFlight.Claims、HandHold 倒计时——凡临时节点必租约
7. **快照迭代**：ArrayPool.Rent + members.Contains 二次确认（Ticker/CardRide/CardDeco/HandMoves 五处同款）
8. **自愈订阅**：回调炸 → Error + 自动 Remove
9. **分层 try-catch + 定向恢复**：Update 五路各自 try，catch 里精确恢复（EndAllActs/SettleAll/FreeTies…）；Reset 全分段
10. **参数化调参**：`Tuning.F(key,def)`（462 键 tuning.ini）+ `Pace()` 全局速度倍率；时长统一 `Seconds()` 出口
11. **防御式 Alive**：IsInstanceValid + IsQueuedForDeletion 双检，400+ 处
12. **程序化资源**：GradientTexture2D/CanvasItemMaterial 运行时生成，零外部依赖
13. **可访问性单点收口**：ReduceScreenEffects → AccessibleColor + 3 处局部减量
14. **替身演出**：真身 Hide + 替身动画 + 结束 Show（CardStandIn + Act/ToCard）
15. **增量式位姿叠加**：`Position += (new − old)` 记录 delta，结束时减回（Strain/Settle/StepStays 同款）——**绝不写绝对位**
16. **弹簧积分子步**：dt 钳 0.1 + ≤0.016s 子步，帧率无关弹簧（Spring/StepStays 同款）
17. **AsyncLocal 上下文链**：AttackFeed 跨 await 归属
18. **反射懒加载 + 一次性失败标记**：`_selectedLooked/_hoveredLooked`
19. **惰性订阅/退订**：FxLifetime/AssetWarmup/AttackFeed 空转即摘 Ticker
20. **命名即调试**：临时节点统一 `SilentFX_*` 前缀（Delay/Beam/Brush/Afterimage/CardDeco/StayLight），场景树一眼可辨

---

## ⑦ 「注释等效物」编目（字符串/日志里的设计意图）

源码几乎无注释，**DevLog 文案与命名常量就是设计文档**：

| 证据 | 读出的设计意图 |
|---|---|
| `"Ticker: main loop is not a SceneTree"` Ticker.cs:34 | 帧泵前提假设显式化 |
| `"Ticker callback failed and was removed: "` Ticker.cs:136 | 自愈设计的公开声明 |
| `"FxKit warm-up: "` FxKit.cs:138 | 预热允许失败 |
| `"Effect tween failed: "` FxKit.cs:685 / `"Immediate effect failed"` / `"Delayed effect failed"` FxKit.cs:817/837 | Drive/After 的一次性熔断语义 |
| `"Prewarm slow unit: {name} {ms} ms (outside combat)"` AssetWarmup.cs:139 | 8ms 预算 + 只在战斗外预热 |
| `"Prewarm queue drained: … GPU timing requires in-game profiling"` AssetWarmup.cs:160 | 自计边界：CPU only |
| `"Perf (CPU only): … >33ms= … >100ms= …"` FrameDiagnostics.cs:76 | 掉帧分档（33ms/100ms）+ GC/分配监控 |
| `"AttackFeed: … planned= landed= durationMs= gapsMs=[…] end=reason"` AttackFeed.cs:329 | 多段攻击节奏遥测即调优文档 |
| `"Expertise: N card(s) stay - they duck under the discard / nothing is thrown away, so only the light"` HandMoves.cs:1981 | 留牌演出二态语义（有弃牌→躲避，无弃牌→只发光） |
| `"HandPace: preference X; effective Y"` HandPace.cs:86 | 配置双态（偏好 vs 生效）可观测 |
| `"HandPace: RitsuLib settings are available at Settings > Mod设置（RitsuLib）> SilentFX"` / `"RitsuLib is not loaded; settings stay available in user_settings.ini"` HandPace.cs:89/137 | 优雅降级 + 用户指引 |
| `"HandCards: no selected hand container"` / `"no button hover field"` HandCards.cs:101/250 | 反射失效（游戏更新）的显式警告 |
| `"Bullet Time visual ownership: "` TimeFreeze.cs:235 | 冻结节点的归属检查 |
| `"FmodNative: the import table entry could not be put back (it keeps calling through us, which is harmless)"` | 已知无害缺陷的坦率记录（工程诚实） |
| `"HandMoves ready"` HandMoves.cs:283 | 初始化哨兵 |
| 常量 `Land=0.85 / CircleIn=0.275 / CircleOut=0.725 / Depth=0.14` HandMoves.cs:185-191 | 动画时间线关键帧命名化 |
| `Until = FxClock.Milliseconds + 600` HandFlow.cs:225 | 600ms 残留窗：异步入场归属的时序解法 |
| `StaleMs = 8000` AttackFeed.cs:45 | 攻击追踪 8s 僵尸兜底 |
| 设置页文案「减少闪光和强屏幕效果 / Reduce flashes and strong screen effects，默认关闭」HandPaceSettings.cs:52 | 可访问性选项的用户语言定义 |
| `"slowest unit {ms} ms"` + `"Prewarm queue drained"` | 预热性能基线的持续记录 |

---

## ⑧ 【可移植清单】按优先级排（UE4 资产 → Godot 特效移植 · DyeFramework）

**P0 · 框架地基（无依赖，直接搬）**
1. **Ticker 双 pass 帧泵**：ProcessFrame + FramePreDraw；dt 三保险（墙钟 / TimeScale 还原 / 0.25 封顶）；ArrayPool 快照 + 倒序 + 二次确认；回调炸自动 Remove。（Ticker.cs 全文 154 行）
2. **FxLifetime 租约回收**：Room 快照跨场景失效 + TimeFreeze 豁免 + 空转自动摘除 + Cleanup 钩子。（FxLifetime.cs 全文 142 行）
3. **FxKit 程序化标准库**：SoftDot/Bubble/Additive 三资源 + Glow/Quad/Ramp/Shape/Star/Bezier/Damp/Hash/Alive + Ribbon/Crescent/Beam/Flash/Burst/Bubbles/Drive/After。**UE 侧对应：把 UE4 材质/贴图资产翻译成这些程序化原语的组合配方。**

**P1 · 编排层（移植项目的核心借鉴）**
4. **CardRide.Spec 声明式时间线**：Pose 四通道 + Shaper 纯函数 + Frame/Arrived 回调 + Duration 钳制 + 实时目标读取。**手牌/挂件/ Actor 动画编排的统一抽象。**
5. **替身演出模式**：真身 Hide + 替身（输入旁路）+ 结束 Show；CardStandIn.Pose/Place 的全局↔局部变换换算与内部 Scale 抵消。
6. **HandFlow 事件总线**：嵌套会话栈 + 600ms 残留窗 + 泛型逐个调用容错 + MP 过滤。
7. **Drive/After 两件套**：万能逐帧驱动（熔断）+ 延时节拍——所有时间行为的原子入口。

**P2 · 生产化设施**
8. **AssetWarmup 分帧预热**：非战斗窗口 + 0.5s 安静期 + 每帧一 task + 分步 Step + 8ms 预算披露 + 排空统计。（UE4 资产转 Godot 后预热成本高，这套直接对应）
9. **FrameDiagnostics 自计**：5s 窗口 wall peak / >33ms / >100ms / 分配 / GC / 回调 Top5，Tuning 开关默认零开销。
10. **AttackFeed AsyncLocal 上下文链** + 8s stale 兜底 + 命中节奏遥测（planned/landed/gapsMs）。
11. **ReduceScreenEffects 单点收口**：AccessibleColor + 3 处局部减量。

**P3 · 资产库与细节**
12. **Brush 刀痕库**：Spindle 双Sprite + Cut/Dart/Arc/Orb/Gust/Follow（路径函数驱动）。
13. **Props/Shade 构件库**：Circle/Teardrop/Quill/Vial/Paw + Ink/Rim/Glow 三色体系 + Puff 雾团。
14. **CardDeco 挂件体系**：Attach 幂等 + Follow 随动 + 离手 Detach 保变换 + Left 回调 + 淡出。
15. **Tuning.ini + Pace 全局倍率**：所有时长/幅度走调参表， Seconds() 统一出口；快/慢/关三档节奏。
16. **增量式位姿叠加 + 弹簧子步积分**：Position += delta 记录法；dt 钳 + 0.016 子步。
17. **命名规范**：SilentFX_* 节点前缀 + DevLog 双通道（GD.Print + 文件 500ms flush + ProcessExit 兜底）。

**移植映射提示（UE4→Godot）**：UE4 的 Niagara/材质参数动画 → FxKit 的 `Burst(...)` 参数族与 `Ramp`；UE4 Timeline/Curve → `Drive` + `Shaper`；UE4 Actor 生命周期 → `FxLifetime` 租约；UE4 的 Tick 组 → Ticker 双 pass；UMG 挂件 → CardDeco。

---

## 附：未展开/待确认

- HandMoves 中 `Intro.BackOut`（UnderShape 调用，HandMoves.cs:1273）实现未读，待确认。
- `TimeFreeze.HoldsVisual` 的 `Known` 表登记逻辑仅读判定分支，注册侧未读，待确认。
- HandPace 与 HandPaceSettings 的 RitsuLib schema 消费端（RitsuLib 为第三方 mod）不在本库，集成方式待确认。
- ShaderLib.NewMaterial("sigil") 的 sigil shader 参数全集（radius/width/dashes/glow/alpha/spin）由 02_shaders.md 覆盖，本报告不重复。
