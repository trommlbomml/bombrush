using System;
using Game2DFramework;
using Game2DFramework.Gui2;

namespace Bombrush.MonoGame.Gui2
{
    class MouseInputController : OverlayInputController
    {
        private readonly Game2D _game;

        public MouseInputController(Game2D game)
        {
            _game = game;
        }

        public void Update(UiFocusContainer container)
        {
            foreach(var uiElement in container.UiElements)
            {
                if(uiElement.GetBounds().Contains(_game.Mouse.X, _game.Mouse.Y))
                {
                    if(container.CurrentElement == uiElement)
                    {
                        if (_game.Mouse.IsLeftButtonDownOnce()) OnAction?.Invoke(uiElement);
                    }
                    else
                    {
                        MoveToElement?.Invoke(uiElement);
                    }
                    break;
                }
            }
        }

        public event Action<UiElement> OnAction;
        public event Action MoveToNextElement;
        public event Action MoveToPreviousElement;
        public event Action<UiElement> MoveToElement;
    }
}
