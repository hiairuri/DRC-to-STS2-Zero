# SilentFX 着色器勘察报告（02_shaders）

> 勘察范围：`3817850774/` 下 `shaders/`（26）、`suppress3/`（11+1 gd）、`wraith/`（12+2 gd）共 **49 个 .gdshader + 3 个 .gd 舞台脚本**；C# 侧对照 `silentfx_decomp/split/`。结论先行：**这是一套"全程序化、零贴图依赖"的 2D 特效管线**——除读屏后处理和少量纹理重着色外，全部形状由解析函数 + FBM 噪声场生成；所有时间参数（clock/flow/scroll/churn）由代码推进而非 shader 内 TIME（仅约 6 个 shader 直接用 TIME），以便子弹时间统一减速。C# 与 GDScript 通过"编译 .gd 舞台 + Call() 驱动"和"ShaderLib 缓存 + SetShaderParameter"双桥接。

---

## ① 着色器总表（49 行）

> render_mode 省略标注时均为默认（blend_mix, unshaded 视声明而定）。"服务对象"依据 shader 头注释与 C# 调用点（`文件:行号`）双向确认。核心手法一列只记最具辨识度的算法。

### shaders/ 目录（26 个）

| # | 文件 | shader_type / render_mode | 服务对象 | 核心手法 | 关键 uniform |
|---|------|--------------------------|---------|---------|-------------|
| 1 | acid_tide | canvas_item / blend_mix, unshaded | Corrosive Wave 腐蚀波（战场地面酸潮，Powers.CorrosiveWaveFx.cs:153,164） | **伪透视地面重建**：每像素按行反算地板坐标 `s=(row-yv)/den, f=((px.x-her.x)/s, focal*(1/s-1))`；FBM 噪声前沿 rag；feet[8] 生物涟漪；plips 气泡程序动画；stain+glow 单pass | alpha,size,horizon,vanish,focal,her,far_soft,reach,crest,flow,pulse_r[4],pulse_a[4],feet[8],feet_n,kick,stain,glow,haze,bubbles,layer,wade,seed,deep,bright,hot |
| 2 | card_edge | canvas_item / unshaded | Precise Cut 持卡磨边（Cards.PreciseCutChains.cs:321,412） | 沿卡缘 SDF 斜面 bevel（`mix(STEEL_HI,STEEL_LO,pow(t,0.75))`+光带）；9 个解析火花（重力 `+0.5*g*t²`、速度尾迹 seg 拉伸）；glint 流光；star 星芒 | size,card,pad,from_x,side_len,depth,bare,hot,spark_age,glint,star,ready,lit,seed |
| 3 | cut_shift | canvas_item / unshaded + **screen** | Precise Cut 出招画面切线错位（Cards.PreciseCutFx.cs:645） | 读屏沿切线两侧反向平移；part 权重（中段大、两端无、远缝无）；alpha=part*24 自清除 | view,point,dir,half_len,reach,shift,band |
| 4 | echo_figure | canvas_item / blend_add, unshaded | Afterimage 残影·程序人形（Powers.AfterimageFx.cs:162） | 解析兜帽人形（head 椭圆+cloak 梯形 flare）；scan 扫描线；flick 闪烁；shift 相位错相 | alpha,shift,tint |
| 5 | echo_outline | canvas_item / blend_add, unshaded | Afterimage 残影·纹理轮廓版 v0.72（Powers.AfterimageFx.cs:114） | **纹理 alpha 8 方向 16 tap 轮廓提取**：shape=max(a,sum/16*1.25), core=min(a,lo), glow=shape-core*inner | alpha,shift,tint,edge_px,inner |
| 6 | expose_wipe | canvas_item（无 render_mode） | Expose 白线扫游戏图标（Cards.ExposeWipe.cs:343,1483） | vertex 传世界坐标 world_y；step 切割双 mode；band 白热；shake 顶点抖动 | cut_y,mode,band,heat,shake,hot |
| 7 | freeze | canvas_item / unshaded + **screen** | Bullet Time 全局滤镜（Cards.TimeFreeze.cs:478） | 读屏去饱和偏冷+drain；四角 vignette；**RGB 分离**（R/B 各偏 ±pull）；8 环折射推挤+空气 shimmer  wobble | amount,cold,drain,lift,corners,split,shimmer,centre,ring_width,ring_bend,rings[8] |
| 8 | malaise_mist | canvas_item / blend_premul_alpha, unshaded | Malaise 红雾（Cards.MalaiseFx.cs:577） | **40 puff 幂混合 M=2.5**（重叠不肿胀）；anchor 平均共享噪声坐标；域扭曲 mist() 三层噪声；噪声蚀边 rag 随 age 增大；内部发光 rim/body/core；预乘混合 | count,at[40],ax[40],ex[40],sx[40],an[40],ro[40],clock,seed,intensity,wisp,core,grain,cover,heat,warp,col_rim,col_body,col_core |
| 9 | malaise_strand | canvas_item / blend_premul_alpha, unshaded | Malaise 单缕雾丝（Cards.MalaiseFx.cs 同文件 strand 部分） | 折线管道 SDF（seg[N] 中点线+半宽 best 距离场）；沿程条纹；两端熔解；front 分层绕珠 | count,seg[32],fl[32],total,root,flow,clock,seed,intensity,layer,cover,grain,waver,col_rim,col_body,col_core |
| 10 | pinpoint_reticle | canvas_item / blend_add, unshaded | Pinpoint 瞄准环（Cards.PinpointFocus.cs:56） | lines() 解析环+4 刻度；focus 模糊↔锐（soft 0.26→0.022）；split 双影；shut 收拢闭点 | focus,alpha,split,lock,shut,col,hot |
| 11 | shade_mist | canvas_item / blend_mix, unshaded | Shadow Step/Shadowmeld 暗雾（Core.ShadeMist.cs:58） | 三形态（柱/球/贴岸堤）；域扭曲；**噪声蚀边** `1-smoothstep(0.12,1.08,edge+(n-0.5)*wisp)`；hollow 中空；thin 处紫光 | intensity,pulse,mode,seed,speed,hollow,foot,bulge,taper,lean,front,reach,wisp,glow,col_ink,col_deep,col_glow |
| 12 | shadow_blur | canvas_item / unshaded + **screen** | Shadow Step 飞牌动态模糊（Cards.ShadowStepFx.cs:1027） | 读屏 7×3=21 tap 高斯（沿 smear 拉长）；dither 抖动去条带；most<0.05 全透明 | view,smear,soft,rim |
| 13 | shadow_cloud | canvas_item / blend_mix, unshaded | Shadow Step 影子雾主体（Cards.ShadowCloud.cs:112） | **48 puff 幂混合**；head/spin/twist 身体雾随转（rim 转得少）；combed 速度梳理（comb/sweep 拉条）；age 残留雾；veil 条纹模式；共享 shade_mist 蚀边 | count,at[48],ax[48],ex[48],sx[48],run[48],cb[48],head,head_r,spin,twist,clock,seed,intensity,pulse,wisp,glow,veil,grain,core,comb,sweep |
| 14 | shadow_pool | canvas_item / blend_mix, unshaded | Shadow Step 脚下暗影水洼（Powers.ShadowPresence.cs:169） | 径向 swirl 旋转（`sw=t*0.35+r*2.2` 外圈滞后）；rim 呼吸边；lip 紫光环 | intensity,pulse,seed,calm,col_core,col_edge,col_glow |
| 15 | shadow_shade | canvas_item / blend_mix, unshaded | Shadow Step 屋顶遮光（Cards.ShadowStepFx.cs:901） | world 坐标区间遮光+高度渐变+lean 地面投影后移 | from_x,to_x,soft,deep,low,lean,amount,col_ink |
| 16 | shadow_shock | canvas_item / unshaded + **screen** | Shadow Step 冲击波环（Cards.ShadowStepFx.cs:717） | 读屏 oval 环形折射（lie 贴地压扁）；ridge+bend 推挤回摆；RGB 分离；dusk 染色；open 中心保护 | view,centre,radius,width,push,lie,wake,split,dusk,tint |
| 17 | sigil | canvas_item / blend_add, unshaded | 通用光环纹（Core.FxKit.cs:554 / Cards.EnergyBeads.cs:187） | band 环带+dash_mask 虚线+ticks 刻度（atan 分箱）+glow 高斯晕+fill 内光+spin | tint,alpha,radius,width,dashes,spin,inner,inner_width,inner_dashes,inner_spin,ticks,tick_len,tick_width,tick_out,glow,fill |
| 18 | toxic_fog | canvas_item / blend_mix, unshaded | Noxious Fumes 毒雾/Mirage 幻影（Powers.FogKit.cs:50） | 双形态：bank 贴地长堤（roll 前滚/cone 喷射/fan 张开/head-tail 前后端）与 ellipse（fill 补洞生长/scatter 炸散/flash 闪）；**canvas 固定坐标噪声防拉伸** | intensity,thick,layer,shape,origin,size,seed,scroll,churn,dir,floor_y,rise,near_soft,far_soft,head,tail,roll,cone,mouth,fan,pulse,pulse_at,fill,flash,scatter,col_deep,col_mid,col_glow |
| 19 | wail_warp | canvas_item / unshaded + **screen** | Piercing Wail 屏幕冲击（Cards.PiercingWailFx.cs:438） | 读屏 oval 波前（tall 竖椭圆/sides 上下切口成双弧）；3 fronts 多波；bend 推挤回摆；RGB 分离；falloff 衰减 | amount,centre,aspect,tall,sides,width,strength,wake,split,falloff,tint,glow,fronts[3],powers[3] |
| 20 | wraith_cardfire | canvas_item（无 render_mode） | Wraith Form 古卡牌火焰变幽灵火（Cards.WraithCardFire.cs:51） | 原火焰纹理双通道重映射（body=max(rgb),heart=min(rgb)）→ 三段色 | green,power,deep,mid,hot |
| 21 | wraith_flame | canvas_item / blend_add, unshaded | Wraith Form 开场火舌（Cards.WraithFigure.cs:772） | 泪滴形 `0.5*sqrt(hh*5)*pow(1-hh,0.85)`；噪声上滚舔焰；core/body 分段色；halo 脚光 | clock,power,seed,lean,col_core,col_mid,col_edge |
| 22 | wraith_gauze | canvas_item / blend_mix, unshaded | Wraith Form 裹尸布条·Line2D（Cards.WraithFigure.cs:455） | fray 磨损边；crease 游走褶皱；hem 卷边高光；mesh 编织网纹（双正弦）；solid 变实体 | solid,glow,threads,col_pale,col_shade,col_lit |
| 23 | wraith_haze | canvas_item / blend_mix, unshaded | Wraith Form 持卡四边渗雾（Cards.WraithHeld.cs:60） | 到四边最近距离 d；reach*(0.35+1.30*n) 舌状内缘；域扭曲；乘 COLOR.a 让手牌可读 | intensity,reach,clock,aspect,seed,col_deep,col_fog,col_light |
| 24 | wraith_scream | canvas_item / unshaded + **screen** | Wraith Form 尖叫双波（Cards.WraithFormIntroFx.cs:1110） | 读屏双 crest（二波 0.58 距/0.75 宽/0.55 强）；bend 推挤；RGB 分离；tint 绿光 | amount,centre,reach,ring_width,bend,split,tint |
| 25 | wraith_shroud | canvas_item / blend_mix, unshaded | Wraith Form 灵体披风巨雾（Cards.WraithFormIntroFx.cs:470） | 椭圆冠+肩+撕裂下摆 edge 场；rise 从心脏聚集；burst 外抛撕裂；layer 前层中空；thin 处酸绿 rim light | clock,intensity,rise,burst,layer,seed,lean,col_dark,col_mid,col_pale,col_rim |
| 26 | wraith_smoke | canvas_item / blend_mix, unshaded | Intangible/Wraith Form 角色烟柱（Powers.WraithFormFx.cs:39,49） | 上窄下宽柱（halfw 渐变）；噪声上滚；hollow 前层中空；pulse 受击闪；内部自发光 | intensity,pulse,layer,seed,speed,col_dark,col_mid,col_rim |

