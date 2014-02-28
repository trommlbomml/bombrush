
using System;
using BombRush.Rendering.Render3D.Framework;
using BombRush.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Rendering.Render3D.GameObjects
{
    class Item3D : IRenderObject
    {
        private readonly IItemDataProvider _item;
        private readonly ModelGeometry _baseGeometry;
        private readonly ModelGeometry _fireGeometry;
        private readonly Matrix _overlayWorld;

        public Item3D(IItemDataProvider item, ModelGeometry baseGeometry, ModelGeometry fireGeometry)
        {
            _item = item;
            _baseGeometry = baseGeometry;
            _fireGeometry = fireGeometry;
            World = Matrix.CreateScale(0.025f) * Matrix.CreateTranslation(_item.TilePosition.X + 0.5f, 0.0f, _item.TilePosition.Y + 0.5f);
            _overlayWorld = Matrix.CreateScale(0.006f) *Matrix.CreateTranslation(_item.TilePosition.X + 0.5f, 1.0f, _item.TilePosition.Y + 0.5f);
        }

        public bool IsActive
        {
            get { return _item.IsActive; }
            set { throw new NotImplementedException("To be removed"); }
        }

        public Matrix World { get; set; }

        public void Render(Render3DContext context, Effect renderEffect, string techniqueName)
        {
            _baseGeometry.Render(World, context, renderEffect, techniqueName);
            _fireGeometry.Render(_overlayWorld, context, renderEffect, techniqueName);
        }
    }
}
