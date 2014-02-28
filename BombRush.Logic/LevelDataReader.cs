using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BombRushData;
using System.Xml;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace BombRush.Logic
{
    internal static class LevelDataReader
    {
        private static int ReadIntegerValue(XmlDocument document, string elementName)
        {
            return int.Parse(document.DocumentElement.SelectSingleNode(elementName).InnerText);
        }

        private static Point ReadPointValue(XmlDocument document, string elementName)
        {
            var content = document.DocumentElement.SelectSingleNode(elementName).InnerText.Trim();
            var elements = content.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(elements.Length == 2);

            return new Point(int.Parse(elements[0]), int.Parse(elements[1]));   
        }

        private static List<int> ReadIntegerList(XmlDocument document, string elementName)
        {
            var content = document.DocumentElement.SelectSingleNode(elementName).InnerText.Trim();
            var entries = content.Split(new [] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(entries.Length == 15 * 13);

            return entries.Select(e => int.Parse(e)).ToList();
        }

        public static LevelData ReadLevelFile(string fileName)
        {
            var levelData = new LevelData();
            var document = new XmlDocument();
            document.Load(fileName);

            levelData.ItemBombCount = ReadIntegerValue(document, "ItemBombCount");
            levelData.ItemFireCount = ReadIntegerValue(document, "ItemFireCount");
            levelData.ItemSpeedCount = ReadIntegerValue(document, "ItemSpeedCount");
            levelData.ItemPunishCount = ReadIntegerValue(document, "ItemPunishCount");
            levelData.ItemMaxRangeBombCount = ReadIntegerValue(document, "ItemMaxRangeBombCount");
            levelData.Player1StartupPosition = ReadPointValue(document, "Player1StartupPosition");
            levelData.Player2StartupPosition = ReadPointValue(document, "Player2StartupPosition");
            levelData.Player3StartupPosition = ReadPointValue(document, "Player3StartupPosition");
            levelData.Player4StartupPosition = ReadPointValue(document, "Player4StartupPosition");
            levelData.GroundLayer = ReadIntegerList(document, "GroundLayer");
            levelData.FringeLayer = ReadIntegerList(document, "FringeLayer");

            return levelData;
        }
    }
}
