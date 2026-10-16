extends Node
# 压制（先古卡）在游戏里的整套表演。C# 的 SuppressRun 每帧调 step()，出事的时候调 lift / aim / unaim / release /
# play_auto / hit / cancel / dismiss。三层（back 在角色身后、front 在角色身前、top 在整个界面之上）的
# GlobalTransform 都是单位阵，所以这里的坐标就是场景坐标。
#
# 流程：悬停 → 四边淡绿雾。拿起 → 牌在鼠标处啪地炸成雾，雾里的链子甩回右边落进她手里，她淡淡跪着成形；
# 链头就是鼠标。指到敌人 → 唰地被吸过去咬住，魂在他身上隐约一亮，光点顺链子往她手里流，她身体不动。
# 松手 → 沉 2 帧 → 化雾冲起 2 帧 → 凝回（链子绷直）→ 等游戏真的打中 → 撕魂：敌人被往她这边猛拽、顿住、
# 炸雾碎光、全屏绿闪 → 魂沿弧线飞进她举起的手 → 定格 → 她和魂一起散成光点。放回 → 链子缩回链头、重新凝成牌。

const F1 := 1.0 / 30.0
const PW := 1456.0                 # 原型画面的宽、高（她的姿势坐标都在这个画面里）
const PH := 817.0
const POSE := {
	"k2": {"pos": Vector2(984, 59), "front": Vector2(1059.2, 491.1), "back": Vector2(1278.8, 385.5)},
	"k25": {"pos": Vector2(973, 51), "front": Vector2(1044.3, 301.8), "back": Vector2(1314.0, 312.8)},
	"k3": {"pos": Vector2(954, 38), "front": Vector2(1059.6, 199.9), "back": Vector2(1317.4, 258.0)},
	"k4": {"pos": Vector2(934.1, 62.7), "front": Vector2(1009.3, 103.6), "back": Vector2(1290.5, 220.2)},
	"k5": {"pos": Vector2(942.8, 61.9), "front": Vector2(1032.1, 130.5), "back": Vector2(1299.2, 226.5)},
}
const SKEW := Vector2(-14, 22)     # 沉到底的位移（原型像素）
const TEX_K := 0.44 / 0.7          # 原型里贴图缩 0.44；这里的贴图是原图的 0.7
const FREEZE := 0.12
const FADE := 0.70
const HS2 := 3.0 / 30.0            # 撕魂那一下顿住（约 0.1 秒）
const SLOW := 0.55                 # 魂到手定格、回到余势以后，收尾按这个速度慢放（只是表演，游戏照常）
const SETTLE := 0.12               # 定格之后先按正常速度回到余势（k5），再开始慢放和散

var DIR := ""
var CACHE: Dictionary = {}
var back: Node2D
var front: Node2D
var top: Node2D
var lo := Vector2.ZERO
var hi := Vector2(1920, 1080)
var S := 1.0                       # 场景像素 / 原型像素
var size_k := 1.0
var max_wait := 0.8                # 松手以后最多等游戏的真伤害多久
# [MP] Optional backend ownership flag; local/default animation is unchanged.
var remote_ui := false

# —— 节点
var her := {}
var hmat: ShaderMaterial
var ghosts: Array = []
var aura_b: Sprite2D
var aura_f: Sprite2D
var slines: Array = []
var hg_f: Sprite2D
var hg_b: Sprite2D
var column: Sprite2D
var rope_bk: MeshInstance2D
var rope_fr: MeshInstance2D
var rmat: ShaderMaterial
var fogb: Sprite2D
var veil: Sprite2D
var lurk: Sprite2D
var lmat: ShaderMaterial
var tether: Sprite2D
var tmat: ShaderMaterial
var soul: Sprite2D
var smat: ShaderMaterial
var sglow: Sprite2D
var gnd: Sprite2D
var head_glow: Sprite2D
var fingers: Array = []
var flash: Sprite2D
var flash2: Sprite2D
var ring1: Sprite2D
var ring2: Sprite2D
var ring3: Sprite2D
var t_sburst := -99.0             # 魂炸开的那一刻
var pool: Array = []
var atmos: Sprite2D
var amat: ShaderMaterial
var white: Sprite2D
var dark_top: Sprite2D
var grade: Sprite2D
var ripple: Sprite2D
var ripple_mat: ShaderMaterial
var burst_q: Sprite2D
var burst_mat: ShaderMaterial
var rope_tail: MeshInstance2D
var tail_mat: ShaderMaterial
var top_pool: Array = []
var TP: Array = []                 # 界面上往上飘的光点
var tp_acc := 0.0
var pclock := 0.0                  # 光点的钟（收尾时跟着慢下来）
var prate := 1.0
var flow_acc := 0.0
var grade_g := 0.0
var effect_k := 1.0
var flash_k := 1.0
var shake2 := false
var dot_tex: Texture2D
var white_tex: Texture2D

# —— 状态
var phase := "hover"               # hover / free / aim / cancel / rel / tear / gone
var clock := 0.0
var presence_ext := 1.0
var mouse := Vector2.ZERO
var t_lift := -99.0
var t_aim := -99.0
var t_unaim := -99.0
var t_rel := -99.0
var t_snap := -99.0
var t_tear := -99.0
var t_cancel := -99.0
var t_dismiss := -99.0
var t_hit := -99.0
var hit_in := false
var auto := false
var has_target := false
var tfeet := Vector2.ZERO
var tchest := Vector2.ZERO
var tw := 200.0
var th := 240.0
var epos := Vector2.ZERO
var esize := Vector2(200, 240)
var G := Vector2.ZERO              # 魂被攥住的那一点（在敌人身上）
var head := Vector2.ZERO           # 链头
var head_v := Vector2.ZERO
var head_from := Vector2.ZERO      # 唰的起点
var snapped_flash := false
var arrived := false
var flight := 0.15
var soul_tex: Texture2D = null
var soul_tpos := Vector2.ZERO
var soul_tsize := Vector2.ONE
var atm := 0.0
var veil_a := 0.0
var fog_a := 0.0
var P: Array = []                  # 光点
var riders: Array = []             # 顺着链子流的光点
var rider_acc := 0.0
var pulse_t := 0.0
var out_shake := 0
var out_hold := 0.0
var card_show := false
var cues: Array = []               # 这一帧该响的声音（C# 读了就清）：lift / bite / tear / burst
var last_pts: Array = []
var rng := RandomNumberGenerator.new()

# ------------------------------------------------------------------ 小工具

func s01(a: float, b: float, x: float) -> float: return smoothstep(a, b, x)
func eout(u: float) -> float:
	u = clampf(u, 0.0, 1.0)
	return 1.0 - pow(1.0 - u, 3.0)
func damp(a: float, b: float, k: float, dt: float) -> float: return lerpf(b, a, exp(-k * dt))
func H(p: Vector2) -> Vector2: return Vector2(hi.x + (p.x - PW) * S, lo.y + p.y * S)

const TEXES := ["her_k2.png", "her_k25.png", "her_k3.png", "her_k4.png", "her_k5.png", "aura_k2.png", "aura_k25.png", "aura_k3.png", "aura_k4.png", "aura_k5.png"]

func tex(nm: String) -> Texture2D:
	var key := "tex/" + nm
	if CACHE.has(key): return CACHE[key]
	var im := Image.load_from_file(DIR + "/tex/" + nm)
	if im == null or im.is_empty(): return null
	var t := ImageTexture.create_from_image(im)
	CACHE[key] = t
	if nm == "her_k5.png": CACHE["img/" + nm] = im     # 散的时候从她的轮廓里取点（不从显卡读回）
	return t

# 提前读图：每调一次读一张还没读的，全读完返回 true（C# 在手里有压制时一帧调一次）
func warm(p_dir: String, p_cache: Dictionary) -> bool:
	DIR = p_dir; CACHE = p_cache
	for nm in TEXES:
		if not CACHE.has("tex/" + nm):
			tex(nm)
			return false
	for f in ["her.gdshader", "aura.gdshader", "lurk.gdshader", "fog.gdshader", "soul.gdshader", "tether.gdshader", "rope.gdshader", "smear.gdshader", "atmos.gdshader", "ripple.gdshader", "burst.gdshader"]:
		if not CACHE.has("sh/" + f):
			shader(f)
			return false
	return true

func shader(file: String) -> Shader:
	var key := "sh/" + file
	if CACHE.has(key): return CACHE[key]
	var sh := Shader.new()
	sh.code = FileAccess.get_file_as_string(DIR + "/" + file)
	CACHE[key] = sh
	return sh

func smat_new(file: String) -> ShaderMaterial:
	var m := ShaderMaterial.new()
	m.shader = shader(file)
	return m

func add_mat() -> CanvasItemMaterial:
	var m := CanvasItemMaterial.new()
	m.blend_mode = CanvasItemMaterial.BLEND_MODE_ADD
	return m

func radial(c: Color) -> GradientTexture2D:
	var g := Gradient.new(); g.set_color(0, c); g.set_color(1, Color(c.r, c.g, c.b, 0.0))
	var t := GradientTexture2D.new(); t.gradient = g; t.fill = GradientTexture2D.FILL_RADIAL
	t.fill_from = Vector2(0.5, 0.5); t.fill_to = Vector2(1, 0.5); t.width = 128; t.height = 128
	return t

func ring_tex() -> ImageTexture:
	if CACHE.has("ring"): return CACHE["ring"]
	var im := Image.create(128, 128, false, Image.FORMAT_RGBA8)
	for y in 128:
		for x in 128:
			var r := Vector2(x - 63.5, y - 63.5).length()
			var a := exp(-pow((r - 52.0) / 4.5, 2.0)) + 0.25 * exp(-pow((r - 46.0) / 8.0, 2.0))
			im.set_pixel(x, y, Color(1, 1, 1, clampf(a, 0.0, 1.0)))
	var t := ImageTexture.create_from_image(im)
	CACHE["ring"] = t
	return t

# 一块盖住矩形的精灵（用来挂着色器；不是 Control，不会挡鼠标）
func quad(parent: Node, m: Material) -> Sprite2D:
	var s := Sprite2D.new(); s.texture = white_tex; s.centered = false; s.material = m
	parent.add_child(s)
	return s

func place_quad(s: Sprite2D, a: Vector2, b: Vector2) -> void:
	s.position = a; s.scale = (b - a).max(Vector2.ONE) / 4.0

func spr(parent: Node, t: Texture2D, additive := true) -> Sprite2D:
	var s := Sprite2D.new(); s.texture = t
	if additive: s.material = add_mat()
	s.visible = false
	parent.add_child(s)
	return s

