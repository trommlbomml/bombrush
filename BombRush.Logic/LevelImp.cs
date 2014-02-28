using System;
using System.Collections.Generic;
using System.Linq;
using BombRush.Interfaces;
using Game2DFramework.Collision;
using Microsoft.Xna.Framework;
using BombRushData;

namespace BombRush.Logic
{
    public class LevelImp : Level
    {
        public const int GameLevelWidth = 15;
        public const int GameLevelHeight = 13;

        private readonly Random _random;
        private readonly HashSet<byte> _availableIds;
        private readonly List<FigureImp> _figures;
        private Point[] _startupPositions;
        private TileBlockImp[] _fringe;
        private TileBlockImp[] _data;
        private readonly List<Bomb> _bombs;
        private ExplosionFragment[] _explosionOverlayData;
        private Item[] _itemData;
        private readonly int _matchTime;
        private bool _afterCalmFinish;
        private float _waitAfterFinishedTime;
        private Dictionary<ItemType, int> _itemTypeCount;
        private List<int> _fringeData;

        public List<Figure> Figures { get { return _figures.Cast<Figure>().ToList(); } } 
        public Point[] StartupPositions { get { return _startupPositions; } }
        public float RemainingGameTime { get; private set; }
        public List<Bomb> ActualBombs { get { return _bombs; } }

        public LevelImp(int matchTime)
        {
            _matchTime = matchTime;
            _random = new Random();
            _bombs = new List<Bomb>();
            _availableIds = new HashSet<byte>();
            _figures = new List<FigureImp>();
            _waitAfterFinishedTime = 0;
        }

        public void AddFigure(FigureImp figure)
        {
            figure.Position = GetWorldStartupPosition(_figures.Count+1);
            _figures.Add(figure);
        }

        public void GenerateLevel(string filePath)
        {
            var levelData = LevelDataReader.ReadLevelFile(filePath);
            FillDefaultData();
            _data = FillLayer(levelData.GroundLayer, (t, p) => new TileBlockImp(t) { TilePosition = p });

            _itemTypeCount = new Dictionary<ItemType, int>
            {
                {ItemType.AdditionalBomb, levelData.ItemBombCount},
                {ItemType.AdditionalFireRange, levelData.ItemFireCount},
                {ItemType.AdditionalSpeed, levelData.ItemSpeedCount},
                {ItemType.Punish, levelData.ItemPunishCount},
                {ItemType.MaxRangeBomb, levelData.ItemMaxRangeBombCount}
            };

            _fringeData = levelData.FringeLayer;
            GenerateContent();

            _startupPositions = new Point[]
            {
                levelData.Player1StartupPosition,
                levelData.Player2StartupPosition,
                levelData.Player3StartupPosition,
                levelData.Player4StartupPosition
            };
        }

        private void GenerateContent()
        {
            foreach (ItemImp item in _itemData) item.Reset();

            _fringe = FillLayer(_fringeData, (t, p) =>
            {
                var tileBlock = new TileBlockImp(t) {TilePosition = p};
                if (t == BlockType.Ground || (t == BlockType.Box && _random.Next(1, 101) < 20)) tileBlock.IsActive = false;
                return tileBlock;
            });

            foreach (var kvp in _itemTypeCount) PlaceItem(kvp.Value, kvp.Key);
            _availableIds.Clear();
            for (byte i = 0; i < 128; i++) _availableIds.Add(i);
        }

        private void PlaceItem(int count, ItemType type)
        {
            var emptyBlocks = new List<TileBlockImp>(_fringe.Where(t => t.Type == BlockType.Box && t.IsActive && t.AttachedItem == ItemType.Empty));
            if (emptyBlocks.Count == 0) return;

            for (var i = 0; i < count; ++i)
            {
                var index = _random.Next(0, emptyBlocks.Count);
                emptyBlocks[index].AttachedItem = type;
                emptyBlocks.RemoveAt(index);
            }
        }

