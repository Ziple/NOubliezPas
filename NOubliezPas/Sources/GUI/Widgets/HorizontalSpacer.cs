using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;

namespace kT.GUI
{
    public class HorizontalSpacer: Widget
    {
        public HorizontalSpacer(UIManager manager_) :
			base(manager_, null)
		{
            Manager.RegisterWidgetType("HorizontalSpacer", "Widget");
		}

		public HorizontalSpacer(UIManager manager_, Widget parent_) :
			base(manager_, parent_)
		{
            Manager.RegisterWidgetType("HorizontalSpacer", "Widget");
		}

        public HorizontalSpacer(UIManager manager_, Widget parent_, float length ) :
			base(manager_, parent_)
		{
            Manager.RegisterWidgetType("HorizontalSpacer", "Widget");
            Length = length;
		}

        public float MinLength
        {
            get { return MinSize.X; }
            set { MinSize = new Vector2f(value, 1f); }
        }
        public float MaxLength
        {
            get { return MaxSize.X; }
            set { MaxSize = new Vector2f(value, 1f); }
        }

        public float Length
        {
            get { return Size.X; }
            set { Size = new Vector2f( value, 1f ); }
        }

		public override void OnDraw(DrawEvent drawEvent)
		{
			base.OnDraw(drawEvent);
            base.EndDraw(drawEvent);
		}
    }
}
