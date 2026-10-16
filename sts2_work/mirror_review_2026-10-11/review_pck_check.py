# -*- coding: utf-8 -*-
# 独立审查用 PCK 校验脚本（mirror-reviewer 自研实现，不复用编写者脚本逻辑）。
# 口径：GDPC v2/v3，头部字段按 Godot pck 格式独立解析；条目偏移统一加 file_base。
import struct, hashlib, sys, json

IO = sys.stdout

def u32(b, o): return struct.unpack_from('<I', b, o)[0]
def u64(b, o): return struct.unpack_from('<Q', b, o)[0]

def read_pck(path):
    blob = open(path, 'rb').read()
    magic = blob[0:4]
    assert magic == b'GDPC', path + ' magic=' + repr(magic)
    ver = u32(blob, 4)
    vmaj = u32(blob, 8)
    vpatch = u32(blob, 16)
    flags = u32(blob, 0x14)
    file_base = struct.unpack_from('<q', blob, 0x18)[0] if ver >= 2 else 0
    dir_off = struct.unpack_from('<q', blob, 0x20)[0] if ver >= 2 else 0x1C if ver == 1 else None
    entries = {}
    p = dir_off
    n = u32(blob, p); p += 4
    for _ in range(n):
        ln = u32(blob, p); p += 4
        nm = blob[p:p+ln].rstrip(b'\x00').decode('utf-8', 'replace'); p += ln
        off, size = struct.unpack_from('<qq', blob, p); p += 16
        md5 = blob[p:p+16]; p += 16
        if ver >= 2:
            flg = u32(blob, p); p += 4
            off = off + file_base if (flg & 2) and (flags & 2) else off
            # 若目录表带 FLAGS_DIR_ENCRYPTED 等不再处理（本项目无加密）
        entries[nm] = (off, size, md5.hex())
    meta = dict(version=ver, vmajor=vmaj, vpatch=vpatch, flags=flags,
                file_base=file_base, dir_off=dir_off, count=n)
    return meta, entries, blob

def sha(b): return hashlib.sha256(b).hexdigest()

MODS = 'D:/Program Files (x86)/Steam/steamapps/common/Slay the Spire 2/Mods'
# 2026-10-16：两 spike 已由旧树迁入 dragonraja 树，路径随之更正。
REPO = 'D:/项目/dragonraja/spikes/LingChar'
SRC_MIRROR = 'D:/项目/dragonraja/spikes/MirrorPupil/pcksrc/effects/mirror/'

meta_r, e_r, blob_r = read_pck(REPO + '/LingChar.pck')
meta_i, e_i, blob_i = read_pck(MODS + '/LingChar/LingChar.pck')
meta_s, e_s, blob_s = read_pck(MODS + '/LingSilentSkin/LingSilentSkin.pck')

IO.write('[1] pck 头: repo=%s\n    installed=%s\n    silent=%s\n' % (meta_r, meta_i, meta_s))
IO.write('[2] 条目数: repo=%d installed=%d silent=%d\n' % (len(e_r), len(e_i), len(e_s)))

# 仓内 vs 已装件：逐条目比对（offset/size/md5 三元组）
diffs = [k for k in set(e_r) | set(e_i) if e_r.get(k) != e_i.get(k)]
IO.write('[3] 仓内 vs 已装件 条目差异数: %d\n' % len(diffs))
for k in sorted(diffs)[:10]:
    IO.write('    !! %s: %s vs %s\n' % (k, e_r.get(k), e_i.get(k)))

# 去污染交集
inter = sorted(set(e_r) & set(e_s))
IO.write('[4] LingChar ∩ LingSilentSkin 交集: %d 条\n' % len(inter))
for k in inter:
    IO.write('    !! %s\n' % k)

# 包内 gdshader 与源逐字节比对（对 MirrorPupil 源 + LingChar/pcksrc 副本双基准）
names = ['ling_mirror_mix.gdshader', 'ling_mirror_add.gdshader',
         'ling_mirror_mul.gdshader', 'ling_mirror_sub.gdshader']
IO.write('[5] 包内 gdshader 校验（repo 包）:\n')
for n in names:
    key = 'res://effects/mirror/' + n
    if key not in e_r:
        key2 = 'effects/mirror/' + n
        key = key2 if key2 in e_r else key
    if key not in e_r:
        IO.write('    !! %s 不在包内\n' % n); continue
    off, size, md5h = e_r[key]
    blob = blob_r[off:off+size]
    src_mp = open(SRC_MIRROR + n, 'rb').read()
    src_lc = open(REPO + '/pcksrc/effects/mirror/' + n, 'rb').read()
    sig = b'srgb_to_linear' in blob and b'linear_to_srgb' in blob and b'0.04045' in blob and b'0.0031308' in blob and b'1.055' in blob
    old_sig = b'vec3 rgb = ambient_add + light_w * (inv + vec3(sheen));' in blob
    IO.write('    %-26s size=%-5d ==MirrorPupil源:%s ==pcksrc副本:%s 新公式特征:%s 旧公式残留:%s\n'
             % (n, size, sha(blob) == sha(src_mp), sha(blob) == sha(src_lc), sig, old_sig))

# 镜像路径清单
mir = sorted(k for k in e_r if 'mirror' in k.lower())
IO.write('[6] 镜像相关条目 %d 条:\n' % len(mir))
for k in mir:
    IO.write('    %s (%d B)\n' % (k, e_r[k][1]))
skin_words = ['silent/', 'drc_dye/profiles/ling/animations/characters/silent']
bad = [k for k in mir if any(w in k for w in skin_words)]
IO.write('[7] 镜像条目中含皮肤侧路径: %d 条 %s\n' % (len(bad), bad if bad else ''))

# SilentSkin sha 与尺寸
raw_s = open(MODS + '/LingSilentSkin/LingSilentSkin.pck', 'rb').read()
IO.write('[8] LingSilentSkin.pck: %d B sha=%s\n' % (len(raw_s), sha(raw_s)))
raw_rp = open(REPO + '/LingChar.pck', 'rb').read()
raw_ip = open(MODS + '/LingChar/LingChar.pck', 'rb').read()
IO.write('[9] LingChar.pck repo: %d B sha=%s\n' % (len(raw_rp), sha(raw_rp)))
IO.write('    LingChar.pck installed: %d B sha=%s\n' % (len(raw_ip), sha(raw_ip)))
raw_dll = open(REPO + '/bin/Release/net9.0/LingChar.dll', 'rb').read()
raw_dll_i = open(MODS + '/LingChar/LingChar.dll', 'rb').read()
IO.write('[10] dll bin: %d B sha=%s ; installed: %d B sha=%s ; 相等=%s\n'
         % (len(raw_dll), sha(raw_dll), len(raw_dll_i), sha(raw_dll_i), raw_dll == raw_dll_i))
