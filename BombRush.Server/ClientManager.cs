using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Lidgren.Network;

namespace BombRush.Server
{
    class ClientManager
    {
        private readonly HashSet<byte> _availableIds;
        private readonly int _maxClientCount;
        private readonly List<GameClient> _connectedClients;

        public ClientManager(int maxClientCount)
        {
            _availableIds = new HashSet<byte>(Enumerable.Range(1, 255).Select(e => (byte)e));
            _maxClientCount = maxClientCount;
            _connectedClients = new List<GameClient>();
        }

        public bool ClientsCanJoin {get { return _connectedClients.Count < _maxClientCount; }}

        public GameClient AddClient(NetIncomingMessage msg)
        {
            var playerName = msg.SenderConnection.RemoteHailMessage.ReadString();
            var client = new GameClient
            {
                EndPoint = msg.SenderEndPoint,
                NetConnection = msg.SenderConnection,
                Name = playerName,
                Id = GetNextId()
            };
            _connectedClients.Add(client);

            return client;
        }

        public GameClient RemoveClient(IPEndPoint endPoint)
        {
            var clientToRemove = _connectedClients.FirstOrDefault(p => p.EndPoint.Equals(endPoint));
            if(clientToRemove == null) return null;

            _connectedClients.Remove(clientToRemove);
            FreeId(clientToRemove.Id);

            return clientToRemove;
        }

        public GameClient GetClientById(byte id)
        {
            return _connectedClients.FirstOrDefault(c => c.Id == id);
        }

        private void FreeId(byte id)
        {
            _availableIds.Add(id);
        }

        private byte GetNextId()
        {
            var nextId = _availableIds.First();
            _availableIds.Remove(nextId);
            return nextId;
        }
    }
}
