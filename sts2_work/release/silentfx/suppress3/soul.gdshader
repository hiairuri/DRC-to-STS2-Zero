shader_type canvas_item;
// 敌人的魂：他自己样子的模糊半透明复制，被攥住的那一点往外扯（头部刚性、脖子拉长），下端和身体若隐若现地连着
uniform sampler2D etex : filter_linear;
uniform float has_tex = 1.0;        // 0 = 没有他的样子，用一个按轮廓大小的魂影
uniform vec2 tpos; uniform vec2 tsize;   // 贴图在场景里的位置和大小
uniform vec2 epos; uniform vec2 esize;   // 敌人在场景里的框
uniform float ws = 1.0;
uniform vec2 pull = vec2(0.0, 0.0);
uniform float t = 0.0;
uniform vec2 head_at = vec2(1060.0, 290.0); uniform float grow = 1.45; uniform float rot = 0.0; uniform vec2 sdir = vec2(1.0, 0.0);
uniform float opacity = 1.0; uniform float stretch = 0.0; uniform float full = 0.0; uniform float full_w = 0.0;
uniform float blur = 6.0; uniform float lift_fade = 0.0;
uniform float glint = 0.0;          // 锁定时在他身上亮一下
uniform float vivid = 0.0;          // 0 = 在他身上隐约的魂影；1 = 拉出来以后，一眼看得出是他（他自己的颜色、亮边）
uniform float burst = 0.0;          // 0..1 魂炸开
varying vec2 wp;
void vertex(){ wp = (MODEL_MATRIX*vec4(VERTEX,0.0,1.0)).xy; }
float h(vec2 p){ return fract(sin(dot(p,vec2(127.1,311.7)))*43758.5453); }
float n(vec2 p){ vec2 i=floor(p), f=fract(p); f=f*f*(3.0-2.0*f);
 return mix(mix(h(i),h(i+vec2(1,0)),f.x),mix(h(i+vec2(0,1)),h(i+vec2(1,1)),f.x),f.y); }
