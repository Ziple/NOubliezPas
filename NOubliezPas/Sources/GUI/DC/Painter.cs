using SFML.Graphics;
using SFML.Window;

namespace kT.GUI
{
	/// <summary>
	/// Interface between the widgets and the rendering API.
	/// </summary>
	public class Painter
	{
		#region Members
        RenderTarget myTarget;
        Texture myTexture;
        Sprite myRect;
		#endregion
		#region Construction
		public Painter(RenderTarget target)
		{
            myTarget = target;
            myTexture = new Texture(1, 1);

            byte[] pixels = new byte[4];
            pixels[0] = pixels[1] = pixels[2] = pixels[3] = 255;
            myTexture.Update(pixels);
            myTexture.Repeated = true;

            myRect = new Sprite(myTexture);
            myRect.Position = new Vector2f(0f, 0f);

            Tint = Color.White;
		}
		#endregion
		#region Accessor
		/// <summary>
		/// Get the sprite batch.
		/// </summary>
		public RenderTarget Target
		{
			get { return myTarget; }
            set { myTarget = value; }
		}

        /// <summary>
        /// Get the one texel texture.
        /// </summary>
        public Texture Texel
        {
            get { return myTexture; }
        }
		#endregion
		#region Translation things.
		/// <summary>
		/// Get/set the translation applied to draw commands.
		/// </summary>
		public Vector2f Translation
		{
			get;
			set;
		}

		/// <summary>
		/// Updates translation accordingly to the translation to be applied.
		/// </summary>
		/// <param name="tr">Translation to apply.</param>
		public void Translate(Vector2f tr)
		{
			Vector2f curTr = Translation;
			curTr.X += tr.X;
			curTr.Y += tr.Y;
			Translation = curTr;
		}

		/// <summary>
		/// Updates translation accordingly to the translation to be removed.
		/// </summary>
		/// <param name="tr">Translation to remove.</param>
		public void TranslateBack(Vector2f tr)
		{
			Vector2f curTr = Translation;
			curTr.X -= tr.X;
			curTr.Y -= tr.Y;
			Translation = curTr;
		}
		#endregion
		#region Tint color things

        Color myOldTint;
		public Color Tint
		{
			get;
			set;
		}

		private Color ActualColor(Color color )
		{
			Color actualColor = Color.Transparent;
			actualColor.R = (byte)(((uint)color.R * (uint)Tint.R) / 255);
            actualColor.G = (byte)(((uint)color.G * (uint)Tint.G) / 255);
            actualColor.B = (byte)(((uint)color.B * (uint)Tint.B) / 255);
            actualColor.A = (byte)(((uint)color.A * (uint)Tint.A) / 255);

			return actualColor;
		}

        private static Color ActualColor(Color color1, Color color2)
        {
            Color actualColor = Color.Transparent;
            actualColor.R = (byte)(((uint)color1.R * (uint)color1.R) / 255);
            actualColor.G = (byte)(((uint)color1.G * (uint)color1.G) / 255);
            actualColor.B = (byte)(((uint)color1.B * (uint)color1.B) / 255);
            actualColor.A = (byte)(((uint)color1.A * (uint)color1.A) / 255);

            return actualColor;

        }
		#endregion
		#region Draw operations
		/// <summary>
		/// Notice the beginning of the drawing.
		/// </summary>
		public void Begin()
		{
            myOldTint = Tint;
		}

		/// <summary>
		/// Notice the end of the drawing.
		/// </summary>
		public void End()
        {
            Tint = myOldTint;
        }

        /// <summary>
        /// Draws a rectangle with the according color.
        /// </summary>
        /// <param name="rect">Rectangle to draw.</param>
        /// <param name="color">Color of the rectangle.</param>
        public void DebugDrawRectangle(FloatRect rect, Color color)
        {
            myRect.Position = new Vector2f(rect.Left + Translation.X, rect.Top + Translation.Y);
            myRect.Scale = new Vector2f(rect.Width, 1f);
            myRect.Color = ActualColor(color);

            myRect.Draw(myTarget, RenderStates.Default);

            myRect.Position = new Vector2f(rect.Left + Translation.X, rect.Top + Translation.Y);
            myRect.Scale = new Vector2f(1f, rect.Height);
            myRect.Color = ActualColor(color);

            myRect.Draw(myTarget, RenderStates.Default);

            myRect.Position = new Vector2f(rect.Left + rect.Width + Translation.X, rect.Top + Translation.Y);
            myRect.Scale = new Vector2f(1f, rect.Height);
            myRect.Color = ActualColor(color);

            myRect.Draw(myTarget, RenderStates.Default);

            myRect.Position = new Vector2f(rect.Left + Translation.X, rect.Top + rect.Height + Translation.Y);
            myRect.Scale = new Vector2f(rect.Width, 1f);
            myRect.Color = ActualColor(color);

            myRect.Draw(myTarget, RenderStates.Default);
        }

