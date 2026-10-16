# SilentFX 文本笔记挖掘报告（03_notes）

> 挖掘代理：recon-notes（初探第一轮）｜日期：2026-10-16
> 材料根目录：`D:\项目\dragonraja\sts2_work\3817850774\`（只读）
> 结论先行：**tuning.ini 是这个 mod 的"设计文档"**——1210 行里 652 个参数键、约 45 个 section，几乎每个参数都带中英文注释，解释"这个数管什么、为什么取这个值、哪个版本改的、旧值是多少"。作者把创意点、踩坑、版本演化史全部以注释形式埋在了配置文件里。C# 侧只提供兜底默认值（`(defaults live in the code: ...)`），tuning.ini 才是发布层的权威。

---

## ① tuning.ini 全 section 地图

### 1.1 文件概况

- 规模：1210 行，652 个参数键，59 个参数前缀，约 45 个显式 section 标题（`# --- 名称 ---`）+ 8 个大节标题（`# === 名称 ===`）。
- 文件头（tuning.ini:1）：`# SilentFX tuning. Reloaded with F8 (dev preview). Sizes are in game pixels.` —— **F8 热重载**，尺寸单位为游戏像素。
- 版本对应：mod 版本 v0.72.7（SilentFX.json:6），日志确认 `v0.72.7.0 initializing`（silentfx.log:2）。

### 1.2 解析机制（对照 SilentFX.Core.Tuning.cs）

- `Tuning.Load()`（SilentFX.Core.Tuning.cs:8-39）：读 `tuning.ini`，逐行 trim；跳过空行与 `#`/`;` 开头行；按第一个 `=` 切分；值同时尝试 `float.TryParse` 与 `int.TryParse`（InvariantCulture），存 `(float?, int?)` 二元组。
- 读取 API：`Tuning.F(key, def)` / `Tuning.I(key, def)`（同文件 41-57 行）——**键不存在返回代码默认值 `def`；键存在但类型不匹配也返回 `def`**。Dictionary 用 `StringComparer.OrdinalIgnoreCase`（键大小写不敏感）。
- 文件缺失时不报错、直接返回（无 tuning.ini 也能跑，全靠代码默认值）。
- 关键设计：tuning.ini 里大量出现 `(defaults live in the code: xxx)` 注释（如 638-640、651-656、663-669、627、701 行），说明**三层默认值机制**：代码默认 → tuning.ini 覆盖 → 用户层覆盖（见 ②）。

### 1.3 参数命名规范

- 形式：`前缀.参数名`（如 `bullet.freeze`、`shadowstep.roof_dim`、`sfx.murder_drone`）。
- 前缀 = 卡名/能力/系统的英文缩写或关键词，一张卡一个前缀；复合卡共用一个 section（如 `deadly/stab/snake` 三张毒系卡共用 tuning.ini:399-427 一节）。
- 前缀规模 Top15：shadowstep 57、sfx 50、malaise 41、tools 38、planner 28、mood 28、bullet 24、thorns 22、wraith 20、tour 20、ricochet 17、burst 16、storm 16、blades 15、snake 15、echo 15。
- 参数名自带语义后缀：`*_time`（秒）、`*_alpha`（透明度 0~1）、`*_mul`（倍率）、`*_px`（像素）、`*_size`、`*_scale`、`on/enable`（开关）。开关惯例 `1 = 开 / 0 = 关`，且注释常注明"0 = 回到旧版行为"（如 265、538、734、746 行）。

### 1.4 Section 清单（按文件顺序，含主题与代表参数）

