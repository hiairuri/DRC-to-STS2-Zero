# SilentFX v0.72.7 二探·卡牌特效技法目录扫掠报告（11_cards_sweep）

> 扫掠代理：deep-cards-sweep｜日期：2026-10-16
> 材料：反编译切分源码 `silentfx_decomp/split/`（只读）、`3817850774/tuning.ini`、初探三报告（01/02/03）
> 结论先行：**这 25+ 个中大型特效共享同一套"时间线驱动 + 阻尼趋近 + Line2D 三件套 + 让位协议"的工程底盘，差异全在编排创意**。最值钱的不是单点技法，而是四类"与游戏协商"的机制：① 改写游戏自身动画数据（飞牌曲线终点、目标箭头、骨骼动画时标）；② 冻结/推拽/提亮真实敌人节点；③ 与真实结算对齐的欠账-补刀机制；④ UI 元素逐帧交互（扫描线与血条/盾/能力图标）。以下按 A~E 五族给出技法目录。

---

## ① 扫掠方法说明

- **范围**：任务清单 A~E 五族共 51 个文件，实际通读 45 个（含 Powers 侧 NoxiousFumesFx/PoisonFx/EnvenomFx/WraithFormFx、Patches 侧 ExposeHooks），其余以结构 grep 补齐。累计精读约 3.1 万行。
- **每个特效抓取六要素**：类结构与入口挂接（OnPlay/OnHit/Held 会话）、核心动画技法（精确到函数名与行号）、1~3 个关键数值（代码常量优先，tuning.ini 佐证）、创意点、注释等效物（字符串字面量与日志文案）、`split文件名:行号` 证据。
- **数值口径**：代码内 `Tuning.F("key", def)` 的 def 是发布兜底，tuning.ini 是精选入口（462 个 code-only 隐藏键的分布在本报告⑤补充）。凡两者不一致处同时标注。
- **可信度**：结构/技法/数值均为静态代码直读；GDScript 舞台（suppress_stage.gd / wraith_form.gd / wraith_play.gd）的运行时行为以 02_shaders.md 逐行结论为参照，C# 侧只写桥接协议。

---

## ② A 族·刀剑系技法目录

### A1. ShivFx.cs（2105 行，static 总控）
- **结构**：`Styles` 字典把 8 张造刀卡映射到 ShivStyle（ShivFx.cs:58-69）；`Stack` 帧栈记录 CardPlay 供 OnShivsCreated 查询（:27,:143）；入口 `OnShivsCreated` 分单机/MP 两路（:272-351）。
- **技法**：`PlayCreate` 按风格分发 8 种出手位形（:1114-1321）：Dance 绕身椭圆轨道（`reach=body.X*0.48+30`、1.2 圈、RingPulse 0.4→1.3 压扁 0.3 虚线 9 段，:1193-1216）；Cloak 斗篷 Crescent 布+三把刀三次贝塞尔（`CloakKnife`，:1513-1639）；Sleeves 双臂交替外抛（奇偶分侧、stagger 0.04s，:1220-1238）；Fan 手中扇张（-12°+Spread*15°，:1239-1259）；Symphony 肩后音符位（±46px，:1260-1278）；Return 从目标侧旋回（`shiv.return_time=0.27`，flowing 缓动，:1279-1295）；Ink 墨爆（InkSplat 16 粒+Glint，:1296-1311）。
- **数值**：`shiv.length=56`、`fly_time=0.36`、`dance_time=0.45`、`return_time=0.27`、`phantom_length=340`、`phantom_thrust=0.11`/`hold=0.16`（tuning.ini:306-317）。
- **创意点**：`Launch` 用 `Func<float,Vector2> pre` 把"出手前运程"与"贝塞尔飞行"缝进同一条 Tween（:909-1039）；Return 的 flowing 模式把 t² 改成 `t*(0.45+0.55t)` 前急后缓；`AdjustVanillaThrow` 直接劫持游戏原生飞刀（改 thrower 原点与 tint，:474-565）。
- **注释等效物**：日志 `"Shiv FX: {n} into the hand ({style}, from #{i}{, n from standby})"`（:343）。

### A2. ShivHeld.cs（1076 行）
- **结构**：HeldSession+IShivSource；`Build` 按 style 造刀与环（:130-225）。
- **技法**：`Tick` 里 8 种 style 的持握位形算法（:468-593）：Dance/Storm 椭圆轨道（后半透明度 0.5/0.55 造纵深）、Sleeves 双臂待抽、Fan 扇张、Return 蓄力后拉 16*arm、Ink 墨滴环；`Symphony` 的 `HopHeight` 是完整"下蹲→腾起→落地压扁"三段（windup squash→air `lift=4u(1-u)`→settle squash，:841-874）；`InkDrip` 粒子 7 粒/0.55s、重力 620（:250-306）。
- **数值**：`symphony.windup=0.12`/`hop_time=0.56`/`settle=0.26`/`pause=0.05`、`hop_height=46`（代码默认，ini 未列）。
- **创意点**：`TryTake` 把待命 blade 节点过继给打出动画（:892-936）——待命与打出共享同一节点、零重建；`ReturnBorrowed` 从幻影刀池借还（:1047-1068）。
- **注释等效物**：`"the card is not flown"` 风格的降级日志。

### A3. SkewerFx.cs（2054 行）
- **结构**：`Shaft : EnergyBeads.Bead`；四状态机 Ready/Flying/Stuck/Gone（:190-196）；`Live` 静态表+`_ticking` 自挂 Ticker。
- **技法**：飞行 t² 加速+长度脉动 `1+0.45sin(πt)`（:1625-1639）；扎入后阻尼振荡 `StuckRot+quiver*exp(-7t)*sin(44t)`（:1644）；伤口墨点缩放 `24+34exp(-9t)+14*pulse`（:1667）；目标身标记 sigil 过冲 `2.3→1`+自旋 `1.5*(1-show)`（:1684-1702）；`Refill` 补箭从能量球位贝塞尔甩出（中点上抬 150px，:984-1017）；`Freeze` 用 CreatureHold 冻结目标 `beat+hold_pad` 秒（:1143-1167）。
- **数值**：`skewer.max=12`、`beat=0.3`、`flight=0.09`、`first_hit=0.16`、`timeout=1.6`、`linger=0.3`、`arrow_length=120`、`bead=24`（tuning.ini 部分+代码默认）。
- **创意点**：黑珠（Chem）与绿珠（Energy）分轨——X 费里被修改的点数变成黑色箭，落点用 InkSigil（运行时把 sigil shader `blend_add` 换 `blend_mix` 派生）。
- **注释等效物**：`"Skewer: {n} arrow(s) for X = {x} ({k} of it black)"`（:296）。

