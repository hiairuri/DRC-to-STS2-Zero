# -*- coding: utf-8 -*-
"""Q2 终判：左墙逐列比值 k(x) 拟合（恒定=黑色叠加层）+ 三通道 k + 人物胸区比值。"""
import numpy as np
from PIL import Image

CUR = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T18-05-27-954Z-d9cd3f6f.jpg"
cur = np.asarray(Image.open(CUR).convert("RGB")).astype(np.float64)

# 左墙 x10..100：above=y208..216, below=y222..230（紧贴边缘、避开过渡行217-221）
above = cur[206:216, 10:100, :].mean(axis=0)   # (90,3)
below = cur[222:232, 10:100, :].mean(axis=0)
lum_a = above.mean(axis=1); lum_b = below.mean(axis=1)

# 逐列亮度比值
k = lum_a / np.maximum(lum_b, 1e-6)
print("== 左墙逐列 k=above/below（亮度） ==")
print(f"k 中位={np.median(k):.3f} 均值={k.mean():.3f} std={k.std():.3f} IQR={np.percentile(k,75)-np.percentile(k,25):.3f}")
print("逐列 k:", " ".join(f"{v:.2f}" for v in k[::5]))

# 回归：above = k*below + c（若 c≈0 且 k 恒定 → 纯乘法）
A = np.vstack([lum_b, np.ones_like(lum_b)]).T
sol, res, *_ = np.linalg.lstsq(A, lum_a, rcond=None)
pred = A @ sol
r2 = 1 - ((lum_a - pred)**2).sum() / ((lum_a - lum_a.mean())**2).sum()
print(f"\n回归 above = {sol[0]:.3f}*below + {sol[1]:.3f}   R²={r2:.3f}")

# 三通道 k
print("\n== 三通道 k（左墙） ==")
for c, ch in enumerate("RGB"):
    kc = above[:, c] / np.maximum(below[:, c], 1e-6)
    print(f"{ch}: 中位={np.median(kc):.3f} IQR={np.percentile(kc,75)-np.percentile(kc,25):.3f}")

# 人物胸区 x215..275（白裙平滑区）
above_c = cur[210:217, 215:275, :].mean(axis=(0,1))
below_c = cur[222:229, 215:275, :].mean(axis=(0,1))
kc = above_c / np.maximum(below_c, 1e-6)
print(f"\n== 人物胸区 above/below ==")
print(f"above=({above_c[0]:.1f},{above_c[1]:.1f},{above_c[2]:.1f}) below=({below_c[0]:.1f},{below_c[1]:.1f},{below_c[2]:.1f}) k=({kc[0]:.3f},{kc[1]:.3f},{kc[2]:.3f})")

# 右墙 x480..530 同样回归
above_r = cur[206:216, 480:530, :].mean(axis=0)
below_r = cur[222:232, 480:530, :].mean(axis=0)
la = above_r.mean(axis=1); lb = below_r.mean(axis=1)
kr = la / np.maximum(lb, 1e-6)
A2 = np.vstack([lb, np.ones_like(lb)]).T
sol2, *_ = np.linalg.lstsq(A2, la, rcond=None)
pred2 = A2 @ sol2
r22 = 1 - ((la - pred2)**2).sum() / ((la - la.mean())**2).sum()
print(f"\n右墙: k中位={np.median(kr):.3f} IQR={np.percentile(kr,75)-np.percentile(kr,25):.3f} 回归 above={sol2[0]:.3f}*below+{sol2[1]:.3f} R²={r22:.3f}")
