using System;
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bombrush.MonoGame.Gui
{
    class ActionMenuItem : MenuItem
    {
        private event Action OnAction;

        public ActionTriggerKind ActionKind { get; set; }

        public ActionMenuItem(Game2D game, string text, Action onAction, ActionTriggerKind actionTriggerKind = ActionTriggerKind.None)
            : base(game)
        {
            Text = text;
            OnAction += onAction;
            ActionKind = actionTriggerKind;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (string.IsNullOrEmpty(Text))
                throw new InvalidOperationException("Text must not be null or empty");

            var origin = new Vector2(Resources.BigFont.MeasureString(Text).X / 2, 0);

            spriteBatch.DrawString(Resources.BigFont, Text, Position.SnapToPixels(), FontColor, 0.0f, origin.SnapToPixels(), 1.0f, SpriteEffects.None, 0);
        }

        public override float GetMaxWidth()
        {
            return string.IsNullOrEmpty(Text) ? 0 : Resources.BigFont.MeasureString(Text).X;
        }

        public virtual void FireAction()
        {
            if (OnAction != null)
                OnAction();
        }

        public override void Update(float elapsed)
        {
            if (Game.Keyboard.IsKeyDownOnce(Keys.Enter) && IsEnabled) FireAction();
        }
    }
}
