// 龙卡镜像战士材质严格移植（2026-10-05 源码定案；2026-10-11 Phase 1 色彩空间修正）。
//
// 源链路（全部原文取值，无反解）：
//   1. fightunitfighter.lua:1001 _GetBaseInitMaterialCfgId —— IsMirrorUnit() 时
//      整套初始材质 = FightConsts.MIRROR_FIGHTER_MATERIAL_RES_CFGID(104200005)
//      → cmaterialrescfg strings[0] =
//        ParentMaterials/MaterialMasters/SpineMaterials/MySpineUnlitHitMaterial_Inst。
//   2. 材质本体（热更包 uasset 属性转储）：唯一标量参数 darkenParam（默认 1.0），
//      贴图 SpriteTexture/NormalTexture，材质函数 ColorDarkenFunc +
//      CameraDirectionVector/DepthFromWorldPosition/AngleBetweenVectors/SafeNormalize，
//      集合 DarkenMPC；BLEND_Translucent + TLM_VolumetricDirectional + TwoSided。
//      没有 MixColor/ScanLineOpacity（那是 M_UI_TEST 家族的参数，此前误借）。
//   3. 编译 FS（uexp 内嵌 20B 哈希 → shaders_index.csv → u066941 等 14 FS，
//      render_work/mirror_shaders/）：像素输出
//        v37   = mix(vec3(1)−albedo.rgb, 0, t)              —— 反相负片项
//        t     = clamp(slope·max(h34,0), 0, cap(darkenParam))（视角坡道）
//        out   = (spec + env + max(v37,0))·TEXCOORD7.w + TEXCOORD7.xyz
//        out.a = clamp(albedo.a × COLOR0.a, 0, 1)           —— COLOR0.rgb 不参与
//   4. 反相项 t 恒 0 的真因（2026-10-11 新定案，01 调研 §2.3）：DarkenMPC 资产默认
//      MaxDarkenAlpha=0.0（darkenmpc.uexp 属性转储实锤）⇒ slope=0 且 cap=0 ⇒
//      t 恒 0，与 darkenParam 取 0 还是 1 完全无关；旧的"组件 +0x920 无人写=0"
//      C 层假设不再必要（part_*.c 已删、无法复核，结论由资产事实接管）。
//
// 色彩空间裁定（2026-10-11，方案 03 §1.1，两条独立证据）：
//   ① 用户实测现输出 sRGB 均值 ≈0.35-0.39，与"sRGB 域直取反"的理论预测 (0.35,0.37,0.39)
//     逐通道吻合 ⇒ 本管线 texture() 返回 sRGB 编码原值（未解码），COLOR 输出也不再被
//     引擎二次编码，即端到端 sRGB 直通；② 同项目 holo/dissolve 族 pipeline_encode=0
//     （raw texture out）旁证。⇒ 必须在 shader 内成对补 ①sRGB 解码 与 ④sRGB 编码，
//     缺一即偏（去掉④爆白、去掉①更暗）。
//
// Phase 2a 灰加项（2026-10-11 晚 R2 取证）：手游镜像"饱和度≈0、亮度高"的真机制 =
// 镜像 FS 独有的三项单色加叠在弱色度负片上——①雾散射(TEXCOORD7.xyz) ②方向光 GGX
// 高光（板片法线恒朝相机⇒NoV≈NoH≈1⇒高光系数≈0.178 全画面均匀）③环境立方图×EnvBRDF；
// 普通战斗角色贴图直出（prog4/32/215 均无此三项），唯独镜像（UnlitHit 家族）有 ⇒
// 发白发灰是结构必然，PC 缺的是加项量级而非去饱和算子（19 份镜像 GLSL 全量 grep：
// 无输出去饱和）。本轮按 03 §2a 观感对齐路径为 ambient_add/sheen 设估算默认值（非
// 运行时实值，非真值），待 hook（H1+H2+H3+H5 一次会话）闭环后替换为实值；旧"禁止调
// ambient_add/light_w 凑亮度"禁令基于"战斗无雾"前提，R2 实证该前提未验证且雾是头号
// 嫌疑，禁令作废（light_w 本身仍保持中性 1.0）。估算依据与候选对照：05 文档 §2。
//
// 2D 直译（Phase 1：spec/env 中性=0，雾项中性 (1,0,0,0)）：
//   COLOR.rgb = linear_to_srgb( (mix(1−srgb_to_linear(base.rgb), 0, reveal) + sheen)
//                               × light_w + ambient_add )
//   COLOR.a   = base.a × v_col.a
// uniform 默认值：reveal=0/light_w=1 为运行时真值；ambient_add/sheen=Phase 2a 估算值
// （见上段），待 hook 闭环替换；v_col.rgb 不乘（源 FS 不乘顶点色 rgb，
// 骨架槽位色本就全白；一旦改乘 rgb，HitFlash 的 /ling/ 过滤会让镜像跟着本体爆白闪）。
shader_type canvas_item;
render_mode blend_mix;

