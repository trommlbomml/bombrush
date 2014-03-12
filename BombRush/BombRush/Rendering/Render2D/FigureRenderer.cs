using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BombRush.Interfaces;
using Game2DFramework.Extensions;

namespace BombRush.Rendering.Render2D
{
    class FigureRenderer
    {
        private const int FigureWidth = 24;
        private const int FigureHeight = 32;

        private Level _gameLevelLogic;

        private Dictionary<byte, Figure2DAnimationController> _animationContoller;
        
        private Vector2 _centeringOffset;

        private Texture2D _figureTexture;

        private Rectangle[, ,] _animationFrames;

        public FigureRenderer(Level _gameLevelLogic, Dictionary<byte, Figure2DAnimationController> _animationContoller, Rectangle[, ,] _animationFrames, Vector2 _centeringOffset, Texture2D _figureTexture)
        {
            this._gameLevelLogic = _gameLevelLogic;
            this._animationContoller = _animationContoller;
            this._centeringOffset = _centeringOffset;
            this._figureTexture = _figureTexture;
            this._animationFrames = _animationFrames;
        }

        internal void render(Interfaces.Figure figure, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch,float elapsedTime)
        {
            Point tilePosition = _gameLevelLogic.GetTilePositionFromWorld(figure.Position);
            Vector2 centeredPosition = _gameLevelLogic.GetWorldFromTilePositionCentered(tilePosition);

            Figure2DAnimationController controller = _animationContoller[figure.Id];
            controller.UpdateAnimation(figure, elapsedTime, centeredPosition, _centeringOffset);

            if (figure.IsAlive)
            {
                spriteBatch.Draw(_figureTexture,
                                 (figure.Position + _centeringOffset).SnapToPixels(),
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
                                         centeredPosition + _centeringOffset,
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
                                         new Rectangle((int)(centeredPosition.X - 16 + _centeringOffset.X), 0, 32, controller.YDistanceForDie - 7),
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

                spriteBatch.DrawString(Resources.PlayerNameFont, figure.Name, resultPosition.SnapToPixels() + _centeringOffset, Color.White, 0.0f, offset.SnapToPixels(), 1.0f, SpriteEffects.None, 0);
            }
        }

        internal void render(Figure figure, SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }
    }
}
