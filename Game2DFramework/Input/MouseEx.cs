
using Microsoft.Xna.Framework.Input;

namespace Game2DFramework.Input
{
    public class MouseEx
    {
        public float X { get; private set; }
        public float Y { get; private set; }

        public MouseEx()
        {
        }

        public void Update()
        {
            X = Mouse.GetState().X;
            Y = Mouse.GetState().Y;
        }
    }
}
