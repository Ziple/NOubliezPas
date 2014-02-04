using System;
using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;

namespace kT.GUI
{
	// ajouter les class name et id.
	// position et size à faire.
	/// <summary>
	/// Base class for all other user interface objects.
	/// </summary>
	public class Widget
	{
		#region Members
		UIManager myManager;
		Widget myParent;

		/// <summary>
		/// List of the widgets that this one must update.
		/// </summary>
		List<Widget> myChildrenWidgets = new List<Widget>();

		/// <summary>
		/// Bounding rectangle of the widget (updated when size/position changed).
		/// </summary>
		FloatRect myBoundingRectangle = new FloatRect();

		/// <summary>
		/// MinSize of the widget.
		/// </summary>
		Vector2f myMinSize = new Vector2f();
		/// <summary>
		/// Max size of the widget.
		/// </summary>
		Vector2f myMaxSize = new Vector2f( float.MaxValue, float.MaxValue );
		/// <summary>
		/// Visibility of the widget.
		/// </summary>
		bool myIsVisible = false;
		#endregion

		#region Constructors.

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="manager_">Gui manager to associate the widget with.</param>
		/// <param name="parent_">widget's parent</param>
		public Widget(UIManager manager_, Widget parent_)
		{
            if (manager_ == null)
                throw new Exception("The manager is null");

			myManager = manager_;
			Parent = parent_;

			// notify the wm that this widget type exists.
			Manager.RegisterWidgetType("Widget", null);
		}
		#endregion

		#region Parent/Children management

		/// <summary>
		/// Returns the reference to the widget manager.
		/// </summary>
		public UIManager Manager { get { return myManager; } }

		/// <summary>
		/// Add a widget to the children list.
		/// </summary>
		/// <param name="widget">child to add</param>
		protected void AddChild(Widget widget) { myChildrenWidgets.Add(widget); }

		/// <summary>
		/// Remove a widget from the children list.
		/// </summary>
		/// <param name="widget">child to remove.</param>
		protected void RemoveChild(Widget widget) { myChildrenWidgets.Remove(widget); }

		/// <summary>
		/// Get/set the parent.
		/// </summary>
		public Widget Parent
		{
			get { return myParent; }
			set
			{
				if (myParent != null)
					myParent.RemoveChild(this);
				else
					myManager.UnmanageWidget(this);

				myParent = value;

				if (myParent != null)
					myParent.AddChild(this);
				else
					myManager.ManageWidget(this);
			}
		}

		/// <summary>
		/// Return the managed widget at the given position.
		/// The position is given in the widget basis.
		/// That's, if the point lies at the top left corner of the widget, then,
		/// the point must be (0,0) and not in Parent coordinates or Screen coordinates.
		/// </summary>
		/// <param name="pos">Position of the widget.</param>
		public virtual Widget PickWidget(Vector2f pos) { return null; }
		#endregion

		#region Position/Size

        public static bool Contains(FloatRect rect, Vector2f point)
        {
            return (rect.Left <= point.X) && (rect.Left + rect.Width >= point.X)
                   && (rect.Top <= point.Y) && (rect.Top + rect.Height >= point.Y);
        }

        public static bool Contains(FloatRect rect1, FloatRect rect2)
        {
            return Contains(rect1, new Vector2f(rect2.Left, rect2.Top))
                   && Contains(rect1, new Vector2f(rect2.Left + rect2.Width, rect2.Top + rect2.Height));
        }

        public bool Contains(Vector2f point)
        {
            return Contains( BoundingRectangle, point );
        }

		/// <summary>
		/// Get the bounding rectangle of the widget (region occupied by it).
		/// </summary>
		public FloatRect BoundingRectangle { get { return myBoundingRectangle; } }

		/// <summary>
		/// Get the Bounding rectangle in local space.
		/// </summary>
		public FloatRect LocalSpaceBoundingRectangle { get { FloatRect ls = BoundingRectangle; ls.Left = ls.Top = 0; return ls; } }

		/// <summary>
		/// Get/set the position of the widget (relative to the parent).
        /// This is the same as TopLeftPosition.
		/// If the new position can be given without a resizement, the widget is resized then.
        /// <remarks>Would better check if the bouding rectangle is inside the parent.</remarks>
		/// </summary>
		public virtual Vector2f Position
		{
			get { return new Vector2f( myBoundingRectangle.Left, myBoundingRectangle.Top ); }
            set { myBoundingRectangle.Left = value.X; myBoundingRectangle.Top = value.Y; Resize(Size); }
		}

