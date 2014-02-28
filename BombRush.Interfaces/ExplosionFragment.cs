using Microsoft.Xna.Framework;

namespace BombRush.Interfaces
{
    public interface ExplosionFragment
    {
        Point TilePosition { get; }
        float BurnTime { get; }
        float ActiveTime { get; }
        bool IsActive { get; }
    }
}
