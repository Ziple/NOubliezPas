using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using SFML.Audio;
using SFML.Graphics;

namespace NOubliezPas
{
    #region Subtitle
    class Subtitle
    {
        float myStartTime;
        float myEndTime;
        bool myContainHole = false;
        String myTextWithAllHoles;
        String myWholeText = null;
        String myDisplayedText = null;

        List<KeyValuePair<String, Color>> mySliceCache = null;

        Mutex mySliceCacheMutex = new Mutex();
        public List<KeyValuePair<String, Color>> MySliceCache
        {
            get {
                mySliceCacheMutex.WaitOne();
                List<KeyValuePair<String, Color>> ret =  mySliceCache;
                mySliceCacheMutex.ReleaseMutex();
                return ret;
            }
            set {
                mySliceCacheMutex.WaitOne();
                mySliceCache = value;
                mySliceCacheMutex.ReleaseMutex();
            }
        }

        public Subtitle(float startTime, float endTime, String text)
        {
            myStartTime = startTime;
            myEndTime = endTime;
            myWholeText = text;
            myTextWithAllHoles = text;
            myDisplayedText = text;

            BuildSlicesCache();
        }

        public Subtitle(float startTime, float endTime, String text, String wholeText )
        {
            myStartTime = startTime;
            myEndTime = endTime;
            myTextWithAllHoles = text;
            myWholeText = wholeText;
            myDisplayedText = text;
            myContainHole = (NumHoles > 0);

            BuildSlicesCache();
        }

        void BuildSlicesCache()
        {
            List<KeyValuePair<String,Color>> ret = new List<KeyValuePair<string,Color>>();

            List<String> slices = GetTextSlices();
            foreach (String s in slices)
                ret.Add(new KeyValuePair<string, Color>(s, Color.White));

            MySliceCache = ret;
        }

        public List<KeyValuePair<String, Color>> GetDisplayedSlices()
        {
            return MySliceCache;
        }

        public bool ContainHole
        {
            get { return myContainHole; }
        }

        public List<String> GetTextSlices()
        {
            List<String> ret = new List<String>();

            int sliceStart = -1;
            for (int i = 0; i < myTextWithAllHoles.Length; )
            {
                if( myTextWithAllHoles[i] == '_' )
                {
                    sliceStart = i;

                    while (i < myTextWithAllHoles.Length && myTextWithAllHoles[i] == '_')
                        i++;

                    int sliceEnd = i - 1;

                    int count = sliceEnd - sliceStart + 1;
                    if( count >= 0 )
                        ret.Add(myTextWithAllHoles.Substring(sliceStart, sliceEnd - sliceStart + 1));

                    sliceStart = -1;
                }
                else
                {
                    sliceStart = i;

                    while (i < myTextWithAllHoles.Length && myTextWithAllHoles[i] != '_')
                        i++;
                    int sliceEnd = i - 1;

                    int count = sliceEnd - sliceStart + 1;
                    if (count >= 0)
                        ret.Add(myTextWithAllHoles.Substring(sliceStart, sliceEnd - sliceStart + 1));

                    sliceStart = -1;
                }
            }

            return ret;
        }
        public List<KeyValuePair<int,int>> GetHolesList()
        {
            List<KeyValuePair<int, int>> ret = new List<KeyValuePair<int, int>>();

            for (int i = 0; i < myTextWithAllHoles.Length; i++  )
            {
                if( myTextWithAllHoles[i] == '_' )
                {
                    int start = i;
                    int end = i;

                    while( i < myTextWithAllHoles.Length && myTextWithAllHoles[i] == '_' )
                    {
                        end = i;
                        i++;
                    }

                    ret.Add(new KeyValuePair<int, int>(start, end));
                }
            }
                return ret;
        }
        public int NumHoles
        {
            get {
                return GetHolesList().Count;
            }
        }

        bool AnswerCanBeCorrect( String replacerText)
        {
            replacerText = replacerText.ToLower();
            String wholeText = myWholeText.ToLower();

            for (int j = 0; j < replacerText.Length; j++ )
            {
                if( replacerText[j] != wholeText[j] )
                    return false;
            }

            return true;
        }

