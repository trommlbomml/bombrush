using BombRush.Network.Framework;
using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Gui
{
    class TitledBorder : GameObject
    {
        private readonly Border _contentBorder;
        private readonly Border _titleBorder;

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RemeasureTitle();
                }
            }
        }

        private Rectangle _clientRectangle;

        public TitledBorder(Game2D game, string title = "", int width = 100, int height = 100, int minUserOffsetY = int.MinValue) : base(game)
        {
            _contentBorder = new Border(game);
            _titleBorder = new Border(game);
            Title = title;

            SetSize(width, height, minUserOffsetY);
        }

        public void SetClientSize(int clientWidth, int clientHeight, int minUserOffsetY)
        {
            SetSize(clientWidth + Border.BorderSize * 2, clientHeight + _titleBorder.Height + Border.BorderSize * 2, minUserOffsetY);
        }

        private void RemeasureTitle()
        {
            _titleBorder.Width = GetTitleBorderWidth();
            _titleBorder.X = (Game.ScreenWidth - _titleBorder.Width) / 2;
        }

        public void SetSize(int width, int height, int minUserOffsetY)
        {
            int startX = (Game.ScreenWidth - width) / 2;
            int startY = (int)MathHelper.Max(minUserOffsetY, (Game.ScreenHeight - height) / 2);

            _titleBorder.Width = GetTitleBorderWidth();
            _titleBorder.Height = GetTitleBorderHeight();
            _titleBorder.X = (Game.ScreenWidth - _titleBorder.Width) / 2;
            _titleBorder.Y = startY;

            _contentBorder.X = startX;
            _contentBorder.Y = startY + _titleBorder.Height / 2;
            _contentBorder.Width = width;
            _contentBorder.Height = height - _titleBorder.Height / 2;

            _clientRectangle = new Rectangle(
                _contentBorder.X + Border.BorderSize,
                _titleBorder.Y + _titleBorder.Height + Border.BorderSize,
                width - Border.BorderSize * 2,
                _contentBorder.Height - _titleBorder.Height / 2 - Border.BorderSize * 2);
        }

        public Rectangle ClientRectangle
        {
            get { return _clientRectangle; }
        }

        private int GetTitleBorderHeight()
        {
            return Resources.BigFont.LineSpacing + Border.BorderSize * 2;
        }

        private int GetTitleBorderWidth()
        {
            int width = Border.BorderSize * 2;

            if (!string.IsNullOrEmpty(Title))
                width += (int)Resources.BigFont.MeasureString(Title).X;

            return width;
        }

        public void Draw(SpriteBatch spriteBatch, bool withDropShadows)
        {
            if (withDropShadows)
            {
                _contentBorder.X += Border.ShadowOffset;
                _contentBorder.Y += Border.ShadowOffset;
                _contentBorder.Draw(Border.ShadowColor);
                _contentBorder.X -= Border.ShadowOffset;
                _contentBorder.Y -= Border.ShadowOffset;

                _titleBorder.X += Border.ShadowOffset;
                _titleBorder.Y += Border.ShadowOffset;
                _titleBorder.Draw(Border.ShadowColor);
                _titleBorder.X -= Border.ShadowOffset;
                _titleBorder.Y -= Border.ShadowOffset;
            }

            _contentBorder.Draw();
            _titleBorder.Draw();
            if (!string.IsNullOrEmpty(Title))
                spriteBatch.DrawString(Resources.BigFont, Title, new Vector2(_titleBorder.X + Border.BorderSize, _titleBorder.Y + Border.BorderSize), Color.White);
        }
    }
}
