// SilentFX - malaise mist (Malaise): the enemy's strength as red mist, drawn out of it by the beads.
// Like "shadow_cloud" there is no shape in it: it is given a handful of soft puffs - where the mist is, how far it
// reaches, how thick, how old, how much of it is seen - and melts them into one body whose rim the noise eats into
// wisps. Unlike it, it is lit from inside (a warm red, brightest where it is densest, dusky where it is thin) and
// blended premultiplied (it covers a little of what is behind it and adds a little light). And each puff carries its
// own piece of the mist: its noise is read at the puff's own "anchor", turned by its own angle, so the texture
// travels, tumbles and stretches with the mass instead of sliding under it.
// One noise grain and one shared noise coordinate for the whole sheet (the puffs' anchors are averaged, never their
// grains): puffs lying on one another that disagree about either show their noise as concentric rings.
// All of it is in the coordinates of the canvas, so the rectangle it is drawn on can be anywhere and any size.
// Driven from code (MalaiseFx). Edit and press F8 in game to reload.
shader_type canvas_item;
render_mode blend_premul_alpha, unshaded;

const int N = 40;
uniform int count = 0;
uniform vec2 at[N];                 // where each puff is, canvas px
uniform vec2 ax[N];                 // x: how far it reaches, px; y: how thick it is, 0..1
uniform vec2 ex[N];                 // x: how old it is (ragged, thin), 0..1; y: how much of it is seen, 0..1
uniform vec2 sx[N];                 // drawn out along this way; its length is how many times longer than wide, less 1
uniform vec2 an[N];                 // the mist it carries: the material coordinate of its middle, px
uniform float ro[N];                // ...and how far that mist has turned, radians

uniform float clock = 0.0;
uniform float seed = 0.0;
uniform float intensity = 1.0;
uniform float wisp = 1.9;           // how ragged its rim is
uniform float core = 1.4;           // how far out it stays dense
uniform float grain = 1.15;         // how coarse its mist is (1 = a cell of 47 px)
uniform float cover = 0.70;         // how much of what is behind it it hides where it is dense
uniform float heat = 0.2;           // extra light in its dense middle (while it is strained)
uniform float warp = 0.8;           // how much its mist is stirred into itself

uniform vec3 col_rim : source_color = vec3(0.22, 0.06, 0.08);
uniform vec3 col_body : source_color = vec3(0.52, 0.15, 0.15);
uniform vec3 col_core : source_color = vec3(0.80, 0.33, 0.27);

varying vec2 world;

void vertex() {
	world = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}

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

vec3 mist(vec2 q, float t) {
	float w = fbm(q + vec2(0.0, t * 0.10));
	float n = fbm(q + vec2((w - 0.5) * 1.6 * warp + t * 0.07, w * 0.8 * warp - t * 0.05));
	float n2 = fbm(q * 2.3 + vec2(w * 1.3 * warp, t * 0.4));
	float fine = fbm(q * 4.6 + vec2(n * 2.2 * warp - t * 0.5, w * 2.0 * warp));
	return vec3(n, n2, fine);
}

void fragment() {
	vec2 p = world;
	const float M = 2.5;
	float sum = 0.0;
	float s_age = 0.0;
	float s_seen = 0.0;
	vec2 s_q = vec2(0.0);
	for (int i = 0; i < N; i++) {
		if (i >= count) break;
		vec2 v = p - at[i];
		float r = max(ax[i].x, 1.0);
		float d2 = dot(v, v);
		float k = dot(sx[i], sx[i]);
		if (k > 0.000001) {
			k = sqrt(k);
			float along = dot(v, sx[i]) / k;
			float long_r = 1.0 + k;
			d2 -= along * along * (1.0 - 1.0 / (long_r * long_r));
		}
		d2 /= r * r;
		if (d2 > 7.0) continue;
		float c = pow(max(ax[i].y, 0.0001), M) * exp(-M * d2) * smoothstep(0.0, 0.3, ex[i].y);
		sum += c;
		s_seen += c * ex[i].y;
		s_age += c * ex[i].x;
		float cs = cos(ro[i]);
		float sn = sin(ro[i]);
		s_q += c * (vec2(cs * v.x + sn * v.y, -sn * v.x + cs * v.y) + an[i]);
	}
	if (sum < 1e-6) {
		COLOR = vec4(0.0);
	} else {
		float age = s_age / sum;
		vec2 q = s_q / sum;
		float field = pow(sum, 1.0 / M);
		float edge = pow(sqrt(max(0.0, -log(min(field, 1.0)))), mix(core, 1.0, smoothstep(0.08, 0.60, age)));
		// (one grain for the whole sheet: a grain that follows how big the puffs are here warps the noise into rings)
		float cell = 47.0 * grain;
		vec3 m = mist(q / (cell * 0.62) + vec2(seed * 1.7, seed), clock);
		float n = m.x;
		float n2 = m.y;
		float fine = m.z;
		float rag = wisp * (1.0 + 1.1 * age);
		// (the noise eats its rim into wisps, but no holes into its middle: a small body with a hole in it reads as a ring)
		float d = 1.0 - smoothstep(0.25, 1.10, edge + (n - 0.5) * rag * (0.30 + 0.70 * smoothstep(0.05, 0.60, edge)));
		d *= 0.72 + 0.56 * n2;
		float a = pow(clamp(d, 0.0, 1.0), 1.4);
		a *= 0.78 + 0.22 * smoothstep(0.30, 0.70, n);
		a *= 1.0 - 0.45 * age;
		// lit from inside: dusky where thin, warm where dense, a hotter vein here and there in the middle
		float dense = smoothstep(0.30, 1.0, d);
		float streaks = smoothstep(0.52, 0.78, fine) * smoothstep(0.30, 0.62, n);
		vec3 col = mix(col_rim, col_body, smoothstep(0.04, 0.45, d));
		col = mix(col, col_core, clamp(smoothstep(0.55, 1.0, d) * (0.20 + 0.80 * smoothstep(0.35, 0.75, n2)) + streaks * 0.10, 0.0, 1.0));
		col *= (0.84 + 0.32 * n2) * (1.0 + heat * dense * 0.6);
		float alpha = clamp(a * 0.85 * intensity * s_seen / sum, 0.0, 1.0);
		COLOR = vec4(col * alpha, alpha * cover);
	}
}
