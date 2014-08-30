using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using System.IO;
using System.Xml;
using kT.GUI;

namespace NOubliezPas
{
    enum TextureDisplayMode
    {
        Stretch,
        Center
    }

    class FrameTexturesLoader
    {
        public static ImagePart LoadTexture( XmlReader reader )
        {
            ImagePart tex = null;
            String r = reader.GetAttribute("src");
            if (r != null)
            {
                tex = new ImagePart(new Texture(r));

                String srcX = reader.GetAttribute("srcX");
                String srcY = reader.GetAttribute("srcY");
                String srcW = reader.GetAttribute("srcW");
                String srcH = reader.GetAttribute("srcH");

                if ((srcX != null) && (srcY != null) && (srcW != null) && (srcH != null))
                {
                    IntRect srcRec = new IntRect(int.Parse(srcX), int.Parse(srcY), int.Parse(srcW), int.Parse(srcH));
                    tex.SourceRectangle = srcRec;
                }
            }

            return tex;
        }
        public static ImagePart[] Load(XmlReader reader)
        {
            ImagePart[] textures = new ImagePart[9];

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            if (reader.Name == "bordhautgauche")
                                textures[0] = LoadTexture(reader);
                            else if (reader.Name == "milieuhaut")
                                textures[1] = LoadTexture(reader);
                            else if (reader.Name == "bordhautdroit")
                                textures[2] = LoadTexture(reader);
                            else if (reader.Name == "milieugauche")
                                textures[3] = LoadTexture(reader);
                            else if (reader.Name == "milieu")
                                textures[4] = LoadTexture(reader);
                            else if (reader.Name == "milieudroit")
                                textures[5] = LoadTexture(reader);
                            else if (reader.Name == "bordbasgauche")
                                textures[6] = LoadTexture(reader);
                            else if (reader.Name == "milieubas")
                                textures[7] = LoadTexture(reader);
                            else if (reader.Name == "bordbasdroit")
                                textures[8] = LoadTexture(reader);
                            break;
                        }
                }
            }

            return textures;
        }
    }
    class ThemeSelectionMenuStyle
    {
        public Color BackgroundColor;
        public ImagePart BackgroundImage;
        public TextureDisplayMode BackgroundDisplayMode;
        public ImagePart[] LabelsTextures;
        public ImagePart[] HoveredLabelsTextures;
        public ImagePart[] PlayersNamesTextures;
        public ImagePart[] PlayersPicsTextures;
        public float ThemesLabelsBottomSpace;
        public float ThemesListRightSpace;
        public float PlayersPhotoSpace;
        public float PlayersSpace;
        public Vector2f PicsSize;
        public Font Font;
        public Color FontNormalColor;
        public Color FontHoveredColor;
        public uint FontSize;


        public ThemeSelectionMenuStyle(
            Color backgroundColor,
            ImagePart backgroundImage,
            TextureDisplayMode backgroundDisplayMode,
            ImagePart[] labelsTextures,
            ImagePart[] hoveredLabelsTextures,
            ImagePart[] playersNamesTextures,
            ImagePart[] playersPicsTextures,
            float themesLabelsBottomSpace,
            float themesListRightSpace,
            float playersPhotoSpace,
            float playersSpace,
            Vector2f picsSize,
            Font font,
            Color fontNormalColor,
            Color fontHoveredColor,
            uint fontSize
            )
        {
            BackgroundColor = backgroundColor;
            BackgroundImage = backgroundImage;
            BackgroundDisplayMode = backgroundDisplayMode;

            LabelsTextures = labelsTextures;
            HoveredLabelsTextures = hoveredLabelsTextures;

            PlayersNamesTextures = playersNamesTextures;
            PlayersPicsTextures = playersPicsTextures;

            ThemesLabelsBottomSpace = themesLabelsBottomSpace;
            ThemesListRightSpace = themesListRightSpace;

            PlayersPhotoSpace = playersPhotoSpace;
            PlayersSpace = playersSpace;
            PicsSize = picsSize;

            Font = font;
            FontNormalColor = fontNormalColor;
            FontHoveredColor = fontHoveredColor;
            FontSize = fontSize;
        }

        public static ThemeSelectionMenuStyle Load(XmlReader reader)
        {
            Color backgroundColor = new Color(0, 255, 0);
            ImagePart backgroundImage = null;
            TextureDisplayMode backgroundDisplayMode = TextureDisplayMode.Stretch;

            ImagePart[] labelsTextures = null;
            ImagePart[] hoveredLabelsTextures = null;


            ImagePart[] playersNamesTextures = null;
            ImagePart[] playersPicsTextures = null;
            float themesLabelsBottomSpace = 10f;
            float themesListRightSpace = 20f;
            float playersPhotoSpace = 5f;
            float playersSpace = 5f;
            Vector2f picsSize = new Vector2f(256f, 256f);
            Font font = null;
            Color fontNormalColor = new Color(255, 255, 255);
            Color fontHoveredColor = new Color(0, 0, 0);
            uint fontSize = 14;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            if (reader.Name == "backgroundColor")
                                backgroundColor = GuiStyle.ParseColor(reader);
                            else if (reader.Name == "backgroundImage")
                            {
                                String r = reader.GetAttribute("src");
                                if (r != null)
                                    backgroundImage = new ImagePart( new Texture(r) );

                                r = reader.GetAttribute("mode");
                                if (r != null)
                                {
                                    if (r == "centered")
                                        backgroundDisplayMode = TextureDisplayMode.Center;
                                    else if (r == "stretched")
                                        backgroundDisplayMode = TextureDisplayMode.Stretch;
                                }
                            }
                            else if (reader.Name == "labelsTextures")
                                labelsTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "hoveredLabelsTextures")
                                hoveredLabelsTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "playersNamesTextures")
                                playersNamesTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "playersPicsTextures")
                                playersPicsTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "themesLabelsBottomSpace")
                            {
                                String r = reader.GetAttribute("val");
                                if (r != null)
                                    themesLabelsBottomSpace = float.Parse(r);
                            }
                            else if (reader.Name == "themesListRightSpace")
                            {
                                String r = reader.GetAttribute("val");
                                if (r != null)
                                    themesListRightSpace = float.Parse(r);
                            }
                            else if (reader.Name == "playersPhotoSpace")
                            {
                                String r = reader.GetAttribute("val");
                                if (r != null)
                                    playersPhotoSpace = float.Parse(r);
                            }
                            else if (reader.Name == "playersSpace")
                            {
                                String r = reader.GetAttribute("val");
                                if (r != null)
                                    playersSpace = float.Parse(r);
                            }
                            else if (reader.Name == "picsSize")
                            {
                                String r = reader.GetAttribute("width");
                                if (r != null)
                                    picsSize.X = float.Parse(r);

                                r = reader.GetAttribute("height");
                                if (r != null)
                                    picsSize.Y = float.Parse(r);
                            }
                            else if (reader.Name == "font")
                            {
                                String r = reader.GetAttribute("src");
                                if (r != null)
                                    font = new Font(r);

                                r = reader.GetAttribute("size");
                                if (r != null)
                                    fontSize = uint.Parse(r);
                            }
                            else if (reader.Name == "fontNormalColor")
                                fontNormalColor = GuiStyle.ParseColor(reader);
                            else if (reader.Name == "fontHoveredColor")
                                fontHoveredColor = GuiStyle.ParseColor(reader);
                            break;
                        }
                }
            }

            return new ThemeSelectionMenuStyle(
                backgroundColor,
                backgroundImage,
                backgroundDisplayMode,
                labelsTextures, 
                hoveredLabelsTextures,
                playersNamesTextures,
                playersPicsTextures,
                themesLabelsBottomSpace,
                themesListRightSpace,
                playersPhotoSpace,
                playersSpace,
                picsSize,
                font,
                fontNormalColor,
                fontHoveredColor,
                fontSize
            );
        }
    }

    class GuiStyle
    {
        public ThemeSelectionMenuStyle ThemeSelectionMenuStyle;

        public GuiStyle(ThemeSelectionMenuStyle themeSelectionMenuStyle )
        {
            ThemeSelectionMenuStyle = themeSelectionMenuStyle;
        }

        static public Color ParseColor(XmlReader reader)
        {
            Color ret = new Color(0, 0, 0);
            String r = reader.GetAttribute("R");
            if (r != null)
                ret.R = byte.Parse(r);

            String g = reader.GetAttribute("G");
            if (g != null)
                ret.G = byte.Parse(g);

            String b = reader.GetAttribute("B");
            if (b != null)
                ret.B = byte.Parse(b);

            return ret;
        }

        public static GuiStyle LoadFromFile(String filename)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;

            StreamReader stream;
            try
            {
                stream = System.IO.File.OpenText(filename);
            }
            catch (System.IO.FileNotFoundException)
            {
                throw new Exception("Can't load the specified file");
            }

            XmlReader reader = XmlReader.Create(stream, settings);

            return Load(reader);
        }

        public static GuiStyle Load(XmlReader reader)
        {
            ThemeSelectionMenuStyle themeSelectionMenuStyle = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "themeSelectionMenu")
                            themeSelectionMenuStyle = ThemeSelectionMenuStyle.Load(reader.ReadSubtree());
                        break;
                }
            }

            return new GuiStyle(themeSelectionMenuStyle);
        }

    }
}
