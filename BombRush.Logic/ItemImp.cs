using BombRush.Interfaces;
using Microsoft.Xna.Framework;

namespace BombRush.Logic
{
    public class ItemImp : Item
    {
        public Point TilePosition { get; set; }
        public ItemType Type { get; private set; }
        public bool IsActive { get; private set; }

        public ItemImp()
        {
            Type = ItemType.Empty;
        }

        public void Reset()
        {
            Type = ItemType.Empty;
            IsActive = false;
        }

        public void Drop(ItemType type)
        {
            IsActive = true;
            Type = type;
        }

        public void Collect()
        {
            IsActive = false;
            Type = ItemType.Empty;
        }
    }
}
