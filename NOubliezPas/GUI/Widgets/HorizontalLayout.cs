using SFML.Window;

namespace kT.GUI
{
	/// <summary>
	/// Enumerations of the possible vertical alignments.
	/// </summary>
	public enum VerticalAlignment
	{
		Top,
		Center,
		Bottom
	}

	/// <summary>Class for the horizontal layouts.</summary>
	public class HorizontalLayout : Layout
	{
		VerticalAlignment myVerticalAlign = VerticalAlignment.Top;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="manager_"></param>
		public HorizontalLayout(UIManager manager_) :
			base(manager_)
		{
			Manager.RegisterWidgetType("HorizontalLayout", "Layout");
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="manager_"></param>
		/// <param name="parent_"></param>
		public HorizontalLayout(UIManager manager_, Widget parent_) :
			base(manager_, parent_)
		{
			Manager.RegisterWidgetType("HorizontalLayout", "Layout");
		}

		/// <summary>
		/// Alignment of the widget.
		/// </summary>
		public VerticalAlignment Alignment
		{
			get { return myVerticalAlign; }
			set { myVerticalAlign = value; }
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
            Vector2f size = new Vector2f(0f,0f);

			foreach (Widget widg in Widgets)
			{
				if (child == widg)
				{
					size.X += requestedSize.X;
					size.Y = requestedSize.Y > size.Y ? requestedSize.Y : size.Y;
				}
				else
				{
					size.X += widg.Size.X;
					size.Y = widg.Size.Y > size.Y ? widg.Size.Y : size.Y;
				}
			}
			return size;
		}

		/// <summary>
		/// Implementation for this class.
		/// </summary>
		protected override void updateSize()
		{
            Vector2f size = new Vector2f(0f,0f);

			foreach (Widget widg in Widgets)
			{
				size.X += widg.Size.X;
				size.Y = widg.Size.Y > size.Y ? widg.Size.Y : size.Y;
			}

			Size = size;
		}

		/// <summary>
		/// Updates the positions of the widgets in the layout.
		/// </summary>
		protected override void updatePositions()
		{
            Vector2f pos = new Vector2f(0f,0f);
            if (Alignment == VerticalAlignment.Top)
			    foreach (Widget widget in Widgets)
			    {
				    if (widget.Visible)
				    {
					    widget.Position = pos;
					    pos.X += widget.Size.X;
				    }
			    }
            else if (Alignment == VerticalAlignment.Center)
                foreach (Widget widget in Widgets)
                {
                    if (widget.Visible)
                    {
                        pos.Y = (Size.Y - widget.Size.Y) / 2;
                        widget.Position = pos;
                        pos.X += widget.Size.X;
                    }
                }
            if (Alignment == VerticalAlignment.Bottom)
                foreach (Widget widget in Widgets)
                {
                    if (widget.Visible)
                    {
                        pos.Y = Size.Y - widget.Size.Y;
                        widget.Position = pos;
                        pos.X += widget.Size.X;
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
            float maxSizeX = Size.X;
            foreach (Widget widget in Widgets)
            {
                if (widget.Visible && widget != child)
                    maxSizeX -= widget.Size.X;
            }

            return new Vector2f(maxSizeX, Size.Y);
        }
	}
}