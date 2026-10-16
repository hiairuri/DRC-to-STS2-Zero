// SilentFX - Expose: what the white line does to the game's own icons as it crosses them.
// Put on an icon (or its number) for the moment the line is on it: with mode 0 whatever is below the line is
// gone (the gear of Artifact, the shield of Block: ground away from the bottom up); with mode 1 only what is
// below the line is there yet (Vulnerable: written in from the bottom up). Along the line itself the thing is
// white-hot. "heat" makes all of it so (it cools after the line has gone); "shake" rattles it sideways.
// The line's height is given in canvas coordinates (the same as GlobalPosition), so the icon can sit anywhere.
// Driven from code (BlueSkills.cs, ExposeWipe). Edit and press F8 in game to reload.
shader_type canvas_item;

uniform float cut_y = 1000000.0;    // where the line is (canvas y; y grows downward)
uniform float mode = 0.0;           // 0 = erase below the line .. 1 = show only below the line
uniform float band = 7.0;           // how far from the line it glows (canvas px)
uniform float heat = 0.0;           // 0..1: all of it white-hot
uniform float shake = 0.0;          // sideways rattle (local px)
uniform vec3 hot : source_color = vec3(1.0, 0.96, 0.86);

varying float world_y;

void vertex() {
	VERTEX.x += shake;
	world_y = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).y;
}

void fragment() {
	float below = step(cut_y, world_y);
	float keep = mix(1.0 - below, below, clamp(mode, 0.0, 1.0));
	float edge = (1.0 - smoothstep(0.0, max(band, 0.001), abs(world_y - cut_y))) * keep;
	vec4 c = COLOR;
	c.rgb = mix(c.rgb, hot, clamp(max(heat, edge), 0.0, 1.0));
	c.a *= keep;
	COLOR = c;
}