        private static TileBlockImp[] FillLayer(IEnumerable<int> layerData, Func<BlockType, Point, TileBlockImp> whatToDo)
        {
            var data = new List<TileBlockImp>();
            var current = 0;
            foreach (var token in layerData)
            {
                var type = (BlockType)token;
                var tilePosition = new Point(current % GameLevelWidth, current / GameLevelWidth);
                data.Add(whatToDo(type, tilePosition));
                current++;
            }
            return data.ToArray();
        }

        private void FillDefaultData()
        {
            const int entries = GameLevelWidth * GameLevelHeight;
            _data = new TileBlockImp[entries];
            _fringe = new TileBlockImp[entries];
            _explosionOverlayData = new ExplosionFragment[entries];
            _bombs.Clear();
            _itemData = new Item[entries];
            for (var i = 0; i < entries; ++i)
            {
                var p = new Point(i % GameLevelWidth, i / GameLevelWidth);
                _explosionOverlayData[i] = new ExplosionFragmentImp { TilePosition = p };
                _itemData[i] = new ItemImp { TilePosition = p };
            }

        }

        public Point GetStartupPosition(int index)
        {
            if (index < 1 || index > 4) throw new ArgumentException("Player index is in Rage 1..4");
            return _startupPositions[index - 1];
        }

        public TileBlock GetData(Point t) { return _data[t.Y * GameLevelWidth + t.X]; }
        public TileBlock GetFringe(Point t) { return _fringe[t.Y * GameLevelWidth + t.X]; }
        public Item GetItemData(Point t) { return _itemData[t.Y * GameLevelWidth + t.X]; }
        public ExplosionFragment GetOverlayData(Point t) { return _explosionOverlayData[t.Y * GameLevelWidth + t.X]; }

        public ExplosionFragment GetOverlayInformation(Point t)
        {
            return _explosionOverlayData[t.Y * GameLevelWidth + t.X];
        }

        public List<Bomb> Bombs
        {
            get { return _bombs.Cast<Bomb>().ToList(); }
        }

        public TileBlock[] Data { get { return _data; } }
        public TileBlock[] Fringe { get { return _fringe; } }
        public Item[] ItemData { get { return _itemData; } }
        public ExplosionFragment[] OverlayData { get { return _explosionOverlayData; } }

        public bool CurrentlyActiveExplosionsOrBombs()
        {
            return _bombs.Any(b => b.IsActive) || _explosionOverlayData.Any(e => e.IsActive);
        }

        public Vector2 GetWorldStartupPosition(int index)
        {
            return GetWorldFromTilePositionCentered(GetStartupPosition(index));
        }

        public Vector2 GetWorldFromTilePositionCentered(Point p)
        {
            return new Vector2(p.X * TileBlockImp.Tilesize + TileBlockImp.Tilesize * 0.5f, p.Y * TileBlockImp.Tilesize + TileBlockImp.Tilesize * 0.5f);
        }

        public Vector2 GetWorldFromTilePosition(Point p)
        {
            return new Vector2(p.X * TileBlockImp.Tilesize, p.Y * TileBlockImp.Tilesize);
        }

        public Point GetTilePositionFromWorld(Vector2 position)
        {
            return new Point(
                ((int)position.X) / TileBlockImp.Tilesize,
                ((int)position.Y) / TileBlockImp.Tilesize);
        }

        public bool MayCollectItem(FigureImp figure, out ItemImp item)
        {
            Item possibleItem = GetItemData(GetTilePositionFromWorld(figure.Position));
            if (possibleItem.IsActive)
            {
                item = (ItemImp)possibleItem;
                return true;
            }

            item = null;
            return false;
        }

        private bool CollideHandle(FigureImp figure, Func<Point, Circle, bool> collisionCheck)
        {
            var currentTile = GetTilePositionFromWorld(figure.Position);
            var playerCircle = figure.CircleBounds;

            for (var x = currentTile.X - 1; x <= currentTile.X + 1; x++)
            {
                for (var y = currentTile.Y - 1; y <= currentTile.Y + 1; y++)
                {
                    if (collisionCheck(new Point(x, y), playerCircle)) return true;
                }
            }

            return false;
        }

