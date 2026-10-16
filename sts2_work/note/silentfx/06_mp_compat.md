# SilentFX 二探报告：MP 同步与兼容降级体系（deep-mp）

> 勘察对象：SilentFX v0.72.7（杀戮尖塔2 mod，作者奶酪）的多人联机特效同步体系 + 游戏版本兼容降级体系
> 材料：`silentfx_decomp/split/` 反编译切分源码（证据格式 `split文件名:行号`，行号为切分文件内行号）、`silentfx.log` 运行日志、初探报告 `01_architecture.md` 第⑤⑨节
> 日志基线（2026-10-16 00:44:08 启动）：`[MP] Exact targeting unavailable; using native peer input: game handshake version metadata is absent`；`Game capabilities: Shiv creation args=3, Poison trigger=False, Concoct=False, Expertise retain FX=False`
> ⚠️ 切分伪影声明：split 文件尾部的 `[HarmonyPatch]` 属性归属源码顺序上的下一个类型，本报告已逐条用 Prefix/Postfix 签名校验归属（见 ⑤.6 补丁归属表）

---

## 结论先行（TL;DR）

1. **MP 特效同步是四层瀑布**：能力协商（反射探测 19 项握手元数据，任缺即整体关闭）→ 消息表注入（把自定义 `AimMessage` 反射注册进游戏私有 `MessageTypes` 缓存）→ 主机权威租约协议（Offer/Ack/Go/ Snapshot 四消息，20Hz 快照，主机中继并改写序列号）→ 本地渲染代理（AsyncLocal 上下文 + modulate 淡出 + 快照查询）。每一层都有独立的 Warn 降级文案，最终形态退化为「游戏原生 peer input」或「无远端 FX」，**永不通向崩溃**。
2. **最稀缺技术是 NormalizeTypes**（AimCompatibility.cs:240-330）：不依赖游戏任何支持，反射拿 `MessageTypes._cache._idToType/_typeToId` 私有表，先按 list 顺序重建 ID 映射、剔除旧注册、防「合并 distinct IDs」、查尾部 ID 占用与 255 容量，全部通过才把 `AimMessage` 追加到表尾。配套全消息表 SHA256 指纹插进握手 `otherMods` 做双向验签——**对方消息表不一致就不启用精确同步**。
3. **本机日志显示 MP 精确层完全关闭**：游戏构建里 `PeerVersionInfo` 类型不存在（`game handshake version metadata is absent`，AimCompatibility.cs:88-91 抛出），整个 Exact 链路未激活，走的是 RemoteHeldFx 的原生输入降级路径。也就是说：**这套协议在本 mod 发行的目标游戏版本上从未真正跑过，是面向更新版游戏的预备队**。
4. **对初探报告的一处修正**：RemoteFxFocus 的 0.5 淡出因子跟踪的是**本地玩家自己**的卡牌租约（所有入口被 `Trackable=IsLocal` 把守，Begin/LetGo 由本地输入补丁驱动），本地玩家把卡拖出/打出时，把**远端玩家**的 FX 淡出到 50%。初探报告「有任何远端卡正在出牌则 factor=0.5」的表述与代码相反，以代码为准（详见 ④.3）。

---

## ① MP 四层协议全景

### 1.1 层图与总开关

```
第1层 能力协商   AimCompatibility        静态探测一次，决定 ExactAvailable
   ↓ 通过
第2层 消息表注入 NormalizeTypes          把 AimMessage 塞进游戏 MessageTypes 私有表
   ↓ 成功（_hasAimType）
第3层 租约+快照   AimSync                Offer→Ack→Go 握手，3s 租约，20Hz Snapshot，主机中继
   ↓ 租约内（Exact）
第4层 渲染代理    AimLocalTarget/RemoteFxFade/RemoteFxFocus/RemoteShivCreation/RemoteHeldFx
```

总开关链：`AimCompatibility.ExactAvailable`（反射探测）∧ `AimSync.Exact`（租约）∧ `MultiplayerFx.Active`（tuning `mp.remote_fx≠0` ∧ 战斗中玩家数>1，MultiplayerFx.cs:19-47）∧ `PlayerSettings.Enabled`（用户设置快照模式）。

### 1.2 AimMessage 消息格式（AimMessage.cs:1-101）

`INetMessage + IPacketSerializable`，四种 Kind（:6-12）：`Offer=1 / Ack=2 / Go=3 / Snapshot=4`。

线格式（Serialize :52-76，位宽精确）：

| 字段 | 位宽 | 说明 |
|---|---|---|
| Kind | 8 | 1/2/3/4 |
| Seed | 64 | RunRngSet.Seed，局内随机种子（上下文键） |
| Room | 32 | AbstractRoom.Id（上下文键） |
| Act | 32 | CurrentActIndex（上下文键） |
| Roster | 64 | 玩家名单 SHA256 前 8 字节，防冒名/串房（上下文键） |
| Epoch | 64 | 主机生成的协议代 nonce |
| Round | 64 | 握手轮次（主机每轮 +1） |
| Generation | 64 | 每客户端代 nonce（主机按 sender 记录） |
| Sequence | 64 | 快照序号，只认单调递增 |
| Source | 64 | 发送者 NetId |
| Active / HasCard / InPlayZone | 1×3 | 快照状态位 |
| Card | 变长 | HasCard 时序列化 `NetCombatCard`（游戏类型，含 CombatCardIndex） |
| Target | 32 | 有值时写 `CombatId`（uint?） |

传输属性（:44-50）：`ShouldBroadcast=false`、`ShouldBuffer=false`——**不借游戏广播/迟加入缓冲**，mod 自己精确单播与中继（20Hz 状态流广播浪费带宽、缓冲无意义）；`Mode=(NetTransferMode)2`、`LogLevel=(LogLevel)2`（枚举名待确认，按 20Hz 快照语义推断为不可靠传输+低日志级别，避免刷屏）。

### 1.3 时序图（文字版）

