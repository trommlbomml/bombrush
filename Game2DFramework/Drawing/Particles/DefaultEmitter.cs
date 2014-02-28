using System;
using Microsoft.Xna.Framework;

namespace Game2DFramework.Drawing.Particles
{
    class DefaultEmitter : IParticleEmitter
    {
        private readonly Random _random;
        private float _elapsedTime;

        public float SpawnTime;
        public Vector2 SpawnPosition;
        public float LifeTime;
        public float Speed;
        public float RotationSpeed;

        private float _sourceAlpha;
        private float _targetAlpha;

        private float _angleFrom;
        private float _angleTo;

        private float _scaleFrom;
        private float _scaleTo;

        public DefaultEmitter()
        {
            _random = new Random();
            Alpha = 1.0f;
            Scale = 1.0f;
            Angle = 0;
        }

        public float Alpha { set { _sourceAlpha = _targetAlpha = value; } }
        public void SetAlpha(float source, float target)
        {
            _sourceAlpha = source;
            _targetAlpha = target;
        }

        public float Angle { set { _angleFrom = _angleTo = value; } }
        public void SetAngleRange(float from, float to)
        {
            _angleFrom = from;
            _angleTo = to;
        }

        public float Scale { set { _scaleFrom = _scaleTo = value; } }
        public void SetScaleRange(float from, float to)
        {
            _scaleFrom = from;
            _scaleTo = to;
        }

        public Particle Update(float elapsed)
        {
            _elapsedTime += elapsed;
            if (_elapsedTime >= SpawnTime)
            {
                _elapsedTime -= SpawnTime;
                float angle = (float) (_random.NextDouble()*(_angleTo - _angleFrom) + _angleFrom);
                return new Particle(SpawnPosition, LifeTime)
                           {
                               Alpha = _sourceAlpha,
                               Velocity = new Vector2((float)Math.Cos(angle), (float)(Math.Sin(angle))) * Speed,
                               Scale = _scaleFrom,
                           };
            }

            return null;
        }

        public void UpdateParticle(float elapsed, Particle particle)
        {
            float delta = (LifeTime - particle.LifeTime)/LifeTime;

            particle.LifeTime -= elapsed;
            particle.Position += particle.Velocity*elapsed;
            particle.Alpha = MathHelper.Lerp(_sourceAlpha, _targetAlpha, delta);
            particle.Scale = MathHelper.Lerp(_scaleFrom, _scaleTo, delta);
            particle.Rotation += RotationSpeed*elapsed;
        }
    }
}
