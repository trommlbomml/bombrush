using System;

namespace BombRush.Server
{
    class ConsoleOutput : LogListener
    {
        public void PrintInfo(string text)
        {
            Console.WriteLine("Info:    {0}", text);
        }

        public void PrintWarning(string text)
        {
            Console.WriteLine("Warning: {0}", text);
        }
    }
}
