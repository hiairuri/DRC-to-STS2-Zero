# -*- coding: utf-8 -*-
"""Q2 决定性判别：
A) 台阶幅度/局部亮度比值是否跨亮度恒定 -> 恒定=叠加层，变化=内容边界
B) 线相对人物高度：当前 vs 历史
C) 线上下纹理连续性：背景格点图案是否被线打断（场景地平线 vs 叠加层）
"""
import numpy as np
from PIL import Image

CUR = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T18-05-27-954Z-d9cd3f6f.jpg"
HIS = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T09-08-36-813Z-d8b15f17.png"
OUT = r"D:\项目\dragonraja\spikes\LingChar\tools"

cur = np.asarray(Image.open(CUR).convert("RGB")).astype(np.float64)
his = np.asarray(Image.open(HIS).convert("RGB")).astype(np.float64)

lum = cur.mean(axis=2)

# ---------- A) 每列段：step / below 是否常数 ----------
print("== A) 台阶幅度/下方亮度（ratio 恒定 => 叠加层） ==")
print(f"{'x段':>10} {'step':>7} {'below_lum':>9} {'step/below':>10}")
for x0 in range(0, 536, 40):
    x1 = min(x0 + 40, 536)
    above = lum[211:217, x0:x1].mean()
    below = lum[222:228, x0:x1].mean()
    step = below - above
    # 跳过纯人物轮廓突变太大的段仅标注
    print(f"x {x0:3d}-{x1:3d} {step:+8.2f} {below:9.1f} {step/max(below,1e-6):+10.3f}")

# ---------- B) 人物 bbox 与线相对位置 ----------
def char_bbox(img, name):
    # 人物=高饱和/高亮像素（衣服白、头发米）大致用亮度>60 且非蓝色光效
    l = img.mean(axis=2)
    mask = l > 55
    cols = mask.sum(axis=1)
    rows = mask.sum(axis=0)
    ys = np.where(cols > 8)[0]
    xs = np.where(rows > 8)[0]
    print(f"[{name}] 亮像素 bbox: x {xs.min()}..{xs.max()}, y {ys.min()}..{ys.max()}")
    return ys.min(), ys.max()

print("\n== B) 线相对人物位置 ==")
y0c, y1c = char_bbox(cur, "当前 01:52版")
line_c = 218.5
print(f"  当前: 线 y={line_c}  (line-head)/(foot-head) = {(line_c-y0c)/(y1c-y0c):.3f}")
y0h, y1h = char_bbox(his, "历史 更早版")
# 历史线 y=180.5
line_h = 180.5
print(f"  历史: 线 y={line_h}  (line-head)/(foot-head) = {(line_h-y0h)/(y1h-y0h):.3f}")

# ---------- C) 线上/下纹理连续性：取背景竖条，计算跨线相关性 ----------
print("\n== C) 背景纹理跨线连续性（相关高=同一纹理被压暗；低=不同纹理=场景边界） ==")
for x0, x1, tag in [(10, 100, "左墙"), (440, 530, "右墙")]:
    above = lum[190:217, x0:x1].mean(axis=0)
    below = lum[222:250, x0:x1].mean(axis=0)
    a = above - above.mean(); b = below - below.mean()
    cc = float((a*b).sum()/(a.std()*b.std()*a.size + 1e-12))
    print(f"[{tag}] above-vs-below 列剖面相关 = {cc:+.3f}")

# 历史图同样
hlum = his.mean(axis=2)
for x0, x1, tag in [(10, 90, "左墙"), (460, 540, "右墙")]:
    above = hlum[150:178, x0:x1].mean(axis=0)
    below = hlum[184:212, x0:x1].mean(axis=0)
    a = above - above.mean(); b = below - below.mean()
    cc = float((a*b).sum()/(a.std()*b.std()*a.size + 1e-12))
    print(f"[历史-{tag}] above-vs-below 列剖面相关 = {cc:+.3f}")

# ---------- D) 线的亚像素位置与过渡宽度（叠加层通常 0-1px 硬边） ----------
print("\n== D) 过渡宽度（全宽平均列剖面） ==")
prof = lum[210:230, :].mean(axis=1)
for i, v in enumerate(prof):
    print(f"y={210+i:3d} lum={v:7.2f} " + "#" * int(max(v, 0)))
