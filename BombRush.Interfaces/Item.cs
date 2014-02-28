
using Microsoft.Xna.Framework;

namespace BombRush.Interfaces
{
    public interface Item
    {
        Point TilePosition { get; }
        ItemType Type { get; }
        bool IsActive { get; }
    }
}
