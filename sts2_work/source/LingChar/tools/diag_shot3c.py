# -*- coding: utf-8 -*-
"""diag_shot3c.py — 精确拆分：光条 / 倾斜四边形 / 亮核 + 全屏横带上下对比"""
from PIL import Image

SHOT = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T09-08-36-813Z-d8b15f17.png"
im = Image.open(SHOT).convert("RGB")
px = im.load(); w, h = im.size

def fx_mask_test(x0,y0,x1,y1, br=5, bmin=90):
    pts=[]
    for y in range(y0,y1):
        for x in range(x0,x1):
            r,g,b = px[x,y]
            if b > r+br and b > bmin:
                pts.append((x,y,r,g,b))
    return pts

def stats(pts, tag):
    if not pts:
        print(f"[{tag}] 无像素"); return
    xs=[p[0] for p in pts]; ys=[p[1] for p in pts]
    n=len(pts)
    avg=tuple(round(sum(p[2+i] for p in pts)/n) for i in range(3))
    mx=tuple(max(p[2+i] for p in pts) for i in range(3))
    print(f"[{tag}] bbox=({min(xs)},{min(ys)})-({max(xs)},{max(ys)}) w={max(xs)-min(xs)+1} h={max(ys)-min(ys)+1} n={n} 平均RGB={avg} 最大RGB={mx}")
    # RGB 通道标准差
    for ci,name in ((0,'R'),(1,'G'),(2,'B')):
        v=[p[2+ci] for p in pts]; m=sum(v)/n
        sd=(sum((x-m)**2 for x in v)/n)**0.5
        print(f"    {name}: mean={m:.0f} sd={sd:.1f} min={min(v)} max={max(v)}")

# 倾斜四边形区域（左半，不含光条）
quad = fx_mask_test(240, 55, 352, 160)
stats(quad, "倾斜四边形区域(青蓝)")

# 亮核（很亮的像素: sum>560）在 fx 区域
core = [(x,y) for y in range(55,170) for x in range(240,470)
        if sum(px[x,y])>560 and px[x,y][2]>px[x,y][0]]
if core:
    xs=[p[0] for p in core]; ys=[p[1] for p in core]
    print(f"\n[亮核 sum>560] bbox=({min(xs)},{min(ys)})-({max(xs)},{max(ys)}) w={max(xs)-min(xs)+1} h={max(ys)-min(ys)+1} n={len(core)}")

# 光条右段（x>352）
bar = fx_mask_test(352, 80, 470, 120)
stats(bar, "光条右段")

# 光条完整水平剖面（含白色核心），y=98
print("\n[光条 y=98 完整剖面] x=290..460 每 6px:")
print("  " + "  ".join(f"{x}:{px[x,98]}" for x in range(290, 462, 6)))

# 四边形垂直剖面 x=300, y=60..150
print("\n[四边形垂直剖面 x=300] y=60..150 每 3px:")
print("  " + "  ".join(f"y{y}:{px[300,y]}" for y in range(60, 152, 3)))

# 四边形水平剖面 y=125（只穿四边形，不穿光条）
print("\n[四边形水平剖面 y=125] x=245..350 每 4px:")
print("  " + "  ".join(f"{x}:{px[x,125]}" for x in range(245, 352, 4)))

# 全屏横带：y=181 上下各取 10 行平均色（x 分三段）
print("\n[全屏横带 y=181] 上下行对比（背景区 x=60..140）:")
for y in (175, 178, 180, 181, 182, 184, 187):
    seg = [px[x,y] for x in range(60,141,2)]
    avg = tuple(round(sum(c[i] for c in seg)/len(seg)) for i in range(3))
    print(f"  y={y}: 平均RGB={avg}")
print("  角色区 x=130..240:")
for y in (175, 181, 187):
    seg = [px[x,y] for x in range(130,241,2)]
    avg = tuple(round(sum(c[i] for c in seg)/len(seg)) for i in range(3))
    print(f"  y={y}: 平均RGB={avg}")
