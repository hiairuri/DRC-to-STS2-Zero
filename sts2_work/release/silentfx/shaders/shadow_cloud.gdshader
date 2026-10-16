// SilentFX - shadow cloud (Shadow Step): her shadow as one body of black mist that travels.
// There is no shape in it. It is given a handful of "puffs" - where the mist is, how far it reaches there,
// how thick, how old - and it melts them into one body: the youngest are the body itself, the older ones are
// what it has left hanging in the air behind it. Then the same mist as "shade_mist" is laid over that: the
// noise eats it away towards its rim, so that it ends in wisps and never at an edge, and a dusky violet shows
// where it is thin. The mist of the body turns with it (that is what makes it roll); what it has left behind
// stays where it was left.
// All of it is in the coordinates of the canvas, so the rectangle it is drawn on can be anywhere and any size.
// Driven from code (ShadowStepFx). Edit and press F8 in game to reload.
shader_type canvas_item;
render_mode blend_mix, unshaded;

const int N = 48;
uniform int count = 0;
uniform vec2 at[N];                 // where each puff is, canvas px
uniform vec2 ax[N];                 // x: how far it reaches, px; y: how thick it is, 0..1
uniform vec2 ex[N];                 // x: how old it is, 0 (the body itself) .. 1 (gone); y: how much of it is seen, 0..1
uniform vec2 sx[N];                 // which way it is drawn out by its own speed; its length is how many times longer than wide it is, less 1 (nothing = round)
uniform vec2 run[N];                // where it lies on the body's own way: x how far along it, y how far to the side of it, px
uniform float cb[N];                // how much its mist is combed out along that way, 0 (not at all: the mist as it always was) .. 1

uniform vec2 head = vec2(0.0);      // the middle of the body: its mist turns round this
uniform float head_r = 120.0;       // how far the body reaches, px
uniform float spin = 0.0;           // how far it has turned, radians
uniform float twist = 1.5;          // how much further its middle is turned than its rim (its mist lies in arms)
uniform float clock = 0.0;
uniform float seed = 0.0;
uniform float intensity = 1.0;
uniform float pulse = 0.0;          // a surge, 0..1
uniform float wisp = 1.25;          // how ragged its rim is
uniform float glow = 0.7;           // how strong the violet in its thin parts is
uniform float veil = 0.0;           // 1 = only streaks of it (the layer drawn over what it carries)
uniform float grain = 1.0;          // how coarse its mist is (1 = as coarse as the body is big)
uniform float core = 1.6;           // how far out it stays dense (1 = it thins from its very middle; 2 = dense nearly to its rim)
uniform float comb = 3.0;           // where its mist is combed out by its speed: how many times longer than wide its wisps are
uniform float sweep = 0.9;          // ...and how far those wisps are swept back towards its sides (0 = they lie straight along its way)

uniform vec3 col_ink : source_color = vec3(0.035, 0.012, 0.075);
uniform vec3 col_deep : source_color = vec3(0.11, 0.05, 0.22);
uniform vec3 col_glow : source_color = vec3(0.52, 0.30, 0.98);

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

// The mist at q: x eats the rim, y mottles the body, z is the fine streaks in it.
vec3 mist(vec2 q, float t) {
	float w = fbm(q + vec2(0.0, t * 0.10));
	float n = fbm(q + vec2((w - 0.5) * 1.6 + t * 0.07, w * 0.8 - t * 0.05));
	float n2 = fbm(q * 2.3 + vec2(w * 1.3, t * 0.4));
	float fine = fbm(q * 4.6 + vec2(n * 2.2 - t * 0.5, w * 2.0));
	return vec3(n, n2, fine);
}

