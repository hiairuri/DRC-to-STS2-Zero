# -*- coding: utf-8 -*-
# dither/压暗检测可视化：新图上缘候选带、旧图线区，x8 放大 + 高频残差
from PIL import Image, ImageChops, ImageFilter
import math

NEW = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T18-05-27-954Z-d9cd3f6f.jpg"
OLD = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T09-08-36-813Z-d8b15f17.png"
OUT = r"D:\项目\dragonraja\spikes\LingChar\tools"

im = Image.open(NEW).convert("RGB")
old = Image.open(OLD).convert("RGB")

# 1) 新图：分割线候选带 y195-245 全宽，x8 放大（对比度增强以便目视微弱梯度）
band = im.crop((0, 195, 536, 245))
big = band.resize((536*2, 50*8), Image.NEAREST)
from PIL import ImageEnhance
big = ImageEnhance.Contrast(big).enhance(2.5)
big.save(OUT + r"\r5_new_edge_band_x8_contrast.png")

# 2) 新图：背景压暗候选区高频残差（blur 后差分 = 高频分量）x8
def hfreq(img, box, scale=8):
    p = img.crop(box)
    hf = ImageChops.subtract(p, p.filter(ImageFilter.GaussianBlur(2)), scale=1, offset=128)
    return hf.resize((p.width*scale, p.height*scale), Image.NEAREST)
hfreq(im, (20, 190, 180, 260)).save(OUT + r"\r5_new_bg_above_hfreq_x8.png")
hfreq(im, (20, 260, 180, 330)).save(OUT + r"\r5_new_bg_below_hfreq_x8.png")

# 3) 旧图：青白条区 y88-118 x300-460，x6（已存 6x 全段）+ 高频
hfreq(old, (300, 85, 460, 120), 6).save(OUT + r"\r5_old_bar_hfreq_x6.png")
print("saved: r5_new_edge_band_x8_contrast.png, r5_new_bg_above/below_hfreq_x8.png, r5_old_bar_hfreq_x6.png")

# 4) 量化：新图背景上/下带高频 RMS 对比（dither 若存在 → 上方显著更高）
def hfrms(img, box):
    p = img.crop(box); pp=p.load()
    s=0; n=0
    for y in range(p.height):
        for x in range(p.width-1):
            d = sum(pp[x,y]) - sum(pp[x+1,y]); s+=d*d; n+=1
    return math.sqrt(s/n)
print(f"new above hfreqRMS = {hfrms(im,(20,190,180,255)):.1f}")
print(f"new below hfreqRMS = {hfrms(im,(20,262,180,327)):.1f}")
print(f"new far-right above = {hfrms(im,(470,190,530,255)):.1f}")
print(f"new far-right below = {hfrms(im,(470,262,530,327)):.1f}")
