using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.PotionPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace LingChar;

/// <summary>
/// 零（龙族）。模型 id = "LING"（Slugify(类名)）。
/// 资产面（id 派生、非虚成员不可覆写的都由 pcksrc 提供同名 ling 资产）：
///   scenes/creature_visuals/ling.tscn               战斗身体（SpineSprite→ling_skel_data）
///   scenes/vfx/card_trail_ling.tscn                 出牌拖尾
///   scenes/combat/energy_counters/ling_energy_counter.tscn
///   scenes/rest_site/characters/ling_rest_site.tscn / merchant/characters/ling_merchant.tscn
///   scenes/screens/char_select/char_select_bg_ling.tscn / materials/transitions/ling_transition_mat.tres
///   scenes/ui/character_icons/ling_icon.tscn（IconPath）+ 选人竖版立绘 char_select_ling.png
///   images/ui/top_panel/character_icon_ling{,_outline}.png + images/ui/hands/multiplayer_hand_ling_*.png
/// 2026-10-08 去污染定案：战斗骨架/选人立绘/背景/图标全部 LingChar 自有路径
/// （res://ling/animations/characters/ling/ 与 animations/*/ling/），由
/// build_lingchar.py 构建期从皮肤源树（LingSilentSkin_battle）受控同步——
/// 运行期与 LingSilentSkin.pck 零共享路径。未解锁剪影/地图标记仍借游戏原生资产。
/// 战斗内视觉装饰（材质/影子/死亡溶解/胜利动画）由本 dll 内嵌的 DyeFramework
/// 按 res://drc_dye/profiles/ling/animations/characters/ling/ 的 profile 挂载。
/// </summary>
public sealed class Ling : CharacterModel
{
	public override Color NameColor => new Color("3ec6e0FF");

	public override CharacterGender Gender => CharacterGender.Feminine;

	protected override CharacterModel? UnlocksAfterRunAs => null;

	public override int StartingHp => 70;

	public override int StartingGold => 99;

	// 卡池临时用无色（用户指定）；专属卡池后补
	public override CardPoolModel CardPool => ModelDb.CardPool<ColorlessCardPool>();

	// 遗物/药水池临时借用 Silent（与借用资产同一策略）
	public override RelicPoolModel RelicPool => ModelDb.RelicPool<SilentRelicPool>();

	public override PotionPoolModel PotionPool => ModelDb.PotionPool<SilentPotionPool>();

	// 初始牌组：六张打击 + 四张防御（用户指定；打击/防御借用静默猎手版本）
	public override IEnumerable<CardModel> StartingDeck => new List<CardModel>
	{
		ModelDb.Card<StrikeSilent>(),
		ModelDb.Card<StrikeSilent>(),
		ModelDb.Card<StrikeSilent>(),
		ModelDb.Card<StrikeSilent>(),
		ModelDb.Card<StrikeSilent>(),
		ModelDb.Card<StrikeSilent>(),
		ModelDb.Card<DefendSilent>(),
		ModelDb.Card<DefendSilent>(),
		ModelDb.Card<DefendSilent>(),
		ModelDb.Card<DefendSilent>(),
	};

	// 初始遗物=言灵·镜瞳（MirrorPupil spike 集成；战斗开始召唤镜像+给瞳印）
	public override IReadOnlyList<RelicModel> StartingRelics =>
		new List<RelicModel> { ModelDb.Relic<global::MirrorPupil.MirrorPupil>() };

	// 龙卡 attack 动画(0.5s)枪臂 IK 峰值/花特效重挂载都在 0.4s——伤害与
	// HitFlash 闪光对齐真实命中帧(此前借用 Silent 的 0.15s 会闪在起手动作上)
	public override float AttackAnimDelay => 0.4f;

	public override float CastAnimDelay => 0.25f;

	// --- UI 资产（2026-10-08 去污染：立绘/图标全部 Ling 自有路径，
	//     由 build_lingchar.py 构建期从皮肤源树同步，运行期不依赖皮肤 mod） ---
	protected override string IconPath => SceneHelper.GetScenePath("ui/character_icons/ling_icon");

	protected override string CharacterSelectIconPath => ImageHelper.GetImagePath("packed/character_select/char_select_ling.png");

	// 未解锁剪影沿用游戏原生资产（游戏自有路径，非 mod 依赖）
	protected override string CharacterSelectLockedIconPath => ImageHelper.GetImagePath("packed/character_select/char_select_silent_locked.png");

	protected override string MapMarkerPath => ImageHelper.GetImagePath("packed/map/icons/map_marker_silent.png");

	public override string CharacterSelectSfx => "event:/sfx/characters/silent/silent_select";

	public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_silent";

	// Architect 战演出特效：借用 Silent 的四件（路径真实存在）
	public override List<string> GetArchitectAttackVfx()
	{
		return new List<string>
		{
			"vfx/vfx_dagger_spray",
			"vfx/vfx_flying_slash",
			"vfx/vfx_dramatic_stab",
			"vfx/vfx_dagger_throw",
		};
	}

	/// <summary>
	/// ling 骨架动画集：attack/block/cast/die/entry/hurt/idle_loop/jump/onfoot/
	/// sit/sitloop/skill01_2/skill01_3/stand/tibu/win/winloop（无 relaxed_loop/shiv）。
	/// 入场 tibu 由 DRCSpineDye 的 entrance watcher 驱动（本处初始态直接 idle_loop，
	/// 避免双重入场）；胜利 win+winloop 同样由框架在 CombatWon 时链式驱动。
	/// Silent 形状的触发分支里，relaxed/shiv 映射到既有动画防缺动画告警。
	/// </summary>
	public override CreatureAnimator GenerateAnimator(MegaSprite controller)
	{
		AnimState idle = new AnimState("idle_loop", isLooping: true);
		AnimState attack = new AnimState("attack");
		AnimState cast = new AnimState("cast");
		AnimState hurt = new AnimState("hurt");
		AnimState die = new AnimState("die");
		AnimState win = new AnimState("win");
		AnimState winloop = new AnimState("winloop", isLooping: true);
		attack.NextState = idle;
		cast.NextState = idle;
		hurt.NextState = idle;
		win.NextState = winloop;
		CreatureAnimator creatureAnimator = new CreatureAnimator(idle, controller);
		creatureAnimator.AddAnyState("Idle", idle);
		creatureAnimator.AddAnyState("Dead", die);
		creatureAnimator.AddAnyState("Hit", hurt);
		creatureAnimator.AddAnyState("Attack", attack);
		creatureAnimator.AddAnyState("Cast", cast);
		creatureAnimator.AddAnyState("PowerUp", cast);
		creatureAnimator.AddAnyState("Shiv", attack);
		creatureAnimator.AddAnyState("Relaxed", idle);
		creatureAnimator.AddAnyState("Win", win);
		return creatureAnimator;
	}
}
