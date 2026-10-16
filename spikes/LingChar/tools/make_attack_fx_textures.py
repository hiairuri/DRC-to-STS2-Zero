#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""LingChar 攻击特效纹理加工（2026-10-10 方案书 A 档 · 条目 30/42/44）。

P1 增补（条目 59，自研攻击特效设计稿 2026-10-15）：streak_mask_v2（96×320
三段轴向色）/ flash_teardrop（224×80 火舌）/ spark_dash（64×16 流光短线单帧）
三张新纹理 + selfcheck_p1；颜色为设计标定（§0.6 色板），alpha 峰一律归一化到
0.90（防饱和硬约束，裁定见 _normalize_peak）。旧三张（spark_grid/streak_mask/
flash_quad）链路保留但 config 不再引用（退役不删）。

三链路取值铁律：所有参数来自源码/资产(uasset·uexp)/运行时 hook，不截图反解。
本脚本走"资产"链路——把 UE 粒子纹理真值转成 Godot 可用的 PNG：

  1) fr_obj_901_3x3 512² 已解码 PNG（只读源
     dragonraja2/render_work/attack_tex/fr_obj_901_3x3.png）-> 直接拷/另存为
     fx_sparks3x3_grid.png（3×3 方形网格，h_frames=3/v_frames=3）。
     条目 42 打回修复：Godot particles_animation 下单帧内容拉伸铺满 quad，
     条带 9×1 数学上不可修成方形火花——必须方形网格纹理，水平 unroll
     （1536×171 拼条）步骤已删除。
  2) 程序生成 fx_streak_mask.png（条目 45：88×260 光条遮罩 = 44×130 的
     2× 重生成，锯齿缓解+更长渐变尾，见下豁免声明）
  3) 程序生成 fx_flash.png（条目 45：192×16 横向闪光面片 = 96×8 的 2×
     重生成，同豁免）

【过曝白核（条目 45 新增，适用于 2)/3)）】
纹理自带"亮核→白"的过曝梯度，是 UE ColorScaleOverLife 20→5 的 LDR 近似
= 真值映射（非观感校准）：UE 侧 L(t)=AlphaOverLife(1→0)×ColorScaleOverLife
(20→5)，输出到 LDR 时被裁剪。裁剪反推（以 8bit 通道满值 1.0 为界）：
  R 通道：基色 0.3698×20×(1-t) ≥1 恒成立到 t≤0.46 → t≤0.46 全裁为白核；
  G 通道：0.8489×20×(1-t) ≥1 → t≤0.765（之后沿 0.8489 线性衰减）；
  B 通道：1.0×20×(1-t) ≥1 → t≤0.80。
即 UE 屏上亮核是"先一段纯白过曝、再回落到基色"——本脚本用
hot=clamp((A-0.40)/0.40)^1.2 × 0.85 把它折进纹理的 RGB（A 越高越白；
条目 45 定案：阈值 0.55→0.40 放宽=亮核区域更大更久，混白 0.96→0.85=
白核柔和不过曝死白）。
注意：既然 C0/Cf 已烘焙进纹理 RGB，Godot 侧 modulate.rgb 必须是白，
否则颜色被应用两次（条目 45 ★"高亮丢失"真根因）。

【程序生成豁免声明（条目 44 定案，条目 45 沿用，适用于 2)/3)）】
UE 转储（unpacked/reversed + iguf 索引）中 fx_ling_attack_01 相关的全部
曲线均为时间轴曲线（t→值），不存在任何沿 U/V 的空间 alpha 曲线；
ma_412_uv 64² 转储实测全白、无形状数据，仅作 CutoutTexture 占位；
"沿长轴亮核→透明"的渐变形状由 UE 材质 Mi_Ling_Skill 的 UV 动画实时合成，
该材质未移植（无 shader 源可译）。因此源数据中不存在可解码的空间渐变
纹理，只能程序生成合成近似——标注为观感校准豁免；颜色真值 C0/Cf 取自
UE 转储（streak 青 (0.3698,0.8489,1.0)、flash (0.4010,0.7404,1.0)）。
教训入档：渐变在材质不在纹理时必须"程序生成+明确豁免"，不可把实心
遮罩当成品（条目 44 打回截图：实心四边形+全宽横带）。

去污染铁律：产物只写 LingChar 自有路径（assets/attack_fx 持久源 +
pcksrc 目标目录当前快照），绝不写回 dragonraja2（只读备份）或
LingSilentSkin_battle/pcksrc（皮肤源树）。
中间产物放工作树 fx_tex_scratch/ 临时目录。

用法（需 pillow/numpy）:
  "C:/Program Files/Python310/python.exe" make_attack_fx_textures.py
