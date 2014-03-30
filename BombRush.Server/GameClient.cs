using System.Net;
using Lidgren.Network;

namespace BombRush.Server
{
    class GameClient
    {
        public string Name;
        public byte Id;
        public Session Session;
        public IPEndPoint EndPoint;
        public bool IsSessionAdministrator;
        public NetConnection NetConnection;
    }
}