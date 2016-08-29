using System;
using System.IO;
using System.Linq;
using BombRush.Interfaces;
using Game2DFramework.Collision;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;

namespace BombRush.Logic
{
    public static class LevelHelper
    {
        private const string FolderOfLevels = "levels";

        public static string GetAssetLevelName(string localAssetName)
        {
            return Path.Combine(FolderOfLevels, localAssetName);
        }

        public static string[] GetLevelNames()
        {
            var basePath = Path.Combine("Content", FolderOfLevels);
            return Directory.GetFiles(basePath).Where(s => s.EndsWith(".xnb")).Select(Path.GetFileNameWithoutExtension).ToArray();
        }

        public static Vector2 HandleCollisionSlide(Level level, Vector2 position, Vector2 moveStep)
        {
            CollisionInfo collisionInfo;
            if (CollisionDetected(level, position, moveStep, out collisionInfo))
            {
                var targetPosition = position + moveStep;
                var playerCircle = new Circle((int)targetPosition.X, (int)targetPosition.Y, FigureImp.CollisionRadius);
                var targetTilePosition = level.GetTilePositionFromWorld(position);
                var offsetToTileCenter = targetPosition - level.GetWorldFromTilePositionCentered(targetTilePosition);

                var dx = Math.Sign(moveStep.X);
                var offsetDx = Math.Sign(offsetToTileCenter.X);
                if (offsetDx == 0) offsetDx = 1;

                var dy = Math.Sign(moveStep.Y);
                var offsetDy = Math.Sign(offsetToTileCenter.Y);
                if (offsetDy == 0) offsetDy = 1;

                var moveLength = moveStep.Length();

                var diagonalTilePosition = targetTilePosition.Offset(offsetDx, offsetDy);
                var collidesDiagonal = CollisionDetected(level, diagonalTilePosition, playerCircle, out collisionInfo);

                var collidesHorizontal = CollisionDetected(level, targetTilePosition.Offset(dx, 0),
                                                                 playerCircle, out collisionInfo);

                var collidesVertical = CollisionDetected(level, targetTilePosition.Offset(0, dy),
                                                               playerCircle, out collisionInfo);

                if (collidesDiagonal)
                {
                    if (dx != 0 && !collidesHorizontal)
                        position.Y -= offsetDy * moveLength;

                    if (dy != 0 && !collidesVertical)
                        position.X -= offsetDx * moveLength;
                }
                else if (!IsCollidable(level, diagonalTilePosition))
                {
                    if (dx != 0 && collidesHorizontal)
                    {
                        var collidesVerticalOffset = CollisionDetected(level, targetTilePosition.Offset(0, offsetDy),
                                                                             playerCircle, out collisionInfo);

                        if (!collidesVerticalOffset)
                            position.Y += offsetDy * moveLength;
                    }

                    if (dy != 0 && collidesVertical)
                    {
                        var collidesHorizontalOffset = CollisionDetected(level, targetTilePosition.Offset(offsetDx, 0),
                                                                               playerCircle, out collisionInfo);

                        if (!collidesHorizontalOffset)
                            position.X += offsetDx * moveLength;
                    }
                }
            }
            else
            {
                position.X += moveStep.X;
                position.Y += moveStep.Y;
            }

            return position;
        }

        private static bool CollisionDetected(Level level, Vector2 position, Vector2 moveStep, out CollisionInfo info)
        {
            info = new CollisionInfo { CollisionDetected = false };

            Vector2 targetPosition = position + moveStep;
            Circle playerCircle = new Circle((int)targetPosition.X, (int)targetPosition.Y, FigureImp.CollisionRadius);
            Point currentTilePosition = level.GetTilePositionFromWorld(position);
            Vector2 offsetToTileCenter = targetPosition - level.GetWorldFromTilePositionCentered(currentTilePosition);

            var signX = Math.Sign(moveStep.X);
            var signOffsetX = Math.Sign(offsetToTileCenter.X);

            var signY = Math.Sign(moveStep.Y);
            var signOffsetY = Math.Sign(offsetToTileCenter.Y);

            return (signOffsetX != 0 && signY != 0) && CollisionDetected(level, new Point(currentTilePosition.X + signOffsetX, currentTilePosition.Y + signY), playerCircle, out info)
                || (signOffsetY != 0 && signX != 0) && CollisionDetected(level, new Point(currentTilePosition.X + signX, currentTilePosition.Y + signOffsetY), playerCircle, out info)
                || signX != 0 && CollisionDetected(level, new Point(currentTilePosition.X + signX, currentTilePosition.Y), playerCircle, out info)
                || signY != 0 && CollisionDetected(level, new Point(currentTilePosition.X, currentTilePosition.Y + signY), playerCircle, out info);
        }

        private static bool CollisionDetected(Level level, Point tilePosition, Circle playerCircle, out CollisionInfo info)
        {
            var collisionDetected = TileCollisionDetected(level, tilePosition, playerCircle)
                                    || BombCollisionDetected(level, tilePosition, playerCircle);

            info = new CollisionInfo { CollisionDetected = collisionDetected, TilePosition = tilePosition };

            return collisionDetected;
        }

        private static bool TileCollisionDetected(Level level, Point tilePosition, Circle playerCircle)
        {
            var linearIndex = tilePosition.Y * 15 + tilePosition.X;

            var t = level.Data[linearIndex];
            var t2 = level.Fringe[linearIndex];
            return ((t2.Type == BlockType.Box && t2.IsActive) || t.Type == BlockType.Wall) &&
                   playerCircle.Intersects(t.Bounds);
        }

        private static bool BombCollisionDetected(Level level, Point tilePosition, Circle playerCircle)
        {
            var bomb = level.Bombs.FirstOrDefault(b => b.TilePosition == tilePosition);
            return bomb != null && playerCircle.Intersects(bomb.Bounds);
        }

        public static bool IsCollidable(Level level, Point t)
        {
            int linearIndex = t.Y * 15 + t.X;

            if (level.Data[linearIndex].Type != BlockType.Ground)
                return true;

            if (level.Fringe[linearIndex].Type == BlockType.Box && level.Fringe[linearIndex].IsActive)
                return true;

            if (level.Bombs.Any(b => b.TilePosition == t))
                return true;

            return false;
        }
    }
}
