using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2DFramework.Drawing.Particles
{
    class SpriteParticleRenderer : IParticleRenderer
    {
        private readonly Texture2D _texture;
        private readonly Rectangle? _sourceRectangle;
        private readonly Vector2 _origin;
        
        public SpriteParticleRenderer(Texture2D texture, 
                                      Rectangle? sourceRectangle = null, 
                                      Vector2 origin = default(Vector2))
        {
            _texture = texture;
            _sourceRectangle = sourceRectangle;
            _origin = origin;
        }

        public void Draw(SpriteBatch spriteBatch, Particle particle)
        {
            spriteBatch.Draw(_texture, 
                             particle.Position, 
                             _sourceRectangle, 
                             particle.Color * particle.Alpha, 
                             particle.Rotation, 
                             _origin, 
                             particle.Scale, 
                             SpriteEffects.None, 
                             0);
        }
    }
}
