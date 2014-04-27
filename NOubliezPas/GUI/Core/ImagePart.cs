using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace kT.GUI
{
    public class ImagePart
    {
        public ImagePart( Texture tex )
        {
            SetFromTexture(tex);
        }
        public ImagePart( Texture tex, IntRect rect )
        {
            SourceTexture = tex;
            SourceRectangle = rect;
        }

        public Texture SourceTexture
        {
            get;
            set;
        }

        public IntRect SourceRectangle
        {
            get;
            set;
        }

        public Vector2f Size
        {
            get { return new Vector2f(SourceRectangle.Width, SourceRectangle.Height);  }
        }

        public void SetFromTexture( Texture tex )
        {
            SourceTexture = tex;
            Vector2u texSize = tex.Size;
            SourceRectangle = new IntRect(0, 0, (int)texSize.X, (int)texSize.Y);
        }
    }
}
