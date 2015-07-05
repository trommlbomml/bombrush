
using System;
using System.Collections.Generic;
using System.Xml;
using BombRush.Interfaces;
using Microsoft.Xna.Framework;

namespace Bombrush.MonoGame.Rendering.Render2D
{
    public class TilesetData
    {
        public string Texture;
        public List<TemplateData> Templates;

        public static TilesetData ReadFromXml(string filePath)
        {
            var tilesetData = new TilesetData();
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);

// ReSharper disable PossibleNullReferenceException
            tilesetData.Texture = xmlDocument.DocumentElement.SelectSingleNode("Texture").InnerXml;

            tilesetData.Templates = new List<TemplateData>();

            foreach (XmlElement itemElement in xmlDocument.DocumentElement.SelectNodes("Templates/Item"))
            {
                tilesetData.Templates.Add(new TemplateData
                {
                    BlockType = (BlockType) Enum.Parse(typeof(BlockType), itemElement.SelectSingleNode("BlockType").InnerXml),
                    SourceRectangle = ParseRectangle(itemElement.SelectSingleNode("SourceRectangle").InnerXml)
                });
            }
// ReSharper restore PossibleNullReferenceException

            return tilesetData;
        }

        private static Rectangle ParseRectangle(string text)
        {
            var coords = text.Split(new[] {' '});
            return new Rectangle(int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]), int.Parse(coords[3]));
        }
    }
}
