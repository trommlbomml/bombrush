
using System.Collections.Generic;

using BombRush.Rendering.Render3D.Framework;
using BombRush.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render3D.GameObjects
{
    class Level : IRenderObject
    {
        private readonly Cube _boxCube;
        private readonly GraphicsDevice _device;
        private readonly Texture2D _wallTexture;
        private readonly Texture2D _grassTexture;
        private VertexBuffer _vertexBuffer;
        private readonly VertexBuffer _groundVertexBuffer;
        private IndexBuffer _indexBuffer;
        private int _vertexCount;
        private int _primitiveCount;
        private readonly ILevelDataProvider _levelData;

        public Level(GraphicsDevice device, ContentManager content, ILevelDataProvider levelDataProvider)
        {
            _levelData = levelDataProvider;
            _device = device;
            _wallTexture = content.Load<Texture2D>("Textures/wall");
            _boxCube = new Cube(device, content.Load<Texture2D>("Textures/box"));
            _grassTexture = content.Load<Texture2D>("Textures/grass");

            _groundVertexBuffer = new VertexBuffer(_device, VertexPositionNormalTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);
            _groundVertexBuffer.SetData(GeometryCreator.GenerateQuad(QuadOrientation.UnitY, Vector3.Zero, 15, 13));

            GenerateContent();
        }

        public bool IsActive { get { return true; } set{ } }

        private void GenerateContent()
        {
            if (_vertexBuffer != null && !_vertexBuffer.IsDisposed)
                _vertexBuffer.Dispose();
            if (_indexBuffer != null && !_indexBuffer.IsDisposed)
                _indexBuffer.Dispose();

            var vertices = new List<VertexPositionNormalTexture>();
            for (int z = 0; z < 13; z++)
            {
                for (int x = 0; x < 15; x++)
                {
                    ITileBlockDataProvider data = _levelData.Data[z*15 + x];
                    if (data.Type == BlockType.Wall)
                    {
                        vertices.AddRange(GeometryCreator.GenerateQuad(QuadOrientation.UnitY, new Vector3(0.5f + x, 1.0f, 0.5f + z)));

                        if (x == 0 || _levelData.Data[z * BombGame.GameLevelWidth + x - 1].Type != BlockType.Wall)
                            vertices.AddRange(GeometryCreator.GenerateQuad(QuadOrientation.InvUnitX, new Vector3(x, 0.5f, 0.5f + z)));

                        if (x == BombGame.GameLevelWidth-1 || _levelData.Data[z * BombGame.GameLevelWidth + x + 1].Type != BlockType.Wall)
                            vertices.AddRange(GeometryCreator.GenerateQuad(QuadOrientation.UnitX, new Vector3(x + 1, 0.5f, 0.5f + z)));

                        if (z == BombGame.GameLevelHeight-1 || _levelData.Data[(z + 1) * BombGame.GameLevelWidth + x].Type != BlockType.Wall)
                            vertices.AddRange(GeometryCreator.GenerateQuad(QuadOrientation.UnitZ, new Vector3(x + 0.5f, 0.5f, 1.0f + z)));

                        if (z == 0 || _levelData.Data[(z - 1) * BombGame.GameLevelWidth + x].Type != BlockType.Wall)
                            vertices.AddRange(GeometryCreator.GenerateQuad(QuadOrientation.InvUnitZ, new Vector3(x + 0.5f, 0.5f, z)));
                    }
                }
            }

            _vertexCount = vertices.Count;
            _primitiveCount = _vertexCount / 2;

            _vertexBuffer = new VertexBuffer(_device, VertexPositionNormalTexture.VertexDeclaration, _vertexCount, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices.ToArray());

            short[] indices = new short[_primitiveCount * 3];
            for (short i = 0; i < _primitiveCount / 2; i++)
            {
                indices[i * 6 + 0] = (short)(i * 4 + 0);
                indices[i * 6 + 1] = (short)(i * 4 + 1);
                indices[i * 6 + 2] = (short)(i * 4 + 2);
                indices[i * 6 + 3] = (short)(i * 4 + 1);
                indices[i * 6 + 4] = (short)(i * 4 + 3);
                indices[i * 6 + 5] = (short)(i * 4 + 2);
            }
            _indexBuffer = new IndexBuffer(_device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(indices);
        }

        public Matrix World
        {
            get { return Matrix.Identity; } set {}
        }

        public void Render(Render3DContext context, Effect renderEffect, string techniqueName)
        {
            renderEffect.CurrentTechnique = renderEffect.Techniques[techniqueName];
            renderEffect.Parameters["View"].SetValue(context.View);
            renderEffect.Parameters["Projection"].SetValue(context.Projection);
            renderEffect.Parameters["LightMatrix"].SetValue(context.LightMatrix);
            if (techniqueName != "ShadowDepth") renderEffect.Parameters["ShadowMap"].SetValue(context.NormalLightDepthRenderTarget);

            var worldParameter = renderEffect.Parameters["World"];

            _device.SetVertexBuffer(_groundVertexBuffer);
            renderEffect.Parameters["DiffuseTexture"].SetValue(_grassTexture);
            worldParameter.SetValue(Matrix.CreateScale(13, 1, 11) * Matrix.CreateTranslation(7.5f, 0.0f, 6.5f));
            foreach (var pass in renderEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            }

            _device.SetVertexBuffer(_vertexBuffer);
            _device.Indices = _indexBuffer;
            renderEffect.Parameters["DiffuseTexture"].SetValue(_wallTexture);
            worldParameter.SetValue(Matrix.Identity);
            foreach (var pass in renderEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertexCount, 0, _primitiveCount);
            }

            for (var z = 0; z < 13; z++)
            {
                for (var x = 0; x < 15; x++)
                {
                    var fringe = _levelData.Fringe[z * 15 + x];
                    if (fringe.IsActive && fringe.Type == BlockType.Box)
                    {
                        worldParameter.SetValue(Matrix.CreateScale(0.8f, 1.0f, 0.8f) * Matrix.CreateTranslation(x + 0.5f, 0.5f, z + 0.5f));
                        _boxCube.Draw(renderEffect);
                    }
                }
            }
        }
    }
}
