# SilentFX 音频三层架构深挖报告（07_audio · deep-audio）

> 对象：SilentFX v0.72.7（杀戮尖塔2 mod，作者奶酪用 Claude 编写）
> 材料：`silentfx_decomp/split/` 反编译切分源码 + `3817850774/` mod 资产（只读）
> 行号均为**切分文件行号**（文件头有原行号映射）。读不透处标「待确认」。
> 结论先行：**这个 mod 用三层结构实现了「零音频资产」——GameSounds 直接从游戏 FMOD 银行切样本重建 Ogg 流，Sfx 用 41 槽 145 候选的参数化选音面板做混音编排，Mood 通过 Win32 导入表钩子直连游戏音乐总线做滤波闪避。任何一层失败都只降级不崩溃。**

---

## ① 音频三层架构图

```
┌─────────────────────────────────────────────────────────────────────┐
│ 第一层 GameSounds（供料 · SilentFX.Core.GameSounds.cs, 503 行）        │
│   直接打开游戏 res://banks/desktop/<bank>.bank（FSB5 容器）            │
│     Open()     扫描 FSB5 魔数 → 解析头/样本索引/名字表（:142-274）      │
│     Setup(crc) 从游戏旁 fmod.dll 镜像里按 CRC 挖出 Vorbis setup 头      │
│                （PE 节表 RVA→文件偏移 + CRC32 校验，:276-369）          │
│     ToOgg()    把 FSB 内 Vorbis 包流重包成标准 Ogg 页流                 │
│                （手写 OggS 页 + identification/comment/setup 三头，    │
│                 :418-505）                                            │
│     Build()    FileAccess 切字节 → LoadFromBuffer → Godot AudioStream  │
│                （:98-140）                                            │
│   产出：AudioStream 缓存（Streams 字典，按 bank/sample 键）             │
│   自警：>8ms 切样告警 / 战斗中冷切告警 / 样本改名一次性 Warn             │
├─────────────────────────────────────────────────────────────────────┤
│ 第二层 Sfx（编排 · SilentFX.Core.Sfx.cs, 2275 行）                     │
│    Slots[41]（:192-788）= 145 个候选 Option                            │
│     Option：Sample/From-To 截取/Lead 提前量/Gain/Speed/Via 总线/        │
│             Soft 淡入/Tail 截尾/With 叠加层（:6-65）                    │
│    choices.ini 手工选音 + Knobs 面板数值（:1105-1187）                  │
│    播放 API：Play/One/PlayAt/Hold/Hush/Stop（:1206-1320,1877-1939）     │
│    语音池：预热 8 个 AudioStreamPlayer，峰值 28（:763,928-937,1859）    │
│    Bed：A/B 双播放器正弦交叉淡入淡出环境底噪（Weave :2088-2131）        │
│    Scatter：战斗中的随机 sprinkles（Sprinkle :1941-2005）              │
│    总线：SilentFX_Out(限幅)→Hall(混响)/Low(低通)（:1658-1755）          │
├─────────────────────────────────────────────────────────────────────┤
│ 第三层 FmodNative + Mood（音乐闪避 · 仅 Windows）                      │
│    FmodNative（1039 行）：                                              │
│      Hook()  VirtualProtect 改页保护 → 往 GodotFmod 导入表写钩子，       │
│              截获 Studio::System::update 调用，从 this 拿 system 句柄   │
│              （:535-668；3 秒等不到即放弃）                             │
│      Attach() GetBus("bus:/master/music")→LockChannelGroup→            │
│              插低通(+EQ 备选)+混响 DSP，bypass 待命（:774-1041）        │
│      Set(cutoff,wet)/Meter()（:374-464）                               │
│    Mood（419 行）：                                                     │
│      Lease 字典（音量/音高/闷罩/房间感/攻/放/租期）→ 阻尼趋近 →          │
│      双通道下发：FmodNative.Set(滤波参数) + 游戏音乐事件                  │
│      set_volume/set_pitch（:121-395）                                  │
│    WraithAudioWarmup（137 行）：非战斗期预挂中立 DSP 链并保持 bypass，   │
│              幽魂出场时零延迟（:14-113）                                │
└─────────────────────────────────────────────────────────────────────┘
三者关系：GameSounds 供料 → Sfx 编排 →（Mood 需要时）FmodNative 直连游戏音乐总线。
Sfx 全部走 Godot 音频（不碰 FMOD）；FmodNative 只服务 Mood，且可整体失败降级。
```

数据面：41 槽 / 145 个候选 Option / 45 处 L() 叠加层定义 / 约 132 个去重游戏样本名
（另有 Stack/Slide/Geyser/Slime 四个按 k 展开的样本族，运行时再翻几倍）。
日志佐证：`Sfx ready: 41 sound(s), 0 picked by hand, 0 number(s) set in the panel`（silentfx.log:8）。

---

## ② Sfx 音槽模型全表（41 槽 × 145 候选 × 参数）

### 2.1 Option 参数模型（SilentFX.Core.Sfx.cs:6-65）

| 字段 | 含义 | 默认 |
|---|---|---|
| `Sample` / `Bank` | 游戏 FSB 银行内样本名 / 银行名 | bank="sfx"（另有 "ambience"） |
| `From` / `To` | 样本内截取区间（秒）；To>From 时播放到 To 停（StopAt 计算 :1649） | 0 / 0 |
| `Lead` | 提前量（秒）：`PlayAt` 提前这么久触发，让音的「大声点」落在指定时刻（:1206-1208） | 0 |
| `Gain` / `Speed` | 增益 / 变速（PitchScale，clamp 0.25~4，:1629） | 1 / 1 |
| `Via` | 总线："hall"（混响）/ "low"（低通）/ null（Out 限幅总线） | null |
| `Soft` | 淡入秒数（VolumeLinear 0.0001→目标 的 Tween，:1635-1641） | 0 |
| `Tail` | 截尾淡出秒数（StopAt 前这么多秒开始 Fade，:1650） | 0.05 |
| `After` | With 叠加层的延迟触发（秒；≤0.004 同帧叠，:1480-1493） | 0 |
| `With` | 叠加层 Option 数组（Plus() 设置，:60-64） | null |

两个工厂函数：`O(标签,样本,from,lead,gain,speed,to,bank)`（:815）与叠加层专用 `L(样本,from,gain,speed,after,to,bank)`（:830，无 Lead）。

### 2.2 Slot 标志位

| 标志 | 效果 | 出现槽 |
|---|---|---|
| `Bed=true` | Hold 走 Bed A/B 交叉淡入淡出（环境底噪） | murder_drone、wraith_held、outbreak_creep、memento_hang、malaise_drink、storm_wind |
| `Fixed=true` | 候选锁定，忽略 choices.ini（Chosen 强制 Default，:998-1001） | flechette_orbit、wraith_scream、pool、storm_wind、storm_gust |
| `Settled=true` | 仅面板显示「已定：」前缀（TestBench），纯 UI 标注 | wraith_scream、pool、storm_wind、storm_gust |
| `Every>0` | Hold 走 Scatter 随机 sprinkles（每隔 Every 秒左右一声） | pool（Every=6） |
| `Default=-1` | 默认「不配」（静音该槽） | suppress_bite、malaise_drink |
| `Page 1..4` | 测试面板分页（纯 UI） | — |
| `FadeIn/FadeOut` | Bed 的升起/消散时间 | 各 Bed 槽 |

### 2.3 41 槽全表

