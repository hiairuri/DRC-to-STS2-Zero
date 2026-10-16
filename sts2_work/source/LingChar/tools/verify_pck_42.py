#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""条目 42 包内核验：新纹理/profile config/dll 字符串/sha 一致性。

用法: python tools/verify_pck_42.py
"""
import hashlib
import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent

PCK = ROOT / 'LingChar.pck'
MODS = Path(r'D:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\Mods\LingChar')
TEX_REL = 'drc_dye/profiles/ling/animations/characters/ling/fx_sparks3x3_grid.png'
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


files, d = list_pck(str(PCK))
check('pck 解析', True, f'{len(files)} 条')

# 1) 新纹理源 + .import 在包内，ctex 产品经 remap 可达
check('新纹理源在包内', TEX_REL in files)
check('新纹理 .import 在包内', TEX_REL + '.import' in files)
imp = d[files[TEX_REL + '.import'][0]:sum(files[TEX_REL + '.import'][:2])].decode('utf-8')
import re
m = re.search(r'path="(res://\.godot/imported/[^"]+)"', imp)
prod = m.group(1).rsplit('/', 1)[-1] if m else ''
check('.import remap 可解析', bool(prod), prod)
check('ctex 产品在包内', f'.godot/imported/{prod}' in files)
check('旧 strip9 不在包内', not any('fx_spark_strip9' in k for k in files))

# 2) ling profile config：sparks.h_frames/v_frames=3、strip_texture 新路径
off, size = files[CFG_REL]
blob = d[off:off + size]
import zlib
if blob[:1] not in (b'{', b'['):
    blob = zlib.decompress(blob, 15, 100000000)
cfg = json.loads(blob)
fx = cfg.get('attack_fx', {})
sp = fx.get('sparks', {})
check('sparks.h_frames=3', sp.get('h_frames') == 3, repr(sp.get('h_frames')))
check('sparks.v_frames=3', sp.get('v_frames') == 3, repr(sp.get('v_frames')))
check('strip_texture 指向新纹理', sp.get('strip_texture', '').endswith('fx_sparks3x3_grid.png'),
      sp.get('strip_texture', ''))

# 3) dll 字符串核验（.NET 字面量在 #US 堆 = UTF-16；顺带查 UTF-8）
for dllp in (ROOT / 'bin/Release/net9.0/LingChar.dll', MODS / 'LingChar.dll'):
    b = dllp.read_bytes()
    for s in ('muzzle_gpos2', '非方形网格'):
        check(f'{dllp.name} 含 {s!r}({dllp.parent.parent.name})',
              s.encode('utf-16-le') in b or s.encode('utf-8') in b)

# 4) sha256：仓内产物 vs 安装实物
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