| 行号 | Section | 主题 | 代表参数 |
|---|---|---|---|
| 3-7 | Openings | 能力牌打出时的"快速开场秀" | intro.on/scale/reach |
| 9-14 | Noxious Fumes | 毒雾（每个敌人脚下坐一团雾） | noxious.burst_gas=14, burst_bubbles=18 |
| 16-27 | Wraith Form / Intangible | 幽魂形态/虚无：身体透明度+每回合开局拢过来的雾 | wraith.body_alpha=0.60, fog_in/hold/out=0.28/0.90/1.10, fog_rest=0.62（浪涌间隙残留） |
| 29-49 | Infinite Blades | 无限之刃：肩后无限符号+每层一把刀 | blades.arc=260, fly_time=0.55, redraw=0.55 |
| 51-63 | Phantom Blades | 幻影刀刃：手里每张飞刀对应一把幽灵刀 | phantom.fan_degrees=120, lean_degrees=30 |
| 65-66 | Envenom | 涂毒（ Silent 身上无表现，全在受击敌人那） | envenom.splash_scale |
| 68-72 | Afterimage | 残影 | afterimage.gap=24, alpha=0.40 |
| 74-109 | Abrasive | 磨蚀（只对来自 Abrasive 卡的荆棘显示）；**注释明说"一切都按 183 单位 hitbox 设计"**（75 行） | thorns.ring_count=12, ring_turn=4.5, flinch_kick=720 |
| 111-127 | Master Planner | 谋划专家：头顶羽毛笔+卡上签名线 | planner.open_delay=0.74（等开场秀演完）, blink=0.09（不飞位，原地淡入淡出） |
| 129-132 | Sneaky | 诡秘烟雾 | sneaky.smoke=16, pulse_time=0.55 |
| 134-156 | Tools of the Trade | 必备工具：刀伺候抽牌堆、拴绳苦无伺候弃牌堆 | tools.fetch=1（刀把牌挑出来甩进手里）, blade_shape=0/1/2 |
| 158-169 | Tracking | 追踪：脚印小径+锁定全套秀 | tracking.slam=1（全套：锁定线、砸落、闪光、环、火星）, shake=2, veil=0.26 |
| 171-193 | Well-Laid Plans / 计划妥当 | **中文注释**：牌上平时什么都没有；回合结束留在手里的牌往弃牌堆冲、被拴在两个下角的蓝丝拽回（v0.43） | plans.seep=1（划线后往下渗）, lunge=260, spring=9.3, damp=0.68 |
| 195-199 | Burst | 爆发（能量小球） | burst.rx_mul/ry_mul, speed=5.2 |
| 203-213 | Murder | 谋杀：悬停预览暗场+标记 | murder.held_dark=0.40, mark_gap=78（注释：这个空隙正好避开敌人意图图标） |
| 215-219 | Assassinate | 刺杀 | assassinate.behind=46, keep_swing=0 |
| 221-265 | Bullet Time | 子弹时间：**本 mod 最复杂的系统**（24 键） | bullet.freeze=1（时停）, hold_sound=1, replay=0.10, enemy_speed=0.04, float_gap=150, hold_death=1, yield_to_other_mods=1 |
| 267-274 | Malaise | 萎靡节奏键（注释写明 v0.52→v0.58→v0.61 三次语义迁移） | malaise.lead=0.10, span=0.80 |
| 276-302 | Storm of Steel | 钢铁风暴：待命时 stacked 风环+吸手牌 | storm.min_layers=7, pull_speed=2.3, gust_every=0.42 |
| 304-317 | Shivs | 飞刀通用：飞刀长度/飞行/舞动/旋回 | shiv.length=56, return_time=0.27（Leading Strike 旋回手里） |
| 319-337 | Knife Trap | 刀刃陷阱：刀圈旋出、每把刀穿过敌人炸开 | trap.pierce_time=0.20, final_shards=12, final_shake=2 |
| 339-344 | Accuracy | 精准瞄准 | accuracy.scope=74, reach=0.85 |
| 346-357 | Poison | 敌人身上的毒：气泡+撕咬 | poison.full_at=64（到此层数最浓，以上起雾）, mist=12 |
| 359-364 | Accelerant | 助燃剂 Chevron | accelerant.chase=7 |
| 366-368 | Corrosive Wave | 腐蚀波：抽牌时沿线下扫的光 | wave.splash=10 |
| 370-375 | Concoct | 调合剂 | concoct.fizz=7, splash=18 |
| 377-387 | Outbreak | 爆发（v0.37 重做：毒蔓爬向每个敌人） | outbreak.drops=110, creep=1.0/creep_fast=1.5 |
| 389-391 | Bubble Bubble | 泡泡泡沫 | bubble.stream=36, big=7 |
| 393-397 | Mirage | 幻影：迷雾 | mirage.mist_speed=620 |
| 399-427 | Deadly Poison / Poisoned Stab / Snakebite | 致命毒药/涂毒刺/蛇牙（31 键共用节） | snake.drag=190, flee=760, keep_bite=0 |
| 429-434 | Showcase (F4) | 展示模式：慢动作、敌人血量、能量 | demo.slow=0.3, enemy_hp=400, energy=6 |
| 436-438 | General | 全局 | fx.screen_shake=1 |
| 440-451 | Developer / perf | 性能与开发者 | perf.prewarm=1, shader_warmup=1, console_log=0, trace=0, tour.on=0 |
| 453-470 | dev / Test bench | F5 慢动作+测试台 | bench.enemy_hp=9999, low_hp=10（注释：The Hunt 打 10 点）, energy=9 |
| 472-578 | 暗影步 Shadow Step | **最大 section（70+ 键）**：影子=黑雾，六阶段"蓄→冲→甩→升→悬→砸"（v0.47.0）；v0.48 天幕；v0.49 翻倍回合 | shadowstep.roof=1（天幕）, roof_dim=0.78（"已经接近全黑"）, spike_over=0.18 |
| 580-588 | Shadowmeld | 暗影融合 | shadowmeld.hollow=0.62 |
| 590-596 | The Hunt | 狩猎 | hunt.trophy=1.0, trophy_time=0.60 |
| 598-608 | Round 14 | 后期牌补充（planner.return、assassinate.dim/hush/hold/ray/cloud） | assassinate.hush=0.42 |
| 610-627 | v0.20/v0.21 手牌相关 | 抽/弃动画速度（2慢1快0关）、Sly 金边（v0.21 起默认关）、飞刀标 | hand.pace=2, sly.gold_edge=0 |
| 629-633 | v0.24 | Noxious Fumes 常驻雾浓度 | fumes.fog_min=0.28 |
| 635-644 | v0.25 Precise Cut | 精准切割：牌自己就是刃，牌沿磨成钢坡（shaders/card_edge.gdshader） | （默认全在代码：edge_from=44 等） |
| 646-656 | v0.27 Wraith Figure | 卡面女子用纱布条带做成（Src/Cards/WraithFigure.cs；WraithFigureData.cs 由 docs/ref/wraith_figure_gen.py 生成）；列出**不再读取**的旧键 | — |
| 658-689 | v0.28 / v0.47 Wraith | 云原型搬进游戏（wraith/ 目录：wraith_form.gd 待命 / wraith_play.gd 打出）；v0.47 幽魂形态跨界面/吹散/模糊 | wraith.ui.top=0.15, blur=1, play.wave_push=0.011 |
| 691-695 | v0.29 Finisher | 收人头：击回卡牌的连击 | finisher.keep_swing=0, keep_stab=0 |
| 697-715 | v0.36/v0.50/v0.53 | Finisher 末击砸地；Skewer；Pinpoint 准星清晰度随费用变化、慢慢漂 | pinpoint.still=1.8, shut=0.14 |
| 716-743 | v0.54/v0.55 Burst | 小球飞牌上炸开 → v0.55 改为炸出牌的虚影取代第二段打出 | burst.ghost=1, ghost_alpha=0.72 |
| 744-763 | v0.56/v0.57 Expose | 暴露：白线抹护盾/磨人工制品、写出易伤 | expose.wipe=1, same_slot=1, max_hold=1.6 |
| 764-774 | v0.58 X号化合物 | 黑球（串刺/萎靡共用）+ 测试台 artifacts/block 按钮 | beads.chem=1, chem_halo=1.0 |
| 775-863 | v0.58~v0.64 Malaise | 萎靡四代演化（见 ③） | malaise.coil=2.2, coil_turns=5.5, puffs=36 |
| 864-891 | v0.67 刀扇 | 小刀展开成 X 把（X=可打击敌人数） | fan.spread=11°, stagger=0.03 |
| 892-918 | v0.51 四个小优化 | 精准拉线、幻影 hover、紧勒 Strangle、Memento Mori | strangle.yank=0.05, over=0.12 |
| 920-1026 | sounds and "feel" (v0.37) | 音效+音乐心情（见 ③） | sfx.ceiling=-1.5（总线限幅 dB）, mood.on=1 |
| 1028-1062 | 手打展示 tour | 字幕展示系统（中文） | tour.font=28, finale_dim=0.90 |
| 1064-1089 | 连续反弹 ricochet | 上下一对镜子白光折（中文） | ricochet.queue=0.2（游戏设"瞬间"时排队上限） |
| 1091-1121 | 谋划专家 v0.42 | 每张牌一支羽毛笔画越摆越小的线（中文） | planner.write=0.90（v0.42.1: 0.24→0.90，"一只手不慌不忙签个名的时间"） |
| 1123-1166 | 必备工具 v0.42 | 刀扎牌堆挑牌+苦无（中文，含 v0.42.2/.3/.4 三次修订） | tools.shot_speed=9000, embed=22 |
| 1168-1179 | 独门技术 v0.43.3/.4/.5 | 牌下沉让位+蓝光扫过；抽牌"从台面下递上来"；删掉"羽毛飘进手" | moves.expertise_duck=130 |
| 1181-1207 | 回响斩击 v0.44.0 | 水平面刀气，跟游戏真实结算走 | echo.catch_up=0.06, force_hits=0（调试键） |
| 1209-1210 | MP | 联机适配总开关 | mp.remote_fx=1 |

