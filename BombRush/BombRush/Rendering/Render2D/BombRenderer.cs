using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render2D
{
    class BombRenderer
    {
        private Tileset _tileset;
        private Interfaces.Level _gameLevelLogic;
        private Vector2 _centeringOffset;

        public BombRenderer(Tileset _tileset, Interfaces.Level _gameLevelLogic, Vector2 _centeringOffset)
        {
            // TODO: Complete member initialization
            this._tileset = _tileset;
            this._gameLevelLogic = _gameLevelLogic;
            this._centeringOffset = _centeringOffset;
        }

        internal void render(Interfaces.Bomb bomb, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            Rectangle bombSourceRectangle = new Rectangle(3 * BombGame.Tilesize + 1,
                                                          (byte)bomb.BombType * BombGame.Tilesize + 1,
                                                          BombGame.Tilesize - 2,
                                                          BombGame.Tilesize - 2);
            Vector2 centerOrigin = new Vector2(BombGame.Tilesize * 0.5f - 2, BombGame.Tilesize * 0.5f - 2);

            if (bomb.IsActive)
            {
                float bombAnimationScale = 0.75f + (float)Math.Cos(bomb.CurrentBurnTime / bomb.BurnTime * MathHelper.TwoPi * 12) * 0.05f;

                spriteBatch.Draw(_tileset.TileSetTexture, _gameLevelLogic.GetWorldFromTilePositionCentered(bomb.TilePosition) + _centeringOffset,
                    bombSourceRectangle, Color.White,
                    0.0f, centerOrigin, bombAnimationScale, SpriteEffects.None, 0);
            }
        }

    }
}
