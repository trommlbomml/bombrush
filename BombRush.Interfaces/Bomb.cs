
using Microsoft.Xna.Framework;

namespace BombRush.Interfaces
{
    public interface Bomb
    {
        byte Id { get; }
        Point TilePosition { get; }
        bool IsActive { get; }
        float CurrentBurnTime { get;}
        float BurnTime { get; }
        Rectangle Bounds { get; }
        BombType BombType { get; }
    }
}
