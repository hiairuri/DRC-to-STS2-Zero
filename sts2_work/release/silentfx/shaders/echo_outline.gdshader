// SilentFX - echo outline (Afterimage, v0.72).
// The same light-made afterimage as echo_figure, but its shape is her own picture:
// TEXTURE is a live copy of the Silent (whatever skin she wears), only its alpha is used.
// Brighter outline, dimmer inside, scan lines and a faint flicker - like echo_figure.
// Edit and press F8 to reload.
shader_type canvas_item;
render_mode blend_add, unshaded;

uniform float alpha = 0.3;       // driven from code
uniform float shift = 0.0;       // phase offset so the echoes do not flicker in sync
uniform vec3 tint : source_color = vec3(0.45, 0.80, 1.00);
uniform float edge_px = 3.0;     // outline thickness, in copy pixels (the copy is half size)
uniform float inner = 0.55;      // how much of the inside is taken away (echo_figure: 0.55)

void fragment() {
	vec2 px = TEXTURE_PIXEL_SIZE;
	float a = texture(TEXTURE, UV).a;
	float sum = 0.0;
	float lo = 1.0;
	for (int i = 0; i < 8; i++) {
		float ang = float(i) * 0.7853982;
		vec2 d = vec2(cos(ang), sin(ang)) * px * edge_px;
		float a1 = texture(TEXTURE, UV + d).a;
		float a2 = texture(TEXTURE, UV + d * 0.5).a;
		sum += a1 + a2;
		lo = min(lo, min(a1, a2));
	}
	float shape = clamp(max(a, sum / 16.0 * 1.25), 0.0, 1.0);   // a little softer and wider than her
	float core = min(a, lo);                                      // her inside (eroded)
	float glow = shape - core * inner;

	float fade = 1.0 - smoothstep(0.80, 1.0, UV.y) * 0.75;
	float scan = 0.78 + 0.22 * sin(UV.y * 70.0 - TIME * 5.0 + shift);
	float flick = 0.90 + 0.10 * sin(TIME * 11.0 + shift * 3.0);

	COLOR = vec4(tint, clamp(glow * fade * scan * flick * alpha, 0.0, 1.0));
}
