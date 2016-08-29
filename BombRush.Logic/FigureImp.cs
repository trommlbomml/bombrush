using System;
using System.Collections.Generic;
using BombRush.Interfaces;
using Microsoft.Xna.Framework;
using Game2DFramework.Collision;

namespace BombRush.Logic
{
    public class FigureImp : Figure
    {
        private readonly Random _random;
        private PunishedType _punishedType;
        private float _punishedTime;

        private const float BaseSpeed = 100;
        private const float PunishedDuration = 10;
        private const int FigureBoundSize = 24;
        public const float SpeedIncrease = 10;
        public const int CollisionRadius = FigureBoundSize/2;
        public bool CanPlaceBombs { get; private set; }
        public bool ShowPlayerName { get; set; }
        public bool IsMatchWinner { get; set; }
        public BombType ActiveBombType { get; private set; }

        public List<ItemType> CollectedItems { get; private set; }

        private bool _animating;
        public FigureDirection Direction { get; private set; }

        private Vector2 _walkDirection;
        public Vector2 WalkDirection
        {
            get { return _walkDirection; }
            set
            {
                _walkDirection = value;

                if (_walkDirection.X < 0)
                    Direction = FigureDirection.Left;
                else if (_walkDirection.X > 0)
                    Direction = FigureDirection.Right;
                else if (_walkDirection.Y < 0)
                    Direction = FigureDirection.Up;
                else if (_walkDirection.Y > 0)
                    Direction = FigureDirection.Down;
            }
        }

        public void ProhibitPlaceBombs()
        {
            CanPlaceBombs = false;
        }

        private void Die(Vector2 position)
        {
            Position = position;
            WalkDirection = Vector2.Zero;
            IsAlive = false;
        }

        public bool IsVisible => _punishedType != PunishedType.Invisible;
        public Vector2 Position { get; set; }

        private string _name;
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name)) _name = $"Player {Id}";
                return _name;
            } 
            set { _name = value; }
        }

        public byte Id { get; }
        public int Wins { get; set; }

        private float _speed;
        public float Speed
        {
            get
            {
                if (_punishedType == PunishedType.SlowMotion)
                    return BaseSpeed*0.6f;
                return _speed;
            }
            set { _speed = value; }
        }
        
        
        public int BombExplosionRange { get; set; }
        public int CurrentPlacableBombCount { get; set; }
        public int PlacableBombCount { get; private set; }

        public FigureImp(byte id)
        {
            Id = id;
            _random = new Random();
            CollectedItems = new List<ItemType>();
            Direction = FigureDirection.Down;
            CanPlaceBombs = true;
            ShowPlayerName = true;
            ActiveBombType = BombType.Normal;
            ReSpawn();
        }

        public FigureController FigureController { get; set; }

        public bool IsAlive { get; private set; }

        public void ReSpawn()
        {
            IsAlive = true;
            Speed = BaseSpeed;
            BombExplosionRange = 1;
            PlacableBombCount = 1;
            CurrentPlacableBombCount = PlacableBombCount;
            WalkDirection = Vector2.Zero;
            Direction = FigureDirection.Down;
            CollectedItems.Clear();
            ActiveBombType = BombType.Normal;
            FigureController?.ResetInputs();
        }
        
        public Rectangle Bounds => new Rectangle((int)Position.X - FigureBoundSize / 2, (int)Position.Y - FigureBoundSize / 2, FigureBoundSize, FigureBoundSize);

        public Circle CircleBounds => GetCircleBoundsFor(Position);

        public Circle GetCircleBoundsFor(Vector2 position)
        {
            return new Circle((int)position.X, (int)position.Y, CollisionRadius);
        }

        public void Update(LevelImp level, float elapsed)
        {
            if (!IsAlive)
                return;

            if (_punishedType != PunishedType.None)
            {
                _punishedTime += elapsed;
                if (_punishedTime >= PunishedDuration)
                    _punishedType = PunishedType.None;
            }

            FigureController?.Update(elapsed);

            Move(level, elapsed);
            HandleItemCollection(level);
            HandleHitByFire(level);
            HandlePlaceBomb(level);

            FigureController?.Reset();
        }

        protected bool PlaceBomb(LevelImp level)
        {
            if (CurrentPlacableBombCount > 0)
            {
                var placedBomb = level.PlaceBomb(this);
                if (placedBomb != null)
                {
                    CurrentPlacableBombCount--;
                    return true;
                }
            }

            return false;
        }

        private void HandlePlaceBomb(LevelImp level)
        {
            if (_punishedType == PunishedType.AutoDrop || (FigureController != null && FigureController.ActionDone))
            {
                PlaceBomb(level);
            }
        }

        private void HandleHitByFire(LevelImp level)
        {
            if (level.HitByFire(this))
            {
                Die(Position);
                level.ProvideItemsOf(this);
            }
        }

        private void Move(LevelImp level, float elapsed)
        {

            //todo: wtf?
            //float elapsed = Math.Abs(TimeStamp - 0) > 0.001
            //                    ? (float) (timeStamp - TimeStamp)
            //                    : 0;

            var tile = (TileBlockImp)level.GetData(level.GetTilePositionFromWorld(Position));

            Vector2 actualMoveDirection = _punishedType == PunishedType.InvertControls
                                              ? -FigureController.MoveDirection
                                              : FigureController.MoveDirection;

            if (FigureController != null && WalkDirection != actualMoveDirection)
            {
                WalkDirection = actualMoveDirection;
            }

            if (WalkDirection == Vector2.Zero && tile.Force == Vector2.Zero)
            {
                _animating = false;
                //TimeStamp = timeStamp;
                return;
            }

            if (!_animating)
            {
                _animating = true;
            }
            
            if(WalkDirection == Vector2.Zero)
            {
                _animating = false;
            }

            float moveDistance = Speed * elapsed;

            Vector2 moveStep = WalkDirection*moveDistance;

            if (moveStep == Vector2.Zero)
            {
                moveStep = tile.Force * moveDistance;
            }
            else
            {
                if (tile.Force != Vector2.Zero)
                {
                    if (tile.Force == WalkDirection) 
                        moveStep *= 2;
                    else if (Math.Abs(tile.Force.X - WalkDirection.X) < 0.01f || Math.Abs(tile.Force.Y - WalkDirection.Y) < 0.01f)
                        moveStep *= 0.5f;
                    else
                        moveStep = (WalkDirection + (tile.Force/2)) * moveDistance;
                }
            }

            Position = level.HandleCollissionsSlide(Position, moveStep);
            //TimeStamp = timeStamp;
        }

        private void HandleItemCollection(LevelImp level)
        {
            ItemImp item;
            if (!level.MayCollectItem(this, out item)) return;
            switch (item.Type)
            {
                case ItemType.AdditionalBomb:
                    PlacableBombCount++;
                    CurrentPlacableBombCount++;
                    break;
                case ItemType.AdditionalFireRange:
                    BombExplosionRange++;
                    break;
                case ItemType.AdditionalSpeed:
                    Speed += SpeedIncrease;
                    break;
                case ItemType.Punish:
                    HandleCollectionPunish();
                    break;
                case ItemType.MaxRangeBomb:
                    ActiveBombType = BombType.MaxRange;
                    break;
            }
            CollectedItems.Add(item.Type);
            item.Collect();
        }

        private void HandleCollectionPunish()
        {
            _punishedType = (PunishedType) _random.Next(1, 7);
            _punishedTime = 0;
        }
    }
}
