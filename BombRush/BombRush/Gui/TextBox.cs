
using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Gui
{
    class TextBox : GameObject
    {
        private readonly StringInputController _inputController;
        private readonly Border _frameBorder;
        private SpriteFont _textInputFont;

        public Vector2 Position { get; set; }

        public TextBox(Game2D game, int width, int height) : base(game)
        {
            _frameBorder = new Border(game.Content.Load<Texture2D>("Textures/border"))
            {
                Width = width, 
                Height = height
            };
            _textInputFont = Resources.NormalFont;

            _inputController = new StringInputController(InputType.AlphaNumeric, 20);
        }

        public void Update(float elapsedTime)
        {
            _inputController.Update(Game.Keyboard, elapsedTime);
        }

        public void Draw()
        {
            _frameBorder.Draw(Game.SpriteBatch);
        }
    }
}
