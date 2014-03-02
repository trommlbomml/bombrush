
using System;
using System.Globalization;
using BombRush.Gui;
using BombRush.Network.Framework;
using BombRush.Interfaces;
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering
{
    class GameInformation : GameObject
    {
        private readonly Border _border;
        private readonly SpriteFont _font;
        private readonly int _defaultSizeForNumber;
        private readonly Level _level;

        public int Width
        {
            get { return _border.Width; }
            set 
            {
                _border.Width = value;
                _border.X = (Game.ScreenWidth - _border.Width)/2;
            }
        }

        public int Height { get { return Border.BorderSize*2 + _font.LineSpacing; } }

        public GameInformation(Game2D game, Level level) : base(game)
        {
            _border = new Border(game);
            _font = Resources.NormalFont;
            _level = level;

            _defaultSizeForNumber = (int)Math.Ceiling(_font.MeasureString("99").X);
            _border.Height = Height;
            _border.Y = 0;
        }

        private string GetTimeString()
        {
            int remaining = Math.Max(0, (int)Math.Truncate(_level.RemainingGameTime));
            return string.Format("{0:00}:{1:00}", remaining / 60, remaining % 60);
        }

        private int GetStartXFromColor(byte id)
        {
            var baseStartX = _border.X + Border.BorderSize;
            var previosOffset = _font.LineSpacing + Border.BorderSize + _defaultSizeForNumber;
            var baseStartRightX = _border.X + _border.Width - Border.BorderSize - 2*previosOffset;
            switch (id)
            {
                case 1:
                    return baseStartX;
                case 2:
                    return baseStartX + previosOffset;
                case 3:
                    return baseStartRightX;
                case 4:
                    return baseStartRightX + previosOffset;
            }

            return 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _border.Draw();

            var timeString = GetTimeString();
            spriteBatch.DrawString(
                _font,
                timeString, 
                new Vector2(Game.ScreenWidth * 0.5f, Border.BorderSize), 
                Color.White, 
                0.0f,
                new Vector2(_font.MeasureString(timeString).X / 2, 0.0f).SnapToPixels(), 
                1.0f, 
                SpriteEffects.None, 
                0);

            var startY = _border.Y + Border.BorderSize;

            foreach (var figure in _level.Figures)
            {
                var startX = GetStartXFromColor(figure.Id);

                Heads.Draw(spriteBatch, figure.Id, startX, startY, _font.LineSpacing, _font.LineSpacing);
                spriteBatch.DrawString(_font, 
                    figure.Wins.ToString(CultureInfo.InvariantCulture), 
                    new Vector2(startX + _font.LineSpacing + Border.BorderSize, startY), 
                    Color.White);
            }
        }
    }
}
