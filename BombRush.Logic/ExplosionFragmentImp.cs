using BombRush.Interfaces;
using Microsoft.Xna.Framework;

namespace BombRush.Logic
{
    public class ExplosionFragmentImp : ExplosionFragment
    {
        public const float BurnTimeSeconds = 0.5f;

        public Point TilePosition { get; set; }

        public float BurnTime
        {
            get { return BurnTimeSeconds; }
        }

        public float ActiveTime { get; private set; }
        public bool IsActive { get; private set; }

        //Die Kollisionsbox der Explosion wird etwas verkleinert, um nicht so schnell zu sterben
        private const int Collisionpadding = 5;
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    TilePosition.X * TileBlockImp.Tilesize + Collisionpadding,
                    TilePosition.Y * TileBlockImp.Tilesize + Collisionpadding,
                    TileBlockImp.Tilesize - 2 * Collisionpadding,
                    TileBlockImp.Tilesize - 2 * Collisionpadding);
            }
        }

        public void Activate()
        {
            IsActive = true;
            ActiveTime = 0;
        }

        public void Update(float elapsed)
        {
            if (IsActive)
            {
                ActiveTime += elapsed;
                if (ActiveTime > BurnTime)
                    IsActive = false;
            }
        }
    }
}
