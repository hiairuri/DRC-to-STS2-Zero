// namespace: SilentFX.Cards
// type: SuppressLook (class)
// source lines: 108857-108946
	internal static class SuppressLook
	{
		public static readonly Color Fog = new Color(0.36f, 0.47f, 0.46f, 1f);

		public static readonly Color Soul = new Color(0.5f, 0.78f, 0.74f, 1f);

		public static readonly Color Pale = new Color(0.8f, 0.95f, 0.92f, 1f);

		public static readonly Color Ash = new Color(0.3f, 0.33f, 0.35f, 1f);

		private const string Noise = "\nfloat hsh(vec2 p){ return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }\nfloat vn(vec2 p){ vec2 i = floor(p); vec2 f = fract(p); f = f * f * (3.0 - 2.0 * f);\n  return mix(mix(hsh(i), hsh(i + vec2(1.0, 0.0)), f.x), mix(hsh(i + vec2(0.0, 1.0)), hsh(i + vec2(1.0, 1.0)), f.x), f.y); }\nfloat fbm(vec2 p){ float s = 0.0; float a = 0.5; mat2 r = mat2(vec2(0.8, 0.6), vec2(-0.6, 0.8));\n  for (int k = 0; k < 5; k++){ s += a * vn(p); p = r * p * 2.03 + vec2(1.7, 9.2); a *= 0.5; } return s; }\nvarying vec4 vmod;\nvoid vertex(){ vmod = COLOR; }\n";

		private const string FieldCode = "shader_type canvas_item;\n\nuniform vec2 size = vec2(1920.0, 1080.0);\nuniform float t = 0.0;\nuniform float density = 0.0;\nuniform float veil = 0.0;\nuniform float floor_y = 800.0;\nuniform float front = 0.0;\nuniform vec4 tint : source_color = vec4(0.36, 0.47, 0.46, 1.0);\nuniform vec4 pale : source_color = vec4(0.80, 0.95, 0.92, 1.0);\nuniform vec2 sink = vec2(0.0);\nuniform float pull = 0.0;\nuniform vec2 wave_at = vec2(0.0);\nuniform float wave_r = -1.0;\nuniform float wave_k = 0.0;\n\nfloat hsh(vec2 p){ return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }\nfloat vn(vec2 p){ vec2 i = floor(p); vec2 f = fract(p); f = f * f * (3.0 - 2.0 * f);\n  return mix(mix(hsh(i), hsh(i + vec2(1.0, 0.0)), f.x), mix(hsh(i + vec2(0.0, 1.0)), hsh(i + vec2(1.0, 1.0)), f.x), f.y); }\nfloat fbm(vec2 p){ float s = 0.0; float a = 0.5; mat2 r = mat2(vec2(0.8, 0.6), vec2(-0.6, 0.8));\n  for (int k = 0; k < 5; k++){ s += a * vn(p); p = r * p * 2.03 + vec2(1.7, 9.2); a *= 0.5; } return s; }\nvarying vec4 vmod;\nvoid vertex(){ vmod = COLOR; }\n\nvoid fragment(){\n  vec2 px = UV * size;\n  vec2 d = px - sink; float dl = length(d) + 1.0;\n  float ang = pull * 2.2 * exp(-dl / 520.0);\n  float ca = cos(ang); float sa = sin(ang);\n  vec2 q = sink + vec2(ca * d.x - sa * d.y, sa * d.x + ca * d.y);\n  q += vec2(t * 22.0, -t * 6.0) * (front > 0.5 ? -1.2 : 1.0);\n  vec2 w = vec2(fbm(q * 0.0036 + vec2(0.0, t * 0.06)), fbm(q * 0.0036 + vec2(5.2, 1.3) - t * 0.05));\n  float f = fbm(q * 0.0029 + w * 1.9 + vec2(t * 0.02, 0.0));\n  float band = smoothstep(floor_y - 560.0, floor_y - 60.0, px.y) * (1.0 - smoothstep(floor_y + 80.0, floor_y + 300.0, px.y));\n  float edge = smoothstep(0.30, 0.0, UV.x) + smoothstep(0.70, 1.0, UV.x) + 0.6 * smoothstep(0.35, 0.0, UV.y);\n  float lowband = smoothstep(floor_y - 170.0, floor_y - 10.0, px.y) * (1.0 - smoothstep(floor_y + 40.0, floor_y + 200.0, px.y));\n  float shape = front > 0.5 ? lowband * 0.75 : (0.30 + 0.70 * band + 0.45 * edge);\n  float dens = density * smoothstep(0.40, 0.88, f) * shape;\n  dens *= 1.0 + pull * 0.9 * exp(-dl / 380.0);\n  float lit = 0.0;\n  if (wave_r > 0.0) {\n    float r = length(px - wave_at);\n    float inside = 1.0 - smoothstep(wave_r * 0.82, wave_r, r);\n    float z = (r - wave_r) / (40.0 + wave_r * 0.08);\n    float rim = exp(-z * z);\n    dens *= 1.0 - wave_k * 0.85 * inside;\n    dens += wave_k * 0.55 * rim * smoothstep(0.3, 0.8, f) * density;\n    lit = wave_k * rim;\n  }\n  vec2 c = UV - vec2(0.5, 0.45);\n  float dark = front > 0.5 ? 0.0 : veil * (0.22 + 0.9 * dot(c, c));\n  float a = clamp(dens * 0.78 + dark, 0.0, 0.92);\n  vec3 col = mix(tint.rgb, pale.rgb, clamp(smoothstep(0.72, 1.0, f) * 0.35 + lit * 0.6, 0.0, 1.0));\n  vec3 rgb = (col * dens * 0.78 + vec3(0.015, 0.02, 0.03) * dark) / max(a, 0.0001);\n  COLOR = vec4(rgb, a * vmod.a) * vec4(vmod.rgb, 1.0);\n}";

		private const string SoulCode = "shader_type canvas_item;\n\nuniform vec2 size = vec2(1920.0, 1080.0);\nuniform float t = 0.0;\nuniform vec4 tint : source_color = vec4(0.50, 0.78, 0.74, 1.0);\nuniform vec4 pale : source_color = vec4(0.80, 0.95, 0.92, 1.0);\nuniform vec2 a0; uniform vec2 b0; uniform vec2 c0; uniform vec4 p0 = vec4(1.0, 0.0, 0.0, 1.0);\nuniform vec2 a1; uniform vec2 b1; uniform vec2 c1; uniform vec4 p1 = vec4(1.0, 0.0, 0.0, 1.0);\nuniform vec2 a2; uniform vec2 b2; uniform vec2 c2; uniform vec4 p2 = vec4(1.0, 0.0, 0.0, 1.0);\nuniform vec2 a3; uniform vec2 b3; uniform vec2 c3; uniform vec4 p3 = vec4(1.0, 0.0, 0.0, 1.0);\nuniform float flow = 1.0;\n\nfloat hsh(vec2 p){ return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }\nfloat vn(vec2 p){ vec2 i = floor(p); vec2 f = fract(p); f = f * f * (3.0 - 2.0 * f);\n  return mix(mix(hsh(i), hsh(i + vec2(1.0, 0.0)), f.x), mix(hsh(i + vec2(0.0, 1.0)), hsh(i + vec2(1.0, 1.0)), f.x), f.y); }\nfloat fbm(vec2 p){ float s = 0.0; float a = 0.5; mat2 r = mat2(vec2(0.8, 0.6), vec2(-0.6, 0.8));\n  for (int k = 0; k < 5; k++){ s += a * vn(p); p = r * p * 2.03 + vec2(1.7, 9.2); a *= 0.5; } return s; }\nvarying vec4 vmod;\nvoid vertex(){ vmod = COLOR; }\n\nvec2 bz(vec2 a, vec2 b, vec2 c, float u){ float v = 1.0 - u; return a * v * v + b * 2.0 * v * u + c * u * u; }\nfloat curve(vec2 px, vec2 a, vec2 b, vec2 c, vec4 p, float seed){\n  if (p.y <= 0.001) return 0.0;\n  float best = 1e9; float bu = 0.0;\n  for (int i = 0; i <= 14; i++){\n    float u = float(i) / 14.0;\n    vec2 q = bz(a, b, c, u); float d = dot(px - q, px - q);\n    if (d < best){ best = d; bu = u; }\n  }\n  float lo = max(bu - 0.08, 0.0); float hi = min(bu + 0.08, 1.0);\n  for (int i = 0; i <= 6; i++){\n    float u = mix(lo, hi, float(i) / 6.0);\n    vec2 q = bz(a, b, c, u); float d = dot(px - q, px - q);\n    if (d < best){ best = d; bu = u; }\n  }\n  float dist = sqrt(best);\n  float w = p.x * (0.75 + 0.5 * sin(bu * 9.0 - t * 3.1 + seed) * 0.5 + 0.25 * vn(vec2(bu * 6.0 + seed, t * 0.7)));\n  float n = fbm(vec2(bu * 7.0 - t * 1.6 * flow + seed, dist / max(p.x, 1.0) * 1.3 + t * 0.4));\n  float core = exp(-pow(dist / max(w, 1.0), 2.0));\n  float wisp = exp(-pow(dist / max(w * 2.2, 1.0), 2.0)) * smoothstep(0.55, 0.85, n);\n  float span = smoothstep(p.z - 0.06, p.z + 0.04, bu) * (1.0 - smoothstep(p.w - 0.04, p.w + 0.06, bu));\n  return p.y * span * (core * (0.35 + 0.75 * n) + wisp * 0.6);\n}\nvoid fragment(){\n  vec2 px = UV * size;\n  vec2 j = vec2(fbm(px * 0.012 + vec2(t * 0.5, 0.0)), fbm(px * 0.012 + vec2(3.1, -t * 0.45))) - 0.5;\n  vec2 pj = px + j * 26.0;\n  float m = curve(pj, a0, b0, c0, p0, 0.0) + curve(pj, a1, b1, c1, p1, 3.7)\n          + curve(pj, a2, b2, c2, p2, 7.1) + curve(pj, a3, b3, c3, p3, 11.3);\n  float a = clamp(m * 0.8, 0.0, 0.9);\n  vec3 col = mix(tint.rgb, pale.rgb, clamp(m - 0.55, 0.0, 1.0));\n  COLOR = vec4(col, a * vmod.a) * vec4(vmod.rgb, 1.0);\n}";

		private const string FigureCode = "shader_type canvas_item;\n\nuniform sampler2D tb : filter_linear;\nuniform sampler2D tc : filter_linear;\nuniform sampler2D td : filter_linear;\nuniform vec4 w = vec4(1.0, 0.0, 0.0, 0.0);\nuniform float glow = 0.0;\nuniform vec4 pale : source_color = vec4(0.80, 0.95, 0.92, 1.0);\nvarying vec4 vmod;\nvoid vertex(){ vmod = COLOR; }\nvec4 pm(vec4 c){ return vec4(c.rgb * c.a, c.a); }\nvoid fragment(){\n  vec4 s = pm(texture(TEXTURE, UV)) * w.x + pm(texture(tb, UV)) * w.y + pm(texture(tc, UV)) * w.z + pm(texture(td, UV)) * w.w;\n  float a = clamp(s.a, 0.0, 1.0);\n  vec3 rgb = s.a > 0.0001 ? s.rgb / s.a : vec3(0.0);\n  rgb = mix(rgb, pale.rgb, clamp(glow, 0.0, 1.0) * 0.5);\n  COLOR = vec4(rgb * vmod.rgb, a * vmod.a);\n}";

		private static Shader _field;

		private static Shader _soul;

		private static Shader _figure;

		private static ShaderMaterial Make(ref Shader cache, string code, string name)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected Obj, but got Unknown
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Expected Obj, but got Unknown
			try
			{
				if (cache == null)
				{
					cache = new Shader
					{
						Code = code
					};
				}
				return new ShaderMaterial
				{
					Shader = cache
				};
			}
			catch (Exception ex)
			{
				DevLog.Warn("Suppress: " + name + " shader: " + ex.Message);
				return null;
			}
		}

		public static ShaderMaterial Field()
		{
			return Make(ref _field, "shader_type canvas_item;\n\nuniform vec2 size = vec2(1920.0, 1080.0);\nuniform float t = 0.0;\nuniform float density = 0.0;\nuniform float veil = 0.0;\nuniform float floor_y = 800.0;\nuniform float front = 0.0;\nuniform vec4 tint : source_color = vec4(0.36, 0.47, 0.46, 1.0);\nuniform vec4 pale : source_color = vec4(0.80, 0.95, 0.92, 1.0);\nuniform vec2 sink = vec2(0.0);\nuniform float pull = 0.0;\nuniform vec2 wave_at = vec2(0.0);\nuniform float wave_r = -1.0;\nuniform float wave_k = 0.0;\n\nfloat hsh(vec2 p){ return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }\nfloat vn(vec2 p){ vec2 i = floor(p); vec2 f = fract(p); f = f * f * (3.0 - 2.0 * f);\n  return mix(mix(hsh(i), hsh(i + vec2(1.0, 0.0)), f.x), mix(hsh(i + vec2(0.0, 1.0)), hsh(i + vec2(1.0, 1.0)), f.x), f.y); }\nfloat fbm(vec2 p){ float s = 0.0; float a = 0.5; mat2 r = mat2(vec2(0.8, 0.6), vec2(-0.6, 0.8));\n  for (int k = 0; k < 5; k++){ s += a * vn(p); p = r * p * 2.03 + vec2(1.7, 9.2); a *= 0.5; } return s; }\nvarying vec4 vmod;\nvoid vertex(){ vmod = COLOR; }\n\nvoid fragment(){\n  vec2 px = UV * size;\n  vec2 d = px - sink; float dl = length(d) + 1.0;\n  float ang = pull * 2.2 * exp(-dl / 520.0);\n  float ca = cos(ang); float sa = sin(ang);\n  vec2 q = sink + vec2(ca * d.x - sa * d.y, sa * d.x + ca * d.y);\n  q += vec2(t * 22.0, -t * 6.0) * (front > 0.5 ? -1.2 : 1.0);\n  vec2 w = vec2(fbm(q * 0.0036 + vec2(0.0, t * 0.06)), fbm(q * 0.0036 + vec2(5.2, 1.3) - t * 0.05));\n  float f = fbm(q * 0.0029 + w * 1.9 + vec2(t * 0.02, 0.0));\n  float band = smoothstep(floor_y - 560.0, floor_y - 60.0, px.y) * (1.0 - smoothstep(floor_y + 80.0, floor_y + 300.0, px.y));\n  float edge = smoothstep(0.30, 0.0, UV.x) + smoothstep(0.70, 1.0, UV.x) + 0.6 * smoothstep(0.35, 0.0, UV.y);\n  float lowband = smoothstep(floor_y - 170.0, floor_y - 10.0, px.y) * (1.0 - smoothstep(floor_y + 40.0, floor_y + 200.0, px.y));\n  float shape = front > 0.5 ? lowband * 0.75 : (0.30 + 0.70 * band + 0.45 * edge);\n  float dens = density * smoothstep(0.40, 0.88, f) * shape;\n  dens *= 1.0 + pull * 0.9 * exp(-dl / 380.0);\n  float lit = 0.0;\n  if (wave_r > 0.0) {\n    float r = length(px - wave_at);\n    float inside = 1.0 - smoothstep(wave_r * 0.82, wave_r, r);\n    float z = (r - wave_r) / (40.0 + wave_r * 0.08);\n    float rim = exp(-z * z);\n    dens *= 1.0 - wave_k * 0.85 * inside;\n    dens += wave_k * 0.55 * rim * smoothstep(0.3, 0.8, f) * density;\n    lit = wave_k * rim;\n  }\n  vec2 c = UV - vec2(0.5, 0.45);\n  float dark = front > 0.5 ? 0.0 : veil * (0.22 + 0.9 * dot(c, c));\n  float a = clamp(dens * 0.78 + dark, 0.0, 0.92);\n  vec3 col = mix(tint.rgb, pale.rgb, clamp(smoothstep(0.72, 1.0, f) * 0.35 + lit * 0.6, 0.0, 1.0));\n  vec3 rgb = (col * dens * 0.78 + vec3(0.015, 0.02, 0.03) * dark) / max(a, 0.0001);\n  COLOR = vec4(rgb, a * vmod.a) * vec4(vmod.rgb, 1.0);\n}", "field");
		}

		public static ShaderMaterial SoulMist()
		{
			return Make(ref _soul, "shader_type canvas_item;\n\nuniform vec2 size = vec2(1920.0, 1080.0);\nuniform float t = 0.0;\nuniform vec4 tint : source_color = vec4(0.50, 0.78, 0.74, 1.0);\nuniform vec4 pale : source_color = vec4(0.80, 0.95, 0.92, 1.0);\nuniform vec2 a0; uniform vec2 b0; uniform vec2 c0; uniform vec4 p0 = vec4(1.0, 0.0, 0.0, 1.0);\nuniform vec2 a1; uniform vec2 b1; uniform vec2 c1; uniform vec4 p1 = vec4(1.0, 0.0, 0.0, 1.0);\nuniform vec2 a2; uniform vec2 b2; uniform vec2 c2; uniform vec4 p2 = vec4(1.0, 0.0, 0.0, 1.0);\nuniform vec2 a3; uniform vec2 b3; uniform vec2 c3; uniform vec4 p3 = vec4(1.0, 0.0, 0.0, 1.0);\nuniform float flow = 1.0;\n\nfloat hsh(vec2 p){ return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }\nfloat vn(vec2 p){ vec2 i = floor(p); vec2 f = fract(p); f = f * f * (3.0 - 2.0 * f);\n  return mix(mix(hsh(i), hsh(i + vec2(1.0, 0.0)), f.x), mix(hsh(i + vec2(0.0, 1.0)), hsh(i + vec2(1.0, 1.0)), f.x), f.y); }\nfloat fbm(vec2 p){ float s = 0.0; float a = 0.5; mat2 r = mat2(vec2(0.8, 0.6), vec2(-0.6, 0.8));\n  for (int k = 0; k < 5; k++){ s += a * vn(p); p = r * p * 2.03 + vec2(1.7, 9.2); a *= 0.5; } return s; }\nvarying vec4 vmod;\nvoid vertex(){ vmod = COLOR; }\n\nvec2 bz(vec2 a, vec2 b, vec2 c, float u){ float v = 1.0 - u; return a * v * v + b * 2.0 * v * u + c * u * u; }\nfloat curve(vec2 px, vec2 a, vec2 b, vec2 c, vec4 p, float seed){\n  if (p.y <= 0.001) return 0.0;\n  float best = 1e9; float bu = 0.0;\n  for (int i = 0; i <= 14; i++){\n    float u = float(i) / 14.0;\n    vec2 q = bz(a, b, c, u); float d = dot(px - q, px - q);\n    if (d < best){ best = d; bu = u; }\n  }\n  float lo = max(bu - 0.08, 0.0); float hi = min(bu + 0.08, 1.0);\n  for (int i = 0; i <= 6; i++){\n    float u = mix(lo, hi, float(i) / 6.0);\n    vec2 q = bz(a, b, c, u); float d = dot(px - q, px - q);\n    if (d < best){ best = d; bu = u; }\n  }\n  float dist = sqrt(best);\n  float w = p.x * (0.75 + 0.5 * sin(bu * 9.0 - t * 3.1 + seed) * 0.5 + 0.25 * vn(vec2(bu * 6.0 + seed, t * 0.7)));\n  float n = fbm(vec2(bu * 7.0 - t * 1.6 * flow + seed, dist / max(p.x, 1.0) * 1.3 + t * 0.4));\n  float core = exp(-pow(dist / max(w, 1.0), 2.0));\n  float wisp = exp(-pow(dist / max(w * 2.2, 1.0), 2.0)) * smoothstep(0.55, 0.85, n);\n  float span = smoothstep(p.z - 0.06, p.z + 0.04, bu) * (1.0 - smoothstep(p.w - 0.04, p.w + 0.06, bu));\n  return p.y * span * (core * (0.35 + 0.75 * n) + wisp * 0.6);\n}\nvoid fragment(){\n  vec2 px = UV * size;\n  vec2 j = vec2(fbm(px * 0.012 + vec2(t * 0.5, 0.0)), fbm(px * 0.012 + vec2(3.1, -t * 0.45))) - 0.5;\n  vec2 pj = px + j * 26.0;\n  float m = curve(pj, a0, b0, c0, p0, 0.0) + curve(pj, a1, b1, c1, p1, 3.7)\n          + curve(pj, a2, b2, c2, p2, 7.1) + curve(pj, a3, b3, c3, p3, 11.3);\n  float a = clamp(m * 0.8, 0.0, 0.9);\n  vec3 col = mix(tint.rgb, pale.rgb, clamp(m - 0.55, 0.0, 1.0));\n  COLOR = vec4(col, a * vmod.a) * vec4(vmod.rgb, 1.0);\n}", "soul");
		}

		public static ShaderMaterial Figure()
		{
			return Make(ref _figure, "shader_type canvas_item;\n\nuniform sampler2D tb : filter_linear;\nuniform sampler2D tc : filter_linear;\nuniform sampler2D td : filter_linear;\nuniform vec4 w = vec4(1.0, 0.0, 0.0, 0.0);\nuniform float glow = 0.0;\nuniform vec4 pale : source_color = vec4(0.80, 0.95, 0.92, 1.0);\nvarying vec4 vmod;\nvoid vertex(){ vmod = COLOR; }\nvec4 pm(vec4 c){ return vec4(c.rgb * c.a, c.a); }\nvoid fragment(){\n  vec4 s = pm(texture(TEXTURE, UV)) * w.x + pm(texture(tb, UV)) * w.y + pm(texture(tc, UV)) * w.z + pm(texture(td, UV)) * w.w;\n  float a = clamp(s.a, 0.0, 1.0);\n  vec3 rgb = s.a > 0.0001 ? s.rgb / s.a : vec3(0.0);\n  rgb = mix(rgb, pale.rgb, clamp(glow, 0.0, 1.0) * 0.5);\n  COLOR = vec4(rgb * vmod.rgb, a * vmod.a);\n}", "figure");
		}

		public static ColorRect Screen(Node parent, Vector2 view, Material m)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			ColorRect val = FxKit.Quad(view, m);
			((Control)val).Position = Vector2.Zero;
			parent.AddChild((Node)(object)val, false, (InternalMode)0);
			return val;
		}

		static SuppressLook()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		}
	}
