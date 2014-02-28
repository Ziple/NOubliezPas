using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using System.Diagnostics;

namespace NOubliezPas
{
    class GameApplication
    {
        public RenderWindow window;
        Styles windowStyle = Styles.Default;
        bool recreateWindow = false;

        public GuiStyle guiStyle;

        Component activeComponent;
        public bool mustChangeComponent = false;
        public Component newComponent = null;

        public GameState game = null;


        public GameApplication()
        {
            game = GameState.LoadFromFile("partie.xml");
            activeComponent = new ThemeSelectionMenu(this );
            RecreateWindow();
        }

        void OnClose(object sender, EventArgs e)
        {
            // Close the window when OnClose event is received
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        void OnKeyPressed(object sender, EventArgs e)
        {
            // Toogle fullscreen/windowed if necessary
            RenderWindow window = (RenderWindow)sender;
            KeyEventArgs args = (KeyEventArgs)e;

            if (args.Code == Keyboard.Key.F)
            {
                if (windowStyle == Styles.Fullscreen)
                    windowStyle = Styles.Default;
                else
                    windowStyle = Styles.Fullscreen;

                recreateWindow = true;
            }
            else
                activeComponent.OnKeyPressed(sender, e);
        }

        void RecreateWindow()
        {
            if (window != null)
            {
                window.Close();
                window.Dispose();
            }

            window = new RenderWindow(VideoMode.DesktopMode, "N'oubliez pas les paroles", windowStyle);
            window.SetActive();
            window.SetVisible(true);
            window.Closed += new EventHandler(OnClose);
            window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);

            guiStyle = GuiStyle.LoadFromFile("configihm.xml");

            activeComponent.Initialize();
            activeComponent.LoadContent();
        }

        void DoTransition()
        {
        }

        public void Run()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (window.IsOpen())
            {
                window.DispatchEvents();

                if (recreateWindow)
                {
                    RecreateWindow();
                    recreateWindow = false;
                }

                window.Clear(Color.Green);

                watch.Stop();
                activeComponent.Update( watch );
                activeComponent.Draw( watch );

                watch.Reset();
                watch.Start();

                window.Display();

                if (mustChangeComponent)
                {
                    activeComponent = newComponent;
                    mustChangeComponent = false;
                }
            }
        }
    }
}
