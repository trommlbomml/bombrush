using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering
{
    class Cursor : GameObject
    {
        private readonly Texture2D _cursorTexture;
        private Vector2 _position;

        public Cursor(Game2D game) : base(game)
        {
            _cursorTexture = Game.Content.Load<Texture2D>("Textures/cursor");
        }

        public void Update()
        {
            _position = new Vector2(Game.Mouse.X, Game.Mouse.Y);
        }

        public void Draw()
        {
            Game.SpriteBatch.Draw(_cursorTexture, _position, Color.White);
        }
    }
}
