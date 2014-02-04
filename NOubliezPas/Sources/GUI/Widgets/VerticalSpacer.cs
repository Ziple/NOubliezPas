using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;

namespace kT.GUI
{
    public class VerticalSpacer : Widget
    {
        public VerticalSpacer(UIManager manager_) :
            base(manager_, null)
        {
            Manager.RegisterWidgetType("VerticalSpacer", "Widget");
        }

        public VerticalSpacer(UIManager manager_, Widget parent_) :
            base(manager_, parent_)
        {
            Manager.RegisterWidgetType("VerticalSpacer", "Widget");
        }

        public VerticalSpacer(UIManager manager_, Widget parent_, float length) :
            base(manager_, parent_)
        {
            Manager.RegisterWidgetType("VerticalSpacer", "Widget");
            Length = length;
        }

        public float MinLength
        {
            get { return MinSize.Y; }
            set { MinSize = new Vector2f(1f, value); }
        }
        public float MaxLength
        {
            get { return MaxSize.Y; }
            set { MaxSize = new Vector2f(1f, value); }
        }

        public float Length
        {
            get { return Size.Y; }
            set { Size = new Vector2f(1f, value); }
        }

        public override void OnDraw(DrawEvent drawEvent)
        {
            base.OnDraw(drawEvent);
            base.EndDraw(drawEvent);
        }
    }
}