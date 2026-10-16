// SilentFX - wraith body (Wraith Form), as she plays. Her body with its sleeves; her horns and what is below
// her elbows are their own pieces. Three things move in it, each "where a point of her goes" (the code has the
// same three for her hair and the pieces that ride on her), found here the other way round by a few steps:
//  - her breath: her chest swells and everything above it is lifted (it goes far past a breath when she howls);
//  - her head: thrown back (or bowed) about her neck - as a round thing it is, its face comes up and its crown
//    goes down and back;
//  - her sleeves: each swung a little about its shoulder.
shader_type canvas_item;
uniform vec2 origin = vec2(0.0);
uniform vec2 extent = vec2(1.0);
uniform vec2 chest = vec2(604.0, 690.0);
uniform float breath = 0.0;
uniform float swell = 0.06;
uniform float lift = 7.0;
uniform vec2 shoulder_l = vec2(444.0, 684.0);
uniform vec2 fist_l = vec2(286.0, 904.0);
uniform vec2 shoulder_r = vec2(764.0, 684.0);
uniform vec2 fist_r = vec2(972.0, 904.0);
uniform float sleeve_l = 0.0;        // radians
uniform float sleeve_r = 0.0;
uniform vec2 headc = vec2(604.0, 470.0);
uniform float headr = 160.0;
uniform float headz = 110.0;
uniform vec2 neck = vec2(604.0, 575.0);
uniform float tilt = 0.0;            // radians: + thrown back, - bowed
uniform float nod = 0.0;             // radians: turned in the plane of the picture
uniform float rise = 0.0;            // her neck drawn out
uniform float hscale = 1.0;          // her head nearer to us: bigger about her neck
varying vec4 tint;
vec2 turn(vec2 v, float a) {
	float c = cos(a);
	float s = sin(a);
	return vec2(v.x * c - v.y * s, v.x * s + v.y * c);
}
vec2 d_breath(vec2 p) {
	vec2 d = p - chest;
	float w = exp(-(d.x * d.x) / (170.0 * 170.0) - (d.y * d.y) / (200.0 * 200.0));
	vec2 q = d * swell * breath * w;
	q.y -= lift * breath * smoothstep(chest.y + 90.0, chest.y - 130.0, p.y);
	return q;
}
vec2 d_head(vec2 p) {
	vec2 d = p - headc;
	float r = length(d);
	float w = 1.0 - smoothstep(headr * 0.80, headr * 1.30, r);
	float z = headz * max(0.0, 1.0 - (r * r) / (headr * headr));
	float yr = neck.y - p.y;
	float y2 = yr * cos(tilt) + z * sin(tilt);
	float z2 = -yr * sin(tilt) + z * cos(tilt);
	vec2 q = vec2(neck.x + (p.x - neck.x) * (1.0 + z2 / 1600.0), neck.y - y2);
	q = neck + turn(q - neck, nod);
	q = neck + (q - neck) * hscale;
	q.y -= rise;
	return (q - p) * w;
}
vec2 d_sleeve(vec2 p, vec2 sh, vec2 fist, float ang) {
	vec2 a = fist - sh;
	float len = length(a);
	a /= len;
	vec2 d = p - sh;
	float t = dot(d, a) / len;
	float side = abs(d.x * a.y - d.y * a.x);
	float near = 1.0 - smoothstep(110.0, 170.0, side);
	float outer = fist.x < sh.x ? 1.0 - smoothstep(sh.x + 10.0, sh.x + 70.0, p.x) : smoothstep(sh.x - 70.0, sh.x - 10.0, p.x);
	return (sh + turn(d, ang * smoothstep(0.0, 0.5, t) * near * outer)) - p;
}
vec2 where(vec2 p) {
	return p + d_breath(p) + d_head(p) + d_sleeve(p, shoulder_l, fist_l, sleeve_l) + d_sleeve(p, shoulder_r, fist_r, sleeve_r);
}
void vertex() {
	tint = COLOR;
}
void fragment() {
	vec2 q = origin + UV * extent;
	vec2 p = q;
	for (int i = 0; i < 10; i++) {
		p -= 0.85 * (where(p) - q);
	}
	vec4 c = texture(TEXTURE, (p - origin) / extent) * tint;
	COLOR = c;
}
