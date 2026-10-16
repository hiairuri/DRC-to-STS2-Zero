// SilentFX - wraith smoke (Intangible / Wraith Form).
// A column of dark green smoke rising around the character. The quad's bottom
// edge sits just below the feet. Fully procedural. Edit and press F8 to reload.
shader_type canvas_item;
render_mode blend_mix, unshaded;

uniform float intensity = 1.0;   // overall fade, 0..1
uniform float pulse = 0.0;       // flare when a hit passes through, 0..1
uniform float layer = 0.0;       // 0 = behind the character, 1 = thin wisps in front
uniform float seed = 0.0;
uniform float speed = 1.0;

uniform vec3 col_dark : source_color = vec3(0.03, 0.17, 0.10);
uniform vec3 col_mid : source_color = vec3(0.16, 0.62, 0.36);
uniform vec3 col_rim : source_color = vec3(0.66, 1.00, 0.74);

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
	vec2 uv = UV;
	float t = TIME * speed;
	float px = (uv.x - 0.5) * 2.0;   // -1..1 across
	float py = uv.y;                 // 0 at the top, 1 at the bottom

	// Noise scrolls upwards: the smoke rises.
	vec2 q = vec2(uv.x * 3.0 + seed, uv.y * 2.6 + t * 0.55);
	float w = fbm(q + vec2(0.0, t * 0.2));
	float n = fbm(vec2(q.x + (w - 0.5) * 1.6, q.y + w * 0.8));
	float n2 = fbm(q * 2.4 + vec2(w * 1.3, t * 0.5));

	// A column that is widest at the feet and narrows towards the top.
	float halfw = mix(0.30, 0.64, smoothstep(0.0, 1.0, py)) * (1.0 + 0.12 * pulse);
	float edge = abs(px) / halfw;                 // 0 in the middle, 1 at the rim
	float column = 1.0 - smoothstep(0.30, 1.0, edge + (n - 0.5) * 1.35);

	// Dissipates at the top; a little thinner right at the floor.
	float vfade = smoothstep(0.04, 0.58, py) * (1.0 - smoothstep(0.90, 1.0, py) * 0.5);
	float d = column * vfade * (0.22 + 1.30 * n);

	// The front layer leaves the middle clear so the character stays readable.
	float hollow = mix(1.0, smoothstep(0.25, 0.85, edge + (n2 - 0.5) * 0.5), 0.85 * layer);
	d *= hollow;

	float a = smoothstep(0.08, 0.66, d * (1.0 + 0.5 * pulse));
	vec3 col = mix(col_dark, col_mid, smoothstep(0.2, 0.9, n2));
	col = mix(col, col_rim, clamp(smoothstep(0.55, 1.0, a) * smoothstep(0.5, 0.85, n2) * 0.6 + pulse * 0.25 * a, 0.0, 1.0));

	// Lit from inside a little, so that it shows against a dark scene (it used to vanish in the jungle).
	col += col_rim * 0.22 * a;
	float alpha = a * intensity * mix(0.95, 0.62, layer);
	float efade = smoothstep(0.0, 0.08, uv.x) * smoothstep(1.0, 0.92, uv.x)
		* smoothstep(0.0, 0.06, uv.y) * smoothstep(1.0, 0.96, uv.y);

	COLOR = vec4(col, clamp(alpha * efade, 0.0, 1.0));
}
