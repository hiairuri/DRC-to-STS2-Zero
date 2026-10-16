#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""LingChar 命中特效纹理加工（条目 57 · fx_ling_attack_01_hit → STS2 移植）。

三链路取值铁律：所有参数来自源码/资产(uasset·uexp)/运行时 hook，不截图反解。
本脚本走"资产"链路——把 UE 粒子纹理真值转成 Godot 可用的 PNG（5 张，全部
LingChar 自有路径，DESIGN.md §6.1 资产清单）：

  1) hitfx_flare.png (512²)  <- hitfx_research_2026-10-11/textures/
     ray_tex_flare_03.png 直拷改名（E3 白星芒；RGB 星芒+黑底，加算混合下
     黑底零贡献，alpha 恒 255 不影响——UE OSM_RedChannel 的 2D 等价物）
  2) hitfx_ray.png (256²)    <- 同目录 ray_t_fx_ray01.png 直拷改名（E4 射线）
  3) hitfx_glow2x2.png (512²) <- 同目录 frame_fr_glow_902_2x2.png
     **alpha 归一化 ×(255/139)=1.83**（A5：原纹理 alpha max 仅 139/255，
     UE 靠 ColorScale 10 提亮——设计把"UE 靠 HDR 提亮"翻译成"LDR 纹理
     提亮+满 alpha"，资产侧一次做完，原 PNG 只读；RGB 不动）
  4) hitfx_ring.png (512²)   程序生成（E0/E1 环盘）：FX_Circle 网格与
     Mi_Ling_Skill 材质真值全仓缺失（README §5 缺口）→ 程序生成+【假设 A2】
     豁免声明。径向剖面：内盘 ≈0.25、rim 高斯峰 r≈0.85、外缘 smoothstep
     平滑落 0；RGB 纯白（E0 的色曲线/E1 的白由 C# modulate 承担——条目 45 ★
     "颜色应用两次"教训：纹理只承载形状，不烘焙基色）
  5) hitfx_streak.png (16×160) 程序生成（E7 火星丝）：真值宽 0.44-0.87px
     亚像素不可见（A4）→ 豁免 16px 宽 10:1 细软条，均匀缩放下宽 4px→长 40px

程序生成豁免声明（条目 44 定案先例，适用于 4)/5)）：UE 转储中不存在可解码的
空间 alpha 渐变纹理（渐变在材质 Mi_Ling_Skill 的 UV 动画里，未移植），只能
程序生成合成近似——标注【假设 A2】/【豁免 A4】；形状自检（四边 alpha=0 等）
沿用条目 44⑤/55② 全部自检规则。

自检项（DESIGN §6.2）：ring 四边 alpha=0 + 剖面（中心≈0.25/rim 峰 r≈0.85/边缘 0）、
glow 归一化后 alpha max=255（≠源 139）、ray/flare IHDR 尺寸断言、streak 四边
alpha=0 + 峰值。

去污染铁律：产物只写 LingChar 自有路径（assets/hit_fx 持久源 + pcksrc 当前
快照同步），绝不写回 hitfx_research_2026-10-11（只读解包树）或
LingSilentSkin_battle/pcksrc（皮肤源树）。

用法（需 pillow/numpy）:
  "C:/Program Files/Python310/python.exe" make_hit_fx_textures.py
