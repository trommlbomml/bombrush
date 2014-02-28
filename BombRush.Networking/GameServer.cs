using System;
using System.Collections.Generic;
using System.Linq;
using BombRush.Interfaces;
using Game2DFramework.Interaction;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using BombRush.Logic;

namespace BombRush.Networking
{
    //public enum GameServerState
    //{
    //    Lobby,
    //    ProvideDataToClients,
    //    ProvidedAndWaitForallReady,
    //    SynchronizeBeforeGameStart,
    //    InGame,
    //    MatchResult,
    //}

    //public class GameServer
    //{
    //    public const string ApplicationNetworkIdentifier = "BombRushNetworkGameIdentifier";

    //    private const float HeadStartupTimeTrigger = 0.2f;
    //    private const float SynchronizeGameSnapShotTime = 0.02f;
    //    private const float HeadStartupTime = 3.0f;
    //    private const float RefreshPlayListTime = 0.5f;

    //    private readonly object _lock = new object();

    //    private MessageTypeMap _messageTypeMap;
    //    private LevelImp _gameLevel;
    //    private List<ClientInformation> _clients;
    //    private NetServer _server;
    //    private string _serverName;
    //    private GameServerState _currentState;
    //    private int _matchesToWin;
    //    private int _matchTime;
    //    private readonly ActionTimer _syncPlayListTimer;
    //    private readonly ActionTimer _inGameSnapShotSendTimer;
    //    private readonly ActionTimer _headStartupTimer;

    //    public GameServer()
    //    {
    //        _messageTypeMap = new MessageTypeMap();
    //        _inGameSnapShotSendTimer = new ActionTimer(SendGameSnapshot, SynchronizeGameSnapShotTime, false);
    //        _headStartupTimer = new ActionTimer(SendWaitSynchronizeMessage, HeadStartupTimeTrigger, false);
    //        _syncPlayListTimer = new ActionTimer(RefreshPlayListOnAllClients, RefreshPlayListTime, false);
    //    }

    //    public bool IsActive
    //    {
    //        get 
    //        { 
    //            return _server != null && _server.Status == NetPeerStatus.Running; 
    //        }
    //    }

    //    public void Activate(int port, string serverName, int matchesToWin, int matchTime)
    //    {
    //        Deactivate();
    //        _clients = new List<ClientInformation>();
    //        _gameLevel = new LevelImp(matchesToWin, matchTime);
    //        _currentState = GameServerState.Lobby;
    //        _syncPlayListTimer.Start();
    //        _matchesToWin = matchesToWin;
    //        _matchTime = matchTime;
    //        _serverName = serverName;
            
    //        var configuration = new NetPeerConfiguration(ApplicationNetworkIdentifier);
    //        configuration.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
    //        configuration.Port = port;

    //        _server = new NetServer(configuration);
    //        _server.RegisterReceivedCallback(s => 
    //        {
    //            lock (_lock)
    //            {
    //                var timeStamp = NetTime.Now;
    //                _server.HandleNetMessages(timeStamp, HandleDataMessages, HandleStatusChangedServer, SendDiscoveryResponse);
    //            }
    //        });
    //        _server.Start();
    //    }

    //    public void Deactivate()
    //    {
    //        if (_server != null)
    //            _server.Shutdown("Server Shutdown");
    //    }

    //    public void CreateLevel(LevelData levelData)
    //    {
    //        _gameLevel.GenerateLevel(levelData);
    //    }

    //    public void StartGame(double timeStamp)
    //    {
    //        _currentState = GameServerState.ProvideDataToClients;
            
    //        var message = new SwitchToReceiveDataMessage(timeStamp);
    //        message.SendToAll(_messageTypeMap, _server);
    //    }

    //    private void SendGameSnapshot(double timeStamp)
    //    {
    //        var msg = new GameStatusMessage(timeStamp,_gameLevel.RemainingGameTime, _gameLevel);
    //        msg.SendToAll(_messageTypeMap, _server);
    //    }
        
    //    public void Update(double timeStamp, float elapsedTime)
    //    {
    //        lock (_lock)
    //        {
    //            if (_server == null || _server.Status == NetPeerStatus.NotRunning)
    //                return;
                