---

## ② 配置分层架构（四层 + 覆盖优先级）

从 tuning.ini 注释、sfx/README.txt 与 PlayerSettings.cs / Sfx.cs 可还原出完整分层：

```
第 4 层（最低）：C# 代码内置默认    —— Tuning.F/I 的 def 参数；tuning.ini 注释 "(defaults live in the code: ...)"
第 3 层：       tuning.ini          —— 作者发布层；v0.71.3 起注明是"干净安装的官方默认"（922-924 行）
第 2 层：       user_settings.ini   —— 玩家层（RitsuLib 设置页写入），仅 9 个键
第 1 层（最高）：sfx/choices.ini    —— 音效候选 picks，胜过 tuning.ini 的 sfx.* 行（921 行："wins over the lines here"）
另：            sfx/<name>.ogg      —— 本机音频文件整体覆盖该名称默认声（README.txt:9）
```

**证据链**：
- PlayerSettings.cs 只管理 9 个键（SilentFX.Core.PlayerSettings.cs:42-58）：`fx.enabled / hand.pace / sfx.on / sfx.volume / mood.on / fx.screen_shake / fx.reduce_screen_effects / perf.prewarm / perf.shader_warmup`。
- **回退链**（PlayerSettings.cs:236-259 `ReadBoolean`/`ReadNumber`）：user_settings.ini 没有该键 → 回退读 tuning.ini 的 legacy 键（如 `sfx.on`）→ 还没有 → 硬编码默认。即"用户设置 > tuning.ini > 代码默认"。
- `ResetPageDefaults()`（209-234 行）硬编码 9 键默认值（fx.enabled=1, hand.pace=2, sfx.on=1, sfx.volume=1, mood.on=1, fx.screen_shake=1, fx.reduce_screen_effects=0, perf.prewarm=1, perf.shader_warmup=1）。
- user_settings.ini 保存策略（SaveUser, 407-469 行）：保留文件中的未知键与注释，写 tmp 后 Move 覆盖；无文件时创建带中文注释头的文件。
- hand.pace 特殊：user_settings.ini 里可写 `off/fast/slow` 字符串或 0/1/2 数字（PlayerSettings.cs:367-378），数字被 Clamp 到 0..2。
- choices.ini 由 Sfx.cs 管理（SilentFX.Core.Sfx.cs:813 `PicksPath`），记录 41 个 slot 各自选了第几个候选；tuning.ini:920-921 说明面板里"靠耳朵"定的值进 choices.ini。
- sfx/choices.ini 与 user_settings.ini **不在本机**（sfx/ 目录只有 README.txt）——README.txt:13 明确"它们是本机个人配置，不属于发布包"；重装/更新时应保留（README.txt:15）。

