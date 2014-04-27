using SFML.Window;
using SFML.Graphics;

namespace kT.GUI
{
	public class Frame : Widget
	{
		public Color BackgroundColor = Color.Transparent;

		ImagePart[] myFrameImages = new ImagePart[9];

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
			get {
                Texture[] ar = new Texture[9];
                for (int i = 0; i < ar.Length; i++)
                    ar[i] = myFrameImages[i].SourceTexture;

                    return ar;
            }
			set
			{
                if( value != null )
                {
                    myFrameImages[0] = value[0] != null ? new ImagePart( value[0] ) : null;
                    myFrameImages[1] = value[1] != null ? new ImagePart( value[1] ) : null;
                    myFrameImages[2] = value[2] != null ? new ImagePart( value[2] ) : null;
                    myFrameImages[3] = value[3] != null ? new ImagePart( value[3] ) : null;
                    myFrameImages[4] = value[4] != null ? new ImagePart( value[4] ) : null;
                    myFrameImages[5] = value[5] != null ? new ImagePart( value[5] ) : null;
                    myFrameImages[6] = value[6] != null ? new ImagePart( value[6] ) : null;
                    myFrameImages[7] = value[7] != null ? new ImagePart( value[7] ) : null;
                    myFrameImages[8] = value[8] != null ? new ImagePart(value[8]) : null;
                }
                else
                {
                    for(uint i = 0; i < 9; i++)
                        myFrameImages[i] = null;
                }

				if (myContainedWidget != null
					&& myContainedWidget.Visible )
					base.ResizeForChild(myContainedWidget, myContainedWidget.Size);
			}
		}

