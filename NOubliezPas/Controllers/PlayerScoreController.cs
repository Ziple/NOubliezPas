using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NOubliezPas.Communication;

using Gtk;

namespace NOubliezPas.Controllers
{
    class PlayerScoreController: Controller
    {
        HBox hBox = null;

        List<SpinButton> mySpinButtons;

        public PlayerScoreController(GUILauncher launcher) :
            base(launcher)
        {
            hBox = new HBox();

            mySpinButtons = new List<SpinButton>();
            GameState state = launcher.OurGameApp.GameState;

            for (int i = 0; i < state.NumPlayers; i++ )
            {
                Label player = new Label("Score joueur " + (i+1).ToString() + ":");
                hBox.Add(player);

                SpinButton playerSpinButton = new SpinButton(0, 9999999d, 1);
                mySpinButtons.Add(playerSpinButton);

                playerSpinButton.ValueChanged += this.OnPlayerScoreEntryChanged;

                playerSpinButton.Digits = 0;
                playerSpinButton.Numeric = true;
                playerSpinButton.Wrap = true;
                playerSpinButton.SnapToTicks = true;
                hBox.Add(playerSpinButton);
            }

            this.Add(hBox);
            this.ShowAll();
        }

        public override void ReadMessage(GameToControllerMessage msg)
        {
            if( msg.GetType() == typeof(GameToControllerMessagePlayerScoreChanged) )
            {
                List<Player> players = myGUILauncher.OurGameApp.GameState.Players;
                for( int i = 0; i < players.Count; i++)
                {
                    mySpinButtons[i].Value = (double)players[i].Score;
                }
            }
        }

        public void OnPlayerScoreEntryChanged( object o, EventArgs a )
        {
            SpinButton sp = o as SpinButton;
            int index = mySpinButtons.FindIndex(
                delegate(SpinButton b)
                {
                    return b == sp;
                });

            if( index >= 0 )
            {
                List<Player> players = myGUILauncher.OurGameApp.GameState.Players;
                players[index].Score = sp.ValueAsInt;

                myGUILauncher.SendMessage(new ControllerToGamePlayerScoreChanged());
            }
        }
    }
}
