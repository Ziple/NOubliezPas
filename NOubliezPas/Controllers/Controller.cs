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
        }

        public virtual void ActivateController()
        {}

        public virtual void DesactivateController()
        {}

        public virtual Box GetPaneBox() { return this; }

        public virtual void ReadMessage(GameToControllerWindowMessage msg) { }
    }
}
