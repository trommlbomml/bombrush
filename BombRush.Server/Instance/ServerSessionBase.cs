using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BombRush.Networking;

namespace BombRush.Server
{
    abstract class ServerSessionBase
    {
        private Queue<Message> _incomingMessages;
        private Queue<Message> _outgoingMessages;

        public MasterServer MasterServer { get; private set; }
        public byte SessionId { get; private set; }
        public bool IsActive { get; protected set; }

        protected ServerSessionBase(MasterServer masterServer)
        {
            MasterServer = masterServer;
            SessionId = MasterServer.GenerateNewId();
            _incomingMessages = new Queue<Message>();
            _outgoingMessages = new Queue<Message>();
        }

        protected Message GetIncomingMessage(Message message)
        {
            lock (_incomingMessages)
            {
                return _incomingMessages.Count > 0 ? _incomingMessages.Dequeue() : null;
            }
        }

        public void InsertIncomingMessage(Message message)
        {
            lock (_incomingMessages)
            {
                _incomingMessages.Enqueue(message);
            }
        }

        protected void InsertOutgoingMessage(Message message)
        {
            MasterServer.PutMessageToSend(message);
        }

        public abstract void Activate();
        public abstract void Deactivate();
        public abstract void OnClientJoined(ushort clientId);
        public abstract void OnClientLeft(ushort clientId);
        public abstract void Update(float elapsedTime);
    }
}
