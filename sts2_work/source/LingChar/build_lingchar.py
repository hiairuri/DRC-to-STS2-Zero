#!/usr/bin/env python3
"""Build + install the LingChar character mod.

去污染架构（2026-10-08 用户定案：与皮肤 mod 不再相互污染）：
- LingChar.pck 与 LingSilentSkin.pck 共享路径 = 空集。所有需要的素材在构建期
  从皮肤源树（sts2_work/source/LingSilentSkin_battle/pcksrc，与已装皮肤 pck 同源）
  同步过来，落到 LingChar 自有路径；导入产物按 md5(新源路径) 改名，连
  .godot/imported 的产品名都不与皮肤 pck 重合。皮肤更新后重跑本脚本即可
  受控同步，运行期两个 mod 互不可见。
- 战斗骨架：ling/animations/characters/ling/（角色）/ …/mirror/（镜像）。
- 选人/商人/篝火：animations/character_select|merchant|rest_site/ling/ 下
  自有 tres + 重定向，场景以皮肤最新版为底重写引用。
- dye 框架已废案（条目 61，2026-10-16）：DyeFramework/溶解/闪红/入场/攻击/命中
  全部移除，影子源码存档见 ShadowSource_Archived.cs（只读、不编译），
  res://drc_dye/ 树不再并入本 pck。
- Icon/hand images: .import remap 指向 LingChar 自有 ctex 产品（图标/立绘）
  或游戏 pck 原生产品（手势/描边——游戏自有资产，非 mod 路径）。
- Loc tables ship at LingChar/localization/<lang>/characters.json (the game
  merges mod tables found at res://<modid>/localization/...).
"""
import shutil
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent
DR_ROOT = Path(r'D:\项目\dragonraja')
TPL = DR_ROOT / 'sts2_work/pck_silent_scenes'          # extracted silent templates
SKIN = DR_ROOT / 'sts2_work/source/LingSilentSkin_battle/pcksrc'  # built spine products
PCKSRC = ROOT / 'pcksrc'
GAME = Path(r'D:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2')
MODS = GAME / 'Mods' / 'LingChar'

SKEL_TRES_UID = 'uid://d0lingskel0001'


def new_uid(tag: str) -> str:
    # deterministic unique-looking uid (a-z0-9 only, valid Godot uid charset)
    return 'uid://d0lingchar' + tag


def surgery(src: Path, dst: Path, *, uid: str, pairs: dict[str, str]) -> None:
    text = src.read_text(encoding='utf-8')
    for old, new in pairs.items():
        assert old in text, f'{src.name}: pattern missing: {old!r}'
        text = text.replace(old, new)
    # header uid -> our own (last, so ext_resource uids we just wrote survive)
    import re
    text = re.sub(r'uid="uid://[a-z0-9]+"', f'uid="{uid}"', text, count=1)
    dst.parent.mkdir(parents=True, exist_ok=True)
    dst.write_text(text, encoding='utf-8')


def png_placeholder(path: Path) -> None:
    import struct, zlib
    def chunk(tag: bytes, data: bytes) -> bytes:
        c = struct.pack('>I', len(data)) + tag + data
        return c + struct.pack('>I', zlib.crc32(tag + data) & 0xffffffff)
    ihdr = struct.pack('>IIBBBBB', 1, 1, 8, 2, 0, 0, 0)
    idat = zlib.compress(b'\x00\x00\x00\x00')
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(b'\x89PNG\r\n\x1a\n' + chunk(b'IHDR', ihdr) + chunk(b'IDAT', idat) + chunk(b'IEND', b''))


def icon_import(dst: Path, silent_import: Path, ling_png: str) -> None:
    """Remap a copied .import onto the GAME pck's imported ctex (no uid).

    The remap path.* lines already point at silent's imported products (which
    exist in the game pck) - keep them verbatim, drop the uid (new resource,
    must not claim silent's uid), and point [deps] source_file at our png."""
    text = silent_import.read_text(encoding='utf-8')
    lines = []
    for line in text.splitlines():
        if line.startswith('uid='):
            continue  # drop uid: new resource, no uid clash with silent's
        # keep remap paths pointing at silent's ctex products (they exist in the game pck)
        lines.append(line)
    text = '\n'.join(lines) + '\n'
    # append deps section pointing source at our ling png
    if '[deps]' not in text:
        text += f'\n[deps]\n\nsource_file="res://{ling_png}"\n'
    dst.parent.mkdir(parents=True, exist_ok=True)
    dst.write_text(text, encoding='utf-8')


