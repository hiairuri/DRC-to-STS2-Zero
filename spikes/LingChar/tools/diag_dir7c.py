# -*- coding: utf-8 -*-
"""
diag_dir7c.py —— 第七轮取证：枪管滑套顶缘直线拟合
====================================================
人工指定 x 区段（避开准星/锤头），对每列取"最上方的枪体亮像素"，最小二乘拟合直线。
输出：两截图枪管轴角 + 与光束角角差。
"""
import math
import numpy as np
from PIL import Image

SHOTS = {
    "new": (r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T21-01-14-817Z-efc90cb2.jpg",
            (418, 476), -24.9),
    "old": (r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T20-03-38-018Z-c15ba30c.jpg",
            (380, 434), -36.3),
}
# 亮度阈值：枪体（亮蓝/白/灰）vs 深色背景
LUM_TH = 80

for key, (path, (x1, x2), beam_ang) in SHOTS.items():
    a = np.asarray(Image.open(path).convert("RGB"), dtype=np.int32)
    lum = a.mean(axis=2)
    pts = []
    for x in range(x1, x2 + 1):
        col = lum[:, x]
        # 在枪身所在 y 邻域内找最上亮像素（限制 y<400 避开白裙误拾）
        for y in range(120, 400):
            if col[y] > LUM_TH and col[y + 1] > LUM_TH and col[y + 2] > LUM_TH:
                pts.append((x, y))
                break
    pts = np.array(pts, dtype=np.float64)
    # 拟合 y = k*x + c
    k, c = np.polyfit(pts[:, 0], pts[:, 1], 1)
    ang = math.degrees(math.atan2(k, 1.0))
    # 拟合残差（px）
    resid = np.abs(pts[:, 1] - (k * pts[:, 0] + c)).max()
    print(f"=== {key} ===")
    print(f"  枪管轴角(滑套顶缘) = {ang:.1f}°  ({len(pts)} 列, 最大残差 {resid:.1f}px)")
    print(f"  光束轴角           = {beam_ang:.1f}°")
    print(f"  角差(枪管-光束)    = {ang - beam_ang:.1f}°")
