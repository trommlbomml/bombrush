using Bombrush.MonoGame.Gui;
using Game2DFramework.States;

namespace Bombrush.MonoGame.States
{
    //todo Reactivate
    class LobbyState : BackgroundState
    {
        private StackedMenu _stackedMenu;
        private ToggleActionMenuItem _sendReadyMenuItem;
        private MenuItem _startGameMenuItem;
        private TableView _playerTable;
        //private GameClient _gameClient;
        private StateChangeInformation _stateChangeInformation;
        private bool _startGameWasEnabled;

        protected override void OnEntered(object enterInformation)
        {
            //_gameClient = (GameClient)enterInformation;

            //_sendReadyMenuItem.IsTrue = false;
            //var selectionIndex = 0;
            //_stackedMenu.RemoveMenuItem(_startGameMenuItem);
            //if (_gameClient.IsAdministrator)
            //{
            //    _stackedMenu.InsertMenuItemAt(0, _startGameMenuItem);
            //    selectionIndex++;
            //}
            //_stackedMenu.SelectItem(selectionIndex);
            //_startGameWasEnabled = false;

            //_stackedMenu.RecalculatePositions();
            //_playerTable.Start = new Vector2(_stackedMenu.ClientRectangle.X, _stackedMenu.ClientRectangle.Y);

            //_playerTable.Clear();
        }

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);

            //_gameClient = (GameClient)enterInformation;

            //_playerTable = new TableView(Game)
            //{
            //    ContentColor = Color.White,
            //    HeaderColor = Color.Lime,
            //    Rows = 4,
            //};
            //_playerTable.AddColumn("PlayerName", 300);
            //_playerTable.AddColumn("Ready", 70);

            //_stackedMenu = new StackedMenu(Game)
            //{
            //    FirstMenuItemStartOffset = _playerTable.RowHeight * 5,
            //    Width = 370,
            //    Title = string.Format("Game of {0}", _gameClient.ServerName),
            //    CancelKey = Keys.Escape,
            //};
            //_startGameMenuItem = _stackedMenu.AppendMenuItem(new ActionMenuItem(Game, "Start Game", OnStartGame) { IsEnabled = false });
            //_sendReadyMenuItem = (ToggleActionMenuItem)_stackedMenu.AppendMenuItem(new ToggleActionMenuItem(Game, "Send Not Ready", "Send Ready", false, () =>                  _gameClient.SendLobbyReady(_sendReadyMenuItem.IsTrue)));
            //_stackedMenu.AppendMenuItem(new ActionMenuItem(Game, "Back", OnBack, ActionTriggerKind.IsCancel));
        }

        public override void OnLeave()
        {
        }

        private void OnBack()
        {
            //if (_gameClient != null) _gameClient.Quit();
            //_stateChangeInformation = StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition));  
        }

        private bool IsStartableGame
        {
            get { return false; } //_gameClient.Members.All(l => l.Ready) && _gameClient.Members.Count > 1; }
        }

        private void OnStartGame()
        {
            //if (_gameClient.IsAdministrator && IsStartableGame)
            //{
            //    //todo: das sollte eine funktion des clients sein.
            //    ((BombGame)Game).GameServer.StartGame(NetTime.Now);
            //    _stateChangeInformation = StateChangeInformation.StateChange(typeof(WaitState), typeof(SlideTransition), _gameClient);
            //}
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);

            _stateChangeInformation = StateChangeInformation.Empty;
            //var result = _gameClient.Update(elapsedTime);
            //_stackedMenu.Update(elapsedTime);

            //_startGameMenuItem.IsEnabled = IsStartableGame;
            //if (_startGameMenuItem.IsEnabled && !_startGameWasEnabled && _gameClient.IsAdministrator)
            //{
            //    _stackedMenu.SelectItem(0);
            //}
            //_startGameWasEnabled = _startGameMenuItem.IsEnabled;

            //switch (result)
            //{
            //    case GameUpdateResult.ServerShutdown:
            //        return StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition), true);
            //    case GameUpdateResult.SwitchToPrepareMatch:
            //        return StateChangeInformation.StateChange(typeof(WaitState), typeof(SlideTransition), _gameClient);
            //}

            //_playerTable.Clear();
            //foreach (var client in _gameClient.Members)
            //{
            //    var c = client.Me ? Color.Orange : (Color?) null;
            //    _playerTable.AddRow(new[]
            //                            {
            //                                new Cell(client.Name, client.Id),
            //                                new Cell(client.Ready ? "Yes" : "No"),
            //                            }, c);
            //}

            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);

            _stackedMenu.Draw(Game.SpriteBatch);
            _playerTable.Draw(Game.SpriteBatch);
        }
    }
}
