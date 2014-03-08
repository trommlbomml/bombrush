using System.Net;

namespace BombRush.Server
{
    class GameClient
    {
        public string Name;
        public byte Id;
        public Session Session;
        public IPEndPoint EndPoint;
        public bool IsSessionAdministrator;
    }
}