        public bool HitByFire(FigureImp figure)
        {
            return CollideHandle(figure, (p, c) =>
            {
                var f = (ExplosionFragmentImp)GetOverlayData(p);
                return f.IsActive && c.Intersects(f.Bounds);
            });
        }

        public Vector2 GetOffsetToTileCenter(FigureImp figure, Point tilePosition)
        {
            return figure.Position - GetWorldFromTilePositionCentered(tilePosition);
        }

        public bool IsCollidable(Point t)
        {
            return LevelHelper.IsCollidable(this, t);
        }

        private bool HasBomb(Point p)
        {
            return _bombs.Any(b => b.TilePosition == p);
        }

        private bool TryGetBomb(Point p, out BombImp bomb)
        {
            bomb = (BombImp)_bombs.FirstOrDefault(b => b.TilePosition == p);
            return bomb != null;
        }

        private byte PullId()
        {
            if (_availableIds.Count == 0)
                throw new InvalidOperationException("No more Ids");

            byte id = _availableIds.First();
            _availableIds.Remove(id);

            return id;
        }

        public Bomb PlaceBomb(FigureImp figure)
        {
            var position = GetTilePositionFromWorld(figure.Position);
            if (!HasBomb(position))
            {
                var b = new BombImp(PullId(), figure.ActiveBombType);
                _bombs.Add(b);
                b.Drop(figure, position);
                return b;
            }
            return null;
        }

        private void HandleExplosionTrace(Bomb bomb, Point startPosition, int range, Func<int, Point> directionIncrement)
        {
            for (int i = 1; i <= range; i++)
            {
                var addCoord = directionIncrement(i);
                var currentPosition = new Point(startPosition.X + addCoord.X, startPosition.Y + addCoord.Y);
                var possibleFragment = (ExplosionFragmentImp)GetOverlayData(currentPosition);
                var currentBlockToTest = GetData(currentPosition);
                if (currentBlockToTest.Type == BlockType.Wall) break;

                var currentFringeBlockToTest = GetFringe(currentPosition);
                if (currentFringeBlockToTest.Type == BlockType.Box && currentFringeBlockToTest.IsActive)
                {
                    if(!possibleFragment.IsActive) ExplodeBox(currentPosition);

                    possibleFragment.Activate();
                    if (bomb.BombType != BombType.MaxRange) break;
                }

                BombImp currentPossibleActiveBomb;
                if (TryGetBomb(currentPosition, out currentPossibleActiveBomb))
                {
                    if (currentPossibleActiveBomb.IsActive)
                    {
                        currentPossibleActiveBomb.Explode();
                        RaiseExplosion(currentPossibleActiveBomb);   
                    }
                    if (bomb.BombType != BombType.MaxRange) break;
                }

                var currentPossibleItem = GetItemData(currentPosition);
                if (currentPossibleItem.IsActive) ((ItemImp)currentPossibleItem).Collect();

                possibleFragment.Activate();
            }
        }

        private void RaiseExplosion(BombImp bomb)
        {
            var explosionFragment = (ExplosionFragmentImp)GetOverlayData(bomb.TilePosition);
            if (explosionFragment.IsActive) return;
            
            var range = bomb.ExplosionRange;
            explosionFragment.Activate();
            HandleExplosionTrace(bomb, bomb.TilePosition, range, i => new Point(i, 0));
            HandleExplosionTrace(bomb, bomb.TilePosition, range, i => new Point(-i, 0));
            HandleExplosionTrace(bomb, bomb.TilePosition, range, i => new Point(0, i));
            HandleExplosionTrace(bomb, bomb.TilePosition, range, i => new Point(0, -i));
        }

        private void ExplodeBox(Point tilePosition)
        {
            var block = (TileBlockImp)GetFringe(tilePosition);
            if (block.Type != BlockType.Box) throw new InvalidOperationException("Only Box Type is valid!");

            if (block.IsActive && block.AttachedItem != ItemType.Empty)
            {
                var item = (ItemImp)GetItemData(block.TilePosition);
                item.Drop(block.AttachedItem);
            }

            block.Explode();
        }