def prod_name_for(rel: str) -> str:
    """Deterministic import-product name for a LingChar-owned source path
    (md5 of the res-relative path). Proven by the mirror skeleton: product
    names inside mod pcks are opaque — the loader follows the .import remap,
    it never re-derives the name."""
    import hashlib
    base = rel.rsplit('/', 1)[-1]
    stem, dot, ext = base.rpartition('.')
    prod_ext = {'json': 'spjson', 'atlas': 'spatlas', 'png': 'ctex'}.get(ext, ext)
    return f'{stem}-{hashlib.md5(rel.encode()).hexdigest()}.{prod_ext}'


def relocate_asset(src_dir: Path, dst_rel_dir: str, names: list[str],
                   products_dir: Path | None = None) -> None:
    """Copy source files + rewritten .imports + renamed products to a
    LingChar-owned res:// path (mirror-skeleton pattern: uid stripped,
    remap -> md5(new path) product, product bytes copied verbatim)."""
    import re as _re
    if products_dir is None:
        products_dir = SKIN / '.godot/imported'
    dst_dir = PCKSRC / dst_rel_dir
    dst_dir.mkdir(parents=True, exist_ok=True)
    for name in names:
        rel = f'{dst_rel_dir}/{name}'
        prod = prod_name_for(rel)
        shutil.copy(src_dir / name, dst_dir / name)
        text = (src_dir / f'{name}.import').read_text(encoding='utf-8')
        lines = [l for l in text.splitlines() if not l.startswith('uid=')]
        text = '\n'.join(lines) + '\n'
        text = _re.sub(r'"res://\.godot/imported/[^"]+"',
                       f'"res://.godot/imported/{prod}"', text)
        text = _re.sub(r'source_file="res://[^"]+"',
                       f'source_file="res://{rel}"', text)
        (dst_dir / f'{name}.import').write_text(text, encoding='utf-8')
        # source product: whatever the original .import remapped to
        m = _re.search(r'path="res://\.godot/imported/([^"]+)"',
                       (src_dir / f'{name}.import').read_text(encoding='utf-8'))
        assert m, f'{name}: no product remap in source .import'
        shutil.copy(products_dir / m.group(1),
                    PCKSRC / '.godot/imported' / prod)


def relocate_import_only(skin_import: Path, dst_rel: str) -> None:
    """Ship ONLY a .import (no source bytes) at a LingChar-owned png path,
    remapped onto a renamed copy of the skin's ctex product. Source file
    itself is not needed in pcks - the remap fully replaces the load."""
    import re as _re
    prod = prod_name_for(dst_rel)
    orig = skin_import.read_text(encoding='utf-8')
    m = _re.search(r'path="res://\.godot/imported/([^"]+)"', orig)
    assert m, f'{skin_import}: no remap'
    shutil.copy(SKIN / '.godot/imported' / m.group(1),
                PCKSRC / '.godot/imported' / prod)
    lines = [l for l in orig.splitlines() if not l.startswith('uid=')]
    text = '\n'.join(lines) + '\n'
    text = _re.sub(r'"res://\.godot/imported/[^"]+"',
                   f'"res://.godot/imported/{prod}"', text)
    text = _re.sub(r'source_file="res://[^"]+"',
                   f'source_file="res://{dst_rel}"', text)
    dst = PCKSRC / (dst_rel + '.import')
    dst.parent.mkdir(parents=True, exist_ok=True)
    dst.write_text(text, encoding='utf-8')


