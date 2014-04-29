using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gtk;
using Gtk.DotNet;
using NOubliezPas.Communication;

using NOubliezPas.Controllers;

namespace NOubliezPas
{
    class GUILauncher
    {
        GameToControllerWindowMessagePipe OurGameToControllerPipe = null;
        ControllerToGameMessagePipe OurControllerToGamePipe = null;
        public GameApplication OurGameApp = null;

        Thread threadRunner = null;
        Thread messagePumper = null;

        Window myWin = null;
        VBox vBox = null;

        FullscreeModeController myFullscreenModeController = null;
        SongTestController mySongTestController = null;

        public GUILauncher(
            GameToControllerWindowMessagePipe gameToControllerPipe,
            ControllerToGameMessagePipe controllerToGamePipe,
            GameApplication app )
        {
            OurGameToControllerPipe = gameToControllerPipe;
            OurControllerToGamePipe = controllerToGamePipe;
            OurGameApp = app;

            threadRunner = new Thread(this.DoWork);
            messagePumper = new Thread(this.PumpMessages);
        }

        /// <summary>
        /// Pump the messages sent by the game application
        /// </summary>
        public void PumpMessages()
        {
            while(true)
            {
                GameToControllerWindowMessage msg = OurGameToControllerPipe.GetMessage();

                ClientEventArgs args = new ClientEventArgs();
                object[] rargs = new object[1];
                rargs[0] = msg;
                args.Args = rargs;

                if (msg == GameToControllerWindowMessage.ApplicationQuit)
                    OnReceiveQuitMessage();
                else if (msg == GameToControllerWindowMessage.GoneFullscreen)
                    myFullscreenModeController.OnClientEvent(this, args);
                else if (msg == GameToControllerWindowMessage.GoneWindowed)
                    myFullscreenModeController.OnClientEvent(this, args);
                else
                    mySongTestController.OnClientEvent(this, args);
            }
        }

        public void SendMessage( ControllerToGameMessage msg )
        {
            OurControllerToGamePipe.SendMessage(msg);
        }

        /// <summary>
        /// Runs in a separate thread.
        /// </summary>
        public void DoWork()
        {
            Application.Init();

            //Create the Window
            myWin = new Window("N'Oubliez pas les paroles  - Fenêtre de contrôle");
            myWin.DeleteEvent += this.OnWindowDeleteEvent;

            vBox = new VBox();
            myWin.Add(vBox);

            //myFullscreenModeController = new FullscreeModeController(this);
            //vBox.Add(myFullscreenModeController.GetPaneBox());

            mySongTestController = new SongTestController(this);
            vBox.Add(mySongTestController.GetPaneBox());

            // Rend tout visible
            myWin.ShowAll();

            // start the message pump
            messagePumper.Start();

            Application.Run();
        }

        /// <summary>
        /// The game has requested the app to close.
        /// </summary>
        public void OnReceiveQuitMessage()
        {
            DoQuit();
        }

        public void DoQuit()
        {
            Application.Quit();
            messagePumper.Abort();
        }

        public void OnWindowDeleteEvent(object o, DeleteEventArgs e)
        {
            OurControllerToGamePipe.SendMessage(ControllerToGameMessage.ApplicationQuit);
            DoQuit();
        }

        public void Run()
        {
            threadRunner.Start();
        }

        public void Join()
        {
            threadRunner.Join();
            messagePumper.Join();
        }
    }
}
