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

        private static Keys[] GetKeysFromPlayerIndex(int index)
        {
            switch (index)
            {
                //todo: Should be implented Data driven.
                case NetPlayerIndex: return GetKeys(string.Format("{0},{1},{2},{3},{4}", Keys.A, Keys.D, Keys.W, Keys.S, Keys.LeftShift));
                case 2: return GetKeys(string.Format("{0},{1},{2},{3},{4}", Keys.J, Keys.L, Keys.I, Keys.K, Keys.H));
                case 3: return GetKeys(string.Format("{0},{1},{2},{3},{4}", Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Enter));
                case 4: return GetKeys(string.Format("{0},{1},{2},{3},{4}", Keys.NumPad1, Keys.NumPad3, Keys.NumPad5, Keys.NumPad2, Keys.NumPad0));
                default: return GetKeys(string.Format("{0},{1},{2},{3},{4}", Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Space));
            }
        }

        private static Keys[] GetKeys(string data)
        {
            var k = new Keys[5];
            var i = 0;
            foreach (string s in data.Split(new[] { ',' }))
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
                    var keys = GetKeysFromPlayerIndex(playerIndex);
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
