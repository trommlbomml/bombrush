using Bombrush.MonoGame.Network;
using Game2DFramework.Gui;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;

namespace Bombrush.MonoGame.States
{
    class NetworkGameState : BackgroundState
    {
        private GuiPanel _panel;
        private GuiElement _connectToServerFrame;
        private Frame _waitDialog;
        private Frame _errorDialog;

        private RemoteGameCreationSession _gameCreationSession;
        private StateChangeInformation _stateChangeInformation;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);

            _panel = new GuiPanel(Game);

            _connectToServerFrame =
                Game.GuiSystem.CreateGuiHierarchyFromXml<GuiElement>("Content/GuiLayouts/NetworkGame_Layout.xml");

            Game.GuiSystem.ArrangeCenteredToScreen(Game, _connectToServerFrame);

            _connectToServerFrame.FindGuiElementById<Button>("ConnectButton").Click += OnConnect;
            _connectToServerFrame.FindGuiElementById<Button>("BackButton").Click += OnBack;

            _panel.AddElement(_connectToServerFrame);

            _waitDialog = Game.GuiSystem.CreateGuiHierarchyFromXml<Frame>("Content/GuiLayouts/WaitDialog.xml");
            _waitDialog.IsActive = false;
            _waitDialog.Title = string.Empty;
            _waitDialog.FindGuiElementById<TextBlock>("MessageText").Text = "Connecting to Server...";
            Game.GuiSystem.ArrangeCenteredToScreen(Game, _waitDialog);
            _panel.AddElement(_waitDialog);

            _errorDialog = Game.GuiSystem.CreateGuiHierarchyFromXml<Frame>("Content/GuiLayouts/ConfirmationDialog.xml");
            _errorDialog.IsActive = false;
            _errorDialog.Title = string.Empty;
            _errorDialog.Color = Color.Red;
            _errorDialog.FindGuiElementById<Button>("ConfirmButton").Click += () =>
            {
                _errorDialog.IsActive = false;
                _connectToServerFrame.IsActive = true;
            };
            Game.GuiSystem.ArrangeCenteredToScreen(Game, _errorDialog);
            _panel.AddElement(_errorDialog);

            _gameCreationSession = new RemoteGameCreationSession(Game);
        }

        private void OnBack()
        {
            _stateChangeInformation = StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition));
        }

        private void OnConnect()
        {
            var host = _connectToServerFrame.FindGuiElementById<TextBox>("ServerTextBox").Text;
            var name = _connectToServerFrame.FindGuiElementById<TextBox>("NickNameTextBox").Text;
            _gameCreationSession.ConnectToServer(host, name);
            _connectToServerFrame.IsActive = false;
            _waitDialog.IsActive = true;
        }

        protected override void OnEntered(object enterInformation)
        {
            _connectToServerFrame.FindGuiElementById<TextBox>("ServerTextBox").Text = "localhost";
            _connectToServerFrame.FindGuiElementById<TextBox>("NickNameTextBox").Text = "guest";
        }

        public override void OnLeave()
        {
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);

            _stateChangeInformation = StateChangeInformation.Empty;
            _panel.Update(elapsedTime);

            if (_gameCreationSession.State == GameCreationSessionState.ConnectionToServerFailed)
            {
                _waitDialog.IsActive = false;
                _errorDialog.FindGuiElementById<TextBlock>("MessageText").Text = _gameCreationSession.GetConnectionFailedMessageAndReset();
                Game.GuiSystem.ArrangeCenteredToScreen(Game, _errorDialog);
                _errorDialog.IsActive = true;
            }
            else if (_gameCreationSession.State == GameCreationSessionState.Connected)
            {
                //do different.
            }

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);
            _panel.Draw();
        }
    }
}