    //            switch (_currentState)
    //            {
    //                case GameServerState.Lobby:
    //                    HandleLobby(timeStamp, elapsedTime);
    //                    break;
    //                case GameServerState.ProvideDataToClients:
    //                    HandleProvideDataToClients(timeStamp);
    //                    break;
    //                case GameServerState.ProvidedAndWaitForallReady:
    //                    HandleProvidedAndWaitForallReady();
    //                    break;
    //                case GameServerState.SynchronizeBeforeGameStart:
    //                    HandleSynchronizeBeforeGameStart(timeStamp, elapsedTime);
    //                    break;
    //                case GameServerState.InGame:
    //                    HandleInGame(timeStamp, elapsedTime);
    //                    break;
    //                case GameServerState.MatchResult:
    //                    HandleMatchResult(timeStamp);
    //                    break;
    //            }
    //        }
    //    }

    //    private void HandleLobby(double timeStamp, float elapsedTime)
    //    {
    //        _syncPlayListTimer.Update(timeStamp, elapsedTime);
    //    }

    //    private void HandleProvideDataToClients(double timeStamp)
    //    {
    //        _gameLevel.GenerateContent();
    //        foreach (ClientInformation client in _clients)
    //        {
    //            client.Figure.Position = _gameLevel.GetWorldStartupPosition(client.Figure.Id);
    //            client.Figure.WalkDirection = Vector2.Zero;
    //        }

    //        Vector2[] startUps = new[]
    //        {
    //            _gameLevel.GetWorldStartupPosition(1),
    //            _gameLevel.GetWorldStartupPosition(2),
    //            _gameLevel.GetWorldStartupPosition(3),
    //            _gameLevel.GetWorldStartupPosition(4)
    //        };

    //        GameDataTransferMessage msg = new GameDataTransferMessage(timeStamp, startUps, _gameLevel.TilesetAssetName, GetMapData());
    //        msg.SendToAll(_messageTypeMap, _server);

    //        _currentState = GameServerState.ProvidedAndWaitForallReady;
    //    }

    //    private void HandleProvidedAndWaitForallReady()
    //    {
    //        if (_clients.All(c => c.LoadForGameReady))
    //        {
    //            _headStartupTimer.Start();
    //            _currentState = GameServerState.SynchronizeBeforeGameStart;
    //        }
    //    }

    //    private void HandleSynchronizeBeforeGameStart(double timeStamp, float elapsed)
    //    {
    //        _headStartupTimer.Update(timeStamp, elapsed);

    //        if (_headStartupTimer.TotalElapsedTime >= HeadStartupTime)
    //        {
    //            _headStartupTimer.Stop();
    //            _inGameSnapShotSendTimer.Start();
    //            _gameLevel.StartMatch();
    //            _currentState = GameServerState.InGame;
    //            var msg = new StartGameMessage(timeStamp);
    //            msg.SendToAll(_messageTypeMap, _server);
    //        }
    //    }

    //    private void HandleMatchResult(double timeStamp)
    //    {
    //        if (_clients.All(c => c.Ready))
    //        {
    //            StartGame(timeStamp);
    //            _clients.ForEach(c => c.Figure.ReSpawn());
    //        }
    //    }

    //    private void HandleInGame(double timeStamp, float elapsed)
    //    {
    //        MatchResultType matchResult = _gameLevel.Update(elapsed);
    //        _inGameSnapShotSendTimer.Update(timeStamp, elapsed);

    //        if (_currentState != GameServerState.InGame)
    //            return;

    //        if (matchResult != MatchResultType.None)
    //            ProvideMatchFinished(timeStamp, matchResult);
    //    }

    //    private void SendWaitSynchronizeMessage(double timeStamp)
    //    {
    //        var msg = new RemainingSynchronizeTimeMessage(timeStamp, HeadStartupTime - _headStartupTimer.TotalElapsedTime);
    //        msg.SendToAll(_messageTypeMap, _server);
    //    }

    //    private byte GetClientId()
    //    {
    //        for (byte i = 1; i < 5; i++)
    //        {
    //            if (_clients.All(c => c.Id != i)) return i;
    //        }
    //        throw new InvalidOperationException("Client ID could not be set correctly");
    //    }

    //    private void HandleStatusChangedServer(double timeStamp, NetIncomingMessage msg)
    //    {
    //        var connectionStatus = (NetConnectionStatus)msg.ReadByte();

    //        if (connectionStatus == NetConnectionStatus.Connected)
    //        {
    //            if (!_clients.Any(p => p.EndPoint.Equals(msg.SenderEndpoint))
    //                && msg.SenderConnection.RemoteHailMessage != null)
    //            {
    //                var playerName = msg.SenderConnection.RemoteHailMessage.ReadString();

