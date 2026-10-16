#!/usr/bin/env python3
"""条目 59 包内核验：P1 自研攻击特效（包络人因化+纹理重生成+火花流光短线，
零 C# diff；派生自 verify_pck_56.py，56 全部几何链/羽化/ramp 断言继承）：
  1) config 新值断言（设计标定，自研攻击特效设计稿 2026-10-15 P1 子集）：
     streak fadeIn 0.12/life 0.32/size [34,110]/halo 2.2×0.45；
     flash fadeIn 0.10/fadeOut 0.22/foStart 0.45/lr 0.20/amount 2/
     color_from α 0.75/color_to 青蓝尾/size [150,54]/max_size_px.flash 160；
     sparks 单帧流光短线（strip fx_spark_dash.png/h=v=1/自旋归零）；
     color_a 青/color_b 深蓝（fade_to 不动）；
  2) 包络自洽断言：flash vis=0.10×0.45+0.22=0.265 ≤ life×(1−lr)=0.288
     → 压缩判据全 k 恒不触发（条目 55 教训机读化）；
  3) 三张新纹理三件套+内容断言：fx_streak_mask2 96×320（峰值 α∈[0.88,0.91]+
     三段色点 白核/青/深蓝）、fx_flash2 224×80（峰值 α+ramp col0..13 递增）、
     fx_spark_dash 64×16（峰值 α+轴向色 青→深蓝 递减）；sha 区分度断言
     （三张新纹理 sha 两两及与旧三张均不同）；
  4) 零 C# diff 断言：dll sha256 == 条目 57 定案值 208c195d…（HitBurstFX
     基线，本轮无 C# 改动 → MVID 确定性重建逐字节不变）；
  5) 其余断言（dir_rot_fix=+7.81/muzzle_hull v2 烘焙/silent 零影响/spark
     羽化/旧 flash ramp/sha 对账）与 56 一致。

复用 GDPC v3 解析（dir 偏移@0x20，file_base 0x70，config 段可能 zlib 压缩）。
用法: "C:/Program Files/Python310/python.exe" tools/verify_pck_59.py
"""
import hashlib
import json
import math
import struct
import sys
import zlib
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent

PCK = ROOT / 'LingChar.pck'
LING_JSON = ROOT / 'pcksrc/ling/animations/characters/ling/ling.json'
ATLAS = ROOT / 'pcksrc/ling/animations/characters/ling/ling.atlas'
PAGE_DIR = ROOT / 'pcksrc/ling/animations/characters/ling'
MODS = Path(r'D:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\Mods\LingChar')
CFG_REL = 'drc_dye/profiles/ling/animations/characters/ling/config.json'
SILENT_CFG_REL = 'drc_dye/profiles/ling/animations/characters/silent/config.json'
FX_DIR = 'drc_dye/profiles/ling/animations/characters/ling'
FX_NAMES = ('fx_sparks3x3_grid.png', 'fx_streak_mask.png', 'fx_flash.png',
            'fx_streak_mask2.png', 'fx_flash2.png', 'fx_spark_dash.png')
SPARK_CTEX = 'fx_sparks3x3_grid.png-fd48a8638dfa798c1455fc7edf038349.ctex'
PNG_SIZE = {
    'fx_streak_mask.png': (88, 260),
    'fx_flash.png': (192, 16),
    'fx_sparks3x3_grid.png': (512, 512),
    'fx_streak_mask2.png': (96, 320),
    'fx_flash2.png': (224, 80),
    'fx_spark_dash.png': (64, 16),
}
# 零 C# diff 锚：条目 57 定案 dll（HitBurstFX 基线，条目 58 实证 MVID 确定性）
DLL_ZERO_DIFF_SHA = '208c195db634bdf594083eda6a44cb745e93c32c6016947b9f78fa1d6cf66cb4'


def list_pck(path):
    d = Path(path).read_bytes()
    assert d[:4] == b'GDPC'
    dir_off = struct.unpack_from('<q', d, 0x20)[0]
    file_base = 0x70
    off = dir_off
    count = struct.unpack_from('<I', d, off)[0]
    off += 4
    files = {}
    for _ in range(count):
        plen = struct.unpack_from('<I', d, off)[0]
        off += 4
        name = d[off:off + plen].rstrip(b'\0').decode('utf-8')
        off += plen
        fo, size = struct.unpack_from('<qq', d, off)
        off += 16 + 16
        off += 4
        files[name] = (file_base + fo, size)
    return files, d


fails = []


def check(name, cond, detail=''):
    print(('PASS  ' if cond else 'FAIL  ') + name + (f'  {detail}' if detail else ''))
    if not cond:
        fails.append(name)


