# 05 · 镜像材质 Phase 2a（灰加项）方案 + 实施记录

> 生成：2026-10-11 ｜ 作者：mirror-builder ｜ 输入：`01_UE4与C层调研.md`（01）、`02_Godot侧现状盘点.md`（02）、`03_镜像材质修正方案.md`（03）、`04_独立审查报告.md`（04）+ 主控 R2 取证轮（2026-10-11 晚，结论已核实，本文如实转述）
> 本文档性质：Phase 2a 设计方案 + 实施记录（实施记录节在构建对账完成后补写）
> 症状锚点：用户反馈**手游镜像饱和度目测 ≈0、亮度高**；PC Phase 1（已实装，用户 19:17-19:22 局内生效：无 SHADER ERROR、四族材质绑定成功）"依旧不符"——**亮，但保留负片互补色残偏**。

---

## 一、背景与因果链（转述主控 R2 取证轮，2026-10-11 晚）

### 1.1 PC 现状量化（Phase 1 生效后）

Phase 1 四步管线（sRGB 解码 → 线性取反 → 中性加项 → sRGB 编码）已生效，当前输出的图集统计口径（`Chr_ling001.png` alpha>0.9，149,447 px，Python 实测）：

| 指标 | 值 |
|---|---|
| mean sRGB | (0.705, 0.757, 0.773) |
| 亮度 p10/p50/p90（Rec.601） | (0.475, 0.767, 0.970) |
| 平均 HSV 饱和度 | **0.1332** |
| 饱和度>0.2 占比 | 16.7% |

⇒ 亮度已达标，但 **HSV 饱和度 0.1332** 意味着负片的互补色残偏（R 通道均值 0.705 vs B 通道 0.773，蓝正红负的残偏）仍然可见——与用户"亮但保留负片互补色残偏"的描述一致。

### 1.2 手游"洗灰"的真机制：三项单色加，而非去饱和算子

**手游链路没有去饱和算子作用于镜像**：19 份镜像 GLSL 全量 grep 实测，luma 权重只用于环境强度比，**无输出去饱和**。洗灰真机制 = 镜像 FS 独有的**三项单色加**叠在弱色度负片上：

| # | 加项 | 来源 | 量级依据 |
|---|---|---|---|
| ① | **雾散射**（TEXCOORD7.xyz，vec3） | VS u024490 雾推导链（01 §2.4a） | 线性域灰加，无雾=0，有雾=雾色×(1-FogFactor)+日照散射 |
| ② | **方向光 GGX 高光** | u066941:159-214 | 板片法线恒朝相机 ⇒ NoV≈NoH≈1 ⇒ h67²=16，高光系数 = EnvBRDF(0.0296)×0.375×16 = **0.178，全画面均匀** |
| ③ | **环境立方图 × EnvBRDF** | u066941:216-270 | ps1 采样 × EnvBRDF(0.5,NoV)，NoV=1 端 0.0296 |

**普通战斗角色 = 贴图直出**（prog4/32/215 三份运行时捕获均无雾/高光/环境项，srcs2 实读），**唯独镜像（UnlitHit 家族）有这三项** ⇒ 发白发灰是**结构必然**。

**结论：PC 缺的不是结构（Phase 1 四步管线结构正确），是加项量级。**

### 1.3 编码实验（估算依据）

线性域加常数灰后编码，同图集口径实测（本轮脚本已逐档复现，见 §2.1）：

| 总线性加 T | 平均 HSV 饱和度 | 亮度均值 |
|---|---|---|
| +0.05 | 0.102 | 0.780 |
| +0.10 | 0.087 | 0.813 |
| +0.15 | 0.076 | 0.843 |
| **+0.20** | **0.068** | **0.872** |
| +0.30 | 0.057 | 0.925 |

⇒ **加 0.15~0.30 ≈ "基本全灰 + 亮度高"**，即手游观感。

### 1.4 真值缺口与未决项（如实落账，本轮不动）

**真值缺口**（hook 后才能闭环）：`pc3_h`（方向光色/SpecularScale）、VS 雾实值、图集 internalformat。一次 hook 捕获会话（H1+H2+H5+H3）可行性高但需用户配合，本轮不执行（提案见 §6.2）。

