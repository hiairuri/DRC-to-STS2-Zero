# -*- coding: utf-8 -*-
"""diag_shot3b.py — 特效区域精细取证：连通域拆分 + 边缘/均匀性 + 全屏横带检测"""
import os, math
from collections import deque
from PIL import Image

SHOT = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T09-08-36-813Z-d8b15f17.png"
im = Image.open(SHOT).convert("RGB")
px = im.load()
w, h = im.size
print(f"截图尺寸: {w}x{h}")

# ---- 1. 连通域拆分（青蓝掩码，B>R+10 且 B>140，排除角色区 x<250 的干扰只用于参考）----
mask = [[False]*w for _ in range(h)]
for y in range(h):
    for x in range(w):
        r, g, b = px[x, y]
        if b > r + 10 and b > 140:
            mask[y][x] = True
seen = [[False]*w for _ in range(h)]
comps = []
for y in range(h):
    for x in range(w):
        if mask[y][x] and not seen[y][x]:
            q = deque([(x, y)]); seen[y][x] = True
            pts = []
            while q:
                cx, cy = q.popleft(); pts.append((cx, cy))
                for dx, dy in ((1,0),(-1,0),(0,1),(0,-1),(1,1),(-1,-1),(1,-1),(-1,1)):
                    nx, ny = cx+dx, cy+dy
                    if 0 <= nx < w and 0 <= ny < h and mask[ny][nx] and not seen[ny][nx]:
                        seen[ny][nx] = True; q.append((nx, ny))
            if len(pts) >= 30:
                comps.append(pts)
comps.sort(key=len, reverse=True)
print(f"\n连通域(>=30px): {len(comps)} 个")
for i, pts in enumerate(comps[:8]):
    xs = [p[0] for p in pts]; ys = [p[1] for p in pts]
    x0, x1, y0, y1 = min(xs), max(xs), min(ys), max(ys)
    avg = tuple(round(sum(px[p][c] for p in pts)/len(pts)) for c in range(3))
    fill = len(pts)/max(1,(x1-x0+1)*(y1-y0+1))
    print(f"  #{i}: bbox=({x0},{y0})-({x1},{y1}) w={x1-x0+1} h={y1-y0+1} n={len(pts)} 填充率={fill:.2f} 平均RGB={avg}")

# ---- 2. 特效区域内部颜色均匀性（取最大连通域=倾斜四边形）----
def uniformity(pts, tag):
    if len(pts) < 50:
        return
    rs=[px[p][0] for p in pts]; gs=[px[p][1] for p in pts]; bs=[px[p][2] for p in pts]
    n=len(pts)
    def sd(v):
        m=sum(v)/n; return (sum((x-m)**2 for x in v)/n)**0.5
    print(f"  [{tag}] 内部标准差 R={sd(rs):.1f} G={sd(gs):.1f} B={sd(bs):.1f}  (sd<8 ≈ 实心平坦)")
    # 颜色分箱：量化到 16 级
    from collections import Counter
    cnt = Counter((px[p][0]//16, px[p][1]//16, px[p][2]//16) for p in pts)
    top = cnt.most_common(5)
    tot = sum(cnt.values())
    print(f"    top5 颜色桶(16级): " + ", ".join(f"({a*16},{b*16},{c*16})x{v} ({v/tot*100:.0f}%)" for (a,b,c),v in top))

if comps:
    uniformity(comps[0], "最大连通域(倾斜四边形主体)")
# 光条：取 y 在 85-105 之间 x>280 的横条分量（第2大或单独）
for i, pts in enumerate(comps[1:5], 1):
    xs=[p[0] for p in pts]
    uniformity(pts, f"连通域#{i}")

# ---- 3. 光条水平剖面（亮核->两端衰减）----
print("\n[光条水平剖面] y=亮度最大行, x 从左到右每 8px 采样:")
best_y, best_v = None, -1
for y in range(80, 110):
    v = sum(1 for x in range(280, w) if mask[y][x])
    if v > best_v: best_v, best_y = v, y
print(f"  最亮行 y={best_y}")
row=[]
for x in range(280, w, 8):
    if any(mask[yy][x] for yy in range(max(0,best_y-3), min(h,best_y+4))):
        c = px[x, best_y]
        row.append(f"{x}:{c}")
print("  " + "  ".join(row))

# ---- 4. 全屏横带检测（逐行平均亮度突跳）----
print("\n[全屏横带检测] 相邻行平均亮度差 > 8 的行:")
import statistics
def rowmean(y):
    s=0
    for x in range(0, w, 4):
        c=px[x,y]; s += (c[0]+c[1]+c[2])/3
    return s/(w//4)
means=[rowmean(y) for y in range(h)]
for y in range(1, h):
    d = means[y]-means[y-1]
    if abs(d) > 8:
        print(f"  y={y}: 行均亮度跳变 {d:+.1f} (上行={means[y-1]:.1f} 本行={means[y]:.1f})")

# ---- 5. 倾斜四边形边缘阶梯化检测：左边缘逐行 x 坐标的跳变分布 ----
if comps:
    pts_set = set(comps[0])
    xs = [p[0] for p in comps[0]]; ys=[p[1] for p in comps[0]]
    y0, y1 = min(ys), max(ys)
    edges=[]
    for y in range(y0, y1+1):
        rowx=[x for x in range(min(xs), max(xs)+1) if (x,y) in pts_set]
        if rowx: edges.append((y, min(rowx)))
    steps=[]
    for i in range(1,len(edges)):
        dy = edges[i][0]-edges[i-1][0]
        dx = edges[i][1]-edges[i-1][1]
        if dy: steps.append(dx/dy)
    if steps:
        from collections import Counter
        cnt = Counter(round(s,1) for s in steps)
        print(f"\n[左边缘斜率分布] (阶梯化则集中于少数整数比) top8: {cnt.most_common(8)}")
