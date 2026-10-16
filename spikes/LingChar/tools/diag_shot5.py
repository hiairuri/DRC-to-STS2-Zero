# -*- coding: utf-8 -*-
# 第五轮定向取证：亮度分割线的乘法模型检验 + dither 检测 + 历史对照
from PIL import Image
import math

NEW = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T18-05-27-954Z-d9cd3f6f.jpg"
OLD = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T09-08-36-813Z-d8b15f17.png"

def lum(p): return 0.299*p[0]+0.587*p[1]+0.114*p[2]

def avg(im, xa, xb, ya, yb):
    px = im.load(); sr=sg=sb=n=0
    for y in range(ya, yb):
        for x in range(xa, xb):
            r,g,b = px[x,y][:3]
            sr+=r; sg+=g; sb+=b; n+=1
    return sr/n, sg/n, sb/n

def edge_trace(im, xa, xb, y0, y1, step=40, win=3):
    px = im.load(); out=[]
    for x in range(xa, xb, step):
        best=(0,None)
        for y in range(y0, y1):
            def rl(yy):
                s=0; c=0
                for dx in range(-win,win+1):
                    if 0<=x+dx<im.width:
                        s+=lum(px[x+dx,yy]); c+=1
                return s/c
            d = rl(y+1)-rl(y)
            if abs(d)>abs(best[0]): best=(d,y)
        out.append((x,best[1],best[0]))
    return out

def checker_residual(im, xa, ya, size=32):
    """2x2 棋盘模板匹配：高频残差幅度"""
    px = im.load()
    g = [[lum(px[x,y][:3]) for x in range(xa,xa+size)] for y in range(ya,ya+size)]
    n=size*size; mu=sum(map(sum,g))/n
    # 2x2 checkerboard correlation
    num=0.0
    for y in range(size-1):
        for x in range(size-1):
            h = (g[y][x]-g[y][x+1]) - (g[y+1][x]-g[y+1][x+1])
            num += h*h
    checker_amp = math.sqrt(num/(size-1)**2)
    # 相邻像素差分 RMS（高频能量）
    dsum=0.0; cnt=0
    for y in range(size):
        for x in range(size-1):
            d=g[y][x]-g[y][x+1]; dsum+=d*d; cnt+=1
    for y in range(size-1):
        for x in range(size):
            d=g[y][x]-g[y+1][x]; dsum+=d*d; cnt+=1
    grad_rms = math.sqrt(dsum/cnt)
    return mu, checker_amp, grad_rms

print("="*72)
print("A. 新图（01:52 构建，02:05 截屏）536x650")
im = Image.open(NEW).convert("RGB")
W,H = im.size
print("A1. 分割线 y(x) 轨迹（搜索 y 180-270，|dLum| 最大处）")
for x,y,d in edge_trace(im, 20, W-20, 180, 270):
    print(f"    x={x:3d}  y={y}  dLum={d:+.1f}")

print("A2. 背景三通道比值（上带 y 195-215 / 下带 y 235-255，分 x 段）")
for tag,(xa,xb) in (("left",  (20,150)),("mid-bg",(420,470)),("far-right",(480,530))):
    ra,ga,ba = avg(im, xa,xb,195,215)
    rb,gb,bb = avg(im, xa,xb,235,255)
    print(f"    {tag:9s} x[{xa},{xb})  above=({ra:5.1f},{ga:5.1f},{ba:5.1f})  below=({rb:5.1f},{gb:5.1f},{bb:5.1f})"
          f"  R={ra/rb:.3f} G={ga/gb:.3f} B={ba/bb:.3f}")

print("A3. 人物区上/下（x 240-400）")
ra,ga,ba = avg(im,240,400,190,212)
rb,gb,bb = avg(im,240,400,232,254)
print(f"    above=({ra:.1f},{ga:.1f},{ba:.1f}) below=({rb:.1f},{gb:.1f},{bb:.1f}) R={ra/rb:.3f} G={ga/gb:.3f} B={ba/bb:.3f}")

print("A4. dither 检测（上方压暗区 32x32 patch，2x2 棋盘幅度 / 梯度RMS）")
for xa,ya in ((30,200),(60,205),(430,200),(460,205),(250,196),(300,198)):
    mu,ca,gr = checker_residual(im, xa, ya)
    print(f"    patch@({xa},{ya}) mean={mu:6.1f} checker2x2={ca:5.2f} gradRMS={gr:5.2f}")
print("    [对照] 下方未压暗区:")
for xa,ya in ((30,300),(430,300),(250,420)):
    mu,ca,gr = checker_residual(im, xa, ya)
    print(f"    patch@({xa},{ya}) mean={mu:6.1f} checker2x2={ca:5.2f} gradRMS={gr:5.2f}")

print("A5. 线本身颜色：y_edge 处 ±2px 与上下对比（x=440 枪口右侧背景）")
px = im.load()
for y in range(206, 240, 2):
    r,g,b = px[445,y][:3]
    print(f"    x=445 y={y}: RGB=({r},{g},{b}) lum={lum((r,g,b)):.0f}")

print("="*72)
print("B. 历史对照图（旧构建）544x239")
old = Image.open(OLD).convert("RGB")
W2,H2 = old.size
print("B1. 分割线 y(x) 轨迹（搜索 y 60-160）")
for x,y,d in edge_trace(old, 10, W2-10, 60, 160, step=40):
    print(f"    x={x:3d}  y={y}  dLum={d:+.1f}")

print("B2. 线的横向范围：y=97..107 每列最大亮度（找线的起止 x）")
px2 = old.load()
row=[]
for x in range(0, W2, 16):
    m = max(lum(px2[x,y][:3]) for y in range(90,115))
    row.append((x, m))
print("    " + " ".join(f"x{x}:{m:.0f}" for x,m in row))

print("B3. 旧图线上/线下三通道比值（x 段，避开人物 x<180）")
for tag,(xa,xb) in (("right-bg",(460,540)),("mid-bg",(200,300))):
    ra,ga,ba = avg(old, xa,xb,60,88)
    rb,gb,bb = avg(old, xa,xb,120,150)
    print(f"    {tag:9s} x[{xa},{xb})  above=({ra:5.1f},{ga:5.1f},{ba:5.1f})  below=({rb:5.1f},{gb:5.1f},{bb:5.1f})"
          f"  R={ra/rb:.3f} G={ga/gb:.3f} B={ba/bb:.3f}")

print("B4. 旧图线中心取样（x=380, y 90..112）")
for y in range(90,113,2):
    r,g,b = px2[380,y][:3]
    print(f"    x=380 y={y}: RGB=({r},{g},{b}) lum={lum((r,g,b)):.0f}")

print("B5. 旧图 dither 检测（线上方压暗区 vs 下方）")
for xa,ya,tag in ((200,40,"above-mid"),(460,40,"above-right"),(200,160,"below-mid"),(460,170,"below-right")):
    mu,ca,gr = checker_residual(old, xa, ya, 32)
    print(f"    {tag:11s} patch@({xa},{ya}) mean={mu:6.1f} checker2x2={ca:5.2f} gradRMS={gr:5.2f}")
