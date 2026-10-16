#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""条目 57 包内核验：hit_fx 新管线（fx_ling_attack_01_hit → STS2）+ 5 张
hitfx 纹理三件套 + config 新 schema + dll 字符串（派生自 verify_pck_56.py，
56 全部 149 项断言继承）：
  1) 包内 5 张 hitfx_*（flare/ray/glow2x2/ring/streak）的源 png+.import+ctex
     三件套在包；PNG IHDR 尺寸断言（512²/256²/512²/512²/16×160）；
  2) glow alpha 归一化断言（包内 glow2x2 alpha max=255 ≠ 源 139，A5）；
     ring 剖面自检（四边 alpha=0/中心≈0.25/rim 峰 r≈0.85）；streak 四边 0；
  3) 包内 ling config 的 hit_fx 新键全表断言（pipeline=hitburst/
     bursts=[0,0.43]/size_boost=1.5/track_target=false/max_size_px 七键/
     八发射器段）与 silent config hit_fx 仍是 legacy 键集（零影响断言，
     52⑤ 先例；不出现新管线键）；
  4) dll 字符串双编码（UTF-8/UTF-16）断言：hit fx armed / wave=/cleanup
     seq=/pipeline/hitburst/flash_disc/flare size=/emitters disc=/breaker
     tripped kind=/texture missing/新 config 键名等；
  5) sha 对账（dll+pck 仓内==Mods 安装件）与 56 全部断言（attack_fx 键/
     muzzle 链/纹理/silent 零影响）继承。

复用 GDPC v3 解析（dir 偏移@0x20，file_base 0x70，config 段可能 zlib 压缩）。
用法: "C:/Program Files/Python310/python.exe" tools/verify_pck_hitfx.py
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
FX_NAMES = ('fx_sparks3x3_grid.png', 'fx_streak_mask.png', 'fx_flash.png')
HIT_NAMES = ('hitfx_flare.png', 'hitfx_ray.png', 'hitfx_glow2x2.png',
             'hitfx_ring.png', 'hitfx_streak.png')
HIT_PNG_SIZE = {
    'hitfx_flare.png': (512, 512),
    'hitfx_ray.png': (256, 256),
    'hitfx_glow2x2.png': (512, 512),
    'hitfx_ring.png': (512, 512),
    'hitfx_streak.png': (16, 160),
}
SPARK_CTEX = 'fx_sparks3x3_grid.png-fd48a8638dfa798c1455fc7edf038349.ctex'
PNG_SIZE = {
    'fx_streak_mask.png': (88, 260),
    'fx_flash.png': (192, 16),
    'fx_sparks3x3_grid.png': (512, 512),
}


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


# ============ 1) ling profile config：attack_fx 段（56 全部断言继承） ============
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
# ---- 条目 53：dir_rot_fix 终值 +7.81（δ 定案注入，管线常量分支）----
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
check('_note_dir_rot_fix 记录 γ_meas 与恒等式推导值',
      '23.08' in _n53 and '15.27' in _n53, repr(_n53[:60]))
check('streak.mode=sprite', st.get('mode') == 'sprite', repr(st.get('mode')))
check('flash.mode=sprite', fl.get('mode') == 'sprite', repr(fl.get('mode')))
check('max_size_px.spark=40', mp.get('spark') == 40.0, repr(mp.get('spark')))
check('max_size_px.streak=256', mp.get('streak') == 256.0, repr(mp.get('streak')))
check('max_size_px.flash=128', mp.get('flash') == 128.0, repr(mp.get('flash')))
check('streak.life=0.2', approx(st.get('life'), 0.2), repr(st.get('life')))
check('streak.size=[32,100]', st.get('size') == [32.0, 100.0], repr(st.get('size')))
check('streak.scale_curve=[0.7,1.0]', st.get('scale_curve') == [0.7, 1.0], repr(st.get('scale_curve')))
check('streak.halo_size_mul=2.0', approx(st.get('halo_size_mul'), 2.0), repr(st.get('halo_size_mul')))
check('streak.halo_alpha=0.6', approx(st.get('halo_alpha'), 0.6), repr(st.get('halo_alpha')))
check('streak.halo=true', st.get('halo') is True, repr(st.get('halo')))
check('streak.streak_fade_in_s=0.05（条目 56 R2：渐亮上升段，保持/落0与 scale 不动）',
      approx(st.get('streak_fade_in_s'), 0.05), repr(st.get('streak_fade_in_s')))
