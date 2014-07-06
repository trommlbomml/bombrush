using System;
using BombRush.Interfaces;

namespace BombRush.Logic
{
    public class GameSessionStartParameters
    {
        public string LevelAssetPath { get; set; }
        public string SessionName { get; set; }
        public int MatchesToWin { get; set; }
        public int MatchTime { get; set; }
        public Func<int, FigureController> ProvidePlayerFigureController { get; set; }
    }
}
