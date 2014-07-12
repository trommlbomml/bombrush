using Microsoft.Xna.Framework.Input;

namespace Game2DFramework.Input
{
    public class MouseEx
    {
        private MouseState _currentState;
        private MouseState _lastState;

        public float X { get; private set; }
        public float Y { get; private set; }

        public MouseEx()
        {
            _currentState = Mouse.GetState();
        }

        public void Update()
        {
            _lastState = _currentState;
            _currentState = Mouse.GetState();
            X = _currentState.X;
            Y = _currentState.Y;
        }

        public bool IsLeftButtonDownOnce
        {
            get { return _lastState.LeftButton == ButtonState.Released && _currentState.LeftButton == ButtonState.Pressed; }
        }
    }
}
