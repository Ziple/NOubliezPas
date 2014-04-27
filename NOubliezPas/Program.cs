using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NOubliezPas.Communication;

namespace NOubliezPas
{
    class Program
    {
        static void Main(string[] args)
        {
            GameToControllerWindowMessagePipe pipeGameToController = new GameToControllerWindowMessagePipe();
            ControllerToGameMessagePipe pipeControllerToGame = new ControllerToGameMessagePipe();

            GameApplication app = new GameApplication(pipeGameToController, pipeControllerToGame);

            GUILauncher guiLauncher = new GUILauncher( pipeGameToController, pipeControllerToGame, app );
            guiLauncher.Run();

            app.Run();

            guiLauncher.Join();
        }
    }
}
