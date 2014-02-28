using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using System.IO;
using System.Xml;

namespace NOubliezPas
{
    class FrameTexturesLoader
    {
        public static Texture[] Load(XmlReader reader)
        {
            Texture[] textures = new Texture[9];

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            if (reader.Name == "bordhautgauche")
                            {
                                String r = reader.GetAttribute("src");
                                if (r != null)
                                    textures[0] = new Texture(r);
                            }
                            else if (reader.Name == "milieuhaut")
                            {
                                String r = reader.GetAttribute("src");
                                if (r != null)
                                    textures[1] = new Texture(r);
                            }
                            else if (reader.Name == "bordhautdroit")
                            {
                                String r = reader.GetAttribute("src");
                                if (r != null)
                                    textures[2] = new Texture(r);
                            }
                            else if (reader.Name == "milieugauche")
                            {
                                String r = reader.GetAttribute("src");
                                if (r != null)
                                    textures[3] = new Texture(r);
                            }
                            else if (reader.Name == "milieu")
                            {
                                String r = reader.GetAttribute("src");
                                if (r != null)
                                    textures[4] = new Texture(r);
                            }
                            else if (reader.Name == "milieudroit")
                            {
                                String r = reader.GetAttribute("src");
                                if (r != null)
                                    textures[5] = new Texture(r);
                            }
                            else if (reader.Name == "bordbasgauche")
                            {
                                String r = reader.GetAttribute("src");
                                if (r != null)
                                    textures[6] = new Texture(r);
                            }
                            else if (reader.Name == "milieubas")
                            {
                                String r = reader.GetAttribute("src");
                                if (r != null)
                                    textures[7] = new Texture(r);
                            }
                            else if (reader.Name == "bordbasdroit")
                            {
                                String r = reader.GetAttribute("src");
                                if (r != null)
                                    textures[8] = new Texture(r);
                            }
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
        public Texture[] LabelsTextures;
        public Texture[] ScoresTextures;
        public Texture[] PlayersNamesTextures;
        public Texture[] PlayersPicsTextures;
        public float ThemesLabelsBottomSpace;
        public float ThemesListRightSpace;
        public float PlayersPhotoSpace;
        public float PlayersSpace;
        public Vector2f PicsSize;
        public Font Font;
        public uint FontSize;


        public ThemeSelectionMenuStyle(
            Color backgroundColor,
            Texture[] labelsTextures,
            Texture[] scoresTextures,
            Texture[] playersNamesTextures,
            Texture[] playersPicsTextures,
            float themesLabelsBottomSpace,
            float themesListRightSpace,
            float playersPhotoSpace,
            float playersSpace,
            Vector2f picsSize,
            Font font,
            uint fontSize
            )
        {
            BackgroundColor = backgroundColor;
            LabelsTextures = labelsTextures;
            ScoresTextures = scoresTextures;
            PlayersNamesTextures = playersNamesTextures;
            PlayersPicsTextures = playersPicsTextures;
            ThemesLabelsBottomSpace = themesLabelsBottomSpace;
            ThemesListRightSpace = themesListRightSpace;
            PlayersPhotoSpace = playersPhotoSpace;
            PlayersSpace = playersSpace;
            PicsSize = picsSize;
            Font = font;
            FontSize = fontSize;
        }

        public static ThemeSelectionMenuStyle Load(XmlReader reader)
        {
            Color backgroundColor = new Color(0, 255, 0);
            Texture[] labelsTextures = null;
            Texture[] scoresTextures = null;
            Texture[] playersNamesTextures = null;
            Texture[] playersPicsTextures = null;
            float themesLabelsBottomSpace = 10f;
            float themesListRightSpace = 20f;
            float playersPhotoSpace = 5f;
            float playersSpace = 5f;
            Vector2f picsSize = new Vector2f(256f, 256f);
            Font font = null;
            uint fontSize = 14;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            if (reader.Name == "themeSelectionMenu")
                            {
                                String r = reader.GetAttribute("backgroundColorR");
                                if (r != null)
                                    backgroundColor.R = byte.Parse(r);

                                String g = reader.GetAttribute("backgroundColorG");
                                if (g != null)
                                    backgroundColor.G = byte.Parse(g);

                                String b = reader.GetAttribute("backgroundColorB");
                                if (b != null)
                                    backgroundColor.B = byte.Parse(b);
                            }
                            else if (reader.Name == "labelsTextures")
                                labelsTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
                            else if (reader.Name == "scoresTextures")
                                scoresTextures = FrameTexturesLoader.Load(reader.ReadSubtree());
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
                            break;
                        }
                }
            }

            return new ThemeSelectionMenuStyle(
                backgroundColor,
                labelsTextures, 
                scoresTextures,
                playersNamesTextures,
                playersPicsTextures,
                themesLabelsBottomSpace,
                themesListRightSpace,
                playersPhotoSpace,
                playersSpace,
                picsSize,
                font,
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