        Mutex myFillHolesMutex = new Mutex();
        public void FillHoles( List<String> holesFillUps, List<bool> validationList = null )
        {
            myFillHolesMutex.WaitOne();

            if (holesFillUps.Count == NumHoles)
            {
                List<KeyValuePair<String, Color>> sliceCache = new List<KeyValuePair<string,Color>>();

                if (validationList == null)
                {
                    validationList = new List<bool>();
                    for (int i = 0; i < holesFillUps.Count; i++)
                        validationList.Add(false);
                }

                List<String> textSlices = GetTextSlices();
                List<String> replacedSlices = new List<String>();

                String replacerText = "";
                int replacerIndex = 0;
                for (int i = 0; i < textSlices.Count; i++)
                {
                    if (textSlices[i].Contains('_'))//a hole
                    {
                        String s = holesFillUps[replacerIndex];
                        replacerText += s;

                        Color c = Color.White;

                        if (validationList[replacerIndex])// Choose the right color
                            c = AnswerCanBeCorrect(replacerText) ? Color.Green : Color.Red;
                        else
                            c = Color.Blue;

                        replacedSlices.Add(s);
                        sliceCache.Add(new KeyValuePair<string, Color>(s, c));
                        replacerIndex++;
                    }
                    else// not a hole
                    {
                        replacerText += textSlices[i];
                        replacedSlices.Add(textSlices[i]);
                        sliceCache.Add(new KeyValuePair<string, Color>(textSlices[i], Color.White));
                    }
                }

                myDisplayedText = replacerText;
                MySliceCache = sliceCache;
            }

            myFillHolesMutex.ReleaseMutex();
        }

        public String CompleteText
        {
            get { return myWholeText; }
        }

        public float StartTime
        {
            get { return myStartTime; }
        }

        public float EndTime
        {
            get { return myEndTime; }
        }
    }
    #endregion
    #region Lyrics
    class Lyrics
    {
        List<Subtitle> mySubtitles;

        public Lyrics( List<Subtitle> subtitles )
        {
            mySubtitles = subtitles;
        }

        /// <summary>
        /// Find the subtitle adapted to the given time.
        /// Null if no one found.
        /// </summary>
        /// <param name="index">Index to begin the search at.</param>
        /// <param name="seconds">Time</param>
        /// <returns>The right subtitle or null if no one found</returns>
        public int AtTime(int index, float seconds)
        {
            while ( index < mySubtitles.Count )
            {
                if (mySubtitles[index].StartTime <= seconds
                    && mySubtitles[index].EndTime >= seconds)
                    break;
                index++;
            }
            return index;
        }

        public int SubtitlesCount
        {
            get { return mySubtitles.Count; }
        }

        public Subtitle GetSubtitle(int index)
        {
            return mySubtitles[index];
        }

        static float AsTime(String time)
        {
            int minPos = time.IndexOf(":");
            int secPos = time.IndexOf(":", minPos + 1);
            int dPos = time.IndexOf(",");

            float rtime = 0.0f;

            string hours = time.Substring(0, minPos);
            rtime += float.Parse(hours) * 60f * 60f;//heures

            string minutes = time.Substring(minPos + 1, secPos - (minPos + 1));
            rtime += float.Parse(minutes) * 60f;//minutes

            string secs = time.Substring(secPos + 1, dPos - (secPos + 1));
            rtime += float.Parse(secs);//secondes

            string fracpart = time.Substring( dPos+1, time.Length - (dPos+1) ).Trim();
            rtime += float.Parse(fracpart ) * (float)Math.Pow( 0.1, (double)fracpart.Length);//partie décimale

            return rtime;
        }

        public static Lyrics LoadFromFile(String src)
        {
            List<Subtitle> lyrics = new List<Subtitle>();
            StreamReader subStream = File.OpenText(src);

            bool read = true;
            while (read)
            {
                String line = subStream.ReadLine();
                if (line != null)
                {
                    line = line.Trim();
                    if (line != "")
                    {
                        Subtitle sub = null;
                        if (line.StartsWith("'"))
                        {
                            // parole à trou
                            String time = subStream.ReadLine();

                            int pos = time.IndexOf("-->");
                            String startTime = time.Substring(0, pos);
                            String endTime = time.Substring(pos + 3, time.Length - (pos + 3));

                            float start = Lyrics.AsTime(startTime);
                            float end = Lyrics.AsTime(endTime);

                            // d'abord le texte à trou
                            String text = "";

                            line = subStream.ReadLine();
                            while ((line != null) && (line != ""))
                            {
                                text += line + "\n";
                                line = subStream.ReadLine();
                            }

                            // ensuite le texte complet
                            String wholetext = "";

                            line = subStream.ReadLine();
                            while ((line != null) && (line != ""))
                            {
                                wholetext += line + "\n";
                                line = subStream.ReadLine();
                            }

                            sub = new Subtitle(start, end, text, wholetext);
                        }
                        else
                        {
                            // parole normale
                            String time = subStream.ReadLine();

                            int pos = time.IndexOf("-->");
                            String startTime = time.Substring(0, pos);
                            String endTime = time.Substring(pos + 3, time.Length - (pos + 3));

                            float start = Lyrics.AsTime(startTime);
                            float end = Lyrics.AsTime(endTime);

                            // d'abord le texte à trou
                            String text = "";

                            line = subStream.ReadLine();
                            while ((line != null) && (line != ""))
                            {
                                text += line + "\n";
                                line = subStream.ReadLine();
                            }

                            sub = new Subtitle(start, end, text);
                        }
                        lyrics.Add(sub);
                    }
                }
                else
                    read = false;
            }
            return new Lyrics(lyrics);
        }
    }