        /// <summary>
        /// Get or set the position of the top left corner relative to parent.
        /// </summary>
        public Vector2f TopLeftPosition
        {
            get { return Position; }
            set { Position = value; }
        }

        /// <summary>
        /// Get or set the position of the top right corner relative to parent.
        /// </summary>
        public Vector2f TopRightPosition
        {
            get { return new Vector2f(Position.X + Size.X, Position.Y); }
            set { Position = new Vector2f(value.X - Size.X, value.Y); }
        }

        /// <summary>
        /// Get or set the position of the bottom left corner relative to parent.
        /// </summary>
        public Vector2f BottomLeftPosition
        {
            get { return new Vector2f(Position.X, Position.Y + Size.Y); }
            set { Position = new Vector2f(value.X, value.Y - Size.Y); }
        }

        /// <summary>
        /// Get or set the position of the bottom right corner relative to parent.
        /// </summary>
        public Vector2f BottomRightPosition
        {
            get { return new Vector2f(Position.X + Size.X, Position.Y + Size.Y); }
            set { Position = new Vector2f(value.X - Size.X, value.Y - Size.Y); }
        }

        /// <summary>
        /// Get or set the position of center of the widget relative to parent.
        /// </summary>
        public Vector2f CenterPosition
        {
            get { return new Vector2f(Position.X + Size.X / 2, Position.Y + Size.Y / 2); }
            set { Position = new Vector2f(value.X - Size.X / 2, value.Y - Size.Y / 2); }
        }

		/// <summary>
		/// Get/set the minimal size of the widget.
		/// </summary>
        public Vector2f MinSize
		{
			get { return myMinSize; }
			set
			{
				myMinSize = value;
                Vector2f newSize = Size;
				if (myMinSize.X > newSize.X)
					newSize.X = myMinSize.X;
				if (myMinSize.Y > newSize.Y)
					newSize.Y = myMinSize.Y;

				if (newSize.X > Size.X || newSize.Y > Size.Y)
					Resize(myMinSize);
			}
		}

		/// <summary>
		/// Get/set the maximal size of the widget.
		/// </summary>
		public Vector2f MaxSize
		{
			get { return myMaxSize; }
			set
			{
				myMaxSize = value;
                Vector2f newSize = Size;
				if (myMaxSize.X < newSize.X)
					newSize.X = myMaxSize.X;
				if (myMaxSize.Y < newSize.Y)
					newSize.Y = myMaxSize.Y;

				if (newSize.X < Size.X || newSize.Y < Size.Y)
					Resize(myMaxSize);
			}
		}

		/// <summary>
		/// Get the ideal size of the widget.
		/// </summary>
        public virtual Vector2f SizeHint
		{
			get { return new Vector2f(0f,0f); }
		}

		/// <summary>
		/// Get/set the size of the widget.
		/// </summary>
        public virtual Vector2f Size
		{
            get { return new Vector2f(myBoundingRectangle.Width, myBoundingRectangle.Height); }
			set { Resize(value); }
		}

		/// <summary>
		/// Method that must be implemented if you have a widget that contains widgets.
		/// This method compute the new widget size so that the child widget can be
		/// resized.
		/// </summary>
		/// <param name="child">Child widget requesting the resizement.</param>
		/// <param name="requestedSize">Size requested by the widget.</param>
		/// <returns></returns>
        protected virtual Vector2f _computeNewSizeForChild(Widget child, Vector2f requestedSize)
		{
			return new Vector2f(0f,0f);
		}

