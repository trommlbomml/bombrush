using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BombRush.Interfaces;
using Game2DFramework;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace BombRush.Network
{
    class LevelClient : Level
    {
        private const float RefreshInterval = 0.5f;

        private float _elapsedRefreshTimeout;
        private double _serverRefreshTimeStamp;
        private GameUpdateResult _updateResult;
        private readonly IFigureController _figureController;
        private readonly List<BombClient> _bombs;

        public bool IsAdministrator { get; private set; }

        public LevelClient(Game2D game)
            : base(game)
        {
        }

        public GameUpdateResult Update(float elapsed)
        {
            _updateResult = GameUpdateResult.None;
            
            return _updateResult;
        }

        public void OnReadyForNextMatch()
        {
            //var message = new ClientReadyMessage(NetTime.Now, ClientId, true, false);
            //message.Send(_messageTypeMap, _netClient.ServerConnection);
            //State = InGameState.PreparingMatchWaitForAllReady;
        }

        private void RefreshMemberList(RefreshClientListMessage msg)
        {
            _members.Clear();
            foreach (RefreshClientListMessage.ClientData client in msg.Clients)
            {
                _members.Add(new GameMember(client.Name, client.ClientId, client.IsReady) { Me = client.ClientId == ClientId });
            }
            _members.Sort((a, b) => a.Id - b.Id);
        }

        private void HandleGameDataTransfer(GameDataTransferMessage msg, double timeStamp)
        {
            const int entries = BombGame.GameLevelWidth * BombGame.GameLevelHeight;

            TilesetAssetName = msg.TileSetAssetName;
            _data = new TileBlockClient[entries];
            _fringe = new TileBlockClient[entries];
            _itemData = new ItemClient[entries];
            _overlayData = new OverlayClient[entries];
            _bombs.Clear();

            for (var i = 0; i < entries; i++)
            {
                Point tilePosition = new Point(i%BombGame.GameLevelWidth, i/BombGame.GameLevelWidth);
                BlockType groundBlockType = msg.Data[i].GroundBlockType;

                _data[i] = new TileBlockClient(groundBlockType) { TilePosition = tilePosition, IsActive = true };

                BlockType fringeBlockType = msg.Data[i].FringeBlockType;
                ItemType fringeItemType = msg.Data[i].FringeItemType;
                _fringe[i] = new TileBlockClient(fringeBlockType)
                {
                    AttachedItem = fringeBlockType == BlockType.Box ? fringeItemType : ItemType.Empty,
                    IsActive = fringeBlockType == BlockType.Box,
                    TilePosition = tilePosition
                };

                _itemData[i] = new ItemClient {IsActive = false, TilePosition = tilePosition};
                _overlayData[i] = new OverlayClient {IsActive = false, TilePosition = tilePosition};
                _bombs.Clear();
            }

            foreach (GameMember gameMember in _members)
            {
                gameMember.InitFromServer(msg.StartUpPositions[gameMember.Id - 1]);
                gameMember.WalkDirection = Vector2.Zero;
            }

            State = InGameState.PreparingMatchLoadData;
            ClientReadyMessage message = new ClientReadyMessage(timeStamp, ClientId, true, true);
            message.Send(_messageTypeMap, _netClient.ServerConnection);
        }

        private void HandleMatchedFinishedMessage(MatchFinishedMessage msg)
        {
            foreach (MatchPlayerInformation p in msg.Players)
            {
                GameMember member = _members.First(c => c.Id == p.Id);
                member.Wins = p.Wins;
                member.IsMatchWinner = p.IsMatchWinner;
            }
            CurrentMatchResultType = msg.ResultType;
            _updateResult = GameUpdateResult.MatchFinished;
            State = InGameState.MatchResult;
        }

        public string TilesetAssetName { get; private set; }

        public List<IBombDataProvider> Bombs
        {
            get { return _bombs.Cast<IBombDataProvider>().ToList(); }
        }

        private TileBlockClient[] _data;
        public ITileBlockDataProvider[] Data { get { return _data; } }

        private TileBlockClient[] _fringe;
        public ITileBlockDataProvider[] Fringe { get { return _fringe; } }

        private ItemClient[] _itemData;
        public IItemDataProvider[] ItemData { get { return _itemData; } }

        private OverlayClient[] _overlayData;
        public IExplosionFragmentDataProvider[] OverlayData { get { return _overlayData; } }

        public IExplosionFragmentDataProvider GetOverlayInformation(Point t)
        {
            return _overlayData[t.Y * BombGame.GameLevelWidth + t.X];
        }

        public Vector2 GetWorldFromTilePositionCentered(Point p)
        {
            return new Vector2(p.X * BombGame.Tilesize + BombGame.Tilesize * 0.5f, p.Y * BombGame.Tilesize + BombGame.Tilesize * 0.5f);
        }

        public Vector2 GetWorldFromTilePosition(Point p)
        {
            return new Vector2(p.X * BombGame.Tilesize, p.Y * BombGame.Tilesize);
        }

        public Point GetTilePositionFromWorld(Vector2 position)
        {
            return new Point(
                ((int)position.X) / BombGame.Tilesize,
                ((int)position.Y) / BombGame.Tilesize);
        }
























        public float RemainingGameTime { get; private set; }
        public List<Figure> Figures { get; private set; }
        public List<Bomb> Bombs { get; private set; }
        public TileBlock[] Data { get; private set; }
        public TileBlock[] Fringe { get; private set; }
        public Item[] ItemData { get; private set; }
        public ExplosionFragment[] OverlayData { get; private set; }
        public ExplosionFragment GetOverlayInformation(Point t)
        {
            throw new NotImplementedException();
        }

        public Vector2 GetWorldFromTilePositionCentered(Point p)
        {
            throw new NotImplementedException();
        }

        public Vector2 GetWorldFromTilePosition(Point p)
        {
            throw new NotImplementedException();
        }

        public Point GetTilePositionFromWorld(Vector2 position)
        {
            throw new NotImplementedException();
        }
    }
}
