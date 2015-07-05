using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.Gui
{
    class SplashBackground : GameObject
    {
        private readonly Texture2D _splashTexture;
        private static float _offset;

        public SplashBackground(Game2D game) : base(game)
        {
            _splashTexture = Game.Content.Load<Texture2D>("textures/splash");
            ModulateColor = Color.White;
        }

        public Color ModulateColor { get; set; }

        public override void Update(float elapsed)
        {
            _offset += elapsed;
            if (_offset > 1.0f)
                _offset -= 1.0f;
            else if (_offset < 0.0f)
                _offset += 1.0f;
        }

        public void Draw()
        {
            var tileCountX = 3 + Game.ScreenWidth/_splashTexture.Width;
            var tileCountY = 3 + Game.ScreenHeight / _splashTexture.Height;
            var offset = new Vector2(_splashTexture.Width, _splashTexture.Height) * _offset;

            for (var y = 0; y < tileCountY; y++)
            {
                for(var x = 0; x < tileCountX; x++)
                {
                   Game.SpriteBatch.Draw(_splashTexture, 
                                     (offset + new Vector2((x - 1) * _splashTexture.Width, (y-1) * _splashTexture.Height)).SnapToPixels(), 
                                     ModulateColor);
                }
            }
        }
    }
}
