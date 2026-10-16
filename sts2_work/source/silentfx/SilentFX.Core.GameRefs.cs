// namespace: SilentFX.Core
// type: GameRefs (class)
// source lines: 37163-37571
	public static class GameRefs
	{
		public static Control EnergyCounter
		{
			get
			{
				try
				{
					NCombatRoom combatRoom = CombatRoom;
					object obj;
					if (combatRoom == null)
					{
						obj = null;
					}
					else
					{
						NCombatUi ui = combatRoom.Ui;
						obj = ((ui != null) ? ui.EnergyCounterContainer : null);
					}
					Control val = (Control)obj;
					if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree())
					{
						return null;
					}
					for (int i = 0; i < ((Node)val).GetChildCount(false); i++)
					{
						Node child = ((Node)val).GetChild(i, false);
						NEnergyCounter val2 = (NEnergyCounter)(object)((child is NEnergyCounter) ? child : null);
						if (val2 != null && FxKit.Alive((GodotObject)(object)val2) && ((Node)val2).IsInsideTree())
						{
							return (Control)(object)val2;
						}
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("GameRefs.EnergyCounter: " + ex);
				}
				return null;
			}
		}

		public static Node Overlay
		{
			get
			{
				try
				{
					NRun instance = NRun.Instance;
					if (!FxKit.Alive((GodotObject)(object)instance))
					{
						return null;
					}
					NGlobalUi globalUi = instance.GlobalUi;
					if (!FxKit.Alive((GodotObject)(object)globalUi))
					{
						return null;
					}
					Control aboveTopBarVfxContainer = globalUi.AboveTopBarVfxContainer;
					return (Node)(object)((FxKit.Alive((GodotObject)(object)aboveTopBarVfxContainer) && ((Node)aboveTopBarVfxContainer).IsInsideTree()) ? aboveTopBarVfxContainer : null);
				}
				catch (Exception ex)
				{
					DevLog.Warn("GameRefs.Overlay: " + ex);
				}
				return null;
			}
		}

		public static NCombatRoom CombatRoom
		{
			get
			{
				try
				{
					NGame instance = NGame.Instance;
					if (!FxKit.Alive((GodotObject)(object)instance))
					{
						return null;
					}
					NRun currentRunNode = instance.CurrentRunNode;
					if (!FxKit.Alive((GodotObject)(object)currentRunNode))
					{
						return null;
					}
					NCombatRoom combatRoom = currentRunNode.CombatRoom;
					return (FxKit.Alive((GodotObject)(object)combatRoom) && ((Node)combatRoom).IsInsideTree()) ? combatRoom : null;
				}
				catch
				{
					return null;
				}
			}
		}

		public static Node UiLayer
		{
			get
			{
				try
				{
					NCombatRoom combatRoom = CombatRoom;
					NCombatUi val = ((combatRoom != null) ? combatRoom.Ui : null);
					return (Node)(object)(FxKit.Alive((GodotObject)(object)val) ? val : null);
				}
				catch
				{
					return null;
				}
			}
		}

		public static int EnergyOf(Player player)
		{
			try
			{
				int? num;
				if (player == null)
				{
					num = null;
				}
				else
				{
					PlayerCombatState playerCombatState = player.PlayerCombatState;
					num = ((playerCombatState != null) ? new int?(playerCombatState.Energy) : ((int?)null));
				}
				int? num2 = num;
				return num2.GetValueOrDefault();
			}
			catch (Exception ex)
			{
				DevLog.Warn("GameRefs.EnergyOf: " + ex);
			}
			return 0;
		}

		public static Vector2 DrawnSize(Control box, Node fxParent)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			if (!FxKit.Alive((GodotObject)(object)box))
			{
				return Vector2.Zero;
			}
			Vector2 val = box.Size;
			try
			{
				Transform2D globalTransform = ((CanvasItem)box).GetGlobalTransform();
				Vector2 scale = globalTransform.Scale;
				Vector2 val2 = scale.Abs();
				CanvasItem val3 = (CanvasItem)(object)((fxParent is CanvasItem) ? fxParent : null);
				Vector2 val4;
				if (val3 == null || !FxKit.Alive((GodotObject)(object)val3))
				{
					val4 = Vector2.One;
				}
				else
				{
					globalTransform = val3.GetGlobalTransform();
					scale = globalTransform.Scale;
					val4 = scale.Abs();
				}
				Vector2 val5 = val4;
				float num = val2.X / Mathf.Max(0.01f, val5.X);
				float num2 = val2.Y / Mathf.Max(0.01f, val5.Y);
				if (num > 0.2f && num < 5f && num2 > 0.2f && num2 < 5f)
				{
					val = new Vector2(val.X * num, val.Y * num2);
				}
			}
			catch
			{
			}
			return val;
		}

		public static Vector2 BottomOf(NCreature node)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Control hitbox = node.Hitbox;
				if (FxKit.Alive((GodotObject)(object)hitbox))
				{
					return ((CanvasItem)hitbox).GetGlobalTransform() * new Vector2(hitbox.Size.X * 0.5f, hitbox.Size.Y);
				}
			}
			catch
			{
			}
			return node.GetBottomOfHitbox();
		}

		public static float FacingOf(NCreature node)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				float result;
				if (FxKit.Alive((GodotObject)(object)node))
				{
					Node2D body = node.Body;
					if (body != null && FxKit.Alive((GodotObject)(object)body) && body.Scale.X < 0f)
					{
						result = -1f;
						goto IL_0038;
					}
				}
				result = 1f;
				goto IL_0038;
				IL_0038:
				return result;
			}
			catch
			{
				return 1f;
			}
		}

		public static Vector2 PilePoint(PileType pile)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Invalid comparison between Unknown and I4
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Invalid comparison between Unknown and I4
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Invalid comparison between Unknown and I4
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = FxKit.ViewSize();
			Vector2 val2;
			if ((int)pile == 1)
			{
				val2 = new Vector2(val.X * 0.045f, val.Y * 0.9f);
			}
			else
			{
				val2 = (((int)pile != 4) ? new Vector2(val.X * 0.955f, val.Y * 0.9f) : new Vector2(val.X * 0.953f, val.Y * 0.73f));
			}
			Vector2 val3 = val2;
			try
			{
				NCombatRoom combatRoom = CombatRoom;
				NCombatUi val4 = ((combatRoom != null) ? combatRoom.Ui : null);
				if (!FxKit.Alive((GodotObject)(object)val4))
				{
					return val3;
				}
				Control val5;
				if ((int)pile == 1)
				{
					val5 = (Control)(object)val4.DrawPile;
				}
				else
				{
					val5 = (((int)pile != 4) ? ((Control)(object)val4.DiscardPile) : ((Control)(object)val4.ExhaustPile));
				}
				Control val6 = val5;
				if (!FxKit.Alive((GodotObject)(object)val6) || !((Node)val6).IsInsideTree())
				{
					return val3;
				}
				Vector2 val7 = val6.GlobalPosition + val6.Size * 0.5f;
				return (val7.X > 0f && val7.X < val.X && val7.Y > 0f && val7.Y < val.Y) ? val7 : val3;
			}
			catch
			{
				return val3;
			}
		}

		public static Vector2 EnergyPoint()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			Vector2 val = FxKit.ViewSize();
			Vector2 val2 = new Vector2(val.X * 0.106f, val.Y * 0.8f);
			try
			{
				NCombatRoom combatRoom = CombatRoom;
				NCombatUi val3 = ((combatRoom != null) ? combatRoom.Ui : null);
				if (!FxKit.Alive((GodotObject)(object)val3))
				{
					return val2;
				}
				Control energyCounterContainer = val3.EnergyCounterContainer;
				if (!FxKit.Alive((GodotObject)(object)energyCounterContainer) || !((Node)energyCounterContainer).IsInsideTree())
				{
					return val2;
				}
				Vector2 val4 = energyCounterContainer.GlobalPosition + energyCounterContainer.Size * 0.5f;
				return (val4.X > 0f && val4.X < val.X && val4.Y > 0f && val4.Y < val.Y) ? val4 : val2;
			}
			catch
			{
				return val2;
			}
		}

		public static void Shake(int strength)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				if (PlayerSettings.Enabled && PlayerSettings.ScreenShakeEnabled)
				{
					NGame instance = NGame.Instance;
					if (FxKit.Alive((GodotObject)(object)instance) && CombatRoom != null)
					{
						ShakeStrength val = (ShakeStrength)((strength >= 3) ? 4 : ((strength == 2) ? 3 : 2));
						instance.ScreenShake(val, (ShakeDuration)1, 90f);
					}
				}
			}
			catch
			{
			}
		}
	}
