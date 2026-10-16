shader_type canvas_item;
// 她：半透明的魂身（边缘被雾吃掉、轻轻飘）＋拖影＋化雾＋散
uniform float dark = 0.72; uniform float ghost = 1.0; uniform float presence = 1.0; uniform float dissolve = 0.0; uniform float t = 0.0;
uniform float desat = 0.18; uniform vec3 cool = vec3(0.92,1.0,1.0);
uniform vec2 smear = vec2(0.0);     // 拖影（贴图像素）
uniform float mist = 0.0;           // 化成雾的程度
uniform float pxk = 1.0;            // 贴图像素 / 原型像素
uniform float ws = 1.0;             // 场景像素 / 原型像素
uniform vec2 light_at = vec2(960.0,300.0); uniform float light_amt = 1.0; uniform float light_r = 150.0;
uniform float feet_y = 548.0; uniform float fade = 70.0;
varying vec2 wp;
void vertex(){ wp = (MODEL_MATRIX*vec4(VERTEX,0.0,1.0)).xy; }
float hh(vec2 p){ return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
float nn(vec2 p){ vec2 i=floor(p), f=fract(p); f=f*f*(3.0-2.0*f);
 return mix(mix(hh(i),hh(i+vec2(1,0)),f.x),mix(hh(i+vec2(0,1)),hh(i+vec2(1,1)),f.x),f.y); }
void fragment(){
 vec2 q = wp / ws;
 float fl = nn(q*vec2(0.02,0.012) + vec2(0.0, t*0.7));
 vec2 wob = (vec2(nn(q*0.03 + vec2(t*0.5, 0.0)), nn(q*0.03 + vec2(4.0, t*0.5))) - 0.5) * TEXTURE_PIXEL_SIZE * 10.0 * pxk * ghost;
 vec4 c;
 if (dot(smear, smear) > 1.0) {
  vec3 acc = vec3(0.0); float aa = 0.0; float wsum = 0.0;
  for (int k = 0; k < 16; k++) {
   float s = float(k) / 15.0; float w = 1.0 - 0.6 * s;
   vec4 sc = texture(TEXTURE, UV + wob + smear * TEXTURE_PIXEL_SIZE * s);
   acc += sc.rgb * sc.a * w; aa += sc.a * w; wsum += w;
  }
  c = vec4(aa > 0.0001 ? acc / aa : vec3(0.0), aa / wsum);
 } else { c = texture(TEXTURE, UV + wob); }
 float edge = 0.0;
 for(int k=0;k<8;k++){ float an = 6.2832*float(k)/8.0; edge += texture(TEXTURE, UV + vec2(cos(an),sin(an))*TEXTURE_PIXEL_SIZE*9.0*pxk).a; }
 edge = 1.0 - edge/8.0;
 float l = dot(c.rgb, vec3(0.299,0.587,0.114));
 c.rgb = mix(c.rgb, vec3(l), desat) * dark * cool;
 float dl = length(wp - light_at);
 c.rgb += vec3(0.35,0.75,0.72)*exp(-dl/light_r)*0.2*light_amt*smoothstep(0.2,0.8,l+0.3);
 c.a *= 1.0 - smoothstep(feet_y - fade, feet_y, wp.y);
 c.rgb = mix(c.rgb, c.rgb*vec3(0.85,1.02,1.02) + vec3(0.03,0.07,0.07), ghost);
 c.rgb += vec3(0.25,0.45,0.44)*edge*ghost*0.6;
 c.a *= mix(1.0, (0.70 + 0.22*fl) * (1.0 - edge*0.65), ghost);
 float mf = nn(q*vec2(0.03,0.012) + vec2(0.0, t*2.0));
 c.rgb = mix(c.rgb, vec3(0.50,0.78,0.76)*(0.75 + 0.5*mf), mist*0.8);
 c.a *= mix(1.0, 0.55 + 0.6*mf, mist);
 c.a *= presence;
 float dn = nn(q*vec2(0.018,0.03) + vec2(0.0, t*1.2))*0.75 + nn(q*0.08 + vec2(t, 0.0))*0.25;
 c.a *= 1.0 - smoothstep(dn - 0.08, dn + 0.08, dissolve);
 c.rgb += vec3(0.30,0.55,0.52) * (smoothstep(dn - 0.25, dn, dissolve) - smoothstep(dn - 0.08, dn + 0.08, dissolve)) * 0.8;
 COLOR = c;
}
