
using Microsoft.Xna.Framework;

namespace BombRush.Interfaces
{
    public interface FigureController
    {
        Figure Figure { get; set; }
        bool IsComputer { get; }
        bool DirectionChanged { get; }
        Vector2 MoveDirection { get; }
        bool ActionDone { get; }
        bool CancelDone { get; }
        void Update(float elapsed);
        void Reset();
        void ResetInputs();
    }
}