# ------------------------------------------------------------------ 搭起来

func build(p_dir: String, p_cache: Dictionary, p_back: Node, p_front: Node, p_lo: Vector2, p_hi: Vector2, params: Dictionary) -> bool:
	DIR = p_dir; CACHE = p_cache
	back = p_back; front = p_front
	lo = p_lo; hi = p_hi
	size_k = clampf(float(params.get("size", 1.0)), 0.5, 1.6)
	max_wait = clampf(float(params.get("hit_wait", 0.8)), 0.1, 2.0)
	# [MP] A remote chain cannot originate from the viewer's card UI.
	remote_ui = bool(params.get("remote_ui", false))
	S = (hi.y - lo.y) / PH * size_k
	rng.seed = 2531
	for nm in TEXES:
		if tex(nm) == null: return false
	var wim := Image.create(4, 4, false, Image.FORMAT_RGBA8); wim.fill(Color(1, 1, 1, 1))
	white_tex = ImageTexture.create_from_image(wim)
	dot_tex = radial(Color(1, 1, 1, 1))
	# 身后：压暗、地雾、她的雾、她、拖影线、手光、链子
	veil = quad(back, null); veil.modulate = Color(0, 0.02, 0.03, 0.0); place_quad(veil, lo, hi)
	fogb = quad(back, smat_new("fog.gdshader")); place_quad(fogb, lo, hi)
	fogb.material.set_shader_parameter("ws", S); fogb.material.set_shader_parameter("tint", Color(0.10, 0.16, 0.16))
	fogb.material.set_shader_parameter("y0", lo.y + 60.0 * S); fogb.material.set_shader_parameter("y1", lo.y + 560.0 * S)
	aura_b = Sprite2D.new(); aura_b.centered = false; aura_b.material = smat_new("aura.gdshader"); back.add_child(aura_b)
	hmat = smat_new("her.gdshader")
	hmat.set_shader_parameter("pxk", 0.7 / 0.44); hmat.set_shader_parameter("ws", S)
	hmat.set_shader_parameter("dark", 0.72); hmat.set_shader_parameter("ghost", 1.0)
	hmat.set_shader_parameter("feet_y", lo.y + 590.0 * S); hmat.set_shader_parameter("fade", 190.0 * S)
	for i in 3:
		var g := Sprite2D.new(); g.centered = false; g.material = add_mat(); g.visible = false; back.add_child(g); ghosts.append(g)
	for k in POSE:
		var sp := Sprite2D.new(); sp.texture = tex("her_%s.png" % k); sp.centered = false; sp.material = hmat; sp.visible = false
		back.add_child(sp); her[k] = sp
	aura_f = Sprite2D.new(); aura_f.centered = false; aura_f.material = smat_new("aura.gdshader"); back.add_child(aura_f)
	for m in [aura_b.material, aura_f.material]: m.set_shader_parameter("ws", S)
	for k in 2:
		var sl := MeshInstance2D.new(); sl.material = smat_new("smear.gdshader"); sl.visible = false; back.add_child(sl); slines.append(sl)
	hg_f = spr(back, dot_tex); hg_b = spr(back, dot_tex); column = spr(back, dot_tex)
	rmat = smat_new("rope.gdshader"); rmat.set_shader_parameter("ws", S)
	tail_mat = smat_new("rope.gdshader"); tail_mat.set_shader_parameter("ws", S)
	rope_tail = MeshInstance2D.new(); rope_tail.material = tail_mat; rope_tail.visible = false; back.add_child(rope_tail)
	rope_bk = MeshInstance2D.new(); rope_bk.material = rmat; back.add_child(rope_bk)
	# 身前：敌人脚下的雾、魂、魂的尾巴、链子、链头、光点、闪光
	lmat = smat_new("lurk.gdshader"); lmat.set_shader_parameter("ws", S)
	lurk = quad(front, lmat); lurk.visible = false
	gnd = spr(front, radial(Color(0.45, 0.85, 0.80, 1.0)))
	tmat = smat_new("tether.gdshader"); tmat.set_shader_parameter("ws", S); tmat.set_shader_parameter("width", 64.0 * S)
	tether = quad(front, tmat); tether.visible = false
	smat = smat_new("soul.gdshader"); smat.set_shader_parameter("ws", S); smat.set_shader_parameter("has_tex", 0.0)
	soul = quad(front, smat); soul.visible = false
	sglow = spr(front, radial(Color(0.8, 1.0, 0.97, 1.0)))
	rope_fr = MeshInstance2D.new(); rope_fr.material = rmat; rope_fr.visible = false; front.add_child(rope_fr)
	for k in 3:
		var fm := MeshInstance2D.new(); var fmat := smat_new("rope.gdshader"); fmat.set_shader_parameter("knots", 0.0); fmat.set_shader_parameter("ws", S)
		fm.material = fmat; fm.visible = false; front.add_child(fm); fingers.append(fm)
	head_glow = spr(front, dot_tex)
	var pd := radial(Color(1, 1, 1, 1))
	for i in 220:
		var sp := Sprite2D.new(); sp.texture = pd; sp.material = add_mat(); sp.visible = false; front.add_child(sp); pool.append(sp)
	burst_mat = smat_new("burst.gdshader"); burst_mat.set_shader_parameter("ws", S)
	burst_q = quad(front, burst_mat); burst_q.visible = false
	flash = spr(front, radial(Color(0.8, 1.0, 0.97, 1.0))); flash2 = spr(front, flash.texture)
	ring1 = spr(front, ring_tex()); ring2 = spr(front, ring1.texture); ring3 = spr(front, ring1.texture)
	return true

# 整个界面之上的一层：全局绿雾和全屏闪
func set_top(p_top: Node) -> bool:
	if p_top == null: return false
	top = p_top
	amat = smat_new("atmos.gdshader"); amat.set_shader_parameter("ws", S)
	amat.set_shader_parameter("lo", lo); amat.set_shader_parameter("hi", hi)
	var mul := CanvasItemMaterial.new(); mul.blend_mode = CanvasItemMaterial.BLEND_MODE_MUL
	grade = quad(top, mul); place_quad(grade, lo, hi); grade.visible = false
	atmos = quad(top, amat); place_quad(atmos, lo, hi); atmos.visible = false
	var pd := radial(Color(1, 1, 1, 1))
	for i in 48:
		var sp := Sprite2D.new(); sp.texture = pd; sp.material = add_mat(); sp.visible = false; top.add_child(sp); top_pool.append(sp)
	var bbc := BackBufferCopy.new(); bbc.copy_mode = BackBufferCopy.COPY_MODE_VIEWPORT; top.add_child(bbc)
	ripple_mat = smat_new("ripple.gdshader"); ripple_mat.set_shader_parameter("span", hi - lo)
	ripple = quad(top, ripple_mat); ripple.visible = false
	white = quad(top, add_mat()); place_quad(white, lo, hi); white.modulate = Color(0.85, 1.0, 0.97, 0.0)
	dark_top = quad(top, null); place_quad(dark_top, lo, hi); dark_top.modulate = Color(0.0, 0.03, 0.02, 0.0); dark_top.visible = false
	return true

func set_soul(t: Texture2D, tpos: Vector2, tsize: Vector2) -> void:
	soul_tex = t; soul_tpos = tpos; soul_tsize = tsize
	if smat == null: return
	if t != null:
		smat.set_shader_parameter("etex", t); smat.set_shader_parameter("has_tex", 1.0)
		smat.set_shader_parameter("tpos", tpos); smat.set_shader_parameter("tsize", tsize)
	else:
		smat.set_shader_parameter("has_tex", 0.0)

# ------------------------------------------------------------------ 事件

func lift(at: Vector2) -> void:
	if phase != "hover": return
	phase = "free"; t_lift = clock; mouse = at; head = at; head_v = Vector2.ZERO
	cues.append("lift")
	# [MP] Keep the original character/chain state, omitting only the card-origin pop particles.
	if remote_ui: return
	# 牌啪地炸成雾
	for i in 26:
		var an := rng.randf_range(0.0, TAU)
		addp(clock, rng.randf_range(0.20, 0.38), at, Vector2(cos(an), sin(an)) * rng.randf_range(260, 760) * S, 7.0, Vector2.ZERO, 0.030 * S, 0.008 * S, Color(0.80, 1.0, 0.95, 0.85), 2.0, 0.01)
	for i in 12:
		var an := rng.randf_range(0.0, TAU)
		addp(clock, rng.randf_range(0.35, 0.6), at + Vector2(cos(an), sin(an)) * 20.0 * S, Vector2(cos(an), sin(an)) * rng.randf_range(80, 220) * S, 4.0, Vector2(0, -30) * S, 0.18 * S, 0.42 * S, Color(0.35, 0.65, 0.58, 0.22), 0.0, 0.05)

func set_target(feet: Vector2, chest: Vector2, w: float, h: float) -> void:
	has_target = true
	tfeet = feet; tchest = chest; tw = maxf(w, 60.0); th = maxf(h, 60.0)
	esize = Vector2(tw, th) * 1.08
	epos = Vector2(feet.x - esize.x * 0.5, feet.y - esize.y)
	G = epos + Vector2(esize.x * 0.5, esize.y * 0.2)
	if smat != null:
		smat.set_shader_parameter("epos", epos); smat.set_shader_parameter("esize", esize)
		if soul_tex == null:
			smat.set_shader_parameter("tpos", epos); smat.set_shader_parameter("tsize", esize)

func aim(feet: Vector2, chest: Vector2, w: float, h: float) -> void:
	if phase != "free" and phase != "aim": return
	if phase == "aim" and feet.distance_to(tfeet) < 1.0:
		set_target(feet, chest, w, h)
		return
	set_target(feet, chest, w, h)
	phase = "aim"; t_aim = clock; head_from = head; pulse_t = 0.35
	cues.append("bite")
	# 唰：咬住的那一下
	for i in 12:
		var an := rng.randf_range(0.0, TAU)
		addp(clock + 0.08, rng.randf_range(0.16, 0.28), G, Vector2(cos(an), sin(an)) * rng.randf_range(200, 520) * S, 8.0, Vector2.ZERO, 0.024 * S, 0.006 * S, Color(0.85, 1.0, 0.97, 0.8), 2.0, 0.01)

func unaim() -> void:
	if phase != "aim": return
	phase = "free"; t_unaim = clock; has_target = false; head_v = Vector2.ZERO

