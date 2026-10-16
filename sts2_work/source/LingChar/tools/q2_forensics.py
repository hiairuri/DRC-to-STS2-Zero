# -*- coding: utf-8 -*-
"""Q2 定向取证：分割线乘法模型 + 抖动检测 + 历史对照。
只读诊断，输出落本目录。"""
import sys, math
import numpy as np
from PIL import Image

CUR = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T18-05-27-954Z-d9cd3f6f.jpg"
HIS = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T09-08-36-813Z-d8b15f17.png"
OUT = r"D:\项目\dragonraja\spikes\LingChar\tools"

cur = np.asarray(Image.open(CUR).convert("RGB")).astype(np.float64)
his = np.asarray(Image.open(HIS).convert("RGB")).astype(np.float64)
print("cur shape", cur.shape, "his shape", his.shape)

H, W, _ = cur.shape

# ---------- 1) 行均亮度剖面（背景列，避开人物 x≈130..470 中的主体与 fx） ----------
# 人物头顶106 脚底467；光条 bbox 在 x≈300..450, y≈150..300 一带。
# 背景安全列：左 0..110，右 480..W-1。分三段统计。
segs = {"left": (0, 110), "right": (478, W)}

def row_profile(img, x0, x1, y0, y1):
    band = img[y0:y1, x0:x1, :]
    return band.mean(axis=1)  # (rows,3)

print("\n== 当前列(y 200..240)背景行剖面 step 探测 ==")
for name, (x0, x1) in segs.items():
    prof = row_profile(cur, x0, x1, 195, 245)
    lum = prof.mean(axis=1)
    # 找最大单行跳变
    d = np.diff(lum)
    i = int(np.argmax(np.abs(d)))
    print(f"[{name}] 最大行跳变 y={195+i}->{195+i+1} dLum={d[i]:+.2f}  lum[195..245]峰谷差={lum.max()-lum.min():.2f}")

# ---------- 2) 乘法模型：线上/线下同列比值 per-channel ----------
# 用紧贴边缘的窄带：above=210..216, below=224..230（精确边缘再由剖面确认）
def ratio_table(img, y_edge, half=6):
    res = {}
    for name, (x0, x1) in segs.items():
        above = img[y_edge-half-2:y_edge-2, x0:x1, :].mean(axis=(0,1))
        below = img[y_edge+3:y_edge+3+half, x0:x1, :].mean(axis=(0,1))
        r = above / np.maximum(below, 1e-6)
        res[name] = (above, below, r)
    return res

y_edge = 220
rt = ratio_table(cur, y_edge)
print("\n== 乘法模型 比值表 (y_edge=%d) ==" % y_edge)
for name, (a, b, r) in rt.items():
    print(f"[{name}] above RGB=({a[0]:.1f},{a[1]:.1f},{a[2]:.1f}) below=({b[0]:.1f},{b[1]:.1f},{b[2]:.1f}) r=R/G/B=({r[0]:.3f},{r[1]:.3f},{r[2]:.3f})")

# x 方向一致性：逐 x 计算 above/below 行均比值（每通道），看是否全线近似常数
def x_consistency(img, y_edge, x0, x1, half=6):
    above = img[y_edge-half-2:y_edge-2, x0:x1, :].mean(axis=0)  # (W',3)
    below = img[y_edge+3:y_edge+3+half, x0:x1, :].mean(axis=0)
    r = above / np.maximum(below, 1e-6)
    return r  # (W',3)

for name, (x0, x1) in segs.items():
    r = x_consistency(cur, y_edge, x0, x1)
    for c, ch in enumerate("RGB"):
        v = r[:, c]
        print(f"[{name}][{ch}] r 中位={np.median(v):.3f} IQR={np.percentile(v,75)-np.percentile(v,25):.3f} min={v.min():.3f} max={v.max():.3f}")

