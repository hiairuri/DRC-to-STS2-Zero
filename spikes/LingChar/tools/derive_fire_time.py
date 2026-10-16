# -*- coding: utf-8 -*-
"""derive_fire_time.py —— 从 ling.json animations.attack 骨骼轨道推导开火时刻 t_fire。

背景（用户反馈 #2）：重编辑后的骨架动画没有第二段攻击（UE 原版双发语义不
适用），且第一拍发射时点不应是 t=0，应取"手臂/枪前伸到位"的瞬间。

方法（数据链优先，无 events 轨时的静态推导）：
1. 枪口锚骨 bone104（config bone 键）的 translate/rotate/scale 轨道极值时刻。
2. IK 轨 IK_L_z / IK_L_z1 / IK_R_z2 的 mix/bendDirection 极值时刻（IK 主导
   时手臂到位=IK 目标锁定瞬间）。
3. 持枪链相关骨骼（bone104 祖先/近邻）translate 极值时刻。

输出各轨道的关键时间点与极值，供人工比对后写入 config bursts=[t_fire]。
只读分析脚本，不改任何产物。
"""
import json
import sys

PATH = r"D:/项目/dragonraja/spikes/LingChar/pcksrc/ling/animations/characters/ling/ling.json"

# 关注骨骼：枪口锚骨 + IK 轨 + 持枪链候选（bone104/qiang02 同链的近邻编号）
FOCUS_BONES = ["bone104", "bone103", "bone105", "qiang02", "bone10", "bone33"]
FOCUS_IK = ["IK_L_z", "IK_L_z1", "IK_R_z2"]


def curve_times(tl):
    """兼容 timeline 格式：bezier 关键帧列表或 [t,v] 步进数组。"""
    out = []
    for f in tl:
        if isinstance(f, dict):
            out.append(f.get("time", 0.0))
        elif isinstance(f, list) and len(f) >= 2:
            out.append(f[0])
    return out


def track_extremes(tl):
    """返回 (time, value) 列表（含 bezier 中间控制点的近似极值不展开，
    只取关键帧值——重编辑动画关键帧密度足够定位极值）。"""
    pts = []
    for f in tl:
        if isinstance(f, dict):
            if "value" in f:
                pts.append((f.get("time", 0.0), f["value"]))
            elif "angle" in f:
                pts.append((f.get("time", 0.0), f["angle"]))
            elif "x" in f or "y" in f:
                pts.append((f.get("time", 0.0), (f.get("x", 0.0), f.get("y", 0.0))))
        elif isinstance(f, list) and len(f) >= 2:
            pts.append((f[0], f[1]))
    return pts


def main():
    d = json.load(open(PATH, encoding="utf-8"))
    a = d["animations"]["attack"]

    # 动画总时长：全轨道最大关键时间
    tmax = 0.0

    def scan(tl):
        nonlocal tmax
        for t in curve_times(tl):
            tmax = max(tmax, t)

    for ch in ("bones", "slots", "ik"):
        for name, tracks in a.get(ch, {}).items():
            # slots/ik 的值可能直接是 timeline（channel->list），统一摊平
            chans = tracks.items() if isinstance(tracks, dict) else [(ch, tracks)]
            for cname, tl in chans:
                if isinstance(tl, list):
                    scan(tl)
    print(f"attack 动画总时长 ≈ {tmax:.6f}s（全轨道最大关键时间）")
    print(f"有无 events 轨：{'有' if 'events' in a else '无（走骨骼轨道推导）'}")
    print()

    bones = a.get("bones", {})
    for name in FOCUS_BONES:
        tr = bones.get(name)
        if not tr:
            continue
        for cname in ("translate", "rotate", "scale"):
            tl = tr.get(cname)
            if not tl:
                continue
            pts = track_extremes(tl)
            print(f"[bone] {name}.{cname}: 关键帧 {pts}")

    for name in FOCUS_IK:
        tr = a.get("ik", {}).get(name)
        if not tr:
            continue
        # ik 轨可能是 channel->list 或直接 list
        chans = tr.items() if isinstance(tr, dict) else [(name, tr)]
        for cname, tl in chans:
            if not isinstance(tl, list):
                continue
            pts = track_extremes(tl)
            print(f"[ik] {name}.{cname}: 关键帧 {pts}")

    # bone104 translate 极值定位：开火=枪沿枪管前伸到位（|Δ| 极大值时刻）
    tr104 = bones.get("bone104", {}).get("translate")
    if tr104:
        pts = track_extremes(tr104)
        norm = []
        for t, v in pts:
            if isinstance(v, tuple):
                norm.append((t, (v[0] ** 2 + v[1] ** 2) ** 0.5))
            else:
                norm.append((t, abs(v)))
        if norm:
            t_ext, v_ext = max(norm, key=lambda p: p[1])
            print()
            print(f"== bone104.translate 模长极大值：t={t_ext:.6f}s 值={v_ext:.4f} ==")


if __name__ == "__main__":
    sys.exit(main())
