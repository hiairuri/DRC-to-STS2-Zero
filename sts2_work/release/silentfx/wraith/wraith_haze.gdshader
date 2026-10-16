// SilentFX - wraith haze (Wraith Form, while the card is held).
// Fog creeping in from all four edges of the screen: a full-screen rect, clear in the middle.
// "intensity" brings it up, "reach" is how far in it comes (in screen heights). Its clock comes from code.
// Fully procedural. Edit and press F8 to reload.
shader_type canvas_item;
render_mode blend_mix, unshaded;

uniform float intensity = 0.0;
uniform float reach = 0.22;
uniform float clock = 0.0;
uniform float aspect = 1.78;     // width / height of the rect
uniform float seed = 0.0;
uniform vec3 col_deep : source_color = vec3(0.012, 0.070, 0.050);
uniform vec3 col_fog : source_color = vec3(0.16, 0.52, 0.36);
uniform vec3 col_light : source_color = vec3(0.62, 0.95, 0.70);

float hash21(vec2 p) {
	p = fract(p * vec2(123.34, 456.21));
	p += dot(p, p + 45.32);
	return fract(p.x * p.y);
}

float vnoise(vec2 p) {
	vec2 i = floor(p);
	vec2 f = fract(p);
	vec2 u = f * f * (3.0 - 2.0 * f);
	float a = hash21(i);
	float b = hash21(i + vec2(1.0, 0.0));
	float c = hash21(i + vec2(0.0, 1.0));
	float d = hash21(i + vec2(1.0, 1.0));
	return mix(mix(a, b, u.x), mix(c, d, u.x), u.y);
}

float fbm(vec2 p) {
	float v = 0.0;
	float amp = 0.5;
	for (int i = 0; i < 5; i++) {
		v += amp * vnoise(p);
		p = vec2(1.6 * p.x + 1.2 * p.y + 3.1, -1.2 * p.x + 1.6 * p.y + 1.7);
		amp *= 0.5;
	}
	return v;
}

void fragment() {
	if (intensity <= 0.003) {
		COLOR = vec4(0.0);
	} else {
		vec2 p = vec2(UV.x * aspect, UV.y);
		// How far it is to the nearest edge, in screen heights.
		float d = min(min(p.x, aspect - p.x), min(p.y, 1.0 - p.y));
		float t = clock;
		// Billows: the noise is dragged about by a slower noise, and all of it drifts.
		vec2 q = p * 2.6 + vec2(seed, seed * 0.37);
		float w = fbm(q * 0.7 + vec2(t * 0.11, -t * 0.07));
		float n = fbm(q + vec2((w - 0.5) * 2.2 + t * 0.06, (w - 0.5) * 1.6 - t * 0.10));
		float n2 = fbm(q * 2.3 + vec2(-t * 0.16, t * 0.09) + w);
		// It comes further in where the billows are, and its inner edge is all tongues.
		float far = reach * (0.35 + 1.30 * n);
		float fog = 1.0 - smoothstep(far * 0.15, far, d);
		fog *= 0.55 + 0.45 * n2;
		vec3 col = mix(col_deep, col_fog, smoothstep(0.25, 0.75, n));
		col = mix(col, col_light, smoothstep(0.62, 0.95, n2) * 0.45);
		COLOR = vec4(col, clamp(fog * intensity, 0.0, 1.0) * COLOR.a);
	}
}
