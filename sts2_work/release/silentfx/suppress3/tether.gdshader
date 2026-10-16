shader_type canvas_item;
// 魂的下端和身体之间若隐若现的一缕
uniform vec2 top; uniform vec2 bot; uniform float width = 64.0; uniform float t = 0.0; uniform float amt = 0.45; uniform float ws = 1.0;
varying vec2 wp;
void vertex(){ wp = (MODEL_MATRIX*vec4(VERTEX,0.0,1.0)).xy; }
float h(vec2 p){ return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
float n(vec2 p){ vec2 i=floor(p), f=fract(p); f=f*f*(3.0-2.0*f);
 return mix(mix(h(i),h(i+vec2(1,0)),f.x),mix(h(i+vec2(0,1)),h(i+vec2(1,1)),f.x),f.y); }
float fbm(vec2 p){ float v=0.0,a=0.5; for(int k=0;k<4;k++){ v+=a*n(p); p*=2.1; a*=0.5;} return v; }
void fragment(){
 vec2 d = top - bot; float L = max(length(d), 1.0); vec2 u = d/L; vec2 v = vec2(-u.y,u.x);
 vec2 r = wp - bot; float sr = dot(r,u)/L; float s = clamp(sr, 0.0, 1.0); float off = dot(r,v);
 float wdt = width*(0.35 + 0.65*pow(abs(s-0.5)*2.0, 1.5));
 off += (fbm(vec2(s*3.0 - t*0.8, 1.7)) - 0.5)*28.0*ws;
 float core = exp(-pow(off/wdt, 2.0)*2.2);
 float str = fbm(vec2(off*0.05/ws, s*6.0 - t*1.6));
 float a = core*smoothstep(0.22,0.68,str)*amt;
 a *= smoothstep(0.0,0.12,s)*0.6 + 0.4;
 a *= smoothstep(-0.06,0.0,sr)*smoothstep(1.06,1.0,sr);
 COLOR = vec4(0.62,0.98,0.94, a);
}
