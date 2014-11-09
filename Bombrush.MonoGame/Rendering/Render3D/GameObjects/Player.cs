
using BombRush.Rendering.Render3D.Framework;
using BombRush.Interfaces;
using Microsoft.Xna.Framework;

namespace BombRush.Rendering.Render3D.GameObjects
{
    class Player : ModelInstance
    {
        private readonly IFigureDataProvider _figure;

        public Player(IFigureDataProvider figure, ModelGeometry geometry) : base(geometry)
        {
            _figure = figure;
        }

        public void Update()
        {
            IsActive = _figure.IsAlive;
            if (!IsActive) return;

            float angle = 0;
            switch(_figure.Direction)
            {
                case FigureDirection.Right:
                    angle = MathHelper.PiOver2;
                    break;
                case FigureDirection.Up:
                    angle = MathHelper.Pi;
                    break;
                    case FigureDirection.Left:
                    angle = -MathHelper.PiOver2;
                    break;
            }

            var pos = _figure.Position / 32.0f;
            World = Matrix.CreateRotationY(angle)* Matrix.CreateScale(0.008f) * Matrix.CreateTranslation(pos.X, 0, pos.Y);
        }
    }
}