```
 Host                                    Client A / B
  │ 每 1s: _round++                        │
  ├─ Offer(1) ──单播每个 peer──────────────▶│ 校验 Current()+Epoch；采纳 host Epoch
  │                                        │ Round≥_round → _round=Round
  │◀──────── Ack(2)（带本端 Generation）────┤
  │ 全员 Ready[].Round==_round 时:          │
  │ _lease = Now+3s                         │
  ├─ Go(3) ──按人带 Generation─────────────▶│ _lease = Now+3s；_hasSent=false
  │                                         │
  │◀──────── Snapshot(4) 20Hz（本地持卡状态）┤
  │ ValidSnapshot 校验 → Sequence 改写为     │
  │ ++_relaySequence → 中继其他 peer ──────▶│ Sequence 只认递增，去重
  │                                         │
  └─ 3s 内任一成功握手轮续期；超时 Exact=false，回退原生输入
```

- 主机自己的 Snapshot 也走 `++_relaySequence` + `SendPeers`（AimSync.cs:471-476）——**主机是全场序列号权威**，客户端发来的 Sequence 仅用于「比已见大」去重（:330/:368）。
- 租约 renewal：主机每 1s 发起一轮 Offer，只要该轮全员 Ack 到达就续 3s；1s  offer 间隔 vs 3s 租约 = 容忍连续两轮失败。

---

## ② NormalizeTypes 消息表注入算法逐行（AimCompatibility.cs:240-330）

**目的**：游戏的 `MessageTypes` 用私有静态 `_cache`（含 `_idToType: List<Type>` 与 `_typeToId: Dictionary<Type,int>`）维护网络消息类型表。mod 要让游戏网络栈认识 `AimMessage`，必须把自己加进这张表。逐行：

```csharp
_normalized = false; _hasAimType = false;                    // :243-244 先置否，异常即保持
object obj = MessageTypes._cache(静态私有)?.GetValue(null);   // :247 反射取私有静态缓存
if (obj == null) return false;                                // :248-251 表不在即失败
FieldInfo _idToType / _typeToId = obj类型的两私有字段;          // :253-254
if (_idToType.GetValue(obj) is List<Type> list)               // :255 表必须是 List<Type>
{
  Dictionary<Type,int> map = _typeToId.GetValue(obj) as ...;  // :258 强转，失败走 :323 return false
  // ── 步骤1：按 list 顺序重建「list 即真相」的映射 :261-265
  dictionary = { list[i] → i };                               // :261-265 下标即 ID
  // ── 步骤2：剔除既有 AimMessage 注册，保证幂等 :266
  list2 = list.Where(t => t != typeof(AimMessage)).ToList();  // :266
  dictionary2 = { list2[i] → i };                             // :267-271 压缩重编号
  // ── 步骤3：回填 map 里有但 list 里没有的条目（别的 mod 可能只写了 dict）:272-278
  foreach (item in map)
    if (item.Key != AimMessage && (!dictionary 有该 Key || item.Value != 旧值))
      dictionary2[item.Key] = item.Value;
  // ── 步骤4：防「合并 distinct IDs」:279-289
  dictionary3 = {};                                           // 新ID → 旧ID
  foreach (item2 in dictionary2) {
    旧ID = map 里有 ? map值 : dictionary值;                     // :282
    if (dictionary3 已占该新ID 且 旧ID不同)                     // :283 两个类型将被压成同一 ID
      { Warn("would merge distinct message IDs"); return false; } // :285-286 宁可放弃
    dictionary3[新ID] = 旧ID;                                  // :288
  }
  // ── 步骤5：注册门槛 :290-296
  flag  = dictionary2.Values.Contains(list2.Count);            // :290 尾部 ID 已被别人占用
  flag2 = ExactAvailable && list2.Count < 255 && !flag;        // :291 容量 255 + 无冲突 + 能力在
  if (flag2) { dictionary2[AimMessage] = list2.Count;          // :294 追加到表尾
               list2.Add(typeof(AimMessage)); }                // :295
  // ── 步骤6：脏检查后写回私有字段 :297-306
  if (list 变了)  _idToType.SetValue(obj, list2);               // :301
  if (map  变了)  _typeToId.SetValue(obj, dictionary2);         // :305
  // ── 步骤7：指纹缓存失效 :307-312（表对象或内容变了就重算 SHA256）
  _normalized = true; _normalizedCache = obj; _hasAimType = flag2; // :313-315
  if (ExactAvailable && !flag2) Warn(占用 ? "ID is reserved" : "table is full"); // :316-319
  return flag2;
}
catch (Exception ex) { Warn("normalization unavailable: " + ex.Message); return false; } // :325-329
```

**五个设计决策**：

1. **幂等**：每次调用先剔除旧 `AimMessage` 再重排（:266），重复调用（`MessageTypes.Initialize` 的 Postfix + 每次 `Advertise`）结果稳定。
2. **list 为真相源**：ID 映射按 list 下标重建（:261-265），不信任 dict 的既有值；再把 dict 里多出的条目回填（:272-278）——防御其他 mod 绕过 list 直接写 dict。
3. **防合并**（:279-289）：压缩重编号后若两个 distinct 类型会共享同一新 ID 且旧 ID 不同，**整体放弃注册**而非挑一个覆盖。这是全算法最保守的一步，对应 Warn 文案 `"[MP] Aim message normalization was skipped because it would merge distinct message IDs."`。
4. **尾部 ID 占用检查**（:290）：`list2.Count` 正是打算分配给 `AimMessage` 的 ID；若任何类型已占用它（例如别的 mod 从 list 删了类型但 dict 没清），放弃并 Warn `"its ID is reserved by another message mapping"`。
5. **255 容量**（:291）：`list2.Count < 255`。消息 ID 走 8-bit 线格式（Kind 也是 `WriteByte(8)`），255 疑为保留哨兵值（待确认，无游戏侧佐证）。

### 2.1 指纹与广告验签

