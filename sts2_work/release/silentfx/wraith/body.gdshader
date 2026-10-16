// SilentFX - wraith body (Wraith Form). Two things move in her body:
//  - she breathes: her chest swells out from its middle and everything above it - shoulders, head, horns -
//    is lifted a little (the same function is in the dress's shader and in the code that moves her hair);
//  - her arms: each swings a little about its shoulder and bends a little at its elbow, as of one who
//    stands with a blade in each hand, ready.
shader_type canvas_item;
uniform vec2 origin = vec2(0.0);     // this sprite's top left, in her own pixels
uniform vec2 extent = vec2(1.0);     // its size
uniform vec2 chest = vec2(604.0, 690.0);
uniform float breath = 0.0;          // 0 = out, 1 = in
uniform float swell = 0.06;
uniform float lift = 7.0;
uniform vec2 shoulder_l = vec2(444.0, 684.0);
uniform vec2 fist_l = vec2(286.0, 904.0);
uniform vec2 shoulder_r = vec2(764.0, 684.0);
uniform vec2 fist_r = vec2(972.0, 904.0);
uniform vec2 arm_l = vec2(0.0);      // radians: the whole arm about its shoulder, the forearm about its elbow
uniform vec2 arm_r = vec2(0.0);
uniform float hands = 1.0;        // 0 = no arms yet, 1 = all of them: they grow from her shoulders down to her fists
uniform float arms_off = 0.0;     // 0 = her arms are there, 1 = gone: they go from her shoulders down, her fists last
uniform float torso = 1.0;        // 0 = none of her body yet, 1 = all: it is wound on, turn by turn
uniform float head = 1.0;         // 0 = no head yet (it is under her hair), 1 = there
uniform float horn = 1.0;         // 0 = no horns yet, 1 = out: they are thrust out of her head
uniform vec2 headc = vec2(604.0, 430.0);
varying vec4 tint;
vec2 breathe(vec2 p) {               // where the pixel shown at p comes from
	vec2 d = p - chest;
	float w = exp(-(d.x * d.x) / (170.0 * 170.0) - (d.y * d.y) / (200.0 * 200.0));
	p -= d * swell * breath * w;
	p.y += lift * breath * smoothstep(chest.y + 90.0, chest.y - 130.0, p.y);
	return p;
}
vec2 turn(vec2 v, float a) {
	float c = cos(a);
	float s = sin(a);
	return vec2(v.x * c - v.y * s, v.x * s + v.y * c);
}
vec2 arm(vec2 p, vec2 sh, vec2 fist, vec2 ang) {
	vec2 a = fist - sh;
	float len = length(a);
	a /= len;
	vec2 d = p - sh;
	float t = dot(d, a) / len;                       // 0 at the shoulder, 1 at the fist
	float side = abs(d.x * a.y - d.y * a.x);          // how far off the arm's own line
	float near = 1.0 - smoothstep(80.0, 125.0, side);
	p = sh + turn(d, -ang.x * smoothstep(0.0, 0.5, t) * near);
	vec2 elbow = mix(sh, fist, 0.5);
	p = elbow + turn(p - elbow, -ang.y * smoothstep(0.45, 0.85, t) * near);
	return p;
}
float hash11(float x) {
	return fract(sin(x * 127.1) * 43758.5453);
}
float arm_zone(vec2 p, vec2 sh, vec2 fist) {      // 1 on (and near) an arm, 0 on the rest of her
	vec2 a = fist - sh;
	float len = length(a);
	a /= len;
	vec2 d = p - sh;
	float t = dot(d, a) / len;
	float side = abs(d.x * a.y - d.y * a.x);
	float outer = fist.x < sh.x ? 1.0 - smoothstep(sh.x, sh.x + 40.0, p.x) : smoothstep(sh.x - 40.0, sh.x, p.x);
	return (1.0 - smoothstep(150.0, 190.0, side)) * smoothstep(-0.06, 0.06, t) * outer;
}
float arm_shown(vec2 p, vec2 sh, vec2 fist) {     // how much of an arm has grown past p
	vec2 a = fist - sh;
	float len = length(a);
	a /= len;
	float t = dot(p - sh, a) / len;
	float edge = mix(-0.12, 1.50, hands);
	float off = mix(-0.30, 1.60, arms_off);
	return (1.0 - smoothstep(edge - 0.10, edge, t)) * smoothstep(off - 0.10, off, t);
}
float wound(vec2 p) {                             // her body, wound on in turns: one from the left, the next from the right
	if (torso >= 0.999) {
		return 1.0;
	}
	if (p.y < 612.0 && abs(p.x - chest.x) < 130.0 || p.y < 545.0) {
		return head;                                // (her head is not wound on: it comes up dark under her hair, once that is there)
	}
	float b = floor(p.y / 30.0);
	float start = hash11(b) * 0.55;
	float prog = clamp((torso - start) / 0.45, 0.0, 1.0);
	float xn = clamp((p.x - (chest.x - 340.0)) / 680.0, 0.0, 1.0);
	if (mod(b, 2.0) < 1.0) {
		xn = 1.0 - xn;
	}
	return 1.0 - smoothstep(prog * 1.10 - 0.10, prog * 1.10, xn);
}
float shown(vec2 p) {
	float zl = arm_zone(p, shoulder_l, fist_l);
	float zr = arm_zone(p, shoulder_r, fist_r);
	float body = wound(p);
	// her horns: out of her head, at the last
	float hz = (1.0 - smoothstep(480.0, 520.0, p.y)) * max(1.0 - smoothstep(535.0, 555.0, p.x), smoothstep(655.0, 675.0, p.x));
	float out_to = mix(40.0, 560.0, horn);
	body = mix(body, 1.0 - smoothstep(out_to - 30.0, out_to, length(p - headc)), hz);
	float a = mix(body, arm_shown(p, shoulder_l, fist_l), zl);
	return mix(a, arm_shown(p, shoulder_r, fist_r), zr);
}
void vertex() {
	tint = COLOR;
}
void fragment() {
	vec2 p = breathe(origin + UV * extent);
	p = arm(p, shoulder_l, fist_l, arm_l);
	p = arm(p, shoulder_r, fist_r, arm_r);
	vec4 c = texture(TEXTURE, (p - origin) / extent) * tint;
	c.a *= shown(p);
	COLOR = c;
}