### suppress3/ 目录（11 个，注释为中文，uniform 排版极简）

| # | 文件 | shader_type / render_mode | 服务对象 | 核心手法 | 关键 uniform |
|---|------|--------------------------|---------|---------|-------------|
| 27 | atmos | canvas_item（默认 blend_mix） | 压制全局绿雾·含 UI（suppress_stage.gd:299-303） | FBM 域扭曲；四边浓中间淡；**hand_keep 手牌区减淡**；flow 往 flow_to 被吸；shock 冲击波推雾+亮环 | t,amt,flash,ws,lo,hi,hand_keep,flow_to,flow,shock_at,shock_r,shock_amt |
| 28 | aura | canvas_item | 她身上上升的雾（suppress_stage.gd:253-263） | FBM 滚动 + TEXTURE alpha 遮罩（只在轮廓内起雾）+ UV 微扰 | t,amt,ws,tint |
| 29 | burst | canvas_item | 冲击放射速度线（suppress_stage.gd:289-290） | atan 角度分箱 bins=150；hash(b) 定有无/宽度/起点/长度；径向 smoothstep | center,r0,r1,amt,t,ws |
| 30 | fog | canvas_item | 地上一层雾（suppress_stage.gd:250-252） | FBM 域扭曲 + 高度带渐入（y0→y1） | t,ws,tint,y0,y1,amt |
| 31 | her | canvas_item | 她的半透明魂身（suppress_stage.gd:254-257） | wob 飘动；**smear 16 tap 预乘拖影**；8 向侵蚀 edge；desat 去饱和；light_at 点光；mist 化雾；**dissolve 双频噪声溶解+亮边** | dark,ghost,presence,dissolve,t,desat,cool,smear,mist,pxk,ws,light_at,light_amt,light_r,feet_y,fade |
| 32 | lurk | canvas_item | 敌人脚下翻涌雾·burst 上炸（suppress_stage.gd:273-274） | 椭圆场+域扭曲；burst 竖拉伸(1+2.6x)+上滚加速；pow(1-burst,1.4) 炸开本体消 | t,center,radius,amt,burst,beat,ws |
| 33 | ripple | canvas_item + **screen_tex**(mipmap) | 撕魂全屏波纹·连 UI（suppress_stage.gd:308-309） | textureLod 环形折射推挤 + 青绿加法；alpha=k*4 自清除 | screen_tex,center,radius,width,amp,span |
| 34 | rope | canvas_item | 雾链（suppress_stage.gd:268-270） | UV.x 沿链长；core 高斯芯；3 股正弦错相 strands；knot 亮结+spike 棘；pulse 脉冲；tension 松紧 | t,ws,flow,tension,total,pulse_pos,pulse_amp,knots,alpha_mul,tail_fade,head_fade,glow |
| 35 | smear | canvas_item | 手的拖影线·尾细头粗（suppress_stage.gd:265-266） | w=mix(0.22,0.85,s) 宽度渐变 + 噪声条 + 两端 fade | t,total,alpha_mul,ws |
| 36 | soul | canvas_item + **etex** sampler | 敌人的魂·外扯拉伸（suppress_stage.gd:278-279） | **src() 逆映射**（rot 旋转+stretch 沿 sdir 拉脖+grow 头放大+pull 二分 y 映射）；13 tap 黄金角螺旋模糊；halo 亮边；neck 条纹；tear 撕裂；burst 噪声炸开；无纹理时程序魂影（含眼窝） | etex,has_tex,tpos,tsize,epos,esize,ws,pull,t,head_at,grow,rot,sdir,opacity,stretch,full,full_w,blur,lift_fade,glint,vivid,burst |
| 37 | tether | canvas_item | 魂下端与身体的连接缕（suppress_stage.gd:276-277） | 线段参数化；wdt 中细两端粗 `(1-|s-0.5|*2)^1.5`；off 加 fbm 摆动 | top,bot,width,t,amt,ws |

