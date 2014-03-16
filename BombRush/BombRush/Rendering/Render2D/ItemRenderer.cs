
using System.Collections.Generic;
using Game2DFramework;
using Microsoft.Xna.Framework;
using BombRush.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render2D
{
    class ItemRenderer : GameObject
    {
        private readonly Dictionary<Point, float> _itemAnimationTimes;
        private readonly Vector2 _centeringOffset;
        private readonly Texture2D _tileset;

        public ItemRenderer(Game2D game, Vector2 centeringOffset) : base(game)
        {
            _tileset = Game.Content.Load<Texture2D>("textures/tileset");
            _centeringOffset = centeringOffset;

            _itemAnimationTimes = new Dictionary<Point, float>();
            for (var y = 0; y < BombGame.GameLevelWidth; y++)
            {
                for (var x = 0; x < BombGame.GameLevelHeight; x++)
                {
                    _itemAnimationTimes.Add(new Point(x, y), 0);
                }
            }
        }

        public void RenderItems(Level level, float elapsedTime)
        {
            foreach (var item in level.ItemData)
            {
                const int baseOffset = 4;
                float time = 0;
                var flop = 0;
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

                    var position = level.GetWorldFromTilePosition(item.TilePosition);
                    var sourceRectangle = new Rectangle((baseOffset + (((int)item.Type) - 1) * 2 + flop) * BombGame.Tilesize, 0, BombGame.Tilesize, BombGame.Tilesize);
                    Game.SpriteBatch.Draw(_tileset, position + _centeringOffset, sourceRectangle, Color.White);
                }

                _itemAnimationTimes[item.TilePosition] = time;
            }
        }
    }
}
