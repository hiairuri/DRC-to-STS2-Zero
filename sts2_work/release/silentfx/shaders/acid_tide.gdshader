// SilentFX - acid_tide (Corrosive Wave): the whole floor of the battlefield under acid.
// The floor is seen at a low angle, so everything is worked out ON THE FLOOR: each pixel is taken back to
// where it lies on the ground (sideways and in depth, measured from the Silent's feet), and the flood, its
// swell, its sheen, the rings that run through it and the ripples round the creatures' feet are drawn there.
// That is what makes them flat and small towards the horizon and wide near the bottom of the screen.
// No outline anywhere: a dark stain with light lying on it. Everything is in canvas pixels.
// Edit and press F8 to reload.
shader_type canvas_item;
render_mode blend_mix, unshaded;

uniform float alpha = 1.0;
uniform vec2 size = vec2(2320.0, 600.0);    // size of the quad
uniform float horizon = 120.0;      // from the quad's top down to the far line of the floor (where the acid ends)
uniform float vanish = 110.0;       // how far above that line the floor's vanishing line lies
uniform float focal = 800.0;        // depth scale: (her row - vanishing line) / how flat a circle on the floor looks
uniform vec2 her = vec2(600.0, 170.0);      // her feet, from the quad's top-left corner
uniform float far_soft = 46.0;      // how gradually the acid thins out towards the far line
uniform float reach = 100000.0;     // how far the flood has run from her (floor units = pixels in her row)
uniform float crest = 0.0;          // the bright leading edge of the flood while it spreads, 0..1
uniform float flow = 0.0;           // the surface's own clock (seconds; code advances it)
uniform vec4 pulse_r = vec4(-1.0);  // up to four rings running out from her: how far each has got...
uniform vec4 pulse_a = vec4(0.0);   // ...and how strong each is
uniform vec4 feet[8];               // creatures standing in it: x, y (quad pixels), half width, age of the ripple a draw sent up (< 0: none)
uniform int feet_n = 0;
uniform float kick = 0.0;           // extra light right after a trigger, 0..1
uniform float stain = 0.46;         // strength of the dark stain
uniform float glow = 0.30;          // strength of the light lying on it
uniform float haze = 0.06;          // faint light standing in the air over the far line
uniform float bubbles = 1.0;        // how many bubbles break the surface, 0..1
uniform float layer = 0.0;          // 0 = the floor (behind the creatures), 1 = only the film over their feet (in front of them)
uniform float wade = 9.0;           // how deep they stand in it (pixels of their feet the film covers)
uniform float seed = 0.0;

uniform vec4 deep : source_color = vec4(0.04, 0.25, 0.07, 1.0);
uniform vec4 bright : source_color = vec4(0.50, 0.98, 0.32, 1.0);
uniform vec4 hot : source_color = vec4(0.82, 1.0, 0.26, 1.0);

float hash(vec2 p) {
	p = fract(p * vec2(123.34, 456.21));
	p += dot(p, p + 45.32);
	return fract(p.x * p.y);
}

float noise(vec2 p) {
	vec2 i = floor(p);
	vec2 f = fract(p);
	f = f * f * (3.0 - 2.0 * f);
	return mix(mix(hash(i), hash(i + vec2(1.0, 0.0)), f.x),
			mix(hash(i + vec2(0.0, 1.0)), hash(i + vec2(1.0, 1.0)), f.x), f.y);
}

float fbm(vec2 p) {
	float v = 0.0;
	float a = 0.5;
	for (int i = 0; i < 3; i++) {
		v += a * noise(p);
		p = p * 2.03 + 17.1;
		a *= 0.5;
	}
	return v / 0.875;
}

// exp(-x*x): a soft bump one unit wide. (pow() of a negative number is undefined, so never pow(x, 2.0).)
float bell(float x) {
	return exp(-x * x);
}