### A4. PreciseCutFx.cs（1669 行）
- **结构**：双模态——有卡飞行（`Take`→`Flight` 四阶段）与纯划线（`Show`→`Build`）；`Live`/`Flights` 双表。
- **技法**：Flight 阶段机（:1049-1124）：0 预备后仰 26px+turn 0.3 →1 接近+沿切线瞄准 →2 卡沿线切割 `CardAlong`（-0.24→1.32，:720-723）→3 三段贝塞尔回家（Home 时长按距离 clamp 0.32~0.52）；`cut_shift` 读屏切线错位（quad+BackBufferCopy，`Aim` 每帧传 view/point/dir/half_len/reach/shift，:658-697）；`Hair` 锥形宽度曲线 Line2D（:546-571）；`Needles` 白针 `3+4*drawn` 根（:505-525）；时间预算 `InTime 0.075+Hold 0.03+Fade 0.05`，`ShiftAt=0.155+beat`（:170-184）。
- **数值**：`precise.shift=Lerp(4,11,drawn)*scale`、`bloom=Lerp(10,20,drawn)`、`cut_length` clamp 0.3~2。
- **创意点**：卡牌飞行被 Claim 后游戏只认 Modulate=0 的原卡，Mod 用复刻 rig 演完全程再 `CardFlight.Release` 交还（:885-911,:1379-1398）——与游戏发牌系统"借还"而非抢占。
- **注释等效物**：`"Precise Cut: the card is not flown, the cut is drawn alone"`（:228）。

### A5. PreciseCutChains.cs（616 行）
- **结构**：CardDeco 挂载（"precise" key）；`State`+`Marks` 字典。
- **技法**：`card_edge` shader quad（420×542，`Build`，:309-357）；磨刃量 `Out` 用二阶弹簧积分 `a=576*(Want-Out)-24*Speed`（ω=24 ζ=0.5，子步 8ms 防爆，:490-502）；`Share` 读 `DynamicVars.CalculatedDamage/CalculationBase` 得到"这张牌被强化了多少"→刃磨多亮（读失败回退数手牌 `Others/9`，:269-307）；`Left` 给离手牌记 3 秒 `Marks`——卡飞出手后刃仍完整（:600-618）；三档闪光时序 glint 0.26/gleam 0.62/star 0.36（`Pose`，:543-598）。
- **数值**：`precise.edge_from=44`、`edge_side=230`、`edge_depth=13`、`gleam_every=2.2`（tuning.ini:635-644 标注 defaults live in code）。
- **创意点**：**强化倍率可视化**——别人把强化画在牌面上，它把强化折算成刀刃打磨进度。
- **注释等效物**：`"PreciseCutChains: switched off for this fight after repeated failures"`（:192）——4 次失败自动关机保平安。

### A6. EchoingSlashFx.cs（1540 行）
- **结构**：`Cast`/`Arc`/`Foe`；五阶段 `Slash→Whirl→Fling→Unravel`（:6-13）。
- **技法**：**整段 GLSL 以 C# 字符串常量内嵌**（HeadWarp/HeadPlain/Middle/Tail 四段拼接，:211-220）——6 条刀气是"水平面里的环片"：近半圆一个矩形（front 层）、远半圆另一个（back 层），`tilt=0.2` 椭圆（shader 注释原文 "a piece of a ring that lies in a level plane and is seen from the side"，:215）；`PowerFor` 对数定半径增速 `clamp(log(x)/log(0.75),1.3,4)`（:923-930）；杀敌后 `Catch` 在尸体处掀 Whirl（`Hold=width*0.45` clamp 55~130）再 `Fling` 找下一组；`Move` 各阶段角度/扩散/侵蚀曲线（:1283-1413）；`Land` 后速度指数衰减到 900px/s（:1421-1424）。
- **数值**：`echo.tilt=0.2`、`size=470`、`first=0.13`、`catch_up=0.06`、`spin=15`、`fade=0.26`、`strength=0.95`、`edge=3.4`、`feather=30`、`thin=0.2`、`bend=12`（tuning.ini:1193-1207）。
- **创意点**：**与真实结算对齐的欠账-补刀**——`Hit` 时若刀气半径还没刮到敌人就记 `Owed` 欠账，等半径推进到了再补一刀（:993-1002,:1404-1412）；`GuessGap` 读游戏 FastMode 设置推断出刀间隔（瞬间=0.06s，:758-793）。
- **注释等效物**：日志 `"blow {n}, {t} s after the last - the wind at {r} of {far}"`（:965）；shader 头 `"the kind that also bends what is behind it"`。

### A7. FinisherFx.cs（1360 行）
- **结构**：`Tally.Begin/Hit` 数本回合攻击牌序；`Expected=TurnLog.Count(Attack)`；`Darts`/`Sinks` 静态表。
- **技法**：`Dart` 把本回合打过的攻击牌复刻成卡飞镖甩向敌人（fly 0.13s t² + sink 0.16s 溃散，末击 `last_scale=1.45`，:368-564）；`Finale` 末击三连：黑 veil + X 交叉双斩（Heavy 88/40px 双层 Brush.Cut）+ `Slam` 砸地（`Sunk` 下沉 0.035s 起+`Jolt` 0.36/0.62 两次余震，:830-851；`Bulged` 地面隆起，:841-851）；`Seize` 抓住 `NCreature.Body` 往下压 `sink_px=20` 再交还（:1181-1350）；三环 RingPulse 0.18/0.36/0.62 延迟尘（:1131-1145）。
- **数值**：`finisher.card=0.34`、`last_scale=1.45`、`dart_stretch=0.25`、`sink_px=20`、`payoff=1.0*(1+0.05n)`、`shake` n≥4 时 3 级。
- **创意点**：**把敌人按进地里**——直接写敌人身体节点的 Position（还原时逐帧核对 Home/Shift 防与游戏动画打架，:1287-1313）；`Replaces`/`CoversStab` 让原生攻击动画让位（:103-129）。
- **注释等效物**：`"Finisher: {n} Attack(s) remembered this turn, {m} card(s) come back"`（:568）。

### A8. FinisherHeld.cs（2452 行）
- **结构**：`Blade` 实例=一张复刻卡 rig；`Marks(out total)` 从 TurnLog 读本回合攻击牌并按 pile 归类（:395-465）。
- **技法**：`SlotOf` 极坐标环排（非末张 ±150°/n clamp 36° 交错，末张放大+外移，:1131-1167）；`MakeDue` 每帧最多实体化 `_perFrame=2` 张防卡顿（:670-707）；`Wobble` 双频颤抖 `0.65sin(x)+0.35sin(2.31x+1.3)`（:346-349）；`Home` 从抽/弃/消耗堆位弧形浮起（pileRise 112，:1189-1251）；glint 掠过周期 2.6/1.5Hz（`Put`，:735-743）。
- **数值**：`finisher.max=9`、`fly_out=0.24`、`stagger=0.03`、`rise=0.2`、`tremble=2.2`、`sure=5`、`lock=0.26`。
- **创意点**：待命时就把三个牌堆的牌"预排"在目标周围——收人头是视觉预言的兑现。
- **注释等效物**：`"Finisher: {n} Attack(s) remembered this turn"`。

