using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NOubliezPas.Communication;

using Gtk;

namespace NOubliezPas.Controllers
{
    class ThemeSelectionController: Controller
    {
        List<Button> menuEntriesButtons;
        Button startButton;

        public ThemeSelectionController( GUILauncher guiLauncher ):
            base(guiLauncher)
        {
            menuEntriesButtons = new List<Button>();

            int numThemes = guiLauncher.OurGameApp.GameState.NumThemes;
            for (int i = 0; i < numThemes; i++ )
            {
                Theme thm = guiLauncher.OurGameApp.GameState.GetTheme(i);

                Button btn = new Button(thm.Name);
                btn.Clicked += this.OnMenuButtonClicked;
                Add(btn);
                menuEntriesButtons.Add(btn);
            }

            startButton = new Button("Lancer");
            startButton.Clicked += this.OnStartButtonClicked;
            startButton.Sensitive = false;
            Add(startButton);

            ShowAll();

            // By default, nothing is sensitive
            DesactivateController();
        }

        public override void ReadMessage(GameToControllerMessage msg)
        {
            if (msg.GetType() == typeof(GameToControllerMessageThemeSelectionMenuEnter))
                ActivateController();
            if (msg.GetType() == typeof(GameToControllerMessageThemeSelectionMenuExit))
                DesactivateController();
        }

        /// <summary>
        /// Activate the controller
        /// </summary>
        public override void ActivateController()
        {
            for( int i = 0; i < menuEntriesButtons.Count; i++ )
            {
                Theme thm = myGUILauncher.OurGameApp.GameState.GetTheme(i);
                if (myGUILauncher.OurGameApp.GameState.IsThemeAvalaible(thm))
                    menuEntriesButtons[i].Sensitive = true;
                else
                    menuEntriesButtons[i].Sensitive = false;
            }

            ShowAll();
        }

        /// <summary>
        /// Disactivate the controller.
        /// </summary>
        public override void DesactivateController()
        {
            for (int i = 0; i < menuEntriesButtons.Count; i++)
                menuEntriesButtons[i].Sensitive = false;

            HideAll();
        }

        public void OnMenuButtonClicked( object o, EventArgs a )
        {
            Button btn = o as Button;
            btn.Sensitive = false;

            int index = 0;
            for( int i = 0; i < menuEntriesButtons.Count; i++ )
            {
                Button b = menuEntriesButtons[i];
                if (b != btn)
                {
                    Theme thm = myGUILauncher.OurGameApp.GameState.Themes[i];
                    b.Sensitive = myGUILauncher.OurGameApp.GameState.IsThemeAvalaible(thm);
                }

                if( b == btn )
                    index = i;
            }

            startButton.Sensitive = true;

            myGUILauncher.SendMessage(new ControllerToGameThemeSelectionChanged(index));
        }

        public void OnStartButtonClicked( object o, EventArgs a )
        {
            myGUILauncher.SendMessage(new ControllerToGameThemeChoosen());
        }
    }
}
