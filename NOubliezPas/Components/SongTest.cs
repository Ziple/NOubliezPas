using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SFML.Window;
using SFML.Graphics;
using kT.GUI;
using NOubliezPas.Communication;

namespace NOubliezPas
{
    class SongTest: Component
    {
        GameApplication myApp = null;
        Player myPlayer = null;
        Song mySong;

        UIManager myUIManager;
        Label lyricsLabel;
        Frame frame;

        Label scoreLabel;

        bool waiting = true;


        Subtitle prevSubtitle = null;
        bool sentWaitingForAnswerNotif = false;
        bool waitingForAnswer = false;

        List<String> currentFillUps = null;
        List<bool> currentValidationList = null;
        int validatedHoleIndex = -1;

        int validated = -1;

        bool atEnd = false;

        Stopwatch totTime = new Stopwatch();

        public SongTest(GameApplication app, Player player, Song song)
        {
            myApp = app;
            myPlayer = player;
            mySong = song;
        }

        public void Activate()
        {
            myApp.SendMessage(GameToControllerWindowMessage.SongTestEnter);
        }

        public void Desactivate()
        {
            myApp.SendMessage(GameToControllerWindowMessage.SongTestExit);
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
                scoreLabel.Text = myPlayer.Name + " - " + myPlayer.Score;
            }
            else if (args.Code == Keyboard.Key.Subtract)
            {
                myPlayer.Score--;
                if (myPlayer.Score < 0)
                    myPlayer.Score = 0;

                scoreLabel.Text = myPlayer.Name + " - " + myPlayer.Score;
            }

            if ( !atEnd )
            {
                if (!waitingForAnswer)
                {
                    if (args.Code == Keyboard.Key.Return)
                    {
                        if (waiting)
                        {
                            waiting = false;
                            mySong.Music.Play();
                            totTime.Start();
                        }
                    }
                }
                else//waiting for answer&&!atEnd
                {
                    if (args.Code == Keyboard.Key.Return
                        || args.Code == Keyboard.Key.Space )// validation de la réponse
                        ValidateAnswer();

                    // validation de mots supplémentaires/en moins au clavier
                    if ((args.Code == Keyboard.Key.V) || (args.Code == Keyboard.Key.B))
                    {
                        if (args.Code == Keyboard.Key.V)
                            validatedHoleIndex++;
                        else if (args.Code == Keyboard.Key.B)
                            validatedHoleIndex--;

                        currentValidationList = BuildValidationList(validatedHoleIndex);
                        if (currentFillUps != null)
                            FillHoles(currentFillUps, currentValidationList);
                    }

                }
            }
            else// at end
            {
                if (waiting)// waiting
                {
                    if (args.Code == Keyboard.Key.Return)
                        DoTransition();
                }
            }
        }

        public void ValidateAnswer()
        {
            waitingForAnswer = false;
            sentWaitingForAnswerNotif = false;

            float t = (float)(totTime.ElapsedMilliseconds / 1000d);
            int index = mySong.Lyrics.AtTime(0, t);
            validated = index;
            mySong.Music.Play();
            totTime.Start();
        }

        void DoTransition()
        {
            mySong.Music.Stop();

            if (myApp.game.CanStartBlindTest())
            {
                Player pl = null;
                for (int i = 0; i < myApp.game.NumPlayers; i++)
                    if (!myApp.game.Players[i].DidBlindTest)
                    {
                        pl = myApp.game.Players[i];
                        break;
                    }

                // reste des joueurs à faire passer au blind test
                if (pl != null)
                    myApp.ChangeComponent(new BlindTest(myApp, pl, myApp.game.SharedSong));
                else
                    throw new Exception("Pas possible de démarrer le blind test? WTF!");

            }
            else if (myApp.game.RemainingThemeAvalaible())
            {
                // reste des themes
                myApp.ChangeComponent(new ThemeSelectionMenu(myApp));
            }
            else
                throw new Exception("Plus de theme restant mais ne peut pas démarrer le blind test? WTF!");
        }

        public void Initialize()
        {
        }

