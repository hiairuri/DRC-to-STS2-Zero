# -*- coding: utf-8 -*-
# 只读诊断脚本：像素取证（第四轮验收）
import sys, math
from PIL import Image

SRC = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T18-05-27-954Z-d9cd3f6f.jpg"
CROP_OUT = r"D:\项目\dragonraja\spikes\LingChar\tools\diag_shot4_crop.png"

im = Image.open(SRC).convert("RGB")
W, H = im.size
px = im.load()
print(f"image size: {W}x{H}")

# ---------- 1. 角色像素身高 ----------
# 人物大致在画面中部 x∈[0.28W,0.75W]，取“亮色/白色服装”掩码估计头顶与脚底
x0, x1 = int(W*0.28), int(W*0.75)
rows = []
for y in range(H):
    cnt = 0
    for x in range(x0, x1):
        r, g, b = px[x, y]
        # 白色裙子/袜子/皮肤/花：三通道均较高
        if r > 165 and g > 160 and b > 150:
            cnt += 1
    rows.append(cnt)

ys = [y for y, c in enumerate(rows) if c >= 6]
if ys:
    y_top, y_bot = ys[0], ys[-1]
    H_cap = y_bot - y_top
    print(f"[height] char top y={y_top}, bottom y={y_bot}, H_cap={H_cap}px, ratio vs 220px => scale={H_cap/220:.3f}")
    # 输出掩码行计数峰值帮助确认
    peak = max(rows)
    print(f"[height] mask peak row count={peak}")

# ---------- 2. 亮度分割线 ----------
# 全图每行平均亮度 + 人物核心区(x∈[0.35W,0.65W])每行平均亮度
def row_lum(xa, xb):
    out = []
    for y in range(H):
        s = 0
        for x in range(xa, xb):
            r, g, b = px[x, y]
            s += 0.299*r + 0.587*g + 0.114*b
        out.append(s / (xb - xa))
    return out

lum_full = row_lum(0, W)
lum_body = row_lum(int(W*0.35), int(W*0.65))

def biggest_jump(lum, lo, hi):
    best = (0, None)
    for y in range(lo+2, hi-2):
        d = lum[y+1] - lum[y]
        if abs(d) > abs(best[0]):
            best = (d, y)
    return best  # (delta, y)  delta>0: 下行更亮

j_full = biggest_jump(lum_full, int(H*0.1), int(H*0.85))
j_body = biggest_jump(lum_body, int(H*0.1), int(H*0.85))
print(f"[line] full-width biggest jump: y={j_full[1]}, dLum={j_full[0]:+.2f}")
print(f"[line] body-zone  biggest jump: y={j_body[1]}, dLum={j_body[0]:+.2f}")

for tag, ystar in (("full", j_full[1]), ("body", j_body[1])):
    if ystar is None: continue
    up = sum(lum_body[max(0,ystar-20):ystar])/max(1,len(lum_body[max(0,ystar-20):ystar]))
    dn = sum(lum_body[ystar+1:ystar+21])/max(1,len(lum_body[ystar+1:ystar+21]))
    dl = up - dn
    rel = dl/up*100 if up else 0
    print(f"[line:{tag}] y*={ystar}  avgLum(above20)={up:.2f}  avgLum(below20)={dn:.2f}  dL={dl:+.2f} ({rel:+.1f}%)")

# 也打印每 10px 的 body 行亮度概览，肉眼找台阶
print("[line] body row-lum every 10px:")
for y in range(int(H*0.15), int(H*0.8), 10):
    print(f"   y={y:4d} lum={lum_body[y]:7.2f} full={lum_full[y]:7.2f}")

# ---------- 3. 栅格判读（块内/块间方差扫描） ----------
# 取分割线附近受影响区域（人物上身），做水平方向的自相关 + cell 扫描
gy0 = (j_body[1] or int(H*0.35)) - 60
gy1 = (j_body[1] or int(H*0.35)) + 60
gy0 = max(0, gy0); gy1 = min(H, gy1)
gray = [[0.299*px[x,y][0]+0.587*px[x,y][1]+0.114*px[x,y][2] for x in range(W)] for y in range(gy0, gy1)]
gh = len(gray)