### A9~A15. 小件七连（各 30~210 行，OnPlay/OnHit 单发式）
- **BackstabFx**（56）：OnPlay 目标身后 `Shade.Puff`（BackstabHeld.Behind 定位），OnHit 横切 26px+闪光（BackstabFx.cs:13,:49）。
- **SuckerPunchFx**（31）：勾拳——Flash 90px+Dust 12+紫色上喷 8 粒（SuckerPunchFx.cs:24-26）。
- **FlickFlackFx**（45）：双闪 X 交叉 Cut（第二道延迟 0.06s，Strokes.Across/Vertical 算半长，:33-34）。
- **LegSweepFx**（46）：Brush.Arc 贴地弧+0.05s 后 Dust+紫爆（:31-40）。
- **DashFx**（71）：Brush.Dart 青色飞矢 `max(260,width*0.65)`+脚下 Dust（:30-31）。
- **DeflectFx**（207）：弹反两段式子弹时间线——来袭 Streak（`u*(0.5+0.5u)` 加速）→命中 Flash/Sparks→反射 spent Streak（`1-(1-u)²` 减速+宽衰减 `1-0.5u`，:119-203）；`deflect.gap=0.08` 双弹间隔、`deflect.reach=2.6*body.X`。
- **共性**：小件全部只调 Brush/FxKit 原语，一行 `Strokes.Scale(c)` 统一缩放，是 A 族大件的"减配百科"。

---

## ③ B 族·毒系技法目录

### B1. SnakebiteFx.cs（938 行）
- **结构**：`Strand` 三层 Line2D（外光晕 2.6x/本体/白芯 0.3x）+`Taper` 三态宽度曲线（Body 头粗尾细/Jaw 张开/Loop 环，:19-150）；`CoversBite` 让位标志（:191-233）。
- **技法**：四段时间线 flight 0.28→wrap 0.24→pull 0.32→leave 0.40；蛇身沿贝塞尔爬行+正弦摇摆 `sway*sin(18s-t*30)*包络`（:573-587）；到位后上下颌张开 `0.8*sin(πu/2)`（:629-645）；`Bite` 咬闭（snake_bite 音效+双毒牙 Cut）；`ShapeLoop` 勒紧成 60 点椭圆环+drag 后拽+95Hz 高频颤动 3.5px（:797-823）；`Burst` Snap 炸散（venom 30 粒+双环+Shake，:876-940）。
- **数值**：`snake.beam=22`、`length=0.58`、`drag=190`、`flee=760`、`hug=14`、`sway=24`、`venom=30`（tuning.ini:412-427）。
- **创意点**：蛇不是"射中"而是"缠"——勒紧时椭圆环的后拽量 `drag*Pull(p)`（p³ 曲线）让绳圈一点点咬进身体；头尾速度差（tail 0.27s 延迟）做出软体追随之感。
- **注释等效物**：`"Snakebite cover"`/`"keep_bite"` 让位语义。

### B2. DeadlyPoisonFx.cs（145）
- `Throw` 毒液弹贝塞尔抛射（中点上抬 `min(120,len*0.16)`），途中 4 次滴落（`t>0.2*(n+1)` 触发 3 粒小爆），落地 splash 18+RingPulse（:82-138）。数值：`deadly.time=0.2`、`splash=18`（tuning.ini:400-401）。

### B3. PoisonedStabFx.cs（106）
- Crescent 双弯刀痕（毒色 15px+白芯 4px）+`stab.spray=18` 定向喷+Flash 2.6x（:88-102）。数值：`stab.length=150`、`stab.spike_length=124`/`spike_halfwidth=15`（tuning.ini:402-406）。

### B4. NoxiousCloudFx.cs（213）
- `RegionOf` 借用 NoxiousFumesFx 计算左右两侧敌带的 near/far/floor（:23）；`Sheet` 生成 toxic_fog 双 quad（back 0.95/front 0.62 浓度差）；`Drive` 四曲线：head `-80+(len+320)*easeOut2.4` 推进、tail 延迟跟随、cone 收口、scroll 双重漂移（:129-141）；前端 Glow 光点沿程插值（:146-149）。数值：`fumes.sheet_rise=0.9`、`sheet_alpha=0.64`、`sheet_veil=0.3`、`sheet_light=0.2`。

### B5. NoxiousFumesFx.cs（698 行，Power）
- `Bank[2]` 左右两汀；`LevelOf(amount)` 层数→浓度（`fog_levels=6`）；`Draw` 把 intensity/thick/rise/floor_y/pulse/pulse_at 逐帧喂 shader（:444-503）；`Surge` 毒触发脉冲浪涌：pulse_at 从 -200 扫到 far+400、强度 `sqrt(sin(πu))`（:467-478）；`Erupt` 每敌人脚下爆气（`noxious.burst_gas=14` 粒子+`burst_bubbles=18` 气泡+双环，:648-700）。
- **数值**：`fumes.fog_rise=250`、`fog_min=0.18`、`fog_max=0.7`、`veil 0.04~0.26`、`fog_soft=260`、`fog_motes=26`（tuning.ini:9-14,629-632）。
- **创意点**：常驻雾不是贴画——两侧雾汀的 near/far 随敌人站位实时重算，敌人换位雾跟着换形。

### B6. PoisonFx.cs（314 行，Power）
- 20 气泡循环+12 雾粒子；`full_at=64` 层以上起雾（`_mistAlpha` 阻尼 2.5，:163-166）；`OnPoisonTriggered` 统计全场 Accelerant 层数决定连咬几次（`_pendingPulses=1+num`，:28-65）；`OnTrigger` 每咬间隔 `pulse_gap=0.2`，`Bite`=RingPulse+Bubbles 14+Burst 9（:231-264）。
- **数值**：`poison.bubbles=20`、`full_at=64`、`mist=12`、`pulse_gap=0.2`、`bite_bubbles=14`（tuning.ini:349-357）。
- **创意点**：**毒与助燃剂的联动可视化**—— Accelerant 存在时每跳毒咬都加倍演出。

