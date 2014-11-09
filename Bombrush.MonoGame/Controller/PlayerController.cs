using System;
using BombRush.Interfaces;
using Bombrush.MonoGame.Input;
using Game2DFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bombrush.MonoGame.Controller
{
    class PlayerController : FigureController
    {
        private const int NetPlayerIndex = 255;

        private readonly IInputDevice _device;

        private static Keys[] GetKeysFromPlayerIndex(Game2D game, int index)
        {
            switch (index)
            {
                case NetPlayerIndex: return GetKeysFromGameProperty(game, BombGame.PlayerNetConfigPropertyName);
                case 1: return GetKeysFromGameProperty(game, BombGame.Player1ConfigPropertyName);
                case 2: return GetKeysFromGameProperty(game, BombGame.Player2ConfigPropertyName);
                case 3: return GetKeysFromGameProperty(game, BombGame.Player3ConfigPropertyName);
                case 4: return GetKeysFromGameProperty(game, BombGame.Player4ConfigPropertyName);
                default: throw new InvalidOperationException();
            }
        }

        private static Keys[] GetKeysFromGameProperty(Game2D game, string propertyName)
        {
            var data = game.GetPropertyStringOrDefault(propertyName);
            var k = new Keys[5];
            var i = 0;
            foreach (var s in data.Split(new[] { ',' }))
            {
                k[i++] = (Keys)Enum.Parse(typeof(Keys), s);
            }
            return k;
        }

        public static FigureController CreateNet(Game2D game, InputDeviceType inputDeviceType)
        {
            return Create(game, inputDeviceType, NetPlayerIndex);
        }

        public static FigureController Create(Game2D game, InputDeviceType inputDeviceType, int playerIndex)
        {
            IInputDevice inputDevice;
            switch (inputDeviceType)
            {
                case InputDeviceType.Keyboard:
                    var keys = GetKeysFromPlayerIndex(game, playerIndex);
                    inputDevice = KeyboardInputDevice.CreateKeyboardInputDevice(game, keys[0], keys[1], keys[2], keys[3], keys[4], Keys.Escape);
                    break;
                case InputDeviceType.Gamepad:
                    inputDevice = new GamePadInputDevice(game, IntToPlayerIndex(playerIndex));
                    break;
                default:
                    throw new InvalidOperationException("Invalid Input Device Type");
            }

            return new PlayerController(inputDevice);
        }

        private static PlayerIndex IntToPlayerIndex(int index)
        {
            switch (index)
            {
                case 1: return PlayerIndex.One;
                case 2: return PlayerIndex.Two;
                case 3: return PlayerIndex.Three;
                case 4: return PlayerIndex.Four;
            }
            throw new InvalidOperationException("Invalid Player Index");
        }

        public PlayerController(IInputDevice device)
        {
            _device = device;
        }

        private bool IsDown(InputKey key)
        {
            return _device.IsDown(key);
        }

        private bool IsDownOnce(InputKey key)
        {
            return _device.IsDownOnce(key);
        }

        public Vector2 MoveDirection { get; private set; }
        public Figure Figure { get; set; }
        public bool IsComputer { get { return false; } }
        public bool DirectionChanged { get; private set; }
        public bool ActionDone { get; private set; }
        public bool CancelDone { get; private set; }

        public void Update(float elapsed)
        {
            ActionDone = IsDownOnce(InputKey.Action);

            var newDirection = Vector2.Zero;

            if (IsDown(InputKey.MoveUp))
                newDirection = -Vector2.UnitY;
            else if (IsDown(InputKey.MoveDown))
                newDirection = Vector2.UnitY;
            else if (IsDown(InputKey.MoveLeft))
                newDirection = -Vector2.UnitX;
            else if (IsDown(InputKey.MoveRight))
                newDirection = Vector2.UnitX;

            DirectionChanged = false;
            if (MoveDirection != newDirection)
            {
                MoveDirection = newDirection;
                DirectionChanged = true;
            }
        }

        public void Reset()
        {
            CancelDone = false;
        }

        public void ResetInputs()
        {

        }
    }
}
