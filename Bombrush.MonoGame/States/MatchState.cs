using System;
using BombRush.Interfaces;
using Bombrush.MonoGame.Rendering;
using Game2DFramework.Animations;
using Game2DFramework.Drawing;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bombrush.MonoGame.States
{
    class MatchState : InitializableState
    {
        private Animator _finishSlideAnimator;
        private Sprite _finishedSprite;
        private StateChangeInformation _stateChangeInformation;
        private GameSession _gameSession;
        private IGameRenderer _gameRenderer;

        protected override void OnEntered(object enterInformation)
        {
            Game.Cursor.IsActive = false;
            _gameSession = (GameSession)enterInformation;
            _stateChangeInformation = StateChangeInformation.Empty;

            //todo: reactivate 3d
            //var rendererType = Settings.Default.GameMode == "3D" ? GameRendererType.ThreeDe : GameRendererType.TwoDe;
            _gameRenderer = GameRendererFactory.CreateGameRenderer(GameRendererType.TwoDe, Game, _gameSession.CurrentLevel);

            _finishSlideAnimator = new Animator();
            _finishSlideAnimator.AddAnimation("RotateText", new DeltaAnimation(1.0f, RotateAndSetAlphaText, false));
            _finishSlideAnimator.AddAnimation("Wait", new WaitAnimation(1.5f));
            _finishSlideAnimator.AddTransition("RotateText", "Wait");
            _finishSlideAnimator.AnimationFinished += OnFinishSlideFinished;

            _finishedSprite = new Sprite(Game.Content.Load<Texture2D>("textures/headings"))
            {
                Position = new Vector2(Game.ScreenWidth*0.5f, Game.ScreenHeight*0.5f),
                Alpha = 0.0f
            };
        }

        private void OnFinishSlideFinished(string name, bool playReversed)
        {
            if (_gameSession.CurrentMatchResultType == MatchResultType.SomeOneWins)
                _stateChangeInformation = StateChangeInformation.StateChange(typeof(MatchResultState), typeof(FlipTransition), _gameSession);

            //todo: Reactivate Winner State
            //_stateChangeInformation = StateChangeInformation.StateChange(typeof(WinnerState), typeof(FlipTransition), _gameSession);
        }

        private void RotateAndSetAlphaText(float step)
        {
            var delta = MathHelper.SmoothStep(0.0f, 1.0f, step);
            _finishedSprite.Alpha = delta;
            _finishedSprite.Rotation = MathHelper.TwoPi*delta;
        }

        protected override void OnInitialize(object enterInformation)
        {
        }

        public override void OnLeave()
        {
            Game.Cursor.IsActive = true;
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            _stateChangeInformation = StateChangeInformation.Empty;

            _finishSlideAnimator.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(elapsedTime)));
            _gameRenderer.Update(elapsedTime);

            if (Game.Keyboard.IsKeyDownOnce(Keys.Escape))
            {
                _gameSession.OnQuit();
                return StateChangeInformation.StateChange(typeof(MainMenuState), typeof (FlipTransition), false);
            }
            
            var result = _gameSession.Update(elapsedTime);
            switch (result)
            {
                case GameUpdateResult.MatchFinished:
                    if (_finishSlideAnimator.CurrentAnimation == null)
                    {
                        _finishSlideAnimator.SetAnimation("RotateText");
                        var y = _gameSession.CurrentMatchResultType == MatchResultType.Draw ? 50 : 100;
                        _finishedSprite.SetSourceRectangle(new Rectangle(0, y, 300, 50));
                    }
                    break;
                case GameUpdateResult.ServerShutdown:
                    return StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition), true);
            }

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            _gameRenderer.Render(elapsedTime, Game.SpriteBatch, TransitionRenderTarget);
            _finishedSprite.Draw(Game.SpriteBatch);
        }
    }
}
