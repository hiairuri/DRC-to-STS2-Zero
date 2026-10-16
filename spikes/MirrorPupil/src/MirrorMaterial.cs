using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace MirrorPupil;

/// <summary>
/// 镜像材质：龙族「镜像战士」材质的 STS2 移植（MySpineUnlitHitMaterial_Inst，
/// 2026-10-05 源码严格定案，见 ling_mirror_mix.gdshader 头注的完整源链路）：
/// 运行时 darkenParam=0（C MID 工厂写组件字段默认值）→ FS 反相项 t=0 →
/// 观感=角色贴图的反相负片 (1−albedo) + 场景光（2D 中性=1）。
/// 龙族侧语义（fightunitfighter.lua:1004）＝镜像单位把整套渲染材质换成该材质；
/// STS2 的 SpineSprite 按 blend mode 拆成 normal/additive/multiply/screen 四族，
/// 四族全换＝"整个单位用这套材质渲染"的等价物。
/// 挂点＝MirrorImage.GenerateAnimator：该时机骨架已初始化（CreatureAnimator 构造
/// 会强制检查动画状态），材质属性一经设置即持久，不会被游戏侧覆盖。
/// 资源路径＝mod pck 内 res://effects/mirror/（pcksrc/effects/mirror 原样打包）。
/// </summary>
internal static class MirrorMaterial
{
	/// <summary>镜像骨架路径的匹配片段（按你的镜像 spine 资源命名调整）。</summary>
	public const string SkeletonMatch = "mirror";

	public static void Apply(MegaSprite controller)
	{
		try
		{
			if (controller.BoundObject is not Node sprite) return;
			var mix = GD.Load<ShaderMaterial>("res://effects/mirror/ling_mirror_material_mix.tres");
			var add = GD.Load<ShaderMaterial>("res://effects/mirror/ling_mirror_material_add.tres");
			var mul = GD.Load<ShaderMaterial>("res://effects/mirror/ling_mirror_material_mul.tres");
			var sub = GD.Load<ShaderMaterial>("res://effects/mirror/ling_mirror_material_sub.tres");
			if (mix == null || add == null || mul == null || sub == null)
			{
				GD.PrintErr("[MirrorPupil] mirror materials missing from pck (res://effects/mirror/)");
				return;
			}
			sprite.Set("normal_material", mix);
			sprite.Set("additive_material", add);
			sprite.Set("multiply_material", mul);
			sprite.Set("screen_material", sub);
		}
		catch (System.Exception e)
		{
			// 材质失败绝不能打断怪物视觉构建（镜像不出现比不换材质严重得多）
			GD.PrintErr("[MirrorPupil] mirror material apply failed: " + e.Message);
		}
	}
}
