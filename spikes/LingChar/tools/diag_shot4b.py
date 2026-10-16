# -*- coding: utf-8 -*-
# 修正版：身高用行亮度陡降 + x 限制；光条掩码限制 x>360 排除蓝裙
from PIL import Image
import math

SRC = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T18-05-27-954Z-d9cd3f6f.jpg"
im = Image.open(SRC).convert("RGB")
W, H = im.size
px = im.load()

# ---- 身高：人物列带 x∈[190,470]，白/亮掩码，但排除血条区域 y>495 ----
def white_rows(xa, xb, ymax):
    rows = []
    for y in range(ymax):
        c = 0
        for x in range(xa, xb):
            r, g, b = px[x, y]
            if r > 165 and g > 160 and b > 150:
                c += 1
        rows.append(c)
    return rows

rows = white_rows(190, 470, 490)
ys = [y for y, c in enumerate(rows) if c >= 8]
y_top, y_bot = ys[0], ys[-1]
H_cap = y_bot - y_top
print(f"[height] top={y_top} bottom={y_bot} H_cap={H_cap} scale=H/220={H_cap/220:.3f}")
# 头顶花可能更高，用头发确认：打印 95-115 的行计数
for y in range(95, 120, 5):
    print(f"   y={y} whitecnt={rows[y]}")

# ---- 光条：蓝色掩码限制 x>=350（枪口右侧），y<400 ----
pts = []
for y in range(120, 400):
    for x in range(350, W):
        r, g, b = px[x, y]
        if b > r + 30 and b > 120:
            pts.append((x, y, max(r, g, b), 0.299*r+0.587*g+0.114*b))
if pts:
    xs = [p[0] for p in pts]; ys2 = [p[1] for p in pts]
    bx0, bx1, by0, by1 = min(xs), max(xs), min(ys2), max(ys2)
    n = len(pts)
    mx = sum(p[0] for p in pts)/n; my = sum(p[1] for p in pts)/n
    sxx = sum((p[0]-mx)**2 for p in pts)/n
    syy = sum((p[1]-my)**2 for p in pts)/n
    sxy = sum((p[0]-mx)*(p[1]-my) for p in pts)/n
    th = 0.5*math.atan2(2*sxy, sxx-syy)
    lmax = max(p[2] for p in pts)
    lav = sum(p[3] for p in pts)/n
    print(f"[beam] bbox=({bx0},{by0})-({bx1},{by1}) size={bx1-bx0+1}x{by1-by0+1} n={n}")
    print(f"[beam] angle={math.degrees(th):+.1f}deg maxRGB={lmax} avgLum={lav:.1f} whitecore={'YES' if lmax>=240 else 'NO'}")
    L = math.hypot(bx1-bx0, by1-by0)
    print(f"[beam] diag length={L:.0f}px  len/H_char={L/H_cap:.3f}  len/220={L/220:.3f}")
    # 沿主轴的长度更准确：投影
    ct, st = math.cos(th), math.sin(th)
    proj = [( (p[0]-mx)*ct + (p[1]-my)*st ) for p in pts]
    print(f"[beam] proj length={max(proj)-min(proj):.0f}px")

# ---- 分割线水平性：对 y* 附近逐行求“左侧背景亮度”验证线是否贯穿 ----
def rowseg_lum(y, xa, xb):
    s = 0
    for x in range(xa, xb):
        r, g, b = px[x, y]
        s += 0.299*r+0.587*g+0.114*b
    return s/(xb-xa)

print("[line-check] left-bg / right-bg above-vs-below y=218:")
for tag, (ya, yb) in (("above", (196, 216)), ("below", (220, 240))):
    l = rowseg_lum((ya+yb)//2, 10, 190)
    r = rowseg_lum((ya+yb)//2, 480, W-10)
    print(f"   {tag}: leftbg={l:.1f} rightbg={r:.1f}")

# ---- 分割线亮区纵向范围：full 行亮度找 y>218 之后亮度回落的位置 ----
def row_lum_full(y):
    s = 0
    for x in range(W):
        r, g, b = px[x, y]
        s += 0.299*r+0.587*g+0.114*b
    return s/W

prev = None
for y in range(210, 520, 4):
    v = row_lum_full(y)
    if prev is not None and abs(v-prev) > 6:
        print(f"[extent] step at y={y}: {prev:.1f}->{v:.1f}")
    prev = v
