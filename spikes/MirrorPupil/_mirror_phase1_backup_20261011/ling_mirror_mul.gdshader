// 龙卡镜像战士材质严格移植——正片叠底族（同 ling_mirror_mix.gdshader 头注的完整源链路，
// 仅 render_mode 换成 spine multiply 槽）。
shader_type canvas_item;
render_mode blend_mul;

uniform float reveal : hint_range(0.0, 1.0) = 0.0; // FS 坡道 t：0=全程反相（运行时真值）
uniform float light_w = 1.0;                       // TEXCOORD7.w（中性 1）
uniform vec3 ambient_add = vec3(0.0);              // TEXCOORD7.xyz（中性 0）
uniform float sheen = 0.0;                         // spec/env 平面合成（中性 0）

varying vec4 v_col;
void vertex() {
	v_col = COLOR;
}

void fragment() {
	vec4 base = texture(TEXTURE, UV);
	vec3 inv = mix(vec3(1.0) - base.rgb, vec3(0.0), reveal);
	vec3 rgb = ambient_add + light_w * (inv + vec3(sheen));
	float a = clamp(base.a * v_col.a, 0.0, 1.0);
	COLOR = vec4(rgb, a);
}
