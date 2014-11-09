using System;
using BombRush.Interfaces;
using Bombrush.MonoGame.Gui;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;

namespace Bombrush.MonoGame.States
{
    class WaitState : BackgroundState
    {
        private Border _border;
        private int _dotCount;
        private float _elapsed;
        private GameSession _gameExecution;
        private GameUpdateResult _updateResult;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);
            _border = new Border(Game);
        }

        public override void OnLeave()
        {
            
        }

        protected override void OnEntered(object enterInformation)
        {
            _gameExecution = (GameSession)enterInformation;
            _border.Width = (int) (Resources.BigFont.MeasureString("XWait For all Ready ...X").X) + Border.BorderSize * 2;
            _border.Height = Resources.BigFont.LineSpacing*2 + Border.BorderSize*2;
            _border.X = (Game.ScreenWidth - _border.Width) / 2;
            _border.Y = (Game.ScreenHeight - _border.Height) / 2;
            _dotCount = 0;
            _elapsed = 0;
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);

            const float timePerDot = 0.33f;
            _elapsed += elapsedTime;
            if (_elapsed > timePerDot)
            {
                _elapsed -= timePerDot;
                if (++_dotCount == 4)
                    _dotCount = 0;
            }

            _updateResult = _gameExecution.Update(elapsedTime);

            switch (_updateResult)
            {
                case GameUpdateResult.SwitchToGame:
                    return StateChangeInformation.StateChange(typeof (MatchState),typeof(GrowTransition), _gameExecution);
                case GameUpdateResult.ServerShutdown:
                    return StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition), true);
            }

            return StateChangeInformation.Empty;
        }

        private static readonly string[] Dots = { "", ".", "..", "..." };

        private string GetCurrentStateInformationString()
        {
            switch (_gameExecution.State)
            {
                case GameSessionState.PreparingMatchLoadData:
                    return string.Format("Waiting for Data {0}", Dots[_dotCount]);
                case GameSessionState.PreparingMatchWaitForAllReady:
                    return string.Format("Wait For all Ready {0}", Dots[_dotCount]);
                case GameSessionState.PreparingMatchSynchronizeStart:
                    return string.Format("Are you Ready? {0} {1}", (int)Math.Ceiling(_gameExecution.RemainingStartupTime), Dots[_dotCount]);
                case GameSessionState.InGame:
                    return string.Format("Start!");
            }

            return string.Empty;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);

            _border.X += Border.ShadowOffset;
            _border.Y += Border.ShadowOffset;
            _border.Draw(Border.ShadowColor);
            _border.X -= Border.ShadowOffset;
            _border.Y -= Border.ShadowOffset;
            _border.Draw();

            Game.SpriteBatch.DrawString(
                Resources.BigFont, 
                GetCurrentStateInformationString(), 
                new Vector2(_border.X, _border.Y + Resources.BigFont.LineSpacing / 2) + new Vector2(Border.BorderSize), 
                Color.White);
        }
    }
}