### B7. EnvenomFx.cs（457 行，Power）
- `Fling`：朝 Silent 反方向甩毒沫（Brush.Cut 9px+`fling_mist=16` 粒子+`fling_drops=10` 液滴；每液滴=暗核+毒晕+高光三 Sprite 组合，解析弹道 `v*e^(-drag*t)+½g t²`+沿速度方向拉伸 `1+|v|/300` clamp 4.5，:236-330）；`Pop` 闪光+环+9 粒；`Drips` 3 滴延时下落（`drop_first=0.3`/`drop_gap=0.36`）；`Way` 每次触发 hash 抖动 ±5°（:117-153）。
- **数值**：`envenom.fling_reach`、`fling_drag=3.2`、`fling_gravity=620`、`fling_fan=11`、`fling_life=1.0`、`fling_rest=0.45`+`fling_repeat=0.6`。
- **创意点**：**连击减效**——0.45s 内重复触发只播 60% 强度，多段攻击不刷屏；液尾拉伸按实时速度算，慢的液滴圆、快的液滴长。

---

## ④ C 族·幽魂/暗影系技法目录

### C1. WraithPlayFx.cs（1236 行，新代出招）
- **结构**：GDScript 舞台 wraith_play.gd 驱动；`Run` 持 Play 节点；`Prepare` 在悬停时预建舞台（:169-221），OnPlay 只负责 `Go`。
- **技法**：`CardFlies` **劫持游戏飞牌**——反射拿私有 `_swooshPath`（AccessTools.FieldRefAccess），把 Curve2D 末点改成她的胸口（:614-706）；`StartFlight` 复制卡面火焰 AnimatedSprite2D×5 沿 Trail[10] 历史轨迹排布（缩放 1.25→0.55、旋转对齐速度，:708-770）；时间线 TBurst 0.6→TGo 0.67→THit 2.12→TEnd 3.72 由 gd `times()` 回传（:439-447）；音效按 `Sfx.Lead` 提前量对齐（:1094-1112）；`Knock/Scream/Land` 三段震+pour 绿光倾泻（:1179-1238）。
- **数值**：`wraith.play.shake=0.6`、`dark=1`、`fire_pad=0.02`、`puff=1`、`wave_push=0.011`、`knock=1`、`intro.shake=3`。
- **创意点**：卡不是"飞向玩家"而是"飞进她胸口"——游戏自己的飞牌曲线被改了终点，玩家看到的是游戏资产在替 mod 叙事；GDScript 每帧耗时用 Stopwatch 自计并在 Free 时汇报（:1077-1089,:1145-1149）。
- **注释等效物**：`"the card flies from {p} to her chest {c} (the game's own end was {e}) for {t} s"`（:704）。

### C2. WraithFigure.cs（1222 行，卡画女子）
- **结构**：`WraithFigureData.Data` 扁平数组（kind,sway,npts,(x,y,w)*n）描述 10 类部件，按 `6,5,2,4,8,1,3,7,0,77` 序绘制（:470-504）；Strip/Dark/Flame 三容器。
- **技法**：`AddStrip` 入场弧=40 点黄金角螺旋从屏幕外旋进+4 次拉普拉斯平滑（:577-596）；`PoseStrip`  hover 蠕动→land 甩入→settle 展开三阶段+三频正弦（W1/W2 双频+sway*0.034*R 沿程衰减，:1041-1080）；`Partial` 逐点描绘（:1203-1224）；`Blast` 部件被从头部吹散（:1082-1090）；`EdgeReach` 射线求屏幕边界交点定入场弧长（:801-838）。
- **数值**：`FormTime=0.85`、`AwayTime=0.5`、`wraith.figure.size` clamp 0.4~1.6。
- **创意点**：**数据驱动人形**——人不是贴图是一份坐标档案，每个部件有自己的 delay/travel/hover/w1/w2 随机相位，"形成"是 25+ 部件各自旋入的总和。
- **注释等效物**：03_notes 记录其由 `docs/ref/wraith_figure_gen.py` 生成。

### C3. WraithFormIntroFx.cs（1213 行，旧代纯 C# 开场）
- **技法**：常量时间线（Sink 0.42/Wind 0.6/Solid 0.04+0.46/Hands 0.24+0.54/Inhale 0.22/Away 0.26/Smoke 0.38/Lift 0.46+0.62/Pour 0.52+0.34+0.22，:116-152）；双层暗幕 VeilBack/Front（dark 0.86/dim 0.3）+加色 FlashRect（`FlashPeak*exp(-t/0.085)`）；`Scream` 读屏双波+三环 0.1→5.2 放射+48 粒爆（:1058-1142）；`AddWisps` 11 条雾丝（双拳 2 粗+9 细，贝塞尔头尾错相 `Lerp(num3,num2,u)`，:996-1056）；`wraith_shroud` 雾团 rise/burst/lean 三驱动（:452-491）。
- **数值**：`wraith.intro.scream_at=1.05`、`dark=0.86`、`dim=0.3`、`flash=0.55`、`mist=0.7`、`fire=1`、`pour_wisps=9`。
- **创意点**：旧代与新代（C1）并存互为 fallback——`WraithStage.Ready` 为假时整体退回 C# 版（WraithPlayFx.cs:245-248），双轨保险。

### C4. WraithHeld2.cs（462 行，新代待命）
- 编译 wraith_form.gd；`set_state(t,wake,clock,show)` 四参驱动（:353-359）；`set_top` 顶层协议（WZ_HEAD 双份绘制，02_shaders.md T11）；`SwayPhase` 把 gd 相位回传给出招接续（:374-392）；`Follower` Late pass 同步顶层 transform（:268-298）。数值：`wraith.held.haze=1`、`shake=0.6`、`haze_ui=0.45`。

### C5. WraithCardFire.cs（267 行）
- 劫持卡面固有火焰：`GetNodeOrNull("CardContainer/AncientBanner/Fire")`（:61-71）换 wraith_cardfire 材质（原火焰纹理双通道重映射 body=max(rgb)/heart=min(rgb)）；green/size 阻尼 9/7；0.25s 无请求自动还原、3s 强制回收（:220-245,:241）；`Restore` 完整还原材质/缩放/位置/速度五件套（:129-153）。
- **创意点**：不改节点只换材质——游戏火焰动画继续放，颜色被偷换成幽灵火。

### C6. WraithFormFx.cs（174 行，Power 虚无烟柱）
- wraith_smoke 前后两层 quad（layer 0/1、seed 0/11.7）；`Fog()` 三段：in 0.28 起→hold 0.9 全浓→out 1.1 消散，间隙 `fog_rest` 呼吸残留 `0.88+0.12sin(1.4t)`（:98-115）；`Waits` 查询 Intro Phase——幽魂开场演出期间 `_gate` 阻尼 9 压住常驻雾避免双重表演（:70-96）。数值：`wraith.body_alpha`、`fog_in=0.28`/`hold=0.9`/`out=1.1`/`rest=0.34`、`width_mul=1.9`/`height_mul=1.5`（tuning.ini:16-27）。

