# 言灵·镜瞳（初始遗物 + 先古版本 + 镜像 + 瞳印）

遗物源代码，不是完整 mod。放进你的 mod 工程（src/ 拷走、loc 合并、ModEntry 补两行）即可用。

`MirrorPupil.csproj` 是对着游戏 DLL（sts2/GodotSharp/0Harmony）的编译验证工程，`dotnet build` 零警告零错误；所有游戏 API 签名都过过真编译。

## 文件

| 文件 | 内容 |
|---|---|
| `src/MirrorPupilBase.cs` | 遗物共享逻辑：战斗开始给瞳印并召唤镜像、打出牌判定、玩家治疗联动、镜像状态同步 |
| `src/MirrorPupil.cs` | 基础版（瞳印 -25，下限 25），Rarity.Starter |
| `src/MirrorPupilAncient.cs` | 先古版（瞳印 -15，下限 40），保持 Starter 稀有度（与 BlackBlood 同款处理） |
| `src/MirrorImage.cs` | 镜像怪物模型：不行动、有血条、动画名与奥斯提同一套约定；GenerateAnimator 里已挂龙族镜像材质 |
| `src/MirrorMaterial.cs` | 龙族镜像战士材质移植：严格移植版（贴图反相负片，资源在 `pcksrc/effects/mirror/`） |
| `src/PupilMarkPower.cs` | 瞳印：Buff / Counter 层数 |
| `src/MirrorShatterPower.cs` | 镜为你碎：镜像版「为你而死」——原版状态 sealed 且文本写死奥斯提，按同样四条裁定自定 |
| `src/MirrorImageCmd.cs` | 召唤辅助（语义与 OstyCmd 对齐，但完全独立于奥斯提槽）+「伤害来源视为镜像」作用域 |
| `src/MirrorPupilPatches.cs` | 三个 Harmony 补丁（两个 postfix + 一个 prefix，PatchAll 自动挂上，ModEntry 不用手动调用） |
| `pcksrc/effects/mirror/` | 镜像材质资源：ling_mirror_{mix,add,mul,sub}.gdshader + 四个 .tres（打包进 mod pck，保持 `effects/mirror/` 目录层级） |
| `loc/localization/{zhs,eng}/…` | relics / powers / monsters 三张表 |

## 镜像材质（龙族移植，2026-10-05 严格定案）

镜像用《龙族》镜像战士的官方材质渲染。源链路（全部原文取值，无反解）：
cfightconsts +172 → CMaterialResCfg 104200005 → MySpineUnlitHitMaterial_Inst →
fightunitfighter.lua:1004 `IsMirrorUnit()` 时整套换材质。材质本体（热更 uasset
属性转储）：唯一标量参数 darkenParam（默认 1.0）+ SpriteTexture/NormalTexture，
材质函数 ColorDarkenFunc / CameraDirectionVector / DepthFromWorldPosition /
AngleBetweenVectors / SafeNormalize，集合 DarkenMPC——**没有 MixColor/ScanLine**
（那是 M_UI_TEST 家族的参数，旧版误借了）。编译 FS（uexp 内嵌哈希 →
shaders_index.csv → u066941 等 14 FS，见龙卡仓 render_work/mirror_shaders/）像素
输出：`out = (spec+env+max(mix(1−albedo,0,t),0))·TEXCOORD7.w + TEXCOORD7.xyz`、
`out.a = clamp(albedo.a×顶点.a)`（顶点色 rgb 不参与）。运行时值：C 侧 MID 工厂
（part_000018.c）对每只 spine 组件无条件 SetScalarParameterValue("darkenParam",
组件+0x920)，该字段无人写=0.0 → 坡道 t=0 → **全程反相负片 (1−albedo)**。
旧版的"p215 全息式"是试衣间全息预览的 shader，与战斗镜像无关（误判已废）。
2D 直译 uniform：reveal（=t，默认 0）、light_w（TEXCOORD7.w，默认 1）、
ambient_add（TEXCOORD7.xyz，默认 0）、sheen（spec/env，默认 0）。
挂点在 `MirrorImage.GenerateAnimator`（骨架此时必然就绪），`MirrorMaterial.Apply`
失败只报错不打断召唤。镜像场景 Visuals 与角色 ling.tscn 同准（(16,−21)×0.29）。
打包 mod 时把 `pcksrc/` 内容按原层级放进 pck（材质路径 `res://effects/mirror/...`）。

## 接线（ModEntry）

