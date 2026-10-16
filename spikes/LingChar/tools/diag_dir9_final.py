# -*- coding: utf-8 -*-
# 第十轮打回取证·定稿：光束角 + 枪管滑套中线角（diag_dir9_shot/gun 精化版）
# 截图: clipboard-2026-10-11T03-55-55-757Z-c622b503.png（本地 11:55:55，
#       活会话=godot.log 11:49:58 启动；日志七拍 tick1 全部 sprrot=0 n2rot=0 fix=0）
# 结构勘定（diag_dir9_muzzle_grid.png ×10 网格目视）：
#   滑套=左侧蓝白发光长条，从 (255,190) 到枪口 (300,143)；
#   光束=从枪口向右上的主体（x>303），顶端 x≈400 后变细（远端光晕）；
#   x<300 与滑套重叠区不进光束拟合，避免滑套污染（本轮教训）。
import numpy as np
from PIL import Image

SRC = r'C:/Users/jirai/.workbuddy/clipboard-images/clipboard-2026-10-11T03-55-55-757Z-c622b503.png'
a = np.asarray(Image.open(SRC).convert('RGB')).astype(np.float32)
R, G, B = a[..., 0], a[..., 1], a[..., 2]
lum = (R + G + B) / 3

def robust_fit(xs, ys, iters=4, q=60, floor=1.2):
    cols = {}
    for x, y in zip(xs, ys):
        cols.setdefault(x, []).append(y)
    cx = np.array(sorted(cols))
    cy = np.array([np.median(cols[x]) for x in cx])
    for _ in range(iters):
        k, c = np.polyfit(cx, cy, 1)
        res = np.abs(k * cx + c - cy)
        m = res < max(floor, np.percentile(res, q))
        cx, cy = cx[m], cy[m]
    k, c = np.polyfit(cx, cy, 1)
    res = np.abs(k * cx + c - cy)
    return np.degrees(np.arctan(k)), len(cx), (cx.min(), cx.max()), res.max()

# --- 光束：亮核心掩码（B 显著高于 R，排白裙），窗口 x∈[303,395]（枪口右侧主体） ---
core = (lum > 120) & (B > R + 25)
ys, xs = np.nonzero(core)
m = (xs >= 303) & (xs <= 395) & (ys < 250)
ang_b, n_b, span_b, res_b = robust_fit(xs[m], ys[m])
print(f'光束角={ang_b:.2f}°  n={n_b}  x跨度={span_b}  最大残差={res_b:.2f}px')

# --- 滑套：亮掩码（蓝白发光），窗口 x∈[252,302], y∈[130,200]；排肤色（手 R>B）与光束亮团 ---
mask = (lum > 110) & (B >= R - 5)
ys, xs = np.nonzero(mask)
m = (xs >= 252) & (xs <= 302) & (ys >= 130) & (ys <= 200)
ang_g, n_g, span_g, res_g = robust_fit(xs[m], ys[m], iters=3)
print(f'滑套中线角={ang_g:.2f}°  n={n_g}  x跨度={span_g}  最大残差={res_g:.2f}px')

print(f'\n像素残差（光束−滑套）={ang_b - ang_g:+.2f}°  （正=光束比枪管仰）')
print(f'对账：理论残差=+15.27(C#屏上相对角)−23.08(γ_meas 渲染枪管相对角)=−7.81°（代数口径，')
print(f'      即光束比枪管仰 7.81°；实测仰 {ang_g - ang_b:.2f}°，含测量不确定度 ±1.5°）')
