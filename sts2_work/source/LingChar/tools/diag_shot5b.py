# -*- coding: utf-8 -*-
# 裁剪放大：新图人物区/背景区 y 160-300；旧图线区 y 40-170
from PIL import Image

NEW = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T18-05-27-954Z-d9cd3f6f.jpg"
OLD = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T09-08-36-813Z-d8b15f17.png"
OUT = r"D:\项目\dragonraja\spikes\LingChar\tools"

im = Image.open(NEW).convert("RGB")
old = Image.open(OLD).convert("RGB")

# 新图：分割线候选区全宽条带 y160-300，x4 放大
c1 = im.crop((0, 160, 536, 300)).resize((536*4//2, 140*4//2), Image.NEAREST)
c1.save(OUT + r"\r5_new_line_strip_2x.png")

# 新图：人物区 x220-460 y190-260, x4
c2 = im.crop((220, 190, 460, 260)).resize((240*4, 70*4), Image.NEAREST)
c2.save(OUT + r"\r5_new_line_4x.png")

# 旧图：线区全宽 y60-160, x3
c3 = old.crop((0, 60, 544, 160)).resize((544*3//2, 100*3//2), Image.NEAREST)
c3.save(OUT + r"\r5_old_line_strip_15x.png")

# 旧图：线段右段 x300-470 y80-125, x6
c4 = old.crop((300, 80, 470, 125)).resize((170*6, 45*6), Image.NEAREST)
c4.save(OUT + r"\r5_old_line_6x.png")
print("saved 4 crops")