记法：`@From(→To)` 截取区间；`L` 提前量；`G` 增益；`×` 速度；`in/cut` 淡入/截尾；`+n` 叠加层数；via 省略=总限幅总线。样本名压缩显示（去公共前后缀）。

| # | 槽名 | 中文标签 | 页 | 特性 | 默认 | 候选（关键参数） |
|---|---|---|---|---|---|---|
| 1 | murder_drone | 谋杀·低沉的底 | 1 | Bed FadeIn0.9 | 1 | ①无餍·低鸣 Act2_Amb_Insaitable @2→32 G5.5 ×0.5 ambience/low ②深处·底噪 Act3_Amb_Base @5→35 G5.5 ×0.55 amb/low ③蓄能·脉动 OrbPlant_ChargedLoop @0.05→2.6 G0.4 ×5.45 low ④末日·轰鸣 doom_kill_layer1 @0.25→0.3 G0.6 ×1.3 low |
| 2 | murder_edge | 谋杀·定格的一丝 | 1 | | 1 | ①金属轻鸣 silent_cast_metallic @0.33 L0.13 G6 in0.02 hall ②细刃 the_kin_slash_blade @0.03 L0.12 G2.4 hall ③快刀·高 attack_blade @0.02 L0.08 G0.3 ×1.5 hall |
| 3 | murder_cut | 谋杀·第一刀（刃+沉） | 1 | | 1 | ①快刀+低沉 attack_blade @0.02 L0.11 G0.95 hall +1层(forge_lowend) ②骨刃+血墙 necro_attack_blade @0.03 L0.13 G0.75 hall +1层(bloodwall) ③双斩+闷震 test_subject_slash @0.18 L0.15 G1.6 in0.01 hall +1层(crab_slam) |
| 4 | murder_cross | 谋杀·第二刀 | 1 | | 0 | ①回刀 attack_blade_rr3 @0 L0.12 G0.6 ×1.18 hall ②细刃 the_kin_slash_blade_rr3 @0.03 L0.12 G2.6 hall ③骨刃·高 necro_blade_rr2 @0.03 L0.13 G0.5 ×1.2 hall |
| 5 | wraith_held | 幽魂·待命 | 1 | Bed FadeIn0.6 | 0 | ①魂核·低吟 soul_nexus_die @0.05→2.65 G0.75 hall ②魂核·灼 soul_burn @0.3→2.1 G0.6 hall ③骑士·鬼火 spectral_knight_flame @0.7→2.3 G0.55 hall ④涅奥·空灵 Neow_Amb @2→18 G1.6 amb hall |
| 6 | wraith_knock | 幽魂·胸口一顿 | 1 | | 2 | ①闷响 Block_Break_Low @0 L0.05 G0.62 ×0.8 ②暗球 dark_channel_l1 @0.04 L0.1 G0.5 ③咒 spectral_knight_hex @0.2 L0.36 G3 ④重击2 HitImpact_Heavy_2 @0 L0.02 |
| 7 | wraith_land | 幽魂·鬼火入体 | 1 | | 2 | ①魂鱼·虚无 soulfysh_intangible @0.15 L0.3 G0.5 ②魂鱼·重现 soulfysh_reappear @0.1 L0.4 G1.1 ③暗球2 dark_channel_l2 @0.04 L0.25 G0.55 ④增益 buff_layer2 @0 L0.23 G3 |
| 8 | wail | 尖啸·一声叫 | 2 | | 3 | ①鸟妖·长啸 byrdonis_hurt @0.05 L0.04 G0.45 hall ②鸟妖·怒啸 byrdonis_angry @0.12 L0.06 G0.42 ×1.1 cut0.15 hall ③骑士·惨叫 knight_hurt @0.05 L0.1 G0.42 hall ④魂鱼·音波(升调) soulfysh_sonicwave @0.5 L0.17 G0.4 ×1.45 in0.03 hall |
| 9 | assassin_beat | 刺杀·心跳 | 2 | | 2 | ①振翅闷拍 owl_fly_loop @0.09 L0.14 G2.6 low ②低鼓 HitImpact_Heavy_3 @0 L0.03 G0.35 ×0.8 low ③闷响 Block_Break_Low @0→0.35 L0.05 G0.3 ×0.7 cut0.1 low |
| 10 | assassin_pierce | 刺杀·穿透 | 2 | | 1 | ①刺客 RubyRaiders_Assassin @0.14 L0.2 G0.6 hall ②潜行刺 gremlin_stab @0.03 L0.1 G0.42 hall ③佣兵刺 merc_stab @0.09 L0.14 G0.4 hall |
| 11 | finisher_last | 终结技·最后一击的分量 | 2 | | 0 | ①低沉 forge_lowend @0 L0.1 G0.6 ②闷震 crab_slam @0.05 L0.12 G0.6 ③血墙 bloodwall @0 L0.12 G0.5 |
| 12 | shadow_slam | 暗影步·扎进弃牌堆 | 2 | | 1 | ①闷响 Block_Break_Low @0 L0.05 G0.5 ×0.85 ②暗球 dark_channel_l1 @0.04 L0.1 G0.45 ③船击 haunted_ship_attack @0.04 L0.18 G0.4 |
| 13 | gamble_shuffle | 计算下注·洗牌 | 2 | | 5 | ①Riffle 细密(seed11,t16,span0.42,bend1,G3.2) ②Riffle 由慢到快(seed23,t14,span0.5,bend0.6,G3.4) ③Riffle 厚牌慢洗(seed37,t10,span0.55,G3.8,×0.78) ④Riffle 两手对洗(seed41,t12,slides2) ⑤只唰一下 Slide3 @0 L0.14 G1.2 +3层 ⑥牌库声 deck_open @0 L0.2 G0.3 +1层 |
| 14 | outbreak_creep | 毒性爆发·蔓延 | 3 | Bed FadeIn0.06 | 2 | ①藤蔓爬 vine_shambler @0.5→1.5 G0.8 ②绞藤 strangler @0.17→0.8 G1.6 ③黏液爬 beserker_slime @0.25→1.45 G1.6 ④毛虫脚 fuzzy_wurm @0.15→0.8 G2 ⑤黏液·缓 Amb_Goo_3 @0.4→3 G4.5 amb |
| 15 | outbreak_erupt | 毒性爆发·喷出来 | 3 | | 0 | ①喷泉+低沉 Geyser3 @0.46 L0.19 G1 +1层(kick,low) ②喷泉·粗 Geyser1 @0.46 L0.19 G1.15 ×0.8 +1层 ③喷泉+重踏 Geyser2 @0.46 L0.19 G0.95 +1层(stomp) ④纯喷泉 Geyser3 @0.47 L0.18 G1.15 |
| 16 | bubble_boil | 咕嘟冒泡·沸腾炸泡 | 3 | | 3 | ①沸腾+泡炸 bubbles @0.56→2.3 L0.3 G1.3 +4层 ②黏液咕嘟 Amb_Goo_2 @0.5→2.6 L0.55 G2 in0.03 amb +2层 ③水弹连炸 splash @0.19 L0.3 G0.4 +3层 ④腐蚀沸腾 steam @9→10.4 L0.2 G0.7 ×0.75 in0.1 cut0.6 +1层 |
| 17 | hunt_trophy | 狩猎·战利品 | 3 | Outlives 豁免 | 3 | ①金币 gold @0.05 L0.6 G0.9 +1层(treasure) ②宝箱 treasure_a1 @0 L0.06 G0.6 hall ③引路星 guiding_star @0.22 L0.16 G0.4 hall ④铃 Tingsha @0 L0.07 G1.6 hall |
| 18 | memento_hang | 铭记死亡·暗牌悬着 | 3 | Bed FadeIn0.5 | 2 | ①末日·气声 doom_kill_l3 @0.35→1.95 G0.4 hall ②咒 knight_hex @0.2→1 G2.2 hall ③魂核·低吟 soul_nexus_die @0.05→2.65 G0.6 ×0.85 hall |
| 19 | memento_slam | 铭记死亡·一起砸进去 | 3 | | 0 | ①骨法+闷响 necro_attack_magic @0.05 L0.2 G0.5 hall +1层 ②暗球 dark_channel_l1 @0.04 L0.1 G0.5 hall ③惊惧 terror_eel @0.03 L0.2 G0.4 cut0.3 hall |
| 20 | snake_bite | 蛇咬·合拢 | 3 | | 1 | ①噬咬 scroll_bite @0.1 L0.11 G1.2 ②大口 insatiable_bite @0.08 L0.18 G0.8 ③鼠咬 rats_bite @0.12 L0.11 G0.35 |
| 21 | snake_snap | 蛇咬·拉断爆开 | 3 | | 0 | ①崩断+毒溅 Block_Break_High @0 L0.02 G0.55 hall +1层 ②水弹 splash @0.36 L0.13 G0.5 in0.03 +1层 ③易伤击+茧 vulnerable @0.04 L0.1 G0.5 hall +1层 |
| 22 | trap_ring | 刀刃陷阱·刀升起 | 3 | | 0 | ①金属轻鸣 cast_metallic @0.33→1.3 G5 in0.03 cut0.35 hall ②铁链 Amb_Chains2 @0.06→1.4 G5 in0.03 cut0.4 amb ③刀扇余音 fanofknives @0.75→1.6 G0.6 in0.03 cut0.3 |
| 23 | track_slam | 跟踪·准星砸下 | 3 | | 1 | ①重落 DropRelic_Heavy @0 L0.04 G0.6 hall ②锁定 Block_Gain_Hit @0.22 L0.04 G0.8 hall ③易伤击 vulnerable @0.04 L0.1 G0.45 hall |
| 24 | flechette_orbit | 飞镖·牌转过身前 | 3 | Fixed | 0 | 唰1..5 = Slide(1/4/6/9/10) @0 L0.14 G0.8/0.7/0.7/0.7/0.8 |
| 25 | suppress_mist | 压制·牌炸成雾 | 4 | | 0 | ①猎人施法风 cast_whoosh @0.3→1.1 L0.18 G4 in0.02 ②雾吹 fog_blow @0.3→1.3 L0.14 G1.2 in0.02 ③出牌+雾起 cardmvmt_power @0.25 L0.15 G1.5 in0.02 +1层 ④雾怪聚起 Fogmog_Summon @0.62→1.1 L0.17 G0.8 in0.02 |
| 26 | suppress_bite | 压制·链头咬住 | 4 | | -1 | ①蟹钳剪 crab_scissor @0.26→0.7 L0.08 G0.4 in0.01 ②卷轴啮咬 scroll_bite @0.14→0.45 L0.08 G1.2 in0.01 ③锁上+铁链 Block_Gain_Hit @0.22 L0.04 G0.6 +1层(Chains1,amb) ④鼠咬 rats_bite @0.15→0.6 L0.08 G0.45 in0.01 |
| 27 | suppress_tear | 压制·撕魂 | 4 | | 1 | ①重击+处决 Heavy_1 @0 L0.03 G1 +2层 ②船击+闷震 ship_attack @0.15→0.5 L0.07 G0.65 +1层 ③贪食者终结 insatiable_finisher @0.38→2.2 L0.08 G0.55 hall ④重击+魂核 Heavy_3 @0 L0.03 G0.9 +1层 |
| 28 | suppress_burst | 压制·魂炸开散去 | 4 | | 2 | ①炸开+消散 fog_explode @0→0.7 L0.04 G0.6 +1层 ②魂鱼散去 soulfysh_die @0.08 L0.09 G0.55 hall ③雾灵消散 fog_die @0.25→1.8 L0.24 G0.75 hall ④亡魂 knight_die @0.08 L0.12 G0.5 hall |
| 29 | shadow_strike | 暗影步·砸地 | 4 | | 0 | ①蟹砸地+重击 crab_slam @0.03 L0.24 G0.8 +1层 ②母兽砸 matriarch_slam @0.15 L0.24 G0.6 ③巨人踏 giant_stomp @0.1→1.2 L0.32 G0.6 ④末日落 doom_kill_l2 @1.95→2.45 L0.32 G0.6 |
| 30 | malaise_drink | 萎靡·汲取 | 4 | Bed FadeIn0.25 | -1 | ①吸 Mite-Suck @0.45→1.35 G1.6 ×0.85 ②液化 insatiable_liquify @0.35→1.3 G0.9 ×0.9 ③魂核灼 soul_burn @0.5→1.8 G1.3 ×0.85 ④流涎 insatiable_salivate @0.35→0.85 G1.1 ×0.85 |
| 31 | malaise_tear | 萎靡·最后一颗球撕下来 | 4 | | 3 | ①减益 debuff_l2 @0 L0.1 G0.9 +1层 ②舔干 SpinyToad-Lick @0.55 L0.39 G0.8 ③虚弱重压 decemillipede @0.24 L0.29 G0.45 ④低沉(原来选的) forge_lowend @0 L0.1 G0.5 |
| 32 | expose_gear | 暴露·白线磨过齿轮 | 4 | | 2 | ①钻头 defect_drill @0.12→0.42 L0.14 G0.7 in0.01 +2层 ②齿轮咔咔 Block_Hit_High @0 L0.03 G0.8 +4层 ③工厂机械 fabricator @0.45→0.72 G1.5 in0.01 +2层 |
| 33 | expose_shield | 暴露·白线压碎护盾 | 4 | | 0 | ①破盾·亮 Block_Hit_High @0 L0.01 G0.5 +2层 ②破盾·闷 Block_Hit_Low @0 L0.06 G0.45 +1层 ③障壁受击 barricade_hurt @0.8→1.3 L0.07 G1.6 |
| 34 | trap_erupt | 刀刃陷阱·牌堆炸出刀 | 4 | | 2 | ①匕首喷射 dagger_spray_rr1 @0→0.55 L0.05 G0.45 cut0.1 ②匕首喷射+闷响 rr3 @0→0.5 L0.2 G0.4 cut0.1 +1层 ③刀扇 fanofknives @0.35→1 L0.16 G0.4 in0.02 |
| 35 | trap_final | 刀刃陷阱·最后一下大爆炸 | 4 | | 0 | ①炸弹+飞刀 magiknight_bomb @1.42→2.4 L0.07 G0.75 in0.01 +1层 ②手雷 the_kin_grenade @0.95→2.3 L0.08 G0.9 in0.01 ③毒蟾爆+碎盾 SpinyToad-Explode @0.55→1.3 L0.15 G0.8 in0.01 +1层 |
| 36 | pinpoint_shut | 精密瞄准·刻度合拢 | 4 | | 2 | ①勾选 settings_check @0 L0.04 G0.9 ②锁定 Block_Gain_Hit @0.22 L0.05 G0.35 ③遗物轻碰 DropRelic_Clink @0 L0.04 G0.5 ④时间线点击 timeline_click @0→0.5 L0.01 G1.5 |
| 37 | planner_write | 谋划专家·羽毛笔写字 | 4 | | 3 | ①地图画线 map_draw_loop @5→5.95 G1.6 in0.03 cut0.06 ②地图画线·二 @9.4→10.35 同 ①③地图画线·快 @2→3.2 ×1.3 ④擦地图 map_Erase @0 G0.6 ×0.5 |
| 38 | wraith_scream | 幽魂·尖啸（鬼船·吼） | — | Fixed+Settled | 0 | 鬼船·吼 ship_die_roar_rr2 @0.3 L0.41 G1.25 ×0.84 hall +2层(rr3@after0.4、rr2@after1.9) |
| 39 | pool | 腐蚀波·毒池 | — | Fixed+Settled Every=6 | 0 | ①泡1..④泡4 = Pop(k=1..4)：bubbles @1.715→1.8/2.11→2.215/1.245→1.33/2.5→2.62，G1.6/3.6/1.2/7，in0.012 cut0.03 ⑤噼啪…波儿 Pop3 +2层 ⑥噼啪啪…波 Pop1 +3层 ⑦波…波儿 Pop2 +1层 |
| 40 | storm_wind | 钢铁风暴·风（蒸汽） | — | Bed+Fixed+Settled | 0 | 蒸汽 waterfall_steam @1→29 G0.9 ×0.8 |
| 41 | storm_gust | 钢铁风暴·放出（刀扇） | — | Fixed+Settled | 0 | 刀扇 fanofknives @0.23 L0.28 G0.45 |

