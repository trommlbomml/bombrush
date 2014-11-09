using Bombrush.MonoGame.Gui2;
using Game2DFramework.States;

namespace Bombrush.MonoGame.States
{
    class CreditsState : BackgroundState
    {
        private Frame _frame;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);

            _frame = GuiSystem.CreateGuiHierarchyFromXml<Frame>(Game, "Content/GuiLayouts/Credits_Layout.xml");
            GuiSystem.ArrangeCenteredToScreen(Game, _frame);
        }

        public override void OnLeave()
        {
        }

        protected override void OnEntered(object enterInformation)
        {
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);

            _frame.Update(elapsedTime);

            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);

            _frame.Draw();
        }
    }
}