- `Fingerprint()`（:506-538）：前置条件 `_normalized ∧ _hasAimType ∧ 同一缓存对象 ∧ AimMessage 恰在表尾且独占该 ID`（:522 还反向检查没有第二个类型占用该 ID）→ 规范化文本 = 每个类型按 list 序一行 `L|i|FullName|AssemblyFullName` + dict 按（类型全名 Ordinal、程序集全名、ID）排序的 `M|id|FullName|AssemblyFullName` 行（:526-528）→ `SHA256("SilentFX.Targeting/4:" + 文本)` 小写 hex（:529）。带缓存与 `SameMappings` 校验（:517/:540-555）。
- `Marker = "SilentFX.Targeting/4:" + Fingerprint()`（:226）；指纹不可用时为 `"...:unavailable"`。
- `Advertise(ref object info)`（:332-361）：`PeerVersionInfo.LocalDefault` 的 Postfix（AimVersionPatch）里调用——把本地版本信息的 `otherMods` 拷贝一份、剔除所有历史 `SilentFX.Targeting/` 前缀条目、按条件插当前 Marker、写回（:341-355）。条件：`NormalizeTypes() 成功 ∧ Tuning.F("mp.remote_fx",1f)≠0 ∧ marker 非 unavailable`（:347-353）。
- `Matches(remote, local)`（:467-504）：**双向兰梦信物**——双方 `otherMods` 都含同一 Marker（:490）∧ `version` 相等 ∧ `branch` 相等 ∧ `idDatabaseHash` 相等 ∧ `gameplayAffectingMods` 排序后全等 ∧ `otherMods` 排序后全等（:494）。即：同游戏构建 + 同玩法 mod 组合 + 同 SilentFX 协议版 + **同消息表指纹**。注意 `otherMods` 也要求全等——一边多装任何一个声明进 otherMods 的无关 mod，双方精确同步即关闭（保守设计，是否有意为之待确认）。
- Host 侧附加校验：`PeerMatches`（逐个 peer 的 versionInfo vs 主机 LocalVersion，:392-406）、`TryGetPeers`（ConnectedPeers → PeerInfo[]，含 `readyForBroadcasting`，:408-437）、`HostMatches`（客户端侧：当前 HostNetId == 握手时记录的 host ∧ Matches，:375-390）。

---

## ③ AimSync 状态机（AimSync.cs:1-593）

### 3.1 状态清单

| 状态 | 行号 | 含义 |
|---|---|---|
| `Ready<netId,Peer{Round,Generation,Sequence}>` | :15 | 主机侧每客户端握手进度 |
| `Attached` (ConditionalWeakTable) | :17 | 每 service 只注册一次消息处理器 |
| `Targets<netId,AimMessage>` / `SeenAt<netId,double>` | :19-21 | 各发送者最新快照与接收时刻 |
| 上下文键 `_service/_room/_players/_seed/_roster/_roomId/_act` | :23-45 | 任一变化即 Reset(clearLocal:false) 并重绑（:159-170） |
| `_epoch/_round/_generation` | :35-37 | 协议代/握手轮/客户端代，全 nonce |
| `_sequence/_relaySequence` | :39-41 | 客户端自发序号/主机中继改写序号 |
| `_lease/_nextOffer/_nextSample/_refresh` | :47-53 | 租约截止/下次 Offer/下次采样/强制刷新 |
| `_sent/_hasSent` | :55-57 | 上次发送内容（松手时靠它做 carry-over） |

### 3.2 Update 门（:102-210）——每道门失败都有明确动作

`ExactAvailable` 假 → Reset；NetService 非 Host/Client → Reset；`Attach` 失败 → Reset；`MultiplayerFx.Active` 假或 CombatRoom 死 → Reset；未连接/无房间 ID → Reset；`TryGetSeed` 失败 → Reset；玩家 <2 或不含自己 → Reset（:108-158）。**上下文键变化**（service/room/players 排序名单/seed/roomId/act）→ `Reset(clearLocal:false)` + 重算 `_roster = SHA256(排序名单逗号连接) 前 8 字节` + `_generation = Nonce()`（:159-170）。

主机分支（:171-190）：`RoomMatches` 失败 → `ClearProtocol`；`_epoch==0` 时生成；`Now>=_nextOffer` → `_nextOffer=Now+1; _round++; SendPeers(Offer)`。
客户端分支（:191-195）：`HostMatches` 失败 → `ClearProtocol`。
`!Exact → _hasSent=false`；`Exact → Sample()`（:196-203）。
整体 try-catch → Reset + `"[MP] Exact targeting fell back to peer input: "`（:205-209）。

`RoomMatches`（:218-236）：peers 数 == 玩家数-1 ∧ 每个 peer 不是 host ∧ 在 `_players` 内 ∧ 不重复 ∧ `ReadyForBroadcasting` ∧ `PeerMatches`。

### 3.3 Receive（:281-378）——主机中继 + 反作弊校验

公共门：`Current(message)`（:254-279：service/room/房间ID/seed/act/玩家名单/上下文全匹配 ∧ `message.Roster==_roster` ∧ Seed/Room/Act 相等）∧ `Epoch≠0`。

**主机路径**（:291-338）：
- 附加门：`RoomMatches ∧ sender≠host ∧ sender∈players ∧ message.Source==sender ∧ message.Epoch==_epoch`（:293）。
- `Ack(2)` 且 `Round==_round ∧ Generation≠0`（:298）：刷新 `Ready[sender]`；**代变化则清掉该 sender 的 Targets/SeenAt**（:300-311，防旧代快照残留）；全员 `Ready.Round==_round` → `_lease=Now+3`，按人回发 `Go(3)` 各带其 Generation（:313-327）。
- `Snapshot(4)` 且 `Exact ∧ Ready[sender].Generation==message.Generation ∧ Sequence>已见 ∧ ValidSnapshot`（:330）：记录 Sequence → **`message.Sequence = ++_relaySequence`（改写！:333）** → 存 Targets/SeenAt → `SendPeers(message, except:sender)` 中继（:334-336）。

**客户端路径**（:340-372）：
- 门：`sender==HostNetId ∧ HostMatches`（:342）。
- `Offer(1)`（:347-361）：Epoch 不同先 `ClearProtocol` 再采纳；`Round>=_round` 则同步轮次并回 `Ack(2)`。
- `Go(3)`（:363-367）：Epoch/Round/Generation 全匹配 → `_lease=Now+3`。
- `Snapshot(4)`（:368-372）：`Exact ∧ Epoch 匹配 ∧ Source≠自己 ∧ Source∈players ∧ Sequence>已见 ∧ ValidSnapshot` → 存表。
- catch → `"[MP] Ignored targeting packet: "`（:374-377）。

### 3.4 ValidSnapshot（:380-424）——不信任任何远端声明

