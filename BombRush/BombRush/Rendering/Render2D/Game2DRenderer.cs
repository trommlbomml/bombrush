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

        private Game2D _game;
        private GameInformation _gameInformation;
        private Rectangle[, ,] _animationFrames;
        private Tileset _tileset;
        private Texture2D _figureTexture;
        private Level _gameLevelLogic;
        private Dictionary<byte, Figure2DAnimationController> _animationContoller;
        private Dictionary<Point, float> _itemAnimationTimes;

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
            _game = game;
            _gameLevelLogic = level;
            _tileset = new Tileset(_game.Content, "tilesets/tileset1");
            _figureTexture = _game.Content.Load<Texture2D>("Textures/figures24x32");
            _gameInformation = new GameInformation(_game, level) { Width = BombGame.Tilesize * 15 };

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

                    float xOffset = float.IsNaN(UserOffset.X) ? (_game.ScreenWidth - BombGame.GameLevelWidth * BombGame.Tilesize) * 0.5f : UserOffset.X;
                    float yOffset = float.IsNaN(UserOffset.Y) ? (_game.ScreenHeight - BombGame.GameLevelHeight * BombGame.Tilesize) * 0.5f : UserOffset.Y;
                    CenteringOffset = new Vector2(xOffset, yOffset);
                }
            }
        }

        public Vector2 CenteringOffset { get; private set; }

        public void Update(float elapsedTime)
        {
            
        }

        public void Render(float elapsedTime, SpriteBatch spriteBatch, RenderTarget2D currentTransitionRenderTarget = null)
        {
            spriteBatch.GraphicsDevice.Clear(Color.Black);
            foreach (var tileBlock in _gameLevelLogic.Data) DrawTileBlock(tileBlock, spriteBatch);
            foreach (var item in _gameLevelLogic.ItemData) DrawItem(item, spriteBatch, elapsedTime);
            foreach (var tileBlock in _gameLevelLogic.Fringe)  DrawTileBlock(tileBlock, spriteBatch);
            foreach (var figure in _gameLevelLogic.Figures.Where(f => !f.IsAlive)) DrawFigure(elapsedTime, figure, spriteBatch);
            foreach (var bomb in _gameLevelLogic.Bombs) DrawBomb(bomb, spriteBatch);
            foreach (var figure in _gameLevelLogic.Figures.Where(f => f.IsAlive && f.IsVisible)) DrawFigure(elapsedTime, figure, spriteBatch);
            foreach (var fragment in _gameLevelLogic.OverlayData) DrawOverlay(fragment, spriteBatch);

            _gameInformation.Draw(spriteBatch);
        }

        private void DrawFigure(float elapsedTime, Figure figure, SpriteBatch spriteBatch)
        {
            Point tilePosition = _gameLevelLogic.GetTilePositionFromWorld(figure.Position);
            Vector2 centeredPosition = _gameLevelLogic.GetWorldFromTilePositionCentered(tilePosition);

            Figure2DAnimationController controller = _animationContoller[figure.Id];
            controller.UpdateAnimation(figure, elapsedTime, centeredPosition, CenteringOffset);

            if (figure.IsAlive)
            {
                spriteBatch.Draw(_figureTexture,
                                 (figure.Position + CenteringOffset).SnapToPixels(),
                                 _animationFrames[figure.Id - 1, (int)figure.Direction, controller.StepFrame],
                                 Color.White,
                                 0.0f,
                                 new Vector2(FigureWidth * 0.5f, FigureHeight * 0.5f + 6),
                                 1.5f,
                                 SpriteEffects.None,
                                 0);
            }
            else
            {
                if (controller.DieAnimationTime >= Figure2DAnimationController.IdleAnimationTime)
                {
                    if (controller.DieAnimationTime < Figure2DAnimationController.OverallDieAnimationDuration)
                    {
                        spriteBatch.Draw(_figureTexture, 
                                         centeredPosition + CenteringOffset, 
                                         new Rectangle(192, 224, 32, 32), 
                                         Color.White * 0.5f, 0.0f, new Vector2(16), 1.0f, SpriteEffects.None, 0);   
                    }

                    spriteBatch.Draw(_figureTexture,
                                    (controller.DieAnimationPosition).SnapToPixels(),
                                    new Rectangle(224, 224, 32, 32),
                                    Color.White,
                                    0.0f,
                                    new Vector2(16),
                                    1.0f,
                                    SpriteEffects.None, 0);

                    if (controller.DieAnimationTime <= Figure2DAnimationController.OverallDieAnimationDuration)
                    {
                        float alpha = MathHelper.Clamp(((controller.DieAnimationTime - Figure2DAnimationController.IdleAnimationTime) * 3) /
                                  Figure2DAnimationController.DieAnimationDuration, 0, 1);

                        spriteBatch.Draw(_figureTexture,
                                         new Rectangle((int) (centeredPosition.X - 16 + CenteringOffset.X), 0, 32, controller.YDistanceForDie - 7), 
                                         new Rectangle(160, 224, 32, 32),
                                         Color.White * 0.5f * alpha);
                    }
                }
            }

            if (figure.ShowPlayerName)
            {
                Vector2 resultPosition = figure.IsAlive ? figure.Position : centeredPosition;
                resultPosition.Y -= figure.IsAlive ? 34.0f : 27.0f;
                Vector2 offset = new Vector2(Resources.PlayerNameFont.MeasureString(figure.Name).X * 0.5f, 0);

                spriteBatch.DrawString(Resources.PlayerNameFont, figure.Name, resultPosition.SnapToPixels() + CenteringOffset, Color.White, 0.0f, offset.SnapToPixels(), 1.0f, SpriteEffects.None, 0);
            }

#if DEBUG
            //todo: refactor
            //DrawPlaceMapIfCom(_game.ShapeRenderer, figure);
#endif
        }

        //private void DrawPlaceMapIfCom(ShapeRenderer shapeRenderer, Figure figure)
        //{
        //    if (figure is Figure && ((Figure)figure).FigureController is ComFigureController)
        //    {
        //        ((ComFigureController)((Figure)figure).FigureController).DebugDraw(shapeRenderer, CenteringOffset);
        //    }
        //}

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
                spriteBatch.Draw(_tileset.TileSetTexture, position + CenteringOffset, sourceRectangle, Color.White);
            }
        }

        private void DrawBomb(Bomb bomb, SpriteBatch spriteBatch)
        {
            Rectangle bombSourceRectangle = new Rectangle(3 * BombGame.Tilesize + 1, 
                                                          (byte)bomb.BombType * BombGame.Tilesize + 1,
                                                          BombGame.Tilesize - 2, 
                                                          BombGame.Tilesize - 2);
            Vector2 centerOrigin = new Vector2(BombGame.Tilesize * 0.5f - 2, BombGame.Tilesize * 0.5f - 2);

            if (bomb.IsActive)
            {
                float bombAnimationScale = 0.75f + (float)Math.Cos(bomb.CurrentBurnTime/bomb.BurnTime*MathHelper.TwoPi * 12)*0.05f;
                
                spriteBatch.Draw(_tileset.TileSetTexture, _gameLevelLogic.GetWorldFromTilePositionCentered(bomb.TilePosition) + CenteringOffset,
                    bombSourceRectangle, Color.White,
                    0.0f, centerOrigin, bombAnimationScale, SpriteEffects.None, 0);    
            }
        }

        private void DrawItem(Item item, SpriteBatch spriteBatch, float elapsedTime)
        {
            const int baseOffset = 4;
            float time = 0;
            int flop = 0;
            if (item.IsActive)
            {
                time = _itemAnimationTimes[item.TilePosition] + elapsedTime;
                if (time > 0.6f)
                {
                    time -= 0.6f;
                    flop = 0;
                }
                else if (time > 0.3f)
                {
                    flop = 1;
                }

                Vector2 position = _gameLevelLogic.GetWorldFromTilePosition(item.TilePosition);
                Rectangle sourceRectangle = new Rectangle((baseOffset + (((int)item.Type)-1) * 2 + flop) * BombGame.Tilesize, 0, BombGame.Tilesize, BombGame.Tilesize);
                spriteBatch.Draw(_tileset.TileSetTexture, position + CenteringOffset, sourceRectangle, Color.White);
            }
            
            _itemAnimationTimes[item.TilePosition] = time;
        }

        private void DrawTileBlock(TileBlock tileBlock, SpriteBatch spriteBatch)
        {
            if (!tileBlock.IsActive)
                return;
            //todo: animated tile blocks
            Rectangle currentRectangle = _tileset.Templates[tileBlock.Type].Rectangles[0]; // (IsAnimated ? Rectangles[_currentFrame] : Rectangles[0]);
            Vector2 position = _gameLevelLogic.GetWorldFromTilePosition(tileBlock.TilePosition);

            spriteBatch.Draw(_tileset.TileSetTexture, position + CenteringOffset, currentRectangle, Color.White);
        }
    }
}
