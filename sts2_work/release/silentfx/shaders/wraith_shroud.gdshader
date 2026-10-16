// SilentFX - wraith shroud (Wraith Form's opening).
// The apparition's body: the great mass of mist that hangs over the battlefield while the screen is
// dark. Its heart is the middle of the quad: a broad rounded crown and shoulders above, drawn in
// below to a hem that is torn to tatters and thins away to nothing; mist keeps falling off it.
// Dark green in its depths, pale where it billows, acid green where it is thin (the rim light of
// the card art). It has no outline - the noise eats it away towards its rim. It gathers out of its
// heart, and the scream throws it outwards.
// Fully procedural; its clock comes from code (so slow motion slows it). Edit and press F8 to reload.
shader_type canvas_item;
render_mode blend_mix, unshaded;

uniform float clock = 0.0;       // seconds, driven from code
uniform float intensity = 0.0;   // overall fade, 0..1
uniform float rise = 1.0;        // 0..1: how far it has gathered (it swells out of its heart)
uniform float burst = 0.0;       // 0..1: the scream throws it outwards and tears it thin
uniform float layer = 0.0;       // 0 = the body, 1 = thin wisps in front of it (the middle stays clear)
uniform float seed = 0.0;
uniform float lean = 0.0;        // how far its hem is carried sideways (-1..1 of the quad's half width)

uniform vec3 col_dark : source_color = vec3(0.020, 0.070, 0.045);
uniform vec3 col_mid : source_color = vec3(0.26, 0.40, 0.33);
uniform vec3 col_pale : source_color = vec3(0.80, 0.93, 0.84);
uniform vec3 col_rim : source_color = vec3(0.42, 1.00, 0.34);

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
	float t = clock;

	// Its heart is the middle of the quad. Gathering, it swells out of there.
	vec2 p = (uv - 0.5) * 2.0;
	float gathered = clamp(rise, 0.0, 1.0);
	vec2 g = p / mix(0.34, 1.0, gathered);
	float v = g.y * 0.5 + 0.5;       // 0 = the top of its crown, 1 = the end of its hem

	// Above its heart a rounded crown (half an ellipse); below, drawn in a little towards the hem.
	// "edge" is 0 in its depths and 1 at its rim, all the way round.
	float taper = mix(1.0, 0.62, smoothstep(0.50, 1.0, v));
	float x = g.x - lean * v * v;
	float up = max(0.5 - v, 0.0) / 0.45;
	float edge = length(vec2(x / (0.95 * taper), up)) + smoothstep(0.93, 1.06, v) * 2.0;

	// The noise falls: mist pours off it. Gathering draws it in; the scream throws it outwards.
	vec2 away = uv - vec2(0.5, 0.45);
	float spread = burst * 1.6 - (1.0 - gathered) * 0.9;
	vec2 q = vec2(uv.x * 2.8 + seed, uv.y * 2.3 - t * 0.30) - away * spread;
	float w = fbm(q + vec2(0.0, -t * 0.12));
	float n = fbm(vec2(q.x + (w - 0.5) * 1.6, q.y + w * 0.8));
	float n2 = fbm(q * 2.3 + vec2(w * 1.3, -t * 0.40));

	// The hem: more and more torn, thinner and thinner.
	float low = smoothstep(0.50, 1.0, v);
	float d = 1.0 - smoothstep(0.10, 1.05, edge + (n - 0.5) * (1.25 + 0.90 * burst + 0.70 * low));
	d *= 0.40 + 1.20 * n2;
	d *= 1.0 - 0.55 * burst * (1.0 - n);
	d *= 1.0 - 0.62 * low;
	float a = smoothstep(0.04, 0.70, d);

	// The front layer leaves the middle clear, so that the fires and the hair stay readable.
	float hollow = mix(1.0, smoothstep(0.25, 0.90, edge + (n2 - 0.5) * 0.5), 0.9 * layer);
	a *= hollow;

	// Dark green in its depths, pale where it billows, acid green where it is thin.
	vec3 col = mix(col_dark, col_mid, smoothstep(0.25, 0.80, n2));
	col = mix(col, col_pale, smoothstep(0.55, 0.95, a) * smoothstep(0.45, 0.85, n) * 0.75);
	float thin = smoothstep(0.02, 0.25, d) * (1.0 - smoothstep(0.25, 0.80, d));
	col = mix(col, col_rim, clamp(thin * (0.45 + 0.50 * burst), 0.0, 1.0));
	col += col_rim * 0.06 * a;

	float alpha = max(a * mix(0.88, 0.50, layer), thin * 0.20 * hollow);
	float efade = smoothstep(0.0, 0.07, uv.x) * smoothstep(1.0, 0.93, uv.x)
		* smoothstep(0.0, 0.06, uv.y) * smoothstep(1.0, 0.96, uv.y);
	COLOR = vec4(col, clamp(alpha * efade * intensity, 0.0, 1.0));
}
