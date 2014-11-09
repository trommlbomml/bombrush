
using System.Xml;
using Game2DFramework;

namespace BombRush.Gui2
{
    static class GuiSystem
    {
        public static TGuiElement CreateGuiHierarchyFromXml<TGuiElement>(Game2D game, string xmlFile) where TGuiElement : GuiElement
        {
            var document = new XmlDocument();
            document.Load(xmlFile);

            return (TGuiElement)GuiElement.CreateFromXmlType(game, document.DocumentElement);
        }

        public static void ArrangeCenteredToScreen(Game2D game, GuiElement guiElement)
        {
            var rect = guiElement.GetMinSize();
            rect.X = game.ScreenWidth / 2 - rect.Width / 2;
            rect.Y = game.ScreenHeight / 2 - rect.Height / 2;
            guiElement.Arrange(rect);
        }
    }
}
