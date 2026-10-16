// SilentFX - wail_warp: the shock fronts of Piercing Wail.
// Drawn over the fight (under the hand and the buttons). It reads what is already on screen and bends it
// where a front is passing: each front is an upright oval round her that grows until it is off the screen,
// cut away above and below her so that what is left is two tall arcs, one racing to each side.
// Everything is driven from code (PiercingWailFx). Edit and press F8 to reload.
shader_type canvas_item;
render_mode unshaded;

uniform sampler2D screen : hint_screen_texture, filter_linear, repeat_disable;

uniform float amount = 1.0;                 // 0 = the scene as it is
uniform vec2 centre = vec2(0.28, 0.5);      // her mouth, 0..1 of the screen
uniform float aspect = 0.0;                 // width / height of the screen; 0 = worked out here
uniform float tall = 0.92;                  // below 1 the fronts are taller than they are wide
uniform float sides = 0.62;                 // how much is cut away above and below her (0 = a full ring)
uniform float width = 0.055;                // how thick a front is, in screen heights
uniform float strength = 0.042;             // how far a front pushes what is behind it, in screen heights
uniform float wake = 0.35;                  // the ripple left behind a front (a fraction of the push)
uniform float split = 0.22;                 // red and blue pulled apart on a front (a fraction of the push)
uniform float falloff = 0.85;               // how quickly a front weakens as it travels
uniform vec4 tint : source_color = vec4(0.72, 0.54, 1.0, 1.0);
uniform float glow = 0.20;                  // light added on a front
uniform float fronts[3];                    // how far each front has got (screen heights; below 0 = no front)
uniform float powers[3];                    // how strong each front is, 0..1

void fragment() {
	vec2 uv = SCREEN_UV;
	float wide = aspect > 0.01 ? aspect : SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.x;
	vec2 oval = uv - centre;
	oval.x *= wide;
	oval.y *= tall;
	float far = length(oval);

	// Which way a front is travelling here (the oval's own normal), and how far to the side of her this is.
	vec2 outwards = vec2(oval.x, oval.y * tall);
	float len = length(outwards);
	outwards = len > 0.0001 ? outwards / len : vec2(0.0);
	float sideways = far > 0.0001 ? abs(oval.x) / far : 0.0;
	float open = sides > 0.001 ? smoothstep(sides * 0.35, sides, sideways) : 1.0;

	float crest = 0.0;
	float bend = 0.0;
	for (int i = 0; i < 3; i++) {
		float at = fronts[i];
		if (at >= 0.0) {
			float x = (far - at) / width;
			float weight = powers[i] / (1.0 + falloff * at);
			float ridge = exp(-x * x);
			crest += ridge * weight;
			// The ridge shoves the picture outwards; just behind it the picture swings back and forth once or twice.
			float behind = clamp(-x, 0.0, 6.0);
			bend += (ridge + wake * sin(behind * 2.4) * exp(-behind * 0.75)) * weight;
		}
	}
	crest = min(crest, 1.3) * open * amount;
	bend = clamp(bend, -1.5, 1.5) * open * amount;

	vec2 push = outwards * bend * strength;
	push.x /= wide;
	vec2 look = uv - push;
	vec2 pull = push * split;
	vec3 seen = vec3(
		texture(screen, look + pull).r,
		texture(screen, look).g,
		texture(screen, look - pull).b);
	seen += tint.rgb * (glow * crest);

	COLOR = vec4(seen, 1.0);
}
