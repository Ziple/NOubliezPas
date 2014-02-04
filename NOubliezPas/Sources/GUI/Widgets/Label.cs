using System.Collections.Generic;
using SFML.Window;
using SFML.Graphics;

namespace kT.GUI
{
	public class BasicLabel : Widget
	{
        DCFont myFont;
        string myText;
        TextStyle myStyle;

		Color myTextColor = Color.White;
        Vector2f myInnerTextSize = new Vector2f(0f,0f);

		public BasicLabel(UIManager manager_) :
			base(manager_, null)
		{
			Manager.RegisterWidgetType("BasicLabel", "Widget");
		}

		public BasicLabel(UIManager manager_, Widget parent_) :
			base(manager_, parent_)
		{
			Manager.RegisterWidgetType("BasicLabel", "Widget");
		}

		public BasicLabel(UIManager manager_, Widget parent_, DCFont font, TextStyle style, Color color, string str) :
			base(manager_, parent_)
		{
			Manager.RegisterWidgetType("BasicLabel", "Widget");

            myFont = font;
            myText = str;
            myStyle = style;
            myTextColor = color;
            updateSize();
		}

		public DCFont Font
		{
			get { return myFont; }
			set { myFont = value; updateSize(); }
		}

        public TextStyle TextStyle
        {
            get { return myStyle; }
        }

        public string StringText
        {
            get { return myText; }
        }

		public Color TextColor
		{
			get { return myTextColor; }
			set { myTextColor = value; }
		}

        public Vector2f InnerTextSize
		{
			get { return myInnerTextSize; }
		}

		protected void updateSize()
		{
			if (myFont == null || myText == null)
			{
				myInnerTextSize = new Vector2f(0f,0f);
				Resize(myInnerTextSize);
			}
			else
			{
				Vector2f size = myFont.MeasureString( new TextString(myText, myStyle) );
				myInnerTextSize = new Vector2f((int)size.X, (int)size.Y); //1 pixel rounded... Don't care!
				Resize(myInnerTextSize);
			}
		}

		public override void OnDraw(DrawEvent drawEvent)
		{
			base.OnDraw(drawEvent);

            Text myTextStr = new Text(myText, myFont.Font);
            myTextStr.Position = Position;

            Text.Styles st = Text.Styles.Regular;
            if ((myStyle & TextStyle.Bold) != 0)
                st |= Text.Styles.Bold;
            if ((myStyle & TextStyle.Italic) != 0)
                st |= Text.Styles.Italic;
            myTextStr.Style = st;

			// Ne tient pas compte du débordement...
			drawEvent.Painter.DrawString(myTextStr, myTextColor);

            base.EndDraw(drawEvent);
		}
	}

	public class Label : Widget
	{
		DCFont myFont = null;
		string myText = null;
		BasicLabel[] myBasicLabels = null;
		Vector2f myInnerTextSize = new Vector2f(0f,0f);

		public Label(UIManager manager_) :
			base(manager_, null)
		{
			Manager.RegisterWidgetType("Label", "Widget");
		}

		public Label(UIManager manager_, Widget parent_) :
			base(manager_, parent_)
		{
			Manager.RegisterWidgetType("Label", "Widget");
		}

		public Label(UIManager manager_, Widget parent_, DCFont font, string str) :
			base(manager_, parent_)
		{
			Manager.RegisterWidgetType("Label", "Widget");
			Font = font;
			Text = str;
		}

		public DCFont Font
		{
			get { return myFont; }
			set
			{
				myFont = value;
				UpdateSize();
			}
		}

		public string Text
		{
			get { return myText; }
			set
			{
				myText = value;

				rebuildTextCache();
				UpdateSize();
			}
		}

		Color myTextColor = Color.White;
		public Color TextColor
		{
			get { return myTextColor; }
            set { myTextColor = value; rebuildTextCache(); }
		}

		/// <summary>
		/// When we change the text of the label we must
		/// rebuild the list of basic label needed and their position.
		/// </summary>
		protected void rebuildTextCache()
		{
			TextString textString = new TextString(myText);
			myBasicLabels = new BasicLabel[textString.FormatedText.Count];

			List<KeyValuePair<TextStyle, string>> formatedText = textString.FormatedText;

			Vector2f pos = new Vector2f(0f,0f);
            Vector2f curLineSize = new Vector2f(0f,0f);

			for (int i = 0; i < formatedText.Count; i++)
			{
				myBasicLabels[i] = new BasicLabel(Manager, this, Font, formatedText[i].Key, TextColor, formatedText[i].Value);
				myBasicLabels[i].Visible = true;
				myBasicLabels[i].Position = pos;

				if (myBasicLabels[i].TextStyle == TextStyle.EndLine)
				{
					pos.X = 0;
					pos.Y += curLineSize.Y;
					curLineSize = new Vector2f(0f,0f);
				}
				else
				{
                    Vector2f vsize = myBasicLabels[i].Size;
					curLineSize.X += (int)vsize.X;
                    curLineSize.Y = (int)vsize.Y > curLineSize.Y ? (int)vsize.Y : curLineSize.Y;
					pos.X += (int)vsize.X;
				}
			}

			if (myFont != null)
				myInnerTextSize = myFont.MeasureString(textString);
			else
				myInnerTextSize = new Vector2f(0f,0f);
		}

		protected void UpdateSize()
		{
			if (myFont == null || myText == null)
				Resize(new Vector2f(0f,0f));
			else
				Resize(myInnerTextSize);
		}

		public override void OnDraw(DrawEvent drawEvent)
		{
			base.OnDraw(drawEvent);
			foreach( BasicLabel label in myBasicLabels )
			{
				// Not any chance that a basic label refuse to be drawn.
                if (Widget.Contains(LocalSpaceBoundingRectangle, label.BoundingRectangle))
                    label.OnDraw(drawEvent);
			}
		}
	}
}