        public Vector2 HandleCollissionsSlide(Vector2 position, Vector2 moveStep)
        {
            return LevelHelper.HandleCollisionSlide(this, position, moveStep);
        }

        public void StartMatch()
        {
            _waitAfterFinishedTime = 0;
            _afterCalmFinish = false;
        }

        private readonly List<Bomb> _toRemove = new List<Bomb>();

        public MatchResultType Update(float elapsed)
        {
            if (_itemData == null || _explosionOverlayData == null) return MatchResultType.None;

            RemainingGameTime = GetRemainingMatchTime(elapsed);
            _figures.ForEach(f => ((FigureImp)f).Update(this, elapsed));

            foreach (BombImp bomb in _bombs)
            {
                if (bomb.Update(elapsed) == UpdateResult.Exploding)
                {
                    RaiseExplosion(bomb);
                }
            }
            _toRemove.Clear();
            _toRemove.AddRange(_bombs.Where(b => b.IsActive == false));
            foreach (var bomb in _toRemove)
            {
                _bombs.Remove(bomb);
                _availableIds.Add(bomb.Id);
            }

            foreach (var block in _data) { block.Update(elapsed); }
            foreach (var block in _fringe) { block.Update(elapsed); }
            foreach (ExplosionFragmentImp explosionFragment in _explosionOverlayData) { explosionFragment.Update(elapsed); }

            var aliveFigures = _figures.Count(f => f.IsAlive);

            if (aliveFigures < 2 && !CurrentlyActiveExplosionsOrBombs() && !_afterCalmFinish)
                _afterCalmFinish = true;

            if (_afterCalmFinish)
            {
                _waitAfterFinishedTime += elapsed;
                if (_waitAfterFinishedTime >= 1)
                {
                    _afterCalmFinish = false;
                    var resultType = MatchResultType.Draw;
                    if (aliveFigures == 1)
                    {
                        var winner = _figures.First(f => f.IsAlive);
                        winner.Wins++;
                        winner.IsMatchWinner = true;
                        resultType = MatchResultType.SomeOneWins;
                    }
                    return resultType;
                }
            }
            else if (IsMatchTimeOver()) return MatchResultType.Draw;

            return MatchResultType.None;
        }

        private bool IsMatchTimeOver()
        {
            return !float.IsNaN(_matchTime)
                   && RemainingGameTime <= 0.0f
                   && Figures.Count(c => c.IsAlive) > 1;
        }

        private float GetRemainingMatchTime(float elapsed)
        {
            return _matchTime == 0 ? float.NaN : _matchTime - elapsed;
        }

        private bool IsFreeField(Point p)
        {
            if (IsCollidable(p)) return false;
            if (GetOverlayData(p).IsActive) return false;
            if (GetItemData(p).IsActive) return false;
            return !_figures.Any(f => f.IsAlive && GetTilePositionFromWorld(f.Position) == p);
        }

        private List<Point> GetFreeFields()
        {
            var points = new List<Point>();
            for (var y = 1; y < GameLevelHeight - 2; y++)
            {
                for(var x = 1; x < GameLevelWidth-2; x++)
                {
                    var p = new Point(x,y);
                    if (IsFreeField(p)) points.Add(p);
                }
            }
            return points;
        }

        public void ProvideItemsOf(FigureImp figure)
        {
            if (figure.CollectedItems.Count == 0) return;

            var freeFields = GetFreeFields();
            foreach (var item in figure.CollectedItems)
            {
                DropOfType(item, freeFields);
                if (freeFields.Count == 0) break;
            }
        }

        private void DropOfType(ItemType itemType, List<Point> freeFields)
        {
            var indexToSet = _random.Next(freeFields.Count);
            var toPlace = freeFields[indexToSet];
            ((ItemImp)GetItemData(toPlace)).Drop(itemType);
            freeFields.RemoveAt(indexToSet);
        }
    }
}
