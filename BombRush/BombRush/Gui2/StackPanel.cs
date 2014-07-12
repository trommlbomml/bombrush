using System;
using System.Collections.Generic;
using Game2DFramework;
using Game2DFramework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BombRush.Gui2
{
    enum Orientation
    {
        Vertical,
        Horizontal,
    }

    class StackPanel : GuiElement
    {
        public StackPanel(Game2D game) : base(game)
        {
            Children = new List<GuiElement>();
        }

        public Orientation Orientation { get; set; }

        public override Rectangle GetMinSize()
        {
            var size = new Rectangle();

            foreach (var guiElement in Children)
            {
                var sizeOfChildElement = guiElement.GetMinSize();

                if (Orientation == Orientation.Vertical)
                {
                    size.Width = Math.Max(size.Width, sizeOfChildElement.Width);
                    size.Height += sizeOfChildElement.Height;
                }
                else
                {
                    size.Height = Math.Max(size.Height, sizeOfChildElement.Height);
                    size.Width += sizeOfChildElement.Width;
                }
            }

            return size;
        }

        public override void Arrange(Rectangle target)
        {
            Bounds = target;

            var startX = target.X;
            var startY = target.Y;

            foreach (var guiElement in Children)
            {
                var minSize = guiElement.GetMinSize();
                var childArrange = Orientation == Orientation.Vertical
                    ? new Rectangle(startX, startY, target.Width, minSize.Height)
                    : new Rectangle(startX, startY, minSize.Width, target.Height);

                if (Orientation == Orientation.Vertical)
                {
                    startY += minSize.Height;
                }
                else
                {
                    startX += minSize.Width;
                }

                guiElement.Arrange(childArrange);    
            }
        }

        public void AddChild(GuiElement child)
        {
            Children.Add(child);
        }

        public override void Draw()
        {
            Children.ForEach(c => c.Draw());
        }

        public override void Update(float elapsedTime)
        {
            foreach (var guiElement in Children)
            {
                guiElement.Update(elapsedTime);
            }
        }
    }
}
