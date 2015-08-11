using Bombrush.MonoGame.Gui;
using Bombrush.MonoGame.Network;
using Game2DFramework.Gui;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;

namespace Bombrush.MonoGame.States
{
    class NetworkGameState : BackgroundState
    {
        private GuiPanel _panel;
        private GuiElement _connectToServerFrame;

        private TimedSplash _timedSplash;
        private GameCreationSessionState _lastState;
        private WaitDialog _waitDialog;
        private RemoteGameCreationSession _gameCreationSession;
        private StateChangeInformation _stateChangeInformation;

        private Border _tableBackgroundBorder;
        private DataTableView _listOfGameInstances;

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

            _waitDialog = new WaitDialog(Game, 300) { Text = "Connecting to Server" };
            _timedSplash = new TimedSplash(Game);

            _gameCreationSession = new RemoteGameCreationSession(Game);
            _lastState = _gameCreationSession.State;

            _listOfGameInstances = new DataTableView(Game, GetRowCount, UpdateTableRow);
            _listOfGameInstances.AddColumn("Name", 200);
            _listOfGameInstances.AddColumn("Players", 80);
            _listOfGameInstances.AddColumn("Running", 80);

            _tableBackgroundBorder = new Border(Game);
            _tableBackgroundBorder.SetClientSize(_listOfGameInstances.Width, _listOfGameInstances.Height);
            _tableBackgroundBorder.CenterHorizontal();
            _tableBackgroundBorder.Y = 100;
            
            _listOfGameInstances.X = _tableBackgroundBorder.ClientX;
            _listOfGameInstances.Y = _tableBackgroundBorder.ClientY;
        }

        private void UpdateTableRow(int i, DataTableRow dataTableRow)
        {
            var currentItem = _gameCreationSession.RunningGameInstances[i];
            _listOfGameInstances.Draw();
            dataTableRow.Columns[0] = currentItem.Name;
            dataTableRow.Columns[1] = currentItem.PlayerCount.ToString("D");
            dataTableRow.Columns[2] = currentItem.IsRunning ? "Yes" : "No";
        }

        private int GetRowCount()
        {
            return _gameCreationSession.RunningGameInstances.Count;
        }

        private void OnBack()
        {
            _stateChangeInformation = StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition));
        }

        private void OnConnect()
        {
            var host = _connectToServerFrame.FindGuiElementById<Game2DFramework.Gui.TextBox>("ServerTextBox").Text;
            var name = _connectToServerFrame.FindGuiElementById<Game2DFramework.Gui.TextBox>("NickNameTextBox").Text;
            _gameCreationSession.ConnectToServer(host, name);
            _connectToServerFrame.IsActive = false;
        }

        protected override void OnEntered(object enterInformation)
        {
            _connectToServerFrame.FindGuiElementById<Game2DFramework.Gui.TextBox>("ServerTextBox").Text = "localhost";
            _connectToServerFrame.FindGuiElementById<Game2DFramework.Gui.TextBox>("NickNameTextBox").Text = "guest";
        }

        public override void OnLeave()
        {
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);
            _panel.Update(elapsedTime);

            _stateChangeInformation = StateChangeInformation.Empty;

            _waitDialog.IsActive = _gameCreationSession.State == GameCreationSessionState.ConnectingToServer;

            _waitDialog.Update(elapsedTime);
            _timedSplash.Update(elapsedTime);
            _listOfGameInstances.Update();
            
            if (_lastState == GameCreationSessionState.ConnectingToServer &&
                _gameCreationSession.State == GameCreationSessionState.ConnectionToServerFailed)
            {
                _timedSplash.Start("Unable to connect:" + _gameCreationSession.ConnectionFailedMessage, 2.0f);
            }
            _lastState = _gameCreationSession.State;

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);

            _panel.Draw();

            if (_gameCreationSession.State == GameCreationSessionState.Connected)
            {
                _tableBackgroundBorder.Draw();
                _listOfGameInstances.Draw();
            }
            else
            {
                _waitDialog.Draw();
                _timedSplash.Draw(Game.SpriteBatch, false);   
            }
        }
    }
}
