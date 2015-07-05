using System.Collections.Generic;
using BombRush.Interfaces;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bombrush.MonoGame.Rendering.Render2D
{
    class Tileset
    {
        public Dictionary<BlockType, BlockTemplate> Templates { get; private set; }
        public Texture2D TileSetTexture { get; private set; }
        public string TileSetTextureName { get; private set; }

        public Tileset(ContentManager content, string tilesetFilePath)
        {
            var tilesetData = TilesetData.ReadFromXml(tilesetFilePath);
            Templates = new Dictionary<BlockType, BlockTemplate>();
            TileSetTextureName = tilesetData.Texture;
            TileSetTexture = content.Load<Texture2D>(TileSetTextureName);

            foreach (TemplateData templateData in tilesetData.Templates)
            {
                Templates[templateData.BlockType] = CreateTemplateFromDataObject(templateData);
            }
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
