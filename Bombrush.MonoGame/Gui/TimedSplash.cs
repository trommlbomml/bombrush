using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.Gui
{
    class TimedSplash : GameObject
    {
        private readonly Border _border;
        private List<string> _text;
        private float _lifeTime;
        private float _elapsed;
        private const int Padding = 10;

        public TimedSplash(Game2D game) : base(game)
        {
            _border = new Border(game);
            _elapsed = 0;
            Running = false;
        }

        private static List<string> WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
        {
            var lines = new List<string>();
            var words = text.Split(' ');

            var lineWidth = 0f;
            var sb = new StringBuilder();
            var spaceWidth = spriteFont.MeasureString(" ").X;

            foreach (var word in words)
            {
                var size = spriteFont.MeasureString(word);

                if (lineWidth + size.X > maxLineWidth)
                {
                    lines.Add(sb.ToString().Trim());
                    sb.Clear();
                    lineWidth = 0;
                }

                sb.Append(word + " ");
                lineWidth += size.X + spaceWidth;
            }

            if (sb.Length > 0) lines.Add(sb.ToString().Trim());

            return lines;
        }

        public void Start(string text, float lifeTime)
        {
            _text = WrapText(Resources.BigFont, text, 400);
            _elapsed = 0;
            Running = true;
            _lifeTime = lifeTime;

            _border.SetClientSize((int)_text.Select(s => Resources.BigFont.MeasureString(s).X).Max(l => l) + 2 * Padding, _text.Count * Resources.BigFont.LineSpacing + 2 * Padding);
            _border.CenterHorizontal();
            _border.CenterVertical();
        }

        public bool Running { get; private set; }

        public override void Update(float elapsed)
        {
            if (!Running) return;

            _elapsed += elapsed;

            if (_elapsed > _lifeTime)
            {
                Running = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Running) return;

            var remaining = _lifeTime - _elapsed;
            var alpha = remaining <= 0.25f ? remaining * 4.0f : 1.0f;

            _border.Draw();

            var start = _border.ClientY + Padding;
            foreach (var line in _text)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    spriteBatch.DrawString( Resources.BigFont, line, new Vector2(_border.ClientX + Padding, start), Color.Red * alpha);
                }
                start += Resources.BigFont.LineSpacing;
            }
        }
    }
}
