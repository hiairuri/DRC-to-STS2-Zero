#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""条目 45 前置：在 45 版 pck 覆盖之前，抓取 44 版包内 silent profile
config.json 的 sha256，作为"silent 零影响"的比对锚点。

用法: "C:/Program Files/Python310/python.exe" tools/capture_44_silent_sha.py
"""
import hashlib
import json
import zlib
from pathlib import Path
import struct

ROOT = Path(__file__).resolve().parent.parent
PCK = ROOT / 'LingChar.pck'
SILENT_CFG_REL = 'drc_dye/profiles/ling/animations/characters/silent/config.json'
LING_CFG_REL = 'drc_dye/profiles/ling/animations/characters/ling/config.json'


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
        off += 16 + 16 + 4
        files[name] = (file_base + fo, size)
    return files, d


files, d = list_pck(str(PCK))
for rel in (SILENT_CFG_REL, LING_CFG_REL):
    off, size = files[rel]
    blob = d[off:off + size]
    if blob[:1] not in (b'{', b'['):
        blob = zlib.decompress(blob, 15, 100000000)
    h = hashlib.sha256(blob).hexdigest()
    print(f'{rel}')
    print(f'  bytes = {len(blob)}')
    print(f'  sha256 = {h}')
    cfg = json.loads(blob)
    if 'attack_fx' in cfg:
        print(f"  attack_fx keys = {sorted(cfg['attack_fx'].keys())}")
