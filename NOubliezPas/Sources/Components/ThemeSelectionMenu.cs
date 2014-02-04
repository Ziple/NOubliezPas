using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using kT.GUI;
using SFML.Graphics;
using SFML.Window;

namespace NOubliezPas
{
    class ThemeSelectionMenu: Component
    {
        GameApplication myApp = null;
        GameState myGame;
        Player myPlayer;

        UIManager myUIManager;

        int currentChoice = 0;
        List<Label> themesLabels = new List<Label>();
        bool displayScores = false;

        public ThemeSelectionMenu( GameApplication app )
        {
            myApp = app;
            myGame = app.game;
        }

        public void OnKeyPressed(object sender, EventArgs e)
        {
            KeyEventArgs args = (KeyEventArgs)e;

            if (themesLabels.Count > 0)
            {
                // validation du choix du theme
                if (args.Code == Keyboard.Key.Return)
                {
                    Theme chosenTheme = myGame.GetTheme(currentChoice);
                    myApp.mustChangeComponent = true;
                    myApp.newComponent = new SongSelectionMenu(myApp, myPlayer, chosenTheme);
                    myPlayer.ChosenThemes.Add(chosenTheme);
                    myApp.newComponent.Initialize();
                    myApp.newComponent.LoadContent();
                }

                // déplacement dans le menu
                int oldChoice = currentChoice;
                if (args.Code == Keyboard.Key.Down)
                    currentChoice = myGame.GetNextAcceptableChoice( currentChoice );
                else if (args.Code == Keyboard.Key.Up)
                    currentChoice = myGame.GetPreviousAcceptableChoice( currentChoice );

                if (currentChoice < 0)
                    throw new Exception("Plus de thème restant!");

                if (currentChoice != oldChoice)
                {
                    themesLabels[oldChoice].TextColor = Color.White;
                    themesLabels[oldChoice].Text = myGame.GetTheme(oldChoice).Name;

                    themesLabels[currentChoice].Text = "<b>" + myGame.GetTheme(currentChoice).Name + "</b>";
                    themesLabels[currentChoice].TextColor = Color.Black;
                }
            }
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            myPlayer = null;
            themesLabels = new List<Label>();

            if (myGame.NumPlayers > 0)
            {
                // on trouve le joueur concerné par la sélection
                int numSelections = myGame.Players[0].ChosenThemes.Count;

                for (int i = 1; i < myGame.NumPlayers; i++)
                {
                    if (myGame.Players[i].ChosenThemes.Count < numSelections)
                    {
                        myPlayer = myGame.Players[i];
                        break;
                    }
                }

                if (myPlayer == null)
                    myPlayer = myGame.Players[0];

                // on détermine si l'on doit afficher les scores.
                for (int i = 0; i < myGame.NumPlayers; i++)
                {
                    if (myGame.Players[i].Score > 0)
                    {
                        displayScores = true;
                        break;
                    }
                }
            }
            else
                throw new Exception("Pas de joueur?");

            Texture[] labelTextures = myApp.guiStyle.labelTextures;
            Texture[] scoreTextures = myApp.guiStyle.scoreTextures;

            myUIManager = new UIManager(myApp.window);

            DCFont myFont = new DCFont(new Font("Content/verdana.ttf"));

            // Affichage des noms des joueurs (éventuellement leurs scores)
            HorizontalLayout hLayout = new HorizontalLayout(myUIManager, null);
            hLayout.Visible = true;

            for (int i = 0; i < myGame.NumPlayers; i++)
            {
                Player player = myGame.Players[i];

                Frame scoreFrame = new Frame(myUIManager, hLayout);
                scoreFrame.BordersImages = scoreTextures;
                scoreFrame.Visible = true;

                string str = player.Name;
                if (displayScores)
                    str += " - " + player.Score.ToString();

                Label scoreLabel = new Label(myUIManager, scoreFrame, myFont, str);

                if (player == myPlayer)
                    scoreLabel.Tint = Color.Black;
                else
                    scoreLabel.Tint = Color.White;

                scoreLabel.Visible = true;

                scoreFrame.ContainedWidget = scoreLabel;
                hLayout.Add(scoreFrame);

                VerticalSpacer sp = new VerticalSpacer(myUIManager, hLayout, 20f);
                sp.Visible = true;
                hLayout.Add(sp);
            }

            // Affichage des thèmes
            VerticalLayout vLayout = new VerticalLayout(myUIManager, null);
            vLayout.Visible = true;

            for (int i = 0; i < myGame.NumThemes; i++)
            {
                Frame themesFrame = new Frame(myUIManager, vLayout);
                themesFrame.BordersImages = labelTextures;
                themesFrame.Visible = true;

                if( !myGame.IsThemeAvalaible( myGame.GetTheme(i) ) )
                    themesFrame.Tint = new Color( 125, 125, 125 );

                Label themesNamelabel = new Label(myUIManager, themesFrame, myFont, myGame.GetTheme(i).Name );
                themesNamelabel.Tint = Color.White;
                themesNamelabel.Visible = true;
                themesLabels.Add(themesNamelabel);

                themesFrame.ContainedWidget = themesNamelabel;
                vLayout.Add(themesFrame);

                VerticalSpacer sp = new VerticalSpacer(myUIManager, vLayout, 40f);
                sp.Visible = true;
                vLayout.Add(sp);
            }

            vLayout.CenterPosition = new Vector2f(myApp.window.Size.X / 2f, myApp.window.Size.Y / 2f + hLayout.Size.Y);

            currentChoice = myGame.GetNextAcceptableChoice(-1);
            themesLabels[currentChoice].TextColor = Color.Black;
            themesLabels[currentChoice].Text = "<b>" + myGame.GetTheme(0).Name + "</b>";
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
