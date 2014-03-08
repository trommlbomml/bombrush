
using Lidgren.Network;

namespace BombRush.Networking.ClientMessages
{
    public class ClientJoinGameSessionMessage : ClientMessageBase
    {
        public string UserName { get; private set; }
        public byte SessionId { get; private set; }

        protected override void ReadFrom(NetIncomingMessage incomingMessage)
        {
            base.ReadFrom(incomingMessage);
            UserName = incomingMessage.ReadString();
            SessionId = incomingMessage.ReadByte();
        }

        protected override void WriteTo(NetOutgoingMessage outgoingMessage)
        {
            base.WriteTo(outgoingMessage);
            outgoingMessage.Write(UserName);
            outgoingMessage.Write(SessionId);
        }
    }
}
