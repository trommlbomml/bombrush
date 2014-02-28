using BombRush.Interfaces;
using Microsoft.Xna.Framework;

namespace BombRush.Logic
{
    public class TileBlockImp : TileBlock
    {
        public const int Tilesize = 32;

        private float _explodeTime;
        private bool _exploding;

        public Point TilePosition { get; set; }
        public BlockType Type { get; private set; }
        public ItemType AttachedItem { get; set; }
        public Vector2 Force { get; private set; }
        public bool IsActive { get; set; }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(TilePosition.X * Tilesize, TilePosition.Y * Tilesize, Tilesize, Tilesize);
            }
        }

        public TileBlockImp(BlockType type)
        {
            SetType(type);
            AttachedItem = ItemType.Empty;
            IsActive = true;
            _exploding = false;
        }

        public void SetType(BlockType type)
        {
            Type = type;

            switch (type)
            {
                case BlockType.TreadmillLeft:
                    Force = -Vector2.UnitX;
                    break;
                case BlockType.TreadmillRight:
                    Force = Vector2.UnitX;
                    break;
                case BlockType.TreadmillUp:
                    Force = -Vector2.UnitY;
                    break;
                case BlockType.TreadmillDown:
                    Force = Vector2.UnitY;
                    break;
                default:
                    Force = Vector2.Zero;
                    break;
            }
        }

        public void Update(float elapsed)
        {
            if (_exploding)
            {
                _explodeTime += elapsed;
                if (_explodeTime >= ExplosionFragmentImp.BurnTimeSeconds)
                {
                    _exploding = false;
                    IsActive = false;
                }
            }
        }

        public void Explode()
        {
            if (IsActive)
            {
                _explodeTime = 0;
                _exploding = true;
            }
        }
    }
}