        public ImagePart[] BordersImagesParts
        {
            get
            {
                return myFrameImages;
            }
            set
            {
                if (value != null)
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
                }
                else
                {
                    for (uint i = 0; i < 9; i++)
                        myFrameImages[i] = null;
                }

                if (myContainedWidget != null
                    && myContainedWidget.Visible)
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
					leftBorderSize = (int)myFrameImages[0].SourceRectangle.Width;
				if (myFrameImages[3] != null)
                    leftBorderSize = myFrameImages[3].SourceRectangle.Width > leftBorderSize ? (int)myFrameImages[3].SourceRectangle.Width : leftBorderSize;
				if (myFrameImages[6] != null)
                    leftBorderSize = myFrameImages[6].SourceRectangle.Width > leftBorderSize ? (int)myFrameImages[6].SourceRectangle.Width : leftBorderSize;

				return leftBorderSize;
			}
		}

		public int RightBorderSize
		{
			get
			{
				int rightBorderSize = 0;

				if (myFrameImages[2] != null)
                    rightBorderSize = (int)myFrameImages[2].SourceRectangle.Width;
				if (myFrameImages[5] != null)
                    rightBorderSize = myFrameImages[5].SourceRectangle.Width > rightBorderSize ? (int)myFrameImages[5].SourceRectangle.Width : rightBorderSize;
				if (myFrameImages[8] != null)
                    rightBorderSize = myFrameImages[8].SourceRectangle.Width > rightBorderSize ? (int)myFrameImages[8].SourceRectangle.Width : rightBorderSize;

				return rightBorderSize;
			}
		}

		public int TopBorderSize
		{
			get
			{
				int topBorderSize = 0;

				if (myFrameImages[2] != null)
                    topBorderSize = (int)myFrameImages[2].SourceRectangle.Height;
				if (myFrameImages[1] != null)
                    topBorderSize = myFrameImages[1].SourceRectangle.Height > topBorderSize ? (int)myFrameImages[1].SourceRectangle.Height : topBorderSize;
				if (myFrameImages[0] != null)
                    topBorderSize = myFrameImages[0].SourceRectangle.Height > topBorderSize ? (int)myFrameImages[0].SourceRectangle.Height : topBorderSize;
				return topBorderSize;
			}
		}

		public int BottomBorderSize
		{
			get
			{
				int bottomBorderSize = 0;

				if (myFrameImages[8] != null)
                    bottomBorderSize = (int)myFrameImages[8].SourceRectangle.Height;
				if (myFrameImages[7] != null)
                    bottomBorderSize = myFrameImages[7].SourceRectangle.Height > bottomBorderSize ? (int)myFrameImages[7].SourceRectangle.Height : bottomBorderSize;
				if (myFrameImages[6] != null)
                    bottomBorderSize = myFrameImages[6].SourceRectangle.Height > bottomBorderSize ? (int)myFrameImages[6].SourceRectangle.Height : bottomBorderSize;

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
				Vector2f size = new Vector2f(requestedSize.X, requestedSize.Y);
				size.X += LeftBorderSize + RightBorderSize + child.Position.X;
				size.Y += TopBorderSize + BottomBorderSize + child.Position.Y;
				return size;
			}
			return new Vector2f(0f,0f);
		}

        /// <summary>
        /// Returns the maximum size that can use a chikld without any parent resizing.
        /// Must be implemented by widgets like layouts, frames etc...
        /// </summary>
        /// <param name="child">Child willing to resize</param>
        /// <returns>Maximum size that can occupy the child.</returns>
        public override Vector2f GetMaxSizeForChild(Widget child)
        {
            FloatRect geom = GeometryRegion;
            return new Vector2f(geom.Width-child.Position.X, geom.Height-child.Position.Y);
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
					drawEvent.Painter.DrawImage(myFrameImages[0].SourceTexture,
												new FloatRect(geometryRegion.Left - myFrameImages[0].Size.X,
															  geometryRegion.Top - myFrameImages[0].Size.Y,
															  myFrameImages[0].Size.X,
															  myFrameImages[0].Size.Y),
                                                myFrameImages[0].SourceRectangle);

				if (myFrameImages[1] != null)
                    drawEvent.Painter.DrawImage(myFrameImages[1].SourceTexture,
												new FloatRect(geometryRegion.Left,
															  geometryRegion.Top - myFrameImages[1].Size.Y,
															  geometryRegion.Width,
                                                              myFrameImages[1].Size.Y),
                                                myFrameImages[1].SourceRectangle);


				if (myFrameImages[2] != null)
                    drawEvent.Painter.DrawImage(myFrameImages[2].SourceTexture,
												new FloatRect(geometryRegion.Left + geometryRegion.Width,
															  geometryRegion.Top - myFrameImages[2].Size.Y,
															  myFrameImages[2].Size.X,
                                                              myFrameImages[2].Size.Y),
                                                myFrameImages[2].SourceRectangle);

				if (myFrameImages[3] != null)
                    drawEvent.Painter.DrawImage(myFrameImages[3].SourceTexture,
												new FloatRect(geometryRegion.Left - myFrameImages[3].Size.X,
															  geometryRegion.Top,
															  myFrameImages[3].Size.X,
                                                              geometryRegion.Height),
                                                myFrameImages[3].SourceRectangle);

				if (myFrameImages[4] != null)
                    drawEvent.Painter.DrawImage(myFrameImages[4].SourceTexture,
                                                geometryRegion,
                                                myFrameImages[4].SourceRectangle);

				if (myFrameImages[5] != null)
                    drawEvent.Painter.DrawImage(myFrameImages[5].SourceTexture,
                                                new FloatRect(geometryRegion.Left + geometryRegion.Width,
															  geometryRegion.Top,
															  myFrameImages[5].Size.X,
                                                              geometryRegion.Height),
                                                myFrameImages[5].SourceRectangle);

				if (myFrameImages[6] != null)
                    drawEvent.Painter.DrawImage(myFrameImages[6].SourceTexture,
												new FloatRect(geometryRegion.Left - myFrameImages[6].Size.X,
                                                              geometryRegion.Top + geometryRegion.Height,
															  myFrameImages[6].Size.X,
                                                              myFrameImages[6].Size.Y),
                                                myFrameImages[6].SourceRectangle);
				if (myFrameImages[7] != null)
                    drawEvent.Painter.DrawImage(myFrameImages[7].SourceTexture,
												new FloatRect(geometryRegion.Left,
                                                              geometryRegion.Top + geometryRegion.Height,
															  geometryRegion.Width,
                                                              myFrameImages[7].Size.Y),
                                                myFrameImages[7].SourceRectangle);

				if (myFrameImages[8] != null)
                    drawEvent.Painter.DrawImage(myFrameImages[8].SourceTexture,
                                                new FloatRect(geometryRegion.Left + geometryRegion.Width,
                                                              geometryRegion.Top + geometryRegion.Height,
															  myFrameImages[8].Size.X,
                                                              myFrameImages[8].Size.Y),
                                                myFrameImages[8].SourceRectangle);
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