using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BombRush.Networking.Extensions;
using Lidgren.Network;
using System.Threading;

namespace BombRush.Server
{
    class GameClient
    {
        public string Name;
        public byte Id;
        public ServerSession Session;
        public IPEndPoint EndPoint;
    }

    class MasterServer
    {
        public const string ApplicationNetworkIdentifier = "BombRushNetworkGameIdentifier";

        private readonly HashSet<byte> _availableIds; 
        private readonly int _maxClientCount;
        private readonly object _networkUpdateLockObject = new object();
        private NetServer _server;
        private List<ServerSession> _allSessions = new List<ServerSession>();
        private readonly List<GameClient> _connectedClients;
                
        private LogListener Tracer { get; set; }

        public MasterServer(MasterServerConfiguration masterServerConfig)
        {
            _availableIds = new HashSet<byte>(Enumerable.Range(1,255).Select(e => (byte)e));
            _maxClientCount = masterServerConfig.MaxGameSessions*4;
            _connectedClients = new List<GameClient>();

            Tracer = masterServerConfig.LogListener;

            CreateSessions(masterServerConfig);
            StartNetServer(masterServerConfig);
        }

        private void CreateSessions(MasterServerConfiguration masterServerConfig)
        {
            _allSessions = new List<ServerSession>(masterServerConfig.MaxGameSessions);
            for (var i = 0; i < masterServerConfig.MaxGameSessions; i++)
            {
                _allSessions.Add(new ServerSession(this));
            }

            ThreadPool.QueueUserWorkItem(OnSessionUpdate);
        }

        private void OnSessionUpdate(object state)
        {
            foreach (var serverSession in _allSessions.Where(s => s.IsActive))
            {
                serverSession.Update(0);
            }
        }

        private void StartNetServer(MasterServerConfiguration masterServerConfig)
        {
            var configuration = new NetPeerConfiguration(ApplicationNetworkIdentifier);
            configuration.Port = masterServerConfig.Port;
            configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            _server = new NetServer(configuration);
            Tracer.PrintInfo(string.Format("Created server with {0} games in {1} Threads.", _allSessions.Count, masterServerConfig.Threads));
            Tracer.PrintInfo(string.Format("Port: {0}", masterServerConfig.Port));

             _server.RegisterReceivedCallback(OnClientMessageReceived);
            _server.Start();
            Tracer.PrintInfo("Server started successfully.");
        }

        private void OnClientMessageReceived(object state)
        {
            lock (_networkUpdateLockObject)
            {
                _server.HandleNetMessages(NetTime.Now, HandleDataMessage, HandleStatusChangedMessage, HandleConnectionApproval);
            }
        }

        private void HandleConnectionApproval(double d, NetIncomingMessage netIncomingMessage)
        {
            //todo: maximale Id-Grenze.
            if (_connectedClients.Count < _maxClientCount && _connectedClients.Count < 255)
            {
                netIncomingMessage.SenderConnection.Deny("Server full.");
                //netIncomingMessage.SenderConnection.Approve();
            }
            else
            {
                netIncomingMessage.SenderConnection.Deny("Server full.");
            }
        }

        private void HandleClientJoined(NetIncomingMessage msg)
        {
            var clientExist = _connectedClients.Any(p => p.EndPoint.Equals(msg.SenderEndPoint));

            if (clientExist) Tracer.PrintWarning(string.Format("Client {0}, is already registered on server", msg.SenderEndPoint));

            if (!clientExist && msg.SenderConnection.RemoteHailMessage != null)
            {
                var playerName = msg.SenderConnection.RemoteHailMessage.ReadString();
                var client = new GameClient
                {
                    EndPoint = msg.SenderEndPoint,
                    Name = playerName,
                    Id = GetNextId()
                };
                _connectedClients.Add(client);

                Tracer.PrintInfo(string.Format("Client {0} ({1}) joined the server.",playerName, msg.SenderEndPoint));
            }
        }

        private void HandleClientLeft(NetIncomingMessage netIncomingMessage)
        {
            var clientToRemove = _connectedClients.FirstOrDefault(p => p.EndPoint.Equals(netIncomingMessage.SenderEndPoint));
            if (clientToRemove == null)
            {
                Tracer.PrintWarning("Try to remove Client " + netIncomingMessage.SenderEndPoint + " but is not registered.");
            }
            else
            {
                Tracer.PrintInfo("Client" + netIncomingMessage.SenderEndPoint + " left server.");
                _connectedClients.Remove(clientToRemove);
            }
        }

        private void HandleStatusChangedMessage(double d, NetIncomingMessage netIncomingMessage)
        {
            var connectionStatus = (NetConnectionStatus)netIncomingMessage.ReadByte();

            if (connectionStatus == NetConnectionStatus.Connected)
            {
                HandleClientJoined(netIncomingMessage);
            }
            else if (connectionStatus == NetConnectionStatus.Disconnected)
            {
                HandleClientLeft(netIncomingMessage);
            }
        }

        private void HandleDataMessage(double d, NetIncomingMessage netIncomingMessage)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            //Message message;
            //do
            //{
            //    lock (_messagesToSend)
            //    {
            //        message = _messagesToSend.Count > 0 ? _messagesToSend.Dequeue() : null;
            //    }
            //    SendMessage(message);
            //} 
            //while (message != null);
            Thread.Sleep(0);
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
