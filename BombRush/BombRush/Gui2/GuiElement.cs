using System;
using System.Collections.Generic;
using System.Xml;
using Game2DFramework;
using Microsoft.Xna.Framework;

namespace BombRush.Gui2
{
    abstract class GuiElement : GameObject
    {
        public static GuiElement CreateFromXmlType(Game2D game, XmlElement element)
        {
            switch (element.LocalName)
            {
                case "Frame": return new Frame(game, element);
                case "Button": return new Button(game, element);
                case "StackPanel": return new StackPanel(game, element);
            }

            throw new ArgumentException("Invalid Element Type", "element");
        }

        protected GuiElement(Game2D game) : base(game)
        {
            Children = new List<GuiElement>();
        }

        protected GuiElement(Game2D game, XmlElement element)
            : base(game)
        {
            Children = new List<GuiElement>();

            if (element.HasAttribute("Margin")) Margin = Margin.Parse(element.GetAttribute("Margin"));
            if (element.HasAttribute("Id")) Id = element.GetAttribute("Id");
        }

        public TGuiElement FindGuiElementById<TGuiElement>(string id) where TGuiElement : GuiElement
        {
            if (Id == id) return (TGuiElement)this;
            foreach (var child in Children)
            {
                var element = child.FindGuiElementById<TGuiElement>(id);
                if (element != null) return element;
            }

            return null;
        }

        public string Id { get; private set; }
        public List<GuiElement> Children { get; private set; }
        public Rectangle Bounds { get; protected set; }
        public Margin Margin { get; set; }

        public abstract Rectangle GetMinSize();

        public abstract void Arrange(Rectangle target);

        public abstract void Draw();

        public abstract void Update(float elapsedTime);
    }
}
