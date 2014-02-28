
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Gui
{
    class SplashBackground : GameObject
    {
        private readonly Texture2D _splashTexture;
        private static float _offset;

        public SplashBackground(Game2D game, Texture2D texture) : base(game)
        {
            _splashTexture = texture;
            ModulateColor = Color.White;
        }

        public Color ModulateColor { get; set; }

        public void Update(float elapsed)
        {
            _offset += elapsed;
            if (_offset > 1.0f)
                _offset -= 1.0f;
            else if (_offset < 0.0f)
                _offset += 1.0f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var tileCountX = 3 + Game.ScreenWidth/_splashTexture.Width;
            var tileCountY = 3 + Game.ScreenHeight / _splashTexture.Height;
            var offset = new Vector2(_splashTexture.Width, _splashTexture.Height) * _offset;

            for (int y = 0; y < tileCountY; y++)
            {
                for(int x = 0; x < tileCountX; x++)
                {
                    spriteBatch.Draw(_splashTexture, 
                                     (offset + new Vector2((x - 1) * _splashTexture.Width, (y-1) * _splashTexture.Height)).SnapToPixels(), 
                                     ModulateColor);
                }
            }
        }
    }
}
