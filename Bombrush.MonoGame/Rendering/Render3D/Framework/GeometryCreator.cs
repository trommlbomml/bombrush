using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render3D.Framework
{
    enum QuadOrientation
    {
        UnitX,
        UnitY,
        UnitZ,
        InvUnitX,
        InvUnitY,
        InvUnitZ,
    }

    static class GeometryCreator
    {
        public static VertexPositionNormalTexture[] GenerateQuad(QuadOrientation orientation, Vector3 offset, float textureScaleX = 1.0f, float textureScaleY = 1.0f)
        {
            switch (orientation)
            {
                case QuadOrientation.UnitX: return GenerateQuadUnitX(offset, textureScaleX, textureScaleY);
                case QuadOrientation.UnitY: return GenerateQuadUnitY(offset, textureScaleX, textureScaleY);
                case QuadOrientation.UnitZ: return GenerateQuadUnitZ(offset, textureScaleX, textureScaleY);
                case QuadOrientation.InvUnitX: return GenerateQuadInvUnitX(offset, textureScaleX, textureScaleY);
                case QuadOrientation.InvUnitY: return GenerateQuadInvUnitY(offset, textureScaleX, textureScaleY);
                case QuadOrientation.InvUnitZ: return GenerateQuadInvUnitZ(offset, textureScaleX, textureScaleY);
            }

            throw new InvalidOperationException("Invalid Geometry");
        }

        public static VertexPositionNormalTexture[] GenerateCube()
        {
            List<VertexPositionNormalTexture> vertexPositionNormalTextures = new List<VertexPositionNormalTexture>();
            vertexPositionNormalTextures.AddRange(GenerateQuadUnitX(new Vector3(0.5f, 0.0f, 0.0f)));
            vertexPositionNormalTextures.AddRange(GenerateQuadUnitY(new Vector3(0.0f, 0.5f, 0.0f)));
            vertexPositionNormalTextures.AddRange(GenerateQuadUnitZ(new Vector3(0.0f, 0.0f, 0.5f)));
            vertexPositionNormalTextures.AddRange(GenerateQuadInvUnitX(new Vector3(-0.5f, 0.0f, 0.0f)));
            vertexPositionNormalTextures.AddRange(GenerateQuadInvUnitY(new Vector3(0.0f, -0.5f, 0.0f)));
            vertexPositionNormalTextures.AddRange(GenerateQuadInvUnitZ(new Vector3(0.0f, 0.0f, -0.5f)));
            return vertexPositionNormalTextures.ToArray();
        }

        public static short[] GenerateQuadIndices(int countQuads)
        {
            short[] indices = new short[countQuads * 6];
            for (short i = 0; i < countQuads; i++)
            {
                indices[i * 6 + 0] = (short)(i * 4 + 0);
                indices[i * 6 + 1] = (short)(i * 4 + 1);
                indices[i * 6 + 2] = (short)(i * 4 + 2);
                indices[i * 6 + 3] = (short)(i * 4 + 1);
                indices[i * 6 + 4] = (short)(i * 4 + 3);
                indices[i * 6 + 5] = (short)(i * 4 + 2);
            }
            return indices;
        }

        private static VertexPositionNormalTexture[] GenerateQuadUnitX(Vector3 offset, float textureScaleX = 1.0f, float textureScaleY = 1.0f)
        {
            return new[]
            {
                new VertexPositionNormalTexture(new Vector3( 0.0f,-0.5f, 0.5f) + offset, Vector3.UnitX, new Vector2(0,textureScaleY)), 
                new VertexPositionNormalTexture(new Vector3( 0.0f, 0.5f, 0.5f) + offset, Vector3.UnitX, new Vector2(0,0)), 
                new VertexPositionNormalTexture(new Vector3( 0.0f,-0.5f,-0.5f) + offset, Vector3.UnitX, new Vector2(textureScaleX,textureScaleY)), 
                new VertexPositionNormalTexture(new Vector3( 0.0f, 0.5f,-0.5f) + offset, Vector3.UnitX, new Vector2(textureScaleX,0)), 
            };
        }

        private static VertexPositionNormalTexture[] GenerateQuadInvUnitX(Vector3 offset, float textureScaleX = 1.0f, float textureScaleY = 1.0f)
        {
            return new[]
            {
                new VertexPositionNormalTexture(new Vector3( 0.0f,-0.5f,-0.5f) + offset,-Vector3.UnitX, new Vector2(0,textureScaleY)), 
                new VertexPositionNormalTexture(new Vector3( 0.0f, 0.5f,-0.5f) + offset,-Vector3.UnitX, new Vector2(0,0)), 
                new VertexPositionNormalTexture(new Vector3( 0.0f,-0.5f, 0.5f) + offset,-Vector3.UnitX, new Vector2(textureScaleX,textureScaleY)), 
                new VertexPositionNormalTexture(new Vector3( 0.0f, 0.5f, 0.5f) + offset,-Vector3.UnitX, new Vector2(textureScaleX,0)), 
            };
        }

        private static VertexPositionNormalTexture[] GenerateQuadUnitZ(Vector3 offset, float textureScaleX = 1.0f, float textureScaleY = 1.0f)
        {
            return new[]
            {
                new VertexPositionNormalTexture(new Vector3(-0.5f,-0.5f, 0.0f) + offset, Vector3.UnitZ, new Vector2(0,textureScaleY)), 
                new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.0f) + offset, Vector3.UnitZ, new Vector2(0,0)), 
                new VertexPositionNormalTexture(new Vector3( 0.5f,-0.5f, 0.0f) + offset, Vector3.UnitZ, new Vector2(textureScaleX,textureScaleY)), 
                new VertexPositionNormalTexture(new Vector3( 0.5f, 0.5f, 0.0f) + offset, Vector3.UnitZ, new Vector2(textureScaleX,0)), 
            };
        }

        private static VertexPositionNormalTexture[] GenerateQuadInvUnitZ(Vector3 offset, float textureScaleX = 1.0f, float textureScaleY = 1.0f)
        {
            return new[]
            {
                new VertexPositionNormalTexture(new Vector3( 0.5f,-0.5f, 0.0f) + offset, -Vector3.UnitZ, new Vector2(0,textureScaleY)), 
                new VertexPositionNormalTexture(new Vector3( 0.5f, 0.5f, 0.0f) + offset, -Vector3.UnitZ, new Vector2(0,0)), 
                new VertexPositionNormalTexture(new Vector3(-0.5f,-0.5f, 0.0f) + offset, -Vector3.UnitZ, new Vector2(textureScaleX,textureScaleY)), 
                new VertexPositionNormalTexture(new Vector3(-0.5f, 0.5f, 0.0f) + offset, -Vector3.UnitZ, new Vector2(textureScaleX,0)), 
            };
        }

        private static VertexPositionNormalTexture[] GenerateQuadUnitY(Vector3 offset, float textureScaleX = 1.0f, float textureScaleY = 1.0f)
        {
            return new[]
            {
                new VertexPositionNormalTexture(new Vector3(-0.5f, 0.0f, 0.5f) + offset, Vector3.UnitY, new Vector2(0,textureScaleY)), 
                new VertexPositionNormalTexture(new Vector3(-0.5f, 0.0f,-0.5f) + offset, Vector3.UnitY, new Vector2(0,0)), 
                new VertexPositionNormalTexture(new Vector3( 0.5f, 0.0f, 0.5f) + offset, Vector3.UnitY, new Vector2(textureScaleX,textureScaleY)), 
                new VertexPositionNormalTexture(new Vector3( 0.5f, 0.0f,-0.5f) + offset, Vector3.UnitY, new Vector2(textureScaleX,0)), 
            };
        }

        private static VertexPositionNormalTexture[] GenerateQuadInvUnitY(Vector3 offset, float textureScaleX = 1.0f, float textureScaleY = 1.0f)
        {
            return new[]
            {
                new VertexPositionNormalTexture(new Vector3( 0.5f, 0.0f, 0.5f) + offset,-Vector3.UnitY, new Vector2(0,textureScaleY)), 
                new VertexPositionNormalTexture(new Vector3( 0.5f, 0.0f,-0.5f) + offset,-Vector3.UnitY, new Vector2(0,0)), 
                new VertexPositionNormalTexture(new Vector3(-0.5f, 0.0f, 0.5f) + offset,-Vector3.UnitY, new Vector2(textureScaleX,textureScaleY)), 
                new VertexPositionNormalTexture(new Vector3(-0.5f, 0.0f,-0.5f) + offset,-Vector3.UnitY, new Vector2(textureScaleX,0)), 
            };
        }
    }
}
