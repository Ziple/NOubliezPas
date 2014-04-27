using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace kT.GUI
{
	/// <summary>
	/// Basic class to hold a font.
	/// </summary>
	public class DCFont
	{
        Font myFont;

		public DCFont(Font font)
		{
			if (font == null)
				throw new Exception("A font can't be created without a spritefont given for the normal text style");
            myFont = font;
		}

		public Font Font
		{
			get { return myFont; }
			set { myFont = value; }
		}

		public  Vector2f MeasureString(TextString str)
		{
			Vector2f size = new Vector2f(0f,0f);
            Vector2f curLineSize = new Vector2f(0f, 0f);

			foreach (KeyValuePair<TextStyle, string> s in str.FormatedText)
			{
				if (s.Key == TextStyle.EndLine)
				{
					size.Y += curLineSize.Y;
					size.X = curLineSize.X > size.X ? curLineSize.X : size.X;
                    curLineSize = new Vector2f(0f, 0f);
				}
				else
				{
                    Text textSlope = new Text(s.Value, Font, str.CharacterSize);
                    Text.Styles textStyle = Text.Styles.Regular;
                    if ((s.Key & TextStyle.Bold) != 0)
                        textStyle |= Text.Styles.Bold;
                    if( (s.Key & TextStyle.Italic) != 0)
                        textStyle |= Text.Styles.Italic;
                    textSlope.Style = textStyle;
                    FloatRect localBounds = textSlope.GetLocalBounds();

					Vector2f ssize = new Vector2f(localBounds.Width,localBounds.Height);
					curLineSize.X += (int)ssize.X;
					curLineSize.Y = (int)ssize.Y > curLineSize.Y ? (int)ssize.Y : curLineSize.Y;
				}
			}

			size.X = curLineSize.X > size.X ? curLineSize.X : size.X;
			size.Y += curLineSize.Y;
			return size;
		}
	}
}