### wraith/ 目录（12 个）

| # | 文件 | shader_type / render_mode | 服务对象 | 核心手法 | 关键 uniform |
|---|------|--------------------------|---------|---------|-------------|
| 38 | arc | canvas_item / blend_add | 嚎叫上半弧环（wraith_play.gd:1120-1122） | ring 半径带 + sect 角度扇形（spread） | radius,width,alpha,spread,colour |
| 39 | body | canvas_item | Wraith Form 待机身体（wraith_form.gd:634-636） | **breathe() 高斯权重呼吸**；arm() 绕肩+绕肘两段旋转；wound() 逐圈缠绕显现（分带+左右交替）；horn 伸出 | origin,extent,chest,breath,swell,lift,shoulder_l/r,fist_l/r,arm_l,arm_r,hands,arms_off,torso,head,horn,headc |
| 40 | cone | canvas_item / blend_add | 嚎叫光锥（wraith_play.gd:1123） | halfw=0.035+0.44*h 锥形扩张 | alpha,colour |
| 41 | dress | canvas_item | 裙子飘动（wraith_form.gd:632-633） | 共享 breathe()；双正弦摆动（phase+uv.y*9 与 2phase-uv.y*17+uv.x*6）；hang 后摆；grow 噪声边长 | phase,amount,hang,origin,extent,chest,grow,breath,swell,lift |
| 42 | fire | canvas_item | 卡牌火焰舔动（wraith_play.gd:1024-1027） | hgt 高度权重双正弦 UV 扭曲 + 亮度调制 | phase,foot,amount,seed |
| 43 | ring | canvas_item / blend_add | 待机展开整环（wraith_form.gd:666-667） | ring 带 + inner 内部 0.10 微光 | radius,width,alpha,colour |
| 44 | ring2 | canvas_item / blend_add | 尖叫整环（wraith_play.gd:1124-1126） | ring 带（ring 的简化无 inner） | radius,width,alpha,colour |
| 45 | strip | canvas_item | 纱布条带·卡片画风（wraith_form.gd:613） | v 分段硬色带（4 段+j 阶梯抖动，剪纸感）；tint.r*31 区分布条 | col_dark,col_mid,col_pale,col_light |
| 46 | torso | canvas_item | 出招身体（wraith_play.gd:1001-1004） | **d_breath/d_head(球面 z 深度 tilt)/d_sleeve 位移叠加 + 10 次迭代不动点反解** `p-=0.85*(where(p)-q)` | origin,extent,chest,breath,swell,lift,shoulder_l/r,fist_l/r,sleeve_l,sleeve_r,headc,headr,headz,neck,tilt,nod,rise,hscale |
| 47 | wraith_haze | canvas_item / blend_mix, unshaded | 同 #23（wraith/ 下副本，内容逐字一致） | 同 #23 | 同 #23 |
| 48 | wraith_veil | canvas_item / unshaded + **screen_tex** | CanvasGroup 运动模糊（wraith_form.gd:865-867 / wraith_play.gd:1455-1457） | textureLod 读组缓冲 **13 tap 加权模糊**；core_a→core_b 线段距离 k 权重（身上清晰、远端模糊）；own 是否再乘组色 | screen_tex,blur,core_a,core_b,near,far,own |
| 49 | wraith_wave | canvas_item / unshaded + **screen** | 尖叫第一波推屏·含 UI（wraith_play.gd:1506-1507） | 读屏波前 ridge 推挤+回摆 `ridge-0.38*sin(behind*2.2)*exp(-behind*0.85)`；tint 辉光 | centre,aspect,front,width,strength,power,tint,glow |

**统计特征**：49 个中 canvas_item 占 100%（无 spatial/particles/sky）；约 20 个共享同一套 `hash21+vnoise+fbm`（4~5 层，旋转矩阵 `vec2(1.6x+1.2y+3.1,-1.2x+1.6y+1.7)` 防轴对齐）；8 个读屏（screen/screen_tex）；仅 6 个直接用 TIME，其余 clock 由代码推进（子弹时间统一减速）；uniform 数组最大 48（shadow_cloud）。

---

## ② .gd 脚本分析（3 个舞台脚本）

### 2.1 suppress_stage.gd（1286 行）——压制（先古卡）整套表演

