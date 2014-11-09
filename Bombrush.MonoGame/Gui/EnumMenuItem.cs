
using System.Collections.Generic;
using BombRush.Network.Framework;
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BombRush.Gui
{
    class EnumMenuItem : MenuItem
    {
        private readonly List<string> _values;
        private int _currentIndex;

        protected string FullText { get { return string.Format("{0} <{1}>", Text, SelectedItem); } }

        public EnumMenuItem(Game2D game, string text, IEnumerable<string> data, string selectedItem = "") : base(game)
        {
            Text = text;
            _values = new List<string>(data);
            _currentIndex = 0;
            if (!string.IsNullOrEmpty(selectedItem))
                SetCurrentItem(selectedItem);
        }

        public string SelectedItem
        {
            get { return _values[_currentIndex]; }
        }

        public void SetCurrentItem(string enumName)
        {
            var index = _values.IndexOf(enumName);
            if (index != -1)
            {
                _currentIndex = index;
            }
        }

        public override void Update(float elapsed)
        {
            if (Game.Keyboard.IsKeyDownOnce(Keys.Left) && IsEnabled)
            {
                _currentIndex--;
                if (_currentIndex < 0)
                {
                    _currentIndex = _values.Count-1;
                }
            }

            if (Game.Keyboard.IsKeyDownOnce(Keys.Right) && IsEnabled)
            {
                _currentIndex++;
                if (_currentIndex == _values.Count)
                {
                    _currentIndex = 0;
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
            var s = string.Empty;
            _values.ForEach(v => s = v.Length > s.Length ? v : s);

            return Resources.BigFont.MeasureString(string.Format("{0} <{1}>", Text, s)).X;
        }
    }
}
