
using System;
using System.Linq;
using System.Collections.Generic;
using BombRush.Interfaces;
using BombRush.Logic;
using Game2DFramework.Drawing;
using Microsoft.Xna.Framework;
using BombRush.Logic.AI.Tasking;

namespace BombRush.Logic.AI
{
    class ComFigureController : FigureController
    {
        private const float RefreshNodeMapTime = 1.0f;

        private float _refreshTimeElapsed;
        private readonly Queue<ITask> _tasks;
        private ITask _currentTask;

        public Figure Figure { get; set; }
        public NodeMap NodeMap { get; private set; }
        public LevelImp Level { get; private set; }
        public bool IsComputer { get { return true; } }
        public bool DirectionChanged { get; private set; }
        public Vector2 MoveDirection { get; private set; }
        public bool CancelDone { get; private set; }
        public bool ActionDone { get; set; }

        public ComFigureController(LevelImp level)
        {
            Level = level;
            NodeMap = new NodeMap();
            _tasks = new Queue<ITask>();
        }

        public void Update(float elapsed)
        {
            if (_refreshTimeElapsed >= RefreshNodeMapTime) RefreshNodeMap();
            if (_currentTask == null || _currentTask.Finished) _currentTask = GetNextTask();
            if (_currentTask != null) _currentTask.Update(elapsed, this);
        }

        private ITask GetNextTask()
        {
            RefreshNodeMap(true);
            if (_tasks.Count == 0) ExamineNewTasks();
            if (_tasks.Count == 0) return null;
            var task = _tasks.Dequeue();
            task.Start();
            return task;
        }

        private Dictionary<Point, PlaceNode> GetSaveNodes()
        {
            var pointsToAvoid = new HashSet<Point>();
            NodeMap.BombNodes.ForEach(p => FillToAvoidWithExplosionRadius(pointsToAvoid, p));
            return NodeMap.ReachableNodes.Where(n => !pointsToAvoid.Contains(n.Value.TilePosition)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private void FillToAvoidWithExplosionRadius(HashSet<Point> pointsToAvoid, PlaceNode bombNode)
        {
            var currentUp = bombNode;
            var currentDown = bombNode;
            var currentLeft = bombNode;
            var currentRight = bombNode;
            pointsToAvoid.Add(bombNode.TilePosition);
            for (var i = 0; i < ((BombImp)bombNode.Bomb).ExplosionRange; i++)
            {
                currentLeft = AddDirective(pointsToAvoid, currentLeft, p2 => p2.Left);
                currentRight = AddDirective(pointsToAvoid, currentRight, p2 => p2.Right);
                currentUp = AddDirective(pointsToAvoid, currentUp, p2 => p2.Up);
                currentDown = AddDirective(pointsToAvoid, currentDown, p2 => p2.Down);
            }
        }

        private PlaceNode AddDirective(HashSet<Point> pointsToAvoid, PlaceNode node, Func<PlaceNode, PlaceNode> navi)
        {
            if (node == null) return null;

            var placeNode = navi(node);
            if (!placeNode.IsReachable) return null;

            pointsToAvoid.Add(placeNode.TilePosition);
            return placeNode;
        }

        private void ExamineNewTasks()
        {
            var current = GetTilePosition();
            var saveNodes = GetSaveNodes();
            var currentActiveBombsOrExplosions = Level.CurrentlyActiveExplosionsOrBombs();

            if (!saveNodes.ContainsKey(current))
            {
                var toReachSave = GetMinCostNode(saveNodes);
                _tasks.Enqueue(new WayPointTask(toReachSave));
            }
            else if (NodeMap.ItemNodes.Count > 0
                     && !currentActiveBombsOrExplosions)
            {
                var itemNode = GetMinCostNode(NodeMap.ItemNodes);
                _tasks.Enqueue(new WayPointTask(itemNode));
            }
            else if (((FigureImp)Figure).CurrentPlacableBombCount > 0
                && NodeMap.ReachableBoxNodes.Count > 0
                && !currentActiveBombsOrExplosions)
            {
                var nearestBoxNode = GetMinCostNode(NodeMap.ReachableBoxNodes);
                _tasks.Enqueue(new WayPointTask(nearestBoxNode));
                _tasks.Enqueue(new PlaceBombTask());
            }
        }

        private static PlaceNode GetMinCostNode(IEnumerable<PlaceNode> nodes)
        {
            PlaceNode min = null;
            foreach (var placeNode in nodes)
            {
                if (min == null)
                    min = placeNode;
                else if (min.CostToReach > placeNode.CostToReach)
                {
                    min = placeNode;
                }
            }
            return min;
        }

        private static PlaceNode GetMinCostNode(IDictionary<Point, PlaceNode> dictionary)
        {
            return GetMinCostNode(dictionary.Values);
        }

        private void RefreshNodeMap(bool resetTime = false)
        {
            NodeMap.ScanFields(Level, Level.GetTilePositionFromWorld(Figure.Position));
            if (resetTime)
                _refreshTimeElapsed = 0;
            else
                _refreshTimeElapsed -= RefreshNodeMapTime;
        }

        public void SetMoveDirectionTo(Vector2 direction)
        {
            DirectionChanged = false;
            if (MoveDirection != direction)
            {
                MoveDirection = direction;
                DirectionChanged = true;
            }
        }

        public Point GetTilePosition()
        {
            return Level.GetTilePositionFromWorld(Figure.Position);
        }

        public void Reset()
        {
            ActionDone = false;
            CancelDone = false;
        }

        public void ResetInputs()
        {
            ActionDone = false;
            CancelDone = false;
            DirectionChanged = false;
            MoveDirection = Vector2.Zero;
        }

        public void DebugDraw(ShapeRenderer shapeRenderer, Vector2 offset)
        {
            NodeMap.DebugDraw(shapeRenderer, Level, offset);
        }

    }
}