		/// <summary>
		/// Called by the child to notice that it wants
		/// to change its size or Visibility.
		/// </summary>
		/// <param name="child">Child willing to resize.</param>
		/// <param name="requestedSize">Size the child wants in the widget.</param>
		/// <returns>True if resizement granted, false otherwise.</returns>
        public bool ResizeForChild(Widget child, Vector2f requestedSize)
		{
			// May be position of the child widget has changed
			// and we just want to update the parent accordingly
			// so we musn't check if requestedSize is the same as before
			// in the view to NOT froward properly the event.
			ChildResizeEvent ev = new ChildResizeEvent(child, requestedSize);
			OnEvent(ev);
			if (ev.Accepted)
			{
				OnChildResizeEvent(ev);
				if (ev.Accepted)
				{
					// We actually need to resize.
					if (ev.NecessaryParentSize.X > Size.X || ev.NecessaryParentSize.Y > Size.Y)
					{
						// Tries to resize to the given size.
                        Vector2f newSize = Resize(ev.NecessaryParentSize);
						// Check if we resized enough to place the widget in...
						if (newSize.X < ev.NecessaryParentSize.X || newSize.Y < ev.NecessaryParentSize.Y)
							return false;
					}
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Tries to resize the widget to the new size.
		/// </summary>
		/// <param name="newSize">Size to give to the widget.</param>
		/// <returns>Size of the widget after the resizement.</returns>
        public Vector2f Resize(Vector2f newSize)
		{
			// May be position of the child widget has changed
			// and we just want to update the parent accordingly
			// so we musn't check if requestedSize is the same as before
			// in the view to NOT froward properly the event.
			ResizeEvent ev = new ResizeEvent(newSize);
			OnEvent(ev);
			if (ev.Accepted)
			{
				OnResizeEvent(ev);
				if (ev.Accepted)
				{
					if ((Parent != null && Parent.ResizeForChild(this, newSize)) || Parent == null)
						DoResize(newSize);
				}
			}
			return Size;
		}

		/// <summary>
		/// Function that must be implemented to carry out the resize operation.
		/// </summary>
		/// <param name="newSize">Size that is given to the widget.</param>
        protected virtual void DoResize(Vector2f newSize)
		{
			myBoundingRectangle.Width = newSize.X;
			myBoundingRectangle.Height = newSize.Y;
		}

		#endregion

		#region Rendering hints
		/// <summary>
		/// Get/set visible property of the widget.
		/// A Set operation can be refused due to Parent wish.
		/// So if you change visibility, double check the changes...
		/// </summary>
		public bool Visible
		{
			get { return myIsVisible; }
			set
			{
				if (myIsVisible == false
					&& value == true )
				{
					if (Parent != null)
					{
						if (Parent.ResizeForChild(this, Size))
							myIsVisible = true;
					}
					else
						myIsVisible = true;
				}
				else if (myIsVisible == true
					&& value == false)
				{
					if (Parent != null)
					{
						if (Parent.ResizeForChild(this, new Vector2f(0f,0f)))
							myIsVisible = false;
					}
					else
						myIsVisible = false;


				}
			}
		}
		#endregion

		#region Color and tinting things.
        Color oldPainterTint = Color.White;

		Color myTintColor = Color.White;

		/// <summary>
		/// Get/Set the tint color of the widget.
		/// This one is propagated to all the children.
		/// </summary>
		public Color Tint
		{
			get { return myTintColor; }
			set { myTintColor = value; }
		}
		#endregion

        #region Event management

        /// <summary>
		/// Dispatch the event to the specialised method.
		/// </summary>
		/// <param name="raisedEvent">Event raised.</param>
		public void DispatchEvent(Event raisedEvent)
		{
            if (raisedEvent.Type == Event.EventType.UpdateEvent)
                OnUpdate((UpdateEvent)raisedEvent);
            else if (raisedEvent.Type == Event.EventType.DrawEvent)
                OnDraw((DrawEvent)raisedEvent);
            else if (raisedEvent.Type == Event.EventType.ChildResizeEvent)
                OnChildResizeEvent((ChildResizeEvent)raisedEvent);
            else if (raisedEvent.Type == Event.EventType.ResizeEvent)
                OnResizeEvent((ResizeEvent)raisedEvent);
            else if (raisedEvent.Type == Event.EventType.TextEnteredEvent)
                OnTextEnteredEvent((TextEnteredEvent)raisedEvent);
            else if (raisedEvent.Type == Event.EventType.KeyEvent)
                OnKeyEvent((KeyEvent)raisedEvent);
            else if (raisedEvent.Type == Event.EventType.MouseEvent)
                OnMouseEvent((MouseEvent)raisedEvent);
            else if (raisedEvent.Type == Event.EventType.RenderingOrderChangedEvent)
                OnRenderingOrderChangedEvent((RenderingOrderChangedEvent)raisedEvent);
            else if (raisedEvent.Type == Event.EventType.FocusChangedEvent)
                OnFocusChangedEvent((FocusChangedEvent)raisedEvent);
            else if (raisedEvent.Type == Event.EventType.ClickEvent)
                OnClickEvent((ClickEvent)raisedEvent);
            else if (raisedEvent.Type == Event.EventType.HoverEvent)
                OnHoverEvent((HoverEvent)raisedEvent);
            else if (raisedEvent.Type == Event.EventType.HoverEndEvent)
                OnHoverEndEvent((HoverEndEvent)raisedEvent);
		}

		/// <summary>
		/// Before the specialsed OnXX method is called this function is called.
		/// If you don't want the event to be dispatched then, don't accept the event.
		/// </summary>
		/// <param name="raisedEvent"></param>
		public virtual void OnEvent(Event raisedEvent) { }

		/// <summary>
		/// Raised when the widgets update is beginning.
		/// </summary>
		/// <param name="beginUpdateEvent">raised event</param>
		public virtual void OnUpdate(UpdateEvent beginUpdateEvent)
		{
			foreach (Widget child in myChildrenWidgets)
				child.OnUpdate(beginUpdateEvent);
		}

		/// <summary>
		/// Raised when drawing the widgets.
		/// Each widget must draw itself and know
		/// that a new basis is build for it: it starts at the upper left corner of the widget.
		/// DON'T consider widget's position while drawing. It's done by it's parent/manager.
		/// </summary>
		/// <param name="drawEvent">raised event</param>
		public virtual void OnDraw(DrawEvent drawEvent)
		{
            oldPainterTint = drawEvent.Painter.Tint;

			Color interpolatedTint = Tint;
			if (Parent != null)
			{
				interpolatedTint.R = (byte)((interpolatedTint.R * drawEvent.Painter.Tint.R) / 255);
				interpolatedTint.G = (byte)((interpolatedTint.G * drawEvent.Painter.Tint.G) / 255);
				interpolatedTint.B = (byte)((interpolatedTint.B * drawEvent.Painter.Tint.B) / 255);
				interpolatedTint.A = (byte)((interpolatedTint.A * drawEvent.Painter.Tint.A) / 255);
			}

			drawEvent.Painter.Tint = interpolatedTint;
		}

        /// <summary>
        /// Restore the old painter tint
        /// </summary>
        public void EndDraw(DrawEvent drawEvent)
        {
            if( drawEvent.DebugDraw )
                drawEvent.Painter.DebugDrawRectangle(LocalSpaceBoundingRectangle, Color.Red);

            drawEvent.Painter.Tint = oldPainterTint;
        }



		/// <summary>
		/// Raised when a child widget wants to change its size.
		/// </summary>
		/// <param name="childResizeEvent"></param>
		public virtual void OnChildResizeEvent(ChildResizeEvent childResizeEvent)
		{
			childResizeEvent.NecessaryParentSize = _computeNewSizeForChild(childResizeEvent.Child, childResizeEvent.RequestedSize);

			if (childResizeEvent.NecessaryParentSize.X > MaxSize.X || childResizeEvent.NecessaryParentSize.Y > MaxSize.Y)
				childResizeEvent.Accepted = false;
		}

		/// <summary>
		/// Raised when the size of the widget is about to change.
		/// </summary>
		/// <param name="resizeEvent"></param>
		public virtual void OnResizeEvent(ResizeEvent resizeEvent)
		{
			if ((resizeEvent.RequestedSize.X > MaxSize.X || resizeEvent.RequestedSize.Y > MaxSize.Y)
				|| (resizeEvent.RequestedSize.X < MinSize.X || resizeEvent.RequestedSize.Y < MinSize.Y))
				resizeEvent.Accepted = false;
		}

		/// <summary>
		/// Raised when a text is entered.
		/// </summary>
		/// <param name="textEvent">raised event.</param>
		public virtual void OnTextEnteredEvent(TextEnteredEvent textEvent) { }

		/// <summary>
		/// Raised when a key is pressed/released.
		/// </summary>
		/// <param name="keyEvent">raised event.</param>
		public virtual void OnKeyEvent(KeyEvent keyEvent) { }

		/// <summary>
		/// Raised when the mouse state change.
		/// </summary>
		/// <param name="mouseEvent">raised event.</param>
		public virtual void OnMouseEvent(MouseEvent mouseEvent) { }

		/// <summary>
		/// Raised when the widget can gain/lose the focus.
		/// </summary>
		/// <param name="focusEvent">raised event.</param>
		public virtual void OnFocusChangedEvent(FocusChangedEvent focusEvent) { }

		/// <summary>
		/// Event generated when the widget's drawing priority changed.
		/// </summary>
		/// <param name="orderEvent"></param>
		public virtual void OnRenderingOrderChangedEvent(RenderingOrderChangedEvent orderEvent) { }

        /// <summary>
        /// Event generated when the widget is clicked on.
        /// </summary>
        /// <param name="clickEvent"></param>
        public virtual void OnClickEvent(ClickEvent clickEvent) { }

        /// <summary>
        /// Event generated when the widget is hovered.
        /// </summary>
        /// <param name="clickEvent"></param>
        public virtual void OnHoverEvent(HoverEvent clickEvent) { }

        /// <summary>
        /// Event generated when the widget isn't anymore hovered.
        /// </summary>
        /// <param name="clickEvent"></param>
        public virtual void OnHoverEndEvent(HoverEndEvent clickEvent) { }

		#endregion
	}
}