| # | 条件 | 判定 |
|---|---|---|
| 1 | `!Active && InPlayZone` | false（非激活却声明在出牌区）:384-387 |
| 2 | `!Active && !HasCard` | 仅当无 Target 才 true（空闲态）:388-391 |
| 3 | `!HasCard` | false（激活必须带卡）:392-395 |
| 4 | `!Active`（带卡释放态） | 必须无 Target ∧ 卡 Owner.NetId == Source（证明释放的是自己的卡）:397-413 |
| 5 | `Active` | 卡 Owner.NetId == Source ∧ 卡在 `Pile.Type==2`（手牌堆，枚举名待确认）∧ Owner.Creature 存活 :415-423 |

即便客户端被篡改，也只能声明「自己持有的、在手牌堆的、活着的」卡——**用本地可验证事实锚定远端输入**。

### 3.5 Sample（:426-485）——20Hz 快照

- 节流 `_nextSample = now+0.05`（:440）。
- 读 `AimLocalTarget.Read()`；持卡 ∧ 手牌堆 ∧ `IsLocal` → Active=true + InPlayZone + Card + Target（:443-458）。
- **否则 carry-over**：`_sent.HasCard` 时发 `InPlayZone=false, HasCard=true, Card=_sent.Card`（:461-466）——松手不发空包，让远端识别「刚才那张卡的待命特效该收了」，避免误判为「从未持卡」。
- 发送条件（:469）：`!_hasSent ∨ now>=_refresh(1s) ∨ Active/InPlayZone/HasCard/CombatCardIndex/Target 任一变化`。
- 序号（:471-476）：`Sequence=++_sequence`；主机改用 `++_relaySequence` 并 `SendPeers`；客户端单播 host。`_refresh=now+1`（:483）。
- 计时全用 `Now = Stopwatch 墙钟`（:59）——**子弹时间（TimeScale→0）下协议层不受影响**。

### 3.6 消费端 TryReadHeld/Resolve（:506-550）

`Exact ∧ Targets 有 ∧ Now-SeenAt<3s ∧ Source==player ∧ Current ∧ ValidSnapshot`（:517）；`card = HasCard ? ToCardModelOrNull : null`；`!Active → return false`（但 card 已带出，RemoteHeldFx.cs:156-159 用它做「已释放」清理由）；Target 按 CombatId 在 Owner 的敌人里匹配活体（:527-537）。

### 3.7 清理语义

`ClearProtocol()`（:563-580）：`_service==null → _generation=0`；否则代未定期换新 nonce；清三表；epoch/round/sequence/relaySequence 归零；租约与定时器归零。`Reset(clearLocal)`（:582-592）= 清上下文 + ClearProtocol + `AimLocalTarget.Reset()`。

---

## ④ 渲染代理四件套（+ 两个支撑）

### 4.1 AimLocalTarget——本地瞄准会话（SilentFX.Core.AimLocalTarget.cs）

- `Session{Play, Card, Target, Room}`（:28-37），`Read()` 返回不可变 `Snapshot{Card,Target,IsHeld,IsTargeting,InPlayZone}`（:6-26）。
- `Begin`（:119-142）：仅当 `IsLocal(card)` 才建会话（本地卡）。`SetTarget/ClearTarget/End`（:144-169）：hover/unhover/cleanup 驱动。
- `Read()` 的全部存活校验（:41-108）：play 实例有效且在树内（`IsLive` :176-186）∧ 卡在手牌堆 ∧ Owner 存活 ∧ 房间匹配 ∧ `NTargetManager.Instance.IsInSelection`（**选牌界面中不算 targeting**，:70-75，try-catch 包裹）∧ 目标节点存在（`GetCreatureNode(val)==null → null`，:81-84）∧ InPlayZone 由 `HeldHooks.OverPlayArea`（鼠标）或 `NControllerCardPlay`（手柄）判定（:91-101）。

### 4.2 RemoteFxFade——远端 VFX 整树 modulate 淡出（SilentFX.Core.RemoteFxFade.cs）

- `Capture(bool remote, params Node[] parents)`（:112-120）返回 `IDisposable` 作用域：`_scope` 栈为空 ∧ 非 remote → 返回 `Empty` 空对象（零开销）；否则 `new Scope(remote & flag, parents)`。
- Scope 构造（:27-55）：压栈；remote 时 `SyncRoom()` + 给每个存活父节点挂 `ChildEnteredTree += Entered`。**作用域内新建的任何子节点都会被登记为淡出 root**（:57-64/:122-156）——这是「远端卡打出的所有后续 VFX 自动纳入淡出」的机制，去重靠祖先检查（`Ancestor` :158-173，上溯上限 1024）与 `MaxRoots=2048`（:90/:147）。
- `BeforeDraw`（:185-276，挂在 `RenderingServer.FramePreDraw` 晚 pass，Ticker.cs:103/110）：先 `Restore` 上一帧的写 → `SyncRoom` → 非激活则 Reset → 清死 root → `factor = RemoteFxFocus.Factor`，`factor==1 或无 root` 直接返回 → 从各 root 迭代走子树（显式栈，`MaxWalk=16384`，:230）→ 对 `CanvasItem`：`(!branch.Inherits || val.TopLevel) && Seen.Add(val)` 时写 `Modulate.A *= factor`（:245-252）。
- **Inherits/TopLevel 语义**（关键细节）：Godot 的 modulate 沿树乘法传播，所以只需写「最顶端 CanvasItem」；`branch.Inherits` 表示祖先已被写过，其非 TopLevel 子孙跳过——**最少写入次数 + 正确语义**（TopLevel 节点不吃父级 modulate，必须单独写）。
- `Restore`（:295-314，挂 `RenderingServer.FramePostDraw`，懒挂钩 :150-154）：逐条 `Modulate==Applied` 才还原（防止撤销期间游戏方的修改）；异常整体 `Restore` + Warn（:265-269）。

### 4.3 RemoteFxFocus——淡出强度租约（SilentFX.Core.RemoteFxFocus.cs）★修正初探结论

