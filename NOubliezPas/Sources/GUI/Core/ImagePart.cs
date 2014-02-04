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
    }
}
