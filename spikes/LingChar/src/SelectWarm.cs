using Godot;

namespace LingChar;

/// <summary>
/// 选人立绘预热 v6（2026-10-09）。
/// 迭代史（详见 光影层移植计划.md 条目 31-34）：
///   v1 CallDeferred 不 flush；v2 同步 AddChild 节点不激活；v3 ProcessFrame 事件
///   驱动状态机生效（tid=1 主线程），但每角色首次切换仍卡 76-160ms。
///   v4 Harmony 两刀（GetScene redirect + UnloadAssets 保护）——v5 打点证实
///   生效（redirect hit、keepAlive=valid、GetScene 0.01ms、SelectCharacter
///   全流程 3-22ms），C# 同步段无罪。
/// v6 定案：卡点在**帧末渲染提交**（用户感知：音效/UI 冻结在第一帧，
///   然后和模型一起瞬间出来）。根因是 entry_player.gd _ready() 里
///   `spr.visible = false`（正式进场时序的先隐藏逻辑）——预热实例的
///   SpineSprite 整棵被藏，所谓"透明画 2 帧"只画了 TextureRect；
///   spine 24 网格的 atlas 纹理 VRAM 上传 + spine/additive/multiply
///   shader 编译从未发生，首次真实切换一次付清 76-160ms。
/// v6 修复：
///   A) 预热实例 AddChild 后递归强制全部 CanvasItem Visible=true
///      （覆盖 EntryPlayer 的 _ready 隐藏），整棵 spine 树以 modulate
///      α=0 真实画帧 → 纹理上传/shader 编译/网格首建全部移到启动期；
///   B) warmup 完成后 ProcessFrame 订阅转为常驻帧冻结监视器：
///      相邻帧间隔 >40ms 打 [LingWarm] HITCH——此后任何卡顿都有直接证据。
/// v7 增强（2026-10-09，用户：疑似没有变化）：
///   ①v6 复测——warmup 帧后首次切换仍 silent 80ms/ling 181ms，且**重复切换
///     （process 仅 3-14ms）后下一帧照样 HITCH ~359ms**=每个新实例的首次
///     完整绘制帧都要付 ~350ms；v6 预热用 modulate α=0 可能被渲染器整项剔除
///     （draw call 根本没提交），spine 仍未被焐热。
///   ②预热改为 α=0.01 + ZIndex=-4096：钻到所有 UI 背后真实绘制（渲染器
///     无法按 α 剔除），屏幕上被主菜单/选人 UI 完全遮住。
///   ③HITCH 监视器升级为分段归因：订阅 RenderingServer.FramePostDraw，
///     慢帧拆两段——proc→postdraw（=节点逻辑+绘制提交）与
///     postdraw→proc（=present/vsync/输入/延迟删除）——350ms 在哪一段
///     一眼定位，决定下一步修绘制还是修销毁。
/// </summary>
public static class SelectWarm
{
    // silent bg 在 LingSilentSkin.pck，ling bg 在 LingChar.pck（两 pck 均已挂载后才会调用本类）
    internal static readonly string[] Paths =
    {
        "res://scenes/screens/char_select/char_select_bg_silent.tscn",
        "res://scenes/screens/char_select/char_select_bg_ling.tscn",
    };

    private static readonly PackedScene[] KeepAlive = new PackedScene[Paths.Length];
    internal static readonly System.Collections.Generic.HashSet<string> _redirectLogged = new();

    internal static PackedScene GetKeepAlive(string path)
    {
        var i = System.Array.IndexOf(Paths, path);
        return i < 0 ? null : KeepAlive[i];
    }

    internal static bool IsProtectedPath(string path) => System.Array.IndexOf(Paths, path) >= 0;

    public static void Warm()
    {
        try
        {
            if (Engine.GetMainLoop() is SceneTree tree)
            {
                _tree = tree;
                _subscribed = true;
                tree.ProcessFrame += OnProcessFrame;
                try
                {
                    RenderingServer.Singleton.FramePostDraw += OnFramePostDraw;
                    _postSubscribed = true;
                }
                catch (System.Exception ex)
                {
                    GD.Print("[LingWarm] FramePostDraw subscribe failed (分段归因不可用): ", ex.Message);
                }
                GD.Print($"[LingWarm] v7 subscribed ProcessFrame at tid={System.Environment.CurrentManagedThreadId}");
            }
            else
            {
                GD.Print("[LingWarm] main loop is not SceneTree, warmup skipped");
            }
        }
        catch (System.Exception ex)
        {
            GD.Print("[LingWarm] init failed (ignored): ", ex.Message);
        }
    }

    private static SceneTree _tree;
    private static Node _warmInstance;
    private static int _stage;      // 0:load0 1:draw0 2:load1 3:draw1 4:done(monitoring)
    private static int _drawFrames;
    private static bool _subscribed;
    private static bool _postSubscribed;
    private static ulong _lastTickUsec;
    private static ulong _lastPostDrawUsec;

    private static void OnFramePostDraw()
    {
        _lastPostDrawUsec = Time.GetTicksUsec();
    }

