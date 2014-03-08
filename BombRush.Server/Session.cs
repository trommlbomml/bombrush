
using System.Collections.Generic;

namespace BombRush.Server
{
    class Session
    {
        private readonly List<GameClient> _clients;

        public byte Id { get; private set; }

        public Session(byte id)
        {
            Id = id;
            _clients = new List<GameClient>();
        }

        public void JoinClient(GameClient client)
        {
            _clients.Add(client);
        }

        public void HandleClientLeft(GameClient client)
        {
            _clients.RemoveAll(c => c.Id == client.Id);
        }
    }
}
