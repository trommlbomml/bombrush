using System;
using System.Collections.Generic;
using System.Linq;
using BombRush.Interfaces;
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render2D
{
    class Game2DRenderer : IGameRenderer
    {
        private const int FigureWidth = 24;
        private const int FigureHeight = 32;

        private GameInformation _gameInformation;
        private Rectangle[, ,] _animationFrames;
        private Tileset _tileset;
        private Texture2D _figureTexture;
        private Level _gameLevelLogic;
        private Dictionary<byte, Figure2DAnimationController> _animationContoller;
        private Dictionary<Point, float> _itemAnimationTimes;

        private int _screenWidth;
        private int _screenHeight;

        public Vector2 _centeringOffset;
        
        public Game2DRenderer(Texture2D figureTexture, Tileset tileset, int screenWidth, int screenHeight)
        {
            this._figureTexture = figureTexture;
            this._tileset = tileset;
            this._screenWidth = screenWidth;
            this._screenHeight = screenHeight;
        }

        private void CreateFigureFrames(int colorIndex, int startX, int startY)
        {
            for (int direction = 0; direction < 4; direction++)
            {
                for (int step = 0; step < 3; step++)
                {
                    _animationFrames[colorIndex, direction, step] = new Rectangle(startX + step * FigureWidth,
                                                                                  startY + direction * FigureHeight,
                                                                                  FigureWidth, FigureHeight);
                }
            }
        }

        public void Initialize(Game2D game, Level level)
        {
            _gameLevelLogic = level;
            _gameInformation = new GameInformation(game, level) { Width = BombGame.Tilesize * 15 };

            UserOffset = new Vector2(float.NaN, _gameInformation.Height);

            _itemAnimationTimes = new Dictionary<Point, float>();
            for (int y = 0; y < BombGame.GameLevelWidth; y++)
            {
                for (int x = 0; x < BombGame.GameLevelHeight; x++)
                {
                    _itemAnimationTimes.Add(new Point(x, y), 0);
                }
            }

            UserOffset = new Vector2(float.NaN);

            _animationContoller = new Dictionary<byte, Figure2DAnimationController>();
            foreach (Figure f in _gameLevelLogic.Figures)
            {
                _animationContoller[f.Id] = new Figure2DAnimationController();
            }

            _animationFrames = new Rectangle[4, 4, 3];
            CreateFigureFrames(0, 0, 0);
            CreateFigureFrames(1, 72, 0);
            CreateFigureFrames(2, 0, 128);
            CreateFigureFrames(3, 72, 128);
        }

        private Vector2 _userOffset;
        private Vector2 UserOffset
        {
            get { return _userOffset; } 
            set
            {
                if (_userOffset != value)
                {
                    _userOffset = value;

                    float xOffset = float.IsNaN(UserOffset.X) ? (_screenWidth - BombGame.GameLevelWidth * BombGame.Tilesize) * 0.5f : UserOffset.X;
                    float yOffset = float.IsNaN(UserOffset.Y) ? (_screenHeight - BombGame.GameLevelHeight * BombGame.Tilesize) * 0.5f : UserOffset.Y;
                    _centeringOffset = new Vector2(xOffset, yOffset);
                }
            }
        }

        public void Update(float elapsedTime)
        {
            
        }

        public void Render(float elapsedTime, SpriteBatch spriteBatch, RenderTarget2D currentTransitionRenderTarget = null)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);
            foreach (var tileBlock in _gameLevelLogic.Data) DrawTileBlock(tileBlock, spriteBatch);
            foreach (var item in _gameLevelLogic.ItemData)
            {
                new ItemRenderer(_tileset,_gameLevelLogic, _itemAnimationTimes,_centeringOffset).render(item, spriteBatch, elapsedTime);
            }
            foreach (var tileBlock in _gameLevelLogic.Fringe)  DrawTileBlock(tileBlock, spriteBatch);
            foreach (var figure in _gameLevelLogic.Figures.Where(f => !f.IsAlive))
            {
                new FigureRenderer(_gameLevelLogic, _animationContoller, _animationFrames, _centeringOffset, _figureTexture).render(figure, spriteBatch, elapsedTime);
            }
            foreach (var bomb in _gameLevelLogic.Bombs)
            {
                new BombRenderer(_tileset, _gameLevelLogic,_centeringOffset).render(bomb, spriteBatch);
            }
            foreach (var figure in _gameLevelLogic.Figures.Where(f => f.IsAlive && f.IsVisible))
            {
                new FigureRenderer(_gameLevelLogic, _animationContoller,_animationFrames, _centeringOffset, _figureTexture).render(figure,spriteBatch,elapsedTime);
            }
            foreach (var fragment in _gameLevelLogic.OverlayData) DrawOverlay(fragment, spriteBatch);

            _gameInformation.Draw(spriteBatch);
        }

        private Rectangle ExamineRectangleFromSurround(ExplosionFragment fragment)
        {
            bool leftActive = _gameLevelLogic.GetOverlayInformation(fragment.TilePosition.Left()).IsActive;
            bool rightActive = _gameLevelLogic.GetOverlayInformation(fragment.TilePosition.Right()).IsActive;
            bool upActive = _gameLevelLogic.GetOverlayInformation(fragment.TilePosition.Up()).IsActive;
            bool downActive = _gameLevelLogic.GetOverlayInformation(fragment.TilePosition.Down()).IsActive;

            int currentFrame = (int) (fragment.ActiveTime / 0.05f) % 4;

            Rectangle source = new Rectangle(0, 128 + currentFrame * BombGame.Tilesize, BombGame.Tilesize, BombGame.Tilesize);

            if (!leftActive && !rightActive && upActive && downActive)
            {
                source.X = 1 * BombGame.Tilesize;
                return source;
            }

            if (leftActive && rightActive && !upActive && !downActive)
            {
                source.X = 2 * BombGame.Tilesize;
                return source;
            }

            if (!leftActive && !rightActive && !upActive && downActive)
            {
                source.X = 3 * BombGame.Tilesize;
                return source;
            }

            if (leftActive && !rightActive && !upActive && !downActive)
            {
                source.X = 4 * BombGame.Tilesize;
                return source;
            }

            if (!leftActive && !rightActive && upActive && !downActive)
            {
                source.X = 5 * BombGame.Tilesize;
                return source;
            }

            if (!leftActive && rightActive && !upActive && !downActive)
            {
                source.X = 6 * BombGame.Tilesize;
                return source;
            }

            return source;
        }

        private void DrawOverlay(ExplosionFragment fragment, SpriteBatch spriteBatch)
        {
            if (fragment.IsActive)
            {
                Vector2 position = _gameLevelLogic.GetWorldFromTilePosition(fragment.TilePosition);
                Rectangle sourceRectangle = ExamineRectangleFromSurround(fragment);
                spriteBatch.Draw(_tileset.TileSetTexture, position + _centeringOffset, sourceRectangle, Color.White);
            }
        }

        private void DrawTileBlock(TileBlock tileBlock, SpriteBatch spriteBatch)
        {
            if (!tileBlock.IsActive)
                return;
            //todo: animated tile blocks
            Rectangle currentRectangle = _tileset.Templates[tileBlock.Type].Rectangles[0]; // (IsAnimated ? Rectangles[_currentFrame] : Rectangles[0]);
            Vector2 position = _gameLevelLogic.GetWorldFromTilePosition(tileBlock.TilePosition);

            spriteBatch.Draw(_tileset.TileSetTexture, position + _centeringOffset, currentRectangle, Color.White);
        }
    }
}
