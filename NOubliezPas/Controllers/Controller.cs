using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gtk;

using NOubliezPas.Communication;

namespace NOubliezPas.Controllers
{
    class Controller: VBox
    {
        protected GUILauncher myGUILauncher;

        public Controller( GUILauncher launcher )
        {
            myGUILauncher = launcher;
            ClientEvent += this.OnClientEvent;
        }

        public void OnClientEvent( object o, ClientEventArgs a )
        {
            foreach( object arg in a.Args )
            {
                GameToControllerWindowMessage msg = (GameToControllerWindowMessage)arg;
                ReadMessage(msg);
            }
        }

        public virtual void ActivateController()
        {}

        public virtual void DesactivateController()
        {}

        public virtual Box GetPaneBox() { return this; }

        public virtual void ReadMessage(GameToControllerWindowMessage msg) { }
    }
}
