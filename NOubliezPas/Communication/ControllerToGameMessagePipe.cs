using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace NOubliezPas.Communication
{
    enum ControllerToGameMessage
    {
        ApplicationQuit,
        GoFullscreen,
        GoWindowed,
        NoMessage
    }

    class ControllerToGameMessagePipe
    {
        ControllerToGameMessage message;
        Semaphore rSemaphore = new Semaphore(0, 1);
        Semaphore wSemaphore = new Semaphore(1, 1);

        public ControllerToGameMessage GetMessage()
        {
            ControllerToGameMessage msg = ControllerToGameMessage.NoMessage;

            rSemaphore.WaitOne();
            msg = message;
            wSemaphore.Release();
            return msg;
        }

        public void SendMessage( ControllerToGameMessage msg )
        {
            wSemaphore.WaitOne();
            message = msg;
            rSemaphore.Release();
        }
    }
}
