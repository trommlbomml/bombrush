using System.Collections.Generic;
using Game2DFramework;
using Game2DFramework.Input;
using Microsoft.Xna.Framework;

namespace BombRush.Input
{
    class GamePadInputDevice : GameObject, IInputDevice
    {
        private readonly Dictionary<InputKey, GamePadButton> _keyMapping;
        private readonly PlayerIndex _playerIndex;

        public GamePadInputDevice(Game2D game, PlayerIndex playerIndex) : base(game)
        {
            _playerIndex = playerIndex;
            _keyMapping = new Dictionary<InputKey, GamePadButton>();
            _keyMapping[InputKey.Action] = GamePadButton.A;
            _keyMapping[InputKey.Back] = GamePadButton.B;
            _keyMapping[InputKey.MoveUp] = GamePadButton.DigitalUp;
            _keyMapping[InputKey.MoveDown] = GamePadButton.DigitalDown;
            _keyMapping[InputKey.MoveLeft] = GamePadButton.DigitalLeft;
            _keyMapping[InputKey.MoveRight] = GamePadButton.DigitalRight;
        }

        public bool IsDown(InputKey inputKey)
        {
            return Game.GamePad.IsDown(_playerIndex, _keyMapping[inputKey]);
        }

        public bool IsDownOnce(InputKey inputKey)
        {
            return Game.GamePad.IsDownOnce(_playerIndex, _keyMapping[inputKey]);
        }
    }
}
