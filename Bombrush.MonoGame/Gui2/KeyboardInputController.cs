using System;
using Game2DFramework;
using Game2DFramework.Gui2;
using Microsoft.Xna.Framework.Input;

namespace Bombrush.MonoGame.Gui2
{
    class KeyboardInputController : OverlayInputController
    {
        private readonly Game2D _game;

        public KeyboardInputController(Game2D game)
        {
            _game = game;
        }

        public void Update(UiFocusContainer container)
        {
            if(_game.Keyboard.IsKeyDownOnce(Keys.W))
            {
                MoveToPreviousElement?.Invoke();
            }
            else if (_game.Keyboard.IsKeyDownOnce(Keys.S))
            {
                MoveToNextElement?.Invoke();
            }

            if(_game.Keyboard.IsKeyDownOnce(Keys.Space) || _game.Keyboard.IsKeyDownOnce(Keys.Enter))
            {
                OnAction?.Invoke(container.CurrentElement);
            }
        }

        public event Action<UiElement> OnAction;
        public event Action MoveToNextElement;
        public event Action MoveToPreviousElement;
        public event Action<UiElement> MoveToElement;
    }
}