- **角色**：C# `SuppressStage.cs:279-287` 用 `GDScript.SourceCode + Reload(false)` 编译它，实例化后 `Call("build")` 搭场景、每帧 `Call("step", dt, mouse, presence)`。三层（back 角色身后 / front 身前 / top 界面之上）GlobalTransform 均为单位阵，脚本坐标=场景坐标（suppress_stage.gd:2-5）。
- **场景结构**（build :233-293）：veil 暗幕、fogb(fog)、aura_b/f(aura)、her[POSE] 5 张姿势图共用 hmat(her.gdshader)（k2/k25/k3/k4/k5，:14-20 姿势表）、ghosts 拖影、slines×2(smear)、rope_bk/fr/tail+fingers×3(rope)、lurk(lurk)、tether(tether)、soul(soul)、sglow/gnd 光斑、flash/flash2、ring1-3、pool 220 光点、top 层 atmos(atmos)、grade(MUL 偏绿)、ripple(ripple)、burst_q(burst)、TP 48 界面光点。
- **uniform 驱动模式**：`shader()/smat_new()` 从 C# 传入的 CACHE 读写 Shader 对象（:179-190）；静态参数（ws 场景缩放、pxk 贴图像素比、feet_y 等）在 build 一次设好（:251-277）；每帧参数在 step() 链路上设置——`hmat.set_shader_parameter("presence"/"ghost"/"dissolve"/"smear"/"mist"/"t"/"light_at")`（:498-501,578）、`rmat` 链子 11 个参数（:811-814）、`smat` 魂 15 个参数（:673-678）、`amat` 全局雾 8 个参数（:1057-1060）。
- **动画/时间线组织**：phase 状态机 hover→free→aim→cancel/rel→tear→gone（:99）；`F1=1/30` 帧常量驱动 2 帧沉底/2 帧化雾冲起（:420-443）；撕魂时间线 tm：flight(0.13~0.24s 魂飞行)→FREEZE(0.12 定格)→SETTLE(0.12)→SLOW(0.55 慢放)→FADE(0.70 散)，光点钟 pclock 跟随 prate 慢放（:426-434）；输出字典 `{"done","yank","shake","hold","card_show","dim","cues"}` 给 C#（震屏/顿帧/拽敌人/压暗/音效提示，:538）。
- **网格生成**：`strip()`（:1268-1285）把折线翻成 TRIANGLE_STRIP 双边条带，累积弧长作 UV.x——雾链、拖影线、雾指头、链尾全部复用。
- **预热**：`warm()`（:167-177）每调一次读一张贴图/编译一个 shader，全读完返回 true；C# `SuppressStage.cs:178` 在手里有压制时每帧调一次，实现分帧不卡顿。

### 2.2 wraith_form.gd（902 行）——Wraith Form 持卡待机

- **角色**：纱布条在屏幕边缘等待→甩入→盘绕成卡画女子→SNAP(0.76s) 啪地展开→透明呼吸待机。由 `WraithStage.cs:89` 编译。
- **场景结构**：root_b/root_f + 可选 root_t（界面之上）；`figure=CanvasGroup`（半透明组，:628）；rig.json 定义 layers/strands（:602-603）。
- **头发 25 缕 strand**：`make_strand()` 从 rig 取中线点，Polygon2D+tris()（:162-215）；`track()` 沿弧取点（s≥0 静态弧二分；s<0 入场弧 ap/cum 二分，:217-247）；`pose_strand()` 三阶段：delay 等待蠕动→land 甩入（stretch 拉伸）→out 展开 settled，叠加 writhe 扭动与 idle_points 呼吸摇摆（lag 滞后，:266-291,293-376）。
- **身体组件**：wraps 16 圈身带 + armwraps 6 臂带 + shreds 26 碎条，均 `make_ribbon()` 双层 Polygon2D（前/后用 vertex_colors alpha 表达 front 分层，:380-393）；`pose_wrap()` 螺旋缠绕（:422-446）。
- **uniform 驱动**：body_mats 数组统一设 breath/arm_l/arm_r/hands/torso/horn/head（:546-552）；dress_mat 的 grow/phase/hang/breath（:535-557）；haze_mats 的 clock/intensity/reach（:565-568）；ring_mat/ring2_mat 的 radius/width/alpha（:578-586）；veil_mat 的 blur/core_a/near/far/own（:882-887）。
- **关键创新 WZ_HEAD 协议**（:698-782）：`zoned()` 运行时把任意 shader 源码注入 `wz_a(COLOR.a)` 份额函数 + vertex 注入 wz_p 世界坐标——同一物体画两份（角色层/顶层），每份只出自己份额的 alpha，两份之和恰好等于画一次。解决"特效要盖过 UI 又必须留在角色层"的分层矛盾。`zone_numbers()` 把 ui_top/ui_bot/ui_corner/ui_left/ui_right/ui_soft 换算成场景坐标带（:760-782）。
- **运动模糊**：`veil_up()` 把节点裹进 CanvasGroup 挂 wraith_veil.gdshader（:859-868）；`pose_veil()` 按距她距离设 blur（:870-895）。
- **节奏输出**：`times()=[SNAP, SNAP+0.30, SNAP+0.95]`、`phase()=[相位, breath]` 供 C# 衔接出招动画（:897-901）。

### 2.3 wraith_play.gd（1525 行）——Wraith Form 出招

- **角色**：卡光入胸(T_BURST 0.60)→爆发前冲(T_GO 0.67, LUNGE 0.52)→火刃出鞘→掷火(T_REL 0.97, FLY 1.15, T_HIT)→嚎叫→纱布被风逐条撕碎(PULL_SPAN 1.15)直至消散。由 `WraithStage.cs:90` 编译，C# 每帧 `Call("set_time", t, clock)`。
- **关键帧动画系统**：KEYS 6 个姿势字典（standby/drawing it in/it bursts/the X behind her/down both sides/stripped，:98-105）+ `x_at(t)` 全参数插值（coil/out 缓动 + trem 颤抖 + bump 过冲 + wob 摆动叠加层，:120-189）——约 45 个姿态参数每帧解算。
- **头发/纱布风场**：`wind_of()` 定风向（远离嘴）:403-424；`wind_turn()` 逐段角度向风插值（自由端先转根端后转）+ FLUT 抖动 :426-446；`torn_of()` 撕裂瞬间姿态快照（与当前时间解耦，可预排）:448-459；`fly()` 飞出（x² 加速+FLY_LAG 根端滞后+侧漂）:470-499；`puff()` 撕裂灰尘 :501-529。
- **火焰系统**：`fire_ways()` Catmull-Rom 采样 101 点路径（fpath）+ 弧长重参数化 + 顶部压回（fire_pad）:1164-1225；`fmap()` 速度曲线（快-慢-顶点悬停-加速下落）:191-199；tails 22 个火舌尾迹沿路径历史排布 :914-935。
- **shader 驱动**：torso_mat/neck_mat（breath/tilt/nod/rise/hscale/sleeve，:762-767）；dress_mat（breath/phase）；fire_mats（phase/amount，:785-789）；嚎叫三件套 arc/cone/ring2 的 alpha/width/spread（:832-861）；wave_mat（centre/front/strength/power，:1514-1517）；`pose_veils/pose_fire_veils` 按风速设 wraith_veil 的 blur（fast=d(fmap)/dτ，:1487-1488）。
- **与 form 共享 WZ_HEAD 协议**（twin_tree/twin_poly/zone_pair/sync_top，:1287-1437）。
- **输出**：`times()=[T_BURST,T_GO,T_HIT,end]`、`chest()` 供 C# 对齐游戏逻辑 :1519-1524。

