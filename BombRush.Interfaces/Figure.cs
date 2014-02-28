
using Microsoft.Xna.Framework;

namespace BombRush.Interfaces
{
    public interface Figure
    {
        bool ShowPlayerName { get;}
        FigureDirection Direction { get; }
        Vector2 WalkDirection { get; }
        Vector2 Position { get; }
        string Name { get; }
        byte Id { get; }
        int Wins { get; }
        float Speed { get; }
        bool IsAlive { get; }
        bool IsVisible { get; }
        bool IsMatchWinner { get; }
    }
}
