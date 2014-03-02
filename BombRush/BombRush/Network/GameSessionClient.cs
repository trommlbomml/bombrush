
using BombRush.Controller;
using BombRush.Interfaces;
using Game2DFramework;
using Lidgren.Network;

namespace BombRush.Network
{
    class GameSessionClient : GameObject, GameSession
    {
        private NetClient _netClient;
        private double _serverTimeStamp;
        private FigureController _figureController;
        
        public GameSessionClient(Game2D game, NetClient client) : base(game)
        {
            _netClient = client;
            _figureController = PlayerController.CreateNet(game, InputDeviceType.Keyboard);
        }

        public byte Id { get; private set; }
        public string SessionName { get; private set; }
        public float RemainingStartupTime { get; private set; }
        public Level CurrentLevel { get; private set; }
        public MatchResultType CurrentMatchResultType { get; private set; }
        public GameSessionState State { get; private set; }
        public GameSessionMember[] Members { get; private set; }

        public GameUpdateResult Update(float elapsedTime)
        {
            if (_netClient != null
                && _netClient.ServerConnection == null
                && State != GameSessionState.Disconnected)
            {
                OnQuit();
                return GameUpdateResult.ServerShutdown;
            }

            var timeStamp = NetTime.Now;
            _serverTimeStamp = _netClient != null && _netClient.ServerConnection != null
                                      ? _netClient.ServerConnection.GetRemoteTime(timeStamp)
                                      : NetTime.Now;

            return GameUpdateResult.None;

            //_netClient.HandleNetMessages(timeStamp, HandleDataMessages, HandleDiscoveryResponse);

            //switch (State)
            //{
            //    case InGameState.Disconnected:
            //        HandleOnDisconnectedMode(timeStamp, elapsed);
            //        break;
            //    case InGameState.InGame:
            //        HandleInGame(timeStamp, elapsed);
            //        break;
            //}

        }

        public void OnQuit()
        {
            if (_netClient != null) _netClient.Shutdown("Regular Quit");

            State = GameSessionState.Disconnected;
        }

        public void StartMatch()
        {
            //var message = new ClientReadyMessage(NetTime.Now, ClientId, true, false);
            //message.Send(_messageTypeMap, _netClient.ServerConnection);
            //State = InGameState.PreparingMatchWaitForAllReady;
        }
    }
}
