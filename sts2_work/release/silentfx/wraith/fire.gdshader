// SilentFX - wraith fire (Wraith Form): the fire of the card art itself, licked about. It stands on "foot"
// (where her fist is) and is thrown about more the higher above it.
shader_type canvas_item;
uniform float phase = 0.0;       // 0..TAU, one loop (whole multiples of it are used, so that it closes)
uniform vec2 foot = vec2(0.5, 1.0);
uniform float amount = 1.0;
uniform float seed = 0.0;
varying vec4 tint;
void vertex() {
	tint = COLOR;       // (in fragment() COLOR already has the texture in it)
}
void fragment() {
	vec2 uv = UV;
	float hgt = clamp((foot.y - uv.y) / foot.y, 0.0, 1.0);
	float w = hgt * hgt * amount;
	uv.x += w * (0.060 * sin(3.0 * phase + uv.y * 13.0 + seed) + 0.034 * sin(5.0 * phase - uv.y * 26.0 + 1.3 + seed * 2.0));
	uv.y += w * 0.040 * sin(2.0 * phase + uv.x * 9.0 + seed) + hgt * amount * 0.014 * sin(4.0 * phase + uv.y * 21.0);
	vec4 c = texture(TEXTURE, uv);
	c.rgb *= 1.0 + 0.16 * hgt * sin(7.0 * phase + uv.y * 6.0 + seed);
	COLOR = c * tint;
}
