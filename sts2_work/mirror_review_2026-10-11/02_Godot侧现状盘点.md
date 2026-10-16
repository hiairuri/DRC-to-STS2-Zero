# 02 · 镜像（MirrorImage）STS2/Godot 侧实现盘点

> 生成：2026-10-11 ｜ 作者：godot-auditor ｜ 本轮**只读盘点**（未改动任何工程文件，仅本报告落盘）
> 取证口径：全部来自源码 / 资产 / 构建脚本 / 包内实测，**无截图反解**（铁律 1）。
> 对照件：`01_UE4与C层调研.md`（本轮并行，尚未生成 → 第 ③ 章留"待对照"占位，本侧事实先钉死）。

---

## ① 当前生效链路（一句话）

**镜像的观感 = 一条没有运行时写入的纯静态 shader**：召唤流程
`MirrorPupilBase.BeforeCombatStart → MirrorImageCmd.SummonFull/SummonOne → PlayerCmd.AddPet<MirrorImage>()`
→ 游戏按 `MonsterModel.VisualsPath`（`decomp/.../MonsterModel.cs:74` = `creature_visuals/<Id小写>`）实例化
`res://scenes/creature_visuals/mirror_image.tscn`（怪物 id `MIRROR_IMAGE`，见 `MirrorPupil/loc/localization/zhs/monsters.json:2`）
→ `MirrorImage.GenerateAnimator` 里 **唯一一次**调 `MirrorMaterial.Apply`（`MirrorImage.cs:86`），把
`res://effects/mirror/ling_mirror_material_{mix,add,mul,sub}.tres` 四份 ShaderMaterial 塞进 SpineSprite 的
`normal/additive/multiply/screen_material` 四个槽 → 生效公式只有一个有效分支（骨架 50 槽全为 `normal`，
见 §2.5）：

```
rgb = ambient_add + light_w * ((1 − base.rgb) + sheen)   // ambient_add=0, light_w=1.0, sheen=0
a   = clamp(base.a * v_col.a, 0, 1)                       // v_col.rgb 全程不参与
```

即 **`rgb = 1 − 反相前的贴图 rgb`（线性空间取反）**。`reveal/light_w/ambient_add/sheen` 四个 uniform
**全仓库无任何 C# 写入点**（已 grep `D:\ModForge\spikes\**\*.cs` 与 `DyeFramework.cs`，零命中），
只有 gdshader 里的默认值 = 当前实际值。

**为什么偏暗/像负片（本侧可证的部分）**：
1. 公式本身就是 UE4 FS 的 `v37 = mix(1−albedo, 0, t)`，`t=0`（`reveal` 默认 0 = 运行时 darkenParam=0 的真值）→ 全画幅反相，这是**设计如此**，不是 bug；
2. 缺的是 UE4 那一串**加项**：源 FS 为 `out = (spec + env + max(v37,0))·TEXCOORD7.w + TEXCOORD7.xyz`，我方把 `spec/env → sheen=0`、`TEXCOORD7.xyz → ambient_add=0`、`TEXCOORD7.w → light_w=1.0` 全部取"2D 中性"，**真机"白色但非纯白、带高亮光影"说明这三项在真机上并非中性**（否则真机就该是纯 1−albedo 的灰蓝负片）；
3. 资产侧量化（图集 `ling/animations/characters/mirror/Chr_ling001.png`，alpha>0.9 的 149,447 像素，
   Python 实测，非目测）：不透明区平均 sRGB **=(0.652,0.629,0.608)** → 线性取反再编码回 sRGB 后均值
   **(0.705,0.757,0.773)**、p10 **(0.428,0.486,0.503)**、p90 **(0.976,0.970,0.979)**。
   ⇒ 当前输出是「**灰蓝、中间调偏亮**」，**不是**用户描述的「白色带高光」——量化缺口就在上面那三个加项上；
4. 半透明边缘（alpha 0.05–0.9，约 1.56 万像素）rgb 仅 0.20–0.26，取反后 ≈ **近白亮边**，会额外加重"负片/发白轮廓"的观感；
5. 由于 `v_col.rgb` 被丢弃，**所有 modulate 型效果对镜像 rgb 无效**：`MirrorImageCmd.cs:115-118` 的亮相
   Transparent→White 只走 alpha；`LingChar/src/HitFlash.cs:37`（过滤条件 `IsLing` 命中 `res://ling/...`，
   镜像路径 `res://ling/animations/characters/mirror/ling.json` 同样命中）的 peak=2.6 白闪在镜像上**只有 alpha 生效、rgb 无变化**。

