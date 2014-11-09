using BombRush.Interfaces;
using BombRush.Logic;
using Microsoft.Xna.Framework;

namespace BombRush.Network
{
    class OverlayClient : IExplosionFragmentDataProvider
    {
        public Point TilePosition { get; set; }
        public float BurnTime { get { return ExplosionFragment.BurnTimeSeconds; } }
        public float ActiveTime { get; set; }
        public bool IsActive { get; set; }
    }
}
