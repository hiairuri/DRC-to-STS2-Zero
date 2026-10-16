#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""对比锚点：光束峰值 vs 角色白衣峰值（判"亮核是否真的亮"）+ 光束 8x 放大 + 矩形/栅格检查。"""
from pathlib import Path
import numpy as np
from PIL import Image

P4 = r'C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T17-20-06-085Z-d9cd3f6f.jpg'
OUT = Path(__file__).resolve().parent
im = Image.open(P4).convert('RGB')
a = np.asarray(im).astype(np.float64)
L = 0.299 * a[..., 0] + 0.587 * a[..., 1] + 0.114 * a[..., 2]

# 白衣参考：中央区近白低饱和（角色躯干 x220-330, y230-420）
reg = a[230:420, 210:340]
Lr = 0.299 * reg[..., 0] + 0.587 * reg[..., 1] + 0.114 * reg[..., 2]
print('=== 白衣亮区参考 ===')
print(f'  mean={Lr.mean():.1f} p90={np.percentile(Lr,90):.1f} p99={np.percentile(Lr,99):.1f} max={Lr.max():.1f}')
print(f'  通道峰 R/G/B={reg[...,0].max():.0f}/{reg[...,1].max():.0f}/{reg[...,2].max():.0f}')

# 光束核心（x384-400, y215-245）
core = a[215:245, 382:400]
Lc = 0.299 * core[..., 0] + 0.587 * core[..., 1] + 0.114 * core[..., 2]
print('=== 光束核心 ===')
print(f'  mean={Lc.mean():.1f} p90={np.percentile(Lc,90):.1f} max={Lc.max():.1f}')
print(f'  通道峰 R/G/B={core[...,0].max():.0f}/{core[...,1].max():.0f}/{core[...,2].max():.0f}')

# 光束 8x 放大
c = im.crop((370, 200, 480, 265))
c.resize((c.width * 8, c.height * 8), Image.NEAREST).save(OUT / 'diag_shot4_crop_beam8x.png')
print(f'[crop] 光束 8x → diag_shot4_crop_beam8x.png {c.size}')

# 栅格/矩形边界检查：在光束右上方找"突然出现的矩形亮度"（halo 矩形边）
print('\n=== 光束周边矩形框检测（halo 是否留硬边） ===')
# 沿 y 扫描 x=395 列的 L，看是否有对称的台阶（halo 上下边）
for x in (388, 400, 430):
    colv = L[205:262, x]
    d = np.diff(colv)
    big = [(205 + i + 1, round(float(d[i]), 1)) for i in np.argsort(np.abs(d))[::-1][:4]]
    print(f'  x={x} 竖直 |ΔL| top4 (y, Δ): {big}')

# 水平方向（看光束远端是否有硬截止边）
for y in (230, 245):
    rowv = L[y, 380:500]
    d = np.diff(rowv)
    big = [(380 + i + 1, round(float(d[i]), 1)) for i in np.argsort(np.abs(d))[::-1][:5]]
    print(f'  y={y} 水平 |ΔL| top5 (x, Δ): {big}')

# 光束亮度沿长轴的剖面（从枪口到尖端）
print('\n=== 沿光束长轴亮度剖面（手采点 muzzle->tip） ===')
pts = [(390, 226), (398, 232), (406, 237), (414, 241), (422, 245), (430, 248), (438, 251), (446, 253)]
for (x, y) in pts:
    print(f'  ({x},{y}) L={L[y, x]:.0f}  RGB=({a[y,x,0]:.0f},{a[y,x,1]:.0f},{a[y,x,2]:.0f})')

# 全图最亮像素在哪（确认光束是否是最亮的）
ym, xm = np.unravel_index(np.argmax(L), L.shape)
print(f'\n全图最亮像素 L={L.max():.0f} at (x={xm},y={ym}) RGB=({a[ym,xm,0]:.0f},{a[ym,xm,1]:.0f},{a[ym,xm,2]:.0f})')
print(f'全图 L>=250 像素数={int((L>=250).sum())}  >=245={int((L>=245).sum())}')
ys, xs = np.where(L >= 250)
if len(xs):
    print(f'  L>=250 的分布 bbox=({xs.min()},{ys.min()},{xs.max()-xs.min()+1},{ys.max()-ys.min()+1})')
    # 分块统计
    print('  x 直方图(10段):', np.histogram(xs, bins=10, range=(0, 536))[0].tolist())
    print('  y 直方图(10段):', np.histogram(ys, bins=10, range=(0, 650))[0].tolist())
