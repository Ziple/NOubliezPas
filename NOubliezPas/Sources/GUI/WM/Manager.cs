using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;
using System.Diagnostics;

namespace kT.GUI
{
	/// <summary>
	/// The GUI manager is the object that dispatches all input
	/// and performs drawing operations and so on.
	/// </summary>
	public class UIManager
	{
        RenderTarget myTarget;
		Painter myPainter;

		/// <summary>
		/// A list of the managed children.
		/// </summary>
		List<Widget> myManagedWidgets = new List<Widget>();

		/// <summary>
		/// The currently focused widgets.
		/// As there are four player at most, there are 4
		/// focused widgets at most too.
		/// </summary>
		Widget myFocusedWidget = null;
        Widget myHoveredWidget = null;

        /// <summary>
        /// Array of the cursor as they are known to be.
        /// </summary>
        Cursor myCursor = null;

        /// <summary>
        /// Dictionnary holding the reference of relationships
        /// between the widgets.
        /// Widget class name/its ancestor.
        /// </summary>
        Dictionary<string, string> myWidgetTypeTree = new Dictionary<string, string>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="device_">Graphics device to use to draw the widgets.</param>
		/// <param name="contentManager_">Content manager used ton loaad the textures and such.</param>
		public UIManager(RenderTarget target )
		{
            myTarget = target;
			myPainter = new Painter(myTarget);

            myCursor = new Cursor();
		}

		/// <summary>Get the graphics device.</summary>
		public RenderTarget Target
		{
			get { return myTarget; }
		}

        /// <summary>
        /// Get the size of the screen (actually the size of the back buffer).
        /// </summary>
        public Vector2f ScreenSize
        {
            get
            {
                return new Vector2f( myTarget.Size.X, myTarget.Size.Y );
            }
        }

		/// <summary>Get the painter used by the manager.</summary>
        public Painter Painter
        {
            get { return myPainter; }
        }

        /// <summary>Get/set the cursors.</summary>
        public Cursor Cursor
        {
            set { myCursor = value; }
            get { return myCursor; }
        }

        /// <summary>
        /// Get the widget type tree.
        /// </summary>
        public Dictionary<string, string> WidgetTypeTree
        {
            get { return myWidgetTypeTree; }
        }

        /// <summary>
        /// Tells wether or not the widget type is already referenced or not.
        /// </summary>
        /// <param name="widgetClassName">Name of the widget class.</param>
        /// <returns>True if already in, false otherwise.</returns>
        public bool IsTypeIn(string widgetClassName)
        {
            return myWidgetTypeTree.ContainsKey(widgetClassName);
        }

        /// <summary>
        /// Add the widget definition to the type-tree.
        /// </summary>
        /// <param name="widgetClassName">Name of the widget class.</param>
        /// <param name="widgetAncestor">Name of the ancestor class of the widget.</param>
        public void RegisterWidgetType(string widgetClassName, string widgetAncestor)
        {
            if (!IsTypeIn(widgetClassName))
                myWidgetTypeTree.Add(widgetClassName, widgetAncestor);
        }

		/// <summary>
		/// Must be called by a widget if it doesn't have any parent.
        /// Will affect the style of the widget by applying the default style to it.
		/// </summary>
		/// <param name="widget"></param>
		public void ManageWidget(Widget widget)
		{
            if (widget != null)
                myManagedWidgets.Add(widget);
		}

		/// <summary>
		/// Must be called if a widget gain a parent.
		/// </summary>
		/// <param name="widget"></param>
		public void UnmanageWidget(Widget widget)
		{
			if (myManagedWidgets.Contains(widget))
				myManagedWidgets.Remove(widget);
		}

