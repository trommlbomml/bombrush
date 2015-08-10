using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BombRush.Interfaces;
using BombRush.Logic;
using Bombrush.MonoGame.Controller;
using Bombrush.MonoGame.Gui;
using Game2DFramework.Gui;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;

namespace Bombrush.MonoGame.States
{
    class LocalGameConfigurationState : BackgroundState
    {
        private StateChangeInformation _stateChangeInformation;

        private GuiPanel _panel;
        private TextBlock _playerCountTextBlock;
        private TextBlock _comCountTextBlock;
        private TextBlock _levelNameTextBlock;

        private int _playerCount = 1;
        private int _comPlayerCount = 1;
        private int _currentLevelIndex;
        private int _timeSeconds = 240;
        private string[] _levelNames;
        private TextBlock _currentTimeTextBlock;

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

            _currentLevelIndex = 0;
            _levelNames = GetLocalLevelNames().ToArray();

            _panel = new GuiPanel(Game);

            var frame = Game.GuiSystem.CreateGuiHierarchyFromXml<GuiElement>("Content/GuiLayouts/LocalGameConfig_Layout.xml");
            Game.GuiSystem.ArrangeCenteredToScreen(Game, frame);
            _panel.AddElement(frame);

            _playerCountTextBlock = frame.FindGuiElementById<TextBlock>("PlayerCountText");
            _comCountTextBlock = frame.FindGuiElementById<TextBlock>("ComCountText");
            _levelNameTextBlock = frame.FindGuiElementById<TextBlock>("CurrentLevelText");
            _currentTimeTextBlock = frame.FindGuiElementById<TextBlock>("CurrentTimeText");

            _playerCountTextBlock.Text = _playerCount.ToString(CultureInfo.InvariantCulture);
            _comCountTextBlock.Text = _comPlayerCount.ToString(CultureInfo.InvariantCulture);
            _levelNameTextBlock.Text = _levelNames[_currentLevelIndex];
            _currentTimeTextBlock.Text = string.Format("{0:00}:{1:00}", _timeSeconds/60, _timeSeconds%60);

            frame.FindGuiElementById<Button>("DecreasePlayerCountButton").Click += () => OnChangePlayerCount(false);
            frame.FindGuiElementById<Button>("IncreasePlayerCountButton").Click += () => OnChangePlayerCount(true);
            frame.FindGuiElementById<Button>("DecreaseComCountButton").Click += () => OnChangeComCount(false);
            frame.FindGuiElementById<Button>("IncreaseComCountButton").Click += () => OnChangeComCount(true);
            frame.FindGuiElementById<Button>("PreviousLevelButton").Click += () => OnChangeLevel(false);
            frame.FindGuiElementById<Button>("NextLevelButton").Click += () => OnChangeLevel(true);
            frame.FindGuiElementById<Button>("DecreaseTimeButton").Click += () => OnChangeTime(false);
            frame.FindGuiElementById<Button>("IncreaseTimeButton").Click += () => OnChangeTime(true);

            //_menu = new StackedMenu(Game) { Title = "Local Game" };
            //_menu.AppendMenuItem(new ActionMenuItem(Game, "Start", StartLocalGame, ActionTriggerKind.IsAccept));;
            //_menu.AppendMenuItem(new EnumMenuItem(Game, "Level", GetLocalLevelNames()));
            //_menu.AppendMenuItem(new TimeMenuItem(Game, "Time", 0, 600) { CurrentValue = 240, });
            //_menu.AppendMenuItem(new ActionMenuItem(Game, "Back", () => { _stateChangeInformation = StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition)); }, ActionTriggerKind.IsCancel));
        }

        private void OnChangeTime(bool íncrease)
        {
            if (íncrease)
            {
                _timeSeconds = Math.Min(600, _timeSeconds + 20);
            }
            else
            {
                _timeSeconds = Math.Max(0, _timeSeconds - 20);
            }

            _currentTimeTextBlock.Text = string.Format("{0:00}:{1:00}", _timeSeconds / 60, _timeSeconds % 60);
        }

        private void OnChangeLevel(bool next)
        {
            if (next)
            {
                if (++_currentLevelIndex == _levelNames.Length)
                {
                    _currentLevelIndex = 0;
                }
            }
            else
            {
                if (--_currentLevelIndex == -1)
                {
                    _currentLevelIndex = _levelNames.Length-1;
                }
            }

            _levelNameTextBlock.Text = _levelNames[_currentLevelIndex];
        }

        private static void BalanceCounts(bool increase, ref int changeValue, ref int balanceValue)
        {
            if (increase)
            {
                if (balanceValue + changeValue < 4)
                {
                    changeValue += 1;
                }
                else if (balanceValue >= 1)
                {
                    balanceValue -= 1;
                    changeValue += 1;
                }
            }
            else
            {
                if (changeValue > 0)
                {
                    changeValue -= 1;
                    if (changeValue + balanceValue < 2)
                    {
                        balanceValue += 1;
                        if (changeValue == 0 && balanceValue == 1)
                        {
                            balanceValue += 1;
                        }
                    }
                }
            }
        }

        private void OnChangePlayerCount(bool increase)
        {
            BalanceCounts(increase, ref _playerCount, ref _comPlayerCount);

            _playerCountTextBlock.Text = _playerCount.ToString(CultureInfo.InvariantCulture);
            _comCountTextBlock.Text = _comPlayerCount.ToString(CultureInfo.InvariantCulture);
        }

        private void OnChangeComCount(bool increase)
        {
            
            BalanceCounts(increase, ref _comPlayerCount, ref _playerCount);

            _playerCountTextBlock.Text = _playerCount.ToString(CultureInfo.InvariantCulture);
            _comCountTextBlock.Text = _comPlayerCount.ToString(CultureInfo.InvariantCulture);
        }

        public override void OnLeave()
        {
            
        }

        protected override void OnEntered(object enterInformation)
        {
            _stateChangeInformation = StateChangeInformation.Empty;
        }

        //private void StartLocalGame()
        //{
        //    var levelAssetLocalName = _menu.GetMenuItem<EnumMenuItem>(3).SelectedItem;

        //    var parameters = new GameSessionStartParameters
        //    {
        //        MatchesToWin = 5,
        //        MatchTime = _menu.GetMenuItem<NumericMenuItem>(4).CurrentValue,
        //        SessionName = "Local Session",
        //        LevelAssetPath = GetFilePathFromLevelName(levelAssetLocalName),
        //        ProvidePlayerFigureController = ProvideFigureController
        //    };
        //    var gameSession = new GameSessionImp(parameters);

        //    int countPlayers = _playersMenuItem.CurrentValue;
        //    int countComPlayers = _comMenuItem.CurrentValue;

        //    for (var i = 0; i < countPlayers; i++) gameSession.AddMember(MemberType.ActivePlayer);
        //    for (var i = 0; i < countComPlayers; i++) gameSession.AddMember(MemberType.Computer);

        //    gameSession.StartMatch();
        //    _stateChangeInformation = StateChangeInformation.StateChange(typeof(WaitState), typeof(SlideTransition), gameSession);
        //}

        private FigureController ProvideFigureController(int playerIndex)
        {
            return PlayerController.Create(Game, InputDeviceType.Keyboard, playerIndex);
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);
            _panel.Update(elapsedTime);

            //_menu.Update(elapsedTime);

            //_comMenuItem.MinValue = Math.Max(0, 2 - _playersMenuItem.CurrentValue);
            //_comMenuItem.MaxValue = BombGame.MaxPlayerCount - _playersMenuItem.CurrentValue;

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);
            _panel.Draw();
        }
    }
}
