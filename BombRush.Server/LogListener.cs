using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BombRush.Server
{
    interface LogListener
    {
        void PrintInfo(string text);
        void PrintWarning(string text);
    }
}
