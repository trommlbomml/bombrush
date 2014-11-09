using System.Collections.Generic;
using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.Gui
{
    class Statusbar : GameObject
    {
        private readonly Border _border;
        private readonly List<string> _entries;

        public int Padding { get; set; }
        public int ItemSpacing { get; set; }
        public int Width { get; set; }

        public Statusbar(Game2D game) : base(game)
        {
            _border = new Border(game);
            _entries = new List<string>();
        }

        public void AddItem(string title)
        {
            _entries.Add(title);
        }

        public void Draw(SpriteBatch spriteBatch, bool withShadows)
        {
            const int bottomOffset = Border.ShadowOffset + 1;

            _border.X = (Game.ScreenWidth - Width)/2;
            _border.Height = Resources.NormalFont.LineSpacing + Border.BorderSize * 2 + Padding * 2;
            _border.Y = Game.ScreenHeight - _border.Height - bottomOffset;
            _border.Width = Width;

            if (withShadows)
            {
                _border.X += Border.ShadowOffset;
                _border.Y += Border.ShadowOffset;
                _border.Draw(Border.ShadowColor);
                _border.X -= Border.ShadowOffset;
                _border.Y -= Border.ShadowOffset;
            }

            _border.Draw();

            int startX = (Game.ScreenWidth - Width) / 2 + Border.BorderSize + Padding;
            int startY = Game.ScreenHeight - (Resources.NormalFont.LineSpacing + Border.BorderSize + Padding) - bottomOffset;
            foreach (string entry in _entries)
            {
                int sizeX = (int)Resources.NormalFont.MeasureString(entry).X;
                spriteBatch.DrawString(Resources.NormalFont, entry, new Vector2(startX, startY), Color.White);
                startX += sizeX + ItemSpacing;
            }
        }
    }
}
