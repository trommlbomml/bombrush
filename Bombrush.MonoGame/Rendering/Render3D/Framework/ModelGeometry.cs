
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render3D.Framework
{
    class ModelGeometry
    {
        private readonly Game3DRenderer _renderer;
        private readonly List<ModelInstance> _existingInstances;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Model _model;

        public ModelGeometry(Game3DRenderer renderer, GraphicsDevice graphicsDevice, Model model, Effect celShadingEffect)
        {
            _renderer = renderer;
            _existingInstances = new List<ModelInstance>();
            _graphicsDevice = graphicsDevice;
            _model = model;
            ChangeEffectUsedByModel(model, celShadingEffect);
        }

        public void ResetActives()
        {
            _existingInstances.ForEach(m => m.IsActive = false);
        }

        public ModelInstance CreateInstance()
        {
            var freeInstance = _existingInstances.FirstOrDefault(m => !m.IsActive);
            if (freeInstance != null)
            {
                freeInstance.IsActive = true;
                return freeInstance;   
            }

            freeInstance = new ModelInstance(this);
            _existingInstances.Add(freeInstance);
            _renderer.AddRenderObject(freeInstance);
            return freeInstance;
        }

        static void ChangeEffectUsedByModel(Model model, Effect replacementEffect)
        {
            var effectMapping = new Dictionary<Effect, Effect>();

            foreach (var mesh in model.Meshes)
            {
                foreach (var oldEffect in mesh.Effects.OfType<BasicEffect>())
                {
                    if (effectMapping.ContainsKey(oldEffect)) continue;
                    
                    var newEffect = replacementEffect.Clone();
                    newEffect.Parameters["CelTexture"].SetValue(replacementEffect.Parameters["CelTexture"].GetValueTexture2D());
                    if (oldEffect.TextureEnabled && oldEffect.Texture != null) newEffect.Parameters["DiffuseTexture"].SetValue(oldEffect.Texture);
                    effectMapping.Add(oldEffect, newEffect);
                }

                if (effectMapping.Count <= 0) continue;
                
                foreach (var meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effectMapping[meshPart.Effect];
                }
            }
        }

        public void Render(Matrix world, Render3DContext context, Effect renderEffect, string techniqueName)
        {
            _graphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;

            var transforms = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (var mesh in _model.Meshes)
            {
                foreach (var effect in mesh.Effects)
                {
                    effect.CurrentTechnique = effect.Techniques[techniqueName];
                    effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] *world);
                    effect.Parameters["View"].SetValue(context.View);
                    effect.Parameters["Projection"].SetValue(context.Projection);
                    effect.Parameters["LightDir"].SetValue(context.LightDir);
                    effect.Parameters["LightMatrix"].SetValue(context.LightMatrix);
                    if (techniqueName != "ShadowDepth") effect.Parameters["ShadowMap"].SetValue(context.NormalLightDepthRenderTarget);
                }
                mesh.Draw();
            }
        }
    }
}
