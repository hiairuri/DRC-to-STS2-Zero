shader_type canvas_item;
// 地上的一层雾（y0 以下渐浓）
uniform float t = 0.0; uniform float ws = 1.0;
uniform vec4 tint : source_color = vec4(0.30,0.42,0.42,1.0);
uniform float y0 = 420.0; uniform float y1 = 600.0; uniform float amt = 0.6;
varying vec2 wp;
void vertex(){ wp = (MODEL_MATRIX*vec4(VERTEX,0.0,1.0)).xy; }
float h(vec2 p){ return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
float n(vec2 p){ vec2 i=floor(p), f=fract(p); f=f*f*(3.0-2.0*f);
 return mix(mix(h(i),h(i+vec2(1,0)),f.x),mix(h(i+vec2(0,1)),h(i+vec2(1,1)),f.x),f.y); }
float fbm(vec2 p){ float v=0.0,a=0.5; for(int k=0;k<4;k++){ v+=a*n(p); p*=2.03; a*=0.5;} return v/0.9375; }
void fragment(){
 vec2 p = wp/(vec2(260.0,120.0)*ws) + vec2(t*0.05,0.0);
 float f = fbm(p + 0.6*vec2(fbm(p*1.3), fbm(p*1.3+4.1)));
 float band = smoothstep(y0, y1, wp.y);
 float a = smoothstep(0.38,0.85,f)*band*amt;
 COLOR = vec4(tint.rgb*(0.7+0.5*f), a);
}
