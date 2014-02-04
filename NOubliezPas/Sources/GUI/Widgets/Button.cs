using System;
using SFML.Window;
using SFML.Graphics;

namespace kT.GUI
{
	/// <summary>
	/// Basic class to implment buttons that can be clicked and hovered.
	/// </summary>
    public class Button : Frame
    {
        public enum ButtonState
        {
            Hovered,
            Clicked,
            Normal
        }

        Texture[] myHoveredFrameImages = new Texture[9];
        Texture[] myClickedFrameImages = new Texture[9];
        Texture[] myNormalFrameImages = new Texture[9];

        LayoutType myLayoutType = LayoutType.Horizontal;
        Layout myInnerLayout = null;
        Caption myCaption = null;
        Label myButtonText = null;

        ButtonState myButtonState = ButtonState.Normal;

        public Action Clicked = null;

        public Button(UIManager manager_, Widget parent_) :
            base(manager_, parent_)
        {
            myCaption = new Caption(manager_, this);
            myCaption.Visible = true;
            myButtonText = new Label(manager_, this);
            myButtonText.Visible = true;
            Layout = LayoutType.Horizontal;
            myInnerLayout.Visible = true;
            ContainedWidget = myInnerLayout;

			Manager.RegisterWidgetType("Button", "Frame");
        }

        public Texture[] HoveredBordersImages
        {
            get { return myHoveredFrameImages; }
            set
            {
                myHoveredFrameImages[0] = value[0];
                myHoveredFrameImages[1] = value[1];
                myHoveredFrameImages[2] = value[2];
                myHoveredFrameImages[3] = value[3];
                myHoveredFrameImages[4] = value[4];
                myHoveredFrameImages[5] = value[5];
                myHoveredFrameImages[6] = value[6];
                myHoveredFrameImages[7] = value[7];
                myHoveredFrameImages[8] = value[8];

                if (myButtonState == ButtonState.Hovered)
                    BordersImages = myHoveredFrameImages;
            }
        }

        public Texture[] NormalBordersImages
        {
            get { return myNormalFrameImages; }
            set
            {
                myNormalFrameImages[0] = value[0];
                myNormalFrameImages[1] = value[1];
                myNormalFrameImages[2] = value[2];
                myNormalFrameImages[3] = value[3];
                myNormalFrameImages[4] = value[4];
                myNormalFrameImages[5] = value[5];
                myNormalFrameImages[6] = value[6];
                myNormalFrameImages[7] = value[7];
                myNormalFrameImages[8] = value[8];

                if (myButtonState == ButtonState.Normal)
                    BordersImages = myNormalFrameImages;
            }
        }

        public Texture[] ClickedBordersImages
        {
            get { return myClickedFrameImages; }
            set
            {
                myClickedFrameImages[0] = value[0];
                myClickedFrameImages[1] = value[1];
                myClickedFrameImages[2] = value[2];
                myClickedFrameImages[3] = value[3];
                myClickedFrameImages[4] = value[4];
                myClickedFrameImages[5] = value[5];
                myClickedFrameImages[6] = value[6];
                myClickedFrameImages[7] = value[7];
                myClickedFrameImages[8] = value[8];

                if (myButtonState == ButtonState.Clicked)
                    BordersImages = myClickedFrameImages;
            }
        }

        public LayoutType Layout
        {
            get { return myLayoutType; }
            set {
                myLayoutType = value;
                if (myLayoutType == LayoutType.Horizontal)
                    myInnerLayout = new HorizontalLayout(Manager, this);
                else if (myLayoutType == LayoutType.Vertival)
                    myInnerLayout = new VerticalLayout(Manager, this);

                myInnerLayout.Add(myCaption);
                myInnerLayout.Add(myButtonText);

                ContainedWidget = myInnerLayout;
            }
        }

        public ImagePart Image
        {
            get { return myCaption.ImagePart; }
            set { myCaption.ImagePart = value; }
        }

        public string Text
        {
            get { return myButtonText.Text;  }
            set { myButtonText.Text = value; }
        }

        public DCFont Font
        {
            get { return myButtonText.Font; }
            set { myButtonText.Font = value; }
        }

        public Color TextColor
        {
            get { return myButtonText.TextColor; }
            set { myButtonText.TextColor = value; }
        }

        public ButtonState State
        {
            get { return myButtonState; }
            set {
                myButtonState = value;
                if (myButtonState == ButtonState.Normal)
                    BordersImages = NormalBordersImages;
                else if (myButtonState == ButtonState.Clicked)
                    BordersImages = ClickedBordersImages;
                else
                    BordersImages = HoveredBordersImages;
            }
        }

        /// <summary>
        /// Event generated when the widget is clicked on.
        /// </summary>
        /// <param name="clickEvent"></param>
        public override void OnClickEvent(ClickEvent clickEvent)
        {
            State = ButtonState.Clicked;

            if (Clicked != null)
                Clicked();
        }

        /// <summary>
        /// Event generated when the widget is hovered.
        /// </summary>
        /// <param name="clickEvent"></param>
        public override void OnHoverEvent(HoverEvent clickEvent)
        {
            // Being clicked is more important than hovered.
            if (State == ButtonState.Normal)
                State = ButtonState.Hovered;
        }

        /// <summary>
        /// Event generated when the widget is hovered.
        /// </summary>
        /// <param name="clickEvent"></param>
        public override void OnHoverEndEvent(HoverEndEvent clickEvent)
        {
            // Being clicked is more important than hovered.
            State = ButtonState.Normal;
        }

        public override Widget PickWidget(Vector2f pos)
        {
            return this;
        }
    }
}