```csharp
// 初始遗物也要进角色的遗物池（原版 BurningBlood 就在 IroncladRelicPool 里）。
ModHelper.AddModelToPool<你的角色遗物池, MirrorPupil>();
// 先古版本由“欧洛巴的抚触”替换获得，不进池；怪物与状态类 ModelDb 自动发现，无需注册。
new Harmony("<你的mod id>").PatchAll(typeof(ModEntry).Assembly);
```

初始遗物怎么成为“角色的初始遗物”属于 mod 工程自己的事（换掉角色 StarterRelic 或开局 RelicCmd.Obtain），这里只交付遗物本体。

## 机制对照

- **每场战斗开始**：施加 100 层瞳印（`PupilMarkPower`，Counter），镜像生命 > 0 时召唤镜像（`BeforeCombatStart`，与 BoundPhylactery 同款时机）。
- **镜像初始生命/上限**：获得遗物时算一次 `角色 StartingHp × 30%`（向上取整），存进 `[SavedProperty]`；之后**只在镜像变动时被镜像自己同步**——生命/上限每次变动都写回遗物，随存档自动持久化，下一场战斗从遗物取值召唤；生命 ≤ 0 则本场不召唤。
- **镜像召唤 1**：活着 → 生命/上限各 +1（`GainMaxHp` 自带等量回复）；死亡 → 以 1/1 复活（与 OstyCmd 的召唤即复活语义一致，尸体靠「镜为你碎」留在场上）。
- **打出牌判定**：`AfterCardPlayed`，只认本遗物主人的牌；`Rng.Niche.NextInt(1, 101)` ≤ 瞳印层数才算成功。
- **再次打出攻击牌**：`CardCmd.AutoPlay`（免费的真正打出，与附魔/重放计数是两套互不相干的系统）。
- **伤害来源视为镜像**：作用域内 Harmony **prefix 整体替换** `AttackCommand.FromCard`，按 FromOsty（奥斯提攻击）同款做法转换——攻击者直接是镜像，攻击动画触发名/时长（0.3s）与音效也都是镜像自己的（`MirrorImage.AttackAnimName/AttackAnimDelay/MirrorAttackSfx`），镜像播放攻击动画。来源变了，力量等按来源结算的增益不再吃玩家的，镜像自己的状态生效。
- **瞳印减少**：`max(下限, 当前 - 减量)`，经 `PowerCmd.ModifyAmount` 走正常状态变动管线。
- **角色治疗联动**：`AfterCurrentHpChanged` 过滤“是玩家且 delta > 0”，回复一半（向上取整）。镜像在场优先回镜像；不在场（一般是战斗外）直接在遗物状态上记账、不超过上限（镜像已死则上限为 0，自然不动）。镜像自己的回复不会再触发（生物对不上）。
- **镜像死亡**：`Died` 事件把遗物状态归零（生命与上限都清 0）——战斗开始不再召唤、战斗外记账也不再生效；之后若有“召唤 1”复活，生命变动事件会把遗物状态重新同步成 1/1。
- **先古升级**：两个 postfix。`TouchOfOrobas.GetUpgradedStarterRelic` 把言灵·镜瞳指到先古版（只认基础版，已是先古版再遇该选项走原版路径换头环，与 BlackBlood 一致）；`RelicCmd.Replace` 在替换完成后把镜像的生命/上限从旧遗物搬进新遗物（AfterObtained 只取 Id 再造新实例，必须在 Replace 上搬）。
- **与奥斯提的关系**：镜像走 `PlayerCmd.AddPet`（宠物通道、吃“玩家宠物”目标档），但 `Player.Osty` 只认 `Monster is Osty`——两边的召唤互不共享、互不可见。替主承伤用的是自定的「镜为你碎」（`MirrorShatterPower`；原版「为你而死」sealed 且文本写死奥斯提）。
- **防递归**：按你的决定不做。重放的牌会再次判定，但每成功一次瞳印掉一档，链子自然衰减。

## 留白（接入 mod 时要补的）

1. **美术与音频**：镜像的 spine 资源（clip 名对齐 `idle_loop/attack/hurt/die/dead_loop`，攻击动画触发名 `Attack`）与图标（`images/powers/pupil_mark.png`、遗物图集条目）；攻击音效填 `MirrorImage.MirrorAttackSfx`（FMOD 路径，null 则不出声）。缺省时游戏按缺图处理，逻辑不受影响。
2. **描述里的数字是写死的**（loc 无动态变量）：改 `ProcDecrement/ProcFloor/StartingMarks` 常量时记得同步 loc 文本。
3. 若要调镜像 30% 比例或“召唤 1 复活为 1/1”等设计点，都集中在 `MirrorPupilBase` 的虚属性与 `MirrorImageCmd`，改一处即可。
