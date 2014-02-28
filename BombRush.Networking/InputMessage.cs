using BombRush.Interfaces;
using Lidgren.Network;
using Microsoft.Xna.Framework;

namespace BombRush.Networking
{
    public enum InputAction
    {
        MoveDirectionChanged,
        Action,
        MoveDirectonChangedAndAction,
    }

    public class InputMessage : ClientMessage
    {
        public InputAction Action { get; private set; }
        public Vector2 MoveDirection { get; private set; }

        public InputMessage()
        {
        }

        public InputMessage(double timeStamp, byte clientId, FigureController controller)
            : base(timeStamp, clientId)
        {

            if (controller.ActionDone && controller.DirectionChanged)
                Action = InputAction.MoveDirectonChangedAndAction;
            else if (controller.ActionDone)
                Action = InputAction.Action;
            else
                Action = InputAction.MoveDirectionChanged;

            MoveDirection = controller.MoveDirection;
        }

        protected override void WriteTo(NetOutgoingMessage outgoingMessage)
        {
            base.WriteTo(outgoingMessage);
            outgoingMessage.Write((byte) Action);
            outgoingMessage.Write(MoveDirection.X);
            outgoingMessage.Write(MoveDirection.Y);
        }

        protected override void ReadFrom(NetIncomingMessage incomingMessage)
        {
            base.ReadFrom(incomingMessage);
            Action = (InputAction) incomingMessage.ReadByte();
            MoveDirection = new Vector2(incomingMessage.ReadFloat(), incomingMessage.ReadFloat());
        }
    }
}
