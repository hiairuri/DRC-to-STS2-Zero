// SilentFX - shade mist (Shadow Step, Shadowmeld).
// Darkness as mist: an ink-black body that the noise eats away towards its rim, so that it
// has no outline - only wisps - and a violet light where it is thin. Three shapes:
//   mode 0  a column standing on its ground line (near the bottom of the quad), rising
//   mode 1  a round blob, centred
//   mode 2  a bank lying against the left edge, whose front creeps and throws tongues out to the right
// Fully procedural. Edit and press F8 in game to reload.
shader_type canvas_item;
render_mode blend_mix, unshaded;

uniform float intensity = 1.0;   // overall fade, 0..1 (driven from code)
uniform float pulse = 0.0;       // a surge, 0..1
uniform float mode = 0.0;
uniform float seed = 0.0;
uniform float speed = 1.0;
uniform float hollow = 0.0;      // 0..1: how much thinner the middle is than the rim (what is behind shows through)
uniform float foot = 0.6;        // column: its width on the ground, as a share of its widest
uniform float bulge = 0.4;       // column: how far up it is widest (0..1)
uniform float taper = 1.6;       // column: more = it keeps its width longer and closes late
uniform float lean = 0.0;        // column: how far its top is carried sideways (-1..1 of the quad's half width)
uniform float front = 0.0;       // bank: where its front is (-1 the left edge .. 1 the right edge)
uniform float reach = 0.3;       // bank: how far its tongues reach past the front
uniform float wisp = 1.2;        // how ragged the rim is
uniform float glow = 0.6;        // how strong the violet light in the thin parts is

uniform vec3 col_ink : source_color = vec3(0.035, 0.012, 0.075);
uniform vec3 col_deep : source_color = vec3(0.11, 0.05, 0.22);
uniform vec3 col_glow : source_color = vec3(0.52, 0.30, 0.98);

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
	float t = TIME * speed;
	float px = (uv.x - 0.5) * 2.0;
	float py = (uv.y - 0.5) * 2.0;

	float edge;     // 0 deep inside it .. 1 at its rim
	vec2 q;         // where the noise is read: it drifts, so the mist moves

	if (mode < 0.5) {
		// The ground line is at 0.94 of the quad's height: what is below it only softens the foot.
		float h = clamp((0.94 - uv.y) / 0.94, 0.0, 1.0);
		float up = clamp((h - bulge) / max(1.0 - bulge, 0.001), 0.0, 1.0);
		float halfw = mix(foot, 1.0, smoothstep(0.0, bulge, h)) * (1.0 - pow(up, taper)) * 0.80 * (1.0 + 0.10 * pulse);
		float x = px - lean * h * h;
		edge = abs(x) / max(halfw, 0.02);
		q = vec2(uv.x * 3.0 + seed, uv.y * 2.6 + t * 0.55);            // it rises
	} else if (mode < 1.5) {
		edge = length(vec2(px, py));
		q = vec2(px * 1.6 + seed + t * 0.25, py * 1.6 - t * 0.18);
	} else {
		float band = fbm(vec2(uv.y * 3.2 + seed, t * 0.55));              // which heights are reaching out right now
		float tongue = smoothstep(0.48, 0.85, band) * reach;
		float crest = (1.0 - uv.y) * (1.0 - uv.y) * 0.16;                  // its top leans over, like a wave about to break
		float f = front + tongue + crest;
		edge = clamp((px + 1.0) / max(f + 1.0, 0.05), 0.0, 2.0);
		edge = max(edge, smoothstep(0.50, 1.0, -py) * 1.2);                // it thins out upwards
		q = vec2(uv.x * 2.6 - t * 0.35 + seed, uv.y * 2.4 + t * 0.12);    // it drifts to the right
	}

	float w = fbm(q);
	float n = fbm(q + vec2((w - 0.5) * 1.6, w * 0.8));
	float n2 = fbm(q * 2.3 + vec2(w * 1.3, t * 0.4));

	// The body: dense inside, and the noise eats it away towards the rim over a long way, so that
	// it feathers out into wisps instead of ending at an edge.
	float d = 1.0 - smoothstep(0.12, 1.08, edge + (n - 0.5) * wisp);
	d *= 0.42 + 1.16 * n2;
	float a = smoothstep(0.02, 0.72, d * (1.0 + 0.6 * pulse));
	a = pow(a, 1.05) * (0.80 + 0.20 * smoothstep(0.30, 0.70, n));

	// Thin in the middle, if asked.
	float middle = 1.0 - smoothstep(0.10, 0.85, edge + (n2 - 0.5) * 0.4);
	a *= 1.0 - hollow * middle;

	// Where it is thin - its rim, its wisps - it is a dusky violet instead of black. A tint, not a line:
	// it is spread over the whole feathered part, and it is never brighter than the mist is thick.
	float thin = smoothstep(0.02, 0.30, d) * (1.0 - smoothstep(0.30, 0.95, d));
	vec3 col = mix(col_ink, col_deep, n2 * 0.6);
	col = mix(col, col_glow, clamp(thin * (0.36 * glow + 0.9 * pulse), 0.0, 1.0));
	float alpha = max(a * 0.92, thin * (0.14 * glow + 0.5 * pulse));

	float efade = smoothstep(0.0, 0.06, uv.x) * smoothstep(1.0, 0.94, uv.x)
		* smoothstep(0.0, 0.06, uv.y) * smoothstep(1.0, 0.95, uv.y);
	COLOR = vec4(col, clamp(alpha * efade * intensity, 0.0, 1.0));
}