**代码事实**：`Trackable(card) = MultiplayerFx.IsLocal(card)`（:298-305）把守全部四个入口；`Begin/LetGo` 由 `HeldFxManager.Begin/End` 调用（HeldFxManager.cs:165/257），其驱动是本地输入补丁（MouseCardPickedUpPatch/ControllerCardPickedUpPatch → HeldHooks.Begin → HeldFxManager.Begin，HeldHooks.cs:91）；`SampleCarried` 同步的 `HeldFxManager.Carried` 也是本地手持卡。**结论：Live 里只有本地玩家自己的卡牌租约**。

- `Factor`（:37-62）：任一本地方租约 `Out ∨ Queued ∨ Playing` → **0.5**，否则 1.0（不写 modulate）。
- 即：**本地玩家把手卡拖进出牌区/打出/播放动画中时，远端玩家的 FX 整体淡出到 50%**（聚焦本地动作）；本地空闲时远端 FX 全亮度。初探报告「有任何远端卡正在出牌则 factor=0.5」表述相反，作废。
- 租约生命周期：`Begin`（:64-93，Held）→ `LetGo`（:95-124，played→Queued，否则移除）→ `BeforeCardPlayed`（:126-182，Hook.BeforeCardPlayed，按 Invocation/PlayIndex 续期或新建 → Playing，记录 Index/Count）→ `AfterCardPlayed`（:184-215，最后一张移除）。匹配键：`Invocation`（AsyncLocal 令牌）或（Invocation 均为空 ∧ `PlayIndex==Index+1 ∧ PlayCount`）。
- `Update(realDt)`（:217-253）：暂停/TimeScale≤0.001 直接返回（不累计）；非 Held 且非 carried 的租约 `Idle+=clamp(dt,0,0.25)`，`Idle>=300s`（`AbandonedSeconds` :35）移除——**AfterCardPlayed 因异常缺席时的兜底回收**。
- `CheckEnvironment`（:277-296）：Active ∧ Enabled ∧ CombatRoom 活；房间切换 Reset。

### 4.4 RemoteShivCreation——远端飞镖可见数补偿（SilentFX.Core.RemoteShivCreation.cs）

- `Frames`（ConditionalWeakTable<invocation, Frame>，:56）：key 是 `MultiplayerFx.CurrentInvocation`（AsyncLocal 令牌），Frame 记录 Room/Play/Card/Owner/Target（:58-85）；`TryGet` 校验 invocation+card+owner+room 四重匹配（:87-96）。
- `Batch`（:6-30）：按**收件人 Player**分批（Frame.For :46-53）；`Add(count)` 返回起始下标；`ShowCloak(count,start,expected,out slot)`（:19-29）：`start==0` 时显示数 = `clamp(max(count,expected),1,4)`（首批最多露 4 张，如 Cloak And Dagger 的 3 张），后续只显示增量。
- 用途（ShivFx.cs:362-472 `OnRemoteShivsCreated`）：飞镖生成事件在所有客户端上都触发，但**每个玩家视野里应看到的张数不同**（自己的牌只露第一批、后续隐藏）。ShivFx 按 invocation 找到 Frame → 按 owner 取 Batch → `ShowCloak`/`TakeHeld` 计算差值 → 只补渲染「该看见而没看见」的部分（:454-470），且整段包在 `RemoteFxFade.Capture(!IsLocal(currentCard), ...)` 里（:443）。

### 4.5 支撑一：MultiplayerFx 的 AsyncLocal 上下文（SilentFX.Core.MultiplayerFx.cs）

- `AsyncLocal<CardModel> Source`（:13）+ `AsyncLocal<object> Invocation`（:15）：`Enter/Leave`（:77-99）保存-设置-还原。
- 注入点：`MultiplayerCardScopePatch` 给 `CardModel.OnPlayWrapper` 打 Prefix/Finalizer（补丁属性在 KnifeTrapAutoPlayScopePatch.cs:31，切分伪影），**每一次出牌（本地或远端模拟执行的）都在「当前卡 + 新 invocation 令牌」作用域内运行**（MultiplayerCardScopePatch.cs:13-33）。
- 消费者：`ShivFx.NoteCardPlayed`（CurrentCard==val ∧ 造飞镖牌 → RemoteShivCreation.Begin(CurrentInvocation,…)，ShivFx.cs:113-115）、`OnRemoteShivsCreated`（TryGet(CurrentInvocation,…)，:375）、`RemoteFxFocus.BeforeCardPlayed/AfterCardPlayed`（Invocation 匹配租约）。
- `IsLocal`（:53-75）：走游戏 `LocalContext.IsMine/IsMe`，catch→false（**fail-closed**：异常时按远端处理）。

### 4.6 支撑二：RemoteHeldFx——降级路径与渲染入口（SilentFX.Cards.RemoteHeldFx.cs）

- `Update()`（:89-288，由 HeldFxManager.Update ← Ticker 主 pass 驱动，HeldFxManager.cs:143/579）：首行 `AimSync.Update()`（:112）；`exact=AimSync.Exact`（:130）。
- **exact 路径**：`TryReadHeld(netId,…)`（:154）拿远端持卡与目标 → 建/续 `HeldSession`（CardFxContext.Remote=true，:444-458）→ 每帧更新 AimPosition/Feet/Chest/Body/Enemies 与 Armed（:241-251）。
- **降级路径**（:164-222，日志文案 `"[MP] Exact targeting unavailable; using native peer input"` 的落点）：反射绑 `PeerInputSynchronizer.GetStateForPlayer(ulong)` + 其返回类型的 `isTargeting` 字段（`Ready()` :31-58，失败即 `_readUnavailable` + Warn `"Remote input API unavailable"`）→ `HoveredModelTracker.GetHoveredModel(netId)` 取卡（:183）→ `NRemoteMouseCursorContainer.GetCursorPosition(netId)` 取光标 → **hitbox 命中测试猜目标**（`Target()` :420-442）。反射失败则 Drop，无远端 FX。
- `Claim/PlayingFor/Finish`（:479-582）：远端卡真正打出时（CardFxManager.cs:267 分流），领取/接管 HeldSession，含造飞镖牌/毒牌等特例分支。
- `TryObserve`（:296-418）：Power 特效侧查询入口（AccuracyFx/BurstFx/MasterPlannerFx/StrangleGripFx 调用），同样双路径。

---

## ⑤ 能力探测与签名垫片

### 5.1 AimCompatibility.Capabilities.Detect（AimCompatibility.cs:79-151）

