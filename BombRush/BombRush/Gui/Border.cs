using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Gui
{
    class Border
    {
        public const int BorderSize = 12;
        public const int ShadowOffset = 8;
        public static readonly Color ShadowColor = Color.Black * 0.6f;

        private readonly Texture2D _ninePatchTexture;
        private readonly NinePatchParameter _ninePatchParameter;

        public Border(Texture2D ninePatch)
        {
            _ninePatchTexture = ninePatch;
            _ninePatchParameter = new NinePatchParameter(BorderSize);
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.DrawNinePatch(_ninePatchTexture,
                                      new Rectangle(0, 0, _ninePatchTexture.Width, _ninePatchTexture.Height),
                                      new Rectangle(X, Y, Width, Height),
                                      color,
                                      _ninePatchParameter);
        }
    }
}
