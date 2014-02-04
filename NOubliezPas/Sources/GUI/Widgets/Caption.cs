using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;

namespace kT.GUI
{
	/// <summary>
	/// A caption can hold a part of an image
	/// and render it.
	/// </summary>
	public class Caption : Widget
	{
		ImagePart myImagePart = null;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="manager_"></param>
		public Caption(UIManager manager_) :
			base(manager_, null)
		{ }

		/// <summary>
		/// Default construtor too.
		/// </summary>
		/// <param name="manager_"></param>
		/// <param name="parent_"></param>
		public Caption(UIManager manager_, Widget parent_) :
			base(manager_, parent_)
		{
			Manager.RegisterWidgetType("Caption", "Widget");
		}

		/// <summary>
		/// Detailed constructor.
		/// </summary>
		/// <param name="manager_"></param>
		/// <param name="parent_"></param>
		/// <param name="imagePart"></param>
		public Caption(UIManager manager_, Widget parent_, ImagePart imagePart ) :
			base(manager_, parent_)
		{
			Manager.RegisterWidgetType("Caption", "Widget");
			ImagePart = imagePart;
		}

		/// <summary>
		/// Get/set the image part you want the caption to display.
		/// </summary>
		public ImagePart ImagePart
		{
			get { return myImagePart; }
			set { myImagePart = value; updateSize(); }
		}

		/// <summary>
		/// Resize the widget accordingly to the image part size.
		/// </summary>
		protected void updateSize()
		{
			if (myImagePart == null
				|| (myImagePart != null && myImagePart.SourceTexture == null) )
			{
				Resize(new Vector2f(0f,0f));
			}
			else
				Resize(new Vector2f(myImagePart.SourceRectangle.Width, myImagePart.SourceRectangle.Height));
		}

		/// <summary>
		/// Draw the component.
		/// </summary>
		/// <param name="drawEvent"></param>
		public override void OnDraw(DrawEvent drawEvent)
		{
			base.OnDraw(drawEvent);
			if( myImagePart != null
				&& myImagePart.SourceTexture != null )
				drawEvent.Painter.DrawImage( myImagePart.SourceTexture, LocalSpaceBoundingRectangle, myImagePart.SourceRectangle );
            base.EndDraw(drawEvent);
		}
	}
}
