// SilentFX - wraith flame (Wraith Form's opening).
// One tongue of acid-green fire: a teardrop standing on a point near the bottom of the quad, its
// tip licked about by rising noise, white-green at its heart. Light only (it is added to what is
// under it), no outline. "power" lights it, feeds it and puts it out.
// Fully procedural; its clock comes from code. Edit and press F8 to reload.
shader_type canvas_item;
render_mode blend_add, unshaded;

uniform float clock = 0.0;       // seconds, driven from code
uniform float power = 1.0;       // 0 = out, 1 = burning, up to 1.6 = flaring
uniform float seed = 0.0;
uniform float lean = 0.0;        // how far its tip is carried sideways (-1..1 of the quad's half width)

uniform vec3 col_core : source_color = vec3(0.90, 1.00, 0.78);
uniform vec3 col_mid : source_color = vec3(0.40, 1.00, 0.22);
uniform vec3 col_edge : source_color = vec3(0.04, 0.50, 0.12);

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
	for (int i = 0; i < 4; i++) {
		v += amp * vnoise(p);
		p = vec2(1.6 * p.x + 1.2 * p.y + 3.1, -1.2 * p.x + 1.6 * p.y + 1.7);
		amp *= 0.5;
	}
	return v;
}

void fragment() {
	vec2 uv = UV;
	float px = (uv.x - 0.5) * 2.0;

	// It stands on a point at 0.88 of the quad's height.
	float h = clamp((0.88 - uv.y) / 0.88, 0.0, 1.0);
	float burn = clamp(power, 0.0, 1.6);
	float top = clamp(0.62 * burn, 0.05, 1.0);      // how high it stands: flaring, it fills the quad
	float hh = h / top;

	// The noise scrolls upwards: the fire rises.
	float t = clock * 2.6;
	float n = fbm(vec2(px * 2.0 + seed, uv.y * 3.2 + t));
	float n2 = fbm(vec2(px * 4.2 - seed * 1.7, uv.y * 6.5 + t * 1.8));

	// A teardrop: round where it stands, drawn out to a tip that the noise throws about.
	float halfw = 0.50 * sqrt(clamp(hh * 5.0, 0.0, 1.0)) * pow(clamp(1.0 - hh, 0.0, 1.0), 0.85);
	halfw *= 0.72 + 0.56 * n2;
	float x = px - lean * hh * hh * 0.6 + (n - 0.5) * 0.70 * hh;
	float v = 1.0 - smoothstep(0.0, 1.0, abs(x) / max(halfw, 0.0005));
	v *= 0.60 + 0.80 * n2;
	v *= smoothstep(0.0, 0.03, halfw);

	float core = smoothstep(0.50, 0.95, v);
	float body = smoothstep(0.05, 0.60, v);
	vec3 col = mix(col_edge, col_mid, body);
	col = mix(col, col_core, core);

	// A soft light about its foot.
	vec2 g = vec2(px, (uv.y - 0.72) * 2.6);
	float halo = exp(-dot(g, g) * 2.6) * 0.38 * min(burn, 1.2);

	float lit = clamp(body * min(burn * 1.5, 1.0), 0.0, 1.0);
	vec3 shine = col * lit * (1.0 + 0.35 * core) + col_mid * halo;
	float efade = smoothstep(0.0, 0.08, uv.x) * smoothstep(1.0, 0.92, uv.x)
		* smoothstep(0.0, 0.08, uv.y) * smoothstep(1.0, 0.94, uv.y);
	COLOR = vec4(shine * efade, 1.0);
}
