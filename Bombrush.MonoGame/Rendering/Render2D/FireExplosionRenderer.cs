using System.Linq;
using BombRush.Interfaces;
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.Rendering.Render2D
{
    class FireExplosionRenderer : GameObject
    {
        private readonly Vector2 _centeringOffset;
        private readonly Texture2D _tileset;

        public FireExplosionRenderer(Game2D game, Vector2 centeringOffset) : base(game)
        {
            _tileset = Game.Content.Load<Texture2D>("textures/tileset");
            _centeringOffset = centeringOffset;
        }

        private static Rectangle ExamineRectangleFromSurround(Level level, ExplosionFragment fragment)
        {
            var leftActive = level.GetOverlayInformation(fragment.TilePosition.Left()).IsActive;
            var rightActive = level.GetOverlayInformation(fragment.TilePosition.Right()).IsActive;
            var upActive = level.GetOverlayInformation(fragment.TilePosition.Up()).IsActive;
            var downActive = level.GetOverlayInformation(fragment.TilePosition.Down()).IsActive;

            var currentFrame = (int)(fragment.ActiveTime / 0.05f) % 4;

            var source = new Rectangle(0, 128 + currentFrame * BombGame.Tilesize, BombGame.Tilesize, BombGame.Tilesize);

            if (!leftActive && !rightActive && upActive && downActive)
            {
                source.X = 1 * BombGame.Tilesize;
                return source;
            }

            if (leftActive && rightActive && !upActive && !downActive)
            {
                source.X = 2 * BombGame.Tilesize;
                return source;
            }

            if (!leftActive && !rightActive && !upActive && downActive)
            {
                source.X = 3 * BombGame.Tilesize;
                return source;
            }

            if (leftActive && !rightActive && !upActive && !downActive)
            {
                source.X = 4 * BombGame.Tilesize;
                return source;
            }

            if (!leftActive && !rightActive && upActive && !downActive)
            {
                source.X = 5 * BombGame.Tilesize;
                return source;
            }

            if (!leftActive && rightActive && !upActive && !downActive)
            {
                source.X = 6 * BombGame.Tilesize;
                return source;
            }

            return source;
        }

        public void Render(Level level, float elapsedTime)
        {
            foreach (var fragment in level.OverlayData.Where(f => f.IsActive))
            {
                var position = level.GetWorldFromTilePosition(fragment.TilePosition);
                var sourceRectangle = ExamineRectangleFromSurround(level, fragment);
                Game.SpriteBatch.Draw(_tileset, position + _centeringOffset, sourceRectangle, Color.White);
            }
        }
    }
}
