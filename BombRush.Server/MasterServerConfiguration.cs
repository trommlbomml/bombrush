using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BombRush.Server
{
    class MasterServerConfiguration
    {
        public byte MaxGameSessions { get; set; }
        public int Port { get; set; }
        public byte Threads { get; set; }
        public LogListener LogListener {get; set;}
    }
}
