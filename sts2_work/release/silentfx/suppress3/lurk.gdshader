shader_type canvas_item;
// 敌人脚下翻涌的雾；burst>0 时往上炸开
uniform float t = 0.0; uniform vec2 center = vec2(1090.0, 545.0); uniform vec2 radius = vec2(175.0, 58.0);
uniform float amt = 0.8; uniform float burst = 0.0; uniform float beat = 0.0; uniform float ws = 1.0;
varying vec2 wp;
void vertex(){ wp = (MODEL_MATRIX*vec4(VERTEX,0.0,1.0)).xy; }
float h(vec2 p){ return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
float n(vec2 p){ vec2 i=floor(p), f=fract(p); f=f*f*(3.0-2.0*f);
 return mix(mix(h(i),h(i+vec2(1,0)),f.x),mix(h(i+vec2(0,1)),h(i+vec2(1,1)),f.x),f.y); }
float fbm(vec2 p){ float v=0.0,a=0.5; for(int k=0;k<5;k++){ v+=a*n(p); p*=2.03; a*=0.5;} return v; }
void fragment(){
 vec2 d = wp - center;
 float up = max(-d.y, 0.0);
 vec2 r = radius * (1.0 + 0.08*beat) * vec2(1.0 + 0.45*burst, 1.0 + 2.6*burst);
 d.y += up * 0.35 * (1.0 - burst);
 float e = length(d / r);
 vec2 q = wp/(vec2(70.0, 40.0)*ws) + vec2(t*0.25, -t*(0.6 + 2.5*burst));
 float f = fbm(q + 0.9*vec2(fbm(q*1.3 + 2.0), fbm(q*1.3 + 8.0)));
 float a = smoothstep(1.0, 0.2, e + (0.5 - f)*(0.8 + 0.6*burst)) * amt * pow(1.0 - burst, 1.4);
 vec3 col = mix(vec3(0.16,0.24,0.25), vec3(0.55,0.80,0.78), smoothstep(0.40, 0.85, f) * (0.45 + 0.35*burst + 0.35*beat));
 COLOR = vec4(col, clamp(a, 0.0, 0.92));
}