		/// <summary>
		/// Notify the ui manager that an event occured.
		/// </summary>
		/// <param name="evt">Event raised.</param>
		public void NotifyEvent(Event evt)
		{
			// check for focus change due to mouse pressure or so.
			if (evt.Type == Event.EventType.MouseEvent)
			{
				MouseEvent mouseEvt = (MouseEvent)evt;
                    Widget receiver = PickWidget(mouseEvt.Position);

				if (mouseEvt.StateChange == MouseEvent.MouseStateChange.ButtonPressed)
                {
                    // The case where the receiver is null is finely handled implicitly.
                    if (mouseEvt.Button == Mouse.Button.Left)
                    {
                        Widget clickedWindow = PickTopWidget(mouseEvt.Position);

                        // Give focus to the widget if it doesn't have the focus yet
                        if (HasFocus(receiver) == false)
                        {
                            // No
                            // try to give focus to this widget:
                            if (SetFocus(receiver) == false)
                            {
                                // refused for X reason, try to give it to the top widget:
                                if (HasFocus(clickedWindow) == false)
                                    SetFocus(clickedWindow);
                            }
                        }

                        // place the clicked window on top anyway.
                        PushWidgetOnTop(clickedWindow);
                    }

                    if (receiver != null)
                    {
                        //In all the cases, we generate a click event.
                        ClickEvent clickEvent = new ClickEvent(mouseEvt.Button);
                        receiver.OnEvent(evt);

                        if (clickEvent.Accepted)
                            receiver.OnClickEvent(clickEvent);
                    }
                }
                else if (mouseEvt.StateChange == MouseEvent.MouseStateChange.Moved)
                    myCursor.Position = mouseEvt.Position;
			}

			// dispatch the event.
			Widget widget = myFocusedWidget;

			if (widget != null)
			{
				widget.OnEvent(evt);
				widget.DispatchEvent(evt);
			}
		}

		/// <summary>
		/// Tells wether or not this widget has the focus.
		/// </summary>
		/// <param name="widget">Widget to test.</param>
		/// <returns>True if the widget has the focus, false otherwise.</returns>
		public bool HasFocus(Widget widget)
		{
			return myFocusedWidget == widget;
		}

		/// <summary>
		/// If possible, gives the focus to the specified widget.
		/// It's possible that the widget that owned previously the focus
		/// REFUSE to give the focus to the one you want to give it.
		/// It's also possible that the desired receiver REFUSES
		/// to get the focus.
		/// If you pass a null reference in focusReceiver, then no one
		/// widget will get the focus from this player.
		/// </summary>
		/// <param name="index">Associated player to the focus.</param>
		/// <param name="focusReceiver">New receiver of the focus.</param>
		/// <returns>True if the widget received the focus, false otherwise.</returns>
		public bool SetFocus(Widget focusReceiver)
		{
			FocusChangedEvent evt = new FocusChangedEvent(FocusChangedEvent.FocusStateChange.LostFocus);

			Widget oldFocusedWidget = myFocusedWidget;

			// the old owner of the focus can refuse the lost of the focus.
			if (oldFocusedWidget != null)
			{
				oldFocusedWidget.OnEvent(evt);

				if (evt.Accepted)
				{
					oldFocusedWidget.OnFocusChangedEvent(evt);

					if (evt.Accepted)
						myFocusedWidget = null;
				}
			}

			// the old owner is ready to loose focus
			// or there is no old owner.
			if (evt.Accepted && focusReceiver != null)
			{
				if (HasFocus(focusReceiver) == false)
				{
					// the focus receiver doesn't have the focus from another player.
					evt.StateChange = FocusChangedEvent.FocusStateChange.GainedFocus;

					focusReceiver.OnEvent(evt);

					if (evt.Accepted)
					{
						focusReceiver.OnFocusChangedEvent(evt);
						if (evt.Accepted)
						{
							//the widget has accepted the focus.
							myFocusedWidget = focusReceiver;
						}
					}
				}
				else
					evt.Accepted = false;//the receiver has the focus from another player.
			}

			return evt.Accepted;
		}