---

## ③ 技术手法归类与系列演化

### 3.1 共享基础件（约 20/49 个 shader 复用）

| 基础件 | 内容 | 出现于 |
|--------|------|--------|
| hash21+vnoise+fbm | `fbm` 4~5 层，每层旋转矩阵 `vec2(1.6x+1.2y+3.1, -1.2x+1.6y+1.7)`（防轴对齐伪影） | acid_tide/malaise_*/shade_mist/shadow_cloud/toxic_fog/wraith_flame/wraith_haze/wraith_shroud/wraith_smoke/suppress3 大部分 |
| mist() 三层噪声 | n（蚀边）/n2（body 斑驳）/fine（细 streak），warp 域扭曲 `fbm(q+warp)` | malaise_mist, shadow_cloud, 及 shade_mist 的内联版 |
| 噪声蚀边公式 | `1-smoothstep(0.12,1.08, edge+(n-0.5)*wisp)`：edge=解析形状场，wisp=破碎度 | shade_mist, shadow_cloud, malaise_mist, toxic_fog, wraith_shroud |
| bell()=exp(-x²) | 软包络（pow 负数无定义，全项目禁用 pow(x,2)） | acid_tide, pinpoint_reticle, card_edge, burst... |
| efade 四边淡出 | smoothstep 乘积防 quad 边缘露馅 | wraith_flame, wraith_shroud, wraith_smoke, toxic_fog(frame) |

### 3.2 雾家族演化（shade_mist → shadow_cloud → malaise_mist → toxic_fog → wraith_shroud）

1. **shade_mist（v1 基础）**：解析 edge 场（三形态）+ 噪声蚀边 + 中空 + 边缘紫光。全部参数静态。
2. **shadow_cloud（v2 场融合）**：引入 48 puff 数组，`pow(thick,2.5)*exp(-2.5*d2)` 幂混合（重叠不肿胀）；新增 spin/twist 身体雾随转（rim 比 middle 转得少，产生"手臂"感）、combed 速度梳理、age 残留雾、veil 条纹模式。**从静态形状进化为可运动体**。
3. **malaise_mist（v3 数据驱动）**：puff 结构升级为 at/ax/ex/sx/an/ro 六数组（位置/半径厚度/年龄可见度/拉伸/噪声锚点/旋转），噪声坐标按锚点加权平均（雾随质量旅行翻滚）；改预乘混合 + 内部发光（rim→body→core 三段色）。**从单体进化为有"物质感"的发光雾**。
4. **toxic_fog（v4 场景化）**：噪声钉死 canvas 坐标（quad 可任意移动不拉伸）；bank 形态带 roll 前滚/cone 喷射嘴/fan 张开/head-tail 前后端；ellipse 形态带 fill 补洞生长/scatter 炸散。**从特效进化为环境**。
5. **wraith_shroud/wraith_smoke（v5 角色化）**：rise 从心脏聚集、burst 外抛撕裂、layer 前层中空（保角色可读）、rim light 酸绿。**与角色表演绑定**。
- suppress3/ 的 fog/aura/lurk/atmos 是这套的极简单文件版（单 fbm+高度带）。

### 3.3 shadow_* 系列演化（pool → shade → cloud → shock → blur）

| 代 | 文件 | 增量 |
|----|------|------|
| v1 | shadow_pool | 原地漩涡水洼：径向 swirl（外圈滞后）+ lip 紫边。地面贴花。 |
| v2 | shadow_shade | 区间遮光片：纯几何（from_x..to_x 软边+高度渐变）。**从贴花进化为光照**。 |
| v3 | shadow_cloud | 可行走雾团（见 3.2 v2）。**从静态进化为动态体**。 |
| v4 | shadow_shock | 读屏冲击环：oval 折射+回摆+RGB 分离+dusk。**从业内进化为全屏后处理**。 |
| v5 | shadow_blur | 读屏 21 tap 方向模糊。**后处理细化**。 |

路径清晰：**地面贴花 → 光照遮挡 → 动态体积 → 屏幕空间后处理**。

### 3.4 wraith_* 系列演化

- **残影**：echo_figure（程序人形）→ echo_outline v0.72（纹理 alpha 8 向 16 tap 轮廓化，适配任意皮肤）。
- **雾**：wraith_haze（持卡四边渗雾）→ wraith_shroud（灵体巨雾）→ wraith_smoke（角色烟柱）→ wraith_gauze（纱布条带 Line2D）。
- **光环**：ring（整环+内光）→ ring2（简化整环）→ arc（扇形弧）→ cone（光锥）→ sigil（完整符文系统：双环+虚线+刻度+光晕）。
- **火**：fire（UV 扭曲贴图火）→ wraith_flame（程序泪滴火舌）→ wraith_cardfire（原火焰重着色双通道映射）。
- **角色驱动**：body.gdshader（待机：呼吸+手臂+wound 缠绕）→ torso.gdshader（出招：+头部 rigid tilt/nod + 10 次迭代反解）→ dress.gdshader（共享 breathe()）。
- **魂**：her.gdshader（魂身：拖影+化雾+溶解）→ soul.gdshader（敌魂：逆映射拉伸+螺旋模糊+炸开）。

### 3.5 读屏后处理家族（8 个，共享 ridge/bend/RGB-split 三件套）

freeze、wail_warp、wraith_scream、shadow_shock、ripple、wraith_wave、cut_shift、shadow_blur。共同配方：
```
crest = exp(-x*x)                      // x=(far-radius)/width
bend  = crest + wake*sin(behind*2.4)*exp(-behind*0.75)   // 推挤+回摆
seen  = vec3(tex(look+pull).r, tex(look).g, tex(look-pull).b)  // RGB 分离
```
差异只在几何（圆/竖椭圆/上下切口/切线）与附加（tint 辉光、dusk 染色、shimmer、vignette）。

---

## ④ C# 侧 shader 管理机制

### 4.1 ShaderLib（缓存与热重载）—— SilentFX.Core.ShaderLib.cs:4-111

- `Cache: Dictionary<string, Shader>` 按名缓存；`Get(name)` 读文件赋 `Shader.Code`（:48-74）；`NewMaterial(name)` 包 ShaderMaterial（:76-90）。
- `Warm()` 扫描 `shaders/*.gdshader` 全部文件名入 AssetWarmup 队列（"shader/<name>"，:10-41）。
- `Reload()` 重读所有文件改 Code（:92-110）——对应每个 shader 头注释 "Edit and press F8 to reload" 的开发循环。
- `Missing` 集合防重复报错；`FxKit.Alive` 检查对象存活。

### 4.2 ShaderWarmup（管线预热）—— SilentFX.Core.ShaderWarmup.cs:4-280

