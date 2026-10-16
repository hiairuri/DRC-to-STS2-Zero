// namespace: SilentFX.Core
// type: RemoteFxFade (class)
// source lines: 41374-41700
	internal static class RemoteFxFade
	{
		private readonly record struct Write(CanvasItem Item, Color Original, Color Applied);

		private readonly record struct Branch(Node Node, bool Inherits);

		private sealed class EmptyScope : IDisposable
		{
			public void Dispose()
			{
			}
		}

		private sealed class Scope : IDisposable
		{
			internal readonly bool Remote;

			private readonly Scope _previous;

			private readonly List<Node> _parents = new List<Node>();

			private bool _disposed;

			internal Scope(bool remote, Node[] parents)
			{
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Expected Obj, but got Unknown
				Remote = remote;
				_previous = _scope;
				_scope = this;
				if (!remote || parents == null)
				{
					return;
				}
				try
				{
					SyncRoom();
					foreach (Node val in parents)
					{
						if (FxKit.Alive((GodotObject)(object)val) && val.IsInsideTree() && !_parents.Contains(val))
						{
							val.ChildEnteredTree += Entered;
							_parents.Add(val);
						}
					}
				}
				catch
				{
					Dispose();
					throw;
				}
			}

			private void Entered(Node node)
			{
				Scope scope = _scope;
				if (scope != null && scope.Remote && MultiplayerFx.Active && PlayerSettings.Enabled)
				{
					Register(node);
				}
			}

			public void Dispose()
			{
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Expected Obj, but got Unknown
				if (_disposed)
				{
					return;
				}
				_disposed = true;
				foreach (Node parent in _parents)
				{
					if (FxKit.Alive((GodotObject)(object)parent))
					{
						parent.ChildEnteredTree -= Entered;
					}
				}
				_parents.Clear();
				if (_scope == this)
				{
					_scope = _previous;
				}
			}
		}

		private const int MaxRoots = 2048;

		private const int MaxWalk = 16384;

		private static readonly List<Node> Roots = new List<Node>();

		private static readonly List<Write> Writes = new List<Write>();

		private static readonly List<Branch> Walk = new List<Branch>();

		private static readonly HashSet<CanvasItem> Seen = new HashSet<CanvasItem>();

		private static readonly HashSet<Node> RootSet = new HashSet<Node>();

		private static readonly IDisposable Empty = new EmptyScope();

		private static Scope _scope;

		private static Node _room;

		private static bool _hooked;

		internal static IDisposable Capture(bool remote, params Node[] parents)
		{
			bool flag = MultiplayerFx.Active && PlayerSettings.Enabled;
			if (_scope == null && (!remote || !flag))
			{
				return Empty;
			}
			return new Scope(remote & flag, parents);
		}

		private static void Register(Node node)
		{
			if (!FxKit.Alive((GodotObject)(object)node) || !node.IsInsideTree())
			{
				return;
			}
			for (int num = Roots.Count - 1; num >= 0; num--)
			{
				Node val = Roots[num];
				if (!FxKit.Alive((GodotObject)(object)val) || !val.IsInsideTree())
				{
					Roots.RemoveAt(num);
				}
				else
				{
					if (val == node || Ancestor(val, node))
					{
						return;
					}
					if (Ancestor(node, val))
					{
						Roots.RemoveAt(num);
					}
				}
			}
			if (Roots.Count < 2048)
			{
				Roots.Add(node);
				if (!_hooked)
				{
					RenderingServer.FramePostDraw += Restore;
					_hooked = true;
				}
			}
		}

		private static bool Ancestor(Node ancestor, Node node)
		{
			for (int i = 0; i < 1024; i++)
			{
				if (!FxKit.Alive((GodotObject)(object)node))
				{
					break;
				}
				node = node.GetParent();
				if (node == ancestor)
				{
					return true;
				}
			}
			return false;
		}

		private static void SyncRoom()
		{
			NCombatRoom combatRoom = GameRefs.CombatRoom;
			if ((object)_room != combatRoom)
			{
				Reset();
				_room = (Node)(object)combatRoom;
			}
		}

		internal static void BeforeDraw()
		{
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			Restore();
			SyncRoom();
			if (!MultiplayerFx.Active || !PlayerSettings.Enabled)
			{
				Reset();
				return;
			}
			for (int num = Roots.Count - 1; num >= 0; num--)
			{
				if (!FxKit.Alive((GodotObject)(object)Roots[num]) || !Roots[num].IsInsideTree())
				{
					Roots.RemoveAt(num);
				}
			}
			float factor = RemoteFxFocus.Factor;
			if (factor == 1f || Roots.Count == 0)
			{
				return;
			}
			Seen.Clear();
			RootSet.Clear();
			Walk.Clear();
			foreach (Node root in Roots)
			{
				RootSet.Add(root);
			}
			foreach (Node root2 in Roots)
			{
				if (!HasRegisteredAncestor(root2))
				{
					Walk.Add(new Branch(root2, Inherits: false));
				}
			}
			int num2 = 0;
			try
			{
				while (Walk.Count > 0 && num2++ < 16384)
				{
					int index = Walk.Count - 1;
					Branch branch = Walk[index];
					Walk.RemoveAt(index);
					Node node = branch.Node;
					if (!FxKit.Alive((GodotObject)(object)node) || !node.IsInsideTree())
					{
						continue;
					}
					bool inherits = false;
					CanvasItem val = (CanvasItem)(object)((node is CanvasItem) ? node : null);
					if (val != null)
					{
						inherits = true;
						if ((!branch.Inherits || val.TopLevel) && Seen.Add(val))
						{
							Color modulate = val.Modulate;
							Color val2 = modulate;
							val2.A *= factor;
							Writes.Add(new Write(val, modulate, val2));
							val.Modulate = val2;
						}
					}
					int childCount = node.GetChildCount(false);
					for (int i = 0; i < childCount; i++)
					{
						if (Walk.Count >= 16384)
						{
							break;
						}
						Walk.Add(new Branch(node.GetChild(i, false), inherits));
					}
				}
			}
			catch (Exception ex)
			{
				Restore();
				DevLog.Warn("RemoteFxFade.BeforeDraw: " + ex.Message);
			}
			finally
			{
				Walk.Clear();
				Seen.Clear();
				RootSet.Clear();
			}
		}

		private static bool HasRegisteredAncestor(Node node)
		{
			for (int i = 0; i < 16384; i++)
			{
				if (!FxKit.Alive((GodotObject)(object)node))
				{
					break;
				}
				node = node.GetParent();
				if (node != null && RootSet.Contains(node))
				{
					return true;
				}
			}
			return false;
		}

		internal static void Restore()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			foreach (Write write in Writes)
			{
				try
				{
					if (FxKit.Alive((GodotObject)(object)write.Item) && write.Item.Modulate == write.Applied)
					{
						write.Item.Modulate = write.Original;
					}
				}
				catch
				{
				}
			}
			Writes.Clear();
		}

		internal static void Reset()
		{
			Restore();
			Roots.Clear();
			Walk.Clear();
			Seen.Clear();
			RootSet.Clear();
			_room = null;
			if (_hooked)
			{
				RenderingServer.FramePostDraw -= Restore;
				_hooked = false;
			}
		}
	}
