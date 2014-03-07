using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game2DFramework.States.Transitions
{
    public class ZappoutTransition : ITransition
    {
        public static readonly Type Id = typeof(ZappoutTransition);

        private readonly GraphicsDevice _device;
        private readonly BasicEffect _basicEffect;
        private float _elapsedTime;
        private const float AnimationTime = 0.5f;
        private readonly VertexBuffer _vertexBuffer;

        public Texture2D Source { get; set; }
        public Texture2D Target { get; set; }
        public bool TransitionReady { get; set; }

        public ZappoutTransition(GraphicsDevice device)
        {
            _device = device;

            _basicEffect = new BasicEffect(device)
            {
                LightingEnabled = false,
                TextureEnabled = true,
                Projection = Matrix.CreatePerspective(2.0f, 2.0f, 1.0f, 1000.0f),
                View = Matrix.Identity
            };

            var vertices = new[]
            {
                new VertexPositionTexture(new Vector3(-1,-1,0),new Vector2(0,1)), 
                new VertexPositionTexture(new Vector3(-1, 1,0),new Vector2(0,0)), 
                new VertexPositionTexture(new Vector3( 1,-1,0),new Vector2(1,1)), 
                new VertexPositionTexture(new Vector3( 1, 1,0),new Vector2(1,0)),
            };

            _vertexBuffer = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);
        }

        public void Begin()
        {
            TransitionReady = false;
            _elapsedTime = 0;
        }

        public void Update(float elapsedTime)
        {
            _elapsedTime += elapsedTime;
            if (_elapsedTime > AnimationTime)
            {
                _elapsedTime = AnimationTime;
                TransitionReady = true;
            }

            var delta = _elapsedTime / AnimationTime;
            var scaling = MathHelper.SmoothStep(0.0f, 1.001f, delta);

            _basicEffect.World = Matrix.CreateScale(1.0f, 1.0f-scaling, 1.0f) * Matrix.CreateTranslation(0, 0, -1.0f);
        }

        public void Render(SpriteBatch spriteBatch)
        {
            _device.Clear(Color.Black);
            _device.RasterizerState = RasterizerState.CullCounterClockwise;

            _device.SetVertexBuffer(_vertexBuffer);

            _basicEffect.Texture = Source;
            foreach (var pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            }
        }
    }
}