- 队列 `Pending: Queue<Shader>`；`Step()` 每次出一个 shader，建 **32×32 SubViewport**（TransparentBg/Disable3D/UseHdr2D/Msaa2D 对齐主视口，:150-161），挂 ColorRect+Sprite2D+Polygon2D+Line2D 共享同一 ShaderMaterial（:166-202）；`_plain`（空 fragment shader）时额外造 3 个粒子 ProcessMaterial（:210-215）。
- 等 2 帧 `RenderingServer.FramePostDraw`（`_draws>=2`）后释放视口，取下一个（:227-234）——**逐 shader 编译并渲染 2 帧，把管线变体预热掉**，避免首次播放卡顿。
- 失败即 `_failed=true` 整体关闭并 Warn（:218-224）；`PlayerSettings.ShaderWarmupEnabled` 可暂停（:118-122）。
- `Primitives()` 预热空 shader（给粒子系统用，:28-41）。

### 4.3 AssetWarmup（统一调度）

`AssetWarmup.Add("shader/<name>", ...)`（ShaderLib.cs:26）、`"canvas pipelines"`（ShaderWarmup.cs:48）、`"Wraith scene pipelines"`（WraithSceneWarmup.cs:84）——每帧执行一步的分帧任务队列，避免加载时一次性卡顿。

### 4.4 uniform 设置约定

- 标量：`mat.SetShaderParameter(StringName, Variant)`，如 `"amount"`、`"rings"`（TimeFreeze.cs:483-489）。
- **数组直接传 C# 数组**：`Vector2[40] _at/_ax/_ex/_sx/_an/_ro`（MalaiseFx.cs:520-530,736-746）、`Vector4[8] _feet/_local`（CorrosiveWaveFx.cs:123-127,282-283）、`float[3] fronts/powers`（PiercingWailFx.cs:288-289）——Godot C# 的 Vector2[]/Vector4[] 可直接 Variant 化传给 `uniform vec2 at[40]`。
- 可调参数走 `Tuning.F("key", default)`（读 tuning.ini）：如 `shadowstep.wisp`（ShadowCloud.cs:118）、`wail.bend`（PiercingWailFx.cs:285）、`wave.flood_stain`（CorrosiveWaveFx.cs:285）、`afterimage.edge_px`（AfterimageFx.cs:122）。
- 预计算 StringName 静态字段（PCount/PAt/PAx...）避免每帧分配（ShadowCloud.cs:8、MalaiseFx.cs 同）。

### 4.5 .gd 舞台桥接（C# ↔ GDScript）

- 编译：`new GDScript(); script.SourceCode = File.ReadAllText(path); script.Reload(false)`，失败则 fallback "the old apparition is used"（SuppressStage.cs:279-287、WraithStage.cs:102-123）。
- 驱动：`script.New()` 实例化 → `Call("build", dir, cache, back, front, lo, hi, ...)`（SuppressStage.cs:375、WraithStage.cs:213,287）→ 每帧 `Call("step"/"set_state"/"set_time", ...)`（WraithSceneWarmup.cs:219,330）→ `Call("set_param", key, v)` 调 tuning。
- **C# 把 Dictionary cache 传进 .gd**：贴图/Shader/rig JSON 跨次游戏复用（suppress_stage.gd:234、wraith_form.gd:599-603、wraith_play.gd:963-967）。
- `WraithSceneWarmup.cs:88-135`：非战斗时在 SubViewport 真 build form/play 场景并渲染 2 帧——**整个 GDScript 舞台的预演预热**。

### 4.6 freeze shader 与游戏原生 Vfx 的交互（补记，二轮协作核验）

freeze（子弹时间）不是孤立滤镜——它是全项目与游戏原生特效系统耦合最深的 shader：

- **通用 Vfx 工厂监视器** `VfxFactoryPatches`（SilentFX.Patches.VfxFactoryPatches.cs:4-53）对 `MegaCrit.Sts2.Core.Nodes.Vfx` 命名空间下所有 `Create` 静态方法打 Postfix（排除 .Forms/.Backgrounds/.Events/.Examples 子命名空间与 27 项 Skip 名单），出口全部通向 freeze 侧：`Made` → `TimeFreeze.OnVfxCreated`（VfxFactoryPatches.cs:143 → TimeFreeze.cs:659）、`Burned` → `TimeFreeze.OnCardBurn`（:95 → TimeFreeze.cs:877）。即**子弹时间期间游戏创建的所有原生 Vfx 都被 TimeFreeze 拦截排队**；另 :128 有 `ShivFan.HidesThrow` 判断（冻结期隐藏飞刀 Vfx）。
- **Skip 名单前两项有专用通道**（非遗漏）：`NDamageNumVfx` 由 TimeFreeze.SpawnNumber 自行重建（TimeFreeze.cs:1199-1224）；`NHitSparkVfx` 走专用补丁 HitSparkHoldPatch.Postfix（HitSparkHoldPatch.cs:6-21）→ TimeFreeze.OnHitSpark（:1226，冻结期隐藏、事后重放）。两路均以 `_replaying` 重入守卫（:80,1202,1222）防止监视器拦截重放本身。
- **横向枢纽**：`TimeFreeze` 是全项目类间依赖最密集的节点——全库 **43 个 .cs 引用 TimeFreeze.***，其中直接调 `KeepLive/HoldsVisual/Defer` 的 21 个文件（Cards 13 + Powers 2 + Core 3，含 FxLifetime.cs:104 租约暂停判据、Sfx.cs:1238 延迟播音）。多个 shader 驱动类在册（ShadowStepHeld.cs:218/246/256、MalaiseFx.cs:1170/1581、WraithFigure.cs:360、SuppressRun.cs:166-167/227、StrangleRope.cs:415/664/681、EnergyBeads.cs:1055、FinisherHeld.cs:520、MementoHeld.cs:570、SkewerFx.cs:570、ExposeWipe.cs:318、PreciseCutFx.cs:797、EchoingSlashFx.cs:1147），决定自身视觉在子弹时间期间是否滞留。KeepLive（续命，视觉继续演算）与 HoldsVisual（暂停计时节流）语义不同，逐调用点全表以 recon-arch 的 01_architecture.md ④.8「类间依赖」为准。
- 归属提示（反编译切分伪影，与 recon-arch 01_architecture.md ③ 节互证）：HitSparkHoldPatch.cs:22 尾部的 `[HarmonyPatch(typeof(VfxCmd),"PlayVfx")]` 实际属于源码顺序下一类型 SceneVfxHoldPatch（source lines 20303-20323），勿按文件内属性直读。

---

## ⑤ Top15 可借鉴技术（移植到龙族零→杀戮尖塔2 / DyeFramework）

> 按"算法可复用性×实现完整度"排序。每条含核心公式与 uniform 设计要点。

