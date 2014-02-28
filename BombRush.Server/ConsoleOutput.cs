using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BombRush.Server
{
    class ConsoleOutput : LogListener
    {
        public void Print(string text)
        {
            Console.WriteLine(text);
        }
    }
}