**未决项两条**（本轮不动，落账即可）：

1. **u066941 镜像族本体无 LinearToSrgb 尾巴**：编码证据只覆盖 UI/粒子族运行时捕获；Phase 1 的"解码+编码"依赖"RT 硬件 sRGB"推断。对本轮无影响——Phase 2a 只在既有四步管线内改两个 uniform 默认值，不触碰编码结构。
2. **演出段捕获 pc5_h[0]=(1000,10000,0.5,1.0)** ⇒ "t 恒 0"降级为"**战斗待机值未确证**"（0.5 是演出段值；方向是压暗，与本洗白现象反号，不影响本轮）。`reveal` 保持 0.0 不动。

### 1.5 语义映射与旧禁令作废说明

| uniform | 新语义（R2 定案） | 本轮处置 |
|---|---|---|
| `ambient_add` | **雾散射**（线性域灰加项，vec3） | 设估算默认值 vec3(0.15) |
| `sheen` | **spec/env**（GGX 高光+环境反射，线性域单色加，标量） | 设估算默认值 0.05 |
| `light_w` | 雾透射 | **保持 1.0** |
| `reveal` | 反相坡道 t | **保持 0.0** |

旧注释里"禁止调 ambient_add/light_w 凑亮度"的禁令基于"战斗无雾"前提——**R2 实证该前提未验证且雾是头号嫌疑**。本轮按 03 §2a"观感对齐"路径给 `ambient_add`/`sheen` 设**估算默认值**，注释如实标注"估算值，非运行时实值，待 hook（H1+H2/H3/H5）闭环后替换"，**不冒充真值**；`light_w` 本身仍保持中性 1.0（其禁令语义仍成立）。

---

## 二、选值（本轮 Python/numpy 实跑）

### 2.1 方法与校准

- 脚本：`C:\Users\jirai\AppData\Local\Temp\phase2a_stats.py`（临时目录，只读图集）
- 图集：`D:\ModForge\spikes\LingChar\pcksrc\ling\animations\characters\mirror\Chr_ling001.png`（find 定位实跑；镜像目录真实源，非中间产物；02 §2.7 同源）
- 口径（02 §2.7 / 04 D 节）：alpha>0.9 掩码；管线 = `srgb_to_linear` → 取反 → 线性域加灰 T → `linear_to_srgb`（Phase 1 公式逐字对应）
- **校准结果**：
  - 像素数 **149,447** 精确一致；T=0 全指标与主控现状口径逐项吻合（mean (0.705,0.757,0.773)、Rec.601 亮度 p10/p50/p90 (0.475,0.767,0.970)、sat 0.1332、sat>0.2 16.7%）⇒ **亮度定义钉死为 Rec.601（0.299,0.587,0.114）**；
  - 主控 §1.3 编码实验表五档**全部复现**（0.102/0.780、0.087/0.813、0.076/0.843、0.068/0.872、0.057/0.925）⇒ **主控模型 = 线性加灰后不设上限**（OETF 输出可 >1，未模拟 RT 钳位）。

### 2.2 预测统计表（候选 A/B 与 Phase 1 现状并排）

`ambient_add` 与 `sheen` 均为灰加（vec3 单值广播/标量），`light_w=1.0` 下公式 `lin = (inv + sheen) × 1.0 + ambient_add` 的预测**只取决于总加量 T = ambient_add + sheen**。

| 指标 | Phase 1 现状 (T=0) | **候选 A：0.15+0.05（T=0.20）** | 候选 B：0.20+0.05（T=0.25，落选） |
|---|---|---|---|
| **估算模型**（主控口径，不设上限） | | | |
| mean sRGB | (0.705, 0.757, 0.773) | (0.835, 0.866, 0.877) | (0.857, 0.886, 0.897) |
| 亮度 p10/p50/p90 | (0.475, 0.767, 0.970) | (0.660, 0.881, 1.056) | (0.696, 0.906, 1.076) |
| 平均 HSV 饱和度 | 0.1332 | **0.068** | 0.062 |
| 饱和度>0.2 占比 | 16.7% | ~9% | ~8% |
| **屏幕实况**（8-bit RT 钳位 ≤1.0，游戏内实际所见） | | | |
| mean sRGB | 同上 | (0.835, 0.866, 0.877) | (0.857, 0.886, 0.897) |
| 亮度 p10/p50/p90 | 同上 | (0.660, 0.881, **1.000**) | (0.696, 0.905, **1.000**) |
| 平均 HSV 饱和度 | 同上 | **0.062** | 0.055 |
| 线性域触顶（输出纯白）像素占比 | 0% | **32.1%** | 38.1% |