### T1. 伪透视地面重建（acid_tide.gdshader:89-101）
```
row = max(px.y, horizon+1);  s = (row-yv)/den;          // 该行的缩放
f = vec2((px.x-her.x)/s, focal*(1/s-1));                // 屏幕像素→地板坐标
thin = 1.3*focal/(s*s*den);                             // 1 像素=多少地板深度
```
一个全屏 quad 即得倾斜地面；远近缩放、涟漪粗细（thin）全部自动。**移植点**：DyeFramework 地面技能范围圈、地形腐蚀/积水，一行 uniform（focal）调透视强度。

### T2. 读屏波前折射三件套（freeze/wail_warp/wraith_scream/shadow_shock:23-57）
```
crest=exp(-x*x); bend=ridge+wake*sin(behind*2.4)*exp(-behind*0.75);
look=uv-push; seen=vec3(tex(look+pull).r, tex(look).g, tex(look-pull).b);
```
推挤+一次回摆+RGB 色散，三个 uniform（strength/wake/split）调质感。**移植点**：受击屏幕扭曲、冲刺冲刺波、Boss 咆哮；配 BackBufferCopy 即可（suppress_stage.gd:307）。

### T3. FBM 域扭曲三层噪声 mist()（malaise_mist.gdshader:72-78）
```
w=fbm(q+vec2(0,t*0.10)); n=fbm(q+vec2((w-0.5)*1.6*warp+t*0.07, w*0.8*warp-t*0.05));
n2=fbm(q*2.3+vec2(w*1.3*warp,t*0.4)); fine=fbm(q*4.6+vec2(n*2.2*warp-t*0.5,w*2.0));
```
一次采样得 rim/body/streak 三通道；warp uniform 调"搅拌度"。**移植点**：一切雾/烟/火/魔法的底色算法，直接抄。

### T4. Puff 场幂混合（shadow_cloud.gdshader:116 / malaise_mist.gdshader:101）
```
c = pow(thick, 2.5) * exp(-2.5*d2); sum += c; field = pow(sum, 1/2.5);
```
M=2.5 是"软度"指数：重叠区几乎不肿胀，天然 SGIF 体积感。**移植点**：烟团、血雾、魔法云；配合 age 做残留拖尾。

### T5. 噪声蚀边/溶解（shade_mist.gdshader:94 / her.gdshader:45-47）
```
d = 1-smoothstep(0.12,1.08, edge+(n-0.5)*wisp);      // 蚀边
dn = nn(q1)*0.75+nn(q2)*0.25;                          // 双频溶解噪声
a *= 1-smoothstep(dn-0.08,dn+0.08,dissolve);           // 阈值带
c.rgb += vec3(0.30,0.55,0.52)*(smoothstep(dn-0.25,dn)-smoothstep(dn-0.08,dn+0.08))*0.8;  // 亮边
```
edge 是解析形状（圆/多边形都行），wisp 是破碎度——**无 outline 只有羽**。dissolve 双频混合比单噪声更有层次，且自带边缘发光。**移植点**：DyeFramework 溶解/影子/亡灵化，可直接替换现有阈值溶解。

### T6. 纹理 alpha 轮廓提取（echo_outline.gdshader:17-30）
```
8 方向×2 半径=16 tap: shape=max(a, sum/16*1.25); core=min(a, lo);
glow = shape - core*inner;  // 亮边+暗芯
```
任意皮肤/立绘实时出残影轮廓。**移植点**：受击闪白、残影、选中高亮；比 Sobel 便宜且无方向伪影。

### T7. 2D 骨骼位移+迭代反解（torso.gdshader:35-81）
```
where(p) = p + d_breath(p) + d_head(p) + d_sleeve(p,L) + d_sleeve(p,R);
fragment: for(i<10) p -= 0.85*(where(p)-q);   // 不动点反解（采样源坐标）
```
正向"骨骼移动顶点"不可行时，反解"这个像素该从哪取"。d_head 用球面 z 深度做 tilt 投影（head_z/headr/headz）。**移植点**：立绘呼吸、受击抖动、头部朝向；body.gdshader 的 arm() 两段旋转（绕肩 smoothstep(0,0.5,t)+绕肘 smoothstep(0.45,0.85,t)）也是标准答案。

### T8. 线段参数化条带（rope.gdshader:16-41 / smear / wraith_gauze）
```
UV.x=沿长(累积弧长), UV.y=横向(-1..1); core=exp(-pow(v/cw,2.0));
w=mix(0.22,0.85,s) 尾细头粗; strands=Σ exp(-((v-c_k)/0.13)²)*(0.55+0.45*n(...))
```
配 GDScript `strip()` 网格生成（suppress_stage.gd:1268-1285）。**移植点**：拖尾、锁链、光束、飘带；uniform tension 一键松紧。

### T9. 风场撕布（wraith_play.gd:426-499）
```
wind_turn: 每段角度 a0+dl*f 插值到风向，f=pow(lam,1+1.8*(1-u))（自由端先转）
fly: d=FLY_FAR*x²*(0.35+0.65*x)（x² 加速）；side 加 FLUT 抖动；root 滞后 FLY_LAG
```
+ `torn_of()` 撕裂瞬间快照（与当前帧解耦，可预排光尾）。**移植点**：布料销毁、碎片飞散、披风破损。

### T10. CanvasGroup 运动模糊（wraith_veil.gdshader:24-60）
```
textureLod 读组缓冲 13 tap（十字0.86×4+斜0.62×8+中0.30×4），权重 0.16/0.115/0.0475；
k=smoothstep(near,far, 到 core_a-core_b 线段的距离); r=blur*k;
```
身上（core 线段附近）清晰、甩到远处模糊——**距离场加权而非全局模糊**。注意 gl_compatibility 渲染器要 transparent_bg/hdr2d 才生效（wraith_form.gd:852-857）。**移植点**：DyeFramework 高速残影/疾速层。

### T11. UI 带双份绘制协议（wraith_form.gd:698-746 / wraith_play.gd:1287-1315）
运行时给 shader 源码注入：
```
fragment 末尾: COLOR.rgb=mix(COLOR.rgb,wz_veil.rgb,wz_veil.a); COLOR.a=wz_a(COLOR.a);
vertex 头部:  wz_p=(MODEL_MATRIX*vec4(VERTEX,0,1)).xy;
wz_a(a)= a*w(顶层份额) 或 a*(1-w)/max(1-wz_outer*a*w,1e-4)（底层份额，含组透明校正）
```
同一物体两份（角色层/顶层）按屏幕 y/x 落在 UI 带（wz_y/wz_c 四向量定义）内外的比例分 alpha，和为 1。**移植点**：任何"特效既要盖 UI 又要留在角色层"的分层难题，这是全项目最巧的工程技巧。

### T12. 逆映射拉伸变形（soul.gdshader:23-37）
```
src(p): 绕 head_at 反向旋转 rot → 沿 sdir 拉伸 stretch（脖子细部放大）→ grow 头部放大 → pull 二分查找 y 映射（向下拽）
模糊: 13 tap 黄金角 r=blur*sqrt(k/13)（均匀圆盘采样）
```
**移植点**：抓取/拖拽/触手牵引变形；EIMG 的"无贴图时程序魂影（含眼窝）"也是省资产的思路。

