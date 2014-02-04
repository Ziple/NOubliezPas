using SFML.Window;
using SFML.Graphics;

namespace kT.GUI
{
	public class Frame : Widget
	{
		public Color BackgroundColor = Color.Transparent;

		Texture[] myFrameImages = new Texture[9];

		Widget myContainedWidget;

		public Frame(UIManager manager_, Widget parent_) :
			base(manager_, parent_)
		{
			Manager.RegisterWidgetType("Frame", "Widget");
		}

		public Widget ContainedWidget
		{
			get { return myContainedWidget; }
			set
			{
				myContainedWidget = value;
				myContainedWidget.Parent = this;
				if( myContainedWidget != null
					&& myContainedWidget.Visible )
					base.ResizeForChild(myContainedWidget, myContainedWidget.Size);
			}
		}

		public Texture[] BordersImages
		{
			get { return myFrameImages; }
			set
			{
				myFrameImages[0] = value[0];
				myFrameImages[1] = value[1];
				myFrameImages[2] = value[2];
				myFrameImages[3] = value[3];
				myFrameImages[4] = value[4];
				myFrameImages[5] = value[5];
				myFrameImages[6] = value[6];
				myFrameImages[7] = value[7];
				myFrameImages[8] = value[8];

				if (myContainedWidget != null
					&& myContainedWidget.Visible )
					base.ResizeForChild(myContainedWidget, myContainedWidget.Size);
			}
		}

		/// <summary>
		/// Returns he rectangle that would be occupied by the borders without the contained widget.
		/// </summary>
		public FloatRect BordersRectangle
		{ get { return new FloatRect(0, 0, LeftBorderSize + RightBorderSize, TopBorderSize + BottomBorderSize); } }

		public int LeftBorderSize
		{
			get
			{
				int leftBorderSize = 0;

				if (myFrameImages[0] != null)
					leftBorderSize = (int)myFrameImages[0].Size.X;
				if (myFrameImages[3] != null)
                    leftBorderSize = myFrameImages[3].Size.X > leftBorderSize ? (int)myFrameImages[3].Size.X : leftBorderSize;
				if (myFrameImages[6] != null)
                    leftBorderSize = myFrameImages[6].Size.X > leftBorderSize ?(int) myFrameImages[6].Size.X : leftBorderSize;

				return leftBorderSize;
			}
		}

		public int RightBorderSize
		{
			get
			{
				int rightBorderSize = 0;

				if (myFrameImages[2] != null)
                    rightBorderSize = (int)myFrameImages[2].Size.X;
				if (myFrameImages[5] != null)
                    rightBorderSize = myFrameImages[5].Size.X > rightBorderSize ? (int)myFrameImages[5].Size.X : rightBorderSize;
				if (myFrameImages[8] != null)
                    rightBorderSize = myFrameImages[8].Size.X > rightBorderSize ? (int)myFrameImages[8].Size.X : rightBorderSize;

				return rightBorderSize;
			}
		}

		public int TopBorderSize
		{
			get
			{
				int topBorderSize = 0;

				if (myFrameImages[2] != null)
                    topBorderSize = (int)myFrameImages[2].Size.Y;
				if (myFrameImages[1] != null)
                    topBorderSize = myFrameImages[1].Size.Y > topBorderSize ? (int)myFrameImages[1].Size.Y : topBorderSize;
				if (myFrameImages[0] != null)
                    topBorderSize = myFrameImages[0].Size.Y > topBorderSize ? (int)myFrameImages[0].Size.Y : topBorderSize;
				return topBorderSize;
			}
		}

		public int BottomBorderSize
		{
			get
			{
				int bottomBorderSize = 0;

				if (myFrameImages[8] != null)
                    bottomBorderSize = (int)myFrameImages[8].Size.Y;
				if (myFrameImages[7] != null)
                    bottomBorderSize = myFrameImages[7].Size.Y > bottomBorderSize ? (int)myFrameImages[7].Size.Y : bottomBorderSize;
				if (myFrameImages[6] != null)
                    bottomBorderSize = myFrameImages[6].Size.Y > bottomBorderSize ? (int)myFrameImages[6].Size.Y : bottomBorderSize;

				return bottomBorderSize;
			}
		}

		/// <summary>
		/// Gets the rectangle that can be occupied by the children widgets.
		/// (Without any resizing).
		/// </summary>
		public FloatRect GeometryRegion
		{
			get
			{
				FloatRect region = LocalSpaceBoundingRectangle;

				region.Left += LeftBorderSize;
				region.Top += TopBorderSize;
				region.Width -= (region.Left + RightBorderSize);
				region.Height -= (region.Top + BottomBorderSize);

				return region;
			}
		}

		/// <summary>
		/// Return the geometry region rectangle in its own local space.
		/// </summary>
		public FloatRect InnerLocalSpaceGeometryRegion
		{
			get { FloatRect ls = GeometryRegion; ls.Left = ls.Top = 0; return ls; }
		}

