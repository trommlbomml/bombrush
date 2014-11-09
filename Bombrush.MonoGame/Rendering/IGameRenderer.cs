using BombRush.Interfaces;
using Game2DFramework;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.Rendering
{
    interface IGameRenderer
    {
        void Initialize(Game2D game, Level levelDataProvider);
        void Update(float elapsedTime);
        void Render(float elapsedTime, SpriteBatch spriteBatch, RenderTarget2D currentTransitionRenderTarget = null);
    }
}