设计模式读法：
- **「刃+沉」双层结构**反复出现（murder_cut 三候选全部 Plus 一个低沉层），对应 tuning.ini:943 注释「a geyser (outbreak_erupt), its loud point on the instant the jets come up」——叠加层用 `After` 精确排布响点。
- **hall/low 分工**（tuning.ini:982-987）：hall=长混响给「该响下去的」（尖叫、待命、谋杀刀切），low=低通给「谋杀的低鸣」。代码里 `.Hall()`/`.Low()` 只是设置 `Via` 字段（:36-46），总线在 Bus() 懒创建（:1701-1723）。
- **Default=-1 = 不配**：suppress_bite、malaise_drink 默认静音（用户要在面板选）。
- **样本跨槽复用**：`sts2_sfx_silent_attack_blade_rr1_v4` 在 murder_edge/murder_cut 用不同区间+增益当成两个音；`STS2_SFX_Block_Break_Low_v1` 出现在 8 个槽。**一个样本多次复用是零资产策略的核心手法。**

---

## ③ 播放编排 API（SilentFX.Core.Sfx.cs）

### 3.1 入口 API 与调用量（全库 grep 统计）

| API | 行号 | 调用量 | 行为 |
|---|---|---|---|
| `Play(name,vol,delay,pitch,vary,evenFrozen)` | :1211 | 32 | vary>0 时 pitch ±vary 随机；delay>0.004 进延迟队列 Queue；否则 `TimeFreeze.Defer` 包裹 Now（子弹时间是否让它响由调用方决定） |
| `PlayAt(name,wait,vol,evenFrozen)` | :1206 | 10 | = `Play(wait - Lead(name))`，用 Option.Lead/Speed 折算提前量，让「大声点」落在 wait |
| `One(name,wait,vol)` | :1253 | 1 | 随机选候选 + 防连续重复（LastOne，:1262-1266）+ pitch ±3% |
| `Hold(name,level,pitch,life)` | :1877 | 9 | Bed/Scatter 持续音；Every>0 走 Scatter，否则 Bed |
| `Hush(name,fade)` | :1931 | 2 | Bed 目标音量归零、缩短淡出 |
| `Stop(name,fade)` | :1297 | 0（定义未用） | 清队列 + 淡出所有同名语音 |
| `Audition(name)` | :1068 | 1 | 面板试听（Bed 试听 3 秒，:1084） |
| `Choose(name,index)` | :1019 | 2 | 面板选音：写 Picked → 试听 → 返回中文结果串 |
| `Lead(name)` | :1189 | 8 | 查询提前量（外部动画对齐用） |
| `Nudge(name,factor)` | :1098 | — | 面板「+/-」按倍数调 sfx.<name>，clamp 0.1~8 |

