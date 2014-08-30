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
    class BlindTest : SongTest
    {
        public BlindTest(GameApplication app, Player player, Song song):
            base(app, player, song)
        {
            player.DidBlindTest = true;
        }

        protected override void DoTransition()
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
                myApp.ChangeComponent(new BlindTest(myApp, pl, myApp.game.SharedSong));
            // Faut lancer la finale gars!
            else
            {
                Player bpl = myApp.game.Players[0];

                for (int i = 1; i < myApp.game.NumPlayers; i++)
                    if (myApp.game.Players[i].Score >= bpl.Score)
                        bpl = myApp.game.Players[i];

                myApp.ChangeComponent(new SongTest(myApp, bpl, myApp.game.FinalSong));
            }
        }
    }
}
