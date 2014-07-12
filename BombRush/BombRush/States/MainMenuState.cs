
using System;
using BombRush.Gui2;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using BombRush.Gui;

namespace BombRush.States
{
    class MainMenuState : BackgroundState
    {
        private Frame _mainMenuFrame;
        private StateChangeInformation _stateChangeInformation;
        private TimedSplash _splash;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);
            _splash = new TimedSplash(Game);

            _mainMenuFrame = new Frame(Game) {Title = "MainMenu"};

            var stackPanel = new StackPanel(Game) { Orientation = Orientation.Vertical };
            _mainMenuFrame.SetContent(stackPanel);

            stackPanel.AddChild(new Button(Game, () => DoTransition(typeof(LocalGameConfigurationState))) { Text = "Local Game" });
            stackPanel.AddChild(new Button(Game, () => DoTransition(typeof(NetworkGameState))) { Text = "Network Game" });
            stackPanel.AddChild(new Button(Game, () => DoTransition(typeof(OptionMenuState))) { Text = "Options" });
            stackPanel.AddChild(new Button(Game, () => DoTransition(typeof(CreditsState))) { Text = "Credits" });
            stackPanel.AddChild(new Button(Game, OnQuitClicked) { Text = "Quit" });

            var rect = _mainMenuFrame.GetMinSize();
            rect.X = Game.ScreenWidth / 2 - rect.Width / 2;
            rect.Y = Game.ScreenHeight / 2 - rect.Height / 2;
            _mainMenuFrame.Arrange(rect);
        }

        private void DoTransition(Type targetState)
        {
            _stateChangeInformation = StateChangeInformation.StateChange(targetState, SlideTransition.Id);
        }

        private void OnQuitClicked()
        {
            _stateChangeInformation = StateChangeInformation.QuitGameInformation(ZappoutTransition.Id);
        }

        protected override void OnEntered(object enterInformation)
        {
            _stateChangeInformation = StateChangeInformation.Empty;

            if (enterInformation is bool && (bool)enterInformation)
            {
                _splash.Start("Server Connection Lost", 1.5f);
            }
        }

        public override void OnLeave()
        {
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);

            _stateChangeInformation = StateChangeInformation.Empty;

            _splash.Update(elapsedTime);
            if (!_splash.Running) _mainMenuFrame.Update(elapsedTime);

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);
            _mainMenuFrame.Draw();
            _splash.Draw(Game.SpriteBatch, true);
            Cursor.Draw();
        }
    }
}
