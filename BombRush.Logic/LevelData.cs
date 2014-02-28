
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BombRush.Logic
{
    public class LevelData
    {
        public int ItemBombCount;
        public int ItemFireCount;
        public int ItemSpeedCount;
        public int ItemPunishCount;
        public int ItemMaxRangeBombCount;
        public Point Player1StartupPosition;
        public Point Player2StartupPosition;
        public Point Player3StartupPosition;
        public Point Player4StartupPosition;
        public List<int> GroundLayer;
        public List<int> FringeLayer;
    }
}