---

## ② 实现清单

### 2.1 四族 gdshader（`D:\ModForge\spikes\MirrorPupil\pcksrc\effects\mirror\`）

| 文件 | render_mode | 行号 | 大小 |
|---|---|---|---|
| `ling_mirror_mix.gdshader` | `blend_mix` | `29-51`（头注 1-27 为源链路全文） | 3015B |
| `ling_mirror_add.gdshader` | `blend_add` | `3-4` / 片元 `16-22` | 862B |
| `ling_mirror_mul.gdshader` | `blend_mul` | `3-4` / 片元 `16-22` | 871B |
| `ling_mirror_sub.gdshader` | `blend_sub` | `3-4` / 片元 `16-22` | 870B |

**uniform 表（四族完全相同，仅 render_mode 不同）**

| uniform | 类型/范围 | 默认值 | 源码行（mix 文件） | 语义（头注溯源） | 运行时写入者 |
|---|---|---|---|---|---|
| `reveal` | float, hint_range(0,1) | **0.0** | `ling_mirror_mix.gdshader:32` | UE4 FS 坡道 `t`；0=全程反相（darkenParam=0 真值），1=反相扫除 | **无**（无任何 C# 写） |
| `light_w` | float | **1.0** | `:34` | `TEXCOORD7.w`（体积透射光乘子），我方取中性 1 | **无** |
| `ambient_add` | vec3 | **vec3(0.0)** | `:35` | `TEXCOORD7.xyz` 环境加色，我方取中性 0 | **无** |
| `sheen` | float | **0.0** | `:36` | `spec+env` 平面合成，我方取中性 0 | **无** |

**片元主公式原文**（`ling_mirror_mix.gdshader:43-51`，add/mul/sub 三份逐字相同，见各自 `:16-22`）：

```glsl
void fragment() {
	vec4 base = texture(TEXTURE, UV);
	// v37：mix(1−albedo, 0, t)
	vec3 inv = mix(vec3(1.0) - base.rgb, vec3(0.0), reveal);
	vec3 rgb = ambient_add + light_w * (inv + vec3(sheen));
	// h41：clamp(albedo.a × COLOR0.a)
	float a = clamp(base.a * v_col.a, 0.0, 1.0);
	COLOR = vec4(rgb, a);
}
```

顶点函数（`:39-41`）只做 `v_col = COLOR;`——**顶点色 rgb 不参与**（UE4 源 FS 亦不乘 COLOR0.rgb，已定案）。

### 2.2 四份 .tres（同目录）

四份结构完全一致（`ling_mirror_material_mix.tres:1-6`，其余同）：

```
[gd_resource type="ShaderMaterial" load_steps=2 format=3]
[ext_resource type="Shader" path="res://effects/mirror/ling_mirror_mix.gdshader" id="1"]
[resource]
shader = ExtResource("1")
```

**无 uid、无任何 shader_parameter 覆写** → uniform 实际值 = gdshader 默认值。
（对照：皮肤/角色用的 `ling_holo_material_*.tres` 同样无参数覆写，`config.json` 的 `shader_parameters` 也是 `{}`
——`DyeFramework.cs:513-523` 那段 uniform 写入目前是空转。）

### 2.3 四族如何被选择（"谁决定用哪个"）

- **不是**代码按骨架挑选：`MirrorMaterial.cs:21` 的 `public const string SkeletonMatch = "mirror";`
  是**死代码**（全 `D:\ModForge\spikes` 仅此一处声明，无任何引用）——历史上留下的匹配片段，当前不参与决策。
- 真正的选择者是 **SpineSprite 内部的槽位混合模式**：`MirrorMaterial.Apply` 一次把四族全塞进四个槽
  （`MirrorMaterial.cs:37-40`），运行时由每个 slot 自己的 blend mode 决定走哪一份：
  ```csharp
  sprite.Set("normal_material", mix);      // MirrorMaterial.cs:37
  sprite.Set("additive_material", add);    // :38
  sprite.Set("multiply_material", mul);    // :39
  sprite.Set("screen_material", sub);      // :40
  ```
  语义依据（类注释 `MirrorMaterial.cs:11-13`）：龙卡是"整个单位换一套材质"，STS2 的 SpineSprite 按
  blend mode 拆四族，四族全换 = 该语义的等价物（2026-10-05 定案）。
