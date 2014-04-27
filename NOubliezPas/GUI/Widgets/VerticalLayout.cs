using SFML.Window;

namespace kT.GUI
{
	public enum HorizontalAlignment
	{
		Left,
		Center,
		Right
	}
	/// <summary>Class for vertical layouts.</summary>
	public class VerticalLayout : Layout
	{
		HorizontalAlignment myHorizontalAlign = HorizontalAlignment.Left;
		public VerticalLayout(UIManager manager_) :
			base(manager_)
		{
			Manager.RegisterWidgetType("VerticalLayout", "Layout");
		}

		public VerticalLayout(UIManager manager_, Widget parent_) :
			base(manager_, parent_)
		{
			Manager.RegisterWidgetType("VerticalLayout", "Layout");
		}

		public HorizontalAlignment Alignment
		{
			get { return myHorizontalAlign; }
			set { myHorizontalAlign = value; }
		}

		/// <summary>
		/// Method that must be implemented if you have a widget that contains widgets.
		/// This method compute the new widget size so that the child widget can be
		/// resized.
		/// </summary>
		/// <param name="child">Child widget requesting the resizement.</param>
		/// <param name="requestedSize">Size requested by the widget.</param>
		/// <returns></returns>
        protected override Vector2f _computeNewSizeForChild(Widget child, Vector2f requestedSize)
		{
            Vector2f size = new Vector2f(0f, 0f);

			foreach (Widget widg in Widgets)
			{
				if (child == widg)
				{
					size.Y += requestedSize.Y;
					size.X = requestedSize.X > size.X ? requestedSize.X : size.X;
				}
				else
				{
					size.Y += widg.Size.Y;
					size.X = widg.Size.X > size.X ? widg.Size.X : size.X;
				}
			}
			return size;
		}

		/// <summary>
		/// Updates the size of the layout.
		/// </summary>
		protected override void updateSize()
		{
            Vector2f size = new Vector2f(0f,0f);

			foreach (Widget widg in Widgets)
			{
				size.Y += widg.Size.Y;
				size.X = widg.Size.X > size.X ? widg.Size.X : size.X;
			}
			Size = size;
		}

		/// <summary>
		/// Updates the positions of the widgets in the layout.
		/// </summary>
		protected override void updatePositions()
		{
            Vector2f pos = new Vector2f(0f,0f);

            if (Alignment == HorizontalAlignment.Left)
                foreach (Widget widget in Widgets)
                {
                    if (widget.Visible)
                    {
                        widget.Position = pos;
                        pos.Y += widget.Size.Y;
                    }
                }
			else if (Alignment == HorizontalAlignment.Center)
				foreach (Widget widget in Widgets)
				{
					if (widget.Visible)
					{
						pos.X = (Size.X - widget.Size.X) / 2;
						widget.Position = pos;
						pos.Y += widget.Size.Y;
					}
				}
			else if (Alignment == HorizontalAlignment.Right)
				foreach (Widget widget in Widgets)
				{
					if (widget.Visible)
					{
						pos.X = Size.X - widget.Size.X;
						widget.Position = pos;
						pos.Y += widget.Size.Y;
					}
				}
		}

        /// <summary>
        /// Returns the maximum size that can use a chikld without any parent resizing.
        /// Must be implemented by widgets like layouts, frames etc...
        /// </summary>
        /// <param name="child">Child willing to resize</param>
        /// <returns>Maximum size that can occupy the child.</returns>
        public override Vector2f GetMaxSizeForChild(Widget child)
        {
            float maxSizeY = Size.Y;
            foreach (Widget widget in Widgets)
            {
                if (widget.Visible && widget != child)
                    maxSizeY -= widget.Size.Y;
            }

            return new Vector2f(Size.X, maxSizeY);
        }
	}
}

