
namespace Game2DFramework.Drawing.Particles
{
    interface IParticleEmitter
    {
        Particle Update(float elapsed);
        void UpdateParticle(float elapsed, Particle particle);
    }
}
