using System;
using System.Collections.Generic;
using System.Text;
using SFML.Window;

namespace kT.GUI
{
    public enum LayoutType
    {
        Vertival,
        Horizontal
    }

	/// <summary>Base class for layouts.</summary>
	public class Layout : Widget
	{
		List<Widget> widgets = new List<Widget>();

        bool resizingAllowed = true;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="manager_"></param>
		public Layout(UIManager manager_) :
			base(manager_, null)
		{
			Manager.RegisterWidgetType("Layout", "Widget");
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="manager_"></param>
		/// <param name="parent_"></param>
		public Layout(UIManager manager_, Widget parent_) :
			base(manager_, parent_)
		{
			Manager.RegisterWidgetType("Layout", "Widget");
		}

		/// <summary>Returns the number of widgets in the layout.</summary>
		public Int32 NumObjects
		{
			get { return widgets.Count; }
		}

		/// <summary>
		/// Return the list of widgets contained in the layout.
		/// </summary>
		public List<Widget> Widgets { get { return widgets; } }

		/// <summary>
		/// Adds a widget at the end.
		/// When you add a widget to a layout, it's Position will be changed
		/// to fit with the one the layout want to assign to it.
		/// </summary>
		public void Add(Widget obj)
		{
			obj.Parent = this;
			widgets.Add(obj);

			// No need to update size if the widget isn't visible.
			if (obj.Visible)
			{
				updateSize();
				updatePositions();
			}
		}

		/// <summary>
		/// Adds a widget at the specified index.
		/// When you insert a widget to a layout, it's Position will be changed
		/// to fit with the one the layout want to assign to it.
		/// </summary>
		public void Insert(Int32 index, Widget obj)
		{
			obj.Parent = this;
			widgets.Insert(index, obj);

			// No need to update size if the widget isn't visible.
			if (obj.Visible)
			{
				updateSize();
				updatePositions();
			}
		}

		/// <summary>Removes the widget from the layout.</summary>
		public void Remove(Widget obj)
		{
			obj.Parent = null;
			widgets.Remove(obj);

			// No need to update size if the widget wasn't visible.
			if (obj.Visible)
			{
				updateSize();
				updatePositions();
			}
		}

		/// <summary>
		/// Updates the size of the layout.
		/// </summary>
		protected virtual void updateSize() {}

		/// <summary>
		/// Updates the positions of the widgets in the layout.
		/// </summary>
		protected virtual void updatePositions() { }

		/// <summary>
		/// Implementation of pick widget for layout classes.
		/// </summary>
		/// <param name="pos">Position of the widget.</param>
		/// <returns>A reference to the widget or null if no one found.</returns>
		public override Widget PickWidget(Vector2f pos)
		{
			Widget innerPickedWidget = null;

			foreach (Widget widget in widgets)
			{
				if (widget.Contains(pos))
				{
					innerPickedWidget = widget;
					break;
				}
			}

			if (innerPickedWidget != null)
			{
				pos.X -= innerPickedWidget.Position.X;
				pos.Y -= innerPickedWidget.Position.Y;
				return innerPickedWidget.PickWidget(pos);
			}

			return null;
		}

        public override void EndChildResize(Widget child, Vector2f size)
        {
            base.EndChildResize(child, size);
            updatePositions();
        }

		/// <summary>
		/// Draws the widgets in the layout.
		/// </summary>
		/// <param name="drawEvent">Draw event.</param>
		public override void OnDraw(DrawEvent drawEvent)
		{
			base.OnDraw(drawEvent);

			foreach (Widget widget in Widgets)
			{
				drawEvent.Accepted = true;
				if (widget.Visible
					&& Widget.Contains( LocalSpaceBoundingRectangle, widget.BoundingRectangle) )
				{
					widget.OnEvent(drawEvent);
					if (drawEvent.Accepted)
					{
						drawEvent.Painter.Translate(widget.Position);
						widget.OnDraw(drawEvent);
						drawEvent.Painter.TranslateBack(widget.Position);
					}
				}
			}

            base.EndDraw(drawEvent);
		}
	}
}