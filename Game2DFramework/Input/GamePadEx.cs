using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game2DFramework.Input
{
    public enum GamePadButton
    {
        A,B,X,Y,Back,Start,
        LeftShoulder,LeftStick,
        RightShoulder,RightStick,
        Bigbutton,
        DigitalLeft,
        DigitalRight,
        DigitalUp,
        DigitalDown,
    }

    public class GamePadEx
    {
        private class GamePadInstance
        {
            public Dictionary<GamePadButton, bool> CurrentState = new Dictionary<GamePadButton, bool>();
            public Dictionary<GamePadButton, bool> LastState = new Dictionary<GamePadButton, bool>();
        }

        private readonly Dictionary<PlayerIndex, GamePadInstance> _gamePads;

        public GamePadEx()
        {
            _gamePads = new Dictionary<PlayerIndex, GamePadInstance>
            {
                {PlayerIndex.One, new GamePadInstance()},
                {PlayerIndex.Two, new GamePadInstance()},
                {PlayerIndex.Three, new GamePadInstance()},
                {PlayerIndex.Four, new GamePadInstance()}
            };

            foreach (var kvp in _gamePads)
            {
                UpdateState(ref kvp.Value.CurrentState);
            }
        }

        private void UpdateState(ref Dictionary<GamePadButton, bool> dataToUpdate)
        {
            var state = GamePad.GetState(PlayerIndex.One);
            dataToUpdate[GamePadButton.A] = state.Buttons.A == ButtonState.Pressed;
            dataToUpdate[GamePadButton.B] = state.Buttons.B == ButtonState.Pressed;
            dataToUpdate[GamePadButton.X] = state.Buttons.X == ButtonState.Pressed;
            dataToUpdate[GamePadButton.Y] = state.Buttons.Y == ButtonState.Pressed;
            dataToUpdate[GamePadButton.Back] = state.Buttons.Back == ButtonState.Pressed;
            dataToUpdate[GamePadButton.Start] = state.Buttons.Start == ButtonState.Pressed;
            dataToUpdate[GamePadButton.LeftShoulder] = state.Buttons.LeftShoulder == ButtonState.Pressed;
            dataToUpdate[GamePadButton.LeftStick] = state.Buttons.LeftStick == ButtonState.Pressed;
            dataToUpdate[GamePadButton.RightShoulder] = state.Buttons.RightShoulder == ButtonState.Pressed;
            dataToUpdate[GamePadButton.RightStick] = state.Buttons.RightStick == ButtonState.Pressed;
            dataToUpdate[GamePadButton.Bigbutton] = state.Buttons.BigButton == ButtonState.Pressed;
            dataToUpdate[GamePadButton.DigitalUp] = state.DPad.Up == ButtonState.Pressed;
            dataToUpdate[GamePadButton.DigitalDown] = state.DPad.Down == ButtonState.Pressed;
            dataToUpdate[GamePadButton.DigitalLeft] = state.DPad.Left == ButtonState.Pressed;
            dataToUpdate[GamePadButton.DigitalRight] = state.DPad.Right == ButtonState.Pressed;
        }
        
        public bool IsDownOnce(PlayerIndex index, GamePadButton button)
        {
            return _gamePads[index].CurrentState[button] && !_gamePads[index].LastState[button];
        }

        public bool IsDown(PlayerIndex index, GamePadButton button)
        {
            return _gamePads[index].CurrentState[button];
        }

        public void Update()
        {
            foreach (var kvp in _gamePads)
            {
                kvp.Value.LastState = new Dictionary<GamePadButton, bool>(kvp.Value.CurrentState);
                UpdateState(ref kvp.Value.CurrentState);
            }
        }
    }
}