静态构造一次（:158）。**3 个类型存在性门**（:85-91）：`PeerVersionInfo`、`NetClientData`、`HandshakeManager`（金丝雀，只查存在、后续不用），任一缺失抛 `"game handshake version metadata is absent"`——**本机日志正是死在这里**，说明安装版游戏无此 API，Exact 全链从未激活。

**19 项成员校验**（全部带类型校验，非仅存在性）：

| # | 成员 | 要求 | 行号 |
|---|---|---|---|
| 1-5 | PeerVersionInfo 的 version(str)/branch/idDatabaseHash(uint)/gameplayAffectingMods(List\<str\>)/otherMods(List\<str\>) | RequiredField | :92-96 |
| 6-8 | NetClientData 的 versionInfo(PeerVersionInfo)/peerId(ulong)/readyForBroadcasting(bool) | RequiredField | :97-99 |
| 9-10 | NetClientGameService/NetHostGameService 的 LocalVersion 属性 | RequiredProperty（无参 getter） | :100-101 |
| 11 | NetHostGameService.ConnectedPeers | IEnumerable ∧ 无参 getter | :102-106 |
| 12 | RunRngSet.Seed | ulong\|uint ∧ 无参 getter | :107-111 |
| 13 | PeerVersionInfo.LocalDefault | 静态无参 → PeerVersionInfo | :112 |
| 14 | NetClientGameService.HandshakeSucceeded(ulong, PeerVersionInfo)→void | 精确签名 | :113-117 |
| 15-16 | NetHostGameService/NetClientGameService .ctor(PeerVersionInfo) | 精确签名 | :118-119 |
| 17-19 | 上述 13/14/构造器返回类型复核 | :120-123 | :120-123 |

任一失败 → `Available=false` + `UnavailableReason=ex.Message`（:126-129）；`Initialize()` 一次性 Warn（:228-238）。四个 `*Target` 属性（:178-224）在不可用时返回 null，供补丁 `Prepare()` 门控。

### 5.2 GameCapabilities——签名探测 + IL 级行为探测（SilentFX.Core.GameCapabilities.cs:1-241）

- `Resolve/Card/Power`（:28-48）：按全名从 `typeof(CardModel).Assembly`（游戏程序集）解析并缓存（锁 + Ordinal 字典）。
- `FindShivCreateInHand`（:82-106）：`Shiv.CreateInHand` **先试 4 参**（Player,int,ICombatState,Player）**再退 3 参**（Player,int,ICombatState），DeclaredOnly。日志 `Shiv creation args=3` 证实本构建是 3 参版。
- `DeclaredMethod`（:108-128）：同名方法 **0 个或 ≥2 个都返回 null**（歧义即禁用，如 PoisonPower.Trigger/ConcoctPower.AfterDamageGiven 本构建双 False——类型不存在）。
- `DetectExpertiseRetention`（:130-146）：`Expertise.OnPlay` → `AsyncStateMachineAttribute.StateMachineType.MoveNext` → 交 IL walker。失败记 `_expertiseProbeFailure`，返回 false → 该特效禁用（日志 Warn `"Expertise retain FX disabled: ..."`）。本机日志无此 Warn 但结果为 False → 探测正常跑完，只是 IL 里没找到调用。
- **`CallsSingleTurnRetain` 手写 opcode walker（:148-232）**——全库唯一读 IL 处：
  1. `GetMethodBody().GetILAsByteArray()`（:150）；
  2. 从 `typeof(OpCodes)` 全部静态字段建 `ushort值→OpCode` 字典（:155-163）——免手工维护操作码表；
  3. 逐指令走：读 1 字节，`0xFE` 前缀再读 1 字节合成双字节操作码（:169-173）；
  4. 按 `OperandType` 定操作数长度：InlineNone=0 / ShortInline*=1 / InlineVar=2 / InlineBrTarget|Field|I|Method|Sig|String|Tok|Type|ShortInlineR=4 / InlineI8|InlineR=8 / InlineSwitch=4+n×4（checked）（:179-220）；每步 `Require` 边界检查（:221/:234-240），越界抛 `"Truncated Expertise IL"`；遇未知操作码/操作数直接抛（:177/:219）——**探测失败一律抛异常，由上层统一降级**；
  5. `Call/Callvirt` 时 `Module.ResolveMethod(token, 泛型实参…)`（:224），命中 `DeclaringType.FullName=="MegaCrit.Sts2.Core.Commands.CardCmd" && Name=="ApplySingleTurnRetain"` → true（:225-228）。
- 意义：不靠「类型/方法存在性」这种弱信号，而是**直接验证游戏方法的行为**（Expertise 出牌时是否调用单回合保留），行为变了特效才跟着关。
- `Initialize()`（:68-80）一次性打印能力摘要行（即日志第 4 行）。

### 5.3 RuntimeApiCompatibility——双签名委托择一（SilentFX.Core.RuntimeApiCompatibility.cs:1-182）

模式：**为每个 API 声明 official/beta 两个私有委托，探测时择优绑定，调用时再无分支**。

| API | Official 签名 | Beta 签名 | 探测 |
|---|---|---|---|
| `Hook.ModifyDamage` | 10 参（无 CardPlay） | 11 参（有 CardPlay） | `FindDamageMethod` 先 Beta 后 Official（:86-108） |
| `CreatureCmd.LoseBlock` | (Creature, decimal) | (PlayerChoiceContext, Creature, decimal, Creature) | `?? 链`（:20） |
| `CardSelectCmd.PushSelector` | (ICardSelector) | (ICardSelector, bool localOnly) | `?? 链`（:22） |

静态构造器按**参数个数**创建对应委托（:162-181）；`PreviewDamage/LoseBlock/PushSelector` 在双缺时才抛 `MissingMethodException`（:124/:137/:150）。另有可选探测：`NDevConsole.IsConsoleVisible`、`NControllerManager.InputType/IsUsingDirectionalNavigation/IsUsingController`、`CardPlay.Player` 属性委托（:176-180），全部 null 容忍；`DescribeInput`（:153-160）与 `PlayerOf`（:68-84）各自有降级分支。**注意只探 Public Static**（:63-66）——与 GameCapabilities 的 DeclaredOnly 策略互补。