### 2.3 判据对照与裁定

判据（任务口径）：平均 HSV 饱和度 ≤0.07、亮度 mean ≥0.85、p90<1.0（保结构不死白）、暗部 p10 不高于 ~0.65。

| 判据 | 候选 A（T=0.20） | 候选 B（T=0.25） |
|---|---|---|
| sat ≤0.07 | **0.068 ✓**（屏幕口径 0.062 ✓） | 0.062 ✓ |
| 亮度 mean ≥0.85 | **0.872 ✓**（屏幕口径 0.858 ✓） | 0.899 ✓ |
| p10 ≤ ~0.65 | **0.660 ✓**（贴线） | 0.696 ✗（暗部被洗掉） |
| p90 <1.0 | 1.056（屏幕 1.000）✗ | 1.076（屏幕 1.000）✗ |

**裁定：选候选 A（ambient_add=0.15, sheen=0.05，总加 0.20）。** 理由：

1. 三项硬判据（sat/亮度/p10）全部达标，B 的 p10 明显超标（暗部结构被洗掉）；
2. p90 判据存在**结构性张力，如实记录**：sat ≤0.07 需要 T≥0.18，而 p90<1.0 需要 T≤约 0.07（T=0.10 时 p90 已达 1.014）——**在主控估算带 0.15~0.30 内，任何常数灰加都无法同时满足两者**（"基本全灰+亮度高"本身就意味着顶部触顶）。p90<1.0 在此作"选更保守一档"的相对判据使用：A 的纯白占比 32.1% 优于 B 的 38.1%；
3. A 更保守、可进可退：若用户验收"还不够灰/不够亮"，升到 B（0.20+0.05）只改一个数；反向则浪费一轮构建。

### 2.4 拆分语义（为什么是 0.15/0.05 而非别的拆法）

平面 2D 模型下 `ambient_add` 与 `sheen` 都是全画面均匀灰加，**拆分在数值上不可辨识**（只有 T 起作用）。拆分是**语义记账**，为 hook 闭环后各自独立替换留位：

- `sheen = 0.05`：锚定 UE4 shader 推导的高光系数 0.178（EnvBRDF(0.0296)×0.375×h67²=16，01 §2.4b）的保守分数（≈28%）——方向光色 pc3_h[0].xyz 与 SpecularScale 未确证，0.05 作为 spec+env 合成的下限估计；
- `ambient_add = 0.15`：余量给头号嫌疑**雾散射**（R2：手游镜像 FS 有雾项而普通角色没有，是"唯独镜像洗灰"的结构差异点）。

---

## 三、改动规格（逐字）

### 3.1 改动对象（仅 4 份 gdshader，仅 2 个 uniform 行 + 注释）

| 文件 | 改动 |
|---|---|
| `D:\ModForge\spikes\MirrorPupil\pcksrc\effects\mirror\ling_mirror_mix.gdshader` | ①头注补一段 Phase 2a 因果链（3.3）；②`ambient_add` 行默认值+注释；③`sheen` 行默认值+注释；④片元内 ③④ 步注释括号里"Phase 1 全中性：sheen=0, …, ambient_add=0"字样同步为新值（纯注释，防头注与正文自相矛盾） |
| 同目录 `ling_mirror_add.gdshader` / `ling_mirror_mul.gdshader` / `ling_mirror_sub.gdshader` | ②③④（render_mode 各自保留 blend_add/blend_mul/blend_sub；头注本就以"同 mix 头注"引用，自动覆盖） |

**uniform 行新文本**（四族逐字一致；注释列对齐沿用原文件列位）：

