using System;
using System.Xml;
using BombRush.Gui;
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;

namespace BombRush.Gui2
{
    class Button : TextBlock
    {
        private readonly AnimatedBomb _animatedBomb;

        public Action OnClick { get; set; }

        public Button(Game2D game, XmlElement element) : base(game, element)
        {
            _animatedBomb = new AnimatedBomb(game.Content);
        }

        public Button(Game2D game) : base(game)
        {
            _animatedBomb = new AnimatedBomb(game.Content);
        }

        public override Rectangle GetMinSize()
        {
            var minSize = base.GetMinSize();
            minSize.Width += 64;
            return minSize;
        }

        public override void Arrange(Rectangle target)
        {
            base.Arrange(target);
            _animatedBomb.Position = new Vector2(target.X + 16 + Margin.Left, target.Y + 16 + Margin.Top);
        }

        private bool _isMouseOver;

        public override void Update(float elapsedTime)
        {
            _animatedBomb.Update(elapsedTime);
            _isMouseOver = Bounds.Contains(new Vector2(Game.Mouse.X, Game.Mouse.Y).ToPoint());

            Color = _isMouseOver ? Color.Yellow : Color.White;

            if (Game.Mouse.IsLeftButtonDownOnce && _isMouseOver && OnClick != null) OnClick();
        }

        public override void Draw()
        {
            base.Draw();
            if (_isMouseOver) _animatedBomb.Draw(Game.SpriteBatch);
        }
    }
}