float hgt(vec2 q){ return clamp((epos.y+esize.y - q.y)/esize.y, 0.0, 1.0); }
float wgt(float hh){ return mix(smoothstep(0.04,0.36,hh), 1.0, full_w); }
vec2 src(vec2 p){
 { vec2 r = p - head_at; float cs = cos(-rot), sn = sin(-rot); r = vec2(cs*r.x - sn*r.y, sn*r.x + cs*r.y);
   float rp = dot(r, sdir); r += sdir*(rp/(1.0+stretch) - rp); vec2 sp2 = vec2(-sdir.y, sdir.x); float rq = dot(r, sp2); r += sp2*(rq/(1.0-0.15*stretch) - rq);
   p = head_at + r; }
 float kk = mix(smoothstep(head_at.y + 150.0*ws, head_at.y + 10.0*ws, p.y), 1.0, full);
 p = head_at + (p - head_at)/mix(1.0, grow, kk);
 p = head_at + (p - head_at)/(1.0 + burst*1.6);
 float lo = epos.y - 900.0*ws, hi = epos.y + esize.y + 40.0*ws;
 for(int k=0;k<15;k++){ float m = 0.5*(lo+hi); float d = m + pull.y*wgt(hgt(vec2(0.0,m))); if(d < p.y) lo = m; else hi = m; }
 float sy = 0.5*(lo+hi); float w = wgt(hgt(vec2(0.0,sy)));
 vec2 q = vec2(p.x - pull.x*w, sy);
 float cx = epos.x + esize.x*0.5;
 q.x = cx + (q.x - cx)*(1.0 + 0.45*sin(3.14159*w)*step(w,0.97));
 return q;
}
// 他的样子：贴图里只取敌人框里的那一块（框外可能是别的敌人）
vec4 EIMG(vec2 q){
 vec2 eu = (q - epos)/esize;
 if(eu.x < -0.05 || eu.y < -0.05 || eu.x > 1.05 || eu.y > 1.02) return vec4(0.0);
 if(has_tex > 0.5){
  vec2 uv = (q - tpos)/tsize;
  if(uv.x<0.0||uv.y<0.0||uv.x>1.0||uv.y>1.0) return vec4(0.0);
  return texture(etex, uv);
 }
 // 魂影：一个上圆下收的影子，两个暗的眼窝
 vec2 c = (eu - vec2(0.5, 0.58)) / vec2(0.46, 0.44);
 float a = smoothstep(1.0, 0.72, length(c));
 vec3 col = vec3(0.55) + 0.35*(1.0 - eu.y);
 float eye = exp(-pow(length((eu - vec2(0.40,0.40))/vec2(0.05,0.07)),2.0)) + exp(-pow(length((eu - vec2(0.60,0.40))/vec2(0.05,0.07)),2.0));
 col *= 1.0 - 0.7*eye;
 return vec4(col, a);
}
void fragment(){
 vec2 wob = vec2(n(wp/ws*0.025 + vec2(0.0, t*0.6)) - 0.5, n(wp/ws*0.025 + vec2(5.2, t*0.6)) - 0.5) * 7.0*ws;
 vec2 q = src(wp + wob);
 vec4 e = vec4(0.0);
 for(int k=0;k<13;k++){ float an = 2.39996*float(k); float r = blur*ws*sqrt(float(k)/13.0); e += EIMG(q + vec2(cos(an),sin(an))*r); }
 e /= 13.0;
 float halo = 0.0;
 for(int k=0;k<8;k++){ float an = 6.2832*float(k)/8.0; halo += EIMG(q + vec2(cos(an),sin(an))*11.0*ws).a; }
 halo /= 8.0;
 float l = dot(e.rgb, vec3(0.299,0.587,0.114));
 vec3 c = mix(vec3(0.06,0.22,0.26), vec3(0.92,1.0,0.98), smoothstep(0.12,0.7,l));
 float w = wgt(hgt(q));
 vec2 dir = length(pull) > 0.01 ? normalize(pull) : vec2(0.0, -1.0);
 vec2 sp = vec2(dot(wp,dir), dot(wp,vec2(-dir.y,dir.x)))/ws;
 float streak = n(vec2(sp.x*0.02 - t*0.8, sp.y*0.22));
 float neck = (1.0-smoothstep(0.75,0.98,w))*smoothstep(0.02,0.2,w);
 float a = e.a*(0.35 + 0.6*smoothstep(0.1,0.6,w)) * mix(1.0, mix(0.45,1.0,streak), neck);
 a += halo*0.30*(1.0-e.a);
 float tear = smoothstep(0.0,0.35,w)*(1.0-smoothstep(0.35,0.7,w));
 a *= 1.0 - tear*smoothstep(0.45,0.75,streak)*0.8;
 vec3 keep = mix(c, e.rgb*vec3(0.75,1.05,1.1)+vec3(0.05,0.12,0.12), 0.32);
 float low = smoothstep(0.30, 1.0, clamp((q.y - epos.y)/esize.y,0.0,1.0));
 float wisp = n(vec2(wp.x/ws*0.05, wp.y/ws*0.012 + t*0.9));
 a *= mix(1.0, (1.0-low*0.7)*mix(0.45,1.0,wisp), lift_fade);
 a *= mix(1.0, 0.9, lift_fade);
 vec3 col = mix(vec3(0.55,0.95,0.92), keep, e.a*0.8);
 // 拉出来以后：他自己的颜色（提亮、带一点青）、实一点、一圈亮边
 vec3 own = e.rgb*vec3(0.88,1.08,1.08)*1.3 + vec3(0.05,0.13,0.12);
 own = mix(own, vec3(0.85,1.0,0.97), 0.15);
 float rim = clamp(halo - e.a*0.9, 0.0, 1.0);
 vec3 vcol = mix(own, vec3(0.62,1.0,0.92), clamp(rim*1.6, 0.0, 1.0));
 float va = e.a*(0.78 + 0.17*smoothstep(0.1,0.6,w)) * mix(1.0, mix(0.6,1.0,streak), neck*0.7) + rim*0.6;
 va *= 1.0 - tear*smoothstep(0.5,0.8,streak)*0.6;
 col = mix(col, vcol, vivid);
 a = mix(a, va, vivid);
 col = mix(col, vec3(0.9,1.0,0.98), glint*0.6);
 // 炸开：往外散、碎掉、变亮
 vec2 bq = (wp - head_at)/ws;
 float br = 0.55*n(bq*0.045 + vec2(t*1.5, 3.0)) + 0.30*n(bq*0.11 + vec2(7.0, t*2.0)) + 0.15*n(bq*0.27 + vec2(t*3.0, 1.0));
 br = mix(br, 1.0, 0.25*smoothstep(0.0, 1.0, length(bq)/(esize.x/ws + 60.0)));   // 外圈先碎
 a *= 1.0 - smoothstep(br - 0.22, br + 0.02, burst*1.2);
 col = mix(col, vec3(0.9,1.0,0.97), burst*0.35);
 COLOR = vec4(col, clamp(a*(1.0 + glint),0.0,0.95)*opacity);
}