### 3.2 内部播放链

```
Play/One/PlayAt
  └─ Queue(Later) 延迟队列（_clock 走 Engine.TimeScale 缩放的游戏时间，:2170-2173）
      └─ Now(name,volume,pitch) (:1434)
           ├─ OwnFile(name)：sfx/<name>.ogg/.wav 用户覆盖优先（:1757-1799）
           ├─ Piece(name)：choices.ini → Default 选出 Option（:1009-1017）
           ├─ StreamOf → GameSounds.Get(bank, sample)（:1597-1604）
           └─ Start(name, stream, option, level, pitch) (:1615)
                ├─ Free() 取语音（:1825）：优先空闲的；满 28 抢最老的；否则 MakeVoice
                ├─ PitchScale = clamp(pitch × Speed, 0.25, 4)
                ├─ VolumeLinear = clamp(level × Gain × PlayerSettings.AudioVolume, 0, 8)
                ├─ Bus(Via)：SilentFX_Out→Hall/Low（:1658-1732）
                ├─ Soft>0.004 → Tween 淡入，否则直排（:1635-1645）
                ├─ Play(From)；StopAt = _real + (To-From)/pitch（:1646-1649）
                └─ With[]：After≤0 同帧 Layer()，否则进 Queue（:1472-1499）
```

关键工程细节：
- **同名 40ms 防抖**：LastPlayed，防止一帧内重复触发（:1446-1449）。
- **Outlives 豁免**：`hunt_trophy`（战利品）不随 Reset 停止（:783, 1405-1408）。
- **音量四级串联**：调用方 level × `Knob("sfx."+name)`（面板/ini）× Option.Gain × 全局 AudioVolume → VolumeLinear → 总线 `AudioEffectHardLimiter`（Ceiling=-1.5dB，Release=0.15s，:1694-1699）——「无论叠多少大声部都不越过」（tuning.ini:988-990）。
- **语音池**：预热只建 8 个（Init :928-937），运行时最多 28（MaxVoices :763）。满时抢 `Since` 最老的（:1851-1855）。
- **总线创建**：`Room()`（:1734-1755）在 Godot AudioServer 里 AddBus + SetBusSend + AddBusEffect；失败一次则 `_noRooms=true` 永久降级直连 SFX/Master（:1726-1731）。日志：「Sfx: no buses of its own (...) - everything goes straight to ...」。
- **settings 联动**：`PlayerSettings.Changed` 时全淡出（0.12s）或按 AudioVolume 重排（SettingsChanged :1322-1363）。

### 3.3 Bed A/B 交叉淡入淡出（Weave :2088-2131）

```
每个 Bed 槽 = BedState{两个 AudioStreamPlayer A/B, 同一 Stream}
每帧 Weave(bed, dt)：
  1. Level 朝 Target MoveToward（升用 FadeIn，降用 FadeOut，:2092）
  2. 归零且不再 Hold → Silence（QueueFree A/B）
  3. Arm() 确保 A/B 已建（流 = 用户文件或 Piece 的 GameSounds 流）
  4. 相位推进：Turn += dt × PitchScale；phaseA = frac(Turn/span)；phaseB = (phaseA+0.5)%1
  5. Pass(A/B)：phase 回绕或未播 → Play(from + phase×span)
     VolumeLinear = loud × sin²(π×phase)   ← 正弦平方包络，两头自然为零
```

