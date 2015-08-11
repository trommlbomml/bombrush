using System;
using Bombrush.MonoGame.Gui;
using Game2DFramework.Gui;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;

namespace Bombrush.MonoGame.States
{
    class MainMenuState : BackgroundState
    {
        private StateChangeInformation _stateChangeInformation;
        private TimedSplash _splash;
        private GuiPanel _panel;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);
            _splash = new TimedSplash(Game);

            var mainMenuFrame = Game.GuiSystem.CreateGuiHierarchyFromXml<GuiElement>("Content/GuiLayouts/MainMenu_Layout.xml");
            Game.GuiSystem.ArrangeCenteredToScreen(Game, mainMenuFrame);

            mainMenuFrame.FindGuiElementById<Button>("LocalGameButton").Click += () => DoTransition(typeof(LocalGameConfigurationState));
            mainMenuFrame.FindGuiElementById<Button>("NetworkGameButton").Click += () => DoTransition(typeof(NetworkGameState));
            mainMenuFrame.FindGuiElementById<Button>("OptionsButton").Click += () => DoTransition(typeof(OptionMenuState));
            mainMenuFrame.FindGuiElementById<Button>("CreditsButton").Click += () => DoTransition(typeof(CreditsState));
            mainMenuFrame.FindGuiElementById<Button>("QuitButton").Click += OnQuitClicked;

            _panel = new GuiPanel(Game);
            _panel.AddElement(mainMenuFrame);
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
            if (!_splash.Running) _panel.Update(elapsedTime);

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);
            _panel.Draw();
            _splash.Draw(Game.SpriteBatch);
        }
    }
}
