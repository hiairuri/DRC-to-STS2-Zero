// namespace: SilentFX.Core
// type: CardDeco (class)
// source lines: 33121-33569
	public static class CardDeco
	{
		public sealed class Handle
		{
			public object State;

			internal Action<Handle, float> Tick;

			internal Action<Handle> Left;

			internal bool Removed;

			internal bool LeftCalled;

			internal float FadeDuration;

			internal float FadeTime;

			internal float FadeAlpha;

			public CardModel Card { get; }

			public string Key { get; }

			public Node2D Root { get; }

			public NCard Host { get; internal set; }

			public float Time { get; internal set; }

			public bool Alive
			{
				get
				{
					if (!Removed)
					{
						return FxKit.Alive((GodotObject)(object)Root);
					}
					return false;
				}
			}

			public Vector2 LastCentre
			{
				[CompilerGenerated]
				get
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					return field;
				}
				[CompilerGenerated]
				internal set
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					field = value;
				}
			}

			public Vector2 LastScale
			{
				[CompilerGenerated]
				get
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					return field;
				}
				[CompilerGenerated]
				internal set
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					field = value;
				}
			}

			public float LastRotation { get; internal set; }

			internal Handle(CardModel card, string key, Action<Handle, float> tick, Action<Handle> left)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Expected Obj, but got Unknown
				LastScale = Vector2.One;
				base..ctor();
				Card = card;
				Key = key;
				Tick = tick;
				Left = left;
				Root = new Node2D
				{
					Name = StringName.op_Implicit("SilentFX_CardDeco"),
					Visible = false
				};
			}

			public void Remove(float fade = 0.15f)
			{
				RemoveHandle(this, fade);
			}
		}

		private static readonly List<Handle> Live = new List<Handle>();

		private static bool _registered;

		internal static void Init()
		{
			if (!_registered)
			{
				Ticker.AddLate(Update);
				_registered = true;
			}
		}

		public static Handle Attach(CardModel card, string key, Action<Handle> build, Action<Handle, float> tick = null, Action<Handle> left = null)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Invalid comparison between Unknown and I4
			Handle handle = Find(card, key);
			if (handle != null)
			{
				return handle;
			}
			Init();
			Handle handle2 = null;
			try
			{
				handle2 = new Handle(card, key, tick, left);
				Live.Add(handle2);
				if (card != null)
				{
					CardPile pile = card.Pile;
					if ((int)((pile != null) ? new PileType?(pile.Type) : ((PileType?)null)).GetValueOrDefault() == 2)
					{
						Follow(handle2);
					}
				}
				build?.Invoke(handle2);
				return handle2;
			}
			catch (Exception ex)
			{
				DevLog.Warn("CardDeco.Attach: " + ex);
				handle2?.Remove(0f);
				return handle2;
			}
		}

		public static Handle Find(CardModel card, string key)
		{
			foreach (Handle item in Live)
			{
				if (item.Alive && item.Card == card && string.Equals(item.Key, key, StringComparison.Ordinal))
				{
					return item;
				}
			}
			return null;
		}

		public static void Remove(CardModel card, string key, float fade = 0.15f)
		{
			Find(card, key)?.Remove(fade);
		}

		public static void RemoveAll(string key, float fade = 0.15f)
		{
			Handle[] array = Live.ToArray();
			foreach (Handle handle in array)
			{
				if (string.Equals(handle.Key, key, StringComparison.Ordinal))
				{
					handle.Remove(fade);
				}
			}
		}

		public static void Reset()
		{
			Handle[] array = Live.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				Free(array[i]);
			}
			Live.Clear();
		}

		internal static void DetachHost(NCard host)
		{
			Handle[] array = Live.ToArray();
			foreach (Handle handle in array)
			{
				try
				{
					if (handle.Host == host || (FxKit.Alive((GodotObject)(object)handle.Root) && (object)((Node)handle.Root).GetParent() == host))
					{
						Detach(handle, hide: true);
					}
				}
				catch (Exception ex)
				{
					DevLog.Warn("CardDeco.DetachHost: " + ex);
					Free(handle);
				}
			}
		}

		private static void Follow(Handle handle)
		{
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			NCard val = HandCards.NodeOf(handle.Card);
			if (!FxKit.Alive((GodotObject)(object)val) || !((Node)val).IsInsideTree() || val.Model != handle.Card)
			{
				val = null;
			}
			if (handle.Host != null && (!FxKit.Alive((GodotObject)(object)handle.Host) || !((Node)handle.Host).IsInsideTree() || handle.Host.Model != handle.Card || handle.Host != val))
			{
				Detach(handle, hide: true);
			}
			if (val == null)
			{
				handle.Host = null;
				((CanvasItem)handle.Root).Visible = false;
				return;
			}
			if ((object)((Node)handle.Root).GetParent() != val)
			{
				Node parent = ((Node)handle.Root).GetParent();
				if (FxKit.Alive((GodotObject)(object)parent))
				{
					parent.RemoveChild((Node)(object)handle.Root);
				}
				((Node)val).AddChild((Node)(object)handle.Root, false, (InternalMode)0);
				handle.Root.Rotation = 0f;
			}
			handle.Host = val;
			handle.Root.Position = Vector2.Zero;
			handle.Root.Scale = (FxKit.Alive((GodotObject)(object)val.Body) ? val.Body.Scale : Vector2.One);
			((CanvasItem)handle.Root).Visible = true;
			CardStandIn.Pose(val, out var centre, out var rotation, out var scale);
			handle.LastCentre = centre;
			handle.LastRotation = rotation;
			handle.LastScale = scale;
		}

		private static bool Detach(Handle handle, bool hide)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			handle.Host = null;
			if (!FxKit.Alive((GodotObject)(object)handle.Root))
			{
				return false;
			}
			Node2D root = handle.Root;
			Node parent = ((Node)root).GetParent();
			Transform2D val = (Transform2D)((FxKit.Alive((GodotObject)(object)parent) && ((Node)root).IsInsideTree()) ? ((CanvasItem)root).GetGlobalTransform() : new Transform2D(handle.LastRotation, handle.LastScale, 0f, handle.LastCentre));
			if (FxKit.Alive((GodotObject)(object)parent))
			{
				parent.RemoveChild((Node)(object)root);
			}
			Node uiLayer = GameRefs.UiLayer;
			if (FxKit.Alive((GodotObject)(object)uiLayer) && uiLayer.IsInsideTree())
			{
				uiLayer.AddChild((Node)(object)root, false, (InternalMode)0);
				root.GlobalTransform = val;
			}
			else
			{
				root.Transform = val;
			}
			((CanvasItem)root).Visible = !hide && ((Node)root).IsInsideTree();
			return ((Node)root).IsInsideTree();
		}

		private static void RemoveHandle(Handle handle, float fade)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			if (handle == null || handle.Removed)
			{
				return;
			}
			handle.Removed = true;
			try
			{
				if (!FxKit.Alive((GodotObject)(object)handle.Root) || !((CanvasItem)handle.Root).Visible || !((Node)handle.Root).IsInsideTree() || !(fade > 0f) || !float.IsFinite(fade) || !Detach(handle, hide: false))
				{
					Free(handle);
					return;
				}
				handle.FadeDuration = fade;
				handle.FadeAlpha = ((CanvasItem)handle.Root).Modulate.A;
				handle.Tick = null;
				handle.Left = null;
			}
			catch (Exception ex)
			{
				DevLog.Warn("CardDeco.Remove: " + ex);
				Free(handle);
			}
		}

		private static void Free(Handle handle)
		{
			if (handle == null)
			{
				return;
			}
			handle.Removed = true;
			handle.Host = null;
			handle.Tick = null;
			handle.Left = null;
			Live.Remove(handle);
			try
			{
				if (GodotObject.IsInstanceValid((GodotObject)(object)handle.Root))
				{
					Node parent = ((Node)handle.Root).GetParent();
					if (GodotObject.IsInstanceValid((GodotObject)(object)parent))
					{
						parent.RemoveChild((Node)(object)handle.Root);
					}
					if (!((GodotObject)handle.Root).IsQueuedForDeletion())
					{
						((Node)handle.Root).QueueFree();
					}
				}
			}
			catch (Exception ex)
			{
				DevLog.Warn("CardDeco.Free: " + ex);
			}
		}

		private static void Update(double realDt)
		{
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Invalid comparison between Unknown and I4
			Handle[] array = null;
			try
			{
				int count = Live.Count;
				if (count == 0)
				{
					return;
				}
				array = ArrayPool<Handle>.Shared.Rent(count);
				Live.CopyTo(array);
				float num = (float)(realDt * (double)Mathf.Clamp((float)Engine.TimeScale, 0f, 4f));
				for (int i = 0; i < count; i++)
				{
					Handle handle = array[i];
					if (!Live.Contains(handle))
					{
						continue;
					}
					try
					{
						if (!FxKit.Alive((GodotObject)(object)handle.Root))
						{
							Free(handle);
							continue;
						}
						if (handle.Removed)
						{
							handle.FadeTime += num;
							float num2 = Mathf.Clamp(handle.FadeTime / handle.FadeDuration, 0f, 1f);
							Color modulate = ((CanvasItem)handle.Root).Modulate;
							modulate.A = handle.FadeAlpha * (1f - num2);
							((CanvasItem)handle.Root).Modulate = modulate;
							if (num2 >= 1f)
							{
								Free(handle);
							}
							continue;
						}
						handle.Time += num;
						if (handle.Host != null && (!FxKit.Alive((GodotObject)(object)handle.Host) || handle.Host.Model != handle.Card))
						{
							Detach(handle, hide: true);
						}
						CardModel card = handle.Card;
						if (card == null)
						{
							goto IL_0180;
						}
						CardPile pile = card.Pile;
						if ((int)((pile != null) ? new PileType?(pile.Type) : ((PileType?)null)).GetValueOrDefault() != 2)
						{
							goto IL_0180;
						}
						Follow(handle);
						handle.Tick?.Invoke(handle, num);
						goto end_IL_0064;
						IL_0180:
						if (!handle.LeftCalled)
						{
							handle.LeftCalled = true;
							try
							{
								handle.Left?.Invoke(handle);
							}
							catch (Exception ex)
							{
								DevLog.Warn("CardDeco (left): " + ex);
							}
						}
						handle.Remove();
						end_IL_0064:;
					}
					catch (Exception ex2)
					{
						DevLog.Warn("CardDeco (callback or frame): " + ex2);
						handle.Remove(0f);
					}
				}
			}
			catch (Exception ex3)
			{
				DevLog.Error("CardDeco.Update: " + ex3);
				Reset();
			}
			finally
			{
				if (array != null)
				{
					ArrayPool<Handle>.Shared.Return(array, clearArray: true);
				}
			}
		}
	}
