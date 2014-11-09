using System.Collections.Generic;
using System.Linq;
using BombRush.Interfaces;
using Bombrush.MonoGame.Gui;
using Bombrush.MonoGame.Rendering;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bombrush.MonoGame.States
{
    class WinnerState : BackgroundState
    {
        private TitledBorder _titledBorder;
        private GameSession _gameExecution;
        private List<GameSessionMember> _sortedMembers;

        private const int Padding = 2;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);
            _titledBorder = new TitledBorder(Game, "Game over!", 100, 100, BombGame.MenuStartY);
        }

        public override void OnLeave()
        {
            
        }

        protected override void OnEntered(object enterInformation)
        {
            _titledBorder.SetClientSize(300, 4 * (Resources.NormalFont.LineSpacing + Padding), BombGame.MenuStartY);
            _gameExecution = (GameSession)enterInformation;

            _sortedMembers = _gameExecution.Members.Where(m => m.Type != MemberType.Watcher).OrderByDescending(m => m.Wins).ToList();
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);

            if (_gameExecution.Update(elapsedTime) == GameUpdateResult.ServerShutdown)
                return StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition), true);    

            if (Game.Keyboard.IsKeyDownOnce(Keys.Escape))
            {
                _gameExecution.OnQuit();
                return StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition), false);
            }

            if (Game.Keyboard.IsKeyDownOnce(Keys.Enter))
            {
                _gameExecution.OnQuit();
                return StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition), false);
            }

            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);

            _titledBorder.Draw(Game.SpriteBatch, true);

            int rowHeight = Resources.NormalFont.LineSpacing + Padding;
            int startY = _titledBorder.ClientRectangle.Y;

            int place = 1;
            foreach (var sortedMember in _sortedMembers)
            {
                int currentX = _titledBorder.ClientRectangle.X;
                Heads.Draw(Game.SpriteBatch, sortedMember.Id, currentX, startY, Resources.NormalFont.LineSpacing,
                           Resources.NormalFont.LineSpacing);

                currentX += rowHeight;
                Game.SpriteBatch.DrawString(Resources.NormalFont, string.Format("{0}. {1} - {2}", place, sortedMember.Name, sortedMember.Wins), new Vector2(currentX, startY), Color.White);
                place++;
                startY += rowHeight;
            }
        }
    }
}
