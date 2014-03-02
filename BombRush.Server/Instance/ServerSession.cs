using System;
using BombRush.Interfaces;
using BombRush.Logic;
using BombRush.Server.Instance;

namespace BombRush.Server
{
    class ServerSession : ServerSessionBase
    {
        private GameSessionImp _gameSession;

        public ServerSession(MasterServer masterServer) : base(masterServer)
        {
        }

        public override void Activate()
        {
            var startParameters = new GameSessionStartParameters
            {
                Id = SessionId,
                MatchTime = 500,
                ProvidePlayerFigureController = CreateFigureController
            };
            _gameSession = new GameSessionImp(startParameters);
            IsActive = true;
        }

        private FigureController CreateFigureController(int playerIndex)
        {
            return new RemoteController();
        }

        public override void Update(float elapsedTime)
        {
            if (!IsActive) return;
            _gameSession.Update(elapsedTime);
        }

        public override void Deactivate()
        {
            throw new NotImplementedException();
        }

        public override void OnClientJoined(ushort clientId)
        {
            throw new NotImplementedException();
        }

        public override void OnClientLeft(ushort clientId)
        {
            throw new NotImplementedException();
        }
    }
}