**A/B 各占半个周期、错相 0.5**：A 响到头时 B 从零升起，接缝无痕。sin² 包络比线性交叉淡化更「呼吸」。span = To-From（Bed 候选普遍给了长 To，如 murder_drone 32s/35s、storm_wind 29s——把游戏环境音样本当无缝循环用）。待确认：若样本本身首尾不齐，接缝处可能有可闻接点（作者未做 crossfade of waveform，只做了 gain 包络）。

### 3.4 Scatter 随机 sprinkles（:1941-2005）

`Hold(name, level)` 建 Scatter{Next=_real+first}；Update 里到点 Sprinkle：随机选候选（防连续重复 :1948-1951）→ level×0.7~1.0 随机 × pitch±8% → Start + With 层；下一声间隔 `every × (0.7 + 0.7×rand)`（:2245-2246）。对应 pool 槽「约每 3 秒一个气泡，四成概率来一小段短语」（tuning.ini:977-981）。

---

## ④ FSB 拆解全链路（GameSounds.cs，503 行）

### 4.1 为什么需要这套东西

mod 不携带音频（README.txt:7：「本模组不携带《杀戮尖塔2》的音频。附加音效从玩家已安装的游戏音频库中按需读取」）。游戏的音效在 FMOD 的 `.bank` 文件里，样本是 **Vorbis 编码但每个包前带 2 字节长度前缀**的 FSB5 私有格式，且 Vorbis 的 codebook/setup 信息不在样本里、存在 fmod.dll 内部。Godot 只吃标准 Ogg Vorbis。所以要做三件事：解析索引、挖 setup、重包容器。

### 4.2 FSB5 索引解析（Open :142-274）

```
res://banks/desktop/<name>.bank
 1. 扫描 "FSB5" 魔数：步进 1048573 字节、每窗 1MB、最多 64MB（:168-184）
 2. FSB5 头（从魔数起 64 字节）：
    +4  version（=1 则样本头 60 字节否则 64，:201）
    +8  numSamples（1..100000）
    +12 sampleHeaders 长度 / +16 nameTable 长度
    +20 样本数据总长 / +24 codec（必须 =15 Vorbis，否则抛「the samples are not Vorbis」）
 3. bank.Data = 魔数偏移 + 头长 + sampleHeaders + nameTable（样本数据区起点）
 4. 样本头：每条 8 字节定长 + 可选扩展块（:215-247）
    bit0   有扩展块
    bits1-4 采样率索引（表：8000/11000/11025/16000/22050/24000/32000/44100/48000，:210）
    bit5   声道数-1（+1）
    bits6-33  数据偏移 ×16
    bits34-63 帧数
    扩展块循环：type/size；type1=声道覆盖、type2=率覆盖、type11=CRC32
 5. 名字表：uint32 偏移数组 → UTF8 字符串；样本 Length = 下一offset − 本offset（:249-259）
 6. 日志：「GameSounds: <name>.bank - N samples of the game's own」
```

率索引表是 FSB5 标准表；偏移 ×16、扩展块 type 编号均为 FSB5 规范，实现与规范一致。

### 4.3 Vorbis setup 挖掘（Setup/FromFmod :276-369）

FSB5 Vorbis 样本只存「音频包」，解码必需的 identification/comment/setup 三头不在文件里——FMOD 把它们按 CRC32 存在 fmod.dll 的只读数据段。挖法：

```
1. 读游戏可执行文件同目录的 fmod.dll 全镜像（一次性，_fmodImage）
2. 解析 PE：e_lfanew@60 → NT头 → 节数@+6、可选头大小@+20、ImageBase@+48
   → DataDirectory 循环建 (VA, Size, RawPtr) 映射表（:319-328）
3. 在镜像里搜 CRC 的小端 4 字节（从 12 起，:330）
4. 每个候选命中读三个字段（j-12, j-4, j+4, j+12, j+16）：
   RVA→文件Offset 换算后做边界校验（:341）
5. 拼 header(Size) + 尾部(Offset..+Len)，Crc32(拼好的) == crc 才算命中（:346）
   —— CRC32 即 zip 多项式 0xEDB88320（:371-393）
6. 返回的字节即 Vorbis setup 头（codebook/time/floor 等）
```

失败文案：「no fmod.dll next to the game (...)」「fmod.dll has no Vorbis setup {crc:X8}」「the game's samples cannot be decoded here - ...」。**这是整个零资产方案里最「黑」的一步**：靠 CRC 指纹在 PE 里反查 FMOD 内置的 Vorbis 头，绕过了 FSB5 不公开的 setup 存储格式。

### 4.4 Ogg 重包（Page/ToOgg :418-505）

```
Page(output, seq, granule, flags, packets...)：
  段表：每包按 255 分块（lacing 值），头 27 字节 + 段表 + 包体
  "OggS" + version0 + type(flags) + granule(8B) + serial=1 + seq + CRC(4B)
  CRC 用 Ogg 多项式 0x4C11DB7（初始 0，非取反，OggCrc :395-416）

ToOgg(sample, setup, data)：
  首页(BOS, granule=0)：identification 头 30 字节
      01 + "vorbis" + ver0 + channels + rate + blocksize(184,1)
  次页：comment 头 20 字节（3 + "vorbis" + "fsb5" 厂商串）+ setup 头（挖来的）
  数据包切分：FSB 里每包前 2 字节小端长度前缀（:479-489）
  每包一页：granule = (前包块数+本包块数)/4，块数按包头 bit1 判 2048/256（:495-499）
  末页 EOS granule = s.Frames（精确总帧数，来自索引）
  → AudioStreamOggVorbis.LoadFromBuffer(bytes)
```

「fsb5」写进 comment 头厂商字段——作者留的记号，也方便将来调试识别来源。

### 4.5 自警与缓存（:50-96）

- **冷切计时**：每次 Get 计时，>8ms → `GameSounds: <key> took X ms to cut`；若 `GameRefs.CombatRoom != null` → `Cold sound during combat: ..., X ms; prewarm did not cover this stream`（:69-76）。这就是「预热预算」的验证回路——Sfx 侧 AssetWarmup 按「sound/<bank>/<sample>」逐样本预热（QueuePiece :1526-1547）。
- **样本改名一次性 Warn**：Told HashSet + Tell()（:90-96），游戏更新改样本名只吵一次。
- 缓存：Banks（打开的银行）/ Streams（bank/sample → AudioStream）/ Setups（crc → setup 头）三个字典，全部 OrdinalIgnoreCase。

---

## ⑤ FMOD 挂钩细节（FmodNative.cs，1039 行）

### 5.1 要解决什么问题

Mood 要处理的是**游戏自己的 FMOD 音乐**（总线 bus:/master/music）。mod 是 C# 跑在 Godot 里，游戏音频走 Godot 的 FMOD 桥（GodotFmod*.dll → fmodstudio.dll）。拿 FMOD Studio System 句柄的正路没有公开 API，作者选择了**导入表钩子**：让游戏每次调 `Studio::System::update` 时先经过 mod，从而偷到 `this` 指针。

### 5.2 Hook() 全流程（:535-641）

