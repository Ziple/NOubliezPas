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
        ThemeSelectionMenuStyle myStyle;
        GameApplication myApp = null;
        GameState myGame;
        Player myPlayer;

        UIManager myUIManager;
        HorizontalLayout mainLayout;
        VerticalLayout themesLayout;
        HorizontalLayout playersLayout;

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
                    ChangeChoice();
            }
        }

        void ChangeChoice()
        {
            foreach( Label themeLabel in themesLabels )
                themeLabel.TextColor = Color.White;

            themesLabels[currentChoice].TextColor = Color.Black;
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            // récupération du style
            myStyle = myApp.guiStyle.ThemeSelectionMenuStyle;

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

            Texture[] labelTextures = myStyle.LabelsTextures;
            Texture[] scoreTextures = myStyle.ScoresTextures;
            Texture[] playersNamesTextures = myStyle.PlayersNamesTextures;
            Texture[] playersPicsTextures = myStyle.PlayersPicsTextures;
            float themesLabelsBottomSpace = myStyle.ThemesLabelsBottomSpace;
            float themesListRightSpace = myStyle.ThemesListRightSpace;
            float playersSpace = myStyle.PlayersSpace;
            Vector2f picsSize = myStyle.PicsSize;
            Font font = myStyle.Font;
            uint fontSize = myStyle.FontSize;

            myUIManager = new UIManager(myApp.window);

            DCFont myFont = new DCFont(font);

            // Layout principal contenant tout le reste
            mainLayout = new HorizontalLayout(myUIManager, null);
            mainLayout.Visible = true;

            // Affichage des thèmes
            {
                themesLayout = new VerticalLayout(myUIManager, mainLayout);
                themesLayout.Visible = true;
                mainLayout.Add(themesLayout);

                for (int i = 0; i < myGame.NumThemes; i++)
                {
                    HorizontalLayout themeHLayout = new HorizontalLayout(myUIManager, themesLayout);
                    themeHLayout.Visible = true;
                    themesLayout.Add(themeHLayout);

                    if (!myGame.IsThemeAvalaible(myGame.GetTheme(i)))
                        themeHLayout.Tint = new Color(125, 125, 125);

                    // Frame contenant le nom du thème
                    {
                        Frame themeFrame = new Frame(myUIManager, themeHLayout);
                        themeFrame.BordersImages = labelTextures;
                        themeFrame.Visible = true;
                        themeHLayout.Add(themeFrame);

                        Label themesNamelabel = new Label(myUIManager, themeFrame, myFont, myGame.GetTheme(i).Name, fontSize);
                        themesNamelabel.Tint = Color.White;
                        themesNamelabel.Visible = true;
                        themesLabels.Add(themesNamelabel);
                        themeFrame.ContainedWidget = themesNamelabel;
                    }

                    // Frame contenant le nombre de points du thème
                    {
                        Frame ptsFrame = new Frame(myUIManager, themeHLayout);
                        ptsFrame.BordersImages = scoreTextures;
                        ptsFrame.Visible = true;
                        themeHLayout.Add(ptsFrame);

                        Label ptslabel = new Label(myUIManager, ptsFrame, myFont, myGame.GetTheme(i).Points.ToString(), fontSize );
                        ptslabel.Tint = Color.White;
                        ptslabel.Visible = true;
                        ptsFrame.ContainedWidget = ptslabel;
                    }

                    if (i < myGame.NumThemes - 1)
                    {
                        VerticalSpacer sp = new VerticalSpacer(myUIManager, themesLayout, themesLabelsBottomSpace);
                        sp.Visible = true;
                        themesLayout.Add(sp);
                    }
                }

                // On met toutes les frames à la même taille
                Vector2f fLMax = new Vector2f(0f, 0f);
                Vector2f fSMax = new Vector2f(0f, 0f);

                foreach (Widget entry in themesLayout.Widgets)
                {
                    if(entry.GetType() == typeof(HorizontalLayout))
                    {
                        HorizontalLayout vLayout = (HorizontalLayout)entry;

                        Frame fL = (Frame)vLayout.Widgets[0];
                        Frame fS = (Frame)vLayout.Widgets[1];

                        fLMax.X = fLMax.X < fL.Size.X ? fL.Size.X : fLMax.X;
                        fLMax.Y = fLMax.Y < fL.Size.Y ? fL.Size.Y : fLMax.Y;

                        fSMax.X = fSMax.X < fS.Size.X ? fS.Size.X : fSMax.X;
                        fLMax.Y = fLMax.Y < fS.Size.Y ? fS.Size.Y : fLMax.Y;
                    }
                }

                foreach (Widget entry in themesLayout.Widgets)
                {
                    if (entry.GetType() == typeof(HorizontalLayout))
                    {
                        HorizontalLayout vLayout = (HorizontalLayout)entry;

                        Frame fL = (Frame)vLayout.Widgets[0];
                        Frame fS = (Frame)vLayout.Widgets[1];

                        fL.Resize(fLMax);
                        fS.Resize(new Vector2f(fSMax.X, fLMax.Y));
                    }
                }
            }

            // Ajout d'un spacer
            HorizontalSpacer msp = new HorizontalSpacer(myUIManager, mainLayout, themesListRightSpace);
            msp.Visible = true;
            mainLayout.Add(msp);

            // Affichage des noms des joueurs (éventuellement leurs scores)
            {
                playersLayout = new HorizontalLayout(myUIManager, mainLayout);
                playersLayout.Visible = true;
                mainLayout.Add(playersLayout);

                for (int i = 0; i < myGame.NumPlayers; i++)
                {
                    Player player = myGame.Players[i];

                    VerticalLayout playerVLayout = new VerticalLayout(myUIManager, playersLayout);
                    playerVLayout.Alignment = HorizontalAlignment.Center;

                    playerVLayout.Visible = true;
                    playersLayout.Add(playerVLayout);

                    // Chargement de la photo du joueur
                    {
                        Frame photoFrame = new Frame(myUIManager, playerVLayout); ;
                        photoFrame.BordersImages = playersPicsTextures;
                        photoFrame.Visible = true;
                        playerVLayout.Add(photoFrame);

                        Caption playerCaption = new Caption(myUIManager, photoFrame);
                        playerCaption.Visible = true;

                        Texture tex = new Texture(player.PhotoSrc);
                        ImagePart img = new ImagePart(tex);
                        playerCaption.ImagePart = img;
                        playerCaption.Size = picsSize;

                        // ajout de la photo
                        photoFrame.ContainedWidget = playerCaption;
                    }

                    // Ajout d'un spacer
                    VerticalSpacer vsp = new VerticalSpacer(myUIManager, playerVLayout, 5f);
                    vsp.Visible = true;
                    playerVLayout.Add(vsp);

                    // Affichage du score du joueur
                    {
                        Frame playerNameFrame = new Frame(myUIManager, playerVLayout);
                        playerNameFrame.BordersImages = playersNamesTextures;
                        playerNameFrame.Visible = true;
                        playerVLayout.Add(playerNameFrame);

                        string str = player.Name;
                        if (displayScores)
                            str += " - " + player.Score.ToString();

                        Label scoreLabel = new Label(myUIManager, playerNameFrame, myFont, str, fontSize);

                        if (player == myPlayer)
                            scoreLabel.Tint = Color.Black;
                        else
                            scoreLabel.Tint = Color.White;

                        scoreLabel.Visible = true;

                        playerNameFrame.ContainedWidget = scoreLabel;
                    }

                    // on ajoute le spacer horizontal que si on ajoute pas le dernier joueur
                    if( i < myGame.NumPlayers-1 )
                    {
                        HorizontalSpacer sp = new HorizontalSpacer(myUIManager, playersLayout, playersSpace);
                        sp.Visible = true;
                        playersLayout.Add(sp);
                    }
                }

                // On met toutes les frames à la même taille
                Vector2f fLMax = new Vector2f(0f, 0f);
                Vector2f fSMax = new Vector2f(0f, 0f);

                foreach (Widget entry in playersLayout.Widgets)
                {
                    if (entry.GetType() == typeof(VerticalLayout))
                    {
                        VerticalLayout vLayout = (VerticalLayout)entry;

                        Frame fL = (Frame)vLayout.Widgets[0];//photo
                        Frame fS = (Frame)vLayout.Widgets[2];//nom

                        fLMax.X = fLMax.X < fL.Size.X ? fL.Size.X : fLMax.X;
                        fLMax.Y = fLMax.Y < fL.Size.Y ? fL.Size.Y : fLMax.Y;

                        fSMax.X = fSMax.X < fS.Size.X ? fS.Size.X : fSMax.X;
                        fSMax.Y = fSMax.Y < fS.Size.Y ? fS.Size.Y : fSMax.Y;
                    }
                }

                foreach (Widget entry in playersLayout.Widgets)
                {
                    if (entry.GetType() == typeof(VerticalLayout))
                    {
                        VerticalLayout vLayout = (VerticalLayout)entry;

                        Frame fL = (Frame)vLayout.Widgets[0];
                        Frame fS = (Frame)vLayout.Widgets[2];

                        fL.Resize(fLMax);
                        fS.Resize(fSMax);
                    }
                }

                // repositionne
                Vector2f pos = playersLayout.CenterPosition;
                pos.Y = mainLayout.Size.Y * 0.5f;
                playersLayout.CenterPosition = pos;
            }

            currentChoice = myGame.GetNextAcceptableChoice(-1);
            ChangeChoice();

            // centrage du layout principal
            mainLayout.CenterPosition = new Vector2f(myApp.window.Size.X / 2f, myApp.window.Size.Y / 2f);
        }

        public void Update(Stopwatch time)
        {
            myUIManager.Update(time);
        }

        public void Draw(Stopwatch time)
        {
            myApp.window.Clear(myStyle.BackgroundColor);
            myUIManager.Render();
        }
    }
}
