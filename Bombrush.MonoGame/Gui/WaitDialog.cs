using Game2DFramework;
using Microsoft.Xna.Framework;

namespace Bombrush.MonoGame.Gui
{
    class WaitDialog : GameObject
    {
        private static readonly string[] Dots = { "", ".", "..", "..." };

        private const float TimePerDot = 0.33f;

        private readonly Border _border;
        private int _dotCount;
        private float _elapsed;

        public string Text { get; set; }
        public bool IsActive { get; set; }

        public WaitDialog(Game2D game, int width) : base(game)
        {
            _border = new Border(game)
            {
                Width = width,
                Height = Resources.BigFont.LineSpacing*2 + Border.BorderSize*2
            };

            _border.X = (Game.ScreenWidth - _border.Width) / 2;
            _border.Y = (Game.ScreenHeight - _border.Height) / 2;
            _elapsed = 0;
        }

        public void Update(float elapsedTime)
        {
            if (!IsActive) return;

            _elapsed += elapsedTime;
            if (_elapsed > TimePerDot)
            {
                _elapsed -= TimePerDot;
                if (++_dotCount == 4) _dotCount = 0;
            }
        }

        private string GetCurrentStateInformationString()
        {
            return string.Format("{0} {1}",Text, Dots[_dotCount]);   
        }

        public void Draw()
        {
            if (!IsActive) return;

            _border.X += Border.ShadowOffset;
            _border.Y += Border.ShadowOffset;
            _border.Draw(Border.ShadowColor);
            _border.X -= Border.ShadowOffset;
            _border.Y -= Border.ShadowOffset;
            _border.Draw();

            Game.SpriteBatch.DrawString(
                Resources.BigFont,
                GetCurrentStateInformationString(),
                new Vector2(_border.X, _border.Y + Resources.BigFont.LineSpacing / 2) + new Vector2(Border.BorderSize),
                Color.White);
        }
    }
}
