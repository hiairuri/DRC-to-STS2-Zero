# -*- coding: utf-8 -*-
"""
diag_dir7d.py —— 第七轮取证复测：光束角（最大连通域）+ 轴线可视化核验
======================================================================
上一版 beam PCA 把远端离散火花一并计入掩码，主轴被拉偏。本版：
1. 掩码后取最大 8 连通域（光束连续主体），剔除离散火花；
2. 用连通域像素质心+协方差 PCA 求主轴，同时对掩码做逐列中位数拟合复核；
3. 把光束轴/枪管滑套拟合线画到截图副本上，供人工核验（落盘 diag_dir7_*_annot.png）。
"""
import math
import numpy as np
from PIL import Image, ImageDraw

SHOTS = {
    "new": (r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T21-01-14-817Z-efc90cb2.jpg",
            (418, 476), -5.0),
    "old": (r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T20-03-38-018Z-c15ba30c.jpg",
            (380, 434), -1.8),
}

def largest_component(mask):
    """8 邻接最大连通域（迭代泛洪，避免递归深度）"""
    from collections import deque
    lab = np.zeros(mask.shape, dtype=np.int32)
    best, best_id, cur = 0, 0, 0
    H, W = mask.shape
    for y in range(H):
        for x in range(W):
            if mask[y, x] and lab[y, x] == 0:
                cur += 1
                q = deque([(y, x)])
                lab[y, x] = cur
                n = 0
                while q:
                    cy, cx = q.popleft()
                    n += 1
                    for dy in (-1, 0, 1):
                        for dx in (-1, 0, 1):
                            ny, nx = cy + dy, cx + dx
                            if 0 <= ny < H and 0 <= nx < W and mask[ny, nx] and lab[ny, nx] == 0:
                                lab[ny, nx] = cur
                                q.append((ny, nx))
                if n > best:
                    best, best_id = n, cur
    return lab == best_id, best

for key, (path, (x1, x2), barrel_ang) in SHOTS.items():
    im = Image.open(path).convert("RGB")
    W, H = im.size
    a = np.asarray(im, dtype=np.int32)
    r, g, b = a[..., 0], a[..., 1], a[..., 2]
    box = (int(W * 0.55), 0, W, int(H * 0.55))
    m = np.zeros((H, W), dtype=bool)
    m[box[1]:box[3], box[0]:box[2]] = True
    mask = m & (b > 140) & (b - r > 40) & (g > 60)
    comp, n = largest_component(mask)
    ys, xs = np.nonzero(comp)
    cx, cy = xs.mean(), ys.mean()
    cov = np.cov(np.stack([xs - cx, ys - cy]))
    w, v = np.linalg.eigh(cov)
    main = v[:, np.argmax(w)]
    ang_pca = math.degrees(math.atan2(main[1], main[0]))
    if ang_pca > 90: ang_pca -= 180
    if ang_pca < -90: ang_pca += 180
    # 复核：连通域逐列中位 y 拟合
    cols = {}
    for xx, yy in zip(xs, ys):
        cols.setdefault(xx, []).append(yy)
    fx = np.array(sorted(cols))
    fy = np.array([np.median(cols[x]) for x in fx])
    k, c = np.polyfit(fx, fy, 1)
    ang_fit = math.degrees(math.atan(k))
    print(f"=== {key} === 光束连通域 {n}px  PCA={ang_pca:.1f}°  逐列中位拟合={ang_fit:.1f}°  枪管={barrel_ang}°")
    # 可视化：画光束轴（红）与滑套线（绿）
    dr = ImageDraw.Draw(im)
    L = 400
    # 光束轴过质心（截图坐标）
    x0, y0 = cx + box[0], cy + box[1]
    dr.line([(x0 - L * math.cos(math.radians(ang_pca)), y0 - L * math.sin(math.radians(ang_pca))),
             (x0 + L * math.cos(math.radians(ang_pca)), y0 + L * math.sin(math.radians(ang_pca)))],
            fill=(255, 0, 0), width=2)
    # 滑套线：y = k_s*(x-x1)+y1，取滑套拟合角
    ks = math.tan(math.radians(barrel_ang))
    # 滑套线过区段中点上方：取枪身盒中心 y 估计
    dr.line([(x1, 300 + ks * 0), (x2, 300 + ks * (x2 - x1))], fill=(0, 255, 0), width=2)
    im.save(rf"D:\项目\dragonraja\spikes\LingChar\tools\diag_dir7_{key}_annot.png")
    print(f"  标注图落盘 diag_dir7_{key}_annot.png（红=光束轴 绿=滑套线）")
