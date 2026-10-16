// SilentFX - shadow pool (Shadow Step: the darkness under her feet).
// A slowly swirling puddle of darkness on the floor. The quad is squashed into an
// ellipse by its size. Fully procedural. Edit and press F8 to reload.
shader_type canvas_item;
render_mode blend_mix, unshaded;

uniform float intensity = 1.0;   // overall fade, 0..1
uniform float pulse = 0.0;       // swells when the power triggers, 0..1
uniform float seed = 0.0;
uniform float calm = 0.0;        // 0..1: how much of the lit edge is taken away (0 = all of it shows, 1 = none)

uniform vec3 col_core : source_color = vec3(0.02, 0.01, 0.05);
uniform vec3 col_edge : source_color = vec3(0.30, 0.16, 0.52);
uniform vec3 col_glow : source_color = vec3(0.42, 0.24, 0.80);

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
	vec2 p = (UV - 0.5) * 2.0;
	float r = length(p);
	float t = TIME;

	// Swirl: the further out, the more the noise lags behind the rotation.
	float sw = t * 0.35 + r * 2.2;
	vec2 q = vec2(cos(sw) * p.x - sin(sw) * p.y, sin(sw) * p.x + cos(sw) * p.y);
	float n = fbm(q * 2.4 + seed);
	float n2 = fbm(q * 5.0 - vec2(t * 0.2, 0.0) + seed * 2.0);

	float rim = (0.70 + 0.14 * pulse) + (n - 0.5) * 0.42;
	float body = 1.0 - smoothstep(rim - 0.40, rim, r);
	float a = body * (0.62 + 0.38 * n2);

	// A violet glow runs around the edge so the pool reads against dark floors.
	float lip = smoothstep(rim - 0.34, rim - 0.10, r) * (1.0 - smoothstep(rim - 0.10, rim, r));
	lip *= 0.55 + 0.45 * n2;
	lip *= 1.0 - clamp(calm, 0.0, 1.0);

	vec3 col = mix(col_core, col_edge, clamp(lip * 1.3 + 0.35 * pulse, 0.0, 1.0));
	col += col_glow * lip * (0.55 + 0.8 * pulse);

	float alpha = max(a * 0.92, lip * 0.85) * intensity * (0.85 + 0.15 * pulse);
	COLOR = vec4(col, clamp(alpha, 0.0, 1.0));
}
