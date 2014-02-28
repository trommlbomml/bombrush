using BombRush.Gui;
using BombRush.Network;
using BombRush.Properties;
using BombRushData;
using BombRush.Logic;
using Game2DFramework.States;
using Game2DFramework.States.Transitions;

namespace BombRush.States
{
    //todo: Reactivate
    class HostNetworkGameState : BackgroundState
    {
        private StateChangeInformation _stateChangeInformation;
        private StackedMenu _hostMenu;
        //private GameClient _gameClient;
        private TimedSplash _timedSplash;

        protected override void OnInitialize(object enterInformation)
        {
            base.OnInitialize(enterInformation);
            _hostMenu = new StackedMenu(Game) { Title = "Host Network Game" };
            _hostMenu.AppendMenuItem(new ActionMenuItem(Game,"Open Match", OnOpenMatch, ActionTriggerKind.IsAccept));
            _hostMenu.AppendMenuItem(new InputMenuItem(Game, "Nickname", 10, InputType.AlphaNumeric) { InputText = Settings.Default.Nickname });
            _hostMenu.AppendMenuItem(new InputMenuItem(Game, "Server Name", 10, InputType.AlphaNumeric) { InputText = Settings.Default.ServerName });
            _hostMenu.AppendMenuItem(new EnumMenuItem(Game, "Level", new[] { "Rookie" }));
            _hostMenu.AppendMenuItem(new NumericMenuItem(Game, "Matches to Win", 3, 10));
            _hostMenu.AppendMenuItem(new TimeMenuItem(Game, "Match Time", 0, 300) { CurrentValue = Settings.Default.MultiPlayerMatchTime });
            _hostMenu.AppendMenuItem(new ActionMenuItem(Game, "Back", OnBackAction, ActionTriggerKind.IsCancel));
            _timedSplash = new TimedSplash(Game);
        }

        public override void OnLeave()
        {
        }

        protected override void OnEntered(object enterInformation)
        {
            _hostMenu.RecalculatePositions();
        }

        private void OnBackAction()
        {
            //if (_gameClient != null) _gameClient.Quit();
            _stateChangeInformation = StateChangeInformation.StateChange(typeof (MainMenuState), BlendTransition.Id);
        }

        private void OnOpenMatch()
        {
            var clientName = _hostMenu.GetMenuItem<InputMenuItem>(1).InputText;
            var serverName = _hostMenu.GetMenuItem<InputMenuItem>(2).InputText;
            var levelName = _hostMenu.GetMenuItem<EnumMenuItem>(3).SelectedItem;
            var matchesToWin = _hostMenu.GetMenuItem<NumericMenuItem>(4).CurrentValue;
            var matchTime = _hostMenu.GetMenuItem<TimeMenuItem>(5).CurrentValue;

            if (string.IsNullOrEmpty(clientName))
            {
                _timedSplash.Start("Name must not be empty", 1.5f);
                return;
            }
            
            if (Settings.Default.Nickname != clientName 
             || Settings.Default.ServerName != serverName 
             || Settings.Default.MultiplayerWins != matchesToWin
             || Settings.Default.MultiPlayerMatchTime != matchTime)
            {
                Settings.Default.Nickname = clientName;
                Settings.Default.ServerName = serverName;
                Settings.Default.MultiplayerWins = matchesToWin;
                Settings.Default.MultiPlayerMatchTime = matchTime;
                Settings.Default.Save();    
            }

            //var gameServer = ((BombGame) Game).GameServer;
            //gameServer.Activate(Settings.Default.MultiplayerPort, serverName, matchesToWin, 4);
            ////todo: das sollte über netzwerk funktionieren
            //gameServer.CreateLevel(Game.Content.Load<LevelData>(LevelHelper.GetAssetLevelName(levelName)));

            //_gameClient = new GameClient(Game, gameServer);
            //_gameClient.JoinLocalServer(clientName, serverName);
            //_stateChangeInformation = StateChangeInformation.StateChange(typeof (LobbyState), typeof (SlideTransition), _gameClient);
        }

        public override StateChangeInformation OnUpdate(float elapsedTime)
        {
            base.OnUpdate(elapsedTime);

            _timedSplash.Update(elapsedTime);
            _stateChangeInformation = StateChangeInformation.Empty;
            _hostMenu.Update(elapsedTime);
            return _stateChangeInformation;
        }

        public override void OnDraw(float elapsedTime)
        {
            base.OnDraw(elapsedTime);
            _hostMenu.Draw(Game.SpriteBatch);
            _timedSplash.Draw(Game.SpriteBatch, true);
        }
    }
}