- **实测（资产侧）**：`ling/animations/characters/mirror/ling.json` 共 **50 个 slot，blend 全为 `normal`**
  （Python 解析统计，非目测）⇒ **当前只有 `mix` 家族真正被绘制**，add/mul/sub 三族是"备好但未使用"。
  ⇒ 改造时若只需改观感，**改 `ling_mirror_mix.gdshader` 一个文件即可覆盖 100% 可见像素**；
  但保持四族同步仍是必要的（骨架换皮/槽位 blend 变化时不会漏）。

### 2.4 材质何时创建 / 挂在哪个槽 / 召唤流程里有几次覆盖

| 时机 | 位置 | 行为 |
|---|---|---|
| 资源加载期（引擎） | `GD.Load<ShaderMaterial>("res://effects/mirror/ling_mirror_material_*.tres")`（`MirrorMaterial.cs:28-31`） | 从 mod pck 读 4 个 tres（每次 Apply 都重新 Load，非缓存；失败打 `[MirrorPupil] mirror materials missing from pck` 并**放弃换材质**，镜像退化为 vanilla） |
| `MirrorImage.GenerateAnimator` | `MirrorImage.cs:57` 方法体末 `MirrorMaterial.Apply(controller)`（`:86`） | **唯一一次**挂载。注释（`:83-85`）说明挂点理由：骨架此时已就绪（CreatureAnimator 构造会检查动画状态），材质属性持久 |
| 召唤亮相 | `MirrorImageCmd.cs:115-118` | 只动 **Node 的 Modulate**（Transparent→White 0.35s、delay 0.1）+ `StartReviveAnim()`，**不动材质** |
| 站位/画序 | `MirrorPupilPatches.cs:187-197 PlaceMirrorLikeOsty` | 只动 `Position` / `MoveChildSafely`（插到主人前一位 = 画在主人之后/之上）、`ToggleIsInteractable`（血条），**不动材质** |
| 战斗开始召唤 | `MirrorPupilBase.cs:104-114 BeforeCombatStart` → `MirrorImageCmd.SummonFull` | 只建单位/设 HP |
| 出牌触发召唤 | `MirrorPupilBase.cs:130-225` → `MirrorImageCmd.SummonOne` / `CreateMirror` | 同上 |

**结论：召唤流程中材质被替换/覆盖的次数 = 1（仅 GenerateAnimator 一次），此后无人再写。**
四份 tres 在 pck 中确认存在（`LingChar.pck` 条目 `effects/mirror/ling_mirror_material_{mix,add,mul,sub}.tres` 各 186B）。

### 2.5 场景对照（`LingChar/pcksrc/scenes/creature_visuals/`）

| 项 | `mirror_image.tscn` | `ling.tscn` |
|---|---|---|
| 根节点 | `[node name="Osty" type="Node2D"]`（`:6`，拷贝自奥斯提模板，名字未改） | `[node name="Silent" type="Node2D"]`（`:6`） |
| 脚本 | `res://src/Core/Nodes/Combat/NCreatureVisuals.cs`（`:3`） | 同（`:3`） |
| 骨架 | `res://ling/animations/characters/mirror/ling_skel_data.tres`（`:4`、`ling_skel_data.tres:3-4` → mirror 目录下的 atlas/json） | `res://animations/characters/ling/ling_skel_data.tres`（`:4`） |
| Visuals 变换 | `position = Vector2(16, -21)`、`scale = Vector2(0.29, 0.29)`（`:17-18`，与 ling 同准，1:1 定案） | 同（`:17-18`）+ `metadata/_edit_lock_ = true`（`:19`） |
| z 序/画序 | 场景内**未设 z_index**；画序由 `PlaceMirrorLikeOsty` 的 `MoveChildSafely(node, playerNode.GetIndex())` 运行时决定（插到主人索引位=先画=在下） | 同上（游戏侧统一） |
| 材质槽 | 场景**未写** `normal_material` 等，也无 CanvasItem `material`、无 shader 参数覆盖 | 同 |
| Bounds / CenterPos / IntentPos | `(-116,-204,116)` / `(-15,-109)` / `(0,-218)`（`:26-28`、`:35`、`:39`，**继承自 osty 宠物模板**） | `(-122,-244,122)` / `(0,-146)` / `(0,-302)`（`:25-26`、`:32`、`:36`） |
| 杂项 | 已剔除 osty 火焰 VFX 与相关 shader/贴图（构建期手术，见 §5） | — |

