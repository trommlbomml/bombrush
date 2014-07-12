
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Gui2
{
    class Border2 : GameObject
    {
        public const int BorderSize = 12;
        public const int ShadowOffset = 8;
        public static readonly Color ShadowColor = Color.Black * 0.6f;

        private static readonly Rectangle _minSize = new Rectangle(0, 0, 2 * BorderSize, 2 * BorderSize);
        private readonly Texture2D _ninePatchTexture;
        private readonly NinePatchParameter _ninePatchParameter;

        public Border2(Game2D game) : base(game)
        {
            _ninePatchTexture = Game.Content.Load<Texture2D>("Textures/border");
            _ninePatchParameter = new NinePatchParameter(BorderSize);
        }

        public Rectangle Bounds { get; set; }

        public Rectangle MinSize
        {
            get { return _minSize; }
        }

        public void Draw(Color color)
        {
            Game.SpriteBatch.DrawNinePatch(_ninePatchTexture,
                                          new Rectangle(0, 0, _ninePatchTexture.Width, _ninePatchTexture.Height),
                                          Bounds, color, _ninePatchParameter);
        }
    }
}
