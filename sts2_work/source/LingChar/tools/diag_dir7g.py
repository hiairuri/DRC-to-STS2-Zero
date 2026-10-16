# -*- coding: utf-8 -*-
"""
diag_dir7g.py —— 贴图内膛线角精测 + 修正向量推导
==================================================
第七轮取证定案（详见主日志条目 49）：
  A. 附件变换假设证伪（zero_qiang_C2/qiang02 rotation=0,x=0,y=0,scale=1）；
  B. 光束冻结实锤：两轮打回截图的光束角与各自会话日志 dir0/dir2 精确吻合
     （旧 -40.63°≈-40.5、新 -21.92°≈-22.0），而枪管已回摆（-1.8°/-5.0°）
     → follower 出生后不再更新（位置+旋转双冻结），持有的 bone 包装对象
     在 burst 后失效/陈旧（部署 dll 反编译含旋转跟随代码，排除漏编译）；
  C. 贴图内倾角实锤：mesh 四边形 u 轴（构建期 hull 方向）与枪管膛线在
     贴图内不平行——diag_dir7e.py 渲染目测约 5°+。
本脚本：对 C2 图集区域拟合滑套顶缘直线（膛线平行于滑套），推导：
  1) 膛线在区域像素系的方向角 θ_top；
  2) 经网格 UV 仿射映射到 bone104 本地系的膛线方向 bore_bone（JSON 系）；
  3) 预测屏幕系本地角 = -atan2(bore_bone.y, bore_bone.x)（运行时 bone 本地
     系相对 JSON y 翻转，实证：u 轴 JSON -15.27° ↔ 屏幕 +15.3），
     并与 u 轴屏幕本地角 +15.3 对照，输出修正量。
"""
import json
import math
import numpy as np
from PIL import Image

LING = r"D:\项目\dragonraja\spikes\LingChar\pcksrc\ling\animations\characters\ling"

# ---- 1) C2 区域滑套顶缘拟合 ----
page = Image.open(LING + r"\Chr_ling001.png").convert("RGBA")
rx, ry, rw, rh = 338, 279, 77, 56  # zero_qiang_C 区域（atlas bounds，无旋转）
a = np.asarray(page, dtype=np.int32)
alpha = a[..., 3]
pts = []
for x in range(rx + 2, rx + rw - 4):  # 避开区域边缘半透明杂点
    col = alpha[ry:ry + rh, x]
    nz = np.nonzero(col > 40)[0]
    if len(nz):
        pts.append((x - rx, nz[0]))  # 区域内坐标（y 向下=顶缘）
pts = np.array(pts, dtype=np.float64)
# 鲁棒直线拟合（准星凸起是离群点：两轮拟合，第二轮剔除残差>2px 的列）
k, c = np.polyfit(pts[:, 0], pts[:, 1], 1)
for _ in range(3):
    r = np.abs(pts[:, 1] - (k * pts[:, 0] + c))
    keep = r <= max(2.0, np.percentile(r, 80))
    k, c = np.polyfit(pts[keep, 0], pts[keep, 1], 1)
theta_top = math.degrees(math.atan(k))
resid = np.abs(pts[keep, 1] - (k * pts[keep, 0] + c)).max()
print(f"C2 区域滑套顶缘角 θ_top = {theta_top:.2f}° ({int(keep.sum())}/{len(pts)} 列, 剔离群后最大残差 {resid:.1f}px)")

# ---- 2) UV 仿射映射：区域像素方向 → bone104 本地系（JSON 系）----
d = json.load(open(LING + r"\ling.json", encoding="utf-8"))
att = d["skins"][0]["attachments"]["qiang02"]["zero_qiang_C2"]
verts = list(att["vertices"])
deform = (20.38768, -32.89978)
v0 = (verts[0] + deform[0], verts[1] + deform[1])  # uv(1,1)
v1 = (verts[2] + deform[0], verts[3] + deform[1])  # uv(0,1)
v2 = (verts[4] + deform[0], verts[5] + deform[1])  # uv(0,0)
v3 = (verts[6] + deform[0], verts[7] + deform[1])  # uv(1,0)
# 平行四边形校验（容差 1e-3：JSON 顶点为 6 位小数浮点）
assert abs(v0[0] + v2[0] - v1[0] - v3[0]) < 1e-3 and abs(v0[1] + v2[1] - v1[1] - v3[1]) < 1e-3
# bone(u,v) = v1 + u*(v0-v1) + (1-v)*(v2-v1)；u=x/(rw-1)，v=y/(rh-1)
U = (v0[0] - v1[0], v0[1] - v1[1])   # ∂bone/∂u
W = (v2[0] - v1[0], v2[1] - v1[1])   # ∂bone/∂(1-v)
# 区域像素方向 (cosθ, sinθ)（y 向下）→ bone 位移：
#   Δx_px → +U/(rw-1)；Δy_px → ∂bone/∂v · 1/(rh-1) = -W/(rh-1)
bx = math.cos(math.radians(theta_top)) * U[0] / (rw - 1) - math.sin(math.radians(theta_top)) * W[0] / (rh - 1)
by = math.cos(math.radians(theta_top)) * U[1] / (rw - 1) - math.sin(math.radians(theta_top)) * W[1] / (rh - 1)
ang_json = math.degrees(math.atan2(by, bx))
n = math.hypot(bx, by)
print(f"膛线 bone 方向（JSON 系）= ({bx:.4f},{by:.4f}) 归一 = ({bx/n:.4f},{by/n:.4f})  JSON 系角 = {ang_json:.2f}°")

# ---- 3) 屏幕系对照与修正量 ----
# 运行时 bone 本地系相对 JSON y 翻转（实证：u 轴 JSON -15.27° → dirdeg-wr=+15.3）
u_ang_json = math.degrees(math.atan2(v0[1] - v1[1], v0[0] - v1[0]))
print(f"\nu 轴 JSON 角 = {u_ang_json:.2f}° → 屏幕本地角 = {-u_ang_json:.2f}°（实证值 +15.3）")
print(f"膛线 屏幕本地角 = {-ang_json:.2f}°  → 修正量（膛线相对 u 轴）= {-ang_json - (-u_ang_json):.2f}°")
# 交叉验证：修正后屏幕本地角应≈截图枪管测量所隐含的本地角
# （新截图：枪管 -5.0°，若 wr≈-14.7 则本地≈+9.7）
print(f"\n交叉验证：新截图枪管 -5.0°，隐含本地角 ≈ -5.0 - wr(≈-14.7) = +9.7°；预测 {-ang_json:.2f}°")
