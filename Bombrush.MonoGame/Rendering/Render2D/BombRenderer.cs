using System;
using System.Linq;
using BombRush.Interfaces;
using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.Rendering.Render2D
{
    class BombRenderer : GameObject
    {
        private readonly Texture2D _tileset;
        private readonly Vector2 _centeringOffset;

        public BombRenderer(Game2D game, Vector2 centeringOffset) : base(game)
        {
            _tileset = Game.Content.Load<Texture2D>("textures/tileset");
            _centeringOffset = centeringOffset;
        }

        public void Render(Level level)
        {
            var bombSourceRectangle = new Rectangle(3 * BombGame.Tilesize + 1, 0,
                                                    BombGame.Tilesize - 2, BombGame.Tilesize - 2);

            foreach (var bomb in level.Bombs.Where(b => b.IsActive))
            {
                bombSourceRectangle.Y = (byte) bomb.BombType*BombGame.Tilesize + 1;
                var centerOrigin = new Vector2(BombGame.Tilesize * 0.5f - 2, BombGame.Tilesize * 0.5f - 2);
                var bombAnimationScale = 0.75f + (float)Math.Cos(bomb.CurrentBurnTime / bomb.BurnTime * MathHelper.TwoPi * 12) * 0.05f;

                Game.SpriteBatch.Draw(_tileset,
                    level.GetWorldFromTilePositionCentered(bomb.TilePosition) + _centeringOffset,
                    bombSourceRectangle, Color.White,
                    0.0f, centerOrigin, bombAnimationScale, SpriteEffects.None, 0);
            }

        }

    }
}
