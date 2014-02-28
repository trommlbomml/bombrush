
using Microsoft.Xna.Framework;

namespace Game2DFramework.Drawing.Particles
{
    class Particle
    {
        public float LifeTime;
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Alpha;
        public float Rotation;
        public float Scale;

        public Particle(Vector2 position, float lifeTime)
        {
            Position = position;
            LifeTime = lifeTime;
            Color = Color.White;
            Alpha = 1.0f;
            Scale = 1.0f;
        }

        public bool Dead { get { return LifeTime <= 0.0f; } }
    }
}
