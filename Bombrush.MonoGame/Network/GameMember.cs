using BombRush.Interfaces;
using BombRush.Logic;
using BombRushData.Network;
using Microsoft.Xna.Framework;

namespace BombRush.Network
{
    class GameMember : Figure
    {
        private Vector2 _serverSendPosition;
        private Vector2 _serverSendWalkDirection;
        private FigureDirection _serverSendDirection;
        private double _lastServerTimeStamp;

        public GameMember(string name, byte id, bool rdy)
        {
            Name = name;
            Ready = rdy;
            Id = id;
            Me = false;
            Wins = 0;
            IsMatchWinner = false;
            IsAlive = true;
            ShowPlayerName = true;
        }

        public void UpdateFromSnapShot(FigureInformation figureInformation, double timeStamp)
        {
            _lastServerTimeStamp = timeStamp;
            _serverSendPosition = figureInformation.Position;
            _serverSendDirection = figureInformation.Direction;
            _serverSendWalkDirection = figureInformation.WalkDirection;
            Speed = figureInformation.Speed;
            IsAlive = figureInformation.IsAlive;
            IsVisible = figureInformation.IsVisible;
        }

        public void Update(ILevelDataProvider level, float elapsed, double localTimeStamp, double remoteTimeStamp)
        {
            Direction = _serverSendDirection;

            float distanceCanTravel = elapsed*Speed;
            if (_serverSendWalkDirection == Vector2.Zero)
            {
                Interpolate(_serverSendPosition, distanceCanTravel);
            }
            else if (_serverSendWalkDirection == WalkDirection)
            {
                Vector2 moveStep =  _serverSendWalkDirection*(float) (remoteTimeStamp - _lastServerTimeStamp)*Speed;
                Vector2 currentPredictedPosition = LevelHelper.HandleCollisionSlide(level, _serverSendPosition, moveStep);
                Interpolate(currentPredictedPosition, distanceCanTravel);
            }
            else
            {
                Vector2 moveStep = _serverSendWalkDirection * (float)(remoteTimeStamp - _lastServerTimeStamp) * Speed;
                Vector2 currentPredictedPosition = LevelHelper.HandleCollisionSlide(level, _serverSendPosition, moveStep);
                Interpolate(currentPredictedPosition, distanceCanTravel);
            }
            WalkDirection = _serverSendWalkDirection;
        }

        private void Interpolate(Vector2 targetPosition, float distanceCanTravel)
        {
            Vector2 distanceVector = targetPosition - Position;
            float distanceToTargetSquard = distanceVector.LengthSquared();

            if (distanceToTargetSquard <= distanceCanTravel * distanceCanTravel
                || distanceToTargetSquard > distanceCanTravel * distanceCanTravel * 3)
            {
                Position = targetPosition;
            }
            else
            {
                Position += Vector2.Normalize(distanceVector)*distanceCanTravel;
            }
        }

        public void InitFromServer(Vector2 position)
        {
            _serverSendPosition = position;
            Position = position;
            Direction = FigureDirection.Down;
        }

        public bool Ready { get; set; }
        public bool Me { get; set; }
        public bool IsVisible { get; set; }
        public bool IsMatchWinner { get; set; }
        public bool ShowPlayerName { get; set; }
        public FigureDirection Direction { get; set; }
        public Vector2 WalkDirection { get; set; }
        public Vector2 Position { get; set; }
        public string Name { get; set; }
        public byte Id { get; set; }
        public int Wins { get; set; }
        public float Speed { get; set; }
        public bool IsAlive { get; set; }
    }
}
