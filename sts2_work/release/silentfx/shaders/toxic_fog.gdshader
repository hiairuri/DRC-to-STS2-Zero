// SilentFX - toxic fog.
// shape 0: a wide, flat bank of fog lying on the floor (Noxious Fumes: the fog that stays over the
//          enemies' side, and the sheet she blows across when the card is played).
// shape 1: mist filling an ellipse (Mirage).
// Everything is measured in canvas pixels, and the noise is pinned to the canvas, so the quad can be
// moved and resized without the fog stretching. Fully procedural: no textures.
// Edit and press F8 in game to reload.
shader_type canvas_item;
render_mode blend_mix, unshaded;

uniform float intensity = 1.0;      // final opacity, 0..1 (fade and stack level, driven from code)
uniform float thick = 1.0;          // how dense the body is
uniform float layer = 0.0;          // 0 = behind the characters, 1 = thin veil in front of them
uniform float shape = 0.0;          // 0 = bank on the floor, 1 = ellipse
uniform vec2 origin = vec2(0.0, 0.0);       // canvas position of the quad's top-left corner
uniform vec2 size = vec2(1200.0, 500.0);    // size of the quad
uniform float seed = 0.0;
uniform float scroll = 0.0;         // how far the fog has drifted so far (pixels; code advances it)
uniform float churn = 0.0;          // seconds of slow boiling (code advances it)

// ---- the bank
uniform float dir = 1.0;            // +1: it lies to her right and drifts right, -1: to her left
uniform float floor_y = 380.0;      // from the quad's top down to the floor line
uniform float rise = 240.0;         // how tall the bank stands above the floor
uniform float near_soft = 260.0;    // it thins out over this much towards her...
uniform float far_soft = 120.0;     // ...and over this much at the far end
uniform float head = 100000.0;      // nothing beyond this (from the near end): the rolling front
uniform float tail = -100000.0;     // nothing before this: the end she has let go of
uniform float roll = 0.0;           // 0..1: the front curls up and glows
uniform float cone = 0.0;           // 0..1: at the near end it is a narrow jet (her breath) that fans out to the floor
uniform float mouth = 200.0;        // height of that jet, from the quad's top
uniform float fan = 420.0;          // the jet has become the full bank this far out
uniform float pulse = 0.0;          // a surge running through it, 0..1
uniform float pulse_at = 0.0;       // where the surge is (from the near end)

// ---- the ellipse
uniform float fill = 1.0;           // 0 = empty, 1 = full of mist
uniform float flash = 0.0;          // 0..1: it lights up
uniform float scatter = 0.0;        // 0 = whole, 1 = blown apart and gone

uniform vec4 col_deep : source_color = vec4(0.07, 0.24, 0.10, 1.0);
uniform vec4 col_mid : source_color = vec4(0.20, 0.52, 0.20, 1.0);
uniform vec4 col_glow : source_color = vec4(0.60, 0.94, 0.36, 1.0);

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
	return v / 0.9375;
}

