// SilentFX - shadow shade (Shadow Step): the light its roof takes away.
// While her shadow crosses the top of the window it leaves its mist lying all along its way - a roof of it. What
// is under that roof is in its shadow: this darkens the field between two heights of the window's width, most
// right under the roof and less at the ground, with soft sides. On the ground the shadow lies a little behind
// the roof (the light slants in under it) while it is moving.
// All of it is in the coordinates of the canvas, so the rectangle it is drawn on can be anywhere and any size.
// Driven from code (ShadowStepFx). Edit and press F8 in game to reload.
shader_type canvas_item;
render_mode blend_mix, unshaded;

uniform float from_x = 0.0;         // canvas px: the roof lies from here...
uniform float to_x = 0.0;           // ...to here
uniform float soft = 260.0;         // how far its shadow runs out to either side, px
uniform float deep = 1080.0;        // the height of the window, px
uniform float low = 0.62;           // how much of it is left at the ground (1 = as dark there as right under the roof)
uniform float lean = 0.0;           // how far its shadow on the ground lies behind it, px (either sign)
uniform float amount = 0.0;         // how dark, 0..1

uniform vec3 col_ink : source_color = vec3(0.02, 0.0, 0.05);

varying vec2 world;

void vertex() {
	world = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}

void fragment() {
	float lo = min(from_x, to_x);
	float hi = max(from_x, to_x);
	float h = clamp(world.y / max(deep, 1.0), 0.0, 1.0);
	float x = world.x - lean * h;
	float s = max(soft, 1.0);
	float a = smoothstep(lo - s, lo + s * 0.3, x) * (1.0 - smoothstep(hi - s * 0.3, hi + s, x));
	a *= mix(1.0, low, smoothstep(0.30, 1.0, h));
	COLOR = vec4(col_ink, clamp(a * amount, 0.0, 0.92));
}