check('streak._note_fade_in 条目 56 注记在包',
      '条目56' in str(st.get('_note_fade_in', '')) and '设计标定' in str(st.get('_note_fade_in', '')),
      repr(str(st.get('_note_fade_in', ''))[:50]))
check('streak.color=[1,1,1]', st.get('color') == [1.0, 1.0, 1.0], repr(st.get('color')))
check('streak.amount=1', st.get('amount') == 1, repr(st.get('amount')))
check('flash.size=[120,12]', fl.get('size') == [120.0, 12.0], repr(fl.get('size')))
check('flash.color_from α=0.85', approx((fl.get('color_from') or [0] * 4)[3], 0.85), repr(fl.get('color_from')))
check('flash.color_from rgb=白', (fl.get('color_from') or [])[:3] == [1.0, 1.0, 1.0], repr(fl.get('color_from')))
check('flash.color_to 不动=[0.7515,0.6615,1,0]',
      fl.get('color_to') == [0.7515, 0.6615, 1.0, 0.0], repr(fl.get('color_to')))
check('sparks.size=[10,22]（条目 55 R1 观感校准：消硬边方块）',
      sp.get('size') == [10.0, 22.0], repr(sp.get('size')))
check('sparks.amount=6（条目 55 R1：减过曝）', sp.get('amount') == 6, repr(sp.get('amount')))
check('sparks.life=0.3（不动）', sp.get('life') == 0.3, repr(sp.get('life')))
check('flash.flash_fade_in_s=0.09（条目 55 R2：0.05→0.09 渐亮可辨）',
      approx(fl.get('flash_fade_in_s'), 0.09), repr(fl.get('flash_fade_in_s')))
check('flash.flash_fade_out_s=0.18（条目 55 R2：0.09→0.18 渐隐可辨）',
      approx(fl.get('flash_fade_out_s'), 0.18), repr(fl.get('flash_fade_out_s')))
check('flash.flash_fadeout_start=0.5（cross 结构不动）',
      approx(fl.get('flash_fadeout_start'), 0.5), repr(fl.get('flash_fadeout_start')))
_vis = 0.09 * 0.5 + 0.18
check('flash 包络总长自洽 ≈0.225s（fadeIn×foStart+fadeOut，峰值段前渐亮 0.045s）',
      abs(_vis - (fl.get('flash_fade_in_s', 0) * fl.get('flash_fadeout_start', 0)
                  + fl.get('flash_fade_out_s', 0))) < 1e-9, f'{_vis:.3f}s')

# ============ 2) muzzle 数据链真值（条目 52：膛线烘焙 v2 交叉验证）============
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
# 烘焙前端/后端中点（未烘焙原始值）
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
check('θ_mid ≈ 0°（条目 51 勘误：θ_top=-2.76 系顶缘污染伪影）',
      abs(theta_mid) < 1.0, f'{theta_mid:.2f}°')
check('hull JSON 系角 ≈ -15.27°（条目 51 预期：u 轴角回归，bake49 -12.50° 伪影撤销）',
      abs(ang_hull - (-15.27)) < 0.3, f'{ang_hull:.2f}°')
