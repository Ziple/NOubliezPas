using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kT.GUI;
using SFML.Window;
using SFML.Graphics;
using System.Diagnostics;

using NOubliezPas.Communication;

namespace NOubliezPas
{
    /*
    class FinalTest : Component
    {
        GameApplication myApp = null;
        Player myPlayer = null;
        Song mySong;

        UIManager myUIManager;

        Frame songNameFrame;

        Label scoreLabel;

        bool waiting = true;
        bool atEnd = false;

        Stopwatch totTime = new Stopwatch();

        public FinalTest(GameApplication app, Player player, Song song)
        {
            myApp = app;
            myPlayer = player;
            myPlayer.DidBlindTest = true;
            mySong = song;
        }

        public void Activate()
        {
            myApp.SendMessage(GameToControllerWindowMessage.FinalTestEnter);
        }

        public void Desactivate()
        {
            myApp.SendMessage(GameToControllerWindowMessage.FinalTestExit);
        }

        public void ReadMessage(ControllerToGameMessage msg)
        {

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
                scoreLabel.Text = myPlayer.Name + "   " + myPlayer.Score;
            }
            else if (args.Code == Keyboard.Key.Subtract)
            {
                myPlayer.Score--;
                if (myPlayer.Score < 0)
                    myPlayer.Score = 0;

                scoreLabel.Text = myPlayer.Name + "   " + myPlayer.Score;
            }

            if (!atEnd)
            {
                if (!waiting)
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
                // (afficher le résultat)
                if (args.Code == Keyboard.Key.Return)
                    DoTransition();
            }
        }

        void DoTransition()
        {
            mySong.Music.Stop();
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            mySong.DoLoad();

            Texture[] labelTextures = myApp.guiStyle.labelTextures;

            myUIManager = new UIManager(myApp.window);

            DCFont myFont = new DCFont(new Font("Content/verdana.ttf"));

            songNameFrame = new Frame(myUIManager, null);
            songNameFrame.BordersImages = labelTextures;
            songNameFrame.Visible = true;

            Label songNameLabel = new Label(myUIManager, null, myFont, mySong.Name);
            songNameLabel.Tint = Color.White;
            songNameLabel.Visible = true;
            songNameFrame.CenterPosition = (new Vector2f(myApp.window.Size.X, myApp.window.Size.Y)-songNameFrame.Size) / 2f; ;
            songNameFrame.ContainedWidget = songNameLabel;

            Frame scoreFrame = new Frame(myUIManager, null);
            scoreFrame.BordersImages = labelTextures;
            scoreFrame.Visible = true;

            scoreLabel = new Label(myUIManager, null, myFont, myPlayer.Name + "   " + myPlayer.Score);
            scoreLabel.Tint = Color.White;
            scoreLabel.Visible = true;
            scoreFrame.Position = new Vector2f(10f, 10f);
            scoreFrame.ContainedWidget = scoreLabel;
        }

        public void Update(Stopwatch time)
        {
            myUIManager.Update(time);

            float t = (float)(totTime.ElapsedMilliseconds / 1000d);
            int index = mySong.Lyrics.AtTime(0, t);

            atEnd = (t >= (mySong.Music.Duration.TotalMilliseconds / 1000d));
            if (atEnd)
                waiting = true;
        }

        public void Draw(Stopwatch time)
        {
            myUIManager.Render();
        }
    }
     * */
}
