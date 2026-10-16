// SilentFX - Pinpoint's reticle: a ring with four ticks across it, drawn in light.
// It is what shows how far her aim is drawn in. Vague, it is a wide soft glow in two images that have not come
// together yet; sharp, the two are one - thin bright lines and a point in the middle. Nothing else changes.
// Driven from code (PinpointFx.cs). Edit and press F8 in game to reload.
shader_type canvas_item;
render_mode blend_add, unshaded;

uniform float focus = 1.0;          // 0 = as vague as it gets .. 1 = sharp
uniform float alpha = 1.0;          // how much of it is seen, 0..1
uniform float split = 0.0;          // how far apart its two images are (a share of half its size)
uniform float lock = 0.0;           // the flash as it comes sharp, 1 falling to 0
uniform float shut = 0.0;           // played: it closes on its middle (0 = as it is .. 1 = a point; a hair under 0 = a hair open)

uniform vec3 col : source_color = vec3(1.0, 0.93, 0.70);
uniform vec3 hot : source_color = vec3(1.0, 1.0, 0.95);

// How near p is to the reticle's lines (1 on them, falling off over "soft").
float lines(vec2 p, float soft) {
	float come = 0.80 * shut;           // how far in its ticks have come (they meet in the middle, then are a point)
	float ring = abs(length(p) - 0.56 * (1.0 - shut));
	vec2 a = abs(p);
	float along = max(a.x, a.y);        // how far out along the nearer axis
	float off = min(a.x, a.y);          // how far from that axis
	float tick = length(vec2(off, max(max(max(0.40 - come, 0.0) - along, along - max(0.80 - come, 0.0)), 0.0)));
	float d = min(ring, tick);
	return exp(-(d * d) / (soft * soft));
}

void fragment() {
	vec2 p = (UV - 0.5) * 2.0;
	float f = clamp(focus, 0.0, 1.0);
	float soft = mix(0.26, 0.022, pow(f, 0.7));
	vec2 s = vec2(split, split * 0.35);
	float v = 0.5 * (lines(p - s, soft) + lines(p + s, soft));
	// Vague, its light is spread thin; sharp, it is bright.
	float gain = mix(0.30, 1.0, f);
	// The point in its middle is only there once it is nearly sharp.
	// (Shut, the point is what all of it has become: it is there however vague the aim was - only dimmer - and a little bigger.)
	float sh = clamp(shut, 0.0, 1.0);
	sh *= sh;
	float point = exp(-dot(p, p) / (0.0016 + 0.02 * (1.0 - f) + 0.010 * sh)) * max(smoothstep(0.55, 1.0, f), sh * mix(0.2, 1.0, f * f));
	// (It thins away before the rectangle's own edge.)
	float edge = 1.0 - smoothstep(0.86, 1.0, max(abs(p.x), abs(p.y)));
	float a = (v * gain * mix(1.0, mix(0.3, 1.6, f), sh) + point) * edge * (1.0 + 1.5 * clamp(lock, 0.0, 1.0));
	vec3 c = mix(col, hot, clamp(v * f + point + lock, 0.0, 1.0));
	COLOR = vec4(c, clamp(a * alpha, 0.0, 1.0));
}
