using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.Gui
{
    class AnimatedBomb
    {
        private readonly Texture2D _tileSet;
        private float _bombAnimationScale;
        private float _bombAnimationScaleDir;

        public AnimatedBomb(ContentManager content)
        {
            _tileSet = content.Load<Texture2D>("textures/tileset");
            Reset();
        }

        public Vector2 Position { get; set; }

        public void Reset()
        {
            _bombAnimationScale = 0.9f;
            _bombAnimationScaleDir = 1.0f;
        }

        public void Update(float elapsedTime)
        {
            _bombAnimationScale += _bombAnimationScaleDir * elapsedTime;
            if (_bombAnimationScale <= 0.9f && _bombAnimationScaleDir < 0.0f)
            {
                _bombAnimationScale = 0.9f;
                _bombAnimationScaleDir = 1.0f;
            }
            else if (_bombAnimationScale >= 1.0f && _bombAnimationScaleDir > 0.0f)
            {
                _bombAnimationScale = 1.0f;
                _bombAnimationScaleDir = -1.0f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_tileSet, Position,
                    new Rectangle(3 * BombGame.Tilesize + 1, 1, BombGame.Tilesize - 2, BombGame.Tilesize - 2), Color.White,
                    0.0f, new Vector2(BombGame.Tilesize * 0.5f), _bombAnimationScale, SpriteEffects.None, 0);
        }
    }
}