⇒ **场景层没有任何材质/参数覆盖**，观感 100% 由 §2.1 的 shader + §2.4 的一次挂载决定。

### 2.6 干扰排查（DyeFramework / drc_dye 是否误伤镜像）

- 探测机制：`DyeFramework.cs:428-442 FindConfigPath`——取骨架文件路径的**目录后缀由长到短**逐层试
  `res://drc_dye/profiles/<dir>/config.json`。镜像路径 `res://ling/animations/characters/mirror/ling.json`
  产生 4 个候选：`…/characters/mirror/`、`animations/characters/mirror/`、`characters/mirror/`、`mirror/`——
  **pck 内均无 config.json**（只有 `profiles/ling/animations/characters/{ling,silent}/config.json`）
  ⇒ 返回 `""` ⇒ `TryAttach` 打 `[DRCSpineDye] …: no dye config, leaving vanilla` 并 return（`DyeFramework.cs:450-455`）。
- 附加保险：即便有 config，`cfgDict["filter"]`（两个 profile 都是 `ling.json`）还要过 `skelPath.Contains(filter)`
  （`DyeFramework.cs:465-470`）。
- ⇒ **影子（silhouette）、溶解（dissolve）、闪红（hit_flash）、入场（entrance tibu）、攻击/命中粒子
  全部不会挂到镜像上**——与历史决策"mirror 无 dye profile = vanilla + MirrorMaterial"一致，√ 无第四条路径意外叠加。
- 唯一"擦边"的干扰：`LingChar/src/HitFlash.cs:37 IsLing` 按路径含 `/ling/` 过滤，镜像路径
  `res://ling/animations/characters/mirror/ling.json` **会命中**，于是 Attack/Cast/PowerUp/Hit 触发时会对镜像
  起 modulate 白闪 tween（`HitFlash.cs:48-58`，Peak 2.6 / Decay 0.13）。
  **但本 shader 丢弃 `v_col.rgb` ⇒ 该白闪对镜像 rgb 完全无效**（只可能经 alpha 影响，而 tween 目标 alpha=1 不变）
  ⇒ 目前无害；**但这是一个隐患**：一旦新方案改为"乘顶点色 rgb"，镜像会跟着一起爆白闪，需届时同步收窄 `IsLing` 过滤。

### 2.7 资源与贴图

- **镜像贴图 = 与本体同源的同一张图集页**：`ling/animations/characters/mirror/Chr_ling001.png`（250,182B），
  构建期从皮肤源树 `SKIN/ling/animations/characters/silent/Chr_ling001.png` 拷贝（`build_lingchar.py:523`），
  atlas/json 同源（`ling.atlas` 2711B / `ling.json` 739,751B，与本体 `animations/characters/ling/` 下同名文件**同一份内容**）。
  区别只有**导入产品名**（去污染 md5 重映射）：
  - 镜像：`Chr_ling001.png-12f0b1c064240f56aa207d27f38772b5.ctex`、`ling.atlas-7de98da4d93599a4a8a9870dd4632626.spatlas`、`ling.json-f9979a8ebedb8c6224a82b5fcea8fe8d.spjson`
    （= `md5("ling/animations/characters/mirror/<file>")`，与 `build_lingchar.py:518-522` 的算法一致，已复算核对通过）
  - 本体：`Chr_ling001-a8131b8e48bce8432f8494240a2ae73e.ctex`（`.import` 文件内 `source_file` 为 ling 路径）
  - 四份产品**均在包内**（实测 `LingChar.pck` 条目命中，203 条中可见）。
- **无 mask / 噪声 / 高光贴图参与镜像**：镜像目录只有 atlas 单页（`ling.atlas:1` 仅一行页名 `Chr_ling001.png`，
  `size:1024,512`、`scale:0.45`，无 pma 标记），shader 只采样 `TEXTURE` 一个采样器。
