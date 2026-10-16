// SilentFX - wraith veil (Wraith Form): what of her is moving - her gauze as the wind takes it, the fire thrown off
// her blades - is drawn through a group of its own (a CanvasGroup; this is its material), so that it can be blurred
// a little as it goes: the faster a thing is, the less of its detail is seen. "blur" is how far, in pixels of the
// screen; it can be kept away from a stretch of the group's own space (her head, her body: what lies still on her
// stays sharp, what has been carried far from her is blurred most). With blur 0 it draws the group as a group is
// drawn. All of it is driven from wraith_play.gd / wraith_form.gd.
shader_type canvas_item;
render_mode unshaded;

uniform sampler2D screen_tex : hint_screen_texture, repeat_disable, filter_linear;
uniform float blur = 0.0;                   // pixels of the screen, where it is blurred most
uniform vec2 core_a = vec2(0.0);            // the stretch that is kept sharp (the group's own coordinates):
uniform vec2 core_b = vec2(0.0);            // from here to here,
uniform float near = 0.0;                   // ... nothing is blurred up to this far from it,
uniform float far = 0.0;                    // ... all of it from this far (far <= near: the same everywhere)
uniform float own = 1.0;                    // 1: the group's own colour and alpha are applied to it, as for any group;
                                            // 0: they are not (what it holds has been drawn with them already)
varying vec2 lp;

void vertex() {
	lp = VERTEX;
}

void fragment() {
	float k = 1.0;
	if (far > near) {
		vec2 ab = core_b - core_a;
		float ll = dot(ab, ab);
		float h = ll > 0.0001 ? clamp(dot(lp - core_a, ab) / ll, 0.0, 1.0) : 0.0;
		k = smoothstep(near, far, length(lp - core_a - ab * h));
	}
	float r = blur * k;
	vec4 c = textureLod(screen_tex, SCREEN_UV, 0.0);
	if (r > 0.3) {
		// (the buffer holds colour times alpha: it can be averaged as it is)
		vec2 px = SCREEN_PIXEL_SIZE * r;
		vec4 ring = textureLod(screen_tex, SCREEN_UV + px * vec2(0.86, 0.0), 0.0)
			+ textureLod(screen_tex, SCREEN_UV + px * vec2(-0.86, 0.0), 0.0)
			+ textureLod(screen_tex, SCREEN_UV + px * vec2(0.0, 0.86), 0.0)
			+ textureLod(screen_tex, SCREEN_UV + px * vec2(0.0, -0.86), 0.0);
		if (c.a + ring.a <= 0.0) {
			// (nothing of it anywhere near: the rest of the taps would find nothing either)
			c = vec4(0.0);
		} else {
			vec4 mid = textureLod(screen_tex, SCREEN_UV + px * vec2(0.30, 0.30), 0.0)
				+ textureLod(screen_tex, SCREEN_UV + px * vec2(-0.30, 0.30), 0.0)
				+ textureLod(screen_tex, SCREEN_UV + px * vec2(0.30, -0.30), 0.0)
				+ textureLod(screen_tex, SCREEN_UV + px * vec2(-0.30, -0.30), 0.0);
			vec4 diag = textureLod(screen_tex, SCREEN_UV + px * vec2(0.62, 0.62), 0.0)
				+ textureLod(screen_tex, SCREEN_UV + px * vec2(-0.62, 0.62), 0.0)
				+ textureLod(screen_tex, SCREEN_UV + px * vec2(0.62, -0.62), 0.0)
				+ textureLod(screen_tex, SCREEN_UV + px * vec2(-0.62, -0.62), 0.0);
			c = c * 0.16 + mid * 0.115 + (ring + diag) * 0.0475;
		}
	}
	if (c.a > 0.0001) {
		c.rgb /= c.a;
	}
	COLOR = own > 0.5 ? COLOR * c : c;
}
