using System.Collections.Generic;
using Game2DFramework;
using Microsoft.Xna.Framework.Input;

namespace Bombrush.MonoGame.Input
{
    class KeyboardInputDevice : GameObject, IInputDevice
    {
        private readonly Dictionary<InputKey, Keys> _keyMapping;

        public static IInputDevice CreateKeyboardInputDevice(Game2D game, Keys left, Keys right, Keys up, Keys down, Keys action, Keys cancel)
        {
            var keyboardInputDevice = new KeyboardInputDevice(game);
            keyboardInputDevice._keyMapping[InputKey.Action] = action;
            keyboardInputDevice._keyMapping[InputKey.Back] = cancel;
            keyboardInputDevice._keyMapping[InputKey.MoveUp] = up;
            keyboardInputDevice._keyMapping[InputKey.MoveDown] = down;
            keyboardInputDevice._keyMapping[InputKey.MoveLeft] = left;
            keyboardInputDevice._keyMapping[InputKey.MoveRight] = right;
            return keyboardInputDevice;
        }

        public KeyboardInputDevice(Game2D game) : base(game)
        {
            _keyMapping = new Dictionary<InputKey, Keys>();
        }

        public void RegisterInputKey(InputKey key, Keys value)
        {
            _keyMapping[key] = value;
        }

        public bool IsDown(InputKey inputKey)
        {
            return Game.Keyboard.IsKeyDown(_keyMapping[inputKey]);
        }

        public bool IsDownOnce(InputKey inputKey)
        {
            return Game.Keyboard.IsKeyDownOnce(_keyMapping[inputKey]);
        }
    }
}
