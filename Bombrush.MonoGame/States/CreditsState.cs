using Game2DFramework.Gui;
using Game2DFramework.States;

namespace Bombrush.MonoGame.States
{
    class CreditsState : BackgroundState
    {
        private GuiPanel _panel;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);

            _panel = new GuiPanel(Game);

            var frame = Game.GuiSystem.CreateGuiHierarchyFromXml<Frame>( "Content/GuiLayouts/Credits_Layout.xml");
            Game.GuiSystem.ArrangeCenteredToScreen(Game, frame);

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
            base.OnUpdate(elapsedTime);

            _panel.Update(elapsedTime);

            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);

            _panel.Draw();
        }
    }
}
