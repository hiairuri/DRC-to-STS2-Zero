shader_type canvas_item;
render_mode blend_add;
uniform float radius = 0.5;      // 0..1 of the quad's half size
uniform float width = 0.04;
uniform float alpha = 1.0;
uniform vec3 colour : source_color = vec3(0.6, 1.0, 0.55);
void fragment() {
	float r = length(UV - vec2(0.5)) * 2.0;
	float ring = 1.0 - smoothstep(0.0, width, abs(r - radius));
	float inner = (1.0 - smoothstep(0.0, radius, r)) * 0.10;
	COLOR = vec4(colour * (ring + inner) * alpha, 1.0);
}
