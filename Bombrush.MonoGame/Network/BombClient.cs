using BombRush.Interfaces;
using BombRush.Logic;
using Microsoft.Xna.Framework;

namespace BombRush.Network
{
    class BombClient : IBombDataProvider
    {
        public BombType BombType { get; set; }
        public byte Id { get; set; }
        public Point TilePosition { get; set; }
        public bool IsActive { get; set; }
        public float CurrentBurnTime { get; set; }
        public float BurnTime { get { return Bomb.BurnTimeSeconds; } }
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(TilePosition.X * BombGame.Tilesize + 2, TilePosition.Y * BombGame.Tilesize + 2, BombGame.Tilesize - 4, BombGame.Tilesize - 4);
            }
        }
    }
}
