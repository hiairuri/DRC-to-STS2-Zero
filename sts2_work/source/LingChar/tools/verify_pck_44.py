#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""条目 44 包内核验：方向链路定案（dir_source/barrel_deg）+ streak/flash
mode=sprite 路线 + 形状豁免参数（halo 1.5/0.25、streak max 120）落包 +
三张 fx 纹理（含程序生成新遮罩/闪光）+ silent profile 零影响 + dll 字符串。

复用 verify_pck_43.py 的 GDPC v3 解析（dir 偏移@0x20，file_base 0x70，
config 段可能 zlib 压缩）。
用法: "C:/Program Files/Python310/python.exe" tools/verify_pck_44.py
"""
import hashlib
import json
import sys
import zlib
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent

PCK = ROOT / 'LingChar.pck'
MODS = Path(r'D:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\Mods\LingChar')
CFG_REL = 'drc_dye/profiles/ling/animations/characters/ling/config.json'
SILENT_CFG_REL = 'drc_dye/profiles/ling/animations/characters/silent/config.json'
FX_DIR = 'drc_dye/profiles/ling/animations/characters/ling'
FX_NAMES = ('fx_sparks3x3_grid.png', 'fx_streak_mask.png', 'fx_flash.png')
# 条目 42 定案的火花 ctex 产品名（名字不变 = 火花纹理未被本轮污染）
SPARK_CTEX = 'fx_sparks3x3_grid.png-fd48a8638dfa798c1455fc7edf038349.ctex'


def list_pck(path):
    """LingChar.pck = pck_writer 的 GDPC v3 布局：dir 偏移@0x20(int64)，
    条目 = u32 padded_path_len + path(pad4) + i64 off(rel file_base 0x70)
    + i64 size + 16B md5 + u32 extra。"""
    import struct
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
        off += 16 + 16  # offset+size+md5
        off += 4        # v3 extra field
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


# ============ 1) ling profile config：条目 44 定案键 ============
check('ling config 在包内', CFG_REL in files)
fx = read_cfg(CFG_REL).get('attack_fx', {})
sp, st, fl, mp = fx.get('sparks', {}), fx.get('streak', {}), fx.get('flash', {}), fx.get('max_size_px', {})

# --- 新增键（D 项定案）---
check('dir_source=auto', fx.get('dir_source') == 'auto', repr(fx.get('dir_source')))
check('barrel_deg=-10.18', approx(fx.get('barrel_deg'), -10.18), repr(fx.get('barrel_deg')))
check('streak.mode=sprite', st.get('mode') == 'sprite', repr(st.get('mode')))
check('flash.mode=sprite', fl.get('mode') == 'sprite', repr(fl.get('mode')))
# --- 形状定案（⑦：halo 收敛 + halo 纳入钳制 + max 160→120）---
check('streak.halo_size_mul=1.5', st.get('halo_size_mul') == 1.5, repr(st.get('halo_size_mul')))
check('streak.halo_alpha=0.25', st.get('halo_alpha') == 0.25, repr(st.get('halo_alpha')))
check('max_size_px.streak=120', mp.get('streak') == 120.0, repr(mp.get('streak')))
# --- 保留的观感豁免（E1-E3/E5）---
check('E1 sparks.amount=10', sp.get('amount') == 10, repr(sp.get('amount')))
check('E2 sparks.life=0.3', sp.get('life') == 0.3, repr(sp.get('life')))
check('E3 sparks.size=[14,30]', sp.get('size') == [14.0, 30.0], repr(sp.get('size')))
check('E5 flash.color_from α=0.55', approx((fl.get('color_from') or [0] * 4)[3], 0.55),
      repr(fl.get('color_from')))

# --- 不动项抽查（sparks 全量 / streak / flash / bursts / muzzle_*）---
check('不动 muzzle_offset=[137.7,-37.0]',
      fx.get('muzzle_offset') == [137.7, -37.0], repr(fx.get('muzzle_offset')))
check('不动 muzzle_dir=[0.984,-0.177]',
      fx.get('muzzle_dir') == [0.984, -0.177], repr(fx.get('muzzle_dir')))
check('不动 muzzle_len=50.0(fallback)', fx.get('muzzle_len') == 50.0, repr(fx.get('muzzle_len')))
check('不动 bursts=[0,0.43]', fx.get('bursts') == [0.0, 0.43], repr(fx.get('bursts')))
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
check('不动 streak.size=[22,65]', st.get('size') == [22.0, 65.0], repr(st.get('size')))
check('不动 streak.scale_curve=[0.5,1]', st.get('scale_curve') == [0.5, 1.0], repr(st.get('scale_curve')))
check('不动 streak.color=[0.37,0.85,1]', st.get('color') == [0.37, 0.85, 1.0], repr(st.get('color')))
check('不动 streak.halo=true', st.get('halo') is True, repr(st.get('halo')))
check('不动 flash.size=[87,6]', fl.get('size') == [87.0, 6.0], repr(fl.get('size')))
check('不动 flash.offset_x=5.8', fl.get('offset_x') == 5.8, repr(fl.get('offset_x')))
check('不动 flash.amount=3/life=0.15/rand=0.33',
      fl.get('amount') == 3 and fl.get('life') == 0.15 and fl.get('lifetime_randomness') == 0.33,
      f"{fl.get('amount')}/{fl.get('life')}/{fl.get('lifetime_randomness')}")
check('不动 flash.color_to=[0.7515,0.6615,1,0]',
      fl.get('color_to') == [0.7515, 0.6615, 1.0, 0.0], repr(fl.get('color_to')))

# ============ 2) silent profile attack_fx 零影响 ============
check('silent config 在包内', SILENT_CFG_REL in files)
sfx = read_cfg(SILENT_CFG_REL).get('attack_fx', {})
banned = ('mode', 'dir_source', 'muzzle_offset', 'muzzle_dir')
leaked = []
for k in banned:
    if k in sfx:
        leaked.append(k)
    for seg in ('sparks', 'streak', 'flash'):
        if isinstance(sfx.get(seg), dict) and k in sfx[seg]:
            leaked.append(f'{seg}.{k}')
check('silent attack_fx 无 mode/dir_source/muzzle_offset/muzzle_dir', not leaked, f'命中={leaked}')
check('silent attack_fx 维持 legacy 键（textures/streak_texture）',
      ('textures' in sfx or 'streak_texture' in sfx or not sfx),
      f"keys={sorted(sfx.keys())[:8]}")

# ============ 3) 三张 fx 纹理：源 + .import + ctex 产品在包内 ============
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

# ============ 4) dll 字符串核验（UTF-16LE + UTF-8 双查）============
for dllp in (ROOT / 'bin/Release/net9.0/LingChar.dll', MODS / 'LingChar.dll'):
    b = dllp.read_bytes()
    for s in ('dirdeg=', 'dirsrc=', 'local_to_world', 'muzzle_gpos2'):
        check(f'{dllp.parent.parent.name} 含 {s!r}',
              s.encode('utf-16-le') in b or s.encode('utf-8') in b)

# ============ 5) sha256：仓内产物 vs 安装实物 ============
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
