# -*- coding: utf-8 -*-
# Phase 1 包内校验：① LingChar.pck ∩ LingSilentSkin.pck = ∅；② 包内 4 个 gdshader
# 与源文件逐字节一致且含 srgb_to_linear / 1.055 特征串。
# PCK v3 规则：flags@0x14 / file_base@0x18 / dir_off@0x20，条目偏移相对 file_base。
import struct, hashlib

def list_pck(path):
    d = open(path, 'rb').read()
    assert d[:4] == b'GDPC', 'not a pck: %s' % d[:4]
    flags, = struct.unpack_from('<I', d, 0x14)
    file_base, = struct.unpack_from('<q', d, 0x18)
    dir_off, = struct.unpack_from('<q', d, 0x20)
    p = dir_off
    count, = struct.unpack_from('<I', d, p)
    p += 4
    files = {}
    for _ in range(count):
        plen, = struct.unpack_from('<I', d, p)
        p += 4
        name = d[p:p + plen].rstrip(b'\0').decode('utf-8')
        p += plen
        foff, fsize = struct.unpack_from('<qq', d, p)
        p += 16 + 16 + 4  # offset + size + md5 + v3 flags
        real = foff + file_base if flags & 2 else foff
        files[name] = (real, fsize)
    return files, d

MODS = r"D:/Program Files (x86)/Steam/steamapps/common/Slay the Spire 2/mods"
lc_path = MODS + "/LingChar/LingChar.pck"
ss_path = MODS + "/LingSilentSkin/LingSilentSkin.pck"
lc_files, lc_blob = list_pck(lc_path)
ss_files, _ = list_pck(ss_path)

# ① 去污染：交集
inter = sorted(set(lc_files) & set(ss_files))
print("LingChar.pck 条目数:", len(lc_files), " LingSilentSkin.pck 条目数:", len(ss_files))
print("交集条目数:", len(inter))
for n in inter:
    print("  !!", n)

# ② 包内 gdshader 字节比对
SRC = r"D:/项目/dragonraja/spikes/MirrorPupil/pcksrc/effects/mirror/"
names = ["ling_mirror_mix.gdshader", "ling_mirror_add.gdshader",
         "ling_mirror_mul.gdshader", "ling_mirror_sub.gdshader"]
ok = True
for n in names:
    key = "effects/mirror/" + n
    if key not in lc_files:
        print("MISSING in pck:", key); ok = False; continue
    off, size = lc_files[key]
    blob = lc_blob[off:off + size]
    src = open(SRC + n, "rb").read()
    same = hashlib.sha256(blob).hexdigest() == hashlib.sha256(src).hexdigest()
    sig = (b"srgb_to_linear" in blob) and (b"1.055" in blob) and (b"linear_to_srgb" in blob)
    print("%-28s size=%d bytes_same=%s sig=%s" % (n, size, same, sig))
    ok = ok and same and sig

# 镜像相关条目路径抽查（无皮肤侧路径）
mirror_paths = sorted(n for n in lc_files if "mirror" in n.lower())
print("镜像相关条目", len(mirror_paths), "条（全部落在 mirror 路径）:")
for n in mirror_paths:
    print("   ", n)

print("RESULT:", "PASS" if (ok and not inter) else "FAIL")
