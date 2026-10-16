shader_type canvas_item;
// 她身上往上升的雾（从模糊的轮廓里冒出来）
uniform float t = 0.0; uniform float amt = 0.5; uniform float ws = 1.0;
uniform vec4 tint : source_color = vec4(0.55,0.75,0.74,1.0);
varying vec2 wp;
void vertex(){ wp = (MODEL_MATRIX*vec4(VERTEX,0.0,1.0)).xy; }
float h(vec2 p){ return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
float n(vec2 p){ vec2 i=floor(p), f=fract(p); f=f*f*(3.0-2.0*f);
 return mix(mix(h(i),h(i+vec2(1,0)),f.x),mix(h(i+vec2(0,1)),h(i+vec2(1,1)),f.x),f.y); }
float fbm(vec2 p){ float v=0.0,a=0.5; for(int k=0;k<5;k++){ v+=a*n(p); p*=2.07; a*=0.5;} return v; }
void fragment(){
 vec2 q = wp/(vec2(90.0,70.0)*ws) + vec2(0.0, t*0.55);
 float f = fbm(q + 0.8*vec2(fbm(q*1.4+1.3), fbm(q*1.4+7.7)));
 float m = texture(TEXTURE, UV + vec2((f-0.5)*0.03, 0.02 + (f-0.5)*0.04)).a;
 float a = smoothstep(0.08, 0.7, m) * smoothstep(0.35, 0.8, f) * amt;
 COLOR = vec4(tint.rgb*(0.75+0.45*f), a);
}