### T13. 火花解析粒子（card_edge.gdshader:131-155）
```
at = start + dir*speed*age + vec2(0,260)*age²;  vel = dir*speed+vec2(0,520)*age;
back = at - normalize(vel)*(7+12*(1-u));        // 速度方向尾迹
k = (1-smoothstep(0.7,1.9, seg(p,at,back)))*(1-u*u);  // seg SDF 画线段
```
9 个火花全在 shader 里解析积分（无粒子系统）。**移植点**：卡牌强化/附魔/锻造火花；burst.gdshader:7-20 的角度分箱放射速度线（bins=150+hash）是配套。

### T14. 液滴/气泡程序动画（acid_tide.gdshader:69-87）
```
cell 网格(230×250)+hash 决定有无(0.80*bubbles)+period 2.4~5.8s 随机;
k=fract(turn)*period/0.95;  ring=bell((r-rad)/w)*(1-k)²;  dome=exp(-r²/(60+thin²))*(1-smoothstep(0,0.35,k));
```
**移植点**：UI 水滴、法力泡、沼泽冒泡；一个 plips() 函数全覆盖。

### T15. 分帧预热体系（ShaderWarmup.cs:52-225 + warm() 模式）
shader 逐个进 32×32 离屏视口渲染 2 帧 → 释放 → 下一个；.gd 侧 warm() 每次只读一张贴图/编译一个 shader（suppress_stage.gd:167-177）；WraithSceneWarmup 非战斗时预演整个场景。**移植点**：移植项目同样有"首次播放卡顿"问题，这套三件套（AssetWarmup 队列+视口逐管线+warm 分帧）可直接搬。

---

## ⑥ 【线索清单】给二探的逐行深挖点

| # | 深挖点 | 入口（文件:行号） | 待确认问题 |
|---|--------|------------------|-----------|
| 1 | acid_tide 地面几何的 C# 侧推算 | CorrosiveWaveFx.cs:269-290 | Floor 结构（Her/Vanish/Focal/Far）如何从战斗场景相机推出？Reach/Crest 曲线是什么形状？ |
| 2 | malaise_mist 的 puff 生成与 bead 联动 | MalaiseFx.cs:520-530, 736-746 | _at/_ax/_ex/_sx/_an/_ro 六数组何时填、珠子拖拽雾时怎么变？ |
| 3 | malaise_strand 的 seg/fl 填充 | MalaiseFx.cs（strand 段） | 32 个点如何从胸口/珠子坐标采样？layer 前后分层怎么切？ |
| 4 | shadow_cloud 的旋转/梳理动力学 | ShadowCloud.cs 全文（尤其 _run/_cb/spin/twist 计算） | 身体速度→comb 的映射；age 残留雾的淘汰策略。 |
| 5 | shadow_shock 冲击环生命周期 | ShadowStepFx.cs:717 附近 | radius/width/push/wake 时间曲线；lie 贴地压扁的来源。 |
| 6 | freeze 的 rings[8] 触发时机 | TimeFreeze.cs:2004 附近（RippleUniform） | 她每次行动 push 几个环？衰减公式？ |
| 7 | wraith_flame 的 power/lean 曲线 | WraithFigure.cs:930-932 | power 0→1.6 的点火/ feed/熄灭曲线；lean 与 Clock 的正弦关系。 |
| 8 | her.gdshader 的 smear 与 ghost_trails 配合 | suppress_stage.gd:580-597 | 拖影精灵（ghosts）与 shader 内 16 tap smear 的分工边界。 |
| 9 | soul.gdshader 的 etex 来源 | SuppressStage.cs set_soul 调用点 | 敌人纹理由 C# 哪条路径传来（Viewport 捕获还是贴图）？has_tex=0 的回退何时用？ |
| 10 | wraith_play/form 的 rig 数据格式 | wraith_form.gd:602-603, wraith_play.gd:964-967 | rig.json/rig2.json 的 strands/layers 字段全表；mk_game_play.py 生成管线（docs/ref/wraith_cut/）是否在仓库内。 |
| 11 | WZ_HEAD 份额合成的数值正确性 | wraith_form.gd:705-715 / wraith_play.gd:1274-1284 | `a*(1-w)/max(1-wz_outer*a*w,1e-4)` 的组透明校正推导；wz_gain 对 haze_ui 的用法。 |
| 12 | wraith_veil 的兼容性回退 | wraith_form.gd:852-857, wraith_play.gd:1440-1445 | gl_compatibility 下 transparent_bg/use_hdr_2d 的实测表现；GHOSTS 拖影（GWIDE/GALPHA）与 blur 的互斥逻辑。 |
| 13 | ShaderWarmup 帧同步细节 | ShaderWarmup.cs:127-135, 227-234 | _draws>=2 为何是 2 帧；UpdateMode=4→0 的切换时机是否真等到了 GPU。 |
| 14 | 游戏本体 Vfx 对照 | decomp/MegaCrit.Sts2.Core.Nodes.Vfx/（NBasicTrail/NBezierTrail/NAdditiveOverlayVfx） | 与 smear/rope/wraith_gauze 的原生实现差异——mod 是重写还是包装？ |
| 15 | Tuning 全量表 | tuning.ini | 所有 shader 可调参数默认值/范围（Tuning.F 调用点已定位 6 处，需补全）。 |
| 16 | suppress 的音频/震屏契约 | suppress_stage.gd:538, SuppressStage.cs step 调用点 | cues=["lift","bite","tear","burst"] 与 C# 音效映射；out_shake/out_hold/yank/dim 的消费方式。 |
| 17 | wraith_cardfire 的纹理来源 | WraithCardFire.cs:51 | "the card's own flame" 贴图从哪来（游戏资源提取？）green 曲线。 |
| 18 | 49 shader 的实际运行帧率/绘制批次 | 无（需运行时） | puff 40/48 循环 + fbm 4~5 层 + 16 tap 的实机成本；1080p 下 freeze/wail_warp 全屏读屏的填充率。 |

---

## 附：勘察方法与可信度

- 49 个 .gdshader 与 3 个 .gd 全部逐行通读（非抽样）；shader_type/render_mode/uniform/算法结论均以文件内文本为准。
- "服务对象"由 shader 头注释 + C# `ShaderLib.NewMaterial("xxx")` 调用点交叉确认（见①表括号内行号）。
- 无法从静态代码确认的（运行时性能、Tuning 全量、rig.json 内容）已移入⑥线索清单，标注【待二探确认】。
- 二轮协作补充（与 recon-arch 01_architecture.md 互证）：④.6 freeze↔游戏 Vfx 交互链已核验写入；反编译切分伪影（文件尾 [HarmonyPatch] 属性归属源码顺序下一类型）已规避——本报告所有 C# 引用均为方法体内调用点，未采信任何文件尾属性；HitSparkHoldPatch.cs:22 属性经 source lines 顺序确证属于 SceneVfxHoldPatch。
