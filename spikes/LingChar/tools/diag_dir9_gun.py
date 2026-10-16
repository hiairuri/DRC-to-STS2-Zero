# -*- coding: utf-8 -*-
# 第十轮打回取证：枪管滑套中线自动拟合（diag_dir9_shot.py 配套）
# 滑套=高亮蓝白发光段：亮掩码（三通道均高且 B 最高）逐列取中位 y，
# 三轮稳健拟合（条目 51 顶缘法被散热筋污染的教训 → 本轮用整段中线）。
import numpy as np
from PIL import Image

SRC = r'C:/Users/jirai/.workbuddy/clipboard-images/clipboard-2026-10-11T03-55-55-757Z-c622b503.png'
im = Image.open(SRC).convert('RGB')
a = np.asarray(im).astype(np.float32)
R, G, B = a[..., 0], a[..., 1], a[..., 2]

# 滑套区域（枪口至握把上方，diag_dir9_gun_x8.png 人工勘定窗口，含 4px 余量）
X0, X1, Y0, Y1 = 300, 362, 145, 200
lum = (R + G + B) / 3
mask = (lum > 150) & (B >= R - 10) & (B >= G - 10)   # 亮且偏蓝白（排肤色：肤 R>B）
ys, xs = np.nonzero(mask)
win = (xs >= X0) & (xs < X1) & (ys >= Y0) & (ys < Y1)
xs, ys = xs[win], ys[win]
print(f'滑套亮掩码像素={len(xs)}  x=[{xs.min()},{xs.max()}]  y=[{ys.min()},{ys.max()}]')

cols = {}
for x, y in zip(xs, ys):
    cols.setdefault(x, []).append(y)
cx = np.array(sorted(cols))
cy = np.array([np.median(cols[x]) for x in cx])
for it in range(3):
    k, c = np.polyfit(cx, cy, 1)
    res = np.abs(k * cx + c - cy)
    m = res < max(1.5, np.percentile(res, 70))
    cx, cy = cx[m], cy[m]
k, c = np.polyfit(cx, cy, 1)
res2 = np.abs(k * cx + c - cy)
ang = np.degrees(np.arctan(k))
print(f'滑套中线角(屏幕y向下)={ang:.2f}°  拟合列数={len(cx)} x=[{cx.min()},{cx.max()}] 最大残差={res2.max():.2f}px')
# 上下缘互证（列内 min/max 各拟合一轮）
top = np.array([min(cols[x]) for x in sorted(cols)])
bot = np.array([max(cols[x]) for x in sorted(cols)])
cxx = np.array(sorted(cols))
for name, yy in (('上缘', top), ('下缘', bot)):
    kk, _ = np.polyfit(cxx, yy, 1)
    print(f'  {name}: {np.degrees(np.arctan(kk)):.2f}°')
