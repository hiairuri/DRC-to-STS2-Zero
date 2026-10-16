# -*- coding: utf-8 -*-
"""最终证据落盘：线区×4、抖动残差可视化、暖上/冷下调色证明图。"""
import numpy as np
from PIL import Image
from numpy.lib.stride_tricks import sliding_window_view

CUR = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T18-05-27-954Z-d9cd3f6f.jpg"
HIS = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T09-08-36-813Z-d8b15f17.png"
OUT = r"D:\项目\dragonraja\spikes\LingChar\tools"

cur = Image.open(CUR).convert("RGB")
W, H = cur.size

# 1) 分割线附近 ×4（含左右背景与人物）
crop = cur.crop((0, 178, W, 262)).resize((W * 4, 84 * 4), Image.NEAREST)
crop.save(OUT + r"\q2_line_crop_x4.png")

# 2) 抖动残差可视化（压暗区 y120..210 墙面）：放大12倍仍无栅格 => 无 dither
c = np.asarray(cur).astype(np.float64)
reg = c[120:210, 8:208, :].mean(axis=2)
pad = np.pad(reg, 1, mode="edge")
low = sliding_window_view(pad, (3, 3)).mean(axis=(2, 3))
res = reg - low
vis = np.clip(128 + res * 12, 0, 255).astype(np.uint8)
Image.fromarray(vis).resize((vis.shape[1] * 4, vis.shape[0] * 4), Image.NEAREST).save(
    OUT + r"\q2_dither_resid_x4.png")

# 3) 暖上/冷下证明：接缝上/下采样块并列 + R/G/B 条形值
yE = 219
top = c[yE - 14:yE - 4, 20:120, :].mean(axis=(0, 1))
bot = c[yE + 4:yE + 14, 20:120, :].mean(axis=(0, 1))
panel = Image.new("RGB", (640, 360), (24, 24, 24))
blk_t = cur.crop((20, yE - 64, 120, yE - 4)).resize((256, 240), Image.NEAREST)
blk_b = cur.crop((20, yE + 4, 120, yE + 64)).resize((256, 240), Image.NEAREST)
panel.paste(blk_t, (16, 30)); panel.paste(blk_b, (16, 30 + 250))
from PIL import ImageDraw
d = ImageDraw.Draw(panel)
for i, (name, v) in enumerate([("上(墙)", top), ("下(地)", bot)]):
    y0 = 30 + i * 250
    d.text((290, y0 + 4), f"{name}  R={v[0]:.1f} G={v[1]:.1f} B={v[2]:.1f}", fill=(230, 230, 230))
    for j, ch in enumerate("RGB"):
        h = int(v[j] * 1.6)
        col = {"R": (220, 60, 60), "G": (60, 200, 90), "B": (70, 120, 230)}[ch]
        d.rectangle([290 + j * 100, y0 + 30 - h, 290 + j * 100 + 60, y0 + 30], fill=col)
d.text((290, 40 + 250 - 20), "上=暖棕墙(R>G,B) / 下=冷蓝地 => 场景内容边界", fill=(255, 210, 120))
panel.save(OUT + r"\q2_horizon_proof.png")

# 4) 历史线区 ×3（已有 _x3 版本，这里补全宽含背景）
h = Image.open(HIS).convert("RGB")
hW2, hH2 = h.size
crop2 = h.crop((0, 150, hW2, 212)).resize((hW2 * 3, 62 * 3), Image.NEAREST)
crop2.save(OUT + r"\q2_history_line_x3.png")

print("saved:")
import os
for f in ["q2_line_crop_x4.png", "q2_dither_resid_x4.png", "q2_horizon_proof.png",
          "q2_history_line_x3.png", "q2_line_char_x3.png", "q2_history_edge181_x3.png"]:
    p = OUT + "\\" + f
    print(" ", f, os.path.getsize(p), "B" if os.path.exists(p) else "MISSING")
