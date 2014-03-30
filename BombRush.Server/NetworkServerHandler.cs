using System;
using System.Collections.Generic;
using System.Linq;
using BombRush.Networking;
using BombRush.Networking.Extensions;
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

    class NetworkServerHandler
    {
        public const string ApplicationNetworkIdentifier = "BombRushNetworkGameIdentifier";

        private readonly MessageTypeMap _messageTypeMap = new MessageTypeMap();
        private readonly object _networkUpdateLockObject = new object();
        private NetServer _server;
        private Func<bool> _approveConnection;
        private Func<NetIncomingMessage, byte> _handleClientJoined;
        private Action<Message> _handleDataMessageReceived;
        private Action<NetIncomingMessage> _handleClientLeft;
        private LogListener Tracer { get; set; }

        public NetworkServerHandler(LogListener tracer, NetworkServerHandlerParameters parameters)
        {
            Tracer = tracer;
            _approveConnection = parameters.ApproveConnection;
            _handleClientJoined = parameters.HandleClientJoined;
            _handleDataMessageReceived = parameters.HandleDataMessageReceived;
            _handleClientLeft = parameters.HandleClientLeft;

            var configuration = new NetPeerConfiguration(ApplicationNetworkIdentifier)
            {
                Port = parameters.Port,
                ConnectionTimeout = 5.0f
            };

            configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            _server = new NetServer(configuration);
            _server.RegisterReceivedCallback(OnClientMessageReceived);
            _server.Start();
        }

        public void SendSessionMessages(Session session)
        {
            var messages = session.GetAndClearMessages();
            var receivers = session.Clients.Select(c => c.NetConnection).ToList();
            foreach (var message in messages)
            {
                message.Send(_messageTypeMap, _server, receivers);
            }
        }

        private void HandleDataMessage(double d, NetIncomingMessage netIncomingMessage)
        {
            var message = Message.Read(_messageTypeMap, netIncomingMessage);
            _handleDataMessageReceived(message);
        }

        private void OnClientMessageReceived(object state)
        {
            lock (_networkUpdateLockObject)
            {
                _server.HandleNetMessages(NetTime.Now, HandleDataMessage, HandleStatusChangedMessage, HandleConnectionApproval);
            }
        }

        private void HandleStatusChangedMessage(double d, NetIncomingMessage netIncomingMessage)
        {
            var connectionStatus = (NetConnectionStatus)netIncomingMessage.ReadByte();

            if (connectionStatus == NetConnectionStatus.Disconnected)
            {
                _handleClientLeft(netIncomingMessage);
            }
        }

        private void HandleConnectionApproval(double d, NetIncomingMessage netIncomingMessage)
        {
            if (_approveConnection())
            {
                var id = _handleClientJoined(netIncomingMessage);

                var msg = _server.CreateMessage();
                msg.Write(id);
                netIncomingMessage.SenderConnection.Approve(msg);
            }
            else
            {
                netIncomingMessage.SenderConnection.Deny("Server full.");
            }
        }
    }
}
