// 龙卡镜像战士材质严格移植——加色族（同 ling_mirror_mix.gdshader 头注的完整源链路
// 与 2026-10-11 色彩空间裁定：端到端 sRGB 直通，shader 内成对补 sRGB 解码/编码），
// 仅 render_mode 换成 spine 加色槽。
shader_type canvas_item;
render_mode blend_add;

// sRGB OETF 标准实现（与 ling_mirror_mix.gdshader 同构；常量来自 sRGB 标准，
// 与 UE4 运行时捕获实码同构，01 调研 §2.4c）
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
	// ① 解码：图集是 sRGB 编码图像，本管线 texture() 不解码（端到端 sRGB 直通）
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