		/// <summary>
		/// Place the widget as much as possible on the top of the rendering stack.
		/// </summary>
		/// <param name="widget">Widget to push on top of the rendering stack.</param>
		void PushWidgetOnTop(Widget widget)
		{
			if (myManagedWidgets.Contains(widget) && myManagedWidgets.Count > 1 )
			{
				int index = myManagedWidgets.FindIndex(delegate(Widget widget_) { return widget_ == widget; });
				myManagedWidgets.RemoveAt(index);

				RenderingOrderChangedEvent evt = new RenderingOrderChangedEvent(RenderingOrderChangedEvent.RenderingOrderChange.RenderedLater);

				//now we must find a new place for this poor widget ;)
				for (int i = myManagedWidgets.Count - 1; i >= 0; i--)
				{
					evt.Accepted = true;
					myManagedWidgets[i].OnEvent(evt);

					if (evt.Accepted)
					{
						myManagedWidgets[i].OnRenderingOrderChangedEvent(evt);

						if (evt.Accepted)
						{
							myManagedWidgets.Insert(i + 1, widget);
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Update all the managed widgets according to the input devices states.
		/// </summary>
		/// <param name="gameTime">game time reference</param>
		public void Update(Stopwatch gameTime)
		{
            /// Dispatch the hovering events.
            Widget receiver = PickWidget(myCursor.Position);
            if (myHoveredWidget != receiver)
            {
                if (myHoveredWidget != null)
                {
                    HoverEndEvent evt = new HoverEndEvent();

                    // the old owner of the focus can refuse the lost of the focus.
                    myHoveredWidget.OnEvent(evt);

                    if (evt.Accepted)
                        myHoveredWidget.OnHoverEndEvent(evt);
                }

                myHoveredWidget = receiver;
                if (receiver != null)
                {
                    HoverEvent evt = new HoverEvent();

                    // the old owner of the focus can refuse the lost of the focus.
                    receiver.OnEvent(evt);

                    if (evt.Accepted)
                        receiver.OnHoverEvent(evt);
                }
            }

			foreach (Widget widget in myManagedWidgets)
			{
				UpdateEvent raisedEvent = new UpdateEvent(gameTime);
				widget.OnEvent(raisedEvent);

				if (raisedEvent.Accepted)
					widget.OnUpdate(raisedEvent);
			}
		}

		/// <summary>
		/// Render all the widgets and cursors.
		/// </summary>
		public void Render( bool debug )
		{
			myPainter.Begin();
			foreach (Widget widget in myManagedWidgets)
			{
				if (widget.Visible)
				{
					DrawEvent drawEvent = new DrawEvent(myPainter, debug);
					widget.OnEvent(drawEvent);

					if (drawEvent.Accepted)
					{
						Vector2f pos = widget.Position;
						drawEvent.Painter.Translate(pos);
						widget.OnDraw(drawEvent);
						drawEvent.Painter.TranslateBack(pos);
					}
				}
			}

            if (myCursor != null)
            {
                if( myCursor.Visible )
                    if( myCursor.Image != null )
                    {
                        FloatRect destRect = new FloatRect(
                            myCursor.Position.X,
                            myCursor.Position.Y,
                            myCursor.Image.SourceRectangle.Width,
                            myCursor.Image.SourceRectangle.Height);
                        myPainter.DrawImage(myCursor.Image.SourceTexture, destRect, myCursor.Image.SourceRectangle);
                    }
                    else
                    {
                        FloatRect destRect = new FloatRect(
                            myCursor.Position.X,
                            myCursor.Position.Y,
                            5,
                            5);
                        myPainter.DrawRectangle(destRect, Color.White);
                    }
            }
			myPainter.End();
		}

        public void Render()
        {
            Render(false);
        }

		/// <summary>
		/// Return the managed widget at the given position.
		/// </summary>
		/// <param name="pos">Position of the widget.</param>
		public Widget PickTopWidget(Vector2f pos)
		{
			Widget ret = null;

			for (int i = myManagedWidgets.Count - 1; i >= 0; i--)
			{
				Widget widget = myManagedWidgets[i];
				if (widget.BoundingRectangle.Contains(pos.X, pos.Y))
				{
					ret = widget;
					break;
				}
			}

			return ret;
		}

		/// <summary>
		/// Return the widget at the given position (the managed one or one of its own children).
		/// </summary>
		/// <param name="pos">Position of the widget.</param>
		public Widget PickWidget(Vector2f pos)
		{
			Widget ret = PickTopWidget(pos);

			if (ret != null)
			{
				pos.X -= ret.Position.X;
				pos.Y -= ret.Position.Y;

				Widget innerRet = ret.PickWidget(pos);

				ret = innerRet != null ? innerRet : ret;
			}

			return ret;
		}
	}
}