# 水平自相关（对列均值去均值后）
colmean = [sum(gray[y][x] for y in range(gh))/gh for x in range(W)]
m = sum(colmean)/W
v = [c-m for c in colmean]
den = sum(a*a for a in v) or 1
best_ac = []
for lag in range(2, 200):
    num = sum(v[x]*v[x+lag] for x in range(W-lag))
    best_ac.append((num/den, lag))
best_ac.sort(reverse=True)
print("[grid] top-5 horizontal autocorr peaks (corr, lag px):")
for c, l in best_ac[:5]:
    print(f"   corr={c:+.3f} lag={l}px")

# 块内 vs 块间方差，按 cell 尺寸
import statistics
def block_score(cell):
    # 将区域分为 cell 大小的块，计算块均值方差与块内方差之比
    bmeans = []; bvars = []
    for by in range(gy0, gy1-cell, cell):
        for bx in range(0, W-cell, cell):
            vals = [gray[y - gy0][x] for y in range(by, by+cell, 2) for x in range(bx, bx+cell, 2)]
            mu = sum(vals)/len(vals)
            bmeans.append(mu)
            bvars.append(sum((a-mu)**2 for a in vals)/len(vals))
    if len(bmeans) < 4: return None
    vm = statistics.pvariance(bmeans)
    vi = sum(bvars)/len(bvars)
    return vm/vi if vi else 0

print("[grid] block-between/within variance ratio by cell size:")
for cell in (4, 8, 16, 32, 64, 128):
    s = block_score(cell)
    print(f"   cell={cell:4d}px ratio={s if s is None else round(s,3)}")

# ---------- 4. 光条（蓝色掩码） ----------
bx0=by0=10**9; bx1=by1=-1; bsum=0.0; bmax=0.0; bcnt=0
blue_pts = []
for y in range(H):
    for x in range(W):
        r, g, b = px[x, y]
        if b > r + 30 and b > 120:
            bx0=min(bx0,x); bx1=max(bx1,x); by0=min(by0,y); by1=max(by1,y)
            lum = 0.299*r+0.587*g+0.114*b
            bsum += lum; bcnt += 1; bmax = max(bmax, max(r,g,b))
            blue_pts.append((x, y, lum))
if bcnt:
    bw, bh = bx1-bx0+1, by1-by0+1
    # 最长轴角度：用粗略 PCA（对掩码点）
    n = len(blue_pts)
    mx = sum(p[0] for p in blue_pts)/n; my = sum(p[1] for p in blue_pts)/n
    sxx = sum((p[0]-mx)**2 for p in blue_pts)/n
    syy = sum((p[1]-my)**2 for p in blue_pts)/n
    sxy = sum((p[0]-mx)*(p[1]-my) for p in blue_pts)/n
    theta = 0.5*math.atan2(2*sxy, sxx-syy)
    print(f"[beam] blue bbox=({bx0},{by0})-({bx1},{by1}) size={bw}x{bh}px, count={n}")
    print(f"[beam] principal angle={math.degrees(theta):+.1f} deg (screen y-down)")
    print(f"[beam] avg lum={bsum/bcnt:.1f}, max channel val={bmax:.0f} (>=240 white core? {'YES' if bmax>=240 else 'NO'})")
    print(f"[beam] len/charH ratio = {bw/H_cap:.3f}" if ys else "")
else:
    print("[beam] no blue mask found")

# ---------- 5. 裁剪放大 ----------
# 人物上半身 + 枪 + 光条区域
cx0, cy0 = int(W*0.45), (y_top if ys else int(H*0.15))
cx1, cy1 = W, min(H, (y_top if ys else int(H*0.15)) + int(H*0.35))
crop = im.crop((cx0, cy0, cx1, cy1))
cw, ch = crop.size
crop = crop.resize((cw*4, ch*4), Image.NEAREST)
crop.save(CROP_OUT)
print(f"[crop] saved {CROP_OUT} region=({cx0},{cy0})-({cx1},{cy1}) x4 nearest")

# 额外：分割线全宽条带裁剪（上下各40px），验证线是否贯穿背景
if j_body[1]:
    ly = j_body[1]
    strip = im.crop((0, max(0, ly-40), W, min(H, ly+40)))
    sw, sh = strip.size
    strip = strip.resize((sw*2, sh*2), Image.NEAREST)
    strip.save(r"D:\项目\dragonraja\spikes\LingChar\tools\diag_shot4_strip.png")
    print("[strip] saved diag_shot4_strip.png")
