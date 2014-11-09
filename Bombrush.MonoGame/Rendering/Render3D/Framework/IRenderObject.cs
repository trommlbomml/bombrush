
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render3D.Framework
{
    interface IRenderObject
    {
        bool IsActive { get; set; }
        Matrix World { get; set; }
        void Render(Render3DContext context, Effect renderEffect, string techniqueName);
    }
}
