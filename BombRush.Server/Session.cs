
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using BombRush.Networking;

namespace BombRush.Server
{
    class Session
    {
        private readonly List<Message> _messagesToSend = new List<Message>(); 
        private readonly List<GameClient> _clients;
        private Thread _executionThread;

        public byte Id { get; private set; }
        public bool IsActive { get; private set; }
        public ReadOnlyCollection<GameClient> Clients { get; private set; }  

        public Session(byte id)
        {
            Id = id;
            _clients = new List<GameClient>();
            Clients = _clients.AsReadOnly();
            IsActive = false;
        }

        public void JoinClient(GameClient client)
        {
            _clients.Add(client);
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;

            _executionThread = new Thread(OnUpdateSession);
            _executionThread.Start();
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        private void OnUpdateSession(object state)
        {
            while (IsActive)
            {
                Thread.Sleep(1);
            }
        }

        public void HandleClientLeft(GameClient client)
        {
            _clients.RemoveAll(c => c.Id == client.Id);
            if (_clients.Count == 0)
            {
                Deactivate();
            }
        }

        public IList<Message> GetAndClearMessages()
        {
            var list = new List<Message>(_messagesToSend);
            _messagesToSend.Clear();
            return list;
        }
    }
}