def approx(a, b, eps=1e-6):
    return a is not None and isinstance(a, (int, float)) and abs(a - b) <= eps


files, d = list_pck(str(PCK))
check('pck 解析', True, f'{len(files)} 条')


def read_cfg(rel):
    off, size = files[rel]
    blob = d[off:off + size]
    if blob[:1] not in (b'{', b'['):
        blob = zlib.decompress(blob, 15, 100000000)
    return json.loads(blob)


# ============ 1) ling profile config：几何链真值沿用 + P1 设计标定值 ============
check('ling config 在包内', CFG_REL in files)
fx = read_cfg(CFG_REL).get('attack_fx', {})
sp, st, fl, mp = fx.get('sparks', {}), fx.get('streak', {}), fx.get('flash', {}), fx.get('max_size_px', {})

check('attack_fx 顶层 enabled=true', fx.get('enabled') is True, repr(fx.get('enabled')))
check('sparks.enabled=true', sp.get('enabled') is True, repr(sp.get('enabled')))
check('streak.enabled=true', st.get('enabled') is True, repr(st.get('enabled')))
check('flash.enabled=true', fl.get('enabled') is True, repr(fl.get('enabled')))
check('track_muzzle=true（条目 47 跟随枪口，48/49 语义=位置+旋转全跟随）',
      fx.get('track_muzzle') is True, repr(fx.get('track_muzzle')))
check('bursts=[0.166667]（单拍化）', fx.get('bursts') == [0.166667], repr(fx.get('bursts')))
check('dir_source=auto', fx.get('dir_source') == 'auto', repr(fx.get('dir_source')))
check('barrel_deg=-10.18', approx(fx.get('barrel_deg'), -10.18), repr(fx.get('barrel_deg')))
# ---- 条目 53：dir_rot_fix 终值 +7.81（几何锚定链，P1 零改动）----
DIR_FIX_EXPECT = 23.08 - 15.27   # = 7.81
check('dir_rot_fix=+7.81（条目 53 定案终值=γ_meas−恒等式）',
      approx(fx.get('dir_rot_fix'), DIR_FIX_EXPECT, 1e-9), repr(fx.get('dir_rot_fix')))
check('dir_rot_fix 与 δ 推导自洽（7.81≈23.08−15.27，hook 链对账）',
      abs(float(fx.get('dir_rot_fix', 0.0)) - DIR_FIX_EXPECT) < 0.005,
      f"dir_rot_fix={fx.get('dir_rot_fix')} 期望≈{DIR_FIX_EXPECT:.2f}")
_n53 = str(fx.get('_note_dir_rot_fix', ''))
check('_note_dir_rot_fix 条目 53 定案注记在包（分支判定+hook 链依据）',
      '条目53' in _n53 and 'sprrot=0' in _n53 and '非截图反解' in _n53,
      repr(_n53[:60]))
check('_note_p1 设计标定声明在包（条目 59）',
      '条目59' in str(fx.get('_note_p1', '')) and '设计标定' in str(fx.get('_note_p1', '')),
      repr(str(fx.get('_note_p1', ''))[:50]))
check('streak.mode=sprite', st.get('mode') == 'sprite', repr(st.get('mode')))
check('flash.mode=sprite', fl.get('mode') == 'sprite', repr(fl.get('mode')))
check('max_size_px.spark=40（不动）', mp.get('spark') == 40.0, repr(mp.get('spark')))
check('max_size_px.streak=256（不动）', mp.get('streak') == 256.0, repr(mp.get('streak')))
check('max_size_px.flash=160（P1：128→160 配火舌 150px）', mp.get('flash') == 160.0,
      repr(mp.get('flash')))
# ---- streak P1 ----
check('streak.life=0.32（P1：0.2→0.32，包络=升0.12+保持0.192+落0.128）',
      approx(st.get('life'), 0.32), repr(st.get('life')))
check('streak.size=[34,110]（P1）', st.get('size') == [34.0, 110.0], repr(st.get('size')))
check('streak.scale_curve=[0.7,1.0]（不动）', st.get('scale_curve') == [0.7, 1.0],
      repr(st.get('scale_curve')))
check('streak.halo_size_mul=2.2（P1：2.0→2.2）', approx(st.get('halo_size_mul'), 2.2),
      repr(st.get('halo_size_mul')))
check('streak.halo_alpha=0.45（P1：0.6→0.45 防饱和关键）', approx(st.get('halo_alpha'), 0.45),
      repr(st.get('halo_alpha')))
