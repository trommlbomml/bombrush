using System;
using Game2DFramework;
using Game2DFramework.Drawing;
using Game2DFramework.Extensions;
using Game2DFramework.Gui2;
using Game2DFramework.Gui2.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.Gui2
{
    internal class Button2 : UiElement
    {
        private NinePatchSprite _currentSprite;
        private Action _onClick;
        public override bool IsInteractable => true;
        public string Text { get; set; }

        public Button2(Game2D game, string text, Rectangle rectangle, Action onClick)
        {
            _onClick = onClick;
            Text = text;
            Bounds = rectangle;

            _currentSprite = new NinePatchSprite(game.Content.Load<Texture2D>("textures/selector"), new Rectangle(0,0,32,32), 15, 15);
            FocusedAnimation = new UiDiscreteAnimation<NinePatchSprite>(0.5f, new []
            {
                _currentSprite,
                new NinePatchSprite(game.Content.Load<Texture2D>("textures/selector"), new Rectangle(32,0,32,32), 15, 15)
            }, OnUpdate, true);
        }

        private void OnUpdate(NinePatchSprite current)
        {
            _currentSprite = current;
        }

        public override void OnAction()
        {
            _onClick?.Invoke();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(string.IsNullOrEmpty(Text)) return;

            var bounds = GetBounds();
            var textSize = Resources.NormalFont.MeasureString(Text);

            var position = new Vector2(bounds.X + bounds.Width * 0.5f - textSize.X * 0.5f, 
                                       bounds.Y + bounds.Height * 0.5f - textSize.Y * 0.5f).SnapToPixels();

            spriteBatch.DrawString(Resources.NormalFont, Text, position, Color.White);

            if(State == UiState.Focused)
            {
                _currentSprite.SetBounds(bounds);
                _currentSprite.Draw(spriteBatch);
            }
        }
    }
}
