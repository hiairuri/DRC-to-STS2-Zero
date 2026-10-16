// SilentFX - her howl (Wraith Form): a cone of light going up from her mouth (the bottom middle of the quad).
shader_type canvas_item;
render_mode blend_add;
uniform float alpha = 1.0;
uniform vec3 colour : source_color = vec3(0.5, 1.0, 0.55);
void fragment() {
	float h = 1.0 - UV.y;
	float halfw = 0.035 + 0.44 * h;
	float x = abs(UV.x - 0.5) / halfw;
	float v = (1.0 - smoothstep(0.0, 1.0, x)) * (1.0 - smoothstep(0.05, 1.0, h)) * smoothstep(0.0, 0.03, h);
	COLOR = vec4(colour * v * alpha, 1.0);
}
