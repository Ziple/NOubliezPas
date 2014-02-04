using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;

namespace kT.GUI
{
    /// <summary>
    /// Class that represent a cursor. The thing like
    /// an arrow you know.
    /// </summary>
    public class Cursor
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Cursor()
        {
            Position = new Vector2f( 0f, 0f);
            Visible = false;
            Image = null;
        }

        /// <summary>
        /// Get/set the position of the cursor.
        /// </summary>
        public Vector2f Position
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the visibility of the cursor.
        /// </summary>
        public bool Visible
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the image representing the cursor.
        /// </summary>
        public ImagePart Image
        {
            get;
            set;
        }
    }
}
