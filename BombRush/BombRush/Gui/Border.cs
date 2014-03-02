using System.Windows.Forms;
using BombRush.Networking;
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Gui
{
    class Border : GameObject
    {
        public const int BorderSize = 12;
        public const int ShadowOffset = 8;
        public static readonly Color ShadowColor = Color.Black * 0.6f;

        private readonly Texture2D _ninePatchTexture;
        private readonly NinePatchParameter _ninePatchParameter;

        public Border(Game2D game) : base(game)
        {
            _ninePatchTexture = Game.Content.Load<Texture2D>("Textures/border");
            _ninePatchParameter = new NinePatchParameter(BorderSize);
        }

        public int ClientX {get { return X + BorderSize; }}
        public int ClientY { get { return Y + BorderSize; } }
        public Vector2 ClientStart{ get { return new Vector2(ClientX, ClientY); }}

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public void SetClientSize(int width, int height)
        {
            Width = width + 2 * BorderSize;
            Height = height + 2 * BorderSize;
        }

        public void CenterHorizontal()
        {
            X = Game.ScreenWidth/2 - Width/2;
        }

        public void Draw()
        {
            Draw(Color.White);
        }

        public void Draw(Color color)
        {
            Game.SpriteBatch.DrawNinePatch(_ninePatchTexture,
                                          new Rectangle(0, 0, _ninePatchTexture.Width, _ninePatchTexture.Height),
                                          new Rectangle(X, Y, Width, Height),
                                          color,
                                          _ninePatchParameter);
        }
    }
}
