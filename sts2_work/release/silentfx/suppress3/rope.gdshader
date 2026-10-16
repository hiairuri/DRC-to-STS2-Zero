shader_type canvas_item;
// 雾链：UV.x = 沿链的长度（场景像素，从后手量起），UV.y = 横向 0..1
uniform float t = 0.0; uniform float ws = 1.0;
uniform float flow = 0.0;          // 已经收走的长度：亮结往后手方向流（场景像素）
uniform float tension = 1.0;       // 0 松 → 1 绷紧
uniform float total = 400.0;       // 场景像素
uniform float pulse_pos = -999.0; uniform float pulse_amp = 0.0;   // 场景像素
uniform float knots = 1.0;
uniform float alpha_mul = 1.0;
uniform float tail_fade = 10.0;    // 末端淡出（场景像素）
uniform float head_fade = 26.0;    // 起端淡入（场景像素）
uniform float glow = 0.0;
float h(vec2 p){ return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
float n(vec2 p){ vec2 i=floor(p), f=fract(p); f=f*f*(3.0-2.0*f);
 return mix(mix(h(i),h(i+vec2(1,0)),f.x),mix(h(i+vec2(0,1)),h(i+vec2(1,1)),f.x),f.y); }
void fragment(){
 float S = UV.x; float s = S / ws; float v = UV.y*2.0 - 1.0;
 float loose = 1.0 - tension;
 float cw = mix(0.26, 0.40, loose);
 float core = exp(-pow(v/cw, 2.0));
 float strands = 0.0;
 for(int k=0;k<3;k++){
  float c = (0.42 + 0.25*loose) * sin(s*0.055 + float(k)*2.094 + t*2.6 + flow/ws*0.05);
  strands += exp(-pow((v - c)/0.13, 2.0)) * (0.55 + 0.45*n(vec2(s*0.04 - t*1.5, float(k)*7.0)));
 }
 float P = 30.0; float ph = (s + flow/ws)/P; float fr = fract(ph) - 0.5;
 float knot = exp(-pow(fr/0.16, 2.0)) * knots;
 float tip = -0.30 - 0.62*(1.0 - clamp(abs(fr)/0.17, 0.0, 1.0));
 float spike = smoothstep(tip - 0.12, tip + 0.2, v) * (1.0 - smoothstep(-0.32, -0.12, v)) * step(abs(fr), 0.17) * knots;
 float pulse = pulse_amp * exp(-pow((S - pulse_pos)/(40.0*ws), 2.0));
 float fl = 0.75 + 0.25*n(vec2(s*0.03 - t*2.0, 3.3));
 float halo = exp(-pow(v/0.85, 2.0));
 float a = (core*(0.45 + 0.25*tension) + halo*0.22 + strands*0.55 + knot*0.38*(0.5+0.5*tension)*halo + spike*0.2 + pulse*halo*1.2) * fl;
 a *= 0.7 + 0.3*n(vec2(s*0.08 + t*3.0, v*2.0));
 a *= smoothstep(0.0, max(head_fade, 1.0), S) * smoothstep(total, total - max(tail_fade, 1.0), S);
 a *= mix(0.65, 1.0, tension);
 vec3 col = mix(vec3(0.50,0.86,0.85), vec3(0.92,1.0,0.99), clamp(core*0.6 + knot*0.3 + pulse, 0.0, 1.0));
 col = mix(col, vec3(0.92,0.88,0.78), spike*0.35);
 a *= 1.0 + glow*0.8; col = mix(col, vec3(0.95,1.0,1.0), clamp(glow*0.5, 0.0, 0.8));
 COLOR = vec4(col, clamp(a, 0.0, 0.95) * alpha_mul);
}
