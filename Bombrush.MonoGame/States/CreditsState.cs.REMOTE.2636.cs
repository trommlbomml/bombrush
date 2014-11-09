using BombRush.Gui2;
using BombRush.Rendering;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;

namespace BombRush.States
{
    class CreditsState : BackgroundState
    {
        private Frame _frame;
        private Cursor _cursor;
        private StateChangeInformation _stateChangeInformation;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);

            _cursor = new Cursor(Game);

            _frame = GuiSystem.CreateGuiHierarchyFromXml<Frame>(Game, "LocalResources/Credits_Layout.xml");
            GuiSystem.ArrangeCenteredToScreen(Game, _frame);

            var button = _frame.FindGuiElementById<Button>("BackButton");
            button.OnClick = OnBackButtonClicked;
        }

        private void OnBackButtonClicked()
        {
            _stateChangeInformation = StateChangeInformation.StateChange(typeof (MainMenuState), typeof (BlendTransition));
        }

        public override void OnLeave()
        {
        }

        protected override void OnEntered(object enterInformation)
        {
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            _stateChangeInformation = StateChangeInformation.Empty;

            base.OnUpdate(elapsedTime);

            _frame.Update(elapsedTime);
            _cursor.Update();

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);

            _frame.Draw();
            _cursor.Draw();
        }
    }
}
