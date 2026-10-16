// SilentFX - cut_shift: the moment a clean cut shows (Precise Cut).
// A small rectangle over the target. It reads what is already on screen and slides the picture on one side of
// the cut against the picture on the other side, a few pixels along the cut: most in the middle of the cut,
// nothing at its two ends and nothing far from it, so that the seam itself is the only break.
// Everything is driven from code (PreciseCutFx). Edit and press F8 to reload.
shader_type canvas_item;
render_mode unshaded;

uniform sampler2D screen : hint_screen_texture, filter_linear, repeat_disable;

uniform vec2 view = vec2(1920.0, 1080.0);   // the size of the screen, in the units everything below is given in
uniform vec2 point = vec2(960.0, 540.0);    // the middle of the cut, on the screen
uniform vec2 dir = vec2(1.0, 0.0);          // along the cut (length 1)
uniform float half_len = 200.0;             // half the length of the cut
uniform float reach = 120.0;                // how far from the cut the picture is carried along
uniform float shift = 0.0;                  // how far the two sides have slid against each other, in all
uniform float band = 1.0;                   // how soft the seam is (0 = a hard break)

void fragment() {
	vec2 px = SCREEN_UV * view;
	vec2 rel = px - point;
	float along = dot(rel, dir);
	float across = dot(rel, vec2(-dir.y, dir.x));

	// How much of the slide this pixel takes part in.
	float ends = 1.0 - smoothstep(0.45, 1.0, abs(along) / max(half_len, 1.0));
	float near = 1.0 - smoothstep(0.30, 1.0, abs(across) / max(reach, 1.0));
	float part = ends * near;

	// One side goes one way, the other side the other way.
	float side = band > 0.001 ? smoothstep(-band, band, across) * 2.0 - 1.0 : sign(across);
	vec2 look = px - dir * (0.5 * shift * side * part);

	// Clear where nothing moves: the rectangle is only ever seen where it has something to show.
	COLOR = vec4(texture(screen, look / view).rgb, clamp(part * 24.0, 0.0, 1.0));
}
