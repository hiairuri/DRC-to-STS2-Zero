// SilentFX - the fire on the banner of an Ancient card (Wraith Form), turned to ghost fire.
// The card's own flame, frame for frame: only its colour is changed. The game's flame is a pale blue-green
// with a white heart; here its body becomes the green of the card art's fire, its dim edge a dark green, and
// only its white heart a yellow-white.
//   green: 0 = the card's own fire, 1 = ghost fire.   power: how hard it burns (brightness, at green = 1).
shader_type canvas_item;
uniform float green : hint_range(0.0, 1.0) = 0.0;
uniform float power = 1.0;
uniform vec3 deep : source_color = vec3(0.02, 0.30, 0.09);
uniform vec3 mid : source_color = vec3(0.16, 0.92, 0.10);
uniform vec3 hot : source_color = vec3(0.80, 1.0, 0.45);
varying vec4 tint;
void vertex() {
	tint = COLOR;
}
void fragment() {
	vec4 c = texture(TEXTURE, UV);
	float body = max(c.r, max(c.g, c.b));
	float heart = min(c.r, min(c.g, c.b));
	vec3 g = mix(deep, mid, smoothstep(0.20, 0.70, body));
	g = mix(g, hot, smoothstep(0.55, 0.95, heart));
	vec3 rgb = mix(c.rgb, g * mix(1.0, power, smoothstep(0.3, 0.9, body)), green);
	COLOR = vec4(rgb, c.a) * tint;
}