```
前置条件（HookModulesReady :687-708）：进程里同时有 fmodstudio.dll 和 GodotFmod*.dll，
   否则返回 Pending 等待（WarmNeutral 的轮询机制）。
1. NativeLibrary.SetDllImportResolver(本程序集, Resolve)：Resolve 只对 "fmod"/"fmodstudio"
   放行，遍历进程模块返回基址（:519-533）——让 DllImport 能找到游戏已加载的原生 DLL。
2. 找 fmodstudio.dll 基址 + GodotFmod 模块基址/大小（:546-559）。
3. NativeLibrary.TryGetExport(fmodstudio, "?update@System@Studio@FMOD@@QEAA?AW4FMOD_RESULT@@XZ")
   拿 Studio::System::update 真实地址（C++ 修饰名，:20）。
4. 解析 GodotFmod 的 PE 导入表（:572-616）：
   e_lfanew@60 → 校验 'PE\0\0'(17744) → Machine==523(AMD64) → 导入表 RVA
   （OptionalHeader+24+112+8 = DataDirectory[1]）→ 边界校验
   遍历 IMAGE_IMPORT_DESCRIPTOR（20 字节/项，FirstThunk RVA@+16）：
   在 IAT 里逐项找 == update 地址的槽 → _slot（该 IAT 槽的绝对地址）
5. 保存链：_next = 原 update 的委托；_hook = OnUpdate；写 _hook 的函数指针到 _slot。
   Write()（:643-652）：VirtualProtect(slot, 8, PAGE_EXECUTE_READWRITE=4) → WriteIntPtr → 恢复旧保护。
6. OnUpdate(self)（:668-676）：_system 未记且 self≠0 → 记 _system=self（即 Studio System 对象），
   然后 return _next(self) 原样透传。钩子只偷看一眼，不改行为。
```

失败分级（Fail :271-310）：
- `_optionalWarmAttempt`（预热路径）失败 → 只 Warn「neutral prewarm unavailable - ...; leaving ordinary Mood attach available」，**不杀常规 Mood 冷接路径**；
- 否则 `_dead=true`，Detach+Unhook，Mood 整体关闭（日志 `FmodNative: off - ...`）。
- 3 秒等不到 update：`Fail("the plug-in never called update through its import table")`（:347-350）。
- Unhook 写回失败仅 Warn：「it keeps calling through us, which is harmless」——钩子是纯转发，写不回去也不出事（:660-663）。

各失败态中文状态串（Status :100-122）：「正在取句柄」「待命」「已接上音乐总线」「没接上：<原因>」。

### 5.3 Attach()：挂 DSP 到音乐总线（AttachCore :833-1041）

```
1. FMOD_Studio_System_IsValid(system) 校验偷来的句柄
2. GetCoreSystem → GetVersion（日志：FMOD 2.0x.xx build N）
3. GetBus(system, "bus:/master/music\0") → LockChannelGroup → FlushCommands → GetChannelGroup
   （锁定 channel group 防止游戏加载/卸载时指针失效）
4. 建 DSP（:868-911）：
   先按类型枚举 {18,20,17,19,32,36}（FMOD_DSP_TYPE：lowpass simple/multiband eq/…）创建，
   按 NameOf(dsp) 归一化匹配："lowpasssimple" / "multibandeq" / "reverb"（排除 convolution）；
   找不到就再扫类型 3..42 全枚举（:873-903）。低通缺失时用 Multiband EQ 顶替（_lowIsEq）。
5. 参数索引反射定位（Parameter :721-751）：
   遍历 DSP 参数描述（结构体+4 处 16 字节 ASCII 名）关键词匹配：
   低通 "Cutoff"/"Freq"；EQ "A"+"Filter"（滤波器类型）与 "A"+"Freq"；混响 "Wet"/"Dry"/"Decay"/"High"+"Cut"。
   ——不写死参数序号，FMOD 版本换了参数顺序也能活。
6. 中立态初始化（:955-998）：两个 DSP 全 SetBypass(1)；lowpass cutoff=20000Hz；
   EQ 模式置 2（LOWPASS）；reverb wet=-80dB、dry=0、decay=mood.room_decay×1000、highcut=mood.room_highcut。
7. AddDSP(group, -1, low) + AddDSP(group, -1, verb) 挂到音乐 channel group 尾部。
8. metering：AllocHGlobal(272)×2 + SetMeteringEnabled(low,1,1)。
   Rms()（:443-464）按 FMOD_DSP_METERING_INFO 布局裸读：numchannels@+260、每通道 rms@+4+128+4i。
```

日志留证：`FmodNative: FMOD 2.02.xx (build N); on bus:/master/music: low-pass + reverb` 或 `multiband EQ as low-pass + reverb`。

### 5.4 Set/Meter/Detach

- `Set(cutoff, wet)`（:374-418）：cutoff≥19500 → bypass 低通；wet≤-59 → bypass 混响；带 1%/0.2dB 死区防每帧重设。cutoff clamp 20~22000，wet clamp -80~6。
- `Meter()`（:420-441）：返回滤波器输入/输出 RMS（dB），供面板显示「X→Y dB」。
- `Detach()`（:466-517）：RemoveDSP → DSP_Release → UnlockChannelGroup，有序拆卸。
- `Want(dt)`（:312-372）：Mood 每帧调用；已挂接则 IsValid 双校验（system/bus 失效就整体重来，日志「the sound system changed - starting over」）；未拿句柄则驱动 Hook 等待计时；拿到就先 Unhook（钩子寿命只到拿句柄为止）再 Attach。

### 5.5 WraithAudioWarmup（137 行）：预热挂接

幽魂出场前就把中立 DSP 链挂好并保持 bypass（Step :90-113 → WarmNeutral :194-260 → Attach 后 Unhook），Mood 首次 Set 时零 attach 开销。守卫生效条件：`PlayerSettings.Enabled && PrewarmEnabled && MusicEnabled && 非战斗 && 树未暂停`（CanRun :69-76）。三态结局：「neutral chain already attached」「unavailable; ordinary Mood cold-attach fallback remains enabled」「neutral DSP chain attached and kept bypassed」。**预热失败永远留常规冷接的后路**——与整库「失败降级不崩溃」一致。

---

## ⑥ Mood 音乐闪避机制（Mood.cs，419 行）

### 6.1 Lease 字典模型（:6-21, 121-155）

`Lease{Volume, Pitch, Muffle, Room, Attack, Release, Until}` 按名字存字典。多个特效同时想改音乐时**租约叠加**（Update :216-223）：

| 参数 | 聚合方式 | 理由 |
|---|---|---|
| Volume / Pitch | **取 min** | 最压制的那个赢（都别想大声） |
| Muffle / Room | **取 max** | 最闷最空的赢 |
| Attack | 取 min | 最快的那个生效时间赢 |
| Release | `_release`（Clear 时单设，:161） | 最后松手那个的释放时间 |

`Hold(key, volume, pitch, muffle, room, attack, release, life)`：life 是租约续期（Until = now + max(0.05, life)），过期自动 Clear。音量/音高 clamp [0,1]/[0.25,1]。

### 6.2 阻尼趋近（Toward :251-265）

目标变化时算速率 `rate = |target-now| / max(0.01, attack或release)`，然后每帧 `MoveToward(now, target, dt×rate)`——**线性斜坡**而非指数，保证 attack/release 秒数精确兑现。方向判定 `deeperIsLower`：volume/pitch 是「往下走=变深=用 attack」，muffle/room 是「往上走=变深=用 attack」（:261）。

全部回中性且无租约 → 停 Ticker（:229-240），零常驻开销。

### 6.3 双通道下发（Apply :319-395）