### 5.4 Aim* 补丁族：如何嵌入游戏网络栈（SilentFX.Patches.*，源 19450-19568）

**补丁归属表**（已用签名逐条校验；属性多在上一文件尾部，切分伪影）：

| 补丁类 | 目标（属性出处） | 钩子 | 行号 |
|---|---|---|---|
| AimMessageTypesPatch | `MessageTypes.Initialize`（属性在 WraithFormFx.cs:177） | Postfix → `NormalizeTypes()` | :6-9 |
| AimVersionPatch | `PeerVersionInfo.LocalDefault`（动态 TargetMethod，Prepare 门控） | Postfix(ref __result) → `Advertise` | :16-19 |
| AimHostHandshakePatch | `NetClientGameService.HandshakeSucceeded`（动态） | Postfix → `RememberHost` | :16-19 |
| AimHostServicePatch | `NetHostGameService..ctor`（动态） | Postfix → `AimSync.Attach` | :16-19 |
| AimClientServicePatch | `NetClientGameService..ctor`（动态） | Postfix → `AimSync.Attach` | :16-19 |
| MouseAimSessionPatch | `NMouseCardPlay.Start`（属性在 AimClientServicePatch.cs:21） | Prefix → `AimLocalTarget.Begin` | :6-9 |
| ControllerAimSessionPatch | `NControllerCardPlay.Start`（属性在 MouseAimSessionPatch.cs:11） | Prefix → `Begin` | :6-9 |
| LocalAimHoverPatch | `NCardPlay.OnCreatureHover`（属性在 ControllerAimSessionPatch.cs:11） | Postfix → `SetTarget(creature.Entity)` | :6-9 |
| LocalAimUnhoverPatch | `NCardPlay.OnCreatureUnhover`（属性在 LocalAimHoverPatch.cs:11） | Postfix → `ClearTarget` | :6-9 |
| LocalAimCleanupPatch | `NCardPlay.Cleanup`（属性在 LocalAimUnhoverPatch.cs:11） | Prefix → `End` | :6-9 |

嵌入点设计：**5 个动态补丁全部 `Prepare()` 门控**（能力缺失时整类跳过，Harmony 不报错）；5 个静态补丁无门控但体内自守。消息处理器在 service 构造时注册（`RegisterMessageHandler<AimMessage>`，AimSync.cs:85-91，每 service 一次，ConditionalWeakTable 防重）。

---

## ⑥ 数值全表

| 数值 | 位置 | 含义 |
|---|---|---|
| 0.05s（20Hz） | AimSync.cs:440 | 快照采样/发送频率上限 |
| 1.0s | AimSync.cs:186 | Offer 广播间隔（_nextOffer） |
| 1.0s | AimSync.cs:483 | 快照强制刷新间隔（无变化也重发，_refresh） |
| 3.0s | AimSync.cs:317/365 | 租约时长（全员 Ack 轮或收到 Go 后续期） |
| 3.0s | AimSync.cs:517 | 快照新鲜度阈值（过期即不可信） |
| 300.0s | RemoteFxFocus.cs:35/238 | 非持有租约空闲回收（兜底防泄漏） |
| 255 | AimCompatibility.cs:291 | 消息表容量上限（8-bit ID，255 疑保留哨兵，待确认） |
| 2048 | RemoteFxFade.cs:90/147 | 淡出 root 数上限 |
| 16384 | RemoteFxFade.cs:92/230/257/280 | 子树遍历/祖先查询节点数上限 |
| 1024 | RemoteFxFade.cs:160 | 单次祖先上溯深度上限 |
| 4 | RemoteShivCreation.cs:21 | Cloak 首批可见飞镖数 clamp 上限（1..4） |
| 8/32/64 bit | AimMessage.cs:52-76 | Kind/ Room·Act·Target / 各 ulong 线宽 |
| SHA256(前8字节) | AimSync.cs:168 | Roster 玩家名单指纹 |
| SHA256(全表) | AimCompatibility.cs:529 | 消息表指纹（Marker 核心） |
| "SilentFX.Targeting/4:" | AimCompatibility.cs:154-156 | 协议前缀+版本号（v4） |
| 0.25s | Ticker.cs:93 / RemoteFxFocus.cs:231 | 帧 dt 封顶 |
| nonce≠0 重抽 | AimSync.cs:552-561 | RandomNumberGenerator 8 字节，禁 0 |
| 默认开启 | MultiplayerFx.cs:23 | `mp.remote_fx` 默认 1f，置 0 全关 |

---

## ⑦ 「注释等效物」编目（从字符串与日志反推设计意图）

**降级链总设计**（每条 Warn 都是一个决策点）：
1. `"[MP] Exact targeting unavailable; using native peer input: {reason}"`（AimCompatibility.cs:235，一次性）——顶层降级宣言；本机实跑出现，reason=`game handshake version metadata is absent`。
2. `"[MP] Aim message normalization was skipped because it would merge distinct message IDs."`（:285）——作者预见其他 mod 可能制造 list/dict 不一致，**安全优先：宁可不同步也不错位**。
3. `"[MP] Aim message was omitted because its ID is reserved by another message mapping."`（:318）——尾部 ID 被占（表内删了 dict 没清）则弃。
4. `"[MP] Aim message was omitted because the message table is full."`（:318）——≥255 则弃；印证 ID 是 8-bit 线格式。
5. `"[MP] Aim message normalization unavailable: {ex}"`（:327）——反射结构不符（游戏改版）时总兜底。
6. `"[MP] Aim capability advertisement unavailable: {ex}"`（:359）、`"[MP] Target receiver unavailable: {ex}"`（:97）、`"[MP] Exact targeting fell back to peer input: {ex}"`（:208）、`"[MP] Ignored targeting packet: {ex}"`（:376）——四层各自的最后防线。
7. `"Remote input API unavailable: {ex}"`（RemoteHeldFx.cs:55）、`"Remote standby stopped: {ex}"`（:286）——降级路径自身的失败宣告；此后远端 FX 全无。
8. `"RemoteFxFade.BeforeDraw: {ex}"`（RemoteFxFade.cs:268）、`"Expertise retain FX disabled: {failure}"`（GameCapabilities.cs:77）——渲染/探测层兜底。
9. `"Game capabilities: Shiv creation args=3, Poison trigger=False, Concoct=False, Expertise retain FX=False"`（GameCapabilities.cs:74）——一行 capability 摘要，排障入口。

