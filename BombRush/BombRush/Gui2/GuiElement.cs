using System.Collections.Generic;
using Game2DFramework;
using Microsoft.Xna.Framework;

namespace BombRush.Gui2
{
    abstract class GuiElement : GameObject
    {
        protected GuiElement(Game2D game) : base(game)
        {

        }

        public List<GuiElement> Children { get; protected set; }
        public Rectangle Bounds { get; protected set; }

        public abstract Rectangle GetMinSize();

        public abstract void Arrange(Rectangle target);

        public abstract void Draw();

        public abstract void Update(float elapsedTime);
    }
}
