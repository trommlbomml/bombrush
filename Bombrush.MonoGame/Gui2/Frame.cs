using System;
using System.Xml;
using Game2DFramework;
using Game2DFramework.Extensions;
using Microsoft.Xna.Framework;

namespace Bombrush.MonoGame.Gui2
{
    class Frame : GuiElement
    {
        private readonly Border2 _border;
        private readonly Border2 _header;

        public string Title { get; set; }

        public Frame(Game2D game, XmlElement element)
            : base(game, element)
        {
            _border = new Border2(game);
            _header = new Border2(game);

            if (element.HasAttribute("Title"))
            {
                Title = element.GetAttribute("Title");
            }

            if (element.HasChildNodes) SetContent(CreateFromXmlType(game, (XmlElement)element.FirstChild));
        }

        public Frame(Game2D game) : base(game)
        {
            _border = new Border2(game);
            _header = new Border2(game);
        }

        public void SetContent(GuiElement guiElement)
        {
            if (Children.Count == 1) Children.Clear();
            Children.Add(guiElement);
        }

        public override Rectangle GetMinSize()
        {
            if (Children.Count == 0)
            {
                if (string.IsNullOrEmpty(Title)) return _border.MinSize;

                var headerSize = GetHeaderSize();
                var size = _border.MinSize;
                size.Width = Math.Max(size.Width, headerSize.Width);
                size.Height = Math.Max(size.Height, headerSize.Height);
                return size;
            }

            var childMinSize = Children[0].GetMinSize();

            var minSize = new Rectangle(0, 0, childMinSize.Width + _border.MinSize.Width, childMinSize.Height);
            if (!string.IsNullOrEmpty(Title))
            {
                var headerSize = GetHeaderSize();
                minSize.Width = Math.Max(minSize.Width, headerSize.Width + 2*Border2.BorderSize);
                minSize.Height = headerSize.Height + childMinSize.Height + Border2.BorderSize;
            }
            else
            {
                minSize.Height += _border.MinSize.Height;
            }

            return minSize;
        }

        private Rectangle GetHeaderSize()
        {
            if (string.IsNullOrEmpty(Title)) return Rectangle.Empty;

            var textSize = Resources.BigFont.MeasureString(Title).ToPoint();
            return new Rectangle(0,0, textSize.X + _header.MinSize.Width, textSize.Y + _header.MinSize.Height);
        }

        public override void Arrange(Rectangle target)
        {
            _border.Bounds = target;

            var headerSize = GetHeaderSize();
            
            if (!string.IsNullOrEmpty(Title))
            {
                _header.Bounds = new Rectangle(target.X + target.Width / 2 - headerSize.Width / 2, target.Y, headerSize.Width, headerSize.Height);
                var bounds = _border.Bounds;
                bounds.Y += headerSize.Height/2;
                bounds.Height -= headerSize.Height/2;
                _border.Bounds = bounds;
            }
            
            if (Children.Count == 1)
            {
                var rectangle = target;
                rectangle.X += Border2.BorderSize;
                rectangle.Y += string.IsNullOrEmpty(Title) ? Border2.BorderSize : headerSize.Height;
                rectangle.Width -= 2 * Border2.BorderSize;
                rectangle.Height -= string.IsNullOrEmpty(Title) ? 2 * Border2.BorderSize : Border2.BorderSize + headerSize.Height;
                Children[0].Arrange(rectangle);
            }
        }

        public override void Draw()
        {
            _border.Draw(Color.White);
            if (!string.IsNullOrEmpty(Title))
            {
                _header.Draw(Color.White);
                Game.SpriteBatch.DrawString(Resources.BigFont, Title, new Vector2(_header.Bounds.X + Border2.BorderSize,_header.Bounds.Y + Border2.BorderSize), Color.White);
            }

            if (Children.Count == 1)
            {
                var child = Children[0];
                child.Draw();
            }
        }

        public override void Update(float elapsedTime)
        {
            if (Children.Count == 1) Children[0].Update(elapsedTime);
        }
    }
}

