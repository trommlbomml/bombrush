using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BombRush.Interfaces;
using BombRush.Logic;
using Bombrush.MonoGame.Controller;
using Bombrush.MonoGame.Gui;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;

namespace Bombrush.MonoGame.States
{
    class LocalGameConfigurationState : BackgroundState
    {
        private StateChangeInformation _stateChangeInformation;
        private StackedMenu _menu;
        private NumericMenuItem _comMenuItem;
        private NumericMenuItem _playersMenuItem;

        private static IEnumerable<string> GetLocalLevelNames()
        {
            return Directory.GetFiles("Content/Levels")
                            .Select(Path.GetFileNameWithoutExtension)
                            .ToArray();
        }

        private static string GetFilePathFromLevelName(string levelName)
        {
            return Path.Combine("Content/Levels", levelName + ".xml");
        }

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);

            _menu = new StackedMenu(Game) { Title = "Local Game" };
            _menu.AppendMenuItem(new ActionMenuItem(Game, "Start", StartLocalGame, ActionTriggerKind.IsAccept));
            _playersMenuItem = _menu.AppendMenuItem(new NumericMenuItem(Game, "Players", 1, 4));
            _comMenuItem = _menu.AppendMenuItem(new NumericMenuItem(Game, "COMs", 0, 3));
            _menu.AppendMenuItem(new EnumMenuItem(Game, "Level", GetLocalLevelNames()));
            _menu.AppendMenuItem(new TimeMenuItem(Game, "Time", 0, 600) { CurrentValue = 240, });
            _menu.AppendMenuItem(new ActionMenuItem(Game, "Back", () => { _stateChangeInformation = StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition)); }, ActionTriggerKind.IsCancel));
        }

        public override void OnLeave()
        {
            
        }

        protected override void OnEntered(object enterInformation)
        {
            _stateChangeInformation = StateChangeInformation.Empty;
            _menu.SelectFirstMenuItem();
            _menu.RecalculatePositions();
        }

        private void StartLocalGame()
        {
            var levelAssetLocalName = _menu.GetMenuItem<EnumMenuItem>(3).SelectedItem;

            var parameters = new GameSessionStartParameters
            {
                MatchesToWin = 5,
                MatchTime = _menu.GetMenuItem<NumericMenuItem>(4).CurrentValue,
                SessionName = "Local Session",
                LevelAssetPath = GetFilePathFromLevelName(levelAssetLocalName),
                ProvidePlayerFigureController = ProvideFigureController
            };
            var gameSession = new GameSessionImp(parameters);

            int countPlayers = _playersMenuItem.CurrentValue;
            int countComPlayers = _comMenuItem.CurrentValue;

            for (var i = 0; i < countPlayers; i++) gameSession.AddMember(MemberType.ActivePlayer);
            for (var i = 0; i < countComPlayers; i++) gameSession.AddMember(MemberType.Computer);

            gameSession.StartMatch();
            _stateChangeInformation = StateChangeInformation.StateChange(typeof(WaitState), typeof(SlideTransition), gameSession);
        }

        private FigureController ProvideFigureController(int playerIndex)
        {
            return PlayerController.Create(Game, InputDeviceType.Keyboard, playerIndex);
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);
            _menu.Update(elapsedTime);

            _comMenuItem.MinValue = Math.Max(0, 2 - _playersMenuItem.CurrentValue);
            _comMenuItem.MaxValue = BombGame.MaxPlayerCount - _playersMenuItem.CurrentValue;

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);
            _menu.Draw(Game.SpriteBatch);
        }
    }
}
