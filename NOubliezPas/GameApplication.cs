using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using System.Diagnostics;

using NOubliezPas.Communication;

namespace NOubliezPas
{
    class GameApplication
    {
        public GameToControllerWindowMessagePipe OurGameToControllerPipe;
        public ControllerToGameMessagePipe OurPipeControllerToGame;

        Thread messagePumper;

        public RenderWindow window;
        Styles windowStyle = Styles.None;

        Mutex recreateWindowMutex = new Mutex();
        bool recreateWindow = false;

        public GuiStyle guiStyle;

        Component activeComponent = null;
        public bool mustChangeComponent = false;
        public Component newComponent = null;

        public GameState game = null;

        public GameState GameState
        {
            get { return game; }
        }

        Mutex runMutex = new Mutex();
        bool run = true;

        public GameApplication( GameToControllerWindowMessagePipe pipe, ControllerToGameMessagePipe toGamePipe )
        {
            OurGameToControllerPipe = pipe;
            OurPipeControllerToGame = toGamePipe;

            game = GameState.LoadFromFile("partie.xml");

            // we dont activate the component at first because there is no window at the moment
            // we need to make a do change because when we recreate the window
            // the active component is initialized.
            ChangeComponent(new ThemeSelectionMenu(this ), false);
            DoChangeComponent();

            RecreateWindow();

            messagePumper = new Thread(this.PumpMessages);
        }

        public Component ActiveComponent
        {
            get { return activeComponent; }
        }

        /// <summary>
        /// Pump the messages sent by the controller
        /// </summary>
        public void PumpMessages()
        {
            while (true)
            {
                ControllerToGameMessage msg = OurPipeControllerToGame.GetMessage();

                if (msg == ControllerToGameMessage.ApplicationQuit)
                    OnReceiveQuitMessage();
                else if (msg == ControllerToGameMessage.GoFullscreen)
                    GoFullscreen();
                else if (msg == ControllerToGameMessage.GoWindowed)
                    GoWindowed();
                else
                    activeComponent.ReadMessage(msg);
            }
        }

        public void SendMessage( GameToControllerWindowMessage msg )
        {
            OurGameToControllerPipe.SendMessage(msg);
        }

        /// <summary>
        /// force the game to quit.
        /// </summary>
        public void OnReceiveQuitMessage()
        {
            runMutex.WaitOne();
            run = false;
            runMutex.ReleaseMutex();

            DoQuit();
        }

        public void DoQuit()
        {
            messagePumper.Abort();
            messagePumper = null;
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
                    GoWindowed();
                else
                    GoFullscreen();
            }
            else
                activeComponent.OnKeyPressed(sender, e);
        }

        public void GoFullscreen()
        {
            recreateWindowMutex.WaitOne();
            if (windowStyle != Styles.Fullscreen)
            {
                windowStyle = Styles.Fullscreen;
                recreateWindow = true;

                OurGameToControllerPipe.SendMessage(GameToControllerWindowMessage.GoneFullscreen);
            }
            recreateWindowMutex.ReleaseMutex();
        }

        public void GoWindowed()
        {
            recreateWindowMutex.WaitOne();
            if (windowStyle == Styles.Fullscreen)
            {
                windowStyle = Styles.None;
                recreateWindow = true;

                OurGameToControllerPipe.SendMessage(GameToControllerWindowMessage.GoneWindowed);
            }
            recreateWindowMutex.ReleaseMutex();
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

        public void ChangeComponent( Component comp, bool initialize = true )
        {
            mustChangeComponent = true;
            newComponent = comp;

            if (initialize)
            {
                comp.Initialize();
                comp.LoadContent();
            }
        }

        void DoChangeComponent()
        {
            if( activeComponent != null )
                activeComponent.Desactivate();

            activeComponent = newComponent;

            if (activeComponent != null)
                activeComponent.Activate();

            mustChangeComponent = false;
        }

        bool Running()
        {
            bool runCopy = true;
            runMutex.WaitOne();
            runCopy = run;
            runMutex.ReleaseMutex();
            return runCopy;
        }

        public void Run()
        {
            messagePumper.Start();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            while (Running() && window.IsOpen())
            {
                window.DispatchEvents();

                recreateWindowMutex.WaitOne();
                if (recreateWindow)
                {
                    RecreateWindow();
                    recreateWindow = false;
                }
                recreateWindowMutex.ReleaseMutex();

                window.Clear(Color.Green);

                watch.Stop();
                activeComponent.Update(watch);
                activeComponent.Draw(watch);

                watch.Reset();
                watch.Start();

                window.Display();

                if (mustChangeComponent)
                    DoChangeComponent();
            }

            // The game application ended
            // we must notify the controller
            if ( Running() )
            {
                OurGameToControllerPipe.SendMessage(GameToControllerWindowMessage.ApplicationQuit);
                DoQuit();
            }
        }
    }
}