### C7. AssassinateFx.cs（404 行）
- OnPlay：目标身后 `Behind(width*0.5+46)` Puff+6 团 0.014s 错相尾迹+黑 veil（`assassinate.hush=0.42`，:204-238）；OnHit：红色刺击光束双 Glow（x0→x1 伸入 0.05s/hold 0.1/缩回 0.28，`ray=46*num` 厚）+双 Puff+16 粒血喷+Shake 1/2（:333-394）。**Replaces 让位**（:60-114）。数值：`assassinate.behind=46`、`hold=0.1`、`ray=46`、`cloud=1.25`、`dim=0.26`（tuning.ini:215-219,603-608）。

### C8. MurderFx.cs（1247 行）
- OnPlay：暗场 veil（held_dark 0.4→dark 0.55）+胸口血光 3.4x 缓放（:136-167）；OnHit 四段：`Wash` 白洗（1.3/0.8 倍身型双 Glow，stop*0.75 时退）→`Slash` 主刀（Crescent 血染+白芯，BackOut 0.35→1 过冲展开，0.035s 弹入）→stop 定格（`CreatureHold` 冻结 `stop`=Lerp(0.08,0.22,Weight)，Weight 由 8~45 伤害映射，:385-403）→`Open` 斩开（spray 22/shards 34+三道血 Dart）+beat 0.08 后 `Cross` 十字第二斩（:1125-1215）。
- **技法**：`Spring` 标准二阶阻尼推拽（ζ=0.42ω/√0.8236，:555-566）；`Light` 受害体提亮 `whiten=2.6` 倍（MP 下 LightGroup 多 blow 取 max 共享，:748-872）；**KaiserCrab 特例**：对 Crusher/Rocket 反射找 `%ArmBoneL/R` 骨骼节点推拽+`MegaSprite.AnimationState.SetTimeScale(0)` 停住 Boss 动画（:915-1018）。
- **数值**：`murder.dark=0.55`、`hold=0.75`、`shards=34`、`held_dark=0.4`、`mark_gap=78`（注释：正好避开敌人意图图标）、`stop_min=0.08`/`stop_max=0.22`、`beat=0.08`、`knock=20`、`spring=0.3`、`whiten=2.6`、`claw_knock=1.4`。
- **创意点**：**Boss 部件级交互**——全库唯一深入 Boss 骨骼节点的特效；定格时长由伤害重量决定（砍轻了停一下、砍重了多停）。
- **注释等效物**：`"the Shared crab let go"`、`"crab let go"`。

---

## ⑤ D 族·压制/暴露系技法目录

### D1. SuppressShow.cs（1417 行，旧代表演）
- **结构**：`_slot[4]`+`_wt[4]` 四帧混合采样（intro 15/idle 40/aim 40/attack 59）。
- **技法**：`Gather` 同时取 4 帧按权重写进 shader 的 tb/tc/td+w——idle/aim 交叉淡入是四路混合（:1040-1097）；攻击段 `_x` 以 `fps*(1.9 命中/1.15)` 推进，到 `Cut=9` 帧触发 `Global()` 斩中特写，之后 1.45/1.2 倍速放到 `SlowEnd=36`（:678-728）；`Shake` 5 帧 Jolt 表（(0,0)(11,-7)(-8,5)(5,-3)(-2,1)）*0.6Sc（:1371-1399）；`DrawSoul` 4 条二次贝塞尔把敌人的魂从胸口扯出+Swirl 环绕（:1171-1342）；`Press` 六道下压墨迹（:831-879）；`HideArrow` 反射 `_targetingArrow` 藏游戏原力箭头。
- **数值**：`suppress.size=1.75` clamp 0.6~3、`hit_wait=0.8`、`hold=0.32`（代码默认，ini 未列）；frames fps=24、cut=9、slow_end=36（suppress_frames.json）。
- **创意点**：**一帧 shader 混合四张序列帧**——避免双精灵交叉淡化的"两张图都半透"问题，任一时点只有一张完整图。

### D2. SuppressRun.cs（1265 行，新代表演)
- **结构**：GDScript 桥；`step(dt,mouse,presence,reduce)` 回传字典 `{done,yank,shake,hold,card_show,dim,cues}`（:568-688）。
- **技法**：`Yank` 把敌人身体节点往她方向拽（矩阵转入局部坐标，多 run 争抢时继承已应用量，:854-962）；`Dim` 压暗敌人身体（R*0.5/G*0.42/B*0.4，被改过就让位，:964-1046）；`SoulCopy` 捕获敌人纹理传 `set_soul`；`Hear` 把 cues 映射四音效（lift/bite/tear/burst，:691-721）；三次失败整体 `Fail` 降级（:457-460）。
- **数值**：`suppress.v3=1`、`v3.size=1`、`v3.hold=1`、`v3.yank=1`、`v3.dim=1`、`v3.hide_arrow=1`、`v3.over_ui=1`（tuning.ini 部分+代码默认）。
- **注释等效物**：`"Suppress: step() did not answer - this showing is removed"`（:578）。

### D3. SuppressStage.cs（398）+SuppressFrames.cs（337）
- Stage：`GDScript.SourceCode+Reload(false)` 编译 suppress_stage.gd（:279-287）；`warm()` 分帧读贴图（Cache 字典跨实例复用）；登记 ShaderWarmup；v3 全开关。
- Frames：`LoadStep` 每帧最多加载 4 张（`suppress.load_per_frame` clamp 1..32）防卡顿；空闲 20s 释放全部纹理（:297-320）。**代码默认 Size=(430,460) 与 03_notes 记录的 JSON (357,372) 不一致——以 JSON 为准，代码是兜底**。

### D4. ExposeWipe.cs（1966 行，UI 层总控）
- **结构**：`Run` 持有 Victim/Bar/Row/Shield/Outline/Gear/Sore 全套 UI 引用（:41-174）。
- **技法**：`Look` 找 NHealthBar/NPowerContainer/BlockContainer/Artifact 图标/Vulnerable 图标（:339-413）；`Open` 给齿轮和盾换 expose_wipe 材质（Dress，:472-516）；`Line` 每帧把扫描线 y 写进 shader `cut_y`（:518-530）；`Step` 逐元素协商：线到盾顶 2px 内记 ShieldPassed，等 `BlockOf<=0` 才放盾（"前七成硬顶、后三成崩"的代码化，:835-857）；齿轮被问（HoldRemoval）且线在其上方停 0.05s 才放（:825-832）；易伤图标 `SoreCut` 以 `(h+14)/0.12` 速度从下往上"写"出（:944-972）；三重保险 `wait=1.4`/`land=0.9`/`max_hold=1.6` 防死等（:308-310）。
- **数值**：`expose.wipe=1`、`wait=1.4`、`land=0.9`、`max_hold=1.6`、`gear_time=0.17`、`shield_time=0.15`、`cool=0.45`（tuning.ini:747-758）。
- **创意点**：**特效与游戏 UI 的逐帧协商协议**——HoldsBar/HoldRemoval/CoversRemoved/CoversApplied/EnterApplied 五个查询口让游戏的能力 vfx 与血条刷新在演出期间闭嘴，演出结束原样交还。
- **注释等效物**：`"the power row cannot be held (NPowerContainer.OnPowerRemoved) - the Artifact icon is left to the game"`（ExposeHooks.cs:33）。