    #endregion
    #region Song
    class Song
    {
        String mySongName;
        String mySongSource;
        String myLyricsSource;

        Music myMusic;
        Lyrics myLyrics;

        public Song( String songname, String songsrc, String textsrc )
        {
            mySongName = songname;
            mySongSource = songsrc;
            myLyricsSource = textsrc;
        }

        public static Song PrepareLoadFromFile(String filename)
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

            return PrepareLoad(reader);
        }

        public static Song PrepareLoad(XmlReader reader)
        {
            String songname = "";
            String songsrc = "";
            String textsrc = "";

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            if (reader.Name == "chanson")
                            {
                                songname = reader.GetAttribute("nom");

                                songsrc = reader.GetAttribute("src");
                                if (songsrc != null)
                                    songsrc = Path.GetFullPath(songsrc);

                                textsrc = reader.GetAttribute("parolesSrc");
                                if (textsrc != null)
                                    textsrc = Path.GetFullPath(textsrc);
                            }
                        }
                        break;
                }
            }

            return new Song(songname, songsrc, textsrc);
        }

        public void DoLoad()
        {
            myMusic = new Music(mySongSource);
            myLyrics = Lyrics.LoadFromFile(myLyricsSource);
        }

        public String Name
        {
            get { return mySongName; }
        }

        public String SongSource
        {
            get { return mySongSource; }
        }

        public String LyricsSource
        {
            get { return myLyricsSource; }
        }

        public Music Music
        {
            get { return myMusic; }
        }

        public Lyrics Lyrics
        {
            get { return myLyrics; }
        }
    }
    #endregion
    #region Theme
    class Theme
    {
        String myName;
        uint myPoints;
        List<Song> mySongs;

        public Theme( String name, uint points, List<Song> songs )
        {
            myName = name;
            myPoints = points;
            mySongs = songs;
        }

        public static Theme LoadFromFile(String filename)
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

        public static Theme Load(XmlReader reader)
        {
            String name = "";
            uint points = 0;

            List<Song> songs = new List<Song>();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            if (reader.Name == "theme")
                            {
                                String rname = reader.GetAttribute("nom");
                                if (rname != null)
                                    name = rname;

                                String rpts = reader.GetAttribute("points");
                                if (rpts != null)
                                    points = UInt32.Parse(rpts);
                            }
                            else if (reader.Name == "chanson")
                            {
                                Song song = null;
                                String src = reader.GetAttribute("src");
                                if (src != null)
                                {
                                    String curDir = System.IO.Directory.GetCurrentDirectory();
                                    String fullSrcPath = System.IO.Path.GetFullPath(src);
                                    String fullDirPath = System.IO.Path.GetDirectoryName(fullSrcPath);
                                    System.IO.Directory.SetCurrentDirectory(fullDirPath);
                                    song = Song.PrepareLoadFromFile(fullSrcPath);
                                    System.IO.Directory.SetCurrentDirectory(curDir);
                                }
                                else
                                    song = Song.PrepareLoad(reader.ReadSubtree() );

                                songs.Add(song);
                            }
                            break;
                        }
                }
            }

            return new Theme(name, points, songs);
        }

        public String Name
        {
            get { return myName; }
        }

        public uint Points
        {
            get { return myPoints; }
        }

        public List<Song> Songs
        {
            get { return mySongs; }
        }

        public int NumSongs
        {
            get { return mySongs.Count; }
        }

        public Song GetSong(int index)
        {
            return mySongs[index];
        }
    }
    #endregion
    #region Player
    class Player
    {
        public String Name = "";
        public String PhotoSrc = null;
        public int Score = 0;
        public List<Theme> ChosenThemes = new List<Theme>();
        public bool DidBlindTest = false;

        public Player( String name, String photosrc )
        {
            Name = name;
            PhotoSrc = photosrc;
        }

        public static Player Load(XmlReader reader)
        {
            String name = null;
            String photo = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "joueur")
                        {
                            name = reader.GetAttribute("nom");
                            photo = reader.GetAttribute("photosrc");
                            photo = Path.GetFullPath(photo);
                        }
                        break;
                }
            }

            return new Player(name, photo);
        }
    }
    #endregion
    #region GameState
    class GameState
    {
        List<Theme> myThemes;
        List<Player> myPlayers;
        Song mySharedSong;
        Song myFinalSong;

        public GameState(String filename)
        {
            LoadFromFile(filename);
        }

        public GameState(List<Theme> themes, List<Player> players, Song sharedSong, Song finalSong )
        {
            myThemes = themes;
            myPlayers = players;
            mySharedSong = sharedSong;
            myFinalSong = finalSong;
        }

        public static GameState LoadFromFile(String filename)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
			settings.DtdProcessing = DtdProcessing.Ignore;

            StreamReader stream;
            try {
                stream = System.IO.File.OpenText(filename);
            } catch (System.IO.FileNotFoundException) {
                throw new Exception("Can't load the specified file");
            }

            XmlReader reader = XmlReader.Create(stream, settings);

            return Load( reader );
        }

        public static GameState Load(XmlReader reader)
        {
            int numJoueurs = 0;
            List<Theme> themes = new List<Theme>();
            List<Player> players = new List<Player>();
            Song sharedSong = null;
            Song finalSong = null;

            while (reader.Read())
            {
                switch(reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "partie")
                        {
                            string strNumJoueurs = reader.GetAttribute("joueurs");
                            if (strNumJoueurs != null)
                                numJoueurs = int.Parse(strNumJoueurs);
                        }
                        else if (reader.Name == "theme")
                        {
                            Theme th = Theme.Load(reader.ReadSubtree());
                            themes.Add(th);
                        }
                        else if (reader.Name == "joueur")
                        {
                            Player rd = Player.Load(reader.ReadSubtree());
                            players.Add(rd);
                        }
                        else if (reader.Name == "chanson")
                        {
                            string src = reader.GetAttribute("src");
                            if (src != null)
                            {
                                String curDir = System.IO.Directory.GetCurrentDirectory();
                                String fullSrcPath = System.IO.Path.GetFullPath(src);
                                String fullDirPath = System.IO.Path.GetDirectoryName(fullSrcPath);
                                System.IO.Directory.SetCurrentDirectory(fullDirPath);

                                if (sharedSong == null)
                                    sharedSong = Song.PrepareLoadFromFile(fullSrcPath);
                                else
                                    finalSong = Song.PrepareLoadFromFile(fullSrcPath);

                                System.IO.Directory.SetCurrentDirectory(curDir);
                            }
                            else
                                throw new Exception("Pas de source pour la chanson commune? WTF");
                        }
                        break;
                }
            }

            return new GameState(themes, players, sharedSong, finalSong);
        }

        public bool CanStartBlindTest()
        {
            bool can = true;

            for( int i = 0; i < NumPlayers; i++ )
                if (myPlayers[i].ChosenThemes.Count < 2)
                {
                    can = false;
                    break;
                }
            return can;
        }

        public bool RemainingThemeAvalaible()
        {
            List<Theme> avalaibleThemes = new List<Theme>();
            for (int i = 0; i < NumThemes; i++)
                avalaibleThemes.Add(Themes[i]);

            for (int i = 0; i < NumPlayers; i++)
            {
                for (int j = 0; j < avalaibleThemes.Count; j++)
                {
                    if (Players[i].ChosenThemes.Contains(avalaibleThemes[j]))
                        avalaibleThemes.Remove(avalaibleThemes[j]);
                }
            }
            return (avalaibleThemes.Count > 0);
        }

        public bool IsThemeAvalaible(Theme theme)
        {
            bool avalaible = true;

            for (int i = 0; i < NumPlayers; i++)
            {
                if (Players[i].ChosenThemes.Contains(theme))
                {
                    avalaible = false;
                    break;
                }
            }
            return avalaible;
        }

        public int CorrectThemeIndex(int choice)
        {
            while (choice < 0)
                choice += NumThemes;

            if (choice >= NumThemes)
                choice = choice % NumThemes;

            return choice;
        }

        public int GetPreviousAcceptableChoice(int choice)
        {
            if (RemainingThemeAvalaible())
            {
                choice = CorrectThemeIndex(choice - 1);
                while (!IsThemeAvalaible(Themes[choice]))
                    choice = CorrectThemeIndex(choice - 1);

                return choice;
            }
            else
                return -1;

        }

        public int GetNextAcceptableChoice(int choice)
        {
            if (RemainingThemeAvalaible())
            {
                choice = CorrectThemeIndex(choice + 1);
                while (!IsThemeAvalaible(Themes[choice]))
                    choice = CorrectThemeIndex(choice + 1);

                return choice;
            }
            else
                return -1;
        }

        public List<Player> Players
        {
            get { return myPlayers; }
        }

        public int NumPlayers
        {
            get { return myPlayers.Count;  }
        }

        public List<Theme> Themes
        {
            get { return myThemes;  }
        }

        public int NumThemes
        {
            get { return myThemes.Count; }
        }

        public Theme GetTheme(int index)
        {
            return myThemes[index];
        }

        public Song SharedSong
        {
            get { return mySharedSong; }
        }

        public Song FinalSong
        {
            get { return myFinalSong; }
        }
    }
    #endregion
}
