// SilentFX - shadow blur (Shadow Step): the cards it has torn off the hand, seen as fast as they go.
// A rectangle a little bigger than one card, lying on it. It reads what is already on screen under it and
// draws it again softened - a little evenly, and more along the way the card is going - so that a card that
// is flung looks flung. Nothing of the card itself is touched: take this away and the card is as it was.
// It fades out towards its own rim, and is clear altogether when there is nothing to soften.
// Everything is driven from code (ShadowStepFx). Edit and press F8 to reload.
shader_type canvas_item;
render_mode unshaded;

uniform sampler2D screen : hint_screen_texture, filter_linear, repeat_disable;

uniform vec2 view = vec2(1920.0, 1080.0);   // the size of the screen, in the units everything below is given in
uniform vec2 smear = vec2(0.0);             // the way the card is going; its length is how far it is smeared that way
uniform float soft = 0.0;                   // how far it is softened evenly
uniform float rim = 0.12;                   // how much of the rectangle, from its edge inwards, it fades in over

float dither(vec2 p) {
	return fract(52.9829189 * fract(dot(p, vec2(0.06711056, 0.00583715))));
}

void fragment() {
	float len = length(smear);
	float most = max(len, soft);
	if (most < 0.05) {
		COLOR = vec4(0.0);
	} else {
		vec2 way = len > 0.001 ? smear / len : vec2(1.0, 0.0);
		vec2 side = vec2(-way.y, way.x);
		float long_s = max(len, soft);          // how far along its way
		float wide_s = soft;                    // how far across it
		vec2 px = SCREEN_UV * view;
		// (Each point starts its row of samples a little differently: the steps between them never show.)
		float j = dither(FRAGCOORD.xy) - 0.5;
		vec3 sum = vec3(0.0);
		float total = 0.0;
		for (int a = -3; a <= 3; a++) {
			float fa = (float(a) + j) / 3.0;        // -1..1: two widths of the bell either way
			float wa = exp(-2.0 * fa * fa);
			for (int b = -1; b <= 1; b++) {
				float fb = float(b);
				float w = wa * (b == 0 ? 1.0 : 0.55);
				vec2 at = px + way * (fa * 2.0 * long_s) + side * (fb * 1.1 * wide_s);
				sum += texture(screen, at / view).rgb * w;
				total += w;
			}
		}
		vec2 e = min(UV, 1.0 - UV);
		float edge = smoothstep(0.0, max(rim, 0.001), min(e.x, e.y));
		COLOR = vec4(sum / total, edge * clamp(most / 1.5, 0.0, 1.0));
	}
}