**通道 A：游戏音乐事件**（不经 FMOD C API，走 Godot 对象反射）：
- `NRun.Instance.RunMusicController` → 子节点 "Proxy" → 字段 `_musicEv` / `_ambienceEv`（:280-286）
- 有 `is_valid` 方法就先问一句；然后 `Call("set_volume", _volume)` / `Call("set_pitch", _pitch)`（:302-351）
- 事件实例 id 变了（换曲）就重置 _sent 缓存，立刻重发一次
- 环境音单独处理：`ambience = lerp(1, _volume, mood.ambience)`——mood.ambience=1 时环境音跟音乐一样被压，0 时不动（:330-331）
- 死区 0.003/0.002 防抖

**通道 B：FMOD 总线 DSP**（经 FmodNative）：
- `cutoff = 20000 × (cutoff_min/20000)^muffle`（:374）——muffle=0 时 20000Hz 全通，1 时 cutoff_min（420Hz），**指数插值**比线性更贴近听感
- `wet = lerp(-34, room_max, room)`（:375）——room=0 时 -34dB（基本听不出），room_max=-5dB
- `FmodNative.Want(dt)` 确保挂接后 `Set(cutoff, wet)`
- `mood.dsp=0` 时整条 DSP 通道关闭，只剩「变更轻变慢」

### 6.4 四套配方（调用点 × tuning.ini:1002-1026）

| 场景 | 租约键 | volume | pitch | far(闷罩) | hollow(房间) | in | back | 特殊 |
|---|---|---|---|---|---|---|---|---|
| 谋杀瞄准 | murder | 0.55 | 1 | 0.55 | 0.65 | 0.6 | 0.45 | 定格瞬间追加 murder_stop：volume 0.06、attack 0.03（:MurderFx.cs） |
| 子弹时间 | bullet | 0.6 | 0.6 | 0.8 | 0.65 | 0.55 | 0.8 | TimeFreeze 持 0.3s 续租 |
| 幽魂形态 | wraith | 0.3 | 1 | 0.8 | 0.65 | — | 0.9 | 出牌到落地 |
| 刺杀瞄准 | assassinate | 0.75 | 1 | 0.3 | 0.2 | 0.5 | 0.5 | 「a little - less than Murder」 |

代码兜底值与 ini 值有差异（如 murder.hollow 代码兜底 0.35 vs ini 0.65；wraith.volume 兜底 0.45 vs ini 0.3）——`Sfx.Knob` 先查面板 Knobs 再 `Tuning.F(key, def)` 读 ini，ini 缺键时才用兜底（:946-969）。**ini 是官方混音单，代码兜底是「ini 被删也能跑」的保险。**

设计意图（tuning.ini:991-993 英文注释）：「"Feel": what some cards do to the GAME'S OWN music while they have the stage (**nothing is added to it**)」——不往音乐里加东西，只把它推远、压闷、放慢。这是「借景」式音频设计。

---

## ⑦ 程序化合成公式

### 7.1 Riffle 洗牌（:855-879）

固定 `Random(seed)` 确定性生成一条「洗牌」复合音（ticks-1 个纸牌堆叠声 stack + slides 个滑动声 + 1 个合上书声），全部来自游戏 `sts2_sfx_cardmvmt_stack_v1_rr1..10` / `slide_v1_rr1..10` / `ui_deck_close` 样本：

```
第 i 个 stack 点击（i = 1..ticks-1）：
  t = i/(ticks-1)
  时刻 after = span × t^bend + (rand-0.5) × span/ticks × 0.5      ← 幂律间隔 + 抖动
  bend=1 均匀；bend=0.6 前密后疏（「由慢到快」）
  增益 = tickGain × (0.65 + 0.35·rand) × (0.75 + 0.25·sin(π·t))   ← 钟形：头尾轻、中间响
  速度 = speed × (0.92 + 0.16·rand)
  长度 0.16s，样本 = Stack(rand 1..10)
滑声（slides 个，slideAt≥0 时）：
  第 j 个：after = slideAt + 0.13j，gain = slideGain × (1-0.15j)，speed × (1+0.06j)
合上书声（tapAt≥0 时）：
  ui_deck_close_v1_layer1_rr(1..4) @0.03s，gain 0.22
返回：主 Option（第一个 stack）+ With[] 全部层
```

四个预设即四种牌感（:362-365）：细密(11,16,span0.42)、由慢到快(23,14,bend0.6)、厚牌慢洗(37,10,×0.78)、两手对洗(41,12,slides2,×1.05)。

### 7.2 Pop 气泡（:891-897 + 查表 :186-190）

同一 `haunted_ship_cast_bubbles` 样本里手工量出**四个气泡音素**的区间：

| k | From | To | Gain |
|---|---|---|---|
| 1 | 1.715 | 1.8 | 1.6 |
| 2 | 2.11 | 2.215 | 3.6 |
| 3 | 1.245 | 1.33 | 1.2 |
| 4 | 2.5 | 2.62 | 7 |

统一 in0.012 淡入 + cut0.03 截尾（短促的「波儿」）。候选 5-7 是**乐句**：主 Pop + 后续 Pop 按 after(0.07~0.62s) 排成「两三个快 pip、停顿、一个肥厚低 bloop」（tuning.ini:977-979），配合 Scatter 的 every×(0.7+0.7·rand) 随机间隔——四成概率出乐句、其余单泡。

### 7.3 方法论提炼

「程序化合成」在这里不是合成波形，而是**用参数排布既有样本**：固定种子保证可复现（choices.ini 存的是 sample@from 的 Id，:34），幂律/钟形/指数三条曲线提供听感形状，随机抖动避免机械感。整套公式没有任何音频素材成本。

---

## ⑧ 「注释等效物」编目（字符串字面量与日志文案）

作者把大量设计意图写进了运行时字符串（中英双语），等于散落的注释：

**A. 音乐/音效设计意图（tuning.ini 英中注释，直接引用）**
- 「all of it is set by ear in the test panel」——41 槽混音全凭耳朵（:920）
- 「the hall = a long reverb, for what should ring on (the scream of Wraith Form, her standby, the cuts of Murder); the low room = a low-pass, for the hum of Murder」——两个总线的分工（:982-987）
- 「it never goes above this (dB), however many loud pieces lie on top of each other」——限幅器存在理由（:988-990）
- 「"Feel": what some cards do to the GAME'S OWN music while they have the stage (**nothing is added to it**)」——Mood 的设计宪法（:991-993）
- 「The acid pool is heard now and then... about four times in ten, a little phrase (two or three quick pips, a pause, a fat low bloop)」——随机乐句的概率说明（:977-981）
- 「every card is heard as it passes in front of her (one turn of the ring = 2 pi / flech.spin = 1.96 s...)」——飞镖环的节奏换算（:970-972）
- 「Suppress, Shadow Step, Malaise, Expose, Knife Trap, Pinpoint, Master Planner - all out of the game's own samples (v0.69.1)」——v0.69 七张牌的新配音全部取自游戏样本，零新素材（:955-956）

**B. 用户可读的中文面板文案（Choose/Audition/Report）**
- 「没有这个声音」「不配」「（试听 3 秒）」「试听失败：…」（:1025,1045,1085）
- 「  注意：sfx 文件夹里有同名文件，实际用的是那个文件」「（没读到这个样本，见日志）」——用户覆盖文件优先的显式告知（:1053-1057）
- Mood.Report：「音乐 音量55% 速度100% 远55% 空65%｜滤波：已接上 -12→-9 dB」——把四个抽象量翻成中文百分比（:420）
- Status：「正在取句柄」「待命」「已接上音乐总线」「没接上：<原因>」（FmodNative.cs:100-122）

