# -*- coding: utf-8 -*-
"""历史 PNG（无损）终判：y=180→181 接缝的逐通道比值，多区域采样。
若 G/B 比值在各独立区域近似同一常数 → 全屏黑色叠加层（α=1-k）。
PNG 无 JPEG 色度噪声，R 通道可信。"""
import numpy as np
from PIL import Image

HIS = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T09-08-36-813Z-d8b15f17.png"
his = np.asarray(Image.open(HIS).convert("RGB")).astype(np.float64)
hH, hW, _ = his.shape
print("history", his.shape)

# 精确定位：全宽行剖面
prof = his.mean(axis=1)
lum = prof.mean(axis=1)
d = np.diff(lum)
top = np.argsort(np.abs(d))[::-1][:6]
print("全宽最大行跳变:", [(int(i), round(float(d[i]), 2)) for i in sorted(top)])

yE = 180  # 台阶行（180->181）
segs = [(8, 90, "左墙"), (95, 135, "左发"), (140, 175, "衣袖上"), (300, 345, "右侧bg"), (350, 395, "光斑右"), (455, 540, "右墙")]
print(f"\n== 历史 y={yE}->{yE+1} 接缝逐通道比值（above=y{yE-8}..{yE-1}, below=y{yE+2}..{yE+9}） ==")
print(f"{'区域':>8} {'above RGB':>20} {'below RGB':>20} {'k=R/G/B':>22}")
ks = []
for x0, x1, tag in segs:
    a = his[yE-8:yE, x0:x1, :].mean(axis=(0, 1))
    b = his[yE+2:yE+10, x0:x1, :].mean(axis=(0, 1))
    k = a / np.maximum(b, 1e-6)
    ks.append((tag, k, a.mean(), b.mean()))
    print(f"{tag:>8} ({a[0]:6.1f},{a[1]:6.1f},{a[2]:6.1f}) ({b[0]:6.1f},{b[1]:6.1f},{b[2]:6.1f}) ({k[0]:.3f},{k[1]:.3f},{k[2]:.3f})")

# 一致性统计（排除亮斑区）
print("\n各区域 G 通道 k:", [f"{k[1]:.3f}" for _, k, _, _ in ks])
print("各区域 B 通道 k:", [f"{k[2]:.3f}" for _, k, _, _ in ks])

# 逐列 G 通道 k 的 x 一致性（左墙+右墙 bg 区）
for x0, x1, tag in [(8, 90, "左bg"), (455, 540, "右bg")]:
    a = his[yE-8:yE, x0:x1, :].mean(axis=0)
    b = his[yE+2:yE+10, x0:x1, :].mean(axis=0)
    kg = a[:, 1] / np.maximum(b[:, 1], 1e-6)
    print(f"[{tag}] G通道逐列k: 中位={np.median(kg):.3f} IQR={np.percentile(kg,75)-np.percentile(kg,25):.3f}")

# 台阶是否全宽存在：逐 40px 段的 G 通道 step
print("\n== G 通道 step 全宽扫描（below-above） ==")
for x0 in range(0, hW - 40, 40):
    a = his[yE-8:yE, x0:x0+40, 1].mean()
    b = his[yE+2:yE+10, x0:x0+40, 1].mean()
    print(f"x{x0:3d}-{x0+40:3d} dG={b-a:+6.2f}", end="  ")
print()
