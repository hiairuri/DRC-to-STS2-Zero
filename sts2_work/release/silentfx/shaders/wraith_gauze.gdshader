// SilentFX - wraith gauze (Wraith Form).
// One strip of grave gauze, drawn on a Line2D: UV.x runs along it, UV.y across it. A loose weave with
// frayed edges, a crease wandering down it, a rolled hem that catches the light. Thin, it is a ghost of
// a thing (you see through the weave); "solid" closes the weave up.
// The line needs a texture (any) so that it has UVs. Edit and press F8 to reload.
shader_type canvas_item;
render_mode blend_mix, unshaded;

uniform float solid = 0.0;       // 0 = a ghost of gauze, 1 = cloth
uniform float glow = 0.0;        // the scream lights it from inside
uniform float threads = 90.0;    // weave count along the strip
uniform vec3 col_pale : source_color = vec3(0.74, 0.88, 0.82);
uniform vec3 col_shade : source_color = vec3(0.34, 0.52, 0.50);
uniform vec3 col_lit : source_color = vec3(0.62, 1.00, 0.55);

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

void fragment() {
	vec4 tint = COLOR;                       // the line's own colour and the fades of everything above it
	float along = UV.x;
	float v = UV.y * 2.0 - 1.0;
	float av = abs(v);
	float id = tint.r * 37.0 + tint.g * 11.0;   // strips are tinted a little differently: that tells them apart

	// Frayed edges: where the strip stops wanders in and out along it.
	float fray = vnoise(vec2(along * 16.0 + id, v * 1.3 + id));
	float edge = 1.0 - smoothstep(0.74, 0.98, av + (fray - 0.5) * 0.26);

	// A crease that wanders down the strip, and the hem that is rolled over at both sides.
	float mid = 0.34 * sin(along * 8.0 + id * 3.0) + 0.12 * sin(along * 21.0 + id);
	float crease = smoothstep(0.0, 0.42, abs(v - mid));
	float hem = smoothstep(0.45, 0.85, av);
	vec3 col = mix(col_shade, col_pale, 0.30 + 0.70 * crease);
	col += vec3(0.10, 0.10, 0.08) * hem;

	// The weave: you see through it, in patches, while it is a ghost.
	float thin = vnoise(vec2(along * 9.0 + id * 1.7, v * 1.6 - id));
	float mesh = 0.5 + 0.5 * sin(v * 13.0 + id) * sin(along * threads * 6.2832);
	float weave = mix(0.62 + 0.30 * thin + 0.08 * mesh, 1.0, clamp(solid, 0.0, 1.0));
	float ends = smoothstep(0.0, 0.03, along) * (1.0 - smoothstep(0.95, 1.0, along));

	col = mix(col, col_lit, clamp(glow, 0.0, 1.0) * 0.55) * (1.0 + 0.6 * glow);
	float alpha = edge * weave * ends * tint.a;
	COLOR = vec4(col * mix(vec3(1.0), tint.rgb, 0.6), alpha);
}