### D5. ExposeFx.cs（680 行，扫描线编排器）
- **技法**：`Schedule` 生成 Leg 计划（Hold 0.05→Run→遇齿轮 Recoil 0.022+Grind `gear_time`（`Teeth` 锯齿：0.42 快+0.58 慢，:328-336）→遇盾 Push 0.7*shield_time+Give 0.3→Body 扫全身→+0.32 收尾，:124-287）；`Sweep` 每帧：wash 底色 Bare→Sore 渐变（底部积起 `min(1,depth/50)` 显现）+三带余晖（0.3/0.21/0.12f 递减）+扫描头 halo/line/hot 三层（1.12/1.0/0.8 宽度比）+障碍 spot/core 高光+flecks 粒子带（EmissionBox 随线宽，:460-567）；`Throw` 按 cue 撒火花（齿轮金 520px/盾冰晶 200-480px，:573-641）。
- **数值**：`expose.scan_time=0.24`、`bare=0.3`、`flecks=18`。

### D6. ExposeHooks.cs（174 行，Patches）
- 按名反射 `NPowerContainer.OnPowerRemoved`/`NCreature.OnPowerIncreased`（:44-82）；`Creates` 枚举 4 个 Vfx 类型（NPowerApplied/Debuff/Buff/RemovedVfx）的全部静态 Create 重载（:84-109）——**把游戏要放的能力 vfx 藏掉换成自己的表演**；验证自身补丁确已装上（:26-31）。

---

## ⑥ E 族·其他大件技法目录（从简）

### E1. StrangleRope.cs（1429）
- `Strand` 三层线+**Cover 渐变掩码**（6 段 offsets 0/0.002/0.003/0.004 极窄过渡，实现"绳在身体前后"的遮挡渐变，:99-125）；四段绳（run 远端 Behind 层+near 近端）；椭圆参数化 a0/b0→a1/b1 勒紧（`loose=0.72`/`bite=0.4`+give 余量）；`recoil=240` 回弹、`squeeze` 身体捏扁（`pinch=0.07`）；`Hang/Tighten/SnapFor` 与卡牌命令时序握手——等真实伤害落地才 Snap（:293-325,:407-430）。数值：`strangle.rope=10`、`girth=1`、`yank=0.05`、`over=0.12`、`await=4`（tuning.ini:906-912）。

### E2. KnifeTrapFx.cs（1510）
- `Trap` 持 Blade 环：半径 `clamp(width*0.75,110,220)*clamp(0.86+0.035n,0.9,1.45)`（:455-485）；sigil 法阵（dashes 12+ticks=max(n,6)）；`Order` 洗牌序逐刀刺入（`pierce_time=0.2`）；`Through` 穿透爆（飞出 `clamp(dist*exit/time,180,420)`+Blast）；`Volley` 与 Fan of Knives 联动（`TakeVolleyKnife` 从环上摘刀，:570-611）。数值：`trap.blades=8`、`time=1.15`、`blade_length=54`、`spin=0.40`+`spin_per_blade=0.075`、`pierce_time=0.20`、`final_shards=12`、`final_shake=2`（tuning.ini:320-337）。

### E3. EnergyBeads.cs（1251，跨卡复用原语）
- `Bead` 六层（Halo/Body/Ring/Glow/Core/Blot）；`ChemOf` 用 `Hook.ModifyXValue` 算出"X 被修改几点"→黑珠数量（:285-306）；`Counter` 跟随能量计数器、Relic 源跟随遗物图标（`FindCounter/FindRelic/Measure`）；`Rest()` 静止位形（gap=34/step=38+正弦浮动）；`OutTime=0.34`/`HomeTime=0.28`；`EnsureInkShader` 运行时把 sigil 的 `blend_add` 换 `blend_mix` 派生墨色版（:180-196）。数值：`beads.chem=1`、`chem_rim=1`、`chem_halo=1`（tuning.ini:766-772）。

### E4. StatusSteps.cs（1165）
- 给敌人能力图标数字做"计数爬升"：`Run/Counter` 作用域（Capture/Seal/Activate/Suspend）；`Count` 阻尼趋近；Flash/Refresh/Conceal/Pop/Settle/Show 六态（:787-964）；`StopEntrance` 阻止游戏自带数字入场；`LookMethods` 反射 NPower 三法（:1086）。

### E5. FlechettesOrbit.cs（1150）
- `Slot` 五段腿（Out→Orbit→Strike→Home→Done）；`Tilt` 把手中真实飞镖卡倾斜（`amp=0.035rad*sin(38Hz)` 高频细颤，ShakeTick:248-293）；`LiftOff` 藏真卡、`CardStandIn.Make` 复制品绕轨（`_rx=body.X*0.95`/`_ry=body.Y*0.32` 椭圆，spin 3.2，打出 1.35 倍）；`_layered` 前后半透明度差；beat 音效按 spin 相位 `Sfx.Lead` 对齐（:405-416）。数值：`flech.shake=0.035`、`orbit_x=0.95`、`orbit_y=0.32`、`spin=3.2`、`mark_size=54`（tuning.ini:624+代码默认）。

### E6. StormHeld.cs（1074）
- `Layer[]` 多层环（`min_layers=7`~10）；`Ring()` 每层一扁椭圆（`rx=body.X*(0.14+0.52*level^1.3)`、`ry=rx*0.26`、高度 `1.22*level`，:458-474）；手牌吞入 layer（`Hidden`+`Swallowed`）；`Speck` 16 粒切屑；`gust_every=0.42` 阵风、`pull_speed=2.3` 吸手牌。数值：`storm.min_layers=7`、`height=1.22`、`spread=0.52`、`turn=3.0`、`blades=10`、`turns=9.5`、`time=1.05`（tuning.ini:281-302）。

### E7. HandTrickWipe.cs（1046）+SlyGold.cs（732）
- HandTrickWipe：`Run/Mark` 记录选中牌；`WipePose` 用 CardRide.Pose 滑卡；`StartWipe` 金扫光（Gold 0.98,0.84,0.45）；`FlightBegan` 与游戏发牌飞行同步。SlyGold：CardDeco 描金边（`gold_edge=0` 默认关、`width=24`、`alpha=0.85`，tuning.ini:619-622）；`Gild` 侧向扫光；`Launch/Flip` 翻牌；`TryWaiting` 队列。