check('streak.halo=true（不动）', st.get('halo') is True, repr(st.get('halo')))
check('streak.streak_fade_in_s=0.12（P1：0.05→0.12 人因阈值慢起）',
      approx(st.get('streak_fade_in_s'), 0.12), repr(st.get('streak_fade_in_s')))
check('streak._note_fade_in P1 注记在包（总长 0.44 明示局限）',
      'P1' in str(st.get('_note_fade_in', '')) and '设计标定' in str(st.get('_note_fade_in', ''))
      and '0.44' in str(st.get('_note_fade_in', '')),
      repr(str(st.get('_note_fade_in', ''))[:50]))
check('streak.color=[1,1,1]（modulate 保持白，条目 45★）', st.get('color') == [1.0, 1.0, 1.0],
      repr(st.get('color')))
check('streak.amount=1（不动）', st.get('amount') == 1, repr(st.get('amount')))
check('streak.texture=fx_streak_mask2.png（P1）',
      st.get('texture', '').endswith('/fx_streak_mask2.png'), repr(st.get('texture')))
_st_total = float(st.get('streak_fade_in_s', 0)) + float(st.get('life', 0))
check('streak 包络总长=fadeIn+life=0.44s（P1 止血口径，超 0.40 上限已明示）',
      abs(_st_total - 0.44) < 1e-9, f'{_st_total:.3f}s')
# ---- flash P1 ----
check('flash.size=[150,54]（P1：细条→火舌体量）', fl.get('size') == [150.0, 54.0],
      repr(fl.get('size')))
check('flash.color_from=[1,1,1,0.75]（P1：α 0.85→0.75 降饱和）',
      fl.get('color_from') == [1.0, 1.0, 1.0, 0.75], repr(fl.get('color_from')))
check('flash.color_from rgb=白（条目 45★）', (fl.get('color_from') or [])[:3] == [1.0, 1.0, 1.0],
      repr(fl.get('color_from')))
check('flash.color_to=[0.55,0.78,1,0]（P1：尾色偏青蓝）',
      fl.get('color_to') == [0.55, 0.78, 1.0, 0.0], repr(fl.get('color_to')))
check('flash.flash_fade_in_s=0.10（P1：0.09→0.10）',
      approx(fl.get('flash_fade_in_s'), 0.10), repr(fl.get('flash_fade_in_s')))
check('flash.flash_fade_out_s=0.22（P1：0.18→0.22，rise:fall=1:2.2）',
      approx(fl.get('flash_fade_out_s'), 0.22), repr(fl.get('flash_fade_out_s')))
check('flash.flash_fadeout_start=0.45（P1：0.5→0.45 fall 占比提高）',
      approx(fl.get('flash_fadeout_start'), 0.45), repr(fl.get('flash_fadeout_start')))
check('flash.lifetime_randomness=0.20（P1：0.33→0.20）',
      approx(fl.get('lifetime_randomness'), 0.20), repr(fl.get('lifetime_randomness')))
check('flash.amount=2（P1：3→2 多片叠亮是 LDR 饱和推手）', fl.get('amount') == 2,
      repr(fl.get('amount')))
check('flash.life=0.36（不动）', approx(fl.get('life'), 0.36), repr(fl.get('life')))
check('flash.texture=fx_flash2.png（P1）', fl.get('texture', '').endswith('/fx_flash2.png'),
      repr(fl.get('texture')))
check('不动 flash.offset_x=5.8', fl.get('offset_x') == 5.8, repr(fl.get('offset_x')))
check('不动 flash.grow_from_root/growth_peak',
      fl.get('flash_grow_from_root') is True and approx(fl.get('flash_growth_peak'), 0.25),
      f"{fl.get('flash_grow_from_root')}/{fl.get('flash_growth_peak')}")
# ---- flash 包络自洽（条目 55 教训机读化）----
_fi, _fo, _fs = float(fl.get('flash_fade_in_s', 0)), float(fl.get('flash_fade_out_s', 0)), \
    float(fl.get('flash_fadeout_start', 0))
_vis = _fi * _fs + _fo
check('flash 包络总长自洽 vis=0.10×0.45+0.22=0.265s',
      abs(_vis - 0.265) < 1e-9, f'{_vis:.3f}s')
check('flash 压缩判据恒不触发（life×(1−lr)=0.36×0.8=0.288 > vis 0.265；'
      '且包络与 l 同随 k 线性 → 全 k 真实可达）',
      fl.get('life', 0) * (1 - fl.get('lifetime_randomness', 0)) > _vis + 1e-9,
      f"l_min={fl.get('life', 0) * (1 - fl.get('lifetime_randomness', 0)):.4f} vs vis={_vis:.4f}")
