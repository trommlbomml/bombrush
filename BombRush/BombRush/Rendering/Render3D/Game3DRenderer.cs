
using System;
using System.Collections.Generic;
using System.Linq;
using BombRush.Rendering.Render3D.Framework;
using BombRush.Rendering.Render3D.GameObjects;
using BombRush.Interfaces;
using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render3D
{
    class Game3DRenderer : IGameRenderer
    {
        private GameInformation _gameInformation;

        private Render3DContext _context;
        private GraphicsDevice _graphicsDevice;
        private List<IRenderObject> _objectsToRender;
        private ModelGeometry _bombGeometry;
        private ModelGeometry _baseItemGeometry;
        private ModelGeometry _fireOverlayGeometry;
        private ILevelDataProvider _levelData;
        private ModelGeometry _playerGeometry;
        private List<Player> _players;
        private ExplosionFragmentManager _explosionFragmentManager;

        public void AddRenderObject(IRenderObject renderObject)
        {
            _objectsToRender.Add(renderObject);
        }

        public void Initialize(Game2D game, ILevelDataProvider levelDataProvider)
        {
            _gameInformation = new GameInformation(game, levelDataProvider) { Width = BombGame.Tilesize * 15 };

            _graphicsDevice = game.GraphicsDevice;
            _levelData = levelDataProvider;
            _context = new Render3DContext(game);
            _objectsToRender = new List<IRenderObject> {new Level(game.GraphicsDevice, game.Content, levelDataProvider)};
            _bombGeometry = new ModelGeometry(this, game.GraphicsDevice, game.Content.Load<Model>("Models/Bomb/BombMesh"), _context.ToonEffect);
            _playerGeometry = new ModelGeometry(this, game.GraphicsDevice, game.Content.Load<Model>("Models/Player/bomberman"), _context.ToonEffect);
            _baseItemGeometry = new ModelGeometry(this, game.GraphicsDevice, game.Content.Load<Model>("Models/items/itemBase"), _context.ToonEffect);
            _fireOverlayGeometry = new ModelGeometry(this, game.GraphicsDevice, game.Content.Load<Model>("Models/items/fireoverlay"), _context.ToonEffect);
            _explosionFragmentManager = new ExplosionFragmentManager(game, _levelData);

            _players = new List<Player>();
            foreach (var figure in _levelData.Figures)
            {
                var player = new Player(figure, _playerGeometry);
                AddRenderObject(player);
                _players.Add(player);
            }

            foreach (var itemDataProvider in _levelData.ItemData)
            {
                AddRenderObject(new Item3D(itemDataProvider, _baseItemGeometry, _fireOverlayGeometry));
            }
        }

        public void Update(float elapsedTime)
        {
            _explosionFragmentManager.Update(_context, elapsedTime);

            _bombGeometry.ResetActives();
            foreach (var bomb in _levelData.Bombs)
            {
                var instance = _bombGeometry.CreateInstance();

                var bombAnimationScale = 0.75f + (float)Math.Cos(bomb.CurrentBurnTime / bomb.BurnTime * MathHelper.TwoPi * 12) * 0.05f;
                var position = _levelData.GetWorldFromTilePositionCentered(bomb.TilePosition) / 32.0f;

                instance.World = Matrix.CreateScale(0.01f * bombAnimationScale) 
                                 * Matrix.CreateRotationZ(-MathHelper.PiOver4 * 0.5f)
                                 * Matrix.CreateRotationX(-MathHelper.PiOver4 * 0.5f)
                                 * Matrix.CreateTranslation(position.X, 0.5f, position.Y);
            }

            _players.ForEach(p => p.Update());
        }

        public void Render(float elapsedTime, SpriteBatch spriteBatch, RenderTarget2D currentTransitionRenderTarget = null)
        {
            spriteBatch.End();

            _context.Update();

            _graphicsDevice.BlendState = BlendState.Opaque;
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;

            DrawNormalDepthForCelShading();
            DrawDepthForShadows();
            DrawSceneWithCelShading();
            DrawPostProcessEdgeDetection(spriteBatch, currentTransitionRenderTarget);

            _explosionFragmentManager.Render(_context);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null);

            _gameInformation.Draw(spriteBatch);
        }

        private void DrawDepthForShadows()
        {
            _graphicsDevice.SetRenderTarget(_context.NormalLightDepthRenderTarget);
            _graphicsDevice.DepthStencilState = DepthStencilState.Default;

            _graphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            foreach (var renderObject in ValidObjects)
            {
                renderObject.Render(_context, _context.ToonEffect, "ShadowDepth");
            }
        }

        private IEnumerable<IRenderObject> ValidObjects
        {
            get { return _objectsToRender.Where(r => r.IsActive); }
        }

        private void DrawPostProcessEdgeDetection(SpriteBatch spriteBatch, RenderTarget2D currentTransitionRenderTarget)
        {
            _graphicsDevice.SetRenderTarget(currentTransitionRenderTarget);
            _context.PreparePostProcessingEdgeDetection();
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, _context.PostProcessingEffect);
            spriteBatch.Draw(_context.SceneRenderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        private void DrawNormalDepthForCelShading()
        {
            _graphicsDevice.SetRenderTarget(_context.NormalDepthRenderTarget);
            _graphicsDevice.Clear(Color.Black);
            foreach (var renderObject in ValidObjects)
            {
                renderObject.Render(_context, _context.ToonEffect, "NormalDepth");
            }
        }

        private void DrawSceneWithCelShading()
        {
            _graphicsDevice.SetRenderTarget(_context.SceneRenderTarget);
            _graphicsDevice.Clear(Color.LightBlue);

            _context.PrepareCelShadingWithShadows();

            foreach (var renderObject in ValidObjects)
            {
                renderObject.Render(_context, _context.ToonEffect, "CelShading");
            }
        }
    }
}