### E8. TheHuntFx.cs（766）
- `WouldKill` **伤害预演**（`PreviewDamage-Block+ModifyHpLost vs CurrentHp`，:145-176）——打出前算准会不会死；kill 记录位置/时刻/变换；`StartTrophy("reward-before-hit")`：奖励还没到时先把奖杯生成在击杀点（:303-306,:322-325）；OnHit 三段金色 Crescent（glow/cut/edge 0.05→1 渐入 0.07s）。数值：`hunt.cut=1.35`、`trophy=1.0`、`trophy_time=0.60`、`mark=0.50`（tuning.ini:593-596）。

### E9. RicochetRun.cs（861）
- `Mirror` 上下双镜（`At(x)=Edge±Sag*(1-u²)` 下垂弧，:30-37）；`Strike` 高斯热痕 `Heat=exp(-((x-a)/reach)²)`（:39-54）；AttackFeed Began/Ended/Damage 驱动、`PlannedHits` 预知段数；"瞬间"设置时 queue 0.2s 排队上限。数值：`ricochet.thick=15`、`thin=5`、`sweep=0.035`、`hold=0.02`、`fade=0.17`、`mirror_alpha=0.22`、`cool=4.5`、`queue=0.2`（tuning.ini:1066-1089）。

### E10. PiercingWailFx.cs（807）
- wail_warp 读屏三波前（fronts 3，Force 1/0.72/0.5，SagShare 1/0.45/0.3）；每波上下双弧（Arc Outer/Core 11 点）；Gather 0.07→Gap 0.1→Travel 0.52；`tall=0.92` 竖椭圆；受害者身体被推（Victim.Applied）。数值：`wail.fronts=3`、`time=0.52`、`tall=0.92`、`arc=0.8`、`sag=0.11`、`thick=0.26`、`bend=0.042`（代码默认，ini 未列）。

### E11. MirageFx.cs（356）
- `PoisonOn` 统计全场毒→`PowerOf`（`full_poison=24` 满功率）；`Ellipse` 身前雾团（`body.X*1.05 × body.Y*1.45`）；toxic_fog ellipse 形态 fill/scatter/flash 三驱动；`Wisp` 从每个中毒敌人拉 7 团雾到身前（0.075s 错相）；无毒也有 0.4 基础强度。数值：`mirage.streak_alpha=0.3`、`mist_alpha=0.13`、`mist_speed=620`（tuning.ini:395-397）。

### E12~E18. 小件群（各 30~215 行）
- **FootworkFx**（215）：三脚印 0.085s 错相（落地 1.75→0.88 回弹缩放+Wave 尘圈）；`Sway` 三残影，宽度按速度微分 `|dLean/du|` 拉伸（`Lean=sin(7.23u)*(1-u)`，:208-211）。
- **BubbleBubbleFx**（172）：有毒 stream=36 泡+big=7 大泡（0.32~0.62s 上升到顶 burst 7 粒）；无毒 8 小泡。
- **ReflexFx**（58）：胸前竖切+UI 层双 Dart 从抽堆飞向手牌。
- **UntouchableFx**（56）：双侧 Arc 护壳。
- **ExpertiseFx**（54）：Brush.Orb 牌飞抽堆——**仅当 `GameCapabilities.ExpertiseRetainsDrawnCards` 探测为真才播**（与本机日志 False 对应）。
- **TacticianFx**（42）/PreparedFx（41）/AnticipateFx（35，收束环 1→0.38）/EscapePlanFx（30，脚下 16 粒尘爆）：全部一句式原语。

---

## ⑦ 跨族共性技法编目（16 条）

1. **Live 表+Ticker 自挂+异常自毁**：每个大特效都是 `static List<Run> Live + bool _ticking`；`Update` 内逐项 try-catch → Free → RemoveAt——单体特效永不通向游戏崩溃（SkewerFx:1357-1384、MurderFx、ShivFan、StrangleRope 同构）。
2. **FxKit.Drive 单时间线**：几乎所有动画一条 `Drive(owner, seconds, step, trans, ease)` 走完，回调内 `u*seconds` 换算（FxKit.cs:663）。
3. **FxKit.Damp 指数阻尼**：全场出现 200+ 次，rate 8~22 是常用区间（趋近/淡入淡出/混合）。
4. **缓动词汇表**：t²（飞行）、1-(1-t)²（浮起）、smoothstep、BackOut（过冲展开）、1-(1-t)³（弹出）、二阶 spring（受击）、exp(-kt)（衰减）、sin(πt)（单峰包络）——同族卡片用同一套词汇保持手感一致。
5. **Line2D 三件套**：WidthCurve（锥形）+Gradient（头尾淡出）+Additive 材质，刀痕/绳/雾丝/触须全部 Line2D 化。
6. **Brush 一句式 API**：Cut/Dart/Arc/Wave/Dust/Sparks/Motes/Gust/Orb 十个动词覆盖 80% 小件。
7. **Glow+SoftDot+Additive 组合**：任何"发光体"都是 FxKit.Glow 一个 Sprite2D。
8. **读屏后处理三件套**：crest=exp(-x²)/bend 推挤回摆/RGB 分离（8 个 shader 共享，02_shaders.md ③.5）。
9. **CreatureHold 冻结真实目标**：串刺/谋杀/压制/收人头四家用——`Hold(target, seconds, token)` 引用计数，子弹时间也走这套。
10. **让位协议**：`Replaces/CoversStab/CoversBite/keep_swing/keep_stab` 一族开关，决定"游戏原生动画闭嘴多少"。
11. **MP 作用域链**：`RemoteFxFade.Capture(...) using` + `Current/Valid/ScopeAlive/OwnerPresent` 四重校验，远端特效可见性/存活判定完全同构。
12. **Tuning 双层默认**：`Tuning.F(key, def)`+ini 精选；每个异步动画都有超时保险（expose.wait/land/max_hold、malaise.max_hold、ricochet.idle、strangle.await、shadowstep.wait）。
13. **反射私有字段**：AccessTools.FieldRefAccess 拿 `_swooshPath`（飞牌曲线）、`_targetingArrow`（原力箭头）、`_animIn`（NPower Tween）、`_rowList`（能力行）、`_targetingArrow`——拿不到就降级不崩。
14. **手牌四件套**：CardStandIn（复刻/藏/显/暗）、CardFlight（认领/交还）、CardDeco（挂件 tick）、CardRide（位姿插值）——所有"牌飞"特效的公共基础设施。
15. **FxKit.Hash(i) 稳定随机**：每次运行同一序列，保证特效可复现可调试。
16. **GDScript 桥双协议**：`Call("step"/"set_time"/...)` 下行 + `times()/chest()/phase()` 上行；Stopwatch 自计每帧毫秒并在销毁时汇报（WraithPlayFx/WraithHeld2）。

