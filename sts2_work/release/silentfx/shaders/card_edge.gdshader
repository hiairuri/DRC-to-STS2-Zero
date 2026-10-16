// SilentFX - card_edge: a card whose edge has been ground to a blade (Precise Cut in the hand).
// One rectangle over the card (the card plus "pad" all round). Nothing is laid ON the card: the card's own rim -
// along its top, from beside the cost to the right-hand corner, and on down its right side - is drawn as steel
// ground to an edge: a flat bevel from the grind line (dark, a little inside the card) out to the rim (a white
// hair), the top face lighter than the side face, fine whetstone marks across both. At the card's corner the two
// faces simply meet: the grind turns the corner with it, and what was a rounded corner is a square, sharp one.
// How much of the rim is ground is "bare" (0..1 of the whole run).
// Everything is driven from code (PreciseCutChains). Edit and press F8 to reload.
shader_type canvas_item;
render_mode unshaded;

uniform vec2 size = vec2(420.0, 542.0);     // the rectangle, in card units
uniform vec2 card = vec2(300.0, 422.0);     // the card, in card units
uniform float pad = 60.0;                   // the rectangle's margin round the card
uniform float from_x = 44.0;                // the grind starts this far from the card's left edge (clear of the cost)
uniform float side_len = 230.0;             // and runs this far down the right side
uniform float depth = 13.0;                 // how wide the ground face is
uniform float bare = 1.0;                   // how much of the run is ground, 0..1 (the spring may carry it past)
uniform float hot = 0.0;                    // 0..1: the stone is on the edge right now (where the grind ends)
uniform float spark_age = 9.0;              // seconds since the stone last bit
uniform float glint = -1.0;                 // a light running along the edge: 0..1 of the ground part; outside = none
uniform float star = 0.0;                   // 0..1: the twinkle at the point
uniform float ready = 0.0;                  // 0..1: wholly ground
uniform float lit = 1.0;                    // overall strength of the lights
uniform float seed = 0.0;

const vec3 RIM = vec3(1.0, 1.0, 1.0);
const vec3 STEEL_HI = vec3(0.90, 0.95, 1.00);
const vec3 STEEL_LO = vec3(0.40, 0.49, 0.62);
const vec3 GRIND = vec3(0.05, 0.08, 0.13);
const vec3 COLD = vec3(0.60, 0.88, 1.00);
const vec3 HOT = vec3(1.00, 0.78, 0.42);

float hash(float n) { return fract(sin(n * 127.1 + seed * 3.7) * 43758.5453); }

float seg(vec2 p, vec2 a, vec2 b) {
	vec2 pa = p - a;
	vec2 ba = b - a;
	float h = clamp(dot(pa, ba) / max(dot(ba, ba), 0.0001), 0.0, 1.0);
	return length(pa - ba * h);
}

