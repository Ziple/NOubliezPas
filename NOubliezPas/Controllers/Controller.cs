using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gtk;

using NOubliezPas.Communication;

namespace NOubliezPas.Controllers
{
    class Controller
    {
        protected GUILauncher myGUILauncher;

        public Controller( GUILauncher launcher )
        {
            myGUILauncher = launcher;
        }

        public virtual void Activate()
        {}

        public virtual void Desactivate()
        {}

        public virtual Box GetPaneBox() { return null; }

        public virtual void ReadMessage(GameToControllerWindowMessage msg) { }
    }
}
