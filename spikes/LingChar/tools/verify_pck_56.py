#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""条目 56 包内核验：flash 锚定前移（R1）+ streak 渐亮包络（R2）+ flash
纹理后缘 ramp 拉宽（R1 硬边）+ R7 格式串（派生自 verify_pck_55.py，55 全部
断言继承）：
  1) config 新增断言：streak.streak_fade_in_s=0.05（条目 56 R2：α 包络头部
     加渐亮上升段，保持/落 0 段与 scale 生长不动）；
  2) 包内 fx_flash.png ①sha256 ≠ 旧版 79e40d58…（ramp 0.05→0.10 重生成，
     有区分度断言防回归）；②后缘 ramp 断言：中心行 col1..col16 alpha 严格
     递增（19.2px 渐入带机读证据）；③峰值 alpha ≈0.835（along 峰随 ramp
     拉宽移到 u=0.10，=0.9^1.1×0.9375，旧值 0.886 作废）；
  3) dll 字符串断言追加 'streak_fade_in_s'（C# Num 兜底键名）；
  4) spark 羽化断言（sha 区分度/alpha 梯度/帧界布局）原样保留；
  5) 其余断言（dir_rot_fix=+7.81/muzzle_hull v2 烘焙/silent 零影响/sha
     对账）与 55 一致。

复用 GDPC v3 解析（dir 偏移@0x20，file_base 0x70，config 段可能 zlib 压缩）。
用法: "C:/Program Files/Python310/python.exe" tools/verify_pck_56.py
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
# ---- 条目 53：dir_rot_fix 终值 +7.81（δ 定案注入，管线常量分支）----
# hook 链：七拍 tick1 sprrot=0/n2rot=0 → 节点链旋转=0 → δ=渲染管线常量；
# δ = γ_meas(23.08, 条目 51 定案) − (dirdeg−wr)(+15.27, 本条目日志恒等式)
#   = +7.81。与构建期 composed 打印（15.27+7.81=23.08）自洽。
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
check('不动 flash.amount=3/rand=0.33', fl.get('amount') == 3 and fl.get('lifetime_randomness') == 0.33,
      f"{fl.get('amount')}/{fl.get('lifetime_randomness')}")
check('flash.life=0.36（条目 55 勘误之二红线 A：0.15→0.36，压缩钳死根修）',
      approx(fl.get('life'), 0.36), repr(fl.get('life')))
# 红线 A 推导的机读断言：最短寿命片 l_min=life×(1−lr)=0.36×0.67=0.2412，
# 包络(envelope)在任意 k 下 =0.225k 与 l=0.36k 同比线性 → 压缩判据
# (fadeIn×foStart+fadeOut)>l 恒不触发 ⇔ life×(1−lr) > 0.225。
check('flash 压缩判据恒不触发（life×(1−0.33)=0.2412 > 包络 0.225 → 0.225 全 k 可达）',
      fl.get('life', 0) * (1 - fl.get('lifetime_randomness', 0)) > _vis + 1e-9,
      f"l_min={fl.get('life', 0) * (1 - fl.get('lifetime_randomness', 0)):.4f} vs envelope={_vis:.4f}")
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

# ============ 3b) 条目 55 勘误之二：sparks 内容轮廓羽化核验 ============
# ①sha 区分度：旧版"帧界羽化"零操作（逐字节=上轮 6b99f4fb…），新断言
#   包内 sha ≠ 旧版；②内容边缘 alpha 梯度：alpha∈[1,254] 过渡像素数
#   >阈值（内容轮廓 box blur 生效证据）；③帧界 alpha=0 改口径"布局
#   不变"（羽化作用于内容轮廓，帧界本就够不着）。
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
              'follower expired', 'follower recycled', 'dir_rot_fix',
              # 条目 55 新增：R1/R2 实机验收日志锚点
              'flash envelope fadeIn=', 'spark damp=', 'shift~',
              # 条目 56 新增：streak 渐亮包络 config 键（C# Num 兜底）
              'streak_fade_in_s'):
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