func release() -> void:
	if phase != "aim": return
	phase = "rel"; t_rel = clock
	# 膝下的雾被蹬开
	var knee := H(Vector2(1185, 578))
	for i in 14:
		var d := Vector2(rng.randf_range(-1.0, 1.0), rng.randf_range(-0.35, 0.02)).normalized()
		addp(clock + rng.randf_range(0.0, 0.05), rng.randf_range(0.35, 0.55), knee + Vector2(rng.randf_range(-40, 40), 0) * S, d * rng.randf_range(260, 560) * S, 5.0, Vector2(0, -40) * S, 0.12 * S, 0.34 * S, Color(0.30, 0.50, 0.48, 0.14), 0.0, 0.05)
	# 化雾冲起时竖着往上窜的光条
	for i in 30:
		var p0 := H(Vector2(rng.randf_range(990, 1330), rng.randf_range(300, 590)))
		addp(clock + rng.randf_range(2.0 * F1 - 0.03, 4.0 * F1 - 0.03), rng.randf_range(0.07, 0.11), p0, Vector2(rng.randf_range(-90, 40), -rng.randf_range(1400, 2300)) * S, 2.0, Vector2.ZERO, 0.032 * S, 0.012 * S, Color(0.70, 1.0, 0.95, 0.75), 5.0, 0.01)

# 没有待命就被打出（键盘快捷键等）：她当场出现，链子当场咬住
func play_auto(feet: Vector2, chest: Vector2, w: float, h: float) -> void:
	if phase == "rel" or phase == "tear" or phase == "gone": return
	if phase == "aim" and feet.distance_to(tfeet) < 1.0:
		release()
		return
	auto = phase == "hover"
	if not auto: t_lift = minf(t_lift, clock - 0.3)
	set_target(feet, chest, w, h)
	head = G; head_from = G; t_aim = clock - 0.3
	if auto: t_lift = clock - 0.3
	phase = "aim"
	cues.append("bite")
	release()

func hit(killed: bool) -> void:
	if not hit_in:
		hit_in = true; t_hit = clock

func cancel(at: Vector2) -> void:
	if phase == "free" or phase == "aim":
		phase = "cancel"; t_cancel = clock; mouse = at

func dismiss() -> void:
	if t_dismiss < 0.0: t_dismiss = clock

# ------------------------------------------------------------------ 每帧

func step(dt: float, p_mouse: Vector2, presence: float, reduce_effects := false) -> Dictionary:
	effect_k = 0.55 if reduce_effects else 1.0
	flash_k = 0.35 if reduce_effects else 1.0
	dt = clampf(dt, 0.0, 0.1)
	clock += dt
	pclock += dt * prate
	mouse = p_mouse
	presence_ext = clampf(presence, 0.0, 1.0)
	out_shake = 0; out_hold = 0.0
	var yank := Vector2.ZERO

	# —— 松手以后：凝回、撕魂
	if phase == "rel":
		var ta := clock - t_rel
		if ta >= 4.0 * F1 and t_snap < 0.0:
			t_snap = clock; out_shake = 1; snap_fx()
		if t_snap >= 0.0 and (hit_in or ta > 4.0 * F1 + max_wait):
			phase = "tear"; t_tear = clock; out_shake = 3; out_hold = 0.26; tear_fx(); cues.append("tear")
	var tt := clock - t_tear
	var te := maxf(0.0, tt - HS2)
	var tm := te
	if phase == "tear":
		if te >= flight and te < flight + FREEZE: tm = flight
		elif te >= flight + FREEZE and te < flight + FREEZE + SETTLE: tm = te - FREEZE
		elif te >= flight + FREEZE + SETTLE: tm = flight + SETTLE + (te - flight - FREEZE - SETTLE) * SLOW
		# 收尾慢放：光点的钟也跟着慢下来（回到余势以后才慢）
		prate = lerpf(1.0, SLOW, s01(flight + FREEZE + SETTLE, flight + FREEZE + SETTLE + 0.12, te))
		if tt >= HS2 + 0.04 and not shake2:
			shake2 = true; out_shake = 1
		if te >= flight and not arrived:
			arrived = true; out_shake = 2; arrive_fx()
		if tm >= flight + SETTLE and t_sburst < 0.0:
			t_sburst = clock; out_shake = maxi(out_shake, 1); soul_burst_fx(); cues.append("burst")
		var mag := 62.0 * S * s01(0.0, 0.035, tt) * exp(-maxf(tt - HS2, 0.0) * 7.0)
		var hand := hand_pos("k4", "front")
		yank = (hand - G).normalized() * mag

	# —— 她
	var p := pose_now(tm)
	var hs: Sprite2D = her[p]
	for k in her: her[k].visible = (k == p)
	var off := Vector2.ZERO
	var sc := Vector2.ONE
	var ta2 := clock - t_rel
	if phase == "rel" or phase == "tear":
		if ta2 < 2.0 * F1:
			var u := clampf(0.6 + 0.4 * ta2 / F1, 0.0, 1.0)
			off = SKEW * u; sc = Vector2(1.0 + 0.03 * u, 1.0 - 0.075 * u)
		elif ta2 < 4.0 * F1:
			off = Vector2(-4, 6) if p == "k25" else Vector2(-2, 2)
		elif phase == "rel":
			var u2 := s01(0.0, 0.3, clock - t_snap)
			off = Vector2(-4, -4) * u2
			var st := 1.0 - s01(0.0, 2.0 * F1, clock - t_snap)
			sc = Vector2(1.0 - 0.015 * st, 1.0 + 0.03 * st)
		else:
			if tm < 2.0 * F1: off = Vector2(-4, -4)
			else: off = Vector2(5, -3) * exp(-(tm - 2.0 * F1) * 6.0)
	var tw_ := float(hs.texture.get_width()); var th_ := float(hs.texture.get_height())
	var k_tex := S * TEX_K
	var pos: Vector2 = H(POSE[p].pos + off)
	var anchor := Vector2(tw_ * (1.0 - sc.x) * 0.5, th_ * (1.0 - sc.y) * 0.9) * k_tex
	hs.scale = sc * k_tex; hs.position = pos + anchor
	var fr: Vector2 = hs.position + (POSE[p].front - POSE[p].pos) * S * sc
	var bk: Vector2 = hs.position + (POSE[p].back - POSE[p].pos) * S * sc

	var shown := her_presence()
	var ghostv := 1.0
	var smv := Vector2.ZERO
	var mistv := 0.0
	var diss := 0.0
	var aura := 0.30 * shown / 0.30 if shown < 0.30 else 0.30
	if phase == "rel" or phase == "tear":
		var r := clock - t_rel
		if r < 2.0 * F1:
			shown = lerpf(0.30, 0.5, s01(0.0, 2.0 * F1, r + 0.02)); aura = lerpf(0.30, 0.5, s01(0.0, 2.0 * F1, r + 0.02))
		elif r < 4.0 * F1:
			shown = 0.70 if p == "k25" else 0.90
			aura = 0.55 if p == "k25" else 0.45
			smv = Vector2(-0.12, -1.0) * (130.0 if p == "k25" else 65.0) * (0.7 / 0.44)
			mistv = 0.6 if p == "k25" else 0.3
			ghostv = 1.0 - 0.55 * (0.0 if p == "k25" else 1.0)
		else:
			shown = 1.0; ghostv = 0.45; aura = 0.55
			if phase == "tear":
				aura = 0.55 + 0.35 * s01(flight, flight + 0.3, tm)
				ghostv = 1.0 - 0.55 * (1.0 - s01(flight, flight + 0.35, tm))
				diss = s01(flight + SETTLE, flight + SETTLE + FADE, tm) * 1.15
				aura *= 1.0 - s01(flight + 0.3, flight + SETTLE + FADE, tm)
		if auto: shown *= s01(0.0, 0.1, r)
	hmat.set_shader_parameter("presence", shown * presence_ext)
	hs.visible = shown * presence_ext > 0.003
	hmat.set_shader_parameter("ghost", ghostv); hmat.set_shader_parameter("dissolve", diss); hmat.set_shader_parameter("t", clock)
	hmat.set_shader_parameter("smear", smv); hmat.set_shader_parameter("mist", mistv)
	her_light(fr, bk, tm)
	ghost_trails(p, tm)
	aura_b.texture = tex("aura_%s.png" % p); aura_f.texture = aura_b.texture
	aura_b.position = hs.position - Vector2(80, 80) * S; aura_f.position = aura_b.position
	aura_b.scale = Vector2.ONE * S * (hs.scale.x / k_tex); aura_f.scale = aura_b.scale
	var aamt := aura * presence_ext
	aura_b.visible = aamt > 0.003; aura_f.visible = aura_b.visible
	aura_b.material.set_shader_parameter("amt", aamt); aura_f.material.set_shader_parameter("amt", aamt * 0.55)
	aura_b.material.set_shader_parameter("t", clock); aura_f.material.set_shader_parameter("t", clock + 3.0)
	hand_smears()

	# —— 魂、链子、敌人脚下的雾
	var grip := soul_grip(tm)
	update_soul(grip, tm)
	update_chain(fr, bk, grip, tm, dt)
	update_lurk(tm)
	update_lights(fr, bk, grip, tm)
	update_atmos(dt, tm)
	update_particles(dt)

	# 放回：链子缩进链头以后，牌在那里重新出现（C# 把牌的透明度还回去）
	if phase == "cancel" and clock - t_cancel > 0.16 and not card_show:
		card_show = true
		for i in 10:
			var an := rng.randf_range(0.0, TAU)
			addp(clock, rng.randf_range(0.18, 0.3), head, Vector2(cos(an), sin(an)) * rng.randf_range(120, 300) * S, 6.0, Vector2.ZERO, 0.08 * S, 0.2 * S, Color(0.45, 0.8, 0.7, 0.25), 0.0, 0.02)
	var done := false
	if phase == "tear" and tm > flight + SETTLE + FADE + 0.4 and P.size() == 0 and atm < 0.01: done = true
	if phase == "cancel" and clock - t_cancel > 0.45: done = true
	if t_dismiss >= 0.0 and clock - t_dismiss > 0.4: done = true
	if done: phase = "gone"
	# 魂被拉走的他：暗下去一截（C# 乘在他身上），魂炸开以后慢慢回来
	var dim := 0.0
	if phase == "tear": dim = s01(0.0, 0.08, clock - t_tear) * (1.0 - s01(flight + SETTLE, flight + SETTLE + FADE + 0.2, tm))
	var heard := cues.duplicate()
	cues.clear()
	return {"done": done, "yank": yank, "shake": out_shake, "hold": out_hold, "card_show": card_show, "dim": dim, "cues": heard}

# ------------------------------------------------------------------ 她

