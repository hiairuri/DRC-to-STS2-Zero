# -*- coding: utf-8 -*-
# 第十轮打回取证：截图像素测角（光束角=蓝掩码脊线拟合；枪管角=滑套中线人工选点）
# 截图: clipboard-2026-10-11T03-55-55-757Z-c622b503.png（UTC 03:55:55=本地 11:55:55，
#       对应 godot.log 活会话 11:49:58 启动，末次攻击 tick1@227.4s）
# 方法沿用条目 51 的 diag_dir8_shot.py / diag_dir8_shot2.py 管线，坐标自适应。
import numpy as np
from PIL import Image

SRC = r'C:/Users/jirai/.workbuddy/clipboard-images/clipboard-2026-10-11T03-55-55-757Z-c622b503.png'
im = Image.open(SRC).convert('RGB')
a = np.asarray(im).astype(np.float32)
R, G, B = a[..., 0], a[..., 1], a[..., 2]
H, W = B.shape
print(f'图像尺寸: {W}x{H}')

# --- 光束：蓝掩码（蓝显著高于红/绿），先看全图分布再定窗口 ---
blue = (B > 120) & (B > R + 40) & (B > G + 30)
ys, xs = np.nonzero(blue)
print(f'蓝掩码像素数={len(xs)}  x范围=[{xs.min()},{xs.max()}]  y范围=[{ys.min()},{ys.max()}]')
# 按列直方看光束主体（避开角色身体上的蓝色花饰/眼睛等）
hist = {}
for x, y in zip(xs, ys):
    hist.setdefault(x, []).append(y)
colw = sorted(hist)
print('列分布采样（每 20 列）:', [(x, len(hist[x])) for x in colw[::20]])

# 光束主体判定：头花簇（列内 10-22px 密集块）与光束（窄条 2-8px/列）混在蓝掩码里。
# 几何先验：枪口/手部在 x≈296-306（rich 列二次抬升），光束向右上延伸——
# 取 x>260（手部右侧）的蓝掩码做光束拟合，并用列内 y 连续游程 ≤10px 的窄条约束。
xs2 = xs[xs > 260]
ys2 = ys[xs > 260]
cols0 = {}
for x, y in zip(xs2, ys2):
    cols0.setdefault(x, []).append(y)
# 列内窄条约束：只保留与该列中位 y 差 ≤6px 的像素（剔裙摆蓝边与远端火花）
cols = {}
for x, yl in cols0.items():
    ymed = np.median(yl)
    keep = [y for y in yl if abs(y - ymed) <= 6]
    if keep:
        cols[x] = keep
cx = np.array(sorted(cols))
cy = np.array([np.median(cols[x]) for x in cx])
# 三轮稳健拟合（剔离群：光晕污染与火花簇）
for it in range(3):
    k, c = np.polyfit(cx, cy, 1)
    res = np.abs(k * cx + c - cy)
    thr = max(1.5, np.percentile(res, 70))
    m = res < thr
    cx, cy = cx[m], cy[m]
k, c = np.polyfit(cx, cy, 1)
res2 = np.abs(k * cx + c - cy)
ang_beam = np.degrees(np.arctan(k))
print(f'\n光束角(屏幕y向下)={ang_beam:.2f}°  拟合列数={len(cx)} x跨度=[{cx.min()},{cx.max()}] '
      f'最大残差={res2.max():.2f}px')
for lo2, hi2 in [(0, 50), (25, 75), (50, 100)]:
    m = (cx >= np.percentile(cx, lo2)) & (cx <= np.percentile(cx, hi2))
    if m.sum() > 10:
        kk, _ = np.polyfit(cx[m], cy[m], 1)
        print(f'  分段 p{lo2}-p{hi2}: {np.degrees(np.arctan(kk)):.2f}° (n={int(m.sum())})')

# --- 枪部放大导出（光束起点附近的枪体区域，×8 供人工滑套中线选点） ---
x0 = int(cx.min())
gx0, gy0 = max(0, x0 - 110), max(0, int(k * x0 + c) - 80)
gx1, gy1 = min(W, x0 + 60), min(H, int(k * x0 + c) + 60)
crop = im.crop((gx0, gy0, gx1, gy1))
scale = 8
crop = crop.resize((crop.width * scale, crop.height * scale), Image.NEAREST)
out = r'D:/项目/dragonraja/spikes/LingChar/tools/diag_dir9_gun_x8.png'
crop.save(out)
print(f'\n已导出 {out}（对应原图 x{gx0}..{gx1}, y{gy0}..{gy1}，×{scale}）')
