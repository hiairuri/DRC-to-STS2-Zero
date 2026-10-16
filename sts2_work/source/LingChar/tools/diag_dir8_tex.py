# -*- coding: utf-8 -*-
# 纹理区域 zero_qiang_C (bounds 338,279,77,56) 放大导出 + 滑套上下缘/中线拟合
import numpy as np, math
from PIL import Image

page = Image.open(r'D:/项目/dragonraja/spikes/LingChar/pcksrc/ling/animations/characters/ling/Chr_ling001.png').convert('RGBA')
rx, ry, rw, rh = 338, 279, 77, 56
reg = page.crop((rx, ry, rx+rw, ry+rh))
reg.resize((rw*10, rh*10), Image.NEAREST).save(
    r'D:/项目/dragonraja/spikes/LingChar/tools/diag_dir8_region_x10.png')
a = np.asarray(reg).astype(np.float32)
alpha = a[..., 3]
lum = a[..., :3].mean(axis=2)
opaque = alpha > 40
print(f'不透明像素: {opaque.sum()}/{rw*rh}')

# 逐列 top/bottom 边缘（不透明像素的最高/最低行），再拟合上下缘与中线
cols, tops, bots = [], [], []
for x in range(rw):
    ys = np.nonzero(opaque[:, x])[0]
    if len(ys) >= 2:
        cols.append(x); tops.append(ys[0]); bots.append(ys[-1])
cols = np.array(cols); tops = np.array(tops, float); bots = np.array(bots, float)


def robust_fit(x, y, it=3):
    m = np.ones(len(x), bool)
    for _ in range(it):
        k, c = np.polyfit(x[m], y[m], 1)
        r = np.abs(k*x + c - y)
        m = r < max(1.2, np.percentile(r[m], 80))
    k, c = np.polyfit(x[m], y[m], 1)
    return k, c, int(m.sum()), float(np.abs(k*x[m]+c-y[m]).max())


kt, ct, nt, rt = robust_fit(cols, tops)
kb, cb, nb, rb = robust_fit(cols, bots)
kc = (kt+kb)/2  # 中线斜率（平行缘平均）
print(f'上缘 θ_top = {math.degrees(math.atan(kt)):.2f}° (n={nt}, 残差{rt:.2f}px)')
print(f'下缘 θ_bot = {math.degrees(math.atan(kb)):.2f}° (n={nb}, 残差{rb:.2f}px)')
print(f'中线角     = {math.degrees(math.atan(kc)):.2f}°')

# 不透明像素 PCA 主轴（枪整体朝向）
ys, xs = np.nonzero(opaque)
cx, cy = xs.mean(), ys.mean()
xx = ((xs-cx)**2).mean(); yy = ((ys-cy)**2).mean(); xy = ((xs-cx)*(ys-cy)).mean()
ang = 0.5*math.degrees(math.atan2(2*xy, xx-yy))
print(f'PCA 主轴 = {ang:.2f}° (质心=({cx:.1f},{cy:.1f}), 不透明={opaque.sum()})')