    private static void OnProcessFrame()
    {
        try
        {
            // 常驻帧冻结监视器：warmup 完成后仍保留订阅，>40ms 间隔即打点（分段归因）
            if (_stage >= 4)
            {
                var now = Time.GetTicksUsec();
                if (_lastTickUsec != 0)
                {
                    double gap = (now - _lastTickUsec) / 1000.0;
                    if (gap > 40.0)
                    {
                        if (_postSubscribed && _lastPostDrawUsec >= _lastTickUsec)
                        {
                            double drawSpan = (_lastPostDrawUsec - _lastTickUsec) / 1000.0;   // 上一帧 proc 开始→postdraw（逻辑+绘制提交）
                            double tailSpan = (now - _lastPostDrawUsec) / 1000.0;             // 上一帧 postdraw→本帧 proc（present/vsync/输入/延迟删除）
                            GD.Print($"[LingWarm] HITCH {gap:F0}ms before frame={Engine.GetProcessFrames()} (proc→postdraw {drawSpan:F0}ms | postdraw→proc {tailSpan:F0}ms)");
                        }
                        else
                        {
                            GD.Print($"[LingWarm] HITCH {gap:F0}ms before frame={Engine.GetProcessFrames()}");
                        }
                    }
                }
                _lastTickUsec = now;
                return;
            }

            if (_stage == 0)
                GD.Print($"[LingWarm] first ProcessFrame callback at tid={System.Environment.CurrentManagedThreadId}");
            Step();
        }
        catch (System.Exception ex)
        {
            GD.Print("[LingWarm] step EXC: ", ex.Message);
            Unsubscribe();
        }
    }

    private static void Step()
    {
        if (_stage >= 4)
            return;

        bool loadPhase = (_stage == 0 || _stage == 2);
        if (loadPhase)
        {
            int idx = _stage / 2;
            var path = Paths[idx];
            try
            {
                var packed = ResourceLoader.Load<PackedScene>(path);
                if (packed == null)
                {
                    GD.Print("[LingWarm] load NULL: ", path);
                }
                else
                {
                    KeepAlive[idx] = packed; // 静态保活（配合 GetScene redirect + UnloadAssets 保护）
                    GD.Print("[LingWarm] loaded: ", path);
                    _warmInstance = packed.Instantiate();
                    if (_warmInstance is CanvasItem ci)
                    {
                        ci.Modulate = new Color(1f, 1f, 1f, 0.01f); // α=0.01：防止渲染器按全透明剔除（α=0 可能整项不画）
                        ci.ZIndex = -4096;                          // 钻到同画布所有 UI 背后，视觉不可见但真实绘制
                    }
                    _tree.Root.AddChild(_warmInstance);
                    // v6 核心：entry_player.gd _ready() 会 spr.visible=false（正式进场时序），
                    // 必须在其后递归强制可见，spine 树才会真实绘制（纹理上传+shader 编译）
                    ForceVisibleRecursive(_warmInstance);
                    GD.Print("[LingWarm] warmup instance on tree (spine forced visible): ", path);
                }
            }
            catch (System.Exception ex)
            {
                GD.Print("[LingWarm] load EXC: ", path, " -> ", ex.Message);
            }
            _drawFrames = 0;
            _stage++;
            return;
        }

        // 画帧阶段：实例以 α≈0 画 2 帧焐热渲染管线（实例可能为 null=该项加载失败，直接跳过）
        if (_warmInstance != null)
        {
            _drawFrames++;
            if (_drawFrames < 2) return;
            // v8：不销毁——预热实例（已付首绘成本）入复用池，首次真实切换直接换入
            int idx = _stage / 2;
            string seedName = idx == 0 ? "SILENT_bg" : "LING_bg";
            _warmInstance.Name = seedName;
            // v8.1 修复（主菜单不可点击）：bg 根是全屏 Control（MouseFilter 默认 Stop），
            // 常驻树中会吞掉整个主菜单的鼠标输入——入池后立即脱离场景树：
            // 不处理输入/不绘制/不 process，节点活着则 GPU 资源保留，复用不受影响
            _warmInstance.GetParent()?.RemoveChild(_warmInstance);
            BgInstancePool.Put(seedName, _warmInstance);
            GD.Print($"[LingWarm] warmup instance seeded into reuse pool: {seedName}");
            _warmInstance = null;
            GD.Print("[LingWarm] warmup done: stage ", _stage);
        }
        _stage++;
        if (_stage >= 4)
        {
            GD.Print("[LingWarm] warmup complete (GPU warmed), monitoring frame hitches >40ms");
            _lastTickUsec = 0; // 从下一帧开始计间隔，排除 warmup 自身的大帧
        }
    }

    private static void ForceVisibleRecursive(Node n)
    {
        if (n is CanvasItem c)
            c.Visible = true;
        foreach (var child in n.GetChildren())
            ForceVisibleRecursive(child);
    }

    private static void Unsubscribe()
    {
        if (!_subscribed) return;
        _subscribed = false;
        if (_tree != null)
            _tree.ProcessFrame -= OnProcessFrame;
        if (_postSubscribed)
        {
            try { RenderingServer.Singleton.FramePostDraw -= OnFramePostDraw; } catch { }
            _postSubscribed = false;
        }
    }
}
