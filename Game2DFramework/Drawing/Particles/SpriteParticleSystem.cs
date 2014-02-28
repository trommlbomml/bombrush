using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2DFramework.Drawing.Particles
{
    class SpriteParticleSystem : ParticleSystem
    {
        public SpriteParticleSystem(IParticleEmitter emitter, 
                                     Texture2D texture, 
                                     Rectangle? sourceRectangle = null, 
                                     Vector2 origin = default(Vector2)) 
            : base(emitter, new SpriteParticleRenderer(texture, sourceRectangle, origin))
        {}
    }
}
