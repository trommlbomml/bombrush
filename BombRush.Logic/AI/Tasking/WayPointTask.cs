
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BombRush.Logic.AI.Tasking
{
    class WayPointTask : ITask
    {
        private readonly PlaceNode _toReachNode;
        private PlaceNode _currentNodeToReach;
        private Queue<PlaceNode> _wayPoints;

        public WayPointTask(PlaceNode toReach)
        {
            _toReachNode = toReach;
        }

        public virtual void Start()
        {
            _wayPoints = null;
        }

        public bool Finished { get; protected set; }

        public void Update(float elapsed, ComFigureController controller)
        {
            if (Finished)
                return;

            var current = controller.GetTilePosition();

            if (_wayPoints == null)
            {
                _wayPoints = new Queue<PlaceNode>(controller.NodeMap.FindWayTo(current, _toReachNode));
                _currentNodeToReach = _wayPoints.Dequeue();
            }

            var toReachVector = controller.Level.GetWorldFromTilePositionCentered(_currentNodeToReach.TilePosition);
            var distance = toReachVector - controller.Figure.Position;
            var moveStep = controller.Figure.Speed*elapsed;

            if (current == _currentNodeToReach.TilePosition && distance.LengthSquared() < moveStep * moveStep)
            {
                ((FigureImp)controller.Figure).Position = toReachVector;
                controller.SetMoveDirectionTo(Vector2.Zero);

                if (_wayPoints.Count == 0)
                {
                    controller.SetMoveDirectionTo(Vector2.Zero);
                    Finished = true;
                }
                else
                    _currentNodeToReach = _wayPoints.Dequeue();   
            }

            if (_currentNodeToReach.TilePosition.X < current.X)
                controller.SetMoveDirectionTo(-Vector2.UnitX);
            else if (_currentNodeToReach.TilePosition.X > current.X)
                controller.SetMoveDirectionTo(Vector2.UnitX);
            else if (_currentNodeToReach.TilePosition.Y < current.Y)
                controller.SetMoveDirectionTo(-Vector2.UnitY);
            else if (_currentNodeToReach.TilePosition.Y > current.Y)
                controller.SetMoveDirectionTo(Vector2.UnitY);
            else if (distance.LengthSquared() > moveStep * moveStep)
                controller.SetMoveDirectionTo(Vector2.Normalize(distance));
        }
    }
}
