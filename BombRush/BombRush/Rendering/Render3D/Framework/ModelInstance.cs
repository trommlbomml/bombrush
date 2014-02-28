
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render3D.Framework
{
    class ModelInstance : IRenderObject
    {
        private readonly ModelGeometry _geometry;

        public ModelInstance(ModelGeometry geometry)
        {
            _geometry = geometry;
            IsActive = true;
        }

        public bool IsActive { get; set; }
        public Matrix World { get; set; }

        public void Render(Render3DContext context, Effect renderEffect, string techniqueName)
        {
            _geometry.Render(World, context, renderEffect, techniqueName);
        }
    }
}