**C. choices.ini 文件头注释（WritePicks :1166）**
- 「# SilentFX - what was chosen in the test panel ("选音效"). Delete this file to go back to the defaults.」——ini 即自文档

**D. 失败与降级的自白（日志 Warn/Info）**
- 「the game's <bank> bank has no sample "<name>" (**a game update may have renamed it**)」——样本改名（GameSounds.cs:107）
- 「Cold sound during combat: ..., X ms; **prewarm did not cover this stream**」——预热没盖住就明说（:71）
- 「no FSB5 block」「the samples are not Vorbis (codec N)」「unexpected FSB5 header」——FSB 解析三连拒（:187-204）
- 「fmod.dll has no Vorbis setup {crc:X8}」——setup 挖掘失败（:302）
- 「no buses of its own (...) - everything goes straight to ...」——总线降级（Sfx.cs:1729）
- 「the plug-in never called update through its import table」——钩子超时（FmodNative.cs:349）
- 「the import table entry could not be put back (**it keeps calling through us, which is harmless**)」——连「无害」都解释（:662）
- 「multiband EQ as low-pass」——低通缺失用 EQ 顶替也如实上报（:1035）
- 「Wraith audio prewarm: unavailable; ordinary Mood cold-attach fallback remains enabled」——预热失败留后路（WraithAudioWarmup.cs:107）
- 「the sound system changed - starting over」——游戏换音频系统就重来（FmodNative.cs:334）
- 「a long frame - X ms - while the mod's sounds were going」——长帧自警（Sfx.cs:2168）
- 「fsb5」写进 Ogg comment 头厂商字段（GameSounds.cs:472）——二进制里留记号

---

## ⑨ 对移植项目的价值（龙族零 · UE4 有音频资产）

结论：**这套架构最值钱的不是代码而是四层可迁移思想**，按价值排序：

1. **「样本切片 + 参数化复用一个样本」的资产经济学**。
   mod 用 41 槽 145 候选只引用约 132 个游戏样本（大量跨槽复用，一个 blade 样本切三段当三个刀音）。龙族零若有 UE4 音频资产，最直接的借鉴是把「整段音效」拆成**切片资产**（SoundWave + 起始偏移），用 DataTable 描述 `Sample/From/To/Gain/Speed/Lead/Via/With`——一个 SoundCue 或一个 DataAsset 就能顶过去几十个 wav。UMG/动画对齐用 Lead 提前量概念（UE 里是 AnimNotify 的提前触发或 SoundCue 的 Delay 节点）。
2. **「总线分层 + 单旋钮混音 + 总线限幅」的可调音架构**。
   hall/low/Out(limiter) 三级总线对应 UE 的 Submix 链（Master Submix 挂 Limiter，两个 Submix Effect：Reverb 与 Lowpass Filter，用 Submix Send 路由）；`sfx.<name>` 每槽一个音量键对应 **SoundClass + SoundMix** 动态调音（运行时 Push/Pop SoundMix，和 mod 的 Knobs 字典一一对应）。sfx.ceiling=-1.5dB 的总限幅是防多音叠加爆音的低成本保险，UE 项目同样值得在 Master Submix 常挂。
3. **音乐闪避（Mood）的 lease + 阻尼模型可直接平移到 UE**。
   Lease 字典（多请求方按 min/max 聚合、attack/release 线性斜坡、到期自动释放）正是 UE 音频缺少的「多系统抢音乐话语权」仲裁器；UE4 落地路径：`UAudioComponent::SetVolume/Pitch`（对应 mod 的游戏事件通道）+ Submix 的 EQ/Reverb effect（对应 FmodNative.Set 通道）。far=指数插值 cutoff、hollow=lerp wet 的数值配方（cutoff=20000×(420/20000)^muffle）可以直接抄进 UE 的调音曲线。
4. **程序化排布公式（Riffle/Pop）搬到 MetaSound/SoundCue**。
   「固定种子 + 幂律间隔 + 钟形增益」在 UE 里可用 SoundCue 的 Variation（Delay 随机化 + Volume/Pitch 随机化）或 MetaSound 节点复刻；Pop 的「一样本四音素查表」就是 UE 的 SoundCue 变体池。零素材生成复合音的思路对「资产不够但节奏要对」的场景极实用。
5. **工程习惯移植（与技术无关但最省钱）**：
   - 冷加载计时告警 + 预热清单（mod 的 AssetWarmup + 「prewarm did not cover this stream」回路）→ UE 的 Audio Asset Manager 预热 + 加载计时；
   - 一切原生/外部依赖失败即降级、一次性 Warn 防刷屏、双语可观测文案 → 任何「借游戏运行时」的移植都要有的卫生习惯；
   - choices.ini「删文件即回默认」的自文档配置 → UE 的 DeveloperSettings + ini 覆盖分层。

**反面提醒**：Win32 导入表钩子（Hook/OnUpdate）只适用于「拿不到官方 API 又要控宿主音频」的 mod 场景；UE 项目自己有引擎源码和音频层，不需要也不应该照搬——UE4 里对应需求（Music 闪避）用 Audio Modulation + Submix 即可，稳定性高几个量级。

**待确认**：Bed 的 sin² 包络接缝在样本本身非无缝循环时是否可闻（作者未做波形级交叉淡化，只做了增益包络）；`Settled` 字段除 TestBench 显示外无其他消费点（疑为遗留或预留）。

---

## 附：本报告证据索引

| 主题 | 主要证据 |
|---|---|
| Option/Slot 结构 | SilentFX.Core.Sfx.cs:6-92, 815-843 |
| 41 槽全表 | SilentFX.Core.Sfx.cs:192-755 |
| Riffle/Pop 公式 | SilentFX.Core.Sfx.cs:845-897, 186-190 |
| choices.ini 读写 | SilentFX.Core.Sfx.cs:1105-1187 |
| Play/One/PlayAt/Hold/Hush/Stop | SilentFX.Core.Sfx.cs:1206-1320, 1877-1939 |
| Weave/Pass Bed 交叉淡化 | SilentFX.Core.Sfx.cs:2029-2131 |
| 总线/限幅/混响/低通 | SilentFX.Core.Sfx.cs:1658-1755 |
| FSB5 解析 | SilentFX.Core.GameSounds.cs:142-274 |
| fmod.dll setup 挖掘 | SilentFX.Core.GameSounds.cs:276-369 |
| Ogg 重包 | SilentFX.Core.GameSounds.cs:395-505 |
| 冷切告警 | SilentFX.Core.GameSounds.cs:50-96 |
| 导入表 Hook | SilentFX.Core.FmodNative.cs:519-676 |
| DSP 挂接/参数反射 | SilentFX.Core.FmodNative.cs:710-1041 |
| Set/Meter/Detach | SilentFX.Core.FmodNative.cs:374-517 |
| Mood lease/趋近/双通道 | SilentFX.Core.Mood.cs:121-395 |
| 幽魂音频预热 | SilentFX.Core.WraithAudioWarmup.cs:14-127 |
| tuning 参数 | 3817850774/tuning.ini:920-1026；sfx/README.txt:5-15 |
| 日志实证 | 3817850774/silentfx.log:8 |

*报告完 · deep-audio · 2026-10-16*