### 2.1 配置层边界实证（recon-arch 差集实证，2026-10-16；详见 01_architecture.md ④.7）

- **键总数**：代码侧 `Tuning.F/I` 字面量共 **1029** 个不同键，tuning.ini 文档化 **652** 个 → **462 个 code-only 隐藏键**（约为 ini 的 71%）。集中度 Top10 前缀：gamble 59 / finisher 36 / wraith 30 / memento 27 / precise 21 / strangle 21 / fumes 20 / envenom 19 / skewer 17 / flech 16。ini 里 5 处 "(defaults live in the code: ...)" 注释只点名约 30 个。**逐键全表（键=默认值+调用点）已由 recon-arch 建成：01_architecture.md 附录 A（852 行，462 键/509 调用点，多调用点键的默认值经校验 0 冲突）。**
- **反向差集修正**（纯字面 regex 会误判"死键"）：约 100 个「ini 有、代码无字面量」的键全部有动态读取路径——① `sfx.*` 每槽家族走 `Sfx.Knob("sfx."+name)` 拼接（Sfx.cs:1079/1100/1195/1268/1440，含 `.every`/`.lead` 后缀）；② `mood.*` 走 `Sfx.Knob("mood.*")`（Mood.cs:330/368/373/375）；③ `moves.*` 大多走本地包装器 `Seconds("moves.*", def)`（HandMoves.cs:369）；④ `perf.prewarm`/`perf.shader_warmup`/`sfx.on`/`mood.on`/`sfx.volume` 是 PlayerSettings 的 legacyTuningKey 旧版回退（PlayerSettings.cs:381）。
- **真实结论**：「ini 是精选入口、代码是全量默认」——可调面约为 ini 所列的 **1.6 倍**；且 Tuning.F/I 的 def 兜底（键缺失/类型不符/文件缺失全静默回退，Tuning.cs:41-57）保证从 ini 删任何键都不炸。
- **配置全字典拼法**（二探可直接用）：`01_architecture.md 附录 A（462 隐藏键+默认值+调用点）` + `tuning.ini（652 文档化键）` + 四条动态路径键（`sfx.*`/`mood.*` Sfx.Knob 拼接、`moves.*` HandMoves.cs Seconds 包装、`perf.*`/`sfx.on`/`mood.on`/`sfx.volume` PlayerSettings legacy 回退）。

---

## ③ 开发笔记与创意点编目

### 3.1 卡牌与系统设计