func pose_now(tm: float) -> String:
	if phase == "rel":
		var r := clock - t_rel
		if r < 2.0 * F1: return "k2"
		if r < 3.0 * F1: return "k25"
		return "k3"
	if phase == "tear":
		if tm < 2.0 * F1: return "k3"
		if tm < flight + 0.05: return "k4"
		return "k5"
	return "k2"

# 她有多实（0..1）：拿起时淡淡地出来，放回、撤掉时淡出
func her_presence() -> float:
	var v := 0.0
	if t_lift > -50.0: v = 0.30 * s01(t_lift, t_lift + 0.25, clock)
	if phase == "cancel": v *= 1.0 - s01(t_cancel, t_cancel + 0.25, clock)
	if t_dismiss >= 0.0: v *= 1.0 - s01(t_dismiss, t_dismiss + 0.2, clock)
	return v

func hand_pos(p: String, which: String) -> Vector2:
	return H(POSE[p][which])

func her_light(fr: Vector2, bk: Vector2, tm: float) -> void:
	var charge := s01(t_aim, t_aim + 0.6, clock) if phase == "aim" else 0.0
	var lat: Vector2 = bk
	var lamt := 0.6 + 1.2 * charge
	var lr := 170.0
	if phase == "rel" or phase == "tear":
		lat = fr.lerp(bk, 0.5); lamt = 2.2; lr = 220.0
		if t_snap >= 0.0:
			lat = fr; lamt = 1.2 + 2.6 * (1.0 - s01(t_snap, t_snap + 0.18, clock)); lr = 200.0
		if phase == "tear":
			if tm > 0.0 and tm <= flight: lat = soul_grip(tm); lamt = maxf(lamt, 2.6); lr = 240.0
			if arrived: lat = soul_grip(flight); lamt = 1.5 + 3.5 * (1.0 - s01(t_tear + HS2 + flight, t_tear + HS2 + flight + FREEZE + 0.3, clock)); lr = 240.0
			lamt *= 1.0 - s01(flight + 0.2, flight + FADE, tm)
	hmat.set_shader_parameter("light_at", lat); hmat.set_shader_parameter("light_amt", lamt); hmat.set_shader_parameter("light_r", lr * S)

func ghost_trails(p: String, tm: float) -> void:
	var gl: Array = []
	if phase == "rel":
		var r := clock - t_rel
		if p == "k25": gl.append(["k2", POSE["k2"].pos + SKEW, 0.45])
		elif p == "k3" and r < 4.0 * F1: gl.append(["k25", POSE["k25"].pos + Vector2(-4, 6), 0.42]); gl.append(["k2", POSE["k2"].pos + SKEW, 0.18])
		elif clock - t_snap < 0.07: gl.append(["k25", POSE["k25"].pos + Vector2(-4, 6), 0.22 * (1.0 - (clock - t_snap) / 0.07)])
	if phase == "tear":
		var d4 := tm - 2.0 * F1
		if d4 >= 0.0 and d4 < 0.09: gl.append(["k3", POSE["k3"].pos.lerp(POSE["k4"].pos, 0.35 * d4 / 0.09), 0.55 * (1.0 - d4 / 0.09)])
		var d5 := tm - (flight + 0.05)
		if d5 >= 0.0 and d5 < 0.09: gl.append(["k4", POSE["k4"].pos.lerp(POSE["k5"].pos, 0.35 * d5 / 0.09), 0.35 * (1.0 - d5 / 0.09)])
	for i in ghosts.size():
		var g: Sprite2D = ghosts[i]
		g.visible = i < gl.size()
		if not g.visible: continue
		g.texture = her[gl[i][0]].texture; g.position = H(gl[i][1]); g.scale = Vector2.ONE * S * TEX_K
		g.modulate = Color(0.35, 0.75, 0.72, float(gl[i][2]) * presence_ext)

func hand_smears() -> void:
	var r := clock - t_rel
	for k in 2:
		var sl: MeshInstance2D = slines[k]
		sl.visible = false
		if (phase != "rel" and phase != "tear") or r < 2.0 * F1 or r >= 4.0 * F1 + 0.08: continue
		var key := "front" if k == 0 else "back"
		var a0: Vector2 = H(POSE["k2"][key] + SKEW); var a1: Vector2 = H(POSE["k25"][key] + Vector2(-4, 6)); var a2: Vector2 = H(POSE["k3"][key])
		var cc := a1 * 2.0 - (a0 + a2) * 0.5
		var u0 := 0.0; var u1 := 0.5; var am := 0.75
		if r >= 3.0 * F1: u0 = 0.08; u1 = 1.0; am = 0.85
		if r >= 4.0 * F1:
			var w := (r - 4.0 * F1) / 0.08
			u0 = lerpf(0.4, 0.95, w); u1 = 1.0; am = 0.6 * (1.0 - w)
		var pts: Array = []
		for j in 16:
			var u := lerpf(u0, u1, float(j) / 15.0)
			pts.append(a0.lerp(cc, u).lerp(cc.lerp(a2, u), u))
		sl.visible = true
		var ln := strip(sl, pts, (20.0 if k == 0 else 16.0) * S)
		var m: ShaderMaterial = sl.material
		m.set_shader_parameter("total", ln); m.set_shader_parameter("t", clock); m.set_shader_parameter("ws", S)
		m.set_shader_parameter("alpha_mul", 0.55 * am * (1.0 if k == 0 else 0.7) * presence_ext)

# ------------------------------------------------------------------ 魂

func hold_point() -> Vector2: return hand_pos("k4", "front") + Vector2(-4, 16) * S

func soul_grip(tm: float) -> Vector2:
	if phase == "rel" and t_snap >= 0.0:
		# 凝回以后、真伤害到之前：魂已经被往外带了一点
		var u := s01(0.0, 0.35, clock - t_snap)
		return G + (hold_point() - G).normalized() * 18.0 * S * u
	if phase == "aim": return G + Vector2(0, -12.0) * S * s01(t_aim + 0.06, t_aim + 0.3, clock)
	if phase != "tear": return G
	var start := G + (hold_point() - G).normalized() * 18.0 * S * s01(0.0, 0.35, t_tear - t_snap)
	var hold := hold_point()
	if tm <= 0.0: return start
	var u2 := eout(tm / flight)
	var c := start.lerp(hold, 0.5) + Vector2(-70, -60) * S
	var p := start.lerp(c, u2).lerp(c.lerp(hold, u2), u2)
	if tm > flight:
		var pp := pose_now(tm)
		p = hold + (hand_pos("k5", "front") - hand_pos("k4", "front")) * 0.8 * s01(flight + 0.01, flight + SETTLE, tm)
	return p

func update_soul(grip: Vector2, tm: float) -> void:
	var detach := 0.0
	var sop := 0.0
	var glint := 0.0
	if phase == "aim" or (phase == "rel" and t_snap < 0.0):
		var since := clock - t_aim
		sop = (0.42 + 0.55 * exp(-maxf(since - 0.08, 0.0) * 5.0) + 0.08 * sin(clock * 4.5)) * s01(0.06, 0.1, since)
		glint = 0.9 * exp(-maxf(since - 0.08, 0.0) * 6.0) * s01(0.06, 0.1, since)
	elif phase == "rel":
		sop = 0.5 + 0.2 * s01(0.0, 0.3, clock - t_snap)
	elif phase == "tear":
		detach = s01(0.0, 0.06, tm)
		sop = 1.0
		if t_sburst >= 0.0: sop = 1.0 - s01(0.25, 0.4, clock - t_sburst)
	elif phase == "cancel" or phase == "free":
		sop = 0.0
	if t_dismiss >= 0.0: sop *= 1.0 - s01(t_dismiss, t_dismiss + 0.2, clock)
	soul.visible = has_target and sop > 0.002
	if not soul.visible:
		tether.visible = false
		return
	var vel := (soul_grip(tm + 0.01) - soul_grip(tm - 0.01)) / 0.02 if phase == "tear" else Vector2.ZERO
	var sdir := vel.normalized() if vel.length() > 1.0 else Vector2(0, -1)
	var st := clampf(vel.length() / (2600.0 * S), 0.0, 0.35) * detach
	if phase == "tear" and tm >= flight and tm < flight + 0.1:
		st = -0.18 * (1.0 - (tm - flight) / 0.1); sdir = Vector2(0.3, -1).normalized()
	var growv := lerpf(1.0, 1.15, detach)
	var sb := s01(0.0, 0.35, clock - t_sburst) if t_sburst >= 0.0 else 0.0
	smat.set_shader_parameter("pull", grip - G); smat.set_shader_parameter("head_at", grip)
	smat.set_shader_parameter("full", detach); smat.set_shader_parameter("full_w", detach); smat.set_shader_parameter("grow", growv)
	smat.set_shader_parameter("lift_fade", detach * 0.6); smat.set_shader_parameter("blur", lerpf(6.0, 2.5, detach)); smat.set_shader_parameter("rot", 0.0)
	smat.set_shader_parameter("vivid", detach); smat.set_shader_parameter("burst", sb)
	smat.set_shader_parameter("sdir", sdir); smat.set_shader_parameter("stretch", st); smat.set_shader_parameter("t", clock * 1.4)
	smat.set_shader_parameter("opacity", sop * presence_ext); smat.set_shader_parameter("glint", glint)
	# 魂只画在他和魂之间的那一块
	var m := maxf(esize.x, esize.y) * (1.1 + 1.6 * (s01(0.0, 0.35, clock - t_sburst) if t_sburst >= 0.0 else 0.0)) + 160.0 * S
	var a := Vector2(minf(epos.x, grip.x - m), minf(epos.y, grip.y - m))
	var b := Vector2(maxf(epos.x + esize.x, grip.x + m), maxf(epos.y + esize.y, grip.y + m))
	a = a.max(lo); b = b.min(hi)
	place_quad(soul, a, b)
	# 魂下端和身体之间那一缕
	var body := epos + Vector2(esize.x * 0.5, esize.y * 0.45)
	var tamt := 0.4 * detach * (1.0 - s01(flight, flight + 0.4, tm))
	tether.visible = tamt > 0.003
	if tether.visible:
		var top_p := grip + Vector2(-20, 90) * S * growv
		tmat.set_shader_parameter("top", top_p); tmat.set_shader_parameter("bot", body)
		tmat.set_shader_parameter("amt", tamt * presence_ext); tmat.set_shader_parameter("t", clock * 2.0)
		var ta := Vector2(minf(top_p.x, body.x), minf(top_p.y, body.y)) - Vector2(90, 90) * S
		var tb := Vector2(maxf(top_p.x, body.x), maxf(top_p.y, body.y)) + Vector2(90, 90) * S
		place_quad(tether, ta.max(lo), tb.min(hi))

