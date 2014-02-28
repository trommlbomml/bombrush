
using System;
using System.Collections.Generic;
using System.Linq;
using BombRush.Interfaces;
using BombRush.Logic;
using Game2DFramework;
using Game2DFramework.Drawing;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;

namespace BombRush.Logic.AI
{
    class NodeMap
    {
        private readonly Dictionary<Point, PlaceNode> _nodesByTilePosition;
        public Dictionary<Point, PlaceNode> ReachableNodes { get; private set; }
        public List<PlaceNode> ReachableBoxNodes { get; private set; }
        public List<PlaceNode> BombNodes { get; private set; }
        public List<PlaceNode> ItemNodes { get; private set; } 

        public NodeMap()
        {
            _nodesByTilePosition = new Dictionary<Point, PlaceNode>();
            ReachableNodes = new Dictionary<Point, PlaceNode>();
            ReachableBoxNodes = new List<PlaceNode>();
            BombNodes = new List<PlaceNode>();
            ItemNodes = new List<PlaceNode>();

            CreatePlaceNodeObjects();
            ConnectPlaceNodeObjects();
        }

        private void CreatePlaceNodeObjects()
        {
            //todo: refactor magic numbers
            for (var x = 0; x < 15; x++)
            {
                for (var y = 0; y < 13; y++)
                {
                    var node = new PlaceNode(new Point(x, y));
                    _nodesByTilePosition.Add(node.TilePosition, node);
                }
            }
        }

        private void ConnectPlaceNodeObjects()
        {
            //todo: refactor magic numbers
            for (var x = 0; x < 15; x++)
            {
                for (var y = 0; y < 13; y++)
                {
                    var current = new Point(x, y);
                    var currentNode = _nodesByTilePosition[current];
                    currentNode.Left = x > 0 ? _nodesByTilePosition[current.Left()] : null;
                    currentNode.Right = x < 15 - 1 ? _nodesByTilePosition[current.Right()] : null;
                    currentNode.Up = y > 0 ? _nodesByTilePosition[current.Up()] : null;
                    currentNode.Down = y < 13 - 1 ? _nodesByTilePosition[current.Down()] : null;
                }
            }
        }

        private void Clear()
        {
            foreach (var keyValuePair in _nodesByTilePosition) keyValuePair.Value.Reset();
            ReachableNodes.Clear();
            BombNodes.Clear();
            ItemNodes.Clear();
        }

        public void ScanFields(LevelImp level, Point current)
        {
            Clear();
            ScanRecursive(level, _nodesByTilePosition[current], 0);
        }

        private void ScanRecursive(LevelImp level, PlaceNode currentNode, int depth)
        {
            if (currentNode.IsReachable)
                return;

            if (level.IsCollidable(currentNode.TilePosition))
            {
                var b = level.GetFringe(currentNode.TilePosition);
                if (b.Type == BlockType.Box && b.IsActive)
                {
                    currentNode.IsBoxToDestroy = true;
                    ReachableBoxNodes.Add(currentNode);
                    return;
                }

                currentNode.Bomb = level.ActualBombs.FirstOrDefault(bb => bb.TilePosition == currentNode.TilePosition);
                if (currentNode.Bomb != null) BombNodes.Add(currentNode);
                if (depth > 0) return;
            }

            var item = level.GetItemData(currentNode.TilePosition);
            if (item.IsActive && item.Type != ItemType.Empty)
            {
                currentNode.ItemType = item.Type;
                ItemNodes.Add(currentNode);
            }

            currentNode.IsReachable = true;
            currentNode.CostToReach = depth;
            ReachableNodes.Add(currentNode.TilePosition, currentNode);
            ScanRecursive(level, currentNode.Left, depth + 1);
            ScanRecursive(level, currentNode.Right, depth + 1);
            ScanRecursive(level, currentNode.Up, depth + 1);
            ScanRecursive(level, currentNode.Down, depth + 1);
        }

        public List<PlaceNode> FindWayTo(Point start, PlaceNode end)
        {
            var openList = new PriorityQueue();
            var closeList = new List<PlaceNode>();

            var startNode = _nodesByTilePosition[start];
            startNode.F = 0;
            openList.Enqueue(startNode);

            while(openList.Count > 0)
            {
                var currentNode = (PlaceNode)openList.Dequeue();
                if (currentNode.TilePosition == end.TilePosition)
                {
                    closeList.Add(currentNode);
                    break;
                }
                
                ExpandNode(currentNode, end, openList, closeList);   
                closeList.Add(currentNode);
            }

            return closeList;
        }

        private static void ExpandNode(PlaceNode currentNode, PlaceNode end, PriorityQueue openList, List<PlaceNode> closeList)
        {
            foreach (var successor in currentNode.GetReachableNeighbours())
            {
                if (closeList.Contains(successor)) continue;

                var tentativeG = successor.CostToReach + 1;

                if (openList.Contains(successor) && tentativeG >= successor.CostToReach) continue;
                if (openList.Contains(successor)) openList.Remove(successor);

                successor.CostToReach = tentativeG;
                successor.F = tentativeG + CalculateHeuristic(successor.TilePosition, end.TilePosition);
                openList.Enqueue(successor);
            }
        }

        private static int CalculateHeuristic(Point from, Point to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        public void DebugDraw(ShapeRenderer shapeRenderer, LevelImp level, Vector2 offset)
        {
            foreach (var placeNode in _nodesByTilePosition)
            {
                if (!placeNode.Value.IsReachable && !placeNode.Value.IsBoxToDestroy) continue;
                var start = level.GetWorldFromTilePosition(placeNode.Value.TilePosition) + offset;
                shapeRenderer.DrawFilledRectangle((int)start.X, (int)start.Y, 32, 32, (placeNode.Value.IsBoxToDestroy ? Color.Orange : Color.Red) * 0.5f);
            }
        }
    }
}
