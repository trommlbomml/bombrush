using System.Collections.Generic;
using System.Linq;
using BombRush.Interfaces;
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.Rendering.Render2D
{
    class FigureRenderer : GameObject
    {
        private const int FigureWidth = 24;
        private const int FigureHeight = 32;

        private Vector2 _centeringOffset;

        private readonly Dictionary<byte, Figure2DAnimationController> _animationContoller;
        private readonly Texture2D _figureTexture;
        private readonly Rectangle[, ,] _animationFrames;

        public FigureRenderer(Game2D game, Level gameLevel, Vector2 centeringOffset) : base(game)
        {
            _centeringOffset = centeringOffset;
            _figureTexture = Game.Content.Load<Texture2D>("Textures/figures24x32");

            _animationFrames = new Rectangle[4, 4, 3];
            CreateFigureFrames(0, 0, 0);
            CreateFigureFrames(1, 72, 0);
            CreateFigureFrames(2, 0, 128);
            CreateFigureFrames(3, 72, 128);

            _animationContoller = new Dictionary<byte, Figure2DAnimationController>();
            foreach (var f in gameLevel.Figures)
            {
                _animationContoller[f.Id] = new Figure2DAnimationController();
            }
        }

        private void CreateFigureFrames(int colorIndex, int startX, int startY)
        {
            for (var direction = 0; direction < 4; direction++)
            {
                for (var step = 0; step < 3; step++)
                {
                    _animationFrames[colorIndex, direction, step] = new Rectangle(startX + step * FigureWidth,
                                                                                  startY + direction * FigureHeight,
                                                                                  FigureWidth, FigureHeight);
                }
            }
        }

        private void RenderFigureAlive(Figure figure, Figure2DAnimationController controller)
        {
            Game.SpriteBatch.Draw(_figureTexture,
                                 (figure.Position + _centeringOffset).SnapToPixels(),
                                 _animationFrames[figure.Id - 1, (int)figure.Direction, controller.StepFrame],
                                 Color.White,
                                 0.0f,
                                 new Vector2(FigureWidth * 0.5f, FigureHeight * 0.5f + 6),
                                 1.5f,
                                 SpriteEffects.None,
                                 0);
        }

        private void RenderFigureDead(Figure2DAnimationController controller, Vector2 centeredPosition)
        {
            if (!(controller.DieAnimationTime >= Figure2DAnimationController.IdleAnimationTime)) return;

            if (controller.DieAnimationTime < Figure2DAnimationController.OverallDieAnimationDuration)
            {
                Game.SpriteBatch.Draw(_figureTexture,
                    centeredPosition + _centeringOffset,
                    new Rectangle(192, 224, 32, 32),
                    Color.White * 0.5f, 0.0f, new Vector2(16), 1.0f, SpriteEffects.None, 0);
            }

            Game.SpriteBatch.Draw(_figureTexture,
                (controller.DieAnimationPosition).SnapToPixels(),
                new Rectangle(224, 224, 32, 32),
                Color.White,
                0.0f,
                new Vector2(16),
                1.0f,
                SpriteEffects.None, 0);

            if (controller.DieAnimationTime <= Figure2DAnimationController.OverallDieAnimationDuration)
            {
                var alpha = MathHelper.Clamp(((controller.DieAnimationTime - Figure2DAnimationController.IdleAnimationTime) * 3) /
                                               Figure2DAnimationController.DieAnimationDuration, 0, 1);

                Game.SpriteBatch.Draw(_figureTexture,
                    new Rectangle((int)(centeredPosition.X - 16 + _centeringOffset.X), 0, 32, controller.YDistanceForDie - 7),
                    new Rectangle(160, 224, 32, 32),
                    Color.White * 0.5f * alpha);
            }
        }

        private void RenderPlayerName(Figure figure, Vector2 centeredPosition)
        {
            if (!figure.ShowPlayerName) return;

            var resultPosition = figure.IsAlive ? figure.Position : centeredPosition;
            resultPosition.Y -= figure.IsAlive ? 34.0f : 27.0f;
            var offset = new Vector2(Resources.PlayerNameFont.MeasureString(figure.Name).X * 0.5f, 0);

            Game.SpriteBatch.DrawString(Resources.PlayerNameFont, figure.Name, resultPosition.SnapToPixels() + _centeringOffset, Color.White, 0.0f, offset.SnapToPixels(), 1.0f, SpriteEffects.None, 0);
        }

        public void RenderAlive(Level level, float elapsedTime)
        {
            foreach (var figure in level.Figures.Where(f => f.IsAlive && f.IsVisible))
            {
                var tilePosition = level.GetTilePositionFromWorld(figure.Position);
                var centeredPosition = level.GetWorldFromTilePositionCentered(tilePosition);
                var controller = _animationContoller[figure.Id];

                controller.UpdateAnimation(figure, elapsedTime, centeredPosition, _centeringOffset);

                RenderFigureAlive(figure, controller);
                RenderPlayerName(figure, centeredPosition);
            }
        }

        public void RenderDead(Level level, float elapsedTime)
        {
            foreach (var figure in level.Figures.Where(f => !f.IsAlive))
            {
                var tilePosition = level.GetTilePositionFromWorld(figure.Position);
                var centeredPosition = level.GetWorldFromTilePositionCentered(tilePosition);
                var controller = _animationContoller[figure.Id];

                controller.UpdateAnimation(figure, elapsedTime, centeredPosition, _centeringOffset);

                RenderFigureDead(controller, centeredPosition);
                RenderPlayerName(figure, centeredPosition);
            }
        }
    }
}
