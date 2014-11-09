using BombRush.Network.Framework;
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BombRush.Gui
{
    class BoolMenuItem : MenuItem
    {
        public bool IsTrue;
        private readonly DecisionType _decisionType;

        protected string FullText
        {
            get
            {
                string value = IsTrue ? "Yes" : "No";
                if (_decisionType == DecisionType.OnOff)
                    value = IsTrue ? "On" : "Off";

                return string.Format("{0} <{1}>", Text, value);
            }
        }

        public BoolMenuItem(Game2D game, string text, DecisionType decisionType, bool defaultValue = false) : base(game)
        {
            _decisionType = decisionType;
            Text = text;
            IsTrue = defaultValue;
        }

        public override void Update(float elapsed)
        {
            if ((Game.Keyboard.IsKeyDownOnce(Keys.Left) || Game.Keyboard.IsKeyDownOnce(Keys.Right)) && IsEnabled)
            {
                IsTrue = !IsTrue;
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
            return Resources.BigFont.MeasureString(string.Format("{0} <{1}>", Text, (_decisionType == DecisionType.YesNo ? "Yes" : "Off"))).X;
        }
    }
}
