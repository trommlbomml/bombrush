
using System.Net;

namespace BombRush.Network
{
    class ServerConnection
    {
        public string ServerName;
        public int PlayerCount;
        public IPEndPoint EndPoint;
        public double TimeStamp;
        public bool Running;

        public ServerConnection(string serverName, int playerCount, IPEndPoint endPoint, double timeStamp, bool running)
        {
            ServerName = serverName;
            PlayerCount = playerCount;
            EndPoint = endPoint;
            TimeStamp = timeStamp;
            Running = running;
        }

        public void Update(string serverName, int playerCount, double timeStamp, bool running)
        {
            ServerName = serverName;
            PlayerCount = playerCount;
            TimeStamp = timeStamp;
            Running = running;
        }
    }
}
