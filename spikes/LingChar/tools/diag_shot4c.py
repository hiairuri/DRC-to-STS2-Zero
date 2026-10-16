# -*- coding: utf-8 -*-
# 第三轮：分割线性质 + 纹理烘焙色验证
from PIL import Image
import math

SRC = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T18-05-27-954Z-d9cd3f6f.jpg"
im = Image.open(SRC).convert("RGB")
W, H = im.size
px = im.load()

# ---- 1. 分割线 y(x) 轨迹：逐列在 y∈[190,250] 找最大亮度梯度 ----
print("[trace] per-column edge y (col: y, dLum):")
edge_ys = []
for x in range(20, W-20, 40):
    best = (0, None)
    for y in range(190, 250):
        def rl(yy):
            s=0
            for dx in range(-3,4):
                r,g,b = px[x+dx, yy]
                s += 0.299*r+0.587*g+0.114*b
            return s/7
        d = rl(y+1)-rl(y)
        if abs(d) > abs(best[0]): best = (d, y)
    edge_ys.append(best[1])
    print(f"   x={x:3d}: y={best[1]} d={best[0]:+.1f}")

# ---- 2. 上下区域统计（避开人物 x<200 与 x>480 的背景带）----
def zone(xa, xb, ya, yb):
    sr=sg=sb=n=0; lums=[]
    for y in range(ya, yb):
        for x in range(xa, xb):
            r,g,b = px[x,y]
            sr+=r; sg+=g; sb+=b; n+=1
            lums.append(0.299*r+0.587*g+0.114*b)
    mu = sum(lums)/n
    var = sum((l-mu)**2 for l in lums)/n
    return sr/n, sg/n, sb/n, mu, var**0.5

print("[zone] left background (x 20-180):")
for tag,(ya,yb) in (("above",(180,215)),("below",(225,260))):
    r,g,b,mu,sd = zone(20,180,ya,yb)
    print(f"   {tag}: RGB=({r:.1f},{g:.1f},{b:.1f}) lum={mu:.1f} stddev={sd:.1f}")
print("[zone] character zone (x 240-400):")
for tag,(ya,yb) in (("above",(185,215)),("below",(225,255))):
    r,g,b,mu,sd = zone(240,400,ya,yb)
    print(f"   {tag}: RGB=({r:.1f},{g:.1f},{b:.1f}) lum={mu:.1f} stddev={sd:.1f}")

# ---- 3. 纹理烘焙色验证 ----
for name, path in (
    ("fx_streak_mask", r"D:\项目\dragonraja\spikes\LingChar\pcksrc\drc_dye\profiles\ling\animations\characters\ling\fx_streak_mask.png"),
    ("fx_flash", r"D:\项目\dragonraja\spikes\LingChar\pcksrc\drc_dye\profiles\ling\animations\characters\ling\fx_flash.png"),
    ("fx_sparks3x3", r"D:\项目\dragonraja\spikes\LingChar\pcksrc\drc_dye\profiles\ling\animations\characters\ling\fx_sparks3x3_grid.png"),
):
    try:
        t = Image.open(path).convert("RGBA")
        tw, th2 = t.size
        tp = t.load()
        # 中心行/列采样
        samples = []
        for fy in (0.15, 0.5, 0.85):
            for fx in (0.15, 0.5, 0.85):
                samples.append(tp[int(fx*(tw-1)), int(fy*(th2-1))])
        mx = max(max(s[0], s[1], s[2]) for s in samples)
        print(f"[tex] {name}: {tw}x{th2} samples(RGBA): " + " ".join(str(s) for s in samples[:5]) + f" maxRGB={mx}")
    except Exception as e:
        print(f"[tex] {name}: ERR {e}")

# ---- 4. 打包 pck 与 config 新旧 ----
import os
for p in (r"D:\项目\dragonraja\spikes\LingChar\DRCSpineDye.pck",
          r"D:\项目\dragonraja\spikes\LingChar\pcksrc\drc_dye\profiles\ling\animations\characters\ling\config.json",
          r"D:\项目\dragonraja\spikes\LingChar\pcksrc\drc_dye\profiles\ling\animations\characters\ling\fx_streak_mask.png"):
    if os.path.exists(p):
        print(f"[file] {os.path.basename(p)}: mtime={os.path.getmtime(p):.0f} size={os.path.getsize(p)}")