def assemble() -> None:
    # 2026-10-10：Mimosa safe-delete 钩子会拦截对整棵 pcksrc 的批量删除
    # （SAFE_DELETE_BULK_CONFIRM_REQUIRED，非交互会话中确认机制无法落地）。
    # 改为无损方案：旧树整体改名保留（可随时手动删除），新树从零装配——
    # 装配确定性不变，打包只含本次产物，旧目录不参与打包。
    import time
    if PCKSRC.exists():
        trash = ROOT / f'pcksrc_old_{time.strftime("%Y%m%d_%H%M%S")}'
        PCKSRC.rename(trash)
        print('old pcksrc preserved as', trash.name, '(可手动删除)')
    (PCKSRC / '.godot/imported').mkdir(parents=True)

    # --- combat body scene: skeleton -> our ling_skel_data.tres -------------
    # the pair INCLUDES the ext_resource uid: a stale uid would win over the
    # rewritten path and load silent's skeleton when the skin mod is absent
    surgery(TPL / 'scenes/creature_visuals/silent.tscn',
            PCKSRC / 'scenes/creature_visuals/ling.tscn', uid=new_uid('0001'),
            pairs={'uid="uid://bqlhf04mk2866" path="res://animations/characters/silent/silent_skel_data.tres"':
                   'path="res://animations/characters/ling/ling_skel_data.tres"'})
    # --- sibling scenes: verbatim, only header uid differs ------------------
    for tpl, out, tag in [
        ('scenes/vfx/card_trail_silent.tscn', 'scenes/vfx/card_trail_ling.tscn', '0002'),
        ('scenes/combat/energy_counters/silent_energy_counter.tscn',
         'scenes/combat/energy_counters/ling_energy_counter.tscn', '0003'),
    ]:
        surgery(TPL / tpl, PCKSRC / out, uid=new_uid(tag), pairs={})
    # --- rest_site：以皮肤最新版为底（新校准 (-2,226)@0.608 + 重定向 tres 引用
    #     换 Ling 自有路径；ext_resource 的 uid 必须随路径一起剥——旧 uid 命中
    #     皮肤的 tres 时会压过改写后的 path）--------------------------
    surgery(SKIN / 'scenes/rest_site/characters/silent_rest_site.tscn',
            PCKSRC / 'scenes/rest_site/characters/ling_rest_site.tscn',
            uid=new_uid('0004'),
            pairs={'uid="uid://cusnivp587wn0" path="res://animations/rest_site/silent/rest_site_silent_skel_data.tres"':
                   'path="res://animations/rest_site/ling/rest_site_ling_skel_data.tres"'})
    # --- merchant：vanilla 模板 + tres 引用换 Ling 自有路径 -----------------
    surgery(TPL / 'scenes/merchant/characters/silent_merchant.tscn',
            PCKSRC / 'scenes/merchant/characters/ling_merchant.tscn',
            uid=new_uid('0005'),
            pairs={'uid="uid://d4li4q7d3n0nb" path="res://animations/merchant/silent/silent_merchant_skel_data.tres"':
                   'path="res://animations/merchant/ling/ling_merchant_skel_data.tres"'})
    # --- char_select：以皮肤最新版为底（entry_player 入场/add+mul 修复材质/
    #     新坐标 (1052,481)@1.0/粒子已剔除），引用全部换 Ling 自有路径 ------
    surgery(SKIN / 'scenes/screens/char_select/char_select_bg_silent.tscn',
            PCKSRC / 'scenes/screens/char_select/char_select_bg_ling.tscn',
            uid=new_uid('0006'),
            pairs={
                'uid="uid://catpspx0suysx" path="res://animations/character_select/silent/characterselect_silent_skel_data.tres"':
                    'path="res://animations/character_select/ling/characterselect_ling_skel_data.tres"',
                'uid="uid://bukbu81wkkgm1" path="res://animations/character_select/silent/character_select_silent_bg.png"':
                    'path="res://animations/character_select/ling/ling_bg.png"',
                'path="res://animations/character_select/silent/entry_player.gd"':
                    'path="res://animations/character_select/ling/entry_player.gd"',
                'path="res://animations/character_select/silent/additive_fix.gdshader"':
                    'path="res://animations/character_select/ling/additive_fix.gdshader"',
                'path="res://animations/character_select/silent/multiply_fix.gdshader"':
                    'path="res://animations/character_select/ling/multiply_fix.gdshader"',
                'path="res://animations/character_select/silent/dev_selfcheck.gd"':
                    'path="res://animations/character_select/ling/dev_selfcheck.gd"',
            })
    # --- transition material: texture refs stay silent's --------------------
    surgery(TPL / 'materials/transitions/silent_transition_mat.tres',
            PCKSRC / 'materials/transitions/ling_transition_mat.tres',
            uid=new_uid('0007'), pairs={})

    # --- skeleton data resource for OUR scene --------------------------------
    # 引用 LingChar 自有的骨架文件（res://ling/animations/characters/ling/，
    # 2026-10-08 去污染：不再指向与皮肤 mod 共享的 …/silent/ 路径）
    (PCKSRC / 'animations/characters/ling').mkdir(parents=True)
    (PCKSRC / 'animations/characters/ling/ling_skel_data.tres').write_text(
        f'''[gd_resource type="SpineSkeletonDataResource" load_steps=7 format=3 uid="{SKEL_TRES_UID}"]

[ext_resource type="SpineAtlasResource" path="res://ling/animations/characters/ling/ling.atlas" id="1_lingat"]
[ext_resource type="SpineSkeletonFileResource" path="res://ling/animations/characters/ling/ling.json" id="2_lingsk"]

[sub_resource type="SpineAnimationMix" id="SpineAnimationMix_hurt1"]
from = "hurt"
to = "hurt"

[sub_resource type="SpineAnimationMix" id="SpineAnimationMix_hurt2"]
from = "hurt"
to = "die"

[sub_resource type="SpineAnimationMix" id="SpineAnimationMix_idle1"]
from = "idle_loop"
to = "hurt"
mix = 0.02

[sub_resource type="SpineAnimationMix" id="SpineAnimationMix_cast1"]
from = "cast"
to = "idle_loop"
mix = 0.02

[resource]
atlas_res = ExtResource("1_lingat")
skeleton_file_res = ExtResource("2_lingsk")
default_mix = 0.05
animation_mixes = [SubResource("SpineAnimationMix_hurt1"), SubResource("SpineAnimationMix_hurt2"), SubResource("SpineAnimationMix_idle1"), SubResource("SpineAnimationMix_cast1")]
''', encoding='utf-8')

    # --- 战斗骨架：从皮肤源树同步到 Ling 自有路径（构建期受控同步，运行期零共享）---
    relocate_asset(SKIN / 'ling/animations/characters/silent',
                   'ling/animations/characters/ling',
                   ['ling.json', 'ling.atlas', 'Chr_ling001.png'])

    # --- 选人立绘骨架资产：pd_ling + 页纹理 + 修复 shader + 入场脚本 -----------
    #     同步自皮肤最新版（entry/addfix/mulfix 是皮肤 2026-10-07 更新的材质）。
    cs_src = SKIN / 'animations/character_select/silent'
    relocate_asset(cs_src, 'animations/character_select/ling',
                   ['pd_ling.atlas', 'pd_ling.json',
                    'spd_page1.png', 'spd_page2.png', 'spd_page3.png',
                    'pdchar_page1.png', 'pdchar_page2.png'])
    for script in ('additive_fix.gdshader', 'multiply_fix.gdshader',
                   'entry_player.gd', 'dev_selfcheck.gd'):
        shutil.copy(cs_src / script,
                    PCKSRC / 'animations/character_select/ling' / script)
    # 选人重定向 tres（uid 剥离：带皮肤的 uid 会命中皮肤的 tres）
    (PCKSRC / 'animations/character_select/ling/characterselect_ling_skel_data.tres').write_text(
        '''[gd_resource type="SpineSkeletonDataResource" load_steps=3 format=3]

[ext_resource type="SpineAtlasResource" path="res://animations/character_select/ling/pd_ling.atlas" id="1"]
[ext_resource type="SpineSkeletonFileResource" path="res://animations/character_select/ling/pd_ling.json" id="2"]

[resource]
atlas_res = ExtResource("1")
skeleton_file_res = ExtResource("2")
''', encoding='utf-8')
    # 选人背景图（fashion bg）：只出 .import 重映射（源 png 不需要进包）
    relocate_import_only(cs_src / 'character_select_silent_bg.png.import',
                         'animations/character_select/ling/ling_bg.png')
    # 选人竖版立绘（Ling 角色卡）：Ling 自有路径，Ling.cs 指向这里
    relocate_import_only(SKIN / 'images/packed/character_select/char_select_silent.png.import',
                         'images/packed/character_select/char_select_ling.png')

    # --- 商人/篝火重定向 tres：Ling 自有路径，指向 Ling 自有战斗骨架 ----------
    for rel, tag in (('animations/merchant/ling/ling_merchant_skel_data.tres', 'm002'),
                     ('animations/rest_site/ling/rest_site_ling_skel_data.tres', 'm003')):
        (PCKSRC / rel).parent.mkdir(parents=True, exist_ok=True)
        (PCKSRC / rel).write_text(
            f'''[gd_resource type="SpineSkeletonDataResource" load_steps=3 format=3 uid="{new_uid(tag)}"]

[ext_resource type="SpineAtlasResource" path="res://ling/animations/characters/ling/ling.atlas" id="1"]
[ext_resource type="SpineSkeletonFileResource" path="res://ling/animations/characters/ling/ling.json" id="2"]

[resource]
atlas_res = ExtResource("1")
skeleton_file_res = ExtResource("2")
default_mix = 0.05
''', encoding='utf-8')

    # --- 角色图标场景（IconPath）：vanilla silent_icon 的克隆，指向 Ling 自有图标 -
    (PCKSRC / 'scenes/ui/character_icons').mkdir(parents=True, exist_ok=True)
    (PCKSRC / 'scenes/ui/character_icons/ling_icon.tscn').write_text(
        '''[gd_scene load_steps=2 format=3]

[ext_resource type="Texture2D" path="res://images/ui/top_panel/character_icon_ling.png" id="1"]

[node name="CharacterIcon" type="TextureRect"]
anchors_preset = 15
anchor_right = 1.0
anchor_bottom = 1.0
grow_horizontal = 2
grow_vertical = 2
mouse_filter = 2
texture = ExtResource("1")
expand_mode = 1
stretch_mode = 5
''', encoding='utf-8')

    # --- icon + hand images --------------------------------------------------
    # Ling 图标 = 皮肤最新版 linglogo（同步到 Ling 自有产品名）；
    # 描边/手势 = 游戏 pck 原生产品（游戏自有资产，非 mod 路径，零冲突）。
    relocate_import_only(SKIN / 'images/ui/top_panel/character_icon_silent.png.import',
                         'images/ui/top_panel/character_icon_ling.png')
    icon_import(PCKSRC / 'images/ui/top_panel/character_icon_ling_outline.png.import',
                TPL / 'images/ui/top_panel/character_icon_silent_outline.png.import',
                'images/ui/top_panel/character_icon_ling_outline.png')
    for kind in ('point', 'rock', 'paper', 'scissors'):
        icon_import(PCKSRC / f'images/ui/hands/multiplayer_hand_ling_{kind}.png.import',
                    TPL / f'images/ui/hands/multiplayer_hand_silent_{kind}.png.import',
                    f'images/ui/hands/multiplayer_hand_ling_{kind}.png')
        png_placeholder(PCKSRC / f'images/ui/hands/multiplayer_hand_ling_{kind}.png')
    for name in ('character_icon_ling.png', 'character_icon_ling_outline.png'):
        png_placeholder(PCKSRC / 'images/ui/top_panel' / name)

    # --- 遗物图标：启灵卡两张（基础=未觉醒，先古=觉醒）----------------------
    # Godot 头less 导入出 .ctex + .import（原版 png 无 import 无 loader），
    # 产品名=md5(源路径不带 res://)；再以 AtlasTexture .tres 覆盖原版图集
    # 精灵路径（mod pck 后挂载=覆盖同路径）。
    import hashlib, re as _re3
    # md5 here reproduces GODOT'S OWN import product naming (.godot/imported/
    # <file>-<md5(source path)>.ctex) — it is a lookup key, not a security
    # primitive; changing it would break the engine's import resolution.
    icon_root = PCKSRC / 'images/relics'
    icon_root.mkdir(parents=True, exist_ok=True)
    GODOT = Path('D:/Tools/Godot/Godot_v4.7.2-stable_mono_win64_console.exe')
    scratch = ROOT / 'icon_import_scratch'
    if scratch.exists():
        shutil.rmtree(scratch)
    src_dir = scratch / 'images/relics'
    src_dir.mkdir(parents=True)
    (scratch / 'project.godot').write_text(
        'config_version=5\n\n[application]\n\nconfig/name="icon_import"\n', encoding='utf-8')
    icons = {}
    for tag, art in (('mirror_pupil', '启灵卡_镜瞳_未觉醒_大图'),
                     ('mirror_pupil_ancient', '启灵卡_镜瞳_觉醒_大图')):
        from PIL import Image
        im = Image.open(DR_ROOT / f'render_work/lingUI/{art}.png')
        w, h = im.size
        crop = im.crop((0, (h - w) // 2, w, (h - w) // 2 + w)).resize((512, 512), Image.LANCZOS)
        dst = src_dir / f'{tag}.png'
        crop.save(dst, optimize=True)
        icons[tag] = dst
    if GODOT.exists():
        r = subprocess.run([str(GODOT), '--headless', '--path', str(scratch), '--import'],
                           capture_output=True, text=True, timeout=300)
        imp = scratch / '.godot/imported'
        (PCKSRC / '.godot/imported').mkdir(parents=True, exist_ok=True)
        for tag in icons:
            src_rel = f'images/relics/{tag}.png'
            imp_text = (scratch / 'images/relics' / f'{tag}.png.import').read_text(encoding='utf-8')
            # the product name is whatever GODOT generated — read it from the
            # remap instead of reproducing its hash formula
            m = _re3.search(r'path="(res://\.godot/imported/[^"]+)"', imp_text)
            if not m:
                raise FileNotFoundError(f'no product path in import for {tag}')
            prod_name = m.group(1).rsplit('/', 1)[-1]
            prod = imp / prod_name
            if not prod.exists():
                raise FileNotFoundError(f'import product missing: {prod}')
            shutil.copy(prod, PCKSRC / '.godot/imported' / prod_name)
            shutil.copy(icons[tag], PCKSRC / f'{src_rel}')
            lines = [l for l in imp_text.splitlines() if not l.startswith('uid=')]
            (PCKSRC / f'{src_rel}.import').write_text('\n'.join(lines) + '\n', encoding='utf-8')
            print('icon imported:', tag, '->', prod_name)
    else:
        print('WARN: Godot not found, relic icons will be missing textures')

    # 图集精灵覆盖（同路径覆盖原版）：relic + outline 两个图集、基础+先古
    tres_tpl = '''[gd_resource type="AtlasTexture" load_steps=2 format=3]

[ext_resource type="Texture2D" path="res://images/relics/{name}.png" id="1"]

[resource]
atlas = ExtResource("1")
region = Rect2(0, 0, 512, 512)
'''
    for atlas, name in (('relic_atlas.sprites', 'mirror_pupil'),
                        ('relic_atlas.sprites', 'mirror_pupil_ancient'),
                        ('relic_outline_atlas.sprites', 'mirror_pupil'),
                        ('relic_outline_atlas.sprites', 'mirror_pupil_ancient')):
        dst = PCKSRC / f'images/atlases/{atlas}/{name}.tres'
        dst.parent.mkdir(parents=True, exist_ok=True)
        dst.write_text(tres_tpl.format(name=name), encoding='utf-8')

    # --- loc: modid-prefixed (the game merges res://<modid>/localization/...) -
    loc_dst = PCKSRC / 'LingChar/localization'
    if loc_dst.exists():
        shutil.rmtree(loc_dst)
    shutil.copytree(ROOT / 'loc/localization', loc_dst)
    # 言灵·镜瞳遗物的三张表（relics/powers/monsters，zhs+eng）并入 mod loc
    # MirrorPupil spike 2026-10-16 已随 LingChar 一同迁入 dragonraja 树，
    # 故改为同级相对引用（原树外绝对路径已撤销）。
    spike_loc = ROOT.parent / 'MirrorPupil/loc/localization'
    for lang in ('eng', 'zhs'):
        for table in ('relics.json', 'powers.json', 'monsters.json'):
            shutil.copy(spike_loc / lang / table, loc_dst / lang / table)

    # --- MirrorPupil 镜像材质资源（反相负片四混合族）--------------------------
    mirror_fx = ROOT.parent / 'MirrorPupil/pcksrc/effects/mirror'
    if mirror_fx.is_dir():
        shutil.copytree(mirror_fx, PCKSRC / 'effects/mirror',
                        dirs_exist_ok=True)

    # --- 镜像召唤物骨架：独立路径（res://ling/animations/characters/mirror/）
    # 独立路径无 dye profile（dye 框架已废案，条目 61）= vanilla 材质保留，
    # MirrorMaterial.Apply（反相负片，2026-10-05 源码严格定案）接管镜像观感
    import hashlib
    import re as _re
    mdir = PCKSRC / 'ling/animations/characters/mirror'
    mdir.mkdir(parents=True, exist_ok=True)
    products = {}
    for base, ext in (('ling.json', 'spjson'), ('ling.atlas', 'spatlas'),
                      ('Chr_ling001.png', 'ctex')):
        src_rel = f'ling/animations/characters/mirror/{base}'
        prod = f'{base}-{hashlib.md5(src_rel.encode()).hexdigest()}.{ext}'
        products[base] = prod
        shutil.copy(SKIN / 'ling/animations/characters/silent' / base, mdir / base)
        # .import：remap 指向新 md5 产物名，uid 剔除（与 silent 路径同 uid 会撞）；
        # 裸引号正则同时覆盖 path= 与 dest_files=["..."]（旧正则漏 dest_files
        # 会残留对皮肤产品名的引用）
        text = (SKIN / 'ling/animations/characters/silent' / f'{base}.import').read_text(encoding='utf-8')
        lines = [l for l in text.splitlines() if not l.startswith('uid=')]
        text = '\n'.join(lines) + '\n'
        text = _re.sub(r'"res://\.godot/imported/[^"]+"',
                       f'"res://.godot/imported/{prod}"',
                       text)
        (mdir / f'{base}.import').write_text(text, encoding='utf-8')
    (PCKSRC / '.godot/imported' / products['ling.json']).write_bytes(
        (SKIN / 'ling/animations/characters/silent/ling.json').read_bytes())
    for base in ('ling.atlas', 'Chr_ling001.png'):
        src_prod = SKIN / '.godot/imported' / (
            f'ling.atlas-a45053df6901f847fc14b06c889f87ab.spatlas' if base == 'ling.atlas'
            else 'Chr_ling001.png-72aa7f3f768c218e0a6ca07e91af047e.ctex')
        shutil.copy(src_prod, PCKSRC / '.godot/imported' / products[base])
    (PCKSRC / 'ling/animations/characters/mirror/ling_skel_data.tres').write_text(
        f'''[gd_resource type="SpineSkeletonDataResource" load_steps=7 format=3 uid="{new_uid('m001')}"]

[ext_resource type="SpineAtlasResource" path="res://ling/animations/characters/mirror/ling.atlas" id="1_matl"]
[ext_resource type="SpineSkeletonFileResource" path="res://ling/animations/characters/mirror/ling.json" id="2_mskl"]

[sub_resource type="SpineAnimationMix" id="SpineAnimationMix_mhurt1"]
from = "hurt"
to = "hurt"

[sub_resource type="SpineAnimationMix" id="SpineAnimationMix_mhurt2"]
from = "hurt"
to = "die"

[sub_resource type="SpineAnimationMix" id="SpineAnimationMix_midle1"]
from = "idle_loop"
to = "hurt"
mix = 0.02

[resource]
atlas_res = ExtResource("1_matl")
skeleton_file_res = ExtResource("2_mskl")
default_mix = 0.05
animation_mixes = [SubResource("SpineAnimationMix_mhurt1"), SubResource("SpineAnimationMix_mhurt2"), SubResource("SpineAnimationMix_midle1")]
''', encoding='utf-8')

    # --- 镜像召唤物战斗场景：用**奥斯提（真宠物）的场景模板**整体拷贝再改
    #     （用户指令：完整拷贝防遗漏——宠物血条/状态/站位布局都按 Bounds 调校，
    #     角色场景的 Bounds 配置与宠物不同）。剥掉奥斯提专属火焰 VFX 节点与
    #     相关贴图/shader ext_resource，骨架换镜像 ling_skel_data。--------
    osty_lines = (TPL / 'scenes/creature_visuals/osty.tscn').read_text(encoding='utf-8').splitlines()
    flame_kw = ('vfx_stepped_shader_fire_dark', 'triangle_noise_tile', 'zigzag_fire_distortion',
                'osty_fire_bottom_mask', 'basic_fire_noise', 'osty_fire_noise_mask', 'osty_fire_shape')
    out = []
    skipping_block = False
    for line in osty_lines:
        if line.startswith('[ext_resource') and any(k in line for k in flame_kw):
            continue  # 火焰特效的 shader/贴图引用
        if line.startswith('[sub_resource type="ShaderMaterial"'):
            skipping_block = True  # 火焰材质 sub_resource
            continue
        if line.startswith('[node name="Flame"') or line.startswith('[node name="SteppedFireMix_dark"'):
            skipping_block = True  # 火焰节点块
            continue
        if skipping_block:
            if line.startswith('['):  # 块结束（下一个资源/节点声明）
                skipping_block = False
            else:
                continue
        out.append(line)
    text = '\n'.join(out) + '\n'
    text = text.replace('load_steps=11', 'load_steps=3')
    text = text.replace('uid="uid://w061e4iwxkil" path="res://animations/monsters/osty/osty_skel_data.tres"',
                        'path="res://ling/animations/characters/mirror/ling_skel_data.tres"')
    import re as _re2
    text = _re2.sub(r'uid="uid://[a-z0-9]+"', f'uid="{new_uid("0008")}"', text, count=1)
    # --- 镜像 1:1（2026-10-05 用户：镜像大小改到和角色 1:1）------------------
    # 场景换的是 ling 骨架（与角色 ling.tscn 同一骨架），Visuals 校准直接照抄
    # ling.tscn：position (16,-21) / scale 0.29。骨架 JSON bounds 的脚线换算
    # 1.906×0.29 随之进 dll 侧 PlaceMirrorLikeOsty 的 mirrorFeetOffset。
    assert '[node name="Visuals"' in text
    text = _re2.sub(r'(\[node name="Visuals"[^\[]*?)position = Vector2\([^)]*\)',
                    r'\1position = Vector2(16, -21)', text, count=1)
    text = _re2.sub(r'(\[node name="Visuals"[^\[]*?)scale = Vector2\([^)]*\)',
                    r'\1scale = Vector2(0.29, 0.29)', text, count=1)
    dst = PCKSRC / 'scenes/creature_visuals/mirror_image.tscn'
    dst.parent.mkdir(parents=True, exist_ok=True)
    dst.write_text(text, encoding='utf-8')
    assert 'osty_skel_data' not in text and 'Flame' not in text, 'osty surgery incomplete'
    assert 'position = Vector2(16, -21)' in text and 'scale = Vector2(0.29, 0.29)' in text, 'mirror 1:1 override missing'


def fix_spine_baked_paths() -> None:
    """条目 62（2026-10-16）：皮肤 mod 退役后的运行时自持修复。

    病根：从皮肤树原样复制的 SpineAtlasResource 产品（.spatlas）在导入时把
    「atlas 自路径 / 贴图页绝对路径」烘焙进了二进制（皮肤树里资产位于
    animations/*/silent/）。旧架构运行期靠 LingSilentSkin.pck 挂载同路径
    资产兜底；皮肤退役后这些路径全部失效 → spine 拿到空纹理 → 选人界面
    原生崩溃（sentry native fatal，GodotSpineTextureLoader::load）。

    修复：扫描全部 .spatlas/.spjson 产品内烘焙的 /animations/.../silent/
    绝对路径，在 pcksrc 对应原路径写 remap 存根（.import，不含源文件），
    指向 ling/ 同名资产已被 relocate 重命名的现有产品 —— 零资产复制、
    零产品改动、全部 relocate 家族（选人/战斗/镜像/商人/篝火）自动覆盖。
    """
    import re as _re3
    imported = PCKSRC / '.godot/imported'
    # 1) 现有 remap 表：资产路径(无 .import 后缀) -> (importer, type, product)
    remaps = {}
    for imp in PCKSRC.rglob('*.import'):
        text = imp.read_text(encoding='utf-8', errors='replace')
        m_p = _re3.search(r'path="(res://\.godot/imported/[^"]+)"', text)
        m_i = _re3.search(r'importer="([^"]+)"', text)
        m_t = _re3.search(r'type="([^"]+)"', text)
        if m_p and m_i and m_t:
            remaps[str(imp.relative_to(PCKSRC)).replace('\\', '/')[:-len('.import')]] = \
                (m_i.group(1), m_t.group(1), m_p.group(1))
    # 2) 逐产品扫描烘焙的完整自路径——正则必须保留「ling/」等可选前缀
    #    （条目 62 v2：战斗产品烘焙的是带前缀混合路径 ling/animations/characters/silent/，
    #      v1 正则从 /animations 起截取丢失前缀，导致战斗链存根写错位置）
    baked = set()
    for prod in imported.glob('*.spatlas'):
        blob = prod.read_bytes()
        for m in _re3.finditer(rb'[\w\-./]*animations/[A-Za-z0-9_/\-.]+\.(?:atlas|png)', blob):
            baked.add(m.group(0).decode('ascii', 'ignore').lstrip('/'))
    # 3) 逐条写 remap 存根：silent 原路径 -> ling 同名资产的现有产品
    #    3a) atlas 自路径存根；3b) 页纹理存根（运行时用「烘焙自路径目录+页名」拼页路径）
    written = 0

    def _write_stub(asset_path: str) -> bool:
        nonlocal written
        stub = PCKSRC / (asset_path + '.import')
        if stub.exists():
            return False
        candidates = [asset_path.replace('/silent/', '/ling/'),
                      'ling/' + asset_path.replace('/silent/', '/ling/')]
        ling = next((c for c in candidates if c in remaps), None)
        if ling is None:
            DevWarn(f'baked path 无 ling 对应，跳过: {asset_path}')
            return False
        importer, typ, product = remaps[ling]
        stub.parent.mkdir(parents=True, exist_ok=True)
        stub.write_text(
            '[remap]\n\n'
            f'importer="{importer}"\n'
            f'type="{typ}"\n'
            f'path="{product}"\n',
            encoding='utf-8')
        written += 1
        return True

    for p in sorted(baked):
        _write_stub(p)
        # 3b) 页纹理：ling 侧 atlas 源文本解析页名（页名行 = 紧跟 size: 的行）
        candidates = [p.replace('/silent/', '/ling/'),
                      'ling/' + p.replace('/silent/', '/ling/')]
        ling_atlas = next((c for c in candidates if c in remaps and c.endswith('.atlas')), None)
        if ling_atlas is None:
            continue
        atlas_src = PCKSRC / ling_atlas
        if not atlas_src.exists():
            continue
        lines = atlas_src.read_text(encoding='utf-8', errors='replace').splitlines()
        silent_dir = p.rsplit('/', 1)[0]
        for i, ln in enumerate(lines[:-1]):
            if lines[i + 1].startswith('size:') and ln.strip() and not ln.startswith(' '):
                _write_stub(silent_dir + '/' + ln.strip())
    print(f'spine baked-path stubs: scanned={len(baked)} written={written}')


def DevWarn(msg: str) -> None:
    print('WARN:', msg)


def pack() -> None:
    sys.path.insert(0, str(DR_ROOT / 'tools'))
    import pck_writer
    files = []
    for p in sorted(PCKSRC.rglob('*')):
        if p.is_file():
            files.append((p.relative_to(PCKSRC).as_posix(), p))
    n, size = pck_writer.build_pck(files, ROOT / 'LingChar.pck')
    print(f'packed {n} files, {size} bytes')
    for rel, _ in files:
        print('  ', rel)


def build_dll() -> None:
    r = subprocess.run(['dotnet', 'build', str(ROOT / 'LingChar.csproj'), '-c', 'Release', '-v', 'q', '--nologo'],
                       capture_output=True, text=True)
    print(r.stdout[-800:], r.stderr[-800:])
    assert r.returncode == 0, 'dotnet build failed'


def install() -> None:
    MODS.mkdir(parents=True, exist_ok=True)
    shutil.copy(ROOT / 'mod/LingChar.json', MODS / 'LingChar.json')
    shutil.copy(ROOT / 'bin/Release/net9.0/LingChar.dll', MODS / 'LingChar.dll')
    shutil.copy(ROOT / 'LingChar.pck', MODS / 'LingChar.pck')
    print('installed to', MODS)
    for f in sorted(MODS.iterdir()):
        print('  ', f.name, f.stat().st_size)


if __name__ == '__main__':
    assemble()
    fix_spine_baked_paths()
    build_dll()
    pack()
    install()