- **`render_work/mirror_shaders/`**：19 份已提取（含 `u066941_FS.glsl`——§① 源公式的出处，
  以及 `myspineunlithitmaterial{,_inst}.uasset/.uexp`），**在研究树内可用，但不在任何 pck 包内**（研究用，非运行时资产）。
- **遮罩 `ma_412_uv`**：真身在基础包（`unpacked/iguf_asset_index.json` 与 `unpacked/spinemats/cand_*.bin` 有索引命中），
  已由 `LingChar/tools/make_attack_fx_textures.py` 解码 + 拉伸归一（44×130）生成为 `fx_streak_mask.png`，
  包内路径 = `res://drc_dye/profiles/ling/animations/characters/ling/fx_streak_mask.png` + 产品
  `.godot/imported/fx_streak_mask.png-98bb2b4fa2eedfc974d8fb307d4c6b22.ctex`（实测在包，2184B）。
  **注意：它属于 ling profile 的攻击特效，与镜像无关**；若将来要给镜像加"高光/流光遮罩"，这是现成的可用遮罩源。

---

## ③ 与 UE4 真值的初步对照

> `01_UE4与C层调研.md` 尚未生成 → 以下为**本侧已钉死的事实 + 待 01 裁定的未知**，不猜参数。

| 项 | 本侧现状（可证） | 待 01 报告裁定 |
|---|---|---|
| 材质来源 | `MySpineUnlitHitMaterial_Inst`（104200005），见 mix 头注 `:4-7` | 同源，应一致 |
| 反相项 | `mix(1−albedo, 0, t)`，t=0（`reveal=0`） | `t=clamp(slope·max(h34,0),0,cap(darkenParam))`，darkenParam 运行时 0 ⇒ t=0 ✓（若 01 有新证据需推翻，改 `reveal` 默认值即可） |
| `light_w` | **1.0**（假设 TEXCOORD7.w 中性） | `TEXCOORD7.w` 真值 = ?（若为 0.x 则整体偏暗；若 >1 则整体偏亮） |
| `ambient_add` | **vec3(0)**（假设 TEXCOORD7.xyz 中性） | `TEXCOORD7.xyz` 真值 = ?（真机"白但非纯白"强烈提示该项 ≠ 0） |
| `sheen` | **0.0**（假设 spec+env 中性） | `spec + env` 在移动端 lit 半透明管线里的实际量级 = ?（"带高亮光影"提示 ≠ 0） |
| alpha | `base.a × v_col.a` | 源 `out.a = clamp(albedo.a × COLOR0.a)` ✓ 一致 |
| 顶点色 | rgb 不参与 | ✓ 一致 |
| 色彩空间 | Godot：texture() 出线性、COLOR 出线性、引擎编码（**旁证**：同项目 `ling_holo_mix.gdshader:57-59` 自带 `encode_srgb()` 且 `pipeline_encode` 默认 0 即"已接受观感"，说明管线确为线性） | UE4 移动端是否同构线性取反再编码？（若 UE4 在 sRGB 空间取反，数值差异巨大，需 01 明确） |
| 四族 | 50 槽全 normal，只用 mix | 龙卡 spine 是否有 add/mul/screen 槽？（当前无 ⇒ 三族改不改都不影响） |

**本侧量化基线（供 01 对齐时直接比数）**：镜像不透明区反相后 sRGB 均值 (0.705,0.757,0.773)、
p10 (0.428,0.486,0.503)、p90 (0.976,0.970,0.979)；原图不透明区均值 (0.652,0.629,0.608)。
若真机是"接近白 + 局部高光"，则等效需求 ≈ 把均值从 0.71~0.77 抬到 0.85+ 且保留高光尾部 —— 
**可由 `ambient_add` / `sheen` 两个 uniform 单值达成，无需改公式结构**。

---

## ④ 可改点清单（改哪个文件 / 影响哪个产物 / 是否重打包 / 风险）

