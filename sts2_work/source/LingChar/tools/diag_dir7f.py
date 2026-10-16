# -*- coding: utf-8 -*-
"""
diag_dir7f.py —— 光束脊线精测（排除枪口闪/火花污染）
======================================================
只取光束直线核心段：人工给定起止走廊，逐列沿垂直方向找亮度峰值（脊线），
最小二乘拟合脊线角度。输出两截图光束精确角 + 与枪管角差。
"""
import math
import numpy as np
from PIL import Image

# 走廊参数：每截图给光束核心段两端点（人工目视选定，避开 flash 团与火花）与走廊半宽
CONFIG = {
    "new": {
        "path": r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T21-01-14-817Z-efc90cb2.jpg",
        "p0": (512, 228), "p1": (648, 186), "half": 14,   # 核心段（避开尾部 flash 团与头部火花）
        "barrel": -5.0,
    },
    "old": {
        "path": r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T20-03-38-018Z-c15ba30c.jpg",
        "p0": (462, 172), "p1": (575, 97), "half": 14,
        "barrel": -1.8,
    },
}

for key, c in CONFIG.items():
    a = np.asarray(Image.open(c["path"]).convert("RGB"), dtype=np.float32)
    lum = a[..., 0] * 0.3 + a[..., 1] * 0.45 + a[..., 2] * 0.25  # 偏蓝亮度
    p0, p1, half = np.array(c["p0"], float), np.array(c["p1"], float), c["half"]
    d = p1 - p0
    L = np.hypot(*d)
    u = d / L                      # 轴向单位
    n = np.array([-u[1], u[0]])    # 法向
    xs, ys, ws = [], [], []
    for t in np.linspace(0.15, 0.85, 60):  # 中段 70%，避开两端
        base = p0 + d * t
        # 沿法向采样找亮度峰值（亚像素抛物线插值）
        offs = np.arange(-half, half + 1)
        pts = base[None, :] + offs[:, None] * n[None, :]
        vals = np.array([lum[int(round(py)), int(round(px))]
                         for px, py in pts])
        i = int(np.argmax(vals))
        if i <= 0 or i >= len(offs) - 1:
            continue
        # 抛物线顶点插值
        y0, y1, y2 = vals[i - 1], vals[i], vals[i + 1]
        den = (y0 - 2 * y1 + y2)
        delta = 0.5 * (y0 - y2) / den if abs(den) > 1e-6 else 0.0
        best = offs[i] + delta
        pt = base + best * n
        xs.append(pt[0]); ys.append(pt[1]); ws.append(y1)
    xs, ys, ws = np.array(xs), np.array(ys), np.array(ws)
    # 加权最小二乘 y = k x + c
    W = ws / ws.sum()
    xm, ym = (xs * W).sum(), (ys * W).sum()
    k = ((xs - xm) * W * (ys - ym)).sum() / ((xs - xm) ** 2 * W).sum()
    ang = math.degrees(math.atan(k))
    resid = np.abs(ys - (k * xs + (ym - k * xm))).mean()
    print(f"=== {key} === 光束脊线角 = {ang:.2f}°  (n={len(xs)}, 平均残差 {resid:.2f}px)")
    print(f"  枪管角 = {c['barrel']:.1f}°   角差(光束-枪管) = {ang - c['barrel']:.2f}°")