"""
import shutil
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent      # LingChar 仓库根
TMP = ROOT / 'fx_tex_scratch'
OUT = ROOT / 'assets/attack_fx'                     # 持久源（build 从这里导入）
PCK_DST = ROOT / 'pcksrc/drc_dye/profiles/ling/animations/characters/ling'

SRC_SPARK_PNG = Path(r'D:\项目\dragonraja2\render_work\attack_tex\fr_obj_901_3x3.png')

# 条目 45 尺寸定案（第五轮打回，2026-10-11）：
#   streak 纹理 44×130 → 88×260【2× 重生成，锯齿缓解】：
#     - 理由：STS2 无 HDR bloom，UE 靠 ColorScale 20→5+引擎 bloom 放大光晕；
#       2D 旋转面片的锯齿只能靠更高纹素密度+更软 alpha 边缓解——2× 分辨率下
#       LINEAR 采样梯度更平滑，旋转时锯齿边缘更柔（观感校准豁免）。
#     - C# 按 tex.GetWidth()/GetHeight() 反算 Scale（DyeFramework.cs
#       EmitStreakSprite/EmitFlashSprite），屏幕尺寸完全由 config size 决定，
#       纹理分辨率改变不影响屏幕尺寸。
#     - 88×260 相对新屏幕尺寸 32×100（config 定案值）≈2.75× 超采样，长宽比
#       0.338 vs 屏幕长宽比 0.32，偏差 5.8% 在观感校准豁免内（横向渐变略压缩）。
#   flash 纹理 96×8 → 192×16【同 2× 重生成逻辑】：屏幕 size 定案 [120,12]，
#     纹理 1.6× 超采样，同锯齿缓解理由。
#   sparks fx_sparks3x3_grid：条目 55 勘误之二起羽化作用于**内容轮廓**——
#     512² 已解码源直出后对 alpha 做半径 1px box blur（每帧内容边缘 1~2px
#     梯度带，只软边不改形状；观感校准豁免，见 spark_grid docstring）。
#     条目 55 原版"帧界羽化"已删（内容距帧界 ≥2.3px，fade 恒=1 零操作）。
STREAK_TEX_W, STREAK_TEX_H = 88, 260
FLASH_W, FLASH_H = 192, 16

# UE 转储颜色真值
C0_STREAK = (0.3698, 0.8489, 1.0)   # 光条基色（青）
C0_FLASH = (0.4010, 0.7404, 1.0)    # 闪光基色
CW = (1.0, 1.0, 1.0)                # 亮核白

# --- P1 自研特效色板（自研攻击特效设计稿 2026-10-15 §0.6，设计标定非真值）---
CORE_WHITE = (0.92, 0.98, 1.00)     # core 近白核（枪口端）
MID_CYAN = (0.37, 0.85, 1.00)       # mid 青（=fade_to，项目主题色）
TAIL_DEEP = (0.16, 0.34, 0.72)      # tail 深蓝（远端/尾段）
P1_ALPHA_PEAK = 0.90                # core 层 α 峰 ≤0.90 防饱和（硬约束）
P1_HOT_M = 0.35                     # 弱 hot 白核混白系数（设计稿：白核亮度交给轴向渐变）


def clamp01(x: float) -> float:
    return 0.0 if x < 0.0 else (1.0 if x > 1.0 else x)


def smoothstep(a: float, b: float, x: float) -> float:
    t = clamp01((x - a) / (b - a))
    return t * t * (3.0 - 2.0 * t)


def smoothstep_np(a: float, b: float, x):
    """smoothstep 的 numpy 向量化版（P1 新纹理函数用；标量版不动）。"""
    import numpy as np
    t = np.clip((x - a) / (b - a), 0.0, 1.0)
    return t * t * (3.0 - 2.0 * t)


def spark_grid(src_png: Path, dst: Path) -> None:
    """512² 3×3 源纹理直出（条目 42）：不裁帧不拼条，方形网格原样进引擎。
    Godot particles_animation 单帧内容拉伸铺满 quad，h=v=3 才能保方形火花。
    条目 55 勘误之二（红线 B 重做）：羽化作用于**内容轮廓**——条目 55 原版
    "帧界 1.5px 羽化带"是零操作（九帧内容距帧界 ≥2.3px，fade 因子恒=1，
    输出与源逐字节相同），已删除。现改为对每帧 alpha 通道做半径 1px 均值
    滤波（3×3 box blur）：内容边缘像素与相邻透明像素平均 → 1~2px alpha
    梯度带，加算混合下硬边多边形软边。RGB 不动（羽化只乘/混 alpha，
    alpha=0 处保留基色防 LINEAR 采样拉暗的思路同 streak）。
      幂等说明：blur 作用于 alpha，每次跑都会再糊一点（梯度带变宽）。
      脚本用已羽化标记（dst 同名 .feathered 标记文件）守卫——标记存在时
      跳过羽化只做搬运；如需强制重新羽化，删除标记文件或 assets 下产物。
    帧布局不变：内容距采样帧界线 ≥2px，3×3 核够不着帧界 → 帧界 alpha
    恒 0（布局不变，自检保留）。"""
    from PIL import Image
    import numpy as np
    im = Image.open(src_png).convert('RGBA')
    w, h = im.size
    arr = np.asarray(im, dtype=np.float32).copy()
    a = arr[..., 3]
    a_before = a.astype(np.int32)
    # 3×3 box blur（半径 1px），只糊 alpha：边缘复制 pad（不引入图像外暗边）
    p = np.pad(a, 1, mode='edge')
    b = np.zeros_like(a)
    for dy in (0, 1, 2):
        for dx in (0, 1, 2):
            b += p[dy:dy + h, dx:dx + w]
    arr[..., 3] = b / 9.0
    out = Image.fromarray(arr.astype(np.uint8), 'RGBA')
    out.save(dst, optimize=True)
    # 幂等标记（内容轮廓羽化已应用的凭证）
    dst.with_suffix(dst.suffix + '.feathered').write_text(
        'content-contour alpha box-blur r=1px applied\n', encoding='utf-8')
    # 自检 1：3×3 全部帧界行列 alpha 仍为 0（布局不变，blur 够不着帧界）
    a2 = np.asarray(out)[:, :, 3]
    fwx, fhy = w / 3, h / 3
    borders = []
    for i in range(4):
        x = min(int(round(i * fwx)), w - 1)
        borders.append(a2[:, x].max())
    for j in range(4):
        y = min(int(round(j * fhy)), h - 1)
        borders.append(a2[y, :].max())
    # 自检 2：内容边缘 alpha 梯度（1~254 的过渡像素数）必须显著增加
    partial = int(((a2 >= 1) & (a2 <= 254)).sum())
    partial_before = int(((a_before >= 1) & (a_before <= 254)).sum())
    print(f'spark grid: {src_png.name} {w}x{h} -> {dst.name} '
          f'(条目 55 勘误之二：内容轮廓 alpha 3×3 box blur，帧界 alpha 最大值='
          f'{max(int(b) for b in borders)} 应 0；alpha 过渡像素 {partial_before}'
          f'->{partial}，应显著增加)')
    assert max(int(b) for b in borders) == 0, '帧界 alpha 必须为 0（布局被 blur 破坏）'
    assert partial > partial_before, '内容轮廓羽化未生效（过渡像素数未增加）'


def streak_mask(dst: Path) -> None:
    """条目 45：程序生成 88×260 光条遮罩（2× 重生成=44×130 的两倍纹素密度；
    屏幕尺寸由 config size 决定、C# 按 tex 尺寸反算 Scale，屏幕观感不变；
    豁免依据见模块头注释——观感校准豁免：锯齿缓解+更长渐变尾）。

    轴向约定：第 0 行=枪口端（亮核/白核），第 259 行=远端（全透明）。
    公式（条目 45 定案，第四轮打回）：
      v=(y+0.5)/H；
      rise=smoothstep(0,0.04,v)              枪口端 4% 快速起坡（≈5px 软起）
      fall=(1-v)^1.2                          沿长轴衰减（衰减尾较原 1.35 加长）
      tip 已并入 fall（条目 45 定案公式无独立 tip 项，远端由 (1-v)^1.2 自然收零）
      along=rise*fall
      u=(x+0.5)/W；t=1-|2u-1|；
      cross=t^1.6                             横向软边（中心列=1，不归一化）
      A=cross*along
      hot=clamp01((A-0.40)/0.40)^1.2         过曝白核（阈值 0.55→0.40 放宽：
                                              亮核区域更大更久）
      m=0.85*hot；rgb=C0+(Cw-C0)*m           白核混白系数 0.96→0.85（条目 45
                                              定案：白核更柔和、避免过曝死白）
    四边 alpha 严格为 0（防硬边）；alpha=0 处 RGB 仍填基色（防 LINEAR
    采样把透明边拉暗）。"""
    from PIL import Image
    W, H = STREAK_TEX_W, STREAK_TEX_H
    im = Image.new('RGBA', (W, H))
    px = im.load()
    for y in range(H):
        v = (y + 0.5) / H
        rise = smoothstep(0.0, 0.04, v)
        fall = (1.0 - v) ** 1.2
        along = rise * fall
        for x in range(W):
            u = (x + 0.5) / W
            t = 1.0 - abs(2.0 * u - 1.0)
            cross = t ** 1.6
            A = cross * along
            if x == 0 or x == W - 1 or y == 0 or y == H - 1:
                A = 0.0  # 四边硬归零（防硬边）
            hot = clamp01((A - 0.40) / 0.40) ** 1.2
            m = 0.85 * hot
            rgb = tuple(round(255 * (C0_STREAK[i] + (CW[i] - C0_STREAK[i]) * m)) for i in range(3))
            px[x, y] = rgb + (round(255 * A),)
    im.save(dst, optimize=True)
    print(f'streak mask: {W}x{H} procedural（条目 45 2× 重生成，锯齿缓解豁免）')


def flash_quad(dst: Path) -> None:
    """条目 45：程序生成 192×16 闪光面片（2× 重生成=96×8 的两倍纹素密度，
    锯齿缓解豁免同 streak_mask；长轴=横向，第 0 列=枪口侧；豁免依据见模块
    头注释）。颜色/透明度时间曲线仍由 C# 侧 modulate 控制，纹理只承载空间
    渐变形状。公式（条目 45 定案，与 streak 同族；条目 56 R1 订正）：
      u=(x+0.5)/W；
      along=smoothstep(0,0.10,u) * (1-u)^1.1
            ↑条目 56（R1 硬边）：后缘=col0 侧（枪口端，锚定端）渐入 ramp
            0.05→0.10（9.6px→19.2px ≥16px）——旧 ~5px 有效硬边在 C# 锚定
            修正后直接贴枪口暴露为硬亮条，拉宽成渐入；远端 (1-u)^1.1 不动。
      s=(y+0.5)/H；t=1-|2s-1|；
      cross=t^1.0=1-|2s-1|                纵向软边（中心行=1，不归一化）
      A=along*cross
      hot=clamp01((A-0.40)/0.40)^1.2；m=0.85*hot
      rgb=Cf+(Cw-Cf)*m                    过曝白核（同 streak）
    峰值影响：along 峰移到 u=0.10 处 =0.9^1.1=0.8906 → 峰值 alpha
    ≈0.8906×0.9375=0.835（旧 0.886，selfcheck 阈值同步 0.85→0.83）。
    四边 alpha 严格为 0 ——条目 45 根修沿用：四边不归零会让 LINEAR 采样在
    横向出现"上下亮线"（打回截图实证）；alpha=0 处 RGB 仍填基色。
    注意：.feathered 幂等守卫是 spark 专用（spark_grid），flash/streak 为
    纯程序生成、每次重跑直接覆盖，守卫逻辑不影响本函数。"""
    from PIL import Image
    W, H = FLASH_W, FLASH_H
    im = Image.new('RGBA', (W, H))
    px = im.load()
    for x in range(W):
        u = (x + 0.5) / W
        along = smoothstep(0.0, 0.10, u) * (1.0 - u) ** 1.1
        for y in range(H):
            s = (y + 0.5) / H
            t = 1.0 - abs(2.0 * s - 1.0)
            cross = t
            A = along * cross
            if x == 0 or x == W - 1 or y == 0 or y == H - 1:
                A = 0.0  # 四边硬归零（条目 45 根修沿用：上下亮线）
            hot = clamp01((A - 0.40) / 0.40) ** 1.2
            m = 0.85 * hot
            rgb = tuple(round(255 * (C0_FLASH[i] + (CW[i] - C0_FLASH[i]) * m)) for i in range(3))
            px[x, y] = rgb + (round(255 * A),)
    im.save(dst, optimize=True)
    print(f'flash quad: {W}x{H} procedural（条目 45 2× 重生成，锯齿缓解豁免）')


def _axial_rgb_3seg(v: float) -> tuple:
    """设计稿三段轴向色（§1.1 core）：v=0→近白核、v=0.35→青、v=1→深蓝，
    分段线性插值。"""
    if v <= 0.35:
        t = 0.0 if v <= 0.0 else v / 0.35
        a, b = CORE_WHITE, MID_CYAN
    else:
        t = 1.0 if v >= 1.0 else (v - 0.35) / 0.65
        a, b = MID_CYAN, TAIL_DEEP
    return tuple(a[i] + (b[i] - a[i]) * t for i in range(3))


def _mix_hot(rgb: tuple, A: float, m_coef: float = P1_HOT_M) -> tuple:
    """弱 hot 白核（设计稿：hot=clamp01((A-0.45)/0.45)^1.2，m=0.35×hot）。
    A 支持 numpy 数组（np.clip 向量化）。"""
    import numpy as np
    hot = np.clip((A - 0.45) / 0.45, 0.0, 1.0) ** 1.2
    m = m_coef * hot
    return rgb + (np.asarray(CW, dtype=np.float32) - rgb) * m


def _normalize_peak(A: 'object', peak: float) -> 'object':
    """归一化 alpha 场到峰值=peak。

    设计稿公式字面值 A=cross×along×0.90 与其自检硬约束"峰值∈[0.88,0.91]"
    不相容（像素中心 cross<1、along 峰<1，乘 0.90 后峰值≈0.843）——按自检
    硬约束意图落地为归一化：A=A_raw/A_raw_max×0.90，"全局乘 0.90"以峰值
    口径成立。边界归零在归一化之后执行（不参与 max）。"""
    import numpy as np
    m = float(A.max())
    if m <= 0.0:
        return A
    return A * (peak / m)


def streak_mask_v2(dst: Path) -> None:
    """P1（设计稿 §1.1）：96×320 光条遮罩 v2——三段轴向色+α 峰 0.90+弱 hot。

    轴向约定不变：第 0 行=枪口端，第 319 行=远端。
    公式（设计稿定案；alpha 归一化裁定见 _normalize_peak docstring）：
      v=(y+0.5)/H；rise=smoothstep(0,0.03,v)；fall=(1-v)^1.5（尾比 1.2 更收）
      along=rise*fall
      u=(x+0.5)/W；t=1-|2u-1|；cross=t^1.8（中心列=1 不归一，像素中心<1 由
        峰值归一化补偿）
      A=normalize(cross*along, peak=0.90)
      轴向 RGB 三段混色（烘焙进纹理，modulate 保持白——条目 45★ 教训）
      hot=clamp01((A-0.45)/0.45)^1.2；m=0.35*hot（比旧 0.85 大幅减弱——白核
        亮度交给 axial 渐变承担，避免 LDR 二次饱和）
    四边 alpha 硬归零、alpha=0 处 RGB 填 mid 色（既有纪律沿用）。
    selfcheck（设计稿 §1.1）：峰值 alpha∈[0.88,0.91]；v≈0.05/0.35/0.70 三点
    RGB 分别≈白核/青/深蓝。"""
    import numpy as np
    W, H = 96, 320
    ys = (np.arange(H) + 0.5) / H
    rise = smoothstep_np(0.0, 0.03, ys)
    fall = (1.0 - ys) ** 1.5
    along = rise * fall
    xs = (np.arange(W) + 0.5) / W
    cross = (1.0 - np.abs(2.0 * xs - 1.0)) ** 1.8
    A = np.outer(along, cross)
    A = _normalize_peak(A, P1_ALPHA_PEAK)
    A[0, :] = 0.0; A[-1, :] = 0.0; A[:, 0] = 0.0; A[:, -1] = 0.0  # 四边硬归零
    rgb = np.zeros((H, W, 3), dtype=np.float32)
    for y in range(H):
        ax = _axial_rgb_3seg(float(ys[y]))
        rgb[y, :, :] = ax
    rgb = _mix_hot(rgb, A[..., None])
    zero = A <= 0.0
    rgb[zero] = MID_CYAN  # alpha=0 处填 mid 色（防 LINEAR 采样拉暗）
    out = np.dstack([np.clip(rgb, 0, 1), A[..., None]])
    from PIL import Image
    im = Image.fromarray((out * 255.0 + 0.5).astype(np.uint8), 'RGBA')
    im.save(dst, optimize=True)
    print(f'streak mask v2: {W}x{H} procedural（P1 三段轴向色+α峰0.90+弱hot）')


def flash_teardrop(dst: Path) -> None:
    """P1（设计稿 §2.1）：224×80 枪口火焰火舌（192×16 细条退役）。

    长宽比 2.8，配 config size [150,54]；第 0 列=枪口侧（沿用轴向约定）。
    公式（设计稿定案；alpha 归一化裁定同 streak_mask_v2）：
      u=(x+0.5)/W；along=smoothstep(0,0.06,u)*(1-u)^1.6
        ↑后缘渐入 ramp（条目 56 R1 纪律沿用）：0.06×224=13.4px 渐入带
      s=(y+0.5)/H；t=1-|2s-1|；cross=t^2.2（椭圆截面=火舌轮廓）
      A=normalize(cross*along, peak=0.90)
      RGB 两段轴向：u=0→(0.92,0.98,1.00)、u=1→(0.37,0.85,1.00) 线性；
      hot 弱化同 streak（m=0.35）
    四边归零 + alpha=0 处 RGB 填 mid 青。"""
    import numpy as np
    W, H = 224, 80
    us = (np.arange(W) + 0.5) / W
    along = smoothstep_np(0.0, 0.06, us) * (1.0 - us) ** 1.6
    ss = (np.arange(H) + 0.5) / H
    cross = (1.0 - np.abs(2.0 * ss - 1.0)) ** 2.2
    A = np.outer(cross, along)
    A = _normalize_peak(A, P1_ALPHA_PEAK)
    A[0, :] = 0.0; A[-1, :] = 0.0; A[:, 0] = 0.0; A[:, -1] = 0.0
    rgb = np.zeros((H, W, 3), dtype=np.float32)
    for x in range(W):
        t = float(us[x])
        ax = tuple(CORE_WHITE[i] + (MID_CYAN[i] - CORE_WHITE[i]) * t for i in range(3))
        rgb[:, x, :] = ax
    rgb = _mix_hot(rgb, A[..., None])
    zero = A <= 0.0
    rgb[zero] = MID_CYAN
    out = np.dstack([np.clip(rgb, 0, 1), A[..., None]])
    from PIL import Image
    im = Image.fromarray((out * 255.0 + 0.5).astype(np.uint8), 'RGBA')
    im.save(dst, optimize=True)
    print(f'flash teardrop: {W}x{H} procedural（P1 火舌形+后缘 ramp 13.4px+α峰0.90）')


def spark_dash(dst: Path) -> None:
    """P1（设计稿 §3.2）：64×16 单帧流光短线（替代 3×3 硬边网格）。

    h=v=1 时 Godot particles_animation SubUV 语义退化=整图单帧（C#
    CanvasItemMaterial HFrames=VFrames=1，uv=frame_offset+uv/(1,1)=全图），
    零 C# 改动。公式（设计稿定案）：
      u=(x+0.5)/W；along=smoothstep(0,0.12,u)*smoothstep(0,0.12,1-u)
        （两端 smoothstep 收头，与 §1.1 runner 同族）
      s=(y+0.5)/H；t=1-|2s-1|；cross=t^1.5
      A=normalize(cross*along, peak=0.90)（设计稿未给峰值的防饱和兜底，
        同 core 层 ≤0.90 纪律）
      RGB 沿长轴 青→深蓝（色由 C# ColorInitialRamp color_a/color_b 再染，
        设计稿两处均明确此双层配色）
    四边归零 + alpha=0 处 RGB 填 mid 青。"""
    import numpy as np
    W, H = 64, 16
    us = (np.arange(W) + 0.5) / W
    along = smoothstep_np(0.0, 0.12, us) * smoothstep_np(0.0, 0.12, 1.0 - us)
    ss = (np.arange(H) + 0.5) / H
    cross = (1.0 - np.abs(2.0 * ss - 1.0)) ** 1.5
    A = np.outer(cross, along)
    A = _normalize_peak(A, P1_ALPHA_PEAK)
    A[0, :] = 0.0; A[-1, :] = 0.0; A[:, 0] = 0.0; A[:, -1] = 0.0
    rgb = np.zeros((H, W, 3), dtype=np.float32)
    for x in range(W):
        t = float(us[x])
        ax = tuple(MID_CYAN[i] + (TAIL_DEEP[i] - MID_CYAN[i]) * t for i in range(3))
        rgb[:, x, :] = ax
    zero = A <= 0.0
    rgb[zero] = MID_CYAN
    out = np.dstack([np.clip(rgb, 0, 1), A[..., None]])
    from PIL import Image
    im = Image.fromarray((out * 255.0 + 0.5).astype(np.uint8), 'RGBA')
    im.save(dst, optimize=True)
    print(f'spark dash: {W}x{H} procedural（P1 流光短线单帧，两端 smoothstep 收头）')


def selfcheck_p1(dst_streak2: Path, dst_flash2: Path, dst_dash: Path) -> None:
    """P1 三张新纹理离线自检（设计稿 §1.1/§2.1/§3.2 selfcheck 增项）。"""
    import numpy as np
    from PIL import Image

    # --- streak v2：峰值 α∈[0.88,0.91]；三段色点 ≈白核/青/深蓝 ---
    im = Image.open(dst_streak2).convert('RGBA')
    W, H = im.size
    assert (W, H) == (96, 320), f'streak v2 尺寸应 96x320，实测 {W}x{H}'
    arr = np.asarray(im, dtype=np.float32) / 255.0
    a = arr[..., 3]
    border = max(a[0].max(), a[-1].max(), a[:, 0].max(), a[:, -1].max())
    print(f'[selfcheck-p1] streak v2 {W}x{H} 四边 alpha 最大值 = {border:.4f}（应 0）')
    assert border == 0.0, 'streak v2 四边 alpha 必须为 0'
    amax = float(a.max())
    print(f'[selfcheck-p1] streak v2 峰值 alpha = {amax:.4f}（应 ∈[0.88,0.91]）')
    assert 0.88 <= amax <= 0.91, f'streak v2 峰值 alpha 应 ∈[0.88,0.91]，实测 {amax:.4f}'
    cx = W // 2
    for label, y, lo, hi, chan in (('v≈0.05 白核', 16, 0.82, 1.01, 0),
                                   ('v≈0.35 青', 112, 0.33, 0.44, 0),
                                   ('v≈0.70 深蓝', 224, 0.0, 0.35, 0)):
        r, g, b = arr[y, cx, 0], arr[y, cx, 1], arr[y, cx, 2]
        print(f'[selfcheck-p1] streak v2 {label}: RGB=({r:.3f},{g:.3f},{b:.3f})')
        assert lo <= r <= hi, f'streak v2 {label} R 通道应 ∈[{lo},{hi}]，实测 {r:.3f}'
    # 深蓝点附加：G/B 同步下沉（非白非青）
    r, g, b = arr[224, cx, 0], arr[224, cx, 1], arr[224, cx, 2]
    assert g < 0.70 and b < 0.92, f'streak v2 v≈0.70 应深蓝，实测 ({r:.3f},{g:.3f},{b:.3f})'

    # --- flash teardrop：峰值 α、后缘 ramp、峰值 RGB ---
    im2 = Image.open(dst_flash2).convert('RGBA')
    W2, H2 = im2.size
    assert (W2, H2) == (224, 80), f'flash2 尺寸应 224x80，实测 {W2}x{H2}'
    arr2 = np.asarray(im2, dtype=np.float32) / 255.0
    a2 = arr2[..., 3]
    border2 = max(a2[0].max(), a2[-1].max(), a2[:, 0].max(), a2[:, -1].max())
    print(f'[selfcheck-p1] flash teardrop {W2}x{H2} 四边 alpha 最大值 = {border2:.4f}（应 0）')
    assert border2 == 0.0, 'flash teardrop 四边 alpha 必须为 0'
    amax2 = float(a2.max())
    print(f'[selfcheck-p1] flash teardrop 峰值 alpha = {amax2:.4f}（应 ∈[0.88,0.91]）')
    assert 0.88 <= amax2 <= 0.91, f'flash teardrop 峰值 alpha 应 ∈[0.88,0.91]，实测 {amax2:.4f}'
    midy = H2 // 2
    rear = [float(a2[midy, x]) for x in range(0, 15)]
    inc = all(rear[i] < rear[i + 1] for i in range(13))  # ramp 带=col0..13（13.4px）
    print(f'[selfcheck-p1] flash teardrop 后缘 ramp col0..14（中心行）='
          f'{[round(v, 3) for v in rear]}（col0..13 应严格递增，ramp 13.4px 渐入带，'
          f'col14 已过 along 峰 u=0.06 属预期回落）')
    assert inc and rear[13] > rear[1] + 0.10, \
        'flash teardrop 后缘渐入 ramp 不存在（col0..13 未单调递增）'
    import numpy as _np
    _py, _px = _np.unravel_index(int(a2.argmax()), a2.shape)
    pk = arr2[_py, _px]
    print(f'[selfcheck-p1] flash teardrop 峰值 RGB = ({pk[0]:.3f},{pk[1]:.3f},{pk[2]:.3f})'
          f'（应≈白核 hot 混合，R≥0.85）')
    assert pk[0] >= 0.85, f'flash teardrop 峰值 R 应 ≥0.85，实测 {pk[0]:.3f}'

    # --- spark dash：峰值 α、两端收头、RGB 青→深蓝 ---
    im3 = Image.open(dst_dash).convert('RGBA')
    W3, H3 = im3.size
    assert (W3, H3) == (64, 16), f'spark dash 尺寸应 64x16，实测 {W3}x{H3}'
    arr3 = np.asarray(im3, dtype=np.float32) / 255.0
    a3 = arr3[..., 3]
    border3 = max(a3[0].max(), a3[-1].max(), a3[:, 0].max(), a3[:, -1].max())
    print(f'[selfcheck-p1] spark dash {W3}x{H3} 四边 alpha 最大值 = {border3:.4f}（应 0）')
    assert border3 == 0.0, 'spark dash 四边 alpha 必须为 0'
    amax3 = float(a3.max())
    print(f'[selfcheck-p1] spark dash 峰值 alpha = {amax3:.4f}（应 ∈[0.88,0.91]）')
    assert 0.88 <= amax3 <= 0.91, f'spark dash 峰值 alpha 应 ∈[0.88,0.91]，实测 {amax3:.4f}'
    my = H3 // 2
    rL, rR = arr3[my, 2, 0], arr3[my, W3 - 3, 0]
    print(f'[selfcheck-p1] spark dash 轴向 RGB：左端 R={rL:.3f}（≈青 0.37）、'
          f'右端 R={rR:.3f}（≈深蓝 0.16），应递减')
    assert rL > rR and rL >= 0.30 and rR <= 0.25, \
        f'spark dash 轴向色应 青→深蓝 递减，实测 {rL:.3f}→{rR:.3f}'
    # 两端收头：col0 与 col63 alpha=0 已由四边断言覆盖；中心行首列过渡带存在
    print(f'[selfcheck-p1] spark dash 中心行 alpha 剖面（抽样）='
          f'{[round(float(a3[my, x]), 3) for x in (1, 4, 8, 16, 31, 47, 56, 60, 62)]}')


def selfcheck(dst_streak: Path, dst_flash: Path) -> None:
    """离线剖面校验（条目 45 自检项，公式按 2× 重生成定案更新）：
    streak（88×260）:
      - 四边 alpha 必须全 0；
      - 纵向剖面（中心列 u≈0.5）：v≈0.05≈0.924、v≈0.20≈0.751、
        v≈0.50≈0.427、v≈0.90≈0.062、v=1.0=0（理论值=(1-v)^1.2×cross0.982）；
      - 峰值 alpha≈0.93（条目 45 定案 cross=t^1.6 不做中心归一化，中心列
        t=1-1/88=0.9886 → cross=0.982，峰值<1 属预期；亮度由 RGB 白核承担）；
      - 峰值 RGB=(231,249,255)（hot=1 → m=0.85 → C0+(Cw-C0)×0.85）。
    flash（192×16）:
      - 四边 alpha 必须全 0（沿用条目 45 根修：上下亮线）；
      - 峰值 alpha≈0.835（中心行 t=1-1/16=0.9375 × along 峰 0.8906；
        条目 56：ramp 0.05→0.10 后峰移到 u=0.10，旧值 0.886）；
      - 后缘 ramp（条目 56 R1 硬边）：中心行 col1→col16 alpha 严格递增
        （19.2px 渐入带存在证据）；
      - 峰值 RGB≈(232,245,255)。"""
    from PIL import Image

    im = Image.open(dst_streak).convert('RGBA')
    W, H = im.size
    px = im.load()
    border = [px[x, 0][3] for x in range(W)] + [px[x, H - 1][3] for x in range(W)] + \
             [px[0, y][3] for y in range(H)] + [px[W - 1, y][3] for y in range(H)]
    print(f'[selfcheck] streak {W}x{H} 四边 alpha 最大值 = {max(border)}（应 0）')
    assert max(border) == 0, 'streak 四边 alpha 必须为 0'
    cx = W // 2
    for label, y in (('v≈0.05', int(0.05 * H)), ('v≈0.20', int(0.20 * H)),
                     ('v≈0.50', H // 2), ('v≈0.90', int(0.90 * H)), ('v=1.0', H - 1)):
        print(f'[selfcheck] streak u=0.5 {label}: alpha={px[cx, y][3] / 255:.4f}')
    # 峰值 alpha / 峰值 RGB / 高亮占比
    amax = 0
    peak_rgb = (0, 0, 0)
    hi = 0
    for y in range(H):
        for x in range(W):
            a = px[x, y][3]
            if a > amax:
                amax = a
                peak_rgb = px[x, y][:3]
            if a > 0.8 * 255:
                hi += 1
    print(f'[selfcheck] streak 峰值 alpha = {amax / 255:.4f}（应 ≈0.93，cross 不归一化）')
    print(f'[selfcheck] streak 峰值 RGB = {peak_rgb}（应 ≈(231,249,255)，白核 m=0.85）')
    assert amax / 255 >= 0.90, f'streak 峰值 alpha 应 ≥0.90，实测 {amax / 255:.4f}'
    assert peak_rgb[0] >= 230 and peak_rgb[1] >= 248, \
        f'streak 峰值 RGB 应 ≈(231,249,255)，实测 {peak_rgb}'
    print(f'[selfcheck] streak alpha>0.8 占比 = {hi / (W * H) * 100:.2f}%'
          f'（条目 45 公式无 v≤0.30 恒 1 高原，白核亮度改由 RGB hot 承担，'
          f'占比仅作参考打印）')

    im2 = Image.open(dst_flash).convert('RGBA')
    W2, H2 = im2.size
    px2 = im2.load()
    top = [px2[x, 0][3] for x in range(W2)]
    bot = [px2[x, H2 - 1][3] for x in range(W2)]
    left = [px2[0, y][3] for y in range(H2)]
    right = [px2[W2 - 1, y][3] for y in range(H2)]
    print(f'[selfcheck] flash {W2}x{H2} 第 0 行 alpha 最大值 = {max(top)}（应 0）')
    print(f'[selfcheck] flash {W2}x{H2} 第 {H2 - 1} 行 alpha 最大值 = {max(bot)}（应 0）')
    print(f'[selfcheck] flash 第 0 列/第 {W2 - 1} 列 alpha 最大值 = '
          f'{max(left)}/{max(right)}（应 0）')
    assert max(top) == 0 and max(bot) == 0, 'flash 上下边 alpha 必须为 0（条目 45 根修）'
    assert max(left) == 0 and max(right) == 0, 'flash 左右边 alpha 必须为 0'
    amax2 = 0
    peak_rgb2 = (0, 0, 0)
    for y in range(H2):
        for x in range(W2):
            a = px2[x, y][3]
            if a > amax2:
                amax2 = a
                peak_rgb2 = px2[x, y][:3]
    print(f'[selfcheck] flash 峰值 alpha = {amax2 / 255:.4f}（应 ≈0.835，条目 56 ramp 拉宽后）')
    print(f'[selfcheck] flash 峰值 RGB = {peak_rgb2}（应 ≈(232,245,255)）')
    assert amax2 / 255 >= 0.83, f'flash 峰值 alpha 应 ≥0.83（条目 56 定案），实测 {amax2 / 255:.4f}'
    assert peak_rgb2[0] >= 230, f'flash 峰值 R 应 ≥230，实测 {peak_rgb2[0]}'
    # 条目 56（R1 硬边）：后缘渐入 ramp 宽度自检——中心行 col1..col16 alpha
    # 严格递增（smoothstep 0.10×192=19.2px 渐入带的机读证据）
    midy = H2 // 2
    rear = [px2[x, midy][3] for x in range(0, 17)]
    inc = all(rear[i] < rear[i + 1] for i in range(len(rear) - 1))
    print(f'[selfcheck] flash 后缘 ramp col0..16（中心行）= {rear}'
          f'（条目 56：应严格递增，ramp 19.2px ≥16px）')
    assert inc and rear[16] > rear[1] + 30, 'flash 后缘 16px 渐入 ramp 不存在（col0..16 未单调递增）'
    print(f'[selfcheck] flash u=0(x=0): {px2[0, H2 // 2][3] / 255:.4f}  '
          f'u≈0.05(x=9): {px2[9, H2 // 2][3] / 255:.4f}  u=0.5(x={W2 // 2}): {px2[W2 // 2, H2 // 2][3] / 255:.4f}  '
          f'u=1(x={W2 - 1}): {px2[W2 - 1, H2 // 2][3] / 255:.4f}')


def main() -> None:
    if TMP.exists():
        shutil.rmtree(TMP)
    TMP.mkdir(parents=True)
    OUT.mkdir(parents=True, exist_ok=True)

    # 1) 火花：512² 已解码源 PNG 直出 3×3 网格（条目 42：不再 unroll 拼条）
    # 条目 55 勘误之二：dragonraja2 只读备份仍不在盘上（D:\项目\dragonraja2
    # 缺失，已登记主日志勘误之二），回退用上轮产物 assets/attack_fx/
    # fx_sparks3x3_grid.png 作基底。幂等守卫：内容轮廓 blur 作用于 alpha，
    # 每次跑都会再糊一点——已羽化标记（.feathered）存在时跳过羽化只搬运，
    # 勿反复跑；如需强制重羽化，删除标记与产物（或恢复原始源后自然走源）。
    spark_dst = OUT / 'fx_sparks3x3_grid.png'
    spark_marker = spark_dst.with_suffix('.png.feathered')
    spark_src = SRC_SPARK_PNG if SRC_SPARK_PNG.exists() else spark_dst
    if not SRC_SPARK_PNG.exists():
        if spark_marker.exists():
            print(f'NOTE: 源纹理 {SRC_SPARK_PNG} 不存在且 {spark_dst.name} 已含内容轮廓羽化'
                  f'（标记文件在）——跳过重复羽化，只同步到 pcksrc（幂等保护，勿反复跑）')
        else:
            print(f'WARN: 源纹理 {SRC_SPARK_PNG} 不存在，回退用上轮产物 {spark_src} 作羽化基底'
                  f'（首次做内容轮廓羽化，跑完写已羽化标记；重复跑会被标记拦下）')
        if spark_marker.exists():
            pass  # 已羽化：跳过 spark_grid，产物保持现状
        else:
            spark_grid(spark_src, spark_dst)
    else:
        spark_grid(spark_src, spark_dst)

    # 2) 光条遮罩：程序生成（条目 44：ma_412_uv 解码直出已删除——全白无形状）
    streak_mask(OUT / 'fx_streak_mask.png')

    # 3) 闪光面片：程序生成（条目 44 新渐变公式）
    flash_quad(OUT / 'fx_flash.png')

    selfcheck(OUT / 'fx_streak_mask.png', OUT / 'fx_flash.png')

    # 4) P1（条目59，自研特效设计稿）：三张新纹理——旧三张链路退役不删
    #    （fx_sparks3x3_grid 保留羽化守卫；config 已改指新名）
    streak_mask_v2(OUT / 'fx_streak_mask2.png')
    flash_teardrop(OUT / 'fx_flash2.png')
    spark_dash(OUT / 'fx_spark_dash.png')
    selfcheck_p1(OUT / 'fx_streak_mask2.png', OUT / 'fx_flash2.png',
                 OUT / 'fx_spark_dash.png')

    # 同步一份到 pcksrc 目标目录（当前快照；正式进包由 build_lingchar.py
    # 在 assemble() 里从 assets/attack_fx 经 Godot 导入完成）。
    # fx_spark_strip9.png 已退役（条目 42），旧文件留在盘上由用户清理。
    PCK_DST.mkdir(parents=True, exist_ok=True)
    for name in ('fx_sparks3x3_grid.png', 'fx_streak_mask.png', 'fx_flash.png',
                 'fx_streak_mask2.png', 'fx_flash2.png', 'fx_spark_dash.png'):
        shutil.copy(OUT / name, PCK_DST / name)
        print('staged:', PCK_DST / name)

    shutil.rmtree(TMP)
    print('done.')


if __name__ == '__main__':
    main()