# ------------------------------------------------------------------ 链子

func update_chain(fr: Vector2, bk: Vector2, grip: Vector2, tm: float, dt: float) -> void:
	var pts: Array = []
	var tension := 0.3
	var ralpha := 0.5
	var tailf := 14.0 * S
	var headf := 26.0 * S
	var pulse_pos := -999.0
	var pulse_amp := 0.0
	var glow := 0.0
	var show_from := 0.0           # 只画链子的一段：拿起时从链头往她那边长出来，放回时往链头缩
	var show_to := 1.0
	var end := head
	var sag_a := 18.0 * S
	var bend := 0.0
	var wave_u := -1.0
	var mouth := grip + Vector2(-0.08 * esize.x, -0.03 * esize.y) * (1.4 if phase == "tear" else 1.0)
	match phase:
		"hover":
			ralpha = 0.0
		"free":
			# 链头跟着鼠标，慢半拍；链身松垂、跟着甩
			var k := 16.0
			var nh := Vector2(damp(head.x, mouse.x, k, dt), damp(head.y, mouse.y, k, dt))
			head_v = (nh - head) / maxf(dt, 0.001)
			head = nh
			end = head
			tension = 0.25; ralpha = 0.55
			show_from = 1.0 - eout((clock - t_lift) / 0.14)
		"aim":
			var u := eout((clock - t_aim) / 0.09)
			head = head_from.lerp(G, u)
			end = head
			tension = 0.55 + 0.25 * u; ralpha = 0.7
			bend = 10.0 * S * (1.0 - u) + 4.0 * S * sin(clock * 3.1)
		"cancel":
			end = head
			tension = 0.3; ralpha = 0.6
			show_from = eout((clock - t_cancel) / 0.2)
		"rel":
			var r := clock - t_rel
			end = G if t_snap < 0.0 else mouth_of(grip)
			if r < 2.0 * F1:
				sag_a = 22.0 * S; bend = 26.0 * S; tension = 0.2; ralpha = 0.6
			elif r < 4.0 * F1:
				var u3 := s01(2.0 * F1 - 0.01, 4.0 * F1, r)
				sag_a = lerpf(16.0, 6.0, u3) * S; bend = lerpf(12.0, 2.0, u3) * S; tension = lerpf(0.5, 0.9, u3); ralpha = 0.9
				wave_u = (r - 2.0 * F1 + F1) / (2.0 * F1 + F1)
			else:
				sag_a = 4.0 * S; tension = 1.0; ralpha = 1.0
				glow = 1.2 * (1.0 - s01(t_snap, t_snap + 0.2, clock)) + 0.15 * (0.5 + 0.5 * sin(clock * 22.0))
		"tear":
			end = mouth_of(grip)
			sag_a = 4.0 * S * (1.0 - s01(0.0, 0.05, tm)); tension = 1.0
			bend = 12.0 * S * s01(0.0, 0.04, tm) * (1.0 - s01(0.06, flight, tm))
			ralpha = 1.0 - s01(flight + 0.08, flight + SETTLE + FADE * 0.75, tm)
			glow = 1.2 * (1.0 - s01(t_tear, t_tear + 0.2, clock))
			if tm > 0.0 and tm < flight: glow = maxf(glow, 0.3)
			if arrived: glow = maxf(glow, 0.8 * (1.0 - s01(0.0, 0.25, clock - (t_tear + HS2 + flight))))
	if t_dismiss >= 0.0: ralpha *= 1.0 - s01(t_dismiss, t_dismiss + 0.2, clock)
	ralpha *= presence_ext
	if ralpha <= 0.003 or (phase != "rel" and phase != "tear" and t_lift < -50.0):
		rope_bk.visible = false; rope_fr.visible = false; head_glow.visible = false; rope_tail.visible = false
		for f in fingers: f.visible = false
		last_pts = []
		return
	# 链子从她伸向敌人的那只手出发，一条顺的弧线到链头（两手之间不再有一段，免得在手上拐一下）
	pts.append(fr)
	var len_a := 0.0
	var dvec := end - fr
	var dist := maxf(dvec.length(), 1.0)
	var nb := dvec.orthogonal() / dist
	var ctrl := fr.lerp(end, 0.5)
	if phase == "free" or phase == "cancel":
		var sag := clampf(0.18 * dist, 20.0 * S, 150.0 * S)
		var swing := (-head_v * 0.05).limit_length(120.0 * S)
		ctrl += Vector2(0, sag) + swing
	for j in range(1, 21):
		var v2 := float(j) / 20.0
		var q := fr.lerp(ctrl, v2).lerp(ctrl.lerp(end, v2), v2)
		q += nb * bend * sin(v2 * PI)
		if wave_u >= 0.0: q += nb * 12.0 * S * exp(-pow((v2 - wave_u) / 0.12, 2.0)) * (1.0 - 0.4 * wave_u)
		pts.append(q)
	last_pts = pts
	# 只画一段
	if show_from > 0.001 or show_to < 0.999:
		var n := pts.size()
		var i0 := int(floor(show_from * float(n - 1)))
		var sub: Array = []
		for j in range(i0, n): sub.append(pts[j])
		pts = sub
		headf = 10.0 * S
	var front_layer := phase == "tear"
	rope_fr.visible = front_layer and pts.size() >= 2; rope_bk.visible = (not front_layer) and pts.size() >= 2
	if pts.size() < 2:
		rope_tail.visible = false
		return
	var rm: MeshInstance2D = rope_fr if front_layer else rope_bk
	var total := strip(rm, pts, 24.0 * S)
	update_tail(fr, pts, ralpha * (1.0 if show_from < 0.05 else 0.0), tension, glow)
	# 光：瞄着的时候一下一下往她手里涌；冲起时一道劲打下去；撕魂时往她那边流
	if phase == "aim":
		pulse_t += dt
		if pulse_t >= 0.7: pulse_t -= 0.7
		var pu := pulse_t / 0.5
		if pu < 1.0: pulse_pos = total * (1.0 - pu); pulse_amp = 0.6 * (1.0 - pu * 0.5)
	elif wave_u >= 0.0:
		pulse_pos = len_a + (total - len_a) * wave_u; pulse_amp = 0.8
	elif phase == "rel" and t_snap >= 0.0:
		var pu2 := fmod(clock - t_snap, 0.35) / 0.3
		if pu2 < 1.0: pulse_pos = total * (1.0 - pu2); pulse_amp = 0.7
	elif phase == "tear" and tm > 0.0 and tm < flight + 0.05:
		pulse_pos = total * (1.0 - tm / (flight + 0.05)); pulse_amp = 1.0
	rmat.set_shader_parameter("t", clock); rmat.set_shader_parameter("tension", tension); rmat.set_shader_parameter("total", total)
	rmat.set_shader_parameter("flow", (grip - G).length()); rmat.set_shader_parameter("pulse_pos", pulse_pos); rmat.set_shader_parameter("pulse_amp", pulse_amp)
	rmat.set_shader_parameter("alpha_mul", ralpha); rmat.set_shader_parameter("tail_fade", tailf); rmat.set_shader_parameter("head_fade", headf)
	rmat.set_shader_parameter("glow", glow)
	# 链头：三根雾指头。没咬住时张开（像爪子），咬住时扣紧
	var tip: Vector2 = pts[pts.size() - 1]
	var prev: Vector2 = pts[max(pts.size() - 4, 0)]
	var dir_e := (tip - prev).normalized() if (tip - prev).length() > 0.5 else Vector2(-1, 0)
	var spread := 0.62
	if phase == "free" or phase == "cancel": spread = 0.95
	var falpha := 0.9 * ralpha
	if phase == "cancel": falpha *= 1.0 - s01(t_cancel + 0.12, t_cancel + 0.22, clock)
	for k in 3:
		var fmi: MeshInstance2D = fingers[k]
		fmi.visible = falpha > 0.02
		if not fmi.visible: continue
		var sp := float(k - 1) * spread
		var c0f := tip - dir_e * 4.0 * S
		var c1 := c0f + dir_e.rotated(sp) * 16.0 * S
		var c2 := c1 + dir_e.rotated(-sp * 0.9) * 13.0 * S
		var fp: Array = []
		for j in 8:
			var u4 := float(j) / 7.0
			fp.append(c0f.lerp(c1, u4).lerp(c1.lerp(c2, u4), u4))
		var fl := strip(fmi, fp, 10.0 * S)
		var fm: ShaderMaterial = fmi.material
		fm.set_shader_parameter("t", clock + float(k)); fm.set_shader_parameter("total", fl + 30.0 * S); fm.set_shader_parameter("alpha_mul", falpha)
	head_glow.visible = phase == "free" or phase == "aim" or phase == "cancel"
	if head_glow.visible:
		var hb := 0.35 + (0.3 * exp(-maxf(clock - t_aim - 0.08, 0.0) * 6.0) if phase == "aim" else 0.0)
		head_glow.position = tip; head_glow.scale = Vector2.ONE * (0.55 + (0.35 if phase == "aim" else 0.0)) * S
		head_glow.modulate = Color(0.75, 1.0, 0.95, hb * falpha)
	# 光点顺着链子流进她手里（瞄着的时候）
	if phase == "aim":
		rider_acc += dt * 26.0
		while rider_acc >= 1.0:
			rider_acc -= 1.0
			riders.append({"u": rng.randf_range(0.9, 1.0), "sp": rng.randf_range(0.9, 1.4), "lat": rng.randf_range(-10.0, 10.0) * S, "sz": rng.randf_range(0.016, 0.026) * S, "age": 0.0})
	else:
		rider_acc = 0.0

# 手后面松垂的一小截链尾（攥在手里的那一头），顺着链子出手的反方向垂下去，轻轻晃
func update_tail(fr: Vector2, pts: Array, alpha: float, tension: float, glow: float) -> void:
	rope_tail.visible = alpha > 0.01 and pts.size() >= 2
	if not rope_tail.visible: return
	var d0: Vector2 = ((pts[mini(2, pts.size() - 1)] as Vector2) - fr)
	var dir := -d0.normalized() if d0.length() > 0.5 else Vector2(1, 0)
	var tp: Array = []
	for j in 11:
		var u := float(j) / 10.0
		var q := fr + dir * 74.0 * S * u + Vector2(0, 30.0 * S * u * u)
		q += Vector2(sin(clock * 2.1 + u * 2.0), cos(clock * 1.7 + u * 1.5) * 0.5) * 5.0 * S * u
		tp.append(q)
	var tl := strip(rope_tail, tp, 20.0 * S)
	tail_mat.set_shader_parameter("t", clock); tail_mat.set_shader_parameter("total", tl)
	tail_mat.set_shader_parameter("tension", tension * 0.5); tail_mat.set_shader_parameter("tail_fade", tl * 0.75)
	tail_mat.set_shader_parameter("head_fade", 4.0 * S); tail_mat.set_shader_parameter("alpha_mul", alpha * 0.85)
	tail_mat.set_shader_parameter("glow", glow * 0.5); tail_mat.set_shader_parameter("pulse_amp", 0.0)

