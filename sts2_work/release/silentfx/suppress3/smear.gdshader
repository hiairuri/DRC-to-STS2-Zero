shader_type canvas_item;
// 手的拖影线：UV.x 从尾（0）量到头（total），尾细头粗
uniform float t = 0.0; uniform float total = 100.0; uniform float alpha_mul = 1.0; uniform float ws = 1.0;
float h(vec2 p){ return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
float n(vec2 p){ vec2 i=floor(p), f=fract(p); f=f*f*(3.0-2.0*f);
 return mix(mix(h(i),h(i+vec2(1,0)),f.x),mix(h(i+vec2(0,1)),h(i+vec2(1,1)),f.x),f.y); }
void fragment(){
 float s = clamp(UV.x / max(total, 1.0), 0.0, 1.0); float v = UV.y*2.0 - 1.0;
 float w = mix(0.22, 0.85, s);
 float lat = exp(-pow(v / w, 2.0));
 float streak = 0.5 + 0.5 * n(vec2(UV.x/ws*0.03 - t*6.0, v*4.0 + 3.0));
 float a = lat * smoothstep(0.0, 0.8, s) * (1.0 - smoothstep(0.9, 1.0, s)) * streak * alpha_mul;
 vec3 col = mix(vec3(0.40,0.66,0.64), vec3(0.75,0.95,0.92), lat * s * 0.45);
 COLOR = vec4(col, clamp(a, 0.0, 0.9));
}
