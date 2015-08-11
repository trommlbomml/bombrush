using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BombRush.Interfaces;
using BombRush.Networking;
using BombRush.Networking.Extensions;
using BombRush.Networking.ServerMessages;
using Game2DFramework;
using Lidgren.Network;

namespace Bombrush.MonoGame.Network
{
    class RemoteGameCreationSession : GameObject
    {
        private const string ApplicationNetworkIdentifier = "BombRushNetworkGameIdentifier";

        private byte _clientId;
        private NetClient _netClient;
        private readonly MessageTypeMap _messageTypeMap;
        private readonly List<GameInstance> _gameInstances;
        private string _connectionFailedMessage;

        public RemoteGameCreationSession(Game2D game) : base(game)
        {
            State = GameCreationSessionState.Disconnected;
            _messageTypeMap = new MessageTypeMap();

            _gameInstances = new List<GameInstance>();
            RunningGameInstances = _gameInstances.AsReadOnly();
        }

        public GameCreationSessionState State { get; private set; }
        public ReadOnlyCollection<GameInstance> RunningGameInstances { get; private set; }

        public void ConnectToServer(string host, string playerName)
        {
            _connectionFailedMessage = string.Empty;
            State = GameCreationSessionState.ConnectingToServer;

            var configuration = new NetPeerConfiguration(ApplicationNetworkIdentifier);
            configuration.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            configuration.ConnectionTimeout = 5.0f;

            _netClient = new NetClient(configuration);
            _netClient.RegisterReceivedCallback(Callback);

            Task.Factory.StartNew(() =>
            {
                _netClient.Start();
                var hail = _netClient.CreateMessage();
                hail.Write(playerName);
                try
                {
                    //todo: Implement Datadriven
                    _netClient.Connect(host, 11175, hail);
                }
                catch (Exception)
                {
                    State = GameCreationSessionState.ConnectionToServerFailed;
                    _connectionFailedMessage = "Connection Failed. Check Hostname.";
                }
                
            });
        }

        public string GetConnectionFailedMessageAndReset()
        {
            if (State != GameCreationSessionState.ConnectionToServerFailed) throw new InvalidOperationException("State must be connection failed");

            State = GameCreationSessionState.Disconnected;
            return _connectionFailedMessage;
        }

        private void Callback(object state)
        {
            _netClient.HandleNetMessages(0, HandleDataMessage, HandleStatusChanged);
        }

        private void HandleStatusChanged(NetIncomingMessage inc, NetConnectionStatus netConnectionStatus, string reason)
        {
            if (netConnectionStatus == NetConnectionStatus.Disconnected)
            {
                State = GameCreationSessionState.ConnectionToServerFailed;
                _connectionFailedMessage = reason;
            }
            else if (netConnectionStatus == NetConnectionStatus.Connected)
            {
                _clientId = inc.SenderConnection.RemoteHailMessage.ReadByte();
                State = GameCreationSessionState.Connected;
            }
        }

        public GameSession CreateGameInstance(string gameName)
        {
            if (State != GameCreationSessionState.Connected) throw new InvalidOperationException("Session must be connected to Create Game Instance");

            throw new NotImplementedException();
        }

        public GameSession JoinGameInstance(GameInstance instance)
        {
            if (State != GameCreationSessionState.Connected) throw new InvalidOperationException("Session must be connected to join Game Instance");

            throw new NotImplementedException();
        }

        public void LeaveGameInstance()
        {
            throw new NotImplementedException();
        }

        public GameSessionClient CreateGameSessionClient()
        {
            if (State != GameCreationSessionState.Connected) throw new InvalidOperationException("Can Only Create Client Object when connection is established");
            if (_clientId == 0) throw new InvalidOperationException("Invalid Client Id");

            _netClient.RegisterReceivedCallback(null);
            return new GameSessionClient(Game, _netClient, _clientId);
        }

        private void HandleDataMessage(double d, NetIncomingMessage netIncomingMessage)
        {
            var message = Message.Read(_messageTypeMap, netIncomingMessage);

            var msg = message as GameCreationStatusMessage;
            if (msg != null) UpdateServerInstanceList(msg);
        }

        private void UpdateServerInstanceList(GameCreationStatusMessage msg)
        {
            _gameInstances.RemoveAll(g => msg.Instances.All(si => si.SessionId != g.SessionId));
            foreach (var serverInstance in msg.Instances)
            {
                var existingInstance = _gameInstances.FirstOrDefault(g => g.SessionId == serverInstance.SessionId);
                if (existingInstance == null)
                {
                    existingInstance = new GameInstance { SessionId = serverInstance.SessionId };
                    _gameInstances.Add(existingInstance);
                }

                existingInstance.Name = serverInstance.Name;
                existingInstance.PlayerCount = serverInstance.PlayerCount;
                existingInstance.IsRunning = serverInstance.IsRunning;
            }
        }
    }
}