// sRGB OETF 标准实现（IEC 61966-2-1；常量 12.92/0.04045/1.055/0.0031308/2.4 来自
// sRGB 标准，非猜测；与 UE4 运行时捕获实码同构，01 调研 §2.4c：13 份 GLSL 一致出现
// pow(InvGamma) + min(c*12.92, pow(max(c,0.00313067),1/2.4)*1.055−0.055)）
vec3 srgb_to_linear(vec3 c) {
	return mix(c / 12.92, pow((c + 0.055) / 1.055, vec3(2.4)), step(vec3(0.04045), c));
}
vec3 linear_to_srgb(vec3 c) {
	return mix(12.92 * c, 1.055 * pow(max(c, vec3(0.0)), vec3(1.0 / 2.4)) - 0.055, step(vec3(0.0031308), c));
}

uniform float reveal : hint_range(0.0, 1.0) = 0.0; // FS 坡道 t（雾扫除项/未确证）：恒 0
                                                   // （DarkenMPC.MaxDarkenAlpha=0.0 资产实锤）；禁止调参凑亮度
uniform float light_w = 1.0;                       // 雾项（TEXCOORD7.w=雾透射），无雾中性值 (1,0,0,0) 之 w=1；
                                                   // 雾不产生亮度；禁止调参凑亮度
uniform vec3 ambient_add = vec3(0.15);             // 雾散射（TEXCOORD7.xyz，线性域灰加）。Phase 2a 估算默认值（非运行时实值）：
                                                   // 与 sheen 合计 +0.20 对齐手游"全灰+高亮"观感；待 hook（H2/H3/H5）闭环后替换
uniform float sheen = 0.05;                        // spec+env 线性域单色加（GGX 高光+环境立方图×EnvBRDF）。Phase 2a 估算默认值
                                                   // （非运行时实值）；待 hook（H1：方向光色/SpecularScale）闭环后替换

varying vec4 v_col;
void vertex() {
	v_col = COLOR;
}

void fragment() {
	vec4 base = texture(TEXTURE, UV);
	// ① 解码：图集是 sRGB 编码图像，本管线 texture() 不解码（端到端 sRGB 直通，见头注裁定）
	vec3 L = srgb_to_linear(clamp(base.rgb, vec3(0.0), vec3(1.0)));
	// ② v37：mix(1−albedo, 0, t)，线性空间取反；t=reveal 恒 0（MaxDarkenAlpha=0.0 实锤）
	vec3 inv = mix(vec3(1.0) - L, vec3(0.0), clamp(reveal, 0.0, 1.0));
	// ③④ UE4 合成序：先线性域乘雾/加雾（Phase 2a 估算：sheen=0.05, light_w=1, ambient_add=0.15），
	//    再整体 sRGB 编码（01 调研 §2.4b:271-278）
	vec3 lin = (inv + vec3(sheen)) * light_w + ambient_add;
	vec3 outc = linear_to_srgb(max(lin, vec3(0.0)));
	// h41：clamp(albedo.a × COLOR0.a)（不动）
	float a = clamp(base.a * v_col.a, 0.0, 1.0);
	COLOR = vec4(outc, a);
}
