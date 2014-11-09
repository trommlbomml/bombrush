
using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render3D.Framework
{
    class Render3DContext
    {
        private readonly Texture2D _celTexture;

        public RenderTarget2D SceneRenderTarget { get; private set; }
        public RenderTarget2D NormalLightDepthRenderTarget { get; private set; }
        public Vector3 CameraPosition { get; private set; }
        public Vector3 LightPosition { get; private set; }
        public Vector3 LightLookAt { get; private set; }
        public Vector3 LightDir { get; private set; }
        public Effect ToonEffect { get; private set; }
        public Effect PostProcessingEffect { get; private set; }
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }
        public Matrix LightMatrix { get; private set; }
        public RenderTarget2D NormalDepthRenderTarget { get; private set; }
        
        public Render3DContext(Game2D game)
        {
            ToonEffect = game.Content.Load<Effect>("Effects/BombRushEffects");
            PostProcessingEffect = game.Content.Load<Effect>("Effects/PostProcessing");
            _celTexture = game.Content.Load<Texture2D>("Textures/Cel");
            ToonEffect.Parameters["CelTexture"].SetValue(_celTexture);

            var pp = game.GraphicsDevice.PresentationParameters;
            var width = pp.BackBufferWidth;
            var height = pp.BackBufferHeight;
            var depthFormat = pp.DepthStencilFormat;
            var bbFormat = pp.BackBufferFormat;

            NormalDepthRenderTarget = new RenderTarget2D(game.GraphicsDevice, width, height, false, bbFormat, depthFormat);
            SceneRenderTarget = new RenderTarget2D(game.GraphicsDevice, width, height, false, bbFormat, depthFormat);
            NormalLightDepthRenderTarget = new RenderTarget2D(game.GraphicsDevice, 1024, 1024, false, SurfaceFormat.Single, depthFormat);

            LightLookAt = new Vector3(7.5f, 0.0f, 6.5f);
            LightPosition = new Vector3(20, 20, 20) * 0.75f;
            CameraPosition = new Vector3(7.5f, 16.0f, 13.0f);
            View = Matrix.CreateLookAt(CameraPosition, new Vector3(7.5f, 0.0f, 6.5f), Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _celTexture.GraphicsDevice.Viewport.AspectRatio, 8.0f, 50.0f);
        }

        public void Update()
        {
            LightDir = LightLookAt - LightPosition;
            LightMatrix = Matrix.CreateLookAt(LightPosition, LightLookAt, Vector3.Up) * Matrix.CreateOrthographic(30, 30, 1.0f, 40.0f);

            ToonEffect.Parameters["LightMatrix"].SetValue(LightMatrix);
        }

        public void PreparePostProcessingEdgeDetection()
        {
            var parameters = PostProcessingEffect.Parameters;
            parameters["ScreenResolution"].SetValue(new Vector2(SceneRenderTarget.Width, SceneRenderTarget.Height));
            parameters["NormalDepthTexture"].SetValue(NormalDepthRenderTarget);
            PostProcessingEffect.CurrentTechnique = PostProcessingEffect.Techniques["EdgeDetect"];
        }

        public void PrepareCelShadingWithShadows()
        {
            ToonEffect.Parameters["LightDir"].SetValue(LightDir);
            ToonEffect.Parameters["ShadowMap"].SetValue(NormalLightDepthRenderTarget);
        }
    }
}