"""
import math
import shutil
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent      # LingChar 仓库根
OUT = ROOT / 'assets/hit_fx'                        # 持久源（build 从这里导入）
PCK_DST = ROOT / 'pcksrc/drc_dye/profiles/ling/animations/characters/ling'
SRC = Path(r'D:\项目\dragonraja\sts2_work\hitfx_research_2026-10-11\textures')

# 真值源（只读解包树，允许拷贝复制）
SRC_FLARE = SRC / 'ray_tex_flare_03.png'
SRC_RAY = SRC / 'ray_t_fx_ray01.png'
SRC_GLOW = SRC / 'frame_fr_glow_902_2x2.png'

GLOW_ALPHA_MAX_EXPECT = 139   # 源纹理实测（2026-10-11 真值报告 A5）
RING_R = 246.0                # 环盘半径 px（512² 内四边留 ≥10px 余量）


def clamp01(x: float) -> float:
    return 0.0 if x < 0.0 else (1.0 if x > 1.0 else x)


def smoothstep(a: float, b: float, x: float) -> float:
    t = clamp01((x - a) / (b - a))
    return t * t * (3.0 - 2.0 * t)


def copy_tex(src: Path, dst: Path, expect_w: int, expect_h: int) -> None:
    """真值 PNG 直拷改名 + IHDR 尺寸断言（源只读，dst 为 LingChar 自有产物）。"""
    from PIL import Image
    shutil.copyfile(src, dst)
    im = Image.open(dst)
    assert im.size == (expect_w, expect_h), \
        f'{src.name}: 尺寸 {im.size} != 预期 ({expect_w},{expect_h})'
    print(f'copy: {src.name} {im.size[0]}x{im.size[1]} -> {dst.name}')


def glow_normalized(dst: Path) -> None:
    """fr_glow_902_2x2 alpha 归一化 ×(255/139)（A5）：RGB 不动，alpha 拉满。"""
    from PIL import Image
    import numpy as np
    im = Image.open(SRC_GLOW).convert('RGBA')
    arr = np.asarray(im, dtype=np.float32).copy()
    a_max = float(arr[..., 3].max())
    assert round(a_max) == GLOW_ALPHA_MAX_EXPECT, \
        f'源 alpha max={a_max} != 预期 {GLOW_ALPHA_MAX_EXPECT}（源被更新？停下核对）'
    scale = 255.0 / a_max
    arr[..., 3] = np.clip(arr[..., 3] * scale, 0.0, 255.0)
    out = Image.fromarray(arr.astype(np.uint8), 'RGBA')
    out.save(dst, optimize=True)
    a2 = int(np.asarray(out)[..., 3].max())
    print(f'glow normalized: {SRC_GLOW.name} alpha max {int(a_max)} -> {a2} '
          f'(x{scale:.4f}，A5；RGB 未动)')
    assert a2 == 255, '归一化后 alpha max 必须=255'


def _ring_profile(t: float) -> float:
    """环盘径向剖面（t=r/R 归一化）：内盘 0.25 平台（r<0.5 起 smooth 落）+
    rim 高斯峰（中心 r=0.85，σ=0.055）+ 外缘 smoothstep(0.90,1.0) 平滑落 0。
    峰值处（t=0.85）：disc 已归零、rim=1.0；中心 t=0：0.25；t≥1：0。"""
    disc = 0.25 * (1.0 - smoothstep(0.50, 0.78, t))
    rim = math.exp(-((t - 0.85) / 0.055) ** 2)
    outer = 1.0 - smoothstep(0.90, 1.00, t)
    return min(1.0, disc + rim) * outer


def ring_tex(dst: Path) -> None:
    """程序生成 512² 环盘（E0/E1，【假设 A2】豁免：FX_Circle 网格底尺寸与
    Mi_Ling_Skill 材质全仓缺失，README §5；形状=软边圆环+微亮内盘，DESIGN §6.1）。
    RGB 纯白（色曲线由 C# modulate 承担，防"颜色应用两次"）；四边 alpha=0
    （R=246 < 256，外缘剖面在 t≥1 处归零 + 边界强制归零双保险）；
    alpha=0 处 RGB 仍填白（防 LINEAR 采样拉暗，条目 55② 思路）。"""
    from PIL import Image
    W = H = 512
    c = (W - 1) / 2.0
    im = Image.new('RGBA', (W, H))
    px = im.load()
    for y in range(H):
        dy = y - c
        for x in range(W):
            dx = x - c
            t = math.hypot(dx, dy) / RING_R
            a = _ring_profile(t) if t < 1.0 else 0.0
            if x == 0 or x == W - 1 or y == 0 or y == H - 1:
                a = 0.0  # 四边硬归零（防硬边/上下亮线，条目 45 根修先例）
            px[x, y] = (255, 255, 255, round(255 * a))
    im.save(dst, optimize=True)
    print(f'ring: {W}x{H} procedural（【假设 A2】程序生成环盘，RGB 白）')


def streak_tex(dst: Path) -> None:
    """程序生成 16×160 细软光条（E7，【豁免 A4】：真值宽 0.44-0.87px 亚像素
    不可见）。长轴=纵向（局部 +Y==速度方向，particle_flag_align_y 载体）；
    横向 t^1.5 高斯式软边；纵向 smoothstep 起坡 + (1-v)^0.7 尾部渐弱；
    RGB 纯白（色曲线 (0.479,0.713,1)→(0.752,0.661,1) 由 C# ColorRamp 承担）；
    四边 alpha=0。"""
    from PIL import Image
    W, H = 16, 160
    im = Image.new('RGBA', (W, H))
    px = im.load()
    for y in range(H):
        v = (y + 0.5) / H
        along = smoothstep(0.0, 0.06, v) * (1.0 - v) ** 0.7
        for x in range(W):
            u = (x + 0.5) / W
            t = 1.0 - abs(2.0 * u - 1.0)
            a = along * (t ** 1.5)
            if x == 0 or x == W - 1 or y == 0 or y == H - 1:
                a = 0.0  # 四边硬归零
            px[x, y] = (255, 255, 255, round(255 * a))
    im.save(dst, optimize=True)
    print(f'streak: {W}x{H} procedural（【豁免 A4】10:1 细软条，RGB 白）')


def selfcheck(out_dir: Path) -> None:
    """离线剖面校验（DESIGN §6.2 内置 selfcheck 清单）。"""
    from PIL import Image
    import numpy as np

    # ring：四边 alpha=0；+x 剖面中心≈0.25、rim 峰 r≈0.85、t=1 处 0
    im = Image.open(out_dir / 'hitfx_ring.png').convert('RGBA')
    a = np.asarray(im)[..., 3].astype(np.int32)
    border = max(int(a[0, :].max()), int(a[-1, :].max()),
                 int(a[:, 0].max()), int(a[:, -1].max()))
    print(f'[selfcheck] ring 四边 alpha 最大值 = {border}（应 0）')
    assert border == 0, 'ring 四边 alpha 必须为 0'
    c = (im.width - 1) / 2.0
    prof = [int(a[int(round(c)), int(round(c + r * RING_R))]) for r in
            (0.0, 0.25, 0.5, 0.7, 0.85, 0.95, 1.0)]
    print(f'[selfcheck] ring 中心列剖面 t=0/0.25/0.5/0.7/0.85/0.95/1.0 = {prof}')
    assert 0.15 <= prof[0] / 255 <= 0.35, f'ring 中心（内盘）应≈0.25，实测 {prof[0] / 255:.3f}'
    assert prof[4] >= 230, f'ring rim 峰（t=0.85）应≈1.0，实测 {prof[4] / 255:.3f}'
    assert prof[-1] == 0, 'ring 外缘（t=1.0）alpha 必须 0'
    # rim 峰位置：+x 方向 alpha 最大值所在 t 应在 0.85±0.04
    ts = np.arange(0.0, 1.0, 0.002)
    vals = [a[int(round(c)), min(int(round(c + t * RING_R)), im.width - 1)] for t in ts]
    t_peak = float(ts[int(np.argmax(vals))])
    print(f'[selfcheck] ring rim 峰位置 t={t_peak:.3f}（应 ≈0.85±0.04）')
    assert abs(t_peak - 0.85) <= 0.04, f'rim 峰位置偏移过大: {t_peak:.3f}'

    # streak：四边 alpha=0；中心列峰值 ≥0.8
    im2 = Image.open(out_dir / 'hitfx_streak.png').convert('RGBA')
    a2 = np.asarray(im2)[..., 3].astype(np.int32)
    border2 = max(int(a2[0, :].max()), int(a2[-1, :].max()),
                  int(a2[:, 0].max()), int(a2[:, -1].max()))
    print(f'[selfcheck] streak {im2.size[0]}x{im2.size[1]} 四边 alpha 最大值 = {border2}（应 0）')
    assert border2 == 0, 'streak 四边 alpha 必须为 0'
    peak = int(a2[:, im2.width // 2].max())
    print(f'[selfcheck] streak 中心列峰值 alpha = {peak / 255:.3f}（应 ≥0.80）')
    assert peak / 255 >= 0.80, 'streak 峰值 alpha 不足'

    # glow：归一化断言（包外产物层——包内断言在 verify_pck_hitfx.py）
    im3 = Image.open(out_dir / 'hitfx_glow2x2.png').convert('RGBA')
    a3 = np.asarray(im3)[..., 3]
    print(f'[selfcheck] glow alpha max = {int(a3.max())}（应 255，源 139）')
    assert int(a3.max()) == 255

    # flare/ray：尺寸断言（直拷产物）
    for name, wh in (('hitfx_flare.png', (512, 512)), ('hitfx_ray.png', (256, 256))):
        im4 = Image.open(out_dir / name)
        assert im4.size == wh, f'{name} 尺寸 {im4.size} != {wh}'
        print(f'[selfcheck] {name} 尺寸 = {im4.size[0]}x{im4.size[1]} ✓')


def main() -> None:
    assert SRC.exists(), f'真值纹理目录缺失（只读解包树）: {SRC}'
    OUT.mkdir(parents=True, exist_ok=True)

    # 1) E3 星芒：直拷改名（RGB 星芒+黑底，加算下黑底零贡献）
    copy_tex(SRC_FLARE, OUT / 'hitfx_flare.png', 512, 512)
    # 2) E4 射线：直拷改名
    copy_tex(SRC_RAY, OUT / 'hitfx_ray.png', 256, 256)
    # 3) E8 柔光：alpha 归一化 ×(255/139)（A5）
    glow_normalized(OUT / 'hitfx_glow2x2.png')
    # 4) E0/E1 环盘：程序生成（A2）
    ring_tex(OUT / 'hitfx_ring.png')
    # 5) E7 火星丝：程序生成（A4）
    streak_tex(OUT / 'hitfx_streak.png')

    selfcheck(OUT)

    # 同步一份到 pcksrc 当前快照（正式进包由 build_lingchar.py 在 assemble()
    # 里从 assets/hit_fx 经 Godot 导入完成，与本脚本的 staging 互不依赖）。
    PCK_DST.mkdir(parents=True, exist_ok=True)
    for name in ('hitfx_flare.png', 'hitfx_ray.png', 'hitfx_glow2x2.png',
                 'hitfx_ring.png', 'hitfx_streak.png'):
        shutil.copy(OUT / name, PCK_DST / name)
        print('staged:', PCK_DST / name)
    print('done.')


if __name__ == '__main__':
    main()
