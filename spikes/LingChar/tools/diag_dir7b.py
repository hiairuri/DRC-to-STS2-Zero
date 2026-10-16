# -*- coding: utf-8 -*-
"""
diag_dir7b.py —— 第七轮取证：截图像素双角测量
================================================
1. 光束轴角：蓝/青掩码（B 高、R 低、亮度高）PCA 主轴，限定枪口右侧区域；
2. 枪管：裁剪枪身区域放大落盘，供人工指定滑套两端坐标后二次拟合（diag_dir7c.py）。
输出：两截图（新=21-01-14、旧=20-03-38）的光束角 + 枪口坐标。
"""
import math
import numpy as np
from PIL import Image

SHOTS = {
    "new": r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T21-01-14-817Z-efc90cb2.jpg",
    "old": r"C:\Users\jirai\.workbuddy\clipboard-images\clipboard-2026-10-10T20-03-38-018Z-c15ba30c.jpg",
}

# 枪身裁剪区（x0,y0,x1,y1）——人工预估，裁后放大 6x 供取点
GUN_BOX = {
    "new": (380, 235, 530, 330),
    "old": (340, 150, 470, 225),
}

def beam_angle(im, box):
    """在 box 区域内用蓝色掩码 PCA 求光束主轴角（屏幕系，y 向下，度）"""
    a = np.asarray(im.crop(box).convert("RGB"), dtype=np.int32)
    r, g, b = a[..., 0], a[..., 1], a[..., 2]
    # 蓝/青光束：B 明显高于 R，且足够亮
    mask = (b > 140) & (b - r > 40) & (g > 60)
    ys, xs = np.nonzero(mask)
    if len(xs) < 30:
        return None, 0, None
    x0, y0 = xs.mean(), ys.mean()
    cov = np.cov(np.stack([xs - x0, ys - y0]))
    w, v = np.linalg.eigh(cov)
    main = v[:, np.argmax(w)]  # 主轴方向
    ang = math.degrees(math.atan2(main[1], main[0]))
    if ang > 90: ang -= 180
    if ang < -90: ang += 180
    # 光束起点（枪口）= 距 PCA 中心最远的低 x 端：取掩码中 x 最小 5% 分位的均值点
    sel = xs <= np.percentile(xs, 2)
    muzzle = (xs[sel].mean() + box[0], ys[sel].mean() + box[1])
    return ang, int(len(xs)), muzzle

for key, path in SHOTS.items():
    im = Image.open(path)
    W, H = im.size
    print(f"=== {key} 截图 size={W}x{H} ===")
    # 光束区域：枪口右上大片区域
    box = (int(W * 0.55), 0, W, int(H * 0.55))
    ang, n, muz = beam_angle(im, box)
    if ang is None:
        print("  [FAIL] 光束掩码点不足")
    else:
        print(f"  光束轴角 = {ang:.1f}°  (掩码 {n} px, 掩码区左下沿≈枪口 {muz[0]:.0f},{muz[1]:.0f})")
    # 枪身裁剪放大
    gb = GUN_BOX[key]
    crop = im.crop(gb)
    scale = 6
    crop.resize((crop.width * scale, crop.height * scale), Image.LANCZOS).save(
        rf"D:\项目\dragonraja\spikes\LingChar\tools\diag_dir7_gun_{key}_x6.png")
    print(f"  枪身裁剪 {gb} 已放大落盘 diag_dir7_gun_{key}_x6.png")
