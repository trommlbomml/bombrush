
using System;
using System.Collections.Generic;
using BombRush.Interfaces;
using BombRush.Logic;
using Microsoft.Xna.Framework;

namespace BombRush.Logic.AI
{
    class PlaceNode : IComparable
    {
        public Point TilePosition;
        public PlaceNode Up;
        public PlaceNode Down;
        public PlaceNode Left;
        public PlaceNode Right;
        public bool IsReachable;
        public bool IsBoxToDestroy;
        public Bomb Bomb;
        public int CostToReach;
        public int F;
        public ItemType ItemType;

        public PlaceNode(Point position)
        {
            TilePosition = position;
            Reset();
        }

        public IEnumerable<PlaceNode> GetReachableNeighbours()
        {
            if (Left.IsReachable) yield return Left;
            if (Right.IsReachable) yield return Right;
            if (Up.IsReachable) yield return Up;
            if (Down.IsReachable) yield return Down;
        }

        public PlaceNode GetLowestCostNeighbour()
        {
            PlaceNode current = null;
            foreach (var placeNode in GetReachableNeighbours())
            {
                if (current == null)
                    current = placeNode;
                else if (placeNode.CostToReach < current.CostToReach)
                        current = placeNode;
            }

            return current;
        }

        public void Reset()
        {
            CostToReach = -1;
            IsReachable = false;
            IsBoxToDestroy = false;
            Bomb = null;
            ItemType = ItemType.Empty;
            F = 0;
        }

        public int CompareTo(object obj)
        {
            var toCompare = (PlaceNode) obj;
            return toCompare.F.CompareTo(F);
        }
    }

}
