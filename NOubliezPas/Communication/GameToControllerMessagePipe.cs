using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NOubliezPas.Communication
{
    enum GameToControllerWindowMessage
    {
        GoneFullscreen,
        GoneWindowed,
        BlindTestEnter,
        BlindTestExit,
        FinalTestEnter,
        FinalTestExit,
        SongSelectionMenuEnter,
        SongSelectionMenuExit,
        SongTestEnter,
        SongTestExit,
        ThemeSelectionMenuEnter,
        ThemeSelectionMenuExit,
        ApplicationQuit,
        ApplicationWaitingAnswer,
        NoMessage
    }

    class GameToControllerWindowMessagePipe
    {
        GameToControllerWindowMessage message;
        Semaphore rSemaphore = new Semaphore(0, 1);
        Semaphore wSemaphore = new Semaphore(1, 1);

        public GameToControllerWindowMessage GetMessage()
        {
            GameToControllerWindowMessage msg = GameToControllerWindowMessage.NoMessage;

            rSemaphore.WaitOne();
            msg = message;
            wSemaphore.Release();
            return msg;
        }

        public void SendMessage( GameToControllerWindowMessage msg )
        {
            wSemaphore.WaitOne();
            message = msg;
            rSemaphore.Release();
        }
    }
}