    //                ConnectionStatusMessage connectionStatusMessage;
    //                if (_clients.Any(c => c.Name == playerName))
    //                {
    //                    connectionStatusMessage = new ConnectionStatusMessage(timeStamp, 0xff, ConnectionInformation.NoUniqueName);
    //                    connectionStatusMessage.Send(_messageTypeMap, msg.SenderConnection);
    //                }
    //                else
    //                {
    //                    var f = _gameLevel.AddFigure(GetClientId(), new RemoteController());
    //                    f.Name = playerName;
    //                    _clients.Add(new ClientInformation(msg.SenderEndpoint, f));
    //                    connectionStatusMessage = new ConnectionStatusMessage(timeStamp, f.Id, ConnectionInformation.ConnectedWithId);
    //                    connectionStatusMessage.Send(_messageTypeMap, msg.SenderConnection);    
    //                }
    //            }
    //        }
    //        else if (connectionStatus == NetConnectionStatus.Disconnected)
    //        {
    //            var playerToRemove = _clients.FirstOrDefault(p => p.EndPoint == msg.SenderEndpoint);
    //            if (playerToRemove != null)
    //            {
    //                _clients.Remove(playerToRemove);
    //                _gameLevel.RemoveFigure(playerToRemove.Figure);
    //            }

    //            if (_clients.Count == 0) Deactivate();
    //        }
    //    }

    //    private void SendDiscoveryResponse(double timeStamp, NetIncomingMessage serverMessage)
    //    {
    //        var response = _server.CreateMessage();
    //        response.Write(_serverName);
    //        response.Write((byte)_clients.Count);
    //        response.Write(_currentState != GameServerState.Lobby);
    //        response.WritePadBits(7);
    //        _server.SendDiscoveryResponse(response, serverMessage.SenderEndpoint);
    //    }

    //    private void RefreshPlayListOnAllClients(double timeStamp)
    //    {
    //        if (_server.ConnectionsCount == 0) return;

    //        var message = new RefreshClientListMessage(timeStamp, _clients.Select(c => new RefreshClientListMessage.ClientData(c.Id, c.Name, c.Ready)));
    //        message.SendToAll(_messageTypeMap, _server);
    //    }

    //    private IEnumerable<GameDataTransferMessage.MapData> GetMapData()
    //    {
    //        for (var i = 0; i < _gameLevel.Data.Length; i++)
    //        {
    //            var ground = _gameLevel.Data[i];
    //            var fringe = _gameLevel.Fringe[i];

    //            var data = new GameDataTransferMessage.MapData
    //            {
    //                GroundBlockType = ground.Type,
    //                FringeBlockType = fringe.IsActive ? fringe.Type : BlockType.Ground,
    //                FringeItemType = fringe.AttachedItem
    //            };

    //            yield return data;
    //        }
    //    }
        
    //    private void ProvideMatchFinished(double timeStamp, MatchResultType resultType)
    //    {
    //        var msg = new MatchFinishedMessage(timeStamp, _clients, resultType);
    //        msg.SendToAll(_messageTypeMap, _server);
    //        _currentState = GameServerState.MatchResult;
    //        _clients.ForEach(c => c.ResetForNextMatch());
    //    }

    //    private void HandleDataMessages(double timeStamp, NetIncomingMessage serverMessage)
    //    {
    //        var message = Message.Read(_messageTypeMap, serverMessage);

    //        if (message is ClientReadyMessage)
    //            HandleClientReady((ClientReadyMessage)message);
    //        else if (message is InputMessage)
    //            HandleInput((InputMessage) message);
    //    }

    //    private void HandleClientReady(ClientReadyMessage msg)
    //    {
    //        var client = _clients.First(c => c.Id == msg.ClientId);
    //        if (msg.LoadForGameReady)
    //            client.LoadForGameReady = true;
    //        else
    //            client.Ready = msg.IsReady;    
    //    }
    //    private void HandleInput(InputMessage msg)
    //    {
    //        var figure = _clients.Select(c => c.Figure).First(f => f.Id == msg.ClientId);
    //        var controller = (RemoteController)figure.FigureController;

    //        switch(msg.Action)
    //        {
    //            case InputAction.Action:
    //                controller.ActionDone = true;
    //                break;
    //            case InputAction.MoveDirectionChanged:
    //                controller.DirectionChanged = true;
    //                controller.MoveDirection = msg.MoveDirection;
    //                break;
    //            case InputAction.MoveDirectonChangedAndAction:
    //                controller.ActionDone = true;
    //                controller.DirectionChanged = true;
    //                controller.MoveDirection = msg.MoveDirection;
    //                break;
    //        }
    //    }
    //}
}
