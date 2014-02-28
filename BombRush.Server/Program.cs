using System;
using System.Threading;

namespace BombRush.Server
{
    class Program
    {
        private static MasterServerConfiguration CreateParametersFromCommandLine(string[] args)
        {
            var configuration = new MasterServerConfiguration
            {
                MaxGameSessions = 10,
                Port = 11175,
                LogListener = new ConsoleOutput(),
                Threads = 1,
            };

            return configuration;
        }

        static void Main(string[] args)
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var configuration = CreateParametersFromCommandLine(args);
            var masterServer = new MasterServer(configuration);

            var running = true;
            Console.CancelKeyPress += (s, e) => running = false;

            while (running)
            {
                try
                {
                    masterServer.Update();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occurred: {0}", ex);
                }
            }
        }
    }
}
