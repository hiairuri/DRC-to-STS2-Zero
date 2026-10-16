// SilentFX - echo figure (Afterimage).
// A hooded silhouette made of light: one "afterimage" of the Silent.
// Fully procedural. Edit and press F8 to reload.
shader_type canvas_item;
render_mode blend_add, unshaded;

uniform float alpha = 0.3;       // driven from code
uniform float shift = 0.0;       // phase offset so the echoes do not flicker in sync
uniform vec3 tint : source_color = vec3(0.45, 0.80, 1.00);

void fragment() {
	vec2 uv = UV;
	float x = abs(uv.x - 0.5);

	// Hooded head.
	float headDist = length(vec2(x * 1.25, uv.y - 0.20));
	float head = 1.0 - smoothstep(0.13, 0.19, headDist);
	float headCore = 1.0 - smoothstep(0.07, 0.13, headDist);

	// Cloak: narrow at the shoulders, flaring towards the floor.
	float hw = mix(0.13, 0.36, smoothstep(0.22, 0.95, uv.y));
	float cloak = (1.0 - smoothstep(hw - 0.07, hw, x)) * smoothstep(0.20, 0.30, uv.y);
	float cloakCore = (1.0 - smoothstep(hw - 0.17, hw - 0.09, x)) * smoothstep(0.24, 0.34, uv.y);

	float shape = max(head, cloak);
	float core = max(headCore, cloakCore);
	float glow = shape - core * 0.55;          // brighter outline, dimmer inside

	float fade = 1.0 - smoothstep(0.78, 1.0, uv.y) * 0.75;
	float scan = 0.78 + 0.22 * sin(uv.y * 70.0 - TIME * 5.0 + shift);
	float flick = 0.90 + 0.10 * sin(TIME * 11.0 + shift * 3.0);

	COLOR = vec4(tint, clamp(glow * fade * scan * flick * alpha, 0.0, 1.0));
}
