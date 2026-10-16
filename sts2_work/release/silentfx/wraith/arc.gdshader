// SilentFX - the sound of her howl (Wraith Form): a part of a ring, about straight up, going out from her mouth.
shader_type canvas_item;
render_mode blend_add;
uniform float radius = 0.5;      // 0..1 of the quad's half size
uniform float width = 0.04;
uniform float alpha = 1.0;
uniform float spread = 1.0;      // radians to either side of straight up
uniform vec3 colour : source_color = vec3(0.6, 1.0, 0.55);
void fragment() {
	vec2 d = (UV - vec2(0.5)) * 2.0;
	float r = length(d);
	float a = atan(d.x, -d.y);
	float ring = 1.0 - smoothstep(0.0, width, abs(r - radius));
	float sect = 1.0 - smoothstep(spread * 0.5, spread, abs(a));
	COLOR = vec4(colour * ring * sect * alpha, 1.0);
}