| # | 改点 | 文件（绝对路径） | 影响产物 | 需重打包？ | 风险与踩坑 |
|---|---|---|---|---|---|
| A1 | **调 uniform 默认值**（推 `ambient_add` / `sheen`，必要时 `light_w`）——最小改动、最可能直接命中观感 | `D:\ModForge\spikes\MirrorPupil\pcksrc\effects\mirror\ling_mirror_mix.gdshader:35-36`（`add/mul/sub` 的 `:8-9` 同步） | `LingChar.pck` | **需要**（`python build_lingchar.py` 或仅 `repack_pck.py pcksrc`） | ① gdshader 无需 Godot 导入，改完直接重打包即可；② 四族要同步改，否则换皮后槽位 blend 变化会不一致；③ 铁律：值必须来自 01 报告的源码/资产证据，禁止"看着调" |
| A2 | 改公式结构（例如加 `max(...,0)` 钳位、加高光 mask 采样、改 `reveal` 语义） | 同上 | `LingChar.pck` | 需要 | 一旦引入第二个采样器（如 `fx_streak_mask`），要同步往 tres 里加 uniform 默认值，并把贴图打进包（走 §5 的 Godot 导入段或 `relocate_asset`） |
| A3 | 加**运行时** uniform 写入（例如受击时抬 `sheen`、随回合渐显） | `MirrorMaterial.cs`（新增 `SetShaderParameter`）/ `MirrorImage.cs:86` 附近 | `LingChar.dll` | 需要重编译 dll（`build_lingchar.py` 会 dotnet build） | `GD.Load` 每只镜像每次 Apply 都重建材质实例 ⇒ **必须持有 ShaderMaterial 引用**才能后来改参数；当前 `Apply` 不保存引用，需一并改造；别在高频循环里读非平凡 Variant（历史泄漏教训） |
| A4 | 改亮相/闪白对镜像的作用方式（让 modulate 生效 → 乘 `v_col.rgb`） | 同 A1（去掉"不乘 rgb"的约束） | `LingChar.pck` | 需要 | 与 §2.6 的 HitFlash 隐患联动：一旦乘 rgb，`HitFlash.cs:37` 的 `/ling/` 过滤会把镜像一起爆白闪，需同时收窄过滤（改 dll） |
| A5 | 改站位/画序/尺寸 | `MirrorPupilPatches.cs:187-197`（dll） / `build_lingchar.py:601-610`（tscn 覆盖+断言） | dll / pck | 各需 | `build_lingchar.py:610` 有硬断言 `position = Vector2(16, -21)` 与 `scale = Vector2(0.29, 0.29)`，改 tscn 校准必须同步改断言，否则构建直接 fail |
| A6 | 给镜像挂 dye profile（影子/溶解/闪红/入场） | 新增 `drc_dye/profiles/ling/animations/characters/mirror/config.json` 到 `DRCSpineDye/pcksrc` | `LingChar.pck` | 需要 | **与既定决策冲突**（mirror 应保持 vanilla + MirrorMaterial）；一旦挂上，`DyeFramework.cs:508-511` 会用 profile 的 holo 材质**覆盖** MirrorMaterial 的四族 ⇒ 镜像观感直接退回本体样式。**非必要不动** |

**最短路径推荐**：A1（只改 4 个 gdshader 的 uniform 默认值 + 跑 `build_lingchar.py`）→ 装游戏验证 → 不够再 A2/A3。

---

## ⑤ 需主控注意的约束冲突与装配副作用

1. **⚠ 基线 sha 与磁盘不一致（本轮实测）**
   - `D:\ModForge\spikes\LingChar\LingChar.pck`：**23,524,326 B**，sha256 `d8620ee5f1dbaa18c8063931bb8082d79cab2eccd3c86403d354e86e0c11aded`
     ——任务书给的基线 `44460388…`（22.4M）**已过期**（pck 在这些基线之后又被重建过）。
   - 已安装件 `…\Mods\LingChar\LingChar.pck` 与仓内**逐字节一致**（同 sha）⇒ 磁盘与游戏是同一份，没有"装了旧包"的问题。
   - `bin/Release/net9.0/LingChar.dll` 与安装件：116,224 B，sha256 `e42de184fc0e9ab19ff4960a1be965d338e52576eb4697d1c15ee78b0a804985`
     ——也**不是**任务书的 `3ceb18f2…`（102,912B，条目 40）。
   - `LingSilentSkin.pck`：20,210,510 B，`7f751f119db9308d92095974175cc9a21a19b9de3a0ad79c8765177d113bc1ed` ✓ **与基线一致**，未动。
   - ⇒ **改完后请以 `d8620ee5…` / `e42de184…` 为"改前基线"对账**，不要拿 `44460388…` 当回归锚点。