check('flash rise:fall=1:2.2（设计稿快亮慢灭）',
      abs(_fi / _fo - 1 / 2.2) < 1e-9, f'{_fi:.3f}:{_fo:.3f}')
# ---- sparks P1 ----
check('sparks.strip_texture=fx_spark_dash.png（P1 流光短线单帧）',
      sp.get('strip_texture', '').endswith('/fx_spark_dash.png'), repr(sp.get('strip_texture')))
check('sparks.h/v_frames=1/1（P1：SubUV 1×1 退化=整图单帧，零 C#）',
      sp.get('h_frames') == 1.0 and sp.get('v_frames') == 1.0,
      f"{sp.get('h_frames')}/{sp.get('v_frames')}")
check('sparks.size=[7,13]（P1：线长 px，整图宽定标）', sp.get('size') == [7.0, 13.0],
      repr(sp.get('size')))
check('sparks.speed=[280,540]（P1：弹射感）', sp.get('speed') == [280.0, 540.0],
      repr(sp.get('speed')))
check('sparks.spread=16（P1：14→16 方向性）', sp.get('spread') == 16.0, repr(sp.get('spread')))
check('sparks.drag=6（P1：7→6）', sp.get('drag') == 6.0, repr(sp.get('drag')))
check('sparks.life=0.26（P1：0.3→0.26）', sp.get('life') == 0.26, repr(sp.get('life')))
check('sparks.angle_random=0（P1：短线纹理必须与速度共线）', sp.get('angle_random') == 0.0,
      repr(sp.get('angle_random')))
check('sparks.angular_velocity=[0,0]（P1：自旋归零，短线不得翻滚）',
      sp.get('angular_velocity') == [0.0, 0.0], repr(sp.get('angular_velocity')))
check('sparks.amount=6（不动）', sp.get('amount') == 6, repr(sp.get('amount')))
check('不动 sparks.emission_sphere_radius=7.25', sp.get('emission_sphere_radius') == 7.25,
      repr(sp.get('emission_sphere_radius')))
check('sparks.anim_speed/offset 保留（1×1 下无视觉效果，键不删）',
      sp.get('anim_speed') == 1.0 and sp.get('anim_offset_min') == 0.0
      and sp.get('anim_offset_max') == 1.0,
      f"{sp.get('anim_speed')}/{sp.get('anim_offset_min')}/{sp.get('anim_offset_max')}")
# ---- 全局色板 P1 ----
check('color_a=[0.37,0.85,1.0]（P1：mid 青）', fx.get('color_a') == [0.37, 0.85, 1.0],
      repr(fx.get('color_a')))
check('color_b=[0.16,0.34,0.72]（P1：tail 深蓝，替代蓝紫两极）',
      fx.get('color_b') == [0.16, 0.34, 0.72], repr(fx.get('color_b')))
check('fade_to 不动=[0.37,0.85,1.0]（mid 层）', fx.get('fade_to') == [0.37, 0.85, 1.0],
      repr(fx.get('fade_to')))

# ============ 2) muzzle 数据链真值（条目 52：膛线烘焙 v2 交叉验证，P1 零改动）============
ling = json.loads(LING_JSON.read_text(encoding='utf-8'))
slot = next(s for s in ling['slots'] if s['name'] == 'qiang02')
att_track = ling['animations']['attack']['slots']['qiang02']['attachment']
att_name = next(f['name'] for f in att_track if f.get('name'))
att = ling['skins'][0]['attachments']['qiang02'][att_name]
verts = list(att['vertices'])
deform_entry = ling['animations']['attack'].get('attachments', {}).get(
    'default', {}).get('qiang02', {}).get(att_name)
if deform_entry and deform_entry.get('deform'):
    dv0 = deform_entry['deform'][0]
    off_idx = int(dv0.get('offset', 0)) * 2
    for i, val in enumerate(dv0['vertices']):
        if off_idx + i < len(verts):
            verts[off_idx + i] += val
pts = [(verts[i], verts[i + 1]) for i in range(0, len(verts), 2)]
f_raw = ((pts[0][0] + pts[3][0]) / 2, (pts[0][1] + pts[3][1]) / 2)
rk_raw = ((pts[1][0] + pts[2][0]) / 2, (pts[1][1] + pts[2][1]) / 2)


def atlas_region(atlas_path, name):
    lines = atlas_path.read_text(encoding='utf-8').splitlines()
    for i, ln in enumerate(lines):
        if ln.strip() == name:
            bounds = None
            for j in range(i + 1, min(i + 6, len(lines))):
                s = lines[j].strip()
                if s.startswith('bounds:'):
                    bounds = s.split(':', 1)[1]
                elif s.startswith('rotate:'):
                    raise AssertionError('rotate 区域需换算轴序')
                elif s and ':' not in s:
                    break
            if bounds:
                return tuple(int(v) for v in bounds.split(','))
    raise AssertionError(f'atlas 未找到 {name}')


