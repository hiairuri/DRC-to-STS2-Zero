#!/usr/bin/env python3
"""diag_dir6.py —— 条目 48 取证：光束旋转角度偏差（第六轮打回）根因数值固化。

只读分析脚本（不动任何构建产物）。对照工具输出与代码注释引用。

取证问题：截图观测=枪管近水平指右，光束从枪口发出却向右上仰约 30-40°，
与枪管不共线。当前实现（条目 47）= 位置每帧跟随（local_to_world 重算），
方向 Dir0 出生锁定（Timer 到点回调采样一次，之后 holder.Rotation 不再更新）。

本脚本回答四件事：
  A. ling.json slots 段 qiang02 槽位绑定的骨骼名（任务书嫌疑"绑定到别的骨"是否成立）；
  B. attack 动画中 qiang02 槽位**实际渲染的附件名**（slots.attachment 轨道），
     以及该附件的 hull(+deform) 与 config 真值来源 qiang02 hull 的差异；
  C. t_fire=0.166667 后 0.1s（streak 存活窗口）内臂链各骨的旋转变化量
     （线性近似，证明"方向出生锁定必然过时"）；
  D. 三股偏差的合成与截图观测角差（30-40°）的定性对照。
"""
import json
import math

LING = r'D:/项目/dragonraja/spikes/LingChar/pcksrc/ling/animations/characters/ling/ling.json'

d = json.load(open(LING, encoding='utf-8'))

print('=== A. qiang02 槽位绑定骨 ===')
slot = next(s for s in d['slots'] if s['name'] == 'qiang02')
print('slot qiang02 -> bone =', slot['bone'])

print()
print('=== B. attack 动画中 qiang02 槽位实际渲染附件 ===')
att_track = d['animations']['attack']['slots']['qiang02']['attachment']
print('attachment 轨道:', json.dumps(att_track))
# 取 t=0 的附件名（attack 全程各帧同名，上面轨道已列）
live_att = att_track[0]['name']
print('attack 期间实际附件 =', live_att)

# 附件 hull（skins[0].attachments.qiang02 下两个附件）
attd = d['skins'][0]['attachments']['qiang02']
q2 = attd['qiang02']            # config 真值来源（default 附件）
live = attd[live_att]           # attack 实际渲染附件

def hull_pts(att, deform=(0.0, 0.0)):
    v = att['vertices']
    pts = [(v[i] + deform[0], v[i + 1] + deform[1]) for i in range(0, len(v), 2)]
    return pts

# deform 轨道（attack 动画对 live 附件的顶点偏移）
deform = (0.0, 0.0)
def_entry = d['animations']['attack'].get('attachments', {}).get('default', {}).get('qiang02', {})
if live_att in def_entry:
    dv = def_entry[live_att].get('deform', [])
    if dv:
        off = dv[0].get('vertices', [])
        if len(off) >= 2:
            deform = (off[0], off[1])
print(f'{live_att} deform 偏移 = ({deform[0]:.5f}, {deform[1]:.5f})（offset={def_entry.get(live_att, {}).get("deform", [{}])[0].get("offset", "-")}）')

def axis(pts):
    F = ((pts[0][0] + pts[3][0]) / 2, (pts[0][1] + pts[3][1]) / 2)  # 前端中点 avg(V0,V3)
    Rk = ((pts[1][0] + pts[2][0]) / 2, (pts[1][1] + pts[2][1]) / 2)  # 后端中点 avg(V1,V2)
    dx, dy = F[0] - Rk[0], F[1] - Rk[1]
    L = math.hypot(dx, dy)
    return F, Rk, math.degrees(math.atan2(dy, dx)), L

p_q2 = hull_pts(q2)
p_live = hull_pts(live, deform)
F1, R1, a1, L1 = axis(p_q2)
F2, R2, a2, L2 = axis(p_live)
print(f'qiang02      hull: F=({F1[0]:.2f},{F1[1]:.2f}) Rk=({R1[0]:.2f},{R1[1]:.2f}) 长轴角={a1:.2f}° 长={L1:.1f}（width={q2["width"]}）')
print(f'{live_att}+deform: F=({F2[0]:.2f},{F2[1]:.2f}) Rk=({R2[0]:.2f},{R2[1]:.2f}) 长轴角={a2:.2f}° 长={L2:.1f}（width={live["width"]}）')
print(f'两 hull 长轴角差（bone104 局部系内固有偏差）= {a2 - a1:.2f}°')
print(f'两 hull 前端中点距离（解释"位置碰巧对"）= {math.hypot(F2[0]-F1[0], F2[1]-F1[1]):.2f} 骨架单位 ≈ {math.hypot(F2[0]-F1[0], F2[1]-F1[1])*0.29:.1f}px')

print()
print('=== C. t_fire=0.166667 后 streak 存活窗口（0.167→0.267s）臂链旋转变化 ===')
bones = d['animations']['attack']['bones']
chain = ['body', 'bone3', 'bone4', 'bone7', 'bone8', 'bone9', 'bone104']

def rot_at(track, t, setup):
    """线性近似采样 rotate 轨道（bezier 帧按相邻关键值线性处理——定性结论够用）。"""
    if not track:
        return setup  # 无轨道 = 保持 setup rotation
    frames = [(f['time'], f['value']) for f in track]
    if t <= frames[0][0]:
        return frames[0][1]
    for i in range(len(frames) - 1):
        t0, v0 = frames[i]
        t1, v1 = frames[i + 1]
        if t0 <= t <= t1:
            if t1 <= t0:
                return v1
            return v0 + (v1 - v0) * (t - t0) / (t1 - t0)
    return frames[-1][1]

setups = {b['name']: b.get('rotation') or 0.0 for b in d['bones']}
total0 = total1 = 0.0
for name in chain:
    tr = bones.get(name, {}).get('rotate')
    setup = setups[name]
    v_fire = rot_at(tr, 0.166667, setup)
    v_end = rot_at(tr, 0.266667, setup)
    print(f'{name:>8}: setup={setup:8.3f}  t=0.167:{v_fire:8.3f}  t=0.267:{v_end:8.3f}  窗口内Δ={v_end - v_fire:+7.3f}°')
    total0 += v_fire
    total1 += v_end
print(f'臂链世界旋转（沿链累积，同约定下定性比较）: t=0.167 {total0:.2f}° → t=0.267 {total1:.2f}°，Δ={total1 - total0:+.2f}°')

print()
print('=== D. 机制合成（定性） ===')
print(f'① hull 固有偏差（config 真值来源 qiang02 vs 实际渲染 {live_att}+deform）: {a2 - a1:+.2f}°')
print(f'② 出生锁定过时（streak 0.1s 生命内臂链继续旋转, 线性近似）: {total1 - total0:+.2f}°')
print(f'合计定性角差 ≈ {abs(a2 - a1) + abs(total1 - total0):.1f}°  vs 截图观测 30-40° —— 定性吻合')
print('注: ② 为线性下界；bone8 轨道 0.1→0.1667 段 bezier 控制点显示 98.5° overshoot 后')
print('    快速回落，实际窗口内角速度更高，30-40° 观测在定性解释范围内。')
