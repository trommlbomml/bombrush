using System;
using BombRush.Interfaces;
using Bombrush.MonoGame.Rendering;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bombrush.MonoGame.States
{
    class MatchState : InitializableState
    {
        private Texture2D _headingTexture;
        private StateChangeInformation _stateChangeInformation;
        private GameSession _gameSession;
        private IGameRenderer _gameRenderer;

        private const float AfterGameEndOutlineTime = 2.0f;
        private bool _finishedSlide;
        private float _elapsed;

        protected override void OnEntered(object enterInformation)
        {
            Game.Cursor.IsActive = false;
            _gameSession = (GameSession)enterInformation;
            _stateChangeInformation = StateChangeInformation.Empty;
            _elapsed = 0;
            _finishedSlide = false;

            //todo: reactivate 3d
            //var rendererType = Settings.Default.GameMode == "3D" ? GameRendererType.ThreeDe : GameRendererType.TwoDe;
            _gameRenderer = GameRendererFactory.CreateGameRenderer(GameRendererType.TwoDe, Game, _gameSession.CurrentLevel);
        }

        protected override void OnInitialize(object enterInformation)
        {
            _headingTexture = Game.Content.Load<Texture2D>("textures/headings");
        }

        public override void OnLeave()
        {
            Game.Cursor.IsActive = true;
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            _stateChangeInformation = StateChangeInformation.Empty;

            if (Game.Keyboard.IsKeyDownOnce(Keys.Escape))
            {
                _gameSession.OnQuit();
                return StateChangeInformation.StateChange(typeof(MainMenuState), typeof (FlipTransition), false);
            }

            _gameRenderer.Update(elapsedTime);
            var result = _gameSession.Update(elapsedTime);
            if (result == GameUpdateResult.ServerShutdown)
                return StateChangeInformation.StateChange(typeof (MainMenuState), typeof (BlendTransition), true);

            if (result == GameUpdateResult.MatchFinished && !_finishedSlide)
            {
                _finishedSlide = true;
                _elapsed = 0;
            }

            if (_finishedSlide)
            {
                _elapsed += elapsedTime;
                if (_elapsed > AfterGameEndOutlineTime)
                {
                    if (_gameSession.CurrentMatchResultType == MatchResultType.SomeOneWins)
                        return StateChangeInformation.StateChange(typeof(MatchResultState), typeof(FlipTransition), _gameSession);
                      
                    //todo: Reactivate Winner State
                    //return StateChangeInformation.StateChange(typeof(WinnerState), typeof(FlipTransition), _gameSession);
                }
            }

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            _gameRenderer.Render(elapsedTime, Game.SpriteBatch, TransitionRenderTarget);

            if (_finishedSlide && _elapsed > 0.5f)
            {
                var position = new Vector2(Game.ScreenWidth * 0.5f, Game.ScreenHeight * 0.5f);
                var delta = MathHelper.SmoothStep(0.0f, 1.0f, Math.Min(_elapsed - 0.5f, 1.0f));

                var sourceRectangle = new Rectangle(0, 100, 300, 50);
                if (_gameSession.CurrentMatchResultType == MatchResultType.Draw) sourceRectangle.Y = 50;

                Game.SpriteBatch.Draw(_headingTexture, position, sourceRectangle, Color.White * delta, MathHelper.TwoPi * delta, 
                                      new Vector2(150, 25), 1.0f, SpriteEffects.None, 0);
            }
        }
    }
}