func mouth_of(grip: Vector2) -> Vector2:
	return grip + Vector2(-0.08 * esize.x, -0.03 * esize.y)

# ------------------------------------------------------------------ 敌人脚下的雾、各种光

func update_lurk(tm: float) -> void:
	var amt := 0.0
	var burst := 0.0
	var beat := 0.0
	if has_target:
		if phase == "aim": amt = 0.75 * s01(t_aim, t_aim + 0.2, clock)
		elif phase == "rel": amt = 0.75; beat = 0.7
		elif phase == "tear":
			if tm < 0.45:
				amt = 0.75 * (1.0 - s01(0.12, 0.45, tm)); burst = s01(0.0, 0.30, tm)
			else:
				# 留在他身上的一缕残雾，慢慢沉下去、淡掉
				amt = 0.38 * s01(0.45, 0.7, tm) * (1.0 - s01(flight + 0.1, flight + SETTLE + FADE + 0.2, tm))
	if phase == "aim":
		var pu := pulse_t / 0.5
		beat = maxf(beat, 0.6 * exp(-pu * 3.0))
	if t_dismiss >= 0.0: amt *= 1.0 - s01(t_dismiss, t_dismiss + 0.2, clock)
	lurk.visible = amt > 0.003
	if not lurk.visible: return
	var r := Vector2(maxf(esize.x * 0.7, 120.0 * S), 52.0 * S)
	var sink := Vector2(0, 18.0 * S * s01(flight, flight + SETTLE + FADE + 0.2, tm)) if phase == "tear" else Vector2.ZERO
	lmat.set_shader_parameter("center", tfeet + sink); lmat.set_shader_parameter("radius", r)
	lmat.set_shader_parameter("t", clock); lmat.set_shader_parameter("amt", amt * presence_ext)
	lmat.set_shader_parameter("burst", burst); lmat.set_shader_parameter("beat", beat)
	var ext := Vector2(r.x * 1.9, r.y * 1.6 + esize.y * 1.5)
	place_quad(lurk, (tfeet - Vector2(ext.x, ext.y)).max(lo), (tfeet + Vector2(ext.x, r.y * 1.6)).min(hi))

func update_lights(fr: Vector2, bk: Vector2, grip: Vector2, tm: float) -> void:
	var pe := presence_ext
	var charge := s01(t_aim, t_aim + 0.6, clock) if phase == "aim" else (0.3 if phase == "free" else 0.0)
	var r := clock - t_rel
	hg_f.visible = false; hg_b.visible = false
	if phase == "free" or phase == "aim":
		var pul := 0.0
		if phase == "aim":
			var pu := pulse_t / 0.5
			pul = 0.15 * exp(-maxf(1.0 - pu, 0.0) * 4.0) if pu < 1.0 else 0.0
		var hp := her_presence() / 0.30
		hg_b.visible = true; hg_b.position = bk; hg_b.scale = Vector2.ONE * (0.32 + 0.40 * charge) * S
		hg_b.modulate = Color(0.75, 1.0, 0.96, (0.12 + 0.30 * charge + pul) * hp * pe)
		hg_f.visible = true; hg_f.position = fr; hg_f.scale = Vector2.ONE * (0.28 + 0.32 * charge) * S
		hg_f.modulate = Color(0.75, 1.0, 0.96, (0.10 + 0.22 * charge + pul) * hp * pe)
	elif phase == "rel" and r < 2.0 * F1:
		hg_b.visible = true; hg_b.position = bk; hg_b.scale = Vector2.ONE * 0.36 * S; hg_b.modulate = Color(0.75, 1.0, 0.96, 0.85 * pe)
		hg_f.visible = true; hg_f.position = fr; hg_f.scale = Vector2.ONE * 0.32 * S; hg_f.modulate = Color(0.75, 1.0, 0.96, 0.75 * pe)
	elif phase == "rel" and r < 4.0 * F1:
		hg_b.visible = true; hg_b.position = bk + Vector2(0, 40) * S; hg_b.scale = Vector2(0.32, 1.5) * S; hg_b.modulate = Color(0.75, 1.0, 0.96, 0.55 * pe)
		hg_f.visible = true; hg_f.position = fr + Vector2(0, 50) * S; hg_f.scale = Vector2(0.36, 1.8) * S; hg_f.modulate = Color(0.75, 1.0, 0.96, 0.65 * pe)
	# 冲起时的竖光柱
	column.visible = phase == "rel" and r >= 2.0 * F1 and r < 4.0 * F1
	if column.visible:
		var k25 := r < 3.0 * F1
		column.position = H(Vector2(1150, 390) if k25 else Vector2(1160, 300))
		column.scale = (Vector2(1.8, 6.4) if k25 else Vector2(1.5, 7.2)) * S
		column.modulate = Color(0.75, 1.0, 0.96, (0.22 if k25 else 0.15) * pe)
	# 地上的光：瞄着时被扯一下亮一下，撕魂时大亮
	var ga := 0.0
	if has_target:
		if phase == "aim":
			var pu2 := pulse_t / 0.5
			ga = (0.10 + 0.14 * exp(-pu2 * 3.0)) * s01(t_aim, t_aim + 0.2, clock)
		elif phase == "rel": ga = 0.18
		elif phase == "tear": ga = 0.08 + 0.55 * (1.0 - s01(0.0, 0.35, clock - t_tear))
		ga *= 1.0 - s01(flight + 0.1, flight + 0.6, tm) if phase == "tear" else 1.0
	gnd.visible = ga > 0.003
	if gnd.visible:
		gnd.position = tfeet + Vector2(0, 6) * S
		var boom := 1.25 if phase == "tear" and clock - t_tear < 0.2 else 1.0
		gnd.scale = Vector2(4.6 * esize.x / 225.0, 0.9 * S) * boom
		gnd.modulate = Color(1, 1, 1, ga * pe)
	# 魂身上的光
	var sa := 0.0
	if phase == "tear": sa = 0.32 * s01(0.0, 0.04, tm) * (1.0 - s01(flight + 0.05, flight + 0.5, tm))
	sglow.visible = sa > 0.005
	if sglow.visible:
		sglow.position = grip + Vector2(-10, 30) * S; sglow.scale = Vector2.ONE * 2.2 * S; sglow.modulate = Color(1, 1, 1, sa * pe)
	# 闪：凝回时手上；撕魂时咬住处；魂到手时手上大闪
	flash.visible = false; flash2.visible = false
	if t_snap >= 0.0 and clock - t_snap < 0.15:
		var u5 := (clock - t_snap) / 0.15
		flash.visible = true; flash.position = fr; flash.scale = Vector2.ONE * lerpf(0.7, 2.0, u5) * S; flash.modulate = Color(1, 1, 1, 0.8 * (1.0 - u5) * pe * flash_k)
	if phase == "tear" and clock - t_tear < 0.18:
		var u7 := (clock - t_tear) / 0.18
		flash2.visible = true; flash2.position = mouth_of(G); flash2.scale = Vector2.ONE * lerpf(1.2, 5.0, u7) * S; flash2.modulate = Color(1, 1, 1, 0.95 * (1.0 - u7) * pe * flash_k)
	var ta := t_tear + HS2 + flight
	if arrived and clock - ta < FREEZE + 0.15:
		var u6 := (clock - ta) / (FREEZE + 0.15)
		flash.visible = true; flash.position = grip + Vector2(0, 30) * S; flash.scale = Vector2.ONE * lerpf(1.6, 4.4, u6) * S; flash.modulate = Color(1, 1, 1, 0.85 * (1.0 - u6) * pe * flash_k)
	# 咬住的那一下：链头一闪、他身上一圈小环
	var sa2 := clock - t_aim - 0.08
	if phase == "aim" and sa2 >= 0.0 and sa2 < 0.2:
		var u9 := sa2 / 0.2
		flash2.visible = true; flash2.position = G; flash2.scale = Vector2.ONE * lerpf(0.6, 2.4, u9) * S; flash2.modulate = Color(1, 1, 1, 0.7 * (1.0 - u9) * pe * flash_k)
	if phase == "aim" and sa2 >= 0.0 and sa2 < 0.28:
		var u10 := sa2 / 0.28
		ring1.visible = true; ring1.position = G; ring1.scale = Vector2(1.0, 0.8) * lerpf(0.3, 1.6, eout(u10)) * S * maxf(esize.x / 225.0, 0.8)
		ring1.modulate = Color(0.85, 1.0, 0.97, 0.6 * (1.0 - u10) * pe)
		return
	# 光环
	ring1.visible = phase == "tear" and clock - t_tear < 0.3
	if ring1.visible:
		var u := (clock - t_tear) / 0.3
		ring1.position = mouth_of(G); ring1.scale = Vector2(1.0, 0.75) * lerpf(0.4, 4.4, eout(u)) * S * maxf(esize.x / 225.0, 0.8)
		ring1.modulate = Color(0.85, 1.0, 0.97, 0.75 * (1.0 - u) * pe)
	ring3.visible = t_sburst >= 0.0 and clock - t_sburst < 0.4
	if ring3.visible:
		var u11 := (clock - t_sburst) / 0.4
		ring3.position = grip; ring3.scale = Vector2.ONE * lerpf(0.6, 4.2, eout(u11)) * S * maxf(esize.x / 225.0, 0.8)
		ring3.modulate = Color(0.85, 1.0, 0.97, 0.7 * (1.0 - u11) * pe)
	if t_sburst >= 0.0 and clock - t_sburst < 0.18:
		var u12 := (clock - t_sburst) / 0.18
		flash2.visible = true; flash2.position = grip; flash2.scale = Vector2.ONE * lerpf(1.2, 3.2, u12) * S * maxf(esize.x / 225.0, 0.8); flash2.modulate = Color(1, 1, 1, 0.7 * (1.0 - u12) * pe * flash_k)
	ring2.visible = arrived and clock - ta < 0.36
	if ring2.visible:
		var u8 := (clock - ta) / 0.36
		ring2.position = hold_point() + Vector2(0, 20) * S; ring2.scale = Vector2.ONE * lerpf(0.4, 3.0, eout(u8)) * S
		ring2.modulate = Color(0.85, 1.0, 0.97, 0.65 * (1.0 - u8) * pe)