1. **Bullet Time 时停的哲学**（tuning.ini:230-234）：「It only changes what is shown, never what happens, and is always off in co-op.」——时停只改表现不改事实；联机永远关闭。这是整个 mod 的设计总纲（SilentFX.json:5「不修改战斗规则或游戏结果」）在单卡上的落地。
2. **追赶压缩**（238-239）：时间恢复后被压住的效果按发生顺序放出，"all but at once"——整个追赶压进 `bullet.replay=0.10` 秒；注释坦承"it used to be 1.6, one thing after another"（旧版 1.6 秒逐个放）。
3. **mod 间共存礼仪**（263-265）：`bullet.yield_to_other_mods=1`——若装了 "Bullet Time: As Depicted"，SilentFX 自己不冻（"two freezes at once fight each other"）。
4. **暗影步的六阶段编排**（472-486）：蓄→冲→甩→升→悬→砸，每阶段一个秒数键；v0.48 天幕的意象注释（535-537）：「砸下来的时候天幕就是它的尾巴，顺着来路被拽过去、从她头顶灌进地里」——把整段动画写成一句视觉叙事。
5. **弹簧重物模型**（190-193）：计划妥当的牌「是挂在弹簧上的重物被踢了两脚」，spring=9.3/damp=0.68，注释精确说明"0.68 时回位只过头一成不到"。
6. **谋划专家签名线**（1091-1121）：每张牌一支羽毛笔在描述区画一条越摆越小的线（ink_waves=3.25 个来回）；v0.42.1 把画线时间 0.24→0.90 秒，理由是"一只手不慌不忙签个名的时间"；`hold=1` 扣住牌晚点飞但注明"只是画面上晚一点飞，游戏本身不等"。
7. **苦无卸力**（1161-1166）：v0.42.4「回到弃牌堆时不急停——苦无带着余势绕空中的固定点转一圈卸力」，loop_keep/loop_lag 控制挂好后晃动。
8. **萎靡的四代演化史**（775-863）：v0.58 能量球吸力量 → v0.60 球在敌人面前闲逛+胸口红光 → v0.62/63 拔河式"拽三下、一下比一下狠" → v0.64「找回 v0.58 的感觉，更浓」换成雾漩涡。每代旧键注明"已删"（777、798、853 行）。还有精确的时间预算注释（825 行）：每颗球咬住到吞下约 1 秒 = 0.12 + 3×0.17 + 2×0.10 + 0.16。
9. **暴露白线**（744-763）：一根白线扫过敌人，磨人工制品（gear_time）、顶崩格挡盾（shield_time，"前七成硬顶，后三成崩开"）、从下往上"写"出易伤图标；三重保险 wait/land/max_hold 防死等。
10. **刀扇**（864-891）：拖出小刀时绕刀柄展开成 X 把（X=敌人数），敌人数变化平滑增减；放回合拢；与幻影刀刃/子弹时间/精准的交互全部写明。
11. **回响斩击的"跟真结算"**（1181-1207）：刀气水平面里飞，"跟游戏真实的每一下结算走"，echo.catch_up=0.06 追赶已结算但刀气未到的情况；force_hits 是调试键（"大于 1 时假装杀了几只"）。
12. **精准准星的生命感**（708-715）：准星清晰度随当前费用变化（原价没有、便宜一费又大又虚、0 费又小又亮不动）；v0.53 准星慢慢漂 1.8px——「对得再准也不是贴在敌人身上的一张图」。
13. **独门技术的"从台面下递上来"**（1175-1179）：抽牌时牌沉到屏幕底边下、一点蓝光贴底滑到位置、牌从底下升进手里；v0.43.5 删掉旧的"羽毛飘进手"（"没有开关了"）。
14. **连续反弹的排队**（1085-1089）：游戏设置"瞬间"时所有伤害同帧出，白光一折一折排队最多落后 0.2 秒；idle/never 两个保险参数防动画不收场。
15. **爆发小球的两代**（716-743）：v0.54 牌一打出小球就飞过去炸开 → v0.55 改成留在身后等第二次打出、炸出金色牌虚影"打出"后消散。

### 3.2 音效与"音乐心情"

16. **全部音效"靠耳朵定"**（920 行）：「all of it is set by ear in the test panel: 测试 -> 选音效」——41 个 slot 的音量混音是耳朵调的，v0.71.3 起把用户确认值回写为官方默认（922-924 行注明来源：archived choices.ini, 2026-10-11）。
17. **两个"房间"混响**（982-987）：hall = 长混响（给尖叫、待命、谋杀刀切这些该"响下去"的）；low = 低通（给谋杀的低鸣）。hall.wet/room/damping 三参可调。
18. **总线限幅器**（988-990）：「never goes above this (dB), however many loud pieces lie on top of each other」——sfx.ceiling=-1.5 dB，无论叠多少大声部都不越过。
19. **mood 系统**（991-1026）：对游戏**自有音乐**做 volume/pitch/far（低通）/hollow（混响）处理，「nothing is added to it」；谋杀/子弹时间/幽魂/刺杀各有配方；mood.room_max=-5、room_highcut=5000 等全局上限。
20. **酸液池的随机乐句**（977-981）：不是一直响，约每 3 秒一个气泡，四成概率来一小段"两三个快 pip、停顿、一个肥厚低 bloop"。
21. **程序化洗牌声**（SilentFX.Core.Sfx.cs:855-879 `Riffle`）：用 `Random(seed)` 把 ticks 次纸牌堆叠声（cardmvmt_stack_v1_rr1..10）+ 滑动声 + 合上书声按幂律间隔排成一条"洗牌"候选——不依赖任何洗牌音频素材。
22. **音效全部来自游戏自有 FMOD 样本**（Sfx.cs Slot Options；README.txt:7）：如谋杀底噪候选 `STS2_Act2_SFX_Amb_Insaitable_v2` 等；v0.69 新配音 7 张牌也全部取自游戏样本（955-956 行）。用户可放 `<name>.ogg` 覆盖（README.txt:9）。
23. **Option 候选的参数语义**（Sfx.cs `O()`/`L()` 构造，815-843 行）已由 recon-arch 据 `Start()` 逐行实证并写入 **01_architecture.md ⑦**：From=起播点、To=截止点（StopAt=(To−From)/速率）、Gain=音量乘子（Clamp 0~8）、Speed=速率乘子（Clamp 0.25~4）、Lead=预滚提前量（wait−Lead/Speed）、Bank=FMOD 银行名；另有 Soft/Tail/Via/With/After 五个修饰与 40ms 去重 + 8 语音池两个节流。本线不再重复实证，直接引用该节。

