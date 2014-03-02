using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BombRush.Interfaces;
using BombRush.Networking;
using BombRush.Networking.Extensions;
using Game2DFramework;
using Lidgren.Network;

namespace BombRush.Network
{
    class RemoteGameCreationSession : GameObject, GameCreationSession
    {
        private const string ApplicationNetworkIdentifier = "BombRushNetworkGameIdentifier";

        private NetClient _netClient;
        private readonly MessageTypeMap _messageTypeMap;
        private readonly List<GameInstance> _gameInstances;

        public RemoteGameCreationSession(Game2D game) : base(game)
        {
            State = GameCreationSessionState.Disconnected;
            _messageTypeMap = new MessageTypeMap();

            _gameInstances = new List<GameInstance>();
            RunningGameInstances = _gameInstances.AsReadOnly();
        }

        public GameCreationSessionState State { get; private set; }
        public ReadOnlyCollection<GameInstance> RunningGameInstances { get; private set; }

        public void ConnectToServer(string host)
        {
            State = GameCreationSessionState.ConnectingToServer;

            var configuration = new NetPeerConfiguration(ApplicationNetworkIdentifier);
            configuration.Port = Properties.Settings.Default.MultiplayerPort;

//#if !DEBUG
            configuration.ConnectionTimeout = 5.0f;
//#endif
            _netClient = new NetClient(configuration);
            _netClient.RegisterReceivedCallback(Callback);

            Task.Factory.StartNew(() =>
            {
                _netClient.Start();
                _netClient.Connect(host, Properties.Settings.Default.MultiplayerPort);
            });
        }

        public bool IsBusy
        {
            get
            {
                return State == GameCreationSessionState.ConnectingToServer;
            }
        }

        private void Callback(object state)
        {
            _netClient.HandleNetMessages(0, HandleDataMessage, HandleStatusChanged);
        }

        private void HandleStatusChanged(double d, NetConnectionStatus netConnectionStatus, string arg3)
        {
            if (netConnectionStatus == NetConnectionStatus.Disconnected)
            {
                State = GameCreationSessionState.ConnectionToServerFailed;
            }
        }

        public GameSession CreateGameInstance(string gameName, string playerName)
        {
            if (State != GameCreationSessionState.Connected) throw new InvalidOperationException("Session must be connected to Create Game Instance");

            throw new NotImplementedException();
        }

        public GameSession JoinGameInstance(GameInstance instance, string playerName)
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

            _netClient.UnregisterReceivedCallback(Callback);
            return new GameSessionClient(Game, _netClient);
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
