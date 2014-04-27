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
        public ImagePart[] FirstLabelsTextures;
        public ImagePart[] FirstHoveredLabelsTextures;
        public ImagePart[] FirstScoresTextures;
        public ImagePart[] FirstHoveredScoresTextures;
        public ImagePart[] LabelsTextures;
        public ImagePart[] HoveredLabelsTextures;
        public ImagePart[] ScoresTextures;
        public ImagePart[] HoveredScoresTextures;
        public ImagePart[] LastLabelsTextures;
        public ImagePart[] LastHoveredLabelsTextures;
        public ImagePart[] LastScoresTextures;
        public ImagePart[] LastHoveredScoresTextures;
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
            ImagePart[] firstLabelsTextures,
            ImagePart[] firstHoveredLabelsTextures,
            ImagePart[] firstScoresTextures,
            ImagePart[] firstHoveredScoresTextures,
            ImagePart[] labelsTextures,
            ImagePart[] hoveredLabelsTextures,
            ImagePart[] scoresTextures,
            ImagePart[] hoveredScoresTextures,
            ImagePart[] lastLabelsTextures,
            ImagePart[] lastHoveredLabelsTextures,
            ImagePart[] lastScoresTextures,
            ImagePart[] lastHoveredScoresTextures,
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

            FirstLabelsTextures = firstLabelsTextures;
            FirstHoveredLabelsTextures = firstHoveredLabelsTextures;

            FirstScoresTextures = firstScoresTextures;
            FirstHoveredScoresTextures = firstHoveredScoresTextures;

            LabelsTextures = labelsTextures;
            HoveredLabelsTextures = hoveredLabelsTextures;

            ScoresTextures = scoresTextures;
            HoveredScoresTextures = hoveredScoresTextures;

            LastLabelsTextures = lastLabelsTextures;
            LastHoveredLabelsTextures = lastHoveredLabelsTextures;

            LastScoresTextures = lastScoresTextures;
            LastHoveredScoresTextures = lastHoveredScoresTextures;

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

            ImagePart[] firstLabelsTextures = null;
            ImagePart[] firstHoveredLabelsTextures = null;
            ImagePart[] firstScoresTextures = null;
            ImagePart[] firstHoveredScoresTextures = null;
            ImagePart[] labelsTextures = null;
            ImagePart[] hoveredLabelsTextures = null;
            ImagePart[] scoresTextures = null;
            ImagePart[] hoveredScoresTextures = null;
            ImagePart[] lastLabelsTextures = null;
            ImagePart[] lastHoveredLabelsTextures = null;
            ImagePart[] lastScoresTextures = null;
            ImagePart[] lastHoveredScoresTextures = null;


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
                            else if (reader.Name == "firstLabelsTextures")
                                firstLabelsTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "firstHoveredLabelsTextures")
                                firstHoveredLabelsTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "firstScoresTextures")
                                firstScoresTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "firstHoveredScoresTextures")
                                firstHoveredScoresTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "labelsTextures")
                                labelsTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "hoveredLabelsTextures")
                                hoveredLabelsTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "scoresTextures")
                                scoresTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "hoveredScoresTextures")
                                hoveredScoresTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "lastLabelsTextures")
                                lastLabelsTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "lastHoveredLabelsTextures")
                                lastHoveredLabelsTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "lastScoresTextures")
                                lastScoresTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "lastHoveredScoresTextures")
                                lastHoveredScoresTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
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
                firstLabelsTextures,
                firstHoveredLabelsTextures,
                firstScoresTextures,
                firstHoveredScoresTextures,
                labelsTextures, 
                hoveredLabelsTextures,
                scoresTextures,
                hoveredScoresTextures,
                lastLabelsTextures,
                lastHoveredLabelsTextures,
                lastScoresTextures,
                lastHoveredScoresTextures,
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
        public Texture[] labelTextures;
        public Texture[] scoreTextures;

        public ThemeSelectionMenuStyle ThemeSelectionMenuStyle;

        public GuiStyle(ThemeSelectionMenuStyle themeSelectionMenuStyle )
        {
            ThemeSelectionMenuStyle = themeSelectionMenuStyle;

            labelTextures = new Texture[9];
            labelTextures[0] = new Texture("Content/Label/bordhautgauche.png");
            labelTextures[1] = new Texture("Content/Label/milieuhaut.png");
            labelTextures[2] = new Texture("Content/Label/bordhautdroit.png");
            labelTextures[3] = new Texture("Content/Label/milieugauche.png");
            labelTextures[4] = new Texture("Content/Label/milieu.png");
            labelTextures[5] = new Texture("Content/Label/milieudroit.png");
            labelTextures[6] = new Texture("Content/Label/bordbasgauche.png");
            labelTextures[7] = new Texture("Content/Label/milieubas.png");
            labelTextures[8] = new Texture("Content/Label/bordbasdroit.png");

            scoreTextures = new Texture[9];
            scoreTextures[0] = new Texture("Content/Score/bordhautgauche.png");
            scoreTextures[1] = new Texture("Content/Score/milieuhaut.png");
            scoreTextures[2] = new Texture("Content/Score/bordhautdroit.png");
            scoreTextures[3] = new Texture("Content/Score/milieugauche.png");
            scoreTextures[4] = new Texture("Content/Score/milieu.png");
            scoreTextures[5] = new Texture("Content/Score/milieudroit.png");
            scoreTextures[6] = new Texture("Content/Score/bordbasgauche.png");
            scoreTextures[7] = new Texture("Content/Score/milieubas.png");
            scoreTextures[8] = new Texture("Content/Score/bordbasdroit.png");
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
