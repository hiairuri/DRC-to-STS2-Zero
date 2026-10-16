// SilentFX - wraith strip (Wraith Form): a loose strip of her gauze, drawn the way the card art draws her hair:
// flat colours - a dark underside, a mid tone, the pale of it, a lighter edge. UV.x along it, UV.y across.
shader_type canvas_item;
uniform vec3 col_dark : source_color = vec3(0.063, 0.141, 0.145);
uniform vec3 col_mid : source_color = vec3(0.353, 0.525, 0.510);
uniform vec3 col_pale : source_color = vec3(0.553, 0.757, 0.663);
uniform vec3 col_light : source_color = vec3(0.749, 0.902, 0.824);
varying vec4 tint;
float hash11(float x) {
	return fract(sin(x * 127.1) * 43758.5453);
}
void vertex() {
	tint = COLOR;
}
void fragment() {
	float v = UV.y;
	float seed = tint.r * 31.0;                     // (strips are told apart by a little tint)
	// the bands wander a little along the strip, in steps (it is cut paper, not a gradient)
	float step_x = floor(UV.x * 14.0 + seed);
	float j = (hash11(step_x) - 0.5) * 0.10;
	vec3 c = col_pale;
	if (v < 0.16 + j) c = col_dark;
	else if (v < 0.40 + j * 1.5) c = col_mid;
	else if (v > 0.84 - j) c = col_light;
	// frayed ends
	float ends = smoothstep(0.0, 0.03, UV.x) * (1.0 - smoothstep(0.96, 1.0, UV.x));
	COLOR = vec4(c, tint.a * ends);
}
