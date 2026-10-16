// SilentFX - sigil: rings, dashes and tick marks drawn with light.
// One shader covers reticles, rune circles, clock faces and shockwave rings.
// Everything is driven from code. Edit and press F8 to reload.
shader_type canvas_item;
render_mode blend_add, unshaded;

uniform vec4 tint : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float alpha = 1.0;

uniform float radius = 0.80;        // outer ring, 0..1 of the quad's half size
uniform float width = 0.030;
uniform float dashes = 0.0;         // break the outer ring into this many dashes (0 = solid)
uniform float spin = 0.0;           // rotation of the outer ring pattern and the ticks (radians)

uniform float inner = 0.0;          // second ring radius (0 = off)
uniform float inner_width = 0.020;
uniform float inner_dashes = 0.0;
uniform float inner_spin = 0.0;

uniform float ticks = 0.0;          // number of tick marks
uniform float tick_len = 0.14;
uniform float tick_width = 0.016;
uniform float tick_out = 0.0;       // 0 = ticks point inwards, 0.5 = centred on the ring, 1 = outwards

uniform float glow = 0.30;          // soft halo around the outer ring
uniform float fill = 0.0;           // faint light inside the ring

float band(float d, float w) {
	return 1.0 - smoothstep(w * 0.45, w, abs(d));
}

float dash_mask(float a, float n) {
	if (n < 0.5) {
		return 1.0;
	}
	float d = fract(a / TAU * n);
	return smoothstep(0.0, 0.06, d) * (1.0 - smoothstep(0.60, 0.66, d));
}

void fragment() {
	vec2 p = (UV - 0.5) * 2.0;
	float r = length(p);
	float a = atan(p.y, p.x);

	float v = band(r - radius, width) * dash_mask(a + spin, dashes);
	v += exp(-pow((r - radius) / (width * 3.5 + 0.0001), 2.0)) * glow;

	if (inner > 0.001) {
		v = max(v, band(r - inner, inner_width) * dash_mask(a + inner_spin, inner_dashes));
	}

	if (ticks > 0.5) {
		float seg = TAU / ticks;
		float da = mod(a + spin + seg * 0.5, seg) - seg * 0.5;
		float lin = abs(da) * r;
		float r0 = mix(radius - tick_len, radius, tick_out);
		float r1 = r0 + tick_len;
		float along = smoothstep(r0 - 0.012, r0, r) * (1.0 - smoothstep(r1, r1 + 0.012, r));
		v = max(v, (1.0 - smoothstep(tick_width * 0.5, tick_width, lin)) * along);
	}

	v += fill * (1.0 - smoothstep(radius * 0.2, radius, r));
	v *= 1.0 - smoothstep(0.94, 1.0, r);

	COLOR = vec4(tint.rgb, clamp(v, 0.0, 1.0) * alpha * tint.a);
}
