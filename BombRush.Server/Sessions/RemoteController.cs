using BombRush.Interfaces;
using Microsoft.Xna.Framework;

namespace BombRush.Server.Sessions
{
    class RemoteController : FigureController
    {
        public Figure Figure { get; set; }
        public bool IsComputer { get { return false; } }
        public bool DirectionChanged { get; set; }
        public Vector2 MoveDirection { get; set; }
        public bool ActionDone { get; set; }
        public bool CancelDone { get; set; }

        public void Update(float elapsed)
        {
        }

        public void Reset()
        {
            ActionDone = false;
            DirectionChanged = false;
            CancelDone = false;
        }

        public void ResetInputs()
        {
            ActionDone = false;
            DirectionChanged = false;
            CancelDone = false;
            MoveDirection = Vector2.Zero;
        }
    }
}