func update_atmos(dt: float, tm: float) -> void:
	var want_atm := 0.0
	var want_veil := 0.0
	var want_fog := 0.0
	var want_grade := 0.0
	match phase:
		"hover":
			want_atm = 0.26; want_grade = 0.05
		"free":
			want_atm = 0.58; want_veil = 0.16; want_fog = 0.40; want_grade = 0.16
		"aim":
			want_atm = 0.72; want_veil = 0.24; want_fog = 0.52; want_grade = 0.22
		"rel":
			want_atm = 0.80; want_veil = 0.32; want_fog = 0.55; want_grade = 0.28
		"tear":
			# 魂到手以后雾还在场上多留一会儿，跟着慢放的钟慢慢退
			var fade := 1.0 - s01(flight + 0.1, flight + SETTLE + FADE + 0.25, tm)
			want_atm = 0.86 * fade; want_veil = 0.34 * fade; want_fog = 0.55 * fade; want_grade = 0.30 * fade
	if phase == "cancel" or t_dismiss >= 0.0:
		want_atm = 0.0; want_veil = 0.0; want_fog = 0.0; want_grade = 0.0
	var k_in := 3.5 if phase == "free" else 5.0
	atm = damp(atm, want_atm, k_in, dt)
	veil_a = damp(veil_a, want_veil, 6.0, dt)
	fog_a = damp(fog_a, want_fog, 5.0, dt)
	var pe := presence_ext
	veil.modulate = Color(0, 0.02, 0.03, veil_a * pe * effect_k)
	veil.visible = veil_a * pe > 0.003
	fogb.material.set_shader_parameter("amt", fog_a * pe); fogb.material.set_shader_parameter("t", clock)
	fogb.visible = fog_a * pe > 0.003
	# 冲击：放射速度线（敌人身前那一层）
	var tt := clock - t_tear
	burst_q.visible = phase == "tear" and tt < 0.2
	if burst_q.visible:
		var c := mouth_of(G)
		var rr := 1100.0 * S
		burst_mat.set_shader_parameter("center", c); burst_mat.set_shader_parameter("r0", 50.0 * S); burst_mat.set_shader_parameter("r1", rr)
		burst_mat.set_shader_parameter("t", tt); burst_mat.set_shader_parameter("amt", (1.0 - s01(0.06, 0.2, tt)) * pe * effect_k)
		place_quad(burst_q, (c - Vector2(rr, rr)).max(lo), (c + Vector2(rr, rr)).min(hi))
	if top == null or not is_instance_valid(top): return
	# 雾往她那边吸：拿起时从四边涌进来，瞄着时整片往她和链子那边收
	var flow_v := 0.0
	if phase == "free": flow_v = 40.0
	elif phase == "aim": flow_v = 70.0
	elif phase == "rel": flow_v = 110.0
	flow_acc += flow_v * S * dt
	var fl := 0.0
	var wa := 0.0
	var dk := 0.0
	if t_snap >= 0.0 and clock - t_snap < 0.07: wa = maxf(wa, 0.14 * (1.0 - (clock - t_snap) / 0.07))
	var shock_r := -1.0
	var shock_amt := 0.0
	if phase == "tear":
		fl = 1.0 - s01(0.0, 0.45, tt)
		# 冲击帧：第一帧强闪，第二帧压暗
		if tt < F1 * 1.01: wa = maxf(wa, 0.55)
		elif tt < F1 * 2.01: dk = 0.42
		elif tt < 0.2: wa = maxf(wa, 0.22 * (1.0 - (tt - 2.0 * F1) / 0.13))
		var ta := t_tear + HS2 + flight
		if arrived and clock - ta < 0.18: wa = maxf(wa, 0.26 * (1.0 - (clock - ta) / 0.18))
		if tt < 0.9:
			shock_r = lerpf(40.0, 1700.0, eout(tt / 0.9)) * S
			shock_amt = 1.0 - s01(0.35, 0.9, tt)
	if amat != null and atmos != null and is_instance_valid(atmos):
		atmos.visible = (atm + fl) * pe > 0.003
		amat.set_shader_parameter("t", clock); amat.set_shader_parameter("amt", atm * pe * effect_k); amat.set_shader_parameter("flash", fl * pe * flash_k)
		amat.set_shader_parameter("flow_to", hand_pos("k2", "front").lerp(G if has_target else head, 0.35))
		amat.set_shader_parameter("flow", flow_acc)
		amat.set_shader_parameter("shock_at", mouth_of(G)); amat.set_shader_parameter("shock_r", shock_r); amat.set_shader_parameter("shock_amt", shock_amt * pe * effect_k)
	# 整个画面偏绿、偏暗
	grade_g = damp(grade_g, want_grade * pe * effect_k, 4.0, dt)
	if grade != null and is_instance_valid(grade):
		var g := clampf(grade_g, 0.0, 1.0)
		grade.modulate = Color(1.0 - 0.38 * g, 1.0 - 0.10 * g, 1.0 - 0.26 * g, 1.0)
		grade.visible = g > 0.003
	# 画面扭曲的波纹
	if ripple != null and is_instance_valid(ripple):
		ripple.visible = phase == "tear" and tt < 0.6
		if ripple.visible:
			var rr2 := lerpf(30.0, 1500.0, eout(tt / 0.6)) * S
			var c2 := mouth_of(G)
			ripple_mat.set_shader_parameter("center", c2); ripple_mat.set_shader_parameter("radius", rr2)
			ripple_mat.set_shader_parameter("width", lerpf(40.0, 120.0, tt / 0.6) * S); ripple_mat.set_shader_parameter("amp", 26.0 * S * (1.0 - s01(0.2, 0.6, tt)) * effect_k)
			var m := rr2 + 200.0 * S
			place_quad(ripple, (c2 - Vector2(m, m)).max(lo), (c2 + Vector2(m, m)).min(hi))
	if white != null and is_instance_valid(white):
		white.visible = wa * pe > 0.003
		white.modulate = Color(0.85, 1.0, 0.97, wa * pe * flash_k)
	if dark_top != null and is_instance_valid(dark_top):
		dark_top.visible = dk * pe > 0.003
		dark_top.modulate = Color(0.0, 0.03, 0.02, dk * pe)
	update_top_motes(dt, tm)

# 整个界面上往上飘的光点和细雾丝（顶层，盖着界面）
func update_top_motes(dt: float, tm: float) -> void:
	var rate := 0.0
	match phase:
		"hover": rate = 4.0
		"free": rate = 14.0
		"aim": rate = 20.0
		"rel": rate = 26.0
		"tear": rate = 34.0 * (1.0 - s01(flight + 0.1, flight + SETTLE + FADE, tm))
	if phase == "cancel" or t_dismiss >= 0.0: rate = 0.0
	tp_acc += rate * dt
	var span := hi - lo
	while tp_acc >= 1.0:
		tp_acc -= 1.0
		var big := rng.randf() < 0.25
		TP.append({"t0": pclock, "life": rng.randf_range(1.8, 3.4), "p0": lo + Vector2(rng.randf() * span.x, span.y * rng.randf_range(0.35, 1.05)),
			"v": Vector2(rng.randf_range(-18, 18), -rng.randf_range(40, 110)) * S, "sz": (rng.randf_range(0.10, 0.22) if big else rng.randf_range(0.02, 0.05)) * S,
			"a": (0.10 if big else 0.55), "ph": rng.randf() * TAU})
	# 撕魂那一下，从咬住点往外喷一圈（在界面上也看得见）
	if phase == "tear" and not shake2 and clock - t_tear < F1:
		for i in 24:
			var an := rng.randf_range(0.0, TAU)
			TP.append({"t0": pclock, "life": rng.randf_range(0.5, 0.9), "p0": mouth_of(G), "v": Vector2(cos(an), sin(an)) * rng.randf_range(700, 1500) * S,
				"sz": rng.randf_range(0.03, 0.06) * S, "a": 0.8, "ph": 0.0, "k": 3.0})
	var keep: Array = []
	var used := 0
	var pe := presence_ext
	for q in TP:
		var a: float = pclock - float(q["t0"])
		var life: float = q["life"]
		if a > life: continue
		keep.append(q)
		if used >= top_pool.size(): continue
		var k: float = float(q.get("k", 0.0))
		var v0: Vector2 = q["v"]
		var pos: Vector2 = q["p0"]
		if k > 0.0: pos += v0 * (1.0 - exp(-k * a)) / k
		else: pos += v0 * a + Vector2(sin(a * 1.3 + float(q["ph"])) * 14.0 * S, 0.0)
		var u := a / life
		var sp: Sprite2D = top_pool[used]; used += 1
		sp.visible = true; sp.position = pos; sp.rotation = 0.0
		sp.scale = Vector2.ONE * float(q["sz"]) * 2.0
		sp.modulate = Color(0.55, 1.0, 0.85, float(q["a"]) * s01(0.0, 0.15, u) * (1.0 - s01(0.6, 1.0, u)) * pe)
	TP = keep
	for i in range(used, top_pool.size()): top_pool[i].visible = false

# ------------------------------------------------------------------ 一次性的特效

func snap_fx() -> void:
	var fk := hand_pos("k3", "front")
	for i in 14:
		var an := rng.randf_range(0.0, TAU)
		addp(clock, rng.randf_range(0.18, 0.30), fk, Vector2(cos(an), sin(an)) * rng.randf_range(300, 760) * S, 8.0, Vector2.ZERO, 0.026 * S, 0.006 * S, Color(0.85, 1.0, 0.97, 0.8), 2.0, 0.01)

