using BombRush.Gui2;
using BombRush.Rendering;
using Game2DFramework.States;

namespace BombRush.States
{
    class CreditsState : BackgroundState
    {
        private Frame _frame;
        private Cursor _cursor;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);

            _cursor = new Cursor(Game);

            _frame = GuiSystem.CreateGuiHierarchyFromXml<Frame>(Game, "LocalResources/Credits_Layout.xml");
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
            _cursor.Update();

            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);

            _frame.Draw();
            _cursor.Draw();
        }
    }
}
