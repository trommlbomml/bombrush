using BombRush.Gui2;
using BombRush.Rendering;
using Game2DFramework;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.States
{
    class GuiTestState : IState
    {
        private Cursor _cursor;
        private Frame _root;
        private StateChangeInformation _stateChangeInformation;

        public Game2D Game { get; set; }
        public void OnEnter(object enterInformation)
        {
            _cursor = new Cursor(Game);
            _root = new Frame(Game);
            _root.Title = "Main Menu";
            
            var stackPanel = new StackPanel(Game) {Orientation = Orientation.Vertical};
            _root.SetContent(stackPanel);

            stackPanel.AddChild(new Button(Game) { Text = "First Menu Item"});
            stackPanel.AddChild(new TextBlock(Game) { Text = "Second Menu Item" });
            stackPanel.AddChild(new TextBlock(Game) { Text = "Second Menu Item" });

            var rect = _root.GetMinSize();
            rect.X = Game.ScreenWidth/2 - rect.Width/2;
            rect.Y = Game.ScreenHeight / 2 - rect.Height / 2;
            _root.Arrange(rect);
        }

        private void OnItemClicked()
        {
            _stateChangeInformation = StateChangeInformation.StateChange(typeof (MainMenuState),
                typeof (SlideTransition));
        }

        public void OnLeave()
        {
        }

        public StateChangeInformation OnUpdate(float elapsedTime)
        {
            _stateChangeInformation = StateChangeInformation.Empty;

            _cursor.Update();
            _root.Update(elapsedTime);
            return _stateChangeInformation;
        }

        public void OnDraw(float elapsedTime)
        {
            Game.GraphicsDevice.Clear(Color.Black);

            _root.Draw();

            _cursor.Draw();
        }

        public RenderTarget2D TransitionRenderTarget { get; set; }
    }
}