		/// <summary>
		/// Draws a rectangle with the according color.
		/// </summary>
		/// <param name="rect">Rectangle to draw.</param>
		/// <param name="color">Color of the rectangle.</param>
		public void DrawRectangle(FloatRect rect, Color color)
		{
            myRect.Position = new Vector2f(rect.Left + Translation.X, rect.Top + Translation.Y);
            myRect.Scale = new Vector2f(rect.Width, rect.Height);
            myRect.Color = ActualColor(color);

            myRect.Draw(myTarget, RenderStates.Default);
		}

		/// <summary>
		/// Draws a rectangle with an image.
		/// If the image doesn't match the size of the rectangle, the image is repeated.
		/// </summary>
		/// <param name="img">Image to use.</param>
		/// <param name="rect">Rectangle to draw.</param>
		/// <param name="color">Tint to give to the image.</param>
		public void DrawImage(Texture img, FloatRect rect, Color color)
		{
            img.Repeated = true;
            Sprite srect = new Sprite(img);
            srect.Position = new Vector2f(rect.Left + Translation.X, rect.Top + Translation.Y);
            srect.Scale = new Vector2f(rect.Width / img.Size.X, rect.Height / img.Size.Y);
            srect.Color = ActualColor(color);

            srect.Draw(myTarget, RenderStates.Default);
		}

		/// <summary>
		/// Draws a rectangle with an image.
		/// If the image doesn't match the size of the rectangle, the image is repeated.
		/// </summary>
		/// <param name="img">Image to use.</param>
		/// <param name="rect">Rectangle to draw.</param>
		public void DrawImage(Texture img, FloatRect rect)
		{
            img.Repeated = true;
            Sprite srect = new Sprite(img);
            srect.Position = new Vector2f(rect.Left + Translation.X, rect.Top + Translation.Y);
            srect.Scale = new Vector2f(rect.Width / img.Size.X, rect.Height / img.Size.Y);
            srect.Color = Tint;

            srect.Draw(myTarget, RenderStates.Default);
		}

		/// <summary>
		/// Draws a rectangle with an image.
		/// If the image doesn't match the size of the rectangle, the image is repeated.
		/// </summary>
		/// <param name="img">Image to use.</param>
		/// <param name="imgSrcRect">Source region of the image to use.</param>
		/// <param name="rect">Rectangle to draw.</param>
		/// <param name="color">Tint to give to the image.</param>
		public void DrawImage(Texture img, FloatRect rect, IntRect imgSrcRect, Color color)
		{
            img.Repeated = true;
            Sprite srect = new Sprite(img, imgSrcRect);
            srect.Position = new Vector2f(rect.Left + Translation.X, rect.Top + Translation.Y);
            srect.Scale = new Vector2f(rect.Width / (float)imgSrcRect.Width, rect.Height / (float)imgSrcRect.Height);
            srect.Color = ActualColor(color);

            srect.Draw(myTarget, RenderStates.Default);
		}

		/// <summary>
		/// Draws a rectangle with an image.
		/// If the image doesn't match the size of the rectangle, the image is repeated.
		/// </summary>
		/// <param name="img">Image to use.</param>
		/// <param name="imgSrcRect">Source region of the image to use.</param>
		/// <param name="rect">Rectangle to draw.</param>
		public void DrawImage(Texture img, FloatRect rect, IntRect imgSrcRect)
		{
            DrawImage(img, rect, imgSrcRect, Color.White);
		}

		/// <summary>
		/// Draws a string.
		/// </summary>
		public void DrawString(Text str, Color color )
		{
            Vector2f oldPos = str.Position;
            str.Position = oldPos +Translation;
            str.Position -= new Vector2f( str.GetLocalBounds().Left, str.GetLocalBounds().Top );

            Color oldCol = str.Color;
            str.Color = ActualColor(color);

            str.Draw(myTarget, RenderStates.Default);

            str.Color = oldCol;
            str.Position = oldPos;
		}

		/// <summary>
		/// Draws a string.
		/// </summary>
		public void DrawString(Text str)
		{
            DrawString(str, Color.White);
		}
		#endregion
	}
}