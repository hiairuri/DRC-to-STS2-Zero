# -*- coding: utf-8 -*-
# 灰色矩形块的精确边界与性质
from PIL import Image

NEW = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T18-05-27-954Z-d9cd3f6f.jpg"
OLD = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T09-08-36-813Z-d8b15f17.png"
im = Image.open(NEW).convert("RGB")
old = Image.open(OLD).convert("RGB")
def lum(p): return 0.299*p[0]+0.587*p[1]+0.114*p[2]

print("== 新图：垂直扫描线（人物白裙/头发区）lum(y)，y 140..300 ==")
px = im.load()
for x in (250, 280, 310, 330):
    row=[]
    for y in range(140, 300, 6):
        r,g,b = px[x,y][:3]
        row.append(f"y{y}:{lum((r,g,b)):.0f}")
    print(f"  x={x}: " + " ".join(row))

print("== 新图：垂直扫描线细扫（找精确过渡 y，步长2）==")
for x in (280, 310):
    prev=None
    print(f"  x={x}:")
    for y in range(150, 300, 2):
        r,g,b = px[x,y][:3]
        L=lum((r,g,b))
        if prev is not None and abs(L-prev)>25:
            print(f"    jump at y={y}: {prev:.0f}->{L:.0f}  RGB=({r},{g},{b})")
        prev=L

print("== 新图：水平扫描 y=205 与 y=230，x 180..520（找灰色块左右缘）==")
for y in (200, 205, 212, 230, 240):
    row=[]
    for x in range(180, 524, 12):
        r,g,b = px[x,y][:3]
        row.append(f"{lum((r,g,b)):.0f}")
    print(f"  y={y}: " + " ".join(row))

print("== 新图：灰色块颜色 vs 白裙（同列上下取样）==")
for x,ya,yb in ((280,205,240),(300,205,240),(320,207,242)):
    ra,ga,ba = px[x,ya][:3]; rb,gb,bb = px[x,yb][:3]
    print(f"  x={x}: above({ya})=({ra},{ga},{ba}) below({yb})=({rb},{gb},{bb})"
          f" ratio=({ra/max(rb,1):.2f},{ga/max(gb,1):.2f},{ba/max(bb,1):.2f})")

print("== 旧图：同位置是否也有灰色块？垂直扫描 x=170,190,210 y 20..200 ==")
px2 = old.load()
for x in (170, 190, 210, 230):
    row=[]
    for y in range(20, 200, 8):
        r,g,b = px2[x,y][:3]
        row.append(f"y{y}:{lum((r,g,b)):.0f}")
    print(f"  x={x}: " + " ".join(row))

print("== 旧图：人物区（x 150-240）上/下带三通道 ==")
def avg(img, xa, xb, ya, yb):
    p=imgg= img = imgg = None
    pp = img = None
    p = img.load() if False else None
    p = img
    return None
def avg2(img, xa, xb, ya, yb):
    pp = img.load(); sr=sg=sb=n=0
    for y in range(ya,yb):
        for x in range(xa,xb):
            r,g,b = pp[x,y][:3]; sr+=r; sg+=g; sb+=b; n+=1
    return sr/n, sg/n, sb/n
for xa,xb in ((150,240),):
    ra,ga,ba = avg2(old, xa,xb,60,90)
    rb,gb,bb = avg2(old, xa,xb,130,160)
    print(f"  x[{xa},{xb}) above=({ra:.0f},{ga:.0f},{ba:.0f}) below=({rb:.0f},{gb:.0f},{bb:.0f})")