### 3.3 MP（联机适配）

24. **总开关** `mp.remote_fx=1`（tuning.ini:1210）。
25. 日志事实（silentfx.log:5）：`[MP] Exact targeting unavailable; using native peer input: game handshake version metadata is absent`——本机构建缺乏握手版本元数据，精确目标不可用，回退到原生玩家输入。mp 前缀在 tuning.ini 中仅此 1 键（其余逻辑待二探确认）。

### 3.4 性能与工程

25. **资源预热**（442-443）：`perf.prewarm=1`——离开战斗 0.5 秒后每帧准备一个资源单元；0 = 恢复懒加载。
26. **Shader 预热**（444-445）：在私有 32x32 viewport 画缓存 shader，注释诚实标注「Best effort; not a guarantee for all driver variants」。
27. **日志镜像**（446-447）：`perf.console_log=0`——INFO 留在 silentfx.log；1 才镜像到 Godot stdout。
28. **计时开销自警**（448-449）：`perf.trace=0`——"profiling itself has overhead"，正常游玩应关闭。
29. **帧纹理分批加载**（SilentFX.Cards.SuppressFrames.cs:255-258）：`suppress.load_per_frame`（默认 4，clamp 1..32）限制每帧加载几张猎人手帧，防卡顿——**该键在 tuning.ini 中没有对应行，属代码默认键（待确认是否有意省略）**。
30. **发布卫生**（450-451）：demo 热键/补丁需要 tour.on=1 或 dev_preview.txt；「Release packages should use 0 and omit the marker.」

### 3.5 调参哲学（元笔记）

31. **F8 热重载**（tuning.ini:1）：改 tuning.ini 即时生效（dev preview）。
32. **"0 = 回到旧版"惯例**：几乎所有开关的 0 值都绑定一个历史版本行为（538 行"回到 v0.47 只有短尾巴的样子"、734 行"完全回到 v0.54"、762 行"回到 v0.56"）——参数即版本史。
33. **保险参数文化**：expose.wait/land/max_hold、malaise.max_hold、ricochet.idle/never、strangle.await、shadowstep.wait——每个异步动画都有"等不到就自己收"的超时。
34. **一切按 hitbox 说话**：尺寸注释反复出现"按 183 单位 hitbox"（75 行）、"enemy widths"（383 行）、"screen heights per second"（245-246 行）——用游戏内可感知单位描述，而非裸像素。
35. **数值即理由**：如 `planner.card_size=330` 注明"按全尺寸牌 330px 宽来算光"（126 行）、`spike_width` 注明旧值 0.95（567 行）——调参结论直接写进注释。

---

## ④ 帧动画与 rig 数据结构解析

### 4.1 suppress/suppress_frames.json（猎人手帧元数据）

- **顶层**（JSON 1-18 行）：`frames_version="202"`, `fps=24`, `scale=0.38`, `size=[357,372]`, `ground=[171.9,336]`, `faces=-1`, `aim_v=112.0`, `attack_from_aim=3`, `cut=9`, `slow_end=36`。
- **note 字段**（18 行，唯一一条自由文本备注）：「hunter faces screen-left in the source (target on the left); f = front fist (rig arms.left, user's right hand), c = near-chest fist (rig arms.right, user's left), h = head, sh = front shoulder; export-pixel coords inside size」——猎人面朝屏幕左；f/c/h/sh 四个锚点与 rig 骨架手臂命名体系的对应关系。
- **groups 四组**：intro 15 帧 / idle 40 帧 / aim 40 帧 / attack 59 帧（共 154 个 PNG，suppress/frames/ 实际 154 个文件吻合）。
- **每帧字段**：`file`（PNG 路径）、`f/c/h/sh`（四个锚点坐标，逐帧变化）、`v`（标量：intro 组从 -1375.0 匀速到 -791.7；aim 组恒等于 aim_v=112；attack 组 0→1186）、`f5`（映射到 C# 的 `Fade`，SuppressFrames.cs:249——即淡入淡出值，不是"第五个拳"）、`snapped`（bool，attack 组 50/59 帧为 true）、`pres`（1.0 全程恒定，intro/idle/aim）；idle/aim 组多 `j` 字段，attack 组多 `i/u` 字段，外加 `speed`。
- **C# 实际只读** f/c/h/sh/v/f5/snapped（SuppressFrames.cs:243-251）；`pres/speed/i/u/j` 是导出脚本附带的参考数据（未被 mod 读取，待确认其语义）。
- **生成管线**：JSON 由 `docs/ref/suppress_cut/attack_v1/export_game_202.py` 生成（SuppressFrames.cs:173 缺失时的报错原文）。
- **加载校验**（SuppressFrames.cs:190）：要求 intro>0、idle>0、aim==idle 帧数、attack > cut+2；缺失记 Error「no hunter」。
- **frames_version=202** 与导出脚本名 `export_game_202.py` 对应（版本号=脚本版本，待确认）。

### 4.2 wraith/rig.json vs wraith/rig2.json

