using Microsoft.Xna.Framework;

namespace BombRush.Rendering.Render3D.GameObjects
{
    class ExplosionFragment
    {
        public const float TotalLifeTime = 0.5f;

        public Point TilePosition { get; set; }
        public Vector3 Position { get; set; }
        public float LifeTime { get; set; }
        public float Scale { get; set; }

        public Matrix World { get; private set; }

        public ExplosionFragment(Point tilePosition, Vector3 position)
        {
            TilePosition = tilePosition;
            Position = position;
            LifeTime = TotalLifeTime;
        }

        public void Update(Vector3 cameraPosition, float elapsed)
        {
            LifeTime -= elapsed;
            var delta = (TotalLifeTime - LifeTime) / TotalLifeTime;
            Scale = MathHelper.SmoothStep(0.0f, 1.5f, delta);
            World = Matrix.CreateScale(Scale) * Matrix.CreateRotationZ(MathHelper.TwoPi * 4 * delta) * Matrix.CreateRotationX(MathHelper.PiOver4) * Matrix.CreateTranslation(Position);
        }

        public bool IsDead
        {
            get { return LifeTime <= 0; }
        }
    }
}