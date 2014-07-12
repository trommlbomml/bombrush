using System;
using BombRush.Gui;
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Gui2
{
    class Button : TextBlock
    {
        private readonly AnimatedBomb _animatedBomb;
        private readonly Action _onClick;

        public Button(Game2D game, Action onClick) : base(game)
        {
            _onClick = onClick;
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
            _animatedBomb.Position = new Vector2(target.X + 16, target.Y + 16);
        }

        private bool _isMouseOver;

        public override void Update(float elapsedTime)
        {
            _animatedBomb.Update(elapsedTime);
            _isMouseOver = Bounds.Contains(new Vector2(Game.Mouse.X, Game.Mouse.Y).ToPoint());

            Color = _isMouseOver ? Color.Yellow : Color.White;

            if (Game.Mouse.IsLeftButtonDownOnce && _isMouseOver)
            {
                _onClick();
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (_isMouseOver) _animatedBomb.Draw(Game.SpriteBatch);
        }
    }
}