**完全相同**：`size=[1212,1704]`、`scale=2`、25 条 strands（逐字节一致，Python 深比较确认）。**chest** 微差：rig `[604,680]` → rig2 `[604,690]`（胸点上移 10px）。

**strands（25 条纱布条带，两份一致）**：每条含 `name / kind(fan|front) / family / tex(s_XX.png) / ox,oy(原点) / pts(路径点 31~113 个) / left,right(每点左右半边宽，构成 fan 形轮廓) / arc / len(弧长 199~844)`。命名分 family：L1~L5（左侧 fan）、R1~R5（右侧 fan，含 t/b 变体）、F1~F10（正面 front）。rig.json 引用 tex/ 目录纹理；tex3/ 目录存在同名的 s_*.png（tex3 是 rig2 时代的重制版纹理，待确认）。

**layers 差异（核心）**：

| | rig.json（7 层，v0.27 幻影版） | rig2.json（25 层，v0.28+ 现役版） |
|---|---|---|
| 躯体 | dress, body | dress, **torso, skull, under_jaw, under_neck, torso_c** |
| 附件 | — | **hornL/hornR（带 base/tip 弯曲角）、lowL/lowR（带 elbow/fist 手臂骨骼）、lowL_c/lowR_c、eyefire、slits** |
| 火焰 | fireL/fireR（仅 ox/oy/w/h + foot） | fireL/fireR（+ **k=3.0 与 shapes=39/35** 程序化火焰关键帧 + tip）、eyefire（k=3.0, shapes=6）、slits（k=3.0, shapes=12） |
| 辉光 | glow, glowL, glowR | + **glow_torso/glow_hornL/glow_hornR/glow_lowL/glow_lowR/glow_eye/glow_slits** |
| 纹理路径 | 全部 tex/ 裸名 | 混用：裸名（tex2/）、`../tex/`、`../tex3/` |

- rig2 的 hornL base/tip、lowL elbow/fist 是显式骨骼锚点（供 GDScript 驱动形变）；k=3.0 + shapes=N 表示该层火焰按 N 个形状关键帧插值（k 疑为曲率/刚度参数，待确认）。
- 两份 rig 由 wraith_form.gd（待命）/wraith_play.gd（打出）消费；gd 头部注释（wraith_form.gd:2-10、wraith_play.gd:2-14）说明它们源自"云原型"（docs/ref/wraith_cut/anim/form.gd 与 anim2.gd），由 mod 而非渲染器驱动，API 为 `build(dir, cache, back, front, lo, hi, org, k[, who])` / `set_state(t, wake, clock, show)` / `set_time(t, clock)` / `times()` / `chest()`。
- suppress3/ 目录是另一套 suppress 材质（suppress_stage.gd + 12 个 gdshader + tex/ 的 aura_k2/k25/k3/k4.png）——k 系列纹理与 rig2 的 k=3.0 疑同源（待确认）。

---

## ⑤ 日志事实清单（silentfx.log，18 行）

| 行 | 事实 |
|---|---|
| 2 | `SilentFX v0.72.7.0 initializing`——与 SilentFX.json version 0.72.7 一致 |
| 3 | `PlayerSettings: loaded user_settings.ini`——本机**有** user_settings.ini（与 sfx/ 目录无 choices.ini 不矛盾；日志存在即文件被读过，待确认是本机残留还是空文件） |
| 4 | **能力探测**：`Shiv creation args=3, Poison trigger=False, Concoct=False, Expertise retain FX=False`——mod 启动时探测游戏构建能力，缺失能力走降级路径 |
| 5 | **[MP] WARN**：精确目标不可用（game handshake version metadata is absent），回退原生 peer input |
| 6-7 | HandPace：当前偏好 slow；RitsuLib 设置页可用（Settings > Mod设置（RitsuLib）> SilentFX） |
| 8 | **Sfx ready: 41 sound(s)**——与 Sfx.cs `new Slot[41]` 精确对应；"0 picked by hand, 0 number(s) set in the panel"（本机无 choices.ini 覆盖） |
| 9 | `Power FX unavailable in this game build: ConcoctPower`——本游戏构建无 ConcoctPower，降级 |
| 10 | **PowerFxManager ready, 26 power effect(s) registered**——26 个能力特效 |
| 12-13 | `Card FX unavailable: Fade / Sidestep`——两个卡牌特效本构建不可用 |
| 14 | **CardFxManager ready, 55 card effect(s) registered**——55 个卡牌特效 |
| 15 | **HeldFxManager ready, 34 standby effect(s) registered**——34 个待命（持牌悬停）特效 |
| 16 | **Harmony patched methods: 80, failed classes: 0**（+ 83 个游戏 effect factories 被 Bullet Time 监视）——80 个运行时补丁零失败；83 个工厂被监视用于时停冻结/恢复 |
| 17 | `AttackFeed: 7 patch(es) installed`——攻击反馈管线 7 个补丁 |
| 18 | `Init finished` |

日志未出现 suppress 帧加载行（`Suppress: hunter frames listed ...`，SuppressFrames.cs:191）——本次日志片段内未触发 suppress 读取（帧在首次使用时才懒加载，待确认）。

