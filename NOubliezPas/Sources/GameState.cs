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

namespace NOubliezPas
{
    #region Subtitle
    class Subtitle
    {
        float myStartTime;
        float myEndTime;
        String myText;

        bool myContainHole = false;
        String myWholeText = null;

        public Subtitle(float startTime, float endTime, String text)
        {
            myStartTime = startTime;
            myEndTime = endTime;
            myText = text;
        }

        public Subtitle(float startTime, float endTime, String text, String wholeText )
        {
            myStartTime = startTime;
            myEndTime = endTime;
            myText = text;
            myWholeText = wholeText;
            myContainHole = true;
        }

        public bool ContainHole
        {
            get { return myContainHole; }
        }

        public String CompleteText
        {
            get
            {
                if (ContainHole)
                    return myWholeText;
                return myText;
            }
        }

        public float StartTime
        {
            get { return myStartTime; }
        }

        public float EndTime
        {
            get { return myEndTime; }
        }

        public String Text
        {
            get { return myText;  }
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
        List<Song> mySongs;

        public Theme( String name, List<Song> songs )
        {
            myName = name;
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

            return new Theme(name, songs);
        }

        public String Name
        {
            get { return myName; }
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
        public int Score = 0;
        public List<Theme> ChosenThemes = new List<Theme>();
        public bool DidBlindTest = false;

        public Player( String name )
        {
            Name = name;
        }

        public static Player Load(XmlReader reader)
        {
            String name = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "joueur")
                            name = reader.GetAttribute("nom");
                        break;
                }
            }

            return new Player(name);
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
