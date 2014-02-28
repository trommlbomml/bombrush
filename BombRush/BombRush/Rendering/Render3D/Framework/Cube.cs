
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render3D.Framework
{
    class Cube
    {
        private const int VertexCount = 24;
        private const int TriangleCount = 12;

        private readonly GraphicsDevice _device;
        private readonly VertexBuffer _vertexBuffer;
        private readonly IndexBuffer _indexBuffer;
        private readonly Texture2D _texture;

        public Cube(GraphicsDevice graphicsDevice, Texture2D texture)
        {
            _texture = texture;
            _device = graphicsDevice;
            _vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, VertexCount, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(GeometryCreator.GenerateCube());
            _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, TriangleCount * 3, BufferUsage.WriteOnly);
            _indexBuffer.SetData(GeometryCreator.GenerateQuadIndices(6));
        }

        public void Draw(Effect effect)
        {
            _device.SetVertexBuffer(_vertexBuffer);
            _device.Indices = _indexBuffer;

            effect.Parameters["DiffuseTexture"].SetValue(_texture);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VertexCount, 0, TriangleCount);
            }
        }
    }
}
