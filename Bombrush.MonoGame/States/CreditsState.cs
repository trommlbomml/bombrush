using Game2DFramework.Gui;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;

namespace Bombrush.MonoGame.States
{
    class CreditsState : BackgroundState
    {
        private GuiPanel _panel;
        private StateChangeInformation _stateChangeInformation;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);

            _panel = new GuiPanel(Game);

            var frame = Game.GuiSystem.CreateGuiHierarchyFromXml<Frame>( "Content/GuiLayouts/Credits_Layout.xml");
            Game.GuiSystem.ArrangeCenteredToScreen(Game, frame);

            frame.FindGuiElementById<Button>("BackButton").Click += () =>
            {
                _stateChangeInformation = StateChangeInformation.StateChange(typeof (MainMenuState),
                    typeof (BlendTransition));
            };

            _panel.AddElement(frame);
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

            _panel.Update(elapsedTime);

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);

            _panel.Draw();
        }
    }
}
