using Microsoft.Xna.Framework;

namespace BombRush.Interfaces
{
    public interface TileBlock
    {
        BlockType Type { get;}
        ItemType AttachedItem { get; }
        Vector2 Force { get; }
        bool IsActive { get; }
        Point TilePosition { get; }
        Rectangle Bounds { get; }
    }
}
