using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gtk;

using NOubliezPas.Communication;

namespace NOubliezPas.Controllers
{
    class FullscreeModeController: Controller
    {
        HBox hBox = null;
        CheckButton fBtn = null;

        public FullscreeModeController( GUILauncher launcher ):
            base(launcher)
        {
            hBox = new HBox();
            Add(hBox);

            fBtn = new CheckButton("Mode plein écran");
            fBtn.Toggled += this.OnToggle;
            hBox.Add(fBtn);
        }

        public override void ReadMessage(GameToControllerWindowMessage msg)
        {
            if( msg == GameToControllerWindowMessage.GoneFullscreen )
                fBtn.Active = true;
            else if ( msg == GameToControllerWindowMessage.GoneWindowed )
                fBtn.Active = false;
        }

        public void OnToggle( object o, EventArgs a )
        {
            if (fBtn.Active)
                myGUILauncher.SendMessage(ControllerToGameMessage.GoFullscreen);
            else
                myGUILauncher.SendMessage(ControllerToGameMessage.GoWindowed);
        }
    }
}
