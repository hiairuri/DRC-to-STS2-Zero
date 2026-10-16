// namespace: SilentFX.Cards
// type: NoxiousCloudFx (class)
// source lines: 81022-81234
	public sealed class NoxiousCloudFx : CardFx
	{
		public override void OnPlay(CardFxContext c)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (c == null || Tuning.F("intro.on", 1f) < 0.5f || !FxKit.Alive((GodotObject)(object)c.FrontParent) || !FxKit.Alive((GodotObject)(object)c.BackParent))
				{
					return;
				}
				try
				{
					bool flag = false;
					float[] array = new float[2] { 1f, -1f };
					foreach (float side in array)
					{
						if (NoxiousFumesFx.RegionOf(c.Enemies, side, c.Chest, c.Feet, c.Body, out var near, out var far, out var floor))
						{
							Sheet(c, side, near, far, floor);
							flag = true;
						}
					}
					if (!flag)
					{
						float num = ((c.Facing < 0f) ? (-1f) : 1f);
						float far2 = ((num > 0f) ? (c.View.X + 140f) : (-140f));
						Sheet(c, num, c.Chest.X + num * (c.Body.X * 0.8f + 320f), far2, c.Feet.Y);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("Noxious Fumes opening failed: " + ex.Message);
				}
			}
			catch (Exception ex2)
			{
				DevLog.Warn("NoxiousCloudFx.OnPlay: " + ex2);
			}
		}

		private static void Sheet(CardFxContext c, float side, float near, float far, float floor)
		{
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_041c: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_047b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0481: Unknown result type (might be due to invalid IL or missing references)
			//IL_049d: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_034f: Unknown result type (might be due to invalid IL or missing references)
			Node2D back = FogKit.Quad(c.BackParent, "SilentFX_NoxiousSheet", out var quad, out var backMat);
			Node2D front = FogKit.Quad(c.FrontParent, "SilentFX_NoxiousSheetVeil", out var quad2, out var frontMat);
			if (back == null)
			{
				if (FxKit.Alive((GodotObject)(object)front))
				{
					((Node)front).QueueFree();
				}
				return;
			}
			float num = NoxiousFumesFx.SheetTravel + NoxiousFumesFx.SheetSettle + 0.1f;
			FxLifetime.Watch((Node)(object)back, num, () =>
			{
				if (FxKit.Alive((GodotObject)(object)front))
				{
					((Node)front).QueueFree();
				}
			});
			if (FxKit.Alive((GodotObject)(object)front))
			{
				FxKit.FreeAfter((Node)(object)front, num);
			}
			float num2 = Mathf.Clamp(c.Body.Y / 244f, 0.6f, 1.3f);
			float num3 = NoxiousFumesFx.RiseFor(c.Body);
			float rise = num3 * Tuning.F("fumes.sheet_rise", 0.9f);
			Vector2 mouth = c.Chest + new Vector2(side * c.Body.X * 0.4f, (0f - c.Body.Y) * Tuning.F("fumes.sheet_mouth", 0.22f));
			float length = Mathf.Max(360f, (far - mouth.X) * side);
			float settleAt = Mathf.Clamp((near - mouth.X) * side, 0f, length - 240f);
			float num4 = Mathf.Min(floor - num3 * 1.75f - 50f, mouth.Y - 130f);
			Vector2 topLeft = new Vector2((side > 0f) ? mouth.X : (mouth.X - length), num4);
			Vector2 size = new Vector2(length, floor + 110f - num4);
			float fan = Mathf.Clamp(settleAt + 80f, 280f, 560f);
			float num5 = NoxiousFumesFx.SeedOf(side);
			FogKit.Place(back, quad, backMat, topLeft, size);
			FogKit.Place(front, quad2, frontMat, topLeft, size);
			Dress(backMat, 0f, side, num5, floor - num4, rise, mouth.Y - num4, fan);
			Dress(frontMat, 1f, side, num5 + 7.3f, floor - num4, rise, mouth.Y - num4, fan);
			Color venom = PoisonFx.Venom;
			Sprite2D light = null;
			if (FxKit.Alive((GodotObject)(object)front))
			{
				light = FxKit.Glow(new Color(venom.R, venom.G, venom.B, 0f), new Vector2(320f * num2, rise * 1.5f));
				((Node)front).AddChild((Node)(object)light, false, (InternalMode)0);
			}
			float travel = NoxiousFumesFx.SheetTravel;
			float settle = NoxiousFumesFx.SheetSettle;
			float total = travel + settle;
			float peak = Tuning.F("fumes.sheet_alpha", 0.64f);
			float veil = Tuning.F("fumes.sheet_veil", 0.3f);
			float drift = Tuning.F("fumes.fog_drift", 46f);
			float lightPeak = Tuning.F("fumes.sheet_light", 0.2f);
			FxKit.Drive((Node)(object)back, total, (float u) =>
			{
				//IL_0243: Unknown result type (might be due to invalid IL or missing references)
				//IL_0295: Unknown result type (might be due to invalid IL or missing references)
				if (FxKit.Alive((GodotObject)(object)back))
				{
					float num6 = u * total;
					float num7 = Mathf.Clamp(num6 / travel, 0f, 1f);
					float num8 = 1f - Mathf.Pow(1f - num7, 2.4f);
					float num9 = -80f + (length + 320f) * num8;
					float num10 = Mathf.Clamp((num6 - travel * 0.28f) / (travel * 0.95f), 0f, 1f);
					float num11 = num10 * num10 * (3f - 2f * num10);
					float tail = -320f + (settleAt + 360f) * num11;
					float num12 = Mathf.Clamp((num6 - travel * 0.5f) / (travel * 0.5f + settle * 0.35f), 0f, 1f);
					float cone = 1f - num12 * num12 * (3f - 2f * num12);
					float num13 = Mathf.Clamp((num6 - travel * 0.85f) / (total - travel * 0.85f), 0f, 1f);
					float num14 = num13 * num13 * (3f - 2f * num13);
					float num15 = Mathf.Min(1f, num6 / 0.1f) * (1f - num14);
					float scroll = num9 * 0.55f + drift * num6;
					Feed(backMat, peak * num15, num9, tail, 1f - num14, cone, scroll, num6);
					Feed(frontMat, veil * num15, num9, tail, 1f - num14, cone, scroll, num6);
					if (FxKit.Alive((GodotObject)(object)light))
					{
						float num16 = Mathf.Clamp(num9 - 70f, 0f, length);
						((Node2D)light).GlobalPosition = new Vector2(mouth.X + side * num16, Mathf.Lerp(mouth.Y, floor - rise * 0.4f, Mathf.Min(1f, num16 / fan)));
						((CanvasItem)light).Modulate = new Color(venom.R, venom.G, venom.B, lightPeak * Mathf.Sin((float)Math.PI * Mathf.Sqrt(num7)) * (1f - num14));
					}
				}
			}, (TransitionType)0, (EaseType)2).TweenCallback(Callable.From((Action)(() =>
			{
				if (FxKit.Alive((GodotObject)(object)front))
				{
					((Node)front).QueueFree();
				}
				if (FxKit.Alive((GodotObject)(object)back))
				{
					((Node)back).QueueFree();
				}
			})));
			Node frontParent = c.FrontParent;
			FxKit.Flash(frontParent, mouth, new Color(venom.R, venom.G, venom.B, 0.4f), 84f * num2, 2f, 0.26);
			Brush.Gust(frontParent, mouth, mouth + new Vector2(side * 320f * num2, 50f * num2), 4, 22f * num2, 9f * num2, new Color(0.66f, 1f, 0.48f, 0.42f), 0.24f);
		}

		private static void Dress(ShaderMaterial mat, float layer, float side, float seed, float floorY, float rise, float mouthY, float fan)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)mat))
			{
				mat.SetShaderParameter(StringName.op_Implicit("shape"), Variant.op_Implicit(0f));
				mat.SetShaderParameter(StringName.op_Implicit("layer"), Variant.op_Implicit(layer));
				mat.SetShaderParameter(StringName.op_Implicit("dir"), Variant.op_Implicit(side));
				mat.SetShaderParameter(StringName.op_Implicit("seed"), Variant.op_Implicit(seed));
				mat.SetShaderParameter(StringName.op_Implicit("floor_y"), Variant.op_Implicit(floorY));
				mat.SetShaderParameter(StringName.op_Implicit("rise"), Variant.op_Implicit(rise));
				mat.SetShaderParameter(StringName.op_Implicit("mouth"), Variant.op_Implicit(mouthY));
				mat.SetShaderParameter(StringName.op_Implicit("fan"), Variant.op_Implicit(fan));
				mat.SetShaderParameter(StringName.op_Implicit("near_soft"), Variant.op_Implicit(36f));
				mat.SetShaderParameter(StringName.op_Implicit("far_soft"), Variant.op_Implicit(120f));
				mat.SetShaderParameter(StringName.op_Implicit("thick"), Variant.op_Implicit(1.12f));
			}
		}

		private static void Feed(ShaderMaterial mat, float alpha, float head, float tail, float roll, float cone, float scroll, float time)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)mat))
			{
				mat.SetShaderParameter(StringName.op_Implicit("intensity"), Variant.op_Implicit(Mathf.Clamp(alpha, 0f, 1f)));
				mat.SetShaderParameter(StringName.op_Implicit("head"), Variant.op_Implicit(head));
				mat.SetShaderParameter(StringName.op_Implicit("tail"), Variant.op_Implicit(tail));
				mat.SetShaderParameter(StringName.op_Implicit("roll"), Variant.op_Implicit(roll));
				mat.SetShaderParameter(StringName.op_Implicit("cone"), Variant.op_Implicit(cone));
				mat.SetShaderParameter(StringName.op_Implicit("scroll"), Variant.op_Implicit(scroll));
				mat.SetShaderParameter(StringName.op_Implicit("churn"), Variant.op_Implicit(time));
			}
		}
	}
