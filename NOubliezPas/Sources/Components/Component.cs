using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace NOubliezPas
{
    interface Component
    {
        void OnKeyPressed(object sender, EventArgs e);

        void Initialize();

        void LoadContent();

        void Update( Stopwatch difftime );

        void Draw( Stopwatch difftime );
    }
}