**反推出的踩坑与意图**：
- **指纹 = 全消息表 SHA256**：Marker 不只是 mod 版本号，而是整个消息 ID 空间的共识哈希。对方多装一个也注册消息的 mod → 指纹不符 → 不启用 Exact。防的是 ID 错位导致的**解析错乱**（比不同步严重得多的故障）。
- **otherMods 全等校验**（Matches :494）：连非玩法 mod 列表都要求完全一致，极端保守（是否有意为之待确认）——MP 下所有客户端模拟必须一致，任何差异都宁可放弃增强。
- **Roster + Sequence + Epoch 三 nonce 体系**：Roster 防串房/重放，Sequence 单调递增防乱序双发，Epoch 防跨协议代的旧包；主机改写 `_relaySequence` 把客户端序号「收编」为权威序号。
- **Sample 的 carry-over**（:461-466）：松手发「inactive + 同卡」而非空包——远端靠此区分「收了待命特效」与「从未持卡」，渲染连续性的关键细节。
- **`ValidSnapshot` 用本地事实锚定远端声明**（Owner.NetId==Source ∧ 手牌堆 ∧ 存活）：不信任任何客户端自报状态。
- **`ShouldBroadcast=false/ShouldBuffer=false`**：20Hz 状态流不走广播与迟加入缓冲，mod 精确单播/中继。
- **协议层用墙钟（Stopwatch），表现层用真实 dt**：子弹时间下握手/租约/采样不受影响，但 RemoteFxFocus 的 idle 累计在暂停/TimeScale≤0.001 时停走（RemoteFxFocus.cs:227-230）——两层时间观刻意分离。
- **fail-open vs fail-closed 的两种默认**：`MultiplayerFx.IsLocal` catch→false（按远端处理，会套淡出，视觉略偏但安全，MultiplayerFx.cs:61-62）；`ShivFx` 的 IsMe catch→true（按本地处理，不渲染远端，ShivFx.cs:88-93）——异常时都指向「不渲染不确定的东西」。
- **`IsInSelection` 不算 targeting**（AimLocalTarget.cs:70-75）：选牌界面（多选一）不能同步成「瞄准中」。
- **`GetCreatureNode(val)==null → val=null`**（:81-84）：目标节点没了就不发目标，比发一个悬空 CombatId 安全。

---

## ⑧ 对移植项目的价值评估

**若我们的项目要做 MP 特效同步（STS2 mod 或其他有私有消息表的联机游戏）——可直接借鉴，按价值排序**：

1. **NormalizeTypes 消息表注入（最高价值，全库稀缺）**：反射私有静态缓存 → list 下标重建映射 → 剔除旧注册保幂等 → 防合并检查 → 尾部占用/容量门 → 脏检查写回。七步可直接照抄；关键是「list 为真相源 + 防合并宁可放弃」两条原则。
2. **指纹验签**：把「整个消息表 + 协议版本」的 SHA256 插进握手元数据，双方比对——用一致性哈希替代脆弱的版本号协商。
3. **主机权威租约协议骨架**：Offer/Ack/Go/Snapshot 四消息 + nonce 三件套（Epoch/Round/Generation）+ 主机中继改写序列号 + ValidSnapshot 式本地事实锚定。20Hz/1s/3s 这组参数是经过推敲的默认值（1s offer vs 3s lease 容忍两连败）。
4. **渲染侧四件套分工**：AsyncLocal 上下文传播（一次 OnPlayWrapper 补丁，全场卡牌获得「当前卡」语境）、modulate 乘法淡出（FramePreDraw 写 / FramePostDraw 还原，`Modulate==Applied` 才还原）、淡出强度租约（本地动作时压远端 FX 到 50%）、可见数差值补偿（ShowCloak）。

**若我们的项目不做 MP——仍有高迁移价值的部分**：

1. **GameCapabilities 的能力探测范式**：DeclaredMethod 歧义返回 null、RequiredField 带类型校验、Lazy 探测 + 一次性日志摘要行。对任何「跟随游戏/引擎版本演进的 mod」直接适用。
2. **手写 IL opcode walker**（CallsSingleTurnRetain）：当「方法存在性」不足以判断行为时，直接扫 IL 验证调用关系；`typeof(OpCodes)` 反射建码表 + OperandType 定长 + 边界检查 + 失败一律抛异常统一降级，是可以复用的探测模板。
3. **RuntimeApiCompatibility 双签名委托择一**：official/beta 双委托 + 参数个数绑定 + 调用点无分支，比满街 `if (method != null)` 干净。
4. **降级链工程文化**：每层独立 Warn 文案、初始化即降级、补丁永不通向崩溃——这套「注释等效物」本身就是可移植的工程规范。

**明确不需要照抄的**：AimSync 的完整状态机复杂度（若我们的联机需求只是「看到队友在瞄准哪张卡」，RemoteHeldFx 的原生输入降级路径——PeerInputSynchronizer + HoveredModelTracker + 光标 hitbox——60 行就能拿到 80% 效果）；RemoteShivCreation 的 ShowCloak 差值补偿高度绑定 SilentFX 自己的 HeldFx 会话体系。

---

## 附：待确认清单

1. `(NetTransferMode)2`、`(LogLevel)2` 的枚举名与确切语义（按 20Hz 快照推断为不可靠传输+低日志）。
2. 消息表 255 容量上限的确切原因（8-bit ID + 255 保留哨兵值为推断）。
3. `Pile.Type==2` 即手牌堆（语义明确，枚举名未证）。
4. `Matches` 要求 `otherMods` 全等是否有意为之（推断为保守设计）。
5. `HandshakeManager` 类型只做存在性金丝雀、后续不使用（推断）。
6. `NetCombatCard` 为游戏侧类型（FromModel/Serialize/ToCardModelOrNull），其内部线格式未展开（不在本报告范围）。
7. 本构建 `PeerVersionInfo` 缺失意味着 Exact 链从未实跑——协议的正确性只有作者自测证据，无生产验证（重要提示）。

---

*报告完 · deep-mp · 2026-10-16*
