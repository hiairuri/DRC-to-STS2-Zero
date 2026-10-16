// namespace: SilentFX.Cards
// type: EchoingSlashFx (class)
// source lines: 49871-51410
	public sealed class EchoingSlashFx : CardFx
	{
		private enum Stage
		{
			Idle,
			Slash,
			Whirl,
			Fling,
			Unravel
		}

		private sealed class Foe
		{
			public object Key;

			public Vector2 Chest;

			public float Width = 200f;

			public float Height = 240f;

			public bool Dead;
		}

		private struct Owed
		{
			public Foe Foe;

			public float Reach;

			public float Way;

			public bool Killed;
		}

		private sealed class Arc
		{
			public Stage Stage;

			public float Time;

			public Vector2 Centre;

			public float Slope;

			public float SlopeGoal;

			public float Radius;

			public float Dir;

			public float Spread = 0.5f;

			public float Lead;

			public bool Both;

			public float Alpha;

			public float Erode;

			public float Wake = 80f;

			public float Seed;

			public float Speed;

			public float From;

			public float Near;

			public float Far;

			public float Plan = 0.2f;

			public float Power = 1.3f;

			public int Sides;

			public bool Landed;

			public float Hurry;

			public float Fade;

			public float Side = 1f;

			public bool Two;

			public float Spin = 1f;

			public float Hold = 80f;

			public float Dir0;

			public float Spread0;

			public float Delta;

			public readonly List<Owed> Cuts = new List<Owed>();
		}

		private sealed class Cast
		{
			public CardModel Card;

			public bool Multiplayer;

			public bool Remote;

			public Creature Owner;

			public Node OwnerNode;

			public Node Room;

			public object Invocation;

			public bool Closed;

			public Node Back;

			public Node Front;

			public Node2D HolderBack;

			public Node2D HolderFront;

			public ShaderMaterial MatBack;

			public ShaderMaterial MatFront;

			public Vector2 Centre;

			public float BodyWidth = 244f;

			public Vector2 View;

			public readonly List<Foe> Foes = new List<Foe>();

			public readonly List<Arc> Arcs = new List<Arc>();

			public readonly List<Arc> Queue = new List<Arc>();

			public readonly HashSet<object> Struck = new HashSet<object>();

			public Arc Hers;

			public Arc Source;

			public int Blow = -1;

			public float BlowAt;

			public float Gap = 0.2f;

			public float Age;

			public float Clock;

			public int Hits;

			public int Kills;

			public bool Acting;

			public int ActLeft;

			public float ActAt;

			public int ActKills;

			public int Forced;

			public readonly float[] X = new float[6];

			public readonly float[] Y = new float[6];

			public readonly float[] Cos = new float[6];

			public readonly float[] Sin = new float[6];

			public readonly float[] R = new float[6];

			public readonly float[] Dir = new float[6];

			public readonly float[] Spread = new float[6];

			public readonly float[] Lead = new float[6];

			public readonly float[] Both = new float[6];

			public readonly float[] Alpha = new float[6];

			public readonly float[] Erode = new float[6];

			public readonly float[] Wake = new float[6];

			public readonly float[] Seed = new float[6];
		}

		private static readonly Color Echo = new Color(0.62f, 0.8f, 1f, 0.85f);

		private const int Slots = 6;

		private const float Full = 5.2f;

		private const string HeadWarp = "// SilentFX - echo air (Echoing Slash), the kind that also bends what is behind it.\nshader_type canvas_item;\nrender_mode unshaded;\n\nuniform sampler2D screen : hint_screen_texture, filter_linear, repeat_disable;\n";

		private const string HeadPlain = "// SilentFX - echo air (Echoing Slash), light only.\nshader_type canvas_item;\nrender_mode blend_add, unshaded;\n";

		private const string Middle = "\n// Up to six arcs of sword wind. Each is a piece of a ring that lies in a level plane and is seen from the side:\n// on the screen an ellipse 'tilt' times as tall as it is wide, turned by a few degrees. The half of it that is\n// nearer than its middle is drawn by one rectangle (in front of everybody), the farther half by another (behind).\nuniform vec2 view = vec2(1920.0, 1080.0);     // the window in px: the rectangle covers exactly this\nuniform float tilt = 0.2;\nuniform float which_half = 1.0;               // +1 the near half, -1 the far half\nuniform float clock = 0.0;\nuniform vec4 tint : source_color = vec4(0.62, 0.80, 1.0, 1.0);\nuniform float strength = 0.95;\nuniform float edge = 3.4;                     // half the thickness of the leading lip, px\nuniform float halo = 20.0;                    // soft light round it, px\nuniform float feather = 30.0;                 // how far it trails away behind the lip, px\nuniform float ripple_gap = 24.0;              // the fainter lines behind the lip (the echo): how far apart, px\nuniform float ripple = 0.26;\nuniform float thin = 0.2;                     // how much of it is always missing: it is air, not a drawn line\nuniform float bend = 12.0;                    // how far it pushes what is behind it, px\n\nuniform float a_x[6];                         // its middle, px\nuniform float a_y[6];\nuniform float a_cos[6];                       // how its plane is turned\nuniform float a_sin[6];\nuniform float a_r[6];                         // how far it has got, px in its plane\nuniform float a_dir[6];                       // the middle of the piece of ring that is there (0 = to the right, a quarter turn = towards us)\nuniform float a_spread[6];                    // half its width, as an angle\nuniform float a_lead[6];                      // above 0 its bright end is the one turning forward, below 0 the other\nuniform float a_both[6];                      // 1 = the same piece again on the opposite side\nuniform float a_alpha[6];\nuniform float a_erode[6];                     // 0 whole .. 1 blown away\nuniform float a_wake[6];                      // how far the thin mist reaches back, px in its plane\nuniform float a_seed[6];\n\nconst float K_PI = 3.14159265;\nconst float K_TAU = 6.28318531;\n\nfloat hash21(vec2 p) {\n\tp = fract(p * vec2(123.34, 456.21));\n\tp += dot(p, p + 45.32);\n\treturn fract(p.x * p.y);\n}\n\nfloat vnoise(vec2 p) {\n\tvec2 i = floor(p);\n\tvec2 f = fract(p);\n\tvec2 u = f * f * (3.0 - 2.0 * f);\n\tfloat a = hash21(i);\n\tfloat b = hash21(i + vec2(1.0, 0.0));\n\tfloat c = hash21(i + vec2(0.0, 1.0));\n\tfloat d = hash21(i + vec2(1.0, 1.0));\n\treturn mix(mix(a, b, u.x), mix(c, d, u.x), u.y);\n}\n\nvoid fragment() {\n\tvec2 p = UV * view;\n\tfloat v = 0.0;\n\tfloat core = 0.0;\n\tvec2 push = vec2(0.0);\n\tfor (int i = 0; i < 6; i++) {\n\t\tfloat al = a_alpha[i];\n\t\tif (al > 0.002) {\n\t\t\tvec2 rel = p - vec2(a_x[i], a_y[i]);\n\t\t\tfloat cs = a_cos[i];\n\t\t\tfloat sn = a_sin[i];\n\t\t\t// Into its plane: q.y above 0 is nearer to us than its middle.\n\t\t\tvec2 q = vec2(rel.x * cs + rel.y * sn, (rel.y * cs - rel.x * sn) / tilt);\n\t\t\tfloat d = length(q) + 0.001;\n\t\t\tfloat big = a_r[i];\n\t\t\tfloat s = big - d;                          // above 0 behind the lip (inside the ring)\n\t\t\tif (s > -260.0 && s < a_wake[i] * 4.0 + 120.0) {\n\t\t\t\tfloat th = atan(q.y, q.x);\n\t\t\t\tfloat turn = mod(th - a_dir[i] + K_PI, K_TAU) - K_PI;\n\t\t\t\tfloat da = abs(turn);\n\t\t\t\tif (a_both[i] > 0.5) {\n\t\t\t\t\tda = min(da, K_PI - da);\n\t\t\t\t}\n\t\t\t\tfloat sp = max(a_spread[i], 0.01);\n\t\t\t\tfloat x = da / sp;                      // 0 the middle of the piece .. 1 its ends\n\t\t\t\tif (x < 1.0) {\n\t\t\t\t\tfloat side = smoothstep(-2.5, 2.5, which_half * q.y * tilt);\n\t\t\t\t\tvec2 n = vec2(q.x, q.y / tilt);     // across the lip, on the screen\n\t\t\t\t\tfloat nl = max(length(n), 0.0001);\n\t\t\t\t\tfloat g = nl / d;                   // px in the plane for one px on the screen, across the lip\n\t\t\t\t\tvec2 c = q / d;\n\t\t\t\t\tfloat seed = a_seed[i];\n\t\t\t\t\tfloat na = vnoise(c * 2.6 + vec2(seed, clock * 1.1));\n\t\t\t\t\tfloat nb = vnoise(c * 22.0 + vec2(seed * 3.1 + s / 360.0, -clock * 0.4));\n\t\t\t\t\tfloat nc = vnoise(c * 7.0 + vec2(seed * 1.7 + s / 900.0, clock * 0.3));\n\t\t\t\t\tfloat ne = 0.55 * vnoise(c * 3.3 + vec2(clock * 0.9, seed * 2.3)) + 0.45 * nb;\n\n\t\t\t\t\t// Thick in the middle, pointed at both ends; and brighter at the end that goes first.\n\t\t\t\t\tfloat tk = sqrt(max(1.0 - x * x, 0.0));\n\t\t\t\t\tfloat bright = 1.0 - smoothstep(0.62, 1.0, x);\n\t\t\t\t\tfloat along = 0.5 + 0.5 * turn / sp;\n\t\t\t\t\tfloat lead = a_lead[i];\n\t\t\t\t\tfloat hd = lead >= 0.0 ? along : 1.0 - along;\n\t\t\t\t\tbright *= mix(1.0, 0.10 + 0.90 * hd * hd, abs(lead));\n\n\t\t\t\t\t// The lip: a clean leading side, and behind it a feathered one that the noise combs out.\n\t\t\t\t\tfloat ss = s / g + (na - 0.5) * 3.0;\n\t\t\t\t\tfloat w_e = edge * (0.45 + 0.55 * tk);\n\t\t\t\t\tfloat w_f = feather * (0.20 + 0.80 * tk) * (0.35 + 1.25 * nb);\n\t\t\t\t\tfloat lip = ss < 0.0 ? exp(-(ss * ss) / (w_e * w_e)) : exp(-ss / w_f);\n\t\t\t\t\tfloat crest = exp(-(ss * ss) / (w_e * w_e * 0.8));\n\t\t\t\t\tfloat g1 = ripple_gap * (0.5 + 0.5 * tk);\n\t\t\t\t\tfloat r1 = exp(-((ss - g1) * (ss - g1)) / (w_e * w_e * 1.7)) * ripple;\n\t\t\t\t\tfloat r2 = exp(-((ss - 2.05 * g1) * (ss - 2.05 * g1)) / (w_e * w_e * 2.6)) * ripple * 0.5;\n\t\t\t\t\tfloat w_h = halo * (0.4 + 0.6 * tk);\n\t\t\t\t\tfloat h = exp(-(ss * ss) / (w_h * w_h));\n\t\t\t\t\tfloat back = max(s, 0.0);\n\t\t\t\t\tfloat ln = min(a_wake[i], big * 0.30) + 1.0;\n\t\t\t\t\tfloat mist = smoothstep(0.0, 8.0, ss) * exp(-back / ln) * nc * nc * (1.0 - smoothstep(0.20, 0.45, back / max(big, 1.0)));\n\t\t\t\t\t// Nothing of it reaches its own middle (the noise runs together there).\n\t\t\t\t\tfloat inner = smoothstep(0.30, 0.62, d / max(big, 1.0));\n\t\t\t\t\tfloat er = a_erode[i] + thin * (1.0 - a_erode[i]);\n\t\t\t\t\tfloat keep = smoothstep(er * 1.15 - 0.30, er * 1.15 + 0.05, ne);\n\t\t\t\t\tfloat w = bright * keep * side * al;\n\n\t\t\t\t\tfloat trail = ss < 0.0 ? lip : lip * inner;\n\t\t\t\t\tv += (trail * 0.62 + crest * 0.30 + (r1 + r2) * inner + h * 0.16 + mist * 0.22 * inner) * w;\n\t\t\t\t\tcore += crest * w;\n\t\t\t\t\t// What is behind it is pushed outwards on the lip and swings back just behind it.\n\t\t\t\t\tfloat ridge = h - 0.6 * exp(-((ss - w_h * 1.3) * (ss - w_h * 1.3)) / (w_h * w_h));\n\t\t\t\t\tvec2 nn = n / nl;\n\t\t\t\t\tpush += vec2(nn.x * cs - nn.y * sn, nn.x * sn + nn.y * cs) * (ridge * w);\n\t\t\t\t}\n\t\t\t}\n\t\t}\n\t}\n\tvec3 rgb = mix(tint.rgb, vec3(1.0), clamp(core * 0.8 - 0.10, 0.0, 0.70));\n";

		private const string TailWarp = "\t// Clear where there is nothing of it: the rectangle is only ever seen where it has something to show.\n\tvec2 shift = push * bend;\n\tfloat cover = clamp(max(v, length(push)) * 24.0, 0.0, 1.0);\n\tvec3 seen = texture(screen, SCREEN_UV - shift / view).rgb;\n\tCOLOR = vec4(seen + rgb * (min(v, 1.2) * strength), cover);\n}\n";

		private const string TailPlain = "\tCOLOR = vec4(rgb, clamp(v, 0.0, 1.0) * strength);\n}\n";

		private static readonly List<Cast> Live = new List<Cast>();

		private static readonly ConditionalWeakTable<object, object> MultiplayerInvocations = new ConditionalWeakTable<object, object>();

		private static Shader _warp;

		private static Shader _plain;

		private static bool _ticking;

		private static bool _warned;

		private static int _seeds;

		internal static void Warm()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected Obj, but got Unknown
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected Obj, but got Unknown
			if (!FxKit.Alive((GodotObject)(object)_warp))
			{
				_warp = new Shader
				{
					Code = "// SilentFX - echo air (Echoing Slash), the kind that also bends what is behind it.\nshader_type canvas_item;\nrender_mode unshaded;\n\nuniform sampler2D screen : hint_screen_texture, filter_linear, repeat_disable;\n\n// Up to six arcs of sword wind. Each is a piece of a ring that lies in a level plane and is seen from the side:\n// on the screen an ellipse 'tilt' times as tall as it is wide, turned by a few degrees. The half of it that is\n// nearer than its middle is drawn by one rectangle (in front of everybody), the farther half by another (behind).\nuniform vec2 view = vec2(1920.0, 1080.0);     // the window in px: the rectangle covers exactly this\nuniform float tilt = 0.2;\nuniform float which_half = 1.0;               // +1 the near half, -1 the far half\nuniform float clock = 0.0;\nuniform vec4 tint : source_color = vec4(0.62, 0.80, 1.0, 1.0);\nuniform float strength = 0.95;\nuniform float edge = 3.4;                     // half the thickness of the leading lip, px\nuniform float halo = 20.0;                    // soft light round it, px\nuniform float feather = 30.0;                 // how far it trails away behind the lip, px\nuniform float ripple_gap = 24.0;              // the fainter lines behind the lip (the echo): how far apart, px\nuniform float ripple = 0.26;\nuniform float thin = 0.2;                     // how much of it is always missing: it is air, not a drawn line\nuniform float bend = 12.0;                    // how far it pushes what is behind it, px\n\nuniform float a_x[6];                         // its middle, px\nuniform float a_y[6];\nuniform float a_cos[6];                       // how its plane is turned\nuniform float a_sin[6];\nuniform float a_r[6];                         // how far it has got, px in its plane\nuniform float a_dir[6];                       // the middle of the piece of ring that is there (0 = to the right, a quarter turn = towards us)\nuniform float a_spread[6];                    // half its width, as an angle\nuniform float a_lead[6];                      // above 0 its bright end is the one turning forward, below 0 the other\nuniform float a_both[6];                      // 1 = the same piece again on the opposite side\nuniform float a_alpha[6];\nuniform float a_erode[6];                     // 0 whole .. 1 blown away\nuniform float a_wake[6];                      // how far the thin mist reaches back, px in its plane\nuniform float a_seed[6];\n\nconst float K_PI = 3.14159265;\nconst float K_TAU = 6.28318531;\n\nfloat hash21(vec2 p) {\n\tp = fract(p * vec2(123.34, 456.21));\n\tp += dot(p, p + 45.32);\n\treturn fract(p.x * p.y);\n}\n\nfloat vnoise(vec2 p) {\n\tvec2 i = floor(p);\n\tvec2 f = fract(p);\n\tvec2 u = f * f * (3.0 - 2.0 * f);\n\tfloat a = hash21(i);\n\tfloat b = hash21(i + vec2(1.0, 0.0));\n\tfloat c = hash21(i + vec2(0.0, 1.0));\n\tfloat d = hash21(i + vec2(1.0, 1.0));\n\treturn mix(mix(a, b, u.x), mix(c, d, u.x), u.y);\n}\n\nvoid fragment() {\n\tvec2 p = UV * view;\n\tfloat v = 0.0;\n\tfloat core = 0.0;\n\tvec2 push = vec2(0.0);\n\tfor (int i = 0; i < 6; i++) {\n\t\tfloat al = a_alpha[i];\n\t\tif (al > 0.002) {\n\t\t\tvec2 rel = p - vec2(a_x[i], a_y[i]);\n\t\t\tfloat cs = a_cos[i];\n\t\t\tfloat sn = a_sin[i];\n\t\t\t// Into its plane: q.y above 0 is nearer to us than its middle.\n\t\t\tvec2 q = vec2(rel.x * cs + rel.y * sn, (rel.y * cs - rel.x * sn) / tilt);\n\t\t\tfloat d = length(q) + 0.001;\n\t\t\tfloat big = a_r[i];\n\t\t\tfloat s = big - d;                          // above 0 behind the lip (inside the ring)\n\t\t\tif (s > -260.0 && s < a_wake[i] * 4.0 + 120.0) {\n\t\t\t\tfloat th = atan(q.y, q.x);\n\t\t\t\tfloat turn = mod(th - a_dir[i] + K_PI, K_TAU) - K_PI;\n\t\t\t\tfloat da = abs(turn);\n\t\t\t\tif (a_both[i] > 0.5) {\n\t\t\t\t\tda = min(da, K_PI - da);\n\t\t\t\t}\n\t\t\t\tfloat sp = max(a_spread[i], 0.01);\n\t\t\t\tfloat x = da / sp;                      // 0 the middle of the piece .. 1 its ends\n\t\t\t\tif (x < 1.0) {\n\t\t\t\t\tfloat side = smoothstep(-2.5, 2.5, which_half * q.y * tilt);\n\t\t\t\t\tvec2 n = vec2(q.x, q.y / tilt);     // across the lip, on the screen\n\t\t\t\t\tfloat nl = max(length(n), 0.0001);\n\t\t\t\t\tfloat g = nl / d;                   // px in the plane for one px on the screen, across the lip\n\t\t\t\t\tvec2 c = q / d;\n\t\t\t\t\tfloat seed = a_seed[i];\n\t\t\t\t\tfloat na = vnoise(c * 2.6 + vec2(seed, clock * 1.1));\n\t\t\t\t\tfloat nb = vnoise(c * 22.0 + vec2(seed * 3.1 + s / 360.0, -clock * 0.4));\n\t\t\t\t\tfloat nc = vnoise(c * 7.0 + vec2(seed * 1.7 + s / 900.0, clock * 0.3));\n\t\t\t\t\tfloat ne = 0.55 * vnoise(c * 3.3 + vec2(clock * 0.9, seed * 2.3)) + 0.45 * nb;\n\n\t\t\t\t\t// Thick in the middle, pointed at both ends; and brighter at the end that goes first.\n\t\t\t\t\tfloat tk = sqrt(max(1.0 - x * x, 0.0));\n\t\t\t\t\tfloat bright = 1.0 - smoothstep(0.62, 1.0, x);\n\t\t\t\t\tfloat along = 0.5 + 0.5 * turn / sp;\n\t\t\t\t\tfloat lead = a_lead[i];\n\t\t\t\t\tfloat hd = lead >= 0.0 ? along : 1.0 - along;\n\t\t\t\t\tbright *= mix(1.0, 0.10 + 0.90 * hd * hd, abs(lead));\n\n\t\t\t\t\t// The lip: a clean leading side, and behind it a feathered one that the noise combs out.\n\t\t\t\t\tfloat ss = s / g + (na - 0.5) * 3.0;\n\t\t\t\t\tfloat w_e = edge * (0.45 + 0.55 * tk);\n\t\t\t\t\tfloat w_f = feather * (0.20 + 0.80 * tk) * (0.35 + 1.25 * nb);\n\t\t\t\t\tfloat lip = ss < 0.0 ? exp(-(ss * ss) / (w_e * w_e)) : exp(-ss / w_f);\n\t\t\t\t\tfloat crest = exp(-(ss * ss) / (w_e * w_e * 0.8));\n\t\t\t\t\tfloat g1 = ripple_gap * (0.5 + 0.5 * tk);\n\t\t\t\t\tfloat r1 = exp(-((ss - g1) * (ss - g1)) / (w_e * w_e * 1.7)) * ripple;\n\t\t\t\t\tfloat r2 = exp(-((ss - 2.05 * g1) * (ss - 2.05 * g1)) / (w_e * w_e * 2.6)) * ripple * 0.5;\n\t\t\t\t\tfloat w_h = halo * (0.4 + 0.6 * tk);\n\t\t\t\t\tfloat h = exp(-(ss * ss) / (w_h * w_h));\n\t\t\t\t\tfloat back = max(s, 0.0);\n\t\t\t\t\tfloat ln = min(a_wake[i], big * 0.30) + 1.0;\n\t\t\t\t\tfloat mist = smoothstep(0.0, 8.0, ss) * exp(-back / ln) * nc * nc * (1.0 - smoothstep(0.20, 0.45, back / max(big, 1.0)));\n\t\t\t\t\t// Nothing of it reaches its own middle (the noise runs together there).\n\t\t\t\t\tfloat inner = smoothstep(0.30, 0.62, d / max(big, 1.0));\n\t\t\t\t\tfloat er = a_erode[i] + thin * (1.0 - a_erode[i]);\n\t\t\t\t\tfloat keep = smoothstep(er * 1.15 - 0.30, er * 1.15 + 0.05, ne);\n\t\t\t\t\tfloat w = bright * keep * side * al;\n\n\t\t\t\t\tfloat trail = ss < 0.0 ? lip : lip * inner;\n\t\t\t\t\tv += (trail * 0.62 + crest * 0.30 + (r1 + r2) * inner + h * 0.16 + mist * 0.22 * inner) * w;\n\t\t\t\t\tcore += crest * w;\n\t\t\t\t\t// What is behind it is pushed outwards on the lip and swings back just behind it.\n\t\t\t\t\tfloat ridge = h - 0.6 * exp(-((ss - w_h * 1.3) * (ss - w_h * 1.3)) / (w_h * w_h));\n\t\t\t\t\tvec2 nn = n / nl;\n\t\t\t\t\tpush += vec2(nn.x * cs - nn.y * sn, nn.x * sn + nn.y * cs) * (ridge * w);\n\t\t\t\t}\n\t\t\t}\n\t\t}\n\t}\n\tvec3 rgb = mix(tint.rgb, vec3(1.0), clamp(core * 0.8 - 0.10, 0.0, 0.70));\n\t// Clear where there is nothing of it: the rectangle is only ever seen where it has something to show.\n\tvec2 shift = push * bend;\n\tfloat cover = clamp(max(v, length(push)) * 24.0, 0.0, 1.0);\n\tvec3 seen = texture(screen, SCREEN_UV - shift / view).rgb;\n\tCOLOR = vec4(seen + rgb * (min(v, 1.2) * strength), cover);\n}\n"
				};
			}
			if (!FxKit.Alive((GodotObject)(object)_plain))
			{
				_plain = new Shader
				{
					Code = "// SilentFX - echo air (Echoing Slash), light only.\nshader_type canvas_item;\nrender_mode blend_add, unshaded;\n\n// Up to six arcs of sword wind. Each is a piece of a ring that lies in a level plane and is seen from the side:\n// on the screen an ellipse 'tilt' times as tall as it is wide, turned by a few degrees. The half of it that is\n// nearer than its middle is drawn by one rectangle (in front of everybody), the farther half by another (behind).\nuniform vec2 view = vec2(1920.0, 1080.0);     // the window in px: the rectangle covers exactly this\nuniform float tilt = 0.2;\nuniform float which_half = 1.0;               // +1 the near half, -1 the far half\nuniform float clock = 0.0;\nuniform vec4 tint : source_color = vec4(0.62, 0.80, 1.0, 1.0);\nuniform float strength = 0.95;\nuniform float edge = 3.4;                     // half the thickness of the leading lip, px\nuniform float halo = 20.0;                    // soft light round it, px\nuniform float feather = 30.0;                 // how far it trails away behind the lip, px\nuniform float ripple_gap = 24.0;              // the fainter lines behind the lip (the echo): how far apart, px\nuniform float ripple = 0.26;\nuniform float thin = 0.2;                     // how much of it is always missing: it is air, not a drawn line\nuniform float bend = 12.0;                    // how far it pushes what is behind it, px\n\nuniform float a_x[6];                         // its middle, px\nuniform float a_y[6];\nuniform float a_cos[6];                       // how its plane is turned\nuniform float a_sin[6];\nuniform float a_r[6];                         // how far it has got, px in its plane\nuniform float a_dir[6];                       // the middle of the piece of ring that is there (0 = to the right, a quarter turn = towards us)\nuniform float a_spread[6];                    // half its width, as an angle\nuniform float a_lead[6];                      // above 0 its bright end is the one turning forward, below 0 the other\nuniform float a_both[6];                      // 1 = the same piece again on the opposite side\nuniform float a_alpha[6];\nuniform float a_erode[6];                     // 0 whole .. 1 blown away\nuniform float a_wake[6];                      // how far the thin mist reaches back, px in its plane\nuniform float a_seed[6];\n\nconst float K_PI = 3.14159265;\nconst float K_TAU = 6.28318531;\n\nfloat hash21(vec2 p) {\n\tp = fract(p * vec2(123.34, 456.21));\n\tp += dot(p, p + 45.32);\n\treturn fract(p.x * p.y);\n}\n\nfloat vnoise(vec2 p) {\n\tvec2 i = floor(p);\n\tvec2 f = fract(p);\n\tvec2 u = f * f * (3.0 - 2.0 * f);\n\tfloat a = hash21(i);\n\tfloat b = hash21(i + vec2(1.0, 0.0));\n\tfloat c = hash21(i + vec2(0.0, 1.0));\n\tfloat d = hash21(i + vec2(1.0, 1.0));\n\treturn mix(mix(a, b, u.x), mix(c, d, u.x), u.y);\n}\n\nvoid fragment() {\n\tvec2 p = UV * view;\n\tfloat v = 0.0;\n\tfloat core = 0.0;\n\tvec2 push = vec2(0.0);\n\tfor (int i = 0; i < 6; i++) {\n\t\tfloat al = a_alpha[i];\n\t\tif (al > 0.002) {\n\t\t\tvec2 rel = p - vec2(a_x[i], a_y[i]);\n\t\t\tfloat cs = a_cos[i];\n\t\t\tfloat sn = a_sin[i];\n\t\t\t// Into its plane: q.y above 0 is nearer to us than its middle.\n\t\t\tvec2 q = vec2(rel.x * cs + rel.y * sn, (rel.y * cs - rel.x * sn) / tilt);\n\t\t\tfloat d = length(q) + 0.001;\n\t\t\tfloat big = a_r[i];\n\t\t\tfloat s = big - d;                          // above 0 behind the lip (inside the ring)\n\t\t\tif (s > -260.0 && s < a_wake[i] * 4.0 + 120.0) {\n\t\t\t\tfloat th = atan(q.y, q.x);\n\t\t\t\tfloat turn = mod(th - a_dir[i] + K_PI, K_TAU) - K_PI;\n\t\t\t\tfloat da = abs(turn);\n\t\t\t\tif (a_both[i] > 0.5) {\n\t\t\t\t\tda = min(da, K_PI - da);\n\t\t\t\t}\n\t\t\t\tfloat sp = max(a_spread[i], 0.01);\n\t\t\t\tfloat x = da / sp;                      // 0 the middle of the piece .. 1 its ends\n\t\t\t\tif (x < 1.0) {\n\t\t\t\t\tfloat side = smoothstep(-2.5, 2.5, which_half * q.y * tilt);\n\t\t\t\t\tvec2 n = vec2(q.x, q.y / tilt);     // across the lip, on the screen\n\t\t\t\t\tfloat nl = max(length(n), 0.0001);\n\t\t\t\t\tfloat g = nl / d;                   // px in the plane for one px on the screen, across the lip\n\t\t\t\t\tvec2 c = q / d;\n\t\t\t\t\tfloat seed = a_seed[i];\n\t\t\t\t\tfloat na = vnoise(c * 2.6 + vec2(seed, clock * 1.1));\n\t\t\t\t\tfloat nb = vnoise(c * 22.0 + vec2(seed * 3.1 + s / 360.0, -clock * 0.4));\n\t\t\t\t\tfloat nc = vnoise(c * 7.0 + vec2(seed * 1.7 + s / 900.0, clock * 0.3));\n\t\t\t\t\tfloat ne = 0.55 * vnoise(c * 3.3 + vec2(clock * 0.9, seed * 2.3)) + 0.45 * nb;\n\n\t\t\t\t\t// Thick in the middle, pointed at both ends; and brighter at the end that goes first.\n\t\t\t\t\tfloat tk = sqrt(max(1.0 - x * x, 0.0));\n\t\t\t\t\tfloat bright = 1.0 - smoothstep(0.62, 1.0, x);\n\t\t\t\t\tfloat along = 0.5 + 0.5 * turn / sp;\n\t\t\t\t\tfloat lead = a_lead[i];\n\t\t\t\t\tfloat hd = lead >= 0.0 ? along : 1.0 - along;\n\t\t\t\t\tbright *= mix(1.0, 0.10 + 0.90 * hd * hd, abs(lead));\n\n\t\t\t\t\t// The lip: a clean leading side, and behind it a feathered one that the noise combs out.\n\t\t\t\t\tfloat ss = s / g + (na - 0.5) * 3.0;\n\t\t\t\t\tfloat w_e = edge * (0.45 + 0.55 * tk);\n\t\t\t\t\tfloat w_f = feather * (0.20 + 0.80 * tk) * (0.35 + 1.25 * nb);\n\t\t\t\t\tfloat lip = ss < 0.0 ? exp(-(ss * ss) / (w_e * w_e)) : exp(-ss / w_f);\n\t\t\t\t\tfloat crest = exp(-(ss * ss) / (w_e * w_e * 0.8));\n\t\t\t\t\tfloat g1 = ripple_gap * (0.5 + 0.5 * tk);\n\t\t\t\t\tfloat r1 = exp(-((ss - g1) * (ss - g1)) / (w_e * w_e * 1.7)) * ripple;\n\t\t\t\t\tfloat r2 = exp(-((ss - 2.05 * g1) * (ss - 2.05 * g1)) / (w_e * w_e * 2.6)) * ripple * 0.5;\n\t\t\t\t\tfloat w_h = halo * (0.4 + 0.6 * tk);\n\t\t\t\t\tfloat h = exp(-(ss * ss) / (w_h * w_h));\n\t\t\t\t\tfloat back = max(s, 0.0);\n\t\t\t\t\tfloat ln = min(a_wake[i], big * 0.30) + 1.0;\n\t\t\t\t\tfloat mist = smoothstep(0.0, 8.0, ss) * exp(-back / ln) * nc * nc * (1.0 - smoothstep(0.20, 0.45, back / max(big, 1.0)));\n\t\t\t\t\t// Nothing of it reaches its own middle (the noise runs together there).\n\t\t\t\t\tfloat inner = smoothstep(0.30, 0.62, d / max(big, 1.0));\n\t\t\t\t\tfloat er = a_erode[i] + thin * (1.0 - a_erode[i]);\n\t\t\t\t\tfloat keep = smoothstep(er * 1.15 - 0.30, er * 1.15 + 0.05, ne);\n\t\t\t\t\tfloat w = bright * keep * side * al;\n\n\t\t\t\t\tfloat trail = ss < 0.0 ? lip : lip * inner;\n\t\t\t\t\tv += (trail * 0.62 + crest * 0.30 + (r1 + r2) * inner + h * 0.16 + mist * 0.22 * inner) * w;\n\t\t\t\t\tcore += crest * w;\n\t\t\t\t\t// What is behind it is pushed outwards on the lip and swings back just behind it.\n\t\t\t\t\tfloat ridge = h - 0.6 * exp(-((ss - w_h * 1.3) * (ss - w_h * 1.3)) / (w_h * w_h));\n\t\t\t\t\tvec2 nn = n / nl;\n\t\t\t\t\tpush += vec2(nn.x * cs - nn.y * sn, nn.x * sn + nn.y * cs) * (ridge * w);\n\t\t\t\t}\n\t\t\t}\n\t\t}\n\t}\n\tvec3 rgb = mix(tint.rgb, vec3(1.0), clamp(core * 0.8 - 0.10, 0.0, 0.70));\n\tCOLOR = vec4(rgb, clamp(v, 0.0, 1.0) * strength);\n}\n"
				};
			}
			ShaderWarmup.Add(_warp);
			ShaderWarmup.Add(_plain);
		}

		public static void Reset()
		{
			foreach (Cast item in Live)
			{
				Free(item);
			}
			Live.Clear();
		}

		public override void OnPlay(CardFxContext c)
		{
			try
			{
				foreach (Cast item in Live)
				{
					if (item.Card != c.Card)
					{
						continue;
					}
					if (c.Card != null && MultiplayerFx.Active)
					{
						if (MultiplayerFx.CurrentInvocation == null || item.Invocation != MultiplayerFx.CurrentInvocation)
						{
							continue;
						}
						Creature owner = item.Owner;
						CardModel card = c.Card;
						object obj;
						if (card == null)
						{
							obj = null;
						}
						else
						{
							Player owner2 = card.Owner;
							obj = ((owner2 != null) ? owner2.Creature : null);
						}
						if (owner != obj || (object)item.Room != GameRefs.CombatRoom)
						{
							continue;
						}
					}
					item.Closed = true;
				}
				Cast cast = Start(c, swing: true);
				if (cast != null)
				{
					if (c.Card == null)
					{
						cast.Acting = true;
						cast.ActLeft = Mathf.Clamp((int)Tuning.F("echo.preview_hits", 3f), 1, 6);
						cast.ActKills = cast.ActLeft - 1;
						cast.ActAt = Mathf.Clamp(Tuning.F("echo.first", 0.13f), 0.03f, 0.8f);
					}
					else
					{
						cast.Forced = Mathf.Clamp((int)Tuning.F("echo.force_hits", 0f), 0, 6);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("EchoingSlashFx.OnPlay: " + ex);
			}
		}

		public static void BlowBegins(CardModel card)
		{
			try
			{
				Cast cast = Find(card);
				if (cast != null && !cast.Acting)
				{
					NextBlow(cast);
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("EchoingSlashFx.BlowBegins: " + ex);
			}
		}

		public override void OnHit(CardFxContext c, Vector2 at, bool killed)
		{
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (c.Card == null || c.Target == null)
				{
					return;
				}
				Cast cast = Find(c.Card);
				if (cast == null && (MultiplayerFx.Active || (MultiplayerFx.CurrentInvocation != null && MultiplayerInvocations.TryGetValue(MultiplayerFx.CurrentInvocation, out var _))))
				{
					return;
				}
				if (cast == null)
				{
					cast = Start(c, swing: false);
				}
				if (cast == null || cast.Acting || (MultiplayerFx.Active && cast.Struck.Contains(c.Target)))
				{
					return;
				}
				if (cast.Blow < 0 || (!MultiplayerFx.Active && cast.Struck.Contains(c.Target)))
				{
					NextBlow(cast);
				}
				Foe foe = null;
				foreach (Foe foe2 in cast.Foes)
				{
					if (foe2.Key == c.Target)
					{
						foe = foe2;
						break;
					}
				}
				if (foe == null)
				{
					foe = new Foe
					{
						Key = c.Target
					};
					cast.Foes.Add(foe);
				}
				foe.Chest = c.TargetChest;
				foe.Width = c.TargetWidth;
				foe.Height = c.TargetHeight;
				Hit(cast, foe, killed);
			}
			catch (Exception ex)
			{
				DevLog.Warn("EchoingSlashFx.OnHit: " + ex);
			}
		}

		private static Cast Find(CardModel card)
		{
			if (card == null)
			{
				return null;
			}
			if (!MultiplayerFx.Active && MultiplayerFx.CurrentInvocation != null && MultiplayerInvocations.TryGetValue(MultiplayerFx.CurrentInvocation, out var _))
			{
				return null;
			}
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				Cast cast = Live[num];
				if (!cast.Closed && cast.Card == card && (!MultiplayerFx.Active || (MultiplayerFx.CurrentInvocation != null && card == MultiplayerFx.CurrentCard && cast.Invocation == MultiplayerFx.CurrentInvocation && ScopeAlive(cast))) && (!cast.Multiplayer || MultiplayerFx.Active))
				{
					return cast;
				}
			}
			return null;
		}

		private static bool ScopeAlive(Cast cast)
		{
			if (cast.Multiplayer && MultiplayerFx.Active && (object)cast.Room == GameRefs.CombatRoom && FxKit.Alive((GodotObject)(object)cast.Room) && cast.Room.IsInsideTree() && FxKit.Alive((GodotObject)(object)cast.Front) && cast.Front.IsInsideTree() && FxKit.Alive((GodotObject)(object)cast.Back) && cast.Back.IsInsideTree() && cast.Owner != null && cast.Owner.IsAlive)
			{
				CardModel card = cast.Card;
				object obj;
				if (card == null)
				{
					obj = null;
				}
				else
				{
					Player owner = card.Owner;
					obj = ((owner != null) ? owner.Creature : null);
				}
				if (obj == cast.Owner && OwnerPresent(cast) && cast.Remote == !MultiplayerFx.IsLocal(cast.Card) && FxKit.Alive((GodotObject)(object)cast.OwnerNode) && cast.OwnerNode.IsInsideTree())
				{
					NCombatRoom combatRoom = GameRefs.CombatRoom;
					return (object)((combatRoom != null) ? combatRoom.GetCreatureNode(cast.Owner) : null) == cast.OwnerNode;
				}
			}
			return false;
		}

		private static bool OwnerPresent(Cast cast)
		{
			CombatManager instance = CombatManager.Instance;
			CombatState val = ((instance != null) ? instance.DebugOnlyGetState() : null);
			if (val == null || (object)cast.Owner.CombatState != val || cast.Card.Owner != cast.Owner.Player)
			{
				return false;
			}
			foreach (Player player in val.Players)
			{
				if (player == cast.Owner.Player)
				{
					return true;
				}
			}
			return false;
		}

		private static Cast Start(CardFxContext c, bool swing)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)c.FrontParent) || !FxKit.Alive((GodotObject)(object)c.BackParent))
			{
				return null;
			}
			Vector2 view = c.View;
			if (!(view.X > 400f) || !(view.Y > 300f))
			{
				return null;
			}
			bool flag = MultiplayerFx.Active && c.Card != null;
			object obj;
			if (!flag)
			{
				obj = null;
			}
			else
			{
				Player owner = c.Card.Owner;
				obj = ((owner != null) ? owner.Creature : null);
			}
			Creature val = (Creature)obj;
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			NCreature val2;
			if (flag)
			{
				val2 = ((combatRoom != null) ? combatRoom.GetCreatureNode(val) : null);
			}
			else
			{
				val2 = null;
			}
			if (flag && (MultiplayerFx.CurrentInvocation == null || c.Card != MultiplayerFx.CurrentCard || val == null || !val.IsAlive || !FxKit.Alive((GodotObject)(object)combatRoom) || !((Node)combatRoom).IsInsideTree() || !FxKit.Alive((GodotObject)(object)val2) || !((Node)val2).IsInsideTree()))
			{
				return null;
			}
			Cast cast = new Cast
			{
				Card = c.Card,
				Multiplayer = flag,
				Remote = (flag && c.Remote),
				Owner = val,
				OwnerNode = (Node)(object)val2,
				Room = (Node)(object)combatRoom,
				Invocation = (flag ? MultiplayerFx.CurrentInvocation : null),
				Back = c.BackParent,
				Front = c.FrontParent,
				Centre = c.Chest,
				BodyWidth = Mathf.Max(c.Body.X, 120f),
				View = view,
				Gap = GuessGap()
			};
			if (c.Enemies != null)
			{
				foreach (FxTarget enemy in c.Enemies)
				{
					if (enemy != null)
					{
						cast.Foes.Add(new Foe
						{
							Key = enemy.Key,
							Chest = enemy.Chest,
							Width = enemy.Width,
							Height = enemy.Height
						});
					}
				}
			}
			bool flag2 = WarpAllowed(c);
			cast.HolderBack = Layer(cast, cast.Back, -1f, flag2, out cast.MatBack);
			cast.HolderFront = Layer(cast, cast.Front, 1f, flag2, out cast.MatFront);
			if (cast.HolderFront == null && cast.HolderBack == null)
			{
				return null;
			}
			if (swing && cast.Foes.Count > 0)
			{
				Arc arc = NewArc(cast, cast.Centre);
				arc.Stage = Stage.Slash;
				arc.Slope = (arc.SlopeGoal = Fit(arc.Centre, cast, null));
				if (Aim(cast, arc, null))
				{
					ref float side = ref arc.Side;
					float num;
					if (arc.Sides != 0)
					{
						num = arc.Sides;
					}
					else
					{
						num = ((c.Facing < 0f) ? (-1f) : 1f);
					}
					side = num;
					arc.Two = arc.Sides == 0;
					arc.From = (arc.Radius = Mathf.Clamp(cast.BodyWidth * 0.4f, 70f, 130f));
					arc.Plan = Mathf.Clamp(Tuning.F("echo.first", 0.13f), 0.03f, 0.8f);
					arc.Power = PowerFor(arc.From, arc.Near, arc.Far);
					cast.Hers = arc;
				}
				else
				{
					cast.Arcs.Remove(arc);
				}
			}
			Live.Add(cast);
			if (cast.Multiplayer)
			{
				MultiplayerInvocations.GetValue(cast.Invocation, (object _) => new object());
			}
			if (!_ticking)
			{
				_ticking = true;
				Ticker.Add(Update);
			}
			DevLog.Info($"Echoing Slash: {cast.Foes.Count} enemy(ies), {((cast.Hers == null) ? "no swing" : (cast.Hers.Two ? "both sides" : ((cast.Hers.Side > 0f) ? "to the right" : "to the left")))}, {cast.Gap:0.00} s between blows expected, {(flag2 ? "bends what is behind it" : "light only")}");
			return cast;
		}

		private static Node2D Layer(Cast cast, Node parent, float half, bool warp, out ShaderMaterial mat)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected Obj, but got Unknown
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Expected Obj, but got Unknown
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Expected Obj, but got Unknown
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Expected Obj, but got Unknown
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Expected Obj, but got Unknown
			mat = null;
			try
			{
				Node2D val = new Node2D
				{
					Name = StringName.op_Implicit((half > 0f) ? "SilentFX_EchoAirFront" : "SilentFX_EchoAirBack")
				};
				parent.AddChild((Node)(object)val, false, (InternalMode)0);
				FxKit.FreeAfter((Node)(object)val, 12.5);
				if (!((Node)val).IsInsideTree())
				{
					((Node)val).QueueFree();
					return null;
				}
				val.GlobalTransform = Transform2D.Identity;
				Shader shader;
				if (warp)
				{
					if (_warp == null || !FxKit.Alive((GodotObject)(object)_warp))
					{
						_warp = new Shader
						{
							Code = "// SilentFX - echo air (Echoing Slash), the kind that also bends what is behind it.\nshader_type canvas_item;\nrender_mode unshaded;\n\nuniform sampler2D screen : hint_screen_texture, filter_linear, repeat_disable;\n\n// Up to six arcs of sword wind. Each is a piece of a ring that lies in a level plane and is seen from the side:\n// on the screen an ellipse 'tilt' times as tall as it is wide, turned by a few degrees. The half of it that is\n// nearer than its middle is drawn by one rectangle (in front of everybody), the farther half by another (behind).\nuniform vec2 view = vec2(1920.0, 1080.0);     // the window in px: the rectangle covers exactly this\nuniform float tilt = 0.2;\nuniform float which_half = 1.0;               // +1 the near half, -1 the far half\nuniform float clock = 0.0;\nuniform vec4 tint : source_color = vec4(0.62, 0.80, 1.0, 1.0);\nuniform float strength = 0.95;\nuniform float edge = 3.4;                     // half the thickness of the leading lip, px\nuniform float halo = 20.0;                    // soft light round it, px\nuniform float feather = 30.0;                 // how far it trails away behind the lip, px\nuniform float ripple_gap = 24.0;              // the fainter lines behind the lip (the echo): how far apart, px\nuniform float ripple = 0.26;\nuniform float thin = 0.2;                     // how much of it is always missing: it is air, not a drawn line\nuniform float bend = 12.0;                    // how far it pushes what is behind it, px\n\nuniform float a_x[6];                         // its middle, px\nuniform float a_y[6];\nuniform float a_cos[6];                       // how its plane is turned\nuniform float a_sin[6];\nuniform float a_r[6];                         // how far it has got, px in its plane\nuniform float a_dir[6];                       // the middle of the piece of ring that is there (0 = to the right, a quarter turn = towards us)\nuniform float a_spread[6];                    // half its width, as an angle\nuniform float a_lead[6];                      // above 0 its bright end is the one turning forward, below 0 the other\nuniform float a_both[6];                      // 1 = the same piece again on the opposite side\nuniform float a_alpha[6];\nuniform float a_erode[6];                     // 0 whole .. 1 blown away\nuniform float a_wake[6];                      // how far the thin mist reaches back, px in its plane\nuniform float a_seed[6];\n\nconst float K_PI = 3.14159265;\nconst float K_TAU = 6.28318531;\n\nfloat hash21(vec2 p) {\n\tp = fract(p * vec2(123.34, 456.21));\n\tp += dot(p, p + 45.32);\n\treturn fract(p.x * p.y);\n}\n\nfloat vnoise(vec2 p) {\n\tvec2 i = floor(p);\n\tvec2 f = fract(p);\n\tvec2 u = f * f * (3.0 - 2.0 * f);\n\tfloat a = hash21(i);\n\tfloat b = hash21(i + vec2(1.0, 0.0));\n\tfloat c = hash21(i + vec2(0.0, 1.0));\n\tfloat d = hash21(i + vec2(1.0, 1.0));\n\treturn mix(mix(a, b, u.x), mix(c, d, u.x), u.y);\n}\n\nvoid fragment() {\n\tvec2 p = UV * view;\n\tfloat v = 0.0;\n\tfloat core = 0.0;\n\tvec2 push = vec2(0.0);\n\tfor (int i = 0; i < 6; i++) {\n\t\tfloat al = a_alpha[i];\n\t\tif (al > 0.002) {\n\t\t\tvec2 rel = p - vec2(a_x[i], a_y[i]);\n\t\t\tfloat cs = a_cos[i];\n\t\t\tfloat sn = a_sin[i];\n\t\t\t// Into its plane: q.y above 0 is nearer to us than its middle.\n\t\t\tvec2 q = vec2(rel.x * cs + rel.y * sn, (rel.y * cs - rel.x * sn) / tilt);\n\t\t\tfloat d = length(q) + 0.001;\n\t\t\tfloat big = a_r[i];\n\t\t\tfloat s = big - d;                          // above 0 behind the lip (inside the ring)\n\t\t\tif (s > -260.0 && s < a_wake[i] * 4.0 + 120.0) {\n\t\t\t\tfloat th = atan(q.y, q.x);\n\t\t\t\tfloat turn = mod(th - a_dir[i] + K_PI, K_TAU) - K_PI;\n\t\t\t\tfloat da = abs(turn);\n\t\t\t\tif (a_both[i] > 0.5) {\n\t\t\t\t\tda = min(da, K_PI - da);\n\t\t\t\t}\n\t\t\t\tfloat sp = max(a_spread[i], 0.01);\n\t\t\t\tfloat x = da / sp;                      // 0 the middle of the piece .. 1 its ends\n\t\t\t\tif (x < 1.0) {\n\t\t\t\t\tfloat side = smoothstep(-2.5, 2.5, which_half * q.y * tilt);\n\t\t\t\t\tvec2 n = vec2(q.x, q.y / tilt);     // across the lip, on the screen\n\t\t\t\t\tfloat nl = max(length(n), 0.0001);\n\t\t\t\t\tfloat g = nl / d;                   // px in the plane for one px on the screen, across the lip\n\t\t\t\t\tvec2 c = q / d;\n\t\t\t\t\tfloat seed = a_seed[i];\n\t\t\t\t\tfloat na = vnoise(c * 2.6 + vec2(seed, clock * 1.1));\n\t\t\t\t\tfloat nb = vnoise(c * 22.0 + vec2(seed * 3.1 + s / 360.0, -clock * 0.4));\n\t\t\t\t\tfloat nc = vnoise(c * 7.0 + vec2(seed * 1.7 + s / 900.0, clock * 0.3));\n\t\t\t\t\tfloat ne = 0.55 * vnoise(c * 3.3 + vec2(clock * 0.9, seed * 2.3)) + 0.45 * nb;\n\n\t\t\t\t\t// Thick in the middle, pointed at both ends; and brighter at the end that goes first.\n\t\t\t\t\tfloat tk = sqrt(max(1.0 - x * x, 0.0));\n\t\t\t\t\tfloat bright = 1.0 - smoothstep(0.62, 1.0, x);\n\t\t\t\t\tfloat along = 0.5 + 0.5 * turn / sp;\n\t\t\t\t\tfloat lead = a_lead[i];\n\t\t\t\t\tfloat hd = lead >= 0.0 ? along : 1.0 - along;\n\t\t\t\t\tbright *= mix(1.0, 0.10 + 0.90 * hd * hd, abs(lead));\n\n\t\t\t\t\t// The lip: a clean leading side, and behind it a feathered one that the noise combs out.\n\t\t\t\t\tfloat ss = s / g + (na - 0.5) * 3.0;\n\t\t\t\t\tfloat w_e = edge * (0.45 + 0.55 * tk);\n\t\t\t\t\tfloat w_f = feather * (0.20 + 0.80 * tk) * (0.35 + 1.25 * nb);\n\t\t\t\t\tfloat lip = ss < 0.0 ? exp(-(ss * ss) / (w_e * w_e)) : exp(-ss / w_f);\n\t\t\t\t\tfloat crest = exp(-(ss * ss) / (w_e * w_e * 0.8));\n\t\t\t\t\tfloat g1 = ripple_gap * (0.5 + 0.5 * tk);\n\t\t\t\t\tfloat r1 = exp(-((ss - g1) * (ss - g1)) / (w_e * w_e * 1.7)) * ripple;\n\t\t\t\t\tfloat r2 = exp(-((ss - 2.05 * g1) * (ss - 2.05 * g1)) / (w_e * w_e * 2.6)) * ripple * 0.5;\n\t\t\t\t\tfloat w_h = halo * (0.4 + 0.6 * tk);\n\t\t\t\t\tfloat h = exp(-(ss * ss) / (w_h * w_h));\n\t\t\t\t\tfloat back = max(s, 0.0);\n\t\t\t\t\tfloat ln = min(a_wake[i], big * 0.30) + 1.0;\n\t\t\t\t\tfloat mist = smoothstep(0.0, 8.0, ss) * exp(-back / ln) * nc * nc * (1.0 - smoothstep(0.20, 0.45, back / max(big, 1.0)));\n\t\t\t\t\t// Nothing of it reaches its own middle (the noise runs together there).\n\t\t\t\t\tfloat inner = smoothstep(0.30, 0.62, d / max(big, 1.0));\n\t\t\t\t\tfloat er = a_erode[i] + thin * (1.0 - a_erode[i]);\n\t\t\t\t\tfloat keep = smoothstep(er * 1.15 - 0.30, er * 1.15 + 0.05, ne);\n\t\t\t\t\tfloat w = bright * keep * side * al;\n\n\t\t\t\t\tfloat trail = ss < 0.0 ? lip : lip * inner;\n\t\t\t\t\tv += (trail * 0.62 + crest * 0.30 + (r1 + r2) * inner + h * 0.16 + mist * 0.22 * inner) * w;\n\t\t\t\t\tcore += crest * w;\n\t\t\t\t\t// What is behind it is pushed outwards on the lip and swings back just behind it.\n\t\t\t\t\tfloat ridge = h - 0.6 * exp(-((ss - w_h * 1.3) * (ss - w_h * 1.3)) / (w_h * w_h));\n\t\t\t\t\tvec2 nn = n / nl;\n\t\t\t\t\tpush += vec2(nn.x * cs - nn.y * sn, nn.x * sn + nn.y * cs) * (ridge * w);\n\t\t\t\t}\n\t\t\t}\n\t\t}\n\t}\n\tvec3 rgb = mix(tint.rgb, vec3(1.0), clamp(core * 0.8 - 0.10, 0.0, 0.70));\n\t// Clear where there is nothing of it: the rectangle is only ever seen where it has something to show.\n\tvec2 shift = push * bend;\n\tfloat cover = clamp(max(v, length(push)) * 24.0, 0.0, 1.0);\n\tvec3 seen = texture(screen, SCREEN_UV - shift / view).rgb;\n\tCOLOR = vec4(seen + rgb * (min(v, 1.2) * strength), cover);\n}\n"
						};
					}
					shader = _warp;
				}
				else
				{
					if (_plain == null || !FxKit.Alive((GodotObject)(object)_plain))
					{
						_plain = new Shader
						{
							Code = "// SilentFX - echo air (Echoing Slash), light only.\nshader_type canvas_item;\nrender_mode blend_add, unshaded;\n\n// Up to six arcs of sword wind. Each is a piece of a ring that lies in a level plane and is seen from the side:\n// on the screen an ellipse 'tilt' times as tall as it is wide, turned by a few degrees. The half of it that is\n// nearer than its middle is drawn by one rectangle (in front of everybody), the farther half by another (behind).\nuniform vec2 view = vec2(1920.0, 1080.0);     // the window in px: the rectangle covers exactly this\nuniform float tilt = 0.2;\nuniform float which_half = 1.0;               // +1 the near half, -1 the far half\nuniform float clock = 0.0;\nuniform vec4 tint : source_color = vec4(0.62, 0.80, 1.0, 1.0);\nuniform float strength = 0.95;\nuniform float edge = 3.4;                     // half the thickness of the leading lip, px\nuniform float halo = 20.0;                    // soft light round it, px\nuniform float feather = 30.0;                 // how far it trails away behind the lip, px\nuniform float ripple_gap = 24.0;              // the fainter lines behind the lip (the echo): how far apart, px\nuniform float ripple = 0.26;\nuniform float thin = 0.2;                     // how much of it is always missing: it is air, not a drawn line\nuniform float bend = 12.0;                    // how far it pushes what is behind it, px\n\nuniform float a_x[6];                         // its middle, px\nuniform float a_y[6];\nuniform float a_cos[6];                       // how its plane is turned\nuniform float a_sin[6];\nuniform float a_r[6];                         // how far it has got, px in its plane\nuniform float a_dir[6];                       // the middle of the piece of ring that is there (0 = to the right, a quarter turn = towards us)\nuniform float a_spread[6];                    // half its width, as an angle\nuniform float a_lead[6];                      // above 0 its bright end is the one turning forward, below 0 the other\nuniform float a_both[6];                      // 1 = the same piece again on the opposite side\nuniform float a_alpha[6];\nuniform float a_erode[6];                     // 0 whole .. 1 blown away\nuniform float a_wake[6];                      // how far the thin mist reaches back, px in its plane\nuniform float a_seed[6];\n\nconst float K_PI = 3.14159265;\nconst float K_TAU = 6.28318531;\n\nfloat hash21(vec2 p) {\n\tp = fract(p * vec2(123.34, 456.21));\n\tp += dot(p, p + 45.32);\n\treturn fract(p.x * p.y);\n}\n\nfloat vnoise(vec2 p) {\n\tvec2 i = floor(p);\n\tvec2 f = fract(p);\n\tvec2 u = f * f * (3.0 - 2.0 * f);\n\tfloat a = hash21(i);\n\tfloat b = hash21(i + vec2(1.0, 0.0));\n\tfloat c = hash21(i + vec2(0.0, 1.0));\n\tfloat d = hash21(i + vec2(1.0, 1.0));\n\treturn mix(mix(a, b, u.x), mix(c, d, u.x), u.y);\n}\n\nvoid fragment() {\n\tvec2 p = UV * view;\n\tfloat v = 0.0;\n\tfloat core = 0.0;\n\tvec2 push = vec2(0.0);\n\tfor (int i = 0; i < 6; i++) {\n\t\tfloat al = a_alpha[i];\n\t\tif (al > 0.002) {\n\t\t\tvec2 rel = p - vec2(a_x[i], a_y[i]);\n\t\t\tfloat cs = a_cos[i];\n\t\t\tfloat sn = a_sin[i];\n\t\t\t// Into its plane: q.y above 0 is nearer to us than its middle.\n\t\t\tvec2 q = vec2(rel.x * cs + rel.y * sn, (rel.y * cs - rel.x * sn) / tilt);\n\t\t\tfloat d = length(q) + 0.001;\n\t\t\tfloat big = a_r[i];\n\t\t\tfloat s = big - d;                          // above 0 behind the lip (inside the ring)\n\t\t\tif (s > -260.0 && s < a_wake[i] * 4.0 + 120.0) {\n\t\t\t\tfloat th = atan(q.y, q.x);\n\t\t\t\tfloat turn = mod(th - a_dir[i] + K_PI, K_TAU) - K_PI;\n\t\t\t\tfloat da = abs(turn);\n\t\t\t\tif (a_both[i] > 0.5) {\n\t\t\t\t\tda = min(da, K_PI - da);\n\t\t\t\t}\n\t\t\t\tfloat sp = max(a_spread[i], 0.01);\n\t\t\t\tfloat x = da / sp;                      // 0 the middle of the piece .. 1 its ends\n\t\t\t\tif (x < 1.0) {\n\t\t\t\t\tfloat side = smoothstep(-2.5, 2.5, which_half * q.y * tilt);\n\t\t\t\t\tvec2 n = vec2(q.x, q.y / tilt);     // across the lip, on the screen\n\t\t\t\t\tfloat nl = max(length(n), 0.0001);\n\t\t\t\t\tfloat g = nl / d;                   // px in the plane for one px on the screen, across the lip\n\t\t\t\t\tvec2 c = q / d;\n\t\t\t\t\tfloat seed = a_seed[i];\n\t\t\t\t\tfloat na = vnoise(c * 2.6 + vec2(seed, clock * 1.1));\n\t\t\t\t\tfloat nb = vnoise(c * 22.0 + vec2(seed * 3.1 + s / 360.0, -clock * 0.4));\n\t\t\t\t\tfloat nc = vnoise(c * 7.0 + vec2(seed * 1.7 + s / 900.0, clock * 0.3));\n\t\t\t\t\tfloat ne = 0.55 * vnoise(c * 3.3 + vec2(clock * 0.9, seed * 2.3)) + 0.45 * nb;\n\n\t\t\t\t\t// Thick in the middle, pointed at both ends; and brighter at the end that goes first.\n\t\t\t\t\tfloat tk = sqrt(max(1.0 - x * x, 0.0));\n\t\t\t\t\tfloat bright = 1.0 - smoothstep(0.62, 1.0, x);\n\t\t\t\t\tfloat along = 0.5 + 0.5 * turn / sp;\n\t\t\t\t\tfloat lead = a_lead[i];\n\t\t\t\t\tfloat hd = lead >= 0.0 ? along : 1.0 - along;\n\t\t\t\t\tbright *= mix(1.0, 0.10 + 0.90 * hd * hd, abs(lead));\n\n\t\t\t\t\t// The lip: a clean leading side, and behind it a feathered one that the noise combs out.\n\t\t\t\t\tfloat ss = s / g + (na - 0.5) * 3.0;\n\t\t\t\t\tfloat w_e = edge * (0.45 + 0.55 * tk);\n\t\t\t\t\tfloat w_f = feather * (0.20 + 0.80 * tk) * (0.35 + 1.25 * nb);\n\t\t\t\t\tfloat lip = ss < 0.0 ? exp(-(ss * ss) / (w_e * w_e)) : exp(-ss / w_f);\n\t\t\t\t\tfloat crest = exp(-(ss * ss) / (w_e * w_e * 0.8));\n\t\t\t\t\tfloat g1 = ripple_gap * (0.5 + 0.5 * tk);\n\t\t\t\t\tfloat r1 = exp(-((ss - g1) * (ss - g1)) / (w_e * w_e * 1.7)) * ripple;\n\t\t\t\t\tfloat r2 = exp(-((ss - 2.05 * g1) * (ss - 2.05 * g1)) / (w_e * w_e * 2.6)) * ripple * 0.5;\n\t\t\t\t\tfloat w_h = halo * (0.4 + 0.6 * tk);\n\t\t\t\t\tfloat h = exp(-(ss * ss) / (w_h * w_h));\n\t\t\t\t\tfloat back = max(s, 0.0);\n\t\t\t\t\tfloat ln = min(a_wake[i], big * 0.30) + 1.0;\n\t\t\t\t\tfloat mist = smoothstep(0.0, 8.0, ss) * exp(-back / ln) * nc * nc * (1.0 - smoothstep(0.20, 0.45, back / max(big, 1.0)));\n\t\t\t\t\t// Nothing of it reaches its own middle (the noise runs together there).\n\t\t\t\t\tfloat inner = smoothstep(0.30, 0.62, d / max(big, 1.0));\n\t\t\t\t\tfloat er = a_erode[i] + thin * (1.0 - a_erode[i]);\n\t\t\t\t\tfloat keep = smoothstep(er * 1.15 - 0.30, er * 1.15 + 0.05, ne);\n\t\t\t\t\tfloat w = bright * keep * side * al;\n\n\t\t\t\t\tfloat trail = ss < 0.0 ? lip : lip * inner;\n\t\t\t\t\tv += (trail * 0.62 + crest * 0.30 + (r1 + r2) * inner + h * 0.16 + mist * 0.22 * inner) * w;\n\t\t\t\t\tcore += crest * w;\n\t\t\t\t\t// What is behind it is pushed outwards on the lip and swings back just behind it.\n\t\t\t\t\tfloat ridge = h - 0.6 * exp(-((ss - w_h * 1.3) * (ss - w_h * 1.3)) / (w_h * w_h));\n\t\t\t\t\tvec2 nn = n / nl;\n\t\t\t\t\tpush += vec2(nn.x * cs - nn.y * sn, nn.x * sn + nn.y * cs) * (ridge * w);\n\t\t\t\t}\n\t\t\t}\n\t\t}\n\t}\n\tvec3 rgb = mix(tint.rgb, vec3(1.0), clamp(core * 0.8 - 0.10, 0.0, 0.70));\n\tCOLOR = vec4(rgb, clamp(v, 0.0, 1.0) * strength);\n}\n"
						};
					}
					shader = _plain;
				}
				mat = new ShaderMaterial
				{
					Shader = shader
				};
				ColorRect val2 = FxKit.Quad(cast.View, (Material)(object)mat);
				val2.Color = new Color(1f, 1f, 1f, 0f);
				if (warp)
				{
					((Node)val2).AddChild((Node)new BackBufferCopy
					{
						CopyMode = (CopyModeEnum)2,
						ShowBehindParent = true
					}, false, (InternalMode)0);
				}
				((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
				mat.SetShaderParameter(StringName.op_Implicit("view"), Variant.op_Implicit(cast.View));
				mat.SetShaderParameter(StringName.op_Implicit("which_half"), Variant.op_Implicit(half));
				mat.SetShaderParameter(StringName.op_Implicit("tilt"), Variant.op_Implicit(Tilt()));
				mat.SetShaderParameter(StringName.op_Implicit("tint"), Variant.op_Implicit(new Color(Echo.R, Echo.G, Echo.B, 1f)));
				mat.SetShaderParameter(StringName.op_Implicit("strength"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("echo.strength", 0.95f), 0f, 1.5f)));
				mat.SetShaderParameter(StringName.op_Implicit("edge"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("echo.edge", 3.4f), 1f, 12f)));
				mat.SetShaderParameter(StringName.op_Implicit("feather"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("echo.feather", 30f), 4f, 120f)));
				mat.SetShaderParameter(StringName.op_Implicit("thin"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("echo.thin", 0.2f), 0f, 0.8f)));
				mat.SetShaderParameter(StringName.op_Implicit("bend"), Variant.op_Implicit(Mathf.Clamp(Tuning.F("echo.bend", 12f), 0f, 40f)));
				mat.SetShaderParameter(StringName.op_Implicit("a_alpha"), Variant.op_Implicit(cast.Alpha));
				return val;
			}
			catch (Exception ex)
			{
				DevLog.Warn("EchoingSlashFx.Layer: " + ex);
				mat = null;
				return null;
			}
		}

		private static bool WarpAllowed(CardFxContext c)
		{
			try
			{
				if (Tuning.F("echo.warp", 1f) < 0.5f)
				{
					return false;
				}
				CardModel card = c.Card;
				object obj;
				if (card == null)
				{
					obj = null;
				}
				else
				{
					ICombatState combatState = card.CombatState;
					obj = ((combatState != null) ? combatState.Encounter : null);
				}
				EncounterModel val = (EncounterModel)obj;
				if (val != null)
				{
					PropertyInfo property = typeof(EncounterModel).GetProperty("HasCustomBackground", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					bool flag = default;
					int num;
					if (property != null)
					{
						object value = property.GetValue(val);
						if (value is bool)
						{
							flag = (bool)value;
							num = 1;
						}
						else
						{
							num = 0;
						}
					}
					else
					{
						num = 0;
					}
					if (((uint)num & (flag ? 1u : 0u)) != 0)
					{
						return false;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				if (!_warned)
				{
					DevLog.Warn("Echoing Slash: could not tell whether the screen can be read, the wind is drawn as light only: " + ex.Message);
				}
				_warned = true;
				return false;
			}
		}

		private static float GuessGap()
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Invalid comparison between Unknown and I4
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Invalid comparison between Unknown and I4
			float num = Tuning.F("echo.gap", 0f);
			if (num > 0.01f)
			{
				return Mathf.Clamp(num, 0.04f, 1f);
			}
			try
			{
				SaveManager instance = SaveManager.Instance;
				PrefsSave val = ((instance != null) ? instance.PrefsSave : null);
				if (val != null)
				{
					FastModeType fastMode = val.FastMode;
					if ((int)fastMode == 2)
					{
						return 0.1f;
					}
					if ((int)fastMode == 3)
					{
						return 0.06f;
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("EchoingSlashFx.GuessGap: " + ex.Message);
			}
			return 0.2f;
		}

		private static float Tilt()
		{
			return Mathf.Clamp(Tuning.F("echo.tilt", 0.2f), 0.06f, 0.6f);
		}

		private static float Window(float radius)
		{
			return Mathf.Clamp(Mathf.Clamp(Tuning.F("echo.size", 470f), 120f, 1200f) / Mathf.Max(radius, 1f), 0.3f, 2.6f);
		}

		private static float Smooth(float x)
		{
			x = Mathf.Clamp(x, 0f, 1f);
			return x * x * (3f - 2f * x);
		}

		private static Arc NewArc(Cast cast, Vector2 centre)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Arc arc = new Arc
			{
				Centre = centre,
				Seed = 1f + 9f * FxKit.Hash(++_seeds * 7 + 3)
			};
			cast.Arcs.Add(arc);
			return arc;
		}

		private static bool Standing(Foe foe)
		{
			if (foe.Dead)
			{
				return false;
			}
			try
			{
				object key = foe.Key;
				Creature val = (Creature)((key is Creature) ? key : null);
				if (val != null && !val.IsAlive)
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("EchoingSlashFx.Standing: " + ex.Message);
			}
			return true;
		}

		private static bool AnyStanding(Cast cast)
		{
			foreach (Foe foe in cast.Foes)
			{
				if (Standing(foe))
				{
					return true;
				}
			}
			return false;
		}

		private static float Fit(Vector2 centre, Cast cast, Foe but)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			float num2 = 0f;
			foreach (Foe foe in cast.Foes)
			{
				if (foe != but && Standing(foe))
				{
					float num3 = foe.Chest.X - centre.X;
					num += num3 * (foe.Chest.Y - centre.Y);
					num2 += num3 * num3;
				}
			}
			if (num2 < 1f)
			{
				return 0f;
			}
			return Mathf.Clamp(Mathf.Atan2(num, num2), -0.16f, 0.16f);
		}

		private static float Along(Arc arc, Foe foe)
		{
			return (foe.Chest.X - arc.Centre.X) * Mathf.Cos(arc.SlopeGoal) + (foe.Chest.Y - arc.Centre.Y) * Mathf.Sin(arc.SlopeGoal);
		}

		private static bool Aim(Cast cast, Arc arc, Foe but)
		{
			bool flag = false;
			bool flag2 = false;
			float num = float.MaxValue;
			float num2 = 0f;
			foreach (Foe foe in cast.Foes)
			{
				if (foe == but || !Standing(foe))
				{
					continue;
				}
				float num3 = Along(arc, foe);
				float num4 = Mathf.Abs(num3);
				if (!(num4 < 20f))
				{
					if (num3 > 0f)
					{
						flag = true;
					}
					else
					{
						flag2 = true;
					}
					num = Mathf.Min(num, num4);
					num2 = Mathf.Max(num2, num4);
				}
			}
			if (!flag && !flag2)
			{
				return false;
			}
			arc.Sides = ((!(flag & flag2)) ? (flag ? 1 : (-1)) : 0);
			arc.Near = num;
			arc.Far = num2;
			return true;
		}

		private static float PowerFor(float from, float near, float far)
		{
			if (far - from < 1f)
			{
				return 1.3f;
			}
			return Mathf.Clamp(Mathf.Log(Mathf.Clamp((Mathf.Max(near - 70f, from + 1f) - from) / (far - from), 0.02f, 0.98f)) / Mathf.Log(0.75f), 1.3f, 4f);
		}

		private static void NextBlow(Cast cast)
		{
			cast.Blow++;
			cast.Struck.Clear();
			float num = cast.Age - cast.BlowAt;
			if (!cast.Acting && cast.Blow >= 1 && num > 0.03f && num < 0.8f && Tuning.F("echo.gap", 0f) <= 0.01f)
			{
				cast.Gap = num;
			}
			Arc arc;
			if (cast.Blow == 0)
			{
				arc = cast.Hers;
			}
			else if (cast.Queue.Count > 0)
			{
				arc = cast.Queue[0];
				cast.Queue.RemoveAt(0);
				if (arc.Stage == Stage.Whirl)
				{
					Fling(cast, arc, 0.05f);
				}
			}
			else
			{
				arc = null;
			}
			string text = "nothing in the air";
			if (arc != null && arc.Stage != Stage.Idle)
			{
				text = ((arc.Stage == Stage.Unravel) ? "nobody left to go to" : $"the wind at {arc.Radius:0} of {arc.Far:0}");
				Land(arc);
			}
			DevLog.Info($"Echoing Slash: blow {cast.Blow + 1}" + ((cast.Blow >= 1) ? $", {num:0.000} s after the last" : $", {cast.Age:0.000} s after the card") + " - " + text);
			cast.Source = arc;
			cast.BlowAt = cast.Age;
		}

		private static void Land(Arc arc)
		{
			if (!arc.Landed)
			{
				arc.Landed = true;
				float num = Mathf.Clamp(Tuning.F("echo.catch_up", 0.06f), 0.01f, 0.5f);
				arc.Hurry = Mathf.Max(arc.Speed, (arc.Far - arc.Radius) / num);
			}
		}

		private static void Hit(Cast cast, Foe foe, bool killed)
		{
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			cast.Struck.Add(foe.Key);
			cast.Hits++;
			Arc source = cast.Source;
			float way;
			if (source != null)
			{
				float num = Along(source, foe);
				way = ((num < 0f) ? (-1f) : 1f);
				float num2 = Mathf.Abs(num);
				if (source.Stage != Stage.Idle && source.Stage != Stage.Unravel && source.Radius < num2 - 24f)
				{
					source.Cuts.Add(new Owed
					{
						Foe = foe,
						Reach = num2,
						Way = way,
						Killed = killed
					});
				}
				else
				{
					Cut(cast, source.Centre, source.SlopeGoal, foe, way, killed);
				}
			}
			else
			{
				way = ((foe.Chest.X < cast.Centre.X) ? (-1f) : 1f);
				Cut(cast, cast.Centre, 0f, foe, way, killed);
			}
			if (killed)
			{
				Catch(cast, foe, way);
			}
		}

		private static void Catch(Cast cast, Foe foe, float way)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			foe.Dead = true;
			cast.Kills++;
			Arc arc = NewArc(cast, foe.Chest);
			arc.Stage = Stage.Whirl;
			arc.Hold = Mathf.Clamp(foe.Width * 0.45f, 55f, 130f);
			arc.Radius = arc.Hold;
			arc.Dir = ((way > 0f) ? ((float)Math.PI) : 0f);
			arc.Spin = 0f - way;
			arc.Lead = arc.Spin;
			arc.Spread = 0.4f;
			cast.Queue.Add(arc);
		}

		private static void Fling(Cast cast, Arc arc, float plan)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			arc.Time = 0f;
			arc.From = arc.Radius;
			arc.Dir0 = arc.Dir;
			arc.Spread0 = arc.Spread;
			arc.SlopeGoal = Fit(arc.Centre, cast, null);
			if (!Aim(cast, arc, null))
			{
				arc.Stage = Stage.Unravel;
				return;
			}
			arc.Stage = Stage.Fling;
			arc.Plan = Mathf.Max(plan, 0.04f);
			arc.Power = PowerFor(arc.From, arc.Near, arc.Far);
			if (arc.Sides != 0)
			{
				float num = ((arc.Sides > 0) ? 0f : ((float)Math.PI));
				arc.Delta = Mathf.PosMod((num - arc.Dir) * arc.Spin, (float)Math.PI * 2f);
			}
		}

		private static void Cut(Cast cast, Vector2 centre, float slope, Foe foe, float way, bool killed)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			Node front = cast.Front;
			if (FxKit.Alive((GodotObject)(object)front) && front.IsInsideTree())
			{
				float num = Mathf.Max(foe.Width / 260f, 1f);
				Vector2 val = new Vector2(Mathf.Cos(slope), Mathf.Sin(slope));
				float num2 = centre.Y + Mathf.Tan(slope) * (foe.Chest.X - centre.X);
				float num3 = foe.Height * 0.22f;
				Vector2 val2 = new Vector2(foe.Chest.X, Mathf.Clamp(num2, foe.Chest.Y - num3, foe.Chest.Y + num3));
				float num4 = Mathf.Max(foe.Width * 1.1f, 140f) * 0.5f;
				Brush.Cut(front, val2 - val * (way * num4), val2 + val * (way * num4), 20f * num, Echo, 0f, 0.05f, 0.03f, 0.24f);
				FxKit.Flash(front, val2, new Color(0.86f, 0.94f, 1f, killed ? 0.72f : 0.5f), (killed ? 90f : 60f) * num, 1.7f, killed ? 0.18 : 0.14);
			}
		}

		private static void Update(double realDt)
		{
			if (Live.Count == 0)
			{
				return;
			}
			float dt = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
			for (int num = Live.Count - 1; num >= 0; num--)
			{
				Cast cast = Live[num];
				bool flag = false;
				try
				{
					flag = Step(cast, dt);
				}
				catch (Exception ex)
				{
					DevLog.Warn("EchoingSlashFx: " + ex);
				}
				if (!flag)
				{
					Free(cast);
					Live.Remove(cast);
				}
			}
		}

		private static void Free(Cast cast)
		{
			try
			{
				if (FxKit.Alive((GodotObject)(object)cast.HolderBack))
				{
					((Node)cast.HolderBack).QueueFree();
				}
				if (FxKit.Alive((GodotObject)(object)cast.HolderFront))
				{
					((Node)cast.HolderFront).QueueFree();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("EchoingSlashFx (freeing): " + ex.Message);
			}
		}

		private static bool Step(Cast cast, float dt)
		{
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			if (cast.Multiplayer && !ScopeAlive(cast))
			{
				return false;
			}
			Node2D val = (FxKit.Alive((GodotObject)(object)cast.HolderFront) ? cast.HolderFront : cast.HolderBack);
			if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree() || !FxKit.Alive((GodotObject)(object)cast.Front))
			{
				return false;
			}
			if (TimeFreeze.HoldsVisual((Node)(object)val))
			{
				return true;
			}
			cast.Age += dt;
			cast.Clock += dt;
			if (cast.Age > 12f)
			{
				return false;
			}
			Act(cast);
			if (cast.Queue.Count > 0 && cast.Queue[0].Stage == Stage.Whirl && !cast.Closed)
			{
				Fling(cast, cast.Queue[0], cast.Gap - (cast.Age - cast.BlowAt));
			}
			for (int num = cast.Arcs.Count - 1; num >= 0; num--)
			{
				Arc arc = cast.Arcs[num];
				Move(cast, arc, dt);
				if (arc.Stage == Stage.Idle)
				{
					foreach (Owed cut in arc.Cuts)
					{
						Cut(cast, arc.Centre, arc.SlopeGoal, cut.Foe, cut.Way, cut.Killed);
					}
					arc.Cuts.Clear();
					cast.Arcs.RemoveAt(num);
					cast.Queue.Remove(arc);
					if (cast.Hers == arc)
					{
						cast.Hers = null;
					}
					if (cast.Source == arc)
					{
						cast.Source = null;
					}
				}
			}
			Show(cast);
			if (cast.Arcs.Count > 0)
			{
				return true;
			}
			if (!cast.Closed)
			{
				if (cast.ActLeft <= 0)
				{
					return cast.Age - cast.BlowAt < 1.5f;
				}
				return true;
			}
			return false;
		}

		private static void Act(Cast cast)
		{
			if (cast.Closed)
			{
				cast.ActLeft = 0;
				return;
			}
			if (!cast.Acting)
			{
				if (cast.Forced < 2 || cast.Blow != 0 || cast.Kills > 0 || cast.Struck.Count == 0 || cast.Age - cast.BlowAt < 0.02f)
				{
					return;
				}
				cast.Acting = true;
				cast.ActLeft = cast.Forced - 1;
				cast.ActAt = cast.BlowAt + cast.Gap;
				int num = 0;
				foreach (Foe foe2 in cast.Foes)
				{
					if (num < cast.Forced - 1 && num < cast.Struck.Count - 1 && cast.Struck.Contains(foe2.Key))
					{
						Pretend(cast, foe2);
						num++;
					}
				}
				DevLog.Info($"Echoing Slash: echo.force_hits {cast.Forced} - {num} enemy(ies) only said to be killed, the other blows are acted");
				return;
			}
			while (cast.ActLeft > 0 && cast.Age >= cast.ActAt)
			{
				if (cast.Blow >= 0 && cast.Queue.Count == 0)
				{
					cast.ActLeft = 0;
					break;
				}
				cast.ActLeft--;
				cast.ActAt = cast.Age + cast.Gap;
				bool flag = cast.Blow < 0;
				NextBlow(cast);
				int num2 = (flag ? cast.ActKills : 0);
				int num3 = 0;
				foreach (Foe foe3 in cast.Foes)
				{
					if (Standing(foe3))
					{
						num3++;
					}
				}
				Foe[] array = cast.Foes.ToArray();
				foreach (Foe foe in array)
				{
					if (Standing(foe))
					{
						bool flag2 = num2 > 0 && (num3 > 1 || cast.Foes.Count == 1);
						if (flag2)
						{
							num2--;
							num3--;
						}
						Hit(cast, foe, flag2);
					}
				}
			}
		}

		private static void Pretend(Cast cast, Foe foe)
		{
			Arc source = cast.Source;
			Cast cast2 = cast;
			Foe foe2 = foe;
			float way;
			if (source == null)
			{
				way = ((foe.Chest.X < cast.Centre.X) ? (-1f) : 1f);
			}
			else
			{
				way = ((Along(source, foe) < 0f) ? (-1f) : 1f);
			}
			Catch(cast2, foe2, way);
		}

		private static void Move(Cast cast, Arc arc, float dt)
		{
			//IL_064b: Unknown result type (might be due to invalid IL or missing references)
			arc.Time += dt;
			float time = arc.Time;
			arc.Slope += (arc.SlopeGoal - arc.Slope) * Mathf.Min(1f, dt / 0.04f);
			float num = Mathf.Clamp(Tuning.F("echo.spin", 15f), 2f, 40f);
			switch (arc.Stage)
			{
			case Stage.Slash:
			{
				float plan2 = arc.Plan;
				arc.Alpha = Mathf.Min(1f, time / 0.03f);
				Advance(arc, dt);
				if (!arc.Two)
				{
					float num4 = Window(arc.Radius);
					float num5 = Mathf.Lerp(2.05f, 0f - num4, Smooth(time / (0.55f * plan2)));
					float num6 = Mathf.Lerp(2.6f, num4, Smooth((time - 0.1f * plan2) / (0.8f * plan2)));
					float num7 = ((arc.Side > 0f) ? 0f : ((float)Math.PI));
					arc.Dir = num7 + arc.Side * (num5 + num6) * 0.5f;
					arc.Spread = (num6 - num5) * 0.5f;
					arc.Lead = (0f - arc.Side) * (1f - Smooth(time / (0.8f * plan2)));
					arc.Both = false;
				}
				else
				{
					float num8 = 0.6f * plan2;
					if (time < num8)
					{
						float num9 = Mathf.Lerp(2.05f, -7.7999997f, Smooth(time / num8));
						arc.Dir = ((arc.Side > 0f) ? ((num9 + 2.6f) * 0.5f) : ((float)Math.PI - (num9 + 2.6f) * 0.5f));
						arc.Spread = (2.6f - num9) * 0.5f;
						arc.Lead = (0f - arc.Side) * (1f - Smooth(time / num8));
						arc.Both = false;
					}
					else
					{
						arc.Both = true;
						arc.Dir = 0f;
						arc.Lead = 0f;
						arc.Spread = Mathf.Lerp(2.6f, Window(arc.Radius), Smooth((time - num8) / (0.3f * plan2)));
					}
				}
				if (!arc.Landed && (cast.Closed || time > plan2 + 0.6f))
				{
					Land(arc);
				}
				break;
			}
			case Stage.Whirl:
				arc.Alpha = Mathf.Min(1f, time / 0.04f);
				arc.Dir += arc.Spin * num * dt;
				arc.Spread = Mathf.Lerp(0.4f, 1.5f, Smooth(time / 0.08f));
				arc.Lead = arc.Spin;
				arc.Both = false;
				arc.Radius = arc.Hold * (1f + 0.05f * Mathf.Sin(time * 20f));
				arc.Wake = 70f;
				if (time > 1.2f || cast.Closed || (time > 0.25f && !AnyStanding(cast)))
				{
					arc.Time = 0f;
					arc.From = arc.Radius;
					arc.Spread0 = arc.Spread;
					arc.Stage = Stage.Unravel;
				}
				break;
			case Stage.Fling:
			{
				float plan = arc.Plan;
				arc.Alpha = Mathf.Min(1f, arc.Alpha + dt / 0.04f);
				Advance(arc, dt);
				if (arc.Sides != 0)
				{
					arc.Dir = arc.Dir0 + arc.Spin * arc.Delta * Smooth(time / (0.5f * plan));
					arc.Spread = Mathf.Lerp(arc.Spread0, Window(arc.Radius), Smooth(time / (0.6f * plan)));
					arc.Lead = arc.Spin * (1f - Smooth(time / (0.6f * plan)));
					arc.Both = false;
				}
				else
				{
					float num3 = 0.45f * plan;
					if (time < num3)
					{
						arc.Spread = Mathf.Lerp(arc.Spread0, 5.2f, Smooth(time / num3));
						arc.Dir = arc.Dir0 + arc.Spin * (arc.Spread - arc.Spread0);
						arc.Lead = arc.Spin * (1f - Smooth(time / num3));
						arc.Both = false;
					}
					else
					{
						arc.Both = true;
						arc.Dir = 0f;
						arc.Lead = 0f;
						arc.Spread = Mathf.Lerp(2.6f, Window(arc.Radius), Smooth((time - num3) / (0.3f * plan)));
					}
				}
				if (!arc.Landed && (cast.Closed || time > plan + 0.6f))
				{
					Land(arc);
				}
				break;
			}
			case Stage.Unravel:
			{
				float num2 = Mathf.Min(time / 0.3f, 1f);
				arc.Spread = Mathf.Lerp(arc.Spread0, 5.2f, Smooth(num2 * 1.6f));
				arc.Dir += arc.Spin * num * dt * (1f - num2);
				arc.Lead = arc.Spin * (1f - Smooth(num2 * 1.6f));
				arc.Both = false;
				arc.Radius = arc.From * (1f + 0.9f * (1f - (1f - num2) * (1f - num2)));
				arc.Erode = num2;
				arc.Alpha = 1f - num2 * num2;
				arc.Wake = 60f;
				if (num2 >= 1f)
				{
					arc.Stage = Stage.Idle;
					arc.Alpha = 0f;
				}
				break;
			}
			}
			for (int num10 = arc.Cuts.Count - 1; num10 >= 0; num10--)
			{
				Owed owed = arc.Cuts[num10];
				if (arc.Stage == Stage.Idle || !(arc.Radius < owed.Reach - 24f))
				{
					arc.Cuts.RemoveAt(num10);
					Cut(cast, arc.Centre, arc.SlopeGoal, owed.Foe, owed.Way, owed.Killed);
				}
			}
		}

		private static void Advance(Arc arc, float dt)
		{
			float radius = arc.Radius;
			float num = Mathf.Max(0f, arc.Far - arc.From) / Mathf.Max(arc.Plan, 0.01f);
			if (arc.Landed && arc.Radius >= arc.Far)
			{
				float num2 = Mathf.Min(arc.Speed, 4000f);
				num2 += (900f - num2) * (1f - Mathf.Exp((0f - dt) / 0.05f));
				arc.Radius += num2 * dt;
				arc.Speed = num2;
				float num3 = Mathf.Max(0.05f, Tuning.F("echo.fade", 0.26f));
				arc.Fade += dt / num3;
				arc.Erode = Mathf.Min(1f, arc.Fade);
				arc.Alpha = Mathf.Min(arc.Alpha, Mathf.Max(0f, 1f - Mathf.Pow(Mathf.Min(arc.Fade, 1f), 1.6f)));
				if (arc.Fade >= 1f)
				{
					arc.Stage = Stage.Idle;
					arc.Alpha = 0f;
				}
			}
			else
			{
				float num4 = arc.Time / Mathf.Max(arc.Plan, 0.01f);
				if (arc.Landed)
				{
					arc.Radius += arc.Hurry * dt;
				}
				else if (num4 <= 1f)
				{
					arc.Radius = Mathf.Max(arc.Radius, arc.From + (arc.Far - arc.From) * Mathf.Pow(num4, arc.Power));
				}
				else
				{
					float num5 = num * arc.Power * Mathf.Exp((0f - (arc.Time - arc.Plan)) / 0.05f) + num * 0.08f;
					arc.Radius = Mathf.Min(arc.Radius + num5 * dt, Mathf.Max(arc.Radius, arc.Far + 40f));
				}
				if (dt > 0.0001f)
				{
					arc.Speed = Mathf.Max(0f, (arc.Radius - radius) / dt);
				}
			}
			arc.Wake = Mathf.Clamp(arc.Speed * 0.045f, 60f, 260f);
		}

		private static void Show(Cast cast)
		{
			int i = 0;
			foreach (Arc arc in cast.Arcs)
			{
				if (i >= 6)
				{
					break;
				}
				if (arc.Stage != Stage.Idle && !(arc.Alpha <= 0.002f))
				{
					cast.X[i] = arc.Centre.X;
					cast.Y[i] = arc.Centre.Y;
					cast.Cos[i] = Mathf.Cos(arc.Slope);
					cast.Sin[i] = Mathf.Sin(arc.Slope);
					cast.R[i] = arc.Radius;
					cast.Dir[i] = arc.Dir;
					cast.Spread[i] = arc.Spread;
					cast.Lead[i] = arc.Lead;
					cast.Both[i] = (arc.Both ? 1f : 0f);
					cast.Alpha[i] = Mathf.Clamp(arc.Alpha, 0f, 1f);
					cast.Erode[i] = Mathf.Clamp(arc.Erode, 0f, 1f);
					cast.Wake[i] = arc.Wake;
					cast.Seed[i] = arc.Seed;
					i++;
				}
			}
			bool visible = i > 0;
			for (; i < 6; i++)
			{
				cast.Alpha[i] = 0f;
			}
			if (FxKit.Alive((GodotObject)(object)cast.HolderBack))
			{
				((CanvasItem)cast.HolderBack).Visible = visible;
			}
			if (FxKit.Alive((GodotObject)(object)cast.HolderFront))
			{
				((CanvasItem)cast.HolderFront).Visible = visible;
			}
			Hand(cast, cast.MatBack);
			Hand(cast, cast.MatFront);
		}

		private static void Hand(Cast cast, ShaderMaterial mat)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)mat))
			{
				mat.SetShaderParameter(StringName.op_Implicit("clock"), Variant.op_Implicit(cast.Clock));
				mat.SetShaderParameter(StringName.op_Implicit("a_x"), Variant.op_Implicit(cast.X));
				mat.SetShaderParameter(StringName.op_Implicit("a_y"), Variant.op_Implicit(cast.Y));
				mat.SetShaderParameter(StringName.op_Implicit("a_cos"), Variant.op_Implicit(cast.Cos));
				mat.SetShaderParameter(StringName.op_Implicit("a_sin"), Variant.op_Implicit(cast.Sin));
				mat.SetShaderParameter(StringName.op_Implicit("a_r"), Variant.op_Implicit(cast.R));
				mat.SetShaderParameter(StringName.op_Implicit("a_dir"), Variant.op_Implicit(cast.Dir));
				mat.SetShaderParameter(StringName.op_Implicit("a_spread"), Variant.op_Implicit(cast.Spread));
				mat.SetShaderParameter(StringName.op_Implicit("a_lead"), Variant.op_Implicit(cast.Lead));
				mat.SetShaderParameter(StringName.op_Implicit("a_both"), Variant.op_Implicit(cast.Both));
				mat.SetShaderParameter(StringName.op_Implicit("a_alpha"), Variant.op_Implicit(cast.Alpha));
				mat.SetShaderParameter(StringName.op_Implicit("a_erode"), Variant.op_Implicit(cast.Erode));
				mat.SetShaderParameter(StringName.op_Implicit("a_wake"), Variant.op_Implicit(cast.Wake));
				mat.SetShaderParameter(StringName.op_Implicit("a_seed"), Variant.op_Implicit(cast.Seed));
			}
		}

		static EchoingSlashFx()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		}
	}
