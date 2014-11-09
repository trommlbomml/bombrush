using BombRush.Interfaces;
using Microsoft.Xna.Framework;

namespace BombRush.Network
{
    class TileBlockClient : ITileBlockDataProvider
    {
        public TileBlockClient(BlockType type)
        {
            Type = type;
        }
       
        public BlockType Type { get; set; }
        public ItemType AttachedItem { get; set; }
        public Vector2 Force { get; set; }
        public bool IsActive { get; set; }
        public Point TilePosition { get; set; }
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(TilePosition.X * BombGame.Tilesize + 2, TilePosition.Y * BombGame.Tilesize + 2, BombGame.Tilesize - 4, BombGame.Tilesize - 4);
            }
        }
    }
}