mdir = fx.get('muzzle_dir') or [0, 0]
ang_mdir = math.degrees(math.atan2(mdir[1], mdir[0]))
print(f'      hull 长轴角={ang_hull:.2f}°  muzzle_dir 角={ang_mdir:.2f}°  固有差={ang_hull - ang_mdir:.2f}°')
check('hull 长轴与 muzzle_dir 固有差 ≈ -5.08°（u 轴时代值回归，条目 48 取证）',
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
check('不动 sparks.speed=[203,435]', sp.get('speed') == [203.0, 435.0], repr(sp.get('speed')))
check('不动 sparks.spread=14', sp.get('spread') == 14.0, repr(sp.get('spread')))
check('不动 sparks.drag=7', sp.get('drag') == 7.0, repr(sp.get('drag')))
check('不动 sparks.emission_sphere_radius=7.25', sp.get('emission_sphere_radius') == 7.25,
      repr(sp.get('emission_sphere_radius')))
check('不动 sparks.anim_speed=1', sp.get('anim_speed') == 1.0, repr(sp.get('anim_speed')))
check('不动 sparks.anim_offset=[0,1]', sp.get('anim_offset_min') == 0.0 and sp.get('anim_offset_max') == 1.0,
      f"{sp.get('anim_offset_min')}/{sp.get('anim_offset_max')}")
check('不动 sparks.h/v_frames=3/3', sp.get('h_frames') == 3.0 and sp.get('v_frames') == 3.0,
      f"{sp.get('h_frames')}/{sp.get('v_frames')}")
check('不动 sparks.strip_texture 路径', sp.get('strip_texture', '').endswith('/fx_sparks3x3_grid.png'),
      repr(sp.get('strip_texture')))
check('不动 flash.offset_x=5.8', fl.get('offset_x') == 5.8, repr(fl.get('offset_x')))
check('不动 flash.amount=3/rand=0.33', fl.get('amount') == 3 and fl.get('lifetime_randomness') == 0.33,
      f"{fl.get('amount')}/{fl.get('lifetime_randomness')}")
check('flash.life=0.36（条目 55 勘误之二红线 A：0.15→0.36，压缩钳死根修）',
      approx(fl.get('life'), 0.36), repr(fl.get('life')))
check('flash 压缩判据恒不触发（life×(1−0.33)=0.2412 > 包络 0.225 → 0.225 全 k 可达）',
      fl.get('life', 0) * (1 - fl.get('lifetime_randomness', 0)) > _vis + 1e-9,
      f"l_min={fl.get('life', 0) * (1 - fl.get('lifetime_randomness', 0)):.4f} vs envelope={_vis:.4f}")
check('不动 streak.texture 路径', st.get('texture', '').endswith('/fx_streak_mask.png'),
      repr(st.get('texture')))
check('不动 flash.texture 路径', fl.get('texture', '').endswith('/fx_flash.png'),
      repr(fl.get('texture')))

# ============ 2b) ling hit_fx 新键全表（条目 57 新增）=========================
hfx = read_cfg(CFG_REL).get('hit_fx', {})
hd, hfd, hp, hfl, hr, hsp, hstk, hg = (hfx.get('disc', {}), hfx.get('flash_disc', {}),
                                       hfx.get('paxi', {}), hfx.get('flare', {}),
                                       hfx.get('rays', {}), hfx.get('sparks', {}),
                                       hfx.get('streaks', {}), hfx.get('glow', {}))
hmp = hfx.get('max_size_px', {})

check('hit_fx 顶层 enabled=true', hfx.get('enabled') is True, repr(hfx.get('enabled')))
check('hit_fx.pipeline=hitburst（★新管线探测键，缺键=legacy 碎片路径）',
      hfx.get('pipeline') == 'hitburst', repr(hfx.get('pipeline')))
check('hit_fx.bursts=[0.0,0.43]（双脉冲真值，README §3/§4）',
      hfx.get('bursts') == [0.0, 0.43], repr(hfx.get('bursts')))
check('hit_fx.scale=1.0', approx(hfx.get('scale'), 1.0), repr(hfx.get('scale')))
check('hit_fx.size_boost=1.5（bloom 补偿豁免；1.0=纯真值）',
      approx(hfx.get('size_boost'), 1.5), repr(hfx.get('size_boost')))
check('hit_fx.offset_y=0.0（旧 -20 为怪物原点口径，不适用 vfx_flying_slash 触发）',
      approx(hfx.get('offset_y'), 0.0), repr(hfx.get('offset_y')))
check('hit_fx.track_target=false（v1 预留键不实现，DESIGN §5）',
      hfx.get('track_target') is False, repr(hfx.get('track_target')))
check('hit_fx 旧 legacy 死键已退役（anims/textures/color_a/color_b/fade_to 不在新段）',
      not any(k2 in hfx for k2 in ('anims', 'textures', 'color_a', 'color_b', 'fade_to')),
      repr(sorted(hfx.keys())[:10]))
check('max_size_px 七键={disc:320,paxi:480,flare:256,ray:256,spark:24,streak:64,glow:48}',
      hmp == {'disc': 320, 'paxi': 480, 'flare': 256, 'ray': 256,
              'spark': 24, 'streak': 64, 'glow': 48}, repr(hmp))
check('disc(E0).size=[220,220]（假设 A2 基准）', hd.get('size') == [220.0, 220.0], repr(hd.get('size')))
check('disc(E0).life=0.5/grow_in_s=0.1（size-by-life 0→1、TimeScale=5 真值）',
      approx(hd.get('life'), 0.5) and approx(hd.get('grow_in_s'), 0.1),
      f"{hd.get('life')}/{hd.get('grow_in_s')}")
check('disc(E0).color_from=[0.7515,0.6615,1]/color_to=[0.3906,0.6646,1]（真值色曲线）',
      hd.get('color_from') == [0.7515, 0.6615, 1.0] and hd.get('color_to') == [0.3906, 0.6646, 1.0],
      f"{hd.get('color_from')}/{hd.get('color_to')}")
check('disc(E0).texture=hitfx_ring.png', str(hd.get('texture', '')).endswith('/hitfx_ring.png'),
      repr(hd.get('texture')))
check('flash_disc(E1).size=[190,190]（E0×0.6/0.7 同基准换算）',
      hfd.get('size') == [190.0, 190.0], repr(hfd.get('size')))
check('flash_disc(E1).hold_frac=0.85/core_scale=0.6/core_hold_frac=0.6（T1 包络配比豁免）',
      approx(hfd.get('hold_frac'), 0.85) and approx(hfd.get('core_scale'), 0.6)
      and approx(hfd.get('core_hold_frac'), 0.6),
      f"{hfd.get('hold_frac')}/{hfd.get('core_scale')}/{hfd.get('core_hold_frac')}")
check('paxi(E2).size=[348,15]（1200×50×0.29 真值）', hp.get('size') == [348.0, 15.0], repr(hp.get('size')))
check('paxi(E2).color=[0.3698,0.6531,1]/alpha_from=0.6/shrink_x=0.5',
      hp.get('color') == [0.3698, 0.6531, 1.0] and approx(hp.get('alpha_from'), 0.6)
      and approx(hp.get('shrink_x'), 0.5),
      f"{hp.get('color')}/{hp.get('alpha_from')}/{hp.get('shrink_x')}")
check('paxi(E2).texture=fx_flash.png 复用（零新条目）',
      str(hp.get('texture', '')).endswith('/fx_flash.png'), repr(hp.get('texture')))
check('flare(E3).size=[145,145]（500×0.29 真值；boost 后 218px）',
      hfl.get('size') == [145.0, 145.0], repr(hfl.get('size')))
check('flare(E3).life=0.1/hold_frac=0.8（B(t)<1 于 t≈0.82，T1 饱和段）',
      approx(hfl.get('life'), 0.1) and approx(hfl.get('hold_frac'), 0.8),
      f"{hfl.get('life')}/{hfl.get('hold_frac')}")
check('flare(E3).texture=hitfx_flare.png（ray_tex_flare_03 直拷）',
      str(hfl.get('texture', '')).endswith('/hitfx_flare.png'), repr(hfl.get('texture')))
check('rays(E4).amount=7/life=[0.12,0.2]（burst 7+7、Peak 15 真值）',
      hr.get('amount') == 7 and hr.get('life') == [0.12, 0.2], f"{hr.get('amount')}/{hr.get('life')}")
check('rays(E4).size_w=[14.5,58]/size_h=[58,217.5]（X∈[50,200]、Y∈[200,750]×0.29）',
      hr.get('size_w') == [14.5, 58.0] and hr.get('size_h') == [58.0, 217.5],
      f"{hr.get('size_w')}/{hr.get('size_h')}")
check('rays(E4).color_core=[0.61,0.8,1]/core_alpha=0.9（T2 mix(C_n,白,0.5)）',
      hr.get('color_core') == [0.61, 0.8, 1.0] and approx(hr.get('core_alpha'), 0.9),
      f"{hr.get('color_core')}/{hr.get('core_alpha')}")
check('rays(E4).halo=true/halo_color=[0.219,0.597,1]/halo_alpha=0.4/halo_size_mul=1.5（T2/T3）',
      hr.get('halo') is True and hr.get('halo_color') == [0.219, 0.597, 1.0]
      and approx(hr.get('halo_alpha'), 0.4) and approx(hr.get('halo_size_mul'), 1.5),
      f"{hr.get('halo')}/{hr.get('halo_color')}/{hr.get('halo_alpha')}/{hr.get('halo_size_mul')}")
check('rays(E4).fade_tail=0.3（末 30% 落 alpha，防 pop-out 豁免）',
      approx(hr.get('fade_tail'), 0.3), repr(hr.get('fade_tail')))
check('rays(E4).texture=hitfx_ray.png（T_Fx_Ray01 直拷）',
      str(hr.get('texture', '')).endswith('/hitfx_ray.png'), repr(hr.get('texture')))
check('sparks(E5).amount=15/life=[0.1,0.25]（burst 15+15、Peak 31 真值）',
      hsp.get('amount') == 15 and hsp.get('life') == [0.1, 0.25],
      f"{hsp.get('amount')}/{hsp.get('life')}")
check('sparks(E5).size=[10,14]（真值 7.25px，豁免防不可见）',
      hsp.get('size') == [10.0, 14.0], repr(hsp.get('size')))
check('sparks(E5).speed=[40,90]（真值 23.2-58 偏静，R8 豁免提速）',
      hsp.get('speed') == [40.0, 90.0], repr(hsp.get('speed')))
check('sparks(E5).emission_sphere_radius=4.35/spread=180（球壳 r=15×0.29/全向）',
      approx(hsp.get('emission_sphere_radius'), 4.35) and approx(hsp.get('spread'), 180.0),
      f"{hsp.get('emission_sphere_radius')}/{hsp.get('spread')}")
check('sparks(E5).gravity=284.2（UE (0,0,-980)×0.29，y-down 取正）',
      approx(hsp.get('gravity'), 284.2), repr(hsp.get('gravity')))
check('sparks(E5).drag=[5,10]（AccelerationDrag 区间真值，直通 Damping）',
      hsp.get('drag') == [5.0, 10.0], repr(hsp.get('drag')))
check('sparks(E5).h/v_frames=3/3/anim_speed=1/anim_offset=[0,1]（SubUV 3×3 真值）',
      hsp.get('h_frames') == 3 and hsp.get('v_frames') == 3 and approx(hsp.get('anim_speed'), 1.0)
      and hsp.get('anim_offset_min') == 0.0 and hsp.get('anim_offset_max') == 1.0,
      f"{hsp.get('h_frames')}/{hsp.get('v_frames')}/{hsp.get('anim_speed')}")
check('sparks(E5).color_a=[0.286,0.3,1]/color_b=[0.083,0.78,1]（StartColor 随机对真值）',
      hsp.get('color_a') == [0.286, 0.3, 1.0] and hsp.get('color_b') == [0.083, 0.78, 1.0],
      f"{hsp.get('color_a')}/{hsp.get('color_b')}")
check('sparks(E5).hot_white_frac=0.65（ColorScale 10→1 白热段 T1）',
      approx(hsp.get('hot_white_frac'), 0.65), repr(hsp.get('hot_white_frac')))
check('sparks(E5).angular_velocity=[-0.7,0.7]（rad/s 真值 RotationRate）',
      hsp.get('angular_velocity') == [-0.7, 0.7], repr(hsp.get('angular_velocity')))
check('sparks(E5).strip_texture=fx_sparks3x3_grid.png 复用（零新条目）',
      str(hsp.get('strip_texture', '')).endswith('/fx_sparks3x3_grid.png'),
      repr(hsp.get('strip_texture')))
check('streaks(E7).amount=15/life=[0.2,0.35]（burst 15+15 真值）',
      hstk.get('amount') == 15 and hstk.get('life') == [0.2, 0.35],
      f"{hstk.get('amount')}/{hstk.get('life')}")
check('streaks(E7).size=[3,5]（真值 0.44-0.87px 亚像素，A4 豁免）',
      hstk.get('size') == [3.0, 5.0], repr(hstk.get('size')))
check('streaks(E7).speed=[40,90]/align_velocity=30/drag=5（同 E5 壳同速+方向载体先例）',
      hstk.get('speed') == [40.0, 90.0] and approx(hstk.get('align_velocity'), 30.0)
      and approx(hstk.get('drag'), 5.0),
      f"{hstk.get('speed')}/{hstk.get('align_velocity')}/{hstk.get('drag')}")
check('streaks(E7).color_from=[0.479,0.713,1]/color_to=[0.752,0.661,1]/shrink_tail=1.0（Y 10→0 真值）',
      hstk.get('color_from') == [0.479, 0.713, 1.0] and hstk.get('color_to') == [0.752, 0.661, 1.0]
      and approx(hstk.get('shrink_tail'), 1.0),
      f"{hstk.get('color_from')}/{hstk.get('color_to')}/{hstk.get('shrink_tail')}")
check('streaks(E7).texture=hitfx_streak.png（16×160 程序生成，A4）',
      str(hstk.get('texture', '')).endswith('/hitfx_streak.png'), repr(hstk.get('texture')))
check('glow(E8).amount=5/life=[0.15,0.3]（burst 5+5、Peak 11 真值）',
      hg.get('amount') == 5 and hg.get('life') == [0.15, 0.3], f"{hg.get('amount')}/{hg.get('life')}")
check('glow(E8).size=[10,26]（粒径 X∈[25,100]×0.29）', hg.get('size') == [10.0, 26.0], repr(hg.get('size')))
check('glow(E8).emission_sphere_radius=43.5（r=150×0.29，boost 后 65px）',
      approx(hg.get('emission_sphere_radius'), 43.5), repr(hg.get('emission_sphere_radius')))
check('glow(E8).speed=[14.5,32]/direction=[0,-1]/spread=25/drag=6（Z∈[50,100] 向上喷真值）',
      hg.get('speed') == [14.5, 32.0] and hg.get('direction') == [0, -1]
      and approx(hg.get('spread'), 25.0) and approx(hg.get('drag'), 6.0),
      f"{hg.get('speed')}/{hg.get('direction')}/{hg.get('spread')}/{hg.get('drag')}")
check('glow(E8).h/v_frames=2/2/alpha_peak=1.0（SubUV 2×2/StartAlpha 0.1-0.2×10 饱和 LDR 近似）',
      hg.get('h_frames') == 2 and hg.get('v_frames') == 2 and approx(hg.get('alpha_peak'), 1.0),
      f"{hg.get('h_frames')}/{hg.get('v_frames')}/{hg.get('alpha_peak')}")
check('glow(E8).color_a=[0.401,0.74,1]/color_b=[0.752,0.661,1]（StartColor 随机对真值）',
      hg.get('color_a') == [0.401, 0.74, 1.0] and hg.get('color_b') == [0.752, 0.661, 1.0],
      f"{hg.get('color_a')}/{hg.get('color_b')}")
check('glow(E8).texture=hitfx_glow2x2.png（fr_glow_902_2x2 alpha 归一化）',
      str(hg.get('texture', '')).endswith('/hitfx_glow2x2.png'), repr(hg.get('texture')))
check('hit_fx._note_schema 注记在包（条目57）',
      '条目57' in str(hfx.get('_note_schema', '')) and 'DESIGN.md' in str(hfx.get('_note_schema', '')),
      repr(str(hfx.get('_note_schema', ''))[:50]))
check('hit_fx._note_size_boost 注记在包（bloom 补偿出处）',
      'bloom' in str(hfx.get('_note_size_boost', '')) and '条目43' in str(hfx.get('_note_size_boost', '')),
      repr(str(hfx.get('_note_size_boost', ''))[:40]))
check('hit_fx._note_bursts 注记在包（双脉冲出处）',
      '0.43' in str(hfx.get('_note_bursts', '')), repr(str(hfx.get('_note_bursts', ''))[:40]))

# ============ 2c) silent profile hit_fx 零影响（legacy 键集不动）===============
check('silent config 在包内', SILENT_CFG_REL in files)
shfx = read_cfg(SILENT_CFG_REL).get('hit_fx', {})
banned_hit = ('pipeline', 'bursts', 'size_boost', 'track_target', 'max_size_px',
              'disc', 'flash_disc', 'paxi', 'flare', 'rays', 'streaks', 'glow')
leaked_hit = [k2 for k2 in banned_hit if k2 in shfx]
check('silent hit_fx 无新管线键（pipeline/bursts/size_boost/八发射器段零泄漏）',
      not leaked_hit, f'命中={leaked_hit}')
check('silent hit_fx 维持 legacy 键（textures/sparks/anims 等）',
      ('textures' in shfx or 'sparks' in shfx or not shfx),
      f"keys={sorted(shfx.keys())[:10]}")

# ============ 3) 包内纹理源 PNG 尺寸核验（attack fx 3 张 + hit fx 5 张）========
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

for name, (w, h_) in HIT_PNG_SIZE.items():
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

# ============ 3b) 条目 55 勘误之二：sparks 内容轮廓羽化核验 ============
import io
import hashlib as _hlib
_sp_rel = f'{FX_DIR}/fx_sparks3x3_grid.png'
_sp_off, _sp_size = files[_sp_rel]
_sp_blob = d[_sp_off:_sp_off + _sp_size]
_sp_sha = _hlib.sha256(_sp_blob).hexdigest()
SPARK_OLD_SHA = '6b99f4fb427277859f02fe5046c2826b89dcf565dbc6562c41e0554f31f99189'
check('sparks 包内 sha256 ≠ 旧版 6b99f4fb…（条目 55 勘误之二：零操作羽化已根治）',
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
check('sparks 3×3 全部帧界 alpha=0（布局不变；羽化已改作用于内容轮廓）', _bmax == 0,
      f'帧界 alpha 最大值={_bmax}')
_sp_partial = int(((_sp_a >= 1) & (_sp_a <= 254)).sum())
SPARK_PARTIAL_MIN = 8000   # 旧版 5470（ASTC 解码残留），羽化后 15469
check('sparks 内容边缘 alpha 梯度存在（过渡像素 1~254 计数>阈值，内容轮廓羽化生效）',
      _sp_partial > SPARK_PARTIAL_MIN, f'{_sp_partial} > {SPARK_PARTIAL_MIN}')
check('sparks 帧内容仍在（max alpha>0，羽化只软边不抹形）', int(_sp_a.max()) > 0,
      f'max={int(_sp_a.max())}')

# ============ 3c) 条目 56：flash 纹理 ramp 拉宽核验（R1 硬边） ============
_fl_rel = f'{FX_DIR}/fx_flash.png'
_fl_off, _fl_size = files[_fl_rel]
_fl_blob = d[_fl_off:_fl_off + _fl_size]
_fl_sha = _hlib.sha256(_fl_blob).hexdigest()
FLASH_OLD_SHA = '79e40d5837810135cd4819258dd8c01813b4b9995ec31f045c96c47604ead8db'
check('flash 包内 sha256 ≠ 旧版 79e40d58…（条目 56：后缘 ramp 重生成，区分度）',
      _fl_sha != FLASH_OLD_SHA, _fl_sha[:16] + '…')
_fl_im = Image.open(io.BytesIO(_fl_blob)).convert('RGBA')
_fl_a = np.asarray(_fl_im)[..., 3]
_midy = _fl_im.height // 2
_rear = [int(_fl_a[_midy, x]) for x in range(0, 17)]
check('flash 后缘 ramp col0..16（中心行）严格递增（ramp 19.2px ≥16px 渐入带）',
      all(_rear[i] < _rear[i + 1] for i in range(len(_rear) - 1)) and _rear[16] > _rear[1] + 30,
      f'{_rear}')
_fl_amax = int(_fl_a.max())
check('flash 峰值 alpha ≈0.835（条目 56：along 峰=0.9^1.1×0.9375，旧 0.886 作废）',
      0.82 <= _fl_amax / 255 <= 0.85, f'{_fl_amax / 255:.4f}')

# ============ 3d) 条目 57：hit fx 5 张纹理内容核验 ============
# ① glow alpha 归一化（包内 alpha max=255 ≠ 源 139，A5）；
# ② ring 剖面自检（四边 alpha=0/中心≈0.25/rim 峰 r≈0.85±0.04）；
# ③ streak 四边 alpha=0+峰值。
_gw_rel = f'{FX_DIR}/hitfx_glow2x2.png'
_gw_off, _gw_size = files[_gw_rel]
_gw_blob = d[_gw_off:_gw_off + _gw_size]
_gw_sha = _hlib.sha256(_gw_blob).hexdigest()
check('glow 包内 sha256 ≠ 源 frame_fr_glow_902_2x2（alpha 归一化有区分度）',
      _gw_sha != _hlib.sha256(
          Path(r'D:\项目\dragonraja\sts2_work\hitfx_research_2026-10-11\textures'
               r'\frame_fr_glow_902_2x2.png').read_bytes()).hexdigest(),
      _gw_sha[:16] + '…')
_gw_im = Image.open(io.BytesIO(_gw_blob)).convert('RGBA')
_gw_a = np.asarray(_gw_im)[..., 3]
check('glow 包内 alpha max=255（A5 归一化生效，≠源 139）', int(_gw_a.max()) == 255,
      f'max={int(_gw_a.max())}')
check('glow 包内 alpha 均值≈源×1.83（46.1→~84，整图提亮非局部）',
      75 <= float(_gw_a.mean()) <= 95, f'mean={float(_gw_a.mean()):.1f}')

_rg_rel = f'{FX_DIR}/hitfx_ring.png'
_rg_off, _rg_size = files[_rg_rel]
_rg_im = Image.open(io.BytesIO(d[_rg_off:_rg_off + _rg_size])).convert('RGBA')
_rg = np.asarray(_rg_im)[..., 3].astype(np.int32)
_bord = max(int(_rg[0, :].max()), int(_rg[-1, :].max()),
            int(_rg[:, 0].max()), int(_rg[:, -1].max()))
check('ring 四边 alpha=0（四边硬归零自检）', _bord == 0, f'边 alpha max={_bord}')
_cc = (_rg_im.width - 1) / 2.0
_RR = 246.0
_center = int(_rg[int(round(_cc)), int(round(_cc))])
check('ring 中心（内盘）alpha≈0.25（径向剖面 0.15~0.35）',
      0.15 <= _center / 255 <= 0.35, f'{_center / 255:.3f}')
_ts = np.arange(0.0, 1.0, 0.002)
_vs = [_rg[int(round(_cc)), min(int(round(_cc + t * _RR)), _rg_im.width - 1)] for t in _ts]
_tpeak = float(_ts[int(np.argmax(_vs))])
check(f'ring rim 峰位置 r≈0.85（A2 剖面定案）', abs(_tpeak - 0.85) <= 0.04, f't={_tpeak:.3f}')
check('ring rim 峰值 alpha≈1.0（≥0.9）', max(_vs) / 255 >= 0.9, f'{max(_vs) / 255:.3f}')
check('ring 外缘（t=1.0）alpha=0',
      int(_rg[int(round(_cc)), min(int(round(_cc + _RR)), _rg_im.width - 1)]) == 0)

_sk_rel = f'{FX_DIR}/hitfx_streak.png'
_sk_off, _sk_size = files[_sk_rel]
_sk_im = Image.open(io.BytesIO(d[_sk_off:_sk_off + _sk_size])).convert('RGBA')
_sk = np.asarray(_sk_im)[..., 3].astype(np.int32)
_sb = max(int(_sk[0, :].max()), int(_sk[-1, :].max()),
          int(_sk[:, 0].max()), int(_sk[:, -1].max()))
check('streak 16×160 四边 alpha=0', _sb == 0, f'边 alpha max={_sb}')
check('streak 中心列峰值 alpha≥0.80',
      int(_sk[:, _sk_im.width // 2].max()) / 255 >= 0.80,
      f'{int(_sk[:, _sk_im.width // 2].max()) / 255:.3f}')

# ============ 4) silent profile attack_fx 零影响 ============
check('silent attack_fx 在包内', 'attack_fx' in read_cfg(SILENT_CFG_REL))
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

# ============ 5) fx 纹理：源 + .import + ctex（attack 3 张 + hit 5 张）=========
for name in FX_NAMES + HIT_NAMES:
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

# ============ 6) dll 字符串核验（UTF-16LE + UTF-8 双查）============
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
              'streak_fade_in_s',
              # 条目 57 新增：hit_fx 新管线日志锚点 + config 键（C# 读取点）
              'hit fx armed', 'hit fx: wave=', 'emitters disc=', 'hit fx: flare size=',
              'hit fx: cleanup seq=', 'breaker tripped kind=', 'texture missing',
              'pipeline', 'hitburst', 'flash_disc', 'track_target', 'size_boost',
              'grow_in_s', 'hold_frac', 'core_scale', 'core_hold_frac',
              'alpha_from', 'shrink_x', 'fade_tail', 'halo_size_mul', 'core_alpha',
              'shrink_tail', 'align_velocity', 'hot_white_frac', 'alpha_peak',
              'emission_sphere_radius', 'particle_flag_align_y', 'angular_velocity'):
        check(f'{dllp.parent.parent.name} 含 {s!r}',
              s.encode('utf-16-le') in b or s.encode('utf-8') in b)
    check(f'{dllp.parent.parent.name} 旧 follow tick1 字符串已退役',
          'follow tick1'.encode('utf-16-le') not in b and 'follow tick1'.encode('utf-8') not in b)

# ============ 7) sha256：仓内产物 vs 安装实物 ============
for name in ('LingChar.dll', 'LingChar.pck'):
    src = (ROOT / 'bin/Release/net9.0' / name) if name.endswith('.dll') else (ROOT / name)
    inst = MODS / name
    h1 = hashlib.sha256(src.read_bytes()).hexdigest()
    h2 = hashlib.sha256(inst.read_bytes()).hexdigest()
    check(f'{name} sha 仓内==安装', h1 == h2, h1[:16] + '…')
    print(f'      {name} sha256 = {h1}')
    print(f'      {name} size = {src.stat().st_size} B')

# ============ 8) 条目 57：双 pck 交集复核（铁律 2）============================
SILENT_PCK = Path(r'D:\项目\dragonraja\sts2_work\LingSilentSkin_battle\LingSilentSkin.pck')
if SILENT_PCK.exists():
    silent_files, _ = list_pck(str(SILENT_PCK))
    inter = sorted(set(files.keys()) & set(silent_files.keys()))
    check('LingChar.pck ∩ LingSilentSkin.pck = ∅（铁律 2 保持，218 条新增路径全 ling 专属）',
          not inter, f'交集={inter[:5]}')
    _ling_prefixed = [n for n in HIT_NAMES if f'{FX_DIR}/{n}' in files]
    check('5 张 hitfx 新路径全部落在 ling 专属目录',
          len(_ling_prefixed) == 5, f'{_ling_prefixed}')
else:
    check('LingSilentSkin.pck 存在（交集复核跳过条件）', False, '皮肤 pck 不存在')

print()
print('== 汇总:', '全部通过' if not fails else f'FAIL x{len(fails)}: {fails}')
sys.exit(1 if fails else 0)
