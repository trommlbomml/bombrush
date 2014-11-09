using System;
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bombrush.MonoGame.Gui
{
    class NumericMenuItem : MenuItem
    {
        protected virtual string FullText { get { return string.Format("{0} <{1}>", Text, CurrentValue); } }

        public NumericMenuItem(Game2D game, string text, int min, int max, bool jumOnEnd = false) : base(game)
        {
            Text = text;
            MinValue = min;
            MaxValue = max;
            CurrentValue = MinValue;
            JumpOnEnd = jumOnEnd;
            Step = 1;
        }

        public int CurrentValue { get; set; }

        private int _minValue;
        public int MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                CurrentValue = Math.Max(_minValue, CurrentValue);
            }
        }

        private int _maxValue;
        public int MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                CurrentValue = Math.Min(_maxValue, CurrentValue);
            }
        }

        public bool JumpOnEnd { get; set; }
        public int Step { get; set; }

        public override void Update(float elapsed)
        {
            if (Game.Keyboard.IsKeyDownOnce(Keys.Left) && IsEnabled)
            {
                CurrentValue -= Step;
                if (CurrentValue < MinValue)
                {
                    CurrentValue = JumpOnEnd ? MaxValue : MinValue;
                }
            }

            if (Game.Keyboard.IsKeyDownOnce(Keys.Right) && IsEnabled)
            {
                CurrentValue += Step;
                if (CurrentValue > MaxValue)
                {
                    CurrentValue = JumpOnEnd ? MinValue : MaxValue;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var text = FullText;
            var origin = new Vector2(Resources.BigFont.MeasureString(text).X / 2, 0);
            spriteBatch.DrawString(Resources.BigFont, text, Position.SnapToPixels(), FontColor, 0.0f, origin.SnapToPixels(), 1.0f, SpriteEffects.None, 0);
        }

        public override float GetMaxWidth()
        {
            return string.IsNullOrEmpty(Text) ? 0 : Resources.BigFont.MeasureString(string.Format("{0} <{1}>", Text, MaxValue)).X;
        }
    }
}
