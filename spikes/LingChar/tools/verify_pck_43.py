#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""条目 43 包内核验：枪口基点/枪管轴/五项观感豁免参数落包 + dll 字符串 + sha 一致性。

复用 verify_pck_42.py 的 GDPC v3 解析（dir 偏移@0x20，file_base 0x70）。
用法: python tools/verify_pck_43.py
"""
import hashlib
import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent

PCK = ROOT / 'LingChar.pck'
MODS = Path(r'D:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\Mods\LingChar')
CFG_REL = 'drc_dye/profiles/ling/animations/characters/ling/config.json'


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

# 1) 包内 ling profile config：条目 43 全部键
check('ling config 在包内', CFG_REL in files)
off, size = files[CFG_REL]
blob = d[off:off + size]
import zlib
if blob[:1] not in (b'{', b'['):
    blob = zlib.decompress(blob, 15, 100000000)
cfg = json.loads(blob)
fx = cfg.get('attack_fx', {})
sp = fx.get('sparks', {})
st = fx.get('streak', {})
fl = fx.get('flash', {})
mp = fx.get('max_size_px', {})

# 枪口基点/枪管轴（muzzle_len 保留作 fallback）
check('muzzle_offset=[137.7,-37.0]',
      approx(fx.get('muzzle_offset', [None])[0] if isinstance(fx.get('muzzle_offset'), list) else None, 137.7)
      and approx(fx.get('muzzle_offset')[1], -37.0), repr(fx.get('muzzle_offset')))
check('muzzle_dir=[0.984,-0.177]',
      approx(fx.get('muzzle_dir', [None])[0] if isinstance(fx.get('muzzle_dir'), list) else None, 0.984)
      and approx(fx.get('muzzle_dir')[1], -0.177), repr(fx.get('muzzle_dir')))
check('muzzle_len 保留=50.0(fallback)', fx.get('muzzle_len') == 50.0, repr(fx.get('muzzle_len')))
# 五项观感豁免（E1-E5）
check('E1 sparks.amount=10', sp.get('amount') == 10, repr(sp.get('amount')))
check('E2 sparks.life=0.3', sp.get('life') == 0.3, repr(sp.get('life')))
check('E3 sparks.size=[14,30]', sp.get('size') == [14.0, 30.0], repr(sp.get('size')))
check('E-sparks.anim_offset_min=0.0', sp.get('anim_offset_min') == 0.0, repr(sp.get('anim_offset_min')))
check('E-sparks.anim_offset_max=1.0', sp.get('anim_offset_max') == 1.0, repr(sp.get('anim_offset_max')))
check('E4 streak.halo_size_mul=2.2', st.get('halo_size_mul') == 2.2, repr(st.get('halo_size_mul')))
check('E4 streak.halo_alpha=0.45', st.get('halo_alpha') == 0.45, repr(st.get('halo_alpha')))
check('E5 flash.color_from α=0.55', approx((fl.get('color_from') or [0]*4)[3], 0.55),
      repr(fl.get('color_from')))
# streak 熔断上限对齐 halo 143px
check('max_size_px.streak=160', mp.get('streak') == 160.0, repr(mp.get('streak')))
# 不动项抽查（方案书铁律：sparks 速度/锥角/拖拽/球半径/动画速率、streak 尺寸/曲线/对齐、flash 尺寸/寿命/间隙、双拍）
check('不动 sparks.speed=[203,435]', sp.get('speed') == [203.0, 435.0], repr(sp.get('speed')))
check('不动 sparks.spread=14', sp.get('spread') == 14.0, repr(sp.get('spread')))
check('不动 sparks.drag=7', sp.get('drag') == 7.0, repr(sp.get('drag')))
check('不动 sparks.emission_sphere_radius=7.25', sp.get('emission_sphere_radius') == 7.25, repr(sp.get('emission_sphere_radius')))
check('不动 sparks.anim_speed=1', sp.get('anim_speed') == 1.0, repr(sp.get('anim_speed')))
check('不动 streak.size=[22,65]', st.get('size') == [22.0, 65.0], repr(st.get('size')))
check('不动 streak.scale_curve=[0.5,1]', st.get('scale_curve') == [0.5, 1.0], repr(st.get('scale_curve')))
check('不动 flash.size=[87,6]', fl.get('size') == [87.0, 6.0], repr(fl.get('size')))
check('不动 flash.offset_x=5.8', fl.get('offset_x') == 5.8, repr(fl.get('offset_x')))
check('不动 bursts=[0,0.43]', fx.get('bursts') == [0.0, 0.43], repr(fx.get('bursts')))

# 2) dll 字符串核验（.NET 字面量在 #US 堆 = UTF-16；顺带查 UTF-8）
for dllp in (ROOT / 'bin/Release/net9.0/LingChar.dll', MODS / 'LingChar.dll'):
    b = dllp.read_bytes()
    for s in ('muzzle_gpos2', 'muzzle_offset', 'muzzle_dir', 'anim_offset_min'):
        check(f'{dllp.name} 含 {s!r}({dllp.parent.parent.name})',
              s.encode('utf-16-le') in b or s.encode('utf-8') in b)

# 3) sha256：仓内产物 vs 安装实物
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