---

## ⑧ 创意点总榜 Top20

| # | 特效 | 创意点 | 证据 |
|---|---|---|---|
| 1 | WraithPlayFx | 反射改游戏飞牌曲线终点到"她的胸口"——游戏资产替 mod 叙事 | WraithPlayFx.cs:663-679 |
| 2 | EchoingSlashFx | 整段 GLSL 以 C# 字符串内嵌；6 弧"水平面环片"近远半圆分两层画 | EchoingSlashFx.cs:211-220 |
| 3 | ExposeWipe/Fx | 扫描线与游戏 UI 逐帧协商：白线扫盾"前顶后崩"、磨齿轮、自下而上"写"出易伤 | ExposeWipe.cs:835-857; ExposeFx.cs:124-287 |
| 4 | FinisherFx | 本回合攻击牌复刻成飞镖；末击把敌人身体节点按进地里再还 | FinisherFx.cs:368-564,:1181-1350 |
| 5 | MurderFx | KaiserCrab 骨骼级停帧（TimeScale=0）+受害体 2.6x 提亮共享组 | MurderFx.cs:915-1018,:748-872 |
| 6 | PreciseCutChains | 读 CalculatedDamage/BaseValue 把"强化倍率"折算成刀刃打磨进度 | PreciseCutChains.cs:269-307 |
| 7 | TheHuntFx | 打出前预演伤害算准会不会死；奖杯生成在击杀点而非奖励点 | TheHuntFx.cs:145-176,:303-306 |
| 8 | EnergyBeads | Hook.ModifyXValue 把"X 被改几点"可视化成黑珠数量 | EnergyBeads.cs:285-306 |
| 9 | EchoingSlashFx | 欠账-补刀：刀气没到就先记账，半径推进到了再补（与真实结算对齐） | EchoingSlashFx.cs:993-1002 |
| 10 | SuppressShow | 一帧 shader 混合四张序列帧（tb/tc/td+w 四路权重） | SuppressShow.cs:1040-1097 |
| 11 | StrangleRope | Cover 渐变掩码（6 段 0.004 宽 offsets）实现绳身遮挡过渡 | StrangleRope.cs:99-125 |
| 12 | WraithFigure | 数据驱动人形（扁平坐标数组+黄金角入场螺旋+逐点描绘） | WraithFigure.cs:470-504,:577-596 |
| 13 | SnakebiteFx | 蛇"缠"而非"射"：椭圆环勒紧+后拽 p³+95Hz 颤动+头尾速度差 | SnakebiteFx.cs:797-823 |
| 14 | FlechettesOrbit | 真卡 38Hz 细颤倾斜+复制品绕轨，分层透明度 | FlechettesOrbit.cs:248-293 |
| 15 | EnvenomFx | 液滴三 Sprite 组合+解析弹道+沿速度拉伸；连击 0.45s 内减效 | EnvenomFx.cs:236-330 |
| 16 | RicochetRun | 上下双镜 Sag 弧+高斯热痕+AttackFeed 预知段数 | RicochetRun.cs:30-54 |
| 17 | WraithHeld2/PlayFx | WZ_HEAD 双份绘制协议（同一物体画两份、alpha 份额和=1） | wraith_form.gd:698-746（02_shaders T11） |
| 18 | FootworkFx | 残影宽度按速度微分拉伸（快=窄长） | FootworkFx.cs:193-201 |
| 19 | StormHeld | 手牌吞入多层环，层数=clamp(手牌数,min_layers) | StormHeld.cs:313-385 |
| 20 | WraithCardFire | 换材质不改节点的卡火劫持+0.25s 空闲自动还原 | WraithCardFire.cs:61-71,:220-245 |

**Top5 速览**：① 飞牌曲线改终点；② 内嵌 GLSL 水平环片；③ 暴露扫描线与 UI 协商；④ 复刻牌飞镖+按敌入地；⑤ Crab 骨骼停帧+提亮共享组。

---

## ⑨ 与初探报告的差异补充

1. **tuning.ini 隐藏键的族分布实证**：strangle/fan/echo/ricochet/wail/footwork/suppress/cloak/symphony/precise/deflect 七族绝大多数键只在代码里（ini 仅列 snake/finch/storm/trap/hunt/assassinate 等主键）。例：`fan.spread=11/draw_back=12/trap_exit=0.16` 有 ini 行，但 `fan.hit_wait=0.35` 之外的 `wail.*` 24 键、`suppress.v3.*` 8 键、`footwork.*` 7 键全 code-only——与 01_architecture ④.7 的 462 隐藏键结论互相印证。
2. **SuppressFrames 尺寸口径**：03_notes 记录 JSON `size=[357,372]`，而 SuppressFrames.cs 默认 `(430,460)`——JSON 是权威，代码默认是兜底（初探未标此差异）。
3. **Expose 的深度被初探低估**：初探认知是"白线抹盾"，实际是与 NHealthBar/NPowerContainer 的逐帧协商协议（五个查询口+三重超时+易伤图标写出动画+齿轮停问 0.05s），是本 mod 与游戏 UI 耦合最深的系统。
4. **CardFx 55 注册的内部差异**：初探只知数量；本次实测同族内部分三层——大件（1000+ 行自带状态机与 Ticker）、中件（200~700 行一条 Drive）、小件（<60 行单发原语），注册表扁平但实现厚度差 40 倍。
5. **缓动手感的族间一致性的证据**：A 族飞行用 t²、C 族浮现用 1-(1-t)²、D 族扫描用 smoothstep、受击全场用同一 spring 公式（ζ=0.42ω/√0.8236，MurderFx:555-566 与 FinisherFx 的 Jolt 同源）——初探未归纳。
6. **"让位"是全局范式而非个例**：除已知的 keep_swing/keep_stab 外，新发现 CoversBite（蛇）、HoldsBar/HoldRemoval/CoversRemoved/CoversApplied/EnterApplied（暴露）、HoldRemoval（StatusSteps）、Swallowed（Storm）、HidesThrow（ShivFan）、Replaces（Finisher/Assassinate）——共 10+ 个让位查询口，构成"mod 与游戏动画层的仲裁协议"。
7. **GDScript 桥的上行协议**：初探只记了 `Call("step")` 下行；本次确认 `times()/chest()/phase()/hand_pos()` 四个上行查询与 Stopwatch 自计时汇报，C# 与 gd 是双向握手。
8. ** ExpertiseFx 的能力门控实例**：初探日志 `Expertise retain FX=False` 的含义落地——ExpertiseFx.cs:29 直接 `if (!GameCapabilities.ExpertiseRetainsDrawnCards) return`，特效整体不存在而非静默失败。

---

*报告完 · deep-cards-sweep · 2026-10-16*
