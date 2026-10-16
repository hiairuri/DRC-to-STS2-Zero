shader_type canvas_item;
// 冲击的放射速度线
uniform vec2 center = vec2(1000.0, 500.0); uniform float r0 = 60.0; uniform float r1 = 900.0; uniform float amt = 1.0; uniform float t = 0.0; uniform float ws = 1.0;
varying vec2 wp;
void vertex(){ wp = (MODEL_MATRIX*vec4(VERTEX,0.0,1.0)).xy; }
float h(float x){ return fract(sin(x*127.1)*43758.5453); }
void fragment(){
 vec2 d = wp - center; float dl = length(d);
 float ang = atan(d.y, d.x) / 6.28318 + 0.5;
 float bins = 150.0; float b = floor(ang*bins); float fb = fract(ang*bins);
 float on = step(0.62, h(b + 3.0));
 float wdt = mix(0.12, 0.38, h(b + 11.0));
 float line = on * (1.0 - smoothstep(wdt*0.5, wdt, abs(fb - 0.5)));
 float start = r0 + h(b + 7.0)*120.0*ws + t*900.0*ws;
 float len = mix(160.0, 520.0, h(b + 19.0))*ws;
 float radial = smoothstep(start, start + 40.0*ws, dl) * (1.0 - smoothstep(start + len*0.6, start + len, dl));
 radial *= 1.0 - smoothstep(r1*0.7, r1, dl);
 float a = line * radial * amt;
 COLOR = vec4(mix(vec3(0.55,1.0,0.85), vec3(1.0), 0.5*line), clamp(a, 0.0, 0.85));
}