rx, ry, rw, rh = atlas_region(ATLAS, att.get('path', att_name))
from PIL import Image
import numpy as np
page = Image.open(PAGE_DIR / ATLAS.read_text(encoding='utf-8').splitlines()[0].strip()).convert('RGBA')
alpha = np.asarray(page, dtype=np.int32)[..., 3]
cols = []
for cx in range(rx + 2, rx + rw - 4):
    col = alpha[ry:ry + rh, cx]
    nz = np.nonzero(col > 40)[0]
    if len(nz):
        cols.append((float(cx - rx), float(nz[0]), float(nz[-1])))
best_n, sup = 0, []
for _, t0, b0 in cols:
    s = [(cx, (t + b) / 2.0) for cx, t, b in cols if t == t0 and b == b0]
    if len(s) > best_n:
        best_n, sup = len(s), s
check('滑套带支持列充足（独立重算）', best_n >= 12, f'{best_n}/{len(cols)} 列')
sup.sort(key=lambda p: p[0])
xs = np.array([p[0] for p in sup])
ys = np.array([p[1] for p in sup])
k, c = np.polyfit(xs, ys, 1)
theta_mid = math.degrees(math.atan(k))
ux, uy = pts[0][0] - pts[1][0], pts[0][1] - pts[1][1]
wx, wy = pts[2][0] - pts[1][0], pts[2][1] - pts[1][1]
bx = math.cos(math.radians(theta_mid)) * ux / (rw - 1) + math.sin(math.radians(theta_mid)) * wx / (rh - 1)
by = math.cos(math.radians(theta_mid)) * uy / (rw - 1) + math.sin(math.radians(theta_mid)) * wy / (rh - 1)
bn = math.hypot(bx, by)
bx, by = bx / bn, by / bn
seg = math.hypot(f_raw[0] - rk_raw[0], f_raw[1] - rk_raw[1])
exp_hull = [round(f_raw[0], 3), round(f_raw[1], 3),
            round(f_raw[0] - seg * bx, 3), round(f_raw[1] - seg * by, 3)]

check(f"muzzle_bone=slots 绑定骨 {slot['bone']!r}",
      fx.get('muzzle_bone') == slot['bone'], repr(fx.get('muzzle_bone')))
check(f'attack 实际附件={att_name!r}', att_name == 'zero_qiang_C2', repr(att_name))
check(f'muzzle_hull={exp_hull}（v2 烘焙，独立重算一致）',
      fx.get('muzzle_hull') == exp_hull, repr(fx.get('muzzle_hull')))
check('muzzle_hull 4 元素',
      isinstance(fx.get('muzzle_hull'), list) and len(fx.get('muzzle_hull', [])) == 4,
      repr(fx.get('muzzle_hull')))
h = fx.get('muzzle_hull') or exp_hull
check('F（位置锚）与烘焙前重算值一致',
      approx(h[0], round(f_raw[0], 3), 1e-6) and approx(h[1], round(f_raw[1], 3), 1e-6),
      f"{h[0]},{h[1]} vs {round(f_raw[0], 3)},{round(f_raw[1], 3)}")
len_new = math.hypot(h[0] - h[2], h[1] - h[3])
len_raw = math.hypot(f_raw[0] - rk_raw[0], f_raw[1] - rk_raw[1])
check('|F−Rk| 轴长不变（只转方向）', abs(len_new - len_raw) < 0.01, f'{len_new:.3f} vs {len_raw:.3f}')
ang_hull = math.degrees(math.atan2(h[1] - h[3], h[0] - h[2]))
print(f'      theta_mid={theta_mid:.2f}°  滑套带支持={best_n}列  bore_dir=({bx:.4f},{by:.4f})  hull JSON 角={ang_hull:.2f}°')
check('θ_mid ≈ 0°（条目 51 勘误）', abs(theta_mid) < 1.0, f'{theta_mid:.2f}°')
check('hull JSON 系角 ≈ -15.27°（条目 51 预期）',
      abs(ang_hull - (-15.27)) < 0.3, f'{ang_hull:.2f}°')
mdir = fx.get('muzzle_dir') or [0, 0]
ang_mdir = math.degrees(math.atan2(mdir[1], mdir[0]))
check('hull 长轴与 muzzle_dir 固有差 ≈ -5.08°（条目 48 取证）',
      abs((ang_hull - ang_mdir) - (-5.08)) < 0.3, f'{ang_hull - ang_mdir:.2f}°')
