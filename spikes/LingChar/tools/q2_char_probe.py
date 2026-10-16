# -*- coding: utf-8 -*-
"""Q2 补充：人物区 vs 背景区沿 y≈220 的台阶对比 + 历史图人物区检查。"""
import numpy as np
from PIL import Image
from numpy.lib.stride_tricks import sliding_window_view

CUR = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T18-05-27-954Z-d9cd3f6f.jpg"
HIS = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T09-08-36-813Z-d8b15f17.png"
OUT = r"D:\项目\dragonraja\spikes\LingChar\tools"

cur = np.asarray(Image.open(CUR).convert("RGB")).astype(np.float64)
his = np.asarray(Image.open(HIS).convert("RGB")).astype(np.float64)
H, W, _ = cur.shape

# ---------- A) 人物列台阶探测：逐 20px 宽列段，扫 y 190..250 ----------
print("== 当前图：人物区各列段 y190..250 最大行跳变 ==")
for x0 in range(120, 480, 30):
    band = cur[190:250, x0:x0+30, :].mean(axis=1).mean(axis=1)
    d = np.diff(band)
    i = int(np.argmax(np.abs(d)))
    flag = "  <== 台阶" if abs(d[i]) > 6 else ""
    print(f"x {x0:3d}..{x0+30:3d}  y={190+i}->{190+i+1} dLum={d[i]:+7.2f}{flag}")

# ---------- B) 人物区精确剖面（选头发/脸/大衣 竖直连续列） ----------
print("\n== 人物列段 x=170..200（头发/脸）y 200..245 逐行 RGB ==")
prof = cur[200:246, 170:200, :].mean(axis=1)
for i, (r, g, b) in enumerate(prof):
    print(f"y={200+i:3d}  R={r:6.1f} G={g:6.1f} B={b:6.1f}")

# ---------- C) 比值法落在人物上：above/below 比值 per-channel（列段） ----------
print("\n== 人物区乘法模型（y_edge=220） ==")
for x0, x1, tag in [(150, 210, "hair/face"), (210, 280, "face/chest"), (280, 340, "arm/gun后"), (380, 460, "裙摆右侧")]:
    a = cur[212:218, x0:x1, :].mean(axis=(0, 1))
    b = cur[224:230, x0:x1, :].mean(axis=(0, 1))
    r = a / np.maximum(b, 1e-6)
    print(f"[{tag}] above=({a[0]:.1f},{a[1]:.1f},{a[2]:.1f}) below=({b[0]:.1f},{b[1]:.1f},{b[2]:.1f}) r=({r[0]:.3f},{r[1]:.3f},{r[2]:.3f})")

# ---------- D) 历史图：人物区 y 扫描（对照是否同款线） ----------
hH, hW, _ = his.shape
print("\n== 历史图：人物区各列段全高最大行跳变 ==")
for x0 in range(60, hW-30, 40):
    band = his[:, x0:x0+30, :].mean(axis=1).mean(axis=1)
    d = np.diff(band)
    i = int(np.argmax(np.abs(d)))
    print(f"x {x0:3d}..{x0+30:3d}  y={i}->{i+1} dLum={d[i]:+7.2f}")

# ---------- E) 落盘可视化 ----------
# 当前图人物+线区域 ×4
crop = Image.open(CUR).convert("RGB").crop((100, 150, 480, 300))
crop = crop.resize((crop.width*3, crop.height*3), Image.NEAREST)
crop.save(OUT + r"\q2_line_char_x3.png")
# 历史图 y≈181 台阶区域 ×3
crop2 = Image.open(HIS).convert("RGB").crop((0, 140, hW, 220))
crop2 = crop2.resize((crop2.width*3, crop2.height*3), Image.NEAREST)
crop2.save(OUT + r"\q2_history_edge181_x3.png")
print("\nsaved q2_line_char_x3.png / q2_history_edge181_x3.png")
