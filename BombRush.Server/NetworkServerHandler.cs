using System;
using System.Collections.Generic;
using System.Linq;
using BombRush.Networking;
using BombRush.Networking.Extensions;
using BombRush.Server.Sessions;
using Lidgren.Network;

namespace BombRush.Server
{
    class NetworkServerHandler
    {
        public const string ApplicationNetworkIdentifier = "BombRushNetworkGameIdentifier";

        private readonly MessageTypeMap _messageTypeMap = new MessageTypeMap();
        private readonly object _networkUpdateLockObject = new object();
        private readonly NetServer _server;
        private readonly Func<bool> _approveConnection;
        private readonly Func<NetIncomingMessage, byte> _handleClientJoined;
        private readonly Action<Message> _handleDataMessageReceived;
        private readonly Action<NetIncomingMessage> _handleClientLeft;

        public NetworkServerHandler(NetworkServerHandlerParameters parameters)
        {
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

        public void SendSessionMessages(Message[] messages, List<NetConnection> receivers)
        {
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
