﻿using BombRush.Interfaces;
using Microsoft.Xna.Framework;

namespace BombRush.Logic
{
    public class BombImp : Bomb
    {
        public const float BurnTimeSeconds = 3;

        private FigureImp _dropByFigure;

        public byte Id { get; }
        public Point TilePosition { get; set; }
        public bool IsActive { get; private set; }
        public float CurrentBurnTime { get; private set; }
        public float BurnTime { get; }
        public BombType BombType { get; }

        public int ExplosionRange => BombType == BombType.MaxRange ? 15 : _dropByFigure.BombExplosionRange;

        public BombImp(byte id, BombType bombType)
        {
            BurnTime = BurnTimeSeconds;
            Id = id;
            BombType = bombType;
        }

        public Rectangle Bounds => new Rectangle(TilePosition.X * TileBlockImp.Tilesize + 2, 
                                                 TilePosition.Y * TileBlockImp.Tilesize + 2, 
                                                 TileBlockImp.Tilesize - 4, 
                                                 TileBlockImp.Tilesize - 4);

        public void Drop(FigureImp dropByFigure,Point tilePosition)
        {
            TilePosition = tilePosition;
            _dropByFigure = dropByFigure;
            IsActive = true;
            CurrentBurnTime = 0;
        }

        public void Explode()
        {
            if (_dropByFigure != null)
                _dropByFigure.CurrentPlacableBombCount++;
            IsActive = false;
        }

        public UpdateResult Update(float elapsed)
        {
            if (IsActive)
            {
                CurrentBurnTime += elapsed;
                if (CurrentBurnTime >= BurnTime)
                {
                    Explode();
                    return UpdateResult.Exploding;
                }
            }

            return UpdateResult.Nothing;
        }
    }
}
