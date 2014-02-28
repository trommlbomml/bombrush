using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using System.Threading;
using BombRush.Networking;

namespace BombRush.Server
{
    class MasterServer
    {
        public const string ApplicationNetworkIdentifier = "BombRushNetworkGameIdentifier";

        private readonly object _networkUpdateLockObject = new object();
        private List<SessionClient> _allClients;
        private NetServer _server;
        private List<ServerSession> _allSessions = new List<ServerSession>();
        private Queue<Message> _messagesToSend;
        private byte _currentNewId;
                
        private LogListener Tracer { get; set; }

        public MasterServer(MasterServerConfiguration masterServerConfig)
        {
            _allClients = new List<SessionClient>();
            _messagesToSend = new Queue<Message>();
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

        public void PutMessageToSend(Message message)
        {
            lock (_messagesToSend)
            {
                _messagesToSend.Enqueue(message);
            }
        }

        private void StartNetServer(MasterServerConfiguration masterServerConfig)
        {
            var configuration = new NetPeerConfiguration(ApplicationNetworkIdentifier);
            configuration.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            configuration.Port = masterServerConfig.Port;

            _server = new NetServer(configuration);
            Tracer.Print(string.Format("Created server with {0} games in {1} Threads.", _allSessions.Count, masterServerConfig.Threads));
            Tracer.Print(string.Format("Port: {0}", masterServerConfig.Port));

             _server.RegisterReceivedCallback(s =>
            {
                lock (_networkUpdateLockObject)
                {
                    var timeStamp = NetTime.Now;
                    //_server.HandleNetMessages(timeStamp, HandleDataMessages, HandleStatusChangedServer, SendDiscoveryResponse);
                }
            });

            _server.Start();
            Tracer.Print("Server started successfully.");
        }

        public void Update()
        {
            Message message;
            do
            {
                lock (_messagesToSend)
                {
                    message = _messagesToSend.Count > 0 ? _messagesToSend.Dequeue() : null;
                }
                SendMessage(message);
            } 
            while (message != null);
            Thread.Sleep(0);
        }

        private void SendMessage(Message message)
        {
            lock (_networkUpdateLockObject)
            {
                //todo: nachrichten an Clients senden.
            }
        }

        internal byte GenerateNewId()
        {
            return _currentNewId++;
        }
    }
}