---

## ⑥ 【线索清单】给二探的深挖点

**tuning.ini 内部**（精确到关键词）：
1. ~~搜 `defaults live in the code`（tuning.ini 638/651/663/627/701 行）→ 挖出只在代码里的键与默认值全集。~~ **【已被 recon-arch 实证完成，见 01_architecture.md ④.7 与本报告 ②.1：1029 代码键 vs 652 ini 键，462 个隐藏键；反向差集的四条动态路径也已厘清】**
2. `suppress.on` 与 `suppress.load_per_frame`（SuppressFrames.cs:166/257）——ini 中无此行，代码默认 1 和 4；确认是否有意省略。
3. `bench.block`（tuning.ini:774）夹在 v0.58 beads 注释里，位置异常，确认归属 section。
4. `tour.*` 20 键（1028-1062）+"手打展示"字幕系统：脚本化巡演（"慢动作 = 开"语法），找 Tour 相关类。
5. `mp.remote_fx`（1210 行）全文唯一 MP 键 → grep `remote_fx` 挖联机适配全貌。
6. `echo.force_hits` / `echo.preview_hits`（1191-1207）调试键 → 找 dev preview 里如何驱动。
7. ~~统计 652 键 vs C# 中 `Tuning.F/I` 调用总数差集——找出 ini 未列出的隐藏键。~~ **【已由 recon-arch 完成：差集 = 462 个隐藏键，且逐键全表（默认值+调用点，0 冲突校验）已建于 01_architecture.md 附录 A；本报告 ②.1 已引用。剩余可选深挖：按前缀建键语义说明（如 finisher.* 36 键与 FinisherFx/Final_dart/blink/shake 的对应）——属代码线，我的文本线不重复做】**

**音效**：
8. 41 slot × 多候选（Sfx.cs:192 起 `new Slot[41]`）与 tuning.ini 48 个 `sfx.*` 音量键的逐一对账；`Page` 字段（v0.69"新配音"页）分页逻辑。
9. `sfx.<name>.lead`（926 行注释）——ogg 迟到补偿机制，grep `lead`。
10. `Riffle` 程序化合成（Sfx.cs:855）与 `mood.*` FMOD DSP（room_max/room_decay/room_highcut）的实现。

**帧动画/rig**：
11. `export_game_202.py`、`wraith_figure_gen.py`、`docs/ref/wraith_cut/anim/form.gd`、`anim2.gd`、`docs/ref/suppress_cut/attack_v1/`——被引用但不在本材料目录的生成管线源头（可能在仓库别处）。
12. suppress_frames.json 的 `pres/speed/i/u/j` 字段语义（C# 不读）；`v` 的确切含义（intro 组 -1375→-791.7 是什么量）；`faces=-1` 如何翻转。
13. rig2 `k=3.0` 与 `shapes=N` 火焰插值参数含义；`torso_c/lowL_c/lowR_c`（_c 后缀=collision？cover？）；tex3/ 与 tex/ 同名纹理的差异。
14. wraith_form.gd / wraith_play.gd 的 `times()` 返回值（[SNAP, ...] / [T_BURST, T_GO, T_HIT, end]）与 C# 侧握手协议。

**日志/能力探测**：
15. "Game capabilities" 探测机制（Shiv args=3、Poison trigger、Concoct、Expertise retain FX 四项 False 的后果）——搜 capabilities/probe 相关类。
16. 83 个 effect factories 的 Bullet Time 监视名单如何收集（Harmony patch 的 patch 目标枚举）。
17. 日志中本次未出现的行（suppress frames listed、MP exact targeting 成功时长什么样）——需要一次含 suppress 出场/联机的完整日志。
18. HandPace "preference slow; effective slow" 的 effective 计算（slow/fast/off 与 hand.fast_scale=0.6 的关系，SilentFX.Core.HandPace.cs）。

**外部事实**：
19. "Bullet Time: As Depicted" mod（tuning.ini:264）——社区已有同类 mod，SilentFX 主动让位；值得作为竞品参考。
20. RitsuLib（设置框架，可选依赖）——SilentFX.json dependencies 为空但日志 7 行显示其可用；确认软依赖检测方式。

---

## 附：本机材料清点

- 已读：tuning.ini（1210 行全读）、SilentFX.json、silentfx.log、sfx/README.txt、suppress/suppress_frames.json（结构+统计）、wraith/rig.json、wraith/rig2.json（结构+对比）、SilentFX.Core.Tuning.cs、SilentFX.Core.PlayerSettings.cs、SilentFX.Core.Sfx.cs（局部）、SilentFX.Cards.SuppressFrames.cs（局部）、wraith_form.gd/wraith_play.gd（头部注释）。
- **本机不存在**：sfx/choices.ini、user_settings.ini（sfx/ 目录仅 README.txt；日志 3 行显示 user_settings.ini 曾被加载，待确认）。
- 未深入（属其他代理职责范围）：shaders/、suppress3/ 的 12 个 gdshader、wraith/ 的 8 个 gdshader、SilentFX.dll 本体。