```glsl
uniform vec3 ambient_add = vec3(0.15);             // 雾散射（TEXCOORD7.xyz，线性域灰加）。Phase 2a 估算默认值（非运行时实值）：
                                                   // 与 sheen 合计 +0.20 对齐手游"全灰+高亮"观感；待 hook（H2/H3/H5）闭环后替换
uniform float sheen = 0.05;                        // spec+env 线性域单色加（GGX 高光+环境立方图×EnvBRDF）。Phase 2a 估算默认值
                                                   // （非运行时实值）；待 hook（H1：方向光色/SpecularScale）闭环后替换
```

（`vec3(0.15)` 为 GLSL/Godot 单分量广播构造，与原 `vec3(0.0)` 同构；`reveal`、`light_w` 两行**一字不动**。）

### 3.2 明确不动项

公式体/OETF 两函数/alpha 行/`v_col`/vertex/`reveal`/`light_w` 全部不动；不新增 uniform/采样器；4 份 `.tres` 不动（无 shader_parameter 覆写，02 §2.2，uniform 实值=gdshader 默认值）；任何 `.cs`/dll 不动；`build_lingchar.py` 不动；LingSilentSkin 侧不动。

### 3.3 mix 头注补段（全文，插在"色彩空间裁定"段之后、"2D 直译"段之前）

```glsl
// Phase 2a 灰加项（2026-10-11 晚 R2 取证）：手游镜像"饱和度≈0、亮度高"的真机制 =
// 镜像 FS 独有的三项单色加叠在弱色度负片上——①雾散射(TEXCOORD7.xyz) ②方向光 GGX
// 高光（板片法线恒朝相机⇒NoV≈NoH≈1⇒高光系数≈0.178 全画面均匀）③环境立方图×EnvBRDF；
// 普通战斗角色贴图直出（prog4/32/215 均无此三项），唯独镜像（UnlitHit 家族）有 ⇒
// 发白发灰是结构必然，PC 缺的是加项量级而非去饱和算子（19 份镜像 GLSL 全量 grep：
// 无输出去饱和）。本轮按 03 §2a 观感对齐路径为 ambient_add/sheen 设估算默认值（非
// 运行时实值，非真值），待 hook（H1+H2+H3+H5 一次会话）闭环后替换为实值；旧"禁止调
// ambient_add/light_w 凑亮度"禁令基于"战斗无雾"前提，R2 实证该前提未验证且雾是头号
// 嫌疑，禁令作废（light_w 本身仍保持中性 1.0）。估算依据与候选对照：05 文档 §2。
```

并把原头注"uniform 默认值=上列运行时真值"一行订正为"uniform 默认值：reveal=0/light_w=1 为运行时真值；ambient_add/sheen=Phase 2a 估算值（见上段），待 hook 闭环替换"（防同头注自相矛盾）。

### 3.4 四族一致性要求

四族改动**逐字一致**（剥离注释后 token 级一致，04 A1 同口径）；唯一差异保持为 render_mode 行。

---

## 四、红线（继承 03 §4.1/§4.4 + 本轮盘面）

