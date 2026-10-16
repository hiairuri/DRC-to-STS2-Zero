// SilentFX - shadow shock (Shadow Step): the air it shoves aside when it goes off, and when it comes down.
// A rectangle just big enough to hold one ring. It reads what is already on screen and pushes it outwards
// where the ring is passing - nothing is drawn of its own but a breath of dusk on the crest. The ring may lie
// flat on the ground (it is then an oval), and only what is near it is touched: the rectangle is clear
// everywhere else, and clear altogether should the screen not be readable.
// Everything is driven from code (ShadowStepFx). Edit and press F8 to reload.
shader_type canvas_item;
render_mode unshaded;

uniform sampler2D screen : hint_screen_texture, filter_linear, repeat_disable;

uniform vec2 view = vec2(1920.0, 1080.0);   // the size of the screen, in the units everything below is given in
uniform vec2 centre = vec2(960.0, 540.0);   // where it went off, on the screen
uniform float radius = 0.0;                 // how far the ring has got
uniform float width = 70.0;                 // how thick the ring is
uniform float push = 0.0;                   // how far the crest shoves the picture outwards
uniform float lie = 1.0;                    // below 1 the ring lies on the ground: it is that much lower than wide
uniform float wake = 0.30;                  // the picture swinging back behind the crest (a share of the push)
uniform float split = 0.18;                 // red and blue pulled apart on the crest (a share of the push)
uniform float dusk = 0.0;                   // how much darker the crest is, 0..1
uniform vec3 tint : source_color = vec3(0.11, 0.05, 0.22);

void fragment() {
	vec2 px = SCREEN_UV * view;
	vec2 rel = px - centre;
	float squash = max(lie, 0.05);
	vec2 oval = vec2(rel.x, rel.y / squash);
	float far = length(oval);
	vec2 outwards = far > 0.001 ? oval / far : vec2(0.0);

	float x = (far - radius) / max(width, 1.0);
	float ridge = exp(-x * x);
	float behind = clamp(-x, 0.0, 6.0);
	float bend = ridge + wake * sin(behind * 2.4) * exp(-behind * 0.75);
	// (Its very middle is left alone: nothing is thrown about where it has only just gone off.)
	float open = smoothstep(0.0, 1.2, far / max(width, 1.0));

	vec2 shove = outwards * vec2(1.0, squash) * (bend * push * open);
	vec2 look = px - shove;
	vec2 pull = shove * split;
	vec3 seen = vec3(
		texture(screen, (look + pull) / view).r,
		texture(screen, look / view).g,
		texture(screen, (look - pull) / view).b);
	seen = mix(seen, tint, clamp(dusk * ridge * open, 0.0, 1.0));

	// Clear where nothing moves.
	float part = clamp(abs(bend) * open * abs(push) * 0.6, 0.0, 1.0);
	COLOR = vec4(seen, part);
}
