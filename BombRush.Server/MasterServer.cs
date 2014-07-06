
using BombRush.Logic;
using BombRush.Networking;
using BombRush.Networking.ClientMessages;
using BombRush.Server.Sessions;
using Lidgren.Network;
using System.Threading;

namespace BombRush.Server
{
    class MasterServer
    {
        private readonly NetworkServerHandler _networkServerHandler;
        private readonly SessionPool _sessionPool;
        private readonly ClientManager _clientManager;
        private LogListener Tracer { get; set; }

        public MasterServer(MasterServerConfiguration masterServerConfig)
        {
            Tracer = masterServerConfig.LogListener;

            _clientManager = new ClientManager(masterServerConfig.MaxGameSessions * 4);
            _sessionPool = new SessionPool(masterServerConfig.MaxGameSessions);

            Tracer.PrintInfo(string.Format("Starting NetServer at Port: {0}", masterServerConfig.Port));
            var parameters = new NetworkServerHandlerParameters
            {
                ApproveConnection = ApproveConnection,
                HandleClientJoined = HandleClientJoined,
                HandleClientLeft = HandleClientLeft,
                HandleDataMessageReceived = HandleDataMessageReceived,
                Port = masterServerConfig.Port
            };
            _networkServerHandler = new NetworkServerHandler(parameters);
            Tracer.PrintInfo("Server started successfully.");
        }

        private void HandleDataMessageReceived(Message message)
        {
            if (message is ClientCreateGameSessionMessage)
            {
                HandleClientCreateGame((ClientCreateGameSessionMessage)message);
            }
            else if (message is ClientJoinGameSessionMessage)
            {
                HandleClientJoinGame((ClientJoinGameSessionMessage)message);
            }
        }

        private byte HandleClientJoined(NetIncomingMessage msg)
        {
            var client = _clientManager.AddClient(msg);
            Tracer.PrintInfo(string.Format("Client {0}, id={1} name='{2}' joined the server", client.EndPoint, client.Id, client.Name));
            return client.Id;
        }

        private bool ApproveConnection()
        {
            return _clientManager.ClientsCanJoin;
        }

        private void HandleClientLeft(NetIncomingMessage netIncomingMessage)
        {
            var clientToRemove = _clientManager.RemoveClient(netIncomingMessage.SenderEndPoint);

            if (clientToRemove == null)
            {
                Tracer.PrintWarning(string.Format("Try to remove Client {0} which is not registired.", netIncomingMessage.SenderEndPoint));
            }
            else
            {
                _sessionPool.HandleClientLeft(clientToRemove);
                Tracer.PrintInfo(string.Format("Client {0}, id={1} name='{2}' left the server", clientToRemove.EndPoint, clientToRemove.Id, clientToRemove.Name));
            }
        }

        private void HandleClientJoinGame(ClientJoinGameSessionMessage message)
        {
            var clientToConnectSession = _clientManager.GetClientById(message.ClientId);
            if (clientToConnectSession == null) return;

            if (!_sessionPool.JoinSession(clientToConnectSession, message.SessionId))
            {
                Tracer.PrintWarning(string.Format("Could not connect client {0} to Session {1}", message.ClientId, message.SessionId));
            }
        }

        private void HandleClientCreateGame(ClientCreateGameSessionMessage message)
        {
            var clientToCreateSession = _clientManager.GetClientById(message.ClientId);
            if (clientToCreateSession == null) return;

            var startParameters = new GameSessionStartParameters
            {
                MatchTime = message.MatchTime,
                MatchesToWin = message.MatchesToWin,
                SessionName = message.GameName,
                LevelAssetPath = "levels/Rookie.xml"
            };
            
            if (!_sessionPool.ActivateSession(clientToCreateSession, startParameters))
            {
                Tracer.PrintWarning(string.Format("Could not create Session for client {0}.", message.ClientId));
            }
        }

        public void Update()
        {
            while (true)
            {
                foreach (var serverSession in _sessionPool.ActiveSessions)
                {
                    var data = serverSession.GetAndClearMessages();
                    var receiverConnections = _clientManager.GetClientNetConnectionsById(data.ReceiversIds);
                    _networkServerHandler.SendSessionMessages(data.MessagesToSend, receiverConnections);
                }
                Thread.Sleep(1);
            }
        }
    }
}
