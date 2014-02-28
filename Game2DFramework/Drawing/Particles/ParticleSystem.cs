using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Game2DFramework.Drawing.Particles
{
    class ParticleSystem
    {
        private readonly List<Particle> _particles;
        private readonly IParticleEmitter _emitter;
        private readonly IParticleRenderer _renderer;

        public ParticleSystem(IParticleEmitter emitter, IParticleRenderer renderer)
        {
            _particles = new List<Particle>();
            _emitter = emitter;
            _renderer = renderer;
        }

        public void Update(float elapsedTime)
        {
            Particle particle = _emitter.Update(elapsedTime);
            if (particle != null)
                _particles.Insert(0, particle);

            _particles.RemoveAll(p => p.Dead);
            _particles.ForEach(p => _emitter.UpdateParticle(elapsedTime, p));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _particles.ForEach(p => _renderer.Draw(spriteBatch, p));
        }
    }
}
