using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kT.GUI;
using SFML.Window;
using SFML.Graphics;
using System.Diagnostics;

namespace NOubliezPas
{
    class BlindTest : Component
    {
        GameApplication myApp = null;
        Player myPlayer = null;
        Song mySong;

        UIManager myUIManager;
        Label lyricsLabel;
        Frame lyricsFrame;

        Frame songNameFrame;

        Label scoreLabel;

        bool displaySongName = true;
        bool waiting = true;
        bool atEnd = false;

        Stopwatch totTime = new Stopwatch();

        public BlindTest(GameApplication app, Player player, Song song)
        {
            myApp = app;
            myPlayer = player;
            myPlayer.DidBlindTest = true;
            mySong = song;
        }

        public void OnKeyPressed(object sender, EventArgs e)
        {
            KeyEventArgs args = (KeyEventArgs)e;

            // possibilitié de continuer le jeu à n'importe quel moment
            if (args.Code == Keyboard.Key.A)
                DoTransition();

            // possibilité d'attribuer des points n'importe quand
            if (args.Code == Keyboard.Key.Add)
            {
                myPlayer.Score++;
                scoreLabel.Text = myPlayer.Name + " - " + myPlayer.Score;
            }
            else if (args.Code == Keyboard.Key.Subtract)
            {
                myPlayer.Score--;
                if (myPlayer.Score < 0)
                    myPlayer.Score = 0;

                scoreLabel.Text = myPlayer.Name + " - " + myPlayer.Score;
            }

            if (!atEnd)
            {
                if (displaySongName && waiting)
                {
                    if (args.Code == Keyboard.Key.Return)
                    {
                        waiting = false;
                        displaySongName = false;
                        mySong.Music.Play();
                        totTime.Start();
                    }
                }
                else if(!waiting)
                {
                    // on veut arrêter pour compter les points par exemple
                    if (args.Code == Keyboard.Key.Space)
                    {
                        waiting = true;
                        mySong.Music.Pause();
                        totTime.Stop();
                    }
                }
            }

            if (waiting)
            {
                // on veut continuer le jeu
                // par exemple lancer le blind test suivant
                if (args.Code == Keyboard.Key.Return)
                    DoTransition();
            }
        }

        void DoTransition()
        {
            mySong.Music.Stop();

            Player pl = null;
            for (int i = 0; i < myApp.game.NumPlayers; i++)
                if (!myApp.game.Players[i].DidBlindTest)
                {
                    pl = myApp.game.Players[i];
                    break;
                }

            // reste des joueurs à faire passer au blind test
            if (pl != null)
            {
                myApp.mustChangeComponent = true;
                myApp.newComponent = new BlindTest(myApp, pl, myApp.game.SharedSong);
                myApp.newComponent.Initialize();
                myApp.newComponent.LoadContent();
            }
            // Faut lancer la finale gars!
            else
            {
                Player bpl = myApp.game.Players[0];

                for (int i = 1; i < myApp.game.NumPlayers; i++)
                    if (myApp.game.Players[i].Score >= bpl.Score)
                        bpl = myApp.game.Players[i];

                myApp.mustChangeComponent = true;
                myApp.newComponent = new BlindTest(myApp, bpl, myApp.game.FinalSong);
                myApp.newComponent.Initialize();
                myApp.newComponent.LoadContent();
            }
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            mySong.DoLoad();

            Texture[] labelTextures = myApp.guiStyle.labelTextures;
            Texture[] scoreTextures = myApp.guiStyle.scoreTextures;

            myUIManager = new UIManager(myApp.window);

            DCFont myFont = new DCFont(new Font("Content/verdana.ttf"));

            songNameFrame = new Frame(myUIManager, null);
            songNameFrame.BordersImages = labelTextures;
            songNameFrame.Visible = true;

            Label songNameLabel = new Label(myUIManager, null, myFont, mySong.Name );
            songNameLabel.Tint = Color.White;
            songNameLabel.Visible = true;
            songNameFrame.CenterPosition = (new Vector2f( myApp.window.Size.X, myApp.window.Size.Y)-songNameFrame.Size) / 2f; ;
            songNameFrame.ContainedWidget = songNameLabel;

            Frame scoreFrame = new Frame(myUIManager, null);
            scoreFrame.BordersImages = scoreTextures;
            scoreFrame.Visible = true;

            scoreLabel = new Label(myUIManager, null, myFont, myPlayer.Name + " - " + myPlayer.Score);
            scoreLabel.Tint = Color.White;
            scoreLabel.Visible = true;
            scoreFrame.Position = new Vector2f(10f, 10f);
            scoreFrame.ContainedWidget = scoreLabel;

            lyricsFrame = new Frame(myUIManager, null);
            lyricsFrame.BordersImages = labelTextures;
            lyricsFrame.Visible = true;

            lyricsLabel = new Label(myUIManager, null, myFont, "");
            lyricsLabel.Tint = Color.White;
            lyricsLabel.Visible = true;
            lyricsFrame.ContainedWidget = lyricsLabel;
            lyricsFrame.CenterPosition = new Vector2f(0.5f * (float)myApp.window.Size.X, 0.75f * (float)myApp.window.Size.Y);
        }

        public void Update(Stopwatch time)
        {
            myUIManager.Update(time);

            float t = (float)(totTime.ElapsedMilliseconds / 1000d);
            int index = mySong.Lyrics.AtTime(0, t);

            atEnd = (t >= (mySong.Music.Duration.TotalMilliseconds / 1000d));
            if (!atEnd)
            {
                if (displaySongName)
                {
                    lyricsFrame.Visible = false;
                    songNameFrame.Visible = true;
                }
                else
                {
                    if (index < mySong.Lyrics.SubtitlesCount
                        && index >= 0)
                    {
                        Subtitle sub = mySong.Lyrics.GetSubtitle(index);
                        lyricsLabel.Text = sub.CompleteText;
                    }
                    else
                        lyricsLabel.Text = "";

                    if (lyricsLabel.Text == "")
                        lyricsFrame.Visible = false;
                    else
                        lyricsFrame.Visible = true;

                    lyricsFrame.CenterPosition = new Vector2f(0.5f * myApp.window.Size.X, 0.75f * myApp.window.Size.Y);
                }
            }
            else
            {
                lyricsFrame.Visible = false;
                waiting = true;
            }
        }

        public void Draw(Stopwatch time)
        {
            myUIManager.Render();
        }
    }
}