# ---------- 3) 抖动/棋盘检测：线上方压暗区高频残差 ----------
def dither_check(img, x0, x1, y0, y1, tag):
    reg = img[y0:y1, x0:x1, :].mean(axis=2)
    # 3x3 均值低通，残差 = 原图 - 低通
    k = np.ones((3,3))/9.0
    from numpy.lib.stride_tricks import sliding_window_view
    pad = np.pad(reg, 1, mode="edge")
    low = sliding_window_view(pad, (3,3)).mean(axis=(2,3))
    res = reg - low
    # 水平/垂直 lag-1, lag-2 自相关
    def ac(v, lag):
        a = v[..., :-lag].ravel(); b = v[..., lag:].ravel()
        a = a - a.mean(); b = b - b.mean()
        return float((a*b).sum()/ (a.std()*b.std()*a.size + 1e-12))
    print(f"[{tag}] 残差std={res.std():.3f} acH1={ac(res,1):.3f} acH2={ac(res,2):.3f} acV1={ac(res,1) if False else ac(res.T.copy(),1):.3f} acV2={ac(res.T.copy(),2):.3f}")
    # 2x2 棋盘模板匹配：sign((-1)^(x+y)) 相关
    yy, xx = np.mgrid[0:reg.shape[0], 0:reg.shape[1]]
    chk = ((xx+yy) % 2)*2.0 - 1.0
    cc = float((res*chk).mean() / (res.std()*chk.std() + 1e-12))
    print(f"[{tag}] 2x2棋盘相关={cc:+.4f}")
    return res

print("\n== 抖动检测（压暗区 y 120..210） ==")
dither_check(cur, 8, 108, 120, 210, "cur-left-wall")
dither_check(cur, 480, W-2, 120, 210, "cur-right-wall")
dither_check(cur, 8, 108, 240, 330, "cur-left-floor")
# 对照：无压暗区（人物中部衣服，若线为叠加层则人物同被覆盖？只作参考）
dither_check(cur, 180, 300, 380, 460, "cur-skirt-ref")

# ---------- 4) 精确边缘定位：背景列平均的逐行剖面打印 ----------
print("\n== 左背景列 y 205..235 逐行 RGB（定位精确边缘） ==")
prof = row_profile(cur, 8, 108, 205, 236)
for i, (r, g, b) in enumerate(prof):
    print(f"y={205+i:3d}  R={r:6.1f} G={g:6.1f} B={b:6.1f}")

# ---------- 5) 历史对照：全高扫描最大水平台阶 ----------
print("\n== 历史 PNG 全高行剖面台阶扫描（背景列） ==")
hH, hW, _ = his.shape
for name, (x0, x1) in {"his-left": (0, int(hW*0.2)), "his-right": (int(hW*0.85), hW-1)}.items():
    prof = his[:, x0:x1, :].mean(axis=1).mean(axis=1)
    d = np.diff(prof)
    top = np.argsort(np.abs(d))[::-1][:5]
    print(f"[{name}] h={hH} 最大5个行跳变: " + ", ".join(f"y={i}->{i+1} d={d[i]:+.2f}" for i in sorted(top)))

# ---------- 6) 可视化落盘 ----------
# 6a 当前沿线 ×4 裁剪
cy = 220
crop = Image.open(CUR).convert("RGB").crop((0, cy-40, W, cy+40))
crop = crop.resize((crop.width*4, crop.height*4), Image.NEAREST)
crop.save(OUT + r"\q2_line_crop_x4.png")
# 6b 抖动残差放大可视化（压暗区）
reg = cur[120:210, 8:108, :].mean(axis=2)
pad = np.pad(reg, 1, mode="edge")
low = sliding_window_view(pad, (3,3)).mean(axis=(2,3))
res = reg - low
vis = np.clip(128 + res*12, 0, 255).astype(np.uint8)
Image.fromarray(vis).resize((vis.shape[1]*4, vis.shape[0]*4), Image.NEAREST).save(OUT + r"\q2_dither_resid_x4.png")
# 6c 历史 PNG 沿其最强台阶裁剪 ×4
ti = int(sorted(np.argsort(np.abs(np.diff(his[:, :int(hW*0.2), :].mean(axis=1).mean(axis=1))))[::-1][:5])[0])
crop2 = Image.open(HIS).convert("RGB").crop((0, max(0,ti-30), hW, min(hH,ti+30)))
crop2 = crop2.resize((crop2.width*3, crop2.height*3), Image.NEAREST)
crop2.save(OUT + r"\q2_history_edge_x3.png")
print("\nsaved: q2_line_crop_x4.png / q2_dither_resid_x4.png / q2_history_edge_x3.png (history edge y≈%d)" % ti)