// A bubble comes up and breaks: a glint, then one ring that opens and is gone. One per cell of the floor, now and then.
float plips(vec2 f, float t, float thin) {
	vec2 cell = vec2(230.0, 250.0);
	vec2 g = f / cell + seed * 1.7;
	vec2 c = floor(g);
	if (hash(c + 11.3) > 0.80 * bubbles) return 0.0;
	float period = 2.4 + 3.4 * hash(c + 7.1);
	float turn = t / period + hash(c + 3.3);
	float k = fract(turn) * period / 0.95;          // 0..1 over the 0.95 s the bubble lasts
	if (k > 1.0) return 0.0;
	float n = floor(turn);
	vec2 o = (vec2(hash(c + n * 0.37 + 1.7), hash(c + n * 0.53 + 5.9)) - 0.5) * 0.36;
	vec2 rel = (fract(g) - 0.5 - o) * cell;
	float r = length(rel);
	float rad = 10.0 + 52.0 * sqrt(k);
	float w = max(6.0, thin);
	float ring = bell((r - rad) / w) * (1.0 - k) * (1.0 - k);
	float dome = exp(-r * r / (60.0 + thin * thin)) * (1.0 - smoothstep(0.0, 0.35, k));
	return ring * 0.75 + dome;
}

void fragment() {
	vec2 px = UV * size;
	float yv = horizon - vanish;
	float den = max(her.y - yv, 1.0);
	float below = px.y - horizon;

	// Back to the floor. Rows above the far line (only the haze lives there) take the far line's own place.
	float row = max(px.y, horizon + 1.0);
	float s = max((row - yv) / den, 0.04);                  // how big things are drawn in this row (1 = her row)
	vec2 f = vec2((px.x - her.x) / s, focal * (1.0 / s - 1.0));
	float d = length(f);
	float thin = 1.3 * focal / (s * s * den);               // one pixel of this row, in floor units of depth
	float near = clamp(below / max(size.y - horizon, 1.0), 0.0, 1.0);

	// How far the flood has run: a ragged, lobed front.
	float rag = (fbm(f / vec2(300.0, 240.0) + seed + vec2(0.0, flow * 0.06)) - 0.5) * 2.0 * min(150.0, reach * 0.30);
	float dr = d + rag;
	float soft = min(110.0, reach * 0.35) + thin + 6.0;
	float run = 1.0 - smoothstep(reach - soft, reach, dr);
	run *= smoothstep(0.0, 60.0, px.x) * smoothstep(0.0, 60.0, size.x - px.x);

	// How much acid lies here: nothing at the far line, thin behind the creatures, deep towards the bottom.
	float edge = below + (noise(vec2(px.x / 110.0 + seed, 2.3)) - 0.5) * far_soft * 0.7;
	float thick = smoothstep(0.0, max(far_soft, 4.0), edge) * mix(0.58, 1.0, smoothstep(0.0, 0.55, near));

	// The surface: a slow swell, and two sheets of sheen drifting across each other.
	vec2 q = f / vec2(330.0, 300.0) + seed;
	float swell = fbm(q * 0.6 + vec2(flow * 0.045, -flow * 0.030));
	vec2 w = q + (swell - 0.5) * 0.9;
	float s1 = fbm(w * vec2(1.0, 1.6) + vec2(flow * 0.10, flow * 0.05));
	float s2 = fbm(w * vec2(1.9, 2.6) - vec2(flow * 0.07, -flow * 0.08) + 7.3);
	float sheen = smoothstep(0.50, 0.80, s1 * 0.62 + s2 * 0.38 + (swell - 0.5) * 0.30);
	float ridge = 1.0 - abs(2.0 * fbm(w * vec2(1.4, 2.0) + vec2(-flow * 0.06, flow * 0.045) + 3.1) - 1.0);
	float veins = smoothstep(0.80, 0.98, ridge) * (0.35 + 0.65 * s2);

	// The leading edge while it spreads, and the rings a card drawn sends out from her.
	float lead = smoothstep(reach - 220.0 - soft, reach - soft * 0.6, dr) * run * crest;
	float rings = 1.0;
	for (int i = 0; i < 4; i++) {
		float x = pulse_r[i] - (d + rag * 0.25);
		float back = max(x, 0.0);
		float ring = smoothstep(-(14.0 + thin), 0.0, x) * (0.42 * exp(-back / 20.0) + 0.40 * exp(-back / 80.0) + 0.18 * exp(-back / 300.0));
		rings *= 1.0 - clamp(pulse_a[i] * ring, 0.0, 1.0) * 0.85;
	}
	rings = 1.0 - rings;

	// The creatures stand in it: a rim where the acid meets them, slow rings leaving them, and the ripple a draw sends up.
	float ripple = 0.0;
	float film = 0.0;
	float lip = 0.0;
	for (int i = 0; i < 8; i++) {
		if (i >= feet_n) break;
		vec4 ft = feet[i];
		float si = max((max(ft.y, horizon + 1.0) - yv) / den, 0.04);
		vec2 fi = vec2((ft.x - her.x) / si, focal * (1.0 / si - 1.0));
		float hw = max(ft.z / si, 20.0);
		float dd = length(f - fi);
		float beyond = max(dd - hw, 0.0);
		float lam = 54.0;
		float fine = smoothstep(2.5 * thin, 6.0 * thin, lam);
		float rim = bell((dd - hw) / max(9.0, thin)) * 0.55;
		float train = (0.5 + 0.5 * sin((beyond / lam - flow * 0.9) * 6.2832)) * exp(-beyond / 85.0) * smoothstep(0.0, 20.0, beyond) * 0.38 * fine;
		float hit = 0.0;
		if (ft.w >= 0.0) {
			float life = clamp(1.0 - ft.w / 0.9, 0.0, 1.0);
			float rad = hw * 0.6 + ft.w * 520.0;
			hit = bell((dd - rad) / max(26.0, thin * 1.5)) * life * life * 1.6;
			hit += exp(-dd * dd / (hw * hw * 1.4)) * life * life * life * 0.8;
		}
		ripple += rim + train + hit;
		// In front of this creature: only the few rows of its feet that are under the surface (nothing below
		// them - its health bar is there).
		float waterline = ft.y - wade * si;
		float strip = smoothstep(waterline - 2.5, waterline + 1.5, px.y) * (1.0 - smoothstep(ft.y - 1.0, ft.y + 3.0, px.y));
		strip *= 1.0 - smoothstep(ft.z * 0.80, ft.z * 1.10, abs(px.x - ft.x));
		film = max(film, strip);
		lip = max(lip, strip * bell((px.y - waterline) / 2.4) * (1.0 + 1.2 * hit));
	}
	ripple = min(ripple, 1.6);

	float bub = plips(f, flow, thin);

	// The stain, and the light lying on it.
	float cover = thick * run * mix(1.0, film * 0.80, layer);
	float a_stain = stain * cover * (0.82 + 0.18 * swell);
	float light = 0.14 + 0.50 * sheen + 0.60 * veins;
	light *= 0.70 + 0.60 * near;
	float a_glow = glow * cover * light * (1.0 + 0.30 * kick);
	a_glow += cover * (0.62 * rings + 0.60 * lead * (0.80 - 0.35 * near));
	a_glow += thick * run * mix(ripple * 0.34 * mix(0.6, 1.0, near) + bub * 0.42, lip * 0.40, layer);
	float heat = clamp(sheen * 0.30 + rings * 0.55 + lead * 0.7 + kick * 0.10, 0.0, 1.0);

	// A faint light standing in the air over the far line (only behind the creatures).
	float up = max(-below, 0.0);
	float air = exp(-up / max(vanish * 0.55, 8.0)) * (1.0 - smoothstep(0.0, far_soft * 1.5, below)) * smoothstep(0.0, 40.0, px.y);
	a_glow += haze * air * run * (0.7 + 0.3 * swell) * (1.0 + 0.6 * kick + 1.2 * rings + 1.5 * lead) * (1.0 - layer);

	// One pass: the stain covers what is under it, the light is added on top of that. Never to white.
	a_glow = 1.0 - exp(-a_glow * 1.25);
	vec3 lit = mix(bright.rgb, hot.rgb, heat);
	float a = clamp(a_stain + a_glow * 0.60, 0.0, 0.92);
	vec3 col = (deep.rgb * a_stain + lit * a_glow) / max(a, 0.001);
	COLOR = vec4(min(col, vec3(1.0)), a * alpha);
}
