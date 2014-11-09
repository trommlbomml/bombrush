using System;
using Bombrush.MonoGame.Gui;
using Bombrush.MonoGame.Gui2;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;

namespace Bombrush.MonoGame.States
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

            _mainMenuFrame = GuiSystem.CreateGuiHierarchyFromXml<Frame>(Game, "Content/GuiLayouts/MainMenu_Layout.xml");
            GuiSystem.ArrangeCenteredToScreen(Game, _mainMenuFrame);

            _mainMenuFrame.FindGuiElementById<Button>("LocalGameButton").OnClick = () => DoTransition(typeof (LocalGameConfigurationState));
            _mainMenuFrame.FindGuiElementById<Button>("NetworkGameButton").OnClick = () => DoTransition(typeof(NetworkGameState));
            _mainMenuFrame.FindGuiElementById<Button>("OptionsButton").OnClick = () => DoTransition(typeof(OptionMenuState));
            _mainMenuFrame.FindGuiElementById<Button>("CreditsButton").OnClick = () => DoTransition(typeof(CreditsState));
            _mainMenuFrame.FindGuiElementById<Button>("QuitButton").OnClick = OnQuitClicked;
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
        }
    }
}