        public void LoadContent()
        {
            mySong.DoLoad();

            Texture[] textures = myApp.guiStyle.labelTextures;

            myUIManager = new UIManager(myApp.window);

            DCFont myFont = new DCFont(new Font("Content/verdana.ttf"));

            Frame f = new Frame(myUIManager, null);
            f.BordersImages = textures;
            f.Visible = true;

            scoreLabel = new Label(myUIManager, null, myFont, myPlayer.Name + " - " + myPlayer.Score);
            scoreLabel.Tint = Color.White;
            scoreLabel.Visible = true;
            f.Position = new Vector2f(10f, 10f);
            f.ContainedWidget = scoreLabel;

            frame = new Frame(myUIManager, null);
            frame.BordersImages = textures;
            frame.Visible = true;

            lyricsLabel = new Label(myUIManager, null, myFont, "");
            lyricsLabel.Tint = Color.White;
            lyricsLabel.Visible = true;
            frame.ContainedWidget = lyricsLabel;
            frame.CenterPosition = new Vector2f(0.5f * (float)myApp.window.Size.X, 0.75f * (float)myApp.window.Size.Y);
        }

        public Subtitle GetCurrentSubtitle()
        {
            float t = (float)(totTime.ElapsedMilliseconds / 1000d);
            int index = mySong.Lyrics.AtTime(0, t);

            if (index < mySong.Lyrics.SubtitlesCount
                    && index >= 0)
                    return mySong.Lyrics.GetSubtitle(index);

            return null;
        }

        public void FillHoles(List<String> holesFillUps, List<bool> validationList = null)
        {
            Subtitle sub = GetCurrentSubtitle();
            if( sub != null && sub.ContainHole )
            {
                currentFillUps = holesFillUps;
                sub.FillHoles(holesFillUps, validationList);

                currentValidationList = validationList;
                if( currentValidationList == null )
                {
                    currentValidationList = new List<bool>();
                    for (int i = 0; i < sub.NumHoles; i++)
                        currentValidationList.Add(false);
                }

                // recalculate the index of the last validated hole.
                validatedHoleIndex = -1;
                for (int i = 0; i < currentValidationList.Count; i++)
                    if (currentValidationList[i])
                        validatedHoleIndex = i;
                    else
                        break;
            }
        }

        List<bool> BuildValidationList( int lastValidatedIndex )
        {
            List<bool> ret = null;
            Subtitle sub = GetCurrentSubtitle();

            if( sub != null )
            {
                ret = new List<bool>();

                for (int i = 0; i < sub.NumHoles; i++)
                    ret.Add( (i <= lastValidatedIndex) );
            }

            return ret;
        }

        /// <summary>
        /// Called when the subtitle change.
        /// Invalidates the hole validation list and indexes
        /// </summary>
        void OnSubtitleChanged()
        {
            currentFillUps = null;
            currentValidationList = null;
            validatedHoleIndex = -1;
        }

        public void Update(Stopwatch time)
        {
            myUIManager.Update(time);

            float t = (float)(totTime.ElapsedMilliseconds / 1000d);
            int index = mySong.Lyrics.AtTime(0, t);

            atEnd = (t >= (mySong.Music.Duration.TotalMilliseconds / 1000d));
            if (!atEnd)
            {
                Subtitle sub = GetCurrentSubtitle();

                if (sub != prevSubtitle)
                    OnSubtitleChanged();

                prevSubtitle = sub;
                if ( sub != null )
                {
                    if (sub.ContainHole)
                    {
                        if (validated < index)// on attend la réponse
                        {
                            waitingForAnswer = true;
                            mySong.Music.Pause();
                            totTime.Stop();
                        }

                        if (waitingForAnswer)
                            lyricsLabel.SetTextFromSlices( sub.GetDisplayedSlices() );
                        else
                            lyricsLabel.Text = sub.CompleteText;
                    }
                    else
                        lyricsLabel.SetTextFromSlices(sub.GetDisplayedSlices());
                }
                else
                    lyricsLabel.Text = "";

                if (lyricsLabel.Text == "")
                    frame.Visible = false;
                else
                    frame.Visible = true;

                frame.CenterPosition = new Vector2f(0.5f * myApp.window.Size.X, 0.75f * myApp.window.Size.Y);
            }
            else
            {
                frame.Visible = false;
                waiting = true;
            }

            if (waitingForAnswer && !sentWaitingForAnswerNotif)
            {
                myApp.OurGameToControllerPipe.SendMessage(GameToControllerWindowMessage.ApplicationWaitingAnswer);
                sentWaitingForAnswerNotif = true;
            }
        }

        public void Draw(Stopwatch time)
        {
            myUIManager.Render();
        }
    }
}
