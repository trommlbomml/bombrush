using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using BombRush.Interfaces;
using BombRush.Logic;
using BombRush.Networking;

namespace BombRush.Server.Sessions
{
    class Session
    {
        private readonly object _sessionAccessLockObject = new object();
        private readonly List<Message> _messagesToSend = new List<Message>(); 
        private readonly List<GameClient> _clients;
        private Thread _executionThread;
        private GameSessionImp _gameSession;

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
            if (!IsActive) throw new InvalidOperationException("Session must be started to join clients");

            lock (_sessionAccessLockObject)
            {
                _clients.Add(client);    
                _gameSession.AddMember(MemberType.ActivePlayer, client.Name);
            }
        }

        public void Activate(GameSessionStartParameters parameters)
        {
            lock (_sessionAccessLockObject)
            {
                if (IsActive) return;
                _gameSession = new GameSessionImp(parameters);
                IsActive = true;
            }

            _executionThread = new Thread(OnUpdateSession);
            _executionThread.Start();
        }

        public void Deactivate()
        {
            lock (_sessionAccessLockObject)
            {
                IsActive = false;
                _gameSession = null;
            }
        }

        private void OnUpdateSession(object state)
        {
            while (IsActive)
            {
                _gameSession.Update(0.1f);
                Thread.Sleep(1);
            }
        }

        public void HandleClientLeft(GameClient client)
        {
            lock (_sessionAccessLockObject)
            {
                _clients.RemoveAll(c => c.Id == client.Id);
                if (_clients.Count == 0)
                {
                    Deactivate();
                }
            }
        }

        public SessionUpdateData GetAndClearMessages()
        {
            SessionUpdateData data;
            lock (_sessionAccessLockObject)
            {
                data = new SessionUpdateData
                {
                    MessagesToSend = _messagesToSend.ToArray(),
                    ReceiversIds = _clients.Select(c => c.Id).ToArray()
                };
                _messagesToSend.Clear();
            }

            return data;
        }
    }
}
