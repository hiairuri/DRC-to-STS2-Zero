shader_type canvas_item;
// 撕魂那一下：从咬住点往外扩的一圈波纹，把后面的画面推得扭一下（连界面）
uniform sampler2D screen_tex : hint_screen_texture, filter_linear_mipmap;
uniform vec2 center = vec2(1000.0, 500.0); uniform float radius = 100.0; uniform float width = 60.0; uniform float amp = 18.0;
uniform vec2 span = vec2(1920.0, 1080.0);   // 屏幕在场景里有多大
varying vec2 wp;
void vertex(){ wp = (MODEL_MATRIX*vec4(VERTEX,0.0,1.0)).xy; }
void fragment(){
 vec2 d = wp - center; float dl = max(length(d), 1.0);
 float k = exp(-pow((dl - radius)/max(width, 1.0), 2.0));
 vec2 off = d/dl * amp * k;
 vec3 c = textureLod(screen_tex, SCREEN_UV - off/span, 0.0).rgb;
 c += vec3(0.30,0.62,0.52) * k * 0.18;
 COLOR = vec4(c, clamp(k*4.0, 0.0, 1.0));
}