void fragment() {
	float fade = COLOR.a;                       // the node's own fading (the card leaving the hand)
	vec2 p = UV * size - vec2(pad);             // card units, the card's top-left corner is the origin
	float w = card.x;
	float top_len = w - from_x;
	float total = top_len + side_len;
	float done = clamp(bare, 0.0, 1.12) * total;

	// Which face: the top one (measured down from the top rim) or the side one (measured in from the right rim).
	float d_top = p.y;                          // into the card from the top rim
	float d_side = w - p.x;                     // into the card from the side rim
	bool on_top = d_top <= d_side;
	float d = on_top ? d_top : d_side;
	float s = on_top ? (p.x - from_x) : (top_len + p.y);        // along the run
	// Past the corner the top face has no business (and the other way round).
	if (on_top && p.x > w + 2.0) s = -1.0;

	// The ground face is shallow where the stone starts, and where it has got to; it runs out down the side.
	float dep = depth;
	dep *= smoothstep(0.0, 30.0, s);
	dep *= smoothstep(0.0, 22.0, done - s);
	dep *= 1.0 - smoothstep(total - 80.0, total, s) * 0.85;
	if (!on_top) dep *= 0.88;

	vec3 col = vec3(0.0);
	float a = 0.0;

	// The card falls away into the grind: a short shadow on the card, just inside the grind line.
	if (s > 0.0 && s < done && dep > 0.4 && d > dep && d < dep + 6.0) {
		float sh = (1.0 - (d - dep) / 6.0);
		col = vec3(0.0, 0.02, 0.04);
		a = 0.42 * sh * sh * smoothstep(0.4, 4.0, dep);
	}

	if (s > 0.0 && s < done && dep > 0.4 && d >= -0.6 && d <= dep + 1.1) {
		float t = clamp(d / dep, 0.0, 1.0);
		// The flat of the grind: pale at the rim, deeper towards the grind line, one band of light lying along
		// it; the side face lies in less light.
		vec3 face = mix(STEEL_HI, STEEL_LO, pow(t, 0.75));
		face += vec3(0.16, 0.17, 0.18) * exp(-(t - 0.34) * (t - 0.34) / 0.012);
		if (!on_top) face *= 0.78;
		// Whetstone marks: the faintest strokes across the face, aslant.
		float mark = hash(floor((s + d * 1.4) / 1.1));
		face *= 0.965 + 0.07 * mark;
		// A long soft sheen that lives on the face.
		face += 0.10 * (0.5 + 0.5 * sin(s * 0.045 + TIME * 1.3)) * (1.0 - t) * lit;
		// The rim: a white hair. The grind line: a dark one, where the face meets the card.
		float rim = 1.0 - smoothstep(0.5, 1.3, d);
		float line = smoothstep(dep - 1.0, dep - 0.2, d);
		face = mix(face, GRIND, line * 0.75);
		face = mix(face, RIM, rim);
		// The corner itself: where the two rims meet the steel is at its thinnest, and catches the light.
		float nib = length(p - vec2(w, 0.0));
		face += vec3(0.30) * exp(-nib * nib / 30.0) * smoothstep(top_len - 6.0, top_len + 6.0, done);
		// A light running along the edge.
		if (glint >= 0.0 && glint <= 1.0) {
			float g = (s + d * 0.9) - glint * done;
			face += vec3(1.0) * exp(-g * g / 180.0) * 0.9 * lit;
		}
		// Wholly ground it breathes a little.
		face *= 1.0 + 0.06 * ready * sin(TIME * 2.2);
		col = face;
		a = 1.0 - smoothstep(dep + 0.1, dep + 1.1, d);
		a *= smoothstep(-0.6, 0.3, d);
	}

	// A breath of cold light just off the rim, along what is ground.
	if (s > 0.0 && s < done && d < 0.5) {
		float off = -d;
		float glow = exp(-off / (3.2 + 2.0 * ready)) * (0.30 + 0.30 * ready) * lit;
		glow *= smoothstep(0.0, 30.0, s) * smoothstep(0.0, 22.0, done - s);
		col = mix(col, COLD, glow / max(a + glow, 0.001));
		a = max(a, glow);
	}

	// Where the grind has got to.
	vec2 end;
	if (done <= top_len) end = vec2(from_x + done, 0.0);
	else end = vec2(w, done - top_len);
	bool end_on_top = done <= top_len;

	// The stone on the edge: a hot point, and sparks thrown back off it.
	if (hot > 0.01) {
		float r = length(p - end);
		float h = exp(-r * r / 60.0) * hot;
		col = mix(col, HOT, h / max(a + h, 0.001));
		a = max(a, h);
	}
	if (spark_age < 0.55) {
		for (int i = 0; i < 9; i++) {
			float fi = float(i);
			float born = hash(fi * 7.3 + 1.0) * 0.14;
			float age = spark_age - born;
			if (age < 0.0) continue;
			float life = 0.22 + 0.22 * hash(fi * 3.1 + 5.0);
			if (age > life) continue;
			float u = age / life;
			// Thrown off the rim: back along the way the stone came, and out from the card.
			float fan = (hash(fi * 11.7 + 2.0) - 0.5) * 1.5;
			vec2 dir = end_on_top ? vec2(-0.72, -0.70) : vec2(0.70, -0.72);
			dir = vec2(dir.x * cos(fan) - dir.y * sin(fan), dir.x * sin(fan) + dir.y * cos(fan));
			float speed = 150.0 + 190.0 * hash(fi * 5.9 + 9.0);
			// It set off from where the stone was then (a little behind where it has got to).
			vec2 start = end + (end_on_top ? vec2(-1.0, 0.0) : vec2(0.0, -1.0)) * (hash(fi * 2.3 + 4.0) * 26.0);
			vec2 at = start + dir * speed * age + vec2(0.0, 260.0) * age * age;
			vec2 vel = dir * speed + vec2(0.0, 520.0) * age;
			vec2 back = at - normalize(vel) * (7.0 + 12.0 * (1.0 - u));
			float k = (1.0 - smoothstep(0.7, 1.9, seg(p, at, back))) * (1.0 - u * u);
			vec3 c = mix(vec3(1.0, 0.97, 0.88), HOT, u);
			col = mix(col, c, k / max(a + k, 0.001));
			a = max(a, k);
		}
	}

	// Part ground: a small steady light where the edge ends for now.
	if (done > 12.0 && ready < 0.6 && hot < 0.5) {
		float r = length(p - end - vec2(end_on_top ? -6.0 : 0.0, end_on_top ? 0.0 : -6.0));
		float e = exp(-r * r / 22.0) * (0.55 + 0.25 * sin(TIME * 5.0)) * (1.0 - ready) * lit;
		col = mix(col, vec3(1.0), e / max(a + e, 0.001));
		a = max(a, e);
	}

	// The twinkle at the point.
	if (star > 0.01) {
		vec2 q = p - vec2(w, 0.0);
		float reach = 30.0 + 26.0 * star;
		float arm = max(exp(-abs(q.x) / reach * 3.2) * exp(-abs(q.y) / 1.5), exp(-abs(q.y) / reach * 3.2) * exp(-abs(q.x) / 1.5));
		float core = exp(-dot(q, q) / 36.0);
		float k = clamp((arm + core) * star * lit, 0.0, 1.0);
		col = mix(col, vec3(1.0), k / max(a + k, 0.001));
		a = max(a, k);
	}

	COLOR = vec4(col, clamp(a, 0.0, 1.0) * fade);
}