check('muzzle_offset=[137.7,-37.0] 保留（fallback）',
      fx.get('muzzle_offset') == [137.7, -37.0], repr(fx.get('muzzle_offset')))
check('muzzle_dir=[0.984,-0.177] 保留（fallback）',
      fx.get('muzzle_dir') == [0.984, -0.177], repr(fx.get('muzzle_dir')))
check('muzzle_len=50.0 保留（fallback）', fx.get('muzzle_len') == 50.0, repr(fx.get('muzzle_len')))
check('_note_muzzle_dir 废弃注记在包', 'muzzle_hull' in str(fx.get('_note_muzzle_dir', '')),
      repr(fx.get('_note_muzzle_dir', '')[:40]))
check('_note_muzzle_hull 含条目 52 v2 烘焙说明（双缘中线+conv-B）',
      '双缘中线' in str(fx.get('_note_muzzle_hull', '')) and 'conv-B' in str(fx.get('_note_muzzle_hull', '')),
      repr(str(fx.get('_note_muzzle_hull', ''))[:60]))

# ============ 3) 包内纹理源 PNG 尺寸核验（6 张）============
for name, (w, h_) in PNG_SIZE.items():
    rel = f'{FX_DIR}/{name}'
    if rel not in files:
        check(f'{name} 源在包内', False)
        continue
    off, size = files[rel]
    blob = d[off:off + size]
    ok = blob[:8] == b'\x89PNG\r\n\x1a\n'
    pw = ph = 0
    if ok:
        pw, ph = struct.unpack_from('>II', blob, 16)
    check(f'{name} 尺寸={w}x{h_}', ok and (pw, ph) == (w, h_), f'实测 {pw}x{ph}')

# ============ 3b) spark 羽化断言（继承 56，旧 3×3 仍在包）============
import io
import hashlib as _hlib
_sp_rel = f'{FX_DIR}/fx_sparks3x3_grid.png'
_sp_off, _sp_size = files[_sp_rel]
_sp_blob = d[_sp_off:_sp_off + _sp_size]
_sp_sha = _hlib.sha256(_sp_blob).hexdigest()
SPARK_OLD_SHA = '6b99f4fb427277859f02fe5046c2826b89dcf565dbc6562c41e0554f31f99189'
check('sparks 旧网格包内 sha256 ≠ 旧版 6b99f4fb…（羽化区分度，继承 56）',
      _sp_sha != SPARK_OLD_SHA, _sp_sha[:16] + '…')
_sp_im = Image.open(io.BytesIO(_sp_blob)).convert('RGBA')
_sp_a = np.asarray(_sp_im)[..., 3]
_fw, _fh = _sp_im.width / 3.0, _sp_im.height / 3.0
_bmax = 0
for _i in range(4):
    _x = min(int(round(_i * _fw)), _sp_im.width - 1)
    _bmax = max(_bmax, int(_sp_a[:, _x].max()))
for _j in range(4):
    _y = min(int(round(_j * _fh)), _sp_im.height - 1)
    _bmax = max(_bmax, int(_sp_a[_y, :].max()))
check('sparks 3×3 全部帧界 alpha=0（布局不变，继承 56）', _bmax == 0,
      f'帧界 alpha 最大值={_bmax}')
_sp_partial = int(((_sp_a >= 1) & (_sp_a <= 254)).sum())
check('sparks 内容边缘 alpha 梯度存在（继承 56）', _sp_partial > 8000, f'{_sp_partial} > 8000')

# ============ 3c) 旧 flash ramp（继承 56）============
_fl_rel = f'{FX_DIR}/fx_flash.png'
_fl_off, _fl_size = files[_fl_rel]
_fl_blob = d[_fl_off:_fl_off + _fl_size]
FLASH_OLD_SHA = '79e40d5837810135cd4819258dd8c01813b4b9995ec31f045c96c47604ead8db'
check('旧 flash 包内 sha256 ≠ 79e40d58…（ramp 区分度，继承 56）',
      _hlib.sha256(_fl_blob).hexdigest() != FLASH_OLD_SHA)
_fl_im = Image.open(io.BytesIO(_fl_blob)).convert('RGBA')
_fl_a = np.asarray(_fl_im)[..., 3]
_midy = _fl_im.height // 2
_rear = [int(_fl_a[_midy, x]) for x in range(0, 17)]
check('旧 flash 后缘 ramp col0..16 严格递增（继承 56）',
      all(_rear[i] < _rear[i + 1] for i in range(len(_rear) - 1)) and _rear[16] > _rear[1] + 30,
      f'{_rear}')

