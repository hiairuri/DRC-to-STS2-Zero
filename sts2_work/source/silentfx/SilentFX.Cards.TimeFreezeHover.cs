// namespace: SilentFX.Cards
// type: TimeFreezeHover (class)
// source lines: 115452-115723
	internal static class TimeFreezeHover
	{
		private sealed class Peek
		{
			internal NHealthBar Bar;

			internal NCreature Node;

			internal Creature Creature;

			internal Label Hp;

			internal Label Block;

			internal Control BlockBox;

			internal bool Showing;

			internal string FrozenHp;

			internal string FrozenBlock;

			internal bool FrozenBlockVisible;

			private int _lastHp;

			private int _lastMaxHp;

			private int _lastBlock;

			private string _liveHp;

			private string _liveBlock;

			internal bool Valid
			{
				get
				{
					if (FxKit.Alive((GodotObject)(object)Bar) && ((Node)Bar).IsInsideTree() && FxKit.Alive((GodotObject)(object)Node) && ((Node)Node).IsInsideTree() && Node.Entity == Creature && FxKit.Alive((GodotObject)(object)Hp) && FxKit.Alive((GodotObject)(object)Block))
					{
						return FxKit.Alive((GodotObject)(object)BlockBox);
					}
					return false;
				}
			}

			internal void Update()
			{
				if (!ContainsMouse(Node.Hitbox) && !ContainsMouse((Control)(object)Bar))
				{
					Restore();
					return;
				}
				if (!Showing)
				{
					FrozenHp = Hp.Text;
					FrozenBlock = Block.Text;
					FrozenBlockVisible = ((CanvasItem)BlockBox).Visible;
					Showing = true;
				}
				int currentHp = Creature.CurrentHp;
				int maxHp = Creature.MaxHp;
				int block = Creature.Block;
				if (_liveHp == null || currentHp != _lastHp || maxHp != _lastMaxHp)
				{
					_lastHp = currentHp;
					_lastMaxHp = maxHp;
					_liveHp = $"{currentHp}/{maxHp}";
				}
				if (_liveBlock == null || block != _lastBlock)
				{
					_lastBlock = block;
					_liveBlock = block.ToString(CultureInfo.InvariantCulture);
				}
				if (((CanvasItem)Hp).Visible)
				{
					Write(Hp, _liveHp);
				}
				Write(Block, _liveBlock);
				bool flag = FrozenBlockVisible || block > 0;
				if (((CanvasItem)BlockBox).Visible != flag)
				{
					((CanvasItem)BlockBox).Visible = flag;
				}
			}

			internal void Restore()
			{
				if (Showing)
				{
					Showing = false;
					if (FxKit.Alive((GodotObject)(object)Hp))
					{
						Write(Hp, FrozenHp);
					}
					if (FxKit.Alive((GodotObject)(object)Block))
					{
						Write(Block, FrozenBlock);
					}
					if (FxKit.Alive((GodotObject)(object)BlockBox))
					{
						((CanvasItem)BlockBox).Visible = FrozenBlockVisible;
					}
				}
			}
		}

		private const BindingFlags Fields = BindingFlags.Instance | BindingFlags.NonPublic;

		private static readonly FieldInfo HpLabel = typeof(NHealthBar).GetField("_hpLabel", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly FieldInfo BlockLabel = typeof(NHealthBar).GetField("_blockLabel", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly FieldInfo BlockContainer = typeof(NHealthBar).GetField("_blockContainer", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly List<Peek> Entries = new List<Peek>();

		private static bool _warned;

		internal static void Track(NHealthBar bar)
		{
			if (!TimeFreeze.Active || !PlayerSettings.Enabled || !FxKit.Alive((GodotObject)(object)bar))
			{
				return;
			}
			try
			{
				for (int num = Entries.Count - 1; num >= 0; num--)
				{
					if (Entries[num].Bar == bar)
					{
						if (Entries[num].Valid)
						{
							return;
						}
						Entries[num].Restore();
						Entries.RemoveAt(num);
					}
				}
				if (HpLabel == null || BlockLabel == null || BlockContainer == null)
				{
					Warn("native health-bar labels are unavailable; keeping the normal frozen display");
					return;
				}
				NCreature val = null;
				Node parent = ((Node)bar).GetParent();
				while (FxKit.Alive((GodotObject)(object)parent))
				{
					NCreature val2 = (NCreature)(object)((parent is NCreature) ? parent : null);
					if (val2 != null)
					{
						val = val2;
						break;
					}
					parent = parent.GetParent();
				}
				if (FxKit.Alive((GodotObject)(object)val) && val.Entity != null)
				{
					Peek peek = new Peek
					{
						Bar = bar,
						Node = val,
						Creature = val.Entity
					};
					object? value = HpLabel.GetValue(bar);
					peek.Hp = (Label)((value is Label) ? value : null);
					object? value2 = BlockLabel.GetValue(bar);
					peek.Block = (Label)((value2 is Label) ? value2 : null);
					object? value3 = BlockContainer.GetValue(bar);
					peek.BlockBox = (Control)((value3 is Control) ? value3 : null);
					Peek peek2 = peek;
					if (peek2.Valid)
					{
						Entries.Add(peek2);
					}
				}
			}
			catch (Exception ex)
			{
				Warn(ex.Message);
			}
		}

		internal static void Update()
		{
			if (!TimeFreeze.Active || !PlayerSettings.Enabled)
			{
				Clear();
				return;
			}
			for (int num = Entries.Count - 1; num >= 0; num--)
			{
				Peek peek = Entries[num];
				try
				{
					if (peek.Valid)
					{
						peek.Update();
						continue;
					}
					peek.Restore();
				}
				catch (Exception ex)
				{
					try
					{
						peek.Restore();
					}
					catch
					{
					}
					Warn(ex.Message);
				}
				Entries.RemoveAt(num);
			}
		}

		internal static void Clear()
		{
			foreach (Peek entry in Entries)
			{
				try
				{
					entry.Restore();
				}
				catch
				{
				}
			}
			Entries.Clear();
		}

		private static bool ContainsMouse(Control control)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (FxKit.Alive((GodotObject)(object)control) && ((Node)control).IsInsideTree() && ((CanvasItem)control).IsVisibleInTree())
			{
				Rect2 val = new Rect2(Vector2.Zero, control.Size);
				return val.HasPoint(((CanvasItem)control).GetLocalMousePosition());
			}
			return false;
		}

		private static void Write(Label label, string text)
		{
			if (!(label.Text == text))
			{
				MegaLabel val = (MegaLabel)(object)((label is MegaLabel) ? label : null);
				if (val != null)
				{
					val.SetTextAutoSize(text);
				}
				else
				{
					label.Text = text;
				}
			}
		}

		private static void Warn(string message)
		{
			if (!_warned)
			{
				_warned = true;
				DevLog.Warn("Bullet Time hover: " + message);
			}
		}
	}
