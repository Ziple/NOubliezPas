using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using NOubliezPas.Communication;

namespace NOubliezPas
{
    interface Component
    {
        void OnKeyPressed(object sender, EventArgs e);

        void Initialize();

        void LoadContent();

        void Update( Stopwatch difftime );

        void Draw( Stopwatch difftime );

        void ReadMessage( ControllerToGameMessage msg);

        void Activate();

        void Desactivate();
    }
}
