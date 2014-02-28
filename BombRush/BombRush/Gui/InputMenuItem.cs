using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Gui
{
    internal class InputMenuItem : MenuItem
    {
        private readonly StringInputController _inputController;

        private bool _active;
        private readonly bool _normalText;
        private float _triggerTime;
        private bool _visibleBlank;
        
        public string InputText 
        {
            get { return _inputController.CurrentText; }
            set { _inputController.CurrentText = value; }
        }

        private SpriteFont Font
        {
            get { return _normalText ? Resources.NormalFont : Resources.BigFont; }
        }

        public InputMenuItem(Game2D game, string text, int maxInputTextLength, InputType inputType, bool normalText = false) : base(game)
        {
            Text = text;
            _normalText = normalText;

            _inputController = new StringInputController(inputType, maxInputTextLength);
        }

        public override void Update(float elapsed)
        {
            _active = true;
            _triggerTime += elapsed;
            if (_triggerTime > 0.25f)
            {
                _triggerTime -= 0.25f;
                _visibleBlank = !_visibleBlank;
            }

            if (!IsEnabled) return;

            _inputController.Update(Game.Keyboard, elapsed);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var text = string.Format("{0}: {1}{2}", Text, InputText, (_visibleBlank && _active ? "_" : ""));
            var origin = new Vector2(GetWidth() / 2, 0);
            spriteBatch.DrawString(Font, text, Position.SnapToPixels(), FontColor, 0.0f, origin.SnapToPixels(), 1.0f, SpriteEffects.None, 0);

            _active = false;
        }

        private float GetWidth()
        {
            return Font.MeasureString(string.Format("{0}: {1}_", Text, InputText)).X;
        }

        public override float GetMaxWidth()
        {
            return Font.MeasureString(string.Format("{0}: {1}_", Text, new string('W', _inputController.MaxInputCharacters))).X;
        }
    }
}
