shader_type canvas_item;
// 全局的绿雾：整个画面（连界面）都飘着；四边浓且暗、中间淡，手牌那一块再淡一点，牌上的字要能读。
// flow：雾往 flow_to 那一点被吸（蓄力）；shock：撕魂时以 shock_at 为圆心一圈冲击把雾推开。
uniform float t = 0.0; uniform float amt = 0.0; uniform float flash = 0.0; uniform float ws = 1.0;
uniform vec2 lo = vec2(0.0); uniform vec2 hi = vec2(1920.0, 1080.0);
uniform float hand_keep = 0.45;
uniform vec2 flow_to = vec2(1500.0, 500.0); uniform float flow = 0.0;
uniform vec2 shock_at = vec2(1000.0, 500.0); uniform float shock_r = -1.0; uniform float shock_amt = 0.0;
varying vec2 wp;
void vertex(){ wp = (MODEL_MATRIX*vec4(VERTEX,0.0,1.0)).xy; }
float h(vec2 p){ return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
float n(vec2 p){ vec2 i=floor(p), f=fract(p); f=f*f*(3.0-2.0*f);
 return mix(mix(h(i),h(i+vec2(1,0)),f.x),mix(h(i+vec2(0,1)),h(i+vec2(1,1)),f.x),f.y); }
float fbm(vec2 p){ float v=0.0,a=0.5; for(int k=0;k<4;k++){ v+=a*n(p); p*=2.03; a*=0.5;} return v/0.9375; }
void fragment(){
 vec2 uv = (wp - lo) / max(hi - lo, vec2(1.0));
 vec2 d = wp - flow_to; float dl = max(length(d), 1.0);
 vec2 pw = wp + d/dl * flow * smoothstep(0.0, 260.0*ws, dl);          // 雾往 flow_to 收
 vec2 q = pw/(vec2(320.0,190.0)*ws) + vec2(t*0.035, -t*0.012);
 float f = fbm(q + 0.7*vec2(fbm(q*1.2 + vec2(t*0.02, 3.1)), fbm(q*1.2 + 7.3)));
 float g = fbm(pw/(vec2(140.0,70.0)*ws) + vec2(t*0.05, t*0.22));
 float ex = abs(uv.x - 0.5)*2.0, ey = abs(uv.y - 0.5)*2.0;
 float edge = smoothstep(0.25, 1.05, max(ex*0.95, ey));
 float body = 0.30 + 0.70*edge;
 float hand = smoothstep(0.72, 0.86, uv.y) * (1.0 - smoothstep(0.18, 0.36, abs(uv.x - 0.5)));
 body *= mix(1.0, hand_keep, hand);
 float a = (smoothstep(0.30, 0.86, f)*0.85 + smoothstep(0.52, 0.9, g)*0.40) * body * amt;
 a += edge*edge*0.30*amt;                                              // 四边压暗的一圈
 vec3 col = mix(vec3(0.07,0.22,0.18), vec3(0.42,0.86,0.70), clamp(f*0.9 + g*0.3, 0.0, 1.0));
 col = mix(col, vec3(0.02,0.07,0.05), edge*0.55);
 if (shock_r > 0.0) {
  float ds = length(wp - shock_at);
  float inside = 1.0 - smoothstep(shock_r - 240.0*ws, shock_r - 20.0*ws, ds);
  a *= 1.0 - shock_amt*0.85*inside;
  float ring = exp(-pow((ds - shock_r)/(70.0*ws), 2.0)) * shock_amt;
  a += ring*0.45*(0.5 + 0.5*f);
  col = mix(col, vec3(0.75,1.0,0.9), ring*0.7);
 }
 a += flash * (0.12 + 0.30*f) * (0.6 + 0.4*edge);
 col = mix(col, vec3(0.80,1.0,0.92), clamp(flash, 0.0, 1.0)*0.6);
 COLOR = vec4(col, clamp(a, 0.0, 0.9));
}
