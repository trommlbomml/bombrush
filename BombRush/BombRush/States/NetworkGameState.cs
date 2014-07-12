using BombRush.Gui;
using BombRush.Network;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;

namespace BombRush.States
{
    class NetworkGameState : BackgroundState
    {
        private TimedSplash _timedSplash;
        private GameCreationSessionState _lastState;
        private WaitDialog _waitDialog;
        private RemoteGameCreationSession _gameCreationSession;
        private StackedMenu _stackedMenu;
        private StateChangeInformation _stateChangeInformation;
        private InputMenuItem _hostAddressMenuItem;
        private InputMenuItem _playerNameMenuItem;

        private Border _tableBackgroundBorder;
        private DataTableView _listOfGameInstances;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);

            _stackedMenu = new StackedMenu(Game) { Title = "Network Game" };
            _hostAddressMenuItem = _stackedMenu.AppendMenuItem(new InputMenuItem(Game, "Server IP", 15, InputType.HostNames) { InputText = "localhost" });
            _playerNameMenuItem = _stackedMenu.AppendMenuItem(new InputMenuItem(Game, "Your Name", 15, InputType.AlphaNumeric) { InputText = "guest" });
            _stackedMenu.AppendMenuItem(new ActionMenuItem(Game, "Connect", OnConnect, ActionTriggerKind.IsAccept));
            _stackedMenu.AppendMenuItem(new ActionMenuItem(Game, "Back", OnBack, ActionTriggerKind.IsAccept));

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
            var host = _hostAddressMenuItem.InputText;
            var name = _playerNameMenuItem.InputText;
            _gameCreationSession.ConnectToServer(host, name);
        }

        protected override void OnEntered(object enterInformation)
        {
            _stackedMenu.SelectFirstMenuItem();
        }

        public override void OnLeave()
        {
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);

            _stateChangeInformation = StateChangeInformation.Empty;

            _waitDialog.IsActive = _gameCreationSession.State == GameCreationSessionState.ConnectingToServer;
            if (!_gameCreationSession.IsBusy) _stackedMenu.Update(elapsedTime);

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

            if (_gameCreationSession.State == GameCreationSessionState.Connected)
            {
                _tableBackgroundBorder.Draw();
                _listOfGameInstances.Draw();
            }
            else
            {
                _stackedMenu.Draw(Game.SpriteBatch);
                _waitDialog.Draw();
                _timedSplash.Draw(Game.SpriteBatch, false);   
            }
            Cursor.Draw();
        }
    }
}