| # | 红线 | 依据 |
|---|---|---|
| 1 | 备份目录必须在 pcksrc 之外（`pcksrc/effects/mirror/` 整目录 copytree 进包，备份放里面会被打进 pck） | 02 §5.5 |
| 2 | 4 份 `.tres` 不动；任何 `.cs`/dll 不动（零 C# 改动） | 03 §2.3/§4.1 |
| 3 | `build_lingchar.py` 不动 | 03 §4.1 |
| 4 | 不用 Bash echo/sed 写源码文件（用编辑工具） | 02 §5.4（Mimosa 钩子约束） |
| 5 | 不碰并行会话任何文件：`D:\项目\dragonraja\sts2_work\hitfx_research_2026-10-11\`（攻击特效线研究目录）、`DyeFramework.cs`（20:19:32/183,985B 锚点）、`attack_fx_ling.json`、`config.json`、fx 贴图、`LingSilentSkin.pck` | 本轮任务书 |
| 6 | `pcksrc_old_*` 备份目录不碰 | 03 §4.4 |
| 7 | 不启动游戏、不执行 hook | 本轮任务书 |
| 8 | 临时脚本/中间产物只放系统临时目录 | 本轮任务书 |

---

## 五、构建 / 对账 / 去污染 / 回滚

### 5.1 构建前预检（Phase B，全部通过才动文件；任一不过→等 5 分钟复查，仍不过→中止报告）

| # | 检查 | 判据 |
|---|---|---|
| a | `ls /d/ModForge/spikes/LingChar \| grep pcksrc_old_20261011_20` | 恰为 `_195514`、`_202221` 两个，未增多 |
| b | `D:\项目\dragonraja\sts2_work\DRCSpineDye\dll\DyeFramework.cs` | mtime=2026-10-11 20:19:32、183,985 B（并行线锚点，禁触） |
| c | `find /d/ModForge/spikes /d/项目/dragonraja -mmin -3`（排除 .git 与自己刚建的文件） | 无新写入 |
| d | `tasklist` | 无 SlayTheSpire2.exe、无正在跑的 dotnet/python 构建进程 |

**本轮预检前的盘面锚点（实测）**：`pcksrc_old_20261011_195514`/`_202221` 均在；DyeFramework.cs=20:19:32/183,985B ✓；LingChar.pck（仓=装件）23,580,265 B sha256 `26bd938044116f1a…`（并行线 20:22 重建产物，条目 56 勘误后盘面）；LingChar.dll（bin=装件）124,416 B sha256 `62b2b4c1f69e9cd1…`；LingSilentSkin.pck 20,210,510 B sha256 `7f751f119db9308d…`（2026-10-07 23:23 未动）；build_lingchar.py mtime 2026-10-11 11:25:14（本轮之前的盘面，不动它）。

### 5.2 备份（两处，均在 pcksrc 之外）

1. `D:\ModForge\spikes\MirrorPupil\backup_pre_phase2_20261011\` ← 复制 4 份 gdshader；
2. `D:\ModForge\spikes\LingChar\backup_installed_pre_phase2_20261011\` ← 复制游戏 Mods 三件 `LingChar.pck / LingChar.dll / LingChar.json`，记 sha256。

### 5.3 构建

```
cd /d/ModForge/spikes/LingChar && "/c/Program Files/Python310/python.exe" build_lingchar.py
```

（内部 assemble → dotnet build → pack → install 一条龙，03 §2.4；`C:\Program Files\Python310\python.exe` 存在已实测，numpy/PIL 可用。）

**中止条件**：构建中若出现 `pcksrc_old_*` 时间戳**秒级碰撞**（非本轮产生的新目录）→ 立即中止报告；dotnet build failed → 中止报告。

### 5.4 构建后对账（全部记入 §7 实施记录）

| # | 项 | 判据 |
|---|---|---|
| 1 | 新 LingChar.pck 大小+sha256 | 必然变化（4 份 gdshader 文本变大），记录为新锚点 |
| 2 | LingChar.dll sha256 vs 基线 `62b2b4c1f69e9cd1…`（124,416 B） | 相同=最佳；不同但 DyeFramework.cs/全部 .cs mtime 未变、csproj 未变 → 如实记录（MVID 非确定性）并继续；**.cs 有任何 mtime 变化 → 立即报告冲突不安装**（脚本已自动安装则如实报告并给回滚步骤） |
| 3 | 安装件三件（Mods/LingChar/）sha | 与构建产物逐一相等 |
| 4 | 包内容：4 份 gdshader 含新默认值特征串（`0.15`/`0.05`）且 `srgb_to_linear`/`linear_to_srgb`/Phase 1 特征串仍在 | 逐份比对 MirrorPupil 源逐字节相等 |
| 5 | 4 份 `ling_mirror_material_*.tres` | 仍 186 B |
| 6 | 并行线内容保全：pck 内 `config.json` 仍含 `streak_fade_in_s` 与 `flash_fade_in_s`/`flash_fadeout_start`/`flash_growth_peak` 键 | 键在 |
| 7 | 镜像相关条目路径 | 全部落在 `ling/animations/characters/mirror/`、`effects/mirror/`、`images/relics\|atlases/*mirror_pupil*`、`scenes/creature_visuals/mirror_image.tscn`、`.godot/imported/*`，无皮肤侧路径 |
| 8 | `LingChar.pck ∩ LingSilentSkin.pck` | = ∅ |
| 9 | LingSilentSkin.pck | 仍 20,210,510 B 未动 |
| 10 | `LingChar\pcksrc\effects\mirror\` 下 4 份 | 与 MirrorPupil 源逐字节一致（copytree 注入成功） |

（对账脚本：复用 `review_pck_check.py` 只读运行 + 临时补充脚本，均放临时目录或以只读方式运行。）

### 5.5 回滚

1. 用 `backup_pre_phase2_20261011` 覆盖 4 份 gdshader → 重跑 `build_lingchar.py` → pck sha 应回到 `26bd9380…`（pck_writer 对相同输入确定性，03 §2.5；若不能还原则以功能验证为准）；
2. 兜底：旧内容在本方案 §3.1 反向对照 + 02 §2.1 有存档，且 `backup_installed_pre_phase2_20261011` 有完整安装件三件可直接放回。

---

## 六、验收与未决项

### 6.1 游戏内验收口径（重启游戏 → 召唤镜像 → 与手游对比）

1. **观感**：镜像应从"亮负片（保留互补色残偏）"变为"**基本全灰 + 高亮度**"（≈手游观感）：HSV 饱和度目测趋零、整体明显更白；暗部（原贴图亮部反相后）仍保留少量结构不死白（预测 p10≈0.66）。
2. **日志**：无 SHADER ERROR、无 `[MirrorPupil] mirror materials missing from pck`。
3. **行为回归**：HitFlash/亮相 modulate/站位/尺寸不变（没碰 dll/tscn，v_col.rgb 仍丢弃，02 §2.6）。
4. **（可选）截图统计法**：对局内截图按 02 §2.7 口径统计，不透明区 sat 目标 ≈0.06±0.02、亮度 mean ≈0.86±0.03（注意截图含背景/特效，仅镜像像素域才与 §2.2 严格可比）。
5. **偏离处置**：若"还不够灰"→ 候选 B（0.20+0.05）一线之改；若"过白没结构"→ 回退 §5.5。**禁止再引入第三组无依据数值**。

### 6.2 未决项与 hook 提案（一次会话 H1+H2+H5+H3，本轮不执行）

| ID | 目标 | 产出将替换哪个 uniform |
|---|---|---|
| **H1** | hook `glShaderSource`，按特征串过滤抓镜像材质本体运行时 FS | 确认编码尾巴与槽位；`pc3_h[0].xyz`（方向光色）+`pc3_h[2].z`（SpecularScale）→ **替换 `sheen=0.05` 估算值** |
| **H2** | hook `glUniform4fv` 抓 `pc5_h` | 确认战斗待机 MaxDarkenAlpha 是否为 0（0.5 是演出段值，§1.4-2）→ `reveal` 维持或修正 |
| **H5** | hook `glTexImage2D`/`glCompressedTexImage2D` internalformat | 闭合"图集 sRGB 解码"推断 → 若非 sRGB 解码，Phase 1 解码/编码对需结构性重审 |
| **H3** | hook `SceneSeparateTranslucencyModulateColor` | 半透明整层统一染色/提亮 → 若非中性，为**第四个加项**（当前模型未含，可能是"更白"的残差来源） |
| （附） | VS 雾实值（TEXCOORD7.xyz 分离捕获） | → **替换 `ambient_add=0.15` 估算值** |

---

## 七、实施记录（构建对账完成后补写）

### 7.1 本轮执行状态：Phase C 中止（2026-10-11 21:32，预检不过）

**已完成**：Phase A 设计（§1-§6 全部落盘，选值=候选 A `ambient_add=vec3(0.15)` / `sheen=0.05`，预测统计见 §2.2，为本轮 Python/numpy 实跑复现主控口径后裁定）。

**中止原因**：Phase B 预检 (b)(c) 两轮均不过，且第二轮恶化——

| 项 | 首查（21:26:36） | 5 分钟后复查（21:31:57） | 判定 |
|---|---|---|---|
| (a) pcksrc_old 今晚段 | 恰 `_195514`/`_202221` 两个 | 同，未增多 | ✓ |
| (b) DyeFramework.cs | mtime **21:25:27**、183,985 B（锚点 20:19:32 已被破坏） | mtime **21:31:24**（33 秒前）、**227,113 B**（+43,128 B） | ✗ 并行会话正在活跃编辑 |
| (c) 3 分钟内新写入 | hit_fx 贴图 ×5（21:22-23）、make_hit_fx_textures.py（21:22:18）、DyeFramework.cs | DyeFramework.cs（21:31:24） | ✗ |
| (d) 进程 | 无 SlayTheSpire2/dotnet/python | 同 | ✓（编辑间隙，不代表安全） |

并行 ZCode 会话（攻击特效线）正在改 `DyeFramework.cs`（21:22 起连续写入）与 hit_fx 纹理，**随时可能触发其 `build_lingchar.py` 重建**——此刻执行本轮 Phase C（备份/改 shader/构建/安装）必然与其构建互撞（pcksrc_old 时间戳碰撞、dll 竞争、安装覆盖），正是 §5.1 中止条件设计的场景。

**本轮实际写入面**（未越界）：①本 05 文档；②临时目录统计脚本 `C:\Users\jirai\AppData\Local\Temp\phase2a_stats.py`。**未动**：4 份 gdshader（sha 仍为 Phase 1 值 407811e7/cadd8752/6d4439c1/39a33606）、任何备份目录、构建脚本、任何 .cs/.tres/json、LingSilentSkin、并行线任何文件。

**恢复条件与续作指引**：待并行线完全静默（连续 ≥3 分钟无新写入 + DyeFramework.cs mtime/大小稳定 + 无构建进程）后重跑 §5.1 预检，通过即按 §5.2→§5.4 执行；注意届时 DyeFramework.cs 锚点应改用并行线**新收口值**（227,113 B 或其最终值）重新记录，§5.4-2 的 dll 对账基线 `62b2b4c1…` 也需按并行线重建后的实际值更新后再用。

### 7.2 实施记录（2026-10-11 21:32-21:58，授权重试成功）

**收口轮询**：21:35:56 起共 13 轮（每轮 sleep 60），21:41-21:43 并行线落账写入（光影层移植计划.md/项目状态交接.md），21:45-21:48 仅剩 .mimosa 治理器状态文件，**21:49:03 与 21:50:17 连续两轮 QUIET** → 收口（累计约 15 分钟，未超 25 分钟上限）。

**锚点快照（收口后立即，21:50-21:52）**：

| 项 | 值 |
|---|---|
| DyeFramework.cs | **21:32:25 / 227,119 B**（并行线定稿） |
| 已装 LingChar.pck | 24,149,874 B（218 条），sha256 `8c9a1cc6068d3e51b0cd13e2b12debbbcb5a44158f3e06aed42089158d72acfc`，21:33:05（条目 57 并行线构建产物；仓内同 sha） |
| 已装 LingChar.dll | 138,240 B，sha256 `208c195db634bdf594083eda6a44cb745e93c32c6016947b9f78fa1d6cf66cb4`，21:33:04（**本轮 dll 对账基线**） |
| LingChar.json | 472 B，sha256 `dd76bd33258ba2099091545cea389f5470fbd1fe80c60ddff6cd0bc94359cd86` |
| pcksrc_old 今晚段 | `_195514`、`_202221`（并行线第一二轮）、`_213241`、`_213257`（第三轮 21:32，间隔 16 秒非碰撞） |
| src 全部 .cs（17 份） | 最新 mtime 2026-10-11 04:14:12（ModEntry.cs），20:00 后无改动 |
| 一致性判据 | DyeFramework.cs(21:32:25) **早于** 已装 dll(21:33:04) ✓ = 并行线已编译安装过其 C# 改动 |

**备份（均在 pcksrc 之外）**：

| 目录 | 内容 | sha256 前 8 |
|---|---|---|
| `MirrorPupil\backup_pre_phase2_20261011` | 4 份 gdshader | mix `407811e7` / add `cadd8752` / mul `6d4439c1` / sub `39a33606`（=Phase 1 现值） |
| `LingChar\backup_installed_pre_phase2_20261011` | Mods 三件 | pck `8c9a1cc6` / dll `208c195d` / json `dd76bd33` |

**改动实施（§3 逐字规格）**：4 份 gdshader 改后 sha256 前 8：mix `fefdc5a1`（6,489B）/ add `cec202f5`（2,633B）/ mul `760a0f67`（2,642B）/ sub `a4085b3c`（2,641B）。校验：uniform 行新文本与 §3.1 逐字一致；四族剥离注释+render_mode 后 token 序列一致；与改前代码 diff 仅默认值差异（add 文件全量 126 token 抽验相等）；`reveal=0.0`/`light_w=1.0`/公式体/OETF/alpha 行未动；mix 头注新增 Phase 2a 因果链段 +"uniform 默认值"行订正；片元 ③④ 注释括号同步（纯注释）。

**预检复查（21:54:14，构建前）**：pcksrc_old 今晚段仍 4 个未增多 ✓；DyeFramework.cs 仍 21:32:25/227,119B ✓；近 3 分钟写入仅为本会话 shader 修改与备份目录 ✓；无构建/游戏进程 ✓。

**构建（21:54，build_lingchar.py 全链零错误）与对账表**：

| # | 项 | 结果 |
|---|---|---|
| 1 | 新 LingChar.pck | **24,151,772 B（218 条）**，sha256 `1efc136f2c0ba6b19e024e13faca3bc6c1a222fda3f461ddc64c6236c24a4d5c`（+1,898B=4 份 shader 文本增量；**新锚点**，并行线 8c9a1cc6 转为改前基线） |
| 2 | LingChar.dll | **138,240 B，sha256 `208c195d…` 与收口快照基线逐字节相同**（零 C# 改动+MVID 确定性=最佳分支） |
| 3 | 安装件三件 | pck/dll/json 与仓内构建产物 sha 逐一相等（`1efc136f`/`208c195d`/`dd76bd33`） |
| 4 | 包内 gdshader | 4 份**条目 md5==MirrorPupil 源**（`2c496101`/`76d7156c`/`d24d68c5`/`e6e427e5`），`off+file_base` 提取逐字节一致；含 `ambient_add = vec3(0.15)`、`uniform float sheen = 0.05;`、srgb_to_linear/0.04045/linear_to_srgb/0.0031308、"Phase 2a 估算默认值"标注；`ambient_add = vec3(0.0)` uniform 旧默认值残留=0（grep 到的 4 处 `vec3(0.0)` 均为公式体合法保留：OETF 钳位/解码 clamp/反相 mix/输出 max） |
| 5 | 4 份 tres | 包内仍 186 B ×4 |
| 6 | 并行线 config.json 键保全 | `streak_fade_in_s`/`flash_fade_in_s`/`flash_fadeout_start`/`flash_growth_peak` 全在 |
| 7 | 镜像路径清洁 | 26 条全部落在 `ling/animations/characters/mirror/`、`effects/mirror/`、`images/relics\|atlases/*mirror_pupil*`、`scenes/creature_visuals/mirror_image.tscn`、`.godot/imported/*`，皮肤侧路径 0 条 |
| 8 | 两 pck 交集 | LingChar(218 条) ∩ LingSilentSkin(46 条) = **∅** |
| 9 | LingSilentSkin.pck | 20,210,510 B，`7f751f119db9308d…` 未动 |
| 10 | pcksrc 注入 | `LingChar\pcksrc\effects\mirror\` 4 份与 MirrorPupil 源 cmp 逐字节一致 |
| （附） | .cs 复查 | 构建后 src 17 份 .cs 无 mtime 变化（最新仍 04:14）；DyeFramework.cs 仍 21:32:25/227,119B |
| （附） | 仓=装件条目差 | 0（218 条三元组逐一相等） |

**异常与工具注记**：

1. **`review_pck_check.py` [5] 误报（工具口径，非包缺陷）**：该脚本对本包报"包内 gdshader==源 False"——本包条目 flags=0（并行线 21:33 起的打包产物不再给条目置相对偏移标志），脚本 `flg&2` 判定不成立即用绝对偏移提取 → 错位 112B。经条目 md5 比对 + `off+file_base` 提取双路复核均为 True，包内数据正确。下轮复核请按条目 md5 或强制 `+file_base` 口径。
2. 首次尝试（21:26-21:32）按 §7.1 中止一次，本轮为授权重试，全程无 pcksrc_old 秒级碰撞、无 dotnet 失败、无任何中止条件触发。

**落账**：光影层移植计划.md **条目 58**（本轮；条目 57 为并行线命中特效移植，其产物记录 `8c9a1cc6`/`208c195d` 与本节快照逐位一致，交叉验证通过）。

