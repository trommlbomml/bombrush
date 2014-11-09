using System;
using System.Xml;
using Game2DFramework;
using Microsoft.Xna.Framework;

namespace Bombrush.MonoGame.Gui2
{
    class StackPanel : GuiElement
    {
        public StackPanel(Game2D game) : base(game)
        {
        }

        public StackPanel(Game2D game, XmlElement element)
            : base(game, element)
        {
            foreach (XmlElement childElement in element.ChildNodes)
            {
                AddChild(CreateFromXmlType(game, childElement));
            }
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

            size.X -= Margin.Left;
            size.Y -= Margin.Top;
            size.Height += Margin.Vertical;
            size.Width += Margin.Horizontal;

            return size;
        }

        public override void Arrange(Rectangle target)
        {
            Bounds = new Rectangle(target.X + Margin.Left, target.Y + Margin.Top, target.Width - Margin.Horizontal, target.Height - Margin.Vertical);
            
            var startX = Bounds.X;
            var startY = Bounds.Y;

            foreach (var guiElement in Children)
            {
                var minSize = guiElement.GetMinSize();
                var childArrange = Orientation == Orientation.Vertical
                    ? new Rectangle(startX, startY, Bounds.Width, minSize.Height)
                    : new Rectangle(startX, startY, minSize.Width, Bounds.Height);

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
