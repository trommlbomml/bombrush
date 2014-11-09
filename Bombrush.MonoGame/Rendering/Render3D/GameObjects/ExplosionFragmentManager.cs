
using System.Collections.Generic;
using System.Linq;
using BombRush.Rendering.Render3D.Framework;
using BombRush.Interfaces;
using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render3D.GameObjects
{
    class ExplosionFragmentManager
    {
        private readonly ILevelDataProvider _level;
        private readonly Texture _texture;
        private readonly Effect _particleEffect;
        private readonly EffectParameter _worldViewProjParameter;
        private readonly EffectParameter _textureParameter;
        private readonly List<ExplosionFragment> _instances;
        private readonly VertexBuffer _quadVertexBuffer;

        public ExplosionFragmentManager(Game2D game, ILevelDataProvider level)
        {
            _level = level;
            _texture = game.Content.Load<Texture2D>("Textures/explosionparticle");
            _particleEffect = game.Content.Load<Effect>("Effects/ParticleEffect");
            _worldViewProjParameter = _particleEffect.Parameters["WorldViewProjection"];
            _textureParameter = _particleEffect.Parameters["DiffuseTexture"];
            _instances = new List<ExplosionFragment>();
            _quadVertexBuffer = new VertexBuffer(game.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);
            _quadVertexBuffer.SetData(GeometryCreator.GenerateQuad(QuadOrientation.UnitY, Vector3.Zero));
        }

        public void Update(Render3DContext context, float elapsedTime)
        {
            foreach (var explosionFragment in _level.OverlayData)
            {
                if (explosionFragment.IsActive && _instances.All(i => i.TilePosition != explosionFragment.TilePosition))
                {
                    SpawnParticle(explosionFragment);
                }   
            }

            _instances.ForEach(i => i.Update(context.CameraPosition, elapsedTime));
            _instances.RemoveAll(p => p.IsDead);
        }

        private void SpawnParticle(IExplosionFragmentDataProvider explosionFragment)
        {
            var instance = new ExplosionFragment(explosionFragment.TilePosition,
                                                new Vector3(explosionFragment.TilePosition.X + 0.5f, 0.5f, explosionFragment.TilePosition.Y + 0.5f));
            _instances.Add(instance);
        }

        public void Render(Render3DContext context)
        {
            var device = _particleEffect.GraphicsDevice;
            device.BlendState = BlendState.AlphaBlend;
            device.DepthStencilState = DepthStencilState.DepthRead;
            device.SetVertexBuffer(_quadVertexBuffer);

            _textureParameter.SetValue(_texture);
            
            foreach (var fragmentInstance in _instances)
            {
                _worldViewProjParameter.SetValue(fragmentInstance.World * context.View * context.Projection);
                _particleEffect.CurrentTechnique.Passes[0].Apply();
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            }

            device.DepthStencilState = DepthStencilState.Default;
        }
    }
}
