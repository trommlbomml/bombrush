using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BombRush.Interfaces;

namespace BombRush.Rendering.Render2D
{
    class ItemRenderer
    {
        private Dictionary<Point, float> _itemAnimationTimes;

        private Level _gameLevelLogic;
        
        private Tileset _tileset;

        private Vector2 _centeringOffset;

        public ItemRenderer(Tileset _tileset, Level _gameLevelLogic, Dictionary<Point, float> _itemAnimationTimes, Vector2 _centeringOffset)
        {
            this._tileset = _tileset;
            this._gameLevelLogic = _gameLevelLogic;
            this._itemAnimationTimes = _itemAnimationTimes;
            this._centeringOffset = _centeringOffset;
        }

        internal void render(Interfaces.Item item, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, float elapsedTime)
        {
            const int baseOffset = 4;
            float time = 0;
            int flop = 0;
            if (item.IsActive)
            {
                time = _itemAnimationTimes[item.TilePosition] + elapsedTime;
                if (time > 0.6f)
                {
                    time -= 0.6f;
                    flop = 0;
                }
                else if (time > 0.3f)
                {
                    flop = 1;
                }

                Vector2 position = _gameLevelLogic.GetWorldFromTilePosition(item.TilePosition);
                Rectangle sourceRectangle = new Rectangle((baseOffset + (((int)item.Type)-1) * 2 + flop) * BombGame.Tilesize, 0, BombGame.Tilesize, BombGame.Tilesize);
                spriteBatch.Draw(_tileset.TileSetTexture, position + _centeringOffset, sourceRectangle, Color.White);
            }
            
            _itemAnimationTimes[item.TilePosition] = time;
        }
    }
}
