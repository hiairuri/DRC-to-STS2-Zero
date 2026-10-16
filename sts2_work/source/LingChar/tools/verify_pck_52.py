#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""条目 52 包内核验：δ 定案诊断构建 + 膛线烘焙 v2（双缘中线 + conv-B）。
在 verify_pck_49.py（104 项）基础上更新：
  1) muzzle_hull 断言升级：verify 侧**独立实现** v2.1 同源算法——
     精确众数带滑套段检测（tol=0）+ 带内中线拟合 θ_mid + conv-B UV 仿射
     （Δy_px→+W，条目 51 渲染无竖直翻转取证）+ Rk'=F−|F−Rk|·bore_dir；
     预期 θ_mid≈0°（bake49 的 θ_top=-2.76° 系顶缘污染伪影，勘误），
     hull JSON 轴角回到 ≈−15.27°（u 轴角）；
  2) 新增断言：config 键 dir_rot_fix 存在且 ==0.0（默认关，行为=条目 51
     产物）；_note_dir_rot_fix 值来源注记在包（hook 日志对账，非截图反解）；
  3) dll 字符串更新：tick1 诊断行新格式（n2rot=/sprrot=/n2gs=/fix=）、
     过期回收日志（follower expired/recycled）、dir_rot_fix 生效日志；
     旧 'follow tick1' 字符串已退役（改为断言其不存在，防新旧混淆）；
  4) 其余断言（bursts/track_muzzle/观感参数/纹理/silent 零影响/sha 对账）
     与 49 一致。

复用 GDPC v3 解析（dir 偏移@0x20，file_base 0x70，config 段可能 zlib 压缩）。
用法: "C:/Program Files/Python310/python.exe" tools/verify_pck_52.py
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


# ============ 1) ling profile config：条目 45/47/48/49 定案键沿用 ============
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
# ---- 条目 52 新增：dir_rot_fix 默认 0（关，行为=条目 51 产物）----
check('dir_rot_fix=0.0（默认关，δ 注入校准键待 hook 定案后填入）',
      approx(fx.get('dir_rot_fix'), 0.0), repr(fx.get('dir_rot_fix')))
check('_note_dir_rot_fix 值来源注记在包（hook 日志对账，非截图反解）',
      '非截图反解' in str(fx.get('_note_dir_rot_fix', '')), repr(str(fx.get('_note_dir_rot_fix', ''))[:50]))
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
check('streak.color=[1,1,1]', st.get('color') == [1.0, 1.0, 1.0], repr(st.get('color')))
check('streak.amount=1', st.get('amount') == 1, repr(st.get('amount')))
check('flash.size=[120,12]', fl.get('size') == [120.0, 12.0], repr(fl.get('size')))
check('flash.color_from α=0.85', approx((fl.get('color_from') or [0] * 4)[3], 0.85), repr(fl.get('color_from')))
check('flash.color_from rgb=白', (fl.get('color_from') or [])[:3] == [1.0, 1.0, 1.0], repr(fl.get('color_from')))
check('flash.color_to 不动=[0.7515,0.6615,1,0]',
      fl.get('color_to') == [0.7515, 0.6615, 1.0, 0.0], repr(fl.get('color_to')))
check('sparks.size=[16,34]', sp.get('size') == [16.0, 34.0], repr(sp.get('size')))
check('sparks.amount=10', sp.get('amount') == 10, repr(sp.get('amount')))
check('sparks.life=0.3', sp.get('life') == 0.3, repr(sp.get('life')))

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

# ---- verify 侧独立实现：atlas 区域解析 + 精确众数带中线 + conv-B UV 仿射 ----
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
# 精确众数带（v2.1）：滑套段=上下双缘同时一致的最长水平段
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
# conv-B（条目 51：渲染无竖直翻转）：Δy_px→+W/(rh−1)
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
# 烘焙不变量：F=枪口尖不动（位置锚）、轴长不变（只转方向）
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
check('不动 flash.amount=3/life=0.15/rand=0.33',
      fl.get('amount') == 3 and fl.get('life') == 0.15 and fl.get('lifetime_randomness') == 0.33,
      f"{fl.get('amount')}/{fl.get('life')}/{fl.get('lifetime_randomness')}")
check('不动 streak.texture 路径', st.get('texture', '').endswith('/fx_streak_mask.png'),
      repr(st.get('texture')))
check('不动 flash.texture 路径', fl.get('texture', '').endswith('/fx_flash.png'),
      repr(fl.get('texture')))

# ============ 3) 包内纹理源 PNG 尺寸核验 ============
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

# ============ 4) silent profile attack_fx 零影响 ============
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

# ============ 5) 三张 fx 纹理：源 + .import + ctex ============
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

# ============ 6) dll 字符串核验（UTF-16LE + UTF-8 双查）============
# 条目 52 更新：tick1 新格式（n2rot/sprrot/n2gs/fix）+ 过期回收日志 +
# dir_rot_fix 生效日志；旧 'follow tick1' 字符串退役（断言不存在）。
for dllp in (ROOT / 'bin/Release/net9.0/LingChar.dll', MODS / 'LingChar.dll'):
    b = dllp.read_bytes()
    for s in ('dirdeg=', 'dirsrc=', 'local_to_world', 'muzzle_gpos2', 'breaker tripped',
              'track=on', 'track=dropped', 'track_muzzle',
              'muzzle_hull', 'muzzle_bone', 'hull_2pt', 'track=downgraded',
              '回退 bone=', 'dir0deg=',
              # 条目 49 沿用：每帧重解析锚骨
              'find_bone', 'get_skeleton',
              # 条目 52 新增：tick1 诊断升级 + 过期回收日志 + dir_rot_fix
              'tick1 kind=', 'n2rot=', 'sprrot=', 'n2gs=', 'fix=',
              'follower expired', 'follower recycled', 'dir_rot_fix'):
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

print()
print('== 汇总:', '全部通过' if not fails else f'FAIL x{len(fails)}: {fails}')
sys.exit(1 if fails else 0)
