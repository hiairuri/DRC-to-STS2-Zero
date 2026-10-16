// SilentFX - wraith scream (Wraith Form's opening).
// The scream itself: drawn over the fight (under the hand and the buttons) for a few tenths of a
// second. It reads what is already on screen and lets two crests of bent air spread from the heart
// of the apparition (the upper middle of the screen) - what is behind a crest is dragged outwards,
// red and blue are pulled apart on it, and it carries a little green light. Where there is no crest
// the screen is left exactly as it is.
// Everything is driven from code. Edit and press F8 to reload.
shader_type canvas_item;
render_mode unshaded;

uniform sampler2D screen : hint_screen_texture, filter_linear, repeat_disable;

uniform float amount = 0.0;                 // 0 = the scene as it is
uniform vec2 centre = vec2(0.50, 0.37);     // where the scream comes from, 0..1 of the screen
uniform float reach = 0.0;                  // how far the first crest has got (screen heights)
uniform float ring_width = 0.085;           // in screen heights
uniform float bend = 0.050;                 // how far a crest pushes what is behind it
uniform float split = 0.0045;               // red and blue pulled apart on a crest (a fraction of the screen width)
uniform vec4 tint : source_color = vec4(0.50, 1.0, 0.42, 1.0);

void fragment() {
	vec2 uv = SCREEN_UV;
	float wide = SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.x;      // width / height
	vec2 from_it = uv - centre;
	from_it.x *= wide;
	float far = length(from_it);
	vec2 outwards = far > 0.0001 ? from_it / far : vec2(0.0);

	// Two crests, the second following the first. None right on top of its heart.
	float x1 = (far - reach) / ring_width;
	float x2 = (far - reach * 0.58) / (ring_width * 0.75);
	float crest = exp(-x1 * x1) + 0.55 * exp(-x2 * x2);
	crest *= smoothstep(0.03, 0.12, far) * amount;

	vec2 push = outwards * crest * bend;
	push.x /= wide;
	vec2 look = uv - push;
	float pull = split * crest;
	vec3 seen = vec3(
		texture(screen, look - vec2(pull, 0.0)).r,
		texture(screen, look).g,
		texture(screen, look + vec2(pull, 0.0)).b);
	seen += tint.rgb * crest * 0.10;

	COLOR = vec4(seen, 1.0);
}
