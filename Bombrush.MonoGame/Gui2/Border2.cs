using Game2DFramework;
using Game2DFramework.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.Gui2
{
    class Border2 : GameObject
    {
        public const int BorderSize = 12;
        public const int ShadowOffset = 8;
        public static readonly Color ShadowColor = Color.Black * 0.6f;

        private static readonly Rectangle _minSize = new Rectangle(0, 0, 2 * BorderSize, 2 * BorderSize);
        private readonly Texture2D _ninePatchTexture;
        private readonly NinePatchSprite _ninePatch;

        public Border2(Game2D game) : base(game)
        {
            _ninePatchTexture = Game.Content.Load<Texture2D>("Textures/border");
            _ninePatch = new NinePatchSprite(_ninePatchTexture, BorderSize);

        }

        public Rectangle Bounds { get; set; }

        public Rectangle MinSize
        {
            get { return _minSize; }
        }

        public void Draw(Color color)
        {
            _ninePatch.Draw(Game.SpriteBatch, Bounds, color);
        }
    }
}
