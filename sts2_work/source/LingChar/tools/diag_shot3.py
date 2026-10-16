# -*- coding: utf-8 -*-
"""diag_shot3.py — 第三轮验收像素取证（只读诊断）"""
import sys, os
from PIL import Image
import math

OUT_DIR = r"D:\项目\dragonraja\spikes\LingChar\tools"
SHOT_NEW = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T09-08-36-813Z-d8b15f17.png"
SHOT_OLD = r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T04-23-25-387Z-9bff734e.png"
CROP_OUT = os.path.join(OUT_DIR, "diag_shot3_crop.png")

def hdr(t):
    print("\n" + "=" * 70)
    print(t)
    print("=" * 70)

def cyan_mask_analysis(img, tag, thr_b=150):
    """提取 B>R 且 B>150 的青蓝掩码，输出 bbox/统计/直方图"""
    hdr(f"[青蓝掩码] {tag}  size={img.size}")
    rgb = img.convert("RGB")
    px = rgb.load()
    w, h = rgb.size
    xs, ys, pts = [], [], []
    for y in range(h):
        for x in range(w):
            r, g, b = px[x, y]
            if b > r and b > thr_b:
                xs.append(x); ys.append(y); pts.append((x, y, r, g, b))
    if not pts:
        print("  无青蓝像素")
        return None
    x0, x1, y0, y1 = min(xs), max(xs), min(ys), max(ys)
    bw, bh = x1 - x0 + 1, y1 - y0 + 1
    n = len(pts)
    avg = tuple(round(sum(p[2 + i] for p in pts) / n, 1) for i in range(3))
    mx = tuple(max(p[2 + i] for p in pts) for i in range(3))
    print(f"  bbox: x={x0} y={y0} w={bw} h={bh}  像素数={n}  面积占bbox={n/(bw*bh)*100:.1f}%")
    print(f"  平均RGB={avg}  最大RGB={mx}")
    # 唯一颜色数（量化到 8 级/通道，避免噪声）
    q = {}
    for p in pts:
        key = (p[2] // 8, p[3] // 8, p[4] // 8)
        q[key] = q.get(key, 0) + 1
    print(f"  唯一颜色数(8级量化)={len(q)}")
    top = sorted(q.items(), key=lambda kv: -kv[1])[:10]
    print("  颜色直方图 top10 (r//8,g//8,b//8)->count, 换算中值RGB:")
    for (r8, g8, b8), c in top:
        print(f"    RGB~({r8*8+4},{g8*8+4},{b8*8+4})  count={c}  ({c/n*100:.1f}%)")
    # 连通块粗分：按 bbox 内 y 间隔分组（简单：对 y 范围做直方图找空隙）
    yrow = sorted(set(ys))
    groups = []
    start = prev = yrow[0]
    for y in yrow[1:]:
        if y - prev > 4:
            groups.append((start, prev)); start = y
        prev = y
    groups.append((start, prev))
    print(f"  y 向连通分组（间隔>4px 视为分离）: {groups}")
    return (x0, y0, bw, bh), pts, rgb

def gradient_analysis(rgb, box, tag):
    """对 bbox 区域：梯度幅值分布 + 长短轴亮度衰减"""
    x0, y0, bw, bh = box
    px = rgb.load()
    region = [(x, y) for x in range(x0, x0 + bw) for y in range(y0, y0 + bh)
              if (lambda c: c[2] > c[0] and c[2] > 150)(px[x, y])]
    if len(region) < 10:
        print("  梯度分析：样本不足"); return
    # 邻域梯度幅值
    grads = []
    pset = set((p[0], p[1]) for p in region)
    for (x, y) in region:
        c = px[x, y]
        for dx, dy in ((1, 0), (0, 1)):
            nx, ny = x + dx, y + dy
            if (nx, ny) in pset:
                c2 = px[nx, ny]
                g = abs(c[0]-c2[0]) + abs(c[1]-c2[1]) + abs(c[2]-c2[2])
                grads.append(g)
    grads.sort()
    n = len(grads)
    print(f"  邻域梯度(|dR|+|dG|+|dB|): n={n} p10={grads[n//10]} p50={grads[n//2]} p90={grads[9*n//10]} max={grads[-1]}")
    print(f"  梯度>60 的比例 = {sum(1 for g in grads if g > 60)/n*100:.1f}%  (>60 说明有硬边/块状跳变)")
    # PCA 求长轴，然后沿长/短轴采样亮度
    mx = sum(p[0] for p in region) / n
    my = sum(p[1] for p in region) / n
    sxx = sum((p[0]-mx)**2 for p in region) / n
    syy = sum((p[1]-my)**2 for p in region) / n
    sxy = sum((p[0]-mx)*(p[1]-my) for p in region) / n
    ang = 0.5 * math.atan2(2*sxy, sxx - syy)
    ux, uy = math.cos(ang), math.sin(ang)
    vx, vy = -uy, ux
    def sample_axis(dx_, dy_, name):
        print(f"  {name}轴亮度衰减(中心->两端, 步长={max(2, bw//12)}px, RGB和):")
        step = max(2, (bw if bw >= bh else bh) // 12)
        # 找轴上的最亮点作为中心
        best = max(region, key=lambda p: sum(px[p[0], p[1]]))
        cx, cy = best
        line = []
        for s in range(-40, 41):
            t = s * step // 4
            X, Y = int(cx + dx_ * t), int(cy + dy_ * t)
            if 0 <= X < rgb.width and 0 <= Y < rgb.height:
                c = px[X, Y]
                line.append((t, c if (c[2] > c[0] and c[2] > 100) else None))
        half = len(line) // 2
        seq = line[:half][::-1] + line[half:]  # 左端...中心...右端
        out = []
        for t, c in seq:
            out.append(f"{t:+d}:{'-'+str(c) if c is None else str(c)}" if False else
                       (f"{t:+d}:----" if c is None else f"{t:+d}:{sum(c)//3}"))
        print("    " + " ".join(out))
    sample_axis(ux, uy, "长")
    sample_axis(vx, vy, "短")

def mosaic_analysis(rgb, box, tag):
    """块状方差分析：4x4/8x8 分块，块内 vs 块间方差"""
    x0, y0, bw, bh = box
    px = rgb.load()
    hdr(f"[马赛克判定] {tag}")
    for bs in (4, 8):
        # 只统计 bbox 内青蓝占比高的块
        block_means = []
        block_internal = []
        for by in range(y0, y0 + bh, bs):
            for bx in range(x0, x0 + bw, bs):
                vals = []
                for y in range(by, min(by + bs, y0 + bh)):
                    for x in range(bx, min(bx + bs, x0 + bw)):
                        c = px[x, y]
                        if c[2] > c[0] and c[2] > 120:
                            vals.append(c)
                if len(vals) >= bs * bs * 0.5:
                    m = tuple(sum(v[i] for v in vals) / len(vals) for i in range(3))
                    var = sum(sum((v[i] - m[i])**2 for i in range(3)) for v in vals) / len(vals)
                    block_means.append(m)
                    block_internal.append(var)
        if len(block_means) < 4:
            print(f"  block={bs}: 有效块不足，跳过")
            continue
        gm = tuple(sum(b[i] for b in block_means) / len(block_means) for i in range(3))
        between = sum(sum((b[i] - gm[i])**2 for i in range(3)) for b in block_means) / len(block_means)
        within = sum(block_internal) / len(block_internal)
        ratio = between / max(within, 0.01)
        print(f"  block={bs}: 有效块数={len(block_means)} 块间方差={between:.0f} 块内方差={within:.0f} 比值={ratio:.1f}")
        if ratio > 5:
            print(f"    -> 块间跳变远大于块内变化：疑似块状/马赛克纹理（块大小~{bs}px）")
        else:
            print(f"    -> 块内变化 >= 块间变化：更像连续渐变")

def ascii_alpha(path, mode="alpha", N=16):
    im = Image.open(path)
    print(f"  {os.path.basename(path)}: size={im.size} mode={im.mode}")
    if im.mode in ("RGBA", "LA", "PA"):
        a = im.convert("RGBA").getchannel("A")
        ext = a.getextrema()
        print(f"  alpha min={ext[0]} mean={sum(a.getdata())/ (im.width*im.height):.1f} max={ext[1]}")
        ch = a
    else:
        ch = im.convert("L")
        print(f"  无 alpha（mode={im.mode}），打印亮度通道")
        ext = ch.getextrema()
        print(f"  L min={ext[0]} max={ext[1]}")
    w, h = im.size
    small = ch.resize((N, N), Image.BOX)
    chars = " .:-=+*#%@"
    for y in range(N):
        row = ""
        for x in range(N):
            v = small.getpixel((x, y))
            row += chars[min(9, v * 10 // 256)]
        print("    |" + row + "|")
    # RGB 通道
    if im.mode == "RGBA":
        rgbm = im.convert("RGB").resize((N, N), Image.BOX)
        print(f"  RGB（缩到 {N}x{N}，每格 (r,g,b) 简写为 R+G*10+B*100 的均值标记 / 表示深色）")
        for y in range(0, N, 2):
            row = ""
            for x in range(N):
                r, g, b = rgbm.getpixel((x, y))
                row += f"({r:3d},{g:3d},{b:3d})"
            print("    " + row)

def texture_report(path):
    hdr(f"[纹理核查] {path}")
    if not os.path.exists(path):
        print("  !! 文件不存在")
        return
    try:
        im = Image.open(path)
    except Exception as e:
        print(f"  !! 打开失败: {e}")
        return
    ascii_alpha(path, N=16 if max(im.size) <= 256 else 24)

def main():
    hdr("STEP1/2 截图像素取证")
    for tag, p in (("本轮截图", SHOT_NEW), ("上轮截图", SHOT_OLD)):
        im = Image.open(p)
        res = cyan_mask_analysis(im, tag)
        if res:
            box, pts, rgb = res
            gradient_analysis(rgb, box, tag)
            mosaic_analysis(rgb, box, tag)
            if tag == "本轮截图":
                x0, y0, bw, bh = box
                pad = 6
                crop = im.convert("RGB").crop((max(0, x0-pad), max(0, y0-pad),
                                               min(im.width, x0+bw+pad), min(im.height, y0+bh+pad)))
                crop = crop.resize((crop.width * 4, crop.height * 4), Image.NEAREST)
                crop.save(CROP_OUT)
                print(f"\n  已保存放大裁剪: {CROP_OUT}  ({crop.width}x{crop.height})")

    hdr("STEP3 纹理资产核查")
    files = [
        r"D:\项目\dragonraja\spikes\LingChar\assets\attack_fx\fx_streak_mask.png",
        r"D:\项目\dragonraja\spikes\LingChar\assets\attack_fx\fx_flash.png",
        r"D:\项目\dragonraja\spikes\LingChar\assets\attack_fx\fx_sparks3x3_grid.png",
        r"D:\ModForge\项目\dragonraja2\render_work\attack_tex\fr_obj_901_3x3.png",
        r"D:\项目\dragonraja2\render_work\attack_tex\fr_obj_901_3x3.png",
    ]
    for f in files:
        texture_report(f)

if __name__ == "__main__":
    main()
