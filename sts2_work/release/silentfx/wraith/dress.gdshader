// SilentFX - wraith dress (Wraith Form): her dress and its roots stir, more the further down they are, and
// hang back as she rises and sinks; where they join her they breathe with her (see body.gdshader).
shader_type canvas_item;
uniform float phase = 0.0;       // 0..TAU, one loop
uniform float amount = 1.0;
uniform float hang = 0.0;        // she is rising (+) or sinking (-): the roots hang back, drawn out or gathered up
uniform vec2 origin = vec2(0.0);
uniform vec2 extent = vec2(1.0);
uniform vec2 chest = vec2(604.0, 690.0);
uniform float grow = 1.0;        // 0 = none of it yet, 1 = all: it grows down out of her
uniform float breath = 0.0;
uniform float swell = 0.06;
uniform float lift = 7.0;
varying vec4 tint;
vec2 breathe(vec2 p) {
	vec2 d = p - chest;
	float w = exp(-(d.x * d.x) / (170.0 * 170.0) - (d.y * d.y) / (200.0 * 200.0));
	p -= d * swell * breath * w;
	p.y += lift * breath * smoothstep(chest.y + 90.0, chest.y - 130.0, p.y);
	return p;
}
void vertex() {
	tint = COLOR;
}
void fragment() {
	vec2 uv = (breathe(origin + UV * extent) - origin) / extent;
	float low = smoothstep(0.12, 1.0, uv.y);
	float a = low * 0.011 * amount;
	uv.x += a * sin(phase + uv.y * 9.0) + a * 0.5 * sin(2.0 * phase - uv.y * 17.0 + uv.x * 6.0);
	uv.y += a * 0.45 * sin(phase + uv.x * 8.0 + 1.0);
	uv.y -= hang * 0.030 * low * low;
	uv.x += hang * 0.012 * low * low * sign(0.5 - uv.x) * -1.0;      // gathered up, they spread; drawn out, they close
	vec4 c = texture(TEXTURE, uv) * tint;
	float edge = grow * 1.25 - 0.20 + 0.05 * sin(UV.x * 37.0) + 0.03 * sin(UV.x * 91.0 + 2.0);
	c.a *= 1.0 - smoothstep(edge - 0.10, edge, UV.y);
	COLOR = c;
}
