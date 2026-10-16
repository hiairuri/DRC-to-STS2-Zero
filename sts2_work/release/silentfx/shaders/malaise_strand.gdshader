// SilentFX - malaise strand (Malaise): one whole strand of the enemy's strength, drawn from its chest into a bead.
// Not mist: one cohesive body. It is given its middle line as a handful of points - where each lies, how wide the
// strand is there (half its width, px), how far along it that is (px), how much of it there lies in front of the bead
// (0..1), how bright it is there - and draws a single tube along them: soft but contained edges (they waver only a
// little), a hotter core, and inside it streaks of red that run along it towards the bead ("flow" moves them). Its two
// ends melt away. The same strand is drawn twice: "layer" 0 under the beads (the part behind the bead), 1 over them
// (the part of its coil that passes in front of the bead).
// All of it is in the coordinates of the canvas, so the rectangle it is drawn on can be anywhere and any size.
// Driven from code (MalaiseFx). Edit and press F8 in game to reload.
shader_type canvas_item;
render_mode blend_premul_alpha, unshaded;

const int N = 32;
uniform int count = 0;
// its middle line, from the chest to the bead, packed in two:
uniform vec4 seg[N];                // xy: where each point lies, canvas px; z: half its width there, px; w: how much of
                                    // it there lies in front of the bead, 0..1
uniform vec2 fl[N];                 // x: how far along it each point is, px; y: how bright it is there (1 = as it always
                                    // is; a wave running along it is more)

uniform float total = 1.0;          // how long it is, px
uniform float root = 12.0;          // it fades in over this much where it leaves the chest, px
uniform float flow = 0.0;           // how far what runs inside it has run towards the bead, px
uniform float clock = 0.0;
uniform float seed = 0.0;
uniform float intensity = 1.0;
uniform float layer = 0.0;          // 0: the part behind the bead; 1: the part in front of it
uniform float cover = 0.85;         // how much of what is behind it it hides
uniform float grain = 1.0;          // how coarse the streaks in it are (1 = 34 px along, 9 px across)
uniform float waver = 0.16;         // how much its edges waver

uniform vec3 col_rim : source_color = vec3(0.30, 0.05, 0.08);
uniform vec3 col_body : source_color = vec3(0.66, 0.12, 0.13);
uniform vec3 col_core : source_color = vec3(1.00, 0.48, 0.34);

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
	for (int i = 0; i < 3; i++) {
		v += amp * vnoise(p);
		p = vec2(1.6 * p.x + 1.2 * p.y + 3.1, -1.2 * p.x + 1.6 * p.y + 1.7);
		amp *= 0.5;
	}
	return v / 0.875;
}

void fragment() {
	vec2 p = world;
	float best = 1e9;           // how far from its middle, in half-widths
	float along = 0.0;
	float side = 0.0;
	float front = 0.0;
	float bright = 1.0;
	for (int i = 0; i < N - 1; i++) {
		if (i >= count - 1) break;
		vec4 g0 = seg[i];
		vec4 g1 = seg[i + 1];
		vec2 a = g0.xy;
		vec2 ab = g1.xy - a;
		float l2 = max(dot(ab, ab), 0.0001);
		float t = clamp(dot(p - a, ab) / l2, 0.0, 1.0);
		float wi = max(mix(g0.z, g1.z, t), 0.5);
		vec2 off = p - (a + ab * t);
		float dd = length(off) / wi;
		if (dd < best) {
			vec2 f = mix(fl[i], fl[i + 1], t);
			best = dd;
			along = f.x;
			side = (ab.x * off.y - ab.y * off.x) / sqrt(l2);
			front = mix(g0.w, g1.w, t);
			bright = f.y;
		}
	}
	vec4 outc = vec4(0.0);
	if (best < 1.6) {
		// what runs inside it: streaks drawn out along it, running towards the bead
		vec2 q = vec2((along - flow) / (34.0 * grain), side / (9.0 * grain)) + vec2(seed * 3.1, seed * 1.7);
		float n = fbm(q + vec2(0.0, clock * 0.12));
		float n2 = fbm(vec2(q.x * 0.45, q.y * 1.4) + vec2(seed * 2.3 + clock * 0.2, 5.0));
		// one body: its edge soft but contained, wavering only a little
		float edge = best + (n2 - 0.5) * waver * 2.0;
		float body = 1.0 - smoothstep(0.55, 1.0, edge);
		// its two ends melt away (where it leaves the chest, into the glow there)
		body *= smoothstep(0.0, root, along) * (1.0 - smoothstep(total - 10.0, total, along));
		float core = 1.0 - smoothstep(0.0, 0.8, best);
		float vein = smoothstep(0.40, 0.85, n);
		vec3 col = mix(col_rim, col_body, smoothstep(0.10, 0.65, body));
		col = mix(col, col_core, clamp(core * core * (0.30 + 0.70 * vein) * bright, 0.0, 1.0));
		col *= (0.82 + 0.30 * n) * (0.80 + 0.30 * bright);
		float part = layer > 0.5 ? front : 1.0 - front;
		float alpha = clamp(body * (0.86 + 0.14 * n) * intensity * part, 0.0, 1.0);
		outc = vec4(col * alpha, alpha * cover);
	}
	COLOR = outc;
}
