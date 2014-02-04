using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using kT.GUI;
using SFML.Window;
using SFML.Graphics;

namespace NOubliezPas
{
    class SongSelectionMenu : Component
    {
        GameApplication myApp = null;
        Player myPlayer = null;
        Theme myTheme;

        UIManager myUIManager;

        int currentChoice = 0;
        List<Label> songsLabels;

        public SongSelectionMenu(GameApplication app, Player player, Theme theme)
        {
            myApp = app;
            myPlayer = player;
            myTheme = theme;
        }

        public void OnKeyPressed(object sender, EventArgs e)
        {
            KeyEventArgs args = (KeyEventArgs)e;

            if (songsLabels.Count > 0)
            {
                // validation du choix de la chanson
                if (args.Code == Keyboard.Key.Return)
                {
                    myApp.mustChangeComponent = true;
                    myApp.newComponent = new SongTest(myApp, myPlayer, myTheme.GetSong(currentChoice));
                    myApp.newComponent.Initialize();
                    myApp.newComponent.LoadContent();
                }

                // mouvement dans le menu
                int oldChoice = currentChoice;

                if (args.Code == Keyboard.Key.Down)
                    currentChoice++;
                else if (args.Code == Keyboard.Key.Up)
                    currentChoice--;

                if (currentChoice >= songsLabels.Count)
                    currentChoice = songsLabels.Count - 1;
                if (currentChoice < 0)
                    currentChoice = 0;

                if (currentChoice != oldChoice)
                {
                    songsLabels[oldChoice].TextColor = Color.White;
                    songsLabels[oldChoice].Text = myTheme.GetSong(oldChoice).Name;

                    songsLabels[currentChoice].Text = "<b>" + myTheme.GetSong(currentChoice).Name + "</b>";
                    songsLabels[currentChoice].TextColor = Color.Black;
                }
            }
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            Texture[] labelTextures = myApp.guiStyle.labelTextures;
            Texture[] scoreTextures = myApp.guiStyle.scoreTextures;

            songsLabels = new List<Label>();

            myUIManager = new UIManager(myApp.window);

            DCFont myFont = new DCFont(new Font("Content/verdana.ttf"));

            Frame scoreFrame = new Frame(myUIManager, null );
            scoreFrame.BordersImages = scoreTextures;
            scoreFrame.Visible = true;

            Label scoreLabel = new Label(myUIManager, null, myFont, myPlayer.Name + " - " + myPlayer.Score );
            scoreLabel.Tint = Color.White;
            scoreLabel.Visible = true;
            scoreFrame.Position = new Vector2f(10f, 10f);
            scoreFrame.ContainedWidget = scoreLabel;

            VerticalLayout vvLayout = new VerticalLayout(myUIManager, null);
            vvLayout.Visible = true;

            for (int i = 0; i < myTheme.NumSongs; i++)
            {
                VerticalSpacer sp = new VerticalSpacer(myUIManager, vvLayout, 40f);
                sp.Visible = true;
                vvLayout.Add(sp);

                Frame songNameFrame = new Frame(myUIManager, vvLayout);
                songNameFrame.BordersImages = labelTextures;
                songNameFrame.Visible = true;

                Label songNamelabel = new Label(myUIManager, null, myFont, myTheme.GetSong(i).Name);
                songNamelabel.Tint = Color.White;
                songNamelabel.Visible = true;
                songsLabels.Add(songNamelabel);

                songNameFrame.ContainedWidget = songNamelabel;
                vvLayout.Add(songNameFrame);
            }

            vvLayout.CenterPosition = new Vector2f(myApp.window.Size.X, myApp.window.Size.Y) / 2f;

            songsLabels[0].TextColor = Color.Black;
            songsLabels[0].Text = "<b>" + myTheme.GetSong(0).Name + "</b>";
        }

        public void Update(Stopwatch time)
        {
            myUIManager.Update(time);
        }

        public void Draw(Stopwatch time)
        {
            myUIManager.Render();
        }
    }
}
