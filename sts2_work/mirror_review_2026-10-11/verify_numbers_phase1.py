# -*- coding: utf-8 -*-
# Phase 1 数值验收：对镜像图集不透明区（alpha>0.9）分别按旧/新公式计算 sRGB 均值与分位数。
# 预期：旧（sRGB 域直取反）≈(0.35,0.37,0.39)；新（线性取反+sRGB 编码）≈(0.705,0.757,0.773)，
#       p10≈0.43-0.50。口径与 02 报告 §2.7 图集统计法一致。
import numpy as np
from PIL import Image

ATLAS = r"D:/项目/dragonraja/spikes/LingChar/pcksrc/ling/animations/characters/mirror/Chr_ling001.png"
img = np.asarray(Image.open(ATLAS).convert("RGBA"), dtype=np.float64) / 255.0
mask = img[..., 3] > 0.9
rgb = img[mask, :3]
print("不透明像素数:", mask.sum())

def srgb_to_linear(c):
    return np.where(c <= 0.04045, c / 12.92, ((c + 0.055) / 1.055) ** 2.4)

def linear_to_srgb(c):
    c = np.maximum(c, 0.0)
    return np.where(c <= 0.0031308, 12.92 * c, 1.055 * c ** (1.0 / 2.4) - 0.055)

old = 1.0 - rgb                                   # 旧：sRGB 域直取反
new = linear_to_srgb(1.0 - srgb_to_linear(rgb))   # 新：解码→线性取反→编码

for tag, arr in (("旧公式(sRGB直取反)", old), ("新公式(线性取反+编码)", new)):
    mean = arr.mean(axis=0)
    flat = arr.mean(axis=1)
    p10, p90 = np.percentile(flat, 10), np.percentile(flat, 90)
    print("%s: mean=(%.3f,%.3f,%.3f) p10=%.3f p90=%.3f" % (tag, *mean, p10, p90))
