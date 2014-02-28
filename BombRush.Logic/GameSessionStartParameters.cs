using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BombRush.Interfaces;

namespace BombRush.Logic
{
    public class GameSessionStartParameters
    {
        public byte Id { get; set; }
        public string LevelAssetPath { get; set; }
        public string SessionName { get; set; }
        public int MatchesToWin { get; set; }
        public int MatchTime { get; set; }
        public Func<int, FigureController> ProvidePlayerFigureController { get; set; }
    }
}
