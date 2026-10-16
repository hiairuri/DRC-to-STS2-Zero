# -*- coding: utf-8 -*-
# 定判 uv v 约定：白标记/准星等特征点在两种约定下的渲染落点 vs 截图实测
# 同时输出两约定下 image_bore(φ) → screen_local 的映射函数
import json, math
import numpy as np
from PIL import Image

BASE = r'D:/项目/dragonraja/spikes/LingChar/pcksrc/ling/animations/characters/ling'
j = json.load(open(BASE + r'\ling.json', encoding='utf-8'))
a = j['skins'][0]['attachments']['qiang02']['zero_qiang_C2']
V = [(a['vertices'][i], a['vertices'][i+1]) for i in range(0, 8, 2)]  # JSON系 y 向下
UV = [(a['uvs'][i], a['uvs'][i+1]) for i in range(0, 8, 2)]
# deform：attack 全程 (20.38768,-32.89978) 均匀平移
D = (20.38768, -32.89978)
Vd = [(x + D[0], y + D[1]) for x, y in V]
v0, v1, v2, v3 = Vd
U = (v0[0]-v1[0], v0[1]-v1[1]); W = (v2[0]-v1[0], v2[1]-v1[1])
un = math.hypot(*U); wn = math.hypot(*W)
uU = (U[0]/un, U[1]/un); uW = (W[0]/wn, W[1]/wn)
print(f'|U|={un:.2f} JSON角={math.degrees(math.atan2(U[1],U[0])):.2f}°  |W|={wn:.2f} JSON角={math.degrees(math.atan2(W[1],W[0])):.2f}°')
print(f'（77/0.45={77/0.45:.1f}, 56/0.45={56/0.45:.1f} —— |U|,|W| ≈ 原始纹理区域尺寸）')

# 纹理找白标记（低饱和度高亮度块）
page = Image.open(BASE + r'\Chr_ling001.png').convert('RGBA')
rx, ry, rw, rh = 338, 279, 77, 56
reg = np.asarray(page.crop((rx, ry, rx+rw, ry+rh))).astype(np.float32)
R, G, B, A = reg[...,0], reg[...,1], reg[...,2], reg[...,3]
mx = np.maximum(np.maximum(R,G),B); mn = np.minimum(np.minimum(R,G),B)
white = (A>200) & (mn>180) & (mx-mn<40)
ys, xs = np.nonzero(white)
print(f'白标记像素: {len(xs)}, 质心(px,py)=({xs.mean():.1f},{ys.mean():.1f}), px范围[{xs.min()},{xs.max()}] py范围[{ys.min()},{ys.max()}]')
pcx, pcy = xs.mean(), ys.mean()

def bone_from_px(px, py, conv):
    # conv='A': Spine标准 v=0=图像顶行（渲染竖直翻转）；conv='B': v=1=图像顶行（不翻转）
    u = px/(rw-1)
    v = py/(rh-1) if conv=='A' else 1-py/(rh-1)
    return (v1[0] + u*U[0] + (1-v)*W[0], v1[1] + u*U[1] + (1-v)*W[1])

for conv in 'AB':
    bx, by = bone_from_px(pcx, pcy, conv)
    # 枪体包围盒（四角变形后）与落点相对位置
    xs4 = [p[0] for p in Vd]; ys4 = [p[1] for p in Vd]
    relx = (bx-min(xs4))/(max(xs4)-min(xs4)); rely = (by-min(ys4))/(max(ys4)-min(ys4))
    print(f'约定{conv}: 白标记 → bone=({bx:.1f},{by:.1f}) 包围盒相对=({relx:.2f},{rely:.2f}) [rely 小=靠上]')

# 两约定下 image φ → screen_local（= -JSON角，y翻转实证）映射斜率与几组值
for conv in 'AB':
    sgn = -1 if conv=='A' else 1   # A: JSON=cosφ·uU−sinφ·uW; B: JSON=cosφ·uU+sinφ·uW
    print(f'约定{conv}: φ_img → screen_local:')
    for phi in (-24, -8, -2.42, 0, 8):
        c, s = math.cos(math.radians(phi)), math.sin(math.radians(phi))
        jx = sgn*s*uW[0] + c*uU[0]; jy = sgn*s*uW[1] + c*uU[1]
        print(f'   φ={phi:>6}: screen={-math.degrees(math.atan2(jy,jx)):>7.2f}°')
