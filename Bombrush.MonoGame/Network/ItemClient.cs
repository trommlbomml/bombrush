using BombRush.Interfaces;
using Microsoft.Xna.Framework;

namespace BombRush.Network
{
    class ItemClient : IItemDataProvider
    {
        public Point TilePosition { get; set; }
        public ItemType Type { get; set; }
        public bool IsActive { get; set; }
    }
}
