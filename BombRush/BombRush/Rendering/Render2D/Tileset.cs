using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using BombRushData;
using BombRush.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render2D
{
    struct BlockTemplate
    {
        public bool IsAnimated;
        public float AnimationTime;
        public Rectangle[] Rectangles;
    }

    class Tileset
    {
        public Dictionary<BlockType, BlockTemplate> Templates { get; private set; }
        public Texture2D TileSetTexture { get; private set; }
        public string TileSetTextureName { get; private set; }

        public Tileset(ContentManager content, string tilesetAssetName, Dictionary<BlockType, BlockTemplate> templates)
        {
            TileSetTextureName = tilesetAssetName;
            TileSetTexture = content.Load<Texture2D>(TileSetTextureName);
            Templates = templates;
        }

        public Tileset(ContentManager content, string tilesetAssetName)
        {
            TilesetData tilesetData = content.Load<TilesetData>(tilesetAssetName);

            Templates = new Dictionary<BlockType, BlockTemplate>();
            TileSetTextureName = tilesetData.Texture;
            TileSetTexture = content.Load<Texture2D>(TileSetTextureName);

            foreach (TemplateData templateData in tilesetData.Templates)
            {
                BlockType type = (BlockType)Enum.Parse(typeof(BlockType), templateData.BlockType);
                BlockTemplate blockTemplate = CreateTemplateFromDataObject(templateData);
                Templates[type] = blockTemplate;
            }
        }

        private static Rectangle ParseRectangle(string tuple)
        {
            string[] t = tuple.Split(';');
            return new Rectangle(int.Parse(t[0]), int.Parse(t[1]), int.Parse(t[2]), int.Parse(t[3]));
        }

        private static Rectangle[] ParseRectangles(string attribute)
        {
            return attribute.Split('|').Select(ParseRectangle).ToArray();
        }

        private static BlockTemplate CreateTemplateFromDataObject(TemplateData templateData)
        {
            return new BlockTemplate
            {
                IsAnimated = false,
                AnimationTime = 0,
                Rectangles = new[] {templateData.SourceRectangle}
            };

            
            //todo: animations
            //if (element.HasAttribute("SourceRectangles"))
            //{
            //    return new BlockTemplate
            //    {
            //        IsAnimated = true,
            //        AnimationTime = float.Parse(element.GetAttribute("AnimationTime")),
            //        Rectangles = ParseRectangles(element.GetAttribute("SourceRectangles")),
            //    };
            //}

            //throw new InvalidOperationException("Invalid Template Node");
        }
    }
}
