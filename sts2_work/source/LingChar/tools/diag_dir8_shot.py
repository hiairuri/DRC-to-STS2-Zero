# -*- coding: utf-8 -*-
# 第八轮打回取证：截图像素测角（光束角=蓝掩码脊线拟合；枪管角=手工区段拟合）
# 截图: clipboard-2026-10-11T02-00-21-001Z-6f36393e.jpg（本地 10:00:21，godot.log 会话）
import numpy as np
from PIL import Image

SRC = r'C:/Users/jirai/.workbuddy/clipboard-images/clipboard-2026-10-11T02-00-21-001Z-6f36393e.jpg'
im = Image.open(SRC).convert('RGB')
a = np.asarray(im).astype(np.float32)
R, G, B = a[..., 0], a[..., 1], a[..., 2]

# --- 光束：蓝掩码（蓝显著高于红/绿），限 y<300 排除裙摆蓝色，x>330 排除枪体 ---
blue = (B > 120) & (B > R + 40) & (B > G + 30)
ys, xs = np.nonzero(blue)
keep = (xs > 330) & (ys < 300)
xs2, ys2 = xs[keep], ys[keep]
cols = {}
for x, y in zip(xs2, ys2):
    cols.setdefault(x, []).append(y)
cx = np.array(sorted(cols))
cy = np.array([np.median(cols[x]) for x in cx])
# 两轮稳健拟合（剔离群：远端火花簇向光束下方散开）
for it in range(3):
    k, c = np.polyfit(cx, cy, 1)
    res = np.abs(k * cx + c - cy)
    thr = max(1.5, np.percentile(res, 70))
    m = res < thr
    cx, cy = cx[m], cy[m]
k, c = np.polyfit(cx, cy, 1)
res2 = np.abs(k * cx + c - cy)
ang_beam = np.degrees(np.arctan(k))
print(f'光束角(屏幕y向下)={ang_beam:.2f}°  拟合列数={len(cx)} x跨度=[{cx.min()},{cx.max()}] '
      f'最大残差={res2.max():.2f}px')
# 分段角（前/中/后三段看是否直）
for lo, hi in [(0, 50), (25, 75), (50, 100)]:
    m = (cx >= np.percentile(cx, lo)) & (cx <= np.percentile(cx, hi))
    if m.sum() > 10:
        kk, _ = np.polyfit(cx[m], cy[m], 1)
        print(f'  分段 p{lo}-p{hi}: {np.degrees(np.arctan(kk)):.2f}° (n={int(m.sum())})')

# --- 枪管：暗像素 ASCII 辅助手选区段 ---
sub = a[190:265, 295:420]
lum = sub.mean(axis=2)
print('\n枪部区段 ASCII（行=y190..264, 列=x295..419, #=暗<90, o=中灰<140）:')
for i in range(sub.shape[0]):
    row = lum[i]
    line = ''.join('#' if row[j] < 90 else ('o' if row[j] < 140 else '.') for j in range(sub.shape[1]))
    if '#' in line or 'o' in line:
        print(f'y={190+i:3d} {line}')
