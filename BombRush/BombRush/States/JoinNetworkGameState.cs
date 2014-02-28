
using BombRush.Gui;
using BombRush.Network;
using BombRush.Properties;
using BombRush.Interfaces;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BombRush.States
{
    //todo: Reactivate.
    class JoinNetworkGameState : BackgroundState
    {
        private const int RowHeight = 30;

        private TimedSplash _timedSplash;
        //private GameClient _gameClient;
        private int _currentMenuIndex;
        private InputMenuItem _clientNameTextBox;
        private AnimatedBomb _bomb;
        private TitledBorder _titledBorder;
        private TableView _serverTable;
        private Statusbar _statusBar;
        private bool _firstServerSelected;

        private const float TableWidth = 430; 

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);

            _titledBorder = new TitledBorder(Game, "Join Game", (int)TableWidth + 2 * Border.BorderSize + 120, 280, BombGame.MenuStartY);

            _serverTable = new TableView(Game)
            {
                ContentColor = Color.White,
                HeaderColor = Color.Lime,
                Rows = 4,
            };
            _serverTable.AddColumn("Server Name", 200);
            _serverTable.AddColumn("IP Address", 150);
            _serverTable.AddColumn("State", 80);

            _clientNameTextBox = new InputMenuItem(Game, "Nickname", 10, InputType.AlphaNumeric, true)
            {
                InputText = Settings.Default.Nickname
            };

            _bomb = new AnimatedBomb(Game.Content);

            _statusBar = new Statusbar(Game) { ItemSpacing = 20, Padding = 0, Width = (int)TableWidth + 2 * Border.BorderSize + 120 };
            _statusBar.AddItem("Enter - Join");
            _statusBar.AddItem("Esc - Back");

            _timedSplash = new TimedSplash(Game);
        }

        public override void OnLeave()
        {
        }

        protected override void OnEntered(object enterInformation)
        {
            _titledBorder.SetSize((int)TableWidth + 2 * Border.BorderSize + 120, 280, BombGame.MenuStartY);
            _serverTable.Start = new Vector2((Game.ScreenWidth - TableWidth)*0.5f, _titledBorder.ClientRectangle.Y + RowHeight);
            _clientNameTextBox.Position = new Vector2(Game.ScreenWidth * 0.5f, _titledBorder.ClientRectangle.Y);

            //_gameClient = new GameClient(Game);
            _serverTable.Clear();
            _firstServerSelected = false;
            _currentMenuIndex = 0;
        }

        //private void AddServerInformationRow(ServerConnection serverInformation)
        //{
        //    var stateInformation = string.Format("{0} / 4", serverInformation.PlayerCount);

        //    if (serverInformation.Running)
        //        stateInformation = "Running";
        //    else if (serverInformation.PlayerCount == 4)
        //        stateInformation = "Full";

        //    _serverTable.AddRow(new[]
        //    {
        //        new Cell(serverInformation.ServerName),
        //        new Cell(serverInformation.EndPoint.Address.ToString()), 
        //        new Cell(stateInformation), 
        //    });   
        //}

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            //base.OnUpdate(elapsedTime);

            //_timedSplash.Update(elapsedTime);
            //var result = _gameClient.Update(elapsedTime);

            //switch(result)
            //{
            //    case GameUpdateResult.ServerShutdown:
            //        return StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition), true);
            //    case GameUpdateResult.ConnectedToServer:
            //        return StateChangeInformation.StateChange(typeof(LobbyState), typeof(SlideTransition), _gameClient);
            //    case GameUpdateResult.NoUniqueName:
            //        _timedSplash.Start(string.Format("The name {0} is already chosen", _clientNameTextBox.InputText), 1.5f);
            //        break;
            //}

            //_serverTable.Clear();
            //foreach (var serverInformation in _gameClient.AvailableServers)
            //{
            //    AddServerInformationRow(serverInformation);
            //}

            //if (!_firstServerSelected && _gameClient.AvailableServers.Count > 0)
            //{
            //    _firstServerSelected = true;
            //    _currentMenuIndex = 1;
            //}

            //float y = _titledBorder.ClientRectangle.Y;
            //if (_currentMenuIndex == 0)
            //{
            //    _clientNameTextBox.Update(elapsedTime);
            //}
            //else
            //{
            //    y = _serverTable.Start.Y + _currentMenuIndex * _serverTable.RowHeight;
            //}
            //_bomb.Position = new Vector2(_serverTable.Start.X - 40, y + _serverTable.RowHeight * 0.5f);
            //_bomb.Update(elapsedTime);

            //if (_timedSplash.Running)
            //    return StateChangeInformation.Empty;

            //if (Game.Keyboard.IsKeyDownOnce(Keys.Escape))
            //{
            //    _gameClient.Quit();
            //    return StateChangeInformation.StateChange(typeof(MainMenuState), typeof(BlendTransition));
            //}

            //if (Game.Keyboard.IsKeyDownOnce(Keys.Enter) && _currentMenuIndex > 0)
            //{
            //    if (string.IsNullOrEmpty(_clientNameTextBox.InputText))
            //    {
            //        _timedSplash.Start("Name must not be empty", 1.5f);
            //        return StateChangeInformation.Empty;
            //    }

            //    ServerConnection selectedServer = _gameClient.AvailableServers[_currentMenuIndex - 1];
            //    if (!selectedServer.Running && selectedServer.PlayerCount < 4)
            //    {

            //        string clientName = _clientNameTextBox.InputText;
            //        _gameClient.JoinServer(_clientNameTextBox.InputText, selectedServer);

            //        Settings.Default.Nickname = clientName;
            //        Settings.Default.Save();
            //    }
            //}

            //if (Game.Keyboard.IsKeyDownOnce(Keys.Down))
            //{
            //    if (++_currentMenuIndex == _gameClient.AvailableServers.Count + 1)
            //        _currentMenuIndex = 0;
            //}

            //if (Game.Keyboard.IsKeyDownOnce(Keys.Up))
            //{
            //    if (--_currentMenuIndex < 0)
            //        _currentMenuIndex = _gameClient.AvailableServers.Count;
            //}            

            return StateChangeInformation.Empty;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);

            _titledBorder.Draw(Game.SpriteBatch, true);
            _clientNameTextBox.Draw(Game.SpriteBatch);
            _serverTable.Draw(Game.SpriteBatch);
            _bomb.Draw(Game.SpriteBatch);
            _statusBar.Draw(Game.SpriteBatch, true);
            _timedSplash.Draw(Game.SpriteBatch, true);
        }
    }
}
