using System;
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Gui2
{
    class TextBlock : GuiElement
    {
        public string Text { get; set; }
        public Color Color { get; set; }

        public TextBlock(Game2D game) : base(game)
        {
            Color = Color.White;
        }

        public override Rectangle GetMinSize()
        {
            if (string.IsNullOrEmpty(Text)) return new Rectangle();

            var size = Resources.BigFont.MeasureString(Text);
            return new Rectangle(0,0, (int)Math.Round(size.X), (int)Math.Round(size.Y));
        }

        public override void Arrange(Rectangle target)
        {
            Bounds = target;
        }

        public override void Draw()
        {
            if (string.IsNullOrEmpty(Text)) return;

            var size = Resources.BigFont.MeasureString(Text);

            var offset = new Vector2(Bounds.Width*0.5f - size.X*0.5f, Bounds.Height*0.5f - size.Y*0.5f);

            Game.SpriteBatch.DrawString(Resources.BigFont, Text, (new Vector2(Bounds.X, Bounds.Y) + offset).SnapToPixels(), Color);
        }

        public override void Update(float elapsedTime)
        {
            
        }
    }
}
