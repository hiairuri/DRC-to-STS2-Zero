// SilentFX - wraith wave (Wraith Form, played): the first wave of her scream. One front goes out from her mouth,
// round, and pushes the picture aside as it passes - the fight and, where this is drawn in front of everything,
// the game's own interface with it. It reads what is already on screen (the rectangle it is drawn on covers the
// screen; a BackBufferCopy is its child). Light only on the very crest of it, and faint. Driven from wraith_play.gd.
shader_type canvas_item;
render_mode unshaded;

uniform sampler2D screen : hint_screen_texture, filter_linear, repeat_disable;

uniform vec2 centre = vec2(0.5, 0.3);       // her mouth, 0..1 of the rectangle
uniform float aspect = 1.78;                // width / height of the rectangle
uniform float front = -1.0;                 // how far the wave has got (in heights of the rectangle)
uniform float width = 0.05;                 // how thick it is
uniform float strength = 0.011;             // how far it pushes what is behind it
uniform float power = 0.0;                  // 0 = the picture as it is
uniform vec3 tint : source_color = vec3(0.45, 1.0, 0.60);
uniform float glow = 0.05;

void fragment() {
	vec2 d = UV - centre;
	d.x *= aspect;
	float far = length(d);
	float x = (far - front) / max(width, 0.001);
	float ridge = exp(-x * x);
	// (just behind the crest the picture swings back once)
	float behind = clamp(-x, 0.0, 6.0);
	float bend = (ridge - 0.38 * sin(behind * 2.2) * exp(-behind * 0.85)) * power;
	vec2 dir = far > 0.0001 ? d / far : vec2(0.0);
	vec2 push = dir * (bend * strength);
	push.x /= aspect;
	vec3 seen = texture(screen, SCREEN_UV - push).rgb;
	seen += tint * (glow * ridge * power);
	COLOR = vec4(seen, 1.0);
}