void fragment() {
	vec2 p = world;

	// The puffs, melted into one body. (Their powers are summed, so that two lying on one another are hardly
	// bigger than one - a plain sum would swell wherever they crowd.)
	const float M = 2.5;
	float sum = 0.0;
	float s_age = 0.0;
	float s_seen = 0.0;
	float s_r = 0.0;
	float s_head = 0.0;
	float s_comb = 0.0;
	vec2 s_way = vec2(0.0);
	for (int i = 0; i < N; i++) {
		if (i >= count) break;
		vec2 v = p - at[i];
		float r = max(ax[i].x, 1.0);
		float d2 = dot(v, v);
		// (One that is going fast is drawn out the way it goes: its mist reaches further along that way.)
		float k = dot(sx[i], sx[i]);
		float along = 0.0;
		float across = 0.0;
		if (k > 0.000001) {
			k = sqrt(k);
			along = dot(v, sx[i]) / k;
			across = dot(v, vec2(-sx[i].y, sx[i].x)) / k;
			float long_r = 1.0 + k;
			d2 -= along * along * (1.0 - 1.0 / (long_r * long_r));
		}
		d2 /= r * r;
		if (d2 > 7.0) continue;
		float c = pow(max(ax[i].y, 0.0001), M) * exp(-M * d2);
		sum += c;
		s_age += c * ex[i].x;
		s_seen += c * ex[i].y;
		s_r += c * r;
		s_head += c * (1.0 - step(0.001, ex[i].x));
		if (k > 0.000001 && cb[i] > 0.001) {
			float w = c * cb[i];
			s_comb += w;
			s_way += w * (run[i] + vec2(along, across));
		}
	}
	if (sum < 1e-7) {
		COLOR = vec4(col_ink, 0.0);
	} else {
		float age = s_age / sum;
		float seen = s_seen / sum;
		float reach = s_r / sum;
		float own = s_head / sum;                 // how much of what is here is the body itself
		float field = pow(sum, 1.0 / M);
		// 0 deep inside it .. 1 at its rim (and on outwards).
		// (What is old has no dense middle any more: it thins from its very middle.)
		float edge = pow(sqrt(max(0.0, -log(min(field, 1.0)))), mix(core, 1.0, smoothstep(0.08, 0.60, age)));

		float t = clock;
		// How coarse the mist is goes with how big the body is - but less than in step with it: a big body
		// has more billows in it than a small one, not the same ones blown up.
		float cell = 170.0 * pow(max(reach, 16.0) / 140.0, 0.5) * grain;
		vec3 m;
		// What has been left behind stays where it was left...
		vec2 qw = p / (cell * 0.62) + vec2(seed * 1.7, seed);
		// ...and the body's own mist turns with it, its middle further than its rim.
		vec2 v = p - head;
		float rad = length(v) / max(head_r, 1.0);
		float turn = spin + twist * (1.0 - smoothstep(0.0, 1.5, rad));
		float cs = cos(turn);
		float sn = sin(turn);
		vec2 qh = vec2(cs * v.x + sn * v.y, -sn * v.x + cs * v.y) / cell + vec2(seed, seed * 2.3);
		// ...and where it goes fast its mist is combed out along its own way (told along that way, so that it
		// follows every turn the body has made).
		float combed = s_comb / sum;
		if (combed > 0.985) {
			vec2 on_way = s_way / s_comb;
			m = mist(vec2((on_way.x + abs(on_way.y) * sweep) / max(comb, 1.0), on_way.y) / (cell * 0.62) + vec2(seed * 0.9, seed * 1.3), t);
		} else {
			if (own > 0.985) {
				m = mist(qh, t);
			} else if (own < 0.015) {
				m = mist(qw, t);
			} else {
				m = mix(mist(qw, t), mist(qh, t), smoothstep(0.0, 1.0, own));
			}
			if (combed > 0.015) {
				vec2 on_way = s_way / s_comb;
				m = mix(m, mist(vec2((on_way.x + abs(on_way.y) * sweep) / max(comb, 1.0), on_way.y) / (cell * 0.62) + vec2(seed * 0.9, seed * 1.3), t), smoothstep(0.0, 1.0, combed));
			}
		}
		float n = m.x;
		float n2 = m.y;
		float fine = m.z;

		// The body: dense inside, and the noise eats it away towards the rim over a long way, so that it
		// feathers out into wisps instead of ending at an edge. What is older is more ragged, and thinner.
		float rag = wisp * (1.0 + 1.1 * age);
		float d = 1.0 - smoothstep(0.12, 1.08, edge + (n - 0.5) * rag);
		d *= 0.42 + 1.16 * n2;
		float a = smoothstep(0.02, 0.72, d * (1.0 + 0.6 * pulse));
		a = pow(a, 1.05) * (0.80 + 0.20 * smoothstep(0.30, 0.70, n));
		a *= 1.0 - 0.45 * age;

		// Where it is thin - its rim, its wisps - it is a dusky violet instead of black. A tint, not a line.
		float thin = smoothstep(0.02, 0.30, d) * (1.0 - smoothstep(0.30, 0.95, d));
		// Inside it: black, with streaks of a lighter dusk lying in the arms its turning draws.
		float streaks = smoothstep(0.50, 0.76, fine) * smoothstep(0.32, 0.62, n);
		vec3 col = mix(col_ink, col_deep, clamp(n2 * 0.6 + streaks * 0.85, 0.0, 1.0));
		col = mix(col, col_glow, clamp(thin * (0.36 * glow + 0.9 * pulse) + streaks * 0.10 * glow, 0.0, 1.0));
		// (That tint has a little body of its own - in patches, never as a band along the rim.)
		float alpha = max(a * 0.94, thin * smoothstep(0.34, 0.66, n) * (0.16 * glow + 0.5 * pulse));

		// Over what it carries: only streaks of it, so that the cards show through.
		float streak = smoothstep(0.40, 0.72, n) * (0.35 + 0.65 * n2);
		alpha = mix(alpha, alpha * (0.22 + 0.66 * streak), clamp(veil, 0.0, 1.0));

		COLOR = vec4(col, clamp(alpha * seen * intensity, 0.0, 1.0));
	}
}