vec4 bank(vec2 px) {
	// Distance from the near end (her end) of the quad.
	float a = dir > 0.0 ? px.x : size.x - px.x;
	vec2 world = origin + px;

	// Noise pulled out sideways so the fog lies in long flat banks; all of it drifts away from her.
	vec2 q = vec2(world.x / 250.0, world.y / 112.0) + vec2(seed, seed * 0.37);
	float sx = dir * scroll / 250.0;
	float w1 = fbm(q * 0.6 + vec2(-sx * 0.6 + 3.1, churn * 0.030));
	float w2 = fbm(q * 0.6 + vec2(-sx * 0.6 + 8.3, 4.7 - churn * 0.040));
	float n = fbm(q + vec2(-sx, 0.0) + vec2(w1, w2) * 0.80);                               // big billows
	float n2 = fbm(q * 1.7 + vec2(-sx * 2.4, churn * 0.09) + vec2(-w2, w1) * 0.60);     // wisps, a little faster

	// The surge and the rolling front both lift the top of the bank and light it.
	float surge_x = (a - pulse_at) / 200.0;
	float surge = pulse * exp(-surge_x * surge_x);
	float curl_x = (head - a - 80.0) / 130.0;
	float curl = roll * exp(-curl_x * curl_x);

	// Top and bottom of the fog at this distance: a narrow jet at her mouth, the full bank further out.
	float out_there = mix(1.0, smoothstep(0.0, max(fan, 1.0), a), clamp(cone, 0.0, 1.0));
	float tall = rise * mix(1.0, 0.42, layer) * (1.0 + 0.30 * surge + 0.55 * curl);
	float y_bot = mix(mouth + 38.0, floor_y, out_there);
	float y_top = mix(mouth - 38.0, floor_y - tall, out_there);
	float h = (y_bot - px.y) / max(y_bot - y_top, 1.0);      // 0 at the floor, 1 at the top

	float body = 1.0 - smoothstep(0.34, 1.0, h + (n - 0.5) * 1.90);     // ragged top
	body *= mix(0.66, 1.0, 1.0 - smoothstep(0.0, 0.80, h));              // densest near the floor
	if (h < 0.0) {
		float foot = h / (0.16 + 0.16 * n2);
		body *= exp(-foot * foot);                                       // only a soft foot below the floor
	}

	// Along its length: thin towards her, soft at the far end, and cut by the front and the tail.
	float ragged = (n2 - 0.5) * 170.0;
	float along = smoothstep(0.0, max(near_soft, 1.0), a) * smoothstep(0.0, max(far_soft, 1.0), size.x - a);
	along *= 1.0 - smoothstep(head - 130.0, head + 20.0, a - ragged);
	along *= smoothstep(tail - 40.0, tail + 210.0, a + ragged * 0.6);

	float billow = smoothstep(0.32, 0.68, n);
	float d = body * (0.36 + 0.86 * billow) * thick * along;
	d *= 1.0 + 0.55 * surge + 0.60 * curl;

	float al = smoothstep(0.03, 0.90, d);
	al *= 0.80 + 0.20 * smoothstep(0.25, 0.75, n2);          // break up the surface

	float hot = smoothstep(0.60, 1.05, d) * smoothstep(0.45, 0.80, n2);
	vec3 col = mix(col_deep.rgb, col_mid.rgb, smoothstep(0.0, 0.8, al));
	col = mix(col, col_glow.rgb, clamp(hot * 0.30 + (surge * 0.60 + curl * 0.45) * al, 0.0, 1.0));

	// The rectangle itself never shows.
	float frame = smoothstep(0.0, 40.0, px.y) * smoothstep(0.0, 26.0, size.y - px.y);
	return vec4(col, al * frame);
}

vec4 ellipse(vec2 px) {
	vec2 c = (px / size - 0.5) * 2.0;        // -1..1 across the quad
	vec2 world = origin + px;
	vec2 q = world / 150.0 + vec2(seed, seed * 0.61);
	float t = churn;
	float w1 = fbm(q * 0.7 + vec2(t * 0.25, -t * 0.20));
	float n = fbm(q + vec2(w1 * 0.80 - t * 0.30, t * 0.45 - scatter * 1.2));
	float n2 = fbm(q * 2.3 + vec2(t * 0.50 + scatter * 1.5, -t * 0.70) - w1 * 0.60);

	// The ellipse proper stops short of the quad, leaving room for its ragged edge and for it to blow apart.
	float r = length(c);
	float edge = 0.66 * (1.0 + 0.42 * scatter);
	float inside = 1.0 - smoothstep(edge - 0.34, edge + 0.10, r + (n - 0.5) * 0.44);

	// Filling: the mist comes out of nothing in patches that run together, the rim first.
	float patches = n * 0.60 + n2 * 0.40 + 0.34 * r;
	float gate = 1.0 - fill * 1.45;
	float filled = smoothstep(gate, gate + 0.32, patches);
	// Going: it is eaten away the same way while it spreads.
	float hole = 1.0 - scatter * 1.50;
	float eaten = 1.0 - smoothstep(hole, hole + 0.38, n2 * 0.55 + n * 0.45 + 0.12);

	float d = inside * filled * eaten * (0.42 + 0.78 * smoothstep(0.22, 0.78, n)) * thick;
	float al = smoothstep(0.04, 0.90, d);
	al *= 0.75 + 0.25 * smoothstep(0.25, 0.75, n2);

	// The flash: the whole body lights up, brightest along its rim.
	float rim_x = (r - edge + 0.07) / 0.085;
	float rim = exp(-rim_x * rim_x) * inside;
	float lit = clamp(flash * (0.62 + 0.38 * n2) + flash * rim * 0.6, 0.0, 1.0);
	vec3 col = mix(col_deep.rgb, col_mid.rgb, smoothstep(0.0, 0.7, al));
	col = mix(col, col_glow.rgb, lit);
	al = clamp(al * (1.0 + 0.70 * flash) + rim * flash * 0.35 * filled * eaten, 0.0, 1.0);
	return vec4(col, al);
}

void fragment() {
	vec2 px = UV * size;
	vec4 fog = shape > 0.5 ? ellipse(px) : bank(px);
	COLOR = vec4(fog.rgb, clamp(fog.a * intensity, 0.0, 1.0));
}
