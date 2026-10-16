// 龙卡镜像战士材质严格移植（2026-10-05 源码定案，替换此前的"白化+蓝 tint"猜测版）。
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
//   4. 运行时值：C 侧 MID 工厂（part_000018.c:19422 区）对每只 spine 组件无条件
//      SetScalarParameterValue("darkenParam", 组件+0x920)；该字段无人写=0.0 →
//      cap=0 → t=0 → 全程反相。（材质默认 1.0 的"反相扫除"只在 C 主动改字段时
//      出现，正常战斗链不发生。）
//
// 2D 直译（STS2 无动态光源/法线/立方体环境，spec/env 项中性=0）：
//   COLOR.rgb = ambient_add + light_w × (1 − base.rgb) × (1 − reveal)
//   COLOR.a   = base.a × v_col.a
// uniform 默认值=上列运行时真值；v_col.rgb 不乘（源 FS 不乘顶点色 rgb，
// 骨架槽位色本就全白）。
shader_type canvas_item;
render_mode blend_mix;

uniform float reveal : hint_range(0.0, 1.0) = 0.0; // FS 坡道 t：0=全程反相（运行时真值），
                                                   // 1=反相扫除（=源 darkenParam: 0→1 的扫除）
uniform float light_w = 1.0;                       // TEXCOORD7.w（体积透射光乘子，中性 1）
uniform vec3 ambient_add = vec3(0.0);              // TEXCOORD7.xyz（环境加色，中性 0）
uniform float sheen = 0.0;                         // spec/env 平面合成（2D 广告牌中性 0）

varying vec4 v_col;
void vertex() {
	v_col = COLOR;
}

void fragment() {
	vec4 base = texture(TEXTURE, UV);
	// v37：mix(1−albedo, 0, t)
	vec3 inv = mix(vec3(1.0) - base.rgb, vec3(0.0), reveal);
	vec3 rgb = ambient_add + light_w * (inv + vec3(sheen));
	// h41：clamp(albedo.a × COLOR0.a)
	float a = clamp(base.a * v_col.a, 0.0, 1.0);
	COLOR = vec4(rgb, a);
}
