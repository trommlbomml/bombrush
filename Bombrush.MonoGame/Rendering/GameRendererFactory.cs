using System;
using BombRush.Interfaces;
using Bombrush.MonoGame.Rendering.Render2D;
using Game2DFramework;

namespace Bombrush.MonoGame.Rendering
{
    static class GameRendererFactory
    {
        public static IGameRenderer CreateGameRenderer(GameRendererType renderType, Game2D game, Level level)
        {
            IGameRenderer renderer;
            switch (renderType)
            {
                case GameRendererType.TwoDe:
                    renderer = new Game2DRenderer(game);
                    break;
                case GameRendererType.ThreeDe:
                    throw new NotImplementedException();
                    //renderer = new Game3DRenderer();
                    break;
                default:
                    throw new InvalidOperationException("Invalid Renderer Type");
            }
            renderer.Initialize(game, level);

            return renderer;
        }
    }
}
