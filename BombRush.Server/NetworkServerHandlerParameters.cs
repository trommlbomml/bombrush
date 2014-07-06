using System;
using BombRush.Networking;
using Lidgren.Network;

namespace BombRush.Server
{
    internal class NetworkServerHandlerParameters
    {
        public int Port;
        public Func<bool> ApproveConnection;
        public Func<NetIncomingMessage, byte> HandleClientJoined;
        public Action<NetIncomingMessage> HandleClientLeft; 
        public Action<Message> HandleDataMessageReceived;
    }
}