2. **去污染仍成立**：实测 `LingChar.pck`(203 条) ∩ `LingSilentSkin.pck`(46 条) = **0**（脚本解析目录表对账）。
   镜像相关的 26 条路径全部落在 `res://ling/animations/characters/mirror/`、`res://effects/mirror/`、
   `res://scenes/creature_visuals/mirror_image.tscn` 与 `.godot/imported/*12f0b1c0/7de98da4/f9979a8e*`，
   **无一条指向皮肤侧路径**。⚠ 唯一"看起来像皮肤路径"的地方：
   `ling/animations/characters/mirror/Chr_ling001.png.import:12` 的 `source_file="res://ling/animations/characters/silent/Chr_ling001.png"`
   ——这是 md5 产品名去污染手法的残留（`.import` 段**没有**重写 source_file，只重写了 `path=`/`dest_files=`），
   运行时 loader 只认 `path=`（产品名），`source_file` 不参与加载 ⇒ 安全，但**审阅时容易被误判为污染**，特此钉死。
3. **改构建脚本后必须跑自检**：`python D:\项目\dragonraja\_migration\toolchain_check.py`（该文件**存在**，
   与条目 40 所述"已不存在"不符——以实测为准），应 FAIL=0（允许 4 条 T2/T3 未随迁 WARN）。
4. **Mimosa 钩子**：不要用 Bash 写源码文件；本报告用 Write 工具落盘 ✓。
5. **装配副作用（改镜像相关文件的连带影响）**
   - `MirrorPupil/pcksrc/effects/mirror/` 是**整目录 copytree** 进 `pcksrc`（`build_lingchar.py:448-452`）
     ⇒ 在该目录里**新增任何文件都会自动进包**（包括临时/备份文件），需谨慎。
   - `build_lingchar.py:509-565` 每次构建**重建** mirror 骨架目录与 `ling_skel_data.tres`，
     并硬编码两个源产品名（`ling.atlas-a45053df6901f847fc14b06c889f87ab.spatlas`、
     `Chr_ling001.png-72aa7f3f768c218e0a6ca07e91af047e.ctex`，见 `:538-539`）
     ⇒ **皮肤源树换皮/重导后这两个硬编码 md5 会失效**（FileNotFound），属已知脆点。
   - `build_lingchar.py:567-610` 的 tscn 手术依赖 `TPL/scenes/creature_visuals/osty.tscn` 模板与
     `load_steps=11` 字面量（`:592`）⇒ 游戏版本升级改了 osty 模板就会断（有 `:609` 断言兜底）。
   - **不波及 `LingSilentSkin.pck`**：镜像链条（shader/tres/骨架/mirror_image.tscn/MirrorPupil 源码）
     100% 落在 LingChar 侧，皮肤 pck 不含任何 mirror/drc_dye/effects 路径（交集实测 0）。
   - 改 `MirrorPupil/src/*.cs` ⇒ 只影响 `LingChar.dll`（csproj `LingChar.csproj:19` 源码级包含
     `..\MirrorPupil\src\**\*.cs`），**不影响 pck**；改 `pcksrc/` 只影响 pck，不影响 dll。
     两者都改就两个产物都变（build 脚本一次跑完：assemble → dotnet build → pack → install，`:643-647`）。

---

## 附：本轮取证清单（可复核）

- 解析脚本口径：PCK v3，`flags@0x14 / file_base@0x18 / dir_off@0x20`，条目偏移相对 file_base（铁律：直读会带 0x70B 杂讯）。
- 包内条目实测：`effects/mirror/` 8 条（4 gdshader + 4 tres）、`ling/animations/characters/mirror/` 7 条、
  `scenes/creature_visuals/mirror_image.tscn` 1 条、`.godot/imported/` 43 条（含镜像 3 个产品）。
- 骨架槽位统计：`ling.json` 50 slots，blend 计数 `{'normal': 50}`。
- 贴图统计：alpha>0.9 像素 149,447；半透明 (0.05–0.9] 像素 ~15,608；alpha=0 像素 354,689（rgb≈0.02）。
- grep 结论：`reveal|light_w|ambient_add|sheen` 在 `D:\ModForge\spikes\**\*.cs` 与 `DyeFramework.cs` **零命中**
  ⇒ 四个 uniform 无任何运行时写入；`SkeletonMatch` 仅 1 处声明、0 处引用（死代码）。
