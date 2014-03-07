
using BombRush.Rendering;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using BombRush.Gui;

namespace BombRush.States
{
    class MainMenuState : BackgroundState
    {
        private Cursor _cursor;
        private StackedMenu _mainMenu;
        private StateChangeInformation _stateChangeInformation;
        private TimedSplash _splash;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);
            _splash = new TimedSplash(Game);
            _cursor = new Cursor(Game);

            _mainMenu = new StackedMenu(Game) { Title = "MainMenu" };
            _mainMenu.AppendMenuItem(new ActionMenuItem(Game,"Local Game", () => _stateChangeInformation = StateChangeInformation.StateChange(typeof(LocalGameConfigurationState), SlideTransition.Id), ActionTriggerKind.IsAccept));
            _mainMenu.AppendMenuItem(new ActionMenuItem(Game, "Network Game", () => _stateChangeInformation = StateChangeInformation.StateChange(typeof(NetworkGameState), SlideTransition.Id), ActionTriggerKind.IsAccept));
            _mainMenu.AppendMenuItem(new ActionMenuItem(Game, "Options", () => _stateChangeInformation = StateChangeInformation.StateChange(typeof(OptionMenuState), SlideTransition.Id), ActionTriggerKind.IsAccept));
            _mainMenu.AppendMenuItem(new ActionMenuItem(Game, "Credits", () => _stateChangeInformation = StateChangeInformation.StateChange(typeof(CreditsState), SlideTransition.Id), ActionTriggerKind.IsAccept));
            _mainMenu.AppendMenuItem(new ActionMenuItem(Game, "Quit", () => _stateChangeInformation = StateChangeInformation.QuitGameInformation(ZappoutTransition.Id), ActionTriggerKind.IsCancel));
        }

        protected override void OnEntered(object enterInformation)
        {
            _mainMenu.SelectFirstMenuItem();
            _mainMenu.RecalculatePositions();
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

            _cursor.Update();
            _splash.Update(elapsedTime);
            if (!_splash.Running) _mainMenu.Update(elapsedTime);

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);
            _mainMenu.Draw(Game.SpriteBatch);
            _splash.Draw(Game.SpriteBatch, true);
            _cursor.Draw();
        }
    }
}
