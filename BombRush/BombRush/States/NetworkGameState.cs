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

        private Border _tableBackgroundBorder;
        private TableView _listOfGameInstancesView;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);

            _stackedMenu = new StackedMenu(Game) { Title = "Network Game" };
            _hostAddressMenuItem = _stackedMenu.AppendMenuItem(new InputMenuItem(Game, "Server IP", 15, InputType.IpAddress) { InputText = "127.0.0.1" });
            _stackedMenu.AppendMenuItem(new ActionMenuItem(Game, "Connect", OnConnect, ActionTriggerKind.IsAccept));
            _stackedMenu.AppendMenuItem(new ActionMenuItem(Game, "Back", OnBack, ActionTriggerKind.IsAccept));

            _waitDialog = new WaitDialog(Game, 300) { Text = "Connecting to Server" };
            _timedSplash = new TimedSplash(Game);

            _gameCreationSession = new RemoteGameCreationSession(Game);
            _lastState = _gameCreationSession.State;

            _tableBackgroundBorder = new Border(Game);
            _tableBackgroundBorder.SetClientSize(200+80+80, 300);
            _tableBackgroundBorder.CenterHorizontal();
            _tableBackgroundBorder.Y = 100;

            _listOfGameInstancesView = new TableView(Game);
            _listOfGameInstancesView.AddColumn("Name", 200);
            _listOfGameInstancesView.AddColumn("Players", 80);
            _listOfGameInstancesView.AddColumn("Running", 80);
            _listOfGameInstancesView.Start = _tableBackgroundBorder.ClientStart;
        }

        private void OnBack()
        {
            _stateChangeInformation = StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition));
        }

        private void OnConnect()
        {
            var host = _hostAddressMenuItem.InputText;
            _gameCreationSession.ConnectToServer(host);
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
            
            if (_lastState == GameCreationSessionState.ConnectingToServer &&
                _gameCreationSession.State == GameCreationSessionState.ConnectionToServerFailed)
            {
                _timedSplash.Start("Unable to connect to server.", 1.0f);
            }
            _lastState = _gameCreationSession.State;

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);

            if (_gameCreationSession.State == GameCreationSessionState.Connected)
            {
                _listOfGameInstancesView.Draw(Game.SpriteBatch);
                _tableBackgroundBorder.Draw();
            }
            else
            {
                _stackedMenu.Draw(Game.SpriteBatch);
                _waitDialog.Draw();
                _timedSplash.Draw(Game.SpriteBatch, false);   
            }
        }
    }
}
