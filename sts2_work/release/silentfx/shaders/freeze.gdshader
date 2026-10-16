// SilentFX - freeze: the world while Bullet Time has stopped it.
// Drawn over the fight (under the hand and the buttons). It reads what is already on screen,
// drains its colour towards a cold blue, darkens the corners, pulls red and blue slightly apart,
// and lets rings of bent light spread out from the Silent each time she acts.
// Everything is driven from code. Edit and press F8 to reload.
shader_type canvas_item;
render_mode unshaded;

uniform sampler2D screen : hint_screen_texture, filter_linear, repeat_disable;

uniform float amount = 0.0;                 // 0 = the scene as it is, 1 = fully frozen
uniform vec4 cold : source_color = vec4(0.66, 0.82, 1.0, 1.0);
uniform float drain = 0.58;                 // how much of the colour is taken out
uniform float lift = 1.16;                  // brightness, so the blue does not just look dark
uniform float corners = 0.26;               // how dark the corners get
uniform float split = 0.0013;               // red and blue pulled apart (a fraction of the screen width)
uniform float shimmer = 0.0015;             // a slow wobble, as if the air itself had thickened

uniform vec2 centre = vec2(0.28, 0.58);     // where she stands, 0..1 of the screen
uniform float ring_width = 0.05;            // in screen heights
uniform float ring_bend = 0.032;            // how far a ring pushes what is behind it
uniform float rings[8];                     // how far each ring has got (screen heights; below 0 = no ring)

void fragment() {
	vec2 uv = SCREEN_UV;
	float wide = SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.x;      // width / height
	vec2 from_her = uv - centre;
	from_her.x *= wide;
	float far = length(from_her);
	vec2 outwards = far > 0.0001 ? from_her / far : vec2(0.0);

	// The rings: thin crests that fade as they spread. None right on top of her.
	float crest = 0.0;
	for (int i = 0; i < 8; i++) {
		float at = rings[i];
		if (at >= 0.0) {
			float x = (far - at) / ring_width;
			crest += exp(-x * x) * smoothstep(0.07, 0.15, at) / (1.0 + 1.6 * at);
		}
	}
	crest = min(crest, 1.2);

	vec2 push = outwards * crest * ring_bend * amount;
	push.x /= wide;
	vec2 look = uv + push;
	look += vec2(sin(look.y * 11.0 + TIME * 0.7), cos(look.x * 9.0 - TIME * 0.55)) * shimmer * amount;

	float pull = split * amount * (1.0 + 3.0 * crest);
	vec3 seen = vec3(
		texture(screen, look - vec2(pull, 0.0)).r,
		texture(screen, look).g,
		texture(screen, look + vec2(pull, 0.0)).b);

	float grey = dot(seen, vec3(0.299, 0.587, 0.114));
	vec3 frozen = mix(seen, vec3(grey), drain) * cold.rgb * lift;
	frozen *= 1.0 - corners * smoothstep(0.35, 0.85, distance(uv, vec2(0.5)));
	frozen += vec3(0.04, 0.09, 0.16) * min(crest, 1.0);

	COLOR = vec4(mix(texture(screen, uv).rgb, frozen, amount), 1.0);
}
