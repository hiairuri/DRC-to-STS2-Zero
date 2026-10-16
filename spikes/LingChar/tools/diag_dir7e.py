# -*- coding: utf-8 -*-
"""
diag_dir7e.py —— 决定性可视化：按 Spine UV 语义渲染枪 mesh 四边形
==================================================================
把 zero_qiang_C2 的 4 顶点（JSON bone104 本地系）按 hull 序连四边形，
从图集 Chr_ling001.png 的 zero_qiang_C 区域按 uvs 采样贴图，
输出渲染结果 + 各边/对角线角度标注。
若渲染出的枪管方向与"u 轴方向(-15.3°)"差 ~30°，即实锤"贴图内倾角"假说
（梁沿 u 轴没错，错在 u 轴≠膛线方向）。
"""
import json
import math
from PIL import Image, ImageDraw

LING = r"D:\项目\dragonraja\spikes\LingChar\pcksrc\ling\animations\characters\ling"
data = json.load(open(LING + r"\ling.json", encoding="utf-8"))
att = data["skins"][0]["attachments"]["qiang02"]["zero_qiang_C2"]
verts = att["vertices"]
uvs = att["uvs"]
deform = (20.38768, -32.89978)  # attack deform（均匀平移，不影响方向）

# JSON 顶点：v0..v3（bone104 本地系，y 值原样=Spine y-up 语义）
pts_json = [(verts[i] + deform[0], verts[i + 1] + deform[1]) for i in range(0, 8, 2)]
uv = [(uvs[i], uvs[i + 1]) for i in range(0, 8, 2)]

# 图集区域（无旋转）：bounds:338,279,77,56
page = Image.open(LING + r"\Chr_ling001.png").convert("RGBA")
rx, ry, rw, rh = 338, 279, 77, 56
region = page.crop((rx, ry, rx + rw, ry + rh))

# 渲染：bone 本地系 → 画布（y 翻转使视觉与"屏幕"一致：JSON y-up → 画布 y-down）
# 包围盒
xs = [p[0] for p in pts_json]; ys = [p[1] for p in pts_json]
S = 6  # 放大
minx, maxx = min(xs) - 20, max(xs) + 20
miny, maxy = min(ys) - 20, max(ys) + 20
Wc, Hc = int((maxx - minx) * S), int((maxy - miny) * S)
canvas = Image.new("RGBA", (Wc, Hc), (30, 30, 40, 255))

def to_canvas(p):
    return ((p[0] - minx) * S, (maxy - p[1]) * S)  # y 翻转（JSON y-up → 画布向下）

# 三角形贴图（两个三角：tri [1,2,3] 与 [1,3,0]，uv 语义 v=0 在区域顶部=PIL y 向）
import numpy as np
reg = np.asarray(region, dtype=np.float32)
tri_idx = att["triangles"]
cnv = np.asarray(canvas, dtype=np.float32).copy()  # 可写副本（整画布持有）
for t in range(0, len(tri_idx), 3):
    i0, i1, i2 = tri_idx[t], tri_idx[t + 1], tri_idx[t + 2]
    p0, p1, p2 = (np.array(to_canvas(pts_json[i])) for i in (i0, i1, i2))
    # 屏幕空间三角形光栅化 + 重心坐标采样（uv.v 直接映射 PIL y：v=0→顶部）
    def uvpx(i):
        return np.array([uv[i][0] * (rw - 1), uv[i][1] * (rh - 1)])
    a0, a1, a2 = uvpx(i0), uvpx(i1), uvpx(i2)
    xmin = int(min(p0[0], p1[0], p2[0])); xmax = int(max(p0[0], p1[0], p2[0])) + 1
    ymin = int(min(p0[1], p1[1], p2[1])); ymax = int(max(p0[1], p1[1], p2[1])) + 1
    den = ((p1[1]-p2[1])*(p0[0]-p2[0]) + (p2[0]-p1[0])*(p0[1]-p2[1]))
    if abs(den) < 1e-9: continue
    for yy in range(max(0, ymin), min(Hc, ymax)):
        for xx in range(max(0, xmin), min(Wc, xmax)):
            w0 = ((p1[1]-p2[1])*(xx-p2[0]) + (p2[0]-p1[0])*(yy-p2[1])) / den
            w1 = ((p2[1]-p0[1])*(xx-p2[0]) + (p0[0]-p2[0])*(yy-p2[1])) / den
            w2 = 1 - w0 - w1
            if w0 < -0.001 or w1 < -0.001 or w2 < -0.001: continue
            su = w0*a0 + w1*a1 + w2*a2
            sx, sy = int(round(su[0])), int(round(su[1]))
            if 0 <= sx < rw and 0 <= sy < rh:
                cnv[yy, xx] = reg[sy, sx]
canvas = Image.fromarray(cnv.astype(np.uint8))

dr = ImageDraw.Draw(canvas)
P = [to_canvas(p) for p in pts_json]
# 四边形边 v0-v1-v2-v3
dr.line(P + [P[0]], fill=(255, 80, 80, 255), width=2)
# u 轴（v1→v0）：构建期 hull 方向
dr.line([P[1], P[0]], fill=(0, 255, 0, 255), width=2)
# 顶点序号
for i, p in enumerate(P):
    dr.ellipse([p[0]-4, p[1]-4, p[0]+4, p[1]+4], fill=(255, 255, 0, 255))
    dr.text((p[0]+6, p[1]-12), f"v{i} uv({uv[i][0]:.0f},{uv[i][1]:.0f})", fill=(255,255,255,255))

ang_u = math.degrees(math.atan2(pts_json[0][1]-pts_json[1][1], pts_json[0][0]-pts_json[1][0]))
print(f"u 轴（v1→v0，JSON 系角）= {ang_u:.2f}°  （JSON y-up：负角=枪口端向下）")
canvas.save(r"D:\项目\dragonraja\spikes\LingChar\tools\diag_dir7_quad_render.png")
print("落盘 tools/diag_dir7_quad_render.png（红=网格四边形边 绿=hull u轴 黄=顶点）")
