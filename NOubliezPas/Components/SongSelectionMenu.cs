using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using kT.GUI;
using SFML.Window;
using SFML.Graphics;

using NOubliezPas.Communication;

namespace NOubliezPas
{
    class SongSelectionMenu : Component
    {
        ThemeSelectionMenuStyle myStyle;
        GameApplication myApp = null;
        Player myPlayer = null;
        Theme myTheme;

        UIManager myUIManager;

        int currentChoice = 0;
        List<Frame> songNameFrames = new List<Frame>();
        List<Label> songNameLabels = new List<Label>();

        Label myPlayerScoreLabel;

        public SongSelectionMenu(GameApplication app, Player player, Theme theme)
        {
            myApp = app;
            myPlayer = player;
            myTheme = theme;
        }

        public void Activate()
        {
            myApp.SendMessage(GameToControllerWindowMessage.SongSelectionMenuEnter);
        }

        public void Desactivate()
        {
            myApp.SendMessage(GameToControllerWindowMessage.SongSelectionMenuExit);
        }

        public void ReadMessage(ControllerToGameMessage msg)
        {

        }

        public void OnKeyPressed(object sender, EventArgs e)
        {
            KeyEventArgs args = (KeyEventArgs)e;

            if (songNameLabels.Count > 0)
            {
                // validation du choix de la chanson
                if (args.Code == Keyboard.Key.Return)
                    myApp.ChangeComponent(new SongTest(myApp, myPlayer, myTheme.GetSong(currentChoice)));

                // mouvement dans le menu
                int oldChoice = currentChoice;

                if (args.Code == Keyboard.Key.Down)
                    currentChoice++;
                else if (args.Code == Keyboard.Key.Up)
                    currentChoice--;

                if (currentChoice >= songNameLabels.Count)
                    currentChoice = songNameLabels.Count - 1;
                if (currentChoice < 0)
                    currentChoice = 0;

                ChangeChoice();
            }
        }

        void ChangeChoice()
        {
            for (int i = 0; i < songNameLabels.Count; i++ )
            {
                songNameFrames[i].BordersImagesParts = myStyle.LabelsTextures;

                songNameLabels[i].TextColor = myStyle.FontNormalColor;
                songNameLabels[i].Text = myTheme.GetSong(i).Name;
            }

            songNameFrames[currentChoice].BordersImagesParts = myStyle.HoveredLabelsTextures;

            songNameLabels[currentChoice].Text = myTheme.GetSong(currentChoice).Name;
            songNameLabels[currentChoice].TextColor = myStyle.FontHoveredColor;
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            // récupération du style
            myStyle = myApp.guiStyle.ThemeSelectionMenuStyle;

            ImagePart[] labelTextures = myStyle.LabelsTextures;

            myUIManager = new UIManager(myApp.window);

            DCFont myFont = new DCFont(new Font("Content/verdana.ttf"));

            Frame scoreFrame = new Frame(myUIManager, null );
            scoreFrame.BordersImagesParts = labelTextures;
            scoreFrame.Visible = true;

            myPlayerScoreLabel = new Label(myUIManager, null, myFont, myPlayer.Name + "   " + myPlayer.Score );
            myPlayerScoreLabel.Tint = myStyle.FontNormalColor;
            myPlayerScoreLabel.Visible = true;
            scoreFrame.Position = new Vector2f(50f, 20f);
            scoreFrame.ContainedWidget = myPlayerScoreLabel;

            VerticalLayout vvLayout = new VerticalLayout(myUIManager, null);
            vvLayout.Visible = true;

            for (int i = 0; i < myTheme.NumSongs; i++)
            {
                VerticalSpacer sp = new VerticalSpacer(myUIManager, vvLayout, 40f);
                sp.Visible = true;
                vvLayout.Add(sp);

                Frame songNameFrame = new Frame(myUIManager, vvLayout);
                songNameFrame.BordersImagesParts = labelTextures;
                songNameFrame.Visible = true;
                songNameFrames.Add( songNameFrame );

                Label songNamelabel = new Label(myUIManager, null, myFont, myTheme.GetSong(i).Name);
                songNamelabel.Tint = myStyle.FontNormalColor;
                songNamelabel.Visible = true;
                songNameLabels.Add(songNamelabel);

                songNameFrame.ContainedWidget = songNamelabel;
                vvLayout.Add(songNameFrame);
            }

            vvLayout.CenterPosition = new Vector2f(myApp.window.Size.X, myApp.window.Size.Y) / 2f;

            ChangeChoice();
        }

        public void Update(Stopwatch time)
        {
            if (myPlayer.Score > 0)
                myPlayerScoreLabel.Text = myPlayer.Name + " " + myPlayer.Score;
            else
                myPlayerScoreLabel.Text = myPlayer.Name;

            myUIManager.Update(time);
        }

        public void Draw(Stopwatch time)
        {
            myApp.window.Clear(myStyle.BackgroundColor);

            if (myStyle.BackgroundImage != null)
            {
                ImagePart part = myStyle.BackgroundImage;
                myUIManager.Painter.Begin();
                if (myStyle.BackgroundDisplayMode == TextureDisplayMode.Stretch)
                    myUIManager.Painter.DrawImage(part.SourceTexture, new FloatRect(0f, 0f, myUIManager.ScreenSize.X, myUIManager.ScreenSize.Y), part.SourceRectangle);
                else
                {
                    // the image must be centered.
                    FloatRect destRect = new FloatRect(
                        0.5f * (myUIManager.ScreenSize.X - myStyle.BackgroundImage.Size.X),
                        0.5f * (myUIManager.ScreenSize.Y - myStyle.BackgroundImage.Size.Y),
                        myStyle.BackgroundImage.Size.X,
                        myStyle.BackgroundImage.Size.Y);
                    myUIManager.Painter.DrawImage(part.SourceTexture, destRect, part.SourceRectangle);
                }

                myUIManager.Painter.End();
            }

            myUIManager.Render();
        }
    }
}
