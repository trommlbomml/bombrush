
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render3D.Framework
{
    struct VertexPositionColorNormal : IVertexType
    {
        public Vector3 Position;
        public Color Color;
        public Vector3 Normal;

        public static readonly VertexDeclaration TheVertexDeclaration = new VertexDeclaration(new[]
        {
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(3 * sizeof(float), VertexElementFormat.Color, VertexElementUsage.Color, 0), 
            new VertexElement(3 * sizeof(float) + sizeof(Int32), VertexElementFormat.Vector3, VertexElementUsage.Normal,0), 
        });

        public VertexDeclaration VertexDeclaration
        {
            get { return TheVertexDeclaration; }
        }

        public VertexPositionColorNormal(Vector3 position, Color color, Vector3 normal)
        {
            Position = position;
            Color = color;
            Normal = normal;
        }
    }
}
