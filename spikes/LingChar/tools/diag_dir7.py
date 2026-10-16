# -*- coding: utf-8 -*-
"""
diag_dir7.py —— 第七轮打回取证：附件变换（rotation/x/y/scale）假设验证
=====================================================================
嫌疑：构建期从 ling.json 提取 mesh hull 两点时，把 mesh 顶点坐标直接当
bone104 局部坐标喂 local_to_world，忽略了 mesh 附件自身的变换
（rotation/x/y/scaleX/scaleY）。若附件带 θ≈20-35° 的 rotation，
方向就会恒定偏 θ —— 与"位置贴得上、方向恒定歪、两轮幅度一致"吻合。

本脚本：
1. 解析 ling.json skins[0].attachments["qiang02"] 段，
   列出 zero_qiang_C2 与 qiang02 两个附件的完整定义；
2. 若有 rotation/x/y：按 Spine 语义把 deform 后的 hull 两点
   从 mesh 空间经附件变换映射到 bone104 空间：
       p_bone = R(θa) · S(v) + t
   （S=scale，θa=rotation，t=(x,y)；Spine 旋转为度、顺时针为正
   ——在 Spine 的 y 向下坐标系里 R(θ) = [[cosθ, -sinθ],[sinθ, cosθ]]
   作用于列向量等价于标准逆时针公式在 y-down 屏幕坐标下的表现，
   这里直接用与 spine-python 一致的实现验证）
3. 输出：θa、新旧轴角差、F/B 点映射前后值、交叉验证结论。
"""
import json
import math
import sys

LING_JSON = r"D:\项目\dragonraja\spikes\LingChar\pcksrc\ling\animations\characters\ling\ling.json"

# 构建期注入 config 的 muzzle_hull（条目 48 现状，源=构建日志/交接文档）
# [F_x, F_y, B_x, B_y]（bone104 局部，deform 后）
MUZZLE_HULL_INJECTED = [131.553, -37.117, -33.413, 7.907]

def main():
    with open(LING_JSON, "r", encoding="utf-8") as f:
        data = json.load(f)

    skins = data["skins"]
    att = None
    # skins 可能为列表（Spine 导出 [{"name":..,"attachments":{...}}]）或字典
    if isinstance(skins, list):
        for s in skins:
            if "qiang02" in s.get("attachments", {}):
                att = s["attachments"]["qiang02"]
                break
    else:
        att = skins[0]["attachments"]["qiang02"] if "attachments" in skins[0] else skins["qiang02"]

    if att is None:
        print("[FAIL] 未在 skins 中找到 qiang02 槽位附件")
        sys.exit(1)

    print("=== qiang02 槽位下的附件清单 ===")
    for name, a in att.items():
        print(f"--- {name} ---")
        for k, v in a.items():
            if k in ("uvs",):
                print(f"  {k}: <{len(v)} 项> 前6={v[:6]}")
            elif k in ("vertices",):
                print(f"  {k}: <{len(v)} 项> 全部={v}")
            else:
                print(f"  {k}: {v}")

    # 重点：zero_qiang_C2 与 qiang02
    for name in ("zero_qiang_C2", "qiang02"):
        a = att.get(name)
        if a is None:
            print(f"\n[WARN] 附件 {name} 不存在")
            continue
        rot = a.get("rotation", 0)
        x, y = a.get("x", 0), a.get("y", 0)
        sx, sy = a.get("scaleX", 1), a.get("scaleY", 1)
        print(f"\n=== {name} 附件变换参数 ===")
        print(f"  type={a.get('type','mesh')} rotation={rot} x={x} y={y} scaleX={sx} scaleY={sy}")

    # ---- 对 zero_qiang_C2 做映射演算 ----
    c2 = att.get("zero_qiang_C2")
    if c2 is None:
        print("[FAIL] zero_qiang_C2 不存在，假设证伪路径需人工介入")
        sys.exit(1)

    rot = math.radians(c2.get("rotation", 0))
    tx, ty = c2.get("x", 0), c2.get("y", 0)
    sx, sy = c2.get("scaleX", 1), c2.get("scaleY", 1)

    # 构建期注入的 hull 两点（deform 后，当前被当作 bone104 局部坐标）
    f_in = (MUZZLE_HULL_INJECTED[0], MUZZLE_HULL_INJECTED[1])
    b_in = (MUZZLE_HULL_INJECTED[2], MUZZLE_HULL_INJECTED[3])

    def angle_deg(v):
        # 屏幕 y 向下坐标系下的方向角（与运行时 dir0deg 同义）
        return math.degrees(math.atan2(v[1], v[0]))

    # 未修正轴角（= 当前注入值的方向）
    dir_old = (f_in[0] - b_in[0], f_in[1] - b_in[1])
    ang_old = angle_deg(dir_old)

    # Spine 附件变换：先 scale，再 rotation，再平移（spine-python SkeletonJson 同序）
    def map_point(v):
        px = v[0] * sx
        py = v[1] * sy
        # Spine 旋转矩阵（y-down 屏幕坐标系，rotation 度为顺时针）
        # spine-python: cos/sin 均用 math.cos/rad，x' = px*cos - py*sin, y' = px*sin + py*cos
        cx, cy = math.cos(rot), math.sin(rot)
        rx = px * cx - py * cy
        ry = px * cy + py * cx
        return (rx + tx, ry + ty)

    f_out = map_point(f_in)
    b_out = map_point(b_in)
    dir_new = (f_out[0] - b_out[0], f_out[1] - b_out[1])
    ang_new = angle_deg(dir_new)

    print("\n=== hull 两点映射演算（mesh 空间 -> bone104 空间）===")
    print(f"  F 映射前={f_in}  映射后={f_out}")
    print(f"  B 映射前={b_in}  映射后={b_out}")
    print(f"  未修正轴角={ang_old:.1f}°  修正后轴角={ang_new:.1f}°  偏差={ang_new-ang_old:.1f}°")

    # 交叉验证（三角度闭环）：
    # 运行时日志 dir0deg（y-down 屏幕系，角色 facing=1，无整体旋转）≈ bone104 空间轴角
    # 观测 dir0deg = -21.2 ~ -34.4（即向右上仰 21~34°）
    # 截图枪管轴角（目测）：近水平/微仰 ≈ 0 ~ -10°
    print("\n=== 三角度闭环 ===")
    print(f"  [运行时真值] dir0deg = -21.2 ~ -34.4 (godot.log dirsrc=hull_2pt)")
    print(f"  [当前注入=未修正] 轴角 = {ang_old:.1f}°  → 应≈运行时观测 ✓/✗")
    print(f"  [修正后] 轴角 = {ang_new:.1f}°  → 应≈截图枪管角(目测近水平 ~0~-10°)")
    ok_old = abs(((ang_old + 28.0 + 180) % 360) - 180) < 8   # 与观测中值 -28 对比
    ok_new = abs(((ang_new + 5.0 + 180) % 360) - 180) < 8    # 与枪管目测 -5 对比
    print(f"  闭环判定: 未修正≈运行时({ok_old})  修正后≈枪管({ok_new})")

if __name__ == "__main__":
    main()
