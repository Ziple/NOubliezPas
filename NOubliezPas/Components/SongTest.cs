using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        [Flags]
        enum State
        {
            Running = 0,
            Waiting = 1,
            WaitingForAnswer = Waiting | 2,
            AtEnd = 4
        }

        protected GameApplication myApp = null;
        protected Player myPlayer = null;
        protected Song mySong;

        UIManager myUIManager;
        Label lyricsLabel;
        Frame lyricsLabelFrame;

        Label playerScoreLabel;

        bool waiting = true;

        Subtitle prevSubtitle = null;
        bool waitingForAnswer = false;

        List<String> myCurrentFillUps = null;
        Mutex myCurrentFillUpsMutex = new Mutex();

        public List<String> MyCurrentFillUps
        {
            get {
                myCurrentFillUpsMutex.WaitOne();
                List<String> val = myCurrentFillUps;
                myCurrentFillUpsMutex.ReleaseMutex();
                return val;
            }
            set
            {
                myCurrentFillUpsMutex.WaitOne();
                myCurrentFillUps = value;
                myCurrentFillUpsMutex.ReleaseMutex();
            }
        }

        List<bool> myCurrentValidationList = null;
        Mutex myCurrentValidationListMutex = new Mutex();

        public List<bool> MyCurrentValidationList
        {
            get {
                myCurrentValidationListMutex.WaitOne();
                List<bool> ret =  myCurrentValidationList;
                myCurrentValidationListMutex.ReleaseMutex();
                return ret;
            }
            set {
                myCurrentValidationListMutex.WaitOne();
                myCurrentValidationList = value;
                myCurrentValidationListMutex.ReleaseMutex();
            }
        }

        int myValidatedHoleIndex = -1;
        Mutex myValidatedHoleIndexMutex = new Mutex();

        public int MyValidatedHoleIndex
        {
            get {
                myValidatedHoleIndexMutex.WaitOne();
                int ret = myValidatedHoleIndex;
                myValidatedHoleIndexMutex.ReleaseMutex();
                return ret;
            }
            set {
                myValidatedHoleIndexMutex.WaitOne();
                myValidatedHoleIndex = value;
                myValidatedHoleIndexMutex.ReleaseMutex();
            }
        }

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
                playerScoreLabel.Text = myPlayer.Name + " - " + myPlayer.Score;
            }
            else if (args.Code == Keyboard.Key.Subtract)
            {
                myPlayer.Score--;
                if (myPlayer.Score < 0)
                    myPlayer.Score = 0;

                playerScoreLabel.Text = myPlayer.Name + " - " + myPlayer.Score;
            }

            if ( !atEnd )
            {
                if (!waiting && !waitingForAnswer)
                    if (args.Code == Keyboard.Key.Space)
                        Stop();

                if (!waitingForAnswer)
                {
                    if (args.Code == Keyboard.Key.Return)
                    {
                        if (waiting)
                            Resume();
                    }
                }
                else//waiting for answer&&!atEnd
                {
                    if (args.Code == Keyboard.Key.Return )// validation de la réponse
                        ValidateAnswer();

                    // validation de mots supplémentaires/en moins au clavier
                    if ((args.Code == Keyboard.Key.V) || (args.Code == Keyboard.Key.B))
                    {
                        if (args.Code == Keyboard.Key.V)
                            MyValidatedHoleIndex = MyValidatedHoleIndex+1;
                        else if (args.Code == Keyboard.Key.B)
                            MyValidatedHoleIndex = MyValidatedHoleIndex - 1;

                        MyCurrentValidationList = BuildValidationList(MyValidatedHoleIndex);
                        FillHoles(MyCurrentFillUps, MyCurrentValidationList);
                    }

                }
            }
            else// at end
            {
                if (args.Code == Keyboard.Key.Return)
                    DoTransition();
            }
        }

        private void Stop()
        {
            waiting = true;
            mySong.Music.Pause();
            totTime.Stop();
        }

        private void Resume()
        {
            waiting = false;
            mySong.Music.Play();
            totTime.Start();
        }

        public void ValidateAnswer()
        {
            waitingForAnswer = false;

            float t = (float)(totTime.ElapsedMilliseconds / 1000d);
            int index = mySong.Lyrics.AtTime(0, t);
            validated = index;

            Resume();
        }

        protected virtual void DoTransition()
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

            ThemeSelectionMenuStyle myStyle = myApp.guiStyle.ThemeSelectionMenuStyle;
            ImagePart[] textures = myStyle.LabelsTextures;

            myUIManager = new UIManager(myApp.window);

            DCFont myFont = new DCFont(new Font("Content/verdana.ttf"));

            Frame playerScoreLabelFrame = new Frame(myUIManager, null);
            playerScoreLabelFrame.BordersImagesParts = textures;
            playerScoreLabelFrame.Visible = true;

            playerScoreLabel = new Label(myUIManager, null, myFont, myPlayer.Name + "   " + myPlayer.Score);
            playerScoreLabel.Tint = Color.White;
            playerScoreLabel.Visible = true;
            playerScoreLabelFrame.Position = new Vector2f(50f, 20f);
            playerScoreLabelFrame.ContainedWidget = playerScoreLabel;

            Frame songNameFrame = new Frame(myUIManager, null);
            songNameFrame.BordersImagesParts = textures;
            songNameFrame.Visible = true;

            Label songNameLabel = new Label(myUIManager, null, myFont, mySong.Name);
            songNameLabel.Tint = Color.White;
            songNameLabel.Visible = true;
            songNameFrame.ContainedWidget = songNameLabel;

            
            songNameFrame.TopRightPosition = new Vector2f(myUIManager.ScreenSize.X - 50f, 20f);
            //songNameFrame.Position = new Vector2f(0.5f * (myUIManager.ScreenSize.X - songNameFrame.Size.X), (1f/8f) * myUIManager.ScreenSize.Y - 0.5f* songNameFrame.Size.Y );

            lyricsLabelFrame = new Frame(myUIManager, null);
            lyricsLabelFrame.BordersImagesParts = textures;
            lyricsLabelFrame.Visible = true;

            lyricsLabel = new Label(myUIManager, null, myFont, "");
            lyricsLabel.Tint = Color.White;
            lyricsLabel.Visible = true;
            lyricsLabelFrame.ContainedWidget = lyricsLabel;
            lyricsLabelFrame.CenterPosition = new Vector2f(0.5f * (float)myApp.window.Size.X, 0.75f * (float)myApp.window.Size.Y);
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

        Mutex myFillHolesMutex = new Mutex();

        public void FillHoles(List<String> holesFillUps, List<bool> validationList = null)
        {
            myFillHolesMutex.WaitOne();
            if (holesFillUps != null)
            {
                Subtitle sub = GetCurrentSubtitle();
                if (sub != null && sub.ContainHole)
                {
                    MyCurrentFillUps = holesFillUps;
                    sub.FillHoles(holesFillUps, validationList);

                    if (validationList == null)
                    {
                        validationList = new List<bool>();
                        for (int i = 0; i < sub.NumHoles; i++)
                            validationList.Add(false);
                    }

                    // recalculate the index of the last validated hole.
                    int validatedHoleIndex = -1;
                    for (int i = 0; i < validationList.Count; i++)
                        if (validationList[i])
                            validatedHoleIndex = i;
                        else
                            break;

                    MyValidatedHoleIndex = validatedHoleIndex;
                    MyCurrentValidationList = validationList;
                }
            }
            myFillHolesMutex.ReleaseMutex();
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
            myCurrentFillUps = null;
            myCurrentValidationList = null;
            myValidatedHoleIndex = -1;
        }

        public void Update(Stopwatch time)
        {
            // updates the score of the player
            
            if (myPlayer.Score > 0)
                playerScoreLabel.Text = myPlayer.Name + " " + myPlayer.Score;
            else
                playerScoreLabel.Text = myPlayer.Name;


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
                    // so that the frame fits perfeclty the subtitle
                    lyricsLabelFrame.Size = new Vector2f(0f, 0f);
                    lyricsLabel.Size = new Vector2f(0f, 0f);

                    if (sub.ContainHole)
                    {
                        if (validated < index)// on attend la réponse
                        {
                            waitingForAnswer = true;
                            Stop();
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
                    lyricsLabelFrame.Visible = false;
                else
                    lyricsLabelFrame.Visible = true;

                lyricsLabelFrame.CenterPosition = new Vector2f(0.5f * myApp.window.Size.X, 0.75f * myApp.window.Size.Y);
            }
            else
            {
                lyricsLabelFrame.Visible = false;
                waiting = true;
            }

            if (waitingForAnswer)// && !sentWaitingForAnswerNotif)
            {
                myApp.OurGameToControllerPipe.SendMessage(GameToControllerWindowMessage.ApplicationWaitingAnswer);
                Thread.Sleep(10);
            }
        }

        public void Draw(Stopwatch time)
        {
            myUIManager.Render();
        }
    }
}