# ============ 3d) P1 三张新纹理内容断言 ============
def _load(rel):
    off, size = files[rel]
    return Image.open(io.BytesIO(d[off:off + size])).convert('RGBA')

def _borders(a):
    return max(float(a[0].max()), float(a[-1].max()),
               float(a[:, 0].max()), float(a[:, -1].max()))

# streak v2：峰值 α∈[0.88,0.91] + 三段色点 白核/青/深蓝 + 四边 0
_st2 = _load(f'{FX_DIR}/fx_streak_mask2.png')
_st2_arr = np.asarray(_st2, dtype=np.float32) / 255.0
_st2_a = _st2_arr[..., 3]
check('streak v2 四边 alpha=0', _borders(_st2_a) == 0.0)
_st2_amax = float(_st2_a.max())
check('streak v2 峰值 alpha ∈[0.88,0.91]（P1 防饱和硬约束）',
      0.88 <= _st2_amax <= 0.91, f'{_st2_amax:.4f}')
_cx = _st2.width // 2
_r05, _r35, _r70 = (float(_st2_arr[y, _cx, 0]) for y in (16, 112, 224))
check('streak v2 v≈0.05 白核（R≥0.82）', _r05 >= 0.82, f'R={_r05:.3f}')
check('streak v2 v≈0.35 青（R∈[0.33,0.44]）', 0.33 <= _r35 <= 0.44, f'R={_r35:.3f}')
check('streak v2 v≈0.70 深蓝（R≤0.35 且 G<0.70）',
      _r70 <= 0.35 and float(_st2_arr[224, _cx, 1]) < 0.70,
      f"R={_r70:.3f} G={float(_st2_arr[224, _cx, 1]):.3f}")
check('streak v2 三段色递减（白核 R > 青 R > 深蓝 R）', _r05 > _r35 > _r70,
      f'{_r05:.3f}>{_r35:.3f}>{_r70:.3f}')

# flash2：峰值 α∈[0.88,0.91] + 后缘 ramp col0..13 递增 + 四边 0
_f2 = _load(f'{FX_DIR}/fx_flash2.png')
_f2_arr = np.asarray(_f2, dtype=np.float32) / 255.0
_f2_a = _f2_arr[..., 3]
check('flash2 四边 alpha=0', _borders(_f2_a) == 0.0)
_f2_amax = float(_f2_a.max())
check('flash2 峰值 alpha ∈[0.88,0.91]（P1 防饱和硬约束）',
      0.88 <= _f2_amax <= 0.91, f'{_f2_amax:.4f}')
_f2_midy = _f2.height // 2
_f2_rear = [float(_f2_a[_f2_midy, x]) for x in range(0, 15)]
check('flash2 后缘 ramp col0..13 严格递增（13.4px 渐入带，R1 纪律）',
      all(_f2_rear[i] < _f2_rear[i + 1] for i in range(13))
      and _f2_rear[13] > _f2_rear[1] + 0.10,
      f"{[round(v, 3) for v in _f2_rear]}")

# spark dash：峰值 α∈[0.88,0.91] + 四边 0 + 轴向色 青→深蓝 递减
_d1 = _load(f'{FX_DIR}/fx_spark_dash.png')
_d1_arr = np.asarray(_d1, dtype=np.float32) / 255.0
_d1_a = _d1_arr[..., 3]
check('spark dash 四边 alpha=0', _borders(_d1_a) == 0.0)
_d1_amax = float(_d1_a.max())
check('spark dash 峰值 alpha ∈[0.88,0.91]', 0.88 <= _d1_amax <= 0.91, f'{_d1_amax:.4f}')
_d1_midy = _d1.height // 2
_dl, _dr = float(_d1_arr[_d1_midy, 2, 0]), float(_d1_arr[_d1_midy, _d1.width - 3, 0])
check('spark dash 轴向色 青→深蓝 递减（左 R≥0.30、右 R≤0.25）',
      _dl > _dr and _dl >= 0.30 and _dr <= 0.25, f'{_dl:.3f}→{_dr:.3f}')

# sha 区分度：三张新纹理 sha 与旧三张及彼此均不同
_shas = {}
for _n in FX_NAMES:
    _o, _s = files[f'{FX_DIR}/{_n}']
    _shas[_n] = _hlib.sha256(d[_o:_o + _s]).hexdigest()
check('新纹理 sha 区分度（三张新 sha 两两及与旧三张均不同）',
      len(set(_shas.values())) == len(FX_NAMES),
      f"streak2={_shas['fx_streak_mask2.png'][:8]} flash2={_shas['fx_flash2.png'][:8]} "
      f"dash={_shas['fx_spark_dash.png'][:8]}")