		/// <summary>
		/// Method that must be implemented if you have a widget that contains widgets.
		/// This method compute the new widget size so that the child widget can be
		/// resized.
		/// </summary>
		/// <param name="child">Child widget requesting the resizement.</param>
		/// <param name="requestedSize">Size requested by the widget.</param>
		/// <returns>The necessary parent size.</returns>
		protected override Vector2f _computeNewSizeForChild(Widget child, Vector2f requestedSize)
		{
			if (child == ContainedWidget)
			{
				Vector2f size = requestedSize;
				size.X += LeftBorderSize + RightBorderSize + child.Position.X;
				size.Y += TopBorderSize + BottomBorderSize + child.Position.Y;
				return size;
			}
			return new Vector2f(0f,0f);
		}

		public override void OnDraw(DrawEvent drawEvent)
		{
			base.OnDraw(drawEvent);

			FloatRect geometryRegion = GeometryRegion;

			// Check if we can draw the borders.
			if( Widget.Contains(LocalSpaceBoundingRectangle,BordersRectangle) )
			{
				drawEvent.Painter.DrawRectangle(geometryRegion, BackgroundColor);

				if (myFrameImages[0] != null)
					drawEvent.Painter.DrawImage(myFrameImages[0],
												new FloatRect(geometryRegion.Left - myFrameImages[0].Size.X,
															  geometryRegion.Top - myFrameImages[0].Size.Y,
															  myFrameImages[0].Size.X,
															  myFrameImages[0].Size.Y) );

				if (myFrameImages[1] != null)
					drawEvent.Painter.DrawImage(myFrameImages[1],
												new FloatRect(geometryRegion.Left,
															  geometryRegion.Top - myFrameImages[1].Size.Y,
															  geometryRegion.Width,
															  myFrameImages[1].Size.Y) );


				if (myFrameImages[2] != null)
					drawEvent.Painter.DrawImage(myFrameImages[2],
												new FloatRect(geometryRegion.Left + geometryRegion.Width,
															  geometryRegion.Top - myFrameImages[2].Size.Y,
															  myFrameImages[2].Size.X,
															  myFrameImages[2].Size.Y) );

				if (myFrameImages[3] != null)
					drawEvent.Painter.DrawImage(myFrameImages[3],
												new FloatRect(geometryRegion.Left - myFrameImages[3].Size.X,
															  geometryRegion.Top,
															  myFrameImages[3].Size.X,
															  geometryRegion.Height) );

				if (myFrameImages[4] != null)
					drawEvent.Painter.DrawImage(myFrameImages[4], geometryRegion, Color.White);

				if (myFrameImages[5] != null)
					drawEvent.Painter.DrawImage(myFrameImages[5],
                                                new FloatRect(geometryRegion.Left + geometryRegion.Width,
															  geometryRegion.Top,
															  myFrameImages[5].Size.X,
															  geometryRegion.Height) );

				if (myFrameImages[6] != null)
					drawEvent.Painter.DrawImage(myFrameImages[6],
												new FloatRect(geometryRegion.Left - myFrameImages[6].Size.X,
                                                              geometryRegion.Top + geometryRegion.Height,
															  myFrameImages[6].Size.X,
															  myFrameImages[6].Size.Y) );
				if (myFrameImages[7] != null)
					drawEvent.Painter.DrawImage(myFrameImages[7],
												new FloatRect(geometryRegion.Left,
                                                              geometryRegion.Top + geometryRegion.Height,
															  geometryRegion.Width,
															  myFrameImages[7].Size.Y) );

				if (myFrameImages[8] != null)
					drawEvent.Painter.DrawImage(myFrameImages[8],
                                                new FloatRect(geometryRegion.Left + geometryRegion.Width,
                                                              geometryRegion.Top + geometryRegion.Height,
															  myFrameImages[8].Size.X,
															  myFrameImages[8].Size.Y) );
			}

			// We also check if we can draw the inner widget.
			if (myContainedWidget != null
				&& myContainedWidget.Visible
				&& Widget.Contains(InnerLocalSpaceGeometryRegion,myContainedWidget.BoundingRectangle) )
			{
				myContainedWidget.OnEvent(drawEvent);
				if (drawEvent.Accepted)
				{
					Vector2f vec = new Vector2f(
						geometryRegion.Left + myContainedWidget.Position.X,
						geometryRegion.Top + myContainedWidget.Position.Y );
					drawEvent.Painter.Translate(vec);
					myContainedWidget.OnDraw(drawEvent);
					drawEvent.Painter.TranslateBack(vec);
				}
			}

            base.EndDraw(drawEvent);
		}

		/// <summary>
		/// Return the managed widget at the given position.
		/// </summary>
		/// <param name="pos">Point in the frame to check.</param>
		/// <returns>The reference to the pointed widget if pointed.</returns>
		public override Widget PickWidget(Vector2f pos)
		{
			if (myContainedWidget != null)
			{
				Vector2f topLeft = new Vector2f(LeftBorderSize, TopBorderSize);
				pos.X -= topLeft.X;
				pos.Y -= topLeft.Y;
				if (myContainedWidget.Contains(pos))
				{
					pos.X -= myContainedWidget.Position.X;
					pos.Y -= myContainedWidget.Position.Y;
					return myContainedWidget.PickWidget(pos);
				}
				else
					return this;
			}
			return this;
		}
	}
}