func tear_fx() -> void:
	var dist := (hold_point() - G).length()
	flight = clampf(0.13 + dist / (5000.0 * S), 0.13, 0.24)
	var bite := mouth_of(G)
	# 咬住处炸开一圈碎光
	for i in 70:
		var an := rng.randf_range(0.0, TAU)
		var d := Vector2(cos(an), sin(an) * 0.8 - 0.35).normalized()
		addp(clock + rng.randf_range(0.0, 0.02), rng.randf_range(0.25, 0.55), bite, d * rng.randf_range(600, 1900) * S, 6.0, Vector2(0, 350) * S, 0.05 * S, 0.01 * S, Color(0.88, 1.0, 0.97, 0.95), 3.0, 0.01)
	# 大块的碎光（少而亮）
	for i in 12:
		var an2 := rng.randf_range(0.0, TAU)
		addp(clock, rng.randf_range(0.18, 0.3), bite, Vector2(cos(an2), sin(an2)) * rng.randf_range(300, 700) * S, 5.0, Vector2.ZERO, 0.16 * S, 0.04 * S, Color(0.8, 1.0, 0.95, 0.7), 1.0, 0.01)
	# 他身后炸出一大团绿雾（往她的反方向喷）
	var away := (G - hold_point()).normalized()
	for i in 40:
		var d2 := away.rotated(rng.randf_range(-1.4, 1.4))
		addp(clock + rng.randf_range(0.0, 0.05), rng.randf_range(0.6, 1.1), G + Vector2(rng.randf_range(-0.3, 0.3) * esize.x, rng.randf_range(0.0, 0.5) * esize.y), d2 * rng.randf_range(300, 950) * S, 3.5, Vector2(0, -60) * S, 0.26 * S, 0.95 * S, Color(0.30, 0.64, 0.52, 0.34), 0.0, 0.04)
	# 一圈贴地扫开的雾
	for i in 18:
		var dx := -1.0 if i % 2 == 0 else 1.0
		addp(clock + rng.randf_range(0.0, 0.04), rng.randf_range(0.5, 0.9), tfeet + Vector2(rng.randf_range(-0.2, 0.2) * esize.x, -6.0 * S), Vector2(dx * rng.randf_range(500, 1100), -rng.randf_range(0, 80)) * S, 4.0, Vector2.ZERO, 0.22 * S, 0.7 * S, Color(0.32, 0.6, 0.5, 0.28), 0.0, 0.03)
	# 魂飞过的光尾（按魂的路线先排好）
	for i in 30:
		var ts := 0.01 + (flight - 0.01) * float(i) / 30.0
		var pp := soul_grip_at(ts) + Vector2(rng.randf_range(-34, 34), rng.randf_range(-20, 50)) * S
		addp(clock + HS2 + ts, rng.randf_range(0.30, 0.50), pp, Vector2(rng.randf_range(-60, 60), rng.randf_range(-30, 70)) * S, 3.0, Vector2.ZERO, 0.07 * S, 0.012 * S, Color(0.75, 1.0, 0.97, 0.45), 0.0, 0.05)

# 撕魂之后某一刻魂的位置（光尾预排用；和 soul_grip 同一条弧）
func soul_grip_at(tm: float) -> Vector2:
	var saved := phase
	phase = "tear"
	var p := soul_grip(tm)
	phase = saved
	return p

# 魂炸开：碎光往外崩、一团雾、一圈环（光点跟着慢放的钟，崩出去以后慢慢飘）
func soul_burst_fx() -> void:
	var c := soul_grip(flight + SETTLE) + Vector2(0, 0.3 * esize.y) * 1.15
	var big := maxf(esize.x / 225.0, 0.8)
	for i in 60:
		var an := rng.randf_range(0.0, TAU)
		addp(clock + rng.randf_range(0.0, 0.03), rng.randf_range(0.35, 0.7), c + Vector2(cos(an), sin(an)) * rng.randf_range(0.0, 0.3) * esize.x, Vector2(cos(an), sin(an)) * rng.randf_range(450, 1400) * S * big, 5.0, Vector2(0, -40) * S, 0.045 * S, 0.01 * S, Color(0.85, 1.0, 0.97, 0.95), 2.5, 0.01)
	for i in 16:
		var an2 := rng.randf_range(0.0, TAU)
		addp(clock + rng.randf_range(0.0, 0.04), rng.randf_range(0.5, 0.9), c, Vector2(cos(an2), sin(an2)) * rng.randf_range(200, 600) * S * big, 3.5, Vector2(0, -50) * S, 0.25 * S * big, 0.8 * S * big, Color(0.40, 0.75, 0.66, 0.30), 0.0, 0.03)

func arrive_fx() -> void:
	var hold := hold_point()
	for i in 40:
		var an := rng.randf_range(0.0, TAU)
		addp(clock + rng.randf_range(0.0, 0.03), rng.randf_range(0.30, 0.55), hold + Vector2(0, 20) * S, Vector2(cos(an), sin(an)) * rng.randf_range(380, 1150) * S, 6.0, Vector2(0, 120) * S, 0.036 * S, 0.008 * S, Color(0.85, 1.0, 0.97, 0.9), 2.0, 0.01)
	# 散：她和魂化成往上飘的光点（从她 k5 的轮廓里取点）
	tex("her_k5.png")
	var im: Image = CACHE.get("img/her_k5.png", null)
	if im == null: return
	var w := im.get_width(); var h := im.get_height()
	var t5 := clock + FREEZE + 0.06
	var n := 0; var tries := 0
	while n < 90 and tries < 6000:
		tries += 1
		var x := rng.randi_range(0, w - 1); var y := rng.randi_range(0, int(h * 0.8))
		if im.get_pixel(x, y).a < 0.6: continue
		n += 1
		var p0: Vector2 = H(POSE["k5"].pos) + Vector2(x, y) * S * TEX_K
		addp(t5 + rng.randf_range(0.0, FADE * 0.75), rng.randf_range(0.6, 1.0), p0, Vector2(rng.randf_range(-30, 50), -rng.randf_range(60, 170)) * S, 1.0, Vector2.ZERO, 0.03 * S, 0.01 * S, Color(0.55, 0.92, 0.86, 0.6), 0.3, 0.15)
	for i in 18:
		addp(t5 + rng.randf_range(0.0, FADE * 0.6), rng.randf_range(0.5, 0.9), hold + Vector2(rng.randf_range(-60, 60), rng.randf_range(-40, 60)) * S, Vector2(rng.randf_range(-40, 40), -rng.randf_range(50, 140)) * S, 1.0, Vector2.ZERO, 0.04 * S, 0.01 * S, Color(0.75, 1.0, 0.97, 0.6), 0.3, 0.15)

# ------------------------------------------------------------------ 光点

func addp(t0: float, life: float, p0: Vector2, v: Vector2, k: float, g: Vector2, s0: float, s1: float, c: Color, st: float, fin: float) -> void:
	t0 = pclock + (t0 - clock)        # 调用的人按普通的钟给延迟
	P.append({"t0": t0, "life": life, "p0": p0, "v": v, "k": k, "g": g, "s0": s0, "s1": s1, "c": c, "st": st, "fin": fin})

func update_particles(dt: float) -> void:
	var used := 0
	var keep: Array = []
	var pe := presence_ext
	if t_dismiss >= 0.0: pe *= 1.0 - s01(t_dismiss, t_dismiss + 0.2, clock)
	for q in P:
		var a: float = pclock - float(q["t0"])
		var life: float = q["life"]
		if a > life: continue
		keep.append(q)
		if a < 0.0 or used >= pool.size(): continue
		var k: float = q["k"]; var v0: Vector2 = q["v"]; var g: Vector2 = q["g"]; var p0: Vector2 = q["p0"]
		var pos: Vector2
		var vel: Vector2
		if k > 0.001:
			pos = p0 + v0 * (1.0 - exp(-k * a)) / k; vel = v0 * exp(-k * a)
		else:
			pos = p0 + v0 * a; vel = v0
		pos += 0.5 * g * a * a; vel += g * a
		var u := a / life
		var c: Color = q["c"]
		var fin: float = q["fin"]
		var al := c.a * clampf(u / maxf(fin, 0.001), 0.0, 1.0) * (1.0 - s01(0.55, 1.0, u)) * pe
		var sz := lerpf(float(q["s0"]), float(q["s1"]), u)
		var sp: Sprite2D = pool[used]; used += 1
		var stv := 1.0 + float(q["st"]) * clampf(vel.length() / (1000.0 * S), 0.0, 3.0)
		sp.visible = true; sp.position = pos; sp.scale = Vector2(sz, sz * stv) * 2.0
		sp.rotation = vel.angle() - PI / 2.0 if vel.length() > 1.0 else 0.0
		sp.modulate = Color(c.r, c.g, c.b, al)
	P = keep
	# 顺着链子流的光点
	var rk: Array = []
	var n := last_pts.size()
	for r in riders:
		r["age"] = float(r["age"]) + dt
		r["u"] = float(r["u"]) - float(r["sp"]) * dt
		if float(r["u"]) <= 0.03 or n < 2 or phase != "aim": continue
		rk.append(r)
		if used >= pool.size(): continue
		var fu: float = float(r["u"]) * float(n - 1)
		var i0 := int(floor(fu)); var i1 := mini(i0 + 1, n - 1)
		var a0: Vector2 = last_pts[i0]; var a1: Vector2 = last_pts[i1]
		var pos2 := a0.lerp(a1, fu - float(i0))
		var nrm := (a1 - a0).orthogonal().normalized() if (a1 - a0).length() > 0.01 else Vector2.UP
		pos2 += nrm * float(r["lat"]) * (0.5 + 0.5 * sin(float(r["age"]) * 9.0))
		var sp2: Sprite2D = pool[used]; used += 1
		sp2.visible = true; sp2.position = pos2; sp2.rotation = 0.0
		sp2.scale = Vector2.ONE * float(r["sz"]) * 2.0
		sp2.modulate = Color(0.6, 1.0, 0.9, 0.7 * s01(0.0, 0.1, float(r["age"])) * s01(0.03, 0.15, float(r["u"])) * pe)
	riders = rk
	for i in range(used, pool.size()): pool[i].visible = false

# ------------------------------------------------------------------ 网格

func strip(mi: MeshInstance2D, pts: Array, half: float) -> float:
	var verts := PackedVector2Array(); var uvs := PackedVector2Array(); var acc := 0.0
	for i in pts.size():
		var pp: Vector2 = pts[i]
		var dd: Vector2 = (pts[mini(i + 1, pts.size() - 1)] as Vector2) - (pts[maxi(i - 1, 0)] as Vector2)
		if dd.length() < 0.001: dd = Vector2(1, 0)
		var nn := dd.normalized().orthogonal()
		if i > 0: acc += (pp - (pts[i - 1] as Vector2)).length()
		verts.append(pp - nn * half); uvs.append(Vector2(acc, 0.0))
		verts.append(pp + nn * half); uvs.append(Vector2(acc, 1.0))
	var arr := []; arr.resize(Mesh.ARRAY_MAX); arr[Mesh.ARRAY_VERTEX] = verts; arr[Mesh.ARRAY_TEX_UV] = uvs
	var m: ArrayMesh = mi.mesh as ArrayMesh
	if m == null:
		m = ArrayMesh.new(); mi.mesh = m
	else:
		m.clear_surfaces()
	m.add_surface_from_arrays(Mesh.PRIMITIVE_TRIANGLE_STRIP, arr)
	return acc
