using System;
using BombRush.Network.Framework;
using Game2DFramework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BombRush.Gui
{
    class TimedSplash : GameObject
    {
        private readonly Border _border;
        private String _text;
        private float _lifeTime;
        private float _elapsed;
        private const int Padding = 10;

        public TimedSplash(Game2D game) : base(game)
        {
            _border = new Border(Game.Content.Load<Texture2D>("textures/border"));
            _elapsed = 0;
            Running = false;
        }

        public void Start(string text, float lifeTime)
        {
            _text = text;
            _elapsed = 0;
            Running = true;
            _lifeTime = lifeTime;

            _border.Width = (int)Resources.BigFont.MeasureString(_text).X + 2 * Padding + 2 * Border.BorderSize;
            _border.Height = Resources.BigFont.LineSpacing + 2 * Padding + 2 * Border.BorderSize;
            _border.X = (Game.ScreenWidth - _border.Width) / 2;
            _border.Y = (Game.ScreenHeight - _border.Height) / 2;
        }

        public bool Running { get; private set; }

        public void Update(float elapsed)
        {
            if (!Running)
                return;

            _elapsed += elapsed;

            if (_elapsed > _lifeTime)
            {
                Running = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, bool withShadows)
        {
            if (!Running)
                return;

            float remaining = _lifeTime - _elapsed;
            float alpha = remaining <= 0.25f ? remaining * 4.0f : 1.0f;

            if (withShadows)
            {
                _border.X += Border.ShadowOffset;
                _border.Y += Border.ShadowOffset;
                _border.Draw(spriteBatch, Color.Black * alpha);
                _border.X -= Border.ShadowOffset;
                _border.Y -= Border.ShadowOffset;
            }

            _border.Draw(spriteBatch, Color.Red * alpha);
                     
            spriteBatch.DrawString(
                Resources.BigFont, 
                _text, 
                new Vector2(_border.X + Padding + Border.BorderSize, _border.Y + Padding + Border.BorderSize),
                Color.Red * alpha);
        }
    }
}
