# -*- coding: utf-8 -*-
# 枪部放大导出 + 光束脊线端点输出（辅助手工选枪管两端点）
import numpy as np
from PIL import Image

SRC = r'C:/Users/jirai/.workbuddy/clipboard-images/clipboard-2026-10-11T02-00-21-001Z-6f36393e.jpg'
im = Image.open(SRC).convert('RGB')
a = np.asarray(im).astype(np.float32)
R, G, B = a[..., 0], a[..., 1], a[..., 2]

# 光束脊线端点（同 diag_dir8_shot.py 管线）
blue = (B > 120) & (B > R + 40) & (B > G + 30)
ys, xs = np.nonzero(blue)
keep = (xs > 330) & (ys < 300)
cols = {}
for x, y in zip(xs[keep], ys[keep]):
    cols.setdefault(x, []).append(y)
cx = np.array(sorted(cols)); cy = np.array([np.median(cols[x]) for x in cx])
for it in range(3):
    k, c = np.polyfit(cx, cy, 1)
    res = np.abs(k * cx + c - cy)
    m = res < max(1.5, np.percentile(res, 70))
    cx, cy = cx[m], cy[m]
k, c = np.polyfit(cx, cy, 1)
print(f'光束脊线: y = {k:.5f}*x + {c:.2f};  起点 x={cx.min()} y={k*cx.min()+c:.1f};  终点 x={cx.max()} y={k*cx.max()+c:.1f}')
print(f'首列(x={cx[:3]}) 中位y={cy[:3]};  末列(x={cx[-3:]}) 中位y={cy[-3:]}')

# 枪部放大 ×8（x300..400, y195..265）
crop = im.crop((300, 195, 400, 265)).resize((800, 560), Image.NEAREST)
crop.save(r'D:/项目/dragonraja/spikes/LingChar/tools/diag_dir8_gun_x8.png')
print('已导出 tools/diag_dir8_gun_x8.png（对应原图 x300..400, y195..265）')
