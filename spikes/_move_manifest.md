# spikes 迁移清单（ModForge → dragonraja）

> 执行者：migration-fix ｜ 执行日：2026-10-16
> 内容：`D:\ModForge\spikes\{LingChar,MirrorPupil}` → `D:\项目\dragonraja\spikes\` 的**收尾工作**（目录搬运由主控完成，本文件只记录路径修订 / 重建验证 / 文档落账）。
> 全过程**未执行任何 git 提交**（红线；统一由主控处理）。

## 0. 进度

- [x] 任务一：修订硬编码引用（26 文件 / 34 处）
- [x] 任务二：重建验证（全链绿 + 产物 sha 归因 + 去污染 + toolchain FAIL=0）
- [x] 任务三：文档落账（本文件 + 光影层移植计划.md 条目 60 + 项目状态交接.md + .gitignore）

---

## 1. 源 → 目标映射表

| # | 源（旧树） | 目标（新树） | 文件数 | 备注 |
|---|---|---|---|---|
| 1 | `D:\ModForge\spikes\LingChar\` | `D:\项目\dragonraja\spikes\LingChar\` | 7,941（含本次构建新产生的 `pcksrc_old_*`/`scratch_old_*`；搬运交付时为 7,337） | 角色 mod 主体；含 `_mirror_phase1_backup_20261011` 以外的全部历史备份目录 |
| 2 | `D:\ModForge\spikes\MirrorPupil\` | `D:\项目\dragonraja\spikes\MirrorPupil\` | 68（24 目录） | 含 `_mirror_phase1_backup_20261011\`（4 份改前 gdshader）、`backup_pre_phase2_20261011\` |

未迁、原地保留（红线：不许动）：`D:\ModForge\spikes\ArtProbe\`、`D:\ModForge\spikes\P0Minimal\`。
`D:\ModForge\spikes\` 现状确认：仅余上述两个目录（实跑 `ls` 验证）。

## 2. 回滚方法

目录层反向 `mv`（Windows 下先把目标同名目录移走，避免合并；`spikes\` 为空/已存在皆可）：

```bash
mv /d/项目/dragonraja/spikes/LingChar    /d/ModForge/spikes/LingChar
mv /d/项目/dragonraja/spikes/MirrorPupil /d/ModForge/spikes/MirrorPupil
```

**① 目录回滚**：上两条（执行前先把同名旧位置的目标清掉，避免目录合并）。
**② 路径修订回滚**：§3 全部改动均为「旧树绝对路径 → 树内相对路径」，反向恢复只需按 §3 表格把「新值」换回「旧值」即可。本轮**未 commit**：已跟踪文件（`tools/*`、`光影层移植计划.md`、`项目状态交接.md`、`.gitignore`）可直接 `git checkout`；`spikes/` 尚未入版本控制，回滚靠 mv + §3 表反向替换。
**③ 产物回滚**：改前已装三件完整备份在 `spikes\LingChar\backup_installed_pre_phase2_20261011\`（pck `8c9a1cc6`/24,149,874B、dll `208c195d`/138,240B、json `dd76bd33`/472B）。注意该备份对应条目 57 基线，**不是**本次重建前的状态；本次重建前无单独留备份，但 (a) pck 逐字节复现了 `3ea68992`（=重建前值），(b) dll 与 `208c195d` 仅调试壳体不同、IL 等价（见 §4.3 归因）——需要精确到 `208c195d` 字节的 dll 只能在旧树路径下重编，现已不可得。

## 3. 改点清单（文件:行号 旧→新）

### 3.1 构建链路（断链必改）

| 文件 | 行 | 常量/表达式 | 旧值 | 新值 |
|---|---|---|---|---|
| `spikes\LingChar\LingChar.csproj` | 24 | `Compile Include` | `..\..\..\项目\dragonraja\sts2_work\DRCSpineDye\dll\DyeFramework.cs` | `..\..\sts2_work\DRCSpineDye\dll\DyeFramework.cs`（树内化） |
| `spikes\LingChar\build_lingchar.py` | 550 | `spike_loc` | `D:\ModForge\spikes\MirrorPupil\loc\localization` | `ROOT.parent / 'MirrorPupil/loc/localization'` |
| `spikes\LingChar\build_lingchar.py` | 556 | `mirror_fx` | `D:\ModForge\spikes\MirrorPupil\pcksrc\effects\mirror` | `ROOT.parent / 'MirrorPupil/pcksrc/effects/mirror'` |

**复核无需改动的硬编码**（目标仍正确，实跑确认存在）：`DR_ROOT`（:28 `D:\项目\dragonraja`）、`GAME`（:32 Steam 目录）、`GODOT`（:424 `D:/Tools/Godot/Godot_v4.7.2-stable_mono_win64_console.exe`，存在=True）、`dye_pcksrc`（:567 `D:\项目\dragonraja\sts2_work\DRCSpineDye\pcksrc`）；csproj `GameAssemblyDir`（:12）指向游戏目录，仍有效。

### 3.2 工具链自检（否则检查脚本自己会失能）

| 文件 | 行 | 旧值 | 新值 |
|---|---|---|---|
| `tools\toolchain_check.py` | 113 | `bl = Path(r'D:\ModForge\spikes\LingChar\build_lingchar.py')` | `bl = ROOT / 'spikes' / 'LingChar' / 'build_lingchar.py'` |
| `tools\toolchain_check.py` | 141-144 | MF 命中时仅在「路径不存在」才 FAIL | 命中任何 ModForge 字面量即 FAIL（迁移后守门：树内脚本不该再引用旧盘） |

### 3.3 验证脚本（referenced by 03/04/05 review docs）

| 文件 | 行 | 旧值前缀 | 新值前缀 |
|---|---|---|---|
| `sts2_work\mirror_review_2026-10-11\verify_pck_phase1.py` | 42 | `D:/ModForge/spikes/MirrorPupil/...` | `D:/项目/dragonraja/spikes/MirrorPupil/...` |
| `sts2_work\mirror_review_2026-10-11\verify_numbers_phase1.py` | 8 | `D:/ModForge/spikes/LingChar/...` | `D:/项目/dragonraja/spikes/LingChar/...` |
| `sts2_work\mirror_review_2026-10-11\review_pck_check.py` | 41, 42, 99 | `REPO`/`SRC_MIRROR`/dll 路径 | 同上新前缀（dll 改 `REPO + '/bin/...'` 拼接） |

### 3.4 兜底排查：spike 内诊断脚本（22 文件 33 处前缀替换）

`spikes\LingChar\tools\`：`derive_fire_time.py`、`diag_dir6.py`、`diag_dir7.py`、`diag_dir7b.py`、`diag_dir7d.py`、`diag_dir7e.py`、`diag_dir7g.py`、`diag_dir8.py`、`diag_dir8_shot2.py`、`diag_dir8_tex.py`、`diag_dir8_uv.py`、`diag_dir9_shot.py`、`diag_shot3.py`、`diag_shot4.py`、`diag_shot4c.py`、`diag_shot5b.py`、`diag_shot5d.py`、`q2_char_probe.py`、`q2_decisive.py`、`q2_final_artifacts.py`、`q2_forensics.py`
统一把 `D:\ModForge\spikes\LingChar` → `D:\项目\dragonraja\spikes\LingChar`（含 `D:/` 变体）。
**唯一例外（保留未改，如实记录）**：`diag_shot3.py:219` 的 `D:\ModForge\项目\dragonraja2\render_work\attack_tex\fr_obj_901_3x3.png` —— 目标是另一份 UE4 提取贴图，该路径 `D:\ModForge\项目\` 本身已不存在，**属迁移前就失效的死引用**；新树 `render_work\` 下无 `attack_tex\` 目录、也无等价物，改无可依。**该脚本此项已失效，待后续按新树补正**（先在新树建立 `render_work\attack_tex\` 源贴图，或改指向现有 `render_work\battle_capture\` 证据链），其余 32 处前缀全部改毕。

### 3.5 配置与文档

| 文件 | 位置 | 改动 |
|---|---|---|
| `tools\toolchain_manifest.json` | `bundle_layout`、`source_roots` 旁注、`artifacts` 2 条、`items` 2 条(root MF→DR)、`rebuild.lingchar_mod`、`path_edits_after_migration` 3 条 | 全部改为入树后口径（跨仓约束作废；dst 改 `项目/dragonraja/spikes/...`）。JSON 语法校验通过 |
| `spikes\LingChar\README.md` | 31 | 「跨仓 Compile Include」→「树内相对引用」+ 单一事实源仍禁改的红字 |
| `spikes\MirrorPupil\README.md` | — | 无 ModForge 路径，**无需改**（实跑 grep 确认） |
| `光影层移植计划.md` | 末 | 追加**条目 60** |
| `项目状态交接.md` | §0 铁律 5、§1 项目地图+目录表、§2 基线顺延提醒、§3 构建链、§4 数据流、§6 迁移包、§8 索引 | 全部同步为入树后口径 |
| `.gitignore` | 末尾 | 追加 `spikes/LingChar/{bin,obj}/`、`spikes/MirrorPupil/{bin,obj}/`、`spikes/LingChar/pcksrc_old_*/`、`spikes/LingChar/icon_import_scratch/`、`__pycache__/` |

**保留不动的历史留档**（刻意不改写，避免篡改时间记录）：`sts2_work\mirror_review_2026-10-11\0{2,3}.md`、`05_*.md`、`sts2_work\hitfx_research_2026-10-11\DESIGN.md`、`迁移说明.md` —— 其中的 `D:\ModForge\spikes\...` 是 2026-10-11 当时的取证记录/当时有效路径，属史实（已于工作群/主日志记录口径）。

## 4. 构建与验证结果

### 4.1 构建

命令：`cd /d/项目/dragonraja/spikes/LingChar && "/c/Program Files/Python310/python.exe" build_lingchar.py`
结果：**EXIT=0**，装配→dotnet→打包→安装全绿。

- `dotnet build`：**0 错误 / 23 警告**（CS8604 ×N、CS0169 ×2 等，全部落在 `DyeFramework.cs` 与既有 `src/`——**迁移前就存在**，与本次路径修订无关；我未修改任何 `.cs` 内容，唯一改动是 csproj 的一个 Include 路径）。
- 打包：`packed 227 files, 24179275 bytes`。
- 安装：`D:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\Mods\LingChar\{LingChar.json, LingChar.dll, LingChar.pck}`。
- **可复现性**：相同环境下**再跑一次**，pck/dll sha 逐一相同 ⇒ 新树构建确定性成立（不是随机噪声）。

> **本机怪癖（重试必读）**：Mimosa safe-delete 钩子会拦 `shutil.rmtree(icon_import_scratch)`（`{"count":58,"threshold":50, ... SAFE_DELETE_BULK_CONFIRM_REQUIRED}`，非交互会话无法确认，脚本会静默中断）。处理办法：重跑前先 `mv icon_import_scratch scratch_old_<ts>` 改名让位（`mv` 不是删除，不被拦）。已同步写入 `项目状态交接.md` §3。

### 4.2 产物 sha 与锚点比对

| 产物 | 大小 | sha256 | 与锚点比对 |
|---|---|---|---|
| `LingChar.pck`（仓内） | 24,179,275 B | `3ea68992bb84a212ab07260a830bb45181bb77fbd3350c71eba443e821eb6156` | = 条目 59（2026-10-15）新基线，**逐字节一致** ✓ |
| `LingChar.pck`（游戏已装件） | 24,179,275 B | 同上 | 与仓内逐字节一致 ✓ |
| `LingChar.dll`（仓内/已装件） | 138,240 B | `17ca0e61869496b524b3be4d4b27c43a50080864fd780d512019851217694f6c` | 与旧树 `208c195d…` **同尺寸但不同 sha**，差异已归因（见 4.3） |
| `LingChar.json` | 472 B | `dd76bd33258ba2099091545cea389f5470fbd1fe80c60ddff6cd0bc94359cd86` | 未变 ✓ |
| `LingSilentSkin.pck` | 20,210,510 B | `7f751f119db9308d92095974175cc9a21a19b9de3a0ad79c8765177d113bc1ed` | = 要求锚点 `7f751f11…`，**未受影响** ✓ |

任务书给的锚点 `256b22ed…` / `7bfaed8d…`(116,224B) 是**条目 46**（2026-10-11 镜像 Phase 1 后）的旧基线，已在条目 57→58→59 三轮重建中顺延作废（证据：`sts2_work\mirror_review_2026-10-11\04_独立审查报告.md:42-44,94`「应以 256b22ed/7bfaed8d 为当前锚点」→ 该结论本身又已被条目 57/58/59 覆盖；条目 59⑧ 明确记录新基线 pck `3ea68992`/dll `208c195d`）。

### 4.3 dll 差异归因（决定性证据）

对照样本：`spikes\LingChar\backup_installed_pre_phase2_20261011\LingChar.dll` = `208c195d…`（138,240B，改前已装件，实跑核验 sha 相符）。
逐字节比较：**258 字节不同，分 46 段**，全部落在调试/元数据壳体：

| 偏移区 | 内容 | 旧 → 新 |
|---|---|---|
| 0x88-0x8B、0xA8、0x100、0x180、0x200 | PE 头 TimeDateStamp / 相关校验字段 | 随内容哈希变化 |
| 0x14836 附近（4 处单/双字节） | 元数据 RVА 指针表 | 全部 **+8**（因下方 pdb 路径串变长） |
| 0x1EB14-0x1EB1F（16B） | **MVID** GUID（ `"LING5_EPOCH"` 字符串后紧随的 16 字节） | 内容确定性哈希随之改变 |
| 0x21218 附近 | **CodeView 调试目录**：PDB GUID + age + **PDB 绝对路径字符串** | `D:\ModForge\spikes\LingChar\obj\...\LingChar.pdb` → `D:\项目\dragonraja\spikes\LingChar\obj\...\LingChar.pdb`（UTF-8 长度 59→68，`.text` 原始尺寸 `0x21210`→`0x21218`） |
| 同区 | PdbChecksum（SHA-256 32B） | 随 pdb 内容变化 |

**结论**：用户 IL 代码、类型/方法表实质内容无差异，差异 100% 来自「把 C# 工程搬到新目录后，编译器把新的 PDB 绝对路径写进 Debug Directory」——**这是 .NET 编译的固有属性，不是源文件变化、不是污染、不是构建不确定性**（同一确定路径下重复构建 sha 稳定，已实测）。
**无并行会话改源码的证据**：`spikes\LingChar\src\*.cs` 最新 mtime `2026-10-11 04:14`（ModEntry.cs）、`spikes\MirrorPupil\src\*.cs` 最新 `2026-10-05`、`sts2_work\DRCSpineDye\dll\DyeFramework.cs` `2026-10-11 21:32:25`（227,119B，与条目 58④ 记录逐位一致）——三者自 10-11 起均未再被写；git 端 `DyeFramework.cs` 相对 HEAD 的改动是 10-11 并行线产物（早于本次），本次未触碰。

### 4.4 去污染 / 自检

- **PCK 解析口径**（与铁律一致）：`GDPC` 头 → 目录偏移读 `0x20` → `file_base=0x70` → `flags=2 (PACK_REL_FILEBASE)` → 条目内容 = `pck[file_base + fo]`。
- `LingChar.pck` **227 条**；`LingSilentSkin.pck` **46 条**；**交集 = 0 条** ✓（`flags` 均为 2、`file_base` 均 0x70）。
- 已装件 227 条 ↔ 仓内 227 条：条目名与 size 逐一相符（整体逐字节相等，见 4.2）。
- `tools\toolchain_check.py`：**FAIL=0，WARN=5**（4 条 T2/T3 未随迁：`unpacked/so/libUE4.so`、`unpacked/reversed/{,lua_source,ue4src}`+1 条游戏侧皮肤 pck 未安装；与条目 59⑤记录完全一致，非迁移引入）。
- `tools\check_ling_pcks.py` 读的是**游戏侧**三个 pck（其中 `LingSilentSkin.pck` 当前未装该 mod，属条目 59⑤ 已知环境状态），故本次去污染核查改用同一 PCK v3 规则直接对**仓内**两份 pck 实施，结论同上。
- Godot 可执行文件存在（`D:/Tools/Godot/...console.exe`=True），构建期 headless 导入（遗物图标 + 6 张 attack fx + 5 张 hit fx）全部成功（日志 `icon imported:` / `attack fx imported:` / `hit fx imported:` 共 13 行全出）。

## 5. 铁律修订说明（原「两树必须同级」）

原条款（现 `项目状态交接.md` §0 铁律 5；任务书称"铁律 3"，编号口径不同，指同一条）立论依据：
`LingChar.csproj` 用 `..\..\..\项目\dragonraja\sts2_work\DRCSpineDye\dll\DyeFramework.cs` **跨仓**编译 DyeFramework，因此要求 `项目\` 与 `ModForge\` 同级。

**修订**：LingChar / MirrorPupil 两个 spike 已于 2026-10-16 入树，该 Include 已改为**树内相对引用** `..\..\sts2_work\DRCSpineDye\dll\DyeFramework.cs`，跨仓依赖消失 ⇒ **「两树同级」对这两个 spike 不再适用**（ModForge 侧仅余与本树无关的 ArtProbe/P0Minimal）。

**仍然成立不可违背的两条**：
1. `D:\项目\dragonraja` 的中文目录名"项目"**不可改**（既有脚本与历史文档按该名定位）。
2. `sts2_work\DRCSpineDye\dll\DyeFramework.cs` 仍是**框架单一事实源**——不允许改其内容、不允许另起第二份（本次零改动，mtime 仍是 10-11 21:32:25）。

## 6. 遗留问题（交主控裁定）

1. **`STS2_work` 之外仍有旧史实路径**：§3.5 列的 4 份历史文档内的 `D:\ModForge\spikes\...` 未改（刻意留作时间记录）。若主控要求"零 ModForge 引用"，需另行指示，届时应以「加脚注」而非「改正文」方式处理。
2. **`diag_shot3.py:219`** 死引用（目标 `D:\ModForge\项目\...` 迁移前就不存在），需源贴图到位才有正确新值。
3. **`pcksrc_old_*` / `scratch_old_*` 垃圾累积**：`spikes\LingChar\` 下已有 **37 个 `pcksrc_old_*`**（每个约 24MB）+ 2 个 `scratch_old_*` + `icon_import_scratch`；并非本次引入（构建脚本为避免触碰 bulk-delete 钩子而"改名保留"），但会显著膨胀入仓体积。已加 `.gitignore` 兜底，**建议后续择机人工清理**（清理属删除动作，Mimosa 钩子会拦 bulk>50，需分批或人工）。
4. **`spikes/` 尚未入版本控制**（`git status` 显示为 `??`）：本次按红线**未 commit**，由主控统一操作。
5. **`项目状态交接.md` §2 交付物表**的 pck/dll 两行仍是条目 57 旧基线（在本次之前就已过期）：已在表后加"基线顺延提醒"指向本文件 §4，**未篡改表体**（留给主控决定是否改写）。
6. **游戏侧 `LingSilentSkin` 未安装**：故 `check_ling_pcks.py` / `review_pck_check.py` 跑不完整（条目 59⑤ 已知的环境状态），本次未触碰皮肤链。