# ============ 4) silent profile attack_fx 零影响（继承）============
check('silent config 在包内', SILENT_CFG_REL in files)
sfx = read_cfg(SILENT_CFG_REL).get('attack_fx', {})
banned = ('mode', 'dir_source', 'muzzle_offset', 'muzzle_dir', 'enabled', 'track_muzzle',
          'muzzle_hull', 'muzzle_bone', 'dir_rot_fix')
leaked = []
for k2 in banned:
    if k2 in sfx:
        leaked.append(k2)
    for seg in ('sparks', 'streak', 'flash'):
        if isinstance(sfx.get(seg), dict) and k2 in sfx[seg]:
            leaked.append(f'{seg}.{k2}')
leaked = [x for x in leaked if x != 'enabled']
check('silent attack_fx 无 mode/dir_source/muzzle_*/track_muzzle/dir_rot_fix/段级 enabled',
      not leaked, f'命中={leaked}')
check('silent attack_fx 维持 legacy 键',
      ('textures' in sfx or 'streak_texture' in sfx or not sfx),
      f"keys={sorted(sfx.keys())[:8]}")

# ============ 5) 六张 fx 纹理：源 + .import + ctex ============
for name in FX_NAMES:
    src_ok = f'{FX_DIR}/{name}' in files
    imp_ok = f'{FX_DIR}/{name}.import' in files
    check(f'{name} 源+.import 在包内', src_ok and imp_ok, f'src={src_ok} imp={imp_ok}')
    if imp_ok:
        off, size = files[f'{FX_DIR}/{name}.import']
        imp_text = d[off:off + size].decode('utf-8', 'replace')
        import re
        m = re.search(r'path="res://\.godot/imported/([^"]+)"', imp_text)
        prod = m.group(1) if m else ''
        check(f'{name} ctex 产品在包内', bool(prod) and f'.godot/imported/{prod}' in files, prod)

check('火花 ctex 产品名不变（防污染锚点）', f'.godot/imported/{SPARK_CTEX}' in files, SPARK_CTEX)

# ============ 6) dll 字符串核验（继承 56：C# 零改动 → 全部锚点原样在）============
for dllp in (ROOT / 'bin/Release/net9.0/LingChar.dll', MODS / 'LingChar.dll'):
    b = dllp.read_bytes()
    for s in ('dirdeg=', 'dirsrc=', 'local_to_world', 'muzzle_gpos2', 'breaker tripped',
              'track=on', 'track=dropped', 'track_muzzle',
              'muzzle_hull', 'muzzle_bone', 'hull_2pt', 'track=downgraded',
              '回退 bone=', 'dir0deg=',
              'find_bone', 'get_skeleton',
              'tick1 kind=', 'n2rot=', 'sprrot=', 'n2gs=', 'fix=',
              'follower expired', 'follower recycled', 'dir_rot_fix',
              'flash envelope fadeIn=', 'spark damp=', 'shift~',
              'streak_fade_in_s'):
        check(f'{dllp.parent.parent.name} 含 {s!r}',
              s.encode('utf-16-le') in b or s.encode('utf-8') in b)
    check(f'{dllp.parent.parent.name} 旧 follow tick1 字符串已退役',
          'follow tick1'.encode('utf-16-le') not in b and 'follow tick1'.encode('utf-8') not in b)

# ============ 7) 零 C# diff + sha256：仓内产物 vs 安装实物 ============
_dll = ROOT / 'bin/Release/net9.0/LingChar.dll'
_dll_sha = hashlib.sha256(_dll.read_bytes()).hexdigest()
check('零 C# diff：dll sha256 == 条目 57 定案基线 208c195d…（MVID 确定性重建）',
      _dll_sha == DLL_ZERO_DIFF_SHA, _dll_sha[:16] + '…')
for name in ('LingChar.dll', 'LingChar.pck'):
    src = (ROOT / 'bin/Release/net9.0' / name) if name.endswith('.dll') else (ROOT / name)
    inst = MODS / name
    h1 = hashlib.sha256(src.read_bytes()).hexdigest()
    h2 = hashlib.sha256(inst.read_bytes()).hexdigest()
    check(f'{name} sha 仓内==安装', h1 == h2, h1[:16] + '…')
    print(f'      {name} sha256 = {h1}')
    print(f'      {name} size = {src.stat().st_size} B')

print()
print('== 汇总:', '全部通过' if not fails else f'FAIL x{len(fails)}: {fails}')
sys.exit(1 if fails else 0)
