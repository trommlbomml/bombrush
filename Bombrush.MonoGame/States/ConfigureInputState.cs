using BombRush.Gui;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;

namespace BombRush.States
{
    class ConfigureInputState : BackgroundState
    {
        private StackedMenu _inputMenu;
        private StateChangeInformation _stateChangeInformation;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);
            _inputMenu = new StackedMenu(Game) { Title = "Input" };
            _inputMenu.AppendMenuItem(new EnumMenuItem(Game, "Player", new[] { "1", "2", "3", "4" }));
            _inputMenu.AppendMenuItem(new EnumMenuItem(Game, "Device", new[] { "Keyboard", "GamePad" }));
            _inputMenu.AppendMenuItem(new ActionMenuItem(Game, "Configure", () => { }));
            _inputMenu.AppendMenuItem(new ActionMenuItem(Game, "Back", HandleBack, ActionTriggerKind.IsCancel));
        }

        protected override void OnEntered(object enterInformation)
        {
            _inputMenu.RecalculatePositions();
            _stateChangeInformation = StateChangeInformation.Empty;
        }

        private void HandleBack()
        {
            _stateChangeInformation = StateChangeInformation.StateChange(typeof(MainMenuState), BlendTransition.Id);
        }

        public override void OnLeave()
        {
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);
            _inputMenu.Update(elapsedTime);
            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);
            _inputMenu.Draw(Game.SpriteBatch);
        }
    }
}
