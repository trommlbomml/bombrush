using System;
using System.Globalization;
using System.Linq;
using BombRush.Interfaces;
using Bombrush.MonoGame.Gui;
using Bombrush.MonoGame.Rendering;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Bombrush.MonoGame.States
{
    class MatchResultState : BackgroundState
    {
        private const int PlayerWidth = 200;
        private const int ScoreWidth = 50;
        private const int Padding = 5;
        private const int PaddingToHeading = 20;

        private TitledBorder _titledBorder;
        private GameSession _gameExecution;

        protected override void OnEntered(object enterInformation)
        {
            _gameExecution = (GameSession)enterInformation;
            _titledBorder.SetClientSize(PlayerWidth + ScoreWidth, (Resources.NormalFont.LineSpacing + Padding) * 6 + PaddingToHeading, BombGame.MenuStartY);
        }

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);
            _titledBorder = new TitledBorder(Game, "Match Result", 100, 100, BombGame.MenuStartY);
        }

        public override void OnLeave()
        {
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);

            if (_gameExecution.Update(elapsedTime) == GameUpdateResult.ServerShutdown)
                return StateChangeInformation.StateChange(typeof (MainMenuState), typeof (BlendTransition), true);

            if (Game.Keyboard.IsKeyDownOnce(Keys.Escape))
            {
                _gameExecution.OnQuit();
                return StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition), true);
            }

            if (Game.Keyboard.IsKeyDownOnce(Keys.Enter))
            {
                _gameExecution.StartMatch();
                return StateChangeInformation.StateChange(typeof(WaitState), typeof(SlideTransition), _gameExecution);
            }

            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);

            _titledBorder.Draw(Game.SpriteBatch, true);

            int rowHeight = Resources.NormalFont.LineSpacing + Padding;
            int startY = _titledBorder.ClientRectangle.Y;

            switch (_gameExecution.CurrentMatchResultType)
            {
                case MatchResultType.SomeOneWins:
                    DrawWinner(startY, Game.SpriteBatch);
                    break;
                case MatchResultType.Draw:
                    DrawDraw(startY, Game.SpriteBatch);
                    break;
            }

            startY += rowHeight;

            Game.ShapeRenderer.DrawLine(_titledBorder.ClientRectangle.X, startY, _titledBorder.ClientRectangle.Right, startY, Color.White);

            startY += PaddingToHeading;

            foreach (var member in _gameExecution.CurrentLevel.Figures)
            {
                Heads.Draw(Game.SpriteBatch, member.Id, _titledBorder.ClientRectangle.X, startY, rowHeight - 2,
                           rowHeight - 2);

                Game.SpriteBatch.DrawString(
                    Resources.NormalFont,
                    member.Name,
                    new Vector2(_titledBorder.ClientRectangle.X + rowHeight, startY), 
                    member.IsMatchWinner ? Color.Orange : Color.White);

                Game.SpriteBatch.DrawString(
                    Resources.NormalFont,
                    member.Wins.ToString(CultureInfo.InvariantCulture),
                    new Vector2(_titledBorder.ClientRectangle.X + PlayerWidth, startY), 
                    member.IsMatchWinner ? Color.Orange : Color.White);
                
                startY += rowHeight;
            }

            startY = _titledBorder.ClientRectangle.Y + (Resources.NormalFont.LineSpacing + Padding) * 5 + PaddingToHeading;
            Game.ShapeRenderer.DrawLine(_titledBorder.ClientRectangle.X, startY, _titledBorder.ClientRectangle.Right, startY, Color.White);

            Game.SpriteBatch.DrawString(Resources.NormalFont, "Press Enter to resume", new Vector2(_titledBorder.ClientRectangle.X, startY), Color.White);

        }

        private void DrawDraw(int startY, SpriteBatch spriteBatch)
        {
            int rowHeight = Resources.NormalFont.LineSpacing + Padding;

            spriteBatch.DrawString(
                Resources.NormalFont,
                "Nobody got points!",
                new Vector2(_titledBorder.ClientRectangle.X + rowHeight, startY), Color.White);
        }

        private void DrawWinner(int startY, SpriteBatch spriteBatch)
        {
            int rowHeight = Resources.NormalFont.LineSpacing + Padding;
            Figure memberWhoWins = _gameExecution.CurrentLevel.Figures.First(c => c.IsMatchWinner);
            int currentX = _titledBorder.ClientRectangle.X;

            const string text = " Winner:";
            spriteBatch.DrawString(Resources.NormalFont, text, new Vector2(currentX, startY), Color.White);

            currentX += (int)Math.Ceiling(Resources.NormalFont.MeasureString(text).X) + Padding;
            Heads.Draw(spriteBatch, memberWhoWins.Id, currentX, startY, rowHeight - 2, rowHeight - 2);

            currentX += rowHeight;
            spriteBatch.DrawString(Resources.NormalFont, memberWhoWins.Name, new Vector2(currentX, startY), Color.White);
        }
    }
}
