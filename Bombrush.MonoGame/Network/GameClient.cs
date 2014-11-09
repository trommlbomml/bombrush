using System;
using System.Collections.Generic;
using System.Linq;
using BombRush.Controller;
using BombRush.Networking;
using BombRush.Properties;
using BombRushData;
using BombRush.Interfaces;
using BombRushData.Network;
using Game2DFramework;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace BombRush.Network
{
//    class GameClient : GameObject, ILevelDataProvider, IGameExecution
//    {
//        private const float RefreshInterval = 0.5f;

//        private readonly MessageTypeMap _messageTypeMap;
//        private float _elapsedRefreshTimeout;
//        private double _serverRefreshTimeStamp;
//        private readonly NetClient _netClient;
//        private readonly List<GameMember> _members;
//        private GameUpdateResult _updateResult;
//        private readonly IFigureController _figureController;
//        private readonly List<BombClient> _bombs;
//        private GameServer _gameServer;

//        public bool IsAdministrator { get; private set; }
//        public InGameState State { get; private set; }
//        public string ClientName { get; private set; }
//        public string ServerName { get; private set; }
//        public byte ClientId { get; private set; }
//        public List<GameMember> Members { get { return _members; } }
//        public List<ServerConnection> AvailableServers { get; private set; }
//        public MatchResultType CurrentMatchResultType { get; private set; }
//        public float RemainingStartupTime { get; private set; }
//        public float RemainingGameTime { get; private set; }
//        public List<IFigureDataProvider> Figures { get { return _members.Cast<IFigureDataProvider>().ToList(); } } 

//        public GameClient(Game2D game, GameServer server = null) : base(game)
//        {
//            _messageTypeMap = new MessageTypeMap();
//            _gameServer = server;
//            AvailableServers = new List<ServerConnection>();
//            _members = new List<GameMember>();
//            ClientId = 0xff;
//            _elapsedRefreshTimeout = 2*RefreshInterval;
//            _bombs = new List<BombClient>();
//            State = InGameState.Disconnected;

//            var configuration = new NetPeerConfiguration(GameServer.ApplicationNetworkIdentifier);
//            configuration.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

//#if !DEBUG
//            configuration.ConnectionTimeout = 5.0f;
//#endif

//            _netClient = new NetClient(configuration);
//            _netClient.Start();

//            _figureController = PlayerController.CreateNet(game, InputDeviceType.Keyboard);
//        }

//        public ILevelDataProvider LevelDataProvider
//        {
//            get { return this; }
//        }

//        public void Quit(bool regular = false)
//        {
//            if (_netClient != null)
//                _netClient.Shutdown(regular ? "Regular Quit" : "");

//            _members.Clear();
//            AvailableServers.Clear();
//            ClientId = 0xff;
//            State = InGameState.Disconnected;

//            var server = ((BombGame) Game).GameServer;
//            if (server.IsActive) server.Deactivate();
//        }

//        public void JoinLocalServer(string clientName, string serverName)
//        {
//            IsAdministrator = true;
//            ClientName = clientName;
//            ServerName = serverName;
//            _netClient.Connect("localhost", Settings.Default.MultiplayerPort, _netClient.CreateMessage(clientName));
//            State = InGameState.Lobby;
//        }

//        public void JoinServer(string clientName, ServerConnection serverToConnect)
//        {
//            IsAdministrator = false;
//            ClientName = clientName;
//            ServerName = serverToConnect.ServerName;
//            _netClient.Connect(serverToConnect.EndPoint, _netClient.CreateMessage(clientName));
//        }

//        public void SendLobbyReady(bool ready)
//        {
//            var message = new ClientReadyMessage(NetTime.Now, ClientId, ready, false);
//            message.Send(_messageTypeMap, _netClient.ServerConnection);
//        }

//        private double _serverTimeStamp;

//        public void OnQuit()
//        {
//            Quit();
//            if (_gameServer!= null && _gameServer.IsActive)
//                _gameServer.Deactivate();
//        }

//        public GameUpdateResult Update(float elapsed)
//        {
//            if (_netClient != null 
//                && _netClient.ServerConnection == null 
//                && State != InGameState.Disconnected)
//            {
//                Quit();
//                return GameUpdateResult.ServerShutdown;
//            }

//            var timeStamp = NetTime.Now;
//            _serverTimeStamp = _netClient != null && _netClient.ServerConnection != null
//                                      ? _netClient.ServerConnection.GetRemoteTime(timeStamp)
//                                      : timeStamp;

//            _updateResult = GameUpdateResult.None;
//            _netClient.HandleNetMessages(timeStamp, HandleDataMessages, HandleDiscoveryResponse);

//            switch(State)
//            {
//                case InGameState.Disconnected:
//                    HandleOnDisconnectedMode(timeStamp, elapsed);
//                    break;
//                case InGameState.InGame:
//                    HandleInGame(timeStamp, elapsed);
//                    break;
//            }

//            return _updateResult;
//        }

//        public void OnReadyForNextMatch()
//        {
//            var message = new ClientReadyMessage(NetTime.Now, ClientId, true, false);
//            message.Send(_messageTypeMap, _netClient.ServerConnection);
//            State = InGameState.PreparingMatchWaitForAllReady;
//        }

//        private void HandleInGame(double timeStamp, float elapsed)
//        {
//            _figureController.Update(elapsed);
//            if (_figureController.DirectionChanged || _figureController.ActionDone)
//            {
//                InputMessage msg = new InputMessage(timeStamp, ClientId, _figureController);
//                msg.Send(_messageTypeMap, _netClient.ServerConnection);
//            }

//            foreach (GameMember gameMember in _members)
//            {
//                gameMember.Update(this, elapsed, timeStamp, _serverTimeStamp);
//            }

//            foreach (BombClient bombThinClient in _bombs)
//            {
//                if (bombThinClient.IsActive)
//                    bombThinClient.CurrentBurnTime += elapsed;
//                else
//                    bombThinClient.CurrentBurnTime = 0;
//            }

//            foreach (OverlayClient overlayThinClient in _overlayData)
//            {
//                if (overlayThinClient.IsActive)
//                    overlayThinClient.ActiveTime += elapsed;
//                else
//                    overlayThinClient.ActiveTime = 0;
//            }
//        }

//        private void HandleOnDisconnectedMode(double timeStamp, float elapsed)
//        {
//            _elapsedRefreshTimeout += elapsed;
//            if (_elapsedRefreshTimeout > RefreshInterval)
//            {
//                _elapsedRefreshTimeout -= RefreshInterval;
//                AvailableServers.RemoveAll(i => i.TimeStamp < _serverRefreshTimeStamp);
//                _serverRefreshTimeStamp = timeStamp;
//                _netClient.DiscoverLocalPeers(Settings.Default.MultiplayerPort);
//            }
//        }

//        private void HandleDiscoveryResponse(double timeStamp, NetIncomingMessage msg)
//        {
//            string serverName = msg.ReadString();
//            byte playerCount = msg.ReadByte();
//            bool isRunning = msg.ReadBoolean();

//            ServerConnection server = AvailableServers.FirstOrDefault(i => i.EndPoint == msg.SenderEndpoint);
//            if (server != null)
//            {
//                server.Update(serverName, playerCount, timeStamp, isRunning);
//            }
//            else
//            {
//                AvailableServers.Add(new ServerConnection(serverName, playerCount, msg.SenderEndpoint, timeStamp, isRunning));
//            }
//        }

//        private void HandleDataMessages(double timeStamp, NetIncomingMessage msg)
//        {
//            var message = Message.Read(_messageTypeMap, msg);

//            if (message is RefreshClientListMessage)
//                RefreshMemberList((RefreshClientListMessage)message);
//            else if (message is ConnectionStatusMessage)
//                HandleConnectionStatusMessage((ConnectionStatusMessage) message);
//            else if (message is SwitchToReceiveDataMessage)
//            {
//                _updateResult = GameUpdateResult.SwitchToPrepareMatch;
//                State = InGameState.PreparingMatchLoadData;
//            }
//            else if (message is GameDataTransferMessage)
//                HandleGameDataTransfer((GameDataTransferMessage) message, timeStamp);
//            else if (message is MatchFinishedMessage)
//                HandleMatchedFinishedMessage((MatchFinishedMessage) message);
//            else if (message is RemainingSynchronizeTimeMessage)
//            {
//                State = InGameState.PreparingMatchSynchronizeStart;
//                RemainingStartupTime = ((RemainingSynchronizeTimeMessage)message).RemainingTime;
//            }
//            else if (message is StartGameMessage)
//            {
//                _updateResult = GameUpdateResult.SwitchToGame;
//                State = InGameState.InGame;
//            }
//            else if (message is GameStatusMessage)
//                HandleGameStatusMessage((GameStatusMessage)message);
//        }

//        private void HandleConnectionStatusMessage(ConnectionStatusMessage message)
//        {
//            if (message.ConnectionInformation == ConnectionInformation.ConnectedWithId)
//            {
//                ClientId = message.ClientId;
//                _updateResult = GameUpdateResult.ConnectedToServer;
//                State = InGameState.Lobby;
//            }
//            else
//            {
//                _netClient.Disconnect(String.Empty);
//                _updateResult = GameUpdateResult.NoUniqueName;
//            }
//        }

//        private void HandleGameStatusMessage(GameStatusMessage msg)
//        {
//            RemainingGameTime = msg.RemainingGameTime;

//            _members.RemoveAll(m => !msg.Figures.Any(f => f.Id == m.Id));

//            foreach (FigureInformation f in msg.Figures)
//            {
//                GameMember member = _members.First(m => m.Id == f.Id);
//                member.UpdateFromSnapShot(f, msg.TimeStamp);
//            }

//            for (int i = 0; i < msg.MapData.Length; i++)
//            {
//                MapDataInformation mapData = msg.MapData[i];
//                _fringe[i].IsActive = mapData.FringeActive;
//                _overlayData[i].IsActive = mapData.OverlayIsActive;

//                _itemData[i].IsActive = mapData.Item != ItemType.Empty;
//                _itemData[i].Type = mapData.Item;
//            }

//            _bombs.RemoveAll(b => !msg.Bombs.Any(bb => bb.Id == b.Id));

//            foreach (BombInformation bomb in msg.Bombs)
//            {
//                BombClient foundBomb = _bombs.FirstOrDefault(b => b.Id == bomb.Id);
//                if (foundBomb == null)
//                {
//                    foundBomb = new BombClient {Id = bomb.Id};
//                    _bombs.Add(foundBomb);
//                }
//                foundBomb.TilePosition =  GetTilePositionFromWorld(bomb.Position);
//                foundBomb.IsActive = true;
//            }
//        }

//        private void RefreshMemberList(RefreshClientListMessage msg)
//        {
//            _members.Clear();
//            foreach (RefreshClientListMessage.ClientData client in msg.Clients)
//            {
//                _members.Add(new GameMember(client.Name, client.ClientId, client.IsReady) { Me = client.ClientId == ClientId });
//            }
//            _members.Sort((a, b) => a.Id - b.Id);
//        }

//        private void HandleGameDataTransfer(GameDataTransferMessage msg, double timeStamp)
//        {
//            const int entries = BombGame.GameLevelWidth * BombGame.GameLevelHeight;

//            TilesetAssetName = msg.TileSetAssetName;
//            _data = new TileBlockClient[entries];
//            _fringe = new TileBlockClient[entries];
//            _itemData = new ItemClient[entries];
//            _overlayData = new OverlayClient[entries];
//            _bombs.Clear();

//            for (var i = 0; i < entries; i++)
//            {
//                Point tilePosition = new Point(i%BombGame.GameLevelWidth, i/BombGame.GameLevelWidth);
//                BlockType groundBlockType = msg.Data[i].GroundBlockType;

//                _data[i] = new TileBlockClient(groundBlockType) { TilePosition = tilePosition, IsActive = true };

//                BlockType fringeBlockType = msg.Data[i].FringeBlockType;
//                ItemType fringeItemType = msg.Data[i].FringeItemType;
//                _fringe[i] = new TileBlockClient(fringeBlockType)
//                {
//                    AttachedItem = fringeBlockType == BlockType.Box ? fringeItemType : ItemType.Empty,
//                    IsActive = fringeBlockType == BlockType.Box,
//                    TilePosition = tilePosition
//                };

//                _itemData[i] = new ItemClient {IsActive = false, TilePosition = tilePosition};
//                _overlayData[i] = new OverlayClient {IsActive = false, TilePosition = tilePosition};
//                _bombs.Clear();
//            }

//            foreach (GameMember gameMember in _members)
//            {
//                gameMember.InitFromServer(msg.StartUpPositions[gameMember.Id - 1]);
//                gameMember.WalkDirection = Vector2.Zero;
//            }

//            State = InGameState.PreparingMatchLoadData;
//            ClientReadyMessage message = new ClientReadyMessage(timeStamp, ClientId, true, true);
//            message.Send(_messageTypeMap, _netClient.ServerConnection);
//        }

//        private void HandleMatchedFinishedMessage(MatchFinishedMessage msg)
//        {
//            foreach (MatchPlayerInformation p in msg.Players)
//            {
//                GameMember member = _members.First(c => c.Id == p.Id);
//                member.Wins = p.Wins;
//                member.IsMatchWinner = p.IsMatchWinner;
//            }
//            CurrentMatchResultType = msg.ResultType;
//            _updateResult = GameUpdateResult.MatchFinished;
//            State = InGameState.MatchResult;
//        }

//        public string TilesetAssetName { get; private set; }

//        public List<IBombDataProvider> Bombs
//        {
//            get { return _bombs.Cast<IBombDataProvider>().ToList(); }
//        }

//        private TileBlockClient[] _data;
//        public ITileBlockDataProvider[] Data { get { return _data; } }

//        private TileBlockClient[] _fringe;
//        public ITileBlockDataProvider[] Fringe { get { return _fringe; } }

//        private ItemClient[] _itemData;
//        public IItemDataProvider[] ItemData { get { return _itemData; } }

//        private OverlayClient[] _overlayData;
//        public IExplosionFragmentDataProvider[] OverlayData { get { return _overlayData; } }

//        public IExplosionFragmentDataProvider GetOverlayInformation(Point t)
//        {
//            return _overlayData[t.Y * BombGame.GameLevelWidth + t.X];
//        }

//        public Vector2 GetWorldFromTilePositionCentered(Point p)
//        {
//            return new Vector2(p.X * BombGame.Tilesize + BombGame.Tilesize * 0.5f, p.Y * BombGame.Tilesize + BombGame.Tilesize * 0.5f);
//        }

//        public Vector2 GetWorldFromTilePosition(Point p)
//        {
//            return new Vector2(p.X * BombGame.Tilesize, p.Y * BombGame.Tilesize);
//        }

//        public Point GetTilePositionFromWorld(Vector2 position)
//        {
//            return new Point(
//                ((int)position.X) / BombGame.Tilesize,
//                ((int)position.Y) / BombGame.Tilesize);
//        }
//    }
}
