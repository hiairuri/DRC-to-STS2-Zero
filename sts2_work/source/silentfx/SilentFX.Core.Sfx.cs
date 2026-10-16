// namespace: SilentFX.Core
// type: Sfx (class)
// source lines: 42297-44571
	public static class Sfx
	{
		public sealed class Option
		{
			public string Label;

			public string Bank = "sfx";

			public string Sample;

			public float From;

			public float To;

			public float Lead;

			public float Gain = 1f;

			public float Speed = 1f;

			public string Via;

			public float Soft;

			public float Tail = 0.05f;

			public float After;

			public Option[] With;

			public string Id => Sample + "@" + From.ToString("0.###", CultureInfo.InvariantCulture);

			public Option Hall()
			{
				Via = "hall";
				return this;
			}

			public Option Low()
			{
				Via = "low";
				return this;
			}

			public Option In(float seconds)
			{
				Soft = seconds;
				return this;
			}

			public Option Cut(float seconds)
			{
				Tail = seconds;
				return this;
			}

			public Option Plus(params Option[] more)
			{
				With = more;
				return this;
			}
		}

		public sealed class Slot
		{
			public string Name;

			public string Label;

			public bool Bed;

			public bool Fixed;

			public bool Settled;

			public float First = 1.2f;

			public float Every;

			public int Page = 1;

			public float FadeIn = 0.35f;

			public float FadeOut = 0.45f;

			public int Default;

			public Option[] Options;
		}

		private sealed class Voice
		{
			public AudioStreamPlayer Player;

			public string Name;

			public float BaseLevel;

			public ulong Since;

			public double StopAt = -1.0;

			public float Out = 0.05f;

			public Tween Tween;
		}

		private sealed class Later
		{
			public double At;

			public string Name;

			public float Volume;

			public float Pitch;

			public Option Piece;
		}

		private sealed class BedState
		{
			public string Name;

			public float Target;

			public float Level;

			public float Pitch = 1f;

			public double Until;

			public double Turn;

			public AudioStreamPlayer A;

			public AudioStreamPlayer B;

			public AudioStream Stream;

			public Option Piece;

			public float PhaseA = -1f;

			public float PhaseB = -1f;

			public float In = 0.35f;

			public float Out = 0.45f;

			public bool Told;
		}

		private sealed class Scatter
		{
			public string Name;

			public float Level;

			public double Until;

			public double Next;

			public int Last = -1;
		}

		private const string Roar2 = "sts2_sfx_haunted_ship_die_roar_v1_rr2";

		private const string Roar3 = "sts2_sfx_haunted_ship_die_roar_v1_rr3";

		private const string Bubbles = "sts2_sfx_haunted_ship_cast_bubbles_v1";

		private const string Steam = "sts2_sfx_act1b_waterfall_giant_loop_steam_v1";

		private const string Spit = "STS2_SFX_E-A2_WorkerBug-Goop-Spit_v1";

		private const string Cocoon = "STS2_SFX_E-A2_WorkerBug-Cocoon-Spit_v1";

		private const string Rocks = "sts2_sfx_act1b_waterfall_giant_loop_rocks_v1";

		private const string Kick = "sts2_sfx_act1b_waterfall_giant_kick_rr1_v1";

		private static readonly float[] PopFrom = new float[4] { 1.715f, 2.11f, 1.245f, 2.5f };

		private static readonly float[] PopTo = new float[4] { 1.8f, 2.215f, 1.33f, 2.62f };

		private static readonly float[] PopGain = new float[4] { 1.6f, 3.6f, 1.2f, 7f };

		public static readonly Slot[] Slots = new Slot[41]
		{
			new Slot
			{
				Name = "murder_drone",
				Label = "谋杀·低沉的底",
				Bed = true,
				Default = 1,
				FadeIn = 0.9f,
				FadeOut = 0.5f,
				Options = new Option[4]
				{
					O("无餍·低鸣", "STS2_Act2_SFX_Amb_Insaitable_v2", 2f, 0f, 5.5f, 0.5f, 32f, "ambience").Low(),
					O("深处·底噪", "STS2_Act3_SFX_Amb_Base_v1", 5f, 0f, 5.5f, 0.55f, 35f, "ambience").Low(),
					O("蓄能·脉动", "STS2_SFX_E-A1_OrbPlant_ChargedLoop_v1", 0.05f, 0f, 2.6f, 0.4f, 5.45f).Low(),
					O("末日·轰鸣", "sts2_sfx_necrobinder_doom_kill_layer1_rr1", 0.25f, 0f, 0.3f, 0.6f, 1.3f).Low()
				}
			},
			new Slot
			{
				Name = "murder_edge",
				Label = "谋杀·定格的一丝",
				Default = 1,
				Options = new Option[3]
				{
					O("金属轻鸣", "sts2_sfx_silent_cast_metallic_v1", 0.33f, 0.13f, 6f).In(0.02f).Hall(),
					O("细刃", "sts2_sfx_act1_the_kin_slash_blade_rr2_v1", 0.03f, 0.12f, 2.4f).Hall(),
					O("快刀·高", "sts2_sfx_silent_attack_blade_rr1_v4", 0.02f, 0.08f, 0.3f, 1.5f).Hall()
				}
			},
			new Slot
			{
				Name = "murder_cut",
				Label = "谋杀·第一刀（刃 + 沉）",
				Default = 1,
				Options = new Option[3]
				{
					O("快刀 + 低沉", "sts2_sfx_silent_attack_blade_rr1_v4", 0.02f, 0.11f, 0.95f).Hall().Plus(L("sts2_sfx_regent_forge_lowend_v1", 0f, 0.5f, 1f, 0.02f)),
					O("骨刃 + 血墙", "sts2_sfx_necrobinder_attack_blade_rr1_v2", 0.03f, 0.13f, 0.75f).Hall().Plus(L("sts2_sfx_ironclad_bloodwall_v1_rr1", 0f, 0.42f, 1f, 0.03f)),
					O("双斩 + 闷震", "sts2_sfx_act3_test_subject_slash_rr1_v1", 0.18f, 0.15f, 1.6f).In(0.01f).Hall().Plus(L("sts2_sfx_kaiser_crab_slam_vfx_v1_rr2", 0.05f, 0.5f, 1f, 0.05f))
				}
			},
			new Slot
			{
				Name = "murder_cross",
				Label = "谋杀·第二刀",
				Default = 0,
				Options = new Option[3]
				{
					O("回刀", "sts2_sfx_silent_attack_blade_rr3_v4", 0f, 0.12f, 0.6f, 1.18f).Hall(),
					O("细刃", "sts2_sfx_act1_the_kin_slash_blade_rr3_v1", 0.03f, 0.12f, 2.6f).Hall(),
					O("骨刃·高", "sts2_sfx_necrobinder_attack_blade_rr2_v2", 0.03f, 0.13f, 0.5f, 1.2f).Hall()
				}
			},
			new Slot
			{
				Name = "wraith_held",
				Label = "幽魂·待命",
				Bed = true,
				Default = 0,
				FadeIn = 0.6f,
				FadeOut = 0.5f,
				Options = new Option[4]
				{
					O("魂核·低吟", "sts2_sfx_act3_soul_nexus_die_v1", 0.05f, 0f, 0.75f, 1f, 2.65f).Hall(),
					O("魂核·灼", "sts2_sfx_act3_soul_nexus_soul_burn_rr1_v1", 0.3f, 0f, 0.6f, 1f, 2.1f).Hall(),
					O("骑士·鬼火", "sts2_sfx_act3_spectral_knight_flame_rr1_v1", 0.7f, 0f, 0.55f, 1f, 2.3f).Hall(),
					O("涅奥·空灵", "STS2_Act1_SFX_Neow_Amb_v2", 2f, 0f, 1.6f, 1f, 18f, "ambience").Hall()
				}
			},
			new Slot
			{
				Name = "wraith_knock",
				Label = "幽魂·胸口一顿",
				Default = 2,
				Options = new Option[4]
				{
					O("闷响", "STS2_SFX_Block_Break_Low_v1", 0f, 0.05f, 0.62f, 0.8f),
					O("暗球", "sts2_sfx_defect_dark_channel_layer1_v1_rr5", 0.04f, 0.1f, 0.5f),
					O("咒", "sts2_sfx_act3_spectral_knight_hex_rr2_v1", 0.2f, 0.36f, 3f),
					O("重击 2", "STS2_SFX_HitImpact_Base_Heavy_2_v1", 0f, 0.02f)
				}
			},
			new Slot
			{
				Name = "wraith_land",
				Label = "幽魂·鬼火入体",
				Default = 2,
				Options = new Option[4]
				{
					O("魂鱼·虚无", "sts2_sfx_soulfysh_intangible_v1", 0.15f, 0.3f, 0.5f),
					O("魂鱼·重现", "sts2_sfx_soulfysh_reappear_v1", 0.1f, 0.4f, 1.1f),
					O("暗球 2", "sts2_sfx_defect_dark_channel_layer2_v1_rr3", 0.04f, 0.25f, 0.55f),
					O("增益", "sts2_sfx_buff_layer2_rr3_v1", 0f, 0.23f, 3f)
				}
			},
			new Slot
			{
				Name = "wail",
				Label = "尖啸·一声叫",
				Page = 2,
				Default = 3,
				Options = new Option[4]
				{
					O("鸟妖·长啸", "sts2_sfx_byrdonis_hurt_VO_v2_rr2", 0.05f, 0.04f, 0.45f).Hall(),
					O("鸟妖·怒啸", "sts2_sfx_byrdonis_get_angry_VO_v2_rr1", 0.12f, 0.06f, 0.42f, 1f, 1.1f).Cut(0.15f).Hall(),
					O("骑士·惨叫", "sts2_sfx_VO_spectral_knight_hurt_v1_rr4", 0.05f, 0.1f, 0.42f).Hall(),
					O("魂鱼·音波（升调）", "sts2_sfx_soulfysh_sonicwave_v1_rr1", 0.5f, 0.17f, 0.4f, 1.45f).In(0.03f).Hall()
				}
			},
			new Slot
			{
				Name = "assassin_beat",
				Label = "刺杀·心跳",
				Page = 2,
				Default = 2,
				Options = new Option[3]
				{
					O("振翅闷拍", "sts2_sfx_act3_owl_magistrate_fly_loop_2_v1", 0.09f, 0.14f, 2.6f).Low(),
					O("低鼓", "STS2_SFX_HitImpact_Base_Heavy_3_v1", 0f, 0.03f, 0.35f, 0.8f).Low(),
					O("闷响", "STS2_SFX_Block_Break_Low_v1", 0f, 0.05f, 0.3f, 0.7f, 0.35f).Cut(0.1f).Low()
				}
			},
			new Slot
			{
				Name = "assassin_pierce",
				Label = "刺杀·穿透",
				Page = 2,
				Default = 1,
				Options = new Option[3]
				{
					O("刺客", "STS2_SFX_E-A1_RubyRaiders_AssassinAttack_rr1_v2", 0.14f, 0.2f, 0.6f).Hall(),
					O("潜行刺", "sts2_sfx_sneaky_gremlin_solo_stab_v1_rr1", 0.03f, 0.1f, 0.42f).Hall(),
					O("佣兵刺", "sts2_sfx_gremlin_merc_stab_v1_rr2", 0.09f, 0.14f, 0.4f).Hall()
				}
			},
			new Slot
			{
				Name = "finisher_last",
				Label = "终结技·最后一击的分量",
				Page = 2,
				Default = 0,
				Options = new Option[3]
				{
					O("低沉", "sts2_sfx_regent_forge_lowend_v1", 0f, 0.1f, 0.6f),
					O("闷震", "sts2_sfx_kaiser_crab_slam_vfx_v1_rr2", 0.05f, 0.12f, 0.6f),
					O("血墙", "sts2_sfx_ironclad_bloodwall_v1_rr1", 0f, 0.12f, 0.5f)
				}
			},
			new Slot
			{
				Name = "shadow_slam",
				Label = "暗影步·扎进弃牌堆",
				Page = 2,
				Default = 1,
				Options = new Option[3]
				{
					O("闷响", "STS2_SFX_Block_Break_Low_v1", 0f, 0.05f, 0.5f, 0.85f),
					O("暗球", "sts2_sfx_defect_dark_channel_layer1_v1_rr5", 0.04f, 0.1f, 0.45f),
					O("船击", "sts2_sfx_haunted_ship_attack_v2_rr1", 0.04f, 0.18f, 0.4f)
				}
			},
			new Slot
			{
				Name = "gamble_shuffle",
				Label = "计算下注·洗牌",
				Page = 2,
				Default = 5,
				Options = new Option[6]
				{
					Riffle("细密", 11, 16, 0.42f, 1f, 3.2f, 1f, 0.4f, 1, 1f, 0.62f),
					Riffle("由慢到快", 23, 14, 0.5f, 0.6f, 3.4f, 1f, 0.5f, 1, 1f, 0.7f),
					Riffle("厚牌慢洗", 37, 10, 0.55f, 1f, 3.8f, 0.78f, 0.52f, 1, 1.1f, 0.78f),
					Riffle("两手对洗", 41, 12, 0.3f, 1f, 3.2f, 1.05f, 0.33f, 2, 1f, 0.68f),
					O("只唰一下", Slide(3), 0f, 0.14f, 1.2f).Plus(L(Slide(7), 0f, 1f, 1.05f, 0.1f), L(Slide(9), 0f, 0.9f, 1.1f, 0.2f), L("sts2_sfx_ui_deck_close_v1_layer1_rr1", 0.03f, 0.22f, 1f, 0.4f)),
					O("牌库声", "sts2_sfx_ui_deck_open_v1_layer1_rr2", 0f, 0.2f, 0.3f).Plus(L("sts2_sfx_ui_deck_open_v1_layer2_rr4", 0.02f, 0.25f))
				}
			},
			new Slot
			{
				Name = "outbreak_creep",
				Label = "毒性爆发·蔓延（跟着线走，到头就停）",
				Bed = true,
				Page = 3,
				Default = 2,
				FadeIn = 0.06f,
				FadeOut = 0.1f,
				Options = new Option[5]
				{
					O("藤蔓爬", "sts2_sfx_act1_vine_shambler_cast_rr1_v1", 0.5f, 0f, 0.8f, 1f, 1.5f),
					O("绞藤", "sts2_sfx_act1_slithering_strangler_cast_rr1_v1", 0.17f, 0f, 1.6f, 1f, 0.8f),
					O("黏液爬", "sts2_sfx_act3_slimed_beserker_slime_rr2_v1", 0.25f, 0f, 1.6f, 1f, 1.45f),
					O("毛虫脚", "sts2_sfx_act1_fuzzy_wurm_crawler_cast_rr1_v1", 0.15f, 0f, 2f, 1f, 0.8f),
					O("黏液·缓", "STS2_Act2_SFX_Amb_Goo_3_v1", 0.4f, 0f, 4.5f, 1f, 3f, "ambience")
				}
			},
			new Slot
			{
				Name = "outbreak_erupt",
				Label = "毒性爆发·喷出来",
				Page = 3,
				Default = 0,
				Options = new Option[4]
				{
					O("喷泉 + 低沉", Geyser(3), 0.46f, 0.19f).Plus(L("sts2_sfx_act1b_waterfall_giant_kick_rr1_v1", 0.22f, 1.2f, 0.85f).Low()),
					O("喷泉·粗", Geyser(1), 0.46f, 0.19f, 1.15f, 0.8f).Plus(L("sts2_sfx_act1b_waterfall_giant_kick_rr1_v1", 0.22f, 1.3f, 0.75f, 0.03f).Low()),
					O("喷泉 + 重踏", Geyser(2), 0.46f, 0.19f, 0.95f).Plus(L("sts2_sfx_act1b_waterfall_giant_stomp_rr1_v1", 0.22f, 0.55f, 0.9f, 0.01f)),
					O("纯喷泉", Geyser(3), 0.47f, 0.18f, 1.15f)
				}
			},
			new Slot
			{
				Name = "bubble_boil",
				Label = "咕嘟冒泡·沸腾炸泡",
				Page = 3,
				Default = 3,
				Options = new Option[4]
				{
					O("沸腾 + 泡炸", "sts2_sfx_haunted_ship_cast_bubbles_v1", 0.56f, 0.3f, 1.3f, 1f, 2.3f).Plus(L("STS2_SFX_E-A2_WorkerBug-Goop-Spit_v1", 0.45f, 0.9f, 0.9f, 0.4f), L(Slime(2), 0f, 2.2f, 0.8f, 0.62f), L("STS2_SFX_E-A2_WorkerBug-Goop-Spit_v1", 0.45f, 0.8f, 1.1f, 0.85f), L("sts2_sfx_act1b_waterfall_giant_loop_steam_v1", 7f, 0.45f, 0.8f, 0.3f, 8.3f).In(0.08f).Cut(0.6f)),
					O("黏液咕嘟", "STS2_Act2_SFX_Amb_Goo_2_v1", 0.5f, 0.55f, 2f, 1f, 2.6f, "ambience").In(0.03f).Plus(L(Slime(1), 0f, 2f, 0.75f, 0.45f), L(Slime(4), 0f, 2.2f, 0.7f, 0.8f)),
					O("水弹连炸", "sts2_sfx_cultists_cast_splash_v2_rr2", 0.19f, 0.3f, 0.4f).Plus(L("sts2_sfx_haunted_ship_cast_bubbles_v1", 0.56f, 1f, 1f, 0.1f, 1.9f), L("STS2_SFX_E-A2_WorkerBug-Cocoon-Spit_v1", 0.43f, 0.9f, 0.85f, 0.35f), L("STS2_SFX_E-A2_WorkerBug-Goop-Spit_v1", 0.45f, 0.9f, 1f, 0.6f)),
					O("腐蚀沸腾", "sts2_sfx_act1b_waterfall_giant_loop_steam_v1", 9f, 0.2f, 0.7f, 0.75f, 10.4f).In(0.1f).Cut(0.6f).Plus(L("sts2_sfx_haunted_ship_cast_bubbles_v1", 0.56f, 1.2f, 0.9f, 0.05f, 2.2f))
				}
			},
			new Slot
			{
				Name = "hunt_trophy",
				Label = "狩猎·战利品",
				Page = 3,
				Default = 3,
				Options = new Option[4]
				{
					O("金币", "sts2_sfx_gold_medium_layer2_rr1_v1", 0.05f, 0.6f, 0.9f).Plus(L("sts2_sfx_ui_treasure_a1_v4", 0f, 0.45f, 1f, 0.58f)),
					O("宝箱", "sts2_sfx_ui_treasure_a1_v3", 0f, 0.06f, 0.6f).Hall(),
					O("引路星", "sts2_sfx_regent_guiding_star_v1_rr1", 0.22f, 0.16f, 0.4f).Hall(),
					O("铃", "sts1_sfx_Relic_Tingsha", 0f, 0.07f, 1.6f).Hall()
				}
			},
			new Slot
			{
				Name = "memento_hang",
				Label = "铭记死亡·暗牌悬着（待命）",
				Bed = true,
				Page = 3,
				Default = 2,
				FadeIn = 0.5f,
				FadeOut = 0.4f,
				Options = new Option[3]
				{
					O("末日·气声", "sts2_sfx_necrobinder_doom_kill_layer3_rr1", 0.35f, 0f, 0.4f, 1f, 1.95f).Hall(),
					O("咒", "sts2_sfx_act3_spectral_knight_hex_rr2_v1", 0.2f, 0f, 2.2f, 1f, 1f).Hall(),
					O("魂核·低吟", "sts2_sfx_act3_soul_nexus_die_v1", 0.05f, 0f, 0.6f, 0.85f, 2.65f).Hall()
				}
			},
			new Slot
			{
				Name = "memento_slam",
				Label = "铭记死亡·一起砸进去",
				Page = 3,
				Default = 0,
				Options = new Option[3]
				{
					O("骨法 + 闷响", "sts2_sfx_necrobinder_attack_magic_rr1_v2", 0.05f, 0.2f, 0.5f).Hall().Plus(L("STS2_SFX_Block_Break_Low_v1", 0f, 0.5f, 0.8f, 0.12f)),
					O("暗球", "sts2_sfx_defect_dark_channel_layer1_v1_rr5", 0.04f, 0.1f, 0.5f).Hall(),
					O("惊惧", "sts2_sfx_terror_eel_debuff_v3_rr2", 0.03f, 0.2f, 0.4f, 1f, 1.4f).Cut(0.3f).Hall()
				}
			},
			new Slot
			{
				Name = "snake_bite",
				Label = "蛇咬·合拢",
				Page = 3,
				Default = 1,
				Options = new Option[3]
				{
					O("噬咬", "sts2_sfx_act3_scroll_of_biting_bite_rr3_v1", 0.1f, 0.11f, 1.2f),
					O("大口", "sts2_sfx_act2_insatiable_bite_rr2_v1", 0.08f, 0.18f, 0.8f),
					O("鼠咬", "sts2_sfx_two_tail_rats_bite_v1_rr3", 0.12f, 0.11f, 0.35f)
				}
			},
			new Slot
			{
				Name = "snake_snap",
				Label = "蛇咬·拉断爆开",
				Page = 3,
				Default = 0,
				Options = new Option[3]
				{
					O("崩断 + 毒溅", "STS2_SFX_Block_Break_High_rr4_v1", 0f, 0.02f, 0.55f).Hall().Plus(L("STS2_SFX_E-A2_WorkerBug-Goop-Spit_v1", 0.45f, 1f, 0.9f, 0.03f)),
					O("水弹", "sts2_sfx_cultists_cast_splash_v2_rr4", 0.36f, 0.13f, 0.5f).In(0.03f).Plus(L(Slime(3), 0f, 2f, 0.85f, 0.02f)),
					O("易伤击 + 茧", "sts2_sfx_attack_vulnerable_rr1_v2", 0.04f, 0.1f, 0.5f).Hall().Plus(L("STS2_SFX_E-A2_WorkerBug-Cocoon-Spit_v1", 0.43f, 0.9f, 0.9f, 0.04f))
				}
			},
			new Slot
			{
				Name = "trap_ring",
				Label = "刀刃陷阱·刀升起（拿起时一声）",
				Page = 3,
				Default = 0,
				Options = new Option[3]
				{
					O("金属轻鸣", "sts2_sfx_silent_cast_metallic_v1", 0.33f, 0f, 5f, 1f, 1.3f).In(0.03f).Cut(0.35f).Hall(),
					O("铁链", "STS2_Act3_SFX_Amb_Chains2_v1", 0.06f, 0f, 5f, 1f, 1.4f, "ambience").In(0.03f).Cut(0.4f),
					O("刀扇余音", "sts2_sfx_silent_fanofknives_v1", 0.75f, 0f, 0.6f, 1f, 1.6f).In(0.03f).Cut(0.3f)
				}
			},
			new Slot
			{
				Name = "track_slam",
				Label = "跟踪·准星砸下",
				Page = 3,
				Default = 1,
				Options = new Option[3]
				{
					O("重落", "sts1_sfx_DropRelic_Heavy", 0f, 0.04f, 0.6f).Hall(),
					O("锁定", "STS2_SFX_Block_Gain_Hit_rr3_v1", 0.22f, 0.04f, 0.8f).Hall(),
					O("易伤击", "sts2_sfx_attack_vulnerable_rr3_v2", 0.04f, 0.1f, 0.45f).Hall()
				}
			},
			new Slot
			{
				Name = "flechette_orbit",
				Label = "飞镖·牌转过身前",
				Fixed = true,
				Page = 3,
				Default = 0,
				Options = new Option[5]
				{
					O("唰 1", Slide(1), 0f, 0.14f, 0.8f),
					O("唰 2", Slide(4), 0f, 0.14f, 0.7f),
					O("唰 3", Slide(6), 0f, 0.14f, 0.7f),
					O("唰 4", Slide(9), 0f, 0.14f, 0.7f),
					O("唰 5", Slide(10), 0f, 0.14f, 0.8f)
				}
			},
			new Slot
			{
				Name = "suppress_mist",
				Label = "压制·牌炸成雾",
				Page = 4,
				Default = 0,
				Options = new Option[4]
				{
					O("猎人施法风", "sts2_sfx_silent_cast_whoosh_rr1_v1", 0.3f, 0.18f, 4f, 1f, 1.1f).In(0.02f),
					O("雾吹", "sts2_sfx_act1b_living_fog_blow_rr2_v1", 0.3f, 0.14f, 1.2f, 1f, 1.3f).In(0.02f),
					O("出牌 + 雾起", "sts2_sfx_cardmvmt_A_layer2_power_v1_rr1", 0.25f, 0.15f, 1.5f).In(0.02f).Plus(L("sts2_sfx_act1b_living_fog_summon_rr1_v1", 0.25f, 1.2f, 1f, 0.02f).In(0.01f)),
					O("雾怪聚起", "STS2_SFX_E-A1_Fogmog_Summon_v1", 0.62f, 0.17f, 0.8f, 1f, 1.1f).In(0.02f)
				}
			},
			new Slot
			{
				Name = "suppress_bite",
				Label = "压制·链头咬住",
				Page = 4,
				Default = -1,
				Options = new Option[4]
				{
					O("蟹钳剪", "sts2_sfx_kaiser_crab_lh_attack_scissor_v1", 0.26f, 0.08f, 0.4f, 1f, 0.7f).In(0.01f),
					O("卷轴啮咬", "sts2_sfx_act3_scroll_of_biting_bite_rr1_v1", 0.14f, 0.08f, 1.2f, 1f, 0.45f).In(0.01f),
					O("锁上 + 铁链", "STS2_SFX_Block_Gain_Hit_rr3_v1", 0.22f, 0.04f, 0.6f).Plus(L("STS2_Act3_SFX_Amb_Chains1_v1", 3.26f, 1.5f, 1f, 0f, 3.75f, "ambience").In(0.01f)),
					O("鼠咬", "sts2_sfx_two_tail_rats_bite_v1_rr1", 0.15f, 0.08f, 0.45f, 1f, 0.6f).In(0.01f)
				}
			},
			new Slot
			{
				Name = "suppress_tear",
				Label = "压制·撕魂",
				Page = 4,
				Default = 1,
				Options = new Option[4]
				{
					O("重击 + 处决", "STS2_SFX_HitImpact_Base_Heavy_1_v1", 0f, 0.03f).Plus(L("sts2_sfx_necrobinder_doom_kill_layer1_rr2", 0.42f, 0.6f, 1f, 0f, 1.6f), L("STS2_SFX_Block_Break_Low_v1", 0f, 0.6f, 0.85f, 0.25f)),
					O("船击 + 闷震", "sts2_sfx_haunted_ship_attack_v2_rr1", 0.15f, 0.07f, 0.65f, 1f, 0.5f).Plus(L("sts2_sfx_kaiser_crab_slam_vfx_v1_rr2", 0.12f, 0.45f, 1f, 0.1f)),
					O("贪食者终结", "sts2_sfx_act2_insatiable_finisher_v1", 0.38f, 0.08f, 0.55f, 1f, 2.2f).Hall(),
					O("重击 + 魂核", "STS2_SFX_HitImpact_Base_Heavy_3_v1", 0f, 0.03f, 0.9f).Plus(L("sts2_sfx_act3_soul_nexus_soul_burn_rr2_v1", 0.55f, 0.9f, 1f, 0f, 2.2f))
				}
			},
			new Slot
			{
				Name = "suppress_burst",
				Label = "压制·魂炸开散去",
				Page = 4,
				Default = 2,
				Options = new Option[4]
				{
					O("炸开 + 消散", "sts2_sfx_act1b_living_fog_minion_explode_rr1_v1", 0f, 0.04f, 0.6f, 1f, 0.7f).Plus(L("sts2_sfx_enemy_fade_high_rr1_v1", 0.05f, 3.5f, 0.9f)),
					O("魂鱼散去", "sts2_sfx_VO_soul_fysh_die_v1_rr2", 0.08f, 0.09f, 0.55f).Hall(),
					O("雾灵消散", "sts2_sfx_act1b_living_fog_die_v1", 0.25f, 0.24f, 0.75f, 1f, 1.8f).Hall(),
					O("亡魂", "sts2_sfx_VO_spectral_knight_die_v1_rr1", 0.08f, 0.12f, 0.5f).Hall()
				}
			},
			new Slot
			{
				Name = "shadow_strike",
				Label = "暗影步·砸地",
				Page = 4,
				Default = 0,
				Options = new Option[4]
				{
					O("蟹砸地 + 重击", "sts2_sfx_kaiser_crab_slam_vfx_v1_rr2", 0.03f, 0.24f, 0.8f).Plus(L("STS2_SFX_HitImpact_Base_Heavy_2_v1", 0f, 0.9f, 1f, 0.22f)),
					O("母兽砸", "sts2_sfx_lagavulinmatriarch_attack_slam_rr1_v1", 0.15f, 0.24f, 0.6f),
					O("巨人踏", "sts2_sfx_act1b_waterfall_giant_stomp_rr1_v1", 0.1f, 0.32f, 0.6f, 1f, 1.2f),
					O("末日落", "sts2_sfx_necrobinder_doom_kill_layer2_rr1", 1.95f, 0.32f, 0.6f, 1f, 2.45f)
				}
			},
			new Slot
			{
				Name = "malaise_drink",
				Label = "萎靡·汲取（拽的时候）",
				Bed = true,
				Page = 4,
				Default = -1,
				FadeIn = 0.25f,
				FadeOut = 0.45f,
				Options = new Option[4]
				{
					O("吸", "STS2_SFX_E-A2_Mite-Suck_v1", 0.45f, 0f, 1.6f, 0.85f, 1.35f),
					O("液化", "sts2_sfx_act2_insatiable_liquify_rr2_v1", 0.35f, 0f, 0.9f, 0.9f, 1.3f),
					O("魂核灼", "sts2_sfx_act3_soul_nexus_soul_burn_rr1_v1", 0.5f, 0f, 1.3f, 0.85f, 1.8f),
					O("流涎", "sts2_sfx_act2_insatiable_salivate_rr2_v1", 0.35f, 0f, 1.1f, 0.85f, 0.85f)
				}
			},
			new Slot
			{
				Name = "malaise_tear",
				Label = "萎靡·最后一颗球撕下来",
				Page = 4,
				Default = 3,
				Options = new Option[4]
				{
					O("减益", "sts2_sfx_debuff_layer2_rr1_v1", 0f, 0.1f, 0.9f).Plus(L("sts2_sfx_debuff_layer1_rr1_v1", 0f, 0.5f)),
					O("舔干", "STS2_SFX_E-A2_SpinyToad-Lick_v1", 0.55f, 0.39f, 0.8f),
					O("虚弱重压", "sts2_sfx_decemillipede_attack_weaken_rr3_v1", 0.24f, 0.29f, 0.45f),
					O("低沉（原来选的）", "sts2_sfx_regent_forge_lowend_v1", 0f, 0.1f, 0.5f)
				}
			},
			new Slot
			{
				Name = "expose_gear",
				Label = "暴露·白线磨过齿轮",
				Page = 4,
				Default = 2,
				Options = new Option[3]
				{
					O("钻头", "sts2_sfx_defect_attack_drill_v4_rr1", 0.12f, 0.14f, 0.7f, 1f, 0.42f).In(0.01f).Plus(L("STS2_SFX_Block_Hit_High_rr1_v1", 0f, 0.8f), L("STS2_SFX_HitImpact_Sweetner_Armour_1_v1", 0f, 1.6f, 1.15f, 0.157f)),
					O("齿轮咔咔", "STS2_SFX_Block_Hit_High_rr2_v1", 0f, 0.03f, 0.8f).Plus(L("STS2_SFX_HitImpact_Sweetner_Armour_1_v1", 0f, 1.4f, 1.2f, 0.013f), L("STS2_SFX_HitImpact_Sweetner_Armour_2_v1", 0f, 1.4f, 1.25f, 0.078f), L("STS2_SFX_HitImpact_Sweetner_Armour_1_v1", 0f, 1.4f, 1.3f, 0.128f), L("sts1_sfx_DropRelic_Clink", 0f, 0.6f, 1f, 0.152f)),
					O("工厂机械", "sts2_sfx_act3_fabricator_fabricate_1_v1", 0.45f, 0f, 1.5f, 1f, 0.72f).In(0.01f).Plus(L("STS2_SFX_Block_Hit_High_rr3_v1", 0f, 0.8f), L("sts1_sfx_DropRelic_Clink", 0f, 0.5f, 1f, 0.152f))
				}
			},
			new Slot
			{
				Name = "expose_shield",
				Label = "暴露·白线压碎护盾",
				Page = 4,
				Default = 0,
				Options = new Option[3]
				{
					O("破盾·亮", "STS2_SFX_Block_Hit_High_rr4_v1", 0f, 0.01f, 0.5f).Plus(L("STS2_SFX_Block_Hit_High_rr5_v1", 0f, 0.55f, 1f, 0.04f), L("STS2_SFX_Block_Break_High_rr1_v1", 0f, 0.9f, 1f, 0.0735f)),
					O("破盾·闷", "STS2_SFX_Block_Hit_Low_rr2_v1", 0f, 0.06f, 0.45f).Plus(L("STS2_SFX_Block_Break_Low_v1", 0f, 0.8f, 1f, 0.0735f)),
					O("障壁受击", "sts2_sfx_act3_barricade_hurt_shield_1_v1", 0.8f, 0.07f, 1.6f, 1f, 1.3f)
				}
			},
			new Slot
			{
				Name = "trap_erupt",
				Label = "刀刃陷阱·牌堆炸出刀（刀扇时）",
				Page = 4,
				Default = 2,
				Options = new Option[3]
				{
					O("匕首喷射", "sts2_sfx_silent_dagger_spray_v1_rr1", 0f, 0.05f, 0.45f, 1f, 0.55f).Cut(0.1f),
					O("匕首喷射 + 闷响", "sts2_sfx_silent_dagger_spray_v1_rr3", 0f, 0.2f, 0.4f, 1f, 0.5f).Cut(0.1f).Plus(L("STS2_SFX_Block_Break_Low_v1", 0f, 0.4f)),
					O("刀扇", "sts2_sfx_silent_fanofknives_v1", 0.35f, 0.16f, 0.4f, 1f, 1f).In(0.02f)
				}
			},
			new Slot
			{
				Name = "trap_final",
				Label = "刀刃陷阱·最后一下大爆炸",
				Page = 4,
				Default = 0,
				Options = new Option[3]
				{
					O("炸弹 + 飞刀", "sts2_sfx_magiknight_bomb_v3_rr1", 1.42f, 0.07f, 0.75f, 1f, 2.4f).In(0.01f).Plus(L("sts2_sfx_silent_dagger_spray_v1_rr2", 0f, 0.35f, 1f, 0.05f, 0.9f)),
					O("手雷", "sts2_sfx_act1_the_kin_grenade_rr1_v1", 0.95f, 0.08f, 0.9f, 1f, 2.3f).In(0.01f),
					O("毒蟾爆 + 碎盾", "STS2_SFX_E-A2_SpinyToad-Explode_v1", 0.55f, 0.15f, 0.8f, 1f, 1.3f).In(0.01f).Plus(L("STS2_SFX_Block_Break_High_rr2_v1", 0f, 0.5f, 0.8f, 0.15f))
				}
			},
			new Slot
			{
				Name = "pinpoint_shut",
				Label = "精密瞄准·刻度合拢",
				Page = 4,
				Default = 2,
				Options = new Option[4]
				{
					O("勾选", "sts2_sfx_ui_settings_check_on_v2", 0f, 0.04f, 0.9f),
					O("锁定", "STS2_SFX_Block_Gain_Hit_rr5_v1", 0.22f, 0.05f, 0.35f),
					O("遗物轻碰", "sts1_sfx_DropRelic_Clink", 0f, 0.04f, 0.5f),
					O("时间线点击", "sts2_sfx_ui_timeline_click_layer1_rr1_v1", 0f, 0.01f, 1.5f, 1f, 0.5f)
				}
			},
			new Slot
			{
				Name = "planner_write",
				Label = "谋划专家·羽毛笔写字",
				Page = 4,
				Default = 3,
				Options = new Option[4]
				{
					O("地图画线", "sts2_sfx_ui_map_draw_loop_v2", 5f, 0f, 1.6f, 1f, 5.95f).In(0.03f).Cut(0.06f),
					O("地图画线·二", "sts2_sfx_ui_map_draw_loop_v2", 9.4f, 0f, 1.6f, 1f, 10.35f).In(0.03f).Cut(0.06f),
					O("地图画线·快", "sts2_sfx_ui_map_draw_loop_v2", 2f, 0f, 1.6f, 1.3f, 3.2f).In(0.03f).Cut(0.06f),
					O("擦地图", "sts2_sfx_ui_map_Drawing_Erase_rr3_v1", 0f, 0f, 0.6f, 0.5f)
				}
			},
			new Slot
			{
				Name = "wraith_scream",
				Label = "幽魂·尖啸（鬼船·吼）",
				Fixed = true,
				Settled = true,
				Default = 0,
				Options = new Option[1] { O("鬼船·吼", "sts2_sfx_haunted_ship_die_roar_v1_rr2", 0.3f, 0.41f, 1.25f, 0.84f).Hall().Plus(L("sts2_sfx_haunted_ship_die_roar_v1_rr3", 0.3f, 0.8f, 0.63f, 0.4f).Hall(), L("sts2_sfx_haunted_ship_die_roar_v1_rr2", 0.3f, 0.5f, 0.47f, 1.9f).Hall()) }
			},
			new Slot
			{
				Name = "pool",
				Label = "腐蚀波·毒池（泡，偶尔一句）",
				Fixed = true,
				Settled = true,
				Every = 6f,
				Default = 0,
				Options = new Option[7]
				{
					Pop("泡 1", 1),
					Pop("泡 2", 2),
					Pop("泡 3", 3),
					Pop("泡 4", 4),
					Pop("噼啪…波儿", 3, 0.55f, 1.75f).Plus(Pop("", 1, 0.6f, 1.5f, 0.08f), Pop("", 2, 1.15f, 0.62f, 0.52f)),
					Pop("噼啪啪…波", 1, 0.5f, 1.85f).Plus(Pop("", 3, 0.55f, 1.6f, 0.07f), Pop("", 1, 0.6f, 1.4f, 0.15f), Pop("", 4, 1.1f, 0.66f, 0.62f)),
					Pop("波…波儿", 2, 0.9f, 0.85f).Plus(Pop("", 4, 1.1f, 0.6f, 0.38f))
				}
			},
			new Slot
			{
				Name = "storm_wind",
				Label = "钢铁风暴·风（蒸汽）",
				Bed = true,
				Fixed = true,
				Settled = true,
				Default = 0,
				Options = new Option[1] { O("蒸汽", "sts2_sfx_act1b_waterfall_giant_loop_steam_v1", 1f, 0f, 0.9f, 0.8f, 29f) }
			},
			new Slot
			{
				Name = "storm_gust",
				Label = "钢铁风暴·放出（刀扇）",
				Fixed = true,
				Settled = true,
				Default = 0,
				Options = new Option[1] { O("刀扇", "sts2_sfx_silent_fanofknives_v1", 0.23f, 0.28f, 0.45f) }
			}
		};

		private static readonly Dictionary<string, Slot> ByName = new Dictionary<string, Slot>();

		private static readonly Dictionary<string, int> Picked = new Dictionary<string, int>();

		private static readonly Dictionary<string, float> Knobs = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);

		private const int MaxVoices = 28;

		private static readonly string[] Kinds = new string[2] { ".ogg", ".wav" };

		private static readonly Dictionary<string, AudioStream> Files = new Dictionary<string, AudioStream>();

		private static readonly HashSet<string> NoFile = new HashSet<string>();

		private static readonly Dictionary<string, ulong> LastPlayed = new Dictionary<string, ulong>();

		private static readonly Dictionary<string, int> LastOne = new Dictionary<string, int>();

		private static readonly List<Voice> Voices = new List<Voice>();

		private static readonly List<Later> Queue = new List<Later>();

		private static readonly Dictionary<string, BedState> Beds = new Dictionary<string, BedState>();

		private static readonly Dictionary<string, Scatter> Scatters = new Dictionary<string, Scatter>();

		private static readonly HashSet<string> Outlives = new HashSet<string> { "hunt_trophy" };

		private static readonly List<string> ToLoad = new List<string>();

		private static readonly Random Dice = new Random();

		private static Node _root;

		private static StringName _bus;

		private static StringName _out;

		private static StringName _hall;

		private static StringName _low;

		private static bool _noRooms;

		private static double _stallSaid = -10.0;

		private static double _clock;

		private static double _real;

		private static bool _ticking;

		private static bool _settingsHooked;

		public static string Dir => Path.Combine(DevLog.ModDir, "sfx");

		private static string PicksPath => Path.Combine(Dir, "choices.ini");

		private static Option O(string label, string sample, float from = 0f, float lead = 0f, float gain = 1f, float speed = 1f, float to = 0f, string bank = "sfx")
		{
			return new Option
			{
				Label = label,
				Sample = sample,
				From = from,
				To = to,
				Lead = lead,
				Gain = gain,
				Speed = speed,
				Bank = bank
			};
		}

		private static Option L(string sample, float from, float gain, float speed = 1f, float after = 0f, float to = 0f, string bank = "sfx")
		{
			return new Option
			{
				Label = "",
				Sample = sample,
				From = from,
				To = to,
				Gain = gain,
				Speed = speed,
				After = after,
				Bank = bank
			};
		}

		private static string Stack(int k)
		{
			return "sts2_sfx_cardmvmt_stack_v1_rr" + k;
		}

		private static string Slide(int k)
		{
			return "sts2_sfx_cardmvmt_slide_v1_rr" + k;
		}

		private static Option Riffle(string label, int seed, int ticks, float span, float bend, float tickGain, float speed, float slideAt = -1f, int slides = 1, float slideGain = 1f, float tapAt = -1f)
		{
			Random random = new Random(seed);
			List<Option> list = new List<Option>();
			for (int i = 1; i < ticks; i++)
			{
				float num = (float)i / (float)Math.Max(1, ticks - 1);
				float num2 = span * Mathf.Pow(num, bend) + ((float)random.NextDouble() - 0.5f) * span / (float)ticks * 0.5f;
				float gain = tickGain * (0.65f + 0.35f * (float)random.NextDouble()) * (0.75f + 0.25f * Mathf.Sin((float)Math.PI * num));
				list.Add(L(Stack(random.Next(1, 11)), 0f, gain, speed * (0.92f + 0.16f * (float)random.NextDouble()), Mathf.Max(0.005f, num2), 0.16f));
			}
			for (int j = 0; j < slides; j++)
			{
				if (!(slideAt >= 0f))
				{
					break;
				}
				list.Add(L(Slide(random.Next(1, 11)), 0f, slideGain * (1f - 0.15f * (float)j), speed * (1f + 0.06f * (float)j), slideAt + 0.13f * (float)j));
			}
			if (tapAt >= 0f)
			{
				list.Add(L("sts2_sfx_ui_deck_close_v1_layer1_rr" + random.Next(1, 5), 0.03f, 0.22f, speed, tapAt));
			}
			return O(label, Stack(random.Next(1, 11)), 0f, 0f, tickGain * 0.8f, speed, 0.16f).Plus(list.ToArray());
		}

		private static string Slime(int k)
		{
			return "STS2_SFX_HitImpact_Sweetner_Slime_" + k + "_v1";
		}

		private static string Geyser(int k)
		{
			return "sts2_sfx_act1b_waterfall_giant_eruption_rr" + k + "_v1";
		}

		private static Option Pop(string label, int k, float gain = 1f, float speed = 1f, float after = 0f)
		{
			int num = Math.Clamp(k, 1, 4) - 1;
			Option option = O(label, "sts2_sfx_haunted_ship_cast_bubbles_v1", PopFrom[num], 0f, PopGain[num] * gain, speed, PopTo[num]).In(0.012f).Cut(0.03f);
			option.After = after;
			return option;
		}

		public static void Init()
		{
			try
			{
				if (!_settingsHooked)
				{
					PlayerSettings.Changed += SettingsChanged;
					_settingsHooked = true;
				}
				Slot[] slots = Slots;
				foreach (Slot slot in slots)
				{
					ByName[slot.Name] = slot;
				}
				ReadPicks();
				slots = Slots;
				foreach (Slot slot2 in slots)
				{
					string name = slot2.Name;
					AssetWarmup.Add("sound plan/" + name, () =>
					{
						QueueWarm(name);
					});
				}
				AssetWarmup.Add("sound buses", () =>
				{
					Bus("hall");
					Bus("low");
				});
				for (int num = 0; num < 8; num++)
				{
					AssetWarmup.Add("sound voice/" + num, () =>
					{
						if (Root() && Voices.Count < 8)
						{
							MakeVoice();
						}
					});
				}
				DevLog.Info($"Sfx ready: {Slots.Length} sound(s), {Picked.Count} picked by hand, {Knobs.Count} number(s) set in the panel");
			}
			catch (Exception ex)
			{
				DevLog.Warn("Sfx.Init: " + ex.Message);
			}
		}

		public static float Knob(string key, float def)
		{
			if (string.Equals(key, "sfx.on", StringComparison.OrdinalIgnoreCase))
			{
				if (!PlayerSettings.AdditionalAudioPreference)
				{
					return 0f;
				}
				return 1f;
			}
			if (string.Equals(key, "mood.on", StringComparison.OrdinalIgnoreCase))
			{
				if (!PlayerSettings.MusicPreference)
				{
					return 0f;
				}
				return 1f;
			}
			if (!Knobs.TryGetValue(key, out var value))
			{
				return Tuning.F(key, def);
			}
			return value;
		}

		public static void SetKnob(string key, float value)
		{
			if (string.Equals(key, "sfx.on", StringComparison.OrdinalIgnoreCase))
			{
				PlayerSettings.SetAdditionalAudioEnabled(value > 0.5f);
				return;
			}
			if (string.Equals(key, "mood.on", StringComparison.OrdinalIgnoreCase))
			{
				PlayerSettings.SetMusicEnabled(value > 0.5f);
				return;
			}
			Knobs[key] = value;
			WritePicks();
		}

		public static bool HasFile(string name)
		{
			return OwnFile(name) != null;
		}

		public static int Chosen(string name)
		{
			if (!ByName.TryGetValue(name, out var value) || value.Options.Length == 0)
			{
				return -1;
			}
			if (value.Fixed)
			{
				return Math.Clamp(value.Default, 0, value.Options.Length - 1);
			}
			if (!Picked.TryGetValue(name, out var value2))
			{
				return Math.Min(value.Default, value.Options.Length - 1);
			}
			return Math.Min(value2, value.Options.Length - 1);
		}

		private static Option Piece(string name)
		{
			int num = Chosen(name);
			if (num >= 0)
			{
				return ByName[name].Options[num];
			}
			return null;
		}

		public static string Choose(string name, int index)
		{
			try
			{
				if (!ByName.TryGetValue(name, out var value))
				{
					return "没有这个声音";
				}
				index = Math.Clamp(index, -1, value.Options.Length - 1);
				Picked[name] = index;
				WritePicks();
				if (value.Bed)
				{
					DropBed(name);
					if (index >= 0)
					{
						Hold(name, 1f, 1f, 3f);
					}
				}
				else if (index >= 0)
				{
					LastPlayed.Remove(name);
					Now(name, 1f, 1f);
				}
				if (index < 0)
				{
					return value.Label + "：不配";
				}
				Option option = value.Options[index];
				bool flag = StreamOf(option) != null;
				Warm(name);
				string text;
				if (HasFile(name))
				{
					text = "  注意：sfx 文件夹里有同名文件，实际用的是那个文件";
				}
				else
				{
					text = (flag ? "" : "  （没读到这个样本，见日志）");
				}
				return value.Label + " ← " + option.Label + (value.Bed ? "（试听 3 秒）" : "") + text;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Sfx.Choose (" + name + "): " + ex.Message);
				return "试听失败：" + ex.Message;
			}
		}

		public static string Audition(string name)
		{
			try
			{
				if (!ByName.TryGetValue(name, out var value))
				{
					return "没有这个声音";
				}
				if (value.Every > 0f)
				{
					Sprinkle(value, 1f);
					return value.Label + "：一声（战斗里每隔 " + Knob("sfx." + name + ".every", value.Every).ToString("0.#") + " 秒左右来一声）";
				}
				if (value.Bed)
				{
					DropBed(name);
					Hold(name, 1f, 1f, 3f);
					return value.Label + "（试听 3 秒）";
				}
				LastPlayed.Remove(name);
				Now(name, 1f, 1f);
				return value.Label;
			}
			catch (Exception ex)
			{
				DevLog.Warn("Sfx.Audition (" + name + "): " + ex.Message);
				return "试听失败：" + ex.Message;
			}
		}

		public static float Nudge(string name, float factor)
		{
			float num = Mathf.Clamp(Knob("sfx." + name, 1f) * factor, 0.1f, 8f);
			SetKnob("sfx." + name, Mathf.Round(num * 100f) / 100f);
			return Knob("sfx." + name, 1f);
		}

		private static void ReadPicks()
		{
			Picked.Clear();
			Knobs.Clear();
			try
			{
				string picksPath = PicksPath;
				if (!File.Exists(picksPath))
				{
					return;
				}
				string[] array = File.ReadAllLines(picksPath);
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i].Trim();
					int num = text.IndexOf('=');
					if (text.Length == 0 || text[0] == '#' || num <= 0)
					{
						continue;
					}
					string text2 = text.Substring(0, num).Trim();
					string text3 = text.Substring(num + 1).Trim();
					if (text2.StartsWith("set ", StringComparison.Ordinal))
					{
						if (float.TryParse(text3, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
						{
							Knobs[text2.Substring(4).Trim()] = result;
						}
					}
					else
					{
						if (!ByName.TryGetValue(text2, out var value) || value.Fixed)
						{
							continue;
						}
						if (text3 == "none")
						{
							Picked[text2] = -1;
							continue;
						}
						for (int j = 0; j < value.Options.Length; j++)
						{
							if (value.Options[j].Id == text3)
							{
								Picked[text2] = j;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Sfx: could not read the picks - " + ex.Message);
			}
		}

		private static void WritePicks()
		{
			try
			{
				Directory.CreateDirectory(Dir);
				List<string> list = new List<string> { "# SilentFX - what was chosen in the test panel (\"选音效\"). Delete this file to go back to the defaults.", "# name = which piece of the game's sound stands for it (sample@start), or none", "# set key = a number set in the panel (it wins over tuning.ini)" };
				Slot[] slots = Slots;
				foreach (Slot slot in slots)
				{
					if (!slot.Fixed && Picked.TryGetValue(slot.Name, out var value))
					{
						list.Add(slot.Name + " = " + ((value < 0) ? "none" : slot.Options[Math.Min(value, slot.Options.Length - 1)].Id));
					}
				}
				List<string> list2 = new List<string>(Knobs.Keys);
				list2.Sort(StringComparer.OrdinalIgnoreCase);
				foreach (string item in list2)
				{
					list.Add("set " + item + " = " + Knobs[item].ToString("0.###", CultureInfo.InvariantCulture));
				}
				File.WriteAllLines(PicksPath, list);
			}
			catch (Exception ex)
			{
				DevLog.Warn("Sfx: could not keep the picks - " + ex.Message);
			}
		}

		public static float Lead(string name)
		{
			try
			{
				if (OwnFile(name) != null)
				{
					return Mathf.Max(0f, Knob("sfx." + name + ".lead", 0f));
				}
				Option option = Piece(name);
				return (option == null) ? 0f : (option.Lead / Mathf.Max(0.1f, option.Speed));
			}
			catch
			{
				return 0f;
			}
		}

		public static void PlayAt(string name, float wait, float volume = 1f, bool evenFrozen = false)
		{
			Play(name, volume, Mathf.Max(0f, wait - Lead(name)), 1f, 0f, evenFrozen);
		}

		public static void Play(string name, float volume = 1f, float delay = 0f, float pitch = 1f, float vary = 0f, bool evenFrozen = false)
		{
			try
			{
				if (string.IsNullOrEmpty(name) || !PlayerSettings.AdditionalAudioEnabled)
				{
					return;
				}
				if (vary > 0f)
				{
					pitch *= 1f + ((float)Dice.NextDouble() * 2f - 1f) * vary;
				}
				if (delay > 0.004f)
				{
					Queue.Add(new Later
					{
						At = _clock + (double)delay,
						Name = name,
						Volume = volume,
						Pitch = pitch
					});
					Tick();
				}
				else
				{
					float v = volume;
					float p = pitch;
					if (evenFrozen || !TimeFreeze.Defer(() =>
					{
						Now(name, v, p);
					}))
					{
						Now(name, v, p);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Sfx.Play (" + name + "): " + ex.Message);
			}
		}

		public static void One(string name, float wait = 0f, float volume = 1f)
		{
			try
			{
				if (string.IsNullOrEmpty(name) || !PlayerSettings.AdditionalAudioEnabled || TimeFreeze.Active || !ByName.TryGetValue(name, out var value) || value.Options == null || value.Options.Length == 0)
				{
					return;
				}
				int num = Dice.Next(value.Options.Length);
				if (value.Options.Length > 1 && LastOne.TryGetValue(name, out var value2) && num == value2)
				{
					num = (num + 1) % value.Options.Length;
				}
				LastOne[name] = num;
				Option option = value.Options[num];
				float num2 = volume * Knob("sfx." + name, 1f);
				if (float.IsFinite(num2) && !(num2 <= 0.004f))
				{
					float pitch = 1f + ((float)Dice.NextDouble() * 2f - 1f) * 0.03f;
					float num3 = wait - option.Lead / Mathf.Max(0.1f, option.Speed);
					if (num3 > 0.004f)
					{
						Queue.Add(new Later
						{
							At = _clock + (double)num3,
							Name = name,
							Volume = num2,
							Pitch = pitch,
							Piece = option
						});
						Tick();
					}
					else
					{
						Layer(name, option, num2, pitch);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Sfx.One (" + name + "): " + ex.Message);
			}
		}

		public static void Stop(string name, float fade = 0.12f)
		{
			try
			{
				for (int num = Queue.Count - 1; num >= 0; num--)
				{
					if (Queue[num].Name == name)
					{
						Queue.RemoveAt(num);
					}
				}
				foreach (Voice voice in Voices)
				{
					if (!(voice.Name != name) && FxKit.Alive((GodotObject)(object)voice.Player) && voice.Player.Playing)
					{
						Fade(voice, fade);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Sfx.Stop (" + name + "): " + ex.Message);
			}
		}

		private static void SettingsChanged()
		{
			try
			{
				if (!PlayerSettings.AdditionalAudioEnabled)
				{
					Queue.Clear();
					Scatters.Clear();
					foreach (BedState value in Beds.Values)
					{
						value.Target = 0f;
						value.Until = 0.0;
						value.Out = 0.12f;
					}
					foreach (Voice voice in Voices)
					{
						if (voice.Name != null && FxKit.Alive((GodotObject)(object)voice.Player) && voice.Player.Playing)
						{
							Fade(voice, 0.12f);
						}
					}
					if (Beds.Count > 0)
					{
						Tick();
					}
					return;
				}
				float num = Mathf.Clamp(PlayerSettings.AudioVolume, 0f, 1f);
				foreach (Voice voice2 in Voices)
				{
					if (voice2.Name != null && FxKit.Alive((GodotObject)(object)voice2.Player) && voice2.Player.Playing)
					{
						Quiet(voice2);
						voice2.Player.VolumeLinear = Mathf.Clamp(voice2.BaseLevel * num, 0f, 8f);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Sfx settings changed: " + ex.Message);
			}
		}

		private static void Fade(Voice voice, float fade)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			AudioStreamPlayer player = voice.Player;
			voice.Name = null;
			voice.StopAt = -1.0;
			Quiet(voice);
			Tween val = ((Node)player).CreateTween();
			val.TweenProperty((GodotObject)(object)player, NodePath.op_Implicit("volume_db"), Variant.op_Implicit(-60f), (double)Mathf.Max(0.02f, fade));
			val.TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)player))
				{
					player.Stop();
				}
			})));
			voice.Tween = val;
		}

		private static void Quiet(Voice voice)
		{
			Tween tween = voice.Tween;
			voice.Tween = null;
			try
			{
				if (tween != null && tween.IsValid())
				{
					tween.Kill();
				}
			}
			catch
			{
			}
		}

		public static void Reset()
		{
			try
			{
				Queue.RemoveAll((Later later) => !Outlives.Contains(later.Name));
				foreach (Voice voice in Voices)
				{
					if (voice.Name == null || !Outlives.Contains(voice.Name))
					{
						voice.StopAt = -1.0;
						if (FxKit.Alive((GodotObject)(object)voice.Player) && voice.Player.Playing)
						{
							voice.Player.Stop();
						}
					}
				}
				foreach (BedState value in Beds.Values)
				{
					Silence(value);
				}
				Beds.Clear();
				Scatters.Clear();
				if (Queue.Count > 0)
				{
					Tick();
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Sfx.Reset: " + ex.Message);
			}
		}

		private static void Now(string name, float volume, float pitch)
		{
			if (!PlayerSettings.AdditionalAudioEnabled)
			{
				return;
			}
			float num = volume * Knob("sfx." + name, 1f);
			if (!float.IsFinite(num) || num <= 0.004f)
			{
				return;
			}
			ulong ticksMsec = Time.GetTicksMsec();
			if (LastPlayed.TryGetValue(name, out var value) && ticksMsec - value < 40)
			{
				return;
			}
			AudioStream val = OwnFile(name);
			Option option = null;
			if (val == null)
			{
				option = Piece(name);
				if (option == null)
				{
					return;
				}
				val = StreamOf(option);
				if (val == null)
				{
					return;
				}
			}
			float num2 = Start(name, val, option, num, pitch);
			if (num2 <= 0f)
			{
				return;
			}
			LastPlayed[name] = ticksMsec;
			int num3 = 0;
			if (option?.With != null)
			{
				Option[] with = option.With;
				foreach (Option option2 in with)
				{
					if (option2 != null)
					{
						num3++;
						if (option2.After <= 0.004f)
						{
							Layer(name, option2, num, pitch);
							continue;
						}
						Queue.Add(new Later
						{
							At = _clock + (double)option2.After,
							Name = name,
							Volume = num,
							Pitch = pitch,
							Piece = option2
						});
					}
				}
				if (num3 > 0)
				{
					Tick();
				}
			}
			if (PowerFxManager.LogTriggers)
			{
				DevLog.Info($"Sfx: {name} at {num:F2} - {((option == null) ? "the player's own file" : (option.Sample + " from " + option.From.ToString("0.##", CultureInfo.InvariantCulture) + " s"))}, loud point {((option == null) ? Lead(name) : (option.Lead / num2)):F2} s in{((num3 > 0) ? $", {num3} more laid over it" : "")}");
			}
		}

		private static void QueueWarm(string name)
		{
			if (OwnFile(name) != null || !ByName.TryGetValue(name, out var value))
			{
				return;
			}
			if (value.Every > 0f)
			{
				Option[] options = value.Options;
				for (int i = 0; i < options.Length; i++)
				{
					QueuePiece(options[i]);
				}
			}
			else
			{
				QueuePiece(Piece(name));
			}
		}

		private static void QueuePiece(Option piece)
		{
			if (piece == null)
			{
				return;
			}
			AssetWarmup.Add("sound/" + piece.Bank + "/" + piece.Sample, () =>
			{
				if (StreamOf(piece) == null)
				{
					throw new IOException("game sound unavailable: " + piece.Sample);
				}
			});
			if (piece.With != null)
			{
				Option[] with = piece.With;
				for (int num = 0; num < with.Length; num++)
				{
					QueuePiece(with[num]);
				}
			}
		}

		private static void Warm(string name)
		{
			if (!ByName.TryGetValue(name, out var value))
			{
				return;
			}
			Option[] options;
			if (value.Every > 0f)
			{
				options = value.Options;
				foreach (Option option in options)
				{
					StreamOf(option);
					if (option.With == null)
					{
						continue;
					}
					Option[] with = option.With;
					foreach (Option option2 in with)
					{
						if (option2 != null)
						{
							StreamOf(option2);
						}
					}
				}
				return;
			}
			Option option3 = Piece(name);
			if (option3 == null)
			{
				return;
			}
			StreamOf(option3);
			if (option3.With == null)
			{
				return;
			}
			options = option3.With;
			foreach (Option option4 in options)
			{
				if (option4 != null)
				{
					StreamOf(option4);
				}
			}
		}

		private static AudioStream StreamOf(Option piece)
		{
			if (piece != null)
			{
				return GameSounds.Get(piece.Bank, piece.Sample);
			}
			return null;
		}

		private static void Layer(string name, Option piece, float level, float pitch)
		{
			AudioStream val = StreamOf(piece);
			if (val != null)
			{
				Start(name, val, piece, level, pitch);
			}
		}

		private static float Start(string name, AudioStream stream, Option piece, float level, float pitch)
		{
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			if (!PlayerSettings.AdditionalAudioEnabled)
			{
				return 0f;
			}
			Voice voice = Free();
			if (voice == null)
			{
				return 0f;
			}
			AudioStreamPlayer player = voice.Player;
			Quiet(voice);
			float num = Mathf.Clamp(pitch * (piece?.Speed ?? 1f), 0.25f, 4f);
			voice.BaseLevel = Mathf.Clamp(level * (piece?.Gain ?? 1f), 0f, 8f);
			float num2 = Mathf.Clamp(voice.BaseLevel * PlayerSettings.AudioVolume, 0f, 8f);
			player.Stream = stream;
			player.Bus = Bus(piece?.Via);
			player.PitchScale = num;
			if (piece != null && piece.Soft > 0.004f)
			{
				player.VolumeLinear = 0.0001f;
				Tween val = ((Node)player).CreateTween();
				val.TweenProperty((GodotObject)(object)player, NodePath.op_Implicit("volume_linear"), Variant.op_Implicit(num2), (double)piece.Soft);
				voice.Tween = val;
			}
			else
			{
				player.VolumeLinear = num2;
			}
			player.Play(piece?.From ?? 0f);
			voice.Name = name;
			voice.Since = Time.GetTicksMsec();
			voice.StopAt = ((piece != null && piece.To > piece.From) ? (_real + (double)((piece.To - piece.From) / num)) : (-1.0));
			voice.Out = Mathf.Clamp(piece?.Tail ?? 0.05f, 0.01f, 1f);
			if (voice.StopAt > 0.0)
			{
				Tick();
			}
			return num;
		}

		private static StringName Bus(string via = null)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected Obj, but got Unknown
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Expected Obj, but got Unknown
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Expected Obj, but got Unknown
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Expected Obj, but got Unknown
			if (_bus == null)
			{
				_bus = new StringName((AudioServer.GetBusIndex(StringName.op_Implicit("SFX")) >= 0) ? "SFX" : "Master");
			}
			if (_noRooms)
			{
				return _bus;
			}
			try
			{
				if (_out == null)
				{
					_out = Room("SilentFX_Out", _bus, (AudioEffect)new AudioEffectHardLimiter
					{
						CeilingDb = Mathf.Clamp(Knob("sfx.ceiling", -1.5f), -12f, -0.1f),
						PreGainDb = 0f,
						Release = 0.15f
					});
				}
				if (via == "hall")
				{
					return _hall ?? (_hall = Room("SilentFX_Hall", _out, (AudioEffect)new AudioEffectReverb
					{
						RoomSize = Mathf.Clamp(Knob("sfx.hall.room", 0.85f), 0f, 1f),
						Damping = Mathf.Clamp(Knob("sfx.hall.damping", 0.35f), 0f, 1f),
						Spread = 1f,
						Hipass = 0.15f,
						PredelayMsec = 60f,
						PredelayFeedback = 0.2f,
						Dry = 1f,
						Wet = Mathf.Clamp(Knob("sfx.hall.wet", 0.33f), 0f, 1f)
					}));
				}
				if (via == "low")
				{
					return _low ?? (_low = Room("SilentFX_Low", _out, (AudioEffect)new AudioEffectLowPassFilter
					{
						CutoffHz = Mathf.Clamp(Knob("sfx.low.cutoff", 420f), 60f, 4000f),
						Resonance = 0.5f,
						Db = (FilterDB)3
					}));
				}
				return _out;
			}
			catch (Exception ex)
			{
				_noRooms = true;
				DevLog.Warn("Sfx: no buses of its own (" + ex.Message + ") - everything goes straight to " + StringName.op_Implicit(_bus));
			}
			return _bus;
		}

		private static StringName Room(string name, StringName into, params AudioEffect[] effects)
		{
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Expected Obj, but got Unknown
			int busIndex = AudioServer.GetBusIndex(StringName.op_Implicit(name));
			if (busIndex < 0)
			{
				AudioServer.AddBus(-1);
				busIndex = AudioServer.BusCount - 1;
				AudioServer.SetBusName(busIndex, name);
				AudioServer.SetBusSend(busIndex, into);
				foreach (AudioEffect val in effects)
				{
					AudioServer.AddBusEffect(busIndex, val, -1);
				}
				if (PowerFxManager.LogTriggers)
				{
					DevLog.Info($"Sfx: made the bus {name} ({effects.Length} effect(s)), feeding {into}");
				}
			}
			return new StringName(name);
		}

		private static AudioStream OwnFile(string name)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Expected Obj, but got Unknown
			if (Files.TryGetValue(name, out var value))
			{
				return value;
			}
			if (NoFile.Contains(name))
			{
				return null;
			}
			AudioStream val = null;
			try
			{
				string[] kinds = Kinds;
				foreach (string text in kinds)
				{
					string text2 = Path.Combine(Dir, name + text);
					if (File.Exists(text2))
					{
						text2 = text2.Replace('\\', '/');
						val = (AudioStream)((text == ".ogg") ? ((object)AudioStreamOggVorbis.LoadFromFile(text2)) : ((object)AudioStreamWav.LoadFromFile(text2, new Dictionary())));
						if (val != null)
						{
							break;
						}
						DevLog.Warn("Sfx: could not read " + text2);
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Sfx: " + name + " - " + ex.Message);
			}
			if (val == null)
			{
				NoFile.Add(name);
				return null;
			}
			Files[name] = val;
			return val;
		}

		private static bool Root()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Expected Obj, but got Unknown
			if (FxKit.Alive((GodotObject)(object)_root) && _root.IsInsideTree())
			{
				return true;
			}
			SceneTree tree = Ticker.Tree;
			if (((tree != null) ? tree.Root : null) == null)
			{
				return false;
			}
			Voices.Clear();
			Beds.Clear();
			_root = new Node
			{
				Name = StringName.op_Implicit("SilentFX_Sfx")
			};
			((Node)tree.Root).AddChild(_root, false, (InternalMode)0);
			return true;
		}

		private static Voice Free()
		{
			if (!Root())
			{
				return null;
			}
			Voice voice = null;
			for (int num = Voices.Count - 1; num >= 0; num--)
			{
				Voice voice2 = Voices[num];
				if (!FxKit.Alive((GodotObject)(object)voice2.Player))
				{
					Voices.RemoveAt(num);
				}
				else
				{
					if (!voice2.Player.Playing)
					{
						return voice2;
					}
					if (voice == null || voice2.Since < voice.Since)
					{
						voice = voice2;
					}
				}
			}
			if (Voices.Count >= 28 && voice != null)
			{
				voice.Player.Stop();
				return voice;
			}
			return MakeVoice();
		}

		private static Voice MakeVoice()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected Obj, but got Unknown
			AudioStreamPlayer val = new AudioStreamPlayer
			{
				Name = StringName.op_Implicit("Voice" + Voices.Count)
			};
			_root.AddChild((Node)(object)val, false, (InternalMode)0);
			Voice voice = new Voice
			{
				Player = val
			};
			Voices.Add(voice);
			return voice;
		}

		public static void Hold(string name, float level, float pitch = 1f, float life = 0.25f)
		{
			try
			{
				if (string.IsNullOrEmpty(name) || !PlayerSettings.AdditionalAudioEnabled)
				{
					return;
				}
				ByName.TryGetValue(name, out var value);
				if (value != null && value.Every > 0f)
				{
					if (!Scatters.TryGetValue(name, out var value2))
					{
						if (level <= 0.05f)
						{
							return;
						}
						value2 = new Scatter
						{
							Name = name,
							Next = _real + (double)Mathf.Max(0.05f, Knob("sfx." + name + ".first", value.First))
						};
						Scatters[name] = value2;
					}
					value2.Level = Mathf.Clamp(level, 0f, 1.5f);
					value2.Until = Math.Max(value2.Until, _real + (double)Mathf.Max(0.05f, life));
					Tick();
					return;
				}
				if (!Beds.TryGetValue(name, out var value3))
				{
					if (level <= 0.004f)
					{
						return;
					}
					value3 = new BedState
					{
						Name = name
					};
					Beds[name] = value3;
				}
				value3.In = value?.FadeIn ?? 0.35f;
				value3.Out = value?.FadeOut ?? 0.45f;
				value3.Target = Mathf.Clamp(level, 0f, 1.5f);
				value3.Pitch = Mathf.Clamp(pitch, 0.4f, 2.5f);
				value3.Until = Math.Max(value3.Until, _real + (double)Mathf.Max(0.05f, life));
				Tick();
			}
			catch (Exception ex)
			{
				DevLog.Warn("Sfx.Hold (" + name + "): " + ex.Message);
			}
		}

		public static void Hush(string name, float fade = 0.05f)
		{
			if (name != null && Beds.TryGetValue(name, out var value))
			{
				value.Target = 0f;
				value.Until = 0.0;
				value.Out = Mathf.Max(0.02f, fade);
			}
		}

		private static void Sprinkle(Slot slot, float level, Scatter state = null)
		{
			if (!PlayerSettings.AdditionalAudioEnabled || slot.Options.Length == 0)
			{
				return;
			}
			int num = Dice.Next(slot.Options.Length);
			if (state != null && slot.Options.Length > 1 && num == state.Last)
			{
				num = (num + 1) % slot.Options.Length;
			}
			if (state != null)
			{
				state.Last = num;
			}
			Option option = slot.Options[num];
			AudioStream val = StreamOf(option);
			if (val == null)
			{
				return;
			}
			float num2 = level * Knob("sfx." + slot.Name, 1f) * (0.7f + 0.3f * (float)Dice.NextDouble());
			if (!float.IsFinite(num2) || num2 <= 0.004f)
			{
				return;
			}
			float pitch = 1f + ((float)Dice.NextDouble() * 2f - 1f) * 0.08f;
			if (Start(slot.Name, val, option, num2, pitch) <= 0f)
			{
				return;
			}
			if (option.With != null)
			{
				bool flag = false;
				Option[] with = option.With;
				foreach (Option option2 in with)
				{
					if (option2 != null)
					{
						if (option2.After <= 0.004f)
						{
							Layer(slot.Name, option2, num2, pitch);
							continue;
						}
						Queue.Add(new Later
						{
							At = _clock + (double)option2.After,
							Name = slot.Name,
							Volume = num2,
							Pitch = pitch,
							Piece = option2
						});
						flag = true;
					}
				}
				if (flag)
				{
					Tick();
				}
			}
			if (PowerFxManager.LogTriggers)
			{
				DevLog.Info($"Sfx: {slot.Name} (now and then) - {option.Label}, at {num2:F2}");
			}
		}

		private static void DropBed(string name)
		{
			if (Beds.TryGetValue(name, out var value))
			{
				Silence(value);
				Beds.Remove(name);
			}
		}

		private static void Silence(BedState bed)
		{
			if (FxKit.Alive((GodotObject)(object)bed.A))
			{
				((Node)bed.A).QueueFree();
			}
			if (FxKit.Alive((GodotObject)(object)bed.B))
			{
				((Node)bed.B).QueueFree();
			}
			bed.A = (bed.B = null);
		}

		private static bool Arm(BedState bed)
		{
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Expected Obj, but got Unknown
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Expected Obj, but got Unknown
			if (bed.Stream != null && FxKit.Alive((GodotObject)(object)bed.A) && FxKit.Alive((GodotObject)(object)bed.B))
			{
				return true;
			}
			if (!Root())
			{
				return false;
			}
			bed.Stream = OwnFile(bed.Name);
			bed.Piece = null;
			if (bed.Stream == null)
			{
				bed.Piece = Piece(bed.Name);
				if (bed.Piece == null)
				{
					return false;
				}
				bed.Stream = StreamOf(bed.Piece);
				if (bed.Stream == null)
				{
					return false;
				}
			}
			StringName bus = Bus(bed.Piece?.Via);
			bed.A = new AudioStreamPlayer
			{
				Name = StringName.op_Implicit("Bed_" + bed.Name + "_A"),
				Stream = bed.Stream,
				Bus = bus,
				VolumeLinear = 0f
			};
			bed.B = new AudioStreamPlayer
			{
				Name = StringName.op_Implicit("Bed_" + bed.Name + "_B"),
				Stream = bed.Stream,
				Bus = bus,
				VolumeLinear = 0f
			};
			_root.AddChild((Node)(object)bed.A, false, (InternalMode)0);
			_root.AddChild((Node)(object)bed.B, false, (InternalMode)0);
			bed.Turn = 0.0;
			bed.PhaseA = (bed.PhaseB = -1f);
			return true;
		}

		private static void Weave(BedState bed, double dt)
		{
			bool flag = bed.Until >= _real && bed.Target > 0.004f;
			float num = (flag ? bed.Target : 0f);
			bed.Level = Mathf.MoveToward(bed.Level, num, (float)dt / Mathf.Max(0.02f, (num > bed.Level) ? bed.In : bed.Out));
			if (bed.Level <= 0.0005f && !flag)
			{
				Silence(bed);
			}
			else if (Arm(bed))
			{
				float num2 = bed.Piece?.From ?? 0f;
				float num3 = (float)bed.Stream.GetLength();
				float num4 = ((bed.Piece != null && bed.Piece.To > num2) ? Mathf.Min(bed.Piece.To, num3 - 0.05f) : (num3 - 0.05f));
				float num5 = Mathf.Clamp(bed.Pitch * (bed.Piece?.Speed ?? 1f), 0.25f, 4f);
				float num6 = Mathf.Max(0.3f, num4 - num2);
				bed.Turn += dt * (double)num5;
				float num7 = (float)(bed.Turn / (double)num6 % 1.0);
				float phase = (num7 + 0.5f) % 1f;
				float loud = bed.Level * Knob("sfx." + bed.Name, 1f) * (bed.Piece?.Gain ?? 1f) * PlayerSettings.AudioVolume;
				Pass(bed.A, num7, ref bed.PhaseA, num2, num6, num5, loud);
				Pass(bed.B, phase, ref bed.PhaseB, num2, num6, num5, loud);
				if (!bed.Told && PowerFxManager.LogTriggers)
				{
					bed.Told = true;
					DevLog.Info($"Sfx: {bed.Name} (a bed) - {((bed.Piece == null) ? "the player's own file" : bed.Piece.Sample)}, {num6:F1} s woven");
				}
			}
		}

		private static void Pass(AudioStreamPlayer player, float phase, ref float was, float from, float span, float speed, float loud)
		{
			if (FxKit.Alive((GodotObject)(object)player))
			{
				player.PitchScale = speed;
				if (was < 0f || phase < was || !player.Playing)
				{
					player.Play(from + phase * span);
				}
				was = phase;
				float num = Mathf.Sin((float)Math.PI * phase);
				player.VolumeLinear = Mathf.Clamp(loud * num * num, 0f, 8f);
			}
		}

		private static void Tick()
		{
			if (!_ticking)
			{
				_ticking = true;
				Ticker.Add(Update);
			}
		}

		private static void Update(double realDt)
		{
			try
			{
				if (ToLoad.Count > 0)
				{
					string name = ToLoad[ToLoad.Count - 1];
					ToLoad.RemoveAt(ToLoad.Count - 1);
					if (OwnFile(name) == null)
					{
						Warm(name);
					}
					if (ToLoad.Count == 0)
					{
						Bus("hall");
						Bus("low");
					}
				}
				bool flag = Ticker.Tree != null && Ticker.Tree.Paused;
				if (!flag)
				{
					_real += realDt;
				}
				if (realDt > 0.1 && !flag && _real - _stallSaid > 1.0 && PowerFxManager.LogTriggers)
				{
					_stallSaid = _real;
					DevLog.Info($"Sfx: a long frame - {realDt * 1000.0:F0} ms - while the mod's sounds were going");
				}
				double num = realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f);
				if (double.IsFinite(num) && num > 0.0)
				{
					_clock += num;
				}
				for (int num2 = Queue.Count - 1; num2 >= 0; num2--)
				{
					Later later = Queue[num2];
					if (!(later.At > _clock))
					{
						Queue.RemoveAt(num2);
						if (later.Piece != null)
						{
							Layer(later.Name, later.Piece, later.Volume, later.Pitch);
						}
						else
						{
							Play(later.Name, later.Volume, 0f, later.Pitch);
						}
					}
				}
				bool flag2 = false;
				foreach (Voice voice in Voices)
				{
					if (!(voice.StopAt < 0.0))
					{
						if (!FxKit.Alive((GodotObject)(object)voice.Player) || !voice.Player.Playing)
						{
							voice.StopAt = -1.0;
						}
						else if (voice.StopAt - (double)voice.Out <= _real)
						{
							Fade(voice, voice.Out);
						}
						else
						{
							flag2 = true;
						}
					}
				}
				if (Beds.Count > 0 && !flag)
				{
					List<string> list = null;
					foreach (BedState value2 in Beds.Values)
					{
						Weave(value2, realDt);
						if (value2.Level <= 0.0005f && value2.Until < _real)
						{
							(list ?? (list = new List<string>())).Add(value2.Name);
						}
					}
					if (list != null)
					{
						foreach (string item in list)
						{
							DropBed(item);
							if (PowerFxManager.LogTriggers)
							{
								DevLog.Info("Sfx: " + item + " (a bed) has died away");
							}
						}
					}
				}
				if (Scatters.Count > 0 && !flag)
				{
					List<string> list2 = null;
					foreach (Scatter value3 in Scatters.Values)
					{
						if (value3.Until < _real || !ByName.TryGetValue(value3.Name, out var value))
						{
							(list2 ?? (list2 = new List<string>())).Add(value3.Name);
						}
						else if (!(_real < value3.Next) && !(value3.Level <= 0.05f) && !TimeFreeze.Active)
						{
							Sprinkle(value, value3.Level, value3);
							float num3 = Mathf.Max(0.15f, Knob("sfx." + value3.Name + ".every", value.Every));
							value3.Next = _real + (double)num3 * (0.699999988079071 + 0.699999988079071 * Dice.NextDouble());
						}
					}
					if (list2 != null)
					{
						foreach (string item2 in list2)
						{
							Scatters.Remove(item2);
						}
					}
				}
				if (Queue.Count == 0 && ToLoad.Count == 0 && Beds.Count == 0 && Scatters.Count == 0 && !flag2)
				{
					Ticker.Remove(Update);
					_ticking = false;
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("Sfx.Update: " + ex.Message);
				Queue.Clear();
				ToLoad.Clear();
				foreach (BedState value4 in Beds.Values)
				{
					Silence(value4);
				}
				Beds.Clear();
				Scatters.Clear();
				Ticker.Remove(Update);
				_ticking = false;
			}
		}
	}
